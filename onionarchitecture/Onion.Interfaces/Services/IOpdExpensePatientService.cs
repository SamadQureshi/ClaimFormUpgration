using System.Collections.Generic;
using Onion.Domain.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Interfaces.Services
{
    public interface IOpdExpensePatientService
    {

        List<OpdExpense_Patient> GetAllOpdExpensePatients();

        List<OpdExpensePatientVM> GetOpdExpensesPatientAgainstOpdExpenseId(int Id);

        OpdExpensePatientVM CreateOpdExpensePatient(OpdExpensePatientVM opdExpense);

        void UpdateOpdExpensePatient(OpdExpensePatientVM opdExpensePatientVM);

        void DeleteOpdExpensePatient(object id);

    }
}