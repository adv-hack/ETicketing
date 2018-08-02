-- =======================================================================
-- B2B -------------------------------------------------------------------
-- =======================================================================

-- tbl_bu
UPDATE [tbl_bu] SET [BUSINESS_UNIT] = 'TRADING_PORTAL' WHERE [BUSINESS_UNIT] = 'SUPPLYNET'


-- tbl_page
UPDATE [tbl_page] SET [IN_USE] = 1 WHERE [PAGE_CODE] = 'QuickOrder.aspx'


-- tbl_page_text_lang
UPDATE [tbl_page_text_lang] SET [TEXT_CONTENT] = '' WHERE [PAGE_CODE] = 'PurchaseHistory.aspx' AND [TEXT_CODE] = 'StatusOptions'
UPDATE [tbl_page_text_lang] SET [TEXT_CONTENT] = 'Order Lines' WHERE [TEXT_CODE] = 'quantityLabel' AND [PAGE_CODE] = 'PurchaseHistory.aspx'
UPDATE [tbl_page_text_lang]
SET [TEXT_CONTENT] = '<div class="ebiz-payment-details ebiz-MerchandisePaymentDetails">
	<h2>Your Payment Details&hellip;</h2>
	<div class="row small-up-1 medium-up-3">
		<div class="columns">
			<span class="ebiz-label">Your Payment Reference</span>
			<span class="ebiz-data"><<ORDER_REFERENCE>><<BACK_OFFICE_ORDER_ID>></span>
		</div>
		<div class="columns">
			<span class="ebiz-label">Your Payment Type</span>
			<span class="ebiz-data"><<PAYMENT_TYPE>></span>
		</div>
		<div class="columns">
			<span class="ebiz-label">Your Payment Amount</span>
			<span class="ebiz-data"><<PAYMENT_AMOUNT>></span>
		</div>
	</div>
</div>'
WHERE [PAGE_CODE] = 'CheckoutOrderConfirmation.aspx' AND [TEXT_CODE] = 'MerchandisePaymentDetails'
INSERT INTO [tbl_page_text_lang]
           ([BUSINESS_UNIT]
           ,[PARTNER_CODE]
           ,[PAGE_CODE]
           ,[TEXT_CODE]
           ,[LANGUAGE_CODE]
           ,[TEXT_CONTENT])
     VALUES
           ('BEKO'
           ,'*ALL'
           ,'PurchaseHistory.aspx'
           ,'StatusLabel'
           ,'ENG'
           ,'Status')


