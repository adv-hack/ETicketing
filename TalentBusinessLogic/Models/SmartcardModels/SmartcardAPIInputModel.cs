
namespace TalentBusinessLogic.Models
{
    public class SmartCardAPIInputModel : BaseInputModel
    {
        public string SessionID { get; set; }
        public string Mode { get; set; }
        public string ProductCode { get; set; }
        public string Stand { get; set; }
        public string Area { get; set; }
        public string Row { get; set; }
        public string Seat { get; set; }
        public string Aplha { get; set; }
        public int PaymentReference { get; set; }
        public string ExistingCardNumber { get; set; }
    }
}
