if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_Basket_GetHeaderIDBySupplyNetSessionID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
drop procedure [dbo].[usp_Basket_GetHeaderIDBySupplyNetSessionID]
end
go

CREATE PROCEDURE [dbo].[usp_Basket_GetHeaderIDBySupplyNetSessionID]
(
	@pa_SessionID NVARCHAR(80) = '',
	@pa_IsAnonymous BIT = 0,
    @pa_IsAuthenticated BIT = 0,
	@pa_LoginID NVARCHAR(100) = '',
	@pa_BusinessUnit NVARCHAR(50) = '',
	@pa_Partner NVARCHAR(50) = '',
	@pa_Processed BIT = 0,
	@pa_StockError BIT = 0,
	@pa_MarketingCampaign BIT = 0,
	@pa_UserSelectFulfilment NVARCHAR(10) = '',
	@pa_PaymentOptions NVARCHAR(32) = '',
	@pa_RestrictPaymentOptions BIT= 0,
	@pa_CampaignCode NVARCHAR(64) = '',
	@pa_PaymentAccountID NVARCHAR(12) = '',
	@pa_ExternalPaymentToken NVARCHAR(200) = '',
	@pa_CatMode VARCHAR(5) = '',
	@pa_CatPrice VARCHAR(20) = ''
)
AS
BEGIN
	SET NOCOUNT ON
		DECLARE @basket_Header_ID BIGINT
		IF NOT EXISTS (SELECT BASKET_HEADER_ID FROM tbl_basket_header WITH (NOLOCK) WHERE (SESSION_ID = @pa_SessionID) AND (BUSINESS_UNIT = @pa_BusinessUnit) AND (PROCESSED = 0))
		BEGIN
			INSERT INTO tbl_basket_header
				(BUSINESS_UNIT, PARTNER, LOGINID, CREATED_DATE, LAST_ACCESSED_DATE, PROCESSED, STOCK_ERROR, MARKETING_CAMPAIGN, USER_SELECT_FULFIL, PAYMENT_OPTIONS, 
					RESTRICT_PAYMENT_OPTIONS, CAMPAIGN_CODE, PAYMENT_ACCOUNT_ID, EXTERNAL_PAYMENT_TOKEN, CAT_MODE, CAT_PRICE, SESSION_ID)
				VALUES     
					(@pa_BusinessUnit, @pa_Partner, @pa_LoginID, GETDATE(), GETDATE(), @pa_Processed, @pa_StockError, @pa_MarketingCampaign, @pa_UserSelectFulfilment, @pa_PaymentOptions, 
						@pa_RestrictPaymentOptions, @pa_CampaignCode, @pa_PaymentAccountID, @pa_ExternalPaymentToken, @pa_CatMode, @pa_CatPrice, @pa_SessionID)
		END
		UPDATE tbl_basket_header SET LAST_ACCESSED_DATE = GETDATE() WHERE (SESSION_ID = @pa_SessionID) AND (BUSINESS_UNIT = @pa_BusinessUnit) AND (PROCESSED = 0)
	SET NOCOUNT OFF
	SELECT BASKET_HEADER_ID FROM tbl_basket_header WITH (NOLOCK) WHERE (SESSION_ID = @pa_SessionID) AND (BUSINESS_UNIT = @pa_BusinessUnit) AND (PROCESSED = 0)
END