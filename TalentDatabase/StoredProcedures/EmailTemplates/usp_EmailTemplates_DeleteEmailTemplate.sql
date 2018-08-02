if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_EmailTemplates_delEmailTemplate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
drop procedure [dbo].[usp_EmailTemplates_delEmailTemplate]
end
go


CREATE PROCEDURE [dbo].[usp_EmailTemplates_delEmailTemplate]
(
	@PA_EMAILTEMPLATE_ID as bigint
)
AS
BEGIN
	
	DELETE tbl_email_templates 
	WHERE [EMAILTEMPLATE_ID] = @PA_EMAILTEMPLATE_ID

END
