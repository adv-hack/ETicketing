'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Product Alerts
'
'       Date                        1st Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEPA- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEProductAlert
    '
    Private _collDETrans As New Collection      ' Transaction details
    Private _collDEAlerts As New Collection     ' Product Alert details
    '
    Public Property CollDETrans() As Collection
        Get
            Return _collDETrans
        End Get
        Set(ByVal value As Collection)
            _collDETrans = value
        End Set
    End Property
    Public Property CollDEAlerts() As Collection
        Get
            Return _collDEAlerts
        End Get
        Set(ByVal value As Collection)
            _collDEAlerts = value
        End Set
    End Property

End Class
