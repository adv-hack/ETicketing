'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Category Details
'
'       Date                        13th Oct 2008
'
'       Author                      Stuart Atkinson
'
'       � CS Group 2008               All rights reserved.
'
'       Error Number Code base      - 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DECategoryDetails

    Private _mode As String = ""
    Private _code As String = ""
    Private _displaySequence As String = ""
    Private _collDEProductDescriptions As New Collection

    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            _code = value
        End Set
    End Property

    Public Property DisplaySequence() As String
        Get
            Return _displaySequence
        End Get
        Set(ByVal value As String)
            _displaySequence = value
        End Set
    End Property

    Public Property CollDEProductDescriptions() As Collection
        Get
            Return _collDEProductDescriptions
        End Get
        Set(ByVal value As Collection)
            _collDEProductDescriptions = value
        End Set
    End Property

End Class
