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
using Onion.WebApp.Utils;
using TCO.TFM.WDMS.Common.Utils;

namespace Onion.WebApp.Controllers
{
    public class HrApprovalController : Controller
    {    
       private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IOpdExpenseService _opdExpenseService;
        private readonly IOpdExpenseImageService _opdExpenseImageService;
        private readonly IOpdExpensePatientService _opdExpensePatientService;
        private readonly IEmailService _emailService;
        private readonly ISetupExpenseAmountService _setupExpenseAmountService;

        public HrApprovalController(IOpdExpenseService opdExpenseService, IOpdExpenseImageService opdExpenseImageService, IOpdExpensePatientService opdExpensePatientService, IEmailService emailService, ISetupExpenseAmountService setupExpenseAmountService)
        {
            _opdExpenseService = opdExpenseService;
            _opdExpenseImageService = opdExpenseImageService;
            _opdExpensePatientService = opdExpensePatientService;
            _emailService = emailService;
            _setupExpenseAmountService = setupExpenseAmountService;
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

                    if (RollTypeStatus() == false)
                        return RedirectToAction(UrlIndex, UrlHome);

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

                logger.Error("HRAPPROVAL : Index()" + ex.Message.ToString());

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
                        RedirectToAction(UrlIndex, UrlHrApproval);
                    }

                    var result2 = GeneralController.GetOPDExpense(idDecrypted, _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);

                    return View(result2);
                }

