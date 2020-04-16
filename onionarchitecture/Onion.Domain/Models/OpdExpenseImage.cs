namespace Onion.Domain.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class OpdExpenseImage : BaseEntity
    {          
        public string ImageName { get; set; }

        public string ImageExt { get; set; }

        public string ImageBase64 { get; set; }

        [StringLength(5000)]
        public string NameExpenses { get; set; }

        public decimal? ExpenseAmount { get; set; }

        public int OpdExpenseId { get; set; }

        [ForeignKey("OpdExpenseId")]
        public virtual OpdExpense OpdExpense { get; set; }

    }
}
