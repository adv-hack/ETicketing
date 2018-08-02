using System.Collections.Generic;
using TalentBusinessLogic.DataTransferObjects.Hospitality;
using Talent.Common;
using TalentBusinessLogic.DataTransferObjects;

namespace TalentBusinessLogic.Models
{
    public class HospitalityBookingViewModel : BaseViewModel
    {
        public HospitalityBookingViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public HospitalityBookingViewModel(bool getContentAndAttributes, string pageCode) : base(new WebFormResource(), getContentAndAttributes, pageCode) { }
        public HospitalityBookingViewModel() { }
        public List<PackageDetails> PackageDetailsList { get; set; }
        public List<ComponentDetails> ComponentDetailsList { get; set; }
        public List<ActivityQuestionAnswer> ActivityQuestionAnswerList { get; set; }        
        public List<ComponentDetails> ExtraComponentDetailsList { get; set; }
        public bool CustomerMatched { get; set; }
        public bool PrintRequestSucceeded { get; set; }
        public bool HasPrintableTickets { get; set; }
        public int NumberOfTicketsToPrint { get; set; }
        public bool GenerateDocumentRequestSuccess { get; set; }
        public long CallId { get; set; }
        public string MergedWordDocumentPath { get; set; }
        public string PaymentOwnerCustomerForename { get; set; }
        public string PaymentOwnerCustomerSurname { get; set; }
    }
}
