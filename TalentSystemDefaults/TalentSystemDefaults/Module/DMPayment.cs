namespace TalentSystemDefaults
{
	namespace TalentModules
	{
		public class DMPayment : DMBase
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
			static private string _ModuleTitle = "Payment";
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
			public const string CheckoutUpdatePanelScriptTimeoutValue = "rOEEXOfK-JDKe9doC-arWve93-arWvem";
			public const string PartPaymentAmountRegularExpression = "rOEEXOfK-rPEDhOo1-arWveaO-arWvep";
			public const string ShowTAndCCheckbox = "rOEEXOfK-J0T45XoB-arWvefq-arWve9";
			public const string StartingNumber = "rOxfXOxB-c0F06tbG-arWveph-arWvea";
			public const string MarketingCampaignSubTitle = "rOxfXOxB-rcv7ag#k-arWve9q-arWvRQ";
			public const string PaymentOptionsTitle = "rOxfXOxB-rjjutkLp-arWveaO-arWvRw";
			public const string PaymentOptionsMessage = "rOxfXOxB-rjjuhOAV-arWvefh-arWvRm";
			public const string PaymentOptionsLegend = "rOxfXOxB-rjju2i1Q-arWvefy-arWvR6";
			public const string SavedCardPaymentMethodLabel = "rOxfXOxB-rWvwmYxK-arWvea3-arWvRf";
			public const string PaymentOptionContinueButton = "rOxfXOxB-rjjuH1xQ-arWve2h-arWvMQ";
			public const string TAndCErrorMessage = "rOxfXOxB-pvl4WZoB-arWve0a-arWvM6";
			public const string TAndCLabelText = "rOxfXOxB-pWq4JISY-arWvRw3-arWvMp";
			public const string ConfirmSaleButtonText = "rOxfXOxB-Blx5BDbt-arWvefa-arWvMf";
			public const string SavedCardTitle = "rOxfXOxB-rWj6vhbB-arWve9h-arWvM2";
			public const string SavedCardSubTitle = "rOxfXOxB-rWz04eWv-arWve93-arWvM9";
			public const string SavedCardLegend = "rOxfXOxB-rWx22xe#-arWvefO-arWvMa";
			public const string LoadingText = "rOxfXOxB-BKEhGYYX-arWve2y-arWvA0";
			public const string ProcessingText = "rOxfXOxB-W8q4nvjZ-arWvefr-arWv1m";
			public const string NoPaymentOptionSelected = "rOxfXOxB-BOD6ghUY-arWveau-arWvZ9";
			public const string PromotionsLabel = "rOxfXOxB-Wc2e5DoE-arWve2h-arWvZa";
			public const string PromotionsButtonText = "rOxfXOxB-WcuXdDeK-arWvefd-arWvZ0";
			public const string PromotionApplied = "rOxfXOxB-WcDzSdsL-arWve0O-arWvsw";
			public const string NoPromoCodeError = "rOxfXOxB-BAxLOD4G-arWveah-arWvsm";
			public const string payError1 = "rOxfXOxB-rAvwQxxf-arWvRmy-arWvs6";
			public const string payError2 = "rOxfXOxB-rAvwQxxf-arWvRmd-arWvsp";
			public const string ErrorFinalisingOrder = "rOxfXOxB-Wcj#Oxx8-arWvR6u-arWvsf";
			public const string ErrorTakingPaymentAvs = "rOxfXOxB-WWZi4x#X-arWvR6y-arWvs2";
			public const string ErrorTakingPayment = "rOxfXOxB-WWZiOxYQ-arWvR6q-arWvs9";
			public const string PaidUsingLabel = "rOxfXOxB-rcv0nJjY-arWvefu-arWvs0";
			public const string NotPaidForLabel = "rOxfXOxB-BL2eQsoj-arWve9O-arWv0Q";
			public const string ResetPaymentsButton = "rOxfXOxB-QWa6cIoc-arWve9a-arWv0w";
			public const string SeasonTicketPaymentOptionsMessage = "rOxfXOxB-Qmx6od0E-arWvMp8-arWv06";
			public const string DeliveryAddressContinueButton = "rOxfXOxB-QvKQ6IeX-arWve2r-arWPR2";
			public const string DeliveryAddressSubTitleText = "rOxfXOxB-QvKQ6Iok-arWve0q-arWPR9";
			public const string DeliveryAddressTitleText = "rOxfXOxB-QvKQGAIB-arWvea8-arWPRa";
			public const string DeliveryAddressNotValidatedText = "rOxfXOxB-QvKQQhoa-arWvRmO-arWPR0";
			public const string PartPaymentLabel = "rOxfXOxB-rPEhdixv-arWve2q-arWPEf";
			public const string ProductOutOfStock = "rOxfXOxB-WjGR5Lit-arWvR0a-arWPE9";
			public const string CustomerPresentText = "rOxfXOxB-5Ox#vg4G-arWve28-arWPEa";
			public const string PartPaymentErrorMessage = "rOxfXOxB-rPELnIxB-arWvea3-arWPE0";
			public const string PartPaymentMandatoryErrorMessage = "rOxfXOxB-rPEF3x0Q-arWve0h-arWPAQ";
			public const string SuccessfullyAppliedPartPayment = "rOxfXOxB-58Swtge9-arWvRfq-arWPAw";
			public const string PartPaymentInvalidAmountErrorMessage = "rOxfXOxB-rPEpSv4Q-arWvRQr-arWPAm";
			public const string CheckoutConfirmationText = "rOxfXOxB-JDaRWA#B-arWvRwu-arWP1w";
			public const string ONACCOUNT_ENABLED = "jlefG3WV-Psg3zBcj-arWvefd-arcK12";
			public const string MerchandisePaymentDetails = "rOxfXOxB-Q0xiGdKK-arWv1wa-arWPsQ";
			public const string MerchandisePaymentMessage = "rOxfXOxB-Q0xiGOKa-arWvEwr-arWPsw";
			public const string TicketingPaymentMessageForGenericSale = "rOxfXOxB-tcvwnIeT-arWvR6h-arWPs6";
			public const string NextSaleButtonText = "rOxfXOxB-Q7E6G8Ly-arWve9y-arWPsp";
			public const string DisplayNextSaleButton = "rOEEXOfK-tPEs5dcf-arWve9d-arWPsf";
			public const string MarketingCampaignTitle = "rOxfXOxB-rcv7dSg4-arWvRm8-arcv0p";
			public const string ShowNumbersOnPaymentSteps = "rOEEXOfK-JlbIdI6#-arWveay-arcv06";
			public DMPayment(DESettings settings, bool initialiseData)
				: base(ref settings, initialiseData)
			{
			}
			public override void SetModuleConfiguration()
			{
				DefaultTabType = DisplayTabsType.HORIZONTAL.ToString();
				MConfigs.Add(StartingNumber, DataType.TEXT, DisplayTabs.TabHeaderOptions);
				MConfigs.Add(MarketingCampaignTitle, DataType.TEXT, DisplayTabs.TabHeaderOptions);
				MConfigs.Add(MarketingCampaignSubTitle, DataType.TEXT, DisplayTabs.TabHeaderOptions);
				MConfigs.Add(PaymentOptionsTitle, DataType.TEXT, DisplayTabs.TabHeaderOptions);
				MConfigs.Add(PaymentOptionsMessage, DataType.TEXT, DisplayTabs.TabHeaderOptions);
				MConfigs.Add(PaymentOptionsLegend, DataType.TEXT, DisplayTabs.TabHeaderOptions);
				MConfigs.Add(SavedCardPaymentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(PaymentOptionContinueButton, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(TAndCErrorMessage, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(TAndCLabelText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(ConfirmSaleButtonText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(SavedCardTitle, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(SavedCardSubTitle, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(SavedCardLegend, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(LoadingText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(ProcessingText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(NoPaymentOptionSelected, DataType.TEXT, DisplayTabs.TabHeaderOptions);
				MConfigs.Add(PromotionsLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(PromotionsButtonText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(PromotionApplied, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(NoPromoCodeError, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(payError1, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(payError2, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(ErrorFinalisingOrder, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(ErrorTakingPaymentAvs, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(ErrorTakingPayment, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(PaidUsingLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(NotPaidForLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(ResetPaymentsButton, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(SeasonTicketPaymentOptionsMessage, DataType.TEXT, DisplayTabs.TabHeaderOptions);
				MConfigs.Add(DeliveryAddressContinueButton, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(DeliveryAddressSubTitleText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(DeliveryAddressTitleText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(DeliveryAddressNotValidatedText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(PartPaymentLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(ProductOutOfStock, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(CustomerPresentText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(PartPaymentErrorMessage, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(PartPaymentMandatoryErrorMessage, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(SuccessfullyAppliedPartPayment, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(PartPaymentInvalidAmountErrorMessage, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(CheckoutUpdatePanelScriptTimeoutValue, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(PartPaymentAmountRegularExpression, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(ShowNumbersOnPaymentSteps, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(ShowTAndCCheckbox, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(DisplayNextSaleButton, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(CheckoutConfirmationText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(MerchandisePaymentDetails, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(MerchandisePaymentMessage, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(TicketingPaymentMessageForGenericSale, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(NextSaleButtonText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				MConfigs.Add(ONACCOUNT_ENABLED, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
				Populate();
			}
		}
	}
}
