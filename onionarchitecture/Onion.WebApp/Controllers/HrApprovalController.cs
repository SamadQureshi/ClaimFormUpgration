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
using Onion.WebApp.Models;
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
                    oPDEXPENSE.STATUS = ClaimStatus.HRAPPROVED;

                   if(oPDEXPENSE.TOTAL_AMOUNT_APPROVED.ToString() == "")
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_TOTALAMOUNTAPPROVED);
                    }                
                   

                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.STATUS = ClaimStatus.HRREJECTED;

                    if (oPDEXPENSE.HR_COMMENT == null)
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_HRCOMMENTS);
                    }
                }
                else
                {
                    oPDEXPENSE.STATUS = ClaimStatus.HRINPROCESS;
                }

               

                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.HR_APPROVAL_DATE = DateTime.Now;
                    oPDEXPENSE.HR_EMAILADDRESS = GetEmailAddress();
                    if (oPDEXPENSE.STATUS == UrlHrApproval)
                    {
                        oPDEXPENSE.HR_APPROVAL = true;
                    }

                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    return RedirectToAction(UrlIndex, UrlHrApproval);

                }

                var opdExpense = GetOPDExpense(Convert.ToInt32(oPDEXPENSE.OPDEXPENSE_ID));
                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.OPDEXPENSE_ID;
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
                    oPDEXPENSE.STATUS = ClaimStatus.HRAPPROVED; 

                    if (oPDEXPENSE.TOTAL_AMOUNT_APPROVED.ToString() == "")
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_TOTALAMOUNTAPPROVED);
                    }


                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.STATUS = ClaimStatus.HRREJECTED;

                    if (oPDEXPENSE.HR_COMMENT == null)
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_HRCOMMENTS);
                    }
                }
                else
                {
                    oPDEXPENSE.STATUS = ClaimStatus.HRINPROCESS;
                }
                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.HR_APPROVAL_DATE = DateTime.Now;
                    oPDEXPENSE.HR_EMAILADDRESS = GetEmailAddress();
                    if (oPDEXPENSE.STATUS == ClaimStatus.HRAPPROVED)
                    {
                        oPDEXPENSE.HR_APPROVAL = true;
                    }

                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    return RedirectToAction(UrlIndex, UrlHrApproval);
                }
              
                var opdExpense = GetHospitalExpense(Convert.ToInt32(oPDEXPENSE.OPDEXPENSE_ID));
                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.OPDEXPENSE_ID;
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

        private OpdExpenseMasterDetail GetOPDExpense(int Id)
        {
            OpdExpenseVM opdExpense = _opdExpenseService.GetOpdExpensesAgainstId(Id);

            var opdInformation = new OpdExpenseMasterDetail()
            {

                ListOPDEXPENSEPATIENT = _opdExpensePatientService.GetOpdExpensesPatientAgainstOpdExpenseId(Id),
                ListOPDEXPENSEIMAGE = _opdExpenseImageService.GetOpdExpensesImageAgainstOpdExpenseId(Id),

                OPDEXPENSE_ID = opdExpense.OPDEXPENSE_ID,
                CLAIMANT_SUFFERED_ILLNESS = opdExpense.CLAIMANT_SUFFERED_ILLNESS,
                CLAIMANT_SUFFERED_ILLNESS_DETAILS = opdExpense.CLAIMANT_SUFFERED_ILLNESS_DETAILS,
                CLAIMANT_SUFFERED_ILLNESS_DATE = opdExpense.CLAIMANT_SUFFERED_ILLNESS_DATE,
                DATE_ILLNESS_NOTICED = opdExpense.DATE_ILLNESS_NOTICED,
                DATE_RECOVERY = opdExpense.DATE_RECOVERY,
                DIAGNOSIS = opdExpense.DIAGNOSIS,
                DOCTOR_NAME = opdExpense.DOCTOR_NAME,
                DRUGS_PRESCRIBED_BOOL = opdExpense.DRUGS_PRESCRIBED_BOOL,
                DRUGS_PRESCRIBED_DESCRIPTION = opdExpense.DRUGS_PRESCRIBED_DESCRIPTION,
                EMPLOYEE_DEPARTMENT = opdExpense.EMPLOYEE_DEPARTMENT,
                EMPLOYEE_NAME = opdExpense.EMPLOYEE_NAME,
                EMPLOYEE_EMAILADDRESS = opdExpense.EMPLOYEE_EMAILADDRESS,
                FINANCE_APPROVAL = opdExpense.FINANCE_APPROVAL,
                FINANCE_COMMENT = opdExpense.FINANCE_COMMENT,
                FINANCE_NAME = opdExpense.FINANCE_NAME,
                HOSPITAL_NAME = opdExpense.HOSPITAL_NAME,
                HR_APPROVAL = opdExpense.HR_APPROVAL,
                HR_COMMENT = opdExpense.HR_COMMENT,
                HR_NAME = opdExpense.HR_NAME,
                MANAGEMENT_APPROVAL = opdExpense.MANAGEMENT_APPROVAL,
                MANAGEMENT_COMMENT = opdExpense.MANAGEMENT_COMMENT,
                MANAGEMENT_NAME = opdExpense.MANAGEMENT_NAME,
                PERIOD_CONFINEMENT_DATE_FROM = opdExpense.PERIOD_CONFINEMENT_DATE_FROM,
                PERIOD_CONFINEMENT_DATE_TO = opdExpense.PERIOD_CONFINEMENT_DATE_TO,
                STATUS = opdExpense.STATUS,
                OPDTYPE = opdExpense.OPDTYPE,
                TOTAL_AMOUNT_CLAIMED = opdExpense.TOTAL_AMOUNT_CLAIMED,
                CLAIM_YEAR = opdExpense.CLAIM_YEAR,
                CREATED_DATE = opdExpense.CreatedDate,
                MODIFIED_DATE = opdExpense.ModifiedDate

            };

            return opdInformation;

           
        }

        private HospitalExpenseMasterDetail GetHospitalExpense(int Id)
        {

            OpdExpenseVM opdExpense = _opdExpenseService.GetOpdExpensesAgainstId(Id);


            var hospitalInformation = new HospitalExpenseMasterDetail()
            {


                ListOPDEXPENSEPATIENT = _opdExpensePatientService.GetOpdExpensesPatientAgainstOpdExpenseId(Id),
                ListOPDEXPENSEIMAGE = _opdExpenseImageService.GetOpdExpensesImageAgainstOpdExpenseId(Id),


                OPDEXPENSE_ID = opdExpense.OPDEXPENSE_ID,
                CLAIMANT_SUFFERED_ILLNESS = opdExpense.CLAIMANT_SUFFERED_ILLNESS,
                CLAIMANT_SUFFERED_ILLNESS_DETAILS = opdExpense.CLAIMANT_SUFFERED_ILLNESS_DETAILS,
                CLAIMANT_SUFFERED_ILLNESS_DATE = opdExpense.CLAIMANT_SUFFERED_ILLNESS_DATE,
                DATE_ILLNESS_NOTICED = opdExpense.DATE_ILLNESS_NOTICED,
                DATE_RECOVERY = opdExpense.DATE_RECOVERY,
                DIAGNOSIS = opdExpense.DIAGNOSIS,
                DOCTOR_NAME = opdExpense.DOCTOR_NAME,
                DRUGS_PRESCRIBED_BOOL = opdExpense.DRUGS_PRESCRIBED_BOOL,
                DRUGS_PRESCRIBED_DESCRIPTION = opdExpense.DRUGS_PRESCRIBED_DESCRIPTION,
                EMPLOYEE_DEPARTMENT = opdExpense.EMPLOYEE_DEPARTMENT,
                EMPLOYEE_NAME = opdExpense.EMPLOYEE_NAME,
                EMPLOYEE_EMAILADDRESS = opdExpense.EMPLOYEE_EMAILADDRESS,
                FINANCE_APPROVAL = opdExpense.FINANCE_APPROVAL,
                FINANCE_COMMENT = opdExpense.FINANCE_COMMENT,
                FINANCE_NAME = opdExpense.FINANCE_NAME,
                HOSPITAL_NAME = opdExpense.HOSPITAL_NAME,
                HR_APPROVAL = opdExpense.HR_APPROVAL,
                HR_COMMENT = opdExpense.HR_COMMENT,
                HR_NAME = opdExpense.HR_NAME,
                MANAGEMENT_APPROVAL = opdExpense.MANAGEMENT_APPROVAL,
                MANAGEMENT_COMMENT = opdExpense.MANAGEMENT_COMMENT,
                MANAGEMENT_NAME = opdExpense.MANAGEMENT_NAME,
                PERIOD_CONFINEMENT_DATE_FROM = opdExpense.PERIOD_CONFINEMENT_DATE_FROM,
                PERIOD_CONFINEMENT_DATE_TO = opdExpense.PERIOD_CONFINEMENT_DATE_TO,
                STATUS = opdExpense.STATUS,
                OPDTYPE = opdExpense.OPDTYPE,
                TOTAL_AMOUNT_CLAIMED = opdExpense.TOTAL_AMOUNT_CLAIMED,
                CLAIM_YEAR = opdExpense.CLAIM_YEAR,
                CREATED_DATE = opdExpense.CreatedDate,
                MODIFIED_DATE = opdExpense.ModifiedDate
            };

            return hospitalInformation;
        }



        #endregion

        private bool AuthenticateEmailAddress(int Id)
        {

            var opdInformation = GetOPDExpense(Convert.ToInt32(Id));
            OfficeManagerController managerController = new OfficeManagerController();

            string currentEmailAddress = managerController.GetEmailAddress();

            if (currentEmailAddress.Equals(opdInformation.EMPLOYEE_EMAILADDRESS))

                return true;
            else
                return false;

        }
    }
}