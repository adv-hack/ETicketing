'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with PurchaseOrder
'
'       Date                        Feb 2007
'
'       Author                      
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEPO- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEPurchaseOrder

    Private _collDETrans As New Collection          ' Transaction details

    Public Property CollDETrans() As Collection
        Get
            Return _collDETrans
        End Get
        Set(ByVal value As Collection)
            _collDETrans = value
        End Set
    End Property

End Class
