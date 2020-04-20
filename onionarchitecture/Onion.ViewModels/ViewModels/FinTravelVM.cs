using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCO.TFM.WDMS.ViewModels.ViewModels
{
    public class FinTravelVM : TravelExpenseMasterDetail
    {
        [Required(ErrorMessage = "The Finance Comments is required.")]
        public new string FinanceComment { get; set; }

        [Required(ErrorMessage = "The Approved Amount is required.")]
        public new decimal? TotalAmountApproved { get; set; }

        [Required(ErrorMessage = "The Manager Name is required.")]
        public new string ManagerName { get; set; }

    }
}
