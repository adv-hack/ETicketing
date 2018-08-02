<Serializable()> _
Public Class DEProductInfo

    Private _bu As String
    Public Property BUSINESS_UNIT() As String
        Get
            Return _bu
        End Get
        Set(ByVal value As String)
            _bu = value
        End Set
    End Property

    Private _ptner As String
    Public Property PARTNER() As String
        Get
            Return _ptner
        End Get
        Set(ByVal value As String)
            _ptner = value
        End Set
    End Property


    Private _languageCode As String
    Public Property LANGUAGE_CODE() As String
        Get
            Return _languageCode
        End Get
        Set(ByVal value As String)
            _languageCode = value
        End Set
    End Property

    Private _productCodes As ArrayList
    Public Property Product_Codes() As ArrayList
        Get
            Return _productCodes
        End Get
        Set(ByVal value As ArrayList)
            _productCodes = value
        End Set
    End Property

    Public Sub New(ByVal businessUnit As String, ByVal partner As String, ByVal productCodes As ArrayList, ByVal languageCode As String)
        MyBase.New()
        Me._productCodes = New ArrayList
        Me.BUSINESS_UNIT = businessUnit
        Me.PARTNER = partner
        Me._productCodes = productCodes
        Me.LANGUAGE_CODE = languageCode
    End Sub

    Public Sub New(ByVal businessUnit As String, ByVal partner As String, ByVal productCode As String, ByVal languageCode As String)
        MyBase.New()
        Me._productCodes = New ArrayList
        Me.BUSINESS_UNIT = businessUnit
        Me.PARTNER = partner
        Me._productCodes.Add(productCode)
        Me.LANGUAGE_CODE = languageCode
    End Sub

End Class
