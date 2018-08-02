using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talent.Common;
using TalentBusinessLogic.BusinessObjects.Environment;

namespace TalentBusinessLogic.BusinessObjects.Definitions
{
    public class ActivitiesDefinition : BusinessObjects
    {
        /// <summary>
        /// Retrieve the list of activity template records from tbl_activity_templates based on the current business unit
        /// </summary>
        /// <returns>A formatted list for use in a view model</returns>
        public List<DataTransferObjects.ActivityTemplate> RetrieveActivityTemplates()
        {
            List<DataTransferObjects.ActivityTemplate> templatesList = new List<DataTransferObjects.ActivityTemplate>();
            DataTable _dtActivityTemplates = new DataTable();
            _dtActivityTemplates = TDataObjects.ActivitiesSettings.GetActivityTemplatesForActivityPage(Environment.Settings.BusinessUnit);
            templatesList = Data.PopulateObjectListFromTable<DataTransferObjects.ActivityTemplate>(_dtActivityTemplates);
            return templatesList;
        }

        /// <summary>
        /// Retrieve the list of activity status records from tbl_activity_status_description based on the current business unit
        /// </summary>
        /// <returns>A formatted list for use in a view model</returns>
        public List<DataTransferObjects.ActivityStatus> RetrieveActivityStatus()
        {
            List<DataTransferObjects.ActivityStatus> statusList = new List<DataTransferObjects.ActivityStatus>();
            DataTable _dtActivityStatus = new DataTable();
            _dtActivityStatus = TDataObjects.ActivitiesSettings.GetActivityStatusByBusinessUnit(Environment.Settings.BusinessUnit);
            statusList = Data.PopulateObjectListFromTable<DataTransferObjects.ActivityStatus>(_dtActivityStatus);
            return statusList;
        }

        /// <summary>
        /// Calls backend for Template ID.
        /// </summary>
        /// <returns></returns>
        public string GetTicketingProductDetailsTemplateID(string productCode, string priceCode, int cacheMinutes, string cacheDependencyPath)
        {
            ErrorObj errObj = new ErrorObj();
            DESettings settingsEntity = new DESettings();
            TalentProduct talProduct = new TalentProduct();
            DataTable dtProductProductDetails = new DataTable();
            talProduct.Settings = Environment.Settings.DESettings;
            talProduct.De.ProductCode = Utilities.CheckForDBNull_String(productCode);
            talProduct.De.Src = "W";
            talProduct.De.PriceCode = priceCode;
            errObj = talProduct.ProductDetails();
            if (talProduct.ResultDataSet.Tables[2].Rows.Count > 0 && !(string.IsNullOrWhiteSpace((string)talProduct.ResultDataSet.Tables[2].Rows[0]["TemplateID"])))
            {
                return (string) talProduct.ResultDataSet.Tables[2].Rows[0]["TemplateID"];
            }
            else
            {
                return "0";
            }
        }

    }
}

        

        


