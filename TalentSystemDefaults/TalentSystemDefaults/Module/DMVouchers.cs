namespace TalentSystemDefaults
{
	namespace TalentModules
	{
		public class DMVouchers : DMBase
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
			static private string _ModuleTitle = "Vouchers Module";
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
			public const string btnAddVoucher = "rOxfXOxB-c9T4#Le0-arWvep8-arcH1a";
			public const string btnAddExternalvouchers = "rOxfXOxB-cderGr2K-arWvefh-arcH10";
			public const string btnBook = "rOxfXOxB-cVuu5gWH-arWvep3-arcHZQ";
			public const string btnDelete = "rOxfXOxB-cOErdILv-arWveph-arcHZw";
			public const string btnExchange = "rOxfXOxB-cYx0WrlK-arWvefO-arcHZm";
			public const string Company = "rOxfXOxB-BPDeWDQf-arWveph-arcHZ6";
			public const string ConfirmVoucherEntryText = "rOxfXOxB-Bl6MFg4K-arWvRQ3-arcHZp";
			public const string EnterVoucherCode = "rOxfXOxB-CAxQGDQX-arWve2h-arcHZf";
			public const string ExternalVouchersLabel = "rOxfXOxB-4WzdGAsB-arWve2a-arcHZ2";
			public const string InvalidVoucherText = "rOxfXOxB-CL6MWVKy-arWvead-arcHZ9";
			public const string lblExchange = "rOxfXOxB-#YxkWVlv-arWvefy-arcHZa";
			public const string lblExternalVoucherCode = "rOxfXOxB-#OSb5Vov-arWve2h-arcHZ0";
			public const string Price = "rOxfXOxB-Wv3QGdKE-arWvepq-arcHsQ";
			public const string RecentlyAdded = "rOxfXOxB-Q7K4UhK9-arWvefr-arcHsw";
			public const string UpgradeText = "rOxfXOxB-uOE8G#of-arWvMQh-arcHsm";
			public const string VoucherAddErrorText = "rOxfXOxB-BvgQ#LJ1-arWvRpO-arcHs6";
			public const string VoucherDeleteErrorText = "rOxfXOxB-Bvx6Gn4X-arWvRQd-arcHsp";
			public const string VoucherConvertErrorText = "rOxfXOxB-BvO4FgoK-arWvRQy-arcHsf";
			public const string VoucherCodeLengthErrorText = "rOxfXOxB-BvxhO7fG-arWvRQa-arcHs2";
			public const string VoucherAlreadyUsedErrorText = "rOxfXOxB-BvxeOxzh-arWvRmO-arcHs9";
			public const string VoucherInvalidErrorText = "rOxfXOxB-BvvkFgoK-arWvRQr-arcHsa";
			public const string VoucherHeading = "rOxfXOxB-BvKEGICZ-arWvefd-arcHs0";
			public const string VoucherScheme = "rOxfXOxB-Bvx7OqfG-arWvefr-arcH0Q";
			public const string VouchersLabel = "rOxfXOxB-BvC4OOfG-arWvefq-arcH0w";
			public const string YourAccountBalance = "rOxfXOxB-BEE55XLQ-arWve2a-arcH0m";
			public const string YourExperience = "rOxfXOxB-B5xrBx7G-arWve23-arcH06";
			public const string NoVoucherFound = "rOxfXOxB-BYpXgxcX-arWve9a-arcH0p";
			public const string ClearCacheDependencyOnAllServersFileCreateError = "rOxfXOxB-FWH43XIX-arWvR6y-arcH0f";
			public const string ClearCacheDependencyOnAllServersError = "rOxfXOxB-FWH43XIF-arWvRmq-arcH02";
			public const string AddOrUpdateVoucherError = "rOxfXOxB-g5xbOxnG-arWvRw3-arcH09";
			public const string InsertOrUpdateVoucherDetailsError = "rOxfXOxB-C4KeGI2K-arWvRma-arcH0a";
			public const string RemoveLinkError = "rOxfXOxB-QZgQ4dz6-arWve0d-arcH00";
			public const string ShowDeleteButton = "rOEEXOfK-J7uXjVLK-arWvefq-arcHCQ";
			public const string ShowExternalVoucherCodeTextBox = "rOEEXOfK-JjvkvIzZ-arWve2y-arcHCw";
			public DMVouchers(DESettings settings, bool initialiseData)
				: base(ref settings, initialiseData)
			{
			}
			public override void SetModuleConfiguration()
			{
				MConfigs.Add(btnAddVoucher, DataType.TEXT, DisplayTabs.TabHeaderButtons);
				MConfigs.Add(btnAddExternalvouchers, DataType.TEXT, DisplayTabs.TabHeaderButtons);
				MConfigs.Add(btnBook, DataType.TEXT, DisplayTabs.TabHeaderButtons);
				MConfigs.Add(btnDelete, DataType.TEXT, DisplayTabs.TabHeaderButtons);
				MConfigs.Add(btnExchange, DataType.TEXT, DisplayTabs.TabHeaderButtons);
				MConfigs.Add(Company, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(ConfirmVoucherEntryText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(EnterVoucherCode, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(ExternalVouchersLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(InvalidVoucherText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(lblExchange, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(lblExternalVoucherCode, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(Price, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(RecentlyAdded, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(UpgradeText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(VoucherAddErrorText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(VoucherDeleteErrorText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(VoucherConvertErrorText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(VoucherCodeLengthErrorText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(VoucherAlreadyUsedErrorText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(VoucherInvalidErrorText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(VoucherHeading, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(VoucherScheme, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(VouchersLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(YourAccountBalance, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(YourExperience, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(NoVoucherFound, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(ClearCacheDependencyOnAllServersFileCreateError, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(ClearCacheDependencyOnAllServersError, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(AddOrUpdateVoucherError, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(InsertOrUpdateVoucherDetailsError, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(RemoveLinkError, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(ShowDeleteButton, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
				MConfigs.Add(ShowExternalVoucherCodeTextBox, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
				Populate();
			}
		}
	}
}
