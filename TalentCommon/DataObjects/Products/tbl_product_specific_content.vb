Namespace DataObjects.TableObjects

    Public Class ProductSpecificContentItem
        Public Property ProductContentId() As Long?
        Public Property ContentType() As String
        Public Property Content() As String
    End Class

    Public Class tbl_product_specific_content
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_product_specific_content"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_product_specific_content" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Gets product content for a given product code
        ''' </summary>
        ''' <param name="contentType">Content Type</param>
        ''' <param name="productCode">Product Code</param>
        ''' <returns>DataSet with records</returns>
        Public Function GetProductContent(ByVal contentType As String, ByVal productCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetProductContent" & _settings.BusinessUnit & contentType & productCode)
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT PRODUCT_CONTENT_ID, PRODUCT_CONTENT FROM tbl_product_specific_content WHERE LANGUAGE_CODE = @LANGUAGE_CODE AND BUSINESS_UNIT = @BUSINESS_UNIT AND CONTENT_TYPE = @CONTENT_TYPE AND ISNULL(PRODUCT_CODE, '') = ISNULL(@PRODUCT_CODE, '')"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LANGUAGE_CODE", Utilities.GetDefaultLanguage()))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", GlobalConstants.STAR_ALL))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CONTENT_TYPE", contentType))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_CODE", productCode))
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

        ''' <summary>
        ''' Gets product content for a given pacakge id
        ''' </summary>
        ''' <param name="contentType">Content Type</param>
        ''' <param name="packageId">Package Id</param>
        ''' <param name="productCode">Product Code</param>
        ''' <returns>DataSet with records</returns>
        Public Function GetPackageContent(ByVal contentType As String, ByVal packageId As String, Optional ByVal productCode As String = "", Optional ByVal cacheing As Boolean = False, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetPackageContent")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT PRODUCT_CONTENT_ID, PRODUCT_CONTENT FROM tbl_product_specific_content WHERE LANGUAGE_CODE = @LANGUAGE_CODE AND BUSINESS_UNIT = @BUSINESS_UNIT AND CONTENT_TYPE = @CONTENT_TYPE AND PACKAGE_ID = @PACKAGE_ID AND ISNULL(PRODUCT_CODE, '') = ISNULL(@PRODUCT_CODE, '')"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LANGUAGE_CODE", Utilities.GetDefaultLanguage()))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", GlobalConstants.STAR_ALL))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CONTENT_TYPE", contentType))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PACKAGE_ID", packageId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_CODE", productCode))
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

        ''' <summary>
        ''' This method is to create input data as data-table
        ''' </summary>
        ''' <param name="contents"></param>
        ''' <returns></returns>
        Private Shared Function CreateData(contents As ProductSpecificContentItem()) As DataTable
            Dim dtProductSpecificContent As New DataTable("ProductSpecificContent")
            With dtProductSpecificContent.Columns
                .Add("PRODUCT_CONTENT_ID", GetType(Integer))
                .Add("CONTENT_TYPE", GetType(String))
                .Add("PRODUCT_CONTENT", GetType(String))
            End With

            For Each content As ProductSpecificContentItem In contents
                Dim dRow As DataRow = dtProductSpecificContent.NewRow
                dRow("PRODUCT_CONTENT_ID") = If(content.ProductContentId, DBNull.Value)
                dRow("CONTENT_TYPE") = content.ContentType
                dRow("PRODUCT_CONTENT") = content.Content
                dtProductSpecificContent.Rows.Add(dRow)
            Next
            Return dtProductSpecificContent
        End Function

#End Region
    End Class
End Namespace