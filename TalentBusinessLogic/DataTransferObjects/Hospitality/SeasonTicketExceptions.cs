using System;

namespace TalentBusinessLogic.DataTransferObjects.Hospitality
{
     public class SeasonTicketExceptions
    {
        public Int64 BasketHeaderId { get; set; }
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public string Module { get; set; }
        public string Seat { get; set; }
        public string SeasonTicketSeat { get; set; }
        public string SeasonTicketProduct { get; set; }
        public bool HasAvailability { get; set; }
        public string SeatType { get; set; }
        public string PriceBand { get; set; }
        public string LoginId { get; set; }
        public string ProductDescription1 { get; set; }
        public string ProductDescription2 { get; set; }
        public string ProductOppositionCode { get; set; }
        public string ProductDate { get; set; }
        public string ProductTime { get; set; }
        public bool Error { get; set; }
        public string PriceCode { get; set; }
        public string ProductType { get; set; }
        public string ProductSubType { get; set; }
        public string ProductTypeActual { get; set; }
        public string RestrictionCode { get; set; }
        public bool CannotApplyFees { get; set; }
        public string CATFlag { get; set; }
        public string StadiumCode { get; set; }
    }
}
