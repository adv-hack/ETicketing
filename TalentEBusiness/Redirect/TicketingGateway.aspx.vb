Imports System.Data
Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports Talent.Common
Imports Talent.eCommerce.Utilities
Imports Talent.eCommerce.CATHelper
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Collections.Generic
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class Redirect_TicketingGateway
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _missingParamErrorCode As String = String.Empty
    Private _productHasRelatedProducts As Boolean = False
    Private _productHasMandatoryRelatedProducts As Boolean = False
    Private _redirectToProductLinkingPage As Boolean = False
    Private _redirectToComponentOrBookingPage As String = String.Empty
    Private _redirectToSeasonTicketExceptionsPage As Boolean = False
    Private _redirectUrl As String = String.Empty
    Private _autoAddProductCode As String = String.Empty
    Private _packageId As Long = 0
    Private _TicketingGatewayFunctions As TicketingGatewayFunctions = Nothing
    Private _linkedMasterProduct As String = String.Empty

#End Region

#Region "Protected Methods"

    Protected Sub Page_PreInit1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        _TicketingGatewayFunctions = New TicketingGatewayFunctions
        Session("TicketingGatewayError") = Nothing
        If Not String.IsNullOrEmpty(Request("page")) AndAlso Not String.IsNullOrEmpty(Request("function")) Then
            Dim page As String = Request("page"), _
                func As String = Request("function")

            Select Case LCase(Request("page"))

                'The validate noise session function needs to be executed here as we are experiencing
                'problems with the profile when used by agents.
                Case Is = "validatesession.aspx"
                    Select Case LCase(func)
                        Case Is = "validatesession"
                            Noise_ValidateSession()
                        Case Is = "validatesessionpos"
                            ' This function is used only when invoking app via agent portal from a PoS device
                            ' In this case POS ip and port are passed on QS and need to go into session
                            ' reday for use by Checkout.aspx
                            If Not Request.QueryString("posip") Is Nothing Then Session("posip") = Request.QueryString("posip")
                            If Not Request.QueryString("posport") Is Nothing Then Session("posport") = Request.QueryString("posport")
                            ' ....then, business as usual.
                            Noise_ValidateSession()
                    End Select
            End Select
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not String.IsNullOrEmpty(Request("page")) AndAlso Not String.IsNullOrEmpty(Request("function")) Then
            Dim page As String = Request("page"), _
                func As String = Request("function")
            If String.IsNullOrWhiteSpace(page) OrElse String.IsNullOrWhiteSpace(func) Then
                page = Session("page").ToString()
                func = Session("func").ToString()
                Session.Remove("page")
                Session.Remove("func")
            End If

            If IsValidAddToBasketRequest(page, func, _productHasRelatedProducts, _productHasMandatoryRelatedProducts) Then
                _linkedMasterProduct = TDataObjects.BasketSettings.TblBasketHeader.GetLinkedMasterProduct(Profile.Basket.Basket_Header_ID)
                Select Case LCase(page)
                    Case Is = "producthome.aspx"
                        Select Case LCase(func)
                            Case Is = "addtobasket" : ProductHome_AddToBasket()
                            Case Is = "kioskfinish" : Home_KioskFinish()
                        End Select

                    Case Is = "productseason.aspx"
                        If LCase(func) = "addtobasket" Then ProductSeason_AddToBasket()

                    Case Is = "visualseatselection.aspx"
                        If LCase(func) = "addtobasket" Then VisualSeatSelection_AddToBasket()

                    Case Is = "seatselection.aspx"
                        If LCase(func) = "addtobasket" Then SeatSelection_AddToBasket()

                    Case Is = "productaway.aspx"
                        If LCase(func) = "addtobasket" Then
                            If String.IsNullOrEmpty(Request("productIsHomeAsAway")) Then
                                ProductAway_AddToBasket()
                            Else
                                If (Request("productIsHomeAsAway").Equals("Y")) Then
                                    ProductHome_AddToBasket()
                                End If
                            End If
                        End If

                    Case Is = "producttravel.aspx", "producttour.aspx"
                        If LCase(func) = "addtobasket" Then TravelProduct_AddToBasket()

                    Case Is = "productevent.aspx", "productfitc.aspx"
                        If LCase(func) = "addtobasket" Then EventProduct_AddToBasket()

                    Case Is = "productmembership.aspx"
                        If LCase(func) = "addtobasket" Then MembershipProduct_AddToBasket()
                        If LCase(func) = "addvariablepricedtobasket" Then MembershipProduct_AddVariablePricedToBasket()

                    Case Is = "checkoutpaymentdetails.aspx"
                        Select Case LCase(func)
                            Case Is = "payment" : _TicketingGatewayFunctions.CheckoutPayment()
                            Case Is = "finalise" : CheckoutPaymentDetails_Finalise()
                            Case Is = "promo" : CheckoutPaymentDetails_Promo()
                            Case Is = "paymentexternalcheckout" : _TicketingGatewayFunctions.CheckoutExternal(True)
                            Case Is = "paymentexternalsuccess" : _TicketingGatewayFunctions.CheckoutExternalSuccess(True, "", True, "")
                            Case Is = "paymentexternalfailure" : _TicketingGatewayFunctions.CheckoutExternalFailure(True, "", False)
                        End Select

                    Case Is = "basket.aspx"
                        Select Case LCase(func)
                            Case Is = "updateandremovefrombasket" : _TicketingGatewayFunctions.Basket_UpdateAndRemoveFromBasket()
                            Case Is = "removefrombasket" : Basket_RemoveFromBasket()
                            Case Is = "updatebasket" : _TicketingGatewayFunctions.Basket_UpdateBasket()
                            Case Is = "clearbasket" : _TicketingGatewayFunctions.Basket_ClearBasket(True)
                            Case Is = "checkout" : _TicketingGatewayFunctions.Basket_Checkout()
                            Case Is = "kioskrestart" : Basket_KioskRestart()
                            Case Is = "kioskfinish" : Home_KioskFinish()
                            Case Is = "kioskcheckout" : Basket_KioskCheckout()
                            Case Is = "retrievebasket" : RetrieveBasket()
                            Case Is = "retailbasketcheckout" : RetailBasketCheckout()
                            Case Is = "overriderestriction" : _TicketingGatewayFunctions.Basket_OverrideRestriction()
                        End Select

                    Case Is = "login.aspx"
                        Select Case LCase(func)
                            Case Is = "clearandadd" : _TicketingGatewayFunctions.Login_ClearAndAdd(_redirectUrl, _redirectToProductLinkingPage)
                            Case Is = "clearbasket" : Login_ClearBasket()
                        End Select

                    Case Is = "home.aspx"
                        Select Case LCase(func)
                            Case Is = "addreserveditemstobasket" : ProductHome_AddReservedItemsToBasket()
                            Case Is = "addseasonticketrenewalstobasket" : ProductSeason_AddSeasonTicketRenewalsToBasket()
                            Case Is = "kioskfinish" : Home_KioskFinish()
                        End Select

                    Case Is = "ticketingprepayments.aspx"
                        Select Case LCase(func)
                            Case Is = "addtobasket" : PPS_AddToBasket()
                        End Select

                    Case Is = "standandareaselection.aspx"
                        Select Case LCase(func)
                            Case Is = "kioskfinish" : Home_KioskFinish()
                            Case Is = "kioskback" : Home_KioskBack()
                            Case Is = "addtobasket" : ProductHome_AddToBasket()
                        End Select

                    Case Is = "ticketinggateway.aspx"
                        Select Case LCase(func)
                            Case Is = "kioskpaymentsuccess" : TicketingGateway_KioskPaymentSuccess()
                            Case Is = "kioskpaymentfailure" : TicketingGateway_KioskPaymentFailure()
                        End Select

                    Case Is = "orderreturn.aspx"
                        Select Case LCase(func)
                            Case Is = "orderreturn" : OrderReturn_OrderReturn()
                        End Select

                    Case "registration.aspx"
                        Select Case LCase(func)
                            Case Is = "addfreemembership" : FreeMembershipProduct_AddToBasket()
                        End Select

                    Case "waitlist.aspx"
                        Select Case func.ToLower
                            Case "withdrawseasonticketwaitlist" : WithdrawSeasonTicketWaitList()
                            Case "addseasonticketwaitlist" : AddSeasonTicketWaitList()
                        End Select

                    Case "matchdayhospitality.aspx"
                        Select Case func.ToLower
                            Case Is = "addtobasket" : ProductHospitality_AddToBasket()
                        End Select

                    Case "registrationparticipants.aspx"
                        Select Case func.ToLower
                            Case Is = "retrievebasket" : RetrieveBasket()
                        End Select
                        _redirectToProductLinkingPage = True

                    Case "catconfirm.aspx"
                        Select Case func.ToLower
                            Case Is = "addtobasket" : CATConfirm_AddToBasket()
                            Case Is = "packageaddtobasket" : CATPackage_AddToBasket()
                        End Select

                End Select

                Dim redirectProductCode As String = _autoAddProductCode
                Dim priceCode As String = String.Empty
                Dim productSubType As String = String.Empty
                Dim callId As Long = 0
                If Request.QueryString("product") IsNot Nothing AndAlso redirectProductCode.Length = 0 Then redirectProductCode = Request.QueryString("product")
                If (_redirectToComponentOrBookingPage <> String.Empty AndAlso _redirectToComponentOrBookingPage <> "N") AndAlso Request.QueryString("product") IsNot Nothing Then redirectProductCode = Request.QueryString("product")
                If Request.QueryString("pricecode") IsNot Nothing Then priceCode = Request.QueryString("pricecode")
                If Request.QueryString("campaign") IsNot Nothing Then priceCode = Request.QueryString("campaign")
                If Request.QueryString("productsubtype") IsNot Nothing Then productSubType = Request.QueryString("productsubtype")
                If Not String.IsNullOrEmpty(Request.QueryString("callid")) Then callId = Request.QueryString("callId")
                _TicketingGatewayFunctions.ProductHasRelatedProducts = _productHasRelatedProducts
                Dim redirectString As String = _TicketingGatewayFunctions.HandleRedirect(_redirectUrl, _redirectToProductLinkingPage, redirectProductCode, priceCode, productSubType, _redirectToComponentOrBookingPage, _redirectToSeasonTicketExceptionsPage, _packageId, _linkedMasterProduct, callId)
                Response.Redirect(redirectString)
            End If
        End If
    End Sub

#End Region

#Region "Noise"

    Private Sub Noise_ValidateSession()
        If ModuleDefaults.NOISE_IN_USE Then
            Dim invalidSessionPageName As String = "SessionError.aspx"
            Dim invalidSessionURL As String = "~/PagesPublic/Error/" & invalidSessionPageName
            Dim sessionVar As String = Request.Form("session_id")
            If String.IsNullOrEmpty(sessionVar) Then
                sessionVar = Request.QueryString("session_id")
            End If

            If Not String.IsNullOrEmpty(sessionVar) Then
                'decrypt session id, and validate
                sessionVar = sessionVar.Replace(" ", "+")
                Dim decryptedStr As String = Talent.Common.Utilities.TripleDESDecode(sessionVar, ModuleDefaults.NOISE_ENCRYPTION_KEY)
                If Not String.IsNullOrEmpty(decryptedStr) Then
                    Dim agentName As String = ""
                    Dim passport As String = ""
                    Dim greenCard As String = ""
                    Dim pin As String = ""
                    Dim userID4 As String = ""
                    Dim userID5 As String = ""
                    Dim userID6 As String = ""
                    Dim userID7 As String = ""
                    Dim userID8 As String = ""
                    Dim userID9 As String = ""
                    Dim agentType As String = String.Empty
                    Dim agentForename As String = String.Empty
                    Dim agentSurname As String = String.Empty
                    Dim agentAddressLine1 As String = String.Empty
                    Dim agentAddressLine2 As String = String.Empty
                    Dim agentAddressLine3 As String = String.Empty
                    Dim agentAddressLine4 As String = String.Empty
                    Dim agentPostCode As String = String.Empty
                    Dim agentEmail As String = String.Empty
                    Try
                        If decryptedStr.Length > 19 Then
                            agentName = decryptedStr.Substring(19, 10).Trim
                            passport = decryptedStr.Substring(29, 20).Trim
                            greenCard = decryptedStr.Substring(49, 20).Trim
                            pin = decryptedStr.Substring(69, 20).Trim
                            userID4 = decryptedStr.Substring(89, 20).Trim
                            userID5 = decryptedStr.Substring(109, 20).Trim
                            userID6 = decryptedStr.Substring(129, 20).Trim
                            userID7 = decryptedStr.Substring(149, 20).Trim
                            userID8 = decryptedStr.Substring(169, 20).Trim
                            userID9 = decryptedStr.Substring(189, 20).Trim
                            agentType = decryptedStr.Substring(209, 10).Trim
                            If (decryptedStr.Length > 219) Then
                                agentForename = decryptedStr.Substring(219, 20).Trim
                                agentSurname = decryptedStr.Substring(239, 30).Trim
                                agentAddressLine1 = decryptedStr.Substring(269, 30).Trim
                                agentAddressLine2 = decryptedStr.Substring(299, 30).Trim
                                agentAddressLine3 = decryptedStr.Substring(329, 25).Trim
                                agentAddressLine4 = decryptedStr.Substring(354, 25).Trim
                                agentPostCode = decryptedStr.Substring(379, 8).Trim
                                agentEmail = decryptedStr.Substring(387, 60).Trim
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    'if valid, test the date for session threshold
                    Dim day, month, year, hours, minutes, seconds As Integer

                    day = CInt(decryptedStr.Substring(0, 2))
                    month = CInt(decryptedStr.Substring(3, 2))
                    year = CInt(decryptedStr.Substring(6, 4))
                    hours = CInt(decryptedStr.Substring(11, 2))
                    minutes = CInt(decryptedStr.Substring(14, 2))
                    seconds = CInt(decryptedStr.Substring(17, 2))
                    Dim testDate As New Date(year, month, day, hours, minutes, seconds)
                    'Talent.eCommerce.Logging.WriteLog("", "", testDate.ToString("dd/MM/yyyy HH:mm:ss"), "")
                    If testDate >= Now.AddMinutes(-ModuleDefaults.NOISE_THRESHOLD_MINUTES) _
                        AndAlso testDate <= Now.AddMinutes(ModuleDefaults.NOISE_THRESHOLD_MINUTES) Then

                        ' if the session is valid, add it to the DB and forward on
                        Dim sessionStartTime As DateTime = Now
                        Dim noiseSettings As DESettings = Talent.eCommerce.Utilities.GetSettingsObject
                        Dim myNoise As New TalentNoise(noiseSettings, _
                                                        Session.SessionID, _
                                                        sessionStartTime, _
                                                        sessionStartTime.AddMinutes(-ModuleDefaults.NOISE_THRESHOLD_MINUTES), _
                                                        ModuleDefaults.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES, _
                                                        Not String.IsNullOrEmpty(agentName), TblActiveNoiseSessions_Usage.TICKETING, Utilities.CheckForDBNull_Int(agentType), agentName)

                        myNoise.De_Noise.ServerIP = HttpContext.Current.Request.ServerVariables("LOCAL_ADDR").ToString
                        myNoise.De_Noise.ClientIP = HttpContext.Current.Request.UserHostAddress()
                        myNoise.AddOrUpdateNoiseSession(ModuleDefaults.NOISE_MAX_CONCURRENT_USERS)
                        If myNoise.SuccessfullCall AndAlso myNoise.RowsAffected > 0 Then

                            'If successful, add agent flag if required
                            If Not String.IsNullOrEmpty(agentName) Then
                                Session.Item("Agent") = agentName
                                'Set some more session variable if it is agent
                                Session.Remove("AgentForename")
                                Session.Remove("AgentSurname")
                                Session.Remove("AgentAddressLine1")
                                Session.Remove("AgentAddressLine2")
                                Session.Remove("AgentAddressLine3")
                                Session.Remove("AgentAddressLine4")
                                Session.Remove("AgentPostCode")
                                Session.Remove("AgentEmail")
                                If Not String.IsNullOrEmpty(agentForename) Then Session.Item("AgentForename") = agentForename
                                If Not String.IsNullOrEmpty(agentSurname) Then Session.Item("AgentSurname") = agentSurname
                                If Not String.IsNullOrEmpty(agentAddressLine1) Then Session.Item("AgentAddressLine1") = agentAddressLine1
                                If Not String.IsNullOrEmpty(agentAddressLine2) Then Session.Item("AgentAddressLine2") = agentAddressLine2
                                If Not String.IsNullOrEmpty(agentAddressLine3) Then Session.Item("AgentAddressLine3") = agentAddressLine3
                                If Not String.IsNullOrEmpty(agentAddressLine4) Then Session.Item("AgentAddressLine4") = agentAddressLine4
                                If Not String.IsNullOrEmpty(agentPostCode) Then Session.Item("AgentPostCode") = agentPostCode
                                If Not String.IsNullOrEmpty(agentEmail) Then Session.Item("AgentEmail") = agentEmail
                            Else
                                Session.Remove("Agent")
                                Session.Remove("AgentForename")
                                Session.Remove("AgentSurname")
                                Session.Remove("AgentAddressLine1")
                                Session.Remove("AgentAddressLine2")
                                Session.Remove("AgentAddressLine3")
                                Session.Remove("AgentAddressLine4")
                                Session.Remove("AgentPostCode")
                                Session.Remove("AgentEmail")
                            End If

                            'If successful, add ID's if required    
                            Session.Remove("PIN")
                            Session.Remove("Greencard")
                            Session.Remove("Passport")
                            Session.Remove("UserID4")
                            Session.Remove("UserID5")
                            Session.Remove("UserID6")
                            Session.Remove("UserID7")
                            Session.Remove("UserID8")
                            Session.Remove("UserID9")

                            If Not String.IsNullOrEmpty(greenCard) Then Session.Item("Greencard") = greenCard
                            If Not String.IsNullOrEmpty(passport) Then Session.Item("Passport") = passport
                            If Not String.IsNullOrEmpty(pin) Then Session.Item("PIN") = pin
                            If Not String.IsNullOrEmpty(userID4) Then Session.Item("UserID4") = userID4
                            If Not String.IsNullOrEmpty(userID5) Then Session.Item("UserID5") = userID5
                            If Not String.IsNullOrEmpty(userID6) Then Session.Item("UserID6") = userID6
                            If Not String.IsNullOrEmpty(userID7) Then Session.Item("UserID7") = userID7
                            If Not String.IsNullOrEmpty(userID8) Then Session.Item("UserID8") = userID8
                            If Not String.IsNullOrEmpty(userID9) Then Session.Item("UserID9") = userID9
                            If Not String.IsNullOrEmpty(agentType) Then Session.Item("AgentType") = agentType

                            'Add a session var so we can tell the 
                            'user how long their session has left
                            If Session.Item("NoiseSessionStartTime") Is Nothing Then
                                Session.Add("NoiseSessionStartTime", sessionStartTime)
                            Else
                                Session.Item("NoiseSessionStartTime") = sessionStartTime
                            End If

                            RedirectFromNoise()
                        Else
                            'if the session was not created correctly, forward to the Invalid session page
                            Response.Redirect(invalidSessionURL)
                        End If

                    Else
                        'if the session is not valid, forward to the Invalid session page
                        Response.Redirect(invalidSessionURL)
                    End If
                Else
                    'if there is no decrypted value, forward to the Invalid session page
                    Response.Redirect(invalidSessionURL)
                End If

            Else
                'if there is no decrypted value, forward to the Invalid session page
                Response.Redirect(invalidSessionURL)
            End If
        Else
            'if noise is not being used we should not ever hit this function but redirect to the homepage in case
            RedirectFromNoise()
        End If
    End Sub

    Private Sub RedirectFromNoise()

        If AgentProfile.IsAgent Then
            CType(Me.Page, TalentBase01).doAutoLogin(True)
            If Not Profile.IsAnonymous AndAlso Not IsNothing(Profile.User.Details.Forename) AndAlso Not IsNothing(Profile.User.Details.Surname) Then
                TEBUtilities.addCustomerLoggedInToSession(ModuleDefaults.NoOfRecentlyUsedCustomers, Profile.User.Details.LoginID, Profile.User.Details.Forename, Profile.User.Details.Surname)
            End If
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnUrl")) Then
            Dim returnUrl As String = Request.QueryString("returnUrl")
            Dim queries As String = ""
            If Not returnUrl.Contains("?") Then
                queries += "?"
            End If
            For Each queryKey As String In Request.QueryString.AllKeys
                If Not LCase(queryKey) = "returnurl" _
                    AndAlso Not LCase(queryKey) = "page" _
                        AndAlso Not LCase(queryKey) = "session_id" _
                            AndAlso Not LCase(queryKey) = "function" Then
                    If Not queries.EndsWith("?") Then queries += "&"

                    queries += queryKey & "=" & HttpContext.Current.Server.UrlEncode(Request.QueryString(queryKey))
                End If
            Next
            returnUrl += queries
            If returnUrl.EndsWith("?") Then returnUrl = returnUrl.TrimEnd("?")

            ' Check if the URL contains a queryString.
            Dim queryStr As String = String.Empty
            Dim pos1 As Integer = returnUrl.IndexOf("?")
            If pos1 >= 0 Then

                ' If a querystring exists, split the url into 2 distinct partsso we can parse the querystring parameters.
                queryStr = returnUrl.Substring(pos1 + 1, returnUrl.Length - pos1 - 1)
                returnUrl = returnUrl.Substring(0, pos1)

                ' Parse the querystring and append it back to the URL after encoding each parameter value individually.
                Dim qscoll As NameValueCollection = HttpUtility.ParseQueryString(queryStr)
                queries = String.Empty
                For Each queryKey As String In qscoll
                    If Not queries = String.Empty Then queries += "&"
                    queries += queryKey & "=" & HttpContext.Current.Server.UrlEncode(qscoll.Get(queryKey))
                Next
                returnUrl += "?" + queries
            End If

            Response.Redirect(returnUrl)
        Else
            Response.Redirect(Talent.eCommerce.Utilities.GetSiteHomePage())
        End If
    End Sub

