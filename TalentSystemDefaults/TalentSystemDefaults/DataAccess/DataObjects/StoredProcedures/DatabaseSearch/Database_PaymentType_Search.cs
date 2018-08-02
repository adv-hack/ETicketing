using System;
using System.Data;
namespace TalentSystemDefaults
{
	public class Database_PaymentType_Search : DBObjectBase
	{
		public Database_PaymentType_Search(ref DESettings settings)
			: base(settings)
		{
		}
		public DataSet PaymentTypeSearch(string searchText, int pageOffset, int pageSize, string sortColumn = "PAYMENT_TYPE_CODE", bool sortOrder = false)
		{
			TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
			ErrorObj err = new ErrorObj();
			DataSet paymentTypeData = new DataSet();
			var strQuery = "SELECT COUNT(*) FROM tbl_payment_type WHERE PAYMENT_TYPE_DESCRIPTION LIKE @SearchText; SELECT PAYMENT_TYPE_CODE AS ID, PAYMENT_TYPE_DESCRIPTION AS DESCRIPTION, \'tbl_payment_type\' as TABLE_NAME FROM tbl_payment_type WHERE PAYMENT_TYPE_DESCRIPTION like @SearchText";
			try
			{
				talentSqlAccessDetail.Settings = settings;
				talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
				talentSqlAccessDetail.CommandElements.CommandText = strQuery;
				talentSqlAccessDetail.CommandElements.CommandParameter.Clear();
				talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SearchText", "%" + searchText + "%"));
				err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005);
				if ((!(err.HasError)) && (!(talentSqlAccessDetail.ResultDataSet == null)))
				{
					paymentTypeData = talentSqlAccessDetail.ResultDataSet;
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
			return paymentTypeData;
		}
	}
}
