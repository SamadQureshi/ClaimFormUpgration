﻿using System;
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

namespace OPDCLAIMFORM.Controllers
{
    public class ManApprovalController : Controller
    {

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IOpdExpenseService _opdExpenseService;
        private readonly IOpdExpenseImageService _opdExpenseImageService;
        private readonly IOpdExpensePatientService _opdExpensePatientService;
        private readonly ITravelExpenseService _travelExpenseService;

        private const string UrlIndex = "Index";
        private const string UrlHome = "Home";
        private const string UrlManApproval = "ManApproval";
        private const string UrlIndexTravel = "IndexManTravel";
        public ManApprovalController(IOpdExpenseService opdExpenseService, IOpdExpenseImageService opdExpenseImageService, IOpdExpensePatientService opdExpensePatientService, ITravelExpenseService travelExpenseService)
        {
            _opdExpenseService = opdExpenseService;
            _opdExpenseImageService = opdExpenseImageService;
            _opdExpensePatientService = opdExpensePatientService;
            _travelExpenseService = travelExpenseService;
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
                    AuthenticateUser();

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
                    AuthenticateUser();

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
                    AuthenticateUser();

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
                    AuthenticateUser();

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
                AuthenticateUser();

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

                }
                return RedirectToAction(UrlIndex, UrlManApproval);

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
                    AuthenticateUser();

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
                AuthenticateUser();

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
                }
                return RedirectToAction(UrlIndex, UrlManApproval);

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
                    AuthenticateUser();

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

                AuthenticateUser();

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
                CreatedDate = opdExpense.CreatedDate,
                ModifiedDate = opdExpense.ModifiedDate
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
                ManagerName = opdExpense.ManagerName

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

        private void AuthenticateUser()
        {
            OfficeManagerController managerController = new OfficeManagerController();
            string emailAddress = GetEmailAddress();
            //if (ValidEmailAddress(emailAddress))
            //{
                ViewBag.RollTypeTravel = "MANTRAVEL";
            //}
           
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


        private string Validation(OpdExpenseVM oPDEXPENSE,string buttonStatus)
        {
            string message = "";

            if (buttonStatus == "rejected" || buttonStatus == "rejected")
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
            return message;
        }


        private bool ClaimAmountAndTotalAmount(OpdExpenseVM oPDEXPENSE)

        {
            bool result = false;

            if (oPDEXPENSE.TotalAmountClaimed <= oPDEXPENSE.TotalAmountApproved)
            {
                result = true;
            }

            return result;
        }
    }
}