namespace MVCAzureStore
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.WindowsAzure.ServiceRuntime;

    //// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    //// visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }); // Parameter defaults 
        }

        protected void Application_Start()
        {
            Microsoft.WindowsAzure.CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));
            });

            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);            
        }   

        protected void Session_Start()
        {
            this.Session["EnableCache"] = false;
            this.Session["EnableLocalCache"] = false;
        }
    }
}