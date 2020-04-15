namespace Onion.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public class OpdExpense_Image : BaseEntity
    {
        [Key]
        public int IMAGE_ID { get; set; }
        
        public int? OPDEXPENSE_ID { get; set; }

        public string IMAGE_NAME { get; set; }

        public string IMAGE_EXT { get; set; }

        public string IMAGE_BASE64 { get; set; }

        [StringLength(5000)]
        public string NAME_EXPENSES { get; set; }

        public decimal? EXPENSE_AMOUNT { get; set; }

        [ForeignKey("OPDEXPENSE_ID")]
        public virtual OpdExpense OpdExpense { get; set; }


    }
}
