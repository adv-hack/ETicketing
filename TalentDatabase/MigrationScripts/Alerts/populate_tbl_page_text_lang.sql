
declare @BUSINESS_UNIT as nvarchar(30) 
declare @PARTNER as nvarchar(30)  = '*ALL'
declare @PAGE_CODE as nvarchar(30)
declare @TEXT_CODE as nvarchar(50)
declare @LANG_CODE as nvarchar(20) = 'ENG'
declare @TEXT_CONTENT as nvarchar(max)
declare @count as int


set @BUSINESS_UNIT = 'UNITEDKINGDOM'
set @PAGE_CODE = 'Alerts.aspx'

set @TEXT_CODE = 'RemoveThisAlertButtonText'
set @TEXT_CONTENT = 'Remove this alert'
set @count = (select count(*) from tbl_page_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_page_text_lang] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[TEXT_CODE],[LANGUAGE_CODE],[TEXT_CONTENT])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@TEXT_CODE,@LANG_CODE,@TEXT_CONTENT)
end
else
begin
print @TEXT_CODE + ' on ' + @PAGE_CODE + ' already exists.'
end

set @TEXT_CODE = 'ShadowBoxCloseJS'
set @TEXT_CONTENT = '<script language="javascript"> window.parent.location.reload();    if(parent.Shadowbox.isOpen()) { parent.Shadowbox.close(); } </script>'
set @count = (select count(*) from tbl_page_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_page_text_lang] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[TEXT_CODE],[LANGUAGE_CODE],[TEXT_CONTENT])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@TEXT_CODE,@LANG_CODE,@TEXT_CONTENT)
end
else
begin
print @TEXT_CODE + ' on ' + @PAGE_CODE + ' already exists.'
end


set @BUSINESS_UNIT = 'MAINTENANCE'
set @PAGE_CODE = 'AlertSelect.aspx'

set @TEXT_CODE = 'HomeLink'
set @TEXT_CONTENT = 'Maintenance Portal'
set @count = (select count(*) from tbl_page_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_page_text_lang] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[TEXT_CODE],[LANGUAGE_CODE],[TEXT_CONTENT])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@TEXT_CODE,@LANG_CODE,@TEXT_CONTENT)
end
else
begin
print @TEXT_CODE + ' on ' + @PAGE_CODE + ' already exists.'
end



set @BUSINESS_UNIT = 'MAINTENANCE'
set @PAGE_CODE = 'AlertDetail.aspx'

set @TEXT_CODE = 'PageTitleUpdateAlertText'
set @TEXT_CONTENT = 'Alert Management (Update)'
set @count = (select count(*) from tbl_page_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_page_text_lang] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[TEXT_CODE],[LANGUAGE_CODE],[TEXT_CONTENT])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@TEXT_CODE,@LANG_CODE,@TEXT_CONTENT)
end
else
begin
print @TEXT_CODE + ' on ' + @PAGE_CODE + ' already exists.'
end

set @TEXT_CODE = 'PageInstructionText'
set @TEXT_CONTENT = 'Manage Customer Alert Definition and Critera'
set @count = (select count(*) from tbl_page_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_page_text_lang] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[TEXT_CODE],[LANGUAGE_CODE],[TEXT_CONTENT])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@TEXT_CODE,@LANG_CODE,@TEXT_CONTENT)
end
else
begin
print @TEXT_CODE + ' on ' + @PAGE_CODE + ' already exists.'
end

set @TEXT_CODE = 'ButtonSaveText'
set @TEXT_CONTENT = 'Save Alert'
set @count = (select count(*) from tbl_page_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_page_text_lang] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[TEXT_CODE],[LANGUAGE_CODE],[TEXT_CONTENT])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@TEXT_CODE,@LANG_CODE,@TEXT_CONTENT)
end
else
begin
print @TEXT_CODE + ' on ' + @PAGE_CODE + ' already exists.'
end

set @TEXT_CODE = 'ButtonCancelText'
set @TEXT_CONTENT = 'Back'
set @count = (select count(*) from tbl_page_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_page_text_lang] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[TEXT_CODE],[LANGUAGE_CODE],[TEXT_CONTENT])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@TEXT_CODE,@LANG_CODE,@TEXT_CONTENT)
end
else
begin
print @TEXT_CODE + ' on ' + @PAGE_CODE + ' already exists.'
end

set @TEXT_CODE = 'ButtonUpdateText'
set @TEXT_CONTENT = 'Update Alert'
set @count = (select count(*) from tbl_page_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_page_text_lang] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[TEXT_CODE],[LANGUAGE_CODE],[TEXT_CONTENT])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@TEXT_CODE,@LANG_CODE,@TEXT_CONTENT)
end
else
begin
print @TEXT_CODE + ' on ' + @PAGE_CODE + ' already exists.'
end

set @TEXT_CODE = 'PageTitleSaveAlertText'
set @TEXT_CONTENT = 'Alert Management (Adding New)'
set @count = (select count(*) from tbl_page_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_page_text_lang] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[TEXT_CODE],[LANGUAGE_CODE],[TEXT_CONTENT])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@TEXT_CODE,@LANG_CODE,@TEXT_CONTENT)
end
else
begin
print @TEXT_CODE + ' on ' + @PAGE_CODE + ' already exists.'
end

set @TEXT_CODE = 'ButtonDeleteText'
set @TEXT_CONTENT = 'Delete'
set @count = (select count(*) from tbl_page_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_page_text_lang] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[TEXT_CODE],[LANGUAGE_CODE],[TEXT_CONTENT])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@TEXT_CODE,@LANG_CODE,@TEXT_CONTENT)
end
else
begin
print @TEXT_CODE + ' on ' + @PAGE_CODE + ' already exists.'
end


set @TEXT_CODE = 'HomeLink'
set @TEXT_CONTENT = 'Maintenance Portal'
set @count = (select count(*) from tbl_page_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_page_text_lang] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[TEXT_CODE],[LANGUAGE_CODE],[TEXT_CONTENT])
VALUES (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@TEXT_CODE,@LANG_CODE,@TEXT_CONTENT)
end
else
begin
print @TEXT_CODE + ' on ' + @PAGE_CODE + ' already exists.'
end


