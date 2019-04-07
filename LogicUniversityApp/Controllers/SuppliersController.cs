using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LogicUniversityApp.BusinessLogic;
using LogicUniversityApp.Models;

namespace LogicUniversityApp.Controllers
{
    [Authorize(Roles = "SC,SS,SM")]
    public class SuppliersController : Controller
    {
        // GET: Suppliers
        public ActionResult Index()
        {
            return View(SupplierDAO.GetSupplierList());
        }

        // GET: Suppliers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = SupplierDAO.FindSupplierById(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: Suppliers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "supplierId,supplierName,contactName,phone,fax,address,email,gstNo")] Supplier supplier)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SupplierDAO.CreateSupplier(supplier);
                    return RedirectToAction("Index");
                }

                return View(supplier);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Error.cshtml");
            }

        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Supplier supplier = SupplierDAO.FindSupplierById(id);
                if (supplier == null)
                {
                    return HttpNotFound();
                }
                return View(supplier);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "supplierId,supplierName,contactName,phone,fax,address,email,gstNo")] Supplier supplier)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SupplierDAO.EditSupplier(supplier);
                    return RedirectToAction("Index");
                }
                return View(supplier);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        // GET: Suppliers/Delete/5
        public ActionResult Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Supplier supplier = SupplierDAO.FindSupplierById(id);
                if (supplier == null)
                {
                    return HttpNotFound();
                }
                return View(supplier);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Error.cshtml");
            }

        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                Supplier supplier = SupplierDAO.FindSupplierById(id);
                SupplierDAO.DeleteSupplier(supplier);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Error.cshtml");
            }

        }
    }
}