namespace LogicUniversityApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Inventory")]
    public partial class Inventory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Inventory()
        {
            AdjustmentVouchers = new HashSet<AdjustmentVoucher>();
            Pricings = new HashSet<Pricing>();
            PurchaseOrderDetails = new HashSet<PurchaseOrderDetail>();
            RequestDetails = new HashSet<RequestDetail>();
            StockCards = new HashSet<StockCard>();
        }

        [Key]
        [StringLength(10)]
        [DisplayName("Item Number")]
        public string itemNo { get; set; }

        [Required]
        [StringLength(10)]
        [DisplayName("Category")]
        public string category { get; set; }

        [Required]
        [DisplayName("Item")]
        public string description { get; set; }

        [DisplayName("Balance")]
        public int? balance { get; set; }

        [DisplayName("Reorder Level")]
        public int reorderLevel { get; set; }

        [DisplayName("Reorder Qty")]
        public int reorderQuantity { get; set; }

        [Required]
        [StringLength(20)]
        [DisplayName("Unit")]
        public string unitMeasure { get; set; }

        [DisplayName("Price")]
        public decimal stdPrice { get; set; }

        [Required]
        [StringLength(25)]
        [DisplayName("Supplier 1")]
        public string supplierId1 { get; set; }

        [Required]
        [StringLength(25)]
        [DisplayName("Supplier 2")]
        public string supplierId2 { get; set; }

        [Required]
        [StringLength(25)]
        [DisplayName("Supplier 3")]
        public string supplierId3 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdjustmentVoucher> AdjustmentVouchers { get; set; }

        public virtual Category Category1 { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual Supplier Supplier1 { get; set; }

        public virtual Supplier Supplier2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pricing> Pricings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequestDetail> RequestDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockCard> StockCards { get; set; }
    }
}
