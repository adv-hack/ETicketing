using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using SystemDefaultsAPI.UtilityClasses;
using TalentSystemDefaults;
namespace SystemDefaultsAPI
{
	[EnableCors("*", "*", "*")]
	[SystemDefaultsAuthorization]
	[UnhandledExceptionFilter]
	public class SystemDefaultsBaseController : ApiController
	{
		protected DESettings settings { get; set; }
		protected Dictionary<string, string> @params { get; set; }
		protected void SetParamsAndSettings()
		{
			string agentName = string.Empty;
			string companyName = string.Empty;
			if (!(Request.GetRouteData().Values["AgentName"] == null))
			{
				agentName = Request.GetRouteData().Values["AgentName"].ToString();
				Request.GetRouteData().Values.Remove("AgentName");
			}
			if (!(Request.GetRouteData().Values["CompanyName"] == null))
			{
				companyName = Request.GetRouteData().Values["CompanyName"].ToString();
				Request.GetRouteData().Values.Remove("CompanyName");
			}
			@params = Utilities.GetParams(Request.GetQueryNameValuePairs());
			settings = Utilities.GetDESettings(@params);
			settings.AgentName = agentName;
			settings.Company = companyName;
		}

		public HttpResponseMessage RaiseError(string errMessage)
		{
			HttpResponseMessage objReturn = new HttpResponseMessage();
			objReturn.StatusCode = HttpStatusCode.InternalServerError;
			objReturn.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			objReturn.Content = new StringContent(errMessage);
			return objReturn;
		}
	}
}
