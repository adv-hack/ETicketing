'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Refund Payment
'
'       Date                        August 2007
'
'       Author                      Andrew Green
'
'       � CS Group 2007             All rights reserved.
'
'       When passed to System21 must restrict _PaymentType to max length of 1024
'       so if greater multiple calls will be needed.
'
'       Error Number Code base      TACDERPY- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DERefundPayment
    '---------------------------------------------------------------------------------
    '
    Private _paymentReference As String = String.Empty
    Private _refundCustomerNo As String = String.Empty
    Private _paymentDetails As New DEPayments
    Private _collRefundItems As New Collection
    Private _src As String = String.Empty
    '
    Public Property PaymentReference() As String
        Get
            Return _paymentReference
        End Get
        Set(ByVal value As String)
            _paymentReference = value
        End Set
    End Property
    Public Property RefundCustomerNo() As String
        Get
            Return _refundCustomerNo
        End Get
        Set(ByVal value As String)
            _refundCustomerNo = value
        End Set
    End Property
    Public Property PaymentDetails() As DEPayments
        Get
            Return _paymentDetails
        End Get
        Set(ByVal value As DEPayments)
            _paymentDetails = value
        End Set
    End Property
    Public Property CollRefundItems() As Collection
        Get
            Return _collRefundItems
        End Get
        Set(ByVal value As Collection)
            _collRefundItems = value
        End Set
    End Property
    Public Property Src() As String
        Get
            Return _src
        End Get
        Set(ByVal value As String)
            _src = value
        End Set
    End Property

    Public Property CancelMode() As String

    Public Function LogString() As String

        Dim sb As New System.Text.StringBuilder
        Dim deri As DERefundItem
        With sb
            .Append(PaymentReference & ",")
            .Append(RefundCustomerNo & ",")
            .Append(PaymentDetails.LogString)
            For Each deri In CollRefundItems
                deri.LogString()
            Next
            .Append(Src)
        End With

        Return sb.ToString.Trim

    End Function
End Class
