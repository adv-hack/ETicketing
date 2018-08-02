using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	namespace DataAccess.DataObjects
	{
		public class tbl_activity_template_type : DBObjectBase
		{
			#region Class Level Fields
			private const string TBL_ACTIVITY_TEMPLATE_TYPE = "tbl_activity_template_type";
			#endregion

			#region Shared Fields
			static private bool _VariableKey1Active = true;
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
			static private bool _BaseDefinition = true;
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
			#endregion

			#region Private Properties
			private string Name = string.Empty;
			#endregion

			#region Constructors
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="settings"></param>
			/// <remarks></remarks>
            public tbl_activity_template_type(ref DESettings settings)
				: base(settings)
			{
				businessUnit = settings.BusinessUnit;
				if (settings.VariableKey1 != string.Empty)
				{
                    Name = settings.VariableKey1;
				}
			}
			#endregion
			
            #region Public Methods

			public override void RetrieveCurrentValues(System.Collections.Generic.List<ConfigurationItem> configs)
			{
				string currentValue = "";
				var currentValues = RetrieveCurrentValues(businessUnit, configs);
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
				return UpdateData(businessUnit, Name, configs, givenTransaction);
			}

			public override List<ConfigurationEntity> RetrieveAllValues(string businessUnit, string[] defaultKeys, string[] variableKeys, string displayName = "")
			{
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				string commandText = string.Format("SELECT * FROM tbl_activity_template_type WHERE BUSINESS_UNIT=@BUSINESS_UNIT AND NAME=@NAME");
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandParameter.Clear();
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NAME", variableKeys[0]));
					ErrorObj err = new ErrorObj();
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

			#endregion

			#region Private Methods

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
					foreach (System.Data.DataColumn column in dt.Columns)
					{
						string columnName = column.ColumnName;
						string dispName = columnName;
						var defaultName = columnName;
						string defaultValue = Utilities.CheckForDBNull_String(row[columnName]);
						results.Add(new ConfigurationEntity(TBL_ACTIVITY_TEMPLATE_TYPE, string.Empty, defaultKeys, variableKeys, dispName, defaultName, defaultValue, string.Empty, string.Empty));
					}
				}
				return results;
			}

			/// <summary>
			/// Returns set of configurations read from datatable passed
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
				System.Collections.Generic.List<string> codes = new System.Collections.Generic.List<string>();
				string commandText = "SELECT * FROM tbl_activity_template_type WHERE BUSINESS_UNIT = @BUSINESS_UNIT AND NAME = @NAME";
				ErrorObj err = new ErrorObj();
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NAME", Name));
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
				return GetCurrentValues(outputDataTable, configs);
			}

			/// <summary>
			/// Updates configurations
			/// </summary>
			/// <param name="businessUnit"></param>
			/// <param name="name"></param>
			/// <param name="configs"></param>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private int UpdateData(string businessUnit, string name, System.Collections.Generic.List<ConfigurationItem> configs, SqlTransaction givenTransaction)
			{
				string[] setValue = new string[configs.Count - 1 + 1];
				string defaultName = "";
				for (int i = 0; i <= configs.Count - 1; i++)
				{
                    defaultName = configs[i].DefaultName;
                    setValue[i] = string.Format("{0}  =  @VAR_{0}", defaultName);
				}
				string values = string.Join(" , ", setValue);
				var commandText = string.Format("UPDATE tbl_activity_template_type SET {0} WHERE BUSINESS_UNIT = @BUSINESS_UNIT AND NAME = @NAME", values);
				int affectedRows = 0;
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				ErrorObj err = new ErrorObj();
				//Construct The Call
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
				talentSqlAccessDetail.CommandElements.CommandText = commandText;
				for (int i = 0; i <= configs.Count - 1; i++)
				{
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter(string.Format("@VAR_{0}", configs[i].DefaultName), configs[i].UpdatedValue));
				}
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NAME", name));
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

			#endregion
		}
	}
}
