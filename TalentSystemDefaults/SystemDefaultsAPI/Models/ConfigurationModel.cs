using System.Collections.Generic;
using System.Linq;
using TalentSystemDefaults;
namespace SystemDefaultsAPI
{
	namespace Models
	{
		public class ConfigurationModel
		{
			public Dictionary<string, TabContentModel> Tabs { get; set; }
			public string StatusMessage { get; set; }
            public bool HasError;
			public string DefaultTabType { get; set; }
			private string _RedirectURL = string.Empty;
			public string RedirectURL
			{
				get
				{
					return _RedirectURL;
				}
				set
				{ 
					_RedirectURL = value;
				}
			}
			private string _BackURL = "#";
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
			public Dictionary<string, string> HiddenFields { get; set; }
			private DESettings settings;
			private DMBase dataModuleObj;
			public bool HasStatus
			{
				get
				{
					return !string.IsNullOrEmpty(StatusMessage);
				}
			}
			public Dictionary<string, string> TabClasses
			{
				get
				{
					return dataModuleObj.DisplayTabs.Classes;
				}
			}
			public Dictionary<string, string> TabTypes
			{
				get
				{
					return DisplayTabs.TabTypes;
				}
			}
			public ConfigurationModel(DESettings settings)
			{
				// VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
				Tabs = new Dictionary<string, TabContentModel>();
				HiddenFields = new Dictionary<string, string>();
				this.settings = settings;
			}
			public void Initialise(List<RequestData> requestDataList = null)
			{
				dataModuleObj = DMFactory.GetDMObject(settings);
				PrepareCommonHiddenFields();
				BaseFieldModel model = null;
				object fieldType = null;
				int i = 0;
				string key = null;
				RequestData requestData = null;
				// clear all tabs
				Tabs.Clear();
				// set default tab orientation
				DefaultTabType = dataModuleObj.DefaultTabType;
				foreach (var mConfig in dataModuleObj.MConfigs)
				{
					fieldType = mConfig.FieldType;
					//get the matching requested data
					if (!(requestDataList == null))
					{
						requestData = requestDataList.Find(x => x.FieldName == (mConfig.ConfigurationItem.DefaultName + mConfig.UniqueId) && x.TableName == mConfig.ConfigurationItem.TableName);
						if (requestData != null)
						{
							string updatedValue = requestData.UpdatedValue;
							setUpdatedValue(mConfig, updatedValue);
						}
					}
					model = null;
					//Dim pageTexts As Dictionary(Of String, String) = settings.PageTexts
					switch ((DataType)fieldType)
					{
						case DataType.TEXT:
							model = new TextBoxFieldModel(i, mConfig);
							break;
						case DataType.TEXTAREA:
							model = new TextAreaFieldModel(i, mConfig);
							break;
						case DataType.DROPDOWN:
							model = new DropdownFieldModel(i, mConfig);
							break;
						case DataType.BOOL:
						case DataType.BOOL_10:
						case DataType.BOOL_YN:
							model = new BooleanFieldModel(i, mConfig);
							break;
						case DataType.LABEL:
							model = new LabelFieldModel(i, mConfig);
							break;
					}
					key = mConfig.TabHeader;
					if (model != null)
					{
						if (!Tabs.ContainsKey(key))
						{
							Tabs.Add(key, new TabContentModel(key, i == 0 ? true : false));
						}
						Tabs[key].Add(model);
					}
					i++;
				}
				BackURL = dataModuleObj.BackUrl();
			}
			private void setUpdatedValue(ModuleConfiguration mConfig, string updatedData)
			{
				if (mConfig.FieldType == DataType.BOOL_YN)
				{
					mConfig.ConfigurationItem.UpdatedValue = (updatedData.ToLower() == "true" || updatedData == "1" || updatedData.ToUpper() == "Y" ? "Y" : "N");
				}
				else if (mConfig.FieldType == DataType.BOOL_10)
				{
					mConfig.ConfigurationItem.UpdatedValue = (updatedData.ToLower() == "true" || updatedData == "1" || updatedData.ToUpper() == "Y" ? "1" : "0");
				}
				else
				{
					mConfig.ConfigurationItem.UpdatedValue = updatedData.Trim();
				}
			}
			public bool Validate()
			{
				return dataModuleObj.Validate();
			}
			public bool Save()
			{
				string key = string.Empty;
				bool result = false;
				if (dataModuleObj.InsertOrUpdate(ref key))
				{
					StatusMessage = "Data saved successfully";
                    HasError = true;
				}
				else
				{
					StatusMessage = "No update occurred as the data has not changed";
                    HasError = false;
				}
				if (settings.Mode == "create")
				{
					// figure out the key added into the system
					settings.VariableKey1 = key;
				}
				// build redirect URL
				RedirectURL = dataModuleObj.UpdateUrl();
				return true;
			}
			public void SetTabWithErrorActive()
			{
				Tabs.Values.ElementAt(0).Active = false;
				foreach (TabContentModel m in Tabs.Values)
				{
					if (m.HasErrors)
					{
						m.Active = true;
						return;
					}
				}
			}
			private void PrepareCommonHiddenFields()
			{
				if (!HiddenFields.ContainsKey("hdnMsgRequiredFieldValidation"))
				{
					HiddenFields.Add("hdnMsgRequiredFieldValidation", dataModuleObj.MandatoryErrorMsg);
				}
				if (!HiddenFields.ContainsKey("hdnMsgMinLengthValidation"))
				{
					HiddenFields.Add("hdnMsgMinLengthValidation", dataModuleObj.MinLengthErrorMsg);
				}
				if (!HiddenFields.ContainsKey("hdnMsgMaxLengthValidation"))
				{
					HiddenFields.Add("hdnMsgMaxLengthValidation", dataModuleObj.MaxLengthErrorMsg);
				}
				if (!HiddenFields.ContainsKey("hdnMsgREValidation"))
				{
					HiddenFields.Add("hdnMsgREValidation", dataModuleObj.InvalidFormatErrorMsg);
				}
                if(!HiddenFields.ContainsKey("hdnMsgNumericValidation"))
                {
                    HiddenFields.Add("hdnMsgNumericValidation", dataModuleObj.NumericErrorMsg);
                }
                if(!HiddenFields.ContainsKey("hdnMsgMaxValueValidation"))
                {
                    HiddenFields.Add("hdnMsgMaxValueValidation", dataModuleObj.LessThanErrorMsg);
                }
                if(!HiddenFields.ContainsKey("hdnMsgMinValueValidation"))
                {
                    HiddenFields.Add("hdnMsgMinValueValidation", dataModuleObj.GreaterThanErrorMsg);
                }
			}
		}
	}
}
