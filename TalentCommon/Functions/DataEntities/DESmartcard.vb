<Serializable()> _
Public Class DESmartcard

    Private _customerNumber As String
    Public Property CustomerNumber() As String
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As String)
            _customerNumber = value
        End Set
    End Property

    Private _productCode As String
    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property

    Private _printer As String
    Public Property Printer() As String
        Get
            Return _printer
        End Get
        Set(ByVal value As String)
            _printer = value
        End Set
    End Property

    Private _cardNumber As String
    Public Property CardNumber() As String
        Get
            Return _cardNumber
        End Get
        Set(ByVal value As String)
            _cardNumber = value
        End Set
    End Property

    Private _saleRecordID As String
    Public Property SaleRecordID() As String
        Get
            Return _saleRecordID
        End Get
        Set(ByVal value As String)
            _saleRecordID = value
        End Set
    End Property

    Private _cardRecordID As String
    Public Property CardRecordID() As String
        Get
            Return _cardRecordID
        End Get
        Set(ByVal value As String)
            _cardRecordID = value
        End Set
    End Property

    Private _requestType As String
    Public Property RequestType() As String
        Get
            Return _requestType
        End Get
        Set(ByVal value As String)
            _requestType = value
        End Set
    End Property

    Private _source As String
    Public Property Src() As String
        Get
            Return _source
        End Get
        Set(ByVal value As String)
            _source = value
        End Set
    End Property

    Private _MediaType As String
    Public Property MediaType() As String
        Get
            Return _MediaType
        End Get
        Set(ByVal value As String)
            _MediaType = value
        End Set
    End Property

    Private _AgentFilter As Boolean
    Public Property AgentFilter() As Boolean
        Get
            Return _AgentFilter
        End Get
        Set(ByVal value As Boolean)
            _AgentFilter = value
        End Set
    End Property

    Private _InactiveProducts As Boolean
    Public Property InactiveProducts() As Boolean
        Get
            Return _InactiveProducts
        End Get
        Set(ByVal value As Boolean)
            _InactiveProducts = value
        End Set
    End Property

    Private _ActiveProducts As Boolean
    Public Property ActiveProducts() As Boolean
        Get
            Return _ActiveProducts
        End Get
        Set(ByVal value As Boolean)
            _ActiveProducts = value
        End Set
    End Property

    Private _EventProducts As Boolean
    Public Property EventProducts() As Boolean
        Get
            Return _EventProducts
        End Get
        Set(ByVal value As Boolean)
            _EventProducts = value
        End Set
    End Property

    Private _CardProducts As Boolean
    Public Property CardProducts() As Boolean
        Get
            Return _CardProducts
        End Get
        Set(ByVal value As Boolean)
            _CardProducts = value
        End Set
    End Property

    Private _DEXListMode As String
    Public Property DEXListMode() As String
        Get
            Return _DEXListMode
        End Get
        Set(ByVal value As String)
            _DEXListMode = value
        End Set
    End Property

    Private _NewFanId As String
    Public Property NewFanId() As String
        Get
            Return _NewFanId
        End Get
        Set(ByVal value As String)
            _NewFanId = value
        End Set
    End Property

    Public Property New4Digits() As String
    Public Property NewIssueNumber() As String

    Private _OldFanId As String
    Public Property OldFanId() As String
        Get
            Return _OldFanId
        End Get
        Set(ByVal value As String)
            _OldFanId = value
        End Set
    End Property

    Public Property Old4Digits() As String
    Public Property OldIssueNumber() As String
    Public Property SessionID() As String
    Public Property Mode() As String
    Public Property Stand() As String
    Public Property Area() As String
    Public Property Row() As String
    Public Property Seat() As String
    Public Property Alpha() As String
    Public Property PaymentReference() As Integer
    Public Property ExistingCardNumber() As String
    Public Property NewCardNumberID() As String
    Public Property ExternalReprintRequests As List(Of DESmartcardExternalReprint)
    Public Property ErrorFlag() As String
    Public Property ErrorCode() As String
    Public Sub New()
        MyBase.New()
        Me.CustomerNumber = ""
        Me.ProductCode = ""
        Me.Printer = ""
        Me.CardNumber = ""
        Me.RequestType = ""
        Me.SaleRecordID = ""
        Me.CardRecordID = ""
        Me.RequestType = ""
        Me.Src = ""
    End Sub

    Public Function LogString() As String

        Dim sb As New System.Text.StringBuilder
        With sb
            .Append(CustomerNumber & ",")
            .Append(ProductCode & ",")
            .Append(Printer & ",")
            .Append(CardNumber & ",")
            .Append(RequestType & ",")
            .Append(SaleRecordID & ",")
            .Append(CardRecordID & ",")
            .Append(RequestType & ",")
            .Append(Src)
        End With

        Return sb.ToString.Trim

    End Function

End Class

