if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_CopyByBU_Alert_SelAndInsByBU]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[usp_CopyByBU_Alert_SelAndInsByBU]
go

CREATE PROCEDURE [dbo].[usp_CopyByBU_Alert_SelAndInsByBU](
	@pa_BusinessUnit_From as varchar(50),
	@pa_BusinessUnit_To as varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;
	
	--delete tbl_attribute_definition, tbl_alert_definition, tbl_alert_critera starts here
	DELETE FROM tbl_attribute_definition WHERE BUSINESS_UNIT=@pa_BusinessUnit_To
	DELETE FROM tbl_alert_definition WHERE BUSINESS_UNIT=@pa_BusinessUnit_To
	DELETE FROM tbl_alert_critera WHERE ALERT_ID NOT IN (SELECT ID FROM tbl_alert_definition)
	--delete ends here
	
	--select and insert tbl_attribute_definition starts here
	INSERT INTO tbl_attribute_definition (
		BUSINESS_UNIT, PARTNER, CATEGORY, NAME,
		DESCRIPTION, TYPE, FOREIGN_KEY, SOURCE
		)
		SELECT
			@pa_BusinessUnit_To AS BUSINESS_UNIT, PARTNER, CATEGORY, NAME,
			DESCRIPTION, TYPE, FOREIGN_KEY, SOURCE
			FROM tbl_attribute_definition WHERE BUSINESS_UNIT = @pa_BusinessUnit_From
	--select and insert tbl_attribute_definition ends here
	
	--select and insert tbl_alert_definition and tbl_alert_critera starts here
	DECLARE @CurrentAlertID_BU_From INT,
			@AlertFound BIT,
			@InsertedAlertID_BU_To INT
	SET @CurrentAlertID_BU_From = 0	
	SET @AlertFound = 1		
	WHILE (@AlertFound = 1)
		BEGIN
			IF NOT EXISTS(SELECT TOP 1 ID FROM tbl_alert_definition 
				WHERE BUSINESS_UNIT = @pa_BusinessUnit_From
					AND ID > @CurrentAlertID_BU_From ORDER BY ID)
				BEGIN
					SET @AlertFound = 0
				END
			
			IF (@AlertFound = 1)
				BEGIN
					SELECT TOP 1 @CurrentAlertID_BU_From = ID FROM tbl_alert_definition 
						WHERE BUSINESS_UNIT = @pa_BusinessUnit_From
							AND ID > @CurrentAlertID_BU_From ORDER BY ID
				
					-- tbl_alert_definition insert
					INSERT INTO tbl_alert_definition (
						BUSINESS_UNIT, PARTNER, NAME, DESCRIPTION, IMAGE_PATH, 
						ACTION, ACTION_DETAILS, ACTIVATION_START_DATETIME, ACTIVATION_END_DATETIME,
						NON_STANDARD, ENABLED, SUBJECT, DELETED
						)
						SELECT
							@pa_BusinessUnit_To AS BUSINESS_UNIT, PARTNER, NAME, DESCRIPTION, IMAGE_PATH, 
						ACTION, ACTION_DETAILS, ACTIVATION_START_DATETIME, ACTIVATION_END_DATETIME,
						NON_STANDARD, ENABLED, SUBJECT, DELETED
						FROM tbl_alert_definition 
							WHERE BUSINESS_UNIT = @pa_BusinessUnit_From
								AND ID = @CurrentAlertID_BU_From
						SET @InsertedAlertID_BU_To = SCOPE_IDENTITY()
				
					-- tbl_alert_critera insert
					INSERT INTO tbl_alert_critera (
						ALERT_ID, ATTR_ID, ALERT_OPERATOR, SEQUENCE, CLAUSE, CLAUSE_TYPE
						)
						SELECT 
							@InsertedAlertID_BU_To AS ALERT_ID, ATTR_ID, ALERT_OPERATOR, SEQUENCE, CLAUSE, CLAUSE_TYPE
							FROM tbl_alert_critera WHERE ALERT_ID = @CurrentAlertID_BU_From			
				END
		END
		--select and insert tbl_alert_definition and tbl_alert_critera ends here
END
GO

