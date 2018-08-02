using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TalentSystemDefaults.DataAccess.ConfigObjects;
using TalentSystemDefaults.DataAccess.DataObjects;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	public class DMFactory
	{
		private const string DL_STR = "DL";
		private const string DM_STR = "DM";
		public static DMBase GetDMObject(DESettings settings, bool initialiseData = true)
		{
			string talentModuleName = settings.Module_Name;
			string className = Utilities.GetClassName(GlobalConstants.NS_TALENTMODULES, talentModuleName);
			object[] parameters = new object[] { settings, initialiseData };
			DMBase obj = ReflectionUtils.CreateInstance(className, parameters);
			return obj;
		}
		public static bool IsiSeriesTable(string tableName)
		{
			bool returnValue = false;
			Type type = ReflectionUtils.GetTypeInfo(Utilities.GetClassName(GlobalConstants.NS_DATAACCESS_DATAOBJECTS, tableName));
			PropertyInfo prop = type.GetProperties().Where(p => p.Name == "IsiSeriesTable").FirstOrDefault();
			if (prop != null)
			{
				returnValue = System.Convert.ToBoolean(prop.GetValue(null));
			}
			return returnValue;
		}
		public static bool EnableSelectedColumn(string tableName)
		{
			bool returnValue = false;
			Type type = ReflectionUtils.GetTypeInfo(Utilities.GetClassName(GlobalConstants.NS_DATAACCESS_DATAOBJECTS, tableName));
			PropertyInfo prop = type.GetProperties().Where(p => p.Name == "EnableSelectedColumns").FirstOrDefault();
			if (prop != null)
			{
				returnValue = System.Convert.ToBoolean(prop.GetValue(null));
			}
			return returnValue;
		}
		public static DBObjectBase GetDBObject(string tableName, DESettings settings)
		{
			string className = Utilities.GetClassName(GlobalConstants.NS_DATAACCESS_DATAOBJECTS, tableName);
			object[] parameters = new object[] { settings };
			DBObjectBase obj = ReflectionUtils.CreateInstance(className, parameters);
			return obj;
		}
		public static DBObjectBase GetModuleObject(string moduleObjectName, DESettings settings)
		{
			string className = Utilities.GetClassName(GlobalConstants.NS_DATAACCESS_MODULEOBJECTS, moduleObjectName);
			object[] parameters = new object[] { settings };
			DBObjectBase obj = ReflectionUtils.CreateInstance(className, parameters);
			return obj;
		}
		public static DLBase GetDLObject(string talentListName, DESettings settings, Dictionary<string, string> filters = null)
		{
			string className = Utilities.GetClassName(GlobalConstants.NS_TALENTLISTS, talentListName);
			object[] parameters = new object[] { settings, filters };
			DLBase obj = ReflectionUtils.CreateInstance(className, parameters);
			return obj;
		}
		public static string[] GetTableNames()
		{
			var q = from t in ReflectionUtils.GetTypes()
					where t.IsClass && (t.Namespace == GlobalConstants.NS_DATAACCESS_DATAOBJECTS && t.DeclaringType == null)
					select t.Name;
			return q.ToArray();
		}
		public static Dictionary<string, string> GetDefaultKeyValues(string tableName)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			Type type = ReflectionUtils.GetTypeInfo(Utilities.GetClassName(GlobalConstants.NS_DATAACCESS_DATAOBJECTS, tableName));
			if (type != null)
			{
				foreach (var prop in type.GetProperties())
				{
					if (prop.Name.Contains("DefaultKey"))
					{
						result.Add(prop.Name, System.Convert.ToString(prop.GetValue(null)));
					}
				}
			}
			return result;
		}
		public static bool IsDisplayNameEnabled(string tableName)
		{
			bool result = false;
			Type type = ReflectionUtils.GetTypeInfo(Utilities.GetClassName(GlobalConstants.NS_DATAACCESS_DATAOBJECTS, tableName));
			if (type != null)
			{
				PropertyInfo prop = type.GetProperties().Where(p => p.Name == "DefaultNameActive").FirstOrDefault();
				if (prop != null)
				{
					result = System.Convert.ToBoolean(prop.GetValue(null));
				}
			}
			return result;
		}
		public static Dictionary<string, string> GetVariableKeyValues(string tableName)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			Type type = ReflectionUtils.GetTypeInfo(Utilities.GetClassName(GlobalConstants.NS_DATAACCESS_DATAOBJECTS, tableName));
			if (type != null)
			{
				foreach (var prop in type.GetProperties())
				{
					if (prop.Name.Contains("VariableKey"))
					{
						result.Add(prop.Name, System.Convert.ToString(prop.GetValue(null)));
					}
				}
			}
			return result;
		}
		public static string IsBaseDefinitionEnabled(string tableName)
		{
			string result = string.Empty;
			Type type = ReflectionUtils.GetTypeInfo(Utilities.GetClassName(GlobalConstants.NS_DATAACCESS_DATAOBJECTS, tableName));
			if (type != null)
			{
				PropertyInfo prop = type.GetProperties().Where(p => p.Name == "BaseDefinition").FirstOrDefault();
				if (prop != null)
				{
					result = System.Convert.ToString(prop.GetValue(null));
				}
			}
			return System.Convert.ToString(result == true.ToString() ? true : false);
		}
		public static string[] GetDisplayTabs(DESettings settings)
		{
			tbl_page_text_lang textLang = new tbl_page_text_lang(ref settings);
			var pageTexts = textLang.GetTextsForPage(settings.DefaultBusinessUnit, "SystemDefaults.aspx");
			var displayTabs = new DisplayTabs(pageTexts);
			Array results = (Array)(displayTabs.Classes.Select(item => item.Key).ToArray());
			return (string[])results;
		}
		public static string[] GetDataTypes()
		{
			Array results = Enum.GetNames(typeof(DataType));
			return (string[])results;
		}
		public static List<ModuleEntity> GetModuleEntities()
		{
			System.Collections.Generic.List<ModuleEntity> types = new System.Collections.Generic.List<ModuleEntity>();
			foreach (Type t in ReflectionUtils.GetTypes())
			{
				if (t.IsClass &&
						(t.Namespace == GlobalConstants.NS_TALENTLISTS || t.Namespace == GlobalConstants.NS_TALENTMODULES))
				{
					PropertyInfo enableAsModuleProp = t.GetProperties().Where(p => p.Name == "EnableAsModule").FirstOrDefault();
					PropertyInfo moduleTitleProp = t.GetProperties().Where(p => p.Name == "ModuleTitle").FirstOrDefault();
					if (enableAsModuleProp != null)
					{
						bool enableAsModule = System.Convert.ToBoolean(enableAsModuleProp.GetValue(null));
						if (enableAsModule)
						{
							ModuleType type = t.Namespace == GlobalConstants.NS_TALENTLISTS ? ModuleType.List : ModuleType.Module;
							string name = t.Name.Replace(DL_STR, string.Empty).Replace(DM_STR, string.Empty);
							string title = System.Convert.ToString(moduleTitleProp.GetValue(null));
							types.Add(new ModuleEntity(name, title, type));
						}
					}
				}
			}
			return types;
		}
		public static dynamic GetModuleNames()
		{
			object results = ReflectionUtils.GetTypes().Where(t =>
			{
				bool result = t.IsClass && (t.Namespace == GlobalConstants.NS_TALENTMODULES) && (t.DeclaringType == null); // to prevent the inner classes (E.g. AccordionGroup)
				return result;
			}).Select(t =>
			{
				return new { Name = t.GetProperty("ModuleTitle").GetValue(null).ToString(), Value = t.Name.Replace("DM", string.Empty) };
			}).ToList();
			return results;
		}
		public static ConfigurationEntity[] GetDMConfigurations(DESettings settings, string talentModuleName)
		{
			string className = Utilities.GetClassName(GlobalConstants.NS_TALENTMODULES, talentModuleName);
			TypeInfo type = (TypeInfo)(ReflectionUtils.GetTypeInfo(className));
			System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
			foreach (FieldInfo field in type.GetFields((System.Reflection.BindingFlags)(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
			{
				list.Add(System.Convert.ToString(field.GetValue(null)));
			}
			tbl_config_detail configDetail = new tbl_config_detail(settings);
			return configDetail.RetrieveConfigurationEntities(list);
		}
	}
}
