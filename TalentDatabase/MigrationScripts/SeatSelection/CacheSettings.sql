-- =================================================================================
-- Remove redundant cache attributes (these aren't used in the system)
-- =================================================================================
DELETE FROM [tbl_control_attribute] WHERE CONTROL_CODE = 'SeatSelection.ascx' AND ATTR_NAME = 'AvailabilityCacheing'
DELETE FROM [tbl_control_attribute] WHERE CONTROL_CODE = 'SeatSelection.ascx' AND ATTR_NAME = 'AvailabilityCacheTimeMinutes'

-- =================================================================================
-- Move cache attributes from tbl_control_attribute to tbl_bu_module_database
-- The stadium availability and product details need to be centralised
-- =================================================================================
IF (EXISTS (SELECT TOP(1) ATTR_VALUE FROM [tbl_control_attribute] WHERE ATTR_NAME = 'StadiumAvailabilityCacheing'))
BEGIN
	UPDATE [tbl_bu_module_database] 
	SET CACHE_ENABLED = (
		SELECT TOP(1) ATTR_VALUE 
		FROM [tbl_control_attribute] 
		WHERE ATTR_NAME = 'StadiumAvailabilityCacheing') 
	WHERE MODULE = 'ProductStadiumAvailability'

	DELETE FROM [tbl_control_attribute] WHERE CONTROL_CODE = 'SeatSelection.ascx' AND ATTR_NAME = 'StadiumAvailabilityCacheing'
END

IF (EXISTS (SELECT TOP(1) CAST(ATTR_VALUE AS INT) * 60 FROM [tbl_control_attribute] WHERE ATTR_NAME = 'StadiumAvailabilityCacheTimeMinutes'))
BEGIN
	UPDATE [tbl_bu_module_database] 
	SET CACHE_SECONDS = (
		SELECT TOP(1) CAST(ATTR_VALUE AS INT) * 60 
		FROM [tbl_control_attribute] 
		WHERE ATTR_NAME = 'StadiumAvailabilityCacheTimeMinutes') 
	WHERE MODULE = 'ProductStadiumAvailability'
	
	DELETE FROM [tbl_control_attribute] WHERE CONTROL_CODE = 'SeatSelection.ascx' AND ATTR_NAME = 'StadiumAvailabilityCacheTimeMinutes'
END

IF (EXISTS (SELECT TOP(1) ATTR_VALUE FROM [tbl_control_attribute] WHERE ATTR_NAME = 'ProductDetailsCaching'))
BEGIN
	UPDATE [tbl_bu_module_database] 
	SET CACHE_ENABLED = (
		SELECT TOP(1) ATTR_VALUE 
		FROM [tbl_control_attribute] 
		WHERE ATTR_NAME = 'ProductDetailsCaching') 
	WHERE MODULE = 'ProductDetails'

	DELETE FROM [tbl_control_attribute] WHERE ATTR_NAME = 'ProductDetailsCaching'
END

IF (EXISTS (SELECT TOP(1) CAST(ATTR_VALUE AS INT) * 60 FROM [tbl_control_attribute] WHERE ATTR_NAME = 'ProductDetailsCacheTimeMinutes'))
BEGIN
	UPDATE [tbl_bu_module_database] 
	SET CACHE_SECONDS = (
		SELECT TOP(1) CAST(ATTR_VALUE AS INT) * 60 
		FROM [tbl_control_attribute] 
		WHERE ATTR_NAME = 'ProductDetailsCacheTimeMinutes') 
	WHERE MODULE = 'ProductDetails'
	
	DELETE FROM [tbl_control_attribute] WHERE ATTR_NAME = 'ProductDetailsCacheTimeMinutes'
END