Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with add ticketing returned items responses
'
'       Date                        28/04/08
'
'       Author                      Ben Ford
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSATRI- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlAddTicketingReservedItemsResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndAddTicketingItems, ndResponse, ndReturnCode, ndSessionID, ndProduct, ndCustomerNumber As XmlNode
        Private _deAddItems As DEAddTicketingItems
        Public Property DeAddItems() As DEAddTicketingItems
            Get
                Return _deAddItems
            End Get
            Set(ByVal value As DEAddTicketingItems)
                _deAddItems = value
            End Set
        End Property
        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndAddTicketingItems = .CreateElement("AddTicketingReservedItems")
                    ndResponse = .CreateElement("Response")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Populate the xml document
                With ndAddTicketingItems
                    .AppendChild(ndResponse)
                End With

                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"
                Const c2 As String = "/TransactionHeader"
                '
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndAddTicketingItems, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSATRI-01"
                    .HasError = True
                End With
            End Try

        End Sub

        Protected Sub CreateResponseSection()

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
                ndSessionID = .CreateElement("SessionID")
                ndProduct = .CreateElement("Product")
                ndCustomerNumber = .CreateElement("CustomerNumber")
            End With

            'Populate the nodes
            ndReturnCode.InnerText = _deAddItems.ErrorCode
            ndSessionID.InnerText = _deAddItems.SessionId
            ndProduct.InnerText = _deAddItems.ProductCode
            ndCustomerNumber.InnerText = _deAddItems.CustomerNumber

            'Append the xml nodes to the parent node 
            With ndResponse
                .AppendChild(ndReturnCode)
                .AppendChild(ndSessionID)
                .AppendChild(ndProduct)
                .AppendChild(ndCustomerNumber)
            End With

        End Sub

    End Class

End Namespace