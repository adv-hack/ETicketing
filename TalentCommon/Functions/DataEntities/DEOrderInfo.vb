Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Lines
'
'       Date                        6th Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEOI- 
'                                   application.code(3) + object code(4) + number(2)
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEOrderInfo
    '
    Private _collDECommentLines As New Collection   ' Order comment lines
    Private _collDEProductLines As New Collection   ' Order product lines
    '
    Public Property CollDECommentLines() As Collection
        Get
            Return _collDECommentLines
        End Get
        Set(ByVal value As Collection)
            _collDECommentLines = value
        End Set
    End Property
    Public Property CollDEProductLines() As Collection
        Get
            Return _collDEProductLines
        End Get
        Set(ByVal value As Collection)
            _collDEProductLines = value
        End Set
    End Property
    '
End Class
