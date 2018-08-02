

--To delete a business unit follow the steps
--1.       Open SQL Studio
--2.       Open new query
--3.       Choose your database
--4.       Copy and paste the below script
--5.       Run it
--6.       Then copy the result
--7.        Result will be your script to delete business unit


SELECT 'DECLARE @BUSINESSUNIT VARCHAR(50)' AS SCRIPT_TO_DELETE
UNION ALL
SELECT 'SET @BUSINESSUNIT = ''BUSINESSUNIT_TODELETE'' -- GIVE THE BUSINESS UNIT NAME HERE' AS SCRIPT_TO_DELETE
UNION ALL
SELECT 'DELETE ' + sys.objects.name + ' WHERE ' + sys.all_columns.name + ' = @BUSINESSUNIT' AS SCRIPT_TO_DELETE  FROM sys.all_columns 
join sys.objects on sys.objects.object_id = sys.all_columns.object_id 
WHERE sys.all_columns.name in ('BUSINESS_UNIT','BUSINESSUNIT')
