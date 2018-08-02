-- =================================================================
-- Reset template tables to use the correct master page templates
-- =================================================================


-- Set all BUSINESS_UNIT to *ALL
UPDATE [tbl_template_page] SET [BUSINESS_UNIT] = '*ALL';


-- Delete duplicate records
WITH CTE AS(
   SELECT [BUSINESS_UNIT], [PARTNER], [PAGE_NAME], [TEMPLATE_NAME],
       RN = ROW_NUMBER()OVER(PARTITION BY [PAGE_NAME] ORDER BY [PAGE_NAME])
   FROM [tbl_template_page]
)
DELETE FROM CTE WHERE RN > 1


-- Set all pages to use '2Column' master page
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '2Column'


-- Set specific pages to use specific master pages
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'SeatSelection.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'VisualSeatSelection.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'StandAndAreaSelection.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'Checkout.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'AdditionalProductInformation.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'checkoutOrderConfirmation.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'Basket.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'PurchaseHistory.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'PurchaseDetails.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'Vouchers.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'myAccount.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'Personalise.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'Home.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'AgentLogin.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'LinkedProducts.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'ProfileActivities.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'EditProfileActivity.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = '1Column' WHERE [PAGE_NAME] = 'AgentPreferences.aspx'

UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Retail' WHERE [PAGE_NAME] = 'Personalisation.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Retail' WHERE [PAGE_NAME] = 'browse01.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Retail' WHERE [PAGE_NAME] = 'browse02.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Retail' WHERE [PAGE_NAME] = 'browse03.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Retail' WHERE [PAGE_NAME] = 'browse04.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Retail' WHERE [PAGE_NAME] = 'browse05.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Retail' WHERE [PAGE_NAME] = 'browse06.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Retail' WHERE [PAGE_NAME] = 'browse07.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Retail' WHERE [PAGE_NAME] = 'browse08.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Retail' WHERE [PAGE_NAME] = 'browse09.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Retail' WHERE [PAGE_NAME] = 'browse10.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Retail' WHERE [PAGE_NAME] = 'product.aspx'

UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Blank' WHERE [PAGE_NAME] = 'ClearCache.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Blank' WHERE [PAGE_NAME] = 'MicroBasket.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Blank' WHERE [PAGE_NAME] = 'TradePlaceLogin.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Blank' WHERE [PAGE_NAME] = 'SapOciLogin.aspx'

UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'MatchDayHospitalitySummary.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'Comments.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'RememberMeWindow.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'ProductSummary.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'PaymentDetailsPopup.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'ApplyCashback.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'SeatHistory.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'SeatPrintHistory.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'PromotionDetails.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'Alerts.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'ApplyCashback.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'StopCodeAudit.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'HopewiserPostcodeAndCountry.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'HopewiserSearch.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'Reveal' WHERE [PAGE_NAME] = 'DatePicker.aspx'

UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'QASContent' WHERE [PAGE_NAME] = 'FlatAddress.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'QASContent' WHERE [PAGE_NAME] = 'FlatCountry.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'QASContent' WHERE [PAGE_NAME] = 'FlatPrompt.aspx'
UPDATE [tbl_template_page] SET [TEMPLATE_NAME] = 'QASContent' WHERE [PAGE_NAME] = 'FlatSearch.aspx'


-- Correct tbl_template records
DELETE FROM tbl_template
INSERT INTO [tbl_template] ([TEMPLATE_NAME],[TEMPLATE_DESC],[DEFAULT_FLAG]) VALUES ('1Column','Responsive layout with 1 column of content','N')
INSERT INTO [tbl_template] ([TEMPLATE_NAME],[TEMPLATE_DESC],[DEFAULT_FLAG]) VALUES ('2Column','Responsive layout with 2 column of content','Y')
INSERT INTO [tbl_template] ([TEMPLATE_NAME],[TEMPLATE_DESC],[DEFAULT_FLAG]) VALUES ('Blank','Blank page','N')
INSERT INTO [tbl_template] ([TEMPLATE_NAME],[TEMPLATE_DESC],[DEFAULT_FLAG]) VALUES ('DatePicker','Date Picker layout','N')
INSERT INTO [tbl_template] ([TEMPLATE_NAME],[TEMPLATE_DESC],[DEFAULT_FLAG]) VALUES ('Modal','Responsive Modal Window','N')
INSERT INTO [tbl_template] ([TEMPLATE_NAME],[TEMPLATE_DESC],[DEFAULT_FLAG]) VALUES ('QASContent','QAS bespoke layout','N')