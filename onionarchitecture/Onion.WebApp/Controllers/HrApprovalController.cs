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
using Onion.WebApp.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;
using Onion.Common.Enum;
using Onion.Common.Constants;

namespace Onion.WebApp.Controllers
{
    public class HrApprovalController : Controller
    {    
       // private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IOpdExpenseService _opdExpenseService;
        private readonly IOpdExpense_ImageService _opdExpense_ImageService;
        private readonly IOpdExpense_PatientService _opdExpense_PatientService;      


        public HrApprovalController(IOpdExpenseService opdExpenseService, IOpdExpense_ImageService opdExpenseImageService, IOpdExpense_PatientService opdExpensePatientService)
        {
            _opdExpenseService = opdExpenseService;
            _opdExpense_ImageService = opdExpenseImageService;
            _opdExpense_PatientService = opdExpensePatientService;

        }



        // GET: OPDEXPENSEs
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    AuthenticateUser();
                    //ViewBag.CurrentSort = sortOrder;
                    //ViewBag.EmployeeNameSortParm = String.IsNullOrEmpty(sortOrder) ? "EmployeeName_desc" : "";
                    //ViewBag.ClaimForMonthSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaimForMonth_desc" : "";
                    //ViewBag.StatusSortParm = String.IsNullOrEmpty(sortOrder) ? "Status_desc" : "";
                    //ViewBag.OPDTypeSortParm = String.IsNullOrEmpty(sortOrder) ? "OPDType_desc" : "";
                    //ViewBag.ExpenseNumberSortParm = String.IsNullOrEmpty(sortOrder) ? "ExpenseNumber_desc" : "";

                    //if (searchString != null)
                    //{
                    //    page = 1;
                    //}
                    //else
                    //{
                    //    searchString = currentFilter;
                    //}


                    ViewBag.CurrentFilter = searchString;

                    string emailAddress = GetEmailAddress();

                    // && e.HR_EMAILADDRESS == emailAddress

                    var opdExp = _opdExpenseService.GetOpdExpensesForHR();
                    //var opdExp = db.OPDEXPENSEs.Where(e => e.STATUS == "Submitted" || e.STATUS == "HRApproved" || e.STATUS == "HRRejected" || e.STATUS == "HRInProcess");
                    //if (!String.IsNullOrEmpty(searchString))
                    //{
                    //    opdExp = opdExp.Where(s => s.EXPENSE_NUMBER.Contains(searchString));
                    //}
                    //switch (sortOrder)
                    //{
                    //    case "EmployeeName_desc":
                    //        opdExp = opdExp.OrderBy(s => s.EMPLOYEE_NAME);
                    //        ViewBag.EmployeeNameSortParm = "EmployeeName_asc";
                    //        break;
                    //    case "ClaimForMonth_desc":
                    //        opdExp = opdExp.OrderBy(s => s.CLAIM_MONTH);
                    //        ViewBag.ClaimForMonthSortParm = "ClaimForMonth_asc";
                    //        break;
                    //    case "Status_desc":
                    //        opdExp = opdExp.OrderBy(s => s.STATUS);
                    //        ViewBag.StatusSortParm = "Status_asc";
                    //        break;
                    //    case "OPDType_desc":
                    //        opdExp = opdExp.OrderBy(s => s.OPDTYPE);
                    //        ViewBag.OPDTypeSortParm = "OPDType_asc";
                    //        break;
                    //    case "ExpenseNumber_desc":
                    //        opdExp = opdExp.OrderBy(s => s.EXPENSE_NUMBER);
                    //        ViewBag.ExpenseNumberSortParm = "ExpenseNumber_asc";
                    //        break;
                    //    case "EmployeeName_asc":
                    //        opdExp = opdExp.OrderByDescending(s => s.EMPLOYEE_NAME);
                    //        break;
                    //    case "ClaimForMonth_asc":
                    //        opdExp = opdExp.OrderByDescending(s => s.CLAIM_MONTH);
                    //        break;
                    //    case "Status_asc":
                    //        opdExp = opdExp.OrderByDescending(s => s.STATUS);
                    //        break;
                    //    case "OPDType_asc":
                    //        opdExp = opdExp.OrderByDescending(s => s.OPDTYPE);
                    //        break;
                    //    case "ExpenseNumber_asc":
                    //        opdExp = opdExp.OrderByDescending(s => s.EXPENSE_NUMBER);
                    //        break;
                    //    default:  // Name ascending 
                    //        opdExp = opdExp.OrderBy(s => s.EXPENSE_NUMBER);
                    //        break;
                    //}

                    //int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
                    //int pageNumber = (page ?? 1);
                    //return View(opdExp.ToPagedList(pageNumber, pageSize));

                    return View(opdExp);

