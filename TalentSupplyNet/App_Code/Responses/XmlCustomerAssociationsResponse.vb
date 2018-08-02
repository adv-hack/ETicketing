Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with customer retrieval responses
'
'       Date                        April 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSCR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlCustomerAssociationsResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndCustomerInfo, ndResponse, ndCustomerDetails, ndReturnCode As XmlNode
        Private ndCustomerNumber, ndForename, ndSurname, ndAssociatedCustomerNumber, _
                ndAddressLine1, ndPostCode, ndPriceBand, ndActiveFlag, ndLoyalityPoints As XmlNode
        Private dtCustomerDetails As DataTable
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndCustomerInfo = .CreateElement("CustomerRetrieval")
                    ndResponse = .CreateElement("Response")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the customer detail section
                CreateCustomerDetail()

                'Populate the xml document
                With ndCustomerInfo
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndCustomerDetails)
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
                ndHeader.InsertAfter(ndCustomerInfo, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSCR-01"
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

        Protected Sub CreateCustomerDetail()

            Dim dr As DataRow

            If Not errorOccurred Then

                'Read the values for the response section
                dtCustomerDetails = ResultDataSet.Tables(1)

                'Loop around the products
                For Each dr In dtCustomerDetails.Rows

                    'Create the customer xml nodes
                    With MyBase.xmlDoc
                        ndCustomerDetails = .CreateElement("CustomerDetails")
                        ndCustomerNumber = .CreateElement("CustomerNumber")
                        ndAssociatedCustomerNumber = .CreateElement("AssociatedCustomerNumber")
                        ndForename = .CreateElement("Forename")
                        ndSurname = .CreateElement("Surname")
                        ndAddressLine1 = .CreateElement("AddressLine1")
                        ndPostCode = .CreateElement("PostCode")
                        ndPriceBand = .CreateElement("PriceBand")
                        ndActiveFlag = .CreateElement("ActiveFlag")
                        ndLoyalityPoints = .CreateElement("LoyaltyPoints")
                    End With

                    'Populate the nodes
                    ndCustomerNumber.InnerText = dr("CustomerNumber")
                    ndAssociatedCustomerNumber.InnerText = dr("AssociatedCustomerNumber")
                    ndForename.InnerText = dr("Forename")
                    ndSurname.InnerText = dr("Surname")
                    ndAddressLine1.InnerText = dr("AddressLine1")
                    ndPostCode.InnerText = dr("Postcode")
                    ndPriceBand.InnerText = dr("PriceBand")
                    ndActiveFlag.InnerText = dr("ActiveFlag")
                    ndLoyalityPoints.InnerText = dr("LoyalityPoints")

                    'Set the xml nodes
                    With ndCustomerDetails
                        .AppendChild(ndCustomerNumber)
                        .AppendChild(ndAssociatedCustomerNumber)
                        .AppendChild(ndForename)
                        .AppendChild(ndSurname)
                        .AppendChild(ndAddressLine1)
                        .AppendChild(ndPostCode)
                        .AppendChild(ndPriceBand)
                        .AppendChild(ndActiveFlag)
                        .AppendChild(ndLoyalityPoints)
                    End With

                    'Add the product to the product list
                    With ndCustomerInfo
                        .AppendChild(ndCustomerDetails)
                    End With

                Next dr
            End If

        End Sub

    End Class

End Namespace