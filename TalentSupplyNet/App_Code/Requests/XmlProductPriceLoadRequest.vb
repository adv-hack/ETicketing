Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Requests
'
'       Date                        26/10/09
'
'       Author                      Ben Ford
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPPPLRQ - 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlProductPriceLoadRequest
        Inherits XmlRequest

        'Invoke constructor on base, passing web service name
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub

        Private _productPriceLoad As DEProductPriceLoad
        Public Property ProductPriceLoad() As DEProductPriceLoad
            Get
                Return _productPriceLoad
            End Get
            Set(ByVal value As DEProductPriceLoad)
                _productPriceLoad = value
            End Set
        End Property
        Private _dep As DEProduct

        Public Overloads Property Dep() As DEProduct
            Get
                Return _dep
            End Get
            Set(ByVal value As DEProduct)
                _dep = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlProductPriceLoadResponse = CType(xmlResp, XmlProductPriceLoadResponse)
            Dim err As ErrorObj = Nothing
            ProductPriceLoad = New DEProductPriceLoad

            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim product As New TalentProduct
            If err.HasError Then
                xmlResp.Err = err
            Else
                With product
                    .DE_ProductPriceLoad = ProductPriceLoad
                    .Settings = Settings
                    .Dep = Dep
                    err = .ProductPriceLoad
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            xmlAction.ResultDataSet = product.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            Dep = New DEProduct
            Dim Node1, Node2, Node3 As XmlNode

            Dim taxCode As DETaxCode
            Dim currencyCode As DECurrencyCode
            Dim priceList As DEPriceList
            Dim priceDefault As DEPriceDefault

            Dim taxCodeCount As Integer = 0
            Dim currencyCodeCount As Integer = 0
            Dim priceListCount As Integer = 0
            Dim defaultCount As Integer = 0

            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//ProductPriceLoadRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Dep.CollDETrans.Add(Extract_TransactionHeader(Node1))

                        Case Is = "TaxCodes"
                            '---------------------------
                            ' Build Tax codes Collection
                            '---------------------------
                            ProductPriceLoad.TotalTaxCodes = Node1.Attributes("Total").Value
                            ProductPriceLoad.TaxCodeMode = Node1.Attributes("Mode").Value
                            ProductPriceLoad.ColTaxCodes = New Collection
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "TaxCode"
                                        taxCodeCount += 1

                                        taxCode = New DETaxCode
                                        taxCode.Mode = Node2.Attributes("Mode").Value
                                        With taxCode
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "Code" : .TaxCode = Node3.InnerText
                                                    Case Is = "Description" : .Description = Node3.InnerText
                                                End Select
                                            Next
                                        End With
                                        ProductPriceLoad.ColTaxCodes.Add(taxCode)
                                End Select
                            Next
                            If taxCodeCount <> ProductPriceLoad.TotalTaxCodes Then
                                With err
                                    .ErrorMessage = "Invalid number of tax codes"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPPPLRQ-02"
                                    .HasError = True
                                End With
                            End If

                        Case Is = "CurrencyCodes"
                            '--------------------------------
                            ' Build Currency codes Collection
                            '--------------------------------
                            ProductPriceLoad.TotalCurrencyCodes = Node1.Attributes("Total").Value
                            ProductPriceLoad.CurrencyCodeMode = Node1.Attributes("Mode").Value
                            ProductPriceLoad.ColCurrencyCodes = New Collection
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "CurrencyCode"
                                        currencyCodeCount += 1

                                        currencyCode = New DECurrencyCode
                                        currencyCode.Mode = Node2.Attributes("Mode").Value
                                        With currencyCode
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "Code" : .CurrencyCode = Node3.InnerText
                                                    Case Is = "Description" : .Description = Node3.InnerText
                                                    Case Is = "HTMLCurrencySymbol" : .HtmlCurrencySymbol = Node3.InnerText
                                                End Select
                                            Next
                                        End With
                                        ProductPriceLoad.ColCurrencyCodes.Add(currencyCode)
                                End Select
                            Next
                            If currencyCodeCount <> ProductPriceLoad.TotalCurrencyCodes Then
                                With err
                                    .ErrorMessage = "Invalid number of currency codes"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPPPLRQ-03"
                                    .HasError = True
                                End With
                            End If

                        Case Is = "PriceLists"
                            '-----------------------------
                            ' Build Price lists collection
                            '-----------------------------
                            ProductPriceLoad.TotalPriceLists = Node1.Attributes("Total").Value
                            ProductPriceLoad.PriceListMode = Node1.Attributes("Mode").Value
                            ProductPriceLoad.ColPriceLists = New Collection
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "PriceList"
                                        priceListCount += 1
                                        priceList = New DEPriceList
                                        priceList.Mode = Node2.Attributes("Mode").Value
                                        With priceList
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "Code" : .Code = Node3.InnerText
                                                    Case Is = "Description" : .Description = Node3.InnerText
                                                    Case Is = "CurrencyCode" : .CurrencyCode = Node3.InnerText
                                                    Case Is = "FreeDeliveryValue" : .FreeDeliveryValue = Node3.InnerText
                                                    Case Is = "MinimumDeliveryValue" : .MinimumDeliveryValue = Node3.InnerText
                                                    Case Is = "StartDate" : .StartDate = Node3.InnerText
                                                    Case Is = "EndDate" : .EndDate = Node3.InnerText
                                                    Case Is = "Products"
                                                        .ProductPricesMode = Node3.Attributes("Mode").Value
                                                        .TotalProductPrices = Node3.Attributes("Total").Value
                                                        .ColProductPrice = ExtractProducts(Node3)
                                                        If .TotalProductPrices <> .ColProductPrice.Count Then
                                                            err.ErrorMessage = "Invalid number of products in price list " & .Code
                                                            err.ErrorStatus = ModuleName & " Error: " & err.ErrorMessage
                                                            err.ErrorNumber = "TTPPPLRQ-04"
                                                            err.HasError = True
                                                        End If
                                                End Select
                                            Next
                                        End With

                                        ProductPriceLoad.ColPriceLists.Add(priceList)
                                End Select
                            Next

                        Case Is = "Defaults"
                            '---------------
                            ' Build defaults
                            '---------------
                            ProductPriceLoad.TotalDefaults = Node1.Attributes("Total").Value
                            ProductPriceLoad.DefaultsMode = Node1.Attributes("Mode").Value
                            ProductPriceLoad.ColDefaults = New Collection
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "Default"
                                        If Node2.Attributes("Name").Value = "PRICE_LIST" OrElse _
                                            Node2.Attributes("Name").Value = "SHOW_PRICES_EXCLUDING_VAT" Then

                                            defaultCount += 1

                                            priceDefault = New DEPriceDefault
                                            priceDefault.DefaultName = Node2.Attributes("Name").Value
                                            With priceDefault
                                                .BusinessUnit = Node2.Attributes("BusinessUnit").Value
                                                .Partner = Node2.Attributes("Partner").Value
                                                .Value = Node2.InnerText
                                                .Mode = Node2.Attributes("Mode").Value
                                            End With
                                            ProductPriceLoad.ColDefaults.Add(priceDefault)
                                        End If


                                End Select
                            Next
                            If taxCodeCount <> ProductPriceLoad.TotalTaxCodes Then
                                With err
                                    .ErrorMessage = "Invalid number of tax codes"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPPPLRQ-02"
                                    .HasError = True
                                End With
                            End If

                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function ExtractProducts(ByVal productNode As XmlNode) As Collection
            Dim err As New ErrorObj
            Dim colProducts As New Collection
            Dim productPrice As DEProductPrice
            Dim node1, node2 As XmlNode
            For Each node1 In productNode.ChildNodes
                Select Case node1.Name
                    Case Is = "Product"
                        productPrice = New DEProductPrice
                        productPrice.Mode = node1.Attributes("Mode").Value
                        With productPrice
                            For Each node2 In node1.ChildNodes
                                Select Case node2.Name
                                    Case Is = "SKU" : .SKU = node2.InnerText
                                    Case Is = "FromDate" : .FromDate = node2.InnerText
                                    Case Is = "ToDate" : .ToDate = node2.InnerText
                                    Case Is = "TariffCode" : .TariffCode = node2.InnerText
                                    Case Is = "TaxCode" : .TaxCode = node2.InnerText
                                    Case Is = "FromDate" : .FromDate = node2.InnerText
                                    Case Is = "Price"
                                        .Price = ExtractPriceBreak(node2)
                                    Case Is = "PriceBreak1"
                                        .PriceBreak1 = ExtractPriceBreak(node2)
                                    Case Is = "PriceBreak2"
                                        .PriceBreak2 = ExtractPriceBreak(node2)
                                    Case Is = "PriceBreak3"
                                        .PriceBreak3 = ExtractPriceBreak(node2)
                                    Case Is = "PriceBreak4"
                                        .PriceBreak4 = ExtractPriceBreak(node2)
                                    Case Is = "PriceBreak5"
                                        .PriceBreak5 = ExtractPriceBreak(node2)
                                    Case Is = "PriceBreak6"
                                        .PriceBreak6 = ExtractPriceBreak(node2)
                                    Case Is = "PriceBreak7"
                                        .PriceBreak7 = ExtractPriceBreak(node2)
                                    Case Is = "PriceBreak8"
                                        .PriceBreak8 = ExtractPriceBreak(node2)
                                    Case Is = "PriceBreak9"
                                        .PriceBreak9 = ExtractPriceBreak(node2)
                                    Case Is = "PriceBreak10"
                                        .PriceBreak10 = ExtractPriceBreak(node2)

                                End Select
                            Next

                        End With
                        colProducts.Add(productPrice)
                End Select

            Next

            Return colProducts
        End Function
        Private Function ExtractPriceBreak(ByVal priceNode As XmlNode) As DEPricingDetails
            Dim pricingDetails As New DEPricingDetails
            Dim node1, node2, node3 As XmlNode
            For Each node1 In priceNode.ChildNodes
                With pricingDetails
                    Select Case node1.Name
                        Case Is = "PriceBreakCode" : .PriceBreakCode = node1.InnerText
                        Case Is = "PriceBreakQuantity" : .PriceBreakQty = node1.InnerText
                        Case Is = "Gross" : .GrossPrice = node1.InnerText
                        Case Is = "Net" : .NetPrice = node1.InnerText
                        Case Is = "Tax" : .Tax = node1.InnerText
                        Case Is = "Delivery"
                            For Each node2 In node1.ChildNodes
                                Select Case node2.Name
                                    Case Is = "Gross" : .DeliveryGross = node2.InnerText
                                    Case Is = "Net" : .DeliveryNet = node2.InnerText
                                    Case Is = "Tax" : .DeliveryTax = node2.InnerText
                                End Select
                            Next
                        Case Is = "Sale"
                            For Each node2 In node1.ChildNodes
                                Select Case node2.Name
                                    Case Is = "PriceBreakQuantity" : .SalePriceBreakQuantity = node2.InnerText
                                    Case Is = "Gross" : .SalePriceGross = node2.InnerText
                                    Case Is = "Net" : .SalePriceNet = node2.InnerText
                                    Case Is = "Delivery"
                                        For Each node3 In node2.ChildNodes
                                            Select Case node2.Name
                                                Case Is = "Gross" : .SaleDeliveryGross = node3.InnerText
                                                Case Is = "Net" : .SaleDeliveryNet = node3.InnerText
                                                Case Is = "Tax" : .SaleDeliveryTax = node3.InnerText
                                            End Select
                                        Next
                                    Case Is = "Tax" : .SalePriceTax = node2.InnerText
                                End Select
                            Next
                    End Select
                End With

            Next

            Return pricingDetails
        End Function
    End Class

End Namespace
