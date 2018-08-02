Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Generate Ticketing Basket ID Requests
'
'       Date                        August 2007
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQGTBID- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlGenerateTicketingBasketIDRequest
        Inherits XmlRequest

        Private _deTID As New DETicketingItemDetails

        Public Property DeTID() As DETicketingItemDetails
            Get
                Return _deTID
            End Get
            Set(ByVal value As DETicketingItemDetails)
                _deTID = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlGenerateTicketingBasketIDResponse = CType(xmlResp, XmlGenerateTicketingBasketIDResponse)
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
                With BASKET
                    .Settings = Settings
                    .De = DeTID
                    .ResultDataSet = Nothing
                    err = .GenerateTicketingBasketID
                End With
            End If

            xmlResp.Err = err
            xmlAction.DeTID.SessionId = BASKET.De.SessionId
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1 As XmlNode
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//GenerateTicketingBasketIDRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "CustomerNumber"

                            DeTID = New DETicketingItemDetails
                            DeTID.SessionId = String.Empty
                            DeTID.CustomerNo = Node1.InnerText

                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQGTBID-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace