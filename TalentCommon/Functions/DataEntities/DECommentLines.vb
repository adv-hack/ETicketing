'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Comments
'
'       Date                        1st Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       When passed to System21 must restrict _commentText to max length of 1024
'       so if greater multiple calls will be needed.
'
'       Error Number Code base      TACDECL- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DeCommentLines
    '---------------------------------------------------------------------------------
    '   Comment, AddComment Request Fields
    '
    Private _commentText As String = String.Empty            ' Please pack well, as it is going by airmail
    Private _commentLine As String = String.Empty            ' 2
    Private _commentLineNumber As String = String.Empty      ' 5
    Private _customerLineNumber As String = String.Empty     ' 055
    Private _suffix As String = String.Empty                 ' 13
    Private _westCoastLineNumber As String = String.Empty    '

    Public Property CommentText() As String
        Get
            Return _commentText
        End Get
        Set(ByVal value As String)
            _commentText = value
        End Set
    End Property             ' 
    Public Property CommentLine() As String
        Get
            Return _commentLine
        End Get
        Set(ByVal value As String)
            _commentLine = value
        End Set
    End Property
    Public Property CommentLineNumber() As String
        Get
            Return _commentLineNumber
        End Get
        Set(ByVal value As String)
            _commentLineNumber = value
        End Set
    End Property
    Public Property CustomerLineNumber() As String
        Get
            Return _customerLineNumber
        End Get
        Set(ByVal value As String)
            _customerLineNumber = value
        End Set
    End Property
    Public Property Suffix() As String
        Get
            Return _suffix
        End Get
        Set(ByVal value As String)
            _suffix = value
        End Set
    End Property
    Public Property WestCoastLineNumber() As String
        Get
            Return _westCoastLineNumber
        End Get
        Set(ByVal value As String)
            _westCoastLineNumber = value
        End Set
    End Property

End Class
