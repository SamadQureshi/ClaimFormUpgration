namespace Onion.Domain.Models
{

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public class OpdExpensePatient : BaseEntity
    {

        [StringLength(100)]
        public string Name{ get; set; }

        public int? Age { get; set; }

        [Required]
        [StringLength(50)]
        public string RelationshipEmployee { get; set; }

        public int OpdExpenseId { get; set; }

        [ForeignKey("OpdExpenseId")]
        public virtual OpdExpense OpdExpense { get; set; }

    }
}
