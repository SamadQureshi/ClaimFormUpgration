﻿using System;
using System.Web.Mvc;
using System.Net;
using Onion.Interfaces.Services;
using TCO.TFM.WDMS.ViewModels.ViewModels;
using Onion.Common.Constants;
using NLog;

namespace Onion.WebApp.Controllers
{
    public class OpdExpenseController : Controller
    {
        
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IOpdExpenseService _opdExpenseService;
        private readonly IOpdExpenseImageService _opdExpenseImageService;
        private readonly IOpdExpensePatientService _opdExpensePatientService;

        private const string UrlIndex = "Index";
        private const string UrlHome = "Home";
        private const string UrlOpdExpense = "OpdExpense";
        private const string UrlTravelExpense = "TravelExpense";

        public OpdExpenseController(IOpdExpenseService opdExpenseService, IOpdExpenseImageService opdExpenseImageService, IOpdExpensePatientService opdExpensePatientService)
        {
            _opdExpenseService = opdExpenseService;
            _opdExpenseImageService = opdExpenseImageService;
            _opdExpensePatientService = opdExpensePatientService;

        }


        public ActionResult Index()
        {
            try
            {
                if (Request.IsAuthenticated)
                {

                    AuthenticateUser();                  

                    string emailAddress = GetEmailAddress();
                    
                    var opdExp = _opdExpenseService.GetOpdExpensesAgainstEmailAddress(emailAddress);

                    return View(opdExp);
                }
                else
                {
                    return RedirectToAction(UrlIndex, UrlHome);

                }
            }
            catch (Exception ex)
            {

                logger.Error("OPD Expense : UrlIndex()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }


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


                var result2 = GetOPDExpense(Convert.ToInt32(id));
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



        public ActionResult Create(string opdType)
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

                    if(OpdExpense.OpdType == FormType.OPDExpense)
                    return RedirectToAction("Edit", UrlOpdExpense, new { id = OpdExpense_Obj.ID , opdType = FormType.OPDExpense });
                    else if (OpdExpense.OpdType == FormType.TravelExpense)
                    return RedirectToAction("Edit", UrlTravelExpense, new { id = OpdExpense_Obj.ID, opdType = FormType.TravelExpense });
                }
                return View(OpdExpense);
            }
            catch (Exception ex)
            {
                logger.Error("OPD Expense : Create([Bind])" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }


        // GET: OPDEXPENSEs/Edit/5
        public ActionResult Edit(int? id , string opdType)
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

                    var opdInformation = GetOPDExpense(Convert.ToInt32(id));
                    ViewData["OPDEXPENSE_ID"] = id;
                    ViewData["OPDTYPE"] = opdInformation.OpdType;


                    if (!(AuthenticateEmailAddress(Convert.ToInt32(id))))
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
                logger.Error("OPD Expense : Edit()" + ex.Message.ToString());

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
                var opdInformation = GetOPDExpense(OpdExpense.ID);
                ViewData["OPDEXPENSE_ID"] = OpdExpense.ID;
                ViewData["OPDTYPE"] = OpdExpense.OpdType;
                if (buttonStatus == "submit")
                {
                    OpdExpense.Status = ClaimStatus.SUBMITTED;
                }
                else
                {
                    OpdExpense.Status = ClaimStatus.INPROGRESS;
                }


                if (OpdExpense.Status == ClaimStatus.SUBMITTED)
                {

                    if (opdInformation.OpdExpensePatients.Count > 0)
                    {
                        if (opdInformation.OpdExpenseImages.Count > 0)
                        {

                            if (GetOPDExpenseAmount(OpdExpense.ID, OpdExpense.TotalAmountClaimed))
                            {
                                if (ModelState.IsValid)
                                {
                                    OpdExpense.ModifiedDate = DateTime.Now;
                                    OpdExpense.EmployeeEmailAddress = GetEmailAddress();
                                    _opdExpenseService.UpdateOpdExpense(OpdExpense);
                                    return RedirectToAction(UrlIndex);
                                }

                            }
                            else
                            {
                                ModelState.AddModelError("", Constants.MSG_GENERAL_OPD_EXPENSE_AMOUNT);
                                return View(opdInformation);
                            }

                        }
                        else
                        {
                            ModelState.AddModelError("", Constants.MSG_GENERAL_ADD_PATIENT_RECEIPTS);
                            return View(opdInformation);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", Constants.MSG_GENERAL_ADD_PATIENT_INFORMATION);
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
                        return RedirectToAction(UrlIndex);
                    }

                }

                return View(opdInformation);
            }
            catch (Exception ex)
            {
                logger.Error("OPD Expense : Edit([Bind])" + ex.Message.ToString());

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
                logger.Error("OPD Expense : Delete()" + ex.Message.ToString());

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
                logger.Error("OPD Expense : DeleteConfirmed()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }






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
                FinanceApproval = opdExpense.FinanceApproval,
                FinanceComment = opdExpense.FinanceComment,
                FinanceName = opdExpense.FinanceName,
                HospitalName = opdExpense.HospitalName,
                HrApproval = opdExpense.HrApproval,
                HrComment = opdExpense.HrComment,
                HrName = opdExpense.HrName,
                ManagementApproval = opdExpense.ManagementApproval,
                ManagementComment = opdExpense.ManagementComment,
                ManagementName = opdExpense.ManagementName,
                PeriodConfinementDateFrom = opdExpense.PeriodConfinementDateFrom,
                PeriodConfinementDateTo = opdExpense.PeriodConfinementDateTo,
                Status = opdExpense.Status,
                OpdType = opdExpense.OpdType,
                TotalAmountClaimed = opdExpense.TotalAmountClaimed,
                ClaimYear = opdExpense.ClaimYear,
                CreatedDate = opdExpense.CreatedDate,
                ModifiedDate = opdExpense.ModifiedDate

            };

            return opdInformation;
        }

        private bool GetOPDExpenseAmount(int Id, decimal? totalAmountClaimed)
        {
            bool result = false;

            var opdInformation = GetOPDExpense(Id);

            decimal? totalAmount = 0;

            foreach (var item in opdInformation.OpdExpenseImages)
            {
                totalAmount += item.ExpenseAmount;
            }               

            if (totalAmount.Equals(totalAmountClaimed))
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

        private void AuthenticateUser()
        {
            OfficeManagerController managerController = new OfficeManagerController();

            ViewBag.RollType = managerController.AuthenticateUser();

            ViewBag.UserName = managerController.GetName();

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

        public ActionResult DownloadFile(int fileId)
        {

            // try
            // {
            // Loading dile info.
            var fileInfo = _opdExpenseImageService.GetOpdExpensesImagesAgainstId(fileId);

            // Info.
            return this.GetFile(fileInfo.ImageBase64, fileInfo.ImageExt);
            //}
            //catch (Exception ex)
            //{
            // Info
            //Console.Write(ex);
            //}

            // Info.
            //return this.View(model);

            //return View();
        }


        #endregion
       
    }
}
