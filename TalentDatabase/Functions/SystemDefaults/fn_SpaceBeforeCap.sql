	IF (OBJECT_ID('fn_SpaceBeforeCap') IS NOT NULL)
	BEGIN 
		DROP FUNCTION dbo.fn_SpaceBeforeCap
	END
	GO 
	CREATE FUNCTION dbo.fn_SpaceBeforeCap
	(
		@str nvarchar(max)
	)
	RETURNS nvarchar(max)
	AS
	BEGIN

	/*************************[DEBUG START]*************************/
		--declare @str nvarchar(max)
		--set @str = '001ChiragBhatt'
	/*************************[DEBUG END]**************************/

		declare @i int, @j int,@returnval nvarchar(max),@w nvarchar(max)
	
		-- init 
		set @returnval = ''
		select @i = 1, @j = len(@str)
		set @w = '' 

		while @i <= @j
		begin
				if substring(@str,@i,1) = UPPER(substring(@str,@i,1)) collate Latin1_General_CS_AS and isnumeric(substring(@str,@i,1)) <> 1
				begin
					if @w is not null
						set @returnval = @returnval + ' ' + @w
					set @w = substring(@str,@i,1)
				end
				else
				begin 
					set @w = @w + substring(@str,@i,1)
				end
			set @i = @i + 1
		end
		if @w is not null
			set @returnval = @returnval + ' ' + @w

	
		/* debug */ 
		--select ltrim(@returnval)

		return ltrim(@returnval)
	end
	GO 