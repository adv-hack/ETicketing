Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal

    Public Class XmlAddPPSEnrolmentResponse
        Inherits XmlResponse

        Private errorOccurred As Boolean = False
        Private _dePPS As DEPPS

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader, ndAddPPSEnrolment, ndSuccessCount, ndErrorCount, ndResponse, ndFailedRequests As XmlNode
        Private ndPPSEnrolment, ndCustomerNo, ndPaymentDetails, ndCreditCard, ndCardNumber, ndExpiryDate, ndStartDate, ndIssueNumber, ndCV2Number As XmlNode
        Private ndDirectDebit, ndAccountName, ndSortCode, ndAccountCode As XmlNode
        Private ndPPSScheme, ndProductCode, ndSeasonTicketSeat, ndRegisteredPost, ndReturnCode As XmlNode

        Private dtStatusResults As DataTable        

        Public Property dePPS() As DEPPS
            Get
                Return _dePPS
            End Get
            Set(ByVal value As DEPPS)
                _dePPS = value
            End Set
        End Property

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndAddPPSEnrolment = .CreateElement("AddPPSEnrolment")
                    ndResponse = .CreateElement("Response")
                    ndFailedRequests = .CreateElement("FailedRequests")
                End With

                'Create the child xml section
                CreateResponseSection()
                CreateFailedRequestsSection()

                'Populate the xml document
                With ndAddPPSEnrolment
                    .AppendChild(ndResponse)
                    .AppendChild(ndFailedRequests)
                End With

                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndAddPPSEnrolment, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPRNL-01"
                    .HasError = True
                End With
            End Try

        End Sub

        Protected Sub CreateResponseSection()

            Dim dr As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndSuccessCount = .CreateElement("SuccessCount")
                ndErrorCount = .CreateElement("ErrorCount")
            End With

            'Read the values for the response section
            dtStatusResults = ResultDataSet.Tables(0)
            dr = dtStatusResults.Rows(0)

            'Populate the nodes
            ndSuccessCount.InnerText = dr("SuccessCount")
            ndErrorCount.InnerText = dr("ErrorCount")

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndSuccessCount)
                .AppendChild(ndErrorCount)
            End With
        End Sub

        Protected Sub CreateFailedRequestsSection()

            Dim dr As DataRow

            'Do we have any failed responses
            If dtStatusResults.Rows(0).Item("ErrorCount") > 0 Then

                'Loop through the failed requests
                For Each dr In ResultDataSet.Tables(1).Rows

                    Dim enrol As Talent.Common.DEPPSEnrolment = dr("PPSEnrolment")

                    'Create the pps xml nodes
                    With MyBase.xmlDoc
                        ndPPSEnrolment = .CreateElement("PPSEnrolment")
                        If Not String.IsNullOrEmpty(enrol.CustomerNumber) Then ndCustomerNo = .CreateElement("CustomerNo")
                        ndPaymentDetails = .CreateElement("PaymentDetails")
                        ndCreditCard = .CreateElement("CreditCard")
                        ndCardNumber = .CreateElement("CardNumber")
                        ndExpiryDate = .CreateElement("ExpiryDate")
                        ndStartDate = .CreateElement("StartDate")
                        ndIssueNumber = .CreateElement("IssueNumber")
                        ndCV2Number = .CreateElement("CV2Number")
                        ndDirectDebit = .CreateElement("DirectDebit")
                        ndAccountName = .CreateElement("AccountName")
                        ndSortCode = .CreateElement("SortCode")
                        ndAccountCode = .CreateElement("AccountCode")
                    End With

                    'Populate the nodes
                    If Not String.IsNullOrEmpty(enrol.CustomerNumber) Then ndCustomerNo.InnerText = enrol.CustomerNumber
                    ndCardNumber.InnerText = enrol.PaymentDetails.CardNumber
                    ndExpiryDate.InnerText = enrol.PaymentDetails.ExpiryDate
                    ndStartDate.InnerText = enrol.PaymentDetails.StartDate
                    ndIssueNumber.InnerText = enrol.PaymentDetails.IssueNumber
                    ndCV2Number.InnerText = enrol.PaymentDetails.CV2Number
                    With ndCreditCard
                        .AppendChild(ndCardNumber)
                        .AppendChild(ndExpiryDate)
                        .AppendChild(ndStartDate)
                        .AppendChild(ndIssueNumber)
                        .AppendChild(ndCV2Number)
                    End With
                    ndAccountName.InnerText = enrol.PaymentDetails.AccountName
                    ndSortCode.InnerText = enrol.PaymentDetails.SortCode
                    ndAccountCode.InnerText = enrol.PaymentDetails.AccountNumber
                    With ndDirectDebit
                        .AppendChild(ndAccountName)
                        .AppendChild(ndSortCode)
                        .AppendChild(ndAccountCode)
                    End With
                    With ndPaymentDetails
                        .AppendChild(ndCreditCard)
                        .AppendChild(ndDirectDebit)
                    End With

                    'Set the xml nodes
                    With ndPPSEnrolment
                        If Not String.IsNullOrEmpty(enrol.CustomerNumber) Then .AppendChild(ndCustomerNo)
                        .AppendChild(ndPaymentDetails)

                        For i As Integer = 0 To enrol.EnrolmentSchemes.Count - 1

                            'Create the pps xml nodes
                            With MyBase.xmlDoc
                                ndPPSScheme = .CreateElement("PPSScheme")
                                If String.IsNullOrEmpty(enrol.CustomerNumber) Then ndCustomerNo = .CreateElement("CustomerNo")
                                ndProductCode = .CreateElement("ProductCode")
                                ndSeasonTicketSeat = .CreateElement("SeasonTicketSeat")
                                ndRegisteredPost = .CreateElement("RegisteredPost")
                                ndReturnCode = .CreateElement("ReturnCode")
                            End With

                            'Populate the nodes
                            If String.IsNullOrEmpty(enrol.CustomerNumber) Then ndCustomerNo.InnerText = enrol.EnrolmentSchemes.Item(i).CustomerNumber
                            ndProductCode.InnerText = enrol.EnrolmentSchemes.Item(i).ProductCode
                            ndSeasonTicketSeat.InnerText = enrol.EnrolmentSchemes.Item(i).SeasonTicket
                            ndRegisteredPost.InnerText = enrol.EnrolmentSchemes.Item(i).RegisteredPost
                            ndReturnCode.InnerText = enrol.EnrolmentSchemes.Item(i).ErrorCode

                            'Set the xml nodes
                            With ndPPSScheme
                                If String.IsNullOrEmpty(enrol.CustomerNumber) Then .AppendChild(ndCustomerNo)
                                .AppendChild(ndProductCode)
                                .AppendChild(ndSeasonTicketSeat)
                                .AppendChild(ndRegisteredPost)
                                .AppendChild(ndReturnCode)
                            End With

                            'Append to the pps enrolment node
                            .AppendChild(ndPPSScheme)
                        Next

                    End With

                    'Append the failed request
                    ndFailedRequests.AppendChild(ndPPSEnrolment)
                Next
            End If

        End Sub


    End Class

End Namespace