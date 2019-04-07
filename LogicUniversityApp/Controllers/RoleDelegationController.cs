using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogicUniversityApp.Models;
using Microsoft.AspNet.Identity;

namespace LogicUniversityApp.Controllers
{
    [Authorize(Roles = "DH,Temp DH,DR,SM,SS")]
    public class RoleDelegationController : Controller
    {
        // GET: RoleDelegation/RoleDelegation
        [HttpGet]
        public ActionResult RoleDelegation()
        {
            string userId = User.Identity.GetUserId();
            Staff currentUser = StaffDepartmentDAO.GetStaffByUserId(userId);
            //Find all staff that are not DH and DR
            List<Staff> staffList = StaffDepartmentDAO.FindAllStaffInDepartment(currentUser.Department.departmentId).Where(x => x.staffId != currentUser.Department.staffIdDH && x.staffId != currentUser.Department.staffIdDR).ToList();
            Staff delegatedStaff = StaffDepartmentDAO.GetDelegatedStaff(currentUser.Department);

            //if no staff had been delegated for the period the viewbag will be null
            ViewBag.DelegatedStaff = delegatedStaff;

            return View(staffList);
        }

        // POST: RoleDelegation/RoleDelegation
        [HttpPost]
        public ActionResult RoleDelegation(List<Staff> Model, int? staffSelected, string removeAuthority, string submit, string startDate, string endDate, int? staffChosen)
        {
            string userId = User.Identity.GetUserId();
            Staff currentUser = StaffDepartmentDAO.GetStaffByUserId(userId);

            // If remove authority button is clicked
            if (removeAuthority != null)
            {
                Staff delegatedStaff = StaffDepartmentDAO.GetDelegatedStaff(currentUser.Department);
                StaffDepartmentDAO.RemoveAuthority(delegatedStaff);

                // Return updated model
                //List<Staff> staffList = StaffDepartmentDAO.FindAllStaffInDepartment(currentUser.Department.departmentId).Where(x => x.staffId != currentUser.Department.staffIdDH && x.staffId != currentUser.Department.staffIdDR).ToList();

                return RedirectToAction("RoleDelegation");
            }

            // If submit button is clicked
            if (submit != null && DateTime.TryParse(startDate, out DateTime d_start) && DateTime.TryParse(endDate, out DateTime d_end))
            {
                StaffDepartmentDAO.GiveAuthority(staffChosen, d_start, d_end);

                return RedirectToAction("RoleDelegation");
            }

            ViewBag.DelegatedStaff = StaffDepartmentDAO.GetStaffByStaffId(staffSelected);
            return View(Model);
        }
    }
}