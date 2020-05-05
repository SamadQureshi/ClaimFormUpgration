using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Onion.Domain.Models;
using Onion.Interfaces;
using Onion.Interfaces.Services;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Services
{
    public class TravelExpenseService : ITravelExpenseService
    {
        private readonly IBaseRepository<TravelExpense> _travelExpenseRepository;
        
        public TravelExpenseService(IBaseRepository<TravelExpense> travelExpenseRepository)
        {
            _travelExpenseRepository = travelExpenseRepository;
        }

        public List<TravelExpense> GetAllTravelExpenses()
        {
            return _travelExpenseRepository.GetQueryable().ToList();
        }


        public List<TravelExpenseVM> GetTravelExpensesAgainstOpdExpenseId(int id)
        {
            var travelExpense = _travelExpenseRepository.GetQueryable()
                 .Where(y => y.OpdExpense.ID == id)               
                 .ToList();

            return Mapper.Map<List<TravelExpenseVM>>(travelExpense);
        }

        public TravelExpenseVM CreateTravelExpense(TravelExpenseVM travelExpenseVM, string emailAddress)
        {
            var ObjOpdExpensePatient = _travelExpenseRepository.Add(Mapper.Map<TravelExpense>(travelExpenseVM), emailAddress);
            return Mapper.Map<TravelExpenseVM>(ObjOpdExpensePatient);
        }


        public void UpdateTravelExpense(TravelExpenseVM travelExpenseVM, string emailAddress)
        {
            _travelExpenseRepository.Update(Mapper.Map<TravelExpense>(travelExpenseVM), emailAddress);
        }

        public void UpdateTravelExpense(TravelExpenseMasterDetail travelExpenseMasterDetail, string emailAddress)
        {
            _travelExpenseRepository.Update(Mapper.Map<TravelExpense>(travelExpenseMasterDetail), emailAddress);
        }
        public void DeleteTravelExpense(object id, string emailAddress)
        {
          
            _travelExpenseRepository.Delete(id, emailAddress);
        }

        public TravelExpenseVM GetTravelExpenseAgainstId(int Id)
        {
            var opdExpenseImage = _travelExpenseRepository.GetById(Id);
            return Mapper.Map<TravelExpenseVM>(opdExpenseImage);
        }


    }
}


