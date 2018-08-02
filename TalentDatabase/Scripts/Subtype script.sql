USE [test_TalentEBusinessDBClientName]

-- CONFIGURATION

declare @BUSINESS_UNIT as nvarchar(30)
set @BUSINESS_UNIT = 'UNITEDKINGDOM'

declare @PAGETITLE as nvarchar(128)
set @PAGETITLE = 'Memberships &ndash; Wolverhampton Wanderers FC' /* Title */

declare @PAGEHEADER as nvarchar(128)
set @PAGEHEADER = 'Memberships' /* Header + Back-End Definition */

declare @PRODUCTTYPE as nvarchar(30)
set @PRODUCTTYPE = 'Membership' /* Home, Away, Season, Membership, Event or Travel */

declare @PRODUCTSUBTYPE as nvarchar(30)
set @PRODUCTSUBTYPE = 'MEMB'

declare @PAGE_TEMPLATE as nvarchar(30)
set @PAGE_TEMPLATE = '1Column'

-- tbl_control_text_lang

declare @NoProductsFoundMessageText as nvarchar(max)
set @NoProductsFoundMessageText = 'There are no memberships currently on sale. Please try back later.'

-- START DELETING

DELETE FROM [tbl_page]
     WHERE [BUSINESS_UNIT] = @BUSINESS_UNIT AND [PAGE_CODE] = 'Product' + @PRODUCTTYPE + '.aspx?ProductSubType=' + @PRODUCTSUBTYPE
     
DELETE FROM [tbl_page_lang]
     WHERE [BUSINESS_UNIT] = @BUSINESS_UNIT AND [PAGE_CODE] = 'Product' + @PRODUCTTYPE + '.aspx?ProductSubType=' + @PRODUCTSUBTYPE
     
DELETE FROM [tbl_page_html]
     WHERE [BUSINESS_UNIT] = @BUSINESS_UNIT AND [PAGE_CODE] = 'Product' + @PRODUCTTYPE + '.aspx?ProductSubType=' + @PRODUCTSUBTYPE
     
DELETE FROM [tbl_template_page]
     WHERE [BUSINESS_UNIT] = @BUSINESS_UNIT AND [PAGE_NAME] = 'Product' + @PRODUCTTYPE + '.aspx?ProductSubType=' + @PRODUCTSUBTYPE
     
DELETE FROM [tbl_control_text_lang]
     WHERE [BUSINESS_UNIT] = @BUSINESS_UNIT AND [PAGE_CODE] = 'Product' + @PRODUCTTYPE + '.aspx?ProductSubType=' + @PRODUCTSUBTYPE

-- START INSERTING 

INSERT INTO [tbl_page]
           ([BUSINESS_UNIT]
           ,[PARTNER_CODE]
           ,[PAGE_CODE]
           ,[DESCRIPTION]
           ,[PAGE_QUERYSTRING]
           ,[USE_SECURE_URL]
           ,[HTML_IN_USE]
           ,[PAGE_TYPE]
           ,[SHOW_PAGE_HEADER]
           ,[BCT_URL]
           ,[BCT_PARENT]
           ,[FORCE_LOGIN]
           ,[IN_USE]
           ,[CSS_PRINT]
           ,[HIDE_IN_MAINTENANCE]
           ,[ALLOW_GENERIC_SALES]
           ,[RESTRICTING_ALERT_NAME]
           ,[BODY_CSS_CLASS])
     VALUES
           (@BUSINESS_UNIT
           ,'*ALL'
           ,'Product' + @PRODUCTTYPE + '.aspx?ProductSubType=' + @PRODUCTSUBTYPE
           ,@PAGEHEADER
           ,''
           ,'FALSE'
           ,'TRUE'
           ,'STANDARD'
           ,'FALSE'
           ,'../../Product' + @PRODUCTTYPE + '.aspx?ProductSubType=' + @PRODUCTSUBTYPE
           ,''
           ,'FALSE'
           ,'TRUE'
           ,''
           ,'FALSE'
           ,''
           ,''
           ,@PRODUCTTYPE + ' ' + @PRODUCTSUBTYPE)
   
INSERT INTO [tbl_page_lang]
           ([BUSINESS_UNIT]
           ,[PARTNER_CODE]
           ,[PAGE_CODE]
           ,[LANGUAGE_CODE]
           ,[TITLE]
           ,[META_KEY]
           ,[META_DESC]
           ,[PAGE_HEADER])
     VALUES
           (@BUSINESS_UNIT
           ,'*ALL'
           ,'Product' + @PRODUCTTYPE + '.aspx?ProductSubType=' + @PRODUCTSUBTYPE
           ,'ENG'
           ,@PAGETITLE
           ,NULL
           ,NULL
           ,@PAGEHEADER)
           
INSERT INTO [tbl_page_html]
           ([BUSINESS_UNIT]
           ,[PARTNER]
           ,[PAGE_CODE]
           ,[PAGE_QUERYSTRING]
           ,[SECTION]
           ,[SEQUENCE]
           ,[HTML_1]
           ,[HTML_2]
           ,[HTML_3]
           ,[HTML_LOCATION])
     VALUES
           (@BUSINESS_UNIT
           ,'*ALL'
           ,'Product' + @PRODUCTTYPE + '.aspx?ProductSubType=' + @PRODUCTSUBTYPE
           ,NULL
           ,'2'
           ,'1'
           ,NULL
           ,NULL
           ,NULL
           ,'UsrDef.' + @PRODUCTTYPE + '.' + @PRODUCTSUBTYPE + '.centre.html')

INSERT INTO [tbl_template_page]
           ([BUSINESS_UNIT]
           ,[PARTNER]
           ,[PAGE_NAME]
           ,[TEMPLATE_NAME])
     VALUES
           (@BUSINESS_UNIT
           ,'*ALL'
           ,'Product' + @PRODUCTTYPE + '.aspx?ProductSubType=' + @PRODUCTSUBTYPE
           ,@PAGE_TEMPLATE)
           
INSERT INTO [tbl_control_text_lang]
           ([LANGUAGE_CODE]
           ,[BUSINESS_UNIT]
           ,[PARTNER_CODE]
           ,[PAGE_CODE]
           ,[CONTROL_CODE]
           ,[TEXT_CODE]
           ,[CONTROL_CONTENT])
     VALUES
           ('ENG'
           ,@BUSINESS_UNIT
           ,'*ALL'
           ,'Product' + @PRODUCTTYPE + '.aspx?ProductSubType=' + @PRODUCTSUBTYPE
           ,'ProductDetail.ascx'
           ,'NoProductsFoundMessageText'
           ,@NoProductsFoundMessageText)