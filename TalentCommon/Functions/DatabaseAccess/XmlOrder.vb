Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Net
Imports System.Xml
Imports System.Text

'Proxy class for BEKO SAP WS
Imports Talent.Common.BEKOSAPSalesOrder

'   Error Code  -   TACXMLO- (TAC -Talent Common, XMLO - class name XmlOrder)
'   Next Error Code Starting is TACXMLO-19

Public Class XmlOrder
    Inherits XmlAccess

    Private _dep As DEOrder
    Public Property Dep() As DEOrder
        Get
            Return _dep
        End Get
        Set(ByVal value As DEOrder)
            _dep = value
        End Set
    End Property

    Private _xmlOrderDoc As New XmlDocument
    Public Property XmlOrderDocument() As XmlDocument
        Get
            Return _xmlOrderDoc
        End Get
        Set(ByVal value As XmlDocument)
            _xmlOrderDoc = value
        End Set
    End Property

    Public Function WriteToXML() As ErrorObj
        Dim errObj As New ErrorObj

        If Not Dep Is Nothing Then

            'Check is it BEKO SAP WS order
            If (Me.Settings.XmlSettings.DestinationType.ToUpper().Equals("SAP")) Then
                If (Me.Settings.XmlSettings.XmlVersion.Trim().Equals("1.0")) Then
                    errObj = BEKOSAPWSOrder()
                End If
            Else
                errObj = DefaultXMLOrder()
                If Not errObj.HasError Then
                    ProcessCreatedXML()
                End If
            End If

        Else
            With errObj
                .ErrorMessage = "DE Order object contains no Data"
                .ErrorNumber = "TACXMLO-01"
                .HasError = True
            End With
        End If

        Return errObj
    End Function

    ''' <summary>
    ''' Pass the order details to BEKO SAP web service and get back the response
    ''' Proxy class is BEKOSAPSalesOrder
    ''' Web services details are provided by tbl_destination_xml table
    ''' </summary>
    ''' <returns>Error object</returns>
    Private Function BEKOSAPWSOrder() As Talent.Common.ErrorObj
        Dim errObj As New ErrorObj
        Dim purchaseNoC As New StringBuilder

        Const DOCTYPE As String = "EBOR"
        Const SALESORG As String = "1111"
        Const DIVISION As String = "01"
        Const DELIVERYBLOCK As String = ""
        Const SHIPCOND As String = "P1"
        Const PRICELIST As String = "P1"
        Const PARTNROLE_0 As String = "AG"
        Const PARTNROLE_1 As String = "WE"
        Const PARTNUM_HOMEDELIVERY As String = "0000001250"
        Const PARTDLV As String = "C"
        Const PLANT As String = "1111"
        Const ITEMCATEG As String = "TAN"
        Const BEHAVEWHENERROR As String = "P" 'BAPIFLAG = S Success, E Error, W Warning, I Info, A Abort
        Const ORDERSUCCESS As String = "S"
        Const REQUIREDDATEFORMAT As String = "yyyy-MM-dd"
        Const LANGUAGE As String = "EN"
        Const PURCHASE_NO_C_SEPARATOR As String = ":"
        Const ADDRESS_LINK As String = "ADDLINKWE"
        Const UNITED_KINGDOM_STRING As String = "United Kingdom"
        Const GB_STRING As String = "GB"
        Const X_VALUE As String = "X"

        For Each deo As DeOrders In Dep.CollDEOrders
            Dim salesOrderCreate As SalesOrderCreateFromDat2
            Dim orderHeader As BEKOSAPSalesOrder.Bapisdhd1
            Dim orderHeaderX As BEKOSAPSalesOrder.Bapisdhd1x
            Dim BapiPartners(1) As BEKOSAPSalesOrder.Bapiparnr
            Dim BapiOrderItems(0) As BEKOSAPSalesOrder.Bapisditm
            Dim BapiOrderItemsX(0) As BEKOSAPSalesOrder.Bapisditmx
            Dim BapiItemSchedules(0) As BEKOSAPSalesOrder.Bapischdl
            Dim BapiItemSchedulesX(0) As BEKOSAPSalesOrder.Bapischdlx
            Dim BapiOrderConditionsIn(0) As BEKOSAPSalesOrder.Bapicond
            Dim BapiOrderConditionsInX(0) As BEKOSAPSalesOrder.Bapicondx
            Dim bapisdtextTextLine As String = ""
            Dim BapiAddr1(0) As BEKOSAPSalesOrder.Bapiaddr1
            orderHeader = New BEKOSAPSalesOrder.Bapisdhd1
            orderHeaderX = New BEKOSAPSalesOrder.Bapisdhd1x

            Try
                Dim address1 As New DeAddress
                address1 = deo.DEOrderHeader.CollDEAddress(1)

                'Assign Order header
                If Not errObj.HasError Then
                    Try
                        With orderHeader
                            .DocType = DOCTYPE
                            .SalesOrg = SALESORG
                            .DistrChan = Dep.DistChan
                            .Division = DIVISION
                            .ShipCond = SHIPCOND
                            .PoMethod = Dep.PoMethod
                            .PriceList = PRICELIST

                            ' for home del, set name at order header level to the 'ordering customer', not delivery customer.
                            If deo.DEOrderHeader.HomeDelivery Then
                                .Name = deo.DEOrderHeader.OrderCustomerName
                            Else
                                .Name = address1.ContactName
                            End If

                            .Ref1 = deo.DEOrderHeader.WebOrderNumber
                            If deo.DEOrderHeader.HomeDelivery Then
                                .Telephone = address1.Line1
                                If .Telephone.Length = 0 AndAlso address1.Line2.Length > 0 Then
                                    .Telephone = address1.Line2
                                End If
                            Else
                                .Telephone = address1.PhoneNumber
                            End If
                            .DlvBlock = DELIVERYBLOCK
                            'the date that customer wants his goods to be delivered in yyyy-dd-MM format
                            .ReqDateH = Format(deo.DEOrderHeader.ProjectedDeliveryDate, REQUIREDDATEFORMAT)

                            'PurchNoC = TEMP_ORDER_ID (prefixed differently for B2B home delivery, B2B trade delivery and B2C) : post code (first 4 chars, no spaces) : customer purchase no : BASKET_PAYMENT_ID (for B2C reconcilation with vanguard)
                            purchaseNoC.Append(deo.DEOrderHeader.WebOrderNumber)
                            If deo.DEOrderHeader.HomeDelivery AndAlso Dep.SendCustomerPostCodeToSAP Then
                                Dim amendedPostCode As String = address1.PostalCode.Replace(" ", String.Empty)
                                If amendedPostCode.Length > 3 Then
                                    amendedPostCode = amendedPostCode.Substring(0, 4)
                                End If
                                purchaseNoC.Append(PURCHASE_NO_C_SEPARATOR).Append(amendedPostCode)
                            End If
                            If (deo.DEOrderHeader.CustomerPO.Trim().Length > 0) Then
                                purchaseNoC.Append(PURCHASE_NO_C_SEPARATOR).Append(deo.DEOrderHeader.CustomerPO)
                            End If
                            If deo.DEOrderHeader.BasketPaymentID > 0 Then
                                purchaseNoC.Append(PURCHASE_NO_C_SEPARATOR).Append(deo.DEOrderHeader.BasketPaymentID)
                            End If
                            .PurchNoC = purchaseNoC.ToString()
                        End With

                        With orderHeaderX
                            .DocType = X_VALUE
                            .SalesOrg = X_VALUE
                            .DistrChan = X_VALUE
                            .Division = X_VALUE
                            .ReqDateH = X_VALUE
                            .PoMethod = X_VALUE
                            .Ref1 = X_VALUE
                            .Name = X_VALUE
                            .Telephone = X_VALUE
                            .PurchNoC = X_VALUE
                            .ShipCond = X_VALUE
                            .PriceList = X_VALUE
                        End With

                        'Assign Partner details
                        BapiPartners(0) = New BEKOSAPSalesOrder.Bapiparnr
                        BapiPartners(0).PartnRole = PARTNROLE_0
                        BapiPartners(0).PartnNumb = Me.Settings.AccountNo1.PadLeft(10, CChar("0"))
                        BapiPartners(1) = New BEKOSAPSalesOrder.Bapiparnr
                        BapiPartners(1).PartnRole = PARTNROLE_1

                        'Assign address details
                        BapiAddr1(0) = New BEKOSAPSalesOrder.Bapiaddr1
                        If deo.DEOrderHeader.HomeDelivery Then
                            BapiPartners(1).AddrLink = ADDRESS_LINK
                            BapiPartners(1).PartnNumb = PARTNUM_HOMEDELIVERY
                            BapiAddr1(0).AddrNo = ADDRESS_LINK
                            BapiAddr1(0).Name = address1.ContactName
                            BapiAddr1(0).Street = address1.Line3
                            BapiAddr1(0).District = address1.City
                            BapiAddr1(0).City = address1.Province
                            BapiAddr1(0).PostlCod1 = address1.PostalCode
                            If address1.Country = UNITED_KINGDOM_STRING Then
                                BapiAddr1(0).Country = GB_STRING
                            End If
                            BapiAddr1(0).Tel1Numbr = address1.Line1
                            BapiAddr1(0).FaxNumber = address1.Line2
                            If Dep.SendCustomerEmailAddressToSAP Then
                                BapiAddr1(0).EMail = address1.Email
                            End If
                        Else
                            If String.IsNullOrEmpty(deo.DEOrderHeader.ShippingCode) Then
                                BapiPartners(1).PartnNumb = Me.Settings.AccountNo1.PadLeft(10, CChar("0"))
                            Else
                                BapiPartners(1).PartnNumb = deo.DEOrderHeader.ShippingCode.PadLeft(10, CChar("0"))
                            End If
                        End If

                        'Assign order details
                        If Not errObj.HasError Then
                            Dim numberOfItems As Integer = deo.DEOrderInfo.CollDEProductLines.Count
                            ReDim Preserve BapiOrderItems(numberOfItems - 1)
                            ReDim Preserve BapiOrderItemsX(numberOfItems - 1)
                            ReDim Preserve BapiItemSchedules(numberOfItems - 1)
                            ReDim Preserve BapiItemSchedulesX(numberOfItems - 1)
                            ReDim Preserve BapiOrderConditionsIn(numberOfItems - 1)
                            ReDim Preserve BapiOrderConditionsInX(numberOfItems - 1)

                            '-1 because so the item counter Bapi array starts with 0 and always equals to (numberOfItems + 1)
                            Dim itemCounter As Integer = -1
                            'Add order detail elememts and info
                            For Each deopl As DeProductLines In deo.DEOrderInfo.CollDEProductLines
                                Try
                                    itemCounter = itemCounter + 1
                                    'increase this by 10 for next item
                                    Dim itemNumber As String = ((itemCounter + 1) * 10).ToString()
                                    Dim material As String = String.Empty
                                    If Dep.EnableAlternativeSKU Then
                                        material = deopl.AlternateSKU
                                        'If AlternateSKU is blank then use SKU
                                        If (material.Trim().Length <= 0) Then
                                            material = deopl.SKU
                                        End If
                                    Else
                                        material = deopl.SKU
                                    End If
                                    If IsNumeric(material) Then
                                        material = material.PadLeft(18, CChar("0"))
                                    End If
                                    'Populate Bapi
                                    BapiOrderItems(itemCounter) = New BEKOSAPSalesOrder.Bapisditm
                                    BapiOrderItems(itemCounter).PartDlv = PARTDLV
                                    BapiOrderItems(itemCounter).Plant = PLANT
                                    BapiOrderItems(itemCounter).ItemCateg = ITEMCATEG
                                    BapiOrderItems(itemCounter).ItmNumber = itemNumber
                                    BapiOrderItems(itemCounter).Material = material

                                    BapiOrderItemsX(itemCounter) = New BEKOSAPSalesOrder.Bapisditmx
                                    BapiOrderItemsX(itemCounter).PartDlv = X_VALUE
                                    BapiOrderItemsX(itemCounter).Plant = X_VALUE
                                    BapiOrderItemsX(itemCounter).ItemCateg = X_VALUE
                                    BapiOrderItemsX(itemCounter).ItmNumber = X_VALUE
                                    BapiOrderItemsX(itemCounter).Material = X_VALUE
                                    BapiOrderItemsX(itemCounter).TargetQty = X_VALUE

                                    BapiItemSchedules(itemCounter) = New BEKOSAPSalesOrder.Bapischdl
                                    BapiItemSchedules(itemCounter).ItmNumber = itemNumber
                                    BapiItemSchedules(itemCounter).ReqQty = CDec(deopl.Quantity)
                                    If deopl.SetlineDelDate Then
                                        BapiItemSchedules(itemCounter).SchedLine = "0001"
                                        BapiItemSchedules(itemCounter).ReqDate = Format(deopl.lineDelDate, REQUIREDDATEFORMAT)
                                    End If

                                    BapiItemSchedulesX(itemCounter) = New BEKOSAPSalesOrder.Bapischdlx
                                    BapiItemSchedulesX(itemCounter).ItmNumber = itemNumber
                                    BapiItemSchedulesX(itemCounter).SchedLine = "0001"
                                    BapiItemSchedulesX(itemCounter).ReqDate = X_VALUE
                                    BapiItemSchedulesX(itemCounter).ReqQty = X_VALUE

                                    'Populate Order Conditions In (for promotions)
                                    If deo.DEOrderHeader.PromotionValue > 0 Then
                                        BapiOrderConditionsIn(itemCounter) = New BEKOSAPSalesOrder.Bapicond
                                        BapiOrderConditionsIn(itemCounter).ItmNumber = itemNumber
                                        BapiOrderConditionsIn(itemCounter).CondStNo = "000"
                                        BapiOrderConditionsIn(itemCounter).Condcount = "00"
                                        BapiOrderConditionsIn(itemCounter).Condtype = "ZP06"
                                        BapiOrderConditionsIn(itemCounter).Condvalue = deo.DEOrderHeader.PromotionValue.ToString("F")
                                        BapiOrderConditionsIn(itemCounter).Currency = "GBP"

                                        BapiOrderConditionsInX(itemCounter) = New BEKOSAPSalesOrder.Bapicondx
                                        BapiOrderConditionsInX(itemCounter).ItmNumber = itemNumber
                                        BapiOrderConditionsInX(itemCounter).CondStNo = "000"
                                        BapiOrderConditionsInX(itemCounter).CondCount = "00"
                                        BapiOrderConditionsInX(itemCounter).CondType = "ZP06"
                                        BapiOrderConditionsInX(itemCounter).CondValue = X_VALUE
                                        BapiOrderConditionsInX(itemCounter).Currency = X_VALUE
                                    End If
                                Catch ex As Exception
                                    With errObj
                                        .ErrorMessage = "DE Order Error - Failed to add Order Detail (products) Lines to BEKO SAP WS"
                                        .ErrorNumber = "TACXMLO-14"
                                        .HasError = True
                                    End With
                                End Try
                            Next
                            'Assign comment lines
                            If Not errObj.HasError Then
                                Try
                                    'Add any comment lines
                                    For Each deocl As DeCommentLines In deo.DEOrderInfo.CollDECommentLines
                                        bapisdtextTextLine = bapisdtextTextLine & deocl.CommentText
                                    Next
                                    If deo.DEOrderHeader.HomeDelivery Then
                                        ' if home delivery then also copy in text (Full address, Name, Phone number, services selected) 
                                        bapisdtextTextLine = bapisdtextTextLine & BuildAdditionalHomeDelDetails(address1, deo)
                                    End If
                                Catch ex As Exception
                                    With errObj
                                        .ErrorMessage = "DE Order Error - Failed to add Order Comment Lines to BEKO SAP WS"
                                        .ErrorNumber = "TACXMLO-15"
                                        .HasError = True
                                    End With
                                End Try
                            End If 'Assign comment lines end if
                        End If 'Assign order details end if
                    Catch ex As Exception
                        With errObj
                            .ErrorMessage = "DE Order Error - Failed to add order header details to BEKO SAP WS"
                            .ErrorNumber = "TACXMLO-11"
                            .HasError = True
                        End With
                    End Try
                End If
            Catch ex As Exception
                With errObj
                    .ErrorMessage = "DE Order Error - Failed to create BEKO SAP WS Order"
                    .ErrorNumber = "TACXMLO-09"
                    .HasError = True
                End With
            End Try

            'Assign the order package to salesorder main object 
            'This is the object passed to the web service
            salesOrderCreate = New BEKOSAPSalesOrder.SalesOrderCreateFromDat2
            If Not errObj.HasError Then
                Try
                    With salesOrderCreate
                        .BehaveWhenError = BEHAVEWHENERROR
                        .OrderHeaderIn = orderHeader
                        .OrderHeaderInx = orderHeaderX
                        .OrderItemsIn = BapiOrderItems
                        .OrderItemsInx = BapiOrderItemsX
                        .OrderSchedulesIn = BapiItemSchedules
                        .OrderSchedulesInx = BapiItemSchedulesX
                        .OrderConditionsIn = BapiOrderConditionsIn
                        .OrderConditionsInx = BapiOrderConditionsInX
                        .OrderPartners = BapiPartners
                        .OrderText = GetBapisdtext(bapisdtextTextLine)
                        .PartnerAddresses = BapiAddr1
                    End With
                Catch ex As Exception
                    With errObj
                        .ErrorMessage = "DE Order Error - Failed to create Main order object for BEKO SAP WS Order"
                        .ErrorNumber = "TACXMLO-16"
                        .HasError = True
                    End With
                End Try
            End If

            'Process order by calling web service if no error
            If Not errObj.HasError Then
                Try
                    Dim BEKOSAPSalesOrderWS As New BEKOSAPSalesOrder.ZSD_2032
                    With BEKOSAPSalesOrderWS
                        .Url = Me.Settings.XmlSettings.PostXmlUrl
                        .Credentials = New NetworkCredential(Me.Settings.XmlSettings.UserName, _
                                                                Me.Settings.XmlSettings.Password, _
                                                                    Me.Settings.XmlSettings.DomainName)
                        .SoapVersion = Web.Services.Protocols.SoapProtocolVersion.Default
                    End With
                    'Assigning a cookie without any values
                    Dim cookieBEKOSAPWS As New CookieContainer
                    BEKOSAPSalesOrderWS.CookieContainer = cookieBEKOSAPWS

                    '----------------------------------------------------------------------
                    '   Serialize the salesOrderCreate object in order to write this to log
                    '
                    Dim sText As String = String.Empty
                    Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(salesOrderCreate.GetType)
                    Dim sw As New System.IO.StringWriter
                    mySerializer.Serialize(sw, salesOrderCreate)
                    Dim txtRes As String = sw.ToString
                    Talent.Common.Utilities.TalentCommonLog("BEKOSAPWSOrder_Request", deo.DEOrderHeader.WebOrderNumber & "/" & deo.DEOrderHeader.CustomerPO, sw.ToString)

                    Dim BEKOSAPSalesOrderWSResponse As BEKOSAPSalesOrder.SalesOrderCreateFromDat2Response
                    BEKOSAPSalesOrderWSResponse = BEKOSAPSalesOrderWS.SalesOrderCreateFromDat2(salesOrderCreate)
                    'Getting back the cookie with values
                    cookieBEKOSAPWS = BEKOSAPSalesOrderWS.CookieContainer
                    Dim responseOutput(2) As String
                    'Salesdocument - SAP Order Number (If Document successfully Created You can get the order number from this field)
                    responseOutput(0) = BEKOSAPSalesOrderWSResponse.Salesdocument
                    responseOutput(1) = ORDERSUCCESS

                    Talent.Common.Utilities.TalentCommonLog("BEKOSAPWSOrder_Response1", deo.DEOrderHeader.WebOrderNumber & "/" & deo.DEOrderHeader.CustomerPO, responseOutput(0))
                    Talent.Common.Utilities.TalentCommonLog("BEKOSAPWSOrder_Response2", deo.DEOrderHeader.WebOrderNumber & "/" & deo.DEOrderHeader.CustomerPO, responseOutput(1))

                    Dim BapiReturnValues() As BEKOSAPSalesOrder.Bapiret2 = BEKOSAPSalesOrderWSResponse.Return

                    If BapiReturnValues.Length > 0 Then
                        For errorCounter As Integer = 0 To BapiReturnValues.Length - 1
                            If (BapiReturnValues(errorCounter).Type.ToUpper() <> ORDERSUCCESS) Then
                                responseOutput(1) = BapiReturnValues(errorCounter).Type.ToUpper()
                                responseOutput(2) &= BapiReturnValues(errorCounter).Message & " "
                            End If
                        Next
                    End If
                    If responseOutput(1) = ORDERSUCCESS Then
                        Dim commitOrder As New BEKOSAPSalesOrder.BapiServiceTransactionCommit
                        commitOrder.WAIT = "X"
                        'Assigning the cookie to make sure all calls to WS in same session
                        BEKOSAPSalesOrderWS.CookieContainer = cookieBEKOSAPWS
                        BEKOSAPSalesOrderWS.BapiServiceTransactionCommit(commitOrder)
                        ' Build Response Table
                        If ResultDataSet Is Nothing Then
                            ResultDataSet = New DataSet
                        End If
                        Dim dtHeader As New Data.DataTable("Header")
                        dtHeader.Columns.Add("OrderNumber")
                        Dim dr As DataRow
                        dr = dtHeader.NewRow
                        dr("OrderNumber") = responseOutput(0)
                        dtHeader.Rows.Add(dr)

                        ResultDataSet.Tables.Add(dtHeader)

                    Else
                        With errObj
                            .ErrorMessage = "DE Order Error - Failure response from BEKO SAP web service and " & _
                                            "response from web service is : " & responseOutput(1) & " : " & responseOutput(2)
                            .ErrorNumber = "TACXMLO-18"
                            .HasError = True
                        End With
                    End If
                Catch ex As Exception
                    With errObj
                        .ErrorMessage = "DE Order Error - Failed while calling BEKO SAP web service" & " : " & ex.Message
                        'Todo: Check error number format with Ben
                        .ErrorNumber = "TACXMLO-17"
                        .HasError = True
                    End With
                End Try
            End If
            'If error occured exit for loop and return the errObj
            If errObj.HasError Then
                Exit For
            End If
        Next ' Orders loop ends

        Return errObj

    End Function

    Private Function BuildAdditionalHomeDelDetails(ByVal address As DeAddress, ByVal deo As DeOrders) As String
        Dim addDets As StringBuilder = New StringBuilder
        With addDets
            .Append(" Address:").Append(address.Line3).Append(",").Append(address.City)
            .Append(",").Append(address.Province).Append(",").Append(address.PostalCode).Append(".")
            .Append("Tel1:").Append(address.Line1).Append(" Tel2:").Append(address.Line2).Append(".")
            .Append("Contact:").Append(address.ContactName)
        End With

        Return addDets.ToString
    End Function

    Private Function GetBapisdtext(ByVal textLine As String) As BEKOSAPSalesOrder.Bapisdtext()
        'find the array of Bapitext object required
        Dim numberOfCharPerLine As Integer = 132
        Dim totalArrayLength As Double = 0
        If textLine.Length > numberOfCharPerLine Then
            totalArrayLength = Fix((textLine.Length / numberOfCharPerLine))
        End If
        Const LANGU As String = "E"
        Const TEXT_ID As String = "ZR01"
        Dim Bapisdtext(CInt(totalArrayLength)) As BEKOSAPSalesOrder.Bapisdtext
        If textLine.Length > numberOfCharPerLine Then
            'Have to split this
            Dim tempTextLine As String = textLine
            Dim arrIndex As Integer = 0
            While tempTextLine.Length > 0
                If tempTextLine.Length > numberOfCharPerLine Then
                    Bapisdtext(arrIndex) = New BEKOSAPSalesOrder.Bapisdtext
                    Bapisdtext(arrIndex).ItmNumber = "000000"
                    Bapisdtext(arrIndex).TextId = TEXT_ID
                    Bapisdtext(arrIndex).Langu = LANGU
                    Bapisdtext(arrIndex).TextLine = tempTextLine.Substring(0, numberOfCharPerLine)
                    tempTextLine = tempTextLine.Substring(numberOfCharPerLine)
                Else
                    Bapisdtext(arrIndex) = New BEKOSAPSalesOrder.Bapisdtext
                    Bapisdtext(arrIndex).ItmNumber = "000000"
                    Bapisdtext(arrIndex).TextId = TEXT_ID
                    Bapisdtext(arrIndex).Langu = LANGU
                    Bapisdtext(arrIndex).TextLine = tempTextLine.Substring(0, tempTextLine.Length)
                    tempTextLine = ""
                End If
                arrIndex += 1
            End While
        Else
            Bapisdtext(0) = New BEKOSAPSalesOrder.Bapisdtext
            Bapisdtext(0).ItmNumber = "000000"
            Bapisdtext(0).TextId = TEXT_ID
            Bapisdtext(0).Langu = LANGU
            Bapisdtext(0).TextLine = textLine
        End If
        Return Bapisdtext
    End Function

    Private Function DefaultXMLOrder() As Talent.Common.ErrorObj
        Dim errObj As New ErrorObj

        'Create the Xml Document, Root Element and Transaction Node
        Me.XmlOrderDocument = New XmlDocument
        Dim root As XmlNode = Me.XmlOrderDocument.CreateElement("OrderRequest")
        Dim ver As XmlNode = Me.XmlOrderDocument.CreateElement("Version")
        Dim trans As XmlNode = Me.XmlOrderDocument.CreateElement("TransactionHeader")

        Me.XmlOrderDocument.AppendChild(root)
        With root
            .AppendChild(ver)
            .AppendChild(trans)
        End With
        ver.InnerText = Me.Settings.XmlSettings.XmlVersion

        'Create and add the Transaction Details
        Dim senderID As XmlNode = Me.XmlOrderDocument.CreateElement("SenderID")
        Dim recieverID As XmlNode = Me.XmlOrderDocument.CreateElement("RecieverID")
        Dim countryCode As XmlNode = Me.XmlOrderDocument.CreateElement("CountryCode")
        Dim loginID As XmlNode = Me.XmlOrderDocument.CreateElement("LoginID")
        Dim password As XmlNode = Me.XmlOrderDocument.CreateElement("Password")
        Dim company As XmlNode = Me.XmlOrderDocument.CreateElement("Company")
        Dim transID As XmlNode = Me.XmlOrderDocument.CreateElement("TransactionID")
        With trans
            .AppendChild(senderID)
            .AppendChild(recieverID)
            .AppendChild(countryCode)
            .AppendChild(loginID)
            .AppendChild(password)
            .AppendChild(company)
            .AppendChild(transID)
        End With

        senderID.InnerText = ""
        recieverID.InnerText = ""
        countryCode.InnerText = ""
        loginID.InnerText = ""
        password.InnerText = ""
        company.InnerText = ""
        transID.InnerText = ""


        For Each deo As DeOrders In Dep.CollDEOrders

            Try
                'Add the order header element and info
                Dim order As XmlNode = Me.XmlOrderDocument.CreateElement("Order")
                Dim orderH As XmlNode = Me.XmlOrderDocument.CreateElement("OrderHeaderInformation")
                Dim orderL As XmlNode = Me.XmlOrderDocument.CreateElement("OrderLineInformation")
                Dim billToSuff As XmlNode = Me.XmlOrderDocument.CreateElement("BillToSuffix")
                Dim shipToSuff As XmlNode = Me.XmlOrderDocument.CreateElement("ShipToSuffix")
                Dim addressingInfo As XmlNode = Me.XmlOrderDocument.CreateElement("AddressingInformation")

                root.AppendChild(order)
                order.AppendChild(orderH)
                order.AppendChild(orderL)
                orderH.AppendChild(billToSuff)
                orderH.AppendChild(shipToSuff)
                orderH.AppendChild(addressingInfo)

                billToSuff.InnerText = deo.DEOrderHeader.BillToSuffix
                shipToSuff.InnerText = deo.DEOrderHeader.ShipToSuffix

                'Add the address element and info
                For Each deoa As DeAddress In deo.DEOrderHeader.CollDEAddress

                    Try
                        Dim addressType As String = deoa.Category

                        Dim addressTo As XmlNode = Me.XmlOrderDocument.CreateElement(addressType)
                        Dim address As XmlNode = Me.XmlOrderDocument.CreateElement("Address")
                        Dim attention As XmlNode = Me.XmlOrderDocument.CreateElement(addressType & "Attention")
                        Dim address1 As XmlNode = Me.XmlOrderDocument.CreateElement(addressType & "Address1")
                        Dim address2 As XmlNode = Me.XmlOrderDocument.CreateElement(addressType & "Address2")
                        Dim address3 As XmlNode = Me.XmlOrderDocument.CreateElement(addressType & "Address3")
                        Dim city As XmlNode = Me.XmlOrderDocument.CreateElement(addressType & "City")
                        Dim province As XmlNode = Me.XmlOrderDocument.CreateElement(addressType & "Provice")
                        Dim postalcode As XmlNode = Me.XmlOrderDocument.CreateElement(addressType & "PostalCode")

                        addressingInfo.AppendChild(addressTo)
                        addressTo.AppendChild(address)
                        With address
                            .AppendChild(attention)
                            .AppendChild(address1)
                            .AppendChild(address2)
                            .AppendChild(address3)
                            .AppendChild(city)
                            .AppendChild(province)
                            .AppendChild(postalcode)
                        End With

                        attention.InnerText = deoa.Forename & " " & deoa.Surname
                        address1.InnerText = deoa.Line1
                        address2.InnerText = deoa.Line2
                        address3.InnerText = deoa.Line3
                        city.InnerText = deoa.City
                        province.InnerText = deoa.Province
                        postalcode.InnerText = deoa.PostalCode
                    Catch ex As Exception
                        With errObj
                            .ErrorMessage = "DE Order Error - Failed to Unpack DEAddress"
                            .ErrorNumber = "TACXMLO-03"
                            .HasError = True
                        End With
                    End Try
                Next

                If Not errObj.HasError Then
                    Try
                        Dim custPO As XmlNode = Me.XmlOrderDocument.CreateElement("CustomerPO")
                        Dim EndUserPO As XmlNode = Me.XmlOrderDocument.CreateElement("EndUserPO")
                        addressingInfo.AppendChild(custPO)
                        addressingInfo.AppendChild(EndUserPO)
                        custPO.InnerText = deo.DEOrderHeader.CustomerPO
                        EndUserPO.InnerText = deo.DEOrderHeader.EndUserPO

                        Dim processingOpt As XmlNode = Me.XmlOrderDocument.CreateElement("")
                        orderH.AppendChild(processingOpt)

                        Dim carrier As XmlNode = Me.XmlOrderDocument.CreateElement("CarrierCode")
                        Dim carrierVal As XmlNode = Me.XmlOrderDocument.CreateElement("CarrierCodeValue")
                        Dim autoRelease As XmlNode = Me.XmlOrderDocument.CreateElement("AutoRelease")
                        Dim salesPerson As XmlNode = Me.XmlOrderDocument.CreateElement("SalesPerson")
                        Dim orderDue As XmlNode = Me.XmlOrderDocument.CreateElement("OrderDueDate")
                        Dim suspendCode As XmlNode = Me.XmlOrderDocument.CreateElement("SuspendCode")
                        Dim shipOpts As XmlNode = Me.XmlOrderDocument.CreateElement("ShipmentOptions")
                        Dim backOrdFlag As XmlNode = Me.XmlOrderDocument.CreateElement("BackOrderFlag")
                        Dim splitShipFlag As XmlNode = Me.XmlOrderDocument.CreateElement("SplitShipmentFlag")
                        Dim splitLine As XmlNode = Me.XmlOrderDocument.CreateElement("SplitLine")
                        Dim shipFromBrances As XmlNode = Me.XmlOrderDocument.CreateElement("ShipFromBrances")

                        With processingOpt
                            .AppendChild(carrier)
                            .AppendChild(carrierVal)
                            .AppendChild(autoRelease)
                            .AppendChild(salesPerson)
                            .AppendChild(orderDue)
                            .AppendChild(suspendCode)
                            .AppendChild(shipOpts)
                        End With

                        With shipOpts
                            .AppendChild(backOrdFlag)
                            .AppendChild(splitShipFlag)
                            .AppendChild(splitLine)
                            .AppendChild(shipFromBrances)
                        End With

                        With deo.DEOrderHeader
                            carrier.InnerText = .CarrierCode
                            carrierVal.InnerText = .CarrierCodeValue
                            autoRelease.InnerText = .AutoRelease
                            salesPerson.InnerText = .SalesPerson
                            orderDue.InnerText = .OrderDueDate
                            suspendCode.InnerText = .SuspendCode

                            backOrdFlag.InnerText = .BackOrderFlag
                            splitShipFlag.InnerText = .SplitShipmentFlag
                            splitLine.InnerText = .SplitLine
                            shipFromBrances.InnerText = .ShipFromBranches

                        End With

                        'Add the payment elements and info
                        For Each deopd As DEPayments In deo.DEOrderHeader.CollDEPayments

                            Try
                                Dim payment As XmlNode = Me.XmlOrderDocument.CreateElement("Payment")
                                Dim PaymentType As XmlNode = Me.XmlOrderDocument.CreateElement("PaymentType")
                                Dim Bank As XmlNode = Me.XmlOrderDocument.CreateElement("Bank")
                                Dim Branch As XmlNode = Me.XmlOrderDocument.CreateElement("Branch")
                                Dim Sortcode As XmlNode = Me.XmlOrderDocument.CreateElement("Sortcode")
                                Dim Amount As XmlNode = Me.XmlOrderDocument.CreateElement("Amount")
                                Dim PaymentDate As XmlNode = Me.XmlOrderDocument.CreateElement("PaymentDate")
                                Dim AccountDetails As XmlNode = Me.XmlOrderDocument.CreateElement("AccountDetails")
                                Dim AccountType As XmlNode = Me.XmlOrderDocument.CreateElement("AccountType")
                                Dim AccountNumber As XmlNode = Me.XmlOrderDocument.CreateElement("AccountNumber")
                                Dim AccountName As XmlNode = Me.XmlOrderDocument.CreateElement("AccountName")
                                Dim AccountpostCode As XmlNode = Me.XmlOrderDocument.CreateElement("AccountpostCode")
                                Dim AccountAddress As XmlNode = Me.XmlOrderDocument.CreateElement("AccountAddress")
                                Dim CreditCardDetails As XmlNode = Me.XmlOrderDocument.CreateElement("CreditCardDetails")
                                Dim CardType As XmlNode = Me.XmlOrderDocument.CreateElement("CardType")
                                Dim CardNumber As XmlNode = Me.XmlOrderDocument.CreateElement("CardNumber")
                                Dim ExpiryDate As XmlNode = Me.XmlOrderDocument.CreateElement("ExpiryDate")
                                Dim StartDate As XmlNode = Me.XmlOrderDocument.CreateElement("StartDate")
                                Dim IssueNumber As XmlNode = Me.XmlOrderDocument.CreateElement("IssueNumber")
                                Dim CV2Number As XmlNode = Me.XmlOrderDocument.CreateElement("CV2Number")
                                Dim CardHolderName As XmlNode = Me.XmlOrderDocument.CreateElement("CardHolderName")
                                Dim CardHolderPostCode As XmlNode = Me.XmlOrderDocument.CreateElement("CardHolderPostCode")
                                Dim CardHolderAddress As XmlNode = Me.XmlOrderDocument.CreateElement("CardHolderAddress")

                                orderH.AppendChild(payment)

                                With payment
                                    .AppendChild(PaymentType)
                                    .AppendChild(Bank)
                                    .AppendChild(Branch)
                                    .AppendChild(Sortcode)
                                    .AppendChild(Amount)
                                    .AppendChild(PaymentDate)
                                    .AppendChild(AccountDetails)
                                    .AppendChild(CreditCardDetails)
                                End With

                                With AccountDetails
                                    .AppendChild(AccountType)
                                    .AppendChild(AccountNumber)
                                    .AppendChild(AccountName)
                                    .AppendChild(AccountpostCode)
                                    .AppendChild(AccountAddress)
                                End With

                                With CreditCardDetails
                                    .AppendChild(CardType)
                                    .AppendChild(CardNumber)
                                    .AppendChild(ExpiryDate)
                                    .AppendChild(StartDate)
                                    .AppendChild(IssueNumber)
                                    .AppendChild(CV2Number)
                                    .AppendChild(CardHolderName)
                                    .AppendChild(CardHolderPostCode)
                                    .AppendChild(CardHolderAddress)
                                End With

                                With deopd
                                    PaymentType.InnerText = .PaymentType
                                    Bank.InnerText = .Bank
                                    Branch.InnerText = .Branch
                                    Sortcode.InnerText = .SortCode
                                    Amount.InnerText = .Amount
                                    PaymentDate.InnerText = .PaymentDate

                                    AccountType.InnerText = .AccountType
                                    AccountNumber.InnerText = .AccountNumber
                                    AccountName.InnerText = .AccountName
                                    AccountpostCode.InnerText = .AccountpostCode
                                    AccountAddress.InnerText = .AccountAddress

                                    CardType.InnerText = .CardType
                                    CardNumber.InnerText = .CardNumber
                                    ExpiryDate.InnerText = .ExpiryDate
                                    StartDate.InnerText = .StartDate
                                    IssueNumber.InnerText = .IssueNumber
                                    CV2Number.InnerText = .CV2Number
                                    CardHolderName.InnerText = .CardName
                                    CardHolderPostCode.InnerText = .CardpostCode
                                    CardHolderAddress.InnerText = .CardAddress
                                End With
                            Catch ex As Exception
                                With errObj
                                    .ErrorMessage = "DE Order Error - Failed to add Payment Details to Xml"
                                    .ErrorNumber = "TACXMLO-05"
                                    .HasError = True
                                End With
                            End Try
                        Next

                        If Not errObj.HasError Then

                            Try
                                Dim orderCharges As XmlNode = Me.XmlOrderDocument.CreateElement("OrderCharges")
                                orderH.AppendChild(orderCharges)
                                For Each charge As DECharges In deo.DEOrderHeader.CollDECharges
                                    Dim oCharge As XmlNode = Me.XmlOrderDocument.CreateElement("Charge")
                                    oCharge.InnerText = charge.Value
                                    orderCharges.AppendChild(oCharge)
                                Next
                            Catch ex As Exception
                                With errObj
                                    .ErrorMessage = "DE Order Error - Failed to add Charges to Xml"
                                    .ErrorNumber = "TACXMLO-06"
                                    .HasError = True
                                End With
                            End Try

                            If Not errObj.HasError Then
                                'Add order detail elememts amd info
                                For Each deopl As DeProductLines In deo.DEOrderInfo.CollDEProductLines
                                    Try
                                        Dim line As XmlNode = Me.XmlOrderDocument.CreateElement("ProductLine")
                                        orderL.AppendChild(line)

                                        Dim sku As XmlNode = Me.XmlOrderDocument.CreateElement("SKU")
                                        Dim altSku As XmlNode = Me.XmlOrderDocument.CreateElement("AlternateSKU")
                                        Dim qty As XmlNode = Me.XmlOrderDocument.CreateElement("Quantity")
                                        Dim fprice As XmlNode = Me.XmlOrderDocument.CreateElement("FixedPrice")
                                        Dim cln As XmlNode = Me.XmlOrderDocument.CreateElement("CustomerLineNumber")
                                        Dim resInv As XmlNode = Me.XmlOrderDocument.CreateElement("ReservedInventory")
                                        Dim comm As XmlNode = Me.XmlOrderDocument.CreateElement("LineComment")

                                        With line
                                            .AppendChild(sku)
                                            .AppendChild(altSku)
                                            .AppendChild(qty)
                                            .AppendChild(fprice)
                                            .AppendChild(cln)
                                            .AppendChild(resInv)
                                            .AppendChild(comm)
                                        End With

                                        Dim resCode As XmlNode = Me.XmlOrderDocument.CreateElement("ReserveCode")
                                        Dim resSeq As XmlNode = Me.XmlOrderDocument.CreateElement("ReserveSequence")
                                        With resInv
                                            .AppendChild(resCode)
                                            .AppendChild(resSeq)
                                        End With

                                        With deopl
                                            sku.InnerText = .SKU
                                            altSku.InnerText = .AlternateSKU
                                            qty.InnerText = .Quantity
                                            fprice.InnerText = .FixedPrice
                                            cln.InnerText = .CustomerLineNumber
                                            resCode.InnerText = .ReserveCode
                                            resSeq.InnerText = .ReserveSequence
                                            comm.InnerText = .LineComment
                                        End With
                                    Catch ex As Exception
                                        With errObj
                                            .ErrorMessage = "DE Order Error - Failed to add Order Detail Lines to Xml"
                                            .ErrorNumber = "TACXMLO-07"
                                            .HasError = True
                                        End With
                                    End Try
                                Next

                                If Not errObj.HasError Then

                                    Try
                                        'Add any comment lines
                                        For Each deocl As DeCommentLines In deo.DEOrderInfo.CollDECommentLines
                                            Dim commentLine As XmlNode = Me.XmlOrderDocument.CreateElement("CommentLine")
                                            Dim commentText As XmlNode = Me.XmlOrderDocument.CreateElement("CommentText")

                                            order.AppendChild(commentLine)
                                            commentLine.AppendChild(commentText)
                                            commentText.InnerText = deocl.CommentText
                                        Next
                                    Catch ex As Exception
                                        With errObj
                                            .ErrorMessage = "DE Order Error - Failed to add Order Comment Lines to Xml"
                                            .ErrorNumber = "TACXMLO-08"
                                            .HasError = True
                                        End With
                                    End Try
                                End If
                            End If
                        End If
                    Catch ex As Exception
                        With errObj
                            .ErrorMessage = "DE Order Error - Failed to add Details to Xml"
                            .ErrorNumber = "TACXMLO-04"
                            .HasError = True
                        End With
                    End Try
                End If
            Catch ex As Exception
                With errObj
                    .ErrorMessage = "DE Order Error - Failed to crete Xml Order"
                    .ErrorNumber = "TACXMLO-02"
                    .HasError = True
                End With
            End Try
        Next

        Return errObj
    End Function

    Private Sub ProcessCreatedXML()
        Dim xmlStr As String = ConvertXmlToString(Me.XmlOrderDocument)

        If Me.Settings.XmlSettings.ArchiveXml Then
            Try
                Me.XmlOrderDocument.Save(Me.Settings.XmlSettings.ArchiveXmlLocation)
            Catch ex As Exception
            End Try
        End If

        If Me.Settings.XmlSettings.EmailXmlAttach Then
            Utilities.Email_Send("xml@orders.com", Me.Settings.XmlSettings.EmailXmlRecipient, "Xml Order", "Xml Order", xmlStr, True)
        ElseIf Me.Settings.XmlSettings.EmailXmlContent Then
            Utilities.Email_Send("xml@orders.com", Me.Settings.XmlSettings.EmailXmlRecipient, "Xml Order", xmlStr)
        End If

        If Me.Settings.XmlSettings.PostXml Then


        End If

        If Me.Settings.XmlSettings.StoreXml Then
            Try
                Me.XmlOrderDocument.Save(Me.Settings.XmlSettings.StoreXmlLocation)
            Catch ex As Exception
            End Try
        End If
    End Sub

End Class