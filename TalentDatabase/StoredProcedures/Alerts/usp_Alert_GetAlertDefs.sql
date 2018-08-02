if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[usp_alert_selForGenerateUserAlerts]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
	drop procedure [dbo].[usp_alert_selForGenerateUserAlerts]
end
go

create procedure dbo.[usp_alert_selForGenerateUserAlerts]
(
	@PA_BUSINESS_UNIT nvarchar(50) = null,
	@PA_PARTNER nvarchar(50) = null,
	@PA_LOGINID nvarchar(50) = null,
	@PA_ALERTID as int = 0,
	@PA_MODE int = 0
)
as

begin

--	set nocount on;
	     
	-- Retrieve tbl_alert_definition records
	if @PA_MODE = 1 and @PA_BUSINESS_UNIT is not null and @PA_PARTNER is not null
	begin           
			-- All standard alert defs that are enabled, within activation period, and have not been used before for the login
			select			AD.*
			from			tbl_alert_definition as AD 
			where			AD.ENABLED = 'True' 
			and				AD.ACTIVATION_START_DATETIME < getdate() 
			and				AD.ACTIVATION_END_DATETIME > getdate() 
			and				AD.[BUSINESS_UNIT] = @PA_BUSINESS_UNIT 
			and				AD.[PARTNER] = @PA_PARTNER
			and				AD.NON_STANDARD=0
			and				AD.ID not in (
								select ALERT_ID 
								from tbl_alert as AL1 
								inner join tbl_alert_definition as AD2 on AD2.ID = AL1.ALERT_ID
								where AL1.LOGIN_ID = @PA_LOGINID and AD2.NON_STANDARD=0
							)
		union
			-- All non-standard alert defs that are enabled and within activation period
			select			AD.*
			from			tbl_alert_definition as AD 
			where			AD.ENABLED = 'True' 
			and				AD.ACTIVATION_START_DATETIME < getdate() 
			and				AD.ACTIVATION_END_DATETIME > getdate() 
			and				AD.[BUSINESS_UNIT] = @PA_BUSINESS_UNIT 
			and				AD.[PARTNER] = @PA_PARTNER
			and				AD.NON_STANDARD=1
		
	end

	
	-- Retrieve tbl_alert_critera records
	if @PA_MODE = 2 and @PA_BUSINESS_UNIT is not null and @PA_PARTNER is not null
	begin
		-- All criteria
		if @PA_ALERTID = 0
		begin
			select			AC.*
			from			tbl_alert_critera as AC
			inner join		tbl_alert_definition as AD on AD.ID = AC.ALERT_ID
			where			AD.ENABLED = 'True' 
			and				AD.ACTIVATION_START_DATETIME < getdate() 
			and				AD.ACTIVATION_END_DATETIME > getdate() 
			and				AD.[BUSINESS_UNIT] = @PA_BUSINESS_UNIT 
			and				AD.[PARTNER] = @PA_PARTNER
			order by		AC.ALERT_ID, AC.ClAUSE, AC.SEQUENCE
		end
		
		-- Alert-specific criteria
		if @PA_ALERTID <> 0
		begin
			select			AC.*
			from			tbl_alert_critera as AC
			inner join		tbl_alert_definition as AD on AD.ID = AC.ALERT_ID
			where			AD.ENABLED = 'True'
			--and				AD.ACTIVATION_START_DATETIME < getdate() 
			--and				AD.ACTIVATION_END_DATETIME > getdate() 
			--and				AD.[BUSINESS_UNIT] = @PA_BUSINESS_UNIT 
			--and				AD.[PARTNER] = @PA_PARTNER
			and				AC.ALERT_ID = @PA_ALERTID 
			order by		AC.ALERT_ID, AC.ClAUSE, AC.SEQUENCE
		end
		
	end

	
	-- Retrieve tbl_user_attribute records
	if @PA_MODE = 3 and @PA_LOGINID is not null
	begin
		select			UA.*
		from			tbl_user_attribute as UA
		where			UA.LOGINID_ID = @PA_LOGINID
	end
	
--	set nocount off;


end

go
