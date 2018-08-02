Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_page_control based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_page_control
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_page_control"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_page_control" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets the page controls from tbl_page_control for the specified page code.
        ''' </summary>
        ''' <param name="pageCode">The page code.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataTable</returns>
        Public Function GetByPageCode(ByVal pageCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByPageCode")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call

                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(pageCode)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_PAGE_CONTROLS Where PAGE_CODE=@PageCode"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))

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

        Public Function InsertOrUpdate(ByVal BusinessUnit As String, ByVal PartnerCode As String, ByVal PageCode As String,
                                       ByVal TextCode As String, ByVal LanguageCode As String, ByVal TextContent As String,
                                       Optional ByRef affectedRows As Integer = 0) As ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheTimeMinutes = 0
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", PartnerCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", PageCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TEXT_CODE", TextCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LANGUAGE_CODE", LanguageCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TEXT_CONTENT", TextContent))
            Dim sqlStatement As System.Text.StringBuilder = New System.Text.StringBuilder
            sqlStatement.Append("UPDATE tbl_page_text_lang ")
            sqlStatement.Append("SET TEXT_CONTENT = @TEXT_CONTENT ")
            sqlStatement.Append("WHERE BUSINESS_UNIT=@BUSINESS_UNIT AND PARTNER_CODE = @PARTNER_CODE ")
            sqlStatement.Append("AND PAGE_CODE=@PAGE_CODE AND LANGUAGE_CODE = @LANGUAGE_CODE AND TEXT_CODE = @TEXT_CODE ")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("IF @@ROWCOUNT <> 0 BEGIN ")

            sqlStatement.Append("SELECT 0 ")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("END")

            sqlStatement.Append(vbLf)
            sqlStatement.Append("ELSE IF @@ROWCOUNT = 0")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("BEGIN ")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("INSERT INTO tbl_page_text_lang(BUSINESS_UNIT, PARTNER_CODE, ")
            sqlStatement.Append(" PAGE_CODE, TEXT_CODE, LANGUAGE_CODE, TEXT_CONTENT) VALUES ")
            sqlStatement.Append("(@BUSINESS_UNIT, @PARTNER_CODE, @PAGE_CODE, @TEXT_CODE, @LANGUAGE_CODE, @TEXT_CONTENT) ")
            sqlStatement.Append("END")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            Try
                errObj = talentSqlAccessDetail.SQLAccess()
                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                Else
                    affectedRows = 0
                End If

            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorMessage = ex.Message
                errObj.ErrorNumber = "TPAGEATTR-01"
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return errObj

        End Function

#End Region

    End Class

End Namespace
