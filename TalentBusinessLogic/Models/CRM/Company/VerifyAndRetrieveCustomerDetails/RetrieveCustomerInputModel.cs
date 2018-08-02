using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;

namespace TalentBusinessLogic.Models
{
    public class VerifyAndRetrieveCustomerInputModel : BaseInputModel
    {
        public string CustomerNumber { get; set; }
        public string BasketID { get; set; }
        public string MembershipNumber { get; set; }
        public string NoiseEncryptionKey { get; set; }
        public bool PerformWatchListCheck { get; set; }
        public string CorporateSaleID { get; set; }
        public string PaymentReference { get; set; }
    }
}
