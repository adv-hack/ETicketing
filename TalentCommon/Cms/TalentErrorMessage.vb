Public Class TalentErrorMessage


    Private _id As Long
    Public Property ID() As Long
        Get
            Return _id
        End Get
        Set(ByVal value As Long)
            _id = value
        End Set
    End Property

    Private _langCode As String
    Public Property LANGUAGE_CODE() As String
        Get
            Return _langCode
        End Get
        Set(ByVal value As String)
            _langCode = value
        End Set
    End Property

    Private _bu As String
    Public Property BUSINESS_UNIT() As String
        Get
            Return _bu
        End Get
        Set(ByVal value As String)
            _bu = value
        End Set
    End Property

    Private _part As String
    Public Property PARTNER_CODE() As String
        Get
            Return _part
        End Get
        Set(ByVal value As String)
            _part = value
        End Set
    End Property

    Private _module As String
    Public Property MODULE_() As String
        Get
            Return _module
        End Get
        Set(ByVal value As String)
            _module = value
        End Set
    End Property

    Private _page As String
    Public Property PAGE_CODE() As String
        Get
            Return _page
        End Get
        Set(ByVal value As String)
            _page = value
        End Set
    End Property

    Private _errcode As String
    Public Property ERROR_CODE() As String
        Get
            Return _errcode
        End Get
        Set(ByVal value As String)
            _errcode = value
        End Set
    End Property

    Private _errmsg As String = ""
    Public Property ERROR_MESSAGE() As String
        Get
            Return _errmsg
        End Get
        Set(ByVal value As String)
            _errmsg = value
        End Set
    End Property

    Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Use this new to define an Error Message Object to pass to the 
    ''' </summary>
    ''' <param name="myLanguageCode"></param>
    ''' <param name="myBusinessUnit"></param>
    ''' <param name="myPartner"></param>
    ''' <param name="myModule"></param>
    ''' <param name="myPageCode"></param>
    ''' <param name="myErrorCode"></param>
    ''' <remarks></remarks>
    Sub New(ByVal myLanguageCode As String, _
            ByVal myBusinessUnit As String, _
            ByVal myPartner As String, _
            ByVal myModule As String, _
            ByVal myPageCode As String, _
            ByVal myErrorCode As String)

        MyBase.New()
        Me.LANGUAGE_CODE = myLanguageCode
        Me.BUSINESS_UNIT = myBusinessUnit
        Me.PARTNER_CODE = myPartner
        Me.MODULE_ = myModule
        Me.PAGE_CODE = myPageCode
        Me.ERROR_CODE = myErrorCode

    End Sub

End Class
