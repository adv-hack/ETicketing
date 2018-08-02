Imports System.Configuration
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Web
Imports System.Xml
Imports System.Xml.Linq
Imports Talent.Common
Imports Talent.Common.Utilities
Imports Talent.Common.VanguardWebService

'todo ck list starts
'1. can we have global level error object what will be the risk
'2. properties are scattered and duplicated b/w this class, devanguardconfig and debasketpayment synchronise this correctly
'3. is devanguardconfig class naming convention requires change, move this class outside base folder
'4. can we move all the request xml string to const in one more private class
'5. update status in order status also
'6. when to call ProcessMerchandiseInBackend
'7. address line mapping is required b/w frontend tbl_address and backend
'todo ck list ends

<Serializable()> _
Public Class TalentVanguard
    Inherits TalentBase


#Region "Inner Class - VG Request XML Constants"
    Private Class VGRequestXML


        Public Const VGGENERATESESSIONREQUEST As String = "<?xml version='1.0'?>" & _
                       "<vggeneratesessionrequest xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='VANGUARD'> " & _
                       "<returnurl><![CDATA[{0}]]></returnurl> " & _
                       "<fullcapture>{1}</fullcapture>" & _
                       "</vggeneratesessionrequest>"

        Public Const VGGETCARDDETAILSREQUEST As String = "<?xml version='1.0'?>" & _
                      "<vggetcarddetailsrequest xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='VANGUARD'> " & _
                      "<sessionguid>{0}</sessionguid>" & _
                      "</vggetcarddetailsrequest>"

        Public Const VGTOKENREGISTRATIONREQUEST As String = "<?xml version='1.0'?>" & _
                                    "<vgtokenregistrationrequest xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='VANGUARD'>" & _
                                        "<sessionguid>{0}</sessionguid>" & _
                                        "<merchantreference>{1}</merchantreference>" & _
                                        "<expirydate>{2}</expirydate>" & _
                                        "<startdate>{3}</startdate>" & _
                                        "<issueno>{4}</issueno>" & _
                                        "<purchase>{5}</purchase>" & _
                                        "<refund>{6}</refund>" & _
                                        "<cashback>{7}</cashback>" & _
                                        "<tokenexpirationdate>{8}</tokenexpirationdate>" & _
                                    "</vgtokenregistrationrequest>"

        Public Const VGTRANSACTIONREQUEST As String = "<?xml version= '1.0'?>" & _
                                    "<vgtransactionrequest xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='VANGUARD'>" & _
                                        "<sessionguid>{0}</sessionguid> " & _
                                        "<merchantreference>{1}</merchantreference> " & _
                                        "<accountid>{2}</accountid>" & _
                                        "<txntype>{3}</txntype>" & _
                                        "<transactioncurrencycode>{4}</transactioncurrencycode>" & _
                                        "<apacsterminalcapabilities>{5}</apacsterminalcapabilities>" & _
                                        "<capturemethod>{6}</capturemethod>" & _
                                        "<processingidentifier>{7}</processingidentifier> " & _
                                        "{8} " & _
                                        "{9}" & _
                                        "<expirydate>{10}</expirydate>" & _
                                        "<issuenumber>{11}</issuenumber> " & _
                                        "<startdate>{12}</startdate> " & _
                                        "<txnvalue>{13}</txnvalue> " & _
                                        "<transactiondatetime>{14}</transactiondatetime> " & _
                                        "<terminalcountrycode>{15}</terminalcountrycode> " & _
                                        "{16}" & _
                                        "<accountpasscode>{17}</accountpasscode>" & _
                                        "</vgtransactionrequest>"


        Public Const VGTRANSACTIONREQUEST_NS As String = "<?xml version=""1.0""?>" & _
                                                        "<transactionrequest xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='TXN'>" & _
                                                            "<merchantreference>{0}</merchantreference>" & _
                                                            "<accountid>{1}</accountid>" & _
                                                            "<accountpasscode>{2}</accountpasscode>" & _
                                                            "<txntype>{3}</txntype>" & _
                                                            "<transactioncurrencycode>{4}</transactioncurrencycode>" & _
                                                            "<terminalcountrycode>{5}</terminalcountrycode>" & _
                                                            "<apacsterminalcapabilities>{6}</apacsterminalcapabilities>" & _
                                                            "<capturemethod>{7}</capturemethod>" & _
                                                            "<processingidentifier>{8}</processingidentifier>" & _
                                                            "<csc>{9}</csc>" & _
                                                            "{10} " & _
                                                            "{11}" & _
                                                            "<tokenid>{12}</tokenid>" & _
                                                            "<txnvalue>{13}</txnvalue>" & _
                                                            "{14}" & _
                                                        "</transactionrequest>"

        Public Const VGCONFIRMATIONREQUEST As String = "<?xml version='1.0'?>" & _
                                                            "<vgconfirmationrequest xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='VANGUARD'>" & _
                                                            "<sessionguid>{0}</sessionguid>" & _
                                                            "<transactionid>{1}</transactionid>" & _
                                                        "</vgconfirmationrequest>"

        Public Const VGCONFIRMATIONREQUEST_NS As String = "<?xml version=""1.0"" ?>" & _
                                                              "<confirmationrequest xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='TXN'>" & _
                                                                  "<transactionid>{0}</transactionid>" & _
                                                              "</confirmationrequest>"

        Public Const VGREJECTIONREQUEST As String = "<?xml version='1.0'?> " & _
                                                    "<vgrejectionrequest xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='VANGUARD'>" & _
                                                        "<sessionguid>{0}</sessionguid>" & _
                                                        "<transactionid>{1}</transactionid>" & _
                                                        "<capturemethod>{2}</capturemethod>" & _
                                                     "</vgrejectionrequest>"

        Public Const VGREJECTIONREQUEST_NS As String = "<?xml version=""1.0"" ?>" & _
                                                              "<rejectionrequest xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='TXN'>" & _
                                                                  "<transactionid>{0}</transactionid>" & _
                                                                  "<tokenid>{1}</tokenid>" & _
                                                                  "<capturemethod>{2}</capturemethod>" & _
                                                              "</rejectionrequest>"

        Public Const VGPAYERAUTHENROLLMENTCHECKREQUEST = "<?xml version='1.0'?>" & _
                                    "<vgpayerauthenrollmentcheckrequest xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='VANGUARD'> " & _
                                        "<sessionguid>{0}</sessionguid>" & _
                                        "<merchantreference>{1}</merchantreference>" & _
                                        "<mkaccountid>{2}</mkaccountid>" & _
                                        "<mkacquirerid>{3}</mkacquirerid>" & _
                                        "<merchantname>{4}</merchantname>" & _
                                        "<merchantcountrycode>{5}</merchantcountrycode> " & _
                                        "<merchanturl>{6}</merchanturl>" & _
                                        "<visamerchantbankid>{7}</visamerchantbankid>" & _
                                        "<visamerchantnumber>{8}</visamerchantnumber>" & _
                                        "<visamerchantpassword>{9}</visamerchantpassword>" & _
                                        "<mcmmerchantbankid>{10}</mcmmerchantbankid>" & _
                                        "<mcmmerchantnumber>{11}</mcmmerchantnumber>" & _
                                        "<mcmmerchantpassword>{12}</mcmmerchantpassword>" & _
                                        "<expirydate>{13}</expirydate>" & _
                                        "<currencycode>{14}</currencycode>" & _
                                        "<currencyexponent>{15}</currencyexponent>" & _
                                        "<browseracceptheader>{16}</browseracceptheader>" & _
                                        "<browseruseragentheader>{17}</browseruseragentheader>" & _
                                        "<transactionamount>{18}</transactionamount>" & _
                                        "<transactiondisplayamount>{19}</transactiondisplayamount>" & _
                                        "<transactiondescription>{20}</transactiondescription> " & _
                                    "</vgpayerauthenrollmentcheckrequest>"


        Public Const VGPAYERAUTHENROLLMENTCHECKREQUEST_NS As String = "<?xml version=""1.0"" ?>" & _
                                                             "<payerauthenrollmentcheckrequest xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='PAYERAUTH'> " & _
                                                                  "<merchantreference>{0}</merchantreference>" & _
                                                                  "<mkaccountid>{1}</mkaccountid>" & _
                                                                  "<mkacquirerid>{2}</mkacquirerid>" & _
                                                                  "<merchantname>{3}</merchantname>" & _
                                                                  "<merchantcountrycode>{4}</merchantcountrycode>" & _
                                                                  "<merchanturl>{5}</merchanturl>" & _
                                                                  "<visamerchantbankid>{6}</visamerchantbankid>" & _
                                                                  "<visamerchantnumber>{7}</visamerchantnumber>" & _
                                                                  "<visamerchantpassword>{8}</visamerchantpassword>" & _
                                                                  "<mcmmerchantbankid>{9}</mcmmerchantbankid>" & _
                                                                  "<mcmmerchantnumber>{10}</mcmmerchantnumber>" & _
                                                                  "<mcmmerchantpassword>{11}</mcmmerchantpassword>" & _
                                                                  "<tokenid>{12}</tokenid>" & _
                                                                  "<cardnumber>{13}</cardnumber>" & _
                                                                  "<cardexpyear>{14}</cardexpyear>" & _
                                                                  "<cardexpmonth>{15}</cardexpmonth>" & _
                                                                  "<currencycode>{16}</currencycode>" & _
                                                                  "<currencyexponent>{17}</currencyexponent>" & _
                                                                  "<transactionamount>{18}</transactionamount>" & _
                                                                  "<transactiondisplayamount>{19}</transactiondisplayamount>" & _
                                                              "</payerauthenrollmentcheckrequest>"


        Public Const VGPAYERAUTHAUTHENTICATIONCHECKREQUEST As String = "<?xml version='1.0'?>" & _
                                                                          "<vgpayerauthauthenticationcheckrequest xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='VANGUARD'> " & _
                                                                            "<sessionguid>{0}</sessionguid>" & _
                                                                            "<merchantreference>{1}</merchantreference>" & _
                                                                            "<payerauthrequestid>{2}</payerauthrequestid>" & _
                                                                            "<pares>{3}</pares>" & _
                                                                            "<enrolled>{4}</enrolled>" & _
                                                                          "</vgpayerauthauthenticationcheckrequest>"

        Public Const VGPAYERAUTHAUTHENTICATIONCHECKREQUEST_NS As String = "<?xml version=""1.0"" ?>" & _
                                                                       "<payerauthauthenticationcheckrequest xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='PAYERAUTH'> " & _
                                                                          "<merchantreference>{0}</merchantreference>" & _
                                                                          "<payerauthrequestid>{1}</payerauthrequestid>" & _
                                                                          "<pares>{2}</pares>" & _
                                                                          "<enrolled>{3}</enrolled>" & _
                                                                      "</payerauthauthenticationcheckrequest>"
        Public Const VGPAYERAUTHAUXILARYDATA As String = "<payerauthauxiliarydata> " & _
                                        "<authenticationstatus>{0}</authenticationstatus>" & _
                                        "<authenticationcavv>{1}</authenticationcavv>" & _
                                        "<authenticationeci>{2}</authenticationeci>" & _
                                        "<atsdata>{3}</atsdata>" & _
                                        "<transactionid>{4}</transactionid>" & _
                                        "</payerauthauxiliarydata>"



    End Class

#End Region

#Region "Inner Class - Status Comment Constants"
    Private Class StatusComment
        Public Const START_PAYMENT As String = "Payment process started"
        Public Const START_SAVEMYCARD As String = "Save my card process started"
        Public Const CARD_TYPE_CHANGED As String = "card type changed"
        Public Const GENERATED_SESSION_IDENTIFIER As String = "Session identifier generated successfully"
        Public Const GET_CARD_DETAIL As String = "Get card detail request started"
        Public Const CARD_TYPE_MISMATCHED As String = "Chosen card type not matched with the given card detail"
        Public Const CARD_TYPE_MATCHED As String = "Chosen card type matched with the given card detail"
        Public Const SAVE_CARD_FLAG_UPDATE As String = "Can save this card flag update"
        Public Const SAVE_CARD_FLAG_UPDATE_FAILED As String = "Failed can save this card flag update"
        Public Const CARD_TOKENISATION_START As String = "Card tokenisation started"
        Public Const CARD_TOKENISATION_END As String = "Card tokenisation completed"
        Public Const TRANSACTION_SEND_START As String = "Transaction request started"
        Public Const TRANSACTION_SEND_END As String = "Transaction request completed"
        Public Const TRANSACTION_CONFIRM_START As String = "Transaction confirm started"
        Public Const TRANSACTION_CONFIRM_END As String = "Transaction confirm completed"
        Public Const TRANSACTION_CONFIRM As String = "Transaction confirmed"
        Public Const TRANSACTION_REJECT As String = "Transaction rejected"
        Public Const PPS1_CONFIRM As String = "PPS1 card detail confirmed"
        Public Const PPS2_CONFIRM As String = "PPS2 card detail confirmed"
        Public Const PAYERAUTHENROLLMENTCHECK_START As String = "PayerAuthEnrollmentCheck started."
        Public Const PAYERAUTHENROLLMENTCHECK_END As String = "PayerAuthEnrollmentCheck completed."
        Public Const PAYERAUTHENTICATIONCHECK_START As String = "PayerAuthenticationCheck started."
        Public Const PAYERAUTHENTICATIONCHECK_END As String = "PayerAuthenticationCheck completed."
        Public Const SAVEMYCARD_CONFIRM As String = "Save my card detail confirmed"

        Public Const START_AMENDPPSCARD As String = "Amend PPS card process started"
        Public Const AMENDPPSCARD_CONFIRM As String = "Amend PPS card detail confirmed"


    End Class
#End Region

    Private Enum StatusID
        START_PAYMENT = 20
        START_SAVEMYCARD = 20
        CARD_TYPE_CHANGED = 21
        GENERATED_SESSION_IDENTIFIER = 30
        SAVE_CARD_FLAG_UPDATE = 31
        GET_CARD_DETAIL = 32
        CARD_TYPE_MISMATCHED = 33
        CARD_TYPE_MATCHED = 34
        PAYERAUTHENROLLMENTCHECK = 35
        SECURE3D = 36
        PAYERAUTHENTICATIONCHECK = 37
        CARD_TOKENISATION = 38
        TRANSACTION_SEND = 39
        TRANSACTION_REJECT = 40
        TRANSACTION_CONFIRM = 50
        PPS1_CONFIRM = 50
        PPS2_CONFIRM = 50
        SAVEMYCARD_CONFIRM = 50

        START_AMENDPPSCARD = 20
        AMENDPPSCARD_CONFIRM = 50
    End Enum

#Region "Class Level Fields"
    Private _logMessage As String = String.Empty
    Private _transactionAmount As Decimal
    Private _errorObj As ErrObject
    Private _talentLogging As TalentLogging = Nothing
    Private _deVanguard As DEVanguard
    Private _basketPayEntity As DEBasketPayment
    Private _cardTypeCvcAndAvs As DECVCAndAVSAuthorization = Nothing
    Private Const CALLERMETHOD_TRANSACTION_SEND As String = "TRANSACTION_SEND"
    Private Const CALLERMETHOD_TRANSACTION_CONFIRM As String = "TRANSACTION_CONFIRM"
    Private Const CALLERMETHOD_TRANSACTION_REJECT As String = "TRANSACTION_REJECT"

#End Region

#Region "Constructor"

    Public Sub New()

        _talentLogging = New Talent.Common.TalentLogging
        _deVanguard = New DEVanguard
        _basketPayEntity = New DEBasketPayment

    End Sub

#End Region

#Region "Properties"

    Public Property ResultDataSet() As DataSet

    Public Property BasketPayEntity() As DEBasketPayment
        Set(value As DEBasketPayment)
            _basketPayEntity = value
        End Set
        Get
            Return _basketPayEntity
        End Get
    End Property

    Public Property VanguardAttributes() As DEVanguard
        Set(value As DEVanguard)
            _deVanguard = value
        End Set
        Get
            If (_deVanguard Is Nothing) Then
                _deVanguard = New DEVanguard
            End If
            Return _deVanguard
        End Get
    End Property

    Private Const CLASSNAME As String = "TALENTVANGUARD"

#End Region

#Region "Public Methods"

    Public Function ProcessVanguard() As ErrorObj

        _talentLogging.FrontEndConnectionString = Settings.FrontEndConnectionString

        Dim err As New ErrorObj
        Try
            If VanguardAttributes.ProcessStep <> Nothing Then
                If VanguardAttributes.PaymentType = GlobalConstants.CCPAYMENTTYPE Then
                    err = ProcessVanguard_SessionBased()
                ElseIf VanguardAttributes.PaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE Then
                    err = ProcessVanguard_NonSession()
                End If
            End If
        Catch ex As Exception

        End Try

        Return err
    End Function

#End Region

#Region "Private Methods"

