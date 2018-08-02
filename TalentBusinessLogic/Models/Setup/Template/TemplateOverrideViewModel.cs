using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talent.Common;
using TalentBusinessLogic.DataTransferObjects.Setup.Template;

namespace TalentBusinessLogic.Models.Setup.Template
{
    public class TemplateOverrideViewModel : BaseViewModel
    {
        /// <summary>
        /// parametered constructor
        /// </summary>
        /// <param name="getContentAndAttributes"></param>

        public TemplateOverrideViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes)
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TemplateOverrideViewModel()
        {
        }

        /// <summary>
        /// List of template overrides
        /// </summary>
        public List<TemplateOverrideHeader> TemplateOverrideList { get; set; }

        /// <summary>
        /// Businessunit list
        /// </summary>
        public Dictionary<string, string> BusinessUnitList { get; set; }

        /// <summary>
        /// list of ticketing override  criteria
        /// </summary>
        public List<TicketingOverrideCriteria> TicketingOverrideCriterias { get; set; }

        /// <summary>
        /// list of package override  criteria
        /// </summary>
        public List<PackageOverrideCriteria> PackageOverrideCriterias { get; set; }

        /// <summary>
        /// List of email confirmation templates
        /// </summary>
        public List<EmailConfirmationItem> EmailConfirmationList { get; set; }

        /// <summary>
        /// List of Q&A templates
        /// </summary>
        public List<QandAItem> QandAList { get; set; }

        /// <summary>
        /// List of data capture templates
        /// </summary>
        public List<DataCaptureItem> DataCaptureList { get; set; }
    }
}