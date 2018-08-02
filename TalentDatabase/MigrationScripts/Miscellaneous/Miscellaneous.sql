-- Clear agent table
TRUNCATE TABLE tbl_agent

-- Clear basket tables
TRUNCATE TABLE tbl_basket_header
TRUNCATE TABLE tbl_basket_detail
TRUNCATE TABLE tbl_basket_fees
TRUNCATE TABLE tbl_basket_promotion_items

-- From Version 2013.002.073
UPDATE tbl_basket_header SET CAT_MODE = '' WHERE CAT_MODE IS NULL;

--reseed basket header based on server name
IF (CHARINDEX('EBUSINESS', UPPER(@@SERVERNAME)) > 0)
BEGIN
	DBCC CHECKIDENT ('tbl_basket_header', RESEED, 10000000);
END
IF (CHARINDEX('TEST', UPPER(@@SERVERNAME)) > 0)
BEGIN
	DBCC CHECKIDENT ('tbl_basket_header', RESEED, 70000000);
END
IF (CHARINDEX('STAGE', UPPER(@@SERVERNAME)) > 0)
BEGIN
	DBCC CHECKIDENT ('tbl_basket_header', RESEED, 90000000);
END

-- This updates the font awesome icons on the website to prevent "?" showing
UPDATE [tbl_control_text_lang]
   SET [CONTROL_CONTENT] = N''
 WHERE [CONTROL_CODE] = 'TicketingBasketDetails.ascx' AND [TEXT_CODE] = 'RemoveButton'
 
UPDATE [tbl_control_text_lang]
   SET [CONTROL_CONTENT] = N''
 WHERE [CONTROL_CODE] = 'AdhocFees.ascx' AND [TEXT_CODE] = 'AdhocFeesRemoveButtonText'
 
UPDATE [tbl_control_text_lang]
   SET [CONTROL_CONTENT] = N''
 WHERE [CONTROL_CODE] = 'TicketingBasketDetails.ascx' AND [TEXT_CODE] = 'SaveFavouriteSeatButtonText'
 
UPDATE [tbl_control_text_lang]
   SET [CONTROL_CONTENT] = N''
 WHERE [CONTROL_CODE] = 'Alerts.ascx' AND [TEXT_CODE] = 'RemoveThisAlertButtonText'
 
UPDATE [tbl_control_text_lang]
   SET [CONTROL_CONTENT] = N''
 WHERE [CONTROL_CODE] = 'ComponentSeats.ascx' AND [TEXT_CODE] = 'lblRemove'

-- Ensure that the clear cache page doesn't inherit any site master pages, it always needs to use blank.master
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Blank' WHERE [PAGE_NAME] = 'ClearCache.aspx'

 -- Vanguard frontend gateway
 -- Removing this from boxoffice as per MBSTS-7886 "Keyed Cardholder Not Present Ecommerce Order" only allowed for PWS 
 DELETE FROM [tbl_defaults] WHERE APPLY_TYPE = 'VANGUARD_CAPTUREMETHOD' AND APPLY_CODE = '12' AND BUSINESS_UNIT = 'BOXOFFICE'

 -- Common on account fee code insert if it doesn't exist
IF ((SELECT COUNT(*) FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'ONACCT') = 0)
BEGIN
	INSERT INTO [dbo].[tbl_control_text_lang]
    ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE]) 
	VALUES ('ENG','*ALL','*ALL','*ALL','BasketSummary.ascx','ONACCT','On Account',0)
END

-- Missing text for Roving descriptions
IF ((SELECT COUNT(*) FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'RovingAreaText' AND [CONTROL_CODE] = 'ComponentSeats.ascx') = 0)
BEGIN
	INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
    VALUES ('ENG','*ALL','*ALL','*ALL','ComponentSeats.ascx','RovingAreaText','Roving',0)
END
IF ((SELECT COUNT(*) FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'RovingAreaText' AND [CONTROL_CODE] = 'StandAndAreaSelection.ascx') = 0)
BEGIN
	INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
    VALUES ('ENG','*ALL','*ALL','*ALL','StandAndAreaSelection.ascx','RovingAreaText','Roving',0)
END
IF ((SELECT COUNT(*) FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'RovingAreaText' AND [CONTROL_CODE] = 'TicketingBasketDetails.ascx') = 0)
BEGIN
	INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
    VALUES ('ENG','*ALL','*ALL','*ALL','TicketingBasketDetails.ascx','RovingAreaText','Roving',0)
END
IF ((SELECT COUNT(*) FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'RovingAreaText' AND [CONTROL_CODE] = 'TransactionDetails.ascx') = 0)
BEGIN
	INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
    VALUES ('ENG','*ALL','*ALL','*ALL','TransactionDetails.ascx','RovingAreaText','Roving',0)
END