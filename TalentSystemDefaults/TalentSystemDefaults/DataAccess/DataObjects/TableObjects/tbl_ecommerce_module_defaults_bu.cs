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
		public class tbl_ecommerce_module_defaults_bu : DBObjectBase
		{
			#region Shared Properties
			static private bool _DefaultKey1Active = false;
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
			#region Class Level Fields
			const string TBL_ECOMMERCE_MODULE_DEFAULTS_BU = "tbl_ecommerce_module_defaults_bu";
			#endregion
			#region Constructors
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="settings"></param>
			/// <remarks></remarks>
			public tbl_ecommerce_module_defaults_bu(ref DESettings settings)
				: base(settings)
			{
				businessUnit = settings.BusinessUnit;
			}
			#endregion
			#region Public Methods
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
			public override int UpdateCurrentValue(System.Collections.Generic.List<ConfigurationItem> configs, SqlTransaction givenTransaction = null)
			{
				return UpdateData(businessUnit, configs, givenTransaction);
			}
			public override List<ConfigurationEntity> RetrieveAllValues(string businessUnit, string[] defaultKeys, string[] variableKeys, string displayName = "")
			{
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				string commandText = "SELECT distinct default_name,value FROM dbo.tbl_ecommerce_module_defaults_bu WHERE business_unit = @BUSINESS_UNIT" + (displayName != string.Empty ? " AND DEFAULT_NAME like \'%\' + @DEFAULT_NAME + \'%\'" : "");
				ErrorObj err = new ErrorObj();
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandParameter.Clear();
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
					if (displayName != string.Empty)
					{
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DEFAULT_NAME", displayName));
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
				return GetConfigurationData(outputDataTable, defaultKeys, variableKeys);
			}
			/// <summary>
			/// Returns the group ID
			/// </summary>
			/// <returns></returns>
			/// <remarks></remarks>
			public int GetGroupID()
			{
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				StringBuilder commandText = new StringBuilder();
				ErrorObj err = new ErrorObj();
				try
				{
					commandText.AppendLine("select value from dbo.tbl_ecommerce_module_defaults_bu a where a.DEFAULT_NAME = \'TALENT_DEFAULTS_GROUP_ID\'");
					commandText.AppendLine("update	dbo.tbl_ecommerce_module_defaults_bu");
					commandText.AppendLine("set		VALUE = cast(VALUE as int) + 1 ");
					commandText.AppendLine("where	DEFAULT_NAME = \'TALENT_DEFAULTS_GROUP_ID\'");
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandText = commandText.ToString();
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
				return int.Parse(System.Convert.ToString(outputDataTable.Rows[0]["value"]));
			}
			/// <summary>
			/// Returns web API url from database
			/// </summary>
			/// <returns></returns>
			/// <remarks></remarks>
			public string GetSystemDefaultsURL()
			{
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				string commandText = "SELECT VALUE FROM DBO.TBL_ECOMMERCE_MODULE_DEFAULTS_BU WHERE BUSINESS_UNIT = @BUSINESS_UNIT AND DEFAULT_NAME = \'SYSTEM_DEFAULTS_URL\'";
				ErrorObj err = new ErrorObj();
				string retVal = string.Empty;
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005);
					if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
					{
						System.Data.DataTable with_3 = talentSqlAccessDetail.ResultDataSet.Tables[0];
						if (with_3.Rows.Count > 0)
						{
							retVal = with_3.Rows[0]["VALUE"].ToString();
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
				return retVal;
			}
			#endregion
			#region Private Methods
			/// <summary>
			/// Returns current configurations
			/// </summary>
			/// <param name="businessUnit"></param>
			/// <param name="configs"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private Dictionary<string, string> RetrieveCurrentValues(string businessUnit, System.Collections.Generic.List<ConfigurationItem> configs)
			{
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				string[] whereClause = new string[configs.Count - 1 + 1];
				string where = string.Empty;
				string commandText = string.Empty;
				ErrorObj err = new ErrorObj();
				try
				{
					for (int i = 0; i <= configs.Count - 1; i++)
					{
						whereClause[i] = "(default_name = @DEFAULT_NAME" + i.ToString() + ")";
					}
					where = string.Join(" OR ", whereClause);
					commandText = string.Format("SELECT distinct ID,DEFAULT_NAME,VALUE FROM dbo.tbl_ecommerce_module_defaults_bu WHERE business_unit = @BUSINESS_UNIT AND ({0})", where);
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandParameter.Clear();
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
					for (int i = 0; i <= configs.Count - 1; i++)
					{
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DEFAULT_NAME" + i.ToString(), configs[i].DefaultName));
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
			/// <summary>
			/// Returns set of configurations read from datatable passed
			/// </summary>
			/// <param name="dt"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private Dictionary<string, string> GetCurrentValues(DataTable dt)
			{
				var results = new Dictionary<string, string>();
				foreach (DataRow row in dt.Rows)
				{
					var attrName = Utilities.CheckForDBNull_String(row["DEFAULT_NAME"]).Trim();
					var attrValue = Utilities.CheckForDBNull_String(row["VALUE"]).Trim();
					if (!results.ContainsKey(attrName))
					{
						results.Add(attrName, attrValue);
					}
				}
				return results;
			}
			/// <summary>
			/// Returns configurations as list of ConfigurationEntity read from dataTable passed
			/// </summary>
			/// <param name="dt"></param>
			/// <param name="defaultKeys"></param>
			/// <param name="variableKeys"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private List<ConfigurationEntity> GetConfigurationData(DataTable dt, string[] defaultKeys, string[] variableKeys)
			{
				var results = new System.Collections.Generic.List<ConfigurationEntity>();
				foreach (DataRow row in dt.Rows)
				{
					var dispName = Utilities.CheckForDBNull_String(row["DEFAULT_NAME"]);
					var attrName = Utilities.CheckForDBNull_String(row["DEFAULT_NAME"]);
					var attrValue = Utilities.CheckForDBNull_String(row["VALUE"]);
					//Dim attrDescription = Utilities.CheckForDBNull_String(row("DESCRIPTION"))
					results.Add(new ConfigurationEntity(TBL_ECOMMERCE_MODULE_DEFAULTS_BU, string.Empty, defaultKeys, variableKeys, dispName, attrName, attrValue, string.Empty, string.Empty, ""));
				}
				return results;
			}
			/// <summary>
			/// Updates configurations
			/// </summary>
			/// <param name="businessUnit"></param>
			/// <param name="configs"></param>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private int UpdateData(string businessUnit, System.Collections.Generic.List<ConfigurationItem> configs, SqlTransaction givenTransaction)
			{
				StringBuilder cases = new StringBuilder();
				for (int i = 0; i <= configs.Count - 1; i++)
				{
					cases.Append(" WHEN  @DEFAULT_NAME" + i.ToString() + " THEN  @DEFAULT_VALUE" + i.ToString());
				}
				string[] whereClause = new string[configs.Count - 1 + 1];
				for (int i = 0; i <= configs.Count - 1; i++)
				{
					whereClause[i] = "(DEFAULT_NAME = @DEFAULT_NAME" + i.ToString() + ")";
				}
				string where = string.Join(" OR ", whereClause);
				var commandText = string.Format("UPDATE dbo.tbl_ecommerce_module_defaults_bu SET VALUE = CASE default_name {0} END  WHERE business_unit = @BUSINESS_UNIT AND ({1})", cases.ToString(), where);
				int affectedRows = 0;
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				ErrorObj err = new ErrorObj();
				//Construct The Call
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
				talentSqlAccessDetail.CommandElements.CommandText = commandText;
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
				for (int i = 0; i <= configs.Count - 1; i++)
				{
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DEFAULT_NAME" + i.ToString(), configs[i].DefaultName));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DEFAULT_VALUE" + i.ToString(), configs[i].UpdatedValue));
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
				//Return results
				return affectedRows;
			}

			public override void AddMissingValues(System.Collections.Generic.List<ConfigurationItem> configs, Dictionary<string, string> currentValues)
			{
				List<ConfigurationItem> missingConfigs;
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
				ErrorObj err = new ErrorObj();
				List<string> items = new List<string>();
				string commandText = String.Empty;
				string values = string.Empty;
				try
				{
					missingConfigs = configs.Where(c => !currentValues.ContainsKey(c.DefaultName)).ToList();
					if (missingConfigs.Count > 0)
					{
						talentSqlAccessDetail.CommandElements.CommandParameter.Clear();
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", ALL_STRING));
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@APPLICATION", string.Empty));
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MODULE", string.Empty));
						for (int i = 0; i < missingConfigs.Count; i++)
						{
							int j = i + 1;
							ConfigurationItem config = missingConfigs[i];
							String text = string.Format("SELECT @BUSINESS_UNIT, @PARTNER_CODE, @APPLICATION, @MODULE, @DEFAULT_NAME_{0}, @VALUE_{0}", j);
							talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DEFAULT_NAME_" + j, config.DefaultName));
							talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VALUE_" + j, config.CurrentValue));
							items.Add(text);
						}

						values = String.Join(" UNION ", items.ToArray());
						commandText = string.Format("INSERT into tbl_ecommerce_module_defaults_bu {0}", values);
						talentSqlAccessDetail.CommandElements.CommandText = commandText;

						err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005);
						if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
						{
							outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables[0];
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
		}
	}
}
