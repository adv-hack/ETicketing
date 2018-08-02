if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_Alert_DelAndInsAttributeDefinition]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
drop procedure [dbo].[usp_Alert_DelAndInsAttributeDefinition]
end
go

CREATE PROCEDURE [dbo].[usp_Alert_DelAndInsAttributeDefinition]
(
	@BUSINESS_UNIT as nvarchar(50),
	@PARTNER as nvarchar(50),
	@SOURCE as nvarchar(100),
	@ATT_DEF_XML_STRING as nvarchar(max)
)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @XMLDOCID as INTEGER
	DECLARE @XPATHSTRING as VARCHAR(500)
	SET @XPATHSTRING = '/ArrayOfDEAttributeDefinition/DEAttributeDefinition'
	
	DELETE TBL_ATTRIBUTE_DEFINITION 
	WHERE [BUSINESS_UNIT] = @BUSINESS_UNIT
	AND [PARTNER] = @PARTNER
	AND [SOURCE] = @SOURCE
	
	-- as we have to return number records inserted
	SET NOCOUNT OFF;
	
	--prepare xml and insert tbl_attribute_definition
	EXEC sp_xml_preparedocument @XMLDOCID OUTPUT, @ATT_DEF_XML_STRING
	INSERT INTO TBL_ATTRIBUTE_DEFINITION ([BUSINESS_UNIT],[PARTNER],[CATEGORY],[NAME],[DESCRIPTION],[TYPE],[FOREIGN_KEY],[SOURCE])
		SELECT @BUSINESS_UNIT, @PARTNER, CATEGORY, NAME, [DESCRIPTION], [TYPE], [FOREIGN_KEY], @SOURCE
        FROM OPENXML(@XMLDOCID, @XPATHSTRING, 2) 
        WITH ([CATEGORY] nvarchar(100), [NAME] nvarchar(100), [DESCRIPTION] nvarchar(100), TYPE nvarchar(100), [FOREIGN_KEY] nvarchar(100))
    EXEC sp_xml_removedocument @XMLDOCID
END