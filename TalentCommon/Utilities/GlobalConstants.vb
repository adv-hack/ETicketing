''' <summary>
''' Global class for constant values and enum
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public Enum DestinationDatabase
    SQL2005
    TALENT_ADMIN
    TALENT_DEFINITION
End Enum

<Serializable()> _
Public Class GlobalConstants

    'Web Source Code
    Public Const SOURCE As String = "W"
    Public Const SOURCESUPPLYNET As String = "S"
    Public Const SOURCE_TALENT_API As String = "W"

    'Basket Types
    Public Const TICKETINGBASKETCONTENTTYPE As String = "T"
    Public Const MERCHANDISEBASKETCONTENTTYPE As String = "M"
    Public Const COMBINEDBASKETCONTENTTYPE As String = "C"
    Public Const BASKETMODULETICKETING As String = "Ticketing"
    Public Const BASKETMODULEMERCHANDISE As String = ""
    Public Const BASKETMODULEALL As String = "*ALL"

    'Payment Types/Modes
    Public Const CCPAYMENTTYPE As String = "CC"
    Public Const SAVEDCARDPAYMENTTYPE As String = "SC"
    Public Const DDPAYMENTTYPE As String = "DD"
    Public Const CQPAYMENTTYPE As String = "CQ"
    Public Const PAYPALPAYMENTTYPE As String = "PP"
    Public Const CBPAYMENTTYPE As String = "CB"
    Public Const EPURSEPAYMENTTYPE As String = "EP"
    Public Const CFPAYMENTTYPE As String = "CF"
    Public Const CSPAYMENTTYPE As String = "CS"
    Public Const INVPAYMENTTYPE As String = "IV"
    Public Const SAVECARDUSEMODE As String = "U"
    Public Const SAVECARDSAVEMODE As String = "S"
    Public Const CHIPANDPINPAYMENTTYPE As String = "CP"
    Public Const ZEROPRICEDBASKETPAYMENTTYPE As String = "ZB"
    Public Const OAPAYMENTTYPE As String = "OA"
    Public Const POINTOFSALEPAYMENTTYPE As String = "PS"
    Public Const GOOGLECHECKOUTPAYMENTTYPE As String = "GC"
    Public Const PDQPAYMENTTYPE As String = "PD"
    Public Const ECENTRICGATEWAY As String = "ECENTRIC"
    Public Const OTHERSPAYMENTTYPE As String = "OT"
    Public Const CARD_DDL_INITIAL_VALUE = " -- "

    Public Const PAYMENTTYPE_VANGUARD = "VG"
    Public Const PAYMENTGATEWAY_VANGUARD = "VANGUARD"
    'may be phase 2 vanguard
    'Public Const PAYMENTGATEWAY_VANGUARD_MULTI = "VANGUARD-MULTI-ACCOUNT-ID"

    'Checkout Stages
    Public Const PPS1STAGE As String = "CHECKOUTSTAGE-PPS1"
    Public Const PPS2STAGE As String = "CHECKOUTSTAGE-PPS2"
    Public Const CHECKOUTASPXSTAGE As String = "CHECKOUTSTAGE-CHECKOUT.ASPX"

    'Product Types
    Public Const HOMEPRODUCTTYPE As String = "H"
    Public Const AWAYPRODUCTTYPE As String = "A"
    Public Const EVENTPRODUCTTYPE As String = "E"
    Public Const TRAVELPRODUCTTYPE As String = "T"
    Public Const PPSPRODUCTTYPE As String = "P"
    Public Const SEASONTICKETPRODUCTTYPE As String = "S"
    Public Const CORPORATEPRODUCTTYPE As String = "C"
    Public Const TICKETINGPRODUCTTYPE As String = "T"
    Public Const MEMBERSHIPPRODUCTTYPE As String = "C"
    Public Const PPSTYPE1 As String = "1"
    Public Const PPSTYPE2 As String = "2"
    Public Const PACKAGEPRODUCTTYPE As String = "P"

    'Package Types
    Public Const MATCHDAY_HOSPITALITY_PACKAGE_TYPE As String = "M"
    Public Const PROMOTED_EVENT_PACKAGE_TYPE As String = "T"
    Public Const SEASONAL_PACKAGE_TYPE As String = "S"
    Public Const PACKAGE_PRICING As String = "P"
    Public Const COMPONENT_PRICING As String = "C"
    Public Const TICKETING_PRICING As String = "T"

    'Error Codes
    Public Const ERRORFLAG As String = "E"
    Public Const SUCCESSFLAG As String = "S"
    Public Const ORPHANSEATERROR As String = "OS"
    Public Const NOFANIDSFOUND As String = "NOFANIDSFOUND"
    Public Const FANIDUPDATED As String = "FANIDUPDATED"
    Public Const FANIDNOTUPDATED As String = "FANIDNOTUPDATED"
    Public Const MULTIPLESMARTCARDS As String = "MULTIPLESMARTCARDS"
    Public Const ERRORWHENADDINGMULTIPLEPRODUCTS As String = "ERRORWHENADDINGMULTIPLEPRODUCTS"
    Public Const ECENTRICSUCCESSCODE1 As String = "00"
    Public Const ECENTRICSUCCESSCODE2 As String = "0"
    Public Const ECENTRICGENERICERROR As String = "ECENTRICGENERICERROR"
    Public Const STATUS_RESULTS_TABLE_NAME As String = "StatusResults"

    'Fee codes
    Public Const BBFEE As String = "BBFEE"
    Public Const BKFEE As String = "BKFEE"
    Public Const WSFEE As String = "WSFEE"
    Public Const ATFEE As String = "ATFEE"
    Public Const DDFEE As String = "DDFEE"
    Public Const CRFEE As String = "CRFEE"
    Public Const CFFEE As String = "CFFEE"
    Public Const ALLFEES As String = "*ALL"

    'Destination DB Types
    Public Const TICKETING_DATABASE = "TALENTTKT"
    Public Const SQL2005DESTINATIONDATABASE As String = "SQL2005"
    Public Const TALENTADMINDESTINATIONDATABASE As String = "TalentAdmin"
    Public Const TALENTDEFINITIONSDATABASE As String = "TalentDefinitions"

    'Destination DB Access Module Names
    Public Const DBACCESS_SQL As String = "SQLAccess"
    Public Const DBACCESS_TALENT_ADMIN As String = "TalentAdminAccess"
    Public Const DBACCESS_TALENT_DEFINITIONS As String = "TalentDefinitionsAccess"

    'Business Unit/Partner specific
    Public Const MAINTENANCEBUSINESSUNIT As String = "MAINTENANCE"
    Public Const BOXOFFICEBUSINESSUNIT As String = "BOXOFFICE"
    Public Const STARALLPARTNER As String = "*ALL"

    'Basket Modes for CAT
    Public Const CATMODE_CANCELALL As String = "CA"
    Public Const CATMODE_CANCELMULTIPLE As String = "CM"
    Public Const CATMODE_CANCEL As String = "C"
    Public Const CATMODE_AMEND As String = "A"
    Public Const CATMODE_TRANSFER As String = "T"

    'Email Templates
    Public Const EMAIL_CATMODE_CANCEL As String = "TicketingCancel"
    Public Const EMAIL_CATMODE_TRANSFER As String = "TicketingTransfer"
    Public Const EMAIL_CATMODE_AMEND As String = "TicketingUpgrade"

    'Email Templates Types
    Public Const GENERIC_CUSTOMER_NUMBER As String = "000000000000"
    Public Const EMAIL_ORDER_CONFIRMATION As String = "TicketingConfirmation"
    Public Const EMAIL_TICKETING_UPGRADE As String = "TicketingUpgrade"
    Public Const EMAIL_TICKETING_TRANSFER As String = "TicketingTransfer"
    Public Const EMAIL_TICKETING_CANCEL As String = "TicketingCancel"
    Public Const EMAIL_FORGOTTEN_PASSWORD As String = "ForgottenPassword"
    Public Const EMAIL_CHANGE_PASSWORD As String = "ChangePassword"
    Public Const EMAIL_CUSTOMER_REGISTRATION As String = "CustomerRegistration"
    Public Const EMAIL_ORDER_RETURN_CONFIRM As String = "OrderReturnConfirm"
    Public Const EMAIL_ORDER_RETURN_CONFIRM_REBOOK As String = "OrderReturnConfirmRebook"
    Public Const EMAIL_PPS_PAYMENT_CONFIRMATION As String = "PPSPaymentConfirmation"
    Public Const EMAIL_PPS_PAYMENT_FAILURE As String = "PPSPaymentFailure"
    Public Const EMAIL_PPS_AMEND As String = "AmendPPS"
    Public Const EMAIL_PPS_AMEND_PAYMENT As String = "AmendPPSPaymentUpdate"
    Public Const EMAIL_RETAIL_ORDER_CONFIRMATION As String = "RetailOrderConfirmation"
    Public Const EMAIL_CONTACT_US As String = "ContactUs"
    Public Const EMAIL_TICKET_EXCHANGE_CONFIRM As String = "TicketExchangeConfirm"
    Public Const EMAIL_TICKET_EXCHANGE_SALE_CONFIRM As String = "TicketExchangeSaleConfirm"
    Public Const EMAIL_HOSPITALITY_Q_AND_A_REMINDER As String = "HospitalityQ&AReminder"

    'Product to Product Linking
    Public Const TICKETINGTYPE As String = "T"
    Public Const RETAILTYPE As String = "R"
    Public Const PRODUCTLINKTYPEBOTH As String = "0"
    Public Const PRODUCTLINKTYPEPHASE1ONLY As String = "1"
    Public Const PRODUCTLINKTYPEPHASE2ONLY As String = "2"
    Public Const PRODUCTLINKTYPEPHASE3ONLY As String = "3"

    'Stadium/Stand/Area constants
    Public Const STARALLSTAND As String = "*AL"
    Public Const STARALLAREA As String = "*ALL"

    'Alert constants
    Public Const ALERT_ACTION_INFO As String = "Info"
    Public Const ALERT_ACTION_HYPERLINK As String = "Link"
    Public Const ALERT_ACTION_PAGE_RESTRICT As String = "PageRestrict"
    Public Const ALERT_ACTION_REDIRECT As String = "Redirect"
    Public Const ALERT_ACTION_SENDEMAIL As String = "SendEmail"
    Public Const ALERT_ACTION_TEXTTICKET As String = "TextTicket"

    'Page Name constants
    Public Const BASKET_PAGE_NAME As String = "Basket.aspx"
    Public Const CHECKOUT_ORDER_SUMMARY_PAGE_NAME As String = "checkoutOrderSummary.aspx"
    Public Const CHECKOUT_PAGE_NAME As String = "Checkout.aspx"

    Public Const FEE_BOOKING_CARDTYPE_EMPTY As String = "NOCARDTYPE"
    'Fee Category
    Public Const FEECATEGORY_ADHOC As String = "ADHOC"
    Public Const FEECATEGORY_AMEND As String = "AMEND"
    Public Const FEECATEGORY_BOOKING As String = "BOOKING"
    Public Const FEECATEGORY_BUYBACK As String = "BUYBACK"
    Public Const FEECATEGORY_CANCEL As String = "CANCEL"
    Public Const FEECATEGORY_CHARITY As String = "CHARITY"
    Public Const FEECATEGORY_DIRECTDEBIT As String = "DIRECTDEBT"
    Public Const FEECATEGORY_FINANCE As String = "FINANCE"
    Public Const FEECATEGORY_FULFILMENT As String = "FULFILMENT"
    Public Const FEECATEGORY_SUPPLYNET As String = "SUPPLYNET"
    Public Const FEECATEGORY_TRANSFER As String = "TRANSFER"
    Public Const FEECATEGORY_VARIABLE As String = "VARIABLE"
    Public Const FEECATEGORY_WEBSALES As String = "WEBSALES"

    'Fee Charge Type
    Public Const FEECHARGETYPE_FIXED As String = "F"
    Public Const FEECHARGETYPE_PERCENTAGE As String = "P"

    'Fee Basket Type
    Public Const FEEAPPLYTO_TICKETING As String = "T"
    Public Const FEEAPPLYTO_MERCHANDISE As String = "R"
    Public Const FEEAPPLYTO_BOTH As String = "B"

    'Fee Type
    Public Const FEETYPE_TRANSACTION As String = "1"
    Public Const FEETYPE_TICKET As String = "2"
    Public Const FEETYPE_PRODUCT As String = "3"

    'Fee Apply Type
    Public Const FEEAPPLYTYPE_BOOKING As String = "1"
    Public Const FEEAPPLYTYPE_FULFILMENT As String = "2"
    Public Const FEEAPPLYTYPE_DIRECTDEBIT As String = "3"
    Public Const FEEAPPLYTYPE_FINANCE As String = "4"
    Public Const FEEAPPLYTYPE_PAYMENTTYPE As String = "5"
    Public Const FEEAPPLYTYPE_CONSIDERCATDETAILS As String = "6"
    Public Const FEEAPPLYTYPE_PARTPAYMENT As String = "7"
    Public Const FEESEXCLUDED_ALL As String = "*ALL"

    'Fee Type to apply on CAT by Status
    Public Const FEE_CONSIDER_CAT_STATUS_STANDARD As String = "STANDARD"
    Public Const FEE_CONSIDER_CAT_ALL_TYPE As String = "*ALL"
    Public Const FEE_CONSIDER_CAT_TRAN_TYPE As String = "TRAN_TYPE"
    Public Const FEE_CONSIDER_CAT_PDT_TYPE As String = "PROD_TYPE"
    Public Const FEE_CONSIDER_CAT_TKT_TYPE As String = "TKT_TYPE"
    Public Const FEE_CONSIDER_CAT_BOTH_TRAN_PROD_TYPE As String = "TRAN_PROD_TYPE"
    Public Const FEE_CONSIDER_CAT_BOTH_TRAN_TKT_TYPE As String = "TRAN_TKT_TYPE"
    Public Const FEE_CONSIDER_CAT_BOTH_PROD_TKT_TYPE As String = "PROD_TKT_TYPE"

    Public Const FEE_PARTPAYMENTFLAG_CHARGE_ALL As String = "CHARGE_ALL"
    Public Const FEE_PARTPAYMENTFLAG_HIGHEST_ONLY As String = "HIGHEST_ONLY"
    Public Const FEE_PARTPAYMENTFLAG_FIRST_ONLY As String = "FIRST_ONLY"

    'basket summary
    Public Const BSKTSMRY_BOOKING_FEE_ACTUAL As String = "BKFEE_ACTUAL"
    Public Const BSKTSMRY_TOTAL_BASKET As String = "TOTAL_BASKET"
    Public Const BSKTSMRY_TOTAL_BASKET_WO_PAYFEE As String = "TOTAL_BASKET_WO_PAYFEE"
    Public Const BSKTSMRY_TOTAL_TICKETING As String = "TOTAL_TICKETING"
    Public Const BSKTSMRY_TOTAL_TICKET_PRICE As String = "TOTAL_TICKET_PRICE"
    Public Const BSKTSMRY_TOTAL_TICKET_DISCOUNT As String = "TOTAL_TICKET_DISCOUNT"
    Public Const BSKTSMRY_TOTAL_TICKET_FEES As String = "TOTAL_TICKET_FEES"
    Public Const BSKTSMRY_TOTAL_CAT_TICKET_PRICE As String = "TOTAL_CAT_TICKET_PRICE"
    Public Const BSKTSMRY_TOTAL_PART_PAYMENTS As String = "TOTAL_PART_PAYMENTS"
    Public Const BSKTSMRY_TOTAL_PART_PAYMENTS_BY_CREDITCARD As String = "TOTAL_PART_PAYMENTS_BY_CREDITCARD"
    Public Const BSKTSMRY_TOTAL_PART_PAYMENTS_BY_OTHERS As String = "TOTAL_PART_PAYMENTS_BY_OTHERS"
    Public Const BSKTSMRY_TOTAL_MERCHANDISE As String = "TOTAL_MERCHANDISE"
    Public Const BSKTSMRY_TOTAL_MERCHANDISE_VAT As String = "TOTAL_MERCHANDISE_VAT"
    Public Const BSKTSMRY_TOTAL_MERCHANDISE_ITEMS_PRICE As String = "TOTAL_MERCHANDISE_ITEMS_PRICE"
    Public Const BSKTSMRY_TOTAL_MERCHANDISE_DELIVERY_PRICE As String = "TOTAL_MERCHANDISE_DELIVERY_PRICE"
    Public Const BSKTSMRY_TOTAL_MERCHANDISE_PROMOTIONS_PRICE As String = "TOTAL_MERCHANDISE_PROMOTIONS_PRICE"
    Public Const BSKTSMRY_TOTAL_ITEMS_MERCHANDISE As String = "TOTAL_MERCHANDISE_ITEMS"
    Public Const BSKTSMRY_TOTAL_ITEMS_TICKETING As String = "TOTAL_TICKETING_ITEMS"
    Public Const BSKTSMRY_TOTAL_ITEMS_APPLIED_CHRTY As String = "TOTAL_APP_CHARITY_ITEMS"
    Public Const BSKTSMRY_TOTAL_ITEMS_APPLIED_ADHOC As String = "TOTAL_APP_ADHOC_ITEMS"
    Public Const BSKTSMRY_TOTAL_ITEMS_APPLIED_VRBLE As String = "TOTAL_APP_VARIABLE_ITEMS"
    Public Const BSKTSMRY_TOTAL_BUYBACK As String = "TOTAL_BUYBACK"
    Public Const BSKTSMRY_TOTAL_ST_EXCEPTIONS_PRICE As String = "TOTAL_ST_EXCEPTIONS_PRICE"

    Public Const BSKTSMRY_DSPLY_SEQ_DEFAULT As Integer = 10000
    ' 0 - 7000 ticketing summary
    Public Const BSKTSMRY_DSPLY_SEQ_TOTAL_TICKET_PRICE As Integer = 20
    Public Const BSKTSMRY_DSPLY_SEQ_TOTAL_TICKET_DISCOUNT As Integer = 25
    Public Const BSKTSMRY_DSPLY_SEQ_TOTAL_CAT_TICKET_PRICE As Integer = 40
    Public Const BSKTSMRY_DSPLY_SEQ_TOTAL_PART_PAYMENTS As Integer = 60
    Public Const BSKTSMRY_DSPLY_SEQ_ONACCOUNT As Integer = 80
    Public Const BSKTSMRY_DSPLY_SEQ_BUYBACK As Integer = 100
    Public Const BSKTSMRY_DSPLY_SEQ_ST_EXCEPTIONS_PRICE As Integer = 110
    Public Const BSKTSMRY_DSPLY_SEQ_FEES_DEFAULT As Integer = 120
    Public Const BSKTSMRY_DSPLY_SEQ_BOOKING As Integer = 140
    Public Const BSKTSMRY_DSPLY_SEQ_DIRECTDEBIT As Integer = 160
    Public Const BSKTSMRY_DSPLY_SEQ_FINANCE As Integer = 180
    Public Const BSKTSMRY_DSPLY_SEQ_TOTAL_CHARITY As Integer = 200
    Public Const BSKTSMRY_DSPLY_SEQ_TOTAL_ADHOC As Integer = 220
    Public Const BSKTSMRY_DSPLY_SEQ_TOTAL_VARIABLE As Integer = 240
    Public Const BSKTSMRY_DSPLY_SEQ_WEBSALES As Integer = 260
    ' 7020 - 9000 merchandise summary
    Public Const BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_ITEMS As Integer = 7020
    Public Const BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_PROMOTIONS As Integer = 7040
    Public Const BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_DELIVERY As Integer = 7060
    Public Const BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_VAT As Integer = 7080
    Public Const BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE As Integer = 9000

    'Negative Sale Basket
    Public Const BSKTSMRY_ERR_CODE_NSB As String = "NEGATIVE_SALE_BASKET"
    'Change in basket total affected allowed payment options
    Public Const BSKTSMRY_ERR_CODE_BTC As String = "BASKET_TOTAL_CHANGED_AFFECTS_PAYOPTIONS"

    'Agent
    Public Const AGENT_SELL_AVAILABLE As String = "SELL_AVAILABLE"
    Public Const AGENT_WRKSTN As String = "*WRKSTN"
    Public Const AGENT_PERMISSIONS_CACHEKEY_PREFIX As String = "AgentPermissionsGroup"
    Public Const AGENT_PROFILE_CACHEKEY As String = "AGENTPROFILE"
    Public Const AGENT_AUTHORITY_SYSTEM_TYPE As String = "SYSTEM"
    Public Const AGENT_AUTHORITY_CUSTOM_TYPE As String = "CUSTOM"

    'Product Question types
    Public Const FREE_TEXT_FIELD As Integer = 0
    Public Const CHECKBOX As Integer = 1
    Public Const QUESTION_DATE As Integer = 2
    Public Const LIST_OF_ANSWERS As Integer = 3

    'Promotions
    Public Const TICKETING_PROMOTION_TYPE As String = "TICKETING"
    Public Const RETAIL_PROMOTION_TYPE As String = "RETAIL"

    'Generic Constants
    Public Const LEADING_ZEROS As String = "0"
    Public Const STAR_ALL As String = "*ALL"
    Public Const ECOMMERCE_TYPE As String = "ECOMMERCE"

    'Package and Components
    Public Const COMPONENT_TYPE_AVAILABILITY As String = "A"
    Public Const SEAT_USE_TYPE_CAR As String = "C"
    Public Const SEAT_USE_TYPE_TRAVEL As String = "T"
    Public Const SEAT_USE_TYPE_PEOPLE As String = "P"
    Public Const PACKAGE_CAT_AGENT_ONLY As String = "A"
    Public Const PACKAGE_CAT_ALLOWED As String = "Y"

    'Fulfilment Types
    Public Const POST_FULFILMENT As String = "P"
    Public Const PRINT_FULFILMENT As String = "N"
    Public Const COLLECT_FULFILMENT As String = "C"
    Public Const REG_POST_FULFILMENT As String = "R"
    Public Const PRINT_AT_HOME_FULFILMENT As String = "H"
    Public Const SMARTCARD_UPLOAD_FULFILMENT As String = "0"

    'Tracking Code Tracking Type
    Public Const TRACKING_TYPE_HEADER As Integer = 0
    Public Const TRACKING_TYPE_ROW As Integer = 1
    Public Const TRACKING_TYPE_FOOTER As Integer = 2

    'Basket Error Codes
    Public Const BASKET_ERROR_FAMILY_AREA_RESTRICTION As String = "A"
    Public Const BASKET_ERROR_PRODUCT_PRE_REQ As String = "M"
    Public Const BASKET_ERROR_PRODUCT_PRE_REQ_FAMILY_AREA As String = "2"

    Public Const BASKET_ERROR_MAX_TICKET_LIMIT As String = "U"
    Public Const BASKET_ERROR_TRANSACTION_LIMIT As String = "X"
    Public Const BASKET_ERROR_PRODUCT_LIMIT_PER_TRANSACTION As String = "R"
    Public Const BASKET_ERROR_MAX_LIMIT_OF_MEMBERSHIPS_PER_TRANSACTION As String = "B"
    Public Const BASKET_ERROR_STAND_LEVEL_MAX_TICKET_LIMIT As String = "1"
    Public Const BASKET_ERROR_MAX_LIMIT_FREE_SEATS As String = "Z"
    Public Const BASKET_ERROR_MAX_PACKAGE_PURCHASE_LIMIT As String = "9"

    'Season Ticket Exception Change/Remove seat mode
    Public Const ST_EXCCEPTION_CHANGE_SEAT As String = "C"
    Public Const ST_EXCCEPTION_REMOVE_SEAT As String = "R"
    Public Const ST_EXCCEPTION_CHOOSE_SEAT As String = "U"
    Public Const ST_EXCCEPTION_UNALLOCATED_SEAT As String = "UNALLOCATED"

    'System Level Seat Reservation Codes
    Public Const RES_CODE_ST_RESERVED As String = "01"
    Public Const RES_CODE_ST_BOOKED As String = "02"
    Public Const RES_CODE_WeB_SALES As String = "03"
    Public Const RES_CODE_CUSTOMER_RESERVED As String = "04"
    Public Const RES_CODE_NOT_ON_SALE As String = "05"
    Public Const RES_CODE_BUY_BACK As String = "06"
    Public Const RES_CODE_AWAY_ALLOCATION As String = "07"
    Public Const RES_CODE_NON_ST_BUY_BACK As String = "08"
    Public Const RES_CODE_TICKET_EXCHANGE As String = "09"

    Public Const TICKET_EXCHANGE_PRICE_CODE As String = "T@"


    'CRUD Operation mode
    Public Enum CRUDOperationMode
        None = 0
        Create = 1
        Read = 2
        Update = 3
        Delete = 4
    End Enum

    'Forgotten/Reset password modes
    Public Const PASSWORD_ENC_MODE_INITIAL As String = "Initial"
    Public Const PASSWORD_ENC_MODE_RESPONSE As String = "Response"
    Public Const PASSWORD_ENC_MODE_USER_SIGNED_IN As String = "UserSignedIn"

    'Ticket Exchange Item Statuses
    Public Enum TicketExchangeItemStatus
        NotOnSale = 0
        OnSale = 1
        Sold = 2
        PriceChanged = 3
        TakingOffSale = 4
        PlacingOnSale = 5
    End Enum

    'Ticket Exchange /Buyback Return Types
    Public Const RETURN_TYPE_TICKET_EXCHANGE As String = "T"
    Public Const RETURN_TYPE_BUYBACK As String = "B"

    ' Querystring Obfuscation/Encryption
    Public Const ENCRYPTED_QUERYSTRING_PARAMETER_NAME As String = "TALENTId="

    'Ticketing Search Types
    Public Const SEARCH_SUBTYPE As String = "S"
    Public Const SEARCH_PRODUCT As String = "P"

    Public Const SORT_BY_DESCRIPTION As String = "Description"
    Public Const SORT_BY_PRODUCT_DATE As String = "ProductDate"
    Public Const SORT_BY_LOCATION As String = "Location"
    Public Const SORT_BY_CATEGORY As String = "Category"
    Public Const SORT_BY_STADIUM As String = "Stadium"
    Public Const SORT_BY_PRODUCT_TYPE As String = "ProductType"

    Public Const VIEW_BY_TILE As String = "T"
    Public Const VIEW_BY_LIST As String = "L"

    Public Const PRICE_BAND_DEFAULT_CUSTOMER As String = "C"
    Public Const PRICE_BAND_DEFAULT_PRODUCT As String = "P"

    Public Const PRICE_BAND_ALTERATIONS_PLUS_ANONYMOUS As String = "A"
    Public Const PRICE_BAND_ALTERATIONS_ALLOWED As String = "Y"
    Public Const PRICE_BAND_ALTERATIONS_RESTRICTED As String = "N"

    'Hospitality Booking Status
    Public Const ENQUIRY_BOOKING_STATUS As String = "E"
    Public Const SOLD_BOOKING_STATUS As String = "S"
    Public Const QUOTE_BOOKING_STATUS As String = "Q"
    Public Const RESERVATION_BOOKING_STATUS As String = "R"
    Public Const CANCELLED_BOOKING_STATUS As String = "C"
    Public Const CREDIT_BOOKING_STATUS As String = "Z"
    Public Const EXPIRED_BOOKING_STATUS As String = "X"
    Public Const IN_PROGRESS_BOOKING_STATUS As String = "I"

    'Hospitality Booking Clear Basket Modes
    Public Const SAVEBACKENDCOMPONENTDETAILS As String = "Y"
    Public Const CANCELBACKENDCOMPONENTDETAILS As String = "C"

    'Booking Options 
    Public Const MARK_FOR_BUISINESS As String = "B"
    Public Const MARK_FOR_PERSONAL As String = "P"

    'Corporate Baskets Redirection 
    Public Const REDIRECT_TO_COMPONENT_SELECTION_PAGE As String = "Y"
    Public Const REDIRECT_TO_BOOKING_PAGE As String = "B"

    'Activity Template Types
    Public Const ACTIVITY_TEMPLATE_TYPE_ID_PROD_TRANS As String = "1"
    Public Const ACTIVITY_TEMPLATE_TYPE_ID_CUSTOMER_NOTE As String = "2"
    Public Const ACTIVITY_TEMPLATE_TYPE_ID_COMPLIMENTARY As String = "3"
    Public Const ACTIVITY_TEMPLATE_TYPE_ID_HOSPITALITY As String = "4"
    Public Const ACTIVITY_TEMPLATE_TYPE_HOSPITALITY_DATA_CAPTURE As String = "5"

    'QandA status options
    Public Const QANDA_STATUS_COMPLETE = "C"
    Public Const QANDA_STATUS_INCOMPLETE = "I"

    'Backend Active and Disabled fields
    Public Const BACKEND_ACTIVE_FLAG_VALUE As String = "A"
    Public Const BACKEND_DISABLED_FLAG_VALUE As String = "D"

    'Backend PWS Agent
    Public Const BACKEND_PWS_AGENT As String = "INTERNET"

    'Backend Business Unit Flags
    Public Const BACKEND_BOXOFFICE_BUSINESS_UNIT_FLAG = "B"
    Public Const BACKEND_UNITEDKINGDOM_BUSINESS_UNIT_FLAG = "P"

    'Backend Interactive Mode
    Public Const BACKEND_INTERACTIVE_MODE As String = "I"

    'Hospitality Booking Print Status
    Public Const NA_STATUS As String = "X"
    Public Const NOT_PRINTED_STATUS As String = "N"
    Public Const PARTIALLY_PRINTED_STATUS As String = "P"
    Public Const FULLY_PRINTED_STATUS As String = "F"

    'Default Date
    Public Const DEFAULT_DATE = "01/01/0001"

    'Template Override Criterias
    Public Const TEMPLATE_OVERRIDE_CRITERIA_PACKAGE As String = "PK"
    Public Const TEMPLATE_OVERRIDE_CRITERIA_PRODUCTCODE As String = "PC"
    Public Const TEMPLATE_OVERRIDE_CRITERIA_SUBTYPE As String = "ST"
    Public Const TEMPLATE_OVERRIDE_CRITERIA_PRODUCTTYPE As String = "PT"
    Public Const TEMPLATE_OVERRIDE_CRITERIA_STADIUM As String = "SD"
    Public Const TEMPLATE_OVERRIDE_READ_MODE As String = "R"
    Public Const TEMPLATE_OVERRIDE_CREATE_MODE As String = "C"
    Public Const TEMPLATE_OVERRIDE_UPDATE_MODE As String = "U"
    Public Const TEMPLATE_OVERRIDE_DELETE_MODE As String = "D"

    'Hospitality Fixture Availability
    Public Const HOSPITALITY_FIXTURE_NOT_AVAILABLE As String = "NOTAVAILABLE"

End Class