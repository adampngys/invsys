using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversityApp
{
    public class DisbursementDAO
    {
        /// <summary>
        /// Find the disbursement list of the department
        /// </summary>
        /// <param name="depId">department id</param>
        /// <returns></returns>
        public static List<DisbursementViewModel> GetDisbursement(string depId)
        {
            var myModel = DbFactory.Instance.context.Inventories.Select(x => new DisbursementViewModel()
            {
                itemNo = x.itemNo,
                description = x.description,
                unitMeasure = x.unitMeasure,
                departmentId = depId
            }).ToList();

            var reqs = DbFactory.Instance.context.Requests.Where(x => x.status == "approved" && x.departmentId == depId).ToList();

            foreach (var req in reqs)
            {
                if (req.RequestDetails != null)
                {
                    foreach (var rd in req.RequestDetails)
                    {
                        if (rd.status == "preparing")
                        {
                            var m = myModel.Where(x => x.itemNo == rd.itemNo).First();
                            m.qtyRequired += rd.quantityPacked;
                            m.qtyActual = m.qtyRequired;
                        }
                    }
                }
            }

            myModel = myModel.Where(x => x.qtyActual != 0).ToList();

            return myModel == null ? new List<DisbursementViewModel>() : myModel;
        }

        /// <summary>
        /// Find the disbursement information of the target item of target department
        /// </summary>
        /// <param name="depId">department id</param>
        /// <param name="itemNo">item number</param>
        /// <returns></returns>
        private static DisbursementViewModel getDisbursementItem(string depId, string itemNo)
        {
            var disbursementItem = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == itemNo).Select(x => new DisbursementViewModel()
            {
                itemNo = x.itemNo,
                description = x.description,
                unitMeasure = x.unitMeasure
            }).First();

            var reqs = DbFactory.Instance.context.Requests.Include("RequestDetails").Where(x => x.status == "approved" && x.departmentId == depId).ToList();

            foreach (var req in reqs)
            {
                if (req.RequestDetails != null)
                {
                    foreach (var rd in req.RequestDetails)
                    {
                        if (rd.status == "preparing" && rd.itemNo == itemNo)
                        {
                            disbursementItem.qtyRequired += rd.quantityPacked;
                            disbursementItem.qtyActual = disbursementItem.qtyRequired;
                        }
                    }
                }
            }

            return disbursementItem;
        }

        /// <summary>
        /// Store clerk confirm the disbursement transaction
        /// </summary>
        /// <param name="model">the list of disbursement information</param>
        /// <returns></returns>
        public static bool ConfirmDisbursement(List<DisbursementViewModel> model)
        {
            if (model != null && model.Count > 0)
            {
                using (var dbContextTransaction = DbFactory.Instance.context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var disb in model)
                        {
                            DisbursementDAO.ConfirmDisbursementItem(
                                disb.departmentId,
                                disb.itemNo,
                                disb.qtyActual,
                                disb.qtyDamaged,
                                disb.qtyMissing);
                        }

                        UpdateDepReqStatus(model[0].departmentId);

                        // commit the transaction
                        dbContextTransaction.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Update the status of department request after confirm the disbursement transaction
        /// </summary>
        /// <param name="depId">department id</param>
        private static void UpdateDepReqStatus(string depId)
        {
            var reqs = DbFactory.Instance.context.Requests.Where(x => x.departmentId == depId && x.status == "approved").ToList();

            foreach (var req in reqs)
            {
                bool bPass = true;
                if (req.RequestDetails != null)
                {
                    foreach (var rd in req.RequestDetails)
                    {
                        if (rd.status != "fulfilled")
                        {
                            bPass = false;
                            break;
                        }
                    }
                }
                if (bPass)
                {
                    req.status = "finish";
                    DbFactory.Instance.context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Sub function to confirm the disbursement transaction
        /// </summary>
        /// <param name="depId">department id</param>
        /// <param name="itemNo">item number</param>
        /// <param name="qtyActual">the actual quantity hand over to the department rep</param>
        /// <param name="qtyDamaged">the damage quantity during delivery</param>
        /// <param name="qtyMissing">the missing quantity during delivery</param>
        private static void ConfirmDisbursementItem(string depId, string itemNo, int qtyActual, int qtyDamaged, int qtyMissing)
        {
            #region Validation
            if (qtyActual < 0 || qtyDamaged < 0 || qtyMissing < 0)
            {
                throw new Exception("qty error");
            }

            var disbursementItem = getDisbursementItem(depId, itemNo);

            int total = qtyActual + qtyDamaged + qtyMissing;
            if (total != disbursementItem.qtyRequired)
            {
                throw new Exception("qty mismatch");
            }
            #endregion

            #region update request record here
            var reqs = DbFactory.Instance.context.Requests.Include("RequestDetails").Where(x => x.departmentId == depId && x.status == "approved").ToList();
            for (int i = 0; i < reqs.Count; i++)
            {
                if (reqs[i].RequestDetails != null)
                {
                    var tempDetails = reqs[i].RequestDetails.ToList();

                    for (int j = 0; j < tempDetails.Count; j++)
                    {
                        if (tempDetails[j].status == "preparing" && tempDetails[j].itemNo == itemNo)
                        {
                            if (qtyActual >= tempDetails[j].quantityPacked)
                            {
                                // record the left qty
                                qtyActual -= tempDetails[j].quantityPacked;

                                tempDetails[j].quantityReceive += tempDetails[j].quantityPacked;
                                if (tempDetails[j].quantityReceive == tempDetails[j].quantityNeed)
                                {
                                    tempDetails[j].status = "fulfilled";
                                }
                                else
                                {
                                    tempDetails[j].status = "unfulfilled";
                                }
                                tempDetails[j].quantityPacked = 0;
                            }
                            else if (qtyActual > 0)
                            {
                                tempDetails[j].quantityReceive += qtyActual;
                                tempDetails[j].status = "unfulfilled";
                                tempDetails[j].quantityPacked = 0;

                                // record the left qty
                                qtyActual = 0;
                            }
                            else
                            {
                                tempDetails[j].status = "unfulfilled";
                                tempDetails[j].quantityPacked = 0;
                            }
                            DbFactory.Instance.context.SaveChanges();
                        }
                    }
                }
            }
            #endregion

            #region stock card, adjustment voucher here
            int? tempValue = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == itemNo).Select(x => x.balance).First();
            int currentBalance = tempValue == null ? 0 : (int)tempValue;

            //string depName = DbFactory.Instance.context.Departments.Where(x => x.departmentId == depId).Select(x => x.departmentName).First();

            int compensateQty = qtyDamaged + qtyMissing;
            if (compensateQty != 0)
            {
                // update the current balance
                currentBalance = currentBalance + compensateQty;

                // goes to stock card record
                var sc = new StockCard()
                {
                    itemNo = itemNo,
                    remark = depId,
                    quantity = compensateQty,
                    balance = currentBalance,
                    dateModified = DateTime.Now.Date
                };
                DbFactory.Instance.context.StockCards.Add(sc);
                DbFactory.Instance.context.SaveChanges();
            }

            if (qtyDamaged != 0)
            {
                // update the current balance
                currentBalance = currentBalance - qtyDamaged;

                CreateAdjustmentVoucherInCollectionPoint(
                    itemNo,
                    -qtyDamaged,
                    currentBalance,
                    "damaged at collection point");
            }

            if (qtyMissing != 0)
            {
                // update the current balance
                currentBalance = currentBalance - qtyMissing;

                CreateAdjustmentVoucherInCollectionPoint(
                    itemNo,
                    -qtyMissing,
                    currentBalance,
                    "missing at collection point");
            }
            #endregion
        }

        /// <summary>
        /// Sub function to confirm the disbursement transaction when there occur item damaged or missing
        /// </summary>
        /// <param name="itemNo"></param>
        /// <param name="qtyChange"></param>
        /// <param name="currentBalance"></param>
        /// <param name="remark"></param>
        private static void CreateAdjustmentVoucherInCollectionPoint(string itemNo, int qtyChange, int currentBalance, string remark = "")
        {
            var adjVoucher = new AdjustmentVoucher()
            {
                itemNo = itemNo,
                quantity = qtyChange,
                remark = remark,
                date = DateTime.Now.Date
            };
            DbFactory.Instance.context.AdjustmentVouchers.Add(adjVoucher);
            DbFactory.Instance.context.SaveChanges();

            var sc = new StockCard()
            {
                itemNo = itemNo,
                remark = string.Format("Adjustment Voucher: {0}", adjVoucher.voucherId),
                quantity = qtyChange,
                balance = currentBalance,
                dateModified = DateTime.Now.Date
            };
            DbFactory.Instance.context.StockCards.Add(sc);
            DbFactory.Instance.context.SaveChanges();
        }

    }
}