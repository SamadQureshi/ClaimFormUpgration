using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace TCO.TFM.WDMS.ViewModels.ViewModels
{
    public class ImgViewModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets Image file.
        /// </summary>
        [Required(ErrorMessage = "The File Upload is required.")]
        [Display(Name = "Upload File")]
        public HttpPostedFileBase FileAttach { get; set; }

        /// <summary>
        /// Gets or sets Image file list.
        /// </summary>
        public List<OpdExpenseImageVM> ImgLst { get; set; }

        public int OPDExpenseID { get; set; }

        [Required(ErrorMessage = "The Receipt Amount is required.")]
        [Display(Name = "Expense Amount")]
        [Range(1, 1000000, ErrorMessage = "The Receipt Amount Must be between 1 and 1000000.")]
        public int ExpenseAmount { get; set; }

        [Required(ErrorMessage = "The Receipt Name is required.")]
        [Display(Name = "Expense Name")]
        public string ExpenseName { get; set; }

        public string OPDType { get; set; }
        #endregion

    }
}