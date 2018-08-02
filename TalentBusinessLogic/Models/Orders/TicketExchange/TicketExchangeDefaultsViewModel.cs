using System.Collections.Generic;
using Talent.Common;
using TalentBusinessLogic.DataTransferObjects;

namespace TalentBusinessLogic.Models
{
    public class TicketExchangeDefaultsViewModel : BaseViewModel
    {
        public string AllowTicketExchangeReturn { get; set; }
        public string ProductDescription { get; set; }
        public string AllowTicketExchangePurchase { get; set; }
        public decimal MinimumResalePrice { get; set; }
        public decimal MaximumResalePrice { get; set; }
        public decimal ClubFee { get; set; }
        public string ClubFeePercentageOrFixed { get; set; }
        public string MinMaxBoundaryPercentageOrFixed { get; set; }
        public string CustomerRetainsPrerequisite { get; set; }
        public string CustomerRetainsMaxLimit { get; set; }
        public int SumOfTicketsAllocatedSold { get; set; }
        public int SumOfTicketsBooked { get; set; }
        public decimal SeasonFromDate { get; set; }
        public decimal SeasonToDate { get; set; }
        public int SystemMaxTEPerCustomer { get; set; }
        public int ProductMaxTEPerCustomer { get; set; }
        public int SumOfTicketsPendingOnTicketExchange { get; set; }
        public int SumOfTicketsExpiredOnTicketExchange { get; set; }
        public int SumOfTicketsSoldOnTicketExchange { get; set; }
        public decimal ValueOfTicketsPendingOnTicketExchange { get; set; }
        public decimal ValueOfTicketsExpiredOnTicketExchange { get; set; }
        public decimal ValueOfTicketsSoldOnTicketExchange { get; set; }
        public decimal ValueOfTeFeesPendingOnTicketExchange { get; set; }
        public decimal ValueOfTeFeesExpiredOnTicketExchange { get; set; }
        public decimal ValueOfTeFeesSoldOnTicketExchange { get; set; }

        public TicketExchangeDefaultsViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public TicketExchangeDefaultsViewModel() { }
        public List<TicketExchangeStandAreaDefaults> TicketExchangeStandAreaDefaultsList { get; set; }
        public List<TicketExchangeProducts> TicketExchangeProductsList { get; set; }
    }

    public class TicketExchangeProducts
    {
        public string ProductCode { get; set; }
        public string ProductDescription  { get; set; }
        public decimal ProductDate { get; set; }
        public string ProductMask { get; set; }
    }
}
