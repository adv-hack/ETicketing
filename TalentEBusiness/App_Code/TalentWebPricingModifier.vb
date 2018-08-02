Imports Microsoft.VisualBasic
Imports System.Data
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common

Public Class TalentWebPricingModifier

    Private _webPriceModifingMode As Integer = 0
    Private _basketHeaderID As String = String.Empty
    Public Property CurrentPageName() As String = String.Empty


#Region "Constructor"

    Public Sub New(ByVal basketHeaderID As String, ByVal webPriceModifingMode As Integer)
        _webPriceModifingMode = webPriceModifingMode
        _basketHeaderID = basketHeaderID
    End Sub

#End Region

#Region "Public Methods"
    Public Function GetModifiedWebPricesBasket(ByVal talBasket As TalentBasket) As TalentBasket
        Select Case _webPriceModifingMode
            Case 0
                Return talBasket
            Case 1
                talBasket = ProcessWebPricesMode_1(talBasket)
            Case 2
                talBasket = ProcessWebPricesMode_2(talBasket)
        End Select
        Return talBasket
    End Function

    Public Function GetModifiedWebPrices(ByVal tempOrderID As String, ByVal tblBasketDetails As DataTable, ByVal webPrices As TalentWebPricing) As TalentWebPricing
        Select Case _webPriceModifingMode
            Case 0
                Return webPrices
            Case 1
                webPrices = ProcessWebPricesMode_1(tblBasketDetails, webPrices)
            Case 2
                webPrices = ProcessWebPricesMode_2(tempOrderID, tblBasketDetails, webPrices)
        End Select
        Return webPrices
    End Function

