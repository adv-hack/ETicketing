using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMPasswordEncryption : DMBase
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
            static private string _ModuleTitle = "Password Encryption";
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
            public const string ENCRYPTED_PASSWORD_PLACEHOLDER = "jlefG3WV-Pm3Dte2p-arWve9y-arovEm";
            public const string msgResetSignedInTitle = "rOxfXOxB-tBEkudYz-arWve2d-arovE6";
            public const string msgResetSignedInError = "rOxfXOxB-tBeuuxYz-arWvR6q-arovEp";
            public const string msgResetNoFieldEntered = "rOxfXOxB-BLxQUvcL-arWve9d-arovEf";
            public const string msgResetUnspecifiedError = "rOxfXOxB-CcKLXd4G-arWvRQa-arovE2";
            public const string msgResetSuccessMessage = "rOxfXOxB-58benIbZ-arWvRQa-arovE9";
            public const string PasswordMismatch = "rOxfXOxB-QWpQcI6a-arWvef8-arovE0";
            public const string ResetPasswordButton = "rOxfXOxB-QWpQcI6a-arWve9q-arovE0";
            public const string ResetPasswordTitle = "rOxfXOxB-QWpQGgzd-arWve9d-arovAQ";
            public const string ResetPasswordLabel = "rOxfXOxB-QWpQGgzd-arWvea3-arovAw";
            public const string PasswordConfirmLabel = "rOxfXOxB-rvaRUCQX-arWveay-arovAm";
            public const string RegexLabel = "rOxfXOxB-QWi4UJoM-arWve08-arovA6";
            public const string msgResetSuccessTitle = "rOxfXOxB-58S4FVoK-arWvefy-arovAp";
            public const string ForgottenPasswordLabel = "rOxfXOxB-Bjv#asLX-arWve0q-arovAf";
            public const string NoEmailError = "rOxfXOxB-B7pQKDxG-arWvR6u-arovA2";
            public const string CustomerError = "rOxfXOxB-5OeuGx0B-arWvRw8-arovA9";
            public const string msgForgottenNoFieldEntered = "rOxfXOxB-BLxQUvcL-arWvRQO-arovAa";
            public const string msgForgottenSuccessMessage = "rOxfXOxB-58benIbZ-arWvRmd-arovA0";
            public const string ForgottenPasswordButton = "rOxfXOxB-Bjv#dvLQ-arWve9h-arov1Q";
            public const string InvalidCharacters = "rOxfXOxB-CLee4dx4-arWvRQr-arov1w";
            public const string msgForgottenSuccessTitle = "rOxfXOxB-58S4FVoK-arWvefa-arov1m";
            public const string msgForgottenUnspecifiedError = "rOxfXOxB-CcKLXd4G-arWvRwd-arov16";
            public const string msgForgottenSignedInTitle = "rOxfXOxB-tBEkudYz-arWve28-arov1p";
            public const string msgForgottenSignedInError = "rOxfXOxB-tBeuuxYz-arWvR6h-arov1f";
            public const string PR = "WAF#GxYV-ww3tccj6-arWvea3-arov12";

            public DMPasswordEncryption(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {


                MConfigs.Add(ENCRYPTED_PASSWORD_PLACEHOLDER, DataType.TEXT, DisplayTabs.TabHeaderDefaults);
                MConfigs.Add(msgResetSignedInTitle, DataType.TEXT, DisplayTabs.TabHeaderLabels, uniqueId: "Reset");
                MConfigs.Add(msgResetSignedInError, DataType.TEXT, DisplayTabs.TabHeaderErrors, uniqueId: "Reset");
                MConfigs.Add(msgResetNoFieldEntered, DataType.TEXT, DisplayTabs.TabHeaderLabels, uniqueId: "Reset");
                MConfigs.Add(msgResetUnspecifiedError, DataType.TEXT, DisplayTabs.TabHeaderErrors, uniqueId: "Reset");
                MConfigs.Add(msgResetSuccessMessage, DataType.TEXT, DisplayTabs.TabHeaderLabels, uniqueId: "Reset");
                MConfigs.Add(PasswordMismatch, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ResetPasswordButton, DataType.TEXT, DisplayTabs.TabHeaderButtons);
                MConfigs.Add(ResetPasswordTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(ResetPasswordLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
                MConfigs.Add(PasswordConfirmLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
                MConfigs.Add(RegexLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
                MConfigs.Add(msgResetSuccessTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts, uniqueId: "Reset");
                MConfigs.Add(ForgottenPasswordLabel, DataType.TEXT, DisplayTabs.TabHeaderLabels);
                MConfigs.Add(NoEmailError, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(CustomerError, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(msgForgottenNoFieldEntered, DataType.TEXT, DisplayTabs.TabHeaderTexts, uniqueId: "Forgotten");
                MConfigs.Add(msgForgottenSuccessMessage, DataType.TEXT, DisplayTabs.TabHeaderTexts, uniqueId: "Forgotten");
                MConfigs.Add(ForgottenPasswordButton, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(InvalidCharacters, DataType.TEXT, DisplayTabs.TabHeaderTexts);
                MConfigs.Add(msgForgottenSuccessTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts, uniqueId: "Forgotten");
                MConfigs.Add(msgForgottenUnspecifiedError, DataType.TEXT, DisplayTabs.TabHeaderTexts, uniqueId: "Forgotten");
                MConfigs.Add(msgForgottenSignedInTitle, DataType.TEXT, DisplayTabs.TabHeaderTexts, uniqueId: "Forgotten");
                MConfigs.Add(msgForgottenSignedInError, DataType.TEXT, DisplayTabs.TabHeaderTexts, uniqueId: "Forgotten");
                MConfigs.Add(PR, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                Populate();
            }
        }
    }
}