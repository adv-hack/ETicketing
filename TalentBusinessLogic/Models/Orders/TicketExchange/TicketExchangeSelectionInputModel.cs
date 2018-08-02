using System.Collections.Generic;
using TalentBusinessLogic.DataTransferObjects;

namespace TalentBusinessLogic.Models
{
    public class TicketExchangeSelectionInputModel : BaseInputModel
    {
        public string CustomerNumber { get; set; }
        public string ProductCode { get; set; }
    }
}
