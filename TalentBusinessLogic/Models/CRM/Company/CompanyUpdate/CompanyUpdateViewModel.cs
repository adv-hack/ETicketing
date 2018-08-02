using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;
using TalentBusinessLogic.DataTransferObjects;
using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class CompanyUpdateViewModel : BaseViewModel
    {
        public CompanyUpdateViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public CompanyUpdateViewModel(bool getContentAndAttributes, string controlCode) : base(new UserControlResource(), getContentAndAttributes, controlCode) { }
        public CompanyUpdateViewModel() { }

        public GlobalConstants.CRUDOperationMode CompanyOperationMode { get; set; }

        public bool AddParent { get; set; }
        public CompanyModel Company { get; set; }

        public List<Agent> AgentList { get; set; }
        public List<VAT> VatCodeList { get; set; }
    }

    public class CompanyModel : BaseViewModel
    {
        public string ParentCompanyNumber { get; set; }
        public string ParentCompanyName { get; set; }
        public string ParentFlag { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyID { get; set; }
        public string CRMCompanyName { get; set; }
        public string CompanyName { get; set; }
        public string WebAddress { get; set; }
        public string TelephoneNumber1 { get; set; }
        public bool Telephone1Use { get; set; }
        public string TelephoneNumber2 { get; set; }
        public bool Telephone2Use { get; set; }
        public string TelephoneNumber3 { get; set; }
        public string VatCodeId { get; set; }
        public string SalesLedgerCode { get; set; }
        public string OwningAgent { get; set; }
        public string SalesLedgerAccount { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }

    }
}
