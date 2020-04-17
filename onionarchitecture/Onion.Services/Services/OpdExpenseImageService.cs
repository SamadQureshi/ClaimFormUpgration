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
        private readonly IBaseRepository<OpdExpenseImage> _opdExpenseImageRepository;
        
        public OpdExpenseImageService(IBaseRepository<OpdExpenseImage> opdExpenseImageRepository)
        {
            _opdExpenseImageRepository = opdExpenseImageRepository;
        }

        public List<OpdExpenseImage> GetAllOpdExpenseImages()
        {
            return _opdExpenseImageRepository.GetQueryable().ToList();
        }

        public List<OpdExpenseImageVM> GetOpdExpensesImageAgainstOpdExpenseId(int Id)
        {
            var opdExpense = _opdExpenseImageRepository.GetQueryable()
                .Where(y => y.OpdExpense.ID == Id)        
                 .ToList();
            return Mapper.Map<List<OpdExpenseImageVM>>(opdExpense);
        }

       
        public OpdExpenseImageVM CreateOpdExpenseImage(OpdExpenseImageVM opdExpenseImageVM)
        {
            var ObjOpdExpenseImage = _opdExpenseImageRepository.Add(Mapper.Map<OpdExpenseImage>(opdExpenseImageVM));
            return Mapper.Map<OpdExpenseImageVM>(ObjOpdExpenseImage);
        }


        public void UpdateOpdExpenseImage(OpdExpenseImageVM opdExpenseImageVM)
        {

            _opdExpenseImageRepository.Update(Mapper.Map<OpdExpenseImage>(opdExpenseImageVM));

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


