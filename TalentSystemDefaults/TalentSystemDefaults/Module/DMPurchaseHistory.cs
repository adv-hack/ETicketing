using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMPurchaseHistory : DMBase
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
            static private string _ModuleTitle = "Purchase History";
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

            // Purchase History Filter Options

            public const string numberOfProductsVariable = "rOEEXOfK-54pZ6Vov-arWve2u-arLPR6";
            public const string PaymentRefRegex = "rOEEXOfK-rji4GjoE-arWvefr-arLPRp";
            public const string RetailFromTicketingDB = "rOEEXOfK-QSqEfI44-arWve2d-arLPRf";
            public const string ShowCorporateProductsCheckBox = "rOEEXOfK-Jvv6dqpY-arWve93-arLPR2";
            public const string ShowPackageProductsOption = "rOEEXOfK-JExiod6f-arWve28-arLPR9";
            public const string ShowProductOrDescriptionSearchBox = "rOEEXOfK-JAEIXDKG-arWve9y-arLPRa";
            public const string ShowRetailPurchaseHistoryOptions = "rOEEXOfK-Jj3X6DgZ-arWve9u-arLPR0";
            public const string ShowTicketingPurchaseHistoryOptions = "rOEEXOfK-JEjrndAL-arWve98-arLPMQ";

            public const string PaymentRefRegexErrorText = "rOxfXOxB-rji4OAoB-arWve0h-arLvsm";
            public const string RetailOrderItemText = "rOxfXOxB-Q4epcOoG-arWve2a-arLvs6";
            public const string SeatsLabel = "rOxfXOxB-QWV4UOoM-arWvefd-arLvsp";
            public const string NoPurchaseFound = "rOxfXOxB-BYkuOOej-arWve93-arLvsf";
            public const string StatusOptions = "rOxfXOxB-c4prodxp-arWvRQa-arLvs2";
            public const string ShowRetailProductsLabel = "rOxfXOxB-Jj3QQVo9-arWvea3-arLvsa";
            public const string RetailOrderNumberLabel = "rOxfXOxB-Q4e3ajAB-arWve9a-arLvs0";
            public const string TicketingPaymentRefLabel = "rOxfXOxB-tcvwJIov-arWve9a-arLv0Q";
            public const string ShowTicketingProductsLabel = "rOxfXOxB-JEjrdOzK-arWvea8-arLv0w";
            public const string WebOrderNumberLabel = "rOxfXOxB-QOF0xBfG-arWve9r-arLv0m";
            public const string BackOfficerOrderNumberLabel = "rOxfXOxB-rNeIaYxV-arWve0O-arLv06";
            public const string NoPaymentReference = "rOxfXOxB-BOxR5io6-arWvefa-arLv0p";
            public const string TicketingPackageIDLabel = "rOxfXOxB-tcvdQVL9-arWvea3-arLv0f";
            public const string quantityLabel = "rOxfXOxB-5jC4dOxK-arWvefa-arLv02";
            public const string RetailCustomerPurchaseRefColumnHeading = "rOxfXOxB-Qap7Qjfy-arWvear-arLv09";
            public const string ProductOrDescriptionSearchLabel = "rOxfXOxB-Wjx#WOWL-arWve03-arLv0a";
            public const string ClearButtonText = "rOxfXOxB-FDalOgpO-arWvefr-arLv00";
            public const string customerNumberLabel = "rOxfXOxB-5OF0ggfG-arWve9q-arLvCQ";
            public const string dateLabel = "rOxfXOxB-rnv6QYxY-arWvef3-arLvCw";
            public const string filterButton = "rOxfXOxB-tpprHDoK-arWvefh-arLvCm";
            public const string FromDateLabel = "rOxfXOxB-WjC4dOzK-arWve2O-arLvC6";
            public const string FromToDateCompareErrorText = "rOxfXOxB-WXruO7zG-arWvRQq-arLvCp";
            public const string FutureDatesErrorText = "rOxfXOxB-5XbLdgLB-arWve0d-arLvCf";
            public const string PayRefLabel = "rOxfXOxB-rZSiacT4-arWve2h-arLvC9";
            public const string ProductTypeA = "rOxfXOxB-WjxfdHzK-arWve23-arLvCa";
            public const string ProductTypeC = "rOxfXOxB-WjxfdHzK-arWve2q-arLvC0";
            public const string ProductTypeE = "rOxfXOxB-WjxfdHzK-arWvef8-arLPeQ";
            public const string ProductTypeH = "rOxfXOxB-WjxfdHzK-arWve23-arLPew";
            public const string ProductTypeLabel = "rOxfXOxB-Wjxh2gxv-arWve28-arLPem";
            public const string ProductTypeS = "rOxfXOxB-WjxfdHzK-arWve2y-arLPe6";
            public const string ProductTypeT = "rOxfXOxB-WjxfdHzK-arWvef8-arLPep";
            public const string showBookedText = "rOxfXOxB-JAq45I0d-arWve9q-arLPe2";
            public const string showBuyBackProductsText = "rOxfXOxB-JPsiFgfK-arWveay-arLPe9";
            public const string ShowCorporateProductsLabel = "rOxfXOxB-Jvv6dOzK-arWvea8-arLPea";
            public const string showLoyaltyInformationText = "rOxfXOxB-JPRp67zL-arWve03-arLPe0";
            public const string showReservationsText = "rOxfXOxB-J8v6dD4B-arWveaO-arLPRQ";
            public const string statusLabel = "rOxfXOxB-cZS#aCTK-arWvefy-arLPRw";
            public const string ToDateLabel = "rOxfXOxB-BZSla7TX-arWvefh-arLPRm";

            // Purchase History Statuses 
            public const string TEONSLh = "rOxfXOxB-6mVhK8bj-arWve2a-arXBZw";
            public const string TESOLDh = "rOxfXOxB-6m21KYTp-arWve28-arXBZm";
            public const string MIXEDh = "rOxfXOxB-oBlpfP2U-arWvefy-arXBZ6";
            public const string SOLDh = "rOxfXOxB-xZ21fYbp-arWvef3-arXBZp";
            public const string BOOKh = "rOxfXOxB-x4GJYBXp-arWvefd-arXBZf";
            public const string PENDh = "rOxfXOxB-6Rw1ftjh-arWvepa-arXBZ2";
            public const string UNUSEDh = "rOxfXOxB-Psg1iZ2#-arWvefO-arXBZ9";
            public const string APPRVLh = "rOxfXOxB-Ru8htnC6-arWvefq-arXBZa";
            public const string RETURNh = "rOxfXOxB-63i3KjEC-arWvefq-arXBZ0";
            public const string RESERVh = "rOxfXOxB-63ibKjEh-arWvefq-arXBsQ";
            public const string PRINTh = "rOxfXOxB-w33tF5cE-arWvefO-arXBsw";
            public const string CANCELh = "rOxfXOxB-paghzZ2O-arWvefd-arXBsm";

            // Purchase Details Statuses 
            public const string SOLD = "rOxfXOxB-xZ21fYbp-arWvef3-arXBA0";
            public const string TESOLD = "rOxfXOxB-6m21KYTp-arWve28-arXB1Q";
            public const string BOOK = "rOxfXOxB-x4GJYBXp-arWvefd-arXB1w";
            public const string PEND = "rOxfXOxB-6Rw1ftjh-arWvepa-arXB1m";
            public const string UNUSED = "rOxfXOxB-Psg1iZ2#-arWvefO-arXB16";
            public const string APPRVL = "rOxfXOxB-Ru8htnC6-arWvefq-arXB1p";
            public const string RETURN = "rOxfXOxB-63i3KjEC-arWvefq-arXB1f";
            public const string RESERV = "rOxfXOxB-63ibKjEh-arWvefq-arXB12";
            public const string PRINT = "rOxfXOxB-w33tF5cE-arWvefO-arXB19";
            public const string CANCEL = "rOxfXOxB-paghzZ2O-arWvefd-arXB1a";
            public const string MIXED = "rOxfXOxB-oBlpfP2U-arWvefy-arXB10";
            public const string TEONSL = "rOxfXOxB-6mVhK8bj-arWve2a-arXBZQ";
            
            // Purchase Details Action Error Descriptions
            public const string ActionErrorDesc_CF = "rOxfXOxB-jde1gD4X-arWvRwq-aroKMw";
            public const string ActionErrorDesc_CS = "rOxfXOxB-jde1gD4X-arWvRQy-aroKMm";
            public const string ActionErrorDesc_DF = "rOxfXOxB-jde1gD4X-arWvRwO-aroKM6";
            public const string ActionErrorDesc_DS = "rOxfXOxB-jde1gD4X-arWvRQu-aroKMp";
            public const string ActionErrorDesc_DT = "rOxfXOxB-jde1gD4X-arWvRQr-aroKMf";
            public const string ActionErrorDesc_EP = "rOxfXOxB-jde1gD4X-arWvRwu-aroKM2";
            public const string ActionErrorDesc_EV = "rOxfXOxB-jde1gD4X-arWvRQO-aroKM9";
            public const string ActionErrorDesc_FE = "rOxfXOxB-jde1gD4X-arWve9h-aroKMa";
            public const string ActionErrorDesc_MS = "rOxfXOxB-jde1gD4X-arWvRwa-aroKM0";
            public const string ActionErrorDesc_PA = "rOxfXOxB-jde1gD4X-arWvRQ8-aroKEQ";
            public const string ActionErrorDesc_PK = "rOxfXOxB-jde1gD4X-arWveah-aroKEw";
            public const string ActionErrorDesc_PP = "rOxfXOxB-jde1gD4X-arWvR6O-aroKEm";
            public const string ActionErrorDesc_SI = "rOxfXOxB-jde1gD4X-arWve0y-aroKE6";
            public const string ActionErrorDesc_SP = "rOxfXOxB-jde1gD4X-arWvRQ3-aroKEp";
            public const string ActionErrorDesc_UV = "rOxfXOxB-jde1gD4X-arWvRQq-aroKEf";
            public const string ActionErrorDesc_TA = "rOxfXOxB-jde1gD4X-arWveah-aroKE2";
            public const string ActionErrorDesc_BA = "rOxfXOxB-jde1gD4X-arWvead-aroKE9";
            public const string ActionErrorDesc_XX = "rOxfXOxB-jde1gD4X-arWve9u-aroKEa";
            public const string ActionErrorDesc_NM = "rOxfXOxB-jde1gD4X-arWvea8-aroKE0";
            public const string ActionErrorDesc_PB = "rOxfXOxB-jde1gD4X-arWvR6q-aroKAQ";
            public const string ActionErrorDesc_HB = "rOxfXOxB-jde1gD4X-arWvRmq-aroKAw";
            public const string ActionErrorDesc_TB = "rOxfXOxB-jde1gD4X-arWvRQa-aroKAm";
            public const string ActionErrorDesc_AB = "rOxfXOxB-jde1gD4X-arWvRQh-aroKA6";
            public const string ActionErrorDesc_P1 = "rOxfXOxB-jde1gD4X-arWve0y-aroKAp";
            public const string ActionErrorDesc_T1 = "rOxfXOxB-jde1gD4X-arWve0r-aroKAf";
            public const string ActionErrorDesc_A1 = "rOxfXOxB-jde1gD4X-arWve0y-aroKA2";
            public const string ActionErrorDesc_C2 = "rOxfXOxB-jde1gD4X-arWvRma-aroKA9";
            public const string ActionErrorDesc_C1 = "rOxfXOxB-jde1gD4X-arWve0h-aroKAa";
            public const string ActionErrorDesc_NB = "rOxfXOxB-jde1gD4X-arWvRQa-aroKs0";
            public const string ActionErrorDesc_AS = "rOxfXOxB-jde1gD4X-arWvRwr-aroK0Q";
            public const string ActionErrorDesc_TS = "rOxfXOxB-jde1gD4X-arWvRw8-aroK0w";
            public const string ActionErrorDesc_PH = "rOxfXOxB-jde1gD4X-arWvR2q-aroK0m";
            public const string ActionErrorDesc_TM = "rOxfXOxB-jde1gD4X-arWvRwy-aroK06";
            public const string ActionErrorDesc_NH = "rOxfXOxB-jde1gD4X-arWvRwy-aroVR9";
            public const string ActionErrorDesc_IC = "rOxfXOxB-jde1gD4X-arWveaq-aroVMw";
            public const string ActionErrorDesc_AR = "rOxfXOxB-jde1gD4X-arWvRwa-arXV00";
            public const string ActionErrorDesc_TX = "rOxfXOxB-jde1gD4X-arWvR6u-arXBAf";
            public const string ActionErrorDesc_TE = "rOxfXOxB-jde1gD4X-arWvRma-arXBAa";
            public const string ActionErrorDesc_SX = "rOxfXOxB-jde1gD4X-arWvRfr-arXKRf";
            public const string PrintSuccessMessage = "rOxfXOxB-WDb#tvoZ-arWvRQy-aroVRa";
            public const string ResendEmailMessage = "rOxfXOxB-QdSFGvsL-arWve2r-aroVR0";
            public const string PrintErrorMessage = "rOxfXOxB-Wvl46ZoB-arWve0y-aroVMQ";
            public const string ShowDirectDebitSummary = "rOEEXOfK-JvH4Q84F-arWve2u-aroVM6";
            public const string ActionErrorDesc_AC = "rOxfXOxB-jde1gD4X-arWvRQr-arWveQ";
            public const string ActionErrorDesc_AA = "rOxfXOxB-jde1gD4X-arWvRQ8-arWveQ";
            public const string ActionErrorDesc_AT = "rOxfXOxB-jde1gD4X-arWvRwO-arXVCw";

            public const string StopCodeDiagCancBtnText = "rOxfXOxB-cLv2FgzK-arWve28-arXC10";
            public const string StopCodeDiagOkBtnText = "rOxfXOxB-cLv2Bgof-arWve2u-arXCZQ";
            public const string StopCodeDiagMessageText = "rOxfXOxB-cLv2FgzK-arWvRQh-arXCZw";
            public const string StopCodeDiagTitleText = "rOxfXOxB-cLv2Bgof-arWve93-arXCZm";

            public const string IncludeCorporateHospitalityItems = "rOEEXOfK-CODuXOL4-arWve9u-arLTM6";

            //Purchase Details Page Labels Screen-1
            public const string membershipMatchLabel = "rOxfXOxB-Q8leUkgG-arWve9u-arLn06";
            public const string seatLabel = "rOxfXOxB-QnxeGYoa-arWvef3-arLn0m";
            public const string ProductDateLabel = "rOxfXOxB-Wjxh2gxv-arWve2h-arLn0w";
            public const string customerNameLabel = "rOxfXOxB-5OF4nkoB-arWve2r-arLn0Q";
            public const string priceLabel = "rOxfXOxB-WWDQUdoM-arWvefd-arLnsa";
            public const string priceBandLabel = "rOxfXOxB-WWv0Dh1Z-arWve2q-arLnsf";
            public const string FulfilmentMethodLabel = "rOxfXOxB-5ll4G1ot-arWve9h-arLnsp";
            public const string PackageStatusLabel = "rOxfXOxB-rOEXQOL4-arWve9y-arLns6";
            public const string TicketNumberLabel = "rOxfXOxB-tRxQgg4G-arWve2r-arLnsm";
            public const string DespatchDateLabel = "rOxfXOxB-QEE4ngoB-arWve2r-arLnsw";

            //Purchase Details Page Labels Screen-2
            public const string QuantityHeading = "rOxfXOxB-5jvZdieA-arWve23-arLnsQ";
            public const string ComponentHeading = "rOxfXOxB-BOxeXIJL-arWve2d-arLnZ0";
            public const string ComponentNetPriceHeading = "rOxfXOxB-BOx6QvzL-arWve93-arLnZ9";
            public const string ComponentDiscountHeading = "rOxfXOxB-BOj#QvzL-arWve9u-arLnZa";
            public const string PriceHeading = "rOxfXOxB-WOa2QvoL-arWvefy-arLnZ2";
            public const string DetailHeading = "rOxfXOxB-QfjrPhL3-arWvef8-arLnZf";

            public DMPurchaseHistory(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }

            public override void SetModuleConfiguration()
            {
                MConfigs.Add(numberOfProductsVariable, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(PaymentRefRegex, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(RetailFromTicketingDB, DataType.BOOL, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ShowCorporateProductsCheckBox, DataType.BOOL, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ShowPackageProductsOption, DataType.BOOL, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ShowProductOrDescriptionSearchBox, DataType.BOOL, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ShowRetailPurchaseHistoryOptions, DataType.BOOL, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ShowTicketingPurchaseHistoryOptions, DataType.BOOL, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(IncludeCorporateHospitalityItems, DataType.BOOL, DisplayTabs.TabHeaderPurchaseHistory);

                MConfigs.Add(customerNumberLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(dateLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(quantityLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(statusLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(RetailCustomerPurchaseRefColumnHeading, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);

                MConfigs.Add(ShowTicketingProductsLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ShowRetailProductsLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(showBookedText, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(showBuyBackProductsText, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ShowCorporateProductsLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(showLoyaltyInformationText, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(showReservationsText, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);

                MConfigs.Add(FromDateLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ToDateLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ProductTypeLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(PayRefLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ProductOrDescriptionSearchLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ProductTypeA, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ProductTypeC, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ProductTypeE, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ProductTypeH, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ProductTypeS, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ProductTypeT, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(TicketingPaymentRefLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(TicketingPackageIDLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(ClearButtonText, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(filterButton, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);

                MConfigs.Add(WebOrderNumberLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(BackOfficerOrderNumberLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);

                MConfigs.Add(RetailOrderItemText, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(PaymentRefRegexErrorText, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(FromToDateCompareErrorText, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(FutureDatesErrorText, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(NoPaymentReference, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);
                MConfigs.Add(NoPurchaseFound, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);

                MConfigs.Add(StatusOptions, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory);

                //Purchase History Statuses
                MConfigs.Add(MIXEDh, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory, uniqueId: "purchaseHistory");
                MConfigs.Add(SOLDh, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory, uniqueId: "purchaseHistory");
                MConfigs.Add(BOOKh, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory, uniqueId: "purchaseHistory");
                MConfigs.Add(PENDh, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory, uniqueId: "purchaseHistory");
                MConfigs.Add(UNUSEDh, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory, uniqueId: "purchaseHistory");
                MConfigs.Add(APPRVLh, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory, uniqueId: "purchaseHistory");
                MConfigs.Add(RETURNh, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory, uniqueId: "purchaseHistory");
                MConfigs.Add(RESERVh, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory, uniqueId: "purchaseHistory");
                MConfigs.Add(PRINTh, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory, uniqueId: "purchaseHistory");
                MConfigs.Add(CANCELh, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory, uniqueId: "purchaseHistory");
                MConfigs.Add(TESOLDh, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory, uniqueId: "purchaseHistory");
                MConfigs.Add(TEONSLh, DataType.TEXT, DisplayTabs.TabHeaderPurchaseHistory, uniqueId: "purchaseHistory");

                // Purchase Details Statuses
                MConfigs.Add(MIXED, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(SOLD, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(BOOK, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(PEND, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(UNUSED, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(APPRVL, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(RETURN, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(RESERV, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(PRINT, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(CANCEL, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(TESOLD, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(TEONSL, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);

                MConfigs.Add(ShowDirectDebitSummary, DataType.BOOL, DisplayTabs.TabHeaderPurchaseDetails);

                // Purchase Details Action Error Descriptions
                MConfigs.Add(ActionErrorDesc_CF, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_CS, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_DF, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_DS, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_DT, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_EP, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_EV, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_FE, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_MS, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_PA, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_PK, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_PP, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_SI, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_SP, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_UV, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_TA, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_BA, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_XX, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_NM, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_PB, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_HB, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_TB, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_AB, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_P1, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_T1, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_A1, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_C2, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_C1, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_NB, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_AS, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_TS, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_PH, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_TM, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_NH, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_IC, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_AR, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_TX, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_TE, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_SX, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_AC, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_AA, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ActionErrorDesc_AT, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(PrintErrorMessage, DataType.TEXT, DisplayTabs.TabHeaderErrors);

                MConfigs.Add(PrintSuccessMessage, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ResendEmailMessage, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(StopCodeDiagCancBtnText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(StopCodeDiagOkBtnText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(StopCodeDiagMessageText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(StopCodeDiagTitleText, DataType.TEXT, DisplayTabs.TabHeaderErrors);

                //Purchase Details Label Screen-1
                MConfigs.Add(membershipMatchLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(seatLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(ProductDateLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(customerNameLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(priceLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(priceBandLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(FulfilmentMethodLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(PackageStatusLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(TicketNumberLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(DespatchDateLabel, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);

                //Purchase Details Label Screen-2
                MConfigs.Add(QuantityHeading, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(ComponentHeading, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(ComponentNetPriceHeading, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(ComponentDiscountHeading, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(PriceHeading, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);
                MConfigs.Add(DetailHeading, DataType.TEXT, DisplayTabs.TabHeaderPurchaseDetails);

                Populate();
            }
        }
    }
}
