Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Data.SqlClient
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    ProductPrice
'
'       Date                        220207
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCTALLST- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       
'--------------------------------------------------------------------------------------------------

Namespace Talent.eCommerce

    Public Class ProductPrice

        '    Public Class Price

        '        Private _fromDate As String
        '        Private _toDate As String
        '        Private _netPrice As Decimal
        '        Private _grossPrice As Decimal
        '        Private _taxAmount As Decimal
        '        Private _saleNetPrice As Decimal
        '        Private _saleGrossPrice As Decimal
        '        Private _saleTaxAmount As Decimal
        '        Private _deliveryNetPrice As Decimal
        '        Private _deliveryGrossPrice As Decimal
        '        Private _deliveryTaxAmount As Decimal
        '        Private _price1 As Decimal
        '        Private _price2 As Decimal
        '        Private _price3 As Decimal
        '        Private _price4 As Decimal
        '        Private _price5 As Decimal
        '        Private _taxCode As String

        '        Public Property FromDate() As String
        '            Get
        '                Return _fromDate
        '            End Get
        '            Set(ByVal value As String)
        '                _fromDate = value
        '            End Set
        '        End Property

        '        Public Property ToDate() As String
        '            Get
        '                Return _toDate
        '            End Get
        '            Set(ByVal value As String)
        '                _toDate = value
        '            End Set
        '        End Property

        '        Public Property NetPrice() As Decimal
        '            Get
        '                Return _netPrice
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _netPrice = value
        '            End Set
        '        End Property

        '        Public Property GrossPrice() As Decimal
        '            Get
        '                Return _grossPrice
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _grossPrice = value
        '            End Set
        '        End Property

        '        Public Property TaxAmount() As Decimal
        '            Get
        '                Return _taxAmount
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _taxAmount = value
        '            End Set
        '        End Property

        '        Public Property SaleNetPrice() As Decimal
        '            Get
        '                Return _saleNetPrice
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _saleNetPrice = value
        '            End Set
        '        End Property

        '        Public Property SaleGrossPrice() As Decimal
        '            Get
        '                Return _saleGrossPrice
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _saleGrossPrice = value
        '            End Set
        '        End Property

        '        Public Property SaleTaxAmount() As Decimal
        '            Get
        '                Return _saleTaxAmount
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _saleTaxAmount = value
        '            End Set
        '        End Property

        '        Public Property DeliveryNetPrice() As Decimal
        '            Get
        '                Return _deliveryNetPrice
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _deliveryNetPrice = value
        '            End Set
        '        End Property

        '        Public Property DeliveryGrossPrice() As Decimal
        '            Get
        '                Return _deliveryGrossPrice
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _deliveryGrossPrice = value
        '            End Set
        '        End Property

        '        Public Property DeliveryTaxAmount() As Decimal
        '            Get
        '                Return _deliveryTaxAmount
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _deliveryTaxAmount = value
        '            End Set
        '        End Property

        '        Public Property Price1() As Decimal
        '            Get
        '                Return _price1
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _price1 = value
        '            End Set
        '        End Property

        '        Public Property Price2() As Decimal
        '            Get
        '                Return _price2
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _price2 = value
        '            End Set
        '        End Property

        '        Public Property Price3() As Decimal
        '            Get
        '                Return _price3
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _price3 = value
        '            End Set
        '        End Property

        '        Public Property Price4() As Decimal
        '            Get
        '                Return _price4
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _price4 = value
        '            End Set
        '        End Property

        '        Public Property Price5() As Decimal
        '            Get
        '                Return _price5
        '            End Get
        '            Set(ByVal value As Decimal)
        '                _price5 = value
        '            End Set
        '        End Property

        '        Public Property TaxCode() As String
        '            Get
        '                Return _taxCode
        '            End Get
        '            Set(ByVal value As String)
        '                _taxCode = value
        '            End Set
        '        End Property
        '        Public Sub New()
        '        End Sub

        '    End Class

        '    Private Shared _PricingStructure As String
        '    Private Shared _Application As String

        '    Public Shared WriteOnly Property PricingStructure() As String
        '        Set(ByVal value As String)
        '            _PricingStructure = value
        '        End Set
        '    End Property

        '    Public Shared WriteOnly Property Application() As String
        '        Set(ByVal value As String)
        '            _Application = value
        '        End Set
        '    End Property
        '    '---------------------
        '    ' Function (Get_Price) 
        '    '---------------------
        '    Public Shared Function Get_Price(ByVal Product As String) As Price
        '        Dim obPrice As New Price
        '        '--------------------------
        '        ' validate input parameters
        '        '--------------------------
        '        If Product = String.Empty Then
        '            obPrice = Nothing
        '        Else
        '            obPrice = ReadProduct(Product)
        '        End If
        '        Return obPrice
        '    End Function
        '    '---------------------
        '    ' Function (Get_Price) 
        '    '---------------------
        '    Public Shared Function Get_Price(ByVal Product As String, ByVal PriceList As String) As Price
        '        Dim obPrice As New Price
        '        '--------------------------
        '        ' validate input parameters
        '        '--------------------------
        '        If Product = String.Empty Then
        '            obPrice = Nothing
        '        Else
        '            obPrice = ReadProduct(Product, PriceList)
        '        End If
        '        Return obPrice
        '    End Function
        '    '-----------------------------
        '    ' Function (Get_Price_And_Tax) 
        '    '-----------------------------
        '    Public Shared Function Get_Price_And_Tax(ByVal Product As String) As Price

        '        Dim obPrice As New Price
        '        '--------------------------
        '        ' validate input parameters
        '        '--------------------------
        '        If Product = String.Empty Then
        '            obPrice = Nothing
        '        Else
        '            obPrice = ReadProduct(Product)
        '            obPrice.TaxAmount = CalculateTax(obPrice.GrossPrice, obPrice.TaxCode)
        '            obPrice.SaleTaxAmount = CalculateTax(obPrice.SaleGrossPrice, obPrice.TaxCode)
        '        End If
        '        Return obPrice

        '    End Function
        '    '-----------------------------
        '    ' Function (Get_Price_And_Tax) 
        '    '-----------------------------
        '    Public Shared Function Get_Price_And_Tax(ByVal Product As String, ByVal PriceList As String) As Price

        '        Dim obPrice As New Price
        '        '--------------------------
        '        ' validate input parameters
        '        '--------------------------
        '        If Product = String.Empty Then
        '            obPrice = Nothing
        '        Else
        '            obPrice = ReadProduct(Product, PriceList)
        '            obPrice.TaxAmount = CalculateTax(obPrice.GrossPrice, obPrice.TaxCode)
        '            obPrice.SaleTaxAmount = CalculateTax(obPrice.SaleGrossPrice, obPrice.TaxCode)
        '        End If
        '        Return obPrice
        '    End Function
        '    '------------------------
        '    ' Function (Get_Line_Tax) 
        '    '------------------------
        '    Public Shared Function Get_Line_Tax(ByVal Product As String, ByVal qty As Integer) As Price

        '        Dim obPrice As New Price
        '        '--------------------------
        '        ' validate input parameters
        '        '--------------------------
        '        If Product = String.Empty Then
        '            obPrice = Nothing
        '        Else
        '            obPrice = ReadProduct(Product)
        '            obPrice.TaxAmount = CalculateTax(obPrice.GrossPrice * qty, obPrice.TaxCode)
        '            obPrice.SaleTaxAmount = CalculateTax(obPrice.SaleGrossPrice * qty, obPrice.TaxCode)
        '        End If

        '        Return obPrice
        '    End Function
        '    '------------------
        '    ' Calculate the Tax
        '    '------------------
        '    Private Shared Function CalculateTax(ByVal price As Double, ByVal taxCode As String) As Double

        '        Dim conTalent As SqlConnection = Nothing
        '        Dim cmdSelect As SqlCommand = Nothing
        '        Dim dtrTaxCode As SqlDataReader = Nothing
        '        Dim tax As Double = 0

        '        Try
        '            Dim SQLString As String = ("Select TAX_PERCENTAGE From TBL_TAX_CODE Where TAX_CODE = @TAX_CODE")

        '            conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings("TALENT_SQL_ConnectionString").ConnectionString)
        '            cmdSelect = New SqlCommand(SQLString, conTalent)

        '            cmdSelect.Parameters.Add(New SqlParameter("@TAX_CODE", SqlDbType.Char, 20)).Value = taxCode.Trim

        '            conTalent.Open()
        '            dtrTaxCode = cmdSelect.ExecuteReader()
        '        Catch ex As Exception
        '        End Try

        '        Try
        '            If dtrTaxCode.HasRows Then

        '                dtrTaxCode.Read()

        '                ' Pricing Structure 1
        '                Select Case _PricingStructure
        '                    Case "1"
        '                        Dim taxRate As Double = CType(dtrTaxCode("TAX_PERCENTAGE"), Double)
        '                        'HttpContext.Current.Session("TaxRate") = taxRate
        '                        tax = (price / 100) * taxRate
        '                End Select

        '            End If
        '        Catch ex As Exception
        '        End Try

        '        Try
        '            dtrTaxCode.Close()
        '            conTalent.Close()
        '        Catch ex As Exception
        '        End Try

        '        Return tax

        '    End Function
        '    '-----------------
        '    ' Read the Product
        '    '-----------------
        '    Private Shared Function ReadProduct(ByVal product As String) As Price

        '        Dim obPrice As New Price
        '        Dim conTalent As SqlConnection = Nothing
        '        Dim cmdSelect As SqlCommand = Nothing
        '        Dim dtrPrice As SqlDataReader = Nothing

        '        Try
        '            Dim SQLString As String = "Select * From TBL_PRICE_LIST_DETAIL where PRODUCT = @PRODUCT"
        '            conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ConnectionString)
        '            cmdSelect = New SqlCommand(SQLString, conTalent)

        '            cmdSelect.Parameters.Add(New SqlParameter("@PRODUCT", SqlDbType.Char, 20)).Value = product
        '            conTalent.Open()
        '            dtrPrice = cmdSelect.ExecuteReader()
        '        Catch ex As Exception
        '        End Try

        '        Try
        '            If dtrPrice.HasRows Then
        '                dtrPrice.Read()

        '                obPrice.FromDate = dtrPrice("FROM_DATE").ToString.Trim
        '                obPrice.ToDate = dtrPrice("TO_DATE").ToString.Trim
        '                obPrice.NetPrice = CDec(dtrPrice("NET_PRICE"))
        '                obPrice.GrossPrice = CDec(dtrPrice("GROSS_PRICE"))
        '                obPrice.TaxAmount = CDec(dtrPrice("TAX_AMOUNT"))
        '                obPrice.SaleNetPrice = CDec(dtrPrice("SALE_NET_PRICE"))
        '                obPrice.SaleGrossPrice = CDec(dtrPrice("SALE_GROSS_PRICE"))
        '                obPrice.SaleTaxAmount = CDec(dtrPrice("SALE_TAX_AMOUNT"))
        '                obPrice.DeliveryNetPrice = CDec(dtrPrice("DELIVERY_NET_PRICE"))
        '                obPrice.DeliveryGrossPrice = CDec(dtrPrice("DELIVERY_GROSS_PRICE"))
        '                obPrice.DeliveryTaxAmount = CDec(dtrPrice("DELIVERY_TAX_AMOUNT"))
        '                obPrice.Price1 = CDec(dtrPrice("PRICE_1"))
        '                obPrice.Price2 = CDec(dtrPrice("PRICE_2"))
        '                obPrice.Price3 = CDec(dtrPrice("PRICE_3"))
        '                obPrice.Price4 = CDec(dtrPrice("PRICE_4"))
        '                obPrice.Price5 = CDec(dtrPrice("PRICE_5"))
        '                obPrice.TaxCode = dtrPrice("TAX_CODE")

        '            End If
        '        Catch ex As Exception
        '        End Try

        '        Try
        '            dtrPrice.Close()
        '            conTalent.Close()
        '        Catch ex As Exception
        '        End Try

        '        Return obPrice

        '    End Function
        '    '-----------------
        '    ' Read the Product
        '    '-----------------
        '    Private Shared Function ReadProduct(ByVal product As String, ByVal PriceList As String) As Price

        '        Dim obPrice As New Price
        '        Dim conTalent As SqlConnection = Nothing
        '        Dim cmdSelect As SqlCommand = Nothing
        '        Dim dtrPrice As SqlDataReader = Nothing

        '        Try
        '            Dim SQLString As String = "Select * From TBL_PRICE_LIST_DETAIL where PRODUCT = @PRODUCT and PRICE_LIST = @PRICE_LIST"

        '            conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ConnectionString)
        '            cmdSelect = New SqlCommand(SQLString, conTalent)

        '            cmdSelect.Parameters.Add(New SqlParameter("@PRODUCT", SqlDbType.Char, 20)).Value = product
        '            cmdSelect.Parameters.Add(New SqlParameter("@PRICE_LIST", SqlDbType.Char, 20)).Value = PriceList

        '            conTalent.Open()
        '            dtrPrice = cmdSelect.ExecuteReader()
        '        Catch ex As Exception
        '        End Try

        '        Try
        '            If dtrPrice.HasRows Then

        '                dtrPrice.Read()

        '                obPrice.FromDate = dtrPrice("FROM_DATE").ToString.Trim
        '                obPrice.ToDate = dtrPrice("TO_DATE").ToString.Trim
        '                obPrice.NetPrice = CDec(dtrPrice("NET_PRICE"))
        '                obPrice.GrossPrice = CDec(dtrPrice("GROSS_PRICE"))
        '                obPrice.TaxAmount = CDec(dtrPrice("TAX_AMOUNT"))
        '                obPrice.SaleNetPrice = CDec(dtrPrice("SALE_NET_PRICE"))
        '                obPrice.SaleGrossPrice = CDec(dtrPrice("SALE_GROSS_PRICE"))
        '                obPrice.SaleTaxAmount = CDec(dtrPrice("SALE_TAX_AMOUNT"))
        '                obPrice.DeliveryNetPrice = CDec(dtrPrice("DELIVERY_NET_PRICE"))
        '                obPrice.DeliveryGrossPrice = CDec(dtrPrice("DELIVERY_GROSS_PRICE"))
        '                obPrice.DeliveryTaxAmount = CDec(dtrPrice("DELIVERY_TAX_AMOUNT"))
        '                obPrice.Price1 = CDec(dtrPrice("PRICE_1"))
        '                obPrice.Price2 = CDec(dtrPrice("PRICE_2"))
        '                obPrice.Price3 = CDec(dtrPrice("PRICE_3"))
        '                obPrice.Price4 = CDec(dtrPrice("PRICE_4"))
        '                obPrice.Price5 = CDec(dtrPrice("PRICE_5"))
        '                obPrice.TaxCode = dtrPrice("TAX_CODE")

        '            End If
        '        Catch ex As Exception
        '        End Try

        '        Try
        '            dtrPrice.Close()
        '            conTalent.Close()
        '        Catch ex As Exception
        '        End Try

        '        Return obPrice

        '    End Function

    End Class

End Namespace

