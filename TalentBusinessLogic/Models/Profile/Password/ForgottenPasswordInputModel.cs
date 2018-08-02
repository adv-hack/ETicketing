
namespace TalentBusinessLogic.Models
{
    public class ForgottenPasswordInputModel : BaseInputModel
    {
        public string CustomerNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Token { get; set; }
        public string HashedToken { get; set; }
        public bool UserSignedIn { get; set; }
        public string Mode { get; set; }
        public bool DoTokenHashing { get; set; }
    }
}
