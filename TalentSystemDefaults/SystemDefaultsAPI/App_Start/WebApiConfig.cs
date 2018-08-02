using System.Web.Http;
namespace SystemDefaultsAPI
{
	public sealed class WebApiConfig
	{
		public static void Register(System.Web.Http.HttpConfiguration config)
		{
			config.EnableCors();
			// Web API configuration and services
			// Web API routes
			config.MapHttpAttributeRoutes();
			config.Routes.MapHttpRoute("DefaultApi1", "api/{controller}");
            config.Routes.MapHttpRoute("DefaultApi2", "api/{controller}/{value}");
            config.Routes.MapHttpRoute("DefaultApi3", "api/{controller}/{action}/{value}");
            config.Routes.MapHttpRoute("DefaultApi4", "api/{controller}/{value1}/{value2}/{value3}", new { value1 = RouteParameter.Optional, value2 = RouteParameter.Optional, value3 = RouteParameter.Optional });
			//Dim appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(Function(t) t.MediaType = "application/xml")
			//config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType)
		}
	}
}
