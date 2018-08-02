Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
Imports Talent.Common

<Serializable()> _
Public Class TalentWebPricing
    Inherits TalentBase

    Private _settings As DEWebPriceSetting
    Private _resultDataSet As DataSet


    Private _prices As Generic.Dictionary(Of String, DEWebPrice)
    Public Property RetrievedPrices() As Generic.Dictionary(Of String, DEWebPrice)
        Get
            Return _prices
        End Get
        Set(ByVal value As Generic.Dictionary(Of String, DEWebPrice))
            _prices = value
        End Set
    End Property


    Private _products As Generic.Dictionary(Of String, WebPriceProduct)
    Public Property Products() As Generic.Dictionary(Of String, WebPriceProduct)
        Get
            Return _products
        End Get
        Set(ByVal value As Generic.Dictionary(Of String, WebPriceProduct))
            _products = value
        End Set
    End Property

    Public Property WebPriceSettings() As DEWebPriceSetting
        Get
            Return _settings
        End Get
        Set(ByVal value As DEWebPriceSetting)
            _settings = value
        End Set
    End Property

    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property

    Private _totalValueGross As Decimal
    Public Property Total_Items_Value_Gross() As Decimal
        Get
            Return _totalValueGross
        End Get
        Set(ByVal value As Decimal)
            _totalValueGross = value
        End Set
    End Property

    Private _totalValueNet As Decimal
    Public Property Total_Items_Value_Net() As Decimal
        Get
            Return _totalValueNet
        End Get
        Set(ByVal value As Decimal)
            _totalValueNet = value
        End Set
    End Property

    Private _totalValueTax As Decimal
    Public Property Total_Items_Value_Tax() As Decimal
        Get
            Return _totalValueTax
        End Get
        Set(ByVal value As Decimal)
            _totalValueTax = value
        End Set
    End Property

    Private _maxDelGross As Decimal
    Public Property Max_Delivery_Gross() As Decimal
        Get
            Return _maxDelGross
        End Get
        Set(ByVal value As Decimal)
            _maxDelGross = value
        End Set
    End Property

    Private _maxDelNet As Decimal
    Public Property Max_Delivery_Net() As Decimal
        Get
            Return _maxDelNet
        End Get
        Set(ByVal value As Decimal)
            _maxDelNet = value
        End Set
    End Property

    Private _maxdeltax As Decimal
    Public Property Max_Delivery_Tax() As Decimal
        Get
            Return _maxdeltax
        End Get
        Set(ByVal value As Decimal)
            _maxdeltax = value
        End Set
    End Property


    Private _promoResultsTable As DataTable
    Public Property PromotionsResultsTable() As DataTable
        Get
            Return _promoResultsTable
        End Get
        Set(ByVal value As DataTable)
            _promoResultsTable = value
        End Set
    End Property

    Private _totalItemsTax As Decimal
    Public Property Total_Items_Tax() As Decimal
        Get
            Return _totalItemsTax
        End Get
        Set(ByVal value As Decimal)
            _totalItemsTax = value
        End Set
    End Property
    Private _totalItemsTax1 As Decimal
    Public Property Total_Items_Tax1() As Decimal
        Get
            Return _totalItemsTax1
        End Get
        Set(ByVal value As Decimal)
            _totalItemsTax1 = value
        End Set
    End Property
    Private _totalItemsTax2 As Decimal
    Public Property Total_Items_Tax2() As Decimal
        Get
            Return _totalItemsTax2
        End Get
        Set(ByVal value As Decimal)
            _totalItemsTax2 = value
        End Set
    End Property
    Private _totalItemsTax3 As Decimal
    Public Property Total_Items_Tax3() As Decimal
        Get
            Return _totalItemsTax3
        End Get
        Set(ByVal value As Decimal)
            _totalItemsTax3 = value
        End Set
    End Property
    Private _totalItemsTax4 As Decimal
    Public Property Total_Items_Tax4() As Decimal
        Get
            Return _totalItemsTax4
        End Get
        Set(ByVal value As Decimal)
            _totalItemsTax4 = value
        End Set
    End Property
    Private _totalItemsTax5 As Decimal
    Public Property Total_Items_Tax5() As Decimal
        Get
            Return _totalItemsTax5
        End Get
        Set(ByVal value As Decimal)
            _totalItemsTax5 = value
        End Set
    End Property

    Private _totalOrderGross As Decimal
    Public Property Total_Order_Value_Gross() As Decimal
        Get
            Return _totalOrderGross
        End Get
        Set(ByVal value As Decimal)
            _totalOrderGross = value
        End Set
    End Property
    Private _totalOrderNet As Decimal
    Public Property Total_Order_Value_Net() As Decimal
        Get
            Return _totalOrderNet
        End Get
        Set(ByVal value As Decimal)
            _totalOrderNet = value
        End Set
    End Property
    Private _totalOrderTax As Decimal
    Public Property Total_Order_Value_Tax() As Decimal
        Get
            Return _totalOrderTax
        End Get
        Set(ByVal value As Decimal)
            _totalOrderTax = value
        End Set
    End Property

    Private _freeDelivery As Boolean
    Public Property Qualifies_For_Free_Delivery() As Boolean
        Get
            Return _freeDelivery
        End Get
        Set(ByVal value As Boolean)
            _freeDelivery = value
        End Set
    End Property

    Private _totalPromotionsValue As Decimal
    Public Property Total_Promotions_Value() As Decimal
        Get
            Return _totalPromotionsValue
        End Get
        Set(ByVal value As Decimal)
            _totalPromotionsValue = value
        End Set
    End Property


    Private _displayPriceIncVAT As Boolean
    Public Property DisplayPriceIncVAT() As Boolean
        Get
            Return _displayPriceIncVAT
        End Get
        Set(ByVal value As Boolean)
            _displayPriceIncVAT = value
        End Set
    End Property

    Private _freeDeliveryPromotion As Boolean
    Public Property FreeDeliveryPromotion() As Boolean
        Get
            Return _freeDeliveryPromotion
        End Get
        Set(ByVal value As Boolean)
            _freeDeliveryPromotion = value
        End Set
    End Property
    Private _freeDeliveryValueForPriceList As Decimal
    Public Property FreeDeliveryValueForPriceList() As Decimal
        Get
            Return _freeDeliveryValueForPriceList
        End Get
        Set(ByVal value As Decimal)
            _freeDeliveryValueForPriceList = value
        End Set
    End Property

    Public Property IsWebPricesModified() As Boolean = False

    Public Sub New(ByVal WebPrice_Settings As DEWebPriceSetting, ByVal products As Generic.Dictionary(Of String, WebPriceProduct), ByVal DisplayPricesIncVAT As Boolean)
        MyBase.New()
        Me.WebPriceSettings = WebPrice_Settings
        Me.Products = products
        Me.DisplayPriceIncVAT = DisplayPricesIncVAT
        Me.Settings = Me.WebPriceSettings
    End Sub

    Public Shared Function CheckDBNull(ByVal value As Object) As Object
        If value.Equals(DBNull.Value) Then
            Return Nothing
        Else
            Return value
        End If
    End Function

    Public Function GetWebPrices() As ErrorObj
        Const ModuleName As String = "GetWebProductPrices"
        Dim err As New ErrorObj

        If Me.Products.Count > 0 Then
            '--------------------------------------------------------------------------
            ' Dim cacheKey As String = ModuleName & WebPriceSettings.BusinessUnit & WebPriceSettings.partner & WebPriceSettings.Company
            'No cache because it is called whenever an item needs pricing
            Me.GetConnectionDetails(WebPriceSettings.BusinessUnit, "", ModuleName)
            Dim dbWebPrice As New DBWebPrice(Me.Products, Me.WebPriceSettings.PriceList, Me.WebPriceSettings.SecondaryPriceList)
            With dbWebPrice
                .Settings = Me.WebPriceSettings
                err = .AccessDatabase
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    ProcessPrices(False, False)
                End If
            End With
        End If

        Return err
    End Function

    ''' <summary>
    ''' This function overrides the product properties with the master product properties to be called
    ''' when the OPTION_PRICE_FROM_MASTER_PRODUCT flag is set to true in ecommerce_module_defaults_bu table.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetWebPricesWithMasterProducts() As ErrorObj
        Const ModuleName As String = "GetWebProductPrices"
        Dim err As New ErrorObj

        If Me.Products.Count > 0 Then
            '--------------------------------------------------------------------------
            ' Dim cacheKey As String = ModuleName & WebPriceSettings.BusinessUnit & WebPriceSettings.partner & WebPriceSettings.Company
            'No cache because it is called whenever an item needs pricing
            Me.GetConnectionDetails(WebPriceSettings.BusinessUnit, "", ModuleName)
            Dim dbWebPrice As New DBWebPrice(Me.Products, Me.WebPriceSettings.PriceList, Me.WebPriceSettings.SecondaryPriceList)
            With dbWebPrice
                .Settings = Me.WebPriceSettings
                err = .AccessDatabase
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    ProcessPrices(False, True)
                End If
            End With
        End If

        Return err
    End Function

    Public Function GetWebPrices_WithPromotionDetails() As ErrorObj
        Const ModuleName As String = "GetWebProductPrices"
        Dim err As New ErrorObj

        If Me.Products.Count > 0 Then
            '--------------------------------------------------------------------------
            Dim cacheKey As String = ModuleName & WebPriceSettings.BusinessUnit & WebPriceSettings.Partner & WebPriceSettings.Company
            If WebPriceSettings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
            Else
                Me.GetConnectionDetails(WebPriceSettings.BusinessUnit, "", ModuleName)
                Dim dbWebPrice As New DBWebPrice(Me.Products, Me.WebPriceSettings.PriceList, Me.WebPriceSettings.SecondaryPriceList)
                With dbWebPrice
                    .Settings = Me.WebPriceSettings
                    err = .AccessDatabase
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        ProcessPrices(True, False)
                    End If
                End With
            End If
        End If
        Return err
    End Function

    Public Function GetWebPricesWithTotals() As ErrorObj
        Const ModuleName As String = "GetWebProductPrices"
        Dim err As New ErrorObj

        If Me.Products.Count > 0 Then
            '--------------------------------------------------------------------------
            Dim cacheKey As String = ModuleName & WebPriceSettings.BusinessUnit & WebPriceSettings.Partner & WebPriceSettings.Company
            If WebPriceSettings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
            Else
                Me.GetConnectionDetails(WebPriceSettings.BusinessUnit, "", ModuleName)
                Dim dbWebPrice As New DBWebPrice(Me.Products, Me.WebPriceSettings.PriceList, Me.WebPriceSettings.SecondaryPriceList)
                With dbWebPrice
                    .Settings = Me.WebPriceSettings
                    err = .AccessDatabase
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        ProcessPrices(True, False)
                        CalculateTotals()
                        EvaluatePricesWithPromotions()
                    End If
                End With
            End If
        End If

        Return err
    End Function

    Public Sub CalculateTotals(Optional ByVal promotionCode As String = "")


        If Not Me.RetrievedPrices Is Nothing Then
            Me.Total_Items_Value_Gross = 0
            Me.Total_Items_Value_Net = 0
            Me.Total_Items_Value_Tax = 0
            For Each productCode As String In Me.RetrievedPrices.Keys
                Dim requestedQty As Decimal = Me.RetrievedPrices(productCode).RequestedQuantity
                Me.Total_Items_Value_Gross += (Me.RetrievedPrices(productCode).Purchase_Price_Gross * requestedQty)
                Me.Total_Items_Value_Net += (Me.RetrievedPrices(productCode).Purchase_Price_Net * requestedQty)
                Me.Total_Items_Value_Tax += (Me.RetrievedPrices(productCode).Purchase_Price_Tax * requestedQty)
                If Me.Max_Delivery_Gross < Me.RetrievedPrices(productCode).DELIVERY_GROSS_PRICE Then
                    Me.Max_Delivery_Gross = Me.RetrievedPrices(productCode).DELIVERY_GROSS_PRICE
                    Me.Max_Delivery_Net = Me.RetrievedPrices(productCode).DELIVERY_NET_PRICE
                    Me.Max_Delivery_Tax = Me.RetrievedPrices(productCode).DELIVERY_TAX_AMOUNT
                End If
            Next
            Me.FreeDeliveryValueForPriceList = GetFreeDeliveryValue()

            If Me.FreeDeliveryValueForPriceList > 0 AndAlso Me.Total_Items_Value_Gross >= Me.FreeDeliveryValueForPriceList Then
                Me.Qualifies_For_Free_Delivery = True
                Me.Total_Order_Value_Gross = Me.Total_Items_Value_Gross
                Me.Total_Order_Value_Net = Me.Total_Items_Value_Net
                Me.Total_Order_Value_Tax = Me.Total_Items_Value_Tax
            Else
                Me.Qualifies_For_Free_Delivery = False
                Me.Total_Order_Value_Gross = Me.Total_Items_Value_Gross + Me.Max_Delivery_Gross
                Me.Total_Order_Value_Net = Me.Total_Items_Value_Net + Me.Max_Delivery_Net
                Me.Total_Order_Value_Tax = Me.Total_Items_Value_Tax + Me.Max_Delivery_Tax
            End If
        End If
    End Sub

    Protected Sub ProcessPrices(ByVal withPromoCheck As Boolean, ByVal includeAllMasterProducts As Boolean)
        Dim results As DataTable = ResultDataSet.Tables(0), _
            wp As New DEWebPrice

        If results.Rows.Count > 0 Then
            Me.RetrievedPrices = New Generic.Dictionary(Of String, DEWebPrice)

            Dim addedProducts As New Generic.List(Of String)

            'Loop through the returned records and add the prices
            For i As Integer = 0 To results.Rows.Count - 1
                If Not Me.RetrievedPrices.ContainsKey(results.Rows(i)("PRODUCT")) Then
                    If Products.ContainsKey(results.Rows(i)("PRODUCT")) Then
                        wp = New DEWebPrice
                        wp = PopulateWebPriceEntity(results, wp, i)
                        wp.RequestedQuantity = Products(results.Rows(i)("PRODUCT")).Quantity
                        Me.RetrievedPrices.Add(wp.PRODUCT_CODE, wp)

                        'Keep a record of the products that have been added
                        addedProducts.Add(wp.PRODUCT_CODE)
                    End If
                End If
            Next

            If Not includeAllMasterProducts Then
                'Add the Master Product prices to any entries that were not found
                If addedProducts.Count < Me.Products.Count Then
                    For Each prod As WebPriceProduct In Me.Products.Values
                        ' Also need to check if master has already been added. 
                        ' If not, this crashes when purchasing a multibuy product
                        ' along with something else
                        If (Not addedProducts.Contains(prod.ProductCode)) AndAlso _
                          (Not addedProducts.Contains(prod.MasterProductCode)) Then
                            For i As Integer = 0 To results.Rows.Count - 1
                                If results.Rows(i)("PRODUCT") = prod.MasterProductCode Then
                                    wp = New DEWebPrice
                                    wp = PopulateWebPriceEntity(results, wp, i)
                                    wp.PRODUCT_CODE = prod.ProductCode
                                    wp.RequestedQuantity = prod.Quantity
                                    Me.RetrievedPrices.Add(wp.PRODUCT_CODE, wp)
                                    Exit For 'We found the result for this product so exit
                                End If
                            Next
                        End If
                    Next
                End If
            Else
                'Replace all the product properties with those of their master products.
                For Each prod As WebPriceProduct In Me.Products.Values
                    For i As Integer = 0 To results.Rows.Count - 1
                        If results.Rows(i)("PRODUCT") = prod.MasterProductCode Then
                            If RetrievedPrices.ContainsKey(prod.ProductCode) Then
                                RetrievedPrices.Remove(prod.ProductCode)
                            End If
                            wp = New DEWebPrice
                            wp = PopulateWebPriceEntity(results, wp, i)
                            wp.PRODUCT_CODE = prod.ProductCode
                            wp.RequestedQuantity = prod.Quantity
                            Me.RetrievedPrices.Add(wp.PRODUCT_CODE, wp)
                            Exit For 'We found the result for this product so exit
                        End If
                    Next
                Next
            End If

            If Me.RetrievedPrices.Count > 0 Then
                CalculatePurchasePrices(withPromoCheck)
            End If

        End If
    End Sub

    Protected Sub CalculatePurchasePrices(ByVal incPromoCheck As Boolean)

        For Each productCode As String In Me.RetrievedPrices.Keys

            Dim wp As DEWebPrice = Me.RetrievedPrices(productCode), _
                qty As Decimal = Products(productCode).Quantity


            If qty = 0 Then qty = 1 'if qty has not been specified, default to 1

            If wp.SALE_PRICE_BREAK_QUANTITY_1 = 0 AndAlso wp.SALE_GROSS_PRICE > 0 Then
                'if the sale price break is not setup, but there is a sale price
                'then treat it as a single sale price

                wp.Purchase_Price_Gross = wp.SALE_GROSS_PRICE
                wp.Purchase_Price_Net = wp.SALE_NET_PRICE
                wp.Purchase_Price_Tax = wp.SALE_TAX_AMOUNT
                wp.From_Price_Gross = wp.SALE_GROSS_PRICE
                wp.From_Price_Net = wp.SALE_NET_PRICE
                wp.From_Price_Tax = wp.SALE_TAX_AMOUNT
                wp.IsSalePrice = True
                wp.IsPartOfPromotion = False

            ElseIf wp.SALE_PRICE_BREAK_QUANTITY_1 > 0 AndAlso wp.SALE_GROSS_PRICE > 0 Then
                ' if the sale price break is setup, process for sale price breaks
                ' determine which is the last level of price break and set
                ' the quantity to be greater than the requested quantity
                ' so that the select will fall into the correct category
                If wp.SALE_PRICE_BREAK_QUANTITY_2 = 0 Then
                    wp.SALE_PRICE_BREAK_QUANTITY_2 = wp.RequestedQuantity + 10
                    wp.From_Price_Gross = wp.SALE_GROSS_PRICE
                    wp.From_Price_Net = wp.SALE_NET_PRICE
                    wp.From_Price_Tax = wp.SALE_TAX_AMOUNT
                ElseIf wp.SALE_PRICE_BREAK_QUANTITY_3 = 0 Then
                    wp.SALE_PRICE_BREAK_QUANTITY_3 = wp.RequestedQuantity + 10
                    wp.From_Price_Gross = wp.SALE_GROSS_PRICE_2
                    wp.From_Price_Net = wp.SALE_NET_PRICE_2
                    wp.From_Price_Tax = wp.SALE_TAX_AMOUNT_2
                ElseIf wp.SALE_PRICE_BREAK_QUANTITY_4 = 0 Then
                    wp.SALE_PRICE_BREAK_QUANTITY_4 = wp.RequestedQuantity + 10
                    wp.From_Price_Gross = wp.SALE_GROSS_PRICE_3
                    wp.From_Price_Net = wp.SALE_NET_PRICE_3
                    wp.From_Price_Tax = wp.SALE_TAX_AMOUNT_3
                ElseIf wp.SALE_PRICE_BREAK_QUANTITY_5 = 0 Then
                    wp.SALE_PRICE_BREAK_QUANTITY_5 = wp.RequestedQuantity + 10
                    wp.From_Price_Gross = wp.SALE_GROSS_PRICE_4
                    wp.From_Price_Net = wp.SALE_NET_PRICE_4
                    wp.From_Price_Tax = wp.SALE_TAX_AMOUNT_4
                ElseIf wp.SALE_PRICE_BREAK_QUANTITY_6 = 0 Then
                    wp.SALE_PRICE_BREAK_QUANTITY_6 = wp.RequestedQuantity + 10
                    wp.From_Price_Gross = wp.SALE_GROSS_PRICE_5
                    wp.From_Price_Net = wp.SALE_NET_PRICE_5
                    wp.From_Price_Tax = wp.SALE_TAX_AMOUNT_5
                ElseIf wp.SALE_PRICE_BREAK_QUANTITY_7 = 0 Then
                    wp.SALE_PRICE_BREAK_QUANTITY_7 = wp.RequestedQuantity + 10
                    wp.From_Price_Gross = wp.SALE_GROSS_PRICE_6
                    wp.From_Price_Net = wp.SALE_NET_PRICE_6
                    wp.From_Price_Tax = wp.SALE_TAX_AMOUNT_6
                ElseIf wp.SALE_PRICE_BREAK_QUANTITY_8 = 0 Then
                    wp.SALE_PRICE_BREAK_QUANTITY_8 = wp.RequestedQuantity + 10
                    wp.From_Price_Gross = wp.SALE_GROSS_PRICE_7
                    wp.From_Price_Net = wp.SALE_NET_PRICE_7
                    wp.From_Price_Tax = wp.SALE_TAX_AMOUNT_7
                ElseIf wp.SALE_PRICE_BREAK_QUANTITY_9 = 0 Then
                    wp.SALE_PRICE_BREAK_QUANTITY_9 = wp.RequestedQuantity + 10
                    wp.From_Price_Gross = wp.SALE_GROSS_PRICE_8
                    wp.From_Price_Net = wp.SALE_NET_PRICE_8
                    wp.From_Price_Tax = wp.SALE_TAX_AMOUNT_8
                ElseIf wp.SALE_PRICE_BREAK_QUANTITY_10 = 0 Then
                    wp.SALE_PRICE_BREAK_QUANTITY_10 = wp.RequestedQuantity + 10
                    wp.From_Price_Gross = wp.SALE_GROSS_PRICE_9
                    wp.From_Price_Net = wp.SALE_NET_PRICE_9
                    wp.From_Price_Tax = wp.SALE_TAX_AMOUNT_9
                Else
                    wp.From_Price_Gross = wp.SALE_GROSS_PRICE_10
                    wp.From_Price_Net = wp.SALE_NET_PRICE_10
                    wp.From_Price_Tax = wp.SALE_TAX_AMOUNT_10
                End If

                Select Case qty
                    Case wp.SALE_PRICE_BREAK_QUANTITY_1 To wp.SALE_PRICE_BREAK_QUANTITY_2
                        wp.Purchase_Price_Gross = wp.SALE_GROSS_PRICE
                        wp.Purchase_Price_Net = wp.SALE_NET_PRICE
                        wp.Purchase_Price_Tax = wp.SALE_TAX_AMOUNT
                    Case wp.SALE_PRICE_BREAK_QUANTITY_2 + 0.001 To wp.SALE_PRICE_BREAK_QUANTITY_3
                        wp.Purchase_Price_Gross = wp.SALE_GROSS_PRICE_2
                        wp.Purchase_Price_Net = wp.SALE_NET_PRICE_2
                        wp.Purchase_Price_Tax = wp.SALE_TAX_AMOUNT_2
                    Case wp.SALE_PRICE_BREAK_QUANTITY_3 + 0.001 To wp.SALE_PRICE_BREAK_QUANTITY_4
                        wp.Purchase_Price_Gross = wp.SALE_GROSS_PRICE_3
                        wp.Purchase_Price_Net = wp.SALE_NET_PRICE_3
                        wp.Purchase_Price_Tax = wp.SALE_TAX_AMOUNT_3
                    Case wp.SALE_PRICE_BREAK_QUANTITY_4 + 0.001 To wp.SALE_PRICE_BREAK_QUANTITY_5
                        wp.Purchase_Price_Gross = wp.SALE_GROSS_PRICE_4
                        wp.Purchase_Price_Net = wp.SALE_NET_PRICE_4
                        wp.Purchase_Price_Tax = wp.SALE_TAX_AMOUNT_4
                    Case wp.SALE_PRICE_BREAK_QUANTITY_5 + 0.001 To wp.SALE_PRICE_BREAK_QUANTITY_6
                        wp.Purchase_Price_Gross = wp.SALE_GROSS_PRICE_5
                        wp.Purchase_Price_Net = wp.SALE_NET_PRICE_5
                        wp.Purchase_Price_Tax = wp.SALE_TAX_AMOUNT_5
                    Case wp.SALE_PRICE_BREAK_QUANTITY_6 + 0.001 To wp.SALE_PRICE_BREAK_QUANTITY_7
                        wp.Purchase_Price_Gross = wp.SALE_GROSS_PRICE_6
                        wp.Purchase_Price_Net = wp.SALE_NET_PRICE_6
                        wp.Purchase_Price_Tax = wp.SALE_TAX_AMOUNT_6
                    Case wp.SALE_PRICE_BREAK_QUANTITY_7 + 0.001 To wp.SALE_PRICE_BREAK_QUANTITY_8
                        wp.Purchase_Price_Gross = wp.SALE_GROSS_PRICE_7
                        wp.Purchase_Price_Net = wp.SALE_NET_PRICE_7
                        wp.Purchase_Price_Tax = wp.SALE_TAX_AMOUNT_7
                    Case wp.SALE_PRICE_BREAK_QUANTITY_8 + 0.001 To wp.SALE_PRICE_BREAK_QUANTITY_9
                        wp.Purchase_Price_Gross = wp.SALE_GROSS_PRICE_8
                        wp.Purchase_Price_Net = wp.SALE_NET_PRICE_8
                        wp.Purchase_Price_Tax = wp.SALE_TAX_AMOUNT_8
                    Case wp.SALE_PRICE_BREAK_QUANTITY_9 + 0.001 To wp.SALE_PRICE_BREAK_QUANTITY_10
                        wp.Purchase_Price_Gross = wp.SALE_GROSS_PRICE_9
                        wp.Purchase_Price_Net = wp.SALE_NET_PRICE_9
                        wp.Purchase_Price_Tax = wp.SALE_TAX_AMOUNT_9
                    Case Is >= wp.SALE_PRICE_BREAK_QUANTITY_10 And wp.PRICE_BREAK_QUANTITY_10 > 0
                        wp.Purchase_Price_Gross = wp.SALE_GROSS_PRICE_10
                        wp.Purchase_Price_Net = wp.SALE_NET_PRICE_10
                        wp.Purchase_Price_Tax = wp.SALE_TAX_AMOUNT_10
                    Case Else
                        'the qty does not fall into a category which means that 
                        'we probably sent in a 0 qty so set to 1st price break
                        wp.Purchase_Price_Gross = wp.SALE_GROSS_PRICE
                        wp.Purchase_Price_Net = wp.SALE_NET_PRICE
                        wp.Purchase_Price_Tax = wp.SALE_TAX_AMOUNT
                End Select

                wp.IsPartOfPromotion = True
                wp.IsSalePrice = True


            ElseIf wp.PRICE_BREAK_QUANTITY_1 = 0 Then
                'if we are not in single SALE price mode, and we are not in sale price break mode
                'check to see if we are in standard single price mode
                wp.Purchase_Price_Gross = wp.GROSS_PRICE
                wp.Purchase_Price_Net = wp.NET_PRICE
                wp.Purchase_Price_Tax = wp.TAX_AMOUNT
                wp.From_Price_Gross = wp.GROSS_PRICE
                wp.From_Price_Net = wp.NET_PRICE
                wp.From_Price_Tax = wp.TAX_AMOUNT
                wp.IsPartOfPromotion = False
                wp.IsSalePrice = False

            Else
                ' otherwise we are in standard price break mode
                ' determine which is the last level of price break and set
                ' the quantity to be greater than the requested quantity
                ' so that the select will fall into the correct category.
                ' BF - Only if requested qty will not fall into any other qty. 
                ' This is because we now need the price break qty's returned to the 
                ' front end when requested qty = 0
                If wp.PRICE_BREAK_QUANTITY_2 = 0 Then
                    If wp.RequestedQuantity > wp.PRICE_BREAK_QUANTITY_1 Then
                        wp.PRICE_BREAK_QUANTITY_2 = wp.RequestedQuantity + 10
                    End If

                    wp.From_Price_Gross = wp.GROSS_PRICE
                    wp.From_Price_Net = wp.NET_PRICE
                    wp.From_Price_Tax = wp.TAX_AMOUNT

                ElseIf wp.PRICE_BREAK_QUANTITY_3 = 0 Then
                    If wp.RequestedQuantity > wp.PRICE_BREAK_QUANTITY_2 Then
                        wp.PRICE_BREAK_QUANTITY_3 = wp.RequestedQuantity + 10
                    End If

                    wp.From_Price_Gross = wp.GROSS_PRICE_2
                    wp.From_Price_Net = wp.NET_PRICE_2
                    wp.From_Price_Tax = wp.TAX_AMOUNT_2

                ElseIf wp.PRICE_BREAK_QUANTITY_4 = 0 Then
                    If wp.RequestedQuantity > wp.PRICE_BREAK_QUANTITY_3 Then
                        wp.PRICE_BREAK_QUANTITY_4 = wp.RequestedQuantity + 10
                    End If

                    wp.From_Price_Gross = wp.GROSS_PRICE_3
                    wp.From_Price_Net = wp.NET_PRICE_3
                    wp.From_Price_Tax = wp.TAX_AMOUNT_3

                ElseIf wp.PRICE_BREAK_QUANTITY_5 = 0 Then
                    If wp.RequestedQuantity > wp.PRICE_BREAK_QUANTITY_4 Then
                        wp.PRICE_BREAK_QUANTITY_5 = wp.RequestedQuantity + 10
                    End If

                    wp.From_Price_Gross = wp.GROSS_PRICE_4
                    wp.From_Price_Net = wp.NET_PRICE_4
                    wp.From_Price_Tax = wp.TAX_AMOUNT_4

                ElseIf wp.PRICE_BREAK_QUANTITY_6 = 0 Then
                    If wp.RequestedQuantity > wp.PRICE_BREAK_QUANTITY_5 Then
                        wp.PRICE_BREAK_QUANTITY_6 = wp.RequestedQuantity + 10
                    End If

                    wp.From_Price_Gross = wp.GROSS_PRICE_5
                    wp.From_Price_Net = wp.NET_PRICE_5
                    wp.From_Price_Tax = wp.TAX_AMOUNT_5

                ElseIf wp.PRICE_BREAK_QUANTITY_7 = 0 Then
                    If wp.RequestedQuantity > wp.PRICE_BREAK_QUANTITY_6 Then
                        wp.PRICE_BREAK_QUANTITY_7 = wp.RequestedQuantity + 10
                    End If

                    wp.From_Price_Gross = wp.GROSS_PRICE_6
                    wp.From_Price_Net = wp.NET_PRICE_6
                    wp.From_Price_Tax = wp.TAX_AMOUNT_6

                ElseIf wp.PRICE_BREAK_QUANTITY_8 = 0 Then
                    If wp.RequestedQuantity > wp.PRICE_BREAK_QUANTITY_7 Then
                        wp.PRICE_BREAK_QUANTITY_8 = wp.RequestedQuantity + 10
                    End If
                    wp.From_Price_Gross = wp.GROSS_PRICE_7
                    wp.From_Price_Net = wp.NET_PRICE_7
                    wp.From_Price_Tax = wp.TAX_AMOUNT_7

                ElseIf wp.PRICE_BREAK_QUANTITY_9 = 0 Then
                    If wp.RequestedQuantity > wp.PRICE_BREAK_QUANTITY_8 Then
                        wp.PRICE_BREAK_QUANTITY_9 = wp.RequestedQuantity + 10
                    End If

                    wp.From_Price_Gross = wp.GROSS_PRICE_8
                    wp.From_Price_Net = wp.NET_PRICE_8
                    wp.From_Price_Tax = wp.TAX_AMOUNT_8

                ElseIf wp.PRICE_BREAK_QUANTITY_10 = 0 Then
                    If wp.RequestedQuantity > wp.PRICE_BREAK_QUANTITY_7 Then
                        wp.PRICE_BREAK_QUANTITY_10 = wp.RequestedQuantity + 10
                    End If

                    wp.From_Price_Gross = wp.GROSS_PRICE_9
                    wp.From_Price_Net = wp.NET_PRICE_9
                    wp.From_Price_Tax = wp.TAX_AMOUNT_9

                Else
                    wp.From_Price_Gross = wp.GROSS_PRICE_10
                    wp.From_Price_Net = wp.NET_PRICE_10
                    wp.From_Price_Tax = wp.TAX_AMOUNT_10
                End If

                Select Case qty
                    Case wp.PRICE_BREAK_QUANTITY_1 To wp.PRICE_BREAK_QUANTITY_2
                        wp.Purchase_Price_Gross = wp.GROSS_PRICE
                        wp.Purchase_Price_Net = wp.NET_PRICE
                        wp.Purchase_Price_Tax = wp.TAX_AMOUNT
                    Case wp.PRICE_BREAK_QUANTITY_2 + 0.001 To wp.PRICE_BREAK_QUANTITY_3
                        wp.Purchase_Price_Gross = wp.GROSS_PRICE_2
                        wp.Purchase_Price_Net = wp.NET_PRICE_2
                        wp.Purchase_Price_Tax = wp.TAX_AMOUNT_2
                    Case wp.PRICE_BREAK_QUANTITY_3 + 0.001 To wp.PRICE_BREAK_QUANTITY_4
                        wp.Purchase_Price_Gross = wp.GROSS_PRICE_3
                        wp.Purchase_Price_Net = wp.NET_PRICE_3
                        wp.Purchase_Price_Tax = wp.TAX_AMOUNT_3
                    Case wp.PRICE_BREAK_QUANTITY_4 + 0.001 To wp.PRICE_BREAK_QUANTITY_5
                        wp.Purchase_Price_Gross = wp.GROSS_PRICE_4
                        wp.Purchase_Price_Net = wp.NET_PRICE_4
                        wp.Purchase_Price_Tax = wp.TAX_AMOUNT_4
                    Case wp.PRICE_BREAK_QUANTITY_5 + 0.001 To wp.PRICE_BREAK_QUANTITY_6
                        wp.Purchase_Price_Gross = wp.GROSS_PRICE_5
                        wp.Purchase_Price_Net = wp.NET_PRICE_5
                        wp.Purchase_Price_Tax = wp.TAX_AMOUNT_5
                    Case wp.PRICE_BREAK_QUANTITY_6 + 0.001 To wp.PRICE_BREAK_QUANTITY_7
                        wp.Purchase_Price_Gross = wp.GROSS_PRICE_6
                        wp.Purchase_Price_Net = wp.NET_PRICE_6
                        wp.Purchase_Price_Tax = wp.TAX_AMOUNT_6
                    Case wp.PRICE_BREAK_QUANTITY_7 + 0.001 To wp.PRICE_BREAK_QUANTITY_8
                        wp.Purchase_Price_Gross = wp.GROSS_PRICE_7
                        wp.Purchase_Price_Net = wp.NET_PRICE_7
                        wp.Purchase_Price_Tax = wp.TAX_AMOUNT_7
                    Case wp.PRICE_BREAK_QUANTITY_8 + 0.001 To wp.PRICE_BREAK_QUANTITY_9
                        wp.Purchase_Price_Gross = wp.GROSS_PRICE_8
                        wp.Purchase_Price_Net = wp.NET_PRICE_8
                        wp.Purchase_Price_Tax = wp.TAX_AMOUNT_8
                    Case wp.PRICE_BREAK_QUANTITY_9 + 0.001 To wp.PRICE_BREAK_QUANTITY_10
                        wp.Purchase_Price_Gross = wp.GROSS_PRICE_9
                        wp.Purchase_Price_Net = wp.NET_PRICE_9
                        wp.Purchase_Price_Tax = wp.TAX_AMOUNT_9
                    Case Is >= wp.PRICE_BREAK_QUANTITY_10
                        wp.Purchase_Price_Gross = wp.GROSS_PRICE_10
                        wp.Purchase_Price_Net = wp.NET_PRICE_10
                        wp.Purchase_Price_Tax = wp.TAX_AMOUNT_10
                    Case Else
                        'the qty does not fall into a category which means that 
                        'we probably sent in a 0 qty so set to 1st price break
                        wp.Purchase_Price_Gross = wp.GROSS_PRICE
                        wp.Purchase_Price_Net = wp.NET_PRICE
                        wp.Purchase_Price_Tax = wp.TAX_AMOUNT
                End Select

                wp.IsPartOfPromotion = True
                wp.IsSalePrice = False

            End If

            wp.PromotionInclusivePriceGross = wp.Purchase_Price_Gross

            If Me.DisplayPriceIncVAT Then
                wp.DisplayPrice = wp.Purchase_Price_Gross
                wp.DisplayPrice_From = wp.From_Price_Gross
            Else
                wp.DisplayPrice = wp.Purchase_Price_Net
                wp.DisplayPrice_From = wp.From_Price_Net
            End If

        Next

        If incPromoCheck Then CheckProductPromotionStatus()


    End Sub

    Protected Sub CheckProductPromotionStatus()
        Dim dbWebPrice As New DBWebPrice(Me.Products, Me.WebPriceSettings.PriceList, Me.WebPriceSettings.SecondaryPriceList)
        dbWebPrice.Settings = Me.WebPriceSettings
        Dim priceBreakDescriptions As New DataTable
        priceBreakDescriptions = dbWebPrice.GetPriceBreakDescription("PriceBreak", "PriceBreak")
        Dim description As String = ""
        Dim dtPromotionsForRequiredProduct As DataTable = Nothing

        Dim dep As New Talent.Common.DEPromotions(Me.WebPriceSettings.BasketHeaderID, _
                                                            Products, _
                                                            Me.WebPriceSettings.BusinessUnit, _
                                                            Me.WebPriceSettings.Username, _
                                                            Now, _
                                                            Me.WebPriceSettings.PromotionTypePriority, _
                                                            Me.WebPriceSettings.Language, _
                                                            Me.WebPriceSettings.Partner, _
                                                            Me.Total_Items_Value_Gross, _
                                                            Me.WebPriceSettings.PromotionCode, _
                                                            Me.Max_Delivery_Gross, _
                                                            Me.WebPriceSettings.InvalidPromotionCodeErrorMessage, _
                                                            Me.WebPriceSettings.TempOrderID, _
                                                            Me.WebPriceSettings.PriceList, _
                                                            Me.WebPriceSettings.PartnerGroup, _
                                                            Me.WebPriceSettings.SecondaryPriceList, _
                                                            Me.WebPriceSettings.UsersAttributeList)

        Dim dbPromo As New DBPromotions
        With dbPromo
            .Dep = dep
            .Settings = Settings
        End With

        dtPromotionsForRequiredProduct = dbPromo.GetPromotionStatusForReqProducts(Me.RetrievedPrices)
        For Each wp As DEWebPrice In Me.RetrievedPrices.Values
            If wp.IsPartOfPromotion Then
                description = ""
                For Each rw As DataRow In priceBreakDescriptions.Rows
                    If UCase(Utilities.CheckForDBNull_String(rw("DESCRIPTION_CODE"))) = UCase(wp.PRICE_BREAK_CODE) Then
                        description = Utilities.CheckForDBNull_String(rw("LANGUAGE_DESCRIPTION"))
                        Exit For
                    End If
                Next

                wp.PromotionDescriptionText = description
                wp.PromotionImagePath = wp.PRICE_BREAK_CODE
            Else
                If dtPromotionsForRequiredProduct IsNot Nothing Then
                    Dim results As Generic.Dictionary(Of String, String) = GetProductPromotionStatus(wp.PRODUCT_CODE, dtPromotionsForRequiredProduct)

                    If Not results Is Nothing AndAlso results.Keys.Count > 0 Then
                        Dim _activationMechanism As String = String.Empty
                        If results.TryGetValue("PromotionActivationMechanism", _activationMechanism) Then
                            If _activationMechanism.Equals("AUTO") Then
                                wp.IsPartOfPromotion = True
                                wp.PromotionDescriptionText = results("DisplayName")
                                wp.PromotionImagePath = results("PromotionCode")
                            End If
                        End If
                    End If
                End If
            End If
        Next
    End Sub

    Protected Sub EvaluatePricesWithPromotions()
        CalculateTotals()
        Dim dep As New Talent.Common.DEPromotions(Me.WebPriceSettings.BasketHeaderID, _
                                                  Products, _
                                                  Me.WebPriceSettings.BusinessUnit, _
                                                  Me.WebPriceSettings.Username, _
                                                  Now, _
                                                  Me.WebPriceSettings.PromotionTypePriority, _
                                                  Me.WebPriceSettings.Language, _
                                                  Me.WebPriceSettings.Partner, _
                                                  Me.Total_Items_Value_Gross, _
                                                  Me.WebPriceSettings.PromotionCode, _
                                                  Me.Max_Delivery_Gross, _
                                                  Me.WebPriceSettings.InvalidPromotionCodeErrorMessage, _
                                                  Me.WebPriceSettings.TempOrderID, _
                                                  Me.WebPriceSettings.PriceList, _
                                                  Me.WebPriceSettings.PartnerGroup, _
                                                  Me.WebPriceSettings.SecondaryPriceList, _
                                                  Me.WebPriceSettings.UsersAttributeList)

        Dim promo As New Talent.Common.TalentPromotions
        promo.Settings = New Talent.Common.DESettings
        promo.Settings.FrontEndConnectionString = Me.WebPriceSettings.FrontEndConnectionString
        promo.Settings.BusinessUnit = Me.WebPriceSettings.BusinessUnit
        promo.Dep = dep
        promo.ResultDataSet = Nothing
        promo.ProcessPromotions(1)

        Dim promoResults As New Data.DataTable
        If Not promo.ResultDataSet Is Nothing AndAlso Not promo.ResultDataSet.Tables("PromotionResults") Is Nothing Then
            promoResults = promo.ResultDataSet.Tables("PromotionResults")
            If promoResults.Rows.Count > 0 Then
                Me.PromotionsResultsTable = promoResults
                CalculateDiscountedPrices()
                ReAssessDeliveryCharges()
            End If
        End If
    End Sub

    Protected Sub ReAssessDeliveryCharges()
        If Not PromotionsResultsTable Is Nothing AndAlso PromotionsResultsTable.Rows.Count > 0 Then
            For Each result As DataRow In PromotionsResultsTable.Rows
                If CBool(Utilities.CheckForDBNull_Boolean_DefaultFalse(result("FreeDelivery"))) Then
                    Me.Qualifies_For_Free_Delivery = True
                    Me.FreeDeliveryPromotion = True
                    Exit For 'exit loop if found 
                End If
            Next
        End If
    End Sub

    Protected Sub CalculateDiscountedPrices()
        Dim NormalPrice As Decimal = 0
        Dim newPrice As Decimal = 0
        Try
            If Not Me.PromotionsResultsTable Is Nothing AndAlso Me.PromotionsResultsTable.Rows.Count > 0 Then
                For Each promo As DataRow In Me.PromotionsResultsTable.Rows
                    If Utilities.CheckForDBNull_Boolean_DefaultFalse(promo("Success")) Then
                        If Utilities.CheckForDBNull_Decimal(promo("PromotionValue")) > 0 Then
                            Me.Total_Promotions_Value += CDec(promo("PromotionValue"))
                            If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(promo("ProductCodes"))) Then
                                Dim products() As String = Utilities.CheckForDBNull_String(promo("ProductCodes")).Split(",")
                                For Each code As String In products
                                    Try
                                        Dim wp As DEWebPrice = Me.RetrievedPrices(code)
                                        If Not wp Is Nothing Then
                                            NormalPrice = wp.Purchase_Price_Gross * wp.RequestedQuantity
                                            newPrice = NormalPrice
                                            If CDec(promo("PromotionValue")) <= (NormalPrice) Then
                                                newPrice = NormalPrice - CDec(promo("PromotionValue"))
                                                promo("PromotionValue") = 0
                                                wp.PromotionInclusivePriceGross = newPrice / wp.RequestedQuantity
                                                Exit For 'We have accounted for all of this promotion's value here, so exit the 'for' loop
                                            ElseIf CDec(promo("PromotionValue")) > (NormalPrice) Then
                                                newPrice = 0
                                                promo("PromotionValue") = CDec(promo("PromotionValue")) - NormalPrice
                                                wp.PromotionInclusivePriceGross = 0
                                            End If
                                        End If
                                    Catch ex As Exception
                                    End Try
                                Next
                            End If
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Function GetFreeDeliveryValue() As Decimal
        Dim dbWebPrice As New DBWebPrice(Me.Products, Me.WebPriceSettings.PriceList, Me.WebPriceSettings.SecondaryPriceList)
        dbWebPrice.Settings = Me.WebPriceSettings

        Return dbWebPrice.GetFreeDeliveryValue_SQL2005
    End Function

    Protected Function GetPropertyNames(ByVal obj As Object) As ArrayList
        Dim al As New ArrayList
        Dim inf() As System.Reflection.PropertyInfo = obj.GetType.GetProperties
        For Each info As System.Reflection.PropertyInfo In inf
            al.Add(info.Name)
        Next
        Return al
    End Function

    Protected Function PopulateProperties(ByVal propertiesList As ArrayList, _
                                            ByVal dt As System.Data.DataTable, _
                                            ByVal webPrice As DEWebPrice, _
                                            ByVal rowIndex As Integer) As Object
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To propertiesList.Count - 1
                If dt.Columns.Contains(propertiesList(i)) Then
                    CallByName(webPrice, _
                                propertiesList(i), _
                                CallType.Set, _
                                CheckDBNull(dt.Rows(rowIndex)(propertiesList(i))))
                Else
                    'If the column does not exist, handle any properties on the class that we know of
                    Select Case UCase(propertiesList(i))
                        Case "PRODUCT_CODE"
                            webPrice.PRODUCT_CODE = Utilities.CheckForDBNull_String(dt.Rows(rowIndex)("PRODUCT"))
                        Case "REQUESTEDQUANTITY"
                            If Products.ContainsKey(Utilities.CheckForDBNull_String(dt.Rows(rowIndex)("PRODUCT"))) Then
                                webPrice.RequestedQuantity = Products(Utilities.CheckForDBNull_String(dt.Rows(rowIndex)("PRODUCT"))).Quantity
                            Else
                                webPrice.RequestedQuantity = 0
                            End If
                    End Select
                End If
            Next
        End If

        Return webPrice
    End Function

    Protected Function PopulateWebPriceEntity(ByVal dt As System.Data.DataTable, _
                                            ByVal webPrice As DEWebPrice, _
                                            ByVal rowIndex As Integer) As Object
        If dt.Rows.Count > 0 Then
            Dim priceListDataRow As DataRow = dt.Rows(rowIndex)
            With webPrice
                .DELIVERY_GROSS_PRICE = CheckDBNull(priceListDataRow("DELIVERY_GROSS_PRICE"))
                .DELIVERY_NET_PRICE = CheckDBNull(priceListDataRow("DELIVERY_NET_PRICE"))
                .DELIVERY_TAX_AMOUNT = CheckDBNull(priceListDataRow("DELIVERY_TAX_AMOUNT"))
                .GROSS_PRICE = CheckDBNull(priceListDataRow("GROSS_PRICE"))
                .GROSS_PRICE_10 = CheckDBNull(priceListDataRow("GROSS_PRICE_10"))
                .GROSS_PRICE_2 = CheckDBNull(priceListDataRow("GROSS_PRICE_2"))
                .GROSS_PRICE_3 = CheckDBNull(priceListDataRow("GROSS_PRICE_3"))
                .GROSS_PRICE_4 = CheckDBNull(priceListDataRow("GROSS_PRICE_4"))
                .GROSS_PRICE_5 = CheckDBNull(priceListDataRow("GROSS_PRICE_5"))
                .GROSS_PRICE_6 = CheckDBNull(priceListDataRow("GROSS_PRICE_6"))
                .GROSS_PRICE_7 = CheckDBNull(priceListDataRow("GROSS_PRICE_7"))
                .GROSS_PRICE_8 = CheckDBNull(priceListDataRow("GROSS_PRICE_8"))
                .GROSS_PRICE_9 = CheckDBNull(priceListDataRow("GROSS_PRICE_9"))
                .NET_PRICE = CheckDBNull(priceListDataRow("NET_PRICE"))
                .NET_PRICE_10 = CheckDBNull(priceListDataRow("NET_PRICE_10"))
                .NET_PRICE_2 = CheckDBNull(priceListDataRow("NET_PRICE_2"))
                .NET_PRICE_3 = CheckDBNull(priceListDataRow("NET_PRICE_3"))
                .NET_PRICE_4 = CheckDBNull(priceListDataRow("NET_PRICE_4"))
                .NET_PRICE_5 = CheckDBNull(priceListDataRow("NET_PRICE_5"))
                .NET_PRICE_6 = CheckDBNull(priceListDataRow("NET_PRICE_6"))
                .NET_PRICE_7 = CheckDBNull(priceListDataRow("NET_PRICE_7"))
                .NET_PRICE_8 = CheckDBNull(priceListDataRow("NET_PRICE_8"))
                .NET_PRICE_9 = CheckDBNull(priceListDataRow("NET_PRICE_9"))
                .PRICE_BREAK_CODE = CheckDBNull(priceListDataRow("PRICE_BREAK_CODE"))
                .PRICE_BREAK_QUANTITY_1 = CheckDBNull(priceListDataRow("PRICE_BREAK_QUANTITY_1"))
                .PRICE_BREAK_QUANTITY_10 = CheckDBNull(priceListDataRow("PRICE_BREAK_QUANTITY_10"))
                .PRICE_BREAK_QUANTITY_2 = CheckDBNull(priceListDataRow("PRICE_BREAK_QUANTITY_2"))
                .PRICE_BREAK_QUANTITY_3 = CheckDBNull(priceListDataRow("PRICE_BREAK_QUANTITY_3"))
                .PRICE_BREAK_QUANTITY_4 = CheckDBNull(priceListDataRow("PRICE_BREAK_QUANTITY_4"))
                .PRICE_BREAK_QUANTITY_5 = CheckDBNull(priceListDataRow("PRICE_BREAK_QUANTITY_5"))
                .PRICE_BREAK_QUANTITY_6 = CheckDBNull(priceListDataRow("PRICE_BREAK_QUANTITY_6"))
                .PRICE_BREAK_QUANTITY_7 = CheckDBNull(priceListDataRow("PRICE_BREAK_QUANTITY_7"))
                .PRICE_BREAK_QUANTITY_8 = CheckDBNull(priceListDataRow("PRICE_BREAK_QUANTITY_8"))
                .PRICE_BREAK_QUANTITY_9 = CheckDBNull(priceListDataRow("PRICE_BREAK_QUANTITY_9"))
                .PRODUCT_CODE = CheckDBNull(priceListDataRow("PRODUCT"))
                If Products.ContainsKey(Utilities.CheckForDBNull_String(priceListDataRow("PRODUCT"))) Then
                    .RequestedQuantity = Products(Utilities.CheckForDBNull_String(dt.Rows(rowIndex)("PRODUCT"))).Quantity
                Else
                    .RequestedQuantity = 0
                End If
                .SALE_GROSS_PRICE = CheckDBNull(priceListDataRow("SALE_GROSS_PRICE"))
                .SALE_GROSS_PRICE_10 = CheckDBNull(priceListDataRow("SALE_GROSS_PRICE_10"))
                .SALE_GROSS_PRICE_2 = CheckDBNull(priceListDataRow("SALE_GROSS_PRICE_2"))
                .SALE_GROSS_PRICE_3 = CheckDBNull(priceListDataRow("SALE_GROSS_PRICE_3"))
                .SALE_GROSS_PRICE_4 = CheckDBNull(priceListDataRow("SALE_GROSS_PRICE_4"))
                .SALE_GROSS_PRICE_5 = CheckDBNull(priceListDataRow("SALE_GROSS_PRICE_5"))
                .SALE_GROSS_PRICE_6 = CheckDBNull(priceListDataRow("SALE_GROSS_PRICE_6"))
                .SALE_GROSS_PRICE_7 = CheckDBNull(priceListDataRow("SALE_GROSS_PRICE_7"))
                .SALE_GROSS_PRICE_8 = CheckDBNull(priceListDataRow("SALE_GROSS_PRICE_8"))
                .SALE_GROSS_PRICE_9 = CheckDBNull(priceListDataRow("SALE_GROSS_PRICE_9"))
                .SALE_NET_PRICE = CheckDBNull(priceListDataRow("SALE_NET_PRICE"))
                .SALE_NET_PRICE_10 = CheckDBNull(priceListDataRow("SALE_NET_PRICE_10"))
                .SALE_NET_PRICE_2 = CheckDBNull(priceListDataRow("SALE_NET_PRICE_2"))
                .SALE_NET_PRICE_3 = CheckDBNull(priceListDataRow("SALE_NET_PRICE_3"))
                .SALE_NET_PRICE_4 = CheckDBNull(priceListDataRow("SALE_NET_PRICE_4"))
                .SALE_NET_PRICE_5 = CheckDBNull(priceListDataRow("SALE_NET_PRICE_5"))
                .SALE_NET_PRICE_6 = CheckDBNull(priceListDataRow("SALE_NET_PRICE_6"))
                .SALE_NET_PRICE_7 = CheckDBNull(priceListDataRow("SALE_NET_PRICE_7"))
                .SALE_NET_PRICE_8 = CheckDBNull(priceListDataRow("SALE_NET_PRICE_8"))
                .SALE_NET_PRICE_9 = CheckDBNull(priceListDataRow("SALE_NET_PRICE_9"))
                .SALE_PRICE_BREAK_QUANTITY_1 = CheckDBNull(priceListDataRow("SALE_PRICE_BREAK_QUANTITY_1"))
                .SALE_PRICE_BREAK_QUANTITY_10 = CheckDBNull(priceListDataRow("SALE_PRICE_BREAK_QUANTITY_10"))
                .SALE_PRICE_BREAK_QUANTITY_2 = CheckDBNull(priceListDataRow("SALE_PRICE_BREAK_QUANTITY_2"))
                .SALE_PRICE_BREAK_QUANTITY_3 = CheckDBNull(priceListDataRow("SALE_PRICE_BREAK_QUANTITY_3"))
                .SALE_PRICE_BREAK_QUANTITY_4 = CheckDBNull(priceListDataRow("SALE_PRICE_BREAK_QUANTITY_4"))
                .SALE_PRICE_BREAK_QUANTITY_5 = CheckDBNull(priceListDataRow("SALE_PRICE_BREAK_QUANTITY_5"))
                .SALE_PRICE_BREAK_QUANTITY_6 = CheckDBNull(priceListDataRow("SALE_PRICE_BREAK_QUANTITY_6"))
                .SALE_PRICE_BREAK_QUANTITY_7 = CheckDBNull(priceListDataRow("SALE_PRICE_BREAK_QUANTITY_7"))
                .SALE_PRICE_BREAK_QUANTITY_8 = CheckDBNull(priceListDataRow("SALE_PRICE_BREAK_QUANTITY_8"))
                .SALE_PRICE_BREAK_QUANTITY_9 = CheckDBNull(priceListDataRow("SALE_PRICE_BREAK_QUANTITY_9"))
                .SALE_TAX_AMOUNT = CheckDBNull(priceListDataRow("SALE_TAX_AMOUNT"))
                .SALE_TAX_AMOUNT_10 = CheckDBNull(priceListDataRow("SALE_TAX_AMOUNT_10"))
                .SALE_TAX_AMOUNT_2 = CheckDBNull(priceListDataRow("SALE_TAX_AMOUNT_2"))
                .SALE_TAX_AMOUNT_3 = CheckDBNull(priceListDataRow("SALE_TAX_AMOUNT_3"))
                .SALE_TAX_AMOUNT_4 = CheckDBNull(priceListDataRow("SALE_TAX_AMOUNT_4"))
                .SALE_TAX_AMOUNT_5 = CheckDBNull(priceListDataRow("SALE_TAX_AMOUNT_5"))
                .SALE_TAX_AMOUNT_6 = CheckDBNull(priceListDataRow("SALE_TAX_AMOUNT_6"))
                .SALE_TAX_AMOUNT_7 = CheckDBNull(priceListDataRow("SALE_TAX_AMOUNT_7"))
                .SALE_TAX_AMOUNT_8 = CheckDBNull(priceListDataRow("SALE_TAX_AMOUNT_8"))
                .SALE_TAX_AMOUNT_9 = CheckDBNull(priceListDataRow("SALE_TAX_AMOUNT_9"))
                .TARIFF_CODE = CheckDBNull(priceListDataRow("TARIFF_CODE"))
                .TAX_AMOUNT = CheckDBNull(priceListDataRow("TAX_AMOUNT"))
                .TAX_AMOUNT_10 = CheckDBNull(priceListDataRow("TAX_AMOUNT_10"))
                .TAX_AMOUNT_2 = CheckDBNull(priceListDataRow("TAX_AMOUNT_2"))
                .TAX_AMOUNT_3 = CheckDBNull(priceListDataRow("TAX_AMOUNT_3"))
                .TAX_AMOUNT_4 = CheckDBNull(priceListDataRow("TAX_AMOUNT_4"))
                .TAX_AMOUNT_5 = CheckDBNull(priceListDataRow("TAX_AMOUNT_5"))
                .TAX_AMOUNT_6 = CheckDBNull(priceListDataRow("TAX_AMOUNT_6"))
                .TAX_AMOUNT_7 = CheckDBNull(priceListDataRow("TAX_AMOUNT_7"))
                .TAX_AMOUNT_8 = CheckDBNull(priceListDataRow("TAX_AMOUNT_8"))
                .TAX_AMOUNT_9 = CheckDBNull(priceListDataRow("TAX_AMOUNT_9"))
                .TAX_CODE = CheckDBNull(priceListDataRow("TAX_CODE"))
            End With
        End If
        Return webPrice
    End Function

    Private Function GetProductPromotionStatus(ByVal productCode As String, ByVal dtPromotions As DataTable) As Generic.Dictionary(Of String, String)
        Dim results As New Generic.Dictionary(Of String, String)

        If Not dtPromotions Is Nothing AndAlso dtPromotions.Rows.Count > 0 Then
            Dim datarows As DataRowCollection = Nothing
            Dim dvPromotions As New DataView
            dtPromotions.DefaultView.RowFilter = "PRODUCT_CODE = '" & productCode & "' AND BUSINESS_UNIT = '" & Me.WebPriceSettings.BusinessUnit & "' AND PARTNER_GROUP = '" & Me.WebPriceSettings.PartnerGroup & "' AND PARTNER = '" & Me.WebPriceSettings.Partner & "' "
            dvPromotions = dtPromotions.DefaultView
            If dvPromotions.Count > 0 Then
                datarows = dvPromotions.ToTable.Rows
            Else
                dtPromotions.DefaultView.RowFilter = "PRODUCT_CODE = '" & productCode & "' AND BUSINESS_UNIT = '" & Me.WebPriceSettings.BusinessUnit & "' AND PARTNER_GROUP = '" & Me.WebPriceSettings.PartnerGroup & "' AND PARTNER = '" & Utilities.GetAllString & "' "
                dvPromotions = dtPromotions.DefaultView
                If dvPromotions.Count > 0 Then
                    datarows = dvPromotions.ToTable.Rows
                Else
                    dtPromotions.DefaultView.RowFilter = "PRODUCT_CODE = '" & productCode & "' AND BUSINESS_UNIT = '" & Me.WebPriceSettings.BusinessUnit & "' AND PARTNER_GROUP = '" & Utilities.GetAllString & "' AND PARTNER = '" & Utilities.GetAllString & "' "
                    dvPromotions = dtPromotions.DefaultView
                    If dvPromotions.Count > 0 Then
                        datarows = dvPromotions.ToTable.Rows
                    Else
                        dtPromotions.DefaultView.RowFilter = "PRODUCT_CODE = '" & productCode & "' AND BUSINESS_UNIT = '" & Utilities.GetAllString & "' AND PARTNER_GROUP = '" & Utilities.GetAllString & "' AND PARTNER = '" & Utilities.GetAllString & "' "
                        dvPromotions = dtPromotions.DefaultView
                        If dvPromotions.Count > 0 Then
                            datarows = dvPromotions.ToTable.Rows
                        End If
                    End If
                End If
            End If

            If datarows IsNot Nothing Then
                Dim promo As DataRow = datarows(0)
                results.Add("PromotionCode", Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")))
                results.Add("PromotionType", Utilities.CheckForDBNull_String(promo("PROMOTION_TYPE")))
                results.Add("PromotionActivationMechanism", Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM")))
                results.Add("DisplayName", Utilities.CheckForDBNull_String(promo("DISPLAY_NAME")))
            End If
            dtPromotions.DefaultView.RowFilter = ""
        End If

        Return results
    End Function

End Class


