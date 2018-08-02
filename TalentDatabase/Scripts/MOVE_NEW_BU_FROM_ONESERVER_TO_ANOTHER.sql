-- Step 01. Copy and Paste the script from script file to SSMS window

-- Step 02. find and replace all [servername].databasename with your servername and databasename
-- for example for warriors in wb01 is [servername].databasename to [10.20.112.1\sqlexpress].TalentEBusinessDBWarriors

-- Step 03. find and replace all Test_Database_Name with you test database name 
-- for example for warriors Test_Database_Name to TEST_TalentEBusinessDBWarriors

-- Step 04. set all the values as mentioned below

-- Step 05. execute

-- Step 06. Repeat the steps from 1 to 5 for any other server as always safer from taking the base script file

-- Step 07. Check once the ecommerce module defaults settings

-- Step 08. Copy manually all the assets folder from test server to each live server

-- Step 09. Copy manually the themes folder to each server

-- Step 10. Have switched on queue make sure queue settings are available for this url

-- Step 11. Browse the site and enjoy if it coming fine 

 -- *** email templates, alerts, promotions tables are not considered

 -- Added all missing tables from usp_CopyByBU_BUTables_SelAndInsByBU.sql script. Also added email templates, alerts SP execution statement

DECLARE @pa_BusinessUnit_From_Test varchar(300),
		@pa_BusinessUnit_To_Live varchar(300),
		@pa_BusinessUnit_To_Live_Desc varchar(300),
		@pa_BusinessUnit_To_Live_Url varchar(300),
		@pa_BusinessUnit_Live_CopyFrom varchar(300),
		@HTML_PATH_ABSOLUTE varchar(300),
		@IMAGE_PATH_ABSOLUTE varchar(300),								
		@THEME_FOLDER  varchar(300),
		@MASTER_PAGE_FOLDER varchar(300),
		@STORED_PROCEDURE_GROUP  varchar(300),
		@ISTESTORLIVE varchar(300),
		@NOISE_ENCRYPTION_KEY varchar(300)
		
--business unit settings from test database
SET @pa_BusinessUnit_From_Test = 'MOBILE'

--business unit settings from test database to live database business unit and its description
SET @pa_BusinessUnit_To_Live = 'MOBILE'
SET @pa_BusinessUnit_To_Live_Desc = 'Mobile Site'

--live database business unit browsing url
SET @pa_BusinessUnit_To_Live_Url = 'www.warriors.talent-sport.co.uk/mobile'

-- some settings needed from live db business unit and then the script updates it for @pa_BusinessUnit_To_Live
-- so mostly you won't need to change this settings
SET @pa_BusinessUnit_Live_CopyFrom = 'UNITEDKINGDOM'

-- defaults html absolute path
SET @HTML_PATH_ABSOLUTE = 'D:\TalentEBusinessSuiteAssets\Warriors\Mobile\HTML'

-- defaults image path absolute
SET @IMAGE_PATH_ABSOLUTE = 'D:\TalentEBusinessSuiteAssets\Warriors\Mobile\Images'

-- theme folder name 
SET @THEME_FOLDER = 'Warriors-MOBILE'

-- Master page folder
SET @MASTER_PAGE_FOLDER = 'BoxOffice'

-- Stored Procedure Group Name
SET @STORED_PROCEDURE_GROUP = 'BoxOffice'

-- Test or Live
SET @ISTESTORLIVE = 'Live'

-- Noise Encryption Key
SET @NOISE_ENCRYPTION_KEY = ''

-----------------------------------------
---- Ecomm Mod Defs
-----------------------------------------

delete from [servername].databasename.dbo.tbl_ecommerce_module_defaults_bu
where business_unit = @pa_BusinessUnit_To_Live

insert into [servername].databasename.dbo.tbl_ecommerce_module_defaults_bu
(BUSINESS_UNIT, PARTNER, APPLICATION, MODULE, DEFAULT_NAME, VALUE)
select @pa_BusinessUnit_To_Live, PARTNER, APPLICATION, MODULE, DEFAULT_NAME, VALUE  
FROM [servername].databasename.dbo.tbl_ecommerce_module_defaults_bu
where business_unit = @pa_BusinessUnit_Live_CopyFrom

update [servername].databasename.dbo.tbl_ecommerce_module_defaults_bu
set value = @HTML_PATH_ABSOLUTE
where business_unit like @pa_BusinessUnit_To_Live and default_name like '%HTML_PATH_ABSOLUTE%'

update [servername].databasename.dbo.tbl_ecommerce_module_defaults_bu
set value = @IMAGE_PATH_ABSOLUTE
where business_unit like @pa_BusinessUnit_To_Live and default_name like '%IMAGE_PATH_ABSOLUTE%'

update [servername].databasename.dbo.tbl_ecommerce_module_defaults_bu
set value = @THEME_FOLDER
where business_unit = @pa_BusinessUnit_To_Live and default_name = 'THEME'

update [servername].databasename.dbo.tbl_ecommerce_module_defaults_bu
set value = 'FALSE'
where business_unit = @pa_BusinessUnit_To_Live and default_name = 'NOISE_IN_USE'

update [servername].databasename.dbo.tbl_ecommerce_module_defaults_bu
set value = 'FALSE'
where business_unit = @pa_BusinessUnit_To_Live and (default_name = 'SHOW_ACCOUNT_ACTIVATION_ON_LOGIN_SCREEN' 
									or default_name = 'SHOW_ACCOUNT_REGISTRATION_ON_LOGIN_SCREEN'
									or default_name = 'SHOW_REGISTRATION_FORM_ON_LOGIN_PAGE')
									
update [servername].databasename.dbo.tbl_ecommerce_module_defaults_bu
set value = 'FALSE'
where business_unit = @pa_BusinessUnit_To_Live and (default_name = 'SHOW_ACCOUNT_ACTIVATION_ON_LOGIN_SCREEN' 
									or default_name = 'SHOW_ACCOUNT_REGISTRATION_ON_LOGIN_SCREEN'
									or default_name = 'SHOW_REGISTRATION_FORM_ON_LOGIN_PAGE')
									
update [servername].databasename.dbo.tbl_ecommerce_module_defaults_bu
set value = @MASTER_PAGE_FOLDER
where business_unit like @pa_BusinessUnit_To_Live and default_name = 'MASTER_PAGE_FOLDER'
									
update [servername].databasename.dbo.tbl_ecommerce_module_defaults_bu
set value = @STORED_PROCEDURE_GROUP
where business_unit like @pa_BusinessUnit_To_Live and default_name = 'STORED_PROCEDURE_GROUP'

update [servername].databasename.dbo.tbl_ecommerce_module_defaults_bu
set value = @ISTESTORLIVE
where business_unit like @pa_BusinessUnit_To_Live and default_name = 'ISTESTORLIVE'

update [servername].databasename.dbo.tbl_ecommerce_module_defaults_bu
set value = @NOISE_ENCRYPTION_KEY
where business_unit like @pa_BusinessUnit_To_Live and default_name = 'NOISE_ENCRYPTION_KEY'

-----------------------------------------
---- Control text lang
-----------------------------------------

delete from [servername].databasename.dbo.tbl_control_text_lang
where business_unit = @pa_BusinessUnit_To_Live

insert into [servername].databasename.dbo.tbl_control_text_lang
(LANGUAGE_CODE,BUSINESS_UNIT,PARTNER_CODE,PAGE_CODE,CONTROL_CODE,TEXT_CODE,CONTROL_CONTENT)
select LANGUAGE_CODE,BUSINESS_UNIT,PARTNER_CODE,PAGE_CODE,CONTROL_CODE,TEXT_CODE,CONTROL_CONTENT  
FROM Test_Database_Name.dbo.tbl_control_text_lang
where business_unit = @pa_BusinessUnit_From_Test

-----------------------------------------
---- Control Attribute
-----------------------------------------

delete from [servername].databasename.dbo.tbl_control_ATTRIBUTE
where business_unit = @pa_BusinessUnit_To_Live

insert into [servername].databasename.dbo.tbl_control_ATTRIBUTE
(BUSINESS_UNIT,PARTNER_CODE,PAGE_CODE,CONTROL_CODE,ATTR_NAME,ATTR_VALUE,DESCRIPTION)
select BUSINESS_UNIT,PARTNER_CODE,PAGE_CODE,CONTROL_CODE,ATTR_NAME,ATTR_VALUE,DESCRIPTION 
FROM Test_Database_Name.dbo.tbl_control_ATTRIBUTE
where business_unit = @pa_BusinessUnit_From_Test

----------------------------------
---- Template Page
----------------------------------

delete from [servername].databasename.dbo.tbl_TEMPLATE_PAGE
where business_unit = @pa_BusinessUnit_To_Live

insert into [servername].databasename.dbo.tbl_TEMPLATE_PAGE
(BUSINESS_UNIT,PARTNER,PAGE_NAME,TEMPLATE_NAME)
select BUSINESS_UNIT,PARTNER,PAGE_NAME,TEMPLATE_NAME
FROM Test_Database_Name.dbo.tbl_TEMPLATE_PAGE
where business_unit = @pa_BusinessUnit_From_Test

--------------------------
---- URL BU
--------------------------

delete from [servername].databasename.dbo.tbl_url_bu
where business_unit = @pa_BusinessUnit_To_Live

insert into [servername].databasename.dbo.tbl_url_bu
(URL,BUSINESS_UNIT,APPLICATION,BU_GROUP)
values (@pa_BusinessUnit_To_Live_Url,@pa_BusinessUnit_To_Live,'EBusiness','')

-------------------
---- title bu
-------------------

update [servername].databasename.dbo.tbl_title_bu
set business_unit = '*ALL'

---------------------
---- Payment type bu
---------------------
delete from [servername].databasename.dbo.tbl_payment_type_bu
where business_unit = @pa_BusinessUnit_To_Live

