IF EXISTS (SELECT * FROM dbo.sysobjects WHERE ID = object_id(N'[dbo].[usp_Alert_SelAndInsByID]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
DROP PROCEDURE [dbo].[usp_Alert_SelAndInsByID]
END
GO

CREATE PROCEDURE [dbo].[usp_Alert_SelAndInsByID](
	@pa_BusinessUnit_From as varchar(50),
	@pa_BusinessUnit_To as varchar(50),
	@pa_ID_From as int,
	@pa_Partner_From as varchar(50),
	@pa_PageCode_From as varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;
	
	-- delete it if already exists start here
	DECLARE @Alert_Name varchar(100)
	DECLARE @DeletedAlertID_BU_To INT
	DECLARE @Action nvarchar(20)
	SELECT @Alert_Name = NAME FROM tbl_alert_definition WHERE ID = @pa_ID_From
	IF EXISTS(SELECT ID FROM tbl_alert_definition WHERE BUSINESS_UNIT = @pa_BusinessUnit_To AND NAME = @Alert_Name)
		BEGIN
			DECLARE @Alert_ID_To_Delete INT
				--delete alert definition
				SELECT @DeletedAlertID_BU_To = ID FROM tbl_alert_definition WHERE BUSINESS_UNIT = @pa_BusinessUnit_To AND NAME = @Alert_Name
				DELETE FROM tbl_alert_definition WHERE ID = @DeletedAlertID_BU_To
				-- as well as its criteria
				DELETE FROM tbl_alert_critera WHERE ID = @DeletedAlertID_BU_To
				
		END
	
	-- delete if if already exists ends here
	
	-- tbl_alert_definition insert
	DECLARE @InsertedAlertID_BU_To INT
		INSERT INTO tbl_alert_definition (
					BUSINESS_UNIT, PARTNER, NAME, DESCRIPTION, IMAGE_PATH, 
					ACTION, ACTION_DETAILS, ACTIVATION_START_DATETIME, ACTIVATION_END_DATETIME,
					NON_STANDARD, ENABLED, SUBJECT, DELETED, PAGE_CODE
					)
						SELECT
							@pa_BusinessUnit_To AS BUSINESS_UNIT, PARTNER, NAME, DESCRIPTION, IMAGE_PATH, 
							ACTION, ACTION_DETAILS, ACTIVATION_START_DATETIME, ACTIVATION_END_DATETIME,
							NON_STANDARD, ENABLED, SUBJECT, DELETED, PAGE_CODE
							FROM tbl_alert_definition 
							WHERE ID = @pa_ID_From
				SET @InsertedAlertID_BU_To = SCOPE_IDENTITY()							
		-- if a restriction, update restricting alert name in tbl_page
		IF (SELECT [Action] from tbl_alert_definition where ID = @pa_ID_From)= 'PageRestrict'
			BEGIN
				UPDATE tbl_page 
				SET RESTRICTING_ALERT_NAME=@Alert_Name
				WHERE BUSINESS_UNIT IN (@pa_BusinessUnit_To,'*ALL')
				AND PARTNER_CODE IN (@pa_Partner_From,'*ALL')
				AND PAGE_CODE =@pa_PageCode_From
			END
			
		-- tbl_alert_critera insert
		INSERT INTO tbl_alert_critera (
					ALERT_ID, ATTR_ID, ALERT_OPERATOR, SEQUENCE, CLAUSE, CLAUSE_TYPE
					)
						SELECT 
							@InsertedAlertID_BU_To AS ALERT_ID, ATTR_ID, ALERT_OPERATOR, SEQUENCE, CLAUSE, CLAUSE_TYPE
							FROM tbl_alert_critera WHERE ALERT_ID = @pa_ID_From	
		SET NOCOUNT OFF;
END