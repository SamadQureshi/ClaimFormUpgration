using Onion.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
namespace TCO.TFM.WDMS.ViewModels.ViewModels
{
    public class OpdExpenseImageVM
    {
        public int ID { get; set; }
        public string ImageName { get; set; }

        public string ImageExt { get; set; }

        public string ImageBase64 { get; set; }

      
        public string NameExpenses { get; set; }


        public decimal? ExpenseAmount { get; set; }

        public int OpdExpenseId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public virtual OpdExpense OpdExpense { get; set; }
    }
}