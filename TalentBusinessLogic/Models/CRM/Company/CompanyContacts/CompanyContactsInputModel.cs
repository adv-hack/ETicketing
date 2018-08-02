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
    public class CompanyContactsInputModel : BaseInputModel
    {
        [RequiredAttribute(ErrorMessage = "FAIL")]
        public string CompanyNumber { get; set; }
    }
}
