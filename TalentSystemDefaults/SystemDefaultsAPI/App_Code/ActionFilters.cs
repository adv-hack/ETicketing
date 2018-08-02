using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Filters;
using TalentSystemDefaults;
namespace SystemDefaultsAPI
{
	namespace UtilityClasses
	{
		public class SystemDefaultsAuthorization : AuthorizationFilterAttribute
		{
			public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
			{
				Dictionary<string, string> @params = new Dictionary<string, string>();
				DESettings settings = default(DESettings);
				bool agentLoggedIn = false;
				HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
				@params = Utilities.GetParams(actionContext.Request.GetRequestContext().Url.Request.GetQueryNameValuePairs());
				settings = Utilities.GetDESettings(@params);
				if (settings.BusinessUnit == string.Empty || settings.BusinessUnit == null)
				{
					settings.BusinessUnit = settings.DefaultBusinessUnit;
				}
				agentLoggedIn = Utilities.CheckAgentLogin(settings);
				//uncomment to test the authorization
				//agentLoggedIn = False
				if (!agentLoggedIn)
				{
					actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
				}
				else
				{
					base.OnAuthorization(actionContext);
					actionContext.RequestContext.RouteData.Values.Add("AgentName", settings.AgentName);
					actionContext.RequestContext.RouteData.Values.Add("CompanyName", settings.Company);
				}
			}
		}
		public class UnhandledExceptionFilter : ExceptionFilterAttribute
		{
			public override void OnException(HttpActionExecutedContext context)
			{
				Dictionary<string, string> @params = new Dictionary<string, string>();
				DESettings settings = default(DESettings);
				HttpResponseMessage objReponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
				string sourceClass, sourceMethod, logCode, logFilter1, logFilter2, logFilter5, logContent;

				@params = Utilities.GetParams(context.Request.GetRequestContext().Url.Request.GetQueryNameValuePairs());
				settings = Utilities.GetDESettings(@params);

				// log error into DB-tbl_logs
				sourceClass = context.Exception.Source;
				sourceMethod = context.Exception.TargetSite.ReflectedType.Name + "." + context.Exception.TargetSite.Name;
				logCode = TalentSystemDefaults.GlobalConstants.LOG_ERROR_CODE;
				logFilter1 = settings.BusinessUnit;
				logFilter2 = context.Request.RequestUri.ToString();
				logFilter5 = context.Exception.StackTrace;
				logContent = context.Exception.Message;

				var objLogInserted = TalentSystemDefaults.Utilities.InsertErrorLog(settings, sourceClass, sourceMethod, logCode, logFilter1, logFilter2, "", "", logFilter5, logContent);

				objReponse.Content = new StringContent(context.Exception.Message);
				objReponse.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
				context.Response = objReponse;

				base.OnException(context);
			}
		}
	}
}
