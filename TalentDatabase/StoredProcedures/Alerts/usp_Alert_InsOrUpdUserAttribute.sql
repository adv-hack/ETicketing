if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_Alert_InsOrUpdUserAttribute]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
drop procedure [dbo].[usp_Alert_InsOrUpdUserAttribute]
end
go

	create procedure dbo.usp_Alert_InsOrUpdUserAttribute
	(
		@PA_ID bigint							= null,
		@PA_LOGINID_ID nvarchar(100)			= null,
		@PA_ATTR_NAME nvarchar(100)				= null,
		@PA_ATTR_ID nvarchar(100)				= null,
		@PA_ATTR_DATA nvarchar(200)				= null
	)
as

declare	@ERRORCODE int, @ROWCOUNT int

set nocount on

if @PA_ID is not null and datalength(@PA_ID) > 0
begin

	update tbl_user_attribute set
		[LOGINID_ID]			= COALESCE(@PA_LOGINID_ID,[LOGINID_ID]),
		[ATTR_NAME]				= COALESCE(@PA_ATTR_NAME,[ATTR_NAME]),
		[ATTR_ID]				= COALESCE(@PA_ATTR_ID,[ATTR_ID]),
		[ATTR_DATA]				= COALESCE(@PA_ATTR_DATA,[ATTR_DATA])
	where ID = @PA_ID

	select @ERRORCODE = @@ERROR,@ROWCOUNT = @@ROWCOUNT
	if @ERRORCODE != 0 goto HANDLE_ERROR

end
else
begin

	Insert into tbl_user_attribute
	(
		[LOGINID_ID],
		[ATTR_NAME],
		[ATTR_ID],
		[ATTR_DATA]
	)
	values
	(
		@PA_LOGINID_ID,
		@PA_ATTR_NAME,
		@PA_ATTR_ID,
		@PA_ATTR_DATA
	)

	select @ERRORCODE = @@ERROR,@ROWCOUNT = @@ROWCOUNT
	if @ERRORCODE != 0 goto HANDLE_ERROR
	
	SELECT @PA_ID = SCOPE_IDENTITY()	
end

set nocount off

select @PA_ID

HANDLE_ERROR:
	select 0
GO

