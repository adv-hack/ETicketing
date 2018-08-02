using System.Collections.Generic;
using Talent.Common;
using TalentBusinessLogic.DataTransferObjects;
using TalentBusinessLogic.DataTransferObjects.Hospitality;

namespace TalentBusinessLogic.Models.Hospitality.Booking
{
   public class HospitalityBookingEnquiryViewModel : BaseViewModel
    {
        public HospitalityBookingEnquiryViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public HospitalityBookingEnquiryViewModel() { }
        public List<HospitalityBookingEnquiryDetails> HospitalityBookingEnquiryList { get; set; }
        public List<Agent> AgentList { get; set; }
        public Dictionary<string,string> StatusList { get; set; }
        public Dictionary<string,string> MarkForOrderList { get; set; }
        public Dictionary<string,string> QandAStatusList { get; set; }
        public Dictionary<string, string> PrintStatusList { get; set; }
        public List<ActivityQuestionAnswer> ActivityQuestionAnswerList { get; set; }
        public bool SuccessfullySentQAndAReminder { get; set; }
        public List<string> CallIdList { get; set; }
        public decimal NumberOfBookings { get; set; }
        public decimal NumberOfTicketsToPrint { get; set; }    
        public bool PrintRequestSuccess { get; set; }
        public bool GenerateDocumentRequestSuccess { get; set; }
        public string MergedWordDocument { get; set; }
        public long CallIdForDocumentProduction { get; set; }
    }
}
