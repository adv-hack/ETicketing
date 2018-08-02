'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Purchase Details
'
'       Date                        June 2007
'
'       Author                      Des Webster
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEPU- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEPurchaseDetails
    '---------------------------------------------------------------------------------
    '
    Private _paymentReference As Long = 0
    Private _customerNumber As String = String.Empty
    Private _totalPrice As String = String.Empty

    Public Property PaymentReference() As Long
        Get
            Return _paymentReference
        End Get
        Set(ByVal value As Long)
            _paymentReference = value
        End Set
    End Property
   
    Public Property CustomerNumber() As Long
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As Long)
            _customerNumber = value
        End Set
    End Property

    Public Property TotalPrice() As String
        Get
            Return _totalPrice
        End Get
        Set(ByVal value As String)
            _totalPrice = value
        End Set
    End Property
End Class