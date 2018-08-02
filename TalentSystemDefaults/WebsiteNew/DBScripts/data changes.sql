	-------------------------------------------------
	-- TBL_ECOMMERCE_MODULE_DEFAULTS_BU
	-------------------------------------------------
	insert into dbo.tbl_ecommerce_module_defaults_bu(BUSINESS_UNIT,PARTNER,APPLICATION,MODULE,DEFAULT_NAME,VALUE)
	select '*ALL','*ALL','','','TALENT_DEFAULTS_GROUP_ID','1'
	
	-------------------------------------------------
	-- TBL_DATABASE_VERSION
	-------------------------------------------------
	insert into tbl_database_version(BUSINESS_UNIT,DESTINATION_DATABASE,PARTNER,DATABASE_VERSION,DATABASE_TYPE1,DATABASE_TYPE2,CONNECTION_STRING)
	values('UNITEDKINGDOM','TalentDefinitions','*ALL','','','','Data Source=192.168.31.104;Initial Catalog=TalentDefinitions; User ID=talentuser; password=talentuser;')

	-------------------------------------------------
	-- TBL_BU_MODULE_DATABASE
	-------------------------------------------------
	insert into tbl_bu_module_database(BUSINESS_UNIT,PARTNER,APPLICATION,MODULE,DESTINATION_DATABASE,SECTION,SUB_SECTION)
	values ('UNITEDKINGDOM','*ALL','*ALL','TalentDefinitionsAccess','TalentDefinitions','*ALL','*ALL')

	-- WEBAPI URL 
	INSERT INTO DBO.TBL_ECOMMERCE_MODULE_DEFAULTS_BU(BUSINESS_UNIT, PARTNER, APPLICATION, MODULE, DEFAULT_NAME, VALUE)
SELECT 'BOXOFFICE','*ALL','','','SYSTEM_DEFAULTS_URL','http://localhost:63628/'

	-------------------------------------------------
	-- TBL_DESTINATION_TYPE
	-------------------------------------------------
	insert into tbl_destination_type(DESTINATION_TYPE_LINK,DESTINATION_TYPE)
	values('TalentDefinitions','DB')

	---------------------------------------------------------------------------------
	-- add new pages entries for showing them into the eBusiness application 
	---------------------------------------------------------------------------------
	
	-- tbl_page
	insert into dbo.tbl_page(BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, DESCRIPTION, PAGE_QUERYSTRING, USE_SECURE_URL, HTML_IN_USE, PAGE_TYPE, SHOW_PAGE_HEADER,								BCT_URL,BCT_PARENT, FORCE_LOGIN, IN_USE, CSS_PRINT, HIDE_IN_MAINTENANCE, ALLOW_GENERIC_SALES, RESTRICTING_ALERT_NAME, BODY_CSS_CLASS)
	select	BUSINESS_UNIT, PARTNER_CODE, 'DefaultList.aspx', 'Default List', PAGE_QUERYSTRING, USE_SECURE_URL, HTML_IN_USE, PAGE_TYPE, SHOW_PAGE_HEADER, BCT_URL,			BCT_PARENT,FORCE_LOGIN, IN_USE, CSS_PRINT, HIDE_IN_MAINTENANCE, ALLOW_GENERIC_SALES, RESTRICTING_ALERT_NAME, 'defaultlist'
	from	dbo.tbl_page a
	where	a.PAGE_CODE = 'SystemDefaults.aspx'
	union 
	select	BUSINESS_UNIT, PARTNER_CODE, 'DatabaseUpdates.aspx', 'Database Updates', PAGE_QUERYSTRING, USE_SECURE_URL, HTML_IN_USE, PAGE_TYPE, SHOW_PAGE_HEADER, BCT_URL, BCT_PARENT,			FORCE_LOGIN, IN_USE, CSS_PRINT, HIDE_IN_MAINTENANCE, ALLOW_GENERIC_SALES, RESTRICTING_ALERT_NAME, 'databaseupdates'
	from	dbo.tbl_page a
	where	a.PAGE_CODE = 'SystemDefaults.aspx'
	union 
	select	BUSINESS_UNIT, PARTNER_CODE, 'DatabaseSearch.aspx', 'Database Search', PAGE_QUERYSTRING, USE_SECURE_URL, HTML_IN_USE, PAGE_TYPE, SHOW_PAGE_HEADER, BCT_URL, BCT_PARENT,			FORCE_LOGIN, IN_USE, CSS_PRINT, HIDE_IN_MAINTENANCE, ALLOW_GENERIC_SALES, RESTRICTING_ALERT_NAME, 'databasesearch'
	from	dbo.tbl_page a
	where	a.PAGE_CODE = 'SystemDefaults.aspx'

	-- tbl_template_page
	insert	into tbl_template_page(BUSINESS_UNIT,PARTNER,PAGE_NAME,TEMPLATE_NAME)
	select	a.BUSINESS_UNIT,a.PARTNER,b.page_name,a.TEMPLATE_NAME 
	from	dbo.tbl_template_page a 
			cross join (select 'DefaultList.aspx' as page_name union select 'DatabaseUpdates.aspx' union select 'DatabaseSearch.aspx') b 
	where	a.PAGE_NAME = 'SystemDefaults.aspx'				

	insert	into dbo.tbl_page_text_lang(BUSINESS_UNIT,LANGUAGE_CODE,PAGE_CODE,PARTNER_CODE, TEXT_CODE,TEXT_CONTENT)
	select	'BOXOFFICE','ENG','SystemDefaultsBasePage.vb','PUBLIC','UnauthorizedRequestMsg','Unauthorized Request - Please log in.'
	union 
	select	'BOXOFFICE','ENG','SystemDefaultsBasePage.vb','PUBLIC','SitedownMsg','System defaults website is down. Contact system administrator.'
	union 
	select	'BOXOFFICE','ENG','SystemDefaultsBasePage.vb','PUBLIC','TimeoutErrorMsg','System defaults website did not respond timely.Contact system administrator.'
	union 
	select	'BOXOFFICE','ENG','SystemDefaultsBasePage.vb','PUBLIC','UnknownErrorMsg','There is some problem in system defaults website. Contact system administrator.'
	union 	
	select 'UNITEDKINGDOM','*ALL','SystemDefaultsBasePage.vb','UnhandledExceptionMsg','ENG','Unhandled exception occurred in WebAPI.Please contact system administrator.'
	union 
	select 'BOXOFFICE','*ALL','SystemDefaultsBasePage.vb','UnhandledExceptionMsg','ENG','Unhandled exception occurred in WebAPI.Please contact system administrator.'


	
