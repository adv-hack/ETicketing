using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMProgressBar : DMBase
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
            static private string _ModuleTitle = "Progress Bar";
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

            public const string PROGRESS_BAR_SUBTYPES = "jlefG3WV-wU5tKjbD-arWvefd-arojE9";
            public const string SHOW_PROGRESS_BAR = "jlefG3WV-13gsocb#-arWvefu-arojEa";

            public DMProgressBar(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {

                MConfigs.Add(SHOW_PROGRESS_BAR, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(PROGRESS_BAR_SUBTYPES, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                Populate();
            }
        }
    }
}