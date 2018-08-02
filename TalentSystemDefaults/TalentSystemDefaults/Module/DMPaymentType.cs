using System.Collections.Generic;
using TalentSystemDefaults.DataAccess.DataObjects;

namespace TalentSystemDefaults
{
	namespace TalentModules
	{
		public class DMPaymentType : DMBase
		{
			static private bool _EnableAsModule = false;

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
			static private string _ModuleTitle = "Payment Type";
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
			public const string DirectDebitPaymentMethodLabel = "rOxfXOxB-tXEiGDsQ-arWve9q-arWvR9";
			public const string EPurseTitle = "rOxfXOxB-RmxLdZSE-arWvefh-arWvAw";
			public const string AutoEnrolPPSCheckboxLabel = "rOxfXOxB-5v3sBrzM-arWvRQr-arWvZf";
			public const string ChipAndPinPaymentMethodLabel = "rOxfXOxB-JL3edhQU-arWve93-arWv0a";
			public const string CCTypeButtonText = "rOxfXOxB-0ppr3zSB-arWvefy-arWPA9";
			public const string TicketingCheckoutLabelCC = "rOxfXOxB-tcT4aeo9-arWvZw8-arWP19";
			public const string TicketingCheckoutLabelCQ = "rOxfXOxB-tcT4aeo9-arWvEwh-arWP1a";
			public const string PayPalReferenceText = "rOxfXOxB-r3e4tcoB-arWvR2h-arWP10";
			public const string TicketingCheckoutLabelCF = "rOxfXOxB-tcT4aeo9-arWvEwh-arWPZQ";
			public const string TicketingCheckoutLabelEP = "rOxfXOxB-tcT4aZo9-arWvEwh-arWPZw";
			public const string CCTypeMessasge = "rOxfXOxB-0qv#GCQc-arWve2q-arWPAa";
			public const string CCTypeSubTitle = "rOxfXOxB-0Uj6GrQc-arWve2u-arWPA0";
			public const string CCTypeTitle = "rOxfXOxB-0mxWdeSO-arWve2y-arWP1Q";
			public const string TicketingCheckoutLabelCS = "rOxfXOxB-tcT4aeo9-arWvEwh-arWP1f";
			public const string NAVIGATE_URL = "rlEw3kWf-pmihKj5C-arWve28-arWP0Q";
			public const string CALL_CREDIT_LIMIT_CHECK = "rlEw3kWf-p3qfPTQm-arWvepr-arWP0w";
			public const string TicketingCheckoutLabelDD = "rOxfXOxB-tcT4ayo9-arWvEwh-arWP12";
			public const string CC = "rOxfXOxB-0arWveQO-arWve23-arWPZm";
			public const string OA = "rOxfXOxB-p4GDzBA1-arWve2O-arWPZ9";
			public const string TicketingCheckoutLabelPD = "rOxfXOxB-tcT4aco9-arWvEwh-arWPsm";
			public const string PAYMENT_TYPE_ID = "rlEw3kWf-pm3LKNDE-arWve6y-arWPs2";
			public const string PAYMENT_TYPE_CODE = "rlEw3kWf-pm3Llt2h-arWve68-arWPs9";
			public const string PAYMENT_TYPE_DESCRIPTION = "rlEw3kWf-pm3LtB2z-arWvepu-arWPsa";
			public const string SEQUENCE = "rlEw3kWf-6a48be2j-arWvemr-arWPs0";
			public const string IS_OTHER_TYPE = "rlEw3kWf-kdmiK7Ph-arWve68-arWP0m";
			public const string PAYMENT_TYPE_BU_ID = "rlEwyH1B-pm3LzZjS-arWvep3-arWP06";
			public const string QUALIFIER = "rlEwyH1B-VB9Db5nA-arWve6h-arWP0p";
			public const string BUSINESS_UNIT = "rlEwyH1B-VUwp9#b#-arWve6h-arWP0f";
			public const string PARTNER = "rlEwyH1B-p3q3KXjk-arWve6u-arWP02";
			public const string DEFAULT_PAYMENT_TYPE = "rlEwyH1B-6mmFKsjM-arWvepr-arWP0a";
			public const string RETAIL_TYPE = "rlEwyH1B-6HgtljPh-arWve6a-arWP00";
			public const string TICKETING_TYPE = "rlEwyH1B-oBqqFuSm-arWvepq-arWPCQ";
			public const string AGENT_RETAIL_TYPE = "rlEwyH1B-I3AhKNTM-arWvepy-arWPCw";
			public const string AGENT_TICKETING_TYPE = "rlEwyH1B-ImglKZQR-arWvepr-arWPCm";
			public const string AGENT_TICKETING_TYPE_REFUND = "rlEwyH1B-ImglKZ5E-arWvefy-arWPCf";
			public const string TICKETING_TYPE_REFUND = "rlEwyH1B-oBqqi5cm-arWvepa-arWPC2";
			public const string GENERIC_SALES = "rlEwyH1B-6a2LvXcO-arWvep3-arWPC9";
			public const string GENERIC_SALES_REFUND = "rlEwyH1B-6a2Lftbz-arWvepr-arWPCa";
			public const string PPS_TYPE = "rlEwyH1B-RwVfycSS-arWve6h-arWPC0";
			public const string IS_GIFT_CARD = "rlEwyH1B-kmi1yjD1-arWvepO-arWTeQ";
			public const string GoogleCheckoutMessage = "rOxfXOxB-BasuhDlT-arWvM08-arWPRw";
			public const string ChipAndPinDevicesLabel = "rOxfXOxB-JLH4aeJF-arWveah-arWv00";
			public const string ZeroPricedBasketLegend = "rOxfXOxB-QcueGMgB-arWvefd-arWvCQ";
			public const string ZeroPricedBasketMessage = "rOxfXOxB-QcuenI4B-arWvefu-arWvCw";
			public const string ZeroPricedBasketPaymentMethodLabel = "rOxfXOxB-QcueSgJ4-arWve9h-arWvCm";
			public const string ZeroPricedBasketSubTitle = "rOxfXOxB-QcueFVjK-arWve98-arWvC6";
			public const string ZeroPricedBasketTitle = "rOxfXOxB-QcueUIKX-arWve2d-arWvCp";
			public const string OnAccountPaymentMethodLabel = "rOxfXOxB-CDvwmYeK-arWve2r-arWvCf";
			public const string OnAccountMessage = "rOxfXOxB-CDx#g104-arWvep8-arWvC2";
			public const string OnAccountLegend = "rOxfXOxB-CDx2gvep-arWveph-arWvC9";
			public const string OnAccountSubTitle = "rOxfXOxB-CDz0zDWv-arWve9d-arWvCa";
			public const string OnAccountTitle = "rOxfXOxB-CDj65gAZ-arWvefy-arWvC0";
			public const string PointOfSaleTerminalsLabel = "rOxfXOxB-BNxlnreM-arWveaa-arWPeQ";
			public const string PointOfSalePaymentMethodLabel = "rOxfXOxB-BNxiGDsQ-arWve9d-arWPew";
			public const string PointOfSaleMessage = "rOxfXOxB-BNxF5go9-arWvepa-arWPem";
			public const string PointOfSaleLegend = "rOxfXOxB-BNxh6BTM-arWvepr-arWPe6";
			public const string PointOfSaleSubTitle = "rOxfXOxB-BNxstvs#-arWve0y-arWPep";
			public const string PointOfSaleTitle = "rOxfXOxB-BNxlW3gK-arWve2O-arWPef";
			public const string PDQPaymentMethodLabel = "rOxfXOxB-zll4GyoE-arWve9O-arWPe2";
			public const string PDQTitle = "rOxfXOxB-z74lFVgK-arWve2y-arWPe9";
			public const string PDQSubTitle = "rOxfXOxB-zmxidcSP-arWve23-arWPea";
			public const string PDQLegend = "rOxfXOxB-zOHGfINE-arWvepO-arWPe0";
			public const string PDQMessage = "rOxfXOxB-z831GoYa-arWvep3-arWPRQ";
			public const string GoogleCheckoutLegend = "rOxfXOxB-Basu2DoB-arWvef3-arWPRm";
			public const string GoogleCheckoutSubTitle = "rOxfXOxB-BasuduQX-arWveay-arWPR6";
			public const string MultiplePPSPaymentOptionsMessage2 = "rOxfXOxB-57ViXD04-arWvM6r-arWv0m";
			public const string GoogleCheckoutTitle = "rOxfXOxB-BasupJKF-arWve2d-arWPRp";
			public const string CCExternalSubTitle = "rOxfXOxB-0vVXvgb9-arWvRQq-arWPAf";
			public const string GoogleCheckoutPaymentMethodLabel = "rOxfXOxB-BasuWIxK-arWve9h-arWPRf";
			public const string OtherPaymentMethodLabel = "rOxfXOxB-cWa6QVj9-arWve2O-arWPMw";
			public const string OthersTitle = "rOxfXOxB-cmxIdBSK-arWvepr-arWPMm";
			public const string OthersSubTitle = "rOxfXOxB-cUj6nrAB-arWve2d-arWPM6";
			public const string OthersMessage = "rOxfXOxB-cqv27ClU-arWvefu-arWPMp";
			public const string OthersLegend = "rOxfXOxB-cZaZGv4B-arWvefq-arWPMf";
			public const string SelectOthersType = "rOxfXOxB-Q4e#GBSc-arWvefr-arWPM2";
			public const string OthersTextValue = "rOxfXOxB-cm8eOIfp-arWve2u-arWPM9";
			public const string PO = "rOxfXOxB-xw3Iocjp-arWvepy-arWPMa";
			public const string LN = "rOxfXOxB-PZ23iYTj-arWve68-arWPM0";
			public const string LN_Mandatory = "rOxfXOxB-PLewQxxX-arWvep8-arWPEQ";
			public const string PO_Mandatory = "rOxfXOxB-xLewQxxX-arWvep8-arWPEw";
			public const string OthersPaymentTypeMandatoryErrorMessage = "rOxfXOxB-cwxrWg44-arWvRwd-arWPEm";
			public const string AutoEnrolPPSTitle = "rOxfXOxB-5v3sdvb#-arWvea3-arWPEp";
			public const string OthersH2 = "rOxfXOxB-cfT4GR4a-arWvefq-arWPE2";
			public const string CCExternalButtonText = "rOxfXOxB-0vuXdZxB-arWvef8-arWPAp";
			public const string CCExternalTitle = "rOxfXOxB-0vqEdvsO-arWve2q-arWPA2";
			public const string CashbackPaymentMethodLabel = "rOxfXOxB-rER75O0X-arWve2y-arWvRa";
			public const string EPursePaymentMethodLabel = "rOxfXOxB-RwxrJI0v-arWve2q-arWvR0";
			public const string CreditCardTitle = "rOxfXOxB-WaqE6OsO-arWve93-arWvMw";
			public const string CreditCardSubTitle = "rOxfXOxB-WaVXOdbY-arWve9q-arWvMm";
			public const string PayPalTitle = "rOxfXOxB-rmxidcS4-arWvefh-arWvM0";
			public const string PayPalSubTitle = "rOxfXOxB-rUj6UrjE-arWve9O-arcv0f";
			public const string PayPalLegend = "rOxfXOxB-rZaZGvxB-arWvepd-arWvEw";
			public const string PayPalMessage = "rOxfXOxB-rqv27CtU-arWvR9d-arWvEm";
			public const string DirectDebitTitle = "rOxfXOxB-tXElGygK-arWve28-arWvE6";
			public const string DirectDebitSubTitle = "rOxfXOxB-tXEsfIgB-arWve9h-arWvEp";
			public const string DirectDebitLegend = "rOxfXOxB-tXEhOgTM-arWvepr-arWvEf";
			public const string DirectDebitMessage = "rOxfXOxB-tXEF6LLL-arWve0a-arWvE2";
			public const string CashbackTitle = "rOxfXOxB-rEEkgd0Z-arWve2O-arWvE9";
			public const string CashbackSubTitle = "rOxfXOxB-rEClmLgK-arWve9y-arWvEa";
			public const string CashbackLegend = "rOxfXOxB-rEZ4QYQF-arWvepy-arWvE0";
			public const string CashbackMessage = "rOxfXOxB-rEb#abYO-arWve0O-arWvAQ";
			public const string EPurseSubTitle = "rOxfXOxB-RUj6Gr2G-arWve93-arWvAm";
			public const string CreditCardLegend = "rOxfXOxB-Wa242eYB-arWvef3-arWvA6";
			public const string CreditCardMessage = "rOxfXOxB-Wal4GgoB-arWvea3-arWvAp";
			public const string EPurseLegend = "rOxfXOxB-RZaZGv0B-arWvepd-arcv02";
			public const string EPurseMessage = "rOxfXOxB-Rqv27CfU-arWvea8-arWvA2";
			public const string PPSPaymentOptionsMessage = "rOxfXOxB-RlGznJx4-arWvMfd-arWvA9";
			public const string MultiplePPSPaymentOptionsMessage1 = "rOxfXOxB-57ViXD04-arWvRwq-arWvAa";
			public const string ConfirmDDMandateButtonText = "rOxfXOxB-Blvrd7eK-arWve2u-arWv1Q";
			public const string CancelDDMandateButtonText = "rOxfXOxB-rXaZ5IKQ-arWve2q-arWv1w";
			public const string CreditFinanceTitle = "rOxfXOxB-WSadOde4-arWve2d-arWv16";
			public const string CreditFinanceSubTitle = "rOxfXOxB-WSadUxgY-arWve9r-arWv1p";
			public const string CreditFinanceMessage = "rOxfXOxB-WSadGIeK-arWvRQ3-arWv1f";
			public const string CreditFinancePaymentMethodLabel = "rOxfXOxB-WSaddgW4-arWve9h-arWv12";
			public const string CashTitle = "rOxfXOxB-rjv#Q7xO-arWvepu-arWv19";
			public const string CashSubTitle = "rOxfXOxB-rnS4FVbK-arWvefh-arWv1a";
			public const string CashLegend = "rOxfXOxB-rKre2CeB-arWvep3-arWv10";
			public const string CashMessage = "rOxfXOxB-r8xWQe04-arWvepq-arWvZQ";
			public const string CashPaymentMethodLabel = "rOxfXOxB-rPEFaet4-arWvef8-arWvZw";
			public const string CreditFinanceLegend = "rOxfXOxB-WSadvhxL-arWvefO-arWvZm";
			public const string ConfirmCFMandateButtonText = "rOxfXOxB-Blvrd7eK-arWve2u-arWvZ6";
			public const string CancelCFMandateButtonText = "rOxfXOxB-raaZ5IKQ-arWve2q-arWvZp";
			public const string AutoEnrolPPSLegend = "rOxfXOxB-5v3sHZjE-arWvepa-arWvZ2";
			public const string CreditCardNoPayment = "rOxfXOxB-WawuvhJ4-arWvRph-arWvsQ";
			public const string SuccessfullyAppliedEPurse = "rOxfXOxB-58SwKxKE-arWvE6y-arWvsa";
			public const string ChipAndPinTitle = "rOxfXOxB-JLqEzcsO-arWvefr-arWv0p";
			public const string ChipAndPinSubTitle = "rOxfXOxB-JLVXmXbQ-arWvear-arWv0f";
			public const string ChipAndPinLegend = "rOxfXOxB-JL24XhYB-arWvep8-arWv02";
			public const string ChipAndPinMessage = "rOxfXOxB-JLl46voB-arWvepr-arWv09";
			public const string AutoEnrolPPSCheckboxDefault = "rOEEXOfK-5v3sB3fX-arWve2q-arWvew";
			public const string ChipAndPinTimeoutValue = "rOEEXOfK-JLqEUeJF-arWvefh-arWve6";
			public const string PointOfSaleTimeoutValue = "rOEEXOfK-BNxlQIAB-arWvef8-arcvs0";
			public const string CreditCardPaymentMethodLabel = "rOxfXOxB-Wa3edhQU-arWve9h-arWvRp";
			public const string PayPalPaymentMethodLabel = "rOxfXOxB-rwxrJIxv-arWve23-arWvR2";

