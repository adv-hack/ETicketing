using System.Collections.Generic;
using TalentSystemDefaults.DataAccess.DataObjects;

namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMGenderType : DMBase
        {
            static private bool _EnableAsModule = false;
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
            static private string _ModuleTitle = "Gender Types";
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

            public const string USER51 = "mzlv1QOP-ksXP9fy6-arWvewa-arLvep";
            public const string PGMD51 = "mzlv1QOP-IwXPpfyP-arWvewa-arLvef";
            public const string UPDT51 = "mzlv1QOP-RsXPtfyk-arWvemO-arLve2";
            public const string ACTR51 = "mzlv1QOP-0uXPvfy6-arWvemO-arLve9";
            public const string CONO51 = "mzlv1QOP-xaXPofyp-arWvemq-arLvea";
            public const string TYPE51 = "mzlv1QOP-TmXPlfyh-arWvemd-arLve0";
            public const string DECP51 = "mzlv1QOP-6XXPKfyE-arWvemO-arLvR6";
            public const string CODE51 = "mzlv1QOP-xaXPofyh-arWvewa-arLvRQ";
            public const string DESC51 = "mzlv1QOP-6XXPKfyO-arWvewa-arLvRw";
            public const string VALU51 = "mzlv1QOP-p9XPzfyC-arWvemO-arLvRm";

            public DMGenderType(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {

                //Front end gender code and description
                if (mode == MODE_CREATE)
                {
                    MConfigs.Add(CODE51, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> { VG.Mandatory, VG.MinLength, VG.MaxLength }, minLength: 1, maxLength: 1));
                }
                else
                {
                    MConfigs.Add(CODE51, DataType.LABEL, DisplayTabs.TabHeaderGeneral);
                }


                MConfigs.Add(USER51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(PGMD51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(UPDT51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ACTR51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CONO51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(TYPE51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(DECP51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(DESC51, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> { VG.Mandatory, VG.MinLength, VG.MaxLength }, minLength: 1, maxLength: 50));
                MConfigs.Add(VALU51, DataType.HIDDEN, DisplayTabs.TabHeaderGeneral);
                Populate();
            }
            public override void SetHiddenFields()
            {
                dtHiddenFields.Rows.Add(USER51, "", HiddenType.SETTING, PropertyName.AGENTNAME.ToString());
                dtHiddenFields.Rows.Add(PGMD51, "", HiddenType.SETTING, PropertyName.STOREDPROC.ToString());
                dtHiddenFields.Rows.Add(UPDT51, "", HiddenType.SETTING, PropertyName.ISERIESTODAYSDATE.ToString());
                dtHiddenFields.Rows.Add(ACTR51, "", HiddenType.DEFAULT);
                dtHiddenFields.Rows.Add(CONO51, "", HiddenType.DEFAULT);
                dtHiddenFields.Rows.Add(TYPE51, "", HiddenType.DEFAULT);
                dtHiddenFields.Rows.Add(DECP51, "", HiddenType.DEFAULT);
                dtHiddenFields.Rows.Add(VALU51, VALU51, HiddenType.FOREIGN);
            }

            public override bool Validate()
            {
                // Validate the default validation first
                bool retVal = base.Validate();
                if (retVal)
                {

                    DataAccess.DataObjects.MD501 objTblGenderType = new DataAccess.DataObjects.MD501(ref settings);
                    string genderTypeCode = string.Empty;
                    bool CheckValid = true;
                    string ValidationMessage = string.Empty;
                    ModuleConfiguration objMConfig = null;
                    if (settings.Mode == "create")
                    {

                        // Validate that the gender type does not exist on the ebusiness tables
                        genderTypeCode = System.Convert.ToString(MConfigs.Find(x => x.ConfigurationItem.DefaultName == "CODE51").ConfigurationItem.UpdatedValue);
                        CheckValid = System.Convert.ToBoolean(objTblGenderType.DoesDescriptionItemExist("GEND",genderTypeCode));
                        if (!CheckValid)
                        {
                            ValidationMessage = "Gender code already exists.";
                            objMConfig = MConfigs.Find(x => x.ConfigurationItem.DefaultName == "CODE51");
                            objMConfig.ErrorMessage = ValidationMessage;
                            retVal = false;
                        }
                    }
                }

                return retVal;
            }

            public override string BackUrl()
            {
                return string.Format("DefaultList.aspx?listname={0}&businessUnit={1}", settings.Module_Name, settings.BusinessUnit);
            }
        }
    }
}
