using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogicUniversityApp.Controllers
{
    [Authorize(Roles = "SC")]
    public class DisbursementController : Controller
    {
        // GET: Disbursement
        public ActionResult Index(string depId)
        {
            List<Department> deps = DepartmentDAO.GetDepartmentsToDisburse();

            ViewBag.depIdList = new SelectList(deps,
                nameof(Department.departmentId),
                nameof(Department.departmentName), depId);

            List<DisbursementViewModel> model = new List<DisbursementViewModel>();
            if (depId != null)
            {
                model = DisbursementDAO.GetDisbursement(depId);
                ViewBag.depInfo = DepartmentDAO.GetDepartmentInfo(depId);
            }

            return View(model);
        }

        // POST: Disbursement
        [HttpPost]
        public ActionResult Index(List<DisbursementViewModel> model)
        {
            List<Department> deps = DepartmentDAO.GetDepartmentsToDisburse();

            ViewBag.depIdList = new SelectList(deps,
                nameof(Department.departmentId),
                nameof(Department.departmentName));

            if (model != null && model.Count > 0)
            {
                if (model[0].departmentId != null)
                {
                    ViewBag.depInfo = DepartmentDAO.GetDepartmentInfo(model[0].departmentId);
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool bRes = DisbursementDAO.ConfirmDisbursement(model);
            //ModelState.Clear();
            if (bRes)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }
    }
}