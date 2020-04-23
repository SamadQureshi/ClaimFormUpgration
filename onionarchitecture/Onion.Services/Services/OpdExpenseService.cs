﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Onion.Common.Constants;
using Onion.Domain.Models;
using Onion.Interfaces;
using Onion.Interfaces.Services;
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
                 //.Select(t => new { t.EMPLOYEE_NAME, t.EMPLOYEE_EMAILADDRESS, t.EMPLOYEE_DEPARTMENT, t.CLAIM_MONTH, t.CLAIM_YEAR, t.TOTAL_AMOUNT_CLAIMED, t.Status, t.OPDTYPE, t.OPDEXPENSE_ID , t.EXPENSE_NUMBER })
                 .ToList();

            return Mapper.Map<List<OpdExpenseVM>>(opdExpense);
        }

        public OpdExpenseVM CreateOpdExpense(OpdExpenseVM opdExpenseVm)
        {
           
            var OpdExpense  =_opdExpenseRepository.Add(Mapper.Map<OpdExpense>(opdExpenseVm));
            return Mapper.Map<OpdExpenseVM>(OpdExpense);
        }


        public void UpdateOpdExpense(OpdExpenseVM opdExpenseVm)
        {

            OpdExpense obj = Mapper.Map<OpdExpense>(opdExpenseVm);
            obj.TotalAmountApproved = opdExpenseVm.TotalAmountApproved ?? obj.TotalAmountApproved;
            obj.FinanceComment = opdExpenseVm.FinanceComment ?? obj.FinanceComment;
            obj.ManagementComment = opdExpenseVm.ManagementComment ?? obj.ManagementComment;
            obj.HrComment = opdExpenseVm.HrComment ?? obj.HrComment;
            _opdExpenseRepository.Update(obj);            

        }

        public void DeleteOpdExpense(object id)
        {

            _opdExpenseRepository.Delete(id);

        }

        public List<OpdExpenseVM> GetOpdExpensesForHR()
        {            
             var opdExpense = _opdExpenseRepository.GetQueryable()
                 .Where(e => e.Status == ClaimStatus.SUBMITTED || e.Status == ClaimStatus.HRAPPROVED || e.Status == ClaimStatus.HRREJECTED || e.Status == ClaimStatus.HRINPROCESS)               
                 .ToList();

            return Mapper.Map<List<OpdExpenseVM>>(opdExpense);
        }

        public List<OpdExpenseVM> GetOpdExpensesForFIN()
        {
            var opdExpense = _opdExpenseRepository.GetQueryable()
                .Where(e => e.Status == ClaimStatus.HRAPPROVED || e.Status == ClaimStatus.FINAPPROVED || e.Status == ClaimStatus.FINREJECTED || e.Status == ClaimStatus.FININPROCESS || e.Status == ClaimStatus.MANGAPPROVED || e.Status == ClaimStatus.MANGREJECT || e.Status == ClaimStatus.COMPLETED)
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


