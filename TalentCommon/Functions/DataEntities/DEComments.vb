'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Talent Order line comments
'
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEComments

    Private _CommentText As String
    Private _Settings As String
    Private _SessionID As String
    Private _CustomerID As String
    Private _CorporatePackageNumericID As String
    Private _ProductCode As String
    Private _Seat As String
    Private _TempBasketID As String
    '---------------------------------------------------------------------------------
    '
    Public Property SessionID As String
        Get
            Return _SessionID
        End Get
        Set(value As String)
            _SessionID = value
        End Set
    End Property
    Public Property Seat As String
        Get
            Return _Seat
        End Get
        Set(value As String)
            _Seat = value
        End Set
    End Property
    Public Property ProductCode As String
        Get
            Return _ProductCode
        End Get
        Set(value As String)
            _ProductCode = value
        End Set
    End Property
    Public Property CorporatePackageNumericID As String
        Get
            Return _CorporatePackageNumericID
        End Get
        Set(value As String)
            _CorporatePackageNumericID = value
        End Set
    End Property
    Public Property CustomerID As String
        Get
            Return _CustomerID
        End Get
        Set(value As String)
            _CustomerID = value
        End Set
    End Property
    Public Property settings As String
        Get
            Return _SessionID
        End Get
        Set(value As String)
            _SessionID = value
        End Set
    End Property
    Public Property setings As String
        Get
            Return _Settings
        End Get
        Set(value As String)
            _Settings = value
        End Set
    End Property

    Public Property CommentText() As String
        Get
            Return _CommentText
        End Get
        Set(value As String)
            _CommentText = value
        End Set
    End Property

    Public Property TempBasketID() As String
        Get
            Return _TempBasketID
        End Get
        Set(value As String)
            _TempBasketID = value
        End Set
    End Property

End Class
