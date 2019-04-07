using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversityApp
{
    public class PurchaseOrderDAO
    {
        /// <summary>
        /// Update a bunch of purchase order information
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="deliveryDate"></param>
        /// <param name="status"></param>
        /// <param name="poDetails"></param>
        /// <param name="poId"></param>
        /// <returns></returns>
        public static bool UpdatePORequestDetails(DateTime orderDate, DateTime deliveryDate, string status, List<PurchaseOrderViewModel> poDetails, int? poId)
        {
            using (var dbContextTransaction = DbFactory.Instance.context.Database.BeginTransaction())
            {
                try
                {
                    var po = DbFactory.Instance.context.PurchaseOrders.Where(y => y.poId == poId).First();
                    po.orderDate = orderDate;
                    po.deliveryDate = deliveryDate;
                    po.status = status;

                    foreach (var detail in poDetails)
                    {
                        var pod = DbFactory.Instance.context.PurchaseOrderDetails.Where(x =>
                          x.itemNo == detail.itemNo &&
                          x.poId == detail.poId).First();

                        pod.quantity = detail.quantity;

                        if (po.status.ToUpper() == "CLOSED")
                        {
                            var item = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == detail.itemNo).First();
                            if (item.balance == null)
                            {
                                item.balance = pod.quantity;
                            }
                            else
                            {
                                item.balance += pod.quantity;
                            }

                            var stackCard = new StockCard()
                            {
                                itemNo = pod.itemNo,
                                remark = po.supplierId,
                                quantity = pod.quantity,
                                balance = (int)item.balance,
                                dateModified = po.deliveryDate
                            };
                            DbFactory.Instance.context.StockCards.Add(stackCard);
                            DbFactory.Instance.context.SaveChanges();
                        }
                    }
                    // Commit the transaction
                    DbFactory.Instance.context.SaveChanges();
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
        /// Update the purchase order information
        /// </summary>
        /// <param name="poDetail"></param>
        /// <returns></returns>
        public static bool UpdatePORequestDetail(PurchaseOrderViewModel poDetail)
        {
            try
            {
                if (poDetail == null)
                    return false;

                PurchaseOrder po = DbFactory.Instance.context.PurchaseOrders.Where(y => y.poId == poDetail.poId).First();
                if (po == null)
                    return false;

                if (po.status == null || po.status.ToLower() == "closed")
                    return false;

                PurchaseOrderDetail pod = DbFactory.Instance.context.PurchaseOrderDetails.Where(x =>
                              x.itemNo == poDetail.itemNo &&
                              x.poId == poDetail.poId).First();

                // change the quantity here
                pod.quantity = poDetail.quantity;
                DbFactory.Instance.context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Find the purchase order information
        /// </summary>
        /// <param name="itemNo"></param>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        private static PurchaseOrderViewModel getPOViewModel(string itemNo, string supplierId)
        {
            PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            try
            {
                decimal tenderPrice = DbFactory.Instance.context.Pricings.AsNoTracking().Where(x => x.itemNo == itemNo && x.supplierId == supplierId).
                    Select(x => x.tenderPrice).First();
                var item = DbFactory.Instance.context.Inventories.AsNoTracking().Where(x => x.itemNo == itemNo).
                    Select(x => new
                    {
                        x.reorderQuantity,
                        x.description,
                        x.category
                    }).First();
                //string supplierName = DbFactory.Instance.context.Suppliers.AsNoTracking().Where(x => x.supplierId == supplierId).Select(x => x.supplierName).First();

                model = new PurchaseOrderViewModel()
                {
                    itemNo = itemNo,

                    quantity = item.reorderQuantity,
                    description = item.description,
                    category = item.category,

                    supplierName = supplierId,
                    supplierID = supplierId,
                    tenderPrice = tenderPrice
                };
            }
            catch (Exception e)
            {

            }

            return model;

            //var price = DbFactory.Instance.context.Pricings.Where(x => x.itemNo == itemNo && x.supplierId == supplierId).FirstOrDefault();
            //var item = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == itemNo).FirstOrDefault();


            //if (price != null && item != null)
            //{
            //    model = new PurchaseOrderViewModel()
            //    {
            //        quantity = item.reorderQuantity,
            //        itemNo = item.itemNo,
            //        description = item.description,
            //        category = item.category,

            //        supplierName = price.Supplier == null ? null : price.Supplier.supplierName,
            //        supplierID = price.supplierId,
            //        tenderPrice = price.tenderPrice
            //    };
            //}
            //return model;
        }

        /// <summary>
        /// Check the duplication and increase the purchase order list
        /// </summary>
        /// <param name="itemNo"></param>
        /// <param name="supplierId"></param>
        /// <param name="list"></param>
        public static void IncreasePOList(string itemNo, string supplierId, ref List<PurchaseOrderViewModel> list)
        {
            if (itemNo == null || supplierId == null)
                return;

            bool bPass = true;
            if (list == null)
            {
                list = new List<PurchaseOrderViewModel>();
            }

            foreach (PurchaseOrderViewModel p in list)
            {
                if (p.itemNo == itemNo && p.supplierID == supplierId)
                {
                    bPass = false;
                    break;
                }
            }

            if (bPass)
            {
                var povm = getPOViewModel(itemNo, supplierId);
                if (povm != null)
                {
                    list.Add(povm);
                }
            }
        }

        /// <summary>
        /// Find the stationery tender information of each stationery of their own first three supplier
        /// </summary>
        /// <returns></returns>
        public static List<PurchaseOrderViewModel> getAllPOVM()
        {
            List<PurchaseOrderViewModel> povmList = new List<PurchaseOrderViewModel>();

            var invList = DbFactory.Instance.context.Inventories.AsNoTracking().
                Select(x => new
                {
                    x.itemNo,
                    x.supplierId1,
                    x.supplierId2,
                    x.supplierId3
                }).ToList();

            foreach (var item in invList)
            {
                var pp = DbFactory.Instance.context.Pricings.AsNoTracking().Include("Inventory").Include("Supplier").
                    Where(x => x.itemNo == item.itemNo && (x.supplierId == item.supplierId1 || x.supplierId == item.supplierId2 || x.supplierId == item.supplierId3)).
                    Select(x => new PurchaseOrderViewModel()
                    {
                        itemNo = item.itemNo,
                        supplierID = x.supplierId,
                        tenderPrice = x.tenderPrice,

                        balance = x.Inventory.balance,
                        reorderLevel = x.Inventory.reorderLevel,
                        quantity = x.Inventory.reorderQuantity,
                        description = x.Inventory.description,
                        category = x.Inventory.category,

                        supplierName = x.Supplier.supplierName
                    }).ToList();
                povmList.AddRange(pp);
            }
            return povmList;
        }

        /// <summary>
        /// Find the stationery tender information of the stationeries under particular category of their own first three supplier
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static List<PurchaseOrderViewModel> getAllPOVM(string categoryId)
        {
            List<PurchaseOrderViewModel> povmList = new List<PurchaseOrderViewModel>();

            var invList = DbFactory.Instance.context.Inventories.AsNoTracking().
                Where(x => x.category == categoryId).
                Select(x => new
                {
                    x.itemNo,
                    x.supplierId1,
                    x.supplierId2,
                    x.supplierId3
                }).ToList();

            foreach (var item in invList)
            {
                var pp = DbFactory.Instance.context.Pricings.AsNoTracking().
                    Where(x =>
                    x.itemNo == item.itemNo &&
                    (x.supplierId == item.supplierId1 || x.supplierId == item.supplierId2 || x.supplierId == item.supplierId3)
                    ).Select(x => new PurchaseOrderViewModel()
                    {
                        itemNo = item.itemNo,
                        supplierID = x.supplierId,
                        tenderPrice = x.tenderPrice,

                        balance = x.Inventory.balance,
                        reorderLevel = x.Inventory.reorderLevel,
                        quantity = x.Inventory.reorderQuantity,
                        description = x.Inventory.description,
                        category = x.Inventory.category,

                        supplierName = x.Supplier.supplierName
                    }).ToList();

                povmList.AddRange(pp);
            }
            return povmList;
        }

        /// <summary>
        /// Find the stationery tender information of each stationery of the given supplier
        /// </summary>
        /// <param name="poId">purchase order id which is used to find the supplier (one purchase id one supplier)</param>
        /// <returns></returns>
        public static List<PurchaseOrderViewModel> getAllPOVM(int poId)
        {
            List<PurchaseOrderViewModel> povmList = new List<PurchaseOrderViewModel>();

            string supplierId = DbFactory.Instance.context.PurchaseOrders.AsNoTracking().Where(x => x.poId == poId).Select(x => x.supplierId).FirstOrDefault();
            if (supplierId == null)
                return povmList;

            povmList = DbFactory.Instance.context.Pricings.AsNoTracking().Where(x => x.supplierId == supplierId).
                Select(x => new PurchaseOrderViewModel()
                {
                    supplierID = x.supplierId,
                    supplierName = x.Supplier.supplierName,
                    itemNo = x.itemNo,
                    description = x.Inventory.description,
                    balance = x.Inventory.balance,
                    reorderLevel = x.Inventory.reorderLevel,
                    quantity = x.Inventory.reorderQuantity,
                    category = x.Inventory.category,
                    tenderPrice = x.tenderPrice
                }).ToList();

            return povmList;
        }

        /// <summary>
        /// Find the stationery tender information of the stationeries under particular category of the given supplier
        /// </summary>
        /// <param name="poId">purchase order id which is used to find the supplier (one purchase id one supplier)</param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static List<PurchaseOrderViewModel> getAllPOVM(int poId, string categoryId)
        {
            List<PurchaseOrderViewModel> povmList = new List<PurchaseOrderViewModel>();

            string supplierId = DbFactory.Instance.context.PurchaseOrders.AsNoTracking().Where(x => x.poId == poId).Select(x => x.supplierId).FirstOrDefault();
            if (supplierId == null)
                return povmList;

            povmList = DbFactory.Instance.context.Pricings.AsNoTracking().
                Where(x => x.supplierId == supplierId && x.Inventory.category == categoryId).
                Select(x => new PurchaseOrderViewModel()
                {
                    supplierID = x.supplierId,
                    supplierName = x.Supplier.supplierName,
                    itemNo = x.itemNo,
                    description = x.Inventory.description,
                    balance = x.Inventory.balance,
                    reorderLevel = x.Inventory.reorderLevel,
                    quantity = x.Inventory.reorderQuantity,
                    category = x.Inventory.category,
                    tenderPrice = x.tenderPrice
                }).ToList();

            return povmList;
        }

        /// <summary>
        /// Create new item record of the given purchase order id
        /// </summary>
        /// <param name="itemNo"></param>
        /// <param name="poId"></param>
        /// <returns></returns>
        public static int addPODetail(string itemNo, int poId)
        {
            // Validate
            if (itemNo == null)
                return 0;

            // Validate
            var po = DbFactory.Instance.context.PurchaseOrders.Where(x => x.poId == poId).FirstOrDefault();
            if (po == null || po.PurchaseOrderDetails == null)
                return 0;

            // Validate
            int reorderQuantity = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == itemNo).Select(x => x.reorderQuantity).FirstOrDefault();
            if (reorderQuantity <= 0)
                return 0;

            // Validate (cannot insert duplicate item)
            var pod = po.PurchaseOrderDetails.Where(x => x.itemNo == itemNo).FirstOrDefault();
            if (pod != null)
                return 0;

            // insert into database
            var newPod = new PurchaseOrderDetail()
            {
                poId = poId,
                itemNo = itemNo,
                quantity = reorderQuantity
            };
            DbFactory.Instance.context.PurchaseOrderDetails.Add(newPod);
            DbFactory.Instance.context.SaveChanges();

            int res = DbFactory.Instance.context.PurchaseOrderDetails.Where(x => x.poId == poId).Count();
            return res;
        }

        /// <summary>
        /// Raise new purchase order
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="deliveryDate"></param>
        /// <param name="supplierId"></param>
        /// <param name="poDetails"></param>
        /// <returns></returns>
        public static bool RaisePORequest(DateTime orderDate, DateTime deliveryDate, string supplierId, List<PurchaseOrderViewModel> poDetails)
        {
            if (poDetails == null || poDetails.Count == 0)
            {
                return false;
            }

            var tempClassList = DbFactory.Instance.context.Suppliers.Select(x => new TempClass()
            {
                deliveryDate = deliveryDate,
                orderDate = orderDate,
                status = "Open",
                supplierId = x.supplierId
            }).ToList();

            // go through each given detail
            foreach (var detail in poDetails)
            {
                var d = tempClassList.Where(x => x.supplierId == detail.supplierID).First();
                var dt = new PurchaseOrderViewModel()
                {
                    itemNo = detail.itemNo,
                    quantity = detail.quantity
                };

                if (d.PurchaseOrderDetails == null)
                {
                    d.PurchaseOrderDetails = new List<PurchaseOrderViewModel>();
                }
                d.PurchaseOrderDetails.Add(dt);
            }

            using (var dbContextTransaction = DbFactory.Instance.context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var tempClass in tempClassList)
                    {
                        // create a po
                        if (tempClass.PurchaseOrderDetails != null && tempClass.PurchaseOrderDetails.Count > 0)
                        {
                            var p = new PurchaseOrder()
                            {
                                supplierId = tempClass.supplierId,
                                orderDate = tempClass.orderDate,
                                deliveryDate = tempClass.deliveryDate,
                                status = tempClass.status
                            };
                            DbFactory.Instance.context.PurchaseOrders.Add(p);
                            DbFactory.Instance.context.SaveChanges();

                            foreach (var detail in tempClass.PurchaseOrderDetails)
                            {
                                var dt = new PurchaseOrderDetail()
                                {
                                    poId = p.poId,
                                    itemNo = detail.itemNo,
                                    quantity = detail.quantity
                                };
                                DbFactory.Instance.context.PurchaseOrderDetails.Add(dt);
                                DbFactory.Instance.context.SaveChanges();
                            }
                        }
                    }

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
        /// Find the purchase orders
        /// </summary>
        /// <returns></returns>
        public static List<PurchaseOrderViewModel> GetPoList()
        {
            List<PurchaseOrderViewModel> povmList = DbFactory.Instance.context.PurchaseOrders.
                Select(p => new PurchaseOrderViewModel()
                {
                    poId = p.poId,
                    supplierID = p.supplierId,
                    supplierName = p.Supplier.supplierName,
                    orderDate = p.orderDate,
                    deliveryDate = p.deliveryDate,
                    status = p.status
                }).OrderByDescending(x => x.poId).ToList();
            //OrderBy(x => x.status.ToUpper().Equals("CLOSED")).ThenBy(x => x.status.ToUpper().Equals("SENT")).ThenBy(x => x.status.ToUpper().Equals("OPEN")).ThenBy(x => x.poId).ToList();

            return povmList;
        }

        /// <summary>
        /// Find the purchase order details of given purcahse order id
        /// </summary>
        /// <param name="poId">purcahse order id</param>
        /// <returns></returns>
        public static List<PurchaseOrderViewModel> GetPoDetailList(int poId)
        {
            // query
            var myPo = DbFactory.Instance.context.PurchaseOrders.
                Where(x => x.poId == poId).
                Select(x =>
                new
                {
                    x.poId,
                    x.status,
                    x.deliveryDate,
                    x.orderDate,
                    x.supplierId,
                    x.Supplier.supplierName
                }).FirstOrDefault();
            if (myPo == null)
                return null;

            var result = DbFactory.Instance.context.PurchaseOrderDetails.
                Where(x => x.poId == poId).
                Select(x => new PurchaseOrderViewModel()
                {
                    supplierID = myPo.supplierId,
                    supplierName = myPo.supplierName,
                    itemNo = x.itemNo,
                    description = x.Inventory.description,
                    quantity = x.quantity,
                    poId = myPo.poId,
                    tenderPrice = DbFactory.Instance.context.Pricings.Where(y => y.supplierId == myPo.supplierId && y.itemNo == x.itemNo).Select(z => z.tenderPrice).FirstOrDefault(),
                    orderDate = myPo.orderDate,
                    deliveryDate = myPo.deliveryDate,
                    status = myPo.status,
                    category = x.Inventory.category,
                    unitMeasure = x.Inventory.unitMeasure
                }).ToList();

            return result;
        }

        /// <summary>
        /// Find the purchase order detail of given purcahse order id and item number
        /// </summary>
        /// <param name="poId">purcahse order id</param>
        /// <param name="itemNo">item number</param>
        /// <returns></returns>
        public static PurchaseOrderViewModel GetPoDetail(int poId, string itemNo)
        {
            PurchaseOrderDetail pod = DbFactory.Instance.context.PurchaseOrderDetails.Where(x => x.poId == poId && x.itemNo == itemNo).FirstOrDefault();
            if (pod == null)
                return null;

            PurchaseOrder po = pod.PurchaseOrder;
            if (po == null)
                return null;

            Pricing price = DbFactory.Instance.context.Pricings.Where(x =>
                 x.supplierId == pod.PurchaseOrder.supplierId &&
                 x.itemNo == itemNo).FirstOrDefault();
            if (price == null)
                return null;

            PurchaseOrderViewModel povm = new PurchaseOrderViewModel()
            {
                // reorder qty here
                poId = pod.poId,
                status = pod.PurchaseOrder.status,
                orderDate = pod.PurchaseOrder.orderDate,
                deliveryDate = pod.PurchaseOrder.deliveryDate,

                quantity = pod.quantity,
                description = pod.Inventory.description,
                category = pod.Inventory.category,

                tenderPrice = price.tenderPrice,
                itemNo = price.itemNo,

                supplierID = po.Supplier.supplierId,
                supplierName = po.Supplier.supplierName,
                unitMeasure = pod.Inventory.unitMeasure
            };
            return povm;
        }

        /// <summary>
        /// Delete the purchase order detail of given purcahse order id and item number
        /// </summary>
        /// <param name="poId">purcahse order id</param>
        /// <param name="itemNo">item number</param>
        public static void DeletePODetail(int poId, string itemNo)
        {
            var pod = DbFactory.Instance.context.PurchaseOrderDetails.Where(x => x.poId == poId && x.itemNo == itemNo).FirstOrDefault();
            if (pod == null)
                return;

            DbFactory.Instance.context.PurchaseOrderDetails.Remove(pod);
            DbFactory.Instance.context.SaveChanges();
        }

        /// <summary>
        /// Find the purchase order by id
        /// </summary>
        /// <param name="poId">purchase order id</param>
        /// <returns></returns>
        public static PurchaseOrder findPoById(int? poId)
        {
            return DbFactory.Instance.context.PurchaseOrders.Find(poId);
        }

        /// <summary>
        /// Find the purchase order by id
        /// </summary>
        /// <param name="poId">purchase order id</param>
        /// <returns></returns>
        public static PurchaseOrderViewModel findPoViewModelById(int poId)
        {
            PurchaseOrder poTemp = DbFactory.Instance.context.PurchaseOrders.Find(poId);
            if (poTemp == null)
            {
                return null;
            }

            PurchaseOrderViewModel po = new PurchaseOrderViewModel()
            {
                supplierID = poTemp.supplierId,
                supplierName = poTemp.Supplier.supplierName,
                poId = poTemp.poId,
                orderDate = poTemp.orderDate,
                deliveryDate = poTemp.deliveryDate,
                status = poTemp.status
            };
            return po;
        }

        /// <summary>
        /// Delete the purchase order by id
        /// </summary>
        /// <param name="poId">purchase order id</param>
        /// <returns></returns>
        public static bool deletePoById(int? poId)
        {
            using (var dbContextTransaction = DbFactory.Instance.context.Database.BeginTransaction())
            {
                try
                {
                    var purchaseOrder = DbFactory.Instance.context.PurchaseOrders.Where(x => x.poId == poId).First();

                    var purchaseOrderDetails = new List<PurchaseOrderDetail>();
                    purchaseOrderDetails.AddRange(purchaseOrder.PurchaseOrderDetails);

                    foreach (var p in purchaseOrderDetails)
                    {
                        DbFactory.Instance.context.PurchaseOrderDetails.Remove(p);
                        DbFactory.Instance.context.SaveChanges();
                    }

                    DbFactory.Instance.context.PurchaseOrders.Remove(purchaseOrder);
                    DbFactory.Instance.context.SaveChanges();

                    dbContextTransaction.Commit();

                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Find purchase orders
        /// </summary>
        /// <param name="poId"></param>
        /// <returns></returns>
        public static List<PurchaseOrder> findPurchaseOrderList(int poId)
        {
            var purchaseOrder = DbFactory.Instance.context.PurchaseOrders.Where(x => x.poId == poId).ToList();
            return purchaseOrder;
        }

        /// <summary>
        /// Find purchase order details
        /// </summary>
        /// <param name="poId"></param>
        /// <returns></returns>
        public static List<PurchaseOrderDetail> findPurchaseOrderDetail(int poId)
        {
            var purchaseOrderDetail = DbFactory.Instance.context.PurchaseOrderDetails.Where(x => x.poId == poId).Distinct().ToList();
            return purchaseOrderDetail;
        }

        /// <summary>
        /// Find inventories in this purchase order
        /// </summary>
        /// <param name="poId"></param>
        /// <returns></returns>
        public static dynamic findInventoriesInPurchaseOrder(int poId)
        {
            var inventories = DbFactory.Instance.context.Inventories.Join(DbFactory.Instance.context.PurchaseOrderDetails,
                u => u.itemNo, uir => uir.itemNo, (u, uir) => new { u, uir }).
                Where(m => m.uir.poId == poId && m.u.itemNo == m.uir.itemNo).Distinct().Select(m => new
                { m.uir.poId, m.uir.itemNo, m.u.stdPrice, m.u.description, m.u.unitMeasure }).Distinct().ToList();
            return inventories;
        }

        /// <summary>
        /// find suppliers in this purchase order
        /// </summary>
        /// <param name="poId"></param>
        /// <returns></returns>
        public static dynamic findSupplierInPurchaseOrder(int poId)
        {
            var supplier = DbFactory.Instance.context.PurchaseOrders.Where(x => x.poId == poId).Distinct().Select(m => new
            {
                m.Supplier.supplierName,
                m.Supplier.address,
                m.Supplier.email,
                m.Supplier.contactName,
                m.Supplier.phone,
            }).Distinct().ToList();
            return supplier;
        }

        /// <summary>
        /// Find tender pricings in this purchase order
        /// </summary>
        /// <param name="poId"></param>
        /// <returns></returns>
        public static List<Pricing> findSupplierPrice(int poId)
        {
            var inventories = DbFactory.Instance.context.Inventories.Join(DbFactory.Instance.context.PurchaseOrderDetails,
                u => u.itemNo, uir => uir.itemNo, (u, uir) => new { u, uir }).
                Where(m => m.uir.poId == poId && m.u.itemNo == m.uir.itemNo).Distinct().Select(m => new
                { m.uir.poId, m.uir.itemNo, m.u.stdPrice, m.u.description, m.u.unitMeasure }).Distinct().ToList();

            var supplier = DbFactory.Instance.context.PurchaseOrders.Where(x => x.poId == poId).Distinct().Select(m => new
            {
                m.Supplier.supplierId,
                m.Supplier.supplierName,
                m.Supplier.address,
                m.Supplier.email,
                m.Supplier.contactName,
                m.Supplier.phone,
            }).Distinct().First();

            List<Pricing> priceList = new List<Pricing>();
            foreach (var i in inventories)
            {
                var price = DbFactory.Instance.context.Pricings.Where(x => x.itemNo == i.itemNo && x.supplierId == supplier.supplierId).ToList();
                priceList.AddRange(price);
            }
            return priceList;
        }

        /// <summary>
        /// Inner class to record data
        /// </summary>  
        private class TempClass
        {
            public int poId { get; set; }
            public string supplierId { get; set; }
            public DateTime orderDate { get; set; }
            public DateTime deliveryDate { get; set; }
            public string status { get; set; }
            public List<PurchaseOrderViewModel> PurchaseOrderDetails;
        }
    }
}