INSERT INTO [servername].databasename.[dbo].[tbl_payment_type_bu]
           ([QUALIFIER]
           ,[BUSINESS_UNIT]
           ,[PARTNER]
           ,[PAYMENT_TYPE_CODE]
           ,[DEFAULT_PAYMENT_TYPE]
           ,[RETAIL_TYPE]
           ,[TICKETING_TYPE]
           ,[AGENT_RETAIL_TYPE]
           ,[AGENT_TICKETING_TYPE]
           ,[AGENT_TICKETING_TYPE_REFUND]
           ,[TICKETING_TYPE_REFUND]
           ,[PPS_TYPE]
           ,[GENERIC_SALES]
           ,[GENERIC_SALES_REFUND])
     SELECT 
     [QUALIFIER]
      ,@pa_BusinessUnit_To_Live
      ,[PARTNER]
      ,[PAYMENT_TYPE_CODE]
      ,[DEFAULT_PAYMENT_TYPE]
      ,[RETAIL_TYPE]
      ,[TICKETING_TYPE]
      ,[AGENT_RETAIL_TYPE]
      ,[AGENT_TICKETING_TYPE]
      ,[AGENT_TICKETING_TYPE_REFUND]
      ,[TICKETING_TYPE_REFUND]
      ,[PPS_TYPE]
      ,[GENERIC_SALES]
      ,[GENERIC_SALES_REFUND]
  FROM Test_Database_Name.[dbo].[tbl_payment_type_bu] WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test
  

-------------------------------
---- ticketing products 
-------------------------------
delete from [servername].databasename.[dbo].[tbl_ticketing_products]
where business_unit = @pa_BusinessUnit_To_Live

INSERT INTO [servername].databasename.[dbo].[tbl_ticketing_products]
           ([BUSINESS_UNIT],[PARTNER],[PRODUCT_TYPE],[DISPLAY_SEQUENCE],[ACTIVE],[LOCATION])
select [BUSINESS_UNIT],[PARTNER],[PRODUCT_TYPE],[DISPLAY_SEQUENCE],[ACTIVE],[LOCATION]
from Test_Database_Name.dbo.[tbl_ticketing_products]
     where business_unit = @pa_BusinessUnit_From_Test
     
delete from [servername].databasename.dbo.tbl_database_version
where business_unit = @pa_BusinessUnit_To_Live

INSERT INTO [servername].databasename.[dbo].[tbl_database_version]
           ([BUSINESS_UNIT],[PARTNER],[DESTINATION_DATABASE],[DATABASE_VERSION],[DATABASE_TYPE1],[DATABASE_TYPE2],[CONNECTION_STRING],[ENCRYPTION_KEY_KEY])
select   @pa_BusinessUnit_To_Live,[PARTNER],[DESTINATION_DATABASE],[DATABASE_VERSION],[DATABASE_TYPE1],[DATABASE_TYPE2],[CONNECTION_STRING],[ENCRYPTION_KEY_KEY]
from [servername].databasename.dbo.tbl_database_version
where business_unit = @pa_BusinessUnit_Live_CopyFrom

-------------------------
---- bu module database
-------------------------
delete from [servername].databasename.[dbo].[tbl_bu_module_database]
where business_unit = @pa_BusinessUnit_To_Live

INSERT INTO [servername].databasename.[dbo].[tbl_bu_module_database]
           ([BUSINESS_UNIT],[PARTNER],[APPLICATION],[MODULE],[DESTINATION_DATABASE],[SECTION],[SUB_SECTION],[TIMEOUT])
select @pa_BusinessUnit_To_Live,[PARTNER],[APPLICATION],[MODULE],[DESTINATION_DATABASE],[SECTION],[SUB_SECTION],[TIMEOUT]
from [servername].databasename.[dbo].[tbl_bu_module_database]
where business_unit = @pa_BusinessUnit_Live_CopyFrom

