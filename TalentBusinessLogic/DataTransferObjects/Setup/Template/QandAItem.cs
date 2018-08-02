using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBusinessLogic.DataTransferObjects.Setup.Template
{
    public class QandAItem
    {
        /// <summary>
        ///Unique Q and A Template Id
        ///</summary>
        public decimal QAndATemplateId { get; set; }

        /// <summary>
        ///Q and A template description
        ///</summary>
        public string QAndATemplateDescription { get; set; }
    }
}