-- tbl_page_attribute
UPDATE [tbl_page_attribute] SET [ATTR_VALUE] = 'False' WHERE [ATTR_NAME] = 'RetailFromTicketingDB' AND [PAGE_CODE] = 'PurchaseHistory.aspx'
UPDATE [tbl_page_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'ShowRetailPurchaseHistoryOptions' AND [PAGE_CODE] = 'PurchaseHistory.aspx'
UPDATE [tbl_page_attribute] SET [ATTR_VALUE] = 'False' WHERE [ATTR_NAME] = 'ShowTicketingPurchaseHistoryOptions' AND [PAGE_CODE] = 'PurchaseHistory.aspx'
UPDATE [tbl_page_attribute] SET [ATTR_VALUE] = '^.{0,20}$' WHERE [ATTR_NAME] = 'PaymentRefRegex' AND [PAGE_CODE] = 'PurchaseHistory.aspx'
UPDATE [tbl_page_attribute] SET [ATTR_VALUE] = 'PRODUCT_SEARCH_KEYWORDS' WHERE [ATTR_NAME] = 'ProductFieldForAutoComplete'
UPDATE [tbl_page_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'ACEProductOptionMasterFilter' AND [PAGE_CODE] = 'QuickOrder.aspx'


-- tbl_page_extra_data
DELETE FROM [tbl_page_extra_data]


-- tbl_page_tracking
DELETE FROM [tbl_page_tracking]


-- tbl_database_version
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'SYSTEM21'
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'TALENTTKT'
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'ADL'
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'TALENTCRM'
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'SQLFARM'
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'TALENTQUEUE'
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'TalentDefinitions'
-- TEST DB
UPDATE [tbl_database_version] SET [CONNECTION_STRING] = 'Data Source=AG-TEST,28212;Initial Catalog=Test_TalentEBusinessDBBekoB2B;Integrated Security=False;User ID=Test_TalentEBusinessBekoB2BUser;Password=Password2007;Pooling=True;Min Pool Size=2;Max Pool Size=100' WHERE [DESTINATION_DATABASE] = 'SQL2005'
-- LIVE DB
UPDATE [tbl_database_version] SET [CONNECTION_STRING] = 'Data Source=AG-EBUSINESS,28210;Initial Catalog=TalentEBusinessDBBekoB2B;Integrated Security=False;User ID=TalentEBusinessBekoB2BUser;Password=Password2007;Pooling=True;Min Pool Size=2;Max Pool Size=100' WHERE [DESTINATION_DATABASE] = 'SQL2005'


-- tbl_defaults
UPDATE [tbl_defaults] SET [APPLY_VALUE] = '11' WHERE [APPLY_TYPE] = 'XML_ORDER_ATTRIBUTES' AND [APPLY_CODE] = 'DIST_CHAN'
UPDATE [tbl_defaults] SET [APPLY_VALUE] = 'TRPT' WHERE [APPLY_TYPE] = 'XML_ORDER_ATTRIBUTES' AND [APPLY_CODE] = 'PO_METHOD'
UPDATE [tbl_defaults] SET [APPLY_VALUE] = 'False' WHERE [APPLY_TYPE] = 'XML_ORDER_ATTRIBUTES' AND [APPLY_CODE] = 'SEND_CUSTOMER_EMAIL_ADDRESS_TO_SAP'
UPDATE [tbl_defaults] SET [APPLY_VALUE] = 'False' WHERE [APPLY_TYPE] = 'XML_ORDER_ATTRIBUTES' AND [APPLY_CODE] = 'SEND_CUSTOMER_POSTCODE_TO_SAP'


-- tbl_destination_xml
-- TEST (ARQ) SAP System
UPDATE [tbl_destination_xml] SET [POST_XML_URL] = 'http://195.87.244.139:8000/sap/bc/srt/rfc/sap/zsd_2032/046/zsd_2032/zsd_2032' WHERE [BUSINESS_UNIT] = 'BEKO' AND [DESTINATION_TYPE] = 'SAP'
-- LIVE (ARP) SAP System
UPDATE [tbl_destination_xml] SET [POST_XML_URL] = 'http://195.87.244.228:8100/sap/bc/srt/rfc/sap/zsd_2032/046/zsd_2032/zsd_2032?sap-language=EN' WHERE [BUSINESS_UNIT] = 'BEKO' AND [DESTINATION_TYPE] = 'SAP'


-- tbl_ecommerce_module_defaults_bu
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'True' WHERE [BUSINESS_UNIT] = 'BEKO' AND [DEFAULT_NAME] = 'USE_GLOBAL_PRICELIST_WITH_CUSTOMER_PRICELIST'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '/HTML/' WHERE [BUSINESS_UNIT] = 'BEKO' AND [DEFAULT_NAME] = 'HTML_PATH_RELATIVE'
-- TEST DB
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'F:\TalentEBusinessSuiteAssets\BekoB2B\Test\WebSales\HTML' WHERE [BUSINESS_UNIT] = 'BEKO' AND [DEFAULT_NAME] = 'HTML_PATH_ABSOLUTE'
-- LIVE DB
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'F:\TalentEBusinessSuiteAssets\BekoB2B\Live\WebSales\HTML' WHERE [BUSINESS_UNIT] = 'BEKO' AND [DEFAULT_NAME] = 'HTML_PATH_ABSOLUTE'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'W' WHERE [DEFAULT_NAME] = 'WEB_ORDER_NUMBER_PREFIX_OVERRIDE'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'SAP' WHERE [DEFAULT_NAME] = 'ORDER_DESTINATION_DATABASE'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'False' WHERE [DEFAULT_NAME] = 'RETRIEVE_ALTERNATIVE_PRODUCTS_AT_CHECKOUT'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'QASONDEMAND' WHERE [DEFAULT_NAME] = 'ADDRESSING_PROVIDER'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'False' WHERE [DEFAULT_NAME] = 'PERFORM_BACK_END_STOCK_CHECK'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'True' WHERE [DEFAULT_NAME] = 'ALLOW_MASTER_PRODUCTS_TO_BE_PURCHASED'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'ADD1' WHERE [DEFAULT_NAME] = 'ADDRESSING_MAP_ADR3'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'ADD2' WHERE [DEFAULT_NAME] = 'ADDRESSING_MAP_ADR4'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'TOWN,COUN' WHERE [DEFAULT_NAME] = 'ADDRESSING_MAP_ADR5'


-- tbl_ebusiness_descriptions
DELETE [tbl_ebusiness_descriptions] WHERE [DESCRIPTION_TYPE] = 'BackOfficeStatus'
INSERT INTO [tbl_ebusiness_descriptions] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','','All')
INSERT INTO [tbl_ebusiness_descriptions] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','50','Paid')
INSERT INTO [tbl_ebusiness_descriptions] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','70','Pending')
INSERT INTO [tbl_ebusiness_descriptions] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','90','Failed')
INSERT INTO [tbl_ebusiness_descriptions] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','100','Warehouse')


-- tbl_ebusiness_descriptions_bu
DELETE [tbl_ebusiness_descriptions_bu] WHERE [QUALIFIER] = 'OrderEnquiry'
INSERT INTO [tbl_ebusiness_descriptions_bu] ([QUALIFIER],[BUSINESS_UNIT],[PARTNER],[DESCRIPTION_TYPE],[DESCRIPTION_CODE])
	VALUES ('OrderEnquiry','BEKO','*ALL','BackOfficeStatus','')
INSERT INTO [tbl_ebusiness_descriptions_bu] ([QUALIFIER],[BUSINESS_UNIT],[PARTNER],[DESCRIPTION_TYPE],[DESCRIPTION_CODE])
	VALUES ('OrderEnquiry','BEKO','*ALL','BackOfficeStatus','50')
INSERT INTO [tbl_ebusiness_descriptions_bu] ([QUALIFIER],[BUSINESS_UNIT],[PARTNER],[DESCRIPTION_TYPE],[DESCRIPTION_CODE])
	VALUES ('OrderEnquiry','BEKO','*ALL','BackOfficeStatus','70')
INSERT INTO [tbl_ebusiness_descriptions_bu] ([QUALIFIER],[BUSINESS_UNIT],[PARTNER],[DESCRIPTION_TYPE],[DESCRIPTION_CODE])
	VALUES ('OrderEnquiry','BEKO','*ALL','BackOfficeStatus','90')
INSERT INTO [tbl_ebusiness_descriptions_bu] ([QUALIFIER],[BUSINESS_UNIT],[PARTNER],[DESCRIPTION_TYPE],[DESCRIPTION_CODE])
	VALUES ('OrderEnquiry','BEKO','*ALL','BackOfficeStatus','100')


-- tbl_ebusiness_descriptions_lang
DELETE [tbl_ebusiness_descriptions_lang] WHERE [DESCRIPTION_TYPE] = 'BackOfficeStatus'
INSERT INTO [tbl_ebusiness_descriptions_lang] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_LANGUAGE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','','ENG','All')
INSERT INTO [tbl_ebusiness_descriptions_lang] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_LANGUAGE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','50','ENG','Paid')
INSERT INTO [tbl_ebusiness_descriptions_lang] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_LANGUAGE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','70','ENG','Pending')
INSERT INTO [tbl_ebusiness_descriptions_lang] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_LANGUAGE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','90','ENG','Failed')
INSERT INTO [tbl_ebusiness_descriptions_lang] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_LANGUAGE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','100','ENG','Warehouse')


-- tbl_group_level_02
UPDATE [tbl_group_level_02] SET 
[GROUP_L02_ADV_SEARCH_TEMPLATE] = NULL, [GROUP_L02_PRODUCT_PAGE_TEMPLATE] = NULL, [GROUP_L02_PRODUCT_LIST_TEMPLATE] = 1, [GROUP_L02_SHOW_CHILDREN_AS_GROUPS] = 0, 
[GROUP_L02_SHOW_PRODUCTS_AS_LIST] = 1, [GROUP_L02_SHOW_IN_NAVIGATION] = 1, [GROUP_L02_SHOW_IN_GROUPED_NAV] = 0, [GROUP_L02_HTML_GROUP] = 1, [GROUP_L02_HTML_GROUP_TYPE] = 'FILE', [GROUP_L02_SHOW_PRODUCT_DISPLAY] = 1


-- tbl_group_level_03
UPDATE [tbl_group_level_03] SET
[GROUP_L03_ADV_SEARCH_TEMPLATE] = '', [GROUP_L03_PRODUCT_PAGE_TEMPLATE] = '', [GROUP_L03_PRODUCT_LIST_TEMPLATE] = 1, [GROUP_L03_SHOW_CHILDREN_AS_GROUPS] = 0, 
[GROUP_L03_SHOW_PRODUCTS_AS_LIST] = 1, [GROUP_L03_SHOW_IN_NAVIGATION] = 1, [GROUP_L03_SHOW_IN_GROUPED_NAV] = 0, [GROUP_L03_HTML_GROUP] = 1, [GROUP_L03_HTML_GROUP_TYPE] = 'FILE', [GROUP_L03_SHOW_PRODUCT_DISPLAY] = 1
	 
	 
-- tbl_authorized_partners
DELETE FROM [tbl_authorized_partners] WHERE [PARTNER] = 'EVERYONE'
UPDATE [tbl_authorized_partners] SET [BUSINESS_UNIT] = 'TRADING_PORTAL' WHERE [BUSINESS_UNIT] = 'SUPPLYNET'


-- tbl_authorized_users
UPDATE [tbl_authorized_users] SET [BUSINESS_UNIT] = 'TRADING_PORTAL' WHERE [BUSINESS_UNIT] = 'SUPPLYNET'


-- tbl_log_defaults
-- TEST DB
UPDATE [tbl_log_defaults] SET [LOG_PATH] = 'E:\TalentEBusinessSuiteLogs\BekoB2B\Test\'
-- LIVE DB
UPDATE [tbl_log_defaults] SET [LOG_PATH] = 'E:\TalentEBusinessSuiteLogs\BekoB2B\Live\'


-- tbl_url_bu
UPDATE [tbl_url_bu] SET [BUSINESS_UNIT_FOR_CONTENT] = 'BEKO'


--tbl_control_attribute
UPDATE [tbl_control_attribute] SET [ATTR_VALUE]='True' WHERE [ATTR_NAME]='EnableACEProductSearch'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE]='False' WHERE [CONTROL_CODE]='BasketDetails.ascx' AND [ATTR_NAME]='DisplayStockIssueLabel'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE]='True' WHERE [CONTROL_CODE]='BasketDetails.ascx' AND [ATTR_NAME]='DisplayStock_Basket'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE]='True' WHERE [ATTR_NAME] = 'ShowRetailHomeDeliveryOptions' AND [CONTROL_CODE] = 'DeliveryAddress.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'False' WHERE [ATTR_NAME] = 'mobileRowVisible' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'False' WHERE [ATTR_NAME] = 'phoneRowVisible' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'townEnableRFV' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'False' WHERE [ATTR_NAME] = 'phoneRowVisible' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'False' WHERE [ATTR_NAME] = 'mobileRowVisible' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'townEnableRFV' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'addressReferenceRowVisible' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'referenceEnableRFV' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'addressReferenceRowVisible' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'referenceEnableRFV' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CODE] = 'productListGraphical.ascx' WHERE [TEXT_CODE] IN ('YourSearchReturnedText1', 'YourSearchReturnedText2', 'YourSearchReturnedText3')
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'addressingOnOff' AND [CONTROL_CODE] = 'DeliveryAddress.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'False' WHERE [ATTR_NAME] = 'MakeAddressFieldsReadOnly' AND [CONTROL_CODE] = 'DeliveryAddress.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = '12' WHERE [ATTR_NAME] = 'productCodeLengthTrim' AND [CONTROL_CODE] = 'BasketDetails.ascx'

