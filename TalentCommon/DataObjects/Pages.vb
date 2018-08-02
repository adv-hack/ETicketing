Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access Page Settings related data table objects
    ''' </summary>
    <Serializable()> _
        Public Class Pages

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings

        Private _tblPage As tbl_page
        Private _tblPageAttribute As tbl_page_attribute
        Private _tblPageControl As tbl_page_control
        Private _tblPageExtraData As tbl_page_extra_data
        Private _tblPageHtml As tbl_page_html
        Private _tblPageLang As tbl_page_lang
        Private _tblPageLeftNav As tbl_page_left_nav
        Private _tblPageTextLang As tbl_page_text_lang
        Private _tblVouchersExternal As tbl_vouchers_external

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Pages" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

        ''' <summary>
        ''' Create and Gets the tbl_page instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblPage() As tbl_page
            Get
                If (_tblPage Is Nothing) Then
                    _tblPage = New tbl_page(_settings)
                End If
                Return _tblPage
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_page_attribute instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblPageAttribute() As tbl_page_attribute
            Get
                If (_tblPageAttribute Is Nothing) Then
                    _tblPageAttribute = New tbl_page_attribute(_settings)
                End If
                Return _tblPageAttribute
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_vouchers_external instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblVouchersExternal() As tbl_vouchers_external
            Get
                If (_tblVouchersExternal Is Nothing) Then
                    _tblVouchersExternal = New tbl_vouchers_external(_settings)
                End If
                Return _tblVouchersExternal
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_page_control instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblPageControl() As tbl_page_control
            Get
                If (_tblPageControl Is Nothing) Then
                    _tblPageControl = New tbl_page_control(_settings)
                End If
                Return _tblPageControl
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_page_extra_data instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblPageExtraData() As tbl_page_extra_data
            Get
                If (_tblPageExtraData Is Nothing) Then
                    _tblPageExtraData = New tbl_page_extra_data(_settings)
                End If
                Return _tblPageExtraData
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_page_html instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblPageHtml() As tbl_page_html
            Get
                If (_tblPageHtml Is Nothing) Then
                    _tblPageHtml = New tbl_page_html(_settings)
                End If
                Return _tblPageHtml
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_page_lang instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblPageLang() As tbl_page_lang
            Get
                If (_tblPageLang Is Nothing) Then
                    _tblPageLang = New tbl_page_lang(_settings)
                End If
                Return _tblPageLang
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_page_left_nav instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblPageLeftNav() As tbl_page_left_nav
            Get
                If (_tblPageLeftNav Is Nothing) Then
                    _tblPageLeftNav = New tbl_page_left_nav(_settings)
                End If
                Return _tblPageLeftNav
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_page_text_lang instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblPageTextLang() As tbl_page_text_lang
            Get
                If (_tblPageTextLang Is Nothing) Then
                    _tblPageTextLang = New tbl_page_text_lang(_settings)
                End If
                Return _tblPageTextLang
            End Get
        End Property

#End Region

    End Class

End Namespace
