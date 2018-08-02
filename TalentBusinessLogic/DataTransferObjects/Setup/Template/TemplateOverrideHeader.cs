using System;
using System.Collections.Generic;
using Talent.Common;

namespace TalentBusinessLogic.DataTransferObjects.Setup.Template
{
    public class TemplateOverrideHeader
    {
        /// <summary>
        /// Template override Id
        /// </summary>
        public decimal TemplateOverrideId { get; set; }

        /// <summary>
        /// Template description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Businessunit for  template override
        /// </summary>
        public string BusinessUnit { get; set; }

        /// <summary>
        /// Sale confirmation email templateId
        /// </summary>
        public decimal SaleConfirmationEmailId { get; set; }

        /// <summary>
        /// Sale confirmation email template description
        /// </summary>
        public string SaleConfirmationEmailDescription { get; set; }

        /// <summary>
        /// Q&A template id
        /// </summary>
        public decimal QAndATemplateId { get; set; }

        /// <summary>
        /// Q&A template description
        /// </summary>
        public string QAndATemplateDescription { get; set; }

        /// <summary>
        /// Data capture template id
        /// </summary>
        public decimal DataCaptureTemplateId { get; set; }

        /// <summary>
        /// Data capture template decsription
        /// </summary>
        public string DataCaptureTemplateDescription { get; set; }

        /// <summary>
        /// Auto expand Q&A
        /// </summary>
        public Int32 AutoExpandQAndA { get; set; }

        /// <summary>
        /// Template override criterias for template override
        /// </summary>
        public List<TemplateOverrideCriteria> TemplateOverrideCriterias { get; set; }

        /// <summary>
        /// Formatted template override criterias
        /// </summary>
        public List<TemplateOverrideCriteriaFormatted> TemplateOverrideCriteriasFormatted { get; set; }

        /// <summary>
        /// Is the template product/package specific?
        /// </summary>
        public bool ProductPacakgeSpecific { get; set; }
       
        /// <summary>
        /// Product Criterias for specific template
        /// </summary>
        public string ProductCriterias { get; set; }
       
        /// <summary>
        /// Package Criterias for specific template
        /// </summary>
        public string PackageCriterias { get; set; }

        /// <summary>
        /// Product SubType Criterias for specific template
        /// </summary>
        public string ProductSubTypeCriterias { get; set; }

        /// <summary>
        /// Product Type Criterias for specific template
        /// </summary>
        public string ProductTypeCriterias { get; set; }

        /// <summary>
        /// Stadium Criterias for specific template
        /// </summary>
        public string StadiumCriterias { get; set; }
    }
}
