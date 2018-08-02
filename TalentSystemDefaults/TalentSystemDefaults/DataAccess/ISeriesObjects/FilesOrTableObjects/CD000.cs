using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	namespace DataAccess.DataObjects
	{
		public class CD000 : DBObjectBase
		{
			#region Class Level Fields
			private const string CD000_Renamed = "CD000";
			#endregion
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
			static private bool _BaseDefinition = false;
			public static bool BaseDefinition
			{
				get
				{
					return _BaseDefinition;
				}
				set
				{
					_BaseDefinition = value;
				}
			}
			static private bool _IsiSeriesTable = true;
			public static bool IsiSeriesTable
			{
				get
				{
					return _IsiSeriesTable;
				}
				set
				{
					_IsiSeriesTable = value;
				}
			}
			static private bool _EnableSelectedColumns = true;
			public static bool EnableSelectedColumns
			{
				get
				{
					return _EnableSelectedColumns;
				}
				set
				{
					_EnableSelectedColumns = value;
				}
			}
			#endregion
			#region Public Methods
			public CD000(ref DESettings settings)
				: base(settings)
			{
				this.DBTypeForTable = DBType.DB2;
			}
			public override List<ConfigurationEntity> RetrieveAlliSeriesValues(string companyCode, string[] defaultKeys, string[] variableKeys, string selectedColumns = "")
			{
				DataTable outputDataTable = new DataTable();
				TalentDB2Access talentDB2AccessDetail = new TalentDB2Access();
				ErrorObj err = new ErrorObj();
				string columns = (selectedColumns != string.Empty ? selectedColumns : "*");
				try
				{
					string commandText = string.Format("SELECT {0} FROM CD000 WHERE CONO00  = @CompanyCode", selectedColumns);
					talentDB2AccessDetail.Settings = settings;
					talentDB2AccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentDB2AccessDetail.CommandElements.CommandText = commandText;
					talentDB2AccessDetail.CommandElements.CommandParameter.Clear();
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@CompanyCode", companyCode, 3));
					//Execute
					err = talentDB2AccessDetail.DB2Access(DestinationDatabase.TALENTTKT);
					if ((!(err.HasError)) && (!(talentDB2AccessDetail.ResultDataSet == null)))
					{
						outputDataTable = talentDB2AccessDetail.ResultDataSet.Tables[0];
					}
				}
				catch (Exception)
				{
					throw;
				}
				finally
				{
					talentDB2AccessDetail = null;
				}
				return GetConfigurationData(outputDataTable, defaultKeys, variableKeys);
			}
			public override void RetrieveCurrentValues(System.Collections.Generic.List<ConfigurationItem> configs)
			{
				var currentValues = RetrieveCurrentValuesData(configs);
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
			public override int UpdateCurrentValueDB2(System.Collections.Generic.List<ConfigurationItem> configs, iDB2Transaction givenTransaction = null)
			{
				return UpdateData(businessUnit, variableKey1Value, configs, givenTransaction);
			}
			#endregion
			#region Private Methods
			/// <summary>
			/// XML Comment
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
					foreach (System.Data.DataColumn column in dt.Columns)
					{
						string columnName = column.ColumnName;
						string dispName = columnName;
						var defaultName = columnName;
						string defaultValue = Utilities.CheckForDBNull_String(row[columnName]);
						results.Add(new ConfigurationEntity(CD000_Renamed, string.Empty, defaultKeys, variableKeys, dispName, defaultName, defaultValue, string.Empty, string.Empty));
					}
				}
				return results;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="businessUnit"></param>
			/// <param name="variableKey1Value"></param>
			/// <param name="configs"></param>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private int UpdateData(string businessUnit, string variableKey1Value, System.Collections.Generic.List<ConfigurationItem> configs, iDB2Transaction givenTransaction)
			{
				int affectedRows = 0;
				TalentDB2Access talentDB2AccessDetail = new TalentDB2Access();
				ErrorObj err = new ErrorObj();
				try
				{
					string[] setValue = new string[configs.Count - 1 + 1];
					for (int i = 0; i <= configs.Count - 1; i++)
					{
						setValue[i] = configs[i].DefaultName + " =  @UPDATED_VALUE" + i.ToString();
					}
					string values = string.Join(",", setValue);
					string commandText = string.Format("Update CD000 SET {0} WHERE CONO00  = @CompanyCode", values);
					talentDB2AccessDetail.Settings = settings;
					talentDB2AccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
					talentDB2AccessDetail.CommandElements.CommandText = commandText;
					talentDB2AccessDetail.CommandElements.CommandParameter.Clear();
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@CompanyCode", companyCode, 3));
					for (int i = 0; i <= configs.Count - 1; i++)
					{
						talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@UPDATED_VALUE" + i.ToString(), configs[i].UpdatedValue));
					}
					//Execute
					affectedRows = DB2Access(talentDB2AccessDetail, givenTransaction);
				}
				catch (Exception)
				{
					throw;
				}
				finally
				{
					talentDB2AccessDetail = null;
				}
				return affectedRows;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="businessUnit"></param>
			/// <param name="configs"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private Dictionary<string, string> RetrieveCurrentValuesData(System.Collections.Generic.List<ConfigurationItem> configs)
			{
				DataTable outputDataTable = new DataTable();
				TalentDB2Access talentDB2AccessDetail = new TalentDB2Access();
				ErrorObj err = new ErrorObj();
				try
				{
					string commandText = "SELECT * FROM CD000 WHERE CONO00 = @CompanyCode";
					//Construct The Call
					talentDB2AccessDetail.Settings = settings;
					talentDB2AccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentDB2AccessDetail.CommandElements.CommandText = commandText;
					talentDB2AccessDetail.CommandElements.CommandParameter.Clear();
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@CompanyCode", companyCode, 3));
					err = talentDB2AccessDetail.DB2Access(DestinationDatabase.TALENTTKT);
					if ((!(err.HasError)) && (!(talentDB2AccessDetail.ResultDataSet == null)))
					{
						outputDataTable = talentDB2AccessDetail.ResultDataSet.Tables[0];
					}
				}
				catch (Exception)
				{
					throw;
				}
				finally
				{
					talentDB2AccessDetail = null;
				}
				return GetCurrentValues(outputDataTable, configs);
			}
			/// <summary>
			/// Xml Comment
			/// </summary>
			/// <param name="dt"></param>
			/// <param name="configs"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private Dictionary<string, string> GetCurrentValues(DataTable dt, System.Collections.Generic.List<ConfigurationItem> configs)
			{
				var results = new Dictionary<string, string>();
				if (dt.Rows.Count > 0)
				{
					DataRow row = dt.Rows[0];
					string key = string.Empty;
					string value = string.Empty;
					foreach (System.Data.DataColumn column in dt.Columns)
					{
						key = column.ColumnName;
						value = Utilities.CheckForDBNull_String(row[key]);
						results.Add(key, value);
					}
				}
				return results;
			}
			#endregion
		}
	}
}