#Region "Initialising Methods"
    Private Function ProcessVanguard_SessionBased() As ErrorObj
        Dim err As New ErrorObj
        Select Case VanguardAttributes.ProcessStep
            Case DEVanguard.ProcessingStep.START_PAYMENT
                err = ProcessStep_StartPayment()
            Case DEVanguard.ProcessingStep.SAVE_CARD_FLAG_UPDATE
                err = ProcessStep_SaveCardFlagUpdate()
            Case DEVanguard.ProcessingStep.GET_CARD_DETAIL
                err = ProcessStep_GetCardDetail()
            Case DEVanguard.ProcessingStep.SECURE3D_OFF_CHECKOUT_TRANSACTION
                err = ProcessStep_Secure3D_Off_Checkout_Transaction()
            Case DEVanguard.ProcessingStep.SECURE3D_OFF_PPS_TRANSACTION
                err = ProcessStep_Secure3D_Off_PPS_Transaction()
            Case DEVanguard.ProcessingStep.SECURE3D_ON_CHECKOUT_PART1
                err = ProcessStep_Secure3D_On_Checkout_Transaction_Part1()
            Case DEVanguard.ProcessingStep.SECURE3D_ON_CHECKOUT_PART2
                err = ProcessStep_Secure3D_On_Checkout_Transaction_Part2(True)
            Case DEVanguard.ProcessingStep.SECURE3D_ON_PPS_TRANSACTION_PART1
                err = ProcessStep_Secure3D_On_PPS_Transaction_Part1()
            Case DEVanguard.ProcessingStep.SECURE3D_ON_PPS_TRANSACTION_PART2
                err = ProcessStep_Secure3D_On_PPS_Transaction_Part2(True)
            Case DEVanguard.ProcessingStep.SECURE3D_OFF_CHECKOUT_PARTPAYMENT
                err = ProcessStep_Secure3D_Off_Checkout_PartPayment()
            Case DEVanguard.ProcessingStep.SECURE3D_ON_CHECKOUT_PARTPAYMENT1
                err = ProcessStep_Secure3D_On_Checkout_PartPayment_Transaction_Part1()
            Case DEVanguard.ProcessingStep.SECURE3D_ON_CHECKOUT_PARTPAYMENT2
                err = ProcessStep_Secure3D_On_Checkout_PartPayment_Transaction_Part2(True)
            Case DEVanguard.ProcessingStep.START_SAVEMYCARD
                err = ProcessStep_StartSaveMyCard()
            Case DEVanguard.ProcessingStep.SAVE_MY_CARD
                err = ProcessStep_Secure3D_Off_SaveMyCard()
            Case DEVanguard.ProcessingStep.START_AMENDPPSCARD
                err = ProcessStep_StartAmendPPSCard()
            Case DEVanguard.ProcessingStep.SAVE_AMENDPPS_CARD
                err = ProcessStep_Secure3D_Off_AmendPPSCard()
            Case Else
                Dim test As String = "wrong step call log it"
        End Select
        Return err
    End Function

    Private Function ProcessVanguard_NonSession() As ErrorObj
        Dim err As New ErrorObj
        Select Case VanguardAttributes.ProcessStep
            Case DEVanguard.ProcessingStep.START_PAYMENT
                err = ProcessStep_StartPayment_NS()
            Case DEVanguard.ProcessingStep.SECURE3D_OFF_CHECKOUT_TRANSACTION
                err = ProcessStep_Secure3D_Off_Checkout_Transaction_NS()
            Case DEVanguard.ProcessingStep.SECURE3D_OFF_PPS_TRANSACTION
                err = ProcessStep_Secure3D_Off_PPS_Transaction_NS()
            Case DEVanguard.ProcessingStep.SECURE3D_ON_CHECKOUT_PART1
                err = ProcessStep_Secure3D_On_Checkout_Transaction_Part1_NS()
            Case DEVanguard.ProcessingStep.SECURE3D_ON_CHECKOUT_PART2
                err = ProcessStep_Secure3D_On_Checkout_Transaction_Part2_NS(True)
            Case DEVanguard.ProcessingStep.SECURE3D_OFF_CHECKOUT_PARTPAYMENT
                err = ProcessStep_Secure3D_Off_Checkout_PartPayment_NS()
            Case DEVanguard.ProcessingStep.SECURE3D_ON_CHECKOUT_PARTPAYMENT1
                err = ProcessStep_Secure3D_On_Checkout_PartPayment_Transaction_Part1_NS()
            Case DEVanguard.ProcessingStep.SECURE3D_ON_CHECKOUT_PARTPAYMENT2
                err = ProcessStep_Secure3D_On_Checkout_PartPayment_Transaction_Part2_NS(True)
            Case Else
                Dim test As String = "wrong step call log it"
        End Select
        Return err
    End Function

    Private Sub SetConfigurationDetail()
        Dim err As ErrorObj = GetVanguardConfigurations()
        If Not err.HasError AndAlso ResultDataSet IsNot Nothing Then
            If ResultDataSet.Tables("VanguardConfiguration") IsNot Nothing AndAlso ResultDataSet.Tables("PayAccountConfigurations") IsNot Nothing Then
                Dim dtVanguardAttributes As DataTable = ResultDataSet.Tables("VanguardConfiguration")
                VanguardAttributes.SystemID = Utilities.CheckForDBNull_String(dtVanguardAttributes.Rows(0)("SystemID"))
                VanguardAttributes.SystemGUID = Utilities.CheckForDBNull_String(dtVanguardAttributes.Rows(0)("SystemGUID"))
                VanguardAttributes.SystemPasscode = Utilities.CheckForDBNull_String(dtVanguardAttributes.Rows(0)("SystemPasscode"))
                VanguardAttributes.GatewayUrl = Utilities.CheckForDBNull_String(dtVanguardAttributes.Rows(0)("WebserviceURL"))
                SetAccountID(ResultDataSet.Tables("PayAccountConfigurations"))
            End If
        End If
        VanguardAttributes.ReturnURL = GetReturnURL()
    End Sub

    Private Sub SetAccountID(ByVal dtAccountDetail As DataTable)
        Dim isBoxOffice As Boolean = False
        Dim accountID As String = String.Empty
        Dim accountPassCode As String = String.Empty

        If Settings.IsAgent AndAlso Settings.OriginatingSource.Trim.Length > 0 Then
            isBoxOffice = True
        End If
        Select Case VanguardAttributes.CaptureMethod
            Case "1" : VanguardAttributes.ApacsTerminalCapabilities = "6290" 'Keyed Cardholder Present
            Case "2" : VanguardAttributes.ApacsTerminalCapabilities = "4290" 'Keyed Cardholder Not Present Mail Order
            Case "3" : VanguardAttributes.ApacsTerminalCapabilities = "6290" 'Swiped
            Case "11" : VanguardAttributes.ApacsTerminalCapabilities = "4290" 'Keyed Cardholder Not Present Telephone Order
            Case "12" : VanguardAttributes.ApacsTerminalCapabilities = "4298" 'Keyed Cardholder Not Present Ecommerce Order
            Case Else : VanguardAttributes.ApacsTerminalCapabilities = "4298" 'Default
        End Select
        If dtAccountDetail IsNot Nothing AndAlso dtAccountDetail.Rows.Count > 0 Then
            'set the default first then we will override based on condition
            If isBoxOffice Then
                accountID = CheckForDBNull_String(dtAccountDetail.Rows(0)("RASMNum")).Trim
                accountPassCode = CheckForDBNull_String(dtAccountDetail.Rows(0)("RASMNumPwd")).Trim
            Else
                accountID = CheckForDBNull_String(dtAccountDetail.Rows(0)("PWSMNum")).Trim
                accountPassCode = CheckForDBNull_String(dtAccountDetail.Rows(0)("PWSMNumPwd")).Trim
            End If
        End If
        VanguardAttributes.AccountID = accountID
        VanguardAttributes.AccountPasscode = accountPassCode
    End Sub

    Private Function GetReturnURL() As String
        Dim sbReturnURL As New StringBuilder
        sbReturnURL.Append(VanguardAttributes.TalentVGGatewayPage)
        sbReturnURL.Append("&_utcr=" & System.Guid.NewGuid.ToString())
        sbReturnURL.Append("&_ubpcr=" & VanguardAttributes.BasketPaymentID)
        sbReturnURL.Append("&_ubhcr=" & VanguardAttributes.BasketHeaderID)
        sbReturnURL.Append("&_ucos=" & VanguardAttributes.CheckOutStage)
        sbReturnURL.Append("&_uspiii=" & VanguardAttributes.SessionID)
        Return sbReturnURL.ToString
    End Function

    Private Function GetVanguardConfigurations() As ErrorObj
        Const ModuleName As String = "GetVanguardConfigurations"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = vgatt=" & "")
        Dim err As New ErrorObj
        Dim dbVG As New DBVanguard
        If VanguardAttributes.CacheConfigSeconds > 0 Then
            Settings.Cacheing = True
            Settings.CacheTimeSeconds = VanguardAttributes.CacheConfigSeconds
        End If
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            With dbVG
                .TDataObjects = TDataObjects
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError Then
                    err = .AccessDatabase()
                End If
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                Else
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Private Function GetVanguardClientHeader() As VanguardWebService.ClientHeader
        SetConfigurationDetail()
        Dim header As New VanguardWebService.ClientHeader
        header.SendAttempt = 0
        header.SystemID = VanguardAttributes.SystemID
        header.SystemGUID = VanguardAttributes.SystemGUID
        header.Passcode = VanguardAttributes.SystemPasscode
        header.CDATAWrapping = VanguardAttributes.CDATAWrapping
        header.ProcessingDB = VanguardAttributes.ProcessingDB
        Return header
    End Function

#End Region

