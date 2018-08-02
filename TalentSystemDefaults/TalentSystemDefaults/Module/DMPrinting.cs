namespace TalentSystemDefaults
{
	namespace TalentModules
	{
		public class DMPrinting : DMBase
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
			static private string _ModuleTitle = "Printing Module";
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
			public const string AL = "WAF#GxYV-Ku5hJXHM-arWvep8-ar#Csa";
			public const string AO = "WAF#GxYV-xu5IoXHp-arWvefu-ar#Cs0";
			public const string DisplayPrintAddressLabelButtonBottom = "BjS6aVxZ-tPa6JIzL-arWvead-ar#C0Q";
			public const string DisplayPrintAddressLabelButtonTop = "BjS6aVxZ-tPa6JIz1-arWveaO-ar#C0w";
			//public const string DisplayPrintAddressLabelOption = "BjS6aVxZ-tPa6JIzG-arWve9r-ar#C0m";
			//public const string PrintAddressLabelItemText = "BjS4aVLZ-WLb#GIeV-arWve9a-ar#C06";
			//public const string PrintAddressLabelHeaderText = "BjS4aVLZ-WLb#Qx4B-arWvea3-ar#C0p";
			//public const string msgCustomerPrintAddressLabelSuccess = "BjS4aVLZ-90KQUL0B-arWvRQu-ar#C0f";
			public const string msgUpdatePrintAddressLabelSuccess = "BjS4aVLZ-90KQUL0B-arWvRQu-ar#C02";
			public const string btnPrintAddressLabel = "BjS4aVLZ-c0KQUvHL-arWveaq-ar#C09";
			public DMPrinting(DESettings settings, bool initialiseData)
				: base(ref settings, initialiseData)
			{
			}
			public override void SetModuleConfiguration()
			{
				MConfigs.Add(AL, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(AO, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(DisplayPrintAddressLabelButtonBottom, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
				MConfigs.Add(DisplayPrintAddressLabelButtonTop, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
				//MConfigs.Add(DisplayPrintAddressLabelOption, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
				//MConfigs.Add(PrintAddressLabelItemText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				//MConfigs.Add(PrintAddressLabelHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				//MConfigs.Add(configID: msgCustomerPrintAddressLabelSuccess, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderLabels, uniqueId: "Customer");
				MConfigs.Add(configID: msgUpdatePrintAddressLabelSuccess, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderLabels, uniqueId: "Update");
				MConfigs.Add(btnPrintAddressLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				Populate();
			}
		}
	}
}
