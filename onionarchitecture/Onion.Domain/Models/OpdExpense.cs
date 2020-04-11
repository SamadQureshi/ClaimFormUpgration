using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Onion.Domain.Models
{
    public  class OpdExpense : BaseEntity
    {
     
        public ICollection<OpdExpense_Image> OpdExpense_Images { get; set; }

        public ICollection<OpdExpense_Patient> OpdExpense_Patients { get; set; }

        [Key]
        public int OPDEXPENSE_ID { get; set; }

        [StringLength(100)]
        public string EMPLOYEE_EMAILADDRESS { get; set; }

        [StringLength(100)]
        public string EMPLOYEE_NAME { get; set; }

        [StringLength(100)]
        public string EMPLOYEE_DEPARTMENT { get; set; }

        [StringLength(50)]
        public string CLAIM_MONTH { get; set; }

        [StringLength(50)]
        public string CLAIM_YEAR { get; set; }

        public decimal? TOTAL_AMOUNT_CLAIMED { get; set; }

        [StringLength(5000)]
        public string HR_COMMENT { get; set; }

        public bool? HR_APPROVAL { get; set; }

        public DateTime? HR_APPROVAL_DATE { get; set; }

        [StringLength(100)]
        public string HR_NAME { get; set; }

        [StringLength(5000)]
        public string FINANCE_COMMENT { get; set; }

        public bool? FINANCE_APPROVAL { get; set; }

        public DateTime? FINANCE_APPROVAL_DATE { get; set; }

        [StringLength(100)]
        public string FINANCE_NAME { get; set; }

        [StringLength(5000)]
        public string MANAGEMENT_COMMENT { get; set; }

        public bool? MANAGEMENT_APPROVAL { get; set; }

        public DateTime? MANAGEMENT_APPROVAL_DATE { get; set; }

        [StringLength(100)]
        public string MANAGEMENT_NAME { get; set; }

        public DateTime? DATE_ILLNESS_NOTICED { get; set; }

        public DateTime? DATE_RECOVERY { get; set; }

        [StringLength(5000)]
        public string DIAGNOSIS { get; set; }

        public bool? CLAIMANT_SUFFERED_ILLNESS { get; set; }

        public DateTime? CLAIMANT_SUFFERED_ILLNESS_DATE { get; set; }

        [StringLength(5000)]
        public string CLAIMANT_SUFFERED_ILLNESS_DETAILS { get; set; }

        [StringLength(500)]
        public string HOSPITAL_NAME { get; set; }

        [StringLength(500)]
        public string DOCTOR_NAME { get; set; }

        public DateTime? PERIOD_CONFINEMENT_DATE_FROM { get; set; }

        public DateTime? PERIOD_CONFINEMENT_DATE_TO { get; set; }

        public bool? DRUGS_PRESCRIBED_BOOL { get; set; }

        public string DRUGS_PRESCRIBED_DESCRIPTION { get; set; }

        [StringLength(100)]
        public string OPDTYPE { get; set; }       

        [StringLength(100)]
        public string HR_EMAILADDRESS { get; set; }

        [StringLength(100)]
        public string FINANCE_EMAILADDRESS { get; set; }

        [StringLength(100)]
        public string MANAGEMENT_EMAILADDRESS { get; set; }

        [StringLength(100)]
        public string STATUS { get; set; }

        public decimal? TOTAL_AMOUNT_APPROVED { get; set; }



        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [StringLength(4000)]
        public string EXPENSE_NUMBER { get; set; }




    }
}
