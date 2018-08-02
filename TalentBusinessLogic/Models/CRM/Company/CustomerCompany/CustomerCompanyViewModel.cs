using System.Collections.Generic;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;
using System.ComponentModel.DataAnnotations;
using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class CustomerCompanyViewModel : BaseViewModel 
    {
        public CustomerCompanyViewModel(bool getContentAndAttributes, string controlCode) : base(new UserControlResource(), getContentAndAttributes, controlCode) { }
         public CustomerCompanyViewModel() { }
         public CompanyModel Company { get; set; }
         //public ErrorModel Error { get; set; }
    }
}
