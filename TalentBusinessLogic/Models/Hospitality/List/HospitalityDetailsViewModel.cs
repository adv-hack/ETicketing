using System.Collections.Generic;
using TalentBusinessLogic.DataTransferObjects.Hospitality;
using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class HospitalityDetailsViewModel : BaseViewModel
    {
        //Product and Package properties
        public HospitalityDetailsViewModel(bool getContentAndAttributes, string controlCode) : base(new UserControlResource(), getContentAndAttributes, controlCode) { }
        public HospitalityDetailsViewModel() { }
        public string ProductOppositionCode { get; set; }
        public string ProductDescription { get; set; }
        public string PackageDescription { get; set; }
        public string ProductDate { get; set; }
        public string ProductTime { get; set; }
        public string ProductCompetitionDescription { get; set; }
        public List<LeadSourceDetails> LeadSourceDetails { get; set; }
        public string PackageCode { get; set; }
        public string PackageID { get; set; }
        public string FormattedPackagePrice { get; set; }
        public bool HideDate { get; set; }
        public bool HideTime { get; set; }
        public string ProductType { get; set; }
        public string ProductSubType { get; set; }
        public string ProductSubTypeDesc { get; set; }
        public decimal MaxDiscountPercent { get; set; }
        public string ProductStadium { get; set; }
        public bool AllowBooking { get; set; }
        public bool AllowDataCapture { get; set; }
        public decimal DataCaptureTemplateID { get; set; }

        //Customer properties
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string CustomerAddress { get; set; }
        public string MobileNumber { get; set; }
        public string HomeNumber { get; set; }
        public string WorkNumber { get; set; }

        //Call ID to aid display of the view model, not set in the builder class
        public string CallId { get; set; }
    }
}
