	IF (OBJECT_ID('usp_SearchText_SetData') IS NOT NULL)
	BEGIN 
		DROP PROCEDURE dbo.usp_SearchText_SetData
	END
	GO 

	CREATE PROCEDURE DBO.usp_SearchText_SetData(
		@XML XML,
		@ERROR_CODE INT OUTPUT, 
		@ERROR_MESSAGE VARCHAR(MAX) OUTPUT
	)
	AS
	BEGIN 
		SET NOCOUNT ON 

		/**************************************************************************
			DEBUG - START
		**************************************************************************/
			--DECLARE @XML XML,@ERROR_CODE INT,@ERROR_MESSAGE VARCHAR(MAX)

			--set @xml = '<data><tables><table id="1">"tbl_page_text_lang"</table><table id="2">"tbl_control_text_lang"</table></tables><rows><row tab="1" id="7594"><text_code>PasswordText</text_code><text_content>lblPassword:::</text_content></row></rows></data>' 

		/**************************************************************************
			DEBUG - END
		**************************************************************************/
		DECLARE @TBLTABLES TABLE(
			TABLE_ID TINYINT,
			TABLE_NAME VARCHAR(50)
		)
		DECLARE @TBLROWS TABLE(
			ROW_ID INT, 
			TABLE_ID INT, 
			TEXT_CODE NVARCHAR(50), 
			TEXT_CONTENT NVARCHAR(MAX)
		)
		DECLARE @IDOC INT
		
		-- init 
		SET @ERROR_CODE = 0 
		SET @ERROR_MESSAGE = '' 

		BEGIN TRY 
			-- prepare xml
			EXEC SP_XML_PREPAREDOCUMENT @IDOC OUTPUT, @XML;
	
			-- read tables 
			INSERT	INTO @TBLTABLES(TABLE_ID,TABLE_NAME)
			SELECT  ID,REPLACE(TABLE_NAME,'"','') AS TABLE_NAME
			FROM	OPENXML (@IDOC, '/data/tables/table',1) 
					WITH (
							ID VARCHAR(5) '@id',
							TABLE_NAME VARCHAR(MAX) '.'
						 )
	
			-- read rows
			INSERT INTO @TBLROWS(ROW_ID,TABLE_ID,TEXT_CODE,TEXT_CONTENT)
			SELECT  ROW_ID,TABLE_ID,TEXT_CODE,TEXT_CONTENT
			FROM	OPENXML (@IDOC, '/data/rows/row',1) 
					WITH (
							ROW_ID INT '@id',
							TABLE_ID INT '@tab',
							TEXT_CODE VARCHAR(50) 'text_code',
							TEXT_CONTENT VARCHAR(MAX) 'text_content'
						 )

			-- dispose xml 
			EXEC SP_XML_REMOVEDOCUMENT @IDOC
		END TRY 
		BEGIN CATCH 
			SELECT @ERROR_CODE = ERROR_NUMBER(), @ERROR_MESSAGE = ERROR_MESSAGE()
			SET @ERROR_MESSAGE = 'Error occurred while reading the xml data:' + @ERROR_MESSAGE
		END CATCH 
		
		-- speical characters conversion 
		UPDATE	@TBLROWS
		SET		TEXT_CONTENT = REPLACE(TEXT_CONTENT,'&amp;','&')
		WHERE	TEXT_CONTENT like '%&amp;%'

		UPDATE	@TBLROWS
		SET		TEXT_CONTENT = REPLACE(TEXT_CONTENT,'&lt;','<')
		WHERE	TEXT_CONTENT like '%&lt;%'

		UPDATE	@TBLROWS
		SET		TEXT_CONTENT = REPLACE(TEXT_CONTENT,'&gt;','>')
		WHERE	TEXT_CONTENT like '%&gt;%'

		-- do the actual work 
		BEGIN TRY 
			IF EXISTS(SELECT 1 FROM @TBLROWS)BEGIN 
				-- write sql queries here to update the records (one by one for each table)
				BEGIN TRAN 
					-- 1. TBL_PAGE_TEXT_LANG 
					UPDATE	DBO.TBL_PAGE_TEXT_LANG
					SET		TEXT_CODE = ISNULL(A.TEXT_CODE,C.TEXT_CODE),
							TEXT_CONTENT = ISNULL(A.TEXT_CONTENT,C.TEXT_CONTENT)
					FROM	@TBLROWS A 
							INNER JOIN @TBLTABLES B ON A.TABLE_ID = B.TABLE_ID 
							INNER JOIN DBO.TBL_PAGE_TEXT_LANG C ON C.ID = A.ROW_ID 
					WHERE	B.TABLE_NAME = 'TBL_PAGE_TEXT_LANG' 

					-- 2. TBL_CONTROL_TEXT_LANG 
					UPDATE	DBO.TBL_CONTROL_TEXT_LANG
					SET		TEXT_CODE = ISNULL(A.TEXT_CODE,C.TEXT_CODE),
							CONTROL_CONTENT = ISNULL(A.TEXT_CONTENT,C.CONTROL_CONTENT)
					FROM	@TBLROWS A 
							INNER JOIN @TBLTABLES B ON A.TABLE_ID = B.TABLE_ID 
							INNER JOIN DBO.TBL_CONTROL_TEXT_LANG C ON C.ID = A.ROW_ID 
					WHERE	B.TABLE_NAME = 'TBL_CONTROL_TEXT_LANG' 
				
				-- commit changes 
				COMMIT 	
			END
		END TRY 
		BEGIN CATCH 
			SELECT @ERROR_CODE = ERROR_NUMBER(), @ERROR_MESSAGE = ERROR_MESSAGE()
			SET @ERROR_MESSAGE = 'Error occurred while updating the data:' + @ERROR_MESSAGE
		END CATCH
	END 