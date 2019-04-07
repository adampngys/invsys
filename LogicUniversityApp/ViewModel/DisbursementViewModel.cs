using LogicUniversityApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LogicUniversityApp.Common;

namespace LogicUniversityApp.ViewModel
{
    public class DisbursementViewModel
    {
        public string departmentId { get; set; }

        [DisplayName("Item Number")]
        public string itemNo { get; set; }

        [DisplayName("Stationery Description")]
        public string description { get; set; }

        [DisplayName("Unit")]
        public string unitMeasure { get; set; }

        [DisplayName("Prepared Qty")]
        public int qtyRequired { get; set; }

        //[Range(0, Int32.MaxValue, ErrorMessage = "number should be equal or larger than zero")]
        [DisplayName("Actual Qty")]
        [DisbursementRange(nameof(qtyRequired), nameof(qtyActual), nameof(qtyDamaged), nameof(qtyMissing))]
        public int qtyActual { get; set; }

        //[Range(0, Int32.MaxValue, ErrorMessage = "number should be equal or larger than zero")]
        [MyRange(nameof(qtyRequired), ErrorMessage = "Value must be between {0} and {1}")]
        [DisplayName("Damaged Qty")]
        public int qtyDamaged { get; set; }

        [MyRange(nameof(qtyRequired), ErrorMessage = "Value must be between {0} and {1}")]
        [DisplayName("Missing Qty")]
        public int qtyMissing { get; set; }
    }
}