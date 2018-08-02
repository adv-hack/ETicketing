-- Change the alerts HTML to use Alerts.ascx embedded into the page rather than UserAlerts.ascx and Alerts.aspx
-- Designed to be run once following an upgrade

DECLARE @BusinessUnit as NVARCHAR(20);

-- ===============================================================================================================
-- BOX OFFICE
-- ===============================================================================================================
SET @BusinessUnit = 'BOXOFFICE'

-- Header Item
DELETE FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'AlertHeaderString' AND [BUSINESS_UNIT] = @BusinessUnit
INSERT INTO [tbl_control_text_lang]
		   ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
	 VALUES
		   ('ENG',@BusinessUnit,'*ALL','*ALL','Alerts.ascx','AlertHeaderString',
		   '<li class="dropdown menu  ebiz-alert-menu">
				<a href="#"><i class="fa fa-bell fa-lg"></i> <span class="alert badge"><<NO_OF_ALERTS>></span></a>
				<ul class="menu">',
			0)

-- Repeated Item
DELETE FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'AlertItemString' AND [BUSINESS_UNIT] = @BusinessUnit
INSERT INTO [tbl_control_text_lang]
		   ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
	 VALUES
		   ('ENG',@BusinessUnit,'*ALL','*ALL','Alerts.ascx','AlertItemString',
		   '<li>
			  <a href="<<ACTION_DETAIL>>" target="<<ALERT_TARGET>>"><img src="<<ALERT_IMAGE_SRC>>" alt="<<ALERT_SUBJECT>>"></a>
			  <span class="ebiz-alert-copy">
				<a href="#">
				  <span class="ebiz-alert-subject"><<ALERT_SUBJECT>></span><br>
				  <span class="ebiz-alert-description"><<ALERT_DESCRIPTION>></span>
				</a>
			  </span>
			  <<REMOVE_ALERT>>
			</li>',
			0)

-- Repeated Item (with no image)
DELETE FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'AlertItemStringNoImage' AND [BUSINESS_UNIT] = @BusinessUnit
INSERT INTO [tbl_control_text_lang]
		   ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
	 VALUES
		   ('ENG',@BusinessUnit,'*ALL','*ALL','Alerts.ascx','AlertItemStringNoImage',
		   '<li>
			  <a href="<<ACTION_DETAIL>>" target="<<ALERT_TARGET>>"><img src="<<ALERT_IMAGE_SRC>>" alt="<<ALERT_SUBJECT>>"></a>
			  <span class="ebiz-alert-copy">
				<a href="#">
				  <span class="ebiz-alert-subject"><<ALERT_SUBJECT>></span><br>
				  <span class="ebiz-alert-description"><<ALERT_DESCRIPTION>></span>
				</a>
			  </span>
			  <<REMOVE_ALERT>>
			</li>',
			0)

-- Remove link button text value
DELETE FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'AlertItemRemoveTextString' AND [BUSINESS_UNIT] = @BusinessUnit
INSERT INTO [tbl_control_text_lang]
		   ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
	 VALUES
		   ('ENG',@BusinessUnit,'*ALL','*ALL','Alerts.ascx','AlertItemRemoveTextString',
		   '<i class="fa fa-times"></i>',
			0)

-- Footer Item
DELETE FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'AlertFooterString' AND [BUSINESS_UNIT] = @BusinessUnit
INSERT INTO [tbl_control_text_lang]
		   ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
	 VALUES
		   ('ENG',@BusinessUnit,'*ALL','*ALL','Alerts.ascx','AlertFooterString',
		   '</ul>
			</li>',
			0)


-- ===============================================================================================================
-- Public Web Sales
-- ===============================================================================================================
SET @BusinessUnit = 'UNITEDKINGDOM'

