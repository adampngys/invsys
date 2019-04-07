using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LogicUniversityApp.ViewModel
{
    public class PurchaseOrderViewModel
    {
        [DisplayName("Supplier ID")]
        public string supplierID { get; set; }

        [DisplayName("Supplier Name")]
        public string supplierName { get; set; }

        [DisplayName("Item Number")]
        public string itemNo { get; set; }

        [DisplayName("Item Description")]
        public string description { get; set; }

        [DisplayName("Quantity")]
        public int quantity { get; set; }

        [DisplayName("Reorder Level")]
        public int reorderLevel { get; set; }

        [DisplayName("Current Balance")]
        public int? balance { get; set; }

        [DisplayName("poId")]
        public int poId { get; set; }

        [DisplayName("Tender Price")]
        public decimal tenderPrice { get; set; }

        [DisplayName("Order Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime orderDate { get; set; }

        [DisplayName("Delivery Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime deliveryDate { get; set; }

        [DisplayName("PO Status")]
        public string status { get; set; }

        [DisplayName("Category")]
        public string category { get; set; }

        [DisplayName("Unit")]
        public string unitMeasure { get; set; }
    }
}