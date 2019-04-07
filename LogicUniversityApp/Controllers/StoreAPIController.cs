using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;

namespace LogicUniversityApp.Controllers
{
    [Authorize(Roles = "SC,SS,SM")]
    [RoutePrefix("api/store")]
    public class StoreAPIController : ApiController
    {
        #region adjustment voucher

        [HttpGet]
        [Route("FindGeneralAdjustmentVoucher/{year}/{month}")]
        public List<AdjustmentVoucherViewModel> FindGeneralAdjustmentVoucher(int year, int month)
        {
            var list = AdjustmentVoucherDAO.FindGeneralAdjustmentVoucher(year, month);
            return list;
        }

        // itemNo, itemNo, remark
        [HttpPost]
        [Route("CreateAdjustmentVoucher")]
        public bool CreateAdjustmentVoucher([FromBody]AdjustmentVoucherViewModel model)
        {
            bool bRes = AdjustmentVoucherDAO.CreateAdjustmentVoucher(model.itemNo, model.quantity, DateTime.Now.Date, MyReasonCode.Else, model.remark);
            return bRes;
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


        #endregion

        #region retrieval
        [HttpGet]
        [Route("ViewRetrievalGoods/{DueDate}")]
        public List<RetrievalViewModel> ViewRetrievalGoods(DateTime DueDate)
        {
            List<RetrievalViewModel> myModels = RequestDAO.ViewRetrievalGoods(DueDate);

            return myModels;
        }

        // http://localhost:51021/api/ViewRetrievalGood/C001/2019-01-28
        [HttpGet]
        [Route("ViewRetrievalGood/{itemNo}/{DueDate}")]
        public RetrievalViewModel ViewRetrievalGood(string itemNo, DateTime DueDate)
        {
            RetrievalViewModel myModel = RequestDAO.ViewRetrievalGood(itemNo, DueDate);

            return myModel;
        }

        [HttpPost]
        [Route("AllocateGoods")]
        public bool AllocateGoods([FromBody]RetrievalViewModel model)
        {
            bool bRes = RequestDAO.AllocateGoods(
                model.itemNo,
                model.DueDate,
                model.quantityRetrieval,
                model.quantityInstoreDamaged,
                model.quantityInstoreMissing);

            if (bRes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region disbursement
        [HttpGet]
        [Route("GetDepartments")]
        public List<DepartmentViewModel> GetDepartments()
        {
            List<DepartmentViewModel> myModels = DepartmentDAO.GetDepartments();

            return myModels;
        }

        [HttpGet]
        [Route("GetDisbursement/{departmentId}")]
        public List<DisbursementViewModel> GetDisbursement(string departmentId)
        {
            List<DisbursementViewModel> myModels = DisbursementDAO.GetDisbursement(departmentId);

            return myModels;
        }

        [HttpGet]
        [Route("GetDepartmentInfo/{departmentId}")]
        public DepartmentViewModel GetDepartmentInfo(string departmentId)
        {
            DepartmentViewModel myModels = DepartmentDAO.GetDepartmentInfo(departmentId);

            return myModels;
        }

        [HttpPost]
        [Route("ConfirmDisbursement")]
        public bool ConfirmDisbursement([FromBody]List<DisbursementViewModel> myModels)
        {
            bool bRes = DisbursementDAO.ConfirmDisbursement(myModels);

            return bRes;
        }

        #endregion

        #region purchase order
        [HttpGet]
        [Route("findPoById/{poId}")]
        public PurchaseOrderViewModel findPoById(int poId)
        {
            PurchaseOrderViewModel po = PurchaseOrderDAO.findPoViewModelById(poId);
            return po;
        }

        [HttpGet]
        [Route("GetPoList")]
        public List<PurchaseOrderViewModel> GetPoList()
        {
            List<PurchaseOrderViewModel> pos = PurchaseOrderDAO.GetPoList();
            return pos;
        }

        [HttpGet]
        [Route("GetPoDetailList/{poId}")]
        public List<PurchaseOrderViewModel> GetPoDetailList(int poId)
        {
            List<PurchaseOrderViewModel> poDetails = PurchaseOrderDAO.GetPoDetailList(poId);
            return poDetails;
        }

        [HttpGet]
        [Route("GetPoDetail/{poId}/{itemNo}")]
        public PurchaseOrderViewModel GetPoDetail(int poId, string itemNo)
        {
            PurchaseOrderViewModel poDetail = PurchaseOrderDAO.GetPoDetail(poId, itemNo);
            return poDetail;
        }

        [HttpPost]
        [Route("RaisePORequest")]
        public bool RaisePORequest([FromBody]List<PurchaseOrderViewModel> poDetails)
        {
            if (poDetails == null || poDetails.Count < 1)
            {
                return false;
            }

            bool bRes = PurchaseOrderDAO.RaisePORequest(
                poDetails[0].orderDate,
                poDetails[0].deliveryDate,
                poDetails[0].supplierID,
                poDetails);

            return bRes;
        }

        [HttpPost]
        [Route("UpdatePORequestDetails")]
        public bool UpdatePORequestDetails([FromBody]List<PurchaseOrderViewModel> poDetails)
        {
            if (poDetails == null || poDetails.Count < 1)
            {
                return false;
            }

            bool bRes = PurchaseOrderDAO.UpdatePORequestDetails(
                poDetails[0].orderDate,
                poDetails[0].deliveryDate,
                poDetails[0].status,
                poDetails,
                poDetails[0].poId);

            return bRes;
        }

        [HttpPost]
        [Route("UpdatePORequestDetail")]
        public bool UpdatePORequestDetail([FromBody]PurchaseOrderViewModel poDetails)
        {
            bool bRes = PurchaseOrderDAO.UpdatePORequestDetail(poDetails);

            return bRes;
        }


        [HttpPost]
        [Route("DeletePODetail")]
        public void DeletePODetail([FromBody]PurchaseOrderViewModel poDetail)
        {
            PurchaseOrderDAO.DeletePODetail(poDetail.poId, poDetail.itemNo);
        }

        [HttpPost]
        [Route("addPODetail")]
        public void addPODetail([FromBody]PurchaseOrderViewModel poDetail)
        {
            PurchaseOrderDAO.addPODetail(poDetail.itemNo, poDetail.poId);
        }

        #endregion
    }
}
