namespace TalentSystemDefaults
{
	namespace TalentModules
	{
		public class DMAuthorities : DMBase
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
			static private string _ModuleTitle = "Authorities";
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
			public const string ltlGroupsAndPermissions = "rOxfXOxB-cDaZ6Cza-arWvear-ar#C10";
			public const string ltlAgentPreferences = "rOxfXOxB-c0xRUX4K-arWve9a-ar#CZQ";
			public const string lblAgent = "rOxfXOxB-#0SDzvYB-arWvefh-ar#CZw";
			public const string lblCurrentGroup = "rOxfXOxB-#vBQHIf9-arWve2d-ar#CZm";
			public const string lblCategory = "rOxfXOxB-#ORk5Vov-arWve2q-ar#CZ6";
			public const string ltlAddNewGroup = "rOxfXOxB-cReu2Es1-arWve9O-ar#CZp";
			public const string lblNewGroupName = "rOxfXOxB-#oD3GxU9-arWve9q-ar#CZf";
			public const string lblBasedOn = "rOxfXOxB-#OS0WVAa-arWve28-ar#CZ2";
			public const string btnSaveNewGroup = "rOxfXOxB-cOBQQtfv-arWve9q-ar#CZ9";
			public const string lblLoadingText = "rOxfXOxB-#Lq4QvsM-arWvRmh-ar#CZa";
			public const string errGroupNameMandatory = "rOxfXOxB-WDF4Ox1D-arWve0q-ar#CZ0";
			public const string errGroupNameNotSelected = "rOxfXOxB-WDF4ghzY-arWve0y-ar#CsQ";
			public const string lblPleaseSelectGroup = "rOxfXOxB-#WxkXVoB-arWvea8-ar#Csw";
			public const string lblPleaseSelectCategory = "rOxfXOxB-#Wxkhioc-arWve0d-ar#Csm";
			public const string selectAgentText = "rOxfXOxB-QuElgJpa-arWvead-ar#Cs6";
			public const string lblAuthorityGroup = "BjS4aVLZ-#YEwUgtc-arWve2u-ar#Csp";
			public const string PleaseSelectAnAuthorityGroupError = "BjS4aVLZ-FU66Oi21-arWvRQ8-ar#Csf";
			public const string SelectAuthorityGroup = "BjS4aVLZ-QupQXVLK-arWve08-ar#Cs2";
			public DMAuthorities(DESettings settings, bool initialiseData)
				: base(ref settings, initialiseData)
			{
			}
			public override void SetModuleConfiguration()
			{
				MConfigs.Add(ltlGroupsAndPermissions, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(ltlAgentPreferences, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(lblAgent, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(lblCurrentGroup, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(lblCategory, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(ltlAddNewGroup, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(lblNewGroupName, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(lblBasedOn, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(btnSaveNewGroup, DataType.TEXT, DisplayTabs.TabHeaderButtons);
				MConfigs.Add(lblLoadingText, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(errGroupNameMandatory, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(errGroupNameNotSelected, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(lblPleaseSelectGroup, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(lblPleaseSelectCategory, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(selectAgentText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(lblAuthorityGroup, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(PleaseSelectAnAuthorityGroupError, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(SelectAuthorityGroup, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				Populate();
			}
		}
	}
}
