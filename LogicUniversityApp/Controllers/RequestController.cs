using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;

namespace LogicUniversityApp.Controllers
{
    [Authorize(Roles = "DH,Temp DH,DR,Staff,SS")]
    public class RequestController : Controller
    {
        // GET: Request
        public ActionResult Index()
        {
            return View();
        }

        // GET: Request/List
        [Authorize]
        public ActionResult List()
        {
            string userId = User.Identity.GetUserId();
            var s1 = StaffDepartmentDAO.GetStaffByUserId(userId);
            var reqList = RequestDAO.GetRequests(s1).ToList();
            ModelState.Clear();
            return View(reqList);
        }

        // Get: /Request/ApproveReject
        [PrincipalPermission(SecurityAction.Demand, Role = "DH,Temp DH,SM")]
        [HttpGet]
        public ActionResult ApproveReject(string ShowHistory)
        {
            TempData["ShowHistory"] = "False";
            string userId = User.Identity.GetUserId();
            Staff s1 = StaffDepartmentDAO.GetStaffByUserId(userId);
            List<RequestViewModel> reqList = RequestDAO.GetPendingRequestsInDepartment(s1);
            if (ShowHistory == "True")
            {
                List<RequestViewModel> reqList0 = RequestDAO.GetHistoryRequestsInDepartment(s1);
                reqList = reqList.Union(reqList0).ToList();
                TempData["ShowHistory"] = "True";
            }
            return View(reqList);
        }

