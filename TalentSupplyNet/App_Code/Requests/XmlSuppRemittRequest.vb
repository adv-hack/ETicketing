Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Supplier Remittance Requests
'
'       Date                        Apr 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQSRA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal
    Public Class XmlSuppRemittRequest
        Inherits XmlRequest
        'Private _dec As DECreditNote

        'Public Property Dec() As DECreditNote
        '    Get
        '        Return _dec
        '    End Get
        '    Set(ByVal value As DECreditNote)
        '        _dec = value
        '    End Set
        'End Property

        Private _der As DeRemittances
        Public Property Der() As DeRemittances
            Get
                Return _der
            End Get
            Set(ByVal value As DeRemittances)
                _der = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            'Dim xmlAction As XmlCreditNoteResponse = CType(xmlResp, XmlCreditNoteResponse)
            Dim xmlAction As XmlSuppRemittResponse = CType(xmlResp, XmlSuppRemittResponse)
            ' Dim creditnoter As New TalentCreditNote()
            Dim remittRequest As New TalentRemittance
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
                'With creditnoter
                '    .Dep = Dec
                '    .Settings = Settings
                '    err = .GetCreditNote
                'End With
                With remittRequest
                    .Der = Der
                    .Settings = Settings
                    err = .SuppRemittances
                End With
                If err.HasError Then _
                    xmlResp.Err = err
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .ResultDataSet = remittRequest.ResultDataSet
                .Err = err
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
            Der = New DeRemittances
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//SupplierRemittanceRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Der.CollDETrans.Add(Extract_TransactionHeader(Node1))
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQSRA-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace
