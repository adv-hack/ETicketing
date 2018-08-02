﻿Public Class DEEcommerceModuleDefaults

    ''' <summary>
    ''' Nested class for returning and storing in cache
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
 Public Class DefaultValues

#Region "Properties"

        Public Property Culture() As String
        Public Property StatementHeaderTemplate() As String
        Public Property InvoiceDetailTemplate() As String
        Public Property InvoiceHeaderTemplate() As String
        Public Property CreditNoteDetailTemplate() As String
        Public Property CreditNoteHeaderTemplate() As String
        Public Property Default_Add_Quantity() As Decimal
        Public Property Min_Add_Quantity() As Decimal
        Public Property NumberOfGroupLevels() As Integer
        Public Property ConfirmationEmail() As Boolean
        Public Property CustomerDestinationDatabase() As String
        Public Property CustomerNumberPrefix() As String
        Public Property DefaultBu() As Boolean
        Public Property DeliveryCostPerc() As Decimal
        Public Property DeliveryRoundingMask() As String
        Public Property HtmlPathAbsolute() As String
        Public Property HtmlPathVirtual() As String
        Public Property HtmlPerPage() As Boolean
        Public Property HtmlPerPageType() As String
        Public Property ImagePathAbsolute() As String
        Public Property ImagePathVirtual() As String
        Public Property ImageSslPathVirtual() As String
        Public Property GlobalImagePath() As String
        Public Property GlobalPathVirtual() As String
        Public Property GlobalSslPathVirtual() As String
        Public Property OrderDestinationDatabase() As String
        Public Property OrderNumber() As Int32
        Public Property OrderNumberPrefix() As String
        Public Property PartnerNumber() As Int32
        Public Property PaymentGatewayType() As String
        Public Property PaymentDetails1() As String
        Public Property PaymentDetails2() As String
        Public Property PaymentDetails3() As String
        Public Property PaymentDetails4() As String
        Public Property PaymentDetails5() As String
        Public Property PaymentURL1() As String
        Public Property PaymentURL2() As String
        Public Property PaymentURL3() As String
        Public Property PaymentRejectAddressAVS() As Boolean
        Public Property PaymentRejectPostcodeAVS() As Boolean
        Public Property PaymentRejectCSC() As Boolean
        Public Property PaymentAllowPartialAVS() As Boolean
        Public Property PaymentDebug() As Boolean
        Public Property PaymentCallBankAPI() As Boolean
        Public Property Payment3DSecureIFrameWidth() As String
        Public Property Payment3DSecureProvider() As String
        Public Property Payment3DSecureSuccessCodes() As Array
        Public Property Payment3DSecureDetails1() As String
        Public Property Payment3DSecureDetails2() As String
        Public Property Payment3DSecureDetails3() As String
        Public Property Payment3DSecureDetails4() As String
        Public Property Payment3DSecureDetails5() As String
        Public Property Payment3DSecureDetails6() As String
        Public Property Payment3DSecureDetails7() As String
        Public Property Payment3DSecureDetails8() As String
        Public Property Payment3DSecureDetails9() As String
        Public Property Payment3DSecureDetails10() As String
        Public Property Payment3DSecureURL1() As String
        Public Property Payment3DSecureURL2() As String
        Public Property PriceList() As String
        Public Property StockLocation() As String
        Public Property StoredProcedureGroup() As String
        Public Property TempOrderNumber() As Int32
        Public Property Theme() As String
        Public Property UserNumber() As Int32
        Public Property Use_Age_Check() As Boolean
        Public Property ProductHTML() As Boolean
        Public Property ProductHTMLType() As String
        Public Property RegistrationConfirmationFromEmail() As String
        Public Property OrdersFromEmail() As String
        Public Property ForgottenPasswordFromEmail() As String
        Public Property ContactUsToEmailCC() As String
        Public Property ContactUsToEmail() As String
        Public Property ContactUsConfirmationFromEmail() As String
        Public Property CrmBranch() As String
        Public Property OrderCarrierCode() As String
        Public Property OrderCreationRetry() As Boolean
        Public Property OrderCreationRetryAttempts() As Integer
        Public Property OrderCreationRetryWait() As Integer
        Public Property OrderCreationRetryErrors() As String
        Public Property StockCheckRetry() As Boolean
        Public Property StockCheckRetryAttempts() As Integer
        Public Property StockCheckRetryWait() As Integer
        Public Property StockCheckRetryErrors() As String
        Public Property StockCheckIgnoreErrors() As String
        Public Property RegistrationRetry() As Boolean
        Public Property RegistrationRetryAttempts() As Integer
        Public Property RegistrationRetryWait() As Integer
        Public Property RegistrationRetryErrors() As String
        Public Property LoginToProductBrowse() As String
        Public Property PagesLoginPath() As String
        Public Property RegistrationEnabled() As String
        Public Property RegistrationTemplate() As String
        Public Property UpdateDetailsTemplate() As String
        Public Property PerformCreditCheck() As Boolean
        Public Property DisableCheckoutIfOverCreditLimit() As Boolean
        Public Property EmailToAddress_OrderCreditLimitExceeded() As String
        Public Property EmailFromAddress_OrderCreditLimitExceeded() As String
        Public Property SendEmail_IfOrderCreditLimitExceeded() As Boolean
        Public Property OrderTemplates() As Boolean
        Public Property SaveOrders() As Boolean
        Public Property SendRegistrationToBackEnd() As Boolean
        Public Property UseLoginLookup() As Boolean
        Public Property LoginLookupType() As String
        Public Property PromotionPriority() As String
        Public Property ProfileSexMandatory() As Boolean
        Public Property SendOrderConfToCustServ() As Boolean
        Public Property CustomerServicesEmail() As String
        Public Property ProductTemplateType() As String
        Public Property PaymentProcess3dSecure() As Boolean
        Public Property GiftMessageEmail_From() As String
        Public Property GiftMessageEmail_To() As String
        Public Property ZoomifyFlashFileURL() As String
        Public Property ZoomifyWidth() As Integer
        Public Property ZoomifyHeight() As Integer
        Public Property ZoomifyInUse() As Boolean
        Public Property AdditionalSearchType() As String
        Public Property AdditionalSearchTypeCriteria() As String
        Public Property Call_Tax_WebService() As Boolean
        Public Property Show_Login_Control_On_Reminder_Page() As Boolean
        Public Property Show_Mini_Login_Control() As Boolean
        Public Property Show_Gift_Message_Option() As Boolean
        Public Property HtmlIncludePathRelative() As String
        Public Property HideUnselectedTopLevelGroups() As Boolean
        Public Property HideAllTopLevelGroups() As Boolean
        Public Property ShowSelectedGroupAndChildren() As Boolean
        Public Property UserNumberPrefix() As String
        Public Property Override_Show_Children_As_Groups_Using_L01_Value() As Boolean
        Public Property Perform_Back_End_Stock_Check() As Boolean
        Public Property Perform_Front_End_Stock_Check() As Boolean
        Public Property AddressLine1RowVisible() As Boolean
        Public Property AddressLine2RowVisible() As Boolean
        Public Property AddressLine3RowVisible() As Boolean
        Public Property AddressLine4RowVisible() As Boolean
        Public Property AddressLine5RowVisible() As Boolean
        Public Property AddressPostcodeRowVisible() As Boolean
        Public Property AddressCountryRowVisible() As Boolean
        Public Property Search_Results_Display_Type() As Integer
        Public Property Order_Enquiry_Template_Type() As String
        Public Property Product_List_Graphical_Template_Type() As Integer
        Public Property ProductRelationshipsTemplateType() As Integer
        Public Property ValidateExternalPassword() As Boolean
        Public Property ExternalPasswordValue() As String
        Public Property ExternalPasswordIsMaster() As Boolean
        Public Property SendRegistrationToBackendFirst() As Boolean
        Public Property AllowDuplicateEmail() As Boolean
        Public Property LoginidIsCustomerNumber() As Boolean
        Public Property AddCustomerToMyFriendsAndFamily() As Boolean
        Public Property AddMeToTheirMyFriendsAndFamily() As Boolean
        Public Property RegisterCustomerForFriendsAndFamily() As Boolean
        Public Property FriendsAndFamily() As Boolean
        Public Property UseDefaultCountryOnDeliveryAddress() As Boolean
        Public Property UseDefaultCountryOnRegistration() As Boolean
        Public Property HomeProduct_MaxBuy() As Decimal
        Public Property AwayProduct_MaxBuy() As Decimal
        Public Property TravelProduct_MaxBuy() As Decimal
        Public Property MembershipProduct_MaxBuy() As Decimal
        Public Property HomeProduct_ForwardToBasket() As Boolean
        Public Property AwayProduct_ForwardToBasket() As Boolean
        Public Property TravelProduct_ForwardToBasket() As Boolean
        Public Property EventProduct_ForwardToBasket() As Boolean
        Public Property MembershipProduct_ForwardToBasket() As Boolean
        Public Property PaymentBillingAddress() As Boolean
        Public Property PaymentOrderSummary() As Boolean
        Public Property TicketingCheckout() As Boolean
        Public Property ClearBackEndBasketOnLogin() As Boolean
        Public Property DisplayAccountSelectionOnLogin() As Boolean
        Public Property Pdf_Path_Absolute() As String
        Public Property AllowCheckoutWhenNoStock() As Boolean
        Public Property ZoomifyDisplayNavValue() As Integer
        Public Property CapacityByStadium() As Boolean
        Public Property Override_Show_Products_Display_Using_L01_Value() As Boolean
        Public Property AddressingLayout() As String
        Public Property AddressingFields() As String
        Public Property AddressingMapCompanyName() As String
        Public Property AddressingMapOrganisation() As String
        Public Property AddressingMapAdr1() As String
        Public Property AddressingMapAdr2() As String
        Public Property AddressingMapAdr3() As String
        Public Property AddressingMapAdr4() As String
        Public Property AddressingMapAdr5() As String
        Public Property AddressingMapPost() As String
        Public Property AddressingMapCountry() As String
        Public Property AddressingProvider() As String
        Public Property ShowActivateAccountOnLoginPage() As Boolean
        Public Property ShowRegisterAccountOnLoginPage() As Boolean
        Public Property RefreshCustomerDataOnLogin() As Boolean
        Public Property LoginidType() As String
        Public Property RestrictUserPaymentType() As Boolean
        Public Property UseVisualSeatSelection() As Boolean
        Public Property CalculateDeliveryDate() As Boolean
        Public Property ContactUsFormShouldAlwaysEmail() As Boolean
        Public Property TicketingStadium() As String
        Public Property AutoCreateAccountNo() As Boolean
        Public Property Account1Prefix() As String
        Public Property Account2Prefix() As String
        Public Property Account1Length() As Integer
        Public Property Account2Length() As Integer
        Public Property Account1NextNumber() As Int32
        Public Property Account2NextNumber() As Int32
        Public Property TicketingKioskMode() As Boolean
        Public Property Kiosk_Inactive_Period_Timeout() As Integer
        Public Property KioskCompleteTimeout() As Integer
        Public Property ForceInactiveShutdown() As Boolean
        Public Property KioskSupplier() As String
        Public Property RedirectOnEmptyBasket() As Boolean
        Public Property RedirectOnEmptyBasketURL() As String
        Public Property pdfUrl() As String
        Public Property PartnerGroupType() As String
        Public Property ShowFromPrices() As Boolean
        Public Property RegistrationPricelistSelectionType() As String
        Public Property RegistrationBranchSelectionType() As String
        Public Property enableBannerControl() As Boolean
        Public Property enablePriceView() As Boolean
        Public Property PPS_ENABLE_1() As Boolean
        Public Property PPS_ENABLE_2() As Boolean
        Public Property PPS_MUST_TICK_FOR_NO_SCHEMES() As Boolean
        Public Property PAYMENT_ENCRYPTION_KEY() As String
        Public Property UpdateDetailsForSeasonTickets() As Boolean
        Public Property ShowPricesExVAT() As Boolean
        Public Property TrackingCodeInUse() As Boolean
        Public Property MetaTagsInUse() As Boolean
        Public Property CompletePartialOrder() As Boolean
        Public Property EnsureValidQueryString() As Boolean
        Public Property BctInUse() As Boolean
        Public Property VatNoRowVisible() As Boolean
        Public Property NOISE_ENCRYPTION_KEY() As String
        Public Property NOISE_IN_USE() As Boolean
        Public Property NOISE_THRESHOLD_MINUTES() As Integer
        Public Property NOISE_MAX_CONCURRENT_USERS() As Integer
        Public Property NOISE_URL() As String
        Public Property DEFAULT_COMPANY_CODE() As String
        Public Property ReplacePriceListDetailDT() As String
        Public Property LogoutPage() As String
        Public Property DataTransfer_BackendDatabaseType() As String
        Public Property CacheDependencyPath() As String
        Public Property ApplicationStartupPage() As String
        Public Property SiteNavigationType() As Integer
        Public Property ShowRegistrationFormOnLoginPage() As Boolean
        Public Property DefaultPriceList() As String
        Public Property Loyalty_Points_In_Use() As Boolean
        Public Property CREATE_FRONTEND_ORDER_RECORDS() As Boolean
        Public Property DISPLAY_FREE_TICKETING_ITEMS() As Boolean
        Public Property PaymentSkipPreAuth() As Boolean
        Public Property DefaultTicketingEmailAddress() As String
        Public Property ValidateUserProfilesWhenLoggedIn() As Boolean
        Public Property EcommerceLoggingInUse() As Boolean
        Public Property StoreCountryAsWholeName() As Boolean
        Public Property ImageNameReplaceSlash() As String
        Public Property LOG_USER_OUT_AFTER_CHECKOUT() As Boolean
        Public Property NOISE_SESSION_EXPIRES_AFTER_CHECKOUT() As Boolean
        Public Property NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES() As Integer
        Public Property NOISE_MAX_SESSION_CHECKOUT_ADD_MINUTES() As Integer
        Public Property NOISE_CLEAR_SESSION_ON_LOGOUT() As Boolean
        Public Property UpdateProductDescriptionsDT() As Boolean
        Public Property DeliveryPriceCalculationType() As String
        Public Property OrderPassRegistrationAddress() As Boolean
        Public Property RetrieveAlternativeProductsAtCheckout() As Boolean
        Public Property DeliveryDateType() As Integer
        Public Property FirstCheckoutPage() As String
        Public Property OrderEnquiryShowOrderSummary() As Boolean
        Public Property AllowMasterProductsToBePurchased() As Boolean
        Public Property InvoiceAmountIncludesTax() As Boolean
        Public Property AllowRebookReturnedSeats() As Boolean
        Public Property FrontEndStockRequiredToAddToBasket() As Boolean
        Public Property ShowOnlyMasterProductsOnSearchResults() As Boolean
        Public Property MissingImagePath() As String
        Public Property Override_Show_Products_As_List_Using_L01_Value() As Boolean
        Public Property AllowAutoLogin() As Boolean
        Public Property CreateSingleSignOnCookies() As Boolean
        Public Property SingleSignOnSecretKey() As String
        Public Property SingleSignOnDomain() As String
        Public Property PAGE_AFTER_LOGIN() As String
        Public Property MARKETING_CAMPAIGNS_ACTIVE() As Boolean
        Public Property MARKETING_CAMPAIGNS_ACTIVE_EBUSINESS() As Boolean
        Public Property ORDER_ENQUIRY_SHOW_PARTNER_ORDERS() As String
        Public Property REDIRECT_AFTER_AGENT_LOGIN_URL() As String
        Public Property REDIRECT_AFTER_AGENT_LOGOUT_URL() As String
        Public Property AGENT_MODE_IGNORE_FREE_PRODUCT_FLAG() As Boolean
        Public Property IgnoreFFforSTRenewals() As Boolean
        Public Property MinimumPurchaseQuantity() As Decimal
        Public Property MinimumPurchaseAmount() As Decimal
        Public Property UseMinimumPurchaseAmount() As Boolean
        Public Property UseMinimumPurchaseQuantity() As Boolean
        Public Property UseOptionPriceFromMasterProduct() As Boolean
        Public Property PricingType() As Integer
        Public Property DeliveryCalculationInUse() As Boolean
        Public Property Menu_LeftProductNav_TreeExpandDepth() As Integer
        Public Property SuppressProductLinks() As Boolean
        Public Property PAGE_BEFORE_CHECKOUT() As String
        Public Property DISPLAY_PAGE_BEFORE_CHECKOUT() As Boolean
        Public Property CREDIT_CHECK_ON_LOGIN() As Boolean
        Public Property GlobalDateFormat() As String
        Public Property OrderTemplatesPerPartner() As Boolean
        Public Property OrderTemplateLayoutType() As String
        Public Property DataTransfer_CreatePartnerOnAddCustomer() As Boolean
        Public Property PostCodeFormat() As String
        Public Property PhoneNoFormat() As String
        Public Property ShowUserIDFields() As Boolean
        Public Property ShowPassportField() As Boolean
        Public Property ShowPinField() As Boolean
        Public Property ShowGreenCardField() As Boolean
        Public Property ShowUserID4Field() As Boolean
        Public Property ShowUserID5Field() As Boolean
        Public Property ShowUserID6Field() As Boolean
        Public Property ShowUserID7Field() As Boolean
        Public Property ShowUserID8Field() As Boolean
        Public Property ShowUserID9Field() As Boolean
        Public Property AgeAtWhichIDFieldsAreMandatory() As Int32
        Public Property PhotoCaptureUrl() As String
        Public Property AdvancedSearchType() As Integer
        Public Property DefaultMembershipQuantity() As Integer
        Public Property AutoEnrolPPSOnPayment() As Boolean
        Public Property SeatDisplay() As Integer
        Public Property ShowProductEntryTime() As Boolean
        Public Property RegistrationRemoveBlanksFromTelNo() As Boolean
        Public Property JavaScriptPath() As String
        Public Property JavaScriptVersion() As String
        Public Property PerformDiscontinuedProductCheck() As Boolean
        Public Property PerformAgentWatchListCheck() As Boolean
        Public Property ServeAllPagesSecure() As Boolean
        Public Property TradingPortalTicket() As Int32
        Public Property TradePlaceEncryptionKey() As String
        Public Property RedirectAfterAutoLogin() As String
        Public Property AuthorityUserProfile() As String
        Public Property DeliveryLeadTime() As String
        Public Property DeliveryLeadTimeHome() As String
        Public Property CalculateStockLeadTime() As Boolean
        Public Property DeliveryCutOffTime() As String
        Public Property PageRestrictionsInUse() As Boolean
        Public Property AllowCheckoutWhenBackEndUnavailable() As Boolean
        Public Property SendEmailAsHTML() As Boolean
        Public Property CustomerSearchMode() As Int32
        Public Property Payment3dSecureOurAccountId() As String
        Public Property Payment3dSecureOurSystemId() As String
        Public Property Payment3dSecureOurPasscode() As String
        Public Property Payment3dSecureOurSystemGUID() As String
        Public Property AgentPortalLinkOnSessionErrorPage() As Boolean
        Public Property SetBackendAccountNoFromUserDetails() As String
        Public Property UsePageExtraDataTable() As Boolean
        Public Property UseEncryptedPassword() As Boolean
        Public Property ForgottenPasswordExpiryTime() As Integer
        Public Property CorporateStadium() As String
        Public Property HospitalityForwardToBasket() As Integer
        Public Property HideOptionsWithoutPrice() As Boolean
        Public Property ParticipantsDefaultAddressInUse() As Boolean
        Public Property DefaultAddressLine1() As String
        Public Property DefaultAddressLine2() As String
        Public Property DefaultCity() As String
        Public Property DefaultCounty() As String
        Public Property DefaultCountry() As String
        Public Property DefaultPostcode() As String
        Public Property CourseStadium() As String
        Public Property CourseProductMaxQuantity() As String
        Public Property BypassDirectDebitPageWhenMixedBasket() As Boolean
        Public Property CheckUserCanAccessFandF() As Boolean
        Public Property Payment3dSecurePassIfEnrolledCodeU() As Boolean
        Public Property ImageNameReplaceDoubleQuote() As String
        Public Property Payment3dSecurePassIfResponseTimeout() As Boolean
        Public Property AmendPPSEnrolmentIgnoreFF() As Boolean
        Public Property CreditFinanceCompanyCode() As String
        Public Property WebOrderNumberPrefixOverride() As String
        Public Property ValidatePartnerDirectAccess() As Boolean
        Public Property SapOciPartner() As String
        Public Property UseEPOSOptions() As Boolean
        Public Property LoginWhenAddingHomeGameToBasket() As Boolean
        Public Property AllowAdditionalPromoWithFreeDel() As Boolean
        Public Property UsePrePromoValueForFreeDelCal() As Boolean
        Public Property PreferredDeliveryDateMaxDays() As Integer
        Public Property CashBackFeeCode() As String
        Public Property ProfilePhotoPermanentPath() As String
        Public Property ImageUploadTempPath() As String
        Public Property ProfilePhotoPermanentPathPhysical() As String
        Public Property ImageUploadTempPathPhysical() As String
        Public Property MaintenanceStartTime() As String
        Public Property MaintenanceEndTime() As String
        Public Property FavouriteSeatFunction() As Boolean
        Public Property AllowReassignmentOfReservedSeats() As String
        Public Property AlertsEnabled() As Boolean
        Public Property AlertsRefreshAttributesAtLogin() As Boolean
        Public Property AlertsGenerateAtLogin() As Boolean
        Public Property AlertsCCExpiryPPSWarnPeriod() As Integer
        Public Property AlertsCCExpirySAVWarnPeriod() As Integer
        Public Property AllowCheckoutWithMixedBasket() As Boolean
        Public Property UseSaveMyCard() As Boolean
        Public Property OnAccountEnabled() As Boolean = False
        Public Property AllowMultipleProductsInDDCheckout() As Boolean = True
        Public Property UseGlobalPriceListWithCustomerPriceList() As Boolean = False
        Public Property CustomerServicesEmailRetail() As String
        Public Property PaymentProcess3dSecureForAgents() As String
        Public Property PriceAndAreaSelection() As Boolean
        Public Property PriceAndAreaSelectionDescendingOrder() As Boolean
        Public Property PaymentHSBCPassWebOrderNo() As Boolean
        Public Property AllowCATTicketsByAgent() As Boolean
        Public Property AllowCATTicketsByUser() As Boolean
        Public Property StatusCheckOnLogin() As Boolean
        Public Property OnAccountModuleActive() As Boolean
        Public Property ShowFFOrderTemplates() As Boolean
        Public Property WholeSiteIsInAgentMode() As Boolean
        Public Property minageforcreditfinance() As Integer
        Public Property NonEnglishMonths() As String
        Public Property PaymentGatewayExternal() As String
        Public Property RememberMeFunction() As Boolean
        Public Property RememberMeCookieName() As String
        Public Property RememberMeCookieEncodeKey() As String
        Public Property RememberMeCookieExiryDays() As Integer
        Public Property QueueDBDirectAccessAllowed() As Boolean
        Public Property Payment3DSecureDefaultECI() As String
        Public Property Payment3DSecureDefaultATSData() As String
        Public Property EPurseTopUpProductCode() As String
        Public Property PageRestrictedAlertErrorPageURL() As String
        Public Property StopCodeForID3() As String
        Public Property UserIDValidationTypeforID3() As String
        Public Property AddressFormat() As String
        Public Property CheckAddressIsValidOnLogin() As Boolean
        Public Property ChangeBusinessUnitAllowed() As Boolean
        Public Property UseTicketingDeliveryAddresses() As Boolean
        Public Property UseRetailDeliveryAddresses() As Boolean
        Public Property AllowPublicTicketingDeliveryAddressSelection() As Boolean
        Public Property CanProcessFeesParallely() As Boolean
        Public Property AllowAgentPreferencesUpdates() As Boolean
        Public Property ISeriesCompanyCode() As String
        Public Property CheckoutPromotionType() As String
        Public Property ShowTravelAsDDL() As Boolean
        Public Property ShowUniqueMemberships() As Boolean
        Public Property RetailMissingImagePath() As String
        Public Property RetrievePartPayments() As Boolean
        Public Property AgentLevelCacheForProductStadiumAvailability() As Boolean
        Public Property TalentAdminClientName() As String
        Public Property MembershipsProductSubType() As String
        Public Property FeeCategoriesNotIncludedInECommerceTracking() As String
        Public Property CashbackRewardsActive() As Boolean
        Public Property BulkSalesModeBasketLimit() As Integer
        Public Property WebPricesModifyingMode() As Integer
        Public Property AllowManualIntervention() As Boolean
        Public Property UserDefinedPageQueryKeys() As String
        Public Property SavedSearchEnabled() As Boolean
        Public Property SavedSearchLimit() As Integer
        Public Property UseScanNowForPrintNowFulfilment() As Boolean
        Public Property ShowAgentDepartment() As Boolean
        Public Property RetailProductCode() As String
        Public Property GetCustomerTitlesFromIseries() As Boolean
        Public Property NotificationsOnConfirmPage() As Boolean
        Public Property MasterPageFolder() As String
        Public Property IsTestOrLive() As String
        Public Property NoOfRecentlyUsedCustomers() As Integer
        Public Property ProfileFullNameWithTitle() As Boolean
		Public Property TalentAPIAddress() As String
        Public Property SystemDefaultsUrl() As String
        Public Property SetRetailOrderToPending() As Boolean
		Public Property StaticContentQueryStringValue() As String
        Public Property SaltString() As String
        Public Property EncryptedPasswordPlaceholder() As String
        Public Property ShowProductTextOnVisualSeatSelection() As Boolean
        Public Property ShowBundleDateAsRange() As Boolean
        Public Property ShowProgressBar() As Boolean
        Public Property ProgressBarSubtypes() As String
        Public Property ShowTicketExchangeProgressBar() As Boolean
        Public Property DefaultStandAndAreaClick() As Boolean
        Public Property PaymentPayPalAccountID() As String
        Public Property PaymentPayPalEnvironment() As String
        Public Property CourierTrackingReferences() As Boolean
        Public Property ProductMainImagePathRelative() As String
        Public Property ProductMagicZoomImagePathRelative() As String
        Public Property DefaultDeliveryZoneCode() As String
        Public Property DiscountIF() As Boolean
        Public Property DiscountIFPSK() As String
        Public Property HighlightLoggedInCustomerPriceBand() As Boolean
        Public Property HospMergedDocumentPathRelative() As String
        Public Property HospitalityPDFAttachmentsOnConfirmationEmail() As String

#End Region
    End Class

End Class