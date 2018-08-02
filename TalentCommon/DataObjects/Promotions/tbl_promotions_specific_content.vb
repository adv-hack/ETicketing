Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_promotions_specific_content
    ''' </summary>
    <Serializable()> _
    Public Class tbl_promotions_specific_content
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_promotions_specific_content"

#End Region

#Region "Constructors"

        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_promotions_specific_content" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets promotion content for a given promotion code and content type
        ''' </summary>
        ''' <param name="contentType">Content Type</param>
        ''' <param name="promotionCode">Promotion Code</param>
        ''' <param name="cacheing">The cache property, default true</param>
        ''' <param name="cacheTimeMinutes">The cache time property, default 30 mins</param>
        ''' <returns>DataSet with records</returns>
        Public Function GetPromotionSpecificContent(ByVal contentType As String, ByVal promotionCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetPromotionSpecificContent" & _settings.BusinessUnit & contentType & promotionCode)
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT PROMOTION_CONTENT_ID, PROMOTION_CONTENT FROM tbl_promotions_specific_content WHERE LANGUAGE_CODE = @LANGUAGE_CODE AND BUSINESS_UNIT = @BUSINESS_UNIT AND CONTENT_TYPE = @CONTENT_TYPE AND ISNULL(PROMOTION_CODE, '') = ISNULL(@PROMOTION_CODE, '')"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LANGUAGE_CODE", Utilities.GetDefaultLanguage()))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", _settings.BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CONTENT_TYPE", contentType))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PROMOTION_CODE", promotionCode))
                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results
            Return outputDataTable
        End Function

#End Region

#Region "Private Methods"

#End Region

    End Class
End Namespace