﻿using System.Collections.Generic;
using Onion.Domain.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Interfaces.Services
{
    public interface IOpdExpenseService
    {

        List<OpdExpense> GetAllOpdExpenses();

        OpdExpenseVM  GetOpdExpensesAgainstId(int Id);

        List<OpdExpenseVM> GetOpdExpensesAgainstEmailAddress(string EmailAddress);

        OpdExpenseVM CreateOpdExpense(OpdExpenseVM opdExpense);

        void UpdateOpdExpense(OpdExpenseVM opdExpenseVm);

        void DeleteOpdExpense(object id);

        List<OpdExpenseVM> GetOpdExpensesForHR();
        List<OpdExpenseVM> GetOpdExpensesForFIN();
        List<OpdExpenseVM> GetOpdExpensesForMAN();

        List<OpdExpenseVM> GetOpdExpensesForMANTravel(string emailAddress);


        decimal? GetClaimAmountAgainstEmailAddress(string EmailAddress, string OpdType, string HospitalType, string MaternityType);

        decimal? GetApprovedAmountAgainstEmailAddress(string EmailAddress, string OpdType, string HospitalType, string MaternityType);

    }
}