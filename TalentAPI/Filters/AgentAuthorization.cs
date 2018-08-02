using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using TalentBusinessLogic.BusinessObjects.Environment;

namespace TalentAPI.Filters
{
	public class AgentAuthorization : AuthorizationFilterAttribute
	{
		public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			bool agentLoggedIn = false;
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
			string sessionId = string.Empty;

			// Is the session id part of the post data
			sessionId = HttpContext.Current.Request.Form["SessionID"];

            // If not, it could be part of the query string values
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                sessionId = HttpUtility.ParseQueryString(HttpContext.Current.Request.Url.Query).Get("SessionID");
            }

			if (!string.IsNullOrWhiteSpace(sessionId))
			{
				// Validate that the session id belongs to an agent
				Agent _agent = new Agent();
				agentLoggedIn = _agent.CheckAgentLogin(sessionId);
			}
			if (!agentLoggedIn)
			{
				actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
			}
		}
	}
}