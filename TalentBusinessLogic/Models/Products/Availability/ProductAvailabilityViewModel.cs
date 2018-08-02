using System.Collections.Generic;
using Talent.Common;
using TalentBusinessLogic.DataTransferObjects;

namespace TalentBusinessLogic.Models
{
    /// <summary>
    /// This view model returns a set of lists (stand/areas, price breaks, price bands and prices) for a given product, based on the availability.
    /// The availability can be filtered based on a set of options such as price break, min price, max price and a ticket exchange flag
    /// </summary>
    public class ProductAvailabilityViewModel : BaseViewModel
    {
        public ProductAvailabilityViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public ProductAvailabilityViewModel(bool getContentAndAttributes, string controlCode) : base(new UserControlResource(), getContentAndAttributes, controlCode) { }
        public ProductAvailabilityViewModel() { }

        public List<StadiumAvailability> StadiumAvailabilityList { get; set; }
        public List<AvailableStands> AvailableStands { get; set; }
        public List<AvailableAreas> AvailableAreas { get; set; }
        public List<PriceBreakAvailability> PriceBreakAvailabilityList { get; set; }
        public List<PriceBandPrices> PriceBandPricesList { get; set; }
        public List<PriceBreakPrices> PriceBreakPrices { get; set; }
        public string StadiumXml { get; set; }
    }
}