
Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Invoice Requests
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQIN- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlInvoiceRequest
        Inherits XmlRequest

        Private _din As DEInvoice
        Public Property Din() As DEInvoice
            Get
                Return _din
            End Get
            Set(ByVal value As DEInvoice)
                _din = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlInvoiceResponse = CType(xmlResp, XmlInvoiceResponse)
            Dim invoicing As New TalentInvoicing()
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
                With invoicing
                    .Dep = Din
                    .Settings = Settings
                    err = .GetInvoice
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .ResultDataSet = invoicing.ResultDataSet
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
            Din = New DEInvoice
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//InvoiceRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Din.CollDETrans.Add(Extract_TransactionHeader(Node1))
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQIN-09"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace