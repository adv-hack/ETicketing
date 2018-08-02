-- ==========================================================================================
-- Migrate old product questions records to activity questions records
-- ==========================================================================================
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_activity_questions_answer_categories')) 
AND (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_product_questions_answer_categories'))
BEGIN
	IF ((SELECT COUNT(*) FROM [tbl_product_questions_answer_categories]) > 0)
	BEGIN
		SET IDENTITY_INSERT [tbl_activity_questions_answer_categories] ON;

		INSERT INTO [tbl_activity_questions_answer_categories] ([CATEGORY_ID],[CATEGORY_NAME])
		SELECT [CATEGORY_ID],[CATEGORY_NAME] FROM [tbl_product_questions_answer_categories];

		SET IDENTITY_INSERT [tbl_activity_questions_answer_categories] OFF;
	END

	DROP TABLE [tbl_product_questions_answer_categories];
	IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_product_questions_answer_categories_work')) 
	BEGIN
		DROP TABLE [tbl_product_questions_answer_categories_work];
	END
END