using System.Collections.Generic;
namespace TalentSystemDefaults
{
	public class DisplayTabs
	{
		private Dictionary<string, string> pageTexts;
		public DisplayTabs(Dictionary<string, string> pageTexts)
		{
			// VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
			TabTypes = new Dictionary<string, string>()
			{
				{DisplayTabsType.HORIZONTAL.ToString(), ""},
					{DisplayTabsType.VERTICAL.ToString(), "float: right;width: 75%;"}
					};
			this.pageTexts = pageTexts;
			InitialiseClasses();
		}
		public const string TabHeaderGeneralKey = "TabHeaderGeneral";
		public const string TabHeaderDefaultsKey = "TabHeaderDefaults";
		public const string TabHeaderTextsKey = "TabHeaderTexts";
		public const string TabHeaderAttributesKey = "TabHeaderAttributes";
		public const string TabHeaderLabelsKey = "TabHeaderLabels";
		public const string TabHeaderExpressionsKey = "TabHeaderExpressions";
		public const string TabHeaderErrorsKey = "TabHeaderErrors";
		public const string TabHeaderButtonsKey = "TabHeaderButtons";
		public const string TabHeaderValidationKey = "TabHeaderValidation";
		public const string TabHeaderPasswordKey = "TabHeaderPassword";
		public const string TabHeaderPhoneKey = "TabHeaderPhone";
		public const string TabHeaderAddressKey = "TabHeaderAddress";
		public const string TabHeaderOptionsKey = "TabHeaderOptions";
		public const string TabHeaderCreditCardKey = "TabHeaderCreditCard";
        public const string TabHeaderTicketExchangeCustomerSummaryKey = "TabHeaderTicketExchangeCustomerSummary";
        public const string TabHeaderTicketExchangeCustomerProductListKey = "TabHeaderTicketExchangeCustomerProductList";
        public const string TabHeaderTicketExchangeProcessKey = "TabHeaderTicketExchangeProcess";
        public const string TabHeaderTicketExchangeDefaultsKey = "TabHeaderTicketExchangeDefaults";
        public const string TabHeaderHospitalityDetailsKey = "TabHeaderHospitalityDetails";
        public const string TabHeaderHospitalityBookingEnquiryKey = "TabHeaderHospitalityBookingEnquiry";
        public const string TabHeaderCustomerSearchKey = "TabHeaderCustomerSearch";
        public const string TabHeaderCompanySearchKey = "TabHeaderCompanySearch";
        public const string TabHeaderPurchaseHistoryKey = "TabHeaderPurchaseHistory";
        public const string TabHeaderPurchaseDetailsKey = "TabHeaderPurchaseDetails";
        public const string TabHeaderRelatingHospitalityProductUpsellKey = "TabHeaderRelatingHospitalityProductUpsell";
        public const string TabHeaderRelatingHospitalityPreBookingDataCaptureKey = "TabHeaderRelatingHospitalityPreBookingDataCapture";
        

