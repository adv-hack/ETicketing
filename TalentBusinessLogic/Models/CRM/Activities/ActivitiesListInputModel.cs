
namespace TalentBusinessLogic.Models
{
    public class ActivitiesListInputModel : BaseInputModelForDataTables
    {
        public string CustomerActivitiesHeaderID { get; set; }
        public string CustomerNumber { get; set; }
        public int TemplateID { get; set; }
        public string ActivityUserName { get; set; }
        public string ActivityDate { get; set; }
        public string ActivitySubject { get; set; }
        public string ActivityStatus { get; set; }
    }
}