#Region "Process Step Methods"

    Private Function ProcessStep_StartPayment_NS() As ErrorObj
        Dim err As New ErrorObj
        Try
            err = InsertBasketStatus(StatusID.START_PAYMENT, StatusComment.START_PAYMENT)
            If Not err.HasError Then
                If Me.BasketPayEntity IsNot Nothing Then
                    With Me.BasketPayEntity
                        .LOGIN_ID = Settings.LoginId
                        'as payment start is card type matched status for saved card
                        .STATUS = StatusID.CARD_TYPE_MATCHED
                        .AGENT_NAME = Settings.OriginatingSource
                    End With
                End If
                Dim basketPaymentID As Integer = TDataObjects.BasketSettings.TblBasketPayment.StartPaymentForStage(Me.BasketPayEntity)
                If basketPaymentID > 0 Then
                    Me.VanguardAttributes.BasketPaymentID = basketPaymentID
                    Me.VanguardAttributes.ReturnURL = GetReturnURL()
                    err = InsertBasketStatus(StatusID.CARD_TYPE_MATCHED, StatusComment.CARD_TYPE_MATCHED)
                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTV-PSSPNS-002"
                    err.ErrorMessage = "Failed start payment insert for saved card"
                    _logMessage = VanguardAttributes.BasketHeaderID.ToString() & ";" & StatusID.CARD_TYPE_MATCHED.ToString & ";" & StatusComment.CARD_TYPE_MATCHED
                End If
            End If
            If err.HasError Then
                LogVanguardProcessError(err, _logMessage, "ProcessStep_StartPayment_NS")
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-PSSPNS-001"
            err.ErrorMessage = "Exception start payment insert"
            LogVanguardProcessError(err, ex, "ProcessStep_StartPayment_NS")
        End Try

        Return err
    End Function

    Private Function ProcessStep_Secure3D_Off_Checkout_Transaction_NS() As ErrorObj
        Dim err As New ErrorObj

        err = ProcessStep_TransactionSend()
        If Not err.HasError Then
            err = ProcessStep_TransactionConfirm()
        Else
            err = ProcessStep_TransactionReject()
        End If

        Return err
    End Function

    Private Function ProcessStep_Secure3D_Off_Checkout_PartPayment_NS() As ErrorObj
        Dim err As New ErrorObj

        err = ProcessStep_TransactionSend()
        If Not err.HasError Then
            err = ProcessStep_TransactionConfirm()
        Else
            err = ProcessStep_TransactionReject()
        End If

        Return err

    End Function

    Private Function ProcessStep_Secure3D_Off_PPS_Transaction_NS() As ErrorObj
        Dim err As New ErrorObj

        If VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.PPS1 Then
            err = InsertBasketStatus(StatusID.PPS1_CONFIRM, StatusComment.PPS1_CONFIRM)
            'update basket status and basket payment with status
        ElseIf VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.PPS2 Then
            err = InsertBasketStatus(StatusID.PPS2_CONFIRM, StatusComment.PPS2_CONFIRM)
            'so update basket status and basket payment with status
        End If

        Return err
    End Function

    Private Function ProcessStep_Secure3D_On_Checkout_Transaction_Part1_NS() As ErrorObj
        Dim err As New ErrorObj
        If Not err.HasError Then
            Dim canCallPart2 As Boolean = False
            Dim isEnrollWSCalled As Boolean = False
            err = ProcessStep_CanProcess3DSecure(canCallPart2, isEnrollWSCalled)
            If (Not err.HasError) AndAlso (canCallPart2) Then
                '3dsecure not enabled for the card type or user not enrolled for the given card
                'so call directly the second part
                err = ProcessStep_Secure3D_On_Checkout_Transaction_Part2_NS(isEnrollWSCalled)
            End If
        End If
        Return err
    End Function

    Private Function ProcessStep_Secure3D_On_Checkout_Transaction_Part2_NS(ByVal isEnrollWSCalled As Boolean) As ErrorObj
        Dim err As New ErrorObj
        If isEnrollWSCalled Then err = ProcessStep_CheckPayerAuthAuthentication()

        If Not err.HasError Then
            err = ProcessStep_TransactionSend()
            If Not err.HasError Then
                err = ProcessStep_TransactionConfirm()
            Else
                err = ProcessStep_TransactionReject()
            End If
        End If

        Return err
    End Function

    Private Function ProcessStep_Secure3D_On_Checkout_PartPayment_Transaction_Part1_NS() As ErrorObj
        Dim err As New ErrorObj
        If Not err.HasError Then
            Dim canCallPart2 As Boolean = False
            Dim isEnrollWSCalled As Boolean = False
            err = ProcessStep_CanProcess3DSecure(canCallPart2, isEnrollWSCalled)
            If (Not err.HasError) AndAlso (canCallPart2) Then
                '3dsecure not enabled for the card type or user not enrolled for the given card
                'so call directly the second part
                err = ProcessStep_Secure3D_On_Checkout_PartPayment_Transaction_Part2_NS(isEnrollWSCalled)
            End If
        End If
        Return err
    End Function

    Private Function ProcessStep_Secure3D_On_Checkout_PartPayment_Transaction_Part2_NS(ByVal isEnrollWSCalled As Boolean) As ErrorObj

        Dim err As New ErrorObj
        If isEnrollWSCalled Then err = ProcessStep_CheckPayerAuthAuthentication()
        If Not err.HasError Then
            err = ProcessStep_TransactionSend()
            If Not err.HasError Then
                err = ProcessStep_TransactionConfirm()
            Else
                err = ProcessStep_TransactionReject()
            End If
        End If

        Return err

    End Function

    Private Function ProcessStep_StartPayment() As ErrorObj
        Dim err As New ErrorObj
        Try
            err = InsertBasketStatus(StatusID.START_PAYMENT, StatusComment.START_PAYMENT)
            If Not err.HasError Then
                Dim basketPayEntity As New DEBasketPayment
                With basketPayEntity
                    .BASKET_HEADER_ID = VanguardAttributes.BasketHeaderID
                    .LOGIN_ID = Settings.LoginId
                    .STATUS = StatusID.START_PAYMENT
                    .CHECKOUT_STAGE = VanguardAttributes.CheckOutStage
                    .CAPTUREMETHOD = VanguardAttributes.CaptureMethod
                    .CAN_SAVE_CARD = VanguardAttributes.SaveThisCard
                    .PAYMENT_TYPE = VanguardAttributes.PaymentType
                    .AGENT_NAME = Settings.OriginatingSource
                    .CARD_TYPE = VanguardAttributes.CardType
                    If VanguardAttributes.PaymentAmount <> "" Then
                        .PAYMENT_AMOUNT = VanguardAttributes.PaymentAmount
                    End If
                    .BASKET_AMOUNT = VanguardAttributes.BasketAmount
                    .TEMP_ORDER_ID = VanguardAttributes.TempOrderID
                End With

                Dim basketPaymentID As Integer = TDataObjects.BasketSettings.TblBasketPayment.StartPaymentForStage(basketPayEntity)
                If basketPaymentID > 0 Then
                    Me.VanguardAttributes.BasketPaymentID = basketPaymentID
                    err = IsSessionIdentifierGenerated()
                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTV-PSSP-002"
                    err.ErrorMessage = "Failed start payment insert"
                    _logMessage = VanguardAttributes.BasketHeaderID.ToString() & ";" & StatusID.START_PAYMENT.ToString & ";" & StatusComment.START_PAYMENT
                End If
            End If
            If err.HasError Then
                LogVanguardProcessError(err, _logMessage, "ProcessStep_StartPayment")
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-PSSP-001"
            err.ErrorMessage = "Exception start payment insert"
            LogVanguardProcessError(err, ex, "ProcessStep_StartPayment")
        End Try

        Return err
    End Function

    Private Function ProcessStep_SaveCardFlagUpdate() As ErrorObj
        Dim err As New ErrorObj
        Try
            err = InsertBasketStatus(StatusID.SAVE_CARD_FLAG_UPDATE, StatusComment.SAVE_CARD_FLAG_UPDATE)
            If Not err.HasError Then
                With BasketPayEntity
                    .CAN_SAVE_CARD = VanguardAttributes.SaveThisCard
                    .BASKET_PAYMENT_ID = VanguardAttributes.BasketPaymentID
                End With
                Dim affectedRows As Integer = TDataObjects.BasketSettings.TblBasketPayment.UpdateSavedCardFlag(BasketPayEntity)

                If affectedRows <= 0 Then
                    err.HasError = True
                    err.ErrorNumber = "TACTV-SCFU-002"
                    err.ErrorMessage = "Error while updating save card flag in basket payment"
                    err = InsertBasketStatus(StatusID.SAVE_CARD_FLAG_UPDATE, StatusComment.SAVE_CARD_FLAG_UPDATE_FAILED)
                End If
            End If
            If err.HasError Then
                LogVanguardProcessError(err, _logMessage, "ProcessStep_SaveCardFlagUpdate")
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-SCFU-001"
            err.ErrorMessage = "Exception while updating save card flag in basket payment"
            LogVanguardProcessError(err, ex, "ProcessStep_SaveCardFlagUpdate")
        End Try
        Return err
    End Function

    Private Function ProcessStep_Secure3D_On_Checkout_Transaction_Part1() As ErrorObj
        Dim err As New ErrorObj
        err = ProcessStep_GetCardDetail()
        If Not err.HasError Then
            Dim canCallPart2 As Boolean = False
            Dim isEnrollWSCalled As Boolean = False
            err = ProcessStep_CanProcess3DSecure(canCallPart2, isEnrollWSCalled)
            If (Not err.HasError) AndAlso (canCallPart2) Then
                '3dsecure not enabled for the card type or user not enrolled for the given card
                'so call directly the second part
                err = ProcessStep_Secure3D_On_Checkout_Transaction_Part2(isEnrollWSCalled)
            End If
        End If
        Return err
    End Function

    Private Function ProcessStep_Secure3D_On_Checkout_Transaction_Part2(ByVal isEnrollWSCalled As Boolean) As ErrorObj
        Dim err As New ErrorObj
        If isEnrollWSCalled Then err = ProcessStep_CheckPayerAuthAuthentication()
        If Not err.HasError Then
            err = ProcessStep_TokeniseTheCard()
            If Not err.HasError Then
                err = ProcessStep_TransactionSend()
                If Not err.HasError Then
                    err = ProcessStep_TransactionConfirm()
                Else
                    err = ProcessStep_TransactionReject()
                End If
            End If
        End If
        Return err
    End Function

    Private Function ProcessStep_Secure3D_On_PPS_Transaction_Part1() As ErrorObj
        Dim err As New ErrorObj
        err = ProcessStep_GetCardDetail()
        If Not err.HasError Then
            Dim canCallPart2 As Boolean = False
            Dim isEnrollWSCalled As Boolean = False
            err = ProcessStep_CanProcess3DSecure(canCallPart2, isEnrollWSCalled)
            If (Not err.HasError) AndAlso (canCallPart2) Then
                '3dsecure not enabled for the card type or user not enrolled for the given card
                'so call directly the second part
                err = ProcessStep_Secure3D_On_PPS_Transaction_Part2(isEnrollWSCalled)
            End If
        End If
        Return err
    End Function

    Private Function ProcessStep_Secure3D_On_PPS_Transaction_Part2(ByVal isEnrollWSCalled As Boolean) As ErrorObj
        Dim err As New ErrorObj
        If isEnrollWSCalled Then err = ProcessStep_CheckPayerAuthAuthentication()
        If Not err.HasError Then
            err = ProcessStep_TokeniseTheCard()
        End If
        Return err
    End Function

    Private Function ProcessStep_Secure3D_Off_Checkout_Transaction() As ErrorObj
        Dim err As New ErrorObj
        err = ProcessStep_GetCardDetail()
        If Not err.HasError Then
            err = ProcessStep_TokeniseTheCard()
            If Not err.HasError Then
                err = ProcessStep_TransactionSend()
                If Not err.HasError Then
                    err = ProcessStep_TransactionConfirm()
                Else
                    err = ProcessStep_TransactionReject()
                End If
            End If
        End If
        Return err
    End Function

    Private Function ProcessStep_Secure3D_Off_PPS_Transaction() As ErrorObj
        Dim err As New ErrorObj
        err = ProcessStep_GetCardDetail()
        If Not err.HasError Then
            err = ProcessStep_TokeniseTheCard()
            If Not err.HasError Then
                'card is tokenised for pps this is confirmation stage
                If VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.PPS1 Then
                    err = InsertBasketStatus(StatusID.PPS1_CONFIRM, StatusComment.PPS1_CONFIRM)
                    'so update basket status and basket payment with status
                ElseIf VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.PPS2 Then
                    err = InsertBasketStatus(StatusID.PPS2_CONFIRM, StatusComment.PPS2_CONFIRM)
                    'so update basket status and basket payment with status
                End If
            End If
        End If
        Return err
    End Function

    Private Function ProcessStep_Secure3D_Off_Checkout_PartPayment() As ErrorObj
        Dim err As New ErrorObj

        err = ProcessStep_GetCardDetail()

        If Not err.HasError Then
            err = ProcessStep_TokeniseTheCard()
            If Not err.HasError Then
                err = ProcessStep_TransactionSend()
                If Not err.HasError Then
                    err = ProcessStep_TransactionConfirm()
                Else
                    err = ProcessStep_TransactionReject()
                End If
            End If
        End If

        Return err
    End Function

    Private Function ProcessStep_Secure3D_On_Checkout_PartPayment_Transaction_Part1() As ErrorObj
        Dim err As New ErrorObj

        err = ProcessStep_GetCardDetail()

        If Not err.HasError Then
            Dim canCallPart2 As Boolean = False
            Dim isEnrollWSCalled As Boolean = False
            err = ProcessStep_CanProcess3DSecure(canCallPart2, isEnrollWSCalled)
            If (Not err.HasError) AndAlso (canCallPart2) Then
                '3dsecure not enabled for the card type or user not enrolled for the given card
                'so call directly the second part
                err = ProcessStep_Secure3D_On_Checkout_PartPayment_Transaction_Part2(isEnrollWSCalled)
            End If

        End If

        Return err

    End Function

    Private Function ProcessStep_Secure3D_On_Checkout_PartPayment_Transaction_Part2(ByVal isEnrollWSCalled As Boolean) As ErrorObj
        Dim err As New ErrorObj
        If isEnrollWSCalled Then err = ProcessStep_CheckPayerAuthAuthentication()
        If Not err.HasError Then
            err = ProcessStep_TokeniseTheCard()
            If Not err.HasError Then
                err = ProcessStep_TransactionSend()
                If Not err.HasError Then
                    err = ProcessStep_TransactionConfirm()
                Else
                    err = ProcessStep_TransactionReject()
                End If
            End If
        End If

        Return err
    End Function

    Private Function ProcessStep_StartSaveMyCard() As ErrorObj

        Dim err As New ErrorObj
        Try
            err = InsertBasketStatus(StatusID.START_SAVEMYCARD, StatusComment.START_SAVEMYCARD)
            If Not err.HasError Then
                Dim basketPayEntity As New DEBasketPayment
                With basketPayEntity
                    .BASKET_HEADER_ID = VanguardAttributes.BasketHeaderID
                    .LOGIN_ID = Settings.LoginId
                    .STATUS = StatusID.START_SAVEMYCARD
                    .CHECKOUT_STAGE = VanguardAttributes.CheckOutStage
                    .CAPTUREMETHOD = VanguardAttributes.CaptureMethod
                    .CAN_SAVE_CARD = VanguardAttributes.SaveThisCard
                    .PAYMENT_TYPE = VanguardAttributes.PaymentType
                    .AGENT_NAME = Settings.OriginatingSource
                    .CARD_TYPE = VanguardAttributes.CardType
                    If VanguardAttributes.PaymentAmount <> "" Then
                        .PAYMENT_AMOUNT = VanguardAttributes.PaymentAmount
                    End If
                    .BASKET_AMOUNT = VanguardAttributes.BasketAmount
                    .TEMP_ORDER_ID = VanguardAttributes.TempOrderID
                End With

                Dim basketPaymentID As Integer = TDataObjects.BasketSettings.TblBasketPayment.StartPaymentForStage(basketPayEntity)
                If basketPaymentID > 0 Then
                    Me.VanguardAttributes.BasketPaymentID = basketPaymentID
                    err = IsSessionIdentifierGenerated()
                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTV-PSSMC-002"
                    err.ErrorMessage = "Failed start save my card insert"
                    _logMessage = VanguardAttributes.BasketHeaderID.ToString() & ";" & StatusID.START_PAYMENT.ToString & ";" & StatusComment.START_SAVEMYCARD
                End If
            End If
            If err.HasError Then
                LogVanguardProcessError(err, _logMessage, "ProcessStep_StartSaveMyCard")
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-PSSMC-001"
            err.ErrorMessage = "Exception start save my card insert"
            LogVanguardProcessError(err, ex, "ProcessStep_StartSaveMyCard")
        End Try

        Return err

    End Function

    Private Function ProcessStep_Secure3D_Off_SaveMyCard() As ErrorObj
        Dim err As New ErrorObj
        err = ProcessStep_GetCardDetail()
        If Not err.HasError Then
            err = ProcessStep_TokeniseTheCard()
            If Not err.HasError Then
                'card is tokenised for save my card
                If VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.SAVEMYCARD Then
                    err = InsertBasketStatus(StatusID.SAVEMYCARD_CONFIRM, StatusComment.SAVEMYCARD_CONFIRM)
                    BasketPayEntity.STATUS = StatusID.SAVEMYCARD_CONFIRM
                    BasketPayEntity.MERCHANT_REFERENCE = GetMerchantReference(BasketPayEntity.BASKET_PAYMENT_ID)
                    TDataObjects.BasketSettings.TblBasketPayment.UpdateStatus(BasketPayEntity.BASKET_PAYMENT_ID, StatusID.SAVEMYCARD_CONFIRM, BasketPayEntity.MERCHANT_REFERENCE)
                End If
            End If
        End If
        Return err
    End Function

    Private Function ProcessStep_StartAmendPPSCard() As ErrorObj

        Dim err As New ErrorObj
        Try
            err = InsertBasketStatus(StatusID.START_AMENDPPSCARD, StatusComment.START_AMENDPPSCARD)
            If Not err.HasError Then
                Dim basketPayEntity As New DEBasketPayment
                With basketPayEntity
                    .BASKET_HEADER_ID = VanguardAttributes.BasketHeaderID
                    .LOGIN_ID = Settings.LoginId
                    .STATUS = StatusID.START_AMENDPPSCARD
                    .CHECKOUT_STAGE = VanguardAttributes.CheckOutStage
                    .CAPTUREMETHOD = VanguardAttributes.CaptureMethod
                    .CAN_SAVE_CARD = VanguardAttributes.SaveThisCard
                    .PAYMENT_TYPE = VanguardAttributes.PaymentType
                    .AGENT_NAME = Settings.OriginatingSource
                    .CARD_TYPE = VanguardAttributes.CardType
                    If VanguardAttributes.PaymentAmount <> "" Then
                        .PAYMENT_AMOUNT = VanguardAttributes.PaymentAmount
                    End If
                    .BASKET_AMOUNT = VanguardAttributes.BasketAmount
                    .TEMP_ORDER_ID = VanguardAttributes.TempOrderID
                End With

                Dim basketPaymentID As Integer = TDataObjects.BasketSettings.TblBasketPayment.StartPaymentForStage(basketPayEntity)
                If basketPaymentID > 0 Then
                    Me.VanguardAttributes.BasketPaymentID = basketPaymentID
                    err = IsSessionIdentifierGenerated()
                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTV-PSAPC-002"
                    err.ErrorMessage = "Failed start amend pps card insert"
                    _logMessage = VanguardAttributes.BasketHeaderID.ToString() & ";" & StatusID.START_PAYMENT.ToString & ";" & StatusComment.START_SAVEMYCARD
                End If
            End If
            If err.HasError Then
                LogVanguardProcessError(err, _logMessage, "ProcessStep_StartAmendPPSCard")
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-PSAPC-001"
            err.ErrorMessage = "Exception start amend pps card insert"
            LogVanguardProcessError(err, ex, "ProcessStep_StartAmendPPSCard")
        End Try

        Return err

    End Function

    Private Function ProcessStep_Secure3D_Off_AmendPPSCard() As ErrorObj
        Dim err As New ErrorObj
        err = ProcessStep_GetCardDetail()
        If Not err.HasError Then
            err = ProcessStep_TokeniseTheCard()
            If Not err.HasError Then
                'card is tokenised for amend pps card
                If VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.AMENDPPSCARD Then
                    err = InsertBasketStatus(StatusID.AMENDPPSCARD_CONFIRM, StatusComment.AMENDPPSCARD_CONFIRM)
                    BasketPayEntity.STATUS = StatusID.AMENDPPSCARD_CONFIRM
                    BasketPayEntity.MERCHANT_REFERENCE = GetMerchantReference(BasketPayEntity.BASKET_PAYMENT_ID)
                    TDataObjects.BasketSettings.TblBasketPayment.UpdateStatus(BasketPayEntity.BASKET_PAYMENT_ID, StatusID.AMENDPPSCARD_CONFIRM, BasketPayEntity.MERCHANT_REFERENCE)
                End If
            End If
        End If
        Return err
    End Function

    Private Function ProcessStep_GetCardDetail() As ErrorObj
        Dim err As New ErrorObj
        err = InsertBasketStatus(StatusID.GET_CARD_DETAIL, StatusComment.GET_CARD_DETAIL)
        If Not err.HasError Then
            err = IsMaskedCardDetailsSaved()
        End If
        If err.HasError Then
            LogVanguardProcessError(err, _logMessage, "ProcessStep_GetCardDetail")
        End If
        Return err
    End Function

    Private Function ProcessStep_CheckPayerAuthEnrollment() As ErrorObj
        Dim err As New ErrorObj
        err = InsertBasketStatus(StatusID.PAYERAUTHENROLLMENTCHECK, StatusComment.PAYERAUTHENROLLMENTCHECK_START)

        If Not err.HasError Then
            err = IsPayerAuthEnrollmentVerified()
        End If
        If err.HasError Then
            LogVanguardProcessError(err, _logMessage, "ProcessStep_CheckPayerAuthEnrollment")
        End If

        Return err
    End Function

    Private Function ProcessStep_CheckPayerAuthAuthentication()
        Dim err As New ErrorObj
        err = InsertBasketStatus(StatusID.PAYERAUTHENTICATIONCHECK, StatusComment.PAYERAUTHENTICATIONCHECK_START)

        If Not err.HasError Then
            err = IsPayerAuthAuthenticationVerified()
        End If
        If err.HasError Then
            LogVanguardProcessError(err, _logMessage, "ProcessStep_CheckPayerAuthAuthentication")
        End If

        Return err
    End Function

    Private Function ProcessStep_TokeniseTheCard() As ErrorObj
        Dim err As New ErrorObj
        err = InsertBasketStatus(StatusID.CARD_TOKENISATION, StatusComment.CARD_TOKENISATION_START)
        If Not err.HasError Then
            err = IsTokenRegistered()
        End If
        If err.HasError Then
            LogVanguardProcessError(err, _logMessage, "ProcessStep_TokeniseTheCard")
        End If
        Return err
    End Function

    Private Function ProcessStep_TransactionSend() As ErrorObj
        Dim err As New ErrorObj
        err = InsertBasketStatus(StatusID.TRANSACTION_SEND, StatusComment.TRANSACTION_SEND_START)
        If Not err.HasError Then
            err = IsTransactionRequestCompleted()
        End If
        If err.HasError Then
            LogVanguardProcessError(err, _logMessage, "ProcessStep_TransactionSend")
        End If
        Return err
    End Function

    Private Function ProcessStep_TransactionConfirm() As ErrorObj
        Dim err As New ErrorObj
        err = InsertBasketStatus(StatusID.TRANSACTION_CONFIRM, StatusComment.TRANSACTION_CONFIRM_START)
        If Not err.HasError Then
            err = IsTransactionConfirmed()
        End If
        If err.HasError Then
            LogVanguardProcessError(err, _logMessage, "ProcessStep_TransactionConfirm")
        Else
            VanguardAttributes.Is3DTransactionCompleted = True
        End If
        Return err
    End Function

    Private Function ProcessStep_TransactionReject() As ErrorObj
        Dim err As New ErrorObj
        err = InsertBasketStatus(StatusID.TRANSACTION_REJECT, StatusComment.TRANSACTION_REJECT)
        If Not err.HasError Then
            err = IsTransactionRejected()
        End If
        If err.HasError Then
            LogVanguardProcessError(err, _logMessage, "ProcessStep_TransactionReject")
        End If
        Return err
    End Function

    Private Function ProcessStep_CanProcess3DSecure(ByRef no3DSecureCallPart2 As Boolean, ByRef isEnrolWSCalled As Boolean) As ErrorObj
        Dim err As New ErrorObj
        If canProcess3DSecure(BasketPayEntity.CARD_TYPE) Then
            err = ProcessStep_CheckPayerAuthEnrollment()
            If Not err.HasError Then
                If Not VanguardAttributes.PayerAuth().IsEnrolled Then
                    no3DSecureCallPart2 = True
                    isEnrolWSCalled = True
                End If
            End If
        Else
            no3DSecureCallPart2 = True
        End If
        Return err
    End Function

