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
-- CustomerAddRequest11
----------------------------------------------------------
declare @module1 as nvarchar(max)
set @module1 = 'CustomerAddRequest'
declare @module1currentversion as nvarchar(max)
set @module1currentversion = '1'
declare @module1type as nvarchar(max)
set @module1type = 'UPDATE'
declare @module1businessunit as nvarchar(max)
set @module1businessunit = @supplynetbu
declare @module1partner as nvarchar(max)
set @module1partner = @supplynetpartner
declare @module1responseversion as nvarchar(max)
set @module1responseversion = '1.1'
declare @module1xmlschemadoc as nvarchar(max)
set @module1xmlschemadoc = 'CustomerAddRequest11.xsd'
declare @module1DB as nvarchar(max)
set @module1DB = 'TALENTTKT'
----------------------------------------------------------
-- CustomerUpdateRequest
----------------------------------------------------------
declare @module2 as nvarchar(max)
set @module2 = 'CustomerUpdateRequest'
declare @module2currentversion as nvarchar(max)
set @module2currentversion = '1'
declare @module2type as nvarchar(max)
set @module2type = 'UPDATE'
declare @module2businessunit as nvarchar(max)
set @module2businessunit = @supplynetbu
declare @module2partner as nvarchar(max)
set @module2partner = @supplynetpartner
declare @module2responseversion as nvarchar(max)
set @module2responseversion = '1.1'
declare @module2xmlschemadoc as nvarchar(max)
set @module2xmlschemadoc = 'CustomerUpdateRequest11.xsd'
declare @module2DB as nvarchar(max)
set @module2DB = 'TALENTTKT'
----------------------------------------------------------
-- •	CustomerRetrievalRequest
----------------------------------------------------------
declare @module3 as nvarchar(max)
set @module3 = 'CustomerRetrievalRequest'
declare @module3currentversion as nvarchar(max)
set @module3currentversion = '1'
declare @module3type as nvarchar(max)
set @module3type = 'ENQUIRY'
declare @module3businessunit as nvarchar(max)
set @module3businessunit = @supplynetbu
declare @module3partner as nvarchar(max)
set @module3partner = @supplynetpartner
declare @module3responseversion as nvarchar(max)
set @module3responseversion = '1.1'
declare @module3xmlschemadoc as nvarchar(max)
set @module3xmlschemadoc = 'CustomerRetrievalRequest11.xsd'
declare @module3DB as nvarchar(max)
set @module3DB = 'TALENTTKT'
----------------------------------------------------------
-- RetrievePasswordRequest
----------------------------------------------------------
declare @module4 as nvarchar(max)
set @module4 = 'RetrievePasswordRequest'
declare @module4currentversion as nvarchar(max)
set @module4currentversion = '1'
declare @module4type as nvarchar(max)
set @module4type = 'ENQUIRY'
declare @module4businessunit as nvarchar(max)
set @module4businessunit = @supplynetbu
declare @module4partner as nvarchar(max)
set @module4partner = @supplynetpartner
declare @module4responseversion as nvarchar(max)
set @module4responseversion = '1.0'
declare @module4xmlschemadoc as nvarchar(max)
set @module4xmlschemadoc = 'RetrievePasswordRequest10.xsd'
declare @module4DB as nvarchar(max)
set @module4DB = 'TALENTTKT'
----------------------------------------------------------
-- •	VerifyPasswordRequest
----------------------------------------------------------
declare @module5 as nvarchar(max)
set @module5 = 'VerifyPasswordRequest'
declare @module5currentversion as nvarchar(max)
set @module5currentversion = '1'
declare @module5type as nvarchar(max)
set @module5type = 'ENQUIRY'
declare @module5businessunit as nvarchar(max)
set @module5businessunit = @supplynetbu
declare @module5partner as nvarchar(max)
set @module5partner = @supplynetpartner
declare @module5responseversion as nvarchar(max)
set @module5responseversion = '1.0'
declare @module5xmlschemadoc as nvarchar(max)
set @module5xmlschemadoc = 'VerifyPasswordRequest10.xsd'
declare @module5DB as nvarchar(max)
set @module5DB = 'TALENTTKT'
----------------------------------------------------------
-- •	CustomerSearchRequest
----------------------------------------------------------
declare @module6 as nvarchar(max)
set @module6 = 'CustomerSearchRequest'
declare @module6currentversion as nvarchar(max)
set @module6currentversion = '1'
declare @module6type as nvarchar(max)
set @module6type = 'ENQUIRY'
declare @module6businessunit as nvarchar(max)
set @module6businessunit = @supplynetbu
declare @module6partner as nvarchar(max)
set @module6partner = @supplynetpartner
declare @module6responseversion as nvarchar(max)
set @module6responseversion = '1.0'
declare @module6xmlschemadoc as nvarchar(max)
set @module6xmlschemadoc = 'CustomerSearchRequest10.xsd'
declare @module6DB as nvarchar(max)
set @module6DB = 'TALENTTKT'

-------------------------------------------------------
-- Execute SQL
-------------------------------------------------------

update 
[10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].[tbl_ecommerce_module_defaults_bu] 
set value = 'True' 
where default_name like '%ALLOW_AUTO_LOGIN%' 

