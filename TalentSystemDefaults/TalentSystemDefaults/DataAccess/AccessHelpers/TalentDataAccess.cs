using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
namespace TalentSystemDefaults
{
	/// <summary>
	/// Provides the gateway to access the Data Access Layer
	/// </summary>
	[Serializable()]
	public class TalentDataAccess : TalentBase
	{
		#region Class Level Fields
		/// <summary>
		/// DESQLCommand Instance
		/// </summary>
		private DESQLCommand _commandElements = new DESQLCommand();
		private DataSet _resultDataSet;
		private string _moduleNameForCacheDependency = "";
		public enum ActionTypes
		{
			Insert,
			Delete,
			Update
		}
		#endregion
		#region Properties
		/// <summary>
		/// Gets or sets the command elements of type DESQLCommand
		/// </summary>
		/// <value>The command elements.</value>
		public DESQLCommand CommandElements
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
		private string _TableName = string.Empty;
		public string TableName
		{
			get
			{
				return _TableName;
			}
			set
			{
				_TableName = value;
			}
		}
		private string _Action = string.Empty;
		public string Action
		{
			get
			{
				return _Action;
			}
			set
			{
				_Action = value;
			}
		}
		private string _DataSource = string.Empty;
		public string DataSource
		{
			get
			{
				return _DataSource;
			}
			set
			{
				_DataSource = value;
			}
		}
		private string _Catalog = string.Empty;
		public string Catalog
		{
			get
			{
				return _Catalog;
			}
			set
			{
				_Catalog = value;
			}
		}
		#endregion
		#region Public Methods
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <remarks></remarks>
		public TalentDataAccess()
		{
			StackTrace stackTrace = new StackTrace();
			Action = stackTrace.GetFrame(1).GetMethod().Name;
			TableName = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;
		}
		/// <summary>
		/// Functionality to provide the gateway to access the Data Access Layer
		/// </summary>
		/// <param name="destinationDatabase"></param>
		/// <param name="allowSerialize"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public ErrorObj SQLAccess(DestinationDatabase destinationDatabase, bool allowSerialize = true)
		{
			string ModuleName = GetModuleNameByDestinationDB(destinationDatabase);
			ErrorObj err = new ErrorObj();
			this.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName);
			Settings.ModuleName = ModuleName;
			DBDataAccess DBDataAccessEntity = new DBDataAccess();
			DBDataAccessEntity.Settings = Settings;
			DBDataAccessEntity.CommandElement = _commandElements;
            //Utilities.LogSQL(DBDataAccessEntity.CommandElement);
			err = DBDataAccessEntity.AccessDatabase();
			if (!err.HasError && !(DBDataAccessEntity.ResultDataSet == null))
			{
				ResultDataSet = DBDataAccessEntity.ResultDataSet;
				// Serialize the transaction
				if (allowSerialize && this.CommandElements.CommandExecutionType == CommandExecution.ExecuteNonQuery)
				{
					SetConnectionStringParameters();
					Utilities.SerializeTransaction(this, Settings);
				}
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
		/// <param name="allowSerialize">To allow serialization of TalentDataAccess instance </param>
		/// <returns></returns>
		public ErrorObj SQLAccess(DestinationDatabase destinationDatabase, SqlTransaction givenTransaction, bool allowSerialize = true)
		{
			string ModuleName = GetModuleNameByDestinationDB(destinationDatabase);
			ErrorObj err = new ErrorObj();
			this.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName);
			Settings.ModuleName = ModuleName;
			DBDataAccess DBDataAccessEntity = new DBDataAccess();
			DBDataAccessEntity.Settings = Settings;
			DBDataAccessEntity.CommandElement = _commandElements;
			err = DBDataAccessEntity.AccessWithTransaction(givenTransaction);
			if (!err.HasError && !(DBDataAccessEntity.ResultDataSet == null))
			{
				ResultDataSet = DBDataAccessEntity.ResultDataSet;
				// Serialize the transaction
				if (allowSerialize && this.CommandElements.CommandExecutionType == CommandExecution.ExecuteNonQuery)
				{
					SetConnectionStringParameters();
					Utilities.SerializeTransaction(this, Settings);
				}
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
		public SqlTransaction BeginTransaction(DestinationDatabase destinationDatabase, ref ErrorObj err, IsolationLevel? givenIsolationLevel = null)
		{
			string ModuleName = GetModuleNameByDestinationDB(destinationDatabase);
			SqlTransaction SqlTrans = null;
			this.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName);
			Settings.ModuleName = ModuleName;
			DBDataAccess DBDataAccessEntity = new DBDataAccess();
			DBDataAccessEntity.Settings = Settings;
			DBDataAccessEntity.CommandElement = _commandElements;
			if (givenIsolationLevel == null)
			{
				SqlTrans = DBDataAccessEntity.BeginTransaction(destinationDatabase, ref err);
			}
			else
			{
				SqlTrans = DBDataAccessEntity.BeginTransaction(destinationDatabase, ref err);
			}
			if (err.HasError)
			{
				SqlTrans = null;
			}
			//'End If
			return SqlTrans;
		}
		/// <summary>
		/// Ends the transaction by passing the object to DBDataAccess
		/// </summary>
		/// <param name="destinationDatabase"></param>
		/// <param name="givenTransaction">The given transaction.</param>
		/// <returns>Error Object</returns>
		public ErrorObj EndTransaction(DestinationDatabase destinationDatabase, SqlTransaction givenTransaction)
		{
			string ModuleName = GetModuleNameByDestinationDB(destinationDatabase);
			ErrorObj err = new ErrorObj();
			this.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName);
			Settings.ModuleName = ModuleName;
			DBDataAccess DBDataAccessEntity = new DBDataAccess();
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
		public ErrorObj EndTransaction(DestinationDatabase destinationDatabase, SqlTransaction givenTransaction, SqlDataReader readerToClose)
		{
			string ModuleName = GetModuleNameByDestinationDB(destinationDatabase);
			ErrorObj err = new ErrorObj();
			this.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName);
			Settings.ModuleName = ModuleName;
			DBDataAccess DBDataAccessEntity = new DBDataAccess();
			DBDataAccessEntity.Settings = Settings;
			DBDataAccessEntity.CommandElement = _commandElements;
			DBDataAccessEntity.EndTransaction(destinationDatabase, ref err, givenTransaction, readerToClose);
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
		private string GetModuleNameByDestinationDB(DestinationDatabase destinationDatabase)
		{
			string moduleName = GlobalConstants.DBACCESS_SQL;
			if (destinationDatabase == TalentSystemDefaults.DestinationDatabase.SQL2005)
			{
				moduleName = GlobalConstants.DBACCESS_SQL;
			}
			else if (destinationDatabase == TalentSystemDefaults.DestinationDatabase.TALENT_ADMIN)
			{
				moduleName = GlobalConstants.DBACCESS_TALENT_ADMIN;
			}
			else if (destinationDatabase == TalentSystemDefaults.DestinationDatabase.TALENT_DEFINITION)
			{
				moduleName = GlobalConstants.DBACCESS_TALENT_DEFINITIONS;
			}
			else if (destinationDatabase == TalentSystemDefaults.DestinationDatabase.TALENT_CONFIG)
			{
				moduleName = GlobalConstants.DBACCESS_TALENT_CONFIGURATION;
			}
			return moduleName;
		}
		/// <summary>
		///  XML Comment
		/// </summary>
		/// <remarks></remarks>
		private void SetConnectionStringParameters()
		{
			SqlConnectionStringBuilder connectionString = default(SqlConnectionStringBuilder);
			if (!string.IsNullOrEmpty(Settings.FrontEndConnectionString))
			{
				connectionString = new SqlConnectionStringBuilder(Settings.FrontEndConnectionString);
				DataSource = connectionString.DataSource;
				Catalog = connectionString.InitialCatalog;
			}
		}
		#endregion
	}
}
