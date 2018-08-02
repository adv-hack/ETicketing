using System.Collections.Generic;

namespace TalentBusinessLogic.Models.Hospitality.Booking
{
   public  class HospitalityBookingEnquiryInputModel : BaseInputModel
    {
        //Properties used for basic create, read and delete package
        public string BoxOfficeUser { get; set; }
        public string LoggedInCustomerNumber { get; set; }
        public decimal CallID { get; set; }
        public decimal FromDate { get; set; }
        public decimal ToDate { get; set; }
        public string Status { get; set; }
        public string CustomerNumber { get; set; }
        public string Package { get; set; }
        public string ProductCode { get; set; }
        public string ProductGroup { get; set; }
        public decimal MaxRecords { get; set; }
        public string MarkOrderFor { get; set; }
        public string QandAStatus { get; set; }
        public string PrintStatus { get; set; }
        public List<string> CallIdList { get; set; }
        public string LoggedInBoxOfficeUser { get; set; }
        public long CallIdToBePrinted { get; set; }
        
        public long CallIdForDocumentProduction { get; set; }        
    }
}
