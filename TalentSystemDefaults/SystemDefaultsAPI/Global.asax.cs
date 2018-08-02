using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
namespace SystemDefaultsAPI
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		public void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
		}
	}
}
