using System;
using System.Collections.Generic;
namespace TalentSystemDefaults
{
	[Serializable()]
	public class DESettings
	{
		#region Class Level Fields
		private string _destinationType;
		private string _backOfficeConnectionString = string.Empty;
		private string _destinationDatabase = string.Empty;
		private string _frontEndConnectionString = string.Empty;
		private string _configurationConnectionString = string.Empty;
		private System.Collections.Generic.List<string> _connectionStringList;
		private string _moduleName = string.Empty;
		private bool _retryFailures = false;
		private int _retryAttempts = 0;
		private int _retryWaitTime = 0;
		private string _retryErrorNumbers = string.Empty;
		private string _agentName = string.Empty;
		private string _company = string.Empty;
		private string _bu;
		private string _IgnoreErrors;
		private string _databaseType1 = string.Empty;
		#endregion
		#region Public Properties
		private string _GroupID = string.Empty;
		public string GroupID
		{
			get
			{
				return _GroupID;
			}
			set
			{
				_GroupID = value;
			}
		}
		private string _Module_Name = string.Empty;
		public string Module_Name
		{
			get
			{
				return _Module_Name;
			}
			set
			{
				_Module_Name = value;
			}
		}
		private string _SessionID = string.Empty;
		public string SessionID
		{
			get
			{
				return _SessionID;
			}
			set
			{
				_SessionID = value;
			}
		}
		private string _ListName = string.Empty;
		public string ListName
		{
			get
			{
				return _ListName;
			}
			set
			{
				_ListName = value;
			}
		}
		private string _Mode = string.Empty;
		public string Mode
		{
			get
			{
				return _Mode;
			}
			set
			{
				_Mode = value;
			}
		}
		public string VariableKey1 { get; set; }
		public string VariableKey2 { get; set; }
		public string VariableKey3 { get; set; }
		public string VariableKey4 { get; set; }
		private bool _EnableDB2AccessQueue = true;
		public bool EnableDB2AccessQueue
		{
			get
			{
				return _EnableDB2AccessQueue;
			}
			set
			{
				_EnableDB2AccessQueue = value;
			}
		}
		private DB2AccessQueue _DB2AccessQueue = new DB2AccessQueue();
		public DB2AccessQueue DB2AccessQueue
		{
			get
			{
				return _DB2AccessQueue;
			}
			set
			{
				_DB2AccessQueue = value;
			}
		}
		public Dictionary<string, string> PageTexts { get; set; }
		public string BusinessUnit
		{
			get
			{
				return _bu;
			}
			set
			{
				_bu = value;
			}
		}
		public string DefaultBusinessUnit { get; set; }
		public string DestinationType
		{
			get
			{
				return _destinationType;
			}
			set
			{
				_destinationType = value;
			}
		}
		public string IgnoreErrors
		{
			get
			{
				return _IgnoreErrors;
			}
			set
			{
				_IgnoreErrors = value;
			}
		}
		public System.Collections.Generic.List<string> ConnectionStringList
		{
			get
			{
				return _connectionStringList;
			}
			set
			{
				_connectionStringList = value;
			}
		}
		public string BackOfficeConnectionString
		{
			get
			{
				return _backOfficeConnectionString;
			}
			set
			{
				_backOfficeConnectionString = value;
			}
		}
		public string DatabaseType1
		{
			get
			{
				return _databaseType1;
			}
			set
			{
				_databaseType1 = value;
			}
		}
		public string DestinationDatabase
		{
			get
			{
				return _destinationDatabase;
			}
			set
			{
				_destinationDatabase = value;
			}
		}
		public string ModuleName
		{
			get
			{
				return _moduleName;
			}
			set
			{
				_moduleName = value;
			}
		}
		public string FunctionName { get; set; }
		public string FrontEndConnectionString
		{
			get
			{
				return _frontEndConnectionString;
			}
			set
			{
				_frontEndConnectionString = value;
			}
		}
		public string ConfigurationConnectionString
		{
			get
			{
				return _configurationConnectionString;
			}
			set
			{
				_configurationConnectionString = value;
			}
		}
		public string RetryFailures
		{
			get
			{
				return System.Convert.ToString(_retryFailures);
			}
			set
			{
				_retryFailures = bool.Parse(value);
			}
		}
		public string RetryAttempts
		{
			get
			{
				return System.Convert.ToString(_retryAttempts);
			}
			set
			{
				_retryAttempts = System.Convert.ToInt32(value);
			}
		}
		public string RetryWaitTime
		{
			get
			{
				return System.Convert.ToString(_retryWaitTime);
			}
			set
			{
				_retryWaitTime = System.Convert.ToInt32(value);
			}
		}
		public string RetryErrorNumbers
		{
			get
			{
				return _retryErrorNumbers;
			}
			set
			{
				_retryErrorNumbers = value;
			}
		}
		public string AgentName
		{
			get
			{
				return _agentName;
			}
			set
			{
				_agentName = value;
			}
		}
		public string Company
		{
			get
			{
				return _company;
			}
			set
			{
				_company = value;
			}
		}
		public string GetPropertyValue(string PropertyName)
		{
			string PropertyValue = "";
			switch (PropertyName)
			{
				case "BUSINESSUNIT":
					PropertyValue = BusinessUnit;
					break;
				case "AGENTNAME":
					PropertyValue = AgentName;
					break;
				case "STOREDPROC":
					PropertyValue = StoredProcedure;
					break;
				case "ISERIESTODAYSDATE":
					PropertyValue = iSeriesTodaysDate;
					break;
			}
			return PropertyValue;
		}
		public string SearchText { get; set; }
		public string StoredProcedure
		{
			get
			{
				return "TD001S";
			}
		}
		public string iSeriesTodaysDate
		{
			get
			{
				int century = (DateTime.Now.Year / 100) - 19;
				return century.ToString() + DateTime.Now.ToString("yyMMdd"); // current date in "CyyMMdd" format;
			}
		}
		#endregion
	}
}
