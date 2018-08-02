using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TalentBusinessLogic.Models;
using Talent.Common;
using AutoMapper;

namespace TalentBusinessLogic.ModelBuilders.Products
{
    public class ProductAvailabilityBuilder : BaseModelBuilder
    {
        private ProductAvailabilityViewModel _viewModel;
        private TalentProduct _talProduct;
        private DEProductDetails _deProdDetails;
        private string _rovingAreaText;
        private string _rovingAreaPrefix;
        private string _rovingAreaPrefixAndText;

        #region Public Functions
        
        public ProductAvailabilityViewModel GetProductAvailability(ProductAvailabilityInputModel inputModel)
        {
            _viewModel = new ProductAvailabilityViewModel(true, "StandAndAreaSelection.ascx");
            Mapper.CreateMap<ProductAvailabilityInputModel, DEProductDetails>();
            ErrorObj err = new ErrorObj();
            DataTable dtStadiumAvailability = new DataTable();
            DataTable dtAvailablePriceBreaks = new DataTable();
            DataTable dtPriceBreakPrices = new DataTable();
            DataTable dtProductPriceBands = new DataTable();
            _rovingAreaPrefix = "*";
            _rovingAreaText = _viewModel.GetControlText("RovingAreaText");
            _rovingAreaPrefixAndText = _rovingAreaPrefix + _rovingAreaText;
            if (inputModel.StandCode != null)
            {
                if (inputModel.StandCode.Contains(_rovingAreaPrefixAndText))
                {
                    inputModel.StandCode = inputModel.StandCode.Substring(0, inputModel.StandCode.IndexOf(_rovingAreaPrefix));
                }
            }
            _deProdDetails = Mapper.Map<DEProductDetails>(inputModel);
            
            dtStadiumAvailability = getStadiumAvailability(err);
            if (!_viewModel.Error.HasError)
            {
                dtAvailablePriceBreaks = getAvailablePriceBreaks(err);
                if (!_viewModel.Error.HasError)
                {
                    dtPriceBreakPrices = getPriceBreakPrices(err, inputModel.PriceBreakId);
                    if (!_viewModel.Error.HasError)
                    {
                        dtProductPriceBands = getProductPriceBands(err);
                        if (!_viewModel.Error.HasError)
                        {
                            buildViewModel(dtStadiumAvailability, dtAvailablePriceBreaks, dtPriceBreakPrices, dtProductPriceBands, inputModel.PriceBreakId, inputModel.DefaultPriceBand, inputModel.StandCode, inputModel.AreaCode);
                        }
                    }
                }
            }
            return _viewModel;
        }
        
        #endregion

        #region Private Functions

        /// <summary>
        /// Retrieve the available stadium Stand/Area codes from WS011R for the given product
        /// </summary>
        /// <param name="talProduct">The TalentProduct object</param>
        /// <param name="err">error object</param>
        /// <returns>The datatable with only the stand and area data</returns>
        private DataTable getStadiumAvailability(ErrorObj err)
        {
            DataTable resultDataTable = new DataTable();
            _talProduct = new TalentProduct();
            _talProduct.Settings = Environment.Settings.DESettings;
            _talProduct.Settings.Cacheing = true;
            _talProduct.De = _deProdDetails;
            err = _talProduct.AvailableStands();
            _viewModel.Error = Data.PopulateErrorObject(err, _talProduct.ResultDataSet, _talProduct.Settings, 3);
            if (!_viewModel.Error.HasError)
            {
                resultDataTable = _talProduct.ResultDataSet.Tables["StadiumAvailability"];
            }
            else
            {
                _talProduct.Settings.Logging.GeneralLog("SeatSelectionError", _viewModel.Error.ReturnCode, "Error getting availablity from WS011R for Product Code: " + _talProduct.De.ProductCode + " | " + _viewModel.Error.ErrorMessage, "SeatSelectionLog");
            }
            return resultDataTable;
        }

