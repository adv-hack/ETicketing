Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Menu Items
'
'       Date                        Nov 2006
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'  
'       Error Number Code base      TTPMENU- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Public Class MenuItem

    Private _businessUnit As String = String.Empty
    Private _menuContent As String
    Private _menuCSSClass As String
    Private _menuItem As String
    Private _menuLink As String
    Private _menuName As String
    Private _menuPosition As Integer
    Private _partnerCode As String = String.Empty

    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property
    Public Property MenuContent() As String
        Get
            Return _menuContent
        End Get
        Set(ByVal Value As String)
            _menuContent = Value
        End Set
    End Property
    Public Property MenuCSSClass() As String
        Get
            Return _menuCSSClass
        End Get
        Set(ByVal Value As String)
            _menuCSSClass = Value
        End Set
    End Property
    Public Property MenuItem() As String
        Get
            Return _menuItem
        End Get
        Set(ByVal Value As String)
            _menuItem = Value
        End Set
    End Property
    Public Property MenuLink() As String
        Get
            Return _menuLink
        End Get
        Set(ByVal Value As String)
            _menuLink = Value
        End Set
    End Property
    Public Property MenuName() As String
        Get
            Return _menuName
        End Get
        Set(ByVal Value As String)
            _menuName = Value
        End Set
    End Property
    Public Property MenuPosition() As Integer
        Get
            Return _menuPosition
        End Get
        Set(ByVal Value As Integer)
            _menuPosition = Value
        End Set
    End Property
    Public Property PartnerCode() As String
        Get
            Return _partnerCode
        End Get
        Set(ByVal value As String)
            _partnerCode = value
        End Set
    End Property

End Class
