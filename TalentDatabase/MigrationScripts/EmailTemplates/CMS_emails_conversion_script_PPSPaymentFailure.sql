

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
-- PPSPaymentFailure
-------------------------
set @type = 'PPSPaymentFailure'
declare @count as int
set @count = (select count(*) from tbl_email_templates where TEMPLATE_TYPE = @type)
if @count > 0 
begin
print 'Conversion already run!'
end
else
begin

set @from_address = (select top 1 VALUE from tbl_ecommerce_module_defaults_bu where DEFAULT_NAME = 'ORDERS_FROM_EMAIL')
set @subject = Isnull((select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'TalentEmail.vb' and TEXT_CODE = 'AmendPPSEmailSubject'),'')
set @html = (select top 1 value from tbl_ecommerce_module_defaults_bu where DEFAULT_NAME = 'SEND_EMAIL_AS_HTML')

if @html = 1
begin
set @body = (convert(nvarchar(max),(select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'TalentEmail.vb' and TEXT_CODE = 'PPSPaymentFailureEmailBodyHTML')))
end
else
begin
set @body = (convert(nvarchar(max),(select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'TalentEmail.vb' and TEXT_CODE = 'PPSPaymentFailureEmailBody')))
end

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





declare @control_code_new as nvarchar(max)
declare @control_code as nvarchar(max)
declare @text_code as nvarchar(50)
declare @control_content as nvarchar(max)

-- Get the ID of the new template - this is used to suffix the control_code for all teh content records
declare @templateid as int
set @templateid = (select top 1 emailtemplate_id from [tbl_email_templates] where [name]=@type and [description]=@type and template_type=@type and business_unit = @businessunit)

print '@templateid=' + convert(nvarchar,@templateid)


set @control_code = 'TalentEmail.vb'

set @text_code = 'StandSeperator1'
set @control_code_new = ltrim(rtrim(@control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)


set @text_code = 'StandSeperator1'
set @control_code_new = ltrim(rtrim(@control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'AreaSeperator1'
set @control_code_new = ltrim(rtrim(@control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'RowSeperator1'
set @control_code_new = ltrim(rtrim(@control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'SeatSeperator1'
set @control_code_new = ltrim(rtrim(@control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'PPSProductHeader'
set @control_code_new = ltrim(rtrim(@control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'PPSProductRow'
set @control_code_new = ltrim(rtrim(@control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'PPSEnrolmentHeader'
set @control_code_new = ltrim(rtrim(@control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'PPSEnrolmentRow'
set @control_code_new = ltrim(rtrim(@control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'EnrolledText'
set @control_code_new = ltrim(rtrim(@control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'NotEnrolledText'
set @control_code_new = ltrim(rtrim(@control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'PPSEnrolmentFooter'
set @control_code_new = ltrim(rtrim(@control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

set @text_code = 'PPSProductFooter'
set @control_code_new = ltrim(rtrim(@control_code)) + '.' + convert(nvarchar,@templateid)
set @control_content = COALESCE((select top 1 control_content from [tbl_control_text_lang] where control_code=@control_code and text_code=@text_code),'')
INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content)

end
end