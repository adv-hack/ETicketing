Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access promotions module
    ''' </summary>
    <Serializable()> _
    Public Class Promotions
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblPromotionsSpecificContent As tbl_promotions_specific_content

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "Promotions"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Promotions" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

        ''' <summary>
        ''' Create and Gets the tbl_promotions_specific_content instance with DESettings
        ''' </summary>
        ''' <value>tbl_promotions_specific_content instance</value>
        Public ReadOnly Property TblPromotionsSpecificContent() As tbl_promotions_specific_content
            Get
                If (_tblPromotionsSpecificContent Is Nothing) Then
                    _tblPromotionsSpecificContent = New tbl_promotions_specific_content(_settings)
                End If
                Return _tblPromotionsSpecificContent
            End Get
        End Property

#End Region

#Region "Public Methods"


#End Region

    End Class
End Namespace
