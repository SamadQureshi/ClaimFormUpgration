using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace TCO.TFM.WDMS.ViewModels.ViewModels
{
    public class ImgTravelModel
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
        public List<TravelExpenseVM> ImgLst { get; set; }

        public int OPDExpenseID { get; set; }

        public string ExpenseType { get; set; }


        [Required(ErrorMessage = "The Amount is required.")]
        [Display(Name = "Amount")]
        [Range(1, 10000000000000, ErrorMessage = "The Amount Must be between 1 and 10000.")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "The Description is required.")]
        [Display(Name = "Desciption")]
        public string Description { get; set; }

        public string OPDType { get; set; }
        #endregion

    }
}