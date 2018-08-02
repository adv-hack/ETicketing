Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with ticketing order details
'       Date                        September 2007
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQOD- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlRetrievePurchaseHistoryRequest
        Inherits XmlRequest

        Private _de As New DETicketingItemDetails
        Private _deO As New DEOrder
        Public Property De() As DETicketingItemDetails
            Get
                Return _de
            End Get
            Set(ByVal value As DETicketingItemDetails)
                _de = value
            End Set
        End Property
        Public Property DeO() As DEOrder
            Get
                Return _deO
            End Get
            Set(ByVal value As DEOrder)
                _deO = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlRetrievePurchaseHistoryResponse = CType(xmlResp, XmlRetrievePurchaseHistoryResponse)
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
            If err.HasError Then
                xmlResp.Err = err
            Else
                With ORDER
                    .Dep = DeO
                    .Settings = Settings
                    err = .OrderEnquiryDetails
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            xmlAction.ResultDataSet = ORDER.ResultDataSet
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
                For Each Node1 In xmlDoc.SelectSingleNode("//RetrievePurchaseHistoryRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "ProductDetail"
                            De = New DETicketingItemDetails
                            With De
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Print Request Details
                                        '
                                        Case Is = "CustomerNumber"
                                            Settings.AccountNo1 = Node2.InnerText
                                        Case Is = "ProductCode"
                                            .ProductCode = Node2.InnerText
                                        Case Is = "ProductType"
                                            .Type = Node2.InnerText
                                    End Select
                                Next Node2
                            End With
                    End Select
                Next Node1

                'Set the xml information to the order data entity
                DeO = New DEOrder
                DeO.CollDEOrders.Add(De)

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQOD-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace