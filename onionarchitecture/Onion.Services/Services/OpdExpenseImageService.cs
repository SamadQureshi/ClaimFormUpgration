using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Onion.Domain.Models;
using Onion.Interfaces;
using Onion.Interfaces.Services;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Services
{
    public class OpdExpenseImageService : IOpdExpenseImageService
    {
        private readonly IBaseRepository<OpdExpense_Image> _opdExpenseImageRepository;
        
        public OpdExpenseImageService(IBaseRepository<OpdExpense_Image> opdExpenseImageRepository)
        {
            _opdExpenseImageRepository = opdExpenseImageRepository;
        }

        public List<OpdExpense_Image> GetAllOpdExpenseImages()
        {
            return _opdExpenseImageRepository.GetQueryable().ToList();
        }

        public List<OpdExpenseImageVM> GetOpdExpensesImageAgainstOpdExpenseId(int Id)
        {
            var opdExpense = _opdExpenseImageRepository.GetQueryable()
                .Where(y => y.OPDEXPENSE_ID == Id)        
                 .ToList();
            return Mapper.Map<List<OpdExpenseImageVM>>(opdExpense);
        }

       
        public OpdExpenseImageVM CreateOpdExpenseImage(OpdExpenseImageVM opdExpenseImageVM)
        {
            object ObjOpdExpenseImage = _opdExpenseImageRepository.Add(Mapper.Map<OpdExpense_Image>(opdExpenseImageVM));
            return Mapper.Map<OpdExpenseImageVM>(ObjOpdExpenseImage);
        }


        public void UpdateOpdExpenseImage(OpdExpenseImageVM opdExpenseImageVM)
        {

            _opdExpenseImageRepository.Update(Mapper.Map<OpdExpense_Image>(opdExpenseImageVM));

        }

        public void DeleteOpdExpenseImage(object id)
        {

            _opdExpenseImageRepository.Delete(id);

        }

        public OpdExpenseImageVM GetOpdExpensesImagesAgainstId(int Id)
        {
            var opdExpenseImage = _opdExpenseImageRepository.GetById(Id);
            return Mapper.Map<OpdExpenseImageVM>(opdExpenseImage);            
        }

    }
}


