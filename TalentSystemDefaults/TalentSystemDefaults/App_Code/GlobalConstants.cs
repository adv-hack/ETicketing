using System;
namespace TalentSystemDefaults
{
	/// <summary>
	/// Global class for constant values and enum
	/// </summary>
	/// <remarks></remarks>
	[Serializable()]
	public enum DestinationDatabase
	{
		SQL2005,
		TALENTTKT,
		TALENT_ADMIN,
		TALENT_DEFINITION,
		TALENT_CONFIG
	}
	[Serializable()]
	public class GlobalConstants
	{
		//Namespaces
		public const string NS_DATAACCESS_DATAOBJECTS = "TalentSystemDefaults.DataAccess.DataObjects";
		public const string NS_DATAACCESS_MODULEOBJECTS = "TalentSystemDefaults.DataAccess.ModuleObjects";
		public const string NS_DATAACCESS_CONFIGOBJECTS = "TalentSystemDefaults.DataAccess.ConfigObjects";
		public const string NS_DATAENTITIES = "TalentSystemDefaults.DataEntities";
		public const string NS_TALENTMODULES = "TalentSystemDefaults.TalentModules";
		public const string NS_TALENTLISTS = "TalentSystemDefaults.TalentLists";
		//classname suffix
		public const string CLASSNAME_SUFFIX_TALENTMODULES = "DM";
		public const string CLASSNAME_SUFFIX_TALENTLISTS = "DL";
		//Web Source Code
		public const string SOURCE = "W";
		public const string SOURCESUPPLYNET = "S";
		//Basket Types
		public const string TICKETINGBASKETCONTENTTYPE = "T";
		public const string MERCHANDISEBASKETCONTENTTYPE = "M";
		public const string COMBINEDBASKETCONTENTTYPE = "C";
		public const string BASKETMODULETICKETING = "Ticketing";
		public const string BASKETMODULEMERCHANDISE = "";
		public const string BASKETMODULEALL = "*ALL";
		//Payment Types/Modes
		public const string CCPAYMENTTYPE = "CC";
		public const string SAVEDCARDPAYMENTTYPE = "SC";
		public const string DDPAYMENTTYPE = "DD";
		public const string CQPAYMENTTYPE = "CQ";
		public const string PAYPALPAYMENTTYPE = "PP";
		public const string CBPAYMENTTYPE = "CB";
		public const string EPURSEPAYMENTTYPE = "EP";
		public const string CFPAYMENTTYPE = "CF";
		public const string CSPAYMENTTYPE = "CS";
		public const string SAVECARDUSEMODE = "U";
		public const string SAVECARDSAVEMODE = "S";
		public const string CHIPANDPINPAYMENTTYPE = "CP";
		public const string ZEROPRICEDBASKETPAYMENTTYPE = "ZB";
		public const string OAPAYMENTTYPE = "OA";
		public const string POINTOFSALEPAYMENTTYPE = "PS";
		public const string GOOGLECHECKOUTPAYMENTTYPE = "GC";
		public const string PDQPAYMENTTYPE = "PD";
		public const string ECENTRICGATEWAY = "ECENTRIC";
		public const string OTHERSPAYMENTTYPE = "OT";
		public const string CARD_DDL_INITIAL_VALUE = " -- ";
		public const string PAYMENTTYPE_VANGUARD = "VG";
		public const string PAYMENTGATEWAY_VANGUARD = "VANGUARD";
		//may be phase 2 vanguard
		//Public Const PAYMENTGATEWAY_VANGUARD_MULTI = "VANGUARD-MULTI-ACCOUNT-ID"
		//Checkout Stages
		public const string PPS1STAGE = "CHECKOUTSTAGE-PPS1";
		public const string PPS2STAGE = "CHECKOUTSTAGE-PPS2";
		public const string CHECKOUTASPXSTAGE = "CHECKOUTSTAGE-CHECKOUT.ASPX";
		//Product Types
		public const string HOMEPRODUCTTYPE = "H";
		public const string AWAYPRODUCTTYPE = "A";
		public const string EVENTPRODUCTTYPE = "E";
		public const string TRAVELPRODUCTTYPE = "T";
		public const string PPSPRODUCTTYPE = "P";
		public const string SEASONTICKETPRODUCTTYPE = "S";
		public const string CORPORATEPRODUCTTYPE = "C";
		public const string TICKETINGPRODUCTTYPE = "T";
		public const string MEMBERSHIPPRODUCTTYPE = "C";
		public const string PPSTYPE1 = "1";
		public const string PPSTYPE2 = "2";
		public const string PACKAGEPRODUCTTYPE = "P";
		//Package Types
		public const string MATCHDAYPACKAGETYPE = "T";
		//Error Codes
		public const string ERRORFLAG = "E";
		public const string SUCCESSFLAG = "S";
		public const string ORPHANSEATERROR = "OS";
		public const string NOFANIDSFOUND = "NOFANIDSFOUND";
		public const string FANIDUPDATED = "FANIDUPDATED";
		public const string FANIDNOTUPDATED = "FANIDNOTUPDATED";
		public const string MULTIPLESMARTCARDS = "MULTIPLESMARTCARDS";
		public const string ERRORWHENADDINGMULTIPLEPRODUCTS = "ERRORWHENADDINGMULTIPLEPRODUCTS";
		public const string ECENTRICSUCCESSCODE1 = "00";
		public const string ECENTRICSUCCESSCODE2 = "0";
		public const string ECENTRICGENERICERROR = "ECENTRICGENERICERROR";
		//Fee codes
		public const string BBFEE = "BBFEE";
		public const string BKFEE = "BKFEE";
		public const string WSFEE = "WSFEE";
		public const string ATFEE = "ATFEE";
		public const string DDFEE = "DDFEE";
		public const string CRFEE = "CRFEE";
		public const string CFFEE = "CFFEE";
		public const string ALLFEES = "*ALL";
		//Destination DB Types
		public const string TICKETING_DATABASE = "TALENTTKT";
		public const string SQL2005DESTINATIONDATABASE = "SQL2005";
		public const string TALENTADMINDESTINATIONDATABASE = "TalentAdmin";
		public const string TALENTDEFINITIONSDATABASE = "TalentDefinitions";
		//Destination DB Access Module Names
		public const string DBACCESS_SQL = "SQLAccess";
		public const string DBACCESS_TALENT_ADMIN = "TalentAdminAccess";
		public const string DBACCESS_TALENT_DEFINITIONS = "TalentDefinitionsAccess";
		public const string DBACCESS_TALENT_CONFIGURATION = "TalentConfigAccess";
		public const string DBACCESS_TALENT_TICKETING = "TalentTKTAccess";
		//Business Unit/Partner specific
		public const string MAINTENANCEBUSINESSUNIT = "MAINTENANCE";
		public const string STARALLPARTNER = "*ALL";
		//Basket Modes for CAT
		public const string CATMODE_CANCELALL = "CA";
		public const string CATMODE_CANCEL = "C";
		public const string CATMODE_AMEND = "A";
		public const string CATMODE_TRANSFER = "T";
		//Email Templates
		public const string EMAIL_CATMODE_CANCEL = "TicketingCancel";
		public const string EMAIL_CATMODE_TRANSFER = "TicketingTransfer";
		public const string EMAIL_CATMODE_AMEND = "TicketingUpgrade";
		//Email Templates Types
		public const string GENERIC_CUSTOMER_NUMBER = "000000000000";
		public const string EMAIL_ORDER_CONFIRMATION = "TicketingConfirmation";
		public const string EMAIL_TICKETING_UPGRADE = "TicketingUpgrade";
		public const string EMAIL_TICKETING_TRANSFER = "TicketingTransfer";
		public const string EMAIL_TICKETING_CANCEL = "TicketingCancel";
		public const string EMAIL_FORGOTTEN_PASSWORD = "ForgottenPassword";
		public const string EMAIL_CHANGE_PASSWORD = "ChangePassword";
		public const string EMAIL_CUSTOMER_REGISTRATION = "CustomerRegistration";
		public const string EMAIL_ORDER_RETURN_CONFIRM = "OrderReturnConfirm";
		public const string EMAIL_ORDER_RETURN_CONFIRM_REBOOK = "OrderReturnConfirmRebook";
		public const string EMAIL_PPS_PAYMENT_CONFIRMATION = "PPSPaymentConfirmation";
		public const string EMAIL_PPS_PAYMENT_FAILURE = "PPSPaymentFailure";
		public const string EMAIL_PPS_AMEND = "AmendPPS";
		public const string EMAIL_PPS_AMEND_PAYMENT = "AmendPPSPaymentUpdate";
		public const string EMAIL_RETAIL_ORDER_CONFIRMATION = "RetailOrderConfirmation";
		public const string EMAIL_CONTACT_US = "ContactUs";
		//Product to Product Linking
		public const string TICKETINGTYPE = "T";
		public const string RETAILTYPE = "R";
		public const string PRODUCTLINKTYPEBOTH = "0";
		public const string PRODUCTLINKTYPEPHASE1ONLY = "1";
		public const string PRODUCTLINKTYPEPHASE2ONLY = "2";
		public const string PRODUCTLINKTYPEPHASE3ONLY = "3";
		//Stadium/Stand/Area constants
		public const string STARALLSTAND = "*AL";
		public const string STARALLAREA = "*ALL";
		//Alert constants
		public const string ALERT_ACTION_INFO = "Info";
		public const string ALERT_ACTION_HYPERLINK = "Link";
		public const string ALERT_ACTION_PAGE_RESTRICT = "PageRestrict";
		public const string ALERT_ACTION_REDIRECT = "Redirect";
		public const string ALERT_ACTION_SENDEMAIL = "SendEmail";
		public const string ALERT_ACTION_TEXTTICKET = "TextTicket";
		//Page Name constants
		public const string BASKET_PAGE_NAME = "Basket.aspx";
		public const string CHECKOUT_ORDER_SUMMARY_PAGE_NAME = "checkoutOrderSummary.aspx";
		public const string CHECKOUT_PAGE_NAME = "Checkout.aspx";
		public const string FEE_BOOKING_CARDTYPE_EMPTY = "NOCARDTYPE";
		//Fee Category
		public const string FEECATEGORY_ADHOC = "ADHOC";
		public const string FEECATEGORY_AMEND = "AMEND";
		public const string FEECATEGORY_BOOKING = "BOOKING";
		public const string FEECATEGORY_BUYBACK = "BUYBACK";
		public const string FEECATEGORY_CANCEL = "CANCEL";
		public const string FEECATEGORY_CHARITY = "CHARITY";
		public const string FEECATEGORY_DIRECTDEBIT = "DIRECTDEBT";
		public const string FEECATEGORY_FINANCE = "FINANCE";
		public const string FEECATEGORY_FULFILMENT = "FULFILMENT";
		public const string FEECATEGORY_SUPPLYNET = "SUPPLYNET";
		public const string FEECATEGORY_TRANSFER = "TRANSFER";
		public const string FEECATEGORY_VARIABLE = "VARIABLE";
		public const string FEECATEGORY_WEBSALES = "WEBSALES";
		//Fee Charge Type
		public const string FEECHARGETYPE_FIXED = "F";
		public const string FEECHARGETYPE_PERCENTAGE = "P";
		//Fee Basket Type
		public const string FEEAPPLYTO_TICKETING = "T";
		public const string FEEAPPLYTO_MERCHANDISE = "R";
		public const string FEEAPPLYTO_BOTH = "B";
		//Fee Type
		public const string FEETYPE_TRANSACTION = "1";
		public const string FEETYPE_TICKET = "2";
		public const string FEETYPE_PRODUCT = "3";
		//Fee Apply Type
		public const string FEEAPPLYTYPE_BOOKING = "1";
		public const string FEEAPPLYTYPE_FULFILMENT = "2";
		public const string FEEAPPLYTYPE_DIRECTDEBIT = "3";
		public const string FEEAPPLYTYPE_FINANCE = "4";
		public const string FEEAPPLYTYPE_PAYMENTTYPE = "5";
		public const string FEEAPPLYTYPE_CONSIDERCATDETAILS = "6";
		public const string FEEAPPLYTYPE_PARTPAYMENT = "7";
		public const string FEESEXCLUDED_ALL = "*ALL";
		//Fee Type to apply on CAT by Status
		public const string FEE_CONSIDER_CAT_STATUS_STANDARD = "STANDARD";
		public const string FEE_CONSIDER_CAT_ALL_TYPE = "*ALL";
		public const string FEE_CONSIDER_CAT_TRAN_TYPE = "TRAN_TYPE";
		public const string FEE_CONSIDER_CAT_PDT_TYPE = "PROD_TYPE";
		public const string FEE_CONSIDER_CAT_TKT_TYPE = "TKT_TYPE";
		public const string FEE_CONSIDER_CAT_BOTH_TRAN_PROD_TYPE = "TRAN_PROD_TYPE";
		public const string FEE_CONSIDER_CAT_BOTH_TRAN_TKT_TYPE = "TRAN_TKT_TYPE";
		public const string FEE_CONSIDER_CAT_BOTH_PROD_TKT_TYPE = "PROD_TKT_TYPE";
		public const string FEE_PARTPAYMENTFLAG_CHARGE_ALL = "CHARGE_ALL";
		public const string FEE_PARTPAYMENTFLAG_HIGHEST_ONLY = "HIGHEST_ONLY";
		public const string FEE_PARTPAYMENTFLAG_FIRST_ONLY = "FIRST_ONLY";
		//basket summary
		public const string BSKTSMRY_BOOKING_FEE_ACTUAL = "BKFEE_ACTUAL";
		public const string BSKTSMRY_TOTAL_BASKET = "TOTAL_BASKET";
		public const string BSKTSMRY_TOTAL_BASKET_WO_PAYFEE = "TOTAL_BASKET_WO_PAYFEE";
		public const string BSKTSMRY_TOTAL_TICKETING = "TOTAL_TICKETING";
		public const string BSKTSMRY_TOTAL_TICKET_PRICE = "TOTAL_TICKET_PRICE";
		public const string BSKTSMRY_TOTAL_TICKET_DISCOUNT = "TOTAL_TICKET_DISCOUNT";
		public const string BSKTSMRY_TOTAL_TICKET_FEES = "TOTAL_TICKET_FEES";
		public const string BSKTSMRY_TOTAL_CAT_TICKET_PRICE = "TOTAL_CAT_TICKET_PRICE";
		public const string BSKTSMRY_TOTAL_PART_PAYMENTS = "TOTAL_PART_PAYMENTS";
		public const string BSKTSMRY_TOTAL_PART_PAYMENTS_BY_CREDITCARD = "TOTAL_PART_PAYMENTS_BY_CREDITCARD";
		public const string BSKTSMRY_TOTAL_PART_PAYMENTS_BY_OTHERS = "TOTAL_PART_PAYMENTS_BY_OTHERS";
		public const string BSKTSMRY_TOTAL_MERCHANDISE = "TOTAL_MERCHANDISE";
		public const string BSKTSMRY_TOTAL_MERCHANDISE_VAT = "TOTAL_MERCHANDISE_VAT";
		public const string BSKTSMRY_TOTAL_MERCHANDISE_ITEMS_PRICE = "TOTAL_MERCHANDISE_ITEMS_PRICE";
		public const string BSKTSMRY_TOTAL_MERCHANDISE_DELIVERY_PRICE = "TOTAL_MERCHANDISE_DELIVERY_PRICE";
		public const string BSKTSMRY_TOTAL_MERCHANDISE_PROMOTIONS_PRICE = "TOTAL_MERCHANDISE_PROMOTIONS_PRICE";
		public const string BSKTSMRY_TOTAL_ITEMS_MERCHANDISE = "TOTAL_MERCHANDISE_ITEMS";
		public const string BSKTSMRY_TOTAL_ITEMS_TICKETING = "TOTAL_TICKETING_ITEMS";
		public const string BSKTSMRY_TOTAL_ITEMS_APPLIED_CHRTY = "TOTAL_APP_CHARITY_ITEMS";
		public const string BSKTSMRY_TOTAL_ITEMS_APPLIED_ADHOC = "TOTAL_APP_ADHOC_ITEMS";
		public const string BSKTSMRY_TOTAL_ITEMS_APPLIED_VRBLE = "TOTAL_APP_VARIABLE_ITEMS";
		public const string BSKTSMRY_TOTAL_BUYBACK = "TOTAL_BUYBACK";
		public const string BSKTSMRY_TOTAL_ST_EXCEPTIONS_PRICE = "TOTAL_ST_EXCEPTIONS_PRICE";
		public const int BSKTSMRY_DSPLY_SEQ_DEFAULT = 10000;
		// 0 - 7000 ticketing summary
		public const int BSKTSMRY_DSPLY_SEQ_TOTAL_TICKET_PRICE = 20;
		public const int BSKTSMRY_DSPLY_SEQ_TOTAL_TICKET_DISCOUNT = 25;
		public const int BSKTSMRY_DSPLY_SEQ_TOTAL_CAT_TICKET_PRICE = 40;
		public const int BSKTSMRY_DSPLY_SEQ_TOTAL_PART_PAYMENTS = 60;
		public const int BSKTSMRY_DSPLY_SEQ_ONACCOUNT = 80;
		public const int BSKTSMRY_DSPLY_SEQ_BUYBACK = 100;
		public const int BSKTSMRY_DSPLY_SEQ_ST_EXCEPTIONS_PRICE = 110;
		public const int BSKTSMRY_DSPLY_SEQ_FEES_DEFAULT = 120;
		public const int BSKTSMRY_DSPLY_SEQ_BOOKING = 140;
		public const int BSKTSMRY_DSPLY_SEQ_DIRECTDEBIT = 160;
		public const int BSKTSMRY_DSPLY_SEQ_FINANCE = 180;
		public const int BSKTSMRY_DSPLY_SEQ_TOTAL_CHARITY = 200;
		public const int BSKTSMRY_DSPLY_SEQ_TOTAL_ADHOC = 220;
		public const int BSKTSMRY_DSPLY_SEQ_TOTAL_VARIABLE = 240;
		public const int BSKTSMRY_DSPLY_SEQ_WEBSALES = 260;
		// 7020 - 9000 merchandise summary
		public const int BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_ITEMS = 7020;
		public const int BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_PROMOTIONS = 7040;
		public const int BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_DELIVERY = 7060;
		public const int BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_VAT = 7080;
		public const int BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE = 9000;
		//Negative Sale Basket
		public const string BSKTSMRY_ERR_CODE_NSB = "NEGATIVE_SALE_BASKET";
		//Change in basket total affected allowed payment options
		public const string BSKTSMRY_ERR_CODE_BTC = "BASKET_TOTAL_CHANGED_AFFECTS_PAYOPTIONS";
		//Agent
		public const string AGENT_SELL_AVAILABLE = "SELL_AVAILABLE";
		public const string AGENT_WRKSTN = "*WRKSTN";
		public const string AGENT_PERMISSIONS_CACHEKEY_PREFIX = "AgentPermissionsGroup";
		public const string AGENT_PROFILE_CACHEKEY = "AGENTPROFILE";
		public const string AGENT_AUTHORITY_SYSTEM_TYPE = "SYSTEM";
		public const string AGENT_AUTHORITY_CUSTOM_TYPE = "CUSTOM";
		//Product Question types
		public const int FREE_TEXT_FIELD = 0;
		public const int CHECKBOX = 1;
		public const int QUESTION_DATE = 2;
		public const int LIST_OF_ANSWERS = 3;
		//Promotions
		public const string TICKETING_PROMOTION_TYPE = "TICKETING";
		public const string RETAIL_PROMOTION_TYPE = "RETAIL";
		//Generic Constants
		public const string LEADING_ZEROS = "0";
		public const string STAR_ALL = "*ALL";
		//Package and Components
		public const string COMPONENT_TYPE_AVAILABILITY = "A";
		public const string SEAT_USE_TYPE_CAR = "C";
		public const string SEAT_USE_TYPE_TRAVEL = "T";
		public const string SEAT_USE_TYPE_PEOPLE = "P";
		//Fulfilment Types
		public const string POST_FULFILMENT = "P";
		public const string PRINT_FULFILMENT = "N";
		public const string COLLECT_FULFILMENT = "C";
		public const string REG_POST_FULFILMENT = "R";
		public const string PRINT_AT_HOME_FULFILMENT = "H";
		public const string SMARTCARD_UPLOAD_FULFILMENT = "0";
		//Tracking Code Tracking Type
		public const int TRACKING_TYPE_HEADER = 0;
		public const int TRACKING_TYPE_ROW = 1;
		public const int TRACKING_TYPE_FOOTER = 2;
		//Basket Error Codes
		public const string BASKET_ERROR_FAMILY_AREA_RESTRICTION = "A";
		public const string BASKET_ERROR_PRODUCT_PRE_REQ = "M";
		public const string BASKET_ERROR_PRODUCT_PRE_REQ_FAMILY_AREA = "2";
		public const string BASKET_ERROR_MAX_TICKET_LIMIT = "U";
		public const string BASKET_ERROR_TRANSACTION_LIMIT = "X";
		public const string BASKET_ERROR_PRODUCT_LIMIT_PER_TRANSACTION = "R";
		public const string BASKET_ERROR_MAX_LIMIT_OF_MEMBERSHIPS_PER_TRANSACTION = "B";
		public const string BASKET_ERROR_STAND_LEVEL_MAX_TICKET_LIMIT = "1";
		public const string BASKET_ERROR_MAX_LIMIT_FREE_SEATS = "Z";
		//Season Ticket Exception Change/Remove seat mode
		public const string ST_EXCCEPTION_CHANGE_SEAT = "C";
		public const string ST_EXCCEPTION_REMOVE_SEAT = "R";
		public const string ST_EXCCEPTION_CHOOSE_SEAT = "U";
		public const string ST_EXCCEPTION_UNALLOCATED_SEAT = "UNALLOCATED";
		public const string LOG_ERROR_CODE = "SDGlobal001";
	}
}
