-- Remove existing index if it exists
IF EXISTS (SELECT * FROM sys.indexes WHERE NAME ='MonitorNameAndBU' AND OBJECT_ID = OBJECT_ID('tbl_offline_processing'))
BEGIN
       DROP INDEX MonitorNameAndBU on tbl_offline_processing
 
       -- Change the data types on the columns to help with performance and indexing
       ALTER TABLE tbl_offline_processing ALTER COLUMN BUSINESS_UNIT varchar(50)
       ALTER TABLE tbl_offline_processing ALTER COLUMN PARTNER varchar(50)
       ALTER TABLE tbl_offline_processing ALTER COLUMN STATUS varchar(50)
       ALTER TABLE tbl_offline_processing ALTER COLUMN SERVER_NAME varchar(50)
       ALTER TABLE tbl_offline_processing ALTER COLUMN MONITOR_NAME varchar(50)
       ALTER TABLE tbl_offline_processing ALTER COLUMN REQUEST_TYPE varchar(512)
       -- Add the primary key constraint
       ALTER TABLE [dbo].tbl_offline_processing ADD  CONSTRAINT [PK_tbl_offline_processing] PRIMARY KEY CLUSTERED 
       ([ID] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
 
       CREATE NONCLUSTERED INDEX [idx_BU_Monitor_Status] ON [dbo].[tbl_offline_processing]
       (
              [BUSINESS_UNIT] ASC,
              [STATUS] ASC,
              [MONITOR_NAME] ASC
       )
       INCLUDE (     [TIMESTAMP_ADDED],
              [TIMESTAMP_LAST_UPDATED]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
 
END
 
GO

