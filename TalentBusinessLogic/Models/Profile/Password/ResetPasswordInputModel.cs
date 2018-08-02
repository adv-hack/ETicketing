using System;

namespace TalentBusinessLogic.Models
{
    public class ResetPasswordInputModel : BaseInputModel
    {
        public string Token { get; set; }
        public string HashedToken { get; set; }
        public string CustomerNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTime DateNow { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Status { get; set; }
        public string Mode { get; set; }
        public bool IsDateValid { get; set; }
        public bool IsCustomerValid { get; set; }
        public bool IsTokenValid { get; set; }
        public bool IsValid { get; set; }
        public string NewPassword { get; set; }
        public string NewHashedPassword { get; set; }
        public string UserName { get; set; }
    }
}
