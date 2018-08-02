using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class ForgottenPasswordViewModel : BaseViewModel
    {
        public ForgottenPasswordViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }

        public ForgottenPasswordViewModel(bool getContentAndAttributes, string controlCode) : base(new UserControlResource(), getContentAndAttributes, controlCode) { }

        public ForgottenPasswordViewModel() { }
        public string CustomerNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Token { get; set; }
        public string HashedToken { get; set; }
        public bool UserSignedIn { get; set; }
        public string Mode { get; set; }
        public string Status { get; set; }
    }
}
