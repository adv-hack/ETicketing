Imports System.Data.SqlClient
Imports System.Text
Imports Talent.Common.DataObjects.TalentDefinitions.TableObjects

Namespace DataObjects

    <Serializable()> _
    Public Class AgentAuthorityGroups
        Inherits TalentBase

#Region "Class Level Fields"

        ' ''' <summary>
        ' ''' DESettings Instance
        ' ''' </summary>
        'Private _settings As New DESettings
        Private _tblAgentGroups As tbl_agent_groups
        Private _tblAgentPermissions As tbl_agent_permissions

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "AgentAuthorityGroups"

#End Region

#Region "Constructors"
        Sub New()

        End Sub

        'Sub New(ByVal settings As DESettings)
        '    _settings = settings
        'End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property TblAgentGroups() As tbl_agent_groups
        Get
            If (_tblAgentGroups Is Nothing) Then
                _tblAgentGroups = New tbl_agent_groups(Settings)
            End If
            Return _tblAgentGroups
        End Get
    End Property

    Public ReadOnly Property TblAgentPermissions() As tbl_agent_permissions
        Get
            If (_tblAgentPermissions Is Nothing) Then
                _tblAgentPermissions = New tbl_agent_permissions(Settings)
            End If
            Return _tblAgentPermissions
        End Get
    End Property

    'Public Property Settings() As DESettings
    '    Get
    '        Return _settings
    '    End Get
    '    Set(value As DESettings)
    '        _settings = value
    '    End Set
    'End Property

#End Region

    End Class

End Namespace