#End Region

#Region "Product Add To Basket"

    Private Sub ProductHome_AddToBasket()
        Dim product As String = Request.QueryString("product")
        Dim stand As String = Request.QueryString("stand")
        Dim area As String = Request.QueryString("area")
        Dim campaignCode As String = Request.QueryString("campaign")
        Dim quantity As String = Request.QueryString("quantity")
        Dim priceBand As String = Request.QueryString("priceBand")
        Dim productType As String = Request.QueryString("type")
        Dim productSubType = Request.QueryString("productsubtype")
        Dim productStadium = Request.QueryString("productstadium")
        Dim favouriteSeat As String = Request.QueryString("favouriteSeat")
        Dim defaultPrice As String = Request.QueryString("defaultPrice")
        Dim selectedMinPrice As String = Request.QueryString("minPrice")
        Dim selectedMaxPrice As String = Request.QueryString("maxPrice")
        Dim priceBreakId As String = Request.QueryString("priceBreakId")
        Dim favouriteSeatSelected As Boolean = False
        Dim missingParam As Boolean = False
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim customerNumber As String = String.Empty
        Dim isCoursePdt As Boolean = False
        Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
        Dim priceBands(25, 1) As String
        Dim buyingUsingMultiPriceBands As Boolean = False
        If Session("SelectedPriceBands") IsNot Nothing Then
            priceBands = Session("SelectedPriceBands")
            Session("SelectedPriceBands") = Nothing
            buyingUsingMultiPriceBands = True
        Else
            priceBands = retrievePriceBandSelectionFromQueryString()
            If Not priceBands Is Nothing Then
                buyingUsingMultiPriceBands = True
            End If
        End If

        If String.IsNullOrEmpty(product) Then
            missingParam = True
            Me._missingParamErrorCode = "PR"
        ElseIf String.IsNullOrEmpty(stand) Then
            missingParam = True
            Me._missingParamErrorCode = "ST"
        ElseIf String.IsNullOrEmpty(area) Then
            missingParam = True
            Me._missingParamErrorCode = "AR"
        ElseIf String.IsNullOrEmpty(quantity) Then
            returnErrorCode = "AAQ"
            missingParam = True
            Me._missingParamErrorCode = "QT"
        ElseIf String.IsNullOrEmpty(priceBand) Then
            missingParam = True
            Me._missingParamErrorCode = "PB"
        End If

        If AgentProfile.IsAgent AndAlso Not AgentProfile.AgentPermissions.CanAddHomeGameToBasket Then
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If

        Dim isItCATSeatSelection As Boolean = False
        'if seat selection is for CAT then check required param are exists
        missingParam = IsCATParamMissing()

        If favouriteSeat IsNot Nothing Then
            Try
                If CBool(favouriteSeat) Then
                    favouriteSeatSelected = True
                End If
            Catch ex As Exception
                favouriteSeatSelected = False
            End Try
        End If

        If Not Profile.IsAnonymous Then
            customerNumber = Profile.User.Details.LoginID.PadLeft(12, "0")
        Else
            If ModuleDefaults.LoginWhenAddingHomeGameToBasket Then
                _redirectUrl = "~/PagesPublic/Login/login.aspx?ReturnUrl=" & Server.UrlEncode(Request.Url.ToString)
            Else
                customerNumber = "000000000000"
            End If
        End If

        'Check whether product is under course stadium
        isCoursePdt = IsCourseProduct(productStadium)
        If isCoursePdt Then
            If String.IsNullOrEmpty(productSubType) Then
                productSubType = ""
            ElseIf String.IsNullOrEmpty(productStadium) Then
                productStadium = ""
            End If
        End If

        If Not missingParam Then
            If isCoursePdt Then
                'availability pre check if exists to registration page else product page
                Dim quantityRequest As Integer = CInt(quantity)
                If quantityRequest <= ModuleDefaults.CourseProductMaxQuantity Then
                    If quantityRequest <= GetTKTAvailableQuantity(productType, product, campaignCode, stand, area) Then
                        _redirectUrl = "~/PagesPublic/Profile/RegistrationParticipants.aspx?productCode=" & product & "&productType=" & productType & "&productStadium=" & productStadium & "&productSubType=" & productSubType & "&quantity=" & quantityRequest & "&standCode=" & stand & "&areaCode=" & area & "&campaignCode=" & campaignCode
                    Else
                        returnError = True
                        returnErrorCode = "NS"
                    End If
                Else
                    returnError = True
                    returnErrorCode = "NS"
                End If
            Else
                If String.IsNullOrEmpty(_linkedMasterProduct) Then
                    Session("QuantityRequested") = quantity
                End If
                Dim deATI As New Talent.Common.DEAddTicketingItems
                'if seat selection is for CAT populate the CAT ticketing item details
                deATI = GetCATTicketingItemDetails(False)
                With deATI
                    .SessionId = Profile.Basket.Basket_Header_ID
                    .CustomerNumber = customerNumber
                    .AreaCode = area
                    .StandCode = stand
                    .ProductCode = product
                    If buyingUsingMultiPriceBands Then
                        .PriceBand01 = priceBands(0, 0)
                        .Quantity01 = priceBands(0, 1)
                        .PriceBand02 = priceBands(1, 0)
                        .Quantity02 = priceBands(1, 1)
                        .PriceBand03 = priceBands(2, 0)
                        .Quantity03 = priceBands(2, 1)
                        .PriceBand04 = priceBands(3, 0)
                        .Quantity04 = priceBands(3, 1)
                        .PriceBand05 = priceBands(4, 0)
                        .Quantity05 = priceBands(4, 1)
                        .PriceBand06 = priceBands(5, 0)
                        .Quantity06 = priceBands(5, 1)
                        .PriceBand07 = priceBands(6, 0)
                        .Quantity07 = priceBands(6, 1)
                        .PriceBand08 = priceBands(7, 0)
                        .Quantity08 = priceBands(7, 1)
                        .PriceBand09 = priceBands(8, 0)
                        .Quantity09 = priceBands(8, 1)
                        .PriceBand10 = priceBands(9, 0)
                        .Quantity10 = priceBands(9, 1)
                        .PriceBand11 = priceBands(10, 0)
                        .Quantity11 = priceBands(10, 1)
                        .PriceBand12 = priceBands(11, 0)
                        .Quantity12 = priceBands(11, 1)
                        .PriceBand13 = priceBands(12, 0)
                        .Quantity13 = priceBands(12, 1)
                        .PriceBand14 = priceBands(13, 0)
                        .Quantity14 = priceBands(13, 1)
                        .PriceBand15 = priceBands(14, 0)
                        .Quantity15 = priceBands(14, 1)
                        .PriceBand16 = priceBands(15, 0)
                        .Quantity16 = priceBands(15, 1)
                        .PriceBand17 = priceBands(16, 0)
                        .Quantity17 = priceBands(16, 1)
                        .PriceBand18 = priceBands(17, 0)
                        .Quantity18 = priceBands(17, 1)
                        .PriceBand19 = priceBands(18, 0)
                        .Quantity19 = priceBands(18, 1)
                        .PriceBand20 = priceBands(19, 0)
                        .Quantity21 = priceBands(19, 1)
                        .PriceBand21 = priceBands(20, 0)
                        .Quantity21 = priceBands(20, 1)
                        .PriceBand22 = priceBands(21, 0)
                        .Quantity22 = priceBands(21, 1)
                        .PriceBand23 = priceBands(22, 0)
                        .Quantity23 = priceBands(22, 1)
                        .PriceBand24 = priceBands(23, 0)
                        .Quantity24 = priceBands(23, 1)
                        .PriceBand25 = priceBands(24, 0)
                        .Quantity25 = priceBands(24, 1)
                    Else
                        .Quantity01 = quantity
                        .PriceBand01 = priceBand
                    End If
                    .LinkedMasterProduct = If(product.Equals(_linkedMasterProduct), String.Empty, _linkedMasterProduct)
                    .LinkedProductID = ticketingGatewayFunctions.ReturnLinkedProductIDFromBasket(_linkedMasterProduct)

                    .FavouriteSeatSelected = favouriteSeatSelected
                    .SeatSelectionArray = GetFormattedSeatList(HttpContext.Current.Session("CATSeatsInSeatSelection"), String.Empty, False)
                    If defaultPrice IsNot Nothing Then .DefaultPrice = CheckForDBNull_Int(defaultPrice.Replace(".", String.Empty))
                    .Source = "W"
                    .ProductType = productType
                    .ProductHasMandtoryRelatedProducts = _productHasMandatoryRelatedProducts
                    If Not Session("Agent") Is Nothing AndAlso Not Session("Agent") = "" AndAlso Not Session("AgentType") Is Nothing AndAlso Session("AgentType") = "2" Then
                        .ByPassPreReqCheck = "Y"
                    Else
                        .ByPassPreReqCheck = "N"
                    End If
                    .SelectedPriceMinimum = TEBUtilities.CheckForDBNull_Decimal(selectedMinPrice)
                    .SelectedPriceMaximum = TEBUtilities.CheckForDBNull_Decimal(selectedMaxPrice)
                    .SelectedPriceBreakId = TEBUtilities.CheckForDBNull_Long(priceBreakId)
                End With

                Dim basket As New Talent.Common.TalentBasket
                basket.DeAddTicketingItems = deATI
                basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
                basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
                basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
                basket.Settings.PerformWatchListCheck = ModuleDefaults.PerformAgentWatchListCheck
                basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
                basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)

                Dim err As New Talent.Common.ErrorObj

                Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductHome_AddToBasket", deATI.CustomerNumber, "TalentEBusiness Request to Talent.Common.AmendBasket.AddTicketingItemsReturnBasket")
                err = basket.AddTicketingItemsReturnBasket
                Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductHome_AddToBasket", deATI.CustomerNumber, "TalentEBusiness Response from Talent.Common.AmendBasket.AddTicketingItemsReturnBasket")

                returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)

                'if exists clear cat sessions if cleared return true
                If Not String.IsNullOrWhiteSpace(HttpContext.Current.Session("catmode")) Then
                    Session("catpayref") = Session("payref")
                End If
                isItCATSeatSelection = IsCATSessionsRemoved()

                If returnErrorCode.Length > 0 Then
                    returnError = True
                    If returnErrorCode.Equals("NC") Then
                        clearStandAreaCache(basket)
                        returnErrorCode = "NS"
                    End If
                End If
                If basket.ClearAvailableStandAreaCache Then clearStandAreaCache(basket)
                If basket.AlternativeSeatSelected Then Session("AssignedAlternativeSeat") = True
                If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                    _redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                    _packageId = basket.DeAddTicketingItems.PackageID
                End If
                _TicketingGatewayFunctions.SetSessionValuesFromBasket(basket)


            End If

        Else 'missing param else
            If returnErrorCode.Trim = "" Then
                returnErrorCode = "PARAMERR"
                Talent.eCommerce.Logging.WriteLog(Profile.UserName, "PH_ATB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - ProductHome_AddToBasket", "Error")
            End If
            returnError = True
        End If
        If isItCATSeatSelection Then
            If returnError Then
                Session("TicketingGatewayError") = returnErrorCode
                Session("TalentErrorCode") = returnErrorCode
            End If
            If returnErrorCode = "WA" OrElse returnErrorCode = "AC" Then
                Talent.eCommerce.Utilities.ClearOrderEnquiryDetailsCache()
            End If
            _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
        Else
            If returnError Then
                Session("TicketingGatewayError") = returnErrorCode
                Session("TalentErrorCode") = returnErrorCode
                If (Not String.IsNullOrWhiteSpace(productType)) AndAlso UCase(productType) = "S" Then
                    Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductHome_AddToBasket", "", "TalentEBusiness Redirect to ~/PagesPublic/ProductBrowse/ProductSeason.aspx as referer is StandAndAreaSelection Flash")
                    _redirectUrl = "../PagesPublic/ProductBrowse/ProductSeason.aspx" & ticketingGatewayFunctions.GetProductDetailQueryString(True)
                Else
                    Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductHome_AddToBasket", "", "TalentEBusiness Redirect to ~/PagesPublic/ProductBrowse/ProductHome.aspx")
                    _redirectUrl = "../PagesPublic/ProductBrowse/ProductHome.aspx" & ticketingGatewayFunctions.GetProductDetailQueryString(True)
                End If
            Else
                If ModuleDefaults.HomeProduct_ForwardToBasket Then
                    Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductHome_AddToBasket", customerNumber, "TalentEBusiness Redirect to ~/PagesPublic/Basket/Basket.aspx")
                    _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                    _redirectToProductLinkingPage = True
                Else
                    Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductHome_AddToBasket", customerNumber, "TalentEBusiness Redirect to ~/PagesPublic/ProductBrowse/ProductHome.aspx")
                    _redirectUrl = "~/PagesPublic/ProductBrowse/ProductHome.aspx" & ticketingGatewayFunctions.GetProductDetailQueryString(True)
                    _redirectToProductLinkingPage = True
                End If
            End If
        End If
    End Sub

    Private Sub ProductSeason_AddToBasket()
        Dim product As String = Request.QueryString("product")
        Dim stand As String = Request.QueryString("stand")
        Dim area As String = Request.QueryString("area")
        Dim quantity As String = Request.QueryString("quantity")
        Dim priceBand As String = Request.QueryString("priceBand")
        Dim campaignCode As String = Request.QueryString("campaign")
        Dim productType As String = Request.QueryString("type")
        Dim productSubType As String = Request.QueryString("productsubtype")
        Dim productStadium As String = Request.QueryString("productstadium")
        Dim favouriteSeat As String = Request.QueryString("favouriteSeat")
        Dim favouriteSeatSelected As Boolean = False
        Dim missingParam As Boolean = False
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim customerNumber As String = ""
        Dim isCoursePdt As Boolean = False

        If String.IsNullOrEmpty(product) Then
            missingParam = True
            Me._missingParamErrorCode = "PR"
        ElseIf String.IsNullOrEmpty(stand) Then
            missingParam = True
            Me._missingParamErrorCode = "ST"
        ElseIf String.IsNullOrEmpty(area) Then
            missingParam = True
            Me._missingParamErrorCode = "AR"
        ElseIf String.IsNullOrEmpty(quantity) Then
            returnErrorCode = "AAQ"
            missingParam = True
            Me._missingParamErrorCode = "QT"
        ElseIf String.IsNullOrEmpty(priceBand) Then
            missingParam = True
            Me._missingParamErrorCode = "PB"
        End If

        If favouriteSeat IsNot Nothing Then
            Try
                If CBool(favouriteSeat) Then
                    favouriteSeatSelected = True
                End If
            Catch ex As Exception
                favouriteSeatSelected = False
            End Try
        End If

        If Not Profile.IsAnonymous Then
            customerNumber = Profile.User.Details.LoginID.PadLeft(12, "0")
        Else
            _redirectUrl = "~/PagesPublic/Login/login.aspx?ReturnUrl=" & Server.UrlEncode("~/PagesPublic/ProductBrowse/ProductSeason.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(True))
        End If

        If AgentProfile.IsAgent AndAlso Not AgentProfile.AgentPermissions.CanAddSeasonGameToBasket Then
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If

        'Check whether product is under course stadium
        isCoursePdt = IsCourseProduct(productStadium)
        If isCoursePdt Then
            If String.IsNullOrEmpty(productSubType) Then
                productSubType = ""
            ElseIf String.IsNullOrEmpty(productStadium) Then
                productStadium = ""
            End If
        End If

        If Not missingParam Then
            If isCoursePdt Then
                'availability pre check if exists to registration page else product page
                Dim quantityRequest As Integer = CInt(quantity)
                If quantityRequest <= ModuleDefaults.CourseProductMaxQuantity Then
                    If quantityRequest <= GetTKTAvailableQuantity(productType, product, campaignCode, stand, area) Then
                        _redirectUrl = "~/PagesPublic/Profile/RegistrationParticipants.aspx?productCode=" & product & "&productType=" & productType & "&productStadium=" & productStadium & "&productSubType=" & productSubType & "&quantity=" & quantityRequest & "&standCode=" & stand & "&areaCode=" & area & "&campaignCode=" & campaignCode
                    Else
                        returnError = True
                        returnErrorCode = "NS"
                    End If
                Else
                    returnError = True
                    returnErrorCode = "NS"
                End If
            Else
                If String.IsNullOrEmpty(_linkedMasterProduct) Then
                    Session("QuantityRequested") = quantity
                End If
                Dim deATI As New Talent.Common.DEAddTicketingItems
                With deATI
                    .SessionId = Profile.Basket.Basket_Header_ID
                    .CustomerNumber = customerNumber
                    .AreaCode = area
                    .StandCode = stand
                    .ProductCode = product
                    .Quantity01 = quantity
                    .LinkedMasterProduct = If(product.Equals(_linkedMasterProduct), String.Empty, _linkedMasterProduct)
                    .PriceBand01 = priceBand
                    .FavouriteSeatSelected = favouriteSeatSelected
                    .CampaignCode = campaignCode
                    .ProductHasMandtoryRelatedProducts = _productHasMandatoryRelatedProducts
                    .Source = "W"
                End With

                Dim basket As New Talent.Common.TalentBasket
                basket.DeAddTicketingItems = deATI
                basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
                basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
                basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
                basket.Settings.PerformWatchListCheck = ModuleDefaults.PerformAgentWatchListCheck
                basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
                basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
                Dim err As New Talent.Common.ErrorObj
                Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductSeason_AddToBasket", deATI.CustomerNumber, "TalentEBusiness Request to Talent.Common.AmendBasket.AddTicketingItemsReturnBasket")
                err = basket.AddTicketingItemsReturnBasket
                Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductSeason_AddToBasket", deATI.CustomerNumber, "TalentEBusiness Response from Talent.Common.AmendBasket.AddTicketingItemsReturnBasket")
                returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)

                If returnErrorCode.Length > 0 Then
                    returnError = True
                    If returnErrorCode.Equals("NC") Then
                        clearStandAreaCache(basket)
                        returnErrorCode = "NS"
                    End If
                End If
                If basket.ClearAvailableStandAreaCache Then clearStandAreaCache(basket)
                If basket.AlternativeSeatSelected Then Session("AssignedAlternativeSeat") = True
                If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                    _redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                    _packageId = basket.DeAddTicketingItems.PackageID
                End If
                _TicketingGatewayFunctions.SetSessionValuesFromBasket(basket)
                If basket.BasketHasExceptionSeats Then _redirectToSeasonTicketExceptionsPage = True
            End If

        Else 'missing param else
            If returnErrorCode.Trim = "" Then
                returnErrorCode = "PARAMERR"
                Talent.eCommerce.Logging.WriteLog(Profile.UserName, "PS_ATB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - ProductSeason_AddToBasket", "Error")
            End If
            returnError = True
        End If

        If returnError Then
            Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductSeason_AddToBasket", "", "TalentEBusiness Redirect to ~/PagesPublic/ProductBrowse/ProductHome.aspx")
            Session("TicketingGatewayError") = returnErrorCode
            Session("TalentErrorCode") = returnErrorCode
            _redirectUrl = "../PagesPublic/ProductBrowse/ProductSeason.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(True)
        Else
            If (ModuleDefaults.PPS_ENABLE_1 AndAlso Not AgentProfile.BulkSalesMode) Then
                Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductSeason_AddToBasket", customerNumber, "TalentEBusiness Redirect to ~/PagesPublic/ProductBrowse/TicketingPrePayment.aspx?ppspage=1")
                _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=1&product=" & Request.QueryString("product") & "&pricecode=" & campaignCode
            ElseIf (ModuleDefaults.PPS_ENABLE_2 AndAlso Not AgentProfile.BulkSalesMode) Then
                Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductSeason_AddToBasket", customerNumber, "TalentEBusiness Redirect to ~/PagesPublic/ProductBrowse/TicketingPrePayment.aspx?ppspage=2")
                _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=2&product=" & Request.QueryString("product") & "&pricecode=" & campaignCode
            Else
                Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductSeason_AddToBasket", customerNumber, "TalentEBusiness Redirect to ~/PagesPublic/Basket/Basket.aspx")
                _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                _redirectToProductLinkingPage = True
            End If
        End If
    End Sub

    Private Sub ProductHome_AddReservedItemsToBasket()
        Dim myTicketingMenu As New TalentTicketingMenu
        myTicketingMenu.LoadTicketingProducts(TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), Talent.eCommerce.Utilities.GetCurrentLanguage)
        If myTicketingMenu.TicketingProductIsActive("CUP") Then
            Dim priceCode As String = String.Empty
            Dim returnError As Boolean = False
            Dim returnErrorCode As String = String.Empty
            Dim product As String = Request("product")
            Dim addQuantity As String = String.Empty
            Dim quantityResult As Integer
            If String.IsNullOrEmpty(product) Then product = String.Empty
            'Checks to see if the quantity has been entered for reserved tickets
            If Not String.IsNullOrEmpty(Request.QueryString("qty")) And (Integer.TryParse(Request.QueryString("qty"), quantityResult)) And quantityResult > 0 Then addQuantity = Request.QueryString("qty").ToString.Trim
            ' The user must be logged in to use this function
            If Not Profile.IsAnonymous Then
                ' Populate the data entity
                Dim deATI As New Talent.Common.DEAddTicketingItems
                With deATI
                    .SessionId = Profile.Basket.Basket_Header_ID
                    .CustomerNumber = Profile.User.Details.LoginID.PadLeft(12, "0")
                    If Not String.IsNullOrEmpty(addQuantity) Then
                        .Quantity01 = addQuantity
                    End If
                    If Not String.IsNullOrEmpty(product) Then
                        .ProductCode = product
                    End If
                    .Source = "W"
                    If Not Session("Agent") Is Nothing AndAlso Not Session("Agent") = "" AndAlso Not Session("AgentType") Is Nothing AndAlso Session("AgentType") = "2" Then
                        .ByPassPreReqCheck = "Y"
                    Else
                        .ByPassPreReqCheck = "N"
                    End If
                End With

                ' Add the customer reserved items to the basket
                Dim basket As New Talent.Common.TalentBasket
                basket.DeAddTicketingItems = deATI
                basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
                basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
                basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
                basket.Settings.PerformWatchListCheck = ModuleDefaults.PerformAgentWatchListCheck
                basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
                basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
                Dim err As New Talent.Common.ErrorObj
                err = basket.AddTicketingReservedItemsReturnBasket

                returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)
                If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                    _redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                    _packageId = basket.DeAddTicketingItems.PackageID
                End If

                If returnErrorCode.Length = 0 Then
                    'Set the appropriate message for the customer
                    If basket.DeAddTicketingItems.ReservedSeats = "Y" Then
                        ' returnErrorCode = "AAG"
                    Else
                        returnErrorCode = "AAF"
                        returnError = True
                    End If
                End If

                If Not returnError Then
                    Dim tickets As Data.DataTable = Nothing
                    Dim productSubType As String = String.Empty
                    tickets = basket.ResultDataSet.Tables(2)
                    For Each ticket As DataRow In tickets.Rows
                        If UCase(Utilities.CheckForDBNull_String(ticket("ProductType"))) = GlobalConstants.TICKETINGPRODUCTTYPE Then
                            _autoAddProductCode = Utilities.CheckForDBNull_String(ticket("ProductCode"))
                            priceCode = Utilities.CheckForDBNull_String(ticket("PriceCode"))
                            productSubType = Utilities.CheckForDBNull_String(ticket("ProductSubType"))
                            Exit For
                        End If
                    Next
                    'Check to see if there are any linked products to this reserved game.
                    'Check here as the product details are only known after they've been added to the the basket.
                    Dim dtProductRelations As New DataTable
                    _productHasRelatedProducts = _TicketingGatewayFunctions.DoesProductHaveRelatedProducts(_autoAddProductCode, dtProductRelations, priceCode, productSubType)
                    If _productHasRelatedProducts Then _productHasMandatoryRelatedProducts = _TicketingGatewayFunctions.IsAnyLinkedProductsMandatory(_autoAddProductCode, dtProductRelations)
                End If
            Else
                ' The user must login before we can use this fuction
                _redirectUrl = "~/PagesPublic/Login/Login.aspx?ReturnUrl=" & Request.Url.AbsolutePath & "?" + HttpContext.Current.Server.UrlEncode("page=home.aspx&function=AddReservedItemsToBasket&product=" & product)
            End If

            If returnError Then
                'Set the message code to be displayed on the basket
                If returnErrorCode.Length > 0 Then
                    Session("TicketingGatewayError") = returnErrorCode
                    Session("TalentErrorCode") = returnErrorCode
                End If
            End If

            If Session("ReturnUrl") Is Nothing Then
                _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                _redirectToProductLinkingPage = True
            Else
                _redirectUrl = Session("ReturnUrl")
                _redirectToProductLinkingPage = False
                Session("ReturnUrl") = Nothing
            End If
        Else
            _redirectUrl = "~/PagesPublic/Error/Unavailable.aspx"
        End If
    End Sub

    Private Sub ProductSeason_AddSeasonTicketRenewalsToBasket()
        Dim myTicketingMenu As New TalentTicketingMenu
        myTicketingMenu.LoadTicketingProducts(TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), Talent.eCommerce.Utilities.GetCurrentLanguage)
        If myTicketingMenu.TicketingProductIsActive("SEASONRENEWALS") Then
            Dim priceCode As String = String.Empty
            Dim returnError As Boolean = False
            Dim returnErrorCode As String = String.Empty

            ' The user must be logged in to use this function - this could have been set in the base page too (me.validUser)
            If (Not Profile.IsAnonymous Or Me.ValidUser) Then

                ' Do we need to update the customers detail before we continue
                Dim updateDetails As String = Request.QueryString("updateDetails")
                If ModuleDefaults.UpdateDetailsForSeasonTickets AndAlso String.IsNullOrEmpty(updateDetails) Then
                    Dim updatedWithin24Hours As Boolean = False
                    Try
                        If CType(Membership.Provider, TalentMembershipProvider).GetUser(Profile.UserName, True).LastPasswordChangedDate > Now.AddDays(-1) Then
                            updatedWithin24Hours = True
                        End If
                    Catch ex As Exception
                    End Try

                    If Not updatedWithin24Hours Then
                        _redirectUrl = "~/PagesLogin/Profile/updateProfile.aspx?ReturnUrl=" & HttpContext.Current.Server.UrlEncode(Request.Url.AbsolutePath & "?page=home.aspx&function=AddSeasonTicketRenewalsToBasket&updateDetails=Y")
                    End If
                End If

                ' Populate the data entity
                Dim deATI As New Talent.Common.DEAddTicketingItems
                With deATI
                    .SessionId = Profile.Basket.Basket_Header_ID
                    .CustomerNumber = Profile.User.Details.LoginID.PadLeft(12, "0")
                    .ProductHasMandtoryRelatedProducts = _productHasMandatoryRelatedProducts
                    .Source = "W"
                    If ModuleDefaults.IgnoreFFforSTRenewals Then
                        .IgnoreFriendsAndFamily = "Y"
                    End If
                    Try
                        Dim sStadia() As String = ModuleDefaults.TicketingStadium.Split(",")
                        Dim count As Integer = 0
                        Do While count < sStadia.Length
                            Select Case count
                                Case Is = 0 : .Stadium1 = sStadia(0)
                                Case Is = 1 : .Stadium2 = sStadia(1)
                                Case Is = 2 : .Stadium3 = sStadia(2)
                                Case Is = 3 : .Stadium4 = sStadia(3)
                                Case Is = 4 : .Stadium5 = sStadia(4)
                            End Select
                            count = count + 1
                        Loop
                    Catch ex As Exception
                    End Try
                End With

                ' Add the customer reserved items to the basket
                Dim basket As New Talent.Common.TalentBasket
                basket.DeAddTicketingItems = deATI
                basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
                basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
                basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
                basket.Settings.PerformWatchListCheck = ModuleDefaults.PerformAgentWatchListCheck
                basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
                basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
                Dim err As New Talent.Common.ErrorObj
                err = basket.AddSeasonTicketRenewalsReturnBasket

                ' Check for errors
                returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)
                If returnErrorCode.Length = 0 OrElse returnErrorCode = "WF" Then
                    'Set the appropriate message for the customer
                    If basket.DeAddTicketingItems.ReservedSeats = "Y" Then
                        ' returnErrorCode = "AAG"
                    Else
                        returnError = True
                        returnErrorCode = "AAH"
                    End If
                Else
                    returnError = True
                End If

                If Not returnError Then
                    If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                        _redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                        _packageId = basket.DeAddTicketingItems.PackageID
                    End If
                    If basket.BasketHasExceptionSeats Then _redirectToSeasonTicketExceptionsPage = True
                    Dim tickets As Data.DataTable = Nothing
                    Dim productSubType As String = String.Empty
                    tickets = basket.ResultDataSet.Tables(2)
                    For Each ticket As DataRow In tickets.Rows
                        If UCase(Utilities.CheckForDBNull_String(ticket("ProductType"))) = GlobalConstants.SEASONTICKETPRODUCTTYPE Then
                            _autoAddProductCode = Utilities.CheckForDBNull_String(ticket("ProductCode"))
                            priceCode = Utilities.CheckForDBNull_String(ticket("PriceCode"))
                            productSubType = Utilities.CheckForDBNull_String(ticket("ProductSubType"))
                            Exit For
                        End If
                    Next
                    'Check to see if there are any linked products to this season ticket renewal.
                    'Check here as the product details are only known after they've been added to the the basket.
                    Dim dtProductRelations As New DataTable
                    _productHasRelatedProducts = _TicketingGatewayFunctions.DoesProductHaveRelatedProducts(_autoAddProductCode, dtProductRelations, priceCode, productSubType)
                    If _productHasRelatedProducts Then _productHasMandatoryRelatedProducts = _TicketingGatewayFunctions.IsAnyLinkedProductsMandatory(_autoAddProductCode, dtProductRelations)
                End If

                'Set the message code to be displayed on the basket
                If returnErrorCode.Length > 0 Then
                    Session("TicketingGatewayError") = returnErrorCode
                    Session("TalentErrorCode") = returnErrorCode
                    returnError = True
                End If
                If ModuleDefaults.PPS_ENABLE_1 AndAlso Not returnError AndAlso Not AgentProfile.BulkSalesMode Then
                    _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=1&product=" & _autoAddProductCode & "&pricecode=" & priceCode
                ElseIf ModuleDefaults.PPS_ENABLE_2 AndAlso Not returnError AndAlso Not AgentProfile.BulkSalesMode Then
                    _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=2&product=" & _autoAddProductCode & "&pricecode=" & priceCode
                Else
                    _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                    _redirectToProductLinkingPage = True
                End If
            Else
                ' The user must login before we can use this fuction
                _redirectUrl = "~/PagesPublic/Login/Login.aspx?ReturnUrl=" & HttpContext.Current.Server.UrlEncode(Request.Url.AbsolutePath & "?page=home.aspx&function=AddSeasonTicketRenewalsToBasket")
            End If
        Else
            _redirectUrl = "~/PagesPublic/Error/Unavailable.aspx"
        End If
    End Sub

    Private Sub AddSeasonTicketWaitList()
        Dim returnErrorCode As String = String.Empty
        Dim returnError As Boolean = False
        Dim missingParam As Boolean = False
        Dim prefStand1 As String = Request.QueryString("stand1")
        Dim prefArea1 As String = Request.QueryString("area1")
        Dim prefStand2 As String = Request.QueryString("stand2")
        Dim prefArea2 As String = Request.QueryString("area2")
        Dim prefStand3 As String = Request.QueryString("stand3")
        Dim prefArea3 As String = Request.QueryString("area3")
        Dim qty As String = Request.QueryString("qty")
        Dim prodCode As String = Request.QueryString("product")
        Dim dPhone As String = Request.QueryString("dp")
        Dim ePhone As String = Request.QueryString("ep")
        Dim mPhone As String = Request.QueryString("mp")
        Dim email As String = Request.QueryString("email")
        Dim comm1 As String = Request.QueryString("c1")
        Dim comm2 As String = Request.QueryString("c2")
        Dim customerList As String = Request.QueryString("customerList")

        If String.IsNullOrEmpty(prefStand1) Then
            missingParam = True
            Me._missingParamErrorCode = "AWL-ST"
        ElseIf String.IsNullOrEmpty(prefArea1) Then
            missingParam = True
            Me._missingParamErrorCode = "AWL-AR"
        ElseIf String.IsNullOrEmpty(qty) Then
            missingParam = True
            Me._missingParamErrorCode = "AWL-QT"
        ElseIf Not String.IsNullOrEmpty(qty) Then
            Try
                Dim i As Integer = CInt(qty)
            Catch ex As Exception
                missingParam = True
                Me._missingParamErrorCode = "AWL-QT"
            End Try
        ElseIf String.IsNullOrEmpty(prodCode) Then
            missingParam = True
            Me._missingParamErrorCode = "AWL-PR"
        ElseIf String.IsNullOrEmpty(customerList) Then
            missingParam = True
            Me._missingParamErrorCode = "AWL-CL"
        End If

        If Not missingParam Then

            If String.IsNullOrEmpty(prefStand2) Then prefStand2 = ""
            If String.IsNullOrEmpty(prefStand3) Then prefStand3 = ""
            If String.IsNullOrEmpty(prefArea2) Then prefArea2 = ""
            If String.IsNullOrEmpty(prefArea3) Then prefArea3 = ""

            Dim ucr As New UserControlResource
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "SeasonTicketWaitList.ascx"
            End With

            Dim de As New Talent.Common.DEWaitList
            Dim twl As New TalentWaitList
            twl.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            twl.DE = de
            With twl.Settings
                .BusinessUnit = TalentCache.GetBusinessUnit
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            End With

            Dim actionComments As New Generic.List(Of String)
            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DaytimePhoneNumber_InUse")) Then
                If Not String.IsNullOrEmpty(dPhone) Then
                    actionComments.Add(ucr.Content("DaytimePhoneLabel", Talent.eCommerce.Utilities.GetCurrentLanguage, True) & " - " & dPhone)
                End If
            End If
            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("EveningPhoneNumber_InUse")) Then
                If Not String.IsNullOrEmpty(ePhone) Then
                    actionComments.Add(ucr.Content("EveningPhoneLabel", Talent.eCommerce.Utilities.GetCurrentLanguage, True) & " - " & ePhone)
                End If
            End If
            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("MobilePhoneNumber_InUse")) Then
                If Not String.IsNullOrEmpty(mPhone) Then
                    actionComments.Add(ucr.Content("MobilePhoneLabel", Talent.eCommerce.Utilities.GetCurrentLanguage, True) & " - " & mPhone)
                End If
            End If
            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("EmailAddress_InUse")) Then
                If Not String.IsNullOrEmpty(email) Then
                    Dim emailEntry As String = ucr.Content("EmailAddressLabel", Talent.eCommerce.Utilities.GetCurrentLanguage, True)
                    Dim emailEntry2 As String = ""
                    emailEntry += " - "
                    If emailEntry.Length + email.Length <= 60 Then
                        emailEntry += email
                    Else
                        Dim prefixLen As Integer = emailEntry.Length
                        emailEntry += email.Substring(0, 60 - prefixLen)
                        emailEntry2 = email.Substring((60 - prefixLen), email.Length - (60 - prefixLen))
                    End If
                    actionComments.Add(emailEntry)
                    If Not String.IsNullOrEmpty(emailEntry2) Then
                        actionComments.Add(emailEntry2)
                    End If
                End If
            End If

            Dim err As New ErrorObj
            With de
                .RequestType = DEWaitList.WaitListType.Add
                .CustomerNumber = Profile.UserName
                .Src = "W"
                .CurrentSeasonTicketProduct = prodCode
                .PreferredArea1 = prefArea1
                .PreferredArea2 = prefArea2
                .PreferredArea3 = prefArea3
                .PreferredStand1 = prefStand1
                .PreferredStand2 = prefStand2
                .PreferredStand3 = prefStand3
                .CustomerEmailAddress = email

                If Not String.IsNullOrEmpty(dPhone) Then
                    .CustomerPhoneNo = dPhone
                ElseIf Not String.IsNullOrEmpty(ePhone) Then
                    .CustomerPhoneNo = ePhone
                ElseIf Not String.IsNullOrEmpty(mPhone) Then
                    .CustomerPhoneNo = mPhone
                End If

                If Not String.IsNullOrEmpty(comm1) Then .Comment1 = comm1
                If Not String.IsNullOrEmpty(comm2) Then .Comment2 = comm2

                .Quantity = qty
                Dim aCust As Array = customerList.Split(",")
                For i As Integer = 1 To qty
                    .WaitListRequests.Add(aCust(i - 1))
                Next

                .CheckPendingRequests = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("CheckPendingRequests"))

                If actionComments.Count > 0 Then .ActionComment1 = actionComments(0)
                If actionComments.Count > 1 Then .ActionComment2 = actionComments(1)
                If actionComments.Count > 2 Then .ActionComment3 = actionComments(2)
                If actionComments.Count > 3 Then .ActionComment4 = actionComments(3)
                If actionComments.Count > 4 Then .ActionComment5 = actionComments(4)
                If actionComments.Count > 5 Then .ActionComment6 = actionComments(5)
            End With

            err = twl.AddCustomerWaitListRequest

            If Not err.HasError Then
                returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(twl.ResultDataSet, err)
                If returnErrorCode.Length > 0 Then
                    returnError = True
                End If
                If returnError Then
                    Session("TicketingGatewayError") = returnErrorCode
                    Session("TalentErrorCode") = returnErrorCode
                End If
            Else
                returnError = True
                returnErrorCode = "AWLERR"
            End If
        Else
            returnError = True
            Session("TicketingGatewayError") = "PARAMERR"
            Session("TalentErrorCode") = Me._missingParamErrorCode
        End If

        If returnError Then
            _redirectUrl = "~/PagesLogin/WaitList/WaitList.aspx"
        Else
            _redirectUrl = "~/PagesLogin/WaitList/WaitListConfirmation.aspx?email=" & email
        End If
    End Sub

    Private Sub WithdrawSeasonTicketWaitList()
        Dim returnErrorCode As String = String.Empty
        Dim returnError As Boolean = False
        Dim missingParam As Boolean = False
        Dim waitListID As String = Request.QueryString("wlid")
        If String.IsNullOrEmpty(waitListID) Then
            missingParam = True
            Me._missingParamErrorCode = "WWL-WL"
        End If

        If Not missingParam Then
            Dim de As New Talent.Common.DEWaitList
            Dim twl As New TalentWaitList
            twl.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            twl.DE = de
            twl.DE.WaitListID = waitListID
            With twl.Settings
                .BusinessUnit = TalentCache.GetBusinessUnit
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            End With

            Dim err As New ErrorObj
            With de
                .RequestType = DEWaitList.WaitListType.Withdraw
                .CustomerNumber = Profile.UserName
                .Src = "W"
            End With

            err = twl.WithdrawCustomerWaitListRequest

            If Not err.HasError Then
                returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(twl.ResultDataSet, err)
                If returnErrorCode.Length > 0 Then
                    returnError = True
                End If
                If returnError Then
                    Session("TicketingGatewayError") = returnErrorCode
                    Session("TalentErrorCode") = returnErrorCode
                End If
            Else
                returnError = True
                returnErrorCode = "WWLERR"
            End If
        Else
            returnError = True
            Session("TicketingGatewayError") = "PARAMERR"
            Session("TalentErrorCode") = Me._missingParamErrorCode
        End If

        _redirectUrl = "~/PagesLogin/WaitList/WaitList.aspx"
    End Sub

    Private Sub FreeMembershipProduct_AddToBasket()
        Dim returnErrorCode As String = String.Empty
        Dim returnError As Boolean = False
        Dim basket As New Talent.Common.TalentBasket
        basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
        basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
        With basket.Settings
            .BusinessUnit = TalentCache.GetBusinessUnit
            .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            .AutoAddMembership = True
            .ModuleName = "AddTicketingReservedItemsReturnBasket"
            .OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
        End With

        Dim deATI As New Talent.Common.DEAddTicketingItems
        With deATI
            .SessionId = Profile.Basket.Basket_Header_ID
            If Profile.IsAnonymous Then
                .CustomerNumber = "000000000000"
            Else
                '-----------------------------------------------------
                ' Also send down the signed in customer to the backend
                ' This is who the payment will go against
                '-----------------------------------------------------
                If Not Profile.User.Details.LoginID Is Nothing Then
                    .SignedInCustomer = Profile.User.Details.LoginID
                End If

                If Not Request("customer") Is Nothing AndAlso Not Request("customer") = String.Empty Then
                    ' Check for a customer value in the request which is encoded
                    .CustomerNumber = Request("customer")
                    'if request
                Else
                    ' Check for a customer value in the request which is encoded
                    Dim rawUrl As String = Request.RawUrl
                    Dim customerSearchstring As String = "%3fcustomer%3d"
                    If rawUrl.Contains(customerSearchstring) Then
                        Dim customerNoFromUrl As String
                        customerNoFromUrl = rawUrl.Substring(rawUrl.LastIndexOf(customerSearchstring) + customerSearchstring.Length, 12)
                        .CustomerNumber = customerNoFromUrl
                    Else
                        .CustomerNumber = Profile.User.Details.LoginID
                    End If
                End If
            End If
            .ProductType = "M"
            .Source = "W"
            If Not Session("Agent") Is Nothing AndAlso Not Session("Agent") = "" AndAlso Not Session("AgentType") Is Nothing AndAlso Session("AgentType") = "2" Then
                .ByPassPreReqCheck = "Y"
            Else
                .ByPassPreReqCheck = "N"
            End If
        End With
        basket.DeAddTicketingItems = deATI

        Dim err As New Talent.Common.ErrorObj
        err = basket.AddTicketingReservedItemsReturnBasket
        returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)
        If returnErrorCode.Length > 0 Then
            returnError = True
        End If

        If returnError Then
            Session("TicketingGatewayError") = returnErrorCode
            Session("TalentErrorCode") = returnErrorCode
        End If
        If String.IsNullOrEmpty(Request.QueryString("ReturnUrl")) Then
            _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
        Else
            _redirectUrl = Server.UrlDecode(Request.QueryString("ReturnUrl"))
        End If
    End Sub

    Private Sub ProductAway_AddToBasket()
        Dim product As String = Request("product")
        Dim includeTravel As String = Request("includetravel")
        Dim missingParam As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim returnError As Boolean = False
        Dim priceCode As String = String.Empty
        If Not Request("priceCode") Is Nothing Then priceCode = Request("priceCode")
        Dim productSubType As String = String.Empty
        If Not Request.QueryString("productsubtype") Is Nothing Then
            productSubType = Request.QueryString("productsubtype")
        End If

        Dim priceBandArray As String(,)
        If Session("PriceBandSelectionOptions") IsNot Nothing Then
            priceBandArray = Session("PriceBandSelectionOptions")
        Else
            priceBandArray = retrievePriceBandSelectionFromQueryString()
        End If

        If String.IsNullOrEmpty(product) Then
            missingParam = True
        ElseIf String.IsNullOrEmpty(includeTravel) Then
            missingParam = True
        ElseIf priceBandArray Is Nothing Then
            missingParam = True
        End If

        If AgentProfile.IsAgent AndAlso Not AgentProfile.AgentPermissions.CanAddAwayGameToBasket Then
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If

        If Not missingParam Then
            Dim deATI As New Talent.Common.DEAddTicketingItems
            Dim totalQuantity As Integer = 0

            For iOuter As Integer = priceBandArray.GetLowerBound(0) To priceBandArray.GetUpperBound(0)
                Dim firstItem As Boolean = True
                For iInner As Integer = priceBandArray.GetLowerBound(1) To priceBandArray.GetUpperBound(1)
                    If firstItem Then
                        firstItem = False
                    Else
                        If Utilities.CheckForDBNull_Int(priceBandArray(iOuter, iInner)) > 0 Then totalQuantity += CInt(priceBandArray(iOuter, iInner))
                    End If
                Next
                firstItem = True
            Next
            If String.IsNullOrEmpty(_linkedMasterProduct) Then
                Session("QuantityRequested") = totalQuantity
            End If

            With deATI
                .SessionId = Profile.Basket.Basket_Header_ID
                .ProductCode = product
                .PriceCode = priceCode
                .IncludeTravelProduct = includeTravel
                .ProductHasMandtoryRelatedProducts = _productHasMandatoryRelatedProducts
                If Not Profile.IsAnonymous Then
                    .CustomerNumber = Profile.User.Details.LoginID.PadLeft(12, "0")
                Else
                    .CustomerNumber = "000000000000"
                End If
                .Source = GlobalConstants.SOURCE
                If Not Session("Agent") Is Nothing AndAlso Not Session("Agent") = "" AndAlso Not Session("AgentType") Is Nothing AndAlso Session("AgentType") = "2" Then
                    .ByPassPreReqCheck = "Y"
                Else
                    .ByPassPreReqCheck = "N"
                End If
                Try
                    .PriceBand01 = CStr(priceBandArray(0, 0))
                    .Quantity01 = CStr(priceBandArray(0, 1))
                    .PriceBand02 = CStr(priceBandArray(1, 0))
                    .Quantity02 = CStr(priceBandArray(1, 1))
                    .PriceBand03 = CStr(priceBandArray(2, 0))
                    .Quantity03 = CStr(priceBandArray(2, 1))
                    .PriceBand04 = CStr(priceBandArray(3, 0))
                    .Quantity04 = CStr(priceBandArray(3, 1))
                    .PriceBand05 = CStr(priceBandArray(4, 0))
                    .Quantity05 = CStr(priceBandArray(4, 1))
                    .PriceBand06 = CStr(priceBandArray(5, 0))
                    .Quantity06 = CStr(priceBandArray(5, 1))
                    .PriceBand07 = CStr(priceBandArray(6, 0))
                    .Quantity07 = CStr(priceBandArray(6, 1))
                    .PriceBand08 = CStr(priceBandArray(7, 0))
                    .Quantity08 = CStr(priceBandArray(7, 1))
                    .PriceBand09 = CStr(priceBandArray(8, 0))
                    .Quantity09 = CStr(priceBandArray(8, 1))
                    .PriceBand10 = CStr(priceBandArray(9, 0))
                    .Quantity10 = CStr(priceBandArray(9, 1))
                    .PriceBand11 = CStr(priceBandArray(10, 0))
                    .Quantity11 = CStr(priceBandArray(10, 1))
                    .PriceBand12 = CStr(priceBandArray(11, 0))
                    .Quantity12 = CStr(priceBandArray(11, 1))
                    .PriceBand13 = CStr(priceBandArray(12, 0))
                    .Quantity13 = CStr(priceBandArray(12, 1))
                    .PriceBand14 = CStr(priceBandArray(13, 0))
                    .Quantity14 = CStr(priceBandArray(13, 1))
                    .PriceBand15 = CStr(priceBandArray(14, 0))
                    .Quantity15 = CStr(priceBandArray(14, 1))
                    .PriceBand16 = CStr(priceBandArray(15, 0))
                    .Quantity16 = CStr(priceBandArray(15, 1))
                    .PriceBand17 = CStr(priceBandArray(16, 0))
                    .Quantity17 = CStr(priceBandArray(16, 1))
                    .PriceBand18 = CStr(priceBandArray(17, 0))
                    .Quantity18 = CStr(priceBandArray(17, 1))
                    .PriceBand19 = CStr(priceBandArray(18, 0))
                    .Quantity19 = CStr(priceBandArray(18, 1))
                    .PriceBand20 = CStr(priceBandArray(19, 0))
                    .Quantity20 = CStr(priceBandArray(19, 1))
                    .PriceBand21 = CStr(priceBandArray(20, 0))
                    .Quantity21 = CStr(priceBandArray(20, 1))
                    .PriceBand22 = CStr(priceBandArray(21, 0))
                    .Quantity22 = CStr(priceBandArray(21, 1))
                    .PriceBand23 = CStr(priceBandArray(22, 0))
                    .Quantity23 = CStr(priceBandArray(22, 1))
                    .PriceBand24 = CStr(priceBandArray(23, 0))
                    .Quantity24 = CStr(priceBandArray(23, 1))
                    .PriceBand25 = CStr(priceBandArray(24, 0))
                    .Quantity25 = CStr(priceBandArray(24, 1))
                    .PriceBand26 = CStr(priceBandArray(25, 0))
                    .Quantity26 = CStr(priceBandArray(25, 1))
                Catch ex As Exception
                End Try
            End With

            Dim basket As New Talent.Common.TalentBasket
            basket.DeAddTicketingItems = deATI
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
            basket.Settings.PerformWatchListCheck = ModuleDefaults.PerformAgentWatchListCheck
            basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            err = basket.AddTicketingItemsReturnBasket
            returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)
            If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                _redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                _packageId = basket.DeAddTicketingItems.PackageID
            End If
            _TicketingGatewayFunctions.SetSessionValuesFromBasket(basket)

            If returnErrorCode.Length > 0 Then
                returnError = True
                If returnErrorCode.Equals("NC") Then
                    Dim tp As New Talent.Common.TalentProduct
                    tp.Settings.Company = basket.Settings.Company
                    tp.De.ProductType = "A"
                    tp.De.ProductSubtype = productSubType
                    If deATI.CustomerNumber <> "000000000000" Then tp.De.CustomerNumber = deATI.CustomerNumber
                    tp.De.StadiumCode = ModuleDefaults.TicketingStadium
                    tp.Settings.OriginatingSource = basket.Settings.OriginatingSource
                    tp.ProductListClearCache()
                    returnErrorCode = "NS"
                End If
            End If
        Else
            returnError = True
            If returnErrorCode.Trim = "" Then
                returnErrorCode = "PARAMERR"
                Talent.eCommerce.Logging.WriteLog(Profile.UserName, "PA_ATB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - ProductAway_AddToBasket", "Error")
            End If
        End If

        If returnError Then
            Session("TicketingGatewayError") = returnErrorCode
            Session("TalentErrorCode") = returnErrorCode
            _redirectUrl = "~/PagesPublic/ProductBrowse/ProductAway.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(True)
        Else
            _redirectToProductLinkingPage = True
            If ModuleDefaults.AwayProduct_ForwardToBasket Then
                _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
            Else
                _redirectUrl = "~/PagesPublic/ProductBrowse/ProductAway.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(True)
            End If
        End If
    End Sub

    Private Sub TravelProduct_AddToBasket()
        Dim product As String = Request("product")
        Dim type As String = Request("type")
        Dim productDetailCode As String = Request("productDetailCode")
        Dim missingParam As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim returnError As Boolean = False
        Dim productSubType As String = String.Empty
        If Not Request.QueryString("productsubtype") Is Nothing Then
            productSubType = Request.QueryString("productsubtype")
        End If

        Dim priceBandArray As String(,)
        If Session("PriceBandSelectionOptions") IsNot Nothing Then
            priceBandArray = Session("PriceBandSelectionOptions")
        Else
            priceBandArray = retrievePriceBandSelectionFromQueryString()
        End If

        If priceBandArray Is Nothing Then
            missingParam = True
        ElseIf String.IsNullOrEmpty(product) Then
            missingParam = True
        ElseIf String.IsNullOrEmpty(type) Then
            missingParam = True
        ElseIf String.IsNullOrEmpty(productDetailCode) Then
            missingParam = True
        End If

        If AgentProfile.IsAgent AndAlso Not AgentProfile.AgentPermissions.CanAddTravelProductToBasket Then
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If

        If Not missingParam Then
            Dim totalQuantity As Integer = 0
            Dim deATI As New Talent.Common.DEAddTicketingItems

            For iOuter As Integer = priceBandArray.GetLowerBound(0) To priceBandArray.GetUpperBound(0)
                Dim firstItem As Boolean = True
                For iInner As Integer = priceBandArray.GetLowerBound(1) To priceBandArray.GetUpperBound(1)
                    If firstItem Then
                        firstItem = False
                    Else
                        If Utilities.CheckForDBNull_Int(priceBandArray(iOuter, iInner)) > 0 Then totalQuantity += CInt(priceBandArray(iOuter, iInner))
                    End If
                Next
                firstItem = True
            Next

            With deATI
                If String.IsNullOrEmpty(_linkedMasterProduct) Then
                    Session("QuantityRequested") = totalQuantity
                End If
                If Not Session("Agent") Is Nothing AndAlso Not Session("Agent") = "" _
                    AndAlso Not Session("AgentType") Is Nothing AndAlso Session("AgentType") = "2" Then
                    .ByPassPreReqCheck = "Y"
                Else
                    .ByPassPreReqCheck = "N"
                End If
                .ProductCode = product
                .ProductType = type
                .LinkedMasterProduct = If(product.Equals(_linkedMasterProduct), String.Empty, _linkedMasterProduct)
                .ProductDetailCode = productDetailCode
                .Source = "W"
                .SessionId = Profile.Basket.Basket_Header_ID
                .ProductHasMandtoryRelatedProducts = _productHasMandatoryRelatedProducts
                If Not Profile.IsAnonymous Then
                    .CustomerNumber = Profile.User.Details.LoginID.PadLeft(12, "0")
                Else
                    .CustomerNumber = "000000000000"
                End If
                Try
                    .PriceBand01 = CStr(priceBandArray(0, 0))
                    .Quantity01 = CStr(priceBandArray(0, 1))
                    .PriceBand02 = CStr(priceBandArray(1, 0))
                    .Quantity02 = CStr(priceBandArray(1, 1))
                    .PriceBand03 = CStr(priceBandArray(2, 0))
                    .Quantity03 = CStr(priceBandArray(2, 1))
                    .PriceBand04 = CStr(priceBandArray(3, 0))
                    .Quantity04 = CStr(priceBandArray(3, 1))
                    .PriceBand05 = CStr(priceBandArray(4, 0))
                    .Quantity05 = CStr(priceBandArray(4, 1))
                    .PriceBand06 = CStr(priceBandArray(5, 0))
                    .Quantity06 = CStr(priceBandArray(5, 1))
                    .PriceBand07 = CStr(priceBandArray(6, 0))
                    .Quantity07 = CStr(priceBandArray(6, 1))
                    .PriceBand08 = CStr(priceBandArray(7, 0))
                    .Quantity08 = CStr(priceBandArray(7, 1))
                    .PriceBand09 = CStr(priceBandArray(8, 0))
                    .Quantity09 = CStr(priceBandArray(8, 1))
                    .PriceBand10 = CStr(priceBandArray(9, 0))
                    .Quantity10 = CStr(priceBandArray(9, 1))
                    .PriceBand11 = CStr(priceBandArray(10, 0))
                    .Quantity11 = CStr(priceBandArray(10, 1))
                    .PriceBand12 = CStr(priceBandArray(11, 0))
                    .Quantity12 = CStr(priceBandArray(11, 1))
                    .PriceBand13 = CStr(priceBandArray(12, 0))
                    .Quantity13 = CStr(priceBandArray(12, 1))
                    .PriceBand14 = CStr(priceBandArray(13, 0))
                    .Quantity14 = CStr(priceBandArray(13, 1))
                    .PriceBand15 = CStr(priceBandArray(14, 0))
                    .Quantity15 = CStr(priceBandArray(14, 1))
                    .PriceBand16 = CStr(priceBandArray(15, 0))
                    .Quantity16 = CStr(priceBandArray(15, 1))
                    .PriceBand17 = CStr(priceBandArray(16, 0))
                    .Quantity17 = CStr(priceBandArray(16, 1))
                    .PriceBand18 = CStr(priceBandArray(17, 0))
                    .Quantity18 = CStr(priceBandArray(17, 1))
                    .PriceBand19 = CStr(priceBandArray(18, 0))
                    .Quantity19 = CStr(priceBandArray(18, 1))
                    .PriceBand20 = CStr(priceBandArray(19, 0))
                    .Quantity20 = CStr(priceBandArray(19, 1))
                    .PriceBand21 = CStr(priceBandArray(20, 0))
                    .Quantity21 = CStr(priceBandArray(20, 1))
                    .PriceBand22 = CStr(priceBandArray(21, 0))
                    .Quantity22 = CStr(priceBandArray(21, 1))
                    .PriceBand23 = CStr(priceBandArray(22, 0))
                    .Quantity23 = CStr(priceBandArray(22, 1))
                    .PriceBand24 = CStr(priceBandArray(23, 0))
                    .Quantity24 = CStr(priceBandArray(23, 1))
                    .PriceBand25 = CStr(priceBandArray(24, 0))
                    .Quantity25 = CStr(priceBandArray(24, 1))
                    .PriceBand26 = CStr(priceBandArray(25, 0))
                    .Quantity26 = CStr(priceBandArray(25, 1))
                Catch ex As Exception
                End Try
            End With
            Dim basket As New Talent.Common.TalentBasket
            basket.DeAddTicketingItems = deATI
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
            basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            err = basket.AddTicketingItemsReturnBasket
            returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)

            If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                _redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                _packageId = basket.DeAddTicketingItems.PackageID
            End If
            _TicketingGatewayFunctions.SetSessionValuesFromBasket(basket)
            If returnErrorCode.Length > 0 Then
                returnError = True
                If returnErrorCode.Equals("NS") Then
                    Dim tp As New Talent.Common.TalentProduct
                    tp.Settings = basket.Settings
                    tp.De.ProductType = "T"
                    tp.De.productSubtype = productSubType
                    Dim stadiumList As String = String.Empty
                    If Not String.IsNullOrEmpty(Request.QueryString("stadiumList")) Then
                        stadiumList = Request.QueryString("stadiumList")
                    End If
                    If Not String.IsNullOrEmpty(stadiumList) Then
                        'only allow upto 5 stadium codes
                        Dim stadiumListArray() As String = stadiumList.Split(",")
                        If stadiumListArray.Length > 4 Then
                            Dim result As String = String.Empty
                            For x As Byte = 0 To 4
                                result &= stadiumListArray(x) + ","
                            Next
                            result = result.Substring(0, result.Length - 1)
                            tp.De.StadiumCode = result
                        Else
                            tp.De.StadiumCode = stadiumList
                        End If
                    Else
                        tp.De.StadiumCode = ModuleDefaults.TicketingStadium
                    End If
                    tp.ProductListClearCache()
                    returnErrorCode = "NS"
                End If
            End If
        Else
            returnError = True
            If returnErrorCode.Trim = "" Then
                returnErrorCode = "PARAMERR"
                Talent.eCommerce.Logging.WriteLog(Profile.UserName, "TP_ATB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - TravelProduct_AddToBasket", "Error")
            End If
        End If

        If returnError Then
            Session("TicketingGatewayError") = returnErrorCode
            Session("TalentErrorCode") = returnErrorCode
            _redirectUrl = "~/PagesPublic/ProductBrowse/ProductTravel.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(True)
        Else
            _redirectToProductLinkingPage = True
            If ModuleDefaults.TravelProduct_ForwardToBasket Then
                _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
            Else
                _redirectUrl = "~/PagesPublic/ProductBrowse/ProductTravel.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(True)
            End If
        End If
    End Sub

    Private Sub EventProduct_AddToBasket()
        Dim product As String = Request("product")
        Dim type As String = Request("type")
        Dim productStadium As String = Request("productStadium")
        Dim productSubType As String = Request("productSubType")
        Dim missingParam As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim returnError As Boolean = False
        Dim isCoursePdt As Boolean = False


        Dim priceBandArray As String(,)
        If Session("PriceBandSelectionOptions") IsNot Nothing Then
            priceBandArray = Session("PriceBandSelectionOptions")
        Else
            priceBandArray = retrievePriceBandSelectionFromQueryString()
        End If

        If priceBandArray Is Nothing Then
            missingParam = True
        ElseIf String.IsNullOrEmpty(product) Then
            missingParam = True
        ElseIf String.IsNullOrEmpty(type) Then
            missingParam = True
        End If

        If AgentProfile.IsAgent AndAlso Not AgentProfile.AgentPermissions.CanAddEventProductToBasket Then
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If

        If Not missingParam Then
            Dim totalQuantity As Integer = 0
            For iOuter As Integer = priceBandArray.GetLowerBound(0) To priceBandArray.GetUpperBound(0)
                Dim firstItem As Boolean = True
                For iInner As Integer = priceBandArray.GetLowerBound(1) To priceBandArray.GetUpperBound(1)
                    If firstItem Then
                        firstItem = False
                    Else
                        If Utilities.CheckForDBNull_Int(priceBandArray(iOuter, iInner)) > 0 Then totalQuantity += CInt(priceBandArray(iOuter, iInner))
                    End If
                Next
                firstItem = True
            Next

            'Check whether product is under course stadium
            isCoursePdt = IsCourseProduct(productStadium)
            If isCoursePdt Then
                'availability pre check if exists to registration page else product page
                Dim quantityRequest As Integer = CInt(priceBandArray(0, 1))
                If quantityRequest <= ModuleDefaults.CourseProductMaxQuantity Then
                    If quantityRequest <= GetTKTAvailableQuantity(product) Then
                        _redirectUrl = "~/PagesPublic/Profile/RegistrationParticipants.aspx?productCode=" & product & "&productType=" & type & "&productStadium=" & productStadium & "&productSubType=" & productSubType & "&quantity=" & quantityRequest & "&standCode=" & "" & "&areaCode=" & "" & "&campaignCode=" & ""
                    Else
                        returnError = True
                        returnErrorCode = "NS"
                    End If
                Else
                    returnError = True
                    returnErrorCode = "NS"
                End If
            Else
                Dim deATI As New Talent.Common.DEAddTicketingItems
                If String.IsNullOrEmpty(_linkedMasterProduct) Then
                    Session("QuantityRequested") = totalQuantity
                End If
                With deATI
                    If Not Session("Agent") Is Nothing AndAlso Not Session("Agent") = "" _
                        AndAlso Not Session("AgentType") Is Nothing AndAlso Session("AgentType") = "2" Then
                        .ByPassPreReqCheck = "Y"
                    Else
                        .ByPassPreReqCheck = "N"
                    End If
                    .ProductCode = product
                    .ProductType = type
                    .Source = "W"
                    .SessionId = Profile.Basket.Basket_Header_ID
                    .ProductHasMandtoryRelatedProducts = _productHasMandatoryRelatedProducts
                    If Not Profile.IsAnonymous Then
                        .CustomerNumber = Profile.User.Details.LoginID.PadLeft(12, "0")
                    Else
                        .CustomerNumber = "000000000000"
                    End If
                    Try
                        .PriceBand01 = CStr(priceBandArray(0, 0))
                        .Quantity01 = CStr(priceBandArray(0, 1))
                        .PriceBand02 = CStr(priceBandArray(1, 0))
                        .Quantity02 = CStr(priceBandArray(1, 1))
                        .PriceBand03 = CStr(priceBandArray(2, 0))
                        .Quantity03 = CStr(priceBandArray(2, 1))
                        .PriceBand04 = CStr(priceBandArray(3, 0))
                        .Quantity04 = CStr(priceBandArray(3, 1))
                        .PriceBand05 = CStr(priceBandArray(4, 0))
                        .Quantity05 = CStr(priceBandArray(4, 1))
                        .PriceBand06 = CStr(priceBandArray(5, 0))
                        .Quantity06 = CStr(priceBandArray(5, 1))
                        .PriceBand07 = CStr(priceBandArray(6, 0))
                        .Quantity07 = CStr(priceBandArray(6, 1))
                        .PriceBand08 = CStr(priceBandArray(7, 0))
                        .Quantity08 = CStr(priceBandArray(7, 1))
                        .PriceBand09 = CStr(priceBandArray(8, 0))
                        .Quantity09 = CStr(priceBandArray(8, 1))
                        .PriceBand10 = CStr(priceBandArray(9, 0))
                        .Quantity10 = CStr(priceBandArray(9, 1))
                        .PriceBand11 = CStr(priceBandArray(10, 0))
                        .Quantity11 = CStr(priceBandArray(10, 1))
                        .PriceBand12 = CStr(priceBandArray(11, 0))
                        .Quantity12 = CStr(priceBandArray(11, 1))
                        .PriceBand13 = CStr(priceBandArray(12, 0))
                        .Quantity13 = CStr(priceBandArray(12, 1))
                        .PriceBand14 = CStr(priceBandArray(13, 0))
                        .Quantity14 = CStr(priceBandArray(13, 1))
                        .PriceBand15 = CStr(priceBandArray(14, 0))
                        .Quantity15 = CStr(priceBandArray(14, 1))
                        .PriceBand16 = CStr(priceBandArray(15, 0))
                        .Quantity16 = CStr(priceBandArray(15, 1))
                        .PriceBand17 = CStr(priceBandArray(16, 0))
                        .Quantity17 = CStr(priceBandArray(16, 1))
                        .PriceBand18 = CStr(priceBandArray(17, 0))
                        .Quantity18 = CStr(priceBandArray(17, 1))
                        .PriceBand19 = CStr(priceBandArray(18, 0))
                        .Quantity19 = CStr(priceBandArray(18, 1))
                        .PriceBand20 = CStr(priceBandArray(19, 0))
                        .Quantity20 = CStr(priceBandArray(19, 1))
                        .PriceBand21 = CStr(priceBandArray(20, 0))
                        .Quantity21 = CStr(priceBandArray(20, 1))
                        .PriceBand22 = CStr(priceBandArray(21, 0))
                        .Quantity22 = CStr(priceBandArray(21, 1))
                        .PriceBand23 = CStr(priceBandArray(22, 0))
                        .Quantity23 = CStr(priceBandArray(22, 1))
                        .PriceBand24 = CStr(priceBandArray(23, 0))
                        .Quantity24 = CStr(priceBandArray(23, 1))
                        .PriceBand25 = CStr(priceBandArray(24, 0))
                        .Quantity25 = CStr(priceBandArray(24, 1))
                        .PriceBand26 = CStr(priceBandArray(25, 0))
                        .Quantity26 = CStr(priceBandArray(25, 1))
                    Catch ex As Exception
                    End Try
                End With
                Dim basket As New Talent.Common.TalentBasket
                basket.DeAddTicketingItems = deATI
                basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
                basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
                basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
                basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
                basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
                Dim err As New Talent.Common.ErrorObj
                err = basket.AddTicketingItemsReturnBasket
                returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)

                If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                    _redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                    _packageId = basket.DeAddTicketingItems.PackageID
                End If
                _TicketingGatewayFunctions.SetSessionValuesFromBasket(basket)
                If returnErrorCode.Length > 0 Then
                    returnError = True
                    If returnErrorCode.Equals("NS") Then
                        Dim tp As New Talent.Common.TalentProduct
                        tp.Settings = basket.Settings
                        tp.De.ProductType = "E"
                        tp.De.productSubtype = productSubType
                        Dim stadiumList As String = String.Empty
                        If Not String.IsNullOrEmpty(Request.QueryString("stadiumList")) Then
                            stadiumList = Request.QueryString("stadiumList")
                        End If
                        If Not String.IsNullOrEmpty(stadiumList) Then
                            'only allow upto 5 stadium codes
                            Dim stadiumListArray() As String = stadiumList.Split(",")
                            If stadiumListArray.Length > 4 Then
                                Dim result As String = String.Empty
                                For x As Byte = 0 To 4
                                    result &= stadiumListArray(x) + ","
                                Next
                                result = result.Substring(0, result.Length - 1)
                                tp.De.StadiumCode = result
                            Else
                                tp.De.StadiumCode = stadiumList
                            End If
                        Else
                            tp.De.StadiumCode = ModuleDefaults.TicketingStadium
                        End If
                        tp.ProductListClearCache()
                        returnErrorCode = "NS"
                    End If
                End If
            End If
        Else
            returnError = True
            If returnErrorCode.Trim = "" Then
                returnErrorCode = "PARAMERR"
                Talent.eCommerce.Logging.WriteLog(Profile.UserName, "TP_ATB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - TravelProduct_AddToBasket", "Error")
            End If
        End If

        If Not isCoursePdt Then
            If returnError Then
                Session("TicketingGatewayError") = returnErrorCode
                Session("TalentErrorCode") = returnErrorCode
                _redirectUrl = "~/PagesPublic/ProductBrowse/ProductEvent.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(True)
            Else
                _redirectToProductLinkingPage = True
                If ModuleDefaults.EventProduct_ForwardToBasket Then
                    _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                Else
                    _redirectUrl = "~/PagesPublic/ProductBrowse/ProductEvent.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(True)
                End If
            End If
        End If
    End Sub

    Private Sub MembershipProduct_AddToBasket()
        Dim product As String = Request("product")
        Dim priceCode As String = Request("priceCode")
        Dim smartcard As String = String.Empty
        Dim smartcardAmount As String = String.Empty
        Dim missingParam As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim returnError As Boolean = False
        Dim productSubType As String = String.Empty
        If Not Request.QueryString("productsubtype") Is Nothing Then
            productSubType = Request.QueryString("productsubtype")
        End If

        If String.IsNullOrEmpty(product) Then
            missingParam = True
        ElseIf String.IsNullOrEmpty(priceCode) Then
            If ModuleDefaults.EPurseTopUpProductCode <> product Then
                missingParam = True
            End If
        End If

        'If the product is an EPurse Top Up Product, try to retrieve the balance and card number
        If Not String.IsNullOrEmpty(ModuleDefaults.EPurseTopUpProductCode) AndAlso ModuleDefaults.EPurseTopUpProductCode = product Then
            If Session("EPurseTopUp-CardNumber") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Session("EPurseTopUp-CardNumber")) Then
                smartcard = Session("EPurseTopUp-CardNumber")
                Session("EPurseTopUp-CardNumber") = Nothing
            End If
            If Session("EPurseTopUp-Amount") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Session("EPurseTopUp-Amount")) Then
                smartcardAmount = Session("EPurseTopUp-Amount")
                Session("EPurseTopUp-Amount") = Nothing
            End If
        End If

        If AgentProfile.IsAgent AndAlso Not AgentProfile.AgentPermissions.CanAddMembershipsProductToBasket Then
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If

        If Not missingParam Then
            Dim deATI As New Talent.Common.DEAddTicketingItems
            Dim priceBandArray(25, 1) As String
            Dim totalQuantity As Integer = 0

            If Session("PriceBandSelectionOptions") IsNot Nothing Then
                priceBandArray = Session("PriceBandSelectionOptions")
            Else
                priceBandArray = retrievePriceBandSelectionFromQueryString()
            End If

            If priceBandArray Is Nothing Then
                Dim priceBandArrayIndex As Integer = 0
                While priceBandArrayIndex < 26
                    priceBandArray(priceBandArrayIndex, 0) = String.Empty
                    priceBandArray(priceBandArrayIndex, 1) = String.Empty
                    priceBandArrayIndex += 1
                End While
            End If

            For iOuter As Integer = priceBandArray.GetLowerBound(0) To priceBandArray.GetUpperBound(0)
                Dim firstItem As Boolean = True
                For iInner As Integer = priceBandArray.GetLowerBound(1) To priceBandArray.GetUpperBound(1)
                    If firstItem Then
                        firstItem = False
                    Else
                        If Utilities.CheckForDBNull_Int(priceBandArray(iOuter, iInner)) > 0 Then totalQuantity += CInt(priceBandArray(iOuter, iInner))
                    End If
                Next
                firstItem = True
            Next
            If String.IsNullOrEmpty(_linkedMasterProduct) Then
                Session("QuantityRequested") = totalQuantity
            End If

            With deATI
                .SessionId = Profile.Basket.Basket_Header_ID
                .ProductCode = product
                .PriceCode = priceCode
                .ProductHasMandtoryRelatedProducts = _productHasMandatoryRelatedProducts
                .SmartcardNumber = smartcard
                .SmartcardAmount = smartcardAmount
                If Profile.IsAnonymous Then
                    .CustomerNumber = "000000000000"
                Else
                    .CustomerNumber = Profile.User.Details.LoginID
                End If
                .Source = GlobalConstants.SOURCE
                If Not Session("Agent") Is Nothing AndAlso Not Session("Agent") = "" AndAlso Not Session("AgentType") Is Nothing AndAlso Session("AgentType") = "2" Then
                    .ByPassPreReqCheck = "Y"
                Else
                    .ByPassPreReqCheck = "N"
                End If
                Try
                    .PriceBand01 = CStr(priceBandArray(0, 0))
                    .Quantity01 = CStr(priceBandArray(0, 1))
                    .PriceBand02 = CStr(priceBandArray(1, 0))
                    .Quantity02 = CStr(priceBandArray(1, 1))
                    .PriceBand03 = CStr(priceBandArray(2, 0))
                    .Quantity03 = CStr(priceBandArray(2, 1))
                    .PriceBand04 = CStr(priceBandArray(3, 0))
                    .Quantity04 = CStr(priceBandArray(3, 1))
                    .PriceBand05 = CStr(priceBandArray(4, 0))
                    .Quantity05 = CStr(priceBandArray(4, 1))
                    .PriceBand06 = CStr(priceBandArray(5, 0))
                    .Quantity06 = CStr(priceBandArray(5, 1))
                    .PriceBand07 = CStr(priceBandArray(6, 0))
                    .Quantity07 = CStr(priceBandArray(6, 1))
                    .PriceBand08 = CStr(priceBandArray(7, 0))
                    .Quantity08 = CStr(priceBandArray(7, 1))
                    .PriceBand09 = CStr(priceBandArray(8, 0))
                    .Quantity09 = CStr(priceBandArray(8, 1))
                    .PriceBand10 = CStr(priceBandArray(9, 0))
                    .Quantity10 = CStr(priceBandArray(9, 1))
                    .PriceBand11 = CStr(priceBandArray(10, 0))
                    .Quantity11 = CStr(priceBandArray(10, 1))
                    .PriceBand12 = CStr(priceBandArray(11, 0))
                    .Quantity12 = CStr(priceBandArray(11, 1))
                    .PriceBand13 = CStr(priceBandArray(12, 0))
                    .Quantity13 = CStr(priceBandArray(12, 1))
                    .PriceBand14 = CStr(priceBandArray(13, 0))
                    .Quantity14 = CStr(priceBandArray(13, 1))
                    .PriceBand15 = CStr(priceBandArray(14, 0))
                    .Quantity15 = CStr(priceBandArray(14, 1))
                    .PriceBand16 = CStr(priceBandArray(15, 0))
                    .Quantity16 = CStr(priceBandArray(15, 1))
                    .PriceBand17 = CStr(priceBandArray(16, 0))
                    .Quantity17 = CStr(priceBandArray(16, 1))
                    .PriceBand18 = CStr(priceBandArray(17, 0))
                    .Quantity18 = CStr(priceBandArray(17, 1))
                    .PriceBand19 = CStr(priceBandArray(18, 0))
                    .Quantity19 = CStr(priceBandArray(18, 1))
                    .PriceBand20 = CStr(priceBandArray(19, 0))
                    .Quantity20 = CStr(priceBandArray(19, 1))
                    .PriceBand21 = CStr(priceBandArray(20, 0))
                    .Quantity21 = CStr(priceBandArray(20, 1))
                    .PriceBand22 = CStr(priceBandArray(21, 0))
                    .Quantity22 = CStr(priceBandArray(21, 1))
                    .PriceBand23 = CStr(priceBandArray(22, 0))
                    .Quantity23 = CStr(priceBandArray(22, 1))
                    .PriceBand24 = CStr(priceBandArray(23, 0))
                    .Quantity24 = CStr(priceBandArray(23, 1))
                    .PriceBand25 = CStr(priceBandArray(24, 0))
                    .Quantity25 = CStr(priceBandArray(24, 1))
                    .PriceBand26 = CStr(priceBandArray(25, 0))
                    .Quantity26 = CStr(priceBandArray(25, 1))
                Catch ex As Exception
                End Try
            End With

            Dim basket As New Talent.Common.TalentBasket
            basket.DeAddTicketingItems = deATI
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
            basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            err = basket.AddTicketingItemsReturnBasket
            returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)

            If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                _redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                _packageId = basket.DeAddTicketingItems.PackageID
            End If
            _TicketingGatewayFunctions.SetSessionValuesFromBasket(basket)
            If returnErrorCode.Length > 0 Then
                If returnErrorCode.Equals("NC") Then
                    Dim tp As New Talent.Common.TalentProduct
                    tp.Settings.Company = basket.Settings.Company
                    tp.De.ProductType = "C"
                    tp.De.productSubtype = productSubType
                    If deATI.CustomerNumber <> "000000000000" Then tp.De.CustomerNumber = deATI.CustomerNumber
                    tp.De.StadiumCode = ModuleDefaults.TicketingStadium
                    tp.Settings.OriginatingSource = basket.Settings.OriginatingSource
                    tp.ProductListClearCache()
                    returnErrorCode = "NS"
                End If
                returnError = True
            End If
        Else
            returnError = True
            If returnErrorCode.Trim = "" Then
                returnErrorCode = "PARAMERR"
                Talent.eCommerce.Logging.WriteLog(Profile.UserName, "MP_ATB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - MembershipProduct_AddToBasket", "Error")
            End If
        End If

        If returnError Then
            Session("TicketingGatewayError") = returnErrorCode
            Session("TalentErrorCode") = returnErrorCode
            If String.IsNullOrWhiteSpace(ModuleDefaults.EPurseTopUpProductCode) OrElse product <> ModuleDefaults.EPurseTopUpProductCode Then
                _redirectUrl = "~/PagesPublic/ProductBrowse/ProductMembership.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(True)
            Else
                _redirectUrl = "~/PagesLogin/Smartcard/EPurse.aspx"
            End If
        Else
            _redirectToProductLinkingPage = True
            If ModuleDefaults.MembershipProduct_ForwardToBasket Then
                _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
            Else
                _redirectUrl = "~/PagesPublic/ProductBrowse/ProductMembership.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(True)
            End If
        End If
    End Sub
    ''' <summary>
    ''' adds variable priced product to the basket (these are setup backend as memberships)    
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub MembershipProduct_AddVariablePricedToBasket()
        Dim product As String = Request("product")
        Dim price As String = Request("price")
        Dim returnErrorCode As String = String.Empty
        Dim returnError As Boolean = False
        Dim productSubType As String = String.Empty

        Dim deATI As New Talent.Common.DEAddTicketingItems
        With deATI
            .VariablePricedProductPrice = price * 100
            .ProductCode = product
            .Source = GlobalConstants.SOURCE
            .SessionId = Profile.Basket.Basket_Header_ID
            .Quantity01 = 1
            If Profile.IsAnonymous Then
                .CustomerNumber = GlobalConstants.GENERIC_CUSTOMER_NUMBER
            Else
                .CustomerNumber = Profile.User.Details.LoginID
            End If
        End With

        Dim basket As New Talent.Common.TalentBasket
        basket.DeAddTicketingItems = deATI
        basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
        basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
        basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
        Dim err As New Talent.Common.ErrorObj
        err = basket.AddTicketingItemsReturnBasket
        returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)
        If returnError Then
            _redirectUrl = "~/PagesPublic/ProductBrowse/ProductMembership.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(True)
        Else
            _redirectToProductLinkingPage = False
            _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
        End If

    End Sub


    Private Sub PPS_AddToBasket()
        Dim tpp As PagesPublic_ProductBrowse_TicketingPrePayments = CType(Context.Handler, PagesPublic_ProductBrowse_TicketingPrePayments)
        Dim pr As Repeater = Talent.eCommerce.Utilities.FindWebControl("ProductRepeater", Talent.eCommerce.Utilities.FindWebControl("ProductDetail1", tpp.Controls).Controls)

        'Scheme Name, Tickets List
        Dim schemes As New Generic.Dictionary(Of String, Generic.List(Of String))
        Dim noSelection As Boolean = False
        Dim returnErrorCode As String = ""
        Dim returnError As Boolean = False
        Dim priceCode As String = String.Empty

        'Determine the season ticket product
        Dim seasonTkt As String = String.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("product")) Then
            seasonTkt = Request.QueryString("product")
        End If

        If Request.QueryString("pricecode") IsNot Nothing Then priceCode = Request.QueryString("pricecode")

        If Not tpp Is Nothing AndAlso Not pr Is Nothing Then

            'Loop through all schemes
            For Each ri As RepeaterItem In pr.Items
                Dim pps As UserControls_TicketingPPS = CType(ri.FindControl("TicketingPPS1"), UserControls_TicketingPPS)
                Dim tickets As New Generic.List(Of String)
                If pps.SeasonTicketsList.Items.Count > 0 Then
                    Dim selectCount As Integer = 0, loopCount As Integer = 0
                    'otherwise check for selected season tickets in the current scheme
                    For Each item As ListItem In pps.SeasonTicketsList.Items
                        loopCount += 1
                        If item.Selected Then
                            'if selected add to the tickets list
                            tickets.Add(item.Value)
                            selectCount += 1
                        End If
                    Next
                    If tickets.Count > 0 Then
                        'If tickets have been selected add to the schemes list
                        If schemes.ContainsKey(pps.HiddenProductCode.Value) Then
                            schemes(pps.HiddenProductCode.Value).AddRange(tickets)
                        Else
                            schemes.Add(pps.HiddenProductCode.Value, tickets)
                        End If
                    End If
                End If
            Next

            Dim deAPPS As New Talent.Common.DEAddPPS
            deAPPS.Source = "W"
            deAPPS.SessionId = Profile.Basket.Basket_Header_ID
            deAPPS.PPSStage = Request.QueryString("ppspage")
            deAPPS.RegisteredPost = tpp.SendRegisteredPost
            deAPPS.SeasonTicket = seasonTkt
            If Profile.IsAnonymous Then
                deAPPS.CustomerNumber = "000000000000"
            Else
                deAPPS.CustomerNumber = Profile.User.Details.LoginID
            End If
            If Not Session("Agent") Is Nothing AndAlso Not Session("Agent") = "" AndAlso Not Session("AgentType") Is Nothing AndAlso Session("AgentType") = "2" Then
                deAPPS.ByPassPreReqCheck = "Y"
            Else
                deAPPS.ByPassPreReqCheck = "N"
            End If

            Dim count As Integer = 0
            For Each scheme As String In schemes.Keys
                For Each ticket As String In schemes(scheme)
                    Dim prd As String = scheme
                    Dim st As String = ticket
                    Dim ppsItem As DePPSItem
                    ppsItem = New DePPSItem
                    ppsItem.ProductCode = prd
                    ppsItem.Seat = st
                    For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                        If UCase(tbi.SEAT) = UCase(ticket) AndAlso UCase(tbi.PRODUCT_TYPE) = "S" Then
                            ppsItem.CustomerNumber = tbi.LOGINID
                            Exit For
                        End If
                    Next
                    deAPPS.PPSItems.Add(ppsItem)
                Next
            Next

            Dim basket As New Talent.Common.TalentBasket
            basket.De_AddPPS = deAPPS
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
            basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            err = basket.AddPPSToBasket
            returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)

            If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                _redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                _packageId = basket.DeAddTicketingItems.PackageID
            End If
            If returnErrorCode.Length > 0 Then returnError = True

            If Not returnError Then
                'Return back to the PPS page when validation errors have occurred
                If basket.ResultDataSet.Tables(0).Rows(0).Item("ValidationError") = "Y" Then
                    Select Case Request.QueryString("ppspage")
                        Case Is = "1"
                            _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=1&product=" & seasonTkt & "&pricecode=" & priceCode
                        Case Is = "2"
                            _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=2&product=" & seasonTkt & "&pricecode=" & priceCode
                        Case Else
                            _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                            _redirectToProductLinkingPage = True
                    End Select
                Else
                    If ModuleDefaults.PPS_ENABLE_2 AndAlso Request.QueryString("ppspage") <> "2" Then
                        _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=2&product=" & seasonTkt & "&pricecode=" & priceCode
                    Else
                        _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                        _redirectToProductLinkingPage = True
                    End If
                End If
            End If
        Else
            returnError = True
            If returnErrorCode.Trim = "" Then
                returnErrorCode = "PARAMERR"
                Talent.eCommerce.Logging.WriteLog(Profile.UserName, "PPS_ATB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - PPS_AddToBasket", "Error")
            End If
        End If

        If returnError Then
            Session("TicketingGatewayError") = returnErrorCode
            Session("TalentErrorCode") = returnErrorCode
            _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=1&pricecode=" & priceCode
        Else
            If String.IsNullOrEmpty(_redirectUrl) Then
                _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                _redirectToProductLinkingPage = True
            End If
        End If
    End Sub

    Private Sub VisualSeatSelection_AddToBasket()
        Dim product As String = Request.QueryString("product")
        Dim stand As String = Request.QueryString("stand")
        Dim area As String = Request.QueryString("area")
        Dim quantity As String = Request.QueryString("quantity")
        Dim priceBand As String = Request.QueryString("priceBand")
        Dim campaignCode As String = Request.QueryString("campaign")
        Dim productType As String = Request.QueryString("type")
        Dim productSubType As String = Request.QueryString("productsubtype")
        Dim productStadium As String = Request.QueryString("productstadium")
        Dim favouriteSeat As String = Request.QueryString("favouriteSeat")
        Dim defaultPrice As String = Request.QueryString("defaultprice")
        If String.IsNullOrWhiteSpace(defaultPrice) Then defaultPrice = "0"
        Dim selectedMinPrice As String = Request.QueryString("minPrice")
        Dim selectedMaxPrice As String = Request.QueryString("maxPrice")
        Dim priceBreakId As String = Request.QueryString("priceBreakId")
        Dim isProductBundle As Boolean = False
        Dim missingParam As Boolean = False
        Dim favouriteSeatSelected As Boolean = False
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim customerNumber As String = ""
        Dim isCoursePdt As Boolean = False
        Dim isItCATSeatSelection As Boolean = False
        _TicketingGatewayFunctions = New TicketingGatewayFunctions
        Dim priceBands(25, 1) As String
        Dim buyingUsingMultiPriceBands As Boolean = False
        If Session("SelectedPriceBands") IsNot Nothing Then
            priceBands = Session("SelectedPriceBands")
            Session("SelectedPriceBands") = Nothing
            buyingUsingMultiPriceBands = True
        Else
            priceBands = retrievePriceBandSelectionFromQueryString()
            If Not priceBands Is Nothing Then
                buyingUsingMultiPriceBands = True
            End If
        End If

        'if seat selection is for CAT then check required param are exists
        missingParam = IsCATParamMissing()
        If String.IsNullOrEmpty(product) Then
            missingParam = True
            Me._missingParamErrorCode = "PR"
        ElseIf String.IsNullOrEmpty(stand) Then
            missingParam = True
            Me._missingParamErrorCode = "ST"
        ElseIf String.IsNullOrEmpty(area) Then
            missingParam = True
            Me._missingParamErrorCode = "AR"
        ElseIf String.IsNullOrEmpty(quantity) AndAlso priceBands.Length = 0 Then
            returnErrorCode = "AAQ"
            missingParam = True
            Me._missingParamErrorCode = "QT"
        ElseIf String.IsNullOrEmpty(priceBand) AndAlso priceBands.Length = 0 Then
            missingParam = True
            Me._missingParamErrorCode = "PB"
        End If
        If favouriteSeat IsNot Nothing Then
            Try
                If CBool(favouriteSeat) Then
                    favouriteSeatSelected = True
                End If
            Catch ex As Exception
                favouriteSeatSelected = False
            End Try
        End If
        If campaignCode Is Nothing Then campaignCode = String.Empty
        If Request.QueryString("isproductbundle") IsNot Nothing Then isProductBundle = Utilities.CheckForDBNull_Boolean_DefaultFalse(Request.QueryString("isproductbundle"))

        If Not Profile.IsAnonymous Then
            customerNumber = Profile.User.Details.LoginID.PadLeft(12, "0")
        Else
            If productType = GlobalConstants.SEASONTICKETPRODUCTTYPE AndAlso Not isProductBundle Then _redirectUrl = "~/PagesPublic/Login/login.aspx?ReturnUrl=" & Server.UrlEncode("~/PagesPublic/ProductBrowse/ProductSeason.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(True))
        End If

        'Check whether product is under course stadium
        isCoursePdt = IsCourseProduct(productStadium)
        If isCoursePdt Then
            If String.IsNullOrEmpty(productSubType) Then
                productSubType = ""
            ElseIf String.IsNullOrEmpty(productStadium) Then
                productStadium = ""
            End If
        End If

        If Not missingParam Then
            If isCoursePdt Then
                'availability pre check if exists to registration page else product page
                Dim quantityRequest As Integer = CInt(quantity)
                If quantityRequest <= ModuleDefaults.CourseProductMaxQuantity Then
                    If quantityRequest <= GetTKTAvailableQuantity(productType, product, campaignCode, stand, area) Then
                        _redirectUrl = "~/PagesPublic/Profile/RegistrationParticipants.aspx?productCode=" & product & "&productType=" & productType & "&productStadium=" & productStadium & "&productSubType=" & productSubType & "&quantity=" & quantityRequest & "&standCode=" & stand & "&areaCode=" & area & "&campaignCode=" & campaignCode
                    Else
                        returnError = True
                        returnErrorCode = "NS"
                    End If
                Else
                    returnError = True
                    returnErrorCode = "NS"
                End If
            Else
                If String.IsNullOrEmpty(_linkedMasterProduct) Then
                    Session("QuantityRequested") = quantity
                End If
                Dim deATI As New Talent.Common.DEAddTicketingItems
                Dim ticketGatewayFunction As New TicketingGatewayFunctions
                'if seat selection is for CAT populate the CAT ticketing item details
                deATI = GetCATTicketingItemDetails(False)
                With deATI
                    .SessionId = Profile.Basket.Basket_Header_ID
                    .CustomerNumber = customerNumber
                    .AreaCode = area.ToUpper()
                    .StandCode = stand.ToUpper()
                    .ProductCode = product
                    .LinkedMasterProduct = If(product.Equals(_linkedMasterProduct), String.Empty, _linkedMasterProduct)
                    If buyingUsingMultiPriceBands Then
                        .PriceBand01 = priceBands(0, 0)
                        .Quantity01 = priceBands(0, 1)
                        .PriceBand02 = priceBands(1, 0)
                        .Quantity02 = priceBands(1, 1)
                        .PriceBand03 = priceBands(2, 0)
                        .Quantity03 = priceBands(2, 1)
                        .PriceBand04 = priceBands(3, 0)
                        .Quantity04 = priceBands(3, 1)
                        .PriceBand05 = priceBands(4, 0)
                        .Quantity05 = priceBands(4, 1)
                        .PriceBand06 = priceBands(5, 0)
                        .Quantity06 = priceBands(5, 1)
                        .PriceBand07 = priceBands(6, 0)
                        .Quantity07 = priceBands(6, 1)
                        .PriceBand08 = priceBands(7, 0)
                        .Quantity08 = priceBands(7, 1)
                        .PriceBand09 = priceBands(8, 0)
                        .Quantity09 = priceBands(8, 1)
                        .PriceBand10 = priceBands(9, 0)
                        .Quantity10 = priceBands(9, 1)
                        .PriceBand11 = priceBands(10, 0)
                        .Quantity11 = priceBands(10, 1)
                        .PriceBand12 = priceBands(11, 0)
                        .Quantity12 = priceBands(11, 1)
                        .PriceBand13 = priceBands(12, 0)
                        .Quantity13 = priceBands(12, 1)
                        .PriceBand14 = priceBands(13, 0)
                        .Quantity14 = priceBands(13, 1)
                        .PriceBand15 = priceBands(14, 0)
                        .Quantity15 = priceBands(14, 1)
                        .PriceBand16 = priceBands(15, 0)
                        .Quantity16 = priceBands(15, 1)
                        .PriceBand17 = priceBands(16, 0)
                        .Quantity17 = priceBands(16, 1)
                        .PriceBand18 = priceBands(17, 0)
                        .Quantity18 = priceBands(17, 1)
                        .PriceBand19 = priceBands(18, 0)
                        .Quantity19 = priceBands(18, 1)
                        .PriceBand20 = priceBands(19, 0)
                        .Quantity21 = priceBands(19, 1)
                        .PriceBand21 = priceBands(20, 0)
                        .Quantity21 = priceBands(20, 1)
                        .PriceBand22 = priceBands(21, 0)
                        .Quantity22 = priceBands(21, 1)
                        .PriceBand23 = priceBands(22, 0)
                        .Quantity23 = priceBands(22, 1)
                        .PriceBand24 = priceBands(23, 0)
                        .Quantity24 = priceBands(23, 1)
                        .PriceBand25 = priceBands(24, 0)
                        .Quantity25 = priceBands(24, 1)
                    Else
                        .Quantity01 = quantity
                        .PriceBand01 = priceBand
                    End If
                    .CampaignCode = campaignCode
                    .ProductHasMandtoryRelatedProducts = _productHasMandatoryRelatedProducts
                    .AllMandatoryLinkedProductsAdded = _TicketingGatewayFunctions.HaveAllLinkedMandatoryProductsBeenAdded(product, _linkedMasterProduct)
                    .FavouriteSeatSelected = favouriteSeatSelected
                    .DefaultPrice = defaultPrice
                    .SelectedPriceMinimum = TEBUtilities.CheckForDBNull_Decimal(selectedMinPrice)
                    .SelectedPriceMaximum = TEBUtilities.CheckForDBNull_Decimal(selectedMaxPrice)
                    .SelectedPriceBreakId = TEBUtilities.CheckForDBNull_Long(priceBreakId)
                    .LinkedProductID = ticketGatewayFunction.ReturnLinkedProductIDFromBasket(_linkedMasterProduct)
                    .SeatSelectionArray = GetFormattedSeatList(HttpContext.Current.Session("CATSeatsInSeatSelection"), String.Empty, False)
                    .Source = GlobalConstants.SOURCE
                End With

                Dim basket As New Talent.Common.TalentBasket
                basket.DeAddTicketingItems = deATI
                basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
                basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
                basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
                basket.Settings.PerformWatchListCheck = ModuleDefaults.PerformAgentWatchListCheck
                basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
                basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
                Dim err As New Talent.Common.ErrorObj
                Talent.Common.Utilities.TalentCommonLog("TicketingGateway.VisualSeatSelection_AddToBasket", deATI.CustomerNumber, "TalentEBusiness Request to Talent.Common.AmendBasket.AddTicketingItemsReturnBasket")
                err = basket.AddTicketingItemsReturnBasket
                Talent.Common.Utilities.TalentCommonLog("TicketingGateway.VisualSeatSelection_AddToBasket", deATI.CustomerNumber, "TalentEBusiness Response from Talent.Common.AmendBasket.AddTicketingItemsReturnBasket")
                returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)

                If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                    _redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                    _packageId = basket.DeAddTicketingItems.PackageID
                End If
                _TicketingGatewayFunctions.SetSessionValuesFromBasket(basket)

                'user is not going back to seat selection for CAT due to orphanseat error so clear cat sessions
                'Clear all transfer session variables
                If Not String.IsNullOrWhiteSpace(HttpContext.Current.Session("catmode")) Then
                    Session("catpayref") = Session("payref")
                End If
                isItCATSeatSelection = IsCATSessionsRemoved()

                If returnErrorCode.Length > 0 Then
                    returnError = True
                    If returnErrorCode.Equals("NC") Then
                        clearStandAreaCache(basket)
                        returnErrorCode = "NS"
                    End If
                End If
                If basket.ClearAvailableStandAreaCache Then clearStandAreaCache(basket)
                If basket.AlternativeSeatSelected Then Session("AssignedAlternativeSeat") = True
                If basket.BasketHasExceptionSeats AndAlso productType = GlobalConstants.SEASONTICKETPRODUCTTYPE Then _redirectToSeasonTicketExceptionsPage = True
            End If

        Else 'missing param else
            If returnErrorCode.Trim = "" Then
                returnErrorCode = "PARAMERR"
                Talent.eCommerce.Logging.WriteLog(Profile.UserName, "PS_ATB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - VisualSeatSelection_AddToBasket", "Error")
            End If
            returnError = True
        End If

        If returnError Then
            Talent.Common.Utilities.TalentCommonLog("TicketingGateway.VisualSeatSelection_AddToBasket", "", "TalentEBusiness Redirect to ~/PagesPublic/ProductBrowse/VisualSeatSelection.aspx")
            Session("TicketingGatewayError") = returnErrorCode
            Session("TalentErrorCode") = returnErrorCode
            Dim visualSeatSelctionUrl As String = "~/PagesPublic/ProductBrowse/VisualSeatSelection.aspx?stadium={0}&product={1}&campaign={2}&type={3}&productsubtype={4}"
            _redirectUrl = String.Format(visualSeatSelctionUrl, productStadium, product, campaignCode, productType, productSubType)
        Else
            If productType = GlobalConstants.SEASONTICKETPRODUCTTYPE AndAlso Not isProductBundle Then
                If (ModuleDefaults.PPS_ENABLE_1 AndAlso Not AgentProfile.BulkSalesMode) Then
                    Talent.Common.Utilities.TalentCommonLog("TicketingGateway.VisualSeatSelection_AddToBasket", customerNumber, "TalentEBusiness Redirect to ~/PagesPublic/ProductBrowse/TicketingPrePayment.aspx?ppspage=1")
                    _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=1&product=" & Request.QueryString("product") & "&pricecode=" & campaignCode & "&productsubtype=" & productSubType
                ElseIf (ModuleDefaults.PPS_ENABLE_2 AndAlso Not AgentProfile.BulkSalesMode) Then
                    Talent.Common.Utilities.TalentCommonLog("TicketingGateway.VisualSeatSelection_AddToBasket", customerNumber, "TalentEBusiness Redirect to ~/PagesPublic/ProductBrowse/TicketingPrePayment.aspx?ppspage=2")
                    _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=2&product=" & Request.QueryString("product") & "&pricecode=" & campaignCode & "&productsubtype=" & productSubType
                Else
                    Talent.Common.Utilities.TalentCommonLog("TicketingGateway.VisualSeatSelection_AddToBasket", customerNumber, "TalentEBusiness Redirect to ~/PagesPublic/Basket/Basket.aspx")
                    _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                    _redirectToProductLinkingPage = True
                End If
            Else
                _redirectToProductLinkingPage = True
                If ModuleDefaults.HomeProduct_ForwardToBasket Then
                    _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                Else
                    _redirectUrl = "~/PagesPublic/ProductBrowse/productHome.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(True)
                End If
            End If
        End If
    End Sub

    Private Sub SeatSelection_AddToBasket()
        Dim product As String = Talent.eCommerce.Utilities.DecodeString(Request.QueryString("product"))
        Dim stand As String = Talent.eCommerce.Utilities.DecodeString(Request.QueryString("stand"))
        Dim area As String = Talent.eCommerce.Utilities.DecodeString(Request.QueryString("area"))
        Dim stadium As String = Talent.eCommerce.Utilities.DecodeString(Request.QueryString("stadium"))
        Dim an As String = Talent.eCommerce.Utilities.DecodeString(Request.QueryString("an"))
        Dim campaign As String = Talent.eCommerce.Utilities.DecodeString(Request.QueryString("campaign"))
        Dim type As String = Talent.eCommerce.Utilities.DecodeString(Request.QueryString("type"))
        Dim productSubType As String = Talent.eCommerce.Utilities.DecodeString(Request.QueryString("productsubtype"))
        Dim rows As New Generic.Dictionary(Of String, String)
        Dim seats As New Generic.Dictionary(Of String, String)
        Dim count As Integer = 0
        Dim noMoreItems As Boolean = False
        Dim missingParam As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim returnError As Boolean = False
        Dim orphanSeatError As String = String.Empty
        Dim isItCATSeatSelection As Boolean = False

        'if seat selection is for CAT then check required param are exists
        missingParam = IsCATParamMissing()

        If String.IsNullOrEmpty(product) Then
            missingParam = True
        ElseIf String.IsNullOrEmpty(stand) Then
            missingParam = True
        ElseIf String.IsNullOrEmpty(area) Then
            missingParam = True
        End If
        If campaign Is Nothing Then campaign = String.Empty

        Do While Not noMoreItems
            count += 1
            Dim seat As String = Request.QueryString("seat" & count)
            If count = 1 AndAlso String.IsNullOrEmpty(seat) Then missingParam = True
            If String.IsNullOrEmpty(seat) Then
                noMoreItems = True
            Else
                rows.Add("row" & count, Talent.eCommerce.Utilities.DecodeString(seat.Split("/")(0).PadRight(4, " ")))
                seats.Add("seat" & count, Talent.eCommerce.Utilities.DecodeString(seat.Split("/")(1)))
            End If
        Loop

        If Not missingParam Then
            If String.IsNullOrEmpty(_linkedMasterProduct) Then
                Session("QuantityRequested") = count - 1
            End If
            Dim deATI As New Talent.Common.DEAddTicketingItems
            'if seat selection is for CAT populate the CAT ticketing item details
            deATI = GetCATTicketingItemDetails(False)
            With deATI
                .SessionId = Profile.Basket.Basket_Header_ID
                If Not Profile.IsAnonymous Then
                    .CustomerNumber = Profile.User.Details.LoginID.PadLeft(12, "0")
                Else
                    .CustomerNumber = "000000000000"
                End If
                .ProductCode = product
                .ProductType = type
                .StandCode = stand
                .AreaCode = area
                .CampaignCode = campaign
                .ProductHasMandtoryRelatedProducts = _productHasMandatoryRelatedProducts
                Dim counter As Integer = 1
                If rows.Count > 0 Then
                    For Each key As String In rows.Keys
                        Select Case counter
                            Case Is = 1 : .RowSeat01 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 2 : .RowSeat02 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 3 : .RowSeat03 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 4 : .RowSeat04 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 5 : .RowSeat05 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 6 : .RowSeat06 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 7 : .RowSeat07 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 8 : .RowSeat08 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 9 : .RowSeat09 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 10 : .RowSeat10 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 11 : .RowSeat11 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 12 : .RowSeat12 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 13 : .RowSeat13 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 14 : .RowSeat14 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 15 : .RowSeat15 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 16 : .RowSeat16 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 17 : .RowSeat17 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 18 : .RowSeat18 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 19 : .RowSeat19 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 20 : .RowSeat20 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 21 : .RowSeat21 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 22 : .RowSeat22 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 23 : .RowSeat23 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 24 : .RowSeat24 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 25 : .RowSeat25 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 26 : .RowSeat26 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 27 : .RowSeat27 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 28 : .RowSeat28 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 29 : .RowSeat29 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 30 : .RowSeat30 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 31 : .RowSeat31 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 32 : .RowSeat32 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 33 : .RowSeat33 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 34 : .RowSeat34 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 35 : .RowSeat35 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 36 : .RowSeat36 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 37 : .RowSeat37 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 38 : .RowSeat38 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 39 : .RowSeat39 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 40 : .RowSeat40 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 41 : .RowSeat41 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 42 : .RowSeat42 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 43 : .RowSeat43 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 44 : .RowSeat44 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 45 : .RowSeat45 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 46 : .RowSeat46 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 47 : .RowSeat47 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 48 : .RowSeat48 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 49 : .RowSeat49 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 50 : .RowSeat50 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                        End Select
                        counter += 1
                    Next
                End If
                ' Blank = Failure to book any single seat means no at all seats will be booked.
                ' Non-blank = Only seats that succeed are booked, others are not.
                .FailOption = ""
                .Source = "W"
                If Not Session("Agent") Is Nothing AndAlso Not Session("Agent") = "" AndAlso Not Session("AgentType") Is Nothing AndAlso Session("AgentType") = "2" Then
                    .ByPassPreReqCheck = "Y"
                Else
                    .ByPassPreReqCheck = "N"
                End If
            End With

            Dim basket As New Talent.Common.TalentBasket
            basket.DeAddTicketingItems = deATI
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
            basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            Talent.Common.Utilities.TalentCommonLog("TicketingGateway.SeatSelection_AddToBasket", deATI.CustomerNumber, "TalentEBusiness Request to Talent.Common.AmendBasket.AddTicketingItemsReturnBasket")
            err = basket.AddTicketingItemsReturnBasket
            Talent.Common.Utilities.TalentCommonLog("TicketingGateway.SeatSelection_AddToBasket", deATI.CustomerNumber, "TalentEBusiness Response from Talent.Common.AmendBasket.AddTicketingItemsReturnBasket")

            If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                _redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                _packageId = basket.DeAddTicketingItems.PackageID
            End If
            _TicketingGatewayFunctions.SetSessionValuesFromBasket(basket)

            returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)
            If basket.OrphanSeatRemaining Then
                Session("TicketingGatewayError") = GlobalConstants.ORPHANSEATERROR
                Session("TalentErrorCode") = GlobalConstants.ORPHANSEATERROR
                Session("OrphanSeatError") = GlobalConstants.ORPHANSEATERROR
                Dim tProduct As New TalentProduct
                tProduct.Settings = basket.Settings
                tProduct.De.StandCode = stand
                tProduct.De.AreaCode = area
                tProduct.De.ProductCode = product
                tProduct.De.CampaignCode = campaign
                tProduct.De.ComponentID = Talent.eCommerce.CATHelper.GetPackageComponentId(product, HttpContext.Current.Request("callid"))
                tProduct.ProductSeatAvailabilityClearCache()
                Dim referralUrl As New StringBuilder
                referralUrl.Append("~/PagesPublic/ProductBrowse/seatSelection.aspx?product=")
                referralUrl.Append(product)
                referralUrl.Append("&stand=")
                referralUrl.Append(stand)
                referralUrl.Append("&area=")
                referralUrl.Append(area)
                referralUrl.Append("&stadium=")
                referralUrl.Append(stadium)
                referralUrl.Append("&an=")
                referralUrl.Append(an)
                referralUrl.Append("&campaign=")
                referralUrl.Append(campaign)
                referralUrl.Append("&type=")
                referralUrl.Append(type)
                _redirectUrl = referralUrl.ToString()
            End If

            'user is not going back to seat selection for CAT due to orphanseat error so clear cat sessions
            'Clear all transfer session variables
            If Not String.IsNullOrWhiteSpace(HttpContext.Current.Session("catmode")) Then
                Session("catpayref") = Session("payref")
            End If
            isItCATSeatSelection = IsCATSessionsRemoved()

            If returnErrorCode.Length > 0 Then
                If returnErrorCode.Equals("NC") Then
                    Dim tp As New Talent.Common.TalentProduct
                    tp.De.ProductCode = product
                    tp.De.CampaignCode = campaign
                    tp.De.ComponentID = Talent.eCommerce.CATHelper.GetPackageComponentId(product, HttpContext.Current.Request("callid"))
                    Dim agent As New Talent.eCommerce.Agent
                    If agent.IsAgent Then
                        tp.De.AvailableToSell03 = agent.IsAvailableToSell03
                        tp.De.AvailableToSellAvailableTickets = agent.SellAvailableTickets
                    Else
                        tp.De.AvailableToSell03 = True
                        tp.De.AvailableToSellAvailableTickets = False
                    End If
                    tp.AvailableStandsClearCache()
                    tp.AvailableStandsWithoutDescriptionsClearCache()
                    returnErrorCode = "NS"
                End If
                returnError = True
            End If
        Else
            returnError = True
            returnErrorCode = "PARAMERR"
            Talent.eCommerce.Logging.WriteLog(Profile.UserName, "SS_ATB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - SeatSelection_AddToBasket", "Error")
        End If
        If isItCATSeatSelection Then
            If returnError Then
                Session("TicketingGatewayError") = returnErrorCode
                Session("TalentErrorCode") = returnErrorCode
            End If
            If returnErrorCode = "WA" OrElse returnErrorCode = "AC" Then
                Talent.eCommerce.Utilities.ClearOrderEnquiryDetailsCache()
            End If
            _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
        Else
            If UCase(type) = "S" Then
                If returnError Then
                    Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductSeason_AddToBasket", "", "TalentEBusiness Redirect to ~/PagesPublic/ProductBrowse/ProductHome.aspx")
                    Session("TicketingGatewayError") = returnErrorCode
                    Session("TalentErrorCode") = returnErrorCode
                    If String.IsNullOrWhiteSpace(productSubType) Then
                        _redirectUrl = "../PagesPublic/ProductBrowse/ProductSeason.aspx"
                    Else
                        _redirectUrl = "../PagesPublic/ProductBrowse/ProductSeason.aspx?productsubtype=" & productSubType
                    End If
                Else
                    If (ModuleDefaults.PPS_ENABLE_1 AndAlso Not AgentProfile.BulkSalesMode) Then
                        _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=1&product=" & Request.QueryString("product") & "&pricecode=" & campaign
                    ElseIf (ModuleDefaults.PPS_ENABLE_2 AndAlso Not AgentProfile.BulkSalesMode) Then
                        _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=2&product=" & Request.QueryString("product") & "&pricecode=" & campaign
                    Else
                        _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                        _redirectToProductLinkingPage = True
                    End If
                End If
            Else
                If returnError Then
                    Session("TicketingGatewayError") = returnErrorCode
                    Session("TalentErrorCode") = returnErrorCode
                End If
                _redirectToProductLinkingPage = True
                If ModuleDefaults.HomeProduct_ForwardToBasket Then
                    _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                End If
            End If
        End If
    End Sub

    Private Sub ProductHospitality_AddToBasket()
        Dim product As String = Request.QueryString("product")
        Dim quantity As String = Request.QueryString("quantity")
        Dim productType As String = Request.QueryString("type")
        Dim packageID As String = Request.QueryString("packageID")
        Dim seatComponentID As String = Request.QueryString("seatComponentID")
        Dim missingParam As Boolean = False
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim customerNumber As String = String.Empty

        If String.IsNullOrEmpty(product) Then
            missingParam = True
            Me._missingParamErrorCode = "PR"
        ElseIf String.IsNullOrEmpty(quantity) Then
            returnErrorCode = "AAQ"
            missingParam = True
            Me._missingParamErrorCode = "QT"
        ElseIf String.IsNullOrEmpty(packageID) Then
            missingParam = True
            Me._missingParamErrorCode = "PKGID"
        ElseIf String.IsNullOrEmpty(seatComponentID) Then
            'missingParam = True
            'Me.MissingParamErrorCode = "SCOMID"
        End If

        If Not Profile.IsAnonymous Then
            customerNumber = Profile.User.Details.LoginID.PadLeft(12, "0")
        Else
            customerNumber = "000000000000"
        End If

        If Not missingParam Then
            If String.IsNullOrEmpty(_linkedMasterProduct) Then
                Session("QuantityRequested") = quantity
            End If
            Dim deATI As New Talent.Common.DEAddTicketingItems
            With deATI
                .SessionId = Profile.Basket.Basket_Header_ID
                .CustomerNumber = customerNumber
                .AreaCode = ""
                .StandCode = ""
                .ProductCode = product
                .Quantity01 = quantity
                .PriceBand01 = "A"
                .Source = "W"
                .ProductType = productType
                .PackageID = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(packageID)
                .SeatComponentID = seatComponentID
                .ProductHasMandtoryRelatedProducts = _productHasMandatoryRelatedProducts
                If Not Session("Agent") Is Nothing AndAlso Not Session("Agent") = "" AndAlso Not Session("AgentType") Is Nothing AndAlso Session("AgentType") = "2" Then
                    .ByPassPreReqCheck = "Y"
                Else
                    .ByPassPreReqCheck = "N"
                End If
            End With

            Dim basket As New Talent.Common.TalentBasket
            basket.DeAddTicketingItems = deATI
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
            basket.Settings.PerformWatchListCheck = ModuleDefaults.PerformAgentWatchListCheck
            basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductHospitality_AddToBasket", deATI.CustomerNumber, "TalentEBusiness Request to Talent.Common.AmendBasket.AddTicketingItemsReturnBasket")
            err = basket.AddTicketingItemsReturnBasket
            Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductHospitality_AddToBasket", deATI.CustomerNumber, "TalentEBusiness Response from Talent.Common.AmendBasket.AddTicketingItemsReturnBasket")
            returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)

            If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                _redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                _packageId = basket.DeAddTicketingItems.PackageID
            End If
            _TicketingGatewayFunctions.SetSessionValuesFromBasket(basket)

            If returnErrorCode.Length > 0 Then
                returnError = True
                If returnErrorCode.Equals("NC") Then
                    Dim tp As New Talent.Common.TalentProduct
                    tp.De.ProductCode = basket.DeAddTicketingItems.ProductCode
                    tp.De.ProductType = basket.DeAddTicketingItems.ProductType
                    tp.Settings.Company = basket.Settings.Company
                    tp.ProductHospitalityClearCache()
                    returnErrorCode = "NS"
                End If
            End If
        Else
            If returnErrorCode.Trim = "" Then
                returnErrorCode = "PARAMERR"
                Talent.eCommerce.Logging.WriteLog(Profile.UserName, "PH_ATB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - ProductHospitality_AddToBasket", "Error")
            End If
            returnError = True
        End If

        If returnError Then
            Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductHospitality_AddToBasket", "", "TalentEBusiness Redirect to ~/PagesPublic/ProductBrowse/MatchDayHospitality.aspx")
            Session("TicketingGatewayError") = returnErrorCode
            Session("TalentErrorCode") = returnErrorCode
            _redirectUrl = "../PagesPublic/ProductBrowse/MatchDayHospitality.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(False)
        Else
            _redirectToProductLinkingPage = True
            If ModuleDefaults.HospitalityForwardToBasket Then
                Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductHospitality_AddToBasket", customerNumber, "TalentEBusiness Redirect to ~/PagesPublic/Basket/Basket.aspx")
                _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
            Else
                Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductHospitality_AddToBasket", customerNumber, "TalentEBusiness Redirect to ~/PagesPublic/ProductBrowse/MatchDayHospitality.aspx")
                _redirectUrl = "../PagesPublic/ProductBrowse/MatchDayHospitality.aspx" & _TicketingGatewayFunctions.GetProductDetailQueryString(False)
            End If
        End If
    End Sub

    Private Sub CATConfirm_AddToBasket()
        Dim product As String = Request.QueryString("product")
        Dim catMode As String = Request.QueryString("catmode")
        Dim catSeatCustomerNo As String = Request.QueryString("catseatcustomerno")
        Dim payRef As String = Request.QueryString("payref")
        Dim isTransationEnquiry As String = Request.QueryString("istrnxenq")
        Dim catSeat As String = Request.QueryString("catseat")
        Dim bulkSalesId As Integer = Request.QueryString("bulksalesid")
        Dim stand As String = Talent.eCommerce.Utilities.DecodeString(Request.QueryString("stand"))
        Dim area As String = Talent.eCommerce.Utilities.DecodeString(Request.QueryString("area"))
        Dim rows As New Generic.Dictionary(Of String, String)
        Dim seats As New Generic.Dictionary(Of String, String)
        Dim count As Integer = 0
        Dim noMoreItems As Boolean = False
        Dim missingParam As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim returnError As Boolean = False

        If Not String.IsNullOrWhiteSpace(Session("catmode")) Then
            product = Session("product")
            catMode = Session("catmode")
            catSeatCustomerNo = Session("catseatcustomerno")
            payRef = Session("payref")
            isTransationEnquiry = Session("istrnxenq")
            catSeat = Session("catseat")
            bulkSalesId = Session("bulksalesid")
        End If

        If String.IsNullOrWhiteSpace(isTransationEnquiry) Then
            missingParam = True
        ElseIf String.IsNullOrWhiteSpace(catMode) Then
            missingParam = True
        ElseIf String.IsNullOrWhiteSpace(payRef) Then
            missingParam = True
        ElseIf catMode <> GlobalConstants.CATMODE_CANCELALL AndAlso catMode <> GlobalConstants.CATMODE_CANCELMULTIPLE Then
            If String.IsNullOrWhiteSpace(catSeatCustomerNo) Then
                missingParam = True
            ElseIf String.IsNullOrWhiteSpace(product) Then
                missingParam = True
            ElseIf String.IsNullOrWhiteSpace(catSeat) AndAlso bulkSalesId = 0 AndAlso catMode <> GlobalConstants.CATMODE_CANCELMULTIPLE Then
                missingParam = True
            End If
        End If

        If catMode = GlobalConstants.CATMODE_TRANSFER Then
            If String.IsNullOrWhiteSpace(stand) Then
                missingParam = True
            ElseIf String.IsNullOrWhiteSpace(area) Then
                missingParam = True
            End If
            Do While Not noMoreItems
                count += 1
                Dim seat As String = Request.QueryString("seat" & count)

                If count = 1 AndAlso String.IsNullOrEmpty(seat) Then missingParam = True

                If String.IsNullOrEmpty(seat) Then
                    noMoreItems = True
                Else
                    rows.Add("row" & count, Talent.eCommerce.Utilities.DecodeString(seat.Split("/")(0).PadRight(4, " ")))
                    seats.Add("seat" & count, Talent.eCommerce.Utilities.DecodeString(seat.Split("/")(1)))
                End If
            Loop
        Else
            stand = ""
            area = ""
        End If

        If Not missingParam Then
            'Clear all transfer session variables
            Session.Remove("catmode")
            Session.Remove("product")
            Session.Remove("seat1")
            Session.Remove("payref")
            Session.Remove("istrnxenq")
            Session.Remove("seatcustomerno")
            Session.Remove("catseat")
            Session.Remove("bulksalesid")
            Session.Remove("cancelMultipleBulkID")
            Dim DEsd As New DESeatDetails
            Dim DEtid As New DETicketingItemDetails
            Dim deATI As New Talent.Common.DEAddTicketingItems

            If catMode <> GlobalConstants.CATMODE_CANCELALL AndAlso catMode <> GlobalConstants.CATMODE_CANCELMULTIPLE Then DEtid.SeatDetails1.FormattedSeat = catSeat
            If bulkSalesId > 0 Then DEtid.BulkSalesID = bulkSalesId
            '
            ' CAT baskets in cancelmultiple mode may need to retrieve the individual seats for a bulk header.
            If catMode = GlobalConstants.CATMODE_CANCELMULTIPLE And bulkSalesId > 0 Then
                Session.Add("cancelMultipleBulkID", bulkSalesId)
            End If

            With deATI
                .SessionId = Profile.Basket.Basket_Header_ID
                If Not Profile.IsAnonymous Then
                    .CustomerNumber = Profile.User.Details.LoginID.PadLeft(12, "0")
                Else
                    .CustomerNumber = "000000000000"
                End If
                .ProductCode = product
                .ProductType = ""
                .StandCode = stand
                .AreaCode = area
                .CampaignCode = ""
                .BulkSalesId = bulkSalesId
                .AgentCanGiveDirectDebitRefund = AgentProfile.AgentPermissions.CanGiveDirectDebitRefund
                Dim counter As Integer = 1
                If rows.Count > 0 Then
                    For Each key As String In rows.Keys
                        Select Case counter
                            Case Is = 1 : .RowSeat01 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 2 : .RowSeat02 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 3 : .RowSeat03 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 4 : .RowSeat04 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 5 : .RowSeat05 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 6 : .RowSeat06 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 7 : .RowSeat07 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 8 : .RowSeat08 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 9 : .RowSeat09 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 10 : .RowSeat10 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 11 : .RowSeat11 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 12 : .RowSeat12 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 13 : .RowSeat13 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 14 : .RowSeat14 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 15 : .RowSeat15 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 16 : .RowSeat16 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 17 : .RowSeat17 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 18 : .RowSeat18 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 19 : .RowSeat19 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 20 : .RowSeat20 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 21 : .RowSeat21 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 22 : .RowSeat22 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 23 : .RowSeat23 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 24 : .RowSeat24 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 25 : .RowSeat25 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 26 : .RowSeat26 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 27 : .RowSeat27 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 28 : .RowSeat28 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 29 : .RowSeat29 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 30 : .RowSeat30 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 31 : .RowSeat31 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 32 : .RowSeat32 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 33 : .RowSeat33 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 34 : .RowSeat34 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 35 : .RowSeat35 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 36 : .RowSeat36 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 37 : .RowSeat37 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 38 : .RowSeat38 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 39 : .RowSeat39 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 40 : .RowSeat40 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 41 : .RowSeat41 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 42 : .RowSeat42 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 43 : .RowSeat43 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 44 : .RowSeat44 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 45 : .RowSeat45 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 46 : .RowSeat46 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 47 : .RowSeat47 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 48 : .RowSeat48 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 49 : .RowSeat49 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                            Case Is = 50 : .RowSeat50 = rows("row" & counter.ToString) & seats("seat" & counter.ToString)
                        End Select
                        counter += 1
                    Next
                End If
                ' Blank = Failure to book any single seat means no at all seats will be booked.
                ' Non-blank = Only seats that succeed are booked, others are not.
                .FailOption = ""
                .Source = GlobalConstants.SOURCE
                'CAT attributes
                If catMode = GlobalConstants.CATMODE_CANCELALL Then
                    .CATMode = GlobalConstants.CATMODE_CANCEL
                    .CATSeatDetails = ""
                    .CATSeatCustomerNo = ""
                    .CATPayRef = payRef
                ElseIf catMode = GlobalConstants.CATMODE_CANCELMULTIPLE Then
                    Dim dtCatModeCancelMultiple As New DataTable
                    .SeatSelectionArray = New List(Of DESeatDetails)
                    Dim seat As DESeatDetails

                    If HttpContext.Current.Session.Item("CATMODE_CANCELMULTIPLE" & TalentCache.GetBusinessUnit) IsNot Nothing Then
                        dtCatModeCancelMultiple = HttpContext.Current.Session.Item("CATMODE_CANCELMULTIPLE" & TalentCache.GetBusinessUnit)
                    End If
                    If dtCatModeCancelMultiple.Rows.Count > 0 Then
                        deATI.CATMode = GlobalConstants.CATMODE_CANCEL
                        deATI.CATPayRef = dtCatModeCancelMultiple.Rows(0).Item("PaymentReference")
                        For Each r In dtCatModeCancelMultiple.Rows
                            seat = New DESeatDetails
                            seat.UnFormattedSeat = r("Seat")
                            seat.CATSeatStatus = "C"
                            seat.PriceBand = "A"
                            seat.ProductCode = r("ProductCode")
                            .SeatSelectionArray.Add(seat)
                        Next
                    End If
                Else
                    .CATMode = catMode
                    .CATSeatDetails = Utilities.FixStringLength(product, 6) & _
                                        Utilities.FixStringLength(DEtid.SeatDetails1.Stand, 3) & _
                                        Utilities.FixStringLength(DEtid.SeatDetails1.Area, 4) & _
                                        Utilities.FixStringLength(DEtid.SeatDetails1.Row, 4) & _
                                        Utilities.FixStringLength(DEtid.SeatDetails1.Seat, 4) & _
                                        Utilities.FixStringLength(DEtid.SeatDetails1.AlphaSuffix, 1) & _
                                        Utilities.PadLeadingZeros(payRef, 15) & _
                                        Utilities.FixStringLength("", 3)
                    .CATSeatCustomerNo = catSeatCustomerNo
                    .CATPayRef = ""
                End If

                If Not Session("Agent") Is Nothing AndAlso Not Session("Agent") = "" AndAlso Not Session("AgentType") Is Nothing AndAlso Session("AgentType") = "2" Then
                    .ByPassPreReqCheck = "Y"
                Else
                    .ByPassPreReqCheck = "N"
                End If
            End With

            Dim basket As New Talent.Common.TalentBasket
            basket.DeAddTicketingItems = deATI
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
            basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            err = basket.ClearAndAddTicketingItemsReturnBasket
            returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)

            If returnErrorCode.Length > 0 Then
                If returnErrorCode.Equals("NC") Then
                    Dim tp As New Talent.Common.TalentProduct
                    tp.De.ProductCode = product
                    tp.De.CampaignCode = ""
                    tp.De.ComponentID = Talent.eCommerce.CATHelper.GetPackageComponentId(product, HttpContext.Current.Request("callid"))
                    Dim agent As New Talent.eCommerce.Agent
                    If agent.IsAgent Then
                        tp.De.AvailableToSell03 = agent.IsAvailableToSell03
                        tp.De.AvailableToSellAvailableTickets = agent.SellAvailableTickets
                    Else
                        tp.De.AvailableToSell03 = True
                        tp.De.AvailableToSellAvailableTickets = False
                    End If
                    tp.AvailableStandsClearCache()
                    tp.AvailableStandsWithoutDescriptionsClearCache()
                    returnErrorCode = "NS"
                End If
                returnError = True
            End If
        Else
            returnError = True
            returnErrorCode = "PARAMERR"
            Talent.eCommerce.Logging.WriteLog(Profile.UserName, "SS_ATB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - SeatSelection_AddToBasket", "Error")
        End If

        If returnError Then
            Talent.Common.Utilities.TalentCommonLog("TicketingGateway.CATConfirm_AddToBasket", "", "TalentEBusiness Redirect to ~/PagesPublic/Basket/Basket.aspx")
            Session("TicketingGatewayError") = returnErrorCode
            Session("TalentErrorCode") = returnErrorCode
            Talent.eCommerce.Utilities.ClearOrderEnquiryDetailsCache()
        End If
        _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
    End Sub

    Private Sub CATPackage_AddToBasket()

        Dim callID As String = Request.QueryString("callid")
        Dim payRef As String = Request.QueryString("payref")
        Dim product As String = Request.QueryString("product")
        Dim catMode As String = Request.QueryString("catmode")

        Dim missingParam As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim returnError As Boolean = False

        If String.IsNullOrWhiteSpace(callID) Then
            missingParam = True
        ElseIf String.IsNullOrWhiteSpace(catMode) Then
            missingParam = True
        ElseIf String.IsNullOrWhiteSpace(payRef) Then
            missingParam = True
        ElseIf String.IsNullOrWhiteSpace(product) Then
            missingParam = True
        End If

        If Not missingParam Then

            Dim DEsd As New DESeatDetails
            Dim DEtid As New DETicketingItemDetails
            Dim deATI As New Talent.Common.DEAddTicketingItems

            With deATI
                .SessionId = Profile.Basket.Basket_Header_ID
                If Not Profile.IsAnonymous Then
                    .CustomerNumber = Profile.User.Details.LoginID.PadLeft(12, "0")
                Else
                    .CustomerNumber = "000000000000"
                End If
                .ProductCode = product
                .ProductType = ""
                .CampaignCode = ""
                ' Blank = Failure to book any single seat means no at all seats will be booked.
                ' Non-blank = Only seats that succeed are booked, others are not.
                .FailOption = ""
                .Source = "W"
                'CAT attributes
                If catMode = GlobalConstants.CATMODE_CANCELALL Then
                    .CATMode = GlobalConstants.CATMODE_CANCEL
                    .CATSeatDetails = ""
                    .CATSeatCustomerNo = ""
                    .CATPayRef = payRef
                Else
                    .CATMode = catMode
                    .CATSeatDetails = ""
                    .CATPayRef = payRef
                    .CallID = callID
                End If

                If Not Session("Agent") Is Nothing AndAlso Not Session("Agent") = "" AndAlso Not Session("AgentType") Is Nothing AndAlso Session("AgentType") = "2" Then
                    .ByPassPreReqCheck = "Y"
                Else
                    .ByPassPreReqCheck = "N"
                End If
            End With

            Dim basket As New Talent.Common.TalentBasket
            basket.DeAddTicketingItems = deATI
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
            basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            err = basket.ClearAndAddTicketingItemsReturnBasket
            returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)

            If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                _redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                _packageId = basket.DeAddTicketingItems.PackageID
            End If
            _TicketingGatewayFunctions.SetSessionValuesFromBasket(basket)

            If returnErrorCode.Length > 0 Then
                If returnErrorCode.Equals("NC") Then
                    Dim tp As New Talent.Common.TalentProduct
                    tp.De.ProductCode = product
                    tp.De.CampaignCode = ""
                    tp.De.ComponentID = Talent.eCommerce.CATHelper.GetPackageComponentId(product, HttpContext.Current.Request("callid"))
                    Dim agent As New Talent.eCommerce.Agent
                    If agent.IsAgent Then
                        tp.De.AvailableToSell03 = agent.IsAvailableToSell03
                        tp.De.AvailableToSellAvailableTickets = agent.SellAvailableTickets
                    Else
                        tp.De.AvailableToSell03 = True
                        tp.De.AvailableToSellAvailableTickets = False
                    End If
                    tp.AvailableStandsClearCache()
                    tp.AvailableStandsWithoutDescriptionsClearCache()
                    returnErrorCode = "NS"
                End If
                returnError = True
            End If
        Else
            returnError = True
            returnErrorCode = "PARAMERR"
            Talent.eCommerce.Logging.WriteLog(Profile.UserName, "SS_ATB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - SeatSelection_AddToBasket", "Error")
        End If

        If returnError Then
            Talent.Common.Utilities.TalentCommonLog("TicketingGateway.CATConfirm_AddToBasket", "", "TalentEBusiness Redirect to ~/PagesPublic/Basket/Basket.aspx")
            Session("TicketingGatewayError") = returnErrorCode
            Session("TalentErrorCode") = returnErrorCode
            Talent.eCommerce.Utilities.ClearOrderEnquiryDetailsCache()
        End If
        _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
    End Sub

    Private Function retrievePriceBandSelectionFromQueryString() As String(,)
        Dim priceBands(25, 1) As String
        Dim priceBandQueryStringKey As String
        Dim priceBandQuantityQueryStringKey As String
        Dim pbi As Integer = 0

        For i As Integer = 0 To 25 Step 1
            priceBandQueryStringKey = "PB" & i.ToString
            priceBandQuantityQueryStringKey = "PBQ" & i.ToString

            If Request.QueryString(priceBandQueryStringKey) IsNot Nothing AndAlso
                Request.QueryString(priceBandQueryStringKey) IsNot String.Empty AndAlso
                Request.QueryString(priceBandQuantityQueryStringKey) IsNot Nothing AndAlso
                Request.QueryString(priceBandQuantityQueryStringKey) IsNot String.Empty Then
                priceBands(pbi, 0) = Request.QueryString(priceBandQueryStringKey)
                priceBands(pbi, 1) = Request.QueryString(priceBandQuantityQueryStringKey)
            Else
                priceBands(pbi, 0) = String.Empty
                priceBands(pbi, 1) = String.Empty
            End If
            pbi += 1
        Next
        If priceBands(0, 0) IsNot String.Empty Then
            Return priceBands
        End If
        Return Nothing
    End Function

