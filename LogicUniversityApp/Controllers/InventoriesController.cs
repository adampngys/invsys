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
    public class InventoriesController : Controller
    {
        // GET: Inventories
        public ActionResult Index(string category, string searchString)
        {
            ViewBag.category = AdjustmentVoucherDAO.GetCategorySelectList();

            List<InventoryViewModel> inventories = AdjustmentVoucherDAO.GetInventoryViewModels();

            if (!String.IsNullOrEmpty(searchString))
            {
                inventories = inventories.Where(s => s.description.ToUpper().Contains(searchString.ToUpper())).ToList();
            }

            if (!string.IsNullOrEmpty(category))
            {
                inventories = inventories.Where(x => x.category == category).ToList();
            }
            return View(inventories.ToList());
        }

        // GET: Inventories/Details
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryViewModel inventory = InventoryDAO.GetInventoryByItemNo(id);
            return View(inventory);
        }

        // GET: Inventories/Create
        public ActionResult Create()
        {
            List<SelectListItem> listItem = new List<SelectListItem>
            {
                new SelectListItem{Text="Dozen",Value="Dozen"},
                new SelectListItem{Text="Box",Value="Box"},
                new SelectListItem{Text="Each",Value="Each"},
                new SelectListItem{Text="Set",Value="Set"},
                new SelectListItem{Text="Packet",Value="Packet"}
            };

            ViewBag.unitMeasure = new SelectList(listItem, "Value", "Text");
            ViewBag.category = AdjustmentVoucherDAO.GetCategorySelectList();
            ViewBag.supplierId1 = AdjustmentVoucherDAO.GetSupplierSelectList();
            ViewBag.supplierId2 = AdjustmentVoucherDAO.GetSupplierSelectList();
            ViewBag.supplierId3 = AdjustmentVoucherDAO.GetSupplierSelectList();
            return View();
        }

        // POST: Inventories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InventoryViewModel inventoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                List<SelectListItem> listItem = new List<SelectListItem>
                {
                    new SelectListItem{Text="Dozen",Value="Dozen"},
                    new SelectListItem{Text="Box",Value="Box"},
                    new SelectListItem{Text="Each",Value="Each"},
                    new SelectListItem{Text="Set",Value="Set"},
                    new SelectListItem{Text="Packet",Value="Packet"}
                };

                ViewBag.unitMeasure = new SelectList(listItem, "Value", "Text");
                ViewBag.category = AdjustmentVoucherDAO.GetCategorySelectList();
                ViewBag.supplierId1 = AdjustmentVoucherDAO.GetSupplierSelectList();
                ViewBag.supplierId2 = AdjustmentVoucherDAO.GetSupplierSelectList();
                ViewBag.supplierId3 = AdjustmentVoucherDAO.GetSupplierSelectList();

                return View(inventoryViewModel);
            }

            bool result = AdjustmentVoucherDAO.CreateInventories(inventoryViewModel);
            if (!result)
            {
                return View("~/Views/Shared/Error.cshtml");
            }
            return RedirectToAction("Index");
        }

        // GET: Inventories/Edit
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            InventoryViewModel inventoryViewModel = InventoryDAO.GetInventoryByItemNo(id);

            if (inventoryViewModel == null)
            {
                return View("~/Views/Shared/Error.cshtml");
            };

            // drop down list 
            List<String> listItem = new List<string>();
            listItem.Add("Dozen");
            listItem.Add("Box");
            listItem.Add("Each");
            listItem.Add("Set");
            listItem.Add("Packet");
            ViewBag.unitMeasure = new SelectList(listItem, inventoryViewModel.unitMeasure);

            // drop down list 
            var categories = InventoryDAO.getAllCategoryId();
            ViewBag.category = new SelectList(categories, inventoryViewModel.category);

            // push to view
            return View(inventoryViewModel);
        }

        // POST: Inventories/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InventoryViewModel inventoryViewModel)
        {
            if (!ModelState.IsValidField(nameof(InventoryViewModel.itemNo))
                || !ModelState.IsValidField(nameof(InventoryViewModel.category))
                || !ModelState.IsValidField(nameof(InventoryViewModel.description))
                || !ModelState.IsValidField(nameof(InventoryViewModel.unitMeasure))
                || !ModelState.IsValidField(nameof(InventoryViewModel.reorderQuantity))
                || !ModelState.IsValidField(nameof(InventoryViewModel.reorderLevel))
                || !ModelState.IsValidField(nameof(InventoryViewModel.stdPrice))
                || !ModelState.IsValidField(nameof(InventoryViewModel.balance))
                )
            {
                // drop down list 
                List<String> listItem = new List<string>();
                listItem.Add("Dozen");
                listItem.Add("Box");
                listItem.Add("Each");
                listItem.Add("Set");
                listItem.Add("Packet");
                ViewBag.unitMeasure = new SelectList(listItem, inventoryViewModel.unitMeasure);

                // drop down list 
                var categories = InventoryDAO.getAllCategoryId();
                ViewBag.category = new SelectList(categories, inventoryViewModel.category);

                return View(inventoryViewModel);
            }

            // biz logic here...
            bool bRes = AdjustmentVoucherDAO.EditInventories(inventoryViewModel);
            if (bRes)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        // GET: Inventories/Delete
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryViewModel inventoryViewModel = InventoryDAO.GetInventoryByItemNo(id);
            if (inventoryViewModel == null)
            {
                return HttpNotFound();
            }
            return View(inventoryViewModel);
        }

        // POST: Inventories/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            InventoryViewModel inventoryViewModel = InventoryDAO.GetInventoryByItemNo(id);
            bool result = AdjustmentVoucherDAO.DeleteInventories(inventoryViewModel);
            if (!result)
            {
                return View("~/Views/Shared/Error.cshtml");
            }

            return RedirectToAction("Index");
        }
    }
}
