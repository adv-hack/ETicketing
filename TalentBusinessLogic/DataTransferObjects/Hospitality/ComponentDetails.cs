using System;
using System.Collections.Generic;

namespace TalentBusinessLogic.DataTransferObjects.Hospitality
{
    public class ComponentDetails
    {
        public long ComponentID { get; set; }
        public long PackageID { get; set; }
        public string ComponentCode { get; set; }
        public string ComponentDescription { get; set; }
        public string Comment1 { get; set; }
        public decimal UnitPrice { get; set; }
        public string FormattedUnitPrice { get; set; }
        public string NumberOfUnits { get; set; }
        public bool IsSeatComponent { get; set; }
        public string MinUnits { get; set; }
        public string MaxUnits { get; set; }
        public decimal MaxDiscountPercent { get; set; }
        public int AvailableUnitsPWS { get; set; } //Units of package available based on qty of this component available for PWS
        public int AvailableUnitsBUI { get; set; } //Units of package available based on qty of this component available for BUI
        public int OriginalUnits { get; set; } //Units of package available based on original qty of this component
        public string Type { get; set; }
        public decimal Discount { get; set; } // Discount %
        public decimal DiscountValue { get; set; } // Discount Value
        public string FormattedPriceBeforeDiscount { get; set; }
        public string Quantity { get; set; }
        public decimal PriceBeforeVAT { get; set; }
        public decimal PriceBeforeVATExclDisc { get; set; }
        public decimal VATPrice { get; set; }
        public decimal PriceIncludingVAT { get; set; }
        public string FormattedPriceBeforeVAT { get; set; }
        public string FormattedVATPrice { get; set; }
        public string FormattedPriceIncludingVAT { get; set; }
        public string FormattedPriceBeforeVATExclDisc { get; set; }
        public bool Completed { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public bool ActiveFlag { get; set; }
        public bool AllocateAutomatically { get; set; }
        public int ComponentGroupSequence { get; set; }
        public long ComponentGroupID { get; set; }
        public int ComponentSequence { get; set; }
        public string ComponentType { get; set; }
        public string ComponentGroupType { get; set; }
        public string ProductCode { get; set; }
        public string AreaInError { get; set; }
        public bool Proceed { get; set; }
        public int MaxQty { get; set; }
        public int MinQty { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool CanAmendSeat { get; set; }
        public List<HospitalitySeatDetails> HospitalitySeatDetailsList { get; set; }
        public bool AvailabilityFlag { get; set; }
        public bool PWSFlag { get; set; }
        public bool HasPrintableTicket { get; set; }
        public bool IsExtraComponent { get; set; }
        public bool HideSeatForPWS { get; set; }
    }
}
