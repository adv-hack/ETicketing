using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class ResetPasswordViewModel : BaseViewModel
    {
        public ResetPasswordViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }

        public ResetPasswordViewModel(bool getContentAndAttributes, string controlCode) : base(new UserControlResource(), getContentAndAttributes, controlCode) { }

        public ResetPasswordViewModel() { }
        public string Token { get; set; }
        public string CustomerNumber { get; set; }
        public string DateNow { get; set; }
        public string ExpireDate { get; set; }
        public bool IsDateValid { get; set; }
        public bool IsCustomerValid { get; set; }
        public bool IsTokenValid { get; set; }
        public string Mode { get; set; }
        public bool IsValid { get; set; }

    }
}
