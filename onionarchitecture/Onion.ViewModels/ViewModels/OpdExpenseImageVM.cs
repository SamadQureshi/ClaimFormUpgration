using Onion.Domain.Models;
using System;
using System.Collections.Generic;
using System.Web;
namespace TCO.TFM.WDMS.ViewModels.ViewModels
{
    public class OpdExpenseImageVM
    {
        public int IMAGE_ID { get; set; }

        public int? OPDEXPENSE_ID { get; set; }

        public string IMAGE_NAME { get; set; }

        public string IMAGE_EXT { get; set; }

        public string IMAGE_BASE64 { get; set; }

        public string NAME_EXPENSES { get; set; }

        public decimal? EXPENSE_AMOUNT { get; set; }
      
        public virtual OpdExpense OpdExpense { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string Created_By { get; set; }
        public string Modified_By { get; set; }
       
        
    }
}