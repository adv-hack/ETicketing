Imports System.Data
Imports System.Collections.Generic
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common
Imports TalentBusinessLogic.ModelBuilders.CRM
Imports TalentBusinessLogic.Models

Partial Class UserControls_BasketButtons
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ucr As Talent.Common.UserControlResource
    Private _languageCode As String
    Private _exceededBulkSalesBasketLimit As Boolean = False

#End Region

#Region "Public Properties"

    Public ReadOnly Property TicketingBasketDetails() As UserControls_TicketingBasketDetails
        Get
            Dim tbd1 As New UserControls_TicketingBasketDetails
            tbd1 = CType(TEBUtilities.FindWebControl("TicketingBasketDetails1", Me.Page.Controls), UserControls_TicketingBasketDetails)
            Return tbd1
        End Get
    End Property
    Public ReadOnly Property StandardBasketDetails() As UserControls_BasketDetails
        Get
            Dim bd1 As New UserControls_BasketDetails
            bd1 = CType(TEBUtilities.FindWebControl("BasketDetails1", Me.Page.Controls), UserControls_BasketDetails)
            Return bd1
        End Get
    End Property

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _ucr = New Talent.Common.UserControlResource
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.Common.Utilities.GetAllString
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "BasketButtons.ascx"
        End With
        plhOrderTemplates.Visible = (ModuleDefaults.OrderTemplates AndAlso Not TEBUtilities.IsPartnerHomeDeliveryType(Profile))
        If Profile.IsAnonymous Then
            plhSaveOrders.Visible = False
        Else
            plhSaveOrders.Visible = (ModuleDefaults.SaveOrders AndAlso Not TEBUtilities.IsPartnerHomeDeliveryType(Profile))
        End If
        FastCashButton.Visible = isFastCashButtonVisible()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then Session("DeliveryDetails") = Nothing
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        ShowButtonsByBasketContentType()
        ReservationSetup()
        ShowButtonsByCATMode()
        If AgentProfile.IsAgent AndAlso Profile.IsAnonymous Then AddLinkRegButton.Visible = False
        If Not Page.IsPostBack Then
            OtherMatchesButton.Text = _ucr.Content("OtherMatchesButton", _languageCode, True)
            AddLinkRegButton.Text = _ucr.Content("AddLinkRegistrationButton", _languageCode, True)
        End If
        If ModuleDefaults.TicketingKioskMode Then
            OtherMatchesButton.Visible = False
            AddLinkRegButton.Visible = False
            ContinueShoppingButton.Visible = False
            UpdateBasketButton.Visible = False
        End If
        If AddLinkRegButton.Visible Then AddLinkRegButton.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("ShowAddFFButton"))
    End Sub

    Protected Sub ContinueShoppingButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ContinueShoppingButton.Click
        If String.IsNullOrWhiteSpace(_ucr.Content("ContinueShoppingTargetURL", _languageCode, True)) Then
            Response.Redirect(TEBUtilities.GetSiteHomePage())
        Else
            Response.Redirect(_ucr.Content("ContinueShoppingTargetURL", _languageCode, True))
        End If
    End Sub

    Protected Sub SaveOrderButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveOrderButton.Click
        Try
            Dim soa As New SavedOrdersDataSetTableAdapters.tbl_order_saved_headerTableAdapter
            Dim dt As DateTime = Now
            soa.CreateNewSavedOrder(TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), Profile.UserName, dt, dt, dt, False)
            Dim savedOrders As SavedOrdersDataSet.tbl_order_saved_headerDataTable
            savedOrders = soa.Get_Unprocessed_By_BU_Partner_LoginID_CreatedDate(TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), Profile.UserName, dt)

            If savedOrders.Rows.Count > 0 Then
                Dim order As SavedOrdersDataSet.tbl_order_saved_headerRow = savedOrders.Rows(0)
                Dim linesAdapter As New SavedOrdersDataSetTableAdapters.tbl_order_saved_detailTableAdapter
                For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                    If Not tbi.IS_FREE Then
                        linesAdapter.AddItemToSavedOrder(order.SAVED_HEADER_ID, tbi.Product, tbi.Quantity, tbi.MASTER_PRODUCT)
                    End If
                Next
            End If
            StandardBasketDetails.Error_List.Items.Add(_ucr.Content("SavedOrderConfirmationText", _languageCode, True))
        Catch ex As Exception
            StandardBasketDetails.Error_List.Items.Add(_ucr.Content("SavedOrderErrorText", _languageCode, True))
        End Try
    End Sub

    Protected Sub UpdateBasketButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles UpdateBasketButton.Click
        Dim isErrorInRetail As Boolean = updateRetailBasket()
        updateTicketingBasket()

        'if we have not already redirected to another page due to the ticketing control, then we should re-load the basket to ensure all changes are displayed
        If Not isErrorInRetail Then
            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End If
    End Sub

    Protected Sub SaveTemplateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveTemplateButton.Click
        Response.Redirect("~/PagesLogin/Template/SaveBasketAsTemplate.aspx")
    End Sub

    Protected Sub ClearBasketButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ClearBasketButton.Click
        Dim clearTicketingBasket As Boolean = False
        If Profile.Basket.BasketItems.Count > 0 AndAlso Not String.IsNullOrWhiteSpace(Profile.Basket.CAT_MODE) Then
            clearTicketingBasket = True
        End If
        TDataObjects.BasketSettings.TblBasketHeader.UpdateLinkedProductMaster(Profile.Basket.Basket_Header_ID, String.Empty)
        StandardBasketDetails.Error_List.Items.Clear()
        Profile.Basket.EmptyBasket()
        Session("AllPrmotionCodesEnteredByUser") = Nothing
        Session("AlternativeProducts") = Nothing
        If Not TicketingBasketDetails.Event_Repeater.Items.Count > 0 Then StandardBasketDetails.BindBasketRepeater()
        If Not Session("personalisationTransactions") Is Nothing Then
            Session("personalisationTransactions") = Nothing
        End If
        If TicketingBasketDetails.Event_Repeater.Items.Count > 0 OrElse clearTicketingBasket Then
            Response.Redirect("~/Redirect/TicketingGateway.aspx?page=Basket.aspx&function=ClearBasket")
        End If
        'if basket has only merchandise items then refresh mini basket
        Dim basketMini As Object = TEBUtilities.FindWebControl("MiniBasket1", Me.Page.Controls)
        If basketMini IsNot Nothing Then
            'refresh profile basket before rebinding minibasket
            CallByName(basketMini, "ReBindBasket", CallType.Method)
        End If
        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, Not Profile.IsAnonymous)
    End Sub

    Protected Sub CheckoutButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckoutButton.Click
        If isCheckoutValid() Then
            If ModuleDefaults.PaymentGatewayType = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
                Dim ticketGatewayFunc As New TicketingGatewayFunctions
                ticketGatewayFunc.CheckoutExternalFailure(False, CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID, True)
                Talent.eCommerce.Checkout.RemovePaymentDetailsFromSession()
            End If
            ProcessCheckOut()
        End If
    End Sub

    Protected Sub OtherMatchesButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles OtherMatchesButton.Click
        Dim url As String = _ucr.Content("OtherMatchesButtonURL", _languageCode, True)
        If Not String.IsNullOrEmpty(url) Then
            Response.Redirect(url)
        End If
    End Sub

    Protected Sub AddLinkRegButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AddLinkRegButton.Click
        Response.Redirect("~/PagesLogin/FriendsAndFamily/FriendsAndFamily.aspx")
    End Sub

    Protected Sub FastCashButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles FastCashButton.Click
        Dim isUpdated As Boolean = False
        Dim redirectQuery As String = TicketingBasketDetails.GetUpdatedItemsQueryString(_exceededBulkSalesBasketLimit)
        If Not _exceededBulkSalesBasketLimit Then
            Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
            Session("RedirectCheckout") = redirectQuery
            ticketingGatewayFunctions.Basket_Checkout(False)
            If isCheckoutValid() Then
                If Talent.eCommerce.Utilities.BasketContentTypeWithOverride = GlobalConstants.TICKETINGBASKETCONTENTTYPE OrElse Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                    Dim talBasketSummary As New Talent.Common.TalentBasketSummary
                    talBasketSummary.Settings = TEBUtilities.GetSettingsObject()
                    talBasketSummary.LoginID = Profile.UserName
                    isUpdated = talBasketSummary.UpdateBasketPayTypeOrCardType(Profile.Basket.Basket_Header_ID, GlobalConstants.CSPAYMENTTYPE, "", False, True)
                Else
                    isUpdated = True
                End If
                Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
                SetupDeliveryDetails()
                Checkout.StoreCSDetails()
                If Not Profile.IsAnonymous Then
                    If Not IsPostBack AndAlso Not AgentProfile.BulkSalesMode Then
                        CType(Me.Page, TalentBase01).BasketContainsItemLinkedToTemplate(Profile.Basket.BasketItems, ModuleDefaults.CacheDependencyPath, Profile.Basket.Basket_Header_ID)
                    End If
                    If Session("TemplateIDs") Is Nothing Then
                        ticketingGatewayFunctions.CheckoutPayment()
                    Else
                        Response.Redirect("~/PagesLogin/Checkout/AdditionalProductInformation.aspx?FastCash=True")
                    End If
                Else
                    ticketingGatewayFunctions.CheckoutPayment()
                End If
            End If
        Else
            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End If
    End Sub

    Protected Sub AddAnotherFixtureButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AddAnotherFixtureButton.Click
        Dim componentID As Int64 = getComponentID(HttpContext.Current.Session("LastAddedProductCode"), HttpContext.Current.Session("LastAddedPackageId"))
        Dim redirectUrl As New StringBuilder
        redirectUrl.Append("~/PagesPublic/Hospitality/HospitalityPackageDetails.aspx?")
        redirectUrl.Append("packageid=").Append(HttpContext.Current.Session("LastAddedPackageId"))
        redirectUrl.Append("&producttype=H")
        redirectUrl.Append("&availabilitycomponentid=").Append(componentID)
        Response.Redirect(redirectUrl.ToString())
    End Sub

#End Region

#Region "Private Methods"
    Private Sub PerformTicketingCheckoutClick(Optional ByVal redirectOnBasketCheckout As Boolean = True)
        If Profile.IsAnonymous AndAlso Not AgentProfile.IsAgent Then
            Response.Redirect("~/PagesPublic/Login/Login.aspx?ReturnUrl=~/PagesPublic/Basket/Basket.aspx")
        End If
        Dim redirectQuery As String = TicketingBasketDetails.GetUpdatedItemsQueryString(_exceededBulkSalesBasketLimit)
        If Not _exceededBulkSalesBasketLimit Then
            Session("RedirectCheckout") = redirectQuery
            Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
            ticketingGatewayFunctions.Basket_Checkout(redirectOnBasketCheckout)
        Else
            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End If
    End Sub

    Private Function ProductCodeExistsInBasketUniques(ByVal basketItem As Talent.Common.DEBasketItem, ByVal basketUniques As List(Of Talent.Common.DEBasketItem)) As Boolean
        For Each basketUnique In basketUniques
            If basketUnique.Product = basketItem.Product AndAlso basketUnique.LOGINID = basketItem.LOGINID Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Final checks before proceed to checkout
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function isCheckoutValid() As Boolean
        'Part refund not allowed if original sale was paid for using credit finance, but may cancel fully
        If Profile.Basket.CAT_MODE <> GlobalConstants.CATMODE_CANCEL Then
            If Profile.Basket.BasketSummary.TotalBasket < 0 AndAlso Profile.Basket.ORIGINAL_SALE_PAID_WITH_CF Then
                Return False
            End If
        End If
        Return True
    End Function

    Private Sub ProcessCheckOut()
        Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
        StandardBasketDetails.Error_List.Items.Clear()
        TEBUtilities.CheckBasketFreeItemHasOptions()
        Select Case Profile.Basket.BasketContentType
            Case GlobalConstants.COMBINEDBASKETCONTENTTYPE
                If ModuleDefaults.AllowCheckoutWithMixedBasket Then
                    If Me.PerformMerchandiseCheckoutClick Then
                        Me.PerformTicketingCheckoutClick(False)
                        If Profile.IsAnonymous Then
                            If AgentProfile.IsAgent Then
                                Response.Redirect("~/PagesPublic/Profile/CustomerSelection.aspx?returnurl=" + Server.UrlEncode("~/Redirect/TicketingGateway.aspx?page=basket.aspx&function=retailbasketcheckout"))
                            Else
                                Response.Redirect("~/PagesPublic/Login/login.aspx?ReturnUrl=~/Redirect/TicketingGateway.aspx?page=basket.aspx&function=retailbasketcheckout")
                            End If
                        Else
                            ticketingGatewayFunctions.Basket_Checkout_Retail(HttpContext.Current.Profile)
                            Response.Redirect("~/PagesLogin/Checkout/Checkout.aspx")
                        End If
                    End If
                Else
                    StandardBasketDetails.Error_List.Items.Add(StandardBasketDetails.MixedBasketNotAllowed)
                End If
            Case GlobalConstants.MERCHANDISEBASKETCONTENTTYPE
                If Me.PerformMerchandiseCheckoutClick Then
                    If Profile.IsAnonymous Then
                        If AgentProfile.IsAgent Then
                            Response.Redirect("~/PagesPublic/Profile/CustomerSelection.aspx?returnurl=" + Server.UrlEncode("~/Redirect/TicketingGateway.aspx?page=basket.aspx&function=retailbasketcheckout"))
                        Else
                            Response.Redirect("~/PagesPublic/Login/login.aspx?ReturnUrl=~/Redirect/TicketingGateway.aspx?page=basket.aspx&function=retailbasketcheckout")
                        End If
                    Else
                        ticketingGatewayFunctions.Basket_Checkout_Retail(HttpContext.Current.Profile)
                        Response.Redirect("~/PagesLogin/Checkout/Checkout.aspx")
                    End If
                End If
            Case GlobalConstants.TICKETINGBASKETCONTENTTYPE
                Me.PerformTicketingCheckoutClick()
            Case Else
                StandardBasketDetails.Error_List.Items.Add(StandardBasketDetails.NoItemsInBasketErrorText)
        End Select
    End Sub

    Private Sub ShowButtonsByBasketContentType()
        Select Case Profile.Basket.BasketContentType
            Case "C"
                OtherMatchesButton.Visible = True
                AddLinkRegButton.Visible = ModuleDefaults.FriendsAndFamily
                plhOrderTemplates.Visible = (ModuleDefaults.OrderTemplates AndAlso Not TEBUtilities.IsPartnerHomeDeliveryType(Profile))
                If Profile.IsAnonymous Then
                    plhSaveOrders.Visible = False
                Else
                    plhSaveOrders.Visible = plhSaveOrders.Visible = (ModuleDefaults.SaveOrders AndAlso Not TEBUtilities.IsPartnerHomeDeliveryType(Profile))
                End If
                AddAnotherFixtureButton.Visible = False

            Case "T"
                OtherMatchesButton.Visible = True
                AddLinkRegButton.Visible = ModuleDefaults.FriendsAndFamily
                plhOrderTemplates.Visible = False
                plhSaveOrders.Visible = False
                ContinueShoppingButton.Visible = False
                Dim componentID As Int64
                For Each tbi As DEBasketItem In Profile.Basket.BasketItems
                    If tbi.PACKAGE_ID > 0 Then
                        componentID = getComponentID(tbi.Product, tbi.PACKAGE_ID)
                    End If
                Next
                If componentID > 0 Then
                    AddAnotherFixtureButton.Visible = True
                Else
                    AddAnotherFixtureButton.Visible = False
                End If

            Case "M"
                OtherMatchesButton.Visible = False
                AddLinkRegButton.Visible = False
                Dim currentBaseketIsHomeDelivery As Boolean = False
                For Each item As TalentBasketItem In Profile.Basket.BasketItems
                    If item.PRODUCT_SUB_TYPE IsNot Nothing Then
                        If item.PRODUCT_SUB_TYPE = "HOMEDEL" Then currentBaseketIsHomeDelivery = True
                    End If
                Next
                If plhOrderTemplates.Visible AndAlso currentBaseketIsHomeDelivery Then plhOrderTemplates.Visible = False
                If plhSaveOrders.Visible AndAlso currentBaseketIsHomeDelivery Then plhSaveOrders.Visible = False
                AddAnotherFixtureButton.Visible = False

            Case Else
                OtherMatchesButton.Visible = False
                Me.Visible = False

        End Select
    End Sub

    Private Sub ShowButtonsByCATMode()
        Select Case Profile.Basket.CAT_MODE
            Case GlobalConstants.CATMODE_CANCEL
                OtherMatchesButton.Visible = False
                AddLinkRegButton.Visible = False
                ContinueShoppingButton.Visible = False
                plhSaveOrders.Visible = False
                plhOrderTemplates.Visible = False
                UpdateBasketButton.Visible = False
                ClearBasketButton.Visible = True
                CheckoutButton.Visible = True
                ReserveAllButton.Visible = False
                AddAnotherFixtureButton.Visible = False
            Case GlobalConstants.CATMODE_AMEND
                OtherMatchesButton.Visible = False
                AddLinkRegButton.Visible = False
                ContinueShoppingButton.Visible = False
                plhSaveOrders.Visible = False
                plhOrderTemplates.Visible = False
                UpdateBasketButton.Visible = True
                ClearBasketButton.Visible = True
                CheckoutButton.Visible = True
                ReserveAllButton.Visible = False
                AddAnotherFixtureButton.Visible = False
            Case GlobalConstants.CATMODE_TRANSFER
                OtherMatchesButton.Visible = False
                AddLinkRegButton.Visible = True
                ContinueShoppingButton.Visible = False
                plhSaveOrders.Visible = False
                plhOrderTemplates.Visible = False
                UpdateBasketButton.Visible = True
                ClearBasketButton.Visible = True
                CheckoutButton.Visible = True
                ReserveAllButton.Visible = False
                AddAnotherFixtureButton.Visible = False
        End Select
    End Sub

    Private Sub SetupDeliveryDetails()
        ' If not displaying the delivery address form but are an Agent and have tickets that are PrintNow then
        ' then always ensure that printing details are passed to the back-end via Session("DeliveryDetails") 
        If AgentProfile.IsAgent Then
            If containsPrint() Then
                Dim deliveryDetails As New Talent.Common.DEDeliveryDetails
                deliveryDetails.PrintOption = SetPrintOption(AgentProfile.PrintAddressLabelDefault, AgentProfile.PrintTransactionReceiptDefault)
                If deliveryDetails.PrintOption.Trim = "2" Or deliveryDetails.PrintOption.Trim = "4" Then
                    Try
                        deliveryDetails.ContactName = Profile.User.Details.Full_Name
                        Dim enumer As Collections.Generic.Dictionary(Of String, TalentProfileAddress).KeyCollection.Enumerator
                        enumer = Profile.User.Addresses.Keys.GetEnumerator
                        Dim userAddress As TalentProfileAddress
                        While enumer.MoveNext
                            userAddress = Profile.User.Addresses(enumer.Current)
                            If String.IsNullOrEmpty(userAddress.Address_Line_1.Trim) Then
                                deliveryDetails.Address1 = userAddress.Address_Line_2
                                deliveryDetails.Address2 = userAddress.Address_Line_3
                                deliveryDetails.Address3 = userAddress.Address_Line_4
                                deliveryDetails.Address4 = userAddress.Address_Line_5
                            Else
                                deliveryDetails.Address1 = userAddress.Address_Line_1
                                If userAddress.Address_Line_2.Trim = "" Then
                                    deliveryDetails.Address2 = userAddress.Address_Line_3
                                Else
                                    If userAddress.Address_Line_3.Trim = "" Then
                                        deliveryDetails.Address2 = userAddress.Address_Line_2
                                    Else
                                        deliveryDetails.Address2 = userAddress.Address_Line_2 + ", " + userAddress.Address_Line_3
                                    End If
                                End If
                                deliveryDetails.Address3 = userAddress.Address_Line_4
                                deliveryDetails.Address4 = userAddress.Address_Line_5
                            End If
                            deliveryDetails.Country = userAddress.Country
                            deliveryDetails.Postcode = userAddress.Post_Code
                            Exit While
                        End While
                    Catch ex As Exception
                        ' no address
                    End Try

                End If
                Session("DeliveryDetails") = deliveryDetails
            End If
        End If
    End Sub

    Private Sub updateTicketingBasket()
        If TicketingBasketDetails.Event_Repeater.Items.Count > 0 Then
            Dim redirectPath As String = "~/Redirect/TicketingGateway.aspx"
            Dim redirectQuery As String = TicketingBasketDetails.GetUpdatedItemsQueryString(_exceededBulkSalesBasketLimit)
            If Not String.IsNullOrEmpty(redirectQuery) AndAlso Not _exceededBulkSalesBasketLimit Then
                Session("RedirectUpdatebasket") = redirectQuery
                Response.Redirect(redirectPath & "?page=Basket.aspx&function=UpdateBasket")
            End If
        End If
    End Sub

    Private Sub ReservationSetup()
        If AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanCreateReservationFromBasket And Not Profile.IsAnonymous Then
            ReserveAllButton.Visible = True
            'If hasBasketReservedItems() AndAlso AgentProfile.AgentPermissions.CanReserveHospitalityBookings Then
            If hasBasketReservedItems() Then
                UnReserveAllButton.Visible = True
                UnReserveAllButton.Text = _ucr.Content("UnReserveAllButtonText", _languageCode, True)
            End If
            If hasBasketCorporatePackage() Then
                hdfIsReserveButtonEnabled.Value = False
                hdfCantReserveBasketText.Value = _ucr.Content("CantReserveBasketText", _languageCode, True)
                If basketHospitalityProductCount() > 1 Then
                    UnReserveAllButton.CssClass = UnReserveAllButton.CssClass & " ebiz-muted-action"
                    UnReserveAllButton.Attributes.Item("title") = _ucr.Content("CantUnreserveBasketText", _languageCode, True)
                    UnReserveAllButton.Enabled = False
                End If
            End If
            ReserveAllButton.Text = _ucr.Content("ReserveAllButtonText", _languageCode, True)
        End If

    End Sub
