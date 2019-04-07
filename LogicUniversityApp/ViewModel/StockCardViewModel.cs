using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogicUniversityApp.ViewModel
{
    public class StockCardViewModel
    {
        [Key]
        [DisplayName("Index")]
        public int index { get; set; }

        [DisplayName("ItemNo")]
        public string itemNo { get; set; }

        [DisplayName("Category")]
        public string category { get; set; }

        [DisplayName("Description")]
        public string description { get; set; }

        [DisplayName("Balance")]
        public int? balance { get; set; }

        [DisplayName("Reason")]
        public string remark { get; set; }

        [DisplayName("Quantity")]
        public int quantity { get; set; }

        [DisplayName("Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime dateModified { get; set; }
    }
}