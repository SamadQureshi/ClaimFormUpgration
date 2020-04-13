using Onion.Interfaces.Services;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Onion.WebApp.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
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


                string emailAddress = GetEmailAddress();

                List<string> HRList = ConfigurationManager.AppSettings["HR:List"].Split(',').ToList<string>();

                List<string> FINList = ConfigurationManager.AppSettings["FIN:List"].Split(',').ToList<string>();

                List<string> MANList = ConfigurationManager.AppSettings["MAN:List"].Split(',').ToList<string>();

                List<string> GENList = ConfigurationManager.AppSettings["GEN:List"].Split(',').ToList<string>();

                if (HRList.Contains(emailAddress))
                {
                    ViewBag.RollType = "HR";
                }
                else if (FINList.Contains(emailAddress))
                {
                    ViewBag.RollType = "FIN";
                }
                else if (MANList.Contains(emailAddress))
                {
                    ViewBag.RollType = "MAN";
                }
                else if (GENList.Contains(emailAddress))
                {
                    ViewBag.RollType = "GEN";
                }

                ViewBag.UserName = userName;
            }

            return View();
        }

        private string GetEmailAddress()
        {
            OfficeManagerController managerController = new OfficeManagerController();
            string emailAddress = managerController.GetEmailAddress();

            return emailAddress;
        }


        public ActionResult About()
        {
            return View();
        }





        //private readonly IUserService _userService;

        //public HomeController(IUserService userService)
        //{
        //    _userService = userService;
        //}

        //public ActionResult Index()
        //{
        //    var users = _userService.GetAllUsers();
        //    return View(users);
        //}

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}