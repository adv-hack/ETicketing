Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Retrieve Ticketing Items Requests
'
'       Date                        June 2007
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

    Public Class XmlRetrieveTicketingItemsRequest
        Inherits XmlRequest

        Private _deres As New DETicketingItemDetails

        Public Property Deres() As DETicketingItemDetails
            Get
                Return _deres
            End Get
            Set(ByVal value As DETicketingItemDetails)
                _deres = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlRetrieveTicketingItemsResponse = CType(xmlResp, XmlRetrieveTicketingItemsResponse)
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
                'Retrieve the items in the shopping basket
                With BASKET
                    .Settings = Settings
                    .De = Deres
                    .ResultDataSet = Nothing
                    err = .RetrieveTicketingItems
                End With
            End If

            xmlResp.Err = err
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
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//RetrieveTicketingItemsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "RetrieveTicketingItems"

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

    End Class

End Namespace