using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;
using Talent.Common;
using TalentBusinessLogic.DataTransferObjects;

namespace TalentBusinessLogic.Models
{
    
    public class TicketExchangeSelectionViewModel : BaseViewModel
    {
        private const string TicketExchangeInfoCodeAreaNotAvailable = "I0";
        private const string TicketExchangeInfoCodeAreaNowNotAvailable = "I1";
        private const string TicketExchangeInfoCodeBoundaryChange = "I2";
        private const string TicketExchangeInfoFeeDefaultsChange = "I3";
        private const string TicketExchangeInfoSold = "I4";
        private const string TicketExchangeInfoItemSoldInBulk = "I5";
        private const string TicketExchangeInfoItemInABasket = "I6";
        private const string TicketExchangeInfoItemDisabledSeat = "I7";
        private const string TicketExchangeInfoItemCarerSeat = "I8";
        
        private const string PercentageType = "P";
        private const string FixedType = "F";
        private const decimal penny = 0.01m;

        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string ProductDate { get; set; }
        public decimal MainSliderMinPrice  { get; set; }
        public decimal MainSliderMaxPrice { get; set; }
        public decimal MainSliderInitialValue { get; set; }
        public string OverallProductFeeType { get; set; }
        public string OverallProductFeeValue { get; set; }

        public List<TicketExchangeItem> TicketExchangeSeatList { get; set; }
       
        public TicketExchangeSelectionViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public TicketExchangeSelectionViewModel() { }

        public void setOriginalWorkProperties() {
            bool MainSliderMinPriceSet = false;
            bool MainSliderMaxPriceSet = false;
            bool OverallFeeSetupSet = false;

            List<decimal> FaceValues = new List<decimal>();
            
            foreach (TicketExchangeItem item in TicketExchangeSeatList)
            {
                item.OriginalStatus = item.Status;
                if (item.RequestedPrice != 0) {
                    item.OriginalResalePrice = item.RequestedPrice;
                } else {
                    item.OriginalResalePrice = item.FaceValuePrice;
                }

                FaceValues.Add(item.FaceValuePrice);

                //Calculate the price selection bondaries based on product setup.
                item.MinPrice = calculateBoundaryValue(item.ProductMinPrice, item.FaceValuePrice, item.ProductMinMaxBoundaryType);
                item.MaxPrice = calculateBoundaryValue(item.ProductMaxPrice, item.FaceValuePrice, item.ProductMinMaxBoundaryType);

                //Evaluate if there are any reasons why the TE item cannot be amended or used.
                item.AllowStatusChange = isStatusChangeAllowed(item);
                item.AllowPriceChange = isPriceChangeAllowed(item);
                item.AdditionalInfo = setAdditionalInfo(item);

                // Overall main slider price selection boundaries
                if (!MainSliderMinPriceSet || item.MinPrice < MainSliderMinPrice ) {
                    MainSliderMinPrice = item.MinPrice;
                    MainSliderMinPriceSet = true;
                }
                if (!MainSliderMaxPriceSet || item.MaxPrice > MainSliderMaxPrice) {
                    MainSliderMaxPrice = item.MaxPrice;
                    MainSliderMaxPriceSet = true;
                }

                // If all products are the same then return a placeholder for the fee type and value (used for header label).
                // If the list contains different products with different fee set then we don't return anything for this header.
                if (!OverallFeeSetupSet)
                {
                    OverallProductFeeType = item.ProductClubHandlingFeeType;
                    OverallProductFeeValue = item.ProductClubHandlingFee.ToString();
                    OverallFeeSetupSet = true;
                }
                if (OverallFeeSetupSet && (OverallProductFeeValue != item.ProductClubHandlingFee.ToString() || OverallProductFeeType != item.ProductClubHandlingFeeType))
                {
                    OverallProductFeeValue = string.Empty;
                    OverallProductFeeType = string.Empty;
                }
            }

            // Retrieve the modal face value of all the TE items to set the main slider value.
            MainSliderInitialValue = FaceValues.GroupBy(x => x).OrderByDescending(x => x.Count()).ThenBy(x => x.Key).Select(x => (decimal)x.Key).FirstOrDefault();
        }

        private decimal calculateBoundaryValue(decimal productValue, decimal productFaceValue, String productBoundaryType) {
            decimal value;
            value = 0;
            if (productBoundaryType == FixedType) {
                value = productValue;
            }
            if (productBoundaryType == PercentageType) {
                value = productFaceValue * (productValue / 100);
            }
            return Math.Round(value, MidpointRounding.AwayFromZero);
        }

