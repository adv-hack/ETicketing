namespace TalentSystemDefaults
{
	public enum ColumnType
	{
		Text,
		Radio,
		HyperLink
	}
	public class DataColumn
	{
		public string Name { get; set; }
		public string PropertyName { get; set; }
		public string Header { get; set; }
		public ColumnType Type { get; set; }
		public DataColumn(string name, string propertyName, string header, ColumnType type)
		{
			this.Name = name;
			this.PropertyName = propertyName;
			this.Header = header;
			this.Type = type;
		}
	}
}
