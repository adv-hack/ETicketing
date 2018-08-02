using System;
using System.Data;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using Talent.Common;
using Talent.Common.DataObjects.TableObjects;

namespace TalentBusinessLogic.DataAnnotation.Base
{
    /// <summary>
    /// This is a helper class that knows how to retrieve
    /// text/attribute values from the database. This also
    /// does reflection related stuff.
    /// </summary>
    public class TalentDataValidator
    {
        #region Class Level Fields
        
        private BusinessObjects.BusinessObjects businessObjects;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public TalentDataValidator()
        {
            businessObjects = new BusinessObjects.BusinessObjects();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method gets the property name
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public string GetPropertyName(Type modelType, string displayName)
        {
            string key = GetCacheKey(modelType.Name, displayName);
            string propName = Convert.ToString(businessObjects.Data.Cache.Get(key));
           
            if (String.IsNullOrEmpty(propName))
            {
                propName = RetrievePropertyName(modelType, displayName);
                propName = propName != null ? propName : displayName;
                businessObjects.Data.Cache.Add(key, propName, businessObjects.Environment.Settings.CacheTimeInMins);
            }
            return propName;
        }

        /// <summary>
        /// This method gets the boolean attribute from the control attribute table, 
        /// if not found it inserts one
        /// </summary>
        /// <param name="controlCode"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public Boolean GetBooleanAttribute(string controlCode, string attributeName, string attributeValue)
        {
            string value = GetAttribute(controlCode, attributeName, attributeValue);
            bool retval;
            Boolean.TryParse(value, out retval);
            return retval;
        }

        /// <summary>
        /// This method gets the attribute from the control attribute table, 
        /// if not found it inserts one
        /// </summary>
        /// <param name="controlCode"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <param name="enableInsert">This flag helps to prevent insertion if needed</param>
        /// <returns></returns>
        public string GetAttribute(string controlCode, string attributeName, string attributeValue, bool enableInsert = true)
        {
            string retval = attributeValue;
            var settings = businessObjects.Environment.Settings.DESettings;

            tbl_control_attribute tblControlAttribute = new tbl_control_attribute(settings);
            DataTable data = tblControlAttribute.GetAttributeByExactValue(settings.BusinessUnit, settings.Partner, Utilities.GetAllString(), controlCode, attributeName);
            if ((data == null || data.Rows.Count == 0))
            {
                if (enableInsert)
                {
                    int affectedRows = 0;
                    tblControlAttribute.InsertOrUpdate(settings.BusinessUnit, settings.Partner, Utilities.GetAllString(), controlCode, attributeName, attributeValue, String.Empty, ref affectedRows);
                }
            }
            else
            {
                retval = data.Rows[0]["ATTR_VALUE"].ToString();
            }

            return retval;
        }

        /// <summary>
        /// This method gets the text from the control text lang table, 
        /// if not found it inserts one
        /// </summary>
        /// <param name="controlCode"></param>
        /// <param name="textCode"></param>
        /// <param name="textContent"></param>
        /// <returns></returns>
        public string GetText(string controlCode, string textCode, string textContent)
        {
            var retval = textContent;
            var settings = businessObjects.Environment.Settings.DESettings;

            tbl_control_text_lang tblControlTextLang = new tbl_control_text_lang(settings);

            DataTable data = tblControlTextLang.GetTextByExactValue(settings.BusinessUnit, settings.Partner, Utilities.GetAllString(), controlCode, textCode);
            if ((data == null || data.Rows.Count == 0))
            {
                int affectedRows = 0;
                tblControlTextLang.InsertOrUpdate(settings.Language, settings.BusinessUnit, settings.Partner, Utilities.GetAllString(), controlCode, textCode, textContent, ref affectedRows);
            }
            else
            {
                retval = data.Rows[0]["CONTROL_CONTENT"].ToString();
            }
            return retval;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method prepares and returns the cache key
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        private string GetCacheKey(string modelType, string displayName)
        {
            return String.Format("{0},dispName={1}", modelType, displayName.Replace(" ", "-"));
        }

        /// <summary>
        /// This method retrieves the property name through reflection
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        private string RetrievePropertyName(Type modelType, string displayName)
        {
            string propName = null;
            foreach (PropertyInfo prop in modelType.GetProperties())
            {
                object[] attributes = prop.GetCustomAttributes(typeof(TalentDisplayAttribute), true);
                if (attributes.Length > 0)
                {
                    TalentDisplayAttribute dispAttribute = (TalentDisplayAttribute)attributes[0];
                    if (dispAttribute.DisplayName.Equals(displayName))
                    {
                        propName = prop.Name;
                        break;
                    }
                }
            }

            return propName;
        }

        #endregion
    }
}