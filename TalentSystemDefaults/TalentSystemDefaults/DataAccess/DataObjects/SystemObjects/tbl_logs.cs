using System;
namespace TalentSystemDefaults.DataAccess.DataObjects.SystemObjects
{
	class tbl_logs : DBObjectBase
	{
		#region Public Methods


		/// <summary>
		/// Constructror
		/// </summary>
		/// <param name="settings"></param>
		/// <remarks></remarks>
		public tbl_logs(DESettings settings)
			: base(settings)
		{

		}

		/// <summary>
		/// Inserts error log details into the SQL database
		/// </summary>
		/// <param name="sourceClass"></param>
		/// <param name="sourceMethod"></param>
		/// <param name="logCode"></param>
		/// <param name="logFilter1"></param>
		/// <param name="logFilter2"></param>
		/// <param name="logFilter3"></param>
		/// <param name="logFilter4"></param>
		/// <param name="logFilter5"></param>
		/// <param name="logContent"></param>
		/// <returns></returns>
		public bool InsertLog(string sourceClass, string sourceMethod, string logCode, string logFilter1, string logFilter2, string logFilter3, string logFilter4, string logFilter5, string logContent)
		{
			TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
			ErrorObj err = new ErrorObj();
			var objCommandText = new System.Text.StringBuilder();

			// prepare command text
			objCommandText.Append(string.Format("INSERT INTO dbo.TBL_LOGS(LOG_SOURCE_CLASS, LOG_SOURCE_METHOD, LOG_CODE, LOG_FILTER_1, LOG_FILTER_2, LOG_FILTER_3, LOG_FILTER_4, LOG_FILTER_5, LOG_CONTENT, LOG_TIMESTAMP, LOG_HEADER_ID) VALUES"));
			objCommandText.Append(string.Format("(@LOG_SOURCE_CLASS,@LOG_SOURCE_METHOD,@LOG_CODE,@LOG_FILTER_1,@LOG_FILTER_2,@LOG_FILTER_3,@LOG_FILTER_4,@LOG_FILTER_5,@LOG_CONTENT,GETDATE(),-1);"));

			// add parameters
			talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LOG_SOURCE_CLASS", sourceClass));
			talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LOG_SOURCE_METHOD", sourceMethod));
			talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LOG_CODE", logCode));
			talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LOG_FILTER_1", logFilter1));
			talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LOG_FILTER_2", logFilter2));
			talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LOG_FILTER_3", logFilter3));
			talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LOG_FILTER_4", logFilter4));
			talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LOG_FILTER_5", logFilter5));
			talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LOG_CONTENT", logContent));

			// execute the command
			try
			{
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
				talentSqlAccessDetail.CommandElements.CommandText = objCommandText.ToString();
				err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005, false);
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				talentSqlAccessDetail = null;
			}

			return !err.HasError;
		}

		#endregion


	}
}
