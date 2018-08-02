using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBusinessLogic.DataTransferObjects.Setup.Template
{
    public class TemplateOverrideCriteriaFormatted
    {
        /// <summary>
        /// Criteria type description for template override e.g PK=Pakage
        /// </summary>
        public String CriteriaTypeDescription { get; set; }

        /// <summary>
        /// Criteria values on which template is overridden
        /// </summary>
        public String CriteriaValues { get; set; }
        
    }
}