#End Region

#Region "Basket Functions"

    Private Sub Basket_RemoveFromBasket()
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim missingParam As Boolean = False
        Dim product As String = Request("product")
        Dim seat As String = Request("seat")
        Dim priceCode As String = Request("priceCode")
        Dim originalCust As String = Request("originalCust")
        Dim packageID As String = Request("packageID")
        Dim orphanSeatRemaining As Boolean = False
        _TicketingGatewayFunctions.Basket_RemoveFromBasket(True, "", product, seat, priceCode, packageID, originalCust)
    End Sub



    Private Sub UpdateBasketHeader(ByVal results As Data.DataSet)
        Try
            If results.Tables.Count > 1 Then
                Dim UpdateStr As String = "UPDATE tbl_basket_header SET MARKETING_CAMPAIGN = @MarketingCampaign, USER_SELECT_FULFIL = @UserSelectFulfil, " & _
                                          "PAYMENT_OPTIONS = @PaymentOptions, RESTRICT_PAYMENT_OPTIONS = @RestrictPaymentOptions, PAYMENT_ACCOUNT_ID = @PaymentAccountId " & _
                                          "WHERE BASKET_HEADER_ID = @BasketHeaderID"

                Dim connStr As String = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                Dim cmd As New System.Data.SqlClient.SqlCommand(UpdateStr, New System.Data.SqlClient.SqlConnection(connStr))
                With cmd
                    .Connection.Open()
                    .Parameters.Add("@BasketHeaderID", Data.SqlDbType.BigInt).Value = Profile.Basket.Basket_Header_ID
                    If results.Tables(1).Rows(0).Item("MarketingCampaign").ToString.Trim = "Y" Then
                        .Parameters.Add("@MarketingCampaign", Data.SqlDbType.Bit).Value = True
                    Else
                        .Parameters.Add("@MarketingCampaign", Data.SqlDbType.Bit).Value = False
                    End If
                    .Parameters.Add("@UserSelectFulfil", Data.SqlDbType.NVarChar).Value = results.Tables(1).Rows(0).Item("UserSelectFulfilment").ToString.Trim
                    .Parameters.Add("@PaymentOptions", Data.SqlDbType.NVarChar).Value = results.Tables(1).Rows(0).Item("PaymentOptions").ToString().Trim()
                    If results.Tables(1).Rows(0).Item("RestrictPaymentOptions").ToString().Trim() = "Y" Then
                        .Parameters.Add("@RestrictPaymentOptions", Data.SqlDbType.Bit).Value = True
                    Else
                        .Parameters.Add("@RestrictPaymentOptions", Data.SqlDbType.Bit).Value = False
                    End If
                    .Parameters.Add("@PaymentAccountId", Data.SqlDbType.NVarChar).Value = results.Tables(1).Rows(0).Item("PaymentAccountId").ToString.Trim
                    cmd.ExecuteNonQuery()
                End With
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub DeleteBasketHeader()
        Dim basketHeader As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
        Try
            basketHeader.Delete_Users_Basket_Before_Migration(TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner, Profile.User.Details.LoginID.PadLeft(12, "0"))
        Catch ex As Exception
        End Try
    End Sub

    Private Sub DeleteBasketDetail()
        Dim deleteStr As String = " DELETE FROM tbl_basket_detail WHERE MODULE = 'Ticketing' AND BASKET_HEADER_ID = @BasketHeaderID "

        Dim connStr As String = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
            cmd As New System.Data.SqlClient.SqlCommand(deleteStr, New System.Data.SqlClient.SqlConnection(connStr))
        Try
            cmd.Connection.Open()
        Catch ex As Exception
        End Try

        If cmd.Connection.State = Data.ConnectionState.Open Then
            cmd.CommandText = deleteStr
            cmd.Parameters.Add("@BasketHeaderID", Data.SqlDbType.BigInt).Value = Profile.Basket.Basket_Header_ID
            cmd.ExecuteNonQuery()
        End If

        Try
            cmd.Connection.Close()
        Catch ex As Exception
        End Try

        Dim payment As New Talent.Common.TalentPayment
        Dim settings As DESettings = Talent.eCommerce.Utilities.GetSettingsObject()
        payment.RemoveItemFromCache("RetrieveCashback" & settings.Company & Profile.UserName)
    End Sub

    Private Sub RefreshBasketContent(Optional ByVal cashBackUpdate As Boolean = False)
        'Set-up the call to talent common
        Dim basket As New Talent.Common.TalentBasket
        basket.De.SessionId = Profile.Basket.Basket_Header_ID
        basket.De.CustomerNo = Profile.UserName
        basket.De.Src = "W"
        basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
        basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
        basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
        'Refresh the ticketing items
        Dim Err As ErrorObj = basket.RetrieveTicketingItems()

        'Update the basket if error free
        Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
        Dim returnErrorCode As String = ticketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, Err)
    End Sub

    Private Sub RetrieveBasket()
        RefreshBasketContent()
        _redirectUrl = "~/PagesPublic/Basket/basket.aspx"
    End Sub

    Private Sub RetailBasketCheckout()
        If Profile.IsAnonymous Then
            If AgentProfile.IsAgent Then
                Response.Redirect("~/PagesPublic/Profile/CustomerSelection.aspx?returnurl=" + Server.UrlEncode("~/Redirect/TicketingGateway.aspx?page=basket.aspx&function=retailbasketcheckout"))
            Else
                Response.Redirect("~/PagesPublic/Login/login.aspx?ReturnUrl=~/Redirect/TicketingGateway.aspx?page=basket.aspx&function=retailbasketcheckout")
            End If
        Else
            Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
            ticketingGatewayFunctions.Basket_Checkout_Retail(HttpContext.Current.Profile)
            Response.Redirect("~/PagesLogin/Checkout/Checkout.aspx")
        End If
    End Sub
