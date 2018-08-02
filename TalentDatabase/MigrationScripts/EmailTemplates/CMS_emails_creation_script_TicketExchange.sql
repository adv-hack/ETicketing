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

-- Retrieve a list of business units which are used for ticketing emails
DECLARE @email_business_units TABLE ( business_unit NVARCHAR(1000) , email_from_address NVARCHAR(1000))
INSERT INTO @email_business_units 
SELECT DISTINCT BUSINESS_UNIT, EMAIL_FROM_ADDRESS FROM [TBL_EMAIL_TEMPLATES] WHERE TEMPLATE_TYPE = 'TicketingConfirmation' AND ACTIVE = 1

-- Loop through each business unit and add the 2 ticket exchange email templates
While EXISTS(SELECT * From @email_business_units)
BEGIN 

	Select Top 1 @businessunit = business_unit From @email_business_units
	Select Top 1 @from_address = email_from_address From @email_business_units

	set @type = 'TicketExchangeConfirm'
	IF NOT EXISTS(SELECT * From tbl_email_templates a, @email_business_units b where a.business_unit = b.business_unit and TEMPLATE_TYPE = @type)
	BEGIN
		set @subject = 'Ticket Exchange Confirmation'		
		set @body =    ''		
		set @html = (select top 1 value from tbl_ecommerce_module_defaults_bu where DEFAULT_NAME = 'SEND_ORDER_CONFIRMATION_EMAIL_AS_HTML')

		INSERT INTO [tbl_email_templates] ([BUSINESS_UNIT],[PARTNER],[ACTIVE],[NAME],[DESCRIPTION],[TEMPLATE_TYPE],[EMAIL_HTML],[EMAIL_FROM_ADDRESS],[EMAIL_SUBJECT],[EMAIL_BODY],[ADDED_DATETIME],[UPDATED_DATETIME])
		VALUES (@businessunit,@partner,1,@type,@type,@type,@html,@from_address,@subject,@body,GetDate(),GetDate())
	END

	set @type = 'TicketExchangeSaleConfirm'
	IF NOT EXISTS(SELECT * From tbl_email_templates a, @email_business_units b where a.business_unit = b.business_unit and TEMPLATE_TYPE = @type)
	BEGIN
		set @subject = 'Ticket Exchange Sale Confirmation'
		set @body =    ''		
		set @html = (select top 1 value from tbl_ecommerce_module_defaults_bu where DEFAULT_NAME = 'SEND_ORDER_CONFIRMATION_EMAIL_AS_HTML')

		INSERT INTO [tbl_email_templates] ([BUSINESS_UNIT],[PARTNER],[ACTIVE],[NAME],[DESCRIPTION],[TEMPLATE_TYPE],[EMAIL_HTML],[EMAIL_FROM_ADDRESS],[EMAIL_SUBJECT],[EMAIL_BODY],[ADDED_DATETIME],[UPDATED_DATETIME])
		VALUES (@businessunit,@partner,1,@type,@type,@type,@html,@from_address,@subject,@body,GetDate(),GetDate())
	END

   DELETE FROM @email_business_units where @businessunit = business_unit  
END

-- Get the ID of the new ticket exchange templates - this is used to suffix the control_code for all the content records
DECLARE @newTemplates TABLE ( emailtemplate_id int, templateType NVARCHAR(1000))
INSERT INTO @newTemplates 
SELECT DISTINCT emailtemplate_id, TEMPLATE_TYPE FROM [TBL_EMAIL_TEMPLATES] WHERE TEMPLATE_TYPE IN ('TicketExchangeSaleConfirm', 'TicketExchangeConfirm') 

