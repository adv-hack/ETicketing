using System.Collections.Generic;

namespace TalentBusinessLogic.DataTransferObjects.Hospitality
{
    public class ProductDetails
    {
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string ProductDate { get; set; }
        public string ProductDateSearch { get; set; }
        public string ProductYear { get; set; }
        public string ProductTime { get; set; }
        public string ProductOnSale { get; set; }
        public string ProductAvailForSale { get; set; }
        public string ProductReqdMem { get; set; }
        public string ProductReqdMemDesc { get; set; }
        public string ProductReqdMemOK { get; set; }
        public string ProductType { get; set; }
        public string ProductReqdLoyalityPoints { get; set; }
        public string ProductMDTE08 { get; set; }
        public string ProductPriceBand { get; set; }
        public string ProductTicketLimit { get; set; }
        public string ProductEntryTime { get; set; }
        public string ProductAssocTravelProd { get; set; }
        public string ProductOppositionCode { get; set; }
        public string ProductOppositionDesc { get; set; }
        public string ProductCompetitionCode { get; set; }
        public string ProductCompetitionDesc { get; set; }
        public string ProductDescription2 { get; set; }
        public string ProductStadium { get; set; }
        public string CampaignCode { get; set; }
        public string ProductSponsorCode { get; set; }
        public bool UseVisualSeatLevelSelection { get; set; }
        public string PPSSchemeType { get; set; }
        public string LoyaltyRequirementMet { get; set; }
        public string LimitRequirementMet { get; set; }
        public string ProductHomeAsAway { get; set; }
        public string ProductSubType { get; set; }
        public string ProductSubTypeDesc { get; set; }
        public bool HasAssociatedTravelProduct { get; set; }
        public bool IsSoldOut { get; set; }
        public string PriceCode { get; set; }
        public bool AlternativeSeatSelection { get; set; }
        public bool AlternativeSeatSelectionAcrossStands { get; set; }
        public bool IsProductBundle { get; set; }
        public bool RestrictGraphical { get; set; }
        public string ProductStadiumDescription { get; set; }
        public string location { get; set; }
        public string BundleStartDate { get; set; }
        public string BundleEndDate { get; set; }
        public bool ExcludeProductFromWebSales { get; set; }
        public bool HideDate { get; set; }
        public bool HideTime { get; set; }
        public List<string> Packages { get; set; }     
        public AvailabilityDetails AvailabilityDetail { get; set; }
        public decimal DataCaptureTemplateID { get; set; }
        // Hospitality product group code
        public string GroupId { get; set; }
        // Hospitality product group foramtted date
        public string FormattedDate { get; set; }
    }
}
