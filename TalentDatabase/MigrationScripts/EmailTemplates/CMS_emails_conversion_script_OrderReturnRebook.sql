

declare @businessunit as nvarchar(50) = 'UNITEDKINGDOM'
declare @partner as nvarchar(50) = 'PUBLIC'

declare @lang as nvarchar(50) = 'ENG'
declare @businessunitALL as nvarchar(50) = '*ALL'
declare @partnerALL as nvarchar(50) = '*ALL'
declare @pagecodeALL as nvarchar(50) = '*ALL'


declare @type as nvarchar(100)
declare @type2 as nvarchar(100)
declare @subject as nvarchar(200) 
declare @from_address as nvarchar(200) 
declare @body as nvarchar(max) 
declare @html as bit 
declare @html_string as nvarchar(10)


-------------------------
-- OrderReturnConfirmation
-------------------------
set @type = 'OrderReturnConfirmRebook'
declare @count as int
set @count = (select count(*) from tbl_email_templates where TEMPLATE_TYPE = @type)
if @count > 0 
begin
print 'Conversion already run!'
end
else
begin
set @from_address = (select top 1 VALUE from tbl_ecommerce_module_defaults_bu where DEFAULT_NAME = 'ORDERS_FROM_EMAIL')
set @subject = isnull((select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'OrderReturnConfirmEmail.vb' and TEXT_CODE = 'EmailSubject'),'')
set @body =     ((convert(nvarchar(max),isnull((select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'OrderReturnConfirmEmailRebook.vb' and TEXT_CODE = 'EmailMergeCode01'),''))) + ' ' +
				(convert(nvarchar(max),isnull((select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'OrderConfirmEmail.vb' and TEXT_CODE = 'EmailMergeCode02'),''))) + ' ' +
				(convert(nvarchar(max),isnull((select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'OrderConfirmEmail.vb' and TEXT_CODE = 'EmailMergeCode03'),''))) + ' ' +
				(convert(nvarchar(max),isnull((select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'OrderConfirmEmail.vb' and TEXT_CODE = 'EmailMergeCode04'),''))) + ' ' +
				(convert(nvarchar(max),isnull((select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'OrderConfirmEmail.vb' and TEXT_CODE = 'EmailMergeCode05'),''))) + ' ' +
				(convert(nvarchar(max),isnull((select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'OrderConfirmEmail.vb' and TEXT_CODE = 'EmailMergeCode06'),''))) + ' ' +
				(convert(nvarchar(max),isnull((select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'OrderConfirmEmail.vb' and TEXT_CODE = 'EmailMergeCode07'),''))) + ' ' +
				(convert(nvarchar(max),isnull((select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'OrderConfirmEmail.vb' and TEXT_CODE = 'EmailMergeCode08'),''))) + ' ' +
				(convert(nvarchar(max),isnull((select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'OrderConfirmEmail.vb' and TEXT_CODE = 'EmailMergeCode09'),''))))

set @html = (select top 1 value from tbl_ecommerce_module_defaults_bu where DEFAULT_NAME = 'SEND_ORDER_CONFIRMATION_EMAIL_AS_HTML')

if (isnull(@body,'') = '')
begin
print '@body is null'
end
else
begin
INSERT INTO [tbl_email_templates] ([BUSINESS_UNIT],[PARTNER],[ACTIVE],[NAME],[DESCRIPTION],[TEMPLATE_TYPE],[EMAIL_HTML],[EMAIL_FROM_ADDRESS],[EMAIL_SUBJECT],[EMAIL_BODY],[ADDED_DATETIME],[UPDATED_DATETIME])
VALUES (@businessunit,@partner,1,@type,@type,@type,@html,@from_address,@subject,@body,GetDate(),GetDate())

print '@type=' + convert(nvarchar,@type)
print '@subject=' + convert(nvarchar,@subject)
print '@body=' + convert(nvarchar,@body)
print '@html=' + convert(nvarchar,@html)





declare @generic_control_code as nvarchar(max)
declare @control_code as nvarchar(max)
declare @control_code_new as nvarchar(max)
declare @text_code as nvarchar(50)
declare @control_content as nvarchar(max)
declare @attr_name as nvarchar(50)
declare @attr_value as nvarchar(max)


-- Get the ID of the new template - this is used to suffix the control_code for all teh content records
declare @templateid as int
set @templateid = (select top 1 emailtemplate_id from [tbl_email_templates] where [name]=@type and [description]=@type and template_type=@type  and business_unit = @businessunit)

print '@templateid=' + convert(nvarchar,@templateid)


set @generic_control_code = 'TalentEmail.vb'
set @control_code = 'OrderReturnConfirmEmailRebook.vb'


set @text_code = 'OrderReturnTableHeader'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'OrderReturnTableRow'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'OrderReturnSeatSeparator'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'OrderReturnText'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'OrderReturnRefText'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'OrderRebookText'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'OrderRebookRefText'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'TicketingSeperator1'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'TicketingSeperator2'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'TicketingSeperator3'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @attr_name = 'TicketingTableStyle'
set @text_code = 'TicketingTableStyle'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @attr_value = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_attribute] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[ATTR_NAME],[ATTR_VALUE],[DESCRIPTION]) VALUES(@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@attr_name,@attr_value,'')




set @text_code = 'NewLine'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'ProcessedOrderId'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'Loginid'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'UserNumber'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'Status'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'ContactName'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'AddressLine1'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'AddressLine2'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'AddressLine3'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'AddressLine4'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'AddressLine5'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'Postcode'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'Country'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'ContactPhone'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'ContactEmail'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'PromotionDescription'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'SpecialInstructions1'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'SpecialInstructions2'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'TrackingNo'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'Currency'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'PaymentType'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'BackOfficeOrderId'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'BackOfficeStatus'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'BackOfficeReference'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'Language'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'Warehouse'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'TotalOrderItemsValueGross'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'TotalOrderItemsValueNet'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'TotalOrderItemsTax'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'TotalDeliveryGross'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'TotalDeliveryTax'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'TotalDeliveryNet'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'PromotionValue'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'TotalOrderValue'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'TotalAmountCharged'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'TotalValueIncPromo'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'CustomerTitle'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'CustomerForename'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'CustomerSurname'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'CreatedDate'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'DeliveryDate'
set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

end
end

