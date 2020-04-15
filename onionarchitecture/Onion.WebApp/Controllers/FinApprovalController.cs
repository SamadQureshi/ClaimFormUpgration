
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

namespace OPDCLAIMFORM.Controllers
{
    public class FinApprovalController : Controller
    {
        //private readonly MedicalInfoEntities db = new MedicalInfoEntities();
        //private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        private readonly IOpdExpenseService _opdExpenseService;
        private readonly IOpdExpenseImageService _opdExpenseImageService;
        private readonly IOpdExpensePatientService _opdExpensePatientService;

        private const string UrlIndex = "Index";
        private const string UrlHome = "Home";
        private const string UrlFinApproval = "FinApproval";

        public FinApprovalController(IOpdExpenseService opdExpenseService, IOpdExpenseImageService opdExpenseImageService, IOpdExpensePatientService opdExpensePatientService)
        {
            _opdExpenseService = opdExpenseService;
            _opdExpenseImageService = opdExpenseImageService;
            _opdExpensePatientService = opdExpensePatientService;

        }






        // GET: OPDEXPENSEs
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            try
            {

                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();                  

                    string emailAddress = GetEmailAddress();
                  
                    var opdExp = _opdExpenseService.GetOpdExpensesForFIN();                   
                   
                    return View(opdExp);


                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlHome);
                }
            }
            catch (Exception ex)
            {

                //logger.Error("FINAPPROVAL : Index()" + ex.Message.ToString());

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
                        return RedirectToAction(UrlIndex, UrlFinApproval);
                    }
                  
                    var result2 = GetOPDExpense(Convert.ToInt32(id));
                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }

            }
            catch (Exception ex)
            {

                //logger.Error("FINAPPROVAL : DetailsForOPDExpense()" + ex.Message.ToString());

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
                        return RedirectToAction(UrlIndex, UrlFinApproval);
                    }             

                    var result2 = GetHOSExpense(Convert.ToInt32(id));
                   
                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }

            }
            catch (Exception ex)
            {

                //logger.Error("FINAPPROVAL : DetailsForHospitalExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }


        // GET: OPDEXPENSEs/Edit/5
        public ActionResult FINOPDExpense(int? id)
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
                        return RedirectToAction(UrlIndex, UrlFinApproval);
                    }


                    var result2 = GetOPDExpense(Convert.ToInt32(id));
                  
                    ViewData["OPDEXPENSE_ID"] = id;
                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }

            }
            catch (Exception ex)
            {

                //logger.Error("FinAPPROVAL : FINOPDExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

        // POST: OPDEXPENSEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FINOPDExpense(OpdExpenseVM oPDEXPENSE)
        {
            try
            {
                string buttonStatus = Request.Form["buttonName"];

                if (buttonStatus == "approved")
                {
                    oPDEXPENSE.STATUS = ClaimStatus.FINAPPROVED;

                    if (oPDEXPENSE.TOTAL_AMOUNT_APPROVED.ToString() == "")
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_TOTALAMOUNTAPPROVED);
                    }
                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.STATUS = ClaimStatus.FINREJECTED;

                    if (oPDEXPENSE.FINANCE_COMMENT == null)
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_FINANCECOMMENTS);
                    }
                }
                else
                {
                    oPDEXPENSE.STATUS = ClaimStatus.FININPROCESS;
                }


                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.FINANCE_APPROVAL_DATE = DateTime.Now;
                    oPDEXPENSE.FINANCE_EMAILADDRESS = GetEmailAddress();
                    if (oPDEXPENSE.STATUS == ClaimStatus.FINAPPROVED)
                    {
                        oPDEXPENSE.HR_APPROVAL = true;
                        oPDEXPENSE.FINANCE_APPROVAL = true;
                    }

                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }
               
                var opdExpense = GetOPDExpense(Convert.ToInt32(oPDEXPENSE.OPDEXPENSE_ID));
                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.OPDEXPENSE_ID;
                return View(opdExpense);
            }
            catch (Exception ex)
            {

                //logger.Error("FINAPPROVAL : FINOPDExpense([Bind])" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

        // GET: OPDEXPENSEs/Edit/5
        public ActionResult FINHospitalExpense(int? id)
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
                        return RedirectToAction(UrlIndex, UrlFinApproval);
                    }

                    //MedicalInfoEntities entities = new MedicalInfoEntities();
                    //OPDEXPENSE oPDEXPENSE = db.OPDEXPENSEs.Find(id);

                    var result2 = GetHOSExpense(Convert.ToInt32(id));
             

                    ViewData["OPDEXPENSE_ID"] = id;
                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }
            }
            catch (Exception ex)
            {

                //logger.Error("FINAPPROVAL : FINHospitalExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

        }

        // POST: OPDEXPENSEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FINHospitalExpense(OpdExpenseVM oPDEXPENSE)
        {
            try
            {
                string buttonStatus = Request.Form["buttonName"];

                if (buttonStatus == "approved")
                {
                    oPDEXPENSE.STATUS = ClaimStatus.FINAPPROVED;

                    if (oPDEXPENSE.TOTAL_AMOUNT_APPROVED.ToString() == "")
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_TOTALAMOUNTAPPROVED);
                    }

                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.STATUS = ClaimStatus.FINREJECTED;

                    if (oPDEXPENSE.FINANCE_COMMENT == null)
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_FINANCECOMMENTS);
                    }
                }
                else
                {
                    oPDEXPENSE.STATUS = ClaimStatus.HRINPROCESS;
                }

                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.FINANCE_APPROVAL_DATE = DateTime.Now;
                    oPDEXPENSE.FINANCE_EMAILADDRESS = GetEmailAddress();
                    if (oPDEXPENSE.STATUS == ClaimStatus.FINAPPROVED)
                    {
                        oPDEXPENSE.HR_APPROVAL = true;
                        oPDEXPENSE.FINANCE_APPROVAL = true;
                    }

                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }
                var opdExpense = GetHOSExpense(Convert.ToInt32(oPDEXPENSE.OPDEXPENSE_ID));
                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.OPDEXPENSE_ID;
                return View(opdExpense);


            }
            catch (Exception ex)
            {

               // logger.Error("FINAPPROVAL :  FINHospitalExpense([Bind])" + ex.Message.ToString());

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

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}



        #endregion

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

        private HospitalExpenseMasterDetail GetHOSExpense(int Id)
        {
            //MedicalInfoEntities entities = new MedicalInfoEntities();
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