using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using TalentSystemDefaults.DataAccess.ConfigObjects;
using TalentSystemDefaults.DataAccess.DataObjects;
namespace TalentSystemDefaults
{
	public enum DataType
	{
		TEXT,
		TEXTAREA,
		DROPDOWN,
		BOOL,
		BOOL_YN,
		BOOL_10,
		LABEL,
		HIDDEN
	}
	public enum HiddenType
	{
		FOREIGN,
		@DEFAULT,
		SETTING
	}
	public enum PropertyName
	{
		BUSINESSUNIT,
		AGENTNAME,
		STOREDPROC,
		ISERIESTODAYSDATE
	}
	public abstract class DMBase
	{
		private const string VARIABLE_KEY1_KEY = "variablekey1";
		private const string VARIABLE_KEY2_KEY = "variablekey2";
		private const string VARIABLE_KEY3_KEY = "variablekey3";
		private const string VARIABLE_KEY4_KEY = "variablekey4";
		private const string MODE_KEY = "mode";
		protected const string MODE_CREATE = "create";
		protected const string MODE_UPDATE = "update";
		const string FROM_COLUMN = "FromColumn";
		const string TO_COLUMN = "ToColumn";
		const string RELATION_TYPE = "RelationType";
		const string PROPERTY_NAME = "PropertyName";
		public ModuleConfigurations MConfigs { get; set; }
		private tbl_config_detail tbl_config_detail;
		private tbl_table_detail tbl_table_detail;
		protected DESettings settings;
		protected string variableKey1;
		protected string variableKey2;
		protected string variableKey3;
		protected string variableKey4;
		protected string mode;
		public string DefaultTabType; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
		protected DataTable dtHiddenFields;
		protected Dictionary<string, string> pageTexts;
		public const string MandatoryErrorMsgKey = "MandatoryErrorMsg";
		public const string NumericErrorMsgKey = "NumericErrorMsg";
		public const string MinLengthErrorMsgKey = "MinLengthErrorMsg";
		public const string MaxLengthErrorMsgKey = "MaxLengthErrorMsg";
		public const string InvalidFormatErrorMsgKey = "InvalidFormatErrorMsg";
		public const string LessThanErrorMsgKey = "LessThanErrorMsg";
		public const string GreaterThanErrorMsgKey = "GreaterThanErrorMsg";
		public DisplayTabs DisplayTabs;
		public AccordionGroup AccordionGroup;
		public string MandatoryErrorMsg
		{
			get
			{
				return ((pageTexts.ContainsKey(MandatoryErrorMsgKey)) ? (pageTexts[MandatoryErrorMsgKey]) : "{0} is required");
			}
		}
		public string NumericErrorMsg
		{
			get
			{
				return ((pageTexts.ContainsKey(NumericErrorMsgKey)) ? (pageTexts[NumericErrorMsgKey]) : "{0} must have a numeric value");
			}
		}
		public string MinLengthErrorMsg
		{
			get
			{
				return ((pageTexts.ContainsKey(MinLengthErrorMsgKey)) ? (pageTexts[MinLengthErrorMsgKey]) : "{0} requires minimum {1} characters");
			}
		}
		public string MaxLengthErrorMsg
		{
			get
			{
				return ((pageTexts.ContainsKey(MaxLengthErrorMsgKey)) ? (pageTexts[MaxLengthErrorMsgKey]) : "{0} exceeds the maximum {1} characters");
			}
		}
		public string InvalidFormatErrorMsg
		{
			get
			{
				return ((pageTexts.ContainsKey(InvalidFormatErrorMsgKey)) ? (pageTexts[InvalidFormatErrorMsgKey]) : "{0} has invalid format");
			}
		}
		public string LessThanErrorMsg
		{
			get
			{
				return ((pageTexts.ContainsKey(LessThanErrorMsgKey)) ? (pageTexts[LessThanErrorMsgKey]) : "The value must be less than {0}");
			}
		}
		public string GreaterThanErrorMsg
		{
			get
			{
				return ((pageTexts.ContainsKey(GreaterThanErrorMsgKey)) ? (pageTexts[GreaterThanErrorMsgKey]) : "The value must be greater than {0}");
			}
		}
		// New Method to add row in data table, from and to GUID and Type (Foreign)
		public DMBase(ref DESettings settings, bool initialiseData)
		{
			// VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
			DefaultTabType = DisplayTabsType.HORIZONTAL.ToString();
			this.settings = settings;
			dtHiddenFields = new DataTable();
			dtHiddenFields.Columns.Add(new System.Data.DataColumn(FROM_COLUMN));
			dtHiddenFields.Columns.Add(new System.Data.DataColumn(TO_COLUMN));
			dtHiddenFields.Columns.Add(new System.Data.DataColumn(RELATION_TYPE));
			dtHiddenFields.Columns.Add(new System.Data.DataColumn(PROPERTY_NAME));
			SetHiddenFields();
			MConfigs = new ModuleConfigurations();
			tbl_config_detail = new tbl_config_detail(settings);
			tbl_table_detail = new tbl_table_detail(settings);
			SetValues();
			tbl_page_text_lang textLang = new tbl_page_text_lang(ref settings);
			pageTexts = textLang.GetTextsForPage(settings.DefaultBusinessUnit, "SystemDefaults.aspx");
			DisplayTabs = new DisplayTabs(pageTexts);
			settings.PageTexts = pageTexts;
			AccordionGroup = new AccordionGroup(pageTexts);
			if (initialiseData)
			{
				SetModuleConfiguration();
			}
		}
		public virtual void SetHiddenFields()
		{
		}
		private void SetValues()
		{
			if (settings.VariableKey1 != string.Empty)
			{
				variableKey1 = settings.VariableKey1;
			}
			if (settings.VariableKey2 != string.Empty)
			{
				variableKey2 = settings.VariableKey2;
			}
			if (settings.VariableKey3 != string.Empty)
			{
				variableKey3 = settings.VariableKey3;
			}
			if (settings.VariableKey4 != string.Empty)
			{
				variableKey4 = settings.VariableKey4;
			}
			if (settings.Mode != string.Empty)
			{
				mode = settings.Mode;
			}
		}
		public abstract void SetModuleConfiguration();
		public void Populate()
		{
			System.Collections.Generic.List<string> ConfigIDs = new System.Collections.Generic.List<string>();
			foreach (ModuleConfiguration mConfig in MConfigs)
			{
				ConfigIDs.Add(mConfig.ConfigID);
			}
			var configItems = tbl_config_detail.RetrieveConfigurationItems(ConfigIDs);
			foreach (ModuleConfiguration mConfig in MConfigs)
			{
				if (configItems.ContainsKey(mConfig.ConfigID))
				{
					mConfig.ConfigurationItem = configItems[mConfig.ConfigID];
				}
			}
			MConfigs.RemoveAll(item => item.ConfigurationItem == null);
			GetCurrentValueForConfig();
		}
		public void GetCurrentValueForConfig()
		{
			string className = null;
			object[] parameters = null;
			DBObjectBase dbBase = null;
			foreach (string tableName in MConfigs.Select(x => x.ConfigurationItem.TableName).Distinct())
			{
				className = Utilities.GetClassName(GlobalConstants.NS_DATAACCESS_DATAOBJECTS, tableName);
				parameters = new[] { settings };
				try
				{
					dbBase = ReflectionUtils.CreateInstance(className, parameters);
					if (dbBase != null)
					{
						System.Collections.Generic.List<ConfigurationItem> configs = GetConfigurationItems(MConfigs, tableName);
						dbBase.RetrieveCurrentValues(configs);
					}
				}
				catch (Exception ex)
				{
					Utilities.InsertErrorLog(settings, this.GetType().BaseType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, GlobalConstants.LOG_ERROR_CODE, settings.BusinessUnit, "", "", "", ex.StackTrace.ToString(), ex.Message);
				}
			}
		}
		public bool InsertOrUpdate(ref string key)
		{
			ProcessHiddenFields(MConfigs);
			int affectedRows = 0;
			if (mode == MODE_CREATE)
			{
				affectedRows = System.Convert.ToInt32(Insert(ref key));
			}
			else if (mode == MODE_UPDATE)
			{
				affectedRows = Update();
			}
			return affectedRows > 0 ? true : false;
		}
		private void ProcessHiddenFields(ModuleConfigurations MConfigs)
		{
			ModuleConfiguration objModuleConfig = null;
			foreach (DataRow row in dtHiddenFields.Rows)
			{
				objModuleConfig = MConfigs.Find(x => x.ConfigID == (string)row[FROM_COLUMN]);
				if (objModuleConfig != null)
				{
					if ((string)row[RELATION_TYPE] == HiddenType.FOREIGN.ToString())
					{
						ModuleConfiguration toConfig = MConfigs.Where(c => c.ConfigID == (string)row[TO_COLUMN]).FirstOrDefault();
						objModuleConfig.ConfigurationItem.UpdatedValue = toConfig.ConfigurationItem.UpdatedValue;
					}
					else if ((string)row[RELATION_TYPE] == HiddenType.DEFAULT.ToString())
					{
						// leaving MConfigs object as it is would work in this case
						ModuleConfiguration DefaultConfig = MConfigs.Where(c => c.ConfigID == (string)row[FROM_COLUMN]).FirstOrDefault();
						objModuleConfig.ConfigurationItem.UpdatedValue = DefaultConfig.ConfigurationItem.DefaultValue;
					}
					else if ((string)row[RELATION_TYPE] == HiddenType.SETTING.ToString())
					{
						//Dim toConfig As ModuleConfiguration = MConfigs.Where(Function(c) c.ConfigID = row(TO_COLUMN)).FirstOrDefault
						//settings.GetPropertyValue("BUSINESSUNIT")
						objModuleConfig.ConfigurationItem.UpdatedValue = settings.GetPropertyValue(System.Convert.ToString(row[PROPERTY_NAME]));
					}
				}
			}
		}
		public int Update()
		{
			ModuleConfigurations updatedMConfig = GetUpdatedMConfigs(MConfigs);
			return UpdateCurrentValueForConfig(updatedMConfig);
		}
		private bool Insert(ref string key)
		{
			return System.Convert.ToBoolean(InsertCurrentValueForConfig(ref key));
		}
		private int UpdateCurrentValueForConfig(ModuleConfigurations objMConfigs)
		{
			int affectedRows = 0;
			object[] parameters = null;
			DBObjectBase dbBase = null;
			foreach (string tableName in objMConfigs.Select(x => x.ConfigurationItem.TableName).Distinct())
			{
				string className = Utilities.GetClassName(GlobalConstants.NS_DATAACCESS_DATAOBJECTS, tableName);
				parameters = new[] { settings };
				dbBase = ReflectionUtils.CreateInstance(className, parameters);
				System.Collections.Generic.List<ConfigurationItem> configs = GetConfigurationItems(objMConfigs, tableName);
				if (dbBase.DBTypeForTable == DBObjectBase.DBType.SQLSERVER)
				{
					affectedRows += dbBase.UpdateCurrentValue(configs);
				}
				else if (dbBase.DBTypeForTable == DBObjectBase.DBType.DB2)
				{
					affectedRows += dbBase.UpdateCurrentValueDB2(configs);
				}
			}
			affectedRows += settings.DB2AccessQueue.Execute(settings);
			return affectedRows;
		}
		private int InsertCurrentValueForConfig(ref string key)
		{
			int affectedRows = 0;
			object[] parameters = null;
			DBObjectBase dbBase = null;
			foreach (string tableName in MConfigs.Select(x => x.ConfigurationItem.TableName).Distinct())
			{
				string className = Utilities.GetClassName(GlobalConstants.NS_DATAACCESS_DATAOBJECTS, tableName);
				parameters = new[] { settings };
				dbBase = ReflectionUtils.CreateInstance(className, parameters);
				System.Collections.Generic.List<ConfigurationItem> configs = GetConfigurationItems(MConfigs, tableName);
				if (dbBase.DBTypeForTable == DBObjectBase.DBType.SQLSERVER)
				{
					affectedRows += dbBase.InsertCurrentValue(configs, null, ref key);
				}
				else if (dbBase.DBTypeForTable == DBObjectBase.DBType.DB2)
				{
					affectedRows += dbBase.InsertCurrentValueDB2(configs, null);
				}
			}
			affectedRows += settings.DB2AccessQueue.Execute(settings);
			return affectedRows;
		}
		private List<ConfigurationItem> GetConfigurationItems(ModuleConfigurations objMConfigs, string tableName)
		{
			var configs = new System.Collections.Generic.List<ConfigurationItem>();
			foreach (ModuleConfiguration mConfig in objMConfigs.Where(x => x.ConfigurationItem.TableName == tableName))
			{
				configs.Add(mConfig.ConfigurationItem);
			}
			return configs;
		}
		private ModuleConfigurations GetUpdatedMConfigs(ModuleConfigurations MConfigs)
		{
			ModuleConfigurations updatedMConfig = new ModuleConfigurations();
			foreach (ModuleConfiguration config in MConfigs)
			{
				bool isEqual = string.Equals(config.ConfigurationItem.CurrentValue.Trim(), config.ConfigurationItem.UpdatedValue.Trim(), StringComparison.CurrentCultureIgnoreCase);
				if (!isEqual)
				{
					updatedMConfig.Add(config);
				}
			}
			return updatedMConfig;
		}
		public virtual string DeleteUrl()
		{
			return string.Empty;
		}
		public virtual string UpdateUrl()
		{
			return string.Empty;
		}
		public virtual string AddUrl()
		{
			return string.Empty;
		}
		public virtual string BackUrl()
		{
			return string.Format("DefaultList.aspx?listname={0}&businessUnit={1}", "Modules", settings.BusinessUnit);
		}
		public virtual bool Validate()
		{
			bool result = true;
			foreach (ModuleConfiguration mConfig in MConfigs)
			{
				var objMConfig = mConfig;
				result = result & Validate(ref objMConfig);
			}
			return result;
		}
		private bool Validate(ref ModuleConfiguration mConfig)
		{
			mConfig.ErrorMessage = string.Empty;
			if (mConfig.ValidationGroup != null)
			{
				if (IsValid(mConfig) && mConfig.ValidationGroup.HasMandatory)
				{
					ValidateMandatory(ref mConfig);
				}

				if (mConfig.ConfigurationItem.UpdatedValue.Trim() != string.Empty && (mConfig.FieldType == DataType.TEXT || mConfig.FieldType == DataType.TEXTAREA))
				{
					// If Regular Expression is ACTIVE, don`t check other validations
					if (IsValid(mConfig) && mConfig.ValidationGroup.HasRegularExp)
					{
						ValidateRegularExp(ref mConfig);
					}
					else
					{
						// if Numeric is active,then check only min/max value
						if (IsValid(mConfig) && mConfig.ValidationGroup.HasNumeric)
						{
							ValidateNumeric(ref mConfig);

							if (IsValid(mConfig) && mConfig.ValidationGroup.HasMinValue)
							{
								ValidateMinValue(ref mConfig);
							}
							if (IsValid(mConfig) && mConfig.ValidationGroup.HasMaxValue)
							{
								ValidateMaxValue(ref mConfig);
							}
						}
						else if (IsValid(mConfig))
						{
							if (IsValid(mConfig) && mConfig.ValidationGroup.HasMaxLength)
							{
								ValidateMaxLength(ref mConfig);
							}

							if (IsValid(mConfig) && mConfig.ValidationGroup.HasMinLength)
							{
								ValidateMinLength(ref mConfig);
							}
						}
					}
				}
			}
			return mConfig.ErrorMessage == string.Empty;
		}
		private void ValidateMandatory(ref ModuleConfiguration mConfig)
		{
			var value = mConfig.ConfigurationItem.UpdatedValue.ToLower();
			switch (mConfig.FieldType)
			{
				case DataType.BOOL:
				case DataType.BOOL_10:
				case DataType.BOOL_YN:
					if (value == "false" || value == "0" || value == "N")
					{
						mConfig.ErrorMessage = string.Format(MandatoryErrorMsg, mConfig.ConfigurationItem.DisplayName);
					}
					break;
				default:
					if (mConfig.ConfigurationItem.UpdatedValue == string.Empty)
					{
						mConfig.ErrorMessage = string.Format(MandatoryErrorMsg, mConfig.ConfigurationItem.DisplayName);
					}
					break;
			}
		}
		public void ValidateNumeric(ref ModuleConfiguration mConfig)
		{
			string value = mConfig.ConfigurationItem.UpdatedValue;
			if (!Information.IsNumeric(value))
			{
				mConfig.ErrorMessage = string.Format(NumericErrorMsg, mConfig.ConfigurationItem.DisplayName);
			}
		}
		private void ValidateMinLength(ref ModuleConfiguration mConfig)
		{
			int minLength = mConfig.ValidationGroup.MinLength;
			if (mConfig.ConfigurationItem.UpdatedValue.Trim().Length < minLength)
			{
				mConfig.ErrorMessage = string.Format(MinLengthErrorMsg, mConfig.ConfigurationItem.DisplayName, minLength);
			}
		}
		private void ValidateMaxLength(ref ModuleConfiguration mConfig)
		{
			int maxLength = mConfig.ValidationGroup.MaxLength;
			if (mConfig.ConfigurationItem.UpdatedValue.Trim().Length > maxLength)
			{
				mConfig.ErrorMessage = string.Format(MaxLengthErrorMsg, mConfig.ConfigurationItem.DisplayName, maxLength);
			}
		}
		private void ValidateMinValue(ref ModuleConfiguration mConfig)
		{
			string value = mConfig.ConfigurationItem.UpdatedValue;
			int minVal = mConfig.ValidationGroup.MinValue;
			if (!string.IsNullOrEmpty(value))
			{
				var intValue = System.Convert.ToInt32(value);
				if (intValue < minVal)
				{
					mConfig.ErrorMessage = string.Format(GreaterThanErrorMsg, minVal);
				}
			}
		}
		private void ValidateMaxValue(ref ModuleConfiguration mConfig)
		{
			string value = mConfig.ConfigurationItem.UpdatedValue;
			int maxVal = mConfig.ValidationGroup.MaxValue;
			if (!string.IsNullOrEmpty(value))
			{
				var intValue = System.Convert.ToInt32(value);
				if (intValue > maxVal)
				{
					mConfig.ErrorMessage = string.Format(LessThanErrorMsg, maxVal);
				}
			}
		}
		private void ValidateRegularExp(ref ModuleConfiguration mConfig)
		{
			Regex regEx = new Regex(mConfig.ValidationGroup.RegularExp);
			if (!regEx.IsMatch(mConfig.ConfigurationItem.UpdatedValue))
			{
				mConfig.ErrorMessage = string.Format(InvalidFormatErrorMsg, mConfig.ConfigurationItem.DisplayName);
			}
		}
		private dynamic IsValid(dynamic mConfig)
		{
			return mConfig.ErrorMessage == string.Empty;
		}
	}
}
