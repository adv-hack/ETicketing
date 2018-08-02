Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Basket Details
'
'       Date                        10/11/08
'
'       Author                      Ben Ford
'
'       CS Group 2007               All rights reserved.
'
'       Error Number Code base      UCBADE- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------

Partial Class UserControls_AlternativeProducts
    Inherits ControlBase


    Private _display As Boolean
    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property

    Private _productCode As String
    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property

    Private _altProducts As Data.DataSet
    Public Property AltProducts() As Data.DataSet
        Get
            Return _altProducts
        End Get
        Set(ByVal value As Data.DataSet)
            _altProducts = value
        End Set
    End Property

    Dim ucr As New Talent.Common.UserControlResource
    Public errMsg As Talent.Common.TalentErrorMessages
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Private _usage As String
    Public Property Usage() As String
        Get
            Return _usage
        End Get
        Set(ByVal value As String)
            _usage = value
        End Set
    End Property

    Private _dt As TalentBasketDataset.tbl_basket_detailDataTable
    Public Property BasketItems() As TalentBasketDataset.tbl_basket_detailDataTable
        Get
            Return _dt
        End Get
        Set(ByVal value As TalentBasketDataset.tbl_basket_detailDataTable)
            _dt = value
        End Set
    End Property

    Public ReadOnly Property GetBasketID() As Integer
        Get
            Return Profile.Basket.Basket_Header_ID
        End Get
    End Property


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        '---------------------------------------------------------------
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "AlternativeProducts.ascx"
        End With
        errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _
                                                        TalentCache.GetBusinessUnitGroup, _
                                                        TalentCache.GetPartner(Profile), _
                                                        ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
        If Me.Display Then
            ErrorList.Items.Clear()
        Else
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If AltProducts Is Nothing AndAlso Not Session("AlternativeProducts") Is Nothing Then
            AltProducts = Session("AlternativeProducts")
        End If
    End Sub

    Public Sub LoadAltProducts()
        'If Not AltProducts Is Nothing AndAlso Not ProductCode Is Nothing Then
        '    If Not Page.IsPostBack Then
        '        Dim dt As Data.DataTable = AltProducts.Tables("ALTPRODUCTRESULTS")
        '        If dt.Rows.Count > 0 Then
        '            ' Extract only the rows which relate to the current product
        '            dt.DefaultView.RowFilter = "ProductCode = '" & ProductCode.Trim & "'"
        '            AlternativeProductsRepeater.DataSource = dt
        '            AlternativeProductsRepeater.DataBind()
        '        End If

        '    End If
        'End If
    End Sub

    Protected Sub AlternativeProductsRepeater_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles AlternativeProductsRepeater.ItemCommand

        Dim strProduct As String = e.CommandArgument.ToString.Trim
        Dim quant As Integer = 1
        Dim lblProductCode As Label = CType(e.Item.FindControl("hdProductCode"), Label)
        If ProductCode Is Nothing OrElse ProductCode = String.Empty Then
            ProductCode = lblProductCode.Text.Trim
        End If
        '-------------------------
        ' Check if enough in stock
        '-------------------------
        Dim availQty As Integer = Stock.GetStockBalance(strProduct)
        If availQty < quant Then
            '---------------
            ' Quantity Error
            '---------------
        Else
            Dim tbi As New TalentBasketItem
            With tbi
                .Product = strProduct
                .Quantity = quant
                Try
                    Select Case ModuleDefaults.PricingType
                        Case 2
                            Dim prices As Data.DataTable = Talent.eCommerce.Utilities.GetChorusPrice(strProduct, quant)
                            If prices.Rows.Count > 0 Then
                                .Gross_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("GrossPrice"))
                                .Net_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("NetPrice"))
                                .Tax_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("TaxPrice"))
                            End If
                        Case Else
                            Dim deWp As Talent.Common.DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(strProduct, quant, strProduct)
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
                Try
                    Dim basketAdapters As New TalentBasketDatasetTableAdapters.tbl_group_productTableAdapter
                    Dim dt As Data.DataTable = basketAdapters.GetData_For_Basket_Product_Link(strProduct, _
                                                                                             TalentCache.GetBusinessUnit(), _
                                                                                             TalentCache.GetPartner(HttpContext.Current.Profile))
                    If dt.Rows.Count > 0 Then

                        .GROUP_LEVEL_01 = dt.Rows(0)("GROUP_L01_GROUP").ToString
                        .GROUP_LEVEL_02 = dt.Rows(0)("GROUP_L02_GROUP").ToString
                        .GROUP_LEVEL_03 = dt.Rows(0)("GROUP_L03_GROUP").ToString
                        .GROUP_LEVEL_04 = dt.Rows(0)("GROUP_L04_GROUP").ToString
                        .GROUP_LEVEL_05 = dt.Rows(0)("GROUP_L05_GROUP").ToString
                        .GROUP_LEVEL_06 = dt.Rows(0)("GROUP_L06_GROUP").ToString
                        .GROUP_LEVEL_07 = dt.Rows(0)("GROUP_L07_GROUP").ToString
                        .GROUP_LEVEL_08 = dt.Rows(0)("GROUP_L08_GROUP").ToString
                        .GROUP_LEVEL_09 = dt.Rows(0)("GROUP_L09_GROUP").ToString
                        .GROUP_LEVEL_10 = dt.Rows(0)("GROUP_L10_GROUP").ToString
                    End If
                    '----------------------------------------------------------
                    ' Add alternative product to basket and remove item on stop
                    '----------------------------------------------------------
                    Profile.Basket.AddItem(tbi)

                    Dim basketAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
                    basketAdapter.Delete_Basket_Item(GetBasketID, ProductCode)

                    '----------------------------------
                    ' Remove alt items for item on stop
                    '----------------------------------
                    Dim ds As New Data.DataSet
                    If Not Session("AlternativeProducts") Is Nothing Then
                        ds = CType(Session("AlternativeProducts"), Data.DataSet)
                        Dim i As Integer = 0
                        For i = ds.Tables("ALTPRODUCTRESULTS").Rows.Count - 1 To 0 Step -1
                            If ds.Tables("ALTPRODUCTRESULTS").Rows(i)("ProductCode").ToString.Trim = ProductCode.Trim Then
                                ds.Tables("ALTPRODUCTRESULTS").Rows(i).Delete()
                            End If
                        Next

                        ds.Tables("ALTPRODUCTRESULTS").AcceptChanges()
                    End If


                    Session("AlternativeProducts") = ds

                Catch ex As Exception
                End Try
            End With

            '--------------
            ' Reload basket 
            '--------------
            Response.Redirect("../../pagesPublic/basket/Basket.aspx")
        End If
     
    End Sub


    Protected Sub AlternativeProductsRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles AlternativeProductsRepeater.ItemDataBound
        ' Find description
        Dim prodCode As String = String.Empty
        Dim lblAltProdCode As Label
        Dim lblDescription As Label
        Dim btnAdd As Button
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            lblAltProdCode = CType(e.Item.FindControl("ProductCodeLabel"), Label)
            lblDescription = CType(e.Item.FindControl("ProductDescription"), Label)
            btnAdd = CType(e.Item.FindControl("AddButton"), Button)
            prodCode = lblAltProdCode.Text

            If Not prodCode = String.Empty Then
                '-------------------------
                ' Find product description
                '-------------------------
                Dim productData As New TalentProductInformationTableAdapters.tbl_productTableAdapter
                Dim dt2 As Data.DataTable = productData.GetDataByProduct_Code(prodCode)
                If dt2.Rows.Count > 0 Then
                    lblDescription.Text = dt2.Rows(0)("PRODUCT_DESCRIPTION_1")
                    '---------------
                    ' Set Add button
                    '---------------
                    With ucr
                        btnAdd.Text = .Content("AddButton", _languageCode, True)
                    End With
                End If

            End If

        End If

    End Sub
   

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not AltProducts Is Nothing AndAlso Not ProductCode Is Nothing Then
            If Not Page.IsPostBack Then
                Dim dt As Data.DataTable = AltProducts.Tables("ALTPRODUCTRESULTS")
                If dt.Rows.Count > 0 Then

                    Dim drc As Data.DataRow() = dt.Select("ProductCode = '" & ProductCode.Trim & "'")
                    If drc.Length > 1 Then
                        ' Extract only the rows which relate to the current product
                        dt.DefaultView.RowFilter = "ProductCode = '" & ProductCode.Trim & "'"
                        AlternativeProductsRepeater.DataSource = dt
                        AlternativeProductsRepeater.DataBind()
                    Else
                        Me.Display = False
                    End If
                Else
                    Me.Display = False
                End If

                End If
        End If

    End Sub
End Class
