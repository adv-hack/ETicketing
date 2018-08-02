Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Invoice responses
'
'       Date                        04/01/06
'
'       Author                      Ben Ford
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSIS- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlSupplierInvoiceAcceptedResponse
        Inherits XmlResponse
        '--------------------------------------------------------------------------------------
        Private ndHeaderRoot, ndHeaderRootHeader As XmlNode
        Private atXmlNsXsi As XmlAttribute
        '
        Private ndHeader, ndInvoiceInfo, ndInvoiceNumber, ndInvoiceStatus As XmlNode

        Private iCounter As Integer = 0

        Protected Overrides Sub InsertBodyV1()
            '--------------------------------------------------------------------------------------
            '   Seperate the tables out of the ResultSet  
            ' 
            '
            Try
                With MyBase.xmlDoc
                    ndHeader = .CreateElement("SupplierInvoiceAccepted")
                    If Not Err.HasError Then
                        dtHeader = ResultDataSet.Tables(0)
                        dtDetail = ResultDataSet.Tables(1)
                        Err2 = InsertDetails()
                    End If
                    '--------------------------------------------------------------------------------------
                    '   Insert the fragment into the XML document
                    '
                    Const c1 As String = "//"                               ' Constants are faster at run time
                    Const c2 As String = "/TransactionHeader"
                    '
                    ndHeaderRoot = .SelectSingleNode(c1 & RootElement())
                    ndHeaderRootHeader = .SelectSingleNode(c1 & RootElement() & c2)
                    ndHeaderRoot.InsertAfter(ndHeader, ndHeaderRootHeader)

                    'Insert the XSD reference & namespace as an attribute within the root node
                    atXmlNsXsi = CreateNamespaceAttribute()
                    ndHeaderRoot.Attributes.Append(atXmlNsXsi)
                End With
            Catch ex As Exception
            End Try
        End Sub

        Private Function InsertDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '---------------------------------------------------------------------------------------------
            Dim dc As DataRow
            Try
                '--------------------------------------------------------------
                If Not dtDetail Is Nothing AndAlso dtDetail.Rows.Count > 0 Then
                    For Each dc In dtDetail.Rows
                        '----------------------------------------------------------
                        err = CreateDetails()
                        ndInvoiceNumber.InnerText = dc("InvoiceNumber")
                        ndInvoiceStatus.InnerText = dc("InvoiceStatus")
                        err = AppendDetails()
                        '------------------------------------------------------
                    Next dc
                Else
                    err = CreateDetails()
                    err = AppendDetails()
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert Invoice Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSIS-02"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndInvoiceInfo = .CreateElement("InvoiceInfo")
                    ndInvoiceNumber = .CreateElement("InvoiceNumber")
                    ndInvoiceStatus = .CreateElement("InvoiceStatus")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Invoice Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSIS-11"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                ndHeader.AppendChild(ndInvoiceInfo)
                With ndInvoiceInfo
                    .AppendChild(ndInvoiceNumber)
                    .AppendChild(ndInvoiceStatus)
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Append Invoice Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSIS-12"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace