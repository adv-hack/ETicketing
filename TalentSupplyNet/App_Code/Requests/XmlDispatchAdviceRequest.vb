Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Disdpatch Advice Requests
'
'       Date                        10th Nov 2006
'
'       Author                      Andy White   
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQDA- 
'                                                                       
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlDispatchAdviceRequest
        Inherits XmlRequest

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlDispatchAdviceResponse = CType(xmlResp, XmlDispatchAdviceResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    Const webService As String = "DispatchAdviceRequest"
                    Const NodeName As String = "DispatchRequest"
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
                    err = .DispatchAdvice
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .ResultDataSet = order.ResultDataSet
                .Err = err
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function

    End Class

End Namespace