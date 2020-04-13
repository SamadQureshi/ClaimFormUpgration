using System;
using System.Web.Mvc;
using System.Net;
using Onion.Interfaces.Services;
using Onion.WebApp.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;
using Onion.Common.Enum;
using Onion.Common.Constants;

namespace Onion.WebApp.Controllers
{
    public class OpdExpenseController : Controller
    {

        private readonly IOpdExpenseService _opdExpenseService;
        private readonly IOpdExpense_ImageService _opdExpense_ImageService;
        private readonly IOpdExpense_PatientService _opdExpense_PatientService;

        public OpdExpenseController(IOpdExpenseService opdExpenseService, IOpdExpense_ImageService opdExpenseImageService, IOpdExpense_PatientService opdExpensePatientService)
        {
            _opdExpenseService = opdExpenseService;
            _opdExpense_ImageService = opdExpenseImageService;
            _opdExpense_PatientService = opdExpensePatientService;

        }


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

                    //ViewBag.CurrentFilter = searchString;

                    string emailAddress = GetEmailAddress();
                    
                    var opdExp = _opdExpenseService.GetOpdExpensesAgainstEmailAddress(emailAddress);

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

                    // return View(db.OPDEXPENSEs.ToList());
                }
                else
                {
                    return RedirectToAction("Index", "Home");

                }
            }
            catch (Exception ex)
            {

                //logger.Error("OPD Expense : Index()" + ex.Message.ToString());

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
                    return RedirectToAction("Index", "OpdExpense");
                }


                var result2 = GetOPDExpense(Convert.ToInt32(id));
                return View(result2);

                }
                else
                {
                    return RedirectToAction("Details()", "OpdExpense");
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //logger.Error("OPD Expense : Details" + ex.Message.ToString());

                //return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
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
                    return RedirectToAction("Index", "OpdExpense");
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
        public ActionResult Create([Bind(Include = "EMPLOYEE_NAME,EMPLOYEE_DEPARTMENT,CLAIM_MONTH,TOTAL_AMOUNT_CLAIMED,CLAIM_YEAR")] OpdExpenseVM OpdExpense)
        {
            try
            {
                if (ModelState.IsValid)
                {
                  
                    OpdExpense.OPDTYPE = "OPD Expense";
                    OpdExpense.STATUS = "InProcess";
                    OpdExpense.CreatedDate = DateTime.Now;
                    OpdExpense.EMPLOYEE_EMAILADDRESS = GetEmailAddress();

                    OpdExpenseVM OpdExpense_Obj = _opdExpenseService.CreateOpdExpense(OpdExpense);                 
                                       
                    ViewData["OPDEXPENSE_ID"] = OpdExpense_Obj.OPDEXPENSE_ID;
                    ViewData["OPDTYPE"] = OpdExpense.OPDTYPE;

                    return RedirectToAction("Edit", "OPDExpense", new { id = OpdExpense_Obj.OPDEXPENSE_ID });

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

                    var opdInformation = GetOPDExpense(Convert.ToInt32(id));
                    ViewData["OPDEXPENSE_ID"] = id;
                    ViewData["OPDTYPE"] = opdInformation.opdEXPENSE.OPDTYPE;


                    if (!(AuthenticateEmailAddress(Convert.ToInt32(id))))
                    {
                        return RedirectToAction("Index", "Home");
                    }


                    return View(opdInformation);



                }
                else
                {
                    return RedirectToAction("Index", "OpdExpense");
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
        public ActionResult Edit([Bind(Include = "OPDEXPENSE_ID,EMPLOYEE_NAME,EMPLOYEE_DEPARTMENT,CLAIM_MONTH,CLAIM_YEAR,TOTAL_AMOUNT_CLAIMED,STATUS,OPDTYPE,CREATED_DATE")] OpdExpenseVM OpdExpense)
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
                    OpdExpense.STATUS = Helper.GeneralStatus.Submitted.ToString();
                }
                else
                {
                    OpdExpense.STATUS = Helper.GeneralStatus.InProcess.ToString();
                }


                if (OpdExpense.STATUS == Helper.GeneralStatus.Submitted.ToString())
                {



                    if (opdInformation.listOPDEXPENSEPATIENT.Count > 0)
                    {
                        if (opdInformation.listOPDEXPENSEIMAGE.Count > 0)
                        {

                            if (GetOPDExpenseAmount(OpdExpense.OPDEXPENSE_ID, OpdExpense.TOTAL_AMOUNT_CLAIMED))
                            {
                                if (ModelState.IsValid)
                                {
                                    OpdExpense.ModifiedDate = DateTime.Now;
                                    OpdExpense.EMPLOYEE_EMAILADDRESS = GetEmailAddress();
                                    _opdExpenseService.UpdateOpdExpense(OpdExpense);
                                    return RedirectToAction("Index");
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
                        return RedirectToAction("Index");
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
                //logger.Error("OPD Expense : DeleteConfirmed()" + ex.Message.ToString());

                return View(new HttpStatusCodeResult(HttpStatusCode.BadRequest));
            }
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

        private bool GetOPDExpenseAmount(int Id, decimal? totalAmountClaimed)
        {
            bool result = false;

            var opdInformation = GetOPDExpense(Id);

            decimal? totalAmount = 0;

            for (int count = 0; count <= opdInformation.listOPDEXPENSEIMAGE.Count - 1; count++)
            {
                totalAmount += opdInformation.listOPDEXPENSEIMAGE[count].EXPENSE_AMOUNT;

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

            if (currentEmailAddress.Equals(opdInformation.opdEXPENSE.EMPLOYEE_EMAILADDRESS))

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

        #endregion

    }
}
