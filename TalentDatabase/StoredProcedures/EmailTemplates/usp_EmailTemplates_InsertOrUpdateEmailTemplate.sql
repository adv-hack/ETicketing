if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_EmailTemplates_InsOrUpdEmailTemplate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
drop procedure [dbo].[usp_EmailTemplates_InsOrUpdEmailTemplate]
end
go


create procedure dbo.usp_EmailTemplates_InsOrUpdEmailTemplate
(
	@PA_ID bigint				= null,
	@PA_BUSINESS_UNIT nvarchar(50)		= null,
	@PA_PARTNER nvarchar(50)		= null,
	@PA_ACTIVE bit				= null,
	@PA_NAME nvarchar(100)			= null,
	@PA_DESCRIPTION nvarchar(200)		= null,
	@PA_TEMPLATE_TYPE nvarchar(50)		= null,
	@PA_EMAIL_HTML bit                   = null,
    @PA_EMAIL_FROM_ADDRESS nvarchar(200) = null,
	@PA_EMAIL_SUBJECT nvarchar(200)		= null,
	@PA_EMAIL_BODY nvarchar(max)		= null,
	@PA_MASTER bit				= null
)
as

declare	@ERRORCODE int, @ROWCOUNT int
declare @dup_count int
declare @id_exists int

set nocount on

if @PA_ID is not null and datalength(@PA_ID) > 0
begin

	-- Check the passed ID exists and if not set @PA_ID = 0 to indicate an update failure
	set @id_exists = (select COUNT(*) from tbl_email_templates where [EMAILTEMPLATE_ID]=@PA_ID)
	if @id_exists <> 1
	begin
		set @PA_ID = 0
	end
	else
	begin
	update tbl_email_templates set
		[BUSINESS_UNIT]			= COALESCE(@PA_BUSINESS_UNIT,[BUSINESS_UNIT]),
		[PARTNER]				= COALESCE(@PA_PARTNER,[PARTNER]),
		[ACTIVE]				= COALESCE(@PA_ACTIVE,[ACTIVE]),
		[NAME]					= COALESCE(@PA_NAME,[NAME]),
		[DESCRIPTION]			= COALESCE(@PA_DESCRIPTION,[DESCRIPTION]),
		[EMAIL_HTML]			= COALESCE(@PA_EMAIL_HTML,[EMAIL_HTML]),
        [EMAIL_FROM_ADDRESS]	= COALESCE(@PA_EMAIL_FROM_ADDRESS,[EMAIL_FROM_ADDRESS]),
		[EMAIL_SUBJECT]			= COALESCE(@PA_EMAIL_SUBJECT,[EMAIL_SUBJECT]),
		[EMAIL_BODY]			= COALESCE(@PA_EMAIL_BODY,[EMAIL_BODY]),
		[UPDATED_DATETIME] 		= GetDate(),
		[MASTER]				= COALESCE(@PA_MASTER,[MASTER])
	where  EMAILTEMPLATE_ID = @PA_ID

	select @ERRORCODE = @@ERROR,@ROWCOUNT = @@ROWCOUNT
	if @ERRORCODE != 0 goto HANDLE_ERROR
	end
end
else
begin
	
	-- Donot allow duplicate alerts to be generated 
	set @dup_count = (select COUNT(*) from tbl_email_templates where [BUSINESS_UNIT]=@PA_BUSINESS_UNIT and [PARTNER]=@PA_PARTNER and [NAME]=@PA_NAME
	 and [TEMPLATE_TYPE]=@PA_TEMPLATE_TYPE and [DESCRIPTION]=@PA_DESCRIPTION)
	
							
	--print convert(varchar,@dup_count)
	if @dup_count=0
	begin
		Insert into tbl_email_templates
		(
			[BUSINESS_UNIT],
			[PARTNER],
			[ACTIVE],
			[NAME],
			[DESCRIPTION],
			[TEMPLATE_TYPE],
			[EMAIL_HTML],
			[EMAIL_FROM_ADDRESS],
			[EMAIL_SUBJECT],
			[EMAIL_BODY],
			[ADDED_DATETIME],
			[UPDATED_DATETIME],
			[MASTER]
					)
		values
		(
			@PA_BUSINESS_UNIT,
			@PA_PARTNER,
			@PA_ACTIVE,
			@PA_NAME,
			@PA_DESCRIPTION,
			@PA_TEMPLATE_TYPE,
			@PA_EMAIL_HTML, 
			@PA_EMAIL_FROM_ADDRESS,
			@PA_EMAIL_SUBJECT,
			@PA_EMAIL_BODY,
			GetDate(),
			GetDate(),
			@PA_MASTER
		)

		select @ERRORCODE = @@ERROR,@ROWCOUNT = @@ROWCOUNT
		if @ERRORCODE != 0 goto HANDLE_ERROR
	
		SELECT @PA_ID = SCOPE_IDENTITY()	
	end
end

set nocount off

select @PA_ID

HANDLE_ERROR:
	select 0



