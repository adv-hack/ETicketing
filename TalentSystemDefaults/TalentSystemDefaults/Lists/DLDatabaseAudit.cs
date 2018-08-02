using System.Collections.Generic;
using System.Data;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	namespace TalentLists
	{
		public class DLDatabaseAudit : DLBase
		{
			static private bool _EnableAsModule = false;
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
			static private string _ModuleTitle = "Database Audits";
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
			public DLDatabaseAudit(DESettings settings, Dictionary<string, string> filters = null)
				: base(settings, filters)
			{
				if (string.IsNullOrEmpty(settings.BusinessUnit))
				{
					settings.BusinessUnit = settings.DefaultBusinessUnit;
				}
			}
			protected override void Initialise()
			{
				DEList.Title = "Database Audits";
				DEList.ModuleName = "DatabaseAudit";
				DEList.TableName = "tbl_config_detail_audit";
				DEList.EnableSelectColumn = true;
				DEList.EnableSelectAsHyperLink = true;
				DEList.DataColumns.Add(new DataColumn("ID", "Column1", "Group ID", ColumnType.Text));
				DEList.DataColumns.Add(new DataColumn("USER_NAME", "Column2", "User", ColumnType.Text));
				DEList.DataColumns.Add(new DataColumn("TIMESTAMP", "Column3", "Date Time", ColumnType.Text));
				DEList.DataColumns.Add(new DataColumn("MODULE_NAME", "Column4", "Module", ColumnType.Text));
				DEList.DataColumns.Add(new DataColumn("DATA_SOURCE", "Column5", "Data Source", ColumnType.Text));
				DEList.DataColumns.Add(new DataColumn("CATALOG", "Column6", "Catalog", ColumnType.Text));
				DEList.DataColumns.Add(new DataColumn("BUSINESS_UNIT", "Column7", "Business Unit", ColumnType.Text));
				DEList.DataColumns.Add(new DataColumn("COMMAND_COUNT", "Column8", "Command Count", ColumnType.Text));
				DEList.VariableKeyColumns.Add(new KeyColumn("ID", "VariableKey1"));
			}
			protected override void RetrieveData()
			{
				DataAccess.ConfigObjects.tbl_config_detail_audit tblAudit = new DataAccess.ConfigObjects.tbl_config_detail_audit(ref settings);
				DataTable dtForList = tblAudit.RetrieveDTForList();
				DEList.Data = GetDEEntities(DEList.DataColumns, DEList.VariableKeyColumns, dtForList);
			}
			protected override void Format(ref System.Collections.Generic.List<DEListDetail> items)
			{
			}
			protected override string GetSelectURL(DEListDetail listDetail)
			{
				return string.Format("/DatabaseUpdates.aspx?moduleName={0}&businessUnit={1}&variablekey1={2}", DEList.ModuleName, settings.BusinessUnit, listDetail.VariableKeys[0].Replace("\"", string.Empty));
			}
		}
	}
}
