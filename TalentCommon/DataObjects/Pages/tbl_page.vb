Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_page based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_page
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_page"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_page" /> class.
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
            Dim sqlStatement As String = "DELETE TBL_PAGE " & _
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
        ''' Copies the records from one business unit to another business unit inside the table tbl_page
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
                Dim sqlStatement As String = "INSERT INTO TBL_PAGE (" & _
                    "BUSINESS_UNIT, BCT_PARENT, BCT_URL, CSS_PRINT, " & _
                    "DESCRIPTION, FORCE_LOGIN, HTML_IN_USE, IN_USE, PAGE_CODE, " & _
                    "PAGE_QUERYSTRING, PAGE_TYPE, PARTNER_CODE, SHOW_PAGE_HEADER, " & _
                    "USE_SECURE_URL) " & _
                    "SELECT " & _
                    "@ToBusinessUnit As BUSINESS_UNIT, BCT_PARENT, BCT_URL, CSS_PRINT, " & _
                    "DESCRIPTION, FORCE_LOGIN, HTML_IN_USE, IN_USE, PAGE_CODE, " & _
                    "PAGE_QUERYSTRING, PAGE_TYPE, PARTNER_CODE, SHOW_PAGE_HEADER, " & _
                    "USE_SECURE_URL " & _
                    "FROM TBL_PAGE " & _
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
        ''' Gets the page setting details from tbl_page for the specified page code.
        ''' </summary>
        ''' <param name="pageCode">The page code.</param>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataTable</returns>
        Public Function GetByPageCode(ByVal pageCode As String, ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByPageCode")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(pageCode) & businessUnit
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_PAGE Where PAGE_CODE=@PageCode AND BUSINESS_UNIT=@BusinessUnit"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))

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
        ''' Gets all the page records from the table tbl_page
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataSet with records</returns>
        Public Function GetAll(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAll")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_PAGE"

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
        ''' Gets all the page records from the table tbl_page by business unit
        ''' </summary>
        ''' <param name="bu">business unit</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataSet with records</returns>
        Public Function GetAllByBU(ByVal bu As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllByBU")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_PAGE where BUSINESS_UNIT = @BUSINESS_UNIT"

                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", bu))

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
        ''' Gets all the page records from the table tbl_page by business unit
        ''' </summary>
        ''' <param name="businessUnit">business unit</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataSet with records</returns>
        Public Function GetAllByBUPartnerMaintenance(ByVal businessUnit As String, ByVal partner As String, ByVal hideInMaintenance As Boolean, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllByBUPartnerMaintenance")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_PAGE where BUSINESS_UNIT = @BUSINESS_UNIT AND HIDE_IN_MAINTENANCE=@HIDE_IN_MAINTENANCE AND PARTNER_CODE=@PARTNER_CODE"

                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@HIDE_IN_MAINTENANCE", hideInMaintenance))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", partner))
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
        Public Function GetRestrictedPage(ByVal businessUnit As String, ByVal partner As String, ByVal restrictingAlert As String, ByVal pageCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetRestrictedPage")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim success As Boolean = False
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select [PAGE_CODE], [BCT_URL], [ID], [DESCRIPTION], [RESTRICTING_ALERT_NAME] FROM [tbl_page] WHERE [BCT_URL] is not null AND [BCT_URL] <> '' AND ([IN_USE] IS NULL OR [IN_USE]=1) AND ([BUSINESS_UNIT] = @BUSINESS_UNIT OR [BUSINESS_UNIT] = '*ALL') AND ([PARTNER_CODE] = @PARTNER_CODE OR [PARTNER_CODE] = '*ALL') AND @PAGE_CODE = [PAGE_CODE] AND RESTRICTING_ALERT_NAME IS NOT NULL"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RESTRICTING_ALERT_NAME", restrictingAlert))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", pageCode))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                    If outputDataTable.Rows.Count > 0 Then
                        success = True
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results
            Return outputDataTable

        End Function
        Public Function GetPageNames(ByVal businessUnit As String, ByVal partner As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetPageNames")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select [PAGE_CODE], [BCT_URL], [ID], [DESCRIPTION], [RESTRICTING_ALERT_NAME] FROM [tbl_page] WHERE [BCT_URL] is not null AND [BCT_URL] <> '' AND ([IN_USE] IS NULL OR [IN_USE]=1) AND ([BUSINESS_UNIT] = @BUSINESS_UNIT OR [BUSINESS_UNIT] = '*ALL') AND ([PARTNER_CODE] = @PARTNER_CODE OR [PARTNER_CODE] = '*ALL')"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", partner))

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
        ''' Update the hide in maintenance setting to true for the page table
        ''' </summary>
        ''' <param name="pageId">The page id as integer</param>
        ''' <param name="givenTransaction">Optional sql transaction</param>
        ''' <returns>Integer of affected rows</returns>
        ''' <remarks></remarks>
        Public Function UpdateHideInMaintenanceToTrue(ByVal pageId As Integer, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim talentSqlAccessDetail0 As New TalentDataAccess
            Dim err As New ErrorObj

            ' Retrieve the list of SQL DB's to be updated from the settings object
            Dim connectionStringCount As Integer = _settings.ConnectionStringList.Count
            Dim connectionStringList As Generic.List(Of String) = _settings.ConnectionStringList

            ' Retrieve the relevant keys to identify the correct tbl_alert record on ANY web server as ID may chnage from box-to-box
            Dim pageDataTable As Data.DataTable
            talentSqlAccessDetail0.Settings = _settings
            talentSqlAccessDetail0.Settings.Cacheing = False
            talentSqlAccessDetail0.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement0 As String = "SELECT [BUSINESS_UNIT], [PARTNER_CODE], [PAGE_CODE] FROM [tbl_page] WHERE [ID]=@PageId"
            talentSqlAccessDetail0.CommandElements.CommandText = sqlStatement0
            talentSqlAccessDetail0.CommandElements.CommandParameter.Add(ConstructParameter("@PageId", pageId))
            err = talentSqlAccessDetail0.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail0.ResultDataSet Is Nothing)) Then
                pageDataTable = talentSqlAccessDetail0.ResultDataSet.Tables(0)

                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                Dim sqlStatement As String = "UPDATE [tbl_page] SET [HIDE_IN_MAINTENANCE]=1 WHERE [BUSINESS_UNIT]=@Business_Unit AND [PARTNER_CODE]=@Partner AND [PAGE_CODE]=@PageCode"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Business_Unit", pageDataTable.Rows(0)(0)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", pageDataTable.Rows(0)(1)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageDataTable.Rows(0)(2)))

                ' Save frontEndConnectionString
                Dim saveFrontEndConnectionString As String = String.Empty
                saveFrontEndConnectionString = talentSqlAccessDetail.Settings.FrontEndConnectionString

                For connStringIndex As Integer = 0 To connectionStringCount - 1
                    talentSqlAccessDetail.Settings.FrontEndConnectionString = connectionStringList(connStringIndex)

                    'Execute
                    If (givenTransaction Is Nothing) Then
                        err = talentSqlAccessDetail.SQLAccess()
                    Else
                        err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                    End If
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                    End If
                Next

                ' Restore frontEndConnectionString
                talentSqlAccessDetail.Settings.FrontEndConnectionString = saveFrontEndConnectionString
                talentSqlAccessDetail = Nothing
            End If

            'Return results
            Return affectedRows
        End Function

        Public Function RemoveRestrictingAlertName(ByVal pageCode As String, ByVal businessUnit As String, ByVal partner As String, ByVal restrictingAlertName As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            Dim whereClauseFetchHierarchy(3) As String
            Dim cacheKeyHierarchyBased(3) As String

            whereClauseFetchHierarchy(0) = "[BUSINESS_UNIT]=@BusinessUnit AND [PARTNER_CODE]=@PartnerCode"
            whereClauseFetchHierarchy(1) = "[BUSINESS_UNIT]=@BusinessUnit AND [PARTNER_CODE]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            whereClauseFetchHierarchy(2) = "[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND [PARTNER_CODE]=@PartnerCode"
            whereClauseFetchHierarchy(3) = "[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND [PARTNER_CODE]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"

            Try
                Dim err As New ErrorObj
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                'RESTRICTING_ALERT_NAME
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("UPDATE [tbl_page] ")
                sqlStatement.Append("SET [RESTRICTING_ALERT_NAME]=NULL ")
                sqlStatement.Append("WHERE [RESTRICTING_ALERT_NAME] = @Restricting_Alert_Name AND ")
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Restricting_Alert_Name", restrictingAlertName))

                'Execute
                If (givenTransaction Is Nothing) Then
                    For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                        talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString() & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                        err = talentSqlAccessDetail.SQLAccess()
                        If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                            If (talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0) > 0) Then
                                Exit For
                            End If
                        Else
                            Exit For
                        End If
                    Next
                Else
                    For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                        talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString() & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                        err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                        If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                            If (talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0) Then
                                Exit For
                            End If
                        Else
                            Exit For
                        End If
                    Next
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
        Public Function UpdateRestrictingAlertName(ByVal pageCode As String, ByVal businessUnit As String, ByVal partner As String, ByVal restrictingAlertName As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim alertName
            If String.IsNullOrEmpty(restrictingAlertName) Then
                alertName = "NULL"
            Else
                alertName = restrictingAlertName
            End If
            Dim whereClauseFetchHierarchy(3) As String
            Dim cacheKeyHierarchyBased(3) As String

            whereClauseFetchHierarchy(0) = "[BUSINESS_UNIT]=@BusinessUnit AND [PARTNER_CODE]=@PartnerCode"
            whereClauseFetchHierarchy(1) = "[BUSINESS_UNIT]=@BusinessUnit AND [PARTNER_CODE]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            whereClauseFetchHierarchy(2) = "[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND [PARTNER_CODE]=@PartnerCode"
            whereClauseFetchHierarchy(3) = "[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND [PARTNER_CODE]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"

            Try
                Dim err As New ErrorObj
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                'RESTRICTING_ALERT_NAME
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("UPDATE [tbl_page] ")
                If String.IsNullOrEmpty(restrictingAlertName) Then
                    sqlStatement.Append("SET [RESTRICTING_ALERT_NAME]=NULL ")
                Else
                    sqlStatement.Append("SET [RESTRICTING_ALERT_NAME]=@Restricting_Alert_Name ")
                End If

                sqlStatement.Append("WHERE [PAGE_CODE]=@PAGE_CODE AND ")
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Restricting_Alert_Name", restrictingAlertName))

                'Execute
                If (givenTransaction Is Nothing) Then
                    For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                        talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString() & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                        err = talentSqlAccessDetail.SQLAccess()
                        If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                            If (talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0) > 0) Then
                                Exit For
                            End If
                        Else
                            Exit For
                        End If
                    Next
                Else
                    For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                        talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString() & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                        err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                        If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                            If (talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0) Then
                                Exit For
                            End If
                        Else
                            Exit For
                        End If
                    Next
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
#End Region

    End Class

End Namespace
