Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with  RMA Status Responses
'
'       Date                        15th Nov 2006
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSRM- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlRMAStatusResponse
        Inherits XmlResponse
        Private ndHeaderRoot, ndHeaderRootHeader As XmlNode
        Private atXmlNsXsi As XmlAttribute
        '
        Private ndHeader, ndBranchOrderNumber, ndCustomerPO, _
                ndOrderSuffix, ndOrderStatus, ndTotal, ndEntryDate, _
                ndInvoiceDate, ndRMAReason As XmlNode
        '
        Private atRMAReason As XmlAttribute

        Protected Overrides Sub InsertBodyV1()
            Dim dr As DataRow
            Dim dt As New DataTable
            '
            With MyBase.xmlDoc
                ndHeader = .CreateElement("RMAStatus")
                If Not Err.HasError Then
                    dt = ResultDataSet.Tables(0)
                    If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                        For Each dr In dt.Rows
                            '-----------------------------------------------------------------------------
                            Err2 = CreateHeader()
                            With dr
                                ndBranchOrderNumber.InnerText = .Item("BranchOrderNumber")
                                ndCustomerPO.InnerText = .Item("CustomerPO")
                                ndOrderSuffix.InnerText = .Item("OrderSuffix")
                                ndOrderStatus.InnerText = .Item("OrderStatus")
                                ndTotal.InnerText = .Item("Total")
                                ndEntryDate.InnerText = .Item("EntryDate")
                                ndInvoiceDate.InnerText = .Item("InvoiceDate")
                                ndRMAReason.InnerText = .Item("RMAReason")
                                atRMAReason.Value = .Item("Code")
                            End With
                            Err2 = AppendHeader()
                            '-----------------------------------------------------------------------------
                        Next
                    Else
                        '---------------------------------------------------------------------------------
                        '  No data so write dummy
                        '
                        Err2 = CreateHeader()
                        Err2 = AppendHeader()
                    End If
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
        End Sub

        Private Function CreateHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndHeader = .CreateElement("RMAStatus")
                    ndBranchOrderNumber = .CreateElement("BranchOrderNumber")
                    ndCustomerPO = .CreateElement("CustomerPO")
                    ndOrderSuffix = .CreateElement("OrderSuffix")
                    ndOrderStatus = .CreateElement("OrderStatus")
                    ndTotal = .CreateElement("Total")
                    ndEntryDate = .CreateElement("EntryDate")
                    ndInvoiceDate = .CreateElement("InvoiceDate")
                    ndRMAReason = .CreateElement("RMAReason")
                    atRMAReason = .CreateAttribute("Code")
                    ndRMAReason.Attributes.Append(atRMAReason)
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSRM-15"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With ndHeader
                    .AppendChild(ndBranchOrderNumber)
                    .AppendChild(ndCustomerPO)
                    .AppendChild(ndOrderSuffix)
                    .AppendChild(ndOrderStatus)
                    .AppendChild(ndTotal)
                    .AppendChild(ndEntryDate)
                    .AppendChild(ndInvoiceDate)
                    .AppendChild(ndRMAReason)
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Append Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSRM-16"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace