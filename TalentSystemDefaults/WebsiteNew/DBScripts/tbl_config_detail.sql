/**************************************************
	TALENT CONFIGURATION
**************************************************/

--------------------------------------------------
-- tbl_config_detail
--------------------------------------------------
CREATE TABLE dbo.tbl_config_detail (
	ID int IDENTITY(1,1) NOT NULL,
	CONFIG_ID varchar(32) NULL,
	TABLE_NAME nvarchar(50) NOT NULL,
	MASTER_CONFIG nvarchar(50),
	DEFAULT_KEY1 nvarchar(250),
	DEFAULT_KEY2 nvarchar(250),
	DEFAULT_KEY3 nvarchar(250),
	DEFAULT_KEY4 nvarchar(250),
	VARIABLE_KEY1 nvarchar(250),
	VARIABLE_KEY2 nvarchar(250),
	VARIABLE_KEY3 nvarchar(250),
	VARIABLE_KEY4 nvarchar(250),
	DISPLAY_NAME nvarchar(250) ,
	DEFAULT_NAME nvarchar(250) NOT NULL,
	DEFAULT_VALUE nvarchar(max) NOT NULL,
	ALLOWED_VALUE nvarchar(max) ,
	ALLOWED_PLACE_HOLDER nvarchar(max) ,
	DESCRIPTION nvarchar(max),
	MODULE_NAME nvarchar(100),
	CREATED_TIMESTAMP datetime,
	LASTMODIFIED_TIMESTAMP datetime,
	QA_ACCEPTED bit,
	IN_USE bit DEFAULT 1
) 

--------------------------------------------------
-- Data Changes (temporary-for our purpose)
--------------------------------------------------

alter table tbl_config_detail
add MODULE_NAME nvarchar(100)
go

alter table tbl_config_detail
add CREATED_TIMESTAMP datetime
go

alter table tbl_config_detail
add LASTMODIFIED_TIMESTAMP datetime
go

alter table tbl_config_detail
add QA_ACCEPTED bit
go

alter table tbl_config_detail
add IN_USE bit DEFAULT 1
go

update	dbo.tbl_config_detail
set		IN_USE = 1 