#End Region

#Region "Payment Functions"

    Protected Sub CheckoutPaymentDetails_Finalise()
        _redirectUrl = "~/PagesLogin/Checkout/CheckoutOrderConfirmation.aspx?PaymentRef=" & Request.QueryString("PaymentRef")
    End Sub

    Protected Sub CheckoutPaymentDetails_Promo()
        Dim promoCode As String = Request("promotionCode")
        Session("TicketingGatewayError") = Nothing
        Dim _dePromotions As New DEPromotions(Profile.Basket.Basket_Header_ID, Nothing, "", "", Nothing, "", "", "", 0, promoCode, 0, "", "", "", "", "", "")
        Dim promotions As TalentPromotions = New TalentPromotions()
        promotions.Dep = _dePromotions
        promotions.Settings = Talent.eCommerce.Utilities.GetSettingsObject
        promotions.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        promotions.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
        promotions.Settings.LoginId = Profile.User.Details.LoginID
        promotions.Settings.OriginatingSourceCode = "W"
        promotions.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
        Dim err As ErrorObj = promotions.ProcessPromotions()

        If err.HasError Then
            Session("TicketingGatewayError") = err.ErrorNumber
        Else
            If Not _TicketingGatewayFunctions.ProcessNewBasket(promotions.ResultDataSet) Then
                Session("TicketingGatewayError") = "PNBERR"
            End If
        End If
        If Session("TicketingGatewayError") Is Nothing Then
            _redirectUrl = "~/PagesLogin/Checkout/checkoutPaymentDetails.aspx?PromoApplied=True"
        Else
            _redirectUrl = "~/PagesLogin/Checkout/checkoutPaymentDetails.aspx"
        End If
    End Sub

