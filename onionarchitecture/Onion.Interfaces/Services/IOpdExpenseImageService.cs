using System.Collections.Generic;
using Onion.Domain.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Interfaces.Services
{
    public interface IOpdExpenseImageService
    {

        List<OpdExpenseImage> GetAllOpdExpenseImages();
       List<OpdExpenseImageVM> GetOpdExpensesImageAgainstOpdExpenseId(int Id);

        OpdExpenseImageVM CreateOpdExpenseImage(OpdExpenseImageVM opdExpense);

        void UpdateOpdExpenseImage(OpdExpenseImageVM opdExpenseImageVM);

        void DeleteOpdExpenseImage(object id);

        OpdExpenseImageVM GetOpdExpensesImagesAgainstId(int Id);
    }
}