        /// <summary>
        /// Retrieve the available price breaks from WS011R for the given product
        /// </summary>
        /// <param name="talProduct">The TalentProduct object</param>
        /// <param name="err">error object</param>
        /// <returns>The datatable with only the price break data</returns>
        private DataTable getAvailablePriceBreaks(ErrorObj err)
        {
            DataTable resultDataTable = new DataTable();
            _talProduct = new TalentProduct();
            _talProduct.Settings = Environment.Settings.DESettings;
            _talProduct.Settings.Cacheing = true;
            _talProduct.De = _deProdDetails;
            err = _talProduct.ProductStadiumAvailability();
            _viewModel.Error = Data.PopulateErrorObject(err, _talProduct.ResultDataSet, _talProduct.Settings, 3);
            if (!_viewModel.Error.HasError)
            {
                resultDataTable = _talProduct.ResultDataSet.Tables["AvailablePriceBreaks"];
            }
            else
            {
                _talProduct.Settings.Logging.GeneralLog("SeatSelectionError", _viewModel.Error.ReturnCode, "Error getting price breaks from WS011R for Product Code: " + _talProduct.De.ProductCode + " | " + _viewModel.Error.ErrorMessage, "SeatSelectionLog");
            }
            return resultDataTable;
        }

        /// <summary>
        /// Retrieve the price break prices for the given product by calling MD141S
        /// </summary>
        /// <param name="talProduct">The TalentProduct object</param>
        /// <param name="err">error object</param>
        /// <returns>The datatable of price break prices</returns>
        private DataTable getPriceBreakPrices(ErrorObj err, long priceBreakId)
        {
            DataTable resultDataTable = new DataTable();
            _talProduct = new TalentProduct();
            _talProduct.Settings = Environment.Settings.DESettings;
            _talProduct.Settings.Cacheing = true;
            _talProduct.De = _deProdDetails;
            err = _talProduct.ProductPriceBreaks();
            _viewModel.Error = Data.PopulateErrorObject(err, _talProduct.ResultDataSet, _talProduct.Settings, 2);
            if (!_viewModel.Error.HasError)
            {
                if (priceBreakId == 0)
                {
                    resultDataTable = _talProduct.ResultDataSet.Tables["ProductPriceBreaks"];
                }
                else
                {
                    DataView priceBreakDefinitions = new DataView(_talProduct.ResultDataSet.Tables["ProductPriceBreaks"]);
                    priceBreakDefinitions.RowFilter = "PriceBreakId = " + priceBreakId;
                    resultDataTable = priceBreakDefinitions.ToTable();
                }
            }
            else
            {
                _talProduct.Settings.Logging.GeneralLog("SeatSelectionError", _viewModel.Error.ReturnCode, "Error getting price break prices from MD141S for Product Code: " + _talProduct.De.ProductCode + " | " + _viewModel.Error.ErrorMessage, "SeatSelectionLog");
            }
            return resultDataTable;
        }

        /// <summary>
        /// Retrieve the product details from WS007R for only the price band information
        /// </summary>
        /// <param name="talProduct">The TalentProduct object</param>
        /// <param name="err">error object</param>
        /// <returns>The datatable with only the descriptive price band data</returns>
        private DataTable getProductPriceBands(ErrorObj err)
        {
            DataTable resultDataTable = new DataTable();
            _talProduct = new TalentProduct();
            _talProduct.Settings = Environment.Settings.DESettings;
            _talProduct.Settings.Cacheing = true;
            _talProduct.De = _deProdDetails;
            err = _talProduct.ProductDetails();
            _viewModel.Error = Data.PopulateErrorObject(err, _talProduct.ResultDataSet, _talProduct.Settings, 5);
            if (!_viewModel.Error.HasError)
            {
                resultDataTable = _talProduct.ResultDataSet.Tables["ProductPriceBands"];
            }
            else
            {
                _talProduct.Settings.Logging.GeneralLog("SeatSelectionError", _viewModel.Error.ReturnCode, "Error getting price bands from WS007R for Product Code: " + _talProduct.De.ProductCode + " | " + _viewModel.Error.ErrorMessage, "SeatSelectionLog");
            }
            return resultDataTable;
        }

