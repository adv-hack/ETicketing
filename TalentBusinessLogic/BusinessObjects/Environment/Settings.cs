using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web;
using Talent.Common;
using Talent.Common.DataObjects.TableObjects;
using TalentBusinessLogic.BusinessObjects.Data;

namespace TalentBusinessLogic.BusinessObjects.Environment
{
    public class Settings
    {
        #region Class Level Fields

        private String _businessUnit;
        private ECommerceModuleDefaults.DefaultValues _defaultValues;
        private String _frontEndConnectionString;
        private String _url;
        private String _partner;
        private String _page;
        private DataSettings _data;
        private DESettings _settings;
        private bool? _isAgent;
        private const string _cacheTimeInMins = "30";

        #endregion

        #region Public Properties

        public String BusinessUnit
        {
            get
            {
                if (String.IsNullOrEmpty(_businessUnit))
                {
                    _businessUnit = GetBusinessUnit();
                }

                return _businessUnit;
            }
        }

        public bool? IsAgent 
        { 
            get
            {
                if (_isAgent == null)
                {
                    _isAgent = DESettings.IsAgent;
                }
                return _isAgent;
            }
        }

        public ECommerceModuleDefaults.DefaultValues DefaultValues { 
            get
            {
                if (_defaultValues == null)
                {
                    var defaults = new ECommerceModuleDefaults();
                    _defaultValues = defaults.GetDefaults(Partner, BusinessUnit);

                }

                return _defaultValues;
            }
        }

        public String Partner 
        { 
            get
            {
                if (String.IsNullOrEmpty(_partner))
                {
                    _partner = GetDefaultPartner();
                }
                return _partner;
            }
        }

        public string Page 
        { 
            get
            {
                if (String.IsNullOrEmpty(_page))
                {
                    _page = GetCurrentPageName();
                }
                return _page;
            }
        }

        public String FrontEndConnectionString 
        { 
            get
            {
                if (String.IsNullOrEmpty(_frontEndConnectionString))
                {
                    _frontEndConnectionString = GetConnectionSting();
                }
                return _frontEndConnectionString;
            }
        }

        public String Url
        {
            get
            {
                if (String.IsNullOrEmpty(_url))
                {
                    _url = GetURL();
                }
                return _url;
            }
        }

        public String OverwriteBU { 
            get
            {
                if (ConfigurationManager.AppSettings["OverwriteBU"] !=  null && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["OverwriteBU"].ToString())) 
                {
                    return ConfigurationManager.AppSettings["OverwriteBU"].ToString();
                }

                return null;
            } 
        }

