using System.Web.Http;

namespace TalentAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi2",
                routeTemplate: "{controller}/{action}/{id}",//http://localhost:1340/ActivitiesList/GetActivity/123
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "DefaultApi1",
                routeTemplate: "{controller}/{id}",//http://localhost:1340/ActivitiesList/123
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