#End Region

#Region "Kiosk Functions"

    Private Sub Home_KioskFinish()
        Dim deTID As New Talent.Common.DETicketingItemDetails
        deTID.SessionId = Profile.Basket.Basket_Header_ID
        Dim basket As New Talent.Common.TalentBasket
        basket.De = deTID
        basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
        basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
        Dim err As New Talent.Common.ErrorObj
        err = basket.RemoveTicketingItemsReturnBasket
        DeleteBasketDetail()
        FormsAuthentication.SignOut()

        Dim script As New HtmlGenericControl("SCRIPT")
        script.Attributes.Add("type", "text/javascript")
        script.InnerText = "window.external.KFWClose(0);"
        Me.Page.Header.Controls.Add(script)
    End Sub

    Private Sub Home_KioskBack()
        Dim deTID As New Talent.Common.DETicketingItemDetails
        deTID.SessionId = Profile.Basket.Basket_Header_ID
        Dim basket As New Talent.Common.TalentBasket
        basket.De = deTID
        basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
        basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
        Dim err As New Talent.Common.ErrorObj
        err = basket.RemoveTicketingItemsReturnBasket
        DeleteBasketDetail()
        _redirectUrl = "~/PagesPublic/ProductBrowse/ProductHome.aspx"
    End Sub

    Private Sub Basket_KioskRestart()
        Dim deTID As New Talent.Common.DETicketingItemDetails
        deTID.SessionId = Profile.Basket.Basket_Header_ID
        Dim basket As New Talent.Common.TalentBasket
        basket.De = deTID
        basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
        basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
        Dim err As New Talent.Common.ErrorObj
        err = basket.RemoveTicketingItemsReturnBasket
        DeleteBasketDetail()
        _redirectUrl = Talent.eCommerce.Utilities.GetSiteHomePage()
    End Sub

    Private Sub Basket_KioskCheckout()
        Dim err As New ErrorObj
        Dim returnErrorCode As String = String.Empty
        Dim payment As New Talent.Common.TalentPayment
        Dim dePayment As New Talent.Common.DEPayments
        With dePayment
            .SessionId = Profile.Basket.Basket_Header_ID
            .Source = "W"
            .PaymentType = "CC"
            'This flag instructs the back end not to take payment but set everything payment pending
            .exPayment = "1"
        End With

        Dim wfrPage As New Talent.Common.WebFormResource
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
        End With

        payment.De = dePayment
        payment.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        payment.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        payment.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
        payment.Settings.Cacheing = CType(wfrPage.Attribute("PaymentCacheing"), Boolean)
        payment.Settings.CacheTimeMinutes = CType(wfrPage.Attribute("PaymentCacheTimeMinutes"), Integer)
        payment.Settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
        payment.Settings.OriginatingSource = "KIOSK"
        err = payment.PaymentPending
        returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(payment.ResultDataSet, err)
        If returnErrorCode.Length > 0 Then
            err.HasError = True
            Session("TalentErrorCode") = returnErrorCode
            Session("TicketingGatewayError") = returnErrorCode
        End If

        'If successful, take payment, if not forward back to the basket page
        If Not err.HasError Then
            ' Attempt to take payment - this will forward to the confirmation page if 
            ' success, basket page if failure.
            Dim total As Decimal
            Dim totalPence As Integer
            Dim quote As String = """"
            Dim script As New HtmlGenericControl("SCRIPT")
            script.Attributes.Add("type", "text/javascript")
            script.InnerText = "function go_now(url) { window.location.href = url; }"
            Me.Page.Header.Controls.Add(script)

            For Each bi As TalentBasketItem In Profile.Basket.BasketItems
                total += bi.Gross_Price
            Next
            totalPence = total * 100

            Dim sb As New StringBuilder
            With sb
                .Append("<SCRIPT type=")
                .Append(quote)
                .Append("text/javascript")
                .Append(quote)
                .Append(">if (window.external.KFWChargeCard('")
                .Append(Session("tktKey"))
                .Append("', '")
                .Append(Profile.Basket.Basket_Header_ID)
                .Append("',")
                .Append(totalPence.ToString)
                .Append(")==true { go_now(")
                .Append(quote)
                .Append("../Redirect/TicketingGateway.aspx?page=TicketingGateway.aspx&function=kioskpaymentsuccess")
                .Append(quote)
                .Append("); } else { go_now(")
                .Append(quote)
                .Append("../Redirect/TicketingGateway.aspx?page=TicketingGateway.aspx&function=kioskpaymentfailure")
                .Append(quote)
                .Append("); }</SCRIPT>")
            End With
            Output.Text = sb.ToString
        Else
            '**TODO**Send errors back to the basket here
            _redirectUrl = "~/PagesPublic/Basket/basket.aspx"
        End If
    End Sub

    Private Sub TicketingGateway_KioskPaymentSuccess()
        'Update the back end
        Dim err As New ErrorObj
        Dim returnErrorCode As String = String.Empty
        Dim payment As New Talent.Common.TalentPayment
        Dim dePayment As New Talent.Common.DEPayments
        With dePayment
            .SessionId = Profile.Basket.Basket_Header_ID
            .Source = "W"
            .PaymentType = "CC"
            'This flag instructs the back end not to take payment but call completion routines
            .exPayment = "2"
        End With

        Dim wfrPage As New Talent.Common.WebFormResource
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
        End With

        payment.De = dePayment
        payment.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        payment.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        payment.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
        payment.Settings.Cacheing = CType(wfrPage.Attribute("PaymentCacheing"), Boolean)
        payment.Settings.CacheTimeMinutes = CType(wfrPage.Attribute("PaymentCacheTimeMinutes"), Integer)
        payment.Settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
        payment.Settings.OriginatingSource = "KIOSK"
        err = payment.TakePaymentReadOrder
        returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(payment.ResultDataSet, err)
        If returnErrorCode.Length > 0 Then
            err.HasError = True
        End If

        'Print the receipt
        Dim quote As String = """"
        Dim printURL As String = "http://" & Request.Url.Host & "/Print/FortressKioskReceipt.aspx"
        Dim script As New HtmlGenericControl("SCRIPT")
        script.Attributes.Add("type", "text/javascript")
        script.InnerText = "function go_now(url) { window.location.href = url; }"
        Me.Page.Header.Controls.Add(script)

        'Send print instruction
        Dim sb As New StringBuilder
        With sb
            .Append("<SCRIPT type=")
            .Append(quote)
            .Append("text/javascript")
            .Append(quote)
            .Append(">window.external.KFWPrinter('")
            .Append(Session("tktKey"))
            .Append("',0,'")
            .Append(printURL)
            .Append("');</SCRIPT>")
        End With
        Output.Text = sb.ToString

        'Auto-forward to confirmation page
        Dim sb2 As New StringBuilder
        With sb2
            .Append("<SCRIPT type=")
            .Append(quote)
            .Append("text/javascript")
            .Append(quote)
            .Append(">go_now(")
            .Append(quote)
            .Append("../PagesLogin/Checkout/CheckoutOrderConfirmation.aspx")
            .Append(quote)
            .Append(");</SCRIPT>")
        End With
        Output.Text &= sb2.ToString
    End Sub

    Private Sub TicketingGateway_KioskPaymentFailure()
        ' Reset back end records (where keep03 = '2')
        Dim err As New ErrorObj
        Dim returnErrorCode As String = String.Empty

        Dim payment As New Talent.Common.TalentPayment
        Dim dePayment As New Talent.Common.DEPayments
        With dePayment
            .SessionId = Profile.Basket.Basket_Header_ID
            .Source = GlobalConstants.SOURCE
            .PaymentType = "CC"
            'This flag instructs the back end to reset everything from payment pending
            .exPayment = "3"
        End With

        Dim wfrPage As New Talent.Common.WebFormResource
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
        End With

        payment.De = dePayment
        payment.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        payment.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        payment.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
        payment.Settings.Cacheing = CType(wfrPage.Attribute("PaymentCacheing"), Boolean)
        payment.Settings.CacheTimeMinutes = CType(wfrPage.Attribute("PaymentCacheTimeMinutes"), Integer)
        payment.Settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
        payment.Settings.OriginatingSource = "KIOSK"
        err = payment.PaymentPending
        returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(payment.ResultDataSet, err)
        If returnErrorCode.Length > 0 Then
            err.HasError = True
            Session("TalentErrorCode") = returnErrorCode
            Session("TicketingGatewayError") = returnErrorCode
        End If
        ' Send errors back to the basket here
        _redirectUrl = "~/PagesPublic/Basket/basket.aspx"
    End Sub

