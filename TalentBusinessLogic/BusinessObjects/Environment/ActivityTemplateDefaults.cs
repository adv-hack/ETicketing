using System;
using System.Collections.Generic;
using Talent.Common;
using System.Data;

namespace TalentBusinessLogic.BusinessObjects.Environment
{
    public class ActivityTemplateDefaults : DEActivityTemplateDefaults
    {
        private BusinessObjects businessObjects;

        public ActivityTemplateDefaults()
        {
            businessObjects = new BusinessObjects();
        }
        /// <summary>
        /// Buffer that can be used to retrieve the default values
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        public DefaultValues GetDefaults(int templateID)
        {
            DefaultValues activityDefaultValues = new DefaultValues();
            activityDefaultValues = GetDefaultValues(templateID);
            return activityDefaultValues;
        }
        /// <summary>
        /// Get the default values for a particular templare
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns>A DefaultValues object</returns>
        private DefaultValues GetDefaultValues(int templateID)
        {
            TimeSpan timeSpan = DateTime.Now.TimeOfDay;
            DefaultValues def = new DefaultValues();
            TalentDataObjects talDataObjects = new TalentDataObjects();
            string cacheKey = "ActivityTemplateDefaults" + Talent.Common.Utilities.FixStringLength("MAINTENANCE", 50) + Talent.Common.Utilities.FixStringLength("*ALL", 50);
            talDataObjects.Settings = businessObjects.Environment.Settings.GetBasicSettingsObject();
            DataTable defaults = talDataObjects.ActivitiesSettings.TblActivityTemplatesDefaults.GetDefaultsByTemplateID(templateID);
            if (Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey))
            {
                def = (DefaultValues)businessObjects.Data.Cache.Get(cacheKey);
            }
            else
            {
                try
                {
                    if (defaults.Rows.Count > 0)
                    {
                        def = PopulateDefaults(def, defaults);
                    }
                }
                catch (Exception ex)
                {

                }
                //Add to cache now
                businessObjects.Data.Cache.Add(cacheKey, def, businessObjects.Environment.Settings.CacheTimeInMins);
            }
            businessObjects.Logging.LoadTestLog("ActivityTemplateDefaults.cs", "GetDefaultValuesByTemplateID", timeSpan);
            return def;
        }

        /// <summary>
        /// Fills the defaultvalues object from tbl_activity_templates_defaults
        /// </summary>
        /// <param name="def"></param>
        /// <param name="defaults"></param>
        /// <returns>Default Values object</returns>
        private DefaultValues PopulateDefaults(DefaultValues def, DataTable defaults)
        {
            try
            {
                foreach (DataRow defRow in defaults.Rows)
                {
                    try
                    {
                        switch (defRow["DEFAULT_NAME"].ToString())
                        {
                            case "COMPLIMENTARY_CHECK_ENABLED":
                                def.ComplimentaryCheckEnabled = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["DEFAULT_VALUE"]);
                                break;
                            case "DISPLAY_ON_PRICE_BAND":
                                def.DisplayOnPriceBand = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["DEFAULT_VALUE"]);
                                break;
                            case "DISPLAY_ON_PRICE_CODE":
                                def.DisplayOnPriceCode = businessObjects.Data.Validation.CheckForDBNull_Boolean_DefaultFalse(defRow["DEFAULT_VALUE"]);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return def;
        }             
    }
}
