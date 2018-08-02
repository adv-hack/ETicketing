using System.Collections.Generic;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	namespace TalentLists
	{
		public class DLPaymentType : DLBase
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
			static private string _ModuleTitle = "Payment Types";
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
			public DLPaymentType(DESettings settings, Dictionary<string, string> filters = null)
				: base(settings, filters)
			{
			}
			protected override void Initialise()
			{
				DEList.ModuleObjectName = "PaymentType";
				DEList.FunctionName = "RetrieveDTForList";
				DEList.Title = "Payment Types";
				DEList.ModuleName = "PaymentType";
				DEList.EnableSelectColumn = false;
				DEList.EnableEditColumn = true;
				DEList.EnableDeleteColumn = true;
				DEList.DataColumns.Add(new DataColumn("PAYMENT_TYPE_CODE", "Column1", "Code", ColumnType.Text));
				DEList.DataColumns.Add(new DataColumn("PAYMENT_TYPE_DESCRIPTION", "Column2", "Description", ColumnType.Text));
				DEList.DataColumns.Add(new DataColumn("IS_OTHER_TYPE", "Column3", "Type", ColumnType.Text));
				DEList.DataColumns.Add(new DataColumn("DEFAULT_PAYMENT_TYPE", "Column4", "Default", ColumnType.Text)); //tbl_payment_type_bu
				DEList.DataColumns.Add(new DataColumn("VALU51", "Column5", "TalentCode", ColumnType.Text)); //MD501
				DEList.VariableKeyColumns.Add(new KeyColumn("PAYMENT_TYPE_CODE", "VariableKey1"));
				DEList.ActionButtons.Add("Add");
			}
			protected override void Format(ref System.Collections.Generic.List<DEListDetail> items)
			{
				foreach (DEListDetail item in items)
				{
					var trueString = true.ToString();
					item.EnableRowWiseEdit = true;
					if (item.Columns[2] == trueString)
					{
						item.EnableRowWiseDelete = true;
					}
					item.Columns[2] = (item.Columns[2].ToString() == trueString ? "User" : "System");
					string key = item.Columns[0];
					item.Columns[3] = (item.Columns[3] == true.ToString() ? "Yes" : "No");
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
				//Return "#" 'To be implemented next..currently on delete action nothing happens
				return string.Format("/DefaultList.aspx?listname={0}&businessUnit={1}", DEList.ModuleName, base.settings.BusinessUnit);
			}
		}
	}
}
