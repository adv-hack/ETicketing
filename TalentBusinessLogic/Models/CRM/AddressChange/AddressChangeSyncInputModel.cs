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
    public class AddressChangeSyncInputModel : BaseInputModel
    {

        public string CustomerList { get; set; }
        public string CustomerNo { get; set; }
        public string OriginalAddress { get; set; }
        public string NewAddress { get; set; }
     
    }
}
