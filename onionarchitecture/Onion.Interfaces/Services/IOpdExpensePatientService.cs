using System.Collections.Generic;
using Onion.Domain.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Interfaces.Services
{
    public interface IOpdExpensePatientService
    {

        List<OpdExpensePatient> GetAllOpdExpensePatients();

        List<OpdExpensePatientVM> GetOpdExpensesPatientAgainstOpdExpenseId(int Id);

        OpdExpensePatientVM CreateOpdExpensePatient(OpdExpensePatientVM opdExpense, string emailAddress);

        void UpdateOpdExpensePatient(OpdExpensePatientVM opdExpensePatientVM, string emailAddress);

        void DeleteOpdExpensePatient(object id, string emailAddress);



    }
}