			public const string USER51 = "mzlv1QOP-ksXP9fy6-arWvewa-arWBZ9";
			public const string PGMD51 = "mzlv1QOP-IwXPpfyP-arWvewa-arWBZa";
			public const string UPDT51 = "mzlv1QOP-RsXPtfyk-arWvewa-arWBZ0";
			public const string ACTR51 = "mzlv1QOP-0uXPvfy6-arWvemO-arWBsQ";
			public const string CONO51 = "mzlv1QOP-xaXPofyp-arWvemq-arWBsw";
			public const string TYPE51 = "mzlv1QOP-TmXPlfyh-arWvemd-arWBsm";
			public const string CODE51 = "mzlv1QOP-xaXPofyh-arWvewa-arWBs6";
			public const string DESC51 = "mzlv1QOP-6XXPKfyO-arWve63-arcvsw";
			public const string VALU51 = "mzlv1QOP-p9XPzfyC-arWvewa-arWBsf";
			public const string DECP51 = "mzlv1QOP-6XXPKfyE-arWvemO-arWBs2";
            public const string XF950A = "0rrveQOP-71WDVQON-arWvewy-aroHA0";

			const string PAYMENT_TYPE_CC = "CC";
			const string PAYMENT_TYPE_DD = "DD";
			const string PAYMENT_TYPE_CS = "CS";
			const string PAYMENT_TYPE_CF = "CF";
			const string PAYMENT_TYPE_PP = "PP";
			const string PAYMENT_TYPE_EP = "EP";
			const string PAYMENT_TYPE_OA = "OA";
			const string PAYMENT_TYPE_PS = "PS";
			const string PAYMENT_TYPE_PD = "PD";
			const string PAYMENT_TYPE_LN = "LN";
			const string PAYMENT_TYPE_PO = "PO";
			const string PAYMENT_TYPE_CQ = "CQ";
			const string PAYMENT_TYPE_OT = "OT";
			const string PAYMENT_TYPE_GC = "GC";
			const string PAYMENT_TYPE_ZB = "ZB";
			const string PAYMENT_TYPE_CP = "CP";

