using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;
using Microsoft.AspNet.Identity;

namespace LogicUniversityApp.Controllers
{
    [Authorize(Roles = "DH,Temp DH,DR,Staff")]
    [RoutePrefix("api/department")]
    public class DepartmentAPIController : ApiController
    {
        #region request(raise new, modify pending request, DH & Temp DH approve/reject
        [HttpGet]
        [Route("ReadRequest")]
        public List<RequestViewModel> ReadRequest()
        {
            var userId = User.Identity.GetUserId();
            Staff s1 = StaffDepartmentDAO.GetStaffByUserId(userId);

            var reqList = new List<RequestViewModel>();

            if (s1 != null)
            {
                reqList = RequestDAO.GetRequests(s1);
            }
            return reqList;
        }

        [HttpGet]
        [Route("ReadRequestDetailByReqId/{requestId}")]
        public List<RequestViewModel> ReadRequestDetailByReqId(int requestId)
        {
            var reqDetails = RequestDAO.GetRequestViewModelListByReqId((int)requestId);
            return reqDetails;
        }

        [HttpGet]
        [Route("ReadCategories")]
        public List<string> ReadCategories()
        {
            var res = InventoryDAO.getAllCategoryId();
            return res;
        }

        [HttpGet]
        [Route("ReadInventory")]
        public List<InventoryViewModel> ReadInventory()
        {
            var list = InventoryDAO.GetInventories();
            return list;
        }

        [HttpGet]
        [Route("ReadInventory/{categoryId}")]
        public List<InventoryViewModel> ReadInventory(string categoryId)
        {
            var list = InventoryDAO.GetInventories(categoryId);
            return list;
        }

        [HttpGet]
        [Route("ListRequestsByDepartmentId")]
        public List<RequestViewModel> ListRequestsByDepartmentId()
        {
            string userId = User.Identity.GetUserId();
            Staff s1 = StaffDepartmentDAO.GetStaffByUserId(userId);
            if (s1 == null)
            {
                return null;
            }

            List<RequestViewModel> reqList = RequestDAO.GetPendingRequestsInDepartment(s1);
            return reqList;
        }

        [HttpPost]
        [Route("RaiseNewRequest")]
        public bool RaiseNewRequest([FromBody]List<RequestViewModel> reqDetails)
        {
            if (reqDetails == null || reqDetails.Count < 1)
            {
                return false;
            }
            string userId = User.Identity.GetUserId();
            Staff s1 = StaffDepartmentDAO.GetStaffByUserId(userId);
            if (s1 == null)
            {
                return false;
            }

            bool bRes = RequestDAO.RaiseRequest(s1.staffId, s1.departmentId, reqDetails);
            return bRes;
        }

        [HttpPost]
        [Route("insertRequestDetail")]
        public void InsertRequestDetail([FromBody]RequestViewModel reqDetail)
        {
            if (reqDetail != null)
            {
                RequestDAO.InsertRequestDetail(reqDetail);
            }
        }

        [HttpPost]
        [Route("deleteRequestDetail")]
        public void DeleteRequestDetail([FromBody]RequestViewModel reqDetail)
        {
            if (reqDetail != null)
            {
                RequestDAO.DeleteRequestDetail(reqDetail);
            }
        }

        [HttpPost]
        [Route("updateRequestDetails")]
        public void UpdateRequestDetails([FromBody]List<RequestViewModel> reqDetails)
        {
            if (reqDetails != null)
            {
                RequestDAO.UpdateRequestDetails(reqDetails);
            }
        }

        [Authorize(Roles = "DH,Temp DH,SM")]
        [HttpPost]
        [Route("updateApproveRejectStatus")]
        public void updateApproveRejectStatus([FromBody]RequestViewModel request)
        {
            if (request != null && request.requestId != null && request.status_request != null)
            {
                if (request.status_request.ToUpper() == "APPROVED")
                {
                    RequestDAO.ApproveRequest(request.requestId, request.remark);
                }
                else if (request.status_request.ToUpper() == "REJECTED")
                {
                    RequestDAO.RejectRequest(request.requestId, request.remark);
                }
            }
        }
        #endregion

        #region role assignment and change collection point

        [Authorize(Roles = "DH,Temp DH")]
        [HttpGet]
        [Route("FindPossibleDRList")]
        public List<StaffViewModel> FindPossibleDRList()
        {
            string userId = User.Identity.GetUserId();
            var s1 = StaffDepartmentDAO.GetStaffByUserId(userId);

            // find the possible DR candidate
            List<Staff> staffList = StaffDepartmentDAO.FindPossibleDRList(s1);

            // convert to plain data
            var svms = new List<StaffViewModel>();
            if (staffList != null)
            {
                foreach (var s in staffList)
                {
                    var svm = StaffDepartmentDAO.staffViewModelConverter(s);
                    svms.Add(svm);
                }
            }
            return svms;
        }

        [Authorize(Roles = "DH,Temp DH")]
        [HttpPost]
        [Route("saveDepartmentRep")]
        public bool saveDepartmentRep(StaffViewModel newDR)
        {
            if (newDR == null)
                return false;

            string userId = User.Identity.GetUserId();
            Staff s1 = StaffDepartmentDAO.GetStaffByUserId(userId);
            if (s1 == null)
            {
                return false;
            }

            bool bRes = StaffDepartmentDAO.changeDR(s1.staffId, newDR.staffId);
            return bRes;
        }

        [Authorize(Roles = "DH,Temp DH,DR")]
        [HttpGet]
        [Route("ListAllCollectionPoints")]
        public List<CollectionPointViewModel> ListAllCollectionPoints()
        {
            var result = CollectionPointDAO.GetCollectionPoints();
            return result;
        }

        [Authorize(Roles = "DH,Temp DH,DR")]
        [HttpPost]
        [Route("saveCollectionPoint")]
        public void saveCollectionPoint(DepartmentViewModel department)
        {
            string userId = User.Identity.GetUserId();
            Staff s1 = StaffDepartmentDAO.GetStaffByUserId(userId);
            if (s1 != null)
            {
                int cId = department.collectionPointId;
                CollectionPointDAO.UpdateCollectionPoint(s1.departmentId, cId);
            }
        }

        [Authorize(Roles = "DH, Temp DH, DR")]
        [HttpGet]
        [Route("departmentDetails")]
        public DepartmentViewModel departmentDetails()
        {
            string userId = User.Identity.GetUserId();
            Staff s1 = StaffDepartmentDAO.GetStaffByUserId(userId);
            if (s1 == null)
            {
                return null;
            }
            Department dep = DepartmentDAO.GetDepartmentByUserId(userId);
            return StaffDepartmentDAO.ConvertDepartmentToDepartmentViewModel(dep);
        }


        #endregion
    }
}
