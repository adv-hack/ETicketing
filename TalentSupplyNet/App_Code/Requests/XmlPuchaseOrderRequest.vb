
Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with PurchaseOrder Requests
'
'       Date                        Feb 2007
'
'       Author                       
'
'       © CS Group 2006             All rights reserved.
'
'       Error Number Code base      TTPRQPO- 
'                                   
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlPurchaseOrderRequest
        Inherits XmlRequest

        Private _din As DEPurchaseOrder
        Public Property Din() As DEPurchaseOrder
            Get
                Return _din
            End Get
            Set(ByVal value As DEPurchaseOrder)
                _din = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlPurchaseOrderResponse = CType(xmlResp, XmlPurchaseOrderResponse)
            Dim purchaseorder As New TalentPurchaseOrder()
            Dim err As ErrorObj = Nothing

            
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()


            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            If Not err.HasError Then
                With purchaseorder
                    .Dep = Din
                    .Settings = Settings
                    err = .GetPurchaseOrder
                End With
            End If


            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                
                .ResultDataSet = purchaseorder.ResultDataSet
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"

            
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Dim Node1 As XmlNode
            Din = New DEPurchaseOrder
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//PurchaseOrderRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Din.CollDETrans.Add(Extract_TransactionHeader(Node1))
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPO-09"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace