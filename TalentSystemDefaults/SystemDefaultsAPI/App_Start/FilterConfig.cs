using System.Web.Mvc;
namespace SystemDefaultsAPI
{
	public sealed class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
