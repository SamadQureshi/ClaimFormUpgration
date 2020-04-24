using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


using Onion.Interfaces.Services;
using TCO.TFM.WDMS.ViewModels.ViewModels;
using Onion.Common.Constants;
using NLog;

namespace Onion.WebApp.Controllers
{
    public class HospitalExpenseController : Controller
    {       
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IOpdExpenseService _opdExpenseService;
        private readonly IOpdExpenseImageService _opdExpenseImageService;
        private readonly IOpdExpensePatientService _opdExpensePatientService;


        private const string UrlIndex = "Index";
        private const string UrlHome = "Home";
        private const string UrlOpdExpense = "OpdExpense";


        public HospitalExpenseController(IOpdExpenseService opdExpenseService, IOpdExpenseImageService opdExpenseImageService, IOpdExpensePatientService opdExpensePatientService)
        {
            _opdExpenseService = opdExpenseService;
            _opdExpenseImageService = opdExpenseImageService;
            _opdExpensePatientService = opdExpensePatientService;

        }

        // GET: OPDEXPENSEs
        public ActionResult Index()
        {
          
            return RedirectToAction(UrlIndex, UrlOpdExpense);


        }

        // GET: OPDEXPENSEs/Details/5
        public ActionResult Details(int? id)
        {

            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlOpdExpense);
                    }

                    var hospitalInformation = GetHospitalExpense(Convert.ToInt32(id));                  
                  
