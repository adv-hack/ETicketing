using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	namespace DataAccess.DataObjects
	{
		public class MD501 : DBObjectBase
		{
			#region Class Level Fields
			private const string MD501_Renamed = "MD501";
			private string type51 = string.Empty;
			#endregion
			#region Shared Properties
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
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="settings"></param>
			/// <remarks></remarks>
			public MD501(ref DESettings settings)
				: base(settings)
			{
				this.DBTypeForTable = DBType.DB2;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="companyCode"></param>
			/// <param name="type51"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public DataTable SelectByCompAndType(string companyCode, string type51)
			{
                //how to get company code, type code and active flag
                string sqlStatement = string.Empty;
                sqlStatement = " SELECT CODE51, DESC51, VALU51 FROM MD501 WHERE CONO51 = @CompanyCode AND TYPE51 = @Type AND ACTR51 = 'A' ";
				DataTable outputDataTable = new DataTable();
				TalentDB2Access talentDB2AccessDetail = new TalentDB2Access();
				try
				{
					talentDB2AccessDetail.Settings = settings;
					talentDB2AccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentDB2AccessDetail.CommandElements.CommandParameter.Clear();
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@CompanyCode", companyCode, 3));
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@Type", type51, 4));
					ErrorObj err = new ErrorObj();
					talentDB2AccessDetail.CommandElements.CommandText = sqlStatement;
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
				return outputDataTable;
			}

			/// <summary>
			/// Returns if the new Payment Type Code being added is unique is not
			/// </summary>
			/// <param name="PaymentTypeCode"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public bool IsPaymentTypeUnique(string TicketingType)
			{
				int affectedRows = 0;
				TalentDB2Access talentDB2AccessDetail = new TalentDB2Access();
				ErrorObj err = new ErrorObj();
				string commandText = "SELECT 1 FROM MD501 WHERE CONO51 = @CompanyCode AND TYPE51 = @Type51 AND VALU51 = @Valu51 AND ACTR51 = 'A'";
				talentDB2AccessDetail.Settings = settings;
				talentDB2AccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
				talentDB2AccessDetail.CommandElements.CommandText = commandText;
				talentDB2AccessDetail.CommandElements.CommandParameter.Clear();
				talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@Type51", "PAYT", 4));
				talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@VALU51", TicketingType, 15, iDB2DbType.iDB2Decimal));
				talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@CompanyCode", companyCode, 3));
				//Execute
				err = talentDB2AccessDetail.DB2Access(DestinationDatabase.TALENTTKT);
				if ((!(err.HasError)) && (!(talentDB2AccessDetail.ResultDataSet == null)))
				{
					affectedRows = talentDB2AccessDetail.ResultDataSet.Tables[0].Rows.Count;
				}
				talentDB2AccessDetail = null;
				return affectedRows > 0 ? false : true;
			}

			public override List<ConfigurationEntity> RetrieveAlliSeriesValues(string companyCode, string[] defaultKeys, string[] variableKeys, string selectedColumns = "")
			{
				DataTable outputDataTable = new DataTable();
				TalentDB2Access talentDB2AccessDetail = new TalentDB2Access();
				ErrorObj err = new ErrorObj();
				string columns = (selectedColumns != string.Empty ? selectedColumns : "*");
				try
				{
					string commandText = string.Format("SELECT {0} FROM MD501 WHERE CONO51 = @CompanyCode AND TYPE51 = @Type51 AND CODE51 = @Code51", selectedColumns);
					talentDB2AccessDetail.Settings = settings;
					talentDB2AccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentDB2AccessDetail.CommandElements.CommandText = commandText;
					talentDB2AccessDetail.CommandElements.CommandParameter.Clear();
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@Type51", defaultKeys[0], 4));
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@Code51", variableKeys[0], 15));
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
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="configs"></param>
			/// <remarks></remarks>
			public override void RetrieveCurrentValues(System.Collections.Generic.List<ConfigurationItem> configs)
			{
				var currentValues = RetrieveCurrentValues(businessUnit, configs);
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
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="configs"></param>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public override int UpdateCurrentValueDB2(System.Collections.Generic.List<ConfigurationItem> configs, iDB2Transaction givenTransaction = null)
			{
				return UpdateData(businessUnit, variableKey1Value, configs, givenTransaction);
			}
			/// <summary>
			/// Inserts configurations for DB2
			/// </summary>
			/// <param name="configs"></param>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			public override int InsertCurrentValueDB2(System.Collections.Generic.List<ConfigurationItem> configs, iDB2Transaction givenTransaction = null)
			{
				return InsertData(configs, givenTransaction);
			}
			/// <summary>
			/// Deletes configurations from DB2
			/// </summary>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			public override int DeleteCurrentValuesDB2(iDB2Transaction givenTransaction = null)
			{
				return DeleteData(settings.VariableKey1, settings.VariableKey2, givenTransaction);
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
						results.Add(new ConfigurationEntity(MD501_Renamed, string.Empty, defaultKeys, variableKeys, dispName, defaultName, defaultValue, string.Empty, string.Empty));
					}
				}
				return results;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="businessUnit"></param>
			/// <param name="configs"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private Dictionary<string, string> RetrieveCurrentValues(string businessUnit, System.Collections.Generic.List<ConfigurationItem> configs)
			{
				DataTable outputDataTable = new DataTable();
				TalentDB2Access talentDB2AccessDetail = new TalentDB2Access();
				ErrorObj err = new ErrorObj();
				try
				{
					string commandText = "SELECT * FROM MD501 WHERE CONO51 = @CompanyCode AND TYPE51 = @Type51 AND CODE51 = @Code51";
					//Construct The Call
					talentDB2AccessDetail.Settings = settings;
					talentDB2AccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentDB2AccessDetail.CommandElements.CommandText = commandText;
					talentDB2AccessDetail.CommandElements.CommandParameter.Clear();
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@CompanyCode", companyCode, 3));
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@Type51", configs[0].DefaultKey1, 4));
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@Code51", variableKey1Value, 15));
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
			/// XML Comment
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
					string values = string.Join(" , ", setValue);
					var commandText = string.Format("UPDATE MD501 SET {0} WHERE CONO51 = @CompanyCode AND TYPE51 = @Type51 AND CODE51 = @Code51", values);
					talentDB2AccessDetail.Settings = settings;
					talentDB2AccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
					talentDB2AccessDetail.CommandElements.CommandText = commandText;
					talentDB2AccessDetail.CommandElements.CommandParameter.Clear();
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@CompanyCode", companyCode, 3));
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@Type51", configs[0].DefaultKey1, 4));
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@Code51", variableKey1Value, 15));
					for (int i = 0; i <= configs.Count - 1; i++)
					{
						if (configs[i].DefaultName == "UPDT51" || configs[i].DefaultName == "VALU51")
						{
							talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@UPDATED_VALUE" + i.ToString(), System.Convert.ToString(configs[i].UpdatedValue), iDB2DbType.iDB2Decimal));
						}
						else
						{
							talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@UPDATED_VALUE" + i.ToString(), System.Convert.ToString(configs[i].UpdatedValue)));
						}
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
			///
			/// </summary>
			/// <param name="businessUnit"></param>
			/// <param name="variableKey1Value"></param>
			/// <param name="configs"></param>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			private int InsertData(System.Collections.Generic.List<ConfigurationItem> configs, iDB2Transaction givenTransaction)
			{
				int affectedRows = 0;
				TalentDB2Access talentDB2AccessDetail;
				ErrorObj err = new ErrorObj();
				string commandText = string.Empty;
				StringBuilder columnNames, columnValues;
				try
				{
					talentDB2AccessDetail = new TalentDB2Access();
					talentDB2AccessDetail.Settings = settings;
					talentDB2AccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
					commandText = "INSERT INTO MD501({0}) VALUES({1})";
					columnNames = new StringBuilder();
					columnValues = new StringBuilder();
					for (int i = 0; i <= configs.Count - 1; i++)
					{
						columnNames.Append(configs[i].DefaultName + ",");
						columnValues.Append("@" + configs[i].DefaultName + ",");
						if (configs[i].DefaultName == "UPDT51" || configs[i].DefaultName == "VALU51")
						{
							talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@" + configs[i].DefaultName, configs[i].UpdatedValue, iDB2DbType.iDB2Decimal));
						}
						else
						{
							talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@" + configs[i].DefaultName, configs[i].UpdatedValue));
						}
					}
					commandText = string.Format(commandText, columnNames.ToString().Substring(0, columnNames.ToString().Length - 1), columnValues.ToString().Substring(0, columnValues.ToString().Length - 1));
					talentDB2AccessDetail.CommandElements.CommandText = commandText;

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
			/// Deletes payment type information from MD501
			/// </summary>
			/// <param name="type51"></param>
			/// <param name="code51"></param>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			public int DeleteData(string type51, string code51, iDB2Transaction givenTransaction = null)
			{
				int affectedRows = 0;
				TalentDB2Access talentDB2AccessDetail = new TalentDB2Access();
				ErrorObj err = new ErrorObj();
				try
				{
					string commandText = "UPDATE MD501 SET ACTR51 = 'D',USER51=@USER51,PGMD51=@PGMD51,UPDT51=@UPDT51 WHERE CONO51 = @CompanyCode AND TYPE51 = @Type51 AND CODE51 = @Code51";
					talentDB2AccessDetail.Settings = settings;
					talentDB2AccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
					talentDB2AccessDetail.CommandElements.CommandText = commandText;
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@CompanyCode", companyCode, 3));
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@Type51", type51, 4));
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@Code51", code51, 15));
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@USER51", settings.AgentName, 10));
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@PGMD51", settings.StoredProcedure, 10));
					talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@UPDT51", settings.iSeriesTodaysDate, iDB2DbType.iDB2Decimal));

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

            public bool DoesDescriptionItemExist(string type, string code)
            {
                int affectedRows = 0;
                TalentDB2Access talentDB2AccessDetail = new TalentDB2Access();
                ErrorObj err = new ErrorObj();
                string commandText = "SELECT 1 FROM MD501 WHERE CONO51 = @CompanyCode AND TYPE51 = @Type51 AND CODE51 = @Code51 AND ACTR51 = 'A'";
                talentDB2AccessDetail.Settings = settings;
                talentDB2AccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
                talentDB2AccessDetail.CommandElements.CommandText = commandText;
                talentDB2AccessDetail.CommandElements.CommandParameter.Clear();
                talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@Type51", type, 4));
                talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@Code51", code, 15, iDB2DbType.iDB2Char));
                talentDB2AccessDetail.CommandElements.CommandParameter.Add(ConstructDB2Parameter("@CompanyCode", companyCode, 3));
                //Execute
                err = talentDB2AccessDetail.DB2Access(DestinationDatabase.TALENTTKT);
                if ((!(err.HasError)) && (!(talentDB2AccessDetail.ResultDataSet == null)))
                {
                    affectedRows = talentDB2AccessDetail.ResultDataSet.Tables[0].Rows.Count;
                }
                talentDB2AccessDetail = null;
                return affectedRows > 0 ? false : true;
            }           
            #endregion
        }
	}
}
