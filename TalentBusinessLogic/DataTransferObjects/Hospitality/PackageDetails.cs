using System.Collections.Generic;

namespace TalentBusinessLogic.DataTransferObjects.Hospitality
{
    public class PackageDetails
    {
        public long PackageID { get; set; }
        public string PackageDescription { get; set; }
        public string Comment1 { get; set; }
        public string Comment2 { get; set; }
        public string PricingMethod { get; set; }        
        public decimal Price { get; set; }
        public string DisplayPrice { get; set; }
        public string MaxQuantity { get; set; }
        public string VatValue { get; set; }
        public string PackageCode { get; set; }
        public bool AllowComments { get; set; }
        public decimal MaxPercentageToSellPWS { get; set; }        
        public int GroupSize { get; set; }
        public string GroupSizeDescription { get; set; }        
        public string OppositionCodesString { get; set; }
        public string CompetitionCodesString { get; set; }
        public string SubTypeCodesString { get; set; }                  
        public bool ShowViewFromArea { get; set; }
        public string ViewAreaImageUrl { get; set; }
        public string PackageType { get; set; }
        public decimal Discount { get; set; }
        public string Quantity { get; set; }
        public string FormattedPriceBeforeVAT { get; set; }
        public string FormattedVATPrice { get; set; }
        public string FormattedPriceIncludingVAT { get; set; }
        public decimal PriceBeforeVAT { get; set; }
        public decimal VATPrice { get; set; }
        public decimal PriceIncludingVAT { get; set; }
        public bool Completed { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public bool ActiveFlag { get; set; }
        public long AvailabilityComponentID { get; set; }
        public string LeadSourceID { get; set; }
        public string LeadSourceDescription { get; set; }
        public long CallID { get; set; }
        public bool PackageDiscountRemovedDueToPriceRecalc { get; set; }
        public decimal PackageDiscountedByValue { get; set; }
        public string FormattedPackageDiscountedByValue { get; set; }
        public AvailabilityDetails AvailabilityDetail { get; set; }
        public decimal PackageComponentLevelDiscountValue { get; set; }
        public string FormattedPackageComponentLevelDiscountValue { get; set; }
        public string CurrencySymbol { get; set; }
        public int Sold { get; set; }
        public int Reserved { get; set; }
        public int Cancelled { get; set; }
        public string MarkOrderFor { get; set; }
        public string CatMode { get; set; }
        public int TemplateID { get; set; }
        public bool ExpandAccordion { get; set; }
        public bool HasSTExceptions { get; set; }
        public string PreReqProductGroup { get; set; }
        public string PreReqDescription { get; set; }
        public string PreReqMultiGroup { get; set; }
        public string PreReqStadium { get; set; }
        public string PreReqValidationRule { get; set; }
        public string PreReqComments { get; set; }
        public string BookingStatus { get; set; }
        public string SoldOutProducts { get; set; }

        /// <summary>
        /// If product group package has sold out products
        /// </summary>
        public bool HasSoldOutProducts { get; set; }

        /// <summary>
        /// package product group code
        /// </summary>
        public string ProductGroupCode { get; set; }
    }
}
