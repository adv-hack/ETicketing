-- ==========================================================================================
-- Migrate old product questions records to activity questions records
-- ==========================================================================================
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_activity_questions')) 
AND (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_product_questions'))
BEGIN
	IF ((SELECT COUNT(*) FROM [tbl_product_questions]) > 0)
	BEGIN
		SET IDENTITY_INSERT [tbl_activity_questions] ON;

		INSERT INTO [tbl_activity_questions]
				   ([QUESTION_ID]
				   ,[QUESTION_TEXT]
				   ,[ANSWER_TYPE]
				   ,[ALLOW_SELECT_OTHER_OPTION]
				   ,[MANDATORY]
				   ,[PRICE_BAND_LIST]
				   ,[REGULAR_EXPRESSION]
				   ,[HYPERLINK]
				   ,[REMEMBERED_ANSWER])
			 SELECT [QUESTION_ID]
				   ,[QUESTION_TEXT]
				   ,[ANSWER_TYPE]
				   ,[ALLOW_SELECT_OTHER_OPTION]
				   ,[MANDATORY]
				   ,[PRICE_BAND_LIST]
				   ,[REGULAR_EXPRESSION]
				   ,[HYPERLINK]
				   ,0 FROM [tbl_product_questions];
	
		SET IDENTITY_INSERT [tbl_activity_questions] OFF;
	END

	DROP TABLE [tbl_product_questions];
	IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_product_questions_work')) 
	BEGIN
		DROP TABLE [tbl_product_questions_work];
	END
END