using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogicUniversityApp.ViewModel
{
    public class AdjustmentVoucherViewModel
    {
        [DisplayName("Voucher ID")]
        public int voucherId { get; set; }

        [DisplayName("Quantity")]
        public int quantity { get; set; }

        [DisplayName("Remark")]
        public string remark { get; set; }

        [DisplayName("Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime date { get; set; }

        [DisplayName("ItemNo")]
        public string itemNo { get; set; }

        [DisplayName("Unit Price")]
        [DataType(DataType.Currency)]
        public decimal unitPrice { get; set; }

        [DisplayName("Item Name")]
        public string description { get; set; }

        [DisplayName("Total Amount")]
        [DataType(DataType.Currency)]
        public decimal totalamount { get; set; }

        public int year { get; set; }

        public int month { get; set; }
    }
}