        /// <summary>
        /// Build the ProductAvailability ViewModel based on the given datatables
        /// This is where all the data is brought together, it is quite complex so be careful when debugging and editing this code!
        /// </summary>
        /// <param name="dtStadiumAvailability">Stand/Area data</param>
        /// <param name="dtAvailablePriceBreaks">Available price break IDs</param>
        /// <param name="dtPriceBreakPrices">Price Break Prices</param>
        /// <param name="dtProductPriceBands">Price bands</param>
        /// <param name="priceBreakId">The selected price break ID, if there is one</param>
        private void buildViewModel(DataTable dtStadiumAvailability, DataTable dtAvailablePriceBreaks, DataTable dtPriceBreakPrices, DataTable dtProductPriceBands, long priceBreakId, string defaultPriceBand, string standCode, string areaCode)
        {
            List<DataTransferObjects.StadiumAvailability> stadiumAvailabilityList = new List<DataTransferObjects.StadiumAvailability>();
            List<DataTransferObjects.PriceBreakAvailability> priceBreakAvailabilityList = new List<DataTransferObjects.PriceBreakAvailability>();
            List<DataTransferObjects.PriceBandPrices> priceBandPricesList = new List<DataTransferObjects.PriceBandPrices>();
            List<decimal> tempPriceBreakPricesList = new List<decimal>();
            List<DataTransferObjects.PriceBreakPrices> priceBreakPricesList = new List<DataTransferObjects.PriceBreakPrices>();
            DataTransferObjects.PriceBreakPrices priceItem = new DataTransferObjects.PriceBreakPrices();
            DataView dvPriceBreakPrices = new DataView(dtPriceBreakPrices);

            //1. Stadium Availablility
            //Get the stadium availability list based on the given datatable
            stadiumAvailabilityList = Data.PopulateObjectListFromTable<DataTransferObjects.StadiumAvailability>(dtStadiumAvailability);
            foreach (DataTransferObjects.StadiumAvailability availability in stadiumAvailabilityList)
            {
                if (availability.Roving == "Y") 
                {
                    availability.StandDescription = _rovingAreaText;
                    availability.StandCode = availability.StandCode + _rovingAreaPrefixAndText;
                    availability.AreaDescription = _rovingAreaText;
                }
            }

            //2. Available Price Breaks
            //Get the available price break IDs with descriptions from the availability table cross references with the definition table
            foreach (DataRow availablePriceBreaksRow in dtAvailablePriceBreaks.Rows)
            {
                foreach (DataRow priceBreakDefinitionRow in dtPriceBreakPrices.Rows)
                {
                    if (availablePriceBreaksRow["PriceBreakId"].ToString() == priceBreakDefinitionRow["PriceBreakId"].ToString())
                    {
                        
                        bool itemAdded = false;
                        if (availablePriceBreaksRow["Stand"].ToString() == standCode && string.IsNullOrEmpty(areaCode))
                        {                   
                            itemAdded = true;
                        }
                        else if (availablePriceBreaksRow["Area"].ToString() == areaCode)
                        {
                            itemAdded = true;
                        }
                        else if (string.IsNullOrEmpty(areaCode) && string.IsNullOrEmpty(standCode)) {       
                            itemAdded = true;
                        }
                        if (itemAdded) {
                            DataTransferObjects.PriceBreakAvailability listItem = new DataTransferObjects.PriceBreakAvailability();
                            listItem.PriceBreakId = Convert.ToInt64(availablePriceBreaksRow["PriceBreakId"].ToString());
                            listItem.PriceBreakDescription = priceBreakDefinitionRow["PriceBreakDescription"].ToString();
                            bool itemAlreadyAdded = false;
                            for (int i = 0; i < priceBreakAvailabilityList.Count; i++)
                            {
                                if (priceBreakAvailabilityList[i].PriceBreakId == listItem.PriceBreakId)
                                {
                                    itemAlreadyAdded = true;
                                    break;
                                }
                            }
                            if (!itemAlreadyAdded) priceBreakAvailabilityList.Add(listItem);
                            break;
                        }
                    }
                }
            }

            priceBreakAvailabilityList.Sort((x, y) => string.Compare(x.PriceBreakDescription, y.PriceBreakDescription));

            //3. Min/Max Prices
            //Get the full list of available prices - for only the default price band. Default Price band is 0-9 or Z then add "0" price.
            int n;
            if (int.TryParse(defaultPriceBand, out n) || defaultPriceBand == "Z")
            {
                priceItem = new DataTransferObjects.PriceBreakPrices();
                priceItem.Price = "0";
                priceItem.DisplayPrice = TDataObjects.PaymentSettings.FormatCurrency(0, Environment.Settings.BusinessUnit, Environment.Settings.Partner);
                priceBreakPricesList.Add(priceItem);
            }
            else
            {
                DataTable dtFilteredPriceBreakPrices = new DataTable();
                StringBuilder rowFilter = new StringBuilder();
                bool addedFirstItem = false;
                if (priceBreakAvailabilityList.Count > 0)
                {
                    rowFilter.Append("PriceBreakId IN (");
                    foreach (DataTransferObjects.PriceBreakAvailability listItem in priceBreakAvailabilityList)
                    {
                        if (addedFirstItem) rowFilter.Append(",");
                        rowFilter.Append(listItem.PriceBreakId.ToString());
                        addedFirstItem = true;
                    }
                    rowFilter.Append(")");
                    dvPriceBreakPrices.RowFilter = rowFilter.ToString();
                }
                dtFilteredPriceBreakPrices = dvPriceBreakPrices.ToTable(true, "PriceBand" + defaultPriceBand);
                foreach (DataRow priceBreakPriceRow in dtFilteredPriceBreakPrices.Rows)
                {
                    if (Convert.ToDecimal(priceBreakPriceRow[0].ToString()) > 0)
                    {
                        decimal price;
                        price = Convert.ToDecimal(priceBreakPriceRow[0].ToString());
                        if (!tempPriceBreakPricesList.Contains(price))
                        {
                            tempPriceBreakPricesList.Add(price);
                        }
                    }
                }
                tempPriceBreakPricesList.Sort();
                foreach (decimal price in tempPriceBreakPricesList)
                {
                    priceItem = new DataTransferObjects.PriceBreakPrices();
                    priceItem.Price = price.ToString();
                    priceItem.DisplayPrice = TDataObjects.PaymentSettings.FormatCurrency(price, Environment.Settings.BusinessUnit, Environment.Settings.Partner);
                    priceBreakPricesList.Add(priceItem);
                }
            }
            
            //4. Available Price Bands with prices
            //Get the price band prices by looking at the available product price bands and check they have a price when only 1 price break has been selected
            if (dtPriceBreakPrices.Rows.Count == 1)
            {
                foreach (DataRow priceBandRow in dtProductPriceBands.Rows)
                {
                    string priceBand;
                    string priceBandDescription;
                    priceBand = priceBandRow["PriceBand"].ToString().Trim();
                    priceBandDescription = priceBandRow["PriceBandDescription"].ToString().Trim();
                    addToPriceBandPricesList(priceBand, priceBandDescription, false, ref dtPriceBreakPrices, ref priceBandPricesList);
                }
            }
            //Get the price bands by looking at available price breaks (when more a selected) but without a price (as there may be multiple)
            else
            {
                foreach (DataRow priceBandRow in dtProductPriceBands.Rows)
                {
                    bool itemAlreadyAdded = false;
                    foreach (DataTransferObjects.PriceBandPrices priceBandPriceItem in priceBandPricesList)
                    {
                        if (priceBandPriceItem.PriceBand == priceBandRow["PriceBand"].ToString())
                        {
                            itemAlreadyAdded = true;
                            break;
                        }
                    }
                    if (!itemAlreadyAdded)
                    {
                        DataTable dtCurrentPriceBreakPrices = dvPriceBreakPrices.ToTable();
                        string priceBand;
                        string priceBandDescription;
                        priceBand = priceBandRow["PriceBand"].ToString().Trim();
                        priceBandDescription = priceBandRow["PriceBandDescription"].ToString().Trim();
                        addToPriceBandPricesList(priceBand, priceBandDescription, true, ref dtCurrentPriceBreakPrices, ref priceBandPricesList);
                    }
                }
            }

            //Get list of stand and area codes along with any roving stand/areas
            foreach (DataRow availabilityRow in dtStadiumAvailability.Rows)
            {
                if (availabilityRow["Roving"].ToString() == "Y")
                {
                    availabilityRow["StandDescription"] = _rovingAreaText;
                    availabilityRow["StandCode"] = availabilityRow["StandCode"] + _rovingAreaPrefixAndText;
                    availabilityRow["AreaDescription"] = _rovingAreaText;
                }
            }
            _viewModel.AvailableAreas = Data.PopulateObjectListFromTable<DataTransferObjects.AvailableAreas>(dtStadiumAvailability);
            _viewModel.AvailableStands = Data.PopulateObjectListFromTable<DataTransferObjects.AvailableStands>(dtStadiumAvailability);

            //Add to the view model
            _viewModel.StadiumAvailabilityList = stadiumAvailabilityList; //General Availability of stand/areas
            _viewModel.PriceBreakAvailabilityList = priceBreakAvailabilityList; //Available price break IDs with descriptions
            _viewModel.PriceBreakPrices = priceBreakPricesList; //List of available prices for the default price band
            _viewModel.PriceBandPricesList = priceBandPricesList; //List of available prices with price band and description
        }

