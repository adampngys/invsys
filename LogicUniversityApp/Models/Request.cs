namespace LogicUniversityApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Request")]
    public partial class Request
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Request()
        {
            RequestDetails = new HashSet<RequestDetail>();
        }

        [Key]
        public int requestId { get; set; }

        [Required]
        [StringLength(10)]
        public string departmentId { get; set; }

        public int staffId { get; set; }

        [Column(TypeName = "date")]
        public DateTime approvedDate { get; set; }

        [Required]
        [StringLength(15)]
        public string status { get; set; }

        public string remark { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequestDetail> RequestDetails { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
