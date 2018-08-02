using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class ActivitiesFileViewModel : BaseViewModelForFineUploader
    {
        public ActivitiesFileViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public ActivitiesFileViewModel(bool getContentAndAttributes, string controlCode) : base(new UserControlResource(), getContentAndAttributes, controlCode) { }
        public ActivitiesFileViewModel() : base() { }

        public string HTMLContent { get; set; }
        public string TemplateID { get; set; }
        public string ActivityFileDateAdded { get; set; }
        public string ActivityFileTimeAdded { get; set; }
        public string ActivityDescriptiveUserName { get; set; }
        public string ActivityFileItemIndex { get; set; }
        public string ActivityFileAttachmentID { get; set; }
        public string ActivityFileName { get; set; }
        public string ActivityFileLink { get; set; }
        public string ActivityFileAttachmentBlurb { get; set; }
        public string ActivityFileDescription { get; set; }
        public bool ActivityFileHasDescription { get; set; }

    }
}
