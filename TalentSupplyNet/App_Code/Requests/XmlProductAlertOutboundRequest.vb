Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Product Alerts
'
'       Date                        8th Nov 2006
'
'       Author                      Andy White   
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQPA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlProductAlertOutboundRequest
        Inherits XmlRequest
        Private _dea As DEProductAlert

        Public Property Dea() As DEProductAlert
            Get
                Return _dea
            End Get
            Set(ByVal value As DEProductAlert)
                _dea = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlProductAlertOutboundResponse = CType(xmlResp, XmlProductAlertOutboundResponse)
            Dim pa As New TalentProductAlert
            Dim err As ErrorObj = Nothing
            '
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            If Not err.HasError Then
                With pa
                    .Dep = Dea
                    .Settings = Settings
                    err = .ProductAlertOutbound
                    ResultDataSet = .ResultDataSet
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .ResultDataSet = ResultDataSet
                .CreateResponse()
            End With

            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Dim Node1 As XmlNode
            Dea = New DEProductAlert
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//ProductAlertOutboundRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Dea.CollDETrans.Add(Extract_TransactionHeader(Node1))
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPA-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace