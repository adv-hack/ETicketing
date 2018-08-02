Option Strict On
Imports System.Data.SqlClient
Imports System.Data
Partial Class UserControls_copyBU
    Inherits System.Web.UI.UserControl

    Private _CLIENT_NAME As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Request.QueryString("ID") Is Nothing Then
            _CLIENT_NAME = Request.QueryString("ID")
            If Not Page.IsPostBack Then preLoadFields()
        End If
    End Sub

    Sub preLoadFields()
        txtCopyFrom.Text = _CLIENT_NAME
        'NoiseURLs
        txtNoiseURLs.Text = TalentUtilities.getSQLVal("SELECT [VALUE] FROM TBL_CLIENT_DEFAULTS WHERE CLIENT_NAME = '" & _CLIENT_NAME & "' AND DEFAULT_NAME = 'NOISE_URLS'")
        txtSPG.Text = TalentUtilities.getSQLVal("SELECT [VALUE] FROM TBL_CLIENT_DEFAULTS WHERE CLIENT_NAME = '" & _CLIENT_NAME & "' AND DEFAULT_NAME = 'STORED_PROCEDURE_GROUP'")
        txtNoiseThres.Text = TalentUtilities.getSQLVal("SELECT [VALUE] FROM TBL_CLIENT_DEFAULTS WHERE CLIENT_NAME = '" & _CLIENT_NAME & "' AND DEFAULT_NAME = 'NOISE_THRESHOLD'")
    End Sub

    Protected Sub btnCopy_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCopy.Click
        'Check for Empty Strings on Mandatory Fields and Validate Input
        lblError.Text = ""
        Dim errorMsg As String = validateFields()
        If errorMsg <> "" Then ' Will return error message if any problems found
            lblError.Text = errorMsg
        Else
            'Continue Processing
            doCopyBU()
        End If
    End Sub

    Function validateFields() As String
        Dim errorMsg As String = ""
        Dim mand As String = " is mandatory"

        'Check for empty fields
        If txtCopyFrom.Text.Length = 0 Then Return lblCopyFrom.Text & mand
        If txtCopyTo.Text.Length = 0 Then Return lblCopyTo.Text & mand
        If txtNoiseURLs.Text.Length = 0 Then Return lblNoiseURLs.Text & mand
        If txtStadiumCode.Text.Length = 0 Then Return lblStadiumCode.Text & mand
        If txtBU.Text.Length = 0 Then Return lblBU.Text & mand
        If txtNoiseKey.Text.Length = 0 Then Return lblNoiseKey.Text & mand
        If txtSPG.Text.Length = 0 Then Return lblStoredProc.Text & mand
        If txtNoiseThres.Text.Length = 0 Then Return lblNoiseThresh.Text & mand

        'Validate Fields
        If CType(TalentUtilities.getSQLVal("SELECT COUNT(1) FROM TBL_CLIENT_BACKEND_SERVERS WHERE CLIENT_NAME = '" & txtCopyTo.Text & "'"), Integer) > 0 Then Return "Chosen 'BU Copy To' already exists, please choose another."

        Return errorMsg
    End Function

    Sub doCopyBU()
        Dim output As StringBuilder = New StringBuilder()
        Dim trans As SqlTransaction
        Dim conn As SqlConnection
        Dim comm As SqlCommand
        Dim result As Integer
        Try
            conn = New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
            conn.Open()
            trans = conn.BeginTransaction()

            'tbl_client_backend_servers
            comm = New SqlCommand( _
                "   insert into tbl_client_backend_servers (" & _
                "       CLIENT_NAME," & _
                "       IP_ADDRESS," & _
                "       TICKETING_CLIENT," & _
                "       EBUSINESS_CLIENT," & _
                "       LIVE_USER," & _
                "       LIVE_PASSWORD," & _
                "       TEST_USER," & _
                "       TEST_PASSWORD" & _
                "   )" & _
                "   select" & _
                "       '" & txtCopyTo.Text & "'," & _
                "       IP_ADDRESS," & _
                "       TICKETING_CLIENT," & _
                "       EBUSINESS_CLIENT," & _
                "       LIVE_USER," & _
                "       LIVE_PASSWORD," & _
                "       TEST_USER," & _
                "       TEST_PASSWORD" & _
                "   from tbl_client_backend_servers" & _
                "   where CLIENT_NAME = '" & txtCopyFrom.Text & "'", conn, trans)
            result = comm.ExecuteNonQuery()
            output.Append("Inserted " & result & " record(s) into tbl_client_backend_servers<br>")
            output.AppendLine()

            'tbl_client_modules
            comm = New SqlCommand( _
                "   insert into tbl_client_modules (" & _
                "       CLIENT_NAME," & _
                "       MODULE," & _
                "       AUTO_PROCESS," & _
                "       AUTO_PROCESS_IN_MINUTES," & _
                "       PROCESS_START_TIME," & _
                "       PROCESS_END_TIME" & _
                "   )" & _
                "   select" & _
                "       '" & txtCopyTo.Text & "'," & _
                "       MODULE," & _
                "       AUTO_PROCESS," & _
                "       AUTO_PROCESS_IN_MINUTES," & _
                "       PROCESS_START_TIME," & _
                "       PROCESS_END_TIME" & _
                "   from tbl_client_modules" & _
                "   where CLIENT_NAME = '" & txtCopyFrom.Text & "'", conn, trans)
            result = comm.ExecuteNonQuery()
            output.Append("Inserted " & result & " record(s) into tbl_client_modules<br>")
            output.AppendLine()

            'tbl_client_versions
            comm = New SqlCommand( _
                "   insert into tbl_client_versions (" & _
                "       CLIENT_NAME," & _
                "       LIVE_OR_TEST," & _
                "       DEFAULT_BUSINESS_UNIT," & _
                "       DEFAULT_PARTNER," & _
                "       DEFAULT_LANGUAGE," & _
                "       VERSION_NO," & _
                "       SUB_VERSION_NO," & _
                "       PTF_NO," & _
                "       CLIENT_SPECIFIC_NO" & _
                "   )" & _
                "   select" & _
                "       '" & txtCopyTo.Text & "'," & _
                "       LIVE_OR_TEST," & _
                "       '" & txtCopyTo.Text & "'," & _
                "       DEFAULT_PARTNER," & _
                "       DEFAULT_LANGUAGE," & _
                "       VERSION_NO," & _
                "       SUB_VERSION_NO," & _
                "       PTF_NO," & _
                "       CLIENT_SPECIFIC_NO" & _
                "   from tbl_client_versions" & _
                "   where CLIENT_NAME = '" & txtCopyFrom.Text & "'", conn, trans)
            result = comm.ExecuteNonQuery()
            output.Append("Inserted " & result & " record(s) into tbl_client_versions<br>")
            output.AppendLine()

            'tbl_client_web_servers
            comm = New SqlCommand( _
                "   insert into tbl_client_web_servers (" & _
                "       CLIENT_NAME," & _
                "       SERVER_NAME," & _
                "       LIVE_OR_TEST," & _
                "       SQL_DATABASE_NAME," & _
                "       SQL_USER," & _
                "       SQL_PASSWORD" & _
                "   )" & _
                "   select" & _
                "       '" & txtCopyTo.Text & "'," & _
                "       SERVER_NAME," & _
                "       LIVE_OR_TEST," & _
                "       SQL_DATABASE_NAME," & _
                "       SQL_USER," & _
                "       SQL_PASSWORD" & _
                "   from tbl_client_web_servers" & _
                "   where CLIENT_NAME = '" & txtCopyFrom.Text & "'", conn, trans)
            result = comm.ExecuteNonQuery()
            output.Append("Inserted " & result & " record(s) into tbl_client_web_servers<br>")
            output.AppendLine()

            'tbl_client_defaults
            comm = New SqlCommand( _
                "   insert into tbl_client_defaults (" & _
                "       CLIENT_NAME," & _
                "       DEFAULT_NAME," & _
                "       [VALUE]" & _
                "   )" & _
                "   select" & _
                "       '" & txtCopyTo.Text & "'," & _
                "       DEFAULT_NAME," & _
                "       [VALUE]" & _
                "   from tbl_client_defaults" & _
                "   where CLIENT_NAME = '" & txtCopyFrom.Text & "' " & _
                "   and isnull(DEFAULT_NAME,'') <> 'BUSINESS_UNIT'", conn, trans)
            result = comm.ExecuteNonQuery()
            output.Append("Inserted " & result & " record(s) into tbl_client_defaults<br>")
            output.AppendLine()

            'update / insert BUSINESS_UNIT on tbl_client_defaults for the new Client
            comm = New SqlCommand( _
                "IF (" & _
                "   select count(1) from tbl_client_defaults " & _
                "   where client_name = '" & txtCopyTo.Text & "' " & _
                "   and DEFAULT_NAME = 'BUSINESS_UNIT'" & _
                ") = 0 begin " & _
                "   insert into tbl_client_defaults (CLIENT_NAME, DEFAULT_NAME, [VALUE]) " & _
                "   values ('" & txtCopyTo.Text & "', 'BUSINESS_UNIT', '" & txtBU.Text & "') " & _
                "end else begin " & _
                "   update tbl_client_defaults set [VALUE] = '" & txtBU.Text & "'" & _
                "   where CLIENT_NAME = '" & txtCopyTo.Text & "' and DEFAULT_NAME = 'BUSINESS_UNIT' " & _
                "end", conn, trans)
            result = comm.ExecuteNonQuery()
            output.Append("Set BUSINESS_UNIT, " & result & " Row(s) Affected<br>")
            output.AppendLine()

            'update / insert NOISE_URLS on tbl_client_defaults for the new Client
            comm = New SqlCommand( _
                "IF (" & _
                "   select count(1) from tbl_client_defaults " & _
                "   where client_name = '" & txtCopyTo.Text & "' " & _
                "   and DEFAULT_NAME = 'NOISE_URLS'" & _
                ") = 0 begin " & _
                "   insert into tbl_client_defaults (CLIENT_NAME, DEFAULT_NAME, [VALUE]) " & _
                "   values ('" & txtCopyTo.Text & "', 'NOISE_URLS', '" & txtNoiseURLs.Text & "') " & _
                "end else begin " & _
                "   update tbl_client_defaults set [VALUE] = '" & txtNoiseURLs.Text & "'" & _
                "   where CLIENT_NAME = '" & txtCopyTo.Text & "' and DEFAULT_NAME = 'NOISE_URLS' " & _
                "end", conn, trans)
            result = comm.ExecuteNonQuery()
            output.Append("Set NOISE_URLS, " & result & " Row(s) Affected<br>")
            output.AppendLine()

            'update / insert NOISE_KEY on tbl_client_defaults for the new Client
            comm = New SqlCommand( _
                "IF (" & _
                "   select count(1) from tbl_client_defaults " & _
                "   where client_name = '" & txtCopyTo.Text & "' " & _
                "   and DEFAULT_NAME = 'NOISE_KEY'" & _
                ") = 0 begin " & _
                "   insert into tbl_client_defaults (CLIENT_NAME, DEFAULT_NAME, [VALUE]) " & _
                "   values ('" & txtCopyTo.Text & "', 'NOISE_KEY', '" & txtNoiseKey.Text & "') " & _
                "end else begin " & _
                "   update tbl_client_defaults set [VALUE] = '" & txtNoiseKey.Text & "'" & _
                "   where CLIENT_NAME = '" & txtCopyTo.Text & "' and DEFAULT_NAME = 'NOISE_KEY' " & _
                "end", conn, trans)
            result = comm.ExecuteNonQuery()
            output.Append("Set NOISE_KEY, " & result & " Row(s) Affected<br>")
            output.AppendLine()

            'update / insert NOISE_THRESHOLD on tbl_client_defaults for the new Client
            comm = New SqlCommand( _
                "IF (" & _
                "   select count(1) from tbl_client_defaults " & _
                "   where client_name = '" & txtCopyTo.Text & "' " & _
                "   and DEFAULT_NAME = 'NOISE_THRESHOLD'" & _
                ") = 0 begin " & _
                "   insert into tbl_client_defaults (CLIENT_NAME, DEFAULT_NAME, [VALUE]) " & _
                "   values ('" & txtCopyTo.Text & "', 'NOISE_THRESHOLD', '" & txtNoiseThres.Text & "') " & _
                "end else begin " & _
                "   update tbl_client_defaults set [VALUE] = '" & txtNoiseThres.Text & "'" & _
                "   where CLIENT_NAME = '" & txtCopyTo.Text & "' and DEFAULT_NAME = 'NOISE_THRESHOLD' " & _
                "end", conn, trans)
            result = comm.ExecuteNonQuery()
            output.Append("Set NOISE_THRESHOLD, " & result & " Row(s) Affected<br>")
            output.AppendLine()

            'update / insert STORED_PROCEDURE_GROUP on tbl_client_defaults for the new Client
            comm = New SqlCommand( _
                "IF (" & _
                "   select count(1) from tbl_client_defaults " & _
                "   where client_name = '" & txtCopyTo.Text & "' " & _
                "   and DEFAULT_NAME = 'STORED_PROCEDURE_GROUP'" & _
                ") = 0 begin " & _
                "   insert into tbl_client_defaults (CLIENT_NAME, DEFAULT_NAME, [VALUE]) " & _
                "   values ('" & txtCopyTo.Text & "', 'STORED_PROCEDURE_GROUP', '" & txtSPG.Text & "') " & _
                "end else begin " & _
                "   update tbl_client_defaults set [VALUE] = '" & txtSPG.Text & "'" & _
                "   where CLIENT_NAME = '" & txtSPG.Text & "' and DEFAULT_NAME = 'STORED_PROCEDURE_GROUP' " & _
                "end", conn, trans)
            result = comm.ExecuteNonQuery()
            output.Append("Set STORED_PROCEDURE_GROUP, " & result & " Row(s) Affected<br>")
            output.AppendLine()

            'update / insert STADIUM_CODE on tbl_client_defaults for the new Client
            comm = New SqlCommand( _
                "IF (" & _
                "   select count(1) from tbl_client_defaults " & _
                "   where client_name = '" & txtCopyTo.Text & "' " & _
                "   and DEFAULT_NAME = 'STADIUM_CODE'" & _
                ") = 0 begin " & _
                "   insert into tbl_client_defaults (CLIENT_NAME, DEFAULT_NAME, [VALUE]) " & _
                "   values ('" & txtCopyTo.Text & "', 'STADIUM_CODE', '" & txtStadiumCode.Text & "') " & _
                "end else begin " & _
                "   update tbl_client_defaults set [VALUE] = '" & txtStadiumCode.Text & "'" & _
                "   where CLIENT_NAME = '" & txtCopyTo.Text & "' and DEFAULT_NAME = 'STADIUM_CODE' " & _
                "end", conn, trans)
            result = comm.ExecuteNonQuery()
            output.Append("Set STADIUM_CODE, " & result & " Row(s) Affected<br>")
            output.AppendLine()

            trans.Commit()
        Catch ex As Exception
            If Not trans Is Nothing Then trans.Rollback()
            output = New StringBuilder()
            output.Append("Copy BU Failed<br><br>" & ex.Message & "<br>All Database changes have been Rolled Back.")
        Finally
            comm.Dispose()
            If (conn.State = ConnectionState.Open) Then
                conn.Close()
            End If
            conn.Dispose()
        End Try
        lblResult.text = output.ToString
    End Sub

End Class