--tbl_control_text_lang
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = '<a class="show-for-medium" href="/PagesPublic/Home/home.aspx">Home</a><a class="show-for-medium" href="/PagesLogin/QuickOrder/QuickOrder.aspx">Quick Order/Home Delivery</a> <a class="show-for-medium" href="/PagesLogin/Orders/PurchaseHistory.aspx">Order History</a>' WHERE [TEXT_CODE] = 'LoggedInLabel'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] ='Please enter a value for quantity.' WHERE [CONTROL_CODE]='ProductOptions.ascx' AND [TEXT_CODE]='nullQuantityErrorText'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = '' WHERE [CONTROL_CODE] LIKE 'RegistrationForm%' AND [TEXT_CODE] = 'registrationHeaderLabel1'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Phone Number *' WHERE [TEXT_CODE] = 'buildingLabel' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'You must enter a phone number' WHERE [TEXT_CODE] = 'buildingRequiredFieldValidator' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'The phone number contains invalid characters' WHERE [TEXT_CODE] = 'buildingErrorText' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Alternative Number' WHERE [TEXT_CODE] = 'streetLabel' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'You must enter an alternative number' WHERE [TEXT_CODE] = 'streetRequiredFieldValidator' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'The alternative number contains invalid characters' WHERE [TEXT_CODE] = 'streetErrorText' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Address *' WHERE [TEXT_CODE] = 'townLabel' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'The address contains invalid characters' WHERE [TEXT_CODE] = 'townErrorText' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'The phone number contains invalid characters' WHERE [TEXT_CODE] = 'buildingErrorText' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Phone Number *' WHERE [TEXT_CODE] = 'buildingLabel' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'You must enter a phone number' WHERE [TEXT_CODE] = 'buildingRequiredFieldValidator' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'The alternative number contains invalid characters' WHERE [TEXT_CODE] = 'streetErrorText' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Alternative Number' WHERE [TEXT_CODE] = 'streetLabel' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'You must enter an alternative number' WHERE [TEXT_CODE] = 'streetRequiredFieldValidator' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'The address contains invalid characters' WHERE [TEXT_CODE] = 'townErrorText' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Address *' WHERE [TEXT_CODE] = 'townLabel' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Phone Number *' WHERE [TEXT_CODE] = 'Address1Label' AND [CONTROL_CODE] = 'DeliveryAddress.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Alternative Number' WHERE [TEXT_CODE] = 'Address2Label' AND [CONTROL_CODE] = 'DeliveryAddress.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Address *' WHERE [TEXT_CODE] = 'Address3Label' AND [CONTROL_CODE] = 'DeliveryAddress.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'You must enter a phone number' WHERE [TEXT_CODE] = 'HouseNoMissingErrorText' AND [CONTROL_CODE] = 'DeliveryAddress.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'You must enter an address' WHERE [TEXT_CODE] = 'AddressLine3MissingErrorText' AND [CONTROL_CODE] = 'DeliveryAddress.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Address Reference *' WHERE [TEXT_CODE] = 'referenceLabel' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'You must enter an address reference' WHERE [TEXT_CODE] = 'referenceRequiredFieldValidator' AND [CONTROL_CODE] = 'RegistrationForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Address Reference *' WHERE [TEXT_CODE] = 'referenceLabel' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'You must enter an address reference' WHERE [TEXT_CODE] = 'referenceRequiredFieldValidator' AND [CONTROL_CODE] = 'UpdateDetailsForm.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Use the quick order form to enter multiple products and quantities. Click Update to check the details you have entered.' WHERE [CONTROL_CODE] = 'QuickOrder.ascx' AND [TEXT_CODE] = 'lblIntro'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Enter alternative delivery address...' WHERE [TEXT_CODE] = 'AddNewAddressText'
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE], [BUSINESS_UNIT], [PARTNER_CODE], [PAGE_CODE], [CONTROL_CODE], [TEXT_CODE], [CONTROL_CONTENT], [HIDE_IN_MAINTENANCE])
VALUES ('ENG', '*ALL', '*ALL', '*ALL', 'RegistrationForm.ascx', 'townRequiredFieldValidator', 'You must enter an address', 0)
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE], [BUSINESS_UNIT], [PARTNER_CODE], [PAGE_CODE], [CONTROL_CODE], [TEXT_CODE], [CONTROL_CONTENT], [HIDE_IN_MAINTENANCE])
VALUES ('ENG', '*ALL', '*ALL', '*ALL', 'UpdateDetailsForm.ascx', 'townRequiredFieldValidator', 'You must enter an address', 0)
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE], [BUSINESS_UNIT], [PARTNER_CODE], [PAGE_CODE], [CONTROL_CODE], [TEXT_CODE], [CONTROL_CONTENT], [HIDE_IN_MAINTENANCE])
VALUES ('ENG', '*ALL', '*ALL', '*ALL', 'UpdateDetailsForm.ascx', 'cityRequiredFieldValidator', 'You must enter a town/city', 0)


