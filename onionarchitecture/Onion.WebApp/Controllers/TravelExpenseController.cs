using Onion.Common.Constants;
using Onion.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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


        public TravelExpenseController(ITravelExpenseService travelExpenseService,IOpdExpenseService opdExpenseService)
        {
            _travelExpenseService = travelExpenseService;
            _opdExpenseService = opdExpenseService;

        }
        public ActionResult Index(int? id, String opdType)
        {
            if (Request.IsAuthenticated)
            {
                AuthenticateUser();
                ViewData["OPDEXPENSE_ID"] = id;

                if(opdType ==null)
                {
                    ViewData["OPDTYPE"] = HttpContext.Request.UrlReferrer.Query.Split('=')[1].Replace("%20", "").ToString();
                }

                else
                {
                    ViewData["OPDTYPE"] = opdType;
                }    

                var travelExpense = _travelExpenseService.GetTravelExpensesAgainstOpdExpenseId(Convert.ToInt32(id));

                //Add a Dummy Row.
                travelExpense.Insert(0, new TravelExpenseVM());


                return View(travelExpense);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }

        }

        [HttpPost]
        public JsonResult InsertTravelExpense(TravelExpenseVM _travelExpenseVM)
        {
            _travelExpenseVM.CreatedDate = DateTime.Now;
            TravelExpenseVM TravelExpenseObj = _travelExpenseService.CreateTravelExpense(_travelExpenseVM);

           return Json(TravelExpenseObj);
        }

        [HttpPost]
        public ActionResult UpdateTravelExpense(TravelExpenseVM _travelExpenseVM)
        {

            _travelExpenseService.UpdateTravelExpense(_travelExpenseVM);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult DeleteTravelExpense(int id)
        {

            _travelExpenseService.DeleteTravelExpense(id);           
            

            return new EmptyResult();
        }

       
        public ActionResult Edit(int? id, string opdType)
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

                    var opdInformation = GetTravelExpense(Convert.ToInt32(id));
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
                var opdInformation = GetTravelExpense(OpdExpense.OPDEXPENSE_ID);
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

                   
                        if (opdInformation.ListTravelExpense.Count > 0)
                        {

                            if (GetTravelExpenseAmount(OpdExpense.OPDEXPENSE_ID, OpdExpense.TOTAL_AMOUNT_CLAIMED))
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



        private TravelExpenseMasterDetail GetTravelExpense(int Id)
        {
            OpdExpenseVM opdExpense = _opdExpenseService.GetOpdExpensesAgainstId(Id);

            var opdInformation = new TravelExpenseMasterDetail()
            {

                ListTravelExpense = _travelExpenseService.GetTravelExpensesAgainstOpdExpenseId(Id),
         

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
                MODIFIED_DATE = opdExpense.ModifiedDate,
                ManagerName = opdExpense.ManagerName

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

            if (currentEmailAddress.Equals(opdInformation.EMPLOYEE_EMAILADDRESS))

                return true;
            else
                return false;

        }
        private void AuthenticateUser()
        {
            OfficeManagerController managerController = new OfficeManagerController();

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



    }
}