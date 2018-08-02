using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;

namespace TalentBusinessLogic.Models
{
    public class VerifyAndRetrieveCustomerViewModel : BaseViewModel
    {
        public bool Valid { get; set; }
        public string CustomerNumber { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyName { get; set; }
        public string CRMCompanyName { get; set; }
        public string WebReady { get; set; }
        public string PaymentReference { get; set; }
        //public ErrorModel Error { get; set; }
    }
}
