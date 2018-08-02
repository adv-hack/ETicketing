using AutoMapper;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using TalentBusinessLogic.Models;
using TalentBusinessLogic.DataTransferObjects.Hospitality;
using TalentBusinessLogic.DataTransferObjects.Customer;
using TalentBusinessLogic.DataTransferObjects.Product;
using Talent.Common;
using TalentBusinessLogic.DataTransferObjects;

namespace TalentBusinessLogic.ModelBuilders.Hospitality
{
    public class HospitalityBookingBuilder : BaseModelBuilder
    {
        private bool _customerMatched = true;
        #region Public Functions

        /// <summary>
        /// Create the hospitality booking
        /// </summary>
        /// <param name="inputModel">The given hospitality booking input model</param>
        /// <returns>The formed hospitality booking view model</returns>
        public HospitalityBookingViewModel CreateHospitalityBooking(HospitalityBookingInputModel inputModel)
        {
            HospitalityBookingViewModel viewModel = new HospitalityBookingViewModel();
            TalentPackage package = new TalentPackage();
            TalentBasket talBasket = new TalentBasket();
            Mapper.CreateMap<HospitalityBookingInputModel, DEAddTicketingItems>();
            ErrorObj err = new ErrorObj();
            DEAddTicketingItems de = new DEAddTicketingItems();
            DESettings settings = Environment.Settings.DESettings;
            int resultsetTableCount = 3;

            package.RemoveCustomerPackageSession(inputModel.PackageID, inputModel.ProductCode, inputModel.CallId);
            de = Mapper.Map<DEAddTicketingItems>(inputModel);
            de.SessionId = inputModel.BasketID;
            de.Source = GlobalConstants.SOURCE;
            talBasket.DeAddTicketingItems = de;
            talBasket.Settings = settings;
            talBasket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(settings.BusinessUnit);
            talBasket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(settings.BusinessUnit);
            err = talBasket.AddTicketingItemsReturnBasket();

            if (talBasket.BasketHasExceptionSeats && talBasket.ResultDataSet.Tables.Contains("BasketDetailExceptions"))
            {
                //When there are season ticket exceptions, another table is added to the data set
                resultsetTableCount = 4;
            }
            viewModel.Error = Data.PopulateErrorObject(err, talBasket.ResultDataSet, settings, "BasketStatus", resultsetTableCount);
            return viewModel;
        }

