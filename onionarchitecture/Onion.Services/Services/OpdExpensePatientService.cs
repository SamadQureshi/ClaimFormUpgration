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
        private readonly IBaseRepository<OpdExpense_Patient> _opdExpensePatientRepository;
        
        public OpdExpensePatientService(IBaseRepository<OpdExpense_Patient> opdExpensePatientRepository)
        {
            _opdExpensePatientRepository = opdExpensePatientRepository;
        }

        public List<OpdExpense_Patient> GetAllOpdExpensePatients()
        {
            return _opdExpensePatientRepository.GetQueryable().ToList();
        }


        public List<OpdExpensePatientVM> GetOpdExpensesPatientAgainstOpdExpenseId(int Id)
        {
            var opdExpense = _opdExpensePatientRepository.GetQueryable()
                 .Where(y => y.OPDEXPENSE_ID == Id)               
                 .ToList();

            return Mapper.Map<List<OpdExpensePatientVM>>(opdExpense);
        }

        public OpdExpensePatientVM CreateOpdExpensePatient(OpdExpensePatientVM opdExpensePatientVM)
        {
            object ObjOpdExpensePatient = _opdExpensePatientRepository.Add(Mapper.Map<OpdExpense_Patient>(opdExpensePatientVM));
            return Mapper.Map<OpdExpensePatientVM>(ObjOpdExpensePatient);
        }


        public void UpdateOpdExpensePatient(OpdExpensePatientVM opdExpensePatientVM)
        {

            _opdExpensePatientRepository.Update(Mapper.Map<OpdExpense_Patient>(opdExpensePatientVM));

        }

        public void DeleteOpdExpensePatient(object id)
        {

            _opdExpensePatientRepository.Delete(id);

        }

    }
}


