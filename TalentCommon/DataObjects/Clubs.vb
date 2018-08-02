Imports System.Data.SqlClient
Imports Talent.Common.DataObjects.TableObjects
Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access clubs details and relations to loginID
    ''' </summary>
    <Serializable()> _
        Public Class Clubs
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblClubDetails As tbl_club_details
        Private _tblPartnerUserClub As tbl_partner_user_club

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "Clubs"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Clubs" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Create and Gets the tbl_club_details instance with DESettings
        ''' </summary>
        ''' <value>tbl_club_details instance</value>
        Public ReadOnly Property TblClubDetails() As tbl_club_details
            Get
                If (_tblClubDetails Is Nothing) Then
                    _tblClubDetails = New tbl_club_details(_settings)
                End If
                Return _tblClubDetails
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_partner_user_club with DESettings
        ''' </summary>
        ''' <value>The tbl_partner_user_club instance.</value>
        Public ReadOnly Property TblPartnerUserClub() As tbl_partner_user_club
            Get
                If (_tblPartnerUserClub Is Nothing) Then
                    _tblPartnerUserClub = New tbl_partner_user_club(_settings)
                End If
                Return _tblPartnerUserClub
            End Get
        End Property
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets the clubs by agent type with login ID tbl_club_details joins with tbl_partner_user_club based on club code
        ''' </summary>
        ''' <param name="agentType">Type of the agent.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function GetClubsByAgentTypeLoginID(ByVal agentType As String, ByVal loginID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetClubsByAgentTypeWithLoginID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(agentType)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentType", agentType))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", loginID))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT  " & _
                                                        "TCD.CLUB_CODE, " & _
                                                        "ISNULL(TPUC.AVAILABLE,0) AVAILABLE, " & _
                                                        "ISNULL(TPUC.IS_DEFAULT,0) IS_DEFAULT, " & _
                                                        "TCD.CLUB_DESCRIPTION, " & _
                                                        "TPUC.LOGINID, " & _
                                                        "TPUC.PARTNER, " & _
                                                        "TCD.AGENT_TYPE " & _
                                                        "FROM TBL_CLUB_DETAILS TCD " & _
                                                        "LEFT OUTER JOIN TBL_PARTNER_USER_CLUB TPUC " & _
                                                        "ON (TPUC.CLUB_CODE=TCD.CLUB_CODE " & _
                                                        "AND TPUC.LOGINID=@LoginID) " & _
                                                        "WHERE " & _
                                                        "TCD.AGENT_TYPE=@AgentType " & _
                                                        "ORDER BY TCD.CLUB_CODE"


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

        Public Function GetPartnerUserClubByAgentTypeLoginID(ByVal agentType As String, ByVal loginID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetTblPartnerUserClubByAgentTypeLoginID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(agentType)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentType", agentType))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", loginID))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT " & _
                                                    "TPUC.PARTNER [PARTNER]" & _
                                                    ", TPUC.LOGINID LOGINID" & _
                                                    ", TPUC.CLUB_CODE CLUB_CODE" & _
                                                    ", TPUC.AVAILABLE AVAILABLE" & _
                                                    ", TPUC.IS_DEFAULT IS_DEFAULT" & _
                                                    ", TCD.CLUB_DESCRIPTION CLUB_DESCRIPTION" & _
                                                    ", TCD.DISPLAY_SEQUENCE DISPLAY_SEQUENCE" & _
                                                    ", TCD.CUSTOMER_VALIDATION_URL CUSTOMER_VALIDATION_URL" & _
                                                    ", TCD.VALID_CUSTOMER_FORWARD_URL VALID_CUSTOMER_FORWARD_URL" & _
                                                    ", TCD.INVALID_CUSTOMER_FORWARD_URL INVALID_CUSTOMER_FORWARD_URL" & _
                                                    ", TCD.STANDARD_CUSTOMER_FORWARD_URL STANDARD_CUSTOMER_FORWARD_URL" & _
                                                    ", TCD.NOISE_ENCRYPTION_KEY NOISE_ENCRYPTION_KEY" & _
                                                    ", TCD.SUPPLYNET_LOGINID SUPPLYNET_LOGINID" & _
                                                    ", TCD.SUPPLYNET_PASSWORD SUPPLYNET_PASSWORD" & _
                                                    ", TCD.SUPPLYNET_COMPANY SUPPLYNET_COMPANY" & _
                                                    ", TCD.AGENT_TYPE AGENT_TYPE" & _
                                                    " FROM TBL_PARTNER_USER_CLUB TPUC " & _
                                                    " INNER JOIN TBL_CLUB_DETAILS TCD ON TPUC.CLUB_CODE=TCD.CLUB_CODE " & _
                                                    " WHERE TPUC.AVAILABLE='TRUE' AND LOGINID=@LoginID AND AGENT_TYPE=@AgentType " & _
                                                    " ORDER BY TCD.DISPLAY_SEQUENCE, TCD.CLUB_DESCRIPTION "
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

        Public Function GetTblClubDetailsByAgentType(ByVal agentType As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetTblClubDetailsByAgentType")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(agentType)
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentType", agentType))
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

                Dim selectString As String = "SELECT * FROM tbl_club_details WHERE AGENT_TYPE=@AgentType " & _
                     "order by DISPLAY_SEQUENCE, CLUB_DESCRIPTION"

                talentSqlAccessDetail.CommandElements.CommandText = selectString
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
