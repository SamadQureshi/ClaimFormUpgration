﻿using System.Collections.Generic;
using Onion.Domain.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Interfaces.Services
{
    public interface ITravelExpenseService
    {

        List<TravelExpense> GetAllTravelExpenses();

        List<TravelExpenseVM> GetTravelExpensesAgainstOpdExpenseId(int id);

        TravelExpenseVM CreateTravelExpense(TravelExpenseVM travelExpenseVM, string emailAddress);

        void UpdateTravelExpense(TravelExpenseVM travelExpenseVM, string emailAddress);

        void DeleteTravelExpense(object id, string emailAddress);
        TravelExpenseVM GetTravelExpenseAgainstId(int Id);

        void UpdateTravelExpense(TravelExpenseMasterDetail travelExpenseMasterDetail, string emailAddress);

    }
}