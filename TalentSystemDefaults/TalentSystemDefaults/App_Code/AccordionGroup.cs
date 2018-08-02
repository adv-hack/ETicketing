using System.Collections.Generic;
namespace TalentSystemDefaults
{
	public class AccordionGroup
	{
        //Generic
        public const string GeneralMessages = "AccordionUserMessages";
        public const string GeneralDataTable = "AccordionDataTable";
		//Registration
		public const string GeneralKey = "AccordionGeneral";
		public const string PersonalKey = "AccordionPersonal";
		public const string AddressKey = "AccordionAddress";
		public const string PhoneNumberKey = "AccordionPhoneNumber";
		public const string LoginKey = "AccordionLogin";
		public const string CustomerKey = "AccordionCustomer";
		public const string CustomerIDsKey = "AccordionCustomerIDs";
		public const string EmailKey = "AccordionEmail";
		//Season Ticket Exception
		public const string BasketKey = "AccordionBasket";
		public const string SeatSelectionKey = "AccordionSeatSelection";
		public const string ExceptionPageColumnHeadingsKey = "AccordionExceptionPageColumnHeadings";
		public const string ExceptionPageTextKey = "AccordionExceptionPageText";
		//Activities
		public const string ProfileActivitiesKey = "AccordionProfileActivities";
		public const string EditProfileKey = "AccordionEditProfile";
		private Dictionary<string, string> pageTexts;
		public AccordionGroup(Dictionary<string, string> pageTexts)
		{
			this.pageTexts = pageTexts;
		}
        public string TableLabels
        {
            get
            {
                return ((pageTexts.ContainsKey(GeneralDataTable)) ? (pageTexts[GeneralDataTable]) : "Table of results");
            }
        }
        public string UserMessages
        {
            get
            {
                return ((pageTexts.ContainsKey(GeneralMessages)) ? (pageTexts[GeneralMessages]) : "User Messages");
            }
        }
		public string General
		{
			get
			{
				return ((pageTexts.ContainsKey(GeneralKey)) ? (pageTexts[GeneralKey]) : "General");
			}
		}
		public string Personal
		{
			get
			{
				return ((pageTexts.ContainsKey(PersonalKey)) ? (pageTexts[PersonalKey]) : "Personal");
			}
		}
		public string Address
		{
			get
			{
				return ((pageTexts.ContainsKey(AddressKey)) ? (pageTexts[AddressKey]) : "Address");
			}
		}
		public string PhoneNumber
		{
			get
			{
				return ((pageTexts.ContainsKey(PhoneNumberKey)) ? (pageTexts[PhoneNumberKey]) : "Phone Number");
			}
		}
		public string Login
		{
			get
			{
				return ((pageTexts.ContainsKey(LoginKey)) ? (pageTexts[LoginKey]) : "Login");
			}
		}
		public string Customer
		{
			get
			{
				return ((pageTexts.ContainsKey(CustomerKey)) ? (pageTexts[CustomerKey]) : "Customer");
			}
		}
		public string CustomerIDs
		{
			get
			{
				return ((pageTexts.ContainsKey(CustomerIDsKey)) ? (pageTexts[CustomerIDsKey]) : "Customer IDs");
			}
		}
		public string Email
		{
			get
			{
				return ((pageTexts.ContainsKey(EmailKey)) ? (pageTexts[EmailKey]) : "Email");
			}
		}
		public string Basket
		{
			get
			{
				return ((pageTexts.ContainsKey(BasketKey)) ? (pageTexts[BasketKey]) : "Basket");
			}
		}
		public string SeatSelection
		{
			get
			{
				return ((pageTexts.ContainsKey(SeatSelectionKey)) ? (pageTexts[SeatSelectionKey]) : "Seat Selection");
			}
		}
		public string ExceptionPageColumnHeadings
		{
			get
			{
				return ((pageTexts.ContainsKey(ExceptionPageColumnHeadingsKey)) ? (pageTexts[ExceptionPageColumnHeadingsKey]) : "Exception Page Column Headings");
			}
		}
		public string ExceptionPageText
		{
			get
			{
				return ((pageTexts.ContainsKey(ExceptionPageTextKey)) ? (pageTexts[ExceptionPageTextKey]) : "Exception Page Text");
			}
		}
		public string ProfileActivities
		{
			get
			{
				return ((pageTexts.ContainsKey(ProfileActivitiesKey)) ? (pageTexts[ProfileActivitiesKey]) : "Display Profile Activities");
			}
		}
		public string EditProfile
		{
			get
			{
				return ((pageTexts.ContainsKey(EditProfileKey)) ? (pageTexts[EditProfileKey]) : "Edit Profile Activity");
			}
		}
	}
}