        public string TabHeaderGeneral
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderGeneralKey)) ? (pageTexts[TabHeaderGeneralKey]) : "General");
			}
		}
		public string TabHeaderDefaults
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderDefaultsKey)) ? (pageTexts[TabHeaderDefaultsKey]) : "Defaults");
			}
		}
		public string TabHeaderTexts
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderTextsKey)) ? (pageTexts[TabHeaderTextsKey]) : "Texts");
			}
		}
		public string TabHeaderAttributes
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderAttributesKey)) ? (pageTexts[TabHeaderAttributesKey]) : "Attributes");
			}
		}
		public string TabHeaderLabels
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderLabelsKey)) ? (pageTexts[TabHeaderLabelsKey]) : "Labels");
			}
		}
		public string TabHeaderExpressions
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderExpressionsKey)) ? (pageTexts[TabHeaderExpressionsKey]) : "Expressions");
			}
		}
		public string TabHeaderErrors
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderErrorsKey)) ? (pageTexts[TabHeaderErrorsKey]) : "Errors");
			}
		}
		public string TabHeaderButtons
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderButtonsKey)) ? (pageTexts[TabHeaderButtonsKey]) : "Buttons");
			}
		}
		public string TabHeaderValidation
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderValidationKey)) ? (pageTexts[TabHeaderValidationKey]) : "Validation");
			}
		}
		public string TabHeaderPassword
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderPasswordKey)) ? (pageTexts[TabHeaderPasswordKey]) : "Password");
			}
		}
		public string TabHeaderPhone
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderPhoneKey)) ? (pageTexts[TabHeaderPhoneKey]) : "Phone");
			}
		}
		public string TabHeaderAddress
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderAddressKey)) ? (pageTexts[TabHeaderAddressKey]) : "Address");
			}
		}
		public string TabHeaderOptions
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderOptionsKey)) ? (pageTexts[TabHeaderOptionsKey]) : "Options");
			}
		}
		public string TabHeaderCreditCard
		{
			get
			{
				return ((pageTexts.ContainsKey(TabHeaderCreditCardKey)) ? (pageTexts[TabHeaderCreditCardKey]) : "Credit Card");
			}
		}
        public string TabHeaderTicketExchangeCustomerSummary
        {
            get
            {
                return ((pageTexts.ContainsKey(TabHeaderTicketExchangeCustomerSummaryKey)) ? (pageTexts[TabHeaderTicketExchangeCustomerSummaryKey]) : "Customer Summary");
            }
        }
        public string TabHeaderTicketExchangeCustomerProductList
        {
            get
            {
                return ((pageTexts.ContainsKey(TabHeaderTicketExchangeCustomerProductListKey)) ? (pageTexts[TabHeaderTicketExchangeCustomerProductListKey]) : "Customer Product List");
            }
        }
        public string TabHeaderTicketExchangeProcess
        {
            get
            {
                return ((pageTexts.ContainsKey(TabHeaderTicketExchangeProcessKey)) ? (pageTexts[TabHeaderTicketExchangeProcessKey]) : "Ticket Exchange Process");
            }
        }

        public string TabHeaderTicketExchangeDefaults
        {
            get
            {
                return ((pageTexts.ContainsKey(TabHeaderTicketExchangeDefaultsKey)) ? (pageTexts[TabHeaderTicketExchangeDefaultsKey]) : "Ticket Exchange Defaults ");
            }
        }

        public string TabHeaderHospitalityDetails
        {
            get
            {
                return ((pageTexts.ContainsKey(TabHeaderHospitalityDetailsKey)) ? (pageTexts[TabHeaderHospitalityDetailsKey]) : "Hospitality Details");
            }
        }

        public string TabHeaderHospitalityBookingEnquiry
        {
            get
            {
                return ((pageTexts.ContainsKey(TabHeaderHospitalityBookingEnquiryKey)) ? (pageTexts[TabHeaderHospitalityBookingEnquiryKey]) : "Hospitality Booking Enquiry");
            }
        }

        public string TabHeaderCustomerSearch
        {
            get
            {
                return ((pageTexts.ContainsKey(TabHeaderCustomerSearchKey)) ? (pageTexts[TabHeaderCustomerSearchKey]) : "Customer Search");
            }
        }

        public string TabHeaderCompanySearch
        {
            get
            {
                return ((pageTexts.ContainsKey(TabHeaderCompanySearchKey)) ? (pageTexts[TabHeaderCompanySearchKey]) : "Company Search");
            }
        }
        public string TabHeaderPurchaseHistory
        {
            get
            {
                return ((pageTexts.ContainsKey(TabHeaderPurchaseHistoryKey)) ? (pageTexts[TabHeaderPurchaseHistoryKey]) : "History");
            }
        }
        public string TabHeaderPurchaseDetails
        {
            get
            {
                return ((pageTexts.ContainsKey(TabHeaderPurchaseDetailsKey)) ? (pageTexts[TabHeaderPurchaseDetailsKey]) : "Details");
            }
        }

        public string TabHeaderRelatingHospitalityProductUpsell
        {
            get
            {
                return ((pageTexts.ContainsKey(TabHeaderRelatingHospitalityProductUpsellKey)) ? (pageTexts[TabHeaderRelatingHospitalityProductUpsellKey]) : "Hospitality to Product Link");
            }
        }

        public string TabHeaderRelatingHospitalityPreBookingDataCapture
        {
            get
            {
                return ((pageTexts.ContainsKey(TabHeaderRelatingHospitalityPreBookingDataCaptureKey)) ? (pageTexts[TabHeaderRelatingHospitalityPreBookingDataCaptureKey]) : "Hospitality Pre-Booking Data Capture");
            }
        }


        public Dictionary<string, string> Classes;
		public static Dictionary<string, string> TabTypes; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
		private void InitialiseClasses()
		{
			Classes = new Dictionary<string, string>()
						{
							{TabHeaderGeneral, "fa-toggle-off"},
								{TabHeaderDefaults, "fa-cog"},
									{TabHeaderTexts, "fa-font"},
										{TabHeaderAttributes, "fa-table"},
											{TabHeaderLabels, "fa-tag"},
												{TabHeaderExpressions, "fa-meh-o"},
													{TabHeaderErrors, "fa-exclamation"},
														{TabHeaderButtons, "fa-toggle-off"},
															{TabHeaderValidation, "fa-check"},
																{TabHeaderPassword, "fa-user-secret"},
																	{TabHeaderPhone, "fa-phone-square"},
																		{TabHeaderAddress, "fa-university"},
																			{TabHeaderOptions, "fa-filter"},
																				{TabHeaderCreditCard, "fa-credit-card"},
                                                                                    {TabHeaderTicketExchangeCustomerSummary, "fa-cog"},
																			            {TabHeaderTicketExchangeCustomerProductList, "fa-cog"},
                                                                                          {TabHeaderTicketExchangeDefaults, "fa-cog"},
																				            {TabHeaderTicketExchangeProcess, "fa-cog"},
                                                                                            {TabHeaderHospitalityDetails, "fa-cog"},
                                                                                            {TabHeaderHospitalityBookingEnquiry, "fa-cog"},
                                                                                            {TabHeaderCustomerSearch, "fa-search"},
                                                                                            {TabHeaderCompanySearch, "fa-search"},
                                                                                            {TabHeaderPurchaseHistory, "fa-history"},
                                                                                            {TabHeaderPurchaseDetails, "fa-server"},
                                                                                            {TabHeaderRelatingHospitalityProductUpsell, "fa-cog"},
                                                                                            {TabHeaderRelatingHospitalityPreBookingDataCapture, "fa-cog"}
                                                                                            };
		}
	}
	public enum DisplayTabsType
	{
		HORIZONTAL,
		VERTICAL
	}
}
