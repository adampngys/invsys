using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogicUniversityApp
{
    public class AdjustmentVoucherDAO
    {
        /// <summary>
        /// Create a new inventory record
        /// </summary>
        /// <param name="inventoryViewModel"></param>
        /// <returns></returns>
        public static bool CreateInventories(InventoryViewModel inventoryViewModel)
        {
            if (inventoryViewModel == null)
            {
                return false;
            }

            if (inventoryViewModel.balance < 0
                || inventoryViewModel.reorderLevel <= 0
                || inventoryViewModel.reorderQuantity <= 0
                || inventoryViewModel.stdPrice <= 0)
            {
                return false;
            }

            // Validation for same supplier
            if (inventoryViewModel.supplierId1 == inventoryViewModel.supplierId2 || inventoryViewModel.supplierId2 == inventoryViewModel.supplierId3 || inventoryViewModel.supplierId1 == inventoryViewModel.supplierId3)
            {
                return false;
            }

            try
            {
                Inventory inventory = new Inventory()
                {
                    itemNo = inventoryViewModel.itemNo,
                    category = inventoryViewModel.category,
                    description = inventoryViewModel.description,
                    balance = inventoryViewModel.balance,
                    reorderLevel = inventoryViewModel.reorderLevel,
                    reorderQuantity = inventoryViewModel.reorderQuantity,
                    unitMeasure = inventoryViewModel.unitMeasure,
                    stdPrice = inventoryViewModel.stdPrice,
                    supplierId1 = inventoryViewModel.supplierId1,
                    supplierId2 = inventoryViewModel.supplierId2,
                    supplierId3 = inventoryViewModel.supplierId3
                };

                DbFactory.Instance.context.Inventories.Add(inventory);
                DbFactory.Instance.context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Edit inventory record
        /// </summary>
        /// <param name="inventoryViewModel"></param>
        /// <returns></returns>
        public static bool EditInventories(InventoryViewModel inventoryViewModel)
        {
            if (inventoryViewModel == null || inventoryViewModel.itemNo == null)
            {
                return false;
            }

            var item = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == inventoryViewModel.itemNo).FirstOrDefault();
            if (item == null)
            {
                return false;
            }

            try
            {
                item.category = inventoryViewModel.category;
                item.description = inventoryViewModel.description;
                item.balance = inventoryViewModel.balance;
                item.stdPrice = inventoryViewModel.stdPrice;
                item.reorderLevel = inventoryViewModel.reorderLevel;
                item.reorderQuantity = inventoryViewModel.reorderQuantity;
                item.unitMeasure = inventoryViewModel.unitMeasure;

                DbFactory.Instance.context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Create a new adjustment voucher record
        /// </summary>
        /// <param name="itemNo"></param>
        /// <param name="qty"></param>
        /// <param name="date"></param>
        /// <param name="reasonCode"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static bool CreateAdjustmentVoucher(string itemNo, int qty, DateTime date, MyReasonCode reasonCode, string remark = "")
        {
            using (var dbContextTransaction = DbFactory.Instance.context.Database.BeginTransaction())
            {
                try
                {
                    RequestDAO.createAdjVoucher(itemNo, qty, date, reasonCode, remark);

                    // commit the transaction
                    dbContextTransaction.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                }
            }
            return false;
        }

        /// <summary>
        /// Find the adjustment vouchers of target item which are raised before the given time
        /// </summary>
        /// <param name="itemNo">target item number</param>
        /// <param name="year">input -1 means any year</param>
        /// <param name="month">input -1 means any month</param>
        /// <returns></returns>
        public static List<AdjustmentVoucherViewModel> FindDetailAdjustmentVoucher(string itemNo, int year, int month)
        {
            List<AdjustmentVoucherViewModel> resultList = new List<AdjustmentVoucherViewModel>();
            if (year == -1 && month == -1)
            {
                // not filter
                resultList = DbFactory.Instance.context.AdjustmentVouchers.AsNoTracking().Where(
                    x => x.itemNo == itemNo).Select(x => new AdjustmentVoucherViewModel()
                    {
                        voucherId = x.voucherId,
                        quantity = x.quantity,
                        remark = x.remark,
                        date = x.date,
                        itemNo = x.itemNo,
                        unitPrice = x.Inventory.stdPrice,
                        description = x.Inventory.description,
                        totalamount = (x.quantity * x.Inventory.stdPrice),
                        year = year,
                        month = month
                    }).ToList();
            }
            else if (year == -1)
            {
                // filter by month
                resultList = DbFactory.Instance.context.AdjustmentVouchers.AsNoTracking().Where(
                    x => x.itemNo == itemNo &&
                    x.date.Month.Equals(month)).Select(x => new AdjustmentVoucherViewModel()
                    {
                        voucherId = x.voucherId,
                        quantity = x.quantity,
                        remark = x.remark,
                        date = x.date,
                        itemNo = x.itemNo,
                        unitPrice = x.Inventory.stdPrice,
                        description = x.Inventory.description,
                        totalamount = (x.quantity * x.Inventory.stdPrice),
                        year = year,
                        month = month
                    }).ToList();
            }
            else if (month == -1)
            {
                // filter by year
                resultList = DbFactory.Instance.context.AdjustmentVouchers.AsNoTracking().Where(
                    x => x.itemNo == itemNo &&
                    x.date.Year.Equals(year)).Select(x => new AdjustmentVoucherViewModel()
                    {
                        voucherId = x.voucherId,
                        quantity = x.quantity,
                        remark = x.remark,
                        date = x.date,
                        itemNo = x.itemNo,
                        unitPrice = x.Inventory.stdPrice,
                        description = x.Inventory.description,
                        totalamount = (x.quantity * x.Inventory.stdPrice),
                        year = year,
                        month = month
                    }).ToList();
            }
            else
            {
                // filter by both year and month
                resultList = DbFactory.Instance.context.AdjustmentVouchers.AsNoTracking().Where(
                    x => x.itemNo == itemNo &&
                    x.date.Year.Equals(year) &&
                    x.date.Month.Equals(month)).Select(x => new AdjustmentVoucherViewModel()
                    {
                        voucherId = x.voucherId,
                        quantity = x.quantity,
                        remark = x.remark,
                        date = x.date,
                        itemNo = x.itemNo,
                        unitPrice = x.Inventory.stdPrice,
                        description = x.Inventory.description,
                        totalamount = (x.quantity * x.Inventory.stdPrice),
                        year = year,
                        month = month
                    }).ToList();
            }

            return resultList;
        }

        /// <summary>
        /// Find the adjustment vouchers which are raised before the given time
        /// </summary>
        /// <param name="year">input -1 means any year</param>
        /// <param name="month">input -1 means any month</param>
        /// <returns></returns>
        public static List<AdjustmentVoucherViewModel> FindGeneralAdjustmentVoucher(int year, int month)
        {
            List<AdjustmentVoucherViewModel> container = DbFactory.Instance.context.Inventories.AsNoTracking().Select(x => new AdjustmentVoucherViewModel()
            {
                itemNo = x.itemNo,
                description = x.description,
                unitPrice = x.stdPrice,
                totalamount = 0,
                year = year,
                month = month
            }).ToList();

            foreach (var containerDetail in container)
            {
                var list = FindDetailAdjustmentVoucher(containerDetail.itemNo, year, month);
                if (list != null)
                {
                    foreach (var d in list)
                    {
                        containerDetail.totalamount += d.totalamount;
                    }
                }
            }

            container = container.Where(x => x.totalamount != 0).ToList();

            container = container.OrderBy(x => x.totalamount).ToList();

            return container;
        }

        /// <summary>
        /// Find the categories
        /// </summary>
        /// <returns></returns>
        public static SelectList GetCategorySelectList()
        {
            SelectList listItem = new SelectList(DbFactory.Instance.context.Categories, "categoryId", "categoryId");

            return listItem;
        }

        /// <summary>
        /// Find the suppliers
        /// </summary>
        /// <returns></returns>
        public static SelectList GetSupplierSelectList()
        {
            SelectList listItem = new SelectList(DbFactory.Instance.context.Suppliers, "supplierId", "supplierName");

            return listItem;
        }

        /// <summary>
        /// Delete inventory record
        /// </summary>
        /// <param name="inventoryViewModel"></param>
        /// <returns></returns>
        public static bool DeleteInventories(InventoryViewModel inventoryViewModel)
        {
            bool result = false;
            try
            {
                Inventory inventory = ToInventory(inventoryViewModel);
                DbFactory.Instance.context.Inventories.Remove(inventory);
                DbFactory.Instance.context.SaveChanges();
                result = true;
            }
            catch (Exception e)
            {

            }
            return result;
        }

        /// <summary>
        /// Invertory view model parser
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        public static InventoryViewModel ToInventoryViewModel(Inventory inventory)
        {
            InventoryViewModel inventoryViewModel = new InventoryViewModel
            {
                balance = inventory.balance,
                category = inventory.category,
                itemNo = inventory.itemNo,
                description = inventory.description,
                stdPrice = inventory.stdPrice,
                reorderLevel = inventory.reorderLevel,
                reorderQuantity = inventory.reorderQuantity,
                supplierId1 = inventory.supplierId1,
                supplierId2 = inventory.supplierId2,
                supplierId3 = inventory.supplierId3,
                unitMeasure = inventory.unitMeasure
            };
            return inventoryViewModel;
        }

        /// <summary>
        /// Inventory model parser
        /// </summary>
        /// <param name="inventoryViewModel"></param>
        /// <returns></returns>
        public static Inventory ToInventory(InventoryViewModel inventoryViewModel)
        {

            Inventory result = DbFactory.Instance.context.Inventories.Where(d => d.itemNo == inventoryViewModel.itemNo).FirstOrDefault();
            if (result == null)
            {
                result = new Inventory();
                result.itemNo = inventoryViewModel.itemNo;
            }
            result.description = inventoryViewModel.description;
            result.category = inventoryViewModel.category;
            result.balance = inventoryViewModel.balance;
            result.stdPrice = inventoryViewModel.stdPrice;
            result.supplierId1 = inventoryViewModel.supplierId1;
            result.supplierId2 = inventoryViewModel.supplierId2;
            result.supplierId3 = inventoryViewModel.supplierId3;
            result.unitMeasure = inventoryViewModel.unitMeasure;
            result.reorderLevel = inventoryViewModel.reorderLevel;
            result.reorderQuantity = inventoryViewModel.reorderQuantity;


            return result;
        }

        /// <summary>
        /// Find the inventories
        /// </summary>
        /// <returns></returns>
        public static List<InventoryViewModel> GetInventoryViewModels()
        {
            List<InventoryViewModel> result = new List<InventoryViewModel>();
            List<Inventory> ilist = DbFactory.Instance.context.Inventories.ToList();
            foreach (Inventory i in ilist)
            {
                InventoryViewModel ivm = ToInventoryViewModel(i);
                result.Add(ivm);
            }
            return result;
        }

        /// <summary>
        /// Find adjustment vouchers before given date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static List<AdjustmentVoucher> GetAdjustmentVouchersByDate(DateTime date)
        {
            return DbFactory.Instance.context.AdjustmentVouchers.Where(x => x.date.Year == date.Year && x.date.Month == date.Month).Distinct().ToList();
        }

        /// <summary>
        /// Find the inventories which are list in the adjustment voucher before given date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static dynamic GetListOfInventories(DateTime date)
        {
            return DbFactory.Instance.context.AdjustmentVouchers.Where(x => x.date.Year == date.Year && x.date.Month == date.Month).Select(m => new { m.Inventory.itemNo, m.Inventory.description, m.Inventory.stdPrice }).Distinct().ToList();
        }
    }
}