        /// <summary>
        /// Add the price band, price, description list item to the price band prices list.
        /// Handle the Z and 0-9 free of charge price bands.
        /// </summary>
        /// <param name="priceBand">The current price band</param>
        /// <param name="priceBandDescription">The current price band description</param>
        /// <param name="multiplePriceBreaks">Are multiple price breaks being selected, if so do not bind price information (unless FOC)</param>
        /// <param name="dtPriceBreaks">The current price break table with the current price break row selected</param>
        /// <param name="priceBandPricesList">The price band list to add the items to</param>
        private void addToPriceBandPricesList(string priceBand, string priceBandDescription, bool multiplePriceBreaks, ref DataTable dtPriceBreaks, ref List<DataTransferObjects.PriceBandPrices>  priceBandPricesList)
        {
            DataTransferObjects.PriceBandPrices listItem = new DataTransferObjects.PriceBandPrices();
            int n;
            decimal price;
            listItem.PriceBand = priceBand;
            listItem.PriceBandDescription = priceBandDescription;

            //If there are multiple prices retrieve all unique prices and sort to retrieve the min and max prices to use for display purposes 
            if (multiplePriceBreaks)
            {
                decimal minPriceDecimal;
                decimal maxPriceDecimal;
                List<decimal> priceBreakPricesList = new List<decimal>();
                foreach (DataRow priceBreakRow in dtPriceBreaks.Rows)
                {
                    if (int.TryParse(priceBand, out n) || priceBand == "Z")
                    {
                        //Price band is free (0-9 or Z)
                        if (Convert.ToBoolean(priceBreakRow["PriceBandAvailable" + priceBand]))
                        {
                            if (!priceBreakPricesList.Contains(0)) priceBreakPricesList.Add(0);
                        }
                    }
                    else
                    {
                        price = Convert.ToDecimal(priceBreakRow["PriceBand" + priceBand]);
                        if (price > 0)
                        {
                            if (!priceBreakPricesList.Contains(price)) priceBreakPricesList.Add(price);
                        }
                    }
                }

                if (priceBreakPricesList.Count > 0)
                {
                    priceBreakPricesList.Sort();
                    minPriceDecimal = priceBreakPricesList[0];
                    maxPriceDecimal = priceBreakPricesList[priceBreakPricesList.Count - 1];
                    if (minPriceDecimal == maxPriceDecimal)
                    {

                        listItem.PriceBandPrice = TDataObjects.PaymentSettings.FormatCurrency(minPriceDecimal, Environment.Settings.BusinessUnit, Environment.Settings.Partner);
                        priceBandPricesList.Add(listItem);
                    }
                    else
                    {
                        string priceDisplayString = _viewModel.GetControlText("MinMaxPriceMask");
                        string minPriceString;
                        string maxPriceString;
                        minPriceString = TDataObjects.PaymentSettings.FormatCurrency(minPriceDecimal, Environment.Settings.BusinessUnit, Environment.Settings.Partner);
                        maxPriceString = TDataObjects.PaymentSettings.FormatCurrency(maxPriceDecimal, Environment.Settings.BusinessUnit, Environment.Settings.Partner);
                        priceDisplayString = priceDisplayString.Replace("<<MIN_PRICE>>", minPriceString);
                        priceDisplayString = priceDisplayString.Replace("<<MAX_PRICE>>", maxPriceString);
                        listItem.PriceBandPrice = priceDisplayString;
                        priceBandPricesList.Add(listItem);
                    }
                }
            }
            else
            {
                if (int.TryParse(priceBand, out n) || priceBand == "Z")
                {
                    //Price band is free (0-9 or Z)
                    if (Convert.ToBoolean(dtPriceBreaks.Rows[0]["PriceBandAvailable" + priceBand]))
                    {
                        listItem.PriceBandPrice = TDataObjects.PaymentSettings.FormatCurrency(0, Environment.Settings.BusinessUnit, Environment.Settings.Partner);
                        priceBandPricesList.Add(listItem);
                    }
                }
                else
                {
                    price = Convert.ToDecimal(dtPriceBreaks.Rows[0]["PriceBand" + priceBand].ToString());
                    if (price > 0)
                    {
                        listItem.PriceBandPrice = TDataObjects.PaymentSettings.FormatCurrency(price, Environment.Settings.BusinessUnit, Environment.Settings.Partner);
                        priceBandPricesList.Add(listItem);
                    }
                }
            }
        }

        #endregion
    }
}