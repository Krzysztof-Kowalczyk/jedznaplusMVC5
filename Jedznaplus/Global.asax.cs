using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Jedznaplus.Resources;

namespace Jedznaplus
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            System.Data.Entity.Database.SetInitializer(new UserDbInitializer());
        }
    }
}