        private bool isPriceChangeAllowed(TicketExchangeItem item)
        {
            // Sold Items cannot be modified.
            if ((GlobalConstants.TicketExchangeItemStatus)item.OriginalStatus == GlobalConstants.TicketExchangeItemStatus.Sold)
            {
                return false;
            }

            // Items which are in areas which are no longer available for ticket exchange cannot be modified.
            if (!(item.ErrorCode == null) && !(item.ErrorCode.Trim() == string.Empty))
            {
                return false;
            }
            
            // Extra Check for items currently on ticket Exchange
            if ((GlobalConstants.TicketExchangeItemStatus)item.OriginalStatus == GlobalConstants.TicketExchangeItemStatus.OnSale)
            {
                // On Sale Items which have a price which is no longer in the valid boundaries cannot be modified.
                if (item.RequestedPrice < item.MinPrice || item.RequestedPrice > item.MaxPrice)
                {
                    return false;
                }

                // On Sale Items which have a calculated fee different to the product defaults  cannot be modified.
                if ((item.ProductClubHandlingFeeType == FixedType && item.Fee != item.ProductClubHandlingFee) ||
                    (item.ProductClubHandlingFeeType == PercentageType && Math.Abs(item.Fee - (item.ProductClubHandlingFee * item.RequestedPrice) / 100) > penny))
                {
                    return false;
                }
            }
            return true;
        }

        private bool isStatusChangeAllowed(TicketExchangeItem item)
        {
            //Sold Items cannot be modified.
            if ((GlobalConstants.TicketExchangeItemStatus)item.OriginalStatus == GlobalConstants.TicketExchangeItemStatus.Sold)
            {
                return false;
            }

            //If the item is currently in another basket it cannot be modified.
            if (!(item.ErrorCode == null) && !(item.ErrorCode.Trim() == string.Empty) && item.ErrorCode.Trim() == TicketExchangeInfoItemInABasket)
            {
                return false; 
            }

            //On Sale Items can always be modified
            if ((GlobalConstants.TicketExchangeItemStatus)item.OriginalStatus == GlobalConstants.TicketExchangeItemStatus.OnSale)
            {
                return true; 
            }

            // Items which are in areas which are no longer available for ticket exchange cannot be modified.
            // The exception is if the item is already on sale. The Customer can choose to bring the item back.
            if (!(item.ErrorCode == null) && !(item.ErrorCode.Trim() == string.Empty))
            {
                return false;
            }

            return true;
        }

        private string setAdditionalInfo(TicketExchangeItem item)
        {
            String additionalInfo = string.Empty;
            

            // Sold items cannot be modified.
            if ((GlobalConstants.TicketExchangeItemStatus)item.OriginalStatus == GlobalConstants.TicketExchangeItemStatus.Sold)
            {
                additionalInfo = GetPageText("TicketExchangeItemInfo" + TicketExchangeInfoSold);
                return additionalInfo;
            }

            // Items which are in areas which are no longer available for ticket exchange cannot be modified.
            // The exception is if the item is already on sale. The Customer can choose to bring the item back.
            if (!(item.ErrorCode == null) && !(item.ErrorCode.Trim() == string.Empty))
            {
                additionalInfo = GetPageText("TicketExchangeItemInfo" + item.ErrorCode.Trim());
                if (item.ErrorCode.Trim() == TicketExchangeInfoItemInABasket) {
                    return additionalInfo;
                }

                if ((GlobalConstants.TicketExchangeItemStatus)item.OriginalStatus == GlobalConstants.TicketExchangeItemStatus.OnSale) {
                    additionalInfo = GetPageText("TicketExchangeItemInfo" + TicketExchangeInfoCodeAreaNowNotAvailable);
                    return additionalInfo;
                }
            }

            // Extra Checks for items curently on sale
            if ((GlobalConstants.TicketExchangeItemStatus)item.OriginalStatus == GlobalConstants.TicketExchangeItemStatus.OnSale)
            {
                // On Sale Items which have a price which is no longer in the valid boundaries cannot be modified.
                if (item.RequestedPrice < item.MinPrice || item.RequestedPrice > item.MaxPrice)
                {
                    additionalInfo = GetPageText("TicketExchangeItemInfo" + TicketExchangeInfoCodeBoundaryChange);
                }

                // On Sale Items which have a calculated fee different to the product defaults  cannot be modified.
                if ((item.ProductClubHandlingFeeType == FixedType && item.Fee != item.ProductClubHandlingFee) ||
                    (item.ProductClubHandlingFeeType == PercentageType && Math.Abs(item.Fee - (item.ProductClubHandlingFee * item.RequestedPrice) / 100) > penny))
                {
                    additionalInfo = GetPageText("TicketExchangeItemInfo" + TicketExchangeInfoFeeDefaultsChange);
                }               
            }

            return additionalInfo;
        } 
    }  
}
