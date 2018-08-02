Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Profile
'                                   Manager transactions
'
'       Date                        24/11/08
'
'       Author                      Ben Ford
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRCPMT- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal
    Public Class XmlReceiveProfileManagerTransactionsRequest
        Inherits XmlRequest

        Private _profileManagerTransactions As DEProfileManagerTransactions
        Public Property ProfileManagerTransactions() As DEProfileManagerTransactions
            Get
                Return _profileManagerTransactions
            End Get
            Set(ByVal value As DEProfileManagerTransactions)
                _profileManagerTransactions = value
            End Set
        End Property

        'Invoke constructor on base, passing web service name
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlReceiveProfileManagerTransactionsResponse = CType(xmlResp, XmlReceiveProfileManagerTransactionsResponse)
            Dim profileManager As New TalentProfileManager()
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            If Not err.HasError Then
                With profileManager
                    .ProfileManagerTransactions = ProfileManagerTransactions
                    .Settings = Settings
                    err = .ReceiveProfileManagerTransactions
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .ResultDataSet = profileManager.ResultDataSet
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2, Node3, Node4, Node5 As XmlNode
            ProfileManagerTransactions = New DEProfileManagerTransactions
            profileManagerTransactions.CollDETrans = New Collection
            profileManagerTransactions.CollDEHeader = New Collection

            Dim profileManagerTransaction As DEProfileManagerTransaction = Nothing
            Dim profileManagerLine As DEProfileManagerTransactionLine = Nothing
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//TransactionInterface").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            profileManagerTransactions.CollDETrans.Add(Extract_TransactionHeader(Node1))
                        Case Is = "ListOfTransactions"
                            For Each Node2 In Node1.ChildNodes

                                Select Case Node2.Name
                                    Case Is = "Transaction"
                                        profileManagerTransaction = New DEProfileManagerTransaction
                                        profileManagerTransaction.ColTransactionLines = New Collection
                                        For Each Node3 In Node2.ChildNodes
                                            Select Case Node3.Name
                                                Case Is = "TransactionHeader"
                                                    For Each Node4 In Node3.ChildNodes
                                                        With profileManagerTransaction
                                                            Select Case Node4.Name
                                                                Case Is = "HeaderID"
                                                                    .HeaderID = Node4.InnerText
                                                                Case Is = "SourceSystemID"
                                                                    .SourceSystemID = Node4.InnerText
                                                                Case Is = "SourceRecordType"
                                                                    .SourceRecordType = Node4.InnerText
                                                                Case Is = "RecordEntryDate"
                                                                    .RecordEntryDate = Node4.InnerText
                                                                Case Is = "RecordEntryTime"
                                                                    .RecordEntryTime = Node4.InnerText
                                                                Case Is = "RecordEntryMethod"
                                                                    .RecordEntryMethod = Node4.InnerText
                                                                Case Is = "HeaderUnitPrice"
                                                                    .HeaderUnitPrice = Node4.InnerText
                                                                Case Is = "HeaderTotalPrice"
                                                                    .HeaderTotalPrice = Node4.InnerText
                                                                Case Is = "HeaderVATValue"
                                                                    .HeaderVatValue = Node4.InnerText
                                                                Case Is = "HeaderMargin"
                                                                    .HeaderMargin = Node4.InnerText
                                                                Case Is = "HeaderTotalQuantity"
                                                                    .HeaderTotalQuantity = Node4.InnerText
                                                                Case Is = "SourceCustomerID"
                                                                    .SourceCustomerID = Node4.InnerText
                                                                Case Is = "TalentCustomerID"
                                                                    .TalentCustomerId = Node4.InnerText
                                                                Case Is = "TalentContactID"
                                                                    .TalentContactId = Node4.InnerText
                                                                Case Is = "MemberNo"
                                                                    .MemberNo = Node4.InnerText
                                                                Case Is = "NoteType"
                                                                    .NoteType = Node4.InnerText
                                                                Case Is = "ActionType"
                                                                    .ActionType = Node4.InnerText
                                                                Case Is = "Attribute1"
                                                                    .Attribute1 = Node4.InnerText
                                                                Case Is = "Attribute2"
                                                                    .Attribute2 = Node4.InnerText
                                                                Case Is = "Attribute3"
                                                                    .Attribute3 = Node4.InnerText
                                                                Case Is = "Attribute4"
                                                                    .Attribute4 = Node4.InnerText
                                                                Case Is = "Attribute5"
                                                                    .Attribute5 = Node4.InnerText
                                                                Case Is = "Attribute6"
                                                                    .Attribute6 = Node4.InnerText
                                                                Case Is = "Attribute7"
                                                                    .Attribute7 = Node4.InnerText
                                                                Case Is = "Attribute8"
                                                                    .Attribute8 = Node4.InnerText
                                                                Case Is = "Attribute9"
                                                                    .Attribute9 = Node4.InnerText
                                                                Case Is = "Attribute10"
                                                                    .Attribute10 = Node4.InnerText
                                                            End Select
                                                        End With
                                                    Next
                                                Case Is = "ListOfTransactionDetail"
                                                    For Each Node4 In Node3.ChildNodes
                                                        Select Case Node4.Name
                                                            Case Is = "TransactionDetail"
                                                                profileManagerLine = New DEProfileManagerTransactionLine
                                                                For Each node5 In Node4.ChildNodes
                                                                    With profileManagerLine
                                                                        Select Case node5.Name
                                                                            Case Is = "DetailHeaderID"
                                                                                .DetailHeaderId = Node5.InnerText
                                                                            Case Is = "DetailSourceSystemID"
                                                                                .DetailSourceSystemId = Node5.InnerText
                                                                            Case Is = "DetailSourceRecordType"
                                                                                .DetailSourceRecordType = Node5.InnerText
                                                                            Case Is = "Type"
                                                                                .Type = Node5.InnerText
                                                                            Case Is = "Date"
                                                                                .LineDate = Node5.InnerText
                                                                            Case Is = "Time"
                                                                                .LineTime = Node5.InnerText
                                                                            Case Is = "Agent"
                                                                                .Agent = Node5.InnerText
                                                                            Case Is = "SaleLocation"
                                                                                .SaleLocation = Node5.InnerText
                                                                            Case Is = "ProductCode"
                                                                                .ProductCode = Node5.InnerText
                                                                            Case Is = "ProductCategory1"
                                                                                .ProductCategory1 = Node5.InnerText
                                                                            Case Is = "ProductCategory2"
                                                                                .ProductCategory2 = Node5.InnerText
                                                                            Case Is = "ProductCategory3"
                                                                                .ProductCategory3 = Node5.InnerText
                                                                            Case Is = "ProductCategory4"
                                                                                .ProductCategory4 = Node5.InnerText
                                                                            Case Is = "ProductCategory5"
                                                                                .ProductCategory5 = Node5.InnerText
                                                                            Case Is = "ProductCategory6"
                                                                                .ProductCategory6 = Node5.InnerText
                                                                            Case Is = "ProductDescriptions"
                                                                                .ProductDescription = Node5.InnerText
                                                                            Case Is = "ProductSupplier"
                                                                                .ProductSupplier = Node5.InnerText
                                                                            Case Is = "Quantity"
                                                                                .Quantity = Node5.InnerText
                                                                            Case Is = "UnitPrice"
                                                                                .UnitPrice = Node5.InnerText
                                                                            Case Is = "TotalPrice"
                                                                                .TotalPrice = Node5.InnerText
                                                                            Case Is = "VATValue"
                                                                                .VatValue = Node5.InnerText
                                                                            Case Is = "LineNumber"
                                                                                .LineNumber = Node5.InnerText
                                                                            Case Is = "PaymentMethod"
                                                                                .PaymentMethod = Node5.InnerText
                                                                            Case Is = "CreditCardType"
                                                                                .CreditCardType = Node5.InnerText
                                                                            Case Is = "Margin"
                                                                                .Margin = Node5.InnerText
                                                                            Case Is = "UOM"
                                                                                .UOM = Node5.InnerText
                                                                            Case Is = "ConversionFactor"
                                                                                .ConversionFactor = Node5.InnerText
                                                                            Case Is = "Currency"
                                                                                .Currency = Node5.InnerText
                                                                            Case Is = "Campaign"
                                                                                .Campaign = Node5.InnerText
                                                                            Case Is = "CampaignCode"
                                                                                .CampaignCode = Node5.InnerText
                                                                            Case Is = "EventCode"
                                                                                .EventCode = Node5.InnerText
                                                                            Case Is = "SpecificDetail"
                                                                                .SpecificDetail = Node5.InnerText
                                                                            Case Is = "DiscountValue"
                                                                                .DiscountValue = Node5.InnerText
                                                                            Case Is = "NoteType"
                                                                                .NoteType = Node5.InnerText
                                                                            Case Is = "ActionType"
                                                                                .ActionType = Node5.InnerText
                                                                        End Select
                                                                    End With
                                                                Next Node5
                                                                profileManagerTransaction.ColTransactionLines.Add(profileManagerLine)
                                                        End Select
                                                    Next Node4
                                            End Select
                                        Next Node3
                                        profileManagerTransactions.CollDEHeader.Add(profileManagerTransaction)
                                End Select
                            Next Node2
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRCPMT-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function


    End Class

End Namespace