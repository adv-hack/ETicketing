using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMThirdPartyIntegration : DMBase
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
            static private string _ModuleTitle = "Third Party Integration";
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

            public const string DISCOUNTIF = "jlefG3WV-oRHpV8DC-arWveph-arXKM2";
            public const string DISCOUNTIFPSK = "jlefG3WV-oR3sisbj-arWve98-arXKM9";

            public DMThirdPartyIntegration(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {
                MConfigs.Add(DISCOUNTIF, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(DISCOUNTIFPSK, DataType.TEXT, DisplayTabs.TabHeaderGeneral);

                Populate();
            }
        }
    }
}