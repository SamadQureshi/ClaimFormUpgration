using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;


namespace Onion.WebApp.Controllers
{
    public class OfficeManagerController : Controller
    {

        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                // Signal OWIN to send an authorization request to Azure
                HttpContext.GetOwinContext().Authentication.Challenge(
                  new AuthenticationProperties { RedirectUri = "/" },
                  OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        public void SignOut()
        {
            if (Request.IsAuthenticated)
            {
                string userId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    // Get the user's token cache and clear it
                    SessionTokenCache tokenCache = new SessionTokenCache(userId, HttpContext);
                    tokenCache.Clear();
                }
            }
            // Send an OpenID Connect sign-out request. 
            HttpContext.GetOwinContext().Authentication.SignOut(
              CookieAuthenticationDefaults.AuthenticationType);
            Response.Redirect("/");
        }

      

        public async Task<string> GetAccessToken()
        {
            string accessToken = null;

            // Load the app config from web.config
            string appId = ConfigurationManager.AppSettings["ida:AppId"];
            string appPassword = ConfigurationManager.AppSettings["ida:AppPassword"];
            string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
            string[] scopes = ConfigurationManager.AppSettings["ida:AppScopes"]
                .Replace(' ', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Get the current user's ID
            string userId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    SessionTokenCache tokenCache = new SessionTokenCache(userId, HttpContext);

                    // Get the user's token cache


                    ConfidentialClientApplication cca = new ConfidentialClientApplication(
                        appId, redirectUri, new ClientCredential(appPassword), tokenCache.GetMsalCacheInstance(), null);

                    // Call AcquireTokenSilentAsync, which will return the cached
                    // access token if it has not expired. If it has expired, it will
                    // handle using the refresh token to get a new one.
                    AuthenticationResult result = await cca.AcquireTokenSilentAsync(scopes, cca.Users.First());

                    accessToken = result.AccessToken;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return accessToken;
        }

        public async Task<ActionResult> Inbox()
        {
            string token = await GetAccessToken();
            if (string.IsNullOrEmpty(token))
            {
                // If there's no token in the session, redirect to Home
                return Redirect("/");
            }

            GraphServiceClient client = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", token);

                        return Task.FromResult(0);
                    }));

            try
            {
                var mailResults = await client.Me.MailFolders.Inbox.Messages.Request()
                                    .OrderBy("receivedDateTime DESC")
                                    .Select("subject,receivedDateTime,from")
                                    .Top(10)
                                    .GetAsync();

                return View(mailResults.CurrentPage);
            }
            catch (ServiceException ex)
            {
                return RedirectToAction("Error", "Home", new { message = "ERROR retrieving messages", debug = ex.Message });
            }
        }

        public async Task<ActionResult> Calendar()
        {
            string token = await GetAccessToken();
            if (string.IsNullOrEmpty(token))
            {
                // If there's no token in the session, redirect to Home
                return Redirect("/");
            }

            GraphServiceClient client = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", token);

                        return Task.FromResult(0);
                    }));

            try
            {
                var eventResults = await client.Me.Events.Request()
                                    .OrderBy("start/dateTime DESC")
                                    .Select("subject,start,end")
                                    .Top(10)
                                    .GetAsync();

                return View(eventResults.CurrentPage);
            }
            catch (ServiceException ex)
            {
                return RedirectToAction("Error", "Home", new { message = "ERROR retrieving events", debug = ex.Message });
            }
        }

        public async Task<ActionResult> Contacts()
        {
            string token = await GetAccessToken();
            if (string.IsNullOrEmpty(token))
            {
                // If there's no token in the session, redirect to Home
                return Redirect("/");
            }

            GraphServiceClient client = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", token);

                        return Task.FromResult(0);
                    }));

            try
            {
                //var contactResults = await client.Me.Contacts.Request()
                //                    .OrderBy("displayName")
                //                    .Select("displayName,emailAddresses,mobilePhone")
                //                    .Top(10)
                //                    .GetAsync();

                var contactResults = await client.Contacts.Request()
                                    .Filter("Tahir")
                                    .OrderBy("displayName")
                                    .Select("displayName,emailAddresses,mobilePhone")
                                    .Top(10)
                                    .GetAsync();

                return View(contactResults.CurrentPage);
            }
            catch (ServiceException ex)
            {
                return RedirectToAction("Error", "Home", new { message = "ERROR retrieving contacts", debug = ex.Message });
            }
        }
        

        public string GetEmailAddress()
        {

            string emailAddress = ClaimsPrincipal.Current.FindFirst("preferred_username").Value;

            return emailAddress;
        }

        public string GetName()
        {         

            string userName = ClaimsPrincipal.Current.FindFirst("name").Value;

            return userName;
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

    }
}