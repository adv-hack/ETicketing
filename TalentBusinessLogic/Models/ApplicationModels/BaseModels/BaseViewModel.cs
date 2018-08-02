using Talent.Common;
using Environment = TalentBusinessLogic.BusinessObjects.Environment;

namespace TalentBusinessLogic.Models
{
    public class BaseViewModel
    {
        private WebFormResource _pageResource;
        private UserControlResource _controlResource;
        private Environment.Settings _settings = new Environment.Settings();
        public BaseViewModel() { }
        public BaseViewModel(WebFormResource resource, bool getContentAndAttributes) 
        {
            if (getContentAndAttributes) 
            { 
                resource.BusinessUnit = _settings.BusinessUnit;
                resource.PartnerCode = _settings.Partner;
                resource.PageCode = _settings.Page;
                resource.KeyCode = _settings.Page;
                resource.FrontEndConnectionString = _settings.FrontEndConnectionString;
                _pageResource = resource;
            }
        }

        public BaseViewModel(WebFormResource resource, bool getContentAndAttributes, string pageCode)
        {
            if (getContentAndAttributes)
            {
                resource.BusinessUnit = _settings.BusinessUnit;
                resource.PartnerCode = _settings.Partner;
                resource.PageCode = pageCode;
                resource.KeyCode = pageCode;
                resource.FrontEndConnectionString = _settings.FrontEndConnectionString;
                _pageResource = resource;
            }
        }

        //control code needs to added to the settings object in a sensible way
        public BaseViewModel(UserControlResource resource, bool getContentAndAttributes, string controlCode)
        {
            if (getContentAndAttributes)
            {
                resource.BusinessUnit = _settings.BusinessUnit;
                resource.PartnerCode = _settings.Partner;
                resource.PageCode = _settings.Page;
                resource.KeyCode = controlCode;
                resource.FrontEndConnectionString = _settings.FrontEndConnectionString;
                _controlResource = resource;
            }
        }

        public string BusinessUnit { get; set; }
        public string Partner { get; set; }
        public string Page { get; set; }
        public ErrorModel Error { get; set; }

        public string GetPageText(string code, bool fromCache = true) 
        {
            if (_pageResource == null)
            {
                return string.Empty;
            }
            else 
            {
                return _pageResource.get_Content(code, "ENG", fromCache);
            }
            
        }

        public string GetControlText(string code, bool fromCache = true) 
        {
            if (_controlResource == null)
            { 
                return string.Empty; 
            }
            else 
            {
                return _controlResource.get_Content(code, "ENG", fromCache); 
            }             
        }

        public string GetPageAttribute(string code, bool fromCache = true)
        {
            if (_pageResource == null)
            {
                return string.Empty;
            }
            else 
            { 
                return _pageResource.get_Attribute(code,"ENG", fromCache);
            }
        }

        public string GetControlAttribute(string code, bool fromCache = true)
        {
            if (_controlResource == null)
            {
                return string.Empty;
            }
            else
            {
                return _controlResource.get_Attribute(code, fromCache);
            }
        }

    }
}
