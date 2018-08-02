
Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Upload Template Requests
'
'       Date                        310707
'
'       Author                      Ben
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQUT- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlUploadTemplatesRequest
        Inherits XmlRequest

        Private _DEuploadTemplates As DEUploadTemplates
        Public Property DEUploadTemplates() As DEUploadTemplates
            Get
                Return _DEuploadTemplates
            End Get
            Set(ByVal value As DEUploadTemplates)
                _DEuploadTemplates = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlUploadTemplatesResponse = CType(xmlResp, XmlUploadTemplatesResponse)
            Dim template As New TalentTemplate()
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
                With template
                    .DEUploadTemplates = DEUploadTemplates
                    .Settings = Settings
                    err = .UploadTemplates
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .ResultDataSet = template.ResultDataSet
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
            DEUploadTemplates = New DEUploadTemplates
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//UploadTemplatesRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            DEUploadTemplates.CollDETrans.Add(Extract_TransactionHeader(Node1))
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQUT-09"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace