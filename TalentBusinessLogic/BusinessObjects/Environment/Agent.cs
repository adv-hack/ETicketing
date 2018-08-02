using System;
using Talent.Common;
using System.Data;

namespace TalentBusinessLogic.BusinessObjects.Environment
{
    public class Agent
    {
        private BusinessObjects businessObjects;

        public Agent()
        {
            businessObjects = new BusinessObjects();
        }

        /// <summary>
        /// Is the current session related to a logged in agent. Check the database for existance of this session string.
        /// </summary>
        /// <param name="sessionID">The session id string</param>
        /// <returns>True if the agent is logged in and the session is valid</returns>
        public bool CheckAgentLogin(string sessionID)
        {
            DataTable tblAgentDetails = new DataTable();
            bool retVal = true;
            tblAgentDetails = businessObjects.TDataObjects.AgentSettings.TblAgent.RetrieveAgentDetailsFromSessionID(sessionID);
            if (tblAgentDetails.Rows.Count == 0)
            {
                retVal = false;
            }
            else
            {
                retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// Get the agent username based on the given session ID
        /// </summary>
        /// <param name="sessionID">The session id string</param>
        /// <returns>The agent username such as TKT472TF</returns>
        public string GetAgentUserNameBySessionId(string sessionID)
        {
            DataTable tblAgentDetails = new DataTable();
            string agentUserName = String.Empty;
            tblAgentDetails = businessObjects.TDataObjects.AgentSettings.TblAgent.RetrieveAgentDetailsFromSessionID(sessionID);
            if (tblAgentDetails.Rows.Count > 0)
            {
                agentUserName = tblAgentDetails.Rows[0]["AGENT_NAME"].ToString();
            }
            return agentUserName;
        }

    }
}