Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with CreditNote responses
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

    Public Class XmlCreditNoteResponse
        Inherits XmlResponse

        Private ndCreditNotes, ndCreditNote, ndCreditNoteHeader, ndCreditNoteType, ndCreditNoteNumber, _
               ndCustomerPO, ndOriginalOrderNumber, ndIMAccountNumber, ndCreditNoteDate, ndDocRoot, _
               ndDocHeaderRoot As XmlNode

        Private ndCAddress, ndCName, ndCAttention, ndCAddressLine1, ndCAddressLine2, ndCAddressLine3, _
                ndCAddressLine4, ndCCity, ndCPostalCode, ndCCountryCode As XmlNode

        Private ndTAddress, ndTName, ndTAttention, ndTAddressLine1, ndTAddressLine2, ndTAddressLine3, _
                ndTAddressLine4, ndTCity, ndTPostalCode, ndTCountryCode As XmlNode

        Private ndFAddress, ndFName, ndFAttention, ndFAddressLine1, ndFAddressLine2, ndFAddressLine3, _
                ndFAddressLine4, ndFCity, ndFPostalCode As XmlNode

        Private ndVAT, ndVATNumber, ndCurrency, ndCurrencyCode, ndPaymentTerms, ndTermsCode, ndPaymentDue, _
               ndPaymentDueDate, ndFCountryCode, ndTotalInformationTotalQuantity, ndTotalInformationOrderLines, _
               ndTotalFinancialTotalAmountDue, ndTotalFinancialTotalLineItemAmount, _
               ndTotalFinancialTotalTaxableAmount, ndTotalFinancialTotalCharges, _
               ndTotalFinancialTotalTaxAmount, ndFinalFinalTaxableAmount, ndFinalFinalTaxAmount As XmlNode

        Private ndLineItemHeader, ndLineItem, ndProduct, ndCustomerSKU, ndEANCode, ndManufacturerSKU, _
               ndSKUDescription1, ndSKUDescription2, ndSerialNumberHeader, ndSerialNumber, ndTAX, _
               ndLineMonetaryInfo, ndUnitPrice, ndLineItemAmount As XmlNode

        Private atCAddressType, atTAddressType, atFAddressType, atTotalQuantity, atOrderLines, atTotalAmountDue, _
                atTotalLineItemAmount, atTotalTaxableAmount, atTotalCharges, atTotalTaxAmount, atFinalTaxableAmount, _
                atFinalTaxAmount, atLineNumber, atSKU, atCreditNoteQuantity, atType As XmlAttribute

        Private qtyCount As Integer = 0
        Private linesCount As Integer = 0
        Private linesAmountCount As Double = 0.0

        Protected Overrides Sub InsertBodyV1()
            '--------------------------------------------------------------------------------
            '   Seperate the tables out of the ResultSet *
            '
            With MyBase.xmlDoc
                If Not Err.HasError Then

                    Dim dr As DataRow
                    '-------------------------------------------------------------------------
                    Try
                        dtHeader = ResultDataSet.Tables(0)
                        dtDetail = ResultDataSet.Tables(1)
                    Catch ex As Exception
                    End Try
                    '-------------------------------------------------------------------------
                    '   Add product lines (recursive) from table 2 *
                    '
                    ndCreditNotes = .CreateElement("CreditNotes")

                    If Not dtHeader Is Nothing AndAlso dtHeader.Rows.Count > 0 Then
                        For Each dr In dtHeader.Rows
                            Err2 = CreateHeader()
                            '--------------------------------------------------------------
                            ndCustomerPO.InnerText = dr("CustomerPO")
                            ndCreditNoteType.InnerText = String.Empty
                            Dim CreditNoteNumber As String = dr("CreditNoteNumber")
                            ndCreditNoteNumber.InnerText = dr("CreditNoteNumber")
                            Dim d As Date = dr("CreditNoteDateTime")
                            ndCreditNoteDate.InnerText = d.Year.ToString & d.Month.ToString & d.Day.ToString
                            ndCustomerPO.InnerText = dr("OrderNumber")
                            ndOriginalOrderNumber.InnerText = String.Empty
                            ndIMAccountNumber.InnerText = dr("AccountNumber")
                            '--------------------------------------------------------------
                            atCAddressType.Value = "Customer"
                            ndCName.InnerText = dr("CustomerName")
                            ndCAttention.InnerText = dr("CustomerAttention")
                            ndCAddressLine1.InnerText = dr("CustomerAddressLine1")
                            ndCAddressLine2.InnerText = dr("CustomerAddressLine2")
                            ndCAddressLine3.InnerText = dr("CustomerAddressLine3")
                            ndCAddressLine4.InnerText = dr("CustomerAddressLine4")
                            ndCCity.InnerText = dr("CustomerAddressLine5")
                            ndCPostalCode.InnerText = dr("CustomerAddressLine6")
                            ndCCountryCode.InnerText = dr("CustomerAddressLine7")
                            '--------------------------------------------------------------
                            atTAddressType.Value = "ShipTo"
                            ndTName.InnerText = dr("ShipToName")
                            ndTAttention.InnerText = dr("ShipToAttention")
                            ndTAddressLine1.InnerText = dr("ShipToAddressLine1")
                            ndTAddressLine2.InnerText = dr("ShipToAddressLine2")
                            ndTAddressLine3.InnerText = dr("ShipToAddressLine3")
                            ndTAddressLine4.InnerText = dr("ShipToAddressLine4")
                            ndTCity.InnerText = dr("ShipToAddressLine5")
                            ndTPostalCode.InnerText = dr("ShipToAddressLine6")
                            ndTCountryCode.InnerText = dr("ShipToAddressLine7")
                            '--------------------------------------------------------------
                            atFAddressType.Value = "ShipFrom"
                            ndFName.InnerText = dr("ShipFromName")
                            ndFAttention.InnerText = dr("ShipFromAttention")
                            ndFAddressLine1.InnerText = dr("ShipFromAddressLine1")
                            ndFAddressLine2.InnerText = dr("ShipFromAddressLine2")
                            ndFAddressLine3.InnerText = dr("ShipFromAddressLine3")
                            ndFAddressLine4.InnerText = dr("ShipFromAddressLine4")
                            ndFCity.InnerText = dr("ShipFromAddressLine5")
                            ndFPostalCode.InnerText = dr("ShipFromAddressLine6")
                            ndFCountryCode.InnerText = dr("ShipFromAddressLine7")
                            '--------------------------------------------------------------
                            ndVATNumber.InnerText = dr("VATNumber")
                            ndCurrencyCode.InnerText = dr("CurrencyCode")
                            ndTermsCode.InnerText = 0
                            ndPaymentDueDate.InnerText = Now.ToString("yyyyMMdd")

                            '---------------------------------------------------------------
                            '   Items 
                            '
                            Err2 = InsertDetails(CreditNoteNumber)

                            '--------------------------------------------------
                            atTotalQuantity.Value = "TotalQuantity"
                            atOrderLines.Value = "OrderLines"
                            atTotalAmountDue.Value = "TotalAmountDue"
                            atTotalTaxAmount.Value = "TotalTaxAmount"
                            atTotalTaxAmount.Value = "FinalTaxableAmount"
                            atFinalTaxAmount.Value = "FinalTaxAmount"
                            atTotalLineItemAmount.Value = "TotalLineItemAmount"
                            atTotalCharges.Value = "TotalCharges"
                            atTotalTaxableAmount.Value = "TotalTaxableAmount"
                            '--------------------------------------------------
                            ndTotalInformationOrderLines.InnerText = linesCount
                            ndTotalInformationTotalQuantity.InnerText = qtyCount
                            ndTotalFinancialTotalAmountDue.InnerText = dr("CreditNoteAmount")
                            ndTotalFinancialTotalLineItemAmount.InnerText = linesAmountCount
                            ndTotalFinancialTotalTaxableAmount.InnerText = String.Empty
                            ndTotalFinancialTotalCharges.InnerText = String.Empty
                            ndTotalFinancialTotalTaxAmount.InnerText = dr("VatAmount")
                            ndFinalFinalTaxableAmount.InnerText = String.Empty
                            ndFinalFinalTaxAmount.InnerText = String.Empty
                            ndFinalFinalTaxAmount.Attributes.Append(atFinalTaxAmount)
                            '--------------------------------------------------
                            linesCount = 0
                            qtyCount = 0
                            linesAmountCount = 0
                            Err2 = AppendHeader()
                        Next
                    Else
                        Err2 = CreateHeader()
                        Err2 = CreateDetails()
                        Err2 = AppendDetails()
                        Err2 = AppendHeader()
                        ndCreditNotes.AppendChild(ndCreditNote)
                    End If
                End If
                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                '   as there is potentially no Request XML this lot may be crap
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '   
                Try
                    ndDocRoot = .SelectSingleNode(c1 & RootElement())
                    ndDocHeaderRoot = .SelectSingleNode(c1 & RootElement() & c2)
                    ndDocRoot.InsertAfter(ndCreditNotes, ndDocHeaderRoot)

                Catch

                End Try

                'Insert the XSD reference & namespace as an attribute within the root node
                Dim atXmlNsXsi As XmlAttribute = CreateNamespaceAttribute()
                ndDocRoot.Attributes.Append(atXmlNsXsi)
            End With

        End Sub

        Private Function CreateHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndCreditNote = .CreateElement("CreditNote")
                    ndCreditNoteHeader = .CreateElement("CreditNoteHeader")
                    ndCreditNoteType = .CreateElement("CreditNoteType")
                    ndCreditNoteNumber = .CreateElement("CreditNoteNumber")
                    ndCreditNoteDate = .CreateElement("CreditNoteDate")
                    ndCustomerPO = .CreateElement("CustomerPO")
                    ndOriginalOrderNumber = .CreateElement("OriginalOrderNumber")
                    ndCAddress = .CreateElement("Address")
                    atCAddressType = .CreateAttribute("Type")
                    ndIMAccountNumber = .CreateElement("AccountNumber")
                    '--------------------------------------------------------------
                    ndCName = .CreateElement("Name")
                    ndCAttention = .CreateElement("Attention")
                    ndCAddressLine1 = .CreateElement("AddressLine1")
                    ndCAddressLine2 = .CreateElement("AddressLine2")
                    ndCAddressLine3 = .CreateElement("AddressLine3")
                    ndCAddressLine4 = .CreateElement("AddressLine4")
                    ndCCity = .CreateElement("City")
                    ndCPostalCode = .CreateElement("PostalCode")
                    ndCCountryCode = .CreateElement("CountryCode")
                    '--------------------------------------------------------------
                    ndTAddress = .CreateElement("Address")
                    atTAddressType = .CreateAttribute("Type")
                    ndTName = .CreateElement("Name")
                    ndTAttention = .CreateElement("Attention")
                    ndTAddressLine1 = .CreateElement("AddressLine1")
                    ndTAddressLine2 = .CreateElement("AddressLine2")
                    ndTAddressLine3 = .CreateElement("AddressLine3")
                    ndTAddressLine4 = .CreateElement("AddressLine4")
                    ndTCity = .CreateElement("City")
                    ndTPostalCode = .CreateElement("PostalCode")
                    ndTCountryCode = .CreateElement("CountryCode")
                    '--------------------------------------------------------------
                    ndFAddress = .CreateElement("Address")
                    atFAddressType = .CreateAttribute("Type")
                    ndFName = .CreateElement("Name")
                    ndFAttention = .CreateElement("Attention")
                    ndFAddressLine1 = .CreateElement("AddressLine1")
                    ndFAddressLine2 = .CreateElement("AddressLine2")
                    ndFAddressLine3 = .CreateElement("AddressLine3")
                    ndFAddressLine4 = .CreateElement("AddressLine4")
                    ndFCity = .CreateElement("City")
                    ndFPostalCode = .CreateElement("PostalCode")
                    ndFCountryCode = .CreateElement("CountryCode")
                    '--------------------------------------------------------------
                    ndVAT = .CreateElement("VAT")
                    ndVATNumber = .CreateElement("VATNumber")
                    ndCurrency = .CreateElement("Currency")
                    ndCurrencyCode = .CreateElement("CurrencyCode")
                    ndPaymentTerms = .CreateElement("PaymentTerms")
                    ndTermsCode = .CreateElement("TermsCode")
                    ndPaymentDue = .CreateElement("PaymentDue")
                    ndPaymentDueDate = .CreateElement("PaymentDueDate")
                    '--------------------------------------------------------------
                    ndLineItemHeader = .CreateElement("LineItemHeader")
                    '--------------------------------------------------------------
                    ndTotalInformationTotalQuantity = .CreateElement("TotalInformation")
                    atTotalQuantity = .CreateAttribute("Type")
                    atOrderLines = .CreateAttribute("Type")
                    ndTotalFinancialTotalAmountDue = .CreateElement("TotalFinancial")
                    ndTotalInformationOrderLines = .CreateElement("TotalInformation")
                    atTotalAmountDue = .CreateAttribute("Type")
                    atTotalLineItemAmount = .CreateAttribute("Type")
                    ndTotalFinancialTotalLineItemAmount = .CreateElement("TotalFinancial")
                    ndTotalFinancialTotalTaxableAmount = .CreateElement("TotalFinancial")
                    atTotalTaxableAmount = .CreateAttribute("Type")
                    ndTotalFinancialTotalCharges = .CreateElement("TotalFinancial")
                    atTotalCharges = .CreateAttribute("Type")
                    ndTotalFinancialTotalTaxAmount = .CreateElement("TotalFinancial")
                    atTotalTaxAmount = .CreateAttribute("Type")
                    ndTotalFinancialTotalTaxAmount.Attributes.Append(atTotalTaxAmount)
                    ndFinalFinalTaxAmount = .CreateElement("Final")
                    atFinalTaxAmount = .CreateAttribute("Type")
                    ndFinalFinalTaxableAmount = .CreateElement("Final")
                    atFinalTaxableAmount = .CreateAttribute("Type")
                    '--------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOD-15"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With ndCreditNoteHeader
                    .AppendChild(ndCreditNoteType)
                    .AppendChild(ndCreditNoteNumber)
                    .AppendChild(ndCreditNoteDate)
                    .AppendChild(ndCustomerPO)
                    .AppendChild(ndOriginalOrderNumber)
                End With
                '--------------------------------------------------------------
                With ndCAddress
                    .Attributes.Append(atCAddressType)
                    .AppendChild(ndCName)
                    .AppendChild(ndCAttention)
                    .AppendChild(ndCAddressLine1)
                    .AppendChild(ndCAddressLine2)
                    .AppendChild(ndCAddressLine3)
                    .AppendChild(ndCAddressLine4)
                    .AppendChild(ndCCity)
                    .AppendChild(ndCPostalCode)
                    .AppendChild(ndCCountryCode)
                End With
                '--------------------------------------------------------------
                With ndTAddress
                    .Attributes.Append(atTAddressType)
                    .AppendChild(ndTName)
                    .AppendChild(ndTAttention)
                    .AppendChild(ndTAddressLine1)
                    .AppendChild(ndTAddressLine2)
                    .AppendChild(ndTAddressLine3)
                    .AppendChild(ndTAddressLine4)
                    .AppendChild(ndTCity)
                    .AppendChild(ndTPostalCode)
                    .AppendChild(ndTCountryCode)
                End With
                '--------------------------------------------------------------
                With ndFAddress
                    .Attributes.Append(atFAddressType)
                    .AppendChild(ndFName)
                    .AppendChild(ndFAttention)
                    .AppendChild(ndFAddressLine1)
                    .AppendChild(ndFAddressLine2)
                    .AppendChild(ndFAddressLine3)
                    .AppendChild(ndFAddressLine4)
                    .AppendChild(ndFCity)
                    .AppendChild(ndFPostalCode)
                    .AppendChild(ndFCountryCode)
                End With
                '--------------------------------------------------------------
                ndVAT.AppendChild(ndVATNumber)
                ndCurrency.AppendChild(ndCurrencyCode)
                ndPaymentTerms.AppendChild(ndTermsCode)
                ndPaymentDue.AppendChild(ndPaymentDueDate)
                '--------------------------------------------------------------
                ndTotalInformationTotalQuantity.Attributes.Append(atTotalQuantity)
                ndTotalInformationOrderLines.Attributes.Append(atOrderLines)
                ndTotalFinancialTotalAmountDue.Attributes.Append(atTotalAmountDue)
                ndTotalFinancialTotalLineItemAmount.Attributes.Append(atTotalLineItemAmount)
                ndTotalFinancialTotalTaxableAmount.Attributes.Append(atTotalTaxableAmount)
                ndTotalFinancialTotalCharges.Attributes.Append(atTotalCharges)
                ndFinalFinalTaxableAmount.Attributes.Append(atTotalTaxAmount)
                With ndCreditNote
                    .AppendChild(ndCreditNoteHeader)
                    .AppendChild(ndIMAccountNumber)
                    .AppendChild(ndCAddress)
                    .AppendChild(ndTAddress)
                    .AppendChild(ndFAddress)
                    .AppendChild(ndVAT)
                    .AppendChild(ndCurrency)
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
                    ndCreditNotes.AppendChild(ndCreditNote)
                End With
                '--------------------------------------------------------------
            Catch ex As Exception
                Const strError As String = "Failed to Append  Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOD-16"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertDetails(ByVal CreditNoteNumber As String) As ErrorObj
            Dim err As ErrorObj = Nothing
            Dim dr As DataRow
            '--------------------------------------------------------------------------
            Try

                If Not dtDetail Is Nothing AndAlso dtDetail.Rows.Count > 0 Then
                    For Each dr In dtDetail.Rows
                        If dr("CreditNoteNumber").Equals(CreditNoteNumber) Then
                            err = CreateDetails()
                            '--------------------------------------------------
                            atLineNumber.Value = dr("LineNumber")
                            atSKU.Value = dr("ProductCode")
                            atCreditNoteQuantity.Value = dr("QuantityCreditNote")
                            ndCustomerSKU.InnerText = String.Empty
                            ndEANCode.InnerText = dr("EANCode")
                            ndManufacturerSKU.InnerText = String.Empty
                            ndSKUDescription1.InnerText = dr("Description1")
                            ndSKUDescription2.InnerText = dr("Description2")
                            ndSerialNumber.InnerText = String.Empty
                            atType.Value = "VAT"
                            ndTAX.InnerText = dr("CreditNoteLineVatAmount")
                            ndUnitPrice.InnerText = dr("ProductPrice")
                            Dim d1 As Decimal = dr("CreditNoteLineNetAmount")
                            Dim d2 As Decimal = dr("CreditNoteLineVatAmount")
                            ndLineItemAmount.InnerText = dr("CreditNoteLineNetAmount") + dr("CreditNoteLineVatAmount")
                            qtyCount += CInt(dr("QuantityCreditNote"))
                            linesCount += 1
                            linesAmountCount += CDec(ndLineItemAmount.InnerText)
                            '--------------------------------------------------
                            err = AppendDetails()
                        End If
                    Next
                Else
                    err = CreateDetails()
                    err = AppendDetails()
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert Order Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOD-171"
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
                    '--------------------------------------------------------------
                    ndLineItem = .CreateElement("LineItem")
                    ndProduct = .CreateElement("Product")
                    atSKU = .CreateAttribute("SKU")
                    atLineNumber = .CreateAttribute("LineNumber")
                    atCreditNoteQuantity = .CreateAttribute("CreditNoteQuantity")
                    ndCustomerSKU = .CreateElement("CustomerSKU")
                    ndEANCode = .CreateElement("EANCode")
                    ndManufacturerSKU = .CreateElement("ManufacturerSKU")
                    ndSKUDescription1 = .CreateElement("SKUDescription1")
                    ndSKUDescription2 = .CreateElement("SKUDescription2")
                    '--------------------------------------------------------------
                    ndSerialNumberHeader = .CreateElement("SerialNumberHeader")
                    ndSerialNumber = .CreateElement("SerialNumber")
                    ndTAX = .CreateElement("Tax")
                    atType = .CreateAttribute("Type")
                    ndLineMonetaryInfo = .CreateElement("LineMonetaryInfo")
                    ndUnitPrice = .CreateElement("UnitPrice")
                    ndLineItemAmount = .CreateElement("LineItemAmount")
                    '--------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOD-17"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                '--------------------------------------------------------------
                ndLineItem.Attributes.Append(atLineNumber)
                ndProduct.Attributes.Append(atSKU)
                ndProduct.Attributes.Append(atCreditNoteQuantity)
                With ndProduct
                    .AppendChild(ndCustomerSKU)
                    .AppendChild(ndEANCode)
                    .AppendChild(ndManufacturerSKU)
                    .AppendChild(ndSKUDescription1)
                    .AppendChild(ndSKUDescription2)
                End With
                ndSerialNumberHeader.AppendChild(ndSerialNumber)
                ndTAX.Attributes.Append(atType)
                '--------------------------------------------------------------
                With ndLineMonetaryInfo
                    .AppendChild(ndUnitPrice)
                    .AppendChild(ndLineItemAmount)
                End With
                With ndLineItem
                    .AppendChild(ndProduct)
                    .AppendChild(ndSerialNumberHeader)
                    .AppendChild(ndTAX)
                    .AppendChild(ndLineMonetaryInfo)
                End With
                ndLineItemHeader.AppendChild(ndLineItem)
                '--------------------------------------------------------------
            Catch ex As Exception
                Const strError As String = "Failed to Append  Order Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOD-18"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace