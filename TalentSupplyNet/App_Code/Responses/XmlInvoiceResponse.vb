Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.Data
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Invoice responses
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSIN- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlInvoiceResponse
        Inherits XmlResponse

        Private ndInvoices, ndInvoice, ndInvoiceHeader, ndInvoiceType, ndInvoiceNumber, _
                ndInvoiceDate, ndCustomerPO, ndOriginalOrderNumber, ndOriginalOrderDate, ndIMAccountNumber As XmlNode

        Private ndCAddress, ndCName, ndCAttention, ndCAddress1, ndCAddress2, ndCAddress3, _
                ndCAddress4, ndCCity, ndCProvince, ndCPostalCode, ndCCountryCode As XmlNode

        Private ndTAddress, ndTName, ndTAttention, ndTAddress1, ndTAddress2, ndTAddress3, _
                ndTAddress4, ndTCity, ndTProvince, ndTPostalCode, ndTCountryCode As XmlNode

        Private ndFAddress, ndFName, ndFAttention, ndFAddress1, ndFAddress2, ndFAddress3, _
                ndFAddress4, ndFCity, ndFProvince, ndFPostalCode, ndFCountryCode As XmlNode

        Private ndDocRoot, ndDocHeaderRoot, ndVAT, ndVATNumber, _
                ndCurrency, ndCurrencyCode, ndPaymentTerms, ndTermsCode, ndPaymentDue, _
                ndPaymentDueDate, ndTotalInformationTotalQuantity, ndTotalInformationOrderLines, _
                ndTotalFinancialTotalAmountDue, ndTotalFinancialTotalLineItemAmount, _
                ndTotalFinancialTotalTaxableAmount, ndTotalFinancialTotalCharges, _
                ndTotalFinancialTotalTaxAmount, ndFinalFinalTaxableAmount, ndFinalFinalTaxAmount, _
                ndLineItemHeader, ndLineItem, ndProduct, ndCustomerSKU, ndEANCode, ndManufacturerSKU, _
                ndSKUDescription1, ndSKUDescription2, ndSerialNumberHeader, ndSerialNumber, ndTAX, _
                ndLineMonetaryInfo, ndUnitPrice, ndLineItemAmount, ndShipToSuffix, ndDispatchSequence, _
                ndComments, ndCommentLine, ndCommentText, ndCommentLineNumber As XmlNode

        Private atCAddressType, atTAddressType, atFAddressType, atTotalQuantity, atOrderLines, atTotalAmountDue, _
                atTotalLineItemAmount, atTotalTaxableAmount, atTotalCharges, atTotalTaxAmount, atFinalTaxableAmount, _
                atFinalTaxAmount, atLineNumber, atSKU, atInvoicedQuantity, atType As XmlAttribute
        Private qtyCount As Integer = 0
        Private linesCount As Integer = 0
        Private linesAmountCount As Double = 0.0
        Protected Overrides Sub InsertBodyV1()
            '------------------------------------------------------------------------------
            '   Seperate the tables out of the ResultSet    
            '
            Try
                With MyBase.xmlDoc
                    ndInvoices = .CreateElement("Invoices")
                    If Not Err.HasError Then
                        dtHeader = ResultDataSet.Tables(0)      ' Header
                        dtDetail = ResultDataSet.Tables(1)      ' Item
                        dtText = ResultDataSet.Tables(2)        ' Original order comments
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
                    ndDocRoot.InsertAfter(ndInvoices, ndDocHeaderRoot)
                    'Insert the XSD reference & namespace as an attribute within the root node
                    Dim atXmlNsXsi As XmlAttribute = CreateNamespaceAttribute()
                    ndDocRoot.Attributes.Append(atXmlNsXsi)
                End With

            Catch ex As Exception
            End Try

        End Sub

        Private Function InsertHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Dim dRow As DataRow
            Try
                If Not dtHeader Is Nothing AndAlso dtHeader.Rows.Count > 0 Then
                    For Each dRow In dtHeader.Rows
                        '--------------------------------------------------------------------------
                        err = CreateHeader()
                        '
                        ndInvoiceType.InnerText = String.Empty
                        '---------------------------------------------------------------------------
                        Dim invoiceNumber As String = dRow("InvoiceNumber")
                        ndInvoiceNumber.InnerText = dRow("InvoiceNumber")
                        '---------------------------------------------------------------------------
                        Dim d1 As Date = dRow("InvoiceDateTime")
                        ndInvoiceDate.InnerText = d1.ToString("yyyyMMdd")
                        'ndCustomerPO.InnerText = dRow("OrderNumber")
                        ndCustomerPO.InnerText = dRow("CustomerPO")
                        ndOriginalOrderNumber.InnerText = dRow("OriginalOrderNo")

                        Dim d2 As Date = dRow("OriginalOrderDate")
                        ndOriginalOrderDate.InnerText = d2.ToString("yyyyMMdd")

                        ndShipToSuffix.InnerText = dRow("CustomerCompanyCode")
                        ndDispatchSequence.InnerText = dRow("DispatchSequence")
                        err = InsertComment(dRow("InvoiceNumber"))
                        '---------------------------------------------------------------------------
                        ndIMAccountNumber.InnerText = dRow("AccountNumber")
                        atCAddressType.Value = "Customer"
                        ndCName.InnerText = dRow("CustomerName")
                        ndCAttention.InnerText = dRow("CustomerAttention")
                        ndCAddress1.InnerText = dRow("CustomerAddress1")
                        ndCAddress2.InnerText = dRow("CustomerAddress2")
                        ndCAddress3.InnerText = dRow("CustomerAddress3")
                        ndCAddress4.InnerText = dRow("CustomerAddress4")
                        ndCCity.InnerText = dRow("CustomerAddress5")
                        ''ndCProvince.InnerText = dRow("CustomerProvince")
                        ndCPostalCode.InnerText = dRow("CustomerAddress6")
                        ndCCountryCode.InnerText = dRow("CustomerAddress7")
                        '---------------------------------------------------------------------------
                        atTAddressType.Value = "ShipTo"
                        ndTName.InnerText = dRow("ShipToName")
                        ndTAttention.InnerText = dRow("ShipToAttention")
                        ndTAddress1.InnerText = dRow("ShipToAddress1")
                        ndTAddress2.InnerText = dRow("ShipToAddress2")
                        ndTAddress3.InnerText = dRow("ShipToAddress3")
                        ndTAddress4.InnerText = dRow("ShipToAddress4")
                        ndTCity.InnerText = dRow("ShipToAddress5")
                        ''ndTProvince.InnerText = dRow("ShipToProvince")
                        ndTPostalCode.InnerText = dRow("ShipToAddress6")
                        ndTCountryCode.InnerText = dRow("ShipToAddress7")
                        '---------------------------------------------------------------------------
                        atFAddressType.Value = "ShipFrom"
                        ndFName.InnerText = dRow("ShipFromName")
                        ndFAttention.InnerText = dRow("ShipFromAttention")
                        ndFAddress1.InnerText = dRow("ShipFromAddress1")
                        ndFAddress2.InnerText = dRow("ShipFromAddress2")
                        ndFAddress3.InnerText = dRow("ShipFromAddress3")
                        ndFAddress4.InnerText = dRow("ShipFromAddress4")
                        ndFCity.InnerText = dRow("ShipFromAddress5")
                        ''ndFProvince.InnerText = dRow("ShipFromProvince")
                        ndFPostalCode.InnerText = dRow("ShipFromAddress6")
                        ndFCountryCode.InnerText = dRow("ShipFromAddress7")
                        '---------------------------------------------------------------------------
                        ndVATNumber.InnerText = dRow("VATNumber")
                        ndCurrencyCode.InnerText = dRow("CurrencyCode")
                        ndTermsCode.InnerText = 0
                        ndPaymentDueDate.InnerText = Now.ToString("yyyyMMdd")
                        '---------------------------------------------------------------------------
                        err = InsertDetails(invoiceNumber)
                        '---------------------------------------------------------------------------
                        atTotalQuantity.Value = "TotalQuantity"
                        atOrderLines.Value = "OrderLines"
                        atTotalAmountDue.Value = "TotalAmountDue"
                        atTotalLineItemAmount.Value = "TotalLineItemAmount"
                        atTotalTaxableAmount.Value = "TotalTaxableAmount"
                        atTotalCharges.Value = "TotalCharges"
                        atTotalTaxAmount.Value = "TotalTaxAmount"
                        atTotalTaxAmount.Value = "FinalTaxableAmount"
                        atFinalTaxAmount.Value = "FinalTaxAmount"
                        '---------------------------------------------------------------------------
                        ndTotalInformationTotalQuantity.InnerText = qtyCount
                        ndTotalInformationOrderLines.InnerText = linesCount
                        Dim totalAmountDue As Decimal = 0
                        totalAmountDue = CDec(dRow("invoiceAmount")) + CDec(dRow("VatAmount"))
                        'ndTotalFinancialTotalAmountDue.InnerText = dRow("InvoiceAmount")
                        ndTotalFinancialTotalAmountDue.InnerText = totalAmountDue.ToString
                        ndTotalFinancialTotalLineItemAmount.InnerText = linesAmountCount
                        ndTotalFinancialTotalTaxableAmount.InnerText = dRow("InvoiceAmount")
                        ndTotalFinancialTotalCharges.InnerText = dRow("ChargesAmount").ToString
                        ndTotalFinancialTotalTaxAmount.InnerText = dRow("VatAmount")
                        ' ndFinalFinalTaxableAmount.InnerText = String.Empty
                        ndFinalFinalTaxableAmount.InnerText = dRow("InvoiceAmount")
                        ndFinalFinalTaxAmount.InnerText = dRow("VatAmount")
                        '---------------------------------------------------------------------------
                        linesCount = 0
                        qtyCount = 0
                        linesAmountCount = 0
                        err = AppendHeader()
                    Next
                Else
                    '------------------------------------------------------------------------------
                    '   No records - Create dummy
                    '
                    err = CreateHeader()
                    err = Create_Comment()
                    err = CreateDetails()
                    err = AppendComment()
                    err = AppendHeader()
                    err = AppendDetails()
                    '------------------------------------------------------------------------------
                    ndInvoices.AppendChild(ndInvoice)
                End If
                '---------------------------------------------------------------------------
            Catch ex As Exception
                Const strError As String = "Failed to Insert Invoice Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSIN-11"
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
                    ndInvoice = .CreateElement("Invoice")
                    ndInvoiceHeader = .CreateElement("InvoiceHeader")
                    ndInvoiceType = .CreateElement("InvoiceType")
                    ndInvoiceNumber = .CreateElement("InvoiceNumber")
                    ndInvoiceDate = .CreateElement("InvoiceDate")
                    ndCustomerPO = .CreateElement("CustomerPO")
                    ndOriginalOrderNumber = .CreateElement("OriginalOrderNumber")
                    ndOriginalOrderDate = .CreateElement("OriginalOrderDate")
                    ndShipToSuffix = .CreateElement("ShipToSuffix")
                    ndDispatchSequence = .CreateElement("DispatchSequence")
                    ndComments = .CreateElement("Comments")
                    ndIMAccountNumber = .CreateElement("AccountNumber")
                    ndCAddress = .CreateElement("Address")
                    '---------------------------------------------------------------------------
                    atCAddressType = .CreateAttribute("Type")
                    ndCName = .CreateElement("Name")
                    ndCAttention = .CreateElement("Attention")
                    ndCAddress1 = .CreateElement("AddressLine1")
                    ndCAddress2 = .CreateElement("AddressLine2")
                    ndCAddress3 = .CreateElement("AddressLine3")
                    ndCAddress4 = .CreateElement("AddressLine4")
                    ndCCity = .CreateElement("City")
                    ndCProvince = .CreateElement("Province")
                    ndCPostalCode = .CreateElement("PostalCode")
                    ndCCountryCode = .CreateElement("CountryCode")
                    '---------------------------------------------------------------------------
                    ndTAddress = .CreateElement("Address")
                    atTAddressType = .CreateAttribute("Type")
                    ndTName = .CreateElement("Name")
                    ndTAttention = .CreateElement("Attention")
                    ndTAddress1 = .CreateElement("AddressLine1")
                    ndTAddress2 = .CreateElement("AddressLine2")
                    ndTAddress3 = .CreateElement("AddressLine3")
                    ndTAddress4 = .CreateElement("AddressLine4")
                    ndTCity = .CreateElement("City")
                    ndTProvince = .CreateElement("Province")
                    ndTPostalCode = .CreateElement("PostalCode")
                    ndTCountryCode = .CreateElement("CountryCode")
                    '---------------------------------------------------------------------------
                    ndFAddress = .CreateElement("Address")
                    atFAddressType = .CreateAttribute("Type")
                    ndFName = .CreateElement("Name")
                    ndFAttention = .CreateElement("Attention")
                    ndFAddress1 = .CreateElement("AddressLine1")
                    ndFAddress2 = .CreateElement("AddressLine2")
                    ndFAddress3 = .CreateElement("AddressLine3")
                    ndFAddress4 = .CreateElement("AddressLine4")
                    ndFCity = .CreateElement("City")
                    ndFProvince = .CreateElement("Province")
                    ndFPostalCode = .CreateElement("PostalCode")
                    ndFCountryCode = .CreateElement("CountryCode")
                    '---------------------------------------------------------------------------
                    ndVAT = .CreateElement("VAT")
                    ndVATNumber = .CreateElement("VATNumber")
                    ndCurrency = .CreateElement("Currency")
                    ndCurrencyCode = .CreateElement("CurrencyCode")
                    ndPaymentTerms = .CreateElement("PaymentTerms")
                    ndTermsCode = .CreateElement("TermsCode")
                    ndPaymentDue = .CreateElement("PaymentDue")
                    ndPaymentDueDate = .CreateElement("PaymentDueDate")
                    '---------------------------------------------------------------------------
                    ndLineItemHeader = .CreateElement("LineItemHeader")
                    '---------------------------------------------------------------------------
                    ndTotalInformationTotalQuantity = .CreateElement("TotalInformation")
                    atTotalQuantity = .CreateAttribute("Type")
                    ndTotalInformationOrderLines = .CreateElement("TotalInformation")
                    atOrderLines = .CreateAttribute("Type")
                    ndTotalFinancialTotalAmountDue = .CreateElement("TotalFinancial")
                    atTotalAmountDue = .CreateAttribute("Type")
                    ndTotalFinancialTotalLineItemAmount = .CreateElement("TotalFinancial")
                    atTotalLineItemAmount = .CreateAttribute("Type")
                    ndTotalFinancialTotalTaxableAmount = .CreateElement("TotalFinancial")
                    atTotalTaxableAmount = .CreateAttribute("Type")
                    ndTotalFinancialTotalCharges = .CreateElement("TotalFinancial")
                    atTotalCharges = .CreateAttribute("Type")
                    ndTotalFinancialTotalTaxAmount = .CreateElement("TotalFinancial")
                    atTotalTaxAmount = .CreateAttribute("Type")
                    ndFinalFinalTaxableAmount = .CreateElement("Final")
                    atFinalTaxableAmount = .CreateAttribute("Type")
                    ndFinalFinalTaxAmount = .CreateElement("Final")
                    atFinalTaxAmount = .CreateAttribute("Type")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create  Invoice Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSIN-12"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With ndInvoiceHeader
                    .AppendChild(ndInvoiceType)
                    .AppendChild(ndInvoiceNumber)
                    .AppendChild(ndInvoiceDate)
                    .AppendChild(ndCustomerPO)
                    .AppendChild(ndOriginalOrderNumber)
                    .AppendChild(ndOriginalOrderDate)
                    .AppendChild(ndShipToSuffix)
                    .AppendChild(ndDispatchSequence)
                End With
                '---------------------------------------------------------------------------
                With ndCAddress
                    .Attributes.Append(atCAddressType)
                    .AppendChild(ndCName)
                    .AppendChild(ndCAttention)
                    .AppendChild(ndCAddress1)
                    .AppendChild(ndCAddress2)
                    .AppendChild(ndCAddress3)
                    .AppendChild(ndCAddress4)
                    .AppendChild(ndCCity)
                    ''.AppendChild(ndCProvince)
                    .AppendChild(ndCPostalCode)
                    .AppendChild(ndCCountryCode)
                End With
                '---------------------------------------------------------------------------
                With ndTAddress
                    .Attributes.Append(atTAddressType)
                    .AppendChild(ndTName)
                    .AppendChild(ndTAttention)
                    .AppendChild(ndTAddress1)
                    .AppendChild(ndTAddress2)
                    .AppendChild(ndTAddress3)
                    .AppendChild(ndTAddress4)
                    .AppendChild(ndTCity)
                    ''.AppendChild(ndTProvince)
                    .AppendChild(ndTPostalCode)
                    .AppendChild(ndTCountryCode)
                End With
                '---------------------------------------------------------------------------
                With ndFAddress
                    .Attributes.Append(atFAddressType)
                    .AppendChild(ndFName)
                    .AppendChild(ndFAttention)
                    .AppendChild(ndFAddress1)
                    .AppendChild(ndFAddress2)
                    .AppendChild(ndFAddress3)
                    .AppendChild(ndFAddress4)
                    .AppendChild(ndFCity)
                    ''.AppendChild(ndTProvince)
                    .AppendChild(ndFPostalCode)
                    .AppendChild(ndFCountryCode)
                End With
                '---------------------------------------------------------------------------
                ndVAT.AppendChild(ndVATNumber)
                ndCurrency.AppendChild(ndCurrencyCode)
                ndPaymentTerms.AppendChild(ndTermsCode)
                ndPaymentDue.AppendChild(ndPaymentDueDate)
                '---------------------------------------------------------------------------
                ndTotalInformationTotalQuantity.Attributes.Append(atTotalQuantity)
                ndTotalInformationOrderLines.Attributes.Append(atOrderLines)
                ndTotalFinancialTotalAmountDue.Attributes.Append(atTotalAmountDue)
                ndTotalFinancialTotalLineItemAmount.Attributes.Append(atTotalLineItemAmount)
                ndTotalFinancialTotalTaxableAmount.Attributes.Append(atTotalTaxableAmount)
                ndTotalFinancialTotalCharges.Attributes.Append(atTotalCharges)
                ndTotalFinancialTotalTaxAmount.Attributes.Append(atTotalTaxAmount)
                ndFinalFinalTaxableAmount.Attributes.Append(atTotalTaxAmount)
                ndFinalFinalTaxAmount.Attributes.Append(atFinalTaxAmount)
                '---------------------------------------------------------------------------
                With ndInvoice
                    .AppendChild(ndInvoiceHeader)
                    .AppendChild(ndComments)
                    .AppendChild(ndIMAccountNumber)
                    .AppendChild(ndCAddress)
                    .AppendChild(ndTAddress)
                    .AppendChild(ndFAddress)
                    .AppendChild(ndVAT)
                    .AppendChild(ndCurrency)
                    .AppendChild(ndPaymentTerms)
                    .AppendChild(ndPaymentDue)
                    .AppendChild(ndLineItemHeader)
                    .AppendChild(ndTotalInformationTotalQuantity)
                    .AppendChild(ndTotalInformationOrderLines)
                    .AppendChild(ndTotalFinancialTotalAmountDue)
                    .AppendChild(ndTotalFinancialTotalLineItemAmount)
                    .AppendChild(ndTotalFinancialTotalTaxableAmount)
                    .AppendChild(ndTotalFinancialTotalCharges)
                    .AppendChild(ndTotalFinancialTotalTaxAmount)
                    .AppendChild(ndFinalFinalTaxableAmount)
                    .AppendChild(ndFinalFinalTaxAmount)
                End With
                '---------------------------------------------------------------------------
                ndInvoices.AppendChild(ndInvoice)
                '---------------------------------------------------------------------------
            Catch ex As Exception
                Const strError As String = "Failed to Append  Invoice Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSIN-14"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertComment(ByVal invoiceNumber As String) As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                Dim dr As DataRow
                '--------------------------------------------------------------------------
                '   Add comment lines from table 3
                '   
                If Not dtText Is Nothing AndAlso dtText.Rows.Count > 0 Then
                    For Each dr In dtText.Rows
                        If dr("InvoiceNumber").Equals(invoiceNumber) Then
                            err = Create_Comment()
                            ndCommentText.InnerText = dr("Text")
                            ndCommentLineNumber.InnerText = dr("TextLineNumber")
                            err = AppendComment()
                        End If
                    Next dr
                Else
                    '--------------------------------------------------------------------------
                    '   No comments, write dummy
                    ' 
                    err = Create_Comment()
                    err = AppendComment()
                    '
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert Comment Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSIN-15"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function Create_Comment() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndCommentLine = .CreateElement("CommentLine")
                    ndCommentText = .CreateElement("CommentText")
                    ndCommentLineNumber = .CreateElement("CommentLineNumber")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Comment Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSIN-16"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendComment() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                ndComments.AppendChild(ndCommentLine)
                With ndCommentLine
                    .AppendChild(ndCommentText)
                    .AppendChild(ndCommentLineNumber)
                End With
            Catch ex As Exception
                Const strError As String = "Failed to append comment Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSIN-17"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertDetails(ByVal InvoiceNumber As String) As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                Dim dr As DataRow
                If Not dtDetail Is Nothing AndAlso dtDetail.Rows.Count > 0 Then
                    For Each dr In dtDetail.Rows
                        If dr("InvoiceNumber").ToString.Trim = InvoiceNumber.Trim Then
                            '-----------------------------------------------------------
                            err = CreateDetails()
                            atLineNumber.Value = dr("LineNumber")
                            atSKU.Value = dr("ProductCode")
                            atInvoicedQuantity.Value = dr("QuantityInvoiced")
                            '------------------------------------------------------------
                            ndCustomerSKU.InnerText = dr("CustomerSKU")
                            ndEANCode.InnerText = dr("EANCode")
                            ndManufacturerSKU.InnerText = String.Empty
                            ndSKUDescription1.InnerText = dr("Description1")
                            ndSKUDescription2.InnerText = dr("Description2")
                            '------------------------------------------------------------
                            ndSerialNumber.InnerText = String.Empty
                            atType.Value = "VAT"
                            ndTAX.InnerText = dr("InvoiceLineVatAmount")
                            ndUnitPrice.InnerText = dr("ProductPrice")
                            '-----------------------------------------------------------
                            Dim d1 As Decimal = dr("InvoiceLineNetAmount")
                            Dim d2 As Decimal = dr("InvoiceLineVatAmount")
                            ' Line item amount should not include VAT
                            ndLineItemAmount.InnerText = dr("InvoiceLineNetAmount")
                            'ndLineItemAmount.InnerText = dr("InvoiceLineNetAmount") + dr("InvoiceLineVatAmount")
                            qtyCount += CInt(dr("QuantityInvoiced"))
                            linesCount += 1
                            linesAmountCount += CDec(ndLineItemAmount.InnerText)
                            err = AppendDetails()
                        End If
                    Next
                Else
                    '---------------------------------------------------------------------------
                    '   No Items so create Dummy
                    '
                    err = CreateDetails()
                    err = AppendDetails()
                    '
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert Invoice Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSIN-18"
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
                    ndLineItem = .CreateElement("LineItem")
                    atLineNumber = .CreateAttribute("LineNumber")
                    ndProduct = .CreateElement("Product")
                    atSKU = .CreateAttribute("SKU")
                    atInvoicedQuantity = .CreateAttribute("InvoicedQuantity")
                    '-----------------------------------------------------------------
                    ndCustomerSKU = .CreateElement("CustomerSKU")
                    ndEANCode = .CreateElement("EANCode")
                    ndManufacturerSKU = .CreateElement("ManufacturerSKU")
                    ndSKUDescription1 = .CreateElement("SKUDescription1")
                    ndSKUDescription2 = .CreateElement("SKUDescription2")
                    '-----------------------------------------------------------------
                    ndSerialNumberHeader = .CreateElement("SerialNumberHeader")
                    ndSerialNumber = .CreateElement("SerialNumber")
                    ndTAX = .CreateElement("Tax")
                    atType = .CreateAttribute("Type")
                    ndLineMonetaryInfo = .CreateElement("LineMonetaryInfo")
                    ndUnitPrice = .CreateElement("UnitPrice")
                    ndLineItemAmount = .CreateElement("LineItemAmount")
                    '-----------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Invoice Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSIN-19"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With ndProduct
                    .Attributes.Append(atSKU)
                    .Attributes.Append(atInvoicedQuantity)
                    .AppendChild(ndCustomerSKU)
                    .AppendChild(ndEANCode)
                    .AppendChild(ndManufacturerSKU)
                    .AppendChild(ndSKUDescription1)
                    .AppendChild(ndSKUDescription2)
                End With
                '-----------------------------------------------------------------
                ndSerialNumberHeader.AppendChild(ndSerialNumber)
                ndTAX.Attributes.Append(atType)
                With ndLineMonetaryInfo
                    .AppendChild(ndUnitPrice)
                    .AppendChild(ndLineItemAmount)
                End With
                '-----------------------------------------------------------------
                With ndLineItem
                    .Attributes.Append(atLineNumber)
                    .AppendChild(ndProduct)
                    .AppendChild(ndSerialNumberHeader)
                    .AppendChild(ndTAX)
                    .AppendChild(ndLineMonetaryInfo)
                End With
                '-----------------------------------------------------------------
                ndLineItemHeader.AppendChild(ndLineItem)
            Catch ex As Exception
                Const strError As String = "Failed to Append Invoice Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSIN-20"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace