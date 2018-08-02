if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_EmailTemplates_SelectAllByBUPartner]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
	drop procedure [dbo].[usp_EmailTemplates_SelectAllByBUPartner]
end
go


create procedure dbo.[usp_EmailTemplates_SelectAllByBUPartner]
(

	@PA_EMAILTEMPLATE_ID int = null,
	@PA_ACTIVE bit = null,
	@PA_BUSINESS_UNIT nvarchar(50) = null,
	@PA_PARTNER nvarchar(50) = null,
	@PA_MASTER bit = null
)
as

begin

--	set nocount on;

    
	-- Retrieve usp_EmailTemplates_SelectAllByBUPartner records
	if @PA_EMAILTEMPLATE_ID is not null
	begin           
			select			 BUSINESS_UNIT,PARTNER,ACTIVE,NAME,DESCRIPTION,TEMPLATE_TYPE,EMAIL_HTML, EMAIL_FROM_ADDRESS, 
EMAIL_SUBJECT,EMAIL_BODY,ADDED_DATETIME,UPDATED_DATETIME,MASTER
			from			tbl_email_templates 
			where			EMAILTEMPLATE_ID = @PA_EMAILTEMPLATE_ID
	end

	if @PA_EMAILTEMPLATE_ID is null
	begin           
			select			EMAILTEMPLATE_ID, NAME, DESCRIPTION,TEMPLATE_TYPE, ACTIVE, EMAIL_HTML,  EMAIL_FROM_ADDRESS, EMAIL_SUBJECT, EMAIL_BODY, MASTER
			from			tbl_email_templates 
			where			((BUSINESS_UNIT = @PA_BUSINESS_UNIT and len(isnull(@PA_BUSINESS_UNIT,''))>0) or (len(isnull(@PA_BUSINESS_UNIT,''))=0))
--			and			    ((PARTNER = @PA_PARTNER and len(isnull(@PA_PARTNER,''))>0) or (len(isnull(@PA_PARTNER,''))=0))
			order by 		NAME
	end
	
	
--	set nocount off;


end


