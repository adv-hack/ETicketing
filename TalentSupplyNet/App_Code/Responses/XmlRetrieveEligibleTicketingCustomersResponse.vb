Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with eligible 
'                                   customer list responses
'
'       Date                        09/06/08
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSRETC- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlRetrieveEligibleTicketingCustomersResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndEligibleCustomersRetrieval, ndResponse, ndCustomerList, ndReturnCode As XmlNode
        Private ndCustomer As XmlNode
        Private DtEligibleCustomers As DataTable
        Private dtStatusResults As DataTable
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndEligibleCustomersRetrieval = .CreateElement("EligibleCustomersRetrieval")
                    ndResponse = .CreateElement("Response")
                    ndCustomerList = .CreateElement("CustomerList")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the customer detail section
                CreateCustomerList()

                'Populate the xml document
                With ndEligibleCustomersRetrieval
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndCustomerList)
                    End If
                End With

                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndEligibleCustomersRetrieval, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSRETC-01"
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
            dtStatusResults = ResultDataSet.Tables("StatusResults")
            dr = dtStatusResults.Rows(0)

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

        Protected Sub CreateCustomerList()

            Dim dr As DataRow

            If Not errorOccurred Then

                'Read the values for the response section
                DtEligibleCustomers = ResultDataSet.Tables("EligibleCustomers")

                'Loop around the products
                For Each dr In DtEligibleCustomers.Rows

                    'Create the customer xml nodes
                    With MyBase.xmlDoc
                        ndCustomer = .CreateElement("Customer")

                        ndCustomer.InnerText = dr("Customer")

                        'Add the product to the product list
                        With ndCustomerList
                            .AppendChild(ndCustomer)
                        End With

                    End With
                Next dr
            End If


        End Sub

    End Class

End Namespace