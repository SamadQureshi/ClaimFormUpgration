

using System.ComponentModel.DataAnnotations;

namespace Onion.Domain.Models
{
    public class ExpenseType : BaseEntity
    {
        [StringLength(50)]
        public string ExpenseName { get; set; }

    }
}
