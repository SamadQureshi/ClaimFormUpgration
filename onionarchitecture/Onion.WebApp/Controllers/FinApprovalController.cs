
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
using NLog;
using TCO.TFM.WDMS.Common.Utils;
using Onion.WebApp.Utils;

namespace OPDCLAIMFORM.Controllers
{
    public class FinApprovalController : Controller
    {

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IOpdExpenseService _opdExpenseService;
        private readonly IOpdExpenseImageService _opdExpenseImageService;
        private readonly IOpdExpensePatientService _opdExpensePatientService;
        private readonly ITravelExpenseService _travelExpenseService;
        private readonly IEmailService _emailService;
        private readonly ISetupExpenseAmountService _setupExpenseAmountService;

        private const string UrlIndex = "Index";
        private const string UrlHome = "Home";
        private const string UrlFinApproval = "FinApproval";

        public FinApprovalController(IOpdExpenseService opdExpenseService, IOpdExpenseImageService opdExpenseImageService, IOpdExpensePatientService opdExpensePatientService, ITravelExpenseService travelExpenseService, IEmailService emailService, ISetupExpenseAmountService setupExpenseAmountService)
        {
            _opdExpenseService = opdExpenseService;
            _opdExpenseImageService = opdExpenseImageService;
            _opdExpensePatientService = opdExpensePatientService;
            _travelExpenseService = travelExpenseService;
            _emailService = emailService;
            _setupExpenseAmountService = setupExpenseAmountService;
        }

        // GET: OPDEXPENSEs
        public ActionResult Index()
        {
            try
            {

                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();                  

                    string emailAddress = GetEmailAddress();

                 if(RollTypeStatus() == false)                      
                        return RedirectToAction(UrlIndex, UrlHome);
                   
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

                logger.Error("FINAPPROVAL : Index()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

        // GET: OPDEXPENSEs/Details/5
        public ActionResult DetailsForOPDExpense(string id)
        {
         
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    int idDecrypted = Security.DecryptId(id);

                    if (!(AuthenticateEmailAddress(idDecrypted)))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlFinApproval);
                    }
                 
                     var result2 = GeneralController.GetOPDExpense(idDecrypted, _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);
                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }

            }
            catch (Exception ex)
            {

                logger.Error("FINAPPROVAL : DetailsForOPDExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }
                      
        public ActionResult DetailsForHospitalExpense(string id)
        {          
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    int idDecrypted = Security.DecryptId(id);

                    if (!(AuthenticateEmailAddress(idDecrypted)))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlFinApproval);
                    }
                  
                    var result2 = GeneralController.GetHospitalExpense(idDecrypted, _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);
                   
                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }

            }
            catch (Exception ex)
            {

                logger.Error("FINAPPROVAL : DetailsForHospitalExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }
        
        // GET: OPDEXPENSEs/Edit/5
        public ActionResult FINOPDExpense(string id)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    int idDecrypted = Security.DecryptId(id);

                    if (!(AuthenticateEmailAddress(idDecrypted)))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlFinApproval);
                    }
                   
                    var result2 = GeneralController.GetOPDExpense(idDecrypted, _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);


                    string remainingAmount = GeneralController.CalculateRemainingAmount(result2.EmployeeEmailAddress, result2.OpdType, result2.HospitalizationType , result2.MaternityType, _opdExpenseService, _setupExpenseAmountService, false);
                    ViewBag.RemainingAmount = remainingAmount;

