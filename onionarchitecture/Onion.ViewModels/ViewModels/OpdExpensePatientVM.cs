using Onion.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCO.TFM.WDMS.ViewModels.ViewModels
{
    public class OpdExpensePatientVM
    {
        public int ID { get; set; }     
        public string Name { get; set; }

        [Range(1, 100, ErrorMessage = "The Age must be between 1 and 100.")]
        public int? Age { get; set; }
        public string RelationshipEmployee { get; set; }
        public int OpdExpenseId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public virtual OpdExpense OpdExpense { get; set; }


    }
}
