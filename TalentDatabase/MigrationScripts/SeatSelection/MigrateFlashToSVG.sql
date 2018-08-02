-- =============================================
-- Normalise the Cache values
-- =============================================
UPDATE [tbl_control_attribute] SET CONTROL_CODE = 'SeatSelection.ascx' WHERE ATTR_NAME = 'AvailabilityCacheing' AND CONTROL_CODE = 'Flash.ascx';
UPDATE [tbl_control_attribute] SET CONTROL_CODE = 'SeatSelection.ascx' WHERE ATTR_NAME = 'AvailabilityCacheTimeMinutes' AND CONTROL_CODE = 'Flash.ascx';
UPDATE [tbl_control_attribute] SET CONTROL_CODE = 'SeatSelection.ascx' WHERE ATTR_NAME = 'ProductDetailsCacheTimeMinutes' AND CONTROL_CODE = 'Flash.ascx';
UPDATE [tbl_control_attribute] SET CONTROL_CODE = 'SeatSelection.ascx' WHERE ATTR_NAME = 'StandAreaDescriptionsCacheing' AND CONTROL_CODE = 'Flash.ascx';
UPDATE [tbl_control_attribute] SET CONTROL_CODE = 'SeatSelection.ascx' WHERE ATTR_NAME = 'StandAreaDescriptionsCacheTimeMinutes' AND CONTROL_CODE = 'Flash.ascx';
UPDATE [tbl_control_attribute] SET CONTROL_CODE = 'SeatSelection.ascx' WHERE ATTR_NAME = 'ProductPricingDetailsCacheing' AND CONTROL_CODE = 'Flash.ascx';
UPDATE [tbl_control_attribute] SET CONTROL_CODE = 'SeatSelection.ascx' WHERE ATTR_NAME = 'ProductPricingDetailsCacheTimeMinutes' AND CONTROL_CODE = 'Flash.ascx';
DELETE FROM [tbl_control_attribute] WHERE CONTROL_CODE='Flash.ascx' AND ATTR_NAME='StadiumAvailabilityCacheing';
DELETE FROM [tbl_control_attribute] WHERE CONTROL_CODE='Flash.ascx' AND ATTR_NAME='StadiumAvailabilityCacheTimeMinutes';
DELETE FROM [tbl_control_attribute] WHERE CONTROL_CODE='Flash.ascx' AND ATTR_NAME='SeatNumberCacheing';
DELETE FROM [tbl_control_attribute] WHERE CONTROL_CODE='Flash.ascx' AND ATTR_NAME='SeatNumberCacheTimeMinutes';
DELETE FROM [tbl_control_attribute] WHERE CONTROL_CODE='Flash.ascx' AND ATTR_NAME='RestrictionCacheing';
DELETE FROM [tbl_control_attribute] WHERE CONTROL_CODE='Flash.ascx' AND ATTR_NAME='RestrictionCacheTimeMinutes';
DELETE FROM [tbl_control_attribute] WHERE CONTROL_CODE='ProductDetail.ascx' AND ATTR_NAME='Flash_requiredMajorVersion';
DELETE FROM [tbl_control_attribute] WHERE CONTROL_CODE='ProductDetail.ascx' AND ATTR_NAME='Flash_requiredMinorVersion';
DELETE FROM [tbl_control_attribute] WHERE CONTROL_CODE='ProductDetail.ascx' AND ATTR_NAME='Flash_requiredRevision';
UPDATE [tbl_control_attribute] SET [CONTROL_CODE] = 'SeatSelection.ascx' WHERE [CONTROL_CODE] = 'Flash.ascx' AND [ATTR_NAME] IN ('AvailabilityCacheing','AvailabilityCacheTimeMinutes');
DELETE FROM [tbl_control_attribute] WHERE [CONTROL_CODE] = 'Flash.ascx' AND [ATTR_NAME] = 'QueryStringParameter1';
DELETE FROM [tbl_control_attribute] WHERE [CONTROL_CODE] = 'Flash.ascx' AND [ATTR_NAME] = 'ProductPricingDetailsCacheViaProductListDependancy';