#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Mode 1
    ''' This mode will calculate the tax and net prices in line level
    ''' Then recalculates all the total values in webprices
    ''' </summary>
    Private Function ProcessWebPricesMode_1(ByVal talBasket As TalentBasket) As TalentBasket
        Dim totalTax As Decimal = 0
        Dim totalNet As Decimal = 0
        Dim totalGross As Decimal = 0
        For Each tbi As TalentBasketItem In talBasket.BasketItems
            If (UCase(tbi.MODULE_) <> "TICKETING") Then
                If Not tbi.IS_FREE Then
                    If (tbi.MASTER_PRODUCT.Length > 0) Then
                        totalTax += TEBUtilities.RoundToValue((tbi.Quantity * talBasket.WebPrices.RetrievedPrices(tbi.MASTER_PRODUCT).Purchase_Price_Tax), 0.01, False)
                        totalNet += TEBUtilities.RoundToValue((tbi.Quantity * talBasket.WebPrices.RetrievedPrices(tbi.MASTER_PRODUCT).Purchase_Price_Net), 0.01, False)
                    Else
                        totalTax += TEBUtilities.RoundToValue((tbi.Quantity * talBasket.WebPrices.RetrievedPrices(tbi.Product).Purchase_Price_Tax), 0.01, False)
                        totalNet += TEBUtilities.RoundToValue((tbi.Quantity * talBasket.WebPrices.RetrievedPrices(tbi.Product).Purchase_Price_Net), 0.01, False)
                    End If
                End If
            End If
        Next
        talBasket.WebPrices = GetModifiedWebPricesTotals_1(talBasket.WebPrices, totalTax, totalNet, totalGross)
        Return talBasket
    End Function

    Private Function ProcessWebPricesMode_1(ByVal tblBasketDetails As DataTable, ByVal webPrices As TalentWebPricing) As TalentWebPricing
        Dim totalTax As Decimal = 0
        Dim totalNet As Decimal = 0
        Dim totalGross As Decimal = 0
        'now modify the total in talWebPrice as per modifying rule
        For Each bItem As Data.DataRow In tblBasketDetails.Rows
            If UCase(Utilities.CheckForDBNull_String(bItem("MODULE"))) <> "TICKETING" Then
                If Not TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(bItem("IS_FREE")) Then
                    If Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(bItem("MASTER_PRODUCT"))) Then
                        totalTax += TEBUtilities.RoundToValue((TEBUtilities.CheckForDBNull_Decimal(bItem("QUANTITY")) * webPrices.RetrievedPrices(bItem("MASTER_PRODUCT")).Purchase_Price_Tax), 0.01, False)
                        totalNet += TEBUtilities.RoundToValue((TEBUtilities.CheckForDBNull_Decimal(bItem("QUANTITY")) * webPrices.RetrievedPrices(bItem("MASTER_PRODUCT")).Purchase_Price_Net), 0.01, False)
                    Else
                        totalTax += TEBUtilities.RoundToValue((TEBUtilities.CheckForDBNull_Decimal(bItem("QUANTITY")) * webPrices.RetrievedPrices(bItem("PRODUCT")).Purchase_Price_Tax), 0.01, False)
                        totalNet += TEBUtilities.RoundToValue((TEBUtilities.CheckForDBNull_Decimal(bItem("QUANTITY")) * webPrices.RetrievedPrices(bItem("PRODUCT")).Purchase_Price_Net), 0.01, False)
                    End If
                End If
            End If
        Next
        webPrices = GetModifiedWebPricesTotals_1(webPrices, totalTax, totalNet, totalGross)
        Return webPrices
    End Function

    Private Function GetModifiedWebPricesTotals_1(ByVal webPrices As TalentWebPricing, ByVal totalTax As Decimal, ByVal totalNet As Decimal, ByVal totalGross As Decimal) As TalentWebPricing

        webPrices.Total_Items_Value_Net = totalNet
        webPrices.Total_Items_Value_Tax = totalTax
        webPrices.Total_Items_Value_Gross = totalNet + totalTax
        If webPrices.FreeDeliveryValueForPriceList > 0 AndAlso webPrices.Total_Items_Value_Gross >= webPrices.FreeDeliveryValueForPriceList Then
            webPrices.Qualifies_For_Free_Delivery = True
            webPrices.Total_Order_Value_Gross = webPrices.Total_Items_Value_Gross
            webPrices.Total_Order_Value_Net = webPrices.Total_Items_Value_Net
            webPrices.Total_Order_Value_Tax = webPrices.Total_Items_Value_Tax
        Else
            webPrices.Qualifies_For_Free_Delivery = False
            webPrices.Total_Order_Value_Gross = webPrices.Total_Items_Value_Gross + webPrices.Max_Delivery_Gross
            webPrices.Total_Order_Value_Net = webPrices.Total_Items_Value_Net + webPrices.Max_Delivery_Net
            webPrices.Total_Order_Value_Tax = webPrices.Total_Items_Value_Tax + webPrices.Max_Delivery_Tax
        End If
        Return webPrices
    End Function
    ''' <summary>
    ''' Mode 2
    ''' This mode will make the tax as zero if the delivery country is exempted from tax 
    ''' Then recalculate the net and gross prices in line level
    ''' as well as all the total values in webprices
    ''' </summary>
    Private Function ProcessWebPricesMode_2(ByVal talBasket As TalentBasket) As TalentBasket
        Dim isWebPricesModified As Boolean = False
        If IsValidForTaxExemption(talBasket.Temp_Order_Id) Then
            Dim totalTax As Decimal = 0
            Dim totalNet As Decimal = 0
            Dim totalGross As Decimal = 0
            For Each tbi As TalentBasketItem In talBasket.BasketItems
                If (UCase(tbi.MODULE_) <> "TICKETING") Then
                    If Not tbi.IS_FREE Then
                        If (tbi.MASTER_PRODUCT.Length > 0) AndAlso talBasket.WebPrices.RetrievedPrices.ContainsKey(tbi.MASTER_PRODUCT) Then
                            'totalTax += TEBUtilities.RoundToValue((tbi.Quantity * talBasket.WebPrices.RetrievedPrices(tbi.MASTER_PRODUCT).Purchase_Price_Tax), 0.01, False)
                            totalGross += TEBUtilities.RoundToValue((tbi.Quantity * (talBasket.WebPrices.RetrievedPrices(tbi.MASTER_PRODUCT).Purchase_Price_Gross - talBasket.WebPrices.RetrievedPrices(tbi.MASTER_PRODUCT).Purchase_Price_Tax)), 0.01, False)
                            talBasket.WebPrices.RetrievedPrices(tbi.MASTER_PRODUCT).Purchase_Price_Gross = talBasket.WebPrices.RetrievedPrices(tbi.MASTER_PRODUCT).Purchase_Price_Gross - talBasket.WebPrices.RetrievedPrices(tbi.MASTER_PRODUCT).Purchase_Price_Tax
                            talBasket.WebPrices.RetrievedPrices(tbi.MASTER_PRODUCT).Purchase_Price_Tax = 0
                            talBasket.WebPrices.RetrievedPrices(tbi.MASTER_PRODUCT).DisplayPrice = talBasket.WebPrices.RetrievedPrices(tbi.MASTER_PRODUCT).Purchase_Price_Gross
                            totalNet += TEBUtilities.RoundToValue((tbi.Quantity * talBasket.WebPrices.RetrievedPrices(tbi.MASTER_PRODUCT).Purchase_Price_Net), 0.01, False)
                        Else
                            'totalTax += TEBUtilities.RoundToValue((tbi.Quantity * talBasket.WebPrices.RetrievedPrices(tbi.Product).Purchase_Price_Tax), 0.01, False)
                            totalGross += TEBUtilities.RoundToValue((tbi.Quantity * (talBasket.WebPrices.RetrievedPrices(tbi.Product).Purchase_Price_Gross - talBasket.WebPrices.RetrievedPrices(tbi.Product).Purchase_Price_Tax)), 0.01, False)
                            talBasket.WebPrices.RetrievedPrices(tbi.Product).Purchase_Price_Gross = talBasket.WebPrices.RetrievedPrices(tbi.Product).Purchase_Price_Gross - talBasket.WebPrices.RetrievedPrices(tbi.Product).Purchase_Price_Tax
                            talBasket.WebPrices.RetrievedPrices(tbi.Product).Purchase_Price_Tax = 0
                            talBasket.WebPrices.RetrievedPrices(tbi.Product).DisplayPrice = talBasket.WebPrices.RetrievedPrices(tbi.Product).Purchase_Price_Gross
                            totalNet += TEBUtilities.RoundToValue((tbi.Quantity * talBasket.WebPrices.RetrievedPrices(tbi.Product).Purchase_Price_Net), 0.01, False)
                        End If
                    End If
                End If
            Next
            talBasket.WebPrices = GetModifiedWebPricesTotals_2(True, talBasket.WebPrices, totalTax, totalNet, totalGross)
        End If

        Return talBasket
    End Function

    Private Function ProcessWebPricesMode_2(ByVal tempOrderID As String, ByVal tblBasketDetails As DataTable, ByVal webPrices As TalentWebPricing) As TalentWebPricing
        If IsValidForTaxExemption(tempOrderID) Then
            Dim totalTax As Decimal = 0
            Dim totalNet As Decimal = 0
            Dim totalGross As Decimal = 0
            'now modify the total in talWebPrice as per modifying rule
            For Each bItem As Data.DataRow In tblBasketDetails.Rows
                If UCase(Utilities.CheckForDBNull_String(bItem("MODULE"))) <> "TICKETING" Then
                    If Not TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(bItem("IS_FREE")) Then
                        If (Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(bItem("MASTER_PRODUCT")))) AndAlso webPrices.RetrievedPrices.ContainsKey(bItem("MASTER_PRODUCT")) Then
                            'totalTax += TEBUtilities.RoundToValue((TEBUtilities.CheckForDBNull_Decimal(bItem("QUANTITY")) * webPrices.RetrievedPrices(bItem("MASTER_PRODUCT")).Purchase_Price_Tax), 0.01, False)
                            totalGross += TEBUtilities.RoundToValue((TEBUtilities.CheckForDBNull_Decimal(bItem("QUANTITY")) * (webPrices.RetrievedPrices(bItem("MASTER_PRODUCT")).Purchase_Price_Gross - webPrices.RetrievedPrices(bItem("MASTER_PRODUCT")).Purchase_Price_Tax)), 0.01, False)
                            webPrices.RetrievedPrices(bItem("MASTER_PRODUCT")).Purchase_Price_Gross = webPrices.RetrievedPrices(bItem("MASTER_PRODUCT")).Purchase_Price_Gross - webPrices.RetrievedPrices(bItem("MASTER_PRODUCT")).Purchase_Price_Tax
                            webPrices.RetrievedPrices(bItem("MASTER_PRODUCT")).Purchase_Price_Tax = 0
                            webPrices.RetrievedPrices(bItem("MASTER_PRODUCT")).DisplayPrice = webPrices.RetrievedPrices(bItem("MASTER_PRODUCT")).Purchase_Price_Gross
                            totalNet += TEBUtilities.RoundToValue((TEBUtilities.CheckForDBNull_Decimal(bItem("QUANTITY")) * webPrices.RetrievedPrices(bItem("MASTER_PRODUCT")).Purchase_Price_Net), 0.01, False)
                        Else
                            'totalTax += TEBUtilities.RoundToValue((TEBUtilities.CheckForDBNull_Decimal(bItem("QUANTITY")) * webPrices.RetrievedPrices(bItem("PRODUCT")).Purchase_Price_Tax), 0.01, False)
                            totalGross += TEBUtilities.RoundToValue((TEBUtilities.CheckForDBNull_Decimal(bItem("QUANTITY")) * (webPrices.RetrievedPrices(bItem("PRODUCT")).Purchase_Price_Gross - webPrices.RetrievedPrices(bItem("PRODUCT")).Purchase_Price_Tax)), 0.01, False)
                            webPrices.RetrievedPrices(bItem("PRODUCT")).Purchase_Price_Gross = webPrices.RetrievedPrices(bItem("PRODUCT")).Purchase_Price_Gross - webPrices.RetrievedPrices(bItem("PRODUCT")).Purchase_Price_Tax
                            webPrices.RetrievedPrices(bItem("PRODUCT")).Purchase_Price_Tax = 0
                            webPrices.RetrievedPrices(bItem("PRODUCT")).DisplayPrice = webPrices.RetrievedPrices(bItem("PRODUCT")).Purchase_Price_Gross
                            totalNet += TEBUtilities.RoundToValue((TEBUtilities.CheckForDBNull_Decimal(bItem("QUANTITY")) * webPrices.RetrievedPrices(bItem("PRODUCT")).Purchase_Price_Net), 0.01, False)
                        End If
                    End If
                End If
            Next
            webPrices = GetModifiedWebPricesTotals_2(True, webPrices, totalTax, totalNet, totalGross)
        End If
        Return webPrices
    End Function

    Private Function GetModifiedWebPricesTotals_2(ByVal isWebPricesModified As Boolean, ByVal webPrices As TalentWebPricing, ByVal totalTax As Decimal, ByVal totalNet As Decimal, ByVal totalGross As Decimal) As TalentWebPricing
        webPrices.Total_Items_Value_Net = totalNet
        webPrices.Total_Items_Value_Tax = totalTax
        webPrices.Total_Items_Value_Gross = totalGross 'totalNet + totalTax
        If webPrices.FreeDeliveryValueForPriceList > 0 AndAlso webPrices.Total_Items_Value_Gross >= webPrices.FreeDeliveryValueForPriceList Then
            webPrices.Qualifies_For_Free_Delivery = True
            webPrices.Total_Order_Value_Gross = webPrices.Total_Items_Value_Gross
            webPrices.Total_Order_Value_Net = webPrices.Total_Items_Value_Net
            webPrices.Total_Order_Value_Tax = webPrices.Total_Items_Value_Tax
        Else
            webPrices.Qualifies_For_Free_Delivery = False
            webPrices.Total_Order_Value_Gross = webPrices.Total_Items_Value_Gross + webPrices.Max_Delivery_Gross
            webPrices.Total_Order_Value_Net = webPrices.Total_Items_Value_Net + webPrices.Max_Delivery_Net
            webPrices.Total_Order_Value_Tax = webPrices.Total_Items_Value_Tax + webPrices.Max_Delivery_Tax
        End If
        webPrices.IsWebPricesModified = isWebPricesModified
        'TEBUtilities.UpdateRetailBasketItems(_basketHeaderID, webPrices)
        Return webPrices
    End Function
    Private Function IsValidForTaxExemption(ByVal tempOrderID As String) As Boolean
        Dim isValid As Boolean = False
        If CurrentPageName IsNot Nothing AndAlso (CurrentPageName.ToLower() = "checkout.aspx" OrElse CurrentPageName.ToLower() = "checkoutorderconfirmation.aspx") Then
            isValid = TEBUtilities.IsValidForTaxExemption(tempOrderID)
        End If
        Return isValid
    End Function

#End Region


End Class
