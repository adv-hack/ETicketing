Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_alert_definition
    ''' </summary>
    <Serializable()> _
    Public Class tbl_alert_definition
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_alert_definition"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_alert_definition" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Gets all the alert definition records
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
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_ALERT_DEFINITION"

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
        ''' Gets the alert definition records based on the given business unit and partner.
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetByBUAndPartner(Optional ByVal showDeletedAlerts As Boolean = False, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBUAndPartner")
            Dim talentSqlAccessDetail As New TalentDataAccess


            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner"
            cacheKeyHierarchyBased(0) = ToUpper(_settings.BusinessUnit) & ToUpper(_settings.Partner)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(_settings.BusinessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", _settings.BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", _settings.Partner))
                Dim sqlStatement As String = "SELECT * FROM TBL_ALERT_DEFINITION WHERE "
                If Not showDeletedAlerts Then
                    sqlStatement = sqlStatement & "[DELETED]=0 AND "
                End If

                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
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

        ''' <summary>
        ''' Gets the alert definition records based on the given alert Name.
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetByAlertName(ByVal alertName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByAlertName")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String

            whereClauseFetchHierarchy(0) = " AND BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner"
            cacheKeyHierarchyBased(0) = ToUpper(_settings.BusinessUnit) & ToUpper(_settings.Partner)

            whereClauseFetchHierarchy(1) = " AND BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(_settings.BusinessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = " AND BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AlertName", alertName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", _settings.BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", _settings.Partner))
                Dim sqlStatement As String = "SELECT * FROM TBL_ALERT_DEFINITION WHERE NAME = @AlertName"

                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(alertName) & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
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
        ''' <summary>
        ''' Gets the alert definition records based on the given alert id.
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetByAlertID(ByVal alertId As Long, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByAlertID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(alertId)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM TBL_ALERT_DEFINITION WHERE ID = @AlertID"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AlertID", alertId))

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
        ''' Checks the tbl_alert_definition records to see if the alertName is enabled
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner code</param>
        ''' <param name="cacheing">Optional cache setting</param>
        ''' <param name="cacheTimeMinutes">Optional cache time</param>
        ''' <returns>true if the alert is enabled</returns>
        ''' <remarks></remarks>
        Public Function AlertInUse(ByVal businessUnit As String, ByVal partner As String, ByVal alertName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Boolean
            Dim alertIsInUse As Boolean = False
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, alertName + "AlertInUse")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim outputDataTable As New DataTable

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String

            whereClauseFetchHierarchy(0) = "AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]=@Partner"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "AND [BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND [PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit) & ToUpper(partner)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                Dim sqlStatement As String = _
                    "SELECT TOP 1 [ENABLED] FROM [tbl_alert_definition] WITH(NOLOCK)" & _
                    "WHERE [NAME] = '" + alertName + "' " & _
                    "AND [ACTIVATION_START_DATETIME] < getdate() " & _
                    "AND [ACTIVATION_END_DATETIME] > getdate() "

                'Execute
                Dim err As New ErrorObj

                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If outputDataTable.Rows.Count > 0 Then
                            alertIsInUse = CBool(outputDataTable.Rows(0)(0))
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
            Return alertIsInUse
        End Function

        ' ''' <summary>
        ' ''' Checks the tbl_alert_definition records to see if the 'Birthday Alert' is enabled
        ' ''' </summary>
        ' ''' <param name="businessUnit">The given business unit</param>
        ' ''' <param name="partner">The given partner code</param>
        ' ''' <param name="cacheing">Optional cache setting</param>
        ' ''' <param name="cacheTimeMinutes">Optional cache time</param>
        ' ''' <returns>true if the alert is enabled</returns>
        ' ''' <remarks></remarks>
        'Public Function BirthdayAlertInUse(ByVal businessUnit As String, ByVal partner As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Boolean
        '    Dim birthdayAlertIsInUse As Boolean = False
        '    Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "BirthdayAlertInUse")
        '    Dim talentSqlAccessDetail As New TalentDataAccess
        '    Dim outputDataTable As New DataTable

        '    'Permutation and Combinations Select statement
        '    'BusinessUnit   Partner
        '    'Given          Given
        '    'Given          *ALL
        '    '*ALL           *ALL

        '    Dim whereClauseFetchHierarchy(2) As String
        '    Dim cacheKeyHierarchyBased(2) As String

        '    whereClauseFetchHierarchy(0) = "AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]=@Partner"
        '    cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

        '    whereClauseFetchHierarchy(1) = "AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
        '    cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

        '    whereClauseFetchHierarchy(2) = "AND [BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND [PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
        '    cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

        '    Try
        '        'Construct The Call
        '        talentSqlAccessDetail.Settings = _settings
        '        talentSqlAccessDetail.Settings.Cacheing = cacheing
        '        talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
        '        talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit) & ToUpper(partner)
        '        talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
        '        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
        '        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
        '        Dim sqlStatement As String = _
        '            "SELECT TOP 1 [ENABLED] FROM [tbl_alert_definition] WITH(NOLOCK)" & _
        '            "WHERE [NAME] = 'BirthdayAlert' " & _
        '            "AND [ACTIVATION_START_DATETIME] < getdate() " & _
        '            "AND [ACTIVATION_END_DATETIME] > getdate() "

        '        'Execute
        '        Dim err As New ErrorObj

        '        For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
        '            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
        '            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
        '            err = talentSqlAccessDetail.SQLAccess()
        '            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
        '                outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
        '                If outputDataTable.Rows.Count > 0 Then
        '                    birthdayAlertIsInUse = CBool(outputDataTable.Rows(0)(0))
        '                    Exit For
        '                End If
        '            Else
        '                Exit For
        '            End If
        '        Next
        '    Catch ex As Exception
        '        Throw
        '    Finally
        '        talentSqlAccessDetail = Nothing
        '    End Try
        '    Return birthdayAlertIsInUse
        'End Function

        ' ''' <summary>
        ' ''' Checks the tbl_alert_definition records to see if the 'Friends and Family Birthday Alert' is enabled
        ' ''' </summary>
        ' ''' <param name="businessUnit">The given business unit</param>
        ' ''' <param name="partner">The given partner code</param>
        ' ''' <param name="cacheing">Optional cache setting</param>
        ' ''' <param name="cacheTimeMinutes">Optional cache time</param>
        ' ''' <returns>true if the alert is enabled</returns>
        ' ''' <remarks></remarks>
        'Public Function CCExpiryAlertInUse(ByVal businessUnit As String, ByVal partner As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Boolean
        '    Dim ccExpiryAlertIsInUse As Boolean = False
        '    Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "CCExpiryAlertInUse")
        '    Dim talentSqlAccessDetail As New TalentDataAccess
        '    Dim outputDataTable As New DataTable

        '    'Permutation and Combinations Select statement
        '    'BusinessUnit   Partner
        '    'Given          Given
        '    'Given          *ALL
        '    '*ALL           *ALL

        '    Dim whereClauseFetchHierarchy(2) As String
        '    Dim cacheKeyHierarchyBased(2) As String

        '    whereClauseFetchHierarchy(0) = "AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]=@Partner"
        '    cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

        '    whereClauseFetchHierarchy(1) = "AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
        '    cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

        '    whereClauseFetchHierarchy(2) = "AND [BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND [PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
        '    cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

        '    Try
        '        'Construct The Call
        '        talentSqlAccessDetail.Settings = _settings
        '        talentSqlAccessDetail.Settings.Cacheing = cacheing
        '        talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
        '        talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit) & ToUpper(partner)
        '        talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
        '        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
        '        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
        '        Dim sqlStatement As String = _
        '            "SELECT TOP 1 [ENABLED] FROM [tbl_alert_definition] WITH(NOLOCK)" & _
        '            "WHERE [NAME] = 'CCExpiryAlert' " & _
        '            "AND [ACTIVATION_START_DATETIME] < getdate() " & _
        '            "AND [ACTIVATION_END_DATETIME] > getdate() "

        '        'Execute
        '        Dim err As New ErrorObj

        '        For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
        '            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
        '            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
        '            err = talentSqlAccessDetail.SQLAccess()
        '            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
        '                outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
        '                If outputDataTable.Rows.Count > 0 Then
        '                    ccExpiryAlertIsInUse = CBool(outputDataTable.Rows(0)(0))
        '                    Exit For
        '                End If
        '            Else
        '                Exit For
        '            End If
        '        Next
        '    Catch ex As Exception
        '        Throw
        '    Finally
        '        talentSqlAccessDetail = Nothing
        '    End Try
        '    Return ccExpiryAlertIsInUse
        'End Function

        ' ''' <summary>
        ' ''' Checks the tbl_alert_definition records to see if the 'Friends and Family Birthday Alert' is enabled
        ' ''' </summary>
        ' ''' <param name="businessUnit">The given business unit</param>
        ' ''' <param name="partner">The given partner code</param>
        ' ''' <param name="cacheing">Optional cache setting</param>
        ' ''' <param name="cacheTimeMinutes">Optional cache time</param>
        ' ''' <returns>true if the alert is enabled</returns>
        ' ''' <remarks></remarks>
        'Public Function FFBirthdayAlertInUse(ByVal businessUnit As String, ByVal partner As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Boolean
        '    Dim ffBirthdayAlertIsInUse As Boolean = False
        '    Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "FFBirthdayAlertInUse")
        '    Dim talentSqlAccessDetail As New TalentDataAccess
        '    Dim outputDataTable As New DataTable

        '    'Permutation and Combinations Select statement
        '    'BusinessUnit   Partner
        '    'Given          Given
        '    'Given          *ALL
        '    '*ALL           *ALL

        '    Dim whereClauseFetchHierarchy(2) As String
        '    Dim cacheKeyHierarchyBased(2) As String

        '    whereClauseFetchHierarchy(0) = "AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]=@Partner"
        '    cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

        '    whereClauseFetchHierarchy(1) = "AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
        '    cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

        '    whereClauseFetchHierarchy(2) = "AND [BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND [PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
        '    cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

        '    Try
        '        'Construct The Call
        '        talentSqlAccessDetail.Settings = _settings
        '        talentSqlAccessDetail.Settings.Cacheing = cacheing
        '        talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
        '        talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit) & ToUpper(partner)
        '        talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
        '        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
        '        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
        '        Dim sqlStatement As String = _
        '            "SELECT TOP 1 [ENABLED] FROM [tbl_alert_definition] WITH(NOLOCK)" & _
        '            "WHERE [NAME] = 'FFBirthdayAlert' " & _
        '            "AND [ACTIVATION_START_DATETIME] < getdate() " & _
        '            "AND [ACTIVATION_END_DATETIME] > getdate() "

        '        'Execute
        '        Dim err As New ErrorObj

        '        For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
        '            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
        '            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
        '            err = talentSqlAccessDetail.SQLAccess()
        '            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
        '                outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
        '                If outputDataTable.Rows.Count > 0 Then
        '                    ffBirthdayAlertIsInUse = CBool(outputDataTable.Rows(0)(0))
        '                    Exit For
        '                End If
        '            Else
        '                Exit For
        '            End If
        '        Next
        '    Catch ex As Exception
        '        Throw
        '    Finally
        '        talentSqlAccessDetail = Nothing
        '    End Try
        '    Return ffBirthdayAlertIsInUse
        'End Function

        ''' <summary>
        ''' Gets the ID from tbl_alert_definition for the given alert name.
        ''' Used for the special alerts: 'BirthdayAlert', 'FFBirthdayAlert' and 'CCExpiryAlert'
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner code</param>
        ''' <param name="alertName">The special alert name</param>
        ''' <param name="cacheing">The boolean value to enable caching, default is true</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins, default is 30</param>
        ''' <returns>The alert ID as integer</returns>
        ''' <remarks></remarks>
        Public Function GetSpecialAlertId(ByVal businessUnit As String, ByVal partner As String, ByVal alertName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Integer
            Dim alertId As Integer = 0
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetSpecialAlertId")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim outputDataTable As New DataTable

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String

            whereClauseFetchHierarchy(0) = "AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]=@Partner"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "AND [BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND [PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit) & ToUpper(partner)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Name", alertName))
                Dim sqlStatement As String = "SELECT TOP 1 [ID] FROM [tbl_alert_definition] WITH(NOLOCK) WHERE [NAME] = @Name "

                'Execute
                Dim err As New ErrorObj

                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter) & alertName
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If outputDataTable.Rows.Count = 1 Then
                            alertId = CInt(outputDataTable.Rows(0)(0))
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
            Return alertId
        End Function

        ''' <summary>
        ''' Sets the alert definition record to a deleted state - marks 'DELETED' to true and 'ENABLED' to false.
        ''' This works across multiple servers
        ''' </summary>
        ''' <param name="alertDefinitionId">The given alert definition ID that has been selected for deletion</param>
        ''' <param name="givenTransaction">The given transaction</param>
        ''' <returns>The number of affected rows</returns>
        ''' <remarks></remarks>
        Public Function DeleteAlert(ByVal alertDefinitionId As Integer, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim talentSqlAccessDetail0 As New TalentDataAccess
            Dim err As New ErrorObj

            ' Retrieve the list of SQL DB's to be updated from the settings object
            Dim connectionStringCount As Integer = _settings.ConnectionStringList.Count
            Dim connectionStringList As Generic.List(Of String) = _settings.ConnectionStringList

            ' Retrieve the relevant keys to identify the correct tbl_alert record on ANY web server as ID may chnage from box-to-box
            Dim alertDatatable As Data.DataTable
            talentSqlAccessDetail0.Settings = _settings
            talentSqlAccessDetail0.Settings.Cacheing = False
            talentSqlAccessDetail0.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement0 As String = "SELECT [BUSINESS_UNIT], [PARTNER], [NAME], [DESCRIPTION] FROM [tbl_alert_definition] WHERE [ID]=@AlertDefinitionId"
            talentSqlAccessDetail0.CommandElements.CommandText = sqlStatement0
            talentSqlAccessDetail0.CommandElements.CommandParameter.Add(ConstructParameter("@AlertDefinitionId", alertDefinitionId))
            err = talentSqlAccessDetail0.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail0.ResultDataSet Is Nothing)) Then

                alertDatatable = talentSqlAccessDetail0.ResultDataSet.Tables(0)

                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                Dim sqlStatement As String = "UPDATE [tbl_alert_definition] SET [ENABLED]=0, [DELETED]=1 " & _
                                            "WHERE [BUSINESS_UNIT]=@Business_Unit AND [PARTNER]=@Partner AND [NAME]=@Name AND [DESCRIPTION]=@Description"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Business_Unit", alertDatatable.Rows(0)(0)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", alertDatatable.Rows(0)(1)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Name", alertDatatable.Rows(0)(2)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Description", alertDatatable.Rows(0)(3)))

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

        ' ''' <summary>
        ' ''' Updates the alert by ID.
        ' ''' </summary>
        ' ''' <param name="alertId">The alert id.</param>
        ' ''' <param name="businessUnit">The business unit.</param>
        ' ''' <param name="partner">The partner.</param>
        ' ''' <param name="name">The name.</param>
        ' ''' <param name="description">The description.</param>
        ' ''' <param name="action">The action.</param>
        ' ''' <param name="actionDetail">The action detail.</param>
        ' ''' <param name="activationStart">The activation start.</param>
        ' ''' <param name="activationEnd">The activation end.</param>
        ' ''' <param name="enabled">The enabled.</param>
        ' ''' <param name="imagePath">The image path.</param>
        ' ''' <param name="givenTransaction">The given transaction.</param><returns></returns>
        'Public Function UpdateAlertByID(ByVal alertId As Integer, _
        '                                ByVal businessUnit As String, ByVal partner As String, _
        '                                ByVal name As String, ByVal description As String, _
        '                                ByVal action As String, ByVal actionDetail As String, _
        '                                ByVal activationStart As String, ByVal activationEnd As String,
        '                                ByVal enabled As String, ByVal imagePath As String,
        '                                Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
        '    Dim affectedRows As Integer = 0
        '    Dim talentSqlAccessDetail As New TalentDataAccess
        '    Dim err As New ErrorObj
        '    'Construct The Call
        '    talentSqlAccessDetail.Settings = _settings
        '    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
        '    Dim sqlStatement As String = "UPDATE TBL_ALERT_DEFINITION SET "
        '    sqlStatement = sqlStatement & "[BUSINESS_UNIT] = @BusinessUnit"
        '    sqlStatement = sqlStatement & ",[PARTNER] = @Partner"
        '    sqlStatement = sqlStatement & ",[NAME] = @Name"
        '    sqlStatement = sqlStatement & ",[DESCRIPTION] = @Description"
        '    sqlStatement = sqlStatement & ",[IMAGE_PATH] = @ImagePath"
        '    sqlStatement = sqlStatement & ",[ACTION] = @Action"
        '    sqlStatement = sqlStatement & ",[ACTION_DETAILS] = @ActionDetails"
        '    sqlStatement = sqlStatement & ",[ACTIVATION_START_DATETIME] = @ActivationStartTime"
        '    sqlStatement = sqlStatement & ",[ACTIVATION_END_DATETIME] = @ActivationEndTime"
        '    sqlStatement = sqlStatement & ",[ENABLED] = @Enabled"
        '    sqlStatement = sqlStatement & " WHERE [ID]=@AlertId"

        '    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AlertId", alertId))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Name", name))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Description", description))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ImagePath", imagePath))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Action", action))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ActionDetails", actionDetail))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ActivationStartTime", activationStart))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ActivationEndTime", activationEnd))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Enabled", enabled))

        '    'Execute
        '    If (givenTransaction Is Nothing) Then
        '        err = talentSqlAccessDetail.SQLAccess()
        '    Else
        '        err = talentSqlAccessDetail.SQLAccess(givenTransaction)
        '    End If
        '    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
        '        affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
        '    End If
        '    talentSqlAccessDetail = Nothing

        '    'Return results
        '    Return affectedRows
        'End Function

        'Public Function InsertAlert(ByVal businessUnit As String, ByVal partner As String, _
        '                                ByVal name As String, ByVal description As String, _
        '                                ByVal action As String, ByVal actionDetail As String, _
        '                                ByVal activationStart As String, ByVal activationEnd As String,
        '                                ByVal enabled As String, ByVal imagePath As String, ByVal nonStandard As String,
        '                                Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
        '    Dim affectedRows As Integer = 0
        '    Dim talentSqlAccessDetail As New TalentDataAccess
        '    Dim err As New ErrorObj
        '    'Construct The Call
        '    talentSqlAccessDetail.Settings = _settings
        '    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
        '    Dim sqlStatement As String = " SET NOCOUNT ON "
        '    sqlStatement = sqlStatement & "INSERT INTO TBL_ALERT_DEFINITION"
        '    sqlStatement = sqlStatement & "(BUSINESS_UNIT,PARTNER,NAME,DESCRIPTION,IMAGE_PATH,ACTION,ACTION_DETAILS"
        '    sqlStatement = sqlStatement & ",ACTIVATION_START_DATETIME,ACTIVATION_END_DATETIME,ENABLED,NON_STANDARD)"
        '    sqlStatement = sqlStatement & " VALUES (@BusinessUnit, @Partner, @Name, @Description, @ImagePath, @Action, @ActionDetails, @ActivationStartTime, @ActivationEndTime, @Enabled, @NonStandard)"
        '    sqlStatement = sqlStatement & " SELECT SCOPE_IDENTITY()"

        '    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Name", name))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Description", description))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ImagePath", imagePath))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Action", action))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ActionDetails", actionDetail))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ActivationStartTime", activationStart, SqlDbType.DateTime))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ActivationEndTime", activationEnd, SqlDbType.DateTime))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Enabled", enabled))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NonStandard", nonStandard))

        '    'Execute
        '    If (givenTransaction Is Nothing) Then
        '        err = talentSqlAccessDetail.SQLAccess()
        '    Else
        '        err = talentSqlAccessDetail.SQLAccess(givenTransaction)
        '    End If
        '    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
        '        affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
        '    End If
        '    talentSqlAccessDetail = Nothing

        '    'Return results
        '    Return affectedRows
        'End Function
#End Region

    End Class
End Namespace