Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_page_extra_data based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_page_extra_data
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_page-extra_data"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_page_extra_data" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Deletes the specified business unit.
        ''' </summary>
        ''' <param name="businessUnitToDelete">The business unit to delete.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function Delete(ByVal businessUnitToDelete As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE TBL_PAGE_EXTRA_DATA " & _
                "WHERE BUSINESS_UNIT=@BusinessUnitToDelete "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnitToDelete", businessUnitToDelete))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows

        End Function

        ''' <summary>
        ''' Copies the records from one business unit to another business unit inside the table tbl_page_extra_data
        ''' irrespective of whether destination business exists or not and returns no of affected rows
        ''' </summary>
        ''' <param name="fromBusinessUnit">From business unit.</param>
        ''' <param name="toBusinessUnit">To business unit.</param>
        ''' <returns>No Of Affected Rows</returns>
        Public Function CopyByBU(ByVal fromBusinessUnit As String, ByVal toBusinessUnit As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                Dim sqlStatement As String = "INSERT INTO TBL_PAGE_EXTRA_DATA (" & _
                    "BUSINESS_UNIT, PARTNER, LANGUAGE_CODE, PAGE_CODE, LOCATION, DATA, SEQUENCE) " & _
                    "SELECT " & _
                    "@ToBusinessUnit As BUSINESS_UNIT, PARTNER, LANGUAGE_CODE, PAGE_CODE, LOCATION, DATA, SEQUENCE " & _
                    "FROM TBL_PAGE_EXTRA_DATA " & _
                    "WHERE BUSINESS_UNIT = @FromBusinessUnit"

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FromBusinessUnit", fromBusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ToBusinessUnit", toBusinessUnit))

                'Execute
                Dim err As New ErrorObj
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess()
                Else
                    err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows

        End Function

        ''' <summary>
        ''' Gets the records based on given language code and by fetch hierachy for business unit, partner and page code
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="partner">The partner.</param>
        ''' <param name="pageCode">The page code.</param>
        ''' <param name="languageCode">The language code.</param>
        ''' <param name="location">HEAD or BODY location</param>
        ''' <param name="pageCodeWithQueryString">The page code including any querystrings</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>Data Table</returns>
        Public Function GetPageData(ByVal businessUnit As String, ByVal partner As String, ByVal pageCode As String, ByVal languageCode As String, ByVal location As String, _
                                    Optional ByVal pageCodeWithQueryString As String = "", Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetPageData")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Dim whereClauseFetchHierarchy(1) As String
            Dim cacheKeyHierarchyBased(1) As String


            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND (PAGE_CODE LIKE @PageCode OR PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "')"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(pageCode)


            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND (PAGE_CODE LIKE @PageCode OR PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "')"
            cacheKeyHierarchyBased(1) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString) & ToUpper(pageCode)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode & "%"))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Location", location))

                Dim sqlStatement As String = "SELECT * FROM TBL_PAGE_EXTRA_DATA WHERE LANGUAGE_CODE=@LanguageCode AND LOCATION=@Location AND "
                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 1 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(location) & ToUpper(languageCode) & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter) & " ORDER BY SEQUENCE"
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        Dim PageCodeView As New DataView(outputDataTable)
                        Dim PageCodeWithQueryStringView As New DataView(outputDataTable)
                        PageCodeWithQueryStringView.RowFilter = "PAGE_CODE = '" & ReplaceSingleQuote(pageCodeWithQueryString) & "' OR PAGE_CODE = '" & pageCode & "'" & " OR PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
                        outputDataTable = PageCodeWithQueryStringView.ToTable()
                        If (outputDataTable.Rows.Count > 0) Then
                            Exit For
                        End If
                    Else
                        Exit For
                    End If
                Next

            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable

        End Function

#End Region

    End Class

End Namespace