                    ViewData["OPDEXPENSE_ID"] = idDecrypted;
                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }

            }
            catch (Exception ex)
            {

                logger.Error("FinAPPROVAL : FINOPDExpense()" + ex.Message.ToString());

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

                AuthenticateUser();

                string message = Validation(oPDEXPENSE, buttonStatus);

                if (message != string.Empty)
                {
                    ModelState.AddModelError("", message);
                }

                if (buttonStatus == "approved")
                {
                    oPDEXPENSE.Status = ClaimStatus.FINAPPROVED;
                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.Status = ClaimStatus.FINREJECTED;
                  
                }
                else if (buttonStatus == "finalapproved")
                {
                    oPDEXPENSE.Status = ClaimStatus.COMPLETED;
                }
                else if (buttonStatus == "managerapproval")
                {
                    oPDEXPENSE.Status = ClaimStatus.MANINPROCESS;
                }
                else
                {
                    oPDEXPENSE.Status = ClaimStatus.FININPROCESS;
                }


                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.FinanceApprovalDate = DateTime.Now;
                    oPDEXPENSE.FinanceEmailAddress = GetEmailAddress();
                    if (oPDEXPENSE.Status == ClaimStatus.FINAPPROVED)
                    {
                        oPDEXPENSE.HrApproval = true;
                        oPDEXPENSE.FinanceApproval = true;
                    }
                    else if (oPDEXPENSE.Status == ClaimStatus.MANAPPROVED)
                    {

                        oPDEXPENSE.ManagementApproval = true;
                    }
                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    EmailSend(oPDEXPENSE);
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }               
                var opdExpense = GeneralController.GetOPDExpense(Convert.ToInt32(oPDEXPENSE.ID), _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);
                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.ID;
                return View(opdExpense);
            }
            catch (Exception ex)
            {

                logger.Error("FINAPPROVAL : FINOPDExpense([Bind])" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }






        // GET: OPDEXPENSEs/Edit/5
        public ActionResult FINHospitalExpense(string id)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    int idDecrypted = Security.DecryptId(id);

                    if (!(AuthenticateEmailAddress(idDecrypted)))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlFinApproval);
                    }
                  
                    var result2 = GeneralController.GetHospitalExpense(idDecrypted, _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);

                    string remainingAmount = GeneralController.CalculateRemainingAmount(result2.EmployeeEmailAddress, result2.OpdType, result2.HospitalizationType, result2.MaternityType, _opdExpenseService, _setupExpenseAmountService, false);
                    ViewBag.RemainingAmount = remainingAmount;


                    ViewData["OPDEXPENSE_ID"] = idDecrypted;
                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }
            }
            catch (Exception ex)
            {

                logger.Error("FINAPPROVAL : FINHospitalExpense()" + ex.Message.ToString());

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

                AuthenticateUser();

                string message = Validation(oPDEXPENSE, buttonStatus);

                if (message != string.Empty)
                {
                    ModelState.AddModelError("", message);
                }              

                if (buttonStatus == "approved")
                {
                    oPDEXPENSE.Status = ClaimStatus.FINAPPROVED;                 
                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.Status = ClaimStatus.FINREJECTED;                   
                }
                else if (buttonStatus == "finalapproved")
                {
                    oPDEXPENSE.Status = ClaimStatus.COMPLETED;
                }
                else if (buttonStatus == "managerapproval")
                {
                    oPDEXPENSE.Status = ClaimStatus.MANINPROCESS;
                }
                else
                {
                    oPDEXPENSE.Status = ClaimStatus.HRINPROCESS;
                }

                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.FinanceApprovalDate = DateTime.Now;
                    oPDEXPENSE.FinanceEmailAddress = GetEmailAddress();
                    if (oPDEXPENSE.Status == ClaimStatus.FINAPPROVED)
                    {
                        oPDEXPENSE.HrApproval = true;
                        oPDEXPENSE.FinanceApproval = true;
                    }
                    else if (oPDEXPENSE.Status == ClaimStatus.MANAPPROVED)
                    {

                        oPDEXPENSE.ManagementApproval = true;
                    }
                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    EmailSend(oPDEXPENSE);
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }

               
                var opdExpense = GeneralController.GetHospitalExpense(Convert.ToInt32(oPDEXPENSE.ID), _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);
                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.ID;
                return View(opdExpense);


            }
            catch (Exception ex)
            {

                logger.Error("FINAPPROVAL :  FINHospitalExpense([Bind])" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }



        // GET: OPDEXPENSEs/Edit/5
        public ActionResult FINTravelExpense(string id)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    int idDecrypted = Security.DecryptId(id);

                    if (!(AuthenticateEmailAddress(idDecrypted)))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlFinApproval);
                    }
                  
                    var result2 = GeneralController.GetTravelExpense(idDecrypted, _opdExpenseService,_travelExpenseService);

                    ViewData["OPDEXPENSE_ID"] = idDecrypted;
                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }

            }
            catch (Exception ex)
            {

                logger.Error("FinAPPROVAL : FINOPDExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

        // POST: OPDEXPENSEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FINTravelExpense(OpdExpenseVM oPDEXPENSE)
        {
            try
            {
                string buttonStatus = Request.Form["buttonName"];

                AuthenticateUser();

                string message = Validation(oPDEXPENSE , buttonStatus);

                if (message != string.Empty)
                {
                    ModelState.AddModelError("", message);
                }

                if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.Status = ClaimStatus.FINREJECTED;                  
                }
                else if (buttonStatus == "finalapproved")
                {
                    oPDEXPENSE.Status = ClaimStatus.COMPLETED;
                }
                else if (buttonStatus == "managerapproval") 
                {
                    oPDEXPENSE.Status = ClaimStatus.MANGINPROCESS;
                }
                else
                {
                    oPDEXPENSE.Status = ClaimStatus.FININPROCESS;
                }

                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.FinanceApprovalDate = DateTime.Now;
                    oPDEXPENSE.FinanceEmailAddress = GetEmailAddress();
                   
                    if (oPDEXPENSE.Status == ClaimStatus.COMPLETED)
                    {
                       oPDEXPENSE.FinanceApproval = true;
                    }                      

                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    EmailSend(oPDEXPENSE);
                    return RedirectToAction(UrlIndex, UrlFinApproval);
                }
               
                var opdExpense = GeneralController.GetTravelExpense(Convert.ToInt32(oPDEXPENSE.ID), _opdExpenseService, _travelExpenseService);
                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.ID;
                return View(opdExpense);
            }
            catch (Exception ex)
            {

                logger.Error("FINAPPROVAL : FINOPDExpense([Bind])" + ex.Message.ToString());

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

            UserAuthorization user = new UserAuthorization(_opdExpenseService);

            string userRoll = user.AuthenticateUser();

            if (user.ValidateEmailAddressManagerTravelApproval())
            {
                ViewBag.RollTypeTravel = "MANTRAVEL";
            }
           
            ViewBag.RollType = userRoll;        
            
            ViewBag.UserName = managerController.GetName();

        }

        private bool AuthenticateEmailAddress(int Id)
        {
            
            var opdInformation = GeneralController.GetOPDExpense(Convert.ToInt32(Id), _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);
            OfficeManagerController managerController = new OfficeManagerController();

            string currentEmailAddress = managerController.GetEmailAddress();

            if (currentEmailAddress.Equals(opdInformation.EmployeeEmailAddress))

                return true;
            else
                return false;

        }


        private string Validation(OpdExpenseVM oPDEXPENSE ,string buttonStatus)
        {
            string message = "";

            if (buttonStatus == "managerapproval" || buttonStatus == "finalapproved"){

                if (oPDEXPENSE.TotalAmountApproved.ToString() == "")
                {
                    message = Constants.MSG_APPROVAL_TOTALAMOUNTAPPROVED;
                }
                else if (oPDEXPENSE.FinanceComment == null)
                {
                    message = Constants.MSG_APPROVAL_FINANCECOMMENTS;
                }
                else if (!ClaimAmountAndTotalAmount(oPDEXPENSE))
                {
                    message = Constants.MSG_GENERAL_TOTALCLAIMEDAMOUNT_TOTALAPPROVEDAMOUNT;
                }
                else if(buttonStatus == "finalapproved")
                {
                    if(oPDEXPENSE.PayRollMonth == null)
                    {
                        message = Constants.MSG_GENERAL_ADD_PAYROLL_MONTH;
                    }
                }

            }
            else if(buttonStatus == "rejected" || buttonStatus == "approved"){
                if (oPDEXPENSE.FinanceComment == null)
                {
                    message = Constants.MSG_APPROVAL_FINANCECOMMENTS;
                }
            }
            return message;
        }


        private bool ClaimAmountAndTotalAmount(OpdExpenseVM oPDEXPENSE)
            
        {
            bool result = false;
            
            if (oPDEXPENSE.TotalAmountApproved <= oPDEXPENSE.TotalAmountClaimed)
            {
                result = true;
            }

            return result;
        }

        public void EmailSend(OpdExpenseVM OpdExpense)
        {
            var patients = _opdExpensePatientService.GetOpdExpensesPatientAgainstOpdExpenseId(OpdExpense.ID);
            OpdExpense.OpdExpensePatients = patients;
            var images = _opdExpenseImageService.GetOpdExpensesImageAgainstOpdExpenseId(OpdExpense.ID);
            OpdExpense.OpdExpenseImages = images;
            var message = EmailUtils.GetMailMessage(OpdExpense);
            _emailService.SendEmail(message);
        }

        public bool RollTypeStatus()
        {
            bool result = false;

            if (ViewBag.RollType != string.Empty)
            {
                if (ViewBag.RollType == "FIN") 
                { 
                    result = true; 
                }
                else if (ViewBag.RollType == "GEN") 
                { 
                    result = true; 
                }
            }

            return result;
        }

    }
}