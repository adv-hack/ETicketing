namespace SystemDefaultsAPI
{
	namespace Models
	{
		public class TabContentModel : System.Collections.Generic.List<BaseFieldModel>
		{
			public string TabHeader { get; set; }
			public bool Active { get; set; }
			public bool HasErrors
			{
				get
				{
					foreach (BaseFieldModel field in this)
					{
						if (field.mConfig.HasError)
						{
							return true;
						}
					}
					return false;
				}
			}
			public string ID
			{
				get
				{
					return "tab" + TabHeader.Replace(" ", string.Empty);
				}
			}
			public TabContentModel(string tabHeader, bool active = false)
			{
				this.TabHeader = tabHeader;
				this.Active = active;
			}
		}
	}
}
