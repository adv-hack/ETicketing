Public Enum LoyaltyPointsType
    Adhoc
    Attendance
End Enum

<Serializable()> _
Public Class DELoyalty

    Private _customerNumber As String = String.Empty
    Public Property CustomerNumber() As String
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As String)
            _customerNumber = value
        End Set
    End Property

    Private _productCode As String = String.Empty
    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property

    Private _theDate As Date = Date.MinValue
    Public Property TheDate() As Date
        Get
            Return _theDate
        End Get
        Set(ByVal value As Date)
            _theDate = value
        End Set
    End Property

    Private _theTime As String
    Public Property TheTime() As String
        Get
            Return _theTime
        End Get
        Set(ByVal value As String)
            _theTime = value
        End Set
    End Property

    Private _stand As String = String.Empty
    Public Property Stand() As String
        Get
            Return _stand
        End Get
        Set(ByVal value As String)
            _stand = value
        End Set
    End Property

    Private _area As String = String.Empty
    Public Property Area() As String
        Get
            Return _area
        End Get
        Set(ByVal value As String)
            _area = value
        End Set
    End Property

    Private _row As String = String.Empty
    Public Property Row() As String
        Get
            Return _row
        End Get
        Set(ByVal value As String)
            _row = value
        End Set
    End Property

    Private _seat As String = String.Empty
    Public Property Seat() As String
        Get
            Return _seat
        End Get
        Set(ByVal value As String)
            _seat = value
        End Set
    End Property

    Private _type As LoyaltyPointsType = LoyaltyPointsType.Adhoc
    Public Property Type() As LoyaltyPointsType
        Get
            Return _type
        End Get
        Set(ByVal value As LoyaltyPointsType)
            _type = value
        End Set
    End Property

    Private _points As Integer = 0
    Public Property Points() As Integer
        Get
            Return _points
        End Get
        Set(ByVal value As Integer)
            _points = value
        End Set
    End Property

End Class
