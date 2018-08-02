'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Reservation Details
'
'       Date                        4th June 2007
'
'       Author                      Des Webster
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDERS- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEReservationDetails
    '
    Private _sessionId As String = ""
    Private _customerNo As String = ""
    Private _productCode As String = ""
    Private _seatDetails1 As New DESeatDetails
    '
    Public Property SessionId() As String
        Get
            Return _sessionId
        End Get
        Set(ByVal value As String)
            _sessionId = value
        End Set
    End Property
    Public Property CustomerNo() As String
        Get
            Return _customerNo
        End Get
        Set(ByVal value As String)
            _customerNo = value
        End Set
    End Property
    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property
    Public Property SeatDetails1() As DESeatDetails
        Get
            Return _seatDetails1
        End Get
        Set(ByVal value As DESeatDetails)
            _seatDetails1 = value
        End Set
    End Property
End Class