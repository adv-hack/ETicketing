Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TalentBusinessLogic.Models
Imports Talent.Common
Imports TalentBusinessLogic.ModelBuilders

Partial Class PagesPublic_Basket
    Inherits TalentBase01

    Private _wfr As Talent.Common.WebFormResource = Nothing
    Private _languageCode As String = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _wfr = New Talent.Common.WebFormResource
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "Basket.aspx"
        End With
        Session("TemplateIDs") = Nothing
        Session("AddInfoCompleted") = Nothing
        If IsPostBack Then
            BasketDetails1.Error_List.Items.Clear()
            Dim searchNewCustomer As String = hdfSearchNewCustomer.Value
            Dim newCustomerNumber As String = hdfNewCustomerNumber.Value
            Dim selectedBasketDetailId As String = hdfSelectedBasketDetailId.Value
            Dim validationError As Boolean = False
            If searchNewCustomer = "true" Then
                Dim customer As VerifyAndRetrieveCustomerViewModel = GetAndValidateCustomer(newCustomerNumber)
                If customer.Valid Then
                    For Each item As DEBasketItem In Profile.Basket.BasketItems
                        If item.Basket_Detail_ID = selectedBasketDetailId Then
                            Dim bulkSalesQuantity As Integer = 0
                            If item.BULK_SALES_ID > 0 Then bulkSalesQuantity = item.Quantity
                            Dim redirectUrl As String
                            redirectUrl = UpdateCustomerBasket(selectedBasketDetailId, item.PRODUCT_TYPE, item.Product, item.PRODUCT_SUB_TYPE, item.PACKAGE_ID, item.PRICE_CODE, item.PRICE_BAND, item.CURR_FULFIL_SLCTN, item.SEAT, item.BULK_SALES_ID, bulkSalesQuantity, newCustomerNumber, item.LOGINID)
                            Response.Redirect(formatUrlForLocalHost(redirectUrl))
                            Exit For
                        End If
                    Next
                Else
                    If Not String.IsNullOrEmpty(newCustomerNumber) Then
                        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
                        Session("ErrorMessage") = String.Format(_wfr.Content("ErrorText_FancardDoesNotExist", _languageCode, True), newCustomerNumber)
                        Response.Redirect(Request.Url.PathAndQuery)
                    End If
                End If
            End If
        Else
            'Entering invalid customer number used to reset the previously selected customer in the customer dropdown to get the right value added the response.redirect to same page
            If Session("ErrorMessage") IsNot Nothing Then
                BasketDetails1.Error_List.Items.Add(Session("ErrorMessage").ToString)
                Session("ErrorMessage") = Nothing
            End If
        End If
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        ' This is a required hidden field that allows the reservation processes to determine if there is a 
        ' basket error (other than ones raised by reservation processes)
        If Not IsPostBack Then
            hdfNoneReservationErrorCount.Value = BasketDetails1.Error_List.Items.Count
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not Page.IsPostBack Then
            If (Session("OrderAlreadyPaid") IsNot Nothing) AndAlso (Session("OrderAlreadyPaid") = "YES") Then
                Dim userProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
                Dim tempOrderID As String = String.Empty
                If userProfile.Basket IsNot Nothing AndAlso (Not String.IsNullOrEmpty(userProfile.Basket.Temp_Order_Id)) Then
                    tempOrderID = userProfile.Basket.Temp_Order_Id
                End If
                BasketAlreadyPaidLabel.Visible = True
                BasketAlreadyPaidLabel.Text = (_wfr.Content("BasketAlreadyPaidLabelText", _languageCode, True)).Replace("<<< temporderid >>>", tempOrderID)
                Session.Remove("OrderAlreadyPaid")
            End If
        End If

        ReservationSetup()
        Page.MaintainScrollPositionOnPostBack = True
    End Sub

    Private Sub ReservationSetup()
        If AgentProfile.IsAgent And Not Profile.IsAnonymous Then
            ReserveTickets.Visible = True
            If hasBasketReservedItems() Then
                UnreserveTickets.Visible = True
            End If
        End If
    End Sub

    Private Function hasBasketReservedItems() As Boolean
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            If item.RESERVED_SEAT = "Y" Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Validates whether the given customer number exists, if yes retrieves the customer details
    ''' </summary>
    ''' <param name="customerNumber"></param>
    ''' <returns>verified customer view model</returns>
    Private Function GetAndValidateCustomer(ByVal customerNumber As String) As VerifyAndRetrieveCustomerViewModel
        Dim setting As New DESettings
        Dim inputModel As New VerifyAndRetrieveCustomerInputModel
        Dim builder As New VerifyAndRetrieveCustomerBuilder

        If (TEBUtilities.CheckForDBNull_String(AgentProfile.Type).Trim().Equals("2")) Or AgentProfile.IsAgent() Then
            inputModel.PerformWatchListCheck = ModuleDefaults.PerformAgentWatchListCheck
        Else
            inputModel.PerformWatchListCheck = False
        End If

        inputModel.BasketID = Profile.Basket.Basket_Header_ID
        inputModel.CustomerNumber = customerNumber.Trim
        inputModel.MembershipNumber = String.Empty
        inputModel.CorporateSaleID = String.Empty
        inputModel.PaymentReference = String.Empty

        Return builder.VerifyAndRetrieveCustomer(inputModel)
    End Function

    ''' <summary>
    ''' This function updates the customer basket
    ''' </summary>
    ''' <param name="BasketID"></param>
    ''' <param name="ProductType"></param>
    ''' <param name="ProductCode"></param>
    ''' <param name="ProductSubType"></param>
    ''' <param name="PackageId"></param>
    ''' <param name="PriceCode"></param>
    ''' <param name="PriceBand"></param>
    ''' <param name="FulfilmentMethod"></param>
    ''' <param name="Seat"></param>
    ''' <param name="BulkSalesId"></param>
    ''' <param name="BulkSalesQuantity"></param>
    ''' <param name="NewUser"></param>
    ''' <param name="OriginalUser"></param>
    ''' <returns>redirectUrl</returns>
    Private Function UpdateCustomerBasket(ByVal BasketID As String,
                                                ByVal ProductType As String,
                                                ByVal ProductCode As String,
                                                ByVal ProductSubType As String,
                                                ByVal PackageId As String,
                                                ByVal PriceCode As String,
                                                ByVal PriceBand As String,
                                                ByVal FulfilmentMethod As String,
                                                ByVal Seat As String,
                                                ByVal BulkSalesId As String,
                                                ByVal BulkSalesQuantity As String,
                                                ByVal NewUser As String,
                                                ByVal OriginalUser As String) As String

        Dim talPackage As New TalentPackage
        talPackage.RemoveCustomerPackageSession(PackageId, ProductCode, 0)

        Dim redirectUrl As New StringBuilder
        redirectUrl.Append("/Redirect/TicketingGateway.aspx?page=basket.aspx&function=updatebasket")
        redirectUrl.Append("&product1=").Append(ProductCode)
        redirectUrl.Append("&customer1=").Append(NewUser)
        redirectUrl.Append("&concession1=").Append(PriceBand)
        redirectUrl.Append("&priceCode1=").Append(PriceCode)
        redirectUrl.Append("&originalCust1=").Append(OriginalUser)
        redirectUrl.Append("&productSubtype1=").Append(ProductSubType)
        redirectUrl.Append("&productType1=").Append(ProductType)
        redirectUrl.Append("&fulfilmentMethod1=").Append(FulfilmentMethod)
        If BulkSalesId > 0 Then
            redirectUrl.Append("&seat1=").Append(Seat.Trim())
        Else
            redirectUrl.Append("&seat1=").Append(Seat)
        End If
        redirectUrl.Append("&packageId1=").Append(PackageId)
        redirectUrl.Append("&customerSelection=").Append("Y")
        redirectUrl.Append("&bulkSalesID1=").Append(BulkSalesId)
        redirectUrl.Append("&bulkSalesQuantity1=").Append(BulkSalesQuantity)
        redirectUrl.Append("&returnurl=").Append(Server.UrlEncode(Request.Url.PathAndQuery))
        Return redirectUrl.ToString
    End Function

    Private Function formatUrlForLocalHost(ByVal url As String) As String
        Dim formattedUrl As String = url
        If Request.Url.AbsoluteUri.Contains("localhost") AndAlso url.ToUpper().Contains("TALENTEBUSINESS") AndAlso url.ToUpper().IndexOf("/TALENTEBUSINESS") > 0 Then
            formattedUrl = url.Substring(url.ToUpper().IndexOf("/TALENTEBUSINESS"), url.Length - url.ToUpper().IndexOf("/TALENTEBUSINESS"))
            Dim port As String = Request.Url.Port
        Else
            formattedUrl = "~" & formattedUrl
        End If
        Return formattedUrl
    End Function
End Class
