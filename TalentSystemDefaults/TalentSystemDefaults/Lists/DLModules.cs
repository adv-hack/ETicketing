using System.Collections.Generic;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	namespace TalentLists
	{
		public class DLModules : DLBase
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
			static private string _ModuleTitle = "Modules";
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
			public DLModules(DESettings settings, Dictionary<string, string> filters = null)
				: base(settings, filters)
			{
				//Modules page need to retrieve business units, before that talent data acccess need to retrieve
				//connection details using destination database, and for that it uses default business unit
				if (string.IsNullOrEmpty(settings.BusinessUnit))
				{
					settings.BusinessUnit = settings.DefaultBusinessUnit;
				}
			}
			const string SystemDefaultURL = "/SystemDefaults.aspx?moduleName={0}&businessUnit={1}";
			const string DefaultListURL = "/DefaultList.aspx?listname={0}&businessUnit={1}";
			protected override void Initialise()
			{
				DEList.Title = "Modules";
				DEList.ModuleName = "DatabaseAudit";
				DEList.EnableBUSelector = true;
				DEList.EnableSelectAsHyperLink = true;
				DEList.DataColumns.Add(new DataColumn("NAME", "Column1", "Name", ColumnType.Text));
			}
			protected override void RetrieveData()
			{
				System.Collections.Generic.List<ModuleEntity> entities = DMFactory.GetModuleEntities();
				System.Collections.Generic.List<DEListDetail> data = new System.Collections.Generic.List<DEListDetail>();
				DEListDetail listDetail = default(DEListDetail);
				foreach (ModuleEntity entity in entities)
				{
					listDetail = new DEListDetail(new string[] { entity.Title }, new string[] { });
					if (entity.Type == ModuleType.List)
					{
						listDetail.SelectURL = string.Format(DefaultListURL, entity.Name, "{1}");
					}
					else
					{
						listDetail.SelectURL = string.Format(SystemDefaultURL, entity.Name, "{1}");
					}
					data.Add(listDetail);
				}
				DEList.Data = data;
				DEList.BusinessUnits = Utilities.GetBusinessUnits(settings);
			}
		}
	}
}
