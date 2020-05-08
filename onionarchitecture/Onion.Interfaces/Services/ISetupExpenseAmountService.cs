using System.Collections.Generic;
using Onion.Domain.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Interfaces.Services
{
    public interface ISetupExpenseAmountService
    {

        string GetDefaultExpenseAmountAgainstExpenseType(string opdType);

    }
}