using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;


namespace LogicUniversityApp.ViewModel
{
    public class DepartmentViewModel
    {
        [DisplayName("Department ID")]
        public string departmentId { get; set; }

        [DisplayName("Department Name")]
        public string departmentName { get; set; }

        [DisplayName("Collection Point Id")]
        public int collectionPointId { get; set; }

        [DisplayName("Collection Point")]
        public string collectionPointDescription { get; set; }

        [DisplayName("departmentRepName")]
        public string departmentRepName { get; set; }

        [DisplayName("Staff Id DH")]
        public int staffIdDH { get; set; }

        [DisplayName("DH Name")]
        public string DHName { get; set; }

        [DisplayName("Staff Id DR")]
        public int staffIdDR { get; set; }

        [DisplayName("DR Name")]
        public string DRName { get; set; }

        [DisplayName("Staff Id Contact")]
        public int staffIdContact { get; set; }

        [DisplayName("Contact Name")]
        public string ContactName { get; set; }

        [DisplayName("Department Fax")]
        public int departmentFax { get; set; }

        [DisplayName("Department Phone")]
        public int departmentPhone { get; set; }
    }
}