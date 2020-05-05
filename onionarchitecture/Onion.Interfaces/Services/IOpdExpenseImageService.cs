using System.Collections.Generic;
using Onion.Domain.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Interfaces.Services
{
    public interface IOpdExpenseImageService
    {

        List<OpdExpenseImage> GetAllOpdExpenseImages();
       List<OpdExpenseImageVM> GetOpdExpensesImageAgainstOpdExpenseId(int Id);

        OpdExpenseImageVM CreateOpdExpenseImage(OpdExpenseImageVM opdExpense, string emailAddress);

        void UpdateOpdExpenseImage(OpdExpenseImageVM opdExpenseImageVM, string emailAddress);

        void DeleteOpdExpenseImage(object id, string emailAddress);

        OpdExpenseImageVM GetOpdExpensesImagesAgainstId(int Id);
    }
}