#End Region

#Region "Wrapper And Webservice Methods"

#Region " Session Request - Response "

    Private Function IsSessionIdentifierGenerated() As ErrorObj
        Dim err As New ErrorObj
        Try
            Dim msg As VanguardWebService.Message
            msg = GenerateSessionRequest()
            err = ProcessMessageResponseError(msg)
            If err.HasError Then
                _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                err.ErrorNumber = "TACTV-SE-001"
                err.ErrorMessage = "Error occured while requesting session identifier"
            Else
                err = ProcessSessionResponse(msg)
                If err.HasError Then
                    err.ErrorNumber = "TACTV-SE-002"
                    err.ErrorMessage = "Error occured while parsing session identifier"
                    _logMessage = err.ErrorNumber & ";" & err.ErrorMessage
                Else
                    'response is fine update tbl_basket_payment, insert basket status
                    err = InsertBasketStatus(StatusID.GENERATED_SESSION_IDENTIFIER, StatusComment.GENERATED_SESSION_IDENTIFIER)
                    If Not err.HasError Then
                        With BasketPayEntity
                            .BASKET_PAYMENT_ID = VanguardAttributes.BasketPaymentID
                            .BASKET_HEADER_ID = VanguardAttributes.BasketHeaderID
                            .PROCESSING_DB = VanguardAttributes.ProcessingDB
                            .SESSION_GUID = VanguardAttributes.SessionGUID
                            .SESSION_PASSCODE = VanguardAttributes.SessionPassCode
                            .STATUS = StatusID.GENERATED_SESSION_IDENTIFIER
                        End With
                        Dim affectedRows As Integer = TDataObjects.BasketSettings.TblBasketPayment.UpdateSessionDetail(BasketPayEntity)
                        If affectedRows <= 0 Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-SE-003"
                            err.ErrorMessage = "Failed updating session identifier to basket payment table"
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-SE-004"
            err.ErrorMessage = "Exception while generating session identifier"
            'log the error and put the custom error code which can be used by caller
            LogVanguardProcessError(err, ex, "IsSessionIdentifierGenerated")
        End Try
        Return err
    End Function

    Private Function GenerateSessionRequest() As VanguardWebService.Message
        Dim header As VanguardWebService.ClientHeader = GetVanguardClientHeader()
        header.ProcessingDB = "" 'To be kept blank for THIS method as per pdf
        header.SendAttempt = 0
        Dim msg As New VanguardWebService.Message
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        msg.ClientHeader = header
        msg.MsgType = "VGGENERATESESSIONREQUEST"
        msg.MsgData = String.Format(VGRequestXML.VGGENERATESESSIONREQUEST, VanguardAttributes.ReturnURL, VanguardAttributes.FullCapture.ToString.ToLower)
        msg = ProcessMsg(msg)
        Return msg
    End Function

    Private Function ProcessSessionResponse(ByVal message As VanguardWebService.Message) As ErrorObj
        Dim sessionResponse As XElement = XElement.Parse(message.MsgData)
        Dim ns As XNamespace = sessionResponse.GetDefaultNamespace().NamespaceName
        Dim err As New ErrorObj
        If sessionResponse.Element(ns + "errorcode") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(sessionResponse.Element(ns + "errorcode").Value) Then
            Try
                If Convert.ToDecimal(sessionResponse.Element(ns + "errorcode").Value) > 0 Then
                    err.HasError = True
                    err.ErrorNumber = sessionResponse.Element(ns + "errorcode").Value
                    If sessionResponse.Element(ns + "errormessage") IsNot Nothing Then
                        err.ErrorMessage = sessionResponse.Element(ns + "errormessage").Value
                    End If

                    Return err
                End If
            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = sessionResponse.Element(ns + "errorcode").Value
                err.ErrorMessage = ex.Message
                Return err
            End Try
        End If

        VanguardAttributes.ProcessingDB = message.ClientHeader.ProcessingDB
        VanguardAttributes.SessionGUID = sessionResponse.Element(ns + "sessionguid").Value ' To be stored in session?
        VanguardAttributes.SessionPassCode = sessionResponse.Element(ns + "sessionpasscode").Value
        Return err
    End Function

    Private Function GetFullCapture() As Boolean
        Return True
    End Function

    Private Function GetBasketStatusEntity() As DEBasketStatus
        Dim basketStatusEntity As New DEBasketStatus
        basketStatusEntity.BasketHeaderId = VanguardAttributes.BasketHeaderID
        basketStatusEntity.BusinessUnit = Settings.BusinessUnit
        basketStatusEntity.Partner = Settings.Partner
        basketStatusEntity.LoginId = Settings.LoginId
        basketStatusEntity.TempOrderId = VanguardAttributes.TempOrderID
        basketStatusEntity.ExternalOrderNumber = ""
        basketStatusEntity.GoogleSerialNumber = ""
        Return basketStatusEntity
    End Function

#End Region

