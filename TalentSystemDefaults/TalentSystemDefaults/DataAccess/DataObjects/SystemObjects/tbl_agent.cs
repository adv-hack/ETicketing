using System;
using System.Data;
namespace TalentSystemDefaults
{
	namespace DataAccess.DataObjects.SystemObjects
	{
		public class tbl_agent : DBObjectBase
		{
			#region Public Methods
			/// <summary>
			/// Constructror
			/// </summary>
			/// <param name="settings"></param>
			/// <remarks></remarks>
			public tbl_agent(ref DESettings settings)
				: base(settings)
			{
			}
			/// <summary>
			/// Returns agent details by session ID
			/// </summary>
			/// <param name="SessionID"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public DataTable RetrieveAgentDetailsFromSessionID(string sessionID)
			{
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				ErrorObj err = new ErrorObj();
                var commandText = string.Format("select SESSIONID, AGENT_NAME, COMPANY from dbo.tbl_agent a where a.SESSIONID = \'{0}\' and  not exists(select 1 from dbo.tbl_agent x where x.AGENT_NAME = a.AGENT_NAME and x.TIMESTAMP_UPDATED > a.TIMESTAMP_UPDATED AND x.SESSIONID <> a.SESSIONID) ", sessionID);
				DataTable agentDetails = new DataTable();
				try
				{
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005);
					if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
					{
						agentDetails = talentSqlAccessDetail.ResultDataSet.Tables[0];
						if (agentDetails.Rows.Count > 0)
						{
							settings.AgentName = System.Convert.ToString(agentDetails.Rows[0]["AGENT_NAME"]);
							settings.Company = System.Convert.ToString(agentDetails.Rows[0]["COMPANY"]);
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
				return agentDetails;
			}
			#endregion
		}
	}
}
