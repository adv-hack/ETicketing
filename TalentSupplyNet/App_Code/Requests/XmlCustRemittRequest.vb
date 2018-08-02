Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                     This class is used to deal with Customer Remittance Requests
'
'       Date                        Apr 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQCRA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal
    Public Class XmlCustRemittRequest
        Inherits XmlRequest
        Private _der As DeRemittances

        Public Property Der() As DeRemittances
            Get
                Return _der
            End Get
            Set(ByVal value As DeRemittances)
                _der = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlCustRemittResponse = CType(xmlResp, XmlCustRemittResponse)
            Dim TalentRemittance As New TalentRemittance()
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
                With TalentRemittance
                    .Der = Der
                    .Settings = Settings
                    err = .CustRemittances
                End With
                If err.HasError Then _
                    xmlResp.Err = err
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .ResultDataSet = TalentRemittance.ResultDataSet
                .Err = err
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
            Dim Node1, Node2, Node3 As XmlNode
            Dim derH As New DeRemittanceHeader                  ' Headers
            Dim derL As New DeRemittanceLines                   ' Items
            Dim RemittanceHeader As String = String.Empty
            Der = New DeRemittances
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//CustomerRemittanceRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Der.CollDETrans.Add(Extract_TransactionHeader(Node1))

                        Case Is = "Remittances"
                            '----------------------------------------------------------------------------------
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "RemittanceHeader"
                                        derH = New DeRemittanceHeader
                                        For Each Node3 In Node2.ChildNodes

                                            With derH
                                                .RemittanceHeader = Node2.Attributes("Header").Value
                                                RemittanceHeader = .RemittanceHeader
                                                Select Case Node3.Name
                                                    Case Is = "CompanyCode" : .CompanyCode = Node3.InnerText
                                                    Case Is = "BankAccountNo" : .BankAccountNo = Node3.InnerText
                                                    Case Is = "SOPorderNo" : .SOPorderNo = Node3.InnerText
                                                    Case Is = "BankReference" : .BankReference = Node3.InnerText
                                                    Case Is = "PaymentMethod" : .PaymentMethod = Node3.InnerText
                                                    Case Is = "PostingDate" : .PostingDate = Node3.InnerText
                                                    Case Is = "CurrencyCode" : .CurrencyCode = Node3.InnerText
                                                    Case Is = "CurrencyValue" : .CurrencyValue = Node3.InnerText
                                                    Case Is = "ConfirmedBaseCurrencyValue" : .ConfirmedBaseCurrencyValue = Node3.InnerText
                                                    Case Is = "CustomerBankCode" : .CustomerBankCode = Node3.InnerText
                                                    Case Is = "ThirdPartyName" : .ThirdPartyName = Node3.InnerText
                                                    Case Is = "ThirdPartyAddressLine1" : .ThirdPartyAddressLine1 = Node3.InnerText
                                                    Case Is = "ThirdPartyAddressLine2" : .ThirdPartyAddressLine2 = Node3.InnerText
                                                    Case Is = "ThirdPartyAddressLine3" : .ThirdPartyAddressLine3 = Node3.InnerText
                                                    Case Is = "ThirdPartyAddressLine4" : .ThirdPartyAddressLine4 = Node3.InnerText
                                                    Case Is = "ThirdPartyAddressLine5" : .ThirdPartyAddressLine5 = Node3.InnerText
                                                    Case Is = "ThirdPartyPostcode" : .ThirdPartyPostcode = Node3.InnerText
                                                    Case Is = "OriginatingAccountName" : .OriginatingAccountName = Node3.InnerText
                                                    Case Is = "OurBankDetails" : .OurBankDetails = Node3.InnerText
                                                    Case Is = "ThirdPartyCountry" : .ThirdPartyCountry = Node3.InnerText
                                                    Case Is = "OurBankCountryCode" : .OurBankCountryCode = Node2.InnerText
                                                End Select
                                            End With
                                        Next
                                        Der.CollDeRemittHeader.Add(derH)
                                        '----------------------------------------------------------
                                    Case Is = "RemittanceLines"
                                        derL = New DeRemittanceLines
                                        For Each Node3 In Node2.ChildNodes
                                            With derL
                                                .RemittanceHeader = RemittanceHeader
                                                Select Case Node3.Name
                                                    Case Is = "RemittanceLine"
                                                        .LineNumber = Node3.Attributes("LineNumber").Value
                                                        .RemittanceLine = Node3.InnerText
                                                    Case Is = "MasterItemType" : .MasterItemType = Node3.InnerText
                                                    Case Is = "LedgerEntryDocumentReference" : .LedgerEntryDocumentReference = Node3.InnerText
                                                    Case Is = "PostingAmountPrime" : .PostingAmountPrime = Node3.InnerText
                                                    Case Is = "DiscountAmountPrime" : .DiscountAmountPrime = Node3.InnerText
                                                    Case Is = "SuppliersReference" : .SuppliersReference = Node3.InnerText
                                                End Select
                                            End With
                                        Next
                                        Der.CollDERemittLines.Add(derL)
                                End Select
                            Next Node2
                            '---------------------------------------------------------------------------------
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQCRA-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace
