using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	#region Enumerations
	/// <summary>
	/// Provides the status types for the STATUS column in the table tbl_log_header
	/// </summary>
	public enum ActivityStatusEnum
	{
		/// <summary>
		/// Start status
		/// </summary>
		STARTED,
		/// <summary>
		/// In progress status
		/// </summary>
		INPROGRESS,
		/// <summary>
		/// Success
		/// </summary>
		SUCCESS,
		/// <summary>
		/// Failed
		/// </summary>
		FAILED
	}
	/// <summary>
	/// Provides the status types for the STATUS column in the table tbl_data_transfer_status
	/// </summary>
	public enum DataTransferStatusEnum
	{
		/// <summary>
		/// Clearing work table (on insert)
		/// </summary>
		CLEARING_WORK_TABLE,
		/// <summary>
		/// In progress status (extracting - on update)
		/// </summary>
		EXTRACTING_FROM_ISERIES,
		/// <summary>
		/// Updating Tables (on update)
		/// </summary>
		UPDATING_SQL_TABLES,
		/// <summary>
		/// Success/Finished (on update)
		/// </summary>
		FINISHED,
		/// <summary>
		/// Failed - exception occurred (on update)
		/// </summary>
		FAILED
	}
	#endregion
	/// <summary>
	/// Provides the functionalities which are common across the data objects
	/// </summary>
	[Serializable()]
	public abstract class DBObjectBase
	{
		#region Class Level Fields
		private const string CLASSNAME = "DBObjectBase";
		private object _customObject = null;
		private DataSet _resultDataSet = null;
		protected string businessUnit = null;
        protected string partnerCode = null;
		protected string variableKey1Value = string.Empty;
		protected string companyCode = string.Empty;
		/// <summary>
		/// Instance of DESettings
		/// </summary>
		protected DESettings settings;
		protected const string ALL_STRING = "*ALL";
		protected const string LANG_ENG = "ENG";
		protected string logSourceClass = string.Empty;
		protected string logSourceMethod = string.Empty;
		protected string logCode = TalentSystemDefaults.GlobalConstants.LOG_ERROR_CODE;
		protected string logFilter1 = string.Empty;
		protected string logFilter2 = string.Empty;
		protected string logFilter3 = string.Empty;
		protected string logFilter4 = string.Empty;
		protected string logFilter5 = string.Empty;
		protected string logContent = string.Empty;
		public enum DBType
		{
			SQLSERVER,
			DB2
		}
		#endregion
		#region Public Properties
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
		/// <summary>
		/// Get database type
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		private DBType _DBTypeForTable = DBType.SQLSERVER;
		public DBType DBTypeForTable
		{
			get
			{
				return _DBTypeForTable;
			}
			set
			{
				_DBTypeForTable = value;
			}
		}
		#endregion
		#region Public Methods
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings"></param>
		/// <remarks></remarks>
		public DBObjectBase(DESettings settings)
		{
			this.settings = settings;
			if (settings.VariableKey1 != string.Empty)
			{
				variableKey1Value = settings.VariableKey1;
			}
			if (settings.Company != string.Empty)
			{
				companyCode = settings.Company;
			}
		}
		/// <summary>
		/// Returns list of Current Configurations
		/// </summary>
		/// <param name="configs"></param>
		/// <remarks></remarks>
		public virtual void RetrieveCurrentValues(List<ConfigurationItem> configs)
		{
		}
		/// <summary>
		/// Inserts the mssing configurations
		/// </summary>
		/// <param name="configs"></param>
		/// <param name="currentValues"></param>
		/// <remarks></remarks>
		public virtual void AddMissingValues(System.Collections.Generic.List<ConfigurationItem> configs, Dictionary<string, string> currentValues)
		{
		}
		/// <summary>
		/// Updates the configuration data
		/// </summary>
		/// <param name="configs"></param>
		/// <param name="givenTransaction"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual int UpdateCurrentValue(System.Collections.Generic.List<ConfigurationItem> configs, SqlTransaction givenTransaction = null)
		{
			int updValue = 0;
			return updValue;
		}
		/// <summary>
		/// Updates the configuration data for DB2
		/// </summary>
		/// <param name="configs"></param>
		/// <param name="givenTransaction"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual int UpdateCurrentValueDB2(System.Collections.Generic.List<ConfigurationItem> configs, iDB2Transaction givenTransaction = null)
		{
			int updValue = 0;
			return updValue;
		}
		/// <summary>
		/// Inserts Configurations
		/// </summary>
		/// <param name="configs"></param>
		/// <param name="givenTransaction"></param>
		/// <param name="Key"></param>
		/// <returns></returns>
		public virtual int InsertCurrentValue(System.Collections.Generic.List<ConfigurationItem> configs, SqlTransaction givenTransaction, ref string Key)
		{
			int insValue = 0;
			return insValue;
		}
		/// <summary>
		/// Inserts Configurations for DB2
		/// </summary>
		/// <param name="configs"></param>
		/// <param name="givenTransaction"></param>
		/// <param name="Key"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual int InsertCurrentValueDB2(System.Collections.Generic.List<ConfigurationItem> configs, iDB2Transaction givenTransaction = null)
		{
			return 0;
		}
		/// <summary>
		/// Returns all configurations
		/// </summary>
		/// <param name="businessUnit"></param>
		/// <param name="defaultKeys"></param>
		/// <param name="variableKeys"></param>
		/// <param name="displayName"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual List<ConfigurationEntity> RetrieveAllValues(string businessUnit, string[] defaultKeys, string[] variableKeys, string displayName = "")
		{
			System.Collections.Generic.List<ConfigurationEntity> lstConfigurationEntity = new System.Collections.Generic.List<ConfigurationEntity>();
			return lstConfigurationEntity;
		}
        /// <summary>
        /// Returns all configurations
        /// </summary>
        /// <param name="businessUnit"></param>
        /// <param name="defaultKeys"></param>
        /// <param name="variableKeys"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual List<ConfigurationEntity> RetrieveAllValues(string businessUnit, string partner, string[] defaultKeys, string[] variableKeys, string displayName = "")
        {
            System.Collections.Generic.List<ConfigurationEntity> lstConfigurationEntity = new System.Collections.Generic.List<ConfigurationEntity>();
            return lstConfigurationEntity;
        }
		/// <summary>
		/// Returns all iseries configurations
		/// </summary>
		/// <param name="companyCode"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual List<ConfigurationEntity> RetrieveAlliSeriesValues(string companyCode, string[] defaultKeys, string[] variableKeys, string selectedColumns = "")
		{
			List<ConfigurationEntity> lstConfigurationEntity = new List<ConfigurationEntity>();
			return lstConfigurationEntity;
		}
		/// <summary>
		/// Returns configurations as a list of DEListDetail
		/// </summary>
		/// <param name="dataColumns"></param>
		/// <param name="defaultKeyColumns"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual List<DEListDetail> RetrieveListValues(System.Collections.Generic.List<DataColumn> dataColumns, System.Collections.Generic.List<KeyColumn> defaultKeyColumns)
		{
			System.Collections.Generic.List<DEListDetail> lstListDetailEntity = new System.Collections.Generic.List<DEListDetail>();
			return lstListDetailEntity;
		}
		/// <summary>
		/// Returns configurations as a list of DEListDetail
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual List<DEListDetail> RetrieveListValues()
		{
			System.Collections.Generic.List<DEListDetail> lstListDetailEntity = new System.Collections.Generic.List<DEListDetail>();
			return lstListDetailEntity;
		}
		/// <summary>
		/// Returns configurations as a DataTable
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual DataTable RetrieveDTForList()
		{
			DataTable dtForList = new DataTable();
			return dtForList;
		}
		/// <summary>
		/// Returns configurations as a DataTable
		/// </summary>
		/// <param name="dataColumns"></param>
		/// <param name="variableKeyColumns"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual DataTable RetrieveDTForList(System.Collections.Generic.List<DataColumn> dataColumns, System.Collections.Generic.List<KeyColumn> variableKeyColumns)
		{
			DataTable dtForList = new DataTable();
			return dtForList;
		}
		/// <summary>
		/// Deletes configurations
		/// </summary>
		/// <param name="givenTransaction"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual int DeleteCurrentValues(SqlTransaction givenTransaction = null)
		{
			int dltValue = 0;
			return dltValue;
		}
		/// <summary>
		/// Deletes configurations in DB2
		/// </summary>
		/// <param name="givenTransaction"></param>
		/// <returns></returns>
		public virtual int DeleteCurrentValuesDB2(iDB2Transaction givenTransaction = null)
		{
			return 0;
		}
		#endregion
		#region Protected Methods
		/// <summary>
		/// Constructs the parameter using the DESQLParamater instance
		/// </summary>
		/// <param name="paramName">Name of the parameter.</param>
		/// <param name="paramValue">The parameter value.</param>
		/// <param name="paramType">Type of the parameter. Default SqlDbType.NVarchar</param>
		/// <param name="paramDirection">The parameter direction. Default ParameterDirection.Input</param>
		/// <returns>DESQLParameter instance with details</returns>
		protected DESQLParameter ConstructParameter(string paramName, object paramValue, SqlDbType paramType = SqlDbType.NVarChar, ParameterDirection paramDirection = ParameterDirection.Input)
		{
			DESQLParameter parameterPiece = new DESQLParameter();
			parameterPiece.ParamDirection = paramDirection;
			parameterPiece.ParamName = paramName;
			parameterPiece.ParamType = paramType;
			parameterPiece.ParamValue = paramValue;
			return parameterPiece;
		}
		/// <summary>
		/// Constructs the parameter using the DEDB2Parameter instance
		/// </summary>
		/// <param name="paramName"></param>
		/// <param name="paramValue"></param>
		/// <param name="paramType"></param>
		/// <param name="paramDirection"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		protected DEDB2Parameter ConstructDB2Parameter(string paramName, string paramValue, iDB2DbType paramType = iDB2DbType.iDB2Char, ParameterDirection paramDirection = ParameterDirection.Input)
		{
			DEDB2Parameter parameterPiece = new DEDB2Parameter();
			parameterPiece.ParamDirection = paramDirection;
			parameterPiece.ParamName = paramName;
			parameterPiece.ParamType = paramType;
			parameterPiece.ParamValue = paramValue;
			return parameterPiece;
		}
		/// <summary>
		/// Constructs the parameter using the DEDB2Paramater instance
		/// </summary>
		/// <param name="paramName">Name of the parameter.</param>
		/// <param name="paramValue">The parameter value.</param>
		/// <param name="paramType">Type of the parameter. Default iDB2DbType.Char</param>
		/// <param name="paramDirection">The parameter direction. Default ParameterDirection.Input</param>
		/// <returns>DEDB2Parameter instance with details</returns>
		protected DEDB2Parameter ConstructDB2Parameter(string paramName, string paramValue, int paramSize, iDB2DbType paramType = iDB2DbType.iDB2Char, ParameterDirection paramDirection = ParameterDirection.Input)
		{
			DEDB2Parameter parameterPiece = new DEDB2Parameter();
			parameterPiece.ParamDirection = paramDirection;
			parameterPiece.ParamName = paramName;
			parameterPiece.ParamType = paramType;
			parameterPiece.ParamValue = paramValue;
			parameterPiece.ParamLength = paramSize;
			return parameterPiece;
		}
		/// <summary>
		/// Change the given string to upper case
		/// </summary>
		/// <param name="stringToUpper">The string to upper.</param>
		/// <returns>if nothing returns empty string otherwise upper case string</returns>
		protected string ToUpper(string stringToUpper)
		{
			if ((!(stringToUpper == null)) && (stringToUpper.Trim().Length > 0))
			{
				stringToUpper = stringToUpper.ToUpper();
			}
			else
			{
				stringToUpper = string.Empty;
			}
			return stringToUpper;
		}
		protected int DB2Access(TalentDB2Access talentDB2AccessDetail, iDB2Transaction givenTransaction)
		{
			int affectedRows = 0;
			if (settings.EnableDB2AccessQueue)
			{
				settings.DB2AccessQueue.Add(talentDB2AccessDetail);
			}
			else
			{
				ErrorObj err = new ErrorObj();
				if (givenTransaction == null)
				{
					err = talentDB2AccessDetail.DB2Access(DestinationDatabase.TALENTTKT);
				}
				else
				{
					err = talentDB2AccessDetail.DB2Access(DestinationDatabase.TALENTTKT, givenTransaction);
				}
				if ((!(err.HasError)) && (!(talentDB2AccessDetail.ResultDataSet == null)))
				{
					affectedRows = System.Convert.ToInt32(talentDB2AccessDetail.ResultDataSet.Tables[0].Rows[0][0]);
				}
			}
			return affectedRows;
		}
		#endregion
	}
}
