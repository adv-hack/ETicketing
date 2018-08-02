
using System.Collections.Generic;
using TalentBusinessLogic.Models;
namespace TalentBusinessLogic.DataTransferObjects.JQuery.DataTables.List
{

    public class CustomerSearchLists
    {
        public List<CustomerSearch.Customers> Customers { get; set; }
        public List<CustomerSearch.Clubs> Clubs { get; set; }
        public ErrorModel Errors { get; set; }
    }

    public class CustomerSearch
    {

        public class Customers
        {
            public KeyValuePair<bool, string> CustomerNumberOptions { get; set; }
            public string CustomerNo { get; set; }
            public string ContactForename { get; set; }
            public string ContactSurname { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string AddressLine3 { get; set; }
            public string AddressLine4 { get; set; }
            public string PostCode { get; set; }
            public string DateBirth { get; set; }
            public string PassportNumber { get; set; }
            public string EmailAddress { get; set; }
            public string PhoneNumber { get; set; }
            public string PhoneNumber1 { get; set; }
            public string PhoneNumber2 { get; set; }
            public string PhoneNumber3 { get; set; }
            public string SessionID { get; set; }
            public string MembershipNumber { get; set; }
            public string WebReady { get; set; }
            public string OnWatchList { get; set; }
            // needed for datatables do not delete https://datatables.net/manual/tech-notes/4
            public string Print { get; set; }
            public string Update { get; set; }

        }

        public class Clubs
        {
            public string ClubCode { get; set; }
            public string ClubDescription { get; set; }
            public string DisplaySequence { get; set; }
            public bool IsDefault { get; set; }
            public string CustomerValidationUrl { get; set; }
            public string InvalidCustomerForwardUrl { get; set; }
            public string ValidCustomerForwardUrl { get; set; }
            public string NoiseEncryptionKey { get; set; }
            public string SupplynetLoginid { get; set; }
            public string SupplynetPassword { get; set; }
            public string SupplynetCompany { get; set; }
            public string AgentType { get; set; }
            public string StandardCustomerForwardUrl { get; set; }

        }
        
    }
}
