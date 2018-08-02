using System.Web.Http.Filters;

namespace TalentAPI.Filters
{
	public class ActionFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			//BaseInputModel.Source = "W";
		}
	}
}