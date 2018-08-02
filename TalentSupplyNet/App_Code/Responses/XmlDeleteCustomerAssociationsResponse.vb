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

    Public Class XmlDeleteCustomerAssociationsResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndCustomerInfo, ndResponse, ndCustomerDetails, ndReturnCode As XmlNode
        Private ndCustomerNumber, ndSurname, ndFriendsAndFamilyID, ndPostCode As XmlNode
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

    End Class

End Namespace