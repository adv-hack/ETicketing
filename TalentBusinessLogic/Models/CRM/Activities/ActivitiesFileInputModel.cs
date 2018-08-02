
namespace TalentBusinessLogic.Models
{
    public class ActivitiesFileInputModel : BaseInputModelForFineUploader
    {
        public string CustomerActivitiesHeaderID { get; set; }
        public int TemplateID { get; set; }
        public string ActivityFileName { get; set; }
        public string ActivityFileDescription { get; set; }
        public string ActivityFileAttachmentID { get; set; }
        public string ActivityFileItemIndex { get; set; }
        public string SessionID { get; set; }
    }
}
