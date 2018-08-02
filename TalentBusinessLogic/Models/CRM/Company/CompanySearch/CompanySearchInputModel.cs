using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;

namespace TalentBusinessLogic.Models
{
    public class CompanySearchInputModel : BaseInputModelForDataTables
    {
        public string ParentCompanyNumber { get; set; }
        public string ChildCompanyNumber { get; set; }
        public string SearchType { get; set; }
        public string SessionID { get; set; }
        public string CompanyName { get; set; }
        public string AddressLine1 { get; set; }
        public string PostCode { get; set; }
        public string TelephoneNumber1 { get; set; }
        public string WebAddress { get; set; }
    }
}
