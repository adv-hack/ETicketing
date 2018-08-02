'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Product Alerts
'
'       Date                        18/05/07
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      - 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEPNARequest
    '
    Private _collDETrans As New Collection      ' Transaction details
    Private _collDEAlerts As New Collection     ' Price and availability requests
    Private _priceUrl As String                 ' Price Url String
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
    Public Property PriceUrl() As String
        Get
            Return _priceUrl
        End Get
        Set(ByVal value As String)
            _priceUrl = value
        End Set
    End Property

End Class
