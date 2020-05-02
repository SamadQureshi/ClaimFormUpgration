using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Onion.Domain.Models
{
    public  class OpdExpense : BaseEntity
    {
     
        public ICollection<OpdExpenseImage> OpdExpenseImages { get; set; }

        public ICollection<OpdExpensePatient> OpdExpensePatients { get; set; }
         
        [StringLength(100)]
        public string EmployeeEmailAddress { get; set; }

        [StringLength(100)]
        public string EmployeeName { get; set; }

        [StringLength(100)]
        public string EmployeeDepartment { get; set; }

        [StringLength(100)]
        public string OpdType { get; set; }

        [StringLength(100)]
        public string Status { get; set; }

        [StringLength(50)]
        public string ClaimMonth { get; set; }

        [StringLength(50)]
        public string ClaimYear { get; set; }

        public decimal? TotalAmountClaimed { get; set; }

        public decimal? TotalAmountApproved { get; set; }

        [StringLength(5000)]
        public string HrComment { get; set; }

        public bool? HrApproval { get; set; }
        
        public DateTime? HrApprovalDate { get; set; }

        [StringLength(100)]
        public string HrEmailAddress { get; set; }

        [StringLength(100)]
        public string HrName { get; set; }

        [StringLength(5000)]
        public string FinanceComment { get; set; }

        public bool? FinanceApproval { get; set; }

        public DateTime? FinanceApprovalDate { get; set; }
        
        [StringLength(100)]
        public string FinanceEmailAddress { get; set; }

        [StringLength(100)]
        public string FinanceName { get; set; }

        [StringLength(5000)]
        public string ManagementComment { get; set; }

        public bool? ManagementApproval { get; set; }

        public DateTime? ManagementApprovalDate { get; set; }

        [StringLength(100)]
        public string ManagementName { get; set; }
        
        [StringLength(100)]
        public string ManagementEmailAddress { get; set; }

        public DateTime? DateIllnessNoticed { get; set; }

        public DateTime? DateRecovery { get; set; }

        [StringLength(5000)]
        public string Diagnosis { get; set; }

        public bool? ClaimantSufferedIllness { get; set; }

        public DateTime? ClaimantSufferedIllnessDate { get; set; }

        [StringLength(5000)]
        public string ClaimantSufferedIllnessDetails { get; set; }

        [StringLength(500)]
        public string HospitalName { get; set; }

        [StringLength(500)]
        public string DoctorName { get; set; }

        public DateTime? PeriodConfinementDateFrom { get; set; }

        public DateTime? PeriodConfinementDateTo { get; set; }

        public bool? DrugsPrescribedBool { get; set; }

        public string DrugsPrescribedDescription { get; set; }
          
        [StringLength(100)]
        public string ManagerName { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [StringLength(4000)]
        public string ExpenseNumber { get; set; }
        
        public bool? PhysicalDocumentReceived { get; set; }

        [StringLength(50)]
        public string PayRollMonth { get; set; }

        [StringLength(500)]
        public string OpdEncrypted { get; set; }




    }
}
