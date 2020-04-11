using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.WebApp.Models
{
    public class OpdExpense_MasterDetail
    {

        public OpdExpenseVM opdEXPENSE { get; set; }

        public List<OpdExpense_PatientVM> listOPDEXPENSEPATIENT { get; set; }

        public List<OpdExpense_ImageVM> listOPDEXPENSEIMAGE { get; set; }





    }
}