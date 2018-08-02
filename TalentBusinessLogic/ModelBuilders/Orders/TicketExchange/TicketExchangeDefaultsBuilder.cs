using System;
using TalentBusinessLogic.Models;
using Talent.Common;
using AutoMapper;
using System.Data;
using System.Linq;
using TalentBusinessLogic.DataTransferObjects;
using System.Collections.Generic;
using System.Text;


namespace TalentBusinessLogic.ModelBuilders
{
    public class TicketExchangeDefaultsBuilder : BaseModelBuilder
    {

        private WebFormResource _pageResource = new WebFormResource();

        private DESettings _settings;

        public TicketExchangeDefaultsViewModel GetTicketExchangeDefaults(TicketExchangeDefaultsInputModel inputModel)
        {

            _settings = Environment.Settings.DESettings;
            _pageResource.BusinessUnit = _settings.BusinessUnit;
            _pageResource.PartnerCode = _settings.Partner;
            _pageResource.PageCode = Environment.Settings.Page;
            _pageResource.KeyCode = Environment.Settings.Page;
            _pageResource.FrontEndConnectionString = _settings.FrontEndConnectionString;
            int YearsOfPastProductsToShow = Data.Validation.CheckForDBNull_Int(_pageResource.get_Attribute("YearsOfPastProductsToShow", "ENG", true));

            TicketExchangeDefaultsViewModel viewModel = new TicketExchangeDefaultsViewModel(true);

            TalentTicketExchange ticketExchange = new TalentTicketExchange();
            ticketExchange.Dep.ProductCode = inputModel.ProductCode;
            ticketExchange.Dep.ReturnAllProducts = inputModel.ReturnAllProducts;

            ticketExchange.Settings = _settings;
            ErrorObj err = ticketExchange.GetTicketExchangeDefaults();

            viewModel.Error = Data.PopulateErrorObject(err, ticketExchange.ResultDataSet, ticketExchange.Settings, 3);

            if (!viewModel.Error.HasError)
            {
                if (ticketExchange.ResultDataSet.Tables["ProductTESummary"].Rows.Count > 0)
                {
                    // Product setting Values from "ProductTESummary" 
                    viewModel = Data.PopulateObjectFromRow<TicketExchangeDefaultsViewModel>(ticketExchange.ResultDataSet.Tables["ProductTESummary"].Rows[0], viewModel);

                    // List of stand ae area defaults from "StandAreaTESummary"
                    if (ticketExchange.ResultDataSet.Tables["StandAreaTESummary"].Rows.Count > 0)
                    {
                        viewModel.TicketExchangeStandAreaDefaultsList = Data.PopulateObjectListFromTable<TicketExchangeStandAreaDefaults>(ticketExchange.ResultDataSet.Tables["StandAreaTESummary"]);
                    }
                    var ProductCodeDateDescriptionMask = (viewModel.GetPageText("ProductCodeDateDescriptionMask"));
                    viewModel.TicketExchangeProductsList = GetTicketExchangeProducts(YearsOfPastProductsToShow, ProductCodeDateDescriptionMask);
                }
            }

            return viewModel;
        }

