Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Supplier Invoice Accepted Requests
'
'       Date                        Mar 2007
'
'       Author                      Andy White 
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSSAI- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlSupplierInvoiceAcceptedRequest
        Inherits XmlRequest
        Private _din As DESupplier
        Public Property Din() As DESupplier
            Get
                Return _din
            End Get
            Set(ByVal value As DESupplier)
                _din = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlSupplierInvoiceResponse = CType(xmlResp, XmlSupplierInvoiceResponse)
            Dim Invoicing As New TalentSupplier()
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
                    err = .AcceptInvoice
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .ResultDataSet = Invoicing.ResultDataSet
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1 As XmlNode
            Dim detr As New DETransaction           ' Items
            Din = New DESupplier
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//SupplierInvoiceAcceptedRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Din.CollDETrans.Add(Extract_TransactionHeader(Node1))

                    End Select
                Next
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRSSAI-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace