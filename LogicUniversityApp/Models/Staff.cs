namespace LogicUniversityApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Staff")]
    public partial class Staff
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Staff()
        {
            Departments = new HashSet<Department>();
            Departments1 = new HashSet<Department>();
            Departments2 = new HashSet<Department>();
            RequestLists = new HashSet<Request>();
        }

        public int staffId { get; set; }

        [Required]
        [StringLength(50)]
        public string staffName { get; set; }

        [Required]
        [StringLength(10)]
        public string departmentId { get; set; }

        public int staffPhone { get; set; }

        [Required]
        [StringLength(50)]
        public string staffEmail { get; set; }

        [StringLength(128)]
        public string userId { get; set; }

        public bool delegatedStatus { get; set; }

        [Column(TypeName = "date")]
        public DateTime? delegatedStartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? delegatedEndDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Department> Departments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Department> Departments1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Department> Departments2 { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request> RequestLists { get; set; }
    }
}
