namespace SystemDefaultsAPI
{
	namespace UtilityClasses
	{
		public class DTParameters
		{
			public string columnnames { get; set; }
			public string searchtext { get; set; }
			public string searchtype { get; set; }
			public int draw { get; set; }
			public int start { get; set; }
			public int length { get; set; }
			public DTOrder[] order { get; set; }
		}
	}
}
