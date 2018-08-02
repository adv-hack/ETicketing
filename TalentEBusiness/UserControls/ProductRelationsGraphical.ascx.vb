Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Xml
Imports System.Globalization
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Product Relations Graphical
'
'       Date                        23.03.2007
'
'       Author                      Andrew Green
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCPRELS- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_ProductRelationsGraphical
    Inherits ControlBase

    Private _businessUnit As String
    Private _partner As String
    Private _currentPage As String
    Private _PageNumber As Integer = 1
    Private _HasRows As Boolean = False
    Private _usage As String = Talent.Common.Utilities.GetAllString
    Private emptyString As String = String.Empty

    Private conTalent As SqlConnection = Nothing
    Private cmdSelect As SqlCommand = Nothing
    Private dtrProduct As SqlDataReader = Nothing

    Private _display As Boolean = True

    Private requestGroups() As String = {"GROUP_L01_GROUP = @GROUP1", "GROUP_L02_GROUP = @GROUP2", _
                                         "GROUP_L03_GROUP = @GROUP3", "GROUP_L04_GROUP = @GROUP4", _
                                         "GROUP_L05_GROUP = @GROUP5", "GROUP_L06_GROUP = @GROUP6", _
                                         "GROUP_L07_GROUP = @GROUP7", "GROUP_L08_GROUP = @GROUP8", _
                                         "GROUP_L09_GROUP = @GROUP9", "GROUP_L10_GROUP = @GROUP10"}
    Private requestGroupsBlank() As String = {"GROUP_L01_GROUP = ''", "GROUP_L02_GROUP = ''", _
                                              "GROUP_L03_GROUP = ''", "GROUP_L04_GROUP = ''", _
                                              "GROUP_L05_GROUP = ''", "GROUP_L06_GROUP = ''", _
                                              "GROUP_L07_GROUP = ''", "GROUP_L08_GROUP = ''", _
                                              "GROUP_L09_GROUP = ''", "GROUP_L10_GROUP = ''"}
    Private pageLevel() As String = {"browse01.aspx", "browse02.aspx", "browse03.aspx", "browse04.aspx", "browse05.aspx", _
                                     "browse06.aspx", "browse07.aspx", "browse08.aspx", "browse09.aspx", "browse10.aspx"}
    Dim productRelationsList As ProductListGen
    Private ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Private intRepeatColumns As Integer = 0
    Private intPageSize As Integer = 0
    Private boolOrderBySequence As Boolean = False
    Private boolShowImage As Boolean = False
    Private boolShowText As Boolean = False
    Private boolShowPrice As Boolean = False
    Private boolShowBuy As Boolean = False
    Private boolShowLink As Boolean = False
    Private strHeaderText As String = String.Empty
    Private strPriceLabel As String = String.Empty
    Private strPriceFromLabel As String = String.Empty
    Private strBuyButtonText As String = String.Empty
    Private intQuantityTextBoxSize As Integer = 0


    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property

    Private _forwardToBasket As Boolean
    Public Property ForwardToBasket() As Boolean
        Get
            Return _forwardToBasket
        End Get
        Set(ByVal value As Boolean)
            _forwardToBasket = value
        End Set
    End Property

    Public Property PageNumber() As Integer
        Get
            Return _PageNumber
        End Get
        Set(ByVal i As Integer)
            _PageNumber = i
        End Set
    End Property

    Public ReadOnly Property HasRows() As Boolean
        Get
            Return _HasRows
        End Get
    End Property

    Public Property Usage() As String
        Get
            Return _usage
        End Get
        Set(ByVal value As String)
            _usage = value
        End Set
    End Property

    Dim _pos As Integer
    Public Property PagePosition() As Integer
        Get
            Return _pos
        End Get
        Set(ByVal value As Integer)
            _pos = value
        End Set
    End Property

    Dim _template As String
    Public Property TemplateType() As String
        Get
            Return _template
        End Get
        Set(ByVal value As String)
            _template = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Display Then
            _businessUnit = TalentCache.GetBusinessUnit
            _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
            _currentPage = Talent.eCommerce.Utilities.GetCurrentPageName()

            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = Usage()
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "productRelationsGraphical.ascx"
            End With

            SetupControl()
            ' This *must* always load regardless of postback otherwise if there's a validation error 
            ' on the screen nothing displays. 

            ' If Not Page.IsPostBack And Display Then

            If Not Page.IsPostBack Then LoadProducts()
        Else
            Me.Visible = False
        End If

    End Sub

    Protected Sub SetupControl()
        If Display Then
            pnlProductRelationsGraphical.Visible = True
        
            Dim ds As DataSet = GetRelationsVariables()

            If Not ds.Tables("RelationsText") Is Nothing _
                                AndAlso Not ds.Tables("RelationsAttributes") Is Nothing Then

                Dim text As DataTable = ds.Tables("RelationsText")
                Dim attrib As DataTable = ds.Tables("RelationsAttributes")

                If text.Rows.Count > 0 Then
                    For Each dr As DataRow In text.Rows
                        Select Case dr("TEXT_CODE")
                            Case Is = "HEADER_TEXT"
                                strHeaderText = dr("TEXT_VALUE")
                            Case Is = "PRICE_LABEL"
                                strPriceLabel = dr("TEXT_VALUE")
                            Case Is = "BUY_BUTTON_TEXT"
                                strBuyButtonText = dr("TEXT_VALUE")
                            Case Is = "FROM_PRICE_LABEL"
                                strPriceFromLabel = dr("TEXT_VALUE")
                        End Select
                    Next
                End If

                ' Defaults
                If attrib.Rows.Count > 0 Then
                    For Each dr As DataRow In attrib.Rows
                        Select Case dr("ATTRIBUTE_CODE")
                            Case Is = "REPEAT_COLUMNS"
                                intRepeatColumns = CInt(dr("ATTRIBUTE_VALUE"))
                            Case Is = "PAGE_SIZE"
                                intPageSize = CInt(dr("ATTRIBUTE_VALUE"))
                            Case Is = "ORDER_BY_SEQUENCE"
                                boolOrderBySequence = CBool(dr("ATTRIBUTE_VALUE"))
                            Case Is = "SHOW_IMAGE"
                                boolShowImage = CBool(dr("ATTRIBUTE_VALUE"))
                            Case Is = "SHOW_TEXT"
                                boolShowText = CBool(dr("ATTRIBUTE_VALUE"))
                            Case Is = "SHOW_PRICE"
                                boolShowPrice = CBool(dr("ATTRIBUTE_VALUE"))
                            Case Is = "SHOW_BUY"
                                boolShowBuy = CBool(dr("ATTRIBUTE_VALUE"))
                            Case Is = "SHOW_LINK"
                                boolShowLink = CBool(dr("ATTRIBUTE_VALUE"))
                            Case Is = "QUANTITY_BOX_MAX_LENGTH"
                                intQuantityTextBoxSize = CInt(dr("ATTRIBUTE_VALUE"))
                            Case Is = "FORWARD_TO_BASKET"
                                ForwardToBasket = CBool(dr("ATTRIBUTE_VALUE"))
                        End Select
                    Next

                End If

                dlsProductRelationsGraphical.RepeatColumns = intRepeatColumns
            End If
        Else
            pnlProductRelationsGraphical.Visible = False
        End If

       
    End Sub

    Protected Function GetRelationsVariables() As Data.DataSet
        Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
        Dim ds As New DataSet("RelationsVariables")

        Dim CacheName As String = "RelationsVariables_" & TalentCache.GetBusinessUnit & "_" & _
                                        Talent.eCommerce.Utilities.GetCurrentPageName.ToLower & "_" & _
                                        Usage & "_" & _
                                        PagePosition & "_" & _
                                        TemplateType
       
        Dim text As New DataTable("RelationsText")
        Dim attrib As New DataTable("RelationsAttributes")

        Const SelectText As String = " SELECT * FROM tbl_product_relations_text_lang WITH (NOLOCK)  " & _
                                        " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                        "  AND PARTNER = @PARTNER " & _
                                        "  AND PAGE_CODE = @PAGE_CODE " & _
                                        "  AND QUALIFIER = @QUALIFIER "

        Const SelectAttrib As String = " SELECT * FROM tbl_product_relations_attribute_values WITH (NOLOCK)  " & _
                                        " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                        "  AND PARTNER = @PARTNER " & _
                                        "  AND PAGE_CODE = @PAGE_CODE " & _
                                        "  AND QUALIFIER = @QUALIFIER " & _
                                        "  AND PAGE_POSITION = @PAGE_POSITION " & _
                                        "  AND TEMPLATE_TYPE = @TEMPLATE_TYPE "
        cmd.CommandText = SelectText


        If Not Talent.Common.TalentThreadSafe.ItemIsInCache(CacheName)  Then
            Try

                Dim da As New SqlDataAdapter(cmd)

                cmd.Connection.Open()

                '--------------------------------------------
                '   tbl_product_relations_attribute_values
                '--------------------------------------------
                If TemplateType Is Nothing Then
                    TemplateType = String.Empty
                End If
                With cmd.Parameters
                    .Clear()
                    .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                    .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
                    .Add("@PAGE_CODE", SqlDbType.NVarChar).Value = Talent.eCommerce.Utilities.GetCurrentPageName
                    .Add("@QUALIFIER", SqlDbType.NVarChar).Value = Usage
                    .Add("@PAGE_POSITION", SqlDbType.NVarChar).Value = PagePosition
                    .Add("@TEMPLATE_TYPE", SqlDbType.NVarChar).Value = TemplateType
                    .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = _languageCode
                End With

                da.Fill(text)

                If Not text.Rows.Count > 0 Then
                    cmd.Parameters("@PARTNER").Value = "*ALL"
                    da.Fill(text)
                    If Not text.Rows.Count > 0 Then
                        cmd.Parameters("@BUSINESS_UNIT").Value = "*ALL"
                        da.Fill(text)
                        If Not text.Rows.Count > 0 Then
                            cmd.Parameters("@BUSINESS_UNIT").Value = TalentCache.GetBusinessUnit
                            cmd.Parameters("@PARTNER").Value = TalentCache.GetPartner(Profile)
                            cmd.Parameters("@PAGE_CODE").Value = "*ALL"
                            da.Fill(text)
                            If Not text.Rows.Count > 0 Then
                                cmd.Parameters("@PARTNER").Value = "*ALL"
                                da.Fill(text)
                                If Not text.Rows.Count > 0 Then
                                    cmd.Parameters("@BUSINESS_UNIT").Value = "*ALL"
                                    da.Fill(text)
                                    If Not text.Rows.Count > 0 Then
                                        cmd.Parameters("@QUALIFIER").Value = "*ALL"
                                        cmd.Parameters("@PAGE_POSITION").Value = "*ALL"
                                        cmd.Parameters("@TEMPLATE_TYPE").Value = "*ALL"
                                        da.Fill(text)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If


                '--------------------------------------------
                '   tbl_product_relations_attribute_values
                '--------------------------------------------
                da.SelectCommand.CommandText = SelectAttrib
                With cmd.Parameters
                    .Clear()
                    .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                    .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
                    .Add("@PAGE_CODE", SqlDbType.NVarChar).Value = Talent.eCommerce.Utilities.GetCurrentPageName
                    .Add("@QUALIFIER", SqlDbType.NVarChar).Value = Usage
                    .Add("@PAGE_POSITION", SqlDbType.NVarChar).Value = PagePosition
                    .Add("@TEMPLATE_TYPE", SqlDbType.NVarChar).Value = TemplateType
                End With

                da.Fill(attrib)

                If Not attrib.Rows.Count > 0 Then
                    cmd.Parameters("@PARTNER").Value = "*ALL"
                    da.Fill(attrib)
                    If Not attrib.Rows.Count > 0 Then
                        cmd.Parameters("@BUSINESS_UNIT").Value = "*ALL"
                        da.Fill(attrib)
                        If Not attrib.Rows.Count > 0 Then
                            cmd.Parameters("@BUSINESS_UNIT").Value = TalentCache.GetBusinessUnit
                            cmd.Parameters("@PARTNER").Value = TalentCache.GetPartner(Profile)
                            cmd.Parameters("@PAGE_CODE").Value = "*ALL"
                            da.Fill(attrib)
                            If Not attrib.Rows.Count > 0 Then
                                cmd.Parameters("@PARTNER").Value = "*ALL"
                                da.Fill(attrib)
                                If Not attrib.Rows.Count > 0 Then
                                    cmd.Parameters("@BUSINESS_UNIT").Value = "*ALL"
                                    da.Fill(attrib)
                                    If Not attrib.Rows.Count > 0 Then
                                        cmd.Parameters("@QUALIFIER").Value = "*ALL"
                                        cmd.Parameters("@PAGE_POSITION").Value = "*ALL"
                                        cmd.Parameters("@TEMPLATE_TYPE").Value = "*ALL"
                                        da.Fill(attrib)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                ds.Tables.Add(text)
                ds.Tables.Add(attrib)
                TalentCache.AddPropertyToCache(CacheName, ds, 30, TimeSpan.Zero, CacheItemPriority.Normal)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(CacheName)
            Catch ex As Exception

            End Try
        Else
            ds = CType(Cache.Item(CacheName), DataSet)
        End If

        Return ds
    End Function

    Protected Sub LoadProducts()

        Dim pageProductsList As IList
        Dim filePath As String = String.Empty


        productRelationsList = New ProductListGen
        productRelationsList.PageSize = intPageSize

        Dim err As New ErrorObj
        Try
            conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ConnectionString)
            conTalent.Open()
        Catch ex As Exception
            Const strError1 As String = "Could not establish connection to the database"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError1
                .ErrorNumber = "UCPRELS-01"
                .HasError = True
            End With
        End Try

        If Not err.HasError Then
            Dim partnerParm As String = Nothing
            '------------------------------------------------------------------------
            ' Determine whether navigation through product groups for this particular
            ' partner should be done using '*ALL' as the partner
            '------------------------------------------------------------------------
            If Talent.eCommerce.Utilities.GroupNavigationUsingAll() Then
                partnerParm = Talent.Common.Utilities.GetAllString
            Else
                partnerParm = _partner
            End If
            Try
                '----------------------
                ' Get Related Products for Page
                '----------------------
                Dim strSelect As String = "SELECT * FROM TBL_PRODUCT_RELATIONS AS PR WITH (NOLOCK)  " & _
                                            "INNER JOIN TBL_PRODUCT AS PD WITH (NOLOCK)  " & _
                                            "ON PR.RELATED_PRODUCT = PD.PRODUCT_CODE WHERE "
                Dim sbWhere As New StringBuilder
                Dim myDefaults As New ECommerceModuleDefaults
                Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
                Dim intMaxNoOfGroupLevels As Integer = def.NumberOfGroupLevels
                Dim intCount As Integer = 1
                Dim currentPageLevel As Integer = Array.IndexOf(pageLevel, _currentPage)
                With sbWhere
                    While intCount < intMaxNoOfGroupLevels
                        If Request.QueryString.ToString.Trim.Contains("group" & intCount.ToString) Then
                            .Append(requestGroups(intCount - 1))
                            .Append(" AND ")
                        Else
                            .Append(requestGroupsBlank(intCount - 1))
                            .Append(" AND ")
                        End If
                        intCount += 1
                    End While
                    .Append(" PR.QUALIFIER = @QUALIFIER AND ")
                    .Append(" PR.BUSINESS_UNIT = @BUSINESS_UNIT AND")
                    .Append(" PR.PARTNER = @PARTNER")
                    .Append(" AND PR.PRODUCT =")
                    If _currentPage = "product.aspx" Then
                        .Append(" @PRODUCT")
                    Else
                        .Append("''")
                    End If
                End With

                strSelect = strSelect & sbWhere.ToString
                If boolOrderBySequence Then
                    strSelect = strSelect & " ORDER BY PR.SEQUENCE"
                End If
                cmdSelect = New SqlCommand(strSelect, conTalent)

                '---------------
                ' Add parameters
                '---------------
                Dim intCount2 As Integer = 1
                While intCount2 <= 10
                    '                    If strSelect.Contains("@GROUP" & intCount2.ToString) Then
                    If Not Request("group" & intCount2) Is Nothing Then
                        cmdSelect.Parameters.Add(New SqlParameter("@GROUP" & intCount2.ToString, SqlDbType.Char, 20)).Value = Request("group" & intCount2)
                    Else
                        cmdSelect.Parameters.Add(New SqlParameter("@GROUP" & intCount2.ToString, SqlDbType.Char, 20)).Value = String.Empty
                    End If
                    '                    End If
                    intCount2 += 1
                End While
                With cmdSelect.Parameters
                    .Add(New SqlParameter("@QUALIFIER", SqlDbType.Char, 50)).Value = _usage
                    .Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                    .Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                End With
                
                If _currentPage = "product.aspx" Then
                    cmdSelect.Parameters.Add(New SqlParameter("@PRODUCT", SqlDbType.Char, 50)).Value = Request("product").ToString.Trim
                End If

                dtrProduct = cmdSelect.ExecuteReader()

                If dtrProduct.HasRows = False Then
                    cmdSelect.Parameters("@BUSINESS_UNIT").Value = _businessUnit
                    cmdSelect.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString
                    dtrProduct = cmdSelect.ExecuteReader()
                    If dtrProduct.HasRows = False Then
                        cmdSelect.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString
                        cmdSelect.Parameters("@PARTNER").Value = partnerParm
                        dtrProduct = cmdSelect.ExecuteReader()
                        If dtrProduct.HasRows = False Then
                            cmdSelect.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString
                            cmdSelect.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString
                            dtrProduct = cmdSelect.ExecuteReader()
                        End If
                    End If
                End If
                '
                ' Process rows
                If dtrProduct.HasRows() Then

                    lblHeaderText.Text = strHeaderText
                    Dim productCodes As New Generic.Dictionary(Of String, WebPriceProduct)

                    _HasRows = True
                    While dtrProduct.Read

                        Dim p As New Product

                        p.Code = dtrProduct("PRODUCT_CODE")
                        p.Description1 = dtrProduct("PRODUCT_DESCRIPTION_1")
                        p.Description2 = dtrProduct("PRODUCT_DESCRIPTION_2")
                        p.Country = dtrProduct("PRODUCT_COUNTRY")
                        p.Colour = dtrProduct("PRODUCT_COLOUR")
                        p.SequenceNo = dtrProduct("SEQUENCE")
                        p.Group1 = dtrProduct("RELATED_GROUP_L01_GROUP")
                        p.Group2 = dtrProduct("RELATED_GROUP_L02_GROUP")
                        p.Group3 = dtrProduct("RELATED_GROUP_L03_GROUP")
                        p.Group4 = dtrProduct("RELATED_GROUP_L04_GROUP")
                        p.Group5 = dtrProduct("RELATED_GROUP_L05_GROUP")
                        p.Group6 = dtrProduct("RELATED_GROUP_L06_GROUP")
                        p.Group7 = dtrProduct("RELATED_GROUP_L07_GROUP")
                        p.Group8 = dtrProduct("RELATED_GROUP_L08_GROUP")
                        p.Group9 = dtrProduct("RELATED_GROUP_L09_GROUP")
                        p.Group10 = dtrProduct("RELATED_GROUP_L10_GROUP")
                        p.AvailableOnline = dtrProduct("AVAILABLE_ONLINE").ToString
                        'p.GrossPrice = ProductPrice.Get_Price(p.Code, def.PriceList).GrossPrice
                        '-----------------
                        ' Find thumb image
                        '-----------------
                        filePath = ImagePath.getImagePath("PRODASSOC", p.Code, _businessUnit, _partner)
                        p.ImagePath = filePath
                        '----------------------------
                        ' Build navigate query string
                        '----------------------------
                        Dim sbQry As New StringBuilder
                        With sbQry
                            .Append("../PagesPublic/ProductBrowse/product.aspx?group1=").Append(p.Group1)
                            If intMaxNoOfGroupLevels > 1 Then
                                .Append("&group2=").Append(p.Group2)
                            End If
                            If intMaxNoOfGroupLevels > 2 Then
                                .Append("&group3=").Append(p.Group3)
                            End If
                            If intMaxNoOfGroupLevels > 3 Then
                                .Append("&group4=").Append(p.Group4)
                            End If
                            If intMaxNoOfGroupLevels > 4 Then
                                .Append("&group5=").Append(p.Group5)
                            End If
                            If intMaxNoOfGroupLevels > 5 Then
                                .Append("&group6=").Append(p.Group6)
                            End If
                            If intMaxNoOfGroupLevels > 6 Then
                                .Append("&group7=").Append(p.Group7)
                            End If
                            If intMaxNoOfGroupLevels > 7 Then
                                .Append("&group8=").Append(p.Group8)
                            End If
                            If intMaxNoOfGroupLevels > 8 Then
                                .Append("&group9=").Append(p.Group9)
                            End If
                            If intMaxNoOfGroupLevels > 9 Then
                                .Append("&group10=").Append(p.Group10)
                            End If
                            .Append("&product=").Append(p.Code)
                        End With
                        p.NavigateURL = sbQry.ToString
                        p.LinkEnabled = Not def.SuppressProductLinks
                        If p.AvailableOnline Then
                            productRelationsList.Add(p)
                            productCodes.Add(p.Code, New WebPriceProduct(p.Code, 0, p.Code))
                        End If

                    End While
                    Me.Visible = True

                    Dim removeProducts As New Generic.List(Of Product)
                    Dim productPrices As Generic.Dictionary(Of String, Talent.Common.DEWebPrice)
                    productPrices = Talent.eCommerce.Utilities.GetWebPrices_WithPromoDetails(productCodes)

                    'We don't want to get the prices for each product individually as
                    'it is inefficient, so it is done in batch here
                    For Each p As Product In productRelationsList.Products
                        Try
                            p.WebPrices = productPrices(p.Code)
                            If productPrices(p.Code).DisplayPrice_From > 0 Then
                                p.PriceForSorting = productPrices(p.Code).DisplayPrice_From
                            Else
                                p.PriceForSorting = productPrices(p.Code).DisplayPrice
                            End If
                        Catch ex As Exception
                            removeProducts.Add(p)
                        End Try
                    Next


                    If removeProducts.Count > 0 Then
                        For Each rp As Product In removeProducts
                            productRelationsList.Products.Remove(rp)
                        Next
                    End If

                Else
                    Me.Visible = False
                End If
                dtrProduct.Close()

            Catch ex As Exception
                Const strError8 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "UCPRELS-06"
                    .HasError = True
                End With
            End Try
            '------
            ' Close
            '------
            Try
                conTalent.Close()
            Catch ex As Exception
                Const strError9 As String = "Failed to close database connection"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError9
                    .ErrorNumber = "UCPRELS-07"
                    .HasError = True
                End With
            End Try
        End If

        If Not productRelationsList Is Nothing Then
            pageProductsList = productRelationsList.GetPageProducts(_PageNumber)
            dlsProductRelationsGraphical.DataSource = pageProductsList
            dlsProductRelationsGraphical.DataBind()
        End If


    End Sub

    '--------------
    ' Add to basket
    '--------------
    Protected Sub dlsProductListGraphical_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles dlsProductRelationsGraphical.ItemCommand
        Dim quant As Integer = ModuleDefaults.Default_Add_Quantity
        '    Dim customValidationSummary As New Talent.Commerce.CustomValidationSummary
        '   Dim pageref As System.Web.UI.Page = Me.Page
        Try
            quant = Integer.Parse(CType(e.Item.FindControl("txtQuantity"), TextBox).Text)
        Catch
        End Try

        Dim strProduct As String = e.CommandArgument.ToString

        Dim al As New ArrayList
        For Each strKey As String In Request.QueryString.Keys
            If strKey.ToLower.Contains("group") Then
                al.Add(Request.QueryString(strKey))
            End If
        Next

        Dim availQty As Integer = Stock.GetStockBalance(strProduct)
        If quant > availQty And ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
            plhShowErrors.Visible = True
            lblError.Text = ucr.Content("stockError", _languageCode, True)
        Else
            If quant >= ModuleDefaults.Min_Add_Quantity Then
                Dim tbi As New TalentBasketItem
                With tbi
                    .Product = strProduct
                    .Quantity = quant
                    Dim products As Data.DataTable = Talent.eCommerce.Utilities.GetProductInfo(strProduct)
                    If products IsNot Nothing Then
                        If products.Rows.Count > 0 Then
                            .ALTERNATE_SKU = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("ALTERNATE_SKU"))
                            .PRODUCT_DESCRIPTION1 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_1"))
                            .PRODUCT_DESCRIPTION2 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_2"))
                            .PRODUCT_DESCRIPTION3 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_3"))
                            .PRODUCT_DESCRIPTION4 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_4"))
                            .PRODUCT_DESCRIPTION5 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_5"))
                            .WEIGHT = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(products.Rows(0)("PRODUCT_WEIGHT"))
                        End If
                    End If
                    Select Case ModuleDefaults.PricingType
                        Case 2
                            Dim prices As DataTable = Talent.eCommerce.Utilities.GetChorusPrice(strProduct, quant)
                            If prices.Rows.Count > 0 Then
                                .Gross_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("GrossPrice"))
                                .Net_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("NetPrice"))
                                .Tax_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("TaxPrice"))
                            End If
                        Case Else
                            Dim deWp As DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(strProduct, quant, strProduct)
                            .Gross_Price = deWp.Purchase_Price_Gross
                            .Net_Price = deWp.Purchase_Price_Net
                            .Tax_Price = deWp.Purchase_Price_Tax
                    End Select
                    Try
                        .GROUP_LEVEL_01 = al(0)
                        .GROUP_LEVEL_02 = al(1)
                        .GROUP_LEVEL_03 = al(2)
                        .GROUP_LEVEL_04 = al(3)
                        .GROUP_LEVEL_05 = al(4)
                        .GROUP_LEVEL_06 = al(5)
                        .GROUP_LEVEL_07 = al(6)
                        .GROUP_LEVEL_08 = al(7)
                        .GROUP_LEVEL_09 = al(8)
                        .GROUP_LEVEL_10 = al(9)
                    Catch ex As Exception
                    End Try
                    If Not Profile.IsAnonymous And Not Profile.PartnerInfo.Details Is Nothing Then
                        .Cost_Centre = Profile.PartnerInfo.Details.COST_CENTRE
                        .Account_Code = Order.GetLastAccountNo(Profile.User.Details.LoginID)
                    End If
                End With
                Profile.Basket.AddItem(tbi)
                If ucr Is Nothing Then
                    With ucr
                        .BusinessUnit = TalentCache.GetBusinessUnit()
                        .PageCode = Usage()
                        .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                        .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                        .KeyCode = "productRelationsGraphical.ascx"
                    End With
                End If
                If ForwardToBasket Then
                    Response.Redirect("../../pagesPublic/basket/Basket.aspx")
                Else
                    Response.Redirect(Request.Url.ToString)
                End If
            Else
                plhShowErrors.Visible = True
                lblError.Text = String.Format(ucr.Content("MinQuantityNotMetError", _languageCode, True), ModuleDefaults.Min_Add_Quantity.ToString("0"))
            End If
        End If
    End Sub

    Protected Sub dlsProductRelationsGraphical_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dlsProductRelationsGraphical.ItemDataBound

        '---------------------------
        ' Set attributes in Datalist
        '---------------------------
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            If boolShowImage Then
                CType(e.Item.FindControl("ImageHyperLink"), HyperLink).Visible = True
            Else
                CType(e.Item.FindControl("ImageHyperLink"), HyperLink).Visible = False
            End If
            If boolShowLink Then
                CType(e.Item.FindControl("hypProductName"), HyperLink).Visible = True
            Else
                CType(e.Item.FindControl("hypProductName"), HyperLink).Visible = False
            End If
            If boolShowText Then
                CType(e.Item.FindControl("lblProductDescription"), Label).Visible = True
            Else
                CType(e.Item.FindControl("lblProductDescription"), Label).Visible = False
            End If
            If boolShowPrice Then
                CType(e.Item.FindControl("lblPriceText"), Label).Text = strPriceLabel
                CType(e.Item.FindControl("lblPriceText"), Label).Visible = True
                CType(e.Item.FindControl("lblPrice"), Label).Visible = True
            Else
                CType(e.Item.FindControl("lblPrice"), Label).Visible = False
                CType(e.Item.FindControl("lblPriceText"), Label).Visible = False
            End If
            If boolShowBuy Then
                CType(e.Item.FindControl("btnBuy"), Button).Text = strBuyButtonText
                CType(e.Item.FindControl("txtQuantity"), TextBox).Columns = intQuantityTextBoxSize
                CType(e.Item.FindControl("btnBuy"), Button).Visible = True
                CType(e.Item.FindControl("txtQuantity"), TextBox).Visible = True
                Try
                    CType(e.Item.FindControl("txtQuantity"), TextBox).Columns = CInt(ucr.Attribute("QuantityTextBoxSize"))
                Catch
                End Try
                Try
                    CType(e.Item.FindControl("txtQuantity"), TextBox).Text = CInt(ModuleDefaults.Default_Add_Quantity)
                Catch
                End Try
            Else
                CType(e.Item.FindControl("btnBuy"), Button).Visible = False
                CType(e.Item.FindControl("txtQuantity"), TextBox).Visible = False
            End If
        End If

        Try
            Dim p As Talent.eCommerce.Product = CType(e.Item.DataItem, Talent.eCommerce.Product)

            Dim img As Image = CType(e.Item.FindControl("promoImage"), Image)

            If p.WebPrices.IsPartOfPromotion AndAlso Not String.IsNullOrEmpty(p.WebPrices.PromotionImagePath) Then
                img.ImageUrl = ImagePath.getImagePath("PROMOMOTION", p.WebPrices.PromotionImagePath, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                img.AlternateText = p.WebPrices.PromotionDescriptionText
            Else
                img.Visible = False
            End If

            If ModuleDefaults.ShowFromPrices Then
                If p.WebPrices.PRICE_BREAK_QUANTITY_1 >= p.WebPrices.PRICE_BREAK_QUANTITY_2 Then
                    CType(e.Item.FindControl("lblPriceText"), Label).Text = strPriceLabel
                Else
                    CType(e.Item.FindControl("lblPriceText"), Label).Text = strPriceFromLabel
                End If
                CType(e.Item.FindControl("lblPrice"), Label).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice_From, ucr.BusinessUnit, ucr.PartnerCode)
            Else
                CType(e.Item.FindControl("lblPriceText"), Label).Text = strPriceLabel
                CType(e.Item.FindControl("lblPrice"), Label).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice, ucr.BusinessUnit, ucr.PartnerCode)
            End If
            If Talent.eCommerce.Stock.GetStockBalance(p.Code) < 1 And ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
                CType(e.Item.FindControl("btnBuy"), Button).Visible = False
                CType(e.Item.FindControl("txtQuantity"), TextBox).Visible = False
                CType(e.Item.FindControl("NoStockLabel"), Label).Visible = True
                CType(e.Item.FindControl("NoStockLabel"), Label).Text = ucr.Content("ProductOutOfStockText", _languageCode, True)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub GetRegExErrorText(ByVal sender As Object, ByVal e As EventArgs)
        Dim regex As RegularExpressionValidator = (CType(sender, RegularExpressionValidator))
        Select Case regex.ID
            Case Is = "QuantityValidator"
                regex.ErrorMessage = ucr.Content("QuantityValidatorErrorText", _languageCode, True)
        End Select
    End Sub

    Protected Sub GetDefaultQuantity(ByVal sender As Object, ByVal e As EventArgs)
        CType(sender, TextBox).Text = ModuleDefaults.Default_Add_Quantity.ToString("0")
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        '    If Not TemplateType Is Nothing Then
        If Display Then pnlProductRelationsGraphical.CssClass = "template-" & TemplateType.ToString
        '   End If

    End Sub
End Class
