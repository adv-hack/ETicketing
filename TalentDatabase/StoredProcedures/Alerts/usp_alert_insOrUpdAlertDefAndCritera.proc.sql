if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_Alert_InsOrUpdAlertDefAndCritera]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
drop procedure [dbo].[usp_Alert_InsOrUpdAlertDefAndCritera]
end
go

CREATE PROCEDURE [dbo].[usp_Alert_InsOrUpdAlertDefAndCritera]
(
	@ALERT_ID as Integer,
	@BUSINESS_UNIT as nvarchar(50),
	@PARTNER as nvarchar(50),
	@NAME as nvarchar(50),
	@DESCRIPTION as nvarchar(200),
	@SUBJECT as nvarchar(200),
	@IMAGE_PATH as nvarchar(200),
	@ACTION as nvarchar(50),
	@ACTION_DETAILS as nvarchar(200),
	@ACTION_DETAILS_URL_OPTION as bit,
	@ACTIVATION_START_DATETIME as Datetime,
	@ACTIVATION_END_DATETIME as Datetime,
	@ENABLED as bit,
	@NON_STANDARD as bit,
	@CRITERA_XML_STRING as nvarchar(max),
	@CRITERIA_CHANGED as bit = null,
	@SUBJDESC_CHANGED as bit = null,
	@ACTDATES_CHANGED as bit = null,
	@FAFEMAIL_CHANGED as bit = null,
	@ACTION_TYPE as bit,
	@PAGE_CODE as nvarchar(50)
)
AS
BEGIN
	DECLARE @ALERT_ID_FOR_CRITERA as BIGINT
	SET NOCOUNT ON;
	
	IF (@ALERT_ID=0)
		BEGIN
			IF NOT EXISTS(SELECT ID FROM TBL_ALERT_DEFINITION WHERE UPPER(NAME) = UPPER(@NAME) AND BUSINESS_UNIT = @BUSINESS_UNIT AND [PARTNER] = @PARTNER AND [DELETED] = 0)
				BEGIN
					INSERT INTO TBL_ALERT_DEFINITION (
						BUSINESS_UNIT,
						[PARTNER],
						NAME,
						[DESCRIPTION],
						[SUBJECT],
						IMAGE_PATH,
						[ACTION],
						ACTION_DETAILS,
						ACTION_DETAILS_URL_OPTION,
						ACTIVATION_START_DATETIME,
						ACTIVATION_END_DATETIME,
						[ENABLED],
						NON_STANDARD,
						DELETED,
						ACTION_TYPE,
						PAGE_CODE) 
					VALUES (
						isnull(@BUSINESS_UNIT,''), 
						isnull(@PARTNER,''), 
						isnull(@NAME,''), 
						isnull(@DESCRIPTION,''), 
						isnull(@SUBJECT,''), 
						isnull(@IMAGE_PATH,''), 
						isnull(@ACTION,''), 
						isnull(@ACTION_DETAILS,''),  
						ISNULL(@ACTION_DETAILS_URL_OPTION, 0),
						@ACTIVATION_START_DATETIME, 
						@ACTIVATION_END_DATETIME, 
						@ENABLED,
						@NON_STANDARD,
						0,
						isnull(@ACTION_TYPE,NULL),
						isnull (@PAGE_CODE, NULL))
					SELECT @ALERT_ID_FOR_CRITERA = SCOPE_IDENTITY()
				END
			ELSE
				BEGIN
					SET @ALERT_ID_FOR_CRITERA = -2
				END
		END
	ELSE
		BEGIN
			IF EXISTS(SELECT ID FROM TBL_ALERT_DEFINITION WHERE ID = @ALERT_ID)
			
				BEGIN
					IF NOT EXISTS(SELECT ID FROM TBL_ALERT_DEFINITION WHERE UPPER(NAME) = UPPER(@NAME) AND BUSINESS_UNIT = @BUSINESS_UNIT AND [PARTNER] = @PARTNER AND ID <> @ALERT_ID)
						BEGIN
						
							UPDATE TBL_ALERT_DEFINITION 
							SET BUSINESS_UNIT = COALESCE(@BUSINESS_UNIT,BUSINESS_UNIT), 
								[PARTNER] = COALESCE(@PARTNER,[PARTNER]),
								NAME = COALESCE(@NAME,NAME), 
								[DESCRIPTION] = COALESCE(@DESCRIPTION,[DESCRIPTION]), 
								[SUBJECT] = COALESCE(@SUBJECT,[SUBJECT]),
								IMAGE_PATH = COALESCE(@IMAGE_PATH,IMAGE_PATH),
								[ACTION] = COALESCE(@ACTION,[ACTION]),
								ACTION_DETAILS = COALESCE(@ACTION_DETAILS,ACTION_DETAILS),
								ACTION_DETAILS_URL_OPTION = COALESCE(@ACTION_DETAILS_URL_OPTION,ACTION_DETAILS_URL_OPTION),
								ACTIVATION_START_DATETIME = @ACTIVATION_START_DATETIME, 
								ACTIVATION_END_DATETIME = @ACTIVATION_END_DATETIME,
								[ENABLED] = @ENABLED, 
								NON_STANDARD = @NON_STANDARD,
								[ACTION_TYPE] = @ACTION_TYPE,
								[PAGE_CODE] = @PAGE_CODE
								WHERE ID = @ALERT_ID
							SET @ALERT_ID_FOR_CRITERA = @ALERT_ID
						END
					ELSE
						BEGIN
							SET @ALERT_ID_FOR_CRITERA = -2
						END
				END
			ELSE
				BEGIN
					SET @ALERT_ID_FOR_CRITERA = -1				
				END
		END
		IF (@ALERT_ID_FOR_CRITERA > 0)
		BEGIN
			DELETE TBL_ALERT_CRITERA WHERE ALERT_ID = @ALERT_ID_FOR_CRITERA
			DECLARE @XMLDOCID as INTEGER
			DECLARE @XPATHSTRING as VARCHAR(500)
			SET @XPATHSTRING = '/ArrayOfDEAlertCritera/DEAlertCritera'
			EXEC sp_xml_preparedocument @XMLDOCID OUTPUT, @CRITERA_XML_STRING
			INSERT INTO TBL_ALERT_CRITERA (ALERT_ID, ATTR_ID, ALERT_OPERATOR, SEQUENCE, CLAUSE, CLAUSE_TYPE)
				SELECT @ALERT_ID_FOR_CRITERA, ATTR_ID, ALERT_OPERATOR, SEQUENCE, CLAUSE, CLAUSE_TYPE
				FROM OPENXML(@XMLDOCID, @XPATHSTRING, 2) 
				WITH (ATTR_ID nvarchar(50), ALERT_OPERATOR nvarchar(50), SEQUENCE bigint, CLAUSE bigint, CLAUSE_TYPE nvarchar(50))
			EXEC sp_xml_removedocument @XMLDOCID
		END


