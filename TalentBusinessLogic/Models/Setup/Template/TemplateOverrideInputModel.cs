
using System.Collections.Generic;
using Talent.Common;
//using TalentBusinessLogic.DataTransferObjects.Setup.Template;

namespace TalentBusinessLogic.Models.Setup.Template
{
    public class TemplateOverrideInputModel : BaseInputModel
    {
        /// <summary>
        /// Businessunit for template override
        /// </summary>
        public string BusinessUnit { get; set; }

        /// <summary>
        /// Mode e.g Retrive,create,update,delete
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Template override id
        /// </summary>
        public decimal TemplateOverrideID { get; set; }

        /// <summary>
        /// template description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Sale confirmation template id
        /// </summary>
        public decimal SaleConfirmationEmailID { get; set; }

        /// <summary>
        /// Sale confirmation template description
        /// </summary>
        public string SaleConfirmationEmailDescription { get; set; }

        /// <summary>
        /// Q&A template id
        /// </summary>
        public decimal QAndATemplateID { get; set; }

        /// <summary>
        /// Q&A template description
        /// </summary>
        public string QAndATemplateDescription { get; set; }

        /// <summary>
        /// Data capture tempalte id
        /// </summary>
        public decimal DataCaptureTemplateID { get; set; }

        /// <summary>
        /// Data capture template decription 
        /// </summary>
        public string DataCaptureTemplateDescription { get; set; }

        /// <summary>
        /// auto expand Q&A
        /// </summary>
        public int AutoExpandQAndA { get; set; }

        /// <summary>
        /// Boxoffice user name
        /// </summary>
        public string BoxOfficeUser { get; set; }

        /// <summary>
        /// list of template override criterias for create/update
        /// </summary>
        public List<TemplateOverrideCriteria> TemplateOverrideCriterias { get; set; }
    }
}
