namespace Onion.Domain.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

   public class Department : BaseEntity
    {

        [StringLength(50)]
        public string DepartmentName { get; set; }
    }
}
