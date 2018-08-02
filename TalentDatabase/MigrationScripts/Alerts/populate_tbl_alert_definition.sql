declare @BUSINESS_UNIT as nvarchar(30) = 'UNITEDKINGDOM'
declare @PARTNER as nvarchar(30)  = 'PUBLIC'
declare @NAME as nvarchar(30)  = ''
declare @count as int

set @NAME = 'BirthdayAlert'
set @count = (select count(*) from tbl_alert_definition where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [NAME]=@NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_alert_definition]
([BUSINESS_UNIT],[PARTNER],[NAME],[DESCRIPTION],[IMAGE_PATH],[ACTION],[ACTION_DETAILS],[ACTIVATION_START_DATETIME],[ACTIVATION_END_DATETIME],[NON_STANDARD],[ENABLED],[SUBJECT],[DELETED])
VALUES
('UNITEDKINGDOM','PUBLIC',@NAME,'It''s your birthday. Congratulations from everyone at Iris.','','link','http://www.google.co.uk/search?q=happy%20birthday','2010-01-01 00:00:00.000','2099-01-01 00:00:00.000',1,1,'HappyBirthday!','False')
end
else
begin
print 'Alert definition already exists.'
end


set @NAME = 'FFBirthdayAlert'
set @count = (select count(*) from tbl_alert_definition where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [NAME]=@NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_alert_definition]
([BUSINESS_UNIT],[PARTNER],[NAME],[DESCRIPTION],[IMAGE_PATH],[ACTION],[ACTION_DETAILS],[ACTIVATION_START_DATETIME],[ACTIVATION_END_DATETIME],[NON_STANDARD],[ENABLED],[SUBJECT],[DELETED])
VALUES
('UNITEDKINGDOM','PUBLIC',@NAME,'It''s <<<customer_forename>>> <<<customer_surname>>>s Birthday','','SendEmail','friend@email.address','2010-01-01 00:00:00.000','2099-01-01 00:00:00.000',1,1,'Don''t forget your friend''s birthday','False')
end
else
begin
print 'Alert definition already exists.'
end


set @NAME = 'CCExpiryAlertPPS'
set @count = (select count(*) from tbl_alert_definition where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [NAME]=@NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_alert_definition]
([BUSINESS_UNIT],[PARTNER],[NAME],[DESCRIPTION],[IMAGE_PATH],[ACTION],[ACTION_DETAILS],[ACTIVATION_START_DATETIME],[ACTIVATION_END_DATETIME],[NON_STANDARD],[ENABLED],[SUBJECT],[DELETED])
VALUES
('UNITEDKINGDOM','PUBLIC',@NAME,'Your credit card ending <<<cc_ending_in>>> is about to expire at end of <<<expires_in>>>','','link','http://ticketing-website-url/PagesPublic/ProductBrowse/AmendPPSEnrolment.aspx','2010-01-01 00:00:00.000','2099-01-01 00:00:00.000',1,1,'Request for CC details update','False')
end
else
begin
print 'Alert definition already exists.'
end


set @NAME = 'CCExpiryAlertSAV'
set @count = (select count(*) from tbl_alert_definition where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [NAME]=@NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_alert_definition]
([BUSINESS_UNIT],[PARTNER],[NAME],[DESCRIPTION],[IMAGE_PATH],[ACTION],[ACTION_DETAILS],[ACTIVATION_START_DATETIME],[ACTIVATION_END_DATETIME],[NON_STANDARD],[ENABLED],[SUBJECT],[DELETED])
VALUES
('UNITEDKINGDOM','PUBLIC',@NAME,'Your credit card ending <<<cc_ending_in>>> is about to expire at end of <<<expires_in>>>','','link','http://ticketing-website-url/Pageslogin/Profile/SaveMyCard.aspx','2010-01-01 00:00:00.000','2099-01-01 00:00:00.000',1,1,'Request for CC details update','False')
end
else
begin
print 'Alert definition already exists.'
end

set @NAME = 'CCExpiryAlertSAV'
set @count = (select count(*) from tbl_alert_definition where [BUSINESS_UNIT]=@BUSINESS_UNIT and [PARTNER]=@PARTNER and [NAME]=@NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_alert_definition]
([BUSINESS_UNIT],[PARTNER],[NAME],[DESCRIPTION],[IMAGE_PATH],[ACTION],[ACTION_DETAILS],[ACTIVATION_START_DATETIME],[ACTIVATION_END_DATETIME],[NON_STANDARD],[ENABLED],[SUBJECT],[DELETED])
VALUES
('UNITEDKINGDOM','PUBLIC',@NAME,'Your credit card ending <<<cc_ending_in>>> is about to expire at end of <<<expires_in>>>','','link','http://ticketing-website-url/Pageslogin/Profile/SaveMyCard.aspx','2010-01-01 00:00:00.000','2099-01-01 00:00:00.000',1,1,'Request for CC details update','False')
end
else
begin
print 'Alert definition already exists.'
end

set @NAME = 'ReservationAlert'
set @count = (select count(*) from tbl_alert_definition where [BUSINESS_UNIT]='BOXOFFICE' and [PARTNER]=@PARTNER and [NAME]=@NAME)
if @count = 0 
begin
INSERT INTO [dbo].[tbl_alert_definition]
([BUSINESS_UNIT],[PARTNER],[NAME],[DESCRIPTION],[IMAGE_PATH],[ACTION],[ACTION_DETAILS],[ACTIVATION_START_DATETIME],[ACTIVATION_END_DATETIME],[NON_STANDARD],[ENABLED],[SUBJECT],[DELETED])
VALUES
('BOXOFFICE','PUBLIC',@NAME,'<<<customer_name>>> has a total of <<<total_reservations>>> total reservations and <<<total_ff_reservations>>> friends and family reservations.','','CustomerReservation','http://ticketing-website-url/BoxOffice/Pageslogin/Profile/CustomerReservations.aspx','2010-01-01 00:00:00.000','2099-01-01 00:00:00.000',1,1,'Customer Reservation notification','False')
end
else
begin
print 'Alert definition already exists.'
end
