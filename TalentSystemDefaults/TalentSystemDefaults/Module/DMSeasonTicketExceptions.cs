using System.Collections.Generic;
namespace TalentSystemDefaults
{
	namespace TalentModules
	{
		public class DMSeasonTicketExceptions : DMBase
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
			static private string _ModuleTitle = "Season Ticket Exceptions";
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
			public const string FLG45 = "0rrveQOP-KSoHJpue-arWvewa-ar#Ve2";
			public const string SEQ01 = "0rrveQOP-6UWPKQOe-arWvewh-ar#Ve9";
			public const string XF004B = "0rrveQOP-71o5Vpux-arWvewa-ar#Vea";
			public const string SeasonTicketExceptionsLinkText = "BjS4aVLZ-Qmx65YpB-arWvRQ8-ar#Ve0";
			public const string TOTAL_ST_EXCEPTIONS_PRICE = "BjS4aVLZ-xUfWy5HE-arWve98-ar#VRQ";
			public const string BackToExceptionsPage = "BjS4aVLZ-rdD6GLKX-arWvear-ar#VRw";
			public const string ChangeExceptionSeatButton = "BjS4aVLZ-JdD6Dgey-arWve9y-ar#VRm";
			public const string BasketButtonText = "rOxfXOxB-rppr8zSB-arWve28-ar#VR6";
			public const string ChangeSeatText = "rOxfXOxB-JUq4GOQQ-arWve2y-ar#VRp";
			public const string KeepSeatsTogetherButtonText = "rOxfXOxB-QWp2dvoy-arWve0y-ar#VRf";
			public const string ExceptionsColumnHeading = "rOxfXOxB-4cru2JLT-arWvRQr-ar#VR2";
			public const string ExceptionSeatColumnHeading = "rOxfXOxB-4cxePhK3-arWve9a-ar#VR9";
			public const string SeasonTicketSeatColumnHeading = "rOxfXOxB-Qmx6HRYO-arWve0q-ar#VRa";
			public const string GenericExceptionsProcessingError = "rOxfXOxB-QExz5C4L-arWvRf8-ar#VR0";
			public const string ProblemKeepingSeatsTogetherText = "rOxfXOxB-WlDEFIo#-arWvRp3-ar#VMQ";
			public const string ProblemRemovingSeatText = "rOxfXOxB-WlpxFgoK-arWvRmr-ar#VMw";
			public const string NoAvailabilityMessage = "rOxfXOxB-B7SEhDxo-arWvRpq-ar#VMm";
			public const string ExceptionsOnCurrentSeatText = "rOxfXOxB-4cGr9gpK-arWvR9h-ar#VM6";
			public const string KeepSeatsTogetherLabelText = "rOxfXOxB-QWp2a7ov-arWvMm3-ar#VMp";
			public const string NoExceptionSeatsText = "rOxfXOxB-B5asdZgB-arWve9y-ar#VMf";
			public const string PageDetailsText = "rOxfXOxB-rjblfOpE-arWvEfr-ar#VM2";
			public const string PickASeatText = "rOxfXOxB-tOxoG7KB-arWve2a-ar#VM9";
			public const string RemoveSeatText = "rOxfXOxB-QUq4GOEX-arWve2y-ar#VMa";
			public const string SeatDisplayFormatText = "rOxfXOxB-Q8RUBI1K-arWvEwh-ar#VM0";
			public const string UnallocatedSeatText = "rOxfXOxB-CEKsbVo4-arWveaO-ar#VEQ";
			public DMSeasonTicketExceptions(DESettings settings, bool initialiseData)
				: base(ref settings, initialiseData)
			{
			}
			public override void SetModuleConfiguration()
			{
				MConfigs.Add(FLG45, DataType.BOOL_10, DisplayTabs.TabHeaderDefaults);
				MConfigs.Add(SEQ01, DataType.TEXT, DisplayTabs.TabHeaderDefaults, FieldValidation.Add(new List<VG> {VG.MaxValue}, maxValue: 100));
				MConfigs.Add(XF004B, DataType.BOOL_YN, DisplayTabs.TabHeaderDefaults);
				MConfigs.Add(configID: SeasonTicketExceptionsLinkText, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.Basket);
				MConfigs.Add(configID: TOTAL_ST_EXCEPTIONS_PRICE, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.Basket);
				MConfigs.Add(configID: BackToExceptionsPage, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.SeatSelection);
				MConfigs.Add(configID: ChangeExceptionSeatButton, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.SeatSelection);
				MConfigs.Add(BasketButtonText, DataType.TEXT, DisplayTabs.TabHeaderButtons);
				MConfigs.Add(ChangeSeatText, DataType.TEXT, DisplayTabs.TabHeaderButtons);
				MConfigs.Add(KeepSeatsTogetherButtonText, DataType.TEXT, DisplayTabs.TabHeaderButtons);
				MConfigs.Add(configID: ExceptionsColumnHeading, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.ExceptionPageColumnHeadings);
				MConfigs.Add(configID: ExceptionSeatColumnHeading, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.ExceptionPageColumnHeadings);
				MConfigs.Add(configID: SeasonTicketSeatColumnHeading, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.ExceptionPageColumnHeadings);
				MConfigs.Add(GenericExceptionsProcessingError, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(ProblemKeepingSeatsTogetherText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(ProblemRemovingSeatText, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(NoAvailabilityMessage, DataType.TEXT, DisplayTabs.TabHeaderErrors);
				MConfigs.Add(configID: ExceptionsOnCurrentSeatText, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.ExceptionPageText);
				MConfigs.Add(configID: KeepSeatsTogetherLabelText, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.ExceptionPageText);
				MConfigs.Add(configID: NoExceptionSeatsText, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.ExceptionPageText);
				MConfigs.Add(configID: PageDetailsText, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.ExceptionPageText);
				MConfigs.Add(configID: PickASeatText, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.ExceptionPageText);
				MConfigs.Add(configID: RemoveSeatText, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.ExceptionPageText);
				MConfigs.Add(configID: SeatDisplayFormatText, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.ExceptionPageText);
				MConfigs.Add(configID: UnallocatedSeatText, fieldType: DataType.TEXT, displayTab: DisplayTabs.TabHeaderTexts, accordionGroup: AccordionGroup.ExceptionPageText);
				Populate();
			}
		}
	}
}
