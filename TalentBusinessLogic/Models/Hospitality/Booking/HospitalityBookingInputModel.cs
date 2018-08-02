using System.Collections.Generic;
using Talent.Common;
using TalentBusinessLogic.DataTransferObjects;

namespace TalentBusinessLogic.Models
{
    public class HospitalityBookingInputModel : BaseInputModel
    {
        //Properties used for basic create, read and delete package
        public long CallId { get; set; }
        public long PackageID { get; set; }
        public string SeatComponentID { get; set; }
        public string ProductCode { get; set; }
        public string BasketID { get; set; }
        public string ProductType { get; set; }
        public string Quantity01 { get; set; }
        public string PriceBand01 { get; set; }
        public string CustomerNumber { get; set; }
        public string SavePackageMode { get; set; }
        public bool RefreshBasket { get; set; }

        //Properties used for amending a package
        public string BoxOfficeUser { get; set; }
        public OperationMode Mode { get; set; }
        public string ComponentGroupID { get; set; }
        public string ComponentID { get; set; }
        public string Status { get; set; }
        public bool MarkAsCompleted { get; set; }
        public decimal Discount { get; set; }
        public bool UpdateDiscount { get; set; }
        public bool RemoveAllDiscounts { get; set; }
        public string LeadSourceID { get; set; }
        public decimal PackageDiscountedByValue { get; set; }
        public List<AmendComponent> AmendComponents { get; set; }
        public List<SeatAllocation> SeatAllocations { get; set; }

        public bool IsSoldBooking { get; set; }
        public string MarkOrderFor { get; set; }
        public List<ActivityQuestionAnswer> ActivityQuestionAnswerList { get; set; }

        //properties used for printing
        public long CallIdToBePrinted { get; set; }
        public string ProductCodeToBePrinted { get; set; }
        public string SeatToBePrinted { get; set; }
        public long CallIdForDocumentProduction { get; set; }
    }
}
