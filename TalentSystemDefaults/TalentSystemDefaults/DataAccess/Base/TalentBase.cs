using System;
using System.Collections.Generic;
using System.Data;
namespace TalentSystemDefaults
{
	[Serializable()]
	public class TalentBase
	{
		#region Class Level Fields
		private string _destinationDatabase;
		private string _connectionString;
		private DESettings _settings = new DESettings();
		private TalentDataSet _talentDataSet;
		#endregion
		#region Public Properties
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
		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			set
			{
				_connectionString = value;
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
		public TalentDataSet TalDataSet
		{
			get
			{
				if (_talentDataSet == null)
				{
					_talentDataSet = new TalentDataSet();
				}
				return _talentDataSet;
			}
			set
			{
				_talentDataSet = value;
			}
		}
		/// <summary>
		/// Gets or sets the fulfilment fee category dictionary (fulfilment method, fee category)
		/// key - fulfilment method
		/// value - fee category
		/// </summary>
		/// <value>
		/// The fulfilment fee category.
		/// </value>
		private Dictionary<string, string> _FulfilmentFeeCategory = null;
		public Dictionary<string, string> FulfilmentFeeCategory
		{
			get
			{
				return _FulfilmentFeeCategory;
			}
			set
			{
				_FulfilmentFeeCategory = value;
			}
		}
		private Dictionary<string, string> _CardTypeFeeCategory = null;
		public Dictionary<string, string> CardTypeFeeCategory
		{
			get
			{
				return _CardTypeFeeCategory;
			}
			set
			{
				_CardTypeFeeCategory = value;
			}
		}
		#endregion
		#region Public Methods
		/// <summary>
		/// Gets the connection details and populating the DESettings object
		/// </summary>
		/// <param name="businessUnit">The business unit.</param>
		/// <param name="application">The application.</param>
		/// <param name="moduleName">Name of the module.</param>
		/// <param name="sectionName">Name of the section.</param>
		/// <param name="subSectionName">Name of the sub section.</param>
		/// <param name="partner">The partner.</param>
		public void GetConnectionDetails(string businessUnit, string application = "", string moduleName = "", string sectionName = "", string subSectionName = "", string partner = "")
		{
			if (string.IsNullOrEmpty(application))
			{
				application = Utilities.GetAllString();
			}
			if (string.IsNullOrEmpty(moduleName))
			{
				moduleName = Utilities.GetAllString();
			}
			if (string.IsNullOrEmpty(sectionName))
			{
				sectionName = Utilities.GetAllString();
			}
			if (string.IsNullOrEmpty(subSectionName))
			{
				subSectionName = Utilities.GetAllString();
			}
			if (string.IsNullOrEmpty(partner))
			{
				partner = Utilities.GetAllString();
			}
			if (!(Settings.FrontEndConnectionString == null))
			{
				connectionDets conDets
					= null;
				bool canOverrideCacheAttributes = false;
				bool isCacheEnabled = false;
				int cacheSeconds = 0;
				string cacheKey = "TalentCommon - GetConnectionDetails - " + businessUnit + "|" + partner + "|" + application + "|" + moduleName + "|" + sectionName + "|" + subSectionName;
				System.Data.SqlClient.SqlDataReader dr = default(System.Data.SqlClient.SqlDataReader);
				System.Data.SqlClient.SqlDataReader drDest = default(System.Data.SqlClient.SqlDataReader);
				System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
				cmd.Connection = new System.Data.SqlClient.SqlConnection(Settings.FrontEndConnectionString);
				const string SqlSelect = " SELECT mdb.*, dt.* " + " FROM tbl_bu_module_database mdb " + "	INNER JOIN tbl_destination_type dt " + "		ON mdb.DESTINATION_DATABASE = dt.DESTINATION_TYPE_LINK " + " WHERE mdb.BUSINESS_UNIT = @BUSINESS_UNIT " + "	AND mdb.PARTNER = @PARTNER " + "	AND mdb.APPLICATION = @APPLICATION " + "	AND mdb.MODULE = @MODULE ";
				const string sectionClause = "   AND mdb.SECTION = @SECTION ";
				const string subSectionClause = "   AND mdb.SUB_SECTION = @SUB_SECTION ";
				try
				{
					cmd.Parameters.Clear();
					cmd.Parameters.Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = businessUnit;
					cmd.Parameters.Add("@APPLICATION", SqlDbType.NVarChar).Value = application;
					cmd.Parameters.Add("@MODULE", SqlDbType.NVarChar).Value = moduleName;
					cmd.Parameters.Add("@SECTION", SqlDbType.NVarChar).Value = sectionName;
					cmd.Parameters.Add("@SUB_SECTION", SqlDbType.NVarChar).Value = subSectionName;
					cmd.Parameters.Add("@PARTNER", SqlDbType.NVarChar).Value = partner;
					cmd.Connection.Open();
					cmd.CommandText = SqlSelect + sectionClause + subSectionClause;
					dr = cmd.ExecuteReader();
					if (!dr.HasRows)
					{
						dr.Close();
						cmd.CommandText = SqlSelect + sectionClause;
						dr = cmd.ExecuteReader();
						if (!dr.HasRows)
						{
							dr.Close();
							cmd.CommandText = SqlSelect;
							dr = cmd.ExecuteReader();
							if (!dr.HasRows)
							{
								dr.Close();
								cmd.CommandText = SqlSelect;
								cmd.Parameters["@PARTNER"].Value = Utilities.GetAllString();
								dr = cmd.ExecuteReader();
								if (!dr.HasRows)
								{
									dr.Close();
									cmd.CommandText = SqlSelect;
									cmd.Parameters["@BUSINESS_UNIT"].Value = Utilities.GetAllString();
									dr = cmd.ExecuteReader();
									if (!dr.HasRows)
									{
										dr.Close();
										cmd.CommandText = SqlSelect;
										cmd.Parameters["@MODULE"].Value = Utilities.GetAllString();
										dr = cmd.ExecuteReader();
									}
								}
							}
						}
					}
					if (dr.HasRows)
					{
						dr.Read();
						Settings.DestinationType = Utilities.CheckForDBNull_String(dr["DESTINATION_TYPE"]);
						string mydest = Utilities.CheckForDBNull_String(dr["DESTINATION_DATABASE"]);
						try
						{
							if ((!dr["CACHE_ENABLED"].Equals(DBNull.Value)) && (dr["CACHE_ENABLED"] != null) && (!string.IsNullOrWhiteSpace(System.Convert.ToString(dr["CACHE_ENABLED"]))))
							{
								canOverrideCacheAttributes = true;
								isCacheEnabled = Utilities.CheckForDBNull_Boolean_DefaultFalse(dr["CACHE_ENABLED"]);
								cacheSeconds = Utilities.CheckForDBNull_Int(dr["CACHE_SECONDS"]);
							}
							else
							{
								canOverrideCacheAttributes = false;
								isCacheEnabled = false;
								cacheSeconds = 0;
							}
						}
						catch (Exception)
						{
							canOverrideCacheAttributes = false;
						}
						dr.Close();
						cmd.Parameters.Clear();
						cmd.Parameters.Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = businessUnit;
						cmd.Parameters.Add("@PARTNER", SqlDbType.NVarChar).Value = partner;
						switch (Settings.DestinationType)
						{
							case "DB":
							case "DATABASE":
								cmd.CommandText = " SELECT * FROM tbl_database_version " + " 	WHERE BUSINESS_UNIT = @BUSINESS_UNIT " + " 	AND [PARTNER] = @PARTNER " + " 	AND DESTINATION_DATABASE = @DESTINATION_DATABASE ";
								cmd.Parameters.Add("@DESTINATION_DATABASE", SqlDbType.NVarChar).Value = mydest;
								break;
							case "XML":
								cmd.CommandText = " SELECT * FROM tbl_destination_xml " + " 	WHERE BUSINESS_UNIT = @BUSINESS_UNIT " + " 	AND [PARTNER] = @PARTNER " + " 	AND DESTINATION_TYPE = @DESTINATION_TYPE ";
								cmd.Parameters.Add("@DESTINATION_TYPE", SqlDbType.NVarChar).Value = mydest;
								break;
							default:
								cmd.CommandText = " SELECT * FROM tbl_database_version " + " 	WHERE BUSINESS_UNIT = @BUSINESS_UNIT " + " 	AND [PARTNER] = @PARTNER " + " 	AND DESTINATION_DATABASE = @DESTINATION_DATABASE ";
								cmd.Parameters.Add("@DESTINATION_DATABASE", SqlDbType.NVarChar).Value = mydest;
								break;
						}
						drDest = cmd.ExecuteReader();
						if (!drDest.HasRows)
						{
							drDest.Close();
							cmd.Parameters["@PARTNER"].Value = Utilities.GetAllString();
							drDest = cmd.ExecuteReader();
							if (!drDest.HasRows)
							{
								drDest.Close();
								cmd.Parameters["@BUSINESS_UNIT"].Value = Utilities.GetAllString();
								drDest = cmd.ExecuteReader();
							}
						}
						if (drDest.HasRows)
						{
							while (drDest.Read())
							{
								switch (Settings.DestinationType)
								{
									case "DB":
									case "DATABASE":
										Settings.DestinationDatabase = Utilities.CheckForDBNull_String(drDest["DESTINATION_DATABASE"]);
										Settings.DatabaseType1 = Utilities.CheckForDBNull_String(drDest["DATABASE_TYPE1"]);
										Settings.BackOfficeConnectionString = Utilities.CheckForDBNull_String(drDest["CONNECTION_STRING"]);
										break;
									//deciding whether to override cache settings or not
									default:
										Settings.DestinationDatabase = Utilities.CheckForDBNull_String(drDest["DESTINATION_DATABASE"]);
										Settings.DatabaseType1 = Utilities.CheckForDBNull_String(drDest["DATABASE_TYPE1"]);
										Settings.BackOfficeConnectionString = Utilities.CheckForDBNull_String(drDest["CONNECTION_STRING"]);
										break;
								}
								Settings.BackOfficeConnectionString = Utilities.CheckForDBNull_String(Settings.BackOfficeConnectionString);
							}
							drDest.Close();
						}
						else
						{
							drDest.Close();
						}
					}
					else
					{
						dr.Close();
					}
				}
				catch (Exception)
				{
				}
				finally
				{
					cmd.Connection.Close();
				}
				// Insert to cache
				try
				{
					conDets = new connectionDets(Settings.DestinationDatabase, Settings.DatabaseType1, Settings.BackOfficeConnectionString, Settings.DestinationType, canOverrideCacheAttributes, isCacheEnabled, cacheSeconds);
				}
				catch (Exception)
				{
				}
			}
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="err"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private string CheckResponseForError(DataSet ds, ErrorObj err)
		{
			string errorCode = string.Empty;
			// The dataset will give us errors from the back end
			if (!(ds == null))
			{
				System.Data.DataTable errorTable = null;
				try
				{
					errorTable = ds.Tables[0];
				}
				catch (Exception)
				{
					errorCode = "AAC";
				}
				if (!(errorTable == null))
				{
					foreach (System.Data.DataRow row in errorTable.Rows)
					{
						if (!row["ReturnCode"].Equals(string.Empty))
						{
							errorCode = System.Convert.ToString(row["ReturnCode"]);
							break;
						}
					}
				}
				else
				{
					errorCode = "AAD";
				}
			}
			else
			{
				errorCode = "AAB";
			}
			// The error object will give us any errors with comms to the back end.
			if (!(errorCode.Length > 0))
			{
				if (err.HasError)
				{
					errorCode = err.ErrorStatus;
				}
			}
			return errorCode;
		}
		#endregion
		private class connectionDets
		{
			#region Class Level Fields
			private string _DestinationDatabase;
			private string _DatabaseType1;
			private string _BackOfficeConnectionString;
			private string _destinationType;
			private bool _canOverrideCacheAttributes = false;
			private bool _isCacheEnabled = false;
			private int _cacheSeconds = 0;
			#endregion
			#region Public Properties
			public string DestinationDatabase
			{
				get
				{
					return _DestinationDatabase;
				}
				set
				{
					_DestinationDatabase = value;
				}
			}
			public string DatabaseType1
			{
				get
				{
					return _DatabaseType1;
				}
				set
				{
					_DatabaseType1 = value;
				}
			}
			public string BackOfficeConnectionString
			{
				get
				{
					return _BackOfficeConnectionString;
				}
				set
				{
					_BackOfficeConnectionString = value;
				}
			}
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
			public bool CanOverrideCacheAttributes
			{
				get
				{
					return _canOverrideCacheAttributes;
				}
				set
				{
					_canOverrideCacheAttributes = value;
				}
			}
			public bool IsCacheEnabled
			{
				get
				{
					return _isCacheEnabled;
				}
				set
				{
					_isCacheEnabled = value;
				}
			}
			public int CacheSeconds
			{
				get
				{
					return _cacheSeconds;
				}
				set
				{
					_cacheSeconds = value;
				}
			}
			#endregion
			#region Public Methods
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="destinationDatabase"></param>
			/// <param name="databaseType1"></param>
			/// <param name="backOfficeConnectionString"></param>
			/// <param name="destinationType"></param>
			/// <param name="canOverrideCacheAttributes"></param>
			/// <param name="isCacheEnabled"></param>
			/// <param name="cacheSeconds"></param>
			/// <remarks></remarks>
			public connectionDets(string destinationDatabase, string databaseType1, string backOfficeConnectionString, string destinationType, bool canOverrideCacheAttributes, bool isCacheEnabled, int cacheSeconds)
			{
				_DestinationDatabase = destinationDatabase;
				_DatabaseType1 = databaseType1;
				_BackOfficeConnectionString = backOfficeConnectionString;
				_destinationType = destinationType;
				_canOverrideCacheAttributes = canOverrideCacheAttributes;
				_isCacheEnabled = isCacheEnabled;
				_cacheSeconds = cacheSeconds;
			}
			#endregion
		}
	}
}
