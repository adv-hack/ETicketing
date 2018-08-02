'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with product requests and responses
'
'       Date                        1st Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEPL- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DeProductLines
    '
    Private _alternateSKU As String = String.Empty              ' 123A321A
    Private _category As String = String.Empty                  ' invoice, Delivery, Shipping etc.  
    Private _customerLineNumber As String = String.Empty        ' 
    Private _sKU As String = String.Empty                       ' 123A321
    Private _quantity As Double = 0                             ' 1
    Private _reserveCode As String = String.Empty               ' C 
    Private _reserveSequence As String = String.Empty           ' 01
    Private _shipFromBranch As String = String.Empty
    Private _shipFromWarehouse As String = String.Empty
    Private _suffix As String = String.Empty
    Private _westCoastLineNumber As String = String.Empty       '
    Private _fixedPrice As String = String.Empty                '
    Private _fixedPriceNet As String = String.Empty                '
    Private _fixedPriceTax As String = String.Empty                '
    Private _cancellationCode As String = String.Empty                '
    Private _productDescription As String = String.Empty
    Private _netLineValue As Decimal = 0
    Private _taxLineValue As Decimal = 0
    Private _grossLineValue As Decimal = 0
    Private _productTaxValue As Decimal = 0
    Private _taxCode1 As String = String.Empty
    Private _taxCode2 As String = String.Empty
    Private _taxCode3 As String = String.Empty
    Private _taxCode4 As String = String.Empty
    Private _taxCode5 As String = String.Empty
    Private _taxAmount1 As Decimal = 0
    Private _taxAmount2 As Decimal = 0
    Private _taxAmount3 As Decimal = 0
    Private _taxAmount4 As Decimal = 0
    Private _taxAmount5 As Decimal = 0
    Private _currency As String = String.Empty
    Private _costCentre As String = String.Empty
    Private _accountCode As String = String.Empty

    Private _lineComment As String = String.Empty                '


    Public Property AlternateSKU() As String
        Get
            Return _alternateSKU
        End Get
        Set(ByVal value As String)
            _alternateSKU = value
        End Set
    End Property
    Public Property Category() As String
        Get
            Return _category
        End Get
        Set(ByVal value As String)
            _category = value
        End Set
    End Property
    Public Property CustomerLineNumber() As String
        Get
            Return _customerLineNumber
        End Get
        Set(ByVal value As String)
            _customerLineNumber = value
        End Set
    End Property
    Public Property SKU() As String
        Get
            Return _sKU
        End Get
        Set(ByVal value As String)
            _sKU = value
        End Set
    End Property
    Public Property Quantity() As Double
        Get
            Return _quantity
        End Get
        Set(ByVal value As Double)
            _quantity = value
        End Set
    End Property
    Public Property FixedPrice() As String
        Get
            Return _fixedPrice
        End Get
        Set(ByVal value As String)
            _fixedPrice = value
        End Set
    End Property
    Public Property FixedPriceNet() As String
        Get
            Return _fixedPriceNet
        End Get
        Set(ByVal value As String)
            _fixedPriceNet = value
        End Set
    End Property
    Public Property FixedPriceTax() As String
        Get
            Return _fixedPriceTax
        End Get
        Set(ByVal value As String)
            _fixedPriceTax = value
        End Set
    End Property
    Public Property ReserveCode() As String
        Get
            Return _reserveCode
        End Get
        Set(ByVal value As String)
            _reserveCode = value
        End Set
    End Property
    Public Property ReserveSequence() As String
        Get
            Return _reserveSequence
        End Get
        Set(ByVal value As String)
            _reserveSequence = value
        End Set
    End Property
    Public Property ShipFromBranch() As String
        Get
            Return _shipFromBranch
        End Get
        Set(ByVal value As String)
            _shipFromBranch = value
        End Set
    End Property
    Public Property ShipFromWarehouse() As String
        Get
            Return _shipFromWarehouse
        End Get
        Set(ByVal value As String)
            _shipFromWarehouse = value
        End Set
    End Property
    Public Property Suffix() As String
        Get
            Return _suffix
        End Get
        Set(ByVal value As String)
            _suffix = value
        End Set
    End Property
    Public Property WestCoastLineNumber() As String
        Get
            Return _westCoastLineNumber
        End Get
        Set(ByVal value As String)
            _westCoastLineNumber = value
        End Set
    End Property
    Public Property CancellationCode() As String
        Get
            Return _cancellationCode
        End Get
        Set(ByVal value As String)
            _cancellationCode = value
        End Set
    End Property

    Public Property LineComment() As String
        Get
            Return _lineComment
        End Get
        Set(ByVal value As String)
            _lineComment = value
        End Set
    End Property

    Public Property ProductDescription() As String
        Get
            Return _productDescription
        End Get
        Set(ByVal value As String)
            _productDescription = value
        End Set
    End Property

    Public Property NetLineValue() As Decimal
        Get
            Return _netLineValue
        End Get
        Set(ByVal value As Decimal)
            _netLineValue = value
        End Set
    End Property

    Public Property TaxLineValue() As Decimal
        Get
            Return _taxLineValue
        End Get
        Set(ByVal value As Decimal)
            _taxLineValue = value
        End Set
    End Property

    Public Property GrossLineValue() As Decimal
        Get
            Return _grossLineValue
        End Get
        Set(ByVal value As Decimal)
            _grossLineValue = value
        End Set
    End Property

    Public Property ProductTaxValue() As Decimal
        Get
            Return _productTaxValue
        End Get
        Set(ByVal value As Decimal)
            _productTaxValue = value
        End Set
    End Property

    Public Property TaxCode1() As String
        Get
            Return _taxCode1
        End Get
        Set(ByVal value As String)
            _taxCode1 = value
        End Set
    End Property
    Public Property TaxCode2() As String
        Get
            Return _taxCode2
        End Get
        Set(ByVal value As String)
            _taxCode2 = value
        End Set
    End Property
    Public Property TaxCode3() As String
        Get
            Return _taxCode3
        End Get
        Set(ByVal value As String)
            _taxCode3 = value
        End Set
    End Property
    Public Property TaxCode4() As String
        Get
            Return _taxCode4
        End Get
        Set(ByVal value As String)
            _taxCode4 = value
        End Set
    End Property
    Public Property TaxCode5() As String
        Get
            Return _taxCode5
        End Get
        Set(ByVal value As String)
            _taxCode5 = value
        End Set
    End Property

    Public Property TaxAmount1() As Decimal
        Get
            Return _taxAmount1
        End Get
        Set(ByVal value As Decimal)
            _taxAmount1 = value
        End Set
    End Property

    Public Property TaxAmount2() As Decimal
        Get
            Return _taxAmount2
        End Get
        Set(ByVal value As Decimal)
            _taxAmount2 = value
        End Set
    End Property

    Public Property TaxAmount3() As Decimal
        Get
            Return _taxAmount3
        End Get
        Set(ByVal value As Decimal)
            _taxAmount3 = value
        End Set
    End Property

    Public Property TaxAmount4() As Decimal
        Get
            Return _taxAmount4
        End Get
        Set(ByVal value As Decimal)
            _taxAmount4 = value
        End Set
    End Property

    Public Property TaxAmount5() As Decimal
        Get
            Return _taxAmount5
        End Get
        Set(ByVal value As Decimal)
            _taxAmount5 = value
        End Set
    End Property

    Public Property Currency() As String
        Get
            Return _currency
        End Get
        Set(ByVal value As String)
            _currency = value
        End Set
    End Property


    Private _promoValue As Decimal
    Public Property PromotionValue() As Decimal
        Get
            Return _promoValue
        End Get
        Set(ByVal value As Decimal)
            _promoValue = value
        End Set
    End Property

    Private _promoPercentage As Decimal
    Public Property PromotionPercentage() As Decimal
        Get
            Return _promoPercentage
        End Get
        Set(ByVal value As Decimal)
            _promoPercentage = value
        End Set
    End Property


    Private _lineError As Boolean = False
    Public Property LineError() As Boolean
        Get
            Return _lineError
        End Get
        Set(ByVal value As Boolean)
            _lineError = value
        End Set
    End Property

    Private _lineErrorMessage As String = String.Empty
    Public Property LineErrorMessage() As String
        Get
            Return _lineErrorMessage
        End Get
        Set(ByVal value As String)
            _lineErrorMessage = value
        End Set
    End Property
    Private _extensionReference1 As String
    Public Property ExtensionReference1() As String
        Get
            Return _extensionReference1
        End Get
        Set(ByVal value As String)
            _extensionReference1 = value
        End Set
    End Property
    Private _extensionReference2 As String
    Public Property ExtensionReference2() As String
        Get
            Return _extensionReference2
        End Get
        Set(ByVal value As String)
            _extensionReference2 = value
        End Set
    End Property
    Private _extensionReference3 As String
    Public Property ExtensionReference3() As String
        Get
            Return _extensionReference3
        End Get
        Set(ByVal value As String)
            _extensionReference3 = value
        End Set
    End Property
    Private _extensionReference4 As String
    Public Property ExtensionReference4() As String
        Get
            Return _extensionReference4
        End Get
        Set(ByVal value As String)
            _extensionReference4 = value
        End Set
    End Property
    Private _extensionReference5 As String
    Public Property ExtensionReference5() As String
        Get
            Return _extensionReference5
        End Get
        Set(ByVal value As String)
            _extensionReference5 = value
        End Set
    End Property
    Private _extensionReference6 As String
    Public Property ExtensionReference6() As String
        Get
            Return _extensionReference6
        End Get
        Set(ByVal value As String)
            _extensionReference6 = value
        End Set
    End Property
    Private _extensionReference7 As String
    Public Property ExtensionReference7() As String
        Get
            Return _extensionReference7
        End Get
        Set(ByVal value As String)
            _extensionReference7 = value
        End Set
    End Property
    Private _extensionReference8 As String
    Public Property ExtensionReference8() As String
        Get
            Return _extensionReference8
        End Get
        Set(ByVal value As String)
            _extensionReference8 = value
        End Set
    End Property
    Private _extensionFlag1 As String
    Public Property ExtensionFlag1() As String
        Get
            Return _extensionFlag1
        End Get
        Set(ByVal value As String)
            _extensionFlag1 = value
        End Set
    End Property
    Private _extensionFlag2 As String
    Public Property ExtensionFlag2() As String
        Get
            Return _extensionFlag2
        End Get
        Set(ByVal value As String)
            _extensionFlag2 = value
        End Set
    End Property
    Private _extensionFlag3 As String
    Public Property ExtensionFlag3() As String
        Get
            Return _extensionFlag3
        End Get
        Set(ByVal value As String)
            _extensionFlag3 = value
        End Set
    End Property
    Private _extensionFlag4 As String
    Public Property ExtensionFlag4() As String
        Get
            Return _extensionFlag4
        End Get
        Set(ByVal value As String)
            _extensionFlag4 = value
        End Set
    End Property
    Private _extensionFlag5 As String
    Public Property ExtensionFlag5() As String
        Get
            Return _extensionFlag5
        End Get
        Set(ByVal value As String)
            _extensionFlag5 = value
        End Set
    End Property
    Private _extensionFlag6 As String
    Public Property ExtensionFlag6() As String
        Get
            Return _extensionFlag6
        End Get
        Set(ByVal value As String)
            _extensionFlag6 = value
        End Set
    End Property
    Private _extensionFlag7 As String
    Public Property ExtensionFlag7() As String
        Get
            Return _extensionFlag7
        End Get
        Set(ByVal value As String)
            _extensionFlag7 = value
        End Set
    End Property
    Private _extensionFlag8 As String
    Public Property ExtensionFlag8() As String
        Get
            Return _extensionFlag8
        End Get
        Set(ByVal value As String)
            _extensionFlag8 = value
        End Set
    End Property
    Private _extensionFlag9 As String
    Public Property ExtensionFlag9() As String
        Get
            Return _extensionFlag9
        End Get
        Set(ByVal value As String)
            _extensionFlag9 = value
        End Set
    End Property
    Private _extensionFlag0 As String
    Public Property ExtensionFlag0() As String
        Get
            Return _extensionFlag0
        End Get
        Set(ByVal value As String)
            _extensionFlag0 = value
        End Set
    End Property

    Private _extensionField1 As String
    Public Property ExtensionField1() As String
        Get
            Return _extensionField1
        End Get
        Set(ByVal value As String)
            _extensionField1 = value
        End Set
    End Property
    Private _extensionField2 As String
    Public Property ExtensionField2() As String
        Get
            Return _extensionField2
        End Get
        Set(ByVal value As String)
            _extensionField2 = value
        End Set
    End Property
    Private _extensionField3 As String
    Public Property ExtensionField3() As String
        Get
            Return _extensionField3
        End Get
        Set(ByVal value As String)
            _extensionField3 = value
        End Set
    End Property
    Private _extensionField4 As String
    Public Property ExtensionField4() As String
        Get
            Return _extensionField4
        End Get
        Set(ByVal value As String)
            _extensionField4 = value
        End Set
    End Property

    Private _extensionFixedPrice1 As Decimal
    Public Property ExtensionFixedPrice1() As Decimal
        Get
            Return _extensionFixedPrice1
        End Get
        Set(ByVal value As Decimal)
            _extensionFixedPrice1 = value
        End Set
    End Property

    Private _extensionFixedPrice2 As Decimal
    Public Property ExtensionFixedPrice2() As Decimal
        Get
            Return _extensionFixedPrice2
        End Get
        Set(ByVal value As Decimal)
            _extensionFixedPrice2 = value
        End Set
    End Property
    Private _extensionFixedPrice3 As Decimal
    Public Property ExtensionFixedPrice3() As Decimal
        Get
            Return _extensionFixedPrice3
        End Get
        Set(ByVal value As Decimal)
            _extensionFixedPrice3 = value
        End Set
    End Property
    Private _extensionFixedPrice4 As Decimal
    Public Property ExtensionFixedPrice4() As Decimal
        Get
            Return _extensionFixedPrice4
        End Get
        Set(ByVal value As Decimal)
            _extensionFixedPrice4 = value
        End Set
    End Property
    Private _extensionFixedPrice5 As Decimal
    Public Property ExtensionFixedPrice5() As Decimal
        Get
            Return _extensionFixedPrice5
        End Get
        Set(ByVal value As Decimal)
            _extensionFixedPrice5 = value
        End Set
    End Property
    Private _extensionFixedPrice6 As Decimal
    Public Property ExtensionFixedPrice6() As Decimal
        Get
            Return _extensionFixedPrice6
        End Get
        Set(ByVal value As Decimal)
            _extensionFixedPrice6 = value
        End Set
    End Property
    Private _extensionFixedPrice7 As Decimal
    Public Property ExtensionFixedPrice7() As Decimal
        Get
            Return _extensionFixedPrice7
        End Get
        Set(ByVal value As Decimal)
            _extensionFixedPrice7 = value
        End Set
    End Property
    Private _extensionFixedPrice8 As Decimal
    Public Property ExtensionFixedPrice8() As Decimal
        Get
            Return _extensionFixedPrice8
        End Get
        Set(ByVal value As Decimal)
            _extensionFixedPrice8 = value
        End Set
    End Property

    Private _extensionDealID1 As String
    Public Property ExtensionDealID1() As String
        Get
            Return _extensionDealID1
        End Get
        Set(ByVal value As String)
            _extensionDealID1 = value
        End Set
    End Property

    Private _extensionDealID2 As String
    Public Property ExtensionDealID2() As String
        Get
            Return _extensionDealID2
        End Get
        Set(ByVal value As String)
            _extensionDealID2 = value
        End Set
    End Property
    Private _extensionDealID3 As String
    Public Property ExtensionDealID3() As String
        Get
            Return _extensionDealID3
        End Get
        Set(ByVal value As String)
            _extensionDealID3 = value
        End Set
    End Property
    Private _extensionDealID4 As String
    Public Property ExtensionDealID4() As String
        Get
            Return _extensionDealID4
        End Get
        Set(ByVal value As String)
            _extensionDealID4 = value
        End Set
    End Property
    Private _extensionDealID5 As String
    Public Property ExtensionDealID5() As String
        Get
            Return _extensionDealID5
        End Get
        Set(ByVal value As String)
            _extensionDealID5 = value
        End Set
    End Property
    Private _extensionDealID6 As String
    Public Property ExtensionDealID6() As String
        Get
            Return _extensionDealID6
        End Get
        Set(ByVal value As String)
            _extensionDealID6 = value
        End Set
    End Property
    Private _extensionDealID7 As String
    Public Property ExtensionDealID7() As String
        Get
            Return _extensionDealID7
        End Get
        Set(ByVal value As String)
            _extensionDealID7 = value
        End Set
    End Property
    Private _extensionDealID8 As String
    Public Property ExtensionDealID8() As String
        Get
            Return _extensionDealID8
        End Get
        Set(ByVal value As String)
            _extensionDealID8 = value
        End Set
    End Property

    Private _extensionStatus As String
    Public Property ExtensionStatus() As String
        Get
            Return _extensionStatus
        End Get
        Set(ByVal value As String)
            _extensionStatus = value
        End Set
    End Property

    Public Property CostCentre() As String
        Get
            Return _costCentre
        End Get
        Set(ByVal value As String)
            _costCentre = value
        End Set
    End Property

    Public Property AccountCode() As String
        Get
            Return _accountCode
        End Get
        Set(ByVal value As String)
            _accountCode = value
        End Set
    End Property
    Public Property lineDelDate() As New Date
    Public Property SetlineDelDate() As Boolean = False
End Class

