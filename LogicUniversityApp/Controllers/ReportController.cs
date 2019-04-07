using CrystalDecisions.CrystalReports.Engine;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogicUniversityApp.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        // To see the analysis report per department
        [Authorize]
        public ActionResult TrendAnalysisReport()
        {
            string userId = User.Identity.GetUserId();
            var s1 = StaffDepartmentDAO.GetStaffByUserId(userId);
            var department = s1.departmentId;
            switch (department)
            {
                case ("ARTS"):
                    {
                        return RedirectToAction("ArtsReport");
                    }
                case ("BUSN"):
                    {
                        return RedirectToAction("BusinessReport");
                    }
                case ("COMM"):
                    {
                        return RedirectToAction("CommerceReport");
                    }
                case ("CPSC"):
                    {
                        return RedirectToAction("ComputerScienceReport");
                    }
                case ("ENGL"):
                    {
                        return RedirectToAction("EnglishReport");
                    }
                case ("FINA"):
                    {
                        return RedirectToAction("FinanceReport");
                    }
                case ("MATH"):
                    {
                        return RedirectToAction("MathematicReport");
                    }
                case ("MEDC"):
                    {
                        return RedirectToAction("MedicineReport");
                    }
                case ("REGR"):
                    {
                        return RedirectToAction("RegistrarReport");
                    }
                case ("ZOOL"):
                    {
                        return RedirectToAction("ZoologyReport");
                    }
                default:
                    {
                        return View("~/Views/Shared/Error.cshtml");
                    }
            }
        }

        [HttpGet]
        public ActionResult ChargeBackReport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChargeBackReport(string submit)
        {
            return Redirect("https://app.powerbi.com/groups/me/reports/bea4487c-12d0-4d7f-b71c-b0930e25d649/ReportSection");
        }

        // Store Trend Analysis
        public ActionResult StoreTrendAnalysis()
        {
            return View();
        }


        #region Redirecting report to its respective view

        public ActionResult ArtsReport()
        {
            return View();
        }

        public ActionResult BusinessReport()
        {
            return View();
        }

        public ActionResult CommerceReport()
        {
            return View();
        }

        public ActionResult ComputerScienceReport()
        {
            return View();
        }

        public ActionResult EnglishReport()
        {
            return View();
        }

        public ActionResult FinanceReport()
        {
            return View();
        }

        public ActionResult MathematicReport()
        {
            return View();
        }

        public ActionResult MedicineReport()
        {
            return View();
        }

        public ActionResult RegistrarReport()
        {
            return View();
        }

        public ActionResult ZoologyReport()
        {
            return View();
        }

        public ActionResult PurchaseReport()
        {
            return View();
        }
        #endregion
    }
}