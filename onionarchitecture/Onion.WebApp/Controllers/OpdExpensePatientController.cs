using Onion.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TCO.TFM.WDMS.Common.Utils;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.WebApp.Controllers
{
    public class OpdExpensePatientController : Controller
    {

        private readonly IOpdExpensePatientService _opdExpensePatientService;
        private readonly IOpdExpenseService _opdExpenseService;

        public OpdExpensePatientController(IOpdExpensePatientService opdExpensePatientService, IOpdExpenseService opdExpenseService)
        {
           _opdExpensePatientService = opdExpensePatientService;
            
           _opdExpenseService = opdExpenseService;

        }
        public ActionResult Index(string id)
        {
            if (Request.IsAuthenticated)
            {
                AuthenticateUser();

                int idDecrypted = Security.DecryptId(Convert.ToString(id));

                ViewData["OPDEXPENSE_ID"] = idDecrypted;          
                                

                var opdExpenseService = _opdExpenseService.GetOpdExpensesAgainstId(Convert.ToInt32(idDecrypted));

                var opdExpense_Patient = _opdExpensePatientService.GetOpdExpensesPatientAgainstOpdExpenseId(Convert.ToInt32(idDecrypted));

                var objOpdExpensePatient = new OpdExpensePatientVM();

                ViewData["OPDTYPE"] = opdExpenseService.OpdType;
                //Add a Dummy Row.
                opdExpense_Patient.Insert(0, objOpdExpensePatient);              

                return View(opdExpense_Patient);
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }

        }

        [HttpPost]
        public JsonResult InsertOPDExpensePatient(OpdExpensePatientVM opdExpense_Patient)
        {
            opdExpense_Patient.CreatedDate = DateTime.Now;

            string emailAddress = GetEmailAddress();

            OpdExpensePatientVM OpdExpensePatientObj = _opdExpensePatientService.CreateOpdExpensePatient(opdExpense_Patient, emailAddress);            
           return Json(OpdExpensePatientObj);
        }

        [HttpPost]
        public ActionResult UpdateOPDExpensePatient(OpdExpensePatientVM opdExpensePatient)
        {
            string emailAddress = GetEmailAddress();
            _opdExpensePatientService.UpdateOpdExpensePatient(opdExpensePatient, emailAddress);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult DeleteOPDExpensePatient(int id)
        {
            string emailAddress = GetEmailAddress();

            _opdExpensePatientService.DeleteOpdExpensePatient(id, emailAddress);           
            

            return new EmptyResult();
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
        private string GetEmailAddress()
        {
            OfficeManagerController managerController = new OfficeManagerController();
            string emailAddress = managerController.GetEmailAddress();

            return emailAddress;

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

    }
}