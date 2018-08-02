using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using SystemDefaultsAPI.Models;
using SystemDefaultsAPI.UtilityClasses;
namespace SystemDefaultsAPI
{
	namespace Controllers
	{
		public class DatabaseSearchController : SystemDefaultsBaseController
		{
			private const string MODULENAME = "DATABASESEARCH";
			//<Route("api/DatabaseSearch/GetSearchPage")>
			public HttpResponseMessage GetSearchPage()
			{
				DatabaseSearchModel model = default(DatabaseSearchModel);
				HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
				string result = string.Empty;
				string baseURL = string.Empty;
				string queryString = string.Empty;
				base.SetParamsAndSettings();
				settings.BusinessUnit = ConfigurationManager.AppSettings["DefaultBusinessUnit"].ToString();
				model = new DatabaseSearchModel(settings);
				baseURL = Utilities.GetSystemDefaultsURL(settings);
				if (string.IsNullOrWhiteSpace(baseURL))
				{
					baseURL = Request.RequestUri.AbsoluteUri.Split("?".ToCharArray())[0];
				}
				else
				{
					if (Request.RequestUri.AbsolutePath.Substring(1, Request.RequestUri.AbsolutePath.Length - 1).ToLower().StartsWith("api/"))
					{
						baseURL = baseURL + Request.RequestUri.AbsolutePath.Substring(1, Request.RequestUri.AbsolutePath.Length - 1);
					}
					else
					{
						baseURL = baseURL + Request.RequestUri.AbsolutePath.Substring(1, Request.RequestUri.AbsolutePath.Length - 1).Split("/".ToCharArray(), 2)[1];
					}
				}
				queryString = string.Format("?businessunit={0}&sessionid={1}", ConfigurationManager.AppSettings["DefaultBusinessUnit"].ToString(), @params["sessionid"]);
				model.GETDataUrl = baseURL + "GetSearchData" + queryString;
				model.UPDATEDataUrl = baseURL + "SaveData" + queryString;
				result = ViewRenderer.RenderView("~/Views/DatabaseSearch/DatabaseSearch.cshtml", model, null);
				response.Content = new StringContent(result);
				response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
				return response;
			}
			[System.Web.Http.Route("api/DatabaseSearch/GetSearchData")]
			[System.Web.Http.HttpPost]
			public JQueryDataTable GetSearchData(DTParameters dtParams)
			{
				JQueryDataTable retVal = new JQueryDataTable();
				DatabaseSearchModel model = default(DatabaseSearchModel);
				int offset = 0;
				int pageSize = 0;
				int draw = 0;
				string sortColumn = string.Empty;
				string searchText = string.Empty;
				string searchType = string.Empty;
				string sortOrder = string.Empty;
				base.SetParamsAndSettings();
				string[] columnNames = dtParams.columnnames.Split(",".ToCharArray(), StringSplitOptions.None);
				var columnIndex = System.Convert.ToInt32(dtParams.order[0].Column);
				offset = dtParams.start;
				pageSize = dtParams.length;
				sortColumn = columnNames[columnIndex];
				sortOrder = dtParams.order[0].Dir.ToString();
				searchText = dtParams.searchtext;
				searchType = dtParams.searchtype;
				draw = dtParams.draw;
				model = new DatabaseSearchModel(settings);
				retVal = model.GetData(@params, draw, searchType, offset, pageSize, sortColumn, sortOrder, searchText);
				return retVal;
			}
			[System.Web.Http.Route("api/DatabaseSearch/SaveData")]
			[System.Web.Http.HttpPost]
			public bool SaveData()
			{
				DatabaseSearchModel model = default(DatabaseSearchModel);
				bool retVal = true;
				string jsonData = string.Empty;
				string searchType = "TEXTSEARCH"; // todo: get it on run time from web page
				base.SetParamsAndSettings();
				settings.Module_Name = MODULENAME;
				jsonData = Request.Content.ReadAsStringAsync().Result;
				model = new DatabaseSearchModel(settings);
				retVal = model.Save(@params, searchType, jsonData);
				return retVal;
			}
		}
	}
}
