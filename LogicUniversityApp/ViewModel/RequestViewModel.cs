using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogicUniversityApp.ViewModel
{
    public class RequestViewModel
    {
        public int index_detail;

        [DisplayName("Request ID")]
        public int? requestId { get; set; }

        [DisplayName("Item Number")]
        public string itemNo { get; set; }

        [DisplayName("Item Name")]
        public string description { get; set; }

        [DisplayName("Price")]
        [DataType(DataType.Currency)]
        public decimal stdPrice { get; set; }
        
        [DisplayName("Unit")]
        public string unitMeasure { get; set; }

        [DisplayName("Category")]
        public string category { get; set; }

        [Range(1, 1000, ErrorMessage = "number should between 1 to 1000")]
        [DisplayName("Qty Need")]
        public int quantityNeed { get; set; }

        [DisplayName("Status")]
        public string status_request { get; set; }

        [DisplayName("Department ID")]
        public string departmentId { get; set; }

        [DisplayName("Staff ID")]
        public int staffId { get; set; }

        [DisplayName("Staff Name")]
        public string staffName { get; set; }

        [DisplayName("Approved Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime approvedDate { get; set; }

        [DisplayName("Status")]
        public string status_requestDetail { get; set; }

        [DisplayName("Remark")]
        public string remark { get; set; }
    }
}