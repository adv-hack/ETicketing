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
		/// <summary>
		/// Provides the functionality to manage data from the table tbl_control_text_lang based on business functionality
		/// </summary>
		[Serializable()]
		public class tbl_page_text_lang : DBObjectBase
		{
			#region Shared Fields
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
			#region Class Level Fields
			const string TBL_PAGE_TEXT_LANG = "tbl_page_text_lang";
			#endregion
			#region Constructors
			public tbl_page_text_lang(ref DESettings settings)
				: base(settings)
			{
				businessUnit = settings.BusinessUnit;
			}
			#endregion
			#region Public Methods
			public override void RetrieveCurrentValues(System.Collections.Generic.List<ConfigurationItem> configs)
			{
                foreach (string pageCode in configs.Select(x => x.DefaultKey1).Distinct())
                {
                    var lstConfigs = configs.FindAll(x => x.DefaultKey1 == pageCode);
                    var currentValues = RetrieveCurrentValues(businessUnit, lstConfigs);
                    AddMissingValues(lstConfigs, currentValues);
                    foreach (ConfigurationItem config in lstConfigs)
                    {
                        string currentValue = "";
                        if (currentValues.ContainsKey(config.DefaultName))
                        {
                            currentValue = currentValues[config.DefaultName];
                            config.CurrentValue = currentValue;
                            config.UpdatedValue = currentValue;
                        }
                    }
                }
			}
			public override int UpdateCurrentValue(System.Collections.Generic.List<ConfigurationItem> configs, SqlTransaction givenTransaction = null)
			{
				return UpdateData(businessUnit, configs, givenTransaction);
			}
			public Dictionary<string, string> GetTextsForPage(string businessUnit, string pageCode)
			{
				Dictionary<string, string> results = new Dictionary<string, string>();
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				string commandText = string.Format("SELECT * FROM tbl_page_text_lang WHERE business_unit = @BUSINESS_UNIT AND page_code = @PAGE_CODE");
				ErrorObj err = new ErrorObj();
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandParameter.Clear();
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", pageCode));
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005);
					if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
					{
						outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables[0];
					}
					foreach (DataRow row in outputDataTable.Rows)
					{
						string key = row["TEXT_CODE"].ToString();
						string value = row["TEXT_CONTENT"].ToString();
						results.Add(key, value);
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
				return results;
			}
			public override List<ConfigurationEntity> RetrieveAllValues(string businessUnit, string[] defaultKeys, string[] variableKeys, string displayName = "")
			{
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				string commandText = string.Format("SELECT * FROM tbl_page_text_lang WHERE business_unit = @BUSINESS_UNIT AND page_code = @PAGE_CODE" + (displayName != string.Empty ? " AND TEXT_CODE like \'%\' + @TEXT_CODE + \'%\'" : "") + " ORDER BY ID");
				ErrorObj err = new ErrorObj();
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandParameter.Clear();
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", defaultKeys[0]));
					if (displayName != string.Empty)
					{
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TEXT_CODE", displayName));
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
			public override List<DEListDetail> RetrieveListValues(System.Collections.Generic.List<DataColumn> dataColumns, System.Collections.Generic.List<KeyColumn> defaultKeyColumns)
			{
				return default(List<DEListDetail>);
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
				ErrorObj err = new ErrorObj();
				string[] whereClause = new string[configs.Count - 1 + 1];
				string where = string.Empty;
				string commandText = string.Empty;
				try
				{
					for (int i = 0; i <= configs.Count - 1; i++)
					{
						whereClause[i] = "(text_code = @TEXT_CODE" + i.ToString() + " AND page_code = @PAGE_CODE" + i.ToString() + ")";
					}
					where = string.Join(" OR ", whereClause);
					commandText = string.Format("SELECT * FROM tbl_page_text_lang WHERE business_unit = @BUSINESS_UNIT AND ({0})", where);
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandParameter.Clear();
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit));
					for (int i = 0; i <= configs.Count - 1; i++)
					{
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TEXT_CODE" + i.ToString(), configs[i].DefaultName));
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE" + i.ToString(), configs[i].DefaultKey1));
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
					var textCode = Utilities.CheckForDBNull_String(row["TEXT_CODE"]).Trim();
					var textContent = Utilities.CheckForDBNull_String(row["TEXT_CONTENT"]).Trim();
					if (!results.ContainsKey(textCode))
					{
						results.Add(textCode, textContent);
					}
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
					cases.Append(" WHEN  @TEXT_CODE" + i.ToString() + " THEN  @TEXT_CONTENT" + i.ToString());
				}
				string[] whereClause = new string[configs.Count - 1 + 1];
				for (int i = 0; i <= configs.Count - 1; i++)
				{
					whereClause[i] = "(text_code = @TEXT_CODE" + i.ToString() + " AND page_code = @PAGE_CODE" + i.ToString() + ")";
				}
				string where = string.Join(" OR ", whereClause);
				var commandText = string.Format("UPDATE tbl_page_text_lang SET text_content = CASE text_code {0} END WHERE business_unit = @BUSINESS_UNIT AND ({1})", cases.ToString(), where);
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
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TEXT_CODE" + i.ToString(), configs[i].DefaultName));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TEXT_CONTENT" + i.ToString(), configs[i].UpdatedValue));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE" + i.ToString(), configs[i].DefaultKey1));
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
					var dispName = Utilities.CheckForDBNull_String(row["TEXT_CODE"]);
					var textCode = Utilities.CheckForDBNull_String(row["TEXT_CODE"]);
					var textContent = Utilities.CheckForDBNull_String(row["TEXT_CONTENT"]);
					results.Add(new ConfigurationEntity(TBL_PAGE_TEXT_LANG, string.Empty, defaultKeys, variableKeys, dispName, textCode, textContent, string.Empty, string.Empty));
				}
				return results;
			}
			#endregion

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
						talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LANGUAGE_CODE", LANG_ENG));
						for (int i = 0; i < missingConfigs.Count; i++)
						{
							int j = i + 1;
							ConfigurationItem config = missingConfigs[i];
							String text = string.Format("SELECT @BUSINESS_UNIT, @PARTNER_CODE, @PAGE_CODE_{0}, @TEXT_CODE_{0}, @LANGUAGE_CODE, @TEXT_CONTENT_{0}", j);
							talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE_" + j, config.DefaultKey1));
							talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TEXT_CODE_" + j, config.DefaultName));
							talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TEXT_CONTENT_" + j, config.CurrentValue));
							items.Add(text);
						}

						values = String.Join(" UNION ", items.ToArray());
						commandText = string.Format("INSERT into tbl_page_text_lang {0}", values);
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
		}
	}
}
