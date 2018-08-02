using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TalentSystemDefaults.DataAccess.DataObjects.SystemObjects;
namespace TalentSystemDefaults
{
	[Serializable()]
	public class Utilities
	{
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public static string GetAllString()
		{
			string allString = "*ALL";
			return allString;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static string CheckForDBNull_String(object obj)
		{
			if (obj.Equals(DBNull.Value) || obj.Equals(string.Empty))
			{
				return string.Empty;
			}
			else
			{
				return (obj).ToString();
			}
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static int CheckForDBNull_Int(object obj)
		{
			if (obj.Equals(DBNull.Value) || obj.Equals(string.Empty))
			{
				return 0;
			}
			else
			{
				return System.Convert.ToInt32(obj);
			}
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static bool CheckForDBNull_Boolean_DefaultFalse(object obj)
		{
			if (obj.Equals(DBNull.Value) || obj.Equals(string.Empty))
			{
				return false;
			}
			else
			{
				return System.Convert.ToBoolean(obj);
			}
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="paramName"></param>
		/// <param name="paramValue"></param>
		/// <param name="paramType"></param>
		/// <param name="paramDirection"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static DESQLParameter ConstructParameter(string paramName, string paramValue, SqlDbType paramType = SqlDbType.NVarChar, ParameterDirection paramDirection = ParameterDirection.Input)
		{
			DESQLParameter parameterPiece = new DESQLParameter();
			parameterPiece.ParamDirection = paramDirection;
			parameterPiece.ParamName = paramName;
			parameterPiece.ParamType = paramType;
			parameterPiece.ParamValue = paramValue;
			return parameterPiece;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="cmd"></param>
		/// <remarks></remarks>
		public static void LogSQL(DESQLCommand cmd)
		{
			var sql = cmd.CommandText;
			foreach (DESQLParameter p in cmd.CommandParameter)
			{
				sql = sql.Replace(p.ParamName, String.Format("'{0}'", p.ParamValue));
			}
			Console.WriteLine(sql);
		}

		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="namespaceString"></param>
		/// <param name="className"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static string GetClassName(string namespaceString, string className)
		{
			if (string.IsNullOrWhiteSpace(className))
			{
				className = string.Empty;
			}
			switch (namespaceString)
			{
				case GlobalConstants.NS_DATAACCESS_CONFIGOBJECTS:
					className = GlobalConstants.NS_DATAACCESS_CONFIGOBJECTS + "." + className;
					break;
				case GlobalConstants.NS_DATAACCESS_DATAOBJECTS:
					className = GlobalConstants.NS_DATAACCESS_DATAOBJECTS + "." + className;
					break;
				case GlobalConstants.NS_DATAACCESS_MODULEOBJECTS:
					className = GlobalConstants.NS_DATAACCESS_MODULEOBJECTS + "." + className;
					break;
				case GlobalConstants.NS_TALENTMODULES:
					className = GlobalConstants.NS_TALENTMODULES + "." + GlobalConstants.CLASSNAME_SUFFIX_TALENTMODULES + className;
					break;
				case GlobalConstants.NS_TALENTLISTS:
					className = GlobalConstants.NS_TALENTLISTS + "." + GlobalConstants.CLASSNAME_SUFFIX_TALENTLISTS + className;
					break;
			}
			return className;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static string Stringify(Dictionary<string, string> values)
		{
			System.Collections.Generic.List<string> results = new System.Collections.Generic.List<string>();
			foreach (var key in values.Keys)
			{
				results.Add(string.Format("{0}={1}", key, values[key]));
			}
			return string.Join(",", results.ToArray());
		}
		/// <summary>
		/// Get next available transaction group id
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static int GetGroupID(DESettings settings)
		{
			TalentSystemDefaults.DataAccess.DataObjects.tbl_ecommerce_module_defaults_bu tblEcommerceDefaults = new TalentSystemDefaults.DataAccess.DataObjects.tbl_ecommerce_module_defaults_bu(ref settings);
			int retVal = 0;
			retVal = tblEcommerceDefaults.GetGroupID();
			return retVal;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static Dictionary<string, string> Constructify(string value)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			string[] items = value.Split(",".ToCharArray());
			foreach (string item in items)
			{
				var values = item.Split("=".ToCharArray());
				result.Add(values[0], values[1]);
			}
			return result;
		}
		/// <summary>
		/// XML Comment
		/// </summary>
		/// <param name="objtalentSqlAccessDetail"></param>
		/// <param name="settings"></param>
		/// <remarks></remarks>
		public static void SerializeTransaction(TalentDataAccess objtalentSqlAccessDetail, DESettings settings)
		{
			DataAccess.ConfigObjects.tbl_config_detail_audit objAudit = new DataAccess.ConfigObjects.tbl_config_detail_audit(ref settings);
			MemoryStream ms = new MemoryStream();
			BinaryFormatter bf = new BinaryFormatter();
			// serialize the object
			bf.Serialize(ms, objtalentSqlAccessDetail);
			if (settings.GroupID == "")
			{
				settings.GroupID = GetGroupID(settings).ToString();
			}
			// add it to DB
			objAudit.Insert(ms, settings.GroupID, settings.Module_Name, settings.AgentName, objtalentSqlAccessDetail.TableName, objtalentSqlAccessDetail.Action, objtalentSqlAccessDetail.DataSource, objtalentSqlAccessDetail.Catalog, settings.BusinessUnit);
		}
		/// <summary>
		/// Compare two strings values
		/// </summary>
		/// <param name="StringValue1"></param>
		/// <param name="StringValue2"></param>
		/// <param name="IgnoreCase">Optional parameter</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static bool MatchStrings(string StringValue1, string StringValue2, bool IgnoreCase = true)
		{
			bool isMatched = false;
			if (StringValue1.Length == StringValue2.Length)
			{
				isMatched = (string.Compare(StringValue1, StringValue2, IgnoreCase) == 0) ? true : false;
			}
			return isMatched;
		}
		/// <summary>
		/// Returns if one string value contains another string value
		/// </summary>
		/// <param name="StringValue1"></param>
		/// <param name="StringValue2"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static bool CompareStrings(string StringValue1, string StringValue2)
		{
			return StringValue1.IndexOf(StringValue2, StringComparison.OrdinalIgnoreCase) >= 0;
		}
		public static string[] GetBusinessUnits(DESettings settings)
		{
			System.Collections.Generic.List<string> buList = new System.Collections.Generic.List<string>();
			string application = "EBusiness";
			DataAccess.DataObjects.SystemObjects.tbl_url_bu objUrlBU = new DataAccess.DataObjects.SystemObjects.tbl_url_bu(ref settings);
			DataTable dataTable = objUrlBU.RetrieveBusinessUnit(application);
			foreach (DataRow row in dataTable.Rows)
			{
				buList.Add(row["BUSINESS_UNIT"].ToString());
			}
			return buList.ToArray();
		}
		public static string GetHTMLEncodedStringForDB(string value)
		{
			string retVal = string.Empty;
			try
			{
				retVal = value.Replace(">", "&gt;");
				retVal = retVal.Replace("<", "&lt;");
				retVal = retVal.Replace("&", "&amp;");
			}
			catch (Exception)
			{
				retVal = "";
			}
			return retVal;
		}

		public static bool InsertErrorLog(DESettings settings, string sourceClass, string sourceMethod, string logCode, string logFilter1, string logFilter2, string logFilter3, string logFilter4, string logFilter5, string logContent)
		{
			bool retVal = false;
			tbl_logs tblLogs = new tbl_logs(settings);
			retVal = tblLogs.InsertLog(sourceClass, sourceMethod, logCode, logFilter1, logFilter2, logFilter3, logFilter4, logFilter5, logContent);
			return retVal;
		}
	}
}
