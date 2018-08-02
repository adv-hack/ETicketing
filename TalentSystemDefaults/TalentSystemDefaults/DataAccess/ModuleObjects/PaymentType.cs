using System;
using System.Collections.Generic;
using System.Data;
using TalentSystemDefaults.DataAccess.DataObjects;
namespace TalentSystemDefaults
{
	namespace DataAccess.ModuleObjects
	{
		public class PaymentType : DBObjectBase
		{
			#region Pay Type Code SQL And DB2 Relation Dictionary
			private static Dictionary<string, string> _payTypeCodeSQLAndDB2 = null;
			public static Dictionary<string, string> PayTypeCodeSQLAndDB2
			{
				get
				{
					return GetPayTypeCodeSQLAndDB2();
				}
			}
			public static Dictionary<string, string> GetPayTypeCodeSQLAndDB2()
			{
				if (_payTypeCodeSQLAndDB2 == null)
				{
					_payTypeCodeSQLAndDB2 = new Dictionary<string, string>();
					_payTypeCodeSQLAndDB2.Add("CS", "1");
					_payTypeCodeSQLAndDB2.Add("ZB", "1");
					_payTypeCodeSQLAndDB2.Add("CQ", "2");
					_payTypeCodeSQLAndDB2.Add("CHQ", "2");
					_payTypeCodeSQLAndDB2.Add("CC", "3");
					_payTypeCodeSQLAndDB2.Add("GC", "3");
					_payTypeCodeSQLAndDB2.Add("VG", "3");
					_payTypeCodeSQLAndDB2.Add("PP", "3");
					_payTypeCodeSQLAndDB2.Add("CP", "3");
					_payTypeCodeSQLAndDB2.Add("DD", "4");
					_payTypeCodeSQLAndDB2.Add("LN", "5");
					_payTypeCodeSQLAndDB2.Add("PO", "6");
					_payTypeCodeSQLAndDB2.Add("PD", "13");
					_payTypeCodeSQLAndDB2.Add("EP", "33");
				}
				return _payTypeCodeSQLAndDB2;
			}
			#endregion
			#region Class Level Fields
			private const string MD501_TYPE_CODE = "PAYT";
			#endregion
			#region Public Methods
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="settings"></param>
			/// <remarks></remarks>
			public PaymentType(ref DESettings settings)
				: base(settings)
			{
				businessUnit = settings.BusinessUnit;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <returns></returns>
			/// <remarks></remarks>
			public override DataTable RetrieveDTForList()
			{
				DataTable dtForList = null;
				dtForList = SelectDTMergedPaymentTypes(SelectSQLPaymentTypes(), SelectDB2PaymentTypes());
				return dtForList;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="dtFromSQL"></param>
			/// <param name="dtFromDB2"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public DataTable SelectDTMergedPaymentTypes(DataTable dtFromSQL, DataTable dtFromDB2)
			{
				DataTable dtMerged = new DataTable();
				dtMerged.Columns.Add("PAYMENT_TYPE_ID", typeof(string));
				dtMerged.Columns.Add("PAYMENT_TYPE_CODE", typeof(string));
				dtMerged.Columns.Add("PAYMENT_TYPE_DESCRIPTION", typeof(string));
				dtMerged.Columns.Add("IS_OTHER_TYPE", typeof(string));
				dtMerged.Columns.Add("DEFAULT_PAYMENT_TYPE", typeof(string));
				dtMerged.Columns.Add("CODE51", typeof(string));
				dtMerged.Columns.Add("VALU51", typeof(string));
				if (dtFromSQL != null && dtFromSQL.Rows.Count > 0)
				{
					DataRow dr = null;
					for (int rowindex = 0; rowindex <= dtFromSQL.Rows.Count - 1; rowindex++)
					{
						dr = dtMerged.NewRow();
						dr["PAYMENT_TYPE_ID"] = Utilities.CheckForDBNull_String(dtFromSQL.Rows[rowindex]["PAYMENT_TYPE_ID"]);
						dr["PAYMENT_TYPE_CODE"] = Utilities.CheckForDBNull_String(dtFromSQL.Rows[rowindex]["PAYMENT_TYPE_CODE"]);
						dr["PAYMENT_TYPE_DESCRIPTION"] = Utilities.CheckForDBNull_String(dtFromSQL.Rows[rowindex]["PAYMENT_TYPE_DESCRIPTION"]);
						dr["IS_OTHER_TYPE"] = Utilities.CheckForDBNull_String(dtFromSQL.Rows[rowindex]["IS_OTHER_TYPE"]);
						dr["DEFAULT_PAYMENT_TYPE"] = Utilities.CheckForDBNull_String(dtFromSQL.Rows[rowindex]["DEFAULT_PAYMENT_TYPE"]);
						dr["VALU51"] = GetDB2RelatedPayCode(Utilities.CheckForDBNull_String(dtFromSQL.Rows[rowindex]["PAYMENT_TYPE_CODE"]).ToUpper(), dtFromDB2);
						dtMerged.Rows.Add(dr);
						dr = null;
					}
				}
				return dtMerged;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="sqlPayTypeCode"></param>
			/// <param name="dtFromDB2"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public string GetDB2RelatedPayCode(string sqlPayTypeCode, DataTable dtFromDB2)
			{
				string db2PayTypeCode = string.Empty;

				if (PayTypeCodeSQLAndDB2.ContainsKey(sqlPayTypeCode))
				{
					db2PayTypeCode = System.Convert.ToString(PayTypeCodeSQLAndDB2[sqlPayTypeCode]);
				}
				if (string.IsNullOrWhiteSpace(db2PayTypeCode))
				{
					if (dtFromDB2 != null && dtFromDB2.Rows.Count > 0)
					{
						for (int rowindex = 0; rowindex <= dtFromDB2.Rows.Count - 1; rowindex++)
						{
							if (Utilities.CheckForDBNull_String(dtFromDB2.Rows[rowindex]["CODE51"]).Trim().ToUpper() == sqlPayTypeCode)
							{
								db2PayTypeCode = Utilities.CheckForDBNull_String(dtFromDB2.Rows[rowindex]["VALU51"]).Trim().ToUpper();
								if (db2PayTypeCode == "0")
								{
									db2PayTypeCode = "";
								}
								break;
							}
						}
					}
				}
				return db2PayTypeCode;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <returns></returns>
			/// <remarks></remarks>
			public DataTable SelectSQLPaymentTypes()
			{
				string sqlStatement = "SELECT tp.PAYMENT_TYPE_ID, tp.PAYMENT_TYPE_CODE, tp.PAYMENT_TYPE_DESCRIPTION, tp.IS_OTHER_TYPE, " +
					" tpbu.DEFAULT_PAYMENT_TYPE " +
					" FROM tbl_payment_type tp " +
					" LEFT OUTER JOIN tbl_payment_type_bu tpbu ON tpbu.PAYMENT_TYPE_CODE = tp.PAYMENT_TYPE_CODE AND tpbu.BUSINESS_UNIT = @BusinessUnit ";
				DataTable outputDataTable = new DataTable();
				TalentDataAccess talentSqlAccessDetail = new TalentDataAccess();
				try
				{
					//Construct The Call
					talentSqlAccessDetail.Settings = settings;
					talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet;
					ErrorObj err = new ErrorObj();
					talentSqlAccessDetail.CommandElements.CommandText = sqlStatement;
					talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit));
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
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <returns></returns>
			/// <remarks></remarks>
			public DataTable SelectDB2PaymentTypes()
			{
				//how to get company code, type code and active flag
				MD501 md501Obj = new MD501(ref settings);
				DataTable outputDataTable = md501Obj.SelectByCompAndType(settings.Company, MD501_TYPE_CODE);
				return outputDataTable;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public override int DeleteCurrentValues(System.Data.SqlClient.SqlTransaction givenTransaction = null)
			{
				int affectedRows = 0;
				tbl_payment_type tblPaymentType = new tbl_payment_type(ref settings);
				tbl_payment_type_bu tblPaymentTypeBu = new tbl_payment_type_bu(ref settings);
				affectedRows = tblPaymentType.DeleteCurrentValues();
				affectedRows += tblPaymentTypeBu.DeleteCurrentValues();
				MD501 dataObj = new MD501(ref settings);
				affectedRows += dataObj.DeleteData("PAYT", settings.VariableKey1);
				return affectedRows;
			}
			#endregion
		}
	}
}