                    return View(hospitalInformation);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlOpdExpense);
                }

            }
            catch (Exception ex)
            {

                logger.Error("Hospital Expense : Details()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

        // GET: OPDEXPENSEs/Create
        public ActionResult Create()
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    return View();
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlOpdExpense);
                }
            }
            catch (Exception ex)
            {

                logger.Error("Hospital Expense : Create()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

        }

        // POST: OPDEXPENSEs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OpdExpenseVM opdExpense)
        {
            try
            {

                string buttonStatus = Request.Form["buttonName"];


                if (ModelState.IsValid)
                {
                    opdExpense.OpdType = FormType.HospitalExpense;
                    opdExpense.Status = ClaimStatus.INPROGRESS; 
                    opdExpense.CreatedDate = DateTime.Now;
                    opdExpense.EmployeeEmailAddress = GetEmailAddress();
                    OpdExpenseVM OpdExpense_Obj = _opdExpenseService.CreateOpdExpense(opdExpense);

                    return RedirectToAction("Edit", "HospitalExpense", new { id = OpdExpense_Obj.ID , opdType = opdExpense.OpdType });
                }

                return RedirectToAction(UrlIndex, UrlOpdExpense); 
            }
            catch (Exception ex)
            {

                logger.Error("Hospital Expense : Create()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

        // GET: OPDEXPENSEs/Edit/5
        public ActionResult Edit(int? id)
        {

            try
            {

                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();

                    if (id == null)
                    {
                        return RedirectToAction(UrlIndex, UrlOpdExpense);
                    }  
                    var hospitalInformation = GetHospitalExpense(Convert.ToInt32(id));                  

                    ViewData["OPDEXPENSE_ID"] = id;
                    ViewData["OPDTYPE"] = hospitalInformation.OpdType;


                    if (!AuthenticateEmailAddress(Convert.ToInt32(id)))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }


                   
                        return View(hospitalInformation);
                       
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlOpdExpense);
                }
            }
            catch (Exception ex)
            {

                logger.Error("Hospital Expense : Create()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

        }

        // POST: OPDEXPENSEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OpdExpenseVM opdExpense)
        {

            try
            {
                AuthenticateUser();
                var hospitalInformation = GetHospitalExpense(opdExpense.ID);
                ViewData["OPDEXPENSE_ID"] = opdExpense.ID;
                ViewData["OPDTYPE"] = opdExpense.OpdType;

                string buttonStatus = Request.Form["buttonName"];

                if (buttonStatus == "submit")
                {
                    opdExpense.Status = ClaimStatus.SUBMITTED;
                }
                else
                {
                    opdExpense.Status = ClaimStatus.INPROGRESS;
                }


                if (opdExpense.Status == ClaimStatus.SUBMITTED)
                {
                    if (hospitalInformation.OpdExpensePatients.Count > 0)
                    {
                        if (hospitalInformation.OpdExpenseImages.Count > 0)
                        {
                            if (GetHOSExpenseAmount(opdExpense, opdExpense.TotalAmountClaimed))
                            {
                                if (ModelState.IsValid)
                                {
                                    opdExpense.ModifiedDate = DateTime.Now;
                                    opdExpense.EmployeeEmailAddress = GetEmailAddress();
                                    _opdExpenseService.UpdateOpdExpense(opdExpense);
                                    return RedirectToAction(UrlIndex, UrlOpdExpense);
                                }

                            }
                            else
                            {
                                ModelState.AddModelError("", Constants.MSG_GENERAL_OPD_EXPENSE_AMOUNT);
                                return View(hospitalInformation);
                            }


                    }
                    else
                        {
                            ModelState.AddModelError("", Constants.MSG_GENERAL_ADD_PATIENT_RECEIPTS);
                            return View(hospitalInformation);
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", Constants.MSG_GENERAL_ADD_PATIENT_INFORMATION);
                        return View(hospitalInformation);
                    }
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        opdExpense.ModifiedDate = DateTime.Now;
                        opdExpense.EmployeeEmailAddress = GetEmailAddress();
                        _opdExpenseService.UpdateOpdExpense(opdExpense);                        
                        return RedirectToAction(UrlIndex, UrlOpdExpense);
                    }
                }

                           
                return View(hospitalInformation); 
            }
            catch (Exception ex)
            {

                logger.Error("Hospital Expense : Create()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

        // GET: OPDEXPENSEs/Delete/5
        public ActionResult Delete(int? id)
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
                        return RedirectToAction(UrlIndex, UrlOpdExpense);
                    }

                    _opdExpenseService.DeleteOpdExpense(id); 

                    return RedirectToAction(UrlIndex, UrlOpdExpense);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlOpdExpense);
                }

            }
            catch (Exception ex)
            {

                logger.Error("Hospital Expense : Create()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

        // POST: OPDEXPENSEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {

                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();
                    if (id == 0)
                    {
                        return RedirectToAction(UrlIndex, UrlOpdExpense);
                    }

                    else
                    {
                        _opdExpenseService.DeleteOpdExpense(id);
                    }

                    return RedirectToAction(UrlIndex, UrlOpdExpense);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlOpdExpense);
                }


            }
            catch (Exception ex)
            {

                logger.Error("Hospital Expense : Create()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }
     

        /// <summary>
        /// GET: /Img/DownloadFile
        /// </summary>
        /// <param name="fileId">File Id parameter</param>
        /// <returns>Return download file</returns>
        public ActionResult DownloadFile(int fileId)
        {
            // Model binding.          

            try
            {
                // Loading dile info.
                var fileInfo = _opdExpenseImageService.GetOpdExpensesImagesAgainstId(fileId);

                // Info.
                return this.GetFile(fileInfo.ImageBase64, fileInfo.ImageExt);
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // Info.
            return View();
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
            FileResult file;
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

        private string GetName()
        {
            OfficeManagerController managerController = new OfficeManagerController();
            string name = managerController.GetName();

            return name;

        }

        private void AuthenticateUser()
        {
           OfficeManagerController managerController = new OfficeManagerController();

            string emailAddress = GetEmailAddress();
            if (ValidEmailAddress(emailAddress))
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
                ClaimYear = opdExpense.ClaimYear,
                ClaimMonth = opdExpense.ClaimMonth,
                CreatedDate = opdExpense.CreatedDate,
                ModifiedDate = opdExpense.ModifiedDate,
                PhysicalDocumentReceived = opdExpense.PhysicalDocumentReceived,
                PayRollMonth = opdExpense.PayRollMonth
            };

            return hospitalInformation;
        }

        private bool GetHOSExpenseAmount(OpdExpenseVM opdExpense, decimal? totalAmountClaimed)
        {
            bool result = false;

            var hospitalInformation = GetHospitalExpense(opdExpense.ID);

            decimal? totalAmount = 0;

            foreach (var item in hospitalInformation.OpdExpenseImages)
            {
                totalAmount += item.ExpenseAmount;
            }

            if (totalAmount.Equals(totalAmountClaimed))
            {
                result = true;
            }

            return result;


        }


        private bool AuthenticateEmailAddress(int id)
        {

            var opdInformation = GetHospitalExpense(Convert.ToInt32(id));
            OfficeManagerController managerController = new OfficeManagerController();

            string currentEmailAddress = managerController.GetEmailAddress();

            if (currentEmailAddress.Equals(opdInformation.EmployeeEmailAddress))

                return true;
            else
                return false;
        }

      

    }
}
