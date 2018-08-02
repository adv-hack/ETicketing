using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMOnAccountDetails : DMBase
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
            static private string _ModuleTitle = "On Account Details";
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

            public const string ltlRefund = "BjS4aVLZ-cDEkdIL9-arWvef8-arLP16";
            public const string ltlSpend = "BjS4aVLZ-c0Ss9v1B-arWvefy-arLP1p";
            public const string ltlReward = "BjS4aVLZ-cWEkdIL9-arWvef8-arLP1f";
            public const string ltlAdjustment = "BjS4aVLZ-cDxrHksy-arWve2y-arLP12";
            public const string ltlSeatTransfer = "BjS4aVLZ-cja#G7o9-arWve9O-arLP19";
            public const string ltlReprice = "BjS4aVLZ-cvS6GVKf-arWvefa-arLP1a";
            public const string ltlSeatCancel = "BjS4aVLZ-cj64dvsK-arWve2h-arLP10";
            public const string ltlSaleCancel = "BjS4aVLZ-cO64GvsB-arWve2h-arLPZQ";
            public const string ltlBuyBackReward = "BjS4aVLZ-cpi4Dz64-arWve9d-arLPZw";
            public const string ltlTicketExchangeCredit = "BjS4aVLZ-cVIdGgKK-arWvea8-arLPZm";
            public const string ltlVoucherRefund = "BjS4aVLZ-cEi4#Liy-arWve9q-arLPZ6";
            public const string ltlVoucherExchange = "BjS4aVLZ-cEgodD2G-arWve9h-arLPZp";
            public const string ltlGiftVoucher = "BjS4aVLZ-cj6MwDsD-arWve2r-arLPZf";
            public const string ltlPayment = "BjS4aVLZ-clS6dVec-arWvefa-arLPZ2";
            public const string ltlPartPayment = "BjS4aVLZ-cjF4OOsE-arWve2r-arLPZ9";
            public const string ltlrefundtocard = "BjS4aVLZ-cDpdGv49-arWve93-arLPZa";

            public DMOnAccountDetails(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {


                MConfigs.Add(ltlRefund, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlSpend, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlReward, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlAdjustment, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlSeatTransfer, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlReprice, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlSeatCancel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlSaleCancel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlBuyBackReward, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlTicketExchangeCredit, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlVoucherRefund, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlVoucherExchange, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlGiftVoucher, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlPayment, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlPartPayment, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ltlrefundtocard, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                Populate();
            }
        }
    }
}