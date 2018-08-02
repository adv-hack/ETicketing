using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace TalentSystemDefaults
{
	namespace DataAccess.ConfigObjects
	{
		public class tbl_config_detail_audit : DBObjectBase
		{
			#region Class Level Fields
			private const string TBL_CONFIG_DETAIL_AUDIT = "tbl_config_detail_audit";
			#endregion
			#region Public Methods
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="settings"></param>
			/// <remarks></remarks>
			public tbl_config_detail_audit(ref DESettings settings)
				: base(settings)
			{
				logSourceClass = this.GetType().BaseType.Name;
			}
			/// <summary>
			/// Inserts serialized transaction into the database
			/// </summary>
			/// <param name="ms"></param>
			/// <param name="GroupID"></param>
			/// <param name="ModuleName"></param>
			/// <param name="UserName"></param>
			/// <param name="TableName"></param>
			/// <param name="Action"></param>
			/// <param name="DataSource"></param>
			/// <param name="Catalog"></param>
			/// <param name="BusinessUnit"></param>
			/// <param name="GivenTransaction"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public bool Insert(MemoryStream ms, string GroupID, string ModuleName, string UserName, string TableName, string Action, string DataSource, string Catalog, string BusinessUnit, SqlTransaction GivenTransaction = null)
			{
				int affectedRows = 0;
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				ErrorObj err = new ErrorObj();
				var commandText = string.Format("INSERT INTO dbo.tbl_config_detail_audit(GROUP_ID, USER_NAME, MODULE_NAME, COMMAND, TIMESTAMP,TABLE_NAME,ACTION,DATA_SOURCE,CATALOG,BUSINESS_UNIT) values (@GROUP_ID, @USER_NAME, @MODULE_NAME, @COMMAND, GETDATE(),@TABLE_NAME,@ACTION,@DATA_SOURCE,@CATALOG,@BUSINESS_UNIT)");
				bool retVal = true;
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GROUP_ID", GroupID));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@USER_NAME", UserName));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MODULE_NAME", ModuleName.ToUpper()));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@COMMAND", ms.GetBuffer(), SqlDbType.VarBinary));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TABLE_NAME", TableName));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ACTION", Action));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DATA_SOURCE", DataSource));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CATALOG", Catalog));
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit.ToUpper()));
					//Execute
					if (GivenTransaction == null)
					{
						err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_DEFINITION, false);
					}
					else
					{
						err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_DEFINITION, GivenTransaction, false);
					}
					if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
					{
						affectedRows = System.Convert.ToInt32(talentSqlAccessDetail.ResultDataSet.Tables[0].Rows[0][0]);
					}
				}
				catch (Exception ex)
				{
					retVal = false;
					Utilities.InsertErrorLog(settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, settings.BusinessUnit, err.ErrorNumber, logFilter3, logFilter4, ex.StackTrace.ToString(), err.ErrorMessage);
				}
				finally
				{
					talentSqlAccessDetail = null;
				}
				return retVal;
			}
			/// <summary>
			/// Returns a set of serialized transactions for a specified group
			/// </summary>
			/// <param name="GroupID"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public List<TalentDataAccess> GetCommandsByGroupId(int GroupID)
			{
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				TalentDataAccess objTalentAccessDetail = default(TalentDataAccess);
				string commandText = string.Format("SELECT COMMAND FROM tbl_config_detail_audit WHERE GROUP_ID=@GroupID ORDER BY ID");
				System.Collections.Generic.List<TalentDataAccess> lstTalentAccessDetail = new System.Collections.Generic.List<TalentDataAccess>();
				ErrorObj err = new ErrorObj();
				BinaryFormatter bf = new BinaryFormatter();
				byte[] byteArr = null;
				MemoryStream ms = default(MemoryStream);
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupID", GroupID));
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_DEFINITION);
					if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
					{
						outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables[0];
					}
					foreach (DataRow dr in outputDataTable.Rows)
					{
						byteArr = (byte[])(dr["COMMAND"]);
						ms = new MemoryStream(byteArr);
						objTalentAccessDetail = (TalentDataAccess)(bf.Deserialize(ms));
						lstTalentAccessDetail.Add(objTalentAccessDetail);
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
				return lstTalentAccessDetail;
			}
			public override DataTable RetrieveDTForList()
			{
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				string commandText = string.Format("select a.GROUP_ID as ID, convert(varchar,min(TIMESTAMP),120) TIMESTAMP, min(user_name) USER_NAME, min(MODULE_NAME) MODULE_NAME, count(1) as COMMAND_COUNT, MIN(BUSINESS_UNIT) as BUSINESS_UNIT, MIN(DATA_SOURCE) AS DATA_SOURCE, min(CATALOG) as CATALOG from tbl_config_detail_audit a group by a.GROUP_ID");
				ErrorObj err = new ErrorObj();
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_DEFINITION);
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
			public Dictionary<string, string> RetrieveAuditGroupInfo(int groupId)
			{
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				string commandText = "select a.GROUP_ID as ID, min(TIMESTAMP) TIMESTAMP, min(user_name) USER_NAME, min(MODULE_NAME) MODULE_NAME, count(1) as COMMAND_COUNT, MIN(BUSINESS_UNIT) as BUSINESS_UNIT, MIN(DATA_SOURCE) AS DATA_SOURCE, min(CATALOG) as CATALOG from tbl_config_detail_audit a where a.group_id=@GroupID group by a.GROUP_ID";
				ErrorObj err = new ErrorObj();
				Dictionary<string, string> results = new Dictionary<string, string>();
				try
				{
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupID", groupId));
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_DEFINITION);
					if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
					{
						outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables[0];
					}
					if (outputDataTable.Rows.Count == 1)
					{
						DataRow row = outputDataTable.Rows[0];
						foreach (System.Data.DataColumn column in outputDataTable.Columns)
						{
							string columnName = column.ColumnName;
							results.Add(columnName, row[columnName].ToString());
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
				return results;
			}
			/// <summary>
			/// Returns next available group ID from database
			/// </summary>
			/// <returns></returns>
			/// <remarks></remarks>
			public int GetNextGroupID()
			{
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				ErrorObj err = new ErrorObj();
				string commandText = string.Format("SELECT ISNULL(MAX(GROUP_ID),0) + 1 as GROUP_ID FROM DBO.TBL_CONFIG_DETAIL_AUDIT");
				int retVal = 0;
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_DEFINITION);
					if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
					{
						outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables[0];
					}
					if (!(outputDataTable == null) && outputDataTable.Rows.Count > 0)
					{
						retVal = System.Convert.ToInt32(outputDataTable.Rows[0]["GROUP_ID"]);
					}
					else
					{
						retVal = 0;
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
		}
	}
}
