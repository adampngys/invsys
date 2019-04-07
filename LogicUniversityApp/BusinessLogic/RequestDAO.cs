using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LogicUniversityApp
{
    public class RequestDAO
    {
        /// <summary>
        /// Find all the pending request of this staff
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        public static List<RequestViewModel> GetPendingRequestsInDepartment(Staff staff)
        {
            var dep = DbFactory.Instance.context.Departments.Where(x => x.departmentId == staff.departmentId).FirstOrDefault();
            var result = DbFactory.Instance.context.Requests.Where(x => x.departmentId == dep.departmentId && x.status.ToUpper() == "PENDING" && x.staffId != staff.staffId).ToList();

            return ConvertRequestListToRequestViewModel(result);
        }

        /// <summary>
        /// Find all the approved, rejected, and finished request of this staff
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        public static List<RequestViewModel> GetHistoryRequestsInDepartment(Staff staff)
        {
            var dep = DbFactory.Instance.context.Departments.Where(x => x.departmentId == staff.departmentId).FirstOrDefault();
            var result = DbFactory.Instance.context.Requests.Where(x => x.departmentId == dep.departmentId && x.status.ToUpper() != "PENDING" && x.staffId != staff.staffId).ToList();

            return ConvertRequestListToRequestViewModel(result);
        }

        /// <summary>
        /// Approves the request
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public static Boolean ApproveRequest(int? requestId, string remarks)
        {
            try
            {
                Request request = DbFactory.Instance.context.Requests.Where(x => x.requestId == requestId && x.status == "pending").First();
                request.approvedDate = DateTime.Now.Date;
                request.status = "approved";
                request.remark = remarks;
                DbFactory.Instance.context.SaveChanges();

                // send the emial when DH approve the request (default clerk)
                string mailSubject = "Notification for Retrieval";
                string mailContent = string.Format("Store clerk: please retrieve goods");

                bool result = MyEmail.SendEmail("sa47team1@gmail.com", mailSubject, mailContent);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Reject the request
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public static Boolean RejectRequest(int? requestId, string remarks)
        {
            try
            {
                Request request = DbFactory.Instance.context.Requests.Where(x => x.requestId == requestId && x.status == "pending").First();
                request.approvedDate = DateTime.Now.Date;
                request.status = "rejected";
                request.remark = remarks;
                DbFactory.Instance.context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// A request view model parser
        /// </summary>
        /// <param name="requestList"></param>
        /// <returns></returns>
        private static List<RequestViewModel> ConvertRequestListToRequestViewModel(List<Request> requestList)
        {
            if (requestList == null)
            {
                return null;
            }

            List<RequestViewModel> result = new List<RequestViewModel>();
            foreach (Request request in requestList)
            {
                RequestViewModel requestViewModel = new RequestViewModel()
                {
                    requestId = request.requestId,
                    staffId = request.staffId,
                    departmentId = request.departmentId,
                    approvedDate = request.approvedDate,
                    status_request = request.status,
                    staffName = request.Staff.staffName,
                    remark = request.remark
                };

                StringBuilder sb = new StringBuilder();
                decimal totalAmount = 0;

                if (request.RequestDetails != null)
                {
                    var myTempList = request.RequestDetails.ToList();
                    for (int i = 0; i < myTempList.Count; i++)
                    {
                        String strItemNoTemp = myTempList[i].itemNo;
                        var itemTemp = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == strItemNoTemp).FirstOrDefault();
                        if (itemTemp == null)
                        {
                            break;
                        }
                        sb.Append(itemTemp.description);
                        sb.Append("$");

                        decimal tempPrice = itemTemp.stdPrice * myTempList[i].quantityNeed;
                        totalAmount += tempPrice;
                    }
                }

                requestViewModel.description = sb == null ? "" : sb.ToString();
                requestViewModel.stdPrice = totalAmount;

                result.Add(requestViewModel);
            }
            return result;
        }

        /// <summary>
        /// Allocate the stationery to each department according to the alphabet after the clerk retrieve goods
        /// </summary>
        /// <param name="itemNo">item number</param>
        /// <param name="DueDate">due date to retrieval goods</param>
        /// <param name="qtyRetrieval">retrieval quantity</param>
        /// <param name="qtyDamaged">damaged quantity</param>
        /// <param name="qtyMissing">missing quantity</param>
        /// <returns></returns>
        public static bool AllocateGoods(string itemNo, DateTime DueDate, int qtyRetrieval, int qtyDamaged, int qtyMissing)
        {
            #region validation
            // Validation here (qty info)
            if (qtyRetrieval <= 0 || qtyDamaged < 0 || qtyMissing < 0)
            {
                return false;
            }

            // Fetch the product info from database
            int? _balance = DbFactory.Instance.context.Inventories.AsNoTracking().Where(x => x.itemNo == itemNo).
                Select(x => x.balance).First();
            int balance = _balance == null ? 0 : (int)_balance;

            RetrievalViewModel model = ViewRetrievalGood(itemNo, DueDate);

            // when the balance can and cannot cover the requirement
            if (balance < (model.quantityTotalNeed + qtyDamaged + qtyMissing))
            {
                // cannot cover
                int inputQ = qtyRetrieval + qtyDamaged + qtyMissing;
                if (inputQ != balance)
                {
                    return false;
                }
            }
            else
            {
                // can cover
                if (model.quantityTotalNeed != qtyRetrieval)
                {
                    return false;
                }
            }
            #endregion

            using (var dbContextTransaction = DbFactory.Instance.context.Database.BeginTransaction())
            {
                try
                {
                    // in here we deal with the problem goods
                    if (qtyDamaged != 0)
                    {
                        createAdjVoucher(itemNo, -qtyDamaged, DateTime.Now.Date, MyReasonCode.InstoreDamaged);
                    }
                    if (qtyMissing != 0)
                    {
                        createAdjVoucher(itemNo, -qtyMissing, DateTime.Now.Date, MyReasonCode.InstoreMissing);
                    }

                    #region Distribute goods here
                    var product = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == itemNo).First();
                    product.balance = product.balance == null ? 0 : (int)product.balance;

                    // get department list
                    var deps = DbFactory.Instance.context.Departments.OrderBy(x => x.departmentId).ToList();

                    // go through each department
                    foreach (var dep in deps)
                    {
                        // get the requests of this department
                        List<Request> requests = DbFactory.Instance.context.Requests.Include("RequestDetails").Where(x => x.departmentId == dep.departmentId && x.status == "approved" && x.approvedDate <= DueDate).ToList();
                        if (requests == null || requests.Count == 0)
                        {
                            continue;
                        }

                        // record the original quantity of this item
                        int qtyOriginal = (int)product.balance;

                        // go through each request
                        foreach (var req in requests)
                        {
                            if (req.RequestDetails != null)
                            {
                                // go through each request detail
                                foreach (var rd in req.RequestDetails)
                                {
                                    if (rd.itemNo == itemNo && rd.status == "unfulfilled")
                                    {
                                        // allocate item here
                                        int qtyPack = rd.quantityNeed - rd.quantityReceive;
                                        if (product.balance >= qtyPack)
                                        {
                                            // update inventory table**
                                            product.balance -= qtyPack;

                                            // update request detail table**
                                            rd.quantityPacked = qtyPack;
                                            rd.status = "preparing";
                                        }
                                        else if (product.balance > 0)
                                        {
                                            // not enough balance
                                            qtyPack = (int)product.balance;
                                            product.balance -= qtyPack;

                                            // update request detail table**
                                            rd.quantityPacked = qtyPack;
                                            rd.status = "preparing";

                                            break;
                                        }
                                        else
                                        {
                                            // when no balance for request
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        // insert stock card record here**
                        int qtyGap = (int)product.balance - qtyOriginal;

                        // when we have allocated the item to this department
                        if (qtyGap != 0)
                        {
                            var stkCard = new StockCard()
                            {
                                itemNo = itemNo,
                                dateModified = DateTime.Now.Date,
                                remark = dep.departmentId,
                                quantity = qtyGap,
                                balance = (int)product.balance
                            };
                            DbFactory.Instance.context.StockCards.Add(stkCard);

                            // update the database!
                            DbFactory.Instance.context.SaveChanges();

                            // send the email to notify the department                            
                            string sEmail = DbFactory.Instance.context.Staffs.AsNoTracking().Where(x => x.staffId == dep.staffIdDR).Select(x => x.staffEmail).FirstOrDefault();
                            if (sEmail != null)
                            {
                                string mailSubject = "Notification for Retrieval";
                                string mailContent = string.Format("{0} department({1}): please collect goods tomorrow", dep.departmentName, sEmail);

                                bool result = MyEmail.SendEmail("sa47team1@gmail.com", mailSubject, mailContent);
                            }
                        }
                    }
                    #endregion

                    // commit the transaction
                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Raise adjustment voucher when clerk finds any stationery is damaged or missing during the retrieval operation
        /// </summary>
        /// <param name="itemNo"></param>
        /// <param name="qty"></param>
        /// <param name="date"></param>
        /// <param name="reasonCode"></param>
        /// <param name="remark"></param>
        public static void createAdjVoucher(string itemNo, int qty, DateTime date, MyReasonCode reasonCode, string remark = "")
        {
            var adjVoucher = new AdjustmentVoucher()
            {
                itemNo = itemNo,
                quantity = qty,
                date = date
            };

            var stockCard = new StockCard()
            {
                itemNo = itemNo,
                dateModified = date
            };

            // put the remark
            switch (reasonCode)
            {
                // damaged
                case MyReasonCode.InstoreDamaged:
                    string rmk0 = "instore damaged";
                    adjVoucher.remark = rmk0;
                    stockCard.remark = rmk0;
                    break;

                // missing
                case MyReasonCode.InstoreMissing:
                    string rmk1 = "instore missing";
                    adjVoucher.remark = rmk1;
                    stockCard.remark = rmk1;
                    break;

                // self defined reason
                case MyReasonCode.Else:
                    adjVoucher.remark = remark;
                    stockCard.remark = remark;
                    break;

                case MyReasonCode.FreeOfCharge:
                    string rmk2 = "instore free of charge";
                    adjVoucher.remark = rmk2;
                    stockCard.remark = rmk2;
                    break;

                case MyReasonCode.InstoreWrongInput:
                    string rmk3 = "instore wrong input";
                    adjVoucher.remark = rmk3;
                    stockCard.remark = rmk3;
                    break;

                default:
                    throw new Exception("no such reasonCode");
            }

            // get the current balance
            var product = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == itemNo).First();
            int balance = product.balance == null ? 0 : (int)product.balance;

            // Validation here
            if (balance < -qty)
            {
                throw new Exception("qty connot larger than current balance");
            }

            // calculation
            balance = balance + qty;

            // update info here            
            adjVoucher.quantity = qty;

            stockCard.quantity = qty;
            stockCard.balance = balance;

            // update the database
            product.balance = balance;
            DbFactory.Instance.context.AdjustmentVouchers.Add(adjVoucher);
            DbFactory.Instance.context.StockCards.Add(stockCard);
            DbFactory.Instance.context.SaveChanges();
        }

        /// <summary>
        /// Find the consolidate retrieval quantity of this item before the given date
        /// </summary>
        /// <param name="itemNo">item number</param>
        /// <param name="DueDate">due date to retrieval goods</param>
        /// <returns></returns>
        public static RetrievalViewModel ViewRetrievalGood(string itemNo, DateTime DueDate)
        {
            var myModel = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == itemNo).Select(x => new RetrievalViewModel()
            {
                itemNo = x.itemNo,
                description = x.description,
                balance = x.balance == null ? 0 : (int)x.balance,
                categoryId = x.category,
                unitMeasure = x.unitMeasure,
                location = x.Category1.location,
                DueDate = DueDate
            }).First();

            List<Request> requests = DbFactory.Instance.context.Requests.Include("RequestDetails").Where(x => x.status == "approved" && x.approvedDate <= DueDate).ToList();

            foreach (var req in requests)
            {
                if (req.RequestDetails != null)
                {
                    foreach (var rd in req.RequestDetails)
                    {
                        if (rd.itemNo == itemNo && rd.status == "unfulfilled")
                        {
                            myModel.quantityTotalNeed += (rd.quantityNeed - rd.quantityReceive);
                        }
                    }
                }
            }

            if (myModel.quantityTotalNeed <= myModel.balance)
            {
                myModel.quantityRetrieval = myModel.quantityTotalNeed;
            }
            else
            {
                myModel.quantityRetrieval = myModel.balance;
            }

            return myModel;
        }

        /// <summary>
        /// Find the consolidate retrieval quantity of each item before the given date
        /// </summary>
        /// <param name="DueDate">due date to retrieval goods</param>
        /// <returns></returns>
        public static List<RetrievalViewModel> ViewRetrievalGoods(DateTime DueDate)
        {
            List<RetrievalViewModel> myModel = DbFactory.Instance.context.Inventories.Select(x => new RetrievalViewModel()
            {
                itemNo = x.itemNo,
                description = x.description,
                balance = x.balance == null ? 0 : (int)x.balance,
                categoryId = x.category,
                unitMeasure = x.unitMeasure,
                location = x.Category1.location,
                DueDate = DueDate
            }).ToList();

            List<Request> requests = DbFactory.Instance.context.Requests.Where(x => x.status == "approved" && x.approvedDate <= DueDate).ToList();

            foreach (var req in requests)
            {
                if (req.RequestDetails != null)
                {
                    foreach (var rd in req.RequestDetails)
                    {
                        if (rd.status == "unfulfilled")
                        {
                            var m = myModel.Where(x => x.itemNo == rd.itemNo).First();
                            m.quantityTotalNeed += (rd.quantityNeed - rd.quantityReceive);
                        }
                    }
                }
            }

            var Res = myModel.Where(x => x.balance > 0 && x.quantityTotalNeed > 0).ToList();
            return Res;
        }

        /// <summary>
        /// Find the stationery info which is used to put in request detail list
        /// </summary>
        /// <param name="itemNo"></param>
        /// <returns></returns>
        public static RequestViewModel CreateRequestViewModelByItemNumber(string itemNo)
        {
            var inventory = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == itemNo).First();
            var detail = new RequestViewModel()
            {
                itemNo = itemNo,
                description = inventory.description,
                stdPrice = inventory.stdPrice,
                unitMeasure = inventory.unitMeasure,
                category = inventory.category,
                quantityNeed = 1,
                status_request = "pending",
                status_requestDetail = "unfulfilled"
            };
            return detail;
        }

        /// <summary>
        /// Department raises a new request
        /// </summary>
        /// <param name="staffId"></param>
        /// <param name="departmentId"></param>
        /// <param name="requestDetails">the details of this request(which item and how many)</param>
        /// <returns></returns>
        public static bool RaiseRequest(int staffId, string departmentId, List<RequestViewModel> requestDetails)
        {
            if (requestDetails == null || requestDetails.Count <= 0)
                return false;

            using (var dbContextTransaction = DbFactory.Instance.context.Database.BeginTransaction())
            {
                try
                {
                    // Create the request
                    Request r1 = new Request()
                    {
                        staffId = staffId,
                        departmentId = departmentId,
                        approvedDate = DateTime.Now.Date,
                        status = "pending"
                    };

                    // Add to RequestList sql table
                    DbFactory.Instance.context.Requests.Add(r1);
                    DbFactory.Instance.context.SaveChanges();

                    // create the request details
                    foreach (var detail in requestDetails)
                    {
                        var rd = new RequestDetail()
                        {
                            requestId = r1.requestId,
                            itemNo = detail.itemNo,
                            quantityNeed = detail.quantityNeed,
                            status = "unfulfilled"
                        };

                        // Add to RequestDetail sql table
                        DbFactory.Instance.context.RequestDetails.Add(rd);
                        DbFactory.Instance.context.SaveChanges();
                    }

                    // Commit the transaction
                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Create a new item record in this request
        /// </summary>
        /// <param name="rd">request detial</param>
        /// <returns></returns>
        public static int InsertRequestDetail(RequestViewModel rd)
        {
            if (rd == null)
            {
                return 0;
            }

            // Check the request table
            var rParent = DbFactory.Instance.context.Requests.Where(x => x.requestId == rd.requestId && x.status == "pending").FirstOrDefault();
            if (rParent == null)
            {
                return 0;
            }

            // Check the request detail table
            var d1 = rParent.RequestDetails.Where(x => x.itemNo == rd.itemNo).FirstOrDefault();
            int iRes = 0;
            if (d1 == null)
            {
                var newRd = new RequestDetail()
                {
                    itemNo = rd.itemNo,
                    requestId = (int)rd.requestId,
                    quantityNeed = rd.quantityNeed,
                    status = "unfulfilled"
                };
                DbFactory.Instance.context.RequestDetails.Add(newRd);
                DbFactory.Instance.context.SaveChanges();
            }
            iRes = DbFactory.Instance.context.RequestDetails.Where(x => x.requestId == rd.requestId).Count();
            return iRes;
        }

        /// <summary>
        /// Get the request view model list
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public static List<RequestViewModel> GetRequestViewModelListByReqId(int requestId)
        {
            var details = DbFactory.Instance.context.RequestDetails.Where(x => x.requestId == requestId)
                            .Select(x => new RequestViewModel()
                            {
                                index_detail = x.index,
                                requestId = x.requestId,
                                itemNo = x.itemNo,
                                description = x.Inventory.description,
                                stdPrice = x.Inventory.stdPrice,
                                unitMeasure = x.Inventory.unitMeasure,
                                category = x.Inventory.category,
                                quantityNeed = x.quantityNeed,
                                status_request = x.Request.status,
                                status_requestDetail = x.status
                            }).ToList();
            return details;
        }

        /// <summary>
        /// Read request by staff
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        public static List<RequestViewModel> GetRequests(Staff staff)
        {
            List<Request> requests = DbFactory.Instance.context.Requests.
                Where(x => x.staffId == staff.staffId).OrderByDescending(x => x.requestId).ToList();

            List<RequestViewModel> model = ConvertRequestListToRequestViewModel(requests);
            return model;
        }

        /// <summary>
        /// Read request by DH
        /// </summary>
        /// <returns></returns>
        public static List<Request> DHGetRequest()
        {
            var requests = DbFactory.Instance.context.Requests.OrderBy(x => x.status).ToList();
            return requests;
        }

        /// <summary>
        /// Update the request info
        /// </summary>
        /// <param name="requestDetails"></param>
        /// <returns></returns>
        public static bool UpdateRequestDetails(List<RequestViewModel> requestDetails)
        {
            using (var dbContextTransaction = DbFactory.Instance.context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var d in requestDetails)
                    {
                        // change here

                        var rd = DbFactory.Instance.context.RequestDetails.Where(x =>
                            x.itemNo == d.itemNo &&
                            x.requestId == d.requestId).First();

                        rd.quantityNeed = d.quantityNeed;
                        DbFactory.Instance.context.SaveChanges();
                    }

                    // Commit the transaction
                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Delete the request detail
        /// </summary>
        /// <param name="rd"></param>
        /// <returns></returns>
        public static bool DeleteRequestDetail(RequestViewModel rvm)
        {
            if (rvm == null || rvm.requestId == null)
                return false;

            int requestId = (int)rvm.requestId;
            string itemNo = rvm.itemNo;

            var item = DbFactory.Instance.context.RequestDetails.Where(x =>
                x.requestId == requestId &&
                x.itemNo == itemNo).FirstOrDefault();

            if (item == null)
            {
                return false;
            }

            // Remove the item
            DbFactory.Instance.context.RequestDetails.Remove(item);
            var bRes = DbFactory.Instance.context.SaveChanges();

            return bRes == 1 ? true : false;
        }

        /// <summary>
        /// Delete the existed reqeust
        /// </summary>
        /// <param name="staffId"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public static bool DeleteRequest(int staffId, int requestId)
        {
            var item = DbFactory.Instance.context.Requests.Where(x =>
                x.requestId == requestId &&
                x.staffId == staffId &&
                x.status == "pending").FirstOrDefault();

            if (item == null)
            {
                return false;
            }

            using (var dbContextTransaction = DbFactory.Instance.context.Database.BeginTransaction())
            {
                try
                {
                    var details = DbFactory.Instance.context.RequestDetails.Where(x => x.requestId == item.requestId).ToList();
                    foreach (var d in details)
                    {
                        DbFactory.Instance.context.RequestDetails.Remove(d);
                        DbFactory.Instance.context.SaveChanges();
                    }
                    DbFactory.Instance.context.Requests.Remove(item);
                    DbFactory.Instance.context.SaveChanges();

                    dbContextTransaction.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    return false;
                }
            }
        }
    }
}