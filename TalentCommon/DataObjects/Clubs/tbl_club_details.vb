
Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_club_details based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_club_details
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_club_details"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_club_details" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Inserts the specified club code to tbl_club_details
        ''' </summary>
        ''' <param name="clubCode">The club code.</param>
        ''' <param name="clubDescription">The club description.</param>
        ''' <param name="displaySequence">The display sequence.</param>
        ''' <param name="isDefault">if set to <c>true</c> [is default].</param>
        ''' <param name="validCustomerForwardUrl">The valid customer forward URL.</param>
        ''' <param name="invalidCustomerForwardUrl">The invalid customer forward URL.</param>
        ''' <param name="noiseEncryptionKey">The noise encryption key.</param>
        ''' <param name="agentType">Type of the agent.</param>
        ''' <param name="customerValidationUrl">The customer validation URL.</param>
        ''' <param name="supplynetLoginid">The supplynet loginid.</param>
        ''' <param name="supplynetPassword">The supplynet password.</param>
        ''' <param name="supplynetCompany">The supplynet company.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns></returns>
        Public Function Insert(ByVal clubCode As String, ByVal clubDescription As String, _
                ByVal displaySequence As String, ByVal isDefault As Boolean, _
                ByVal validCustomerForwardUrl As String, ByVal invalidCustomerForwardUrl As String, _
                ByVal noiseEncryptionKey As String, ByVal agentType As String, _
                Optional ByVal customerValidationUrl As String = Nothing, Optional ByVal supplynetLoginid As String = Nothing, _
                Optional ByVal supplynetPassword As String = Nothing, Optional ByVal supplynetCompany As String = Nothing, _
                Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            If customerValidationUrl Is Nothing Then
                customerValidationUrl = String.Empty
            End If
            If supplynetLoginid Is Nothing Then
                supplynetLoginid = String.Empty
            End If

            If supplynetPassword Is Nothing Then
                supplynetPassword = String.Empty
            End If

            If supplynetCompany Is Nothing Then
                supplynetCompany = String.Empty
            End If
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Check Whether the given new club code and agent type already exists or not
                Dim clubCodeExistence As DataTable
                clubCodeExistence = GetClubsByIDClubCodeAgentType("0", clubCode, agentType, False)
                'If the new club is default one then update all others to false for given agent type
                If isDefault Then
                    affectedRows = UpdateDefaultByAgentType(agentType, False)
                End If
                affectedRows = 0
                If (clubCodeExistence.Rows.Count <= 0) Then
                    'Construct The Call
                    talentSqlAccessDetail.Settings = _settings
                    talentSqlAccessDetail.Settings.Cacheing = False
                    talentSqlAccessDetail.Settings.CacheStringExtension = ""
                    'Execute
                    Dim err As New ErrorObj
                    Dim sqlStatement As String = "INSERT INTO TBL_CLUB_DETAILS (" & _
                        "CLUB_CODE, CLUB_DESCRIPTION, DISPLAY_SEQUENCE, IS_DEFAULT, " & _
                        "CUSTOMER_VALIDATION_URL, VALID_CUSTOMER_FORWARD_URL, INVALID_CUSTOMER_FORWARD_URL, NOISE_ENCRYPTION_KEY, " & _
                        "SUPPLYNET_LOGINID, SUPPLYNET_PASSWORD, SUPPLYNET_COMPANY, AGENT_TYPE)" & _
                        "VALUES (" & _
                        "@ClubCode, @ClubDescription, @DisplaySequence, @IsDefault, " & _
                        "@CustomerValidationUrl, @ValidCustomerForwardUrl, @InvalidCustomerForwardUrl, @NoiseEncryptionKey, " & _
                        "@SupplynetLoginid, @SupplynetPassword, @SupplynetCompany, @AgentType) "
                    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClubCode", clubCode))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClubDescription", clubDescription))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DisplaySequence", displaySequence))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsDefault", isDefault))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CustomerValidationUrl", customerValidationUrl))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ValidCustomerForwardUrl", validCustomerForwardUrl))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@InvalidCustomerForwardUrl", invalidCustomerForwardUrl))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NoiseEncryptionKey", noiseEncryptionKey))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SupplynetLoginid", supplynetLoginid))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SupplynetPassword", supplynetPassword))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SupplynetCompany", supplynetCompany))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentType", agentType))
                    'Execute Insert
                    If (givenTransaction Is Nothing) Then
                        err = talentSqlAccessDetail.SQLAccess()
                    Else
                        err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                    End If
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                    End If
                Else
                    affectedRows = 0
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
        ''' Updates the club details for the specified club ID
        ''' </summary>
        ''' <param name="clubID">The club ID.</param>
        ''' <param name="clubCode">The club code.</param>
        ''' <param name="clubDescription">The club description.</param>
        ''' <param name="displaySequence">The display sequence.</param>
        ''' <param name="isDefault">if set to <c>true</c> [is default].</param>
        ''' <param name="validCustomerForwardUrl">The valid customer forward URL.</param>
        ''' <param name="invalidCustomerForwardUrl">The invalid customer forward URL.</param>
        ''' <param name="noiseEncryptionKey">The noise encryption key.</param>
        ''' <param name="agentType">Type of the agent.</param>
        ''' <param name="customerValidationUrl">The customer validation URL.</param>
        ''' <param name="supplynetLoginid">The supplynet loginid.</param>
        ''' <param name="supplynetPassword">The supplynet password.</param>
        ''' <param name="supplynetCompany">The supplynet company.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns></returns>
        Public Function Update(ByVal clubID As String, ByVal clubCode As String, _
                ByVal clubDescription As String, ByVal displaySequence As String, _
                ByVal isDefault As Boolean, ByVal validCustomerForwardUrl As String, _
                ByVal invalidCustomerForwardUrl As String, ByVal noiseEncryptionKey As String, ByVal agentType As String, _
                Optional ByVal customerValidationUrl As String = Nothing, Optional ByVal supplynetLoginid As String = Nothing, _
                Optional ByVal supplynetPassword As String = Nothing, Optional ByVal supplynetCompany As String = Nothing, _
                Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                Dim clubCodeExistence As DataTable
                clubCodeExistence = GetClubsByIDClubCodeAgentType(clubID, clubCode, agentType, False)
                'If the updating club is default one then update all others to false for given agent type
                If isDefault Then
                    affectedRows = UpdateDefaultByAgentType(agentType, False)
                End If
                affectedRows = 0
                If (clubCodeExistence.Rows.Count <= 0) Then
                    'Construct The Call
                    talentSqlAccessDetail.Settings = _settings
                    talentSqlAccessDetail.Settings.Cacheing = False
                    talentSqlAccessDetail.Settings.CacheStringExtension = ""
                    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery

                    'Execute
                    Dim err As New ErrorObj
                    Dim sqlStatement As String = "UPDATE TBL_CLUB_DETAILS " & _
                        "SET CLUB_CODE=@ClubCode, " & _
                        "CLUB_DESCRIPTION=@ClubDescription, " & _
                        "DISPLAY_SEQUENCE=@DisplaySequence, " & _
                        "IS_DEFAULT=@IsDefault, " & _
                        "VALID_CUSTOMER_FORWARD_URL=@ValidCustomerForwardUrl, " & _
                        "INVALID_CUSTOMER_FORWARD_URL=@InvalidCustomerForwardUrl, " & _
                        "NOISE_ENCRYPTION_KEY=@NoiseEncryptionKey, " & _
                        "AGENT_TYPE=@AgentType"

                    If customerValidationUrl IsNot Nothing Then
                        sqlStatement = sqlStatement & ", CUSTOMER_VALIDATION_URL=@CustomerValidationUrl"
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CustomerValidationUrl", customerValidationUrl))
                    End If

                    If supplynetLoginid IsNot Nothing Then
                        sqlStatement = sqlStatement & ", SUPPLYNET_LOGINID=@SupplynetLoginid"
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SupplynetLoginid", supplynetLoginid))
                    End If

                    If supplynetPassword IsNot Nothing Then
                        sqlStatement = sqlStatement & ", SUPPLYNET_PASSWORD=@SupplynetPassword"
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SupplynetPassword", supplynetPassword))
                    End If

                    If supplynetCompany IsNot Nothing Then
                        sqlStatement = sqlStatement & ", SUPPLYNET_COMPANY=@SupplynetCompany"
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SupplynetCompany", supplynetCompany))
                    End If
                    sqlStatement = sqlStatement & " WHERE ID=@ClubID"

                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClubID", clubID))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClubCode", clubCode))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClubDescription", clubDescription))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DisplaySequence", displaySequence))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsDefault", isDefault))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ValidCustomerForwardUrl", validCustomerForwardUrl))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@InvalidCustomerForwardUrl", invalidCustomerForwardUrl))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NoiseEncryptionKey", noiseEncryptionKey))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentType", agentType))
                    'Execute Insert
                    If (givenTransaction Is Nothing) Then
                        err = talentSqlAccessDetail.SQLAccess()
                    Else
                        err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                    End If
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                    End If
                Else
                    affectedRows = 0
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
        ''' Updates the isdefault for all clubs for the given agentType.
        ''' </summary>
        ''' <param name="agentType">Type of the agent.</param>
        ''' <param name="isDefault">if set to <c>true</c> [is default].</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns></returns>
        Public Function UpdateDefaultByAgentType(ByVal agentType As String, ByVal isDefault As Boolean, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "UPDATE TBL_CLUB_DETAILS " & _
                        "SET IS_DEFAULT=@IsDefault " & _
                        "WHERE AGENT_TYPE=@AgentType"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentType", agentType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsDefault", isDefault))
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
        ''' Deletes the club for the given ID.
        ''' </summary>
        ''' <param name="clubID">The club ID.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns></returns>
        Public Function DeleteByID(ByVal clubID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE TBL_CLUB_DETAILS " & _
                "WHERE ID=@ClubID "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClubId", clubID))

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
        ''' Gets the clubs details for the given ID.
        ''' </summary>
        ''' <param name="clubID">The club ID.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function GetClubsByID(ByVal clubID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetClubsByID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(clubID)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClubId", clubID))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM TBL_CLUB_DETAILS WHERE ID=@clubID"

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
        ''' Gets the club details for the given club code, agent type which is not having given club ID.
        ''' </summary>
        ''' <param name="clubID">The club ID.</param>
        ''' <param name="clubCode">The club code.</param>
        ''' <param name="agentType">Type of the agent.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function GetClubsByIDClubCodeAgentType(ByVal clubID As String, ByVal clubCode As String, ByVal agentType As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetClubsByIDClubCodeAgentType")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(clubID) & ToUpper(clubCode) & ToUpper(agentType)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClubID", clubID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClubCode", clubCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentType", agentType))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM TBL_CLUB_DETAILS WHERE CLUB_CODE=@ClubCode AND AGENT_TYPE=@AgentType AND ID<>@ClubID"

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
        ''' Gets all the club details.
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function GetClubsAll(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetClubsAll")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT ID, CLUB_CODE, AGENT_TYPE, CLUB_DESCRIPTION," & _
                                            "DISPLAY_SEQUENCE, IS_DEFAULT, " & _
                                            "VALID_CUSTOMER_FORWARD_URL, INVALID_CUSTOMER_FORWARD_URL, " & _
                                            "NOISE_ENCRYPTION_KEY FROM TBL_CLUB_DETAILS ORDER BY AGENT_TYPE, CLUB_CODE "

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

    End Class
End Namespace

