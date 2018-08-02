using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using SystemDefaultsAPI.Models;
using TalentSystemDefaults;
namespace SystemDefaultsAPI
{
	public class SystemDefaultsListController : SystemDefaultsBaseController
	{
		// GET api/<controller>
		public HttpResponseMessage GetValues()
		{
			SetParamsAndSettings();
			if (settings.ListName == string.Empty)
			{
				settings.ListName = "Modules";
			}
			try
			{
				DLBase defaultList = GetDLObject(settings);
				return GetContent(settings, defaultList);
			}
			catch (TypeLoadException)
			{
				string message = "This list name does not exists in the application";
				return GetEmptyContent(message);
			}
		}
		// POST api/<controller>
		public HttpResponseMessage PostValue([FromBody()]Dictionary<string, string> values)
		{
			if (values["action"] == "delete")
			{
				base.SetParamsAndSettings();
				DLBase defaultList = GetDLObject(settings, values);
				if (values != null)
				{
					settings.VariableKey1 = values["variableKey1"];
					settings.VariableKey2 = values["variableKey2"];
					settings.VariableKey3 = values["variableKey3"];
					settings.VariableKey4 = values["variableKey4"];
					defaultList.Delete();
				}
				return GetContent(settings, defaultList);
			}
			return GetEmptyContent();
		}
		private HttpResponseMessage GetContent(DESettings settings, DLBase defaultList)
		{
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
			string result = string.Empty;
			defaultList.Populate();
			DefaultListModel model = new DefaultListModel(defaultList.DEList);
			model.BackURL = string.Format(model.BackURL, settings.BusinessUnit);
			result = ViewRenderer.RenderView("~/Views/SystemDefaults/DefaultList.cshtml", model, null);
			response.Content = new StringContent(result);
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return response;
		}
		private DLBase GetDLObject(DESettings settings, Dictionary<string, string> filters = null)
		{
			string listName = settings.ListName;
			DLBase defaultList = DMFactory.GetDLObject(listName, settings, filters);
			return defaultList;
		}
		private HttpResponseMessage GetEmptyContent(string message = "")
		{
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
			response.Content = new StringContent(message);
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return response;
		}
	}
}
