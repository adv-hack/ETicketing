using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;
using System.ComponentModel.DataAnnotations;

namespace TalentBusinessLogic.Models
{
    public class CustomerSearchInputModel : BaseInputModelForDataTables
    {
        public string ContactNumber { get; set; }
        public string ContactForename { get; set; }
        public string ContactSurname { get; set; }
        public string PassportNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string PostCode { get; set; }
        public string DateBirth { get; set; }
        public string WebAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Club { get; set; }
        public string AgentLoginID { get; set; }
        public string AgentType { get; set; }

    }
}
