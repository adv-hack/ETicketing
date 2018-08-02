using System;
using System.Data;
using System.Collections.Generic;
using TalentBusinessLogic.Models;
using Talent.Common;
using TalentBusinessLogic.DataTransferObjects.Hospitality;
using System.Linq;
using TalentBusinessLogic.BusinessObjects.Definitions;

namespace TalentBusinessLogic.ModelBuilders.Hospitality
{
    public class HospitalityListBuilder : BaseModelBuilder
    {
        #region Class Level Fields

        private Dictionary<long, List<string>> _packageCompetitionCodes = new Dictionary<long, List<string>>();
        private Dictionary<long, List<string>> _packageOppositionCodes = new Dictionary<long, List<string>>();
        private Dictionary<long, List<string>> _packageSubTypeCodes = new Dictionary<long, List<string>>();
        private HospitalityProductListViewModel _productListViewModel = null;
        private HospitalityPackageListViewModel _packageListViewModel = null;

        #endregion

        #region Public Functions

        /// <summary>
        /// Retrieve the list of corporate hospitality products
        /// Home and Season products are sorted and returned as two lists
        /// The date is formatted in a very specific way to allow javascript date formatting to work correctly with time zones.
        /// </summary>
        /// <param name="inputModel">The given hospitality product list input model</param>
        /// <returns>The formed hospitality product list view model</returns>
        public HospitalityProductListViewModel GetHospitalityProductList(HospitalityProductListInputModel inputModel)
        {
            _productListViewModel = new HospitalityProductListViewModel();
            if (String.IsNullOrEmpty(inputModel.PackageID))
            {
                DataTable dtProductListHome = buildProductListDataTable(GlobalConstants.HOMEPRODUCTTYPE);
                DataTable dtProductListSeason = buildProductListDataTable(GlobalConstants.SEASONTICKETPRODUCTTYPE);
                List<ProductDetails> productListHome = new List<ProductDetails>();
                List<ProductDetails> productListSeason = new List<ProductDetails>();
                productListHome = Data.PopulateObjectListFromTable<ProductDetails>(dtProductListHome);
                productListSeason = Data.PopulateObjectListFromTable<ProductDetails>(dtProductListSeason);

                foreach (ProductDetails product in productListHome)
                {
                    DateTime dateUpdated = new DateTime();
                    dateUpdated = Utilities.ISeriesDate(product.ProductMDTE08);
                    product.ProductDate = dateUpdated.ToString(Environment.Settings.DefaultValues.GlobalDateFormat);
                    product.ProductDateSearch = dateUpdated.ToString("yyyy-MM-ddT00:00:00Z");//specific time format for filtering
                }
                foreach (ProductDetails product in productListSeason)
                {
                    DateTime dateUpdated = new DateTime();
                    dateUpdated = Utilities.ISeriesDate(product.ProductMDTE08);
                    product.ProductDate = dateUpdated.ToString(Environment.Settings.DefaultValues.GlobalDateFormat);
                    product.ProductDateSearch = dateUpdated.ToString("yyyy-MM-ddT00:00:00Z");//specific time format for filtering
                }

                _productListViewModel.ProductListDetailsHome = productListHome;
                _productListViewModel.ProductListDetailsSeason = productListSeason;
                if (productListHome.Count > 0)
                {
                    _productListViewModel.MinDateHome = productListHome.OrderBy((x) => x.ProductMDTE08).Select((x) => x.ProductDateSearch).First();
                    _productListViewModel.MaxDateHome = productListHome.OrderBy((x) => x.ProductMDTE08).Select((x) => x.ProductDateSearch).Last();
                }
                if (productListSeason.Count > 0)
                {
                    _productListViewModel.MinDateSeason = productListSeason.OrderBy((x) => x.ProductMDTE08).Select((x) => x.ProductDateSearch).First();
                    _productListViewModel.MaxDateSeason = productListSeason.OrderBy((x) => x.ProductMDTE08).Select((x) => x.ProductDateSearch).Last();
                }
            }
            else
            {
                // PackageId must have been pass into viewModel
                // Retrieve the product list based on the selected package ID
                List<ProductDetails> productList = new List<ProductDetails>();
                DataTable dtProductList = buildProductListDataTable(inputModel.ProductType);
                foreach (DataRow productRow in dtProductList.Rows)
                {
                    DataSet dsPackagesAndComponents = retrieveHospitalityPackages(productRow["ProductCode"].ToString());
                    DataTable dtPackages = dsPackagesAndComponents.Tables["PackageList"];
                    DataTable dtComponents = dsPackagesAndComponents.Tables["ComponentList"];
                    DataView dvFilteredPackages;
                    DataTable dtFilteredPackages;
                    if (dtPackages != null && dtComponents != null)
                    {
                        //check if packageList contains selected package
                        dvFilteredPackages = new DataView(dtPackages);
                        dvFilteredPackages.RowFilter = String.Format("PackageId = '{0}'", inputModel.PackageID.ToString());
                        dtFilteredPackages = dvFilteredPackages.ToTable();
                        if (dtFilteredPackages.Rows.Count > 0)
                        {
                            ProductDetails product = new ProductDetails();
                            List<PackageDetails> packageList = new List<PackageDetails>();
                            List<ComponentDetails> componentsList = new List<ComponentDetails>();
                            List<ComponentDetails> filteredComponentsList = new List<ComponentDetails>();
                            DateTime dateUpdated = new DateTime();

                            dateUpdated = Utilities.ISeriesDate(productRow["ProductMDTE08"].ToString());
                            productRow["ProductDate"] = dateUpdated.ToString(Environment.Settings.DefaultValues.GlobalDateFormat);
                            product = Data.PopulateObjectFromRow<ProductDetails>(productRow);
                            packageList = Data.PopulateObjectListFromTable<PackageDetails>(dtFilteredPackages);
                            componentsList = Data.PopulateObjectListFromTable<ComponentDetails>(dtComponents);
                            filteredComponentsList = componentsList.Where(x => x.PackageID.ToString() == inputModel.PackageID).ToList();
                            product.AvailabilityDetail = getAvailabilityDetails(packageList[0], filteredComponentsList);
                            productList.Add(product);
                        }
                    }
                }
                _productListViewModel.ProductListDetailByPackageId = productList;
            }
            return _productListViewModel;
        }

