Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with PNA order responses
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSPN- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlPNAResponse
        Inherits XmlResponse

        Private _availableRebQty As Integer
        Private _availability As Integer
        Private _branch As String = String.Empty
        Private _branchID As String = String.Empty
        Private _branchName As String = String.Empty
        Private _description As String = String.Empty
        Private _etaDate As String = String.Empty
        Private _manufacturerPartNumber As String = String.Empty
        Private _manufacturerPartNumberOccurs As String = String.Empty
        Private _onOrder As Integer
        Private _priceAndAvailabilitySku As String = String.Empty
        Private _priceAndAvailabilityQuantity As Integer
        Private _price As Double
        Private _specialPriceFlag As Double
        Private _vendorNumber As String = String.Empty
        Private _reserveInventoryFlag As String = String.Empty

        Public Property Availability() As Integer
            Get
                Return _availability
            End Get
            Set(ByVal value As Integer)
                _availability = value
            End Set
        End Property
        Public Property AvailableRebQty() As Integer
            Get
                Return _availableRebQty
            End Get
            Set(ByVal value As Integer)
                _availableRebQty = value
            End Set
        End Property
        Public Property Branch() As String
            Get
                Return _branch
            End Get
            Set(ByVal value As String)
                _branch = value
            End Set
        End Property
        Public Property BranchID() As String
            Get
                Return _branchID
            End Get
            Set(ByVal value As String)
                _branchID = value
            End Set
        End Property
        Public Property BranchName() As String
            Get
                Return _branchName
            End Get
            Set(ByVal value As String)
                _branch = value
            End Set
        End Property
        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property
        Public Property EtaDate() As String
            Get
                Return _etaDate
            End Get
            Set(ByVal value As String)
                _etaDate = value
            End Set
        End Property
        Public Property ManufacturerPartNumber() As String
            Get
                Return _manufacturerPartNumber
            End Get
            Set(ByVal value As String)
                _manufacturerPartNumber = value
            End Set
        End Property
        Public Property ManufacturerPartNumberOccurs() As String
            Get
                Return _manufacturerPartNumberOccurs
            End Get
            Set(ByVal value As String)
                _manufacturerPartNumberOccurs = value
            End Set
        End Property
        Public Property OnOrder() As Integer
            Get
                Return _onOrder
            End Get
            Set(ByVal value As Integer)
                _onOrder = value
            End Set
        End Property
        Public Property Price() As Double
            Get
                Return _price
            End Get
            Set(ByVal value As Double)
                _price = value
            End Set
        End Property
        Public Property PriceAndAvailabilitySku() As String
            Get
                Return _priceAndAvailabilitySku
            End Get
            Set(ByVal value As String)
                _priceAndAvailabilitySku = value
            End Set
        End Property
        Public Property PriceAndAvailabilityQuantity() As Integer
            Get
                Return _priceAndAvailabilityQuantity
            End Get
            Set(ByVal value As Integer)
                _priceAndAvailabilityQuantity = value
            End Set
        End Property
        Public Property ReserveInventoryFlag() As String
            Get
                Return _reserveInventoryFlag
            End Get
            Set(ByVal value As String)
                _reserveInventoryFlag = value
            End Set
        End Property
        Public Property SpecialPriceFlag() As Double
            Get
                Return _specialPriceFlag
            End Get
            Set(ByVal value As Double)
                _specialPriceFlag = value
            End Set
        End Property
        Public Property VendorNumber() As String
            Get
                Return _vendorNumber
            End Get
            Set(ByVal value As String)
                _vendorNumber = value
            End Set
        End Property

        Private ndHeaderRoot, ndHeaderRootHeader, _
                ndHeader, ndPrice, ndCurrencyCode, ndSpecialPriceFlag, ndManufacturerPartNumber, ndManufacturerPartNumberOccurs, _
                ndVendorNumber, ndDescription, ndReserveInventoryFlag, ndAvailableRebQty, ndBranch, _
                ndAvailability, ndOnOrder, ndEtaDate, ndSection As XmlNode

        Private atXmlNsXsi, atSku, atQuantity, atID, atName As XmlAttribute

        Private ndWHsection As XmlNode
        Private atWHid As XmlAttribute


        Protected Overrides Sub InsertBodyV1()

            Dim dtr As DataTableReader
            Dim drDetail As DataRow
            Dim dt As Data.DataTable
            If Not ResultDataSet Is Nothing AndAlso ResultDataSet.Tables.Count > 0 Then
                dtr = New DataTableReader(ResultDataSet.Tables(0))
                'If Not dtr Is Nothing AndAlso dtr.HasRows() Then
                '    While dtr.Read
                '        dtr.Read()
                '        _manufacturerPartNumber = dtr.Item(0).ToString.Trim
                '        _price = dtr.Item(1).ToString.Trim
                '    End While

                'End If
            End If
            If Not ResultDataSet Is Nothing Then

                ' dtDetail = ResultDataSet.Tables(0)
                dt = ResultDataSet.Tables(0)
                '--------------------------------------------------------------------------
                With MyBase.xmlDoc
                    ndHeader = .CreateElement("MultiPNA")

                    Dim currentProduct As String = String.Empty

                    For Each drDetail In dt.Rows

                        If currentProduct <> drDetail("ProductNumber").ToString.Trim Then
                            Err2 = CreateHeader()
                            '----------------------------------------------------------------------

                            atQuantity.Value = drDetail("QuantityRequested").ToString.Trim

                            ndPrice.InnerText = drDetail("Price").ToString.Trim
                            ndDescription.InnerText = drDetail("Description").ToString.Trim
                            ndCurrencyCode.InnerText = drDetail("CurrencyCode").ToString.Trim

                            If Not SpecialPriceFlag = 0 Then _
                               ndSpecialPriceFlag.InnerText = SpecialPriceFlag().ToString
                            If Not AvailableRebQty = 0 Then _
                               ndAvailableRebQty.InnerText = AvailableRebQty().ToString
                            If Not Availability = 0 Then _
                                ndAvailability.InnerText = Availability().ToString
                            If Not OnOrder = 0 Then _
                                ndOnOrder.InnerText = OnOrder().ToString
                            '----------------------------------------------------------------------
                            ndManufacturerPartNumber.InnerText = drDetail("AltPartNo").ToString.Trim
                            ndManufacturerPartNumberOccurs.InnerText = ManufacturerPartNumberOccurs()
                            ndVendorNumber.InnerText = VendorNumber()
                            '   ndDescription.InnerText = Description()

                            CreateWarehouse()
                            atWHid.Value = drDetail("Warehouse").ToString.Trim
                            ndAvailability.InnerText = drDetail("QuantityAvailable").ToString.Trim
                            ndOnOrder.InnerText = drDetail("QuantityOnOrder").ToString.Trim

                            ndEtaDate.InnerText = EtaDate()
                            ndReserveInventoryFlag.InnerText = ReserveInventoryFlag()
                            ' ndBranch.InnerText = Branch()
                            '  atID.Value = BranchID()
                            '   atName.Value = BranchName()
                            atSku.Value = drDetail("ProductNumber").ToString.Trim
                            '--------------------------------------------------------------------------
                            Err2 = AppendHeader()
                            currentProduct = drDetail("ProductNumber").ToString.Trim
                        Else
                            CreateWarehouse()
                            atWHid.Value = drDetail("Warehouse").ToString.Trim
                            ndAvailability.InnerText = drDetail("QuantityAvailable").ToString.Trim
                            ndOnOrder.InnerText = drDetail("QuantityOnOrder").ToString.Trim
                            currentProduct = drDetail("ProductNumber").ToString.Trim
                        End If

                    Next

                End With
                '------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                ndHeaderRoot = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeaderRoot.InsertAfter(ndHeader, ndHeaderRootHeader)

                'Insert the XSD reference & namespace as an attribute within the root node
                atXmlNsXsi = CreateNamespaceAttribute()
                ndHeaderRoot.Attributes.Append(atXmlNsXsi)
            End If


        End Sub

        Private Function CreateHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    '  ndHeader = .CreateElement("PriceAndAvailability")
                    ndSection = .CreateElement("PriceAndAvailability")
                    ndManufacturerPartNumber = .CreateElement("AlternateSKU")
                    ndManufacturerPartNumberOccurs = .CreateElement("AlternateSKUOccurs")
                    ndVendorNumber = .CreateElement("VendorNumber")
                    ndDescription = .CreateElement("Description")
                    ndReserveInventoryFlag = .CreateElement("ReserveInventoryFlag")
                    ndPrice = .CreateElement("Price")
                    ndCurrencyCode = .CreateElement("CurrencyCode")
                    ndAvailableRebQty = .CreateElement("AvailableRebQty")
                    '    ndBranch = .CreateElement("Branch")
                
                    ndSpecialPriceFlag = .CreateElement("SpecialPriceFlag")
                    '
                    atSku = .CreateAttribute("SKU")
                    atQuantity = .CreateAttribute("Quantity")
                    atID = .CreateAttribute("ID")
                    atName = .CreateAttribute("Name")
                    With ndSection
                        .AppendChild(ndPrice)
                        .AppendChild(ndCurrencyCode)
                        .AppendChild(ndSpecialPriceFlag)
                        .AppendChild(ndManufacturerPartNumber)
                        .AppendChild(ndManufacturerPartNumberOccurs)
                        .AppendChild(ndVendorNumber)
                        .AppendChild(ndDescription)
                        .AppendChild(ndReserveInventoryFlag)
                        .AppendChild(ndAvailableRebQty)
                        ''  .AppendChild(ndBranch)
                      
                    End With
                    ' ndBranch.Attributes.Append(atID)
                    ndSection.Attributes.Append(atSku)
                    ndSection.Attributes.Append(atQuantity)
                    '  ndBranch.Attributes.Append(atName)
                    ndHeader.AppendChild(ndSection)
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-15"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
               ndHeader.AppendChild(ndSection)
            Catch ex As Exception
                Const strError As String = "Failed to Append Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-16"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateWarehouse() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndWHsection = .CreateElement("Branch")
                  
                    ndAvailability = .CreateElement("Availability")
                    ndOnOrder = .CreateElement("OnOrder")
                    ndEtaDate = .CreateElement("ETADate")
                    atWHid = .CreateAttribute("ID")

                    With ndWHsection
                        .AppendChild(ndAvailability)
                        .AppendChild(ndOnOrder)
                        .AppendChild(ndEtaDate)
                    End With
                    ndWHsection.Attributes.Append(atWHid)
                    ndSection.AppendChild(ndWHsection)
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-15"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace