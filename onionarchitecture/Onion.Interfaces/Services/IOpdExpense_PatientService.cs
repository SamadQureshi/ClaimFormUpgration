using System.Collections.Generic;
using Onion.Domain.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Interfaces.Services
{
    public interface IOpdExpense_PatientService
    {

        List<OpdExpense_Patient> GetAllOpdExpensePatients();

        List<OpdExpense_PatientVM> GetOpdExpenses_PatientAgainstOpdExpenseId(int Id);

        OpdExpense_PatientVM CreateOpdExpense_Patient(OpdExpense_PatientVM opdExpense);

        void UpdateOpdExpense_Patient(OpdExpense_PatientVM opdExpense_PatientVM);

        void DeleteOpdExpense_Patient(object id);

    }
}