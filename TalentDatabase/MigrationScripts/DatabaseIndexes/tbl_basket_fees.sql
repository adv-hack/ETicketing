--Added on Version="2015" Release="2" Build="25"
-- Remove existing index if it exists
IF EXISTS (SELECT * FROM sys.indexes WHERE NAME ='BSKTHEAD_EXCLUD_CCT_FCAT_FCODE' AND OBJECT_ID = OBJECT_ID('tbl_basket_fees'))
BEGIN
	DROP INDEX BSKTHEAD_EXCLUD_CCT_FCAT_FCODE on tbl_basket_fees
END
GO

-- Add the new index if it doesn't exist already
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='idx_BsktHead_Exclud_CCT_FCat_FCode' AND OBJECT_ID = OBJECT_ID('tbl_basket_fees'))
BEGIN

	-- Create the new index
	CREATE NONCLUSTERED INDEX [idx_BsktHead_Exclud_CCT_FCat_FCode] ON [dbo].[tbl_basket_fees]
	(
	  [BASKET_HEADER_ID] ASC,
	  [IS_EXCLUDED] ASC,
	  [CARD_TYPE_CODE] ASC,
	  [FEE_CATEGORY] ASC,
	  [FEE_CODE] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
END
GO