--		If criteria are changed then 
--			•	Standard alerts - all read and unread alerts for this alert_id are deleted.
--			•	Non-standard alerts – n/a
		IF (@CRITERIA_CHANGED = 1)
		BEGIN
			IF (@NON_STANDARD = 0)
			BEGIN
				DELETE TBL_ALERT WHERE ALERT_ID = @ALERT_ID_FOR_CRITERA
			END
		END

--		If subject or description is changed then:
--			•	Standard alerts - all unread for this alert_id are deleted 
--			•	Non-standard alerts - all read and unread alerts for this id within current activation dates are deleted
		IF (@SUBJDESC_CHANGED = 1)
		BEGIN
			IF (@NON_STANDARD = 0)
			BEGIN
				DELETE TBL_ALERT WHERE ALERT_ID = @ALERT_ID_FOR_CRITERA AND [READ] <> 1
			END
			IF (@NON_STANDARD = 1)
			BEGIN
				DELETE TBL_ALERT WHERE ALERT_ID = @ALERT_ID_FOR_CRITERA AND ([ACTIVATION_START_DATETIME] <= GETDATE() AND [ACTIVATION_END_DATETIME] >= GETDATE())
			END
		END

--		If activation dates are changed then:
--			•	Standard alerts – all unread for this alert_id are deleted
--			•	Non-standard alerts – n/a
		IF (@ACTDATES_CHANGED = 1)
		BEGIN
			IF (@NON_STANDARD = 0)
			BEGIN
				DELETE TBL_ALERT WHERE ALERT_ID = @ALERT_ID_FOR_CRITERA AND [READ] <> 1
			END
		END

--		If F&FBirthdayAlert has changed SendEmail email address (action detail) then:
--			•	Standard alerts – n/a
--			•	Non-standard alerts – all unread for this alert_id are deleted
		IF (@FAFEMAIL_CHANGED = 1)
		BEGIN
			BEGIN
				DELETE TBL_ALERT WHERE ALERT_ID = @ALERT_ID_FOR_CRITERA AND [READ] <> 1
			END
		END

	SET NOCOUNT OFF;
	SELECT @ALERT_ID_FOR_CRITERA
END
