<Serializable()> _
Public Class DECreditCheck


    Private _accNo As String
    Public Property AccountNumber() As String
        Get
            Return _accNo
        End Get
        Set(ByVal value As String)
            _accNo = value
        End Set
    End Property

    Sub New(ByVal _accountNumber As String)
        MyBase.New()
        Me.AccountNumber = _accountNumber
    End Sub


    Private _totalOrderValue As Decimal
    Public Property TotalOrderValue() As Decimal
        Get
            Return _totalOrderValue
        End Get
        Set(ByVal value As Decimal)
            _totalOrderValue = value
        End Set
    End Property


End Class
