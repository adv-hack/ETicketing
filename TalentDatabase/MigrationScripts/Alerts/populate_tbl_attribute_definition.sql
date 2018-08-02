declare @BUSINESS_UNIT as nvarchar(30) = 'UNITEDKINGDOM'
declare @PARTNER as nvarchar(30)  = 'PUBLIC'
declare @NAME as nvarchar(30)  
declare @count as int

set @NAME = 'BirthdayAlert'
set @count = (select count(*) from tbl_attribute_definition where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [NAME]=@NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_attribute_definition] 
           ([BUSINESS_UNIT],[PARTNER],[CATEGORY],[NAME],[DESCRIPTION],[TYPE],[FOREIGN_KEY],[SOURCE])
     VALUES ('UNITEDKINGDOM','PUBLIC','xxx',@NAME,@NAME,'2','1000000000001','TALENTCRMDATERELATED')
end
else
begin
print 'Attribute definition already exists.'
end

set @NAME = 'FFBirthdayAlert'
set @count = (select count(*) from tbl_attribute_definition where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [NAME]=@NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_attribute_definition]
           ([BUSINESS_UNIT],[PARTNER],[CATEGORY],[NAME],[DESCRIPTION],[TYPE],[FOREIGN_KEY],[SOURCE])
     VALUES ('UNITEDKINGDOM','PUBLIC','xxx',@NAME,@NAME,'2','1000000000002','TALENTCRMDATERELATED')
end
else
begin
print 'Attribute definition already exists.'
end

set @NAME = 'CCExpiryAlertPPS'
set @count = (select count(*) from tbl_attribute_definition where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [NAME]=@NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_attribute_definition]
           ([BUSINESS_UNIT],[PARTNER],[CATEGORY],[NAME],[DESCRIPTION],[TYPE],[FOREIGN_KEY],[SOURCE])
     VALUES ('UNITEDKINGDOM','PUBLIC','xxx',@NAME,@NAME,'2','1000000000003','TALENTCRMDATERELATED')
end
else
begin
print 'Attribute definition already exists.'
end

set @NAME = 'CCExpiryAlertSAV'
set @count = (select count(*) from tbl_attribute_definition where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [NAME]=@NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_attribute_definition]
           ([BUSINESS_UNIT],[PARTNER],[CATEGORY],[NAME],[DESCRIPTION],[TYPE],[FOREIGN_KEY],[SOURCE])
     VALUES ('UNITEDKINGDOM','PUBLIC','xxx',@NAME,@NAME,'2','1000000000004','TALENTCRMDATERELATED')
end
else
begin
print 'Attribute definition already exists.'
end

set @NAME = 'ReservationAlert'
set @count = (select count(*) from tbl_attribute_definition where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [NAME]=@NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_attribute_definition]
           ([BUSINESS_UNIT],[PARTNER],[CATEGORY],[NAME],[DESCRIPTION],[TYPE],[FOREIGN_KEY],[SOURCE])
     VALUES ('BOXOFFICE','PUBLIC','xxx',@NAME,@NAME,'2','1000000000005','TALENTCRMDATERELATED')
end
else
begin
print 'Attribute definition already exists.'
end

GO