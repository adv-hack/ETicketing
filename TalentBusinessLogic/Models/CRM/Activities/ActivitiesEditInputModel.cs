
namespace TalentBusinessLogic.Models
{
    public class ActivitiesEditInputModel : BaseInputModel
    {
        public string CustomerActivitiesHeaderID { get; set; }
        public int TemplateID { get; set; }
        public string ActivityCommentID { get; set; }
        public string ActivityCommentText { get; set; }
        public string ActivityCommentItemIndex { get; set; }
        public string SessionID { get; set; }
    }
}