update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].[tbl_ecommerce_module_defaults_bu] 
set VALUE = 'TRUE' 
where DEFAULT_NAME = 'CREATE_SINGLE_SIGNON_COOKIES' 


update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].[tbl_ecommerce_module_defaults_bu] 
set VALUE = 'G00DW00D' 
where DEFAULT_NAME = 'SINGLE_SIGNON_SECRET_KEY' 


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

print('----------------------------')
print(@module2)
print('----------------------------')

if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults 
		where module = @module2
		and business_unit = @module2businessunit)
	insert into [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults
		(business_unit, module, request_current_version, xml_schema_document, module_type, 
		logging_enabled, store_xml, outgoing_xml_response, auto_process, 
		auto_process_wait_period_minutes, retry_failures, retry_attempts, retry_wait_time, 
		retry_error_numbers)
	values (@module2businessunit, @module2, @module2currentversion,'',@module2type, 'true',
			'true','','false',0,'false',0,0,0)
	else
		update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults
		set request_current_version = @module2currentversion,
			module_type = @module2type,
			logging_enabled = 'true',
			store_xml = 'true',
			outgoing_xml_response = '', 
			auto_process = 'false', 
			auto_process_wait_period_minutes = 0, 
			retry_failures = 'false', 
			retry_attempts = 0, 
			retry_wait_time = 0, 
			retry_error_numbers = 0
		where module = @module2
		and business_unit = @module2businessunit


print('----------------------------')
print(@module3)
print('----------------------------')


if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults 
		where module = @module3
		and business_unit = @module3businessunit)
	insert into [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults
		(business_unit, module, request_current_version, xml_schema_document, module_type, 
		logging_enabled, store_xml, outgoing_xml_response, auto_process, 
		auto_process_wait_period_minutes, retry_failures, retry_attempts, retry_wait_time, 
		retry_error_numbers)
	values (@module3businessunit, @module3, @module3currentversion,'',@module3type, 'true',
			'true','','false',0,'false',0,0,0)
	else
		update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults
		set request_current_version = @module3currentversion,
			module_type = @module3type,
			logging_enabled = 'true',
			store_xml = 'true',
			outgoing_xml_response = '', 
			auto_process = 'false', 
			auto_process_wait_period_minutes = 0, 
			retry_failures = 'false', 
			retry_attempts = 0, 
			retry_wait_time = 0, 
			retry_error_numbers = 0
		where module = @module3
		and business_unit = @module3businessunit



print('----------------------------')
print(@module4)
print('----------------------------')

if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults 
		where module = @module4
		and business_unit = @module4businessunit)
	insert into [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults
		(business_unit, module, request_current_version, xml_schema_document, module_type, 
		logging_enabled, store_xml, outgoing_xml_response, auto_process, 
		auto_process_wait_period_minutes, retry_failures, retry_attempts, retry_wait_time, 
		retry_error_numbers)
	values (@module4businessunit, @module4, @module4currentversion,'',@module4type, 'true',
			'true','','false',0,'false',0,0,0)
	else
		update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults
		set request_current_version = @module4currentversion,
			module_type = @module4type,
			logging_enabled = 'true',
			store_xml = 'true',
			outgoing_xml_response = '', 
			auto_process = 'false', 
			auto_process_wait_period_minutes = 0, 
			retry_failures = 'false', 
			retry_attempts = 0, 
			retry_wait_time = 0, 
			retry_error_numbers = 0
		where module = @module4
		and business_unit = @module4businessunit

		

print('----------------------------')
print(@module5)
print('----------------------------')

if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults 
		where module = @module5
		and business_unit = @module5businessunit)
	insert into [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults
		(business_unit, module, request_current_version, xml_schema_document, module_type, 
		logging_enabled, store_xml, outgoing_xml_response, auto_process, 
		auto_process_wait_period_minutes, retry_failures, retry_attempts, retry_wait_time, 
		retry_error_numbers)
	values (@module5businessunit, @module5, @module5currentversion,'',@module5type, 'true',
			'true','','false',0,'false',0,0,0)
	else
		update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults
		set request_current_version = @module5currentversion,
			module_type = @module5type,
			logging_enabled = 'true',
			store_xml = 'true',
			outgoing_xml_response = '', 
			auto_process = 'false', 
			auto_process_wait_period_minutes = 0, 
			retry_failures = 'false', 
			retry_attempts = 0, 
			retry_wait_time = 0, 
			retry_error_numbers = 0
		where module = @module5
		and business_unit = @module5businessunit





print('----------------------------')
print(@module6)
print('----------------------------')

if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults 
		where module = @module6
		and business_unit = @module6businessunit)
	insert into [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults
		(business_unit, module, request_current_version, xml_schema_document, module_type, 
		logging_enabled, store_xml, outgoing_xml_response, auto_process, 
		auto_process_wait_period_minutes, retry_failures, retry_attempts, retry_wait_time, 
		retry_error_numbers)
	values (@module6businessunit, @module6, @module6currentversion,'',@module6type, 'true',
			'true','','false',0,'false',0,0,0)
	else
		update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_defaults
		set request_current_version = @module6currentversion,
			module_type = @module6type,
			logging_enabled = 'true',
			store_xml = 'true',
			outgoing_xml_response = '', 
			auto_process = 'false', 
			auto_process_wait_period_minutes = 0, 
			retry_failures = 'false', 
			retry_attempts = 0, 
			retry_wait_time = 0, 
			retry_error_numbers = 0
		where module = @module6
		and business_unit = @module6businessunit



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



print('----------------------------')
print(@module2)
print('----------------------------')

if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults 
		where module = @module2
		and partner = @module2partner)
	insert into [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults
		(partner, module, incoming_style_sheet,outgoing_style_sheet,response_version,destination_database,xml_schema_document,cache_time_minutes,
		logging_enabled,store_xml,outgoing_xml_response,auto_process,email,email_xml_response,auto_process_split,stored_procedure_group,log_requests,response_directory)
	values (@module2partner, @module2,'','',@module2responseversion,@module2DB,@module2xmlschemadoc,0,'true','true','','false','','false','false', @storedprocgroup,'false','')
	else
		update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults
		set incoming_style_sheet = '',
			outgoing_style_sheet = '',
			response_version = @module2responseversion,
			destination_database = @module2DB,
			xml_schema_document = @module2xmlschemadoc,
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
		where module = @module2
		and partner = @module2partner



print('----------------------------')
print(@module3)
print('----------------------------')

if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults 
		where module = @module3
		and partner = @module3partner)
	insert into [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults
		(partner, module, incoming_style_sheet,outgoing_style_sheet,response_version,destination_database,xml_schema_document,cache_time_minutes,
		logging_enabled,store_xml,outgoing_xml_response,auto_process,email,email_xml_response,auto_process_split,stored_procedure_group,log_requests,response_directory)
	values (@module3partner, @module3,'','',@module3responseversion,@module3DB,@module3xmlschemadoc,0,'true','true','','false','','false','false',@storedprocgroup,'false','')
	else
		update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults
		set incoming_style_sheet = '',
			outgoing_style_sheet = '',
			response_version = @module3responseversion,
			destination_database = @module3DB,
			xml_schema_document = @module3xmlschemadoc,
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
		where module = @module3
		and partner = @module3partner



print('----------------------------')
print(@module4)
print('----------------------------')

if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults 
		where module = @module4
		and partner = @module4partner)
	insert into [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults
		(partner, module, incoming_style_sheet,outgoing_style_sheet,response_version,destination_database,xml_schema_document,cache_time_minutes,
		logging_enabled,store_xml,outgoing_xml_response,auto_process,email,email_xml_response,auto_process_split,stored_procedure_group,log_requests,response_directory)
	values (@module4partner, @module4,'','',@module4responseversion,@module4DB,@module4xmlschemadoc,0,'true','true','','false','','false','false',@storedprocgroup,'false','')
	else
		update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults
		set incoming_style_sheet = '',
			outgoing_style_sheet = '',
			response_version = @module4responseversion,
			destination_database = @module4DB,
			xml_schema_document = @module4xmlschemadoc,
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
		where module = @module4
		and partner = @module4partner



print('----------------------------')
print(@module5)
print('----------------------------')

if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults 
		where module = @module5
		and partner = @module5partner)
	insert into [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults
		(partner, module, incoming_style_sheet,outgoing_style_sheet,response_version,destination_database,xml_schema_document,cache_time_minutes,
		logging_enabled,store_xml,outgoing_xml_response,auto_process,email,email_xml_response,auto_process_split,stored_procedure_group,log_requests,response_directory)
	values (@module5partner, @module5,'','',@module5responseversion,@module5DB,@module5xmlschemadoc,0,'true','true','','false','','false','false',@storedprocgroup,'false','')
	else
		update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults
		set incoming_style_sheet = '',
			outgoing_style_sheet = '',
			response_version = @module5responseversion,
			destination_database = @module5DB,
			xml_schema_document = @module5xmlschemadoc,
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
		where module = @module5
		and partner = @module5partner



print('----------------------------')
print(@module6)
print('----------------------------')

if not exists (select * from [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults 
		where module = @module6
		and partner = @module6partner)
	insert into [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults
		(partner, module, incoming_style_sheet,outgoing_style_sheet,response_version,destination_database,xml_schema_document,cache_time_minutes,
		logging_enabled,store_xml,outgoing_xml_response,auto_process,email,email_xml_response,auto_process_split,stored_procedure_group,log_requests,response_directory)
	values (@module6partner, @module6,'','',@module6responseversion,@module6DB,@module6xmlschemadoc,0,'true','true','','false','','false','false',@storedprocgroup,'false','')
	else
		update [10.20.126.177].[STAGE_TalentEBusinessDBGoodwood].[dbo].tbl_supplynet_module_partner_defaults
		set incoming_style_sheet = '',
			outgoing_style_sheet = '',
			response_version = @module6responseversion,
			destination_database = @module6DB,
			xml_schema_document = @module6xmlschemadoc,
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
		where module = @module6
		and partner = @module6partner

