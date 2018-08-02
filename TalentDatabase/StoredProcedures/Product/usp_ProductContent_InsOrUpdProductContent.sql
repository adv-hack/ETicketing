if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_ProductContent_InsOrUpdProductContent]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[usp_ProductContent_InsOrUpdProductContent]
GO

	CREATE PROCEDURE  [dbo].[usp_ProductContent_InsOrUpdProductContent]
	(
		@LANGUAGE_CODE VARCHAR(20)  = null,
		@BUSINESS_UNIT VARCHAR(30)  = null,
		@PRODUCT_CODE nvarchar(50)  = null,
		@PACKAGE_ID nvarchar(50)	= null,
		@DATA ProductList READONLY, 
		@ERROR_CODE INT OUTPUT,
		@ERROR_MESSAGE VARCHAR(MAX) OUTPUT
	)	
	AS
	BEGIN 
			DECLARE	@ROWCOUNT int
			DECLARE @dup_count int
			SET @ERROR_CODE = 0 
			SET @ERROR_MESSAGE = ''

			BEGIN TRAN
					-- insert 
					INSERT	INTO tbl_product_specific_content
					SELECT	@LANGUAGE_CODE, @BUSINESS_UNIT, d.CONTENT_TYPE, @PRODUCT_CODE, @PACKAGE_ID, ISNULL(d.PRODUCT_CONTENT, '')
					FROM	@data d
					WHERE	
					d.PRODUCT_CONTENT_ID IS NULL	
	
					IF(@@ERROR > 0) GOTO error;
					ELSE SELECT @ROWCOUNT = @@ROWCOUNT

					-- update 
					UPDATE		tbl_product_specific_content 
					SET			PRODUCT_CONTENT = ISNULL(b.PRODUCT_CONTENT, '')
					FROM		tbl_product_specific_content AS a 
								INNER JOIN  @data As b on a.PRODUCT_CONTENT_ID = b.PRODUCT_CONTENT_ID	
					IF(@@ERROR > 0) GOTO error;
					ELSE SELECT @ROWCOUNT = @@ROWCOUNT

			--ROLLBACK
			COMMIT 
			SELECT PRODUCT_CONTENT_ID, CONTENT_TYPE, PRODUCT_CONTENT from tbl_product_specific_content WHERE ISNULL(PRODUCT_CODE, '')=ISNULL(@PRODUCT_CODE, '') AND ISNULL(PACKAGE_ID, '')=ISNULL(@PACKAGE_ID, '')
			error:
				SELECT @ERROR_CODE = ERROR_NUMBER(),@ERROR_MESSAGE = @ERROR_MESSAGE
	END 
GO


