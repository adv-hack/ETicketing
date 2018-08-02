using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;

namespace TalentBusinessLogic.Models
{
    public class AddressLabelInputModel : BaseInputModel
    {
        public string CustomerNumber { get; set; }
        public string SessionID { get; set; }
    }
}
