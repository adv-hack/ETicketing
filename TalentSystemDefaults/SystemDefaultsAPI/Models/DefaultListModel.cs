using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Web.Mvc;
using TalentSystemDefaults;
using TalentSystemDefaults.DataEntities;
namespace SystemDefaultsAPI
{
    namespace Models
    {
        public class DefaultListModel
        {
            private DEList deList;
            public const string BUSINESS_UNIT = "BUSINESS_UNIT";
            public const string OPTIONS = "Options";
            public List<SelectListItem> BusinessUnits { get; set; }
            private string _BackURL = string.Empty;
            public string BackURL
            {
                get
                {
                    return _BackURL;
                }
                set
                {
                    _BackURL = value;
                }
            }
            public bool ShowBackURL
            {
                get
                {
                    return BackURL == string.Empty ? false : true;
                }
            }
            public string StatusMessage
            {
                get
                {
                    return this.deList.StatusMessage;
                }
            }
            public DefaultListModel(DEList deList)
            {
                this.deList = deList;
                Initialise();
            }
            private void Initialise()
            {
                if (EnableBUSelector)
                {
                    BusinessUnits = new List<SelectListItem>();
                    foreach (string bu in deList.BusinessUnits)
                    {
                        BusinessUnits.Add(new SelectListItem { Text = bu, Value = bu });
                    }
                }
                if (!(deList.Title.ToLower() == "modules" || deList.Title.ToLower() == "database audits"))
                {
                    BackURL = "/defaultList.aspx?listname=Modules&businessUnit={0}";
                }
            }
            public bool HasStatus
            {
                get
                {
                    return !string.IsNullOrEmpty(StatusMessage);
                }
            }
            public string Title
            {
                get
                {
                    return deList.Title;
                }
            }
            public string ModuleName
            {
                get
                {
                    return deList.ModuleName;
                }
            }
            public List<TalentSystemDefaults.DataColumn> DataColumns
            {
                get
                {
                    return deList.DataColumns;
                }
            }
            public bool EnableBUSelector
            {
                get
                {
                    return deList.EnableBUSelector;
                }
            }
            public bool EnableSelectColumn
            {
                get
                {
                    return deList.EnableSelectColumn;
                }
            }
            public bool EnableSelectAsHyperLink
            {
                get
                {
                    return deList.EnableSelectAsHyperLink;
                }
            }
            public bool EnableEditColumn
            {
                get
                {
                    return deList.EnableEditColumn;
                }
            }
            public bool EnableDeleteColumn
            {
                get
                {
                    return deList.EnableDeleteColumn;
                }
            }
            public List<DEListDetail> Data
            {
                get
                {
                    return deList.Data;
                }
            }
            public List<string> ActionButtons
            {
                get
                {
                    return deList.ActionButtons;
                }
            }
            public string AddURL
            {
                get
                {
                    return deList.AddURL;
                }
            }
            public string UpdateURL
            {
                get
                {
                    return deList.UpdateURL;
                }
            }
            public string DeleteURL
            {
                get
                {
                    return deList.DeleteURL;
                }
            }
            public string GetProperCase(string name)
            {
                name = Strings.StrConv(name, VbStrConv.ProperCase);
                name = name.Replace("_", " ");
                return name;
            }
            public DEList GetDEList
            {
                get
                {
                    return deList;
                }
            }
        }

    }
}
