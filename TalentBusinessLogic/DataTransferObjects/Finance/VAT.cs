namespace TalentBusinessLogic.DataTransferObjects
{
    public class VAT
    {
            public double VATUniqueID { get; set; }
            public string VATCode { get; set; }
            public decimal VATRate { get; set; }
            public bool DefaultVATCode { get; set; }
    }
}
