Imports System.Data.SqlClient
Imports Talent.Common.DataObjects.TableObjects
Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access clubs details and relations to loginID
    ''' </summary>
    <Serializable()> _
        Public Class Sports
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblSportBu As tbl_sport_bu
        Private _tblSportTeamBu As tbl_sport_team_bu
        Private _tblSportTeamClubBu As tbl_sport_team_club_bu

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "Sports"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Sports" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Create and Gets the tbl_sport_bu instance with DESettings
        ''' </summary>
        ''' <value>tbl_sport_bu instance</value>
        Public ReadOnly Property TblSportBu() As tbl_sport_bu
            Get
                If (_tblSportBu Is Nothing) Then
                    _tblSportBu = New tbl_sport_bu(_settings)
                End If
                Return _tblSportBu
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_sport_team_bu instance with DESettings
        ''' </summary>
        ''' <value>tbl_sport_team_bu instance</value>
        Public ReadOnly Property TblSportTeamBu() As tbl_sport_team_bu
            Get
                If (_tblSportTeamBu Is Nothing) Then
                    _tblSportTeamBu = New tbl_sport_team_bu(_settings)
                End If
                Return _tblSportTeamBu
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_sport_team_club_bu instance with DESettings
        ''' </summary>
        ''' <value>tbl_sport_team_club_bu instance</value>
        Public ReadOnly Property TblSportTeamClubBu() As tbl_sport_team_club_bu
            Get
                If (_tblSportTeamClubBu Is Nothing) Then
                    _tblSportTeamClubBu = New tbl_sport_team_club_bu(_settings)
                End If
                Return _tblSportTeamClubBu
            End Get
        End Property
#End Region

#Region "Public Methods"


#End Region
    End Class
End Namespace
