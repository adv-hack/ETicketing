namespace TalentBusinessLogic.DataTransferObjects.Product
{
    public class ProductPriceBandDetails 
    {
        public string Product  { get; set; }
        public string PriceBand { get; set; }
        public string PriceBandDescription { get; set; }
        public string PriceBandPriority { get; set; }
        public string PriceBandValue { get; set; }
        public string PriceBandMinQuantity { get; set; }
        public string PriceBandMaxQuantity { get; set; }
        public string PriceBandIsFamilyType { get; set; }
    }
}