-- tbl_partner
UPDATE [tbl_partner] SET [ENABLE_ALTERNATE_SKU] = 0 WHERE [PARTNER] <> 'BEKO' AND [PARTNER] <> 'IRIS'


-- tbl_payment_type
DELETE FROM [tbl_payment_type] WHERE [PAYMENT_TYPE_CODE] <> 'IV'
UPDATE [tbl_payment_type] SET [IS_OTHER_TYPE] = 0 WHERE [PAYMENT_TYPE_CODE] = 'IV'


-- tbl_payment_type_lang
DELETE FROM [tbl_payment_type_lang] WHERE [PAYMENT_TYPE_CODE] <> 'IV'


-- tbl_payment_type_bu
UPDATE  [tbl_payment_type_bu] SET 
			[DEFAULT_PAYMENT_TYPE] = 1
           ,[RETAIL_TYPE]= 1
           ,[TICKETING_TYPE]= 1
           ,[AGENT_RETAIL_TYPE]= 1
           ,[AGENT_TICKETING_TYPE]= 1
           ,[AGENT_TICKETING_TYPE_REFUND]= 1
           ,[TICKETING_TYPE_REFUND]= 1
           ,[AGENT_TICKETING_TYPE_CANCEL]= 1
           ,[TICKETING_TYPE_CANCEL]= 1
           ,[PPS_TYPE]= 1
           ,[GENERIC_SALES]= 1
           ,[GENERIC_SALES_REFUND]= 1
		   WHERE PAYMENT_TYPE_CODE = 'IV'
DELETE FROM [tbl_payment_type_bu] WHERE [PAYMENT_TYPE_CODE] <> 'IV'


-- tbl_product_relations_defaults
UPDATE [tbl_product_relations_defaults] SET [ONOFF] = 1 WHERE [BUSINESS_UNIT] = 'BEKO' AND [PARTNER] = '*ALL' AND [PAGE_CODE] = 'Basket.aspx'
UPDATE [tbl_product_relations_defaults] SET [ONOFF] = 1 WHERE [BUSINESS_UNIT] = 'BEKO' AND [PARTNER] = '*ALL' AND [PAGE_CODE] = 'CheckoutOrderConfirmation.aspx'


-- tbl_template_page
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'QuickOrder.aspx'


-- tbl_supplynet_defaults
UPDATE [tbl_supplynet_defaults] SET [BUSINESS_UNIT] = 'TRADING_PORTAL' WHERE [BUSINESS_UNIT] = 'SUPPLYNET'


-- tbl_supplynet_module_defaults
UPDATE [tbl_supplynet_module_defaults] SET [BUSINESS_UNIT] = 'TRADING_PORTAL' WHERE [BUSINESS_UNIT] = 'SUPPLYNET'


-- tbl_supplynet_module_defaults_bu
UPDATE [tbl_supplynet_module_defaults_bu] SET [VALUE] = 'True' WHERE [DEFAULT_NAME] = 'UPDATE_DESCRIPTIONS'


-- =======================================================================
-- B2C -------------------------------------------------------------------
-- =======================================================================

-- tbl_address
DELETE FROM [tbl_address] WHERE [PARTNER] <> 'BEKO'
UPDATE [tbl_address] SET [PARTNER] = '98117' WHERE [PARTNER] = '*ALL'


-- tbl_authorized_users
DELETE FROM [tbl_authorized_users] WHERE [PARTNER] <> 'BEKO'
UPDATE [tbl_authorized_users] SET [BUSINESS_UNIT] = 'TRADING_PORTAL' WHERE [BUSINESS_UNIT] = 'SUPPLYNET'
UPDATE [tbl_authorized_users] SET [PARTNER] = '98117' WHERE [PARTNER] = '*ALL'


-- tbl_bu
UPDATE [tbl_bu] SET [BUSINESS_UNIT] = 'TRADING_PORTAL' WHERE [BUSINESS_UNIT] = 'SUPPLYNET'


-- tbl_email_templates
UPDATE [tbl_email_templates] SET [BUSINESS_UNIT] = 'BEKO' WHERE [BUSINESS_UNIT] = 'UNITEDKINGDOM'


-- tbl_page
UPDATE [tbl_page] SET [IN_USE] = 0 WHERE [PAGE_CODE] = 'QuickOrder.aspx'


