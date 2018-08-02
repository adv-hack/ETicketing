declare @businessunit as nvarchar(50) = 'UNITEDKINGDOM'
declare @partner as nvarchar(50) = 'PUBLIC'

declare @lang as nvarchar(50) = 'ENG'
declare @businessunitALL as nvarchar(50) = '*ALL'
declare @partnerALL as nvarchar(50) = '*ALL'
declare @pagecodeALL as nvarchar(50) = '*ALL'

declare @emailtemplate_id as int
declare @type as nvarchar(100)
declare @subject as nvarchar(200) 
declare @from_address as nvarchar(200) 
declare @body as nvarchar(max) 
declare @html as bit 

declare @control_code_new as nvarchar(max)
declare @generic_control_code as nvarchar(max) = 'TalentEmail.vb'
declare @control_code as nvarchar(max)
declare @text_code as nvarchar(50)
declare @control_content as nvarchar(max)
declare @attr_name as nvarchar(50)
declare @attr_value as nvarchar(max)

set @body = 
'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xmlns="http://www.w3.org/1999/xhtml" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;">
<head>
<!-- If you delete this meta tag, Half Life 3 will never be released. -->
<meta name="viewport" content="width=device-width" />

<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
<title>Q&amp;A reminder email</title>
	


<style>img {
max-width: 100%;
}
body {
-webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; width: 100% !important; height: 100%;
}
@media only screen and (max-width: 600px) {
  a[class="btn"] {
    display: block !important; margin-bottom: 10px !important; background-image: none !important; margin-right: 0 !important;
  }
  div[class="column"] {
    width: auto !important; float: none !important;
  }
  table.social div[class="column"] {
    width: auto !important;
  }
}
</style></head>
 
<body bgcolor="#FFFFFF" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; width: 100% !important; height: 100%; margin: 0; padding: 0;">

<!-- HEADER -->
<table class="head-wrap" bgcolor="#333333" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; width: 100%; margin: 0; padding: 0;">
	<tr style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;">
		<td style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;"></td>
		<td class="header container" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; display: block !important; max-width: 600px !important; clear: both !important; margin: 0 auto; padding: 0;">
				
				<div class="content" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; max-width: 600px; display: block; margin: 0 auto; padding: 15px;">
				<table bgcolor="#333333" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; width: 100%; margin: 0; padding: 0;">
					<tr style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;">
						<td style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;"><img src="https://a248.e.akamai.net/images.talentarena.co.uk/Global/Assets/ADV.png" alt="ADV" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; max-width: 100%; margin: 0; padding: 0;" /></td>
					</tr>
				</table>
				</div>
				
		</td>
		<td style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;"></td>
	</tr>
</table><!-- /HEADER -->


<!-- BODY -->
<table class="body-wrap" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; width: 100%; margin: 0; padding: 0;">
	<tr style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;">
		<td style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;"></td>
		<td class="container" bgcolor="#FFFFFF" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; display: block !important; max-width: 600px !important; clear: both !important; margin: 0 auto; padding: 0;">

			<div class="content" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; max-width: 600px; display: block; margin: 0 auto; padding: 15px;">
			<table style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; width: 100%; margin: 0; padding: 0;">
				<tr style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;">
					<td style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;">
						<h3 style="font-family: &quot;HelveticaNeue-Light&quot;, &quot;Helvetica Neue Light&quot;, &quot;Helvetica Neue&quot;, Helvetica, Arial, &quot;Lucida Grande&quot;, sans-serif; line-height: 1.1; color: #333333; font-weight: 500; font-size: 27px; margin: 0 0 15px; padding: 0;">Hi [[Title]] [[Surname]],</h3>
						<!-- Callout Panel -->
						<p style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; font-weight: normal; font-size: 14px; line-height: 1.6; color: #333333; margin: 0 0 10px; padding: 0;">To make the day as special as possible we need you to answer a few questions relating to your booking. Please follow this link [[INSERT LINK TO BOOKING PAGE HERE]] and complete your booking questionnaire.</p>						
						<p class="callout" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; background-color: #d9edf7; border-radius: 4px; color: #31708f; font-weight: normal; font-size: 14px; line-height: 1.6; margin: 0 0 15px; padding: 15px; border: 1px solid #bce8f1;">
							If you have any questions or queries, please contact the Ticket office via our <a href="#" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; color: #31708f; font-weight: bold; margin: 0; padding: 0;">enquiry form</a> or telephone a member of our team on 01234 567890
						</p><!-- /Callout Panel -->															
						
					</td>
				</tr>
			</table>
			</div><!-- /content -->
									
		</td>
		<td style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;"></td>
	</tr>
