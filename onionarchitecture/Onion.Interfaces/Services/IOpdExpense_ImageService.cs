using System.Collections.Generic;
using Onion.Domain.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Interfaces.Services
{
    public interface IOpdExpense_ImageService
    {

        List<OpdExpense_Image> GetAllOpdExpenseImages();
       List<OpdExpense_ImageVM> GetOpdExpenses_ImageAgainstOpdExpenseId(int Id);

    }
}