Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access Flash Settings related data table objects
    ''' </summary>
    <Serializable()> _
        Public Class Flash

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings

        Private _tblFlashSettings As tbl_flash_settings

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
        Public ReadOnly Property TblFlashSettings() As tbl_flash_settings
            Get
                If (_tblFlashSettings Is Nothing) Then
                    _tblFlashSettings = New tbl_flash_settings(_settings)
                End If
                Return _tblFlashSettings
            End Get
        End Property
#End Region

    End Class

End Namespace
