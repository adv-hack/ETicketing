using TalentBusinessLogic.DataTransferObjects.Customer;
using TalentBusinessLogic.DataTransferObjects.Product;
using System.Collections.Generic;

namespace TalentBusinessLogic.DataTransferObjects.Hospitality
{
    public class HospitalitySeatDetails
    {
        public long ComponentID {get; set; }
        public string ComponentDescription { get; set; }
        public string ProductCode {get; set; }
        public string SeatDetails { get; set; }
        public string FormattedSeatDetails { get; set; }
        public string AlphaSuffix {get; set; }
        public string CustomerNumber { get; set; }
        public string PriceBand {get; set; }
        public string ValidPriceBands { get; set; }
        public string PriceCode { get; set; }
        public string ErrorCode { get; set; }
        public int MaxLimit { get; set; }
        public int UserLimit { get; set; }
        public string ErrorInformation { get; set; }
        public string RovingOrUnreserved { get; set; }
        public int BulkQuantity { get; set; }
        public long BulkID { get; set; }
        public bool CanAmendSeat { get; set; }
        public string StandCode { get; set; }
        public string AreaCode { get; set; }
        public string StandDescription { get; set; }
        public string AreaDescription { get; set; }
        public List<FriendsAndFamilyDetails> FriendsAndFamilyDetails { get; set; }
        public List<ProductPriceBandDetails> ProductPriceBands { get; set; }
        public List<ProductPriceCodeDetails> ProductPriceCodes { get; set; }
        public string SeatedComponentFormattedDisplay { get; set; }
        public List<SeasonTicketExceptions> SeasonTicketExceptions { get; set; }
        public string ComponentType { get; set; }
        public string DefaultProductPriceBand { get; set; }
    }
}
