-- =======================================================================
-- Script for 2017R4 Hospitality Email Changes (MBSTS-13318)--------------
-- =======================================================================

--tbl_email_templates
UPDATE [tbl_email_templates] SET [MASTER] = 1 WHERE [ACTIVE] = 1
UPDATE [tbl_email_templates] SET [MASTER] = 0 WHERE [ACTIVE] = 0
