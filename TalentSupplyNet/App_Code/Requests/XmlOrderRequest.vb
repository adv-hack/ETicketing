Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Requests
'
'       Date                        Nov 2006
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQOR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       30/08/07    /001    Ben     Add 'LineComment' to product lines
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlOrderRequest
        Inherits XmlRequest

        Private _customerIDFromXmlV1_1 As String = String.Empty

        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlOrderResponse = CType(xmlResp, XmlOrderResponse)
            Dim err As ErrorObj = Nothing
            Dim pricingErr As New ErrorObj
            Dim altProductsErr As New ErrorObj
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
                Case Is = "1.1"
                    err = LoadXmlV1_1()
            End Select

            Dim ss As DEOrderSettings
            ss = CType(Settings(), DEOrderSettings)
            '-----------------------------------------
            ' Check if need to search for alt products
            '-----------------------------------------            
            If ss.OrderCheckForAltProducts Then
                altProductsErr = CheckForAltProducts()
            End If
            '-----------------------------------------------
            ' Check if need to reprice blank incoming prices
            '-----------------------------------------------
            If ss.RepriceBlankPrice AndAlso Not altProductsErr.HasError Then
                pricingErr = RepriceBlankPrices()
            End If

            '-------------------------------
            ' Place the Request if no errors
            '-------------------------------
            Dim ORDER As New TalentOrder
            If err.HasError Then
                xmlResp.Err = err
            Else
                If altProductsErr.HasError Then
                    xmlResp.Err = altProductsErr
                Else
                    If pricingErr.HasError Then
                        xmlResp.Err = pricingErr
                    Else
                        If (_customerIDFromXmlV1_1.Length > 0) Then
                            Settings.LoginId = _customerIDFromXmlV1_1
                            _customerIDFromXmlV1_1 = String.Empty
                        End If

                        With ORDER
                            .Dep = Dep
                            .Settings = Settings
                            err = .Create
                        End With
                        If err.HasError Or Not err Is Nothing Then
                            xmlResp.Err = err
                        End If
                    End If
                End If
            End If

            xmlAction.ResultDataSet = ORDER.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function

        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2, Node3, Node4, Node5 As XmlNode
            Dim detr As New DETransaction           ' Items
            Dim deos As New DeOrders                ' DeOrderHeader, DEOrderInfo  as collections
            Dim deoh As New DeOrderHeader           ' Items and DEAddress as Collection
            Dim dead1 As New DeAddress              ' Single Line Item
            Dim dead2 As New DeAddress              ' Single Line Item
            '
            Dim deop As New DEPayments              ' Multiple Line Item
            Dim deoc As New DECharges               ' Multiple Line Item
            '
            Dim deoi As New DEOrderInfo             ' DEProductLines, DECommentLines  as collections
            Dim depr As DeProductLines              ' Multiple Line Item
            Dim decl As DeCommentLines              ' Multiple Line Item
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Dep = New DEOrder
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//OrderRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Dep.CollDETrans.Add(Extract_TransactionHeader(Node1))

                        Case Is = "Order"
                            deos = New DeOrders
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "OrderHeaderInformation"
                                        deoh = New DeOrderHeader

                                        deoh.ProjectedDeliveryDate = New Date(1900, 1, 1)
                                        With deoh
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "BillToSuffix"
                                                        .BillToSuffix = Node3.InnerText
                                                    Case Is = "ShipToSuffix"
                                                        .ShipToSuffix = Node3.InnerText
                                                    Case Is = "AddressingInformation"
                                                        For Each Node4 In Node3.ChildNodes
                                                            '
                                                            Select Case Node4.Name
                                                                Case Is = "CustomerPO" : .CustomerPO = Node4.InnerText
                                                                Case Is = "EndUserPO" : .EndUserPO = Node4.InnerText
                                                                Case Is = "ShipTo"
                                                                    dead1 = New DeAddress
                                                                    dead1.Category = Node4.Name
                                                                    With dead1
                                                                        For Each Node5 In Node4.SelectSingleNode("Address").ChildNodes
                                                                            Select Case Node5.Name
                                                                                '-----------------------------------------------------------
                                                                                '   Shipping Address info
                                                                                '
                                                                                Case Is = "ShipToAttention" : .ContactName = Node5.InnerText
                                                                                Case Is = "ShipToAddress1" : .Line1 = Node5.InnerText
                                                                                Case Is = "ShipToAddress2" : .Line2 = Node5.InnerText
                                                                                Case Is = "ShipToAddress3" : .Line3 = Node5.InnerText
                                                                                Case Is = "ShipToCity" : .City = Node5.InnerText
                                                                                Case Is = "ShipToProvince" : .Province = Node5.InnerText
                                                                                Case Is = "ShipToPostalCode" : .PostalCode = Node5.InnerText
                                                                            End Select
                                                                        Next Node5
                                                                    End With
                                                                    deoh.CollDEAddress.Add(dead1)
                                                                Case Is = "BillTo"
                                                                    dead2 = New DeAddress
                                                                    dead2.Category = Node4.Name
                                                                    With dead2
                                                                        For Each Node5 In Node4.SelectSingleNode("Address").ChildNodes
                                                                            Select Case Node5.Name
                                                                                '----------------------------------------------------------
                                                                                '   Billing Address info
                                                                                '
                                                                                Case Is = "BillToAttention" : .ContactName = Node5.InnerText
                                                                                Case Is = "BillToAddress1" : .Line1 = Node5.InnerText
                                                                                Case Is = "BillToAddress2" : .Line2 = Node5.InnerText
                                                                                Case Is = "BillToAddress3" : .Line3 = Node5.InnerText
                                                                                Case Is = "BillToCity" : .City = Node5.InnerText
                                                                                Case Is = "BillToProvince" : .Province = Node5.InnerText
                                                                                Case Is = "BillToPostalCode" : .PostalCode = Node5.InnerText

                                                                            End Select
                                                                        Next Node5
                                                                    End With
                                                                    deoh.CollDEAddress.Add(dead2)
                                                            End Select
                                                        Next Node4
                                                    Case Is = "ProcessingOptions"
                                                        For Each Node4 In Node3.ChildNodes
                                                            Select Case Node4.Name
                                                                Case Is = "CarrierCode" : .CarrierCode = Node4.InnerText
                                                                Case Is = "CarrierCodeValue" : .CarrierCodeValue = Node4.InnerText
                                                                Case Is = "AutoRelease" : .AutoRelease = Node4.InnerText
                                                                Case Is = "SalesPerson" : .SalesPerson = Node4.InnerText
                                                                Case Is = "OrderDueDate" : .OrderDueDate = Node4.InnerText
                                                                Case Is = "SuspendCode" : .SuspendCode = Node4.InnerText
                                                                Case Is = "BackOrderFlag" : .BackOrderFlag = Node4.InnerText
                                                                Case Is = "ShipmentOptions"
                                                                    For Each Node5 In Node4.ChildNodes
                                                                        Select Case Node5.Name
                                                                            Case Is = "BackOrderFlag" : .BackOrderFlag = Node5.InnerText
                                                                            Case Is = "SplitShipmentFlag" : .SplitShipmentFlag = Node5.InnerText
                                                                            Case Is = "SplitLine" : .SplitLine = Node5.InnerText
                                                                            Case Is = "ShipFromBranches" : .ShipFromBranches = Node5.InnerText
                                                                        End Select
                                                                    Next Node5
                                                                Case Is = "MessageLines" : .MessageLines = Node4.InnerText
                                                            End Select
                                                        Next Node4
                                                    Case Is = "Payment"
                                                        '------------------------------------------------------------------------------
                                                        ' see Utilities.ValidateCardNumber
                                                        '
                                                        deop = New DEPayments
                                                        For Each Node4 In Node3.ChildNodes
                                                            With deop
                                                                Select Case Node4.Name
                                                                    Case Is = "PaymentType" : .PaymentType = Node4.InnerText
                                                                    Case Is = "PaymentDate" : .PaymentDate = Node4.InnerText
                                                                    Case Is = "Bank" : .Bank = Node4.InnerText
                                                                    Case Is = "Branch" : .Branch = Node4.InnerText
                                                                    Case Is = "Sortcode" : .SortCode = Node4.InnerText
                                                                    Case Is = "Amount" : .Amount = Node4.InnerText
                                                                    Case Is = "AccountDetails", "CardDetails", "CreditCardDetails"
                                                                        For Each Node5 In Node4.ChildNodes
                                                                            Select Case Node5.Name
                                                                                Case Is = "AccountType" : .AccountType = Node5.InnerText
                                                                                Case Is = "AccountNumber" : .AccountNumber = Node5.InnerText
                                                                                Case Is = "AccountName" : .AccountName = Node5.InnerText
                                                                                Case Is = "AccountpostCode" : .AccountpostCode = Node5.InnerText
                                                                                Case Is = "AccountAddress" : .AccountAddress = Node5.InnerText
                                                                                    '
                                                                                Case Is = "CardType" : .CardType = Node5.InnerText
                                                                                Case Is = "CardNumber" : .CardNumber = Node5.InnerText
                                                                                Case Is = "CardHolderName" : .CardName = Node5.InnerText
                                                                                Case Is = "CardHolderPostCode" : .CardpostCode = Node5.InnerText
                                                                                Case Is = "CardHolderAddress" : .CardAddress = Node5.InnerText
                                                                                    '
                                                                                Case Is = "ExpiryDate" : .ExpiryDate = Node5.InnerText
                                                                                Case Is = "StartDate" : .StartDate = Node5.InnerText
                                                                                Case Is = "IssueNumber" : .IssueNumber = Node5.InnerText
                                                                                Case Is = "CV2Number" : .CV2Number = Node5.InnerText
                                                                            End Select
                                                                        Next Node5
                                                                End Select
                                                            End With
                                                        Next Node4
                                                        deoh.CollDEPayments.Add(deop)

                                                    Case Is = "OrderCharges"
                                                        For Each Node4 In Node3.ChildNodes
                                                            deoc = New DECharges
                                                            deoc.Charge = Node4.InnerText
                                                            deoh.CollDECharges.Add(deoc)
                                                        Next Node4

                                                    Case Is = "DynamicMessage"
                                                        For Each Node4 In Node3.ChildNodes
                                                            .MessageLines &= Node4.InnerText
                                                        Next Node4
                                                    Case Is = "Extension"
                                                        For Each Node4 In Node3.ChildNodes
                                                            Select Case Node4.Name
                                                                Case Is = "Reference1" : .ExtensionReference1 = Node4.InnerText
                                                                Case Is = "Reference2" : .ExtensionReference2 = Node4.InnerText
                                                                Case Is = "Reference3" : .ExtensionReference3 = Node4.InnerText
                                                                Case Is = "Reference4" : .ExtensionReference4 = Node4.InnerText
                                                                Case Is = "FixedPrice1" : .ExtensionFixedPrice1 = Node4.InnerText
                                                                Case Is = "FixedPrice2" : .ExtensionFixedPrice2 = Node4.InnerText
                                                                Case Is = "FixedPrice3" : .ExtensionFixedPrice3 = Node4.InnerText
                                                                Case Is = "FixedPrice4" : .ExtensionFixedPrice4 = Node4.InnerText
                                                                Case Is = "DealID1" : .ExtensionDealID1 = Node4.InnerText
                                                                Case Is = "DealID2" : .ExtensionDealID2 = Node4.InnerText
                                                                Case Is = "DealID3" : .ExtensionDealID3 = Node4.InnerText
                                                                Case Is = "DealID4" : .ExtensionDealID4 = Node4.InnerText
                                                                Case Is = "DealID5" : .ExtensionDealID5 = Node4.InnerText
                                                                Case Is = "DealID6" : .ExtensionDealID6 = Node4.InnerText
                                                                Case Is = "DealID7" : .ExtensionDealID7 = Node4.InnerText
                                                                Case Is = "DealID8" : .ExtensionDealID8 = Node4.InnerText
                                                                Case Is = "Flag1" : .ExtensionFlag1 = Node4.InnerText
                                                                Case Is = "Flag2" : .ExtensionFlag2 = Node4.InnerText
                                                                Case Is = "Flag3" : .ExtensionFlag3 = Node4.InnerText
                                                                Case Is = "Flag4" : .ExtensionFlag4 = Node4.InnerText
                                                                Case Is = "Flag5" : .ExtensionFlag5 = Node4.InnerText
                                                                Case Is = "Flag6" : .ExtensionFlag6 = Node4.InnerText
                                                                Case Is = "Flag7" : .ExtensionFlag7 = Node4.InnerText
                                                                Case Is = "Status" : .ExtensionStatus = Node4.InnerText

                                                            End Select
                                                        Next Node4

                                                End Select
                                            Next Node3
                                        End With
                                        deos.DEOrderHeader = deoh
                                        '----------------------------------------------------------------------------------------------
                                    Case Is = "OrderLineInformation"
                                        deoi = New DEOrderInfo
                                        For Each Node3 In Node2.ChildNodes
                                            Select Case Node3.Name
                                                Case Is = "ProductLine"
                                                    depr = New DeProductLines
                                                    With depr
                                                        For Each Node4 In Node3.ChildNodes
                                                            .Category = "Insert"
                                                            Select Case Node4.Name
                                                                Case Is = "SKU" : .SKU = Node4.InnerText
                                                                Case Is = "AlternateSKU" : .AlternateSKU = Node4.InnerText
                                                                Case Is = "Quantity" : .Quantity = Node4.InnerText
                                                                Case Is = "FixedPrice" : .FixedPrice = Node4.InnerText
                                                                Case Is = "CustomerLineNumber" : .CustomerLineNumber = Node4.InnerText
                                                                Case Is = "ReservedInventory"
                                                                    For Each Node5 In Node4.ChildNodes
                                                                        Select Case Node5.Name
                                                                            Case Is = "ReserveCode" : .ReserveCode = Node5.InnerText
                                                                            Case Is = "ReserveSequence" : .ReserveSequence = Node5.InnerText
                                                                        End Select
                                                                    Next
                                                                Case Is = "LineComment" : .LineComment = Node4.InnerText
                                                                Case Is = "Extension"
                                                                    For Each Node5 In Node4.ChildNodes
                                                                        Select Case Node5.Name
                                                                            Case Is = "Reference1" : .ExtensionReference1 = Node5.InnerText
                                                                            Case Is = "Reference2" : .ExtensionReference2 = Node5.InnerText
                                                                            Case Is = "Reference3" : .ExtensionReference3 = Node5.InnerText
                                                                            Case Is = "Reference4" : .ExtensionReference4 = Node5.InnerText
                                                                            Case Is = "Reference5" : .ExtensionReference5 = Node5.InnerText
                                                                            Case Is = "Reference6" : .ExtensionReference6 = Node5.InnerText
                                                                            Case Is = "Reference7" : .ExtensionReference7 = Node5.InnerText
                                                                            Case Is = "Reference8" : .ExtensionReference8 = Node5.InnerText
                                                                            Case Is = "Flag1" : .ExtensionFlag1 = Node5.InnerText
                                                                            Case Is = "Flag2" : .ExtensionFlag2 = Node5.InnerText
                                                                            Case Is = "Flag3" : .ExtensionFlag3 = Node5.InnerText
                                                                            Case Is = "Flag4" : .ExtensionFlag4 = Node5.InnerText
                                                                            Case Is = "Flag5" : .ExtensionFlag5 = Node5.InnerText
                                                                            Case Is = "Flag6" : .ExtensionFlag6 = Node5.InnerText
                                                                            Case Is = "Flag7" : .ExtensionFlag7 = Node5.InnerText
                                                                            Case Is = "Flag8" : .ExtensionFlag8 = Node5.InnerText
                                                                            Case Is = "Flag9" : .ExtensionFlag9 = Node5.InnerText
                                                                            Case Is = "Flag0" : .ExtensionFlag0 = Node5.InnerText
                                                                            Case Is = "Field1" : .ExtensionField1 = Node5.InnerText
                                                                            Case Is = "Field2" : .ExtensionField2 = Node5.InnerText
                                                                            Case Is = "Field3" : .ExtensionField3 = Node5.InnerText
                                                                            Case Is = "Field4" : .ExtensionField4 = Node5.InnerText
                                                                            Case Is = "FixedPrice1" : .ExtensionFixedPrice1 = Node5.InnerText
                                                                            Case Is = "FixedPrice2" : .ExtensionFixedPrice2 = Node5.InnerText
                                                                            Case Is = "FixedPrice3" : .ExtensionFixedPrice3 = Node5.InnerText
                                                                            Case Is = "FixedPrice4" : .ExtensionFixedPrice4 = Node5.InnerText
                                                                            Case Is = "FixedPrice5" : .ExtensionFixedPrice5 = Node5.InnerText
                                                                            Case Is = "FixedPrice6" : .ExtensionFixedPrice6 = Node5.InnerText
                                                                            Case Is = "FixedPrice7" : .ExtensionFixedPrice7 = Node5.InnerText
                                                                            Case Is = "FixedPrice8" : .ExtensionFixedPrice8 = Node5.InnerText
                                                                            Case Is = "DealID1" : .ExtensionDealID1 = Node5.InnerText
                                                                            Case Is = "DealID2" : .ExtensionDealID2 = Node5.InnerText
                                                                            Case Is = "DealID3" : .ExtensionDealID3 = Node5.InnerText
                                                                            Case Is = "DealID4" : .ExtensionDealID4 = Node5.InnerText
                                                                            Case Is = "DealID5" : .ExtensionDealID5 = Node5.InnerText
                                                                            Case Is = "DealID6" : .ExtensionDealID6 = Node5.InnerText
                                                                            Case Is = "DealID7" : .ExtensionDealID7 = Node5.InnerText
                                                                            Case Is = "DealID8" : .ExtensionDealID8 = Node5.InnerText
                                                                            Case Is = "Status" : .ExtensionStatus = Node5.InnerText

                                                                        End Select
                                                                    Next Node5
                                                            End Select
                                                        Next Node4
                                                    End With
                                                    deoi.CollDEProductLines.Add(depr)

                                                    '---------------------------------------------------------------------------------
                                                Case Is = "CommentLine"
                                                    For Each Node4 In Node3.ChildNodes
                                                        decl = New DeCommentLines
                                                        decl.CommentText = Node4.InnerText
                                                        deoi.CollDECommentLines.Add(decl)
                                                    Next Node4
                                            End Select
                                        Next Node3

                                        '----------------------------------------------------------------------------------------------
                                    Case Is = "ShowDetail"
                                        deos.ShowDetail = Node2.InnerText
                                        deos.DEOrderInfo = deoi
                                        Dep.CollDEOrders.Add(deos)

                                End Select

                            Next Node2
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQOR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function LoadXmlV1_1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1_1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2, Node3, Node4, Node5 As XmlNode
            Dim detr As New DETransaction           ' Items
            Dim deos As New DeOrders                ' DeOrderHeader, DEOrderInfo  as collections
            Dim deoh As New DeOrderHeader           ' Items and DEAddress as Collection
            Dim dead1 As New DeAddress              ' Single Line Item
            Dim dead2 As New DeAddress              ' Single Line Item
            '
            Dim deop As New DEPayments              ' Multiple Line Item
            Dim deoc As New DECharges               ' Multiple Line Item
            '
            Dim deoi As New DEOrderInfo             ' DEProductLines, DECommentLines  as collections
            Dim depr As DeProductLines              ' Multiple Line Item
            Dim decl As DeCommentLines              ' Multiple Line Item
            Dim tempNodeCounter As Integer = 0
            Dim tempTotalOrderItemsValue As Decimal = 0
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Dep = New DEOrder
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//OrderRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Dep.CollDETrans.Add(Extract_TransactionHeader(Node1))

                        Case Is = "Order"
                            deos = New DeOrders
                            tempTotalOrderItemsValue = 0
                            _customerIDFromXmlV1_1 = String.Empty
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "OrderHeaderInformation"
                                        deoh = New DeOrderHeader

                                        deoh.ProjectedDeliveryDate = Today

                                        With deoh
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "CustomerID"
                                                        If (Node3.InnerText.Trim.Length > 0) Then
                                                            _customerIDFromXmlV1_1 = Node3.InnerText
                                                        Else
                                                            _customerIDFromXmlV1_1 = String.Empty
                                                        End If
                                                    Case Is = "WebOrderNo"
                                                        .WebOrderNumber = Node3.InnerText
                                                    Case Is = "BillToSuffix"
                                                        .BillToSuffix = Node3.InnerText
                                                    Case Is = "ShipToSuffix"
                                                        .ShipToSuffix = Node3.InnerText
                                                    Case Is = "AddressingInformation"
                                                        For Each Node4 In Node3.ChildNodes
                                                            '
                                                            Select Case Node4.Name
                                                                Case Is = "CustomerPO" : .CustomerPO = Node4.InnerText
                                                                Case Is = "EndUserPO" : .EndUserPO = Node4.InnerText
                                                                Case Is = "ShipTo"
                                                                    dead1 = New DeAddress
                                                                    dead1.Category = Node4.Name
                                                                    With dead1
                                                                        For Each Node5 In Node4.SelectSingleNode("Address").ChildNodes
                                                                            Select Case Node5.Name
                                                                                '-----------------------------------------------------------
                                                                                '   Shipping Address info
                                                                                '
                                                                                Case Is = "ShipToAttention" : .ContactName = Node5.InnerText
                                                                                Case Is = "ShipToAddress1" : .Line1 = Node5.InnerText
                                                                                Case Is = "ShipToAddress2" : .Line2 = Node5.InnerText
                                                                                Case Is = "ShipToAddress3" : .Line3 = Node5.InnerText
                                                                                Case Is = "ShipToCity" : .City = Node5.InnerText
                                                                                Case Is = "ShipToProvince" : .Province = Node5.InnerText
                                                                                Case Is = "ShipToPostalCode" : .PostalCode = Node5.InnerText
                                                                                Case Is = "ShipToCountry" : .Country = Node5.InnerText
                                                                            End Select
                                                                        Next Node5
                                                                    End With
                                                                    deoh.CollDEAddress.Add(dead1)
                                                                Case Is = "BillTo"
                                                                    dead2 = New DeAddress
                                                                    dead2.Category = Node4.Name
                                                                    With dead2
                                                                        For Each Node5 In Node4.SelectSingleNode("Address").ChildNodes
                                                                            Select Case Node5.Name
                                                                                '----------------------------------------------------------
                                                                                '   Billing Address info
                                                                                '
                                                                                Case Is = "BillToAttention" : .ContactName = Node5.InnerText
                                                                                Case Is = "BillToAddress1" : .Line1 = Node5.InnerText
                                                                                Case Is = "BillToAddress2" : .Line2 = Node5.InnerText
                                                                                Case Is = "BillToAddress3" : .Line3 = Node5.InnerText
                                                                                Case Is = "BillToCity" : .City = Node5.InnerText
                                                                                Case Is = "BillToProvince" : .Province = Node5.InnerText
                                                                                Case Is = "BillToPostalCode" : .PostalCode = Node5.InnerText
                                                                                Case Is = "BillToCountry" : .Country = Node5.InnerText
                                                                            End Select
                                                                        Next Node5
                                                                    End With
                                                                    deoh.CollDEAddress.Add(dead2)
                                                            End Select
                                                        Next Node4
                                                    Case Is = "ProcessingOptions"
                                                        For Each Node4 In Node3.ChildNodes
                                                            Select Case Node4.Name
                                                                Case Is = "CarrierCode" : .CarrierCode = Node4.InnerText
                                                                Case Is = "CarrierCodeValue" : .CarrierCodeValue = Node4.InnerText
                                                                Case Is = "AutoRelease" : .AutoRelease = Node4.InnerText
                                                                Case Is = "SalesPerson" : .SalesPerson = Node4.InnerText
                                                                Case Is = "OrderDueDate" : .OrderDueDate = Node4.InnerText
                                                                Case Is = "SuspendCode" : .SuspendCode = Node4.InnerText
                                                                Case Is = "BackOrderFlag" : .BackOrderFlag = Node4.InnerText
                                                                Case Is = "ShipmentOptions"
                                                                    For Each Node5 In Node4.ChildNodes
                                                                        Select Case Node5.Name
                                                                            Case Is = "BackOrderFlag" : .BackOrderFlag = Node5.InnerText
                                                                            Case Is = "SplitShipmentFlag" : .SplitShipmentFlag = Node5.InnerText
                                                                            Case Is = "SplitLine" : .SplitLine = Node5.InnerText
                                                                            Case Is = "ShipFromBranches" : .ShipFromBranches = Node5.InnerText
                                                                        End Select
                                                                    Next Node5
                                                                Case Is = "MessageLines" : .MessageLines = Node4.InnerText
                                                            End Select
                                                        Next Node4
                                                    Case Is = "Payment"
                                                        '------------------------------------------------------------------------------
                                                        ' see Utilities.ValidateCardNumber
                                                        '
                                                        deop = New DEPayments
                                                        tempNodeCounter = 0
                                                        For Each Node4 In Node3.ChildNodes
                                                            With deop
                                                                Select Case Node4.Name
                                                                    Case Is = "PaymentType" : If tempNodeCounter = 0 Then .PaymentType = Node4.InnerText
                                                                    Case Is = "PaymentDate" : .PaymentDate = Node4.InnerText
                                                                    Case Is = "Bank" : .Bank = Node4.InnerText
                                                                    Case Is = "Branch" : .Branch = Node4.InnerText
                                                                    Case Is = "Sortcode" : .SortCode = Node4.InnerText
                                                                    Case Is = "Amount" : .Amount = Node4.InnerText
                                                                    Case Is = "AccountDetails", "CardDetails", "CreditCardDetails"
                                                                        For Each Node5 In Node4.ChildNodes
                                                                            Select Case Node5.Name
                                                                                Case Is = "AccountType" : .AccountType = Node5.InnerText
                                                                                Case Is = "AccountNumber" : .AccountNumber = Node5.InnerText
                                                                                Case Is = "AccountName" : .AccountName = Node5.InnerText
                                                                                Case Is = "AccountpostCode" : .AccountpostCode = Node5.InnerText
                                                                                Case Is = "AccountAddress" : .AccountAddress = Node5.InnerText
                                                                                    '
                                                                                Case Is = "CardType" : .CardType = Node5.InnerText
                                                                                Case Is = "CardNumber" : .CardNumber = Node5.InnerText
                                                                                Case Is = "CardHolderName" : .CardName = Node5.InnerText
                                                                                Case Is = "CardHolderPostCode" : .CardpostCode = Node5.InnerText
                                                                                Case Is = "CardHolderAddress" : .CardAddress = Node5.InnerText
                                                                                    '
                                                                                Case Is = "ExpiryDate" : .ExpiryDate = Node5.InnerText
                                                                                Case Is = "StartDate" : .StartDate = Node5.InnerText
                                                                                Case Is = "IssueNumber" : .IssueNumber = Node5.InnerText
                                                                                Case Is = "CV2Number" : .CV2Number = Node5.InnerText
                                                                            End Select
                                                                        Next Node5
                                                                End Select
                                                            End With
                                                            tempNodeCounter = tempNodeCounter + 1
                                                        Next Node4
                                                        deoh.CollDEPayments.Add(deop)

                                                    Case Is = "OrderCharges"
                                                        For Each Node4 In Node3.ChildNodes
                                                            deoc = New DECharges
                                                            deoc.Charge = Node4.InnerText
                                                            deoh.CollDECharges.Add(deoc)
                                                        Next Node4

                                                    Case Is = "DynamicMessage"
                                                        For Each Node4 In Node3.ChildNodes
                                                            .MessageLines &= Node4.InnerText
                                                        Next Node4
                                                    Case Is = "Extension"
                                                        For Each Node4 In Node3.ChildNodes
                                                            Select Case Node4.Name
                                                                Case Is = "Reference1" : .ExtensionReference1 = Node4.InnerText
                                                                Case Is = "Reference2" : .ExtensionReference2 = Node4.InnerText
                                                                Case Is = "Reference3" : .ExtensionReference3 = Node4.InnerText
                                                                Case Is = "Reference4" : .ExtensionReference4 = Node4.InnerText
                                                                Case Is = "FixedPrice1" : .ExtensionFixedPrice1 = Node4.InnerText
                                                                Case Is = "FixedPrice2" : .ExtensionFixedPrice2 = Node4.InnerText
                                                                Case Is = "FixedPrice3" : .ExtensionFixedPrice3 = Node4.InnerText
                                                                Case Is = "FixedPrice4" : .ExtensionFixedPrice4 = Node4.InnerText
                                                                Case Is = "DealID1" : .ExtensionDealID1 = Node4.InnerText
                                                                Case Is = "DealID2" : .ExtensionDealID2 = Node4.InnerText
                                                                Case Is = "DealID3" : .ExtensionDealID3 = Node4.InnerText
                                                                Case Is = "DealID4" : .ExtensionDealID4 = Node4.InnerText
                                                                Case Is = "DealID5" : .ExtensionDealID5 = Node4.InnerText
                                                                Case Is = "DealID6" : .ExtensionDealID6 = Node4.InnerText
                                                                Case Is = "DealID7" : .ExtensionDealID7 = Node4.InnerText
                                                                Case Is = "DealID8" : .ExtensionDealID8 = Node4.InnerText
                                                                Case Is = "Flag1" : .ExtensionFlag1 = Node4.InnerText
                                                                Case Is = "Flag2" : .ExtensionFlag2 = Node4.InnerText
                                                                Case Is = "Flag3" : .ExtensionFlag3 = Node4.InnerText
                                                                Case Is = "Flag4" : .ExtensionFlag4 = Node4.InnerText
                                                                Case Is = "Flag5" : .ExtensionFlag5 = Node4.InnerText
                                                                Case Is = "Flag6" : .ExtensionFlag6 = Node4.InnerText
                                                                Case Is = "Flag7" : .ExtensionFlag7 = Node4.InnerText
                                                                Case Is = "Status" : .ExtensionStatus = Node4.InnerText

                                                            End Select
                                                        Next Node4

                                                End Select
                                            Next Node3
                                        End With

                                        '----------------------------------------------------------------------------------------------
                                    Case Is = "OrderLineInformation"
                                        deoi = New DEOrderInfo
                                        For Each Node3 In Node2.ChildNodes
                                            Select Case Node3.Name
                                                Case Is = "ProductLine"
                                                    depr = New DeProductLines
                                                    With depr
                                                        For Each Node4 In Node3.ChildNodes
                                                            .Category = "Insert"
                                                            Select Case Node4.Name
                                                                Case Is = "SKU" : .SKU = Node4.InnerText
                                                                Case Is = "AlternateSKU" : .AlternateSKU = Node4.InnerText
                                                                Case Is = "Quantity" : .Quantity = Node4.InnerText
                                                                Case Is = "FixedPrice" : .FixedPrice = Node4.InnerText
                                                                Case Is = "CustomerLineNumber" : .CustomerLineNumber = Node4.InnerText
                                                                Case Is = "ReservedInventory"
                                                                    For Each Node5 In Node4.ChildNodes
                                                                        Select Case Node5.Name
                                                                            Case Is = "ReserveCode" : .ReserveCode = Node5.InnerText
                                                                            Case Is = "ReserveSequence" : .ReserveSequence = Node5.InnerText
                                                                        End Select
                                                                    Next
                                                                Case Is = "LineComment" : .LineComment = Node4.InnerText
                                                                Case Is = "Extension"
                                                                    For Each Node5 In Node4.ChildNodes
                                                                        Select Case Node5.Name
                                                                            Case Is = "Reference1" : .ExtensionReference1 = Node5.InnerText
                                                                            Case Is = "Reference2" : .ExtensionReference2 = Node5.InnerText
                                                                            Case Is = "Reference3" : .ExtensionReference3 = Node5.InnerText
                                                                            Case Is = "Reference4" : .ExtensionReference4 = Node5.InnerText
                                                                            Case Is = "Reference5" : .ExtensionReference5 = Node5.InnerText
                                                                            Case Is = "Reference6" : .ExtensionReference6 = Node5.InnerText
                                                                            Case Is = "Reference7" : .ExtensionReference7 = Node5.InnerText
                                                                            Case Is = "Reference8" : .ExtensionReference8 = Node5.InnerText
                                                                            Case Is = "Flag1" : .ExtensionFlag1 = Node5.InnerText
                                                                            Case Is = "Flag2" : .ExtensionFlag2 = Node5.InnerText
                                                                            Case Is = "Flag3" : .ExtensionFlag3 = Node5.InnerText
                                                                            Case Is = "Flag4" : .ExtensionFlag4 = Node5.InnerText
                                                                            Case Is = "Flag5" : .ExtensionFlag5 = Node5.InnerText
                                                                            Case Is = "Flag6" : .ExtensionFlag6 = Node5.InnerText
                                                                            Case Is = "Flag7" : .ExtensionFlag7 = Node5.InnerText
                                                                            Case Is = "Flag8" : .ExtensionFlag8 = Node5.InnerText
                                                                            Case Is = "Flag9" : .ExtensionFlag9 = Node5.InnerText
                                                                            Case Is = "Flag0" : .ExtensionFlag0 = Node5.InnerText
                                                                            Case Is = "Field1" : .ExtensionField1 = Node5.InnerText
                                                                            Case Is = "Field2" : .ExtensionField2 = Node5.InnerText
                                                                            Case Is = "Field3" : .ExtensionField3 = Node5.InnerText
                                                                            Case Is = "Field4" : .ExtensionField4 = Node5.InnerText
                                                                            Case Is = "FixedPrice1" : .ExtensionFixedPrice1 = Node5.InnerText
                                                                            Case Is = "FixedPrice2" : .ExtensionFixedPrice2 = Node5.InnerText
                                                                            Case Is = "FixedPrice3" : .ExtensionFixedPrice3 = Node5.InnerText
                                                                            Case Is = "FixedPrice4" : .ExtensionFixedPrice4 = Node5.InnerText
                                                                            Case Is = "FixedPrice5" : .ExtensionFixedPrice5 = Node5.InnerText
                                                                            Case Is = "FixedPrice6" : .ExtensionFixedPrice6 = Node5.InnerText
                                                                            Case Is = "FixedPrice7" : .ExtensionFixedPrice7 = Node5.InnerText
                                                                            Case Is = "FixedPrice8" : .ExtensionFixedPrice8 = Node5.InnerText
                                                                            Case Is = "DealID1" : .ExtensionDealID1 = Node5.InnerText
                                                                            Case Is = "DealID2" : .ExtensionDealID2 = Node5.InnerText
                                                                            Case Is = "DealID3" : .ExtensionDealID3 = Node5.InnerText
                                                                            Case Is = "DealID4" : .ExtensionDealID4 = Node5.InnerText
                                                                            Case Is = "DealID5" : .ExtensionDealID5 = Node5.InnerText
                                                                            Case Is = "DealID6" : .ExtensionDealID6 = Node5.InnerText
                                                                            Case Is = "DealID7" : .ExtensionDealID7 = Node5.InnerText
                                                                            Case Is = "DealID8" : .ExtensionDealID8 = Node5.InnerText
                                                                            Case Is = "Status" : .ExtensionStatus = Node5.InnerText

                                                                        End Select
                                                                    Next Node5
                                                            End Select
                                                        Next Node4
                                                        If (IsNumeric(.FixedPrice)) And (IsNumeric(.Quantity)) Then
                                                            tempTotalOrderItemsValue = tempTotalOrderItemsValue + (CDec(.FixedPrice) * CDec(.Quantity))
                                                        End If
                                                    End With
                                                    deoi.CollDEProductLines.Add(depr)

                                                    '---------------------------------------------------------------------------------
                                                Case Is = "CommentLine"
                                                    For Each Node4 In Node3.ChildNodes
                                                        decl = New DeCommentLines
                                                        decl.CommentText = Node4.InnerText
                                                        deoi.CollDECommentLines.Add(decl)
                                                    Next Node4
                                            End Select
                                        Next Node3

                                        '----------------------------------------------------------------------------------------------
                                    Case Is = "ShowDetail"
                                        deos.ShowDetail = Node2.InnerText
                                        deoh.TotalOrderItemsValue = tempTotalOrderItemsValue.ToString
                                        If (IsNumeric(deoh.CarrierCodeValue)) Then
                                            deoh.TotalOrderValue = tempTotalOrderItemsValue * CDec(deoh.CarrierCodeValue)
                                            deoh.TotalValueCharged = tempTotalOrderItemsValue * CDec(deoh.CarrierCodeValue)
                                        End If
                                        deos.DEOrderHeader = deoh
                                        '.TotalOrderValue
                                        '.TotalValueCharged

                                        deos.DEOrderInfo = deoi
                                        Dep.CollDEOrders.Add(deos)

                                        'reset all temp variables
                                        tempTotalOrderItemsValue = 0
                                End Select
                            Next Node2
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQOR-04"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function RepriceBlankPrices() As ErrorObj
            Dim err As New ErrorObj
            Dim def As New SupplynetDefaults(ConfigurationManager.AppSettings("DefaultBusinessUnit"), Settings.Company)
            Dim pricing As New TalentPricing()
            Dim depnarequestNew As New DEPNARequest
            Dim pa As New DEAlerts
            Dim dt As Data.DataTable
            '------------------------
            ' Set up Pricing defaults
            '------------------------
            Dim pricingSettings As New DESettings
            Dim pr As New Profile
            pr.BusinessUnit = Settings.BusinessUnit
            pr.Company = Settings.Company
            Dim pricingDefaults As New Defaults

            Try
                err = pr.CreateProfile(Me.LoginId, Me.Password, Me.Company, "PNARequest")
                If Not err.HasError Then
                    With pricingDefaults
                        .WebServiceName = "PNARequest"
                        .Company = Company
                        ' Override Defaults with profile settings
                        .DestinationDatabase = pr.WebServiceDestinationDatabase()
                        .BusinessUnit = pr.BusinessUnit
                        .Xsd = pr.Xsd
                        .CacheTimeMinutes = pr.CacheTimeMinutes()
                        .WriteLog = pr.WriteLog
                        .StoreXml = pr.StoreXml
                        .DatabaseType1 = pr.DatabaseType1
                        err = .GetDefaults()
                    End With
                    With pricingSettings
                        .AccountNo1 = pricingDefaults.AccountNo1                           ' account number part 1
                        .AccountNo2 = pricingDefaults.AccountNo2                           ' account number part 2
                        .AccountNo3 = pricingDefaults.AccountNo3                           ' account number part 3
                        .AccountNo4 = pricingDefaults.AccountNo4                           ' account number part 4
                        .AccountNo5 = pricingDefaults.AccountNo5                           ' account number part 5
                        .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("SYSTEM21").ToString
                        .Cacheing = pricingDefaults.Cacheing()                             ' Cacheing?
                        .CacheTimeMinutes = pr.CacheTimeMinutes()                   ' Cache Time
                        .Company = Company                                          ' Company
                        .DatabaseType1 = pricingDefaults.DatabaseType1()                   ' Database type
                        .DestinationDatabase = pricingDefaults.DestinationDatabase()       ' Destination Database
                        .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                        .WebServiceName = "MultiAvailabilityRequest"                            ' service instance name
                        .RetryFailures = pricingDefaults.RetryFailures                     ' Retry Failures on/off
                        .RetryAttempts = pricingDefaults.RetryAttempts                     ' Retry attempts
                        .RetryWaitTime = pricingDefaults.RetryWaitTime                     ' Retry wait time
                        .RetryErrorNumbers = pricingDefaults.RetryErrorNumbers             ' Retry error numbers
                        ' Set Stored Procedure Group from DB, or if not found, from WebConfig
                        If Not String.IsNullOrEmpty(pr.WebServiceStoredProcedureGroup) Then
                            .StoredProcedureGroup = pr.WebServiceStoredProcedureGroup
                        Else
                            .StoredProcedureGroup = ConfigurationManager.AppSettings("DefaultStoredProcedureGroup")
                        End If
                    End With
                    '--------------------
                    ' Loop through orders
                    '--------------------
                    For Each orderObj As DeOrders In Dep.CollDEOrders
                        For Each lineItem As DeProductLines In orderObj.DEOrderInfo.CollDEProductLines
                            '------------------------------------------------------------------
                            ' Loop through items and if any are blank then call pricing routine
                            '------------------------------------------------------------------
                            If lineItem.FixedPrice = String.Empty AndAlso Not lineItem.LineError Then
                                depnarequestNew = New DEPNARequest
                                pa = New DEAlerts
                                pa.LineNo = 0
                                pa.Quantity = lineItem.Quantity
                                pa.ProductCode = lineItem.SKU
                                pa.AvailabilQty = 0
                                pa.BranchID = lineItem.ShipFromWarehouse
                                pa.ManufacturerPartNumber = String.Empty
                                pa.OnOrder = String.Empty
                                pa.Description = String.Empty

                                depnarequestNew.CollDEAlerts.Add(pa)
                                pricing = New TalentPricing
                                Try
                                    With pricing
                                        ' .Dep = DePNA
                                        depnarequestNew.PriceUrl = def.GetDefault("PRICE_URL")
                                        .Depnarequest = depnarequestNew
                                        .Settings = pricingSettings
                                        .ResultDataSet = New Data.DataSet
                                        err = .PnaRequest
                                        If Not err.HasError Then
                                            dt = .ResultDataSet.Tables(0)
                                            lineItem.FixedPrice = dt.Rows(0)("Price").ToString.Trim
                                        End If
                                    End With
                                Catch ex As Exception
                                    '---------------------------------------------------
                                    ' Either pricing URL failed or item not found by URL
                                    ' Mark the line as having failed initial validation. 
                                    ' This is picked up by DB order and the order won't
                                    ' by placed.
                                    '---------------------------------------------------
                                    lineItem.LineError = True
                                    lineItem.LineErrorMessage = "Pricing Error - Unable to price line"
                                End Try
                            End If
                        Next lineItem

                    Next orderObj
                End If

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = "Unexpected Pricing Error: " & ex.Message
                    .ErrorNumber = "TTPRQOR-02"
                    .HasError = True
                End With
            End Try

            Return err
        End Function

        Private Function CheckForAltProducts() As ErrorObj
            Dim err As New ErrorObj

            Dim product As New TalentProduct()
            Dim productCollection As New Collection()
            Dim dt As Data.DataTable
            '------------------------
            ' Set up Product defaults
            '------------------------
            Dim productSettings As New DESettings
            Dim pr As New Profile
            pr.BusinessUnit = Settings.BusinessUnit
            pr.Company = Settings.Company
            Dim productDefaults As New Defaults

            Try
                err = pr.CreateProfile(Me.LoginId, Me.Password, Me.Company, "RetrieveAlternativeProductsRequest")
                If Not err.HasError Then
                    With productDefaults
                        .WebServiceName = "RetrieveAlternativeProductsRequest"
                        .Company = Company
                        ' Override Defaults with profile settings
                        .DestinationDatabase = pr.WebServiceDestinationDatabase()
                        .BusinessUnit = pr.BusinessUnit
                        .Xsd = pr.Xsd
                        .CacheTimeMinutes = pr.CacheTimeMinutes()
                        .WriteLog = pr.WriteLog
                        .StoreXml = pr.StoreXml
                        .DatabaseType1 = pr.DatabaseType1
                        err = .GetDefaults()
                    End With
                    With productSettings
                        .AccountNo1 = productDefaults.AccountNo1                           ' account number part 1
                        .AccountNo2 = productDefaults.AccountNo2                           ' account number part 2
                        .AccountNo3 = productDefaults.AccountNo3                           ' account number part 3
                        .AccountNo4 = productDefaults.AccountNo4                           ' account number part 4
                        .AccountNo5 = productDefaults.AccountNo5                           ' account number part 5
                        .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("SYSTEM21").ToString
                        .Cacheing = productDefaults.Cacheing()                             ' Cacheing?
                        .CacheTimeMinutes = pr.CacheTimeMinutes()                   ' Cache Time
                        .Company = Company                                          ' Company
                        .DatabaseType1 = productDefaults.DatabaseType1()                   ' Database type
                        .DestinationDatabase = productDefaults.DestinationDatabase()       ' Destination Database
                        .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                        .WebServiceName = "MultiAvailabilityRequest"                            ' service instance name
                        .RetryFailures = productDefaults.RetryFailures                     ' Retry Failures on/off
                        .RetryAttempts = productDefaults.RetryAttempts                     ' Retry attempts
                        .RetryWaitTime = productDefaults.RetryWaitTime                     ' Retry wait time
                        .RetryErrorNumbers = productDefaults.RetryErrorNumbers             ' Retry error numbers
                        ' Set Stored Procedure Group from DB, or if not found, from WebConfig
                        If Not String.IsNullOrEmpty(pr.WebServiceStoredProcedureGroup) Then
                            .StoredProcedureGroup = pr.WebServiceStoredProcedureGroup
                        Else
                            .StoredProcedureGroup = ConfigurationManager.AppSettings("DefaultStoredProcedureGroup")
                        End If
                    End With
                    '--------------------
                    ' Loop through orders
                    '--------------------
                    For Each orderObj As DeOrders In Dep.CollDEOrders
                        For Each lineItem As DeProductLines In orderObj.DEOrderInfo.CollDEProductLines
                            '-----------------------------------------------
                            ' Loop through items and if have an Alt SKU then  
                            ' need to try and obtain the real SKU
                            '-----------------------------------------------
                            If lineItem.AlternateSKU <> String.Empty Then
                                productCollection = New Collection
                                productCollection.Add(lineItem.AlternateSKU)
                                product = New TalentProduct
                                Try
                                    With product
                                        .ProductCollection = productCollection
                                        .Settings = productSettings
                                        err = .RetrieveAlternativeProducts
                                        If Not err.HasError Then
                                            dt = .ResultDataSet.Tables("ALTPRODUCTRESULTS")
                                            lineItem.SKU = dt.Rows(0)("ProductCode").ToString
                                        End If
                                    End With
                                Catch ex As Exception
                                    '------------------------------------------------
                                    ' Either Alt SKU routine failed or item not found
                                    '------------------------------------------------
                                    lineItem.LineError = True
                                    lineItem.LineErrorMessage = "Alt SKU Error - Unable to find SKU"
                                    lineItem.SKU = lineItem.AlternateSKU
                                End Try
                            End If
                        Next lineItem

                    Next orderObj
                End If
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = "Unexpected Alt Part Error: " & ex.Message
                    .ErrorNumber = "TTPRQOR-03"
                    .HasError = True
                End With
            End Try

            Return err
        End Function
    End Class

End Namespace