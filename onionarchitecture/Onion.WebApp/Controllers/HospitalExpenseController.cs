using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


using Onion.Interfaces.Services;
using Onion.WebApp.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;
using Onion.Common.Enum;
using Onion.Common.Constants;

namespace Onion.WebApp.Controllers
{
    public class HospitalExpenseController : Controller
    {       
        //private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        private readonly IOpdExpenseService _opdExpenseService;
        private readonly IOpdExpense_ImageService _opdExpense_ImageService;
        private readonly IOpdExpense_PatientService _opdExpense_PatientService;

        public HospitalExpenseController(IOpdExpenseService opdExpenseService, IOpdExpense_ImageService opdExpenseImageService, IOpdExpense_PatientService opdExpensePatientService)
        {
            _opdExpenseService = opdExpenseService;
            _opdExpense_ImageService = opdExpenseImageService;
            _opdExpense_PatientService = opdExpensePatientService;

        }

        // GET: OPDEXPENSEs
        public ActionResult Index()
        {

            //return View(db.OPDEXPENSEs.Where(e => e.OPDTYPE == "Hospital Expense").ToList());
            return RedirectToAction("Index", "OpdExpense");


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
                        return RedirectToAction("Index", "OpdExpense");
                    }

                    //OPDEXPENSE oPDEXPENSE = db.OPDEXPENSEs.Find(id);

                    var hospitalInformation = GetHospitalExpense(Convert.ToInt32(id));                  
                  
                    return View(hospitalInformation);
                }
                else
                {
                    return RedirectToAction("Index", "OpdExpense");
                }

            }
            catch (Exception ex)
            {

                //logger.Error("Hospital Expense : Details()" + ex.Message.ToString());

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
                    return RedirectToAction("Index", "OpdExpense");
                }
            }
            catch (Exception ex)
            {

                //logger.Error("Hospital Expense : Create()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

        }

        // POST: OPDEXPENSEs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OPDEXPENSE_ID,EMPLOYEE_NAME,EMPLOYEE_DEPARTMENT,DATE_ILLNESS_NOTICED,DATE_RECOVERY,DIAGNOSIS,CLAIMANT_SUFFERED_ILLNESS,CLAIMANT_SUFFERED_ILLNESS_DATE,CLAIMANT_SUFFERED_ILLNESS_DETAILS,HOSPITAL_NAME,DOCTOR_NAME,PERIOD_CONFINEMENT_DATE_FROM,PERIOD_CONFINEMENT_DATE_TO,DRUGS_PRESCRIBED_BOOL,DRUGS_PRESCRIBED_DESCRIPTION,TOTAL_AMOUNT_CLAIMED,CLAIM_YEAR")] OpdExpenseVM opdExpense)
        {
            try
            {

                string buttonStatus = Request.Form["buttonName"];


                if (ModelState.IsValid)
                {
                    opdExpense.OPDTYPE = "Hospital Expense";
                    opdExpense.STATUS = "InProcess";
                    opdExpense.CreatedDate = DateTime.Now;
                    opdExpense.EMPLOYEE_EMAILADDRESS = GetEmailAddress();
                    OpdExpenseVM OpdExpense_Obj = _opdExpenseService.CreateOpdExpense(opdExpense);

                    return RedirectToAction("Edit", "HospitalExpense", new { id = OpdExpense_Obj.OPDEXPENSE_ID , opdType = opdExpense.OPDTYPE });
                }

                return RedirectToAction("Index", "OpdExpense"); 
            }
            catch (Exception ex)
            {

                //logger.Error("Hospital Expense : Create()" + ex.Message.ToString());

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
                        return RedirectToAction("Index", "OpdExpense");
                    }                    

                    //OPDEXPENSE oPDEXPENSE = db.OPDEXPENSEs.Find(id);
                    var hospitalInformation = GetHospitalExpense(Convert.ToInt32(id));                  

                    ViewData["OPDEXPENSE_ID"] = id;
                    ViewData["OPDTYPE"] = hospitalInformation.OPDTYPE;


                    if (!AuthenticateEmailAddress(Convert.ToInt32(id)))
                    {
                        return RedirectToAction("Index", "Home");
                    }


                   
                        return View(hospitalInformation);
                       
                }
                else
                {
                    return RedirectToAction("Index", "OpdExpense");
                }
            }
            catch (Exception ex)
            {

                //logger.Error("Hospital Expense : Create()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

        }

        // POST: OPDEXPENSEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OPDEXPENSE_ID,EMPLOYEE_NAME,EMPLOYEE_DEPARTMENT,CLAIM_MONTH,TOTAL_AMOUNT_CLAIMED,HR_COMMENT,HR_APPROVAL,HR_APPROVAL_DATE,HR_NAME,FINANCE_COMMENT,FINANCE_APPROVAL,FINANCE_APPROVAL_DATE,FINANCE_NAME,MANAGEMENT_COMMENT,MANAGEMENT_APPROVAL,MANAGEMENT_APPROVAL_DATE,MANAGEMENT_NAME,DATE_ILLNESS_NOTICED,DATE_RECOVERY,DIAGNOSIS,CLAIMANT_SUFFERED_ILLNESS,CLAIMANT_SUFFERED_ILLNESS_DATE,CLAIMANT_SUFFERED_ILLNESS_DETAILS,HOSPITAL_NAME,DOCTOR_NAME,PERIOD_CONFINEMENT_DATE_FROM,PERIOD_CONFINEMENT_DATE_TO,DRUGS_PRESCRIBED_BOOL,DRUGS_PRESCRIBED_DESCRIPTION,OPDTYPE,STATUS,CLAIM_YEAR,CREATED_DATE,CLAIMANT_SUFFERED_ILLNESS_DATE,CLAIMANT_SUFFERED_ILLNESS_DETAILS,DRUGS_PRESCRIBED_DESCRIPTION")] OpdExpenseVM opdExpense)
        {

            try
            {
                AuthenticateUser();
                var hospitalInformation = GetHospitalExpense(opdExpense.OPDEXPENSE_ID);
                ViewData["OPDEXPENSE_ID"] = opdExpense.OPDEXPENSE_ID;
                ViewData["OPDTYPE"] = opdExpense.OPDTYPE;

                string buttonStatus = Request.Form["buttonName"];

                if (buttonStatus == "submit")
                {
                    opdExpense.STATUS = Helper.GeneralStatus.Submitted.ToString(); ;
                }
                else
                {
                    opdExpense.STATUS = Helper.GeneralStatus.InProcess.ToString();
                }


                if (opdExpense.STATUS == Helper.GeneralStatus.Submitted.ToString())
                {
                    if (hospitalInformation.ListOPDEXPENSEPATIENT.Count > 0)
                    {
                        if (hospitalInformation.ListOPDEXPENSEIMAGE.Count > 0)
                        {
                            if (GetHOSExpenseAmount(opdExpense, opdExpense.TOTAL_AMOUNT_CLAIMED))
                            {
                                if (ModelState.IsValid)
                                {
                                    opdExpense.ModifiedDate = DateTime.Now;
                                    opdExpense.EMPLOYEE_EMAILADDRESS = GetEmailAddress();
                                    _opdExpenseService.UpdateOpdExpense(opdExpense);
                                    return RedirectToAction("Index", "OpdExpense");
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
                        opdExpense.EMPLOYEE_EMAILADDRESS = GetEmailAddress();
                        _opdExpenseService.UpdateOpdExpense(opdExpense);                        
                        return RedirectToAction("Index", "OpdExpense");
                    }
                }

                           
                return View(hospitalInformation); 
            }
            catch (Exception ex)
            {

                //logger.Error("Hospital Expense : Create()" + ex.Message.ToString());

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
                        return RedirectToAction("Index", "Home");
                    }

                    if (id == null)
                    {
                        return RedirectToAction("Index", "OpdExpense");
                    }

                    _opdExpenseService.DeleteOpdExpense(id); 

                    return RedirectToAction("Index", "OpdExpense");
                }
                else
                {
                    return RedirectToAction("Index", "OpdExpense");
                }

            }
            catch (Exception ex)
            {

                //logger.Error("Hospital Expense : Create()" + ex.Message.ToString());

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
                        return RedirectToAction("Index", "OpdExpense");
                    }

                    else
                    {
                        _opdExpenseService.DeleteOpdExpense(id);
                    }

                    return RedirectToAction("Index", "OpdExpense");
                }
                else
                {
                    return RedirectToAction("Index", "OpdExpense");
                }


            }
            catch (Exception ex)
            {

                //logger.Error("Hospital Expense : Create()" + ex.Message.ToString());

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
                var fileInfo = _opdExpense_ImageService.GetOpdExpensesImagesAgainstId(fileId);

                // Info.
                return this.GetFile(fileInfo.IMAGE_BASE64, fileInfo.IMAGE_EXT);
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

            ViewBag.RollType = managerController.AuthenticateUser();

            ViewBag.UserName = managerController.GetName();

        }

        private HospitalExpense_MasterDetail GetHospitalExpense(int Id)
        {

            OpdExpenseVM opdExpense = _opdExpenseService.GetOpdExpensesAgainstId(Id);


            var hospitalInformation = new HospitalExpense_MasterDetail()
            {

                
                ListOPDEXPENSEPATIENT = _opdExpense_PatientService.GetOpdExpenses_PatientAgainstOpdExpenseId(Id),
                ListOPDEXPENSEIMAGE = _opdExpense_ImageService.GetOpdExpenses_ImageAgainstOpdExpenseId(Id),

               
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

            return hospitalInformation;
        }

        private bool GetHOSExpenseAmount(OpdExpenseVM opdExpense, decimal? totalAmountClaimed)
        {
            bool result = false;

            var hospitalInformation = GetHospitalExpense(opdExpense.OPDEXPENSE_ID);

            decimal? totalAmount = 0;

            for (int count = 0; count <= hospitalInformation.ListOPDEXPENSEIMAGE.Count - 1; count++)
            {
                totalAmount = totalAmount + hospitalInformation.ListOPDEXPENSEIMAGE[count].EXPENSE_AMOUNT;

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

            if (currentEmailAddress.Equals(opdInformation.EMPLOYEE_EMAILADDRESS))

                return true;
            else
                return false;
        }

      

    }
}
