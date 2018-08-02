-- Sequencing update over tbl_group_product.
-- The old maintenance sequencing didn't work correctly and you had to manually alter the sequencing, 
-- this update statement removes the non-numeric sequence value from any adhoc groups.

SELECT * FROM tbl_group_product WHERE ISNUMERIC(SEQUENCE) = 0 AND GROUP_ADHOC = 1
UPDATE tbl_group_product SET SEQUENCE = '' WHERE ISNUMERIC(SEQUENCE) = 0 AND GROUP_ADHOC = 1