-- ===========================================================
-- Insert new records in new tables based on existing stadiums
-- ===========================================================
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_flash_stadiums'))
BEGIN
	DECLARE @FlashStadiumName AS NVARCHAR(50);
	DECLARE @TicketingStadium AS NVARCHAR(50);
	SET @TicketingStadium = (SELECT [VALUE] FROM [tbl_ecommerce_module_defaults_bu] WHERE [DEFAULT_NAME] = 'TICKETING_STADIUM' AND [BUSINESS_UNIT] = 'UNITEDKINGDOM');

	IF @TicketingStadium LIKE '%,%'
	BEGIN
		-- There is more than 1 ticketing stadium
		DECLARE @ThisFlashStadiumName AS NVARCHAR(50);
		DECLARE @ThisTicketingStadium AS NVARCHAR(5);
		DECLARE @position INT;
		DECLARE @length INT;
		SET @position = 0;
		SET @length = 0;

		WHILE CHARINDEX(',', @TicketingStadium, @position+1) > 0
		BEGIN
			SET @length = CHARINDEX(',', @TicketingStadium, @position+1) - @position
			SET @ThisTicketingStadium = SUBSTRING(@TicketingStadium, @position, @length)
		
			-- Check if the ticketing stadium has a flash movie
			IF EXISTS (SELECT * FROM [tbl_flash_stadiums] WHERE [STADIUM_CODE] = @ThisTicketingStadium)
			BEGIN
				SET @ThisFlashStadiumName = (SELECT [STADIUM_NAME] FROM [tbl_flash_stadiums] WHERE [STADIUM_CODE] = @ThisTicketingStadium);

				INSERT INTO [dbo].[tbl_stadiums]
						([STADIUM_CODE],[STADIUM_NAME],[STADIUM_WIDTH],[STADIUM_HEIGHT],[PITCH_POSITION],[QUICK_BUY],[STADIUM_WIDTH_RESIZED],[STADIUM_HEIGHT_RESIZED],[SEATING_AREA_WIDTH],[SEATING_AREA_HEIGHT],[VIEW_FROM_AREA_ENABLED],[ORPHAN_SEAT_VALIDATION],[ORPHAN_SEAT_BENCHMARK])
				VALUES (@ThisTicketingStadium,@ThisFlashStadiumName,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,0,0)

				INSERT INTO [dbo].[tbl_stadium_area_colours]
					   ([BUSINESS_UNIT],[STADIUM_CODE],[KEY_NAME],[SEQUENCE],[MIN],[MAX],[AREA_CATEGORY],[COLOUR],[FF_IN_AREA_COLOUR],[TEXT],[CSS_CLASS])
				SELECT [BUSINESS_UNIT],@ThisTicketingStadium,[KEY_NAME],[SEQUENCE],[MIN],[MAX],[AREA_CATEGORY],[COLOUR],[FF_IN_AREA_COLOUR],[TEXT],[CSS_CLASS]
				FROM [dbo].[tbl_stadium_area_colours]
				WHERE [STADIUM_CODE] = '*ALL'

				INSERT INTO [dbo].[tbl_stadium_seat_colours]
					   ([BUSINESS_UNIT],[STADIUM_CODE],[SEQUENCE],[SEAT_TYPE],[OUTLINE_COLOUR],[FILL_COLOUR],[TEXT],[CSS_CLASS],[DISPLAY_COLOUR])
				SELECT [BUSINESS_UNIT],@ThisTicketingStadium,[SEQUENCE],[SEAT_TYPE],[OUTLINE_COLOUR],[FILL_COLOUR],[TEXT],[CSS_CLASS],[DISPLAY_COLOUR]
				FROM [dbo].[tbl_stadium_seat_colours]
				WHERE [STADIUM_CODE] = '*ALL'
			END

			SET @position = CHARINDEX(',', @TicketingStadium, @position+@length) +1
		END
	END
	ELSE
		BEGIN
			-- There is only 1 ticketing stadium/flash movie
			SET @FlashStadiumName = (SELECT ISNULL([VALUE],'') FROM [tbl_ecommerce_module_defaults_bu] WHERE [DEFAULT_NAME] = 'FLASH_STADIUM_NAME');

			IF LEN(LTRIM(@FlashStadiumName))>0
			BEGIN
				INSERT INTO [dbo].[tbl_stadiums]
					   ([STADIUM_CODE],[STADIUM_NAME],[STADIUM_WIDTH],[STADIUM_HEIGHT],[PITCH_POSITION],[QUICK_BUY],[STADIUM_WIDTH_RESIZED],[STADIUM_HEIGHT_RESIZED],[SEATING_AREA_WIDTH],[SEATING_AREA_HEIGHT],[VIEW_FROM_AREA_ENABLED],[ORPHAN_SEAT_VALIDATION],[ORPHAN_SEAT_BENCHMARK])
				VALUES (@TicketingStadium,@FlashStadiumName,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,0,0)

				INSERT INTO [dbo].[tbl_stadium_area_colours]
					   ([BUSINESS_UNIT],[STADIUM_CODE],[KEY_NAME],[SEQUENCE],[MIN],[MAX],[AREA_CATEGORY],[COLOUR],[FF_IN_AREA_COLOUR],[TEXT],[CSS_CLASS])
				SELECT [BUSINESS_UNIT],@TicketingStadium,[KEY_NAME],[SEQUENCE],[MIN],[MAX],[AREA_CATEGORY],[COLOUR],[FF_IN_AREA_COLOUR],[TEXT],[CSS_CLASS]
				FROM [dbo].[tbl_stadium_area_colours]
				WHERE [STADIUM_CODE] = '*ALL'

				INSERT INTO [dbo].[tbl_stadium_seat_colours]
					   ([BUSINESS_UNIT],[STADIUM_CODE],[SEQUENCE],[SEAT_TYPE],[OUTLINE_COLOUR],[FILL_COLOUR],[TEXT],[CSS_CLASS],[DISPLAY_COLOUR])
				SELECT [BUSINESS_UNIT],@TicketingStadium,[SEQUENCE],[SEAT_TYPE],[OUTLINE_COLOUR],[FILL_COLOUR],[TEXT],[CSS_CLASS],[DISPLAY_COLOUR]
				FROM [dbo].[tbl_stadium_seat_colours]
				WHERE [STADIUM_CODE] = '*ALL'
			END 

		END
