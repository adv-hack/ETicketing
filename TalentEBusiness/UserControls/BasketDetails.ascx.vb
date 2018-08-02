Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports System.Collections.Generic
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Data

Partial Class UserControls_BasketDetails
    Inherits ControlBase

#Region "Class Level Fields"

    Private _masterProductsPriceList As Generic.Dictionary(Of String, Talent.Common.DEWebPrice) = Nothing
    Private _talentWebPricing As Talent.Common.TalentWebPricing = Nothing
    Private _webPrices As Generic.Dictionary(Of String, Talent.Common.DEWebPrice) = Nothing
    Private _partPaymentsTotal As Decimal = 0
    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

#End Region

#Region "Constants"

    Const ONACCOUNTKEYCODE As String = "OnAccount.ascx"
    Const KEYCODE As String = "BasketDetails.ascx"

#End Region

#Region "Public Properties"

    Public Property Display() As Boolean
    Public Property DisplaySummaryTotals() As Boolean
    Public ReadOnly Property NoItemsInBasketErrorText() As String
        Get
            Return _ucr.Content("NoBasketItemsError", _languageCode, True)
        End Get
    End Property
    Public ReadOnly Property InsufficientStockErrorText() As String
        Get
            Return _ucr.Content("InsufficientStockError", _languageCode, True)
        End Get
    End Property
    Public ReadOnly Property MixedBasketNotAllowed() As String
        Get
            Return _ucr.Content("MixedBasketNotAllowedError", _languageCode, True)
        End Get
    End Property
    Public ReadOnly Property Basket_Repeater() As Repeater
        Get
            Return BasketRepeater
        End Get
    End Property
    Public ReadOnly Property Error_List() As BulletedList
        Get
            Return ErrorList
        End Get
    End Property
    Public ReadOnly Property Summary_Totals1() As UserControls_SummaryTotals
        Get
            Return SummaryTotals1
        End Get
    End Property
    Public Property CurrentTempOrderID() As String
    Public ErrMsg As Talent.Common.TalentErrorMessages
    Public Property Usage() As String
    Private Property BasketItems() As DataTable
    Public ReadOnly Property GetBasketID() As Integer
        Get
            Return Profile.Basket.Basket_Header_ID
        End Get
    End Property
    Public Property BulkSalesModeCssClass() As String = String.Empty

    Public ReadOnly Property ErrorListRepeater() As Repeater
        Get
            Return rptErrorList
        End Get
    End Property

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.Common.Utilities.GetAllString
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "BasketDetails.ascx"
        End With

        Me.CurrentTempOrderID = Profile.Basket.TempOrderID
        'Loop through the basket and see if any items are non-ticketing
        '---------------------------------------------------------------
        Me.Display = False
        Select Case Profile.Basket.BasketContentType
            Case "M"
                merchandisingBasketHeaderLabel.Text = _ucr.Content("merchandisingBasketHeaderLabel", _languageCode, True)
                Me.Display = True
                Me.Basket_HTMLInclude2.Visible = False
            Case "C"
                merchandisingBasketHeaderLabel.Text = _ucr.Content("merchandisingBasketHeaderLabel", _languageCode, True)
                Me.Display = True
                If Me.Usage.ToUpper = "ORDERSUMMARY" Then
                    Me.TicketingBasketDetails1.Usage = "ORDER"
                Else
                    Me.TicketingBasketDetails1.Usage = Me.Usage
                End If
            Case Is = "T"
                merchandisingBasketHeaderLabel.Visible = False
                Me.Display = True
                If Me.Usage.ToUpper = "ORDERSUMMARY" Then
                    Me.TicketingBasketDetails1.Usage = "ORDER"
                Else
                    Me.TicketingBasketDetails1.Usage = Me.Usage
                End If


        End Select
        '---------------------------------------------------------------

        ErrMsg = New Talent.Common.TalentErrorMessages(_languageCode, _
                                                        TalentCache.GetBusinessUnitGroup, _
                                                        TalentCache.GetPartner(Profile), _
                                                        ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
        If Me.Display Then
            ErrorList.Items.Clear()
            SummaryTotals1.Usage = Usage
            Select Case Usage.ToUpper
                Case "PAYMENT"
                    If Profile.Basket.BasketContentType <> "C" Then
                        SummaryTotals1.Visible = False
                    End If
                    BasketSummary1.Visible = False
                Case "ORDER"
                    BasketSummary1.RemovePartPaymentAllowed = False
                Case Else
                    BasketSummary1.RemovePartPaymentAllowed = True
            End Select
        Else
            ecommerceBasketWrapper.Visible = False
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Talent.eCommerce.Utilities.ShowPrices(Profile) Then
            DisplaySummaryTotals = False
        End If
        Try
            If Not Session("TalentErrorCode") Is Nothing AndAlso Not Usage.ToUpper = "ORDER" AndAlso Not Usage.ToUpper = "PAYMENT" Then
                Dim myError As String = CStr(Session("TalentErrorCode"))
                ErrorList.Items.Add(ErrMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, myError).ERROR_MESSAGE)
                Session("TalentErrorCode") = Nothing
                Session("TicketingGatewayError") = Nothing
            End If

            'get the external payment gateway error message
            If Not String.IsNullOrWhiteSpace(Session("GatewayErrorMessage")) Then
                If Not Utilities.GetCurrentPageName.ToLower = "checkout.aspx" Then
                    ErrorList.Items.Add(Session("GatewayErrorMessage").ToString())
                    Session.Remove("GatewayErrorMessage")
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Me.Display Then
            setPriceObjects()
            ' need to do everytime otherwise it stops displaying on checkout when you change addresss..
            If Not Page.IsPostBack OrElse Talent.eCommerce.Utilities.GetCurrentPageName().ToUpper = "CHECKOUT.ASPX" Then
                BindBasketRepeater()
            End If
            CheckAgeLimits()
            Dim showingStockError As Boolean = False
            Dim showingDiscontinuedError As Boolean = False
            If Profile.Basket.STOCK_ERROR Then
                For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                    If tbi.STOCK_ERROR Then
                        DisplayBasketItemsOutOfStock()
                        Select Case tbi.STOCK_ERROR_CODE
                            Case Is = "DISC"
                                If Not showingDiscontinuedError Then
                                    ErrorList.Items.Add(_ucr.Content("DiscontinuedProductError", _languageCode, True))
                                    showingDiscontinuedError = True
                                End If

                            Case Else
                                If Not showingStockError Then
                                    ErrorList.Items.Add(_ucr.Content("InsufficientStockError", _languageCode, True))
                                    showingStockError = True
                                End If
                        End Select

                    End If
                Next
            End If
            bindErrorListRepeater()

            ' Check for alternative items for products on stop
            If Not Session("AlternativeProducts") Is Nothing Then
                CheckAlternativeItems()
            End If
            If ModuleDefaults.Call_Tax_WebService Then
                If Not HttpContext.Current.Session.Item("DunhillWSError") Is Nothing Then
                    If CBool(HttpContext.Current.Session.Item("DunhillWSError")) Then
                        'Dunhill WS Error
                        ErrorList.Items.Add(_ucr.Content("WebServiceError", _languageCode, True))
                    Else
                        HttpContext.Current.Session.Item("DunhillWSError") = Nothing
                    End If
                End If
            End If
        End If

        If Not DisplaySummaryTotals Then
            SummaryTotals1.Visible = False
        Else
            Select Case Usage.ToUpper
                Case "PAYMENT"
                    If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                        SummaryTotals1.Visible = True
                    Else
                        SummaryTotals1.Visible = False
                    End If
                Case "BASKET"
                    If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                        SummaryTotals1.Visible = False
                    End If
            End Select
        End If

        CheckIfBasketEmpty()
        If Me.Display AndAlso Usage.ToUpper = "ORDER" Then SummaryTotals1.ReBindSummaryTotalsOnConfirmation(Me.CurrentTempOrderID)
        If Me.Display AndAlso Usage.ToUpper = "ORDER" Then Me.ErrorList.Visible = False
        If (Session("ReservationSuccessful") IsNot Nothing AndAlso Session("ReservationSuccessful") = "True") Then
            Session.Remove("ReservationSuccessful")
            blSuccessList.Items.Add(_ucr.Content("ReservationSuccessful", _languageCode, True))
        End If
        tidyUpBasket()
    End Sub

    Protected Sub BasketRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles BasketRepeater.ItemDataBound
        If Not e.Item.ItemIndex = -1 Then
            Dim item As DataRow = CType(CType(e.Item.DataItem, Data.DataRowView).Row, Data.DataRow)
            Dim ri As RepeaterItem = e.Item
            Dim box As TextBox = CType(ri.FindControl("QuantityBox"), TextBox)
            Dim deleteLink As Button = CType(ri.FindControl("DeleteButton"), Button)
            Dim unitLbl As Label = CType(ri.FindControl("UnitLabel"), Label)
            Dim valLbl As Label = CType(ri.FindControl("ValueLabel"), Label)
            Dim qtyLbl As Label = CType(ri.FindControl("QuantityLabel"), Label)
            Dim reg As RegularExpressionValidator = CType(ri.FindControl("QuantityRegEx"), RegularExpressionValidator)
            Dim costCentreLbl As Label = CType(ri.FindControl("CostCentreLabel"), Label)
            Dim accountNoLbl As Label = CType(ri.FindControl("AccountNoLabel"), Label)
            Dim costCentreBox As TextBox = CType(ri.FindControl("CostCentreTextBox"), TextBox)
            Dim accountNoBox As TextBox = CType(ri.FindControl("AccountNoTextBox"), TextBox)
            Dim productCodeHidden As Label = CType(ri.FindControl("ProductCodeLabel"), Label)
            Dim basketDetailIdHidden As Label = CType(ri.FindControl("BasketDetailIdLabel"), Label)
            Dim xmlConfigHidden As Label = CType(ri.FindControl("XmlConfigLabel"), Label)
            Dim productMasterHidden As Label = CType(ri.FindControl("ProductMasterLabel"), Label)
            Dim stocklbl As Label = CType(ri.FindControl("StockLabel"), Label)
            Dim StockIssueLabel As Label = CType(ri.FindControl("StockIssueLabel"), Label)
            Dim ProductDiscontinuedLabel As Label = CType(ri.FindControl("ProductDiscontinuedLabel"), Label)
            Dim colourlbl As Label = CType(ri.FindControl("ColourLabel"), Label)
            Dim sizelbl As Label = CType(ri.FindControl("SizeLabel"), Label)
            Dim productLink As HyperLink = CType(ri.FindControl("ProductHyperlink"), HyperLink)
            Dim productDesc As Label = CType(ri.FindControl("ProductDescription"), Label)
            Dim productCodeVisible As Label = CType(ri.FindControl("ProductCodeLabelVisible"), Label)
            Dim personalisationButton As Button = CType(ri.FindControl("PersonalisationButton"), Button)
            Dim transactionsLabel As Label = CType(ri.FindControl("TransactionLabel"), Label)
            Dim img As Image = CType(ri.FindControl("ProductImage"), Image)
            If Not Utilities.ShowPrices(Profile) Then
                unitLbl.Visible = False
                valLbl.Visible = False
            End If

            If String.IsNullOrEmpty(item("MASTER_PRODUCT").ToString().Trim) Then
                img.ImageUrl = GetImageURL(item("PRODUCT"))
            Else
                img.ImageUrl = GetImageURL(item("MASTER_PRODUCT"))
            End If

            Dim products As Data.DataTable = GetProductInfo(item("PRODUCT"))
            Dim prodCode As String = item("PRODUCT")
            Dim trimLength As Integer = CType(_ucr.Attribute("productCodeLengthTrim"), Integer)
            If trimLength > 0 AndAlso prodCode.Length >= trimLength Then
                productCodeVisible.Text = prodCode.Substring(0, trimLength)
            Else
                productCodeVisible.Text = prodCode
            End If

            Dim personalisedProduct As Boolean = False
            Dim personalisedSubProduct As Boolean = False
            personalisationButton.Text = _ucr.Content("btnPersonalisationText", _languageCode, True)
            If item("XML_CONFIG") = "1" Then
                personalisedProduct = True
                personalisationButton.Visible = True
            ElseIf item("XML_CONFIG") = "2" Then
                personalisedSubProduct = True
                deleteLink.Visible = False
            End If
            If products.Rows.Count > 0 AndAlso (IsPriceListExists(item("MASTER_PRODUCT"), item("PRODUCT"), item("IS_FREE"))) Then
                img.AlternateText = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_1"))
                productLink.Text = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_1"))
                productDesc.Text = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_2"))
                costCentreBox.Text = Utilities.CheckForDBNull_String(item("COST_CENTRE"))
                costCentreLbl.Text = Utilities.CheckForDBNull_String(item("COST_CENTRE"))
                accountNoBox.Text = Utilities.CheckForDBNull_String(item("ACCOUNT_CODE"))
                accountNoLbl.Text = Utilities.CheckForDBNull_String(item("ACCOUNT_CODE"))

                If Not Session("personalisationTransactions") Is Nothing Then

                    Dim tbl As Data.DataTable
                    tbl = Session("personalisationTransactions")
                    Dim aUniqueTrans As ArrayList = New ArrayList
                    For Each row As Data.DataRow In tbl.Rows
                        Dim sTrans As String = row.Item("TRANSACTION_ID")
                        If aUniqueTrans.IndexOf(sTrans) = -1 Then
                            aUniqueTrans.Add(sTrans)
                        End If
                    Next
                    Dim personalisationsCount As Integer = 0
                    transactionsLabel.Text += "<div class='personalisations'>"
                    For Each transId As String In aUniqueTrans
                        Dim headerCount As Integer = 0
                        For Each row As Data.DataRow In tbl.Rows
                            If row.Item("PRODUCT_CODE") = products.Rows(0)("PRODUCT_CODE") And _
                            row.Item("TRANSACTION_ID") = transId Then
                                'We are in a transaction for this product
                                If Utilities.CheckForDBNull_Boolean_DefaultFalse(products.Rows(0)("PERSONALISABLE")) Then
                                    'Lock out QTY Control
                                    box.Enabled = False
                                End If
                                If headerCount = 0 Then
                                    transactionsLabel.Text += "<div class='personalisation'>" & row.Item("DISPLAY_IN_BASKET")
                                    If Usage.ToUpper = "BASKET" Then
                                        personalisationsCount += 1
                                        transactionsLabel.Text += " <a href='../../PagesPublic/ProductBrowse/Personalise.aspx?product=" & products.Rows(0)("PRODUCT_CODE") & "&transactionId=" & row.Item("TRANSACTION_ID") & "&master=" & productMasterHidden.Text & "'>edit</a>"
                                        transactionsLabel.Text += " <a href='../../Redirect/Launcher.aspx?function=removetransaction&transactionId=" & row.Item("TRANSACTION_ID") & "'>delete</a>"
                                        deleteLink.Visible = False
                                    End If
                                End If
                                headerCount += 1
                            End If
                        Next
                        transactionsLabel.Text += "</div>"
                    Next
                    transactionsLabel.Text += "</div>"
                    If personalisationsCount = CType(qtyLbl.Text, Integer) Then personalisationButton.Text += " Another"
                Else
                    'Session doesn't exist or has been lost, remove any lines with XML_CONFIG = "2"
                    Dim tbisToRemove As ArrayList = New ArrayList
                    Dim tbisToRemoveCount As Integer = 0
                    For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                        If tbi.Xml_Config = "2" Then
                            tbisToRemove.Add(tbi)
                            tbisToRemoveCount += 1
                        End If
                    Next
                    For Each tbi As TalentBasketItem In tbisToRemove
                        Profile.Basket.BasketItems.Remove(tbi)
                        Profile.Basket.IsDirty = True
                        Dim basketAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
                        basketAdapter.Delete_Basket_Item(Profile.Basket.Basket_Header_ID, tbi.Product)
                    Next
                    If tbisToRemoveCount > 0 Then Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                End If
            Else
                Dim basketAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
                basketAdapter.Delete_Basket_Item(item("BASKET_HEADER_ID"), item("PRODUCT"))
                Profile.Save()
                Session("TalentErrorCode") = "UCBADE-1"
                Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            End If

            productCodeHidden.Text = item("PRODUCT")
            basketDetailIdHidden.Text = item("BASKET_DETAIL_ID")
            If item("IS_FREE") Then
                box.Visible = False
                qtyLbl.Visible = True
                reg.Visible = False
                reg.Enabled = False
                deleteLink.Visible = False
                unitLbl.Text = TDataObjects.PaymentSettings.FormatCurrency(0, _ucr.BusinessUnit, _ucr.PartnerCode)
                valLbl.Text = TDataObjects.PaymentSettings.FormatCurrency(0, _ucr.BusinessUnit, _ucr.PartnerCode)
            Else
                If personalisedSubProduct = True Or personalisedProduct = True Then
                    box.Enabled = False
                End If

                Dim uPrice As Decimal = 0
                Dim vPrice As Decimal = 0
                Dim roundUp As Boolean = False

                Select Case ModuleDefaults.PricingType
                    Case 2
                        If ModuleDefaults.ShowPricesExVAT Then
                            uPrice = item("NET_PRICE")
                        Else
                            uPrice = item("GROSS_PRICE")
                        End If

                    Case Else
                        'check the MASTER_PRODUCT quantities for multibuys
                        Dim masterQty As Decimal = item("QUANTITY")
                        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                            If UCase(tbi.Product) <> UCase(item("PRODUCT")) Then
                                If Not tbi.MASTER_PRODUCT Is Nothing _
                                    AndAlso Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(item("MASTER_PRODUCT"))) Then
                                    If UCase(tbi.MASTER_PRODUCT) = UCase(Utilities.CheckForDBNull_String(item("MASTER_PRODUCT"))) Then
                                        masterQty += tbi.Quantity
                                    End If
                                End If
                            End If
                        Next

                        If masterQty >= item("QUANTITY") Then
                            'Check to see if the multibuys are configured for this master product
                            If String.IsNullOrEmpty(item("MASTER_PRODUCT")) Then
                                Dim productPriceList As Talent.Common.DEWebPrice = Nothing

                                If (_masterProductsPriceList.TryGetValue(item("MASTER_PRODUCT"), productPriceList)) Then

                                    If productPriceList.SALE_PRICE_BREAK_QUANTITY_1 > 0 OrElse productPriceList.PRICE_BREAK_QUANTITY_1 > 0 Then
                                        'multibuys are configured
                                        uPrice = _webPrices(item("MASTER_PRODUCT")).DisplayPrice
                                    Else
                                        uPrice = _webPrices(item("PRODUCT")).DisplayPrice
                                    End If
                                Else
                                    uPrice = _webPrices(item("PRODUCT")).DisplayPrice
                                End If
                            Else
                                uPrice = _webPrices(item("PRODUCT")).DisplayPrice
                            End If
                        Else
                            uPrice = _webPrices(item("PRODUCT")).DisplayPrice
                        End If
                End Select

                vPrice = uPrice * item("QUANTITY")
                unitLbl.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(uPrice, 0.01, roundUp), _ucr.BusinessUnit, _ucr.PartnerCode)
                valLbl.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(vPrice, 0.01, roundUp), _ucr.BusinessUnit, _ucr.PartnerCode)

                Dim stockTime As String = String.Empty
                If products.Rows.Count > 0 Then
                    Dim product As Data.DataRow = products.Rows(0)
                    Dim stockVal As Decimal = Stock.GetStockBalance(productCodeHidden.Text)
                    If stockVal > 0 Then
                        stocklbl.Text = _ucr.Content("InStockText", _languageCode, True)
                    Else
                        stocklbl.Text = Stock.GetNoStockDescription(productCodeHidden.Text, stockTime)
                    End If
                    If CType(_ucr.Attribute("DisplayStockIssueLabel"), Boolean) And ModuleDefaults.Perform_Front_End_Stock_Check Then
                        If stockVal < item("QUANTITY") Then
                            StockIssueLabel.Text = _ucr.Content("StockIssueText", _languageCode, True).Replace("<<stock>>", stockVal.ToString("########0"))
                            StockIssueLabel.Visible = True
                        End If
                    End If
                    colourlbl.Text = Utilities.CheckForDBNull_String(product("PRODUCT_COLOUR"))
                    If (ModuleDefaults.Perform_Back_End_Stock_Check And Not ModuleDefaults.Perform_Front_End_Stock_Check) Then
                        If CType(_ucr.Attribute("DisplayStockIssueLabel"), Boolean) Then
                            For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                                If tbi.Product = item("PRODUCT") And tbi.STOCK_ERROR Then
                                    StockIssueLabel.Text = _ucr.Content("StockIssueText", _languageCode, True).Replace("<<stock>>", tbi.QUANTITY_AVAILABLE.ToString("########0"))
                                    StockIssueLabel.Visible = True

                                End If
                            Next
                        End If
                    End If

                End If

            End If

            Dim altProd As UserControls_AlternativeProducts = CType(ri.FindControl("AlternativeProducts"), UserControls_AlternativeProducts)
            If ModuleDefaults.RetrieveAlternativeProductsAtCheckout Then
                altProd.Display = True
                altProd.Visible = True
                If Not altProd Is Nothing Then
                    Select Case Profile.Basket.BasketContentType
                        Case "C", "M"

                            If Usage.ToUpper = "BASKET" Then
                                ' Set alternative products 
                                altProd.ProductCode = prodCode
                                If Not Session("AlternativeProducts") Is Nothing Then
                                    altProd.AltProducts = Session("AlternativeProducts")
                                End If
                                ' If item has alt products then qty box is not visible
                                Dim ds As Data.DataSet
                                If Not Session("AlternativeProducts") Is Nothing Then
                                    ds = CType(Session("AlternativeProducts"), Data.DataSet)
                                    Dim dt As Data.DataTable = ds.Tables("ALTPRODUCTRESULTS")
                                    Dim drc As Data.DataRow() = dt.Select("ProductCode = '" & prodCode.Trim & "'")
                                    If drc.Length > 1 Then
                                        box.Enabled = False
                                    End If
                                End If
                            Else
                                altProd.Display = False
                            End If
                        Case Else
                            altProd.Display = False
                    End Select
                End If
            Else
                altProd.Display = False
                altProd.Visible = False
            End If
            Dim plhAlternativeProduct As PlaceHolder = CType(e.Item.FindControl("plhAlternativeProduct"), PlaceHolder)
            If plhAlternativeProduct IsNot Nothing Then plhAlternativeProduct.Visible = altProd.Visible

            Dim plhTransactionLabel As PlaceHolder = CType(e.Item.FindControl("plhTransactionLabel"), PlaceHolder)
            If plhTransactionLabel IsNot Nothing Then
                plhTransactionLabel.Visible = (transactionsLabel.Text.Length > 0)
            End If

            Dim hplComments As HtmlAnchor = CType(e.Item.FindControl("hplComments"), HtmlAnchor)
            setCommentsLink(e, hplComments, item("PRODUCT"))
        End If
    End Sub

    Protected Sub SetVisibility(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Display Then
            'If Not Page.IsPostBack Then
            With _ucr
                Select Case UCase((sender.ID).ToString)
                    'CONTROLS
                    Case Is = "BUTTONSPANEL"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = True
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = False
                        End Select
                    Case Is = "PRODUCTDESCRIPTION"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = False
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = True
                        End Select
                    Case Is = "QUANTITYBOX", "COSTCENTRETEXTBOX", "ACCOUNTNOTEXTBOX"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = True
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = False
                        End Select
                    Case Is = "QUANTITYLABEL", "COSTCENTRELABEL", "ACCOUNTNOLABEL"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = False
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = True
                        End Select
                    Case Is = "QUANTITYREGEX"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = True
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = False
                                sender.Enabled = False
                        End Select

                        'COLUMNS
                    Case Is = "IMAGECOLUMN"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(.Attribute("DisplayProductImage_Basket"))
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = True
                        End Select
                    Case Is = "UPDATECOLUMN"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = True
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = False
                        End Select

                    Case Is = "PRODUCTCODECOLUMN"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayProductCode_Basket"))
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayProductCode_OrderSummary"))
                        End Select

                    Case Is = "PRODUCTNAMECOLUMN"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayProductName_Basket"))
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayProductName_OrderSummary"))
                        End Select

                    Case Is = "COLOURCOLUMN"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayColour_Basket"))
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayColour_OrderSummary"))
                        End Select

                    Case Is = "SIZECOLUMN"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplaySize_Basket"))
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplaySize_OrderSummary"))
                        End Select

                    Case Is = "QUANTITYCOLUMN"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayQuantity_Basket"))
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayQuantity_OrderSummary"))
                        End Select

                    Case Is = "VALUECOLUMN", "UNITPRICECOLUMN"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayValue_Basket"))
                                If ((sender.Visible) AndAlso (Profile.PartnerInfo.Details IsNot Nothing)) Then
                                    sender.Visible = Not Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(Profile.PartnerInfo.Details.HIDE_PRICES)
                                End If
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayValue_OrderSummary"))
                                If ((sender.Visible) AndAlso (Profile.PartnerInfo.Details IsNot Nothing)) Then
                                    sender.Visible = Not Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(Profile.PartnerInfo.Details.HIDE_PRICES)
                                End If
                        End Select
                    Case Is = "STOCKCOLUMN"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayStock_Basket"))
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayStock_OrderSummary"))
                        End Select

                    Case Is = "COSTCENTRECOLUMN"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayCostCentre_Basket"))
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayCostCentre_OrderSummary"))
                        End Select
                        '
                        ' Only display the cost Centre and account fields when the field is present in the profile.
                        '
                        If Profile.IsAnonymous Or Profile.PartnerInfo.Details Is Nothing Then
                            sender.Visible = False
                        End If
                        If Not Profile.PartnerInfo.Details Is Nothing Then
                            If Profile.PartnerInfo.Details.COST_CENTRE Is String.Empty Or Profile.PartnerInfo.Details.COST_CENTRE Is Nothing Then
                                sender.Visible = False
                            End If
                        End If

                    Case Is = "ACCOUNTNOCOLUMN"
                        Select Case Usage.ToUpper
                            Case "BASKET"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayAccountNo_Basket"))
                            Case "ORDERSUMMARY", "PAYMENT", "ORDER"
                                sender.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("DisplayAccountNo_OrderSummary"))
                        End Select
                        '
                        ' Only display the cost Centre and account fields when the field is present in the profile.
                        '
                        If Profile.IsAnonymous Or Profile.PartnerInfo.Details Is Nothing Then
                            sender.Visible = False
                        End If
                        If Not Profile.PartnerInfo.Details Is Nothing Then
                            If Profile.PartnerInfo.Details.COST_CENTRE Is String.Empty Or Profile.PartnerInfo.Details.COST_CENTRE Is Nothing Then
                                sender.Visible = False
                            End If
                        End If

                End Select
            End With
            ' End If
        End If
    End Sub

    Protected Sub GetRegExSettings(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Display Then
            Select Case sender.ID
                Case Is = "QuantityRegEx"
                    Dim reg As RegularExpressionValidator = CType(sender, RegularExpressionValidator)
                    reg.ValidationExpression = "^[0-9]{0,20}$"
                    reg.ErrorMessage = _ucr.Content("QuantityNotWholeNumberError", _languageCode, True)
            End Select
        End If
    End Sub

    Protected Sub GetURL(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Display Then
            Dim productcode As Label = CType(CType(sender.Parent, HtmlTableCell).Parent, RepeaterItem).FindControl("ProductCodeLabel")
            Dim basketDetailId As Label = CType(CType(sender.Parent, HtmlTableCell).Parent, RepeaterItem).FindControl("BasketDetailIdLabel")
            Dim productlink As HyperLink = CType(CType(sender.Parent, HtmlTableCell).Parent, RepeaterItem).FindControl("ProductHyperLink")

            Dim navigateString As String = "~/PagesPublic/ProductBrowse/product.aspx?"

            Dim defaults As New Talent.eCommerce.ECommerceModuleDefaults
            Dim myDefaults As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
            myDefaults = defaults.GetDefaults
            Dim noOfGroups As Integer = myDefaults.NumberOfGroupLevels

            If BasketItems Is Nothing Then
                BasketItems = TDataObjects.BasketSettings.TblBasketDetail.GetNonTicketingDetailByBasketHeaderID(GetBasketID)
            End If
            If Not BasketItems Is Nothing Then
                For Each item As DataRow In BasketItems.Rows
                    If item("PRODUCT") = productcode.Text AndAlso item("BASKET_DETAIL_ID") = basketDetailId.Text Then
                        navigateString = "~/PagesPublic/ProductBrowse/product.aspx?"
                        For i As Integer = 1 To noOfGroups
                            navigateString = navigateString & "group" & (i).ToString & "=" & item(String.Format("GROUP_LEVEL_{0}", (i).ToString("00")))
                            If Not i = noOfGroups Then
                                navigateString = navigateString & "&"
                            End If
                        Next
                        If Not String.IsNullOrEmpty(item("MASTER_PRODUCT").ToString().Trim) Then
                            navigateString += "&product=" & item("MASTER_PRODUCT")
                        Else
                            navigateString += "&product=" & item("PRODUCT")
                        End If
                        If item("IS_FREE") Then
                            If item("ALLOW_SELECT_OPTION") Then
                                navigateString += "&promotionbasketid=" & item("BASKET_DETAIL_ID")
                            Else
                                navigateString = ""
                            End If
                        End If
                        productlink.NavigateUrl = navigateString
                    End If
                Next
            End If
        End If
    End Sub

    Protected Sub CheckAgeLimits()
        Dim defs As New Talent.eCommerce.ECommerceModuleDefaults
        Dim values As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
        values = defs.GetDefaults

        If Not Profile.IsAnonymous AndAlso values.Use_Age_Check Then
            Dim products As New TalentProductInformationTableAdapters.tbl_productTableAdapter
            Dim dt As Data.DataTable
            Dim UnderAge As Boolean = False
            For Each ri As RepeaterItem In BasketRepeater.Items
                Try
                    Dim pc As String = CType(ri.FindControl("ProductCodeLabel"), Label).Text
                    Dim ItemErrorLabel As Label = CType(ri.FindControl("ItemErrorLabel"), Label)
                    dt = products.GetDataByProduct_Code(pc)
                    If Not String.IsNullOrEmpty(pc) Then
                        If Utilities.CheckForDBNull_Int(dt.Rows(0)("PRODUCT_MINIMUM_AGE")) > ProfileHelper.GetAge(Profile.User.Details.DOB) Then
                            ItemErrorLabel.Text = "**"
                            ItemErrorLabel.Visible = True
                            UnderAge = True
                        End If
                    End If
                Catch ex As Exception
                End Try
            Next
            If UnderAge Then
                ErrorList.Items.Add(_ucr.Content("UserUnderAgeErrorMessage", _languageCode, True))
            End If
        End If
    End Sub

    Protected Sub CheckIfBasketEmpty()
        If Profile.Basket.CAT_MODE <> GlobalConstants.CATMODE_CANCEL Then
            If Usage.ToUpper = "PAYMENT" _
                AndAlso Not Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("ShowBasketContentsOnPaymentScreen")) Then

                promoSummaryTitle.Visible = False
                PromotionsSummary.Visible = False
            Else
                If BasketRepeater.Items.Count = 0 Then
                    BasketRepeater.Visible = False
                    SummaryTotals1.Visible = False
                    promoSummaryTitle.Visible = False
                    PromotionsSummary.Visible = False
                    Dim areOtherItems As Boolean = False

                    Try
                        Dim tbd As UserControls_TicketingBasketDetails = CType(Utilities.FindWebControl("TicketingBasketDetails1", Me.Page.Controls), UserControls_TicketingBasketDetails)
                        If tbd.Event_Repeater.Items.Count > 0 Then
                            areOtherItems = True
                        End If
                    Catch ex As Exception
                    End Try

                    If Not areOtherItems Then
                        If ModuleDefaults.RedirectOnEmptyBasket And Not String.IsNullOrEmpty(ModuleDefaults.RedirectOnEmptyBasketURL) Then
                            Response.Redirect(ModuleDefaults.RedirectOnEmptyBasketURL)
                        End If
                        Dim newLi As New ListItem(NoItemsInBasketErrorText)
                        If Not ErrorList.Items.Contains(newLi) Then
                            ErrorList.Items.Add(NoItemsInBasketErrorText)
                        End If
                    End If

                Else
                    BasketRepeater.Visible = True
                End If
            End If
        End If
    End Sub

    Protected Sub PersonaliseItem(ByVal sender As Object, ByVal e As EventArgs)
        Dim row As RepeaterItem = CType(sender, Button).Parent.Parent
        Dim productcode As Label = row.FindControl("ProductCodeLabel")
        Dim productmaster As Label = row.FindControl("ProductMasterLabel")
        Response.Redirect("~/PagesPublic/ProductBrowse/Personalise.aspx?product=" & productcode.Text & "&master=" & productmaster.Text)
    End Sub

    Protected Sub DeleteItem(ByVal sender As Object, ByVal e As EventArgs)
        Dim row As RepeaterItem = CType(sender, Button).Parent.Parent
        Dim productcode As Label = row.FindControl("ProductCodeLabel")
        Dim basketdetailid As Label = row.FindControl("BasketDetailIdLabel")
        Dim xmlconfig As Label = row.FindControl("XmlConfigLabel")
        Dim masterProduct As Label = row.FindControl("ProductMasterLabel")
        Dim basketAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        If Not Session("personalisationTransactions") Is Nothing Then
            Dim tbl As Data.DataTable
            Dim rowsToRemove As Collection = New Collection()
            tbl = Session("personalisationTransactions")
            For Each pRow As Data.DataRow In tbl.Rows
                If pRow.Item("PRODUCT_CODE") = productcode.Text Then
                    rowsToRemove.Add(pRow)
                End If
            Next
            Session("personalisationTransactions") = tbl
            For Each dRow As Data.DataRow In rowsToRemove
                tbl.Rows.Remove(dRow)
            Next
        End If

        basketAdapter.Delete_Basket_Item(GetBasketID, productcode.Text)
        If BasketItems Is Nothing Then
            Dim items As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
            BasketItems = TDataObjects.BasketSettings.TblBasketDetail.GetNonTicketingDetailByBasketHeaderID(GetBasketID)
        End If
        BasketItems.DefaultView.RowFilter = "IS_FREE=0"
        If BasketItems.DefaultView.Count <= 0 Then
            Dim basketDetails As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
            basketDetails.Delete_All_Retail_Free_Items(GetBasketID)
        End If

        ' Remove alt items for item on stop
        Dim ds As New Data.DataSet
        If Not Session("AlternativeProducts") Is Nothing Then
            ds = CType(Session("AlternativeProducts"), Data.DataSet)
            Dim i As Integer = 0
            For i = ds.Tables("ALTPRODUCTRESULTS").Rows.Count - 1 To 0 Step -1
                If ds.Tables("ALTPRODUCTRESULTS").Rows(i)("ProductCode").ToString.Trim = productcode.Text.Trim Then
                    ds.Tables("ALTPRODUCTRESULTS").Rows(i).Delete()
                End If
            Next
            ds.Tables("ALTPRODUCTRESULTS").AcceptChanges()
        End If

        Dim talBasketProcessor As New Talent.Common.TalentBasketProcessor
        talBasketProcessor.Settings = Utilities.GetSettingsObject()
        Dim errObj As Talent.Common.ErrorObj = talBasketProcessor.ProcessSummaryForUpdatedBasket(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
        Response.Redirect("basket.aspx")
    End Sub

    Protected Sub DisplayBasketItemsOutOfStock()
        Dim pc As String
        Dim ItemErrorLabel As Label
        Dim StockIssueLabel As Label
        For Each ri As RepeaterItem In BasketRepeater.Items
            pc = CType(ri.FindControl("ProductCodeLabel"), Label).Text
            For Each bi As TalentBasketItem In Profile.Basket.BasketItems
                If bi.Product = pc AndAlso bi.STOCK_ERROR Then
                    ItemErrorLabel = CType(ri.FindControl("ItemErrorLabel"), Label)
                    Select Case bi.STOCK_ERROR_CODE
                        Case Is = "DISC"
                            ItemErrorLabel.Text = "***"
                            StockIssueLabel = CType(ri.FindControl("StockIssueLabel"), Label)
                            StockIssueLabel.Text = _ucr.Content("StockIssueText", _languageCode, True).Replace("<<stock>>", bi.QUANTITY_AVAILABLE.ToString("########0"))
                            StockIssueLabel.Visible = True
                        Case Else
                            ItemErrorLabel.Text = "**"
                            ItemErrorLabel.Visible = True
                    End Select
                    Exit For
                End If
            Next
        Next
    End Sub

    Protected Sub CheckAlternativeItems()
        Dim ds As Data.DataSet
        ds = CType(Session("AlternativeProducts"), Data.DataSet)
        Dim dt As Data.DataTable = ds.Tables("ALTPRODUCTRESULTS")

        Dim altItemsErrorMessage As String = _ucr.Content("AltItemsErrorMessage", _languageCode, True)
        ErrorList.Items.Remove(altItemsErrorMessage)

        Dim basketDetail As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        Dim dtBasket As Data.DataTable = basketDetail.GetBasketItems_ByHeaderID_ALL(CType(Profile.Basket.Basket_Header_ID, Long))

        ' Loop through basket and check if any items have alt items. 
        ' If so put out message.
        For Each row As Data.DataRow In dtBasket.Rows
            Dim drc As Data.DataRow() = dt.Select("ProductCode = '" & row("PRODUCT").ToString.Trim & "'")
            If drc.Length > 1 Then
                If ErrorList.Items.FindByText(altItemsErrorMessage) Is Nothing Then
                    ErrorList.Items.Add(altItemsErrorMessage)
                End If
            End If
        Next
    End Sub

    Protected Sub rptErrorList_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptErrorList.ItemCommand
        If e.CommandName = "OVERRIDE" Then
            Dim exceededBulkSalesBasketLimit As Boolean = False
            Dim redirectPath As New StringBuilder("~/Redirect/TicketingGateway.aspx")
            Dim redirectQuery As String = TicketingBasketDetails1.GetUpdatedItemsQueryString(exceededBulkSalesBasketLimit, True)
            If Not String.IsNullOrEmpty(redirectQuery) AndAlso Not exceededBulkSalesBasketLimit Then
                Dim hdfProductCodeInError As HiddenField = CType(e.Item.FindControl("hdfProductCodeInError"), HiddenField)
                Dim hdfStandCodeInError As HiddenField = CType(e.Item.FindControl("hdfStandCodeInError"), HiddenField)
                Dim productCode As String = hdfProductCodeInError.Value
                Dim standCode As String = hdfStandCodeInError.Value
                Dim errorCode As String = e.CommandArgument
                Session("RedirectUpdatebasket") = redirectQuery
                redirectPath.Append("?page=Basket.aspx&function=OverrideRestriction")
                redirectPath.Append("&ProductCodeInError=").Append(productCode)
                redirectPath.Append("&StandCodeInError=").Append(standCode)
                redirectPath.Append("&ErrorCodeToOverride=").Append(errorCode)
                Response.Redirect(redirectPath.ToString())
            End If
        End If
    End Sub

    Protected Sub rptErrorList_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptErrorList.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim errorMessageText As String = CType(e.Item.DataItem, KeyValuePair(Of String, TalentBasketItem)).Key
            Dim basketItem As TalentBasketItem = CType(e.Item.DataItem, KeyValuePair(Of String, TalentBasketItem)).Value
            Dim ltlErrorMessage As Literal = CType(e.Item.FindControl("ltlErrorMessage"), Literal)
            Dim lbtnOverrideOption As LinkButton = CType(e.Item.FindControl("lbtnOverrideOption"), LinkButton)
            Dim ltlOverrideOption As Literal = CType(e.Item.FindControl("ltlOverrideOption"), Literal)
            Dim hdfProductCodeInError As HiddenField = CType(e.Item.FindControl("hdfProductCodeInError"), HiddenField)
            Dim hdfStandCodeInError As HiddenField = CType(e.Item.FindControl("hdfStandCodeInError"), HiddenField)
            ltlErrorMessage.Text = errorMessageText
            lbtnOverrideOption.Visible = False

            'Set visibility of the override button based on the error code and if the agent has the permission to do so
            Select Case basketItem.STOCK_ERROR_CODE
                Case Is = GlobalConstants.BASKET_ERROR_FAMILY_AREA_RESTRICTION : lbtnOverrideOption.Visible = (AgentProfile.AgentPermissions.CanOverrideFamilyAreaRestrictionCheck)
                    'Product Pre-Req Overrides
                Case Is = GlobalConstants.BASKET_ERROR_PRODUCT_PRE_REQ : lbtnOverrideOption.Visible = (AgentProfile.AgentPermissions.CanOverrideProductPreReqCheck)
                Case Is = GlobalConstants.BASKET_ERROR_PRODUCT_PRE_REQ_FAMILY_AREA : lbtnOverrideOption.Visible = (AgentProfile.AgentPermissions.CanOverrideProductPreReqCheckFamilyArea)
                    'Ticket Limit Overrides
                Case Is = GlobalConstants.BASKET_ERROR_MAX_TICKET_LIMIT : lbtnOverrideOption.Visible = (AgentProfile.AgentPermissions.CanOverrideMaxTicketCheck)
                Case Is = GlobalConstants.BASKET_ERROR_TRANSACTION_LIMIT : lbtnOverrideOption.Visible = (AgentProfile.AgentPermissions.CanOverrideTransactionLimit)
                Case Is = GlobalConstants.BASKET_ERROR_PRODUCT_LIMIT_PER_TRANSACTION : lbtnOverrideOption.Visible = (AgentProfile.AgentPermissions.CanOverrideProductLimitPerTransaction)
                Case Is = GlobalConstants.BASKET_ERROR_MAX_LIMIT_OF_MEMBERSHIPS_PER_TRANSACTION : lbtnOverrideOption.Visible = (AgentProfile.AgentPermissions.CanOverrideMembershipLimitPerTransaction)
                Case Is = GlobalConstants.BASKET_ERROR_STAND_LEVEL_MAX_TICKET_LIMIT : lbtnOverrideOption.Visible = (AgentProfile.AgentPermissions.CanOverrideStandMaxTicketLimit)
                Case Is = GlobalConstants.BASKET_ERROR_MAX_LIMIT_FREE_SEATS : lbtnOverrideOption.Visible = (AgentProfile.AgentPermissions.CanOverrideFreeSeatsLimit)
                Case Is = GlobalConstants.BASKET_ERROR_MAX_PACKAGE_PURCHASE_LIMIT : lbtnOverrideOption.Visible = (AgentProfile.AgentPermissions.CanOverrideMaxTicketCheck)
            End Select
            If lbtnOverrideOption.Visible Then
                ltlOverrideOption.Text = _ucr.Content("OverrideBasketRestrictionButtonText", _languageCode, True)
                hdfProductCodeInError.Value = basketItem.Product
                lbtnOverrideOption.CommandArgument = basketItem.STOCK_ERROR_CODE
                hdfStandCodeInError.Value = basketItem.SEAT.Substring(0, 3)
            End If
        End If
    End Sub
#End Region

#Region "Public Methods"
    Public Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Display Then
            With _ucr
                Dim obj As Object = sender
                Select Case obj.ID.ToString
                    Case Is = "ProductNameLabel" : CType(obj, Label).Text = .Content("ProductColumnHeaderText", _languageCode, True)
                    Case Is = "ProductCodeLabel" : CType(obj, Label).Text = .Content("ProductCodeColumnHeaderText", _languageCode, True)
                    Case Is = "ColourLabel" : CType(obj, Label).Text = .Content("ColourColumnHeaderText", _languageCode, True)
                    Case Is = "SizeLabel" : CType(obj, Label).Text = .Content("SizeColumnHeaderText", _languageCode, True)
                    Case Is = "StockLabel" : CType(obj, Label).Text = .Content("StockColumnHeaderText", _languageCode, True)
                    Case Is = "UnitPriceLabel" : CType(obj, Label).Text = .Content("UnitColumnHeaderText", _languageCode, True)
                    Case Is = "QuantityLabel" : CType(obj, Label).Text = .Content("QuantityColumnHeaderText", _languageCode, True)
                    Case Is = "ValueLabel" : CType(obj, Label).Text = .Content("ValueColumnHeaderText", _languageCode, True)
                    Case Is = "CostCentreLabel" : CType(obj, Label).Text = .Content("CostCentreColumnHeaderText", _languageCode, True)
                    Case Is = "AccountNoLabel" : CType(obj, Label).Text = .Content("AccountNoColumnHeaderText", _languageCode, True)
                    Case Is = "DeleteButton" : CType(obj, Button).Text = .Content("DeleteButtonText", _languageCode, True)
                    Case Is = "promoSummaryTitle" : CType(obj, Label).Text = .Content("promoSummaryTitle", _languageCode, True)
                End Select
            End With
        End If
    End Sub

    Public Sub BindBasketRepeater()
        If Usage.ToUpper = "PAYMENT" AndAlso Not Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("ShowBasketContentsOnPaymentScreen")) Then
            BasketRepeater.Visible = False
            If Profile.Basket.BasketContentType = "C" Then
                SummaryTotals1.SetUpTotals()
            Else
                SummaryTotals1.Visible = False
            End If
        Else

            Dim defs As New Talent.eCommerce.ECommerceModuleDefaults
            Dim values As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
            values = defs.GetDefaults

            Dim dt As New Data.DataTable
            dt = TDataObjects.BasketSettings.TblBasketDetail.GetNonTicketingDetailByBasketHeaderID(GetBasketID)

            BasketRepeater.DataSource = dt
            BasketRepeater.DataBind()

            Select Case Usage.ToUpper
                Case "PAYMENT"
                    If Profile.Basket.BasketContentType = "C" Then
                        SummaryTotals1.SetUpTotals()
                    Else
                        SummaryTotals1.Visible = False
                    End If
                Case Else
                    SummaryTotals1.SetUpTotals()
            End Select

            'Check Ticketing basket also
            CheckIfBasketEmpty()
            If dt.Rows.Count > 0 Then

                'If there are active promotions, display the relevant information
                Dim promoResults As Data.DataTable = TestForPromotions()

                If Not promoResults Is Nothing AndAlso promoResults.Rows.Count > 0 Then

                    ' If promotions returned results then re-bind basket as 'Free' 
                    ' items may have been added during testforpromotions 
                    ' (otherwise these don't get shown when first go into basket)
                    dt = TDataObjects.BasketSettings.TblBasketDetail.GetNonTicketingDetailByBasketHeaderID(GetBasketID)
                    BasketRepeater.DataSource = dt
                    BasketRepeater.DataBind()

                    Dim productCode As Label
                    Dim promoLabel As Label

                    'Loop through all items
                    For Each row As RepeaterItem In BasketRepeater.Items
                        productCode = CType(row.FindControl("ProductCodeLabel"), Label)
                        promoLabel = CType(row.FindControl("PromoLabel"), Label)

                        For Each promo As Data.DataRow In promoResults.Rows
                            If CBool(promo("Success")) Then
                                If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(promo("ProductCodes"))) Then
                                    Dim codes() As String = CStr(promo("ProductCodes")).Split(",")
                                    Dim promoForPdtDisplay As String = Utilities.CheckForDBNull_String(_ucr.Content("PromoForProductDisplay", _languageCode, True))
                                    For Each code As String In codes
                                        If UCase(code) = UCase(productCode.Text) Then
                                            If Not String.IsNullOrEmpty(CStr(promo("PromotionDisplayName"))) Then
                                                promoForPdtDisplay = (promoForPdtDisplay.Replace("<<PromotionDisplayName>>", Utilities.CheckForDBNull_String(promo("PromotionDisplayName"))).Replace("<<ApplicationCount>>", Utilities.CheckForDBNull_String(promo("ApplicationCount"))).Replace("<<PromotionCode>>", Utilities.CheckForDBNull_String(promo("PromotionCode"))))
                                                promoLabel.Text = promoForPdtDisplay
                                                promoLabel.Visible = True
                                                Exit For
                                            End If
                                        End If
                                    Next
                                End If
                            End If
                        Next
                    Next
                End If
            End If
        End If
    End Sub

