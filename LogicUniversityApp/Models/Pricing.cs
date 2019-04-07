namespace LogicUniversityApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Pricing")]
    public partial class Pricing
    {
        [Key]
        public int index { get; set; }

        [Required]
        [StringLength(10)]
        public string itemNo { get; set; }

        [Required]
        [StringLength(25)]
        public string supplierId { get; set; }

        [Required]
        [StringLength(10)]
        public string category { get; set; }

        public decimal tenderPrice { get; set; }

        public virtual Category Category1 { get; set; }

        public virtual Inventory Inventory { get; set; }

        public virtual Supplier Supplier { get; set; }
    }
}