END


-- ===========================================================
-- Remove redundant settings
-- ===========================================================
DELETE FROM [tbl_ecommerce_module_defaults_bu] WHERE DEFAULT_NAME='FLASH_STADIUM_NAME';
DELETE FROM [tbl_ecommerce_module_defaults_bu] WHERE DEFAULT_NAME='FLASH_VERSION';
DELETE FROM [tbl_ecommerce_module_defaults_bu] WHERE DEFAULT_NAME='FLASH_PATH';
DELETE FROM [tbl_ecommerce_module_defaults_bu] WHERE DEFAULT_NAME='FLASH_URL';
DELETE FROM [tbl_ecommerce_module_defaults_bu] WHERE DEFAULT_NAME='USE_FLASH_QTY_SELECT'


-- ===========================================================
-- Remove redundant tables
-- ===========================================================
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_flash_stadiums'))
BEGIN
	DROP TABLE [dbo].[tbl_flash_stadiums]
END
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_flash_stadium_settings'))
BEGIN
	DROP TABLE [dbo].[tbl_flash_stadium_settings]
END
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_flash_stadium_settings_work'))
BEGIN
	DROP TABLE [dbo].[tbl_flash_stadium_settings_work]
END
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_flash_availability_bands'))
BEGIN
	DROP TABLE [dbo].[tbl_flash_availability_bands]
END
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'tbl_flash_availability_bands_work'))
BEGIN
	DROP TABLE [dbo].[tbl_flash_availability_bands_work]
END