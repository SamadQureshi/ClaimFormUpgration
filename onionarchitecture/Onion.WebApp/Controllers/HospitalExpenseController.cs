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
using TCO.TFM.WDMS.Common.Utils;
using Onion.WebApp.Utils;

namespace Onion.WebApp.Controllers
{
    public class HospitalExpenseController : Controller
    {       
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IOpdExpenseService _opdExpenseService;
        private readonly IOpdExpenseImageService _opdExpenseImageService;
        private readonly IOpdExpensePatientService _opdExpensePatientService;
        private readonly IEmailService _emailService;
        private readonly ISetupExpenseAmountService _setupExpenseAmountService;

        private const string UrlIndex = "Index";
        private const string UrlHome = "Home";
        private const string UrlOpdExpense = "OpdExpense";


        public HospitalExpenseController(IOpdExpenseService opdExpenseService, IOpdExpenseImageService opdExpenseImageService, IOpdExpensePatientService opdExpensePatientService,
            IEmailService emailService,ISetupExpenseAmountService setupExpenseAmountService)
        {
            _opdExpenseService = opdExpenseService;
            _opdExpenseImageService = opdExpenseImageService;
            _opdExpensePatientService = opdExpensePatientService;
            _emailService = emailService;
            _setupExpenseAmountService = setupExpenseAmountService;
        }

        // GET: OPDEXPENSEs
        public ActionResult Index()
        {
          
            return RedirectToAction(UrlIndex, UrlOpdExpense);


        }

        // GET: OPDEXPENSEs/Details/5
        public ActionResult Details(string id)
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

                    int idDecrypted = Security.DecryptId(Convert.ToString(id));

                    var hospitalInformation = GeneralController.GetHospitalExpense(Convert.ToInt32(idDecrypted), _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);                  
                  
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

                    return RedirectToAction("Edit", "HospitalExpense", new { id = Security.EncryptId(OpdExpense_Obj.ID)});
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
        public ActionResult Edit(string id)
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

                    int idDecrypted = Security.DecryptId(Convert.ToString(id));

                    var hospitalInformation = GeneralController.GetHospitalExpense(Convert.ToInt32(idDecrypted), _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);                   

                    ViewData["OPDEXPENSE_ID"] = idDecrypted;
                    ViewData["OPDTYPE"] = hospitalInformation.OpdType;
                    ViewBag.EmployeeDepartment = hospitalInformation.EmployeeDepartment;

                    string remainingAmount = GeneralController.CalculateRemainingAmount(hospitalInformation.EmployeeEmailAddress, hospitalInformation.OpdType, _opdExpenseService,_setupExpenseAmountService);
                    ViewBag.RemainingAmount = remainingAmount;

                    if (!AuthenticateEmailAddress(Convert.ToInt32(idDecrypted)))
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
                var hospitalInformation = GeneralController.GetHospitalExpense(opdExpense.ID, _opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);
                ViewData["OPDEXPENSE_ID"] = opdExpense.ID;
                ViewData["OPDTYPE"] = opdExpense.OpdType;
                ViewBag.EmployeeDepartment = hospitalInformation.EmployeeDepartment; 
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
                                    EmailSend(opdExpense);
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
                        EmailSend(opdExpense);
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

            UserAuthorization user = new UserAuthorization(_opdExpenseService);

            string userRoll = user.AuthenticateUser();

            if (user.ValidateEmailAddressManagerTravelApproval())
            {
                ViewBag.RollTypeTravel = "MANTRAVEL";
            }

            ViewBag.RollType = userRoll;          

            ViewBag.UserName = managerController.GetName();

        }       


        private bool GetHOSExpenseAmount(OpdExpenseVM opdExpense, decimal? totalAmountClaimed)
        {
            bool result = false;

            var hospitalInformation = GeneralController.GetHospitalExpense(opdExpense.ID,_opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);

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

            var opdInformation = GeneralController.GetHospitalExpense(Convert.ToInt32(id),_opdExpenseService, _opdExpensePatientService, _opdExpenseImageService);
            OfficeManagerController managerController = new OfficeManagerController();

            string currentEmailAddress = managerController.GetEmailAddress();

            if (currentEmailAddress.Equals(opdInformation.EmployeeEmailAddress))

                return true;
            else
                return false;
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

    }
}
