using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using SystemDefaultsAPI.Models;
using TalentSystemDefaults;
namespace SystemDefaultsAPI
{
	public class SystemDefaultsValuesController : SystemDefaultsBaseController
	{
		// GET api/SystemDefaultsValues
		public HttpResponseMessage GetValues()
		{
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
			string result = string.Empty;
			// If systemdefaults.aspx is called directly(or missing required querystring parameters) then redirect to the "Module List" paeg
			base.SetParamsAndSettings();
			if (!@params.ContainsKey("modulename") || !@params.ContainsKey("businessunit"))
			{
				result = "returnURL=DefaultList.aspx";
			}
			else
			{
				ConfigurationModel model = new ConfigurationModel(settings);
				model.Initialise();
				result = ViewRenderer.RenderView("~/Views/SystemDefaults/Configuration.cshtml", model, null);
			}
			response.Content = new StringContent(result);
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return response;
		}
		// POST api/SystemDefaultsValues
		public HttpResponseMessage PostValue(DefaultValues[] values)
		{
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
			base.SetParamsAndSettings();
			ConfigurationModel model = new ConfigurationModel(settings);
			string result = string.Empty;
			List<RequestData> requestDataList = GetRequestDataList(values);
			model.Initialise(requestDataList);
			bool isValid = model.Validate();
			if (isValid)
			{
				model.Save();
				model.Initialise();
			}
			else
			{
				model.SetTabWithErrorActive();
			}
			if (model.RedirectURL == string.Empty)
			{
				result = ViewRenderer.RenderView("~/Views/SystemDefaults/Configuration.cshtml", model, null);
			}
			else
			{
				result = "returnURL=" + model.RedirectURL;
			}
			response.Content = new StringContent(result);
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return response;
		}
		private List<RequestData> GetRequestDataList(DefaultValues[] values)
		{
			Hasher hasher = new Hasher();
			List<RequestData> requestDataList = new List<RequestData>();
			string updatedValue = "";
			foreach (var value in values)
			{
				string[] items = hasher.DeHashFullString(value.MetaData).Split(",".ToCharArray(), StringSplitOptions.None);
				updatedValue = value.UpdatedValue;
				requestDataList.Add(new RequestData() { FieldType = items[0], TableName = items[1], DefaultKey1 = items[2], FieldName = items[3], Id = items[4], CurrentValue = hasher.DeHashFullString(value.CurrentValue), UpdatedValue = updatedValue });
			}
			return requestDataList;
		}
	}
}
