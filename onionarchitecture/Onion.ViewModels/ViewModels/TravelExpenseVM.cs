using Onion.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCO.TFM.WDMS.ViewModels.ViewModels
{
    public class TravelExpenseVM
    {
        public int ID { get; set; }     
        public string ExpenseType { get; set; }
        public decimal? Amount { get; set; }
        public string Description { get; set; }

        public string ImageName { get; set; }

        public string ImageExt { get; set; }

        public string ImageBase64 { get; set; }

        public int OpdExpenseId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public virtual OpdExpense OpdExpense { get; set; }
      

    }
}
