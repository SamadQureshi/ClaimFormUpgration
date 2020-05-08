using Onion.Interfaces.Services;
using Onion.WebApp.Utils;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Web.Mvc;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.WebApp.Controllers
{
    public class HomeController : Controller
    {

        private readonly IEmailService _emailService;
        private readonly IOpdExpenseService _opdExpenseService;

        public HomeController(IOpdExpenseService opdExpenseService, IEmailService emailService)
        {
            _opdExpenseService = opdExpenseService;
            _emailService = emailService;

        }

        public  ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                string userName = ClaimsPrincipal.Current.FindFirst("name").Value;

                string userId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userId))
                {
                    // Invalid principal, sign out
                    return RedirectToAction("SignOut", "OfficeManager", null);
                }

                // Since we cache tokens in the session, if the server restarts
                // but the browser still has a cached cookie, we may be
                // authenticated but not have a valid token cache. Check for this
                // and force signout.
                SessionTokenCache tokenCache = new SessionTokenCache(userId, HttpContext);
                if (!tokenCache.HasData())
                {
                    // Cache is empty, sign out
                    return RedirectToAction("SignOut", "OfficeManager", null);
                }

                UserAuthorization user = new UserAuthorization(_opdExpenseService);

                string userRoll = user.AuthenticateUser();

                if (user.ValidateEmailAddressManagerTravelApproval())
                {
                    ViewBag.RollTypeTravel = "MANTRAVEL";
                }
              
                ViewBag.RollType = userRoll;
                              

                ViewBag.UserName = userName;
            }

            return View();
        }  


        public ActionResult About()
        {
            return View();
        }      

    }
}