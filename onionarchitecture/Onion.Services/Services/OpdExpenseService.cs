using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Onion.Common.Enum;
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
            //.Where(y => y.OPDEXPENSE_ID == Id)
            //.Select(t => new { t.EMPLOYEE_NAME, t.EMPLOYEE_EMAILADDRESS, t.EMPLOYEE_DEPARTMENT, t.CLAIM_MONTH, t.CLAIM_YEAR, t.TOTAL_AMOUNT_CLAIMED, t.STATUS, t.OPDTYPE, t.OPDEXPENSE_ID, t.EXPENSE_NUMBER })
            //.ToList();
            return Mapper.Map<OpdExpenseVM>(opdExpense);
            //return opdExpense;
        }

        public List<OpdExpenseVM> GetOpdExpensesAgainstEmailAddress(string EmailAddress)
        {
            var opdExpense = _opdExpenseRepository.GetQueryable()
                 .Where(y => y.EMPLOYEE_EMAILADDRESS == EmailAddress)
                 //.Select(t => new { t.EMPLOYEE_NAME, t.EMPLOYEE_EMAILADDRESS, t.EMPLOYEE_DEPARTMENT, t.CLAIM_MONTH, t.CLAIM_YEAR, t.TOTAL_AMOUNT_CLAIMED, t.STATUS, t.OPDTYPE, t.OPDEXPENSE_ID , t.EXPENSE_NUMBER })
                 .ToList();

            return Mapper.Map<List<OpdExpenseVM>>(opdExpense);
        }

        public OpdExpenseVM CreateOpdExpense(OpdExpenseVM opdExpenseVm)
        {
           
            object OpdExpense  =_opdExpenseRepository.Add(Mapper.Map<OpdExpense>(opdExpenseVm));
            return Mapper.Map<OpdExpenseVM>(OpdExpense);
        }


        public void UpdateOpdExpense(OpdExpenseVM opdExpenseVm)
        {

            _opdExpenseRepository.Update(Mapper.Map<OpdExpense>(opdExpenseVm));
            
        }

        public void DeleteOpdExpense(object id)
        {

            _opdExpenseRepository.Delete(id);

        }

        public List<OpdExpenseVM> GetOpdExpensesForHR()
        {            
             var opdExpense = _opdExpenseRepository.GetQueryable()
                 .Where(e => e.STATUS == Helper.GeneralStatus.Submitted.ToString() || e.STATUS == Helper.GeneralStatus.HRApproved.ToString() || e.STATUS == Helper.GeneralStatus.HRRejected.ToString() || e.STATUS == Helper.GeneralStatus.HRInProcess.ToString())               
                 .ToList();

            return Mapper.Map<List<OpdExpenseVM>>(opdExpense);
        }

        public List<OpdExpenseVM> GetOpdExpensesForFIN()
        {
            var opdExpense = _opdExpenseRepository.GetQueryable()
                .Where(e => e.STATUS == Helper.GeneralStatus.HRApproved.ToString() || e.STATUS == Helper.GeneralStatus.FINApproved.ToString() || e.STATUS == Helper.GeneralStatus.FINRejected.ToString() || e.STATUS == Helper.GeneralStatus.FINInProcess.ToString())
                .ToList();

            return Mapper.Map<List<OpdExpenseVM>>(opdExpense);
        }

        public List<OpdExpenseVM> GetOpdExpensesForMAN()
        {
            var opdExpense = _opdExpenseRepository.GetQueryable()
                .Where(e => e.STATUS == Helper.GeneralStatus.FINApproved.ToString() || e.STATUS == Helper.GeneralStatus.MANApproved.ToString() || e.STATUS == Helper.GeneralStatus.MANRejected.ToString() || e.STATUS == Helper.GeneralStatus.MANInProcess.ToString())
                .ToList();

            return Mapper.Map<List<OpdExpenseVM>>(opdExpense);
        }
    }
}