#Region "Card Details Request - Response"

    ''' <summary>
    ''' Make the request to get the card details token and populate the basket pay entity.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsMaskedCardDetailsSaved() As ErrorObj
        Dim err As New ErrorObj
        Try
            Dim msg As VanguardWebService.Message
            msg = GetCardDetailsRequest()

            err = ProcessMessageResponseError(msg)
            If err.HasError Then
                _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                err.ErrorNumber = "TACTV-CD-002"
                err.ErrorMessage = "Error from vangaurd webservice while getting card details"
            Else
                Dim basketPaymentEntity As New DEBasketPayment
                err = ProcessCardDetailsResponse(msg, basketPaymentEntity)

                If err.HasError Then
                    _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                    err.ErrorNumber = "TACTV-CD-003"
                    err.ErrorMessage = "Error from vangaurd webservice while getting card details"
                Else
                    If err.ErrorNumber.Trim("0").Length = 0 AndAlso basketPaymentEntity IsNot Nothing Then
                        'card type validation is matched update the status with card detail otherwise set status for card mismatch
                        If basketPaymentEntity.CARD_TYPE.ToUpper <> VanguardAttributes.CardType.ToUpper AndAlso VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.CHECKOUT Then
                            'todo card type not matched update the status and throw error
                            err.HasError = True
                            err.ErrorNumber = "TACTV-CD-004"
                            err.ErrorMessage = "card type mismatch"
                            _logMessage = _logMessage & "Chosen card type:" & VanguardAttributes.CardType & ";" & "Given card type:" & basketPaymentEntity.CARD_TYPE
                        Else
                            'card type matched move all the detail to basket payment
                            basketPaymentEntity.STATUS = StatusID.CARD_TYPE_MATCHED
                            basketPaymentEntity.BASKET_PAYMENT_ID = VanguardAttributes.BasketPaymentID
                            Dim affectedRows As Integer = TDataObjects.BasketSettings.TblBasketPayment.UpdateCardDetail(basketPaymentEntity)
                            If affectedRows <= 0 Then
                                err.HasError = True
                                err.ErrorNumber = "TACTV-CD-005"
                                err.ErrorMessage = "Error while inserting card detail to basket payment"
                            Else
                                err = InsertBasketStatus(StatusID.CARD_TYPE_MATCHED, StatusComment.CARD_TYPE_MATCHED)
                                If Not err.HasError Then
                                    BasketPayEntity.BASKET_HEADER_ID = VanguardAttributes.BasketHeaderID
                                    BasketPayEntity.BASKET_PAYMENT_ID = VanguardAttributes.BasketPaymentID
                                    BasketPayEntity.PROCESSING_DB = VanguardAttributes.ProcessingDB
                                    BasketPayEntity.SESSION_GUID = VanguardAttributes.SessionGUID
                                    BasketPayEntity.SESSION_PASSCODE = VanguardAttributes.SessionPassCode
                                    BasketPayEntity.PAYMENT_TYPE = VanguardAttributes.PaymentType
                                    BasketPayEntity.STATUS = basketPaymentEntity.STATUS
                                    BasketPayEntity.PAYMENT_AMOUNT = VanguardAttributes.PaymentAmount
                                    BasketPayEntity.CARD_TYPE = basketPaymentEntity.CARD_TYPE
                                    BasketPayEntity.CARDNUMBER = basketPaymentEntity.CARDNUMBER
                                    BasketPayEntity.EXPIRYMONTH = basketPaymentEntity.EXPIRYMONTH
                                    BasketPayEntity.EXPIRYYEAR = basketPaymentEntity.EXPIRYYEAR
                                    BasketPayEntity.STARTMONTH = basketPaymentEntity.STARTMONTH
                                    BasketPayEntity.STARTYEAR = basketPaymentEntity.STARTYEAR
                                    BasketPayEntity.ADDRESS_LINE_1 = basketPaymentEntity.ADDRESS_LINE_1
                                    BasketPayEntity.POST_CODE = basketPaymentEntity.POST_CODE
                                    BasketPayEntity.ISSUENUMBER = basketPaymentEntity.ISSUENUMBER
                                    If String.IsNullOrWhiteSpace(BasketPayEntity.ADDRESS_LINE_1) Then
                                        BasketPayEntity.ADDRESS_LINE_1 = VanguardAttributes.AddressLine1
                                    End If
                                    If String.IsNullOrWhiteSpace(BasketPayEntity.POST_CODE) Then
                                        BasketPayEntity.POST_CODE = VanguardAttributes.Postcode
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-CD-001"
            err.ErrorMessage = "Exception while getting card details"
            'log the error and put the custom error code which can be used by caller
            LogVanguardProcessError(err, ex, "IsMaskedCardDetailsSaved")
        End Try
        Return err
    End Function

    Private Function GetCardDetailsRequest() As VanguardWebService.Message
        Dim msg As New VanguardWebService.Message
        msg.MsgType = "VGGETCARDDETAILSREQUEST"
        msg.ClientHeader = GetVanguardClientHeader()
        msg.MsgData = String.Format(VGRequestXML.VGGETCARDDETAILSREQUEST, VanguardAttributes.SessionGUID)
        msg = ProcessMsg(msg)
        Return msg
    End Function

    Private Function ProcessCardDetailsResponse(ByVal message As VanguardWebService.Message, ByRef basketPaymentEntity As DEBasketPayment) As ErrorObj
        'todo log the response received before process it
        Dim cardDetailResponse As XElement = Nothing
        Dim ns As XNamespace = Nothing
        Dim err As New ErrorObj
        Dim mkCardSchemeID As String = String.Empty
        Dim cardSchemeName As String = String.Empty
        Dim expirydate As String = String.Empty
        Dim startdate As String = String.Empty

        Try
            cardDetailResponse = XElement.Parse(message.MsgData)
            ns = cardDetailResponse.GetDefaultNamespace().NamespaceName
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '' STEP 1. If there is any error received from VanGuard then log it & return out of the function 
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If cardDetailResponse.Element(ns + "errorcode") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cardDetailResponse.Element(ns + "errorcode").Value) Then
                If Convert.ToDecimal(cardDetailResponse.Element(ns + "errorcode").Value) > 0 Then
                    err.HasError = True
                    err.ErrorNumber = cardDetailResponse.Element(ns + "errorcode").Value
                    If cardDetailResponse.Element(ns + "errordescription") IsNot Nothing Then   'The PDF says 'errormessage' whereas the actual response has 'errordescription' and thats what has been used here.
                        err.ErrorMessage = cardDetailResponse.Element(ns + "errordescription").Value
                    End If
                    Return err
                End If
            End If
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '' STEP 2. Check if retrieved sessionGUID/basketPaymentID is matched with what`s in memory   
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            err = VerifyTransactionResponseParams(cardDetailResponse, ns)
            If Not err.HasError Then
                If cardDetailResponse.Element(ns + "mkcardschemeid") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cardDetailResponse.Element(ns + "mkcardschemeid").Value) Then
                    mkCardSchemeID = cardDetailResponse.Element(ns + "mkcardschemeid").Value
                End If
                If cardDetailResponse.Element(ns + "schemename") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cardDetailResponse.Element(ns + "schemename").Value) Then
                    cardSchemeName = cardDetailResponse.Element(ns + "schemename").Value
                End If
                basketPaymentEntity.CARD_TYPE = GetCardTypeCode(mkCardSchemeID, cardSchemeName)

                If cardDetailResponse.Element(ns + "panstar") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cardDetailResponse.Element(ns + "panstar").Value) Then
                    basketPaymentEntity.CARDNUMBER = cardDetailResponse.Element(ns + "panstar").Value
                End If

                If cardDetailResponse.Element(ns + "expirydate") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cardDetailResponse.Element(ns + "expirydate").Value) Then
                    expirydate = cardDetailResponse.Element(ns + "expirydate").Value
                    If expirydate.Trim.Length >= 4 Then
                        basketPaymentEntity.EXPIRYMONTH = expirydate.Substring(0, 2)
                        basketPaymentEntity.EXPIRYYEAR = expirydate.Substring(2, 2)
                    End If
                End If

                If cardDetailResponse.Element(ns + "startdate") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cardDetailResponse.Element(ns + "startdate").Value) Then
                    startdate = cardDetailResponse.Element(ns + "startdate").Value
                    If startdate.Trim.Length >= 4 Then
                        basketPaymentEntity.STARTMONTH = startdate.Substring(0, 2)
                        basketPaymentEntity.STARTYEAR = startdate.Substring(2, 2)
                    End If
                End If

                If cardDetailResponse.Element(ns + "issueno") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cardDetailResponse.Element(ns + "issueno").Value) Then
                    basketPaymentEntity.ISSUENUMBER = cardDetailResponse.Element(ns + "issueno").Value
                End If

                If cardDetailResponse.Element(ns + "address1") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cardDetailResponse.Element(ns + "address1").Value) Then
                    basketPaymentEntity.ADDRESS_LINE_1 = cardDetailResponse.Element(ns + "address1").Value
                End If

                If cardDetailResponse.Element(ns + "postcode") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cardDetailResponse.Element(ns + "postcode").Value) Then
                    basketPaymentEntity.POST_CODE = cardDetailResponse.Element(ns + "postcode").Value
                End If

                If cardDetailResponse.Element(ns + "cardholdername") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cardDetailResponse.Element(ns + "cardholdername").Value) Then
                    basketPaymentEntity.CARD_HOLDER_NAME = cardDetailResponse.Element(ns + "cardholdername").Value
                End If
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = cardDetailResponse.Element(ns + "errorcode").Value
            err.ErrorMessage = ex.Message
            Return err
        Finally
            cardDetailResponse = Nothing
            ns = Nothing
            mkCardSchemeID = Nothing
            cardSchemeName = Nothing
            expirydate = Nothing
            startdate = Nothing
        End Try
        '''''''''''''''''''''''''''''''''''''''''''''''
        ' RETURN 
        '''''''''''''''''''''''''''''''''''''''''''''''
        Return err
    End Function

#End Region

#Region "PayerAuthEnrollment Request - Response"

    Private Function IsPayerAuthEnrollmentVerified() As ErrorObj
        Dim err As New ErrorObj
        Try
            Dim msg As VanguardWebService.Message
            SetTxtnAmtAndDisplay()
            If VanguardAttributes.PaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE Then
                msg = GetPayerAuthEnrollmentCheckRequest_NS()
            Else
                msg = GetPayerAuthEnrollmentCheckRequest()
            End If

            err = ProcessMessageResponseError(msg)
            If err.HasError Then
                _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                err.ErrorNumber = "TACTV-IPAV-001"
                err.ErrorMessage = "Error in PayerAuthEnrollmentCheckRequest."
            Else
                If VanguardAttributes.PaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE Then
                    err = ProcessPayerAuthEnrollmentCheckResponse_NS(msg)
                Else
                    err = ProcessPayerAuthEnrollmentCheckResponse(msg)
                End If
                If err.HasError Then
                    _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                    err.HasError = True
                    err.ErrorNumber = "TACTV-IPAV-002"
                    err.ErrorMessage = "Error while processing PayerAuthEnrollmentCheckResponse."
                Else
                    err = InsertBasketStatus(StatusID.PAYERAUTHENROLLMENTCHECK, StatusComment.PAYERAUTHENROLLMENTCHECK_END)
                End If
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-IPAV-004"
            err.ErrorMessage = "Exception while checking PayerAuthEnrollment"
            'log the error and put the custom error code which can be used by caller
            LogVanguardProcessError(err, ex, "IsPayerAuthEnrollmentVerified")
        End Try

        Return err
    End Function

    Private Function GetPayerAuthEnrollmentCheckRequest() As VanguardWebService.Message
        Dim msg As New VanguardWebService.Message
        msg.MsgType = "VGPAYERAUTHENROLLMENTCHECKREQUEST"
        msg.ClientHeader = GetVanguardClientHeader()
        msg.MsgData = String.Format(VGRequestXML.VGPAYERAUTHENROLLMENTCHECKREQUEST,
                                     VanguardAttributes.SessionGUID,
                                     GetMerchantReference(VanguardAttributes.BasketPaymentID),
                                     VanguardAttributes.MKAccountID,
                                     VanguardAttributes.MKAcquirerId,
                                     VanguardAttributes.MerchantName,
                                     VanguardAttributes.TerminalCountryCode,
                                     VanguardAttributes.MerchantUrl,
                                     VanguardAttributes.VisaMerchantBankId,
                                     VanguardAttributes.VisaMerchantNumber,
                                     VanguardAttributes.VisaMerchantPassword,
                                     VanguardAttributes.MCMMerchantBankId,
                                     VanguardAttributes.MCMMerchantNumber,
                                     VanguardAttributes.MCMMerchantPassword,
                                     BasketPayEntity.EXPIRYDATE,
                                     VanguardAttributes.TransactionCurrencyCode,
                                     VanguardAttributes.CurrencyExponent,
                                     VanguardAttributes.BrowserAcceptHeader,
                                     VanguardAttributes.BrowserUserAgentHeader,
                                     VanguardAttributes.TransactionAmount,
                                     VanguardAttributes.TransactionDisplayAmount,
                                     VanguardAttributes.TransactionDescription)
        msg = ProcessMsg(msg)
        Return msg
    End Function

    Private Function GetPayerAuthEnrollmentCheckRequest_NS() As Message
        Dim msg As New VanguardWebService.Message
        msg.MsgType = "PAI"
        msg.ClientHeader = GetVanguardClientHeader()
        msg.MsgData = String.Format(VGRequestXML.VGPAYERAUTHENROLLMENTCHECKREQUEST_NS,
                                        GetMerchantReference(VanguardAttributes.BasketPaymentID),
                                        VanguardAttributes.MKAccountID,
                                        VanguardAttributes.MKAcquirerId,
                                        VanguardAttributes.MerchantName,
                                        VanguardAttributes.TerminalCountryCode,
                                        VanguardAttributes.MerchantUrl,
                                        VanguardAttributes.VisaMerchantBankId,
                                        VanguardAttributes.VisaMerchantNumber,
                                        VanguardAttributes.VisaMerchantPassword,
                                        VanguardAttributes.MCMMerchantBankId,
                                        VanguardAttributes.MCMMerchantNumber,
                                        VanguardAttributes.MCMMerchantPassword,
                                        BasketPayEntity.TOKENID,
                                        BasketPayEntity.CARDNUMBER,
                                        VanguardAttributes.ExpiryYear,
                                        VanguardAttributes.ExpiryMonth,
                                        VanguardAttributes.TransactionCurrencyCode,
                                        VanguardAttributes.CurrencyExponent,
                                        VanguardAttributes.TransactionAmount,
                                        VanguardAttributes.TransactionDisplayAmount)

        msg = ProcessMsg(msg)
        Return msg
    End Function

    Private Function ProcessPayerAuthEnrollmentCheckResponse(ByVal message As Message) As ErrorObj
        Dim payerAuthEnrollmentResponse As XElement = Nothing
        Dim ns As XNamespace = Nothing
        Dim err As New ErrorObj

        payerAuthEnrollmentResponse = XElement.Parse(message.MsgData)
        ns = payerAuthEnrollmentResponse.GetDefaultNamespace().NamespaceName
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' STEP 1. Check Vanguard response message and see if there is any error occurred 
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If payerAuthEnrollmentResponse.Element(ns + "errorcode") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(payerAuthEnrollmentResponse.Element(ns + "errorcode").Value) Then
            Try
                If Convert.ToDecimal(payerAuthEnrollmentResponse.Element(ns + "errorcode").Value) > 0 Then
                    err.HasError = True
                    err.ErrorNumber = payerAuthEnrollmentResponse.Element(ns + "errorcode").Value

                    If payerAuthEnrollmentResponse.Element(ns + "errordescription") IsNot Nothing Then
                        err.ErrorMessage = payerAuthEnrollmentResponse.Element(ns + "errordescription").Value
                    End If

                    Return err
                End If
            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = payerAuthEnrollmentResponse.Element(ns + "errorcode").Value
                err.ErrorMessage = ex.Message
                Return err
            End Try
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' STEP 2. There is no error occurred; check other fields
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        err = VerifyTransactionResponseParams(payerAuthEnrollmentResponse, ns)
        If Not err.HasError Then
            VanguardAttributes.PayerAuth().PayerAuthRequestID = Convert.ToDecimal(payerAuthEnrollmentResponse.Element(ns + "payerauthrequestid").Value)

            If payerAuthEnrollmentResponse.Element(ns + "enrolled") IsNot Nothing Then
                VanguardAttributes.PayerAuth().Enrolled = payerAuthEnrollmentResponse.Element(ns + "enrolled").Value.ToUpper()
            End If

            If payerAuthEnrollmentResponse.Element(ns + "acsurl") IsNot Nothing Then
                VanguardAttributes.PayerAuth().AcsUrl = payerAuthEnrollmentResponse.Element(ns + "acsurl").Value
            End If

            If payerAuthEnrollmentResponse.Element(ns + "pareq") IsNot Nothing Then
                VanguardAttributes.PayerAuth().PareQ = payerAuthEnrollmentResponse.Element(ns + "pareq").Value
            End If
        End If

        ''''''''''''''''''''''''''''''''''''''
        'RETURN 
        ''''''''''''''''''''''''''''''''''''''
        Return err
    End Function

    Private Function ProcessPayerAuthEnrollmentCheckResponse_NS(ByVal message As Message) As ErrorObj

        Dim payerAuthEnrollmentResponse As XElement = Nothing
        Dim ns As XNamespace = Nothing
        Dim err As New ErrorObj

        payerAuthEnrollmentResponse = XElement.Parse(message.MsgData)
        ns = payerAuthEnrollmentResponse.GetDefaultNamespace().NamespaceName
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' STEP 1. Check Vanguard response message and see if there is any error occurred 
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If payerAuthEnrollmentResponse.Element(ns + "errorcode") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(payerAuthEnrollmentResponse.Element(ns + "errorcode").Value) Then
            Try
                If Convert.ToDecimal(payerAuthEnrollmentResponse.Element(ns + "errorcode").Value) > 0 Then
                    err.HasError = True
                    err.ErrorNumber = payerAuthEnrollmentResponse.Element(ns + "errorcode").Value

                    If payerAuthEnrollmentResponse.Element(ns + "errordescription") IsNot Nothing Then
                        err.ErrorMessage = payerAuthEnrollmentResponse.Element(ns + "errordescription").Value
                    End If
                    Return err
                End If
            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = payerAuthEnrollmentResponse.Element(ns + "errorcode").Value
                err.ErrorMessage = ex.Message
                Return err
            End Try
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' STEP 2. There is no error occurred; check other fields
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        err = VerifyTransactionResponseParams_NS(payerAuthEnrollmentResponse, ns)
        If Not err.HasError Then

            Dim processingdb As String
            If payerAuthEnrollmentResponse.Element(ns + "processingdb") IsNot Nothing Then
                processingdb = payerAuthEnrollmentResponse.Element(ns + "processingdb").Value
            End If

            VanguardAttributes.PayerAuth().PayerAuthRequestID = Convert.ToDecimal(payerAuthEnrollmentResponse.Element(ns + "payerauthrequestid").Value)

            If payerAuthEnrollmentResponse.Element(ns + "enrolled") IsNot Nothing Then
                VanguardAttributes.PayerAuth().Enrolled = payerAuthEnrollmentResponse.Element(ns + "enrolled").Value.ToUpper()
            End If

            If payerAuthEnrollmentResponse.Element(ns + "acsurl") IsNot Nothing Then
                VanguardAttributes.PayerAuth().AcsUrl = payerAuthEnrollmentResponse.Element(ns + "acsurl").Value
            End If

            If payerAuthEnrollmentResponse.Element(ns + "pareq") IsNot Nothing Then
                VanguardAttributes.PayerAuth().PareQ = payerAuthEnrollmentResponse.Element(ns + "pareq").Value
            End If
        End If

        ''''''''''''''''''''''''''''''''''''''
        'RETURN 
        ''''''''''''''''''''''''''''''''''''''
        Return err


    End Function

#End Region

#Region "PayerAuthAuthentication Request - Response"

    Private Function IsPayerAuthAuthenticationVerified() As ErrorObj
        Dim err As New ErrorObj
        Try
            Dim msg As VanguardWebService.Message
            If VanguardAttributes.PaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE Then
                msg = PayerAuthAuthenticationCheckRequest_NS()
            Else
                msg = PayerAuthAuthenticationCheckRequest()
            End If

            err = ProcessMessageResponseError(msg)
            If err.HasError Then
                _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                err.ErrorNumber = "TACTV-IPAAV-001"
                err.ErrorMessage = "Error in PayerAuthAuthenticationCheckRequest."
            Else
                If VanguardAttributes.PaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE Then
                    err = ProcessPayerAuthAuthenticationCheckResponse_NS(msg)
                Else
                    err = ProcessPayerAuthAuthenticationCheckResponse(msg)
                End If

                If err.HasError Then
                    _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                    err.HasError = True
                    err.ErrorNumber = "TACTV-IPAAV-002"
                    err.ErrorMessage = "Error while processing PayerAuthAuthenticationCheckResponse."
                ElseIf VanguardAttributes.PayerAuth().IsEnrolled Then
                    If VanguardAttributes.PayerAuth().AuthenticationStatus = "N" Then
                        'Only reject the authentication status = "N" when the user is enrolled into 3D Secure
                        err.HasError = True
                        err.ErrorNumber = "TACTV-IPAAV-003a"
                        err.ErrorMessage = "authenticationstatus = 'N', Rejecting Transaction"
                        _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                    Else
                        'Check the authentication status against the ECommerce Module Defaults
                        Dim transactionOk As Boolean = False
                        For Each code As String In Settings.EcommerceModuleDefaultsValues.Payment3DSecureSuccessCodes
                            If code = VanguardAttributes.PayerAuth().AuthenticationStatus Then
                                transactionOk = True
                            End If
                        Next
                        If transactionOk Then
                            err = InsertBasketStatus(StatusID.PAYERAUTHENTICATIONCHECK, StatusComment.PAYERAUTHENTICATIONCHECK_END)
                        Else
                            err.HasError = True
                            err.ErrorNumber = "TACTV-IPAAV-003b"
                            err.ErrorMessage = "authenticationstatus = '" & VanguardAttributes.PayerAuth().AuthenticationStatus & "', Rejecting Transaction"
                            _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                        End If
                    End If
                Else
                    err = InsertBasketStatus(StatusID.PAYERAUTHENTICATIONCHECK, StatusComment.PAYERAUTHENTICATIONCHECK_END)
                End If
            End If

        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-IPAV-004"
            err.ErrorMessage = "Exception while checking PayerAuthentication."
            LogVanguardProcessError(err, ex, "IsPayerAuthAuthenticationVerified")
        End Try

        Return err
    End Function

    Private Function PayerAuthAuthenticationCheckRequest() As Message
        Dim msg As New VanguardWebService.Message
        msg.MsgType = "VGPAYERAUTHAUTHENTICATIONCHECKREQUEST"
        msg.ClientHeader = GetVanguardClientHeader()
        msg.MsgData = String.Format(VGRequestXML.VGPAYERAUTHAUTHENTICATIONCHECKREQUEST,
                                    VanguardAttributes.SessionGUID,
                                    GetMerchantReference(VanguardAttributes.BasketPaymentID),
                                    VanguardAttributes.PayerAuth.PayerAuthRequestID,
                                    VanguardAttributes.PayerAuth.PareS,
                                    VanguardAttributes.PayerAuth.Enrolled)

        msg = ProcessMsg(msg)
        Return msg
    End Function

    Private Function PayerAuthAuthenticationCheckRequest_NS() As Message
        Dim msg As New VanguardWebService.Message
        msg.MsgType = "PAI"
        msg.ClientHeader = GetVanguardClientHeader()
        msg.MsgData = String.Format(VGRequestXML.VGPAYERAUTHAUTHENTICATIONCHECKREQUEST_NS,
                                    GetMerchantReference(VanguardAttributes.BasketPaymentID),
                                    VanguardAttributes.PayerAuth.PayerAuthRequestID,
                                    VanguardAttributes.PayerAuth.PareS,
                                    VanguardAttributes.PayerAuth.Enrolled)
        msg = ProcessMsg(msg)
        Return msg
    End Function

    Private Function ProcessPayerAuthAuthenticationCheckResponse(ByVal message As Message) As ErrorObj
        Dim err As New ErrorObj
        Dim payerAuthenticationCheckResponse As XElement = Nothing
        Dim ns As XNamespace = Nothing

        payerAuthenticationCheckResponse = XElement.Parse(message.MsgData)
        ns = payerAuthenticationCheckResponse.GetDefaultNamespace().NamespaceName

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' STEP 1. Check to see if an error has occurred 
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If payerAuthenticationCheckResponse.Element(ns + "errorcode") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(payerAuthenticationCheckResponse.Element(ns + "errorcode").Value) Then
            Try
                If Convert.ToDecimal(payerAuthenticationCheckResponse.Element(ns + "errorcode").Value) > 0 Then
                    err.HasError = True
                    err.ErrorNumber = payerAuthenticationCheckResponse.Element(ns + "errorcode").Value

                    If payerAuthenticationCheckResponse.Element(ns + "errordescription") IsNot Nothing Then
                        err.ErrorMessage = payerAuthenticationCheckResponse.Element(ns + "errordescription").Value
                    End If

                    Return err
                End If
            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = payerAuthenticationCheckResponse.Element(ns + "errorcode").Value
                err.ErrorMessage = ex.Message
                Return err
            End Try

        End If

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' STEP 2. If error occurred log it & throw it OR read the response parameters sent by Vanguard 
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        err = VerifyTransactionResponseParams(payerAuthenticationCheckResponse, ns)
        If Not err.HasError Then
            VanguardAttributes.PayerAuth.PayerAuthRequestID = Convert.ToDecimal(payerAuthenticationCheckResponse.Element(ns + "payerauthrequestid").Value)
            If payerAuthenticationCheckResponse.Element(ns + "atsdata") IsNot Nothing Then
                VanguardAttributes.PayerAuth().ATSData = payerAuthenticationCheckResponse.Element(ns + "atsdata").Value
            End If
            If payerAuthenticationCheckResponse.Element(ns + "authenticationstatus") IsNot Nothing Then
                VanguardAttributes.PayerAuth().AuthenticationStatus = payerAuthenticationCheckResponse.Element(ns + "authenticationstatus").Value
            End If
            If payerAuthenticationCheckResponse.Element(ns + "authenticationcertificate") IsNot Nothing Then
                VanguardAttributes.PayerAuth().AuthenticationCertificate = payerAuthenticationCheckResponse.Element(ns + "authenticationcertificate").Value
            End If

            If payerAuthenticationCheckResponse.Element(ns + "authenticationcavv") IsNot Nothing Then
                VanguardAttributes.PayerAuth().AuthenticationCAVV = payerAuthenticationCheckResponse.Element(ns + "authenticationcavv").Value
            End If

            If payerAuthenticationCheckResponse.Element(ns + "authenticationeci") IsNot Nothing Then
                VanguardAttributes.PayerAuth().AuthenticationeCI = payerAuthenticationCheckResponse.Element(ns + "authenticationeci").Value
            End If

            If payerAuthenticationCheckResponse.Element(ns + "authenticationtime") IsNot Nothing Then
                VanguardAttributes.PayerAuth().AuthenticationTime = payerAuthenticationCheckResponse.Element(ns + "authenticationtime")
            End If
        End If

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' RETURN 
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return err
    End Function

    Private Function ProcessPayerAuthAuthenticationCheckResponse_NS(ByVal message As Message) As ErrorObj
        Dim err As New ErrorObj
        Dim payerAuthenticationCheckResponse As XElement = Nothing
        Dim ns As XNamespace = Nothing
        payerAuthenticationCheckResponse = XElement.Parse(message.MsgData)
        ns = payerAuthenticationCheckResponse.GetDefaultNamespace().NamespaceName

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' STEP 1. Check to see if an error has occurred 
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If payerAuthenticationCheckResponse.Element(ns + "errorcode") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(payerAuthenticationCheckResponse.Element(ns + "errorcode").Value) Then
            Try
                If Convert.ToDecimal(payerAuthenticationCheckResponse.Element(ns + "errorcode").Value) > 0 Then
                    err.HasError = True
                    err.ErrorNumber = payerAuthenticationCheckResponse.Element(ns + "errorcode").Value

                    If payerAuthenticationCheckResponse.Element(ns + "errordescription") IsNot Nothing Then
                        err.ErrorMessage = payerAuthenticationCheckResponse.Element(ns + "errordescription").Value
                    End If

                    Return err
                End If
            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = payerAuthenticationCheckResponse.Element(ns + "errorcode").Value
                err.ErrorMessage = ex.Message
                Return err
            End Try

        End If
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' STEP 2. If error occurred log it & throw it OR read the response parameters sent by Vanguard 
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        err = VerifyTransactionResponseParams_NS(payerAuthenticationCheckResponse, ns)
        If Not err.HasError Then
            VanguardAttributes.PayerAuth.PayerAuthRequestID = Convert.ToDecimal(payerAuthenticationCheckResponse.Element(ns + "payerauthrequestid").Value)
            If payerAuthenticationCheckResponse.Element(ns + "authenticationstatus") IsNot Nothing Then
                VanguardAttributes.PayerAuth().AuthenticationStatus = payerAuthenticationCheckResponse.Element(ns + "authenticationstatus").Value
            End If
            If payerAuthenticationCheckResponse.Element(ns + "authenticationcertificate") IsNot Nothing Then
                VanguardAttributes.PayerAuth().AuthenticationCertificate = payerAuthenticationCheckResponse.Element(ns + "authenticationcertificate").Value
            End If
            If payerAuthenticationCheckResponse.Element(ns + "authenticationcavv") IsNot Nothing Then
                VanguardAttributes.PayerAuth().AuthenticationCAVV = payerAuthenticationCheckResponse.Element(ns + "authenticationcavv").Value
            End If
            If payerAuthenticationCheckResponse.Element(ns + "authenticationeci") IsNot Nothing Then
                VanguardAttributes.PayerAuth().AuthenticationeCI = payerAuthenticationCheckResponse.Element(ns + "authenticationeci").Value
            End If
            If payerAuthenticationCheckResponse.Element(ns + "authenticationtime") IsNot Nothing Then
                VanguardAttributes.PayerAuth().AuthenticationTime = payerAuthenticationCheckResponse.Element(ns + "authenticationtime")
            End If
            If payerAuthenticationCheckResponse.Element(ns + "atsdata") IsNot Nothing Then
                VanguardAttributes.PayerAuth().ATSData = payerAuthenticationCheckResponse.Element(ns + "atsdata").Value
            End If
            Dim processingdb As String
            If payerAuthenticationCheckResponse.Element(ns + "processingdb") IsNot Nothing Then
                processingdb = payerAuthenticationCheckResponse.Element(ns + "processingdb").Value
            End If
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' RETURN 
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Return err
    End Function

#End Region

#Region "Token Registered Request - Response"

    Private Function IsTokenRegistered() As ErrorObj
        Dim err As New ErrorObj
        Try
            Dim msg As Message
            msg = GetTokenRegistrationRequest()
            err = ProcessMessageResponseError(msg)
            If err.HasError Then
                _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                err.ErrorNumber = "TACTV-TR-001"
                err.ErrorMessage = "Error while processing response for token registration"
            Else
                BasketPayEntity.BASKET_PAYMENT_ID = VanguardAttributes.BasketPaymentID
                err = ProcessTokenRegistrationResponse(msg, BasketPayEntity)
                If err.HasError Then
                    _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                    err.HasError = True
                    err.ErrorNumber = "TACTV-TR-002"
                    err.ErrorMessage = "Error while processing response for token registration"
                Else
                    If err.ErrorNumber.Trim("0").Length = 0 AndAlso BasketPayEntity IsNot Nothing Then
                        BasketPayEntity.STATUS = StatusID.CARD_TOKENISATION
                        Dim affectedRows As Integer = TDataObjects.BasketSettings.TblBasketPayment.UpdateTokenID(BasketPayEntity)
                        If affectedRows <= 0 Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-TR-003"
                            err.ErrorMessage = "Error while inserting token id to basket payment"
                        Else
                            err = InsertBasketStatus(StatusID.CARD_TOKENISATION, StatusComment.CARD_TOKENISATION_END)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-TR-004"
            err.ErrorMessage = "Exception while registering token"
            'log the error and put the custom error code which can be used by caller
            LogVanguardProcessError(err, ex, "IsTokenRegistered")
        End Try
        Return err
    End Function

    Private Function GetTokenRegistrationRequest() As Message
        Dim tokenexpirationdate As String = System.DateTime.DaysInMonth("20" & BasketPayEntity.EXPIRYYEAR, BasketPayEntity.EXPIRYMONTH).ToString & BasketPayEntity.EXPIRYMONTH & "20" & BasketPayEntity.EXPIRYYEAR
        Dim msg As New VanguardWebService.Message
        msg.MsgType = "VGTOKENREGISTRATIONREQUEST"
        msg.ClientHeader = GetVanguardClientHeader()
        msg.MsgData = String.Format(VGRequestXML.VGTOKENREGISTRATIONREQUEST,
                                   VanguardAttributes.SessionGUID, GetMerchantReference(VanguardAttributes.BasketPaymentID), BasketPayEntity.EXPIRYDATE, "", "", "true", "true", "true", tokenexpirationdate)
        msg = ProcessMsg(msg)
        Return msg
    End Function

    Private Function ProcessTokenRegistrationResponse(ByVal message As Message, ByRef basketPaymentEntity As DEBasketPayment) As ErrorObj
        Dim err As New ErrorObj
        Dim tokenRegistrationResponse As XElement = XElement.Parse(message.MsgData)
        Dim ns As XNamespace = tokenRegistrationResponse.GetDefaultNamespace().NamespaceName

        Try
            If tokenRegistrationResponse.Element(ns + "errorcode") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(tokenRegistrationResponse.Element(ns + "errorcode").Value) Then
                If Convert.ToDecimal(tokenRegistrationResponse.Element(ns + "errorcode").Value) > 0 Then
                    err.HasError = True
                    err.ErrorNumber = tokenRegistrationResponse.Element(ns + "errorcode").Value
                    If tokenRegistrationResponse.Element(ns + "errordescription") IsNot Nothing Then
                        err.ErrorMessage = tokenRegistrationResponse.Element(ns + "errordescription").Value
                    End If
                    Return err
                End If
            End If
            err = VerifyTransactionResponseParams(tokenRegistrationResponse, ns)
            If Not err.HasError Then
                If tokenRegistrationResponse.Element(ns + "tokenid") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(tokenRegistrationResponse.Element(ns + "tokenid").Value) Then
                    basketPaymentEntity.TOKENID = tokenRegistrationResponse.Element(ns + "tokenid").Value
                End If
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = tokenRegistrationResponse.Element(ns + "errorcode").Value
            err.ErrorMessage = ex.Message
        Finally
            tokenRegistrationResponse = Nothing
            ns = Nothing
        End Try
        Return err
    End Function

#End Region

#Region "Transaction Request - Response"

    Private Function IsTransactionRequestCompleted() As ErrorObj
        Dim err As New ErrorObj
        Dim msg As Message
        Dim affectedRows As Integer = 0
        Try
            If VanguardAttributes.PaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE Then
                msg = GetTransactionRequest_NS()
            Else
                msg = GetTransactionRequest()
            End If

            err = ProcessMessageResponseError(msg)
            If err.HasError Then
                _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                err.ErrorNumber = "TACTV-TXNR-001"
                err.ErrorMessage = "Error while processing response for transaction send request"
            Else
                If VanguardAttributes.PaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE Then
                    err = ProcessTransactionResponse_NS(CALLERMETHOD_TRANSACTION_SEND, msg)
                Else
                    err = ProcessTransactionResponse(CALLERMETHOD_TRANSACTION_SEND, msg)
                End If
                BasketPayEntity.BASKET_PAYMENT_ID = VanguardAttributes.BasketPaymentID
                If err.HasError Then
                    _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                    err.HasError = True
                    err.ErrorNumber = "TACTV-TXNR-002"
                    err.ErrorMessage = "Error while processing response for transaction send request"
                Else
                    If err.ErrorNumber.Trim("0").Length = 0 AndAlso BasketPayEntity IsNot Nothing Then
                        BasketPayEntity.MERCHANT_REFERENCE = GetMerchantReference(BasketPayEntity.BASKET_PAYMENT_ID)
                        BasketPayEntity.STATUS = StatusID.TRANSACTION_SEND
                        affectedRows = TDataObjects.BasketSettings.TblBasketPayment.UpdateStatus(BasketPayEntity.BASKET_PAYMENT_ID, StatusID.TRANSACTION_SEND, BasketPayEntity.MERCHANT_REFERENCE)
                        If affectedRows <= 0 Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-TXNR-003"
                            err.ErrorMessage = "Error while updating status to basket payment"
                        Else
                            err = InsertBasketStatus(StatusID.TRANSACTION_SEND, StatusComment.TRANSACTION_SEND_END)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-TR-004"
            err.ErrorMessage = "Exception while transaction request-response"
            'log the error and put the custom error code which can be used by caller
            LogVanguardProcessError(err, ex, "IsTransactionRequestCompleted")
        Finally
            msg = Nothing
            affectedRows = Nothing
        End Try
        Return err
    End Function

    Private Function GetTransactionRequest() As Message
        BasketPayEntity.BASKET_PAYMENT_ID = VanguardAttributes.BasketPaymentID
        BasketPayEntity.CAPTUREMETHOD = VanguardAttributes.CaptureMethod
        BasketPayEntity.TXNType = VanguardAttributes.TxnType
        BasketPayEntity.PAYMENT_AMOUNT = VanguardAttributes.PaymentAmount

        Dim payerauthauxiliarydata As String = String.Empty
        If VanguardAttributes.PayerAuth IsNot Nothing Then
            If String.IsNullOrWhiteSpace(VanguardAttributes.PayerAuth.PayerAuthRequestID) Then
                'None supporting card scheme, set the default values
                VanguardAttributes.PayerAuth.ATSData = Settings.EcommerceModuleDefaultsValues.Payment3DSecureDefaultATSData
                VanguardAttributes.PayerAuth.AuthenticationeCI = Settings.EcommerceModuleDefaultsValues.Payment3DSecureDefaultECI
            End If
            payerauthauxiliarydata = String.Format(VGRequestXML.VGPAYERAUTHAUXILARYDATA,
                                                   VanguardAttributes.PayerAuth.AuthenticationStatus,
                                                   VanguardAttributes.PayerAuth.AuthenticationCAVV,
                                                   VanguardAttributes.PayerAuth.AuthenticationeCI,
                                                   VanguardAttributes.PayerAuth.ATSData,
                                                   VanguardAttributes.PayerAuth.PayerAuthRequestID)
        End If
        
        Dim avsHouse As String = String.Empty
        Dim avsPostCode As String = String.Empty
        CVCAndAVSGetCardTypeSetting(BasketPayEntity.CARD_TYPE)
        If _cardTypeCvcAndAvs IsNot Nothing Then
            If _cardTypeCvcAndAvs.AVS_Addr_Enabled Then
                avsHouse = "<avshouse>" & GetHouseNumber(BasketPayEntity.ADDRESS_LINE_1) & "</avshouse>"
            End If
            If _cardTypeCvcAndAvs.AVS_PC_Enabled Then
                avsPostCode = "<avspostcode>" & GetNumeric(BasketPayEntity.POST_CODE) & "</avspostcode>"
            End If
        End If
        Dim msg As New VanguardWebService.Message
        msg.MsgType = "VGTRANSACTIONREQUEST"
        msg.ClientHeader = GetVanguardClientHeader()
        msg.MsgData = String.Format(VGRequestXML.VGTRANSACTIONREQUEST,
                                            VanguardAttributes.SessionGUID,
                                            GetMerchantReference(BasketPayEntity.BASKET_PAYMENT_ID),
                                            VanguardAttributes.AccountID,
                                            String.Format("{0:00}", CInt(BasketPayEntity.TXNType)),
                                            VanguardAttributes.TransactionCurrencyCode,
                                            VanguardAttributes.ApacsTerminalCapabilities,
                                            BasketPayEntity.CAPTUREMETHOD,
                                            CInt(VanguardAttributes.ProcessIdentifier),
                                            avsHouse,
                                            avsPostCode,
                                            BasketPayEntity.EXPIRYYEAR & BasketPayEntity.EXPIRYMONTH,
                                            BasketPayEntity.ISSUENUMBER,
                                            BasketPayEntity.STARTYEAR & BasketPayEntity.STARTMONTH,
                                            Math.Abs(BasketPayEntity.PAYMENT_AMOUNT),
                                            Date.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                                            VanguardAttributes.TerminalCountryCode,
                                            payerauthauxiliarydata,
                                            VanguardAttributes.AccountPasscode)
        msg = ProcessMsg(msg)
        Return msg
    End Function

    Private Function GetTransactionRequest_NS() As Message
        BasketPayEntity.BASKET_PAYMENT_ID = VanguardAttributes.BasketPaymentID
        BasketPayEntity.CAPTUREMETHOD = VanguardAttributes.CaptureMethod
        BasketPayEntity.TXNType = VanguardAttributes.TxnType
        BasketPayEntity.PAYMENT_AMOUNT = VanguardAttributes.PaymentAmount

        Dim avsHouse As String = String.Empty
        Dim avsPostCode As String = String.Empty
        CVCAndAVSGetCardTypeSetting(BasketPayEntity.CARD_TYPE)
        If _cardTypeCvcAndAvs IsNot Nothing Then
            If _cardTypeCvcAndAvs.AVS_Addr_Enabled Then
                avsHouse = "<avshouse>" & GetHouseNumber(BasketPayEntity.ADDRESS_LINE_1) & "</avshouse>"
            End If
            If _cardTypeCvcAndAvs.AVS_PC_Enabled Then
                avsPostCode = "<avspostcode>" & GetNumeric(BasketPayEntity.POST_CODE) & "</avspostcode>"
            End If
        End If
        Dim payerauthauxiliarydata As String = ""
        If VanguardAttributes.PayerAuth IsNot Nothing Then
            If String.IsNullOrWhiteSpace(VanguardAttributes.PayerAuth.PayerAuthRequestID) Then
                'None supporting card scheme, set the default values
                VanguardAttributes.PayerAuth.ATSData = Settings.EcommerceModuleDefaultsValues.Payment3DSecureDefaultATSData
                VanguardAttributes.PayerAuth.AuthenticationeCI = Settings.EcommerceModuleDefaultsValues.Payment3DSecureDefaultECI
            End If
            payerauthauxiliarydata = String.Format(VGRequestXML.VGPAYERAUTHAUXILARYDATA,
                                                   VanguardAttributes.PayerAuth.AuthenticationStatus,
                                                   VanguardAttributes.PayerAuth.AuthenticationCAVV,
                                                   VanguardAttributes.PayerAuth.AuthenticationeCI,
                                                   VanguardAttributes.PayerAuth.ATSData,
                                                   VanguardAttributes.PayerAuth.PayerAuthRequestID)
        End If
        Dim msg As New VanguardWebService.Message
        msg.MsgType = "TXN"
        msg.ClientHeader = GetVanguardClientHeader()
        msg.MsgData = String.Format(VGRequestXML.VGTRANSACTIONREQUEST_NS,
                                    GetMerchantReference(VanguardAttributes.BasketPaymentID),
                                    VanguardAttributes.AccountID,
                                    VanguardAttributes.AccountPasscode,
                                    String.Format("{0:00}", CInt(BasketPayEntity.TXNType)),
                                    VanguardAttributes.TransactionCurrencyCode,
                                    VanguardAttributes.TerminalCountryCode,
                                    VanguardAttributes.ApacsTerminalCapabilities,
                                    BasketPayEntity.CAPTUREMETHOD,
                                    CInt(VanguardAttributes.ProcessIdentifier),
                                    VanguardAttributes.SecurityNumber,
                                    avsHouse,
                                    avsPostCode,
                                    BasketPayEntity.TOKENID,
                                    Math.Abs(BasketPayEntity.PAYMENT_AMOUNT),
                                    payerauthauxiliarydata)
        msg = ProcessMsg(msg)
        Return msg
    End Function

    Private Function ProcessTransactionResponse(ByVal callerMethod As String, ByVal message As Message) As ErrorObj
        'Postcode, address line AVS and CVC configuartion are in CD00A get the value from there in VG300S or system default stored proc
        Dim transactionResponse As XElement = XElement.Parse(message.MsgData)
        Dim ns As XNamespace = transactionResponse.GetDefaultNamespace().NamespaceName

        Dim err As New ErrorObj
        VanguardAttributes.TransactionID = transactionResponse.Element(ns + "transactionid").Value

        If transactionResponse.Element(ns + "errormsg") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(transactionResponse.Element(ns + "errormsg").Value) Then
            Try
                'todo: set errornumber but what as it called from 3 methods
                err.HasError = True
                err.ErrorMessage = transactionResponse.Element(ns + "errormsg").Value
            Catch ex As Exception
                err.HasError = True
                err.ErrorMessage = ex.Message
            End Try
        End If

        If transactionResponse.Element(ns + "txnresult") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(transactionResponse.Element(ns + "txnresult").Value) Then
            If transactionResponse.Element(ns + "txnresult").Value = "ERROR" _
            OrElse transactionResponse.Element(ns + "txnresult").Value = "REFERRAL" _
            OrElse transactionResponse.Element(ns + "txnresult").Value = "COMMSDOWN" _
            OrElse transactionResponse.Element(ns + "txnresult").Value = "DECLINED" _
            OrElse transactionResponse.Element(ns + "txnresult").Value = "REJECTED" Then
                err.HasError = True
                err.ErrorMessage += " | " & transactionResponse.Element(ns + "txnresult").Value + " Response - Rejecting Transaction"
            End If
        End If

        If Not err.HasError Then
            err = VerifyTransactionResponseParams(transactionResponse, ns)
            If Not err.HasError Then
                If callerMethod = CALLERMETHOD_TRANSACTION_SEND Then
                    'verify the AVS and CVC
                    Dim pcavsresult As Integer = Convert.ToInt32(transactionResponse.Element(ns + "pcavsresult").Value)
                    Dim ad1avsresult As Integer = Convert.ToInt32(transactionResponse.Element(ns + "ad1avsresult").Value)
                    Dim cvcresult As Integer = Convert.ToInt32(transactionResponse.Element(ns + "cvcresult").Value)
                    err = CVCAndAVSValidate(ad1avsresult, pcavsresult, cvcresult)
                End If
            End If
        End If
        Return err
    End Function

    Private Function ProcessTransactionResponse_NS(ByVal callerMethod As String, ByVal message As Message) As ErrorObj
        Dim transactionResponse As XElement = XElement.Parse(message.MsgData)
        Dim ns As XNamespace = transactionResponse.GetDefaultNamespace().NamespaceName
        Dim err As New ErrorObj
        VanguardAttributes.TransactionID = transactionResponse.Element(ns + "transactionid").Value

        If transactionResponse.Element(ns + "errormsg") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(transactionResponse.Element(ns + "errormsg").Value) Then
            Try
                'todo: set errornumber but what as it called from 3 methods
                err.HasError = True
                err.ErrorMessage = transactionResponse.Element(ns + "errormsg").Value
            Catch ex As Exception
                err.HasError = True
                err.ErrorMessage = ex.Message
            End Try
        End If
        If transactionResponse.Element(ns + "txnresult") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(transactionResponse.Element(ns + "txnresult").Value) Then
            If transactionResponse.Element(ns + "txnresult").Value = "ERROR" _
            OrElse transactionResponse.Element(ns + "txnresult").Value = "REFERRAL" _
            OrElse transactionResponse.Element(ns + "txnresult").Value = "COMMSDOWN" _
            OrElse transactionResponse.Element(ns + "txnresult").Value = "DECLINED" _
            OrElse transactionResponse.Element(ns + "txnresult").Value = "REJECTED" Then
                err.HasError = True
                err.ErrorMessage += " | " & transactionResponse.Element(ns + "txnresult").Value + " Response - Rejecting Transaction"
            End If
        End If
        If Not err.HasError Then
            err = VerifyTransactionResponseParams_NS(transactionResponse, ns)
            If Not err.HasError Then
                If callerMethod = CALLERMETHOD_TRANSACTION_SEND Then
                    'verify the AVS and CVC
                    Dim pcavsresult As Integer = Convert.ToInt32(transactionResponse.Element(ns + "pcavsresult").Value)
                    Dim ad1avsresult As Integer = Convert.ToInt32(transactionResponse.Element(ns + "ad1avsresult").Value)
                    Dim cvcresult As Integer = Convert.ToInt32(transactionResponse.Element(ns + "cvcresult").Value)
                    err = CVCAndAVSValidate(ad1avsresult, pcavsresult, cvcresult)
                End If
            End If
        End If
        Return err
    End Function

    Private Function VerifyTransactionResponseParams(ByVal XResponseElement As XElement, ByVal XNameSpace As XNamespace) As ErrorObj
        Dim err As New ErrorObj
        Try
            ' CHECK SESSIONGUID
            If XResponseElement.Element(XNameSpace + "sessionguid").Value <> VanguardAttributes.SessionGUID Then
                err.HasError = True
                err.ErrorNumber = "TACTV-TR-002"
                err.ErrorMessage = "Transaction Response - SessionGUID is not matching."
                Return err
            End If
            ' CHECK MERCHANTREFERENCE   
            If Not IsNothing(XResponseElement.Element(XNameSpace + "merchantreference")) AndAlso _
                Not String.IsNullOrWhiteSpace(XResponseElement.Element(XNameSpace + "merchantreference").Value) AndAlso _
                Not IsValidMerchantReference(XResponseElement.Element(XNameSpace + "merchantreference").Value, VanguardAttributes.BasketPaymentID) Then
                err.HasError = True
                err.ErrorNumber = "TACTV-VTRP-003"
                err.ErrorMessage = "Transaction Response - BasketPaymentID is not matching."
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-TR-001"
            err.ErrorMessage = "Error occurred while verifying the reponse parameters"
        End Try
        Return err
    End Function

    Private Function VerifyTransactionResponseParams_NS(ByVal XResponseElement As XElement, ByVal XNameSpace As XNamespace) As ErrorObj
        Dim err As New ErrorObj
        Try
            ' CHECK MERCHANTREFERENCE   
            If Not IsNothing(XResponseElement.Element(XNameSpace + "merchantreference")) AndAlso _
                Not String.IsNullOrWhiteSpace(XResponseElement.Element(XNameSpace + "merchantreference").Value) AndAlso _
                  Not IsValidMerchantReference(XResponseElement.Element(XNameSpace + "merchantreference").Value, VanguardAttributes.BasketPaymentID) Then
                err.HasError = True
                err.ErrorNumber = "TACTV-VTRPNS-003"
                err.ErrorMessage = "Transaction Response - BasketPaymentID not matching."
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-TRNS-001"
            err.ErrorMessage = "Error occurred while verifying the reponse parameters"
        End Try
        Return err
    End Function

    Private Function CVCAndAVSValidate(ByVal avsResultAddr As Integer, ByVal avsResultPC As Integer, ByVal cvcResult As Integer) As ErrorObj
        Dim err As New ErrorObj

        If VanguardAttributes.TxnType <> DEVanguard.TransactionType.REFUND Then
            If _cardTypeCvcAndAvs IsNot Nothing Then
                If _cardTypeCvcAndAvs.CVCEnabled Then
                    If cvcResult = 0 AndAlso Not _cardTypeCvcAndAvs.CVCAccept_NotProvided Then
                        err.HasError = True
                        err.ErrorNumber = "TACTV-CVC-000"
                        err.ErrorMessage = "CVC not provided"
                    ElseIf cvcResult = 1 AndAlso Not _cardTypeCvcAndAvs.CVCAccept_NotChecked Then
                        err.HasError = True
                        err.ErrorNumber = "TACTV-CVC-001"
                        err.ErrorMessage = "CVC not checked"
                    ElseIf cvcResult = 2 AndAlso Not _cardTypeCvcAndAvs.CVCAccept_Matched Then
                        err.HasError = True
                        err.ErrorNumber = "TACTV-CVC-002"
                        err.ErrorMessage = "CVC matched"
                    ElseIf cvcResult = 4 AndAlso Not _cardTypeCvcAndAvs.CVCAccept_NotMatched Then
                        err.HasError = True
                        err.ErrorNumber = "TACTV-CVC-004"
                        err.ErrorMessage = "CVC not matched"
                    End If
                End If
                If Not err.HasError Then
                    If _cardTypeCvcAndAvs.AVS_Addr_Enabled Then
                        If avsResultAddr = 0 AndAlso Not _cardTypeCvcAndAvs.AVSAccept_Addr_NotProvided Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-ADDR-000"
                            err.ErrorMessage = "AVS AD1 not provided"
                        ElseIf avsResultAddr = 1 AndAlso Not _cardTypeCvcAndAvs.AVSAccept_Addr_NotChecked Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-ADDR-001"
                            err.ErrorMessage = "AVS AD1 not checked"
                        ElseIf avsResultAddr = 2 AndAlso Not _cardTypeCvcAndAvs.AVSAccept_Addr_Matched Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-ADDR-002"
                            err.ErrorMessage = "AVS AD1 matched"
                        ElseIf avsResultAddr = 4 AndAlso Not _cardTypeCvcAndAvs.AVSAccept_Addr_NotMatched Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-ADDR-004"
                            err.ErrorMessage = "AVS AD1 not matched"
                        ElseIf avsResultAddr = 8 AndAlso Not _cardTypeCvcAndAvs.AVSAccept_Addr_PartialMatch Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-ADDR-008"
                            err.ErrorMessage = "AVS AD1 partial match"
                        End If
                    End If
                End If

                If Not err.HasError Then
                    If _cardTypeCvcAndAvs.AVS_PC_Enabled Then
                        If avsResultPC = 0 AndAlso Not _cardTypeCvcAndAvs.AVSAccept_PC_NotProvided Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-PC-000"
                            err.ErrorMessage = "AVS PC not provided"
                        ElseIf avsResultPC = 1 AndAlso Not _cardTypeCvcAndAvs.AVSAccept_PC_NotChecked Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-PC-001"
                            err.ErrorMessage = "AVS PC not checked"
                        ElseIf avsResultPC = 2 AndAlso Not _cardTypeCvcAndAvs.AVSAccept_PC_Matched Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-PC-002"
                            err.ErrorMessage = "AVS PC matched"
                        ElseIf avsResultPC = 4 AndAlso Not _cardTypeCvcAndAvs.AVSAccept_PC_NotMatched Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-PC-004"
                            err.ErrorMessage = "AVS PC not matched"
                        ElseIf avsResultPC = 8 AndAlso Not _cardTypeCvcAndAvs.AVSAccept_PC_PartialMatch Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-PC-008"
                            err.ErrorMessage = "AVS PC partial match"
                        End If
                    End If
                End If

            End If
        End If

        Return err
    End Function

    Private Sub CVCAndAVSGetCardTypeSetting(ByVal cardType As String)
        _cardTypeCvcAndAvs = New DECVCAndAVSAuthorization
        _cardTypeCvcAndAvs = TDataObjects.PaymentSettings.TblCreditCard.GetCardTypeCVCAndAVS(cardType)
    End Sub


#End Region

#Region "Confirmation-Rejection Request"

    Private Function IsTransactionConfirmed() As ErrorObj
        Dim err As New ErrorObj
        Dim msg As Message
        Dim affectedRows As Integer = 0
        Try
            If VanguardAttributes.PaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE Then
                msg = ConfirmationRequest_NS()
            Else
                msg = ConfirmationRequest()
            End If
            err = ProcessMessageResponseError(msg)
            If err.HasError Then
                _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage

                err.ErrorNumber = "TACTV-ITC-001"
                err.ErrorMessage = "Error while processing response for transaction send request"
            Else
                If VanguardAttributes.PaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE Then
                    err = ProcessTransactionResponse_NS(CALLERMETHOD_TRANSACTION_CONFIRM, msg)
                Else
                    err = ProcessTransactionResponse(CALLERMETHOD_TRANSACTION_CONFIRM, msg)
                End If
                BasketPayEntity.BASKET_PAYMENT_ID = VanguardAttributes.BasketPaymentID
                If err.HasError Then
                    _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                    err.ErrorNumber = "TACTV-ITC-002"
                    err.ErrorMessage = "Error while processing response for confirmation request"
                Else
                    If err.ErrorNumber.Trim("0").Length = 0 AndAlso BasketPayEntity IsNot Nothing Then
                        BasketPayEntity.STATUS = StatusID.TRANSACTION_CONFIRM
                        BasketPayEntity.TRANSACTION_ID = VanguardAttributes.TransactionID
                        BasketPayEntity.MERCHANT_REFERENCE = GetMerchantReference(BasketPayEntity.BASKET_PAYMENT_ID)
                        affectedRows = TDataObjects.BasketSettings.TblBasketPayment.UpdateStatus(BasketPayEntity.BASKET_PAYMENT_ID, StatusID.TRANSACTION_CONFIRM, BasketPayEntity.MERCHANT_REFERENCE, BasketPayEntity.TRANSACTION_ID)
                        If affectedRows <= 0 Then
                            err.HasError = True
                            err.ErrorNumber = "TACTV-ITC-003"
                            err.ErrorMessage = "Error while updating status to basket payment"
                            _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                        Else
                            err = InsertBasketStatus(StatusID.TRANSACTION_CONFIRM, StatusComment.TRANSACTION_CONFIRM_END)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-ITC-004"
            err.ErrorMessage = "Exception while transaction-confirm request:" & ex.Message
            _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
        End Try

        Return err
    End Function

    Private Function ConfirmationRequest() As Message
        Dim msg As New VanguardWebService.Message
        msg.MsgType = "VGCONFIRMATIONREQUEST"
        msg.ClientHeader = GetVanguardClientHeader()
        msg.MsgData = String.Format(VGRequestXML.VGCONFIRMATIONREQUEST, VanguardAttributes.SessionGUID, VanguardAttributes.TransactionID)
        msg = ProcessMsg(msg)
        Return msg
    End Function

    Private Function ConfirmationRequest_NS() As Message
        Dim msg As New VanguardWebService.Message
        msg.MsgType = "CNF"
        msg.ClientHeader = GetVanguardClientHeader()
        msg.MsgData = String.Format(VGRequestXML.VGCONFIRMATIONREQUEST_NS, VanguardAttributes.TransactionID)
        msg = ProcessMsg(msg)
        Return msg
    End Function

    Private Function IsTransactionRejected() As ErrorObj
        Dim err As New ErrorObj
        Dim msg As Message
        Dim affectedRows As Integer = 0
        Try
            If VanguardAttributes.PaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE Then
                msg = RejectionRequest_NS()
            Else
                msg = RejectionRequest()
            End If

            err = ProcessMessageResponseError(msg)
            If err.HasError Then
                _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage

                err.ErrorNumber = "TACTV-ITR-001"
                err.ErrorMessage = "Error while processing response for transaction send request"
            Else
                If VanguardAttributes.PaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE Then
                    err = ProcessTransactionResponse_NS(CALLERMETHOD_TRANSACTION_CONFIRM, msg)
                Else
                    err = ProcessTransactionResponse(CALLERMETHOD_TRANSACTION_CONFIRM, msg)
                End If
                If err.HasError Then
                    _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                    err.ErrorNumber = "TACTV-ITR-002"
                    err.ErrorMessage = "Error while processing response for rejection request"
                Else
                    BasketPayEntity.MERCHANT_REFERENCE = GetMerchantReference(BasketPayEntity.BASKET_PAYMENT_ID)
                    affectedRows = TDataObjects.BasketSettings.TblBasketPayment.UpdateStatus(VanguardAttributes.BasketPaymentID, StatusID.TRANSACTION_REJECT, BasketPayEntity.MERCHANT_REFERENCE)
                    If affectedRows <= 0 Then
                        err.HasError = True
                        err.ErrorNumber = "TACTV-ITR-003"
                        err.ErrorMessage = "Error while updating the basket payment status"
                        _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
                    End If
                End If
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTV-ITR-004"
            err.ErrorMessage = "Exception while transaction-reject request:" & ex.Message
            _logMessage = _logMessage & err.ErrorNumber & ";" & err.ErrorMessage
        End Try
        Return err
    End Function

    Private Function RejectionRequest() As Message
        Dim msg As New VanguardWebService.Message
        msg.MsgType = "VGREJECTIONREQUEST"
        msg.ClientHeader = GetVanguardClientHeader()

        msg.MsgData = String.Format(VGRequestXML.VGREJECTIONREQUEST,
                                     VanguardAttributes.SessionGUID,
                                     VanguardAttributes.TransactionID,
                                     VanguardAttributes.CaptureMethod)
        msg = ProcessMsg(msg)
        Return msg
    End Function

    Private Function RejectionRequest_NS() As Message
        Dim msg As New VanguardWebService.Message
        msg.MsgType = "RJT"
        msg.ClientHeader = GetVanguardClientHeader()

        msg.MsgData = String.Format(VGRequestXML.VGREJECTIONREQUEST_NS,
                                    VanguardAttributes.TransactionID,
                                    BasketPayEntity.TOKENID,
                                    VanguardAttributes.CaptureMethod)
        msg = ProcessMsg(msg)
        Return msg
    End Function

#End Region

#Region "Webservice Call and Process"

    Private Function ProcessMsg(ByVal msg As Message)
        'Log the Request
        LogVanguardProcess(msg.ClientHeader, msg.MsgType, msg.MsgData)

        Dim service As New VanguardWebService.CommideaGateway
        service.Url = VanguardAttributes.GatewayUrl
        Dim message As Message = service.ProcessMsg(msg)
        message.MsgData = message.MsgData.Replace("<![CDATA[", "").Replace("]]>", "")

        'Log the Responses
        LogVanguardProcess(message.ClientHeader, message.MsgType, message.MsgData)

        Return message
    End Function

    Private Function ProcessMessageResponseError(ByVal message As VanguardWebService.Message) As ErrorObj
        '<ERROR>
        '  <CODE>0012</CODE>
        '  <MSGTXT>XML missing or wrong format</MSGTXT>
        '</ERROR>

        Dim err As New ErrorObj
        If message.MsgType = "ERROR" Then
            Dim errormsg As XElement = XElement.Parse(message.MsgData)
            err.HasError = True
            err.ErrorNumber = errormsg.Element("CODE").Value
            err.ErrorMessage = errormsg.Element("MSGTXT").Value
        End If
        Return err
    End Function

#End Region

#End Region

#Region "Logging Methods"

    Private Sub LogVanguardProcessError(ByVal err As ErrorObj, ByVal logMessage As String, ByVal methodName As String)
        logMessage = VanguardAttributes.BasketHeaderID & ";" & logMessage
        Dim filter4 As String = VanguardAttributes.BasketPaymentID & ";" & VanguardAttributes.ProcessStep
        _talentLogging.Logging(CLASSNAME, methodName, logMessage, err, LogTypeConstants.TCBMVANGUARDERRLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId, filter4, VanguardAttributes.BasketHeaderID)
    End Sub

    Private Sub LogVanguardProcessError(ByVal err As ErrorObj, ByVal ex As Exception, ByVal methodName As String)
        Dim filter4 As String = VanguardAttributes.BasketPaymentID & ";" & VanguardAttributes.ProcessStep
        _talentLogging.Logging(CLASSNAME, methodName, err, ex, LogTypeConstants.TCBMVANGUARDERRLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId, filter4, VanguardAttributes.BasketHeaderID)
    End Sub

    Private Sub LogVanguardProcess(ByVal clientHeaderEntity As VanguardWebService.ClientHeader, ByVal messageType As String, ByVal XML As String)
        If VanguardAttributes.CanLogTheProcessXML Then
            Dim headerDetails As String = ""
            Try
                If clientHeaderEntity IsNot Nothing Then
                    headerDetails = clientHeaderEntity.CDATAWrapping
                    headerDetails = headerDetails & ";" & clientHeaderEntity.Passcode & ";" & clientHeaderEntity.ProcessingDB
                    headerDetails = headerDetails & ";" & clientHeaderEntity.SendAttempt
                    headerDetails = headerDetails & ";" & clientHeaderEntity.SystemGUID & ";" & clientHeaderEntity.SystemID
                    XML = headerDetails & ";" & XML
                End If
            Catch ex As Exception

            End Try

            Try
                _talentLogging.Logging(CLASSNAME, messageType, XML, VanguardAttributes.BasketPaymentID, LogTypeConstants.TCBMVANGUARDPROLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId, VanguardAttributes.ProcessStep, VanguardAttributes.BasketHeaderID)
            Catch ex As Exception

            End Try
        End If
    End Sub

#End Region

#Region "Helpers"

    Private Function InsertBasketStatus(ByVal flowStatus As Integer, ByVal statusMessage As String) As ErrorObj
        Dim err As New ErrorObj
        Dim basketStatusEntity As DEBasketStatus = GetBasketStatusEntity()
        basketStatusEntity.Comment = statusMessage
        basketStatusEntity.Status = flowStatus
        If Not String.IsNullOrWhiteSpace(VanguardAttributes.TransactionID) Then
            basketStatusEntity.ExternalOrderNumber = VanguardAttributes.TransactionID
        End If
        If basketStatusEntity IsNot Nothing Then
            TDataObjects.BasketSettings.TblBasketStatus.BasketStatusEntity = basketStatusEntity
            Dim affectedRows As Integer = TDataObjects.BasketSettings.TblBasketStatus.Insert()
            If affectedRows <= 0 Then
                'error log it
                err.HasError = True
                err.ErrorNumber = "TACTV-BS-002"
                err.ErrorMessage = "Failed basket status insert"
                Dim logMessage As String = VanguardAttributes.BasketHeaderID.ToString() & ";" & Settings.BusinessUnit & ";" & Settings.LoginId & ";" & flowStatus.ToString & ";" & statusMessage
                LogVanguardProcessError(err, logMessage, "InsertBasketStatus")
            End If
        Else
            err.HasError = True
            err.ErrorNumber = "TACTV-BS-001"
            err.ErrorMessage = "Failed basket status insert; basket status entity is empty"
            Dim logMessage As String = VanguardAttributes.BasketHeaderID.ToString() & ";" & Settings.BusinessUnit & ";" & Settings.LoginId & ";" & flowStatus.ToString & ";" & statusMessage
            LogVanguardProcessError(err, logMessage, "InsertBasketStatus")
        End If

        Return err
    End Function

    Private Sub SetTxtnAmtAndDisplay()
        VanguardAttributes.TransactionAmount = VanguardAttributes.PaymentAmount * 100
        VanguardAttributes.TransactionDisplayAmount = VanguardAttributes.PaymentAmount
    End Sub

    Private Function GetCardTypeCode(ByVal cardSchemeID As String, ByVal cardSchemeName As String) As String
        'use tbl_creditcard and get the card type code 
        Dim cardTypeCode As String = cardSchemeName
        cardTypeCode = TDataObjects.PaymentSettings.TblCreditCard.GetCardTypeCodeForVanguard(cardSchemeID, cardSchemeName)
        Return cardTypeCode
    End Function

    Private Function GetHouseNumber(ByVal addressLine As String) As String
        '------------------------------------------------------------------
        ' GetHouseNumber - This retrieves the first portion of the address
        ' required for AVS, which can either be the house number or name
        '------------------------------------------------------------------
        Dim houseNumber As String = String.Empty
        Dim firstSpace As Integer = addressLine.IndexOf(" ")
        '-------------------------------------
        ' To long - just return first 30 chars
        '-------------------------------------
        If firstSpace > 30 Or firstSpace < 0 Then
            '-------------------------------------
            ' To long - just return first 30 chars
            '-------------------------------------
            houseNumber = addressLine.Substring(0, addressLine.Length)
        Else
            '--------------------------------
            ' Just return first section
            '--------------------------------
            houseNumber = addressLine.Substring(0, firstSpace)
        End If

        Return houseNumber
    End Function

    Private Function GetNumeric(ByVal value As String) As String
        Dim output As StringBuilder = New StringBuilder
        For i As Integer = 0 To value.Length - 1
            Dim num As Integer
            If Integer.TryParse(value(i), num) Then
                output.Append(value(i))
            End If
        Next
        Return output.ToString()
    End Function

    Private Function canProcess3DSecure(ByVal cardType As String) As Boolean
        Dim process3DSecure As Boolean = True
        If Not String.IsNullOrWhiteSpace(cardType) Then
            process3DSecure = TDataObjects.PaymentSettings.TblCreditCard.CanCardTypeUse3DSecure(cardType)
        End If
        Return process3DSecure
    End Function


    Private Function GetMerchantReference(ByVal BasketPaymentID As Long) As String
        'Merchant Reference in BUI = Basket Pay ID / Customer Number / Agent Name 
        'Merchant Reference in PWS = Basket Pay ID / Customer Number
        Dim merchReference As String = BasketPaymentID.ToString
        If Not String.IsNullOrWhiteSpace(Settings.LoginId) Then
            merchReference = merchReference & "/" & Settings.LoginId.Trim
        End If
        If Not String.IsNullOrWhiteSpace(Settings.AgentEntity.AgentUsername) Then
            merchReference = merchReference & "/" & Settings.AgentEntity.AgentUsername.Trim
        End If
        Return merchReference
    End Function

    Private Function IsValidMerchantReference(ByVal merchantReference As String, ByVal BasketPaymetnID As Long) As Boolean
        'Merchant Reference in BUI = Basket Pay ID / Customer Number / Agent Name 
        'Merchant Reference in PWS = Basket Pay ID / Customer Number
        Dim isValid As Boolean = False
        'split based on /
        If Not String.IsNullOrWhiteSpace(merchantReference) Then
            Dim merRef As String() = merchantReference.Split("/")
            If merRef.Length > 0 Then
                If IsNumeric(merRef(0)) Then
                    If CLng(merRef(0)) = BasketPaymetnID Then
                        isValid = True
                    End If
                End If
            End If
        End If
        Return isValid
    End Function

#End Region

#End Region

#Region "Time Being Unused Methods"

    Private Function CVCAndAVSGetSettingsType() As String
        Dim CVCAVSSettingsType As String = "TKT"
        ' "COR"
        Return CVCAVSSettingsType
    End Function

    Private Sub CVCAndAVSGetCardTypeSetting_Backend(ByVal cardType As String)
        Dim CVCAVSSettingsType As String = CVCAndAVSGetSettingsType()
        _cardTypeCvcAndAvs = New DECVCAndAVSAuthorization
        If VanguardAttributes.CacheConfigSeconds > 0 Then
            Settings.Cacheing = True
            Settings.CacheTimeSeconds = VanguardAttributes.CacheConfigSeconds
        End If
        Settings.ModuleName = "GetCardTypeCVCAndAVS"
        Dim cacheKey As String = Settings.ModuleName & Settings.Company & cardType.ToUpper & CVCAVSSettingsType
        If Settings.Cacheing And Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            _cardTypeCvcAndAvs = CType(HttpContext.Current.Cache.Item(cacheKey), DECVCAndAVSAuthorization)
        Else
            Dim err As ErrorObj = GetVanguardConfigurations()

            If Not err.HasError AndAlso ResultDataSet IsNot Nothing Then
                If ResultDataSet.Tables("CVCAVSSettings") IsNot Nothing AndAlso ResultDataSet.Tables("CVCAVSSettings").Rows.Count > 0 Then
                    'populate the cardtypeCVCandavs
                    _cardTypeCvcAndAvs = CVCAndAVSPopulateByCardType(cardType, CVCAVSSettingsType, _cardTypeCvcAndAvs, ResultDataSet.Tables("CVCAVSSettings"))
                    If Not err.HasError Then
                        AddItemToCache(cacheKey, _cardTypeCvcAndAvs, Settings)
                    Else
                        Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                    End If
                End If
            End If
        End If
    End Sub

    Private Function CVCAndAVSPopulateByCardType(ByVal cardType As String, ByVal CVCAVSSettingsType As String, ByVal cardTypeCVCAndAVS As DECVCAndAVSAuthorization, ByVal dtCVCAVSSettings As DataTable) As DECVCAndAVSAuthorization
        If dtCVCAVSSettings.Rows.Count > 0 Then
            If CheckForDBNull_Boolean_DefaultFalse(dtCVCAVSSettings.Rows(0)("CSC_AVS_MODULE_ON")) Then

                If cardType.ToUpper = "AMEX" OrElse cardType.ToUpper = "AMERICAN EXPRESS" Then
                    If CVCAVSSettingsType = "TKT" Then
                        cardTypeCVCAndAVS = CVCAndAVSPopulateCVC(cardTypeCVCAndAVS, _
                                                                  CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_CSC_ON")).ConvertFromISeriesYesNoToBoolean, _
                                                                  CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_CSC_MANDATORY")).ConvertFromISeriesYesNoToBoolean, _
                                                                  CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_CSC_FORCE_REJECT")).ConvertFromISeriesYesNoToBoolean)
                        'avs addr and pc settings
                        cardTypeCVCAndAVS = CVCAndAVSPopulateAVSAddr(cardTypeCVCAndAVS, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_AVS_AMEX_ON")).ConvertFromISeriesYesNoToBoolean, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_AVS_FORCE_REJECT_ADDR")).ConvertFromISeriesYesNoToBoolean)

                        cardTypeCVCAndAVS = CVCAndAVSPopulateAVSPost(cardTypeCVCAndAVS, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_AVS_AMEX_ON")).ConvertFromISeriesYesNoToBoolean, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_AVS_FORCE_REJECT_PC")).ConvertFromISeriesYesNoToBoolean)

                    Else
                        cardTypeCVCAndAVS = CVCAndAVSPopulateCVC(cardTypeCVCAndAVS, _
                                                            CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_CSC_ON")).ConvertFromISeriesYesNoToBoolean, _
                                                            CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_CSC_MANDATORY")).ConvertFromISeriesYesNoToBoolean, _
                                                            CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_CSC_FORCE_REJECT")).ConvertFromISeriesYesNoToBoolean)
                        'avs addr and pc settings
                        cardTypeCVCAndAVS = CVCAndAVSPopulateAVSAddr(cardTypeCVCAndAVS, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_AVS_ON")).ConvertFromISeriesYesNoToBoolean, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_AVS_FORCE_REJECT_ADDR")).ConvertFromISeriesYesNoToBoolean)

                        cardTypeCVCAndAVS = CVCAndAVSPopulateAVSPost(cardTypeCVCAndAVS, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_AVS_ON")).ConvertFromISeriesYesNoToBoolean, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_AVS_FORCE_REJECT_PC")).ConvertFromISeriesYesNoToBoolean)


                    End If
                Else
                    If CVCAVSSettingsType = "TKT" Then
                        cardTypeCVCAndAVS = CVCAndAVSPopulateCVC(cardTypeCVCAndAVS, _
                                                                  CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_CSC_ON")).ConvertFromISeriesYesNoToBoolean, _
                                                                  CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_CSC_MANDATORY")).ConvertFromISeriesYesNoToBoolean, _
                                                                  CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_CSC_FORCE_REJECT")).ConvertFromISeriesYesNoToBoolean)
                        'avs addr and pc settings
                        cardTypeCVCAndAVS = CVCAndAVSPopulateAVSAddr(cardTypeCVCAndAVS, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_AVS_ALL_CARDS")).ConvertFromISeriesYesNoToBoolean, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_AVS_FORCE_REJECT_ADDR")).ConvertFromISeriesYesNoToBoolean)

                        cardTypeCVCAndAVS = CVCAndAVSPopulateAVSPost(cardTypeCVCAndAVS, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_AVS_ALL_CARDS")).ConvertFromISeriesYesNoToBoolean, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("TKT_AVS_FORCE_REJECT_PC")).ConvertFromISeriesYesNoToBoolean)


                    Else
                        cardTypeCVCAndAVS = CVCAndAVSPopulateCVC(cardTypeCVCAndAVS, _
                                                            CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_CSC_ON")).ConvertFromISeriesYesNoToBoolean, _
                                                            CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_CSC_MANDATORY")).ConvertFromISeriesYesNoToBoolean, _
                                                            CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_CSC_FORCE_REJECT")).ConvertFromISeriesYesNoToBoolean)
                        'avs addr and pc settings
                        cardTypeCVCAndAVS = CVCAndAVSPopulateAVSAddr(cardTypeCVCAndAVS, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_AVS_ON")).ConvertFromISeriesYesNoToBoolean, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_AVS_FORCE_REJECT_ADDR")).ConvertFromISeriesYesNoToBoolean)

                        cardTypeCVCAndAVS = CVCAndAVSPopulateAVSPost(cardTypeCVCAndAVS, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_AVS_ON")).ConvertFromISeriesYesNoToBoolean, _
                                                                              CheckForDBNull_String(dtCVCAVSSettings.Rows(0)("COR_AVS_FORCE_REJECT_PC")).ConvertFromISeriesYesNoToBoolean)

                    End If

                End If


            End If
        End If
        Return cardTypeCVCAndAVS
    End Function

    Private Function CVCAndAVSPopulateCVC(ByVal cardTypeCVCAndAVS As DECVCAndAVSAuthorization, ByVal cvcModuleOn As Boolean, ByVal cvcMandatory As Boolean, ByVal cvcForceReject As Boolean) As DECVCAndAVSAuthorization
        With cardTypeCVCAndAVS
            If cvcModuleOn Then
                .CVCEnabled = True
                If Not cvcMandatory Then
                    .CVCAccept_NotProvided = True
                    .CVCAccept_NotChecked = True
                    .CVCAccept_PartialMatch = True
                    .CVCAccept_Matched = True
                    .CVCAccept_NotMatched = False
                End If
                'override the above based on force reject
                If cvcForceReject Then
                    .CVCAccept_NotProvided = False
                    .CVCAccept_NotChecked = False
                    .CVCAccept_PartialMatch = False
                    .CVCAccept_NotMatched = False
                    .CVCAccept_Matched = True
                End If
            End If
        End With
        Return cardTypeCVCAndAVS
    End Function

    Private Function CVCAndAVSPopulateAVSAddr(ByVal cardTypeCVCAndAVS As DECVCAndAVSAuthorization, ByVal avsModuleOn As Boolean, ByVal addrForceReject As Boolean) As DECVCAndAVSAuthorization
        With cardTypeCVCAndAVS
            If avsModuleOn Then
                .AVS_Addr_Enabled = True
                If addrForceReject Then
                    .AVSAccept_Addr_Matched = True
                    .AVSAccept_Addr_NotChecked = False
                    .AVSAccept_Addr_NotMatched = False
                    .AVSAccept_Addr_NotProvided = False
                    .AVSAccept_Addr_PartialMatch = False
                End If
            End If
        End With
        Return cardTypeCVCAndAVS
    End Function

    Private Function CVCAndAVSPopulateAVSPost(ByVal cardTypeCVCAndAVS As DECVCAndAVSAuthorization, ByVal avsModuleOn As Boolean, ByVal postForceReject As Boolean) As DECVCAndAVSAuthorization
        With cardTypeCVCAndAVS
            If avsModuleOn Then
                .AVS_PC_Enabled = True
                If postForceReject Then
                    .AVSAccept_PC_Matched = True
                    .AVSAccept_PC_NotChecked = False
                    .AVSAccept_PC_NotMatched = False
                    .AVSAccept_PC_NotProvided = False
                    .AVSAccept_PC_PartialMatch = False
                End If
            End If
        End With
        Return cardTypeCVCAndAVS
    End Function
    Private Function IsValidStepRequestForStage() As Boolean
        Dim isValid As Boolean = True
        If VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.CHECKOUT Then
            If VanguardAttributes.ProcessStep < VanguardAttributes.PreviousProcessingStep Then
                isValid = False
            End If
        End If
        Return isValid
    End Function

    Private Function GetProcessingDB(basketPaymentID As Integer) As Integer
        'TDataObjects.AppVariableSettings.tbl()
    End Function

    Private Function IsValidHandshakeID(ByVal handshakeid As String) As Boolean
        Return True
    End Function

#End Region

End Class
