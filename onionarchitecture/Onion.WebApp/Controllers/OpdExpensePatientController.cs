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

        private readonly IOpdExpense_PatientService _opdExpense_PatientService;

        public OpdExpensePatientController(IOpdExpense_PatientService opdExpensePatientService)
        {
           _opdExpense_PatientService = opdExpensePatientService;

        }
        public ActionResult Index(int? id, String opdType)
        {
            //if (Request.IsAuthenticated)
            //{
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
               

             

                var opdExpense_Patient = _opdExpense_PatientService.GetOpdExpenses_PatientAgainstOpdExpenseId(Convert.ToInt32(id));

                //Add a Dummy Row.
                opdExpense_Patient.Insert(0, new OpdExpense_PatientVM());


                return View(opdExpense_Patient);
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");

            //}

        }

        [HttpPost]
        public JsonResult InsertOPDExpensePatient(OpdExpense_PatientVM opdExpense_Patient)
        {

            _opdExpense_PatientService.CreateOpdExpense_Patient(opdExpense_Patient);                  
           
            return Json(opdExpense_Patient);
        }

        [HttpPost]
        public ActionResult UpdateOPDExpensePatient(OpdExpense_PatientVM opdExpense_Patient)
        {

            _opdExpense_PatientService.UpdateOpdExpense_Patient(opdExpense_Patient);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult DeleteOPDExpensePatient(int id)
        {

            _opdExpense_PatientService.DeleteOpdExpense_Patient(id);           
            

            return new EmptyResult();
        }

        private void AuthenticateUser()
        {
            //OFFICEAPIMANAGERController managerController = new OFFICEAPIMANAGERController();

            //ViewBag.RollType = managerController.AuthenticateUser();

            //ViewBag.UserName = managerController.GetName();

        }

    }
}