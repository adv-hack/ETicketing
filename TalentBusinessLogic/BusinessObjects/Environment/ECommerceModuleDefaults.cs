using System;
using System.Data;
using System.Web;
using Talent.Common;

namespace TalentBusinessLogic.BusinessObjects.Environment
{
    public class ECommerceModuleDefaults : DEEcommerceModuleDefaults
    {
        private BusinessObjects businessObjects;

        public ECommerceModuleDefaults()
        {
            businessObjects = new BusinessObjects();
        }

        public DefaultValues GetDefaults(string partner, string businessUnit)
        {
            DefaultValues moduleDefaultValues = new DefaultValues();
            moduleDefaultValues = GetDefaultValues(partner, businessUnit);
            return moduleDefaultValues;
        }

        public DefaultValues GetDefaults()
        {
            string partner = businessObjects.Environment.Settings.GetPartner(); //Get it from the environment business object
            string businessUnit = businessObjects.Environment.Settings.BusinessUnit; //Get it from the environment business object

            DefaultValues moduleDefaultValues = new DefaultValues();
            moduleDefaultValues = GetDefaultValues(partner, businessUnit);
            return moduleDefaultValues;
        }

        /// <summary>
        /// Get the ecommerce module defaults values for the given partner and business unit.
        /// This function uses a local TalentDataObjects variable and retreives a basic settings object in order to prevent a circular reference.
        /// </summary>
        /// <param name="partner">The partner code</param>
        /// <param name="businessUnit">The given business unit</param>
        /// <returns>This function returns the defaults values object.</returns>
        private DefaultValues GetDefaultValues(string partner, string businessUnit)
        {
            // Declare this first! Used for Logging function duration
            TimeSpan timeSpan = DateTime.Now.TimeOfDay;
            DefaultValues def = new DefaultValues();
            string cacheKey = "ECommerceModuleDefaults" + Talent.Common.Utilities.FixStringLength(businessUnit, 50) + Talent.Common.Utilities.FixStringLength(partner, 50);

            if (Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey))
            {
                def = (DefaultValues)businessObjects.Data.Cache.Get(cacheKey);
            }
            else
            {
                TalentDataObjects talDataObjects = new TalentDataObjects();
                talDataObjects.Settings = businessObjects.Environment.Settings.GetBasicSettingsObject();
                DataTable defaults = talDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetDefaultValues(Talent.Common.Utilities.GetAllString(), businessUnit);
                try
                {
                    if (defaults.Rows.Count > 0)
                    {
                        def = PopulateDefaults(def, defaults);
                        if (!string.IsNullOrEmpty(def.PriceList))
                            def.DefaultPriceList = def.PriceList;
                    }

                    //Get any additional Partner specific values

                    defaults = talDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetDefaultValues(partner, businessUnit);
                    if (defaults.Rows.Count > 0)
                    {
                        def = PopulateDefaults(def, defaults);
                    }

                }
                catch (Exception)
                {
                }

                // Add to cache
                businessObjects.Data.Cache.Add(cacheKey, def, businessObjects.Environment.Settings.CacheTimeInMins);
            }

            businessObjects.Logging.LoadTestLog("ECommerceModuleDefaults.vb", "GetDefaultValues", timeSpan);
            return def;
        }

        protected DefaultValues PopulateDefaults(DefaultValues def, DataTable defaults)
        {
            try
            {
                foreach (DataRow defRow in defaults.Rows)
                {
                    try
                    {
                        switch (defRow["DEFAULT_NAME"].ToString())
                        {
                            case  "DT_CREATEPARTNERONADDCUSTOMER":
                                def.DataTransfer_CreatePartnerOnAddCustomer = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CULTURE":
                                def.Culture = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CONFIRMATION_EMAIL":
                                def.ConfirmationEmail = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CUSTOMER_NUMBER_PREFIX":
                                def.CustomerNumberPrefix = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "DEFAULT_BU":
                                def.DefaultBu = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "DELIVERY_COST_PERC":
                                def.DeliveryCostPerc = businessObjects.Data.Validation.CheckForDBNull_Decimal(defRow["VALUE"]);
                                break;
                            case  "DELIVERY_ROUNDING_MASK":
                                def.DeliveryRoundingMask = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "HTML_PATH_ABSOLUTE":
                                def.HtmlPathAbsolute = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "HTML_PER_PAGE":
                                def.HtmlPerPage = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "HTML_PER_PAGE_TYPE":
                                def.HtmlPerPageType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "IMAGE_PATH_ABSOLUTE":
                                def.ImagePathAbsolute = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "IMAGE_PATH_VIRTUAL":
                                def.ImagePathVirtual = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "IMAGE_SSL_PATH_VIRTUAL":
                                def.ImageSslPathVirtual = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "GLOBAL_IMAGE_PATH":
                                def.GlobalImagePath = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "GLOBAL_PATH_VIRTUAL":
                                def.GlobalPathVirtual = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "GLOBAL_SSL_PATH_VIRTUAL":
                                def.GlobalSslPathVirtual = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "NUMBER_OF_GROUP_LEVELS":
                                def.NumberOfGroupLevels = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "ORDER_NUMBER":
                                def.OrderNumber = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "ORDER_NUMBER_PREFIX":
                                def.OrderNumberPrefix = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PARTNER_NUMBER":
                                def.PartnerNumber = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_GATEWAY_TYPE":
                                def.PaymentGatewayType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_DETAILS_1":
                                def.PaymentDetails1 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_DETAILS_2":
                                def.PaymentDetails2 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_DETAILS_3":
                                def.PaymentDetails3 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_DETAILS_4":
                                def.PaymentDetails4 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_DETAILS_5":
                                def.PaymentDetails5 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_URL_1":
                                def.PaymentURL1 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_URL_2":
                                def.PaymentURL2 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_URL_3":
                                def.PaymentURL3 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_REJECT_ADDRESS_AVS":
                                def.PaymentRejectAddressAVS = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_REJECT_POSTCODE_AVS":
                                def.PaymentRejectPostcodeAVS = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_REJECT_CSC":
                                def.PaymentRejectCSC = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_ALLOW_PARTIAL_AVS":
                                def.PaymentAllowPartialAVS = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_DEBUG":
                                def.PaymentDebug = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_CALL_BANK_API":
                                def.PaymentCallBankAPI = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_IFRAMEWIDTH":
                                def.Payment3DSecureIFrameWidth = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_PROVIDER":
                                def.Payment3DSecureProvider = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_SUCCESS_CODES":
                                def.Payment3DSecureSuccessCodes = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]).Split(',');
                                break;
                            case  "PAYMENT_3DSECURE_DETAILS_1":
                                def.Payment3DSecureDetails1 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_DETAILS_2":
                                def.Payment3DSecureDetails2 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_DETAILS_3":
                                def.Payment3DSecureDetails3 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_DETAILS_4":
                                def.Payment3DSecureDetails4 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_DETAILS_5":
                                def.Payment3DSecureDetails5 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_DETAILS_6":
                                def.Payment3DSecureDetails6 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_DETAILS_7":
                                def.Payment3DSecureDetails7 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_DETAILS_8":
                                def.Payment3DSecureDetails8 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_DETAILS_9":
                                def.Payment3DSecureDetails9 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_DETAILS_10":
                                def.Payment3DSecureDetails10 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_OUR_ACCOUNT_ID":
                                def.Payment3dSecureOurAccountId = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_OUR_SYSTEM_ID":
                                def.Payment3dSecureOurSystemId = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_OUR_PASSCODE":
                                def.Payment3dSecureOurPasscode = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_OUR_SYSTEM_GUID":
                                def.Payment3dSecureOurSystemGUID = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_URL_1":
                                def.Payment3DSecureURL1 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_URL_2":
                                def.Payment3DSecureURL2 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PRICE_LIST":
                                def.PriceList = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "STOCK_LOCATION":
                                def.StockLocation = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "TEMP_ORDER_NUMBER":
                                def.TempOrderNumber = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "THEME":
                                def.Theme = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "USER_NUMBER":
                                def.UserNumber = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "USE_AGE_CHECK":
                                def.Use_Age_Check = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ORDER_DESTINATION_DATABASE":
                                def.OrderDestinationDatabase = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CUSTOMER_DESTINATION_DATABASE":
                                def.CustomerDestinationDatabase = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "STORED_PROCEDURE_GROUP":
                                def.StoredProcedureGroup = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PRODUCT_HTML":
                                def.ProductHTML = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PRODUCT_HTML_TYPE":
                                def.ProductHTMLType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "MIN_ADD_QUANTITY":
                                def.Min_Add_Quantity = businessObjects.Data.Validation.CheckForDBNull_Decimal(defRow["VALUE"]);
                                break;
                            case  "DEFAULT_ADD_QUANTITY":
                                def.Default_Add_Quantity = businessObjects.Data.Validation.CheckForDBNull_Decimal(defRow["VALUE"]);
                                break;
                            case  "CONTACT_US_TO_EMAIL":
                                def.ContactUsToEmail = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CONTACT_US_TO_EMAIL_CC":
                                def.ContactUsToEmailCC = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CONTACT_US_FROM_EMAIL":
                                def.ContactUsConfirmationFromEmail = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CONTACT_US_FORM_SHOULD_ALWAYS_EMAIL":
                                def.ContactUsFormShouldAlwaysEmail = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "FORGOTTEN_PASSWORD_FROM_EMAIL":
                                def.ForgottenPasswordFromEmail = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ORDERS_FROM_EMAIL":
                                def.OrdersFromEmail = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "REGISTRATION_CONFIRMATION_FROM_EMAIL":
                                def.RegistrationConfirmationFromEmail = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CRM_BRANCH":
                                def.CrmBranch = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ORDER_CARRIER_CODE":
                                def.OrderCarrierCode = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ORDER_CREATION_RETRY":
                                def.OrderCreationRetry = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ORDER_CREATION_RETRY_ATTEMPTS":
                                def.OrderCreationRetryAttempts = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "ORDER_CREATION_RETRY_WAIT":
                                def.OrderCreationRetryWait = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "ORDER_CREATION_RETRY_ERRORS":
                                def.OrderCreationRetryErrors = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "STOCK_CHECK_RETRY":
                                def.StockCheckRetry = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "STOCK_CHECK_RETRY_ATTEMPTS":
                                def.StockCheckRetryAttempts = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "STOCK_CHECK_RETRY_WAIT":
                                def.StockCheckRetryWait = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "STOCK_CHECK_RETRY_ERRORS":
                                def.StockCheckRetryErrors = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "STOCK_CHECK_IGNORE_ERRORS":
                                def.StockCheckIgnoreErrors = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "REGISTRATION_RETRY":
                                def.RegistrationRetry = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "REGISTRATION_RETRY_ATTEMPTS":
                                def.RegistrationRetryAttempts = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "REGISTRATION_RETRY_WAIT":
                                def.RegistrationRetryWait = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "REGISTRATION_RETRY_ERRORS":
                                def.RegistrationRetryErrors = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "LOGIN_TO_PRODUCT_BROWSE":
                                def.LoginToProductBrowse = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]).ToString();
                                break;
                            case  "PAGES_LOGIN_PATH":
                                def.PagesLoginPath = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "REGISTRATION_ENABLED":
                                def.RegistrationEnabled = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]).ToString();
                                break;
                            case  "REGISTRATION_TEMPLATE":
                                def.RegistrationTemplate = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "UPDATE_DETAILS_TEMPLATE":
                                def.UpdateDetailsTemplate = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CREDITNOTE_HEADER_TEMPLATE":
                                def.CreditNoteHeaderTemplate = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CREDITNOTE_DETAIL_TEMPLATE":
                                def.CreditNoteDetailTemplate = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "INVOICE_HEADER_TEMPLATE":
                                def.InvoiceHeaderTemplate = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "INVOICE_DETAIL_TEMPLATE":
                                def.InvoiceDetailTemplate = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "STATEMENT_HEADER_TEMPLATE":
                                def.StatementHeaderTemplate = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PERFORM_CREDIT_CHECK":
                                def.PerformCreditCheck = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "DISABLE_CHECKOUT_IF_CREDIT_EXCEEDED":
                                def.DisableCheckoutIfOverCreditLimit = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SEND_EMAIL_IF_CREDIT_LIMIT_EXCEEDED":
                                def.SendEmail_IfOrderCreditLimitExceeded = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "CREDIT_LIMIT_EXCEEDED_EMAIL_TO_ADDRESS":
                                def.EmailToAddress_OrderCreditLimitExceeded = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CREDIT_LIMIT_EXCEEDED_EMAIL_FROM_ADDRESS":
                                def.EmailFromAddress_OrderCreditLimitExceeded = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "SAVE_ORDERS":
                                def.SaveOrders = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ORDER_TEMPLATES":
                                def.OrderTemplates = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SEND_REGISTRATION_TO_BACK_END":
                                def.SendRegistrationToBackEnd = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "USE_LOGIN_LOOKUP":
                                def.UseLoginLookup = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "LOGIN_LOOKUP_TYPE":
                                def.LoginLookupType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PROMOTION_PRIORITY":
                                def.PromotionPriority = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PROFILE_SEX_MANDATORY":
                                def.ProfileSexMandatory = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SEND_ORDER_CONF_TO_CUST_SERV":
                                def.SendOrderConfToCustServ = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CUSTOMER_SERVICES_EMAIL":
                                def.CustomerServicesEmail = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PRODUCT_TEMPLATE_TYPE":
                                def.ProductTemplateType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_PROCESS_3D_SECURE":
                                def.PaymentProcess3dSecure = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "GIFT_MESSAGE_EMAIL_FROM":
                                def.GiftMessageEmail_From = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "GIFT_MESSAGE_EMAIL_TO":
                                def.GiftMessageEmail_To = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ZOOMIFY_FLASH_LOCATION":
                                def.ZoomifyFlashFileURL = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ZOOMIFY_WIDTH":
                                def.ZoomifyWidth = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "ZOOMIFY_HEIGHT":
                                def.ZoomifyHeight = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "ZOOMIFY_IN_USE":
                                def.ZoomifyInUse = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ADDITIONAL_SEARCH_TYPE":
                                def.AdditionalSearchType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ADDITIONAL_SEARCH_TYPE_CRITERIA":
                                def.AdditionalSearchTypeCriteria = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CALL_TAX_WEBSERVICE":
                                def.Call_Tax_WebService = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SHOW_LOGIN_CONTROL_ON_LOGIN_REMINDER_PAGE":
                                def.Show_Login_Control_On_Reminder_Page = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SHOW_MINI_LOGIN_CONTROL":
                                def.Show_Mini_Login_Control = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SHOW_GIFT_MESSAGE_OPTION":
                                def.Show_Gift_Message_Option = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "HTML_PATH_RELATIVE":
                                def.HtmlIncludePathRelative = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "HIDE_UNSELECTED_TOP_LEVEL_GROUPS":
                                def.HideUnselectedTopLevelGroups = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "HIDE_ALL_TOP_LEVEL_GROUPS":
                                def.HideAllTopLevelGroups = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "USER_NUMBER_PREFIX":
                                def.UserNumberPrefix = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "USE_GROUP_LEVEL01_SHOW_CHILDREN_AS_GROUPS":
                                def.Override_Show_Children_As_Groups_Using_L01_Value = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PERFORM_FRONT_END_STOCK_CHECK":
                                def.Perform_Front_End_Stock_Check = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "PERFORM_BACK_END_STOCK_CHECK":
                                def.Perform_Back_End_Stock_Check = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "ADDRESS_LINE1_ROW_VISIBLE":
                                def.AddressLine1RowVisible = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ADDRESS_LINE2_ROW_VISIBLE":
                                def.AddressLine2RowVisible = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ADDRESS_LINE3_ROW_VISIBLE":
                                def.AddressLine3RowVisible = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ADDRESS_LINE4_ROW_VISIBLE":
                                def.AddressLine4RowVisible = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ADDRESS_LINE5_ROW_VISIBLE":
                                def.AddressLine5RowVisible = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ADDRESS_POSTCODE_ROW_VISIBLE":
                                def.AddressPostcodeRowVisible = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case "ADDRESS_COUNTRY_ROW_VISIBLE":
                                def.AddressCountryRowVisible = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SEARCH_RESULTS_DISPLAY_TYPE":
                                def.Search_Results_Display_Type = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "ORDER_ENQUIRY_TEMPLATE_TYPE":
                                def.Order_Enquiry_Template_Type = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PRODUCT_LIST_GRAPHICAL_TEMPLATE_TYPE":
                                def.Product_List_Graphical_Template_Type = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "PRODUCT_RELATIONSHIPS_TEMPLATE_TYPE":
                                def.ProductRelationshipsTemplateType = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "VALIDATE_EXTERNAL_PASSWORD":
                                def.ValidateExternalPassword = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "EXTERNAL_PASSWORD_VALUE":
                                def.ExternalPasswordValue = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "EXTERNAL_PASSWORD_IS_MASTER":
                                def.ExternalPasswordIsMaster = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SEND_REGISTRATION_TO_BACKEND_FIRST":
                                def.SendRegistrationToBackendFirst = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALLOW_DUPLICATE_EMAIL":
                                def.AllowDuplicateEmail = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "LOGINID_IS_CUSTOMER_NUMBER":
                                def.LoginidIsCustomerNumber = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ADD_CUSTOMER_TO_MY_FRIENDS_AND_FAMILY":
                                def.AddCustomerToMyFriendsAndFamily = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ADD_ME_TO_THEIR_FRIENDS_AND_FAMILY":
                                def.AddMeToTheirMyFriendsAndFamily = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "REGISTER_CUSTOMER_FOR_FRIENDS_AND_FAMILY":
                                def.RegisterCustomerForFriendsAndFamily = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "FRIENDS_AND_FAMILY":
                                def.FriendsAndFamily = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "USE_DEFAULT_COUNTRY_ON_DELIVERY_ADDRESS":
                                def.UseDefaultCountryOnDeliveryAddress = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "USE_DEFAULT_COUNTRY_ON_REGISTRATION":
                                def.UseDefaultCountryOnRegistration = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "HOME_PRODUCT_MAX_BUY":
                                def.HomeProduct_MaxBuy = businessObjects.Data.Validation.CheckForDBNull_Decimal(defRow["VALUE"]);
                                break;
                            case  "AWAY_PRODUCT_MAX_BUY":
                                def.AwayProduct_MaxBuy = businessObjects.Data.Validation.CheckForDBNull_Decimal(defRow["VALUE"]);
                                break;
                            case  "TRAVEL_PRODUCT_MAX_BUY":
                                def.TravelProduct_MaxBuy = businessObjects.Data.Validation.CheckForDBNull_Decimal(defRow["VALUE"]);
                                break;
                            case  "MEMBERSHIP_PRODUCT_MAX_BUY":
                                def.MembershipProduct_MaxBuy = businessObjects.Data.Validation.CheckForDBNull_Decimal(defRow["VALUE"]);
                                break;
                            case  "HOME_PRODUCT_FORWARD_TO_BASKET":
                                def.HomeProduct_ForwardToBasket = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "AWAY_PRODUCT_FORWARD_TO_BASKET":
                                def.AwayProduct_ForwardToBasket = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "TRAVEL_PRODUCT_FORWARD_TO_BASKET":
                                def.TravelProduct_ForwardToBasket = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "EVENT_PRODUCT_FORWARD_TO_BASKET":
                                def.EventProduct_ForwardToBasket = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "MEMBERSHIP_PRODUCT_FORWARD_TO_BASKET":
                                def.MembershipProduct_ForwardToBasket = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "TICKETING_CHECKOUT":
                                def.TicketingCheckout = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_BILLING_ADDRESS":
                                def.PaymentBillingAddress = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_ORDER_SUMMARY":
                                def.PaymentOrderSummary = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CLEAR_BACK_END_BASKET_ON_LOGIN":
                                def.ClearBackEndBasketOnLogin = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "DISPLAY_ACCOUNT_SELECTION_ON_LOGIN":
                                def.DisplayAccountSelectionOnLogin = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PDF_PATH_ABSOLUTE":
                                def.Pdf_Path_Absolute = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ALLOW_CHECKOUT_WHEN_NOT_IN_STOCK":
                                def.AllowCheckoutWhenNoStock = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ZOOMIFY_DISPLAY_NAV_VALUE":
                                def.ZoomifyDisplayNavValue = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case "CAPACITY_BY_STADIUM":
                                def.CapacityByStadium = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case "OVERRIDE_SHOW_PRODUCTS_DISPLAY_WITH_LEVEL1_VALUE":
                                def.Override_Show_Products_Display_Using_L01_Value = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ADDRESSING_LAYOUT":
                                def.AddressingLayout = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ADDRESSING_FIELDS":
                                def.AddressingFields = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ADDRESSING_MAP_COMPANY_NAME":
                                def.AddressingMapCompanyName = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ADDRESSING_MAP_ORG":
                                def.AddressingMapOrganisation = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ADDRESSING_MAP_ADR1":
                                def.AddressingMapAdr1 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ADDRESSING_MAP_ADR2":
                                def.AddressingMapAdr2 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ADDRESSING_MAP_ADR3":
                                def.AddressingMapAdr3 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ADDRESSING_MAP_ADR4":
                                def.AddressingMapAdr4 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ADDRESSING_MAP_ADR5":
                                def.AddressingMapAdr5 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case "ADDRESSING_MAP_POST":
                                def.AddressingMapPost = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ADDRESSING_MAP_COUNTRY":
                                def.AddressingMapCountry = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ADDRESSING_PROVIDER":
                                def.AddressingProvider = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "SHOW_ACCOUNT_REGISTRATION_ON_LOGIN_SCREEN":
                                def.ShowRegisterAccountOnLoginPage = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SHOW_ACCOUNT_ACTIVATION_ON_LOGIN_SCREEN":
                                def.ShowActivateAccountOnLoginPage = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "REFRESH_CUSTOMER_DATA_ON_LOGIN":
                                def.RefreshCustomerDataOnLogin = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "LOGINID_TYPE":
                                def.LoginidType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "RESTRICT_USER_PAYMENT_TYPE":
                                def.RestrictUserPaymentType = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "USE_VISUAL_SEAT_SELECTION":
                                def.UseVisualSeatSelection = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CALCULATE_DELIVERY_DATE":
                                def.CalculateDeliveryDate = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "TICKETING_STADIUM":
                                def.TicketingStadium = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "AUTO_CREATE_ACCOUNT_NO":
                                def.AutoCreateAccountNo = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ACCOUNT_1_PREFIX":
                                def.Account1Prefix = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ACCOUNT_2_PREFIX":
                                def.Account2Prefix = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ACCOUNT_1_LENGTH":
                                def.Account1Length = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "ACCOUNT_2_LENGTH":
                                def.Account2Length = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "ACCOUNT_1_NEXT_NUMBER":
                                def.Account1NextNumber = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "ACCOUNT_2_NEXT_NUMBER":
                                def.Account2NextNumber = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "TICKETING_KIOSK_MODE":
                                def.TicketingKioskMode = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "KIOSK_INACTIVE_PERIOD_TIMEOUT":
                                def.Kiosk_Inactive_Period_Timeout = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "KIOSK_COMPLETE_TIMEOUT":
                                def.KioskCompleteTimeout = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "FORCE_INACTIVE_SHUTDOWN":
                                def.ForceInactiveShutdown = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "KIOSK_SUPPLIER":
                                def.KioskSupplier = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "REDIRECT_ON_EMPTY_BASKET":
                                def.RedirectOnEmptyBasket = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "REDIRECT_ON_EMPTY_BASKET_URL":
                                def.RedirectOnEmptyBasketURL = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PDF_URL":
                                def.pdfUrl = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PARTNER_GROUP_TYPE":
                                def.PartnerGroupType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "SHOW_FROM_PRICES":
                                def.ShowFromPrices = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "REGISTRATION_BRANCH_SELECTION_TYPE":
                                def.RegistrationBranchSelectionType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "REGISTRATION_PRICELIST_SELECTION_TYPE":
                                def.RegistrationPricelistSelectionType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ENABLE_BANNER_CONTROL":
                                def.enableBannerControl = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ENABLE_PRICE_VIEW":
                                def.enablePriceView = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "PPS_ENABLE_1":
                                def.PPS_ENABLE_1 = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PPS_ENABLE_2":
                                def.PPS_ENABLE_2 = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PPS_MUST_TICK_FOR_NO_SCHEMES":
                                def.PPS_MUST_TICK_FOR_NO_SCHEMES = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_ENCRYPTION_KEY":
                                def.PAYMENT_ENCRYPTION_KEY = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "UPDATE_DETAILS_FOR_SEASON_TICKETS":
                                def.UpdateDetailsForSeasonTickets = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SHOW_PRICES_EXCLUDING_VAT":
                                def.ShowPricesExVAT = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "TRACKING_CODE_IN_USE":
                                def.TrackingCodeInUse = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "META_TAGS_IN_USE":
                                def.MetaTagsInUse = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case "COMPLETE_PARTIAL_ORDER":
                                def.CompletePartialOrder = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ENSURE_VALID_QUERYSTRING":
                                def.EnsureValidQueryString = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "BCT_IN_USE":
                                def.BctInUse = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "VAT_ROW_VISIBLE":
                                def.VatNoRowVisible = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "NOISE_ENCRYPTION_KEY":
                                def.NOISE_ENCRYPTION_KEY = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "NOISE_IN_USE":
                                if (HttpContext.Current.Request.Url.ToString().Contains("http://localhost"))
                                {
                                    def.NOISE_IN_USE = false;
                                }
                                else
                                {
                                    def.NOISE_IN_USE = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                }
                                break;
                            case  "NOISE_THRESHOLD_MINUTES":
                                def.NOISE_THRESHOLD_MINUTES = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "NOISE_MAX_CONCURRENT_USERS":
                                def.NOISE_MAX_CONCURRENT_USERS = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "NOISE_URL":
                                def.NOISE_URL = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "DEFAULT_COMPANY_CODE":
                                def.DEFAULT_COMPANY_CODE = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "DT_REPLACE_PRICE_LIST_DETAIL":
                                def.ReplacePriceListDetailDT = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]).ToString();
                                break;
                            case  "LOGOUT_PAGE":
                                def.LogoutPage = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "DATA_TRANSFER_BACKEND_DATABASE_TYPE":
                                def.DataTransfer_BackendDatabaseType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CACHE_DEPENDENCY_PATH":
                                def.CacheDependencyPath = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "APPLICATION_STARTUP_PAGE":
                                def.ApplicationStartupPage = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "SITE_NAVIGATION_TYPE":
                                def.SiteNavigationType = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "SHOW_REGISTRATION_FORM_ON_LOGIN_PAGE":
                                def.ShowRegistrationFormOnLoginPage = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "LOYALTY_POINTS_IN_USE":
                                def.Loyalty_Points_In_Use = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CREATE_FRONTEND_ORDER_RECORDS":
                                def.CREATE_FRONTEND_ORDER_RECORDS = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "DISPLAY_FREE_TICKETING_ITEMS":
                                def.DISPLAY_FREE_TICKETING_ITEMS = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_SKIP_PRE_AUTH":
                                def.PaymentSkipPreAuth = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "DEFAULT_TICKETING_EMAIL_ADDRESS":
                                def.DefaultTicketingEmailAddress = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "VALIDATE_USER_PROFILES_WHEN_LOGGED_IN":
                                def.ValidateUserProfilesWhenLoggedIn = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ECOMMERCE_LOGGING_IN_USE":
                                def.EcommerceLoggingInUse = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "STORE_COUNTRY_VALUE_AS_WHOLE_NAME":
                                def.StoreCountryAsWholeName = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case "IMAGE_NAME_REPLACE_SLASH":
                                def.ImageNameReplaceSlash = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "NOISE_SESSION_EXPIRES_AFTER_CHECKOUT":
                                def.NOISE_SESSION_EXPIRES_AFTER_CHECKOUT = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "LOG_USER_OUT_AFTER_CHECKOUT":
                                def.LOG_USER_OUT_AFTER_CHECKOUT = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES":
                                def.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "NOISE_CLEAR_SESSION_ON_LOGOUT":
                                def.NOISE_CLEAR_SESSION_ON_LOGOUT = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "NOISE_MAX_SESSION_CHECKOUT_ADD_MINUTES":
                                def.NOISE_MAX_SESSION_CHECKOUT_ADD_MINUTES = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "DT_UPDATE_PRODUCT_DESCRIPTIONS":
                                def.UpdateProductDescriptionsDT = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "DELIVERY_PRICE_CALCULATION_TYPE":
                                def.DeliveryPriceCalculationType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ORDER_PASS_REGISTRATION_ADDRESS":
                                def.OrderPassRegistrationAddress = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "IMAGE_NAME_REPLACE_DOUBLE_QUOTE":
                                def.ImageNameReplaceDoubleQuote = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "RETRIEVE_ALTERNATIVE_PRODUCTS_AT_CHECKOUT":
                                def.RetrieveAlternativeProductsAtCheckout = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALLOW_MASTER_PRODUCTS_TO_BE_PURCHASED":
                                def.AllowMasterProductsToBePurchased = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "FIRST_CHECKOUT_PAGE":
                                def.FirstCheckoutPage = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ORDER_ENQUIRY_SHOW_ORDER_SUMMARY":
                                def.OrderEnquiryShowOrderSummary = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "INVOICE_AMOUNT_INCLUDES_TAX":
                                def.InvoiceAmountIncludesTax = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SHOW_SELECTED_GROUP_AND_CHILDREN":
                                def.ShowSelectedGroupAndChildren = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALLOW_REBOOK_RETURNED_SEATS":
                                def.AllowRebookReturnedSeats = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "FRONT_END_STOCK_REQUIRED_TO_ADD_TO_BASKET":
                                def.FrontEndStockRequiredToAddToBasket = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case "SHOW_ONLY_MASTER_PRODUCTS_ON_SEARCH_RESULTS":
                                def.ShowOnlyMasterProductsOnSearchResults = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "MISSING_IMAGE_PATH":
                                def.MissingImagePath = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "USE_GROUP_LEVEL01_SHOW_PRODUCTS_AS_LIST":
                                def.Override_Show_Products_As_List_Using_L01_Value = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALLOW_AUTO_LOGIN":
                                def.AllowAutoLogin = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CREATE_SINGLE_SIGNON_COOKIES":
                                def.CreateSingleSignOnCookies = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SINGLE_SIGNON_SECRET_KEY":
                                def.SingleSignOnSecretKey = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "SINGLE_SIGNON_DOMAIN":
                                def.SingleSignOnDomain = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAGE_AFTER_LOGIN":
                                def.PAGE_AFTER_LOGIN = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "MARKETING_CAMPAIGNS_ACTIVE":
                                def.MARKETING_CAMPAIGNS_ACTIVE = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "MARKETING_CAMPAIGNS_ACTIVE_EBUSINESS":
                                def.MARKETING_CAMPAIGNS_ACTIVE_EBUSINESS = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case "REDIRECT_AFTER_AGENT_LOGIN_URL":
                                def.REDIRECT_AFTER_AGENT_LOGIN_URL = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "REDIRECT_AFTER_AGENT_LOGOUT_URL":
                                def.REDIRECT_AFTER_AGENT_LOGOUT_URL = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "AGENT_MODE_IGNORE_FREE_PRODUCT_FLAG":
                                def.AGENT_MODE_IGNORE_FREE_PRODUCT_FLAG = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ORDER_ENQUIRY_SHOW_PARTNER_ORDERS":
                                def.ORDER_ENQUIRY_SHOW_PARTNER_ORDERS = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]).ToString();
                                break;
                            case  "IGNORE_FF_FOR_ST_RENEWALS":
                                def.IgnoreFFforSTRenewals = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "MINIMUM_PURCHASE_QUANTITY":
                                def.MinimumPurchaseQuantity = businessObjects.Data.Validation.CheckForDBNull_Decimal(defRow["VALUE"]);
                                break;
                            case  "MINIMUM_PURCHASE_AMOUNT":
                                def.MinimumPurchaseAmount = businessObjects.Data.Validation.CheckForDBNull_Decimal(defRow["VALUE"]);
                                break;
                            case  "USE_MINIMUM_PURCHASE_QUANTITY":
                                def.UseMinimumPurchaseQuantity = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "USE_MINIMUM_PURCHASE_AMOUNT":
                                def.UseMinimumPurchaseAmount = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "OPTION_PRICE_FROM_MASTER_PRODUCT":
                                def.UseOptionPriceFromMasterProduct = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PRICING_TYPE":
                                def.PricingType = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "DELIVERY_CALCULATION_IN_USE":
                                def.DeliveryCalculationInUse = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "LEFT_PRODUCT_NAV_TREEVIEW_EXPAND_DEPTH":
                                def.Menu_LeftProductNav_TreeExpandDepth = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "SUPPRESS_PRODUCT_LINKS":
                                def.SuppressProductLinks = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CREDIT_CHECK_ON_LOGIN":
                                def.CREDIT_CHECK_ON_LOGIN = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "DISPLAY_PAGE_BEFORE_CHECKOUT":
                                def.DISPLAY_PAGE_BEFORE_CHECKOUT = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAGE_BEFORE_CHECKOUT":
                                def.PAGE_BEFORE_CHECKOUT = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "GLOBAL_DATE_FORMAT":
                                def.GlobalDateFormat = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ORDER_TEMPLATES_PER_PARTNER":
                                def.OrderTemplatesPerPartner = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "ORDER_TEMPLATE_LAYOUT_TYPE":
                                def.OrderTemplateLayoutType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "POSTCODE_FORMAT":
                                def.PostCodeFormat = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PHONENO_FORMAT":
                                def.PhoneNoFormat = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ADVANCED_SEARCH_TYPE":
                                def.AdvancedSearchType = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "DELIVERY_DATE_TYPE":
                                def.DeliveryDateType = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "DEFAULT_MEMBERSHIP_QUANTITY":
                                def.DefaultMembershipQuantity = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "AUTO_ENROL_PPS_ON_PAYMENT":
                                def.AutoEnrolPPSOnPayment = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "SEAT_DISPLAY":
                                def.SeatDisplay = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "SHOW_PRODUCT_ENTRY_TIME":
                                def.ShowProductEntryTime = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "REGISTRATION_REMOVE_BLANKS_FROM_TELNO":
                                def.RegistrationRemoveBlanksFromTelNo = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SHOW_USER_ID_FIELDS":
                                def.ShowUserIDFields = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "AGE_AT_WHICH_ID_FIELDS_ARE_MANDATORY":
                                def.AgeAtWhichIDFieldsAreMandatory = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "PHOTOCAPTURE_URL":
                                def.PhotoCaptureUrl = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "SHOW_PASSPORT_FIELD":
                                def.ShowPassportField = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "SHOW_PIN_FIELD":
                                def.ShowPinField = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "SHOW_GREENCARD_FIELD":
                                def.ShowGreenCardField = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "SHOW_USER_ID4_FIELD":
                                def.ShowUserID4Field = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "SHOW_USER_ID5_FIELD":
                                def.ShowUserID5Field = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "SHOW_USER_ID6_FIELD":
                                def.ShowUserID6Field = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "SHOW_USER_ID7_FIELD":
                                def.ShowUserID7Field = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "SHOW_USER_ID8_FIELD":
                                def.ShowUserID8Field = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "SHOW_USER_ID9_FIELD":
                                def.ShowUserID9Field = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "JAVA_SCRIPT_PATH":
                                def.JavaScriptPath = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "JAVA_SCRIPT_VERSION":
                                def.JavaScriptVersion = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PERFORM_DISCONTINUED_PRODUCT_CHECK":
                                def.PerformDiscontinuedProductCheck = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SERVE_ALL_PAGES_SECURE":
                                def.ServeAllPagesSecure = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "TRADINGPORTALTICKET":
                                def.TradingPortalTicket = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "TRADEPLACE_ENCRYPTION_KEY":
                                def.TradePlaceEncryptionKey = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "REDIRECT_AFTER_AUTO_LOGIN":
                                def.RedirectAfterAutoLogin = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "AUTHORITY_USER_PROFILE":
                                def.AuthorityUserProfile = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PERFORM_AGENT_WATCH_LIST_CHECK":
                                def.PerformAgentWatchListCheck = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "DELIVERY_LEAD_TIME":
                                def.DeliveryLeadTime = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "DELIVERY_LEAD_TIME_HOME":
                                def.DeliveryLeadTimeHome = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CALCULATE_STOCK_LEAD_TIME":
                                def.CalculateStockLeadTime = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "DELIVERY_CUT_OFF_TIME":
                                def.DeliveryCutOffTime = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAGE_RESTRICTIONS_IN_USE":
                                def.PageRestrictionsInUse = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALLOW_CHECKOUT_WHEN_BACKEND_UNAVAILABLE":
                                def.AllowCheckoutWhenBackEndUnavailable = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SEND_EMAIL_AS_HTML":
                                def.SendEmailAsHTML = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CUSTOMER_SEARCH_MODE":
                                def.CustomerSearchMode = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "AGENT_PORTAL_LINK_ON_SESSION_ERROR_PAGE":
                                def.AgentPortalLinkOnSessionErrorPage = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SET_BACKEND_ACCOUNT_NO_FROM_USER_DETAILS":
                                def.SetBackendAccountNoFromUserDetails = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultTrue(defRow["VALUE"]).ToString();
                                break;
                            case  "PAGE_EXTRA_DATA":
                                def.UsePageExtraDataTable = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "USE_ENCRYPTED_PASSWORD":
                                def.UseEncryptedPassword = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case "FORGOTTEN_PASSWORD_EXPIRY_TIME":
                                def.ForgottenPasswordExpiryTime = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "CORPORATE_STADIUM":
                                def.CorporateStadium = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "HOSPITALITY_FORWARD_TO_BASKET":
                                def.HospitalityForwardToBasket = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "HIDE_OPTIONS_WITHOUT_PRICE":
                                def.HideOptionsWithoutPrice = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PARTICIPANTS_DEFAULT_ADDRESS_IN_USE":
                                def.ParticipantsDefaultAddressInUse = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "DEFAULT_ADDRESS_LINE_1":
                                def.DefaultAddressLine1 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "DEFAULT_ADDRESS_LINE_2":
                                def.DefaultAddressLine2 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "DEFAULT_CITY":
                                def.DefaultCity = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "DEFAULT_COUNTY":
                                def.DefaultCounty = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "DEFAULT_COUNTRY":
                                def.DefaultCountry = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "DEFAULT_POSTCODE":
                                def.DefaultPostcode = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "COURSE_STADIUM":
                                def.CourseStadium = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "COURSE_PRODUCT_MAX_QUANTITY":
                                def.CourseProductMaxQuantity = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "BYPASS_DIRECT_DEBIT_PAGE_WHEN_BASKET_INVALID":
                                def.BypassDirectDebitPageWhenMixedBasket = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CHECK_USER_CAN_ACCESS_F_AND_F":
                                def.CheckUserCanAccessFandF = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_PASS_IF_ENROLLED_CODE_U":
                                def.Payment3dSecurePassIfEnrolledCodeU = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_PASS_IF_RESPONSE_TIMEOUT":
                                def.Payment3dSecurePassIfResponseTimeout = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "AMEND_PPS_ENROLMENT_IGNORE_FF":
                                def.AmendPPSEnrolmentIgnoreFF = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CREDIT_FINANCE_COMPANY_CODE":
                                def.CreditFinanceCompanyCode = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "WEB_ORDER_NUMBER_PREFIX_OVERRIDE":
                                def.WebOrderNumberPrefixOverride = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "VALIDATE_PARTNER_DIRECT_ACCESS":
                                def.ValidatePartnerDirectAccess = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SAP_OCI_PARTNER":
                                def.SapOciPartner = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "USE_EPOS_OPTIONS":
                                def.UseEPOSOptions = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "LOGIN_WHEN_ADDING_HOME_GAME_TO_BASKET":
                                def.LoginWhenAddingHomeGameToBasket = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALLOW_ADDITIONAL_PROMOTION_WITH_FREE_DEL":
                                def.AllowAdditionalPromoWithFreeDel = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "USE_PRE_PROMOTION_VALUE_FOR_FREE_DEL_CAL":
                                def.UsePrePromoValueForFreeDelCal = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PREFERRED_DELIVERY_DATE_MAX_DAYS":
                                def.PreferredDeliveryDateMaxDays = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "CASHBACK_FEE_CODE":
                                def.CashBackFeeCode = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PROFILE_PHOTO_PERMANENT_PATH":
                                def.ProfilePhotoPermanentPath = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case "IMAGE_UPLOAD_TEMP_PATH":
                                def.ImageUploadTempPath = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PROFILE_PHOTO_PERMANENT_PATH_PHYSICAL":
                                def.ProfilePhotoPermanentPathPhysical = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "IMAGE_UPLOAD_TEMP_PATH_PHYSICAL":
                                def.ImageUploadTempPathPhysical = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "MAINTENANCE_START_TIME":
                                def.MaintenanceStartTime = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "MAINTENANCE_END_TIME":
                                def.MaintenanceEndTime = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "FAVOURITE_SEAT_FUNCTION":
                                def.FavouriteSeatFunction = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALLOW_REASSIGNMENT_OF_RESERVED_SEATS":
                                def.AllowReassignmentOfReservedSeats = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]).ToString();
                                break;
                            case  "ALERTS_ENABLED":
                                def.AlertsEnabled = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALERTS_REFRESH_ATTRIBUTES_AT_LOGIN":
                                def.AlertsRefreshAttributesAtLogin = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALERTS_GENERATE_AT_LOGIN":
                                def.AlertsGenerateAtLogin = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALERTS_CC_EXPIRY_PPS_WARN_PERIOD":
                                def.AlertsCCExpiryPPSWarnPeriod = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "ALERTS_CC_EXPIRY_SAV_WARN_PERIOD":
                                def.AlertsCCExpirySAVWarnPeriod = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "ALLOW_CHECKOUT_WITH_MIXED_BASKET":
                                def.AllowCheckoutWithMixedBasket = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "USE_SAVE_MY_CARD":
                                def.UseSaveMyCard = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ONACCOUNT_ENABLED":
                                def.OnAccountEnabled = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALLOW_MULTIPLE_PRODUCTS_IN_DD_CHECKOUT":
                                def.AllowMultipleProductsInDDCheckout = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultTrue(defRow["VALUE"]);
                                break;
                            case  "USE_GLOBAL_PRICELIST_WITH_CUSTOMER_PRICELIST":
                                def.UseGlobalPriceListWithCustomerPriceList = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CUSTOMER_SERVICES_EMAIL_RETAIL":
                                def.CustomerServicesEmailRetail = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_PROCESS_3D_SECURE_FOR_AGENTS":
                                def.PaymentProcess3dSecureForAgents = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]).ToString();
                                break;
                            case  "PRICE_AND_AREA_SELECTION_ENABLED":
                                def.PriceAndAreaSelection = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case "PRICE_AND_AREA_SELECTION_DESCENDING_ORDER":
                                def.PriceAndAreaSelectionDescendingOrder = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_HSBC_PASS_WEB_ORDER_NO":
                                def.PaymentHSBCPassWebOrderNo = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALLOW_CAT_TICKETS_BY_AGENT":
                                def.AllowCATTicketsByAgent = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALLOW_CAT_TICKETS_BY_USER":
                                def.AllowCATTicketsByUser = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "STATUS_CHECK_ON_LOGIN":
                                def.StatusCheckOnLogin = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ON_ACCOUNT_MODULE_ACTIVE":
                                def.OnAccountModuleActive = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SHOW_FF_ORDER_TEMPLATES":
                                def.ShowFFOrderTemplates = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "WHOLE_SITE_IS_IN_AGENT_MODE":
                                def.WholeSiteIsInAgentMode = businessObjects.Data.Validation.CheckForDBNullOrBlank_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "MIN_AGE_FOR_CREDIT_FINANCE":
                                def.minageforcreditfinance = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "NON_ENGLISH_MONTHS":
                                def.NonEnglishMonths = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_GATEWAY_EXTERNAL":
                                def.PaymentGatewayExternal = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "REMEMBER_ME_FUNCTION":
                                def.RememberMeFunction = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "REMEMBER_ME_COOKIE_NAME":
                                def.RememberMeCookieName = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "REMEMBER_ME_COOKIE_ENCODE_KEY":
                                def.RememberMeCookieEncodeKey = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "REMEMBER_ME_COOKIE_EXPIRY_DAYS":
                                def.RememberMeCookieExiryDays = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "QUEUE_DB_DIRECT_ACCESS_ALLOWED":
                                def.QueueDBDirectAccessAllowed = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_DEFAULT_ECI":
                                def.Payment3DSecureDefaultECI = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAYMENT_3DSECURE_DEFAULT_ATSDATA":
                                def.Payment3DSecureDefaultATSData = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "EPURSE_TOP_UP_PRODUCT_CODE":
                                def.EPurseTopUpProductCode = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "PAGE_RESTRICTED_ALERT_ERROR_PAGE_URL":
                                def.PageRestrictedAlertErrorPageURL = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "STOP_CODE_FOR_ID3":
                                def.StopCodeForID3 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "USER_ID_VALIDATION_TYPE":
                                def.UserIDValidationTypeforID3 = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ADDRESS_FORMAT":
                                def.AddressFormat = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CHECK_ADDRESS_IS_VALID_ON_LOGIN":
                                def.CheckAddressIsValidOnLogin = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CHANGE_BUSINESS_UNIT_ALLOWED":
                                def.ChangeBusinessUnitAllowed = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "USE_TICKETING_DELIVERY_ADDRESSES":
                                def.UseTicketingDeliveryAddresses = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "USE_RETAIL_DELIVERY_ADDRESSES":
                                def.UseRetailDeliveryAddresses = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALLOW_PUBLIC_TICKETING_DELIVERY_ADDRESS_SELECTION":
                                def.AllowPublicTicketingDeliveryAddressSelection = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "ALLOW_AGENT_PREFERENCES_UPDATES":
                                def.AllowAgentPreferencesUpdates = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "CHECKOUT_PROMOTION_TYPE":
                                def.CheckoutPromotionType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]).Trim().ToUpper();
                                break;
                            case  "SHOW_TRAVEL_AS_DDL":
                                def.ShowTravelAsDDL = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SHOW_UNIQUE_MEMBERSHIPS":
                                def.ShowUniqueMemberships = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "RETAIL_MISSING_IMAGE_PATH":
                                def.RetailMissingImagePath = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "RETRIEVE_PART_PAYMENTS":
                                def.RetrievePartPayments = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "AGENT_LEVEL_CACHE_FOR_PRODUCT_STADIUM_AVAILABILITY":
                                def.AgentLevelCacheForProductStadiumAvailability = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "TALENT_ADMIN_CLIENT_NAME":
                                def.TalentAdminClientName = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "MEMBERSHIPS_PRODUCT_SUB_TYPE":
                                def.MembershipsProductSubType = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "FEE_CATEGORIES_NOT_INCLUDED_IN_ECOMMERCE_TRACKING":
                                def.FeeCategoriesNotIncludedInECommerceTracking = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "CASHBACK_REWARDS_ACTIVE":
                                def.CashbackRewardsActive = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "BULK_SALES_MODE_BASKET_LIMIT":
                                def.BulkSalesModeBasketLimit = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "WEBPRICES_MODIFYING_MODE":
                                def.WebPricesModifyingMode = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "ALLOW_MANUAL_INTERVENTION":
                                def.AllowManualIntervention = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "USERDEFINED_PAGE_QUERYKEYS":
                                def.UserDefinedPageQueryKeys = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "SAVED_SEARCH_ENABLED":
                                def.SavedSearchEnabled = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SAVED_SEARCH_LIMIT":
                                def.SavedSearchLimit = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "USE_SCAN_NOW_FOR_PRINT_NOW_FULFILMENT":
                                def.UseScanNowForPrintNowFulfilment = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SHOW_AGENT_DEPARTMENT":
                                def.ShowAgentDepartment = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "COURIER_TRACKING_REFERENCES":
                                  def.CourierTrackingReferences = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "RETAIL_PRODUCT_CODE":
                                def.RetailProductCode = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "GET_CUSTOMER_TITLES_FROM_ISERIES":
                                def.GetCustomerTitlesFromIseries = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "NOTIFICATIONS_ON_CONFIRM_PAGE":
                                def.NotificationsOnConfirmPage = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "MASTER_PAGE_FOLDER":
                                def.MasterPageFolder = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "ISTESTORLIVE":
                                def.IsTestOrLive = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case  "NO_OF_RECENTLY_SEARCHED_CUSTOMERS":
                                def.NoOfRecentlyUsedCustomers = businessObjects.Data.Validation.CheckForDBNull_Int(defRow["VALUE"]);
                                break;
                            case  "PROFILE_FULL_NAME_WITH_TITLE":
                                def.ProfileFullNameWithTitle = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case  "SYSTEM_DEFAULTS_URL":
                                def.SystemDefaultsUrl = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case "SET_RETAIL_ORDER_TO_PENDING":
                                def.SetRetailOrderToPending = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case "TALENT_API_ADDRESS":
                                def.TalentAPIAddress = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
							case "STATIC_CONTENT_QUERYSTRING_VALUE":
                                def.StaticContentQueryStringValue = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);								
                                break;
							case "CLIENT_SALT":
                                def.SaltString = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case "ENCRYPTED_PASSWORD_PLACEHOLDER":
                                def.EncryptedPasswordPlaceholder = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case "SHOW_PRODUCT_TEXT_ON_VISUAL_SEATSELECTION":
                                def.ShowProductTextOnVisualSeatSelection = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case "SHOW_BUNDLE_DATE_AS_RANGE":
                                def.ShowBundleDateAsRange = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break; 
                            case "SHOW_PROGRESS_BAR":
                                def.ShowProgressBar = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case "PROGRESS_BAR_SUBTYPES":
                                def.ProgressBarSubtypes = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case "SHOW_TE_PROGRESS_BAR":
                                def.ShowTicketExchangeProgressBar = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case "DEFAULT_STAND_AREA_CLICK":
                                def.DefaultStandAndAreaClick = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case "PAYMENT_PAYPAL_ACCOUNT_ID":
                                def.PaymentPayPalAccountID = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case "PAYMENT_PAYPAL_ENVIRONMENT":
                                def.PaymentPayPalEnvironment = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case "PRODUCT_MAINIMAGE_PATH_RELATIVE":
                                def.ProductMainImagePathRelative = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case "PRODUCT_MAGICZOOM_PATH_RELATIVE":
                                def.ProductMagicZoomImagePathRelative = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case "DEFAULT_DELIVERY_ZONE_CODE":
                                def.DefaultDeliveryZoneCode = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case "DISCOUNTIF":
                                def.DiscountIF = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case "DISCOUNTIFPSK":
                                def.DiscountIFPSK = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case "HIGHLIGHT_LOGGED_IN_CUSTOMER_PRICE_BAND":
                                def.HighlightLoggedInCustomerPriceBand = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["VALUE"]);
                                break;
                            case "HOSP_MERGED_DOCUMENT_PATH_RELATIVE":
                                def.HospMergedDocumentPathRelative = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                            case "HOSPITALITY_PDF_ATTACHMENTS_ON_CONFIRMATION_EMAIL":
                                def.HospitalityPDFAttachmentsOnConfirmationEmail = businessObjects.Data.Validation.CheckForDBNull_String(defRow["VALUE"]);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

            }
            catch (Exception)
            {
            }
            return def;
        }

    }

}