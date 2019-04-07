using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;
using Microsoft.AspNet.Identity;

namespace LogicUniversityApp.Controllers
{
    [Authorize(Roles = "DH,Temp DH,DR")]
    public class CollectionPointController : Controller
    {
        // GET: CollectionPoint/ChangeCollectionPoint
        [HttpGet]
        public ActionResult ChangeCollectionPoint()
        {
            string id = User.Identity.GetUserId();
            var staff = StaffDepartmentDAO.GetStaffByUserId(id);

            Department dep = staff.Department;
            DepartmentViewModel depView = StaffDepartmentDAO.ConvertDepartmentToDepartmentViewModel(dep);

            // Set CollectionPoint viewbag here!
            ViewBag.CollectionPoint = CollectionPointDAO.GetCollectionPoints();

            if (User.IsInRole("DH") || User.IsInRole("Temp DH"))
            {
                // get DR candidate list
                List<Staff> possibleList = StaffDepartmentDAO.FindPossibleDRList(staff);
                possibleList = possibleList == null ? new List<Staff>() : possibleList;
                Staff currentDR = StaffDepartmentDAO.FindDepartmentRole(staff.departmentId, "DR");

                // Set StaffList viewbag here!
                ViewBag.StaffList = new SelectList(
                    possibleList,
                    "staffId",
                    "staffName",
                    currentDR == null ? -1 : currentDR.staffId);
            }
            return View(depView);
        }

        // POST: /CollectionPoint/ChangeCollectionPoint
        [HttpPost]
        public ActionResult ChangeCollectionPoint(Department department, int chosenPoint, int? staffList, int? oldDR)
        {
            string id = User.Identity.GetUserId();
            var staff = StaffDepartmentDAO.GetStaffByUserId(id);
            Department dep = staff.Department;

            CollectionPointDAO.UpdateCollectionPoint(dep.departmentId, chosenPoint);

            if (User.IsInRole("DH") || User.IsInRole("Temp DH"))
            {
                if (staffList != null)
                {
                    StaffDepartmentDAO.changeDR(staff.staffId, (int)staffList);
                }
            }
            return View("~/Views/Home/Index.cshtml");
        }
    }
}