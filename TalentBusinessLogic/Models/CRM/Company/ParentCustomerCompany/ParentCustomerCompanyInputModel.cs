using System.Collections.Generic;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;
using System.ComponentModel.DataAnnotations;
using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class ParentCustomerCompanyInputModel : BaseInputModel
    {
        public string ParentCompanyNumber { get; set; }
        public string ChildCompanyNumber { get; set; }
        public string CompanyNumber { get; set; }
        public GlobalConstants.CRUDOperationMode CompanyOperationMode { get; set; }
    }
}
