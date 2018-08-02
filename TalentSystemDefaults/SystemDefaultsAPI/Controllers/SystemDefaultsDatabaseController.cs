using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TalentSystemDefaults;
namespace SystemDefaultsAPI
{
	public class SystemDefaultsDatabaseController : SystemDefaultsBaseController
	{
		// GET
		public HttpResponseMessage GetValues()
		{
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
			base.SetParamsAndSettings();
			response = GetContent(settings);
			return response;
		}
		// POST api/<controller>
		public HttpResponseMessage PostValue([FromBody()]Dictionary<string, string> values)
		{
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
			base.SetParamsAndSettings();
			DatabaseUpdatesModel model = new DatabaseUpdatesModel(settings);
			model.BusinessUnitSelected = (!values.ContainsKey("BUSINESS_UNIT") ? settings.BusinessUnit : values["BUSINESS_UNIT"]);
			// save
			model.Save();
			response = GetContent(settings, model);
			return response;
		}
		private HttpResponseMessage GetContent(DESettings settings, DatabaseUpdatesModel model = null, string action = "get")
		{
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
			string result = string.Empty;
			if (settings.Module_Name != string.Empty && settings.BusinessUnit != string.Empty)
			{
				if (model == null)
				{
					model = new DatabaseUpdatesModel(settings);
				}
				result = ViewRenderer.RenderView("~/Views/DatabaseUpdates/DatabaseUpdates.cshtml", model, null);
			}
			response.Content = new StringContent(result);
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return response;
		}
	}
}
