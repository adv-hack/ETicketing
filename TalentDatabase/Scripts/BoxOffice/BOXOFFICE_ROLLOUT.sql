-- ===========================================================
-- Config Script for rolling out box office for the first time
-- ===========================================================

DECLARE @BusinessUnit AS NVARCHAR(10);
SET @BusinessUnit = 'BOXOFFICE'

--[tbl_ecommerce_module_defaults_bu]
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '~/PagesAgent/Profile/CustomerSelection.aspx' WHERE [DEFAULT_NAME] = 'REDIRECT_AFTER_AGENT_LOGOUT_URL' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '~/PagesAgent/Profile/CustomerSelection.aspx' WHERE [DEFAULT_NAME] = 'REDIRECT_AFTER_AGENT_LOGIN_URL' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '~/PagesAgent/Profile/CustomerSelection.aspx' WHERE [DEFAULT_NAME] = 'APPLICATION_STARTUP_PAGE' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '~/PagesAgent/Profile/CustomerSelection.aspx' WHERE [DEFAULT_NAME] = 'LOGOUT_PAGE' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '~/PagesAgent/Profile/CustomerSelection.aspx' WHERE [DEFAULT_NAME] = 'PAGE_AFTER_LOGIN' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'False' WHERE [DEFAULT_NAME] = 'PERFORM_CREDIT_CHECK' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'False' WHERE [DEFAULT_NAME] = 'CREDIT_CHECK_ON_LOGIN' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'False' WHERE [DEFAULT_NAME] = 'NOISE_SESSION_EXPIRES_AFTER_CHECKOUT' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'True' WHERE [DEFAULT_NAME] = 'ALLOW_AGENT_PREFERENCES_UPDATES' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = '1' WHERE [DEFAULT_NAME] = 'CUSTOMER_SEARCH_MODE' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'True' WHERE [DEFAULT_NAME] = 'WHOLE_SITE_IS_IN_AGENT_MODE' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'False' WHERE [DEFAULT_NAME] = 'NOISE_IN_USE' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'BUI' WHERE [DEFAULT_NAME] = 'THEME' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_ecommerce_module_defaults_bu] SET [VALUE] = 'True' WHERE [DEFAULT_NAME] = 'ALLOW_CAT_TICKETS_BY_AGENT' AND [BUSINESS_UNIT] = @BusinessUnit

-- [tbl_control_text_lang]
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = '<<FullName>> (<<UserName>>)' WHERE [TEXT_CODE] = 'AgentAndCustomerDetailsLabel' AND [CONTROL_CODE] = 'AgentPersonalisation.ascx' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = '<<AgentName>>' WHERE [TEXT_CODE] = 'AgentDetailsLabel' AND [CONTROL_CODE] = 'AgentPersonalisation.ascx' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = '' WHERE [BUSINESS_UNIT] = @BusinessUnit AND [CONTROL_CODE] = 'ProductDetail.ascx' AND [TEXT_CODE] LIKE 'sponsoredbyTextForProductType%' 
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = '' WHERE [BUSINESS_UNIT] = @BusinessUnit AND [CONTROL_CODE] = 'ProductDetail.ascx' AND [TEXT_CODE] LIKE '%Label' 
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = '' WHERE [BUSINESS_UNIT] = @BusinessUnit AND [CONTROL_CODE] = 'ProductDetail.ascx' AND [TEXT_CODE] LIKE '%LabelText' 
UPDATE [tbl_control_text_lang] SET [CONTROL_CONTENT] = 'Select Stand' WHERE (TEXT_CODE = 'standDDLPleaseSelectLabel') AND (CONTROL_CODE = 'StandAndAreaSelection.ascx')


-- [tbl_club_details]
DELETE FROM [tbl_club_details]
INSERT INTO [tbl_club_details] ([CLUB_CODE],[CLUB_DESCRIPTION],[DISPLAY_SEQUENCE],[IS_DEFAULT],[CUSTOMER_VALIDATION_URL],[VALID_CUSTOMER_FORWARD_URL],[INVALID_CUSTOMER_FORWARD_URL],[NOISE_ENCRYPTION_KEY]
           ,[SUPPLYNET_LOGINID],[SUPPLYNET_PASSWORD],[SUPPLYNET_COMPANY],[AGENT_TYPE],[STANDARD_CUSTOMER_FORWARD_URL]) 
