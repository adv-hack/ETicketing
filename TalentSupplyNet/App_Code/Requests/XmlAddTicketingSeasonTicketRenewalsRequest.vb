Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Add Ticketing 
'                                   Season Ticket Renewals
'
'       Date                        19/06/2009
'
'       Author                      Ben Ford     
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQATSTR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlAddTicketingSeasonTicketRenewalsRequest
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

            Dim xmlAction As XmlAddTicketingSeasonTicketRenewalsResponse = CType(xmlResp, XmlAddTicketingSeasonTicketRenewalsResponse)
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
            If err.HasError Then
                xmlResp.Err = err
            Else
                With BASKET
                    .DeAddTicketingItems = DeAddItems
                    .DeAddTicketingItems.Source = "S"
                    .Settings = Settings
                    err = .AddSeasonTicketRenewalsReturnBasket
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            xmlAction.DeAddItems = _deAddItems
            xmlAction.ResultDataSet = BASKET.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
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
                For Each Node1 In xmlDoc.SelectSingleNode("//AddTicketingSeasonTicketRenewalsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "Header"
                            DeAddItems = New DEAddTicketingItems
                            With DeAddItems
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        Case Is = "SessionID"
                                            .SessionId = Node2.InnerText
                                        Case Is = "CustomerNumber"
                                            .CustomerNumber = Node2.InnerText
                                        Case Is = "StadiumCode"
                                            .Stadium1 = Node2.InnerText
                                        Case Is = "IgnoreFriendsAndFamily"
                                            .IgnoreFriendsAndFamily = Node2.InnerText
                                    End Select

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
                    .ErrorNumber = "TTPRQATSTR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace