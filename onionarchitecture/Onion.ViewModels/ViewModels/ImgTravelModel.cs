using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Onion.Common.Utils;

namespace TCO.TFM.WDMS.ViewModels.ViewModels
{
    public class ImgTravelModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets Image file.
        /// </summary>
        [Required(ErrorMessage = "The File Upload is required.")]
        [Display(Name = "Supported Files .png | .jpg | .xlsx | .docs | .pdf | .gif")]
        [AllowExtensions(Extensions = "png,jpg,xlsx,docs,pdf,gif", ErrorMessage = "Please select only Supported Files .png | .jpg | .xlsx | .docs | .pdf | .gif ")]
        public HttpPostedFileBase FileAttach { get; set; }

        /// <summary>
        /// Gets or sets Image file list.
        /// </summary>
        public List<TravelExpenseVM> ImgLst { get; set; }

        public int OPDExpenseID { get; set; }

        public string ExpenseType { get; set; }


        [Required(ErrorMessage = "The Amount is required.")]
        [Display(Name = "Amount")]
        [Range(1, 1000000, ErrorMessage = "The Amount Must be between 1 and 1000000.")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "The Description is required.")]
        [Display(Name = "Desciption")]
        public string Description { get; set; }

        public string OPDType { get; set; }
        #endregion

    }
}