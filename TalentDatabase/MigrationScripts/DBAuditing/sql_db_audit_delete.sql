/******************************************************************
					DROP PROCEDURES 
 ******************************************************************/
if exists(select 1 from sys.objects a where a.name = 'spCMS_getMenuItemByLanguage')
begin 
	drop procedure dbo.spCMS_getMenuItemByLanguage
end
go 


if exists(select 1 from sys.objects a where a.name = 'spCMS_getMenuItemByCulture')
begin 
	drop procedure dbo.spCMS_getMenuItemByCulture
end
go 

/******************************************************************
					DROP REDUNDANT TABLES 
 ******************************************************************/
 
 /* Please take a note:
	Before running following script, make sure there is no dependency(foreign-key constraints) for following tables in database. 
 */

if exists(select 1 from sys.tables a where a.name = 'tbl_application')
begin 
	drop table dbo.tbl_application
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_application_module')
begin 
	drop table dbo.tbl_application_module
end
go 
 
if exists(select 1 from sys.tables a where a.name = 'tbl_authorized_users_qanda')
begin 
	drop table dbo.tbl_authorized_users_qanda
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_bu_work')
begin 
	drop table dbo.tbl_bu_work
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_contact_method')
begin 
	drop table dbo.tbl_contact_method
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_descriptions_header')
begin 
	drop table dbo.tbl_descriptions_header
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_dotnet_culture')
begin 
	drop table dbo.tbl_dotnet_culture
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_ecommerce_defaults')
begin 
	drop table dbo.tbl_ecommerce_defaults
end
go
 
if exists(select 1 from sys.tables a where a.name = 'tbl_load_test_audit_detail')
begin 
	drop table dbo.tbl_load_test_audit_detail
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_load_test_detail')
begin 
	drop table dbo.tbl_load_test_detail
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_load_test_header')
begin 
	drop table dbo.tbl_load_test_header
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_load_test_log')
begin 
	drop table dbo.tbl_load_test_log
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_module')
begin 
	drop table dbo.tbl_module
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_navigation_menu')
begin 
	drop table dbo.tbl_navigation_menu
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_navigation_menu_item')
begin 
	drop table dbo.tbl_navigation_menu_item
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_page_myaccount_navx')
begin 
	drop table dbo.tbl_page_myaccount_navx
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_product_bu')
begin 
	drop table dbo.tbl_product_bu
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_product_BKUP')
begin 
	drop table dbo.tbl_product_BKUP
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_product_bu_lang')
begin 
	drop table dbo.tbl_product_bu_lang
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_product_from_backup')
begin 
	drop table dbo.tbl_product_from_backup
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_product_relations_text')
begin 
	drop table dbo.tbl_product_relations_text
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_product_relations_attributes')
begin 
	drop table dbo.tbl_product_relations_attributes
end
go 

--if exists(select 1 from sys.tables a where a.name = 'tbl_qualifier')
--begin 
--	drop table dbo.tbl_qualifier
--end
--go 

if exists(select 1 from sys.tables a where a.name = 'tbl_menu_item_lang')
begin 
	drop table dbo.tbl_menu_item_lang
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_menu_item')
begin 
	drop table dbo.tbl_menu_item
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_menu')
begin 
	drop table dbo.tbl_menu
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_navigation_site')
begin 
	drop table dbo.tbl_navigation_site
end
go 


if exists(select 1 from sys.tables a where a.name = 'tbl_ticketing_stadium')
begin 
	drop table dbo.tbl_ticketing_stadium
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_control_text')
begin 
	drop table dbo.tbl_control_text
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_page_text')
begin 
	drop table dbo.tbl_page_text
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_ecommerce_module_defaults_bu2')
begin 
	drop table dbo.tbl_ecommerce_module_defaults_bu2
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_group_level_lang')
begin 
	drop table dbo.tbl_group_level_lang
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_login_lookup')
begin 
	drop table dbo.tbl_login_lookup
end
go 

if exists(select 1 from sys.tables a where a.name = 'tbl_ecommerce_module_defaults')
begin 
	drop table dbo.tbl_ecommerce_module_defaults
end
go 

