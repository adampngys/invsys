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
    public class RetrievalController : Controller
    {
        // GET: Retrieval
        [HttpGet]
        public ActionResult Index(string DueDate)
        {
            List<RetrievalViewModel> Res = new List<RetrievalViewModel>();

            if (DueDate != null)
            {
                if (DateTime.TryParse(DueDate, out DateTime myDate))
                {
                    ViewBag.myDate = DueDate;
                    Res = RequestDAO.ViewRetrievalGoods(myDate);
                }
            }
            else
            {
                // the default date is today
                DateTime myDate = DateTime.Now.Date;
                ViewBag.myDate = myDate.ToString("yyyy/MM/dd");
                Res = RequestDAO.ViewRetrievalGoods(myDate);
            }
            return View(Res);
        }

        // GET: Retrieval/Edit
        public ActionResult Edit(string itemNo, DateTime DueDate)
        {
            var myModel = RequestDAO.ViewRetrievalGood(itemNo, DueDate);

            return View(myModel);
        }

        // POST: Retrieval/Edit
        [HttpPost]
        public ActionResult Edit(RetrievalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool bRes = RequestDAO.AllocateGoods(
                model.itemNo,
                model.DueDate,
                model.quantityRetrieval,
                model.quantityInstoreDamaged,
                model.quantityInstoreMissing);

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