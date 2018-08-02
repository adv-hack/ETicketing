namespace  TalentBusinessLogic.DataTransferObjects
{
    public class TicketExchangeItem
    {
        // Seat & Seat Owner Details
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string ProductDate { get; set; }
        public string StandCode { get; set; }
        public string StandDescription { get; set; }
        public string AreaCode { get; set; }
        public string AreaDescription { get; set; }
        public string RowNo { get; set; }
        public string SeatNo { get; set; }
        public string AlphaSuffix { get; set; }
        public string SeatedCustomerNo { get; set; }
        public string SeatedCustomerName { get; set; }
        public decimal FaceValuePrice { get; set; }
        public decimal OriginalSalePrice { get; set; }
        public string IsSeasonTicketBookedSeat { get; set; }
        public string CustomerOwnsPurchase { get; set; }  

        // Payment & Payment Owner Details
        public int PaymentRef { get; set; }
        public string PaymentOwnerCustomerNo { get; set; }
        public string PaymentOwnerCustomerName { get; set; }

        // Ticket Exchange Details
        public int TicketExchangeId { get; set; }
        public decimal Fee { get; set; }
        public string FeeType { get; set; }
        public decimal RequestedPrice { get; set; }
        public decimal PotentialEarning { get; set; }
        public string ListedByCustomerNo { get; set; }
        public string ListedByCustomerName { get; set; }
        public string Comment1 { get; set; }
        public string Comment2 { get; set; }

        // Resale Price Conditions
        public decimal ProductMinPrice { get; set; }
        public decimal ProductMaxPrice { get; set; }
        public decimal ProductClubHandlingFee { get; set; }
        public string ProductClubHandlingFeeType { get; set; }
        public string ProductMinMaxBoundaryType { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        
        //0=Not For Sale, 1=For Sale, 2=Sold, 3=Placing On Sale, 4=Taking Off Sale, 5=Pricing Change 
        public int Status { get; set; }

        // Error Code if item cannot be amended on ticket exchange.
        public string ErrorCode { get; set; }

        // Work Variables
        public int OriginalStatus { get; set; }
        public decimal OriginalResalePrice { get; set; }
        public bool HasChanged { get; set; }
        public bool AllowStatusChange { get; set; }
        public bool AllowPriceChange { get; set; }
        public string AdditionalInfo { get; set; }

        public string ProductAndSeatCodes { 
            get {
            return ProductCode + StandCode + AreaCode + RowNo + SeatNo + AlphaSuffix;            
            } 
        }
    }
}
