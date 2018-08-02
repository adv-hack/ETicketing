using System.Collections.Generic;
using System.Data;
namespace TalentSystemDefaults
{
	namespace DataEntities
	{
		public class ConfigurationEntity
		{
			private string _Id = string.Empty;
			public string Id
			{
				get
				{
					return _Id;
				}
				set
				{
					_Id = value;
				}
			}
			private string _ConfigurationId = string.Empty;
			public string ConfigurationId
			{
				get
				{
					return _ConfigurationId;
				}
				set
				{
					_ConfigurationId = value;
				}
			}
			private string _MasterConfigId = string.Empty;
			public string MasterConfigId
			{
				get
				{
					return _MasterConfigId;
				}
				set
				{
					_MasterConfigId = value;
				}
			}
			private string _NewConfigurationId = string.Empty;
			public string NewConfigurationId
			{
				get
				{
					return _NewConfigurationId;
				}
				set
				{
					_NewConfigurationId = value;
				}
			}
			public string TableName { get; set; }
			public string DefaultKey1
			{
				get
				{
					return defaultKeys[0];
				}
				set
				{
					defaultKeys[0] = value;
				}
			}
			public string DefaultKey2
			{
				get
				{
					return defaultKeys[1];
				}
				set
				{
					defaultKeys[1] = value;
				}
			}
			public string DefaultKey3
			{
				get
				{
					return defaultKeys[2];
				}
				set
				{
					defaultKeys[2] = value;
				}
			}
			public string DefaultKey4
			{
				get
				{
					return defaultKeys[3];
				}
				set
				{
					defaultKeys[3] = value;
				}
			}
			public string VariableKey1
			{
				get
				{
					return variableKeys[0];
				}
				set
				{
					variableKeys[0] = value;
				}
			}
			public string VariableKey2
			{
				get
				{
					return variableKeys[1];
				}
				set
				{
					variableKeys[1] = value;
				}
			}
			public string VariableKey3
			{
				get
				{
					return variableKeys[2];
				}
				set
				{
					variableKeys[2] = value;
				}
			}
			public string VariableKey4
			{
				get
				{
					return variableKeys[3];
				}
				set
				{
					variableKeys[3] = value;
				}
			}
			private string[] defaultKeys { get; set; }
			private string[] variableKeys { get; set; }
			public string DisplayName { get; set; }
			private string _FormattedDisplayName = string.Empty;
			public string FormattedDisplayName
			{
				get
				{
					return _FormattedDisplayName;
				}
				set
				{
					_FormattedDisplayName = value;
				}
			}
			public string DefaultName { get; set; }
			public string DefaultValue { get; set; }
			public string AllowedValues { get; set; }
			public string AllowedPlaceHolders { get; set; }
			public string Description { get; set; }
			public string DataType { get; set; }
			public string DisplayTab { get; set; }
			public List<string> ValidationGroup { get; set; }
			private int? _MinLength = null;
			public int? MinLength
			{
				get
				{
					return _MinLength;
				}
				set
				{
					_MinLength = value;
				}
			}
			private int? _MaxLength = null;
			public int? MaxLength
			{
				get
				{
					return _MaxLength;
				}
				set
				{
					_MaxLength = value;
				}
			}
			public ConfigurationEntity(string tableName, string masterConfigId, string[] defaultKeys, string[] variableKeys, string displayName, string defaultName, string defaultValue, string allowedValues, string allowedPlaceHolders, string description = "", string dataType = "", string displayTab = "")
			{
				this.TableName = tableName;
				this.MasterConfigId = masterConfigId;
				this.defaultKeys = defaultKeys;
				this.variableKeys = variableKeys;
				this.DisplayName = displayName;
				this.DefaultName = defaultName;
				this.DefaultValue = defaultValue;
				this.AllowedValues = allowedValues;
				this.AllowedPlaceHolders = allowedPlaceHolders;
				this.Description = description;
				this.DataType = dataType;
				this.DisplayTab = displayTab;
			}
			private ConfigurationEntity()
			{
			}
			public static ConfigurationEntity GetEntity(DataRow row)
			{
				string Id = Utilities.CheckForDBNull_String(row["ID"]);
				string configId = Utilities.CheckForDBNull_String(row["CONFIG_ID"]);
				string masterConfigId = Utilities.CheckForDBNull_String(row["MASTER_CONFIG"]);
				string tableName = Utilities.CheckForDBNull_String(row["TABLE_NAME"]);
				string defaultKey1 = Utilities.CheckForDBNull_String(row["DEFAULT_KEY1"]);
				string defaultKey2 = Utilities.CheckForDBNull_String(row["DEFAULT_KEY2"]);
				string defaultKey3 = Utilities.CheckForDBNull_String(row["DEFAULT_KEY3"]);
				string defaultKey4 = Utilities.CheckForDBNull_String(row["DEFAULT_KEY4"]);
				string variableKey1 = Utilities.CheckForDBNull_String(row["VARIABLE_KEY1"]);
				string variableKey2 = Utilities.CheckForDBNull_String(row["VARIABLE_KEY2"]);
				string variableKey3 = Utilities.CheckForDBNull_String(row["VARIABLE_KEY3"]);
				string variableKey4 = Utilities.CheckForDBNull_String(row["VARIABLE_KEY4"]);
				string displayName = Utilities.CheckForDBNull_String(row["DISPLAY_NAME"]);
				string defaultName = Utilities.CheckForDBNull_String(row["DEFAULT_NAME"]);
				string defaultValue = Utilities.CheckForDBNull_String(row["DEFAULT_VALUE"]);
				string allowedValues = Utilities.CheckForDBNull_String(row["ALLOWED_VALUE"]);
				string allowedPlaceHolders = Utilities.CheckForDBNull_String(row["ALLOWED_PLACE_HOLDER"]);
				string description = Utilities.CheckForDBNull_String(row["DESCRIPTION"]);
				string[] defaultKeys = new string[] { defaultKey1, defaultKey2, defaultKey3, defaultKey4 };
				string[] variableKeys = new string[] { variableKey1, variableKey2, variableKey3, variableKey4 };
				ConfigurationEntity entity = new ConfigurationEntity()
					{
						Id = Id,
						ConfigurationId = configId,
						MasterConfigId = masterConfigId,
						TableName = tableName,
						defaultKeys = defaultKeys,
						variableKeys = variableKeys,
						DisplayName = displayName,
						DefaultName = defaultName,
						DefaultValue = defaultValue,
						AllowedValues = allowedValues,
						AllowedPlaceHolders = allowedPlaceHolders,
						Description = description
					};
				return entity;
			}
		}
	}
}
