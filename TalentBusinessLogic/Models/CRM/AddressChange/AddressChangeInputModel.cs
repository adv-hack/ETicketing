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
    public class AddressChangeInputModel : BaseInputModel
    {
      
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
    }
}