-- tbl_page_text_lang
UPDATE [tbl_page_text_lang] SET [TEXT_CONTENT] = '' WHERE [PAGE_CODE] = 'PurchaseHistory.aspx' AND [TEXT_CODE] = 'StatusOptions'
UPDATE [tbl_page_text_lang] SET [TEXT_CONTENT] = 'Order Lines' WHERE [TEXT_CODE] = 'quantityLabel' AND [PAGE_CODE] = 'PurchaseHistory.aspx'
UPDATE [tbl_page_text_lang]
SET [TEXT_CONTENT] = '<div class="ebiz-payment-details ebiz-MerchandisePaymentDetails">
	<h2>Your Payment Details&hellip;</h2>
	<div class="row small-up-1 medium-up-3">
		<div class="columns">
			<span class="ebiz-label">Your Payment Reference</span>
			<span class="ebiz-data"><<ORDER_REFERENCE>><<BACK_OFFICE_ORDER_ID>></span>
		</div>
		<div class="columns">
			<span class="ebiz-label">Your Payment Type</span>
			<span class="ebiz-data"><<PAYMENT_TYPE>></span>
		</div>
		<div class="columns">
			<span class="ebiz-label">Your Payment Amount</span>
			<span class="ebiz-data"><<PAYMENT_AMOUNT>></span>
		</div>
	</div>
</div>'
WHERE [PAGE_CODE] = 'CheckoutOrderConfirmation.aspx' AND [TEXT_CODE] = 'MerchandisePaymentDetails'
INSERT INTO [tbl_page_text_lang]
           ([BUSINESS_UNIT]
           ,[PARTNER_CODE]
           ,[PAGE_CODE]
           ,[TEXT_CODE]
           ,[LANGUAGE_CODE]
           ,[TEXT_CONTENT])
     VALUES
           ('BEKO'
           ,'*ALL'
           ,'PurchaseHistory.aspx'
           ,'StatusLabel'
           ,'ENG'
           ,'Status')


-- tbl_page_extra_data
DELETE FROM [tbl_page_extra_data]


