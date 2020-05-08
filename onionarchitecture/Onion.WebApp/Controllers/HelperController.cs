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
        
        public HelperController(IDepartmentService departmentService, IExpenseTypeService expenseTypeService)
        {
            _departmentService = departmentService;
            _expenseTypeService = expenseTypeService;     

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



    }


}
        
