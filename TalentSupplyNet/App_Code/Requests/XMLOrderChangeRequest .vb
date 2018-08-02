Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Change Requests
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQOC- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlOrderChangeRequest
        Inherits XmlRequest
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub


        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlOrderChangeResponse = CType(xmlResp, XmlOrderChangeResponse)
            Dim err As ErrorObj = Nothing
            Dim pricingErr As New ErrorObj
            Dim altProductsErr As New ErrorObj
            '
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
            End Select

            Dim ss As DEOrderSettings
            ss = CType(Settings(), DEOrderSettings)
            '-----------------------------------------
            ' Check if need to search for alt products
            '-----------------------------------------            
            If ss.OrderCheckForAltProducts Then
                '   altProductsErr = RepriceBlankPrices()
            End If
            '-----------------------------------------------
            ' Check if need to reprice blank incoming prices
            '-----------------------------------------------
            If ss.RepriceBlankPrice Then
                pricingErr = RepriceBlankPrices()
            End If
            '--------------------------------------------------------------------
            '   Place the Request
            '
            If Not err.HasError AndAlso Not pricingErr.HasError AndAlso Not altProductsErr.HasError Then
                Dim order As New TalentOrder
                With order
                    .Dep = Dep
                    .Settings = Settings
                    err = .Update
                    ResultDataSet = .ResultDataSet
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .ResultDataSet = ResultDataSet
                .CreateResponse()
            End With

            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Dim Node1, Node2, Node3, Node4 As XmlNode
            Dim deoh As New DeOrderHeader           ' Items and DEAddress as Collection
            Dim dead1 As New DeAddress              ' Single Line Item
            Dim dead2 As New DeAddress              ' Single Line Item
            '
            Dim deos As New DeOrders                ' DeOrderHeader, DEOrderInfo  as collections
            Dim deoi As New DEOrderInfo             ' DEProductLines, DECommentLines  as collections
            Dim depr As DeProductLines              ' Multiple Line Item
            Dim decl As DeCommentLines              ' Multiple Line Item
            Dep = New DEOrder
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//OrderChangeRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Dep.CollDETrans.Add(Extract_TransactionHeader(Node1))
                        Case Is = "OrderHeaderInformation"
                            '   Dep.CollDEOrders.Add(Extract_OrderHeader(Node1))
                            deos.DEOrderHeader = Extract_OrderHeader(Node1)
                            ''With deoh
                            ''    For Each Node2 In Node1.ChildNodes
                            ''        Select Case Node2.Name
                            ''            Case Is = "BranchOrderNumber" : .BranchOrderNumber = Node2.InnerText
                            ''            Case Is = "OrderSuffix" : .OrderSuffix = Node2.InnerText
                            ''            Case Is = "CustomerPO" : .CustomerPO = Node2.InnerText
                            ''            Case Is = "OrderActionCode" : .OrderActionCode = Node2.InnerText
                            ''            Case Is = "ShipmentAddressInformation"
                            ''                With dead1
                            ''                    .Category = "ShipTo"
                            ''                    For Each Node3 In Node2.ChildNodes
                            ''                        Select Case Node3.Name
                            ''                            '-----------------------------------------------------------
                            ''                            '   Shipping Address info
                            ''                            '
                            ''                            Case Is = "NewCarrierCode" : deoh.CarrierCode = Node3.InnerText
                            ''                            Case Is = "NewCustomerPO" : deoh.NewCustomerPO = Node3.InnerText
                            ''                            Case Is = "NewEndUserPO" : deoh.EndUserPO = Node3.InnerText
                            ''                            Case Is = "NewBillToSuffix" : deoh.BillToSuffix = Node3.InnerText
                            ''                            Case Is = "NewShipToSuffix" : deoh.ShipToSuffix = Node3.InnerText

                            ''                            Case Is = "NewShipToAttention" : .ContactName = Node3.InnerText
                            ''                            Case Is = "NewShipToAddress1" : .Line1 = Node3.InnerText
                            ''                            Case Is = "NewShipToAddress2" : .Line2 = Node3.InnerText
                            ''                            Case Is = "NewShipToAddress3" : .Line3 = Node3.InnerText
                            ''                            Case Is = "NewShipToCity" : .City = Node3.InnerText
                            ''                            Case Is = "NewShipToProvince" : .Province = Node3.InnerText
                            ''                            Case Is = "NewShipToPostalCode" : .PostalCode = Node3.InnerText
                            ''                        End Select
                            ''                    Next Node3
                            ''                End With
                            ''                deoh.CollDEAddress.Add(dead1)
                            ''            Case Is = "BillToAddressInformation"
                            ''                With dead2
                            ''                    .Category = "BillTo"
                            ''                    For Each Node3 In Node2.SelectSingleNode("BillToAddressInformation").ChildNodes
                            ''                        Select Case Node3.Name
                            ''                            '-----------------------------------------------------------
                            ''                            '   Billing Address info
                            ''                            '
                            ''                            Case Is = "NewBillToAttention" : .ContactName = Node3.InnerText
                            ''                            Case Is = "NewBillToAddress1" : .Line1 = Node3.InnerText
                            ''                            Case Is = "NewBillToAddress2" : .Line2 = Node3.InnerText
                            ''                            Case Is = "NewBillToAddress3" : .Line3 = Node3.InnerText
                            ''                            Case Is = "NewBillToCity" : .City = Node3.InnerText
                            ''                            Case Is = "NewBillToProvince" : .Province = Node3.InnerText
                            ''                            Case Is = "NewBillToPostalCode" : .PostalCode = Node3.InnerText
                            ''                        End Select
                            ''                    Next Node3
                            ''                End With
                            ''                deoh.CollDEAddress.Add(dead2)
                            ''        End Select
                            ''    Next Node2
                            ''End With
                            ''deos.DEOrderHeader = deoh
                            ' deos.DEOrderHeader = deoh
                            '----------------------------------------------------------------------------------------------
                        Case Is = "LineInformation"
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "AddLine", "ChangeLine", "DeleteLine"
                                        depr = New DeProductLines
                                        With depr
                                            For Each Node3 In Node2.ChildNodes
                                                .Category = Node2.Name
                                                Select Case Node3.Name.ToUpper
                                                    Case Is = "WESTCOASTLINENUMBER" : .WestCoastLineNumber = Node3.InnerText
                                                    Case Is = "SKU" : .SKU = Node3.InnerText
                                                    Case Is = "QUANTITY" : .Quantity = Node3.InnerText
                                                    Case Is = "CUSTOMERLINENUMBER" : .CustomerLineNumber = Node3.InnerText
                                                    Case Is = "SHIPFROMWAREHOUSE" : .ShipFromWarehouse = Node3.InnerText
                                                    Case Is = "SUFFIX" : .Suffix = Node3.InnerText
                                                    Case Is = "CANCELLATIONCODE" : .CancellationCode = Node3.InnerText
                                                    Case Is = "EXTENSION"
                                                        For Each Node4 In Node3.ChildNodes
                                                            Select Case Node4.Name
                                                                Case Is = "Reference1" : .ExtensionReference1 = Node4.InnerText
                                                                Case Is = "Reference2" : .ExtensionReference2 = Node4.InnerText
                                                                Case Is = "Reference3" : .ExtensionReference3 = Node4.InnerText
                                                                Case Is = "Reference4" : .ExtensionReference4 = Node4.InnerText
                                                                Case Is = "Reference5" : .ExtensionReference5 = Node4.InnerText
                                                                Case Is = "Reference6" : .ExtensionReference6 = Node4.InnerText
                                                                Case Is = "Reference7" : .ExtensionReference7 = Node4.InnerText
                                                                Case Is = "Reference8" : .ExtensionReference8 = Node4.InnerText
                                                                Case Is = "Flag1" : .ExtensionFlag1 = Node4.InnerText
                                                                Case Is = "Flag2" : .ExtensionFlag2 = Node4.InnerText
                                                                Case Is = "Flag3" : .ExtensionFlag3 = Node4.InnerText
                                                                Case Is = "Flag4" : .ExtensionFlag4 = Node4.InnerText
                                                                Case Is = "Flag5" : .ExtensionFlag5 = Node4.InnerText
                                                                Case Is = "Flag6" : .ExtensionFlag6 = Node4.InnerText
                                                                Case Is = "Flag7" : .ExtensionFlag7 = Node4.InnerText
                                                                Case Is = "Flag8" : .ExtensionFlag8 = Node4.InnerText
                                                                Case Is = "Flag9" : .ExtensionFlag9 = Node4.InnerText
                                                                Case Is = "Flag0" : .ExtensionFlag0 = Node4.InnerText
                                                                Case Is = "Field1" : .ExtensionField1 = Node4.InnerText
                                                                Case Is = "Field2" : .ExtensionField2 = Node4.InnerText
                                                                Case Is = "Field3" : .ExtensionField3 = Node4.InnerText
                                                                Case Is = "Field4" : .ExtensionField4 = Node4.InnerText
                                                                Case Is = "FixedPrice1" : .ExtensionFixedPrice1 = Node4.InnerText
                                                                Case Is = "FixedPrice2" : .ExtensionFixedPrice2 = Node4.InnerText
                                                                Case Is = "FixedPrice3" : .ExtensionFixedPrice3 = Node4.InnerText
                                                                Case Is = "FixedPrice4" : .ExtensionFixedPrice4 = Node4.InnerText
                                                                Case Is = "FixedPrice5" : .ExtensionFixedPrice5 = Node4.InnerText
                                                                Case Is = "FixedPrice6" : .ExtensionFixedPrice6 = Node4.InnerText
                                                                Case Is = "FixedPrice7" : .ExtensionFixedPrice7 = Node4.InnerText
                                                                Case Is = "FixedPrice8" : .ExtensionFixedPrice8 = Node4.InnerText
                                                                Case Is = "DealID1" : .ExtensionDealID1 = Node4.InnerText
                                                                Case Is = "DealID2" : .ExtensionDealID2 = Node4.InnerText
                                                                Case Is = "DealID3" : .ExtensionDealID3 = Node4.InnerText
                                                                Case Is = "DealID4" : .ExtensionDealID4 = Node4.InnerText
                                                                Case Is = "DealID5" : .ExtensionDealID5 = Node4.InnerText
                                                                Case Is = "DealID6" : .ExtensionDealID6 = Node4.InnerText
                                                                Case Is = "DealID7" : .ExtensionDealID7 = Node4.InnerText
                                                                Case Is = "DealID8" : .ExtensionDealID8 = Node4.InnerText
                                                                Case Is = "Status" : .ExtensionStatus = Node4.InnerText

                                                            End Select
                                                        Next Node4
                                                End Select
                                            Next Node3
                                        End With
                                        deoi.CollDEProductLines.Add(depr)
                                        '---------------------------------------------------------------------------------
                                    Case Is = "AddComment"
                                        decl = New DeCommentLines
                                        With decl
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "CommentText" : .CommentText = Node3.InnerText
                                                    Case Is = "CustomerLineNumber" : .CustomerLineNumber = Node3.InnerText
                                                    Case Is = "Suffix" : .Suffix = Node3.InnerText
                                                End Select
                                            Next Node3
                                        End With
                                        deoi.CollDECommentLines.Add(decl)
                                End Select
                            Next Node2
                            deos.DEOrderInfo = deoi
                            '----------------------------------------------------------------------------------------------
                        Case Is = "ShowDetail"
                            deos.ShowDetail = Node1.InnerText
                            '
                    End Select
                Next Node1
                '   deos.or()
                Dep.CollDEOrders.Add(deos)

                'For i As Integer = 1 To Dep.CollDEOrders.Count
                '    Dim str As String
                '    str = Dep.CollDEOrders.Item(i)
                'Next
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQOC-09"
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
                            '--------------------------------------------------
                            ' Loop through items and if any are blank and 
                            ' it's a change or and addthen call pricing routine
                            '--------------------------------------------------
                            If (lineItem.Category = "AddLine" OrElse lineItem.Category = "ChangeLine") AndAlso _
                                    lineItem.FixedPrice = String.Empty AndAlso Not lineItem.LineError Then
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
                    .ErrorNumber = "TTPRQOC-02"
                    .HasError = True
                End With
            End Try

            Return err
        End Function
        '----------------------------------------------
        ' CheckForAltProducts - NOT CURRENTLY CALLED AS
        ' INPUT XML DOESN'T INCLUDE ALT ITEM TAG
        '----------------------------------------------
        Private Function CheckForAltProducts() As ErrorObj
            Dim err As New ErrorObj

            Dim product As New TalentProduct()
            Dim productCollection As New Collection()
            Dim pa As New DEAlerts
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
                            If (lineItem.Category = "AddLine" OrElse lineItem.Category = "ChangeLine") AndAlso _
                                        lineItem.AlternateSKU <> String.Empty Then
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
                    .ErrorNumber = "TTPRQOC-03"
                    .HasError = True
                End With
            End Try

            Return err
        End Function
    End Class



End Namespace