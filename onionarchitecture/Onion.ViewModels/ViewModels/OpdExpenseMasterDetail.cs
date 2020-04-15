using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TCO.TFM.WDMS.ViewModels.ViewModels
{
    public class OpdExpenseMasterDetail
    {
        public int OPDEXPENSE_ID { get; set; }
   
        public string EMPLOYEE_EMAILADDRESS { get; set; }

        [DisplayName("Employee Name")]
        [Required(ErrorMessage = "The Employee Name is required.")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Incorrect Employee Name")]
        public string EMPLOYEE_NAME { get; set; }

        public string EMPLOYEE_DEPARTMENT { get; set; }


        public string CLAIM_MONTH { get; set; }

        public string CLAIM_YEAR { get; set; }


        [DisplayName("Total Amount")]
        [Required(ErrorMessage = "The Total Amount is required.")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Invalid Number")]
        public decimal? TOTAL_AMOUNT_CLAIMED { get; set; }

        public string HR_COMMENT { get; set; }

        public bool? HR_APPROVAL { get; set; }

        public DateTime? HR_APPROVAL_DATE { get; set; }


        public string HR_NAME { get; set; }


        public string FINANCE_COMMENT { get; set; }

        public bool? FINANCE_APPROVAL { get; set; }

        public DateTime? FINANCE_APPROVAL_DATE { get; set; }

        public string FINANCE_NAME { get; set; }

        public string MANAGEMENT_COMMENT { get; set; }

        public bool? MANAGEMENT_APPROVAL { get; set; }

        public DateTime? MANAGEMENT_APPROVAL_DATE { get; set; }


        public string MANAGEMENT_NAME { get; set; }

        public DateTime? DATE_ILLNESS_NOTICED { get; set; }

        public DateTime? DATE_RECOVERY { get; set; }


        public string DIAGNOSIS { get; set; }

        public bool? CLAIMANT_SUFFERED_ILLNESS { get; set; }

        public DateTime? CLAIMANT_SUFFERED_ILLNESS_DATE { get; set; }


        public string CLAIMANT_SUFFERED_ILLNESS_DETAILS { get; set; }

        public string HOSPITAL_NAME { get; set; }

        public string DOCTOR_NAME { get; set; }

        public DateTime? PERIOD_CONFINEMENT_DATE_FROM { get; set; }

        public DateTime? PERIOD_CONFINEMENT_DATE_TO { get; set; }

        public bool? DRUGS_PRESCRIBED_BOOL { get; set; }

        public string DRUGS_PRESCRIBED_DESCRIPTION { get; set; }

        public string OPDTYPE { get; set; }


        public string HR_EMAILADDRESS { get; set; }


        public string FINANCE_EMAILADDRESS { get; set; }


        public string MANAGEMENT_EMAILADDRESS { get; set; }


        public string STATUS { get; set; }

        public decimal? TOTAL_AMOUNT_APPROVED { get; set; }

        public DateTime? CREATED_DATE { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }


        public List<OpdExpensePatientVM> ListOPDEXPENSEPATIENT { get; set; }

        public List<OpdExpenseImageVM> ListOPDEXPENSEIMAGE { get; set; }

        public string ManagerName { get; set; }



    }
}