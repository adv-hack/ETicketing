using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	namespace DataAccess.DataObjects
	{
		public class tbl_error_messages : DBObjectBase
		{
			#region Class Level Fields
			const string TBL_ERROR_MESSAGES = "tbl_error_messages";
			#endregion
			#region Public Properties
			static private bool _DefaultKey1Active = true;
			public static bool DefaultKey1Active
			{
				get
				{
					return _DefaultKey1Active;
				}
				set
				{
					_DefaultKey1Active = value;
				}
			}
			static private bool _DefaultKey2Active = false;
			public static bool DefaultKey2Active
			{
				get
				{
					return _DefaultKey2Active;
				}
				set
				{
					_DefaultKey2Active = value;
				}
			}
			static private bool _DefaultKey3Active = false;
			public static bool DefaultKey3Active
			{
				get
				{
					return _DefaultKey3Active;
				}
				set
				{
					_DefaultKey3Active = value;
				}
			}
			static private bool _DefaultKey4Active = false;
			public static bool DefaultKey4Active
			{
				get
				{
					return _DefaultKey4Active;
				}
				set
				{
					_DefaultKey4Active = value;
				}
			}
			static private bool _DefaultNameActive = true;
			public static bool DefaultNameActive
			{
				get
				{
					return _DefaultNameActive;
				}
				set
				{
					_DefaultNameActive = value;
				}
			}
			static private bool _VariableKey1Active = false;
			public static bool VariableKey1Active
			{
				get
				{
					return _VariableKey1Active;
				}
				set
				{
					_VariableKey1Active = value;
				}
			}
			static private bool _VariableKey2Active = false;
			public static bool VariableKey2Active
			{
				get
				{
					return _VariableKey2Active;
				}
				set
				{
					_VariableKey2Active = value;
				}
			}
			static private bool _VariableKey3Active = false;
			public static bool VariableKey3Active
			{
				get
				{
					return _VariableKey3Active;
				}
				set
				{
					_VariableKey3Active = value;
				}
			}
			static private bool _VariableKey4Active = false;
			public static bool VariableKey4Active
			{
				get
				{
					return _VariableKey4Active;
				}
				set
				{
					_VariableKey4Active = value;
				}
			}
			#endregion
			#region Public Methods
			public tbl_error_messages(ref DESettings settings)
				: base(settings)
			{
				businessUnit = settings.BusinessUnit;
			}
			public override void RetrieveCurrentValues(System.Collections.Generic.List<ConfigurationItem> configs)
			{
				var currentValues = RetrieveCurrentValues(businessUnit, configs);
				AddMissingValues(configs, currentValues);
				string currentValue = "";
				foreach (ConfigurationItem config in configs)
				{
					if (currentValues.ContainsKey(config.DefaultName))
					{
						currentValue = currentValues[config.DefaultName];
						config.CurrentValue = currentValue;
						config.UpdatedValue = currentValue;
					}
				}
			}
			public override List<ConfigurationEntity> RetrieveAllValues(string businessUnit, string[] defaultKeys, string[] variableKeys, string displayName = "")
			{
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				string commandText = "SELECT * FROM tbl_error_messages where ERROR_CODE = @ERROR_CODE and (BUSINESS_UNIT = @BUSINESS_UNIT)";
				ErrorObj err = new ErrorObj();
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandParameter.Clear();
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ERROR_CODE", defaultKeys[0]));
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005);
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
				return GetConfigurationData(outputDataTable, defaultKeys, variableKeys);
			}
			public override int UpdateCurrentValue(System.Collections.Generic.List<ConfigurationItem> configs, SqlTransaction givenTransaction = null)
			{
				return UpdateData(businessUnit, configs, givenTransaction);
			}

			public override void AddMissingValues(System.Collections.Generic.List<ConfigurationItem> configs, Dictionary<string, string> currentValues)
			{
				List<ConfigurationItem> missingConfigs;
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
				ErrorObj err = new ErrorObj();
				string commandText = string.Empty;
				try
				{
					missingConfigs = configs.Where(c => !currentValues.ContainsKey(c.DefaultName)).ToList();
					if (missingConfigs.Count > 0)
					{
						foreach (ConfigurationItem config in missingConfigs)
						{
							commandText = "INSERT INTO tbl_error_messages VALUES(@LANGUAGE_CODE, @BUSINESS_UNIT, @PARTNER_CODE, @MODULE, @PAGE_CODE, @ERROR_CODE, @ERROR_MESSAGE)";
							//Construct The Call
							talentSqlAccessDetail.CommandElements.CommandParameter.Clear();
							talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LANGUAGE_CODE", LANG_ENG));
							talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
							talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", ALL_STRING));
							talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MODULE", ALL_STRING));
							talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", ALL_STRING));
							talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ERROR_CODE", config.DefaultName));
							talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ERROR_MESSAGE", config.CurrentValue));
							talentSqlAccessDetail.CommandElements.CommandText = commandText;
							err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005);
							if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
							{
								outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables[0];
							}
						}
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
			}
			#endregion
			#region Private Methods
			private List<ConfigurationEntity> GetConfigurationData(DataTable dt, string[] defaultKeys, string[] variableKeys)
			{
				var results = new System.Collections.Generic.List<ConfigurationEntity>();
				foreach (DataRow row in dt.Rows)
				{
					var dispName = Utilities.CheckForDBNull_String(row["ERROR_CODE"]);
					var errorcode = Utilities.CheckForDBNull_String(row["ERROR_CODE"]);
					var errormessage = Utilities.CheckForDBNull_String(row["ERROR_MESSAGE"]);
					results.Add(new ConfigurationEntity(TBL_ERROR_MESSAGES, string.Empty, defaultKeys, variableKeys, dispName, errorcode, errormessage, string.Empty, string.Empty, string.Empty));
				}
				return results;
			}
			private Dictionary<string, string> RetrieveCurrentValues(string businessUnit, System.Collections.Generic.List<ConfigurationItem> configs)
			{
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				ErrorObj err = new ErrorObj();
				string[] whereClause = new string[configs.Count - 1 + 1];
				string where = string.Empty;
				string commandText = string.Empty;
				try
				{
					for (int i = 0; i <= configs.Count - 1; i++)
					{
						whereClause[i] = "(error_code = @ERROR_CODE" + i.ToString() + " AND page_code = @PAGE_CODE" + i.ToString() + ")";
					}
					where = string.Join(" OR ", whereClause);
					commandText = string.Format("SELECT * FROM tbl_error_messages WHERE business_unit = @BUSINESS_UNIT AND ({0})", where);
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandParameter.Clear();
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
					for (int i = 0; i <= configs.Count - 1; i++)
					{
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ERROR_CODE" + i.ToString(), configs[i].DefaultName));
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE" + i.ToString(), ALL_STRING));
					}
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005);
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
				return GetCurrentValues(outputDataTable);
			}
			private Dictionary<string, string> GetCurrentValues(DataTable dt)
			{
				var results = new Dictionary<string, string>();
				foreach (DataRow row in dt.Rows)
				{
					var attrName = Utilities.CheckForDBNull_String(row["ERROR_CODE"]).Trim();
					var attrValue = Utilities.CheckForDBNull_String(row["ERROR_MESSAGE"]).Trim();
					if (!results.ContainsKey(attrName))
					{
						results.Add(attrName, attrValue);
					}
				}
				return results;
			}

			private int UpdateData(string businessUnit, List<ConfigurationItem> configs, SqlTransaction givenTransaction)
			{
				StringBuilder cases = new StringBuilder();
				for (int i = 0; i <= configs.Count - 1; i++)
				{
					cases.Append(" WHEN  @ERROR_CODE" + i + " THEN  @ERROR_MESSAGE" + i);
				}

				string[] whereClause = new string[configs.Count - 1 + 1];
				for (int i = 0; i <= configs.Count - 1; i++)
				{
					whereClause[i] = "(error_code = @ERROR_CODE" + i + ")";
				}

				string where = string.Join(" OR ", whereClause);
				var commandText = string.Format("UPDATE tbl_error_messages SET error_message = CASE error_code {0} END WHERE business_unit = @BUSINESS_UNIT AND ({1})", cases.ToString(), where);
				int affectedRows = 0;
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				ErrorObj err = new ErrorObj();
				//Construct The Call
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
				talentSqlAccessDetail.CommandElements.CommandText = commandText;
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
				for (int i = 0; i <= configs.Count() - 1; i++)
				{
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ERROR_CODE" + i, configs[i].DefaultName));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ERROR_MESSAGE" + i, configs[i].UpdatedValue));
				}
				//Execute
				if (givenTransaction == null)
				{
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005);
				}
				else
				{
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005, givenTransaction);
				}
				if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
				{
					affectedRows = System.Convert.ToInt32(talentSqlAccessDetail.ResultDataSet.Tables[0].Rows[0][0]);
				}
				talentSqlAccessDetail = null;
				return affectedRows;
			}

			#endregion
		}
	}
}
