
declare @BUSINESS_UNIT as nvarchar(50) = 'UNITEDKINGDOM'
declare @PARTNER as nvarchar(50)  = '*ALL'
declare @DEFAULT_NAME as nvarchar(50)
declare @VALUE as nvarchar(max)
declare @count as int


set @DEFAULT_NAME = 'ALERTS_ENABLED'
set @VALUE = 'True'
set @count = (select count(*) from tbl_ecommerce_module_defaults_bu where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [DEFAULT_NAME]=@DEFAULT_NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_ecommerce_module_defaults_bu] ([BUSINESS_UNIT],[PARTNER],[APPLICATION],[MODULE],[DEFAULT_NAME],[VALUE])
VALUES (@BUSINESS_UNIT,@PARTNER,'','',@DEFAULT_NAME,@VALUE)
end
else
begin
print @DEFAULT_NAME + ' already exists.'
end


set @DEFAULT_NAME = 'ALERTS_REFRESH_ATTRIBUTES_AT_LOGIN'
set @VALUE = 'True'
set @count = (select count(*) from tbl_ecommerce_module_defaults_bu where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [DEFAULT_NAME]=@DEFAULT_NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_ecommerce_module_defaults_bu] ([BUSINESS_UNIT],[PARTNER],[APPLICATION],[MODULE],[DEFAULT_NAME],[VALUE])
VALUES (@BUSINESS_UNIT,@PARTNER,'','',@DEFAULT_NAME,@VALUE)
end
else
begin
print @DEFAULT_NAME + ' already exists.'
end

set @DEFAULT_NAME = 'ALERTS_GENERATE_AT_LOGIN'
set @VALUE = 'True'
set @count = (select count(*) from tbl_ecommerce_module_defaults_bu where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [DEFAULT_NAME]=@DEFAULT_NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_ecommerce_module_defaults_bu] ([BUSINESS_UNIT],[PARTNER],[APPLICATION],[MODULE],[DEFAULT_NAME],[VALUE])
VALUES (@BUSINESS_UNIT,@PARTNER,'','',@DEFAULT_NAME,@VALUE)
end
else
begin
print @DEFAULT_NAME + ' already exists.'
end

set @DEFAULT_NAME = 'ALERTS_CC_EXPIRY_PPS_WARN_PERIOD'
set @VALUE = '30'
set @count = (select count(*) from tbl_ecommerce_module_defaults_bu where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [DEFAULT_NAME]=@DEFAULT_NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_ecommerce_module_defaults_bu] ([BUSINESS_UNIT],[PARTNER],[APPLICATION],[MODULE],[DEFAULT_NAME],[VALUE])
VALUES (@BUSINESS_UNIT,@PARTNER,'','',@DEFAULT_NAME,@VALUE)
end
else
begin
print @DEFAULT_NAME + ' already exists.'
end

set @DEFAULT_NAME = 'ALERTS_CC_EXPIRY_SAV_WARN_PERIOD'
set @VALUE = '30'
set @count = (select count(*) from tbl_ecommerce_module_defaults_bu where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [DEFAULT_NAME]=@DEFAULT_NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_ecommerce_module_defaults_bu] ([BUSINESS_UNIT],[PARTNER],[APPLICATION],[MODULE],[DEFAULT_NAME],[VALUE])
VALUES (@BUSINESS_UNIT,@PARTNER,'','',@DEFAULT_NAME,@VALUE)
end
else
begin
print @DEFAULT_NAME + ' already exists.'
end

set @DEFAULT_NAME = 'ROOT_IMAGE_PATH_ABSOLUTE'
set @VALUE = 'D:\TalentEBusinessSuiteAssets\club-name-in-here\Test\Images\Alerts'
set @count = (select count(*) from tbl_ecommerce_module_defaults_bu where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [DEFAULT_NAME]=@DEFAULT_NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_ecommerce_module_defaults_bu] ([BUSINESS_UNIT],[PARTNER],[APPLICATION],[MODULE],[DEFAULT_NAME],[VALUE])
VALUES (@BUSINESS_UNIT,@PARTNER,'','',@DEFAULT_NAME,@VALUE)
end
else
begin
print @DEFAULT_NAME + ' already exists.'
end

set @DEFAULT_NAME = 'ROOT_IMAGE_PATH_LOCAL'
set @VALUE = '/Assets/Images/Alerts'
set @count = (select count(*) from tbl_ecommerce_module_defaults_bu where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [DEFAULT_NAME]=@DEFAULT_NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_ecommerce_module_defaults_bu] ([BUSINESS_UNIT],[PARTNER],[APPLICATION],[MODULE],[DEFAULT_NAME],[VALUE])
VALUES (@BUSINESS_UNIT,@PARTNER,'','',@DEFAULT_NAME,@VALUE)
end
else
begin
print @DEFAULT_NAME + ' already exists.'
end


