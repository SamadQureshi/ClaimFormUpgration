using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Onion.Interfaces.Services;
using TCO.TFM.WDMS.ViewModels.ViewModels;
using Onion.WebApp.Controllers;
using Onion.Common.Constants;
using NLog;
using Onion.WebApp.Utils;
using TCO.TFM.WDMS.Common.Utils;

namespace OPDCLAIMFORM.Controllers
{
    public class ManApprovalController : Controller
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
        private const string UrlManApproval = "ManApproval";
        private const string UrlIndexTravel = "IndexManTravel";
        public ManApprovalController(IOpdExpenseService opdExpenseService, IOpdExpenseImageService opdExpenseImageService, IOpdExpensePatientService opdExpensePatientService, ITravelExpenseService travelExpenseService, IEmailService emailService, ISetupExpenseAmountService setupExpenseAmountService)
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
                    AuthenticateUser("Index");

                    string emailAddress = GetEmailAddress();

                    if (RollTypeStatus() == false)
                        return RedirectToAction(UrlIndex, UrlHome);

                    var opdExp = _opdExpenseService.GetOpdExpensesForMAN();

                    return View(opdExp);

                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlHome);

                }

            }
            catch (Exception ex)
            {

                logger.Error("MANAPPROVAL : Index()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

        public ActionResult IndexManTravel()
        {
            try
            {

                if (Request.IsAuthenticated)
                {
                    AuthenticateUser("IndexManTravel");

                    string emailAddress = GetEmailAddress();

                    //if (RollTypeStatus() == false)
                    //    return RedirectToAction(UrlIndex, UrlHome);


                    var opdExp = _opdExpenseService.GetOpdExpensesForMANTravel(emailAddress);

                    return View(opdExp);

                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlHome);

                }

            }
            catch (Exception ex)
            {

                logger.Error("MANAPPROVAL : Index()" + ex.Message.ToString());

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
                    AuthenticateUser("DetailsForOPDExpense");

                    int idDecrypted = Security.DecryptId(Convert.ToString(id));

                    if (!(AuthenticateEmailAddress(idDecrypted)))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlManApproval);
                    }

                    var result2 = GeneralController.GetOPDExpense(idDecrypted, _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);
                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlManApproval);
                }
            }
            catch (Exception ex)
            {

                logger.Error("MANAPPROVAL : DetailsForOPDExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }



        public ActionResult DetailsForHospitalExpense(string id)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser("DetailsForHospitalExpense");

                    int idDecrypted = Security.DecryptId(Convert.ToString(id));


                    if (!(AuthenticateEmailAddress(Convert.ToInt32(idDecrypted))))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlManApproval);
                    }

                    var result2 = GeneralController.GetHospitalExpense(Convert.ToInt32(idDecrypted), _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);

                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlManApproval);
                }
            }
            catch (Exception ex)
            {

                logger.Error("MANAPPROVAL : DetailsForHospitalExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }


        // GET: OPDEXPENSEs/Edit/5
        public ActionResult MANOPDExpense(string id)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser("MANOPDExpense");


                    int idDecrypted = Security.DecryptId(id);

                    if (!(AuthenticateEmailAddress(idDecrypted)))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlManApproval);
                    }


                    var result2 = GeneralController.GetOPDExpense(idDecrypted, _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);


                    string remainingAmount = GeneralController.CalculateRemainingAmount(result2.EmployeeEmailAddress, result2.OpdType, result2.HospitalizationType, result2.MaternityType, _opdExpenseService, _setupExpenseAmountService, false);
                    ViewBag.RemainingAmount = remainingAmount;


                    ViewData["OPDEXPENSE_ID"] = idDecrypted;
                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlManApproval);
                }

            }
            catch (Exception ex)
            {

                logger.Error("MANAPPROVAL : MANOPDExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

        // POST: OPDEXPENSEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MANOPDExpense(OpdExpenseVM oPDEXPENSE)
        {
            try
            {
                string buttonStatus = Request.Form["buttonName"];
                AuthenticateUser("MANOPDExpense");

                string message = Validation(oPDEXPENSE, buttonStatus);

                if (message != string.Empty)
                {
                    ModelState.AddModelError("", message);
                }


                if (buttonStatus == "approved")
                {
                    oPDEXPENSE.Status = ClaimStatus.MANAPPROVED;
                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.Status = ClaimStatus.MANREJECTED;
                }
                else
                {
                    oPDEXPENSE.Status = ClaimStatus.MANINPROCESS;
                }

                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.ManagementApprovalDate = DateTime.Now;
                    oPDEXPENSE.ManagementEmailAddress = GetEmailAddress();
                    if (oPDEXPENSE.Status == ClaimStatus.MANAPPROVED)
                    {
                        oPDEXPENSE.HrApproval = true;
                        oPDEXPENSE.FinanceApproval = true;
                        oPDEXPENSE.ManagementApproval = true;
                    }


                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);

                    EmailSend(oPDEXPENSE);

                    return RedirectToAction(UrlIndex, UrlManApproval);
                }

                var result2 = GeneralController.GetOPDExpense(Convert.ToInt32(oPDEXPENSE.ID), _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);

                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.ID;
                return View(result2);
            }
            catch (Exception ex)
            {

                logger.Error("MANAPPROVAL : MANOPDExpense([Bind])" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }





        // GET: OPDEXPENSEs/Edit/5
        public ActionResult MANHospitalExpense(string id)
        {
            try
            {

                if (Request.IsAuthenticated)
                {
                    AuthenticateUser("MANHospitalExpense");

                    int idDecrypted = Security.DecryptId(id);

                    if (!(AuthenticateEmailAddress(idDecrypted)))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlManApproval);
                    }

                    var result2 = GeneralController.GetHospitalExpense(idDecrypted, _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);


                    string remainingAmount = GeneralController.CalculateRemainingAmount(result2.EmployeeEmailAddress, result2.OpdType, result2.HospitalizationType, result2.MaternityType, _opdExpenseService, _setupExpenseAmountService, false);
                    ViewBag.RemainingAmount = remainingAmount;

                    ViewData["OPDEXPENSE_ID"] = idDecrypted;
                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlManApproval);
                }

            }
            catch (Exception ex)
            {

                logger.Error("MANAPPROVAL : MANHospitalExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

        }

        // POST: OPDEXPENSEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MANHospitalExpense(OpdExpenseVM oPDEXPENSE)
        {

            try
            {
                string buttonStatus = Request.Form["buttonName"];
                AuthenticateUser("MANHospitalExpense");

                string message = Validation(oPDEXPENSE, buttonStatus);

                if (message != string.Empty)
                {
                    ModelState.AddModelError("", message);
                }


                if (buttonStatus == "approved")
                {
                    oPDEXPENSE.Status = ClaimStatus.MANAPPROVED;
                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.Status = ClaimStatus.MANREJECTED;
                }
                else
                {
                    oPDEXPENSE.Status = ClaimStatus.MANINPROCESS;
                }

                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.ManagementApprovalDate = DateTime.Now;
                    oPDEXPENSE.ManagementEmailAddress = GetEmailAddress();
                    if (oPDEXPENSE.Status == ClaimStatus.MANAPPROVED)
                    {
                        oPDEXPENSE.HrApproval = true;
                        oPDEXPENSE.FinanceApproval = true;
                        oPDEXPENSE.ManagementApproval = true;
                    }

                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    EmailSend(oPDEXPENSE);
                    return RedirectToAction(UrlIndex, UrlManApproval);
                }

                var result2 = GeneralController.GetHospitalExpense(Convert.ToInt32(oPDEXPENSE.ID), _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);

                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.ID;
                return View(result2);
            }
            catch (Exception ex)
            {

                logger.Error("MANAPPROVAL : MANHospitalExpense([Bind])" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }


        // GET: OPDEXPENSEs/Edit/5
        public ActionResult ManTravelExpense(string id)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser("ManTravelExpense");

                    int idDecrypted = Security.DecryptId(id);

                    if (!(AuthenticateEmailAddress(idDecrypted)))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndexTravel, UrlManApproval);
                    }


                    var result2 = GeneralController.GetTravelExpense(idDecrypted, _opdExpenseService, _travelExpenseService);

                    ViewData["OPDEXPENSE_ID"] = idDecrypted;
                    return View(result2);
                }
                else
                {
                    return RedirectToAction(UrlIndexTravel, UrlManApproval);
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
        public ActionResult ManTravelExpense(OpdExpenseVM oPDEXPENSE)
        {
            try
            {
                string buttonStatus = Request.Form["buttonName"];

                AuthenticateUser("ManTravelExpense");

                string message = Validation(oPDEXPENSE, buttonStatus);

                if (message != string.Empty)
                {
                    ModelState.AddModelError("", message);
                }


                if (buttonStatus == "approved")
                {
                    oPDEXPENSE.Status = ClaimStatus.MANGAPPROVED;
                    oPDEXPENSE.ManagementApprovalDate = DateTime.Now;
                    oPDEXPENSE.ManagementEmailAddress = GetEmailAddress();
                    oPDEXPENSE.ManagementApproval = true;

                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.Status = ClaimStatus.MANGREJECT;
                    oPDEXPENSE.ManagementEmailAddress = GetEmailAddress();

                }
                else
                {
                    oPDEXPENSE.Status = ClaimStatus.MANGINPROCESS;
                }


                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;               
                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    EmailSend(oPDEXPENSE);
                    return RedirectToAction(UrlIndexTravel, UrlManApproval);
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

        private void AuthenticateUser(string IndexControllerName)
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

        private string GetEmailAddress()
        {
            OfficeManagerController managerController = new OfficeManagerController();
            string emailAddress = managerController.GetEmailAddress();

            return emailAddress;
        }


        private string Validation(OpdExpenseVM oPDEXPENSE, string buttonStatus)
        {
            string message = "";

            if (buttonStatus == "approved")
            {
                if (oPDEXPENSE.ManagementComment == null)
                {
                    message = Constants.MSG_APPROVAL_MANAGERCOMMENTS;
                }

                if (oPDEXPENSE.TotalAmountApproved.ToString() == "")
                {
                    message = Constants.MSG_APPROVAL_TOTALAMOUNTAPPROVED;
                }

                else if (!ClaimAmountAndTotalAmount(oPDEXPENSE))
                {
                    message = Constants.MSG_GENERAL_TOTALCLAIMEDAMOUNT_TOTALAPPROVEDAMOUNT;
                }
            }
            else if (buttonStatus == "rejected")
            {
                if (oPDEXPENSE.ManagementComment == null)
                {
                    message = Constants.MSG_APPROVAL_MANAGERCOMMENTS;
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
                if (ViewBag.RollType == "MAN")
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