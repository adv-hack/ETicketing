using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMCheckoutOrderConfirmation : DMBase
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
            static private string _ModuleTitle = "Checkout Order Confirmation";
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

            public const string SingleBasketPaymentDetails = "rOxfXOxB-tpx6fOeP-arWvM0d-arXKRa";
            public const string SingleBasketPaymentMessage = "rOxfXOxB-tpx67CeU-arWvEwa-arXKR0";

            public DMCheckoutOrderConfirmation(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {
                MConfigs.Add(SingleBasketPaymentDetails, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(SingleBasketPaymentMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                Populate();
            }
        }
    }
}