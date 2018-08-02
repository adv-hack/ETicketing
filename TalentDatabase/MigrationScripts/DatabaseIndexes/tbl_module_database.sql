-- Add the new index if it doesn't exist already
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='idx_BU_Partner_Appl_Module' AND OBJECT_ID = OBJECT_ID('tbl_bu_module_database'))
BEGIN
	-- Remove existing primary key constraint
	IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME ='PK_tbl_bu_module_database'AND TABLE_NAME = 'tbl_bu_module_database')
	BEGIN
		ALTER TABLE tbl_bu_module_database DROP CONSTRAINT PK_tbl_bu_module_database
	END

	-- Change the data types on the columns to help with performance and indexing
	ALTER TABLE tbl_bu_module_database ALTER COLUMN [BUSINESS_UNIT] varchar(50)
	ALTER TABLE tbl_bu_module_database ALTER COLUMN [PARTNER] varchar(50)
	ALTER TABLE tbl_bu_module_database ALTER COLUMN [APPLICATION] varchar(50)
	ALTER TABLE tbl_bu_module_database ALTER COLUMN [MODULE] varchar(50)
	ALTER TABLE tbl_bu_module_database ALTER COLUMN [DESTINATION_DATABASE] varchar(50)
	ALTER TABLE tbl_bu_module_database ALTER COLUMN [SECTION] varchar(50)
	ALTER TABLE tbl_bu_module_database ALTER COLUMN [SUB_SECTION] varchar(50)
 
	-- Add the primary key constraint
	ALTER TABLE [dbo].[tbl_bu_module_database] ADD  CONSTRAINT [PK_tbl_bu_module_database] PRIMARY KEY CLUSTERED 
	([ID] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

	-- Create the new index
	CREATE NONCLUSTERED INDEX [idx_BU_Partner_Appl_Module] ON [dbo].[tbl_bu_module_database]
	(
		   [BUSINESS_UNIT] ASC,
		   [PARTNER] ASC,
		   [APPLICATION] ASC,
		   [MODULE] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	END
GO