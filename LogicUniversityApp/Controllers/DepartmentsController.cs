using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LogicUniversityApp.Models;
using Microsoft.AspNet.Identity;

namespace LogicUniversityApp.Controllers
{
    [Authorize(Roles = "DH,Temp DH,DR,SM,SS")]
    public class DepartmentsController : Controller
    {
        // GET: Departments/Details
        public ActionResult Details()
        {
            string userId = User.Identity.GetUserId();
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = DepartmentDAO.GetDepartmentByUserId(userId);
            if (department == null)
            {
                return HttpNotFound();
            }
            ModelState.Clear();
            return View(department);
        }

        // GET: Departments/Edit
        public ActionResult Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Department department = DbFactory.Instance.context.Departments.Find(id);
                if (department == null)
                {
                    return HttpNotFound();
                }
                List<Staff> staffList = StaffDepartmentDAO.FindAllStaffInDepartment(id);

                ViewBag.department = department.departmentName;

                ViewBag.staffIdContact = new SelectList(staffList, "staffId", "staffName", department.staffIdContact);
                return View(department);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        // POST: Departments/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string departmentId, int staffIdContact, int departmentPhone, int departmentFax)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DepartmentDAO.UpdateDepartmentContacts(departmentId, staffIdContact, departmentPhone, departmentFax);
                    return RedirectToAction("Details");
                }
                string userId = User.Identity.GetUserId();
                Staff staff = StaffDepartmentDAO.GetStaffByUserId(userId);

                Department department = DepartmentDAO.GetDepartmentByUserId(userId);

                List<Staff> staffList = StaffDepartmentDAO.FindAllStaffInDepartment(staff.departmentId);

                ViewBag.Department = staff.Department.departmentName;

                ViewBag.staffIdContact = new SelectList(staffList, "staffId", "staffName", department.staffIdContact);
                return View(department);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }
    }
}
