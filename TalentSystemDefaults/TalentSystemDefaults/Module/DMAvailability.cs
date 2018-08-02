using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMAvailability : DMBase
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
            static private string _ModuleTitle = "Availability";
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

            public const string travelDDLAvailabilityMaskLabel = "BjS4aVLZ-WXOedOov-arWvAmd-aroBAm";
            public const string travelAvailabilityMaskLabel = "BjS4aVLZ-WuSeQY4U-arWvApO-aroHAp";
            public const string awayAvailabilityMaskLabel = "BjS4aVLZ-3WCE8rtM-arWvA6r-aroHAf";
            public const string eventAvailabilityMaskLabel = "BjS4aVLZ-eQv0nOoa-arWvA6a-aroHA2";

            public DMAvailability(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {
                MConfigs.Add(eventAvailabilityMaskLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(awayAvailabilityMaskLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(travelAvailabilityMaskLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(travelDDLAvailabilityMaskLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                Populate();
            }
        }
    }
}