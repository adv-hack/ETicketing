using System.Collections.Generic;
using TalentBusinessLogic.DataTransferObjects;

namespace TalentBusinessLogic.Models
{
    public class TicketExchangeInputModel : BaseInputModel
    {
        public string ListingCustomerNumber { get; set; }
        public List<TicketExchangeItem> Tickets { get; set; }
    }
}
