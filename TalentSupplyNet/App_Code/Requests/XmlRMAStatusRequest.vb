Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with RMA Status Requests
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQRM- 
'                                   
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlRMAStatusRequest
        Inherits XmlRequest

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlRMAStatusResponse = CType(xmlResp, XmlRMAStatusResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    Const webService As String = "RMAStatusRequest"
                    Const NodeName As String = "RMARequest"
                    err = LoadDefaultXmlV1(webService, NodeName)

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            If Not err.HasError Then
                Dim order As New TalentOrder
                With order
                    .Dep = Dep
                    .Settings = Settings
                    err = .RMAStatus
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function

    End Class

End Namespace