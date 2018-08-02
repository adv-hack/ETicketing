Imports Microsoft.VisualBasic

Public Class TalentTicketingMenuItem


    Private _id As Long
    Public Property ID() As Long
        Get
            Return _id
        End Get
        Set(ByVal value As Long)
            _id = value
        End Set
    End Property

    Private _bu As String
    Public Property Business_Unit() As String
        Get
            Return _bu
        End Get
        Set(ByVal value As String)
            _bu = value
        End Set
    End Property


    Private _partner As String
    Public Property Partner() As String
        Get
            Return _partner
        End Get
        Set(ByVal value As String)
            _partner = value
        End Set
    End Property

    Private _prodType As String
    Public Property Product_Type() As String
        Get
            Return _prodType
        End Get
        Set(ByVal value As String)
            _prodType = value
        End Set
    End Property

    Private _Language_Code As String
    Public Property Language_Code() As String
        Get
            Return _Language_Code
        End Get
        Set(ByVal value As String)
            _Language_Code = value
        End Set
    End Property

    Private _master As String
    Public Property Master() As String
        Get
            Return _master
        End Get
        Set(ByVal value As String)
            _master = value
        End Set
    End Property

    Private _active As String
    Public Property Active() As String
        Get
            Return _active
        End Get
        Set(ByVal value As String)
            _active = value
        End Set
    End Property

    Private _displayContent As String
    Public Property Display_Content() As String
        Get
            Return _displayContent
        End Get
        Set(ByVal value As String)
            _displayContent = value
        End Set
    End Property

    Private _cssClass As String
    Public Property CSS_Class() As String
        Get
            Return _cssClass
        End Get
        Set(ByVal value As String)
            _cssClass = value
        End Set
    End Property


    Private _navigateUrl As String
    Public Property Navigate_URL() As String
        Get
            Return _navigateUrl
        End Get
        Set(ByVal value As String)
            _navigateUrl = value
        End Set
    End Property

    Private _imageUrl As String
    Public Property Image_URL() As String
        Get
            Return _imageUrl
        End Get
        Set(ByVal value As String)
            _imageUrl = value
        End Set
    End Property

    Private _location As String
    Public Property Location() As String
        Get
            Return _location
        End Get
        Set(ByVal value As String)
            _location = value
        End Set
    End Property

End Class
