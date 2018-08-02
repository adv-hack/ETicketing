Imports Talent.Common.DataObjects.TableObjects
Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access Flash Settings related data table objects
    ''' </summary>
    <Serializable()> _
        Public Class Version
#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblVersionDeployed As tbl_version_deployed
#End Region
#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Flash" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region
#Region "Properties"
        ''' <summary>
        ''' Create and Gets the tbl_flash_settings instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblVersionDeployed() As tbl_version_deployed
            Get
                If (_tblVersionDeployed Is Nothing) Then
                    _tblVersionDeployed = New tbl_version_deployed(_settings)
                End If
                Return _tblVersionDeployed
            End Get
        End Property
#End Region
    End Class
End Namespace
