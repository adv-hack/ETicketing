namespace TalentSystemDefaults
{
	public class KeyColumn
	{
		public string Name { get; set; }
		public string PropertyName { get; set; }
		public KeyColumn(string name, string propertyName)
		{
			this.Name = name;
			this.PropertyName = propertyName;
		}
	}
}
