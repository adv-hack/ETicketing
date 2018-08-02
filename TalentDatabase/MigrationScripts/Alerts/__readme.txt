Ensure all defaults are set correctly on tbl_ecommerce_module_defaults.
•	ALERTS_REFRESH_ATTRIBUTES_AT_LOGIN          = True
•	ALERTS_GENERATE_AT_LOGIN                                 = True
•	ALERTS_CC_EXPIRY_WARN_PERIOD                       = 30
•	ALERTS_ENABLED                                                            = True


Ensure all alert database tables are correct and present.  (Warning: The sql files will delete exiting instances of the tables and any data contained in the tables)
•	Create_tbl_alert_definition.sql
•	Create_tbl_attribute_definition.sql
•	Create_tbl_alert_criteria.sql
•	Create_tbl_alert.sql


Ensure all alert database tables are correctly populated.
•	Populate_tbl_alert_definition.sql
•	Populate_tbl_attribute_definition.sql
•	Populate_tbl_alert_criteria.sql


Ensure all stored procedures are correct and present.
•	usp_alert_delAndInsAttributeDefinition.proc.sql
•	usp_Alert_DelUserAttribute.sql
•	usp_Alert_InsOrUpdAlert.sql
•	usp_alert_insOrUpdAlertDefAndCritera.proc.sql
•	usp_Alert_InsOrUpdUserAttribute.sql
•	usp_Alert_SelForGenerateUserAlerts.sql



Trouble Shooting

Issue #1
When accessing AlertsDetail.aspx page and receive server error, as follows:
Server Error in '/Maintenance' Application.
Runtime Error 
Description: An application error occurred on the server. The current custom error settings for this application prevent the details of the application error from being viewed.
Solution
Ensure that the Maintenance application in IIS has IIS Authentication for ASP.NET Impersonation disabled.


Issue #2
Alert definition updates are not being saved.  
Solution
Ensure that the ‘liveUpdateDatabase0x’ value(s) are correctly populated in the web.config for the Maintenance application


Issue #3
When clicking on ‘Image Path’ button in AlertDetail.aspx and receive server error as follows:
Server Error in '/TalentMaintenance' Application.
Could not find a part of the path 'D:\TalentEBusinessSuiteAssets\Leeds\Test\Images\Alerts\'.
Solution
Ensure that physical path exists at 'D:\TalentEBusinessSuiteAssets\Leeds\Test\Images\Alerts\' plus the default image files.


Issue #4
Browse image path button missing text.
Solution
Ensure tbl_control_text_lang record exists for: (AlertsEdit.ascx, BrowseButton, Browse)


Issue #5
BrowseImages.aspx page missing button text etc.
Solution
Ensure tbl_page_text_lang records exists for: (BrowseImages.aspx, PageHeader/NoImagesText/Instructions/Cancel/ViewImage/UseImage)


Issue #6
Incorrect layout in Alerts winow.
Solution
Ensure the following CSS is present in relevant style sheet.


/* =============== */
/* === Alerts  === */
/* =============== */
#user-alerts{margin:10px 0 0;}
#user-alerts a{background:#ff5e99 url("img/speech-bubble.png") no-repeat 5px 5px;font-weight:700;text-decoration:none;padding:5px 5px 5px 26px;}
#user-alerts-page ul{list-style:none;margin:0;padding:0;}
#user-alerts-page li{border:solid 1px #fff;overflow:hidden;width:auto;margin:0 0 10px;padding:10px;}
#user-alerts-page img{float:left;height:140px;overflow:hidden;width:140px;}
#user-alerts-page .more{display:block;float:left;font:700 1.5em 'Trebuchet MS','Lucida Sans Unicode','Lucida Grande','Lucida Sans',Arial,sans-serif;width:270px;margin:0 0 0 20px;}
#user-alerts-page .desc{display:block;float:left;width:270px;margin:0 0 0 20px;}
#user-alerts-page .remove-this-alert{float:left;margin:10px 0 0 20px;}
#user-alerts-page cc_ending_in{background:#ff5e99;color:#fff;}




Issue #7
.Net message when updating existing Alert as follows: 'Server Error in '/Maintenance' Application. A potentially dangerous Request.Form value was detected from the client (ctl00$Content1$AlertEdit1$txtDescription="... ending <<<cc_ending_in>>> is ...").
Solution 
1.  Ensure AlertDetail.aspx contains 'validateRequest="false"'
2.  Ensure web.config contains <httpRuntime requestValidationMode="2.0" />



Issue #8
Refreshing attributes not populating tbl_attribute_definition
Solution
Ensure that web.config setting for liveUpdateDtabase01 is populated


