using System;
using System.Web.Mvc;
using System.Net;
using Onion.Interfaces.Services;
using Onion.WebApp.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;
using Onion.Common.Constants;

namespace Onion.WebApp.Controllers
{
    public class OpdExpenseController : Controller
    {

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

                //logger.Error("OPD Expense : UrlIndex()" + ex.Message.ToString());

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
                
                //logger.Error("OPD Expense : Details" + ex.Message.ToString());

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

                //logger.Error("OPD Expense : Create()" + ex.Message.ToString());

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
                  
                   // OpdExpense.OPDTYPE = FormType.OPDExpense;
                    OpdExpense.STATUS = ClaimStatus.INPROGRESS;
                    OpdExpense.CreatedDate = DateTime.Now;
                    OpdExpense.EMPLOYEE_EMAILADDRESS = GetEmailAddress();

                    OpdExpenseVM OpdExpense_Obj = _opdExpenseService.CreateOpdExpense(OpdExpense);                 
                                       
                    ViewData["OPDEXPENSE_ID"] = OpdExpense_Obj.OPDEXPENSE_ID;
                    ViewData["OPDTYPE"] = OpdExpense.OPDTYPE;

                    if(OpdExpense.OPDTYPE == FormType.OPDExpense)
                    return RedirectToAction("Edit", UrlOpdExpense, new { id = OpdExpense_Obj.OPDEXPENSE_ID , opdType = FormType.OPDExpense });
                    else if (OpdExpense.OPDTYPE == FormType.TravelExpense)
                    return RedirectToAction("Edit", UrlTravelExpense, new { id = OpdExpense_Obj.OPDEXPENSE_ID, opdType = FormType.TravelExpense });
                }
                return View(OpdExpense);
            }
            catch (Exception ex)
            {
                //logger.Error("OPD Expense : Create([Bind])" + ex.Message.ToString());

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
                    ViewData["OPDTYPE"] = opdInformation.OPDTYPE;


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
                //logger.Error("OPD Expense : Edit()" + ex.Message.ToString());

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

               // AuthenticateUser();
                var opdInformation = GetOPDExpense(OpdExpense.OPDEXPENSE_ID);
                ViewData["OPDEXPENSE_ID"] = OpdExpense.OPDEXPENSE_ID;
                ViewData["OPDTYPE"] = OpdExpense.OPDTYPE;
                if (buttonStatus == "submit")
                {
                    OpdExpense.STATUS = ClaimStatus.SUBMITTED;
                }
                else
                {
                    OpdExpense.STATUS = ClaimStatus.INPROGRESS;
                }


                if (OpdExpense.STATUS == ClaimStatus.SUBMITTED)
                {

                    if (opdInformation.ListOPDEXPENSEPATIENT.Count > 0)
                    {
                        if (opdInformation.ListOPDEXPENSEIMAGE.Count > 0)
                        {

                            if (GetOPDExpenseAmount(OpdExpense.OPDEXPENSE_ID, OpdExpense.TOTAL_AMOUNT_CLAIMED))
                            {
                                if (ModelState.IsValid)
                                {
                                    OpdExpense.ModifiedDate = DateTime.Now;
                                    OpdExpense.EMPLOYEE_EMAILADDRESS = GetEmailAddress();
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
                        OpdExpense.EMPLOYEE_EMAILADDRESS = GetEmailAddress();
                        _opdExpenseService.UpdateOpdExpense(OpdExpense);
                        return RedirectToAction(UrlIndex);
                    }

                }

                return View(opdInformation);
            }
            catch (Exception ex)
            {
                //logger.Error("OPD Expense : Edit([Bind])" + ex.Message.ToString());

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
                //logger.Error("OPD Expense : Delete()" + ex.Message.ToString());

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
                //logger.Error("OPD Expense : DeleteConfirmed()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
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

        private bool GetOPDExpenseAmount(int Id, decimal? totalAmountClaimed)
        {
            bool result = false;

            var opdInformation = GetOPDExpense(Id);

            decimal? totalAmount = 0;

            for (int count = 0; count <= opdInformation.ListOPDEXPENSEIMAGE.Count - 1; count++)
            {
                totalAmount += opdInformation.ListOPDEXPENSEIMAGE[count].EXPENSE_AMOUNT;

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

            if (currentEmailAddress.Equals(opdInformation.EMPLOYEE_EMAILADDRESS))

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
            return this.GetFile(fileInfo.IMAGE_BASE64, fileInfo.IMAGE_EXT);
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
