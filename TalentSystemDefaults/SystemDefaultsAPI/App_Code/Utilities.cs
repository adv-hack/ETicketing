using System.Collections.Generic;
using System.Configuration;
using System.Data;
using TalentSystemDefaults;
using TalentSystemDefaults.DataAccess.DataObjects.SystemObjects;
namespace SystemDefaultsAPI
{
	public class Utilities
	{
		#region Public Properties
		private string _NoRecordsFoundMessage = "No records found";
		public string NoRecordsFoundMessage
		{
			get
			{
				return _NoRecordsFoundMessage;
			}
			set
			{
				_NoRecordsFoundMessage = value;
			}
		}
		#endregion
		#region Public Methods
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="params"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static DESettings GetDESettings(Dictionary<string, string> @params)
		{
			DESettings settings = new DESettings();
			settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings["TalentEBusinessDBConnectionString"].ToString();
			settings.SessionID = @params["sessionid"];
			if (@params.ContainsKey("businessunit"))
			{
				settings.BusinessUnit = @params["businessunit"];
			}
			settings.DefaultBusinessUnit = ConfigurationManager.AppSettings.Get("DefaultBusinessUnit");
			if (@params.ContainsKey("listname"))
			{
				settings.ListName = @params["listname"];
			}
			if (@params.ContainsKey("modulename"))
			{
				settings.Module_Name = @params["modulename"];
			}
			else if (settings.Module_Name == string.Empty && settings.ListName != string.Empty)
			{
				settings.Module_Name = settings.ListName;
			}
			if (@params.ContainsKey("mode"))
			{
				settings.Mode = @params["mode"];
			}
			if (@params.ContainsKey("variablekey1"))
			{
				settings.VariableKey1 = @params["variablekey1"];
			}
			if (@params.ContainsKey("variablekey2"))
			{
				settings.VariableKey2 = @params["variablekey2"];
			}
			if (@params.ContainsKey("variablekey3"))
			{
				settings.VariableKey3 = @params["variablekey3"];
			}
			if (@params.ContainsKey("variablekey4"))
			{
				settings.VariableKey4 = @params["variablekey4"];
			}
			if (@params.ContainsKey("searchtext"))
			{
				settings.SearchText = @params["searchtext"];
			}
			return settings;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="inputParams"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static Dictionary<string, string> GetParams(IEnumerable<KeyValuePair<string, string>> inputParams)
		{
			Dictionary<string, string> @params = new Dictionary<string, string>();
			foreach (var item in inputParams)
			{
				@params.Add(item.Key.ToLower(), item.Value);
			}
			if (!@params.ContainsKey("mode"))
			{
				@params.Add("mode", "update"); //set "UPDATE" as default mode; just in case when URL does not querystring parameter(mode) passed
			}
			return @params;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static bool CheckAgentLogin(DESettings settings)
		{
			string SessionID = settings.SessionID;
			DataTable agentDetails = new DataTable();
			bool retVal = true;
			agentDetails = RetrieveAgentDetailsFromSessionID(settings, SessionID);
			if (agentDetails.Rows.Count == 0)
			{
                retVal = false;
			}
			else
			{
				if (agentDetails.Rows.Count > 0)
				{
					settings.AgentName = TalentSystemDefaults.Utilities.CheckForDBNull_String(agentDetails.Rows[0]["AGENT_NAME"]);
					settings.Company = TalentSystemDefaults.Utilities.CheckForDBNull_String(agentDetails.Rows[0]["COMPANY"]);
				}
				retVal = true;
			}
			return retVal;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="sessionID"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static DataTable RetrieveAgentDetailsFromSessionID(DESettings settings, string sessionID)
		{
			tbl_agent tblAgent = new tbl_agent(ref settings);
			DataTable tblAgentDetails = new DataTable();
			tblAgentDetails = tblAgent.RetrieveAgentDetailsFromSessionID(sessionID);
			return tblAgentDetails;
		}
		/// <summary>
		/// Get WebAPI endpoint
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static string GetSystemDefaultsURL(DESettings settings)
		{
			TalentSystemDefaults.DataAccess.DataObjects.tbl_ecommerce_module_defaults_bu tblEcommerceDefaults = new TalentSystemDefaults.DataAccess.DataObjects.tbl_ecommerce_module_defaults_bu(ref settings);
			return tblEcommerceDefaults.GetSystemDefaultsURL();
		}
		#endregion
	}
}