#End Region

#Region "Public Function"
    Public Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        If Not Page.IsPostBack Then
            With _ucr
                Dim obj As Object = sender
                Select Case obj.ID.ToString
                    Case Is = "SaveTemplateButton" : CType(obj, Button).Text = .Content("TemplateButtonText", _languageCode, True)
                    Case Is = "SaveOrderButton" : CType(obj, Button).Text = .Content("SaveOrderButtonText", _languageCode, True)
                    Case Is = "UpdateBasketButton" : CType(obj, Button).Text = .Content("UpdateBasketButtonText", _languageCode, True)
                    Case Is = "ClearBasketButton" : CType(obj, Button).Text = .Content("ClearBasketButtonText", _languageCode, True)
                    Case Is = "CheckoutButton" : CType(obj, Button).Text = .Content("CheckoutButtonText", _languageCode, True)
                    Case Is = "ContinueShoppingButton" : CType(obj, Button).Text = .Content("ContinueShoppingButtonText", _languageCode, True)
                    Case Is = "AddAnotherFixtureButton" : CType(obj, Button).Text = .Content("AddAnotherFixtureButtonText", _languageCode, True)
                    Case Is = "FastCashButton" : CType(obj, Button).Text = .Content("FastCashButtonText", _languageCode, True)
                    Case Is = "FastCardButton" : CType(obj, Button).Text = .Content("FastCardButtonText", _languageCode, True)
                End Select
            End With
        End If
    End Sub

