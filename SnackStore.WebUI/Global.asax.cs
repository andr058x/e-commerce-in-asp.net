using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SnackStore.Domain.Entities;
using SnackStore.WebUI.Binders;
using SnackStore.WebUI.Infrastructure;
using SnackStore.WebUI.Models;

namespace SnackStore.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());
        }
    }
}
