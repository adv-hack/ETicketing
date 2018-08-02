Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports System.Data
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Payment Requests
'
'       Date                        June 2007
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQPY- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlPaymentRequest
        Inherits XmlRequest

        Private _depay As New DEPayments
        Private _dePED As New DEPaymentExternalDetails
        Public Property Depay() As DEPayments
            Get
                Return _depay
            End Get
            Set(ByVal value As DEPayments)
                _depay = value
            End Set
        End Property
        Public Property DePED() As DEPaymentExternalDetails
            Get
                Return _dePED
            End Get
            Set(ByVal value As DEPaymentExternalDetails)
                _dePED = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlPaymentResponse = CType(xmlResp, XmlPaymentResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim PAYMENT As New TalentPayment
            If Not err.HasError Then

                'Take the payment
                With PAYMENT
                    .Settings = Settings
                    Depay.Source = "S"
                    .De = Depay
                    .DePED = DePED
                    err = .TakePaymentReadOrder
                End With
            End If

            xmlResp.Err = err
            xmlAction.ResultDataSet = PAYMENT.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2, Node3 As XmlNode
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//PaymentRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "Payment"

                            Depay = New DEPayments
                            DePED = New DEPaymentExternalDetails
                            With Depay
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Payment details
                                        '
                                        Case Is = "PaymentMode"
                                            .PaymentMode = Node2.InnerText

                                        Case Is = "PaymentMethod"
                                            Select Case Node2.InnerText
                                                Case Is = "Cash" : .PaymentType = "CS"
                                                Case Is = "Cheque" : .PaymentType = "CQ"
                                                Case Is = "CreditCard" : .PaymentType = "CC"
                                            End Select

                                        Case Is = "SessionId"
                                            .SessionId = Node2.InnerText

                                        Case Is = "ChequeNumber"
                                            .ChequeNumber = Node2.InnerText

                                        Case Is = "ChequeAccount"
                                            .ChequeAccount = Node2.InnerText

                                        Case Is = "CardNumber"
                                            .CardNumber = Node2.InnerText

                                        Case Is = "ExpiryDate"
                                            .ExpiryDate = Node2.InnerText

                                        Case Is = "StartDate"
                                            .StartDate = Node2.InnerText

                                        Case Is = "IssueNumber"
                                            .IssueNumber = Node2.InnerText

                                        Case Is = "CV2Number"
                                            .CV2Number = Node2.InnerText

                                        Case Is = "SoldByUser"
                                            Settings.OriginatingSource = Node2.InnerText.ToString.ToUpper

                                        Case Is = "PaymentDetails"
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "Reference"
                                                        DePED.ExtPaymentReference = Node3.InnerText
                                                    Case Is = "Name"
                                                        DePED.ExtPaymentName = Node3.InnerText
                                                    Case Is = "Address1"
                                                        DePED.ExtPaymentAddress1 = Node3.InnerText
                                                    Case Is = "Address2"
                                                        DePED.ExtPaymentAddress2 = Node3.InnerText
                                                    Case Is = "Address3"
                                                        DePED.ExtPaymentAddress3 = Node3.InnerText
                                                    Case Is = "Address4"
                                                        DePED.ExtPaymentAddress4 = Node3.InnerText
                                                    Case Is = "Country"
                                                        DePED.ExtPaymentCountry = Node3.InnerText
                                                    Case Is = "PostCode"
                                                        DePED.ExtPaymentPostCode = Node3.InnerText
                                                    Case Is = "Telephone1"
                                                        DePED.ExtPaymentTel1 = Node3.InnerText
                                                    Case Is = "Telephone2"
                                                        DePED.ExtPaymentTel2 = Node3.InnerText
                                                    Case Is = "Telephone3"
                                                        DePED.ExtPaymentTel3 = Node3.InnerText
                                                    Case Is = "EmailAddress"
                                                        DePED.ExtPaymentEmail = Node3.InnerText
                                                End Select
                                            Next
                                    End Select
                                Next Node2
                            End With
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPY-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace