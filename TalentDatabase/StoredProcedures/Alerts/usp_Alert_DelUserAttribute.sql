if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_Alert_DelUserAttribute]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
drop procedure [dbo].[usp_Alert_DelUserAttribute]
end
go

create procedure dbo.usp_Alert_DelUserAttribute
(
	@PA_LOGINID_ID bigint					= null,
	@PA_ATTR_ID nvarchar(50)				= null
)
as

declare	@ERRORCODE int, @ROWCOUNT int

set nocount on

if (@PA_LOGINID_ID is not null and datalength(@PA_LOGINID_ID) > 0) and (@PA_ATTR_ID is not null and datalength(@PA_ATTR_ID) > 0)
begin
	delete tbl_user_attribute
	where LOGINID_ID = @PA_LOGINID_ID and ATTR_ID = @PA_ATTR_ID

	select @ERRORCODE = @@ERROR,@ROWCOUNT = @@ROWCOUNT
	if @ERRORCODE != 0 goto HANDLE_ERROR

end

set nocount off
select 1

HANDLE_ERROR:
	select 0

GO

