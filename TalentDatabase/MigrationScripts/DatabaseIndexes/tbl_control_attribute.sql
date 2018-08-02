-- Remove existing index if it exists
IF EXISTS (SELECT * FROM sys.indexes WHERE NAME ='idx_control_attribute_1' AND OBJECT_ID = OBJECT_ID('tbl_control_attribute'))
BEGIN
	DROP INDEX idx_control_attribute_1 on tbl_control_attribute
END
GO

-- Add the new index if it doesn't exist already
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='idx_BU_Partner_Page_Control_Attr' AND OBJECT_ID = OBJECT_ID('tbl_control_attribute'))
BEGIN
	-- Remove existing primary key constraint
	IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME ='PK_tbl_control_attribute'AND TABLE_NAME = 'tbl_control_attribute')
	BEGIN
		ALTER TABLE tbl_control_attribute DROP CONSTRAINT PK_tbl_control_attribute
	END

	-- Change the data types on the columns to help with performance and indexing
	ALTER TABLE tbl_control_attribute ALTER COLUMN BUSINESS_UNIT varchar(30)
	ALTER TABLE tbl_control_attribute ALTER COLUMN PARTNER_CODE varchar(30)
	ALTER TABLE tbl_control_attribute ALTER COLUMN PAGE_CODE varchar(50)
	ALTER TABLE tbl_control_attribute ALTER COLUMN CONTROL_CODE varchar(100)
	ALTER TABLE tbl_control_attribute ALTER COLUMN ATTR_NAME varchar(50)
 
	-- Add the primary key constraint
	ALTER TABLE [dbo].[tbl_control_attribute] ADD  CONSTRAINT [PK_tbl_control_attribute] PRIMARY KEY CLUSTERED 
	([ID] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

	-- Create the new index
	CREATE NONCLUSTERED INDEX [idx_BU_Partner_Page_Control_Attr] ON [dbo].[tbl_control_attribute]
	(
		   [BUSINESS_UNIT] ASC,
		   [PARTNER_CODE] ASC,
		   [PAGE_CODE] ASC,
		   [CONTROL_CODE] ASC,
		   [ATTR_NAME] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	END
GO