                    //return View(db.OPDEXPENSEs.Where(e => e.STATUS == "Submitted" || e.STATUS == "HRApproved" || e.STATUS == "HRRejected").ToList());
                }
                else
                {
                    return RedirectToAction("Index", "Home");

                }

            }
            catch (Exception ex)
            {

                //logger.Error("HRAPPROVAL : Index()" + ex.Message.ToString());

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
                        return RedirectToAction("Index", "Home");
                    }

                    if (id == null)
                    {
                        RedirectToAction("Index", "HrApproval");
                    }

                    var result2 = GetOPDExpense(Convert.ToInt32(id));

                    return View(result2);
                }

                else
                {
                    return RedirectToAction("Index", "HrApproval");
                }

            }
            catch (Exception ex)
            {

                //logger.Error("HRAPPROVAL : DetailsForOPDExpense()" + ex.Message.ToString());

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
                        return RedirectToAction("Index", "Home");
                    }

                    if (id == null)
                    {
                        RedirectToAction("Index", "HrApproval");
                    }                 

                    var result2 = GetHospitalExpense(Convert.ToInt32(id));

                    return View(result2);
                }

                else
                {
                    return RedirectToAction("Index", "HRAPPROVAL");
                }

            }
            catch (Exception ex)
            {

                //logger.Error("HRAPPROVAL : DetailsForHospitalExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }


        // GET: OPDEXPENSEs/Edit/5
        public ActionResult HROPDExpense(int? id)
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
                        RedirectToAction("Index", "HrApproval");
                    }                 

                    var opdExpense = GetOPDExpense(Convert.ToInt32(id));

                    ViewData["OPDEXPENSE_ID"] = id;
                    return View(opdExpense);
                }

                else
                {
                    return RedirectToAction("Index", "HrApproval");
                }

            }
            catch (Exception ex)
            {

               // logger.Error("HRAPPROVAL : HROPDExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }

        // POST: OPDEXPENSEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HROPDExpense([Bind(Include = "OPDEXPENSE_ID,EMPLOYEE_NAME,EMPLOYEE_DEPARTMENT,CLAIM_MONTH,CLAIM_YEAR,TOTAL_AMOUNT_CLAIMED,STATUS,OPDTYPE,HR_COMMENT,HR_APPROVAL_DATE,HR_APPROVAL,HR_NAME,FINANCE_COMMENT,FINANCE_APPROVAL,FINANCE_APPROVAL_DATE,FINANCE_NAME,MANAGEMENT_COMMENT,MANAGEMENT_APPROVAL,MANAGEMENT_APPROVAL_DATE,MANAGEMENT_NAME,TOTAL_AMOUNT_APPROVED,CREATED_DATE,EMPLOYEE_EMAILADDRESS")] OpdExpenseVM oPDEXPENSE)
        {
            try
            {
                string buttonStatus = Request.Form["buttonName"];

                if (buttonStatus == "approved")
                {
                    oPDEXPENSE.STATUS = Helper.GeneralStatus.HRApproved.ToString();

                   if(oPDEXPENSE.TOTAL_AMOUNT_APPROVED.ToString() == "")
                    {
                        ModelState.AddModelError("", "Please Add Total Approved Amount");
                    }                
                   

                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.STATUS = Helper.GeneralStatus.HRRejected.ToString();

                    if (oPDEXPENSE.HR_COMMENT == null)
                    {
                        ModelState.AddModelError("", "Please Add HR Comments");
                    }
                }
                else
                {
                    oPDEXPENSE.STATUS = Helper.GeneralStatus.HRInProcess.ToString();
                }

               

                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.HR_APPROVAL_DATE = DateTime.Now;
                    oPDEXPENSE.HR_EMAILADDRESS = GetEmailAddress();
                    if (oPDEXPENSE.STATUS == "HrApproval")
                    {
                        oPDEXPENSE.HR_APPROVAL = true;
                    }

                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    return RedirectToAction("Index", "HRAPPROVAL");

                }

                var opdExpense = GetOPDExpense(Convert.ToInt32(oPDEXPENSE.OPDEXPENSE_ID));
                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.OPDEXPENSE_ID;
                return View(opdExpense);

            }
            catch (Exception ex)
            {

                //logger.Error("HRAPPROVAL : HROPDExpense([Bind])" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
        }





        // GET: OPDEXPENSEs/Edit/5
        public ActionResult HRHospitalExpense(int? id)
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
                        RedirectToAction("Index", "HRAPPROVAL");
                    }

                    var result2 = GetHospitalExpense(Convert.ToInt32(id));

                    ViewData["OPDEXPENSE_ID"] = id;

                    return View(result2);
                }
                else
                {
                    return RedirectToAction("Index", "HRAPPROVAL");
                }

            }
            catch (Exception ex)
            {

                //logger.Error("HRAPPROVAL : HRHospitalExpense()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }

        }

        // POST: OPDEXPENSEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HRHospitalExpense([Bind(Include = "OPDEXPENSE_ID,EMPLOYEE_NAME,EMPLOYEE_DEPARTMENT,CLAIM_MONTH,TOTAL_AMOUNT_CLAIMED,DATE_ILLNESS_NOTICED,DATE_RECOVERY,DIAGNOSIS,CLAIMANT_SUFFERED_ILLNESS,CLAIMANT_SUFFERED_ILLNESS_DATE,CLAIMANT_SUFFERED_ILLNESS_DETAILS,HOSPITAL_NAME,DOCTOR_NAME,PERIOD_CONFINEMENT_DATE_FROM,PERIOD_CONFINEMENT_DATE_TO,DRUGS_PRESCRIBED_BOOL,DRUGS_PRESCRIBED_DESCRIPTION,OPDTYPE,STATUS,HR_COMMENT,HR_APPROVAL,HR_APPROVAL_DATE,HR_NAME,FINANCE_COMMENT,FINANCE_APPROVAL,FINANCE_APPROVAL_DATE,FINANCE_NAME,MANAGEMENT_COMMENT,MANAGEMENT_APPROVAL,MANAGEMENT_APPROVAL_DATE,MANAGEMENT_NAME,CLAIM_YEAR,TOTAL_AMOUNT_APPROVED,CREATED_DATE,EMPLOYEE_EMAILADDRESS")] OpdExpenseVM oPDEXPENSE)
        {
            try
            {
                string buttonStatus = Request.Form["buttonName"];

                if (buttonStatus == "approved")
                {
                    oPDEXPENSE.STATUS = Helper.GeneralStatus.HRApproved.ToString(); 

                    if (oPDEXPENSE.TOTAL_AMOUNT_APPROVED.ToString() == "")
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_TOTALAMOUNTAPPROVED);
                    }


                }
                else if (buttonStatus == "rejected")
                {
                    oPDEXPENSE.STATUS = Helper.GeneralStatus.HRRejected.ToString();

                    if (oPDEXPENSE.HR_COMMENT == null)
                    {
                        ModelState.AddModelError("", Constants.MSG_APPROVAL_HRCOMMENTS);
                    }
                }
                else
                {
                    oPDEXPENSE.STATUS = Helper.GeneralStatus.HRInProcess.ToString();
                }
                if (ModelState.IsValid)
                {
                    oPDEXPENSE.ModifiedDate = DateTime.Now;
                    oPDEXPENSE.HR_APPROVAL_DATE = DateTime.Now;
                    oPDEXPENSE.HR_EMAILADDRESS = GetEmailAddress();
                    if (oPDEXPENSE.STATUS == Helper.GeneralStatus.HRApproved.ToString())
                    {
                        oPDEXPENSE.HR_APPROVAL = true;
                    }

                    _opdExpenseService.UpdateOpdExpense(oPDEXPENSE);
                    return RedirectToAction("Index", "HRAPPROVAL");
                }
              
                var opdExpense = GetHospitalExpense(Convert.ToInt32(oPDEXPENSE.OPDEXPENSE_ID));
                ViewData["OPDEXPENSE_ID"] = oPDEXPENSE.OPDEXPENSE_ID;
                return View(opdExpense);

            }
            catch (Exception ex)
            {
                //logger.Error("HRAPPROVAL : HRHospitalExpense([Bind])" + ex.Message.ToString());

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
            ViewBag.RollType = managerController.AuthenticateUser();

            ViewBag.UserName = managerController.GetName();

        }

        private OpdExpense_MasterDetail GetOPDExpense(int Id)
        {
            var opdInformation = new OpdExpense_MasterDetail()
            {
                opdEXPENSE = _opdExpenseService.GetOpdExpensesAgainstId(Id),
                listOPDEXPENSEPATIENT = _opdExpense_PatientService.GetOpdExpenses_PatientAgainstOpdExpenseId(Id),
                listOPDEXPENSEIMAGE = _opdExpense_ImageService.GetOpdExpenses_ImageAgainstOpdExpenseId(Id),

            };

            return opdInformation;
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



        #endregion

        private bool AuthenticateEmailAddress(int Id)
        {

            var opdInformation = GetOPDExpense(Convert.ToInt32(Id));
            OfficeManagerController managerController = new OfficeManagerController();

            string currentEmailAddress = managerController.GetEmailAddress();

            if (currentEmailAddress.Equals(opdInformation.opdEXPENSE.EMPLOYEE_EMAILADDRESS))

                return true;
            else
                return false;

        }
    }
}