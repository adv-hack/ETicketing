using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBusinessLogic.DataTransferObjects.Setup.Template
{
    public class DataCaptureItem
    {
        /// <summary>
        ///Unique data capture template id
        ///</summary>
        public decimal DataCaptureTemplateId { get; set; }

        /// <summary>
        ///Data capture template description
        ///</summary>
        public string DataCaptureTemplateDescription { get; set; }
    }
}
