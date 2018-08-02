'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with language text
'
'       Date                        6th Dec 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEDS- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEDescriptions
    '---------------------------------------------------------------------------------
    '   Comment, AddComment Request Fields
    '
    Private _code As String = String.Empty                   ' 
    Private _description As String = String.Empty            ' 
    Private _key As String = String.Empty                    ' 
    Private _language As String = String.Empty               ' 
    Private _type As String = String.Empty                   ' 

    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            _code = value
        End Set
    End Property
    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property
    Public Property Key() As String
        Get
            Return _key
        End Get
        Set(ByVal value As String)
            _key = value
        End Set
    End Property                     ' 
    Public Property Language() As String
        Get
            Return _language
        End Get
        Set(ByVal value As String)
            _language = value
        End Set
    End Property                ' 
    Public Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
        End Set
    End Property

End Class
