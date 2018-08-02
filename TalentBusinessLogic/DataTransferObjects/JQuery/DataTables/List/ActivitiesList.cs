using System.Collections.Generic;

namespace TalentBusinessLogic.DataTransferObjects.JQuery.DataTables.List
{
    public class ActivitiesList
    {
        public int CustomerActivitiesHeaderID { get; set; }
        public string CustomerNumber { get; set; }
        public int TemplateID { get; set; }
        public string ActivityUserName { get; set; }
        public string DescriptiveUserName { get; set; }
        public string ActivityDate { get; set; }
        public string FormattedDate { get; set; }
        public string ActivitySubject { get; set; }
        public string CustomerName { get; set; }
        public string ActivityStatus { get; set; }
    }
}
