using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;
using System.ComponentModel.DataAnnotations;
using Talent.Common;
namespace TalentBusinessLogic.Models
{
    public class CompanyOperationsInputModel : BaseInputModel
    {
        public string CompanyNumber { get; set; }
        public string PreviousCompanyNumber { get; set; }
        public GlobalConstants.CRUDOperationMode CompanyOperationMode { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string WebAddress { get; set; }
        public string TelephoneNumber1 { get; set; }
        public bool Telephone1Use { get; set; }
        public string TelephoneNumber2 { get; set; }
        public bool Telephone2Use { get; set; }
        public string VATCodeId { get; set; }
        public string SalesLedgerCode { get; set; }
        public string OwningAgent { get; set; }
        public string SalesLedgerAccount { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
    }

    
}