VALUES ('BOXOFFICE','Box Office','1',1,'','/Redirect/TicketingGateway.aspx?page=validatesession.aspx&function=validatesession&returnurl=%7E%2FPagesPublic%2FProductBrowse%2FProductHome.aspx',
           '/Redirect/TicketingGateway.aspx?page=validatesession.aspx&function=validatesession&returnurl=%7E%2FPagesPublic%2FProfile%2FRegistration.aspx','','','','',
           '1','/Redirect/TicketingGateway.aspx?page=validatesession.aspx&function=validatesession&returnurl=%7E%2FPagesPublic%2FProductBrowse%2FProductHome.aspx')
INSERT INTO [tbl_club_details] ([CLUB_CODE],[CLUB_DESCRIPTION],[DISPLAY_SEQUENCE],[IS_DEFAULT],[CUSTOMER_VALIDATION_URL],[VALID_CUSTOMER_FORWARD_URL],[INVALID_CUSTOMER_FORWARD_URL],[NOISE_ENCRYPTION_KEY]
           ,[SUPPLYNET_LOGINID],[SUPPLYNET_PASSWORD],[SUPPLYNET_COMPANY],[AGENT_TYPE],[STANDARD_CUSTOMER_FORWARD_URL])
VALUES ('BOXOFFICE','Box Office','1',1,'','/Redirect/TicketingGateway.aspx?page=validatesession.aspx&function=validatesession&returnurl=%7E%2FPagesPublic%2FProductBrowse%2FProductHome.aspx',
           '/Redirect/TicketingGateway.aspx?page=validatesession.aspx&function=validatesession&returnurl=%7E%2FPagesPublic%2FProfile%2FRegistration.aspx','','','','',
           '2','/Redirect/TicketingGateway.aspx?page=validatesession.aspx&function=validatesession&returnurl=%7E%2FPagesPublic%2FProductBrowse%2FProductHome.aspx')
UPDATE [tbl_club_details] SET [NOISE_ENCRYPTION_KEY] = (SELECT TOP(1) [VALUE] FROM [tbl_ecommerce_module_defaults_bu] WHERE [DEFAULT_NAME] = 'NOISE_ENCRYPTION_KEY')


-- [tbl_page]
UPDATE [tbl_page] SET [SHOW_PAGE_HEADER] = 0 WHERE BUSINESS_UNIT = @BusinessUnit
UPDATE [tbl_page] SET [FORCE_LOGIN] = 0, [IN_USE] = 1 WHERE BUSINESS_UNIT = @BusinessUnit AND [PAGE_CODE] = 'CustomerSelection.aspx'
UPDATE [tbl_page] SET [FORCE_LOGIN] = 0, [IN_USE] = 1, [ALLOW_GENERIC_SALES] = 1 WHERE BUSINESS_UNIT = @BusinessUnit AND [PAGE_CODE] = 'TransactionEnquiry.aspx'
UPDATE [tbl_page] SET [FORCE_LOGIN] = 0, [IN_USE] = 1, [ALLOW_GENERIC_SALES] = 1 WHERE BUSINESS_UNIT = @BusinessUnit AND [PAGE_CODE] = 'PurchaseDetails.aspx'
UPDATE [tbl_page] SET [FORCE_LOGIN] = 0, [IN_USE] = 1, [ALLOW_GENERIC_SALES] = 1 WHERE BUSINESS_UNIT = @BusinessUnit AND [PAGE_CODE] = 'Checkout.aspx'


