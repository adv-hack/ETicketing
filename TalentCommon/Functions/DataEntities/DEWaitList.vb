<Serializable()> _
Public Class DEWaitList

    Public Enum WaitListType
        Add = 1
        Withdraw = 2
    End Enum

    Private _customerNumber As String
    Public Property CustomerNumber() As String
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As String)
            _customerNumber = value
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


    Private _waitListID As String
    Public Property WaitListID() As String
        Get
            Return _waitListID
        End Get
        Set(ByVal value As String)
            _waitListID = value
        End Set
    End Property

    Private _requestType As WaitListType
    Public Property RequestType() As WaitListType
        Get
            Return _requestType
        End Get
        Set(ByVal value As WaitListType)
            _requestType = value
        End Set
    End Property


    Private _currentSeasonTicketProduct As String
    Public Property CurrentSeasonTicketProduct() As String
        Get
            Return _currentSeasonTicketProduct
        End Get
        Set(ByVal value As String)
            _currentSeasonTicketProduct = value
        End Set
    End Property


    Private _custPhoneNo As String
    Public Property CustomerPhoneNo() As String
        Get
            Return _custPhoneNo
        End Get
        Set(ByVal value As String)
            _custPhoneNo = value
        End Set
    End Property

    Private _custEmail As String
    Public Property CustomerEmailAddress() As String
        Get
            Return _custEmail
        End Get
        Set(ByVal value As String)
            _custEmail = value
        End Set
    End Property

    Private _comment1 As String
    Public Property Comment1() As String
        Get
            Return _comment1
        End Get
        Set(ByVal value As String)
            _comment1 = value
        End Set
    End Property

    Private _comment2 As String
    Public Property Comment2() As String
        Get
            Return _comment2
        End Get
        Set(ByVal value As String)
            _comment2 = value
        End Set
    End Property

    Private _qty As Integer
    Public Property Quantity() As Integer
        Get
            Return _qty
        End Get
        Set(ByVal value As Integer)
            _qty = value
        End Set
    End Property

    Private _preferredStand1 As String
    Public Property PreferredStand1() As String
        Get
            Return _preferredStand1
        End Get
        Set(ByVal value As String)
            _preferredStand1 = value
        End Set
    End Property

    Private _preferredStand2 As String
    Public Property PreferredStand2() As String
        Get
            Return _preferredStand2
        End Get
        Set(ByVal value As String)
            _preferredStand2 = value
        End Set
    End Property

    Private _preferredStand3 As String
    Public Property PreferredStand3() As String
        Get
            Return _preferredStand3
        End Get
        Set(ByVal value As String)
            _preferredStand3 = value
        End Set
    End Property


    Private _preferredArea1 As String
    Public Property PreferredArea1() As String
        Get
            Return _preferredArea1
        End Get
        Set(ByVal value As String)
            _preferredArea1 = value
        End Set
    End Property

    Private _preferredArea2 As String
    Public Property PreferredArea2() As String
        Get
            Return _preferredArea2
        End Get
        Set(ByVal value As String)
            _preferredArea2 = value
        End Set
    End Property

    Private _preferredArea3 As String
    Public Property PreferredArea3() As String
        Get
            Return _preferredArea3
        End Get
        Set(ByVal value As String)
            _preferredArea3 = value
        End Set
    End Property

    Private _WaitListRequests As List(Of String)
    Public Property WaitListRequests() As List(Of String)
        Get
            Return _WaitListRequests
        End Get
        Set(ByVal value As List(Of String))
            _WaitListRequests = value
        End Set
    End Property



    Private _TalentUser As String
    Public Property TalentUser() As String
        Get
            Return _TalentUser
        End Get
        Set(ByVal value As String)
            _TalentUser = value
        End Set
    End Property

    Private _actionComment1 As String
    Public Property ActionComment1() As String
        Get
            Return _actionComment1
        End Get
        Set(ByVal value As String)
            _actionComment1 = value
        End Set
    End Property

    Private _actionComment2 As String
    Public Property ActionComment2() As String
        Get
            Return _actionComment2
        End Get
        Set(ByVal value As String)
            _actionComment2 = value
        End Set
    End Property

    Private _actionComment3 As String
    Public Property ActionComment3() As String
        Get
            Return _actionComment3
        End Get
        Set(ByVal value As String)
            _actionComment3 = value
        End Set
    End Property

    Private _actionComment4 As String
    Public Property ActionComment4() As String
        Get
            Return _actionComment4
        End Get
        Set(ByVal value As String)
            _actionComment4 = value
        End Set
    End Property

    Private _actionComment5 As String
    Public Property ActionComment5() As String
        Get
            Return _actionComment5
        End Get
        Set(ByVal value As String)
            _actionComment5 = value
        End Set
    End Property

    Private _actionComment6 As String
    Public Property ActionComment6() As String
        Get
            Return _actionComment6
        End Get
        Set(ByVal value As String)
            _actionComment6 = value
        End Set
    End Property


    Private _CheckPendingRequests As Boolean
    Public Property CheckPendingRequests() As Boolean
        Get
            Return _CheckPendingRequests
        End Get
        Set(ByVal value As Boolean)
            _CheckPendingRequests = value
        End Set
    End Property

    Public Sub New()
        MyBase.New()

        Me.ActionComment1 = ""
        Me.ActionComment2 = ""
        Me.ActionComment3 = ""
        Me.ActionComment4 = ""
        Me.ActionComment5 = ""
        Me.ActionComment6 = ""
        Me.Comment1 = ""
        Me.Comment2 = ""
        Me.CurrentSeasonTicketProduct = ""
        Me.CustomerEmailAddress = ""
        Me.CustomerNumber = ""
        Me.CustomerPhoneNo = ""
        Me.PreferredArea1 = ""
        Me.PreferredArea2 = ""
        Me.PreferredArea3 = ""
        Me.PreferredStand1 = ""
        Me.PreferredStand2 = ""
        Me.PreferredStand3 = ""
        Me.Quantity = 0
        Me.RequestType = WaitListType.Add
        Me.Src = ""
        Me.TalentUser = ""
        Me.WaitListID = ""
        Me.WaitListRequests = New List(Of String)
    End Sub


End Class
