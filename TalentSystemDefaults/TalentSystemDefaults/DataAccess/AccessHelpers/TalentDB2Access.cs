using IBM.Data.DB2.iSeries;
using System;
using System.Data;
namespace TalentSystemDefaults
{
	/// <summary>
	/// Provides the gateway to access the Data Access Layer for DB2
	/// </summary>
	[Serializable()]
	public class TalentDB2Access : TalentBase
	{
		#region Class Level Fields
		/// <summary>
		/// DEDB2Command Instance
		/// </summary>
		private DEDB2Command _commandElements = new DEDB2Command();
		private DataSet _resultDataSet;
		private string _moduleNameForCacheDependency = "";
		#endregion
		#region Properties
		/// <summary>
		/// Gets or sets the command elements of type DEDB2Command
		/// </summary>
		/// <value>The command elements.</value>
		public DEDB2Command CommandElements
		{
			get
			{
				return _commandElements;
			}
			set
			{
				_commandElements = value;
			}
		}
		/// <summary>
		/// Gets or sets the result data set.
		/// </summary>
		/// <value>The result data set.</value>
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
		#endregion
		#region Public Methods
		/// <summary>
		/// Functionality to provide the gateway to access the Data Access Layer for DB2
		/// </summary>
		/// <param name="destinationDatabase"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public ErrorObj DB2Access(DestinationDatabase destinationDatabase)
		{
			string ModuleName = GetModuleNameByDestinationDB(destinationDatabase);
			ErrorObj err = new ErrorObj();
			this.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName);
			Settings.ModuleName = ModuleName;
			DBDB2Access DBDataAccessEntity = new DBDB2Access();
			DBDataAccessEntity.Settings = Settings;
			DBDataAccessEntity.CommandElement = _commandElements;
			err = DBDataAccessEntity.AccessDatabase();
			if (!err.HasError && !(DBDataAccessEntity.ResultDataSet == null))
			{
				ResultDataSet = DBDataAccessEntity.ResultDataSet;
			}
			return err;
		}
		public ErrorObj DB2DefaultsAccess(DestinationDatabase destinationDatabase, DEDB2Defaults deDB2Defaults)
		{
			string ModuleName = GetModuleNameByDestinationDB(destinationDatabase);
			ErrorObj err = new ErrorObj();
			this.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName);
			Settings.ModuleName = ModuleName;
			DB2Defaults dbDB2Access = new DB2Defaults();
			dbDB2Access.Settings = Settings;
			dbDB2Access.DEDB2Defaults = deDB2Defaults;
			err = dbDB2Access.AccessDatabase();
			if (!err.HasError && !(dbDB2Access.ResultDataSet == null))
			{
				ResultDataSet = dbDB2Access.ResultDataSet;
			}
			return err;
		}
		/// <summary>
		/// Use this for Insert, Update and Delete
		/// Functionality to provide the gateway to access the Data Access Layer with Transaction object
		/// If any exception transaction will be rollbacked here
		/// </summary>
		/// <param name="destinationDatabase"></param>
		/// <param name="givenTransaction">The given transaction.</param>
		/// <returns></returns>
		public ErrorObj DB2Access(DestinationDatabase destinationDatabase, iDB2Transaction givenTransaction)
		{
			string ModuleName = GetModuleNameByDestinationDB(destinationDatabase);
			ErrorObj err = new ErrorObj();
			this.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName);
			Settings.ModuleName = ModuleName;
			DBDB2Access DBDataAccessEntity = new DBDB2Access();
			DBDataAccessEntity.Settings = Settings;
			DBDataAccessEntity.CommandElement = _commandElements;
			err = DBDataAccessEntity.AccessWithTransaction(givenTransaction);
			if (!err.HasError && !(DBDataAccessEntity.ResultDataSet == null))
			{
				ResultDataSet = DBDataAccessEntity.ResultDataSet;
			}
			else
			{
				givenTransaction.Rollback();
				//before call this get the previous error details
				string errMessage = err.ErrorMessage;
				err = EndTransaction(destinationDatabase, givenTransaction);
				if (err.HasError)
				{
					errMessage = errMessage + ";" + err.ErrorMessage;
				}
				//whether end transaction gives error or not
				//always assign err object has error
				err.HasError = true;
				err.ErrorMessage = errMessage;
			}
			return err;
		}
		/// <summary>
		/// Begins the transaction and returns the transaction object with open state connection.
		/// Make sure to call EndTransaction method to close transaction
		/// </summary>
		/// <param name="destinationDatabase"></param>
		/// <param name="err">The err.</param>
		/// <param name="givenIsolationLevel">The given isolation level.</param>
		/// <returns></returns>
		public iDB2Transaction BeginTransaction(DestinationDatabase destinationDatabase, ref ErrorObj err, IsolationLevel? givenIsolationLevel = null)
		{
			string ModuleName = GetModuleNameByDestinationDB(destinationDatabase);
			iDB2Transaction db2Trans = null;
			this.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName);
			Settings.ModuleName = ModuleName;
			DBDB2Access DBDataAccessEntity = new DBDB2Access();
			DBDataAccessEntity.Settings = Settings;
			DBDataAccessEntity.CommandElement = _commandElements;
			if (givenIsolationLevel == null)
			{
				db2Trans = DBDataAccessEntity.BeginTransaction(destinationDatabase, ref err);
			}
			else
			{
				db2Trans = DBDataAccessEntity.BeginTransaction(destinationDatabase, ref err);
			}
			if (err.HasError)
			{
				db2Trans = null;
			}
			//'End If
			return db2Trans;
		}
		/// <summary>
		/// Ends the transaction by passing the object to DBDataAccess
		/// </summary>
		/// <param name="destinationDatabase"></param>
		/// <param name="givenTransaction">The given transaction.</param>
		/// <returns>Error Object</returns>
		public ErrorObj EndTransaction(DestinationDatabase destinationDatabase, iDB2Transaction givenTransaction)
		{
			string ModuleName = GetModuleNameByDestinationDB(destinationDatabase);
			ErrorObj err = new ErrorObj();
			this.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName);
			Settings.ModuleName = ModuleName;
			DBDB2Access DBDataAccessEntity = new DBDB2Access();
			DBDataAccessEntity.Settings = Settings;
			DBDataAccessEntity.CommandElement = _commandElements;
			DBDataAccessEntity.EndTransaction(destinationDatabase, ref err, givenTransaction);
			return err;
		}
		/// <summary>
		/// Ends the transaction and close the reader by passing the object to DBDataAccess
		/// </summary>
		/// <param name="destinationDatabase"></param>
		/// <param name="givenTransaction">The given transaction.</param>
		/// <param name="readerToClose">The reader to close.</param>
		/// <returns>Error Object</returns>
		public ErrorObj EndTransaction(DestinationDatabase destinationDatabase, iDB2Transaction givenTransaction, iDB2DataReader readerToClose)
		{
			string ModuleName = GetModuleNameByDestinationDB(destinationDatabase);
			ErrorObj err = new ErrorObj();
			this.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName);
			Settings.ModuleName = ModuleName;
			DBDB2Access DBDataAccessEntity = new DBDB2Access();
			DBDataAccessEntity.Settings = Settings;
			DBDataAccessEntity.CommandElement = _commandElements;
			DBDataAccessEntity.EndTransaction(destinationDatabase, ref err, givenTransaction, readerToClose);
			return err;
		}
		#endregion
		#region Private Methods
		private string GetModuleNameByDestinationDB(DestinationDatabase destinationDatabase)
		{
			string moduleName = GlobalConstants.DBACCESS_TALENT_TICKETING;
			if (destinationDatabase == TalentSystemDefaults.DestinationDatabase.TALENTTKT)
			{
				moduleName = GlobalConstants.DBACCESS_TALENT_TICKETING;
			}
			return moduleName;
		}
		#endregion
	}
}
