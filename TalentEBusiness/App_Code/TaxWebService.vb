Imports Microsoft.VisualBasic
Imports System.Data

Namespace Talent.eCommerce

    Public Class TaxWebService

        Private _inHeader As DataTable
        Public Property InHeader() As DataTable
            Get
                Return _inHeader
            End Get
            Set(ByVal value As DataTable)
                _inHeader = value
            End Set
        End Property

        Private _inDetail As DataTable
        Public Property InDetail() As DataTable
            Get
                Return _inDetail
            End Get
            Set(ByVal value As DataTable)
                _inDetail = value
            End Set
        End Property

        Private _orderHeader As DataTable
        Public Property OrderHeader() As DataTable
            Get
                Return _orderHeader
            End Get
            Set(ByVal value As DataTable)
                _orderHeader = value
            End Set
        End Property

        Private _orderDetail As DataTable
        Public Property OrderDetail() As DataTable
            Get
                Return _orderDetail
            End Get
            Set(ByVal value As DataTable)
                _orderDetail = value
            End Set
        End Property

        Private _promoCode As String
        Public Property PromotionCode() As String
            Get
                Return _promoCode
            End Get
            Set(ByVal value As String)
                _promoCode = value
            End Set
        End Property


        Private Sub NewInHeaderTable()
            Dim dt As New DataTable("Address")
            With dt.Columns
                .Add(New System.Data.DataColumn("CustomerCode", GetType(String)))
                .Add(New System.Data.DataColumn("BasketNo", GetType(String)))
                .Add(New System.Data.DataColumn("From_Line1", GetType(String)))
                .Add(New System.Data.DataColumn("From_Line2", GetType(String)))
                .Add(New System.Data.DataColumn("From_City", GetType(String)))
                .Add(New System.Data.DataColumn("From_Region", GetType(String)))
                .Add(New System.Data.DataColumn("From_PostCode", GetType(String)))
                .Add(New System.Data.DataColumn("From_Country", GetType(String)))
                .Add(New System.Data.DataColumn("To_Line1", GetType(String)))
                .Add(New System.Data.DataColumn("To_Line2", GetType(String)))
                .Add(New System.Data.DataColumn("To_City", GetType(String)))
                .Add(New System.Data.DataColumn("To_Region", GetType(String)))
                .Add(New System.Data.DataColumn("To_PostCode", GetType(String)))
                .Add(New System.Data.DataColumn("To_Country", GetType(String)))
                .Add(New System.Data.DataColumn("Delivery_Charge", GetType(Decimal)))
                .Add(New System.Data.DataColumn("Promotion_Code", GetType(String)))
                .Add(New System.Data.DataColumn("Order_Status", GetType(String)))
                .Add(New System.Data.DataColumn("Culture_Code", GetType(String)))
            End With

            InHeader = dt
        End Sub

        Private Sub NewInDetailTable()
            Dim dt As New DataTable("Product")
            With dt.Columns
                .Add(New System.Data.DataColumn("LineNo", GetType(String)))
                .Add(New System.Data.DataColumn("SKUCode", GetType(String)))
                .Add(New System.Data.DataColumn("TariffCode", GetType(String)))
                .Add(New System.Data.DataColumn("Quantity", GetType(Integer)))
                .Add(New System.Data.DataColumn("Price", GetType(Decimal)))
                .Add(New System.Data.DataColumn("Weight", GetType(Decimal)))
                .Add(New System.Data.DataColumn("CountryCode", GetType(String)))
                .Add(New System.Data.DataColumn("TaxCode", GetType(String)))
                '.Add(New System.Data.DataColumn("Include", GetType(String)))
                .Add(New System.Data.DataColumn("DutyPrice", GetType(Decimal)))
            End With

            InDetail = dt
        End Sub

        Public Function CallTaxWebService(ByVal source As String) As DataSet
            Return CallTaxWebService(source, "", "")
        End Function

        ''' <summary>
        ''' Calls the Tax Calculator web service and returns the resulting dataset
        ''' </summary>
        ''' <param name="source">The source to use to get the products for the calculations
        ''' Options: BASKET = the current user's basket, ORDER = a current or previous order (must supply TempOrderID)
        ''' </param>
        ''' <param name="tempOrderID">If the source is set to ORDER then a TempOrderID needs to be supplied</param>
        ''' <param name="promoCode">A user supplied promotional code</param>
        ''' <returns>The resulting DataSet from the web service</returns>
        ''' <remarks></remarks>
        Public Function CallTaxWebService(ByVal source As String, _
                                            ByVal tempOrderID As String, _
                                            ByVal promoCode As String) As DataSet

            PromotionCode = promoCode


            Dim inDS As New DataSet, _
                outDS As New DataSet, _
                taxWS As New com.dunhill.teststore.Service
            'taxWS As New com.dunhill.store.Service

            Dim cred As New System.Net.NetworkCredential
            cred.UserName = "testcsgadmin"
            cred.Password = "testcsg1an"
            taxWS.Credentials = cred

            If Not String.IsNullOrEmpty(tempOrderID) Then
                Dim orderHeaderTA As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                Dim orderDetailTA As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
                OrderHeader = orderHeaderTA.Get_Header_By_Temp_Order_Id(tempOrderID)
                OrderDetail = orderDetailTA.Get_Order_Lines(tempOrderID)
            End If

            Dim success As Boolean = False

            If CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems.Count > 0 Then
                inDS = GetInDataSet(source, tempOrderID)
                Try
                    outDS = taxWS.CalculateTax(inDS)
                Catch ex As Exception
                    'error calling webservice or internal web service error
                    'HttpContext.Current.Response.Write(inDS.GetXml.ToString)
                    success = False
                End Try
            End If

            Try
                success = CBool(outDS.Tables(0).Rows(0)("Success"))
                If CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems.Count > outDS.Tables(1).Rows.Count Then
                    success = False
                End If
            Catch ex As Exception
                success = False
            End Try

            If success = False Then
                If Not HttpContext.Current.Profile.IsAnonymous Then
                    Logging.WriteLog(HttpContext.Current.Profile.UserName, _
                                        "ADETAXCALC", _
                                        "TAX CALCULATOR ERROR" & vbCrLf & _
                                            "In DataSet:" & vbCrLf & _
                                            inDS.GetXml & vbCrLf & _
                                            "Out DataSet:" & vbCrLf & outDS.GetXml, _
                                        "", _
                                        TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
                End If
            End If

            'Dim taxHeader As DataTable = outDS.Tables(0)
            'Returned Header Table Columns
            '-----------------------------
            'DeliveryGROSS
            'DeliveryTAX
            'DeliveryNET
            'GoodsTotalNet
            'TotalGross
            'TotalTax1
            'TaxTypeCode1
            'DisplayTax1
            'DisplayInclusive1
            'TotalTax2
            'TaxTypeCode2
            'DisplayTax2
            'DisplayInclusive2
            'TotalTax3
            'TaxTypeCode3
            'DisplayTax3
            'DisplayInclusive3
            'TotalTax4
            'TaxTypeCode4
            'DisplayTax4
            'DisplayInclusive4
            'TotalTax5
            'TaxTypeCode5
            'DisplayTax5
            'DisplayInclusive5
            'PromotionalCode
            'DiscountDescription
            'DiscountValue
            'DiscountErrorFlag
            'DiscountErrorDesc
            'DisplayPromAmount


            'Dim taxDetails As DataTable = outDS.Tables(1)
            'Returned Detail Table Columns
            '-----------------------------
            'SKUCODE
            'TariffCode
            'NetAmount
            'GrossAmount
            'taxAmount1
            'taxAmount2
            'taxAmount3
            'taxAmount4
            'taxAmount5
            'Quantity
            'Group
            'Category
            'SubCategory

            If HttpContext.Current.Session.Item("DunhillWSError") Is Nothing Then
                HttpContext.Current.Session.Add("DunhillWSError", Not success)
            Else
                HttpContext.Current.Session.Item("DunhillWSError") = Not success
            End If


            Return outDS
        End Function


        Private Function GetInDataSet(ByVal source As String, ByVal tempOrderID As String) As DataSet
            Dim results As New DataSet

            Dim defaults As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults

            NewInHeaderTable()
            NewInDetailTable()

            Dim basketAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter 
            Dim liveBasketItems As Data.DataTable = basketAdapter.GetBasketItems_ByHeaderID_NonTicketing( _
                                                       CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)

            Dim qtyAndCodes As New Generic.Dictionary(Of String, Talent.Common.WebPriceProduct)
            For Each bItem As Data.DataRow In liveBasketItems.Rows
                If Not Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(bItem("IS_FREE")) Then
                    qtyAndCodes.Add(Utilities.CheckForDBNull_String(bItem("PRODUCT")), _
                                            New Talent.Common.WebPriceProduct(Utilities.CheckForDBNull_String(bItem("PRODUCT")), _
                                                                                Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), _
                                                                                Utilities.CheckForDBNull_String(bItem("MASTER_PRODUCT"))))
                End If
            Next

            Dim productPrices As Talent.Common.TalentWebPricing = Utilities.GetWebPrices_WithTotals_CalledFromTaxWS(qtyAndCodes, tempOrderID, defaults.PromotionPriority, PromotionCode)

            'Add the header
            '-------------------------
            Dim hr As DataRow = InHeader.NewRow
            hr("CustomerCode") = "PreLogin"
            hr("BasketNo") = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
            hr("From_Line1") = ""
            hr("From_Line2") = ""
            hr("From_City") = ""
            hr("From_Region") = ""
            hr("From_PostCode") = ""
            hr("From_Country") = "USA" 'TalentCache.GetBusinessUnit
            hr("To_Line1") = ""
            hr("To_Line2") = ""
            hr("To_City") = ""
            hr("To_Region") = ""
            hr("To_PostCode") = ""
            hr("To_Country") = "USA" 'TalentCache.GetBusinessUnit
            If productPrices.Qualifies_For_Free_Delivery Then
                hr("Delivery_Charge") = 0
            Else
                hr("Delivery_Charge") = productPrices.Max_Delivery_Gross
            End If
            hr("Promotion_Code") = ""
            hr("Order_Status") = ""
            hr("Culture_Code") = ""

            If Not productPrices.Qualifies_For_Free_Delivery _
                AndAlso Not String.IsNullOrEmpty(tempOrderID) _
                    AndAlso UCase(source) = "ORDER" _
                        AndAlso OrderHeader.Rows.Count > 0 Then
                hr("Delivery_Charge") = Utilities.CheckForDBNull_Decimal(OrderHeader.Rows(0)("TOTAL_DELIVERY_GROSS"))
            End If

            If Not HttpContext.Current.Profile.IsAnonymous Then
                hr("CustomerCode") = HttpContext.Current.Profile.UserName
                hr("From_Line1") = ""
                hr("From_Line2") = ""
                hr("From_City") = ""
                hr("From_Region") = ""
                hr("From_PostCode") = ""
                hr("From_Country") = TalentCache.GetBusinessUnit
                hr("To_Line1") = ""
                hr("To_Line2") = ""
                hr("To_City") = ""
                hr("To_Region") = ""
                hr("To_PostCode") = ""
                hr("To_Country") = TalentCache.GetBusinessUnit
                hr("Promotion_Code") = ""
                hr("Order_Status") = ""
                hr("Culture_Code") = ""
            End If

            InHeader.Rows.Add(hr)
            '---------------------------


            'Add the details
            '---------------------------


            If Not productPrices Is Nothing Then



                liveBasketItems = basketAdapter.GetBasketItems_ByHeaderID_NonTicketing( _
                                                                             CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
                Dim productInfo As DataTable = GetProductInformation(source, tempOrderID, liveBasketItems)

                Select Case UCase(source)
                    Case Is = "BASKET"

                        Do_BasketItems_Add(liveBasketItems, productPrices, productInfo, tempOrderID, defaults.PromotionPriority)

                    Case Is = "ORDER"

                        Do_OrderItems_Add(liveBasketItems, productPrices, productInfo, tempOrderID, defaults.PromotionPriority)

                End Select

            End If
            '------------------------

            results.Tables.Add(InHeader)
            results.Tables.Add(InDetail)


            'Mop any remaining promotions that are not Product Specific and trribute them to the
            'products based on the highest price first
            If Not productPrices.PromotionsResultsTable Is Nothing AndAlso productPrices.PromotionsResultsTable.Rows.Count > 0 Then
                Dim orderedRows As DataRow() = InDetail.Select("1=1", "Price Desc")
                For Each promo As DataRow In productPrices.PromotionsResultsTable.Rows
                    If CBool(promo("Success")) Then
                        For Each detail As DataRow In orderedRows
                            If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(promo("ProductCodes"))) Then
                                If CDec(promo("PromotionValue")) <= CDec(detail("Price")) Then
                                    detail("Price") = CDec(detail("Price")) - CDec(promo("PromotionValue"))
                                    promo("PromotionValue") = 0
                                    Exit For
                                ElseIf CDec(promo("PromotionValue")) > CDec(detail("Price")) Then
                                    promo("PromotionValue") = CDec(promo("PromotionValue")) - CDec(detail("Price"))
                                    detail("Price") = 0
                                End If
                            End If
                        Next
                    End If
                Next

            End If


            Return results
        End Function

        Protected Sub Do_OrderItems_Add(ByVal liveBasket As DataTable, _
                                        ByVal productPrices As Talent.Common.TalentWebPricing, _
                                        ByVal productInfo As DataTable, _
                                        ByVal tempOrderID As String, _
                                        ByVal promoPriority As String)


            Dim newRow As DataRow, _
               count As Integer = 0, _
               price As Decimal = 0, _
               dutyPrice As Decimal = 0


            If OrderDetail.Rows.Count > 0 Then
                For Each oItem As DataRow In OrderDetail.Rows
                    Dim oCode As String = Utilities.CheckForDBNull_String(oItem("PRODUCT_CODE"))
                    Dim oQty As Decimal = Utilities.CheckForDBNull_Decimal(oItem("QUANTITY"))
                    Dim product As New Talent.Common.DEWebPrice

                    'try to retrieve the product from the product prices that were retreived
                    '-------------------------------------------------------------------------
                    If productPrices.RetrievedPrices(oCode) Is Nothing Then
                        'if no record found, try to get the record individually
                        Try
                            product = Utilities.GetWebPrices_WithTotals(oCode, _
                                                                        oQty, _
                                                                        tempOrderID, _
                                                                        promoPriority, _
                                                                        oItem("MASTER_PRODUCT"), _
                                                                        PromotionCode).RetrievedPrices(oCode)
                        Catch ex As Exception
                        End Try
                    Else
                        product = productPrices.RetrievedPrices(oCode)
                    End If

                    If oQty > 0 Then
                        Dim found As Boolean = False
                        For Each bItem As DataRow In liveBasket.Rows
                            Dim bCode As String = Utilities.CheckForDBNull_String(bItem("PRODUCT"))
                            Dim bQty As Decimal = Utilities.CheckForDBNull_Decimal(bItem("QUANTITY"))
                            Dim bIsFree As Boolean = Utilities.CheckForDBNull_Boolean_DefaultFalse(bItem("IS_FREE"))

                            If UCase(oCode) = UCase(bCode) Then
                                dutyPrice = product.Purchase_Price_Gross * product.RequestedQuantity
                                If bIsFree Then
                                    price = 0
                                Else
                                    price = product.PromotionInclusivePriceGross * product.RequestedQuantity
                                End If
                                found = True
                                Exit For
                            End If
                        Next

                        If found Then

                            'add the row to the web service detail table
                            '-------------------------------------------------------------------------
                            Dim infoRow As DataRow = (New DataTable).NewRow
                            For Each rw As DataRow In productInfo.Rows
                                If UCase(Utilities.CheckForDBNull_String(rw("PRODUCT_CODE"))) = UCase(oCode) Then
                                    infoRow = rw
                                    Exit For
                                End If
                            Next

                            If Not infoRow Is Nothing Then
                                count += 1
                                newRow = InDetail.NewRow
                                newRow("LineNo") = count.ToString
                                newRow("SKUCode") = oCode
                                newRow("TariffCode") = product.TARIFF_CODE
                                newRow("Quantity") = CInt(Math.Floor(oQty))
                                newRow("Price") = price
                                newRow("Weight") = Utilities.CheckForDBNull_Decimal(infoRow("PRODUCT_WEIGHT"))
                                newRow("CountryCode") = Utilities.GetCountryCode()
                                newRow("TaxCode") = product.TAX_CODE
                                newRow("DutyPrice") = dutyPrice

                                InDetail.Rows.Add(newRow)
                            End If
                            '-------------------------------------------------------------------------
                        End If


                    End If


                Next
            End If
        End Sub


        Protected Sub Do_BasketItems_Add(ByVal liveBasket As DataTable, _
                                            ByVal productPrices As Talent.Common.TalentWebPricing, _
                                            ByVal productInfo As DataTable, _
                                            ByVal tempOrderID As String, _
                                            ByVal promoPriority As String)

            Dim newRow As DataRow, _
                count As Integer = 0, _
                price As Decimal = 0, _
                dutyPrice As Decimal = 0

            For Each item As DataRow In liveBasket.Rows
                Dim pCode As String = Utilities.CheckForDBNull_String(item("PRODUCT"))
                Dim pQty As Decimal = Utilities.CheckForDBNull_Decimal(item("QUANTITY"))
                Dim pIsFree As Boolean = Utilities.CheckForDBNull_Boolean_DefaultFalse(item("IS_FREE"))
                Dim product As New Talent.Common.DEWebPrice

                'try to retrieve the product from the product prices that were retreived
                '-------------------------------------------------------------------------
                If productPrices.RetrievedPrices(pCode) Is Nothing Then
                    'if no record found, try to get the record individually
                    Try
                        product = Utilities.GetWebPrices_WithTotals(pCode, _
                                                                    pQty, _
                                                                    tempOrderID, _
                                                                    promoPriority, _
                                                                    item("MASTER_PRODUCT"), _
                                                                    PromotionCode).RetrievedPrices(pCode)
                    Catch ex As Exception
                    End Try
                Else
                    product = productPrices.RetrievedPrices(pCode)
                End If
                '-------------------------------------------------------------------------

                'set the duty price and the selling price
                '-------------------------------------------------------------------------
                dutyPrice = product.Purchase_Price_Gross * product.RequestedQuantity
                If pIsFree Then
                    price = 0
                Else
                    price = product.PromotionInclusivePriceGross * product.RequestedQuantity
                End If
                '-------------------------------------------------------------------------

                'add the row to the web service detail table
                '-------------------------------------------------------------------------
                Dim infoRow As DataRow = (New DataTable).NewRow
                For Each rw As DataRow In productInfo.Rows
                    If UCase(Utilities.CheckForDBNull_String(rw("PRODUCT_CODE"))) = UCase(pCode) Then
                        infoRow = rw
                        Exit For
                    End If
                Next

                If Not infoRow Is Nothing Then
                    count += 1
                    newRow = InDetail.NewRow
                    newRow("LineNo") = count.ToString
                    newRow("SKUCode") = pCode
                    newRow("TariffCode") = product.TARIFF_CODE
                    newRow("Quantity") = CInt(Math.Floor(pQty))
                    newRow("Price") = price
                    newRow("Weight") = Utilities.CheckForDBNull_Decimal(infoRow("PRODUCT_WEIGHT"))
                    newRow("CountryCode") = Utilities.GetCountryCode()
                    newRow("TaxCode") = product.TAX_CODE
                    newRow("DutyPrice") = dutyPrice

                    InDetail.Rows.Add(newRow)
                End If
                '-------------------------------------------------------------------------


            Next
        End Sub


        Private Function GetProductInformation(ByVal source As String, _
                                                ByVal tempOrderID As String, _
                                                Optional ByVal liveBasket As DataTable = Nothing) As DataTable
            Dim err As Talent.Common.ErrorObj
            Dim products As DataTable
            Dim productCodes As New ArrayList

            'Populate the list of product codes
            '-------------------------------------
            Select Case UCase(source)
                Case Is = "BASKET"
                    If Not liveBasket Is Nothing AndAlso liveBasket.Rows.Count > 0 Then
                        For Each item As DataRow In liveBasket.Rows
                            Dim pCode As String = Utilities.CheckForDBNull_String(item("PRODUCT"))
                            productCodes.Add(pCode)
                        Next
                    End If

                Case Is = "ORDER"
                    If OrderDetail.Rows.Count > 0 Then
                        For Each dr As DataRow In OrderDetail.Rows
                            If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("PRODUCT_CODE"))) Then
                                productCodes.Add(dr("PRODUCT_CODE"))
                            End If
                        Next
                    End If
            End Select



            Dim prodInfo As New Talent.Common.DEProductInfo(TalentCache.GetBusinessUnit, _
                                                                 TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                 productCodes, _
                                                                 Talent.Common.Utilities.GetDefaultLanguage)

            Dim DBProdInfo As New Talent.Common.DBProductInfo(prodInfo)
            DBProdInfo.Settings = Talent.eCommerce.Utilities.GetSettingsObject()

            'Get the product info
            '------------------------
            err = DBProdInfo.AccessDatabase()

            If Not err.HasError Then
                products = DBProdInfo.ResultDataSet.Tables("ProductInformation")
            Else
                'ERROR: could not retrieve product info
                products = Nothing
            End If

            Return products
        End Function



    End Class
End Namespace