-- Loop through each template and add the required control text and attributes for the template id.
While EXISTS(SELECT * From @newTemplates)
BEGIN 
	Select Top 1 @emailtemplate_id = emailtemplate_id From @newTemplates
	Select Top 1 @type = templateType From @newTemplates
	set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@emailtemplate_id)

	DECLARE @newContent TABLE (code NVARCHAR(1000), content NVARCHAR(1000))
	IF @type = 'TicketExchangeConfirm'
	BEGIN
		INSERT INTO @newContent 
		VALUES	( 'ltlOnSale', 'On Sale' ), 
				(  'ltlRebooked', 'Reclaimed from Ticket Exchange' ), 
				(  'TicketExchangePriceChangeHeader', '<tr> <th scope="col">Name</th> <th scope="col"  style="white-space:nowrap">Membership No.</th> <th scope="col">Product</th> <th scope="col"  style="white-space:nowrap">Turnstile(s)</th> <th scope="col">Stand</th> <th scope="col">Block</th> <th scope="col">Row</th> <th scope="col">Seat</th> <th scope="col">Ticket Face Value</th><th scope="col">Original sale Ref</th><th scope="col">Previous Requested Price</th><th scope="col">Your Requested Price</th><th scope="col">Club Fee</th><th scope="col">You Will Earn</th><th scope="col">Ticket Exchange Ref</th></tr>' ), 
				(  'TicketExchangePriceChangeHeading', 'The following tickets have had their price altered on Ticket Exchange' ) ,
				(  'TicketExchangePriceChangeRow', '<tr> <td><<CustomerForename>> <<CustomerSurname>></td> <td><<CustomerNumber>></td> <td><<ProductDescription>></td> <td><<Turnstile>></td> <td><<Stand>></td> <td><<Area>></td> <td><<Row>></td> <td><<Seat>><<Alpha>></td><td><<FaceValue>></td><td><<OriginalSalePayref>></td><td><<PreviousRequestedPrice>></td><td><<RequestedPrice>></td><td><<ClubHandlingFee>></td><td><<Earnings>></td><td><<TicketExchangeRef>></td></tr>' ), 
				(  'TicketExchangePutOnSaleHeader', '<tr> <th scope="col">Name</th> <th scope="col"  style="white-space:nowrap">Membership No.</th> <th scope="col">Product</th> <th scope="col"  style="white-space:nowrap">Turnstile(s)</th> <th scope="col">Stand</th> <th scope="col">Block</th> <th scope="col">Row</th> <th scope="col">Seat</th> <th scope="col">Ticket Face Value</th><th scope="col">Original sale Ref</th><th scope="col">Your Requested Price</th><th scope="col">Club Fee</th><th scope="col">You Will Earn</th><th scope="col">Ticket Exchange Ref</th></tr>' ), 
				(  'TicketExchangePutOnSaleHeading', 'The following tickets were placed on Ticket Exchange' ), 
				(  'TicketExchangePutOnSaleRow', '<tr> <td><<CustomerForename>> <<CustomerSurname>></td> <td><<CustomerNumber>></td> <td><<ProductDescription>></td> <td><<Turnstile>></td> <td><<Stand>></td> <td><<Area>></td> <td><<Row>></td> <td><<Seat>><<Alpha>></td><td><<FaceValue>></td><td><<OriginalSalePayref>></td><td><<RequestedPrice>></td><td><<ClubHandlingFee>></td><td><<Earnings>></td><td><<TicketExchangeRef>></td></tr>' ), 
				(  'TicketExchangeRebookHeader', '<tr> <th scope="col">Name</th> <th scope="col"  style="white-space:nowrap">Membership No.</th> <th scope="col">Product</th> <th scope="col"  style="white-space:nowrap">Turnstile(s)</th> <th scope="col">Stand</th> <th scope="col">Block</th> <th scope="col">Row</th> <th scope="col">Seat</th> <th scope="col">Ticket Face Value</th><th scope="col">Original sale Ref</th><th scope="col">Ticket Exchange Ref</th></tr>' ), 
				(  'TicketExchangeRebookHeading', 'The following tickets have been reclaimed from Ticket Exchange' ), 
				(  'TicketExchangeRebookRow', '<tr> <td><<CustomerForename>> <<CustomerSurname>></td> <td><<CustomerNumber>></td> <td><<ProductDescription>></td> <td><<Turnstile>></td> <td><<Stand>></td> <td><<Area>></td> <td><<Row>></td> <td><<Seat>><<Alpha>></td><td><<FaceValue>></td><td><<OriginalSalePayref>></td><td><<TicketExchangeRef>></td></tr>' ), 
				(  'TicketExchangeRefText', '' )
	END
	IF @type = 'TicketExchangeSaleConfirm'
	BEGIN
		INSERT INTO @newContent 
		VALUES	( 'TicketExchangeSoldHeader', '<tr> <th scope="col">Name</th> <th scope="col"  style="white-space:nowrap">Membership No.</th> <th scope="col">Product</th> <th scope="col"  style="white-space:nowrap">Turnstile(s)</th> <th scope="col">Stand</th> <th scope="col">Block</th> <th scope="col">Row</th> <th scope="col">Seat</th> <th scope="col">Ticket Face Value</th><th scope="col">Your Exchange Price</th><th scope="col">Club Fee</th><th scope="col">Added to your On Account</th><th scope="col">Ticket Exchange Ref</th></tr>' ), 
				(  'TicketExchangeSoldHeading', 'You have successfully exchanged the following items' ), 
				(  'TicketExchangeSoldRow', '<tr> <td><<CustomerForename>> <<CustomerSurname>></td> <td><<CustomerNumber>></td> <td><<ProductDescription>></td> <td><<Turnstile>></td> <td><<Stand>></td> <td><<Area>></td> <td><<Row>></td> <td><<Seat>><<Alpha>></td><td><<FaceValue>></td><td><<RequestedPrice>></td><td><<ClubHandlingFee>></td><td><<Earnings>></td><td><<TicketExchangeRef>></td></tr>' ) 
	END

	While EXISTS(SELECT * From @newContent)
	BEGIN 
		Select Top 1 @text_code = code From @newContent
		Select Top 1 @control_content = content From @newContent
			IF NOT EXISTS(SELECT * From tbl_control_text_lang where control_code = @control_code_new and text_code = @text_code)
			BEGIN
				INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE]) 
				VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content,0)
			END
		DELETE FROM @newContent where @text_code = code
	END

	set @attr_name = 'TicketingTableStyle'
	set @attr_value = 'cellspacing="0" border="0" cellpadding="2" style="font-size:9pt;font-family:Arial,Helvetica,sans-serif;text-align:left;margin-top:1em;color: #0C2C50;"'
	IF NOT EXISTS(SELECT * From [tbl_control_attribute] where control_code = @control_code_new and attr_name = @attr_name)
	BEGIN		
		INSERT INTO [tbl_control_attribute] ([BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[ATTR_NAME],[ATTR_VALUE],[DESCRIPTION],[HIDE_IN_MAINTENANCE]) 
		VALUES(@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@attr_name,@attr_value,'',0)
	END
	
	DELETE FROM @newTemplates where @emailtemplate_id = emailtemplate_id  
END


-- Get the ID of the existing order confirmation tempates - we need to add some options ticket exchange text placeholders.
DELETE FROM @newTemplates
INSERT INTO @newTemplates 
SELECT DISTINCT emailtemplate_id, TEMPLATE_TYPE FROM [TBL_EMAIL_TEMPLATES] WHERE TEMPLATE_TYPE = 'TicketingConfirmation'

-- Loop through each template and add the required control text and attributes for the template id.
While EXISTS(SELECT * From @newTemplates)
BEGIN 
	Select Top 1 @emailtemplate_id = emailtemplate_id From @newTemplates
	set @control_code_new = ltrim(rtrim(@generic_control_code)) + '.' + convert(nvarchar,@emailtemplate_id)

	DELETE FROM @newContent
	BEGIN
		INSERT INTO @newContent 
		VALUES	( 'TicketExchangeText', '' ) 
	END
	
	While EXISTS(SELECT * From @newContent)
	BEGIN 
		Select Top 1 @text_code = code From @newContent
		Select Top 1 @control_content = content From @newContent
			IF NOT EXISTS(SELECT * From tbl_control_text_lang where control_code = @control_code_new and text_code = @text_code)
			BEGIN
				INSERT INTO [tbl_control_text_lang] ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE]) 
				VALUES(@lang,@businessunitALL,@partnerALL,@pagecodeALL,@control_code_new,@text_code,@control_content,0)
			END
		DELETE FROM @newContent where @text_code = code
	END
	
	DELETE FROM @newTemplates where @emailtemplate_id = emailtemplate_id  
END