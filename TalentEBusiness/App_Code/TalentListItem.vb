Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    User Control - Page Menu Collection
'
'       Date                        Jan 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCTALLST- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Public Class TalentListItem

    Private _Text As String
    Private _ClickURL As String
    Private _ImageURL As String
    Private _CssClass As String

    Public Property Text() As String
        Get
            Return _Text
        End Get
        Set(ByVal value As String)
            _Text = value
        End Set
    End Property
    Public Property ClickURL() As String
        Get
            Return _ClickURL
        End Get
        Set(ByVal value As String)
            _ClickURL = value
        End Set
    End Property
    Public Property ImageURL() As String
        Get
            Return _ImageURL
        End Get
        Set(ByVal value As String)
            _ImageURL = value
        End Set
    End Property
    Public Property CssClass() As String
        Get
            Return _CssClass
        End Get
        Set(ByVal value As String)
            _CssClass = value
        End Set
    End Property

End Class
