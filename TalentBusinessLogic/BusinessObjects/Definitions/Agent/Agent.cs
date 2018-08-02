using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBusinessLogic.Models;
using TalentBusinessLogic.BusinessObjects;
using TalentBusinessLogic.BusinessObjects.Definitions;
using TalentBusinessLogic.DataTransferObjects;
using Talent;
using Talent.Common;

namespace TalentBusinessLogic.BusinessObjects.Definitions
{
    public class Agent : BusinessObjects
    {
        public List<TalentBusinessLogic.DataTransferObjects.Agent> retrieveAgents()
        {
            List<TalentBusinessLogic.DataTransferObjects.Agent> AgentList = new List<TalentBusinessLogic.DataTransferObjects.Agent>();
            TalentAgent talAgent = new TalentAgent();
            DEAgent agentDataEntity = new DEAgent();
            ErrorModel errModel = new ErrorModel();
            ErrorObj err = new ErrorObj();

            agentDataEntity.Source = GlobalConstants.SOURCE;
            talAgent.AgentDataEntity = agentDataEntity;
            talAgent.Settings = Environment.Settings.DESettings;
            err = talAgent.RetrieveAllAgents();
            errModel = Data.PopulateErrorObject(err, talAgent.ResultDataSet, talAgent.Settings, "ErrorStatus", null);

            if (!errModel.HasError)
            {
               AgentList = Data.PopulateObjectListFromTable<TalentBusinessLogic.DataTransferObjects.Agent>(talAgent.ResultDataSet.Tables["AgentUsers"]);
            }
            return AgentList;
        }

        /// <summary>
        /// Get the descriptive agent name based on the given user code, Eg. "TKT472TF"
        /// </summary>
        /// <param name="agentUserCode">The given agent user code</param>
        /// <returns>The descriptive agent name</returns>
        public string GetAgentDescriptiveNameByAgentUserCode(string agentUserCode)
        {
            string agentDescriptiveName = agentUserCode;
            TalentAgent talAgent = new TalentAgent();
            DEAgent agentDataEntity = new DEAgent();
            ErrorModel errModel = new ErrorModel();
            ErrorObj err = new ErrorObj();

            agentDataEntity.Source = GlobalConstants.SOURCE;
            talAgent.AgentDataEntity = agentDataEntity;
            talAgent.Settings = Environment.Settings.DESettings;
            err = talAgent.RetrieveAllAgents();
            errModel = Data.PopulateErrorObject(err, talAgent.ResultDataSet, talAgent.Settings, "ErrorStatus", null);

            if (!errModel.HasError)
            {
                foreach (DataRow row in talAgent.ResultDataSet.Tables["AgentUsers"].Rows)
                {
                    if (row["USERCODE"].ToString() == agentUserCode.Trim())
                    {
                        agentDescriptiveName = row["USERNAME"].ToString();
                        break;
                    }
                }
            }
            return agentDescriptiveName;
        }
    }
}
