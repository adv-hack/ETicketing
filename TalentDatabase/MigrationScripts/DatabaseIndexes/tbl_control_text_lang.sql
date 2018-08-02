-- Remove existing index
IF EXISTS (SELECT * FROM sys.indexes WHERE NAME ='idx_control_text_lang_1' AND OBJECT_ID = OBJECT_ID('tbl_control_text_lang'))
BEGIN
	DROP INDEX idx_control_text_lang_1 on tbl_control_text_lang
END
GO

-- Add the new index if it doesn't exist already
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE NAME='idx_Lang_BU_Partner_Page_Control_Text' AND OBJECT_ID = OBJECT_ID('tbl_control_text_lang'))
BEGIN
	-- Remove existing primary key constraint
	IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME ='PK_tbl_control_text_lang'AND TABLE_NAME = 'tbl_control_text_lang')
	BEGIN
		ALTER TABLE tbl_control_text_lang DROP CONSTRAINT PK_tbl_control_text_lang
	END
	
	-- Change the data types on the columns to help with performance and indexing 
	ALTER TABLE tbl_control_text_lang ALTER COLUMN LANGUAGE_CODE varchar(20)
	ALTER TABLE tbl_control_text_lang ALTER COLUMN BUSINESS_UNIT varchar(30)
	ALTER TABLE tbl_control_text_lang ALTER COLUMN PARTNER_CODE varchar(30)
	ALTER TABLE tbl_control_text_lang ALTER COLUMN PAGE_CODE varchar(50)
	ALTER TABLE tbl_control_text_lang ALTER COLUMN CONTROL_CODE varchar(100)
	ALTER TABLE tbl_control_text_lang ALTER COLUMN TEXT_CODE varchar(50)
 
	-- Add the primary key constraint
	ALTER TABLE [dbo].[tbl_control_text_lang] ADD CONSTRAINT [PK_tbl_control_text_lang] PRIMARY KEY CLUSTERED 
	([ID] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

	-- Create the new index
	CREATE NONCLUSTERED INDEX [idx_Lang_BU_Partner_Page_Control_Text] ON [dbo].[tbl_control_text_lang]
	(
		   [LANGUAGE_CODE] ASC,
		   [BUSINESS_UNIT] ASC,
		   [PARTNER_CODE] ASC,
		   [PAGE_CODE] ASC,
		   [CONTROL_CODE] ASC,
		   [TEXT_CODE] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO