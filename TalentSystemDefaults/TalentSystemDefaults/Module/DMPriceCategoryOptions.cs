using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMPriceCategoryOptions : DMBase
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
            static private string _ModuleTitle = "Seat/Price Selection Options";
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

            public const string ShowPriceBandList = "BjS6aVxZ-Jcvr5xeQ-arWve2r-arojAf";
            public const string ShowPriceBandListAsDropDown = "BjS6aVxZ-JcvrfHla-arWve9r-arojA2";
            public const string ShowPriceDropDownList = "BjS6aVxZ-JceunqKd-arWve9q-arojA0";
            public const string ShowPricingOptionsAsDropDown = "BjS6aVxZ-JcZInDba-arWve9a-aroj1Q";
            public const string ShowStandAreaAsDropDown = "BjS6aVxZ-JWe4fvLQ-arWve9u-aroj1w";
            public const string XF018B = "0rrveQOP-71n5Vari-arWvR6r-arLPE6";
            public const string XF019B = "0rrveQOP-71#5V0ai-arWvR6r-arLPEp";
            public const string XF020B = "0rrveQOP-71W5VQOl-arWvR6r-arLPEf";
            public const string XF021B = "0rrveQOP-71N5Vw3l-arWvR6r-arLPE2";
            public const string XF022B = "0rrveQOP-71Y5Vmql-arWvR6r-arLPE9";
            public const string XF023B = "0rrveQOP-71c5V6dl-arWvR6r-arLPEa";
            public const string XF024B = "0rrveQOP-71o5Vpul-arWvea8-arLPE0";
            public const string XF025B = "0rrveQOP-71X5Vfyl-arWvea8-arLPAQ";
            public const string XF026B = "0rrveQOP-71L5V2hl-arWvea8-arLPAw";
            public const string XF027B = "0rrveQOP-71U5V98l-arWvea8-arLPAm";
            public const string XF028B = "0rrveQOP-71n5Varl-arWvea8-arLPA6";
            public const string XF029B = "0rrveQOP-71#5V0al-arWvea8-arLPAp";
            public const string ResetButtonText = "BjS4aVLZ-QDaldgp6-arWve9y-aroj19";
            public const string NoQuantityErrorMessage = "BjS4aVLZ-BjgQQtLX-arWve0y-aroj1a";
            public const string MaximumPriceLabelText = "BjS4aVLZ-rl64BOjL-arWvea3-aroj10";
            public const string MinimumPriceLabelText = "BjS4aVLZ-tl64BdjL-arWvea3-arojZQ";
            public const string SelectMaxPriceText = "BjS4aVLZ-QqeEGL4E-arWveaq-arojZw";
            public const string SelectMinPriceText = "BjS4aVLZ-QqeEGL4E-arWveaq-arojZm";
            public const string ExessiveQuantityErrorMessage = "BjS4aVLZ-4Qvr5I2G-arWvMQr-arojZ6";
            public const string PricingFilterOptionText = "BjS4aVLZ-WKE4FgeK-arWvead-arojZp";
            public const string MinMaxPricingHeaderText = "BjS4aVLZ-twjrFgpK-arWve0O-arojZf";
            public const string SelectStandAreaOptionsText = "BjS4aVLZ-QUKD57sX-arWve0r-arojZ2";
            public const string SelectStandAreaHeaderText = "BjS4aVLZ-QUKDGIoG-arWve08-arojZ9";
            public const string PriceBandOptionsText = "BjS4aVLZ-WWD6ddJH-arWve9r-arojZa";
            public const string PriceBandOptionsHeaderText = "BjS4aVLZ-WWD627gY-arWveau-arojZ0";
            public const string MinMaxPriceMask = "BjS4aVLZ-twxFQx0U-arWve0q-arojsQ";
            public const string StandAreaDescriptionMask = "BjS4aVLZ-cvx#WCJ4-arWvRwy-arojsw";

            public const string PRICE_AND_AREA_SELECTION_ENABLED = "jlefG3WV-wu5tvBX1-arWve2a-arojsm";
            public const string HIGHLIGHT_LOGGED_IN_CUSTOMER_PRICE_BAND = "jlefG3WV-oo2Iy8PD-arWve9h-arLPZ0";

            public DMPriceCategoryOptions(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {

                //Defaults
                MConfigs.Add(ShowPriceBandList, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(ShowPricingOptionsAsDropDown, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(ShowPriceDropDownList, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(ShowStandAreaAsDropDown, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(ShowPriceBandListAsDropDown, DataType.BOOL, DisplayTabs.TabHeaderDefaults);                           
                MConfigs.Add(PRICE_AND_AREA_SELECTION_ENABLED, DataType.BOOL, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(HIGHLIGHT_LOGGED_IN_CUSTOMER_PRICE_BAND, DataType.BOOL, DisplayTabs.TabHeaderDefaults);

                MConfigs.Add(XF018B, DataType.TEXT, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(XF019B, DataType.TEXT, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(XF020B, DataType.TEXT, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(XF021B, DataType.TEXT, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(XF022B, DataType.TEXT, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(XF023B, DataType.TEXT, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(XF024B, DataType.TEXT, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(XF025B, DataType.TEXT, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(XF026B, DataType.TEXT, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(XF027B, DataType.TEXT, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(XF028B, DataType.TEXT, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(XF029B, DataType.TEXT, DisplayTabs.TabHeaderDefaults);

                //Text
                MConfigs.Add(ResetButtonText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(NoQuantityErrorMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(MaximumPriceLabelText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(MinimumPriceLabelText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(SelectMaxPriceText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(SelectMinPriceText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ExessiveQuantityErrorMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(PricingFilterOptionText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(MinMaxPricingHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(SelectStandAreaOptionsText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(SelectStandAreaHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(PriceBandOptionsText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(PriceBandOptionsHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(MinMaxPriceMask, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(StandAreaDescriptionMask, DataType.TEXT, DisplayTabs.TabHeaderTexts);

                Populate();
            }
        }
    }
}