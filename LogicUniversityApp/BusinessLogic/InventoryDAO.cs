using LogicUniversityApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversityApp
{
    public class InventoryDAO
    {
        /// <summary>
        /// Find the inventories
        /// </summary>
        /// <returns></returns>
        public static List<InventoryViewModel> GetInventories()
        {
            var list = DbFactory.Instance.context.Inventories.Select(x => new InventoryViewModel()
            {
                itemNo = x.itemNo,
                category = x.category,
                description = x.description,
                balance = x.balance,
                reorderLevel = x.reorderLevel,
                reorderQuantity = x.reorderQuantity,
                unitMeasure = x.unitMeasure,
                stdPrice = x.stdPrice,
                supplierId1 = x.supplierId1,
                supplierId2 = x.supplierId2,
                supplierId3 = x.supplierId3
            }).ToList();

            return list;
        }

        /// <summary>
        /// Find the inventories
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static List<InventoryViewModel> GetInventories(string categoryId)
        {
            var list = DbFactory.Instance.context.Inventories.Where(x => x.category == categoryId).Select(x => new InventoryViewModel()
            {
                itemNo = x.itemNo,
                category = x.category,
                description = x.description,
                balance = x.balance,
                reorderLevel = x.reorderLevel,
                reorderQuantity = x.reorderQuantity,
                unitMeasure = x.unitMeasure,
                stdPrice = x.stdPrice,
                supplierId1 = x.supplierId1,
                supplierId2 = x.supplierId2,
                supplierId3 = x.supplierId3
            }).ToList();

            return list;
        }

        /// <summary>
        /// Find the inventory information by item number
        /// </summary>
        /// <param name="itemNo"></param>
        /// <returns></returns>
        public static InventoryViewModel GetInventoryByItemNo(string itemNo)
        {
            var list = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == itemNo).
                Select(x => new InventoryViewModel()
                {
                    itemNo = x.itemNo,
                    category = x.category,
                    description = x.description,
                    balance = x.balance,
                    reorderLevel = x.reorderLevel,
                    reorderQuantity = x.reorderQuantity,
                    unitMeasure = x.unitMeasure,
                    stdPrice = x.stdPrice,
                    supplierId1 = x.supplierId1,
                    supplierId2 = x.supplierId2,
                    supplierId3 = x.supplierId3
                }).FirstOrDefault();

            return list;
        }

        /// <summary>
        /// Find the categories
        /// </summary>
        /// <returns></returns>
        public static List<string> getAllCategoryId()
        {
            var ids = DbFactory.Instance.context.Categories.Select(x => x.categoryId).ToList();
            return ids;
        }
    }
}