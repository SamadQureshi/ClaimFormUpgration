using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.WebApp.Models
{
    public class ImgViewModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets Image file.
        /// </summary>
        [Required]
        [Display(Name = "Upload File")]
        public HttpPostedFileBase FileAttach { get; set; }

        /// <summary>
        /// Gets or sets Image file list.
        /// </summary>
        public List<OpdExpense_ImageVM> ImgLst { get; set; }

        public int OPDExpense_ID { get; set; }

        [Required]
        [Display(Name = "Expense Amount")]
        [Range(1, 10000000000000)]
        public int ExpenseAmount { get; set; }

        [Required]
        [Display(Name = "Expense Name")]
        public string ExpenseName { get; set; }

        public string OPDType { get; set; }
        #endregion

    }
}