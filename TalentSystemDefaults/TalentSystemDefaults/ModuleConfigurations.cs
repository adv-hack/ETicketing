namespace TalentSystemDefaults
{
	public class ModuleConfigurations : System.Collections.Generic.List<ModuleConfiguration>
	{
		public void Add(string configID, DataType fieldType, string displayTab, ValidationGroup validationGroup = null, string accordionGroup = null, string uniqueId = "")
		{
			var mConfig = new ModuleConfiguration();
			mConfig.ConfigID = configID;
			mConfig.FieldType = fieldType;
			mConfig.TabHeader = displayTab;
			mConfig.ValidationGroup = validationGroup;
			mConfig.AccordionGroup = accordionGroup;
			mConfig.ConfigurationItem = null;
			mConfig.UniqueId = uniqueId;
			base.Add(mConfig);
		}
	}
}
