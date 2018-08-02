Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Amend Ticketing Items Requests
'
'       Date                        July 2007
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQAMTI- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlAmendTicketingItemsRequest

        Inherits XmlRequest

        Private _deAmendTicketingItems As New DEAmendTicketingItems
        Private _deTicketingItemDetails As New DETicketingItemDetails

        Public Property DeAmendTicketingItems() As DEAmendTicketingItems
            Get
                Return _deAmendTicketingItems
            End Get
            Set(ByVal value As DEAmendTicketingItems)
                _deAmendTicketingItems = value
            End Set
        End Property

        Public Property DeTicketingItemDetails() As DETicketingItemDetails
            Get
                Return _deTicketingItemDetails
            End Get
            Set(ByVal value As DETicketingItemDetails)
                _deTicketingItemDetails = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlAmendTicketingItemsResponse = CType(xmlResp, XmlAmendTicketingItemsResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim BASKET As New TalentBasket
            If Not err.HasError Then
                ' Remove the Ticketing Items from the ticketing shopping basket
                With BASKET
                    .Settings = Settings
                    .DeAmendTicketingItems = _deAmendTicketingItems
                    .De = DeTicketingItemDetails
                    .De.Src = "S"
                    '                    err = .AmendTicketingItemsReturnBasket
                    err = .AmendTicketingItems
                End With
                If Not err.HasError Then
                    'Retrieve the remaining items in the shopping basket
                    With BASKET
                        .ResultDataSet = Nothing
                        .Settings = Settings
                        .De = DeTicketingItemDetails
                        err = .RetrieveTicketingItems
                    End With
                End If
            End If

            xmlResp.Err = err
            xmlAction.ResultDataSet = BASKET.ResultDataSet
            xmlAction.DeAmendTicketingItems = BASKET.DeAmendTicketingItems
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function

        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2, Node3 As XmlNode
            Dim DeTicketingBasketItem As DETicketingBasketItem
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//AmendTicketingItemsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "AmendTicketingItems"

                            DeAmendTicketingItems = New DEAmendTicketingItems
                            DeAmendTicketingItems.Src = "S"
                            DeTicketingItemDetails = New DETicketingItemDetails
                            With DeAmendTicketingItems

                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Reservation details
                                        '
                                        Case Is = "SessionID"
                                            .SessionID = Node2.InnerText
                                            DeTicketingItemDetails.SessionId = Node2.InnerText

                                        Case Is = "CustomerNumber"
                                            .CustomerNo = Node2.InnerText

                                        Case Is = "Item"
                                            DeTicketingBasketItem = New DETicketingBasketItem
                                            DeTicketingBasketItem.PurchaseMemberNo = .CustomerNo
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "CustomerNumber"
                                                        DeTicketingBasketItem.AllocatedMemberNo = Node3.InnerText
                                                    Case Is = "ProductCode"
                                                        DeTicketingBasketItem.ProductCode = Node3.InnerText
                                                    Case Is = "Seat"
                                                        DeTicketingBasketItem.Seat = Node3.InnerText
                                                    Case Is = "PriceCode"
                                                        DeTicketingBasketItem.PriceCode = Node3.InnerText
                                                    Case Is = "PriceBand"
                                                        DeTicketingBasketItem.PriceBand = Node3.InnerText
                                                    Case Is = "PostOrCollect"
                                                        DeTicketingBasketItem.FulfilmentMethod = Node3.InnerText
                                                End Select
                                            Next Node3
                                            .CollAmendItems.Add(DeTicketingBasketItem)
                                    End Select
                                Next Node2
                            End With

                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQAMTI-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace