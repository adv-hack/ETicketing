using IBM.Data.DB2.iSeries;
using Microsoft.VisualBasic;
using System;
using System.Data;
namespace TalentSystemDefaults
{
	/// <summary>
	/// Provides the functionality to execute the command object against the given connection string
	/// by using DBAccess Class
	/// </summary>
	public class DBDB2Access : DBAccess
	{
		#region Class Level Fields
		private const string CLASSNAME = "DBDB2Access";
		private DEDB2Command _commandElement;
		const string ERROR_COMMANDTYPE = "Command type is empty or Invalid type";
		const string ERROR_COMMANDTEXT = "Command text is empty or Having invalid characters";
		const string ERROR_COMMANDPARAMETERNAME = "Parameter name is empty or Having invalid characters";
		const string ERROR_COMMANDPARAMETERTYPE = "Parameter type is empty or Having invalid characters";
		const string ERROR_COMMANDPARAMETERTYPEVALUE = "Parameter value is not matching with type or Having invalid characters";
		#endregion
		#region Protected Fields
		protected string businessUnit = string.Empty;
		#endregion
		#region Properties
		/// <summary>
		/// Sets the command element of type DEDB2Command
		/// </summary>
		/// <value>The command element.</value>
		public DEDB2Command CommandElement
		{
			set
			{
				_commandElement = value;
			}
		}
		#endregion
		#region Public Methods
		/// <summary>
		/// Opens the connection and create and returns the transaction object
		/// </summary>
		/// <param name="err">The error object as ref</param>
		/// <param name="givenIsolationLevel">The given isolation level.</param>
		/// <returns>iDB2Transaction instance</returns>
		public iDB2Transaction BeginTransaction(DestinationDatabase destinationDatabase, ref ErrorObj err, IsolationLevel? givenIsolationLevel = null)
		{
			iDB2Transaction db2Trans = null;
			err = ConnectionByDestinationDBOpen(destinationDatabase);
			if (!err.HasError)
			{
				if (givenIsolationLevel == null)
				{
					db2Trans = conTALENTTKT.BeginTransaction();
				}
				else
				{
					db2Trans = conTALENTTKT.BeginTransaction((IsolationLevel)givenIsolationLevel);
				}
			}
			else
			{
				db2Trans = null;
			}
			return db2Trans;
		}
		/// <summary>
		/// Ends the transaction and disposes any unhandled transaction object
		/// </summary>
		/// <param name="err">The error object as ref</param>
		/// <param name="givenTransaction">The given transaction.</param>
		public void EndTransaction(DestinationDatabase destinationDatabase, ref ErrorObj err, iDB2Transaction givenTransaction)
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
		public void EndTransaction(DestinationDatabase destinationDatabase, ref ErrorObj err, iDB2Transaction givenTransaction, iDB2DataReader readerToClose)
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
		public ErrorObj AccessWithTransaction(iDB2Transaction givenTransaction)
		{
			ErrorObj err = new ErrorObj();
			iDB2Command db2CommandEntity = new iDB2Command();
			db2CommandEntity.Transaction = givenTransaction;
			db2CommandEntity.Connection = givenTransaction.Connection;
			err = PrepareCommandElement(db2CommandEntity);
			if (!(err.HasError))
			{
				try
				{
					if (_commandElement.CommandExecutionType == CommandExecution.ExecuteDataSet)
					{
						this.ResultDataSet = ExecuteDataSet(db2CommandEntity);
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteNonQuery)
					{
						this.ResultDataSet = ExecuteNonQuery(db2CommandEntity);
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteReader)
					{
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteScalar)
					{
					}
				}
				catch (iDB2Exception db2Ex)
				{
					err.HasError = true;
					err.ErrorMessage = db2Ex.Message + "; Err Code:" + System.Convert.ToString(db2Ex.ErrorCode) + "; MessageDetails:" + db2Ex.MessageDetails + "; Source:" + db2Ex.Source;
					Utilities.InsertErrorLog(Settings, logSourceClass, System.Reflection.MethodBase.GetCurrentMethod().Name, logCode, Settings.BusinessUnit, logFilter2, logFilter3, logFilter4, db2Ex.StackTrace.ToString(), err.ErrorMessage);
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
		/// Access the data base DB2 TalentTKT
		/// This is called by DBAccess Class
		/// </summary>
		/// <returns></returns>
		protected override ErrorObj AccessDataBaseTALENTTKT()
		{
			ErrorObj err = new ErrorObj();
			iDB2Command db2CommandEntity = new iDB2Command();
			db2CommandEntity.Connection = conTALENTTKT;
			try
			{
				err = PrepareCommandElement(db2CommandEntity);
				if (!(err.HasError))
				{
					if (_commandElement.CommandExecutionType == CommandExecution.ExecuteDataSet)
					{
						this.ResultDataSet = ExecuteDataSet(db2CommandEntity);
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteNonQuery)
					{
						this.ResultDataSet = ExecuteNonQuery(db2CommandEntity);
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteReader)
					{
					}
					else if (_commandElement.CommandExecutionType == CommandExecution.ExecuteScalar)
					{
					}
				}
			}
			catch (iDB2Exception ex)
			{
				err.HasError = true;
				err.ErrorMessage = ex.Message;
				if (_commandElement.CommandType == CommandType.StoredProcedure)
				{
					err.ErrorMessage = err.ErrorMessage + " (Source=" + ex.Source + ", ErrorCode=" + ex.ErrorCode.ToString() + ", MessageDetails=" + ex.MessageDetails + ")";
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
			if (destinationDatabase == TalentSystemDefaults.DestinationDatabase.TALENTTKT)
			{
				err = TALENTTKTOpen();
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
			if (destinationDatabase == TalentSystemDefaults.DestinationDatabase.TALENTTKT)
			{
				err = TALENTTKTClose();
			}
			return err;
		}
		/// <summary>
		/// Executes the command object using DB2DataAdapter to fill the dataset and return
		/// </summary>
		/// <param name="commandToExecute">The command to execute.</param>
		/// <returns>DataSet</returns>
		private DataSet ExecuteDataSet(iDB2Command commandToExecute)
		{
			DataSet outputDataSet = null;
			using (iDB2DataAdapter db2DataAdapterExecuter = new iDB2DataAdapter(commandToExecute))
			{
				outputDataSet = new DataSet();
				db2DataAdapterExecuter.Fill(outputDataSet);
			}
			return outputDataSet;
		}
		/// <summary>
		/// Executes the command object using executenonquery method and
		/// returns a dataset with a table with single column and row ROWS_AFFECTED
		/// </summary>
		/// <param name="commandToExecute">The command to execute.</param>
		/// <returns>A dataset with a table with single column and row ROWS_AFFECTED</returns>
		private DataSet ExecuteNonQuery(iDB2Command commandToExecute)
		{
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
		/// Assign the command instance properties from DEDB2Command instance
		/// </summary>
		/// <param name="givenDB2Command">The given DB2 command.</param>
		/// <returns>ErrorObj</returns>
		private ErrorObj PrepareCommandElement(iDB2Command givenDB2Command)
		{
			ErrorObj err = new ErrorObj();
			if ((!(err.HasError)) && HasValue(_commandElement.CommandText))
			{
				givenDB2Command.CommandText = _commandElement.CommandText;
			}
			else
			{
				err.ErrorMessage = ERROR_COMMANDTEXT;
				err.HasError = true;
			}
			if ((!(err.HasError)) && HasValue(_commandElement.CommandType))
			{
				givenDB2Command.CommandType = _commandElement.CommandType;
			}
			else
			{
				err.ErrorMessage = ERROR_COMMANDTYPE;
				err.HasError = true;
			}
			if (!(err.HasError))
			{
				err = AttachParametersToCommand(givenDB2Command);
			}
			return err;
		}
		/// <summary>
		/// Attaches the parameters to the given DB2 command instance
		/// </summary>
		/// <param name="givenDB2Command">The given DB2 command.</param>
		/// <returns></returns>
		private ErrorObj AttachParametersToCommand(iDB2Command givenDB2Command)
		{
			ErrorObj err = new ErrorObj();
			if (!(_commandElement.CommandParameter == null))
			{
				if (_commandElement.CommandParameter.Count > 0)
				{
					System.Collections.Generic.List<DEDB2Parameter> paramaterList = _commandElement.CommandParameter;
					DEDB2Parameter parameterPiece = default(DEDB2Parameter);
					foreach (DEDB2Parameter tempLoopVar_parameterPiece in paramaterList)
					{
						parameterPiece = tempLoopVar_parameterPiece;
						iDB2Parameter tempDB2Parameter = new iDB2Parameter();
						if ((!(err.HasError)) && HasValue(parameterPiece.ParamName))
						{
							tempDB2Parameter.ParameterName = parameterPiece.ParamName;
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
							tempDB2Parameter.Value = DBNull.Value;
						}
						else
						{
							if ((!(err.HasError)) && HasValue(System.Convert.ToString(parameterPiece.ParamValue), parameterPiece.ParamType))
							{
								tempDB2Parameter.iDB2DbType = parameterPiece.ParamType;
								tempDB2Parameter.Value = parameterPiece.ParamValue;
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
							tempDB2Parameter.Direction = parameterPiece.ParamDirection;
						}
						if (!(err.HasError))
						{
							givenDB2Command.Parameters.Add(tempDB2Parameter);
						}
						tempDB2Parameter = null;
					}
					parameterPiece = null;
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
		/// Determines whether the specified given value is matching with given iDB2DbType.
		/// </summary>
		/// <param name="givenValueToCheck">The given value to check.</param>
		/// <param name="givenTypeToCheck">The given type to check.</param>
		/// <returns>
		/// <c>true</c> if the specified given value is matching with given iDB2DbType; otherwise, <c>false</c>.
		/// </returns>
		private bool HasValue(string givenValueToCheck, iDB2DbType givenTypeToCheck)
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
					case iDB2DbType.iDB2VarChar:
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
					case iDB2DbType.iDB2Integer:
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
					case iDB2DbType.iDB2Decimal:
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
					case iDB2DbType.iDB2Double:
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
					case iDB2DbType.iDB2SmallInt:
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
					case iDB2DbType.iDB2BigInt:
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
					case iDB2DbType.iDB2Date:
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
