using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class BaseViewModelForFineUploader : BaseViewModel
    {
        public BaseViewModelForFineUploader(WebFormResource resource, bool getContentAndAttributes) : base(resource, getContentAndAttributes) { }
        public BaseViewModelForFineUploader(UserControlResource resource, bool getContentAndAttributes, string controlCode) : base(resource, getContentAndAttributes, controlCode) { }
        public BaseViewModelForFineUploader() : base() { }

        public bool success { get; set; }
        public string error { get; set; }
        public bool preventRetry { get; set; }
        public string fileName { get; set; }
    }
}
