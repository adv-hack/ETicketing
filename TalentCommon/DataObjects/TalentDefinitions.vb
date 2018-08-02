Imports Talent.Common.DataObjects.TableObjects
Imports System.Data.SqlClient
Imports System.Text

<Serializable()> _
Public Class TalentDefinitions
    Inherits DBObjectBase

#Region "Class Level Fields"

    ''' <summary>
    ''' DESettings Instance
    ''' </summary>
    Private _settings As New DESettings

    Private _tblAgentGroupRoles As tbl_agent_group_roles
    Private _tblAgentPermissionCategories As tbl_agent_permission_categories
    Private _tblAgentGroupPermissions As tbl_agent_group_permissions

    Const CACHEKEY_CLASSNAME As String = "TalentDefinitions_"

#End Region

#Region "Constructors"

    Sub New()
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the <see cref="TalentDefinitions" /> class.
    ''' </summary>
    ''' <param name="settings">DESettings Instance</param>
    Sub New(ByVal settings As DESettings)
        _settings = settings
    End Sub

#End Region

#Region "Public Properties"

    Public ReadOnly Property TblAgentGroupRoles() As tbl_agent_group_roles
        Get
            If (_tblAgentGroupRoles Is Nothing) Then
                _tblAgentGroupRoles = New tbl_agent_group_roles(_settings)
            End If
            Return _tblAgentGroupRoles
        End Get
    End Property

    Public ReadOnly Property TblAgentPermissionCategories() As tbl_agent_permission_categories
        Get
            If (_tblAgentPermissionCategories Is Nothing) Then
                _tblAgentPermissionCategories = New tbl_agent_permission_categories(_settings)
            End If
            Return _tblAgentPermissionCategories
        End Get
    End Property

    Public ReadOnly Property TblAgentGroupPermissions() As tbl_agent_group_permissions
        Get
            If (_tblAgentGroupPermissions Is Nothing) Then
                _tblAgentGroupPermissions = New tbl_agent_group_permissions(_settings)
            End If
            Return _tblAgentGroupPermissions
        End Get
    End Property

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Insert a new permisison group to system 
    ''' </summary>
    ''' <param name="GroupName">Group Name</param>
    ''' <param name="BasedOn">ID of the Group on which the new Group is based.</param>
    ''' <param name="GroupID">The GroupID parameter which will hold the ID of the group created.</param>
    ''' <returns>No of rows affected.</returns>
    ''' <remarks></remarks>
    Public Function InsertGroup(ByVal GroupName As String, ByVal BasedOn As Integer, ByRef GroupID As Integer, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

        Dim nErrorCode As Integer = 0
        Dim strErrorMessage As String = String.Empty

        Dim affectedRows As Integer = 0
        Dim err As New ErrorObj
        Dim talentSqlAccessDetail As TalentDataAccess = Nothing
        Try
            talentSqlAccessDetail = New TalentDataAccess
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            talentSqlAccessDetail.CommandElements.CommandText = "usp_AgentGroupPermission_Insert"
            talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_GroupName", GroupName, SqlDbType.NVarChar, ParameterDirection.Input))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_BasedOn", BasedOn, SqlDbType.Int, ParameterDirection.Input))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_ErrorCode", "0", SqlDbType.Int, ParameterDirection.InputOutput))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_ErrorMessage", "", SqlDbType.VarChar, ParameterDirection.InputOutput))

            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_DEFINITION)
            Else
                err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_DEFINITION, givenTransaction)
            End If

            If (Not (err.HasError)) AndAlso (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                If talentSqlAccessDetail.ResultDataSet.Tables(0).Columns.Contains("groupID") Then
                    GroupID = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)("groupID")
                End If

                ' read output parameter(s) 
                If Not IsNothing(talentSqlAccessDetail.CommandElements.CommandParameter(2)) AndAlso Not IsDBNull(talentSqlAccessDetail.CommandElements.CommandParameter(2).ParamValue) Then
                    nErrorCode = CInt(talentSqlAccessDetail.CommandElements.CommandParameter(2).ParamValue)
                End If
                If Not IsNothing(talentSqlAccessDetail.CommandElements.CommandParameter(3)) AndAlso Not IsDBNull(talentSqlAccessDetail.CommandElements.CommandParameter(3).ParamValue) Then
                    strErrorMessage = talentSqlAccessDetail.CommandElements.CommandParameter(3).ParamValue.ToString()
                End If

                If (nErrorCode > 0) Then
                    err.HasError = True
                    err.ErrorNumber = nErrorCode
                    err.ErrorMessage = strErrorMessage
                End If
            End If
        Catch ex As Exception
            Throw
        Finally
            talentSqlAccessDetail = Nothing
        End Try

        Return affectedRows

    End Function

    ''' <summary>
    ''' Get group permissions by Category/Group selected 
    ''' </summary>
    ''' <param name="GroupID">GroupID for which Permissions are being fetched.</param>
    ''' <param name="CategoryID">CategoryID for which Permissions are being fetched.</param>
    ''' <returns>Datatable containing Permissions for the passed GroupID and PermissionID</returns>
    ''' <remarks></remarks>
    Public Function GetPermissionsByCategoryAndGroup(ByVal GroupID As Integer, ByVal CategoryID As Integer, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
        Dim moduleName As String = "GetPermissionsByCategoryAndGroup"
        Dim dtOutput As DataTable = Nothing
        Dim cacheKey As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, moduleName & _settings.BusinessUnit & _settings.Partner & GroupID & CategoryID)
        Dim isCacheNotFound As Boolean = False
        Dim talentSqlAccessDetail As TalentDataAccess = Nothing
        Try
            Me.ResultDataSet = TryGetFromCache(Of DataSet)(isCacheNotFound, cacheing, cacheKey)
            If isCacheNotFound Then
                Dim strCommandText As New StringBuilder
                strCommandText.Append("SELECT	a.PERMISSION_DESCRIPTION,a.PERMISSION_ID,")
                strCommandText.Append("CASE  WHEN c.PERMISSION_ID IS NULL THEN 0 ELSE 1 END AS IS_CHECKED ")
                strCommandText.Append("FROM	tbl_agent_permissions a ")
                strCommandText.Append(" LEFT JOIN tbl_agent_group_permissions c ON c.PERMISSION_ID = a.PERMISSION_ID AND c.GROUP_ID = @pa_PermissionGroup ")
                strCommandText.Append("WHERE	a.CATEGORY_ID = @pa_PermissionCategory")
                'Construct The Call
                talentSqlAccessDetail = New TalentDataAccess
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKey
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = strCommandText.ToString
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_PermissionGroup", GroupID, SqlDbType.Int, ParameterDirection.Input))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_PermissionCategory", CategoryID, SqlDbType.Int, ParameterDirection.Input))
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_DEFINITION, cacheing, cacheTimeMinutes)
                If (Not (err.HasError)) AndAlso (talentSqlAccessDetail.ResultDataSet IsNot Nothing) Then
                    Me.ResultDataSet = talentSqlAccessDetail.ResultDataSet
                End If
            End If
            If Me.ResultDataSet IsNot Nothing AndAlso Me.ResultDataSet.Tables.Count > 0 Then
                dtOutput = Me.ResultDataSet.Tables(0)
            End If
        Catch ex As Exception
            Throw
        Finally
            talentSqlAccessDetail = Nothing
        End Try

        Return dtOutput
    End Function

    ''' <summary>
    ''' Get the agent permissions for the given permission group based on the group ID
    ''' </summary>
    ''' <param name="groupId">The given group ID</param>
    ''' <param name="cacheTimeMinutes">Optional cache time in mins default 720</param>
    ''' <param name="cacheing">Optionval cache enabled default true</param>
    ''' <returns>Dictionary of string values</returns>
    ''' <remarks></remarks>
    Public Function GetAgentGroupPermissions(ByVal groupId As Integer, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 720) As Dictionary(Of String, String)
        Dim agentGroupPermissions As Dictionary(Of String, String) = Nothing
        Dim moduleName As String = "GetAgentGroupPermissions"
        Dim cacheKey As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, moduleName & _settings.BusinessUnit & _settings.Partner & groupId)
        Dim isCacheNotFound As Boolean = False
        Dim talentSqlAccessDetail As TalentDataAccess = Nothing
        Try
            agentGroupPermissions = TryGetFromCache(Of Dictionary(Of String, String))(isCacheNotFound, cacheing, cacheKey)
            If isCacheNotFound Then
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT [PERMISSION_NAME] FROM [tbl_agent_group_permissions] G, [tbl_agent_permissions] P ")
                sqlStatement.Append("WHERE P.[PERMISSION_ID] = G.[PERMISSION_ID] ")
                sqlStatement.Append("AND [GROUP_ID] = @GroupID")
                'Construct The Call
                talentSqlAccessDetail = New TalentDataAccess
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKey
                talentSqlAccessDetail.ModuleNameForCacheDependency = GlobalConstants.AGENT_PERMISSIONS_CACHEKEY_PREFIX & groupId 'The module name here is used for cache dependancy
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupID", groupId))
                agentGroupPermissions = GetCustomDictionaryEntities(Of Dictionary(Of String, String))(DestinationDatabase.TALENT_DEFINITION, cacheing, cacheTimeMinutes, talentSqlAccessDetail, moduleName)
            End If

        Catch ex As Exception
            Throw
        Finally
            talentSqlAccessDetail = Nothing
        End Try

        Return agentGroupPermissions
    End Function

    ''' <summary>
    ''' Get the list of authority groups with their group type description
    ''' </summary>
    ''' <returns>Data table of results</returns>
    ''' <remarks></remarks>
    Public Function GetAllGroupsWithTypes(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
        Dim moduleName As String = "GetAllGroupsWithTypes"
        Dim dtOutput As DataTable = Nothing
        Dim cacheKey As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, moduleName & _settings.BusinessUnit & _settings.Partner)
        Dim isCacheNotFound As Boolean = False
        Dim talentSqlAccessDetail As TalentDataAccess = Nothing
        Try
            Me.ResultDataSet = TryGetFromCache(Of DataSet)(isCacheNotFound, cacheing, cacheKey)
            If isCacheNotFound Then
                'Construct The Call
                talentSqlAccessDetail = New TalentDataAccess
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKey
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT GROUP_ID, GROUP_NAME, TYPE_DESC FROM [tbl_agent_group_roles] G, [tbl_agent_group_types] T WHERE G.TYPE_ID = T.TYPE_ID ORDER BY GROUP_NAME"
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_DEFINITION, cacheing, cacheTimeMinutes)
                If (Not (err.HasError)) AndAlso (talentSqlAccessDetail.ResultDataSet IsNot Nothing) Then
                    Me.ResultDataSet = talentSqlAccessDetail.ResultDataSet
                End If
            End If
            If Me.ResultDataSet IsNot Nothing AndAlso Me.ResultDataSet.Tables.Count > 0 Then
                dtOutput = Me.ResultDataSet.Tables(0)
            End If
        Catch ex As Exception
            Throw
        Finally
            talentSqlAccessDetail = Nothing
        End Try

        Return dtOutput
    End Function

#End Region

#Region "Protected Methods"

    ''' <summary>
    ''' Populates the custom dictionary entities. Overridden method called from DBObjectBase
    ''' </summary>
    ''' <param name="dtSourceToPopulate">The dt source to populate.</param>
    Protected Overrides Sub PopulateCustomDictionaryEntities(ByVal dtSourceToPopulate As System.Data.DataTable, ByVal callingModuleName As String)
        If dtSourceToPopulate IsNot Nothing AndAlso dtSourceToPopulate.Rows.Count > 0 Then
            Dim agentPermissionsDictionary As New Dictionary(Of String, String)
            For rowIndex As Integer = 0 To dtSourceToPopulate.Rows.Count - 1
                Dim keyString As String = (Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("PERMISSION_NAME"))).Trim
                If Not agentPermissionsDictionary.ContainsKey(keyString) Then
                    agentPermissionsDictionary.Add(keyString, True)
                End If
            Next
            If agentPermissionsDictionary.Count > 0 Then
                MyBase.CustomObject = agentPermissionsDictionary
            End If
        End If
    End Sub

#End Region

End Class
