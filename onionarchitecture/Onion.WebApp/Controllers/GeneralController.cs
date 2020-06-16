using Onion.Common.Constants;
using Onion.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.WebApp.Controllers
{
    public static class GeneralController 
    {


 



        public static OpdExpenseVM GetOPDExpense(int Id, IOpdExpenseService _opdExpenseService, IOpdExpensePatientService _opdExpensePatientService, IOpdExpenseImageService _opdExpenseImageService)
        {
            OpdExpenseVM opdExpense = _opdExpenseService.GetOpdExpensesAgainstId(Id);

            var opdInformation = new OpdExpenseVM()
            {

                OpdExpensePatients = _opdExpensePatientService.GetOpdExpensesPatientAgainstOpdExpenseId(Id),
                OpdExpenseImages = _opdExpenseImageService.GetOpdExpensesImageAgainstOpdExpenseId(Id),

                ID = opdExpense.ID,
                ClaimantSufferedIllness = opdExpense.ClaimantSufferedIllness,
                ClaimantSufferedIllnessDetails = opdExpense.ClaimantSufferedIllnessDetails,
                ClaimantSufferedIllnessDate = opdExpense.ClaimantSufferedIllnessDate,
                DateIllnessNoticed = opdExpense.DateIllnessNoticed,
                DateRecovery = opdExpense.DateRecovery,
                Diagnosis = opdExpense.Diagnosis,
                DoctorName = opdExpense.DoctorName,
                DrugsPrescribedBool = opdExpense.DrugsPrescribedBool,
                DrugsPrescribedDescription = opdExpense.DrugsPrescribedDescription,
                EmployeeDepartment = opdExpense.EmployeeDepartment,
                EmployeeName = opdExpense.EmployeeName,
                EmployeeEmailAddress = opdExpense.EmployeeEmailAddress,



                HospitalName = opdExpense.HospitalName,

                FinanceApproval = opdExpense.FinanceApproval,
                FinanceComment = opdExpense.FinanceComment,
                FinanceApprovalDate = opdExpense.FinanceApprovalDate,
                FinanceEmailAddress = opdExpense.FinanceEmailAddress,
                FinanceName = opdExpense.FinanceName,


                HrApproval = opdExpense.HrApproval,
                HrComment = opdExpense.HrComment,
                HrName = opdExpense.HrName,
                HrApprovalDate = opdExpense.HrApprovalDate,
                HrEmailAddress = opdExpense.HrEmailAddress,


                ManagementApproval = opdExpense.ManagementApproval,
                ManagementComment = opdExpense.ManagementComment,
                ManagementName = opdExpense.ManagementName,
                ManagementApprovalDate = opdExpense.ManagementApprovalDate,
                ManagementEmailAddress = opdExpense.ManagementEmailAddress,

                PeriodConfinementDateFrom = opdExpense.PeriodConfinementDateFrom,
                PeriodConfinementDateTo = opdExpense.PeriodConfinementDateTo,
                Status = opdExpense.Status,
                OpdType = opdExpense.OpdType,
                TotalAmountClaimed = opdExpense.TotalAmountClaimed,
                TotalAmountApproved = opdExpense.TotalAmountApproved,
                ClaimMonth = opdExpense.ClaimMonth,
                ClaimYear = opdExpense.ClaimYear,
                CreatedDate = opdExpense.CreatedDate,
                ModifiedDate = opdExpense.ModifiedDate,
                ManagerName = opdExpense.ManagerName,
                PhysicalDocumentReceived = opdExpense.PhysicalDocumentReceived,
                PayRollMonth = opdExpense.PayRollMonth,
                ExpenseNumber = opdExpense.ExpenseNumber,
                OpdEncrypted = opdExpense.OpdEncrypted,
                HospitalizationType = opdExpense.HospitalizationType,
                MaternityType = opdExpense.MaternityType

            };

            return opdInformation;
        }
        public static HospitalExpenseVM GetHospitalExpense(int Id, IOpdExpenseService _opdExpenseService, IOpdExpensePatientService _opdExpensePatientService, IOpdExpenseImageService _opdExpenseImageService)
        {

            OpdExpenseVM opdExpense = _opdExpenseService.GetOpdExpensesAgainstId(Id);


            var hospitalInformation = new HospitalExpenseVM()
            {

                OpdExpensePatients = _opdExpensePatientService.GetOpdExpensesPatientAgainstOpdExpenseId(Id),
                OpdExpenseImages = _opdExpenseImageService.GetOpdExpensesImageAgainstOpdExpenseId(Id),

                ID = opdExpense.ID,
                ClaimantSufferedIllness = opdExpense.ClaimantSufferedIllness,
                ClaimantSufferedIllnessDetails = opdExpense.ClaimantSufferedIllnessDetails,
                ClaimantSufferedIllnessDate = opdExpense.ClaimantSufferedIllnessDate,
                DateIllnessNoticed = opdExpense.DateIllnessNoticed,
                DateRecovery = opdExpense.DateRecovery,
                Diagnosis = opdExpense.Diagnosis,
                DoctorName = opdExpense.DoctorName,
                DrugsPrescribedBool = opdExpense.DrugsPrescribedBool,
                DrugsPrescribedDescription = opdExpense.DrugsPrescribedDescription,
                EmployeeDepartment = opdExpense.EmployeeDepartment,
                EmployeeName = opdExpense.EmployeeName,
                EmployeeEmailAddress = opdExpense.EmployeeEmailAddress,

                HospitalName = opdExpense.HospitalName,

                FinanceApproval = opdExpense.FinanceApproval,
                FinanceComment = opdExpense.FinanceComment,
                FinanceApprovalDate = opdExpense.FinanceApprovalDate,
                FinanceEmailAddress = opdExpense.FinanceEmailAddress,
                FinanceName = opdExpense.FinanceName,


                HrApproval = opdExpense.HrApproval,
                HrComment = opdExpense.HrComment,
                HrName = opdExpense.HrName,
                HrApprovalDate = opdExpense.HrApprovalDate,
                HrEmailAddress = opdExpense.HrEmailAddress,


                ManagementApproval = opdExpense.ManagementApproval,
                ManagementComment = opdExpense.ManagementComment,
                ManagementName = opdExpense.ManagementName,
                ManagementApprovalDate = opdExpense.ManagementApprovalDate,
                ManagementEmailAddress = opdExpense.ManagementEmailAddress,


                PeriodConfinementDateFrom = opdExpense.PeriodConfinementDateFrom,
                PeriodConfinementDateTo = opdExpense.PeriodConfinementDateTo,
                Status = opdExpense.Status,
                OpdType = opdExpense.OpdType,
                TotalAmountClaimed = opdExpense.TotalAmountClaimed,
                TotalAmountApproved = opdExpense.TotalAmountApproved,
                ClaimYear = opdExpense.ClaimYear,
                ClaimMonth = opdExpense.ClaimMonth,
                CreatedDate = opdExpense.CreatedDate,
                ModifiedDate = opdExpense.ModifiedDate,
                PhysicalDocumentReceived = opdExpense.PhysicalDocumentReceived,
                PayRollMonth = opdExpense.PayRollMonth,
                ExpenseNumber = opdExpense.ExpenseNumber,
                OpdEncrypted = opdExpense.OpdEncrypted,
                HospitalizationType = opdExpense.HospitalizationType,
                MaternityType = opdExpense.MaternityType
            };

            return hospitalInformation;
        }

        public static TravelExpenseMasterDetail GetTravelExpense(int Id, IOpdExpenseService _opdExpenseService, ITravelExpenseService _travelExpenseService)
        {
            OpdExpenseVM opdExpense = _opdExpenseService.GetOpdExpensesAgainstId(Id);

            var opdInformation = new TravelExpenseMasterDetail()
            {

                ListTravelExpense = _travelExpenseService.GetTravelExpensesAgainstOpdExpenseId(Id),

                ID = opdExpense.ID,
                ClaimantSufferedIllness = opdExpense.ClaimantSufferedIllness,
                ClaimantSufferedIllnessDetails = opdExpense.ClaimantSufferedIllnessDetails,
                ClaimantSufferedIllnessDate = opdExpense.ClaimantSufferedIllnessDate,
                DateIllnessNoticed = opdExpense.DateIllnessNoticed,
                DateRecovery = opdExpense.DateRecovery,
                Diagnosis = opdExpense.Diagnosis,
                DoctorName = opdExpense.DoctorName,
                DrugsPrescribedBool = opdExpense.DrugsPrescribedBool,
                DrugsPrescribedDescription = opdExpense.DrugsPrescribedDescription,
                EmployeeDepartment = opdExpense.EmployeeDepartment,
                EmployeeName = opdExpense.EmployeeName,
                EmployeeEmailAddress = opdExpense.EmployeeEmailAddress,

                HospitalName = opdExpense.HospitalName,

                FinanceApproval = opdExpense.FinanceApproval,
                FinanceComment = opdExpense.FinanceComment,
                FinanceApprovalDate = opdExpense.FinanceApprovalDate,
                FinanceEmailAddress = opdExpense.FinanceEmailAddress,
                FinanceName = opdExpense.FinanceName,


                HrApproval = opdExpense.HrApproval,
                HrComment = opdExpense.HrComment,
                HrName = opdExpense.HrName,
                HrApprovalDate = opdExpense.HrApprovalDate,
                HrEmailAddress = opdExpense.HrEmailAddress,


                ManagementApproval = opdExpense.ManagementApproval,
                ManagementComment = opdExpense.ManagementComment,
                ManagementName = opdExpense.ManagementName,
                ManagementApprovalDate = opdExpense.ManagementApprovalDate,
                ManagementEmailAddress = opdExpense.ManagementEmailAddress,


                PeriodConfinementDateFrom = opdExpense.PeriodConfinementDateFrom,
                PeriodConfinementDateTo = opdExpense.PeriodConfinementDateTo,
                Status = opdExpense.Status,
                OpdType = opdExpense.OpdType,
                TotalAmountClaimed = opdExpense.TotalAmountClaimed,
                TotalAmountApproved = opdExpense.TotalAmountApproved,
                ClaimMonth = opdExpense.ClaimMonth,
                ClaimYear = opdExpense.ClaimYear,
                CreatedDate = opdExpense.CreatedDate,
                ModifiedDate = opdExpense.ModifiedDate,
                ManagerName = opdExpense.ManagerName,
                PhysicalDocumentReceived = opdExpense.PhysicalDocumentReceived,
                PayRollMonth = opdExpense.PayRollMonth,
                ExpenseNumber = opdExpense.ExpenseNumber,
                OpdEncrypted = opdExpense.OpdEncrypted,
                HospitalizationType = opdExpense.HospitalizationType,
                MaternityType = opdExpense.MaternityType

            };

            return opdInformation;
        }
     


        //public static string DisplayRemainingAmount(string EmailAddress, string OpdExpense, IOpdExpenseService _opdExpenseService, ISetupExpenseAmountService _setupExpenseAmountService)
        //{
           

        //    decimal? approvedAmount = _opdExpenseService.GetApprovedAmountAgainstEmailAddress(EmailAddress, OpdExpense, string.Empty, string.Empty);

        //    string defaultAmount = _setupExpenseAmountService.GetDefaultExpenseAmountAgainstExpenseType(OpdExpense);

        //    decimal? totalUsedAmount = approvedAmount;

        //    string totalAmount = Convert.ToString(Convert.ToDecimal(defaultAmount) - Convert.ToDecimal(totalUsedAmount));

        //    return totalAmount;

        //}

        public static string CalculateRemainingAmount(string EmailAddress, string OpdExpense, string hospitalizationType, string maternityType, IOpdExpenseService _opdExpenseService, ISetupExpenseAmountService _setupExpenseAmountService , bool forEmployee)
        {
            decimal? claimAmount = 0;

            if (forEmployee == true)
            {
                claimAmount = _opdExpenseService.GetClaimAmountAgainstEmailAddress(EmailAddress, OpdExpense, hospitalizationType, maternityType);
            } 

            decimal? approvedAmount = _opdExpenseService.GetApprovedAmountAgainstEmailAddress(EmailAddress, OpdExpense, hospitalizationType, maternityType);

            if(hospitalizationType == null) 
            {
                hospitalizationType = HospitalizationType.OpdExpense;
            }
            else if(hospitalizationType == string.Empty)
            {
                hospitalizationType = HospitalizationType.OpdExpense;
            }

            else if (hospitalizationType == HospitalizationType.InPatient)
            {
                hospitalizationType = HospitalizationType.InPatient;
            }
            else if (hospitalizationType == HospitalizationType.Maternity)
            {

                if (maternityType == HospitalizationType.Normal)
                {
                    hospitalizationType = "Maternity_Normal";
                }
                else if(maternityType == HospitalizationType.CSection)
                {
                    hospitalizationType = "Maternity_CSection";
                }

            }

            string defaultAmount = _setupExpenseAmountService.GetDefaultExpenseAmountAgainstExpenseType(hospitalizationType);

            decimal? totalUsedAmount = claimAmount + approvedAmount;

            string totalAmount = Convert.ToString(Convert.ToDecimal(defaultAmount) - Convert.ToDecimal(totalUsedAmount));

            return totalAmount;

        }

    }


}
        
