namespace LogicUniversityApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Supplier")]
    public partial class Supplier
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Supplier()
        {
            Inventories = new HashSet<Inventory>();
            Inventories1 = new HashSet<Inventory>();
            Inventories2 = new HashSet<Inventory>();
            Pricings = new HashSet<Pricing>();
            PurchaseOrders = new HashSet<PurchaseOrder>();
        }

        [StringLength(25)]
        public string supplierId { get; set; }

        [Required]
        [StringLength(120)]
        public string supplierName { get; set; }

        [Required]
        [StringLength(120)]
        public string contactName { get; set; }

        public int phone { get; set; }

        public int fax { get; set; }

        [Required]
        public string address { get; set; }

        [Required]
        [StringLength(120)]
        public string email { get; set; }

        [StringLength(120)]
        public string gstNo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Inventory> Inventories { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Inventory> Inventories1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Inventory> Inventories2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pricing> Pricings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
