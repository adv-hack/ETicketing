Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports System.Collections.Generic
Imports System
Imports System.Web
Imports System.Configuration
Imports System.Web.Caching
Imports System.Text


Public Class TrackingCodes

    Public Shared Function DoInsertTrackingCodes(ByVal location As String) As String
        Dim providers As DataTable = GetTrackingCodeProviders()
        Dim codes As New DataTable, settings As New DataTable
        Dim returnStr As String = ""
        Try
            If location.Equals("HEADER") Then
                returnStr += BuildTrackingScripts()
            End If
            For Each provider As DataRow In providers.Rows
                codes = GetTrackingCodeScript(location, Utilities.CheckForDBNull_String(provider("TRACKING_PROVIDER")))
                If codes.Rows.Count > 0 Then
                    settings = GetTrackingCodeProviderSettings(Utilities.CheckForDBNull_String(provider("TRACKING_PROVIDER")))
                    For Each rw As DataRow In codes.Rows
                        If Not String.IsNullOrEmpty(returnStr) Then returnStr += vbCrLf
                        If provider("TRACKING_PROVIDER").Equals("GOOGLEV3") Then

                        Else
                            returnStr += ReplacePlaceholders(rw("TRACKING_CONTENT"), settings, Utilities.CheckForDBNull_String(provider("TRACKING_PROVIDER")))
                        End If

                    Next
                End If
            Next
        Catch ex As Exception
        End Try

        returnStr = returnStr & getUserTrackingDetails(location)
        Return returnStr
    End Function

    Public Shared Function GetTrackingCodeProviders() As DataTable
        Dim dt As New DataTable
        Dim cacheKey As String = "TrackingProvidersCache_" & TalentCache.GetBusinessUnit

        If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Const SelectStr As String = "SELECT * FROM tbl_tracking_providers WITH (NOLOCK)  WHERE BUSINESS_UNIT = @BUSINESS_UNIT AND PARTNER = @PARTNER "
            cmd.CommandText = SelectStr
            With cmd.Parameters
                .Clear()
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
            End With
            Dim da As New SqlDataAdapter(cmd)

            Try
                cmd.Connection.Open()

                'try specific bu, specific partner and specific page
                da.Fill(dt)

                'try specific bu, specific partner and ANY_page
                If Not dt.Rows.Count > 0 Then
                    cmd.Parameters.Item("@PARTNER").Value = Utilities.GetAllString
                    da.Fill(dt)

                    'try specific bu, ANY_partner and specific page
                    If Not dt.Rows.Count > 0 Then
                        cmd.Parameters.Item("@BUSINESS_UNIT").Value = Utilities.GetAllString
                        da.Fill(dt)
                    End If
                End If

            Catch ex As Exception
            Finally
                cmd.Connection.Close()
            End Try
            TalentCache.AddPropertyToCache(cacheKey, dt, CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes")), TimeSpan.Zero, CacheItemPriority.Normal)
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
        Else
            dt = CType(HttpContext.Current.Cache.Item(cacheKey), DataTable)
        End If
        Return dt
    End Function

    Public Shared Function GetTrackingCodeProviderSettings(ByVal provider As String) As DataTable
        Dim dt As New DataTable
        Dim cacheKey As String = "TrackingSettingsCache_" & TalentCache.GetBusinessUnit & "_" & provider

        If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Const SelectStr As String = "SELECT * FROM tbl_tracking_settings_values WITH (NOLOCK)  " & _
                                        " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                        " AND PARTNER = @PARTNER " & _
                                        " AND TRACKING_PROVIDER = @TRACKING_PROVIDER "

            cmd.CommandText = SelectStr
            With cmd.Parameters
                .Clear()
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                .Add("@TRACKING_PROVIDER", SqlDbType.NVarChar).Value = provider
            End With
            Dim da As New SqlDataAdapter(cmd)

            Try
                cmd.Connection.Open()

                'try specific bu, specific partner and specific page
                da.Fill(dt)

                'try specific bu, specific partner and ANY_page
                If Not dt.Rows.Count > 0 Then
                    cmd.Parameters.Item("@PARTNER").Value = Utilities.GetAllString
                    da.Fill(dt)

                    'try specific bu, ANY_partner and specific page
                    If Not dt.Rows.Count > 0 Then
                        cmd.Parameters.Item("@BUSINESS_UNIT").Value = Utilities.GetAllString
                        da.Fill(dt)
                    End If
                End If

            Catch ex As Exception
            Finally
                cmd.Connection.Close()
            End Try
            TalentCache.AddPropertyToCache(cacheKey, dt, CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes")), TimeSpan.Zero, CacheItemPriority.Normal)
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
        Else
            dt = CType(HttpContext.Current.Cache.Item(cacheKey), DataTable)
        End If
        Return dt
    End Function

    Private Shared Function getBasketItemsWithoutSpecifiedFeeCategories(ByRef valueOfFeesNotIncluded As Decimal, ByRef header As TalentBasketDataset.tbl_order_headerDataTable) As List(Of DEBasketItem)
        Dim basketItems As List(Of DEBasketItem) = CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
        Dim myDefaults As New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
        valueOfFeesNotIncluded = 0
        If header.Rows.Count > 0 Then
            Dim hRow As TalentBasketDataset.tbl_order_headerRow = header.Rows(0)
            valueOfFeesNotIncluded = hRow.TOTAL_DELIVERY_GROSS
        End If

        If def.FeeCategoriesNotIncludedInECommerceTracking.Length > 0 Then
            Dim feeCategories() As String = def.FeeCategoriesNotIncludedInECommerceTracking.Split(",")
            Dim tempBasketItems As List(Of DEBasketItem) = New List(Of DEBasketItem)
            For Each item In basketItems
                Dim categoryFound As Boolean = False
                For Each category As String In feeCategories
                    If item.FEE_CATEGORY = category Then
                        categoryFound = True
                        Exit For
                    End If
                Next
                If categoryFound Then
                    valueOfFeesNotIncluded += item.Gross_Price
                Else
                    tempBasketItems.Add(item)
                End If
            Next
            Return tempBasketItems
        Else
            Return basketItems
        End If
    End Function

    Private Shared Function BuildTrackingScripts() As String
        Dim pageCode As String = Talent.eCommerce.Utilities.GetCurrentPageName()
        Dim sbJavaScript As New StringBuilder
        '
        ' Only process if on checkoutOrderConfirmation
        If pageCode.ToUpper.Equals("CHECKOUTORDERCONFIRMATION.ASPX") Then
            Dim valueOfShippingFees As Decimal = 0
            Dim ordersHeaderTA As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
            Dim ordersDetailTA As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
            Dim header As TalentBasketDataset.tbl_order_headerDataTable
            header = ordersHeaderTA.Get_PROCESSED_Order(CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
            Dim details As TalentBasketDataset.tbl_order_detailDataTable
            Dim basket As DEBasket = CType(HttpContext.Current.Profile, TalentProfile).Basket
            Dim basketItems As List(Of DEBasketItem) = getBasketItemsWithoutSpecifiedFeeCategories(valueOfShippingFees, header)
            Dim basketSummary As DEBasketSummary = CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketSummary
            Dim totalItems As Decimal = CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketSummary.TotalBasket

            Dim paymentRef As String = String.Empty
            If basketItems.Count > 0 Then
                Dim basketType = Talent.eCommerce.Utilities.BasketContentTypeWithOverride
                If basketType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                    If header.Rows.Count > 0 Then
                        Dim hRow As TalentBasketDataset.tbl_order_headerRow = header.Rows(0)
                        details = ordersDetailTA.Get_Order_Lines(hRow.TEMP_ORDER_ID)
                        paymentRef = hRow.PROCESSED_ORDER_ID
                    End If
                Else
                    paymentRef = HttpContext.Current.Request.QueryString("PaymentRef")
                End If
                If Not String.IsNullOrEmpty(paymentRef) Then

                    'Create a new list as we need to list tickets the same as retail eg. HOME01 x3 rather than HOME01 x1, HOME01 x1, HOME01 x1
                    Dim amendedBasketItems As New List(Of DEBasketItem)
                    Dim addItem As Boolean = False
                    For Each item As DEBasketItem In basketItems
                        Dim mode As String = item.MODULE_
                        If amendedBasketItems.Count > 0 AndAlso mode.ToUpper.Equals("TICKETING") Then
                            Dim index As Integer = 0
                            addItem = True
                            For Each amendedBasketItem As DEBasketItem In amendedBasketItems
                                If amendedBasketItem.Product = item.Product AndAlso amendedBasketItem.PRODUCT_TYPE = item.PRODUCT_TYPE AndAlso amendedBasketItem.Gross_Price = item.Gross_Price Then
                                    addItem = False
                                    Exit For
                                End If
                                index += 1
                            Next
                            If addItem Then
                                amendedBasketItems.Add(item)
                            Else
                                amendedBasketItems.Item(index).Quantity += 1
                            End If
                        Else
                            amendedBasketItems.Add(item)
                        End If
                    Next

                    sbJavaScript.Append("<script>")
                    sbJavaScript.Append(" var dataLayer = []; ")
                    sbJavaScript.Append(" dataLayer.push({ ")
                    sbJavaScript.Append("   'transactionId': '").Append(paymentRef).Append("',")
                    sbJavaScript.Append("   'transactionTotal': ").Append(totalItems).Append(",")
                    sbJavaScript.Append("   'transactionShipping': ").Append(valueOfShippingFees).Append(",")
                    sbJavaScript.Append("   'transactionProducts': [{ ")
                    For Each item As DEBasketItem In amendedBasketItems
                        Dim index = amendedBasketItems.IndexOf(item)
                        Dim mode As String = item.MODULE_
                        sbJavaScript.Append("       'sku': '").Append(item.Product).Append("',")
                        sbJavaScript.Append("       'name': '").Append(item.PRODUCT_DESCRIPTION1.TrimEnd).Append("',")
                        If mode.ToUpper.Equals("TICKETING") Then
                            sbJavaScript.Append("       'category': '").Append(item.PRODUCT_TYPE).Append("',")
                            If item.PACKAGE_ID > 0 Then
                                Dim price As Decimal = 0
                                Decimal.TryParse(item.Net_Price / item.Quantity, price)
                                sbJavaScript.Append("       'price': ").Append(price).Append(",")
                            Else
                                sbJavaScript.Append("       'price': ").Append(item.Net_Price).Append(",")
                            End If
                        Else
                            sbJavaScript.Append("       'category': '").Append(item.GROUP_LEVEL_01).Append("',")
                            sbJavaScript.Append("       'price': ").Append(Talent.eCommerce.Utilities.GetWebPrices(item.Product, 1, item.MASTER_PRODUCT).DisplayPrice).Append(",")
                        End If
                        sbJavaScript.Append("       'quantity': ").Append(item.Quantity)
                        If (index + 1) <> amendedBasketItems.Count Then
                            sbJavaScript.Append("   },{ ")
                        End If
                    Next
                    sbJavaScript.Append("   }],")
                    sbJavaScript.Append("   'event': 'trackTrans'")
                    sbJavaScript.Append(" });")
                    sbJavaScript.Append("</script>")
                End If
            End If
        End If
        Return sbJavaScript.ToString
    End Function

    Public Shared Function ReplacePlaceholders(ByVal str As String, ByVal settings As DataTable, ByVal provider As String) As String
        Dim pageCode As String = Talent.eCommerce.Utilities.GetCurrentPageName()
        '
        ' Only process if on checkoutOrderConfirmation
        If pageCode.ToUpper.Equals("CHECKOUTORDERCONFIRMATION.ASPX") Then
            Try
                Dim totalSaleValue As Decimal = 0
                Dim currencyCorrection As Decimal = 1
                Dim currencyFormatString As String = String.Empty
                Dim currencyRoundTo As Decimal = 0.01
                Dim currencyRoundUp As Boolean = True
                Dim merchantID As String = ""
                Dim showPricesAsNet As Boolean = False
                Dim setDomainName As String = String.Empty

                Try
                    For Each setting As DataRow In settings.Rows
                        Select Case setting("SETTING_NAME")
                            Case Is = "CURRENCY_CORRECTION"
                                currencyCorrection = Utilities.CheckForDBNull_Decimal(setting("VALUE"))
                            Case Is = "CURRENCY_FORMAT_STRING"
                                currencyFormatString = Utilities.CheckForDBNull_String(setting("VALUE"))
                            Case Is = "CURRENCY_ROUND_UP"
                                currencyRoundUp = Utilities.CheckForDBNull_Boolean_DefaultTrue(setting("VALUE"))
                            Case Is = "CURRENCY_ROUND_TO"
                                currencyRoundTo = Utilities.CheckForDBNull_Decimal(setting("VALUE"))
                            Case Is = "MERCHANT_ID"
                                merchantID = Utilities.CheckForDBNull_String(setting("VALUE"))
                            Case Is = "SHOW_PRICES_AS_NET_VALUES"
                                showPricesAsNet = Utilities.CheckForDBNull_Boolean_DefaultFalse(setting("VALUE"))
                            Case Is = "SET_DOMAIN_NAME"
                                setDomainName = Utilities.CheckForDBNull_String(setting("VALUE"))
                        End Select
                    Next
                Catch ex As Exception
                End Try


                Dim ordersHeaderTA As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                Dim ordersDetailTA As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
                Dim header As TalentBasketDataset.tbl_order_headerDataTable
                Dim details As TalentBasketDataset.tbl_order_detailDataTable
                header = ordersHeaderTA.Get_PROCESSED_Order(CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)

                If header.Rows.Count > 0 AndAlso Talent.eCommerce.Utilities.BasketContentTypeWithOverride = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                    'retail
                    Dim hRow As TalentBasketDataset.tbl_order_headerRow = header.Rows(0)
                    If showPricesAsNet Then
                        totalSaleValue = Utilities.CheckForDBNull_Decimal(hRow.TOTAL_ORDER_ITEMS_VALUE_NET)
                    Else
                        totalSaleValue = Utilities.CheckForDBNull_Decimal(hRow.TOTAL_AMOUNT_CHARGED)
                    End If
                    details = ordersDetailTA.Get_Order_Lines(hRow.TEMP_ORDER_ID)

                    totalSaleValue = Utilities.RoundToValue(totalSaleValue * currencyCorrection, currencyRoundTo, currencyRoundUp)

                    Select Case UCase(provider)
                        Case Is = "GOOGLE", "GOOGLEV1"
                            str = str.Replace("<TOTAL_SALE_VALUE>", totalSaleValue.ToString(currencyFormatString))
                            str = str.Replace("<GOOGLE_VARIABLES>", GetOrderFormatForGoogle(hRow, details, currencyCorrection, currencyFormatString, currencyRoundTo, currencyRoundUp, showPricesAsNet))
                            str = str.Replace("<CUSTOMER_ORDER_REFERENCE>", Utilities.CheckForDBNull_String(hRow.PROCESSED_ORDER_ID))
                            str = str.Replace("<CURRENT_LANGUAGE>", Talent.Common.Utilities.GetDefaultLanguage)
                        Case Is = "GOOGLEV2"
                            str = GetOrderFormatForGoogle_V2(hRow, details, currencyCorrection, currencyFormatString, currencyRoundTo, currencyRoundUp, showPricesAsNet, merchantID, setDomainName)
                        Case Is = "AW"
                            str = str.Replace("<TOTAL_SALE_VALUE>", totalSaleValue.ToString(currencyFormatString))
                            str = str.Replace("<CUSTOMER_ORDER_REFERENCE>", Utilities.CheckForDBNull_String(hRow.PROCESSED_ORDER_ID))
                            str = str.Replace("<CURRENT_LANGUAGE>", Talent.Common.Utilities.GetDefaultLanguage)
                            str = str.Replace("<MERCHANT_ID>", merchantID)
                            str = str.Replace("<AFFILIATE_WINDOW_ORDER_ITEMS>", GetOrderFormatForAffiliateWindow(merchantID, hRow, details, currencyCorrection, currencyFormatString, currencyRoundTo, currencyRoundUp, showPricesAsNet))
                    End Select
                Else
                    Select Case CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
                        Case "T"
                            Select Case UCase(provider)
                                Case Is = "GOOGLE", "GOOGLEV1"
                                    ' Blank placeholders to prevent javascript errors
                                    str = str.Replace("<TOTAL_SALE_VALUE>", HttpContext.Current.Request.QueryString("paymentAmount"))
                                    str = str.Replace("<GOOGLE_VARIABLES>", "''")
                                    str = str.Replace("<CUSTOMER_ORDER_REFERENCE>", HttpContext.Current.Request.QueryString("PaymentRef"))
                                    str = str.Replace("<CURRENT_LANGUAGE>", Talent.Common.Utilities.GetDefaultLanguage)
                                Case Is = "GOOGLEV2"
                                    str = GetOrderFormatForGoogle_V2Tickets(currencyCorrection, currencyFormatString, currencyRoundTo, currencyRoundUp, showPricesAsNet, merchantID, setDomainName)
                                Case Is = "AW"
                                    ' Blank placeholders to prevent javascript errors
                                    str = str.Replace("<TOTAL_SALE_VALUE>", "''")
                                    str = str.Replace("<CUSTOMER_ORDER_REFERENCE>", "''")
                                    str = str.Replace("<CURRENT_LANGUAGE>", "''")
                                Case Is = "ICROSSING"
                                    str = GetTrackingCodeForICrossing_Tickets(currencyCorrection, currencyFormatString, currencyRoundTo, currencyRoundUp, showPricesAsNet, merchantID)
                            End Select
                        Case Else
                            Select Case UCase(provider)
                                Case Is = "GOOGLE", "GOOGLEV1"
                                    ' Blank placeholders to prevent javascript errors
                                    str = str.Replace("<TOTAL_SALE_VALUE>", HttpContext.Current.Request.QueryString("paymentAmount"))
                                    str = str.Replace("<GOOGLE_VARIABLES>", "''")
                                    str = str.Replace("<CUSTOMER_ORDER_REFERENCE>", HttpContext.Current.Request.QueryString("PaymentRef"))
                                    str = str.Replace("<CURRENT_LANGUAGE>", Talent.Common.Utilities.GetDefaultLanguage)
                                Case Is = "AW"
                                    ' Blank placeholders to prevent javascript errors
                                    str = str.Replace("<TOTAL_SALE_VALUE>", "''")
                                    str = str.Replace("<CUSTOMER_ORDER_REFERENCE>", "''")
                                    str = str.Replace("<CURRENT_LANGUAGE>", "''")
                            End Select
                    End Select
                End If
            Catch ex As Exception

            End Try
        End If
        Return str
    End Function

    Public Shared Function GetOrderFormatForAffiliateWindow(ByVal merchantID As String, _
                                                            ByVal hRow As TalentBasketDataset.tbl_order_headerRow, _
                                                            ByVal details As TalentBasketDataset.tbl_order_detailDataTable, _
                                                            ByVal currencyCorrection As Decimal, _
                                                            ByVal currencyFormatString As String, _
                                                            ByVal currencyRoundTo As Decimal, _
                                                            ByVal currencyRoundUp As Boolean, _
                                                            ByVal showPricesAsNet As Boolean) As String

        Dim returnStr As String = ""
        Try
            Dim purchasePrice As Decimal = 0
            For Each detail As TalentBasketDataset.tbl_order_detailRow In details.Rows
                If showPricesAsNet Then
                    purchasePrice = Utilities.CheckForDBNull_Decimal(detail.PURCHASE_PRICE_NET) * currencyCorrection
                Else
                    purchasePrice = Utilities.CheckForDBNull_Decimal(detail.PURCHASE_PRICE_GROSS) * currencyCorrection
                End If
                purchasePrice = Utilities.RoundToValue(purchasePrice, currencyRoundTo, currencyRoundUp)

                If detail.IsPRODUCT_DESCRIPTION_1Null Then
                    detail.PRODUCT_DESCRIPTION_1 = ""
                End If
                If detail.IsGROUP_LEVEL_01Null Then
                    detail.GROUP_LEVEL_01 = ""
                End If

                'AW:P|[merchant_id]|[order_ref]|[product_id]|[product_name]|[unit_price]|[quantity]|[sku_data]|[cg]|[category]
                '{0} = Merchant ID
                '{1} = Order Ref
                '{2} = Product ID
                '{3} = Product Name
                '{4} = Unit Price
                '{5} = Quantity
                '{6} = SKU Data
                '{7} = CG??
                '{8} = Category
                returnStr += String.Format(vbTab & "AW:P|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}" & vbTab & vbCrLf, _
                                                merchantID, _
                                                hRow.PROCESSED_ORDER_ID, _
                                                Utilities.CheckForDBNull_String(detail.PRODUCT_CODE), _
                                                Utilities.CheckForDBNull_String(detail.PRODUCT_DESCRIPTION_1), _
                                                purchasePrice.ToString(currencyFormatString), _
                                                Utilities.CheckForDBNull_Decimal(detail.QUANTITY).ToString, _
                                                "", _
                                                "", _
                                                Utilities.CheckForDBNull_String(detail.GROUP_LEVEL_01))

            Next

        Catch ex As Exception
        Finally
        End Try
        Return returnStr
    End Function

    Public Shared Function GetOrderFormatForGoogle(ByVal hRow As TalentBasketDataset.tbl_order_headerRow, _
                                                    ByVal details As TalentBasketDataset.tbl_order_detailDataTable, _
                                                    ByVal currencyCorrection As Decimal, _
                                                    ByVal currencyFormatString As String, _
                                                    ByVal currencyRoundTo As Decimal, _
                                                    ByVal currencyRoundUp As Boolean, _
                                                    ByVal showPricesAsNet As Boolean) As String

        Dim orderString As String = String.Empty
        Try

            Dim ordVal, tax, del As Decimal

            If showPricesAsNet Then
                ordVal = Utilities.CheckForDBNull_Decimal(hRow.TOTAL_ORDER_ITEMS_VALUE_NET) * currencyCorrection
            Else
                ordVal = Utilities.CheckForDBNull_Decimal(hRow.TOTAL_ORDER_ITEMS_VALUE_GROSS) * currencyCorrection
            End If
            tax = Utilities.CheckForDBNull_Decimal(hRow.TOTAL_ORDER_ITEMS_TAX) * currencyCorrection
            del = Utilities.CheckForDBNull_Decimal(hRow.TOTAL_DELIVERY_GROSS) * currencyCorrection

            ordVal = Utilities.RoundToValue(ordVal, currencyRoundTo, currencyRoundUp)
            tax = Utilities.RoundToValue(tax, currencyRoundTo, currencyRoundUp)
            del = Utilities.RoundToValue(del, currencyRoundTo, currencyRoundUp)

            'Build The Google String
            '-------------------------------------
            orderString = String.Format("'UTM:T|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7} '", _
                                            hRow.PROCESSED_ORDER_ID, _
                                            TalentCache.GetBusinessUnit, _
                                            ordVal.ToString(currencyFormatString), _
                                            tax.ToString(currencyFormatString), _
                                            del.ToString(currencyFormatString), _
                                            Utilities.CheckForDBNull_String(hRow.ADDRESS_LINE_1), _
                                            Utilities.CheckForDBNull_String(hRow.ADDRESS_LINE_2), _
                                            Utilities.CheckForDBNull_String(hRow.POSTCODE))


            Dim purchasePrice As Decimal

            For Each detail As TalentBasketDataset.tbl_order_detailRow In details.Rows
                If showPricesAsNet Then
                    purchasePrice = Utilities.CheckForDBNull_Decimal(detail.PURCHASE_PRICE_NET) * currencyCorrection
                Else
                    purchasePrice = Utilities.CheckForDBNull_Decimal(detail.PURCHASE_PRICE_GROSS) * currencyCorrection
                End If
                purchasePrice = Utilities.RoundToValue(purchasePrice, currencyRoundTo, currencyRoundUp)

                If detail.IsPRODUCT_DESCRIPTION_1Null Then
                    detail.PRODUCT_DESCRIPTION_1 = ""
                End If
                If detail.IsGROUP_LEVEL_01Null Then
                    detail.GROUP_LEVEL_01 = ""
                End If

                orderString += String.Format(" + 'UTM:I|{0}|{1}|{2}|{3}|{4}|{5} ' ", _
                                                hRow.PROCESSED_ORDER_ID, _
                                                Utilities.CheckForDBNull_String(detail.PRODUCT_CODE), _
                                                Utilities.CheckForDBNull_String(detail.PRODUCT_DESCRIPTION_1), _
                                                Utilities.CheckForDBNull_String(detail.GROUP_LEVEL_01), _
                                                purchasePrice.ToString(currencyFormatString), _
                                                Utilities.CheckForDBNull_Decimal(detail.QUANTITY).ToString)
            Next

        Catch ex As Exception
        Finally
        End Try
        Return orderString
    End Function

    Public Shared Function GetOrderFormatForGoogle_V2Tickets(ByVal currencyCorrection As Decimal, _
                                                                ByVal currencyFormatString As String, _
                                                                ByVal currencyRoundTo As Decimal, _
                                                                ByVal currencyRoundUp As Boolean, _
                                                                ByVal showPricesAsNet As Boolean, _
                                                                ByVal MerchantID As String, _
                                                                ByVal setDomainName As String) As String

        Dim orderString As String = String.Empty
        Try
            orderString += "<script type=""text/javascript"">" & vbCrLf
            orderString += "var gaJsHost = ((""https:"" == document.location.protocol) ? ""https://ssl."" : ""http://www."");" & vbCrLf
            orderString += "document.write(unescape(""%3Cscript src='"" + gaJsHost + ""google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E""));" & vbCrLf
            orderString += "</script>" & vbCrLf
            orderString += "" & vbCrLf
            orderString += "<script type=""text/javascript"">" & vbCrLf
            orderString += "    var pageTracker = _gat._getTracker(""" & MerchantID & """);" & vbCrLf
            orderString += "    pageTracker._setDomainName(""" & setDomainName & """);" & vbCrLf
            orderString += "    pageTracker._initData();" & vbCrLf
            orderString += "    pageTracker._trackPageview();" & vbCrLf

            Dim ordVal, tax, del As Decimal
            Dim basketItems As New Generic.List(Of TalentBasketItem)

            For Each tbi As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
                If Not tbi.IS_FREE Then
                    'Product cannot be a fee
                    If Not Talent.eCommerce.Utilities.IsTicketingFee(tbi.MODULE_.Trim, tbi.Product.Trim, tbi.FEE_CATEGORY.Trim) Then
                        basketItems.Add(tbi)
                        ordVal += tbi.Gross_Price
                    End If
                End If
            Next
            ordVal = ordVal * currencyCorrection
            'tax = 0 * currencyCorrection

            For Each tbi As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
                If tbi.IS_FREE Then
                    If Not Talent.eCommerce.Utilities.IsTicketingFee(tbi.MODULE_.Trim, tbi.Product.Trim, tbi.FEE_CATEGORY.Trim) Then
                        del += tbi.Gross_Price
                    End If
                End If
            Next

            del = del * currencyCorrection
            ordVal = Utilities.RoundToValue(ordVal, currencyRoundTo, currencyRoundUp)
            tax = Utilities.RoundToValue(tax, currencyRoundTo, currencyRoundUp)
            del = Utilities.RoundToValue(del, currencyRoundTo, currencyRoundUp)

            'Build The Google String
            '-------------------------------------
            ' pageTracker._addTrans(
            '    "1234",                                     // Order ID
            '    "Mountain View",                            // Affiliation
            '    "11.99",                                    // Total
            '    "1.29",                                     // Tax
            '    "5",                                        // Shipping
            '    "San Jose",                                 // City
            '    "California",                               // State
            '    "USA"                                       // Country
            '  );

            Dim address As New TalentProfileAddress
            address = ProfileHelper.ProfileAddressEnumerator(0, CType(HttpContext.Current.Profile, TalentProfile).User.Addresses)
            orderString += String.Format("  pageTracker._addTrans(""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}"" ); " & vbCrLf, _
                                            HttpContext.Current.Request.QueryString("PaymentRef"), _
                                            TalentCache.GetBusinessUnit, _
                                            ordVal.ToString(currencyFormatString), _
                                            tax.ToString(currencyFormatString), _
                                            del.ToString(currencyFormatString), _
                                            address.Address_Line_2, _
                                            address.Address_Line_4, _
                                            address.Country)


            Dim purchasePrice As Decimal
            For Each tbi As TalentBasketItem In basketItems
                purchasePrice = Utilities.CheckForDBNull_Decimal(tbi.Gross_Price) * currencyCorrection
                purchasePrice = Utilities.RoundToValue(purchasePrice, currencyRoundTo, currencyRoundUp)

                'Add the Items
                '-------------------------------------
                ' pageTracker._addItem(
                '    "1234",                                     // Order ID
                '    "DD44",                                     // SKU
                '    "T-Shirt",                                  // Product Name 
                '    "Green Medium",                             // Category
                '    "11.99",                                    // Price
                '    "1"                                         // Quantity
                '  );

                orderString += String.Format("  pageTracker._addItem(""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}""); " & vbCrLf, _
                                                HttpContext.Current.Request.QueryString("PaymentRef"), _
                                                tbi.Product & tbi.Seat, _
                                                tbi.PRODUCT_DESCRIPTION1.Trim, _
                                                tbi.PRODUCT_DESCRIPTION6.Trim, _
                                                purchasePrice.ToString(currencyFormatString), _
                                                1)
            Next

            orderString += "    pageTracker._trackTrans();" & vbCrLf
            orderString += "</script>" & vbCrLf
            orderString += ""

        Catch ex As Exception
        Finally
        End Try
        Return orderString
    End Function

    Public Shared Function GetOrderFormatForGoogle_V2(ByVal hRow As TalentBasketDataset.tbl_order_headerRow, _
                                                    ByVal details As TalentBasketDataset.tbl_order_detailDataTable, _
                                                    ByVal currencyCorrection As Decimal, _
                                                    ByVal currencyFormatString As String, _
                                                    ByVal currencyRoundTo As Decimal, _
                                                    ByVal currencyRoundUp As Boolean, _
                                                    ByVal showPricesAsNet As Boolean, _
                                                    ByVal MerchantID As String, _
                                                    ByVal setDomainName As String) As String

        Dim orderString As String = String.Empty
        Try
            orderString += "<script type=""text/javascript"">" & vbCrLf
            orderString += "var gaJsHost = ((""https:"" == document.location.protocol) ? ""https://ssl."" : ""http://www."");" & vbCrLf
            orderString += "document.write(unescape(""%3Cscript src='"" + gaJsHost + ""google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E""));" & vbCrLf
            orderString += "</script>" & vbCrLf
            orderString += "" & vbCrLf
            orderString += "<script type=""text/javascript"">" & vbCrLf
            orderString += "    var pageTracker = _gat._getTracker(""" & MerchantID & """);" & vbCrLf
            orderString += "    pageTracker.setDomainName('" & setDomainName & "');" & vbCrLf
            orderString += "    pageTracker._initData();" & vbCrLf
            orderString += "    pageTracker._trackPageview();" & vbCrLf

            Dim ordVal, tax, del As Decimal
            If showPricesAsNet Then
                ordVal = Utilities.CheckForDBNull_Decimal(hRow.TOTAL_ORDER_ITEMS_VALUE_NET) * currencyCorrection
            Else
                ordVal = Utilities.CheckForDBNull_Decimal(hRow.TOTAL_ORDER_ITEMS_VALUE_GROSS) * currencyCorrection
            End If
            tax = Utilities.CheckForDBNull_Decimal(hRow.TOTAL_ORDER_ITEMS_TAX) * currencyCorrection
            del = Utilities.CheckForDBNull_Decimal(hRow.TOTAL_DELIVERY_GROSS) * currencyCorrection

            ordVal = Utilities.RoundToValue(ordVal, currencyRoundTo, currencyRoundUp)
            tax = Utilities.RoundToValue(tax, currencyRoundTo, currencyRoundUp)
            del = Utilities.RoundToValue(del, currencyRoundTo, currencyRoundUp)

            'Build The Google String
            '-------------------------------------
            ' pageTracker._addTrans(
            '    "1234",                                     // Order ID
            '    "Mountain View",                            // Affiliation
            '    "11.99",                                    // Total
            '    "1.29",                                     // Tax
            '    "5",                                        // Shipping
            '    "San Jose",                                 // City
            '    "California",                               // State
            '    "USA"                                       // Country
            '  );
            orderString += String.Format("  pageTracker._addTrans(""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}"" ); " & vbCrLf, _
                                            hRow.PROCESSED_ORDER_ID, _
                                            TalentCache.GetBusinessUnit, _
                                            ordVal.ToString(currencyFormatString), _
                                            tax.ToString(currencyFormatString), _
                                            del.ToString(currencyFormatString), _
                                            Utilities.CheckForDBNull_String(hRow.ADDRESS_LINE_1), _
                                            Utilities.CheckForDBNull_String(hRow.ADDRESS_LINE_2), _
                                            Utilities.CheckForDBNull_String(hRow.COUNTRY))


            Dim purchasePrice As Decimal
            For Each detail As TalentBasketDataset.tbl_order_detailRow In details.Rows
                If showPricesAsNet Then
                    purchasePrice = Utilities.CheckForDBNull_Decimal(detail.PURCHASE_PRICE_NET) * currencyCorrection
                Else
                    purchasePrice = Utilities.CheckForDBNull_Decimal(detail.PURCHASE_PRICE_GROSS) * currencyCorrection
                End If
                purchasePrice = Utilities.RoundToValue(purchasePrice, currencyRoundTo, currencyRoundUp)

                If detail.IsPRODUCT_DESCRIPTION_1Null Then
                    detail.PRODUCT_DESCRIPTION_1 = ""
                End If
                If detail.IsGROUP_LEVEL_01Null Then
                    detail.GROUP_LEVEL_01 = ""
                End If

                'Add the Items
                '-------------------------------------
                ' pageTracker._addItem(
                '    "1234",                                     // Order ID
                '    "DD44",                                     // SKU
                '    "T-Shirt",                                  // Product Name 
                '    "Green Medium",                             // Category
                '    "11.99",                                    // Price
                '    "1"                                         // Quantity
                '  );

                orderString += String.Format("  pageTracker._addItem(""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}""); " & vbCrLf, _
                                                hRow.PROCESSED_ORDER_ID, _
                                                Utilities.CheckForDBNull_String(detail.PRODUCT_CODE), _
                                                Utilities.CheckForDBNull_String(detail.PRODUCT_DESCRIPTION_1), _
                                                Utilities.CheckForDBNull_String(detail.GROUP_LEVEL_01), _
                                                purchasePrice.ToString(currencyFormatString), _
                                                Utilities.CheckForDBNull_Decimal(detail.QUANTITY).ToString)
            Next

            orderString += "    pageTracker._trackTrans();" & vbCrLf
            orderString += "</script>" & vbCrLf
            orderString += ""

        Catch ex As Exception
        Finally
        End Try
        Return orderString
    End Function

    Public Shared Function GetTrackingCodeScript(ByVal location As String, ByVal provider As String) As DataTable
        Dim dt As New DataTable
        Dim cacheKey As String = "TrackingCodeCache_" & _
                                    TalentCache.GetBusinessUnit & "_" & _
                                    Talent.eCommerce.Utilities.GetCurrentPageName.ToLower & "_" & _
                                    provider & "_" & _
                                    location & "_" & _
                                    Utilities.GetDefaultLanguage

        If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            location = UCase(location)
            Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Const SelectStr As String = "SELECT * FROM tbl_page_tracking WITH (NOLOCK)  " & _
                                        " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                        " AND PARTNER = @PARTNER " & _
                                        " AND PAGE_CODE = @PAGE_CODE " & _
                                        " AND TRACKING_PROVIDER = @TRACKING_PROVIDER " & _
                                        " AND LOCATION = @LOCATION " & _
                                        " AND LANGUAGE_CODE = @LANGUAGE_CODE " & _
                                        " "

            cmd.CommandText = SelectStr
            With cmd.Parameters
                .Clear()
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                .Add("@PAGE_CODE", SqlDbType.NVarChar).Value = Talent.eCommerce.Utilities.GetCurrentPageName
                .Add("@TRACKING_PROVIDER", SqlDbType.NVarChar).Value = provider
                .Add("@LOCATION", SqlDbType.NVarChar).Value = location
                .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = Utilities.GetDefaultLanguage
            End With
            Dim da As New SqlDataAdapter(cmd)

            Try
                cmd.Connection.Open()

                'try specific bu, specific partner and specific page
                da.Fill(dt)

                'try specific bu, specific partner and ANY_page
                If Not dt.Rows.Count > 0 Then
                    cmd.Parameters.Item("@PAGE_CODE").Value = Utilities.GetAllString
                    da.Fill(dt)

                    'try specific bu, ANY_partner and specific page
                    If Not dt.Rows.Count > 0 Then
                        cmd.Parameters.Item("@PAGE_CODE").Value = Talent.eCommerce.Utilities.GetCurrentPageName
                        cmd.Parameters.Item("@PARTNER").Value = Utilities.GetAllString
                        da.Fill(dt)

                        'try specific bu, ANY_partner and ANY_page
                        If Not dt.Rows.Count > 0 Then
                            cmd.Parameters.Item("@PAGE_CODE").Value = Utilities.GetAllString
                            da.Fill(dt)

                            'try ANY_bu, ANY_partner and specific page
                            If Not dt.Rows.Count > 0 Then
                                cmd.Parameters.Item("@PAGE_CODE").Value = Talent.eCommerce.Utilities.GetCurrentPageName
                                cmd.Parameters.Item("@BUSINESS_UNIT").Value = Utilities.GetAllString
                                da.Fill(dt)

                                'try ANY_bu, ANY_partner and ANY_page
                                If Not dt.Rows.Count > 0 Then
                                    cmd.Parameters.Item("@PAGE_CODE").Value = Utilities.GetAllString
                                    da.Fill(dt)
                                End If
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
            Finally
                cmd.Connection.Close()
            End Try

            TalentCache.AddPropertyToCache(cacheKey, dt, CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes")), TimeSpan.Zero, CacheItemPriority.Normal)
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
        Else
            dt = CType(HttpContext.Current.Cache.Item(cacheKey), DataTable)
        End If
        Return dt
    End Function

    Public Shared Function GetTrackingCodeForICrossing_Tickets(ByVal currencyCorrection As Decimal, _
                                                                         ByVal currencyFormatString As String, _
                                                                         ByVal currencyRoundTo As Decimal, _
                                                                         ByVal currencyRoundUp As Boolean, _
                                                                         ByVal showPricesAsNet As Boolean, _
                                                                         ByVal MerchantID As String) As String

        Dim orderString As String = String.Empty
        Try
            '<img src="https://cc.gbppc.com/ct/689/x.gif?ngcr=<<OrderValue>>&cid=<<OrderRef>>">

            orderString += "<img src=""https://cc.gbppc.com/ct/689/x.gif?ngcr=<<OrderValue>>&cid=<<OrderRef>>"">"
            Dim ordVal As Decimal = 0
            Dim basketItems As New Generic.List(Of TalentBasketItem)

            For Each tbi As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
                If UCase(tbi.MODULE_) = "TICKETING" AndAlso Not tbi.IS_FREE Then
                    'Product cannot be a fee
                    If tbi.Product.Trim <> "ATFEE" AndAlso tbi.Product.Trim <> "WSFEE" _
                        AndAlso tbi.Product.Trim <> "BKFEE" AndAlso tbi.Product.Trim <> "CRFEE" Then
                        basketItems.Add(tbi)
                        ordVal += tbi.Gross_Price
                    End If
                End If
            Next

            ordVal = ordVal * currencyCorrection
            ordVal = Utilities.RoundToValue(ordVal, currencyRoundTo, currencyRoundUp)
            orderString = orderString.Replace("<<OrderValue>>", ordVal.ToString(currencyFormatString)).Replace("<<OrderRef>>", HttpContext.Current.Request.QueryString("PaymentRef"))

        Catch ex As Exception
        Finally
        End Try
        Return orderString
    End Function

    ''' <summary>
    ''' Get order/user level tracking based on current user information and basket items
    ''' </summary>
    ''' <returns>The formatted tracking string</returns>
    ''' <remarks></remarks>
    Private Shared Function getUserTrackingDetails(ByVal location As String) As String
        Dim trackingString As String = String.Empty
        Dim headerSection As String = String.Empty
        Dim singleRow As String = String.Empty
        Dim rowSection As String = String.Empty
        Dim footerSection As String = String.Empty
        Dim pageCode As String = Talent.eCommerce.Utilities.GetCurrentPageName()
        Dim tDataObjects As New TalentDataObjects
        Dim dtTrackingUserDetails As New DataTable
        Dim profile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
        Dim myDefaults As New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()

        tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        dtTrackingUserDetails = tDataObjects.TrackingSettings.TblTrackingUserDetails.GetUserTrackingDetails(TalentCache.GetBusinessUnit, pageCode, location)
        For Each row As DataRow In dtTrackingUserDetails.Rows
            Select Case Talent.eCommerce.Utilities.CheckForDBNull_Int(row("TRACKING_TYPE"))
                Case Is = 0 : headerSection = headerSection & Talent.eCommerce.Utilities.CheckForDBNull_String(row("CONTENT"))
                Case Is = 1 : singleRow = singleRow & Talent.eCommerce.Utilities.CheckForDBNull_String(row("CONTENT"))
                Case Is = 2 : footerSection = footerSection & Talent.eCommerce.Utilities.CheckForDBNull_String(row("CONTENT"))
            End Select
        Next

        If profile IsNot Nothing AndAlso profile.Basket.BasketItems.Count > 0 AndAlso singleRow.Length > 0 Then
            For Each item As TalentBasketItem In profile.Basket.BasketItems
                If Not Talent.eCommerce.Utilities.IsTicketingFee(String.Empty, item.MODULE_, item.Product, String.Empty) Then
                    Dim temp As String = singleRow
                    temp = temp.Replace("[[PRODUCT_CODE]]", item.Product)
                    temp = temp.Replace("[[MASTER_PRODUCT]]", item.MASTER_PRODUCT)
                    temp = temp.Replace("[[PRODUCT_QUANTITY]]", item.Quantity)
                    temp = temp.Replace("[[PRODUCT_GROSS_PRICE]]", item.Gross_Price)
                    temp = temp.Replace("[[PRODUCT_NET_PRICE]]", item.Net_Price)
                    temp = temp.Replace("[[PRODUCT_TAX_PRICE]]", item.Tax_Price)
                    temp = temp.Replace("[[PRODUCT_DESCRIPTION]]", item.PRODUCT_DESCRIPTION1)
                    rowSection = rowSection & temp
                End If
            Next
        End If

        trackingString = headerSection & rowSection & footerSection
        If trackingString.Length > 0 Then
            If pageCode.ToUpper = "CHECKOUTORDERCONFIRMATION.ASPX" Then
                If HttpContext.Current.Request.QueryString("PaymentRef") IsNot Nothing Then
                    trackingString = trackingString.Replace("[[PAYMENT_REFERENCE]]", HttpContext.Current.Request.QueryString("PaymentRef"))
                Else
                    Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                    Dim tempOrderId As String = profile.Basket.TempOrderID
                    Dim dt As Data.DataTable = orders.Get_Header_By_Temp_Order_Id(tempOrderId)
                    If dt.Rows.Count > 0 Then
                        trackingString = trackingString.Replace("[[PAYMENT_REFERENCE]]", dt.Rows(0)("PROCESSED_ORDER_ID"))
                    End If
                End If
            End If
            If profile IsNot Nothing Then
                If Not profile.IsAnonymous Then
                    trackingString = trackingString.Replace("[[CUSTOMER_NUMBER]]", profile.User.Details.LoginID)
                    trackingString = trackingString.Replace("[[CUSTOMER_EMAIL]]", profile.User.Details.Email)
                    trackingString = trackingString.Replace("[[CUSTOMER_TITLE]]", profile.User.Details.Title)
                    trackingString = trackingString.Replace("[[CUSTOMER_FORENAME]]", profile.User.Details.Forename)
                    trackingString = trackingString.Replace("[[CUSTOMER_SURNAME]]", profile.User.Details.Surname)
                Else
                    trackingString = trackingString.Replace("[[CUSTOMER_NUMBER]]", String.Empty)
                    trackingString = trackingString.Replace("[[CUSTOMER_EMAIL]]", String.Empty)
                    trackingString = trackingString.Replace("[[CUSTOMER_TITLE]]", String.Empty)
                    trackingString = trackingString.Replace("[[CUSTOMER_FORENAME]]", String.Empty)
                    trackingString = trackingString.Replace("[[CUSTOMER_SURNAME]]", String.Empty)
                End If
                If profile.Basket.BasketItems.Count > 0 Then
                    trackingString = trackingString.Replace("[[BASKET_ITEMS]]", profile.Basket.BasketItems.Count)
                    trackingString = trackingString.Replace("[[BASKET_TOTAL]]", profile.Basket.BasketSummary.TotalBasket)
                Else
                    trackingString = trackingString.Replace("[[BASKET_ITEMS]]", 0)
                    trackingString = trackingString.Replace("[[BASKET_TOTAL]]", 0)
                End If
                If HttpContext.Current.Request.QueryString("product") IsNot Nothing Then
                    trackingString = trackingString.Replace("[[VIEW_PRODUCT_CODE]]", HttpContext.Current.Request.QueryString("product"))
                End If
                If HttpContext.Current.Session("ViewProductList") IsNot Nothing Then
                    trackingString = trackingString.Replace("[[VIEW_PRODUCT_LIST]]", HttpContext.Current.Session("ViewProductList"))
                    HttpContext.Current.Session("ViewProductList") = Nothing
                Else
                    trackingString = trackingString.Replace("[[VIEW_PRODUCT_LIST]]", String.Empty)
                End If
            End If
        End If

        Return trackingString
    End Function

End Class
