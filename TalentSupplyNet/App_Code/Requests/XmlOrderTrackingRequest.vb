Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Tracking Requests
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQOT- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlOrderTrackingRequest
        Inherits XmlRequest


        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlOrderTrackingResponse = CType(xmlResp, XmlOrderTrackingResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    Const webService As String = "OrderTrackingRequest"
                    Const NodeName As String = "OrderHeaderInfo"
                    err = LoadDefaultXmlV1(webService, NodeName)

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim order As New TalentOrder
            If Not err.HasError Then
                With order
                    .Dep = Dep
                    .Settings = Settings
                    err = .OrderTracking
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .ResultDataSet = order.ResultDataSet
                .SenderID = Settings.SenderID
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function

    End Class

End Namespace