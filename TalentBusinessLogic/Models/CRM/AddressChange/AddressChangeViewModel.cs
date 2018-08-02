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
    public class AddressChangeViewModel : BaseViewModel
    {
        public AddressChangeViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public AddressChangeViewModel() { }
        public List<AddressChangeModel> AddressChange { get; set; }
        //public ErrorModel Error { get; set; }
    }
    public class AddressChangeModel : BaseViewModel
    {
        public string CustomerNumber { get; set; }
        public string ForeName { get; set; }
        public string SurName { get; set; }
       
    }
}
