using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talent;
using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class ActivitiesTemplateInputModel : BaseInputModel
    {
        public List<DEBasketItem> basket { get; set; }
        public string BasketHeaderID { get; set; }
        public string CacheDependencyPath { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
    }
}