        /// <summary>
        /// Retrieve the list of corporate hospitality packages
        /// </summary>
        /// <param name="inputModel">The given hospitality package list input model</param>
        /// <returns>The formed hospitality package list view model</returns>
        public HospitalityPackageListViewModel GetHospitalityPackageList(HospitalityPackageListInputModel inputModel)
        {
            _packageListViewModel = new HospitalityPackageListViewModel(true);
            _packageListViewModel.FormattedPackagePrice = _packageListViewModel.GetPageText("packagePriceFormat");
            _packageListViewModel.FormattedPackageGroupSizeDescription = _packageListViewModel.GetPageText("packageGroupSizeFormat");

            if (string.IsNullOrEmpty(inputModel.ProductCode) && string.IsNullOrEmpty(inputModel.ProductGroupCode))
            {
                //No product code or product group code is passed in
                DataTable dtProductListHome = buildProductListDataTable(GlobalConstants.HOMEPRODUCTTYPE);
                DataTable dtProductListSeason = buildProductListDataTable(GlobalConstants.SEASONTICKETPRODUCTTYPE);
                List<PackageDetails> packageListHome = buildPackageList(dtProductListHome);
                List<PackageDetails> packageListSeason = buildPackageList(dtProductListSeason);
                _packageListViewModel.PackageListDetailsHome = packageListHome;
                _packageListViewModel.PackageListDetailsSeason = packageListSeason;

                //Calculate Min and Max values for filters on other pages
                if (packageListHome.Count > 0)
                {
                    _packageListViewModel.MinGroupSizeHome = packageListHome.OrderBy((x) => x.GroupSize).Select((x) => x.GroupSize).First();
                    _packageListViewModel.MaxGroupSizeHome = packageListHome.OrderBy((x) => x.GroupSize).Select((x) => x.GroupSize).Last();
                    _packageListViewModel.MinBudgetHome = packageListHome.OrderBy((x) => x.Price).Select((x) => x.Price).First();
                    _packageListViewModel.MaxBudgetHome = packageListHome.OrderBy((x) => x.Price).Select((x) => x.Price).Last();
                    foreach (PackageDetails package in _packageListViewModel.PackageListDetailsHome)
                    {
                        string price = TDataObjects.PaymentSettings.FormatCurrency(package.Price, Environment.Settings.BusinessUnit, Environment.Settings.Partner);
                        package.DisplayPrice = _packageListViewModel.FormattedPackagePrice.Replace("<<PackagePrice>>", price);
                        package.HasSoldOutProducts = false;
                    }
                }
                if (packageListSeason.Count > 0)
                {
                    _packageListViewModel.MinGroupSizeSeason = packageListSeason.OrderBy((x) => x.GroupSize).Select((x) => x.GroupSize).First();
                    _packageListViewModel.MaxGroupSizeSeason = packageListSeason.OrderBy((x) => x.GroupSize).Select((x) => x.GroupSize).Last();
                    _packageListViewModel.MinBudgetSeason = packageListSeason.OrderBy((x) => x.Price).Select((x) => x.Price).First();
                    _packageListViewModel.MaxBudgetSeason = packageListSeason.OrderBy((x) => x.Price).Select((x) => x.Price).Last();
                    foreach (PackageDetails package in _packageListViewModel.PackageListDetailsSeason)
                    {
                        string price = TDataObjects.PaymentSettings.FormatCurrency(package.Price, Environment.Settings.BusinessUnit, Environment.Settings.Partner);
                        package.DisplayPrice = _packageListViewModel.FormattedPackagePrice.Replace("<<PackagePrice>>", price);
                        package.HasSoldOutProducts = false;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(inputModel.ProductCode))
            {
                //A product code has been passed into the viewmodel
                List<PackageDetails> packageListProduct = new List<PackageDetails>();
                DataTable dtProduct = new DataTable();
                dtProduct.Columns.Add("ProductCode", typeof(string));
                dtProduct.Rows.Add(inputModel.ProductCode);
                packageListProduct = buildPackageList(dtProduct);
                //If the user has selected a package then it should be removed from the list before bound to the view model
                //This is used to show the related packages for the current fixture that relate to the current package being selected
                packageListProduct = packageListProduct.Where(x => x.PackageID != inputModel.PackageId).ToList();
                _packageListViewModel.PackageListDetailsByProductCode = packageListProduct;
                if (packageListProduct.Count > 0)
                {
                    foreach (PackageDetails package in _packageListViewModel.PackageListDetailsByProductCode)
                    {
                        string price = TDataObjects.PaymentSettings.FormatCurrency(package.Price, Environment.Settings.BusinessUnit, Environment.Settings.Partner);
                        package.DisplayPrice = _packageListViewModel.FormattedPackagePrice.Replace("<<PackagePrice>>", price);
                        package.HasSoldOutProducts = false;
                    }
                }           
            }
            else if (!string.IsNullOrEmpty(inputModel.ProductGroupCode))
            {
                //A product group code has been passed into the viewmodel
                HospitalityProductGroupViewModel viewModel = new HospitalityProductGroupViewModel(getContentAndAttributes: true);
                DataSet dsProductGroupPackageList = new DataSet();
                DataSet dsProductGroupFixtures;
                DataSet dsSoldOutFixturesList = new DataSet();
                ErrorObj err = new ErrorObj();
                DEProductDetails de = new DEProductDetails();
                DESettings settings = Environment.Settings.DESettings;
                List<PackageDetails> packageListProductGroup = new List<PackageDetails>();
                List<ProductDetails> productGroupFixturesList = new List<ProductDetails>();
                List<ComponentDetails> componentsList = new List<ComponentDetails>();
                List<ComponentDetails> filteredComponentsList;
                List<ComponentDetails> filteredComponentsListPackage;

                dsProductGroupPackageList = retrivePackagesByProductGroup(inputModel.ProductGroupCode);
                viewModel.Error = Data.PopulateErrorObject(err, dsProductGroupPackageList, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 4);
                if (!viewModel.Error.HasError)
                {                   
                    packageListProductGroup = Data.PopulateObjectListFromTable<PackageDetails>(dsProductGroupPackageList.Tables["PackageList"]);                  
                    componentsList = Data.PopulateObjectListFromTable<ComponentDetails>(dsProductGroupPackageList.Tables["ComponentList"]);

                    //retrieve fixtures for product group
                    dsProductGroupFixtures = new DataSet();
                    dsProductGroupFixtures = retrieveProductGroupFixtures(inputModel.ProductGroupCode);
                    viewModel.Error = Data.PopulateErrorObject(err, dsProductGroupFixtures, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 2);

                    if (!viewModel.Error.HasError)
                    {
                        productGroupFixturesList = Data.PopulateObjectListFromTable<ProductDetails>(dsProductGroupFixtures.Tables["ProductGroupFixturesDeatils"]).Where(s=>(Convert.ToDateTime(s.ProductDate)) > DateTime.Now).ToList();
                        foreach (PackageDetails package in packageListProductGroup)
                        {
                            package.SoldOutProducts = string.Empty;
                            package.ProductGroupCode = inputModel.ProductGroupCode;
                            //get package group size
                            filteredComponentsListPackage = new List<ComponentDetails>();
                            filteredComponentsListPackage = componentsList.Where(x => (x.PackageID == package.PackageID && x.ProductCode == productGroupFixturesList.Select(s=>s.ProductCode).Distinct().FirstOrDefault().Trim() && x.ComponentType=="A")).ToList();
                          
                            if (filteredComponentsListPackage.Count > 0)
                            {
                                package.AvailabilityComponentID = filteredComponentsListPackage[0].ComponentID;
                                package.GroupSize = Int32.Parse(filteredComponentsListPackage.Select(x => x.NumberOfUnits).FirstOrDefault().ToString());
                            }
                            else
                            {
                                package.GroupSize = 1;
                            }
                            if (package.GroupSize == 1)
                            {
                                package.GroupSizeDescription = _packageListViewModel.GetPageText("packageGroupSizePerPersonText");
                            }
                            else
                            {
                                package.GroupSizeDescription = _packageListViewModel.FormattedPackageGroupSizeDescription.Replace("<<GroupSize>>", Convert.ToString(package.GroupSize));
                            }

                            //get sold out products                              
                            foreach (ProductDetails product in productGroupFixturesList)
                            {                            
                                filteredComponentsList = new List<ComponentDetails>();
                                filteredComponentsList = componentsList.Where(x => ( x.PackageID == package.PackageID && x.ProductCode==product.ProductCode)).ToList();
                                product.AvailabilityDetail = getAvailabilityDetails(package, filteredComponentsList);

                                if (product.AvailabilityDetail.AvailabilityText.Replace("-", "").Replace(" ", "").ToUpper() == GlobalConstants.HOSPITALITY_FIXTURE_NOT_AVAILABLE)
                                {
                                    if ( string.IsNullOrEmpty(package.SoldOutProducts))
                                    {
                                        package.SoldOutProducts = product.ProductCode.Trim();
                                    }
                                    else
                                    {
                                        package.SoldOutProducts = package.SoldOutProducts + ", " + product.ProductCode;
                                    }                                   
                                }
                            }

                            if (package.SoldOutProducts.Length>0)
                            {
                                package.HasSoldOutProducts = true;
                            }
                            else
                            {
                                package.HasSoldOutProducts = false;
                            }
                        }
                    }
                }

                _packageListViewModel.PackageListDetailsByProductGroup = packageListProductGroup;             
            }
            return _packageListViewModel;
        }

        /// <summary>
        /// Get details of Hospitality Product groups and its Fixtures
        /// </summary>
        /// <param name="inputModel">The given hospitality product group list input model</param>
        /// <returns>Hospitality Product groups and its fixtures list</returns>
        public HospitalityProductGroupViewModel GetHospitalityProductGroups(HospitalityProductGroupInputModel inputModel)
        {
            HospitalityProductGroupViewModel viewModel = new HospitalityProductGroupViewModel(getContentAndAttributes: true);
            DataSet dsProductGroupList = new DataSet();
            DataSet dsProductGroupFixturesList = new DataSet();
            ErrorObj err = new ErrorObj();
            DEProductDetails de = new DEProductDetails();
            DESettings settings = Environment.Settings.DESettings;
            List<ProductGroupDetails> productGroupDetailsList = new List<ProductGroupDetails>();
            List<ProductDetails> productGroupFixturesList = new List<ProductDetails>();

            dsProductGroupList = retrieveProductGroups();
            viewModel.Error = Data.PopulateErrorObject(err, dsProductGroupList, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 2);

            if (!viewModel.Error.HasError)
            {
                productGroupDetailsList = Data.PopulateObjectListFromTable<ProductGroupDetails>(dsProductGroupList.Tables["ProductGroupsDetails"]);
                foreach (ProductGroupDetails productGroup in productGroupDetailsList)
                {
                    HospitalityProductGroupInputModel inputModelProductGroup = new HospitalityProductGroupInputModel();
                    inputModelProductGroup.ProductGroupCode = productGroup.GroupId;
                    dsProductGroupFixturesList = retrieveProductGroupFixtures(inputModelProductGroup.ProductGroupCode);
                    viewModel.Error = Data.PopulateErrorObject(err, dsProductGroupFixturesList, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 2);

                    if (!viewModel.Error.HasError)
                    {
                        productGroup.ProductGroupFixturesList = new List<ProductDetails>();
                        productGroup.ProductGroupFixturesList = Data.PopulateObjectListFromTable<ProductDetails>(dsProductGroupFixturesList.Tables["ProductGroupFixturesDeatils"]);
                    }
                }
                viewModel.ProductGroupDetailsList = productGroupDetailsList;
            }
            return viewModel;
        }

        /// <summary>
        /// Get details of Hospitality Product group Fixtures
        /// </summary>
        /// <param name="inputModel">The given hospitality product group input model</param>
        /// <returns>Hospitality Product group fixtures list</returns>
        public HospitalityProductGroupViewModel GetHospitalityProductGroupFixtures(HospitalityProductGroupInputModel inputModel)
        {
            HospitalityProductGroupViewModel viewModel = new HospitalityProductGroupViewModel(getContentAndAttributes: true);
            DataSet dsProductGroupFixtures = new DataSet();
            ErrorObj err = new ErrorObj();
            DEProductDetails de = new DEProductDetails();
            DESettings settings = Environment.Settings.DESettings;
            List<ProductDetails> productGroupFixturesList = new List<ProductDetails>();

            dsProductGroupFixtures = retrieveProductGroupFixtures(inputModel.ProductGroupCode);
            viewModel.Error = Data.PopulateErrorObject(err, dsProductGroupFixtures, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 2);

            if (!viewModel.Error.HasError)
            {
                productGroupFixturesList = Data.PopulateObjectListFromTable<ProductDetails>(dsProductGroupFixtures.Tables["ProductGroupFixturesDeatils"]);
                viewModel.ProductGroupFixturesList = productGroupFixturesList;
            }
            return viewModel;
        }

        /// <summary>
        /// Call the functions of creating the HospitalityProductListViewModel and HospitalityProductGroupViewModel
        /// then use the HospitalityProductGroupViewModel to filter HospitalityProductListViewModel based on ProductCode        
        /// </summary>
        /// <param name="inputModelProductList">The given hospitality product list input model</param>
        /// <param name="inputModelProductGroup">The given hospitality product group input model</param>
        /// <returns>The formed hospitality product group view model</returns>
        public HospitalityProductGroupViewModel GetHospitalityProductListByProductGroup(HospitalityProductListInputModel inputModelProductList, HospitalityProductGroupInputModel inputModelProductGroup)
        {
            HospitalityProductListViewModel productListViewModel = new HospitalityProductListViewModel(getContentAndAttributes: true);
            HospitalityProductGroupViewModel productGroupViewModel = new HospitalityProductGroupViewModel(getContentAndAttributes: true);
            productListViewModel = GetHospitalityProductList(inputModelProductList);
            if (productListViewModel.Error!=null && productListViewModel.Error.HasError)
            {
                productGroupViewModel.Error.HasError = true;
            }
            else
            {
                productGroupViewModel = GetHospitalityProductGroups(inputModelProductGroup);
                if (productGroupViewModel.Error==null || !productGroupViewModel.Error.HasError)
                {
                    productGroupViewModel.ProductGroupDetailsList = productGroupViewModel.ProductGroupDetailsList.Where(a => a.GroupId.Equals(inputModelProductGroup.ProductGroupCode)).ToList();

                    if (productGroupViewModel.ProductGroupDetailsList.Count > 0)
                    {
                        productGroupViewModel.ProductGroupFixturesList = new List<ProductDetails>();
                        productGroupViewModel.ProductGroupFixturesList = productListViewModel.ProductListDetailByPackageId.Where(a => productGroupViewModel.ProductGroupDetailsList[0].ProductGroupFixturesList.Any(b => b.ProductCode.Trim().Equals(a.ProductCode.Trim()))).ToList();
                    }
                    else
                    {
                        productGroupViewModel.Error.HasError = true;
                    }
                }                
            }
                                        
            return productGroupViewModel;
        }

        #endregion

            #region Private Functions

            /// <summary>
            /// Build product list
            /// Create Dataview of formatted product list
            /// Filter by passed in product type or product code
            /// </summary>
            /// <param name="productType">The given product type</param>
            /// <returns>datatable of available corporate hospitality products acoording to producttype</returns>
        private DataTable buildProductListDataTable(string productType)
        {
            DataTable dtFormattedProductList = new DataTable();
            string corporateStadiumCode = Environment.Settings.DefaultValues.CorporateStadium;
            dtFormattedProductList = retrieveCorporateStadiumProducts(corporateStadiumCode);
            DataView dViewFormattedProductList = new DataView(dtFormattedProductList);
            dViewFormattedProductList.RowFilter = string.Format("ProductType = '{0}'", productType);
            dtFormattedProductList = dViewFormattedProductList.ToTable();
            return dtFormattedProductList;
        }

        /// <summary>
        /// Build package list. Set all the properties for the package including availability and all the package components
        /// </summary>
        /// <param name="dtProductList">The given product datatable</param>
        /// <returns>list of available corporate hospitality packages</returns>
        private List<PackageDetails> buildPackageList(DataTable dtProductList)
        {
            List<PackageDetails> packageList = new List<PackageDetails>();
            List<ComponentDetails> componentList = new List<ComponentDetails>();

            //build up the packages and components list for each available product
            foreach (DataRow productRow in dtProductList.Rows)
            {
                DataSet dsPackagesAndComponents = retrieveHospitalityPackages(productRow["ProductCode"].ToString());
                DataTable dtPackages = dsPackagesAndComponents.Tables["PackageList"];
                DataTable dtComponents = dsPackagesAndComponents.Tables["ComponentList"];
                if (dtPackages != null && dtComponents != null)
                {
                    List<PackageDetails> productPackages = new List<PackageDetails>();
                    List<ComponentDetails> components = new List<ComponentDetails>();
                    productPackages = Data.PopulateObjectListFromTable<PackageDetails>(dtPackages);
                    components = Data.PopulateObjectListFromTable<ComponentDetails>(dtComponents);
                    if (productRow.Table.Columns.Count > 1)
                    {
                        setFilters(productRow, productPackages);
                    }
                    packageList.AddRange(productPackages);
                    componentList.AddRange(components);
                }
            }

            //select distinct packages from all the packages that have been retrieved
            packageList = packageList.GroupBy((x) => x.PackageID).Select((x) => x.First()).ToList();
            //select distinct components from all the componennts that have been retrieved
            componentList = componentList.GroupBy(x => new { x.ComponentID, x.PackageID }).Select((x) => x.First()).ToList();

            foreach (PackageDetails package in packageList)
            {
                //Filter by package ID and availability component to get the package group size
                List<ComponentDetails> filteredComponentsList = componentList.Where(x => x.PackageID == package.PackageID && x.ComponentType == "A").ToList();
                List<ComponentDetails> componentsInThisPackage = componentList.Where(x => x.PackageID == package.PackageID).ToList();
                if (filteredComponentsList.Count > 0)
                {
                    package.AvailabilityComponentID = filteredComponentsList[0].ComponentID;
                    package.GroupSize = Int32.Parse(filteredComponentsList.Select(x => x.NumberOfUnits).FirstOrDefault().ToString());
                }
                else
                {
                    package.GroupSize = 1;
                }
                if (package.GroupSize == 1)
                {
                    package.GroupSizeDescription = _packageListViewModel.GetPageText("packageGroupSizePerPersonText");
                }
                else
                {
                    package.GroupSizeDescription = _packageListViewModel.FormattedPackageGroupSizeDescription.Replace("<<GroupSize>>", Convert.ToString(package.GroupSize));
                }
                package.AvailabilityDetail = getAvailabilityDetails(package, componentsInThisPackage);

                //if we have filters set, set their filter strings
                if (_packageCompetitionCodes.Count > 0) setFilterStrings(package);
            }

            return packageList;
        }

        /// <summary>
        /// Get the availability details for a fixture-package combination
        /// </summary>
        /// <param name="product">The product object</param>
        /// <param name="packageID">Provide the PackageID when product is given</param>
        /// <param name="package">The package details object</param>
        /// <param name="componentList">List of components for selected package</param>
        /// <returns>AvailabilityDetails object containing details of availability for fixture-package combination</returns>
        private AvailabilityDetails getAvailabilityDetails(PackageDetails package, List<ComponentDetails> componentList)
        {
            AvailabilityDetails availabilityDetail = new AvailabilityDetails();
            Availability av = new Availability();
            decimal minCapacity = 999;
            decimal minAvailable = 999;
            decimal percentageRemaining = 0;
            string availabilityColour = string.Empty;
            string availabilityText = string.Empty;
            string availabilityCSS = string.Empty;
            string stadiumCode = Environment.Settings.DefaultValues.CorporateStadium;
            string businessunit = Environment.Settings.BusinessUnit;
            long seatBasedComponentID = 0;


            // Loop through components and set the package capacity and available units to that of the last available component 
            availabilityDetail.SoldOutComponents = string.Empty;
            foreach (ComponentDetails component in componentList)
            {
                if (component.IsSeatComponent)
                {
                    seatBasedComponentID = component.ComponentID;
                    if (component.ComponentType == "A") availabilityDetail.AvailabilityComponentID = component.ComponentID;
                    var availableUnits = Environment.Settings.DESettings.IsAgent ? component.AvailableUnitsBUI : component.AvailableUnitsPWS;
                    if (availableUnits < minAvailable) minAvailable = availableUnits;
                    if (component.OriginalUnits < minCapacity) minCapacity = component.OriginalUnits;
                    if (availableUnits == 0)
                    {
                        //add a comma after the first component description
                        if (availabilityDetail.SoldOutComponents.Length > 0)
                        {
                            availabilityDetail.SoldOutComponents = string.Concat(availabilityDetail.SoldOutComponents, ", ");
                        }
                        availabilityDetail.SoldOutComponents = string.Concat(availabilityDetail.SoldOutComponents, component.ComponentDescription);
                    }
                }
            }

            // If PWS may only be able to sell up to a % of what was originally available (maintained on jwalk web product maint and is Capacity Limit %) 
            // If this is set reduce available qty and origianal qty as these were never available for PWS   
            if (package.MaxPercentageToSellPWS != 0 && package.MaxPercentageToSellPWS != 0 && !Environment.Settings.DESettings.IsAgent)
            {
                decimal numberPackageUnitsReservedForBoxOffice = Math.Ceiling(minCapacity * (100 - package.MaxPercentageToSellPWS) / 100);
                minAvailable -= numberPackageUnitsReservedForBoxOffice;
                if (minAvailable < 0) minAvailable = 0;
                minCapacity -= numberPackageUnitsReservedForBoxOffice;
                if (minCapacity < 0) minCapacity = 0;
            }

            // We need a component to pass to the backend - if availability defined use that otherwise use a seat based one we found
            if (availabilityDetail.AvailabilityComponentID == 0) availabilityDetail.AvailabilityComponentID = seatBasedComponentID;
            availabilityDetail.AvailableUnits = Convert.ToInt16(minAvailable);
            availabilityDetail.OriginalUnits = Convert.ToInt16(minCapacity);
            if (availabilityDetail.OriginalUnits > 0 && availabilityDetail.AvailableUnits > 0)
            {
                percentageRemaining = ((decimal)availabilityDetail.AvailableUnits / (decimal)availabilityDetail.OriginalUnits) * 100;
            }
            availabilityDetail.PercentageRemaining = Convert.ToInt16(Math.Round(percentageRemaining, 0));
            availabilityDetail.HaveSoldOutComponents = false;
            if (percentageRemaining == 0) availabilityDetail.HaveSoldOutComponents = true;

            //Get the stadium based availability properties
            av.GetAvailabilityProperties(businessunit, ref availabilityText, ref availabilityCSS, ref availabilityColour, availabilityDetail.PercentageRemaining, stadiumCode);
            availabilityDetail.AvailabilityText = availabilityText;
            availabilityDetail.AvailabilityCSS = availabilityCSS;
            availabilityDetail.AvailabilityColour = availabilityColour;
            availabilityDetail.ComponentListDetails = componentList;
            availabilityDetail.Sold = package.Sold;
            availabilityDetail.Cancelled = package.Cancelled;
            availabilityDetail.Reserved = package.Reserved;
            return availabilityDetail;
        }

        /// <summary>
        /// Get the available products for the corporate hospitality stadium - Call to WS016R
        /// </summary>
        /// <param name="corporateStadiumCode">The given corporate stadium code</param>
        /// <returns>DataTable of available corporate hospitality products</returns>
        private DataTable retrieveCorporateStadiumProducts(string corporateStadiumCode)
        {
            DataTable dtHospitalityProducts = new DataTable();
            DataSet dsResults = new DataSet();
            TalentProduct talProduct = new TalentProduct();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();
            talProduct.Settings = settings;
            talProduct.De.PriceAndAreaSelection = Environment.Settings.DefaultValues.PriceAndAreaSelection;
            talProduct.De.Src = GlobalConstants.SOURCE;
            talProduct.De.StadiumCode = Environment.Settings.DefaultValues.CorporateStadium;
            err = talProduct.ProductList();
            dsResults = talProduct.ResultDataSet;
            if (dsResults != null && dsResults.Tables.Count > 0)
            {
                dtHospitalityProducts = dsResults.Tables["ProductListResults"];
            }
            return dtHospitalityProducts;
        }

        /// <summary>
        /// Get the available packages for the given available product code - Call to WS002R
        /// </summary>
        /// <param name="productCode">The hospitality product code</param>
        /// <returns>DataSet of available corporate hospitality packages and components</returns>
        private DataSet retrieveHospitalityPackages(string productCode)
        {
            DataSet dsResults = new DataSet();
            TalentProduct talProduct = new TalentProduct();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();
            talProduct.Settings = settings;
            talProduct.De.ProductCode = productCode;
            talProduct.De.BusinessUnit = settings.BusinessUnit;
            err = talProduct.ProductHospitality();
            dsResults = talProduct.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        /// Get the available hospitality product groups - Call to PG001S
        /// </summary>
        /// <returns>DataSet of available corporate hospitality product groups</returns>
        private DataSet retrieveProductGroups()
        {
            DataSet dsResults = new DataSet();
            TalentProduct talProduct = new TalentProduct();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();

            talProduct.Settings = settings;
            talProduct.De.Src = GlobalConstants.SOURCE;
            talProduct.De.BusinessUnit = settings.BusinessUnit;
            err = talProduct.RetrieveProductGroups();
            dsResults = talProduct.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        /// Get the available products groups fixtures - Call to PG002S
        /// </summary>
        /// <param name="productGroupCode">The given product group code</param>
        /// <returns>DataSet of available corporate hospitality product groups fixtures</returns>
        private DataSet retrieveProductGroupFixtures(string productGroupCode)
        {
            DataSet dsResults = new DataSet();
            TalentProduct talProduct = new TalentProduct();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();

            talProduct.Settings = settings;
            talProduct.De.Src = GlobalConstants.SOURCE;
            talProduct.De.ProductGroupCode = productGroupCode;
            err = talProduct.RetrieveProductGroupFixtures();
            dsResults = talProduct.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        /// Get the available products groups packages
        /// </summary>
        /// <param name="productGroupCode">The given product group code</param>
        /// <returns>DataSet of available corporate hospitality product groups packages</returns>
        private DataSet retrivePackagesByProductGroup(string productGroupCode)
        {
            DataSet dsResults = new DataSet();
            TalentProduct talProduct = new TalentProduct();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();

            talProduct.Settings = settings;
            talProduct.De.Src = GlobalConstants.SOURCE;
            talProduct.De.ProductGroupCode = productGroupCode;
            talProduct.De.BusinessUnit = settings.BusinessUnit;
            err = talProduct.RetrieveProductGroupPackages();
            dsResults = talProduct.ResultDataSet;
            return dsResults;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Set the filter strings for the given package
        /// </summary>
        /// <param name="package">The package being worked with</param>
        private void setFilterStrings(PackageDetails package)
        {
            //Populate competition codes query string
            List<string> competitionCodes = _packageCompetitionCodes[package.PackageID];
            if (competitionCodes != null)
            {
                package.CompetitionCodesString = String.Join(" ", competitionCodes.Select((x) => "{0}" + x).ToArray());
            }
            //Populate opposition codes query string
            List<string> oppositionCodes = _packageOppositionCodes[package.PackageID];
            if (oppositionCodes != null)
            {
                package.OppositionCodesString = String.Join(" ", oppositionCodes.Select((x) => "{0}" + x).ToArray());
            }
            //Populate product sub-types query string
            List<string> subTypeCodes = _packageSubTypeCodes[package.PackageID];
            if (subTypeCodes != null)
            {
                package.SubTypeCodesString = String.Join(" ", subTypeCodes.Select((x) => "{0}" + x).ToArray());
            }
        }

        /// <summary>
        /// Set Filters - Add competition, sub types and product codes to a list
        /// </summary>
        /// <param name="productRow">The given packageId</param>
        /// <param name="productPackages">The given packagelist</param>
        private void setFilters(DataRow productRow, List<PackageDetails> productPackages)
        {
            foreach (PackageDetails package in productPackages)
            {
                string competitionCode = productRow["ProductCompetitionCode"].ToString();
                string oppositionCode = productRow["ProductOppositionCode"].ToString();
                string subTypeCode = productRow["ProductSubType"].ToString();
                setCompetitionCode(package.PackageID, competitionCode);
                setOppositionCode(package.PackageID, oppositionCode);
                setSubTypeCode(package.PackageID, subTypeCode);
            }
        }

        /// <summary>
        /// Creates opposition codes list 
        /// </summary>
        /// <param name="packageID">PackageId</param>
        /// <param name="oppositionCode">The given opposition code</param>
        private void setOppositionCode(long packageID, string oppositionCode)
        {
            if (!_packageOppositionCodes.ContainsKey(packageID))
            {
                _packageOppositionCodes.Add(packageID, new List<string> { oppositionCode });
            }
            else
            {
                List<string> oppositionCodes = _packageOppositionCodes[packageID];
                if (!oppositionCodes.Contains(oppositionCode))
                {
                    oppositionCodes.Add(oppositionCode);
                }
            }
        }

        /// <summary>
        /// Creates competition codes list 
        /// </summary>
        /// <param name="packageID">PackageId</param>
        /// <param name="competitionCode">The given competition code</param>
        private void setCompetitionCode(long packageID, string competitionCode)
        {
            if (!_packageCompetitionCodes.ContainsKey(packageID))
            {
                _packageCompetitionCodes.Add(packageID, new List<string> { competitionCode });
            }
            else
            {
                List<string> competitionCodes = _packageCompetitionCodes[packageID];
                if (!competitionCodes.Contains(competitionCode))
                {
                    competitionCodes.Add(competitionCode);
                }
            }
        }

        /// <summary>
        /// Creates subtype codes list 
        /// </summary>
        /// <param name="packageID">PackageId</param>
        /// <param name="subTypeCode">The given sub type code</param>
        private void setSubTypeCode(long packageID, string subTypeCode)
        {
            if (!_packageSubTypeCodes.ContainsKey(packageID))
            {
                _packageSubTypeCodes.Add(packageID, new List<string> { subTypeCode });
            }
            else
            {
                List<string> subTypeCodes = _packageSubTypeCodes[packageID];
                if (!subTypeCodes.Contains(subTypeCode))
                {
                    subTypeCodes.Add(subTypeCode);
                }
            }
        }

        #endregion
    }
}