﻿using Onion.DependencyResolution;
using System.IdentityModel.Claims;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TrackerEnabledDbContext.Common.Configuration;

namespace Onion.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
       
        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());
            AutomapperConfiguration.IntializeAutomapper();

            var kernel = WebApiConfig.Register(GlobalConfiguration.Configuration);

            GlobalConfiguration.Configuration.EnsureInitialized();

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            GlobalTrackingConfig.DisconnectedContext = true;
        }

    }
}