#End Region

#Region "Public Functions"

    Public Function TestForPromotions() As Data.DataTable
        PromotionsSummary.Items.Clear()

        Select Case ModuleDefaults.PricingType
            Case 2
                Return New Data.DataTable
            Case Else
                Dim promoResults As Data.DataTable
                If (_talentWebPricing Is Nothing) Then
                    promoResults = New Data.DataTable
                Else
                    If Not _talentWebPricing.PromotionsResultsTable Is Nothing Then
                        promoResults = _talentWebPricing.PromotionsResultsTable
                    Else
                        promoResults = New Data.DataTable
                    End If
                End If
                If Not promoResults Is Nothing AndAlso promoResults.Rows.Count > 0 Then
                    For Each result As Data.DataRow In promoResults.Rows
                        If CBool(result("Success")) Then
                            Dim sPromoDisplay As String = "<<PromotionDisplayName>> ( x <<ApplicationCount>>)"
                            If result("ActivationMechanism") = Talent.Common.DBPromotions.code Then
                                Dim sPromoDisplayCode As String = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("PromotionsSummaryDisplayCode", _languageCode, True))
                                If sPromoDisplayCode <> String.Empty Then
                                    sPromoDisplay = sPromoDisplayCode
                                End If

                            ElseIf result("ActivationMechanism") = Talent.Common.DBPromotions.auto Then
                                Dim sPromoDisplayAuto As String = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("PromotionsSummaryDisplayAuto", _languageCode, True))
                                If sPromoDisplayAuto <> String.Empty Then
                                    sPromoDisplay = sPromoDisplayAuto
                                End If

                            End If
                            PromotionsSummary.Items.Add(sPromoDisplay.Replace("<<PromotionDisplayName>>", result("PromotionDisplayName")).Replace("<<ApplicationCount>>", result("ApplicationCount")).Replace("<<PromotionCode>>", result("PromotionCode")))
                        End If
                    Next
                End If
                Return promoResults
        End Select
    End Function

    Public Function GetProductInfo(ByVal productcode As String) As Data.DataTable
        Dim err As Talent.Common.ErrorObj
        Dim product As Data.DataTable
        Dim prodInfo As New Talent.Common.DEProductInfo(TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile), productcode, Talent.Common.Utilities.GetDefaultLanguage)
        Dim DBProdInfo As New Talent.Common.DBProductInfo(prodInfo)
        DBProdInfo.Settings = Talent.eCommerce.Utilities.GetSettingsObject()

        'Get the product info
        err = DBProdInfo.AccessDatabase()
        If Not err.HasError Then
            product = DBProdInfo.ResultDataSet.Tables("ProductInformation")
        Else
            'ERROR: could not retrieve product info
            product = Nothing
        End If
        Return product
    End Function

    Public Function IsValidQuantity(ByVal quantity As String) As Boolean
        Try
            If String.IsNullOrEmpty(quantity) Then
                ErrorList.Items.Add(_ucr.Content("NoQuantityError", _languageCode, True))
                Return False
            End If
            Dim quant As Decimal = CInt(quantity)
            If quant < ModuleDefaults.Min_Add_Quantity Then
                ErrorList.Items.Add(String.Format(_ucr.Content("MinQuantityNotMetError", _languageCode, True), ModuleDefaults.Min_Add_Quantity.ToString("0")))
                Return False
            End If
            If (Utilities.IsBasketHomeDelivery(Profile) AndAlso quant > 5) Then
                ErrorList.Items.Add(_ucr.Content("QuantityNotANumberError", _languageCode, True))
                Return False
            End If
        Catch ex As Exception
            ErrorList.Items.Add(_ucr.Content("QuantityNotANumberError", _languageCode, True))
            Return False
        End Try
        Return True
    End Function

    Public Function IsValidCostCentre() As Boolean
        Try
            ' Loop through each cost centre text box and determine whether it is valid.
            For Each ri As RepeaterItem In BasketRepeater.Items
                Dim costCentreBox As TextBox = CType(ri.FindControl("CostCentreTextBox"), TextBox)
                Dim ItemErrorLabel As Label = CType(ri.FindControl("ItemErrorLabel"), Label)

                If costCentreBox.Visible And costCentreBox.Enabled Then
                    ' Check the input
                    If Not Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("CostCentreExpression")) Is String.Empty Then
                        Dim regEx As New Regex(_ucr.Attribute("CostCentreExpression"))
                        If Not regEx.IsMatch(costCentreBox.Text) Then
                            ErrorList.Items.Add(_ucr.Content("CostCentreValError", _languageCode, True))
                            If ItemErrorLabel.Text Is String.Empty Then
                                ItemErrorLabel.Text = "***"
                                ItemErrorLabel.Visible = True
                            End If
                            Return False
                        End If
                    End If

                    ' Check if the field is mandatory
                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("CostCentreMandatory")) Then
                        If _ucr.Attribute("CostCentreMandatory") And costCentreBox.Text Is String.Empty Then
                            ErrorList.Items.Add(_ucr.Content("CostCentreValError", _languageCode, True))
                            If ItemErrorLabel.Text Is String.Empty Then
                                ItemErrorLabel.Text = "***"
                                ItemErrorLabel.Visible = True
                            End If
                            Return False
                        End If
                    End If
                End If
            Next

            ' No errors return a success
            Return True
        Catch ex As Exception
            ErrorList.Items.Add(_ucr.Content("CostCentreValError", _languageCode, True))
            Return False
        End Try
        Return True
    End Function

    Public Function IsValidAccountNo() As Boolean
        Try
            ' Loop through each cost centre text box and determine whether it is valid.
            For Each ri As RepeaterItem In BasketRepeater.Items
                Dim accountNoBox As TextBox = CType(ri.FindControl("AccountNoTextBox"), TextBox)
                Dim ItemErrorLabel As Label = CType(ri.FindControl("ItemErrorLabel"), Label)
                If accountNoBox.Visible And accountNoBox.Enabled Then
                    ' Check the input
                    If Not Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("AccountNoExpression")) Is String.Empty Then
                        Dim regEx As New Regex(_ucr.Attribute("AccountNoExpression"))
                        If Not regEx.IsMatch(accountNoBox.Text) Then
                            ErrorList.Items.Add(_ucr.Content("AccountNoValError", _languageCode, True))
                            If ItemErrorLabel.Text Is String.Empty Then
                                ItemErrorLabel.Text = "***"
                                ItemErrorLabel.Visible = True
                            End If
                            Return False
                        End If
                    End If

                    ' Check if the field is mandatory
                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("AccountNoMandatory")) Then
                        If _ucr.Attribute("AccountNoMandatory") And accountNoBox.Text Is String.Empty Then
                            ErrorList.Items.Add(_ucr.Content("AccountNoValError", _languageCode, True))
                            If ItemErrorLabel.Text Is String.Empty Then
                                ItemErrorLabel.Text = "***"
                                ItemErrorLabel.Visible = True
                            End If
                            Return False
                        End If
                    End If
                End If
            Next

            ' No errors return a success
            Return True
        Catch ex As Exception
            ErrorList.Items.Add(_ucr.Content("AccountNoValError", _languageCode, True))
            Return False
        End Try
        Return True
    End Function

    Public Function ProductInStock(ByVal productcode As String, ByVal quantity As Decimal) As Boolean
        If Stock.GetStockBalance(productcode) < quantity Then Return False Else Return True
    End Function

    Public Function ProductsInStock() As Boolean
        Dim AllInStock As Boolean = True
        Try
            Dim pc As String
            Dim quantity As Decimal
            For Each ri As RepeaterItem In BasketRepeater.Items
                pc = CType(ri.FindControl("ProductCodeLabel"), Label).Text
                quantity = CDec(CType(ri.FindControl("QuantityLabel"), Label).Text)
                Dim ItemErrorLabel As Label = CType(ri.FindControl("ItemErrorLabel"), Label)
                If Not String.IsNullOrEmpty(pc) Then
                    If Stock.GetStockBalance(pc) < quantity Then
                        Dim noStockDescription As String = Stock.GetNoStockDescription(pc, String.Empty)
                        If noStockDescription.ToUpper.Equals("NOT AVAILABLE") Then
                            'Product not available - no stock and has "not available" description
                            AllInStock = False
                            ItemErrorLabel.Text = "**"
                            ItemErrorLabel.Visible = True
                        ElseIf noStockDescription.Trim().Length = 0 Then
                            'Product not available - no stock and has no description
                            AllInStock = False
                            ItemErrorLabel.Text = "**"
                            ItemErrorLabel.Visible = True
                        Else
                            'Product available - no stock but has a description that informs the customer it can still be purchased
                            AllInStock = True
                            ItemErrorLabel.Text = String.Empty
                            ItemErrorLabel.Visible = False
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
        End Try
        Return AllInStock
    End Function

    Public Function removeFromXml(ByVal key As String, ByVal x As String) As String
        Dim xmlDoc As New XmlDocument
        Dim configNodes As XmlNodeList
        Dim configNode As XmlNode
        Dim baseDataNodes As XmlNodeList
        If x <> "" Then
            xmlDoc.LoadXml(x)
            configNodes = xmlDoc.GetElementsByTagName("config")
            For Each configNode In configNodes
                baseDataNodes = configNode.ChildNodes
                For Each baseDataNode As XmlNode In baseDataNodes
                    If baseDataNode.Attributes("name").Value.ToString = key Then
                        configNode.RemoveChild(baseDataNode)
                        Exit For
                    End If
                Next
            Next
        End If
        Return xmlDoc.InnerXml
    End Function

    Public Function GetImageURL(ByVal productcode As String) As String
        Dim str As String = ImagePath.getImagePath("PRODLIST", productcode, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
        ' If no image is returned then return the string with the missing image source
        If str = String.Empty Then
            Dim moduleDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = ModuleDefaults.GetDefaults()
            str = def.MissingImagePath
        End If
        Return str
    End Function

#End Region

#Region "Private Functions"

    Private Function IsPriceListExists(ByVal masterProductCode As String, ByVal productCode As String, ByVal isFree As Boolean) As Boolean
        Dim isExists As Boolean = False
        If isFree Then
            isExists = True
        Else
            Dim masterProductPriceList As Talent.Common.DEWebPrice = Nothing
            Dim productPriceList As Talent.Common.DEWebPrice = Nothing
            setPriceObjects()
            If String.IsNullOrWhiteSpace(masterProductCode) Then
                If (_masterProductsPriceList.TryGetValue(productCode, masterProductPriceList)) Then
                    If masterProductPriceList.SALE_PRICE_BREAK_QUANTITY_1 > 0 OrElse masterProductPriceList.PRICE_BREAK_QUANTITY_1 > 0 Then
                        If _webPrices.TryGetValue(productCode, productPriceList) Then
                            isExists = True
                        End If
                    Else
                        If _webPrices.TryGetValue(productCode, productPriceList) Then
                            isExists = True
                        End If
                    End If
                Else
                    If _webPrices.TryGetValue(productCode, productPriceList) Then
                        isExists = True
                    End If
                End If
            Else
                If (_masterProductsPriceList.TryGetValue(masterProductCode, masterProductPriceList)) Then
                    If masterProductPriceList.SALE_PRICE_BREAK_QUANTITY_1 > 0 OrElse masterProductPriceList.PRICE_BREAK_QUANTITY_1 > 0 Then
                        If _webPrices.TryGetValue(masterProductCode, productPriceList) Then
                            isExists = True
                        End If
                    Else
                        If _webPrices.TryGetValue(productCode, productPriceList) Then
                            isExists = True
                        End If
                    End If
                Else
                    If _webPrices.TryGetValue(productCode, productPriceList) Then
                        isExists = True
                    End If
                End If
            End If
        End If
        Return isExists
    End Function

#End Region

#Region "Private Methods"

    Private Sub bindErrorListRepeater()
        If AgentProfile.IsAgent AndAlso TicketingBasketDetails1.ErrorListForRepeater.Count > 0 Then
            rptErrorList.DataSource = TicketingBasketDetails1.ErrorListForRepeater
            rptErrorList.DataBind()
        End If
    End Sub

    ''' <summary>
    ''' This sub routine tidies the basket HTML based on what is in it (tickets/retail/both/empty)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub tidyUpBasket()
        If Profile.Basket.BasketItems.Count = 0 Then
            ecommerceBasketWrapper.Visible = False
        Else
            ecommerceBasketWrapper.Visible = True
            Select Case Profile.Basket.BasketContentType
                Case Is = GlobalConstants.COMBINEDBASKETCONTENTTYPE
                    plhMerchandiseBasket.Visible = True
                    plhTicketingBasket.Visible = True
                Case Is = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE
                    plhMerchandiseBasket.Visible = True
                    plhTicketingBasket.Visible = False
                Case Is = GlobalConstants.TICKETINGBASKETCONTENTTYPE
                    plhMerchandiseBasket.Visible = False
                    plhTicketingBasket.Visible = True
            End Select
            If plhMerchandiseBasket.Visible Then
                If PromotionsSummary.Items.Count > 0 Then
                    plhPromotionsSummary.Visible = True
                Else
                    plhPromotionsSummary.Visible = False
                End If
            End If
        End If


        plhErrorList.Visible = ((Error_List.Items.Count > 0) OrElse (rptErrorList.Items.Count > 0))
        plhSuccessMessages.Visible = (blSuccessList.Items.Count > 0)
        ecommerceBasketWrapper.Visible = (TicketingBasketDetails1.Visible OrElse CATBasket1.Visible OrElse CashBackSummary.Visible OrElse plhMerchandiseBasket.Visible)
        If plhTicketingBasket.Visible AndAlso AgentProfile.BulkSalesMode Then
            BulkSalesModeCssClass = "ebiz-bulk-sales-mode"
        Else
            BulkSalesModeCssClass = String.Empty
        End If
    End Sub


    ''' <summary>
    ''' Retrieve the part payments total value that have already been applied.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub calculatePartPayments_OLD()
        If ModuleDefaults.OnAccountEnabled AndAlso Profile.Basket.BasketContentType <> GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
            Dim dtPartPayments As New Data.DataTable
            _partPaymentsTotal = 0
            Dim err As New Talent.Common.ErrorObj
            Dim tp As New Talent.Common.TalentPayment
            Dim dePayment As New Talent.Common.DEPayments
            With dePayment
                .SessionId = Profile.Basket.Basket_Header_ID.ToString
                If Not Profile.User.Details Is Nothing Then
                    .CustomerNumber = Profile.User.Details.LoginID
                End If
            End With

            _ucr.KeyCode = ONACCOUNTKEYCODE
            tp.De = dePayment
            tp.Settings = Utilities.GetSettingsObject()
            tp.Settings.Cacheing = _ucr.Attribute("Cacheing")
            tp.Settings.CacheTimeMinutes = _ucr.Attribute("CacheTimeMinutes")
            err = tp.RetrievePartPayments()
            _ucr.KeyCode = KEYCODE

            ' Was the call successful
            If Not err.HasError AndAlso Not tp.ResultDataSet Is Nothing AndAlso tp.ResultDataSet.Tables.Count = 2 AndAlso tp.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
                dtPartPayments = tp.ResultDataSet.Tables("PartPayments")
                For Each dr As Data.DataRow In dtPartPayments.Rows
                    _partPaymentsTotal = _partPaymentsTotal + CType(dr.Item("PaymentAmount"), Decimal)
                Next
            End If
        End If
    End Sub

    Private Sub setPriceObjects()
        If (Profile.Basket.MasterProductsPriceList Is Nothing) Then
            _masterProductsPriceList = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)
        Else
            _masterProductsPriceList = Profile.Basket.MasterProductsPriceList
        End If
        If (Profile.Basket.WebPrices Is Nothing) Then
            _webPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)
        Else
            _talentWebPricing = Profile.Basket.WebPrices
            _webPrices = Profile.Basket.WebPrices.RetrievedPrices
        End If
    End Sub

    Private Sub setCommentsLink(ByRef e As RepeaterItemEventArgs, ByRef hplComments As HtmlAnchor, ByRef ProductCode As String)
        Dim showComments As Boolean = False
        showComments = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("ShowComments"))
        If Profile.IsAnonymous OrElse AgentProfile.BulkSalesMode Then
            hplComments.Visible = False
        Else
            If Profile.Basket.CAT_MODE = GlobalConstants.CATMODE_AMEND Then
                showComments = False
            End If
            If showComments Then
                Dim ltlCommentsContent As Literal = CType(e.Item.FindControl("ltlCommentsContent"), Literal)
                hplComments.Visible = True
                ltlCommentsContent.Text = _ucr.Content("AddCommentsLabel", _languageCode, True)
                hplComments.HRef = "~/PagesPublic/Basket/Comments.aspx" + "?Seat=" + ProductCode + "&Customer=" + Profile.User.Details.Account_No_1 + "&Product=RTPROD&TempBasketID=" + Profile.Basket.TempOrderID
            Else
                hplComments.Visible = False
            End If
        End If
    End Sub

#End Region


End Class