        private List<TicketExchangeProducts> GetTicketExchangeProducts(int YearsOfPastProductsToShow, string ProductCodeDateDescriptionMask)
        {
           List<TicketExchangeProducts> productList = new List<TicketExchangeProducts>();

           // Call Talent Common
           // Retrieve the table form Talent Common
           // Turn this data into a list for the viewmodel
          DEProductDetails deProduct = new DEProductDetails();
          deProduct.ProductType = GlobalConstants.HOMEPRODUCTTYPE;
          deProduct.ProductSubtype = string.Empty;
          deProduct.ProductSupertype = string.Empty;
          deProduct.YearsOfPastProductsToShow = YearsOfPastProductsToShow;
          ErrorObj err = new ErrorObj();
          TalentProduct talentProduct = new TalentProduct();
          talentProduct.Settings = Environment.Settings.DESettings;
          talentProduct.Settings.Cacheing = true;
          talentProduct.De = deProduct; 
          err = talentProduct.RetrieveProductsFiltered();
          DataTable products = talentProduct.ResultDataSet.Tables["ProductEntries"];
          if (!err.HasError) {
              productList = Data.PopulateObjectListFromTable<TicketExchangeProducts>(products);
              productList = productList.OrderByDescending(x => x.ProductDate).ToList();
          }

          
          
          foreach (var product in productList)
          {
              StringBuilder sBProductCodeDateDescriptionMask = new StringBuilder(ProductCodeDateDescriptionMask);
              sBProductCodeDateDescriptionMask.Replace("<<ProductCode>>", Utilities.CheckForDBNull_String(product.ProductCode));
              sBProductCodeDateDescriptionMask.Replace("<<ProductDate>>", Utilities.CheckForDBNull_String(Utilities.ISeriesDate(product.ProductDate.ToString())));
              sBProductCodeDateDescriptionMask.Replace("<<ProductDescription>>", Utilities.CheckForDBNull_String(product.ProductDescription));
              product.ProductMask = sBProductCodeDateDescriptionMask.ToString();
          }


          return productList;
        }


        public TicketExchangeConfirmViewModel TicketingExchangeDefaultsConfirm(TicketExchangeDefaultsConfirmInputModel inputModel)
        {
            TicketExchangeConfirmViewModel viewModel = new TicketExchangeConfirmViewModel(true);
            TalentTicketExchange ticketExchange = new TalentTicketExchange();

            ticketExchange.Dep.ProductCode = inputModel.ProductCode;
            ticketExchange.Dep.AllowTicketExchangePurchase = inputModel.AllowTicketExchangePurchase;
            ticketExchange.Dep.AllowTicketExchangeReturn = inputModel.AllowTicketExchangeReturn;
            ticketExchange.Dep.MinimumResalePrice = inputModel.MinimumResalePrice;
            ticketExchange.Dep.MaximumResalePrice = inputModel.MaximumResalePrice;
            ticketExchange.Dep.ClubFee = inputModel.ClubFee;
            ticketExchange.Dep.ClubFeePercentageOrFixed = inputModel.ClubFeePercentageOrFixed;
            ticketExchange.Dep.MinMaxBoundaryPercentageOrFixed = inputModel.MinMaxBoundaryPercentageOrFixed;
            ticketExchange.Dep.CustomerRetainsPrerequisite = inputModel.CustomerRetainsPrerequisite;
            ticketExchange.Dep.CustomerRetainsMaxLimit = inputModel.CustomerRetainsMaxLimit;


            // davetodo add rest of fields here 
            ticketExchange.Settings = Environment.Settings.DESettings;
            ticketExchange.Settings.Cacheing = false;

            foreach (TicketExchangeStandAreaDefaults sad in inputModel.StandAreaDefaults)
            {
                // Map stand and area defaults
                DETicketExchangeStandAreaDefaults desad = new DETicketExchangeStandAreaDefaults();
                desad.StandAreaStandCode = sad.StandCode;
                desad.StandAreaAreaCode = sad.AreaCode;
                desad.StandAreaAllowTicketExchangePurchaseFlag = Utilities.convertToBool(sad.AllowTicketExchangePurchase);
                desad.StandAreaAllowTicketExchangeReturnFlag = Utilities.convertToBool(sad.AllowTicketExchangeReturn);

                ticketExchange.Dep.NumberOfStandAreas = ticketExchange.Dep.NumberOfStandAreas + 1;
                ticketExchange.Dep.StandAreaDefaults.Add(desad);

            }

            ErrorObj err = ticketExchange.UpdateTicketExchangeDefaults();
            viewModel.Error = Data.PopulateErrorObject(err, ticketExchange.ResultDataSet, ticketExchange.Settings, null, true);

            return viewModel;
        }
    }
}


