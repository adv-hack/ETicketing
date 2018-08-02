using System.Collections.Generic;

namespace TalentBusinessLogic.Models
{

    public class ClubModel : BaseViewModel
    {
        public string ClubCode { get; set; }
        public string ClubDescription { get; set; }
        public string DisplaySequence { get; set; }
        public bool IsDefault { get; set; }
        public string CustomerValidationUrl { get; set; }
        public string InvalidCustomerForwardUrl { get; set; }
        public string ValidCustomerForwardUrl { get; set; }
        public string NoiseEncryptionKey { get; set; }
        public string SupplynetLoginid { get; set; }
        public string SupplynetPassword { get; set; }
        public string SupplynetCompany { get; set; }
        public string AgentType { get; set; }
        public string StandardCustomerForwardUrl { get; set; }

    }
}
