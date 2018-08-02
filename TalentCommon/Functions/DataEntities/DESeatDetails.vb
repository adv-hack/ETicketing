'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Seat Details
'
'       Date                        4th June 2007
'
'       Author                      Des Webster
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDERS- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DESeatDetails
    '
    Private _stand As String = ""
    Private _area As String = ""
    Private _row As String = ""
    Private _seat As String = ""
    Private _alphaSuffix As String = ""
    Private _standDescription As String = ""
    Private _areaDescription As String = ""
    Private _seatStringMask As String = ""
    Private _formattedSeat As String = ""
    Private _stringMaskFormattedSeat As String = ""
    Private _catSeatStatus As String = ""
    Private _valueArray() As String
    Private _valueList As New List(Of String)
    '
    Public Property FormattedSeat() As String
        Get
            'Format the seat from the local variables
            If Stand.Trim <> "" Then
                _formattedSeat = _stand.Trim & "/" & _area.Trim & "/" & _row.Trim & "/" & _seat.Trim
                If _alphaSuffix.Trim <> "" Then
                    _formattedSeat = _formattedSeat.Trim & "/" & _alphaSuffix.Trim
                End If
            Else
                _formattedSeat = ""
            End If
            'Return the formatted seat
            Return _formattedSeat
        End Get
        Set(ByVal value As String)
            _valueList = New List(Of String)

            'Is the value blank
            If value.Trim = "" Then
                _stand = ""
                _area = ""
                _row = ""
                _seat = ""
                _alphaSuffix = ""
                _formattedSeat = ""
                _catSeatStatus = ""
            Else
                'Create array of values, then convert to list
                If value.Contains("/") Then
                    _valueArray = value.Split("/")
                    For Each v As String In _valueArray
                        _valueList.Add(v)
                    Next

                    'Pad with empty values if too short
                    While _valueList.Count < 5
                        _valueList.Add("")
                    End While

                    'Assign variables
                    _stand = _valueList(0).ToString
                    _area = _valueList(1).ToString
                    _row = _valueList(2).ToString
                    _seat = _valueList(3).ToString
                    _alphaSuffix = _valueList(4).ToString

                    'Putting a failsafe in for alpha seats when the format comes in as 'IPS/IPSA/A/0001A' opposed to 'IPS/IPSA/A/0001/A'
                    If _valueList(3).ToString.Length = 5 AndAlso _valueList(4).ToString.Trim Is String.Empty Then
                        _seat = _valueList(3).ToString.Substring(0, 4)
                        _alphaSuffix = _valueList(3).ToString.Substring(4, 1)
                    End If
                Else
                    _seat = value
                End If
            End If
        End Set
    End Property
    Public Property StringMaskFormattedSeat() As String
        Get
            'Format the seat from the local variables
            If _seatStringMask.Trim <> "" Then
                _stringMaskFormattedSeat = String.Format(_seatStringMask, _stand, _area, _row, _seat, _standDescription, AreaDescription)
            Else
                _stringMaskFormattedSeat = String.Format("{0}/{1}/{2}/{3}", _stand, _area, _row, _seat)
            End If
            'Return the string mask formatted seat
            Return _stringMaskFormattedSeat
        End Get
        Set(ByVal value As String)
        End Set
    End Property
    Public Property SeatStringMask() As String
        Get
            Return _seatStringMask
        End Get
        Set(ByVal value As String)
            _seatStringMask = value
        End Set
    End Property
    Public Property Stand() As String
        Get
            Return _stand
        End Get
        Set(ByVal value As String)
            _stand = value
        End Set
    End Property
    Public Property Area() As String
        Get
            Return _area
        End Get
        Set(ByVal value As String)
            _area = value
        End Set
    End Property
    Public Property Row() As String
        Get
            Return _row
        End Get
        Set(ByVal value As String)
            _row = value
        End Set
    End Property
    Public Property Seat() As String
        Get
            Return _seat
        End Get
        Set(ByVal value As String)
            _seat = value
        End Set
    End Property
    Public Property AlphaSuffix() As String
        Get
            Return _alphaSuffix
        End Get
        Set(ByVal value As String)
            _alphaSuffix = value
        End Set
    End Property
    Public Property StandDescription() As String
        Get
            Return _standDescription
        End Get
        Set(ByVal value As String)
            _standDescription = value
        End Set
    End Property
    Public Property AreaDescription() As String
        Get
            Return _areaDescription
        End Get
        Set(ByVal value As String)
            _areaDescription = value
        End Set
    End Property
    Public Property CATSeatStatus() As String
        Get
            Return _catSeatStatus
        End Get
        Set(ByVal value As String)
            _catSeatStatus = value
        End Set
    End Property

    Public Property ProductCode() As String
    Public Property Quantity() As String
    Public Property CustomerNumber() As String
    Public Property PriceCode() As String
    Public Property PriceBand() As String

    ''' <summary>
    ''' Set the public properties based on an unformatted seat string. Eg. "S  SA  A   0001 "
    ''' </summary>
    ''' <value>The value to format</value>
    ''' <returns>The formatted values</returns>
    ''' <remarks></remarks>
    Public Property UnFormattedSeat() As String
        Get
            Return Utilities.FixStringLength(_stand, 3) &
                    Utilities.FixStringLength(_area, 4) &
                    Utilities.FixStringLength(_row, 4) &
                    Utilities.FixStringLength(_seat, 4) & _alphaSuffix
        End Get
        Set(value As String)
            If value.Trim = "" Then
                _stand = ""
                _area = ""
                _row = ""
                _seat = ""
                _alphaSuffix = ""
                _formattedSeat = ""
                _catSeatStatus = ""
            Else
                _stand = value.Substring(0, 3)
                _area = value.Substring(3, 4)
                _row = value.Substring(7, 4)
                _seat = value.Substring(11, 4)
                If value.Length > 15 Then
                    _alphaSuffix = value.Substring(15, 1)
                End If
            End If
        End Set
    End Property
    Public Function LogString() As String

        Dim sb As New System.Text.StringBuilder

        With sb
            .Append(Stand & ",")
            .Append(Area & ",")
            .Append(Row & ",")
            .Append(Seat & ",")
            .Append(AlphaSuffix & ",")
            .Append(FormattedSeat & ",")
        End With

        Return sb.ToString.Trim

    End Function
End Class
