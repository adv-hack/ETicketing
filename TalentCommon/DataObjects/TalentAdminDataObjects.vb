Imports System.Data.SqlClient
Imports Talent.Common.Dataobjects

''' <summary>
''' OBSOLETE CLASS, DONT USE IT
''' Creates and provides the instances to access the TalentAdminDB data objects
''' </summary>
<Serializable()> _
Public Class TalentAdminDataObjects
    Inherits DBObjectBase

#Region "Class Level Fields"
    ''' <summary>
    ''' DESettings instance
    ''' </summary>
    Private _settings As New DESettings

    Private _talentAdmin As TalentAdmin

    'Used for logging
    Private Const SOURCECLASS As String = "TALENTADMINDATAOBJECTS"

#End Region

#Region "Properties"
    ''' <summary>
    ''' Gets or sets the settings value from DESettings
    ''' </summary>
    ''' <value>The settings.</value>
    Public Property Settings() As DESettings
        Get
            Return _settings
        End Get
        Set(ByVal value As DESettings)
            _settings = value
        End Set
    End Property

    ''' <summary>
    ''' Gets the TalentAdmin settings.
    ''' </summary>
    ''' <value>The TalentAdmin settings.</value>
    Public ReadOnly Property TalentAdminSettings() As TalentAdmin
        Get
            If (_talentAdmin Is Nothing) Then
                _talentAdmin = New TalentAdmin(_settings)
            End If
            Return _talentAdmin
        End Get
    End Property

#End Region

End Class