</table><!-- /BODY -->

<!-- FOOTER -->
<table class="footer-wrap" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; width: 100%; clear: both !important; margin: 0; padding: 0;">
	<tr style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;">
		<td style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;"></td>
		<td class="container" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; display: block !important; max-width: 600px !important; clear: both !important; margin: 0 auto; padding: 0;">
			
				<!-- content -->
				<div class="content" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; max-width: 600px; display: block; margin: 0 auto; padding: 15px;">
				<table style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; width: 100%; margin: 0; padding: 0;">
				<tr style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;">
					<td align="center" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;">
						<p style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; font-weight: normal; font-size: 14px; line-height: 1.6; color: #333333; margin: 0 0 10px; padding: 0;">
							<a href="#" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; color: #e9510e; margin: 0; padding: 0;">Terms</a> |
							<a href="#" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; color: #e9510e; margin: 0; padding: 0;">Privacy</a> |
							<a href="#" style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; color: #e9510e; margin: 0; padding: 0;"><unsubscribe style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;">Unsubscribe</unsubscribe></a>
						</p>
					</td>
				</tr>
			</table>
				</div><!-- /content -->
				
		</td>
		<td style="font-family: &quot;Helvetica Neue&quot;, &quot;Helvetica&quot;, Helvetica, Arial, sans-serif; margin: 0; padding: 0;"></td>
	</tr>
</table><!-- /FOOTER -->

</body>
</html>
'

-- Retrieve a list of business units which are used for ticketing emails
DECLARE @email_business_units TABLE ( business_unit NVARCHAR(1000) , email_from_address NVARCHAR(1000))
INSERT INTO @email_business_units 
SELECT DISTINCT BUSINESS_UNIT, EMAIL_FROM_ADDRESS FROM [TBL_EMAIL_TEMPLATES] WHERE TEMPLATE_TYPE = 'TicketingConfirmation' AND ACTIVE = 1

-- Loop through each business unit and add the hospitality Q&A email template
While EXISTS(SELECT * From @email_business_units)
BEGIN 

	Select Top 1 @businessunit = business_unit From @email_business_units
	Select Top 1 @from_address = email_from_address From @email_business_units

	set @type = 'HospitalityQ&AReminder'
	IF NOT EXISTS(SELECT * From tbl_email_templates a, @email_business_units b where a.business_unit = b.business_unit and TEMPLATE_TYPE = @type)
	BEGIN
		set @subject = 'Hospitality Q and A Reminder'					
		set @html = (select top 1 value from tbl_ecommerce_module_defaults_bu where DEFAULT_NAME = 'SEND_ORDER_CONFIRMATION_EMAIL_AS_HTML')

		INSERT INTO [tbl_email_templates] ([BUSINESS_UNIT],[PARTNER],[ACTIVE],[NAME],[DESCRIPTION],[TEMPLATE_TYPE],[EMAIL_HTML],[EMAIL_FROM_ADDRESS],[EMAIL_SUBJECT],[EMAIL_BODY],[ADDED_DATETIME],[UPDATED_DATETIME], [MASTER])
		VALUES (@businessunit,@partner,1,@type,@type,@type,@html,@from_address,@subject,@body,GetDate(),GetDate(),1)
	END

   DELETE FROM @email_business_units where @businessunit = business_unit  
END
