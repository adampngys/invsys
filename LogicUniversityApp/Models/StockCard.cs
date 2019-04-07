namespace LogicUniversityApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StockCard")]
    public partial class StockCard
    {
        [Key]
        public int index { get; set; }

        [Required]
        [StringLength(10)]
        public string itemNo { get; set; }

        [Required]
        public string remark { get; set; }

        public int quantity { get; set; }

        public int balance { get; set; }

        [Column(TypeName = "date")]
        public DateTime dateModified { get; set; }

        public virtual Inventory Inventory { get; set; }
    }
}
