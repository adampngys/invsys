namespace LogicUniversityApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Department")]
    public partial class Department
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Department()
        {
            RequestLists = new HashSet<Request>();
            Staffs = new HashSet<Staff>();
        }

        [StringLength(10)]
        public string departmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string departmentName { get; set; }

        public int collectionPointId { get; set; }

        public int staffIdDH { get; set; }

        public int staffIdDR { get; set; }

        public int staffIdContact { get; set; }

        [Range(0000000, 9999999)]
        public int departmentFax { get; set; }

        [Range(0000000, 9999999)]
        public int departmentPhone { get; set; }

        public virtual CollectionPoint CollectionPoint { get; set; }

        public virtual Staff Staff { get; set; }

        public virtual Staff Staff1 { get; set; }

        public virtual Staff Staff2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request> RequestLists { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Staff> Staffs { get; set; }
    }
}
