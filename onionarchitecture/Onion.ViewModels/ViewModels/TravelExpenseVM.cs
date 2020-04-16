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
        public int OpdExpenseId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string Created_By { get; set; }
        public string Modified_By { get; set; }

        public string ManagerName { get; set; }

    }
}
