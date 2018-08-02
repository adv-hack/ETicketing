Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
Imports System.Data
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Remove Ticketing Items Requests
'
'       Date                        July 2007
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQFR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlRemoveTicketingItemsRequest
        Inherits XmlRequest

        Private _deres As New DETicketingItemDetails
        Private _detb As New DETicketingBasket
        Public Property Deres() As DETicketingItemDetails
            Get
                Return _deres
            End Get
            Set(ByVal value As DETicketingItemDetails)
                _deres = value
            End Set
        End Property
        Public Property Detb() As DETicketingBasket
            Get
                Return _detb
            End Get
            Set(ByVal value As DETicketingBasket)
                _detb = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlRemoveTicketingItemsResponse = CType(xmlResp, XmlRemoveTicketingItemsResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
                Case Is = "2.0"
                    err = LoadXmlV2()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim BASKET As New TalentBasket
            Dim dt As New DataTable
            If Not err.HasError Then
                ' Remove the Ticketing Items from the ticketing shopping basket
                With BASKET
                    .Settings = Settings
                    .De = Deres
                    err = .RemoveTicketingItems
                End With
                If Not err.HasError Then

                    'Save a copy of the status table from RemoveTicketingItems 
                    dt = BASKET.ResultDataSet.Tables(0).Copy
                    dt.TableName = "Table1a"

                    'Retrieve the remaining items in the shopping basket
                    With BASKET
                        .ResultDataSet = Nothing
                        err = .RetrieveTicketingItems
                    End With
                End If
            End If

            xmlResp.Err = err

            xmlAction.ResultDataSet = BASKET.ResultDataSet
            xmlAction.ResultDataSet.Tables.Add(dt)
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
                For Each Node1 In xmlDoc.SelectSingleNode("//RemoveTicketingItemsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "RemoveTicketingItems"

                            Deres = New DETicketingItemDetails
                            Deres.Src = "S"
                            With Deres
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Reservation details
                                        '
                                        Case Is = "SessionId"
                                            .SessionId = Node2.InnerText

                                        Case Is = "ProductCode"
                                            .ProductCode = Node2.InnerText

                                        Case Is = "CustomerNo"
                                            .CustomerNo = Node2.InnerText

                                        Case Is = "Seat"
                                            .SeatDetails1.FormattedSeat = Node2.InnerText

                                    End Select
                                Next Node2
                            End With

                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQFR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function LoadXmlV2() As ErrorObj
            Const ModuleName As String = "LoadXmlV2"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2, Node3 As XmlNode
            Dim DeTBI As DETicketingBasketItem
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//RemoveTicketingItemsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "RemoveTicketingItems"

                            Deres = New DETicketingItemDetails
                            Dim DeTB As New DETicketingBasket
                            With Deres
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Reservation details
                                        '
                                        Case Is = "SessionId"
                                            .SessionId = Node2.InnerText

                                        Case Is = "ProductCode"
                                            .ProductCode = Node2.InnerText

                                        Case Is = "Item"
                                            DeTBI = New DETicketingBasketItem
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case node3.name
                                                    Case Is = "CustomerNo"
                                                        DeTBI.ProductCode = Node3.InnerText
                                                    Case Is = "ProductCode"
                                                        DeTBI.ProductCode = Node3.InnerText
                                                    Case Is = "Seat"
                                                        DeTBI.Seat = Node3.InnerText
                                                    Case Is = "PriceCode"
                                                        DeTBI.PriceCode = Node3.InnerText
                                                End Select
                                            Next
                                            DeTB.CollDETicketingBasket.Add(DeTBI)

                                    End Select
                                Next Node2
                            End With

                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQFR-02"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace