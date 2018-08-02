Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Lines
'
'       Date                        28th August 2007
'
'       Author                      Andrew Green
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDETB- 
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DETicketingBasket
    '
    Private _collDETicketingBasket As New Collection
    '
    Public Property CollDETicketingBasket() As Collection
        Get
            Return _collDETicketingBasket
        End Get
        Set(ByVal value As Collection)
            _collDETicketingBasket = value
        End Set
    End Property
End Class
