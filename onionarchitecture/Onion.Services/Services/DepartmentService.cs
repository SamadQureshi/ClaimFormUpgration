using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Onion.Domain.Models;
using Onion.Interfaces;
using Onion.Interfaces.Services;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Services
{
    public class DepartmentService : IDepartmentService
    {
        private IBaseRepository<Department> _departmentRepository;
        
        public DepartmentService(IBaseRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public List<DepartmentVM> GetAllDepartments()
        {
            var allDepartment = _departmentRepository.GetQueryable().ToList();


            return Mapper.Map<List<DepartmentVM>>(allDepartment);
        }
        


    }
}


