Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access Talent Maintenance specific tables
    ''' </summary>
    <Serializable()> _
    Public Class Maintenance

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings

        Private _tblMaintenanceModules As tbl_maintenance_modules
        Private _tblMaintenancePublishTypes As tbl_maintenance_publish_types

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Maintenance" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

        ''' <summary>
        ''' Create and Gets the tbl_maintenance_modules instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblMaintenanceModules() As tbl_maintenance_modules
            Get
                If (_tblMaintenanceModules Is Nothing) Then
                    _tblMaintenanceModules = New tbl_maintenance_modules(_settings)
                End If
                Return _tblMaintenanceModules
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_maintenance_modules instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblMaintenancePublishTypes() As tbl_maintenance_publish_types
            Get
                If (_tblMaintenancePublishTypes Is Nothing) Then
                    _tblMaintenancePublishTypes = New tbl_maintenance_publish_types(_settings)
                End If
                Return _tblMaintenancePublishTypes
            End Get
        End Property

#End Region

    End Class
End Namespace