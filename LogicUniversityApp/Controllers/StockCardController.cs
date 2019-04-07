using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;

namespace LogicUniversityApp.Controllers
{
    [Authorize(Roles = "SC,SS,SM")]
    public class StockCardController : Controller
    {
        // GET: StockCard
        public ActionResult Index(string id)
        {
            List<StockCardViewModel> stockCardViewModel = new List<StockCardViewModel>();
            if (!string.IsNullOrEmpty(id))
            {
                TempData["id"] = id;
                stockCardViewModel = StockCardDAO.GetStockCardsById(id);
            }
            return View(stockCardViewModel);
        }

        // GET: StockCard/Create
        public ActionResult Create()
        {
            string itemNo = null;
            if (TempData.ContainsKey("id"))
            {
                itemNo = TempData["id"].ToString();
                TempData["id"] = itemNo;
            }
            InventoryViewModel i = InventoryDAO.GetInventoryByItemNo(itemNo);
            StockCardViewModel stockCardViewModel = new StockCardViewModel();
            stockCardViewModel.balance = i.balance;
            stockCardViewModel.category = i.category;
            stockCardViewModel.description = i.description;
            stockCardViewModel.itemNo = i.itemNo;
            string today = DateTime.Now.ToShortDateString();
            ViewBag.date = today;

            return View(stockCardViewModel);
        }

        // POST: StockCard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StockCardViewModel stockCardViewModel)
        {
            string itemNo = null;
            if (TempData.ContainsKey("id"))
            {
                itemNo = TempData["id"].ToString();
            }
            InventoryViewModel i = InventoryDAO.GetInventoryByItemNo(itemNo);
            stockCardViewModel.balance = i.balance;
            stockCardViewModel.category = i.category;
            stockCardViewModel.description = i.description;
            stockCardViewModel.itemNo = i.itemNo;
            stockCardViewModel.dateModified = DateTime.Today;
            bool result = StockCardDAO.CreateStockCard(stockCardViewModel);
            if (result)
            { return RedirectToAction("Index", new { id = itemNo }); }
            else
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        // GET: StockCard/ViewStockCard
        public ActionResult ViewStockCard(string category)
        {
            List<string> categories = InventoryDAO.getAllCategoryId();
            ViewBag.categoryList = new SelectList(categories, category);

            List<InventoryViewModel> list = new List<InventoryViewModel>();
            if (!String.IsNullOrEmpty(category) && category.ToLower() != "all")
            {
                list = InventoryDAO.GetInventories(category);
            }
            else
            {
                list = InventoryDAO.GetInventories();
            }

            return View(list);
        }
    }
}
