

using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMDespatchProcess : DMBase
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
            static private string _ModuleTitle = "Despatch Process";
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

            public const string F0044B = "0rrveQOP-aSo5epu5-arWvewy-arojAp";

            public DMDespatchProcess(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {
                MConfigs.Add(F0044B, DataType.DROPDOWN, DisplayTabs.TabHeaderGeneral);
                Populate();
            }
        }
    }
}

