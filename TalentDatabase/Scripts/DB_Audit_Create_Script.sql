
	/* 
		TBL_ECOMMERCE_MODULE_DEFAULTS 
	*/ 
	if not exists(select 1 from sys.tables a where a.name = 'tbl_ecommerce_module_defaults')
	begin 
		CREATE TABLE [dbo].[tbl_ecommerce_module_defaults](
			[BU_PARTNER_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[BUSINESS_UNIT] [nvarchar](50) NOT NULL,
			[PARTNER] [nvarchar](50) NOT NULL,
			[CONFIRMATION_EMAIL] [bit] NULL,
			[CUSTOMER_NUMBER_PREFIX] [nvarchar](10) NULL,
			[DEFAULT_BU] [bit] NULL,
			[DELIVERY_COST_PERC] [decimal](18, 5) NULL,
			[DELIVERY_ROUNDING_MASK] [nvarchar](20) NULL,
			[HTML_PATH_ABSOLUTE] [nvarchar](100) NULL,
			[HTML_PER_PAGE] [bit] NULL,
			[HTML_PER_PAGE_TYPE] [nvarchar](20) NULL,
			[IMAGE_PATH_ABSOLUTE] [nvarchar](100) NULL,
			[IMAGE_PATH_VIRTUAL] [nvarchar](100) NULL,
			[NUMBER_OF_GROUP_LEVELS] [int] NULL,
			[ORDER_NUMBER] [bigint] NULL,
			[ORDER_NUMBER_PREFIX] [nvarchar](10) NULL,
			[PARTNER_NUMBER] [bigint] NULL,
			[PAYMENT_GATEWAY_TYPE] [nvarchar](50) NULL,
			[PAYMENT_DETAILS_1] [nvarchar](100) NULL,
			[PAYMENT_DETAILS_2] [nvarchar](100) NULL,
			[PAYMENT_DETAILS_3] [nvarchar](100) NULL,
			[PAYMENT_DETAILS_4] [nvarchar](100) NULL,
			[PAYMENT_DETAILS_5] [nvarchar](100) NULL,
			[PAYMENT_URL_1] [nvarchar](100) NULL,
			[PAYMENT_URL_2] [nvarchar](100) NULL,
			[PAYMENT_URL_3] [nvarchar](100) NULL,
			[PAYMENT_REJECT_ADDRESS_AVS] [bit] NULL,
			[PAYMENT_REJECT_POSTCODE_AVS] [bit] NULL,
			[PAYMENT_REJECT_CSC] [bit] NULL,
			[PAYMENT_ALLOW_PARTIAL_AVS] [bit] NULL,
			[PAYMENT_DEBUG] [bit] NULL,
			[PAYMENT_CALL_BANK_API] [bit] NULL,
			[PRICE_LIST] [nvarchar](20) NULL,
			[STOCK_LOCATION] [nvarchar](20) NULL,
			[TEMP_ORDER_NUMBER] [bigint] NULL,
			[THEME] [nvarchar](50) NULL,
			[USER_NUMBER] [bigint] NULL,
			[USE_AGE_CHECK] [bit] NULL,
			[ORDER_DESTINATION_DATABASE] [nvarchar](20) NULL,
			[CUSTOMER_DESTINATION_DATABASE] [nvarchar](20) NULL,
			[STORED_PROCEDURE_GROUP] [nvarchar](50) NULL,
			[PRODUCT_HTML] [bit] NULL,
			[PRODUCT_HTML_TYPE] [nvarchar](100) NULL,
			[MIN_ADD_QUANTITY] [decimal](18, 3) NULL,
			[DEFAULT_ADD_QUANTITY] [decimal](18, 3) NULL,
			[REGISTRATION_CONFIRMATION_FROM_EMAIL] [nvarchar](max) NULL,
			[ORDERS_FROM_EMAIL] [nvarchar](max) NULL,
			[FORGOTTEN_PASSWORD_FROM_EMAIL] [nvarchar](max) NULL,
			[CONTACT_US_TO_EMAIL] [nvarchar](max) NULL,
			[CONTACT_US_TO_EMAIL_CC] [nvarchar](max) NULL,
			[CONTACT_US_FROM_EMAIL] [nvarchar](max) NULL,
			[CRM_BRANCH] [nvarchar](20) NULL,
			[ORDER_CARRIER_CODE] [nvarchar](20) NULL,
			[ORDER_CREATION_RETRY] [bit] NULL,
			[ORDER_CREATION_RETRY_ATTEMPTS] [int] NULL,
			[ORDER_CREATION_RETRY_WAIT] [int] NULL,
			[ORDER_CREATION_RETRY_ERRORS] [nvarchar](max) NULL,
			[STOCK_CHECK_RETRY] [bit] NULL,
			[STOCK_CHECK_RETRY_ATTEMPTS] [int] NULL,
			[STOCK_CHECK_RETRY_WAIT] [int] NULL,
			[STOCK_CHECK_RETRY_ERRORS] [nvarchar](max) NULL,
			[STOCK_CHECK_IGNORE_ERRORS] [nvarchar](max) NULL,
			[REGISTRATION_RETRY] [bit] NULL,
			[REGISTRATION_RETRY_ATTEMPTS] [int] NULL,
			[REGISTRATION_RETRY_WAIT] [int] NULL,
			[REGISTRATION_RETRY_ERRORS] [nvarchar](max) NULL,
			[LOGIN_TO_PRODUCT_BROWSE] [bit] NULL,
			[REGISTRATION_ENABLED] [bit] NULL,
			[REGISTRATION_TEMPLATE] [nvarchar](10) NULL,
			[UPDATE_DETAILS_TEMPLATE] [nvarchar](10) NULL,
			[INVOICE_HEADER_TEMPLATE] [nvarchar](10) NULL,
			[INVOICE_DETAIL_TEMPLATE] [nvarchar](10) NULL,
			[STATEMENT_HEADER_TEMPLATE] [nvarchar](10) NULL,
			[CREDITNOTE_HEADER_TEMPLATE] [nvarchar](10) NULL,
			[CREDITNOTE_DETAIL_TEMPLATE] [nvarchar](10) NULL,
		 CONSTRAINT [PK_tbl_bu_partner] PRIMARY KEY CLUSTERED 
		(
			[BUSINESS_UNIT] ASC,
			[PARTNER] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	end

	GO

	/* 
		TBL_GROUP_LEVEL_LANG 
	*/ 
	if not exists(select 1 from sys.tables a where a.name = 'tbl_group_level_lang')
	begin
		CREATE TABLE [dbo].[tbl_group_level_lang](
			[GROUP_LEVEL_LANG_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[GROUP_LANGUAGE] [nvarchar](20) NOT NULL,
			[GROUP_BUSINESS_UNIT] [nvarchar](50) NOT NULL,
			[GROUP_PARTNER] [nvarchar](50) NOT NULL,
			[GROUP_L01_GROUP] [nvarchar](20) NOT NULL,
			[GROUP_L02_GROUP] [nvarchar](20) NOT NULL,
			[GROUP_L03_GROUP] [nvarchar](20) NOT NULL,
			[GROUP_L04_GROUP] [nvarchar](20) NOT NULL,
			[GROUP_L05_GROUP] [nvarchar](20) NOT NULL,
			[GROUP_L06_GROUP] [nvarchar](20) NOT NULL,
			[GROUP_L07_GROUP] [nvarchar](20) NOT NULL,
			[GROUP_L08_GROUP] [nvarchar](20) NOT NULL,
			[GROUP_L09_GROUP] [nvarchar](20) NOT NULL,
			[GROUP_L10_GROUP] [nvarchar](20) NOT NULL,
			[GROUP_DESCRIPTION_1] [nvarchar](100) NULL,
			[GROUP_DESCRIPTION_2] [nvarchar](max) NULL,
			[GROUP_HTML_1] [nvarchar](max) NULL,
			[GROUP_HTML_2] [nvarchar](max) NULL,
			[GROUP_HTML_3] [nvarchar](max) NULL,
			[GROUP_PAGE_TITLE] [nvarchar](max) NULL,
			[GROUP_META_DESCRIPTION] [nvarchar](max) NULL,
			[GROUP_META_KEYWORDS] [nvarchar](max) NULL,
		 CONSTRAINT [PK_tbl_group_level_lang] PRIMARY KEY CLUSTERED 
		(
			[GROUP_LANGUAGE] ASC,
			[GROUP_BUSINESS_UNIT] ASC,
			[GROUP_PARTNER] ASC,
			[GROUP_L01_GROUP] ASC,
			[GROUP_L02_GROUP] ASC,
			[GROUP_L03_GROUP] ASC,
			[GROUP_L04_GROUP] ASC,
			[GROUP_L05_GROUP] ASC,
			[GROUP_L06_GROUP] ASC,
			[GROUP_L07_GROUP] ASC,
			[GROUP_L08_GROUP] ASC,
			[GROUP_L09_GROUP] ASC,
			[GROUP_L10_GROUP] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]	
	end
	GO


	/* 
		TBL_MENU 
	*/ 
	if not exists(select 1 from sys.tables a where a.name = 'tbl_menu')
	begin
		CREATE TABLE [dbo].[tbl_menu](
			[ID] [int] IDENTITY(1,1) NOT NULL,
			[BUSINESS_UNIT] [nvarchar](30) NOT NULL,
			[PARTNER_CODE] [nvarchar](30) NOT NULL,
			[MENU] [nvarchar](50) NOT NULL,
			[DESCRIPTION] [nvarchar](max) NULL,
			 CONSTRAINT [PK_tbl_menu] PRIMARY KEY CLUSTERED 
			(
				[BUSINESS_UNIT] ASC,
				[PARTNER_CODE] ASC,
				[MENU] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
			) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	end
	GO


	/* 
		TBL_MENU_ITEM 
	*/ 
	if not exists(select 1 from sys.tables a where a.name = 'tbl_menu_item')
	begin
		CREATE TABLE [dbo].[tbl_menu_item](
			[ID] [int] IDENTITY(1,1) NOT NULL,
			[BUSINESS_UNIT] [nvarchar](30) NOT NULL,
			[PARTNER_CODE] [nvarchar](30) NOT NULL,
			[MENU] [nvarchar](50) NOT NULL,
			[MENU_ITEM] [nvarchar](50) NOT NULL,
		 CONSTRAINT [PK_tbl_menu_item] PRIMARY KEY CLUSTERED 
		(
			[BUSINESS_UNIT] ASC,
			[PARTNER_CODE] ASC,
			[MENU] ASC,
			[MENU_ITEM] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]	
	end
	GO 

	ALTER TABLE [dbo].[tbl_menu_item] ADD  CONSTRAINT [DF_tbl_menu_item_BUSINESS_UNIT]  DEFAULT (N'CSG') FOR [BUSINESS_UNIT]
	GO

	ALTER TABLE [dbo].[tbl_menu_item] ADD  CONSTRAINT [DF_tbl_menu_item_PARTNER_CODE]  DEFAULT (N'CSG') FOR [PARTNER_CODE]
	GO

	/* 
		TBL_MENU_ITEM_LANG 
	*/ 
	if not exists(select 1 from sys.tables a where a.name = 'tbl_menu_item_lang')
	begin
		CREATE TABLE [dbo].[tbl_menu_item_lang](
			[ID] [int] IDENTITY(1,1) NOT NULL,
			[BUSINESS_UNIT] [nvarchar](30) NOT NULL,
			[PARTNER_CODE] [nvarchar](30) NOT NULL,
			[MENU] [nvarchar](50) NOT NULL,
			[MENU_ITEM] [nvarchar](50) NOT NULL,
			[LANGUAGE_CODE] [nvarchar](20) NOT NULL,
			[MENU_CONTENT] [nvarchar](max) NULL,
			[MENU_POSITION] [int] NOT NULL,
			[CSS_CLASS] [nvarchar](max) NULL,
			[HYPERLINK] [nvarchar](max) NULL,
		 CONSTRAINT [PK_tbl_menu_item_lang] PRIMARY KEY CLUSTERED 
		(
			[BUSINESS_UNIT] ASC,
			[PARTNER_CODE] ASC,
			[MENU] ASC,
			[MENU_ITEM] ASC,
			[LANGUAGE_CODE] ASC,
			[MENU_POSITION] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]	
	end
	GO

	/* 
		TBL_PAGE_TEXT 
	*/ 
	if not exists(select 1 from sys.tables a where a.name = 'tbl_page_text')
	begin
		CREATE TABLE [dbo].[tbl_page_text](
			[ID] [int] IDENTITY(1,1) NOT NULL,
			[BUSINESS_UNIT] [nvarchar](30) NOT NULL,
			[PARTNER_CODE] [nvarchar](30) NOT NULL,
			[PAGE_CODE] [nvarchar](50) NOT NULL,
			[TEXT_CODE] [nvarchar](50) NOT NULL,
			[DESCRIPTION] [nvarchar](max) NULL,
		 CONSTRAINT [PK_tbl_page_text] PRIMARY KEY CLUSTERED 
		(
			[BUSINESS_UNIT] ASC,
			[PARTNER_CODE] ASC,
			[PAGE_CODE] ASC,
			[TEXT_CODE] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	end
	GO

	if not exists(select 1 from sys.tables a where a.name = 'tbl_application')
	begin
		CREATE TABLE [dbo].[tbl_application](
		[APPLICATION_ID] [bigint] IDENTITY(1,1) NOT NULL,
		[APPLICATION] [nvarchar](50) NULL,
		[APPLICATION_DESCRIPTION] [nvarchar](100) NULL
		) ON [PRIMARY]
	end
	GO

	if not exists(select 1 from sys.tables a where a.name = 'tbl_application_module')
	begin
		CREATE TABLE [dbo].[tbl_application_module](
			[APPLICATION_MODULE_ID] [bigint] NULL,
			[APPLICATION] [nvarchar](50) NULL,
			[MODULE] [nvarchar](50) NULL
		) ON [PRIMARY]
	end
	GO


	if not exists(select 1 from sys.tables a where a.name = '[tbl_authorized_users_qanda]')
	begin
		CREATE TABLE [dbo].[tbl_authorized_users_qanda](
			[AUTH_QANDA_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[BUSINESS_UNIT] [nvarchar](50) NULL,
			[PARTNER] [nvarchar](50) NULL,
			[LOGINID] [nvarchar](100) NULL,
			[SEQUENCE] [nvarchar](50) NULL,
			[QUESTION] [nvarchar](max) NULL,
			[ANSWER] [nvarchar](max) NULL
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = '[tbl_bu_work]')
	begin
		CREATE TABLE [dbo].[tbl_bu_work](
			[BUSINESS_UNIT_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[BUSINESS_UNIT] [nvarchar](50) NOT NULL,
			[BUSINESS_UNIT_DESC] [nvarchar](100) NULL,
		 CONSTRAINT [PK_tbl_bu_work] PRIMARY KEY CLUSTERED 
		(
			[BUSINESS_UNIT] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_contact_method')
	begin
		CREATE TABLE [dbo].[tbl_contact_method](
			[ID] [bigint] IDENTITY(1,1) NOT NULL,
			[CONTACT_CODE] [nvarchar](20) NOT NULL,
			[DESCRIPTION] [nvarchar](100) NOT NULL
		) ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_descriptions_header')
	begin
		CREATE TABLE [dbo].[tbl_descriptions_header](
			[DESCRIPTIONS_HEADER_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[LANGUAGE] [nvarchar](3) NOT NULL,
			[TYPE] [nvarchar](10) NOT NULL,
			[DESCRIPTION] [nvarchar](64) NOT NULL,
		 CONSTRAINT [PK_tbl_descriptions_header] PRIMARY KEY CLUSTERED 
		(
			[LANGUAGE] ASC,
			[TYPE] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_dotnet_culture')
	begin
		CREATE TABLE [dbo].[tbl_dotnet_culture](
			[LCID] [int] NOT NULL,
			[NAME] [nvarchar](20) NULL,
			[THREE_LETTER_ISO_NAME] [char](3) NOT NULL,
			[TWO_LETTER_ISO_NAME] [char](2) NOT NULL,
			[THREE_LETTER_WINDOWS_NAME] [char](3) NOT NULL,
			[ENGLISH_NAME] [nvarchar](max) NULL,
		 CONSTRAINT [PK_tbl_dotnet_culture] PRIMARY KEY CLUSTERED 
		(
			[LCID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_ecommerce_defaults')
	begin
		CREATE TABLE [dbo].[tbl_ecommerce_defaults](
			[ECOMMERCE_DEFAULTS_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[BUSINESS_UNIT] [nvarchar](50) NULL
		) ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_load_test_audit_detail')
	begin
		CREATE TABLE [dbo].[tbl_load_test_audit_detail](
			[LOAD_TEST_ID] [bigint] NULL,
			[LOAD_TEST_DETAIL_ID] [nchar](10) NULL,
			[STAGE] [nvarchar](50) NULL,
			[START_TIME] [datetime] NULL,
			[CURR_TIME] [datetime] NULL,
			[SECTION_TIME] [int] NULL,
			[TOTAL_TIME] [int] NULL
		) ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_load_test_detail')
	begin
		CREATE TABLE [dbo].[tbl_load_test_detail](
			[LOAD_TEST_DETAIL_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[LOGINID] [nvarchar](50) NULL,
			[PASSWORD] [nvarchar](50) NULL
		) ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_load_test_header')
	begin
		CREATE TABLE [dbo].[tbl_load_test_header](
			[LOAD_TEST_HEADER_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[LOAD_TEST_DETAIL_ID] [bigint] NULL,
			[LOAD_TEST_ID] [bigint] NULL
		) ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_load_test_log')
	begin
		CREATE TABLE [dbo].[tbl_load_test_log](
			[LOAD_TEST_LOG] [nvarchar](max) NULL,
			[LOAD_TEST_ID] [bigint] NULL,
			[CURR_TIME] [datetime] NULL
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_module')
	begin
		CREATE TABLE [dbo].[tbl_module](
			[MODULE_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[MODULE] [nvarchar](50) NULL,
			[MODULE_DESCRIPTION] [nvarchar](50) NULL
		) ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_navigation_menu')
	begin
		CREATE TABLE [dbo].[tbl_navigation_menu](
			[ID] [bigint] IDENTITY(1,1) NOT NULL,
			[MENU_ID] [nvarchar](50) NOT NULL,
			[DESCRIPTION] [nvarchar](500) NOT NULL
		) ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_navigation_menu_item')
	begin
		CREATE TABLE [dbo].[tbl_navigation_menu_item](
			[ID] [bigint] IDENTITY(1,1) NOT NULL,
			[MENU_ID] [nvarchar](50) NOT NULL,
			[MENU_ITEM_ID] [nvarchar](50) NOT NULL,
			[DESCRIPTION] [nvarchar](500) NOT NULL
		) ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_page_myaccount_navx')
	begin
		CREATE TABLE [dbo].[tbl_page_myaccount_navx](
			[ID] [bigint] IDENTITY(1,1) NOT NULL,
			[BUSINESS_UNIT] [nvarchar](30) NOT NULL,
			[PARTNER] [nvarchar](30) NOT NULL,
			[PAGE_CODE] [nvarchar](50) NOT NULL,
			[UPDATE_DETAILS] [int] NOT NULL,
			[ORDER_TEMPLATE] [int] NOT NULL,
			[ORDER_ENQUIRY] [int] NOT NULL,
			[CONTACT_US] [int] NOT NULL,
			[INVOICE_ENQUIRY] [int] NULL
		) ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_product_bu')
	begin
		CREATE TABLE [dbo].[tbl_product_bu](
			[PRODUCT_BU_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[PRODUCT_BUSINESS_UNIT] [nvarchar](50) NOT NULL,
			[PRODUCT_PARTNER] [nvarchar](50) NOT NULL,
			[PRODUCT] [nvarchar](20) NOT NULL,
			[PRODUCT_DESCRIPTION_1] [nvarchar](100) NULL,
			[PRODUCT_DESCRIPTION_2] [nvarchar](max) NULL,
			[PRODUCT_DESCRIPTION_3] [nvarchar](max) NULL,
			[PRODUCT_DESCRIPTION_4] [nvarchar](max) NULL,
			[PRODUCT_DESCRIPTION_5] [nvarchar](max) NULL,
			[PRODUCT_TASTING_NOTES_1] [nvarchar](max) NULL,
			[PRODUCT_TASTING_NOTES_2] [nvarchar](max) NULL,
			[PRODUCT_ABV] [nvarchar](50) NULL,
			[PRODUCT_SUPPLIER] [nvarchar](50) NULL,
			[PRODUCT_COUNTRY] [nvarchar](50) NULL,
			[PRODUCT_REGION] [nvarchar](50) NULL,
			[PRODUCT_AREA] [nvarchar](50) NULL,
			[PRODUCT_GRAPE] [nvarchar](50) NULL,
			[PRODUCT_CLOSURE] [nvarchar](50) NULL,
			[PRODUCT_HTML_1] [nvarchar](max) NULL,
			[PRODUCT_HTML_2] [nvarchar](max) NULL,
			[PRODUCT_HTML_3] [nvarchar](max) NULL,
			[PRODUCT_SEARCH_KEYWORDS] [nvarchar](max) NULL,
			[PRODUCT_PAGE_TITLE] [nvarchar](max) NULL,
			[PRODUCT_META_DESCRIPTION] [nvarchar](max) NULL,
			[PRODUCT_META_KEYWORDS] [nvarchar](max) NULL,
			[PRODUCT_SEARCH_RANGE_01_FROM] [int] NULL,
			[PRODUCT_SEARCH_RANGE_01_TO] [int] NULL,
			[PRODUCT_SEARCH_RANGE_02_FROM] [int] NULL,
			[PRODUCT_SEARCH_RANGE_02_TO] [int] NULL,
			[PRODUCT_SEARCH_RANGE_03_FROM] [int] NULL,
			[PRODUCT_SEARCH_RANGE_03_TO] [int] NULL,
			[PRODUCT_SEARCH_RANGE_04_FROM] [int] NULL,
			[PRODUCT_SEARCH_RANGE_04_TO] [int] NULL,
			[PRODUCT_SEARCH_RANGE_05_FROM] [int] NULL,
			[PRODUCT_SEARCH_RANGE_05_TO] [int] NULL,
			[PRODUCT_SEARCH_CRITERIA_01] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_02] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_03] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_04] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_05] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_06] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_07] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_08] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_09] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_10] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_11] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_12] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_13] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_14] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_15] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_16] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_17] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_18] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_19] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_20] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_SWITCH_01] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_02] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_03] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_04] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_05] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_06] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_07] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_08] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_09] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_10] [bit] NULL,
			[PRODUCT_SEARCH_DATE_01_FROM] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_01_TO] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_02_FROM] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_02_TO] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_03_FROM] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_03_TO] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_04_FROM] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_04_TO] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_05_FROM] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_05_TO] [datetime] NULL,
		 CONSTRAINT [PK_tbl_product_bu] PRIMARY KEY CLUSTERED 
		(
			[PRODUCT_BUSINESS_UNIT] ASC,
			[PRODUCT_PARTNER] ASC,
			[PRODUCT] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_product_BKUP')
	begin
		CREATE TABLE [dbo].[tbl_product_BKUP](
			[PRODUCT_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[PRODUCT_CODE] [nvarchar](20) NOT NULL,
			[PRODUCT_DESCRIPTION_1] [nvarchar](100) NULL,
			[PRODUCT_DESCRIPTION_2] [nvarchar](max) NULL,
			[PRODUCT_DESCRIPTION_3] [nvarchar](max) NULL,
			[PRODUCT_DESCRIPTION_4] [nvarchar](max) NULL,
			[PRODUCT_DESCRIPTION_5] [nvarchar](max) NULL,
			[PRODUCT_LENGTH] [decimal](18, 3) NULL,
			[PRODUCT_LENGTH_UOM] [nvarchar](20) NULL,
			[PRODUCT_WIDTH] [decimal](18, 3) NULL,
			[PRODUCT_WIDTH_UOM] [nvarchar](20) NULL,
			[PRODUCT_DEPTH] [decimal](18, 3) NULL,
			[PRODUCT_DEPTH_UOM] [nvarchar](20) NULL,
			[PRODUCT_HEIGHT] [decimal](18, 3) NULL,
			[PRODUCT_HEIGHT_UOM] [nvarchar](20) NULL,
			[PRODUCT_SIZE] [decimal](18, 3) NULL,
			[PRODUCT_SIZE_UOM] [nvarchar](20) NULL,
			[PRODUCT_WEIGHT] [decimal](18, 3) NULL,
			[PRODUCT_WEIGHT_UOM] [nvarchar](20) NULL,
			[PRODUCT_VOLUME] [decimal](18, 3) NULL,
			[PRODUCT_VOLUME_UOM] [nvarchar](20) NULL,
			[PRODUCT_COLOUR] [nvarchar](50) NULL,
			[PRODUCT_PACK_SIZE] [nvarchar](20) NULL,
			[PRODUCT_PACK_SIZE_UOM] [nvarchar](20) NULL,
			[PRODUCT_SUPPLIER_PART_NO] [nvarchar](20) NULL,
			[PRODUCT_CUSTOMER_PART_NO] [nvarchar](20) NULL,
			[PRODUCT_TASTING_NOTES_1] [nvarchar](max) NULL,
			[PRODUCT_TASTING_NOTES_2] [nvarchar](max) NULL,
			[PRODUCT_ABV] [nvarchar](20) NULL,
			[PRODUCT_VINTAGE] [int] NULL,
			[PRODUCT_SUPPLIER] [nvarchar](50) NULL,
			[PRODUCT_COUNTRY] [nvarchar](50) NULL,
			[PRODUCT_REGION] [nvarchar](50) NULL,
			[PRODUCT_AREA] [nvarchar](50) NULL,
			[PRODUCT_GRAPE] [nvarchar](50) NULL,
			[PRODUCT_CLOSURE] [nvarchar](50) NULL,
			[PRODUCT_CATALOG_CODE] [nvarchar](50) NULL,
			[PRODUCT_VEGETARIAN] [bit] NULL,
			[PRODUCT_VEGAN] [bit] NULL,
			[PRODUCT_ORGANIC] [bit] NULL,
			[PRODUCT_BIODYNAMIC] [bit] NULL,
			[PRODUCT_LUTTE] [bit] NULL,
			[PRODUCT_MINIMUM_AGE] [int] NULL,
			[PRODUCT_HTML_1] [nvarchar](max) NULL,
			[PRODUCT_HTML_2] [nvarchar](max) NULL,
			[PRODUCT_HTML_3] [nvarchar](max) NULL,
			[PRODUCT_SEARCH_KEYWORDS] [nvarchar](max) NULL,
			[PRODUCT_PAGE_TITLE] [nvarchar](max) NULL,
			[PRODUCT_META_DESCRIPTION] [nvarchar](max) NULL,
			[PRODUCT_META_KEYWORDS] [nvarchar](max) NULL,
			[PRODUCT_SEARCH_RANGE_01] [decimal](18, 5) NULL,
			[PRODUCT_SEARCH_RANGE_02] [decimal](18, 5) NULL,
			[PRODUCT_SEARCH_RANGE_03] [decimal](18, 5) NULL,
			[PRODUCT_SEARCH_RANGE_04] [decimal](18, 5) NULL,
			[PRODUCT_SEARCH_RANGE_05] [decimal](18, 5) NULL,
			[PRODUCT_SEARCH_CRITERIA_01] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_02] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_03] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_04] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_05] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_06] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_07] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_08] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_09] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_10] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_11] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_12] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_13] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_14] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_15] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_16] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_17] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_18] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_19] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_20] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_SWITCH_01] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_02] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_03] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_04] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_05] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_06] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_07] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_08] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_09] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_10] [bit] NULL,
			[PRODUCT_SEARCH_DATE_01] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_02] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_03] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_04] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_05] [datetime] NULL,
			[PRODUCT_TARIFF_CODE] [nvarchar](15) NULL,
			[PRODUCT_OPTION_MASTER] [bit] NULL
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_product_bu_lang')
	begin
		CREATE TABLE [dbo].[tbl_product_bu_lang](
			[PRODUCT_BU_LANG_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[PRODUCT_LANGUAGE] [nvarchar](20) NOT NULL,
			[PRODUCT_BUSINESS_UNIT] [nvarchar](50) NOT NULL,
			[PRODUCT_PARTNER] [nvarchar](50) NOT NULL,
			[PRODUCT] [nvarchar](20) NOT NULL,
			[PRODUCT_DESCRIPTION_1] [nvarchar](100) NULL,
			[PRODUCT_DESCRIPTION_2] [nvarchar](max) NULL,
			[PRODUCT_DESCRIPTION_3] [nvarchar](max) NULL,
			[PRODUCT_DESCRIPTION_4] [nvarchar](max) NULL,
			[PRODUCT_DESCRIPTION_5] [nvarchar](max) NULL,
			[PRODUCT_TASTING_NOTES_1] [nvarchar](max) NULL,
			[PRODUCT_TASTING_NOTES_2] [nvarchar](max) NULL,
			[PRODUCT_ABV] [nvarchar](50) NULL,
			[PRODUCT_SUPPLIER] [nvarchar](50) NULL,
			[PRODUCT_COUNTRY] [nvarchar](50) NULL,
			[PRODUCT_REGION] [nvarchar](50) NULL,
			[PRODUCT_AREA] [nvarchar](50) NULL,
			[PRODUCT_GRAPE] [nvarchar](50) NULL,
			[PRODUCT_CLOSURE] [nvarchar](50) NULL,
			[PRODUCT_HTML_1] [nvarchar](max) NULL,
			[PRODUCT_HTML_2] [nvarchar](max) NULL,
			[PRODUCT_HTML_3] [nvarchar](max) NULL,
			[PRODUCT_SEARCH_KEYWORDS] [nvarchar](max) NULL,
			[PRODUCT_PAGE_TITLE] [nvarchar](max) NULL,
			[PRODUCT_META_DESCRIPTION] [nvarchar](max) NULL,
			[PRODUCT_META_KEYWORDS] [nvarchar](max) NULL,
			[PRODUCT_SEARCH_CRITERIA_01] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_02] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_03] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_04] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_05] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_06] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_07] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_08] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_09] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_10] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_11] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_12] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_13] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_14] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_15] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_16] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_17] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_18] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_19] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_20] [nvarchar](50) NULL,
		 CONSTRAINT [PK_tbl_product_bu_lang] PRIMARY KEY CLUSTERED 
		(
			[PRODUCT_LANGUAGE] ASC,
			[PRODUCT_BUSINESS_UNIT] ASC,
			[PRODUCT_PARTNER] ASC,
			[PRODUCT] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_product_from_backup')
	begin
		CREATE TABLE [dbo].[tbl_product_from_backup](
			[PRODUCT_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[PRODUCT_CODE] [nvarchar](20) NOT NULL,
			[PRODUCT_DESCRIPTION_1] [nvarchar](100) NULL,
			[PRODUCT_DESCRIPTION_2] [nvarchar](max) NULL,
			[PRODUCT_DESCRIPTION_3] [nvarchar](max) NULL,
			[PRODUCT_DESCRIPTION_4] [nvarchar](max) NULL,
			[PRODUCT_DESCRIPTION_5] [nvarchar](max) NULL,
			[PRODUCT_LENGTH] [decimal](18, 3) NULL,
			[PRODUCT_LENGTH_UOM] [nvarchar](20) NULL,
			[PRODUCT_WIDTH] [decimal](18, 3) NULL,
			[PRODUCT_WIDTH_UOM] [nvarchar](20) NULL,
			[PRODUCT_DEPTH] [decimal](18, 3) NULL,
			[PRODUCT_DEPTH_UOM] [nvarchar](20) NULL,
			[PRODUCT_HEIGHT] [decimal](18, 3) NULL,
			[PRODUCT_HEIGHT_UOM] [nvarchar](20) NULL,
			[PRODUCT_SIZE] [decimal](18, 3) NULL,
			[PRODUCT_SIZE_UOM] [nvarchar](20) NULL,
			[PRODUCT_WEIGHT] [decimal](18, 3) NULL,
			[PRODUCT_WEIGHT_UOM] [nvarchar](20) NULL,
			[PRODUCT_VOLUME] [decimal](18, 3) NULL,
			[PRODUCT_VOLUME_UOM] [nvarchar](20) NULL,
			[PRODUCT_COLOUR] [nvarchar](50) NULL,
			[PRODUCT_PACK_SIZE] [nvarchar](20) NULL,
			[PRODUCT_PACK_SIZE_UOM] [nvarchar](20) NULL,
			[PRODUCT_SUPPLIER_PART_NO] [nvarchar](20) NULL,
			[PRODUCT_CUSTOMER_PART_NO] [nvarchar](20) NULL,
			[PRODUCT_TASTING_NOTES_1] [nvarchar](max) NULL,
			[PRODUCT_TASTING_NOTES_2] [nvarchar](max) NULL,
			[PRODUCT_ABV] [nvarchar](20) NULL,
			[PRODUCT_VINTAGE] [int] NULL,
			[PRODUCT_SUPPLIER] [nvarchar](50) NULL,
			[PRODUCT_COUNTRY] [nvarchar](50) NULL,
			[PRODUCT_REGION] [nvarchar](50) NULL,
			[PRODUCT_AREA] [nvarchar](50) NULL,
			[PRODUCT_GRAPE] [nvarchar](50) NULL,
			[PRODUCT_CLOSURE] [nvarchar](50) NULL,
			[PRODUCT_CATALOG_CODE] [nvarchar](50) NULL,
			[PRODUCT_VEGETARIAN] [bit] NULL,
			[PRODUCT_VEGAN] [bit] NULL,
			[PRODUCT_ORGANIC] [bit] NULL,
			[PRODUCT_BIODYNAMIC] [bit] NULL,
			[PRODUCT_LUTTE] [bit] NULL,
			[PRODUCT_MINIMUM_AGE] [int] NULL,
			[PRODUCT_HTML_1] [nvarchar](max) NULL,
			[PRODUCT_HTML_2] [nvarchar](max) NULL,
			[PRODUCT_HTML_3] [nvarchar](max) NULL,
			[PRODUCT_SEARCH_KEYWORDS] [nvarchar](max) NULL,
			[PRODUCT_PAGE_TITLE] [nvarchar](max) NULL,
			[PRODUCT_META_DESCRIPTION] [nvarchar](max) NULL,
			[PRODUCT_META_KEYWORDS] [nvarchar](max) NULL,
			[PRODUCT_SEARCH_RANGE_01] [decimal](18, 5) NULL,
			[PRODUCT_SEARCH_RANGE_02] [decimal](18, 5) NULL,
			[PRODUCT_SEARCH_RANGE_03] [decimal](18, 5) NULL,
			[PRODUCT_SEARCH_RANGE_04] [decimal](18, 5) NULL,
			[PRODUCT_SEARCH_RANGE_05] [decimal](18, 5) NULL,
			[PRODUCT_SEARCH_CRITERIA_01] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_02] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_03] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_04] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_05] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_06] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_07] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_08] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_09] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_10] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_11] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_12] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_13] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_14] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_15] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_16] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_17] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_18] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_19] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_CRITERIA_20] [nvarchar](50) NULL,
			[PRODUCT_SEARCH_SWITCH_01] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_02] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_03] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_04] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_05] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_06] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_07] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_08] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_09] [bit] NULL,
			[PRODUCT_SEARCH_SWITCH_10] [bit] NULL,
			[PRODUCT_SEARCH_DATE_01] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_02] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_03] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_04] [datetime] NULL,
			[PRODUCT_SEARCH_DATE_05] [datetime] NULL,
			[PRODUCT_TARIFF_CODE] [nvarchar](15) NULL,
			[PRODUCT_OPTION_MASTER] [bit] NULL
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_product_relations_text')
	begin
		CREATE TABLE [dbo].[tbl_product_relations_text](
			[ID] [bigint] IDENTITY(1,1) NOT NULL,
			[TEXT_CODE] [nvarchar](50) NOT NULL,
			[DESCRIPTION] [nvarchar](max) NULL
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	end
	GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_product_relations_attributes')
	begin
		CREATE TABLE [dbo].[tbl_product_relations_attributes](
			[ID] [bigint] IDENTITY(1,1) NOT NULL,
			[ATTRIBUTE_CODE] [nvarchar](50) NOT NULL,
			[DESCRIPTION] [nvarchar](max) NULL,
			[DATATYPE] [nvarchar](50) NOT NULL
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	end
	GO
	
	--if not exists(select 1 from sys.tables a where a.name = 'tbl_qualifier')
	--begin
	--	CREATE TABLE [dbo].[tbl_qualifier](
	--		[QUALIFIER_ID] [bigint] IDENTITY(1,1) NOT NULL,
	--		[QUALIFIER] [nvarchar](20) NOT NULL,
	--		[QUALIFIER_DESCRIPTION] [nvarchar](100) NULL,
	--	 CONSTRAINT [PK_tbl_qualifier] PRIMARY KEY CLUSTERED 
	--	(
	--		[QUALIFIER] ASC
	--	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	--	) ON [PRIMARY]
	--end
	--GO
	
	if not exists(select 1 from sys.tables a where a.name = 'tbl_navigation_site')
	begin
		CREATE TABLE [dbo].[tbl_navigation_site](
			[ID] [bigint] IDENTITY(1,1) NOT NULL,
			[BUSINESS_UNIT] [nvarchar](50) NOT NULL,
			[PARTNER] [nvarchar](50) NOT NULL,
			[PAGE] [nvarchar](50) NOT NULL,
			[LOCATION] [nvarchar](50) NOT NULL,
			[MENU_BEFORE] [bit] NOT NULL,
			[TICKETING_PRODUCT_NAVIGATION] [bit] NOT NULL,
			[PRODUCT_NAVIGATION] [bit] NOT NULL,
			[MENU_AFTER] [bit] NOT NULL
		) ON [PRIMARY]
	end
	GO

	if not exists(select 1 from sys.tables a where a.name = 'tbl_tax_code')
	begin
		CREATE TABLE [dbo].[tbl_tax_code](
			[TAX_CODE_ID] [bigint] IDENTITY(1,1) NOT NULL,
			[TAX_CODE] [nvarchar](20) NOT NULL,
			[TAX_CODE_DESCRIPTION] [nvarchar](100) NULL,
			[TAX_PERCENTAGE] [decimal](18, 5) NULL,
		 CONSTRAINT [PK_tbl_tax_code] PRIMARY KEY CLUSTERED 
		(
			[TAX_CODE] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	end
	GO 

	if not exists(select 1 from sys.tables a where a.name = 'tbl_ticketing_stadium')
	begin
		CREATE TABLE [dbo].[tbl_ticketing_stadium](
			[PATNER] [nvarchar](50) NOT NULL,
			[STADIUM] [nvarchar](2) NOT NULL,
			[STAND] [nvarchar](3) NOT NULL,
			[STAND_DESCRIPTION] [nvarchar](50) NOT NULL,
			[AREA] [nvarchar](4) NOT NULL,
			[AREA_DESCRIPTION] [nvarchar](50) NOT NULL
		) ON [PRIMARY]
	end
	GO

	if not exists(select 1 from sys.tables a where a.name = 'tbl_control_text')
	begin
		CREATE TABLE [dbo].[tbl_control_text](
			[ID] [int] IDENTITY(1,1) NOT NULL,
			[CONTROL_CODE] [nvarchar](max) NULL,
			[TEXT_CODE] [nvarchar](50) NOT NULL,
			[DESCRIPTION] [nvarchar](max) NULL
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	end
	GO 

	if not exists(select 1 from sys.tables a where a.name = 'tbl_login_lookup')
	begin
		CREATE TABLE [dbo].[tbl_login_lookup](
			[ID] [bigint] IDENTITY(1,1) NOT NULL,
			[BUSINESS_UNIT] [nvarchar](50) NOT NULL,
			[PARTNER] [nvarchar](50) NOT NULL,
			[LOOKUP_KEY] [nvarchar](100) NOT NULL,
			[LOGINID] [nvarchar](100) NOT NULL,
			[LOOKUP_TYPE] [nvarchar](50) NOT NULL
		) ON [PRIMARY]
	end
	GO 


