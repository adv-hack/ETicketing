using System.Collections.Generic;
using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class TicketExchangeProductsViewModel : BaseViewModel
    {
        public int TotalExpiredOnTicketExchange { get; set; }
		public int TotalPendingOnTicketExchange { get; set; } 
		public int TotalSoldOnTicketExchange { get; set; }
        public decimal TotalReSalePricePending { get; set; }
        public decimal TotalReSalePriceExpired { get; set; }
        public decimal TotalReSalePriceSold { get; set; }    
        public decimal TotalPotentialEarningPricePending { get; set; }
        public decimal TotalPotentialEarningPriceExpired { get; set; }
        public decimal TotalPotentialEarningPriceSold { get; set; }        
        public decimal TotalHandlingFeePending { get; set; }
        public decimal TotalHandlingFeeExpired { get; set; }
        public decimal TotalHandlingFeeSold { get; set; }
  
        public TicketExchangeProductsViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public TicketExchangeProductsViewModel() { }
        public List<TicketExchangeProductSummary> TicketExchangeProductSummaryList { get; set; }
    }
    public class TicketExchangeProductSummary
    {
        public string ProductDate { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductMaxTicketExchangeAllowedPerCustomer { get; set; }
        public string CompetitionDescription { get; set; }
        public int TotalPurchased { get; set; }
        public int TotalWithCustomer { get; set; }
        public int TotalPendingOnTicketExchange { get; set; }
        public int TotalSoldOnTicketExchange { get; set; }
        public decimal TotalPurchasedPrice { get; set; }
        public decimal TotalResalePricePending { get; set; }
        public decimal TotalResalePriceSold { get; set; }
        public decimal TotalPotentialEarningPricePending { get; set; }
        public decimal TotalPotentialEarningPriceSold { get; set; }
        public decimal TotalHandlingFeePending { get; set; }
        public decimal TotalHandlingFeeSold { get; set; }       
        public string  StatusCode { get; set; }
   } 
}
