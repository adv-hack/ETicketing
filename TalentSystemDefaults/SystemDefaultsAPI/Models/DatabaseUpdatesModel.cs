using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TalentSystemDefaults;
using TalentSystemDefaults.DataAccess.ConfigObjects;
namespace SystemDefaultsAPI
{
	public class DatabaseUpdatesModel
	{
		private DESettings settings;
		private tbl_config_detail_audit objAudit;
		const string AT_SYMBOL = "@";
		public const string BUSINESS_UNIT = "BUSINESS_UNIT";
		public const string APPLY_IN_BUSINESS_UNIT = "Apply in Business Unit";
		public const string CONNECTION_STRING = "CONNECTION_STRING";
		public List<SelectListItem> BusinessUnits { get; set; }
		private string _BackURL = "/DefaultList.aspx?listname={0}";
		public string BackURL
		{
			get
			{
				return _BackURL;
			}
			set
			{
				_BackURL = value;
			}
		}
		private string _StatusMessage = string.Empty;
		public string StatusMessage
		{
			get
			{
				return _StatusMessage;
			}
			set
			{
				_StatusMessage = value;
			}
		}
		public List<SelectListItem> ConnectionStrings { get; set; }
		//Public Property ConnectionStringSelected As String = String.Empty ' NOTE: PLEASE DO NOT REMOVE THIS LINE
		private string _BusinessUnitSelected = string.Empty;
		public string BusinessUnitSelected
		{
			get
			{
				return _BusinessUnitSelected;
			}
			set
			{
				_BusinessUnitSelected = value;
			}
		}
		private bool _HasBusinessUnit = false;
		public bool HasBusinessUnit
		{
			get
			{
				return _HasBusinessUnit;
			}
			set
			{
				_HasBusinessUnit = value;
			}
		}
		public Dictionary<string, string> AuditGroupInfo { get; set; }
		public List<TalentDataAccess> TalentAccessDetailList { get; set; }
		public bool HasStatus
		{
			get
			{
				return !string.IsNullOrEmpty(StatusMessage);
			}
		}
		public DatabaseUpdatesModel(DESettings settings)
		{
			// VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
			BusinessUnits = new List<SelectListItem>();
			ConnectionStrings = new List<SelectListItem>();
			this.settings = settings;
			objAudit = new tbl_config_detail_audit(ref settings);
			Initialise();
		}
		protected void Initialise()
		{
			string groupID = settings.VariableKey1;
			TalentAccessDetailList = new List<TalentDataAccess>();
			try
			{
				TalentAccessDetailList = objAudit.GetCommandsByGroupId(int.Parse(groupID));
				DESQLParameter currBusinessUnit = null;
				foreach (TalentDataAccess sqlAccess in TalentAccessDetailList)
				{
					currBusinessUnit = sqlAccess.CommandElements.CommandParameter.Find(x => x.ParamName.Contains(BUSINESS_UNIT));
					if (!(currBusinessUnit == null))
					{
						HasBusinessUnit = true;
					}
				}
				if (HasBusinessUnit)
				{
					string[] items = TalentSystemDefaults.Utilities.GetBusinessUnits(settings);
					foreach (string item in items)
					{
						bool selected = string.Compare(currBusinessUnit.ParamValue.ToString(), item, true) == 0;
						BusinessUnits.Add(new SelectListItem { Text = item, Value = item, Selected = selected });
					}
				}
				ConnectionStrings.Add(new SelectListItem { Text = "TALENTEBUSINESS", Value = "SQL2005", Selected = true });
				ConnectionStrings.Add(new SelectListItem { Text = "TALENT CONFIGURATION", Value = "TALENT_CONFIG", Selected = false });
				if (!string.IsNullOrEmpty(settings.SearchText))
				{
					BackURL += "&searchText={1}";
				}
				BackURL = string.Format(BackURL, settings.Module_Name, settings.SearchText);
				AuditGroupInfo = objAudit.RetrieveAuditGroupInfo(int.Parse(groupID));
			}
			catch (Exception)
			{
			}
		}
		public bool Save()
		{
			bool result = true;
			List<TalentDataAccess> lstTalentAccessDetail = new List<TalentDataAccess>();
			string Module_Name = string.Empty;
			try
			{
				lstTalentAccessDetail = objAudit.GetCommandsByGroupId(int.Parse(settings.VariableKey1));
				foreach (TalentDataAccess objTalentAccess in lstTalentAccessDetail)
				{
					this.settings.Module_Name = objTalentAccess.Settings.Module_Name;
					this.settings.BusinessUnit = BusinessUnitSelected;
					objTalentAccess.Settings = this.settings;
					// change the parameter value
					foreach (var commandParam in objTalentAccess.CommandElements.CommandParameter)
					{
						if (commandParam.ParamName == "@BUSINESS_UNIT")
						{
							commandParam.ParamValue = BusinessUnitSelected;
							break;
						}
					}
					// NOTE: PLEASE DO NOT REMOVE THE COMMENTED CODE
					// update
					//If ConnectionStringSelected = "SQL2005" Then
					objTalentAccess.SQLAccess(DestinationDatabase.SQL2005);
					//ElseIf ConnectionStringSelected = "TALENT_CONFIG" Then
					//objTalentAccess.SQLAccess(DestinationDatabase.TALENT_CONFIG)
					//End If
				}
				StatusMessage = "Data saved successfully";
			}
			catch (Exception)
			{
				result = false;
			}
			return true;
		}
		public string GetParamName(string paramName)
		{
			paramName = paramName.Replace("@VAR_", string.Empty).Replace("@", string.Empty);
			paramName = Regex.Replace(paramName, @"[0-9]", string.Empty);
			return GetProperCase(paramName);
		}
		public string GetProperCase(string name)
		{
			name = Strings.StrConv(name, VbStrConv.ProperCase);
			name = name.Replace("_", " ");
			return name;
		}
	}
}
