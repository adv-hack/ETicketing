namespace TalentBusinessLogic.Models
{
    public class ProductAvailabilityInputModel : BaseInputModel
    {
        public string ProductCode { get; set; }
        public bool CapacityByStadium { get; set; }
        public string CampaignCode { get; set; }
        public string ProductType { get; set; }
        public string CATMode { get; set; }
        public string StadiumCode { get; set; }
        public string ComponentID { get; set; }
        public bool AgentLevelCacheForProductStadiumAvailability { get; set; }
        public long PriceBreakId { get; set; }
        public bool IncludeTicketExchangeSeats { get; set; }
        public decimal SelectedMinimumPrice { get; set; }
        public decimal SelectedMaximumPrice { get; set; }
        public string DefaultPriceBand { get; set; }
        public string StandCode { get; set; }
        public string AreaCode { get; set; }
    }
}
