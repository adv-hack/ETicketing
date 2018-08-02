Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access templates data related table objects
    ''' </summary>
    <Serializable()> _
        Public Class Templates

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings

        Private _tblTemplatePageData As tbl_template_page
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Templates" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

        ''' <summary>
        ''' Create and Gets the tbl_template_page instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblTemplatePage() As tbl_template_page
            Get
                If (_tblTemplatePageData Is Nothing) Then
                    _tblTemplatePageData = New tbl_template_page(_settings)
                End If
                Return _tblTemplatePageData
            End Get
        End Property
#End Region

    End Class

End Namespace
