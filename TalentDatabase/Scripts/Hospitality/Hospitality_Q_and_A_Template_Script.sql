-- =======================================================================
-- Script for Hospitality Q&A templates (MBSTS-13341)---------------------
-- =======================================================================

--tbl_activity_templates
UPDATE [tbl_activity_templates] SET [TEMPLATE_TYPE] = 1 WHERE ([TEMPLATE_TYPE] in (SELECT [ID] from [tbl_activity_template_type] where [NAME]='Product/Transactional'))
UPDATE [tbl_activity_templates] SET [TEMPLATE_TYPE] = 2 WHERE ([TEMPLATE_TYPE] in (SELECT [ID] from [tbl_activity_template_type] where [NAME]='Customer Activity Notes'))
UPDATE [tbl_activity_templates] SET [TEMPLATE_TYPE] = 3 WHERE ([TEMPLATE_TYPE] in (SELECT [ID] from [tbl_activity_template_type] where [NAME]='Complimentary'))

--tbl_activity_template_type
UPDATE [tbl_activity_template_type] SET [TEMPLATE_TYPE_ID] = 1 WHERE [NAME] = 'Product/Transactional'
UPDATE [tbl_activity_template_type] SET [TEMPLATE_TYPE_ID] = 2 WHERE [NAME] = 'Customer Activity Notes'
UPDATE [tbl_activity_template_type] SET [TEMPLATE_TYPE_ID] = 3 WHERE [NAME] = 'Complimentary'
UPDATE [tbl_activity_template_type] SET [TEMPLATE_TYPE_ID] = 4 WHERE [NAME] = 'Corporate Hospitality'