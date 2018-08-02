using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talent.Common;
using System.Data;
using TalentBusinessLogic.BusinessObjects;
using TalentBusinessLogic.BusinessObjects.Definitions;

namespace TalentBusinessLogic.BusinessObjects
{
    public class Activities : BusinessObjects
    {
        /// <summary>
        /// Sets all activites templates
        /// </summary>
        /// <returns>templates string</returns>
        public List<DataTransferObjects.ActivityTemplateQA> SetActivitiesTemplates(List<DEBasketItem> basket, string basketHeaderID, string cacheDependencyPath, string userName, string fullName)
        {
            List<string> uniqueProducts = new List<string>();
            //string templates = string.Empty;
            List<DataTransferObjects.ActivityTemplateQA> templatesList = new List<DataTransferObjects.ActivityTemplateQA>();
            List<string> uniqueComplimentaryProducts = new List<string>();

            foreach (DEBasketItem basketItem in basket)
            {
                if (!Fees.IsTicketingFee(basketItem.MODULE_, basketItem.Product, basketItem.FEE_CATEGORY))
                {
                    string templateID = new ActivitiesDefinition().GetTicketingProductDetailsTemplateID(basketItem.Product, basketItem.PRICE_CODE, 30, cacheDependencyPath);
                    string complimentaryTemplateID = "0";
                    DataTable complimentaryTemplateTable = TDataObjects.ActivitiesSettings.TblActivityTemplates.GetByTemplateName("Complimentary", Environment.Settings.BusinessUnit);

                    DataTable template = TDataObjects.ActivitiesSettings.TblActivityTemplates.GetByTemplateID(templateID, true);

                    bool isTemplatePerProduct = false;
                    bool isComplimentaryTemplatePerProduct = false;

                    if (complimentaryTemplateTable.Rows.Count > 0)
                    {
                        complimentaryTemplateID = (complimentaryTemplateTable.Rows[0]["TEMPLATE_ID"]).ToString();
                    }

                    if (template.Rows.Count > 0)
                    {
                        isTemplatePerProduct = Data.Validation.CheckForDBNull_Boolean_DefaultFalse(template.Rows[0]["TEMPLATE_PER_PRODUCT"]);
                    }

                    if (!Convert.ToInt32(templateID).Equals(0))
                    {
                        if (isTemplatePerProduct)
                        {
                            if (!uniqueProducts.Contains(basketItem.Product))
                            {
                                SetActivityTemplate(basketItem, ref templatesList, false, 0, templateID, basketHeaderID, isTemplatePerProduct.ToString(), userName, fullName);
                                uniqueProducts.Add(basketItem.Product);
                            }
                        }
                        else
                        {
                            SetActivityTemplate(basketItem, ref templatesList, false, 0, templateID, basketHeaderID, isTemplatePerProduct.ToString(), userName, fullName);
                        }
                    }
                    //Need to see if we should add a complimentary template for this product
                    checkComplimentaryTemplateEnabled(complimentaryTemplateID, basketItem, ref templatesList, basketHeaderID, userName, fullName, isComplimentaryTemplatePerProduct, ref uniqueComplimentaryProducts);

                }
            }

            return templatesList;

        }