                else
                {
                    return RedirectToAction(UrlIndex, UrlHrApproval);
                }

            }
            catch (Exception ex)
            {

                logger.Error("HRAPPROVAL : DetailsForOPDExpense()" + ex.Message.ToString());

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
                        RedirectToAction(UrlIndex, UrlHrApproval);
                    }                 

                    var result2 = GeneralController.GetHospitalExpense(idDecrypted, _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);

                    


                    return View(result2);
                }

                else
                {
                    return RedirectToAction(UrlIndex, UrlHrApproval);
                }

            }
            catch (Exception ex)
            {

                logger.Error("HRAPPROVAL : DetailsForHospitalExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }


        // GET: OPDEXPENSEs/Edit/5
        public ActionResult HROPDExpense(string id)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    int idDecrypted = Security.DecryptId(Convert.ToString(id));


                    if (!(AuthenticateEmailAddress(idDecrypted)))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        RedirectToAction(UrlIndex, UrlHrApproval);
                    }                 

                    var opdExpense = GeneralController.GetOPDExpense(idDecrypted, _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);

                    string remainingAmount = GeneralController.CalculateRemainingAmount(opdExpense.EmployeeEmailAddress, opdExpense.OpdType, opdExpense.HospitalizationType, opdExpense.MaternityType, _opdExpenseService, _setupExpenseAmountService, false);
                    ViewBag.RemainingAmount = remainingAmount;
                   
                    ViewData["OPDEXPENSE_ID"] = idDecrypted;
                    return View(opdExpense);
                }

                else
                {
                    return RedirectToAction(UrlIndex, UrlHrApproval);
                }

            }
            catch (Exception ex)
            {

                logger.Error("HRAPPROVAL : HROPDExpense()" + ex.Message.ToString());

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

                AuthenticateUser();

                string message = Validation(oPDEXPENSE, buttonStatus);

                if (message != string.Empty)
                {
                    ModelState.AddModelError("", message);
                }

                if (ModelState.IsValid)
                {
                    if (buttonStatus == "approved")
                    {
                        oPDEXPENSE.Status = ClaimStatus.HRAPPROVED;
                        oPDEXPENSE.HrApproval = true;
                    }
                    else if (buttonStatus == "rejected")
                    {
                        oPDEXPENSE.Status = ClaimStatus.HRREJECTED;                     
                    }
                    else if (buttonStatus == "managerapproval")
                    {
                        oPDEXPENSE.Status = ClaimStatus.MANINPROCESS;
                    }
                    else
                    {
                        oPDEXPENSE.Status = ClaimStatus.HRINPROCESS;
                    }

                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.HrApprovalDate = DateTime.Now;
                    oPDEXPENSE.HrEmailAddress = GetEmailAddress();                 

                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
         
                    EmailSend(oPDEXPENSE);

                    return RedirectToAction(UrlIndex, UrlHrApproval);

                }

                var opdExpense = GeneralController.GetOPDExpense(Convert.ToInt32(oPDEXPENSE.ID), _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);
                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.ID;
                return View(opdExpense);

            }
            catch (Exception ex)
            {

                logger.Error("HRAPPROVAL : HROPDExpense([Bind])" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }





        // GET: OPDEXPENSEs/Edit/5
        public ActionResult HRHospitalExpense(string id)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    int idDecrypted = Security.DecryptId(Convert.ToString(id));


                    if (!(AuthenticateEmailAddress(idDecrypted)))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        RedirectToAction(UrlIndex, UrlHrApproval);
                    }

                    var result2 = GeneralController.GetHospitalExpense(idDecrypted, _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);

                    string remainingAmount = GeneralController.CalculateRemainingAmount(result2.EmployeeEmailAddress, result2.OpdType, result2.HospitalizationType, result2.MaternityType, _opdExpenseService, _setupExpenseAmountService, false);
                    ViewBag.RemainingAmount = remainingAmount;


                    ViewData["OPDEXPENSE_ID"] = idDecrypted;

                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlHrApproval);
                }

            }
            catch (Exception ex)
            {

                logger.Error("HRAPPROVAL : HRHospitalExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

        }

        // POST: OPDEXPENSEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HRHospitalExpense(OpdExpenseVM oPDEXPENSE)
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
                    oPDEXPENSE.Status = ClaimStatus.HRAPPROVED;
                    oPDEXPENSE.HrApproval = true;

                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.Status = ClaimStatus.HRREJECTED;                    
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
                    oPDEXPENSE.HrApprovalDate = DateTime.Now;
                    oPDEXPENSE.HrEmailAddress = GetEmailAddress();                   

                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);

                    EmailSend(oPDEXPENSE);

                    return RedirectToAction(UrlIndex, UrlHrApproval);
                }
              
                var opdExpense = GeneralController.GetHospitalExpense(Convert.ToInt32(oPDEXPENSE.ID), _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);
                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.ID;
                return View(opdExpense);

            }
            catch (Exception ex)
            {
                logger.Error("HRAPPROVAL : HRHospitalExpense([Bind])" + ex.Message.ToString());

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

            UserAuthorization user = new UserAuthorization(_opdExpenseService);

            string userRoll = user.AuthenticateUser();

            if (user.ValidateEmailAddressManagerTravelApproval())
            {
                ViewBag.RollTypeTravel = "MANTRAVEL";
            }
            
             ViewBag.RollType = userRoll;          
             ViewBag.UserName = managerController.GetName();

        }
       
        #endregion

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


        private string Validation(OpdExpenseVM oPDEXPENSE, string buttonStatus)
        {
            string message = "";

            if (buttonStatus == "approved" || buttonStatus == "managerapproval" )
            {

                if (oPDEXPENSE.TotalAmountApproved.ToString() == "")
                {
                    message = Constants.MSG_APPROVAL_TOTALAMOUNTAPPROVED;
                }
                else if (oPDEXPENSE.HrComment == null)
                {
                    message = Constants.MSG_APPROVAL_HRCOMMENTS;
                }
                else if (!ClaimAmountAndTotalAmount(oPDEXPENSE))
                {
                    message = Constants.MSG_GENERAL_TOTALCLAIMEDAMOUNT_TOTALAPPROVEDAMOUNT;
                }
            } 
            else if( buttonStatus == "rejected")
            {
                if (oPDEXPENSE.HrComment == null)
                {
                    message = Constants.MSG_APPROVAL_HRCOMMENTS;
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
                if (ViewBag.RollType == "HR")
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