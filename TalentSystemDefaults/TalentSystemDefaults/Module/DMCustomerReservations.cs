

using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMCustomerReservations : DMBase
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
            static private string _ModuleTitle = "Customer Reservations";
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

            public const string ProductColumnHeadingText = "rOxfXOxB-Wjz7hAfB-arWve9a-arojs6";
            public const string CustomerColumnHeadingText = "rOxfXOxB-5OSXWILT-arWvea3-arojsp";
            public const string PriceCodeColumnHeadingText = "rOxfXOxB-WApk67gL-arWve9a-arojsf";
            public const string PriceBandColumnHeadingText = "rOxfXOxB-WWpk67gL-arWve9a-arojs2";
            public const string ExpiryDateColumnHeadingText = "rOxfXOxB-4Xru2Jp4-arWvea3-arojs9";
            public const string SeatColumnHeadingText = "rOxfXOxB-Q7y4BIfK-arWve2r-arojsa";
            public const string DespatchDateColumnHeadingText = "rOxfXOxB-QEE4GdLV-arWveay-arojs0";
            public const string QuantityColumnHeadingText = "rOxfXOxB-5jSXWIeT-arWve9h-aroj0Q";
            public const string SelectColumnHeadingText = "rOxfXOxB-QaFrFgLK-arWve9q-aroj0w";
            public const string TicketNumberColumnHeadingText = "rOxfXOxB-tRxQGdLV-arWveay-aroj0m";
            public const string ltlNotDespatched = "rOxfXOxB-cXv6iyKF-arWve9d-aroj06";

            
            public const string ShowCustomerColumn = "rOEEXOfK-J8xQmeoV-arWve2y-aroj0a";
            public const string ShowDespatchDateColumn = "rOEEXOfK-J86MH80F-arWve2a-aroj00";
            public const string ShowExpiryDateColumn = "rOEEXOfK-J5HeWD4u-arWve28-arojCQ";
            public const string ShowPriceCodeColumn = "rOEEXOfK-JcpZ9EQZ-arWve2h-arojCw";
            public const string ShowProductColumn = "rOEEXOfK-JAEW5xQO-arWve2u-arojCm";
            public const string ShowQuantityColumn = "rOEEXOfK-JWEwmoLL-arWve2y-arojC6";
            public const string ShowReservationReferenceColumn = "rOEEXOfK-J8v6OLU6-arWve98-arojCp";
            public const string ShowSeatColumn = "rOEEXOfK-JWSXGebd-arWve23-arojCf";
            public const string ShowSelectColumn = "rOEEXOfK-J7rujVsy-arWve2d-arojC2";
            public const string ShowTicketNumberColumn = "rOEEXOfK-JEwXH8KF-arWve2a-arojC9";

            public DMCustomerReservations(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {

                // Texts tab
                MConfigs.Add(ProductColumnHeadingText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(CustomerColumnHeadingText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(PriceCodeColumnHeadingText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(PriceBandColumnHeadingText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ExpiryDateColumnHeadingText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(SeatColumnHeadingText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(DespatchDateColumnHeadingText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(QuantityColumnHeadingText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(SelectColumnHeadingText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(TicketNumberColumnHeadingText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlNotDespatched, DataType.TEXT, DisplayTabs.TabHeaderTexts);

                MConfigs.Add(ShowCustomerColumn, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(ShowDespatchDateColumn, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(ShowExpiryDateColumn, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(ShowPriceCodeColumn, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(ShowProductColumn, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(ShowQuantityColumn, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(ShowReservationReferenceColumn, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(ShowSeatColumn, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(ShowSelectColumn, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(ShowTicketNumberColumn, DataType.BOOL, DisplayTabs.TabHeaderDefaults);


                Populate();
            }
        }
    }
}