#End Region

#Region "Other Functions"

    Private Sub Login_ClearBasket()
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim missingParam As Boolean = False
        Dim returnURL As String = Server.UrlDecode(Request("returnURL"))

        If String.IsNullOrEmpty(returnURL) Then missingParam = True
        If Not missingParam Then
            Dim deTID As New Talent.Common.DETicketingItemDetails
            deTID.SessionId = Profile.Basket.Basket_Header_ID

            Dim basket As New Talent.Common.TalentBasket
            basket.De = deTID
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
            basket.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            err = basket.RemoveTicketingItemsReturnBasket
            returnErrorCode = _TicketingGatewayFunctions.CheckResponseForError(basket.ResultDataSet, err)

            If returnErrorCode.Length > 0 AndAlso returnErrorCode <> "WF" Then returnError = True
            If Not returnError Then DeleteBasketDetail()
        Else
            returnErrorCode = "PARAMERR"
            Talent.eCommerce.Logging.WriteLog(Profile.UserName, "LI_CB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - Login_ClearBasket", "Error")
        End If
        If Not returnError Then
            DeleteBasketDetail()
        Else
            Session("TicketingGatewayError") = returnErrorCode
            Session("TalentErrorCode") = returnErrorCode
        End If
        _redirectUrl = returnURL
    End Sub

    Private Sub OrderReturn_OrderReturn()
        Try
            Dim order As New Talent.Common.TalentTicketExchange
            Dim settings As New Talent.Common.DESettings
            Dim err As New Talent.Common.ErrorObj
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.BusinessUnit = TalentCache.GetBusinessUnit()
            settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            settings.LoginId = Talent.Common.Utilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)
            settings.Cacheing = False
            order.Settings() = settings

            'Loop through all of the records displayed and add them to the TalentOrder collection.
            Dim myList As New Generic.List(Of String())(6)
            myList = CType(Session.Item("SelectedOrderList"), Generic.List(Of String()))
            If Not myList Is Nothing AndAlso myList.Count > 0 Then
                For Each entry() As String In myList

                    'Create a seat details object
                    Dim deSeatDtls As New Talent.Common.DESeatDetails
                    With deSeatDtls
                        If entry(4).Length >= 15 Then
                            .Stand = entry(4).Substring(0, 3)
                            .Area = entry(4).Substring(3, 4)
                            .Row = entry(4).Substring(7, 4)
                            .Seat = entry(4).Substring(11, 4)
                            If entry(4).Length >= 16 Then .AlphaSuffix = entry(4).Substring(15, 1).Trim
                        End If
                    End With

                    'Determine if we are returning a seat, or reebooking a returned seat.
                    Dim type As Decimal
                    If entry(5).ToString.Trim = "Booked" Then type = GlobalConstants.TicketExchangeItemStatus.PlacingOnSale
                    If entry(5).ToString.Trim = "Returned" Then type = GlobalConstants.TicketExchangeItemStatus.TakingOffSale

                    Dim deTicketingItemDetails As New Talent.Common.DETicketExchangeItem
                    With deTicketingItemDetails
                        .SeatedCustomerNo = entry(0)
                        .ProductCode = entry(3)
                        .SeatDetails = deSeatDtls
                        .Status = type
                        .Fee = 0
                        .FeeType = String.Empty
                    End With

                    order.Dep.OriginatingSourceCode = GlobalConstants.SOURCE
                    order.Dep.Comment1 = Request("comment1")
                    order.Dep.Comment2 = Request("comment2")
                    order.Dep.TicketExchangeItems.Add(deTicketingItemDetails)
                    order.Dep.ListingCustomer = entry(0)
                Next

                'Call the Order Return Function
                err = order.OrderReturn()

                If Not err.HasError Then
                    If order.ResultDataSet().Tables(0).Rows(0).Item("ErrorOccurred").ToString = GlobalConstants.ERRORFLAG Then
                        Session("errorMsg") = (order.ResultDataSet().Tables(0).Rows(0).Item("ReturnCode").ToString()).Trim()
                        ' Clear the selected order list.
                        Session.Remove("SelectedOrderList")
                        _redirectUrl = "~/PagesLogin/Orders/OrderReturnEnquiry.aspx"
                    Else
                        'Display Confirmation Page and send email on success 
                        _redirectUrl = "~/PagesLogin/Orders/OrderReturnConfirmation.aspx?orderReturnRef=" & Server.UrlEncode(order.ResultDataSet().Tables(1).Rows(0).Item("TicketExchangeReference").ToString().Trim)
                    End If
                Else
                    Session("errorMsg") = "XX"
                    ' Clear the selected order list.
                    Session.Remove("SelectedOrderList")
                    _redirectUrl = "~/PagesLogin/Orders/OrderReturnEnquiry.aspx"
                End If
            Else
                'Output 'No orders for selection' Text.
                Session("errorMsg") = "XX"
                ' Clear the selected order list.
                Session.Remove("SelectedOrderList")
                _redirectUrl = "~/PagesLogin/Orders/OrderReturnEnquiry.aspx"
            End If

        Catch ex As System.Threading.ThreadAbortException
            'Do nothing on successful redirect.
        Catch ex As Exception
            ' Clear the selected order list. And display an error if the call was not a success.
            Session("errorMsg") = "XX"
            Session.Remove("SelectedOrderList")
            _redirectUrl = "~/PagesLogin/Orders/OrderReturnEnquiry.aspx"
        End Try
    End Sub

    Private Sub clearStandAreaCache(ByRef basket As Talent.Common.TalentBasket)
        Dim tp As New Talent.Common.TalentProduct
        tp.De.ProductCode = basket.DeAddTicketingItems.ProductCode
        tp.De.ProductType = basket.DeAddTicketingItems.ProductType
        tp.De.CampaignCode = basket.DeAddTicketingItems.CampaignCode
        tp.De.ComponentID = Talent.eCommerce.CATHelper.GetPackageComponentId(basket.DeAddTicketingItems.ProductCode, HttpContext.Current.Request("callid"))
        Dim agent As New Talent.eCommerce.Agent
        If agent.IsAgent Then
            tp.De.AvailableToSell03 = agent.IsAvailableToSell03
            tp.De.AvailableToSellAvailableTickets = agent.SellAvailableTickets
        Else
            tp.De.AvailableToSell03 = True
            tp.De.AvailableToSellAvailableTickets = False
        End If
        tp.Settings.Company = basket.Settings.Company
        tp.AvailableStandsClearCache()
        tp.AvailableStandsWithoutDescriptionsClearCache()
    End Sub

#End Region

End Class