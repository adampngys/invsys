using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LogicUniversityApp.Common;

namespace LogicUniversityApp.ViewModel
{
    public class RetrievalViewModel
    {
        [DataType(DataType.Date)]
        [DisplayName("Due Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }

        [DisplayName("Item Number")]
        public string itemNo { get; set; }

        [DisplayName("Item Name")]
        public string description { get; set; }

        [DisplayName("Unit")]
        public string unitMeasure { get; set; }

        [DisplayName("Category")]
        public string categoryId { get; set; }

        [DisplayName("Bin")]
        public string location { get; set; }

        [DisplayName("Current Balance")]
        public int balance { get; set; }

        [DisplayName("Request Quantity")]
        public int quantityTotalNeed { get; set; }

        [DisplayName("Retrieval Quantity")]
        [MySpecialRange(
            nameof(quantityTotalNeed)
            , nameof(balance)
            , nameof(quantityRetrieval)
            , nameof(quantityInstoreDamaged)
            , nameof(quantityInstoreMissing))]
        public int quantityRetrieval { get; set; }

        [DisplayName("Instore Damaged")]
        [MyRange(nameof(balance), ErrorMessage = "Value must be between {0} and {1}")]
        public int quantityInstoreDamaged { get; set; }

        [DisplayName("Instore Missing")]
        [MyRange(nameof(balance), ErrorMessage = "Value must be between {0} and {1}")]
        public int quantityInstoreMissing { get; set; }
    }
}