        private void checkComplimentaryTemplateEnabled(string complimentaryTemplateID, DEBasketItem basketItem, ref List<DataTransferObjects.ActivityTemplateQA> templatesList, string basketHeaderID, string userName, string fullName, bool isComplimentaryTemplatePerProduct, ref List<string> uniqueComplimentaryProducts)
        {
            List<string> complimentaryBandList = new List<string>(new string[] { "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            bool priceCodeFoc = false;
            

            
            //Check to see if this is a complimentary ticket, if so, add the template ID to the context
            if (!Convert.ToInt32(complimentaryTemplateID).Equals(0))
            {
                DataTable template = TDataObjects.ActivitiesSettings.TblActivityTemplates.GetByTemplateID(complimentaryTemplateID, true);
                if (template.Rows.Count > 0)
                {
                    isComplimentaryTemplatePerProduct = Data.Validation.CheckForDBNull_Boolean_DefaultFalse(template.Rows[0]["TEMPLATE_PER_PRODUCT"]);
                }

                TalentBusinessLogic.BusinessObjects.Environment.ActivityTemplateDefaults activityTemplateDefaults = new TalentBusinessLogic.BusinessObjects.Environment.ActivityTemplateDefaults();
                TalentBusinessLogic.BusinessObjects.Environment.ActivityTemplateDefaults.DefaultValues actDef = activityTemplateDefaults.GetDefaults(Convert.ToInt32(complimentaryTemplateID));
                priceCodeFoc = isPriceCodeFoC(basketItem);
                if (actDef.ComplimentaryCheckEnabled)
                {
                    //Check defaults against basket
                    if ((complimentaryBandList.Contains(basketItem.PRICE_BAND) & actDef.DisplayOnPriceBand) | (priceCodeFoc & actDef.DisplayOnPriceCode))
                    {
                        //Mirror above logic for templates per product. This time for complimentary
                        if (isComplimentaryTemplatePerProduct)
                        {
                            if (!uniqueComplimentaryProducts.Contains(basketItem.Product))
                            {

                                SetActivityTemplate(basketItem, ref templatesList, true, Convert.ToInt32(complimentaryTemplateID), string.Empty, basketHeaderID, isComplimentaryTemplatePerProduct.ToString(), userName, fullName);
                                uniqueComplimentaryProducts.Add(basketItem.Product);
                            }
                        }
                        else
                        {
                            SetActivityTemplate(basketItem, ref templatesList, true, Convert.ToInt32(complimentaryTemplateID), string.Empty, basketHeaderID, isComplimentaryTemplatePerProduct.ToString(), userName, fullName);
                        }
                    }
                }
            }
            
        }
        /// <summary>
        /// Checks to see if the price code of a basketItem is a free of charge price code
        /// </summary>
        /// <param name="basketItem"></param>
        /// <returns>a boolean</returns>
        private bool isPriceCodeFoC(DEBasketItem basketItem)
        {
            bool priceCodeFoc = false;
            ErrorObj err = new ErrorObj();
            TalentProduct talProduct = new TalentProduct();
            talProduct.Settings = Environment.Settings.DESettings;
            talProduct.De.ProductCode = basketItem.Product;
            talProduct.De.PriceCode = basketItem.PRICE_CODE;
            talProduct.De.AllowPriceException = false;
            err = talProduct.ProductDetails();
            if (!err.HasError)
            {
                if (talProduct.ResultDataSet != null && talProduct.ResultDataSet.Tables["PriceCodes"].Rows.Count > 0)
                {
                    foreach (DataRow row in talProduct.ResultDataSet.Tables["PriceCodes"].Rows)
                    {
                        if (row["PriceCode"].ToString() == basketItem.PRICE_CODE)
                        {
                            priceCodeFoc = Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(row["FreeOfCharge"]);
                            break; 
                        }
                    }
                }
            }
            return priceCodeFoc;

        }

        /// <summary>
        /// Sets session values for individual templates
        /// </summary>
        private void SetActivityTemplate(DEBasketItem item, ref List<DataTransferObjects.ActivityTemplateQA> templatesList, bool isComplimentary, int complimentaryTemplateID, string templateID, string basketHeaderID, string isTemplatePerProduct, string userName, string fullName)
        {
            if (isComplimentary)
            {
                string compTemplateID = complimentaryTemplateID.ToString();
                templatesList.Add(PopulateActivityTemplateQA(compTemplateID, item.LOGINID, userName, item.PRODUCT_DESCRIPTION1, "", "", item.PRODUCT_DESCRIPTION4, item.PRODUCT_DESCRIPTION5, item.Product, item.SEAT, item.PRICE_BAND, basketHeaderID, fullName, isTemplatePerProduct));
            }
            else
            {
                //show timeslot on Travel Product not default product time  
                if (Data.Validation.CheckForDBNull_String(item.PRODUCT_TYPE_ACTUAL) == GlobalConstants.TRAVELPRODUCTTYPE.ToString())
                {
                    string desc5 = item.PRODUCT_DESCRIPTION5;
                    if (!string.IsNullOrWhiteSpace(item.SEAT) && item.SEAT.Length >= 6)
                    {
                        desc5 = item.SEAT.Substring(0, 6).ToString();
                    }
                    templatesList.Add(PopulateActivityTemplateQA(templateID, item.LOGINID, userName, item.PRODUCT_DESCRIPTION1, "", "", item.PRODUCT_DESCRIPTION4, desc5, item.Product, item.SEAT, item.PRICE_BAND, basketHeaderID, fullName, isTemplatePerProduct));

                }
                else
                {
                    templatesList.Add(PopulateActivityTemplateQA(templateID, item.LOGINID, userName, item.PRODUCT_DESCRIPTION1, "", "", item.PRODUCT_DESCRIPTION4, item.PRODUCT_DESCRIPTION5, item.Product, item.SEAT, item.PRICE_BAND, basketHeaderID, fullName, isTemplatePerProduct));
                }
            }

        }

        private DataTransferObjects.ActivityTemplateQA PopulateActivityTemplateQA(string templateID, string loginID, string userName, string productDesc1, string productDesc2, string productDesc3, string productDesc4, string productDesc5, string product, string seat, string priceBand, string basketHeaderID, string fullName, string isTemplatePerProduct)
        {
            DataTransferObjects.ActivityTemplateQA actTemplateQA = new DataTransferObjects.ActivityTemplateQA();
            actTemplateQA.TemplateID = templateID;
            actTemplateQA.LoginID = loginID;
            actTemplateQA.UserName = userName;
            actTemplateQA.ProductDescription1 = productDesc1;
            actTemplateQA.ProductDescription2 = productDesc2;
            actTemplateQA.ProductDescription3 = productDesc3;
            actTemplateQA.ProductDescription4 = productDesc4;
            actTemplateQA.ProductDescription5 = productDesc5;
            actTemplateQA.Product = product;
            actTemplateQA.Seat = seat;
            actTemplateQA.BasketHeaderID = basketHeaderID;
            actTemplateQA.PriceBand = priceBand;
            actTemplateQA.FullName = fullName;
            actTemplateQA.IsTemplatePerProduct = isTemplatePerProduct;

            return actTemplateQA;

        }
    }
}
