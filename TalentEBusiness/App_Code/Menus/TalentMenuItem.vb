Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce

Public Class TalentMenuItem

    Private _id As Long
    Public Property ID() As Long
        Get
            Return _id
        End Get
        Set(ByVal value As Long)
            _id = value
        End Set
    End Property


    Private _menuId As String
    Public Property Menu_ID() As String
        Get
            Return _menuId
        End Get
        Set(ByVal value As String)
            _menuId = value
        End Set
    End Property

    Private _menuItemId As String
    Public Property Menu_Item_ID() As String
        Get
            Return _menuItemId
        End Get
        Set(ByVal value As String)
            _menuItemId = value
        End Set
    End Property

    Private _menuseq As Integer
    Public Property Menu_Sequence() As Integer
        Get
            Return _menuseq
        End Get
        Set(ByVal value As Integer)
            _menuseq = value
        End Set
    End Property


    Private _langCode As String
    Public Property Language_Code() As String
        Get
            Return _langCode
        End Get
        Set(ByVal value As String)
            _langCode = value
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
    Public Property Css_Class() As String
        Get
            Return _cssClass
        End Get
        Set(ByVal value As String)
            _cssClass = value
        End Set
    End Property

    Private _Image_Url As String
    Public Property Image_URL() As String
        Get
            Return _Image_Url
        End Get
        Set(ByVal value As String)
            _Image_Url = value
        End Set
    End Property

    Private _navUrl As String
    Public Property Navigate_Url() As String
        Get
            Return _navUrl
        End Get
        Set(ByVal value As String)
            _navUrl = value
        End Set
    End Property

End Class
