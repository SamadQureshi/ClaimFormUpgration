using NLog;
using Onion.Common.Constants;
using Onion.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TCO.TFM.WDMS.Common.Utils;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.WebApp.Controllers
{
    public class TravelExpenseController : Controller
    {

        private readonly ITravelExpenseService _travelExpenseService;
        private readonly IOpdExpenseService _opdExpenseService;   
        private const string UrlIndex = "Index";
        private const string UrlHome = "Home";
        private const string UrlOpdExpense = "OpdExpense";
        private const string UrlTravelExpense = "TravelExpense";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public TravelExpenseController(ITravelExpenseService travelExpenseService,IOpdExpenseService opdExpenseService)
        {
            _travelExpenseService = travelExpenseService;
            _opdExpenseService = opdExpenseService;

        }
        public ActionResult Index(string id)
        {
            if (Request.IsAuthenticated)
            {
                AuthenticateUser();

                int idDecrypted = Security.DecryptId(Convert.ToString(id));

                var opdExpenseService = _opdExpenseService.GetOpdExpensesAgainstId(idDecrypted);

                ViewData["OPDTYPE"] = opdExpenseService.OpdType;
                ViewData["OPDEXPENSE_ID"] = idDecrypted;

                ImgTravelModel model = new ImgTravelModel { FileAttach = null, ImgLst = new List<TravelExpenseVM>() };

                model.ImgLst = _travelExpenseService.GetTravelExpensesAgainstOpdExpenseId(idDecrypted);

                model.OPDExpenseID = idDecrypted;
                return this.View(model);
            }
            else
            {
                return RedirectToAction(UrlIndex, UrlHome);

            }

        }


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

                logger.Error("OPD Expense : Create()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

        }

        // POST: OPDEXPENSEs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OpdExpenseVM OpdExpense)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    OpdExpense.Status = ClaimStatus.INPROGRESS;
                    OpdExpense.CreatedDate = DateTime.Now;
                    OpdExpense.EmployeeEmailAddress = GetEmailAddress();

                    OpdExpenseVM OpdExpense_Obj = _opdExpenseService.CreateOpdExpense(OpdExpense);

                    ViewData["OPDEXPENSE_ID"] = OpdExpense_Obj.ID;
                    ViewData["OPDTYPE"] = OpdExpense_Obj.OpdType;

                    if (OpdExpense.OpdType == FormType.OPDExpense)
                        return RedirectToAction("Edit", UrlOpdExpense, new { id = Security.EncryptId(OpdExpense_Obj.ID)});
                    else if (OpdExpense.OpdType == FormType.EmployeeExpense)
                        return RedirectToAction("Edit", UrlTravelExpense, new { id = Security.EncryptId(OpdExpense_Obj.ID) });
                }
                return View(OpdExpense);
            }
            catch (Exception ex)
            {
                logger.Error("OPD Expense : Create([Bind])" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }



        #region POST: /Img/Index

        /// <summary>
        /// POST: /Img/Index
        /// </summary>
        /// <param name="model">Model parameter</param>
        /// <returns>Return - Response information</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ImgTravelModel model)
        {

            if (Request.IsAuthenticated)
            {
                AuthenticateUser();

                if (ModelState.IsValid)
                {
                    // Converting to bytes.
                    byte[] uploadedFile = new byte[model.FileAttach.InputStream.Length];
                    model.FileAttach.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                    TravelExpenseVM opdExpense_Image = new TravelExpenseVM();

                    ViewData["OPDTYPE"] = model.OPDType;

                    if (model.OPDExpenseID == 0)
                    {
                        model.OPDExpenseID = Security.DecryptId(Request.Url.Segments[3].ToString());
                    }
                    string emailAddress = GetEmailAddress();

                    ViewData["OPDEXPENSE_ID"] = model.OPDExpenseID;
                    // Initialization.
                    opdExpense_Image.OpdExpenseId = model.OPDExpenseID;
                    opdExpense_Image.ImageBase64 = Convert.ToBase64String(uploadedFile);
                    opdExpense_Image.ImageExt = model.FileAttach.ContentType;
                    opdExpense_Image.CreatedDate = DateTime.Now;
                    opdExpense_Image.ImageName = model.FileAttach.FileName;
                    opdExpense_Image.Description = model.Description;
                    opdExpense_Image.Amount = model.Amount;
                    opdExpense_Image.ExpenseType = model.ExpenseType;
                    TravelExpenseVM OpdExpensePatient_Obj = _travelExpenseService.CreateTravelExpense(opdExpense_Image, emailAddress);

                    ImgTravelModel modelUploaded = new ImgTravelModel { FileAttach = null, ImgLst = new List<TravelExpenseVM>() };
                    ModelState.Clear();

                    //// Settings.
                    modelUploaded.ImgLst = _travelExpenseService.GetTravelExpensesAgainstOpdExpenseId(Convert.ToInt32(model.OPDExpenseID));

                    // Info
                    return this.View(modelUploaded);
                }
                else
                {

                    if (model.OPDExpenseID == 0)
                    {
                        model.OPDExpenseID = Security.DecryptId(Request.Url.Segments[3].ToString());
                    }
                    model.ImgLst = _travelExpenseService.GetTravelExpensesAgainstOpdExpenseId(Convert.ToInt32(model.OPDExpenseID));

                    // Info
                    return this.View(model);
                }
            }
            else
            {
                return RedirectToAction(UrlIndex, UrlHome);

            }
           
        }

       
        /// <summary>
        /// GET: /Img/DownloadFile
        /// </summary>
        /// <param name="fileId">File Id parameter</param>
        /// <returns>Return download file</returns>
        public ActionResult DownloadFile(int fileId)
        {

            var fileInfo = _travelExpenseService.GetTravelExpenseAgainstId(fileId);

            return this.GetFile(fileInfo.ImageBase64, fileInfo.ImageExt);

        }


        // POST: OPDEXPENSEIMAGE/Delete/5
        public ActionResult Delete(int id, int opdexpenseid)
        {

            if (Request.IsAuthenticated)
            {
                AuthenticateUser();
                string emailAddress = GetEmailAddress();
                _travelExpenseService.DeleteTravelExpense(id, emailAddress);

                // Info.
                return RedirectToAction(UrlIndex, "TravelExpense", new { id = Security.EncryptId(opdexpenseid)});
            }
            else
            {
                return RedirectToAction(UrlIndex, UrlHome);

            }
        }

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

        [HttpPost]
        public ActionResult DeleteTravelExpense(int id)
        {
            try
            {
                string emailAddress = GetEmailAddress();
                // Loading dile info.
                _travelExpenseService.DeleteTravelExpense(id, emailAddress);
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // Info.
            return new EmptyResult();
        }



        #endregion




        #region Travel Expense 

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

                    var opdInformation = GetTravelExpense(Convert.ToInt32(idDecrypted));
                    ViewData["OPDEXPENSE_ID"] = idDecrypted;
                    ViewData["OPDTYPE"] = opdInformation.OpdType;
                    ViewBag.EmployeeDepartment = opdInformation.EmployeeDepartment;

                    if (!(AuthenticateEmailAddress(Convert.ToInt32(idDecrypted))))
                    {
                        return RedirectToAction(UrlIndex, UrlHome);
                    }


                    return View(opdInformation);



                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlOpdExpense);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Travel Expense : Edit()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

        }

        // POST: OPDEXPENSEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OpdExpenseVM OpdExpense)
        {
            try
            {
                string buttonStatus = Request.Form["buttonName"];

                AuthenticateUser();
                var opdInformation = GetTravelExpense(OpdExpense.ID);
                ViewData["OPDEXPENSE_ID"] = OpdExpense.ID;
                ViewData["OPDTYPE"] = OpdExpense.OpdType;
                ViewBag.EmployeeDepartment = opdInformation.EmployeeDepartment;

                if (buttonStatus == "submit")
                {
                    OpdExpense.Status = ClaimStatus.FININPROCESS;
                }
                else
                {
                    OpdExpense.Status = ClaimStatus.INPROGRESS;
                }


                if (OpdExpense.Status == ClaimStatus.FININPROCESS)
                {                  
                    if (opdInformation.ListTravelExpense.Count > 0)
                        {

                            if (GetTravelExpenseAmount(OpdExpense.ID, OpdExpense.TotalAmountClaimed))
                            {
                                if (ModelState.IsValid)
                                {
                                    OpdExpense.ModifiedDate = DateTime.Now;
                                    OpdExpense.EmployeeEmailAddress = GetEmailAddress();
                                    _opdExpenseService.UpdateOpdExpense(OpdExpense);
                                    return RedirectToAction(UrlIndex, UrlOpdExpense);
                                }

                            }
                            else
                            {
                                ModelState.AddModelError("", Constants.MSG_GENERAL_TRAVEL_EXPENSE_AMOUNT);
                                return View(opdInformation);
                            }

                        }
                        else
                        {
                            ModelState.AddModelError("", Constants.MSG_GENERAL_ADD_TRAVEL_EXPENSE_RECEIPTS);
                            return View(opdInformation);
                        }
                   
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        OpdExpense.CreatedDate = DateTime.Now;
                        OpdExpense.ModifiedDate = DateTime.Now;
                        OpdExpense.EmployeeEmailAddress = GetEmailAddress();
                        _opdExpenseService.UpdateOpdExpense(OpdExpense);
                        return RedirectToAction(UrlIndex, UrlOpdExpense);
                    }

                }

                return View(opdInformation);
            }
            catch (Exception ex)
            {
                logger.Error("Travel Expense : Edit([Bind])" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

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
                    var result2 = GetTravelExpense(idDecrypted);
                    return View(result2);

                }
                else
                {
                    return RedirectToAction("Details()", UrlOpdExpense);
                }
            }
            catch (Exception ex)
            {

                logger.Error("OPD Expense : Details" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

        private TravelExpenseMasterDetail GetTravelExpense(int Id)
        {
            OpdExpenseVM opdExpense = _opdExpenseService.GetOpdExpensesAgainstId(Id);

            var opdInformation = new TravelExpenseMasterDetail()
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
                ManagerName = opdExpense.ManagerName,
                PhysicalDocumentReceived = opdExpense.PhysicalDocumentReceived,
                PayRollMonth = opdExpense.PayRollMonth,             
                ExpenseNumber = opdExpense.ExpenseNumber,
                OpdEncrypted = opdExpense.OpdEncrypted

            };

            return opdInformation;
        }
        private string GetEmailAddress()
        {
            OfficeManagerController managerController = new OfficeManagerController();
            string emailAddress = managerController.GetEmailAddress();

            return emailAddress;

        }
        private bool AuthenticateEmailAddress(int Id)
        {

            var opdInformation = GetTravelExpense(Convert.ToInt32(Id));
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
            if (ValidEmailAddress(emailAddress))
            {
                ViewBag.RollTypeTravel = "MANTRAVEL";
            }
           
             ViewBag.RollType = managerController.AuthenticateUser();
            

            ViewBag.UserName = managerController.GetName();

        }
        private bool GetTravelExpenseAmount(int Id, decimal? totalAmountClaimed)
        {
            bool result = false;

            var opdInformation = GetTravelExpense(Id);

            decimal? totalAmount = 0;

            for (int count = 0; count <= opdInformation.ListTravelExpense.Count - 1; count++)
            {
                totalAmount += opdInformation.ListTravelExpense[count].Amount;

            }

            if (totalAmount.Equals(totalAmountClaimed))
            {
                result = true;
            }

            return result;


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


        #endregion

    }
}