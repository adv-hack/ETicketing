Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Detail Requests
'
'       Date                        Nov 2006
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

    Public Class XmlOrderDetailRequest
        Inherits XmlRequest

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlOrderDetailResponse = CType(xmlResp, XmlOrderDetailResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    Const webService As String = "OrderDetailRequest"
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
                    err = .OrderDetails
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .ResultDataSet = order.ResultDataSet
                .Err = err
                .SenderID = Settings.SenderID
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function
    End Class

End Namespace