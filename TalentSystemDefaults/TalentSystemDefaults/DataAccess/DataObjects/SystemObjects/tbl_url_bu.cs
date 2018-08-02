using System;
using System.Data;
namespace TalentSystemDefaults
{
	namespace DataAccess.DataObjects.SystemObjects
	{
		public class tbl_url_bu : DBObjectBase
		{
			#region Public Methods
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="settings"></param>
			/// <remarks></remarks>
			public tbl_url_bu(ref DESettings settings)
				: base(settings)
			{
			}
			/// <summary>
			/// Retrieve BusinessUnits
			/// </summary>
			/// <param name="application"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public DataTable RetrieveBusinessUnit(string application)
			{
				DataTable outputDataTable = new DataTable();
				string commandText = "SELECT distinct BUSINESS_UNIT FROM tbl_url_bu WHERE APPLICATION=@Application";
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				ErrorObj err = new ErrorObj();
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					talentSqlAccessDetail.CommandElements.CommandText = commandText;
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Application", application));
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
				return outputDataTable;
			}
			#endregion
		}
	}
}
