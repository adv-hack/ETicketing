Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.Common.Utilities

<Serializable()> _
Public Class DBNoise
    Inherits DBAccess

#Region "Class Level Fields"

    Private _deNoise As DENoise
    Private _moduleName As String
    Private _usersOnLine As Integer
    Private _SuccessfullCall As Boolean
    Private _rowsAffected As Integer

#End Region

#Region "Public Properties"

    Public Property DE_Noise() As DENoise
        Get
            Return _deNoise
        End Get
        Set(ByVal value As DENoise)
            _deNoise = value
        End Set
    End Property
    Public Property ModuleName() As String
        Get
            Return _moduleName
        End Get
        Set(ByVal value As String)
            _moduleName = value
        End Set
    End Property
    Public Property UsersOnLine() As Integer
        Get
            Return _usersOnLine
        End Get
        Set(ByVal value As Integer)
            _usersOnLine = value
        End Set
    End Property
    Public Property SuccessfullCall() As Boolean
        Get
            Return _SuccessfullCall
        End Get
        Set(ByVal value As Boolean)
            _SuccessfullCall = value
        End Set
    End Property
    Public Property RowsAffected() As Integer
        Get
            Return _rowsAffected
        End Get
        Set(ByVal value As Integer)
            _rowsAffected = value
        End Set
    End Property

    Public Property Results As DataSet
#End Region

#Region "Constructor"

    Sub New(ByVal _deNoise_ As DENoise, ByVal _ModuleName_ As String)
        MyBase.New()

        Me.DE_Noise = _deNoise_
        Me.ModuleName = _ModuleName_

    End Sub

#End Region

