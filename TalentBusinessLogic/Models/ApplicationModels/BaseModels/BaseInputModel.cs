
using System.Collections.Generic;

namespace TalentBusinessLogic.Models
{
    public class BaseInputModel
    {
        public string Source { get; set; }
        //public string FrontEndConnectionString { get; set; }
        //public string Url { get; set; }
        ////TODO: We will remove this property when the Profile is available.
        ////Please pass this information down from EBusiness to make it work.
        //public string LoginId { get; set; }

        public string ControlCode { get; set; }

        public List<string> ExcludeForRequiredFieldValidation { get; set; }

        public void ExcludeForRequiredField(params string[] properties)
        {
            ExcludeForRequiredFieldValidation = new List<string>();
            ExcludeForRequiredFieldValidation.AddRange(properties);
        }
    }
}
