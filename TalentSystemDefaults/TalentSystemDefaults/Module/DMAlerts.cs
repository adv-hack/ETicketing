using System.Collections.Generic;
namespace TalentSystemDefaults
{
	namespace TalentModules
	{
		public class DMAlerts : DMBase
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
			static private string _ModuleTitle = "Alerts Module";
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
			public const string AlertsPerPage = "BjS6aVxZ-Fwv2tcoE-arWvef3-ar#VE9";
			public const string SingleAlertText = "BjS4aVLZ-tuElUVp#-arWvR6r-ar#VEa";
			public const string MultipleAlertText = "BjS4aVLZ-57xQUH4G-arWvRp3-ar#VE0";
			public const string AttributeRFVErrMessage = "BjS4aVLZ-cDkbQXfK-arWveaa-ar#VAQ";
			public const string CriteriasTitle = "BjS4aVLZ-Wcj6OCQK-arWve2r-ar#VAw";
			public const string ExistsAttMissingErrMess = "BjS4aVLZ-4uj#7C0a-arWvRph-ar#VAm";
			public const string RowAddButtonText = "BjS4aVLZ-BpprzzSB-arWve2d-ar#VA6";
			public const string InvalidAttributeText = "BjS4aVLZ-CLeEdGLL-arWve98-ar#VAp";
			public const string LabelAction = "BjS4aVLZ-rEah6YK4-arWvefq-ar#VAf";
			public const string LabelActionDetail = "BjS4aVLZ-rEa1aXNP-arWve2y-ar#VA2";
			public const string LabelActivationStartDate = "BjS4aVLZ-rEv6dgs4-arWveaO-ar#VA9";
			public const string LabelActivationEndDate = "BjS4aVLZ-rEv6QYK4-arWvea3-ar#VAa";
			public const string LabelDescription = "BjS4aVLZ-rOjzGILL-arWve2q-ar#VA0";
			public const string LabelEnabled = "BjS4aVLZ-r0xZQIs9-arWvefd-ar#V1Q";
			public const string LabelImagePath = "BjS4aVLZ-rl3euJTB-arWvefa-ar#V1w";
			public const string LabelName = "BjS4aVLZ-rWv0QVxM-arWvepr-ar#V1m";
			public const string NameRFVErrMess = "BjS4aVLZ-r9l4VxcB-arWve9q-ar#V16";
			public const string DescriptionRFVErrMess = "BjS4aVLZ-Q5atnILZ-arWveah-ar#V1p";
			public const string StartDateRFVErrMess = "BjS4aVLZ-cWkb9xEK-arWve98-ar#V1f";
			public const string EndDateRFVErrMess = "BjS4aVLZ-COgQ2g4G-arWve9d-ar#V12";
			public const string ActionDetailRFVErrMess = "BjS4aVLZ-jXjkGXNZ-arWvead-ar#V19";
			public const string ActionDetailCVErrMess = "BjS4aVLZ-jXjknLoL-arWvRwq-ar#V1a";
			public const string DescriptionCVErrMess = "BjS4aVLZ-Q5aWnCgL-arWvRQq-ar#V10";
			public const string NewAlertButtonText = "BjS4aVLZ-QvE6GVLy-arWve2h-ar#VZQ";
			public const string SubjectCVErrMess = "BjS4aVLZ-5jeQLgkB-arWve0u-ar#VZw";
			public const string LabelSubject = "BjS4aVLZ-rD66aLsB-arWvefu-ar#VZm";
			public const string StartDateREVErrMess = "BjS4aVLZ-cWgb9xEK-arWvRwh-ar#VZ6";
			public const string EndDateREVErrMess = "BjS4aVLZ-COgQ2g4G-arWvRwq-ar#VZp";
			public const string lblActionDetailURLOption = "BjS4aVLZ-#cx6XDKL-arWve9r-ar#VZf";
			public const string lblActionDetailFFOption = "BjS4aVLZ-#cx6dvLQ-arWvead-ar#VZ2";
			public const string EndDateCVErrMess = "BjS4aVLZ-COeQfIkB-arWve0d-ar#VZ9";
			public const string BrowseButton = "BjS4aVLZ-WpprHD0K-arWvefd-ar#VZa";
			public const string RedirectMessageText = "BjS4aVLZ-QEb#cdoK-arWvRwO-ar#VZ0";
			public const string RestrictiontMessageText = "BjS4aVLZ-QEa6FggK-arWvRwu-ar#VsQ";
			public const string ALERTS_CC_EXPIRY_PPS_WARN_PERIOD = "jlefG3WV-KHgy9XEz-arWve2h-ar#Vsw";
			public const string ALERTS_CC_EXPIRY_SAV_WARN_PERIOD = "jlefG3WV-KHgy#XEz-arWve2h-ar#Vsm";
			public const string ALERTS_CC_EXPIRY_WARN_PERIOD = "jlefG3WV-KHgyccHI-arWve2q-ar#Vs6";
			public const string ALERTS_ENABLED = "jlefG3WV-KHuh9tH6-arWvefO-ar#Vsp";
			public const string ALERTS_GENERATE_AT_LOGIN = "jlefG3WV-KHgtJ5SD-arWve2O-ar#Vsf";
			public const string ALERTS_REFRESH_ATTRIBUTES_AT_LOGIN = "jlefG3WV-KHiLu7Ph-arWve9O-ar#Vs2";
			public const string ALERT_CC_EXPIRY_PPS_WARN_PERIOD_AFETRWARDS = "jlefG3WV-KafiyjD6-arWve9h-ar#Vs9";
			public const string ALERT_CC_EXPIRY_SAV_WARN_PERIOD_AFETRWARDS = "jlefG3WV-KafiyjD6-arWve9h-ar#Vsa";
			public DMAlerts(DESettings settings, bool initialiseData)
				: base(ref settings, initialiseData)
			{
			}
			public override void SetModuleConfiguration()
			{
				//General Tab
				MConfigs.Add(ALERTS_ENABLED, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(ALERTS_GENERATE_AT_LOGIN, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(ALERTS_REFRESH_ATTRIBUTES_AT_LOGIN, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(AlertsPerPage, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> {VG.Numeric }));
				//Credit Card Tab
				MConfigs.Add(ALERTS_CC_EXPIRY_WARN_PERIOD, DataType.TEXT, DisplayTabs.TabHeaderCreditCard, FieldValidation.Add(new List<VG> {VG.Numeric}));
                MConfigs.Add(ALERTS_CC_EXPIRY_PPS_WARN_PERIOD, DataType.TEXT, DisplayTabs.TabHeaderCreditCard, FieldValidation.Add(new List<VG>{VG.Numeric}));
				MConfigs.Add(ALERT_CC_EXPIRY_PPS_WARN_PERIOD_AFETRWARDS, DataType.TEXT, DisplayTabs.TabHeaderCreditCard, FieldValidation.Add(new List<VG> {VG.Numeric}));
				MConfigs.Add(ALERTS_CC_EXPIRY_SAV_WARN_PERIOD, DataType.TEXT, DisplayTabs.TabHeaderCreditCard, FieldValidation.Add(new List<VG> {VG.Numeric}));
				MConfigs.Add(ALERT_CC_EXPIRY_SAV_WARN_PERIOD_AFETRWARDS, DataType.TEXT, DisplayTabs.TabHeaderCreditCard, FieldValidation.Add(new List<VG> {VG.Numeric}));
				//Text Tab
				MConfigs.Add(SingleAlertText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(MultipleAlertText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
				//Error Tab
				MConfigs.Add(AttributeRFVErrMessage, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(ExistsAttMissingErrMess, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(NameRFVErrMess, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(DescriptionRFVErrMess, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(ActionDetailRFVErrMess, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(ActionDetailCVErrMess, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(DescriptionCVErrMess, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(SubjectCVErrMess, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(StartDateREVErrMess, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(StartDateRFVErrMess, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(EndDateRFVErrMess, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(EndDateREVErrMess, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(EndDateCVErrMess, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(RedirectMessageText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(RestrictiontMessageText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(InvalidAttributeText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				//Label Tab
				MConfigs.Add(CriteriasTitle, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(LabelEnabled, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(LabelName, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(LabelSubject, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(LabelDescription, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(LabelAction, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(LabelActionDetail, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(LabelActivationStartDate, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(LabelActivationEndDate, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(LabelImagePath, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(lblActionDetailURLOption, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				MConfigs.Add(lblActionDetailFFOption, DataType.TEXT, DisplayTabs.TabHeaderLabels);
				//Button Tab
				MConfigs.Add(BrowseButton, DataType.TEXT, DisplayTabs.TabHeaderButtons);
				MConfigs.Add(RowAddButtonText, DataType.TEXT, DisplayTabs.TabHeaderButtons);
				MConfigs.Add(NewAlertButtonText, DataType.TEXT, DisplayTabs.TabHeaderButtons);
				Populate();
			}
		}
	}
}
