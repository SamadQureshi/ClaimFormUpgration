using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Onion.Common.Constants;
using Onion.Domain.Models;
using Onion.Interfaces;
using Onion.Interfaces.Services;
using TCO.TFM.WDMS.Common.Utils;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Services
{
    public class OpdExpenseService : IOpdExpenseService
    {
        private IBaseRepository<OpdExpense> _opdExpenseRepository;
      
        public OpdExpenseService(IBaseRepository<OpdExpense> opdExpenseRepository)
        {
            _opdExpenseRepository = opdExpenseRepository;
        }

        public List<OpdExpense> GetAllOpdExpenses()
        {
            return _opdExpenseRepository.GetQueryable().ToList();
        }

        public OpdExpenseVM GetOpdExpensesAgainstId(int Id)
        {
            var opdExpense = _opdExpenseRepository.GetById(Id);        
            return Mapper.Map<OpdExpenseVM>(opdExpense);
           
        }


        public List<OpdExpenseVM> GetOpdExpensesAgainstEmailAddress(string EmailAddress)
        {
            var opdExpense = _opdExpenseRepository.GetQueryable()
                 .Where(y => y.EmployeeEmailAddress == EmailAddress)   
                 .ToList();

            return Mapper.Map<List<OpdExpenseVM>>(opdExpense);
        }

        public OpdExpenseVM CreateOpdExpense(OpdExpenseVM opdExpenseVm)
        {
           
            var OpdExpense  =_opdExpenseRepository.Add(Mapper.Map<OpdExpense>(opdExpenseVm), opdExpenseVm.EmployeeEmailAddress);
            return Mapper.Map<OpdExpenseVM>(OpdExpense);
        }


        public void UpdateOpdExpense(OpdExpenseVM opdExpenseVm)
        {
            OpdExpense obj = Mapper.Map<OpdExpense>(opdExpenseVm);           
            _opdExpenseRepository.Update(obj, opdExpenseVm.EmployeeEmailAddress);         

        }

        public void DeleteOpdExpense(object id)
        {
            var opdExpense = _opdExpenseRepository.GetById(id);
            _opdExpenseRepository.Delete(id, opdExpense.EmployeeEmailAddress);
        }

        public List<OpdExpenseVM> GetOpdExpensesForHR()
        {            
             var opdExpense = _opdExpenseRepository.GetQueryable()
                 .Where(e => e.Status == ClaimStatus.SUBMITTED || e.Status == ClaimStatus.HRAPPROVED || e.Status == ClaimStatus.HRREJECTED || e.Status == ClaimStatus.HRINPROCESS || e.Status == ClaimStatus.FINREJECTED)               
                 .ToList();

            return Mapper.Map<List<OpdExpenseVM>>(opdExpense);
        }

        public List<OpdExpenseVM> GetOpdExpensesForFIN()
        {
            var opdExpense = _opdExpenseRepository.GetQueryable()
                .Where(e => e.Status == ClaimStatus.HRAPPROVED || e.Status == ClaimStatus.FINAPPROVED || e.Status == ClaimStatus.FINREJECTED || e.Status == ClaimStatus.FININPROCESS || e.Status == ClaimStatus.MANGAPPROVED || e.Status == ClaimStatus.MANGREJECT || e.Status == ClaimStatus.COMPLETED || e.Status == ClaimStatus.MANREJECTED)
                .ToList();

            return Mapper.Map<List<OpdExpenseVM>>(opdExpense);
        }

        public List<OpdExpenseVM> GetOpdExpensesForMAN()
        {
            var opdExpense = _opdExpenseRepository.GetQueryable()
                .Where(e => e.Status == ClaimStatus.FINAPPROVED || e.Status == ClaimStatus.MANINPROCESS || e.Status == ClaimStatus.MANAPPROVED || e.Status == ClaimStatus.MANREJECTED)
                .ToList();

            return Mapper.Map<List<OpdExpenseVM>>(opdExpense);
        }

        public List<OpdExpenseVM> GetOpdExpensesForMANTravel(string emailAddress)
        {
            var opdExpense = _opdExpenseRepository.GetQueryable()
                .Where(e => e.ManagerName.Equals(emailAddress) && e.Status == ClaimStatus.MANGINPROCESS)
                .ToList();

            return Mapper.Map<List<OpdExpenseVM>>(opdExpense);
        }
    }
}


