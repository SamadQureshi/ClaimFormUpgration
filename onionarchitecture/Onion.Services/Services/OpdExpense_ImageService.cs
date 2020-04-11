using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Onion.Domain.Models;
using Onion.Interfaces;
using Onion.Interfaces.Services;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Services
{
    public class OpdExpense_ImageService : IOpdExpense_ImageService
    {
        private IBaseRepository<OpdExpense_Image> _opdExpense_ImageRepository;
        
        public OpdExpense_ImageService(IBaseRepository<OpdExpense_Image> opdExpense_ImageRepository)
        {
            _opdExpense_ImageRepository = opdExpense_ImageRepository;
        }

        public List<OpdExpense_Image> GetAllOpdExpenseImages()
        {
            return _opdExpense_ImageRepository.GetQueryable().ToList();
        }

        public List<OpdExpense_ImageVM> GetOpdExpenses_ImageAgainstOpdExpenseId(int Id)
        {
            var opdExpense = _opdExpense_ImageRepository.GetQueryable()
                 .Where(y => y.OPDEXPENSE_ID == Id)
                 //.Select(t => new { t.IMAGE_ID,t.OPDEXPENSE_ID, t.IMAGE_NAME, t.NAME_EXPENSES, t.EXPENSE_AMOUNT, t.IMAGE_EXT,  t.IMAGE_BASE64   })
                 .ToList();
            return Mapper.Map<List<OpdExpense_ImageVM>>(opdExpense);
        }
    }
}