#Region "Protected Functions"

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj

        Select Case ModuleName
            Case Is = "AddOrUpdateNoiseSession"
                err = AddOrUpdateNoiseSession_SQL2005()
            Case Is = "CheckAndUpdateNoiseSession"
                err = CheckAndUpdateNoiseSession_SQL2005()
            Case Is = "RemoveExpiredNoiseSessions"
                err = RemoveExpiredNoiseSessions_SQL2005()
            Case Is = "RemoveSpecificNoiseSession"
                err = RemoveSpecificNoiseSession_SQL2005()
            Case Is = "RemoveSpecificNoiseSessionByAgentName"
                err = RemoveSpecificNoiseSessionByAgentName_SQL2005()
            Case Is = "CheckAndUpdateAgentNoiseSession"
                err = CheckAndUpdateAgentNoiseSession_SQL2005()
            Case Is = "UpdateNoiseSessionBU"
                err = UpdateNoiseSessionBU_SQL2005()
            Case Is = "CheckForExistingAgentNoiseSession"
                err = CheckForExistingAgentNoiseSession_SQL2005()
            Case Is = "GetNoiseSessionBySessionID"
                err = GetNoiseSessionBySessionID_SQL2005()
        End Select

        Return err
    End Function

    Protected Function GetNoiseSessionBySessionID_SQL2005() As ErrorObj
        Dim selectStr As String = String.Empty
        Dim online As Integer = 0
        Dim err As New ErrorObj
        Dim reader As SqlDataReader
        'Create the Status data table
        Dim DtActiveNoiseSessions As New DataTable("ActiveNoiseSessions")
        ResultDataSet.Tables.Add(DtActiveNoiseSessions)
        With DtActiveNoiseSessions.Columns
            .Add("SessionID", GetType(String))
            .Add("LastActivity", GetType(String))
            .Add("SessionStart", GetType(Date))
            .Add("IsAgent", GetType(String))
            .Add("BusinessUnit", GetType(String))
            .Add("Username", GetType(String))
            .Add("Usage", GetType(String))
            .Add("ClientIP", GetType(String))
            .Add("ServerIP", GetType(String))
            .Add("AgentType", GetType(String))
        End With

        selectStr = " SELECT COUNT(*) FROM tbl_active_noise_sessions WITH (NOLOCK) WHERE SESSIONID = @SessionID"

        Dim cmd As New SqlClient.SqlCommand(selectStr, conSql2005)
        Try
            If conSql2005.State = ConnectionState.Open Then

                cmd.Parameters.Add("@SessionID ", SqlDbType.NVarChar).Value = Me.DE_Noise.SessionID
                cmd.CommandText = selectStr
                reader = cmd.ExecuteReader()
                While reader.HasRows
                    Dim acitveNoiseSessionRow As DataRow = DtActiveNoiseSessions.NewRow
                    acitveNoiseSessionRow("SessionID") = reader("SESSIONID")
                    acitveNoiseSessionRow("LastActivity") = reader("LAST_ACTIVITY")
                    acitveNoiseSessionRow("SessionStart") = reader("SESSION_START")
                    acitveNoiseSessionRow("IsAgent") = reader("IS_AGENT")
                    acitveNoiseSessionRow("BusinessUnit") = reader("BUSINESS_UNIT")
                    acitveNoiseSessionRow("Username") = reader("USERNAME")
                    acitveNoiseSessionRow("Usage") = reader("USAGE")
                    acitveNoiseSessionRow("ClientIP") = reader("CLIENT_IP")
                    acitveNoiseSessionRow("ServerIP") = reader("SERVER_IP")
                    acitveNoiseSessionRow("AgentType") = reader("AGENT_TYPE")
                    DtActiveNoiseSessions.Rows.Add(acitveNoiseSessionRow)
                End While
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "DBNoise01"
            err.ErrorMessage = "Failed to retrieve client data (GetNoiseSessionBySessionID): " + ex.Message
        Finally
            reader.Close()
        End Try
        
    End Function
    Protected Function AddOrUpdateNoiseSession_SQL2005() As ErrorObj
        Dim err As New ErrorObj
        Dim result As Integer = 0
        Dim online As Integer = 0
        Dim selectStr As String = String.Empty
        Dim insertStr As String = String.Empty
        Dim isAgentNoiseSession As Boolean = False
        If Me.DE_Noise.Usage = TblActiveNoiseSessions_Usage.TRADEPLACE Then
            selectStr = " SELECT COUNT(*) FROM tbl_active_noise_sessions WITH (NOLOCK) WHERE IS_AGENT = 'False' AND BUSINESS_UNIT = @BusinessUnit AND USERNAME = @UserName AND USAGE = @Usage "
            insertStr = "     IF EXISTS (" & _
                                        "           SELECT * " & _
                                        "           FROM tbl_active_noise_sessions WITH (NOLOCK)" & _
                                        "           WHERE SESSIONID = @SessionID AND BUSINESS_UNIT = @BusinessUnit " & _
                                        "           AND USERNAME = @UserName AND USAGE = @Usage " & _
                                        "           )" & _
                                        "       UPDATE tbl_active_noise_sessions " & _
                                        "       SET LAST_ACTIVITY = @LastActivity, " & _
                                        "           SESSION_START = @SessionStart " & _
                                        "       WHERE SESSIONID = @SessionID AND BUSINESS_UNIT = @BusinessUnit " & _
                                        "       AND USERNAME = @UserName AND USAGE = @Usage " & _
                                        "    ELSE " & _
                                        "       INSERT INTO tbl_active_noise_sessions " & _
                                        "       (SESSIONID, LAST_ACTIVITY, IS_AGENT, SESSION_START, " & _
                                        "       BUSINESS_UNIT, USERNAME, USAGE, CLIENT_IP, SERVER_IP) " & _
                                        "       VALUES ( " & _
                                        "           @SessionID, " & _
                                        "           @LastActivity, " & _
                                        "           @IsAgent, " & _
                                        "           @SessionStart, " & _
                                        "           @BusinessUnit, " & _
                                        "           @UserName, " & _
                                        "           @Usage, " & _
                                        "           @ClientIP, " & _
                                        "           @ServerIP " & _
                                        "         ) "
        ElseIf Me.DE_Noise.IsAgent Then
            err = AddOrUpdateAgentNoiseSession()
            isAgentNoiseSession = True
        Else
            selectStr = " SELECT COUNT(*) FROM tbl_active_noise_sessions WITH (NOLOCK) WHERE IS_AGENT = 'False' AND BUSINESS_UNIT = @BusinessUnit AND USAGE = @Usage "
            insertStr = "     IF EXISTS (" & _
                                        "           SELECT * " & _
                                        "           FROM tbl_active_noise_sessions WITH (NOLOCK)" & _
                                        "           WHERE SESSIONID = @SessionID AND BUSINESS_UNIT = @BusinessUnit " & _
                                        "           )" & _
                                        "       UPDATE tbl_active_noise_sessions " & _
                                        "       SET LAST_ACTIVITY = @LastActivity, " & _
                                        "           SESSION_START = @SessionStart " & _
                                        "       WHERE SESSIONID = @SessionID AND BUSINESS_UNIT = @BusinessUnit AND USAGE = @Usage " & _
                                        "   ELSE " & _
                                        "       INSERT INTO tbl_active_noise_sessions " & _
                                        "       (SESSIONID, LAST_ACTIVITY, IS_AGENT, SESSION_START, " & _
                                        "       BUSINESS_UNIT, USAGE, CLIENT_IP, SERVER_IP, USERNAME, AGENT_TYPE) " & _
                                        "       VALUES ( " & _
                                        "           @SessionID, " & _
                                        "           @LastActivity, " & _
                                        "           @IsAgent, " & _
                                        "           @SessionStart, " & _
                                        "           @BusinessUnit, " & _
                                        "           @Usage, " & _
                                        "           @ClientIP, " & _
                                        "           @ServerIP, " & _
                                        "           @UserName, " & _
                                        "           @AgentType) "
        End If

        If Not isAgentNoiseSession Then
            Dim cmd As New SqlClient.SqlCommand(selectStr, conSql2005)
            Try
                If conSql2005.State = ConnectionState.Open Then

                    With cmd.Parameters
                        .Clear()
                        .Add("@SessionID ", SqlDbType.NVarChar).Value = Me.DE_Noise.SessionID
                        .Add("@LastActivity ", SqlDbType.DateTime).Value = Me.DE_Noise.LastActivity
                        .Add("@SessionStart ", SqlDbType.DateTime).Value = Me.DE_Noise.LastActivity
                        .Add("@IsAgent ", SqlDbType.Bit).Value = Me.DE_Noise.IsAgent
                        .Add("@AgentType ", SqlDbType.Int).Value = Me.DE_Noise.AgentType
                        .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Me.Settings.BusinessUnit
                        If Me.DE_Noise.Usage = TblActiveNoiseSessions_Usage.TRADEPLACE Then
                            .Add("@UserName", SqlDbType.NVarChar).Value = Me.Settings.LoginId
                        ElseIf Me.DE_Noise.IsAgent Then
                            .Add("@UserName", SqlDbType.NVarChar).Value = Me.DE_Noise.AgentName
                        Else
                            .Add("@UserName", SqlDbType.NVarChar).Value = Me.Settings.LoginId
                        End If
                        .Add("@Usage", SqlDbType.NVarChar).Value = System.Enum.GetName(GetType(TblActiveNoiseSessions_Usage), Me.DE_Noise.Usage)
                        .Add("@ClientIP", SqlDbType.NVarChar).Value = Me.DE_Noise.ClientIP
                        .Add("@ServerIP", SqlDbType.NVarChar).Value = Me.DE_Noise.ServerIP
                    End With

                    online = CInt(cmd.ExecuteScalar)
                    If online < DE_Noise.MaxConcurrentUsers OrElse DE_Noise.MaxConcurrentUsers = -1 Then
                        cmd.CommandText = insertStr
                        result = cmd.ExecuteNonQuery()
                    Else
                        err.HasError = True
                        err.ErrorNumber = "DBNoise01"
                        err.ErrorMessage = "Failed to create noise session - max concurrent users exceeded."
                    End If

                End If
            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = "DBNoise02"
                err.ErrorMessage = ex.Message
            End Try

            Me.SuccessfullCall = Not err.HasError
            Me.RowsAffected = result
        End If
        Return err
    End Function

    Protected Function UpdateNoiseSessionBU_SQL2005() As ErrorObj
        Dim err As New ErrorObj
        Dim result As Integer = 0
        Dim online As Integer = 0
        Dim selectStr As String = String.Empty
        Dim insertStr As String = String.Empty
        Dim isAgentNoiseSession As Boolean = False
        If Me.DE_Noise.Usage = TblActiveNoiseSessions_Usage.TRADEPLACE Then
            'selectStr = " SELECT COUNT(*) FROM tbl_active_noise_sessions WITH (NOLOCK) WHERE IS_AGENT = 'False' AND BUSINESS_UNIT = @BusinessUnit AND USERNAME = @UserName AND USAGE = @Usage "
            'insertStr = "     IF EXISTS (" & _
            '                            "           SELECT * " & _
            '                            "           FROM tbl_active_noise_sessions WITH (NOLOCK)" & _
            '                            "           WHERE SESSIONID = @SessionID AND BUSINESS_UNIT = @BusinessUnit " & _
            '                            "           AND USERNAME = @UserName AND USAGE = @Usage " & _
            '                            "           )" & _
            '                            "       UPDATE tbl_active_noise_sessions " & _
            '                            "       SET LAST_ACTIVITY = @LastActivity, " & _
            '                            "           SESSION_START = @SessionStart " & _
            '                            "       WHERE SESSIONID = @SessionID AND BUSINESS_UNIT = @BusinessUnit " & _
            '                            "       AND USERNAME = @UserName AND USAGE = @Usage " & _
            '                            "    ELSE " & _
            '                            "       INSERT INTO tbl_active_noise_sessions " & _
            '                            "       (SESSIONID, LAST_ACTIVITY, IS_AGENT, SESSION_START, " & _
            '                            "       BUSINESS_UNIT, USERNAME, USAGE, CLIENT_IP, SERVER_IP) " & _
            '                            "       VALUES ( " & _
            '                            "           @SessionID, " & _
            '                            "           @LastActivity, " & _
            '                            "           @IsAgent, " & _
            '                            "           @SessionStart, " & _
            '                            "           @BusinessUnit, " & _
            '                            "           @UserName, " & _
            '                            "           @Usage, " & _
            '                            "           @ClientIP, " & _
            '                            "           @ServerIP " & _
            '                            "         ) "
        ElseIf Me.DE_Noise.IsAgent Then
            'err = AddOrUpdateAgentNoiseSession()
            'isAgentNoiseSession = True
        Else
            selectStr = " SELECT COUNT(*) FROM tbl_active_noise_sessions WITH (NOLOCK) WHERE IS_AGENT = 'False' AND BUSINESS_UNIT = @BusinessUnit AND USAGE = @Usage "
            insertStr = "     IF EXISTS (" & _
                                        "           SELECT * " & _
                                        "           FROM tbl_active_noise_sessions WITH (NOLOCK)" & _
                                        "           WHERE SESSIONID = @SessionID AND BUSINESS_UNIT = @BusinessUnit " & _
                                        "           )" & _
                                        "       UPDATE tbl_active_noise_sessions " & _
                                        "       SET BUSINESS_UNIT = @BusinessUnitNew " & _
                                        "       WHERE SESSIONID = @SessionID AND BUSINESS_UNIT = @BusinessUnit AND USAGE = @Usage "

            '"   ELSE " & _
            '"       INSERT INTO tbl_active_noise_sessions " & _
            '"       (SESSIONID, LAST_ACTIVITY, IS_AGENT, SESSION_START, " & _
            '"       BUSINESS_UNIT, USAGE, CLIENT_IP, SERVER_IP, USERNAME, AGENT_TYPE) " & _
            '"       VALUES ( " & _
            '"           @SessionID, " & _
            '"           @LastActivity, " & _
            '"           @IsAgent, " & _
            '"           @SessionStart, " & _
            '"           @BusinessUnit, " & _
            '"           @Usage, " & _
            '"           @ClientIP, " & _
            '"           @ServerIP, " & _
            '"           @UserName, " & _
            '"           @AgentType) "
        End If

        If Not isAgentNoiseSession Then
            Dim cmd As New SqlClient.SqlCommand(selectStr, conSql2005)
            Try
                If conSql2005.State = ConnectionState.Open Then

                    With cmd.Parameters
                        .Clear()
                        .Add("@SessionID ", SqlDbType.NVarChar).Value = Me.DE_Noise.SessionID
                        '                        .Add("@LastActivity ", SqlDbType.DateTime).Value = Me.DE_Noise.LastActivity
                        '                        .Add("@SessionStart ", SqlDbType.DateTime).Value = Me.DE_Noise.LastActivity
                        .Add("@IsAgent ", SqlDbType.Bit).Value = Me.DE_Noise.IsAgent
                        .Add("@AgentType ", SqlDbType.Int).Value = Me.DE_Noise.AgentType
                        .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Me.Settings.BusinessUnit
                        .Add("@BusinessUnitNew", SqlDbType.NVarChar).Value = Me.Settings.BusinessUnitNew
                        If Me.DE_Noise.Usage = TblActiveNoiseSessions_Usage.TRADEPLACE Then
                            .Add("@UserName", SqlDbType.NVarChar).Value = Me.Settings.LoginId
                        ElseIf Me.DE_Noise.IsAgent Then
                            .Add("@UserName", SqlDbType.NVarChar).Value = Me.DE_Noise.AgentName
                        Else
                            .Add("@UserName", SqlDbType.NVarChar).Value = Me.Settings.LoginId
                        End If
                        .Add("@Usage", SqlDbType.NVarChar).Value = System.Enum.GetName(GetType(TblActiveNoiseSessions_Usage), Me.DE_Noise.Usage)
                        '                        .Add("@ClientIP", SqlDbType.NVarChar).Value = Me.DE_Noise.ClientIP
                        '                        .Add("@ServerIP", SqlDbType.NVarChar).Value = Me.DE_Noise.ServerIP
                    End With

                    online = CInt(cmd.ExecuteScalar)
                    If online < DE_Noise.MaxConcurrentUsers OrElse DE_Noise.MaxConcurrentUsers = -1 Then
                        cmd.CommandText = insertStr
                        result = cmd.ExecuteNonQuery()
                    Else
                        err.HasError = True
                        err.ErrorNumber = "DBNoise01"
                        err.ErrorMessage = "Failed to create noise session - max concurrent users exceeded."
                    End If

                End If
            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = "DBNoise02"
                err.ErrorMessage = ex.Message
            End Try

            Me.SuccessfullCall = Not err.HasError
            Me.RowsAffected = result
        End If
        Return err
    End Function

    Protected Function CheckAndUpdateNoiseSession_SQL2005() As ErrorObj
        Dim err As New ErrorObj
        Dim result As Integer = 0
        Dim checkNoMaxKeepAliveStr As String = String.Empty
        Dim checkWithMaxKeepAliveStr As String = String.Empty
        Dim deleteStr As String = String.Empty

        If Me.DE_Noise.Usage = TblActiveNoiseSessions_Usage.TRADEPLACE Then
            checkNoMaxKeepAliveStr = "     IF EXISTS (" & _
                                                                "           SELECT * " & _
                                                                "           FROM tbl_active_noise_sessions WITH (NOLOCK)" & _
                                                                "           WHERE SESSIONID = @SessionID " & _
                                                                "           AND BUSINESS_UNIT = @BusinessUnit AND USERNAME = @UserName AND USAGE = @Usage " & _
                                                                "           )" & _
                                                                "       UPDATE tbl_active_noise_sessions " & _
                                                                "       SET LAST_ACTIVITY = @LastActivity " & _
                                                                "       WHERE SESSIONID = @SessionID " & _
                                                                "       AND BUSINESS_UNIT = @BusinessUnit AND USERNAME = @UserName AND USAGE = @Usage " & _
                                                                "  "

            checkWithMaxKeepAliveStr = "    IF EXISTS (" & _
                                                       "           SELECT * " & _
                                                       "           FROM tbl_active_noise_sessions WITH (NOLOCK)" & _
                                                       "           WHERE SESSIONID = @SessionID " & _
                                                       "           AND BUSINESS_UNIT = @BusinessUnit AND USERNAME = @UserName AND USAGE = @Usage" & _
                                                       "           )" & _
                                                       "       UPDATE tbl_active_noise_sessions " & _
                                                       "       SET LAST_ACTIVITY = @LastActivity " & _
                                                       "       WHERE SESSIONID = @SessionID " & _
                                                       "       AND BUSINESS_UNIT = @BusinessUnit AND USERNAME = @UserName AND USAGE = @Usage"

            deleteStr = " DELETE FROM tbl_active_noise_sessions " & _
                                        " WHERE SESSIONID = @SessionID " & _
                                        " AND BUSINESS_UNIT = @BusinessUnit AND USERNAME = @UserName AND USAGE = @Usage"
        Else
            checkNoMaxKeepAliveStr = "     IF EXISTS (" & _
                                                                "           SELECT * " & _
                                                                "           FROM tbl_active_noise_sessions WITH (NOLOCK)" & _
                                                                "           WHERE SESSIONID = @SessionID " & _
                                                                "           AND LAST_ACTIVITY >= @LastActivityThreshold " & _
                                                                "           AND BUSINESS_UNIT = @BusinessUnit AND USAGE = @Usage " & _
                                                                "           )" & _
                                                                "       UPDATE tbl_active_noise_sessions " & _
                                                                "       SET LAST_ACTIVITY = @LastActivity " & _
                                                                "       WHERE SESSIONID = @SessionID " & _
                                                                "       AND BUSINESS_UNIT = @BusinessUnit AND USAGE = @Usage " & _
                                                                "  "

            checkWithMaxKeepAliveStr = "    IF EXISTS (" & _
                                                       "           SELECT * " & _
                                                       "           FROM tbl_active_noise_sessions WITH (NOLOCK)" & _
                                                       "           WHERE SESSIONID = @SessionID " & _
                                                       "           AND LAST_ACTIVITY >= @LastActivityThreshold " & _
                                                       "           AND SESSION_START >= @SessionStart " & _
                                                       "           AND BUSINESS_UNIT = @BusinessUnit AND USAGE = @Usage " & _
                                                       "           )" & _
                                                       "       UPDATE tbl_active_noise_sessions " & _
                                                       "       SET LAST_ACTIVITY = @LastActivity " & _
                                                       "       WHERE SESSIONID = @SessionID " & _
                                                       "       AND BUSINESS_UNIT = @BusinessUnit AND USAGE = @Usage "

            deleteStr = " DELETE FROM tbl_active_noise_sessions " & _
                                        " WHERE SESSIONID = @SessionID " & _
                                        " AND BUSINESS_UNIT = @BusinessUnit AND USAGE = @Usage AND IS_AGENT = 'FALSE'"
        End If

        Dim cmd As New SqlClient.SqlCommand("", conSql2005)
        If Me.DE_Noise.MaxSessionKeepAliveMinutes > 0 Then
            cmd.CommandText = checkWithMaxKeepAliveStr
        Else
            cmd.CommandText = checkNoMaxKeepAliveStr
        End If

        Try
            If conSql2005.State = ConnectionState.Open Then
                With cmd.Parameters
                    .Clear()
                    .Add("@SessionID ", SqlDbType.NVarChar).Value = Me.DE_Noise.SessionID
                    .Add("@LastActivity ", SqlDbType.DateTime).Value = Me.DE_Noise.LastActivity
                    .Add("@LastActivityThreshold ", SqlDbType.DateTime).Value = Me.DE_Noise.LastActivityThreshold
                    .Add("@SessionStart ", SqlDbType.DateTime).Value = Me.DE_Noise.LastActivity.AddMinutes(-Me.DE_Noise.MaxSessionKeepAliveMinutes)
                    .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Me.Settings.BusinessUnit
                    If Me.DE_Noise.Usage = TblActiveNoiseSessions_Usage.TRADEPLACE Then
                        .Add("@UserName", SqlDbType.NVarChar).Value = Me.Settings.LoginId
                    End If
                    .Add("@Usage", SqlDbType.NVarChar).Value = System.Enum.GetName(GetType(TblActiveNoiseSessions_Usage), Me.DE_Noise.Usage)
                End With

                result = cmd.ExecuteNonQuery()

                If result <= 0 AndAlso Me.DE_Noise.MaxSessionKeepAliveMinutes > 0 Then
                    cmd.CommandText = deleteStr
                    cmd.ExecuteNonQuery()
                End If

            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACDBNO-CUA2"
            err.ErrorMessage = ex.Message
        End Try

        Me.SuccessfullCall = Not err.HasError
        Me.RowsAffected = result

        If Me.SuccessfullCall Then
            err = GetIsAgentValueBySessionID()
        End If
        Return err
    End Function

    Protected Function RemoveExpiredNoiseSessions_SQL2005() As ErrorObj
        Dim err As New ErrorObj
        Dim result As Integer = 0
        Dim online As Integer = 0

        Const deleteStr As String = " DELETE FROM tbl_active_noise_sessions WHERE LAST_ACTIVITY < @LastActivityThreshold " & _
                                    "   AND IS_AGENT = 'False' AND BUSINESS_UNIT = @BusinessUnit "
        Const selectStr As String = " SELECT COUNT(*) FROM tbl_active_noise_sessions WITH (NOLOCK) WHERE IS_AGENT = 'False' AND BUSINESS_UNIT = @BusinessUnit "
        Dim cmd As New SqlClient.SqlCommand(deleteStr, conSql2005)
        Try
            If conSql2005.State = ConnectionState.Open Then
                With cmd.Parameters
                    .Add("@LastActivityThreshold ", SqlDbType.DateTime).Value = Me.DE_Noise.LastActivityThreshold
                    .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Me.Settings.BusinessUnit
                End With

                result = cmd.ExecuteNonQuery()
                Try
                    cmd.CommandText = selectStr
                    online = CInt(cmd.ExecuteScalar)
                Catch ex As Exception

                End Try

            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "DBNoise03"
            err.ErrorMessage = ex.Message
        End Try

        Me.SuccessfullCall = Not err.HasError
        Me.RowsAffected = result
        Me.UsersOnLine = online
        Return err
    End Function

    Protected Function RemoveExpiredAgentNoiseSessions_SQL2005() As ErrorObj
        Dim err As New ErrorObj
        Dim result As Integer = 0
        Dim online As Integer = 0
        Const deleteStr As String = " DELETE FROM tbl_active_noise_sessions WHERE LAST_ACTIVITY < @LastActivityThreshold " & _
                                    "   AND IS_AGENT = 'True' AND BUSINESS_UNIT = @BusinessUnit "
        Const selectStr As String = " SELECT COUNT(*) FROM tbl_active_noise_sessions WITH (NOLOCK) WHERE IS_AGENT = 'True' AND BUSINESS_UNIT = @BusinessUnit "
        Dim cmd As New SqlClient.SqlCommand(deleteStr, conSql2005)
        Try
            If conSql2005.State = ConnectionState.Open Then
                With cmd.Parameters
                    .Add("@LastActivityThreshold ", SqlDbType.DateTime).Value = Now.AddDays(-1)
                    .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Me.Settings.BusinessUnit
                End With

                result = cmd.ExecuteNonQuery()
                Try
                    cmd.CommandText = selectStr
                    online = CInt(cmd.ExecuteScalar)
                Catch ex As Exception

                End Try

            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "DBNoise04"
            err.ErrorMessage = ex.Message
        End Try

        Me.SuccessfullCall = Not err.HasError
        Me.RowsAffected = result
        Me.UsersOnLine = online
        Return err
    End Function

    Protected Function RemoveSpecificNoiseSession_SQL2005() As ErrorObj
        Dim err As New ErrorObj
        Dim result As Integer = 0
        Dim online As Integer = 0
        Const deleteStr As String = " DELETE FROM tbl_active_noise_sessions WHERE SESSIONID = @SessionID AND BUSINESS_UNIT = @BusinessUnit "
        Const selectStr As String = " SELECT COUNT(*) FROM tbl_active_noise_sessions WITH (NOLOCK) WHERE IS_AGENT = 'False' AND BUSINESS_UNIT = @BusinessUnit "
        Dim cmd As New SqlClient.SqlCommand(deleteStr, conSql2005)
        Try
            If conSql2005.State = ConnectionState.Open Then
                With cmd.Parameters
                    .Add("@SessionID ", SqlDbType.NVarChar).Value = Me.DE_Noise.SessionID
                    .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Me.Settings.BusinessUnit
                End With

                result = cmd.ExecuteNonQuery()
                cmd.CommandText = selectStr
                online = CInt(cmd.ExecuteScalar)
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "DBNoise05"
            err.ErrorMessage = ex.Message
        End Try

        Me.SuccessfullCall = Not err.HasError
        Me.RowsAffected = result
        Me.UsersOnLine = online
        Return err
    End Function

    Protected Function RemoveSpecificNoiseSessionByAgentName_SQL2005() As ErrorObj
        Dim err As New ErrorObj
        Dim result As Integer = 0
        Const deleteStr As String = "DELETE FROM tbl_active_noise_sessions WHERE IS_AGENT = 'True' AND USERNAME = @UserName AND BUSINESS_UNIT = @BusinessUnit"
        Dim cmd As New SqlClient.SqlCommand(deleteStr, conSql2005)
        Try
            If conSql2005.State = ConnectionState.Open Then
                With cmd.Parameters
                    .Add("@UserName", SqlDbType.NVarChar).Value = Me.DE_Noise.AgentName
                    .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Me.Settings.BusinessUnit
                End With
                result = cmd.ExecuteScalar()
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "DBNoise05"
            err.ErrorMessage = ex.Message
        End Try

        Me.SuccessfullCall = Not err.HasError
        Me.RowsAffected = result
        Return err
    End Function

    Protected Function CheckAndUpdateAgentNoiseSession_SQL2005() As ErrorObj
        Dim err As ErrorObj = GetIsAgentValueBySessionID()
        If Not err.HasError AndAlso DE_Noise.IsAgent Then err = AddOrUpdateAgentNoiseSession()
        Return err
    End Function

    Protected Function CheckForExistingAgentNoiseSession_SQL2005() As ErrorObj
        Dim err As New ErrorObj
        Dim result As Integer = 0
        Dim online As Integer = 0
        Const selectStr As String = " SELECT COUNT(*) FROM tbl_active_noise_sessions WITH (NOLOCK) WHERE IS_AGENT = 'True' AND BUSINESS_UNIT = @BusinessUnit AND USERNAME = @UserName"
        Dim cmd As New SqlClient.SqlCommand(selectStr, conSql2005)
        Try
            If conSql2005.State = ConnectionState.Open Then
                With cmd.Parameters
                    .Add("@UserName ", SqlDbType.NVarChar).Value = Me.DE_Noise.AgentName
                    .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Me.Settings.BusinessUnit
                End With
                result = cmd.ExecuteScalar()
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "DBNoise08"
            err.ErrorMessage = ex.Message
        End Try

        Me.SuccessfullCall = Not err.HasError
        Me.UsersOnLine = result
        Return err
    End Function

