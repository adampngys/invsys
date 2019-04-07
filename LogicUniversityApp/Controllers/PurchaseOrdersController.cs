using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;

namespace LogicUniversityApp.Controllers
{
    [Authorize(Roles = "SC,SS,SM")]
    public class PurchaseOrdersController : Controller
    {
        // GET: PurchaseOrders
        public ActionResult Index()
        {
            var povms = PurchaseOrderDAO.GetPoList();
            return View(povms);
        }

        // GET: PurchaseOrders/Details
        [HttpGet]
        public ActionResult Details(int? poId)
        {
            ViewBag.poId = poId;
            if (poId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var poDetails = PurchaseOrderDAO.GetPoDetailList((int)poId);
            if (poDetails == null)
            {
                return HttpNotFound();
            }
            return View(poDetails);
        }

        // GET: PurchaseOrders/Create
        [HttpGet]
        public ActionResult Create(string itemNo, string supplierId, int? poId, string category)
        {
            ViewBag.poId = poId;

            ViewBag.category = category;

            ViewBag.categoryList = new SelectList(InventoryDAO.getAllCategoryId(), category);

            if (itemNo != null && supplierId != null)
            {
                switch (poId)
                {
                    // new po
                    case null:
                        var li = Session["pocartItems"] as List<PurchaseOrderViewModel>;
                        PurchaseOrderDAO.IncreasePOList(itemNo, supplierId, ref li);
                        Session["pocartItems"] = li;
                        break;

                    // existed po
                    default:
                        int count = PurchaseOrderDAO.addPODetail(itemNo, (int)poId);
                        ViewBag.DetailItemCount = count;
                        break;
                }
            }

            List<PurchaseOrderViewModel> list = null;
            if (!string.IsNullOrEmpty(category) && category != "0")
            {
                // only for this category
                if (poId == null)
                {
                    list = PurchaseOrderDAO.getAllPOVM(category);
                }
                else
                {
                    list = PurchaseOrderDAO.getAllPOVM((int)poId, category);
                }
            }
            else
            {
                if (poId == null)
                {
                    list = PurchaseOrderDAO.getAllPOVM();
                }
                else
                {
                    list = PurchaseOrderDAO.getAllPOVM((int)poId);
                }
            }

            return View(list);
        }

        // POST: PurchaseOrders/Create
        [HttpPost]
        public ActionResult Create(int? poId)
        {
            ViewBag.poId = poId;
            if (poId == null)
            {
                var li = Session["pocartItems"] as List<PurchaseOrderViewModel>;
                if (li == null)
                {
                    return View("~/Views/Shared/Error.cshtml");
                }

                // Pass data from controller to controller
                TempData["ViewModelInfo"] = li;
            }

            return RedirectToAction("POCart", "PurchaseOrders", new { poId = poId });
        }

        // GET: PurchaseOrders/POCart
        [HttpGet]
        public ActionResult POCart(int? poId)
        {
            // Pass the requestid to view
            ViewBag.poId = poId;

            var poDetails = new List<PurchaseOrderViewModel>();
            ModelState.Clear();
            if (poId == null)
            {
                // Read from temp data                
                if (TempData.ContainsKey("ViewModelInfo"))
                {
                    poDetails = TempData["ViewModelInfo"] as List<PurchaseOrderViewModel>;
                }
                return View(poDetails);
            }
            else
            {
                // Read from database
                poDetails = PurchaseOrderDAO.GetPoDetailList((int)poId);
                return View(poDetails);
            }
        }

        // POST: PurchaseOrders/POCart
        [HttpPost]
        public ActionResult POCart(List<PurchaseOrderViewModel> Model, int? poId, string btn_itemNo)
        {
            ViewBag.poId = poId;

            string itemNo = null, supplierId = null;

            string[] strs = null;
            if (!string.IsNullOrEmpty(btn_itemNo))
            {
                strs = btn_itemNo.Split('/');
                if (strs != null && strs.Length == 2)
                {
                    itemNo = strs[0];
                    supplierId = strs[1];
                }
            }

            ViewBag.poId = poId;

            #region delete button click
            if (strs != null && strs.Length == 2)
            {
                if (Model == null || Model.Count == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    switch (poId)
                    {
                        // First time raise the request
                        case null:
                            var i1 = Model.Where(x => x.itemNo == itemNo && x.supplierID == supplierId).First();
                            Model.Remove(i1);
                            Session["pocartItems"] = Model;
                            ModelState.Clear();
                            return View(Model);

                        default:
                            PurchaseOrderDAO.DeletePODetail((int)poId, itemNo);
                            var poDetails = PurchaseOrderDAO.GetPoDetailList((int)poId);
                            ModelState.Clear();
                            return View(poDetails);
                    }

                }
            }
            #endregion

            #region submit button click
            if (Model == null || Model.Count == 0)
            {
                return View();
            }

            bool bRes = false;
            switch (poId)
            {
                // First time raise the request
                case null:
                    bRes = PurchaseOrderDAO.RaisePORequest(Model[0].orderDate, Model[0].deliveryDate, supplierId, Model);
                    if (bRes)
                    {
                        // Clear the session value
                        Session["pocartItems"] = null;
                    }
                    break;

                // Existed request
                default:
                    bRes = PurchaseOrderDAO.UpdatePORequestDetails(Model[0].orderDate, Model[0].deliveryDate, Model[0].status, Model, poId);
                    break;
            }

            if (bRes)
            {
                return RedirectToAction("index");
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml");
            }
            #endregion
        }

        // GET: PurchaseOrders/Edit
        [HttpGet]
        public ActionResult Edit(int? poId)
        {
            ViewBag.poId = poId;
            return RedirectToAction("Create", "PurchaseOrders", new { poId = poId });
        }

        // GET: PurchaseOrders/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrder purchaseOrder = PurchaseOrderDAO.findPoById(id);
            if (purchaseOrder == null)
            {
                return HttpNotFound();
            }
            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            PurchaseOrderDAO.deletePoById(id);
            return RedirectToAction("Index", "PurchaseOrders");
        }

        // GET: PurchaseOrders/ExportPurchaseOrder
        public ActionResult ExportPurchaseOrder(int poId)
        {
            var purchaseOrder = PurchaseOrderDAO.findPurchaseOrderList(poId);
            var purchaseOrderDetails = PurchaseOrderDAO.findPurchaseOrderDetail(poId);
            var inventoryList = PurchaseOrderDAO.findInventoriesInPurchaseOrder(poId);
            var supplierList = PurchaseOrderDAO.findSupplierInPurchaseOrder(poId);
            var price = PurchaseOrderDAO.findSupplierPrice(poId);

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~"), "CrystalReport2.rpt"));

            rd.Database.Tables[0].SetDataSource(purchaseOrderDetails);
            rd.Database.Tables[1].SetDataSource(purchaseOrder);
            rd.Database.Tables[2].SetDataSource(inventoryList);
            rd.Database.Tables[3].SetDataSource(supplierList);
            rd.Database.Tables[4].SetDataSource(price);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application1/pdf", string.Format("PurchaseOrderForm({0} - {1}).pdf", purchaseOrder.First().orderDate, purchaseOrder.First().Supplier.supplierName));
        }
    }
}