			public const string BU_PAYMENT_TYPE_CODE = "rlEwyH1B-pm3Llt2h-arWvepO-arWP09";
			private const string MD501_TYPE_CODE = "PAYT";
			public DMPaymentType(DESettings settings, bool initialiseData)
				: base(ref settings, initialiseData)
			{
			}
			public override void SetModuleConfiguration()
			{
				IsCustomPaymentTypeCode = IsCustomPaymentType(variableKey1);
				DefaultTabType = DisplayTabsType.HORIZONTAL.ToString();

				// Front end payment code and description
				if (mode == MODE_CREATE)
				{
					MConfigs.Add(PAYMENT_TYPE_CODE, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> {VG.Mandatory , VG.MaxLength}, maxLength: 2));
				}
				else
				{
					MConfigs.Add(PAYMENT_TYPE_CODE, DataType.LABEL, DisplayTabs.TabHeaderGeneral);
				}
				MConfigs.Add(PAYMENT_TYPE_DESCRIPTION, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> {VG.Mandatory , VG.MaxLength}, maxLength: 50));
				MConfigs.Add(NAVIGATE_URL, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> {VG.Mandatory}));

				// Ticketing payment code is only valid for user defined
				if (IsCustomPaymentTypeCode)
				{
					MConfigs.Add(USER51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
					MConfigs.Add(PGMD51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
					MConfigs.Add(UPDT51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
					MConfigs.Add(ACTR51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
					MConfigs.Add(CONO51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
					MConfigs.Add(TYPE51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
					MConfigs.Add(CODE51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
					MConfigs.Add(DESC51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
					MConfigs.Add(VALU51, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> {VG.Mandatory , VG.Numeric , VG.MinValue , VG.MaxValue}, minValue: 84, maxValue: 100));
					MConfigs.Add(DECP51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
				}

				// Add all of the payment switches
				MConfigs.Add(DEFAULT_PAYMENT_TYPE, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(RETAIL_TYPE, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(TICKETING_TYPE, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(AGENT_RETAIL_TYPE, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(AGENT_TICKETING_TYPE, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(AGENT_TICKETING_TYPE_REFUND, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(TICKETING_TYPE_REFUND, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(GENERIC_SALES, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(GENERIC_SALES_REFUND, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(PPS_TYPE, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(IS_GIFT_CARD, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(BU_PAYMENT_TYPE_CODE, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(CALL_CREDIT_LIMIT_CHECK, DataType.BOOL, DisplayTabs.TabHeaderGeneral);


				switch (variableKey1)
				{
					case PAYMENT_TYPE_CC:
						MConfigs.Add(CreditCardPaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(CreditCardTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CreditCardSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CreditCardLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CreditCardMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CreditCardNoPayment, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CCExternalButtonText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CCExternalSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CCExternalTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CCTypeButtonText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CCTypeMessasge, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CCTypeSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CCTypeTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(TicketingCheckoutLabelCC, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(CC, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						break;
					case PAYMENT_TYPE_DD:
						MConfigs.Add(DirectDebitPaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(DirectDebitTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(DirectDebitSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(DirectDebitLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(DirectDebitMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(ConfirmDDMandateButtonText, DataType.TEXT, DisplayTabs.TabHeaderButtons);
						MConfigs.Add(CancelDDMandateButtonText, DataType.TEXT, DisplayTabs.TabHeaderButtons);
						MConfigs.Add(TicketingCheckoutLabelDD, DataType.TEXT, DisplayTabs.TabHeaderLabels);
                        MConfigs.Add(XF950A, DataType.BOOL_YN, DisplayTabs.TabHeaderGeneral);
						break;
					case PAYMENT_TYPE_CS:
						MConfigs.Add(CashbackPaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(CashbackTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CashbackSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CashbackLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CashbackMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CashTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CashSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CashLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CashMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CashPaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(TicketingCheckoutLabelCS, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						break;
					case PAYMENT_TYPE_CF:
						MConfigs.Add(CreditFinancePaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(CreditFinanceTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CreditFinanceSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CreditFinanceMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(CreditFinanceLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(ConfirmCFMandateButtonText, DataType.TEXT, DisplayTabs.TabHeaderButtons);
						MConfigs.Add(CancelCFMandateButtonText, DataType.TEXT, DisplayTabs.TabHeaderButtons);
						MConfigs.Add(TicketingCheckoutLabelCF, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						break;
					case PAYMENT_TYPE_PP:
						MConfigs.Add(PayPalPaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(PayPalTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(PayPalSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(PayPalLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(PayPalMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(PayPalReferenceText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(PPSPaymentOptionsMessage, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
						MConfigs.Add(MultiplePPSPaymentOptionsMessage1, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
						MConfigs.Add(AutoEnrolPPSCheckboxLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
						MConfigs.Add(AutoEnrolPPSLegend, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
						MConfigs.Add(MultiplePPSPaymentOptionsMessage2, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
						MConfigs.Add(AutoEnrolPPSTitle, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
						MConfigs.Add(AutoEnrolPPSCheckboxDefault, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
						break;
					case PAYMENT_TYPE_EP:
						MConfigs.Add(EPursePaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(EPurseTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(EPurseSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(EPurseLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(EPurseMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(SuccessfullyAppliedEPurse, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(TicketingCheckoutLabelEP, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						break;
					case PAYMENT_TYPE_OA:
						MConfigs.Add(OnAccountPaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(OA, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(OnAccountMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(OnAccountLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(OnAccountSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(OnAccountTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						break;
					case PAYMENT_TYPE_PS:
						MConfigs.Add(PointOfSalePaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(PointOfSaleTimeoutValue, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(PointOfSaleTerminalsLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(PointOfSaleMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(PointOfSaleLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(PointOfSaleSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(PointOfSaleTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						break;
					case PAYMENT_TYPE_PD:
						MConfigs.Add(PDQPaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(PDQTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(PDQSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(PDQLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(PDQMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(TicketingCheckoutLabelPD, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						break;
					case PAYMENT_TYPE_LN:
						MConfigs.Add(LN, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(LN_Mandatory, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						break;
					case PAYMENT_TYPE_PO:
						MConfigs.Add(PO, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(PO_Mandatory, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						break;
					case PAYMENT_TYPE_CQ:
						MConfigs.Add(TicketingCheckoutLabelCQ, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						break;
					case PAYMENT_TYPE_OT:
						MConfigs.Add(OtherPaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(OthersTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(OthersSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(OthersMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(OthersLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(SelectOthersType, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(OthersTextValue, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(OthersPaymentTypeMandatoryErrorMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(OthersH2, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						break;
					case PAYMENT_TYPE_GC:
						MConfigs.Add(GoogleCheckoutPaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(GoogleCheckoutMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(GoogleCheckoutLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(GoogleCheckoutSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(GoogleCheckoutTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						break;
					case PAYMENT_TYPE_ZB:
						MConfigs.Add(ZeroPricedBasketPaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(ZeroPricedBasketLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(ZeroPricedBasketMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(ZeroPricedBasketSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(ZeroPricedBasketTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						break;
					case PAYMENT_TYPE_CP:
						MConfigs.Add(ChipAndPinPaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(ChipAndPinTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(ChipAndPinSubTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(ChipAndPinLegend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(ChipAndPinMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						MConfigs.Add(ChipAndPinDevicesLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
						MConfigs.Add(ChipAndPinTimeoutValue, DataType.TEXT, DisplayTabs.TabHeaderTexts);
						break;
				}

				//Set additional hidden fields
				MConfigs.Add(QUALIFIER, DataType.HIDDEN, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(BUSINESS_UNIT, DataType.HIDDEN, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(PARTNER, DataType.HIDDEN, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(SEQUENCE, DataType.HIDDEN, DisplayTabs.TabHeaderTexts);
				MConfigs.Add(IS_OTHER_TYPE, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);

				Populate();
			}
			public override void SetHiddenFields()
			{
				dtHiddenFields.Rows.Add(BU_PAYMENT_TYPE_CODE, PAYMENT_TYPE_CODE, HiddenType.FOREIGN);
				dtHiddenFields.Rows.Add(QUALIFIER, "", HiddenType.DEFAULT);
				dtHiddenFields.Rows.Add(BUSINESS_UNIT, "", HiddenType.SETTING, PropertyName.BUSINESSUNIT.ToString());
				dtHiddenFields.Rows.Add(AGENT_TICKETING_TYPE, TICKETING_TYPE, HiddenType.FOREIGN);
				dtHiddenFields.Rows.Add(AGENT_RETAIL_TYPE, RETAIL_TYPE, HiddenType.FOREIGN);
				dtHiddenFields.Rows.Add(AGENT_TICKETING_TYPE_REFUND, TICKETING_TYPE_REFUND, HiddenType.FOREIGN);
                dtHiddenFields.Rows.Add(IS_OTHER_TYPE, IS_OTHER_TYPE, HiddenType.FOREIGN);

				if (IsCustomPaymentTypeCode || settings.Mode == "create")
				{
					dtHiddenFields.Rows.Add(USER51, "", HiddenType.SETTING, PropertyName.AGENTNAME.ToString());
					dtHiddenFields.Rows.Add(PGMD51, "", HiddenType.SETTING, PropertyName.STOREDPROC.ToString());
					dtHiddenFields.Rows.Add(UPDT51, "", HiddenType.SETTING, PropertyName.ISERIESTODAYSDATE.ToString());
					dtHiddenFields.Rows.Add(ACTR51, "", HiddenType.DEFAULT);
					dtHiddenFields.Rows.Add(CONO51, "", HiddenType.DEFAULT);
					dtHiddenFields.Rows.Add(TYPE51, "", HiddenType.DEFAULT);
					dtHiddenFields.Rows.Add(DESC51, PAYMENT_TYPE_DESCRIPTION, HiddenType.FOREIGN);
					dtHiddenFields.Rows.Add(DECP51, "", HiddenType.DEFAULT);
					dtHiddenFields.Rows.Add(CODE51, PAYMENT_TYPE_CODE, HiddenType.FOREIGN);

					// Sequence should have the ticketing payment code for custom payment types
					dtHiddenFields.Rows.Add(SEQUENCE, VALU51, HiddenType.FOREIGN);
				}
				else
				{
					// Otherwise it should already be set up so use the default
                    dtHiddenFields.Rows.Add(SEQUENCE, SEQUENCE, HiddenType.FOREIGN);
				}

			}
			public override string UpdateUrl()
			{
				string variableKey1 = string.Empty;
				string mode = string.Empty;
				string businessUnit = string.Empty;
				string URL = string.Empty;
				businessUnit = settings.BusinessUnit;
				if (settings.Mode == "create" && settings.Module_Name != string.Empty)
				{
					mode = "update";
					switch (settings.Module_Name.ToLower())
					{
						case "paymenttype":
							variableKey1 = settings.VariableKey1;
							URL = string.Format("systemdefaults.aspx?modulename=paymenttype&businessUnit={0}&variableKey1={1}&mode={2}", businessUnit, variableKey1, mode);
							break;
					}
				}
				return URL;
			}
			public override string BackUrl()
			{
				return string.Format("DefaultList.aspx?listname={0}&businessUnit={1}", settings.Module_Name, settings.BusinessUnit);
			}
			public override bool Validate()
			{

				// Validate the default validation first
				bool retVal = base.Validate();
				if (retVal)
				{

					DataAccess.DataObjects.tbl_payment_type objTblPaymentType = new DataAccess.DataObjects.tbl_payment_type(ref settings);
					string paymentTypeCode = string.Empty;
					string ticketingPaymentType = string.Empty;
					string currentTicketingPaymentType = string.Empty;
					bool CheckValid = true;
					string ValidationMessage = string.Empty;
					ModuleConfiguration objMConfig = null;
					if (settings.Mode == "create")
					{

						// Validate that the payment type does not exist on the ebusiness tables
						paymentTypeCode = System.Convert.ToString(MConfigs.Find(x => x.ConfigurationItem.DefaultName == "PAYMENT_TYPE_CODE").ConfigurationItem.UpdatedValue);
						CheckValid = System.Convert.ToBoolean(objTblPaymentType.IsPaymentTypeUnique(paymentTypeCode));
						if (!CheckValid)
						{
							ValidationMessage = "Payment type code already exists.";
							objMConfig = MConfigs.Find(x => x.ConfigurationItem.DefaultName == "PAYMENT_TYPE_CODE");
							objMConfig.ErrorMessage = ValidationMessage;
							retVal = false;
						}
					}

					// Has the ticketing payment type changed?
                    if (IsCustomPaymentTypeCode)
                    {
                        ticketingPaymentType = System.Convert.ToString(MConfigs.Find(x => x.ConfigID == VALU51).ConfigurationItem.UpdatedValue);
                        currentTicketingPaymentType = System.Convert.ToString(MConfigs.Find(x => x.ConfigID == VALU51).ConfigurationItem.CurrentValue);
                        if (!(string.IsNullOrWhiteSpace(ticketingPaymentType)) && (ticketingPaymentType != currentTicketingPaymentType))
                        {
                            // Ticketing payment type must also be unique
                            MD501 md501Obj = new MD501(ref settings);
                            CheckValid = md501Obj.IsPaymentTypeUnique(ticketingPaymentType);
                            if (!CheckValid)
                            {
                                ValidationMessage = "Ticketing code already exists.";
                                objMConfig = MConfigs.Find(x => x.ConfigID == VALU51);
                                objMConfig.ErrorMessage = ValidationMessage;
                                retVal = false;
                            }
                        }
                    }


				}

				return retVal;
			}

			private bool IsCustomPaymentTypeCode { get; set; }
			#region Private Methods

			private bool IsCustomPaymentType(string paymentTypeCode)
			{
				bool retVal = false;
				if (settings.Mode == "create")
				{
					retVal = true;
				}
				else
				{
					DataAccess.DataObjects.tbl_payment_type objTblPaymentType = new DataAccess.DataObjects.tbl_payment_type(ref settings);
					retVal = objTblPaymentType.IsCustomPaymentTypeCode(paymentTypeCode);
				}
				return retVal;
			}

			#endregion

		}
	}
}
