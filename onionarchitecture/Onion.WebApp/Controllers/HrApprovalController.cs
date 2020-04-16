using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Onion.Interfaces.Services;
using Onion.WebApp.Controllers;
using TCO.TFM.WDMS.ViewModels.ViewModels;
using Onion.Common.Constants;

namespace Onion.WebApp.Controllers
{
    public class HrApprovalController : Controller
    {    
       // private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IOpdExpenseService _opdExpenseService;
        private readonly IOpdExpenseImageService _opdExpenseImageService;
        private readonly IOpdExpensePatientService _opdExpensePatientService;      


        public HrApprovalController(IOpdExpenseService opdExpenseService, IOpdExpenseImageService opdExpenseImageService, IOpdExpensePatientService opdExpensePatientService)
        {
            _opdExpenseService = opdExpenseService;
            _opdExpenseImageService = opdExpenseImageService;
            _opdExpensePatientService = opdExpensePatientService;

        }

        private const string UrlIndex = "Index";
        private const string UrlHome = "Home";
        private const string UrlHrApproval = "HrApproval";


        // GET: OPDEXPENSEs
        public ActionResult Index()
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();            

                    string emailAddress = GetEmailAddress();                  

                    var opdExp = _opdExpenseService.GetOpdExpensesForHR();               
                              
