
declare @LANGUAGE_CODE as nvarchar(20) = 'ENG'
declare @BUSINESS_UNIT as nvarchar(30) = '*ALL'
declare @PARTNER as nvarchar(30)  = '*ALL'
declare @PAGE_CODE as nvarchar(50) = '*ALL'
declare @CONTROL_CODE as nvarchar(max)
declare @TEXT_CODE as nvarchar(50)
declare @CONTROL_CONTENT as nvarchar(max)
declare @count as int


set @CONTROL_CODE = 'AlertCriterias.ascx'
set @TEXT_CODE = 'AttributeRFVErrMessage'
set @CONTROL_CONTENT = 'AttributeRFVErrMessage'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertCriterias.ascx'
set @TEXT_CODE = 'CriteriasTitle'
set @CONTROL_CONTENT = 'Alert Criteria'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end


set @CONTROL_CODE = 'AlertCriterias.ascx'
set @TEXT_CODE = 'InvalidAttributeErrMessage'
set @CONTROL_CONTENT = 'Please enter valid attribute'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end


set @CONTROL_CODE = 'AlertCriterias.ascx'
set @TEXT_CODE = 'ExistsAttMissingErrMess'
set @CONTROL_CONTENT = 'Existing attributes are missing. Please replace those with any available attributes'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end


set @CONTROL_CODE = 'AlertCriterias.ascx'
set @TEXT_CODE = 'RowAddButtonText'
set @CONTROL_CONTENT = 'Add Row'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end


set @CONTROL_CODE = 'AlertCriterias.ascx'
set @TEXT_CODE = 'InvalidAttributeText'
set @CONTROL_CONTENT = 'INVALID ATTRIBUTE'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end


set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'LabelAction'
set @CONTROL_CONTENT = 'LabelAction'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'LabelActionDetail'
set @CONTROL_CONTENT = 'Action Detail'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'LabelActivationStartDate'
set @CONTROL_CONTENT = 'Activation Start Date'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'LabelActivationEndDate'
set @CONTROL_CONTENT = 'Activation End Date Time'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'LabelDescription'
set @CONTROL_CONTENT = 'Description'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'LabelEnabled'
set @CONTROL_CONTENT = 'Active'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'LabelImagePath'
set @CONTROL_CONTENT = 'Image Path'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'LabelName'
set @CONTROL_CONTENT = 'Name'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'NameRFVErrMess'
set @CONTROL_CONTENT = 'Please Enter Alert Name'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'DescriptionRFVErrMess'
set @CONTROL_CONTENT = 'Please Enter Alert Description'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'StartDateRFVErrMess'
set @CONTROL_CONTENT = 'Please Enter Start Date'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'EndDateRFVErrMess'
set @CONTROL_CONTENT = 'Please Enter End Date'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'ActionDetailRFVErrMess'
set @CONTROL_CONTENT = 'Please Enter Action Detail'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'ActionDetailCVErrMess'
set @CONTROL_CONTENT = 'Please enter valid action detail for the selected action'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'DescriptionCVErrMess'
set @CONTROL_CONTENT = 'Only 200 characters are allowed for description'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'SubjectCVErrMess'
set @CONTROL_CONTENT = 'Only 200 characters are allowed for subject'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'LabelSubject'
set @CONTROL_CONTENT = 'Subject'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'SubjectRFVErrMess'
set @CONTROL_CONTENT = 'Please enter subject'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'StartDateREVErrMess'
set @CONTROL_CONTENT = 'DD/MM/YYYY HH:MM is the allowed format for start date and time'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'EndDateREVErrMess'
set @CONTROL_CONTENT = 'DD/MM/YYYY HH:MM is the allowed format for end date and time'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'EndDateCVErrMess'
set @CONTROL_CONTENT = 'End Date should be greater than Start Date'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'BrowseButton'
set @CONTROL_CONTENT = 'Browse'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'lblActionDetailURLOption'
set @CONTROL_CONTENT = 'Open in New Window?'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertEdit.ascx'
set @TEXT_CODE = 'lblActionDetailFFOption'
set @CONTROL_CONTENT = 'Use Friends Email Address'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @CONTROL_CODE = 'AlertList.ascx'
set @TEXT_CODE = 'NewAlertButtonText'
set @CONTROL_CONTENT = 'Add New Alert'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @BUSINESS_UNIT = 'UNITEDKINGDOM'
set @CONTROL_CODE = 'UserAlerts.ascx'
set @TEXT_CODE = 'SingleAlertText'
set @CONTROL_CONTENT = 'You have <<NUMBER_OF_ALERTS>> unread alert.'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end


set @BUSINESS_UNIT = 'UNITEDKINGDOM'
set @CONTROL_CODE = 'UserAlerts.ascx'
set @TEXT_CODE = 'MultipleAlertText'
set @CONTROL_CONTENT = 'You have <<NUMBER_OF_ALERTS>> unread alerts.'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end



set @BUSINESS_UNIT = '*ALL'
set @CONTROL_CODE = 'AttributeDefinition.ascx'
set @TEXT_CODE = 'ButtonRefreshAttDefText'
set @CONTROL_CONTENT = 'Refresh Attributes Definition'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @BUSINESS_UNIT = '*ALL'
set @CONTROL_CODE = 'AttributeDefinition.ascx'
set @TEXT_CODE = 'RefreshFailedMessage'
set @CONTROL_CONTENT = 'Failed to refresh attributes definition'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @BUSINESS_UNIT = '*ALL'
set @CONTROL_CODE = 'AttributeDefinition.ascx'
set @TEXT_CODE = 'RefreshSuccessMessage'
set @CONTROL_CONTENT = 'Attributes definition successfully refreshed'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end

set @BUSINESS_UNIT = '*ALL'
set @CONTROL_CODE = 'AttributeDefinition.ascx'
set @TEXT_CODE = 'RebuildAttDefFailedMessage'
set @CONTROL_CONTENT = 'Failed to rebuild attributes definition. May be an issue with refresh.'
set @count = (select count(*) from tbl_control_text_lang where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER_CODE]=@PARTNER and [PAGE_CODE]=@PAGE_CODE and [CONTROL_CODE]=@CONTROL_CODE and [TEXT_CODE]=@TEXT_CODE)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT])
VALUES (@LANGUAGE_CODE,@BUSINESS_UNIT,@PARTNER,@PAGE_CODE,@CONTROL_CODE,@TEXT_CODE,@CONTROL_CONTENT)
end
else
begin
print @TEXT_CODE + ' for ' + @CONTROL_CODE + ' already exists.'
end
