Imports System.Data.SqlClient
Namespace DataObjects.TableObjects

    <Serializable()> _
    Public Class tbl_agent_group_permissions
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

#End Region

#Region "Constructors"

        Sub New()

        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_agent_group_permissions" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Save Permissions for selected group 
        ''' </summary>
        ''' <param name="GroupID">GroupID of the group for which the permissions are to be saved.</param>
        ''' <param name="PermissionID">PermissionID to be added or removed.</param>
        ''' <param name="PermissionStatus">Boolean, Whether the permission is Checked or not</param>
        ''' <returns>Number of affected rows</returns>
        ''' <remarks></remarks>
        Public Function SavePermissionsForGroup(ByVal GroupID As Integer, ByVal PermissionID As Integer, ByVal PermissionStatus As Boolean, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
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
                talentSqlAccessDetail.CommandElements.CommandText = "usp_AgentGroupPermission_Update"
                talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_PermissionGroupId", GroupID, SqlDbType.Int, ParameterDirection.Input))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_PermissionId", PermissionID, SqlDbType.Int, ParameterDirection.Input))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_PermissionStatus", PermissionStatus.ToString(), SqlDbType.Bit, ParameterDirection.Input))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_ErrorCode", "0", SqlDbType.Int, ParameterDirection.Output))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_ErrorMessage", "", SqlDbType.VarChar, ParameterDirection.Output))

                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_DEFINITION)
                Else
                    err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_DEFINITION, givenTransaction)
                End If

                If (Not (err.HasError)) AndAlso (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                    ' read output parameter(s) 
                    If Not IsNothing(talentSqlAccessDetail.CommandElements.CommandParameter(3)) AndAlso Not IsDBNull(talentSqlAccessDetail.CommandElements.CommandParameter(3).ParamValue) Then
                        nErrorCode = CInt(talentSqlAccessDetail.CommandElements.CommandParameter(3).ParamValue)
                    End If
                    If Not IsNothing(talentSqlAccessDetail.CommandElements.CommandParameter(4)) AndAlso Not IsDBNull(talentSqlAccessDetail.CommandElements.CommandParameter(4).ParamValue) Then
                        strErrorMessage = talentSqlAccessDetail.CommandElements.CommandParameter(4).ParamValue.ToString()
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

#End Region

    End Class

End Namespace
