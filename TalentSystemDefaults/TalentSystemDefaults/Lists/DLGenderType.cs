using System.Collections.Generic;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	namespace TalentLists
	{
		public class DLGenderType : DLBase
		{
			static private bool _EnableAsModule = true;
			public static bool EnableAsModule
			{
				get
				{
					return _EnableAsModule;
				}
				set
				{
					_EnableAsModule = value;
				}
			}
			static private string _ModuleTitle = "Gender Types";
			public static string ModuleTitle
			{
				get
				{
					return _ModuleTitle;
				}
				set
				{
					_ModuleTitle = value;
				}
			}
			public DLGenderType(DESettings settings, Dictionary<string, string> filters = null)
				: base(settings, filters)
			{
			}
			protected override void Initialise()
			{
				DEList.ModuleObjectName = "GenderType";
				DEList.FunctionName = "RetrieveDTForList";
				DEList.Title = "Gender Types";
				DEList.ModuleName = "GenderType";
				DEList.EnableSelectColumn = false;
				DEList.EnableEditColumn = true;
				DEList.EnableDeleteColumn = true;
				DEList.DataColumns.Add(new DataColumn("GENDER_TYPE_CODE", "Column1", "Code", ColumnType.Text));
				DEList.DataColumns.Add(new DataColumn("GENDER_TYPE_DESCRIPTION", "Column2", "Description", ColumnType.Text));
				DEList.DataColumns.Add(new DataColumn("IS_OTHER_TYPE", "Column3", "Type", ColumnType.Text));
				DEList.VariableKeyColumns.Add(new KeyColumn("GENDER_TYPE_CODE", "VariableKey1"));
				DEList.ActionButtons.Add("Add");
			}
			protected override void Format(ref System.Collections.Generic.List<DEListDetail> items)
			{
				foreach (DEListDetail item in items)
				{
					item.EnableRowWiseEdit = true;
					if (item.Columns[2] == "0")
					{
						item.EnableRowWiseDelete = true; 
					}
					item.Columns[2] = (item.Columns[2].ToString() == "0" ? "User" : "System");
					string key = item.Columns[0];
				}
			}
			protected override string GetAddURL()
			{
				return string.Format("/SystemDefaults.aspx?moduleName={0}&businessUnit={1}&variablekey1=&mode=create", DEList.ModuleName, base.settings.BusinessUnit);
			}
			protected override string GetUpdateURL()
			{
				return string.Format("/SystemDefaults.aspx?moduleName={0}&businessUnit={1}&variablekey1={2}", DEList.ModuleName, base.settings.BusinessUnit, "{2}");
			}
			protected override string GetDeleteURL()
			{
				return string.Format("/DefaultList.aspx?listname={0}&businessUnit={1}", DEList.ModuleName, base.settings.BusinessUnit);
			}
		}
	}
}
