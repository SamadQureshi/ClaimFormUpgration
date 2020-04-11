using System.Collections.Generic;
using System.Web.Http;
using Ninject;
using Ninject.Modules;
using Onion.DependencyResolution;
using WebApiContrib.IoC.Ninject;

namespace Onion.WebApp
{
    public static class WebApiConfig
    {
        public static StandardKernel Register(HttpConfiguration config)
        {
            config.Filters.Add(new AuthorizeAttribute());
            var kernel = new StandardKernel();
            Register(config, kernel);
            return kernel;
        }

        public static void Register(HttpConfiguration config, IKernel kernel)
        {
            // Web API configuration and services
            // comment in to mock the persistence layer
            var modules = new List<INinjectModule>
            {
                new RepositoryModule(),
                new ServiceModule()
            };

            kernel.Load(modules);

            config.DependencyResolver = new NinjectResolver(kernel);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
            );
        }
    }
}