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

    Public Class XmlAmendTicketingOrderRequest

        Inherits XmlRequest

        Private _deAmendTicketingOrder As New DEAmendTicketingOrder


        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlAmendTicketingOrderResponse = CType(xmlResp, XmlAmendTicketingOrderResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim ORDER As New TalentOrder
            If Not err.HasError Then
                ' Remove the Ticketing Items from the ticketing shopping basket
                With ORDER
                    .Settings = Settings
                    .DeAmendTicketingOrder = _deAmendTicketingOrder
                    .DeAmendTicketingOrder.Src = "S"
                    .AmendTicketingOrder()
                End With
            End If

            xmlResp.Err = err
            xmlAction.ResultDataSet = ORDER.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function

        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2, Node3 As XmlNode
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//AmendTicketingOrderRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "AmendTicketingOrder"


                            With _deAmendTicketingOrder

                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name

                                        Case Is = "CustomerNumber"
                                            .CustomerNo = Node2.InnerText

                                        Case Is = "PaymentReference"
                                            .PaymentReference = Node2.InnerText

                                        Case Is = "Item"
                                            Dim basketItem As New DETicketingBasketItem
                                            basketItem.PurchaseMemberNo = .CustomerNo
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "CustomerNumber"
                                                        basketItem.AllocatedMemberNo = Node3.InnerText
                                                    Case Is = "ProductCode"
                                                        basketItem.ProductCode = Node3.InnerText
                                                    Case Is = "Seat"
                                                        basketItem.Seat = Node3.InnerText
                                                End Select
                                            Next Node3
                                            .BasketItem.Add(basketItem)
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