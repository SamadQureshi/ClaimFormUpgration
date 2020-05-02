using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace TCO.TFM.WDMS.ViewModels.ViewModels
{
    public class HospitalExpenseVM
    {

        public int ID { get; set; }

        public ICollection<OpdExpenseImageVM> OpdExpenseImages { get; set; }

        public ICollection<OpdExpensePatientVM> OpdExpensePatients { get; set; }
       

        public string EmployeeEmailAddress { get; set; }

        [DisplayName("Employee Name")]
        [Required(ErrorMessage = "The Employee Name is required.")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Incorrect Employee Name")]
        public string EmployeeName { get; set; }

        public string EmployeeDepartment { get; set; }

        public string OpdType { get; set; }

        public string Status { get; set; }

        public string ClaimMonth { get; set; }

        public string ClaimYear { get; set; }



        [DisplayName("Total Amount")]
        [Required(ErrorMessage = "The Total Amount is required.")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Invalid Number")]
        public decimal? TotalAmountClaimed { get; set; }

        public decimal? TotalAmountApproved { get; set; }

        public string HrComment { get; set; }

        public bool? HrApproval { get; set; }

        public DateTime? HrApprovalDate { get; set; }

        public string HrEmailAddress { get; set; }

        public string HrName { get; set; }

        public string FinanceComment { get; set; }

        public bool? FinanceApproval { get; set; }

        public DateTime? FinanceApprovalDate { get; set; }

        public string FinanceEmailAddress { get; set; }

        public string FinanceName { get; set; }

        public string ManagementComment { get; set; }

        public bool? ManagementApproval { get; set; }

        public DateTime? ManagementApprovalDate { get; set; }

        public string ManagementName { get; set; }

        public string ManagementEmailAddress { get; set; }

        [Required(ErrorMessage = "The Date of Illness is required.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateIllnessNoticed { get; set; }

        [Required(ErrorMessage = "The Date of Recovery is required.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateRecovery { get; set; }

        [Required(ErrorMessage = "The Diagnosis is required.")]
        public string Diagnosis { get; set; }

        public bool? ClaimantSufferedIllness { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ClaimantSufferedIllnessDate { get; set; }

        public string ClaimantSufferedIllnessDetails { get; set; }

        [Required(ErrorMessage = "The Hospital Name is required.")]
        public string HospitalName { get; set; }

        [Required(ErrorMessage = "The Doctor Name is required.")]
        public string DoctorName { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Required(ErrorMessage = "The Period of Confinement is required.")]
        public DateTime? PeriodConfinementDateFrom { get; set; }



        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Required(ErrorMessage = "The Period of Confinement is required.")]
        public DateTime? PeriodConfinementDateTo { get; set; }

        public bool? DrugsPrescribedBool { get; set; }

        public string DrugsPrescribedDescription { get; set; }

        public string ManagerName { get; set; }

        public string ExpenseNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public bool? PhysicalDocumentReceived { get; set; }

        public string PayRollMonth { get; set; }

        public string OpdEncrypted { get; set; }

    }
}