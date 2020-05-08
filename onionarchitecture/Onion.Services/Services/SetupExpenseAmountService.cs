using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Onion.Domain.Models;
using Onion.Interfaces;
using Onion.Interfaces.Services;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Services
{
    public class SetupExpenseAmountService : ISetupExpenseAmountService
    {
        private IBaseRepository<SetupExpenseAmount> _setupExpenseAmountRepository;
        
        public SetupExpenseAmountService(IBaseRepository<SetupExpenseAmount> setupExpenseAmountRepository)
        {
            _setupExpenseAmountRepository = setupExpenseAmountRepository;
        }

        public string GetDefaultExpenseAmountAgainstExpenseType(string opdType)
        {
            var defaultExpenseAmount = _setupExpenseAmountRepository.GetQueryable()
                                .Where(y => y.ExpenseKey == opdType)
                                .Select(y => y.ExpenseValue).FirstOrDefault();


            return defaultExpenseAmount;
        }
        


    }
}


