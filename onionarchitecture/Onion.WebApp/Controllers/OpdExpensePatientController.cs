using Onion.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
        public ActionResult Index(int? id)
        {
            if (Request.IsAuthenticated)
            {
                AuthenticateUser();
                ViewData["OPDEXPENSE_ID"] = id;          
                                

                var opdExpenseService = _opdExpenseService.GetOpdExpensesAgainstId(Convert.ToInt32(id));

                var opdExpense_Patient = _opdExpensePatientService.GetOpdExpensesPatientAgainstOpdExpenseId(Convert.ToInt32(id));

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
            OpdExpensePatientVM OpdExpensePatientObj = _opdExpensePatientService.CreateOpdExpensePatient(opdExpense_Patient);            
           return Json(OpdExpensePatientObj);
        }

        [HttpPost]
        public ActionResult UpdateOPDExpensePatient(OpdExpensePatientVM opdExpensePatient)
        {

            _opdExpensePatientService.UpdateOpdExpensePatient(opdExpensePatient);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult DeleteOPDExpensePatient(int id)
        {

            _opdExpensePatientService.DeleteOpdExpensePatient(id);           
            

            return new EmptyResult();
        }

        private void AuthenticateUser()
        {
            OfficeManagerController managerController = new OfficeManagerController();

            ViewBag.RollType = managerController.AuthenticateUser();

            ViewBag.UserName = managerController.GetName();

        }

    }
}