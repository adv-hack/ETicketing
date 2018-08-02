Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal

    Public Class XmlAddTicketingItemsResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndAddTicketingItems, ndResponse, ndReturnCode, ndSessionID, ndProductCode, ndStandCode, ndAreaCode, ndCustomerNumber, ndAlternativeSeat As XmlNode
        Private _deAddItems As DEAddTicketingItems
        Private _alternativeSeatSelected As Boolean

        Public Property DeAddItems() As DEAddTicketingItems
            Get
                Return _deAddItems
            End Get
            Set(ByVal value As DEAddTicketingItems)
                _deAddItems = value
            End Set
        End Property
        Public Property AlternativeSeatSelected() As Boolean
            Get
                Return _alternativeSeatSelected
            End Get
            Set(value As Boolean)
                _alternativeSeatSelected = value
            End Set
        End Property

        Protected Overrides Sub InsertBodyV1()
            Try
                With MyBase.xmlDoc
                    ndAddTicketingItems = .CreateElement("AddTicketingItems")
                    ndResponse = .CreateElement("Response")
                End With

                CreateResponseSection()

                With ndAddTicketingItems
                    .AppendChild(ndResponse)
                End With

                Const c1 As String = "//"
                Const c2 As String = "/TransactionHeader"
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndAddTicketingItems, ndHeaderRootHeader)
            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSATI-01"
                    .HasError = True
                End With
            End Try
        End Sub

        Protected Overrides Sub InsertBodyV1_1()
            Try
                With MyBase.xmlDoc
                    ndAddTicketingItems = .CreateElement("AddTicketingItems")
                    ndResponse = .CreateElement("Response")
                End With

                CreateResponseSection()

                With ndAddTicketingItems
                    .AppendChild(ndResponse)
                End With

                Const c1 As String = "//"
                Const c2 As String = "/TransactionHeader"
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndAddTicketingItems, ndHeaderRootHeader)
            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSATI-02"
                    .HasError = True
                End With
            End Try
        End Sub

        Protected Sub CreateResponseSection()
            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
                ndSessionID = .CreateElement("SessionID")
                ndProductCode = .CreateElement("ProductCode")
                ndStandCode = .CreateElement("StandCode")
                ndAreaCode = .CreateElement("AreaCode")
                ndCustomerNumber = .CreateElement("CustomerNumber")
                ndAlternativeSeat = .CreateElement("AlternativeSeat")
            End With

            'Populate the nodes
            ndReturnCode.InnerText = _deAddItems.ErrorCode
            ndSessionID.InnerText = _deAddItems.SessionId
            ndProductCode.InnerText = _deAddItems.ProductCode
            ndStandCode.InnerText = _deAddItems.StandCode
            ndAreaCode.InnerText = _deAddItems.AreaCode
            ndCustomerNumber.InnerText = _deAddItems.CustomerNumber
            ndAlternativeSeat.InnerText = _alternativeSeatSelected

            'Append the xml nodes to the parent node 
            With ndResponse
                .AppendChild(ndReturnCode)
                .AppendChild(ndSessionID)
                .AppendChild(ndProductCode)
                .AppendChild(ndStandCode)
                .AppendChild(ndAreaCode)
                .AppendChild(ndCustomerNumber)
                .AppendChild(ndAlternativeSeat)
            End With
        End Sub

    End Class

End Namespace