using System.Web.Mvc;
namespace SystemDefaultsAPI
{
	public sealed class RouteConfig
	{
		public static void RegisterRoutes(System.Web.Routing.RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }
				);
		}
	}
}