        public DESettings DESettings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = GetSettingsObject();
                }
                return _settings;
            }
        }

        public DataSettings Data {
            get
            {
                if (_data == null)
                {
                    _data = new DataSettings();
                }
                return _data;
            }
        }

        public String CacheTimeInMins 
        { 
            get
            {
                return _cacheTimeInMins;
            }
        }

        #endregion

        #region Get DESettings

        public DESettings GetBasicSettingsObject(bool withBusinessUnit = true)
        {
            DESettings settings = new DESettings();
            if (withBusinessUnit) {
              settings.BusinessUnit = BusinessUnit; 
            }
            settings.FrontEndConnectionString = FrontEndConnectionString;
            settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE;
            return settings;
        }

        private DESettings GetSettingsObject(bool withConnStringList = true)
        {
            DESettings settings = new DESettings();
            settings.BusinessUnit = BusinessUnit;
            settings.Partner = Partner;
            settings.FrontEndConnectionString = FrontEndConnectionString;
            settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE;
            settings.OriginatingSourceCode = GlobalConstants.SOURCE;
            settings.CacheDependencyPath = DefaultValues.CacheDependencyPath;
            settings.EcommerceModuleDefaultsValues = DefaultValues;
            settings.CanProcessFeesParallely = DefaultValues.CanProcessFeesParallely;
            settings.AgentEntity = GetAgentEntity(DefaultValues.CacheDependencyPath);
            settings.StoredProcedureGroup = DefaultValues.StoredProcedureGroup;
            if (HttpContext.Current.Session != null && Data.Session.Get("Agent") != null)
            {
                settings.IsAgent = true;
                settings.OriginatingSource = GetOriginatingSource(Data.Session.Get("Agent").ToString());
            }
            if (withConnStringList)
            {
                settings.ConnectionStringList = GetConnectionStringList();
            }
            settings.LoginId = GlobalConstants.GENERIC_CUSTOMER_NUMBER;
            settings.CurrentPageName = Page;

            //TODO: The BusinessLogic layer doesn't have the Profile object at the moment

            //if (HttpContext.Current.Profile != null)
            //{
            //    TalentProfileUser talProfileUser = (HttpContext.Current.Profile).User;
            //    if (talProfileUser != null && talProfileUser.Details != null)
            //    {
            //        if (!string.IsNullOrWhiteSpace(talProfileUser.Details.LoginID))
            //        {
            //            settings.LoginId = talProfileUser.Details.LoginID;
            //        }
            //    }
            //}

            //current page name
            //settings.CurrentPageName = (new FileInfo(Url)).Name.ToLower();
            //settings.DeliveryCountryCode = GetDeliveryCountryISOAlpha3Code();
            return settings;
        }

        private DEAgent GetAgentEntity(string cacheDependencyPath)
        {
            DEAgent agentEntity = new DEAgent();
            if (!String.IsNullOrEmpty(Data.Session.SessionId) && Data.Session.Get("Agent") != null && Data.Session.Get("Agent").ToString().Trim().Length > 0)
            {
                TalentDataObjects talDataObject = new TalentDataObjects();
                DESettings settings = new DESettings();
                settings.FrontEndConnectionString = FrontEndConnectionString;
                settings.CacheDependencyPath = cacheDependencyPath;
                settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE;
                talDataObject.Settings = settings;

                DataTable dtTblAgent = talDataObject.AgentSettings.TblAgent.GetByAgentName(Data.Session.SessionId, Data.Session.Get("Agent").ToString());
                if (dtTblAgent != null && dtTblAgent.Rows.Count > 0)
                {
                    agentEntity.AgentUsername = Data.Validation.CheckForDBNull_String(dtTblAgent.Rows[0]["AGENT_NAME"]);
                    agentEntity.AgentType = Data.Session.Get("AgentType").ToString();
                    agentEntity.Department = Data.Validation.CheckForDBNull_String(dtTblAgent.Rows[0]["DEPARTMENT"]).Trim();
                    agentEntity.PrinterNameDefault = Data.Validation.CheckForDBNull_String(dtTblAgent.Rows[0]["PRINTER_NAME_DEFAULT"]);
                    agentEntity.BulkSalesMode = Data.Validation.CheckForDBNull_Boolean_DefaultFalse(dtTblAgent.Rows[0]["BULK_SALES_MODE"]);
                }
                else
                {
                    if (!Url.ToLower().Contains("agentlogin.aspx?logout=expired"))
                    {
                        Data.Session.Remove("Agent");
                        Data.Session.Add("IsAgentProfileEmpty", true);
                        HttpContext.Current.Response.Redirect("~/PagesPublic/Agent/AgentLogin.aspx?logout=expired");
                    }
                }
            }
            return agentEntity;
        }

        private string GetOriginatingSource(string agentFromSession)
        {
            string originatingSource = string.Empty;
            if (DefaultValues.TicketingKioskMode)
            {
                originatingSource = "KIOSK";
            }
            else
            {
                if ((agentFromSession != null))
                {
                    originatingSource = Convert.ToString(agentFromSession);
                }
                else
                {
                    originatingSource = string.Empty;
                }
            }
            return originatingSource;
        }

        private List<string> GetConnectionStringList()
        {
            List<string> connStringList = new List<string>();
            string cacheKey = "ConnectionStringList";
            if (TalentThreadSafe.ItemIsInCache(cacheKey))
            {
                connStringList = (List<String>) Data.Cache.Get(cacheKey);
            }
            else
            {
                string tempConnectionString = string.Empty;
                if (TryGetConnectionString("liveUpdateDatabase01", out tempConnectionString))
                    connStringList.Add(tempConnectionString);
                if (TryGetConnectionString("liveUpdateDatabase02", out tempConnectionString))
                    connStringList.Add(tempConnectionString);
                if (TryGetConnectionString("liveUpdateDatabase03", out tempConnectionString))
                    connStringList.Add(tempConnectionString);
                if (TryGetConnectionString("liveUpdateDatabase04", out tempConnectionString))
                    connStringList.Add(tempConnectionString);
                if (TryGetConnectionString("liveUpdateDatabase05", out tempConnectionString))
                    connStringList.Add(tempConnectionString);
                if (TryGetConnectionString("liveUpdateDatabase06", out tempConnectionString))
                    connStringList.Add(tempConnectionString);
                if (TryGetConnectionString("liveUpdateDatabase07", out tempConnectionString))
                    connStringList.Add(tempConnectionString);
                if (TryGetConnectionString("liveUpdateDatabase08", out tempConnectionString))
                    connStringList.Add(tempConnectionString);
                if (TryGetConnectionString("liveUpdateDatabase09", out tempConnectionString))
                    connStringList.Add(tempConnectionString);
                if (TryGetConnectionString("liveUpdateDatabase10", out tempConnectionString))
                    connStringList.Add(tempConnectionString);

                //Cache the result
                Data.Cache.Add(cacheKey, connStringList, CacheTimeInMins);
            }
            return connStringList;
        }

        private bool TryGetConnectionString(string appSetConnVariableName, out string connectionString)
        {
            bool isExists = false;
            try
            {
                connectionString = ConfigurationManager.AppSettings[appSetConnVariableName].Trim();
                if (connectionString.Length <= 0)
                {
                    connectionString = string.Empty;
                    isExists = false;
                }
                else
                {
                    isExists = true;
                }
            }
            catch (Exception)
            {
                // Handle web.config not containing any entries for 'liveUpdateDatabaseXX' - in this case assume standard frontend database to update 
                if (appSetConnVariableName == "liveUpdateDatabase01")
                {
                    connectionString = FrontEndConnectionString;
                    isExists = true;
                }
                else
                {
                    connectionString = string.Empty;
                    isExists = false;
                }
            }
            return isExists;
        }

        #endregion

        #region Private Methods

        private string GetConnectionSting()
        {
            string connStr = ConfigurationManager.ConnectionStrings["TalentEBusinessDBConnectionString"].ToString();
            return connStr;
        }

        private string GetURL()
        {
            var url = HttpContext.Current.Request.Url.AbsoluteUri;
            return url;
        }

        private string GetAbsolutePath()
        {
            string path = HttpContext.Current.Request.Url.AbsolutePath;
            return path;
        }

        private String GetBusinessUnit()
        {
            var values = Url.Split('/');
            int j = 0;
            if (Url.Contains("localhost:"))
            {
                do
                {
                    if (values[j].Contains("localhost:"))
                    {
                        values[j] = "localhost";
                        break;
                    }
                    j++;
                } while (j < values.Length);
            }

            string cachedBusinessUnit = string.Empty;
            DEBusinessUnitURLDevice BusinessUnitURLDeviceEntity = TryToGetCachedBusinessUnit(values);
            //----------------------------
            // If not in cache get from DB
            //-----------------------------
            if (BusinessUnitURLDeviceEntity == null || string.IsNullOrEmpty(BusinessUnitURLDeviceEntity.BusinessUnit))
            {
                BusinessUnitURLDeviceEntity = GetBusinessUnitFromDB(values);
                cachedBusinessUnit = BusinessUnitURLDeviceEntity.BusinessUnit;
            }
            else
            {
                cachedBusinessUnit = BusinessUnitURLDeviceEntity.BusinessUnit;
            }

            if (OverwriteBU != null)
            {
                cachedBusinessUnit = OverwriteBU;
            }

            return cachedBusinessUnit;
        }

        private DEBusinessUnitURLDevice TryToGetCachedBusinessUnit(string[] url)
        {
            //Loop backwards through the length of the array
            DEBusinessUnitURLDevice businessUnitURLDeviceEntity = null;
            for (int i = url.Length - 1; i >= 0; i += -1)
            {
                string cacheString = GetNextURLString(url, i);
                if (Data.Cache.Get("BU" + cacheString) == null)
                {
                }
                else
                {
                    return (DEBusinessUnitURLDevice)Data.Cache.Get("BU" + cacheString);
                }
            }
            return businessUnitURLDeviceEntity;
        }

        private DEBusinessUnitURLDevice GetBusinessUnitFromDB(string[] url)
        {
            DEBusinessUnitURLDevice BusinessUnitURLDeviceEntity = new DEBusinessUnitURLDevice();
            string cacheString = null;
            //Loop backwards through the length of the array
            for (int i = url.Length - 1; i >= 0; i += -1)
            {
                cacheString = GetNextURLString(url, i);
                TalentDataObjects tDataObjects = new TalentDataObjects();
                tDataObjects.Settings = GetBasicSettingsObject(false); 
                System.Data.DataTable dt = tDataObjects.AppVariableSettings.TblUrlBu.GetBUByURL(cacheString);
                if (dt.Rows.Count > 0)
                {
                    if (!Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows[0]["DISABLED"]))
                    {
                        BusinessUnitURLDeviceEntity.BusinessUnit = dt.Rows[0]["BUSINESS_UNIT"].ToString();
                        BusinessUnitURLDeviceEntity.BusinessUnitGroup = Utilities.CheckForDBNull_String(dt.Rows[0]["BU_GROUP"]);
                        BusinessUnitURLDeviceEntity.DeviceType = Utilities.CheckForDBNull_String(dt.Rows[0]["DEVICE_TYPE"]);
                        BusinessUnitURLDeviceEntity.URL = Utilities.CheckForDBNull_String(dt.Rows[0]["URL"]);
                        BusinessUnitURLDeviceEntity.URLGroup = Utilities.CheckForDBNull_String(dt.Rows[0]["URL_GROUP"]);
                        //todo call ecom module defaults data object for the given bu to get queue url
                        Data.Cache.Add("BU" + cacheString, BusinessUnitURLDeviceEntity, CacheTimeInMins);
                        return BusinessUnitURLDeviceEntity;
                    }
                }
            }
            return BusinessUnitURLDeviceEntity;
        }

        public String GetPartner()
        {
            string partner = "*ALL";
            TalentDataObjects tDataObjects = new TalentDataObjects();
            tDataObjects.Settings = GetBasicSettingsObject();
            //string sesh = Data.Session.SessionId.ToString();
            long basketHeaderID = tDataObjects.BasketSettings.TblBasketHeader.GetHeaderByBasketSessionID(Data.Session.SessionId.ToString(), BusinessUnit);
            DataTable dt = tDataObjects.BasketSettings.TblBasketHeader.GetHeaderByBasketHeaderID(basketHeaderID.ToString());

            if (dt != null && dt.Rows.Count > 0)
            {
                if (Utilities.CheckForDBNull_String(dt.Rows[0]["PARTNER"]) != string.Empty)
                {
                    partner = dt.Rows[0]["PARTNER"].ToString();
                }
            }
            return partner;
        }

        private string GetNextURLString(string[] url, int i)
        {
            string cacheString = string.Empty;
            //Contruct the url string
            cacheString = string.Empty;
            for (int j = 0; j <= i; j++)
            {
                if (url[j] == "http:" || url[j] == "https:" || string.IsNullOrEmpty(url[j]))
                {
                }
                else
                {
                    cacheString += url[j] + "/";
                }
            }

            //Remove the end "/"
            if (cacheString.EndsWith("/"))
            {
                cacheString = cacheString.TrimEnd('/');
            }

            //' if it's localhost and a specified port (debugging) then remove port
            //If cacheString.Contains("localhost:") And i = 2 Then
            //    cacheString = "localhost"
            //End If
            return cacheString;
        }

        private string GetDefaultPartner()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            try
            {
                // Try to retrieve the table from cache
                dt = (System.Data.DataTable)Data.Cache.Get("DefaultPartnersTable");

                if (dt == null)
                {
                    tbl_authorized_partners partners = new tbl_authorized_partners(GetBasicSettingsObject());
                    dt = partners.GetByBU(BusinessUnit);
                    Data.Cache.Add("DefaultPartnersTable", dt, CacheTimeInMins);
                }

                //Retrieve the default partner for this business unit
                if ((dt != null))
                {
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        if (BusinessUnit.Equals(row["BUSINESS_UNIT"]))
                        {
                            return row["PARTNER"].ToString();
                        }
                    }
                }

                return Utilities.GetAllString();
            }
            catch (Exception)
            {
                return Utilities.GetAllString();
            }
        }

        private string GetCurrentPageName()
        {
            string path = GetAbsolutePath();
            System.IO.FileInfo oInfo = new System.IO.FileInfo(path);
            string sRet = oInfo.Name;
            string saveRet = sRet;
            NameValueCollection queryString = HttpContext.Current.Request.QueryString;
            if (!sRet.EndsWith(".aspx"))
            {
                //This assumes the page name is being retreieved from a webmethod such as: "VisualSeatSelection.aspx/GetSeating"
                //We need the page name "VisualSeatSelection.aspx" and not the method name "GetSeating"
                sRet = oInfo.Directory.Name;
            }
            switch (sRet.ToUpper())
            {
                case "USERDEFINED.ASPX":
                    sRet = sRet + "?";
                    string[] pageCodeQueryKeys = DefaultValues.UserDefinedPageQueryKeys.Split(';');
                    if (pageCodeQueryKeys != null && pageCodeQueryKeys.Length > 0)
                    {
                        for (int arrIndex = 0; arrIndex <= pageCodeQueryKeys.Length - 1; arrIndex++)
                        {
                            foreach (string queryKey in queryString.AllKeys)
                            {
                                if (pageCodeQueryKeys[arrIndex].ToLower() == queryKey.ToLower())
                                {
                                    sRet = sRet + queryKey + "=" + queryString[queryKey] + "&";
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                            }
                        }
                    }
                    sRet = sRet.TrimEnd('&');
                    break;
                case "PRODUCTHOME.ASPX":
                    if (!(string.IsNullOrEmpty(queryString["ProductSubType"])))
                    {
                        sRet = sRet + "?" + "ProductSubType=" + queryString["ProductSubType"].ToUpper();
                    }
                    break;
                case "PRODUCTSEASON.ASPX":
                    if (!(string.IsNullOrEmpty(queryString["ProductSubType"])))
                    {
                        sRet = sRet + "?" + "ProductSubType=" + queryString["ProductSubType"].ToUpper();
                    }
                    break;
                case "PRODUCTAWAY.ASPX":
                    if (!(string.IsNullOrEmpty(queryString["ProductSubType"])))
                    {
                        sRet = sRet + "?" + "ProductSubType=" + queryString["ProductSubType"].ToUpper();
                    }
                    break;
                case "PRODUCTEVENT.ASPX":
                case "PRODUCTTRAVEL.ASPX":
                    if (!(string.IsNullOrEmpty(queryString["ProductSubType"])))
                    {
                        sRet = sRet + "?" + "ProductSubType=" + queryString["ProductSubType"].ToUpper();
                    }
                    break;
                case "PRODUCTMEMBERSHIP.ASPX":
                    if (!(string.IsNullOrEmpty(queryString["ProductSubType"])))
                    {
                        sRet = sRet + "?" + "ProductSubType=" + queryString["ProductSubType"].ToUpper();
                    }
                    break;
            }

            //Validate that a page exists if we have added a query string for product groups
            if (saveRet != sRet && sRet.ToUpper() != "USERDEFINED.ASPX")
            {
                TalentDataObjects tDataObjects = new TalentDataObjects();
                tDataObjects.Settings = GetBasicSettingsObject();
                DataTable dt = tDataObjects.PageSettings.TblPage.GetByPageCode(sRet, BusinessUnit);
                if (dt == null || dt.Rows.Count == 0)
                {
                    sRet = saveRet;
                }
            }

            return sRet;
        }

        #endregion
    }
}
