Imports System.Data.SqlClient
Imports Talent.Common.DataObjects.TableObjects
Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access Feeds details
    ''' </summary>
    <Serializable()> _
    Public Class Feeds
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _feedsEntity As New DEFeeds
        Private _tblFeedsTemplate As tbl_feeds_template
        Private _tblFeedsTextLang As tbl_feeds_text_lang

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "Feeds"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Clubs" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

        Public WriteOnly Property FeedsEntity() As DEFeeds
            Set(ByVal value As DEFeeds)
                _feedsEntity = value
            End Set
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_feeds_template instance with DESettings
        ''' </summary>
        ''' <value>tbl_feeds_template instance</value>
        Public ReadOnly Property TblFeedsTemplate() As tbl_feeds_template
            Get
                If (_tblFeedsTemplate Is Nothing) Then
                    _tblFeedsTemplate = New tbl_feeds_template(_settings)
                End If
                _tblFeedsTemplate.FeedsEntity = _feedsEntity
                Return _tblFeedsTemplate
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_feeds_text_lang with DESettings
        ''' </summary>
        ''' <value>The tbl_feeds_text_lang instance.</value>
        Public ReadOnly Property TblFeedsTextLang() As tbl_feeds_text_lang
            Get
                If (_tblFeedsTextLang Is Nothing) Then
                    _tblFeedsTextLang = New tbl_feeds_text_lang(_settings)
                End If
                _tblFeedsTextLang.FeedsEntity = _feedsEntity
                Return _tblFeedsTextLang
            End Get
        End Property

#End Region

#Region "Public Methods"

#End Region
    End Class
End Namespace
