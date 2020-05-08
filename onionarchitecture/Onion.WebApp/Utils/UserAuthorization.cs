using Onion.Interfaces.Services;
using Onion.WebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.WebApp.Utils
{
    public class UserAuthorization 
    {
       
        private  readonly IOpdExpenseService _opdExpenseService;

        public UserAuthorization(IOpdExpenseService opdExpenseService)
        {
            _opdExpenseService = opdExpenseService;
           

        }

        public string AuthenticateUser()
        {

            string emailAddress = GetEmailAddress();
            string rollType = string.Empty;

            List<string> HRList = ConfigurationManager.AppSettings["HR:List"].Split(',').ToList<string>();

            List<string> FINList = ConfigurationManager.AppSettings["FIN:List"].Split(',').ToList<string>();

            List<string> GENList = ConfigurationManager.AppSettings["GEN:List"].Split(',').ToList<string>();

            List<string> MANList = ConfigurationManager.AppSettings["MAN:List"].Split(',').ToList<string>();

            if (HRList.Contains(emailAddress))
            {
                rollType = "HR";
            }
            else if (FINList.Contains(emailAddress))
            {
                rollType = "FIN";
            }
            else if (MANList.Contains(emailAddress))
            {
                rollType = "MAN";
            }
            else if (GENList.Contains(emailAddress))
            {
                rollType = "GEN";
            }
           
            return rollType;

        }

        public bool ValidateEmailAddressManagerTravelApproval()
        {
            bool result = false;
            string emailAddress = GetEmailAddress();
            if (ValidEmailAddress(emailAddress))
            {
                result = true;
            }

            return result;
        }





        public bool ValidEmailAddress(string emailAddress)
        {

            bool result = false;

            List<OpdExpenseVM> list = _opdExpenseService.GetOpdExpensesForMANTravel(emailAddress);

            if (list.Count > 0)
            {
                result = true;
            }
            return result;
        }

        private string GetEmailAddress()
        {
            OfficeManagerController managerController = new OfficeManagerController();
            string emailAddress = managerController.GetEmailAddress();

            return emailAddress;
        }
    }
}