-- Header Item
DELETE FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'AlertHeaderString' AND [BUSINESS_UNIT] = @BusinessUnit
INSERT INTO [tbl_control_text_lang]
		   ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
	 VALUES
		   ('ENG',@BusinessUnit,'*ALL','*ALL','Alerts.ascx','AlertHeaderString',
		   '<div class="ebiz-alert-wrapper">
              <a href="#" data-open="ebiz-alert-reveal"><i class=" fa fa-bell fa-lg"></i> <span class="ebiz-alert-count"><<NO_OF_ALERTS>></span></a>
              <div id="ebiz-alert-reveal" class="reveal" data-reveal>
                  <ul class="ebiz-alert-list">',
			0)

-- Repeated Item
DELETE FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'AlertItemString' AND [BUSINESS_UNIT] = @BusinessUnit
INSERT INTO [tbl_control_text_lang]
		   ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
	 VALUES
		   ('ENG',@BusinessUnit,'*ALL','*ALL','Alerts.ascx','AlertItemString',
		   '<li class="ebiz-alert-item">
                <a href="<<ACTION_DETAIL>>" target="<<ALERT_TARGET>>"><img src="<<ALERT_IMAGE_SRC>>" alt="<<ALERT_SUBJECT>>"></a>
                <span class="ebiz-alert-copy">
                    <a href="<<ACTION_DETAIL>>" target="<<ALERT_TARGET>>">
                        <span class="ebiz-alert-subject"><<ALERT_SUBJECT>></span><br>
                        <span class="ebiz-alert-description"><<ALERT_DESCRIPTION>></span>
                    </a>
                </span>
                <<REMOVE_ALERT>>
            </li>',
			0)

-- Repeated Item (with no image)
DELETE FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'AlertItemStringNoImage' AND [BUSINESS_UNIT] = @BusinessUnit
INSERT INTO [tbl_control_text_lang]
		   ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
	 VALUES
		   ('ENG',@BusinessUnit,'*ALL','*ALL','Alerts.ascx','AlertItemStringNoImage',
		   '<li class="ebiz-alert-item ebiz-alert-item-no-image">
                <a href="<<ACTION_DETAIL>>" target="<<ALERT_TARGET>>">
                    <span class="ebiz-alert-copy">
                        <span class="ebiz-alert-title"><<ALERT_SUBJECT>></span><br>
                        <span class="ebiz-alert-description"><<ALERT_DESCRIPTION>></span>
                    </span>
                </a>
                <<REMOVE_ALERT>>
            </li>',
			0)

-- Remove link button text value
DELETE FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'AlertItemRemoveTextString' AND [BUSINESS_UNIT] = @BusinessUnit
INSERT INTO [tbl_control_text_lang]
		   ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
	 VALUES
		   ('ENG',@BusinessUnit,'*ALL','*ALL','Alerts.ascx','AlertItemRemoveTextString',
		   '<i class="fa fa-times"></i>',
			0)

-- Footer Item
DELETE FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'AlertFooterString' AND [BUSINESS_UNIT] = @BusinessUnit
INSERT INTO [tbl_control_text_lang]
		   ([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT],[HIDE_IN_MAINTENANCE])
	 VALUES
		   ('ENG',@BusinessUnit,'*ALL','*ALL','Alerts.ascx','AlertFooterString',
		   '</ul>
              </div>
            </div>',
			0)



-- ===============================================================================================================
-- Clean up script
-- ===============================================================================================================
DELETE FROM [tbl_control_text_lang] WHERE [TEXT_CODE] = 'ShadowBoxCloseJS' AND [CONTROL_CODE] = 'Alerts.ascx'
DELETE FROM [tbl_control_text_lang] WHERE [CONTROL_CODE] = 'UserAlerts.ascx'
DELETE FROM [tbl_page] WHERE [PAGE_CODE] = 'Alerts.aspx'
DELETE FROM [tbl_page_lang] WHERE [PAGE_CODE] = 'Alerts.aspx'
DELETE FROM [tbl_page_text_lang] WHERE [PAGE_CODE] = 'Alerts.aspx'
DELETE FROM [tbl_page_html] WHERE [PAGE_CODE] = 'Alerts.aspx'
DELETE FROM [tbl_template_page] WHERE [PAGE_NAME] = 'Alerts.aspx'