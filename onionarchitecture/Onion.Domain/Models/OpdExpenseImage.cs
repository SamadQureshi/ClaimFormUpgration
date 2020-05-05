namespace Onion.Domain.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
[TrackChanges]
    public class OpdExpenseImage : BaseEntity
    {
        [SkipTracking]
        public string ImageName { get; set; }
        [SkipTracking]
        public string ImageExt { get; set; }
        [SkipTracking]
        public string ImageBase64 { get; set; }

        [StringLength(5000)]
        public string NameExpenses { get; set; }

        public decimal? ExpenseAmount { get; set; }

        public int OpdExpenseId { get; set; }

        [ForeignKey("OpdExpenseId")]
        public virtual OpdExpense OpdExpense { get; set; }

    }
}
