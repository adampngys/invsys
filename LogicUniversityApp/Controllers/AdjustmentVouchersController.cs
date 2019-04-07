using CrystalDecisions.CrystalReports.Engine;
using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace LogicUniversityApp.Controllers
{
    [Authorize]
    public class AdjustmentVouchersController : Controller
    {
        // GET: AdjustmentVouchers/ViewTotal
        public ActionResult ViewTotal(string month, string year)
        {
            DateTime year0 = DateTime.Today;
            DateTime year1 = DateTime.Today.AddYears(-1);
            DateTime year2 = DateTime.Today.AddYears(-2);

            List<SelectListItem> listItem1 = new List<SelectListItem>
            {
                new SelectListItem{Text="WHOLE YEAR",Value="WHOLE YEAR"},
                new SelectListItem{Text=year0.Year.ToString(),Value=year0.Year.ToString()},
                new SelectListItem{Text=year1.Year.ToString(),Value=year1.Year.ToString()},
                new SelectListItem{Text=year2.Year.ToString(),Value=year2.Year.ToString()}
             };

            List<SelectListItem> listItem2 = new List<SelectListItem>
            {
                new SelectListItem{Text="WHOLE MONTH",Value="WHOLE MONTH"},
                new SelectListItem{Text="January",Value="1"},
                new SelectListItem{Text="February",Value="2"},
                new SelectListItem{Text="March",Value="3"},
                new SelectListItem{Text="April",Value="4"},
                new SelectListItem{Text="May",Value="5"},
                new SelectListItem{Text="June",Value="6"},
                new SelectListItem{Text="July",Value="7"},
                new SelectListItem{Text="August",Value="8"},
                new SelectListItem{Text="September",Value="9"},
                new SelectListItem{Text="October",Value="10"},
                new SelectListItem{Text="November",Value="11"},
                new SelectListItem{Text="December",Value="12"}
             };

            ViewBag.year = new SelectList(listItem1, "Value", "Text");
            ViewBag.month = new SelectList(listItem2, "Value", "Text");

            // convert the data here...
            int Year = -1, Month = -1;

            if (Int32.TryParse(year, out int tempYear))
            {
                Year = tempYear;
            }

            if (Int32.TryParse(month, out int tempMonth))
            {
                Month = tempMonth;
            }

            // whether show the result
            if (Year != -1 && Month != -1)
            {
                ViewBag.showReport = true;
                ViewBag.tempYear = Year;
                ViewBag.tempMonth = Month;
            }
            else
            {
                ViewBag.showReport = null;
            }

            List<AdjustmentVoucherViewModel> myList = AdjustmentVoucherDAO.FindGeneralAdjustmentVoucher(Year, Month);
            if (myList.Count == 0)
            {
                ViewBag.showReport = null;
            }

            return View(myList);
        }

        // GET: AdjustmentVouchers/ViewDetail
        public ActionResult ViewDetail(string itemNo, int? year, int? month)
        {
            if (year == null || month == null)
            {
                return View(new List<AdjustmentVoucherViewModel>());
            }

            List<AdjustmentVoucherViewModel> list = AdjustmentVoucherDAO.FindDetailAdjustmentVoucher(itemNo, (int)year, (int)month);
            return View(list);
        }

        // GET: AdjustmentVouchers/Create
        public ActionResult Create()
        {
            //ViewBag.category = AdjustmentVoucherDAO.GetCategorySelectList();
            List<InventoryViewModel> items = InventoryDAO.GetInventories();
            ViewBag.stationeryList = new SelectList(items, "itemNo", "description");
            AdjustmentVoucherViewModel model = new AdjustmentVoucherViewModel();
            model.quantity = 1;
            model.remark = "0";
            return View(model);
        }

        // POST: AdjustmentVouchers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdjustmentVoucherViewModel adjustmentVoucherViewModel)
        {
            if (ModelState.IsValidField("quantity") && adjustmentVoucherViewModel.quantity != 0 && adjustmentVoucherViewModel.remark != null)
            {
                if (Int32.TryParse(adjustmentVoucherViewModel.remark, out int result))
                {
                    MyReasonCode myCode = (MyReasonCode)result;

                    // set the reasonable quantity
                    if (myCode == MyReasonCode.InstoreDamaged || myCode == MyReasonCode.InstoreMissing)
                    {
                        adjustmentVoucherViewModel.quantity = -Math.Abs(adjustmentVoucherViewModel.quantity);
                    }
                    else if (myCode == MyReasonCode.FreeOfCharge)
                    {
                        adjustmentVoucherViewModel.quantity = Math.Abs(adjustmentVoucherViewModel.quantity);
                    }

                    // create the voucher
                    AdjustmentVoucherDAO.CreateAdjustmentVoucher(adjustmentVoucherViewModel.itemNo, adjustmentVoucherViewModel.quantity, DateTime.Now.Date, myCode);
                    return RedirectToAction("ViewTotal");
                }
            }

            return RedirectToAction("Create");
        }

        // GET: AdjustmentVouchers/ExportAdjustmentVoucher
        public ActionResult ExportAdjustmentVoucher(int month, int year)
        {
            DateTime date = new DateTime(year, month, 1);
            var adjustmentVoucher = AdjustmentVoucherDAO.GetAdjustmentVouchersByDate(date);
            var inventoriesList = AdjustmentVoucherDAO.GetListOfInventories(date);

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~"), "CrystalReport3.rpt"));

            rd.Database.Tables[0].SetDataSource(adjustmentVoucher);
            rd.Database.Tables[1].SetDataSource(inventoriesList);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application1/pdf", String.Format("AdjustmentVoucher({0}-{1}).pdf", month, year));
        }

        // GET: AdjustmentVouchers/NotifyManager
        public ActionResult NotifyManager(int month, int year)
        {
            string mailSubject = "Notification for AdjustmentVoucher";
            string mailContent = "Please issue the AdjustmentVoucher in " + month + "/" + year;
            bool result = MyEmail.SendEmail("adampngys@gmail.com", mailSubject, mailContent);
            if (result)
            {
                return Content("<script>alert('Send Email Successful!');history.go(-1);</script>");
            }
            else
            {
                return Content("Send Email Failed ");
            }
        }
    }
}