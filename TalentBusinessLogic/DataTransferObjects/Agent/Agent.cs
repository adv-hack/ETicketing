namespace  TalentBusinessLogic.DataTransferObjects
{
    public class Agent
    {
        public string Usercode { get; set; }
        public string Username { get; set; }
        public string AgentType { get; set; }
        public string PrinterGroup { get; set; }
        public string DefaultTicketPrinter { get; set; }
        public string DefaultSmartcardPrinter { get; set; }
        public string OverridePrinterHome { get; set; }
        public string OverridePrinterEvent { get; set; }
        public string OverridePrinterTravel { get; set; }
        public string OverridePrinterSeasonticket { get; set; }
        public string OverridePrinterAddress { get; set; }
        public string PrintAddressLabelsDefault { get; set; }
        public string PrintTransactionReceiptsDefault { get; set; }
        public string Company { get; set; }
        public string BulkSalesMode { get; set; }
        public string Department { get; set; }
        public string PrinterNameDefault { get; set; }
        public string GroupId { get; set; }
        public string PrintAlways { get; set; }
        public string CaptureMethod { get; set; }
    }
}
