using System;
using System.Data;
using TalentBusinessLogic.Models;
using System.Collections.Generic;
using TalentBusinessLogic.DataTransferObjects.Hospitality;
using Talent.Common;

namespace TalentBusinessLogic.ModelBuilders.Hospitality
{
    public class HospitalityDetailsBuilder : BaseModelBuilder
    {

        #region Public Functions

        /// <summary>
        /// Get the product and package details based on the selected product and package
        /// </summary>
        /// <param name="inputModel">The given input model</param>
        /// <returns>The formatted view model</returns>
        public HospitalityDetailsViewModel GetProductPackageDetails(HospitalityDetailsInputModel inputModel)
        {
            HospitalityDetailsViewModel viewModel = new HospitalityDetailsViewModel(true, "HospitalityFixturePackageHeader.ascx");
            BaseViewModel pageViewModel = new BaseViewModel(new WebFormResource(), true);
            ErrorObj err = new ErrorObj();
            DESettings settings = Environment.Settings.DESettings;
            DataSet dsPackageAndLeadSource = new DataSet();
            DataTable dtProductDetails = new DataTable();
            DataTable dtPackageDetails = new DataTable();
            DataTable dtLeadSourceDetails = new DataTable();            
            viewModel.ProductOppositionCode = String.Empty;
            viewModel.PackageDescription = String.Empty;
            viewModel.CustomerName = String.Empty;
            viewModel.CompanyName = String.Empty;
            viewModel.FormattedPackagePrice = String.Empty;

            dtProductDetails = retrieveProductDetails(inputModel.ProductCode);
            viewModel.Error = Data.PopulateErrorObject(err, dtProductDetails, settings);
            if (!viewModel.Error.HasError)
            {
                if (dtProductDetails.Rows.Count > 0)
                {
                    DataRow row = dtProductDetails.Rows[0];
                    DateTime dateUpdated = new DateTime();
                    dateUpdated = Utilities.ISeriesDate(row["ProductMDTE08"].ToString());
                    viewModel.ProductDate = dateUpdated.ToString(Environment.Settings.DefaultValues.GlobalDateFormat);
                    viewModel.ProductTime = row["ProductTime"].ToString();
                    viewModel.ProductDescription = row["ProductDescription"].ToString();
                    viewModel.ProductCompetitionDescription = row["ProductCompetitionDesc"].ToString();
                    viewModel.ProductOppositionCode = row["ProductOppositionCode"].ToString();
                    viewModel.HideDate = Utilities.CheckForDBNull_Boolean_DefaultFalse(row["HideDate"].ToString());
                    viewModel.HideTime = Utilities.CheckForDBNull_Boolean_DefaultFalse(row["HideTime"].ToString());
                    viewModel.ProductSubType = row["ProductSubType"].ToString();
                    viewModel.ProductSubTypeDesc = row["ProductSubTypeDesc"].ToString();
                    viewModel.ProductStadium = row["ProductStadium"].ToString();
                    viewModel.ProductType = row["ProductType"].ToString();
                }
                if (inputModel.PackageID.Length > 0)
                {
                    dsPackageAndLeadSource = retrievePackageDetails(inputModel.ProductCode);
                    viewModel.Error = Data.PopulateErrorObject(err, dsPackageAndLeadSource, settings, 4);
                    if (!viewModel.Error.HasError)
                    {
                        dtPackageDetails = dsPackageAndLeadSource.Tables["PackageList"];
                        if (dtPackageDetails.Rows.Count > 0)
                        {
                            foreach (DataRow row in dtPackageDetails.Rows)
                            {
                                if (row["PackageID"].ToString() == inputModel.PackageID)
                                {
                                    decimal packagePrice = 0;
                                    string formattedPrice = String.Empty;
                                    packagePrice = Convert.ToDecimal(row["Price"]);
                                    formattedPrice = TDataObjects.PaymentSettings.FormatCurrency(packagePrice, Environment.Settings.BusinessUnit, Environment.Settings.Partner);
                                    viewModel.FormattedPackagePrice = pageViewModel.GetPageText("packagePriceFormat");
                                    viewModel.FormattedPackagePrice = viewModel.FormattedPackagePrice.Replace("<<PackagePrice>>", formattedPrice);
                                    viewModel.PackageDescription = row["PackageDescription"].ToString();
                                    viewModel.PackageCode = row["PackageCode"].ToString();
                                    viewModel.DataCaptureTemplateID = Convert.ToDecimal(row["DataCaptureTemplateID"]);
                                    viewModel.PackageID = inputModel.PackageID;
                                    if (Environment.Settings.IsAgent == true)
                                    {
                                        viewModel.AllowBooking = true;
                                        if (viewModel.DataCaptureTemplateID != 0)
                                        {
                                            viewModel.AllowDataCapture = true;
                                        }
                                        else
                                        {
                                            viewModel.AllowDataCapture = false;
                                        }
                                    }
                                    else
                                    {
                                        if (viewModel.DataCaptureTemplateID != 0)
                                        {
                                            viewModel.AllowDataCapture = true;
                                            viewModel.AllowBooking = false;
                                        }
                                        else
                                        {
                                            viewModel.AllowDataCapture = false;
                                            viewModel.AllowBooking = true;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        dtLeadSourceDetails = dsPackageAndLeadSource.Tables["LeadSourceList"];
                        List<LeadSourceDetails> leadSourceList = Data.PopulateObjectListFromTable<LeadSourceDetails>(dtLeadSourceDetails);
                        viewModel.LeadSourceDetails = leadSourceList;
                    }
                }  
            }         
            return viewModel;
        }

        #endregion


        #region Private Functions
        
        /// <summary>
        /// Get the available packages for the given available product code - Call to WS002R
        /// </summary>
        /// <param name="productCode">The hospitality product code</param>
        /// <returns>DataTable of available corporate hospitality packages</returns>
        private DataSet retrievePackageDetails(string productCode)
        {
            DataSet dsTalProductDataSet = new DataSet();
            DataTable dtHospitalityPackages = new DataTable();            
            TalentProduct talProduct = new TalentProduct();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();
            ErrorModel errModel = new ErrorModel();
            talProduct.Settings = settings;
            talProduct.De.ProductCode = productCode;
            
            err = talProduct.ProductHospitality();
            errModel = Data.PopulateErrorObject(err, talProduct.ResultDataSet, settings, 4);
            if (!errModel.HasError) 
            {
                dsTalProductDataSet = talProduct.ResultDataSet;
            }
            return dsTalProductDataSet;
        }

        /// <summary>
        /// Get the product details for the given product code - Call to WS016R and filter the products
        /// </summary>
        /// <param name="productCode">The given product code</param>
        /// <returns>DataSet of product details</returns>
        private DataTable retrieveProductDetails(string productCode)
        {
            DataSet dsProductList = new DataSet();
            DataTable dtProduct = new DataTable();
            DataView dvProduct = new DataView();
            TalentProduct talProduct = new TalentProduct();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();
            ErrorModel errModel = new ErrorModel();
            string filter;
            talProduct.Settings = settings;
            talProduct.De.PriceAndAreaSelection = Environment.Settings.DefaultValues.PriceAndAreaSelection;
            talProduct.De.Src = GlobalConstants.SOURCE;
            talProduct.De.StadiumCode = Environment.Settings.DefaultValues.CorporateStadium;

            err = talProduct.ProductList();
            errModel = Data.PopulateErrorObject(err, talProduct.ResultDataSet.Tables[GlobalConstants.STATUS_RESULTS_TABLE_NAME], settings);
            if (!errModel.HasError) 
            {
                dsProductList = talProduct.ResultDataSet;
                filter = "ProductCode = '" + productCode + "'";
                dvProduct = new DataView(dsProductList.Tables["ProductListResults"]);
                dvProduct.RowFilter = filter;
                dtProduct = dvProduct.ToTable();
            }
            return dtProduct;
        }        
        #endregion
    }
}