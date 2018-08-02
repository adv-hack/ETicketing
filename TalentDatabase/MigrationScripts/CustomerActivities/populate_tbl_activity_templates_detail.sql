-- ==========================================================================================
-- Migrate old product questions records to activity questions records
-- ==========================================================================================
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_activity_templates_detail')) 
AND (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_product_questions_templates_detail'))
BEGIN
	IF ((SELECT COUNT(*) FROM [tbl_product_questions_templates_detail]) > 0)
	BEGIN
		SET IDENTITY_INSERT [tbl_activity_templates_detail] ON;

		INSERT INTO [tbl_activity_templates_detail] ([ID],[TEMPLATE_ID],[QUESTION_ID],[SEQUENCE])
		SELECT [ID],[TEMPLATE_ID],[QUESTION_ID],[SEQUENCE] FROM [tbl_product_questions_templates_detail]
	
		SET IDENTITY_INSERT [tbl_activity_templates_detail] OFF;
	END

	DROP TABLE [tbl_product_questions_templates_detail];
	IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_product_questions_templates_detail_work')) 
	BEGIN
		DROP TABLE [tbl_product_questions_templates_detail_work];
	END
END