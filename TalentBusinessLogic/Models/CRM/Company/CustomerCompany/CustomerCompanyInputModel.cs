using System.Collections.Generic;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;
using System.ComponentModel.DataAnnotations;
using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class CustomerCompanyInputModel : BaseInputModel 
    {
        public string SessionID { get; set; }
        public string CompanyNumber { get; set; }
        public string CustomerNumber { get; set; }
        public string AgentName { get; set; }
        //public string ControlCode { get; set; }
        public GlobalConstants.CRUDOperationMode CompanyOperationMode { get; set; }
    }
}
