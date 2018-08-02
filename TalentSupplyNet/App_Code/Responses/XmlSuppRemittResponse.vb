Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Supplier Remittance responses
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

    Public Class XmlSuppRemittResponse
        Inherits XmlResponse

        Private ndDocRoot, ndDocHeaderRoot, ndErrorStatus As XmlNode
        Private ndEcrmInput, ndXmlLine As XmlNode
        Private ndSupplierRemittanceAdvice As XmlNode
        ' Define header nodes
        Private ndRemittanceAdvice, ndRemittanceAdviceHeader As XmlNode
        Private ndSupplierCode, ndPaymentRunNo, ndPaymentReference, ndPaymentMethod, ndDocumentDate, _
                ndChequeNo, ndRemittanceCurrCode, ndRemittanceValue, ndSupplierName, _
                ndSupplierAddress1, ndSupplierAddress2, ndSupplierAddress3, ndSupplierAddress4, _
                ndSupplierAddress5, ndSupplierPostcode, ndSupplierBankAccNo, ndSupplierBankName, _
                ndRemittanceLines As XmlNode
        ' Define detail nodes
        Private ndRemittanceLine, ndRemittanceAdviceDetail As XmlNode
        Private ndSupplierCodeDT, ndPaymentRunNoDT, ndPaymentReferenceDT, ndRemittanceLineNo, _
                ndMasterItemType, ndLedgerItemDocumentRefe, ndSuppliersRef, ndSOPOrderNo, ndPostingAmount, _
                ndDiscountAmount, ndVat As XmlNode

        Private atId, atXmlId, atTitle, atErrorNumber As XmlAttribute

        Protected Overrides Sub InsertBodyV1()
            '------------------------------------------------------------------------------
            '   Seperate the tables out of the ResultSet    
            '
            Dim iCounter As Integer = 0
            Try
                With MyBase.xmlDoc
                    ndSupplierRemittanceAdvice = .CreateElement("SupplierRemittanceAdvice")
                    '---------------------------------------------------------------------------------
                    ndErrorStatus = .CreateElement("ErrorStatus")
                    If Not Err2 Is Nothing Then
                        For iCounter = 1 To 500
                            If Not Err2.ItemErrorStatus(iCounter) = Nothing Then
                                ndErrorStatus.InnerText = Err2.ItemErrorStatus(iCounter)
                                atErrorNumber = .CreateAttribute("ErrorNumber")
                                atErrorNumber.Value = Err2.ItemErrorCode(iCounter)
                                ndErrorStatus.Attributes.Append(atErrorNumber)
                                ndSupplierRemittanceAdvice.AppendChild(ndErrorStatus)
                            Else
                                Exit For
                            End If
                        Next
                    End If
                    If Not Err Is Nothing Then
                        For iCounter = 1 To 500
                            If Not Err.ItemErrorStatus(iCounter) = Nothing Then
                                ndErrorStatus.InnerText = Err.ItemErrorStatus(iCounter)
                                atErrorNumber = .CreateAttribute("ErrorNumber")
                                atErrorNumber.Value = Err.ItemErrorCode(iCounter)
                                ndErrorStatus.Attributes.Append(atErrorNumber)
                                ndSupplierRemittanceAdvice.AppendChild(ndErrorStatus)
                            Else
                                Exit For
                            End If
                        Next
                    End If
                    '---------------------------------------------------------------------------------
                    If Not Err.HasError Then
                        dtHeader = ResultDataSet.Tables(0)      ' Header
                        dtDetail = ResultDataSet.Tables(1)      ' Item
                        Err2 = InsertHeader()
                    End If
                    '--------------------------------------------------------------------------------------
                    '   Insert the fragment into the XML document
                    '
                    Const c1 As String = "//"                               ' Constants are faster at run time
                    Const c2 As String = "/TransactionHeader"
                    '
                    ndDocRoot = .SelectSingleNode(c1 & RootElement())
                    ndDocHeaderRoot = .SelectSingleNode(c1 & RootElement() & c2)
                    ndDocRoot.InsertAfter(ndSupplierRemittanceAdvice, ndDocHeaderRoot)
                    'Insert the XSD reference & namespace as an attribute within the root node
                    Dim atXmlNsXsi As XmlAttribute = CreateNamespaceAttribute()
                    ndDocRoot.Attributes.Append(atXmlNsXsi)
                End With

            Catch ex As Exception
            End Try

        End Sub
        Private Function InsertHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '-------------------------------------------------------------------------------------
            Dim dr As DataRow
            Dim sWork As String = String.Empty
            Try
                '---------------------------------------------------------------------------------
                If Not dtHeader Is Nothing AndAlso dtHeader.Rows.Count > 0 Then
                    For Each dr In dtHeader.Rows
                        err = CreateHeader()
                        Dim supplierCode As String = dr("SupplierCode")
                        Dim payRef As String = dr("PaymentReference")
                        ' Set values
                        ndSupplierCode.InnerText = supplierCode
                        ndPaymentRunNo.InnerText = dr("PaymentRunNo")
                        ndPaymentReference.InnerText = payRef
                        ndPaymentMethod.InnerText = dr("PaymentMethod")
                        ndDocumentDate.InnerText = dr("DocumentDate")
                        ndChequeNo.InnerText = dr("ChequeNo")
                        ndRemittanceCurrCode.InnerText = dr("RemittanceCurrCode")
                        ndRemittanceValue.InnerText = dr("RemittanceValue")
                        ndSupplierName.InnerText = dr("SupplierName")
                        ndSupplierAddress1.InnerText = dr("SupplierAddress1")
                        ndSupplierAddress2.InnerText = dr("SupplierAddress2")
                        ndSupplierAddress3.InnerText = dr("SupplierAddress3")
                        ndSupplierAddress4.InnerText = dr("SupplierAddress4")
                        ndSupplierAddress5.InnerText = dr("SupplierAddress5")
                        ndSupplierPostcode.InnerText = dr("SupplierPostcode")
                        ndSupplierBankAccNo.InnerText = dr("SupplierBankAccNo")
                        ndSupplierBankName.InnerText = dr("SupplierBankName")
                        ndRemittanceLines.InnerText = dr("RemittanceLines")


                        err = InsertDetails(supplierCode, payRef)
                        err = AppendHeader()
                    Next
                Else
                    err = CreateHeader()
                    err = AppendHeader()
                End If
                'ndecrminputs.AppendChild(ndecrminput)
                '---------------------------------------------------------------------------
            Catch ex As Exception
                Const strError As String = "Failed to Insert xml Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQSRA-11"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndRemittanceAdvice = .CreateElement("RemittanceAdvice")
                    ndRemittanceAdviceHeader = .CreateElement("RemittanceAdviceHeader")
                    ndSupplierCode = .CreateElement("SupplierCode")
                    ndPaymentRunNo = .CreateElement("PaymentRunNo")
                    ndPaymentReference = .CreateElement("PaymentReference")
                    ndPaymentMethod = .CreateElement("PaymentMethod")
                    ndDocumentDate = .CreateElement("DocumentDate")
                    ndChequeNo = .CreateElement("ChequeNo")
                    ndRemittanceCurrCode = .CreateElement("RemittanceCurrCode")
                    ndRemittanceValue = .CreateElement("RemittanceValue")
                    ndSupplierName = .CreateElement("SupplierName")
                    ndSupplierAddress1 = .CreateElement("SupplierAddress1")
                    ndSupplierAddress2 = .CreateElement("SupplierAddress2")
                    ndSupplierAddress3 = .CreateElement("SupplierAddress3")
                    ndSupplierAddress4 = .CreateElement("SupplierAddress4")
                    ndSupplierAddress5 = .CreateElement("SupplierAddress5")
                    ndSupplierPostcode = .CreateElement("SupplierPostcode")
                    ndSupplierBankAccNo = .CreateElement("SupplierBankAccNo")
                    ndSupplierBankName = .CreateElement("SupplierBankName")
                    ndRemittanceLines = .CreateElement("RemittanceLines")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQSRA-15"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With ndRemittanceAdviceHeader
                    .AppendChild(ndSupplierCode)
                    .AppendChild(ndPaymentRunNo)
                    .AppendChild(ndPaymentReference)
                    .AppendChild(ndPaymentMethod)
                    .AppendChild(ndDocumentDate)
                    .AppendChild(ndChequeNo)
                    .AppendChild(ndRemittanceCurrCode)
                    .AppendChild(ndRemittanceValue)
                    .AppendChild(ndSupplierName)
                    .AppendChild(ndSupplierAddress1)
                    .AppendChild(ndSupplierAddress2)
                    .AppendChild(ndSupplierAddress3)
                    .AppendChild(ndSupplierAddress4)
                    .AppendChild(ndSupplierAddress5)
                    .AppendChild(ndSupplierPostcode)
                    .AppendChild(ndSupplierBankAccNo)
                    .AppendChild(ndSupplierBankName)
                    .AppendChild(ndRemittanceLines)
                End With
                ndRemittanceAdvice.AppendChild(ndRemittanceAdviceHeader)
                ndRemittanceAdvice.AppendChild(ndRemittanceAdviceDetail)
                ndSupplierRemittanceAdvice.AppendChild(ndRemittanceAdvice)
            Catch ex As Exception
                Const strError As String = "Failed to Append Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQSRA-16"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertDetails(ByVal supplierCode As String, ByVal payRef As String) As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                Dim dr As DataRow
                '--------------------------------------------------------------------------
                '   Add details lines from table 2
                '   
                ndRemittanceAdviceDetail = MyBase.xmlDoc.CreateElement("RemittanceAdviceDetail")
                If Not dtDetail Is Nothing AndAlso dtDetail.Rows.Count > 0 Then
                    For Each dr In dtDetail.Rows
                        If dr("SupplierCode").Equals(supplierCode) AndAlso dr("PaymentReference").Equals(payRef) Then
                            err = CreateDetail()

                            ndRemittanceLineNo.InnerText = dr("RemittanceLineNo")
                            ndMasterItemType.InnerText = dr("MasterItemType")
                            ndLedgerItemDocumentRefe.InnerText = dr("LedgerItemDocumentRefe")
                            ndSuppliersRef.InnerText = dr("SuppliersRef")
                            ndSOPOrderNo.InnerText = dr("SOPOrderNo")
                            ndPostingAmount.InnerText = dr("PostingAmount")
                            ndDiscountAmount.InnerText = dr("DiscountAmount")
                            ndVat.InnerText = dr("VAT")

                            err = AppendDetail()
                        End If
                    Next dr
                Else
                    '--------------------------------------------------------------------------
                    '   No details, write dummy
                    ' 
                    err = CreateDetail()
                    err = AppendDetail()
                    '
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert Comment Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQSRA-17"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateDetail() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc

                    ndRemittanceLine = .CreateElement("RemittanceLine")
                    ndRemittanceLineNo = .CreateElement("RemittanceLineNo")
                    ndMasterItemType = .CreateElement("MasterItemType")
                    ndLedgerItemDocumentRefe = .CreateElement("LedgerItemDocumentRef")
                    ndSuppliersRef = .CreateElement("SuppliersRef")
                    ndSOPOrderNo = .CreateElement("SOPOrderNo")
                    ndPostingAmount = .CreateElement("PostingAmount")
                    ndDiscountAmount = .CreateElement("DiscountAmount")
                    ndVat = .CreateElement("VAT")

                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQSRA-18"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendDetail() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With ndRemittanceLine
                    .AppendChild(ndRemittanceLineNo)
                    .AppendChild(ndMasterItemType)
                    .AppendChild(ndLedgerItemDocumentRefe)
                    .AppendChild(ndSuppliersRef)
                    .AppendChild(ndSOPOrderNo)
                    .AppendChild(ndPostingAmount)
                    .AppendChild(ndDiscountAmount)
                    .AppendChild(ndVat)
                End With

                ndRemittanceAdviceDetail.AppendChild(ndRemittanceLine)
            Catch ex As Exception
                Const strError As String = "Failed to append detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQSRA-19"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace
