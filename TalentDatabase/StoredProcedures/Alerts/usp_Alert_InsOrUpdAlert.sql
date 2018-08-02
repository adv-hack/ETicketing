if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_Alert_InsOrUpdAlert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
drop procedure [dbo].[usp_Alert_InsOrUpdAlert]
end
go

create procedure dbo.usp_Alert_InsOrUpdAlert
(
	@PA_ID bigint							= null,
	@PA_BUSINESS_UNIT nvarchar(50)			= null,
	@PA_PARTNER nvarchar(50)				= null,
	@PA_LOGIN_ID nvarchar(100)				= null,
	@PA_ALERT_ID bigint						= null,
	@PA_DESCRIPTION nvarchar(200)			= null,
	@PA_ACTION_DETAILS nvarchar(200)		= null,
	@PA_SUBJECT nvarchar(200)				= null,
	@PA_ACTIVATION_START_DATETIME datetime	= null,
	@PA_ACTIVATION_END_DATETIME datetime	= null,
	@PA_READ bit							= null
)
as

declare	@ERRORCODE int, @ROWCOUNT int
declare @dup_count int

set nocount on

if @PA_ID is not null and datalength(@PA_ID) > 0
begin

	update tbl_alert set
		[BUSINESS_UNIT]				= COALESCE(@PA_BUSINESS_UNIT,[BUSINESS_UNIT]),
		[PARTNER]					= COALESCE(@PA_PARTNER,[PARTNER]),
		[LOGIN_ID]					= COALESCE(@PA_LOGIN_ID,[LOGIN_ID]),
		[ALERT_ID]					= COALESCE(@PA_ALERT_ID,[ALERT_ID]),
		[DESCRIPTION]				= COALESCE(@PA_DESCRIPTION,[DESCRIPTION]),
		[ACTION_DETAILS]			= COALESCE(@PA_ACTION_DETAILS,[ACTION_DETAILS]),
		[SUBJECT]					= COALESCE(@PA_SUBJECT,[SUBJECT]),
		[ACTIVATION_START_DATETIME] = COALESCE(@PA_ACTIVATION_START_DATETIME,[ACTIVATION_START_DATETIME]),
		[ACTIVATION_END_DATETIME]	= COALESCE(@PA_ACTIVATION_END_DATETIME,[ACTIVATION_END_DATETIME]),
		[READ]						= COALESCE(@PA_READ,[READ])
	where ID = @PA_ID

	select @ERRORCODE = @@ERROR,@ROWCOUNT = @@ROWCOUNT
	if @ERRORCODE != 0 goto HANDLE_ERROR

end
else
begin
	
	-- Donot allow duplicate alerts to be generated 
	set @dup_count = (select COUNT(*) from tbl_alert 
						where [BUSINESS_UNIT]=@PA_BUSINESS_UNIT and [PARTNER]=@PA_PARTNER
							and [LOGIN_ID]=@PA_LOGIN_ID and [ALERT_ID]=@PA_ALERT_ID
							and [DESCRIPTION]=@PA_DESCRIPTION and [SUBJECT]=@PA_SUBJECT
							and [ACTIVATION_START_DATETIME]=@PA_ACTIVATION_START_DATETIME
							and [ACTIVATION_END_DATETIME]=@PA_ACTIVATION_END_DATETIME)
							
	--print convert(varchar,@dup_count)
	if @dup_count=0
	begin
		Insert into tbl_alert
		(
			[BUSINESS_UNIT],
			[PARTNER],
			[LOGIN_ID],
			[ALERT_ID],
			[DESCRIPTION],
			[ACTION_DETAILS],
			[ACTIVATION_START_DATETIME],
			[ACTIVATION_END_DATETIME],
			[READ],
			[SUBJECT]

		)
		values
		(
			@PA_BUSINESS_UNIT,
			@PA_PARTNER,
			@PA_LOGIN_ID,
			@PA_ALERT_ID,
			@PA_DESCRIPTION,
			@PA_ACTION_DETAILS,
			@PA_ACTIVATION_START_DATETIME,
			@PA_ACTIVATION_END_DATETIME,
			@PA_READ,
			@PA_SUBJECT
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

GO

