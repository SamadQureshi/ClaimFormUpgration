namespace Onion.Domain.Models
{

    using System.ComponentModel.DataAnnotations;


    public class RelationShipEmployee : BaseEntity
    {

        [StringLength(50)]
        public string RelationShip { get; set; }
    }
}
