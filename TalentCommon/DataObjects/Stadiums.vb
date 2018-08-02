Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access Stadium Settings related data table objects
    ''' </summary>
    <Serializable()> _
    Public Class Stadiums

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings

        Private _tblStadiums As tbl_stadiums
        Private _tblStadiumAreaColours As tbl_stadium_area_colours
        Private _tblStadiumSeatColours As tbl_stadium_seat_colours
        Private _tblStadiumOverride As tbl_stadium_override

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Stadiums" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

        ''' <summary>
        ''' Create and Gets the tbl_stadiums instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblStadiums() As tbl_stadiums
            Get
                If (_tblStadiums Is Nothing) Then
                    _tblStadiums = New tbl_stadiums(_settings)
                End If
                Return _tblStadiums
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_stadium_area_colours instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblStadiumAreaColours() As tbl_stadium_area_colours
            Get
                If (_tblStadiumAreaColours Is Nothing) Then
                    _tblStadiumAreaColours = New tbl_stadium_area_colours(_settings)
                End If
                Return _tblStadiumAreaColours
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_stadium_seat_colours instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblStadiumSeatColours() As tbl_stadium_seat_colours
            Get
                If (_tblStadiumSeatColours Is Nothing) Then
                    _tblStadiumSeatColours = New tbl_stadium_seat_colours(_settings)
                End If
                Return _tblStadiumSeatColours
            End Get
        End Property


        ''' <summary>
        ''' Create and Gets the tbl_stadium_override instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblStadiumOverride() As tbl_stadium_override
            Get
                If (_tblStadiumOverride Is Nothing) Then
                    _tblStadiumOverride = New tbl_stadium_override(_settings)
                End If
                Return _tblStadiumOverride
            End Get
        End Property
#End Region

    End Class

End Namespace
