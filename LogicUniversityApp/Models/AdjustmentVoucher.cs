namespace LogicUniversityApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AdjustmentVoucher")]
    public partial class AdjustmentVoucher
    {
        [Key]
        public int voucherId { get; set; }

        [Required]
        [StringLength(10)]
        public string itemNo { get; set; }

        public int quantity { get; set; }

        [Required]
        public string remark { get; set; }

        [Column(TypeName = "date")]
        public DateTime date { get; set; }

        public virtual Inventory Inventory { get; set; }
    }
}
