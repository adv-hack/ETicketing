using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMHospitality : DMBase
        {
            static private bool _EnableAsModule = true;
            public static bool EnableAsModule
            {
                get
                {
                    return _EnableAsModule;
                }
                set
                {
                    _EnableAsModule = value;
                }
            }
            static private string _ModuleTitle = "Hospitality Module";
            public static string ModuleTitle
            {
                get
                {
                    return _ModuleTitle;
                }
                set
                {
                    _ModuleTitle = value;
                }
            }

            public const string BudgetFilterVisible = "rOEEXOfK-5SxQDJLL-arWvef8-arXVe2";
            public const string CompetitionFilterVisible = "rOEEXOfK-BcaUnVov-arWve2q-arXVe9";
            public const string DateFilterVisible = "rOEEXOfK-r78EddgL-arWvefy-arXVea";
            public const string GroupSizeFilterVisible = "rOEEXOfK-WcjkaugG-arWve2O-arXVe0";
            public const string OppositionFilterVisible = "rOEEXOfK-ujkE6IgB-arWve23-arXVRQ";
            public const string SubtypeFilterVisible = "rOEEXOfK-5OE4Grgf-arWvefr-arXVR6";

            //Booking type options
            public const string ShowBookingType = "rOEEXOfK-JAZlDb1#-arWve2O-arLT00";
            public const string BookingTypeLabel = "rOxfXOxB-BKxh8Jxv-arWveah-arLnCQ";
            public const string PersonalLabel = "rOxfXOxB-QWC4QO44-arWve2q-arLnCw";
            public const string BusinessLabel = "rOxfXOxB-58C4nO0a-arWve2q-arLnCm";
            public const string BookingTypeRequiredErrorText = "rOxfXOxB-BKxtKDXB-arWve08-arLj16";


            //Booking reference 
            public const string BookingRefPanelVisible = "BjS6aVxZ-BK3eazYX-arWvead-arLvMf";

            public const string fixturesTabLabel = "rOxfXOxB-tOChdIxv-arWvefr-arXKZ9";
            public const string packagesTabLabel = "rOxfXOxB-rOCh8Ixv-arWvefr-arXKZa";
            public const string fixtureSeasonTabLabel = "rOxfXOxB-tObuGdbK-arWve23-arXKZ0";
            public const string packageSeasonTabLabel = "rOxfXOxB-rObuGObn-arWve23-arXKsQ";
            public const string subTypeText = "rOxfXOxB-5OE#GCoy-arWvefq-arXKsw";
            public const string competitionText = "rOxfXOxB-BcalGgpZ-arWve2O-arXKsm";
            public const string anyText = "rOxfXOxB-Cjq4Bvxk-arWvepu-arXKs6";
            public const string groupSliderLabel = "rOxfXOxB-W7ehHVxv-arWvefy-arXKsp";
            public const string budgetSliderLabel = "rOxfXOxB-5UxQ2g4G-arWvef8-arXKsf";
            public const string dateSliderLabel = "rOxfXOxB-rc2e9hoY-arWvefd-arXKs2";
            public const string resetOptionLabel = "rOxfXOxB-Q5ahGHxv-arWvefy-arXKs9";

            //package price format
            public const string packagePriceFormat = "rOxfXOxB-rO64QOKL-arWvRw8-arXK02";

            //package groupsize description format
            public const string packageGroupSizeFormat = "rOxfXOxB-rOzzSHo4-arWve0q-arXVRp";

            //HospitalityBooking Lead source options
            public const string ShowLeadSource = "rOEEXOfK-JWzQG8bd-arWvefa-arXKCf";
            public const string leadSourceLabel = "rOxfXOxB-QD2e9xo9-arWveaO-arXK0a";
            public const string leadSourceDisplayFormat = "rOxfXOxB-QDHEOgzK-arWvRaa-arXKC9";
            public const string LeadSourceRequired = "rOEEXOfK-QDi4G8EB-arWve2d-arLvEQ";

            // Availability related text 
            public const string Availability = "BjS4aVLZ-enEw6gsL-arWve2r-arXKCp";
            public const string AvailabilityMask = "BjS4aVLZ-enEw6rk4-arWvEmh-arXK00";
            public const string Cancelled = "BjS4aVLZ-r7vrQIxO-arWve2d-arXKCQ";
            public const string Reserved = "BjS4aVLZ-QOb4GI4o-arWve23-arXKCw";
            public const string Sold = "BjS4aVLZ-B7SZ2VbX-arWvefd-arXKCm";
            public const string SoldOutComponents = "BjS4aVLZ-BjDuU1zX-arWvea3-arXKC6";

            //hospitality booking
            public const string LeadSourceRequiredErrorText = "rOxfXOxB-QDi4Oxoh-arWve0y-arXVZf";
            public const string AlertifyCancelBookingCancelButtonText = "rOxfXOxB-FNadWOfO-arWveaO-arXVZ2";
            public const string AlertifyCancelBookingMessage = "rOxfXOxB-FNadWIHn-arWvR6q-arXVZ9";
            public const string AlertifyCancelBookingOKButtonText = "rOxfXOxB-FNadWTeB-arWve98-arXVZa";
            public const string AlertifyCancelBookingTitle = "rOxfXOxB-FNadWdoQ-arWvea8-arXVZ0";
            public const string ApplyComponentDiscountText = "rOxfXOxB-uAa4H71y-arWvea3-arXVsQ";
            public const string BookingNotFoundError = "rOxfXOxB-BKkuODzQ-arWve9a-arXVsw";
            public const string BookingTotalCostLabelText = "rOxfXOxB-BKvkGI#9-arWve93-arXVsm";
            public const string BookingVATLabelText = "rOxfXOxB-BK2eDbS0-arWve2d-arXVs6";
            public const string CancelButtonText = "rOxfXOxB-rpprgzSB-arWve2d-arXVsp";
            public const string ChangeComponentSeatText = "rOxfXOxB-JaprFgoK-arWve9y-arXVsf";
            public const string ContinueButtonText = "rOxfXOxB-BDE65dLy-arWve28-arXVs2";
            public const string CustomerDropDownDisplayFormat = "rOxfXOxB-5OpzXiLP-arWvMpy-arXVs9";
            public const string CustomerDDLFAndFOptionText = "rOxfXOxB-5O2U670L-arWvRQd-arLPsQ";
            public const string CustomerDDLSelectNewCustomerOptionText = "rOxfXOxB-5O2sjCAG-arWve0u-arLPsw";
            public const string CustomerHeaderText = "rOxfXOxB-5OvZHDxB-arWve28-arXVsa";
            public const string IncludedExtrasHeaderText = "rOxfXOxB-COEQOAfB-arWveaO-arXVs0";
            public const string PackageDescriptionDetailsText = "rOxfXOxB-rO6QGdLX-arWvRpq-arXV0Q";
            public const string PackageIncludedExtras = "rOxfXOxB-rOSXQODn-arWvRpu-arXV0w";
            public const string PleaseSelectLeadSource = "rOxfXOxB-FU66Ocb9-arWve9h-arXV0m";
            public const string PriceBandHeaderText = "rOxfXOxB-WWxetLwQ-arWve9O-arXV06";
            public const string PriceCodeHeaderText = "rOxfXOxB-WAxetLwY-arWve9O-arXV0p";
            public const string PriceHeaderText = "rOxfXOxB-WOelGOpE-arWve23-arXV0f";
            public const string QuantityHeaderText = "rOxfXOxB-5jvZHgxB-arWve28-arXV02";
            public const string RemoveComponentText = "rOxfXOxB-QaprcD1X-arWve2h-arXV09";
            public const string SeatedComponentDisplayFormat = "rOxfXOxB-QaprUsba-arWvEpy-arXV0a";
            public const string UnavailableExtraItemsText = "rOxfXOxB-C7xLSIIa-arWvRfO-arLV00";
            public const string ShowCreatePDFButton = "rOEEXOfK-JO319Eo4-arWve2y-arLn0a";

            public const string SubTypeVisible = "rOEEXOfK-5Oj0Xdbk-arWvefq-arXCZ6";
            public const string SubTypeOnUpcomingFixturesVisible = "BjS6aVxZ-5ODdBxgV-arWvea3-arXCZf";
            public const string SubTypeDescVisible = "BjS6aVxZ-5O6bHiKa-arWve9a-arXCZ2";
            public const string SubTypeDescLabel = "BjS4aVLZ-5O6hFIxv-arWve0q-arXCZ9";
            public const string HospitalityPDFAttachmentsOnConfirmationEmail = "jlefG3WV-xumfzSAU-arWve0r-arLn0p";

            //Show the status messages on basis of availability
            public const string msgNoResultsAvailable = "rOxfXOxB-9OE#UC0j-arWve0a-arXCsw";
            public const string msgNoHomeFixturesAvailable = "rOxfXOxB-9AjoQOY4-arWve0h-arXCsm";
            public const string msgNoHomePackagesAvailable = "rOxfXOxB-9AvdQOY4-arWve0h-arXCs6";
            public const string msgNoSeasonFixturesAvailable = "rOxfXOxB-9OaUzdUB-arWvRQO-arXCsp";
            public const string msgNoSeasonPackagesAvailable = "rOxfXOxB-9OaizdUB-arWvRQO-arXCsf";

            //Show/Hide chage seat options
            public const string HideChangeSeatsOptions = "rOEEXOfK-tWV45RxL-arWve2r-arUvef";

            //Hospitality booking enquiry
            public const string AgentColumnVisible = "rOEEXOfK-LAabhgeV-arWve9O-arLPsm";
            public const string AgentFilterVisible = "rOEEXOfK-Lcebhg4B-arWve9O-arLPs6";
            public const string CallIDColumnVisible = "rOEEXOfK-raFrvVfX-arWve93-arLPsp";
            public const string CallIDFilterVisible = "rOEEXOfK-rSxQvVLL-arWve93-arLPsf";
            public const string CustomerFilterVisible = "rOEEXOfK-5OS6U14K-arWve9d-arLPs2";
            public const string DateColumnVisible = "rOEEXOfK-r78EdDgL-arWve2a-arLPs9";
            public const string FromDateFilterVisible = "rOEEXOfK-WjS6UxoV-arWve9d-arLPsa";
            public const string MaxBookingsToDisplay = "rOEEXOfK-rVbl3AeX-arWve93-arLPs0";
            public const string PackageColumnVisible = "rOEEXOfK-rOz7GLzT-arWve9q-arLP0Q";
            public const string PackageFilterVisible = "rOEEXOfK-rOE4GLgT-arWve9q-arLP0w";
            public const string ProductColumnVisible = "rOEEXOfK-Wjz7GDzZ-arWve9q-arLP0m";
            public const string ProductFilterVisible = "rOEEXOfK-WjE4GDgZ-arWve9q-arLP06";
            public const string ProductGroupFilterVisible = "rOEEXOfK-Wjzz6rJa-arWve98-arLP0p";
            public const string StatusColumnVisible = "rOEEXOfK-caFr9gfX-arWve93-arLP0f";
            public const string StatusFilterVisible = "rOEEXOfK-cSxQ9gLL-arWve93-arLP02";
            public const string ToDateFilterVisible = "rOEEXOfK-BSxQFOLL-arWve93-arLP09";
            public const string ValueColumnVisible = "rOEEXOfK-rAabQIeV-arWve9O-arLP0a";
            public const string PrintStatusColumnVisible = "rOEEXOfK-WjbWnVLv-arWve9h-arLV0a";
            public const string QandAStatusFilterVisible = "rOEEXOfK-rjbUnVHv-arWve9h-arUveQ";
            public const string PrintStatusFilterVisible = "rOEEXOfK-WjbUnVLv-arWve9h-arUvew";
            public const string BookingTypeFilterVisible = "rOEEXOfK-BKxUnVgv-arWve9h-arUvem";
            public const string QandAStatusColumnVisible = "rOEEXOfK-rjbWnVHv-arWve9h-arUvep";

            public const string CallIDColumnHeading = "rOxfXOxB-raFrvVfX-arWveau-arLP00";
            public const string PackageColumnHeading = "rOxfXOxB-rOz7hLzT-arWve9y-arLPCQ";
            public const string ProductColumnHeading = "rOxfXOxB-Wjz7hDzZ-arWve9y-arLPCw";
            public const string AgentColumnHeading = "rOxfXOxB-LAaNhgeV-arWve93-arLPCm";
            public const string DateColumnHeading = "rOxfXOxB-r7y4dDoB-arWve2a-arLPC6";
            public const string ValueColumnHeading = "rOxfXOxB-rAaNQIeV-arWve93-arLPCp";
            public const string StatusColumnHeading = "rOxfXOxB-caFr9gfX-arWve9d-arLPCf";
            public const string CallIDLabelText = "rOxfXOxB-rZSluOpO-arWveaO-arLPC2";
            public const string FromDateLabelText = "rOxfXOxB-WjC45OoB-arWve9u-arLPC9";
            public const string ToDateLabelText = "rOxfXOxB-BZSldOpk-arWve9O-arLPCa";
            public const string StatusLabelText = "rOxfXOxB-cZSlHOp#-arWve2a-arLPC0";
            public const string ProductLabelText = "rOxfXOxB-Wjxk2gSB-arWve93-arLTeQ";
            public const string PackageLabelText = "rOxfXOxB-rOxk8ISB-arWve93-arLTew";
            public const string ProductGroupLabelText = "rOxfXOxB-WjzzBx5Y-arWveaq-arLTem";
            public const string AgentLabelText = "rOxfXOxB-LWq4JIHQ-arWve28-arLTe6";
            public const string CustomerLabelText = "rOxfXOxB-5OC4nkoB-arWveaO-arLTep";
            public const string ClearButtonText = "rOxfXOxB-FDalOgpO-arWve2r-arLTef";
            public const string SearchButtonText = "rOxfXOxB-QpprOzSB-arWve9O-arLTe2";
            public const string AllAgentsText = "rOxfXOxB-F0xoW7sQ-arWve93-arLTe9";
            public const string AllBookingStatusText = "rOxfXOxB-FVV6dVeX-arWve93-arLTea";
            public const string CallIdPlaceholder = "rOxfXOxB-rwxMUhlF-arWve9h-arLTe0";
            public const string FromDatePlaceholder = "rOxfXOxB-WjvdVksB-arWve9h-arLTRQ";
            public const string ToDatePlaceholder = "rOxfXOxB-BwxMfIlF-arWve9q-arLTRw";
            public const string PackagePlaceholder = "rOxfXOxB-rO64QOK4-arWve98-arLTRm";
            public const string ProductPlaceholder = "rOxfXOxB-Wj64O1K4-arWve98-arLTR6";
            public const string ProductGroupPlaceholder = "rOxfXOxB-WjzzUxKG-arWvear-arLTRp";
            public const string CustomerPlaceholder = "rOxfXOxB-5OvdvgsG-arWve9a-arLTRf";
            public const string ProductMask = "rOxfXOxB-WjsiQcLG-arWvRpy-arLTR2";
            public const string PackageMask = "rOxfXOxB-rOsiQco4-arWvRpy-arLTR9";
            public const string AgentMask = "rOxfXOxB-LWZ4hgY1-arWvR6d-arLTRa";

            public const string LoginConfirmationBoxText = "rOxfXOxB-BAe7BAeB-arWvR0h-arLTR0";
            public const string LoginConfirmationCancelButtonText = "rOxfXOxB-BAe7WVe4-arWvea8-arLTMQ";
            public const string LoginConfirmationOkButtonText = "rOxfXOxB-BAe7DgLQ-arWvead-arLTMw";
            public const string LoginConfirmationBoxTitle = "rOxfXOxB-BAe7Bggk-arWve0u-arLTMm";
            public const string ShowGeneratePDFIcon = "rOEEXOfK-J0E49ExB-arWve9q-arLnC6";

            //Hospitality Upgradation from ProductList and SeatSelection page
            public const string UpgradeToHospitalityLinkTextForProductList = "BjS4aVLZ-uOp#3v9G-arWvRQd-arLjCw";
            public const string UpgradeToHospitalityLinkTextForSeatSelection = "BjS4aVLZ-uOp#3v9f-arWvRQy-arLjCm";
            public const string upgradeToHospitalityText = "BjS4aVLZ-uOp#3AxB-arWve0h-arLV06";
            public const string upgradeToHospitalityDescText = "BjS4aVLZ-uOp#3CfL-arWve28-arLV0p";
            public const string ShowUpgradeToHospitalityOnProductList = "BjS6aVxZ-JKxlQgz4-arWveaO-arLjC6";
            public const string ShowUpgradeToHospitalityOnSeatSelectionPage = "BjS6aVxZ-JKxlQgxX-arWveah-arLjCp";
            public const string ShowUpgradeToHospitalityWhenProductSoldOut = "BjS6aVxZ-JKxlQgjd-arWveay-arLjCf";
            public const string ShowUpgradeToHospitalityOnSVGWhenProductSoldOut = "BjS6aVxZ-JKxlQg5y-arWve03-arLKC6";

            //Hospitality PreBooking form
            public const string HospitalityPreBookingButtonText = "rOxfXOxB-BWRiW1oH-arWve0O-arLB1m";
            public const string HospitalityPreBookingActivitySubject = "rOxfXOxB-BWRiWLtK-arWvEQ3-arLHe0";
            public const string HospitalityDataCaptureCreationSuccessMessage = "BjS4aVLZ-BWR1HezX-arWvRmu-arLHRw";
            public const string HospitalityPreBookingEnquiryPrompt = "BjS4aVLZ-BWRiWvjE-arWvRf8-arLV09";

            public DMHospitality(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {
                MConfigs.Add(BudgetFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(CompetitionFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(DateFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(GroupSizeFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(OppositionFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(SubtypeFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderAttributes);

                //Booking type option
                MConfigs.Add(ShowBookingType, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(BookingTypeLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(PersonalLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(BusinessLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(BookingTypeRequiredErrorText, DataType.TEXT, DisplayTabs.TabHeaderTexts);

                //Booking reference
                MConfigs.Add(BookingRefPanelVisible, DataType.BOOL, DisplayTabs.TabHeaderAttributes);

                MConfigs.Add(fixturesTabLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(packagesTabLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(fixtureSeasonTabLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(packageSeasonTabLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(subTypeText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(competitionText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(anyText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(groupSliderLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(budgetSliderLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(dateSliderLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(resetOptionLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);


                // Availability related text 
                MConfigs.Add(Availability, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(AvailabilityMask, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(Cancelled, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(Reserved, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(Sold, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(SoldOutComponents, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                

                //package price format
                MConfigs.Add(packagePriceFormat, DataType.TEXT, DisplayTabs.TabHeaderHospitalityDetails);

                //package groupsize description format
                MConfigs.Add(packageGroupSizeFormat, DataType.TEXT, DisplayTabs.TabHeaderHospitalityDetails);

                //HospitalityBooking Lead source options
                MConfigs.Add(ShowLeadSource, DataType.BOOL, DisplayTabs.TabHeaderAttributes);

                MConfigs.Add(leadSourceLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(leadSourceDisplayFormat, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(LeadSourceRequired, DataType.BOOL, DisplayTabs.TabHeaderAttributes);

                //booking
                MConfigs.Add(LeadSourceRequiredErrorText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(AlertifyCancelBookingCancelButtonText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(AlertifyCancelBookingMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(AlertifyCancelBookingOKButtonText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(AlertifyCancelBookingTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ApplyComponentDiscountText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(BookingNotFoundError, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(BookingTotalCostLabelText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(BookingVATLabelText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(CancelButtonText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ChangeComponentSeatText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ContinueButtonText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(CustomerDropDownDisplayFormat, DataType.TEXT, DisplayTabs.TabHeaderHospitalityDetails);
                MConfigs.Add(CustomerDDLFAndFOptionText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityDetails);
                MConfigs.Add(CustomerDDLSelectNewCustomerOptionText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityDetails);
                MConfigs.Add(CustomerHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(IncludedExtrasHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(PackageDescriptionDetailsText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(PackageIncludedExtras, DataType.TEXT, DisplayTabs.TabHeaderHospitalityDetails);
                MConfigs.Add(PleaseSelectLeadSource, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(PriceBandHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(PriceCodeHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(PriceHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(QuantityHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(RemoveComponentText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(SeatedComponentDisplayFormat, DataType.TEXT, DisplayTabs.TabHeaderHospitalityDetails);
                MConfigs.Add(ShowCreatePDFButton, DataType.BOOL, DisplayTabs.TabHeaderAttributes);

                MConfigs.Add(SubTypeVisible, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(SubTypeOnUpcomingFixturesVisible, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(SubTypeDescVisible, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(SubTypeDescLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(UnavailableExtraItemsText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(HospitalityPDFAttachmentsOnConfirmationEmail, DataType.BOOL, DisplayTabs.TabHeaderAttributes);

                //Show the status messages on basis of availability
                MConfigs.Add(msgNoResultsAvailable, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(msgNoHomeFixturesAvailable, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(msgNoHomePackagesAvailable, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(msgNoSeasonFixturesAvailable, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(msgNoSeasonPackagesAvailable, DataType.TEXT, DisplayTabs.TabHeaderTexts);

                //Show/Hide chage seat options
                MConfigs.Add(HideChangeSeatsOptions, DataType.BOOL, DisplayTabs.TabHeaderAttributes);

                //Hospitality booking enquiry
                MConfigs.Add(AgentColumnVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(AgentFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(CallIDColumnVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(CallIDFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(CustomerFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(DateColumnVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(FromDateFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(MaxBookingsToDisplay, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(PackageColumnVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(PackageFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ProductColumnVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ProductFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ProductGroupFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(StatusColumnVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(StatusFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ToDateFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ValueColumnVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(PrintStatusColumnVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(QandAStatusFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(PrintStatusFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(BookingTypeFilterVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(QandAStatusColumnVisible, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);

                MConfigs.Add(CallIDColumnHeading, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(PackageColumnHeading, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ProductColumnHeading, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(AgentColumnHeading, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(DateColumnHeading, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ValueColumnHeading, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(StatusColumnHeading, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(CallIDLabelText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(FromDateLabelText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ToDateLabelText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(StatusLabelText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ProductLabelText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(PackageLabelText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ProductGroupLabelText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(AgentLabelText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(CustomerLabelText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ClearButtonText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(SearchButtonText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(AllAgentsText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(AllBookingStatusText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(CallIdPlaceholder, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(FromDatePlaceholder, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ToDatePlaceholder, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(PackagePlaceholder, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ProductPlaceholder, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ProductGroupPlaceholder, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(CustomerPlaceholder, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ProductMask, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(PackageMask, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(AgentMask, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(LoginConfirmationBoxText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(LoginConfirmationCancelButtonText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(LoginConfirmationOkButtonText, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(LoginConfirmationBoxTitle, DataType.TEXT, DisplayTabs.TabHeaderHospitalityBookingEnquiry);
                MConfigs.Add(ShowGeneratePDFIcon, DataType.BOOL, DisplayTabs.TabHeaderHospitalityBookingEnquiry);

                //Hospitality Upgradation from ProductList and SeatSelection page
                MConfigs.Add(UpgradeToHospitalityLinkTextForProductList, DataType.TEXT, DisplayTabs.TabHeaderRelatingHospitalityProductUpsell);
                MConfigs.Add(UpgradeToHospitalityLinkTextForSeatSelection, DataType.TEXT, DisplayTabs.TabHeaderRelatingHospitalityProductUpsell);
                MConfigs.Add(upgradeToHospitalityText, DataType.TEXT, DisplayTabs.TabHeaderRelatingHospitalityProductUpsell);
                MConfigs.Add(upgradeToHospitalityDescText, DataType.TEXTAREA, DisplayTabs.TabHeaderRelatingHospitalityProductUpsell);
                MConfigs.Add(ShowUpgradeToHospitalityOnProductList, DataType.BOOL, DisplayTabs.TabHeaderRelatingHospitalityProductUpsell);
                MConfigs.Add(ShowUpgradeToHospitalityOnSeatSelectionPage, DataType.BOOL, DisplayTabs.TabHeaderRelatingHospitalityProductUpsell);
                MConfigs.Add(ShowUpgradeToHospitalityWhenProductSoldOut, DataType.BOOL, DisplayTabs.TabHeaderRelatingHospitalityProductUpsell);
                MConfigs.Add(ShowUpgradeToHospitalityOnSVGWhenProductSoldOut, DataType.BOOL, DisplayTabs.TabHeaderRelatingHospitalityProductUpsell);

                //Hospitality PreBooking form
                MConfigs.Add(HospitalityPreBookingButtonText, DataType.TEXT, DisplayTabs.TabHeaderRelatingHospitalityPreBookingDataCapture);
                MConfigs.Add(HospitalityPreBookingActivitySubject, DataType.TEXT, DisplayTabs.TabHeaderRelatingHospitalityPreBookingDataCapture);
                MConfigs.Add(HospitalityDataCaptureCreationSuccessMessage, DataType.TEXT, DisplayTabs.TabHeaderRelatingHospitalityPreBookingDataCapture);
                MConfigs.Add(HospitalityPreBookingEnquiryPrompt, DataType.TEXT, DisplayTabs.TabHeaderRelatingHospitalityPreBookingDataCapture);

                Populate();
            }
        }
    }
}