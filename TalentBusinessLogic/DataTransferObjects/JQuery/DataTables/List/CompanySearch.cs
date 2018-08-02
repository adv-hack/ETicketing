
using System.Collections.Generic;
using TalentBusinessLogic.Models;
namespace TalentBusinessLogic.DataTransferObjects.JQuery.DataTables.List
{

    public class CompanySearchLists
    {
        public List<CompanySearch.Company> Companies { get; set; }
        public int RecordCount { get; set; }
        public ErrorModel Errors { get; set; }
    }

    public class CompanySearch
    {

        public class Company
        {

            public string CompanyNumber { get; set; }
            public string CompanyName { get; set; }
            public string AddressLine1 { get; set; }
            public string PostCode { get; set; }
            public string Telephone { get; set; }
            public string WebAddress { get; set; }
        }        
    }
}
