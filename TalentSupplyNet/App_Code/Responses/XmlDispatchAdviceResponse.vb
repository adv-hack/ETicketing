Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Dispatch Advice Response
'
'       Date                        30th Jan 2007
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQDA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlDispatchAdviceResponse
        Inherits XmlResponse

        Private ndDispatchAdviceResponse, ndDocRoot, ndDocHeaderRoot, _
                ndDispatchAdvices, ndDispatchAdvice As XmlNode

        Private ndDispatchAdviceHeader, ndDispatchAdviceNumber, _
                ndBranchOrderNumber, ndCustomerPO, ndAccountNumber, _
                ndDateHeader, ndDate1, ndDate2, ndDate3 As XmlNode

        Private ndCAddress, ndCName, ndCAttention, ndCAddress1, ndCAddress2, ndCAddress3, _
                ndCAddress4, ndCCity, ndCProvince, ndCPostalCode, ndCCountryCode As XmlNode

        Private ndTAddress, ndTName, ndTAttention, ndTAddress1, ndTAddress2, ndTAddress3, _
                ndTAddress4, ndTCity, ndTProvince, ndTPostalCode, ndTCountryCode, ndTShipToSuffix As XmlNode

        Private ndFAddress, ndFName, ndFAttention, ndFAddress1, ndFAddress2, ndFAddress3, _
                ndFAddress4, ndFCity, ndFProvince, ndFPostalCode, ndFCountryCode As XmlNode

        Private ndConsignmentHeader, ndSequenceNumber, ndNumberOfPackages, _
                ndMeasurementHeader, ndUnitOfMeasure, ndWeight, _
                ndPackageHeader, ndIdentificationHeader, ndIdentification, _
                ndTotalLinesShipped As XmlNode

        Private ndLineHeader, ndLineItem, ndProduct, ndItem1, ndItem2, _
                ndSKUDescription1, ndSKUDescription2, ndSerialNumberHeader, _
                ndSerialNumber As XmlNode

        Private atCAddressType, atTAddressType, atFAddressType, _
                atDateType1, atDateType2, atDateType3, _
                atTrackingNumber, atLineNumber, atSKU, atDispatchQuantity, _
                atItemType1, atItemType2 As XmlAttribute

        Private linesCount As Integer = 0


        Protected Overrides Sub InsertBodyV1()
            '------------------------------------------------------------------------------
            '   Seperate the tables out of the ResultSet    
            '
            Try
                With MyBase.xmlDoc
                    ndDispatchAdviceResponse = .CreateElement("DispatchAdviceResponse")
                    If Not Err.HasError Then
                        dtHeader = ResultDataSet.Tables("DtHeader")                  ' Header
                        dtDetail = ResultDataSet.Tables("DtDetail")                  ' Items
                        dtText = ResultDataSet.Tables("DtText")                      ' Serial numbers
                        dtProduct = ResultDataSet.Tables("DtProduct")                ' Serial numbers
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
                    ndDocRoot.InsertAfter(ndDispatchAdviceResponse, ndDocHeaderRoot)
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
            Dim backOfficeOrderNumber As String = String.Empty
            Dim dispatchNo As String = String.Empty
            Dim OrderNumber As String = String.Empty
            Try
                If Not dtHeader Is Nothing AndAlso dtHeader.Rows.Count > 0 Then
                    For Each dRow In dtHeader.Rows
                        '--------------------------------------------------------------------------
                        err = CreateHeader()
                        '
                        ndAccountNumber.InnerText = String.Empty
                        '---------------------------------------------------------------------------
                        'Dim OrderNumber As String = dRow("DispatchAdviceNumber")
                        OrderNumber = dRow("OrderNumber").ToString.Trim
                        backOfficeOrderNumber = dRow("BackOfficeOrderNumber").ToString
                        ndDispatchAdviceNumber.InnerText = backOfficeOrderNumber & "-" & _
                                                           dRow("DispatchNoteSequence").ToString.Trim
                        dispatchNo = dRow("DispatchNoteSequence").ToString.Trim

                        ndBranchOrderNumber.InnerText = backOfficeOrderNumber.Trim
                        ndCustomerPO.InnerText = Utilities.CheckForDBNull_String(dRow("CustomerPO"))
                        ndAccountNumber.InnerText = dRow("AccountNumber")
                        '---------------------------------------------------------------------------
                        Try
                            Dim d1 As Date = dRow("DocumentDate")
                            ndDate1.InnerText = d1.ToString("yyyyMMdd")
                        Catch
                        End Try
                        Try
                            Dim d2 As Date = dRow("DispatchDate")
                            ndDate2.InnerText = d2.ToString("yyyyMMdd")
                        Catch ex As Exception
                        End Try
                        Try
                            Dim d3 As Date = dRow("DeliveryDate")
                            ndDate3.InnerText = d3.ToString("yyyyMMdd")
                        Catch ex As Exception
                        End Try
                        '---------------------------------------------------------------------------
                        ndCName.InnerText = Utilities.CheckForDBNull_String(dRow("CustomerName"))
                        ndCAttention.InnerText = Utilities.CheckForDBNull_String(dRow("CustomerAttention"))
                        ndCAddress1.InnerText = Utilities.CheckForDBNull_String(dRow("CustomerAddressLine1"))
                        ndCAddress2.InnerText = Utilities.CheckForDBNull_String(dRow("CustomerAddressLine2"))
                        ndCAddress3.InnerText = Utilities.CheckForDBNull_String(dRow("CustomerAddressLine3"))
                        ndCAddress4.InnerText = Utilities.CheckForDBNull_String(dRow("CustomerAddressLine4"))
                        ndCCity.InnerText = Utilities.CheckForDBNull_String(dRow("CustomerCity"))
                        ndCProvince.InnerText = Utilities.CheckForDBNull_String(dRow("CustomerProvince"))

                        ndCPostalCode.InnerText = Utilities.CheckForDBNull_String(dRow("CustomerPostalCode"))
                        ndCCountryCode.InnerText = Utilities.CheckForDBNull_String(dRow("CustomerCountryCode"))
                        '---------------------------------------------------------------------------
                        ndTName.InnerText = Utilities.CheckForDBNull_String(dRow("ShipToName"))
                        ndTAttention.InnerText = Utilities.CheckForDBNull_String(dRow("ShipToAttention"))
                        ndTAddress1.InnerText = Utilities.CheckForDBNull_String(dRow("ShipToAddressLine1"))
                        ndTAddress2.InnerText = Utilities.CheckForDBNull_String(dRow("ShipToAddressLine2"))
                        ndTAddress3.InnerText = Utilities.CheckForDBNull_String(dRow("ShipToAddressLine3"))
                        ndTAddress4.InnerText = Utilities.CheckForDBNull_String(dRow("ShipToAddressLine4"))
                        ndTCity.InnerText = Utilities.CheckForDBNull_String(dRow("ShipToCity"))
                        ndTProvince.InnerText = Utilities.CheckForDBNull_String(dRow("ShipToProvince"))
                        ndTPostalCode.InnerText = Utilities.CheckForDBNull_String(dRow("ShipToPostalCode"))
                        ndTCountryCode.InnerText = Utilities.CheckForDBNull_String(dRow("ShipToCountryCode"))
                        ndTShipToSuffix.InnerText = Utilities.CheckForDBNull_String(dRow("CustomerShipToSuffix"))
                        '---------------------------------------------------------------------------
                        ndFName.InnerText = Utilities.CheckForDBNull_String(dRow("ShipFromName"))
                        ndFAttention.InnerText = Utilities.CheckForDBNull_String(dRow("ShipFromAttention"))
                        ndFAddress1.InnerText = Utilities.CheckForDBNull_String(dRow("ShipFromAddressLine1"))
                        ndFAddress2.InnerText = Utilities.CheckForDBNull_String(dRow("ShipFromAddressLine2"))
                        ndFAddress3.InnerText = Utilities.CheckForDBNull_String(dRow("ShipFromAddressLine3"))
                        ndFAddress4.InnerText = Utilities.CheckForDBNull_String(dRow("ShipFromAddressLine4"))
                        ndFCity.InnerText = Utilities.CheckForDBNull_String(dRow("ShipFromCity"))
                        ndFProvince.InnerText = Utilities.CheckForDBNull_String(dRow("ShipFromProvince"))
                        ndFPostalCode.InnerText = Utilities.CheckForDBNull_String(dRow("ShipFromPostalCode"))
                        ndFCountryCode.InnerText = Utilities.CheckForDBNull_String(dRow("ShipFromCountryCode"))
                        '---------------------------------------------------------------------------
                        ndSequenceNumber.InnerText = Utilities.CheckForDBNull_String(dRow("DispatchNoteSequence"))
                        ndNumberOfPackages.InnerText = Utilities.CheckForDBNull_String(dRow("NumberOfPackages"))
                        '
                        ndUnitOfMeasure.InnerText = Utilities.CheckForDBNull_String(dRow("UnitOfMeasure"))
                        ndWeight.InnerText = Utilities.CheckForDBNull_String(dRow("Weight"))
                        '
                        atTrackingNumber.Value = Utilities.CheckForDBNull_String(dRow("TrackingNumber"))
                        '---------------------------------------------------------------------------
                        '    err = InsertDetails(OrderNumber)
                        err = InsertDetails(backOfficeOrderNumber, dispatchNo)

                        ' ndTotalLinesShipped.InnerText = dRow("TotalLinesShipped")
                        ndTotalLinesShipped.InnerText = linesCount.ToString
                        linesCount = 0
                        err = AppendHeader()
                        ndDispatchAdviceResponse.AppendChild(ndDispatchAdvice)
                    Next
                Else
                    '------------------------------------------------------------------------------
                    '   No records - Create dummy
                    '
                    err = CreateHeader()
                    err = CreateDetails()
                    err = CreateComments()
                    err = AppendComments()
                    err = AppendDetails()
                    err = AppendHeader()
                    ndDispatchAdviceResponse.AppendChild(ndDispatchAdvice)
                End If
                '---------------------------------------------------------------------------
            Catch ex As Exception
                Const strError As String = "Failed to Insert Dispatch Advice Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQDA-11"
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
                    ndDispatchAdvices = .CreateElement("DispatchAdvices")
                    ndDispatchAdvice = .CreateElement("DispatchAdvice")
                    ndDispatchAdviceHeader = .CreateElement("DispatchAdviceHeader")
                    ndDispatchAdviceNumber = .CreateElement("DispatchAdviceNumber")
                    ndBranchOrderNumber = .CreateElement("BranchOrderNumber")
                    ndCustomerPO = .CreateElement("CustomerPO")
                    ndAccountNumber = .CreateElement("AccountNumber")
                    '---------------------------------------------------------------------------
                    ndDateHeader = .CreateElement("DateHeader")
                    ndDate1 = .CreateElement("Date")
                    atDateType1 = .CreateAttribute("Type")
                    atDateType1.Value = "Document"
                    ndDate2 = .CreateElement("Date")
                    atDateType2 = .CreateAttribute("Type")
                    atDateType2.Value = "Dispatch"
                    ndDate3 = .CreateElement("Date")
                    atDateType3 = .CreateAttribute("Type")
                    atDateType3.Value = "Delivery"
                    '---------------------------------------------------------------------------
                    ndCAddress = .CreateElement("Address")
                    atCAddressType = .CreateAttribute("Type")
                    atCAddressType.Value = "Customer"
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
                    atTAddressType.Value = "ShipTo"
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
                    ndTShipToSuffix = .CreateElement("ShipToSuffix")
                    '---------------------------------------------------------------------------
                    ndFAddress = .CreateElement("Address")
                    atFAddressType = .CreateAttribute("Type")
                    atFAddressType.Value = "ShipFrom"
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
                    ndConsignmentHeader = .CreateElement("ConsignmentHeader")
                    ndSequenceNumber = .CreateElement("SequenceNumber")
                    ndNumberOfPackages = .CreateElement("NumberOfPackages")
                    '
                    ndMeasurementHeader = .CreateElement("MeasurementHeader")
                    ndUnitOfMeasure = .CreateElement("UnitOfMeasure")
                    ndWeight = .CreateElement("Weight")
                    atTrackingNumber = .CreateAttribute("TrackingNumber")
                    '
                    ndPackageHeader = .CreateElement("PackageHeader")
                    ndIdentificationHeader = .CreateElement("IdentificationHeader")
                    ndIdentification = .CreateElement("Identification")
                    '---------------------------------------------------------------------------
                    ndLineHeader = .CreateElement("LineHeader")
                    ndTotalLinesShipped = .CreateElement("TotalLinesShipped")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create DispatchAdvice Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQDA-12"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With ndDispatchAdviceHeader
                    .AppendChild(ndAccountNumber)
                    .AppendChild(ndDispatchAdviceNumber)
                    .AppendChild(ndBranchOrderNumber)
                    .AppendChild(ndCustomerPO)
                    .AppendChild(ndDateHeader)
                    .AppendChild(ndDate1)
                    .AppendChild(ndDate2)
                    .AppendChild(ndDate3)
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
                    .AppendChild(ndCProvince)
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
                    .AppendChild(ndTProvince)
                    .AppendChild(ndTPostalCode)
                    .AppendChild(ndTCountryCode)
                    .AppendChild(ndTShipToSuffix)
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
                    .AppendChild(ndTProvince)
                    .AppendChild(ndFPostalCode)
                    .AppendChild(ndFCountryCode)
                End With
                '---------------------------------------------------------------------------
                With ndConsignmentHeader
                    .AppendChild(ndSequenceNumber)
                    .AppendChild(ndNumberOfPackages)
                End With
                With ndMeasurementHeader
                    .AppendChild(ndUnitOfMeasure)
                    .AppendChild(ndWeight)
                End With
                ndPackageHeader.AppendChild(ndIdentificationHeader)
                ndIdentificationHeader.AppendChild(ndIdentification)
                '---------------------------------------------------------------------------
                With ndDispatchAdvice
                    .AppendChild(ndDispatchAdviceHeader)
                    .AppendChild(ndCAddress)
                    .AppendChild(ndTAddress)
                    .AppendChild(ndFAddress)
                    .AppendChild(ndConsignmentHeader)
                    .AppendChild(ndMeasurementHeader)
                    .AppendChild(ndPackageHeader)
                    .AppendChild(ndLineHeader)
                End With
                '---------------------------------------------------------------------------
                ndDispatchAdviceResponse.AppendChild(ndDispatchAdvice)
                '---------------------------------------------------------------------------
            Catch ex As Exception
                Const strError As String = "Failed to Append Dispatch Advice Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSIN-14"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertDetails(ByVal OrderNumber As String, ByVal dispatchSeq As String) As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                Dim dr As DataRow
                If Not dtDetail Is Nothing AndAlso dtDetail.Rows.Count > 0 Then
                    For Each dr In dtDetail.Rows
                        If dr("OrderNumber").Equals(OrderNumber) AndAlso dr("itemDispatchSequence").ToString = dispatchSeq Then

                            '-----------------------------------------------------------
                            err = CreateDetails()
                            Dim LineNumber As String = dr("LineNumber")
                            atLineNumber.Value = LineNumber
                            atSKU.Value = dr("ProductCode")
                            atDispatchQuantity.Value = dr("DispatchQuantity")
                            '------------------------------------------------------------
                            ndItem1.InnerText = dr("EANCode")
                            ndItem2.InnerText = dr("CustomerSKU")
                            ndSKUDescription1.InnerText = dr("Description1")
                            ndSKUDescription2.InnerText = dr("Description2")
                            '------------------------------------------------------------
                            ' err = InsertComments(OrderNumber, LineNumber)
                            err = AppendSerialNumbers(OrderNumber, LineNumber)
                            linesCount += 1
                            err = AppendDetails()
                        End If
                    Next
                Else
                    '---------------------------------------------------------------------------
                    '   No Items so create Dummy
                    '
                    err = CreateDetails()
                    err = CreateComments()
                    err = AppendComments()
                    err = AppendDetails()
                    '
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert Dispatch Advice Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQDA-18"
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
                    atDispatchQuantity = .CreateAttribute("DispatchQuantity")
                    '
                    ndItem1 = .CreateElement("Item")
                    atItemType1 = .CreateAttribute("Type")
                    atItemType1.Value = "EANCode"
                    ndItem1.Attributes.Append(atItemType1)
                    '
                    ndItem2 = .CreateElement("Item")
                    atItemType2 = .CreateAttribute("Type")
                    atItemType2.Value = "CustomerSKU"
                    ndItem2.Attributes.Append(atItemType2)
                    '
                    ndSKUDescription1 = .CreateElement("SKUDescription1")
                    ndSKUDescription2 = .CreateElement("SKUDescription2")
                    ndSerialNumberHeader = .CreateElement("SerialNumberHeader")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Dispatch Advice Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQDA-19"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With ndLineItem
                    .Attributes.Append(atLineNumber)
                    .AppendChild(ndProduct)
                    With ndProduct
                        .Attributes.Append(atSKU)
                        .Attributes.Append(atDispatchQuantity)
                        .AppendChild(ndItem1)
                        .AppendChild(ndItem2)
                        .AppendChild(ndSKUDescription1)
                        .AppendChild(ndSKUDescription2)
                        .AppendChild(ndSerialNumberHeader)
                    End With
                    '-----------------------------------------------------------------
                End With
                '-----------------------------------------------------------------
                ndLineHeader.AppendChild(ndLineItem)
            Catch ex As Exception
                Const strError As String = "Failed to Append Dispatch Advice Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQDA-20"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertComments(ByVal OrderNumber As String, ByVal LineNumber As String) As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                Dim dr As DataRow
                If Not dtText Is Nothing AndAlso dtText.Rows.Count > 0 Then
                    For Each dr In dtText.Rows
                        If dr("OrderNumber").Equals(OrderNumber) And dr("LineNumber").Equals(LineNumber) Then
                            '-----------------------------------------------------------
                            err = CreateComments()
                            ndSerialNumber.InnerText = dr("SerialNumber")
                            err = AppendComments()
                        End If
                    Next
                Else
                    '---------------------------------------------------------------------------
                    '   No Items so create Dummy
                    '
                    err = CreateComments()
                    err = AppendComments()
                    '
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert Serial Number Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQDA-21"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendSerialNumbers(ByVal OrderNumber As String, ByVal LineNumber As String) As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                Dim dr As DataRow
                If Not dtProduct Is Nothing AndAlso dtProduct.Rows.Count > 0 Then
                    For Each dr In dtProduct.Rows
                        If dr("OrderNumber").Equals(OrderNumber) And dr("OrderLine").Equals(LineNumber) Then
                            '-----------------------------------------------------------
                            With MyBase.xmlDoc
                                ndSerialNumber = .CreateElement("SerialNumber")
                                ndSerialNumber.InnerText = dr("SerialNumber")
                                ndSerialNumberHeader.AppendChild(ndSerialNumber)
                                err = AppendComments()
                            End With

                        End If
                    Next
                Else
                    '---------------------------------------------------------------------------
                    '   No Items so create Dummy
                    '
                    err = CreateComments()
                    err = AppendComments()
                    '
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert Serial Number Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQDA-21"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateComments() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndSerialNumber = .CreateElement("SerialNumber")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Serial Number Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQDA-22"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendComments() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                ndSerialNumberHeader.AppendChild(ndSerialNumber)
                '
            Catch ex As Exception
                Const strError As String = "Failed to Append Serial Number Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRQDA-23"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace