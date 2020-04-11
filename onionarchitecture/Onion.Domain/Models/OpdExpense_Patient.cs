namespace Onion.Domain.Models
{

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public class OpdExpense_Patient : BaseEntity
    {
        [Key]
        public int ID { get; set; }

        [StringLength(100)]
        public string NAME { get; set; }

        public int? AGE { get; set; }

        [Required]
        [StringLength(50)]
        public string RELATIONSHIP_EMPLOYEE { get; set; }

        
        public int OPDEXPENSE_ID { get; set; }

        [ForeignKey("OPDEXPENSE_ID")]
        public virtual OpdExpense OpdExpense { get; set; }

    }
}
