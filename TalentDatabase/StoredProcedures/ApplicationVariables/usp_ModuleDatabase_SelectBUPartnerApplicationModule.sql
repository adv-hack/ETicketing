if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_ModuleDatabase_SelectBUPartnerApplicationModule]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
drop procedure [dbo].[usp_ModuleDatabase_SelectBUPartnerApplicationModule]
end
go

-- =============================================
-- Author:          Des Webster
-- Create date: 15/12/2014
-- Description:     Retrieve Module Database Record
-- =============================================
CREATE PROCEDURE [dbo].[usp_ModuleDatabase_SelectBUPartnerApplicationModule]
(
	@pa_BusinessUnit VARCHAR(50) = '*ALL',
    @pa_Partner VARCHAR(50) = '*ALL',
    @pa_Application VARCHAR(50) = '*ALL',
    @pa_Module VARCHAR(50) = ''

)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @LoopCount AS INT = 0
    DECLARE @DestinationDatabase AS VARCHAR(50);

	-- Set the busines unit, partner and application to *ALL when blank
	IF LTRIM(@pa_BusinessUnit) = '' SET @pa_BusinessUnit = '*ALL' 
	IF LTRIM(@pa_Partner) = '' SET @pa_Partner = '*ALL'
	IF LTRIM(@pa_Application) = '' SET @pa_Application = '*ALL'

    WHILE @LoopCount < 4
    BEGIN
            SELECT @DestinationDatabase = [DESTINATION_DATABASE] FROM tbl_bu_module_database
            WHERE [BUSINESS_UNIT] = @pa_BusinessUnit
            AND [PARTNER] = @pa_Partner
            AND [APPLICATION] = @pa_Application
            AND [MODULE] = @pa_Module

            SET @LoopCount += 1
            IF @DestinationDatabase IS NOT NULL
                SET @LoopCount = 4
            ELSE
                BEGIN
                        IF @LoopCount = 1 SET @pa_Application = '*ALL'
                        ELSE IF @LoopCount = 2 SET @pa_Partner = '*ALL'
                        ELSE IF @LoopCount = 3 SET @pa_BusinessUnit = '*ALL'
                END
    END

	SELECT @DestinationDatabase AS 'DestinationDatabase'

END