using Onion.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Domain.Models
{
    public class SetupExpenseAmount : BaseEntity
    {
        [StringLength(100)]
        public string ExpenseKey { get; set; }

        [StringLength(100)]
        public string ExpenseValue { get; set; }

        [StringLength(50)]
        public string Year { get; set; }



    }
}
