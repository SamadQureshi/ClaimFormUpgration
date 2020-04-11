namespace Onion.Domain.Models
{

    using System.ComponentModel.DataAnnotations;


    public class RelationShip_Employee : BaseEntity
    {
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        public string RELATIONSHIP { get; set; }
    }
}
