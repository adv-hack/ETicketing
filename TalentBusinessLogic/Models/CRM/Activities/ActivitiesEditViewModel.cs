using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class ActivitiesEditViewModel : BaseViewModel
    {
        public ActivitiesEditViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public ActivitiesEditViewModel(bool getContentAndAttributes, string controlCode) : base(new UserControlResource(), getContentAndAttributes, controlCode) { }
        public ActivitiesEditViewModel() { }

        public string CustomerActivitiesHeaderID { get; set; }
        public string ActivityCommentUpdatedDate { get; set; }
        public string ActivityCommentUpdatedTime { get; set; }
        public string ActivityDescriptiveUserName { get; set; }
        public string ActivityCommentItemIndex { get; set; }
        public string ActivityCommentID { get; set; }
        public string ActivityCommentText { get; set; }
        public string ActivityCommentBlurb { get; set; }
    }
}
