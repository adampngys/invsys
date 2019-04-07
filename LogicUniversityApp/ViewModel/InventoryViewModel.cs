using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogicUniversityApp.ViewModel
{
    public class InventoryViewModel
    {
        [Required]
        [DisplayName("Item Number")]
        public string itemNo { get; set; }

        [Required]
        [DisplayName("Category")]
        public string category { get; set; }

        [Required]
        [DisplayName("Item Name")]
        public string description { get; set; }

        [Required]
        [DisplayName("Store Balance")]
        [Range(0, Int32.MaxValue)]
        public int? balance { get; set; }

        [Required]
        [DisplayName("Reorder Level")]
        [Range(1, Int32.MaxValue)]
        public int reorderLevel { get; set; }

        [Required]
        [DisplayName("Reorder Quantity")]
        [Range(1, Int32.MaxValue)]
        public int reorderQuantity { get; set; }

        [Required]
        [DisplayName("Unit")]
        public string unitMeasure { get; set; }

        [Required]
        [DisplayName("Price")]
        [DataType(DataType.Currency)]
        [Range(0.1, Double.MaxValue)]
        public decimal stdPrice { get; set; }

        [Required]
        [DisplayName("Supplier 1")]
        public string supplierId1 { get; set; }

        [Required]
        [DisplayName("Supplier 2")]
        public string supplierId2 { get; set; }

        [Required]
        [DisplayName("Supplier 3")]
        public string supplierId3 { get; set; }
    }
}