namespace TalentSystemDefaults
{
	public enum ModuleType
	{
		@Module,
		List
	}
	public class ModuleEntity
	{
		public string Name { get; set; }
		public string Title { get; set; }
		public ModuleType Type { get; set; }
		public ModuleEntity(string name, string title, ModuleType type)
		{
			this.Name = name;
			this.Title = title;
			this.Type = type;
		}
	}
}
