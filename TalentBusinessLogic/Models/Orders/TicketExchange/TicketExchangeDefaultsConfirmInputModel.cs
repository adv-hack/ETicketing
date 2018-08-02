using System.Collections.Generic;
using TalentBusinessLogic.DataTransferObjects;
namespace TalentBusinessLogic.Models
{
    public class TicketExchangeDefaultsConfirmInputModel : BaseInputModel
    {
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public bool AllowTicketExchangeReturn { get; set; }
        public bool AllowTicketExchangePurchase { get; set; }
        public decimal MinimumResalePrice { get; set; }
        public decimal MaximumResalePrice { get; set; }
        public decimal ClubFee { get; set; }
        public string ClubFeePercentageOrFixed { get; set; }
        public string MinMaxBoundaryPercentageOrFixed { get; set; }
        public bool CustomerRetainsPrerequisite { get; set; }
        public bool CustomerRetainsMaxLimit { get; set; }
        public List<TicketExchangeStandAreaDefaults> StandAreaDefaults { get; set; }
     
    }
}
