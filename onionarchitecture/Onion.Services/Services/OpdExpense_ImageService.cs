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
                 .ToList();
            return Mapper.Map<List<OpdExpense_ImageVM>>(opdExpense);
        }

       
        public OpdExpense_ImageVM CreateOpdExpense_Image(OpdExpense_ImageVM opdExpense_ImageVM)
        {
            object Obj_OpdExpense_Image = _opdExpense_ImageRepository.Add(Mapper.Map<OpdExpense_Image>(opdExpense_ImageVM));
            return Mapper.Map<OpdExpense_ImageVM>(Obj_OpdExpense_Image);
        }


        public void UpdateOpdExpense_Image(OpdExpense_ImageVM opdExpense_ImageVM)
        {

            _opdExpense_ImageRepository.Update(Mapper.Map<OpdExpense_Image>(opdExpense_ImageVM));

        }

        public void DeleteOpdExpense_Image(object id)
        {

            _opdExpense_ImageRepository.Delete(id);

        }

        public OpdExpense_ImageVM GetOpdExpensesImagesAgainstId(int Id)
        {
            var opdExpenseImage = _opdExpense_ImageRepository.GetById(Id);
            return Mapper.Map<OpdExpense_ImageVM>(opdExpenseImage);            
        }

    }
}


