using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace TalentSystemDefaults
{
	namespace DataEntities
	{
		public class DEListDetail
		{
			const string JS_NULL = "null";
			private bool _EnableRowWiseEdit = false;
			public bool EnableRowWiseEdit
			{
				get
				{
					return _EnableRowWiseEdit;
				}
				set
				{
					_EnableRowWiseEdit = value;
				}
			}
			private bool _EnableRowWiseDelete = false;
			public bool EnableRowWiseDelete
			{
				get
				{
					return _EnableRowWiseDelete;
				}
				set
				{
					_EnableRowWiseDelete = value;
				}
			}
			public string SelectURL { get; set; }
			public string[] Columns { get; set; }
			public string[] VariableKeys { get; set; }
			public DEListDetail(string[] values, string[] variableKeyValues)
			{
				VariableKeys = new string[4] { JS_NULL, JS_NULL, JS_NULL, JS_NULL };
				Initialise(values, variableKeyValues);
			}
			private void Initialise(string[] values, string[] variableKeyValues)
			{
				IEnumerable<PropertyInfo> properties = this.GetType().GetProperties();
				Columns = new string[values.Count() - 1 + 1];
				int i = 0;
				foreach (var value in values)
				{
					Columns[i] = value;
					i++;
				}
				i = 0;
				foreach (var value in variableKeyValues)
				{
					VariableKeys[i] = value;
					i++;
				}
			}
		}
	}
}
