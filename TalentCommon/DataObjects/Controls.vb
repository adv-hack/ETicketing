Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access Controls data related table objects
    ''' </summary>
    <Serializable()> _
        Public Class Controls

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings

        Private _tblControlAttribute As tbl_control_attribute
        Private _tblControlTextLang As tbl_control_text_lang

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Controls" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

        ''' <summary>
        ''' Create and Gets the tbl_control_attribute instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblControlAttribute() As tbl_control_attribute
            Get
                If (_tblControlAttribute Is Nothing) Then
                    _tblControlAttribute = New tbl_control_attribute(_settings)
                End If
                Return _tblControlAttribute
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_control_text_lang instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblControlTextLang() As tbl_control_text_lang
            Get
                If (_tblControlTextLang Is Nothing) Then
                    _tblControlTextLang = New tbl_control_text_lang(_settings)
                End If
                Return _tblControlTextLang
            End Get
        End Property

#End Region

    End Class

End Namespace
