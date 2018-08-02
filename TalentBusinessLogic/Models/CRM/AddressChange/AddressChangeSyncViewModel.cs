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
    public class AddressChangeSyncViewModel : BaseViewModel
    {
        public AddressChangeSyncViewModel(BaseInputModel inputModel) : base(new WebFormResource(),true) { }
        public List<string> CustomerNumbers { get; set; }
        //public ErrorModel Error { get; set; }        
    }
}
