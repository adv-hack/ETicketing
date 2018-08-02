using System.Collections.Generic;
namespace SystemDefaultsAPI
{
	namespace UtilityClasses
	{
		public class JQueryDataTable
		{
			public int draw { get; set; }
			public int recordsTotal { get; set; }
			public int recordsFiltered { get; set; }
			public List<object> data { get; set; }
		}
	}
}
