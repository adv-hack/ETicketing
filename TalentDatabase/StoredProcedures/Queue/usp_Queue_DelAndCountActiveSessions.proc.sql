IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[usp_Queue_DelAndCountActiveSessions]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[usp_Queue_DelAndCountActiveSessions]
GO

-- =============================================
-- Author:		Des Webster
-- Create date: 07/06/2012
-- Description:	Remove expired active sessions
-- =============================================
CREATE PROCEDURE [dbo].[usp_Queue_DelAndCountActiveSessions]
(
	-- Add the parameters for the stored procedure here
	@pa_DeletePeriod int = null, 
	@pa_BusinessUnit nvarchar(50) = null
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Create the inactivity period
	DECLARE @DeleteTime DATETIME;
	DECLARE @AgentDeleteTime DATETIME;
	SET @DeleteTime = DATEADD(MINUTE, -@pa_DeletePeriod, CURRENT_TIMESTAMP);
	SET @AgentDeleteTime = DATEADD(MINUTE, -120, CURRENT_TIMESTAMP)
	DECLARE @DeletedAgentSessionTable Table (SESSIONID nvarchar(max), AGENT_NAME nvarchar(25));
	
	IF @pa_BusinessUnit = '*ALL'
	
		BEGIN
			-- Delete the expired sessions
			DELETE FROM tbl_active_noise_sessions WHERE LAST_ACTIVITY < @DeleteTime AND IS_AGENT = 'False';
			
			-- Delete the expired sessions of agent as well as their details in tbl_agent
			DELETE FROM tbl_active_noise_sessions 
				OUTPUT DELETED.SESSIONID, DELETED.USERNAME INTO @DeletedAgentSessionTable
				WHERE LAST_ACTIVITY < @AgentDeleteTime AND IS_AGENT = 'True';
			DELETE tbl_agent FROM tbl_agent 
				INNER JOIN @DeletedAgentSessionTable DGST
					ON tbl_agent.SESSIONID = DGST.SESSIONID
						AND tbl_agent.AGENT_NAME = DGST.AGENT_NAME
			-- Delete agents who are not exists in session table
			DELETE FROM tbl_agent
				WHERE SESSIONID NOT IN (SELECT SESSIONID FROM tbl_active_noise_sessions WHERE IS_AGENT = 'True')	

			-- Retrieve the users online
			SELECT COUNT(*) AS ONLINE FROM tbl_active_noise_sessions WITH (NOLOCK) WHERE IS_AGENT = 'False';
		END
		
	ELSE 
	
		BEGIN
			-- Delete the expired sessions
			DELETE FROM tbl_active_noise_sessions WHERE LAST_ACTIVITY < @DeleteTime AND IS_AGENT = 'False' AND BUSINESS_UNIT = @pa_BusinessUnit; 
			
			-- Delete the expired sessions of agent as well as their details in tbl_agent			
			DELETE FROM tbl_active_noise_sessions 
				OUTPUT DELETED.SESSIONID, DELETED.USERNAME INTO @DeletedAgentSessionTable
					WHERE LAST_ACTIVITY < @AgentDeleteTime AND IS_AGENT = 'True' AND BUSINESS_UNIT = @pa_BusinessUnit; 
			DELETE tbl_agent FROM tbl_agent 
				INNER JOIN @DeletedAgentSessionTable DGST
					ON tbl_agent.SESSIONID = DGST.SESSIONID
						AND tbl_agent.AGENT_NAME = DGST.AGENT_NAME
			-- Delete agents who are not exists in session table
			DELETE FROM tbl_agent
				WHERE SESSIONID NOT IN (SELECT SESSIONID FROM tbl_active_noise_sessions WHERE IS_AGENT = 'True')
						
			-- Retrieve the users online
			SELECT COUNT(*) AS ONLINE FROM tbl_active_noise_sessions WITH (NOLOCK) WHERE IS_AGENT = 'False' AND BUSINESS_UNIT = @pa_BusinessUnit;
		END
		
	RETURN
END
GO