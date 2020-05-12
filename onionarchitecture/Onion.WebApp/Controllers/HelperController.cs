using Onion.Common.Constants;
using Onion.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.WebApp.Controllers
{
    public class HelperController : Controller
    {

        private readonly IDepartmentService _departmentService;
        private readonly IExpenseTypeService _expenseTypeService;
        private readonly IOpdExpenseService _opdExpenseService;
        private readonly IOpdExpenseImageService _opdExpenseImageService;
        private readonly IOpdExpensePatientService _opdExpensePatientService;
        private readonly IEmailService _emailService;
        private readonly ISetupExpenseAmountService _setupExpenseAmountService;

        public HelperController(IDepartmentService departmentService, IExpenseTypeService expenseTypeService, IOpdExpenseService opdExpenseService, IOpdExpenseImageService opdExpenseImageService, IOpdExpensePatientService opdExpensePatientService,
            IEmailService emailService, ISetupExpenseAmountService setupExpenseAmountService)
        {
            _departmentService = departmentService;
            _expenseTypeService = expenseTypeService;
            _opdExpenseService = opdExpenseService;
            _opdExpenseImageService = opdExpenseImageService;
            _opdExpensePatientService = opdExpensePatientService;
            _emailService = emailService;
            _setupExpenseAmountService = setupExpenseAmountService;

        }

        // GET: Helper
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult GetDepartments()
        {

            IEnumerable<SelectListItem> regions = GetDepartment();
            return Json(regions, JsonRequestBehavior.AllowGet);

        }

        public IEnumerable<SelectListItem> GetDepartment()
        {
            IEnumerable<SelectListItem> regions = _departmentService.GetAllDepartments()
                    .OrderBy(n => n.DepartmentName)
                    .Select(n =>
                       new SelectListItem
                       {
                           Value = n.DepartmentName,
                           Text = n.DepartmentName
                       }).ToList();
            return new SelectList(regions, "Value", "Text");
        }

        [HttpGet]
        public ActionResult GetExpenseTypes()
        {

            IEnumerable<SelectListItem> regions = GetExpenseType();
            return Json(regions, JsonRequestBehavior.AllowGet);

        }
        public IEnumerable<SelectListItem> GetExpenseType()
        {
            IEnumerable<SelectListItem> regions = _expenseTypeService.GetAllExpenseTypes()
                    .OrderBy(n => n.ExpenseName)
                    .Select(n =>
                       new SelectListItem
                       {
                           Value = n.ExpenseName,
                           Text = n.ExpenseName
                       }).ToList();
            return new SelectList(regions, "Value", "Text");
        }

        [HttpGet]
        public ActionResult GetRemainingAmountForHospital(string userName, string hospitalizationType,string maternityType)
        {
                      
            
            string result = GeneralController.CalculateRemainingAmount(userName, FormType.HospitalExpense, hospitalizationType,maternityType ,_opdExpenseService, _setupExpenseAmountService, true);
                  
             return Json(result, JsonRequestBehavior.AllowGet);



}
        }


     


    }
        
