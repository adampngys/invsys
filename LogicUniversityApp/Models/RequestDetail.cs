namespace LogicUniversityApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RequestDetail")]
    public partial class RequestDetail
    {
        [Key]
        public int index { get; set; }

        public int requestId { get; set; }

        [Required]
        [StringLength(10)]
        public string itemNo { get; set; }
        
        public int quantityNeed { get; set; }

        public int quantityReceive { get; set; }

        public int quantityPacked { get; set; }

        [Required]
        [StringLength(15)]
        public string status { get; set; }

        public virtual Inventory Inventory { get; set; }

        public virtual Request Request { get; set; }
    }
}
