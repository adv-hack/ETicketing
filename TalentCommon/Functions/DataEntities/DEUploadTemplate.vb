'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Amend Template Requests
'
'       Date                        310707
'
'       Author                      Bne
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEUT- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEUploadTemplates
    '
    Private _collDETrans As New Collection      ' Transaction details
    Private _collDEAlerts As New Collection     ' Amend Template details
    '
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
