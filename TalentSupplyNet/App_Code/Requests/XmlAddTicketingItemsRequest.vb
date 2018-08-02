Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Add Ticketing Items
'
'       Date                        April 2007
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQATI- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlAddTicketingItemsRequest
        Inherits XmlRequest

        Private _deAddItems As New DEAddTicketingItems
        Public Property DeAddItems() As DEAddTicketingItems
            Get
                Return _deAddItems
            End Get
            Set(ByVal value As DEAddTicketingItems)
                _deAddItems = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlAddTicketingItemsResponse = CType(xmlResp, XmlAddTicketingItemsResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
                Case Is = "1.1"
                    err = LoadXmlV1_1()
            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim BASKET As New TalentBasket
            If err.HasError Then
                xmlResp.Err = err
            Else
                With BASKET
                    ' DeAddItems.PriceCode = "CC"
                    .DeAddTicketingItems = DeAddItems
                    .DeAddTicketingItems.Source = "S"
                    .Settings = Settings
                    err = .AddTicketingItemsReturnBasket
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            xmlAction.DeAddItems = _deAddItems
            xmlAction.ResultDataSet = BASKET.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlAction.AlternativeSeatSelected = BASKET.AlternativeSeatSelected
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)


        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2 As XmlNode
            Dim intItemCount As Integer = 0
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data Entity
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//AddTicketingItemsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "Header"
                            DeAddItems = New DEAddTicketingItems
                            With DeAddItems
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        Case Is = "SessionID"
                                            .SessionId = Node2.InnerText
                                        Case Is = "ProductCode"
                                            .ProductCode = Node2.InnerText
                                        Case Is = "StandCode"
                                            If Node2.InnerText = GlobalConstants.STARALLAREA Then
                                                .StandCode = GlobalConstants.STARALLSTAND
                                            Else
                                                .StandCode = Node2.InnerText
                                            End If
                                        Case Is = "AreaCode"
                                            .AreaCode = Node2.InnerText
                                        Case Is = "CustomerNumber"
                                            .CustomerNumber = Node2.InnerText
                                        Case Is = "ExcludeRestrictedSeats"
                                            .ExcludeRestrictedSeats = Node2.InnerText
                                        Case Is = "DefaultPrice"
                                            If Node2.InnerText.Length > 0 Then .DefaultPrice = Node2.InnerText.Replace(".", String.Empty)
                                    End Select

                                Next Node2
                            End With
                        Case Is = "Item"
                            intItemCount = intItemCount + 1
                            With DeAddItems
                                For Each Node2 In Node1.ChildNodes
                                    If Node2.Name = "Quantity" Then
                                        Select Case intItemCount
                                            Case Is = 1 : .Quantity01 = Node2.InnerText
                                            Case Is = 2 : .Quantity02 = Node2.InnerText
                                            Case Is = 3 : .Quantity03 = Node2.InnerText
                                            Case Is = 4 : .Quantity04 = Node2.InnerText
                                            Case Is = 5 : .Quantity05 = Node2.InnerText
                                            Case Is = 6 : .Quantity06 = Node2.InnerText
                                            Case Is = 7 : .Quantity07 = Node2.InnerText
                                            Case Is = 8 : .Quantity08 = Node2.InnerText
                                            Case Is = 9 : .Quantity09 = Node2.InnerText
                                            Case Is = 10 : .Quantity10 = Node2.InnerText
                                            Case Is = 11 : .Quantity11 = Node2.InnerText
                                            Case Is = 12 : .Quantity12 = Node2.InnerText
                                            Case Is = 13 : .Quantity13 = Node2.InnerText
                                            Case Is = 14 : .Quantity14 = Node2.InnerText
                                            Case Is = 15 : .Quantity15 = Node2.InnerText
                                            Case Is = 16 : .Quantity16 = Node2.InnerText
                                            Case Is = 17 : .Quantity17 = Node2.InnerText
                                            Case Is = 18 : .Quantity18 = Node2.InnerText
                                            Case Is = 19 : .Quantity19 = Node2.InnerText
                                            Case Is = 20 : .Quantity20 = Node2.InnerText
                                            Case Is = 21 : .Quantity21 = Node2.InnerText
                                            Case Is = 22 : .Quantity22 = Node2.InnerText
                                            Case Is = 23 : .Quantity23 = Node2.InnerText
                                            Case Is = 24 : .Quantity24 = Node2.InnerText
                                            Case Is = 25 : .Quantity25 = Node2.InnerText
                                            Case Is = 26 : .Quantity26 = Node2.InnerText
                                        End Select
                                    End If
                                    If Node2.Name = "PriceBand" Then
                                        Select Case intItemCount
                                            Case Is = 1 : .PriceBand01 = Node2.InnerText
                                            Case Is = 2 : .PriceBand02 = Node2.InnerText
                                            Case Is = 3 : .PriceBand03 = Node2.InnerText
                                            Case Is = 4 : .PriceBand04 = Node2.InnerText
                                            Case Is = 5 : .PriceBand05 = Node2.InnerText
                                            Case Is = 6 : .PriceBand06 = Node2.InnerText
                                            Case Is = 7 : .PriceBand07 = Node2.InnerText
                                            Case Is = 8 : .PriceBand08 = Node2.InnerText
                                            Case Is = 9 : .PriceBand09 = Node2.InnerText
                                            Case Is = 10 : .PriceBand10 = Node2.InnerText
                                            Case Is = 11 : .PriceBand11 = Node2.InnerText
                                            Case Is = 12 : .PriceBand12 = Node2.InnerText
                                            Case Is = 13 : .PriceBand13 = Node2.InnerText
                                            Case Is = 14 : .PriceBand14 = Node2.InnerText
                                            Case Is = 15 : .PriceBand15 = Node2.InnerText
                                            Case Is = 16 : .PriceBand16 = Node2.InnerText
                                            Case Is = 17 : .PriceBand17 = Node2.InnerText
                                            Case Is = 18 : .PriceBand18 = Node2.InnerText
                                            Case Is = 19 : .PriceBand19 = Node2.InnerText
                                            Case Is = 20 : .PriceBand20 = Node2.InnerText
                                            Case Is = 21 : .PriceBand21 = Node2.InnerText
                                            Case Is = 22 : .PriceBand22 = Node2.InnerText
                                            Case Is = 23 : .PriceBand23 = Node2.InnerText
                                            Case Is = 24 : .PriceBand24 = Node2.InnerText
                                            Case Is = 25 : .PriceBand25 = Node2.InnerText
                                            Case Is = 26 : .PriceBand26 = Node2.InnerText
                                        End Select
                                    End If
                                Next Node2
                            End With
                    End Select
                Next Node1
                DeAddItems.Source = "S"
                DeAddItems.ErrorCode = String.Empty
                DeAddItems.ErrorFlag = String.Empty

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQATI-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function LoadXmlV1_1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1_1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2, Node3 As XmlNode
            Dim intItemCount As Integer = 0
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data Entity
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//AddTicketingItemsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "Header"
                            DeAddItems = New DEAddTicketingItems
                            With DeAddItems
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        Case Is = "SessionID"
                                            .SessionId = Node2.InnerText
                                        Case Is = "ProductCode"
                                            .ProductCode = Node2.InnerText
                                        Case Is = "StandCode"
                                            If Node2.InnerText = GlobalConstants.STARALLAREA Then
                                                .StandCode = GlobalConstants.STARALLSTAND
                                            Else
                                                .StandCode = Node2.InnerText
                                            End If
                                        Case Is = "AreaCode"
                                            .AreaCode = Node2.InnerText
                                        Case Is = "CustomerNumber"
                                            .CustomerNumber = Node2.InnerText
                                        Case Is = "ExcludeRestrictedSeats"
                                            .ExcludeRestrictedSeats = Node2.InnerText
                                        Case Is = "Membership"
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "PriceCode"
                                                        .PriceCode = Node3.InnerText
                                                End Select
                                            Next Node3
                                        Case Is = "ReservationMethod"
                                            DeAddItems.ReservationMethod = Node1.InnerText
                                        Case Is = "DefaultPrice"
                                            If Node2.InnerText.Length > 0 Then .DefaultPrice = Node2.InnerText.Replace(".", String.Empty)
                                    End Select
                                Next Node2
                            End With
                        Case Is = "Item"
                            intItemCount = intItemCount + 1
                            With DeAddItems
                                For Each Node2 In Node1.ChildNodes
                                    If Node2.Name = "Quantity" Then
                                        Select Case intItemCount
                                            Case Is = 1 : .Quantity01 = Node2.InnerText
                                            Case Is = 2 : .Quantity02 = Node2.InnerText
                                            Case Is = 3 : .Quantity03 = Node2.InnerText
                                            Case Is = 4 : .Quantity04 = Node2.InnerText
                                            Case Is = 5 : .Quantity05 = Node2.InnerText
                                            Case Is = 6 : .Quantity06 = Node2.InnerText
                                            Case Is = 7 : .Quantity07 = Node2.InnerText
                                            Case Is = 8 : .Quantity08 = Node2.InnerText
                                            Case Is = 9 : .Quantity09 = Node2.InnerText
                                            Case Is = 10 : .Quantity10 = Node2.InnerText
                                            Case Is = 11 : .Quantity11 = Node2.InnerText
                                            Case Is = 12 : .Quantity12 = Node2.InnerText
                                            Case Is = 13 : .Quantity13 = Node2.InnerText
                                            Case Is = 14 : .Quantity14 = Node2.InnerText
                                            Case Is = 15 : .Quantity15 = Node2.InnerText
                                            Case Is = 16 : .Quantity16 = Node2.InnerText
                                            Case Is = 17 : .Quantity17 = Node2.InnerText
                                            Case Is = 18 : .Quantity18 = Node2.InnerText
                                            Case Is = 19 : .Quantity19 = Node2.InnerText
                                            Case Is = 20 : .Quantity20 = Node2.InnerText
                                            Case Is = 21 : .Quantity21 = Node2.InnerText
                                            Case Is = 22 : .Quantity22 = Node2.InnerText
                                            Case Is = 23 : .Quantity23 = Node2.InnerText
                                            Case Is = 24 : .Quantity24 = Node2.InnerText
                                            Case Is = 25 : .Quantity25 = Node2.InnerText
                                            Case Is = 26 : .Quantity26 = Node2.InnerText
                                        End Select
                                    End If
                                    If Node2.Name = "PriceBand" Then
                                        Select Case intItemCount
                                            Case Is = 1 : .PriceBand01 = Node2.InnerText
                                            Case Is = 2 : .PriceBand02 = Node2.InnerText
                                            Case Is = 3 : .PriceBand03 = Node2.InnerText
                                            Case Is = 4 : .PriceBand04 = Node2.InnerText
                                            Case Is = 5 : .PriceBand05 = Node2.InnerText
                                            Case Is = 6 : .PriceBand06 = Node2.InnerText
                                            Case Is = 7 : .PriceBand07 = Node2.InnerText
                                            Case Is = 8 : .PriceBand08 = Node2.InnerText
                                            Case Is = 9 : .PriceBand09 = Node2.InnerText
                                            Case Is = 10 : .PriceBand10 = Node2.InnerText
                                            Case Is = 11 : .PriceBand11 = Node2.InnerText
                                            Case Is = 12 : .PriceBand12 = Node2.InnerText
                                            Case Is = 13 : .PriceBand13 = Node2.InnerText
                                            Case Is = 14 : .PriceBand14 = Node2.InnerText
                                            Case Is = 15 : .PriceBand15 = Node2.InnerText
                                            Case Is = 16 : .PriceBand16 = Node2.InnerText
                                            Case Is = 17 : .PriceBand17 = Node2.InnerText
                                            Case Is = 18 : .PriceBand18 = Node2.InnerText
                                            Case Is = 19 : .PriceBand19 = Node2.InnerText
                                            Case Is = 20 : .PriceBand20 = Node2.InnerText
                                            Case Is = 21 : .PriceBand21 = Node2.InnerText
                                            Case Is = 22 : .PriceBand22 = Node2.InnerText
                                            Case Is = 23 : .PriceBand23 = Node2.InnerText
                                            Case Is = 24 : .PriceBand24 = Node2.InnerText
                                            Case Is = 25 : .PriceBand25 = Node2.InnerText
                                            Case Is = 26 : .PriceBand26 = Node2.InnerText
                                        End Select
                                    End If
                                    If Node2.Name = "RowSeat" Then
                                        Select Case intItemCount
                                            Case Is = 1 : .RowSeat01 = formatRowSeat(Node2.InnerText)
                                            Case Is = 2 : .RowSeat02 = formatRowSeat(Node2.InnerText)
                                            Case Is = 3 : .RowSeat03 = formatRowSeat(Node2.InnerText)
                                            Case Is = 4 : .RowSeat04 = formatRowSeat(Node2.InnerText)
                                            Case Is = 5 : .RowSeat05 = formatRowSeat(Node2.InnerText)
                                            Case Is = 6 : .RowSeat06 = formatRowSeat(Node2.InnerText)
                                            Case Is = 7 : .RowSeat07 = formatRowSeat(Node2.InnerText)
                                            Case Is = 8 : .RowSeat08 = formatRowSeat(Node2.InnerText)
                                            Case Is = 9 : .RowSeat09 = formatRowSeat(Node2.InnerText)
                                            Case Is = 10 : .RowSeat10 = formatRowSeat(Node2.InnerText)
                                            Case Is = 11 : .RowSeat11 = formatRowSeat(Node2.InnerText)
                                            Case Is = 12 : .RowSeat12 = formatRowSeat(Node2.InnerText)
                                            Case Is = 13 : .RowSeat13 = formatRowSeat(Node2.InnerText)
                                            Case Is = 14 : .RowSeat14 = formatRowSeat(Node2.InnerText)
                                            Case Is = 15 : .RowSeat15 = formatRowSeat(Node2.InnerText)
                                            Case Is = 16 : .RowSeat16 = formatRowSeat(Node2.InnerText)
                                            Case Is = 17 : .RowSeat17 = formatRowSeat(Node2.InnerText)
                                            Case Is = 18 : .RowSeat18 = formatRowSeat(Node2.InnerText)
                                            Case Is = 19 : .RowSeat19 = formatRowSeat(Node2.InnerText)
                                            Case Is = 20 : .RowSeat20 = formatRowSeat(Node2.InnerText)
                                            Case Is = 21 : .RowSeat21 = formatRowSeat(Node2.InnerText)
                                            Case Is = 22 : .RowSeat22 = formatRowSeat(Node2.InnerText)
                                            Case Is = 23 : .RowSeat23 = formatRowSeat(Node2.InnerText)
                                            Case Is = 24 : .RowSeat24 = formatRowSeat(Node2.InnerText)
                                            Case Is = 25 : .RowSeat25 = formatRowSeat(Node2.InnerText)
                                            Case Is = 26 : .RowSeat26 = formatRowSeat(Node2.InnerText)
                                            Case Is = 27 : .RowSeat27 = formatRowSeat(Node2.InnerText)
                                            Case Is = 28 : .RowSeat28 = formatRowSeat(Node2.InnerText)
                                            Case Is = 29 : .RowSeat29 = formatRowSeat(Node2.InnerText)
                                            Case Is = 30 : .RowSeat30 = formatRowSeat(Node2.InnerText)
                                            Case Is = 31 : .RowSeat31 = formatRowSeat(Node2.InnerText)
                                            Case Is = 32 : .RowSeat32 = formatRowSeat(Node2.InnerText)
                                            Case Is = 33 : .RowSeat33 = formatRowSeat(Node2.InnerText)
                                            Case Is = 34 : .RowSeat34 = formatRowSeat(Node2.InnerText)
                                            Case Is = 35 : .RowSeat35 = formatRowSeat(Node2.InnerText)
                                            Case Is = 36 : .RowSeat36 = formatRowSeat(Node2.InnerText)
                                            Case Is = 37 : .RowSeat37 = formatRowSeat(Node2.InnerText)
                                            Case Is = 38 : .RowSeat38 = formatRowSeat(Node2.InnerText)
                                            Case Is = 39 : .RowSeat39 = formatRowSeat(Node2.InnerText)
                                            Case Is = 40 : .RowSeat40 = formatRowSeat(Node2.InnerText)
                                            Case Is = 41 : .RowSeat41 = formatRowSeat(Node2.InnerText)
                                            Case Is = 42 : .RowSeat42 = formatRowSeat(Node2.InnerText)
                                            Case Is = 43 : .RowSeat43 = formatRowSeat(Node2.InnerText)
                                            Case Is = 44 : .RowSeat44 = formatRowSeat(Node2.InnerText)
                                            Case Is = 45 : .RowSeat45 = formatRowSeat(Node2.InnerText)
                                            Case Is = 46 : .RowSeat46 = formatRowSeat(Node2.InnerText)
                                            Case Is = 47 : .RowSeat47 = formatRowSeat(Node2.InnerText)
                                            Case Is = 48 : .RowSeat48 = formatRowSeat(Node2.InnerText)
                                            Case Is = 49 : .RowSeat49 = formatRowSeat(Node2.InnerText)
                                            Case Is = 50 : .RowSeat50 = formatRowSeat(Node2.InnerText)
                                        End Select
                                    End If
                                Next Node2
                            End With                        
                    End Select
                Next Node1
                DeAddItems.Source = "S"
                DeAddItems.ErrorCode = String.Empty
                DeAddItems.ErrorFlag = String.Empty

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQATI-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function formatRowSeat(ByVal rowSeat As String) As String

            ' Format incoming string (Row/Seat/Alpha) into fixed width required
            ' by back end routines RRRRSSSSA  (R = row, S=seat, A=Alpha)
            Dim formattedRowSeat As String = String.Empty
            If rowSeat <> String.Empty Then
                Dim seat() As String = rowSeat.Split("/")
                Dim row As String = seat(0)
                Dim seatNo As String = seat(1)
                Dim alpha As String = " "
                If seat.Length > 2 Then
                    alpha = seat(2)
                End If
                formattedRowSeat = Utilities.FixStringLength(row, 4) & _
                                   Utilities.FixStringLength(seatNo, 4) & _
                                   Utilities.FixStringLength(alpha, 1)
            End If

            Return formattedRowSeat
        End Function


    End Class

End Namespace