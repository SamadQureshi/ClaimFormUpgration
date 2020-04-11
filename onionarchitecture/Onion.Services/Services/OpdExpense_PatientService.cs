using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Onion.Domain.Models;
using Onion.Interfaces;
using Onion.Interfaces.Services;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Services
{
    public class OpdExpense_PatientService : IOpdExpense_PatientService
    {
        private IBaseRepository<OpdExpense_Patient> _opdExpense_PatientRepository;
        
        public OpdExpense_PatientService(IBaseRepository<OpdExpense_Patient> opdExpense_PatientRepository)
        {
            _opdExpense_PatientRepository = opdExpense_PatientRepository;
        }

        public List<OpdExpense_Patient> GetAllOpdExpensePatients()
        {
            return _opdExpense_PatientRepository.GetQueryable().ToList();
        }


        public List<OpdExpense_PatientVM> GetOpdExpenses_PatientAgainstOpdExpenseId(int Id)
        {
            var opdExpense = _opdExpense_PatientRepository.GetQueryable()
                 .Where(y => y.OPDEXPENSE_ID == Id)               
                 .ToList();

            return Mapper.Map<List<OpdExpense_PatientVM>>(opdExpense);
        }

        public OpdExpense_PatientVM CreateOpdExpense_Patient(OpdExpense_PatientVM opdExpense_PatientVM)
        {
            object Obj_OpdExpense_Patient = _opdExpense_PatientRepository.Add(Mapper.Map<OpdExpense_Patient>(opdExpense_PatientVM));
            return Mapper.Map<OpdExpense_PatientVM>(Obj_OpdExpense_Patient);
        }


        public void UpdateOpdExpense_Patient(OpdExpense_PatientVM opdExpense_PatientVM)
        {

            _opdExpense_PatientRepository.Update(Mapper.Map<OpdExpense_Patient>(opdExpense_PatientVM));

        }

        public void DeleteOpdExpense_Patient(object id)
        {

            _opdExpense_PatientRepository.Delete(id);

        }

    }
}


