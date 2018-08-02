using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMBasket : DMBase
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
            static private string _ModuleTitle = "Basket";
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

            public const string UpdateBasketPromptText = "BjS4aVLZ-upx6G#Xf-arWve03-aroBEp";
            public const string HighlightUpdateBasketButtonWhenRequired = "BjS6aVxZ-tKDZG1lT-arWve03-aroBEf";
            public const string ShowUpdateBasketPrompt = "BjS6aVxZ-JLueS8JF-arWve9y-aroHA9";
            public const string UpdateBasketPromptDuration = "BjS6aVxZ-upx6HgJy-arWve9y-aroBE9";
            public const string ShowDirectDebitSummary = "BjS6aVxZ-JvH4Q84F-arWve23-aroVMm";
            public const string SEQ02 = "0rrveQOP-6UWTKQOe-arWvewq-arLPEw";
            public const string FLG20 = "0rrveQOP-KSYvJmqe-arWvewq-arLPEm";
            public const string XF630A = "0rrveQOP-71WDVQOg-arWvewq-arLPMm";
            public const string TM080A = "0rrveQOP-mmWD7QOW-arWvewq-arLPM6";
            public const string TM090A = "0rrveQOP-mmWD7QOr-arWvewq-arLPMp";
            public const string XF760A = "0rrveQOP-71WDVQOJ-arWvewq-arLPMf";
            public const string XF770A = "0rrveQOP-71WDVQOb-arWvewq-arLPM2";
            public const string DT050A = "0rrveQOP-MXWDFQON-arWvewq-arLPM9";
            public const string XF270A = "0rrveQOP-71WDVQOb-arWvewq-arLPMa";
            public const string XF810A = "0rrveQOP-71WDVQOi-arWvewq-arLPM0";
            public const string XF820A = "0rrveQOP-71WDVQOl-arWvewq-arLPEQ";

            //Basket Error text
            public const string TicketingBasketErrorText = "WAF#GxYV-tcv#OAoB-arWvMma-arLj1p";

            public DMBasket(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {
                MConfigs.Add(UpdateBasketPromptText, DataType.TEXT, DisplayTabs.TabHeaderTexts);

                MConfigs.Add(XF630A, DataType.BOOL_YN, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(SEQ02, DataType.TEXT, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(TM080A, DataType.TEXT, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(TM090A, DataType.TEXT, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(FLG20, DataType.BOOL_10, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(XF760A, DataType.BOOL_YN, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(XF770A, DataType.BOOL_YN, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(DT050A, DataType.TEXT, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(XF270A, DataType.BOOL_YN, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(XF810A, DataType.BOOL_YN, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(XF820A, DataType.BOOL_YN, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(HighlightUpdateBasketButtonWhenRequired, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(ShowUpdateBasketPrompt, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(UpdateBasketPromptDuration, DataType.TEXT, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(ShowDirectDebitSummary, DataType.BOOL, DisplayTabs.TabHeaderAttributes);

                //Basket Error Text
                MConfigs.Add(TicketingBasketErrorText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                Populate();
            }
        }
    }
}