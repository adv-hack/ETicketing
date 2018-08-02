Imports System.Data.SqlClient
Imports System.Text
Imports System.Transactions
Imports Talent.Common.DataObjects.TableObjects
Imports System.Web

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to interface with the alerts tables
    ''' </summary>
    <Serializable()> _
    Public Class Alerts
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblAlert As tbl_alert
        Private _tblAlertDefinition As tbl_alert_definition
        Private _tblAttributeDefinition As tbl_attribute_definition
        Private _tblUserAttribute As tbl_user_attribute
        Private _tblAlertCritera As tbl_alert_critera

        Private _dblAlertsCCExpiryPPSWarnPeriod As Double
        Private _dblAlertsCCExpirySAVWarnPeriod As Double

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "Alerts"

        'Used for logging
        Private Const SOURCEAPPLICATION As String = "MAINTENANCE"
        Private Const SOURCECLASS As String = "ALERTS"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Alerts" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Create and Gets the _tblAlerts instance with DESettings
        ''' </summary>
        ''' <value>tbl_alerts_definition instance</value>
        Public ReadOnly Property TblAlert() As tbl_alert
            Get
                If (_tblAlert Is Nothing) Then
                    _tblAlert = New tbl_alert(_settings)
                End If
                Return _tblAlert
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the _tblAlertCritera instance with DESettings
        ''' </summary>
        ''' <value>tbl_alert_critera instance</value>
        Public ReadOnly Property TblAlertCritera() As tbl_alert_critera
            Get
                If (_tblAlertCritera Is Nothing) Then
                    _tblAlertCritera = New tbl_alert_critera(_settings)
                End If
                Return _tblAlertCritera
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the _tblAlertsDefinition instance with DESettings
        ''' </summary>
        ''' <value>tbl_alerts_definition instance</value>
        Public ReadOnly Property TblAlertDefinition() As tbl_alert_definition
            Get
                If (_tblAlertDefinition Is Nothing) Then
                    _tblAlertDefinition = New tbl_alert_definition(_settings)
                End If
                Return _tblAlertDefinition
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the _tblAttributeDefinition instance with DESettings
        ''' </summary>
        ''' <value>tbl_club_details instance</value>
        Public ReadOnly Property TblAttributeDefinition() As tbl_attribute_definition
            Get
                If (_tblAttributeDefinition Is Nothing) Then
                    _tblAttributeDefinition = New tbl_attribute_definition(_settings)
                End If
                Return _tblAttributeDefinition
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the _tblUserAttribute with DESettings
        ''' </summary>
        ''' <value>The tbl_user_attribute instance.</value>
        Public ReadOnly Property TblUserAttribute() As tbl_user_attribute
            Get
                If (_tblUserAttribute Is Nothing) Then
                    _tblUserAttribute = New tbl_user_attribute(_settings)
                End If
                Return _tblUserAttribute
            End Get
        End Property

#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Gets the current user alerts
        ''' </summary>
        ''' <param name="businessUnit">The current business unit</param>
        ''' <param name="partner">The current partner</param>
        ''' <param name="loginID">The current user login id</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>data table of alerts</returns>
        Public Function GetUserAlertsByBUPartnerLoginID(ByVal businessUnit As String, ByVal partner As String, ByVal loginID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetUserAlertsByBUPartnerLoginID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String

            whereClauseFetchHierarchy(0) = "AND d.[BUSINESS_UNIT]=@BusinessUnit AND d.[PARTNER]=@Partner AND a.[BUSINESS_UNIT]=@BusinessUnit AND a.[PARTNER]=@Partner"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "AND d.[BUSINESS_UNIT]=@BusinessUnit AND d.[PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & _
                "' AND a.[BUSINESS_UNIT]=@BusinessUnit AND a.[PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "AND d.[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND d.[PARTNER]='" & _
                ReplaceSingleQuote(Utilities.GetAllString) & "' AND a.[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND a.[PARTNER]='" & _
                ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", loginID))
                Dim sqlStatement As String = _
                    "SELECT a.[ID], a.[DESCRIPTION] AS DESCRIPTION_1, a.[READ], d.[IMAGE_PATH], d.[ACTION], d.[ACTION_DETAILS], d.[ACTION_DETAILS_URL_OPTION], d.[DESCRIPTION] AS DESCRIPTION_2 " & _
                    "FROM [tbl_alert] a, [tbl_alert_definition] d " & _
                    "WITH(NOLOCK) " & _
                    "WHERE a.[ALERT_ID] = d.[ID] " & _
                    "AND d.[ENABLED] = 1 " & _
                    "AND d.[ACTIVATION_START_DATETIME] < getdate() " & _
                    "AND d.[ACTIVATION_END_DATETIME] > getdate() " & _
                    "AND a.[LOGIN_ID] = @LoginID " & _
                    "AND a.[ACTIVATION_START_DATETIME] < getdate() " & _
                    "AND a.[ACTIVATION_END_DATETIME] > getdate() "

                'Execute
                Dim err As New ErrorObj

                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(loginID) & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter) & "  ORDER BY a.[ACTIVATION_END_DATETIME] ASC "
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If outputDataTable.Rows.Count > 0 Then
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
        ''' Gets the current user alerts
        ''' </summary>
        ''' <param name="businessUnit">The current business unit</param>
        ''' <param name="partner">The current partner</param>
        ''' <param name="loginID">The current user login id</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>data table of alerts</returns>
        Public Function GetUserAlertsByNameLoginID(ByVal alertName As String, ByVal businessUnit As String, ByVal partner As String, ByVal loginID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetUserAlertsByNameLoginID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String

            whereClauseFetchHierarchy(0) = "AND d.[BUSINESS_UNIT]=@BusinessUnit AND d.[PARTNER]=@Partner AND a.[BUSINESS_UNIT]=@BusinessUnit AND a.[PARTNER]=@Partner"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "AND d.[BUSINESS_UNIT]=@BusinessUnit AND d.[PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & _
                "' AND a.[BUSINESS_UNIT]=@BusinessUnit AND a.[PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "AND d.[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND d.[PARTNER]='" & _
                ReplaceSingleQuote(Utilities.GetAllString) & "' AND a.[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND a.[PARTNER]='" & _
                ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit) & ToUpper(partner) & ToUpper(loginID) & ToUpper(alertName)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", loginID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AlertName", alertName))
                Dim sqlStatement As String = _
                    "SELECT a.[ID], a.[DESCRIPTION] AS DESCRIPTION_1, a.[READ], d.[IMAGE_PATH], d.[ACTION], d.[ACTION_DETAILS], d.[ACTION_DETAILS_URL_OPTION], d.[DESCRIPTION] AS DESCRIPTION_2 " & _
                    "FROM [tbl_alert] a, [tbl_alert_definition] d " & _
                    "WITH(NOLOCK) " & _
                    "WHERE a.[ALERT_ID] = d.[ID] " & _
                    "AND d.[ENABLED] = 1 " & _
                    "AND d.[ACTIVATION_START_DATETIME] < getdate() " & _
                    "AND d.[ACTIVATION_END_DATETIME] > getdate() " & _
                    "AND a.[LOGIN_ID] = @LoginID " & _
                    "AND a.[SUBJECT] = @AlertName " & _
                    "AND a.[ACTIVATION_START_DATETIME] < getdate() " & _
                    "AND a.[ACTIVATION_END_DATETIME] > getdate() "

                'Execute
                Dim err As New ErrorObj

                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter) & "  ORDER BY a.[ACTIVATION_END_DATETIME] ASC "
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If outputDataTable.Rows.Count > 0 Then
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
        ''' Insert or update alert definition and alert critera.
        ''' </summary>
        ''' <param name="sConnString">The connection string</param>
        ''' <param name="deAlertDef">Alert Deinition Data Entity</param>
        ''' <param name="deAlertCriteraList">Generic List of Alert Critera Data Entity</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param><returns></returns>
        Public Function InsOrUpdAlertDefAndCritera(ByVal sConnString As String, ByVal deAlertDef As DEAlertDefinition, ByVal deAlertCriteraList As Generic.List(Of DEAlertCritera),
                                                   ByVal boolCriteriaChanged As Boolean, ByVal boolSubjOrDescChanged As Boolean, ByVal boolActDatesChanged As Boolean, ByVal boolFFEmailChanged As Boolean,
                                                   Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As ErrorObj

            Dim errObj As New ErrorObj

            'variable initialisations
            Dim currentConnectionString As String = String.Empty
            Dim loggingConnectionString As String = String.Empty
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "InsOrUpdAlertDefAndCritera")
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing
            Dim dataMode As String = String.Empty
            Dim alertDetailsToLog As String = String.Empty
            If (deAlertDef.AlertID > 0) Then
                dataMode = "Updated"
            Else
                dataMode = "Inserted"
            End If

            Dim logHeaderId As Integer = 0
            Dim SOURCEMETHOD As String = "INSORUPDALERTDEFANDCRITERA"
            Dim additionalDetails As String = "InsOrUpdAlertDefAndCritera started"
            Dim loggingSettings As New Logging(_settings)
            Dim tempReturnAlertID As Integer = 0
            loggingConnectionString = _settings.FrontEndConnectionString

            Dim affectedRows As Integer = loggingSettings.TblLogHeader.Create(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS, additionalDetails, logHeaderId)

            If (logHeaderId > 0) Then
                Try
                    If (Not errObj.HasError) Then
                        'initialising TalentDataAccess Object
                        'this below settings are common for all execution under transaction for TalentDataAccess
                        cacheing = False
                        talentSqlAccessDetail = New TalentDataAccess
                        talentSqlAccessDetail.Settings = _settings
                        talentSqlAccessDetail.Settings.Cacheing = False
                        talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                        talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                        talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                        talentSqlAccessDetail.CommandElements.CommandText = "usp_Alert_InsOrUpdAlertDefAndCritera"
                        talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ALERT_ID", deAlertDef.AlertID))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NAME", deAlertDef.Name))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DESCRIPTION", deAlertDef.Description))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", deAlertDef.BusinessUnit))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", deAlertDef.Partner))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SUBJECT", deAlertDef.Subject))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IMAGE_PATH", deAlertDef.ImagePath))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ACTION", deAlertDef.Action))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ACTION_DETAILS", deAlertDef.ActionDetail))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ACTION_DETAILS_URL_OPTION", deAlertDef.ActionDetailURLOption))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ACTIVATION_START_DATETIME", deAlertDef.ActivationStartDate, SqlDbType.DateTime))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ACTIVATION_END_DATETIME", deAlertDef.ActivationEndDate, SqlDbType.DateTime))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ENABLED", deAlertDef.Enabled))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NON_STANDARD", deAlertDef.NonStandard))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CRITERA_XML_STRING", (GenericListToXmlString(Of Generic.List(Of DEAlertCritera))(deAlertCriteraList)).Replace("'", "''")))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CRITERIA_CHANGED", boolCriteriaChanged))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SUBJDESC_CHANGED", boolSubjOrDescChanged))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ACTDATES_CHANGED", boolActDatesChanged))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FAFEMAIL_CHANGED", boolFFEmailChanged))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ACTION_TYPE", deAlertDef.ActionType))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", deAlertDef.PageCode))
                        'build alert details for logging
                        alertDetailsToLog = deAlertDef.AlertID & ";" & deAlertDef.Name & ";" & deAlertDef.Description
                    End If
                Catch ex As Exception
                    errObj.HasError = True
                    errObj.ErrorMessage = ex.Message
                    errObj.ErrorNumber = "TACALERTS-02"
                End Try

                If (Not errObj.HasError) Then
                    Try

                        talentSqlAccessDetail.Settings.FrontEndConnectionString = sConnString
                        errObj = talentSqlAccessDetail.SQLAccess()
                        If (Not errObj.HasError) Then
                            If (talentSqlAccessDetail.ResultDataSet IsNot Nothing) Then
                                If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                                    tempReturnAlertID = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)

                                End If
                            End If
                        Else
                            errObj.ErrorNumber = "TACALERTS-04"
                        End If

                        If (talentSqlAccessDetail.ResultDataSet IsNot Nothing) AndAlso (talentSqlAccessDetail.ResultDataSet.Tables.Count > 0) Then
                            tempReturnAlertID = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                            If (tempReturnAlertID <> Nothing) AndAlso (tempReturnAlertID > 0) Then

                            ElseIf (tempReturnAlertID = -1) AndAlso (deAlertDef.AlertID > 0) Then
                                errObj.HasError = True
                                errObj.ErrorMessage = "Alert ID is not exists in one of the DB. Update failed."
                                errObj.ErrorNumber = "TACALERTS-09"
                            ElseIf (tempReturnAlertID = -2) AndAlso (deAlertDef.AlertID = 0) Then
                                errObj.HasError = True
                                errObj.ErrorMessage = "Name already exists. Please give different name. Save failed."
                                errObj.ErrorNumber = "TACALERTS-08"
                            ElseIf (tempReturnAlertID = -2) AndAlso (deAlertDef.AlertID > 0) Then
                                errObj.HasError = True
                                errObj.ErrorMessage = "Name already exists. Please give different name. Update failed."
                                errObj.ErrorNumber = "TACALERTS-07"
                            Else
                                errObj.HasError = True
                                errObj.ErrorMessage = "Alert ID returned as zero"
                                errObj.ErrorNumber = "TACALERTS-06"
                            End If
                        Else
                            errObj.HasError = True
                            errObj.ErrorMessage = "Alert ID not returned"
                            errObj.ErrorNumber = "TACALERTS-05"
                        End If

                    Catch ex As Exception
                        errObj.HasError = True
                        errObj.ErrorMessage = ex.Message
                        errObj.ErrorNumber = "TACALERTS-03"
                    Finally
                        'Todo: any resources to dispose
                    End Try
                End If
            Else
                errObj.HasError = True
                errObj.ErrorMessage = "Failed to create log header ID"
                errObj.ErrorNumber = "TACALERTS-01"
            End If
            talentSqlAccessDetail = Nothing

            'Logging the transaction ends or error
            _settings.FrontEndConnectionString = loggingConnectionString
            If (Not errObj.HasError) Then
                affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.SUCCESS, " Successfully " & dataMode & " Alert Definition And Critera Data ")
                affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, SOURCECLASS, SOURCEMETHOD, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, "Alert Details : " & alertDetailsToLog)
                Me.TblAlertDefinition.GetByBUAndPartner(False)
                deAlertDef.AlertID = tempReturnAlertID
            Else
                affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, dataMode & " Failed For Alert Definition And Critera Data ")
                Dim logContent As String = " Error Connection String : " & currentConnectionString & _
                                            " Error Message : " & errObj.ErrorMessage & _
                                            " Alert Details :  " & alertDetailsToLog
                affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, SOURCECLASS, SOURCEMETHOD, errObj.ErrorNumber, Nothing, Nothing, Nothing, Nothing, Nothing, logContent)
            End If
            'Logging ends

            Return errObj
        End Function

        ''' <summary>
        ''' Insert or update alert definition and alert critera.
        ''' </summary>
        ''' <param name="sConnString">The connection string</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param><returns></returns>
        Public Function CopyToAllBusinessUnits(ByVal sConnString As String, ByVal businessUnitFrom As String, ByVal BUTable As DataTable, ByVal deAlertDef As DEAlertDefinition,
                                                   Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As ErrorObj

            Dim errObj As New ErrorObj

            'variable initialisations
            Dim currentConnectionString As String = String.Empty
            Dim loggingConnectionString As String = String.Empty
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "CopyToAllBusinessUnits")
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing

            For Each row As DataRow In BUTable.Rows
                Dim businessUnitTo As String = row.Item("BUSINESS_UNIT")
                If businessUnitFrom <> businessUnitTo Then
                    Try
                        If (Not errObj.HasError) Then
                            cacheing = False
                            talentSqlAccessDetail = New TalentDataAccess
                            talentSqlAccessDetail.Settings = _settings
                            talentSqlAccessDetail.Settings.Cacheing = False
                            talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                            talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                            talentSqlAccessDetail.CommandElements.CommandText = "usp_Alert_SelAndInsByID"
                            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_BusinessUnit_From", businessUnitFrom))
                            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_BusinessUnit_To", businessUnitTo))
                            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_Partner_From", deAlertDef.Partner))
                            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_PageCode_From", deAlertDef.PageCode))
                            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_ID_From", deAlertDef.AlertID))
                        End If
                        talentSqlAccessDetail.Settings.FrontEndConnectionString = sConnString
                        errObj = talentSqlAccessDetail.SQLAccess()
                    Catch ex As Exception
                        errObj.HasError = True
                        errObj.ErrorMessage = ex.Message
                        errObj.ErrorNumber = "Business Unit Copy Failed"
                    End Try
                    If (errObj.HasError) Then
                        Exit For
                    End If
                End If
            Next
            Return errObj
        End Function

        ''' <summary>
        ''' Insert or update alert definition and alert critera.
        ''' </summary>
        ''' <param name="connectionStringList">The connection string list to target more than one database.</param>
        ''' <param name="deAlertDef">Alert Deinition Data Entity</param>
        ''' <param name="deAlertCriteraList">Generic List of Alert Critera Data Entity</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param><returns></returns>
        Public Function InsOrUpdAlertDefAndCritera_Transactional(ByVal connectionStringList As Generic.List(Of String), ByVal deAlertDef As DEAlertDefinition, ByVal deAlertCriteraList As Generic.List(Of DEAlertCritera), ByVal boolCriteriaChanged As Boolean,
                                        Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As ErrorObj

            Dim errObj As New ErrorObj

            'variable initialisations
            Dim currentConnectionString As String = String.Empty
            Dim loggingConnectionString As String = String.Empty
            Dim hasTransactionError As Boolean = True
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "InsOrUpdAlertDefAndCritera")
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing
            Dim connectionStringCount As Integer = connectionStringList.Count
            Dim returnAlertIDFromDBServer As Integer = 0
            Dim dataMode As String = String.Empty
            Dim alertDetailsToLog As String = String.Empty
            If (deAlertDef.AlertID > 0) Then
                dataMode = "Updated"
            Else
                dataMode = "Inserted"
            End If
            'Logging the transaction starts
            Dim logHeaderId As Integer = 0
            Dim SOURCEMETHOD As String = "INSORUPDALERTDEFANDCRITERA"
            Dim additionalDetails As String = "InsOrUpdAlertDefAndCritera transaction started" & _
                    " ConnectionStringCount : " & connectionStringCount
            Dim loggingSettings As New Logging(_settings)
            loggingConnectionString = _settings.FrontEndConnectionString
            Dim affectedRows As Integer = loggingSettings.TblLogHeader.Create(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS, additionalDetails, logHeaderId)

            If (logHeaderId > 0) Then
                Try
                    If (Not errObj.HasError) Then
                        'initialising TalentDataAccess Object
                        'this below settings are common for all execution under transaction for TalentDataAccess
                        cacheing = False
                        talentSqlAccessDetail = New TalentDataAccess
                        talentSqlAccessDetail.Settings = _settings
                        talentSqlAccessDetail.Settings.Cacheing = False
                        talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                        talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                        talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                        talentSqlAccessDetail.CommandElements.CommandText = "usp_Alert_InsOrUpdAlertDefAndCritera"
                        talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ALERT_ID", deAlertDef.AlertID))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", deAlertDef.BusinessUnit))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", deAlertDef.Partner))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NAME", deAlertDef.Name))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DESCRIPTION", deAlertDef.Description))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SUBJECT", deAlertDef.Subject))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IMAGE_PATH", deAlertDef.ImagePath))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ACTION", deAlertDef.Action))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ACTION_DETAILS", deAlertDef.ActionDetail))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ACTION_DETAILS_URL_OPTION", deAlertDef.ActionDetailURLOption))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ACTIVATION_START_DATETIME", deAlertDef.ActivationStartDate, SqlDbType.DateTime))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ACTIVATION_END_DATETIME", deAlertDef.ActivationEndDate, SqlDbType.DateTime))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ENABLED", deAlertDef.Enabled))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NON_STANDARD", deAlertDef.NonStandard))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CRITERA_XML_STRING", (GenericListToXmlString(Of Generic.List(Of DEAlertCritera))(deAlertCriteraList)).Replace("'", "''")))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CRITERIA_CHANGED", boolCriteriaChanged))
                        'build alert details for logging
                        alertDetailsToLog = deAlertDef.AlertID & ";" & deAlertDef.Name & ";" & deAlertDef.Description
                    End If
                Catch ex As Exception
                    errObj.HasError = True
                    errObj.ErrorMessage = ex.Message
                    errObj.ErrorNumber = "TACALERTS-02"
                End Try

                'Have You Finished all kinds validation and initialise any common settings
                'make sure everything ready so that under transaction just open and execute
                'transaction starts.
                '
                ' No longer using scope object or list of connections as Staging now publishes chna
                If (Not errObj.HasError) Then
                    Using scopeObject As TransactionScope = New TransactionScope()
                        Try
                            For connStringIndex As Integer = 0 To connectionStringCount - 1
                                Dim tempReturnAlertID As Integer = 0
                                hasTransactionError = True
                                'Assign the current connection string
                                currentConnectionString = connectionStringList(connStringIndex)
                                talentSqlAccessDetail.Settings.FrontEndConnectionString = currentConnectionString
                                errObj = talentSqlAccessDetail.SQLAccess()
                                If (Not (errObj.HasError)) Then
                                    If (talentSqlAccessDetail.ResultDataSet IsNot Nothing) AndAlso (talentSqlAccessDetail.ResultDataSet.Tables.Count > 0) Then
                                        tempReturnAlertID = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                                        If (tempReturnAlertID <> Nothing) AndAlso (tempReturnAlertID > 0) Then
                                            If connStringIndex = 0 Then
                                                returnAlertIDFromDBServer = tempReturnAlertID
                                                hasTransactionError = False
                                            Else
                                                If (returnAlertIDFromDBServer = tempReturnAlertID) Then
                                                    hasTransactionError = False
                                                Else
                                                    errObj.HasError = True
                                                    errObj.ErrorMessage = "Alert ID returned not matching with previous DB server alert id. Save failed."
                                                    errObj.ErrorNumber = "TACALERTS-10"
                                                    Exit For
                                                End If
                                            End If
                                        ElseIf (tempReturnAlertID = -1) AndAlso (deAlertDef.AlertID > 0) Then
                                            errObj.HasError = True
                                            errObj.ErrorMessage = "Alert ID is not exists in one of the DB. Update failed."
                                            errObj.ErrorNumber = "TACALERTS-09"
                                            Exit For
                                        ElseIf (tempReturnAlertID = -2) AndAlso (deAlertDef.AlertID = 0) Then
                                            errObj.HasError = True
                                            errObj.ErrorMessage = "Name already exists. Please give different name. Save failed."
                                            errObj.ErrorNumber = "TACALERTS-08"
                                            Exit For
                                        ElseIf (tempReturnAlertID = -2) AndAlso (deAlertDef.AlertID > 0) Then
                                            errObj.HasError = True
                                            errObj.ErrorMessage = "Name already exists. Please give different name. Update failed."
                                            errObj.ErrorNumber = "TACALERTS-07"
                                            Exit For
                                        Else
                                            errObj.HasError = True
                                            errObj.ErrorMessage = "Alert ID returned as zero"
                                            errObj.ErrorNumber = "TACALERTS-06"
                                            Exit For
                                        End If
                                    Else
                                        errObj.HasError = True
                                        errObj.ErrorMessage = "Alert ID not returned"
                                        errObj.ErrorNumber = "TACALERTS-05"
                                        Exit For
                                    End If
                                    If hasTransactionError Then
                                        Exit For
                                    End If
                                Else
                                    hasTransactionError = True
                                    errObj.ErrorNumber = "TACALERTS-04"
                                    Exit For
                                End If
                            Next

                            'transaction to commit or abort
                            If (Not hasTransactionError) Then
                                scopeObject.Complete()
                            Else
                                'There is error in the transaction so no commit it automatically abort
                            End If
                        Catch ex As Exception
                            errObj.HasError = True
                            errObj.ErrorMessage = ex.Message
                            errObj.ErrorNumber = "TACALERTS-03"
                        Finally
                            'Todo: any resources to dispose
                        End Try
                    End Using
                End If
            Else
                errObj.HasError = True
                errObj.ErrorMessage = "Failed to create log header ID"
                errObj.ErrorNumber = "TACALERTS-01"
            End If
            talentSqlAccessDetail = Nothing

            'Logging the transaction ends or error
            _settings.FrontEndConnectionString = loggingConnectionString
            If (Not errObj.HasError) Then
                affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.SUCCESS, " Successfully " & dataMode & " Alert Definition And Critera Data ")
                affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, SOURCECLASS, SOURCEMETHOD, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, "Alert Details : " & alertDetailsToLog)
                Me.TblAlertDefinition.GetByBUAndPartner(False)
            Else
                affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, dataMode & " Failed For Alert Definition And Critera Data ")
                Dim logContent As String = " Error Connection String : " & currentConnectionString & _
                                            " Error Message : " & errObj.ErrorMessage & _
                                            " Alert Details :  " & alertDetailsToLog
                affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, SOURCECLASS, SOURCEMETHOD, errObj.ErrorNumber, Nothing, Nothing, Nothing, Nothing, Nothing, logContent)
            End If
            'Logging ends

            Return errObj
        End Function

        Public Function DelAndInsAttributeDefinition(ByVal connectionStringList As Generic.List(Of String), ByVal businessUnit As String, ByVal partner As String, ByVal source As String, ByVal deAttributeDefinitionList As Generic.List(Of DEAttributeDefinition),
                                        Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As ErrorObj

            Dim errObj As New ErrorObj

            'variable initialisations
            Dim currentConnectionString As String = String.Empty
            Dim loggingConnectionString As String = String.Empty
            Dim hasTransactionError As Boolean = True
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "DelAndInsAttributeDefinition")
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing
            Dim connectionStringCount As Integer = connectionStringList.Count
            Dim returnInsertedFromDBServer As Integer = 0
            Dim attributeDefDetailsToLog As String = String.Empty

            'Logging the transaction starts
            Dim logHeaderId As Integer = 0
            Dim SOURCEMETHOD As String = "DELANDINSATTRIBUTEDEFINITION"
            Dim additionalDetails As String = "DelAndInsAttributeDefinition transaction started" & _
                    " ConnectionStringCount : " & connectionStringCount
            _settings.CacheStringExtension = ""
            Dim loggingSettings As New Logging(_settings)
            loggingConnectionString = _settings.FrontEndConnectionString
            Dim affectedRows As Integer = loggingSettings.TblLogHeader.Create(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS, additionalDetails, logHeaderId)

            If (logHeaderId > 0) Then
                Try
                    If (Not errObj.HasError) Then
                        'initialising TalentDataAccess Object
                        'this below settings are common for all execution under transaction for TalentDataAccess
                        cacheing = False
                        talentSqlAccessDetail = New TalentDataAccess
                        talentSqlAccessDetail.Settings = _settings
                        talentSqlAccessDetail.Settings.Cacheing = cacheing
                        talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                        talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                        talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                        talentSqlAccessDetail.CommandElements.CommandText = "usp_Alert_DelAndInsAttributeDefinition"
                        talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", partner))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SOURCE", source))
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ATT_DEF_XML_STRING", (GenericListToXmlString(Of Generic.List(Of DEAttributeDefinition))(deAttributeDefinitionList)).Replace("'", "''")))
                        'build alert details for logging
                        attributeDefDetailsToLog = businessUnit & ";" & partner & ";" & source
                    End If
                Catch ex As Exception
                    errObj.HasError = True
                    errObj.ErrorMessage = ex.Message
                    errObj.ErrorNumber = "TACALERTS-12"
                End Try

                'Have You Finished all kinds validation and initialise any common settings
                'make sure everything ready so that under transaction just open and execute
                'transaction starts
                If (Not errObj.HasError) Then
                    Try
                        For connStringIndex As Integer = 0 To connectionStringCount - 1
                            Using scopeObject As TransactionScope = New TransactionScope()
                                Dim tempAttDefInsertedCount As Integer = 0
                                hasTransactionError = True
                                'Assign the current connection string
                                currentConnectionString = connectionStringList(connStringIndex)
                                talentSqlAccessDetail.Settings.FrontEndConnectionString = currentConnectionString
                                errObj = talentSqlAccessDetail.SQLAccess()
                                If (Not (errObj.HasError)) Then
                                    If (talentSqlAccessDetail.ResultDataSet IsNot Nothing) AndAlso (talentSqlAccessDetail.ResultDataSet.Tables.Count > 0) Then
                                        tempAttDefInsertedCount = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                                        If (tempAttDefInsertedCount <> Nothing) AndAlso (tempAttDefInsertedCount > 0) Then
                                            If connStringIndex = 0 Then
                                                returnInsertedFromDBServer = tempAttDefInsertedCount
                                                hasTransactionError = False
                                            Else
                                                If (returnInsertedFromDBServer = tempAttDefInsertedCount) Then
                                                    hasTransactionError = False
                                                Else
                                                    errObj.HasError = True
                                                    errObj.ErrorMessage = "Number of affected rows returned not matching with previous DB server. Refresh failed."
                                                    errObj.ErrorNumber = "TACALERTS-17"
                                                    Exit For
                                                End If
                                            End If
                                        Else
                                            errObj.HasError = True
                                            errObj.ErrorMessage = "Number of affected rows returned as zero. Refresh may failed."
                                            errObj.ErrorNumber = "TACALERTS-16"
                                            Exit For
                                        End If
                                    Else
                                        errObj.HasError = True
                                        errObj.ErrorMessage = "Number of affected rows not returned. Refresh may failed."
                                        errObj.ErrorNumber = "TACALERTS-15"
                                        Exit For
                                    End If
                                    If hasTransactionError Then
                                        Exit For
                                    End If
                                Else
                                    hasTransactionError = True
                                    errObj.ErrorNumber = "TACALERTS-14"
                                    Exit For
                                End If

                                'transaction to commit or abort
                                If (Not hasTransactionError) Then
                                    scopeObject.Complete()
                                Else
                                    'There is error in the transaction so no commit it automatically abort
                                End If
                            End Using
                        Next

                    Catch ex As Exception
                        errObj.HasError = True
                        errObj.ErrorMessage = ex.Message
                        errObj.ErrorNumber = "TACALERTS-13"
                    Finally
                        'Todo: any resources to dispose
                    End Try
                End If
            Else
                errObj.HasError = True
                errObj.ErrorMessage = "Failed to create log header ID"
                errObj.ErrorNumber = "TACALERTS-11"
            End If
            talentSqlAccessDetail = Nothing

            'Logging the transaction ends or error
            _settings.FrontEndConnectionString = loggingConnectionString
            If (Not errObj.HasError) Then
                affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.SUCCESS, " Successfully Refreshed Attribute Definition Data ")
                affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, SOURCECLASS, SOURCEMETHOD, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, "Attribute Definition : " & attributeDefDetailsToLog)
            Else
                affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, " Refresh Failed For Attribute Definition Data ")
                Dim logContent As String = " Error Connection String : " & currentConnectionString & _
                                            " Error Message : " & errObj.ErrorMessage & _
                                            " Attribute Definition :  " & attributeDefDetailsToLog
                affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, SOURCECLASS, SOURCEMETHOD, errObj.ErrorNumber, Nothing, Nothing, Nothing, Nothing, Nothing, logContent)
            End If
            'Logging ends

            Return errObj
        End Function

        ''' <summary>
        ''' Gets the current unread user alerts
        ''' </summary>
        ''' <param name="businessUnit">The current business unit</param>
        ''' <param name="partner">The current partner</param>
        ''' <param name="loginID">The current user login id</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>data table of alerts</returns>
        Public Function GetUnReadUserAlertsByBUPartnerLoginID(ByVal businessUnit As String, ByVal partner As String, ByVal loginID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetUnReadUserAlertsByBUPartnerLoginID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String

            whereClauseFetchHierarchy(0) = "AND d.[BUSINESS_UNIT]=@BusinessUnit AND d.[PARTNER]=@Partner AND a.[BUSINESS_UNIT]=@BusinessUnit AND a.[PARTNER]=@Partner"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "AND d.[BUSINESS_UNIT]=@BusinessUnit AND d.[PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & _
                "' AND a.[BUSINESS_UNIT]=@BusinessUnit AND a.[PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "AND d.[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND d.[PARTNER]='" & _
                ReplaceSingleQuote(Utilities.GetAllString) & "' AND a.[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND a.[PARTNER]='" & _
                ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", loginID))
                Dim sqlStatement As String = _
                    "SELECT a.[ID], a.[READ], d.[IMAGE_PATH], d.[ACTION], " & _
                    "COALESCE(a.[ACTION_DETAILS],d.[ACTION_DETAILS]) as [ACTION_DETAILS],  a.[ALERT_ID], " & _
                    "d.[ACTION_DETAILS_URL_OPTION], a.[DESCRIPTION], a.[SUBJECT], d.[ENABLED], d.[ACTIVATION_START_DATETIME], d.[ACTIVATION_END_DATETIME], d.[ACTION_TYPE] " & _
                    "FROM [tbl_alert] a, [tbl_alert_definition] d " & _
                    "WITH(NOLOCK) " & _
                    "WHERE a.[ALERT_ID] = d.[ID] " & _
                    "AND d.[ENABLED] = 1 " & _
                    "AND d.[ACTIVATION_START_DATETIME] < getdate() " & _
                    "AND d.[ACTIVATION_END_DATETIME] > getdate() " & _
                    "AND a.[LOGIN_ID] = @LoginID " & _
                    "AND a.[ACTIVATION_START_DATETIME] < getdate() " & _
                    "AND a.[ACTIVATION_END_DATETIME] > getdate() " & _
                    "AND a.[READ]=0 "
                '& _
                '"AND (d.[ACTION_TYPE] = 0 OR d.[ACTION_TYPE] IS NULL) "

                'Execute
                Dim err As New ErrorObj

                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(loginID) & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter) & "  ORDER BY a.[ACTIVATION_END_DATETIME] ASC "
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) Then
                        If (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                            outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                            If outputDataTable.Rows.Count > 0 Then
                                Exit For
                            End If
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
        ''' Gets the current unread user alerts
        ''' </summary>
        ''' <param name="businessUnit">The current business unit</param>
        ''' <param name="partner">The current partner</param>
        ''' <param name="alertName">The alert name</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>data table of alerts</returns>
        Public Function GetAlertByAlertName(ByVal businessUnit As String, ByVal partner As String, ByVal alertName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAlertByAlertName")
            Dim talentSqlAccessDetail As New TalentDataAccess

            If String.IsNullOrEmpty(alertName) Then
                alertName = ""
            End If

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String

            whereClauseFetchHierarchy(0) = "AND d.[BUSINESS_UNIT]=@BusinessUnit AND d.[PARTNER]=@Partner"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "AND d.[BUSINESS_UNIT]=@BusinessUnit AND d.[PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"

            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "AND d.[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND d.[PARTNER]='" & _
                ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                cacheKeyPrefix = cacheKeyPrefix & ToUpper(alertName)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Name", alertName))
                Dim sqlStatement As String = _
                    "SELECT d.[IMAGE_PATH], d.[ACTION], " & _
                    "d.[ACTION_DETAILS], " & _
                    "d.[ACTION_DETAILS_URL_OPTION], d.[ENABLED], d.[ACTIVATION_START_DATETIME], d.[ACTIVATION_END_DATETIME], d.[NAME], d.[ID] " & _
                    "FROM [tbl_alert_definition] d " & _
                    "WITH(NOLOCK) " & _
                    "WHERE d.[DELETED] = 0 " & _
                    "AND d.[NAME] = @Name "


                'Execute
                Dim err As New ErrorObj

                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                            outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
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
        ''' Gets the current unread user alerts
        ''' </summary>
        ''' <param name="businessUnit">The current business unit</param>
        ''' <param name="partner">The current partner</param>
        ''' <param name="alertName">The alert name</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>data table of alerts</returns>
        Public Function GetRestrictedAlertByAlertName(ByVal businessUnit As String, ByVal partner As String, ByVal alertName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Boolean
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetRestrictedAlertByAlertName")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim success As New Boolean
            success = False

            If String.IsNullOrEmpty(alertName) Then
                alertName = ""
            End If

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String

            whereClauseFetchHierarchy(0) = "AND d.[BUSINESS_UNIT]=@BusinessUnit AND d.[PARTNER]=@Partner"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "AND d.[BUSINESS_UNIT]=@BusinessUnit AND d.[PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"

            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "AND d.[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND d.[PARTNER]='" & _
                ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                cacheKeyPrefix = cacheKeyPrefix & ToUpper(alertName)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Name", alertName))
                Dim sqlStatement As String = _
                    "SELECT d.[IMAGE_PATH], d.[ACTION], " & _
                    "d.[ACTION_DETAILS], " & _
                    "d.[ACTION_DETAILS_URL_OPTION], d.[ENABLED], d.[ACTIVATION_START_DATETIME], d.[ACTIVATION_END_DATETIME], d.[NAME], d.[ID] " & _
                    "FROM [tbl_alert_definition] d " & _
                    "WITH(NOLOCK) " & _
                    "WHERE d.[ENABLED] = 1 " & _
                    "AND d.[ACTIVATION_START_DATETIME] < getdate() " & _
                    "AND d.[ACTIVATION_END_DATETIME] > getdate() " & _
                    "AND d.[ACTION] = 'PageRestrict' " & _
                    "AND d.[NAME] = @Name "


                'Execute
                Dim err As New ErrorObj

                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If outputDataTable.Rows.Count > 0 Then
                            System.Web.HttpContext.Current.Session("RestrictedAlertId") = outputDataTable.Rows(0).Item("ID")
                            success = True
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
            Return success
        End Function
        ''' <summary>
        ''' Removes the current unread user alerts tables from cache.
        ''' </summary>
        ''' <param name="businessUnit">The current business unit</param>
        ''' <param name="partner">The current partner</param>
        ''' <param name="loginID">The current user login id</param>
        Public Sub GetUnReadUserAlertsByBUPartnerLoginID_RemoveResultsFromCache(ByVal businessUnit As String, ByVal partner As String, ByVal loginID As String)

            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetUnReadUserAlertsByBUPartnerLoginID_RemoveResultsFromCache")
            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String

            whereClauseFetchHierarchy(0) = "AND d.[BUSINESS_UNIT]=@BusinessUnit AND d.[PARTNER]=@Partner AND a.[BUSINESS_UNIT]=@BusinessUnit AND a.[PARTNER]=@Partner"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "AND d.[BUSINESS_UNIT]=@BusinessUnit AND d.[PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND a.[BUSINESS_UNIT]=@BusinessUnit AND a.[PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "AND d.[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND d.[PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND a.[BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND a.[PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                Dim err As New ErrorObj
                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    Dim cacheKey As String = "SQLAccess" & cacheKeyPrefix & loginID & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    If HttpContext.Current.Cache.Item(cacheKey) Then
                        HttpContext.Current.Cache.Remove(cacheKey)
                    End If
                Next
            Catch ex As Exception
                Throw
            End Try

        End Sub

        ''' <summary>
        ''' Mark an alert as read based on the given alert id and gets the current unread user alerts, after the data has changed
        ''' </summary>
        ''' <param name="businessUnit">The current business unit</param>
        ''' <param name="partner">The current partner</param>
        ''' <param name="loginID">The current user login id</param>
        ''' <param name="alertId">The given alert ID</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>data table of alerts</returns>
        Public Function MarkAlertAsReadReturnAlerts(ByVal businessUnit As String, ByVal partner As String, ByVal loginID As String, ByVal alertId As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            If TblAlert.MarkAlertAsRead(alertId) > 0 Then
                outputDataTable = GetUnReadUserAlertsByBUPartnerLoginID(businessUnit, partner, loginID, False)
            End If
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Generate User Alerts from data in tbl_alert_criteria, tbl_user_attributes, tbl_alert_definition
        ''' </summary>
        ''' <returns>Integer value of the number of rows affect (alert records created)</returns>
        ''' <remarks></remarks>
        Public Function GenerateUserAlerts(ByVal businessUnit As String, ByVal partner As String, ByVal loginID As String, ByVal intAlertsCCExpiryPPSWarnPeriod As Integer, ByVal intAlertsCCExpirySAVWarnPeriod As Integer) As Integer

            Dim timeSpan As TimeSpan = Now.TimeOfDay

            ' Set class level vars
            _dblAlertsCCExpiryPPSWarnPeriod = intAlertsCCExpiryPPSWarnPeriod
            _dblAlertsCCExpirySAVWarnPeriod = intAlertsCCExpirySAVWarnPeriod

            ' Return var
            Dim affectedRows As Integer = 0

            ' Retrieve the user attributes for the logged in user as well as all current valid Alert Definitions.
            Dim dsAlertDefs As Data.DataSet
            Dim dsUserAttributes As Data.DataSet
            Dim dicUserAttributeData As New Dictionary(Of String, String)

            ' Delete read alerts
            TblAlert.DeleteUnReadAlertsByID(loginID)

            dsAlertDefs = GetAlertRecordSet(businessUnit, partner, loginID, 0, 1)
            dsUserAttributes = GetAlertRecordSet(businessUnit, partner, loginID, 0, 3)

            ' If user has no attributes then cannot satisify any alert criteria
            If dsUserAttributes IsNot Nothing And dsUserAttributes.Tables(0).Rows.Count = 0 Then
                Return 0
            End If

            ' Process each retrieved alert
            If dsAlertDefs IsNot Nothing And dsAlertDefs.Tables.Count > 0 And dsAlertDefs.Tables(0).Rows.Count > 0 Then
                For Each alertDefRow As Data.DataRow In dsAlertDefs.Tables(0).Rows

                    Try
                        ' Retrieve the alert criteria for the alert being processed
                        Dim alertID As Integer = alertDefRow(0)
                        Dim dsAlertCriteria As Data.DataSet
                        dsAlertCriteria = GetAlertRecordSet(businessUnit, partner, loginID, alertID, 2)
                        If dsAlertCriteria IsNot Nothing And dsAlertCriteria.Tables.Count > 0 And dsAlertCriteria.Tables(0).Rows.Count > 0 Then


                            ' Generate list of user attribuites and key-value-pair of user attributes data
                            If dicUserAttributeData.Count = 0 Then
                                Dim dtUserAttributes As Data.DataTable = dsUserAttributes.Tables(0)
                                For Each dr As Data.DataRow In dtUserAttributes.Rows
                                    If Not dicUserAttributeData.ContainsKey(dr(3)) Then
                                        dicUserAttributeData.Add(dr(3), dr(4))
                                    Else
                                        ' If key already in dictionary then append new data and re-add.
                                        ' (This is to support multiple F&F alerts)
                                        Dim sAttrData As String = dicUserAttributeData.Item(dr(3)).ToString
                                        sAttrData = sAttrData.Trim + "||" + dr(4).ToString.Trim
                                        dicUserAttributeData.Remove(dr(3))
                                        dicUserAttributeData.Add(dr(3), sAttrData)
                                    End If
                                Next
                            End If

                            ' Test the user current attributes against the alert criteria
                            Dim boolAlert As Boolean = False
                            boolAlert = CheckAlertCriteraVersusCustomerAttributes(dsAlertCriteria, dsUserAttributes)


                            ' If user meets the alert criteria then create the tbl_alert record etc
                            If boolAlert Then
                                Dim boolOK As Boolean = False
                                boolOK = CreateAlert(alertDefRow, dsAlertCriteria, dicUserAttributeData, loginID)
                                If boolOK Then affectedRows += 1
                            End If

                        End If
                    Catch ex As Exception

                    End Try

                Next
            End If

            _settings.Logging.LoadTestLog("Alerts.vb", "GenerateUserAlerts_loginID=" + loginID, timeSpan)

            Return affectedRows

        End Function

        Private Function CheckAlertCriteraVersusCustomerAttributes(ByVal dsAlertCriteria As Data.DataSet, ByVal dsUserAttributes As Data.DataSet) As Boolean

            Dim timeSpan As TimeSpan = Now.TimeOfDay

            Dim boolReturn As Boolean = False

            Dim dtAlertCriteria As Data.DataTable = dsAlertCriteria.Tables(0)
            Dim dtUserAttributes As Data.DataTable = dsUserAttributes.Tables(0)

            Dim sb As New StringBuilder
            Dim sFilter As String = String.Empty
            Dim nRows() As DataRow
            Dim intLastClause As Integer = 1
            Dim intCLAUSE As Integer = 0
            Dim sCLAUSE_TYPE As String = String.Empty
            Dim sOPERATOR As String = String.Empty
            Dim boolResult As Boolean = False

            Dim listOperator As New List(Of String)
            Dim listClauseResult As New List(Of Boolean)

            Try
                Dim intCount As Integer = 0
                For Each dr As Data.DataRow In dtAlertCriteria.Rows

                    ' If change of clause
                    If dr("CLAUSE") <> intLastClause Then

                        ' Finish filter string for previous clause and determine its trueness
                        sb.Append("))")
                        sFilter = sb.ToString
                        nRows = dtUserAttributes.Select(sFilter)
                        Select Case sCLAUSE_TYPE
                            Case "OR"
                                If nRows.GetLength(0) > 0 Then
                                    boolResult = True
                                Else
                                    boolResult = False
                                End If
                            Case "AND"
                                If nRows.GetLength(0) = intCount Then
                                    boolResult = True
                                Else
                                    boolResult = False
                                End If
                        End Select

                        ' Store clause operator and result 
                        listOperator.Insert(intCLAUSE - 1, sOPERATOR)
                        listClauseResult.Insert(intCLAUSE - 1, boolResult)

                        'Reset ready for next clause
                        intCount = 0
                        sb.Clear()
                    End If

                    ' Generate filter string for the clause (i.e. "(ATTR_ID IN ('0000000001', '000000002',....))"
                    If dr("SEQUENCE") = 1 Then
                        intCLAUSE = dr("CLAUSE")
                        sCLAUSE_TYPE = dr("CLAUSE_TYPE").ToString.Trim
                        sOPERATOR = dr("ALERT_OPERATOR").ToString.Trim
                        sb.Append("(ATTR_ID in ('" + dr("ATTR_ID") + "'")
                    Else
                        sb.Append(",'" + dr("ATTR_ID") + "'")
                    End If

                    intCount = intCount + 1

                    ' Set the last used clause in order to detect when changed
                    intLastClause = dr("CLAUSE")


                Next

                ' Finish filter string for previous clause and determine its trueness
                sb.Append("))")
                sFilter = sb.ToString
                nRows = dtUserAttributes.Select(sFilter)
                Select Case sCLAUSE_TYPE
                    Case "OR"
                        If nRows.GetLength(0) > 0 Then
                            boolResult = True
                        Else
                            boolResult = False
                        End If
                    Case "AND"
                        ' Now testing for '>=' rather than simply '='
                        ' (This is to satisify multiple F&F attributes)
                        If nRows.GetLength(0) >= intCount Then
                            boolResult = True
                        Else
                            boolResult = False
                        End If
                End Select
                intCount = 0

                ' Store clause operator and result 
                listOperator.Insert(intCLAUSE - 1, sOPERATOR)
                listClauseResult.Insert(intCLAUSE - 1, boolResult)


                ' Determine the truness of the NOT clause if present (assume success, i.e. boolNotClauseResult=False)
                Dim boolNotClauseResult As Boolean = False

                ' Is there a NOT clause and if so what clause number
                Dim intNOTClauseIdx As Integer = 0
                For idx As Integer = 0 To (listOperator.Count - 1) Step 1
                    If listOperator(idx) = "NOT" Then
                        intNOTClauseIdx = idx
                        Exit For
                    End If
                Next
                'If NOT clause is found then determine its trueness
                If intNOTClauseIdx > 0 Then
                    For idx As Integer = intNOTClauseIdx To (listOperator.Count - 1) Step 1
                        If listClauseResult(idx) = True Then
                            boolNotClauseResult = True
                            Exit For
                        End If
                    Next
                End If


                ' Only continue processing if NOT clause (if present) was determined to be false
                If Not boolNotClauseResult Then
                    ' Perform logic over results
                    Select Case listOperator.Count
                        Case 0
                            boolReturn = False
                        Case 1
                            If listClauseResult(0) = True Then boolReturn = True
                        Case 2
                            If listOperator(1) = "AND" And (listClauseResult(0) = True And listClauseResult(1) = True) Then boolReturn = True
                            If listOperator(1) = "OR" And (listClauseResult(0) = True Or listClauseResult(1) = True) Then boolReturn = True
                            If listOperator(1) = "NOT" And (listClauseResult(0) = True And listClauseResult(1) = False) Then boolReturn = True
                        Case Else

                            'Set the upper limit of elements to check (this needs to stop at the NOT clause)
                            Dim intUpperidx As Integer = listClauseResult.Count - 1
                            If intNOTClauseIdx > 0 Then intUpperidx = intNOTClauseIdx - 1

                            ' AND clause can only be followed by one or more OR clauses
                            ' (i.e. first clause must be true plus one must be true from any of the remaining OR clauses)
                            If listOperator(1) = "AND" Then
                                If listClauseResult(0) = True Then
                                    For idx As Integer = 0 To intUpperidx Step 1
                                        If listClauseResult(idx) = True Then
                                            boolReturn = True
                                            Exit For
                                        End If
                                    Next

                                End If
                            End If
                            ' OR clause can only be followed by one or more OR clauses
                            ' (i.e. any one of the OR clauses need to be true)
                            If listOperator(1) = "OR" Then
                                For idx As Integer = 0 To intUpperidx Step 1
                                    If listClauseResult(idx) = True Then
                                        boolReturn = True
                                        Exit For
                                    End If
                                Next
                            End If
                    End Select
                End If
            Catch
                boolReturn = False
            End Try

            _settings.Logging.LoadTestLog("Alerts.vb", "CheckAlertCriteraVersusCustomerAttributes", timeSpan)

            Return boolReturn

        End Function
        Private Function CreateAlert(ByVal alertDefRow As Data.DataRow, ByVal dsAlertCriteria As Data.DataSet, ByVal dicUserAttributeData As Dictionary(Of String, String), ByVal loginID As String) As Boolean


            Dim boolReturn As Boolean = True
            Dim errObj As New ErrorObj
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing
            Dim talentSqlAccessDetail2 As TalentDataAccess = Nothing

            Dim strAttrIDNonStandardAlert As String = String.Empty

            ' Determine the values required for the all standard and non-standard alerts
            Dim intAlertId As Integer = alertDefRow("ID")
            Dim strAlertBU As String = alertDefRow("BUSINESS_UNIT")
            Dim strAlertPart As String = alertDefRow("PARTNER")
            Dim strAlertName As String = alertDefRow("NAME")
            Dim strAlertDesc As String = String.Empty
            Dim strAlertSubject As String = String.Empty
            Dim strAlertAction As String = String.Empty
            Dim strAlertActionDetails As String = String.Empty
            Dim strAlertActionDetailURLOption As Boolean = False
            Dim datAlertStartDateTime As DateTime = alertDefRow("ACTIVATION_START_DATETIME")
            Dim datAlertEndDateTime As DateTime = alertDefRow("ACTIVATION_END_DATETIME")
            Dim boolAlertNonStandard As Boolean = alertDefRow("NON_STANDARD")
            Dim strAttrID As String = String.Empty

            ' Retrieve the Alert description and Alert action
            strAlertDesc = alertDefRow("DESCRIPTION")
            strAlertSubject = alertDefRow("SUBJECT")
            strAlertAction = alertDefRow("IMAGE_PATH")
            '            strAlertActionDetails = alertDefRow(7)

            ' Determine/override values to be used for non-standard alerts
            If boolAlertNonStandard Then

                ' Retrieve the attribute data
                Dim strAttributeData As String = String.Empty
                strAttrID = dsAlertCriteria.Tables(0).Rows(0)(2)
                strAttrIDNonStandardAlert = strAttrID

                dicUserAttributeData.TryGetValue(strAttrID, strAttributeData)

                ' Set the description, 
                If strAttributeData <> String.Empty Then
                    Dim sData As String() = strAttributeData.Split(New Char() {"¬"c})
                    Select Case strAlertName

                        Case "BirthdayAlert"
                            '                            If strAlertAction = "SendEmail" Then strAlertActionDetails = sData(3).ToString
                            strAlertDesc = strAlertDesc.Replace("<<<customer_name>>>", sData(0).ToString.Trim + " " + sData(1).ToString.Trim + " " + sData(2).ToString.Trim)
                            strAlertSubject = strAlertSubject.Replace("<<<customer_name>>>", sData(0).ToString.Trim + " " + sData(1).ToString.Trim + " " + sData(2).ToString.Trim)
                            strAlertDesc = strAlertDesc.Replace("<<<customer_title>>>", sData(0).ToString.Trim)
                            strAlertSubject = strAlertSubject.Replace("<<<customer_title>>>", sData(0).ToString.Trim)
                            strAlertDesc = strAlertDesc.Replace("<<<customer_forename>>>", sData(1).ToString.Trim)
                            strAlertSubject = strAlertSubject.Replace("<<<customer_forename>>>", sData(1).ToString.Trim)
                            strAlertDesc = strAlertDesc.Replace("<<<customer_surname>>>", sData(2).ToString.Trim)
                            strAlertSubject = strAlertSubject.Replace("<<<customer_surname>>>", sData(2).ToString.Trim)
                            datAlertStartDateTime = DateTime.Today
                            datAlertEndDateTime = DateTime.Today.AddDays(1)

                            boolReturn = CreateAlertRecord(strAlertBU, strAlertPart, loginID, intAlertId, strAlertDesc, strAlertSubject, "", datAlertStartDateTime, datAlertEndDateTime)

                        Case "FFBirthdayAlert"
                            ' Note: there may be more than 1 set of FFBirthdayAlerts data
                            For idx As Integer = 0 To sData.GetUpperBound(0) Step 4
                                '                                strAlertActionDetails = alertDefRow(7)
                                If strAlertAction = "SendEmail" And alertDefRow(7) = "friend@email.address" Then
                                    strAlertActionDetails = sData(idx + 3).ToString
                                End If
                                strAlertDesc = strAlertDesc.Replace("<<<customer_name>>>", sData(idx + 0).ToString.Trim + " " + sData(idx + 1).ToString.Trim + " " + sData(idx + 2).ToString.Trim)
                                strAlertSubject = strAlertSubject.Replace("<<<customer_name>>>", sData(idx + 0).ToString.Trim + " " + sData(idx + 1).ToString.Trim + " " + sData(idx + 2).ToString.Trim)
                                strAlertDesc = strAlertDesc.Replace("<<<customer_title>>>", sData(idx + 0).ToString.Trim)
                                strAlertSubject = strAlertSubject.Replace("<<<customer_title>>>", sData(idx + 0).ToString.Trim)
                                strAlertDesc = strAlertDesc.Replace("<<<customer_forename>>>", sData(idx + 1).ToString.Trim)
                                strAlertSubject = strAlertSubject.Replace("<<<customer_forename>>>", sData(idx + 1).ToString.Trim)
                                strAlertDesc = strAlertDesc.Replace("<<<customer_surname>>>", sData(idx + 2).ToString.Trim)
                                strAlertSubject = strAlertSubject.Replace("<<<customer_surname>>>", sData(idx + 2).ToString.Trim)
                                datAlertStartDateTime = DateTime.Today
                                datAlertEndDateTime = DateTime.Today.AddDays(1)

                                boolReturn = CreateAlertRecord(strAlertBU, strAlertPart, loginID, intAlertId, strAlertDesc, strAlertSubject, strAlertActionDetails, datAlertStartDateTime, datAlertEndDateTime)
                            Next

                        Case "CCExpiryAlertPPS"
                            ' Note: there may be more than 1 set of FFBirthdayAlerts data
                            For idx As Integer = 0 To sData.GetUpperBound(0) Step 3
                                '                                strAlertActionDetails = alertDefRow(7)

                                strAlertDesc = strAlertDesc.Replace("<<<cc_ending_in>>>", sData(idx + 0).ToString.Trim)
                                strAlertDesc = strAlertDesc.Replace("<<<expires_in>>>", sData(idx + 1).ToString.Trim)
                                strAlertSubject = strAlertSubject.Replace("<<<cc_ending_in>>>", sData(idx + 0).ToString.Trim)
                                strAlertSubject = strAlertSubject.Replace("<<<expires_in>>>", sData(idx + 1).ToString.Trim)

                                ' '' Ensure that end date is the last day of the month that the CC expires
                                ''datAlertEndDateTime = DateTime.Today.AddDays(_dblAlertsCCExpiryWarnPeriod)
                                ''Dim intDaysInMonth As Integer = 0
                                ''Select Case datAlertEndDateTime.Month
                                ''    Case Is = 9 Or 6 Or 4 Or 11
                                ''        intDaysInMonth = 30
                                ''    Case Is = 2
                                ''        intDaysInMonth = 28
                                ''    Case Else
                                ''        intDaysInMonth = 31
                                ''End Select
                                ''datAlertEndDateTime = datAlertEndDateTime.AddDays(intDaysInMonth - datAlertEndDateTime.Day)

                                ' '' Ensure that start date is X days before 1st day of the month of the end date (where X = _dblAlertsCCExpiryWarnPeriod)
                                ' '' (this ensures that dupliacte alerst are not generated)
                                ' ''datAlertStartDateTime = DateTime.Today
                                ''Dim y As New DateTime(datAlertEndDateTime.Year, datAlertEndDateTime.Month, 1)
                                ''datAlertStartDateTime = y.AddDays(0 - _dblAlertsCCExpiryWarnPeriod)


                                ' Ensure that end date is the last day of the month that the CC expires
                                Dim intExpMM As Integer = 12
                                Dim intExpYY As Integer = DateTime.Today.Year
                                Dim intDaysInMonth As Integer = 0
                                Dim sExpMM As String = sData(idx + 1).ToString.Trim.Substring(0, 2)
                                Dim sExpYY As String = "20" + sData(idx + 1).ToString.Trim.Substring(2, 2)
                                If IsNumeric(sExpMM) Then intExpMM = CType(sExpMM, Integer)
                                If IsNumeric(sExpYY) Then intExpYY = CType(sExpYY, Integer)
                                Select Case sExpMM
                                    Case Is = "01"
                                        intDaysInMonth = 31
                                    Case Is = "02"
                                        intDaysInMonth = 28
                                    Case Is = "03"
                                        intDaysInMonth = 31
                                    Case Is = "04"
                                        intDaysInMonth = 30
                                    Case Is = "05"
                                        intDaysInMonth = 31
                                    Case Is = "06"
                                        intDaysInMonth = 30
                                    Case Is = "07"
                                        intDaysInMonth = 31
                                    Case Is = "08"
                                        intDaysInMonth = 31
                                    Case Is = "09"
                                        intDaysInMonth = 30
                                    Case Is = "10"
                                        intDaysInMonth = 31
                                    Case Is = "11"
                                        intDaysInMonth = 30
                                    Case Is = "12"
                                        intDaysInMonth = 31
                                    Case Else
                                        intDaysInMonth = 28
                                End Select
                                Dim endDat As New DateTime(intExpYY, intExpMM, intDaysInMonth, 23, 59, 59)

                                'get the number of days which can be added to expiry date

                                endDat = endDat.AddDays(GetNumberOfDaysToAdd(strAlertBU, strAlertPart, "ALERT_CC_EXPIRY_PPS_WARN_PERIOD_AFETRWARDS"))
                                datAlertEndDateTime = endDat


                                ' Ensure that start date is X days before 1st day of the month of the end date (where X = _dblAlertsCCExpiryPPSWarnPeriod)
                                Dim startDat As New DateTime(intExpYY, intExpMM, 1)
                                datAlertStartDateTime = startDat.AddDays(0 - _dblAlertsCCExpiryPPSWarnPeriod)


                                boolReturn = CreateAlertRecord(strAlertBU, strAlertPart, loginID, intAlertId, strAlertDesc, strAlertSubject, "", datAlertStartDateTime, datAlertEndDateTime)
                            Next

                        Case "CCExpiryAlertSAV"
                            ' Note: there may be more than 1 set of FFBirthdayAlerts data
                            For idx As Integer = 0 To sData.GetUpperBound(0) Step 3

                                '                                strAlertActionDetails = alertDefRow(7)

                                strAlertDesc = strAlertDesc.Replace("<<<cc_ending_in>>>", sData(idx + 0).ToString.Trim)
                                strAlertDesc = strAlertDesc.Replace("<<<expires_in>>>", sData(idx + 1).ToString.Trim)
                                strAlertSubject = strAlertSubject.Replace("<<<cc_ending_in>>>", sData(idx + 0).ToString.Trim)
                                strAlertSubject = strAlertSubject.Replace("<<<expires_in>>>", sData(idx + 1).ToString.Trim)

                                ' '' Ensure that end date is the last day of the month that the CC expires
                                ''datAlertEndDateTime = DateTime.Today.AddDays(_dblAlertsCCExpiryWarnPeriod)
                                ''Dim intDaysInMonth As Integer = 0
                                ''Select Case datAlertEndDateTime.Month
                                ''    Case Is = 9 Or 6 Or 4 Or 11
                                ''        intDaysInMonth = 30
                                ''    Case Is = 2
                                ''        intDaysInMonth = 28
                                ''    Case Else
                                ''        intDaysInMonth = 31
                                ''End Select
                                ''datAlertEndDateTime = datAlertEndDateTime.AddDays(intDaysInMonth - datAlertEndDateTime.Day)

                                ' '' Ensure that start date is X days before 1st day of the month of the end date (where X = _dblAlertsCCExpiryWarnPeriod)
                                ' '' (this ensures that dupliacte alerst are not generated)
                                ' ''datAlertStartDateTime = DateTime.Today
                                ''Dim y As New DateTime(datAlertEndDateTime.Year, datAlertEndDateTime.Month, 1)
                                ''datAlertStartDateTime = y.AddDays(0 - _dblAlertsCCExpiryWarnPeriod)


                                ' Ensure that end date is the last day of the month that the CC expires
                                Dim intExpMM As Integer = 12
                                Dim intExpYY As Integer = DateTime.Today.Year
                                Dim intDaysInMonth As Integer = 0
                                Dim sExpMM As String = sData(idx + 1).ToString.Trim.Substring(0, 2)
                                Dim sExpYY As String = "20" + sData(idx + 1).ToString.Trim.Substring(2, 2)
                                If IsNumeric(sExpMM) Then intExpMM = CType(sExpMM, Integer)
                                If IsNumeric(sExpYY) Then intExpYY = CType(sExpYY, Integer)
                                Select Case sExpMM
                                    Case Is = "01"
                                        intDaysInMonth = 31
                                    Case Is = "02"
                                        intDaysInMonth = 28
                                    Case Is = "03"
                                        intDaysInMonth = 31
                                    Case Is = "04"
                                        intDaysInMonth = 30
                                    Case Is = "05"
                                        intDaysInMonth = 31
                                    Case Is = "06"
                                        intDaysInMonth = 30
                                    Case Is = "07"
                                        intDaysInMonth = 31
                                    Case Is = "08"
                                        intDaysInMonth = 31
                                    Case Is = "09"
                                        intDaysInMonth = 30
                                    Case Is = "10"
                                        intDaysInMonth = 31
                                    Case Is = "11"
                                        intDaysInMonth = 30
                                    Case Is = "12"
                                        intDaysInMonth = 31
                                    Case Else
                                        intDaysInMonth = 28
                                End Select
                                Dim endDat As New DateTime(intExpYY, intExpMM, intDaysInMonth, 23, 59, 59)
                                endDat = endDat.AddDays(GetNumberOfDaysToAdd(strAlertBU, strAlertPart, "ALERT_CC_EXPIRY_SAV_WARN_PERIOD_AFETRWARDS"))
                                datAlertEndDateTime = endDat

                                ' Ensure that start date is X days before 1st day of the month of the end date (where X = _dblAlertsCCExpirySAVWarnPeriod)
                                Dim startDat As New DateTime(intExpYY, intExpMM, 1)
                                datAlertStartDateTime = startDat.AddDays(0 - _dblAlertsCCExpirySAVWarnPeriod)


                                boolReturn = CreateAlertRecord(strAlertBU, strAlertPart, loginID, intAlertId, strAlertDesc, strAlertSubject, "", datAlertStartDateTime, datAlertEndDateTime)
                            Next
                        Case "ReservationAlert"
                            strAlertDesc = strAlertDesc.Replace("<<<customer_name>>>", sData(1).ToString.Trim)
                            strAlertDesc = strAlertDesc.Replace("<<<total_reservations>>>", sData(2).ToString.Trim)
                            strAlertDesc = strAlertDesc.Replace("<<<total_ff_reservations>>>", sData(3).ToString.Trim)
                            datAlertStartDateTime = DateTime.Today
                            datAlertEndDateTime = DateTime.Today.AddDays(1)

                            boolReturn = CreateAlertRecord(strAlertBU, strAlertPart, loginID, intAlertId, strAlertDesc, strAlertSubject, "", datAlertStartDateTime, datAlertEndDateTime)
                    End Select
                End If
            Else

                boolReturn = CreateAlertRecord(strAlertBU, strAlertPart, loginID, intAlertId, strAlertDesc, strAlertSubject, "", datAlertStartDateTime, datAlertEndDateTime)

            End If


            ' If stored procedure executed correctly and is a non-standard alert then delete the user attribute so that alert can be re-used in subsequent date-period
            If boolReturn Then
                If boolAlertNonStandard Then
                    If (Not errObj.HasError) Then
                        Try
                            talentSqlAccessDetail = New TalentDataAccess
                            talentSqlAccessDetail.Settings = _settings
                            talentSqlAccessDetail.Settings.Cacheing = False
                            talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                            talentSqlAccessDetail.CommandElements.CommandText = "usp_Alert_DelUserAttribute"
                            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_LOGINID_ID", loginID))
                            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_ATTR_ID", strAttrID))
                        Catch ex As Exception
                            _settings.Logging.ExceptionLog("Alerts.vb|CreateAlert", ex.Message)
                        End Try

                    End If

                    ' Execute call to stored procedure
                    If (Not errObj.HasError) Then
                        Try
                            errObj = talentSqlAccessDetail.SQLAccess()
                        Catch ex As Exception
                            _settings.Logging.ExceptionLog("Alerts.vb|CreateAlert", ex.Message)
                        End Try
                    End If
                End If
            End If

            Return boolReturn

        End Function

        Private Function CreateAlertRecord(ByVal strAlertBU As String, ByVal strAlertPart As String, ByVal loginID As String, ByVal intAlertId As Integer, _
                                           ByVal strAlertDesc As String, ByVal strAlertSubject As String, ByVal strAlertActionDetails As String, _
                                            ByVal datAlertStartDateTime As DateTime, ByVal datAlertEndDateTime As DateTime)

            Dim boolReturn As Boolean = True
            Dim errObj As New ErrorObj
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing

            Dim connectionStringCount As Integer = _settings.ConnectionStringList.Count
            Dim connectionStringList As Generic.List(Of String) = _settings.ConnectionStringList


            ' Setup call to stored procedure usp_Alert_InsOrUpdAlert
            Try
                talentSqlAccessDetail = New TalentDataAccess
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                talentSqlAccessDetail.CommandElements.CommandText = "usp_Alert_InsOrUpdAlert"
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_BUSINESS_UNIT", strAlertBU))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_PARTNER", strAlertPart))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_LOGIN_ID", loginID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_ALERT_ID", intAlertId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_DESCRIPTION", strAlertDesc))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_SUBJECT", strAlertSubject))
                If strAlertActionDetails.ToString.Trim.Length > 0 Then
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_ACTION_DETAILS", strAlertActionDetails))
                End If
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_ACTIVATION_START_DATETIME", datAlertStartDateTime, SqlDbType.DateTime))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_ACTIVATION_END_DATETIME", datAlertEndDateTime, SqlDbType.DateTime))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_READ", False))
            Catch ex As Exception
                boolReturn = False
                _settings.Logging.ExceptionLog("Alerts.vb|CreateAlert", ex.Message)
            End Try

            ' Execute call to stored procedure
            If boolReturn Then
                Try
                    For connStringIndex As Integer = 0 To connectionStringCount - 1
                        talentSqlAccessDetail.Settings.FrontEndConnectionString = connectionStringList(connStringIndex)
                        errObj = talentSqlAccessDetail.SQLAccess()
                    Next
                Catch ex As Exception
                    boolReturn = False
                    _settings.Logging.ExceptionLog("Alerts.vb|CreateAlert", ex.Message)
                End Try
            End If

            Return boolReturn

        End Function

        Private Function GetAlertRecordSet(ByVal businessUnit As String, ByVal partner As String, ByVal loginID As String, ByVal alertID As String, ByVal mode As Integer) As DataSet

            Dim dsReturn As Data.DataSet = Nothing
            Dim errObj As New ErrorObj
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing

            ' Setup call to stored procedure usp_Alert_SelForGenerateUserAlerts
            Try
                talentSqlAccessDetail = New TalentDataAccess
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                talentSqlAccessDetail.CommandElements.CommandText = "usp_Alert_SelForGenerateUserAlerts"
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_PARTNER", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_LOGINID", loginID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_ALERTID", alertID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_MODE", mode))
            Catch ex As Exception
                _settings.Logging.ExceptionLog("Alerts.vb|GetAlertRecordSet", ex.Message)
            End Try

            ' Execute call to stored procedure
            If (Not errObj.HasError) Then
                Try
                    errObj = talentSqlAccessDetail.SQLAccess()
                Catch ex As Exception
                    _settings.Logging.ExceptionLog("Alerts.vb|GetAlertRecordSet", ex.Message)
                End Try
            End If

            ' Retrieve results of stored procedure
            If (Not errObj.HasError) Then
                If (talentSqlAccessDetail.ResultDataSet IsNot Nothing) AndAlso (talentSqlAccessDetail.ResultDataSet.Tables.Count > 0) Then
                    dsReturn = talentSqlAccessDetail.ResultDataSet
                End If
            End If

            talentSqlAccessDetail = Nothing

            Return dsReturn


        End Function

        Private Function GetNumberOfDaysToAdd(ByVal businessUnit As String, ByVal partner As String, ByVal defaultName As String) As Integer
            Dim numberOfDaysToAdd As Integer = 0
            'get the number of days which can be added to expiry date
            Dim numberOfDaysToAddString As String = String.Empty
            Try
                Dim talDataObjAppVariables As New ApplicationVariables(_settings)
                numberOfDaysToAddString = talDataObjAppVariables.TblEcommerceModuleDefaultsBu.GetDefaultNameValue(businessUnit, partner, defaultName, False)
                If numberOfDaysToAddString.Trim.Length <= 0 Then
                    numberOfDaysToAddString = talDataObjAppVariables.TblEcommerceModuleDefaultsBu.GetDefaultNameValue(businessUnit, Utilities.GetAllString, defaultName, False)
                End If
                If numberOfDaysToAddString.Trim.Length <= 0 Then
                    numberOfDaysToAddString = talDataObjAppVariables.TblEcommerceModuleDefaultsBu.GetDefaultNameValue(Utilities.GetAllString, partner, defaultName, False)
                End If
                If numberOfDaysToAddString.Trim.Length <= 0 Then
                    numberOfDaysToAddString = talDataObjAppVariables.TblEcommerceModuleDefaultsBu.GetDefaultNameValue(Utilities.GetAllString, Utilities.GetAllString, defaultName, False)
                End If
                If IsNumeric(numberOfDaysToAddString) Then
                    numberOfDaysToAdd = CInt(numberOfDaysToAddString)
                End If
            Catch ex As Exception
                numberOfDaysToAdd = 0
            End Try
            Return numberOfDaysToAdd
        End Function

#End Region

    End Class
End Namespace
