/* Page Headers */

UPDATE [dbo].[tbl_page]
   SET [SHOW_PAGE_HEADER] = 'True'
 WHERE [BUSINESS_UNIT] = 'BOXOFFICE'

UPDATE [dbo].[tbl_page]
   SET [SHOW_PAGE_HEADER] = 'False'
 WHERE [BUSINESS_UNIT] = 'BOXOFFICE' AND [PAGE_CODE] = 'VisualSeatSelection.aspx'

UPDATE [dbo].[tbl_page]
   SET [SHOW_PAGE_HEADER] = 'False'
 WHERE [PAGE_CODE] = 'hopewiser'

UPDATE [dbo].[tbl_page]
   SET [SHOW_PAGE_HEADER] = 'False'
	  ,[BODY_CSS_CLASS] = 'AgentLogin'
 WHERE [BUSINESS_UNIT] = 'BOXOFFICE' AND [PAGE_CODE] = 'AgentLogin.aspx'

/* Templates */

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'CustomerSelection.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'AmendPPSPayments.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'FavouriteSeat.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'FriendsAndFamily.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'FavouriteSeat.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'Registration.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'ProfileMembership.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'ProfilePhoto.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'CustomerText.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'UpdateProfile.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'AmendPPSEnrolment.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'OrderReturnEnquiry.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'OrderReturn.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'DirectDebitDetails.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'OnAccountAdjustment.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'ProfileAttributes.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'CourseDetails.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'EndOfDay.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'TransactionEnquiry.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'TransactionEnquiry.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'TicketCollection.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'TransactionEnquiry.aspx'
 
UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'PromotionHistory.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'OnAccount.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'OnAccountAdjustment.aspx'

UPDATE [dbo].[tbl_template_page]
   SET [TEMPLATE_NAME] = '1Column'
 WHERE [PAGE_NAME] = 'PromotionHistoryDetail.aspx'