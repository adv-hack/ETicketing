if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_CopyByBU_EmailTemplates_SelAndInsByBU]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[usp_CopyByBU_EmailTemplates_SelAndInsByBU]
go

CREATE PROCEDURE [dbo].[usp_CopyByBU_EmailTemplates_SelAndInsByBU](
	@pa_BusinessUnit_From as varchar(50),
	@pa_BusinessUnit_To as varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ControlCodePrefix AS VARCHAR(100)
	SET @ControlCodePrefix = 'TalentEmail.vb.'
	--delete tbl_email_templates, tbl_control_text_lang, tbl_control_attribute starts here
	DECLARE @DeletedTemplateID Table (EMAILTEMPLATE_ID BIGINT);
	DELETE FROM tbl_email_templates 
		OUTPUT DELETED.EMAILTEMPLATE_ID INTO @DeletedTemplateID (EMAILTEMPLATE_ID)
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_To
	DELETE FROM tbl_control_text_lang 
		WHERE BUSINESS_UNIT IN (@pa_BusinessUnit_To, '*ALL')
			AND CONTROL_CODE IN (SELECT @ControlCodePrefix + CAST(EMAILTEMPLATE_ID AS VARCHAR(1000)) FROM @DeletedTemplateID)
	DELETE FROM tbl_control_attribute
		WHERE BUSINESS_UNIT IN (@pa_BusinessUnit_To, '*ALL')
			AND CONTROL_CODE IN (SELECT @ControlCodePrefix + CAST(EMAILTEMPLATE_ID AS VARCHAR(1000)) FROM @DeletedTemplateID)			
	--delete ends here
	
	--select and insert tbl_email_templates, tbl_control_text_lang, tbl_control_attribute starts here
	DECLARE @CurrentEmailTemplateID_BU_From INT,
			@EmailTemplateFound BIT,
			@InsertedEmailTemplateID_BU_To INT
	SET @CurrentEmailTemplateID_BU_From = 0	
	SET @EmailTemplateFound = 1		
	WHILE (@EmailTemplateFound = 1)
		BEGIN
			IF NOT EXISTS(SELECT TOP 1 EMAILTEMPLATE_ID FROM tbl_email_templates 
				WHERE BUSINESS_UNIT = @pa_BusinessUnit_From
					AND EMAILTEMPLATE_ID > @CurrentEmailTemplateID_BU_From ORDER BY EMAILTEMPLATE_ID)
				BEGIN
					SET @EmailTemplateFound = 0
				END
			
			IF (@EmailTemplateFound = 1)
				BEGIN
					SELECT TOP 1 @CurrentEmailTemplateID_BU_From = EMAILTEMPLATE_ID FROM tbl_email_templates 
						WHERE BUSINESS_UNIT = @pa_BusinessUnit_From
							AND EMAILTEMPLATE_ID > @CurrentEmailTemplateID_BU_From ORDER BY EMAILTEMPLATE_ID
				
					-- tbl_email_templates insert
					INSERT INTO tbl_email_templates (
						BUSINESS_UNIT, PARTNER, ACTIVE, NAME, DESCRIPTION, 
						TEMPLATE_TYPE, EMAIL_HTML, EMAIL_FROM_ADDRESS, EMAIL_SUBJECT,
						EMAIL_BODY, ADDED_DATETIME, UPDATED_DATETIME
						)
						SELECT @pa_BusinessUnit_To AS BUSINESS_UNIT, PARTNER, ACTIVE, NAME, DESCRIPTION, 
						TEMPLATE_TYPE, EMAIL_HTML, EMAIL_FROM_ADDRESS, EMAIL_SUBJECT,
						EMAIL_BODY, ADDED_DATETIME, UPDATED_DATETIME
						FROM tbl_email_templates WHERE BUSINESS_UNIT = @pa_BusinessUnit_From
								AND EMAILTEMPLATE_ID = @CurrentEmailTemplateID_BU_From
						SET @InsertedEmailTemplateID_BU_To = SCOPE_IDENTITY()

					-- tbl_control_text_lang insert
					INSERT INTO tbl_control_text_lang (
						BUSINESS_UNIT, PAGE_CODE, PARTNER_CODE,
						LANGUAGE_CODE, CONTROL_CODE, TEXT_CODE, CONTROL_CONTENT
						)
						SELECT CASE BUSINESS_UNIT
								WHEN @pa_BusinessUnit_From THEN @pa_BusinessUnit_To
								WHEN '*ALL' THEN '*ALL'
								ELSE BUSINESS_UNIT
								END AS BUSINESS_UNIT,
							PAGE_CODE, PARTNER_CODE, LANGUAGE_CODE, 
							@ControlCodePrefix + CAST(@InsertedEmailTemplateID_BU_To AS VARCHAR(100)) AS CONTROL_CODE, 
							TEXT_CODE, CONTROL_CONTENT
							FROM tbl_control_text_lang
							WHERE CONTROL_CODE = @ControlCodePrefix + CAST(@CurrentEmailTemplateID_BU_From AS VARCHAR(100))
							
					-- tbl_control_attribute
					INSERT INTO tbl_control_attribute (
						BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, CONTROL_CODE,
						ATTR_NAME, ATTR_VALUE, DESCRIPTION
						)
						SELECT CASE BUSINESS_UNIT
								WHEN @pa_BusinessUnit_From THEN @pa_BusinessUnit_To
								WHEN '*ALL' THEN '*ALL'
								ELSE BUSINESS_UNIT
								END AS BUSINESS_UNIT,
							PARTNER_CODE, PAGE_CODE,
							@ControlCodePrefix + CAST(@InsertedEmailTemplateID_BU_To AS VARCHAR(100)) AS CONTROL_CODE,
							ATTR_NAME, ATTR_VALUE, DESCRIPTION
							FROM tbl_control_attribute
							WHERE CONTROL_CODE = @ControlCodePrefix + CAST(@CurrentEmailTemplateID_BU_From AS VARCHAR(100))
				END
		END
		--select and insert tbl_email_templates, tbl_control_text_lang, tbl_control_attribute ends here
END
GO

