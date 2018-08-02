using System;
using System.Collections.Generic;
using System.Data;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	public class Database_Text_Search : DBObjectBase
	{
		public Database_Text_Search(ref DESettings settings)
			: base(settings)
		{
		}
		/// <summary>
		/// Returns text/control lang data from Front-End database
		/// </summary>
		/// <param name="searchText"></param>
		/// <param name="pageOffset"></param>
		/// <param name="pageSize"></param>
		/// <param name="sortColumn"></param>
		/// <param name="sortOrder"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public DataSet TextSearch(string searchText, int pageOffset, int pageSize, string sortColumn = "TEXT_CODE", bool sortOrder = false)
		{
			TalentSystemDefaults.TalentDataAccess talentSqlAccessDetail = new TalentSystemDefaults.TalentDataAccess();
			ErrorObj err = new ErrorObj();
			DataSet textSearchData = new DataSet();
			try
			{
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
				talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure;
				talentSqlAccessDetail.CommandElements.CommandText = "usp_SearchText_GetData";
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@searchText", searchText));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@sortColumn", sortColumn));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@sortOrder", sortOrder, SqlDbType.Bit));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pageOffset", pageOffset, SqlDbType.Int));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pageSize", pageSize, SqlDbType.Int));
				err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005);
				if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
				{
					textSearchData = talentSqlAccessDetail.ResultDataSet;
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
			return textSearchData;
		}
		/// <summary>
		/// Updates text/control lang data into Front-End database
		/// </summary>
		/// <param name="modifiedRows"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool SaveTextSearchData(System.Collections.Generic.List<TextSearchEntity> modifiedRows, Dictionary<string, string> tableList)
		{
			TalentSystemDefaults.TalentDataAccess talentSqlAccessDetail = new TalentSystemDefaults.TalentDataAccess();
			ErrorObj err = new ErrorObj();
			System.Text.StringBuilder xmlData = new System.Text.StringBuilder();
			System.Collections.Generic.List<TextSearchEntity> gridRows = new System.Collections.Generic.List<TextSearchEntity>();
			//Dim tables As New List(Of String)
			int errorCode = 0;
			string errorMessage = string.Empty;
			try
			{
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
				talentSqlAccessDetail.CommandElements.CommandText = "usp_SearchText_SetData";
				talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure;
				// get list of tables to be updated
				//tables = modifiedRows.Select(Function(x) x.TableName).Distinct().ToList()
				xmlData.AppendLine("<data>");
				// mention tables to be updated
				xmlData.AppendLine("<tables>");
				foreach (var pair in tableList)
				{
					xmlData.AppendLine(string.Format("<table id=\"{0}\">{1}</table>", pair.Value, pair.Key));
				}
				xmlData.AppendLine("</tables>");
				// mention rows to be updated
				xmlData.AppendLine("<rows>");
				foreach (var row in modifiedRows)
				{
					xmlData.AppendLine(string.Format("<row tab=\"{0}\" id=\"{1}\">", row.TableName, row.ID.ToString()));
					if (row.Text_Code != string.Empty)
					{
						xmlData.AppendLine("<text_code>" + row.Text_Code + "</text_code>");
					}
					if (row.Text_Content != string.Empty)
					{
						xmlData.AppendLine("<text_content>" + Utilities.GetHTMLEncodedStringForDB(System.Convert.ToString(row.Text_Content)).Replace("'","''") + "</text_content>");
					}
					xmlData.AppendLine("</row>");
				}
				xmlData.AppendLine("</rows>");
				xmlData.AppendLine("</data>");
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@xml", xmlData.ToString(), SqlDbType.Xml));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Error_Code", errorCode, SqlDbType.Int, ParameterDirection.Output));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Error_Message", errorMessage, SqlDbType.VarChar, ParameterDirection.Output));
				err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005);
				if (!err.HasError)
				{
					return true;
				}
				else
				{
					// read the output error parameters
					errorCode = System.Convert.ToInt32(talentSqlAccessDetail.CommandElements.CommandParameter[1].ParamValue);
					errorMessage = System.Convert.ToString(talentSqlAccessDetail.CommandElements.CommandParameter[2].ParamValue);
					return false;
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
			return false;
		}
		/// <summary>
		/// Replaces the search-text passed with the replace text into the control/page text DB tables
		/// It is not in use anymore.
		/// </summary>
		/// <param name="searchString"></param>
		/// <param name="replaceString"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool ReplaceSearchText(string searchString, string replaceString)
		{
			TalentSystemDefaults.TalentDataAccess talentSqlAccessDetail = new TalentSystemDefaults.TalentDataAccess();
			ErrorObj err = new ErrorObj();
			string query = "UPDATE tbl_page_text_lang SET TEXT_CONTENT = CAST( REPLACE(CAST(TEXT_CONTENT as NVarchar(max)), @searchString, @replaceString) AS NText)  WHERE TEXT_CONTENT LIKE @searchText AND TEXT_CONTENT NOT LIKE \'%<\' + @searchString + \'>%\'; " +
				"UPDATE tbl_control_text_lang SET CONTROL_CONTENT = CAST( REPLACE(CAST(CONTROL_CONTENT as NVarchar(max)), @searchString, @replaceString) AS NText)  WHERE CONTROL_CONTENT LIKE @searchText AND CONTROL_CONTENT NOT LIKE \'%<\' + @searchString + \'>%\' ;";
			try
			{
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery;
				talentSqlAccessDetail.CommandElements.CommandText = query;
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@searchText", "%" + searchString + "%"));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@searchString", searchString));
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@replaceString", replaceString));
				err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005);
				if (!err.HasError)
				{
					return true;
				}
				else
				{
					return false;
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
			return false;
		}
	}
}
