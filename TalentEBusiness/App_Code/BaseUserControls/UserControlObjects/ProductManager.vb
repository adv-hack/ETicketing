Imports Microsoft.VisualBasic
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce
Imports Talent.Common

Namespace Talent.UI

    Public Class ProductManager
        Inherits BaseManager

        Private productCodes As Generic.Dictionary(Of String, WebPriceProduct) = New Generic.Dictionary(Of String, WebPriceProduct)
        Private productsList As List(Of IProduct) = New List(Of IProduct)
        Private hasHTMLContent As Boolean = False

        ''' <summary>
        ''' Loads products into a list of IProduct objects
        ''' </summary>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function loadProducts(ByVal parameters As NameValueCollection) As List(Of IProduct)            
            Dim conTalent As SqlConnection = Nothing
            Dim dtrSearch As SqlDataReader = Nothing

            If Not parameters Is Nothing Then
                Try
                    'Create the select sql command based upon passed in parameters
                    Dim cmdSelect As SqlCommand = getSelectCommand(parameters, conTalent)
                    dtrSearch = cmdSelect.ExecuteReader()

                    'If we have some products then load them into the list otherwise try
                    'without being partner specific
                    If dtrSearch.HasRows Then
                        getProducts(dtrSearch)
                    Else
                        dtrSearch.Close()
                        getProductsForAllPartners(parameters, cmdSelect, dtrSearch)
                    End If
                Catch ex As Exception
                Finally
                    conTalent.Close()
                End Try
            End If
            Return productsList
        End Function


        ' Gets the products for all partners and loads them into the provided list of IProduct objects
        Private Sub getProductsForAllPartners(ByVal parameters As NameValueCollection, ByVal cmdSelect As SqlCommand, ByVal dtrSearch As SqlDataReader)
            Dim s As String = productsList.ToString()
            cmdSelect.Parameters.RemoveAt("@PARTNER")
            cmdSelect.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
            dtrSearch = cmdSelect.ExecuteReader()
            If dtrSearch.HasRows Then
                getProducts(dtrSearch)
            End If
        End Sub

        Private Sub getProducts(ByVal dtrSearch As SqlDataReader)
            Dim productCodes As New Generic.Dictionary(Of String, WebPriceProduct)
            populateProductList(dtrSearch)
            getPrices(dtrSearch)
        End Sub

        Private Sub populateProductList(ByVal dtrSearch As SqlDataReader)            
            While dtrSearch.Read()
                ' Populate details of product object and add to list
                Dim p As IProduct = New ProductImpl
                p = FillDetails(def.NumberOfGroupLevels, dtrSearch)

                ' Make sure it's not already added 
                If Not productCodes.ContainsKey(p.ProductCode) AndAlso p.ProductAvailableOnline Then
                    productsList.Add(p)
                    productCodes.Add(p.ProductCode, New WebPriceProduct(p.ProductCode, 0, p.ProductCode))
                End If
            End While
        End Sub

        Private Sub getPrices(ByVal dtrSearch As SqlDataReader)
            'We don't want to get the prices for each product individually as
            'it is inefficient, so it is done in batch here
            Dim removeProducts As New Generic.List(Of IProduct)
            Dim productPrices As Talent.Common.TalentWebPricing = Talent.eCommerce.Utilities.GetWebPrices_WithPromotionDetails(productCodes)

            For Each p As IProduct In productsList
                Try
                    p.ProductWebPrice = productPrices.RetrievedPrices(p.ProductCode)
                    If productPrices.RetrievedPrices(p.ProductCode).DisplayPrice_From > 0 Then
                        p.ProductSortPrice = productPrices.RetrievedPrices(p.ProductCode).DisplayPrice_From
                    Else
                        p.ProductSortPrice = productPrices.RetrievedPrices(p.ProductCode).DisplayPrice
                    End If
                Catch ex As Exception
                    removeProducts.Add(p)
                End Try
            Next

            If removeProducts.Count > 0 Then
                For Each rp As IProduct In removeProducts
                    productsList.Remove(rp)
                Next
            End If            
        End Sub

        Private Function getSelectCommand(ByVal parameters As NameValueCollection, ByRef conTalent As SqlConnection) As SqlCommand
            Dim selectStatement As String = getLoadProductsSelectString(parameters)
            Dim cmdSelect As SqlCommand = Nothing
            'Open the connection to the front end database
            conTalent = New SqlConnection(frontEndConnectionString)
            conTalent.Open()

            cmdSelect = New SqlCommand(selectStatement, conTalent)
            addParameters(parameters, cmdSelect)
            Return cmdSelect
        End Function

        ''' <summary>
        ''' Constructs the select statement for loading the products.
        ''' 
        ''' TODO - This should be refactored to reduce the duplication
        ''' of logic for each section.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function getLoadProductsSelectString(ByVal parameters As NameValueCollection) As String
            Dim selectString As StringBuilder = New StringBuilder()
            Dim completeSelectString As StringBuilder = New StringBuilder()
            Dim i As Integer = 0

            With selectString
                .Append("SELECT DISTINCT p.PRODUCT_CODE, p.PRODUCT_DESCRIPTION_1, pld.GROSS_PRICE, pld.SALE_GROSS_PRICE,")
                .Append(" p.PRODUCT_DESCRIPTION_2, p.PRODUCT_COUNTRY, p.PRODUCT_COLOUR, p.AVAILABLE_ONLINE, ")
                .Append(" p.PRODUCT_HTML_1,PRODUCT_HTML_2,PRODUCT_HTML_3, p.PRODUCT_PACK_SIZE, p.PRODUCT_DESCRIPTION_5, ")
                .Append(" gp.GROUP_L01_GROUP, gp.GROUP_L02_GROUP, gp.GROUP_L03_GROUP, gp.GROUP_L04_GROUP, gp.GROUP_L05_GROUP,")
                .Append(" gp.GROUP_L06_GROUP, gp.GROUP_L07_GROUP, gp.GROUP_L08_GROUP, gp.GROUP_L09_GROUP, gp.GROUP_L10_GROUP, gp.SEQUENCE,")
                .Append(" CALC_PRICE = CASE WHEN pld.SALE_GROSS_PRICE <> 0 THEN pld.SALE_GROSS_PRICE ELSE pld.GROSS_PRICE END")
                .Append(" FROM tbl_group_product AS gp WITH (NOLOCK)  ")
                .Append(" JOIN tbl_product AS p WITH (NOLOCK)  ON gp.PRODUCT = p.PRODUCT_CODE")
                .Append(" JOIN tbl_price_list_detail AS pld WITH (NOLOCK)  ON gp.PRODUCT = pld.PRODUCT")
                .Append(" WHERE (GP.GROUP_BUSINESS_UNIT = @BUSINESS_UNIT OR GP.GROUP_BUSINESS_UNIT = '*ALL')")
                .Append(" AND gp.GROUP_PARTNER = @PARTNER")
                .Append(" AND pld.PRICE_LIST = @PRICE_LIST")


                If def.ShowOnlyMasterProductsOnSearchResults Then
                    .Append(" AND p.PRODUCT_OPTION_MASTER = 'True'")
                End If

                ' Groups
                For i = 1 To 10
                    If i = 10 Then
                        If Not parameters.Item("group10") Is Nothing Then
                            .Append(" AND gp.GROUP_L10_GROUP = @GROUP10")
                        End If
                    Else
                        If Not parameters.Item("group0" & i.ToString) Is Nothing Then
                            .Append(" AND gp.GROUP_L0" & i.ToString & "_GROUP = @GROUP0" & i.ToString)
                        End If
                    End If
                Next

                ' Range - Used as exact values
                For i = 1 To 5
                    If Not parameters.Item("range0" & i.ToString) Is Nothing Then
                        .Append(" AND p.PRODUCT_SEARCH_RANGE_0" & i.ToString & " = @VALUE0" & i.ToString)
                    End If
                Next

                ' Range - Used as range
                For i = 1 To 5
                    If Not parameters.Item("rangefr0" & i.ToString) Is Nothing And Not parameters.Item("rangeto0" & i.ToString) Is Nothing Then
                        .Append(" AND p.PRODUCT_SEARCH_RANGE_0" & i.ToString & " BETWEEN @RANGEFR0" & i.ToString & " AND @RANGETO0" & i.ToString)
                    End If
                Next
                ' Criteria
                For i = 1 To 20
                    If i < 10 Then
                        If Not parameters.Item("criteria0" & i.ToString) Is Nothing Then
                            .Append(" AND p.PRODUCT_SEARCH_CRITERIA_0" & i.ToString & " = @CRITERIA0" & i.ToString)
                        End If
                    Else
                        If Not parameters.Item("criteria" & i.ToString) Is Nothing Then
                            .Append(" AND p.PRODUCT_SEARCH_CRITERIA_" & i.ToString & " = @CRITERIA" & i.ToString)
                        End If
                    End If
                Next
                ' Switches
                For i = 1 To 10
                    If i = 10 Then
                        If Not parameters.Item("switch10") Is Nothing Then
                            .Append(" AND p.PRODUCT_SEARCH_SWITCH_10" & " = @SWITCH10")
                        End If
                    Else
                        If Not parameters.Item("switch0" & i.ToString) Is Nothing Then
                            .Append(" AND p.PRODUCT_SEARCH_SWITCH_0" & i.ToString & " = @SWITCH0" & i.ToString)
                        End If
                    End If
                Next

                ' Date - Used as exact date
                For i = 1 To 5
                    If Not parameters.Item("date0" & i.ToString) Is Nothing Then
                        .Append(" AND p.PRODUCT_SEARCH_DATE_0" & i.ToString & " = @DATE0" & i.ToString)
                    End If
                Next
                ' Date - Used as range
                For i = 1 To 5
                    If Not parameters.Item("datefr0" & i.ToString) Is Nothing And Not parameters.Item("dateto0" & i.ToString) Is Nothing Then
                        .Append(" AND p.PRODUCT_SEARCH_DATE_0" & i.ToString & " BETWEEN @DATEFR0" & i.ToString & " AND @DATETO0" & i.ToString)
                    End If
                Next
                ' Product Code
                If Not parameters.Item("productcode") Is Nothing Then
                    .Append(" AND gp.PRODUCT = @PRODUCT")
                End If
                ' Keywords
                i = 1
                While Not parameters.Item("keyword" & i.ToString) Is Nothing
                    .Append(" AND p.PRODUCT_SEARCH_KEYWORDS LIKE @KEYWORD" & i.ToString)
                    i += 1
                End While
            End With

            ' Price
            Dim selectPriceString1 As String = "SELECT * FROM ("
            Dim selectPriceString2 As New StringBuilder

            If Not parameters.Item("price") Is Nothing Or (Not parameters.Item("pricefr") Is Nothing) Then

                selectPriceString2.Append(") AS TABLE1 WHERE")
                If Not parameters.Item("price") Is Nothing Then
                    selectPriceString2.Append(" CALC_PRICE = @PRICE")
                End If
                If Not parameters.Item("pricefr") Is Nothing Then
                    If Not parameters.Item("price") Is Nothing Then
                        selectPriceString2.Append(" AND")
                    End If
                    If parameters.Item("priceto") Is Nothing Then
                        selectPriceString2.Append(" CALC_PRICE >= @PRICEFR")
                    Else
                        selectPriceString2.Append(" CALC_PRICE BETWEEN @PRICEFR AND @PRICETO")
                    End If
                End If

                ' Append to front and back of selectString
                With completeSelectString
                    .Append(selectPriceString1)
                    .Append(selectString)
                    .Append(selectPriceString2)
                End With

            Else
                completeSelectString.Append(selectString)
            End If

            Return completeSelectString.ToString()
        End Function

        ''' <summary>
        ''' Adds the parameters to the select command for loading the
        ''' products
        ''' 
        ''' TODO - Refactor to reduce duplication of code and logic.
        ''' </summary>
        ''' <param name="parameters"></param>
        ''' <remarks></remarks>
        Private Sub addParameters(ByVal parameters As NameValueCollection, ByRef cmdSelect As SqlCommand)
            Dim i As Integer = 0
            ' Business Unit
            cmdSelect.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = TalentCache.GetBusinessUnit

            ' Price List
            cmdSelect.Parameters.Add(New SqlParameter("@PRICE_LIST", SqlDbType.Char, 50)).Value = def.PriceList

            ' Groups 
            For i = 1 To 10
                If i = 10 Then
                    If Not parameters.Item("group10") Is Nothing Then
                        cmdSelect.Parameters.Add(New SqlParameter("@GROUP10", SqlDbType.Char, 50)).Value = parameters.Item("group10")
                    End If
                Else
                    If Not parameters.Item("group0" & i.ToString) Is Nothing Then
                        cmdSelect.Parameters.Add(New SqlParameter("@GROUP0" & i.ToString, SqlDbType.Char, 50)).Value = parameters.Item("group0" & i.ToString)
                    End If
                End If
            Next
            ' Range - Used as exact values
            For i = 1 To 5
                If Not parameters.Item("range0" & i.ToString) Is Nothing Then
                    Try
                        cmdSelect.Parameters.Add(New SqlParameter("@VALUE0" & i.ToString, SqlDbType.Decimal)).Value = parameters.Item("range0" & i.ToString)
                    Catch ex As Exception
                    End Try
                End If
            Next
            ' Range - Used as range
            For i = 1 To 5
                If Not parameters.Item("rangefr0" & i.ToString) Is Nothing And Not parameters.Item("rangeto0" & i.ToString) Is Nothing Then
                    Try
                        cmdSelect.Parameters.Add(New SqlParameter("@RANGEFR0" & i.ToString, SqlDbType.Decimal)).Value = parameters.Item("rangefr0" & i.ToString)
                        cmdSelect.Parameters.Add(New SqlParameter("@RANGETO0" & i.ToString, SqlDbType.Decimal)).Value = parameters.Item("rangeto0" & i.ToString)
                    Catch ex As Exception
                    End Try
                End If
            Next
            ' Criteria
            For i = 1 To 20
                If i < 10 Then
                    If Not parameters.Item("criteria0" & i.ToString) Is Nothing Then
                        cmdSelect.Parameters.Add(New SqlParameter("@CRITERIA0" & i.ToString, SqlDbType.Char, 50)).Value = parameters.Item("criteria0" & i.ToString)
                    End If
                Else
                    If Not parameters.Item("criteria" & i.ToString) Is Nothing Then
                        cmdSelect.Parameters.Add(New SqlParameter("@CRITERIA" & i.ToString, SqlDbType.Char, 50)).Value = parameters.Item("criteria" & i.ToString)
                    End If
                End If
            Next
            ' Switches
            For i = 1 To 10
                If i = 10 Then
                    If Not parameters.Item("switch10") Is Nothing Then
                        Try
                            cmdSelect.Parameters.Add(New SqlParameter("@SWITCH10", SqlDbType.Bit)).Value = Convert.ToBoolean(parameters.Item("SWITCH10"))
                        Catch ex As Exception
                        End Try
                    End If
                Else
                    If Not parameters.Item("switch0" & i.ToString) Is Nothing Then
                        Try
                            cmdSelect.Parameters.Add(New SqlParameter("@SWITCH0" & i.ToString, SqlDbType.Bit)).Value = Convert.ToBoolean(parameters.Item("switch0" & i.ToString))
                        Catch ex As Exception
                        End Try
                    End If
                End If
            Next
            ' Date - Used as exact date. Should be in format yyyy-mm-dd
            For i = 1 To 5
                If Not parameters.Item("date0" & i.ToString) Is Nothing Then
                    Try
                        cmdSelect.Parameters.Add(New SqlParameter("@DATE0" & i.ToString, SqlDbType.DateTime)).Value = Convert.ToDateTime(parameters.Item("date0" & i.ToString))
                    Catch ex As Exception
                    End Try
                End If
            Next
            ' Date - Used as range
            For i = 1 To 5
                If Not parameters.Item("datefr0" & i.ToString) Is Nothing And Not parameters.Item("dateto0" & i.ToString) Is Nothing Then
                    Try
                        cmdSelect.Parameters.Add(New SqlParameter("@DATEFR0" & i.ToString, SqlDbType.DateTime)).Value = Convert.ToDateTime(parameters.Item("datefr0" & i.ToString))
                        cmdSelect.Parameters.Add(New SqlParameter("@DATETO0" & i.ToString, SqlDbType.DateTime)).Value = Convert.ToDateTime(parameters.Item("dateto0" & i.ToString))
                    Catch ex As Exception
                    End Try
                End If
            Next
            ' Price - Used as exact price 
            If Not parameters.Item("price") Is Nothing Then
                Try
                    cmdSelect.Parameters.Add(New SqlParameter("@PRICE", SqlDbType.Decimal)).Value = parameters.Item("price")
                Catch ex As Exception
                End Try
            End If
            ' Price - Used as range
            If Not parameters.Item("pricefr") Is Nothing Then
                Try
                    cmdSelect.Parameters.Add(New SqlParameter("@PRICEFR", SqlDbType.Decimal)).Value = parameters.Item("pricefr")
                Catch ex As Exception
                End Try
            End If
            If Not parameters.Item("priceto") Is Nothing Then
                Try
                    cmdSelect.Parameters.Add(New SqlParameter("@PRICETO", SqlDbType.Decimal)).Value = parameters.Item("priceto")
                Catch ex As Exception
                End Try
            End If
            ' Product Code
            If Not parameters.Item("productcode") Is Nothing Then
                cmdSelect.Parameters.Add(New SqlParameter("@PRODUCT", SqlDbType.Char, 50)).Value = parameters.Item("productcode")
            End If
            ' Keywords
            i = 1
            While Not parameters.Item("keyword" & i.ToString) Is Nothing
                cmdSelect.Parameters.Add(New SqlParameter("@KEYWORD" & i.ToString, SqlDbType.NVarChar, 500)).Value = "%" & parameters.Item("keyword" & i.ToString).Trim & "%"
                i += 1
            End While

            ' Try default partner first
            cmdSelect.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
        End Sub

        Public Function FillDetails(ByVal intMaxNoOfGroupLevels As Integer, ByVal dtrSearch As SqlDataReader) As IProduct

            Dim p As IProduct = New ProductImpl()
            p.ProductCode = dtrSearch("PRODUCT_CODE")
            p.ProductDescription1 = dtrSearch("PRODUCT_DESCRIPTION_1")
            p.ProductDescription2 = dtrSearch("PRODUCT_DESCRIPTION_2")
            p.ProductCountry = dtrSearch("PRODUCT_COUNTRY")
            p.ProductColour = dtrSearch("PRODUCT_COLOUR")
            p.ProductSequenceNumber = dtrSearch("SEQUENCE")
            p.ProductGroup1 = dtrSearch("GROUP_L01_GROUP")
            p.ProductGroup2 = dtrSearch("GROUP_L02_GROUP")
            p.ProductGroup3 = dtrSearch("GROUP_L03_GROUP")
            p.ProductGroup4 = dtrSearch("GROUP_L04_GROUP")
            p.ProductGroup5 = dtrSearch("GROUP_L05_GROUP")
            p.ProductGroup6 = dtrSearch("GROUP_L06_GROUP")
            p.ProductGroup7 = dtrSearch("GROUP_L07_GROUP")
            p.ProductGroup8 = dtrSearch("GROUP_L08_GROUP")
            p.ProductGroup9 = dtrSearch("GROUP_L09_GROUP")
            p.ProductGroup10 = dtrSearch("GROUP_L10_GROUP")
            p.ProductAvailableOnline = dtrSearch("AVAILABLE_ONLINE")

            '     If hasHTMLContent Then
            p.ProductHTML1 = Talent.eCommerce.Utilities.CheckForDBNull_String(dtrSearch("PRODUCT_HTML_1"))
            p.ProductHTML2 = Talent.eCommerce.Utilities.CheckForDBNull_String(dtrSearch("PRODUCT_HTML_2"))
            p.ProductHTML3 = Talent.eCommerce.Utilities.CheckForDBNull_String(dtrSearch("PRODUCT_HTML_3"))
            p.ProductPackSize = Talent.eCommerce.Utilities.CheckForDBNull_String(dtrSearch("PRODUCT_PACK_SIZE"))
            p.ProductBrand = Talent.eCommerce.Utilities.CheckForDBNull_String(dtrSearch("PRODUCT_DESCRIPTION_5"))
            '    End If

            ' Find list image
            p.ProductImagePath = ImagePath.getImagePath("PRODLIST", p.ProductCode, _businessUnit, _partner)

            If p.ProductImagePath <> String.Empty And p.ProductImagePath <> def.RetailMissingImagePath Then
                p.ProductAltText = p.ProductDescription1
            Else
                p.ProductAltText = String.Empty
            End If

            ' Build navigate query string                   
            p.ProductNavigateURL = p.generateQueryString(intMaxNoOfGroupLevels)
            p.ProductLinkEnabled = Not def.SuppressProductLinks
            Return p
        End Function

        Public Function sortList(ByVal sortBy As String, ByVal order As String, ByVal productList As List(Of IProduct)) As List(Of IProduct)
            If order = "A" Then
                productList.Sort(New GenericComparer(Of IProduct)(sortBy, GenericComparer(Of Global.IProduct).SortOrder.Ascending))
            Else
                productList.Sort(New GenericComparer(Of IProduct)(sortBy, GenericComparer(Of Global.IProduct).SortOrder.Descending))
            End If
            Return productList
        End Function

        Public Function getAsProductListGen() As ProductListGen
            Dim products As ProductListGen = New ProductListGen()
            For Each prod As IProduct In productsList
                products.Add(convertIProductToProduct(prod))
            Next
            Return products
        End Function

        Public Function convertIProductToProduct(ByVal iproduct As IProduct) As Product
            Dim product As Product = New Product
            product.Code = iproduct.ProductCode
            product.Description1 = iproduct.ProductDescription1
            product.Description2 = iproduct.ProductDescription2
            product.NavigateURL = iproduct.ProductNavigateURL
            product.Country = iproduct.ProductCountry
            product.Colour = iproduct.ProductColour
            product.SequenceNo = iproduct.ProductSequenceNumber
            product.ImagePath = iproduct.ProductImagePath
            product.Group1 = iproduct.ProductGroup1
            product.Group2 = iproduct.ProductGroup2
            product.Group3 = iproduct.ProductGroup3
            product.Group4 = iproduct.ProductGroup4
            product.Group5 = iproduct.ProductGroup5
            product.Group6 = iproduct.ProductGroup6
            product.Group7 = iproduct.ProductGroup7
            product.Group8 = iproduct.ProductGroup8
            product.Group9 = iproduct.ProductGroup9
            product.Group10 = iproduct.ProductGroup10
            product.AltText = iproduct.ProductAltText
            product.AvailableOnline = iproduct.ProductAvailableOnline
            product.LinkEnabled = iproduct.ProductLinkEnabled
            product.HTML_1 = iproduct.ProductHTML1
            product.HTML_2 = iproduct.ProductHTML2
            product.HTML_3 = iproduct.ProductHTML3
            product.Brand = iproduct.ProductBrand
            product.PackSize = iproduct.ProductPackSize
            product.PriceForSorting = iproduct.ProductSortPrice
            product.WebPrices = iproduct.ProductWebPrice
            product.IsSalePrice = iproduct.ProductIsSalePrice
            Return product
        End Function

    End Class
End Namespace