-- [tbl_payment_type_bu]
UPDATE [tbl_payment_type_bu] SET [AGENT_TICKETING_TYPE] = 1, [AGENT_RETAIL_TYPE] = 1, [AGENT_TICKETING_TYPE_REFUND] = 1, [GENERIC_SALES] = 1 WHERE [PAYMENT_TYPE_CODE] = 'CC' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [AGENT_TICKETING_TYPE] = 1, [AGENT_RETAIL_TYPE] = 0, [AGENT_TICKETING_TYPE_REFUND] = 1, [GENERIC_SALES] = 1 WHERE [PAYMENT_TYPE_CODE] = 'CS' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [AGENT_TICKETING_TYPE] = 0, [AGENT_RETAIL_TYPE] = 0, [AGENT_TICKETING_TYPE_REFUND] = 0, [GENERIC_SALES] = 0 WHERE [PAYMENT_TYPE_CODE] = 'DD' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [AGENT_TICKETING_TYPE] = 1, [AGENT_RETAIL_TYPE] = 0, [AGENT_TICKETING_TYPE_REFUND] = 1, [GENERIC_SALES] = 0 WHERE [PAYMENT_TYPE_CODE] = 'EP' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [AGENT_TICKETING_TYPE] = 1, [AGENT_RETAIL_TYPE] = 0, [AGENT_TICKETING_TYPE_REFUND] = 1, [GENERIC_SALES] = 1 WHERE [PAYMENT_TYPE_CODE] = 'CP' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [AGENT_TICKETING_TYPE] = 0, [AGENT_RETAIL_TYPE] = 0, [AGENT_TICKETING_TYPE_REFUND] = 1, [GENERIC_SALES] = 0 WHERE [PAYMENT_TYPE_CODE] = 'CP' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [AGENT_TICKETING_TYPE] = 0, [AGENT_RETAIL_TYPE] = 0, [AGENT_TICKETING_TYPE_REFUND] = 0, [GENERIC_SALES] = 0 WHERE [PAYMENT_TYPE_CODE] = 'GC' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [AGENT_TICKETING_TYPE] = 0, [AGENT_RETAIL_TYPE] = 0, [AGENT_TICKETING_TYPE_REFUND] = 0, [GENERIC_SALES] = 0 WHERE [PAYMENT_TYPE_CODE] = 'CF' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [AGENT_TICKETING_TYPE] = 0, [AGENT_RETAIL_TYPE] = 0, [AGENT_TICKETING_TYPE_REFUND] = 0, [GENERIC_SALES] = 0 WHERE [PAYMENT_TYPE_CODE] = 'PD' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [AGENT_TICKETING_TYPE] = 0, [AGENT_RETAIL_TYPE] = 0, [AGENT_TICKETING_TYPE_REFUND] = 0, [GENERIC_SALES] = 0 WHERE [PAYMENT_TYPE_CODE] = 'CQ' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [AGENT_TICKETING_TYPE] = 0, [AGENT_RETAIL_TYPE] = 0, [AGENT_TICKETING_TYPE_REFUND] = 0, [GENERIC_SALES] = 0 WHERE [PAYMENT_TYPE_CODE] = 'PO' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [AGENT_TICKETING_TYPE] = 0, [AGENT_RETAIL_TYPE] = 0, [AGENT_TICKETING_TYPE_REFUND] = 0, [GENERIC_SALES] = 0 WHERE [PAYMENT_TYPE_CODE] = 'PP' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [AGENT_TICKETING_TYPE] = 1, [AGENT_RETAIL_TYPE] = 0, [AGENT_TICKETING_TYPE_REFUND] = 0, [GENERIC_SALES] = 0 WHERE [PAYMENT_TYPE_CODE] = 'ZB' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [TICKETING_TYPE] = 1 WHERE [PAYMENT_TYPE_CODE] = 'CS' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_payment_type_bu] SET [TICKETING_TYPE] = 1 WHERE [PAYMENT_TYPE_CODE] = 'ZB' AND [BUSINESS_UNIT] = @BusinessUnit


-- [tbl_control_attribute]
INSERT INTO [tbl_control_attribute] ([BUSINESS_UNIT] ,[PARTNER_CODE] ,[PAGE_CODE] ,[CONTROL_CODE]  ,[ATTR_NAME] ,[ATTR_VALUE] ,[DESCRIPTION])
     VALUES (@BusinessUnit ,'*ALL' ,'*ALL' ,'ProductDetail.ascx' ,'numberOfProductsVariable' ,'99' ,'Number of Products per Page')
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = '~/PagesPublic/Profile/CustomerSelection.aspx?source=customerselect&displayMode=basket' WHERE [ATTR_NAME] = 'NewCustomerUrl' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'Allow_Cancel_By_Agent' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'Allow_Transfer_By_Agent' AND [BUSINESS_UNIT] = @BusinessUnit
UPDATE [tbl_control_attribute] SET [ATTR_VALUE] = 'True' WHERE [ATTR_NAME] = 'Allow_Amend_By_Agent' AND [BUSINESS_UNIT] = @BusinessUnit


-- [tbl_error_messages]
INSERT INTO [tbl_error_messages] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[MODULE],[PAGE_CODE],[ERROR_CODE],[ERROR_MESSAGE]) 
	SELECT [LANGUAGE_CODE],'BOXOFFICE',[PARTNER_CODE],[MODULE],[PAGE_CODE],[ERROR_CODE],[ERROR_MESSAGE] 
	FROM [tbl_error_messages] WHERE [BUSINESS_UNIT] = 'UNITEDKINGDOM'
	AND [ERROR_CODE] NOT IN (SELECT [ERROR_CODE] FROM [tbl_error_messages] where [BUSINESS_UNIT] = 'BOXOFFICE')