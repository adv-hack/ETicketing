-- this will generate script for all db in a sql server and then run the generated script
-- all are commented to avoid mistakenly upgrade run this
--1.       Open SQL Studio
--2.       Open new query
--3.       Choose master db
--4.       Copy and paste the below script
--5.       Run it
--6.       Then copy the result
--7.       Result will be your script to update noise
--8.       Select from below to end and click uncomment to get the script and do the above steps

---- TO MAKE NOISE IN USE TO FALSE

--DECLARE @LINEBREAK AS varchar(2)
--SET @LINEBREAK = CHAR(13) + CHAR(10)

--DECLARE @DBID int, @DBNAME NVARCHAR(1000), @SCRIPT_NOISE_OFFLINE NVARCHAR(max), @NOISE_STATUS_BEFORE_UPDATE VARCHAR(MAX)

--SET @SCRIPT_NOISE_OFFLINE = ''

--SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + 'SELECT ''IF NOISE IN USE IS ALREADY FALSE OTHER THAN BOX OFFICE THEY WILL BE LISTED BELOW. CHANGE MANUALLY THE GENERATED SCRIPT TO MAKE ONLINE FOR THOSE DB'''


--SELECT TOP 1 @DBID = ISNULL(database_id,0), @DBNAME = NAME FROM sys.databases 
----DB WHICH ARE NOT REQUIRED TO TURN ON OR OFF NOISE_IN_USE
--WHERE NAME NOT IN ('master','model','msdb','tempdb','TalentAdminDB','ReportServer') ORDER BY database_id

--WHILE (@DBID > 0)
--	BEGIN
--	        SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + ' -- ************************* ' + @DBNAME + + ' ************************* -- ' 
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + 'USE ' + @DBNAME +
--		+ @LINEBREAK + 'GO'
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + 'IF EXISTS(SELECT * FROM SYS.TABLES A WHERE LOWER(A.NAME) = ''tbl_ecommerce_module_defaults_bu'')'
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + 'BEGIN'
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + '	SELECT ''' + @DBNAME + ''' as DBNAME, BUSINESS_UNIT, DEFAULT_NAME, [VALUE] FROM tbl_ecommerce_module_defaults_bu WHERE DEFAULT_NAME = ''NOISE_IN_USE'' AND LOWER(ISNULL(VALUE,'''')) = ''false'' AND BUSINESS_UNIT <> ''BOXOFFICE'''
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + '	UPDATE tbl_ecommerce_module_defaults_bu SET [VALUE] = ''False'' WHERE DEFAULT_NAME = ''NOISE_IN_USE'''
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + 'END'
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + ' ------------------------------ ends -------------------------- '

--		IF EXISTS(SELECT TOP 1 database_id FROM sys.databases WHERE database_id > @DBID AND NAME NOT IN ('master','model','msdb','tempdb','TalentAdminDB','ReportServer') ORDER BY database_id)
--			BEGIN
--				SELECT TOP 1 @DBID = ISNULL(database_id,0), @DBNAME = NAME FROM sys.databases 
--				WHERE database_id > @DBID
--				--DB WHICH ARE NOT REQUIRED TO TURN ON OR OFF NOISE_IN_USE 
--				AND NAME NOT IN ('master','model','msdb','tempdb','TalentAdminDB','ReportServer') ORDER BY database_id
--			END
--		ELSE
--			BEGIN
--				SET @DBID = 0
--			END
--	END
--	SELECT @SCRIPT_NOISE_OFFLINE


----- TO MAKE NOISE IN USE TO TURE EXCEPT BOXOFFICE BUSINESS UNIT

--DECLARE @LINEBREAK AS varchar(2)
--SET @LINEBREAK = CHAR(13) + CHAR(10)

--DECLARE @DBID int, @DBNAME NVARCHAR(1000), @SCRIPT_NOISE_OFFLINE NVARCHAR(max), @NOISE_STATUS_BEFORE_UPDATE VARCHAR(MAX)

--SET @SCRIPT_NOISE_OFFLINE = ''

--SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + 'SELECT ''IF NOISE IN USE IS FALSE AFTER UPDATE TO TRUE OTHER THAN BOX OFFICE THEY WILL BE LISTED BELOW.'''


--SELECT TOP 1 @DBID = ISNULL(database_id,0), @DBNAME = NAME FROM sys.databases 
----DB WHICH ARE NOT REQUIRED TO TURN ON OR OFF NOISE_IN_USE
--WHERE NAME NOT IN ('master','model','msdb','tempdb','TalentAdminDB','ReportServer') ORDER BY database_id

--WHILE (@DBID > 0)
--	BEGIN
--	    SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + ' -- ************************* ' + @DBNAME + + ' ************************* -- ' 
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + 'USE ' + @DBNAME +
--		+ @LINEBREAK + 'GO'
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + 'IF EXISTS(SELECT * FROM SYS.TABLES A WHERE LOWER(A.NAME) = ''tbl_ecommerce_module_defaults_bu'')'
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + 'BEGIN'
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + '	UPDATE tbl_ecommerce_module_defaults_bu SET [VALUE] = ''True'' WHERE DEFAULT_NAME = ''NOISE_IN_USE'' AND BUSINESS_UNIT <> ''BOXOFFICE'''
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + '	SELECT ''' + @DBNAME + ''' as DBNAME, BUSINESS_UNIT, DEFAULT_NAME, [VALUE] FROM tbl_ecommerce_module_defaults_bu WHERE DEFAULT_NAME = ''NOISE_IN_USE'' AND LOWER(ISNULL(VALUE,'''')) = ''false'' AND BUSINESS_UNIT <> ''BOXOFFICE'''
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + 'END'
--		SET @SCRIPT_NOISE_OFFLINE = @SCRIPT_NOISE_OFFLINE + @LINEBREAK + ' ------------------------------ ends -------------------------- '

--		IF EXISTS(SELECT TOP 1 database_id FROM sys.databases WHERE database_id > @DBID AND NAME NOT IN ('master','model','msdb','tempdb','TalentAdminDB','ReportServer') ORDER BY database_id)
--			BEGIN
--				SELECT TOP 1 @DBID = ISNULL(database_id,0), @DBNAME = NAME FROM sys.databases 
--				WHERE database_id > @DBID
--				--DB WHICH ARE NOT REQUIRED TO TURN ON OR OFF NOISE_IN_USE 
--				AND NAME NOT IN ('master','model','msdb','tempdb','TalentAdminDB','ReportServer') ORDER BY database_id
--			END
--		ELSE
--			BEGIN
--				SET @DBID = 0
--			END
--	END
--	SELECT @SCRIPT_NOISE_OFFLINE