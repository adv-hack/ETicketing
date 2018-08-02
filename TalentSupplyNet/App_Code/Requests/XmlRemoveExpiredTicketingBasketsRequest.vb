Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Product List Retrieval Requests
'
'       Date                        May 2007
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQRE- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlRemoveExpiredTicketingBasketsRequest
        Inherits XmlRequest

        Dim SQLConnectionStrings As New Generic.List(Of String)

        Private _de As New DETicketingItemDetails
        Public Property De() As DETicketingItemDetails
            Get
                Return _de
            End Get
            Set(ByVal value As DETicketingItemDetails)
                _de = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlRemoveExpiredTicketingBasketsResponse = CType(xmlResp, XmlRemoveExpiredTicketingBasketsResponse)
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
                    .Settings = Settings
                    .De = De
                    If Me.SQLConnectionStrings.Count > 0 Then
                        .MultipleSQLConnectionStrings = Me.SQLConnectionStrings
                        err = .RemoveExpiredBaskets_MultiDBs
                    Else
                        err = .RemoveExpiredBaskets
                    End If
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

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
                For Each Node1 In xmlDoc.SelectSingleNode("//RemoveExpiredTicketingBasketsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "RemoveExpiredTicketingBaskets"

                            De = New DETicketingItemDetails
                            De.Src = "S"
                            With De
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Stadium Details
                                        '
                                        Case Is = "StadiumCode"
                                            .StadiumCode = Node2.InnerText

                                    End Select
                                Next Node2
                            End With

                        Case Is = "SQLConnectionStrings"
                            'loop through and add the ip addresses
                            For Each node3 As XmlNode In Node1.ChildNodes
                                Select Case node3.Name
                                    Case Is = "ConnectionString"
                                        If Not SQLConnectionStrings.Contains(node3.InnerText) Then
                                            SQLConnectionStrings.Add(node3.InnerText)
                                        End If
                                End Select
                            Next

                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQRE-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace