namespace LogicUniversityApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PurchaseOrderDetail")]
    public partial class PurchaseOrderDetail
    {
        [Key]
        public int index { get; set; }

        public int poId { get; set; }

        [Required]
        [StringLength(10)]
        public string itemNo { get; set; }

        public int quantity { get; set; }

        public virtual Inventory Inventory { get; set; }

        public virtual PurchaseOrder PurchaseOrder { get; set; }
    }
}
