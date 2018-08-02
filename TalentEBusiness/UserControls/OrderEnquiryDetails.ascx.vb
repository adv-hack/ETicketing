Imports Talent.eCommerce

Partial Class UserControls_OrderEnquiryDetails
    Inherits ControlBase

    Dim ucr As New Talent.Common.UserControlResource
    Dim moduleDefaults As New ECommerceModuleDefaults
    Dim defaults As New ECommerceModuleDefaults.DefaultValues
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Private _items As TalentBasketDataset.tbl_order_detailDataTable
    Public Property OrderItems() As TalentBasketDataset.tbl_order_detailDataTable
        Get
            Return _items
        End Get
        Set(ByVal value As TalentBasketDataset.tbl_order_detailDataTable)
            _items = value
        End Set
    End Property

    Private _tempOrderNo As String
    Public Property TempOrderNo() As String
        Get
            Return _tempOrderNo
        End Get
        Set(ByVal value As String)
            _tempOrderNo = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "OrderEnquiryDetails.ascx"
        End With

        If Not Page.IsPostBack Then
            If Not Request.QueryString("wid") Is Nothing Then
                '------------------------------
                '   Populate the Header Info
                '------------------------------
                Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                Dim order As TalentBasketDataset.tbl_order_headerDataTable = orders.Get_Order_Header_By_Processed_Order_ID(Request.QueryString("wid"))
                If (order.Rows.Count > 0) AndAlso ((order.Rows(0)("LOGINID") = Profile.User.Details.LoginID) OrElse (Profile.PartnerInfo.Details.Order_Enquiry_Show_Partner_Orders)) Then
                    OrderInfoView.DataSource = order
                    OrderInfoView.DataBind()

                    '------------------------------
                    '   Populate the Order Lines
                    '------------------------------
                    If String.IsNullOrEmpty(TempOrderNo) Then
                        Dim orderRow As TalentBasketDataset.tbl_order_headerRow = order.Rows(0)
                        TempOrderNo = orderRow.TEMP_ORDER_ID
                    End If
                    Dim orderDetails As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
                    Dim orderDetail As TalentBasketDataset.tbl_order_detailDataTable = orderDetails.Get_Order_Lines(TempOrderNo)

                    OrderLines.DataSource = orderDetail
                    OrderLines.DataBind()

                    '--------------------------------------
                    ' Determine whether to show the summary
                    '--------------------------------------
                    SummaryTotals1.Visible = False
                    defaults = moduleDefaults.GetDefaults

                    If defaults.OrderEnquiryShowOrderSummary Then
                        SummaryTotals1.Visible = True
                    End If

                    '---------------------------
                    '   Unload all DB objects
                    '---------------------------
                    orders.Dispose()
                    order.Dispose()
                    orderDetails.Dispose()
                    orderDetail.Dispose()
                    If Utilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("ShowReOrderButton")) AndAlso Utilities.IsPartnerHomeDeliveryType(Profile) Then
                        ReOrderButton.Visible = True
                        ReOrderButton.Text = ucr.Content("ReOrderButtonText", _languageCode, True)
                    Else
                        ReOrderButton.Visible = False
                    End If
                Else
                    ErrorList.Items.Clear()
                    ErrorList.Items.Add(ucr.Content("NoOrderDetailsFound", _languageCode, True))
                    ReOrderButton.Visible = False
                    SummaryTotals1.Visible = False
                End If
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (ErrorList.Items.Count > 0)
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Select Case sender.ID
            Case Is = "LineNoHeader"
                CType(sender, Label).Text = ucr.Content("LineNoHeader", _languageCode, True)
            Case Is = "ProductCodeHeader"
                CType(sender, Label).Text = ucr.Content("ProductCodeHeader", _languageCode, True)
            Case Is = "DescriptionHeader"
                CType(sender, Label).Text = ucr.Content("DescriptionHeader", _languageCode, True)
            Case Is = "ShipmentNoHeader"
                CType(sender, Label).Text = ucr.Content("ShipmentNoHeader", _languageCode, True)
            Case Is = "DespatchDateHeader"
                CType(sender, Label).Text = ucr.Content("DespatchDateHeader", _languageCode, True)
            Case Is = "QuantityOrderedHeader"
                CType(sender, Label).Text = ucr.Content("QuantityOrderedHeader", _languageCode, True)
            Case Is = "QuantityShippedHeader"
                CType(sender, Label).Text = ucr.Content("QuantityShippedHeader", _languageCode, True)
            Case Is = "ValueHeader"
                CType(sender, Label).Text = ucr.Content("ValueHeader", _languageCode, True)
            Case Is = "DeliveryTrackingHeader"
                CType(sender, Label).Text = ucr.Content("DeliveryTrackingHeader", _languageCode, True)
            Case Is = "WebOrderNoLabel"
                CType(sender, Label).Text = ucr.Content("WebOrderNoLabel", _languageCode, True)
            Case Is = "OrderNoLabel"
                CType(sender, Label).Text = ucr.Content("OrderNoLabel", _languageCode, True)
            Case Is = "CustomerRefLabel"
                CType(sender, Label).Text = ucr.Content("CustomerRefLabel", _languageCode, True)
            Case Is = "DeliveryContactLabel"
                CType(sender, Label).Text = ucr.Content("DeliveryContactLabel", _languageCode, True)
            Case Is = "OrderDateLabel"
                CType(sender, Label).Text = ucr.Content("OrderDateLabel", _languageCode, True)
            Case Is = "DeliverToLabel"
                CType(sender, Label).Text = ucr.Content("DeliverToLabel", _languageCode, True)
            Case Is = "DeliveryTypeLabel"
                CType(sender, Label).Text = ucr.Content("DeliveryTypeLabel", _languageCode, True)
        End Select
    End Sub


    Protected Sub ReOrderButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ReOrderButton.Click
        ErrorList.Items.Clear()

        '------------------------------
        '   Populate the Order Lines
        '------------------------------
        If String.IsNullOrEmpty(TempOrderNo) Then
            Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
            Dim order As TalentBasketDataset.tbl_order_headerDataTable = orders.Get_Order_Header_By_Processed_Order_ID(Request.QueryString("wid"))
            OrderInfoView.DataSource = order
            OrderInfoView.DataBind()
            Dim orderRow As TalentBasketDataset.tbl_order_headerRow = order.Rows(0)
            TempOrderNo = orderRow.TEMP_ORDER_ID
            orders.Dispose()
            order.Dispose()
        End If
        Dim orderDetails As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
        Dim orderDetail As TalentBasketDataset.tbl_order_detailDataTable = orderDetails.Get_Order_Lines(TempOrderNo)

        Dim productCodes As New ArrayList
        Dim codesAndQty As New Generic.Dictionary(Of String, Decimal)
        Dim codesAndMasters As New Generic.Dictionary(Of String, String)
        For Each dtl As TalentBasketDataset.tbl_order_detailRow In orderDetail.Rows
            productCodes.Add(dtl.PRODUCT_CODE)
            codesAndQty.Add(dtl.PRODUCT_CODE, dtl.QUANTITY)
            If dtl.IsMASTER_PRODUCTNull Then
                codesAndMasters.Add(dtl.PRODUCT_CODE, "")
            Else
                codesAndMasters.Add(dtl.PRODUCT_CODE, dtl.MASTER_PRODUCT)
            End If
        Next

        Dim products As Data.DataTable = GetProductInfo(productCodes)

        If products.Rows.Count > 0 Then
            For Each prod As Data.DataRow In products.Rows
                Dim tbi As New TalentBasketItem
                With tbi
                    .Product = prod("PRODUCT_CODE")
                    .Quantity = codesAndQty(.Product)
                    .GROUP_LEVEL_01 = Utilities.CheckForDBNull_String(prod("GROUP_L01_GROUP"))
                    .GROUP_LEVEL_02 = Utilities.CheckForDBNull_String(prod("GROUP_L02_GROUP"))
                    .GROUP_LEVEL_03 = Utilities.CheckForDBNull_String(prod("GROUP_L03_GROUP"))
                    .GROUP_LEVEL_04 = Utilities.CheckForDBNull_String(prod("GROUP_L04_GROUP"))
                    .GROUP_LEVEL_05 = Utilities.CheckForDBNull_String(prod("GROUP_L05_GROUP"))
                    .GROUP_LEVEL_06 = Utilities.CheckForDBNull_String(prod("GROUP_L06_GROUP"))
                    .GROUP_LEVEL_07 = Utilities.CheckForDBNull_String(prod("GROUP_L07_GROUP"))
                    .GROUP_LEVEL_08 = Utilities.CheckForDBNull_String(prod("GROUP_L08_GROUP"))
                    .GROUP_LEVEL_09 = Utilities.CheckForDBNull_String(prod("GROUP_L09_GROUP"))
                    .GROUP_LEVEL_10 = Utilities.CheckForDBNull_String(prod("GROUP_L10_GROUP"))
                    .LOGINID = Profile.UserName
                    .MASTER_PRODUCT = codesAndMasters(.Product)
                    .Size = Utilities.CheckForDBNull_String(prod("PRODUCT_DESCRIPTION_1"))
                    .ALTERNATE_SKU = Utilities.CheckForDBNull_String(prod("ALTERNATE_SKU"))
                    .PRODUCT_DESCRIPTION1 = Utilities.CheckForDBNull_String(prod("PRODUCT_DESCRIPTION_1"))
                    Try
                        Select Case defaults.PricingType
                            Case 2
                                Dim prices As Data.DataTable = Talent.eCommerce.Utilities.GetChorusPrice(.Product, .Quantity)
                                If prices.Rows.Count > 0 Then
                                    .Gross_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("GrossPrice"))
                                    .Net_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("NetPrice"))
                                    .Tax_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("TaxPrice"))
                                End If
                            Case Else
                                Dim deWp As Talent.Common.DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(.Product, .Quantity, .MASTER_PRODUCT)
                                .Gross_Price = deWp.Purchase_Price_Gross
                                .Net_Price = deWp.Purchase_Price_Net
                                .Tax_Price = deWp.Purchase_Price_Tax
                        End Select
                    Catch ex As Exception
                    End Try
                    If Not Profile.IsAnonymous And Not Profile.PartnerInfo.Details Is Nothing Then
                        .Cost_Centre = Profile.PartnerInfo.Details.COST_CENTRE
                        .Account_Code = Order.GetLastAccountNo(Profile.User.Details.LoginID)
                    End If
                End With
                Profile.Basket.AddItem(tbi)
            Next
            Profile.Save()

            Try
                Dim myCtrl As UserControl = Utilities.FindWebControl("MiniBasket1", Me.Page.Controls)
                If Not myCtrl Is Nothing Then
                    CallByName(myCtrl, "ReBindBasket", CallType.Method)
                End If
            Catch ex As Exception
            End Try

            If products.Rows.Count <> orderDetail.Rows.Count Then
                Dim notAdded As New Generic.Dictionary(Of String, String)
                For Each dtl As TalentBasketDataset.tbl_order_detailRow In orderDetail.Rows
                    Dim found As Boolean = False
                    For Each prod As Data.DataRow In products.Rows
                        If Utilities.CheckForDBNull_String(prod("PRODUCT_CODE")) = dtl.PRODUCT_CODE Then
                            found = True
                            Exit For
                        End If
                    Next
                    If Not found Then
                        notAdded.Add(dtl.PRODUCT_CODE, dtl.PRODUCT_DESCRIPTION_1)
                    End If
                Next

                ErrorList.Items.Add(ucr.Content("SomeItemsNotAddedError", _languageCode, True))
                For Each key As String In notAdded.Keys
                    ErrorList.Items.Add(key & " - " & notAdded(key))
                Next
            Else
                ErrorList.Items.Add(ucr.Content("AllItemsAddedConfirmationText", _languageCode, True))
            End If
        Else
            ErrorList.Items.Add(ucr.Content("NoItemsAddedError", _languageCode, True))
        End If
    End Sub

    Public Function GetProductInfo(ByVal productcode As ArrayList) As Data.DataTable
        Dim err As Talent.Common.ErrorObj
        Dim product As Data.DataTable
        Dim prodInfo As New Talent.Common.DEProductInfo(TalentCache.GetBusinessUnit, _
                                                                TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                productcode, _
                                                                Talent.Common.Utilities.GetDefaultLanguage)

        Dim DBProdInfo As New Talent.Common.DBProductInfo(prodInfo)
        DBProdInfo.Settings = Talent.eCommerce.Utilities.GetSettingsObject()

        'Get the product info
        '------------------------
        err = DBProdInfo.AccessDatabase()

        If Not err.HasError Then
            product = DBProdInfo.ResultDataSet.Tables("ProductInformation")
        Else
            'ERROR: could not retrieve product info
            product = Nothing
        End If
        Return product
    End Function
    Protected Sub OrderLines_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles OrderLines.ItemDataBound
        '-----------------
        ' Set Tracking URL
        '-----------------
        If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
            Dim trackingUrl As String = ucr.Content("trackingUrl", _languageCode, True)
            Dim trackingNo As String = Utilities.CheckForDBNull_String(e.Item.DataItem("TRACKING_NO"))
            If trackingUrl <> String.Empty AndAlso trackingNo <> String.Empty Then
                Dim hypTracking As HyperLink = e.Item.FindControl("hypTrack")
                hypTracking.NavigateUrl = trackingUrl.Replace("<<TRACKING_NO>>", trackingNo)
                hypTracking.Target = "_blank"
            End If

        End If
    End Sub
    Protected Sub CanDisplayThisColumn(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Select Case sender.id
                Case "plhShowValue"
                    sender.Visible = Utilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("ShowOrderHeaderColumn"))
            End Select
        Catch ex As Exception
            sender.Visible = False
        End Try
    End Sub
    ''' <summary>
    ''' Format the currency for the given value
    ''' </summary>
    ''' <param name="value">The valye amount</param>
    ''' <returns>The formatted value string</returns>
    ''' <remarks></remarks>
    Public Function FormatCurrency(ByVal value As Decimal) As String
        Dim formattedString As String = value
        formattedString = TDataObjects.PaymentSettings.FormatCurrency(value, ucr.BusinessUnit, ucr.PartnerCode)
        Return formattedString
    End Function

End Class
