using System.Collections.Generic;
using TalentBusinessLogic.DataTransferObjects;
using TalentBusinessLogic.DataTransferObjects.JQuery.DataTables.List;
using TalentBusinessLogic.Models.ApplicationModels.BaseModels;
using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class ActivitiesListViewModel : BaseViewModelForDataTables<ActivitiesList>
    { 
        public ActivitiesListViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public ActivitiesListViewModel() { }

        public List<Agent> AgentList { get; set; }
        public List<ActivityTemplate> TemplatesList { get; set; }
        public List<ActivityStatus> StatusList { get; set; }
        public string ActivityUserName { get; set; }
        public string ActivitySubject { get; set; }
        public string ActivityDate { get; set; }
        public string ActivityStatus { get; set; }
        public int TemplateID { get; set; }
    }
}
