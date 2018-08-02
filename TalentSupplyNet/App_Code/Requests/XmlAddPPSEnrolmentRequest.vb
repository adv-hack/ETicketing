Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common


Namespace Talent.TradingPortal

    Public Class XmlAddPPSEnrolmentRequest
        Inherits XmlRequest

        Private dePPS As DEPPS = New DEPPS
        Private customerNumber As String = String.Empty

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlAddPPSEnrolmentResponse = CType(xmlResp, XmlAddPPSEnrolmentResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
            End Select

            'Populate the session id
            Dim BASKET As New TalentBasket
            If Not err.HasError Then
                With BASKET
                    .Settings = Settings
                    .De.CustomerNo = customerNumber
                    .ResultDataSet = Nothing
                    err = .GenerateTicketingBasketID
                    dePPS.SessionId = .De.SessionId
                End With
            End If

            '-------------------------------
            ' Place the Request if no errors
            '-------------------------------

            Dim PPS As New TalentPPS
            If err.HasError Then
                xmlResp.Err = err
            Else
                With PPS
                    .DEPPS = dePPS
                    .Settings = Settings
                    Settings.OriginatingSourceCode = "S"
                    err = .AddPPSRequest
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            xmlAction.ResultDataSet = PPS.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)
        End Function

        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2 As XmlNode

            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//AddPPSEnrolmentRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "AddPPSEnrolment"

                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "PPSEnrolment"
                                        Dim dePPSEnrolment As DEPPSEnrolment = New DEPPSEnrolment
                                        For Each Node3 As XmlNode In Node2.ChildNodes
                                            Select Case Node3.Name
                                                Case Is = "CustomerNo"
                                                    dePPSEnrolment.CustomerNumber = Node3.InnerText
                                                    If String.IsNullOrEmpty(customerNumber) Then customerNumber = Node3.InnerText
                                                Case Is = "PaymentDetails"
                                                    dePPSEnrolment.PaymentDetails = ProcessPaymentDetails(Node3)
                                                Case Is = "PPSScheme"
                                                    dePPSEnrolment.EnrolmentSchemes.Add(ProcessEnrolmentScheme(Node3))
                                            End Select
                                        Next
                                        dePPS.Enrolments.Add(dePPSEnrolment)
                                End Select
                            Next
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

        Private Function ProcessPaymentDetails(ByVal detailsNode As XmlNode) As DEPayments
            Dim paymentDetails As New DEPayments
            For Each node As XmlNode In detailsNode.ChildNodes
                Select Case node.Name
                    Case Is = "CreditCard"
                        For Each node2 As XmlNode In node.ChildNodes
                            Select Case node2.Name
                                Case Is = "CardNumber"
                                    paymentDetails.CardNumber = node2.InnerText
                                Case Is = "ExpiryDate"
                                    paymentDetails.ExpiryDate = node2.InnerText
                                Case Is = "StartDate"
                                    paymentDetails.StartDate = node2.InnerText
                                Case Is = "IssueNumber"
                                    paymentDetails.IssueNumber = node2.InnerText
                                Case Is = "CV2Number"
                                    paymentDetails.CV2Number = node2.InnerText
                            End Select
                        Next
                    Case Is = "DirectDebit"
                        For Each node2 As XmlNode In node.ChildNodes
                            Select Case node2.Name
                                Case Is = "AccountName"
                                    paymentDetails.AccountName = node2.InnerText
                                Case Is = "SortCode"
                                    paymentDetails.SortCode = node2.InnerText
                                Case Is = "AccountCode"
                                    paymentDetails.AccountNumber = node2.InnerText
                            End Select
                        Next
                End Select
            Next

            'Set the payment type - Default is credit card
            paymentDetails.PaymentType = "CC"
            If Not String.IsNullOrEmpty(paymentDetails.AccountName) Then
                paymentDetails.PaymentType = "DD"
            End If

            Return paymentDetails
        End Function

        Private Function ProcessEnrolmentScheme(ByVal enrolmentSchemeNode As XmlNode) As DEPPSEnrolmentScheme
            Dim enrolmentScheme As DEPPSEnrolmentScheme = New DEPPSEnrolmentScheme            
            For Each node As XmlNode In enrolmentSchemeNode.ChildNodes
                Select Case node.Name
                    Case Is = "CustomerNo"
                        enrolmentScheme.CustomerNumber = node.InnerText
                        If String.IsNullOrEmpty(customerNumber) Then customerNumber = node.InnerText
                    Case Is = "ProductCode"
                        enrolmentScheme.ProductCode = node.InnerText
                    Case Is = "SeasonTicketSeat"
                        enrolmentScheme.SeasonTicket = node.InnerText
                    Case Is = "RegisteredPost"
                        enrolmentScheme.RegisteredPost = node.InnerText
                End Select
            Next
            Return enrolmentScheme
        End Function
    End Class
End Namespace