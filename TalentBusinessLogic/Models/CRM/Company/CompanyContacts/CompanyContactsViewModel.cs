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
    public class CompanyContactsViewModel : BaseViewModel
    {
        public CompanyContactsViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public CompanyContactsViewModel() { }
        public List<CompanyContactModel> CompanyContacts { get; set; }
    }
    public class CompanyContactModel : BaseViewModel
    {
        public string CustomerNumber { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Salutation { get; set; }
        public string Telephone { get; set; }
    }
}
