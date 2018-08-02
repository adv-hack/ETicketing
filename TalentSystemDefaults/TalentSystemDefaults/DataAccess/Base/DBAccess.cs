using IBM.Data.DB2.iSeries;
using System;
using System.Data;
using System.Data.SqlClient;
namespace TalentSystemDefaults
{
	[Serializable()]
	public class DBAccess
	{
		#region Class Level Fields
		private const string CLASSNAME = "DBAccess";
		protected SqlConnection conSql2005 = null;
		protected iDB2Connection conTALENTTKT = null;
		protected const string SQL2005 = "SQL2005";
		protected const string TALENTTKT = "TALENTTKT";
		protected const string TALENTCONFIG = "TalentConfiguration";
		protected const string TALENTDEFINITIONS = "TalentDefinitions";
		protected DataSet _resultDataSet;
		protected DESettings _settings = new DESettings();
		protected bool _resetConnection = false;
		protected const string Param0 = "@Param0";
		protected const string Param1 = "@Param1";
		protected const string Param2 = "@Param2";
		protected const string Param3 = "@Param3";
		protected const string Param4 = "@Param4";
		protected const string Param5 = "@Param5";
		protected const string Param6 = "@Param6";
		protected const string Param7 = "@Param7";
		protected const string Param8 = "@Param8";
		protected const string Param9 = "@Param9";
		protected const string Param10 = "@Param10";
		protected const string Param11 = "@Param11";
		protected const string Param12 = "@Param12";
		protected const string Param13 = "@Param13";
		protected const string Param14 = "@Param14";
		protected const string Param15 = "@Param15";
		protected const string Param16 = "@Param16";
		protected const string Param17 = "@Param17";
		protected const string Param18 = "@Param18";
		protected string logSourceClass = string.Empty;
		protected string logSourceMethod = string.Empty;
		protected string logCode = TalentSystemDefaults.GlobalConstants.LOG_ERROR_CODE;
		protected string logFilter1 = string.Empty;
		protected string logFilter2 = string.Empty;
		protected string logFilter3 = string.Empty;
		protected string logFilter4 = string.Empty;
		protected string logFilter5 = string.Empty;
		protected string logContent = string.Empty;
		#endregion
		#region Public Properties
		public bool ResetConnection
		{
			get
			{
				return _resetConnection;
			}
			set
			{
				_resetConnection = value;
			}
		}
		public DataSet ResultDataSet
		{
			get
			{
				return _resultDataSet;
			}
			set
			{
				_resultDataSet = value;
			}
		}
		public DESettings Settings
		{
			get
			{
				return _settings;
			}
			set
			{
				_settings = value;
			}
		}
		#endregion
		#region Protected Methods
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected ErrorObj Sql2005Open()
		{
			ErrorObj err = new ErrorObj();
			short iCounter = (short)0;
			//---------------------------------------------------------------------------------
			//   Attempt to open database
			//
			const string strError = "Could not establish connection to the SQL database";
			try
			{
				while (iCounter < 10)
				{
					if (conSql2005 == null)
					{
						conSql2005 = new SqlConnection(Settings.FrontEndConnectionString);
						conSql2005.Open();
					}
					else if (conSql2005.State != ConnectionState.Open)
					{
						conSql2005 = new SqlConnection(Settings.FrontEndConnectionString);
						conSql2005.Open();
					}
					//---------------------------------------------------------------------------------
					//   Possible the server needs to wake up so
					//
					if (conSql2005.State == ConnectionState.Open)
					{
						break;
					}
					else
					{
						iCounter++;
						if (iCounter > 10)
						{
							err.ErrorMessage = conSql2005.State.ToString();
							err.ErrorStatus = strError;
							err.ErrorNumber = "TACDBAC-P1a";
							err.HasError = true;
							break;
						}
						System.Threading.Thread.Sleep(2500); // Sleep for 2.5 seconds
					}
					//---------------------------------------------------------------------------------
				}
			}
			catch (Exception ex)
			{
				err.ErrorMessage = ex.Message;
				err.ErrorStatus = strError;
				err.ErrorNumber = "TACDBAC-P1b";
				err.HasError = true;
				Utilities.InsertErrorLog(Settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, Settings.BusinessUnit, err.ErrorNumber, logFilter3, logFilter4, ex.StackTrace.ToString(), err.ErrorMessage);
			}
			return err;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected ErrorObj Sql2005Close()
		{
			ErrorObj err = new ErrorObj();
			//------------------------------------------------------------------------
			//   Warning :   Using ErrorObj here when closing the DB does cause a
			//               problem as it will clear the err object when an actual
			//               error has occured elsewhere
			//------------------------------------------------------------------------
			try
			{
				if (!(conSql2005 == null))
				{
					if (conSql2005.State == ConnectionState.Open)
					{
						conSql2005.Close();
					}
				}
			}
			catch (Exception ex)
			{
				const string strError4 = "Failed to close the Sql2005 database connection";
				err.ErrorMessage = ex.Message;
				err.ErrorStatus = strError4;
				err.ErrorNumber = "TACDBAC-P2";
				err.HasError = true;
				Utilities.InsertErrorLog(Settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, Settings.BusinessUnit, err.ErrorNumber, logFilter3, logFilter4, ex.StackTrace.ToString(), err.ErrorMessage);
				return err;
			}
			return err;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected ErrorObj TALENTTKTOpen()
		{
			ErrorObj err = new ErrorObj();
			//---------------------------------------------------------------------------------
			//   Attempt to open database
			//
			string connectionString = string.Empty;
			connectionString = Settings.BackOfficeConnectionString;
			try
			{
				if (conTALENTTKT == null)
				{
					conTALENTTKT = new iDB2Connection(connectionString);
					conTALENTTKT.Open();
				}
				else if (conTALENTTKT.State != ConnectionState.Open)
				{
					conTALENTTKT = new iDB2Connection(connectionString);
					conTALENTTKT.Open();
				}
			}
			catch (Exception ex)
			{
				const string strError = "Could not establish connection to the TALENTTKT database";
				err.ErrorMessage = ex.Message;
				err.ErrorStatus = strError;
				err.ErrorNumber = "TACDBAC-P5";
				err.HasError = true;
				Utilities.InsertErrorLog(Settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, Settings.BusinessUnit, err.ErrorNumber, logFilter3, logFilter4, ex.StackTrace.ToString(), err.ErrorMessage);
			}
			return err;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected ErrorObj TALENTTKTClose()
		{
			ErrorObj err = new ErrorObj();
			//------------------------------------------------------------------------
			//   Warning :   Using ErrorObj here when closing the DB does cause a
			//               problem as it will clear the err object when an actual
			//               error has occured elsewhere
			//------------------------------------------------------------------------
			try
			{
				if (!(conTALENTTKT == null))
				{
					if (conTALENTTKT.State == ConnectionState.Open)
					{
						conTALENTTKT.Close();
					}
				}
				conTALENTTKT = null;
			}
			catch (Exception ex)
			{
				const string strError4 = "Failed to close the TALENTTKT database connection";
				err.ErrorMessage = ex.Message;
				err.ErrorStatus = strError4;
				err.ErrorNumber = "TACDBAC-P6";
				err.HasError = true;
				Utilities.InsertErrorLog(Settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, Settings.BusinessUnit, err.ErrorNumber, logFilter3, logFilter4, ex.StackTrace.ToString(), err.ErrorMessage);
				return err;
			}
			return err;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected virtual ErrorObj AccessDataBaseSQL2005()
		{
			ErrorObj err = new ErrorObj();
			//--------------------------------------------------------------------
			if (!err.HasError)
			{
			}
			//--------------------------------------------------------------------
			return err;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected ErrorObj TalentSQLOpen()
		{
			ErrorObj err = new ErrorObj();
			short iCounter = (short)0;
			const string strError = "Could not establish connection to the SQL database";
			try
			{
				while (iCounter < 10)
				{
					if (conSql2005 == null)
					{
						conSql2005 = new SqlConnection(Settings.BackOfficeConnectionString);
						conSql2005.Open();
					}
					else if (conSql2005.State != ConnectionState.Open)
					{
						conSql2005 = new SqlConnection(Settings.BackOfficeConnectionString);
						conSql2005.Open();
					}
					if (conSql2005.State == ConnectionState.Open)
					{
						break;
					}
					else
					{
						iCounter++;
						if (iCounter > 10)
						{
							err.ErrorMessage = conSql2005.State.ToString();
							err.ErrorStatus = strError;
							err.ErrorNumber = "TACDBAC-P2a";
							err.HasError = true;
							break;
						}
						System.Threading.Thread.Sleep(2500);
					}
				}
			}
			catch (Exception ex)
			{
				err.ErrorMessage = ex.Message;
				err.ErrorStatus = strError;
				err.ErrorNumber = "TACDBAC-P2b";
				err.HasError = true;
				Utilities.InsertErrorLog(Settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, Settings.BusinessUnit, err.ErrorNumber, logFilter3, logFilter4, ex.StackTrace.ToString(), err.ErrorMessage);
				//TalentLogger.Logging(CLASSNAME, "TalentAdminOpen", strError, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
			}
			return err;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected ErrorObj TalentSQLClose()
		{
			ErrorObj err = new ErrorObj();
			try
			{
				if (!(conSql2005 == null))
				{
					if (conSql2005.State == ConnectionState.Open)
					{
						conSql2005.Close();
					}
				}
			}
			catch (Exception ex)
			{
				const string strError4 = "Failed to close the Sql2005 database connection";
				err.ErrorMessage = ex.Message;
				err.ErrorStatus = strError4;
				err.ErrorNumber = "TACDBAC-P2c";
				err.HasError = true;
				Utilities.InsertErrorLog(Settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, Settings.BusinessUnit, err.ErrorNumber, logFilter3, logFilter4, ex.StackTrace.ToString(), err.ErrorMessage);
				return err;
				//TalentLogger.Logging(CLASSNAME, "TalentAdminClose", strError4, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
			}
			return err;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected virtual ErrorObj AccessDataBaseTalentSQL()
		{
			ErrorObj err = new ErrorObj();
			//--------------------------------------------------------------------
			if (!err.HasError)
			{
			}
			//--------------------------------------------------------------------
			return err;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected virtual ErrorObj AccessDataBaseTALENTTKT()
		{
			ErrorObj err = new ErrorObj();
			//--------------------------------------------------------------------
			if (!err.HasError)
			{
			}
			//--------------------------------------------------------------------
			return err;
		}
		/// <summary>
		/// Executes the command object using SQLDataAdapter to fill the dataset and return
		/// </summary>
		/// <param name="commandToExecute">The command to execute.</param>
		/// <returns>DataSet</returns>
		protected DataSet ExecuteDataSetForSQLDB(SqlCommand commandToExecute)
		{
			DataSet outputDataSet = null;
			SqlDataAdapter sqlAdapater = new SqlDataAdapter();
			using (SqlDataAdapter SqlDataAdapterExecuter = new SqlDataAdapter(commandToExecute))
			{
				outputDataSet = new DataSet();
				SqlDataAdapterExecuter.Fill(outputDataSet);
			}
			return outputDataSet;
		}
		#endregion
		#region Public Methods
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual ErrorObj AccessDatabase()
		{
			// Declare this first! Used for Logging function duration
			TimeSpan timeSpan = DateTime.Now.TimeOfDay;
			ErrorObj err = new ErrorObj();
			//------------------------------------------------------------------------
			//   Warning :   Using ErrorObj here when closing the DB does cause a
			//               problem as it will clear the err object when an actual
			//               error has occured in the Access Data
			//------------------------------------------------------------------------
			switch (Settings.DestinationDatabase)
			{
				case SQL2005:
					err = Sql2005Open();
					if (!err.HasError)
					{
						err = AccessDataBaseSQL2005();
					}
					Sql2005Close();
					break;
				case TALENTTKT:
					err = TALENTTKTOpen();
					if (!err.HasError)
					{
						err = AccessDataBaseTALENTTKT();
						//
						// Ignore certain errors
						if (err.HasError && !string.IsNullOrEmpty(Settings.IgnoreErrors) && Settings.IgnoreErrors.IndexOf(err.ErrorNumber.Trim()) + 1 > 0)
						{
							err = null;
						}
						else
						{
							//
							// Start RETRY
							//
							// If there is an error AND Retry is switched AND RetryAttempts is > 0 AND the
							// error number generated is defined to be retried then ...
							int intPos = 0;
							int intCount = 0;
							if (err.HasError & bool.Parse(Settings.RetryFailures) && int.Parse(Settings.RetryAttempts) > 0)
							{
								intPos = Settings.RetryErrorNumbers.IndexOf(err.ErrorNumber.Trim()) + 1;
								if (intPos > 0)
								{
									intCount = 1;
									while (intCount <= int.Parse(Settings.RetryAttempts) && err.HasError)
									{
										if (int.Parse(Settings.RetryWaitTime) > 0)
										{
											System.Threading.Thread.Sleep(System.Convert.ToInt32(Settings.RetryWaitTime));
										}
										err.ErrorMessage = string.Empty;
										err.ErrorStatus = string.Empty;
										err.ErrorNumber = string.Empty;
										err.HasError = false;
										err = AccessDataBaseTALENTTKT();
										intCount++;
									}
								}
							}
							// End   RETRY
							//
						}
						// End Ignore certain errors
						//
					}
					TALENTTKTClose();
					break;
				case TALENTCONFIG:
				case TALENTDEFINITIONS:
					err = TalentSQLOpen();
					if (!err.HasError)
					{
						err = AccessDataBaseTalentSQL();
					}
					TalentSQLClose();
					break;
			}
			return err;
		}
		#endregion
	}
}
