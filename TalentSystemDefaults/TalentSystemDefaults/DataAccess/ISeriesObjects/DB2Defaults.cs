using IBM.Data.DB2.iSeries;
using System;
using System.Data;
namespace TalentSystemDefaults
{
	public class DB2Defaults : DBDB2Access
	{
		#region Class Level Fields
		private const string UpdateDB2Data = "UpdateDB2Data";
		#endregion
		#region Public Properties
		public DEDB2Defaults DEDB2Defaults { get; set; }
		#endregion
		#region Public Function
		protected override ErrorObj AccessDataBaseTALENTTKT()
		{
			ErrorObj err = new ErrorObj();
			switch (_settings.FunctionName)
			{
				case UpdateDB2Data:
					err = AccessDatabaseTD001S();
					break;
			}
			return err;
		}
		#endregion
		#region Private Function
		private ErrorObj AccessDatabaseTD001S()
		{
			ErrorObj err = new ErrorObj();
			DataSet ResultDataSet = new DataSet();
			//Create the Status data table
			DataTable DtStatusResults = new DataTable("Error Status");
			ResultDataSet.Tables.Add(DtStatusResults);
			DtStatusResults.Columns.Add("ErrorOccurred", typeof(string));
			DtStatusResults.Columns.Add("ReturnCode", typeof(string));
			//Create the Attribute Definition data table
			DataTable DtDB2Defaults = new DataTable("DB2Defaults");
			ResultDataSet.Tables.Add(DtDB2Defaults);
			DtDB2Defaults.Columns.Add("AffectedRows", typeof(string));
			try
			{
				iDB2Command cmd = conTALENTTKT.CreateCommand();
				cmd.CommandText = "Call TD001S(@PARAM0,@PARAM1,@PARAM2,@PARAM3)";
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 0;
				//pErrorCode
				iDB2Parameter parmIO_0;
				parmIO_0 = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10);
				parmIO_0.Value = "";
				parmIO_0.Direction = ParameterDirection.InputOutput;
				//pSource
				iDB2Parameter parmIO_1;
				parmIO_1 = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1);
				parmIO_1.Value = "W";
				parmIO_1.Direction = ParameterDirection.Input;
				//pXML
				iDB2Parameter parmIO_2;
				parmIO_2 = cmd.Parameters.Add(Param2, iDB2DbType.iDB2VarChar, 32739);
				parmIO_2.Value = DEDB2Defaults.XMLString;
				parmIO_2.Direction = ParameterDirection.Input;
				//pRowCount
				iDB2Parameter parmIO_3;
				parmIO_3 = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Integer);
				parmIO_3.Value = 0;
				parmIO_3.Direction = ParameterDirection.InputOutput;
				IBM.Data.DB2.iSeries.iDB2DataAdapter cmdAdapter = new IBM.Data.DB2.iSeries.iDB2DataAdapter();
				cmdAdapter.SelectCommand = cmd;
				cmdAdapter.Fill(ResultDataSet, "DB2Defaults");
				DataRow drStatus = DtStatusResults.NewRow();
				if ((cmd.Parameters[0].Value).ToString().Trim().Length > 0)
				{
					drStatus["ErrorOccurred"] = (cmd.Parameters[0].Value).ToString().Trim();
					drStatus["ReturnCode"] = "E";
				}
				DtStatusResults.Rows.Add(drStatus);
				DataRow drAffectedRows = DtDB2Defaults.NewRow();
				if ((cmd.Parameters[3].Value).ToString().Trim().Length > 0)
				{
					var affectedRows = (cmd.Parameters[3].Value).ToString().Trim();
					drAffectedRows["AffectedRows"] = affectedRows;
				}
				DtDB2Defaults.Rows.Add(drAffectedRows);
				this.ResultDataSet = ResultDataSet;
			}
			catch (Exception ex)
			{
				const string strError8 = "Error during database Access";
				err.ErrorMessage = ex.Message;
				err.ErrorStatus = strError8;
				err.ErrorNumber = "TACDBPD-TD001S";
				err.HasError = true;
			}
			return err;
		}
		#endregion
	}
}
