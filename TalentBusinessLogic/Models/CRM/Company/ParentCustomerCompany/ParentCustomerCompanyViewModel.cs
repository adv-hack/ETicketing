using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;
using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class ParentCustomerCompanyViewModel : BaseViewModel
    {
        public ParentCustomerCompanyViewModel(bool getContentAndAttributes, string controlCode) : base(new UserControlResource(), getContentAndAttributes, controlCode) { }
        public ParentCustomerCompanyViewModel() { }
        public string ControlCode { get; set; }
        public ParentCompanyModel ParentCompany { get; set; }
        //public ErrorModel Error { get; set; }
    }
    public class ParentCompanyModel : BaseViewModel
    {
        public string ParentCompanyNumber { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string WebAddress { get; set; }
        public string TelephoneNumber1 { get; set; }
        public bool Telephone1Use { get; set; }
        public string TelephoneNumber2 { get; set; }
        public bool Telephone2Use { get; set; }
        public string TelephoneNumber3 { get; set; }
        public string VATCode { get; set; }
        public string SalesLedgerCode { get; set; }
        public string OwningAgent { get; set; }
        public string SalesLedgerAccount { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
    }
}