-- tbl_page_attribute
UPDATE [tbl_page_attribute] SET [ATTR_VALUE] = 'False' WHERE [ATTR_NAME] = 'RetailFromTicketingDB' AND [PAGE_CODE] = 'PurchaseHistory.aspx'
UPDATE [tbl_page_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'ShowRetailPurchaseHistoryOptions' AND [PAGE_CODE] = 'PurchaseHistory.aspx'
UPDATE [tbl_page_attribute] SET [ATTR_VALUE] = 'False' WHERE [ATTR_NAME] = 'ShowTicketingPurchaseHistoryOptions' AND [PAGE_CODE] = 'PurchaseHistory.aspx'
UPDATE [tbl_page_attribute] SET [ATTR_VALUE] = '^.{0,20}$' WHERE [ATTR_NAME] = 'PaymentRefRegex' AND [PAGE_CODE] = 'PurchaseHistory.aspx'
UPDATE [tbl_page_attribute] SET [ATTR_VALUE] = 'PRODUCT_SEARCH_KEYWORDS' WHERE [ATTR_NAME] = 'ProductFieldForAutoComplete'


-- tbl_page_tracking
DELETE FROM [tbl_page_tracking]


-- tbl_database_version
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'SYSTEM21'
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'TALENTTKT'
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'ADL'
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'TALENTCRM'
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'SQLFARM'
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'TALENTQUEUE'
DELETE FROM [tbl_database_version] WHERE [DESTINATION_DATABASE] = 'TalentDefinitions'
-- TEST DB
UPDATE [tbl_database_version] SET [CONNECTION_STRING] = 'Data Source=AG-TEST,28212;Initial Catalog=Test_TalentEBusinessDBBekoB2C;Integrated Security=False;User ID=Test_TalentEBusinessBekoB2CUser;Password=Password2007;Pooling=True;Min Pool Size=2;Max Pool Size=100' WHERE [DESTINATION_DATABASE] = 'SQL2005'
-- LIVE DB
UPDATE [tbl_database_version] SET [CONNECTION_STRING] = 'Data Source=AG-EBUSINESS,28210;Initial Catalog=TalentEBusinessDBBekoB2C;Integrated Security=False;User ID=TalentEBusinessBekoB2CUser;Password=Password2007;Pooling=True;Min Pool Size=2;Max Pool Size=100' WHERE [DESTINATION_DATABASE] = 'SQL2005'


-- tbl_ecommerce_module_defaults_bu
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'False' WHERE [DEFAULT_NAME] = 'EXTERNAL_PASSWORD_IS_MASTER'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'True' WHERE [BUSINESS_UNIT] = 'BEKO' AND [DEFAULT_NAME] = 'USE_GLOBAL_PRICELIST_WITH_CUSTOMER_PRICELIST'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '/HTML/' WHERE [BUSINESS_UNIT] = 'BEKO' AND [DEFAULT_NAME] = 'HTML_PATH_RELATIVE'
-- TEST DB
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'F:\TalentEBusinessSuiteAssets\BekoB2C\Test\WebSales\HTML' WHERE [BUSINESS_UNIT] = 'BEKO' AND [DEFAULT_NAME] = 'HTML_PATH_ABSOLUTE'
-- LIVE DB
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'F:\TalentEBusinessSuiteAssets\BekoB2C\Live\WebSales\HTML' WHERE [BUSINESS_UNIT] = 'BEKO' AND [DEFAULT_NAME] = 'HTML_PATH_ABSOLUTE'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'VANGUARD' WHERE [BUSINESS_UNIT] = 'BEKO' AND [PARTNER] = '*ALL' AND [DEFAULT_NAME] = 'PAYMENT_GATEWAY_TYPE'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'True' WHERE [DEFAULT_NAME] = 'USE_LOGIN_LOOKUP'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'False' WHERE DEFAULT_NAME = 'REFRESH_CUSTOMER_DATA_ON_LOGIN'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '0' WHERE DEFAULT_NAME = 'TRADINGPORTALTICKET'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'US' WHERE [DEFAULT_NAME] = 'WEB_ORDER_NUMBER_PREFIX_OVERRIDE'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'SAP' WHERE [DEFAULT_NAME] = 'ORDER_DESTINATION_DATABASE'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '000000000000' WHERE [DEFAULT_NAME] = 'USER_NUMBER'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'BEKOSALT#1' WHERE [DEFAULT_NAME] = 'CLIENT_SALT'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'FALSE' WHERE [DEFAULT_NAME] = 'ALLOW_DUPLICATE_EMAIL'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'True' WHERE [DEFAULT_NAME] = 'USE_ENCRYPTED_PASSWORD'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'False' WHERE [DEFAULT_NAME] = 'RETRIEVE_ALTERNATIVE_PRODUCTS_AT_CHECKOUT'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'Z89' WHERE [DEFAULT_NAME] = 'DEFAULT_DELIVERY_ZONE_CODE'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'True' WHERE [DEFAULT_NAME] = 'ALLOW_MASTER_PRODUCTS_TO_BE_PURCHASED'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'False' WHERE [DEFAULT_NAME] = 'SET_BACKEND_ACCOUNT_NO_FROM_USER_DETAILS'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'QASONDEMAND' WHERE [DEFAULT_NAME] = 'ADDRESSING_PROVIDER'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '98117' WHERE [DEFAULT_NAME] = 'PRICE_LIST'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'False' WHERE [DEFAULT_NAME] = 'PERFORM_BACK_END_STOCK_CHECK'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '2:00 PM' WHERE [DEFAULT_NAME] = 'DELIVERY_CUT_OFF_TIME'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '1' WHERE [DEFAULT_NAME] = 'DELIVERY_LEAD_TIME_HOME'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '1' WHERE [DEFAULT_NAME] = 'DELIVERY_LEAD_TIME'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'ADD1' WHERE [DEFAULT_NAME] = 'ADDRESSING_MAP_ADR3'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'ADD2' WHERE [DEFAULT_NAME] = 'ADDRESSING_MAP_ADR4'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'TOWN,COUN' WHERE [DEFAULT_NAME] = 'ADDRESSING_MAP_ADR5'

-- LIVE 3D Secure settings
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'VANGUARD' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_PROVIDER'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'True' WHERE [DEFAULT_NAME] = 'PAYMENT_PROCESS_3D_SECURE'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '20071463' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_1'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '24' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_2'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'Beko plc' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_3'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '465733' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_4'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '013154007000000' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_5'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'Comm1D3a' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_6'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '531615' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_7'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '013154007000000' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_8'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_9'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'Y,A,E' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_SUCCESS_CODES'
-- TEST 3D Secure settings
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'VANGUARD' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_PROVIDER'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'True' WHERE [DEFAULT_NAME] = 'PAYMENT_PROCESS_3D_SECURE'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '10006780' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_1'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '2' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_2'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'IRIS' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_3'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '123456' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_4'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '22270438' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_5'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '12345678' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_6'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '542515' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_7'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '83589362' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_8'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '12345678' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_DETAILS_9'
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'Y,A,E' WHERE [DEFAULT_NAME] = 'PAYMENT_3DSECURE_SUCCESS_CODES'

INSERT INTO tbl_ecommerce_module_defaults_bu 
([BUSINESS_UNIT], [PARTNER], [APPLICATION],[MODULE],[DEFAULT_NAME],[VALUE]) 
VALUES ('BEKO', '*ALL', '','','PAYMENT_GATEWAY_EXTERNAL','VANGUARD')

DELETE FROM [tbl_ecommerce_module_defaults_bu] WHERE [DEFAULT_NAME] = 'PRICE_LIST' AND [PARTNER] <> '*ALL'


-- tbl_ebusiness_descriptions
INSERT INTO [tbl_ebusiness_descriptions] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','','All')
INSERT INTO [tbl_ebusiness_descriptions] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','50','Paid')
INSERT INTO [tbl_ebusiness_descriptions] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','70','Pending')
INSERT INTO [tbl_ebusiness_descriptions] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','90','Failed')
INSERT INTO [tbl_ebusiness_descriptions] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','100','Warehouse')


-- tbl_ebusiness_descriptions_bu
INSERT INTO [tbl_ebusiness_descriptions_bu] ([QUALIFIER],[BUSINESS_UNIT],[PARTNER],[DESCRIPTION_TYPE],[DESCRIPTION_CODE])
	VALUES ('OrderEnquiry','BEKO','*ALL','BackOfficeStatus','')
INSERT INTO [tbl_ebusiness_descriptions_bu] ([QUALIFIER],[BUSINESS_UNIT],[PARTNER],[DESCRIPTION_TYPE],[DESCRIPTION_CODE])
	VALUES ('OrderEnquiry','BEKO','*ALL','BackOfficeStatus','50')
INSERT INTO [tbl_ebusiness_descriptions_bu] ([QUALIFIER],[BUSINESS_UNIT],[PARTNER],[DESCRIPTION_TYPE],[DESCRIPTION_CODE])
	VALUES ('OrderEnquiry','BEKO','*ALL','BackOfficeStatus','70')
INSERT INTO [tbl_ebusiness_descriptions_bu] ([QUALIFIER],[BUSINESS_UNIT],[PARTNER],[DESCRIPTION_TYPE],[DESCRIPTION_CODE])
	VALUES ('OrderEnquiry','BEKO','*ALL','BackOfficeStatus','90')
INSERT INTO [tbl_ebusiness_descriptions_bu] ([QUALIFIER],[BUSINESS_UNIT],[PARTNER],[DESCRIPTION_TYPE],[DESCRIPTION_CODE])
	VALUES ('OrderEnquiry','BEKO','*ALL','BackOfficeStatus','100')


-- tbl_ebusiness_descriptions_lang
INSERT INTO [tbl_ebusiness_descriptions_lang] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_LANGUAGE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','','ENG','All')
INSERT INTO [tbl_ebusiness_descriptions_lang] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_LANGUAGE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','50','ENG','Paid')
INSERT INTO [tbl_ebusiness_descriptions_lang] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_LANGUAGE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','70','ENG','Pending')
INSERT INTO [tbl_ebusiness_descriptions_lang] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_LANGUAGE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','90','ENG','Failed')
INSERT INTO [tbl_ebusiness_descriptions_lang] ([DESCRIPTION_TYPE],[DESCRIPTION_CODE],[DESCRIPTION_LANGUAGE],[DESCRIPTION_DESCRIPTION])
     VALUES ('BackOfficeStatus','100','ENG','Warehouse')


-- tbl_destination_xml
-- TEST (ARQ) SAP System
UPDATE [tbl_destination_xml] SET [POST_XML_URL] = 'http://195.87.244.139:8000/sap/bc/srt/rfc/sap/zsd_2032/046/zsd_2032/zsd_2032' WHERE [BUSINESS_UNIT] = 'BEKO' AND [DESTINATION_TYPE] = 'SAP'
-- LIVE (ARP) SAP System
UPDATE [tbl_destination_xml] SET [POST_XML_URL] = 'http://195.87.244.228:8100/sap/bc/srt/rfc/sap/zsd_2032/046/zsd_2032/zsd_2032?sap-language=EN' WHERE [BUSINESS_UNIT] = 'BEKO' AND [DESTINATION_TYPE] = 'SAP'


-- tbl_control_text_lang
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = '' WHERE [CONTROL_CODE] LIKE 'RegistrationForm%' AND [TEXT_CODE] = 'registrationHeaderLabel1'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Email address' WHERE [TEXT_CODE] = 'UsernameLabelText' AND [CONTROL_CODE] = 'LoginBox.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'You must supply an email address to login' WHERE [TEXT_CODE] = 'UsernameRequiredErrorMessage' AND [CONTROL_CODE] = 'LoginBox.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = '<span class="alert">Your email address is incorrect</span>' WHERE [TEXT_CODE] = 'UsernameFailedLoginText'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Stay signed in' WHERE [TEXT_CODE] = 'RememberMeText'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Are you a returning customer?' WHERE [TEXT_CODE] = 'TitleText' AND [CONTROL_CODE] = 'LoginBox.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Is this your first time purchasing?' WHERE [TEXT_CODE] = 'RegisterAccountTitleText'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = '' WHERE [TEXT_CODE] = 'RegisterAccountInfoText'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Register' WHERE [TEXT_CODE] = 'RegisterButtonText'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = '<i class="fa fa-shopping-basket" aria-hidden="true"></i> <span class="ebiz-basket-summary">Basket:</span> <span class="ebiz-basket-items"><<BASKET_ITEMS>> items:</span> <span class="ebiz-basket-total"><<BASKET_TOTAL>></span>' WHERE [TEXT_CODE] = 'BasketLinkText'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = '<a href="../../PagesPublic/Login/Login.aspx">Sign In</a>' WHERE [TEXT_CODE] = 'AnonymousLabel'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = '<a href="../../PagesPublic/Profile/Registration.aspx">Register</a>' WHERE [TEXT_CODE] = 'RegisterLinkText' AND [CONTROL_CODE] = 'PersonalisationBar.ascx'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Hello,&nbsp;<<FORENAME>>' WHERE [TEXT_CODE] = 'LoggedInLabel'
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Please enter a value for quantity.' WHERE [CONTROL_CODE]='ProductOptions.ascx' AND [TEXT_CODE]='nullQuantityErrorText'
UPDATE [tbl_control_text_lang] SET [CONTROL_CODE] = 'productListGraphical.ascx' WHERE [TEXT_CODE] IN ('YourSearchReturnedText1', 'YourSearchReturnedText2', 'YourSearchReturnedText3')
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Enter alternative delivery address...' WHERE [TEXT_CODE] = 'AddNewAddressText'


-- tbl_control_attribute
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'ShowSearchBar'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'False' WHERE [ATTR_NAME] = 'AddMembership_CheckedByDefault'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'False' WHERE [ATTR_NAME] = 'AddMembership_ShowCheckbox'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [BUSINESS_UNIT] = 'BEKO' AND [CONTROL_CODE] = 'SearchBar.ascx' AND [ATTR_NAME] = 'EnableACEProductSearch'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [CONTROL_CODE] = 'DeliveryAddress.ascx' AND [ATTR_NAME] = 'deliveryDayRowVisible'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [CONTROL_CODE] = 'DeliveryAddress.ascx' AND [ATTR_NAME] = 'deliveryDateRowVisible'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [CONTROL_CODE] = 'DeliveryAddress.ascx' AND [ATTR_NAME] = 'DisplayDeliveryMessage'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE]='False' WHERE [BUSINESS_UNIT]='BEKO' AND [CONTROL_CODE]='BasketDetails.ascx' AND [ATTR_NAME]='DisplayStockIssueLabel'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE]='True' WHERE [BUSINESS_UNIT]='BEKO' AND [CONTROL_CODE]='BasketDetails.ascx' AND [ATTR_NAME]='DisplayStock_Basket'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [CONTROL_CODE] = 'RegistrationForm.ascx' AND [ATTR_NAME] = 'addressingOnOff'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'addressingOnOff' AND [CONTROL_CODE] = 'DeliveryAddress.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'X,T' WHERE [ATTR_NAME] = 'UnavailableRestockCodes' AND [CONTROL_CODE] = 'ProductControls.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'False' WHERE [ATTR_NAME] = 'MakeAddressFieldsReadOnly' AND [CONTROL_CODE] = 'DeliveryAddress.ascx'
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = '12' WHERE [ATTR_NAME] = 'productCodeLengthTrim' AND [CONTROL_CODE] = 'BasketDetails.ascx'
INSERT INTO [tbl_control_attribute] 
(BUSINESS_UNIT,PARTNER_CODE,PAGE_CODE,CONTROL_CODE,ATTR_NAME,ATTR_VALUE,DESCRIPTION,HIDE_IN_MAINTENANCE) 
VALUES ('*ALL','*ALL','*ALL','RegistrationForm.ascx','sexRowVisible','FALSE','',0)


