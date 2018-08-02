

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





------------------------------
-- RegistrationConfirmation
------------------------------
set @type = 'RegistrationConfirmation'
set @type2 = 'CustomerRegistration'
declare @count as int
set @count = (select count(*) from tbl_email_templates where TEMPLATE_TYPE = @type2)
if @count > 0 
begin
print 'Conversion already run!'
end
else
begin

set @subject = Isnull((select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'RegistrationForm.ascx' and TEXT_CODE = 'ConfirmationEmailSubject'),'')
set @from_address = (select top 1 VALUE from tbl_ecommerce_module_defaults_bu where DEFAULT_NAME = 'REGISTRATION_CONFIRMATION_FROM_EMAIL')

set @html_string = (select top 1 ATTR_VALUE from tbl_control_attribute where CONTROL_CODE='RegistrationForm.ascx' and ATTR_NAME = 'emailFormat')
if UPPER(@html_string) = 'HTML'
begin
set @html = 1
end
else
begin
set @html = 0
end

if @html = 1
begin
set @body =  (convert(nvarchar(max),(select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'RegistrationForm.ascx' and TEXT_CODE = 'ConfirmationEmailBodyHTML')))
end
else
begin
set @body =  (convert(nvarchar(max),(select top 1 CONTROL_CONTENT from tbl_control_text_lang where CONTROL_CODE = 'RegistrationForm.ascx' and TEXT_CODE = 'ConfirmationEmailBody')))
end

if (isnull(@body,'') = '')
begin
print '@body is null'
end
else
begin
INSERT INTO [tbl_email_templates] ([BUSINESS_UNIT],[PARTNER],[ACTIVE],[NAME],[DESCRIPTION],[TEMPLATE_TYPE],[EMAIL_HTML],[EMAIL_FROM_ADDRESS],[EMAIL_SUBJECT],[EMAIL_BODY],[ADDED_DATETIME],[UPDATED_DATETIME])
VALUES (@businessunit,@partner,1,@type2,@type2,@type2,@html,@from_address,@subject,@body,GetDate(),GetDate())

print '@type=' + convert(nvarchar,@type)
print '@subject=' + convert(nvarchar,@subject)
print '@body=' + convert(nvarchar,@body)
print '@html=' + convert(nvarchar,@html)

end
end




