using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversityApp
{
    public class StockCardDAO
    {
        /// <summary>
        /// Find the stock card record of this item
        /// </summary>
        /// <param name="itemNo"></param>
        /// <returns></returns>
        public static List<StockCardViewModel> GetStockCardsById(string itemNo)
        {
            var item = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == itemNo).FirstOrDefault();

            if (item == null)
                return null;

            var list = DbFactory.Instance.context.StockCards.Where(x => x.itemNo == itemNo).Select(x =>
                  new StockCardViewModel()
                  {
                      index = x.index,
                      itemNo = x.itemNo,
                      category = item.category,
                      description = item.description,
                      balance = x.balance,
                      remark = x.remark,
                      quantity = x.quantity,
                      dateModified = x.dateModified
                  }).ToList();
            return list;
        }

        /// <summary>
        /// Create a new stock card record
        /// </summary>
        /// <param name="model">stock card info</param>
        /// <returns></returns>
        public static bool CreateStockCard(StockCardViewModel model)
        {
            using (var dbContextTransaction = DbFactory.Instance.context.Database.BeginTransaction())
            {
                try
                {
                    // change the inventory balance...
                    var item = DbFactory.Instance.context.Inventories.Where(x => x.itemNo == model.itemNo).First();


                    if (item.balance == null)
                        item.balance = 0;

                    if (item.balance >= -model.quantity)
                    {
                        item.balance = item.balance + model.quantity;

                        StockCard sc = new StockCard()
                        {
                            itemNo = model.itemNo,
                            remark = model.remark,
                            quantity = model.quantity,
                            balance = (int)item.balance,
                            dateModified = DateTime.Now.Date
                        };
                        DbFactory.Instance.context.StockCards.Add(sc);
                        DbFactory.Instance.context.SaveChanges();

                        // Commit the transaction
                        dbContextTransaction.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                }
                return false;
            }
        }
    }
}