-- tbl_authorized_partners
DELETE FROM [tbl_authorized_partners] WHERE [PARTNER] <> '*ALL' AND [BUSINESS_UNIT] = 'BEKO'
UPDATE [tbl_authorized_partners] SET [BUSINESS_UNIT] = 'TRADING_PORTAL' WHERE [BUSINESS_UNIT] = 'SUPPLYNET'
INSERT INTO [tbl_authorized_partners] (BUSINESS_UNIT, PARTNER, DEFAULT_PARTNER)
VALUES ('BEKO', '98117', 1)


-- tbl_log_defaults
-- TEST DB
UPDATE [tbl_log_defaults] SET [LOG_PATH] = 'E:\TalentEBusinessSuiteLogs\BekoB2C\Test\'
-- LIVE DB
UPDATE [tbl_log_defaults] SET [LOG_PATH] = 'E:\TalentEBusinessSuiteLogs\BekoB2C\Live\'


-- tbl_url_bu
UPDATE [tbl_url_bu] SET [BUSINESS_UNIT_FOR_CONTENT] = 'BEKO'


-- tbl_bu_module_database
UPDATE [tbl_bu_module_database] SET [DESTINATION_DATABASE]='SQL2005' WHERE [MODULE]='GetVanguardConfigurations'
UPDATE [tbl_bu_module_database] SET [DESTINATION_DATABASE] = 'SQL2005' WHERE [MODULE] = 'CustomerRetrieval'
UPDATE [tbl_bu_module_database] SET [DESTINATION_DATABASE] = 'SQL2005' WHERE [MODULE] = 'RetrievePassword'


-- tbl_carrier
DELETE FROM [tbl_carrier]
INSERT INTO [tbl_carrier] ([CARRIER_CODE],[INSTALLATION_AVAILABLE],[COLLECT_OLD_AVAILABLE],[DELIVER_MONDAY],[DELIVER_TUESDAY],[DELIVER_WEDNESDAY],[DELIVER_THURSDAY],[DELIVER_FRIDAY],[DELIVER_SATURDAY],[DELIVER_SUNDAY])
     VALUES ('B2C',0,0,1,1,1,1,1,0,0)


