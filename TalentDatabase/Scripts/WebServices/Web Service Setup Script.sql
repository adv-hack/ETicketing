-- Complete the details for the new user and the modules for SSO
-- Find and replace the customer database and SQL server IP/Name for the database you wish to update

declare @supplynetbu nvarchar(50)
set @supplynetbu = 'TRADING_PORTAL'
declare @supplynetpartner nvarchar(50)
set @supplynetpartner = 'AUGUST'
declare @supplynetloginid nvarchar(50)
set @supplynetloginid = 'AUGUST'
declare @supplynetpw nvarchar(50)
set @supplynetpw = '4UGU57'
declare @storedprocgroup nvarchar(50)
set @storedprocgroup = 'GDWWSV3'



----------------------------------------------------------
-- ProductListRequest10
----------------------------------------------------------
declare @module1 as nvarchar(max)
set @module1 = 'ProductListRequest'
declare @module1currentversion as nvarchar(max)
set @module1currentversion = '1'
declare @module1type as nvarchar(max)
set @module1type = 'UPDATE'
declare @module1businessunit as nvarchar(max)
set @module1businessunit = @supplynetbu
declare @module1partner as nvarchar(max)
set @module1partner = @supplynetpartner
declare @module1responseversion as nvarchar(max)
set @module1responseversion = '1.0'
declare @module1xmlschemadoc as nvarchar(max)
set @module1xmlschemadoc = 'ProductListRequest10.xsd'
declare @module1DB as nvarchar(max)
set @module1DB = 'TALENTTKT'

-------------------------------------------------------
-- Execute SQL
-------------------------------------------------------



print('----------------------------')
print('Creating supplynet user and partner')
print('----------------------------')
if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_authorized_users 
			where business_unit = @supplynetbu 
			and partner = @supplynetpartner 
			and loginid = @supplynetloginid)
begin
INSERT INTO [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_authorized_users
           ([BUSINESS_UNIT],[PARTNER],[LOGINID],[PASSWORD],[AUTO_PROCESS_DEFAULT_USER],
		   [IS_APPROVED],[IS_LOCKED_OUT],[CREATED_DATE],[LAST_LOGIN_DATE],
		   [LAST_PASSWORD_CHANGED_DATE],[LAST_LOCKED_OUT_DATE])
     VALUES
           (@supplynetbu,@supplynetpartner,@supplynetloginid,@supplynetpw,
		   'False','True','False','01/01/1900','01/01/1900','01/01/1900','01/01/1900')
end

if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_authorized_partners 
				where business_unit = @supplynetbu and partner = @supplynetpartner)
begin
INSERT INTO [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_authorized_partners
           ([BUSINESS_UNIT],[PARTNER],[DEFAULT_PARTNER])
     VALUES
           (@supplynetbu,@supplynetpartner,'False')
end


if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_partner_user 
				where partner = @supplynetpartner 
				and loginid = @supplynetloginid)
begin
INSERT INTO [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_partner_user
           ([PARTNER],[LOGINID],[EMAIL],[TITLE],[INITIALS],[FORENAME],[SURNAME],
		   [FULL_NAME],[SALUTATION],[POSITION],[DOB])
     VALUES
           (@supplynetpartner,@supplynetloginid,@supplynetpartner,'Mr',''
           ,@supplynetpartner,@supplynetpartner,@supplynetpartner,'','','01/01/1900')
end


if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_partner 
				where partner = @supplynetpartner)
begin
INSERT INTO [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_partner
           ([PARTNER],[PARTNER_DESC],[DESTINATION_DATABASE],[CACHEING_ENABLED],
		   [CACHE_TIME_MINUTES],[LOGGING_ENABLED],[STORE_XML])
     VALUES
           (@supplynetpartner,@supplynetpartner,'SQL2005','True', '1','False','False')
end


print('-------------------------------------')
print('Adding Supplynet Module settings')
print('-------------------------------------')
print('')
print('----------------------------')
print(@module1)
print('----------------------------')
if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults 
		where module = @module1
		and business_unit = @module1businessunit)
	insert into [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults
		(business_unit, module, request_current_version, xml_schema_document, module_type, 
		logging_enabled, store_xml, outgoing_xml_response, auto_process, 
		auto_process_wait_period_minutes, retry_failures, retry_attempts, retry_wait_time, 
		retry_error_numbers)
	values (@module1businessunit, @module1, @module1currentversion,'',@module1type, 'true',
			'true','','false',0,'false',0,0,0)
	else
		update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults
		set request_current_version = @module1currentversion,
			module_type = @module1type,
			logging_enabled = 'true',
			store_xml = 'true',
			outgoing_xml_response = '', 
			auto_process = 'false', 
			auto_process_wait_period_minutes = 0, 
			retry_failures = 'false', 
			retry_attempts = 0, 
			retry_wait_time = 0, 
			retry_error_numbers = 0
		where module = @module1
		and business_unit = @module1businessunit


------------------------------------------------
-- Supplynet Module Partner Defaults
------------------------------------------------

print('----------------------------')
print('-- Supplynet Module Partner Defaults')
print('----------------------------')

print('----------------------------')
print(@module1)
print('----------------------------')

if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults 
		where module = @module1
		and partner = @module1partner)
	insert into [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults
		(partner, module, incoming_style_sheet,outgoing_style_sheet,response_version,destination_database,xml_schema_document,cache_time_minutes,
		logging_enabled,store_xml,outgoing_xml_response,auto_process,email,email_xml_response,auto_process_split,stored_procedure_group,log_requests,response_directory)
	values (@module1partner, @module1,'','',@module1responseversion,@module1DB,@module1xmlschemadoc,0,'true','true','','false','','false','false', @storedprocgroup,'false','')
	else
		update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults
		set incoming_style_sheet = '',
			outgoing_style_sheet = '',
			response_version = @module1responseversion,
			destination_database = @module1DB,
			xml_schema_document = @module1xmlschemadoc,
			cache_time_minutes = 0,
			logging_enabled = 'true',
			store_xml = 'true',
			outgoing_xml_response = '',
			auto_process = 'false',
			email = '',
			email_xml_response = 'false',
			auto_process_split = 'false',
			stored_procedure_group = @storedprocgroup,
			log_requests = 'false',
			response_directory = ''
		where module = @module1
		and partner = @module1partner