#End Region

#Region "Private Functions"
    Private Function hasBasketReservedItems() As Boolean
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            If item.RESERVED_SEAT = "Y" Then
                Return True
            End If
        Next
        Return False
    End Function
    Private Function PerformMerchandiseCheckoutClick() As Boolean
        Dim success As Boolean = True
        If ModuleDefaults.Perform_Front_End_Stock_Check _
            AndAlso Not StandardBasketDetails.ProductsInStock() _
                AndAlso Not ModuleDefaults.AllowCheckoutWhenNoStock Then
            success = False
            StandardBasketDetails.Error_List.Items.Add(StandardBasketDetails.InsufficientStockErrorText)
        End If
        If Not StandardBasketDetails.IsValidCostCentre Then success = False
        If Not StandardBasketDetails.IsValidAccountNo Then success = False
        Return success
    End Function

    Private Function updateRetailBasket() As Boolean
        Dim isErrorInRetail As Boolean = False
        If StandardBasketDetails.Basket_Repeater.Items.Count > 0 Then
            StandardBasketDetails.Error_List.Items.Clear()
            Dim basketDetail As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
            Dim productCode As Label
            Dim price As Label
            Dim itemErrorLabel As Label
            Dim quantity As TextBox
            Dim isFree As CheckBox
            Dim AllInStock As Boolean = True
            Dim NoStockItemIndex As New ArrayList
            Dim costCentre As New TextBox
            Dim accountNo As New TextBox

            'Ensure cost centre and account number is valid
            If Not StandardBasketDetails.IsValidCostCentre Then Return isErrorInRetail
            If Not StandardBasketDetails.IsValidAccountNo Then Return isErrorInRetail

            'Loop through all items
            For Each row As RepeaterItem In StandardBasketDetails.Basket_Repeater.Items
                productCode = CType(row.FindControl("ProductCodeLabel"), Label)
                price = CType(row.FindControl("UnitLabel"), Label)
                itemErrorLabel = CType(row.FindControl("ItemErrorLabel"), Label)
                quantity = CType(row.FindControl("QuantityBox"), TextBox)
                isFree = CType(row.FindControl("isFree"), CheckBox)
                costCentre = CType(row.FindControl("CostCentreTextBox"), TextBox)
                accountNo = CType(row.FindControl("AccountNoTextBox"), TextBox)

                'Ensure quantity entered is valid
                If Not StandardBasketDetails.IsValidQuantity(quantity.Text) Then Return isErrorInRetail

                Dim inStock As Boolean = True
                If ModuleDefaults.Perform_Front_End_Stock_Check Then
                    inStock = StandardBasketDetails.ProductInStock(productCode.Text, CDec(quantity.Text))
                End If
                'If there is stock for the item, add it
                If inStock OrElse ModuleDefaults.AllowCheckoutWhenNoStock Then
                    If Not isFree.Checked Then
                        Dim gp As Decimal = 0
                        Dim np As Decimal = 0
                        Dim tp As Decimal = 0

                        Select Case ModuleDefaults.PricingType
                            Case 2
                                Dim prices As Data.DataTable = TEBUtilities.GetChorusPrice(productCode.Text, CDec(quantity.Text))
                                If prices.Rows.Count > 0 Then
                                    gp = TEBUtilities.CheckForDBNull_Decimal(prices.Rows(0)("GrossPrice"))
                                    np = TEBUtilities.CheckForDBNull_Decimal(prices.Rows(0)("NetPrice"))
                                    tp = TEBUtilities.CheckForDBNull_Decimal(prices.Rows(0)("TaxPrice"))
                                End If
                            Case Else
                                Dim masterCode As String = ""
                                For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                                    If UCase(tbi.Product) = UCase(productCode.Text) Then
                                        masterCode = tbi.MASTER_PRODUCT
                                        Exit For
                                    End If
                                Next
                                Dim webPrice As Decimal = 0
                                Try
                                    webPrice = TEBUtilities.GetWebPrices(productCode.Text, CDec(quantity.Text), masterCode).Purchase_Price_Gross()
                                Catch ex As Exception
                                End Try
                                Dim deWp As Talent.Common.DEWebPrice = TEBUtilities.GetWebPrices(productCode.Text, CDec(quantity.Text), masterCode)
                                gp = deWp.Purchase_Price_Gross
                                np = deWp.Purchase_Price_Net
                                tp = deWp.Purchase_Price_Tax
                        End Select
                        basketDetail.Update_Basket_Item(CDec(quantity.Text), False, 0, gp, np, tp, costCentre.Text, accountNo.Text, "", "", "", "", "", "", StandardBasketDetails.GetBasketID, productCode.Text)
                    End If
                End If
                If Not inStock Then
                    NoStockItemIndex.Add(row.ItemIndex)
                    AllInStock = False
                End If
            Next

            If ModuleDefaults.Perform_Front_End_Stock_Check Then
                StandardBasketDetails.BindBasketRepeater()
                Dim talBasketProcessor As New TalentBasketProcessor
                talBasketProcessor.Settings = TEBUtilities.GetSettingsObject()
                Dim errObj As ErrorObj = talBasketProcessor.ProcessSummaryForUpdatedBasket(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
                Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, Not Profile.IsAnonymous)
                StandardBasketDetails.Summary_Totals1.SetUpTotals()
                'If not all items are instock display the error
                If Not AllInStock Then
                    StandardBasketDetails.Error_List.Items.Add(_ucr.Content("UpdateFail_InsufficientStock", _languageCode, True))
                    For i As Integer = 0 To NoStockItemIndex.Count - 1
                        'If not flag the error
                        itemErrorLabel = CType(StandardBasketDetails.Basket_Repeater.Items(NoStockItemIndex(i)).FindControl("ItemErrorLabel"), Label)
                        itemErrorLabel.Text = "**"
                        itemErrorLabel.Visible = True
                    Next
                    isErrorInRetail = True
                End If
            Else
                isErrorInRetail = False
            End If
        End If
        Return isErrorInRetail
    End Function

    Private Function isFastCashButtonVisible() As Boolean
        Dim buttonIsVisible As Boolean = False
        If AgentProfile.IsAgent Then
            If Talent.eCommerce.Utilities.BasketContentTypeWithOverride = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                buttonIsVisible = False
            Else
                If Not Profile.Basket.DoesBasketHavePPS() Then
                    For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                        If String.IsNullOrEmpty(tbi.STOCK_ERROR_CODE.Trim()) Then
                            If Not String.IsNullOrEmpty(TEBUtilities.CheckForDBNull_String(tbi.FULFIL_OPT_PRINT)) Then
                                If (Not tbi.CURR_FULFIL_SLCTN Is Nothing AndAlso tbi.CURR_FULFIL_SLCTN.Trim = "N") Then
                                    buttonIsVisible = True
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                End If
            End If
        End If
        Return buttonIsVisible
    End Function
    ''' <summary>
    ''' Does the basket contain the print option or is print always selected against the agent profile
    ''' </summary>
    ''' <returns>True if the basket contains the print option</returns>
    ''' <remarks></remarks>
    Private Function containsPrint() As Boolean
        Dim success As Boolean = False
        Dim printAlways As Boolean = AgentProfile.PrintAlways
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.CURR_FULFIL_SLCTN)) Then
                If (Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.FULFIL_OPT_PRINT))) AndAlso tbi.CURR_FULFIL_SLCTN.Trim = GlobalConstants.PRINT_FULFILMENT Then
                    success = True
                Else
                    If printAlways AndAlso
                        (
                            (Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.FULFIL_OPT_COLL)) AndAlso tbi.CURR_FULFIL_SLCTN.Trim = GlobalConstants.COLLECT_FULFILMENT) _
                            OrElse (Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.FULFIL_OPT_POST)) AndAlso tbi.CURR_FULFIL_SLCTN.Trim = GlobalConstants.POST_FULFILMENT) _
                            OrElse (Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.FULFIL_OPT_REGPOST)) AndAlso tbi.CURR_FULFIL_SLCTN.Trim = GlobalConstants.REG_POST_FULFILMENT)
                        ) Then
                        success = True

                    End If
                End If
                If success Then
                    Exit For
                End If
            End If
        Next
        Return success
    End Function

    Private Function SetPrintOption(ByVal boolPrintAddress As Boolean, ByVal boolPrintReceipt As Boolean) As String
        Dim printOption As String = ""
        Dim basketContainsTicketItems As Boolean = containsTicketItem()
        If basketContainsTicketItems Then
            If boolPrintAddress AndAlso boolPrintReceipt Then
                printOption = "4"
            ElseIf Not boolPrintAddress AndAlso boolPrintReceipt Then
                printOption = "3"
            ElseIf boolPrintAddress AndAlso Not boolPrintReceipt Then
                printOption = "2"
            Else
                printOption = "1"
            End If
        End If
        Return printOption
    End Function

    Private Function containsTicketItem() As Boolean
        Dim success As Boolean = False
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If (Not tbi.PRODUCT_TYPE Is Nothing AndAlso tbi.PRODUCT_TYPE = GlobalConstants.TICKETINGBASKETCONTENTTYPE) Then
                success = True
                Exit For
            End If
        Next
        Return success
    End Function
    ''' <summary>
    ''' Returns the availability component ID for the respective package
    ''' </summary>
    ''' <returns>Component id if the package has a availability component</returns>
    ''' <remarks></remarks>
    Private Function getComponentID(ByVal productCode As String, ByVal packageID As String) As Int64
        Dim product As New Talent.Common.TalentProduct
        Dim settings As Talent.Common.DESettings = TEBUtilities.GetSettingsObject()
        Dim err As New Talent.Common.ErrorObj
        Dim dtComponentList As New DataTable
        Dim componentID As Int64
        product.Settings() = settings
        product.De.ProductCode = productCode
        product.De.Src = GlobalConstants.SOURCE
        err = product.ProductHospitality
        If (Not err.HasError) AndAlso (product.ResultDataSet IsNot Nothing) Then
            dtComponentList = product.ResultDataSet.Tables(2)
        End If
        Dim drComponentList() As DataRow = dtComponentList.Select("PackageID=" & Talent.Common.Utilities.PadLeadingZeros(packageID, 13) & " and ComponentType = 'A'")
        If drComponentList.Length > 0 Then
            componentID = drComponentList(0)("ComponentID")
        Else
            componentID = 0
        End If
        Return componentID
    End Function

    ''' <summary>
    ''' Returns if the basket contains corporate package
    ''' </summary>
    ''' <returns>True if basket contains corporate package</returns>
    ''' <remarks></remarks>
    Private Function hasBasketCorporatePackage() As Boolean
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            If item.PACKAGE_ID > 0 Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Returns count if the basket contains more than one corporate package 
    ''' </summary>
    ''' <returns>Count which will decide whether to disable it or not</returns>
    ''' <remarks></remarks>
    Private Function basketHospitalityProductCount() As Integer
        Dim count As New Integer
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            If item.PACKAGE_ID > 0 Then
                count += 1
            End If
        Next
        Return count
    End Function

#End Region

End Class
