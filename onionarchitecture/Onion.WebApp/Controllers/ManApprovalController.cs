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

        private const string UrlIndex = "Index";
        private const string UrlHome = "Home";
        private const string UrlManApproval = "ManApproval";
        private const string UrlIndexTravel = "IndexManTravel";
        public ManApprovalController(IOpdExpenseService opdExpenseService, IOpdExpenseImageService opdExpenseImageService, IOpdExpensePatientService opdExpensePatientService, ITravelExpenseService travelExpenseService, IEmailService emailService)
        {
            _opdExpenseService = opdExpenseService;
            _opdExpenseImageService = opdExpenseImageService;
            _opdExpensePatientService = opdExpensePatientService;
            _travelExpenseService = travelExpenseService;
            _emailService = emailService;
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
        public ActionResult DetailsForOPDExpense(int? id)
        {

            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser("DetailsForOPDExpense");

                    if (!(AuthenticateEmailAddress(Convert.ToInt32(id))))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlManApproval);
                    }

                    var result2 = GetOPDExpense(Convert.ToInt32(id));
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



        public ActionResult DetailsForHospitalExpense(int? id)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser("DetailsForHospitalExpense");

                    if (!(AuthenticateEmailAddress(Convert.ToInt32(id))))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlManApproval);
                    }

                    var result2 = GetHospitalExpense(Convert.ToInt32(id));

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
        public ActionResult MANOPDExpense(int? id)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser("MANOPDExpense");

                    if (!(AuthenticateEmailAddress(Convert.ToInt32(id))))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlManApproval);
                    }


                    var result2 = GetOPDExpense(Convert.ToInt32(id));

                    ViewData["OPDEXPENSE_ID"] = id;
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

                    var patients = _opdExpensePatientService.GetOpdExpensesPatientAgainstOpdExpenseId(oPDEXPENSE.ID);
                    oPDEXPENSE.OpdExpensePatients = patients;
                    var images = _opdExpenseImageService.GetOpdExpensesImageAgainstOpdExpenseId(oPDEXPENSE.ID);
                    oPDEXPENSE.OpdExpenseImages = images;

                    var emailMessage = EmailUtils.GetMailMessage(oPDEXPENSE);
                    _emailService.SendEmail(emailMessage);

                    return RedirectToAction(UrlIndex, UrlManApproval);
                }

                var result2 = GetOPDExpense(Convert.ToInt32(oPDEXPENSE.ID));

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
        public ActionResult MANHospitalExpense(int? id)
        {
            try
            {

                if (Request.IsAuthenticated)
                {
                    AuthenticateUser("MANHospitalExpense");

                    if (!(AuthenticateEmailAddress(Convert.ToInt32(id))))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlManApproval);
                    }

                    var result2 = GetHospitalExpense(Convert.ToInt32(id));

                    ViewData["OPDEXPENSE_ID"] = id;
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

                    return RedirectToAction(UrlIndex, UrlManApproval);
                }

                var result2 = GetHospitalExpense(Convert.ToInt32(oPDEXPENSE.ID));

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
        public ActionResult ManTravelExpense(int? id)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser("ManTravelExpense");

                    if (!(AuthenticateEmailAddress(Convert.ToInt32(id))))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndexTravel, UrlManApproval);
                    }


                    var result2 = GetTravelExpense(Convert.ToInt32(id));

                    ViewData["OPDEXPENSE_ID"] = id;
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
                    // oPDEXPENSE.FinanceApprovalDate = DateTime.Now;
                    // oPDEXPENSE.FinanceEmailAddress = GetEmailAddress();
                    //if (oPDEXPENSE.Status == ClaimStatus.MANGAPPROVED)
                    // {
                    //     oPDEXPENSE.ManagementApprovalDate = DateTime.Now;
                    //     oPDEXPENSE.ManagementEmailAddress = GetEmailAddress();
                    //     oPDEXPENSE.ManagementApproval = true;
                    // }
                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    return RedirectToAction(UrlIndexTravel, UrlManApproval);
                }

                var opdExpense = GetTravelExpense(Convert.ToInt32(oPDEXPENSE.ID));
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
                PhysicalDocumentReceived = opdExpense.PhysicalDocumentReceived,
                PayRollMonth = opdExpense.PayRollMonth

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
                PayRollMonth = opdExpense.PayRollMonth
            };

            return hospitalInformation;
        }

        private ManTravelVM GetTravelExpense(int Id)
        {
            OpdExpenseVM opdExpense = _opdExpenseService.GetOpdExpensesAgainstId(Id);

            var opdInformation = new ManTravelVM()
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
                HospitalName = opdExpense.HospitalName,

                EmployeeDepartment = opdExpense.EmployeeDepartment,
                EmployeeName = opdExpense.EmployeeName,
                EmployeeEmailAddress = opdExpense.EmployeeEmailAddress,


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
                PayRollMonth = opdExpense.PayRollMonth

            };

            return opdInformation;
        }
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

        private void AuthenticateUser(string IndexControllerName)
        {
            OfficeManagerController managerController = new OfficeManagerController();
            string emailAddress = GetEmailAddress();
            if (IndexControllerName == "IndexManTravel")
            {
                ViewBag.RollTypeTravel = "MANTRAVEL";
            }

            ViewBag.RollType = managerController.AuthenticateUser();

            ViewBag.UserName = managerController.GetName();

        }

        public bool ValidEmailAddress(string emailAddress)
        {

            bool result = false;

            List<OpdExpenseVM> list = _opdExpenseService.GetOpdExpensesForMANTravel(emailAddress);

            if (list.Count > 0)
            {
                result = true;
            }
            return result;
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
    }
}