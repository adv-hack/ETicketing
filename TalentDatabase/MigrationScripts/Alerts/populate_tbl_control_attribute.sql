
declare @BUSINESS_UNIT as nvarchar(30) = '*ALL'
declare @PARTNER as nvarchar(30)  = '*ALL'
declare @PAGE_CODE as nvarchar(50) = '*ALL'
declare @CONTROL_CODE as nvarchar(max)
declare @ATTR_NAME as nvarchar(50)
declare @ATTR_VALUE as nvarchar(max)
declare @DESCRIPTION as nvarchar(max)
declare @count as int


set @CONTROL_CODE = 'AlertEdit.ascx'
set @ATTR_NAME = 'DateTimeRegExpression'
set @ATTR_VALUE = '^[0-9]{2}\/[0|1]{1}[0-9]{1}\/[0-9]{4}[\s]{1}[012]{1}[0-9]{1}[:][012345]{1}[0-9]{1}$'
set @DESCRIPTION = 'To validate date time format'
set @count = (select count(*) from tbl_control_attribute where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [ATTR_NAME]=@ATTR_NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_attribute] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[ATTR_NAME],[ATTR_VALUE],[DESCRIPTION])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@ATTR_NAME,@ATTR_VALUE,@DESCRIPTION)
end
else
begin
print @ATTR_NAME + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @ATTR_NAME = 'SendEmailRegExpression'
set @ATTR_VALUE = '^.+@{1}.+\.{1}[A-Za-z0-9]{2,}$'
set @DESCRIPTION = 'To validate email address format'
set @count = (select count(*) from tbl_control_attribute where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [ATTR_NAME]=@ATTR_NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_attribute] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[ATTR_NAME],[ATTR_VALUE],[DESCRIPTION])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@ATTR_NAME,@ATTR_VALUE,@DESCRIPTION)
end
else
begin
print @ATTR_NAME + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @ATTR_NAME = 'LinkRegExpression'
set @ATTR_VALUE = '^[a-zA-Z0-9]+(.)*$'
set @DESCRIPTION = 'To validate link'
set @count = (select count(*) from tbl_control_attribute where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [ATTR_NAME]=@ATTR_NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_attribute] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[ATTR_NAME],[ATTR_VALUE],[DESCRIPTION])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@ATTR_NAME,@ATTR_VALUE,@DESCRIPTION)
end
else
begin
print @ATTR_NAME + ' for ' + @CONTROL_CODE + ' already exists.'
end


set @CONTROL_CODE = 'AlertList.ascx'
set @ATTR_NAME = 'AlertsPerPage'
set @ATTR_VALUE = '25'
set @DESCRIPTION = 'Number of alerts listing per page'
set @count = (select count(*) from tbl_control_attribute where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [ATTR_NAME]=@ATTR_NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_attribute] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[ATTR_NAME],[ATTR_VALUE],[DESCRIPTION])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@ATTR_NAME,@ATTR_VALUE,@DESCRIPTION)
end
else
begin
print @ATTR_NAME + ' for ' + @CONTROL_CODE + ' already exists.'
end


set @BUSINESS_UNIT = 'UNITEDKINGDOM'
set @CONTROL_CODE = 'UserAlerts.ascx'
set @ATTR_NAME = 'AlertLinkRelAttributeValue'
set @ATTR_VALUE = 'shadowbox'
set @DESCRIPTION = 'rel attribute for the shadow box display'
set @count = (select count(*) from tbl_control_attribute where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [ATTR_NAME]=@ATTR_NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_attribute] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[ATTR_NAME],[ATTR_VALUE],[DESCRIPTION])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@ATTR_NAME,@ATTR_VALUE,@DESCRIPTION)
end
else
begin
print @ATTR_NAME + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @BUSINESS_UNIT = 'UNITEDKINGDOM'
set @CONTROL_CODE = 'UserAlerts.ascx'
set @ATTR_NAME = 'JavaScriptString'
set @ATTR_VALUE = '<script type="text/javascript">window.onload = function () {OpenAlertsWindow();};</script>'
set @DESCRIPTION = 'JavaScriptString setting: to automatically open the shadow box when alerts are present'
set @count = (select count(*) from tbl_control_attribute where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [ATTR_NAME]=@ATTR_NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_attribute] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[ATTR_NAME],[ATTR_VALUE],[DESCRIPTION])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@ATTR_NAME,@ATTR_VALUE,@DESCRIPTION)
end
else
begin
print @ATTR_NAME + ' for ' + @CONTROL_CODE + ' already exists.'
end

