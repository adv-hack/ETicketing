using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMActivities : DMBase
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
            static private string _ModuleTitle = "Activities";
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

            public const string ShowDateColumn = "BjS6aVxZ-JjSXQebd-arWve2d-arcC1f";
            public const string ShowStatusColumn = "BjS6aVxZ-JWrujOsy-arWve2y-arcC12";
            public const string ShowSubjectColumn = "BjS6aVxZ-JnEW51QO-arWve2h-arcC19";
            public const string ShowUserColumn = "BjS6aVxZ-JOSXnebd-arWve2d-arcC1a";
            public const string SubjectRequiredError = "BjS4aVLZ-5jzEOroZ-arWve0O-arcC10";
            public const string UserNotFoundError = "BjS4aVLZ-9jaZGDJY-arWvRpy-arcCZQ";
            public const string lblUserDropdown = "BjS4aVLZ-#vDZny69-arWve2u-arcCZw";
            public const string lblDate = "BjS4aVLZ-#OHedrsP-arWvefh-arcCZm";
            public const string lblSubject = "BjS4aVLZ-#6S0dVKv-arWve2q-arcCZ6";
            public const string BackButtonText = "BjS4aVLZ-rjq4HDXn-arWve2d-arcCZp";
            public const string QuestionAnswerTextLengthExceededError = "BjS4aVLZ-5AbcGgo1-arWvMpa-arcCZf";
            public const string PleaseSelectOptionText = "BjS4aVLZ-FU66Gcb9-arWveaO-arcCZ2";
            public const string InvalidAnswerError = "BjS4aVLZ-CLt4WV6a-arWvRfh-arcCZ9";
            public const string lblActivity = "BjS4aVLZ-#cRk6Vgv-arWve2u-arcCZa";
            public const string MandatoryQuestionError = "BjS4aVLZ-rAz4OSz4-arWvRfu-arcCZ0";
            public const string OtherText = "BjS4aVLZ-cOEMdxLp-arWvefa-arcCsQ";
            public const string PleaseSelectAValidUserError = "BjS4aVLZ-FU66nZsC-arWve0r-arcCsw";
            public const string SaveActivity = "BjS4aVLZ-rjEw6gHL-arWve23-arcCsm";
            public const string SpecifyText = "BjS4aVLZ-uPEsG8tf-arWve9O-arcCs6";
            public const string AllActivitiesText = "rOxfXOxB-Fcj4UgoB-arWve2h-arcCsf";
            public const string AllAgentsText = "rOxfXOxB-F0xoW7sQ-arWvefa-arcCs2";
            public const string SubjectTextLabel = "rOxfXOxB-5jEhLgxv-arWve2y-arcCs9";
            public const string ActivityLabel = "rOxfXOxB-jjC4dOLK-arWvefr-arcCsa";
            public const string UserLabel = "rOxfXOxB-9nb4nY0C-arWvefO-arcCs0";
            public const string StatusLabel = "rOxfXOxB-cZSsa8TK-arWvefu-arcC0Q";
            public const string DateLabel = "rOxfXOxB-rnv6QYxP-arWvefO-arcC0w";
            public const string CreateButtonText = "rOxfXOxB-WpprQzSB-arWve9q-arcC0m";
            public const string CustomerHeaderText = "rOxfXOxB-5OvZHDxB-arWve2d-arcC06";
            public const string UserHeaderText = "rOxfXOxB-9Wq4GInG-arWvefy-arcC0p";
            public const string DateHeaderText = "rOxfXOxB-rWq4GINB-arWvefy-arcC0f";
            public const string SubjectHeaderText = "rOxfXOxB-5jK4aLoB-arWve23-arcC02";
            public const string ActionsHeaderText = "rOxfXOxB-j8K4dvoB-arWve23-arcC09";
            public const string AnyStatusText = "rOxfXOxB-Cjxod7tK-arWve2O-arcC0a";
            public const string StatusHeaderText = "rOxfXOxB-cfxQdRSB-arWvefa-arcC00";
            public const string CreateActivityWarningText = "rOxfXOxB-WuOEWIxT-arWvRmr-arcCCQ";
            public const string SuccessfullyAddedActivity = "rOxfXOxB-58SwddKL-arWve0d-arcCCw";
            public const string SuccessfullyUpdatedActivity = "rOxfXOxB-58SwzdfY-arWve08-arcCCm";
            public const string SearchButtonText = "rOxfXOxB-QpprOzSB-arWve9q-arcCC6";
            public const string SelectAnActivityText = "rOxfXOxB-QuEEdVHK-arWve08-arcCCp";
            public const string DataTablesLengthMenu = "rOxfXOxB-rn24Hgo4-arWvRpa-arcCCf";
            public const string DataTablesZeroRecords = "rOxfXOxB-rnJ42Os4-arWve9u-arcCC2";
            public const string DataTablesInfo = "rOxfXOxB-rnArQIN4-arWvRpy-arcCC9";
            public const string DataTablesInfoEmpty = "rOxfXOxB-rnArfO09-arWve9h-arcCCa";
            public const string DataTablesInfoFiltered = "rOxfXOxB-rnArOyW4-arWvRm8-arcCC0";
            public const string DataTablesPreviousPage = "rOxfXOxB-rn3QQyW4-arWve28-arcneQ";
            public const string DataTablesNextPage = "rOxfXOxB-rnw4Q7ca-arWvefa-arcnew";
            public const string SuccessfullyDeletedActivityText = "rOxfXOxB-58Swzdo9-arWvRQ3-arcnem";
            public const string DatePickerClearDateText = "rOxfXOxB-rErkFggK-arWve9O-arcne6";
            public const string ErrorUpdatingActivity = "WAF#GxYV-W5jrdxJX-arWvRmy-arcnep";
            public const string ErrorCreatingActivity = "WAF#GxYV-WvjrdxoX-arWvRmy-arcnef"; 
            public const string LOCAL_ROOT_DIRECTORY = "jcE6digB-x311leAR-arWvRmr-arojR9";
            public const string REMOTE_ROOT_DIRECTORY = "jcE6digB-6HqfcZEp-arWve9a-arojRa";
            public const string MAX_FILE_UPLOAD_SIZE = "jcE6digB-pZ3hKPPz-arWvef8-arojR0";
            public const string ALLOWABLE_FILE_TYPES = "jcE6digB-Kpkp9Y21-arWve9a-arojMQ";
            public const string ACTIVE = "jcE6digB-0u8LvnCz-arWvepO-arojMw";

            public DMActivities(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {
                //Attributes
                MConfigs.Add(ShowDateColumn, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(ShowStatusColumn, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(ShowSubjectColumn, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                MConfigs.Add(ShowUserColumn, DataType.BOOL, DisplayTabs.TabHeaderAttributes);
                //Buttons
                MConfigs.Add(BackButtonText, DataType.TEXT, DisplayTabs.TabHeaderButtons);
                MConfigs.Add(CreateButtonText, DataType.TEXT, DisplayTabs.TabHeaderButtons);
                MConfigs.Add(SearchButtonText, DataType.TEXT, DisplayTabs.TabHeaderButtons);
                MConfigs.Add(SaveActivity, DataType.TEXT, DisplayTabs.TabHeaderButtons);
                //Messages
                MConfigs.Add(CreateActivityWarningText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.UserMessages);
                MConfigs.Add(SuccessfullyAddedActivity, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.UserMessages);
                MConfigs.Add(SuccessfullyUpdatedActivity, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.UserMessages);
                MConfigs.Add(SuccessfullyDeletedActivityText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.UserMessages);
                //General Labels
                MConfigs.Add(lblUserDropdown, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(lblDate, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(lblSubject, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(PleaseSelectOptionText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(lblActivity, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(OtherText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(SpecifyText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(AllActivitiesText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(AllAgentsText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(SubjectTextLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(ActivityLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(UserLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(StatusLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(DateLabel, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(AnyStatusText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(SelectAnActivityText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                MConfigs.Add(DatePickerClearDateText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.General);
                //Tables
                MConfigs.Add(DataTablesLengthMenu, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.TableLabels);
                MConfigs.Add(DataTablesZeroRecords, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.TableLabels);
                MConfigs.Add(DataTablesInfo, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.TableLabels);
                MConfigs.Add(DataTablesInfoEmpty, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.TableLabels);
                MConfigs.Add(DataTablesInfoFiltered, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.TableLabels);
                MConfigs.Add(DataTablesPreviousPage, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.TableLabels);
                MConfigs.Add(DataTablesNextPage, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.TableLabels);
                MConfigs.Add(CustomerHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.TableLabels);
                MConfigs.Add(UserHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.TableLabels);
                MConfigs.Add(DateHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.TableLabels);
                MConfigs.Add(SubjectHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.TableLabels);
                MConfigs.Add(ActionsHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.TableLabels);
                MConfigs.Add(StatusHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTexts, null, AccordionGroup.TableLabels);
                //Error
                MConfigs.Add(ErrorUpdatingActivity, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(ErrorCreatingActivity, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(SubjectRequiredError, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(UserNotFoundError, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(QuestionAnswerTextLengthExceededError, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(InvalidAnswerError, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(MandatoryQuestionError, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                MConfigs.Add(PleaseSelectAValidUserError, DataType.TEXT, DisplayTabs.TabHeaderErrors);
                //Defaults
                MConfigs.Add(LOCAL_ROOT_DIRECTORY, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> { VG.Mandatory }));
                MConfigs.Add(REMOTE_ROOT_DIRECTORY, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> { VG.Mandatory }));
                MConfigs.Add(MAX_FILE_UPLOAD_SIZE, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> { VG.Mandatory }, minLength: 0));
                MConfigs.Add(ALLOWABLE_FILE_TYPES, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> { VG.Mandatory }));
                MConfigs.Add(ACTIVE, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                Populate();
            }
        }
    }
}