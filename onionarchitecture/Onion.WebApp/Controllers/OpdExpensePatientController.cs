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

        public OpdExpensePatientController(IOpdExpensePatientService opdExpensePatientService)
        {
           _opdExpensePatientService = opdExpensePatientService;

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
               

             

                var opdExpense_Patient = _opdExpensePatientService.GetOpdExpensesPatientAgainstOpdExpenseId(Convert.ToInt32(id));

                //Add a Dummy Row.
                opdExpense_Patient.Insert(0, new OpdExpensePatientVM());


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