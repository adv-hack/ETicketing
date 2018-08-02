-- ==========================================================================================
-- Migrate old product questions records to activity questions records
-- ==========================================================================================
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_activity_questions_answers')) 
AND (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_product_questions_answers'))
BEGIN
	IF ((SELECT COUNT(*) FROM [tbl_product_questions_templates]) > 0)
	BEGIN
		SET IDENTITY_INSERT tbl_activity_questions_answers ON;

		INSERT INTO [dbo].[tbl_activity_questions_answers] ([ANSWER_ID],[CATEGORY_ID],[ANSWER_TEXT])
		SELECT [ANSWER_ID],[CATEGORY_ID],[ANSWER_TEXT] FROM [tbl_product_questions_answers];
	
		SET IDENTITY_INSERT tbl_activity_questions_answers OFF;
	END

	DROP TABLE [tbl_product_questions_answers];
	IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_product_questions_answers_work')) 
	BEGIN
		DROP TABLE [tbl_product_questions_answers_work];
	END
END