using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Data.SqlClient;
namespace TalentSystemDefaults
{
	/// <summary>
	/// Provides the functionality to execute the command object against the given connection string
	/// by using DBAccess Class
	/// </summary>
	public class DBDataAccess : DBAccess
	{
		#region Class Level Fields
		private const string CLASSNAME = "DBDataAccess";
		private DESQLCommand _commandElement;
		const string ERROR_COMMANDTYPE = "Command type is empty or Invalid type";
		const string ERROR_COMMANDTEXT = "Command text is empty or Having invalid characters";
		const string ERROR_COMMANDPARAMETERNAME = "Parameter name is empty or Having invalid characters";
		const string ERROR_COMMANDPARAMETERTYPE = "Parameter type is empty or Having invalid characters";
		const string ERROR_COMMANDPARAMETERTYPEVALUE = "Parameter value is not matching with type or Having invalid characters";
		#endregion
		#region Protected Fields
		protected string businessUnit = string.Empty;
		#endregion
		#region Private Fields
		#endregion
		#region Properties
		/// <summary>
		/// Sets the command element of type DESQLCommand
		/// </summary>
		/// <value>The command element.</value>
		public DESQLCommand CommandElement
		{
			set
			{
				_commandElement = value;
			}
			get
			{
				return _commandElement;
			}
		}
		#endregion
		#region Public Methods

		/// <summary>
		/// Constructor
		/// </summary>
		public DBDataAccess()
		{
			logSourceClass = this.GetType().BaseType.Name;
		}
		/// <summary>
		/// Opens the connection and create and returns the transaction object
		/// </summary>
		/// <param name="err">The error object as ref</param>
		/// <param name="givenIsolationLevel">The given isolation level.</param>
		/// <returns>SQLTransaction instance</returns>
		public SqlTransaction BeginTransaction(DestinationDatabase destinationDatabase, ref ErrorObj err, IsolationLevel? givenIsolationLevel = null)
		{
			SqlTransaction SqlTrans = null;
			err = ConnectionByDestinationDBOpen(destinationDatabase);
			if (!err.HasError)
			{
				if (givenIsolationLevel == null)
				{
					SqlTrans = conSql2005.BeginTransaction();
				}
				else
				{
					SqlTrans = conSql2005.BeginTransaction((IsolationLevel)givenIsolationLevel);
				}
			}
			else
			{
				SqlTrans = null;
			}
			return SqlTrans;
		}
		/// <summary>
		/// Ends the transaction and disposes any unhandled transaction object
		/// </summary>
		/// <param name="err">The error object as ref</param>
		/// <param name="givenTransaction">The given transaction.</param>
		public void EndTransaction(DestinationDatabase destinationDatabase, ref ErrorObj err, SqlTransaction givenTransaction)
		{
			if (!(givenTransaction.Connection == null))
			{
				givenTransaction.Dispose();
				givenTransaction = null;
			}
			err = ConnectionByDestinationDBClose(destinationDatabase);
		}
		/// <summary>
		/// Ends the transaction and disposes any unhandled transaction object and closes the reader object
		/// </summary>
		/// <param name="err">The error object as ref</param>
		/// <param name="givenTransaction">The given transaction.</param>
		/// <param name="readerToClose">The reader to close.</param>
		public void EndTransaction(DestinationDatabase destinationDatabase, ref ErrorObj err, SqlTransaction givenTransaction, SqlDataReader readerToClose)
		{
			if (!(givenTransaction.Connection == null))
			{
				givenTransaction.Dispose();
				givenTransaction = null;
			}
			if (!(readerToClose == null))
			{
				if (!(readerToClose.IsClosed))
				{
					readerToClose.Close();
				}
				readerToClose.Dispose();
			}
			err = ConnectionByDestinationDBClose(destinationDatabase);
		}
		/// <summary>
		/// Execute the command obejct with transaction
		/// </summary>
		/// <param name="givenTransaction">The given transaction.</param>
		/// <returns>Error Object</returns>
		public ErrorObj AccessWithTransaction(SqlTransaction givenTransaction)
		{
			ErrorObj err = new ErrorObj();
			SqlCommand SqlCommandEntity = new SqlCommand();
			SqlCommandEntity.Transaction = givenTransaction;
			SqlCommandEntity.Connection = givenTransaction.Connection;
			err = PrepareCommandElement(SqlCommandEntity);
			if (!(err.HasError))
			{
				try
				{
					if (_commandElement.CommandExecutionType == CommandExecution.ExecuteDataSet)
					{
						this.ResultDataSet = ExecuteDataSet(SqlCommandEntity);
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteNonQuery)
					{
						this.ResultDataSet = ExecuteNonQuery(SqlCommandEntity);
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteReader)
					{
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteScalar)
					{
					}
				}
				catch (SqlException sqlEx)
				{
					err.HasError = true;
					err.ErrorMessage = sqlEx.Message + "; Err Code:" + System.Convert.ToString(sqlEx.ErrorCode) + "; Line:" + System.Convert.ToString(sqlEx.LineNumber) + "; Name:" + sqlEx.Procedure;
					Utilities.InsertErrorLog(Settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, Settings.BusinessUnit, logFilter2, logFilter3, logFilter4, sqlEx.StackTrace.ToString(), err.ErrorMessage);
				}
				catch (Exception ex)
				{
					err.HasError = true;
					err.ErrorMessage = ex.Message;
					Utilities.InsertErrorLog(Settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, Settings.BusinessUnit, logFilter2, logFilter3, logFilter4, ex.StackTrace.ToString(), err.ErrorMessage);
				}
			}
			return err;
		}
		#endregion
		#region Protected Methods
		/// <summary>
		/// Access the data base SQL 2005
		/// This is called by DBAccess Class
		/// </summary>
		/// <returns></returns>
		protected override ErrorObj AccessDataBaseSQL2005()
		{
			ErrorObj err = new ErrorObj();
			SqlCommand SqlCommandEntity = new SqlCommand();
			SqlCommandEntity.Connection = conSql2005;
			try
			{
				err = PrepareCommandElement(SqlCommandEntity);
				if (!(err.HasError))
				{
					if (_commandElement.CommandExecutionType == CommandExecution.ExecuteDataSet)
					{
						this.ResultDataSet = ExecuteDataSet(SqlCommandEntity);
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteNonQuery)
					{
						this.ResultDataSet = ExecuteNonQuery(SqlCommandEntity);
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteReader)
					{
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteScalar)
					{
					}
				}
			}
			catch (SqlException ex)
			{
				err.HasError = true;
				err.ErrorMessage = ex.Message;
				if (_commandElement.CommandType == CommandType.StoredProcedure)
				{
					err.ErrorMessage = err.ErrorMessage + " (Server=" + ex.Server.ToString() + ", Procedure=" + ex.Procedure.ToString() + ", LineNumber=" + ex.LineNumber.ToString() + ")";
				}
				Utilities.InsertErrorLog(Settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, Settings.BusinessUnit, logFilter2, logFilter3, logFilter4, ex.StackTrace.ToString(), err.ErrorMessage);
			}
			catch (Exception ex)
			{
				err.HasError = true;
				err.ErrorMessage = ex.Message;
				Utilities.InsertErrorLog(Settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, Settings.BusinessUnit, logFilter2, logFilter3, logFilter4, ex.StackTrace.ToString(), err.ErrorMessage);
			}
			return err;
		}
		/// <summary>
		///  XML Comment
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		protected override ErrorObj AccessDataBaseTalentSQL()
		{
			ErrorObj err = new ErrorObj();
			SqlCommand SqlCommandEntity = new SqlCommand();
			SqlCommandEntity.Connection = conSql2005;
			try
			{
				err = PrepareCommandElement(SqlCommandEntity);
				if (!(err.HasError))
				{
					if (_commandElement.CommandExecutionType == CommandExecution.ExecuteDataSet)
					{
						this.ResultDataSet = ExecuteDataSet(SqlCommandEntity);
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteNonQuery)
					{
						this.ResultDataSet = ExecuteNonQuery(SqlCommandEntity);
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteReader)
					{
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteScalar)
					{
					}
				}
			}
			catch (SqlException ex)
			{
				err.HasError = true;
				err.ErrorMessage = ex.Message;
				if (_commandElement.CommandType == CommandType.StoredProcedure)
				{
					err.ErrorMessage = err.ErrorMessage + " (Server=" + ex.Server.ToString() + ", Procedure=" + ex.Procedure.ToString() + ", LineNumber=" + ex.LineNumber.ToString() + ")";
				}
				Utilities.InsertErrorLog(Settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, Settings.BusinessUnit, logFilter2, logFilter3, logFilter4, ex.StackTrace.ToString(), err.ErrorMessage);
				//TalentLogger.Logging(CLASSNAME, "AccessDataBaseTalentConfig", "SQL Exception Occured" & GetStackFrameDetails(), err, ex, LogTypeConstants.TCDBDATAOBJECTSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
			}
			catch (Exception ex)
			{
				err.HasError = true;
				err.ErrorMessage = ex.Message;
				Utilities.InsertErrorLog(Settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, Settings.BusinessUnit, logFilter2, logFilter3, logFilter4, ex.StackTrace.ToString(), err.ErrorMessage);
				//TalentLogger.Logging(CLASSNAME, "AccessDataBaseTalentConfig", "Exception Occured" & GetStackFrameDetails(), err, ex, LogTypeConstants.TCDBDATAOBJECTSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
			}
			return err;
		}
		#endregion
		#region Private Methods
		/// <summary>
		///  XML Comment
		/// </summary>
		/// <param name="destinationDatabase"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private ErrorObj ConnectionByDestinationDBOpen(DestinationDatabase destinationDatabase)
		{
			ErrorObj err = new ErrorObj();
			if (destinationDatabase == TalentSystemDefaults.DestinationDatabase.SQL2005)
			{
				err = Sql2005Open();
			}
			else if (destinationDatabase == TalentSystemDefaults.DestinationDatabase.TALENT_CONFIG |
				destinationDatabase == TalentSystemDefaults.DestinationDatabase.TALENT_DEFINITION)
			{
				err = TalentSQLOpen();
			}
			return err;
		}
		/// <summary>
		///  XML Comment
		/// </summary>
		/// <param name="destinationDatabase"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private ErrorObj ConnectionByDestinationDBClose(DestinationDatabase destinationDatabase)
		{
			ErrorObj err = new ErrorObj();
			if (destinationDatabase == TalentSystemDefaults.DestinationDatabase.SQL2005)
			{
				err = Sql2005Close();
			}
			else if (destinationDatabase == TalentSystemDefaults.DestinationDatabase.TALENT_CONFIG |
				destinationDatabase == TalentSystemDefaults.DestinationDatabase.TALENT_DEFINITION)
			{
				err = TalentSQLClose();
			}
			return err;
		}
		/// <summary>
		/// Executes the command object using SQLDataAdapter to fill the dataset and return
		/// </summary>
		/// <param name="commandToExecute">The command to execute.</param>
		/// <returns>DataSet</returns>
		private DataSet ExecuteDataSet(SqlCommand commandToExecute)
		{
			DataSet outputDataSet = null;
			using (SqlDataAdapter SqlDataAdapterExecuter = new SqlDataAdapter(commandToExecute))
			{
				outputDataSet = new DataSet();
				SqlDataAdapterExecuter.Fill(outputDataSet);
			}
			return outputDataSet;
		}
		/// <summary>
		/// Executes the command object using executenonquery method and
		/// returns a dataset with a table with single column and row ROWS_AFFECTED
		/// </summary>
		/// <param name="commandToExecute">The command to execute.</param>
		/// <returns>A dataset with a table with single column and row ROWS_AFFECTED</returns>
		private DataSet ExecuteNonQuery(SqlCommand commandToExecute)
		{
			//Utilities.LogSQL(commandToExecute)
			DataSet outputDataSet = null;
			int rowsAffected = commandToExecute.ExecuteNonQuery();
			DataTable tempDataTable = new DataTable();
			tempDataTable.Columns.Add("ROWS_AFFECTED", typeof(string));
			DataRow tempDataRow = null;
			tempDataRow = tempDataTable.NewRow();
			tempDataRow["ROWS_AFFECTED"] = rowsAffected;
			tempDataTable.Rows.Add(tempDataRow);
			outputDataSet = new DataSet();
			outputDataSet.Tables.Add(tempDataTable);
			tempDataTable = null;
			return outputDataSet;
		}
		/// <summary>
		/// Prepares the command element
		/// Assign the command instance properties from DESQLCommand instance
		/// </summary>
		/// <param name="givenSqlCommand">The given SQL command.</param>
		/// <returns>ErrorObj</returns>
		private ErrorObj PrepareCommandElement(SqlCommand givenSqlCommand)
		{
			ErrorObj err = new ErrorObj();
			if ((!(err.HasError)) && HasValue(_commandElement.CommandText))
			{
				givenSqlCommand.CommandText = _commandElement.CommandText;
			}
			else
			{
				err.ErrorMessage = ERROR_COMMANDTEXT;
				err.HasError = true;
			}
			if ((!(err.HasError)) && HasValue(_commandElement.CommandType))
			{
				givenSqlCommand.CommandType = _commandElement.CommandType;
			}
			else
			{
				err.ErrorMessage = ERROR_COMMANDTYPE;
				err.HasError = true;
			}
			if (!(err.HasError))
			{
				err = AttachParametersToCommand(givenSqlCommand);
			}
			return err;
		}
		/// <summary>
		/// Attaches the parameters to the given SQL command instance
		/// </summary>
		/// <param name="givenSqlCommand">The given SQL command.</param>
		/// <returns></returns>
		private ErrorObj AttachParametersToCommand(SqlCommand givenSqlCommand)
		{
			ErrorObj err = new ErrorObj();
			if (!(_commandElement.CommandParameter == null))
			{
				if (_commandElement.CommandParameter.Count > 0)
				{
					System.Collections.Generic.List<DESQLParameter> paramaterList = _commandElement.CommandParameter;
					DESQLParameter parameterPiece = default(DESQLParameter);
					foreach (DESQLParameter tempLoopVar_parameterPiece in paramaterList)
					{
						parameterPiece = tempLoopVar_parameterPiece;
						SqlParameter tempSqlParameter = new SqlParameter();
						if ((!(err.HasError)) && HasValue(parameterPiece.ParamName))
						{
							tempSqlParameter.ParameterName = parameterPiece.ParamName;
						}
						else
						{
							err.HasError = true;
							err.ErrorMessage = ERROR_COMMANDPARAMETERNAME;
							break;
						}
						//If paramValue is nothing considering it as DBNull.Value
						if (parameterPiece.ParamValue == null)
						{
							tempSqlParameter.Value = DBNull.Value;
						}
						else
						{
							if ((!(err.HasError)) && HasValue(parameterPiece.ParamValue, parameterPiece.ParamType))
							{
								tempSqlParameter.SqlDbType = parameterPiece.ParamType;
								tempSqlParameter.Value = parameterPiece.ParamValue;
							}
							else
							{
								err.HasError = true;
								err.ErrorMessage = ERROR_COMMANDPARAMETERTYPEVALUE;
								break;
							}
						}
						if (!(err.HasError))
						{
							if (parameterPiece.ParamDirection == null)
							{
								parameterPiece.ParamDirection = ParameterDirection.Input;
							}
							tempSqlParameter.Direction = parameterPiece.ParamDirection;
						}
						if (!(err.HasError))
						{
							givenSqlCommand.Parameters.Add(tempSqlParameter);
						}
						tempSqlParameter = null;
					}
					parameterPiece = null;
					//tempSqlParameter = Nothing
				}
			}
			return err;
		}
		/// <summary>
		/// Determines whether the specified given value has value.
		/// </summary>
		/// <param name="givenValueToCheck">The given value to check.</param>
		/// <returns>
		/// <c>true</c> if the specified given value to check has value; otherwise, <c>false</c>.
		/// </returns>
		private bool HasValue(string givenValueToCheck)
		{
			bool tempIsExists = false;
			if ((!(givenValueToCheck == null)) && (givenValueToCheck.Length > 0))
			{
				tempIsExists = true;
			}
			else
			{
				tempIsExists = false;
			}
			return tempIsExists;
		}
		/// <summary>
		/// Determines whether the specified given type to has any CommandType.
		/// </summary>
		/// <param name="givenTypeToCheck">The given type to check.</param>
		/// <returns>
		/// <c>true</c> if the specified given type has value; otherwise, <c>false</c>.
		/// </returns>
		private bool HasValue(CommandType givenTypeToCheck)
		{
			bool tempIsExists = false;
			if (!(givenTypeToCheck == null))
			{
				tempIsExists = true;
			}
			else
			{
				tempIsExists = false;
			}
			return tempIsExists;
		}
		/// <summary>
		/// Determines whether the specified given value is matching with given SQLDbType.
		/// </summary>
		/// <param name="givenValueToCheck">The given value to check.</param>
		/// <param name="givenTypeToCheck">The given type to check.</param>
		/// <returns>
		/// <c>true</c> if the specified given value is matching with given SQLDbType; otherwise, <c>false</c>.
		/// </returns>
		private bool HasValue(object givenValueToCheck, SqlDbType givenTypeToCheck)
		{
			bool tempIsExists = false;
			if (givenValueToCheck.Equals(DBNull.Value))
			{
				tempIsExists = true;
			}
			else
			{
				switch (givenTypeToCheck)
				{
					case SqlDbType.VarChar:
						if (!(givenValueToCheck == null))
						{
							tempIsExists = true;
						}
						else if (givenValueToCheck.Equals(string.Empty))
						{
							tempIsExists = true;
						}
						else
						{
							tempIsExists = false;
						}
						break;
					case SqlDbType.NVarChar:
						if (!(givenValueToCheck == null))
						{
							tempIsExists = true;
						}
						else if (givenValueToCheck.Equals(string.Empty))
						{
							tempIsExists = true;
						}
						else
						{
							tempIsExists = false;
						}
						break;
					//IsNumeric
					case SqlDbType.Int:
						if (!(givenValueToCheck == null))
						{
							if (Information.IsNumeric(givenValueToCheck))
							{
								tempIsExists = true;
							}
							else
							{
								tempIsExists = false;
							}
						}
						else
						{
							tempIsExists = false;
						}
						break;
					case SqlDbType.Decimal:
						if (!(givenValueToCheck == null))
						{
							if (Information.IsNumeric(givenValueToCheck))
							{
								tempIsExists = true;
							}
							else
							{
								tempIsExists = false;
							}
						}
						else
						{
							tempIsExists = false;
						}
						break;
					case SqlDbType.Float:
						if (!(givenValueToCheck == null))
						{
							if (Information.IsNumeric(givenValueToCheck))
							{
								tempIsExists = true;
							}
							else
							{
								tempIsExists = false;
							}
						}
						else
						{
							tempIsExists = false;
						}
						break;
					case SqlDbType.TinyInt:
						if (!(givenValueToCheck == null))
						{
							if (Information.IsNumeric(givenValueToCheck))
							{
								tempIsExists = true;
							}
							else
							{
								tempIsExists = false;
							}
						}
						else
						{
							tempIsExists = false;
						}
						break;
					case SqlDbType.SmallInt:
						if (!(givenValueToCheck == null))
						{
							if (Information.IsNumeric(givenValueToCheck))
							{
								tempIsExists = true;
							}
							else
							{
								tempIsExists = false;
							}
						}
						else
						{
							tempIsExists = false;
						}
						break;
					case SqlDbType.BigInt:
						if (!(givenValueToCheck == null))
						{
							if (Information.IsNumeric(givenValueToCheck))
							{
								tempIsExists = true;
							}
							else
							{
								tempIsExists = false;
							}
						}
						else
						{
							tempIsExists = false;
						}
						break;
					//IsDate
					case SqlDbType.Date:
						if (!(givenValueToCheck == null))
						{
							if (Information.IsDate(givenValueToCheck))
							{
								tempIsExists = true;
							}
							else
							{
								tempIsExists = false;
							}
						}
						else
						{
							tempIsExists = false;
						}
						break;
					case SqlDbType.DateTime:
						if (!(givenValueToCheck == null))
						{
							if (Information.IsDate(givenValueToCheck))
							{
								tempIsExists = true;
							}
							else
							{
								tempIsExists = false;
							}
						}
						else
						{
							tempIsExists = false;
						}
						break;
					case SqlDbType.SmallDateTime:
						if (!(givenValueToCheck == null))
						{
							if (Information.IsDate(givenValueToCheck))
							{
								tempIsExists = true;
							}
							else
							{
								tempIsExists = false;
							}
						}
						else
						{
							tempIsExists = false;
						}
						break;
					//Case SqlDbType.Binary
					case SqlDbType.Bit:
						tempIsExists = true;
						break;
					//Case SqlDbType.Char
					//Case SqlDbType.DateTime2
					//Case SqlDbType.DateTimeOffset
					//Case SqlDbType.Image
					//Case SqlDbType.Money
					//Case SqlDbType.NChar
					//Case SqlDbType.NText
					//Case SqlDbType.NVarChar
					//Case SqlDbType.Real
					//Case SqlDbType.SmallMoney
					//Case SqlDbType.Structured
					//Case SqlDbType.Text
					//Case SqlDbType.Time
					//Case SqlDbType.Timestamp
					//Case SqlDbType.Udt
					//Case SqlDbType.UniqueIdentifier
					case SqlDbType.VarBinary:
						if (!(givenValueToCheck == null))
						{
							tempIsExists = true;
						}
						else
						{
							tempIsExists = false;
						}
						break;
					//Case SqlDbType.Variant
					case SqlDbType.Xml:
						if (!(givenValueToCheck == null))
						{
							tempIsExists = true;
						}
						else
						{
							tempIsExists = false;
						}
						break;
					default:
						if (!(givenValueToCheck == null))
						{
							tempIsExists = true;
						}
						else
						{
							tempIsExists = false;
						}
						break;
				}
			}
			return tempIsExists;
		}
		#endregion
	}
}
