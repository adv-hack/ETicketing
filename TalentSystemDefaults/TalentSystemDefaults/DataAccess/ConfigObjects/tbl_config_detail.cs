using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
//using TalentSystemDefaults.DBObjectBase;
using System.Text.RegularExpressions;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	namespace DataAccess.ConfigObjects
	{
		public class tbl_config_detail
		{
			#region Class Level Fields
			private DESettings settings;
			private GUIDGenerator guidGenerator = new GUIDGenerator();
			const string VALUE_ALL = "*ALL";
			#endregion
			#region Private Methods
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="dt"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private Dictionary<string, ConfigurationItem> GetConfigurationItems(DataTable dt)
			{
				var results = new Dictionary<string, ConfigurationItem>();
				foreach (DataRow row in dt.Rows)
				{
					try
					{
						var id = Utilities.CheckForDBNull_String(row["ID"]);
						var configID = Utilities.CheckForDBNull_String(row["CONFIG_ID"]);
						ConfigurationItem config = new ConfigurationItem();
						config.DisplayName = Utilities.CheckForDBNull_String(row["DISPLAY_NAME"]);
						config.DefaultName = Utilities.CheckForDBNull_String(row["DEFAULT_NAME"]);
						config.TableName = Utilities.CheckForDBNull_String(row["TABLE_NAME"]);
						config.DefaultKey1 = Utilities.CheckForDBNull_String(row["DEFAULT_KEY1"]);
						config.DefaultKey2 = Utilities.CheckForDBNull_String(row["DEFAULT_KEY2"]);
						config.DefaultKey3 = Utilities.CheckForDBNull_String(row["DEFAULT_KEY3"]);
						config.DefaultKey4 = Utilities.CheckForDBNull_String(row["DEFAULT_KEY4"]);
						config.VariableKey1 = Utilities.CheckForDBNull_String(row["VARIABLE_KEY1"]);
						config.VariableKey2 = Utilities.CheckForDBNull_String(row["VARIABLE_KEY2"]);
						config.VariableKey3 = Utilities.CheckForDBNull_String(row["VARIABLE_KEY3"]);
						config.VariableKey4 = Utilities.CheckForDBNull_String(row["VARIABLE_KEY4"]);
						config.AllowedValues = GetAllowedValues(row);
						config.AllowedPlaceHolders = Utilities.CheckForDBNull_String(row["ALLOWED_PLACE_HOLDER"]);
						config.Description = Utilities.CheckForDBNull_String(row["DESCRIPTION"]);
						config.DefaultValue = Utilities.CheckForDBNull_String(row["DEFAULT_VALUE"]);
						config.CurrentValue = config.DefaultValue;
						config.UpdatedValue = config.DefaultValue;
						if (config.VariableKey1 == "*ALL")
						{
							UpdateConfigItem(configID, config);
						}
						config.GuidValue = guidGenerator.GenerateGUIDString(System.Convert.ToInt32(id), ConfigurationEntity.GetEntity(row));
						var prevConfigId = configID.Substring(0, configID.LastIndexOf("-"));
						var newConfigId = config.GuidValue.Substring(0, config.GuidValue.LastIndexOf("-"));
						if (prevConfigId == newConfigId)
						{
							results.Add(configID, config);
						}
					}
					catch (InvalidConfigurationException)
					{
						//NOTE: Do nothing so it will skip this configuration and proceed with next.
					}
				}
				return results;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="row"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private List<string> GetAllowedValues(DataRow row)
			{
				string allowValues = Utilities.CheckForDBNull_String(row["ALLOWED_VALUE"]);
				if (!string.IsNullOrWhiteSpace(allowValues))
				{
					string[] lstAllowValues = allowValues.Split("|".ToCharArray());
					return lstAllowValues.ToList();
				}
				return default(List<string>);
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="masterConfigId"></param>
			/// <param name="config"></param>
			/// <remarks></remarks>
			private void UpdateConfigItem(string masterConfigId, ConfigurationItem config)
			{
				if (!string.IsNullOrWhiteSpace(settings.VariableKey1))
				{
					var variableKey1 = settings.VariableKey1;
					var dt = RetrieveConfigurationItem(masterConfigId, variableKey1);
					if (dt.Rows.Count > 0)
					{
						DataRow row = dt.Rows[0];
						var generatedGuid = guidGenerator.GenerateGUIDString(System.Convert.ToInt32(row["ID"]), ConfigurationEntity.GetEntity(row));
						var configID = Utilities.CheckForDBNull_String(row["CONFIG_ID"]);
						if (CompareGUIDVaues(masterConfigId, configID))
						{
							string value = Utilities.CheckForDBNull_String(row["DEFAULT_VALUE"]);
							config.DefaultValue = value;
							config.CurrentValue = value;
							config.UpdatedValue = value;
							config.DisplayName = Utilities.CheckForDBNull_String(row["DISPLAY_NAME"]);
							config.AllowedValues = GetAllowedValues(row);
							config.AllowedPlaceHolders = Utilities.CheckForDBNull_String(row["ALLOWED_PLACE_HOLDER"]);
							config.Description = Utilities.CheckForDBNull_String(row["DESCRIPTION"]);
						}
						else
						{
							throw (new InvalidConfigurationException("Invalid GUID"));
						}
					}
				}
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="masterConfig"></param>
			/// <param name="config"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private bool CompareGUIDVaues(string masterConfig, string config)
			{
				string[] masterConfigs = masterConfig.Split("-".ToCharArray());
				string[] configs = config.Split("-".ToCharArray());
				if (masterConfigs[0] == configs[0] && masterConfigs[1] == configs[1])
				{
					return true;
				}
				return false;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private int IncrementUniqueId(SqlTransaction givenTransaction)
			{
				string commandText = "UPDATE [tbl_defaults] SET UniqueId = UniqueId + 1";
				int affectedRows = 0;
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				ErrorObj err = new ErrorObj();
				//Construct The Call
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
				talentSqlAccessDetail.CommandElements.CommandText = commandText;
				//Execute
				if (givenTransaction == null)
				{
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG);
				}
				else
				{
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG, givenTransaction);
				}
				if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
				{
					affectedRows = System.Convert.ToInt32(talentSqlAccessDetail.ResultDataSet.Tables[0].Rows[0][0]);
				}
				talentSqlAccessDetail = null;
				//Return results
				return affectedRows;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="tableName"></param>
			/// <param name="defaultNames"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private string[] GetMasterConfigurations(string tableName, string[] defaultNames)
			{
				string[] whereClause = new string[defaultNames.Count() - 1 + 1];
				for (int i = 0; i <= defaultNames.Count() - 1; i++)
				{
					whereClause[i] = "default_name = @DEFAULT_NAME" + i.ToString();
				}
				string where = string.Join(" OR ", whereClause);
				string commandText = string.Format("SELECT * FROM tbl_config_detail WHERE table_name=@TABLE_NAME AND variable_key1=@VARIABLE_KEY1 AND variable_key2=@VARIABLE_KEY2 AND variable_key3=@VARIABLE_KEY3 AND variable_key4=@VARIABLE_KEY4 AND ({0})", where);
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@TABLE_NAME", tableName));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@VARIABLE_KEY1", settings.VariableKey1));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@VARIABLE_KEY2", settings.VariableKey2));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@VARIABLE_KEY3", settings.VariableKey3));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@VARIABLE_KEY4", settings.VariableKey4));
					for (int i = 0; i <= defaultNames.Count() - 1; i++)
					{
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@DEFAULT_NAME" + i.ToString(), defaultNames[i]));
					}
					ErrorObj err = new ErrorObj();
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG);
					if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
					{
						outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables[0];
					}
				}
				catch (Exception)
				{
					throw;
				}
				finally
				{
					talentSqlAccessDetail = null;
				}
				System.Collections.Generic.List<string> masterConfigIds = new System.Collections.Generic.List<string>();
				string masterConfigId = string.Empty;
				if (outputDataTable != null)
				{
					foreach (DataRow row in outputDataTable.Rows)
					{
						masterConfigId = Utilities.CheckForDBNull_String(row["config_id"]);
						masterConfigIds.Add(masterConfigId);
					}
				}
				return masterConfigIds.ToArray();
			}
			#endregion
			#region Public Methods
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="settings"></param>
			/// <remarks></remarks>
			public tbl_config_detail(DESettings settings)
			{
				this.settings = settings;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="ids"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public Dictionary<string, ConfigurationItem> RetrieveConfigurationItems(System.Collections.Generic.List<string> ids)
			{
				string[] whereClause = new string[ids.Count - 1 + 1];
				for (int i = 0; i <= ids.Count - 1; i++)
				{
					//whereClause(i) = "Config_ID = @configId" & i.ToString & " OR Master_Config_ID = @masterConfigId" & i.ToString
					whereClause[i] = "Config_ID = @param" + i.ToString();
				}
				string results = string.Join(" OR ", whereClause);
				string commandText = string.Format("SELECT * FROM tbl_config_detail WHERE ({0})", results);
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					for (int i = 0; i <= ids.Count - 1; i++)
					{
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@param" + i.ToString(), ids[i]));
					}
					ErrorObj err = new ErrorObj();
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG);
					if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
					{
						outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables[0];
					}
				}
				catch (Exception)
				{
					throw;
				}
				finally
				{
					talentSqlAccessDetail = null;
				}
				return GetConfigurationItems(outputDataTable);
			}
			public ConfigurationEntity[] RetrieveConfigurationEntities(System.Collections.Generic.List<string> ids)
			{
				string[] whereClause = new string[ids.Count - 1 + 1];
				for (int i = 0; i <= ids.Count - 1; i++)
				{
					//whereClause(i) = "Config_ID = @configId" & i.ToString & " OR Master_Config_ID = @masterConfigId" & i.ToString
					whereClause[i] = "Config_ID = @param" + i.ToString();
				}
				string results = string.Join(" OR ", whereClause);
				string commandText = string.Format("SELECT * FROM tbl_config_detail WHERE ({0})", results);
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					for (int i = 0; i <= ids.Count - 1; i++)
					{
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@param" + i.ToString(), ids[i]));
					}
					ErrorObj err = new ErrorObj();
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG);
					if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
					{
						outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables[0];
					}
				}
				catch (Exception)
				{
					throw;
				}
				finally
				{
					talentSqlAccessDetail = null;
				}
				return GetConfigurationEntities(outputDataTable);
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="masterConfig"></param>
			/// <param name="variableKey1"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public DataTable RetrieveConfigurationItem(string masterConfig, string variableKey1)
			{
				string commandText = "SELECT * FROM tbl_config_detail WHERE master_config = @MASTER_CONFIG AND variable_key1 = @VARIABLE_KEY1";
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@MASTER_CONFIG", masterConfig));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@VARIABLE_KEY1", variableKey1));
					ErrorObj err = new ErrorObj();
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG);
					if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
					{
						outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables[0];
					}
				}
				catch (Exception)
				{
					throw;
				}
				finally
				{
					talentSqlAccessDetail = null;
				}
				return outputDataTable;
			}
			//Verify Function and selectedTable through reflection find BaseVersion is enabled, if yes see if *ALL exists, if no return False and display Error
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="tableName"></param>
			/// <param name="defaultNames"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public string[] GetMasterConfigIds(string tableName, string[] defaultNames)
			{
				if (bool.Parse(DMFactory.IsBaseDefinitionEnabled(tableName)))
				{
					Dictionary<string, string> variableKeyValues = DMFactory.GetVariableKeyValues(tableName);
					string masterVK1 = string.Empty;
					string masterVK2 = string.Empty;
					string masterVK3 = string.Empty;
					string masterVK4 = string.Empty;
					foreach (var key in variableKeyValues.Keys)
					{
						bool value = System.Convert.ToBoolean(variableKeyValues[key]);
						if ((string)key == "VariableKey1Active")
						{
							if (value)
							{
								masterVK1 = VALUE_ALL;
							}
						}
						else if ((string)key == "VariableKey2Active")
						{
							if (value)
							{
								masterVK2 = VALUE_ALL;
							}
						}
						else if ((string)key == "VariableKey3Active")
						{
							if (value)
							{
								masterVK3 = VALUE_ALL;
							}
						}
						else if ((string)key == "VariableKey4Active")
						{
							if (value)
							{
								masterVK4 = VALUE_ALL;
							}
						}
					}
					return GetMasterConfigurations(tableName, defaultNames);
				}
				return null;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="entities"></param>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public bool SaveAll(ConfigurationEntity[] entities, string moduleName, SqlTransaction givenTransaction = null)
			{
				bool result = true;
				foreach (var entity in entities)
				{
					entity.ConfigurationId = guidGenerator.GenerateGUIDString(GetUniqueId(givenTransaction), entity);
					//entity.MasterConfigId =
					// call the function to fill the master config id
					// if tbl_payment_type, BaseVersion, how many default keys are active, 2, GUID for *ALL
					result = result && SaveEntity(entity, moduleName, givenTransaction);
				}
				return result;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="entity"></param>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public bool SaveEntity(ConfigurationEntity entity, string moduleName, SqlTransaction givenTransaction)
			{
				bool result = false;
				string commandText = "INSERT INTO [tbl_config_detail] ([CONFIG_ID],[TABLE_NAME], [MASTER_CONFIG], [DEFAULT_KEY1],[DEFAULT_KEY2], [DEFAULT_KEY3], [DEFAULT_KEY4], [VARIABLE_KEY1], [VARIABLE_KEY2], [VARIABLE_KEY3], [VARIABLE_KEY4], [DISPLAY_NAME], [DEFAULT_NAME],[DEFAULT_VALUE], [ALLOWED_VALUE], [ALLOWED_PLACE_HOLDER], [Description], [MODULE_NAME], [CREATED_TIMESTAMP])" +
					" VALUES (@CONFIG_ID, @TABLE_NAME, @MASTER_CONFIG, @DEFAULT_KEY1, @DEFAULT_KEY2, @DEFAULT_KEY3, @DEFAULT_KEY4, @VARIABLE_KEY1, @VARIABLE_KEY2, @VARIABLE_KEY3, @VARIABLE_KEY4, @DISPLAY_NAME, @DEFAULT_NAME, @DEFAULT_VALUE, @ALLOWED_VALUE, @ALLOWED_PLACE_HOLDER, @Description, @ModuleName, GETDATE())";
				int affectedRows = 0;
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				ErrorObj err = new ErrorObj();
				//Construct The Call
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
				talentSqlAccessDetail.CommandElements.CommandText = commandText;
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@CONFIG_ID", entity.ConfigurationId));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@TABLE_NAME", entity.TableName));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@MASTER_CONFIG", entity.MasterConfigId));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@DEFAULT_KEY1", entity.DefaultKey1));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@DEFAULT_KEY2", entity.DefaultKey2));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@DEFAULT_KEY3", entity.DefaultKey3));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@DEFAULT_KEY4", entity.DefaultKey4));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@VARIABLE_KEY1", entity.VariableKey1));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@VARIABLE_KEY2", entity.VariableKey2));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@VARIABLE_KEY3", entity.VariableKey3));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@VARIABLE_KEY4", entity.VariableKey4));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@DISPLAY_NAME", entity.DisplayName));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@DEFAULT_NAME", entity.DefaultName));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@DEFAULT_VALUE", entity.DefaultValue));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@ALLOWED_VALUE", entity.AllowedValues));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@ALLOWED_PLACE_HOLDER", entity.AllowedPlaceHolders));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@Description", entity.Description));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@ModuleName", moduleName));
				//Execute
				if (givenTransaction == null)
				{
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG);
				}
				else
				{
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG, givenTransaction);
				}
				if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
				{
					affectedRows = System.Convert.ToInt32(talentSqlAccessDetail.ResultDataSet.Tables[0].Rows[0][0]);
				}
				talentSqlAccessDetail = null;
				if (affectedRows > 0)
				{
					affectedRows = IncrementUniqueId(givenTransaction);
					if (affectedRows > 0)
					{
						return true;
					}
				}
				return false;
			}
			public bool UpdateAll(ConfigurationEntity[] entities, string moduleName)
			{
				bool result = true;
				foreach (var entity in entities)
				{
					result = result && UpdateEntity(entity, moduleName);
				}
				return result;
			}
			public bool UpdateEntity(ConfigurationEntity entity, string moduleName)
			{
				bool result = false;
				GUIDGenerator guidGenerator = new GUIDGenerator();
				entity.NewConfigurationId = guidGenerator.GenerateGUIDString(guidGenerator.ExtractUniqueId(entity.ConfigurationId), entity);
				string commandText = "UPDATE [tbl_config_detail] SET CONFIG_ID= @NEW_CONFIG_ID, DISPLAY_NAME = @DISPLAY_NAME, DEFAULT_VALUE = @DEFAULT_VALUE, ALLOWED_VALUE = @ALLOWED_VALUE, ALLOWED_PLACE_HOLDER = @ALLOWED_PLACE_HOLDER, Description = @Description, MODULE_NAME=@MODULE_NAME, LASTMODIFIED_TIMESTAMP=GETDATE() WHERE CONFIG_ID=@CONFIG_ID";
				int affectedRows = 0;
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				ErrorObj err = new ErrorObj();
				//Construct The Call
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
				talentSqlAccessDetail.CommandElements.CommandText = commandText;
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@NEW_CONFIG_ID", entity.NewConfigurationId));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@DISPLAY_NAME", entity.DisplayName));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@DEFAULT_VALUE", entity.DefaultValue));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@ALLOWED_VALUE", entity.AllowedValues));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@ALLOWED_PLACE_HOLDER", entity.AllowedPlaceHolders));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@Description", entity.Description));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@CONFIG_ID", entity.ConfigurationId));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@MODULE_NAME", moduleName));
				//Execute
				err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG);
				if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
				{
					affectedRows = System.Convert.ToInt32(talentSqlAccessDetail.ResultDataSet.Tables[0].Rows[0][0]);
				}
				talentSqlAccessDetail = null;
				if (affectedRows > 0)
				{
					if (entity.ConfigurationId == entity.NewConfigurationId)
					{
						entity.NewConfigurationId = string.Empty;
					}
					return true;
				}
				return false;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public int GetUniqueId(SqlTransaction givenTransaction)
			{
				int result = 0;
				string commandText = "SELECT UniqueId FROM [tbl_defaults]";
				int uniqueId = 0;
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				ErrorObj err = new ErrorObj();
				//Construct The Call
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
				talentSqlAccessDetail.CommandElements.CommandText = commandText;
				//Execute
				if (givenTransaction == null)
				{
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG);
				}
				else
				{
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG, givenTransaction);
				}
				if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
				{
					uniqueId = System.Convert.ToInt32(talentSqlAccessDetail.ResultDataSet.Tables[0].Rows[0][0]);
				}
				talentSqlAccessDetail = null;
				return uniqueId;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="oldGUID"></param>
			/// <param name="newGUID"></param>
			/// <remarks></remarks>
			public string UpdateConfigId(string configId, SqlTransaction givenTransaction = null)
			{
				TalentDataAccess talentAccessDetail = new TalentDataAccess();
				System.Collections.Generic.List<ConfigurationItem> configs = new System.Collections.Generic.List<ConfigurationItem>();
				int affectedRows = 0;
				ErrorObj err = new ErrorObj();
				//Dim con As New SqlConnection
				string commandText = "UPDATE tbl_config_detail SET CONFIG_ID=@NEW_CONFIG_ID WHERE CONFIG_ID=@CONFIG_ID";
				//Dim command As New SqlCommand(query, con)
				talentAccessDetail.Settings = settings;
				talentAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
				talentAccessDetail.CommandElements.CommandText = commandText;
				ConfigurationEntity configEntity = GetConfigurationEntity(configId);
				if (configEntity == null)
				{
					return null;
				}
				var newConfigId = guidGenerator.GenerateGUIDString(GetUniqueId(givenTransaction), configEntity);
				talentAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@NEW_CONFIG_ID", newConfigId));
				talentAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@CONFIG_ID", configId));
				if (givenTransaction == null)
				{
					err = talentAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG);
				}
				else
				{
					err = talentAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG, givenTransaction);
				}
				if ((!(err.HasError)) && (!(talentAccessDetail.ResultDataSet == null)))
				{
					affectedRows = System.Convert.ToInt32(talentAccessDetail.ResultDataSet.Tables[0].Rows[0][0]);
				}
				talentAccessDetail = null;
				if (affectedRows > 0)
				{
					affectedRows = IncrementUniqueId(givenTransaction);
					if (affectedRows > 0)
					{
						return newConfigId;
					}
				}
				return null;
			}
			public ConfigurationEntity GetConfigurationEntity(string configId)
			{
				string commandText = "SELECT * FROM tbl_config_detail WHERE config_id=@CONFIG_ID";
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(Utilities.ConstructParameter("@CONFIG_ID", configId));
					ErrorObj err = new ErrorObj();
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG);
					if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
					{
						outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables[0];
					}
				}
				catch (Exception)
				{
					throw;
				}
				finally
				{
					talentSqlAccessDetail = null;
				}
				if (outputDataTable.Rows.Count > 0)
				{
					return ConfigurationEntity.GetEntity(outputDataTable.Rows[0]);
				}
				return null;
			}
			#endregion
			private ConfigurationEntity[] GetConfigurationEntities(DataTable outputDataTable)
			{
				System.Collections.Generic.List<ConfigurationEntity> list = new System.Collections.Generic.List<ConfigurationEntity>();
				foreach (DataRow row in outputDataTable.Rows)
				{
					ConfigurationEntity entity = ConfigurationEntity.GetEntity(row);
					entity.FormattedDisplayName = GetProperCase(entity.DisplayName);
					entity.DisplayName = UpperCaseFirst(entity.DisplayName);
					list.Add(entity);
				}
				return list.ToArray();
			}
			public string GetProperCase(string name)
			{
				//name = StrConv(name, VbStrConv.ProperCase)
				name = name.Replace("_", string.Empty);
				name = UpperCaseFirst(name);
				name = SplitCamelCase(name);
				return name;
			}
			public static string SplitCamelCase(string str)
			{
				return Regex.Replace(Regex.Replace(str, "(\\P{Ll})(\\P{Ll}\\p{Ll})", "$1 $2"), "(\\p{Ll})(\\P{Ll})", "$1 $2");
			}
			public static string UpperCaseFirst(string str)
			{
				return char.ToUpper(str[0]) + str.Substring(1);
			}
		}
	}
}
