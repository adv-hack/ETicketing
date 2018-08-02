Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Remittance Lines
'
'       Date                        Apr 2007
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEREL- 
'                                    
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DeRemittanceLines
    '
    Private _remittanceHeader As String = String.Empty
    Private _remittanceLine As String = String.Empty
    Private _lineNumber As String = String.Empty
    Private _masterItemType As String = String.Empty
    Private _ledgerEntryDocumentReference As String = String.Empty
    Private _postingAmountPrime As Decimal = 0
    Private _discountAmountPrime As Decimal = 0
    Private _suppliersReference As String = String.Empty

    Public Property RemittanceHeader() As String
        Get
            Return _remittanceHeader
        End Get
        Set(ByVal value As String)
            _remittanceHeader = value
        End Set
    End Property
    Public Property RemittanceLine() As String
        Get
            Return _remittanceLine
        End Get
        Set(ByVal value As String)
            _remittanceLine = value
        End Set
    End Property
    Public Property LineNumber() As String
        Get
            Return _lineNumber
        End Get
        Set(ByVal value As String)
            _lineNumber = value
        End Set
    End Property
    Public Property MasterItemType() As String
        Get
            Return _masterItemType
        End Get
        Set(ByVal value As String)
            _masterItemType = value
        End Set
    End Property
    Public Property LedgerEntryDocumentReference() As String
        Get
            Return _ledgerEntryDocumentReference
        End Get
        Set(ByVal value As String)
            _ledgerEntryDocumentReference = value
        End Set
    End Property
    Public Property PostingAmountPrime() As Decimal
        Get
            Return _postingAmountPrime
        End Get
        Set(ByVal value As Decimal)
            _postingAmountPrime = value
        End Set
    End Property
    Public Property DiscountAmountPrime() As Decimal
        Get
            Return _discountAmountPrime
        End Get
        Set(ByVal value As Decimal)
            _discountAmountPrime = value
        End Set
    End Property
    Public Property SuppliersReference() As String
        Get
            Return _suppliersReference
        End Get
        Set(ByVal value As String)
            _suppliersReference = value
        End Set
    End Property
    '
End Class