-- tbl_defaults
UPDATE [tbl_defaults] SET [APPLY_VALUE] = '16' WHERE [APPLY_TYPE] = 'XML_ORDER_ATTRIBUTES' AND [APPLY_CODE] = 'DIST_CHAN'
UPDATE [tbl_defaults] SET [APPLY_VALUE] = 'WSHP' WHERE [APPLY_TYPE] = 'XML_ORDER_ATTRIBUTES' AND [APPLY_CODE] = 'PO_METHOD'
-- TEST DB
UPDATE [tbl_defaults] SET [APPLY_VALUE] = 'https://vg-cst.cxmlpg.com/vanguard.aspx' WHERE [APPLY_TYPE] = 'VANGUARD_ATTRIBUTES' AND [APPLY_CODE] = 'CARD_CAPTURE_PAGE'
UPDATE [tbl_defaults] SET [APPLY_VALUE] = 'https://txn-cst.cxmlpg.com/XML4/commideagateway.asmx' WHERE [APPLY_TYPE] = 'VANGUARD_CONFIGURATION' AND [APPLY_CODE] = 'WebserviceURL'
-- LIVE DB
UPDATE [tbl_defaults] SET [APPLY_VALUE] = 'https://vg-a.cxmlpg.com/vanguard.aspx' WHERE [APPLY_TYPE] = 'VANGUARD_ATTRIBUTES' AND [APPLY_CODE] = 'CARD_CAPTURE_PAGE'
UPDATE [tbl_defaults] SET [APPLY_VALUE] = 'https://payment.cxmlpg.com/XML4/commideagateway.asmx' WHERE [APPLY_TYPE] = 'VANGUARD_CONFIGURATION' AND [APPLY_CODE] = 'WebserviceURL'
UPDATE [tbl_defaults] SET [APPLY_VALUE] = 'True' WHERE [APPLY_TYPE] = 'XML_ORDER_ATTRIBUTES' AND [APPLY_CODE] = 'SEND_CUSTOMER_EMAIL_ADDRESS_TO_SAP'
UPDATE [tbl_defaults] SET [APPLY_VALUE] = 'True' WHERE [APPLY_TYPE] = 'XML_ORDER_ATTRIBUTES' AND [APPLY_CODE] = 'SEND_CUSTOMER_POSTCODE_TO_SAP'


-- tbl_delivery_unavailable_dates
DELETE FROM [tbl_delivery_unavailable_dates]
INSERT INTO [tbl_delivery_unavailable_dates] ([CARRIER_CODE],[DATE])
     VALUES ('B2C','28/08/2017 00:00:00')
INSERT INTO [tbl_delivery_unavailable_dates] ([CARRIER_CODE],[DATE])
     VALUES ('B2C','25/12/2017 00:00:00')
INSERT INTO [tbl_delivery_unavailable_dates] ([CARRIER_CODE],[DATE])
     VALUES ('B2C','26/12/2017 00:00:00')


-- tbl_delivery_zone
DELETE FROM [tbl_delivery_zone]
INSERT INTO [tbl_delivery_zone] ([BUSINESS_UNIT],[PARTNER],[DELIVERY_ZONE_CODE],[DELIVERY_ZONE_REFERENCE],[DELIVERY_ZONE_TYPE],[MONDAY],[TUESDAY],[WEDNESDAY],[THURSDAY],[FRIDAY],[SATURDAY],[SUNDAY])
     VALUES ('BEKO','*ALL','Z89','','1',1,1,1,1,1,0,0)


-- tbl_partner
DELETE FROM [tbl_partner] WHERE [PARTNER] <> 'BEKO' AND [PARTNER] <> 'IRIS'
INSERT INTO [dbo].[tbl_partner]
           ([PARTNER],[PARTNER_DESC],[DESTINATION_DATABASE],[CACHEING_ENABLED],[CACHE_TIME_MINUTES],[LOGGING_ENABLED],[STORE_XML]
           ,[ACCOUNT_NO_1],[ACCOUNT_NO_2],[ACCOUNT_NO_3],[ACCOUNT_NO_4],[ACCOUNT_NO_5],[EMAIL],[TELEPHONE_NUMBER]
           ,[FAX_NUMBER],[PARTNER_URL],[PARTNER_NUMBER],[ORIGINATING_BUSINESS_UNIT],[CRM_BRANCH],[VAT_NUMBER],[ENABLE_PRICE_VIEW],[ORDER_ENQUIRY_SHOW_PARTNER_ORDERS]
           ,[COST_CENTRE],[ENABLE_ALTERNATE_SKU],[MINIMUM_PURCHASE_QUANTITY],[MINIMUM_PURCHASE_AMOUNT],[USE_MINIMUM_PURCHASE_QUANTITY],[USE_MINIMUM_PURCHASE_AMOUNT],[SHOW_PREFERRED_DELIVERY_DATE]
           ,[PARTNER_TYPE],[ORDER_CONFIRMATION_EMAIL],[HIDE_PRICES],[CARRIER_CODE],[RESTRICT_DIRECT_ACCESS])
     VALUES
           ('98117','All Public Users','SQL2005',1,1,0,0,
		   '98117','','','','','','',
		   '','',0,'','','',1,0,
		   '',0,0,0,0,0,1,
		   'HOME',1,0,'B2C',0)


-- tbl_partner_user
DELETE FROM [tbl_partner_user] WHERE [PARTNER] <> 'BEKO'
UPDATE [tbl_partner_user] SET [PARTNER] = '98117' WHERE [PARTNER] = '*ALL'


-- tbl_order_header
TRUNCATE TABLE [tbl_order_header]


-- tbl_order_detail
TRUNCATE TABLE [tbl_order_detail]


-- tbl_order_status
TRUNCATE TABLE [tbl_order_status]


-- tbl_serialized_objects
TRUNCATE TABLE [tbl_serialized_objects]


-- tbl_supplynet_defaults
UPDATE [tbl_supplynet_defaults] SET [BUSINESS_UNIT] = 'TRADING_PORTAL' WHERE [BUSINESS_UNIT] = 'SUPPLYNET'


-- tbl_supplynet_module_defaults
UPDATE [tbl_supplynet_module_defaults] SET [BUSINESS_UNIT] = 'TRADING_PORTAL' WHERE [BUSINESS_UNIT] = 'SUPPLYNET'


-- tbl_supplynet_module_defaults_bu
UPDATE [tbl_supplynet_module_defaults_bu] SET [VALUE] = 'True' WHERE [DEFAULT_NAME] = 'UPDATE_DESCRIPTIONS'


-- tbl_template_page
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'checkout3dsecure.aspx'