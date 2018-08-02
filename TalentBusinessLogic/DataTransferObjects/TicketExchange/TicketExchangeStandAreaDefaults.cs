namespace TalentBusinessLogic.DataTransferObjects
{
    public class TicketExchangeStandAreaDefaults
    {
        public string StandCode { get; set; }
        public string StandDescription { get; set; }
        public string AreaCode { get; set; }
        public string AreaDescription { get; set; }
        public string AreaDescriptionAdditional { get; set; }
        public int SumOfTicketsAllocatedSold { get; set; }
        public int SumOfTicketsBooked { get; set; }
        public int SumOfTicketsPendingOnTicketExchange { get; set; }
        public int SumOfTicketsExpiredOnTicketExchange { get; set; }
        public int SumOfTicketsSoldOnTicketExchange { get; set; }
        public decimal ValueOfTicketsPendingOnTicketExchange { get; set; }
        public decimal ValueOfTicketsExpiredOnTicketExchange { get; set; }
        public decimal ValueOfTicketsSoldOnTicketExchange { get; set; }
        public decimal ValueOfTeFeesPendingOnTicketExchange { get; set; }
        public decimal ValueOfTeFeesExpiredOnTicketExchange { get; set; }
        public decimal ValueOfTeFeesSoldOnTicketExchange { get; set; }
        public string AllowTicketExchangeReturn { get; set; }
        public string AllowTicketExchangePurchase { get; set; }
    }
}
