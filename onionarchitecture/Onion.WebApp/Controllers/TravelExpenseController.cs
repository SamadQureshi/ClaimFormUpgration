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
                var opdInformation = GetTravelExpense(OpdExpense.ID);
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

                   
                        if (opdInformation.ListTravelExpense.Count > 0)
                        {

                            if (GetTravelExpenseAmount(OpdExpense.ID, OpdExpense.TotalAmountClaimed))
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
                ModifiedDate = opdExpense.ModifiedDate,
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

            if (currentEmailAddress.Equals(opdInformation.EmployeeEmailAddress))

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