Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Remittances Lines
'
'       Date                        Apr 2007
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEREA- 
'                                    
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DeRemittances
    '
    Private _collDETrans As New Collection              ' Transaction details
    Private _collDeRemittHeader As New Collection       ' Remittance Headers
    Private _collDERemittLines As New Collection        ' Remittance lines
    '
    Public Property CollDETrans() As Collection
        Get
            Return _collDETrans
        End Get
        Set(ByVal value As Collection)
            _collDETrans = value
        End Set
    End Property
    Public Property CollDeRemittHeader() As Collection
        Get
            Return _collDeRemittHeader
        End Get
        Set(ByVal value As Collection)
            _collDeRemittHeader = value
        End Set
    End Property
    Public Property CollDERemittLines() As Collection
        Get
            Return _collDERemittLines
        End Get
        Set(ByVal value As Collection)
            _collDERemittLines = value
        End Set
    End Property
    '
End Class