                    return View(opdExp);

                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlHome);

                }

            }
            catch (Exception ex)
            {

                //logger.Error("HRAPPROVAL : Index()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

}

        // GET: OPDEXPENSEs/Details/5
        public ActionResult DetailsForOPDExpense(int? id)
        {         
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    if (!(AuthenticateEmailAddress(Convert.ToInt32(id))))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        RedirectToAction(UrlIndex, UrlHrApproval);
                    }

                    var result2 = GetOPDExpense(Convert.ToInt32(id));

                    return View(result2);
                }

                else
                {
                    return RedirectToAction(UrlIndex, UrlHrApproval);
                }

            }
            catch (Exception ex)
            {

                //logger.Error("HRAPPROVAL : DetailsForOPDExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
}



        public ActionResult DetailsForHospitalExpense(int? id)
        {
           try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    if (!(AuthenticateEmailAddress(Convert.ToInt32(id))))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        RedirectToAction(UrlIndex, UrlHrApproval);
                    }                 

                    var result2 = GetHospitalExpense(Convert.ToInt32(id));

                    return View(result2);
                }

                else
                {
                    return RedirectToAction(UrlIndex, UrlHrApproval);
                }

            }
            catch (Exception ex)
            {

                //logger.Error("HRAPPROVAL : DetailsForHospitalExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }


        // GET: OPDEXPENSEs/Edit/5
        public ActionResult HROPDExpense(int? id)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    if (!(AuthenticateEmailAddress(Convert.ToInt32(id))))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        RedirectToAction(UrlIndex, UrlHrApproval);
                    }                 

                    var opdExpense = GetOPDExpense(Convert.ToInt32(id));

                    ViewData["OPDEXPENSE_ID"] = id;
                    return View(opdExpense);
                }

                else
                {
                    return RedirectToAction(UrlIndex, UrlHrApproval);
                }

            }
            catch (Exception ex)
            {

               // logger.Error("HRAPPROVAL : HROPDExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

        // POST: OPDEXPENSEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HROPDExpense(OpdExpenseVM oPDEXPENSE)
        {
            try
            {
                string buttonStatus = Request.Form["buttonName"];

                if (buttonStatus == "approved")
                {
                    oPDEXPENSE.Status = ClaimStatus.HRAPPROVED;

                   if(oPDEXPENSE.TotalAmountApproved.ToString() == "")
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_TOTALAMOUNTAPPROVED);
                    }                
                   

                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.Status = ClaimStatus.HRREJECTED;

                    if (oPDEXPENSE.HrComment == null)
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_HRCOMMENTS);
                    }
                }
                else
                {
                    oPDEXPENSE.Status = ClaimStatus.HRINPROCESS;
                }

               

                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.HrApprovalDate = DateTime.Now;
                    oPDEXPENSE.HrEmailAddress = GetEmailAddress();
                    if (oPDEXPENSE.Status == UrlHrApproval)
                    {
                        oPDEXPENSE.HrApproval = true;
                    }

                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    return RedirectToAction(UrlIndex, UrlHrApproval);

                }

                var opdExpense = GetOPDExpense(Convert.ToInt32(oPDEXPENSE.ID));
                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.ID;
                return View(opdExpense);

            }
            catch (Exception ex)
            {

                //logger.Error("HRAPPROVAL : HROPDExpense([Bind])" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }





        // GET: OPDEXPENSEs/Edit/5
        public ActionResult HRHospitalExpense(int? id)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    if (!(AuthenticateEmailAddress(Convert.ToInt32(id))))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        RedirectToAction(UrlIndex, UrlHrApproval);
                    }

                    var result2 = GetHospitalExpense(Convert.ToInt32(id));

                    ViewData["OPDEXPENSE_ID"] = id;

                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlHrApproval);
                }

            }
            catch (Exception ex)
            {

                //logger.Error("HRAPPROVAL : HRHospitalExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

        }

        // POST: OPDEXPENSEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HRHospitalExpense( OpdExpenseVM oPDEXPENSE)
        {
            try
            {
                string buttonStatus = Request.Form["buttonName"];

                if (buttonStatus == "approved")
                {
                    oPDEXPENSE.Status = ClaimStatus.HRAPPROVED; 

                    if (oPDEXPENSE.TotalAmountApproved.ToString() == "")
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_TOTALAMOUNTAPPROVED);
                    }


                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.Status = ClaimStatus.HRREJECTED;

                    if (oPDEXPENSE.HrComment == null)
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_HRCOMMENTS);
                    }
                }
                else
                {
                    oPDEXPENSE.Status = ClaimStatus.HRINPROCESS;
                }
                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.HrApprovalDate = DateTime.Now;
                    oPDEXPENSE.HrEmailAddress = GetEmailAddress();
                    if (oPDEXPENSE.Status == ClaimStatus.HRAPPROVED)
                    {
                        oPDEXPENSE.HrApproval = true;
                    }

                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    return RedirectToAction(UrlIndex, UrlHrApproval);
                }
              
                var opdExpense = GetHospitalExpense(Convert.ToInt32(oPDEXPENSE.ID));
                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.ID;
                return View(opdExpense);

            }
            catch (Exception ex)
            {
                //logger.Error("HRAPPROVAL : HRHospitalExpense([Bind])" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }







        #region Get file method.

        /// <summary>
        /// Get file method.
        /// </summary>
        /// <param name="fileContent">File content parameter.</param>
        /// <param name="fileContentType">File content type parameter</param>
        /// <returns>Returns - File.</returns>
        private FileResult GetFile(string fileContent, string fileContentType)
        {
            // Initialization.
            FileResult file = null;

            try
            {
                // Get file.
                byte[] byteContent = Convert.FromBase64String(fileContent);
                file = this.File(byteContent, fileContentType);
            }
            catch (Exception ex)
            {
                // Info.
                throw ex;
            }

            // info.
            return file;
        }



        private string GetEmailAddress()
        {
            OfficeManagerController managerController = new OfficeManagerController();
            string emailAddress = managerController.GetEmailAddress();

            return emailAddress;
        }

        private void AuthenticateUser()
        {
            OfficeManagerController managerController = new OfficeManagerController();
            ViewBag.RollType = managerController.AuthenticateUser();

            ViewBag.UserName = managerController.GetName();

        }

        private OpdExpenseVM GetOPDExpense(int Id)
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
                FinanceApproval = opdExpense.FinanceApproval,
                FinanceComment = opdExpense.FinanceComment,
                FinanceName = opdExpense.FinanceName,
                HospitalName = opdExpense.HospitalName,
                HrApproval = opdExpense.HrApproval,
                HrComment = opdExpense.HrComment,
                HrName = opdExpense.HrName,
                ManagementApproval = opdExpense.ManagementApproval,
                ManagementComment = opdExpense.ManagementComment,
                ManagementName = opdExpense.ManagementName,
                PeriodConfinementDateFrom = opdExpense.PeriodConfinementDateFrom,
                PeriodConfinementDateTo = opdExpense.PeriodConfinementDateTo,
                Status = opdExpense.Status,
                OpdType = opdExpense.OpdType,
                TotalAmountClaimed = opdExpense.TotalAmountClaimed,
                ClaimYear = opdExpense.ClaimYear,
                CreatedDate = opdExpense.CreatedDate,
                ModifiedDate = opdExpense.ModifiedDate

            };

            return opdInformation;
        }

        private HospitalExpenseVM GetHospitalExpense(int Id)
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
                FinanceApproval = opdExpense.FinanceApproval,
                FinanceComment = opdExpense.FinanceComment,
                FinanceName = opdExpense.FinanceName,
                HospitalName = opdExpense.HospitalName,
                HrApproval = opdExpense.HrApproval,
                HrComment = opdExpense.HrComment,
                HrName = opdExpense.HrName,
                ManagementApproval = opdExpense.ManagementApproval,
                ManagementComment = opdExpense.ManagementComment,
                ManagementName = opdExpense.ManagementName,
                PeriodConfinementDateFrom = opdExpense.PeriodConfinementDateFrom,
                PeriodConfinementDateTo = opdExpense.PeriodConfinementDateTo,
                Status = opdExpense.Status,
                OpdType = opdExpense.OpdType,
                TotalAmountClaimed = opdExpense.TotalAmountClaimed,
                ClaimYear = opdExpense.ClaimYear,
                CreatedDate = opdExpense.CreatedDate,
                ModifiedDate = opdExpense.ModifiedDate
            };

            return hospitalInformation;
        }



        #endregion

        private bool AuthenticateEmailAddress(int Id)
        {

            var opdInformation = GetOPDExpense(Convert.ToInt32(Id));
            OfficeManagerController managerController = new OfficeManagerController();

            string currentEmailAddress = managerController.GetEmailAddress();

            if (currentEmailAddress.Equals(opdInformation.EmployeeEmailAddress))

                return true;
            else
                return false;

        }
    }
}