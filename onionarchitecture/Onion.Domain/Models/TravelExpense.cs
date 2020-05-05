using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Domain.Models
{
[TrackChanges]
    public class TravelExpense : BaseEntity
    {
        [SkipTracking]
        public string ImageName { get; set; }
        [SkipTracking]
        public string ImageExt { get; set; }
        [SkipTracking]
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
