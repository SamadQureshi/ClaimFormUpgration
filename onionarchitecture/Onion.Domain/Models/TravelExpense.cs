using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Domain.Models
{
    public class TravelExpense : BaseEntity
    {
        public string ImageName { get; set; }

        public string ImageExt { get; set; }

        public string ImageBase64 { get; set; }

        [StringLength(100)]
        public string ExpenseType { get; set; }

        public decimal? Amount { get; set; }

        [StringLength(5000)]
        public string Description { get; set; }

        public int OpdExpenseId { get; set; }

        [ForeignKey("OpdExpenseId")]
        public virtual OpdExpense OpdExpense { get; set; }



    }
}
