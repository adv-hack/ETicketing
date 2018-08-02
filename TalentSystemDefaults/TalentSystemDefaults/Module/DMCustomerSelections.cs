using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMCustomerSelections : DMBase
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
            static private string _ModuleTitle = "Customer/Company Search";
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

            public const string CustomerSearch_AddressLine1 = "BjS6aVxZ-g8xPJIoQ-arWvefh-arXCs2";
            public const string CustomerSearch_AddressLine2 = "BjS6aVxZ-g8xTJIoQ-arWvef8-arXCs9";
            public const string CustomerSearch_AddressLine3 = "BjS6aVxZ-g8xjJIoQ-arWvef8-arXCsa";
            public const string CustomerSearch_AddressLine4 = "BjS6aVxZ-g8xBJIoQ-arWvef8-arXCs0";
            public const string CustomerSearch_EmailMaxLength = "BjS6aVxZ-qWa27Y2L-arWvefh-arXC0Q";
            public const string CustomerSearch_PhoneNumberMaxLength = "BjS6aVxZ-JDeFmDWj-arWve2q-arXC0w";
            public const string CustomerSearch_ShowPassportNumberCriteria = "BjS6aVxZ-J8e6OIzG-arWve9O-arXC0m";
            public const string CustomerSearch_PassportNumberText = "BjS4aVLZ-rvF0QHUy-arWve9d-arXC06";
            public const string CustomerSearch_AddButtonText = "BjS4aVLZ-gjxod7JK-arWvefh-arXC0p";
            public const string CustomerSearch_AddressLine2Text = "BjS4aVLZ-g8xTOCSB-arWve9O-arXC0f";
            public const string CustomerSearch_AddressLine3Text = "BjS4aVLZ-g8xjOCSB-arWve9O-arXC02";
            public const string CustomerSearch_AddressLine4Text = "BjS4aVLZ-g8xBOCSB-arWve9O-arXC09";
            public const string CustomerSearch_AddressPostCodeText = "BjS4aVLZ-g8EWzx0E-arWve2r-arXC0a";
            public const string CustomerSearch_BackButtonText = "BjS4aVLZ-rjq4HDXn-arWvefr-arXC00";
            public const string CustomerSearch_ContactNumberText = "BjS4aVLZ-BjC4WLoB-arWve9O-arXCCQ";
            public const string CustomerSearch_AddressLine1Text = "BjS4aVLZ-g8xPOCSB-arWve9O-arXCCw";
            public const string CustomerSearch_FormHeaderText = "BjS4aVLZ-5OvQGIQV-arWvead-arXCCm";
            public const string CustomerSearch_EmailText = "BjS4aVLZ-qOFeSVUh-arWvefu-arXCC6";
            public const string CustomerSearch_ForenameText = "BjS4aVLZ-BlI6GAeB-arWve2O-arXCCp";
            public const string CustomerSearch_PerformSearchButtonText = "BjS4aVLZ-QlEug1oG-arWve98-arXCCf";
            public const string CustomerSearch_PhoneNumberText = "BjS4aVLZ-JDelGkpE-arWve28-arXCC2";
            public const string CustomerSearch_SurnameText = "BjS4aVLZ-5OEsG8oy-arWvefr-arXCC9";
            public const string CompanySearch_AddressLine1Text = "BjS4aVLZ-g8xPOCSB-arWve2a-arXCCa";
            public const string CompanySearch_btnAddButtonText = "BjS4aVLZ-cpprzzSB-arWvefr-arXCC0";
            public const string CompanySearch_NameText = "BjS4aVLZ-BPxlQtpO-arWve2h-arXneQ";
            public const string CompanySearch_FormHeaderText = "BjS4aVLZ-BPedQxzB-arWveaO-arXnew";
            public const string CompanySearch_hplBackText = "BjS4aVLZ-uVEMGq#f-arWvefu-arXnem";
            public const string CompanySearch_PerformSearchButtonText = "BjS4aVLZ-QlDemgpB-arWve9y-arXne6";
            public const string CompanySearch_PostCodeText = "BjS4aVLZ-BLI6GAQB-arWvefa-arXnep";
            public const string CompanySearch_TelephoneNumberText = "BjS4aVLZ-QAz7FIcQ-arWve9u-arXnef";
            public const string CompanySearch_WebAddressText = "BjS4aVLZ-Qvq42CG1-arWve2u-arXne2";
            public const string CompanySearch_ParentSearchFormHeaderText = "BjS4aVLZ-rU6M274Y-arWveah-arXne9";
            public const string CompanySearch_SubsideriesSearchFormHeaderText = "BjS4aVLZ-5ObsOIoG-arWveaa-arXnea";
            public const string CompanySearchChangePageSize = "rOEEXOfK-BPedtIzB-arWve93-arXne0";
            public const string CompanySearchChangePageSizeSelection = "rOEEXOfK-BPedtIo4-arWve03-arXnRQ";
            public const string CompanySearchNonSortableColumnArray = "rOEEXOfK-BPeddVUG-arWveay-arXnRw";
            public const string CompanySearchPageSize = "rOEEXOfK-BPedyDbf-arWve2d-arXnRm";
            public const string CustomerSearchChangePageSize = "rOEEXOfK-5OvQGJQQ-arWve9q-arXnR6";
            public const string CustomerSearchChangePageSizeSelection = "rOEEXOfK-5OvQGJb#-arWve0q-arXnRp";
            public const string CustomerSearchNonSortableColumnArray = "rOEEXOfK-5OvQOrfB-arWve9r-arXnRf";
            public const string CustomerSearchPageSize = "rOEEXOfK-5OvQ6eoy-arWve2u-arXnR2";
            public const string CustomerSearchResultsColumnVisibilityAddress = "rOEEXOfK-5OvQdDgy-arWvear-arLvMm";
            public const string CustomerSearchResultsColumnVisibilityDOB = "rOEEXOfK-5OvQdDgV-arWveau-arLvRf";
            public const string CustomerSearchResultsColumnVisibilityEmail = "rOEEXOfK-5OvQdDgK-arWveah-arLvR2";
            public const string CustomerSearchResultsColumnVisibilityMembership = "rOEEXOfK-5OvQdDgL-arWve03-arLvR9";
            public const string CustomerSearchResultsColumnVisibilityName = "rOEEXOfK-5OvQdDgX-arWveay-arLvRa";
            public const string CustomerSearchResultsColumnVisibilityPassport = "rOEEXOfK-5OvQdDgO-arWve0O-arLvR0";
            public const string CustomerSearchResultsColumnVisibilityPhoneNumber = "rOEEXOfK-5OvQdDgv-arWve0q-arLvMQ";
            public const string CustomerSearchResultsColumnVisibilityPostCode = "rOEEXOfK-5OvQdDgO-arWveaa-arLvMw";
            public const string SearchResultLimit = "rOEEXOfK-Q3S6QqLK-arWvefa-arXnMf";
            public const string ShowCorporateSaleIDSearch = "rOEEXOfK-Jvv69x6B-arWve2a-arXnM2";
            public const string lblRestrictionText = "rOxfXOxB-#jEEaILZ-arWvRQO-arXnM9";
            public const string FanCardBoxAutoFocusScript = "rOxfXOxB-rL5X9dQZ-arWvR9d-arXnMa";
            public const string MembershipNumberText = "rOxfXOxB-Q8wXdkgG-arWve98-arXnM0";
            public const string ErrorText_MembershipDoesNotExist = "rOxfXOxB-WOl4XIpv-arWve0q-arXnEQ";
            public const string ForenameText = "rOxfXOxB-BlI6GAeB-arWve2O-arXnEw";
            public const string UpdateHeaderText = "rOxfXOxB-ufxQQRSB-arWve2q-arXnEm";
            public const string ContactLabelHeaderText = "rOxfXOxB-BjxkGeLX-arWve2a-arXnE6";
            public const string SelectHeaderText = "rOxfXOxB-QfxQGRSB-arWve2q-arXnEp";
            public const string CompanyNameHeaderText = "rOxfXOxB-BPxNBDcf-arWve9d-arXnEf";
            public const string TelephoneHeaderText = "rOxfXOxB-QAxeFIwQ-arWve9y-arXnE2";
            public const string CorporateSaleIDText = "rOxfXOxB-BWvkvHbK-arWve9h-arXnE9";
            public const string PaymentReferenceText = "rOxfXOxB-rjxQdioQ-arWve98-arXnEa";
            public const string ErrorText_PayrefDoesNotExist = "rOxfXOxB-WO3eng2X-arWve0y-arXnE0";
            public const string ErrorText_CorporateSaleDoesNotExist = "rOxfXOxB-WOru9IzO-arWvRQa-arXnAQ";
            public const string CompanySearchLengthMenuText = "rOxfXOxB-BPed71zF-arWve0r-arXnAw";
            public const string CustomerSearchLengthMenuText = "rOxfXOxB-5OvQmvQT-arWve0a-arXnAm";
            public const string CustomerSelectionSearchHeaderText = "rOxfXOxB-5OS4Qq4Z-arWvear-arXnA6";
            public const string CustomerSearchTabTitleText = "rOxfXOxB-5OvQd70K-arWvea3-arXnAp";
            public const string CompanySearchTabTitleText = "rOxfXOxB-BPedUI1B-arWve9a-arXnAf";
            public const string ParentSearchHeaderText = "rOxfXOxB-rU6MGcb4-arWvead-arXnA2";
            public const string SubsidiariesSearchHeaderText = "rOxfXOxB-5cx#GIbF-arWvea8-arXnA9";
            public const string SelectContactHeaderText = "rOxfXOxB-QavdFgLK-arWveah-arXnAa";
            public const string PassportHeaderText = "rOxfXOxB-rvvZQHxB-arWve2a-arXnA0";
            public const string PassportNumberText = "rOxfXOxB-rvF0QHUy-arWve2r-arXn1Q";
            public const string AddButtonText = "rOxfXOxB-gjxod7JK-arWvefh-arXn1w";
            public const string AddressHeaderText = "rOxfXOxB-g8K42CoB-arWve2u-arXn1m";
            public const string AddressLine1Text = "rOxfXOxB-g8xPOCSB-arWve9O-arXn16";
            public const string AddressLine2Text = "rOxfXOxB-g8xTOCSB-arWve9O-arXn1p";
            public const string AddressLine3Text = "rOxfXOxB-g8xjOCSB-arWve9O-arXn1f";
            public const string AddressLine4Text = "rOxfXOxB-g8xBOCSB-arWve9O-arXn12";
            public const string AddressPostCodeText = "rOxfXOxB-g8EWzx0E-arWve2r-arXn19";
            public const string BackButtonText = "rOxfXOxB-rjq4HDXn-arWvefr-arXn1a";
            public const string ClubSelectionLabelText = "rOxfXOxB-F7juGes9-arWve9d-arXn10";
            public const string DateOfBirthHeaderText = "rOxfXOxB-rpTNBOgB-arWve9u-arXnZQ";
            public const string CustomerNameHeaderText = "rOxfXOxB-5OF4Geoy-arWve2h-arXnZw";
            public const string CustomerNumberHeaderText = "rOxfXOxB-5OF0OAzB-arWve9q-arXnZm";
            public const string CustomerSearchFormHeaderText = "rOxfXOxB-5OvQGIQV-arWvead-arXnZ6";
            public const string EMailAddressHeaderText = "rOxfXOxB-mLb#GZJU-arWve2r-arXnZp";
            public const string EmailText = "rOxfXOxB-qOFeSVUh-arWvefu-arXnZf";
            public const string ErrorText_CustomerDoesNotExist = "rOxfXOxB-WOrX5t0V-arWvRpd-arXnZ2";
            public const string ErrorText_CustomerIsNotWebReady = "rOxfXOxB-WOrXngxX-arWvRwu-arXnZ9";
            public const string ErrorText_CustomerOnWatchList = "rOxfXOxB-WOrXWgLB-arWvRfu-arXnZa";
            public const string ErrorText_FancardDoesNotExist = "rOxfXOxB-WOkeGDLY-arWvea8-arXnZ0";
            public const string ErrorText_MoreThan1Entry = "rOxfXOxB-WOluKx4K-arWvRQr-arXnsQ";
            public const string ErrorText_NoClubSelection = "rOxfXOxB-WOwuGdzZ-arWve0u-arXnsw";
            public const string ErrorText_NoCustomerDataEntered = "rOxfXOxB-WOwuOg4a-arWvRQ3-arXnsm";
            public const string ErrorText_NoForwardUrl = "rOxfXOxB-WOwubZoG-arWveay-arXns6";
            public const string ErrorText_NoResultsFound = "rOxfXOxB-WOwuVv4y-arWvRwd-arXnsp";
            public const string ErrorText_PleaseEnterASearchParameter = "rOxfXOxB-WO3kG8jK-arWvRp8-arXnsf";
            public const string FancardLabelText = "rOxfXOxB-rLxkghSB-arWve28-arXns2";
            public const string GenericSalesButtonText = "rOxfXOxB-QEx#GuKB-arWve9a-arXns9";
            public const string GreenCardLabelText = "rOxfXOxB-WWv0OvxM-arWve93-arXnsa";
            public const string IDHeaderText = "rOxfXOxB-zOI6OAxB-arWvefu-arXns0";
            public const string lblLastNCustomersUsed = "rOxfXOxB-#jb6GrcM-arWveau-arXn0Q";
            public const string lblUserID5Text = "rOxfXOxB-#vq4GysC-arWve2d-arXn0w";
            public const string lblUserID6Text = "rOxfXOxB-#vq4GysC-arWve2d-arXn0m";
            public const string lblUserID7Text = "rOxfXOxB-#vq4GysC-arWve2d-arXn06";
            public const string lblUserID8Text = "rOxfXOxB-#vq4GysC-arWve2d-arXn0p";
            public const string lblUserID9Text = "rOxfXOxB-#vq4GysC-arWve2d-arXn0f";
            public const string MembershipHeaderText = "rOxfXOxB-Q8y4dkgG-arWve9h-arXn02";
            public const string MembershipNumberLabelText = "rOxfXOxB-Q8wXGIW9-arWve2a-arXn09";
            public const string msgPrintAddressLabelSuccess = "rOxfXOxB-90KQUL0B-arWvRQ3-arXn0a";
            public const string OpenCustomerSearchButtonText = "rOxfXOxB-u8xQHDAF-arWve9u-arXn00";
            public const string PassportLabelText = "rOxfXOxB-rvC4nDoB-arWve2r-arXnCQ";
            public const string PerformCustomerSearchButtonText = "rOxfXOxB-QlEug1oG-arWve98-arXnCw";
            public const string PINLabelText = "rOxfXOxB-oOI6UAxB-arWvefy-arXnCm";
            public const string PostCodeHeaderText = "rOxfXOxB-BLvZ5exB-arWve28-arXnC6";
            public const string PrintAddressLabelHeaderText = "rOxfXOxB-WLb#Qx4B-arWve9r-arXnCp";
            public const string PrintAddressLabelItemText = "rOxfXOxB-WLb#GIeV-arWve9h-arXnCf";
            public const string ProgressLabelText = "rOxfXOxB-W8C45IoB-arWvRwd-arXnC2";
            public const string RequiresLoginMessage = "rOxfXOxB-QOZEGUTG-arWvRwq-arXnC9";
            public const string SelectLinkItemText = "rOxfXOxB-QZA6GLDn-arWve2u-arXnCa";
            public const string SelectListItemText = "rOxfXOxB-QZA6GLDK-arWve93-arXnC0";
            public const string SubmitButtonText = "rOxfXOxB-5pprSzSB-arWve2q-arLveQ";
            public const string SurnameText = "rOxfXOxB-5OEsG8oy-arWvefr-arLvew";
            public const string UpdateLinkHeaderText = "rOxfXOxB-uZy4dheB-arWve2h-arLvem";
            public const string UpdateLinkItemText = "rOxfXOxB-uZA6XgDn-arWve2u-arLve6";

            public DMCustomerSelections(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {

                MConfigs.Add(CompanySearchChangePageSize, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CompanySearchChangePageSizeSelection, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CompanySearchNonSortableColumnArray, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CompanySearchPageSize, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchChangePageSize, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchChangePageSizeSelection, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchNonSortableColumnArray, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchPageSize, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchResultsColumnVisibilityAddress, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchResultsColumnVisibilityDOB, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchResultsColumnVisibilityEmail, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchResultsColumnVisibilityMembership, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchResultsColumnVisibilityName, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchResultsColumnVisibilityPassport, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchResultsColumnVisibilityPhoneNumber, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchResultsColumnVisibilityPostCode, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SearchResultLimit, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ShowCorporateSaleIDSearch, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(lblRestrictionText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(FanCardBoxAutoFocusScript, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(MembershipNumberText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ErrorText_MembershipDoesNotExist, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ForenameText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(UpdateHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ContactLabelHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SelectHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CompanyNameHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(TelephoneHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CorporateSaleIDText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(PaymentReferenceText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ErrorText_PayrefDoesNotExist, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ErrorText_CorporateSaleDoesNotExist, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CompanySearchLengthMenuText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchLengthMenuText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSelectionSearchHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchTabTitleText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CompanySearchTabTitleText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ParentSearchHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SubsidiariesSearchHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SelectContactHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(PassportHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(PassportNumberText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(AddButtonText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(AddressHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(AddressLine1Text, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(AddressLine2Text, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(AddressLine3Text, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(AddressLine4Text, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(AddressPostCodeText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(BackButtonText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ClubSelectionLabelText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(DateOfBirthHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerNameHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerNumberHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CustomerSearchFormHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(EMailAddressHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(EmailText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(FancardLabelText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(GenericSalesButtonText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(GreenCardLabelText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(IDHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(lblLastNCustomersUsed, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(lblUserID5Text, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(lblUserID6Text, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(lblUserID7Text, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(lblUserID8Text, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(lblUserID9Text, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(MembershipHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(MembershipNumberLabelText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(msgPrintAddressLabelSuccess, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(OpenCustomerSearchButtonText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(PassportLabelText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(PerformCustomerSearchButtonText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(PINLabelText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(PostCodeHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(PrintAddressLabelHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(PrintAddressLabelItemText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ProgressLabelText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(RequiresLoginMessage, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SelectLinkItemText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SelectListItemText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SubmitButtonText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SurnameText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(UpdateLinkHeaderText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(UpdateLinkItemText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ErrorText_CustomerDoesNotExist, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ErrorText_CustomerIsNotWebReady, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ErrorText_CustomerOnWatchList, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ErrorText_FancardDoesNotExist, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ErrorText_MoreThan1Entry, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ErrorText_NoClubSelection, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ErrorText_NoCustomerDataEntered, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ErrorText_NoForwardUrl, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ErrorText_NoResultsFound, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ErrorText_PleaseEnterASearchParameter, DataType.TEXT, DisplayTabs.TabHeaderGeneral);

                MConfigs.Add(CustomerSearch_AddressLine1, DataType.BOOL, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_AddressLine2, DataType.BOOL, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_AddressLine3, DataType.BOOL, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_AddressLine4, DataType.BOOL, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_ShowPassportNumberCriteria, DataType.BOOL, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_ForenameText, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_SurnameText, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_AddressLine1Text, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_AddressLine2Text, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_AddressLine3Text, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_AddressLine4Text, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_AddressPostCodeText, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_EmailText, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_EmailMaxLength, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_PhoneNumberText, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_PhoneNumberMaxLength, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_ContactNumberText, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_PassportNumberText, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_FormHeaderText, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_AddButtonText, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_BackButtonText, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");
                MConfigs.Add(CustomerSearch_PerformSearchButtonText, DataType.TEXT, DisplayTabs.TabHeaderCustomerSearch, uniqueId: "customerSearch");

                MConfigs.Add(CompanySearch_NameText, DataType.TEXT, DisplayTabs.TabHeaderCompanySearch, uniqueId: "companySearch");
                MConfigs.Add(CompanySearch_AddressLine1Text, DataType.TEXT, DisplayTabs.TabHeaderCompanySearch, uniqueId: "companySearch");
                MConfigs.Add(CompanySearch_PostCodeText, DataType.TEXT, DisplayTabs.TabHeaderCompanySearch, uniqueId: "companySearch");
                MConfigs.Add(CompanySearch_TelephoneNumberText, DataType.TEXT, DisplayTabs.TabHeaderCompanySearch, uniqueId: "companySearch");
                MConfigs.Add(CompanySearch_WebAddressText, DataType.TEXT, DisplayTabs.TabHeaderCompanySearch, uniqueId: "companySearch");
                MConfigs.Add(CompanySearch_FormHeaderText, DataType.TEXT, DisplayTabs.TabHeaderCompanySearch, uniqueId: "companySearch");
                MConfigs.Add(CompanySearch_ParentSearchFormHeaderText, DataType.TEXT, DisplayTabs.TabHeaderCompanySearch, uniqueId: "companySearch");
                MConfigs.Add(CompanySearch_SubsideriesSearchFormHeaderText, DataType.TEXT, DisplayTabs.TabHeaderCompanySearch, uniqueId: "companySearch");
                MConfigs.Add(CompanySearch_btnAddButtonText, DataType.TEXT, DisplayTabs.TabHeaderCompanySearch, uniqueId: "companySearch");
                MConfigs.Add(CompanySearch_hplBackText, DataType.TEXT, DisplayTabs.TabHeaderCompanySearch, uniqueId: "companySearch");
                MConfigs.Add(CompanySearch_PerformSearchButtonText, DataType.TEXT, DisplayTabs.TabHeaderCompanySearch, uniqueId: "companySearch");

                Populate();
            }
        }
    }
}