﻿/*
	Example code for checking for duplicates:

	SELECT  BUSINESS_UNIT, [PARTNER], DEFAULT_NAME, VALUE
	FROM tbl_ecommerce_module_defaults_bu
	GROUP BY BUSINESS_UNIT, [PARTNER], DEFAULT_NAME, VALUE
	HAVING COUNT(*) > 1

*/
BEGIN
	WITH duplicateModuleDefaults AS (
	  SELECT BUSINESS_UNIT, [PARTNER], DEFAULT_NAME, VALUE,
		 row_number() OVER(PARTITION BY BUSINESS_UNIT, [PARTNER], DEFAULT_NAME, VALUE ORDER BY BUSINESS_UNIT, [PARTNER], DEFAULT_NAME, VALUE) AS [rowNumber]
	  FROM tbl_ecommerce_module_defaults_bu
	)
	DELETE duplicateModuleDefaults WHERE [rowNumber] > 1
END
GO
BEGIN
	WITH duplicateControlAttributes AS (
	  SELECT BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, CONTROL_CODE, ATTR_NAME, ATTR_VALUE, [DESCRIPTION],
		 row_number() OVER(PARTITION BY BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, CONTROL_CODE, ATTR_NAME, ATTR_VALUE, [DESCRIPTION] ORDER BY BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, CONTROL_CODE, ATTR_NAME, ATTR_VALUE, [DESCRIPTION]) AS [rowNumber]
	  FROM tbl_control_attribute
	)
	DELETE duplicateControlAttributes WHERE [rowNumber] > 1
END
GO
BEGIN
	WITH duplicateControlText AS (
	  SELECT LANGUAGE_CODE, BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, CONTROL_CODE, TEXT_CODE, CONTROL_CONTENT,
		 row_number() OVER(PARTITION BY LANGUAGE_CODE, BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, CONTROL_CODE, TEXT_CODE, CAST(CONTROL_CONTENT AS NVARCHAR(500)) ORDER BY LANGUAGE_CODE, BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, CONTROL_CODE, TEXT_CODE, CAST(CONTROL_CONTENT AS NVARCHAR(500))) AS [rowNumber]
	  FROM tbl_control_text_lang
	)
	DELETE duplicateControlText WHERE [rowNumber] > 1
END
GO
BEGIN
	WITH duplicatePageText AS (
	  SELECT BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, TEXT_CODE, LANGUAGE_CODE, TEXT_CONTENT,
		 row_number() OVER(PARTITION BY BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, TEXT_CODE, LANGUAGE_CODE, CAST(TEXT_CONTENT AS NVARCHAR(500)) ORDER BY BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, TEXT_CODE, LANGUAGE_CODE, CAST(TEXT_CONTENT AS NVARCHAR(500))) AS [rowNumber]
	  FROM tbl_page_text_lang
	)
	DELETE duplicatePageText WHERE [rowNumber] > 1
END
GO
BEGIN
	WITH duplicatePageAttribute AS (
	  SELECT BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, ATTR_NAME, ATTR_VALUE, [DESCRIPTION],
		 row_number() OVER(PARTITION BY BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, ATTR_NAME, ATTR_VALUE, [DESCRIPTION] ORDER BY BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, ATTR_NAME, ATTR_VALUE, [DESCRIPTION]) AS [rowNumber]
	  FROM tbl_page_attribute
	)
	DELETE duplicatePageAttribute WHERE [rowNumber] > 1
END
GO