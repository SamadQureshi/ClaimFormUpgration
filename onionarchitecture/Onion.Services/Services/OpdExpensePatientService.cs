using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Onion.Domain.Models;
using Onion.Interfaces;
using Onion.Interfaces.Services;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Services
{
    public class OpdExpensePatientService : IOpdExpensePatientService
    {
        private readonly IBaseRepository<OpdExpensePatient> _opdExpensePatientRepository;
        
        public OpdExpensePatientService(IBaseRepository<OpdExpensePatient> opdExpensePatientRepository)
        {
            _opdExpensePatientRepository = opdExpensePatientRepository;
        }

        public List<OpdExpensePatient> GetAllOpdExpensePatients()
        {
            return _opdExpensePatientRepository.GetQueryable().ToList();
        }


        public List<OpdExpensePatientVM> GetOpdExpensesPatientAgainstOpdExpenseId(int Id)
        {
            var opdExpense = _opdExpensePatientRepository.GetQueryable()
                 .Where(y => y.OpdExpenseId == Id)               
                 .ToList();

            return Mapper.Map<List<OpdExpensePatientVM>>(opdExpense);
        }

        public OpdExpensePatientVM CreateOpdExpensePatient(OpdExpensePatientVM opdExpensePatientVM,string emailAddress)
        {
           
            
            var ObjOpdExpensePatient = _opdExpensePatientRepository.Add(Mapper.Map<OpdExpensePatient>(opdExpensePatientVM), emailAddress);
            return Mapper.Map<OpdExpensePatientVM>(ObjOpdExpensePatient);
        }


        public void UpdateOpdExpensePatient(OpdExpensePatientVM opdExpensePatientVM, string emailAddress)
        {

            _opdExpensePatientRepository.Update(Mapper.Map<OpdExpensePatient>(opdExpensePatientVM), emailAddress);

        }

        public void DeleteOpdExpensePatient(object id, string emailAddress)
        {
           _opdExpensePatientRepository.Delete(id, emailAddress);

        }

    }
}


