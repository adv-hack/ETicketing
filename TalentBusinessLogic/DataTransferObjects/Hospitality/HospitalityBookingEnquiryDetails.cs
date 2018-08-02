
namespace TalentBusinessLogic.DataTransferObjects.Hospitality
{
    public class HospitalityBookingEnquiryDetails
    {
        public decimal BookingRef { get; set; }
        public decimal CustomerId { get; set; }
        public string TicketingCustomerMember { get; set; }
        public string PackageId { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public decimal ExpiryDate { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string ProductLabel { get; set; }
        public string ProductGroupCode { get; set; }
        public decimal ProcessDate { get; set; }
        public decimal ProcessTime { get; set; }
        public string ProcessedBy { get; set; }
        public string ProcessedByLabel { get; set; }
        public decimal PackageDefId { get; set; }
        public string PackageDescription { get; set; }
        public string PackageLabel { get; set; }
        public string PricedBy { get; set; }
        public decimal NettValueExclDiscount { get; set; }
        public decimal GrossValueExclDiscount { get; set; }
        public decimal NettValueInclDiscount { get; set; }
        public decimal GrossValueInclDiscount { get; set; }
        public decimal VatValueInclDiscount { get; set; }
        public decimal VatValueExclDiscount { get; set; }
        public decimal DiscountInclVat { get; set; }
        public decimal DiscountExclVat { get; set; }
        public string FormattedNettValueExclDiscount { get; set; }
        public string FormattedGrossValueExclDiscount { get; set; }
        public string FormattedNettValueInclDiscount { get; set; }
        public string FormattedGrossValueInclDiscount { get; set; }
        public string FormattedVatValueExclDiscount { get; set; }
        public string FormattedVatValueInclDiscount { get; set; }
        public string FormattedDiscountInclVat { get; set; }
        public string FormattedDiscountExclVat { get; set; }
        public string DiscountType { get; set; }
        public string FormattedDate { get; set; }
        public string BookingForwardingUrl { get; set; }
        public string PDFForwardingUrl { get; set; }
        public bool RequiresLogin { get; set; }
        public string QAndAStatus { get; set; }
        public string QAndAStatusDescription { get; set; }
        public bool RequiredQAndAReminder { get; set; }
        public decimal TicketsInBooking { get; set; }
        public decimal TicketsPrintedInBooking { get; set; }
        public string PrintStatus { get; set; }
    }
}