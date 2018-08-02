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

    Public Class XmlRefundPaymentRequest
        Inherits XmlRequest

        Private _derpay As New DERefundPayment

        Public Property Derpay() As DERefundPayment
            Get
                Return _derpay
            End Get
            Set(ByVal value As DERefundPayment)
                _derpay = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlRefundPaymentResponse = CType(xmlResp, XmlRefundPaymentResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
                Case Is = "1.1"
                    err = LoadXmlV11()
                Case Else
                    err = New ErrorObj
                    With err
                        .ErrorMessage = "Invalid document version " & MyBase.DocumentVersion
                        .ErrorStatus = "RefundPaymentRequest Error - Invalid Doc Version " & MyBase.DocumentVersion
                        .ErrorNumber = "TTPRQPY-03"
                        .HasError = True
                    End With
            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim PAYMENT As New TalentPayment
            If Not err.HasError Then

                'Take the payment based on version
                Select Case MyBase.DocumentVersion
                    Case Is = "1.0"
                        With PAYMENT
                            .Settings = Settings
                            .Derpay.Src = "S"
                            .Derpay = Derpay
                            err = .RefundPayment
                        End With
                    Case Is = "1.1"
                        If (CType(Derpay.CollRefundItems.Item(1), DERefundItem).ProductDetails <> "") Then
                            With PAYMENT
                                .Settings = Settings
                                .Derpay.Src = "S"
                                .Derpay = Derpay
                                err = .RefundPayment
                            End With
                        Else
                            With PAYMENT
                                .Settings = Settings
                                .Derpay.Src = "S"
                                .Derpay = Derpay
                                err = .CancelAllPayments
                            End With
                        End If
                End Select

                If Not err.HasError Then

                    'Extract the information from the data sets
                    Dim dr0 As DataRow
                    Dim dtStatusDetails As New DataTable
                    dtStatusDetails = PAYMENT.ResultDataSet.Tables(0).Copy

                    'Check for an internal error code
                    dr0 = dtStatusDetails.Rows(0)
                    If dr0("ErrorOccurred") <> "E" Then

                        Dim DtPaymentResults As New DataTable
                        DtPaymentResults = PAYMENT.ResultDataSet.Tables(1).Copy
                        dr0 = DtPaymentResults.Rows(0)

                    End If
                End If

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
            Dim Node1, Node2, Node3 As XmlNode
            Dim Deri As DERefundItem
            Dim Depay As DEPayments

            '-------------------------------------------------------------------------------------
            '   We have the full XML document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//RefundPaymentRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "RefundPayment"

                            'Derpay = New DERefundPayment
                            Depay = New DEPayments

                            With Derpay
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Payment details
                                        '
                                        Case Is = "PaymentReference"
                                            .PaymentReference = Node2.InnerText

                                        Case Is = "RefundCustomerNo"
                                            .RefundCustomerNo = Node2.InnerText

                                        Case Is = "RefundMode"
                                            .PaymentDetails.PaymentMode = Node2.InnerText

                                        Case Is = "RefundMethod"
                                            Select Case Node2.InnerText
                                                Case Is = "Cash" : .PaymentDetails.PaymentType = "CS"
                                                Case Is = "Cheque" : .PaymentDetails.PaymentType = "CQ"
                                                Case Is = "CreditCard" : .PaymentDetails.PaymentType = "CC"
                                            End Select

                                        Case Is = "CardNumber"
                                            .PaymentDetails.CardNumber = Node2.InnerText

                                        Case Is = "ExpiryDate"
                                            .PaymentDetails.ExpiryDate = Node2.InnerText

                                        Case Is = "StartDate"
                                            .PaymentDetails.StartDate = Node2.InnerText

                                        Case Is = "IssueNumber"
                                            .PaymentDetails.IssueNumber = Node2.InnerText

                                        Case Is = "CV2Number"
                                            .PaymentDetails.CV2Number = Node2.InnerText

                                        Case Is = "RetainCustomerReservations"
                                            If Node2.InnerText = "Y" Then
                                                .PaymentDetails.RetainCustomerReservations = "Y"
                                            End If

                                        Case Is = "Product"
                                            Deri = New DERefundItem
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "CustomerNo"
                                                        Deri.CustomerNo = Node3.InnerText
                                                    Case Is = "ProductCode"
                                                        Deri.ProductCode = Node3.InnerText
                                                    Case Is = "ProductDetails"
                                                        Deri.ProductDetails = Node3.InnerText
                                                    Case Is = "Quantity"
                                                        Deri.Quantity = Node3.InnerText
                                                End Select
                                            Next Node3
                                            .CollRefundItems.Add(Deri)
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
        Private Function LoadXmlV11() As ErrorObj

            Const ModuleName As String = "LoadXmlV11"
            Dim err As New ErrorObj
            Dim Node1, Node2, Node3 As XmlNode
            Dim Deri As DERefundItem
            Dim Depay As DEPayments

            '-------------------------------------------------------------------------------------
            '   We have the full XML document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//RefundPaymentRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "RefundPayment"

                            'Derpay = New DERefundPayment
                            Depay = New DEPayments

                            With Derpay
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Payment details
                                        '
                                        Case Is = "PaymentReference"
                                            .PaymentReference = Node2.InnerText

                                        Case Is = "RefundCustomerNo"
                                            .RefundCustomerNo = Node2.InnerText

                                        Case Is = "RefundMode"
                                            .PaymentDetails.PaymentMode = Node2.InnerText

                                        Case Is = "RefundMethod"
                                            Select Case Node2.InnerText
                                                Case Is = "Cash" : .PaymentDetails.PaymentType = "CS"
                                                Case Is = "Cheque" : .PaymentDetails.PaymentType = "CQ"
                                                Case Is = "CreditCard" : .PaymentDetails.PaymentType = "CC"
                                            End Select

                                        Case Is = "CardNumber"
                                            .PaymentDetails.CardNumber = Node2.InnerText

                                        Case Is = "ExpiryDate"
                                            .PaymentDetails.ExpiryDate = Node2.InnerText

                                        Case Is = "StartDate"
                                            .PaymentDetails.StartDate = Node2.InnerText

                                        Case Is = "IssueNumber"
                                            .PaymentDetails.IssueNumber = Node2.InnerText

                                        Case Is = "CV2Number"
                                            .PaymentDetails.CV2Number = Node2.InnerText

                                        Case Is = "RetainCustomerReservations"
                                            If Node2.InnerText = "Y" Then
                                                .PaymentDetails.RetainCustomerReservations = "Y"
                                            End If

                                        Case Is = "Product"
                                            Deri = New DERefundItem
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "CustomerNo"
                                                        Deri.CustomerNo = Node3.InnerText
                                                    Case Is = "ProductCode"
                                                        Deri.ProductCode = Node3.InnerText
                                                    Case Is = "ProductDetails"
                                                        Deri.ProductDetails = Node3.InnerText
                                                    Case Is = "Quantity"
                                                        Deri.Quantity = Node3.InnerText
                                                    Case Is = "PriceBand"
                                                        Deri.PriceBand = Node3.InnerText
                                                    Case Is = "PriceCode"
                                                        Deri.PriceCode = Node3.InnerText
                                                    Case Is = "CancelAllMatching"
                                                        Deri.CancelAllMatching = Node3.InnerText
                                                End Select
                                            Next Node3
                                            .CollRefundItems.Add(Deri)
                                    End Select
                                Next Node2
                            End With
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPY-02"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace