declare @BUSINESS_UNIT as nvarchar(30) = 'UNITEDKINGDOM'
declare @PARTNER as nvarchar(30)  = 'PUBLIC'
declare @ATTR_ID as nvarchar(30)  = ''
declare @count as int

set @ATTR_ID = '1000000000001'
set @count = (select count(*) from tbl_alert_critera where [ATTR_ID]=@ATTR_ID)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_alert_critera]
           ([ALERT_ID],[ATTR_ID],[ALERT_OPERATOR],[SEQUENCE],[CLAUSE],[CLAUSE_TYPE])
     VALUES (1,@ATTR_ID,'',1,1,'AND')
end
else
begin
print 'Alert criteria already exists.'
end

set @ATTR_ID = '1000000000002'
set @count = (select count(*) from tbl_alert_critera where [ATTR_ID]=@ATTR_ID)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_alert_critera]
           ([ALERT_ID],[ATTR_ID],[ALERT_OPERATOR],[SEQUENCE],[CLAUSE],[CLAUSE_TYPE])
     VALUES (2,@ATTR_ID,'',1,1,'AND')
end
else
begin
print 'Alert criteria already exists.'
end



set @ATTR_ID = '1000000000003'
set @count = (select count(*) from tbl_alert_critera where [ATTR_ID]=@ATTR_ID)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_alert_critera]
           ([ALERT_ID],[ATTR_ID],[ALERT_OPERATOR],[SEQUENCE],[CLAUSE],[CLAUSE_TYPE])
     VALUES (3,@ATTR_ID,'',1,1,'AND')
end
else
begin
print 'Alert criteria already exists.'
end


set @ATTR_ID = '1000000000004'
set @count = (select count(*) from tbl_alert_critera where [ATTR_ID]=@ATTR_ID)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_alert_critera]
           ([ALERT_ID],[ATTR_ID],[ALERT_OPERATOR],[SEQUENCE],[CLAUSE],[CLAUSE_TYPE])
     VALUES (4,@ATTR_ID,'',1,1,'AND')
end
else
begin
print 'Alert criteria already exists.'
end

set @ATTR_ID = '1000000000005'
set @count = (select count(*) from tbl_alert_critera where [ATTR_ID]=@ATTR_ID)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_alert_critera]
           ([ALERT_ID],[ATTR_ID],[ALERT_OPERATOR],[SEQUENCE],[CLAUSE],[CLAUSE_TYPE])
     VALUES (5,@ATTR_ID,'',1,1,'AND')
end
else
begin
print 'Alert criteria already exists.'
end

GO