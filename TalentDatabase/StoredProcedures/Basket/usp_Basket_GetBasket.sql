if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_Basket_GetBasket]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
drop procedure [dbo].[usp_Basket_GetBasket]
end
go

CREATE PROCEDURE [dbo].[usp_Basket_GetBasket]
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
	@pa_CatMode VARCHAR(5) = ''
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @loginid NVARCHAR(100), @basket_Header_ID BIGINT, @isNewBasketGenerated BIT
	SET @isNewBasketGenerated = 0
	--not authenticated
	IF (@pa_IsAuthenticated = 0)
		BEGIN
			IF (@pa_IsAnonymous = 1)
				BEGIN
					DECLARE @anonymous_Basket_Header_ID BIGINT
					SET @loginid = NULL
					SELECT TOP 1 @anonymous_Basket_Header_ID = BASKET_HEADER_ID, @loginid = LOGINID FROM tbl_basket_header WITH (NOLOCK) WHERE (SESSION_ID = @pa_SessionID) AND (BUSINESS_UNIT = @pa_BusinessUnit) AND (PROCESSED = 0)
					IF @loginid IS NOT NULL
					BEGIN
						-- same session id but in different business unit don't enable the below line check with karthik 
						-- DELETE tbl_basket_header WHERE SESSION_ID = @pa_SessionID AND (BUSINESS_UNIT <> @pa_BusinessUnit) AND (PROCESSED = 0)
						IF @loginid <> @pa_LoginID
						BEGIN
							--same login id but in different basket header id
							DELETE tbl_basket_header WHERE (SESSION_ID = @pa_SessionID) AND (BUSINESS_UNIT = @pa_BusinessUnit) AND (PROCESSED = 0) AND BASKET_HEADER_ID <> @anonymous_Basket_Header_ID
							UPDATE tbl_basket_header SET LOGINID = @pa_LoginID WHERE SESSION_ID = @pa_SessionID AND (BUSINESS_UNIT = @pa_BusinessUnit) AND (PROCESSED = 0)
						END
					END
				END -- is anonymous if ends
			SET @loginid = NULL
			SELECT @loginid = LOGINID FROM tbl_basket_header WITH (NOLOCK) WHERE (LOGINID = @pa_LoginID) AND (BUSINESS_UNIT = @pa_BusinessUnit) AND (PROCESSED = 0)
			IF @loginid IS NULL
				BEGIN
					-- create new basket
					INSERT INTO tbl_basket_header
					   (BUSINESS_UNIT, PARTNER, LOGINID, CREATED_DATE, LAST_ACCESSED_DATE, PROCESSED, STOCK_ERROR, MARKETING_CAMPAIGN, USER_SELECT_FULFIL, PAYMENT_OPTIONS, 
							RESTRICT_PAYMENT_OPTIONS, CAMPAIGN_CODE, PAYMENT_ACCOUNT_ID, EXTERNAL_PAYMENT_TOKEN, CAT_MODE, SESSION_ID, ORIGINAL_SALE_PAID_WITH_CF)
					   VALUES     
					   (@pa_BusinessUnit, @pa_Partner, @pa_LoginID, GETDATE(), GETDATE(), @pa_Processed, @pa_StockError, @pa_MarketingCampaign, @pa_UserSelectFulfilment, @pa_PaymentOptions, 
							@pa_RestrictPaymentOptions, @pa_CampaignCode, @pa_PaymentAccountID, @pa_ExternalPaymentToken, @pa_CatMode, @pa_SessionID, 0)
					SET @isNewBasketGenerated = 1
				END
			UPDATE tbl_basket_header SET LAST_ACCESSED_DATE = GETDATE() WHERE (LOGINID = @pa_LoginID) AND (BUSINESS_UNIT = @pa_BusinessUnit) AND (PROCESSED = 0)
			SELECT @basket_Header_ID = BASKET_HEADER_ID FROM tbl_basket_header WITH (NOLOCK) WHERE (LOGINID = @pa_LoginID) AND (BUSINESS_UNIT = @pa_BusinessUnit) AND (PROCESSED = 0)
		END
	-- authenticated user
	IF (@pa_IsAuthenticated = 1)
		BEGIN
			SET @loginid = NULL
			SELECT @loginid = LOGINID FROM tbl_basket_header WITH (NOLOCK) WHERE (BUSINESS_UNIT = @pa_BusinessUnit) AND ([PARTNER] = @pa_Partner) AND (LOGINID = @pa_LoginID) AND (PROCESSED = 0)
			IF @loginid IS NULL
				BEGIN
					-- if login id is null, try and get the basket header record by session
					SELECT @loginid = LOGINID FROM tbl_basket_header WITH (NOLOCK) WHERE (BUSINESS_UNIT = @pa_BusinessUnit) AND (SESSION_ID = @pa_SessionID) AND (PROCESSED = 0)
					IF @loginid IS NULL
						BEGIN
							-- if no record is found by session or login id then create new basket
							INSERT INTO tbl_basket_header
								(BUSINESS_UNIT, PARTNER, LOGINID, CREATED_DATE, LAST_ACCESSED_DATE, PROCESSED, STOCK_ERROR, MARKETING_CAMPAIGN, USER_SELECT_FULFIL, PAYMENT_OPTIONS, 
									RESTRICT_PAYMENT_OPTIONS, CAMPAIGN_CODE, PAYMENT_ACCOUNT_ID, EXTERNAL_PAYMENT_TOKEN, CAT_MODE, SESSION_ID, ORIGINAL_SALE_PAID_WITH_CF)
								VALUES     
								(@pa_BusinessUnit, @pa_Partner, @pa_LoginID, GETDATE(), GETDATE(), @pa_Processed, @pa_StockError, @pa_MarketingCampaign, @pa_UserSelectFulfilment, @pa_PaymentOptions, 
									@pa_RestrictPaymentOptions, @pa_CampaignCode, @pa_PaymentAccountID, @pa_ExternalPaymentToken, @pa_CatMode, @pa_SessionID, 0)
							SET @isNewBasketGenerated = 1
						END
					ELSE
						BEGIN
							-- record found update the basket header record
							UPDATE tbl_basket_header SET 
							PARTNER = @pa_Partner, LOGINID = @pa_LoginID, STOCK_ERROR = @pa_StockError, MARKETING_CAMPAIGN = @pa_MarketingCampaign, USER_SELECT_FULFIL = @pa_UserSelectFulfilment, PAYMENT_OPTIONS = @pa_PaymentOptions,
							RESTRICT_PAYMENT_OPTIONS = @pa_RestrictPaymentOptions, CAMPAIGN_CODE = @pa_CampaignCode, PAYMENT_ACCOUNT_ID = @pa_PaymentAccountID, EXTERNAL_PAYMENT_TOKEN = @pa_ExternalPaymentToken, CAT_MODE = @pa_CatMode, ORIGINAL_SALE_PAID_WITH_CF = 0
							WHERE (BUSINESS_UNIT = @pa_BusinessUnit) AND (SESSION_ID = @pa_SessionID) AND (PROCESSED = 0) AND (LOGINID = @loginid)
						END
				END
			UPDATE tbl_basket_header SET LAST_ACCESSED_DATE = GETDATE(), SESSION_ID = @pa_SessionID WHERE (BUSINESS_UNIT = @pa_BusinessUnit) AND ([PARTNER] = @pa_Partner) AND (LOGINID = @pa_LoginID) AND (PROCESSED = 0)
			SELECT @basket_Header_ID = BASKET_HEADER_ID FROM tbl_basket_header WITH (NOLOCK) WHERE (BUSINESS_UNIT = @pa_BusinessUnit) AND ([PARTNER] = @pa_Partner) AND (LOGINID = @pa_LoginID) AND (PROCESSED = 0)
		END
		SET NOCOUNT OFF
	SELECT @isNewBasketGenerated As IS_NEW_BASKET
	SELECT * FROM tbl_basket_header WITH (NOLOCK) WHERE (BASKET_HEADER_ID = @basket_Header_ID)
	SELECT * FROM tbl_basket_detail WITH (NOLOCK) WHERE (BASKET_HEADER_ID = @basket_Header_ID)
END