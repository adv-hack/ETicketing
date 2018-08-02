Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access Fees Settings related data table objects
    ''' </summary>
    <Serializable()> _
    Public Class Fees

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings

        Private _tblFeesRelations As tbl_fees_relations

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Fees" /> class.
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
        Public ReadOnly Property TblFeesRelations() As tbl_fees_relations
            Get
                If (_tblFeesRelations Is Nothing) Then
                    _tblFeesRelations = New tbl_fees_relations(_settings)
                End If
                Return _tblFeesRelations
            End Get
        End Property

#End Region

    End Class

End Namespace
