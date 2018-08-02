Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Pricing Requests
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRPNA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    

    Public Class XmlPNARequest
        Inherits XmlRequest

        Private Class savedProduct
            Public partNo As String
            Public altPartNo As String
            Public requestedQty As Integer

            Public Sub New(ByVal partno As String, _
                           ByVal altpartno As String, _
                           ByVal requestedQty As Integer)
                Me.partNo = partno
                Me.altPartNo = altpartno
                Me.requestedQty = requestedQty
            End Sub

        End Class

        Private _depna As New DePNA
        Private _depnarequest As New DEPNARequest


        Public Property Depna() As DePNA
            Get
                Return _depna
            End Get
            Set(ByVal value As DePNA)
                _depna = value
            End Set
        End Property
        Public Property Depnarequest() As DEPNARequest
            Get
                Return _depnarequest
            End Get
            Set(ByVal value As DEPNARequest)
                _depnarequest = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            '   Dim xmlAction As XmlInvoiceResponse = CType(xmlResp, XmlInvoiceResponse)
            Dim xmlAction As XmlPNAResponse = CType(xmlResp, XmlPNAResponse)
            Dim pricing As New TalentPricing()
            Dim err As ErrorObj = Nothing
            Dim def As New SupplynetDefaults(ConfigurationManager.AppSettings("DefaultBusinessUnit"), Settings.Company)
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '---------------------------------------------
            '   Place the Request - Do stock Request first
            '---------------------------------------------
            '----------------------
            ' Set up Stock defaults
            '----------------------
            Dim stockSettings As New DESettings
            Dim pr As New Profile
            pr.BusinessUnit = Settings.BusinessUnit
            pr.Company = Settings.Company
            Dim stockDefaults As New Defaults

            pr.CreateProfile(Me.LoginId, Me.Password, Me.Company, "MultiAvailabilityRequest")

            With stockDefaults
                .WebServiceName = "MultiAvailabilityRequest"
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
            With stockSettings
                .AccountNo1 = stockDefaults.AccountNo1                           ' account number part 1
                .AccountNo2 = stockDefaults.AccountNo2                           ' account number part 2
                .AccountNo3 = stockDefaults.AccountNo3                           ' account number part 3
                .AccountNo4 = stockDefaults.AccountNo4                           ' account number part 4
                .AccountNo5 = stockDefaults.AccountNo5                           ' account number part 5
                .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("SYSTEM21").ToString
                .Cacheing = stockDefaults.Cacheing()                             ' Cacheing?
                .CacheTimeMinutes = pr.CacheTimeMinutes()                   ' Cache Time
                .Company = Company                                          ' Company
                .DatabaseType1 = stockDefaults.DatabaseType1()                   ' Database type
                .DestinationDatabase = stockDefaults.DestinationDatabase()       ' Destination Database
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                ''''''''????   .SenderID = senderID                                        ' SenderID
                .WebServiceName = "MultiAvailabilityRequest"                            ' service instance name
                .RetryFailures = stockDefaults.RetryFailures                     ' Retry Failures on/off
                .RetryAttempts = stockDefaults.RetryAttempts                     ' Retry attempts
                .RetryWaitTime = stockDefaults.RetryWaitTime                     ' Retry wait time
                .RetryErrorNumbers = stockDefaults.RetryErrorNumbers             ' Retry error numbers
                ' Set Stored Procedure Group from DB, or if not found, from WebConfig
                If Not String.IsNullOrEmpty(pr.WebServiceStoredProcedureGroup) Then
                    .StoredProcedureGroup = pr.WebServiceStoredProcedureGroup
                Else
                    .StoredProcedureGroup = ConfigurationManager.AppSettings("DefaultStoredProcedureGroup")
                End If
            End With
            Dim stock As New TalentAvailibilty
            Dim dep As New DEProductAlert
            '----------------------------------------
            ' Store additional information which won't 
            ' come back in the results into a hash table
            '-------------------------------------------
            Dim htSavedProducts As New Hashtable
            For Each prodAlert As DEAlerts In Depnarequest.CollDEAlerts
                htSavedProducts.Add(prodAlert.LineNo, New savedProduct(prodAlert.ProductCode, prodAlert.ManufacturerPartNumber, prodAlert.Quantity))
            Next
            dep.CollDETrans = Depnarequest.CollDETrans
            dep.CollDEAlerts = Depnarequest.CollDEAlerts
            '------------------------
            ' Call Stock Availability
            '------------------------
            With stock
                .Dep = dep
                .Settings = stockSettings
                err = .MultipleAvailability
            End With

            Dim tblResults As New Data.DataTable
            If Not err.HasError Then
                tblResults = stock.ResultDataSet.Tables(0)
            End If
            '--------------------------------------------------------
            ' Retrieve results and build Pricing request from results
            ' and original saved line information
            ' - Note there could be extra line from the original
            '   request
            '--------------------------------------------------------
            Dim depnarequestNew As New DEPNARequest
            Dim pa As New DEAlerts
            If tblResults.Rows.Count > 0 Then
                For Each dr As Data.DataRow In tblResults.Rows
                    '----------------------
                    ' Don't add if in error
                    '----------------------
                    If dr("ErrorCode") = String.Empty Then
                        pa = New DEAlerts
                        pa.LineNo = CInt(dr("LineNo"))
                        Dim sp As savedProduct = CType(htSavedProducts(pa.LineNo), savedProduct)
                        pa.Quantity = sp.requestedQty
                        pa.ProductCode = dr("ProductNumber")
                        pa.AvailabilQty = dr("Quantity")
                        pa.BranchID = dr("Warehouse")
                        pa.ManufacturerPartNumber = sp.altPartNo
                        pa.OnOrder = dr("QuantityOnOrder")
                        pa.Description = dr("Description")

                        depnarequestNew.CollDEAlerts.Add(pa)
                    End If
                Next
            End If
            '---------------------
            ' Call pricing routine
            '---------------------
            If Not err.HasError Then
                With pricing
                    .Dep = Depna
                    depnarequestNew.PriceUrl = def.GetDefault("PRICE_URL")
                    .Depnarequest = depnarequestNew
                    .Settings = Settings
                    .ResultDataSet = New Data.DataSet
                    err = .PnaRequest
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .ResultDataSet = pricing.ResultDataSet
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
            Dim Node1 As XmlNode
            Dim dea As New DEAlerts                 ' Items
            Dim lineNo As Integer = 1
            ' Depna = New DePNA
            '-------------------------------------------------------------------------------------
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//PNARequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Depna.CollDETrans.Add(Extract_TransactionHeader(Node1))
                        Case Is = "PNAInformation"
                            '
                            '   <PNAInformation SKU="123A321" ManufacturerPartNumber="" Quantity="10" ReservedInventory ="Y"/> 
                            '-----------------------------------------------------------------------
                            'With Depna
                            '    .SKU = Node1.Attributes("SKU").Value
                            '    .ManufacturerPartNumber = Node1.Attributes("ManufacturerPartNumber").Value
                            '    .Quantity = Node1.Attributes("Quantity").Value
                            '    .ReservedInventory = Node1.Attributes("ReservedInventory").Value
                            'End With
                            dea = New DEAlerts
                            With dea
                                .ProductCode = Node1.Attributes("SKU").Value
                                .BranchID = Node1.Attributes("Warehouse").Value
                                .ManufacturerPartNumber = Node1.Attributes("AlternateSKU").Value
                                .ReservedInventory = Node1.Attributes("ReservedInventory").Value
                                .Quantity = Node1.Attributes("Quantity").Value
                                .LineNo = lineNo
                            End With
                            lineNo += 1
                            Depnarequest.CollDEAlerts.Add(dea)
                        Case Is = "ShowDetail"
                            '
                            '  <ShowDetail>1</ShowDetail>
                            '----------------------------------------------------------------------------------------------
                            With MyBase.xmlDoc.SelectSingleNode("/" & RootElement() & "/ShowDetail")
                                Depna.ShowDetail = .Value
                            End With
                            '----------------------------------------------------------------------------------------------
                            '
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRPNA-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace