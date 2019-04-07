using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogicUniversityApp.ViewModel
{
    public class StaffViewModel
    {
        public int staffId { get; set; }

        public string staffName { get; set; }
        
        public string departmentId { get; set; }

        public bool delegatedStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? delegatedStartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? delegatedEndDate { get; set; }
    }
}