--adhoc fees

	DELETE [servername].databasename.dbo.tbl_adhoc_fees WHERE BUSINESS_UNIT=@pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_adhoc_fees (BUSINESS_UNIT, [PARTNER], LANGUAGE_CODE, FEE_CODE, FEE_DESCRIPTION, IS_NEGATIVE_FEE, 
			FEE_PRICE, FEE_TYPE, FEE_HTML, FEE_GIFT_AID, FEE_SOURCE, FEE_IN_USE)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, [PARTNER], LANGUAGE_CODE, FEE_CODE, FEE_DESCRIPTION, IS_NEGATIVE_FEE, 
				FEE_PRICE, FEE_TYPE, FEE_HTML, FEE_GIFT_AID, FEE_SOURCE, FEE_IN_USE
			FROM Test_Database_Name.dbo.tbl_adhoc_fees WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	--tbl_authorized_partners
	DELETE [servername].databasename.dbo.tbl_authorized_partners WHERE BUSINESS_UNIT=@pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_authorized_partners (BUSINESS_UNIT, [PARTNER], DEFAULT_PARTNER)
			SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, [PARTNER], DEFAULT_PARTNER 
					FROM Test_Database_Name.dbo.tbl_authorized_partners WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test			
					
					
	--tbl_bu
	DELETE [servername].databasename.dbo.tbl_bu WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT [servername].databasename.dbo.tbl_bu (BUSINESS_UNIT, BUSINESS_UNIT_DESC) 
					 VALUES (@pa_BusinessUnit_To_Live, @pa_BusinessUnit_To_Live_Desc)
					 

	--tbl_ebusiness_descriptions_bu
	DELETE [servername].databasename.dbo.tbl_ebusiness_descriptions_bu WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_ebusiness_descriptions_bu (BUSINESS_UNIT, QUALIFIER, [PARTNER], DESCRIPTION_TYPE, DESCRIPTION_CODE) 
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, 
			QUALIFIER, [PARTNER], DESCRIPTION_TYPE, DESCRIPTION_CODE 
				FROM Test_Database_Name.dbo.tbl_ebusiness_descriptions_bu WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	--tbl_country_bu
	DELETE [servername].databasename.dbo.tbl_country_bu WHERE BUSINESS_UNIT=@pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_country_bu (BUSINESS_UNIT, QUALIFIER, [PARTNER], COUNTRY_CODE, DEFAULT_COUNTRY, POSTCODE_MANDATORY)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, 
			QUALIFIER, [PARTNER], COUNTRY_CODE, DEFAULT_COUNTRY, POSTCODE_MANDATORY 
				FROM Test_Database_Name.dbo.tbl_country_bu WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test									 					
				       

	--tbl_language_bu
	DELETE [servername].databasename.dbo.tbl_language_bu WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_language_bu (BUSINESS_UNIT, [PARTNER], [LANGUAGE], ALLOW_SELECTION)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, 
			[PARTNER], [LANGUAGE], ALLOW_SELECTION
				FROM Test_Database_Name.dbo.tbl_language_bu WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	--tbl_page
		delete from [servername].databasename.dbo.tbl_PAGE where business_unit = @pa_BusinessUnit_To_Live
		insert into [servername].databasename.dbo.tbl_PAGE
		(BUSINESS_UNIT,PARTNER_CODE,PAGE_CODE,DESCRIPTION,PAGE_QUERYSTRING,USE_SECURE_URL,HTML_IN_USE,PAGE_TYPE,SHOW_PAGE_HEADER,BCT_URL,BCT_PARENT,FORCE_LOGIN,IN_USE,CSS_PRINT)
			select BUSINESS_UNIT,PARTNER_CODE,PAGE_CODE,DESCRIPTION,PAGE_QUERYSTRING,USE_SECURE_URL,HTML_IN_USE,PAGE_TYPE,SHOW_PAGE_HEADER,BCT_URL,BCT_PARENT,FORCE_LOGIN,IN_USE,CSS_PRINT 
			FROM Test_Database_Name.dbo.tbl_PAGE
			where business_unit = @pa_BusinessUnit_From_Test
	                    
	--tbl_page_attribute
	DELETE [servername].databasename.[dbo].tbl_page_attribute WHERE BUSINESS_UNIT=@pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.[dbo].tbl_page_attribute (BUSINESS_UNIT, ATTR_NAME, ATTR_VALUE, [DESCRIPTION], PAGE_CODE, PARTNER_CODE) 
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, 
			ATTR_NAME, ATTR_VALUE, [DESCRIPTION], PAGE_CODE, PARTNER_CODE 
				FROM Test_Database_Name.dbo.tbl_page_attribute WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test                
         

	--tbl_page_html
	DELETE [servername].databasename.dbo.tbl_page_html WHERE BUSINESS_UNIT=@pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_page_html (BUSINESS_UNIT, HTML_1, HTML_2, HTML_3, HTML_LOCATION, PAGE_CODE, PAGE_QUERYSTRING, [PARTNER], SECTION, SEQUENCE) 
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, 
			HTML_1, HTML_2, HTML_3, HTML_LOCATION, PAGE_CODE, PAGE_QUERYSTRING, [PARTNER], SECTION, SEQUENCE 
				FROM Test_Database_Name.dbo.tbl_page_html WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test
	                                    
	--tbl_page_lang
	DELETE [servername].databasename.dbo.tbl_page_lang WHERE BUSINESS_UNIT=@pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_page_lang (BUSINESS_UNIT, LANGUAGE_CODE, META_DESC, META_KEY, PAGE_CODE, PAGE_HEADER, PARTNER_CODE, TITLE) 
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, 
			LANGUAGE_CODE, META_DESC, META_KEY, PAGE_CODE, PAGE_HEADER, PARTNER_CODE, TITLE 
				FROM Test_Database_Name.dbo.tbl_page_lang WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test
	                                    
	--tbl_page_left_nav
	DELETE [servername].databasename.dbo.tbl_page_left_nav WHERE BUSINESS_UNIT=@pa_BusinessUnit_To_Live 
	INSERT INTO [servername].databasename.dbo.tbl_page_left_nav (BUSINESS_UNIT, BOTTOM_USER_NAV1, BOTTOM_USER_NAV2, BOTTOM_USER_NAV3, BOTTOM_USER_NAV4, BOTTOM_USER_NAV5, 
			MY_ACCOUNT_NAV, PAGE_NAME, PARTNER_CODE, PRODUCT_NAV, TOP_USER_NAV1) 
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, BOTTOM_USER_NAV1, BOTTOM_USER_NAV2, BOTTOM_USER_NAV3, BOTTOM_USER_NAV4, BOTTOM_USER_NAV5, 
			MY_ACCOUNT_NAV, PAGE_NAME, PARTNER_CODE, PRODUCT_NAV, TOP_USER_NAV1 
				FROM Test_Database_Name.dbo.tbl_page_left_nav WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test
	                                   
	                                    
	--tbl_sport_bu
	DELETE [servername].databasename.dbo.tbl_sport_bu WHERE BUSINESS_UNIT=@pa_BusinessUnit_To_Live 
	INSERT INTO [servername].databasename.dbo.tbl_sport_bu (BUSINESS_UNIT, [PARTNER], SPORT_CODE) 
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, [PARTNER], SPORT_CODE 
			FROM Test_Database_Name.dbo.tbl_sport_bu WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test                

	--tbl_sport_team_bu
	DELETE [servername].databasename.dbo.tbl_sport_team_bu WHERE BUSINESS_UNIT=@pa_BusinessUnit_To_Live 
	INSERT INTO [servername].databasename.dbo.tbl_sport_team_bu (BUSINESS_UNIT, [PARTNER], SPORT_CODE, TEAM_CODE) 
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, [PARTNER], SPORT_CODE, TEAM_CODE 
			FROM Test_Database_Name.dbo.tbl_sport_team_bu WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test
	                                    
	--tbl_sport_team_club_bu
	DELETE [servername].databasename.dbo.tbl_sport_team_club_bu WHERE BUSINESS_UNIT=@pa_BusinessUnit_To_Live 
	INSERT INTO [servername].databasename.dbo.tbl_sport_team_club_bu (BUSINESS_UNIT, [PARTNER], SPORT_CODE, TEAM_CODE, SC_CODE) 
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, [PARTNER], SPORT_CODE, TEAM_CODE, SC_CODE 
			FROM Test_Database_Name.dbo.tbl_sport_team_club_bu WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test
	                                    
	--tbl_template_page
	DELETE [servername].databasename.dbo.tbl_template_page WHERE BUSINESS_UNIT=@pa_BusinessUnit_To_Live 
	INSERT INTO [servername].databasename.dbo.tbl_template_page (BUSINESS_UNIT, PAGE_NAME, [PARTNER], TEMPLATE_NAME) 
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, PAGE_NAME, [PARTNER], TEMPLATE_NAME 
			FROM Test_Database_Name.dbo.tbl_template_page WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test
	                                    
	--tbl_currency_format_bu
	DELETE [servername].databasename.dbo.tbl_currency_format_bu WHERE BUSINESS_UNIT=@pa_BusinessUnit_To_Live 
	INSERT INTO [servername].databasename.dbo.tbl_currency_format_bu (BUSINESS_UNIT, CURRENCY_CODE, FORMAT_STRING, RULE_TYPE)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, CURRENCY_CODE, FORMAT_STRING, RULE_TYPE 
			FROM Test_Database_Name.dbo.tbl_currency_format_bu WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test
	                                        
	--tbl_group_level_01
	DELETE [servername].databasename.dbo.tbl_group_level_01 WHERE GROUP_L01_BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_group_level_01 (
		GROUP_L01_BUSINESS_UNIT, GROUP_L01_PARTNER, GROUP_L01_L01_GROUP,
		GROUP_L01_SEQUENCE, GROUP_L01_DESCRIPTION_1, GROUP_L01_DESCRIPTION_2,
		GROUP_L01_HTML_1, GROUP_L01_HTML_2, GROUP_L01_HTML_3,
		GROUP_L01_PAGE_TITLE, GROUP_L01_META_DESCRIPTION, GROUP_L01_META_KEYWORDS,
		GROUP_L01_ADV_SEARCH_TEMPLATE, GROUP_L01_PRODUCT_PAGE_TEMPLATE, GROUP_L01_PRODUCT_LIST_TEMPLATE,
		GROUP_L01_SHOW_CHILDREN_AS_GROUPS, GROUP_L01_SHOW_PRODUCTS_AS_LIST, GROUP_L01_SHOW_IN_NAVIGATION,
		GROUP_L01_SHOW_IN_GROUPED_NAV, GROUP_L01_HTML_GROUP, GROUP_L01_HTML_GROUP_TYPE,
		GROUP_L01_SHOW_PRODUCT_DISPLAY, GROUP_L01_ADHOC_GROUP, GROUP_L01_THEME
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT,
			GROUP_L01_PARTNER, GROUP_L01_L01_GROUP,
			GROUP_L01_SEQUENCE, GROUP_L01_DESCRIPTION_1, GROUP_L01_DESCRIPTION_2,
			GROUP_L01_HTML_1, GROUP_L01_HTML_2,GROUP_L01_HTML_3,
			GROUP_L01_PAGE_TITLE, GROUP_L01_META_DESCRIPTION,GROUP_L01_META_KEYWORDS,
			GROUP_L01_ADV_SEARCH_TEMPLATE, GROUP_L01_PRODUCT_PAGE_TEMPLATE, GROUP_L01_PRODUCT_LIST_TEMPLATE,
			GROUP_L01_SHOW_CHILDREN_AS_GROUPS, GROUP_L01_SHOW_PRODUCTS_AS_LIST, GROUP_L01_SHOW_IN_NAVIGATION,
			GROUP_L01_SHOW_IN_GROUPED_NAV, GROUP_L01_HTML_GROUP, GROUP_L01_HTML_GROUP_TYPE,
			GROUP_L01_SHOW_PRODUCT_DISPLAY, GROUP_L01_ADHOC_GROUP, GROUP_L01_THEME
		FROM Test_Database_Name.dbo.tbl_group_level_01 WHERE GROUP_L01_BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	-- tbl_group_level_02
	DELETE [servername].databasename.dbo.tbl_group_level_02 WHERE GROUP_L02_BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_group_level_02 (
		GROUP_L02_BUSINESS_UNIT, GROUP_L02_PARTNER,
		GROUP_L02_L01_GROUP, GROUP_L02_L02_GROUP,
		GROUP_L02_SEQUENCE, GROUP_L02_DESCRIPTION_1, GROUP_L02_DESCRIPTION_2,
		GROUP_L02_HTML_1, GROUP_L02_HTML_2, GROUP_L02_HTML_3,
		GROUP_L02_PAGE_TITLE, GROUP_L02_META_DESCRIPTION, GROUP_L02_META_KEYWORDS,
		GROUP_L02_ADV_SEARCH_TEMPLATE, GROUP_L02_PRODUCT_PAGE_TEMPLATE, GROUP_L02_PRODUCT_LIST_TEMPLATE,
		GROUP_L02_SHOW_CHILDREN_AS_GROUPS, GROUP_L02_SHOW_PRODUCTS_AS_LIST, GROUP_L02_SHOW_IN_NAVIGATION,
		GROUP_L02_SHOW_IN_GROUPED_NAV, GROUP_L02_HTML_GROUP, GROUP_L02_HTML_GROUP_TYPE,
		GROUP_L02_SHOW_PRODUCT_DISPLAY, GROUP_L02_ADHOC_GROUP, GROUP_L02_THEME
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, GROUP_L02_PARTNER,
			GROUP_L02_L01_GROUP, GROUP_L02_L02_GROUP,
			GROUP_L02_SEQUENCE, GROUP_L02_DESCRIPTION_1, GROUP_L02_DESCRIPTION_2,
			GROUP_L02_HTML_1, GROUP_L02_HTML_2, GROUP_L02_HTML_3,
			GROUP_L02_PAGE_TITLE, GROUP_L02_META_DESCRIPTION, GROUP_L02_META_KEYWORDS,
			GROUP_L02_ADV_SEARCH_TEMPLATE, GROUP_L02_PRODUCT_PAGE_TEMPLATE, GROUP_L02_PRODUCT_LIST_TEMPLATE,
			GROUP_L02_SHOW_CHILDREN_AS_GROUPS, GROUP_L02_SHOW_PRODUCTS_AS_LIST, GROUP_L02_SHOW_IN_NAVIGATION,
			GROUP_L02_SHOW_IN_GROUPED_NAV, GROUP_L02_HTML_GROUP, GROUP_L02_HTML_GROUP_TYPE,
			GROUP_L02_SHOW_PRODUCT_DISPLAY, GROUP_L02_ADHOC_GROUP, GROUP_L02_THEME
		FROM Test_Database_Name.dbo.tbl_group_level_02 WHERE GROUP_L02_BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	--tbl_group_level_03	
	DELETE [servername].databasename.dbo.tbl_group_level_03 WHERE GROUP_L03_BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_group_level_03 (
		GROUP_L03_BUSINESS_UNIT, GROUP_L03_PARTNER,
		GROUP_L03_L01_GROUP, GROUP_L03_L02_GROUP, GROUP_L03_L03_GROUP,
		GROUP_L03_SEQUENCE, GROUP_L03_DESCRIPTION_1, GROUP_L03_DESCRIPTION_2,
		GROUP_L03_HTML_1, GROUP_L03_HTML_2, GROUP_L03_HTML_3,
		GROUP_L03_PAGE_TITLE, GROUP_L03_META_DESCRIPTION, GROUP_L03_META_KEYWORDS,
		GROUP_L03_ADV_SEARCH_TEMPLATE, GROUP_L03_PRODUCT_PAGE_TEMPLATE, GROUP_L03_PRODUCT_LIST_TEMPLATE,
		GROUP_L03_SHOW_CHILDREN_AS_GROUPS, GROUP_L03_SHOW_PRODUCTS_AS_LIST, GROUP_L03_SHOW_IN_NAVIGATION, 
		GROUP_L03_SHOW_IN_GROUPED_NAV, GROUP_L03_HTML_GROUP, GROUP_L03_HTML_GROUP_TYPE, 
		GROUP_L03_SHOW_PRODUCT_DISPLAY, GROUP_L03_ADHOC_GROUP, GROUP_L03_THEME
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, GROUP_L03_PARTNER,
		GROUP_L03_L01_GROUP, GROUP_L03_L02_GROUP, GROUP_L03_L03_GROUP,
		GROUP_L03_SEQUENCE, GROUP_L03_DESCRIPTION_1, GROUP_L03_DESCRIPTION_2,
		GROUP_L03_HTML_1, GROUP_L03_HTML_2, GROUP_L03_HTML_3,
		GROUP_L03_PAGE_TITLE, GROUP_L03_META_DESCRIPTION, GROUP_L03_META_KEYWORDS,
		GROUP_L03_ADV_SEARCH_TEMPLATE, GROUP_L03_PRODUCT_PAGE_TEMPLATE, GROUP_L03_PRODUCT_LIST_TEMPLATE,
		GROUP_L03_SHOW_CHILDREN_AS_GROUPS, GROUP_L03_SHOW_PRODUCTS_AS_LIST, GROUP_L03_SHOW_IN_NAVIGATION, 
		GROUP_L03_SHOW_IN_GROUPED_NAV, GROUP_L03_HTML_GROUP, GROUP_L03_HTML_GROUP_TYPE, 
		GROUP_L03_SHOW_PRODUCT_DISPLAY, GROUP_L03_ADHOC_GROUP, GROUP_L03_THEME
		FROM Test_Database_Name.dbo.tbl_group_level_03 WHERE GROUP_L03_BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	-- tbl_group_level_04
	DELETE [servername].databasename.dbo.tbl_group_level_04 WHERE GROUP_L04_BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_group_level_04 (
		GROUP_L04_BUSINESS_UNIT, GROUP_L04_PARTNER,
		GROUP_L04_L01_GROUP, GROUP_L04_L02_GROUP, GROUP_L04_L03_GROUP, GROUP_L04_L04_GROUP,
		GROUP_L04_SEQUENCE, GROUP_L04_DESCRIPTION_1, GROUP_L04_DESCRIPTION_2,
		GROUP_L04_HTML_1, GROUP_L04_HTML_2, GROUP_L04_HTML_3,
		GROUP_L04_PAGE_TITLE, GROUP_L04_META_DESCRIPTION, GROUP_L04_META_KEYWORDS,
		GROUP_L04_ADV_SEARCH_TEMPLATE, GROUP_L04_PRODUCT_PAGE_TEMPLATE, GROUP_L04_PRODUCT_LIST_TEMPLATE,
		GROUP_L04_SHOW_CHILDREN_AS_GROUPS, GROUP_L04_SHOW_PRODUCTS_AS_LIST, GROUP_L04_SHOW_IN_NAVIGATION,
		GROUP_L04_SHOW_IN_GROUPED_NAV, GROUP_L04_HTML_GROUP, GROUP_L04_HTML_GROUP_TYPE,
		 GROUP_L04_ADHOC_GROUP, GROUP_L04_THEME
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, GROUP_L04_PARTNER,
		GROUP_L04_L01_GROUP, GROUP_L04_L02_GROUP, GROUP_L04_L03_GROUP, GROUP_L04_L04_GROUP,
		GROUP_L04_SEQUENCE, GROUP_L04_DESCRIPTION_1, GROUP_L04_DESCRIPTION_2,
		GROUP_L04_HTML_1, GROUP_L04_HTML_2, GROUP_L04_HTML_3,
		GROUP_L04_PAGE_TITLE, GROUP_L04_META_DESCRIPTION, GROUP_L04_META_KEYWORDS,
		GROUP_L04_ADV_SEARCH_TEMPLATE, GROUP_L04_PRODUCT_PAGE_TEMPLATE, GROUP_L04_PRODUCT_LIST_TEMPLATE,
		GROUP_L04_SHOW_CHILDREN_AS_GROUPS, GROUP_L04_SHOW_PRODUCTS_AS_LIST, GROUP_L04_SHOW_IN_NAVIGATION,
		GROUP_L04_SHOW_IN_GROUPED_NAV, GROUP_L04_HTML_GROUP, GROUP_L04_HTML_GROUP_TYPE,
		 GROUP_L04_ADHOC_GROUP, GROUP_L04_THEME
		FROM Test_Database_Name.dbo.tbl_group_level_04 WHERE GROUP_L04_BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	-- tbl_group_level_05
	DELETE [servername].databasename.dbo.tbl_group_level_05 WHERE GROUP_L05_BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_group_level_05 (
		GROUP_L05_BUSINESS_UNIT, GROUP_L05_PARTNER, 
		GROUP_L05_L01_GROUP, GROUP_L05_L02_GROUP, GROUP_L05_L03_GROUP, GROUP_L05_L04_GROUP, GROUP_L05_L05_GROUP,
		GROUP_L05_SEQUENCE, GROUP_L05_DESCRIPTION_1, GROUP_L05_DESCRIPTION_2,
		GROUP_L05_HTML_1, GROUP_L05_HTML_2, GROUP_L05_HTML_3,
		GROUP_L05_PAGE_TITLE, GROUP_L05_META_DESCRIPTION, GROUP_L05_META_KEYWORDS,
		GROUP_L05_ADV_SEARCH_TEMPLATE, GROUP_L05_PRODUCT_PAGE_TEMPLATE, GROUP_L05_PRODUCT_LIST_TEMPLATE,
		GROUP_L05_SHOW_CHILDREN_AS_GROUPS, GROUP_L05_SHOW_PRODUCTS_AS_LIST, GROUP_L05_SHOW_IN_NAVIGATION,
		GROUP_L05_SHOW_IN_GROUPED_NAV, GROUP_L05_HTML_GROUP, GROUP_L05_HTML_GROUP_TYPE,
		GROUP_L05_SHOW_PRODUCT_DISPLAY, GROUP_L05_ADHOC_GROUP, GROUP_L05_THEME
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, GROUP_L05_PARTNER, 
		GROUP_L05_L01_GROUP, GROUP_L05_L02_GROUP, GROUP_L05_L03_GROUP, GROUP_L05_L04_GROUP, GROUP_L05_L05_GROUP,
		GROUP_L05_SEQUENCE, GROUP_L05_DESCRIPTION_1, GROUP_L05_DESCRIPTION_2,
		GROUP_L05_HTML_1, GROUP_L05_HTML_2, GROUP_L05_HTML_3,
		GROUP_L05_PAGE_TITLE, GROUP_L05_META_DESCRIPTION, GROUP_L05_META_KEYWORDS,
		GROUP_L05_ADV_SEARCH_TEMPLATE, GROUP_L05_PRODUCT_PAGE_TEMPLATE, GROUP_L05_PRODUCT_LIST_TEMPLATE,
		GROUP_L05_SHOW_CHILDREN_AS_GROUPS, GROUP_L05_SHOW_PRODUCTS_AS_LIST, GROUP_L05_SHOW_IN_NAVIGATION,
		GROUP_L05_SHOW_IN_GROUPED_NAV, GROUP_L05_HTML_GROUP, GROUP_L05_HTML_GROUP_TYPE,
		GROUP_L05_SHOW_PRODUCT_DISPLAY, GROUP_L05_ADHOC_GROUP, GROUP_L05_THEME
		FROM Test_Database_Name.dbo.tbl_group_level_05 WHERE GROUP_L05_BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	--tbl_group_level_06
	DELETE [servername].databasename.dbo.tbl_group_level_06 WHERE GROUP_L06_BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_group_level_06 (
		GROUP_L06_BUSINESS_UNIT, GROUP_L06_PARTNER,
		GROUP_L06_L01_GROUP, GROUP_L06_L02_GROUP, GROUP_L06_L03_GROUP, GROUP_L06_L04_GROUP, GROUP_L06_L05_GROUP, GROUP_L06_L06_GROUP,
		GROUP_L06_SEQUENCE, GROUP_L06_DESCRIPTION_1, GROUP_L06_DESCRIPTION_2,
		GROUP_L06_HTML_1, GROUP_L06_HTML_2, GROUP_L06_HTML_3,
		GROUP_L06_PAGE_TITLE, GROUP_L06_META_DESCRIPTION, GROUP_L06_META_KEYWORDS,
		GROUP_L06_ADV_SEARCH_TEMPLATE, GROUP_L06_PRODUCT_PAGE_TEMPLATE, GROUP_L06_PRODUCT_LIST_TEMPLATE,
		GROUP_L06_SHOW_CHILDREN_AS_GROUPS, GROUP_L06_SHOW_PRODUCTS_AS_LIST, GROUP_L06_SHOW_IN_NAVIGATION,
		GROUP_L06_SHOW_IN_GROUPED_NAV, GROUP_L06_HTML_GROUP, GROUP_L06_HTML_GROUP_TYPE,
		GROUP_L06_SHOW_PRODUCT_DISPLAY, GROUP_L06_ADHOC_GROUP, GROUP_L06_THEME
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, GROUP_L06_PARTNER,
		GROUP_L06_L01_GROUP, GROUP_L06_L02_GROUP, GROUP_L06_L03_GROUP, GROUP_L06_L04_GROUP, GROUP_L06_L05_GROUP, GROUP_L06_L06_GROUP,
		GROUP_L06_SEQUENCE, GROUP_L06_DESCRIPTION_1, GROUP_L06_DESCRIPTION_2,
		GROUP_L06_HTML_1, GROUP_L06_HTML_2, GROUP_L06_HTML_3,
		GROUP_L06_PAGE_TITLE, GROUP_L06_META_DESCRIPTION, GROUP_L06_META_KEYWORDS,
		GROUP_L06_ADV_SEARCH_TEMPLATE, GROUP_L06_PRODUCT_PAGE_TEMPLATE, GROUP_L06_PRODUCT_LIST_TEMPLATE,
		GROUP_L06_SHOW_CHILDREN_AS_GROUPS, GROUP_L06_SHOW_PRODUCTS_AS_LIST, GROUP_L06_SHOW_IN_NAVIGATION,
		GROUP_L06_SHOW_IN_GROUPED_NAV, GROUP_L06_HTML_GROUP, GROUP_L06_HTML_GROUP_TYPE,
		GROUP_L06_SHOW_PRODUCT_DISPLAY, GROUP_L06_ADHOC_GROUP, GROUP_L06_THEME
		FROM Test_Database_Name.dbo.tbl_group_level_06 WHERE GROUP_L06_BUSINESS_UNIT = @pa_BusinessUnit_From_Test
		
	--tbl_group_level_07
	DELETE [servername].databasename.dbo.tbl_group_level_07 WHERE GROUP_L07_BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_group_level_07 (
		GROUP_L07_BUSINESS_UNIT, GROUP_L07_PARTNER,
		GROUP_L07_L01_GROUP, GROUP_L07_L02_GROUP, GROUP_L07_L03_GROUP, GROUP_L07_L04_GROUP, 
		GROUP_L07_L05_GROUP, GROUP_L07_L06_GROUP, GROUP_L07_L07_GROUP, 
		GROUP_L07_SEQUENCE, GROUP_L07_DESCRIPTION_1, GROUP_L07_DESCRIPTION_2,
		GROUP_L07_HTML_1, GROUP_L07_HTML_2, GROUP_L07_HTML_3,
		GROUP_L07_PAGE_TITLE, GROUP_L07_META_DESCRIPTION, GROUP_L07_META_KEYWORDS,
		GROUP_L07_ADV_SEARCH_TEMPLATE, GROUP_L07_PRODUCT_PAGE_TEMPLATE, GROUP_L07_PRODUCT_LIST_TEMPLATE,
		GROUP_L07_SHOW_CHILDREN_AS_GROUPS, GROUP_L07_SHOW_PRODUCTS_AS_LIST, GROUP_L07_SHOW_IN_NAVIGATION,
		GROUP_L07_SHOW_IN_GROUPED_NAV, GROUP_L07_HTML_GROUP, GROUP_L07_HTML_GROUP_TYPE,
		GROUP_L07_SHOW_PRODUCT_DISPLAY, GROUP_L07_ADHOC_GROUP, GROUP_L07_THEME
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, GROUP_L07_PARTNER,
		GROUP_L07_L01_GROUP, GROUP_L07_L02_GROUP, GROUP_L07_L03_GROUP, GROUP_L07_L04_GROUP, 
		GROUP_L07_L05_GROUP, GROUP_L07_L06_GROUP, GROUP_L07_L07_GROUP, 
		GROUP_L07_SEQUENCE, GROUP_L07_DESCRIPTION_1, GROUP_L07_DESCRIPTION_2,
		GROUP_L07_HTML_1, GROUP_L07_HTML_2, GROUP_L07_HTML_3,
		GROUP_L07_PAGE_TITLE, GROUP_L07_META_DESCRIPTION, GROUP_L07_META_KEYWORDS,
		GROUP_L07_ADV_SEARCH_TEMPLATE, GROUP_L07_PRODUCT_PAGE_TEMPLATE, GROUP_L07_PRODUCT_LIST_TEMPLATE,
		GROUP_L07_SHOW_CHILDREN_AS_GROUPS, GROUP_L07_SHOW_PRODUCTS_AS_LIST, GROUP_L07_SHOW_IN_NAVIGATION,
		GROUP_L07_SHOW_IN_GROUPED_NAV, GROUP_L07_HTML_GROUP, GROUP_L07_HTML_GROUP_TYPE,
		GROUP_L07_SHOW_PRODUCT_DISPLAY, GROUP_L07_ADHOC_GROUP, GROUP_L07_THEME
		FROM Test_Database_Name.dbo.tbl_group_level_07 WHERE GROUP_L07_BUSINESS_UNIT = @pa_BusinessUnit_From_Test
		
	--tbl_group_level_08
	DELETE [servername].databasename.dbo.tbl_group_level_08 WHERE GROUP_L08_BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_group_level_08 (
		GROUP_L08_BUSINESS_UNIT, GROUP_L08_PARTNER,
		GROUP_L08_L01_GROUP, GROUP_L08_L02_GROUP, GROUP_L08_L03_GROUP, GROUP_L08_L04_GROUP,
		GROUP_L08_L05_GROUP, GROUP_L08_L06_GROUP, GROUP_L08_L07_GROUP, GROUP_L08_L08_GROUP,
		GROUP_L08_SEQUENCE, GROUP_L08_DESCRIPTION_1, GROUP_L08_DESCRIPTION_2,
		GROUP_L08_HTML_1, GROUP_L08_HTML_2, GROUP_L08_HTML_3,
		GROUP_L08_PAGE_TITLE, GROUP_L08_META_DESCRIPTION, GROUP_L08_META_KEYWORDS,
		GROUP_L08_ADV_SEARCH_TEMPLATE, GROUP_L08_PRODUCT_PAGE_TEMPLATE, GROUP_L08_PRODUCT_LIST_TEMPLATE,
		GROUP_L08_SHOW_CHILDREN_AS_GROUPS, GROUP_L08_SHOW_PRODUCTS_AS_LIST, GROUP_L08_SHOW_IN_NAVIGATION,
		GROUP_L08_SHOW_IN_GROUPED_NAV, GROUP_L08_HTML_GROUP, GROUP_L08_HTML_GROUP_TYPE,
		GROUP_L08_SHOW_PRODUCT_DISPLAY, GROUP_L08_ADHOC_GROUP, GROUP_L08_THEME
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, GROUP_L08_PARTNER,
		GROUP_L08_L01_GROUP, GROUP_L08_L02_GROUP, GROUP_L08_L03_GROUP, GROUP_L08_L04_GROUP,
		GROUP_L08_L05_GROUP, GROUP_L08_L06_GROUP, GROUP_L08_L07_GROUP, GROUP_L08_L08_GROUP,
		GROUP_L08_SEQUENCE, GROUP_L08_DESCRIPTION_1, GROUP_L08_DESCRIPTION_2,
		GROUP_L08_HTML_1, GROUP_L08_HTML_2, GROUP_L08_HTML_3,
		GROUP_L08_PAGE_TITLE, GROUP_L08_META_DESCRIPTION, GROUP_L08_META_KEYWORDS,
		GROUP_L08_ADV_SEARCH_TEMPLATE, GROUP_L08_PRODUCT_PAGE_TEMPLATE, GROUP_L08_PRODUCT_LIST_TEMPLATE,
		GROUP_L08_SHOW_CHILDREN_AS_GROUPS, GROUP_L08_SHOW_PRODUCTS_AS_LIST, GROUP_L08_SHOW_IN_NAVIGATION,
		GROUP_L08_SHOW_IN_GROUPED_NAV, GROUP_L08_HTML_GROUP, GROUP_L08_HTML_GROUP_TYPE,
		GROUP_L08_SHOW_PRODUCT_DISPLAY, GROUP_L08_ADHOC_GROUP, GROUP_L08_THEME
		FROM Test_Database_Name.dbo.tbl_group_level_08 WHERE GROUP_L08_BUSINESS_UNIT = @pa_BusinessUnit_From_Test
		
	--tbl_group_level_09
	DELETE [servername].databasename.dbo.tbl_group_level_09 WHERE GROUP_L09_BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_group_level_09 (
		GROUP_L09_BUSINESS_UNIT, GROUP_L09_PARTNER,
		GROUP_L09_L01_GROUP, GROUP_L09_L02_GROUP, GROUP_L09_L03_GROUP, GROUP_L09_L04_GROUP, GROUP_L09_L05_GROUP, 
		GROUP_L09_L06_GROUP, GROUP_L09_L07_GROUP, GROUP_L09_L08_GROUP, GROUP_L09_L09_GROUP, 
		GROUP_L09_SEQUENCE, GROUP_L09_DESCRIPTION_1, GROUP_L09_DESCRIPTION_2,
		GROUP_L09_HTML_1, GROUP_L09_HTML_2, GROUP_L09_HTML_3,
		GROUP_L09_PAGE_TITLE, GROUP_L09_META_DESCRIPTION, GROUP_L09_META_KEYWORDS,
		GROUP_L09_ADV_SEARCH_TEMPLATE, GROUP_L09_PRODUCT_PAGE_TEMPLATE, GROUP_L09_PRODUCT_LIST_TEMPLATE,
		GROUP_L09_SHOW_CHILDREN_AS_GROUPS, GROUP_L09_SHOW_PRODUCTS_AS_LIST, GROUP_L09_SHOW_IN_NAVIGATION,
		GROUP_L09_SHOW_IN_GROUPED_NAV, GROUP_L09_HTML_GROUP, GROUP_L09_HTML_GROUP_TYPE,
		GROUP_L09_SHOW_PRODUCT_DISPLAY, GROUP_L09_ADHOC_GROUP, GROUP_L09_THEME
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, GROUP_L09_PARTNER,
		GROUP_L09_L01_GROUP, GROUP_L09_L02_GROUP, GROUP_L09_L03_GROUP, GROUP_L09_L04_GROUP, GROUP_L09_L05_GROUP, 
		GROUP_L09_L06_GROUP, GROUP_L09_L07_GROUP, GROUP_L09_L08_GROUP, GROUP_L09_L09_GROUP, 
		GROUP_L09_SEQUENCE, GROUP_L09_DESCRIPTION_1, GROUP_L09_DESCRIPTION_2,
		GROUP_L09_HTML_1, GROUP_L09_HTML_2, GROUP_L09_HTML_3,
		GROUP_L09_PAGE_TITLE, GROUP_L09_META_DESCRIPTION, GROUP_L09_META_KEYWORDS,
		GROUP_L09_ADV_SEARCH_TEMPLATE, GROUP_L09_PRODUCT_PAGE_TEMPLATE, GROUP_L09_PRODUCT_LIST_TEMPLATE,
		GROUP_L09_SHOW_CHILDREN_AS_GROUPS, GROUP_L09_SHOW_PRODUCTS_AS_LIST, GROUP_L09_SHOW_IN_NAVIGATION,
		GROUP_L09_SHOW_IN_GROUPED_NAV, GROUP_L09_HTML_GROUP, GROUP_L09_HTML_GROUP_TYPE,
		GROUP_L09_SHOW_PRODUCT_DISPLAY, GROUP_L09_ADHOC_GROUP, GROUP_L09_THEME
		FROM Test_Database_Name.dbo.tbl_group_level_09 WHERE GROUP_L09_BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	--tbl_group_level_10
	DELETE [servername].databasename.dbo.tbl_group_level_10 WHERE GROUP_L10_BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_group_level_10 (
		GROUP_L10_BUSINESS_UNIT, GROUP_L10_PARTNER,
		GROUP_L10_L01_GROUP, GROUP_L10_L02_GROUP, GROUP_L10_L03_GROUP, GROUP_L10_L04_GROUP, GROUP_L10_L05_GROUP,
		GROUP_L10_L06_GROUP, GROUP_L10_L07_GROUP, GROUP_L10_L08_GROUP, GROUP_L10_L09_GROUP, GROUP_L10_L10_GROUP,
		GROUP_L10_SEQUENCE, GROUP_L10_DESCRIPTION_1, GROUP_L10_DESCRIPTION_2,
		GROUP_L10_HTML_1, GROUP_L10_HTML_2, GROUP_L10_HTML_3,
		GROUP_L10_PAGE_TITLE, GROUP_L10_META_DESCRIPTION, GROUP_L10_META_KEYWORDS,
		GROUP_L10_ADV_SEARCH_TEMPLATE, GROUP_L10_PRODUCT_PAGE_TEMPLATE, GROUP_L10_PRODUCT_LIST_TEMPLATE,
		GROUP_L10_SHOW_CHILDREN_AS_GROUPS, GROUP_L10_SHOW_PRODUCTS_AS_LIST, GROUP_L10_SHOW_IN_NAVIGATION,
		GROUP_L10_SHOW_IN_GROUPED_NAV, GROUP_L10_HTML_GROUP, GROUP_L10_HTML_GROUP_TYPE,
		GROUP_L10_SHOW_PRODUCT_DISPLAY, GROUP_L10_ADHOC_GROUP, GROUP_L10_THEME
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, GROUP_L10_PARTNER,
		GROUP_L10_L01_GROUP, GROUP_L10_L02_GROUP, GROUP_L10_L03_GROUP, GROUP_L10_L04_GROUP, GROUP_L10_L05_GROUP,
		GROUP_L10_L06_GROUP, GROUP_L10_L07_GROUP, GROUP_L10_L08_GROUP, GROUP_L10_L09_GROUP, GROUP_L10_L10_GROUP,
		GROUP_L10_SEQUENCE, GROUP_L10_DESCRIPTION_1, GROUP_L10_DESCRIPTION_2,
		GROUP_L10_HTML_1, GROUP_L10_HTML_2, GROUP_L10_HTML_3,
		GROUP_L10_PAGE_TITLE, GROUP_L10_META_DESCRIPTION, GROUP_L10_META_KEYWORDS,
		GROUP_L10_ADV_SEARCH_TEMPLATE, GROUP_L10_PRODUCT_PAGE_TEMPLATE, GROUP_L10_PRODUCT_LIST_TEMPLATE,
		GROUP_L10_SHOW_CHILDREN_AS_GROUPS, GROUP_L10_SHOW_PRODUCTS_AS_LIST, GROUP_L10_SHOW_IN_NAVIGATION,
		GROUP_L10_SHOW_IN_GROUPED_NAV, GROUP_L10_HTML_GROUP, GROUP_L10_HTML_GROUP_TYPE,
		GROUP_L10_SHOW_PRODUCT_DISPLAY, GROUP_L10_ADHOC_GROUP, GROUP_L10_THEME
		FROM Test_Database_Name.dbo.tbl_group_level_10 WHERE GROUP_L10_BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	
	--tbl_group_product
	DELETE [servername].databasename.dbo.tbl_group_product WHERE GROUP_BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_group_product (
		GROUP_BUSINESS_UNIT, GROUP_PARTNER,
		GROUP_L01_GROUP, GROUP_L02_GROUP, GROUP_L03_GROUP, GROUP_L04_GROUP, GROUP_L05_GROUP,
		GROUP_L06_GROUP, GROUP_L07_GROUP, GROUP_L08_GROUP, GROUP_L09_GROUP, GROUP_L10_GROUP,
		PRODUCT, SEQUENCE, GROUP_ADHOC
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, GROUP_PARTNER,
		GROUP_L01_GROUP, GROUP_L02_GROUP, GROUP_L03_GROUP, GROUP_L04_GROUP, GROUP_L05_GROUP,
		GROUP_L06_GROUP, GROUP_L07_GROUP, GROUP_L08_GROUP, GROUP_L09_GROUP, GROUP_L10_GROUP,
		PRODUCT, SEQUENCE, GROUP_ADHOC
		FROM Test_Database_Name.dbo.tbl_group_product WHERE GROUP_BUSINESS_UNIT = @pa_BusinessUnit_From_Test
		
	--tbl_product_option_defaults
	DELETE [servername].databasename.dbo.tbl_product_option_defaults WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_product_option_defaults (
		BUSINESS_UNIT, PARTNER, 
		MASTER_PRODUCT, OPTION_TYPE, MATCH_ACTION, IS_DEFAULT,
		APPEND_SEQUENCE, DISPLAY_SEQUENCE, DISPLAY_TYPE
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER, 
		MASTER_PRODUCT, OPTION_TYPE, MATCH_ACTION, IS_DEFAULT,
		APPEND_SEQUENCE, DISPLAY_SEQUENCE, DISPLAY_TYPE
		FROM Test_Database_Name.dbo.tbl_product_option_defaults WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	--tbl_product_option_definitions_lang
	DELETE [servername].databasename.dbo.tbl_product_option_definitions_lang WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_product_option_definitions_lang (
		BUSINESS_UNIT, PARTNER,
		OPTION_CODE, LANGUAGE_CODE, DISPLAY_NAME
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER,
		OPTION_CODE, LANGUAGE_CODE, DISPLAY_NAME
		FROM Test_Database_Name.dbo.tbl_product_option_definitions_lang WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	-- tbl_product_option_types_lang
	DELETE [servername].databasename.dbo.tbl_product_option_types_lang WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_product_option_types_lang (
		BUSINESS_UNIT, PARTNER,
		OPTION_TYPE, LANGUAGE_CODE, DISPLAY_NAME, LABEL_TEXT
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER,
		OPTION_TYPE, LANGUAGE_CODE, DISPLAY_NAME, LABEL_TEXT
		FROM Test_Database_Name.dbo.tbl_product_option_types_lang WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test
		
	--tbl_product_options
	DELETE [servername].databasename.dbo.tbl_product_options WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_product_options (
		BUSINESS_UNIT, PARTNER,
		MASTER_PRODUCT, OPTION_TYPE, OPTION_CODE, PRODUCT_CODE, DISPLAY_ORDER
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER,
		MASTER_PRODUCT, OPTION_TYPE, OPTION_CODE, PRODUCT_CODE, DISPLAY_ORDER
		FROM Test_Database_Name.dbo.tbl_product_options WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test
		
	--tbl_product_relations
	DELETE [servername].databasename.dbo.tbl_product_relations WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_product_relations (
		BUSINESS_UNIT, PARTNER, QUALIFIER,
		GROUP_L01_GROUP, GROUP_L02_GROUP, GROUP_L03_GROUP, GROUP_L04_GROUP, GROUP_L05_GROUP,
		GROUP_L06_GROUP, GROUP_L07_GROUP, GROUP_L08_GROUP, GROUP_L09_GROUP, GROUP_L10_GROUP,
		PRODUCT, RELATED_GROUP_L01_GROUP, RELATED_GROUP_L02_GROUP, RELATED_GROUP_L03_GROUP,
		RELATED_GROUP_L04_GROUP, RELATED_GROUP_L05_GROUP, RELATED_GROUP_L06_GROUP,
		RELATED_GROUP_L07_GROUP, RELATED_GROUP_L08_GROUP, RELATED_GROUP_L09_GROUP,
		RELATED_GROUP_L10_GROUP, RELATED_PRODUCT, SEQUENCE, TICKETING_PRODUCT_TYPE, 
		TICKETING_PRODUCT_SUB_TYPE, RELATED_TICKETING_PRODUCT_TYPE, RELATED_TICKETING_PRODUCT_SUB_TYPE
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER, QUALIFIER,
		GROUP_L01_GROUP, GROUP_L02_GROUP, GROUP_L03_GROUP, GROUP_L04_GROUP, GROUP_L05_GROUP,
		GROUP_L06_GROUP, GROUP_L07_GROUP, GROUP_L08_GROUP, GROUP_L09_GROUP, GROUP_L10_GROUP,
		PRODUCT, RELATED_GROUP_L01_GROUP, RELATED_GROUP_L02_GROUP, RELATED_GROUP_L03_GROUP,
		RELATED_GROUP_L04_GROUP, RELATED_GROUP_L05_GROUP, RELATED_GROUP_L06_GROUP,
		RELATED_GROUP_L07_GROUP, RELATED_GROUP_L08_GROUP, RELATED_GROUP_L09_GROUP,
		RELATED_GROUP_L10_GROUP, RELATED_PRODUCT, SEQUENCE, TICKETING_PRODUCT_TYPE, 
		TICKETING_PRODUCT_SUB_TYPE, RELATED_TICKETING_PRODUCT_TYPE, RELATED_TICKETING_PRODUCT_SUB_TYPE
		FROM Test_Database_Name.dbo.tbl_product_relations WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	--tbl_product_relations_defaults
	DELETE [servername].databasename.dbo.tbl_product_relations_defaults WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_product_relations_defaults (
		BUSINESS_UNIT, PARTNER, PAGE_CODE, QUALIFIER,
		TEMPLATE_TYPE, PAGE_POSITION, ONOFF, SEQUENCE
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER, PAGE_CODE, QUALIFIER,
		TEMPLATE_TYPE, PAGE_POSITION, ONOFF, SEQUENCE
		FROM Test_Database_Name.dbo.tbl_product_relations_defaults WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test	

		
	--tbl_campaign_codes
	DELETE [servername].databasename.dbo.tbl_campaign_codes WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_campaign_codes (
		BUSINESS_UNIT, PARTNER, COUNTRY_CODE, CAMPAIGN_CODE
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER, COUNTRY_CODE, CAMPAIGN_CODE
		FROM Test_Database_Name.dbo.tbl_campaign_codes WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	--tbl_catalog_promo_codes
	DELETE [servername].databasename.dbo.tbl_catalog_promo_codes WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_catalog_promo_codes (
		BUSINESS_UNIT, PARTNER_CODE, PROMO_CODE
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER_CODE, PROMO_CODE
		FROM Test_Database_Name.dbo.tbl_catalog_promo_codes WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test 	

	--tbl_chip_and_pin_devices
	DELETE [servername].databasename.dbo.tbl_chip_and_pin_devices WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_chip_and_pin_devices (
		BUSINESS_UNIT, PARTNER, TERMINAL_DEVICE_NAME,
		TERMINAL_DEVICE_IP_ADDRESS, TERMINAL_DEVICE_USER
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER, TERMINAL_DEVICE_NAME,
		TERMINAL_DEVICE_IP_ADDRESS, TERMINAL_DEVICE_USER
		FROM Test_Database_Name.dbo.tbl_chip_and_pin_devices WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test 	

	--tbl_checkout_stage
	DELETE [servername].databasename.dbo.tbl_checkout_stage WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_checkout_stage (
		BUSINESS_UNIT, PARTNER, CHECKOUT_STAGE,
		PAGE_CODE, URL, PAYMENT_OPTIONS, MODULE,
		PRODUCT_TYPE, PRODUCT_SUB_TYPE, DISPLAY_PAYMENT_SUMMARY,
		AGENT_PAYMENT_OPTIONS
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER, CHECKOUT_STAGE,
		PAGE_CODE, URL, PAYMENT_OPTIONS, MODULE,
		PRODUCT_TYPE, PRODUCT_SUB_TYPE, DISPLAY_PAYMENT_SUMMARY,
		AGENT_PAYMENT_OPTIONS
		FROM Test_Database_Name.dbo.tbl_checkout_stage WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test 	

	--tbl_page_text_lang
	DELETE [servername].databasename.dbo.tbl_page_text_lang WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_page_text_lang (
		BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, TEXT_CODE, LANGUAGE_CODE, TEXT_CONTENT
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, TEXT_CODE, LANGUAGE_CODE, TEXT_CONTENT
		FROM Test_Database_Name.dbo.tbl_page_text_lang WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test	

	--tbl_stadium_area_colours
	DELETE [servername].databasename.dbo.tbl_stadium_area_colours WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_stadium_area_colours (
		BUSINESS_UNIT, STADIUM_CODE, [KEY_NAME], SEQUENCE, [MIN], [MAX], AREA_CATEGORY, COLOUR, FF_IN_AREA_COLOUR, [TEXT], CSS_CLASS
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, STADIUM_CODE, [KEY_NAME], SEQUENCE, [MIN], [MAX], AREA_CATEGORY, COLOUR, FF_IN_AREA_COLOUR, [TEXT], CSS_CLASS
		FROM Test_Database_Name.dbo.tbl_stadium_area_colours WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	--tbl_stadium_seat_colours
	DELETE [servername].databasename.dbo.tbl_stadium_seat_colours WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_stadium_seat_colours (
		BUSINESS_UNIT, STADIUM_CODE, SEQUENCE, SEAT_TYPE, OUTLINE_COLOUR, FILL_COLOUR, [TEXT], CSS_CLASS, DISPLAY_COLOUR
		)
		SELECT
			@pa_BusinessUnit_To_Live As [BUSINESS_UNIT], STADIUM_CODE, SEQUENCE, SEAT_TYPE, OUTLINE_COLOUR, FILL_COLOUR, [TEXT], CSS_CLASS, DISPLAY_COLOUR
		FROM Test_Database_Name.dbo.tbl_stadium_seat_colours WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	--tbl_error_messages
	DELETE [servername].databasename.dbo.tbl_error_messages WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO [servername].databasename.dbo.tbl_error_messages (
		LANGUAGE_CODE, BUSINESS_UNIT, PARTNER_CODE, MODULE, PAGE_CODE, ERROR_CODE, [ERROR_MESSAGE]
		)
		SELECT
			LANGUAGE_CODE, @pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER_CODE, MODULE, PAGE_CODE, ERROR_CODE, [ERROR_MESSAGE]
		FROM Test_Database_Name.dbo.tbl_error_messages WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test

-- ***************************************** Missing table start from here *********************************

	--tbl_flash_settings
	DELETE tbl_flash_settings WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_flash_settings (BUSINESS_UNIT, ATTRIBUTE_NAME, ATTRIBUTE_VALUE, PAGE_CODE, PARTNER_CODE, SEQUENCE, QUERYSTRING_PARAMETER)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, 
			ATTRIBUTE_NAME, ATTRIBUTE_VALUE, PAGE_CODE, PARTNER_CODE, SEQUENCE, QUERYSTRING_PARAMETER
				FROM tbl_flash_settings WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test

				
	--tbl_page_extra_data
	DELETE tbl_page_extra_data WHERE BUSINESS_UNIT=@pa_BusinessUnit_To_Live 
	INSERT INTO tbl_page_extra_data (BUSINESS_UNIT, [PARTNER], LANGUAGE_CODE, PAGE_CODE, LOCATION, DATA, SEQUENCE) 
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT, 
			[PARTNER], LANGUAGE_CODE, PAGE_CODE, LOCATION, DATA, SEQUENCE 
				FROM tbl_page_extra_data WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test

				
	--tbl_promotions
	DELETE tbl_promotions WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_promotions (
		BUSINESS_UNIT, PARTNER_GROUP, PARTNER,
		PROMOTION_CODE, PROMOTION_TYPE, ACTIVATION_MECHANISM,
		START_DATE, END_DATE, REDEEM_COUNT, REDEEM_MAX,
		MIN_SPEND, MIN_ITEMS, NEW_PRICE, USER_REDEEM_MAX, PRIORITY_SEQUENCE,
		ACTIVE, REQUIRED_USER_ATTRIBUTE, ADHOC_PROMOTION
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER_GROUP, PARTNER,
		PROMOTION_CODE, PROMOTION_TYPE, ACTIVATION_MECHANISM,
		START_DATE, END_DATE, REDEEM_COUNT, REDEEM_MAX,
		MIN_SPEND, MIN_ITEMS, NEW_PRICE, USER_REDEEM_MAX, PRIORITY_SEQUENCE,
		ACTIVE, REQUIRED_USER_ATTRIBUTE, ADHOC_PROMOTION
		FROM tbl_promotions WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_promotions_discounts
	DELETE tbl_promotions_discounts WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_promotions_discounts (
		BUSINESS_UNIT, PARTNER_GROUP, PARTNER,
		PROMOTION_CODE, IS_PERCENTAGE, VALUE, PRODUCT_CODE
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER_GROUP, PARTNER,
		PROMOTION_CODE, IS_PERCENTAGE, VALUE, PRODUCT_CODE
		FROM tbl_promotions_discounts WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_promotions_free_products
	DELETE tbl_promotions_free_products WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_promotions_free_products (
		BUSINESS_UNIT, PARTNER_GROUP, PARTNER,
		PROMOTION_CODE, PRODUCT_CODE, QUANTITY, ALLOW_SELECT_OPTION
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER_GROUP, PARTNER,
		PROMOTION_CODE, PRODUCT_CODE, QUANTITY, ALLOW_SELECT_OPTION
		FROM tbl_promotions_free_products WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_promotions_lang
	DELETE tbl_promotions_lang WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_promotions_lang (
		BUSINESS_UNIT, PARTNER_GROUP, PARTNER,
		PROMOTION_CODE, LANGUAGE_CODE, DISPLAY_NAME,
		REQUIREMENTS_DESCRIPTION, RULES_NOT_MET_ERROR, 
		USER_REDEEM_MAX_EXCEEDED_ERROR
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER_GROUP, PARTNER,
		PROMOTION_CODE, LANGUAGE_CODE, DISPLAY_NAME,
		REQUIREMENTS_DESCRIPTION, RULES_NOT_MET_ERROR, 
		USER_REDEEM_MAX_EXCEEDED_ERROR
		FROM tbl_promotions_lang WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_promotions_required_products
	DELETE tbl_promotions_required_products WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_promotions_required_products (
		BUSINESS_UNIT, PARTNER_GROUP, PARTNER,
		PROMOTION_CODE, PRODUCT_CODE, QUANTITY
		)
		SELECT
			@pa_BusinessUnit_To_Live As BUSINESS_UNIT, PARTNER_GROUP, PARTNER,
		PROMOTION_CODE, PRODUCT_CODE, QUANTITY
		FROM tbl_promotions_required_products WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_point_of_sale_terminals
	DELETE tbl_point_of_sale_terminals WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_point_of_sale_terminals (BUSINESS_UNIT, PARTNER, POS_TERMINAL_NAME, POS_TERMINAL_IP_ADDRESS, POS_TERMINAL_TCP_PORT)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, POS_TERMINAL_NAME, POS_TERMINAL_IP_ADDRESS, POS_TERMINAL_TCP_PORT
		FROM tbl_point_of_sale_terminals 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_delivery_zone
	DELETE tbl_delivery_zone WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_delivery_zone (BUSINESS_UNIT, PARTNER, DELIVERY_ZONE_CODE, DELIVERY_ZONE_REFERENCE, DELIVERY_ZONE_TYPE, MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, DELIVERY_ZONE_CODE, DELIVERY_ZONE_REFERENCE, DELIVERY_ZONE_TYPE, MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY
		FROM tbl_delivery_zone 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_page_tracking
	DELETE tbl_page_tracking WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_page_tracking (BUSINESS_UNIT, PARTNER, PAGE_CODE, LANGUAGE_CODE, LOCATION, TRACKING_PROVIDER, TRACKING_CONTENT)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, PAGE_CODE, LANGUAGE_CODE, LOCATION, TRACKING_PROVIDER, TRACKING_CONTENT
		FROM tbl_page_tracking 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_activity_templates
	DELETE tbl_activity_templates WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_activity_templates (BUSINESS_UNIT, NAME, TEMPLATE_TYPE)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,NAME, TEMPLATE_TYPE
		FROM tbl_activity_templates 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_page_header_nav
	DELETE tbl_page_header_nav WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_page_header_nav (BUSINESS_UNIT, PARTNER_CODE, PAGE_NAME, LEFT_USER_NAV, CONTACT_US, MY_ACCOUNT, BASKET, CHECKOUT, LOGOUT, QUICK_ORDER, RIGHT_USER_NAV1, RIGHT_USER_NAV2, RIGHT_USER_NAV3)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER_CODE, PAGE_NAME, LEFT_USER_NAV, CONTACT_US, MY_ACCOUNT, BASKET, CHECKOUT, LOGOUT, QUICK_ORDER, RIGHT_USER_NAV1, RIGHT_USER_NAV2, RIGHT_USER_NAV3
		FROM tbl_page_header_nav 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_delivery
	DELETE tbl_delivery WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_delivery (BUSINESS_UNIT, PARTNER, DELIVERY_TYPE, SEQUENCE, DELIVERY_PARENT, [DEFAULT], DELIVERY_TYPE_ZONE_CODE)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, DELIVERY_TYPE, SEQUENCE, DELIVERY_PARENT, [DEFAULT], DELIVERY_TYPE_ZONE_CODE
		FROM tbl_delivery 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_delivery_values
	DELETE tbl_delivery_values WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_delivery_values (BUSINESS_UNIT, PARTNER, DELIVERY_TYPE, UPPER_BREAK, GREATER, AVAILABLE, GROSS_VALUE, NET_VALUE, TAX_VALUE, UPPER_BREAK_MODE)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, DELIVERY_TYPE, UPPER_BREAK, GREATER, AVAILABLE, GROSS_VALUE, NET_VALUE, TAX_VALUE, UPPER_BREAK_MODE
		FROM tbl_delivery_values 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_product_relations_text_lang
	DELETE tbl_product_relations_text_lang WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_product_relations_text_lang (BUSINESS_UNIT, PARTNER, PAGE_CODE, QUALIFIER, TEMPLATE_TYPE, PAGE_POSITION, LANGUAGE_CODE, TEXT_CODE, TEXT_VALUE)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, PAGE_CODE, QUALIFIER, TEMPLATE_TYPE, PAGE_POSITION, LANGUAGE_CODE, TEXT_CODE, TEXT_VALUE
		FROM tbl_product_relations_text_lang 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_product_relations_attribute_values
	DELETE tbl_product_relations_attribute_values WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_product_relations_attribute_values (BUSINESS_UNIT, PARTNER, PAGE_CODE, QUALIFIER, TEMPLATE_TYPE, PAGE_POSITION, ATTRIBUTE_CODE, ATTRIBUTE_VALUE)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, PAGE_CODE, QUALIFIER, TEMPLATE_TYPE, PAGE_POSITION, ATTRIBUTE_CODE, ATTRIBUTE_VALUE
		FROM tbl_product_relations_attribute_values 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_registration_type
	DELETE tbl_registration_type WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_registration_type (LANGUAGE_CODE, BUSINESS_UNIT, PARTNER_CODE, REGISTRATION_TYPE, REGISTRATION_BRANCH, IS_DEFAULT, IS_COMPANY_MANDATORY)
		SELECT LANGUAGE_CODE,@pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER_CODE, REGISTRATION_TYPE, REGISTRATION_BRANCH, IS_DEFAULT, IS_COMPANY_MANDATORY
		FROM tbl_registration_type 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_page_myaccount_nav
	DELETE tbl_page_myaccount_nav WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_page_myaccount_nav (BUSINESS_UNIT, PARTNER, PAGE_CODE, ITEM_ORDER, ITEM_NAME)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, PAGE_CODE, ITEM_ORDER, ITEM_NAME
		FROM tbl_page_myaccount_nav 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_page_footer_nav
	DELETE tbl_page_footer_nav WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_page_footer_nav (BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, USER_NAV1, USER_NAV2, USER_NAV3, USER_NAV4, USER_NAV5)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER_CODE, PAGE_CODE, USER_NAV1, USER_NAV2, USER_NAV3, USER_NAV4, USER_NAV5
		FROM tbl_page_footer_nav 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_page_top_nav
	DELETE tbl_page_top_nav WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_page_top_nav (BUSINESS_UNIT, PARTNER_CODE, PAGE_NAME, LEFT_USER_NAV, TOP_LEVEL_PRODUCT_NAV, TOP_LEVEL_PRODUCT_GROUPING, RIGHT_USER_NAV1, RIGHT_USER_NAV2, RIGHT_USER_NAV3)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER_CODE, PAGE_NAME, LEFT_USER_NAV, TOP_LEVEL_PRODUCT_NAV, TOP_LEVEL_PRODUCT_GROUPING, RIGHT_USER_NAV1, RIGHT_USER_NAV2, RIGHT_USER_NAV3
		FROM tbl_page_top_nav 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_tracking_providers
	DELETE tbl_tracking_providers WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_tracking_providers (BUSINESS_UNIT, PARTNER, TRACKING_PROVIDER, SEQUENCE)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, TRACKING_PROVIDER, SEQUENCE
		FROM tbl_tracking_providers 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_tracking_settings_values
	DELETE tbl_tracking_settings_values WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_tracking_settings_values (BUSINESS_UNIT, PARTNER, TRACKING_PROVIDER, SETTING_NAME, VALUE)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, TRACKING_PROVIDER, SETTING_NAME, VALUE
		FROM tbl_tracking_settings_values 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_vouchers_external
	DELETE tbl_vouchers_external WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_vouchers_external (BUSINESS_UNIT, PARTNER, VOUCHER_ID, COMPANY, PRICE, AGREEMENT_CODE)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, VOUCHER_ID, COMPANY, PRICE, AGREEMENT_CODE
		FROM tbl_vouchers_external 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_destination_xml
	DELETE tbl_destination_xml WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_destination_xml (BUSINESS_UNIT, PARTNER, DESTINATION_TYPE, XML_VERSION, INPUT_XML_LOCATION, VALIDATE_XSD, VALIDATE_XSD_LOCATION, STORE_XML, STORE_XML_LOCATION, ARCHIVE_XML, ARCHIVE_XML_LOCATION, POST_XML, POST_XML_URL, EMAIL_XML_ATTACH, EMAIL_XML_CONTENT, EMAIL_XML_RECIPIENT, USERNAME, PASSWORD, DOMAIN_NAME)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, DESTINATION_TYPE, XML_VERSION, INPUT_XML_LOCATION, VALIDATE_XSD, VALIDATE_XSD_LOCATION, STORE_XML, STORE_XML_LOCATION, ARCHIVE_XML, ARCHIVE_XML_LOCATION, POST_XML, POST_XML_URL, EMAIL_XML_ATTACH, EMAIL_XML_CONTENT, EMAIL_XML_RECIPIENT, USERNAME, PASSWORD, DOMAIN_NAME
		FROM tbl_destination_xml 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_payment_defaults
	DELETE tbl_payment_defaults WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_payment_defaults (BUSINESS_UNIT, PARTNER, ACCOUNT_ID, DEFAULT_NAME, VALUE, MEANING_OF_VALUE)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, ACCOUNT_ID, DEFAULT_NAME, VALUE, MEANING_OF_VALUE
		FROM tbl_payment_defaults 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_stadiums
	DELETE tbl_stadiums WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_stadiums (STADIUM_CODE, STADIUM_NAME, STADIUM_WIDTH, STADIUM_HEIGHT, PITCH_POSITION, QUICK_BUY, STADIUM_WIDTH_RESIZED, STADIUM_HEIGHT_RESIZED, SEATING_AREA_WIDTH, SEATING_AREA_HEIGHT, VIEW_FROM_AREA_ENABLED, ORPHAN_SEAT_VALIDATION, ORPHAN_SEAT_BENCHMARK, BUSINESS_UNIT, CHANGE_STANDVIEW_VISIBLE, TOGGLE_ROW_SEATS_ON_SVG, FAVOURITE_SEAT)
		SELECT STADIUM_CODE, STADIUM_NAME, STADIUM_WIDTH, STADIUM_HEIGHT, PITCH_POSITION, QUICK_BUY, STADIUM_WIDTH_RESIZED, STADIUM_HEIGHT_RESIZED, SEATING_AREA_WIDTH, SEATING_AREA_HEIGHT, VIEW_FROM_AREA_ENABLED, ORPHAN_SEAT_VALIDATION, ORPHAN_SEAT_BENCHMARK, @pa_BusinessUnit_To_Live As BUSINESS_UNIT, CHANGE_STANDVIEW_VISIBLE, TOGGLE_ROW_SEATS_ON_SVG, FAVOURITE_SEAT
		FROM tbl_stadiums 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_marketing_campaigns_lang
	DELETE tbl_marketing_campaigns_lang WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_marketing_campaigns_lang (LANGUAGE_CODE, BUSINESS_UNIT, PARTNER_CODE, MARKETING_CAMPAIGN_CODE, MARKETING_CAMPAIGN_DESCRIPTION)
		SELECT LANGUAGE_CODE,@pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER_CODE, MARKETING_CAMPAIGN_CODE, MARKETING_CAMPAIGN_DESCRIPTION
		FROM tbl_marketing_campaigns_lang 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_creditcard_bu
	DELETE tbl_creditcard_bu WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_creditcard_bu (QUALIFIER, BUSINESS_UNIT, PARTNER, CARD_CODE, IS_DEFAULT)
		SELECT QUALIFIER,@pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, CARD_CODE, IS_DEFAULT
		FROM tbl_creditcard_bu 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_event_category
	DELETE tbl_event_category WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_event_category (BUSINESS_UNIT, PARTNER, CATEGORY_NUMBER, CATEGORY_DESCRIPTION)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, CATEGORY_NUMBER, CATEGORY_DESCRIPTION
		FROM tbl_event_category 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_contact_method_bu
	DELETE tbl_contact_method_bu WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_contact_method_bu (BUSINESS_UNIT, PARTNER, CONTACT_CODE, IS_DEFAULT)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, CONTACT_CODE, IS_DEFAULT
		FROM tbl_contact_method_bu 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_fees_relations
	DELETE tbl_fees_relations WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_fees_relations (BUSINESS_UNIT, PARTNER, FEE_CODE, FEE_CATEGORY, FEE_APPLY_TYPE, APPLY_RELATED_CODE, APPLY_RELATED_CODE_DESCRIPTION, ACTIVE)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, FEE_CODE, FEE_CATEGORY, FEE_APPLY_TYPE, APPLY_RELATED_CODE, APPLY_RELATED_CODE_DESCRIPTION, ACTIVE
		FROM tbl_fees_relations 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_defaults
	DELETE tbl_defaults WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_defaults (BUSINESS_UNIT, PARTNER, LANGUAGE_CODE, TIMESTAMP_ADDED, APPLY_TYPE, APPLY_CODE, APPLY_VALUE)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, LANGUAGE_CODE, TIMESTAMP_ADDED, APPLY_TYPE, APPLY_CODE, APPLY_VALUE
		FROM tbl_defaults 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_navigation_site_menus
	DELETE tbl_navigation_site_menus WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_navigation_site_menus (BUSINESS_UNIT, PARTNER, PAGE, LOCATION, POSITION, SEQUENCE, MENU_ID, XML_DOC, AGENT_MODE_BEHAVIOUR)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, PAGE, LOCATION, POSITION, SEQUENCE, MENU_ID, XML_DOC, AGENT_MODE_BEHAVIOUR
		FROM tbl_navigation_site_menus 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test


	--tbl_ticketing_products_lang
	DELETE tbl_ticketing_products_lang WHERE BUSINESS_UNIT = @pa_BusinessUnit_To_Live
	INSERT INTO tbl_ticketing_products_lang (BUSINESS_UNIT, PARTNER, PRODUCT_TYPE, LANGUAGE_CODE, DISPLAY_CONTENT, CSS_CLASS, NAVIGATE_URL, IMAGE_URL)
		SELECT @pa_BusinessUnit_To_Live As BUSINESS_UNIT,PARTNER, PRODUCT_TYPE, LANGUAGE_CODE, DISPLAY_CONTENT, CSS_CLASS, NAVIGATE_URL, IMAGE_URL
		FROM tbl_ticketing_products_lang 
		WHERE BUSINESS_UNIT = @pa_BusinessUnit_From_Test

	-- alerts
	EXEC usp_CopyByBU_Alert_SelAndInsByBU @pa_BusinessUnit_From_Test, @pa_BusinessUnit_To_Live
	
	-- email templates
	EXEC usp_CopyByBU_EmailTemplates_SelAndInsByBU @pa_BusinessUnit_From_Test, @pa_BusinessUnit_To_Live