#End Region

#Region "Private Functions"

    Private Function GetIsAgentValueBySessionID() As ErrorObj
        Dim err As New ErrorObj
        Const selectStr As String = "SELECT USERNAME, AGENT_TYPE FROM tbl_active_noise_sessions WITH (NOLOCK) WHERE SESSIONID = @SessionID AND BUSINESS_UNIT = @BusinessUnit AND IS_AGENT = 1"
        Dim cmd As New SqlClient.SqlCommand(selectStr, conSql2005)
        Dim sqlReader As SqlClient.SqlDataReader

        Try
            If cmd.Connection.State = ConnectionState.Open Then
                With cmd.Parameters
                    .Add("@SessionID ", SqlDbType.NVarChar).Value = Me.DE_Noise.SessionID
                    .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Me.Settings.BusinessUnit
                End With
                sqlReader = cmd.ExecuteReader
                While sqlReader.Read()
                    DE_Noise.IsAgent = True
                    DE_Noise.AgentType = sqlReader.Item("AGENT_TYPE")
                    DE_Noise.AgentName = sqlReader.Item("USERNAME")
                End While
                sqlReader.Close()
                sqlReader = Nothing
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "DBNoise06"
            err.ErrorMessage = ex.Message
        End Try

        Me.SuccessfullCall = Not err.HasError
        Return err
    End Function

    Private Function AddOrUpdateAgentNoiseSession() As ErrorObj
        Dim err As New ErrorObj
        Dim result As Integer = 0
        Dim updateString As String = String.Empty

        updateString = "     IF EXISTS (" & _
                                    "           SELECT * " & _
                                    "           FROM tbl_active_noise_sessions WITH (NOLOCK)" & _
                                    "           WHERE SESSIONID = @SessionID AND BUSINESS_UNIT = @BusinessUnit" & _
                                    "           )" & _
                                    "       UPDATE tbl_active_noise_sessions " & _
                                    "       SET LAST_ACTIVITY = @LastActivity, " & _
                                    "           SESSION_START = @SessionStart, " & _
                                    "           IS_AGENT = @IsAgent, " & _
                                    "           AGENT_TYPE = @AgentType, " & _
                                    "           USERNAME = @UserName, " & _
                                    "           CLIENT_IP = @ClientIP, " & _
                                    "           SERVER_IP = @ServerIP " & _
                                    "       WHERE SESSIONID = @SessionID AND BUSINESS_UNIT = @BusinessUnit AND USAGE = @Usage " & _
                                    "   ELSE " & _
                                    "       INSERT INTO tbl_active_noise_sessions " & _
                                    "       (SESSIONID, LAST_ACTIVITY, IS_AGENT, SESSION_START, " & _
                                    "       BUSINESS_UNIT, USAGE, CLIENT_IP, SERVER_IP, USERNAME, AGENT_TYPE) " & _
                                    "       VALUES ( " & _
                                    "           @SessionID, " & _
                                    "           @LastActivity, " & _
                                    "           @IsAgent, " & _
                                    "           @SessionStart, " & _
                                    "           @BusinessUnit, " & _
                                    "           @Usage, " & _
                                    "           @ClientIP, " & _
                                    "           @ServerIP, " & _
                                    "           @UserName, " & _
                                    "           @AgentType) "

        Dim cmd As New SqlClient.SqlCommand(updateString, conSql2005)
        Try
            If conSql2005.State = ConnectionState.Open Then
                With cmd.Parameters
                    .Clear()
                    .Add("@SessionID ", SqlDbType.NVarChar).Value = Me.DE_Noise.SessionID
                    .Add("@LastActivity ", SqlDbType.DateTime).Value = Me.DE_Noise.LastActivity
                    .Add("@SessionStart ", SqlDbType.DateTime).Value = Me.DE_Noise.LastActivity
                    .Add("@IsAgent ", SqlDbType.Bit).Value = Me.DE_Noise.IsAgent
                    .Add("@AgentType ", SqlDbType.Int).Value = Me.DE_Noise.AgentType
                    .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Me.Settings.BusinessUnit
                    .Add("@UserName", SqlDbType.NVarChar).Value = Me.DE_Noise.AgentName
                    .Add("@Usage", SqlDbType.NVarChar).Value = System.Enum.GetName(GetType(TblActiveNoiseSessions_Usage), Me.DE_Noise.Usage)
                    .Add("@ClientIP", SqlDbType.NVarChar).Value = Me.DE_Noise.ClientIP
                    .Add("@ServerIP", SqlDbType.NVarChar).Value = Me.DE_Noise.ServerIP
                End With
                result = cmd.ExecuteNonQuery()
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "DBNoise07"
            err.ErrorMessage = ex.Message
        End Try

        Me.SuccessfullCall = Not err.HasError
        Me.RowsAffected = result
        Return err
    End Function

#End Region

End Class