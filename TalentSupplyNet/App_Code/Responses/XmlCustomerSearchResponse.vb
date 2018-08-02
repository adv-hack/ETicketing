Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal

    Public Class XmlCustomerSearchResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndCustomerInfo, ndResponse, ndCustomerDetails, ndCustomers, ndCustomer, ndReturnCode, ndTotalResults As XmlNode
        Private ndCustomerNo, ndContactTitle, ndContactInitials, ndContactForename, ndContactSurname, ndAddressLine1, ndAddressLine2, ndAddressLine3, ndAddressLine4, ndAddressLine5, ndPostCode, ndDOB, ndEmailAddress, ndOnWatchList, ndRowNumber As XmlNode
        Private dtCustomerDetails As DataTable
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()
            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndCustomerInfo = .CreateElement("CustomerSearch")
                    ndResponse = .CreateElement("Response")
                    ndCustomers = .CreateElement("Customers")
                End With

                CreateResponseSection()
                CreateCustomerList(False)

                'Populate the xml document
                With ndCustomerInfo
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndCustomers)
                    End If
                End With

                Const c1 As String = "//"
                Const c2 As String = "/TransactionHeader"
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndCustomerInfo, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSCSR-01"
                    .HasError = True
                End With
            End Try
        End Sub

        Protected Overrides Sub InsertBodyV1_1()
            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndCustomerInfo = .CreateElement("CustomerSearch")
                    ndResponse = .CreateElement("Response")
                    ndCustomers = .CreateElement("Customers")
                End With

                CreateResponseSection()
                CreateCustomerList(False)

                'Populate the xml document
                With ndCustomerInfo
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndCustomers)
                    End If
                End With

                Const c1 As String = "//"
                Const c2 As String = "/TransactionHeader"
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndCustomerInfo, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSCSR-01"
                    .HasError = True
                End With
            End Try
        End Sub

        Protected Sub CreateResponseSection()
            Dim dr As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
            End With

            'Read the values for the response section
            dtStatusDetails = ResultDataSet.Tables(0)
            dr = dtStatusDetails.Rows(0)

            'Populate the nodes
            ndReturnCode.InnerText = dr("ReturnCode")
            If dr("ErrorOccurred") = "E" Then
                errorOccurred = True
            End If

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
            End With
        End Sub

        Protected Sub CreateCustomerList(ByVal includeLoyaltyPoints As Boolean)
            Dim dr As DataRow
            dtCustomerDetails = ResultDataSet.Tables(1)

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndTotalResults = .CreateElement("TotalResults")
                ndTotalResults.InnerText = dtCustomerDetails.Rows.Count.ToString
            End With
            'Set the xml nodes
            With ndResponse
                .AppendChild(ndTotalResults)
            End With

            For Each dr In dtCustomerDetails.Rows
                If Not errorOccurred Then

                    'Create the customer xml nodes
                    With MyBase.xmlDoc
                        ndCustomer = .CreateElement("Customer")
                        ndCustomerNo = .CreateElement("CustomerNo")
                        ndCustomerDetails = .CreateElement("CustomerDetails")
                        ndContactTitle = .CreateElement("ContactTitle")
                        ndContactInitials = .CreateElement("ContactInitials")
                        ndContactForename = .CreateElement("ContactForename")
                        ndContactSurname = .CreateElement("ContactSurname")
                        ndAddressLine1 = .CreateElement("AddressLine1")
                        ndAddressLine2 = .CreateElement("AddressLine2")
                        ndAddressLine3 = .CreateElement("AddressLine3")
                        ndAddressLine4 = .CreateElement("AddressLine4")
                        ndAddressLine5 = .CreateElement("AddressLine5")
                        ndPostCode = .CreateElement("PostCode")
                        ndDOB = .CreateElement("DOB")
                        ndEmailAddress = .CreateElement("EmailAddress")
                        ndOnWatchList = .CreateElement("OnWatchList")
                        ndRowNumber = .CreateElement("RowNumber")
                    End With

                    'Populate the nodes
                    ndCustomerNo.InnerText = dr("CustomerNo").ToString.Trim
                    ndContactTitle.InnerText = dr("ContactTitle").ToString.Trim
                    ndContactInitials.InnerText = dr("ContactInitials").ToString.Trim
                    ndContactForename.InnerText = dr("ContactForename").ToString.Trim
                    ndContactSurname.InnerText = dr("ContactSurname").ToString.Trim
                    ndAddressLine1.InnerText = dr("AddressLine1").ToString.Trim
                    ndAddressLine2.InnerText = dr("AddressLine2").ToString.Trim
                    ndAddressLine3.InnerText = dr("AddressLine3").ToString.Trim
                    ndAddressLine4.InnerText = dr("AddressLine4").ToString.Trim
                    ndAddressLine5.InnerText = dr("AddressLine5").ToString.Trim
                    ndPostCode.InnerText = dr("PostCode").ToString.Trim
                    ndDOB.InnerText = dr("DateBirth").ToString.Trim
                    ndEmailAddress.InnerText = dr("EmailAddress").ToString.Trim
                    ndOnWatchList.InnerText = dr("OnWatchList").ToString.Trim
                    ndRowNumber.InnerText = dtCustomerDetails.Rows.IndexOf(dr).ToString

                    'Set the xml nodes
                        With ndCustomer
                        .AppendChild(ndCustomerNo)
                        .AppendChild(ndContactTitle)
                        .AppendChild(ndContactInitials)
                        .AppendChild(ndContactForename)
                        .AppendChild(ndContactSurname)
                        .AppendChild(ndAddressLine1)
                        .AppendChild(ndAddressLine2)
                        .AppendChild(ndAddressLine3)
                        .AppendChild(ndAddressLine4)
                        .AppendChild(ndAddressLine5)
                        .AppendChild(ndPostCode)
                        .AppendChild(ndDOB)
                        .AppendChild(ndEmailAddress)
                        .AppendChild(ndOnWatchList)
                        .AppendChild(ndRowNumber)
                    End With

                    ' add to the list
                    With ndCustomers
                        .AppendChild(ndCustomer)
                    End With
                End If
            Next
        End Sub

    End Class

End Namespace