        // POST: /Request/ApproveReject
        [PrincipalPermission(SecurityAction.Demand, Role = "DH,Temp DH,SM")]
        [HttpPost]
        public ActionResult ApproveReject(string remarks, int? approveSelected, int? rejectSelected)
        {
            string userId = User.Identity.GetUserId();
            var s1 = StaffDepartmentDAO.GetStaffByUserId(userId);

            // If reject request is submitted
            if (rejectSelected != null)
            {
                RequestDAO.RejectRequest(rejectSelected, remarks);
                return RedirectToAction("ApproveReject");
            }
            else if (approveSelected != null)
            {
                RequestDAO.ApproveRequest(approveSelected, remarks);
                return RedirectToAction("ApproveReject");
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        // GET: /Request/Detail
        [HttpGet]
        public ActionResult Detail(int? requestId)
        {
            if (TempData.ContainsKey("ShowHistory"))
            {
                ViewBag.ShowHistory = TempData["ShowHistory"];
            }
            ViewBag.requestId = requestId;
            // Read from database
            var reqDetails = RequestDAO.GetRequestViewModelListByReqId((int)requestId);
            return View(reqDetails);
        }

        // GET: /Request/Delete
        [Authorize]
        [HttpGet]
        public ActionResult Delete(int requestId)
        {
            string userId = User.Identity.GetUserId();
            var s1 = StaffDepartmentDAO.GetStaffByUserId(userId);

            if (s1 != null)
            {
                // Delete the request
                RequestDAO.DeleteRequest(s1.staffId, requestId);
            }

            return RedirectToAction("List");
        }

        // GET: /Request/Store
        [Authorize]
        [HttpGet]
        public ActionResult Store(string itemNo, int? requestId, string category)
        {
            ViewBag.requestId = requestId;

            ViewBag.category = category;

            ViewBag.categoryList = new SelectList(InventoryDAO.getAllCategoryId(), category);

            // Click add to cart
            if (itemNo != null)
            {
                switch (requestId)
                {
                    // New request (store in session)
                    case null:
                        {
                            var li = Session["cartItems"] as List<string>;
                            IncreaseStringList(ref li, itemNo);
                            Session["cartItems"] = li;
                        }
                        break;

                    // Existed request (store in database
                    default:
                        {
                            // insert the request detail
                            RequestViewModel rd = new RequestViewModel()
                            {
                                itemNo = itemNo,
                                requestId = Convert.ToInt32(requestId),
                                quantityNeed = 1,
                                status_requestDetail = "unfulfilled"
                            };

                            int count = RequestDAO.InsertRequestDetail(rd);
                            ViewBag.RequestItemCount = count;
                        }
                        break;
                }
            }

            List<InventoryViewModel> items = null;
            if (!string.IsNullOrEmpty(category) && category != "0")
            {
                items = InventoryDAO.GetInventories(category);
            }
            else
            {
                items = InventoryDAO.GetInventories();
            }
            return View(items);
        }

        // POST: /Request/Store
        [Authorize]
        [HttpPost]
        public ActionResult Store(int? requestId)
        {
            if (requestId == null)
            {
                var li = Session["cartItems"] as List<string>;
                if (li == null)
                {
                    return View("~/Views/Shared/Error.cshtml");
                }
                var details = new List<RequestViewModel>();
                foreach (var itemNo in li)
                {
                    var detail = RequestDAO.CreateRequestViewModelByItemNumber(itemNo);
                    details.Add(detail);
                }

                // Pass data from controller to controller
                TempData["ViewModelInfo"] = details;
            }

            return RedirectToAction("DepartmentCart", "Request", new { requestId = requestId });
        }

        // GET: /Request/DepartmentCart
        [Authorize]
        [HttpGet]
        public ActionResult DepartmentCart(int? requestId)
        {
            // Pass the requestid to view
            ViewBag.requestId = requestId;

            var reqDetails = new List<RequestViewModel>();

            if (requestId == null)
            {
                // Read from temp data                
                if (TempData.ContainsKey("ViewModelInfo"))
                {
                    reqDetails = TempData["ViewModelInfo"] as List<RequestViewModel>;
                }
                return View(reqDetails);
            }
            else
            {
                // Read from database
                reqDetails = RequestDAO.GetRequestViewModelListByReqId((int)requestId);
                return View(reqDetails);
            }
        }

        // POST: /Request/DepartmentCart
        [Authorize]
        [HttpPost]
        public ActionResult DepartmentCart(List<RequestViewModel> Model, int? requestId, string btn_itemNo)
        {
            ViewBag.requestId = requestId;

            if (!ModelState.IsValid)
            {
                return View(Model);
            }

            // Get current staff
            string userId = User.Identity.GetUserId();
            var s1 = StaffDepartmentDAO.GetStaffByUserId(userId);

            #region delete button click
            if (btn_itemNo != null)
            {
                if (Model == null || Model.Count == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    switch (requestId)
                    {
                        // First time raise the request
                        case null:
                            var i1 = Model.Where(x => x.itemNo == btn_itemNo).First();
                            Model.Remove(i1);
                            ModelState.Clear();
                            Session["cartItems"] = Model.Select(x => x.itemNo).ToList();
                            return View(Model);

                        default:
                            var reqDetail = new RequestViewModel()
                            {
                                requestId = requestId,
                                itemNo = btn_itemNo
                            };
                            RequestDAO.DeleteRequestDetail(reqDetail);
                            var reqDetails = RequestDAO.GetRequestViewModelListByReqId((int)requestId);
                            ModelState.Clear();
                            return View(reqDetails);
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
            switch (requestId)
            {
                // First time raise the request
                case null:

                    //if (Model != null && Model.Count > 0)
                    //{
                    //    Model[0].staffId = s1.staffId;
                    //    Model[0].departmentId = s1.departmentId;
                    //}
                    bRes = RequestDAO.RaiseRequest(s1.staffId, s1.departmentId, Model);
                    if (bRes)
                    {
                        // Clear the session value
                        Session["cartItems"] = null;
                    }
                    break;

                // Existed request
                default:
                    bRes = RequestDAO.UpdateRequestDetails(Model);
                    break;
            }

            if (bRes)
            {
                return RedirectToAction("list");
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml");
            }
            #endregion
        }

        // function to check the duplicate input and increase the item list
        private void IncreaseStringList(ref List<string> strings, string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return;
            }

            if (strings == null)
            {
                strings = new List<string>();
            }

            // Loop through the list to check that the item is not exist now
            bool bExist = false;
            for (int i = 0; i < strings.Count; i++)
            {
                if (strings[i] == str)
                {
                    bExist = true;
                }
            }
            // prevent add same multiple times
            if (!bExist)
            {
                strings.Add(str);
            }
        }
    }
}