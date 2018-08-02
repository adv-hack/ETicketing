'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with the transaction section of
'                                   the input XML document
'
'       Date                        1st Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDETR- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DETransaction
    ' 
    Private _company As String = String.Empty                 ' CSG
    Private _countryCode As String = String.Empty             ' UK
    Private _loginId As String = String.Empty                 ' stuart
    Private _password As String = String.Empty                ' stuart
    Private _receiverID As String = String.Empty              ' 987654321
    Private _senderID As String = String.Empty                ' 123456789
    Private _showDetail As String = String.Empty              '
    Private _transactionID As String = String.Empty           ' 12345
    '
    Public Property Company() As String
        Get
            Return _company
        End Get
        Set(ByVal value As String)
            _company = value
        End Set
    End Property
    Public Property CountryCode() As String
        Get
            Return _countryCode
        End Get
        Set(ByVal value As String)
            _countryCode = value
        End Set
    End Property
    Public Property loginId() As String
        Get
            Return _loginId
        End Get
        Set(ByVal value As String)
            _loginId = value
        End Set
    End Property
    Public Property Password() As String
        Get
            Return _password
        End Get
        Set(ByVal value As String)
            _password = value
        End Set
    End Property
    Public Property ReceiverID() As String
        Get
            Return _receiverID
        End Get
        Set(ByVal value As String)
            _receiverID = value
        End Set
    End Property
    Public Property SenderID() As String
        Get
            Return _senderID
        End Get
        Set(ByVal value As String)
            _senderID = value
        End Set
    End Property
    Public Property ShowDetail() As String
        Get
            Return _showDetail
        End Get
        Set(ByVal value As String)
            _showDetail = value
        End Set
    End Property
    Public Property TransactionID() As String
        Get
            Return _transactionID
        End Get
        Set(ByVal value As String)
            _transactionID = value
        End Set
    End Property
    '
End Class
