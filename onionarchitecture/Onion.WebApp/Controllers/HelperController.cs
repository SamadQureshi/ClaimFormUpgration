using Onion.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPDCLAIMFORM.Controllers
{
    public class HelperController : Controller
    {


        private readonly IDepartmentService _departmentService;


        public HelperController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
            

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


    }


}
        