        /// <summary>
        /// Retrieve the hospitality booking
        /// </summary>
        /// <param name="inputModel">The given hospitality booking input model</param>
        /// <returns>The formed hospitality booking view model</returns>
        public HospitalityBookingViewModel RetrieveHospitalityBooking(HospitalityBookingInputModel inputModel)
        {
            HospitalityBookingViewModel viewModel = new HospitalityBookingViewModel(true, "HospitalityBooking.aspx");
            DataSet dsBookingDetails = new DataSet();
            DataSet dsStandAreas = new DataSet();
            DataSet dsFriendsAndFamily = new DataSet();
            DataSet dsProductDetails = new DataSet();
            ErrorObj err = new ErrorObj();
            DESettings settings = Environment.Settings.DESettings;
            List<long> seatedComponentIds = new List<long>();
            DataTable dtQAndATemplete = new DataTable();
            DataTable dtBasketDetailExceptions = new DataTable();

            dsBookingDetails = retrieveBooking(inputModel.BasketID, inputModel.ProductCode, inputModel.PackageID, inputModel.CallId, inputModel.RefreshBasket, inputModel.IsSoldBooking, inputModel.CustomerNumber);
            viewModel.Error = Data.PopulateErrorObject(err, dsBookingDetails, settings, 6);
            if (!viewModel.Error.HasError && _customerMatched)
            {
                if (dsBookingDetails.Tables["Package"].Rows.Count > 0)
                {
                    dsStandAreas = retrieveStadiumStandAndAreas(Environment.Settings.DefaultValues.CorporateStadium);
                    viewModel.Error = Data.PopulateErrorObject(err, dsStandAreas, settings, 3);
                    if (!viewModel.Error.HasError)
                    {
                        dsProductDetails = retrieveProductDetails(inputModel.ProductCode);
                        viewModel.Error = Data.PopulateErrorObject(err, dsProductDetails, settings, 5);
                        if (!viewModel.Error.HasError)
                        {
                            dsFriendsAndFamily = retrieveFriendsAndFamily(inputModel.CustomerNumber);
                            viewModel.Error = Data.PopulateErrorObject(err, dsFriendsAndFamily, settings, 2);
                            if (viewModel.Error.ReturnCode == "NL")
                            {
                                //An NL error is returned when there are no F&F for the customer. Don't treat this as an error.
                                viewModel.Error.HasError = false;
                            }
                            if (!viewModel.Error.HasError)
                            {
                                if (dsProductDetails.Tables["ProductDetails"].Rows[0]["ProductType"].ToString() == GlobalConstants.SEASONTICKETPRODUCTTYPE)
                                {
                                    dtBasketDetailExceptions = TDataObjects.BasketSettings.TblBasketDetailExceptions.GetByBasketDetailHeaderIDModuleCorporateSTProductAndPackage(inputModel.BasketID, GlobalConstants.BASKETMODULETICKETING, inputModel.ProductCode, inputModel.PackageID);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //No package records for the given input model details 
                    viewModel.Error.HasError = true;
                }
            }

            if (!viewModel.Error.HasError && _customerMatched)
            {
                List<PackageDetails> packageList = new List<PackageDetails>();
                List<ComponentDetails> componentsList = new List<ComponentDetails>();
                List<ComponentDetails> extraComponentsList = new List<ComponentDetails>();
                List<ComponentDetails> filteredComponentsList = new List<ComponentDetails>();
                List<HospitalitySeatDetails> hospitalitySeatList = new List<HospitalitySeatDetails>();
                List<FriendsAndFamilyDetails> friendsAndFamiliyList = new List<FriendsAndFamilyDetails>();
                List<ProductPriceBandDetails> productPriceBandList = new List<ProductPriceBandDetails>();
                List<ProductPriceCodeDetails> productPriceCodeList = new List<ProductPriceCodeDetails>();
                List<HospitalitySeatDetails> tempHospitalitySeatList = new List<HospitalitySeatDetails>();
                List<SeasonTicketExceptions> seasonTicketExceptions = new List<SeasonTicketExceptions>();
                HospitalitySeatDetails seatedComponent = new HospitalitySeatDetails();
                HospitalitySeatDetails hospitalitySeat = new HospitalitySeatDetails();
                DESeatDetails seatDataEntity = new DESeatDetails();
                string currencyCode;
                List<ActivityQuestionAnswer> activityQuestionAnswerList = new List<ActivityQuestionAnswer>();
                bool questionAllowToAdd = true;

                currencyCode = TDataObjects.PaymentSettings.GetCurrencyCode(settings.BusinessUnit, settings.Partner);
                packageList = Data.PopulateObjectListFromTable<PackageDetails>(dsBookingDetails.Tables["Package"]);
                componentsList = Data.PopulateObjectListFromTable<ComponentDetails>(dsBookingDetails.Tables["Component"]);
                hospitalitySeatList = Data.PopulateObjectListFromTable<HospitalitySeatDetails>(dsBookingDetails.Tables["Seat"]);
                extraComponentsList = Data.PopulateObjectListFromTable<ComponentDetails>(dsBookingDetails.Tables["ExtraComponents"]);
                friendsAndFamiliyList = Data.PopulateObjectListFromTable<FriendsAndFamilyDetails>(dsFriendsAndFamily.Tables["FriendsAndFamily"]);
                productPriceBandList = Data.PopulateObjectListFromTable<ProductPriceBandDetails>(dsProductDetails.Tables["ProductPriceBands"]);
                productPriceCodeList = Data.PopulateObjectListFromTable<ProductPriceCodeDetails>(dsProductDetails.Tables["PriceCodes"]);
                seasonTicketExceptions = Data.PopulateObjectListFromTable<SeasonTicketExceptions>(dtBasketDetailExceptions);
                packageList[0].FormattedPriceBeforeVAT = TDataObjects.PaymentSettings.FormatCurrency(Convert.ToDecimal(packageList[0].PriceBeforeVAT), settings.BusinessUnit, settings.Partner);
                packageList[0].FormattedPriceIncludingVAT = TDataObjects.PaymentSettings.FormatCurrency(Convert.ToDecimal(packageList[0].PriceIncludingVAT), settings.BusinessUnit, settings.Partner);
                packageList[0].FormattedVATPrice = TDataObjects.PaymentSettings.FormatCurrency(Convert.ToDecimal(packageList[0].VATPrice), settings.BusinessUnit, settings.Partner);
                packageList[0].FormattedPackageDiscountedByValue = TDataObjects.PaymentSettings.FormatCurrency(Convert.ToDecimal(packageList[0].PackageDiscountedByValue), settings.BusinessUnit, settings.Partner);
                packageList[0].FormattedPackageComponentLevelDiscountValue = TDataObjects.PaymentSettings.FormatCurrency(Convert.ToDecimal(packageList[0].PackageComponentLevelDiscountValue), settings.BusinessUnit, settings.Partner);
                packageList[0].CurrencySymbol = TDataObjects.PaymentSettings.TblCurrency.GetCurrencySymbolByCurrencyCode(currencyCode);
                packageList[0].HasSTExceptions = dtBasketDetailExceptions.Rows.Count > 0 ? true : false;

                if (componentsList.Where(s=>s.HasPrintableTicket==true).ToList().Count>0)
                {
                    viewModel.HasPrintableTickets = true;                      
                }
                else
                {
                    viewModel.HasPrintableTickets = false;
                }

                foreach (ComponentDetails component in componentsList)
                {
                    decimal priceBeforeDiscount = 0;
                    component.HospitalitySeatDetailsList = new List<HospitalitySeatDetails>();
                    component.FormattedPriceBeforeVAT = TDataObjects.PaymentSettings.FormatCurrency(Convert.ToDecimal(component.PriceBeforeVAT), settings.BusinessUnit, settings.Partner);
                    component.FormattedPriceBeforeVATExclDisc = TDataObjects.PaymentSettings.FormatCurrency(Convert.ToDecimal(component.PriceBeforeVATExclDisc), settings.BusinessUnit, settings.Partner);
                    component.FormattedPriceIncludingVAT = TDataObjects.PaymentSettings.FormatCurrency(Convert.ToDecimal(component.PriceIncludingVAT), settings.BusinessUnit, settings.Partner);
                    component.FormattedVATPrice = TDataObjects.PaymentSettings.FormatCurrency(Convert.ToDecimal(component.VATPrice), settings.BusinessUnit, settings.Partner);
                    component.FormattedUnitPrice = TDataObjects.PaymentSettings.FormatCurrency(Convert.ToDecimal(component.UnitPrice), settings.BusinessUnit, settings.Partner);
                    priceBeforeDiscount = component.PriceBeforeVAT + component.DiscountValue;
                    component.FormattedPriceBeforeDiscount = TDataObjects.PaymentSettings.FormatCurrency(priceBeforeDiscount, settings.BusinessUnit, settings.Partner);
                    
                    if (component.CanAmendSeat)
                    {
                        tempHospitalitySeatList = hospitalitySeatList.Where(x => x.ComponentID == component.ComponentID).ToList<HospitalitySeatDetails>();
                        if (component.HasPrintableTicket)
                        {
                            viewModel.NumberOfTicketsToPrint += tempHospitalitySeatList.Count();
                        }
                        for (int i = 0; i < tempHospitalitySeatList.Count; i++)
                        {
                            if (component.ComponentType == "A")
                            {
                                if (packageList[0].TemplateID != 0)
                                {
                                    dtQAndATemplete = TDataObjects.ActivitiesSettings.GetExistingActivityQuestionByTemplateID(packageList[0].TemplateID.ToString());
                                    if (dtQAndATemplete.Rows.Count > 0)
                                    {
                                        foreach (DataRow row in dtQAndATemplete.Rows)
                                        {
                                            if (Utilities.CheckForDBNull_Boolean_DefaultFalse(row["ASK_QUESTION_PER_HOSPITALITY_BOOKING"]))
                                            {
                                                if(activityQuestionAnswerList.Exists(q => q.QuestionID == Convert.ToInt64(row["QUESTION_ID"].ToString())))
                                                {
                                                    questionAllowToAdd = false;
                                                }
                                            }
                                            else
                                            {
                                                questionAllowToAdd = true;
                                            }
                                            if (questionAllowToAdd)
                                            {
                                                ActivityQuestionAnswer activityQuestionAnswer = new ActivityQuestionAnswer();
                                                activityQuestionAnswer.QuestionID = Convert.ToInt64(row["QUESTION_ID"].ToString());
                                                activityQuestionAnswer.Answer = string.Empty;
                                                activityQuestionAnswer.TemplateID = packageList[0].TemplateID;
                                                activityQuestionAnswer.QuestionText = row["QUESTION_TEXT"].ToString();
                                                activityQuestionAnswer.AnswerType = Convert.ToInt32(row["ANSWER_TYPE"]);
                                                activityQuestionAnswer.RegularExpression = row["REGULAR_EXPRESSION"].ToString();
                                                activityQuestionAnswer.HyperLink = row["HYPERLINK"].ToString();
                                                activityQuestionAnswer.Sequence = Convert.ToInt64(row["SEQUENCE"].ToString());
                                                activityQuestionAnswer.ListOfAnswers = getListOfPreDefinedAnswers(activityQuestionAnswer.QuestionID.ToString());
                                                activityQuestionAnswer.TotalNumberOfSeats = tempHospitalitySeatList.Count;
                                                activityQuestionAnswer.NumberOfQuestions = dtQAndATemplete.Rows.Count;
                                                if (Utilities.CheckForDBNull_Boolean_DefaultFalse(row["ASK_QUESTION_PER_HOSPITALITY_BOOKING"]))
                                                {
                                                    activityQuestionAnswer.IsQuestionPerBooking = true;
                                                }
                                                else
                                                {
                                                    activityQuestionAnswer.IsQuestionPerBooking = false;
                                                }
                                                activityQuestionAnswerList.Add(activityQuestionAnswer);
                                            }
                                        }
                                    }
                                }
                            }
                            seatedComponent = tempHospitalitySeatList[i];
                            seatedComponent.FriendsAndFamilyDetails = friendsAndFamiliyList;
                            seatedComponent.ProductPriceBands = returnValidSeatPriceBands(seatedComponent.ValidPriceBands, productPriceBandList);
                            if (seatedComponent.ProductPriceBands.Count == 0)
                            { 
                                if(!String.IsNullOrEmpty(seatedComponent.DefaultProductPriceBand))
                                {
                                    seatedComponent.ProductPriceBands = returnValidSeatPriceBands(seatedComponent.DefaultProductPriceBand, productPriceBandList);
                                }
                                else
                                {
                                    seatedComponent.ProductPriceBands = returnValidSeatPriceBands(dsBookingDetails.Tables["Seat"].Rows[i]["productdefaultpriceband"].ToString(), productPriceBandList);
                                }
                            }
                            seatedComponent.ProductPriceCodes = productPriceCodeList;
                            seatDataEntity.UnFormattedSeat = seatedComponent.SeatDetails;
                            seatedComponent.FormattedSeatDetails = seatDataEntity.FormattedSeat;
                            seatedComponent.StandCode = seatDataEntity.Stand;
                            seatedComponent.AreaCode = seatDataEntity.Area;
                            seatedComponent.SeasonTicketExceptions = getSTExceptionsSeatDetails(seatedComponent.SeatDetails, seasonTicketExceptions);
                            setStandAreaDescription(ref dsStandAreas, ref seatedComponent);
                            setSeatValuesFromComponent(ref seatedComponent, ref componentsList, ref viewModel); //pass component rather than componentList
                            seatedComponent.ComponentType = component.ComponentType;
                            component.HospitalitySeatDetailsList.Add(seatedComponent);
                        }
                    }
                    if (!seatedComponentIds.Contains(component.ComponentID))
                    {
                        filteredComponentsList.Add(component);
                    }
                }

                if (packageList[0].TemplateID != 0)
                {
                    if (dtQAndATemplete.Rows.Count > 0)
                    {
                        if (inputModel.CallId == 0)
                        {
                            inputModel.CallId = packageList[0].CallID;
                        }
                        DataSet dsSavedQAndA = getSavedQAndAofCustomer(inputModel, dtQAndATemplete);
                        viewModel.Error = Data.PopulateErrorObject(err, dsSavedQAndA, settings, 3);
                        if (!viewModel.Error.HasError)
                        {
                            if (dsSavedQAndA != null && dsSavedQAndA.Tables["CurrentQuestionsAndAnswers"].Rows.Count > 0)
                            {
                                List<ActivityQuestionAnswer> currentActivityQuestionAnswerList = new List<ActivityQuestionAnswer>();
                                foreach (DataRow currentQandA in dsSavedQAndA.Tables["CurrentQuestionsAndAnswers"].Rows)
                                {

                                    foreach (DataRow row in dtQAndATemplete.Rows)
                                    {
                                        ActivityQuestionAnswer activityQuestionAnswer = new ActivityQuestionAnswer();
                                        activityQuestionAnswer.QuestionID = Convert.ToInt64(row["QUESTION_ID"].ToString());
                                        if (activityQuestionAnswer.QuestionID == Convert.ToInt64(currentQandA["QuestionID"].ToString()))
                                        {
                                            activityQuestionAnswer.Answer = currentQandA["Answer"].ToString();
                                            activityQuestionAnswer.TemplateID = packageList[0].TemplateID;
                                            activityQuestionAnswer.QuestionText = row["QUESTION_TEXT"].ToString();
                                            activityQuestionAnswer.AnswerType = Convert.ToInt32(row["ANSWER_TYPE"]);
                                            activityQuestionAnswer.RegularExpression = row["REGULAR_EXPRESSION"].ToString();
                                            activityQuestionAnswer.HyperLink = row["HYPERLINK"].ToString();
                                            activityQuestionAnswer.Sequence = Convert.ToInt64(row["SEQUENCE"].ToString());
                                            activityQuestionAnswer.ListOfAnswers = getListOfPreDefinedAnswers(activityQuestionAnswer.QuestionID.ToString());
                                            activityQuestionAnswer.TotalNumberOfSeats = tempHospitalitySeatList.Count;
                                            activityQuestionAnswer.NumberOfQuestions = dtQAndATemplete.Rows.Count;
                                            if (Utilities.CheckForDBNull_Boolean_DefaultFalse(row["ASK_QUESTION_PER_HOSPITALITY_BOOKING"]))
                                            {
                                                activityQuestionAnswer.IsQuestionPerBooking = true;
                                            }
                                            else
                                            {
                                                activityQuestionAnswer.IsQuestionPerBooking = false;
                                            }
                                            currentActivityQuestionAnswerList.Add(activityQuestionAnswer);
                                            break;
                                        }
                                    }
                                }
                                if (currentActivityQuestionAnswerList.Count > activityQuestionAnswerList.Count)
                                {
                                    // In case of remove 
                                    for (int i = 0; i < activityQuestionAnswerList.Count; i++)
                                    {
                                        activityQuestionAnswerList[i] = currentActivityQuestionAnswerList[i];
                                    }
                                }
                                else
                                {
                                    // In case of add extra seat
                                    for (int i = 0; i < currentActivityQuestionAnswerList.Count; i++)
                                    {
                                        activityQuestionAnswerList[i] = currentActivityQuestionAnswerList[i];
                                    }
                                }
                            }
                            else if (dsSavedQAndA != null && dsSavedQAndA.Tables["PreviousQuestionsAndAnswers"].Rows.Count > 0)
                            {
                                List<ActivityQuestionAnswer> previousActivityQuestionAnswerList = new List<ActivityQuestionAnswer>();
                                foreach (DataRow previousQandA in dsSavedQAndA.Tables["PreviousQuestionsAndAnswers"].Rows)
                                {
                                    foreach (DataRow row in dtQAndATemplete.Rows)
                                    {
                                        ActivityQuestionAnswer activityQuestionAnswer = new ActivityQuestionAnswer();
                                        activityQuestionAnswer.QuestionID = Convert.ToInt64(row["QUESTION_ID"].ToString());
                                        if (activityQuestionAnswer.QuestionID == Convert.ToInt64(previousQandA["QuestionID"].ToString()) && inputModel.CallId != 0 && inputModel.CallId == Convert.ToInt64(previousQandA["CallID"].ToString()))
                                        {
                                            activityQuestionAnswer.Answer = previousQandA["Answer"].ToString();
                                            activityQuestionAnswer.TemplateID = packageList[0].TemplateID;
                                            activityQuestionAnswer.QuestionText = row["QUESTION_TEXT"].ToString();
                                            activityQuestionAnswer.AnswerType = Convert.ToInt32(row["ANSWER_TYPE"]);
                                            activityQuestionAnswer.RegularExpression = row["REGULAR_EXPRESSION"].ToString();
                                            activityQuestionAnswer.HyperLink = row["HYPERLINK"].ToString();
                                            activityQuestionAnswer.Sequence = Convert.ToInt64(row["SEQUENCE"].ToString());
                                            activityQuestionAnswer.ListOfAnswers = getListOfPreDefinedAnswers(activityQuestionAnswer.QuestionID.ToString());
                                            activityQuestionAnswer.TotalNumberOfSeats = tempHospitalitySeatList.Count;
                                            activityQuestionAnswer.NumberOfQuestions = dtQAndATemplete.Rows.Count;
                                            if (Utilities.CheckForDBNull_Boolean_DefaultFalse(row["ASK_QUESTION_PER_HOSPITALITY_BOOKING"]))
                                            {
                                                activityQuestionAnswer.IsQuestionPerBooking = true;
                                            }
                                            else
                                            {
                                                activityQuestionAnswer.IsQuestionPerBooking = false;
                                            }
                                            previousActivityQuestionAnswerList.Add(activityQuestionAnswer);
                                            break;
                                        }
                                    }
                                }
                                if (previousActivityQuestionAnswerList.Count > activityQuestionAnswerList.Count)
                                {
                                    // In case of remove 
                                    for (int i = 0; i < activityQuestionAnswerList.Count; i++)
                                    {
                                        activityQuestionAnswerList[i] = previousActivityQuestionAnswerList[i];
                                    }
                                }
                                else
                                {
                                    // In case of add extra seat
                                    for (int i = 0; i < previousActivityQuestionAnswerList.Count; i++)
                                    {
                                        activityQuestionAnswerList[i] = previousActivityQuestionAnswerList[i];
                                    }
                                }
                            }
                        }
                        else
                        {
                            viewModel.Error.HasError = true;
                        }
                    }

                }

                //Set view model
                viewModel.PackageDetailsList = packageList;
                viewModel.ComponentDetailsList = componentsList;
                viewModel.ActivityQuestionAnswerList = activityQuestionAnswerList;
                viewModel.ExtraComponentDetailsList = extraComponentsList;
            }
            if (!_customerMatched)
            {
                viewModel.Error.HasError = true;
                viewModel.CustomerMatched = false;
            }
            return viewModel;
        }

        /// <summary>
        /// Update/Amend the hospitality booking
        /// </summary>
        /// <param name="inputModel">The given hospitality booking input model</param>
        /// <returns>The formed hospitality booking view model</returns>
        public HospitalityBookingViewModel UpdateHospitalityBooking(HospitalityBookingInputModel inputModel)
        {
            HospitalityBookingViewModel viewModel = new HospitalityBookingViewModel(true);
            TalentPackage talPackage = new TalentPackage();
            Mapper.CreateMap<HospitalityBookingInputModel, DEPackages>();
            ErrorObj err = new ErrorObj();
            DEPackages de = new DEPackages();
            DESettings settings = Environment.Settings.DESettings;

            de = Mapper.Map<DEPackages>(inputModel);
            if (Environment.Settings.IsAgent == true)
            {
                de.BoxOfficeUser = Environment.Settings.DESettings.AgentEntity.AgentUsername;
            }
            talPackage.Settings = settings;
            talPackage.DePackages = de;
            err = talPackage.UpdateCustomerComponentDetails();
            viewModel.Error = Data.PopulateErrorObject(err, talPackage.ResultDataSet, settings, 6);

            if (!viewModel.Error.HasError)
            {
                updateProductQuestionAnswer(ref inputModel, ref viewModel);
            }

            return viewModel;
        }

        /// <summary>
        /// Update/Amend the hospitality sold booking
        /// </summary>
        /// <param name="inputModel">The given hospitality booking input model</param>
        /// <returns>The formed hospitality booking view model</returns>
        public HospitalityBookingViewModel UpdateHospitalityForSoldBooking(HospitalityBookingInputModel inputModel)
        {
            HospitalityBookingViewModel viewModel = new HospitalityBookingViewModel(true);
            ErrorObj err = new ErrorObj();
            updateProductQuestionAnswer(ref inputModel, ref viewModel);
            return viewModel;
        }

        /// <summary>
        /// Cancel the hospitality booking
        /// </summary>
        /// <param name="inputModel">The given hospitality booking input model</param>
        /// <returns>The formed hospitality booking view model</returns>
        public HospitalityBookingViewModel DeleteHospitalityBooking(HospitalityBookingInputModel inputModel)
        {
            HospitalityBookingViewModel viewModel = new HospitalityBookingViewModel();
            TalentPackage package = new TalentPackage();
            TalentBasket talBasket = new TalentBasket();
            Mapper.CreateMap<HospitalityBookingInputModel, DETicketingItemDetails>();
            ErrorObj err = new ErrorObj();
            DETicketingItemDetails de = new DETicketingItemDetails();
            DESettings settings = Environment.Settings.DESettings;
            DataSet dsResults = new DataSet();

            package.RemoveCustomerPackageSession(inputModel.PackageID, inputModel.ProductCode, inputModel.CallId);
            de = Mapper.Map<DETicketingItemDetails>(inputModel);
            de.SessionId = inputModel.BasketID;
            de.Src = GlobalConstants.SOURCE;
            talBasket.De = de;
            talBasket.Settings = settings;
            talBasket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(settings.BusinessUnit);
            talBasket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(settings.BusinessUnit);
            err = talBasket.RemoveTicketingItemsReturnBasket();

            viewModel.Error = Data.PopulateErrorObject(err, talBasket.ResultDataSet, settings, "BasketStatus", 3);
            return viewModel;
        }

        /// <summary>
        /// Save the hospitality booking as an enquiry
        /// </summary>
        /// <param name="inputModel">The given hospitality booking input model</param>
        /// <returns>The formed hospitality booking view model</returns>
        public HospitalityBookingViewModel SaveHospitalityBookingAsEnquiry(HospitalityBookingInputModel inputModel)
        {
            HospitalityBookingViewModel viewModel = new HospitalityBookingViewModel(true);
            TalentPackage package = new TalentPackage();
            TalentBasket talBasket = new TalentBasket();
            ErrorObj err = new ErrorObj();
            DETicketingItemDetails de = new DETicketingItemDetails();
            DEPackages dePack = new DEPackages();
            DESettings settings = Environment.Settings.DESettings;
            DataSet dsResults = new DataSet();

            // First Attempt to Update the status fo the header before clearing out the basket.
            Mapper.CreateMap<HospitalityBookingInputModel, DEPackages>();
            dePack = Mapper.Map<DEPackages>(inputModel);
            if (Environment.Settings.IsAgent == true)
            {
                dePack.BoxOfficeUser = Environment.Settings.DESettings.AgentEntity.AgentUsername;
            }
            dePack.Source = GlobalConstants.SOURCE;
            package.Settings = settings;
            package.DePackages = dePack;
            err = package.UpdateHospitalityBookingStatus();

            viewModel.Error = Data.PopulateErrorObject(err, package.ResultDataSet, settings, 1);
            if (!viewModel.Error.HasError)
            {
                package.RemoveCustomerPackageSession(inputModel.PackageID, inputModel.ProductCode, inputModel.CallId);
                Mapper.CreateMap<HospitalityBookingInputModel, DETicketingItemDetails>();
                de = Mapper.Map<DETicketingItemDetails>(inputModel);
                de.SessionId = inputModel.BasketID;
                de.Src = GlobalConstants.SOURCE;
                talBasket.De = de;
                talBasket.Settings = settings;
                talBasket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(settings.BusinessUnit);
                talBasket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(settings.BusinessUnit);
                err = talBasket.RemoveTicketingItemsReturnBasket();

                viewModel.Error = Data.PopulateErrorObject(err, talBasket.ResultDataSet, settings, "BasketStatus", 3);
            }
            return viewModel;
        }

        /// <summary>
        /// Add Non-seated components to Extra section in Booking
        /// </summary>
        /// <param name="inputModel">The given hospitality booking input model</param>
        /// <returns>The formed hospitality booking view model</returns>
        public HospitalityBookingViewModel AddExtraNonSeatedComponent(HospitalityBookingInputModel inputModel)
        {
            HospitalityBookingViewModel viewModel = new HospitalityBookingViewModel(true);
            TalentPackage talPackage = new TalentPackage();
            Mapper.CreateMap<HospitalityBookingInputModel, DEPackages>();
            ErrorObj err = new ErrorObj();
            DEPackages de = new DEPackages();
            DESettings settings = Environment.Settings.DESettings;

            de = Mapper.Map<DEPackages>(inputModel);
            if (Environment.Settings.IsAgent == true)
            {
                de.BoxOfficeUser = Environment.Settings.DESettings.AgentEntity.AgentUsername;
            }
            talPackage.Settings = settings;
            talPackage.DePackages = de;
            err = talPackage.UpdateCustomerComponentDetails();
            viewModel.Error = Data.PopulateErrorObject(err, talPackage.ResultDataSet, settings, 6);

            if (!viewModel.Error.HasError)
            {
                updateProductQuestionAnswer(ref inputModel, ref viewModel);
            }

            return viewModel;
        }

        /// <summary>
        /// Hospitality booking with print request status.
        /// </summary>
        /// <param name="inputModel">The given hospitality booking  input model</param>
        /// <returns>Status of Print Request</returns>
        public HospitalityBookingViewModel PrintHospitalityBooking(HospitalityBookingInputModel inputModel)
        {
            HospitalityBookingViewModel viewModel = new HospitalityBookingViewModel(true);
            DataSet dsPrintBooking = new DataSet();
            ErrorObj err = new ErrorObj();
            DESettings settings = Environment.Settings.DESettings;

            viewModel = RetrieveHospitalityBooking(inputModel);

            if (!viewModel.Error.HasError)
            {
                dsPrintBooking = printBooking(inputModel);
                viewModel.Error = Data.PopulateErrorObject(err, dsPrintBooking, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 1);
                if (!viewModel.Error.HasError)
                {
                    DataTable dtStatusResult = dsPrintBooking.Tables[GlobalConstants.STATUS_RESULTS_TABLE_NAME];
                    if (dtStatusResult.Rows.Count > 0 && !string.IsNullOrEmpty(dtStatusResult.Rows[0]["ReturnCode"].ToString()))
                    {
                        viewModel.PrintRequestSucceeded = false;
                    }
                    else
                    {
                        viewModel.PrintRequestSucceeded = true;
                    }
                }
            }
            return viewModel;
        }

        /// <summary>
        /// Hospitality booking with document generation request status.
        /// </summary>
        /// <param name="inputModel">The given hospitality booking  input model</param>
        /// <returns>Status of Document Creation Request</returns>
        public HospitalityBookingViewModel GenerateDocumentForBooking(HospitalityBookingInputModel inputModel)
        {
            HospitalityBookingViewModel viewModel = new HospitalityBookingViewModel(true);
            DataSet dsDocumentBooking = new DataSet();
            ErrorObj err = new ErrorObj();
            DESettings settings = Environment.Settings.DESettings;

            viewModel = RetrieveHospitalityBooking(inputModel);

            if (!viewModel.Error.HasError)
            {
                dsDocumentBooking = createDocument(inputModel);
                viewModel.Error = Data.PopulateErrorObject(err, dsDocumentBooking, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 2);
                if (!viewModel.Error.HasError)
                {
                    DataTable dtStatusResult = dsDocumentBooking.Tables[GlobalConstants.STATUS_RESULTS_TABLE_NAME];
                    DataTable dtDocumentInformation = dsDocumentBooking.Tables["DocumentInformation"];
                    if (dtStatusResult.Rows.Count > 0 && !string.IsNullOrEmpty(dtStatusResult.Rows[0]["ReturnCode"].ToString()))
                    {
                        viewModel.GenerateDocumentRequestSuccess = false;
                    }
                    else
                    {
                        viewModel.GenerateDocumentRequestSuccess = true;
                        viewModel.CallId = inputModel.CallIdForDocumentProduction;
                        viewModel.MergedWordDocumentPath = dtDocumentInformation.Rows[0].ItemArray[1].ToString();
                    }
                }
            }
            return viewModel;
        }

        #endregion

        #region Private Functions



        /// <summary>
        /// Get the Hospitality Booking information from WS003, WS009 and the C#xx files based on the given parameters
        /// </summary>
        /// <param name="basketID">The current user basket unique ID</param>
        /// <param name="productCode">The ticketing product code the booking goes with</param>
        /// <param name="packageID">The hospitality package ID the booking goes with</param>
        /// <param name="CallId">The hospitality Call ID (booking reference)</param>
        /// <param name="refreshBasket">Force the retrieval of the booking to also retrieve the basket information</param>
        /// <param name="isSoldBooking">Flag to indicate if this booking is sold (true) or not (false)</param>
        /// <returns>A data set of results for all the hospitality information</returns>
        private DataSet retrieveBooking(string basketID, string productCode, long packageID, long CallId, bool refreshBasket, bool isSoldBooking, string loginCustomerNumber)
        {
            DataSet dsResults = new DataSet();
            TalentPackage talPackage = new TalentPackage();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();

            talPackage.Settings = settings;
            talPackage.DePackages.BasketId = basketID;
            talPackage.DePackages.ProductCode = productCode;
            talPackage.DePackages.PackageID = packageID;
            talPackage.DePackages.CallId = CallId;
            talPackage.DePackages.Source = GlobalConstants.SOURCE;
            talPackage.DePackages.BusinessUnit = settings.BusinessUnit;
            if (Environment.Settings.IsAgent == true)
            {
                talPackage.DePackages.BoxOfficeUser = Environment.Settings.DESettings.AgentEntity.AgentUsername;
            }

            if (isSoldBooking)
            {
                err = talPackage.GetSoldHospitalityBookingDetails();
                if (Environment.Settings.IsAgent == false)
                {
                    if (!err.HasError && talPackage.ResultDataSet != null && talPackage.ResultDataSet.Tables["Package"].Rows.Count > 0)
                    {
                        string bookingCustomer = talPackage.ResultDataSet.Tables["Package"].Rows[0]["BookingCustomerNumber"].ToString();
                        if (String.Compare(bookingCustomer, loginCustomerNumber, true) != 0 && !String.IsNullOrEmpty(bookingCustomer))
                        {
                            _customerMatched = false;
                        }
                    }
                }
            }
            else
            {
                err = talPackage.GetCustomerPackageInformation(false, 0, refreshBasket);
                talPackage.RemoveCustomerPackageSession(talPackage.DePackages.PackageID, talPackage.DePackages.ProductCode, talPackage.DePackages.CallId);
            }

            dsResults = talPackage.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        /// Get the stadium stand and area data from WS118R
        /// </summary>
        /// <param name="stadium">The stadium code to retrieve descriptions and codes from</param>
        /// <returns>A data set of results for the stadium data</returns>
        private DataSet retrieveStadiumStandAndAreas(string stadium)
        {
            DataSet dsResults = new DataSet();
            TalentProduct talProduct = new TalentProduct();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();

            talProduct.Settings = settings;
            talProduct.De.StadiumCode = stadium;
            err = talProduct.StandDescriptions();
            dsResults = talProduct.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        /// Get the product details from WS007R
        /// </summary>
        /// <param name="productCode">The product code to retrieve details for</param>
        /// <returns>A data set of results for the product</returns>
        private DataSet retrieveProductDetails(string productCode)
        {
            DataSet dsResults = new DataSet();
            TalentProduct talProduct = new TalentProduct();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();

            talProduct.Settings = settings;
            talProduct.De.ProductCode = productCode;
            err = talProduct.ProductDetails();
            dsResults = talProduct.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        /// Get the customer friends and family details from WS026R
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        private DataSet retrieveFriendsAndFamily(string customer)
        {
            DataSet dsResults = new DataSet();
            TalentCustomer talCustomer = new TalentCustomer();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();
            DECustomerV11 deCustV11 = new DECustomerV11();
            DECustomer deCust = new DECustomer();

            deCust.CustomerNumber = customer;
            deCust.Source = GlobalConstants.SOURCE;
            deCust.IncludeBoxOfficeLinks = (Environment.Settings.IsAgent == true);
            deCustV11.DECustomersV1.Add(deCust);
            talCustomer.Settings = settings;
            talCustomer.DeV11 = deCustV11;
            err = talCustomer.CustomerAssociations();
            dsResults = talCustomer.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        /// Get the valid product price bands with descriptions for each seat from the product details
        /// </summary>
        /// <param name="validPriceBandsForSeat">The concatenated list of valid product price bands for the seat</param>
        /// <param name="productPriceBands">The list of price bands for the product</param>
        /// <returns>A list of price bands with descriptions etc for the product that are valid</returns>
        private List<ProductPriceBandDetails> returnValidSeatPriceBands(string validPriceBandsForSeat, List<ProductPriceBandDetails> productPriceBands)
        {
            List<ProductPriceBandDetails> ValidSeatPriceBands = new List<ProductPriceBandDetails>();
            foreach (ProductPriceBandDetails pb in productPriceBands)
            {
                if (validPriceBandsForSeat.Contains(pb.PriceBand)) ValidSeatPriceBands.Add(pb);
            }
            return ValidSeatPriceBands;            
        }

        /// <summary>
        /// Get saved Question And Anser of Customer for backend
        /// </summary>
        /// <param name="inputModel">Object of input model</param>
        /// <param name="dtQAndATemplete">QAndA data table</param>
        /// <returns></returns>
        private DataSet getSavedQAndAofCustomer(HospitalityBookingInputModel inputModel, DataTable dtQAndATemplete)
        {
            TalentProduct product = new TalentProduct();
            product.Settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();
            DataSet dsResults = new DataSet();
            DEProduct productEntity = new DEProduct();
            DEProductQuestionsAnswers questionAnswers = new DEProductQuestionsAnswers();
            List<DEProductQuestionsAnswers> dEProductQuestionsAnswersList = new List<DEProductQuestionsAnswers>();
            foreach (DataRow row in dtQAndATemplete.Rows)
            {
                DEProductQuestionsAnswers de = new DEProductQuestionsAnswers();
                de.QuestionID = Convert.ToInt32(row["QUESTION_ID"]);
                de.QuestionText = row["QUESTION_TEXT"].ToString();
                de.AnswerText = string.Empty;
                de.CallID = inputModel.CallId;
                de.TemplateID = 0;
                de.CustomerNumber = inputModel.CustomerNumber;
                de.AllocationCustomerNumber = inputModel.CustomerNumber;
                de.ProductCode = inputModel.ProductCode;
                de.DateTime = DateTime.Now;
                de.Source = GlobalConstants.SOURCE;
                dEProductQuestionsAnswersList.Add(de);
            }

            questionAnswers.BasketID = inputModel.BasketID;
            questionAnswers.DateTime = DateTime.Now;
            questionAnswers.AlphaSeat = ' ';
            questionAnswers.SeatData = string.Empty;
            productEntity.ProductQuestionAnswers = questionAnswers;
            productEntity.CollDEProductQuestionAnswers = dEProductQuestionsAnswersList;
            productEntity.ProductQuestionAnswers.CustomerNumber = inputModel.CustomerNumber;
            productEntity.ProductQuestionAnswers.CallID = inputModel.CallId;
            product.Dep = productEntity;
            err = product.RetrieveProductQuestionAnswers();
            dsResults = product.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        /// Get season ticket exception details for each component seat
        /// </summary>
        /// <param name="seatDetails">The component seat details</param>
        /// <param name="seasonTicketExceptions">The list of season ticket exceptions</param>
        /// <returns>A list of SeasonTicketExceptions for each seated component</returns>
        private List<SeasonTicketExceptions> getSTExceptionsSeatDetails(string seatDetails, List<SeasonTicketExceptions> seasonTicketExceptions)
        {
            List<SeasonTicketExceptions> SeatExceptions = new List<SeasonTicketExceptions>();

            foreach (SeasonTicketExceptions exception in seasonTicketExceptions)
            {
                if (exception.SeasonTicketSeat.ToString().Trim() == seatDetails.ToString().Trim())
                {
                    DateTime dateUpdated = new DateTime();
                    dateUpdated = Utilities.ISeriesDate(exception.ProductDate);
                    exception.ProductDate = dateUpdated.ToString(Environment.Settings.DefaultValues.GlobalDateFormat);
                    SeatExceptions.Add(exception);
                }
            }

            return SeatExceptions;
        }
        /// <summary>
        /// Get list of pre defined answers of question
        /// </summary>
        /// <param name="questionId">question id</param>
        /// <returns></returns>
        private Dictionary<long, string> getListOfPreDefinedAnswers(string questionId)
        {
            Dictionary<long, string> getListOfAnswer = new Dictionary<long, string>();
            DataTable qAndAdt = TDataObjects.ActivitiesSettings.GetAnswerByQuestionID(questionId);
            if (qAndAdt.Rows.Count > 0)
            {
                getListOfAnswer = qAndAdt.AsEnumerable().ToDictionary<DataRow, long, string>(qaRow => qaRow.Field<long>(0), qaRow => qaRow.Field<string>(1));
            }
            return getListOfAnswer;
        }
        
        /// <summary>
        ///Print Request status for sold Hospitality Booking(for single seat and single reference both).
        /// </summary>
        /// <param name="inputModel">The given hospitality booking  input model</param>
        /// <returns>A data set of result with  print request status</returns>
        private DataSet printBooking(HospitalityBookingInputModel inputModel)
        {
            DataSet dsResults = new DataSet();
            TalentPackage talPackage = new TalentPackage();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();
            talPackage.Settings = settings;
            talPackage.DePackages.HospitalityBookingFilters = new HospitalityBookingFilters();      
            talPackage.DePackages.BoxOfficeUser = inputModel.BoxOfficeUser;
            talPackage.DePackages.CallId = Convert.ToInt64(inputModel.CallIdToBePrinted);
            talPackage.DePackages.ProductCodeToBePrinted = inputModel.ProductCodeToBePrinted;
            talPackage.DePackages.SeatToBePrinted = inputModel.SeatToBePrinted;
            talPackage.DePackages.ComponentID =Convert.ToInt64(inputModel.ComponentID);
            err = talPackage.PrintHospitalityBookings();
            dsResults = talPackage.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        /// Generate the Document for the booking
        /// </summary>
        /// <param name="inputModel">The given hospitality booking  input model</param>
        /// <returns>A data set of result with  document creation request status</returns>
        private DataSet createDocument(HospitalityBookingInputModel inputModel)
        {
            DataSet dsResults = new DataSet();
            TalentPackage talPackage = new TalentPackage();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();
            talPackage.Settings = settings;
            talPackage.DePackages.HospitalityBookingFilters = new HospitalityBookingFilters();
            talPackage.DePackages.BoxOfficeUser = inputModel.BoxOfficeUser;
            talPackage.DePackages.CallId = inputModel.CallIdForDocumentProduction;
            talPackage.DePackages.CustomerNumber = inputModel.CustomerNumber;
            err = talPackage.CreateHospitalityBookingDocument();
            dsResults = talPackage.ResultDataSet;
            return dsResults;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Amend/Update the product Question Answer
        /// </summary>
        /// <param name="inputModel">HospitalityBookingInputModel</param>
        /// <param name="err">ErrorObject</param>
        /// <param name="viewModel">HospitalityBookingViewModel</param>
        /// <returns>HospitalityBookingViewModel</returns>
        private void updateProductQuestionAnswer(ref HospitalityBookingInputModel inputModel, ref HospitalityBookingViewModel viewModel)
        {
            DEProductDetails productDetails = new DEProductDetails();
            TalentProduct product = new Talent.Common.TalentProduct();
            product.Settings = Environment.Settings.DESettings;
            DataTable dtQAndATemplete = new DataTable();
            ErrorObj err = new ErrorObj();
            bool questionAllowToAdd = true;
            if (inputModel.ActivityQuestionAnswerList != null && inputModel.ActivityQuestionAnswerList.Count > 0)
            {
                dtQAndATemplete = TDataObjects.ActivitiesSettings.GetExistingActivityQuestionByTemplateID(inputModel.ActivityQuestionAnswerList[0].TemplateID.ToString());

                DEProduct productEntity = new DEProduct();
                List<DEProductQuestionsAnswers> dEProductQuestionsAnswersList = new List<DEProductQuestionsAnswers>();

                foreach (ActivityQuestionAnswer activityQuestionAnswer in inputModel.ActivityQuestionAnswerList)
                {
                    foreach (DataRow row in dtQAndATemplete.Rows)
                    {
                        if (activityQuestionAnswer.QuestionID == Convert.ToInt32(row["QUESTION_ID"].ToString()))
                        {
                            if (Utilities.CheckForDBNull_Boolean_DefaultFalse(row["ASK_QUESTION_PER_HOSPITALITY_BOOKING"]))
                            {
                                if (dEProductQuestionsAnswersList.Exists(q => q.QuestionID == Convert.ToInt64(row["QUESTION_ID"].ToString())))
                                {
                                    questionAllowToAdd = false;
                                }
                            }
                            else
                            {
                                questionAllowToAdd = true;
                            }
                            if (questionAllowToAdd)
                            {
                                DEProductQuestionsAnswers dep = new DEProductQuestionsAnswers();
                                dep.QuestionID = Convert.ToInt32(row["QUESTION_ID"]);
                                dep.QuestionText = row["QUESTION_TEXT"].ToString();
                                dep.AnswerText = activityQuestionAnswer.Answer;
                                dep.CallID = inputModel.CallId;
                                dep.TemplateID = activityQuestionAnswer.TemplateID;
                                dep.CustomerNumber = inputModel.CustomerNumber;
                                dep.AllocationCustomerNumber = inputModel.CustomerNumber;
                                dep.ProductCode = inputModel.ProductCode;
                                dep.PriceBand = ' ';
                                dep.RememberedAnswer = activityQuestionAnswer.RememberedAnswer;
                                dep.QuestionPerBooking = activityQuestionAnswer.IsQuestionPerBooking;
                                dep.SeatData = string.Empty;
                                dep.AlphaSeat = ' ';
                                dep.BasketID = inputModel.BasketID;
                                dep.NewCallFlag = string.Empty;
                                dep.ProductDescription = string.Empty;
                                dep.Source = GlobalConstants.SOURCE;
                                if (Environment.Settings.IsAgent == true)
                                {
                                    dep.AgentName = Environment.Settings.DESettings.AgentEntity.AgentUsername;
                                }
                                dEProductQuestionsAnswersList.Add(dep);
                            }
                        }
                    }
                }

                productDetails.SessionId = inputModel.BasketID;
                productEntity.CollDEProductQuestionAnswers = dEProductQuestionsAnswersList;
                product.Dep = productEntity;
                product.De = productDetails;
                err = product.AddProductQuestionAnswers();
                viewModel.Error = Data.PopulateErrorObject(err, product.ResultDataSet, Environment.Settings.DESettings, 1);
            }
        }

        /// <summary>
        /// Set the stand and area descriptions against the seat based on the dataset of stadium data
        /// </summary>
        /// <param name="dsStandAreas">The data set of stadium stand, area codes and descriptions</param>
        /// <param name="seat">The hospitality seat object to set descriptions for</param>
        private void setStandAreaDescription(ref DataSet dsStandAreas, ref HospitalitySeatDetails seat)
        {
            foreach (DataRow dr in dsStandAreas.Tables["StandAreas"].Rows)
            {
                if (dr["StandCode"].ToString().Trim() == seat.StandCode.ToString().Trim() && (dr["AreaCode"].ToString().Trim() == seat.AreaCode.ToString().Trim()))
                {
                    seat.StandDescription = dr["StandDescription"].ToString();
                    seat.AreaDescription = dr["AreaDescription"].ToString();
                    break;
                }
            }
        }

        /// <summary>
        /// Set the addtional seat values against the hospitality seat that are from the component list
        /// Set the seated component display string for the booking page
        /// </summary>
        /// <param name="seat">The hospitality seat to set values for</param>
        /// <param name="componentsList">The component list to work with</param>
        /// <param name="viewModel">The booking view model</param>
        private void setSeatValuesFromComponent(ref HospitalitySeatDetails seat, ref List<ComponentDetails> componentsList, ref HospitalityBookingViewModel viewModel)
        {
            foreach (ComponentDetails component in componentsList)
            {
                if (component.ComponentID == seat.ComponentID)
                {
                    seat.ComponentDescription = component.ComponentDescription;
                    seat.SeatedComponentFormattedDisplay = viewModel.GetPageText("SeatedComponentDisplayFormat");
                    seat.SeatedComponentFormattedDisplay = seat.SeatedComponentFormattedDisplay.Replace("<<ComponentDescription>>", seat.ComponentDescription);
                    seat.SeatedComponentFormattedDisplay = seat.SeatedComponentFormattedDisplay.Replace("<<StandDescription>>", seat.StandDescription);
                    seat.SeatedComponentFormattedDisplay = seat.SeatedComponentFormattedDisplay.Replace("<<AreaDescription>>", seat.AreaDescription);
                    if(Environment.Settings.IsAgent == false && component.HideSeatForPWS)
                    {
                       seat.SeatedComponentFormattedDisplay = seat.SeatedComponentFormattedDisplay.Replace("<<FormattedSeatDetails>>", string.Empty);
                    }
                    else
                    {
                        seat.SeatedComponentFormattedDisplay = seat.SeatedComponentFormattedDisplay.Replace("<<FormattedSeatDetails>>", "(" + seat.FormattedSeatDetails + ")");
                    }
                    break;
                }
            }
        }

        #endregion
    }
}
