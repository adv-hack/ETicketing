Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with the whole Order
'
'       Date                        6th Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEOO- 
'                                   application.code(3) + object code(4) + number(2)
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DeOrders
    '
    Private _deOrderHeader As DeOrderHeader     ' Order header info
    Private _deOrderInfo As DEOrderInfo         ' Order lines info
    Private _showDetail As String
    '
    Public Property DEOrderHeader() As DeOrderHeader
        Get
            Return _deOrderHeader
        End Get
        Set(ByVal value As DeOrderHeader)
            _deOrderHeader = value
        End Set
    End Property
    Public Property DEOrderInfo() As DEOrderInfo
        Get
            Return _deOrderInfo
        End Get
        Set(ByVal value As DEOrderInfo)
            _deOrderInfo = value
        End Set
    End Property
    Public Property ShowDetail() As String
        Get
            Return _showDetail
        End Get
        Set(ByVal value As String)
            _showDetail = value
        End Set
    End Property
    '
End Class
