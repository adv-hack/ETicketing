-- ==========================================================================================
-- Migrate old product questions records to activity questions records
-- ==========================================================================================
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_activity_templates')) 
AND (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_product_questions_templates'))
BEGIN
	IF ((SELECT COUNT(*) FROM [tbl_product_questions_templates]) > 0)
	BEGIN
		SET IDENTITY_INSERT [tbl_activity_templates] ON;

		INSERT INTO [tbl_activity_templates]
           ([TEMPLATE_ID]
		   ,[BUSINESS_UNIT]
           ,[NAME]
           ,[TEMPLATE_TYPE])
		SELECT [TEMPLATE_ID]
		   ,[BUSINESS_UNIT]
		   ,[NAME]
		   ,1
			FROM [tbl_product_questions_templates]
	
		SET IDENTITY_INSERT [tbl_activity_templates] OFF;
	END

	DROP TABLE [tbl_product_questions_templates];
	IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_product_questions_templates_work')) 
	BEGIN
		DROP TABLE [tbl_product_questions_templates_work];
	END
END