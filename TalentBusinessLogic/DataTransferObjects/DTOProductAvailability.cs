namespace TalentBusinessLogic.DataTransferObjects
{
    public class StadiumAvailability
    {
        public string StandCode { get; set; }
        public string StandDescription { get; set; }
        public string AreaCode { get; set; }
        public string AreaDescription { get; set; }
        public string Roving { get; set; }
        public string Availability { get; set; }
        public string AdditionalText { get; set; }
        public string Capacity { get; set; }
        public string Reserved { get; set; }
        public string SeatSelection { get; set; }
        public bool TicketExchangeAllowPurchase { get; set; }
        public string MinSliderPrice { get; set; }
        public string MaxSliderPrice { get; set; }
    }

    public class PriceBreakAvailability
    {
        public long PriceBreakId { get; set; }
        public string PriceBreakDescription { get; set; }
    }

    public class AvailableStands
    {
        public string StandCode { get; set; }
    }

    public class AvailableAreas
    {
        public string AreaCode { get; set; }
    }

    public class PriceBandPrices
    {
        public string PriceBand { get; set; }
        public string PriceBandDescription { get; set; }
        public string PriceBandPrice { get; set; }
    }

    public class PriceBreakPrices
    {
        public string Price { get; set; }
        public string DisplayPrice { get; set; }
    }
}
