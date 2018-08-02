using System.Collections.Generic;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	public class DEList
	{
		public DEList()
		{
			// VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
			FilterFields = new List<string>();
			FilterButtons = new List<string>();
			DataColumns = new List<DataColumn>();
			VariableKeyColumns = new List<KeyColumn>();
			ActionButtons = new List<string>();
		}
		public string TableName { get; set; }
		public string ModuleObjectName { get; set; }
		public string FunctionName { get; set; }
		public string Title { get; set; }
		public List<string> FilterFields { get; set; }
		public List<string> FilterButtons { get; set; }
		public List<DataColumn> DataColumns { get; set; }
		public List<KeyColumn> VariableKeyColumns { get; set; }
		public List<string> ActionButtons { get; set; }
		public List<DEListDetail> Data { get; set; }
		public string ModuleName { get; set; }
		public bool EnableBUSelector { get; set; }
		public string[] BusinessUnits { get; set; }
		public bool EnableSelectColumn { get; set; }
		public bool EnableSelectAsHyperLink { get; set; }
		public bool EnableEditColumn { get; set; }
		public bool EnableDeleteColumn { get; set; }
		public string AddURL { get; set; }
		public string DeleteURL { get; set; }
		public string UpdateURL { get; set; }
		public string StatusMessage { get; set; }
        public bool HasError { get; set; }
	}
}
