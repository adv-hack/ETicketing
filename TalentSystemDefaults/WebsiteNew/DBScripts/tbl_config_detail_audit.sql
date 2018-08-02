
	if OBJECT_ID('tbl_config_detail_audit') is not null 
		drop table dbo.tbl_config_detail_audit
	go 
	create table dbo.tbl_config_detail_audit (
		id int identity(1,1) not null,
		group_id int null,
		user_name varchar(50) null,
		module_name varchar(100) null,
		command varbinary(max) null,
		timestamp datetime null,
		table_name varchar(100) null,
		action varchar(20) null,
		data_source varchar(30), 
		catalog varchar(30), 
		business_unit varchar(30),
		constraint pk_tbl_config_detail_audit primary key(id) 
	)
	go 
