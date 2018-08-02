-- ==========================================================================================
-- Migrate old product questions records to activity questions records
-- ==========================================================================================
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_activity_questions_with_answers')) 
AND (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_product_questions_with_answers'))
BEGIN
	IF ((SELECT COUNT(*) FROM [tbl_product_questions_with_answers]) > 0)
	BEGIN
		SET IDENTITY_INSERT [tbl_activity_questions_with_answers] ON;

		INSERT INTO [dbo].[tbl_activity_questions_with_answers] ([ID],[QUESTION_ID],[ANSWER_ID])
		SELECT [ID],[QUESTION_ID],[ANSWER_ID] FROM [tbl_product_questions_with_answers];
	
		SET IDENTITY_INSERT [tbl_activity_questions_with_answers] OFF;
	END

	DROP TABLE [tbl_product_questions_with_answers];
	IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_product_questions_with_answers_work')) 
	BEGIN
		DROP TABLE [tbl_product_questions_with_answers_work];
	END
END