using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using SystemDefaultsAPI.UtilityClasses;
using TalentSystemDefaults;
using TalentSystemDefaults.DataEntities;
namespace SystemDefaultsAPI
{
	namespace Models
	{
		public class DatabaseSearchModel
		{
			#region Private Variables
			private const string TEXTSEARCH = "TEXTSEARCH";
			private const string PAYMENTTYPESEARCH = "PAYMENTTYPESEARCH";
			private List<SearchDataConfigs> _dataConfigs;
			#endregion
			#region Public Variables
			public enum ConfigurationTypes
			{
				Column = 1,
				Table = 2
			}
			public DESettings settings { get; set; }
			public string StatusMessage { get; set; }
			private bool _hasStatus = false;
			public bool hasStatus
			{
				get
				{
					return _hasStatus;
				}
				set
				{
					_hasStatus = value;
				}
			}
			public string GETDataUrl { get; set; }
			public string UPDATEDataUrl { get; set; }
			private string _Title = "Database Search";
			public string Title
			{
				get
				{
					return _Title;
				}
				set
				{
					_Title = value;
				}
			}
			public List<SearchDataConfigs> DataConfigs
			{
				get
				{
					return _dataConfigs;
				}
			}
			public string TablesInformation
			{
				get
				{
					return GetJSON(System.Convert.ToInt32(ConfigurationTypes.Table));
				}
			}
			public string ColumnsInformation
			{
				get
				{
					return GetJSON(System.Convert.ToInt32(ConfigurationTypes.Column));
				}
			}
			#endregion
			#region Public Methods
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="settings"></param>
			/// <remarks></remarks>
			public DatabaseSearchModel(DESettings settings)
			{
				this.settings = settings;
				Initialise();
			}
			/// <summary>
			/// Prepares class instance to fulfil data requests
			/// </summary>
			/// <remarks></remarks>
			public void Initialise()
			{
				PrepareConfigurations();
			}
			// ''' <summary>
			// ''' Returns data to build a data grid
			// ''' </summary>
			// ''' <param name="params"></param>
			// ''' <returns></returns>
			// ''' <remarks></remarks>
			//Public Function GetData(ByVal params As Dictionary(Of String, String)) As JQueryDataTable
			//    Dim gridData As New JQueryDataTable
			//    Dim offset As Integer = params("start")
			//    Dim pageSize As Integer = params("length")
			//    Dim columnNames As String() = params("columnnames").Split(",")
			//    Dim columnIndex = CType(params("order[0].column"), Integer)
			//    Dim sortColumn As String = columnNames(columnIndex)
			//    Dim sortOrder As String = params("order[0].dir")
			//    Dim searchText As String = params("searchtext")
			//    Dim searchType As String = params("searchtype")
			//    Dim searchData As New DataSet
			//    Dim totalRecords As Integer
			//    Dim sortingOrder As Boolean
			//    sortingOrder = IIf(sortOrder.ToLower = "asc", False, True)
			//    Select Case searchType
			//        Case Is = "TEXTSEARCH"
			//            Dim dbTextSearch As New DBTextSearch(settings)
			//            searchData = dbTextSearch.TextSearch(searchText, offset, pageSize, sortColumn, IIf(sortOrder.ToLower = "asc", False, True))
			//        Case Is = "PAYMENTTYPESEARCH"
			//            Dim dbPaymentTypeSearch As New DBPaymentTypeSearch(settings)
			//            searchData = dbPaymentTypeSearch.PaymentTypeSearch(searchText, offset, pageSize, sortColumn, IIf(sortOrder.ToLower = "asc", False, True))
			//    End Select
			//    totalRecords = CType(searchData.Tables(0).Rows(0)(0), Integer)
			//    gridData.draw = params("draw")
			//    gridData.recordsTotal = totalRecords
			//    gridData.recordsFiltered = totalRecords
			//    gridData.data = ConvertDataTableToList(searchData.Tables(1))
			//    Return gridData
			//End Function
			/// <summary>
			/// Returns data to build a data grid
			/// </summary>
			/// <param name="params"></param>
			/// <param name="searchType"></param>
			/// <param name="offset"></param>
			/// <param name="pageSize"></param>
			/// <param name="sortColumn"></param>
			/// <param name="sortOrder"></param>
			/// <param name="searchText"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public JQueryDataTable GetData(Dictionary<string, string> @params, int draw, string searchType, int offset, int pageSize, string sortColumn, string sortOrder, string searchText)
			{
				JQueryDataTable gridData = new JQueryDataTable();
				DataSet searchData = new DataSet();
				int totalRecords = 0;
				bool sortingOrder;
				sortingOrder = sortOrder.ToLower() == "asc" ? false : true;
				switch (searchType)
				{
					case "TEXTSEARCH":
						TalentSystemDefaults.DESettings temp_settings = settings;
						Database_Text_Search dbTextSearch = new Database_Text_Search(ref temp_settings);
						settings = temp_settings;
						searchData = dbTextSearch.TextSearch(searchText, offset, pageSize, sortColumn, sortOrder.ToLower() == "asc" ? false : true);
						break;
					case "PAYMENTTYPESEARCH":
						TalentSystemDefaults.DESettings temp_settings2 = settings;
						Database_PaymentType_Search dbPaymentTypeSearch = new Database_PaymentType_Search(ref temp_settings2);
						settings = temp_settings2;
						searchData = dbPaymentTypeSearch.PaymentTypeSearch(searchText, offset, pageSize, sortColumn, sortOrder.ToLower() == "asc" ? false : true);
						break;
				}
				totalRecords = System.Convert.ToInt32(searchData.Tables[0].Rows[0][0]);
				gridData.draw = draw;
				gridData.recordsTotal = totalRecords;
				gridData.recordsFiltered = totalRecords;
				gridData.data = ConvertDataTableToList(searchData.Tables[1]);
				return gridData;
			}
			/// <summary>
			/// Updates data modified inside a data grid
			/// </summary>
			/// <param name="params"></param>
			/// <param name="searchType"></param>
			/// <param name="jsonData"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public bool Save(Dictionary<string, string> @params, string searchType, string jsonData)
			{
				bool retVal = true;
				JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
				object objEntity = new object();
				Database_Text_Search dbTextSearch = default(Database_Text_Search);
				List<TextSearchEntity> lstTextSearchEntity = new List<TextSearchEntity>();
				TextSearchEntity objTextSearchEntity = default(TextSearchEntity);
				var typeTextSearchUpdatedData = (new List<TextSearchUpdateData>()).GetType();
				List<SearchDataConfigs> lstTables = null;
				Dictionary<string, string> tableList = new Dictionary<string, string>();
				switch (searchType)
				{
					case "TEXTSEARCH":
						objEntity = jsSerializer.Deserialize(jsonData, typeTextSearchUpdatedData);
						try
						{
							TalentSystemDefaults.DESettings temp_settings = settings;
							dbTextSearch = new Database_Text_Search(ref temp_settings);
							settings = temp_settings;
							lstTables = DataConfigs.FindAll(x => x.ConfigType == System.Convert.ToInt32(ConfigurationTypes.Table) & x.SearchType == TEXTSEARCH);
							foreach (var tableConfig in lstTables)
							{
								tableList.Add(tableConfig.TableName, tableConfig.ConfigIndex.ToString());
							}
							// convert into an entity object
							foreach (TextSearchUpdateData UpdatedRow in ((List<TextSearchUpdateData>)objEntity))
							{
								objTextSearchEntity = new TextSearchEntity();
								objTextSearchEntity.ID = int.Parse(UpdatedRow.id);
								objTextSearchEntity.TableName = UpdatedRow.dbTable;
								objTextSearchEntity.Text_Code = UpdatedRow.text_code;
								objTextSearchEntity.Text_Content = UpdatedRow.text_content;
								lstTextSearchEntity.Add(objTextSearchEntity);
							}
							retVal = dbTextSearch.SaveTextSearchData(lstTextSearchEntity, tableList);
						}
						catch (Exception)
						{
							throw;
						}
						break;
					case "PAYMENTTYPESEARCH":
						break;
				}
				return retVal;
			}
			// todo: give some meaningful name
			// TBD: receive two parameters from the page instead of such object. should not have unnecessary class(ReplaceData) just for 2 fields
			#endregion
			#region Private Methods
			/// <summary>
			/// Returns list of columnName-Value pair
			/// </summary>
			/// <param name="tblTextLang"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private List<object> ConvertDataTableToList(DataTable tblTextLang)
			{
				List<object> listObj = new List<object>();
				Hashtable hashTable = new Hashtable();
				foreach (DataRow dr in tblTextLang.Rows)
				{
					hashTable = new Hashtable();
					foreach (System.Data.DataColumn dc in tblTextLang.Columns)
					{
						if (dc.DataType == typeof(int))
						{
							hashTable.Add(dc.ColumnName, (Information.IsDBNull(dr[dc.ColumnName])) ? "" : (dr[dc.ColumnName]));
						}
						else
						{
							hashTable.Add(dc.ColumnName, (Information.IsDBNull(dr[dc.ColumnName])) ? "" : (HttpUtility.HtmlEncode(dr[dc.ColumnName].ToString())));
						}
					}
					listObj.Add(hashTable);
				}
				return listObj;
			}
			/// <summary>
			/// Prepare all configurations regarding to different search types available on the page
			/// </summary>
			/// <remarks></remarks>
			private void PrepareConfigurations()
			{
				_dataConfigs = new List<SearchDataConfigs>();
				///'''''''''''''''''''''''''''''''''''''''''''''''''
				// Text Lang
				///'''''''''''''''''''''''''''''''''''''''''''''''''
				_dataConfigs.Add(new SearchDataConfigs(System.Convert.ToInt32(ConfigurationTypes.Column), "ID", "ID", "text", TEXTSEARCH, true, false, false, true));
				_dataConfigs.Add(new SearchDataConfigs(System.Convert.ToInt32(ConfigurationTypes.Column), "BUSINESS_UNIT", "Business Unit", "text", TEXTSEARCH, true, true, false, true));
				_dataConfigs.Add(new SearchDataConfigs(System.Convert.ToInt32(ConfigurationTypes.Column), "MODULE", "Module", "text", TEXTSEARCH, true, true, false, false));
				_dataConfigs.Add(new SearchDataConfigs(System.Convert.ToInt32(ConfigurationTypes.Column), "TEXT_CODE", "Text Code", "text", TEXTSEARCH, true, true, false, false));
				_dataConfigs.Add(new SearchDataConfigs(System.Convert.ToInt32(ConfigurationTypes.Column), "TEXT_CONTENT", "Content", "text", TEXTSEARCH, false, true, true, false));
				_dataConfigs.Add(new SearchDataConfigs(0, System.Convert.ToInt32(ConfigurationTypes.Table), "tbl_page_text_lang", TEXTSEARCH));
				_dataConfigs.Add(new SearchDataConfigs(1, System.Convert.ToInt32(ConfigurationTypes.Table), "tbl_control_text_lang", TEXTSEARCH));
				///'''''''''''''''''''''''''''''''''''''''''''''''''
				// Payment types
				///'''''''''''''''''''''''''''''''''''''''''''''''''
				_dataConfigs.Add(new SearchDataConfigs(System.Convert.ToInt32(ConfigurationTypes.Column), "ID", "ID", "text", PAYMENTTYPESEARCH, true, true, false, true));
				_dataConfigs.Add(new SearchDataConfigs(System.Convert.ToInt32(ConfigurationTypes.Column), "DESCRIPTION", "Description", "text", PAYMENTTYPESEARCH, true, true, false, false));
				_dataConfigs.Add(new SearchDataConfigs(0, System.Convert.ToInt32(ConfigurationTypes.Table), "tbl_payment_type", PAYMENTTYPESEARCH));
			}
			/// <summary>
			/// Returns JSON object having set all columns/tables configurations
			/// </summary>
			/// <param name="configType"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			private string GetJSON(int configType)
			{
				List<SearchDataConfigs> lstConfigurations = default(List<SearchDataConfigs>);
				JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
				List<Dictionary<string, object>> arrConfigurations = new List<Dictionary<string, object>>();
				Dictionary<string, object> configurationProperty = default(Dictionary<string, object>);
				lstConfigurations = DataConfigs.FindAll(x => x.ConfigType == configType);
				foreach (SearchDataConfigs configuration in lstConfigurations)
				{
					configurationProperty = new Dictionary<string, object>();
					configurationProperty.Add("searchType", configuration.SearchType);
					if (configuration.ConfigType == 2)
					{
						configurationProperty.Add("idx", configuration.ConfigIndex);
						configurationProperty.Add("table", configuration.TableName);
					}
					else
					{
						configurationProperty.Add("dbColumn", configuration.ColumnName);
						configurationProperty.Add("header", configuration.ColumnHeaderText);
						configurationProperty.Add("colType", configuration.ColumnType);
						configurationProperty.Add("sort", configuration.IsSortable ? "1" : "0");
						configurationProperty.Add("show", configuration.IsVisible ? "1" : "0");
						configurationProperty.Add("edit", configuration.IsEditable ? "1" : "0");
						configurationProperty.Add("pk", configuration.IsPrimaryKey ? "1" : "0");
					}
					arrConfigurations.Add(configurationProperty);
				}
				return jsSerializer.Serialize(arrConfigurations);
			}
			#endregion
		}
		// todo: Further we should think about moving this class to some other location
		/// <summary>
		/// DataConfigs contains all table/column configurations for the grid
		/// </summary>
		/// <remarks></remarks>
		public class SearchDataConfigs
		{
			public int ConfigType { get; set; } //1: Column 2: Table
			public int ConfigIndex { get; set; }
			public string TableName { get; set; }
			public string ColumnName { get; set; }
			public string ColumnHeaderText { get; set; }
			public string ColumnType { get; set; }
			public string SearchType { get; set; }
			public bool IsSortable { get; set; }
			public bool IsVisible { get; set; }
			public bool IsEditable { get; set; }
			public bool IsPrimaryKey { get; set; }
			// column configuration constructor
			public SearchDataConfigs(int configType, string ColumnName, string ColumnHeaderText, string ColumnType, string SearchType, bool IsSortable, bool IsVisible, bool IsEditable, bool IsPrimaryKey)
			{
				this.ConfigType = configType;
				this.ColumnName = ColumnName;
				this.ColumnHeaderText = ColumnHeaderText;
				this.ColumnType = ColumnType;
				this.SearchType = SearchType;
				this.IsSortable = IsSortable;
				this.IsVisible = IsVisible;
				this.IsEditable = IsEditable;
				this.IsPrimaryKey = IsPrimaryKey;
			}
			// table configuration constructor
			public SearchDataConfigs(int configIndex, int configType, string TableName, string SearchType)
			{
				this.ConfigIndex = configIndex;
				this.ConfigType = configType;
				this.TableName = TableName;
				this.SearchType = SearchType;
			}
		}
	}
}
