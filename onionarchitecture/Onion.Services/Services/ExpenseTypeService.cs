using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Onion.Domain.Models;
using Onion.Interfaces;
using Onion.Interfaces.Services;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Services
{
    public class ExpenseTypeService : IExpenseTypeService
    {
        private IBaseRepository<ExpenseType> _expenseTypeRepository;
        
        public ExpenseTypeService(IBaseRepository<ExpenseType> expenseTypeRepository)
        {
            _expenseTypeRepository = expenseTypeRepository;
        }

        public List<ExpenseTypeVM> GetAllExpenseTypes()
        {
            var allDepartment = _expenseTypeRepository.GetQueryable().ToList();


            return Mapper.Map<List<ExpenseTypeVM>>(allDepartment);
        }
        


    }
}


