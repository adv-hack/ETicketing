Imports System.Data
Imports System.Data.SqlClient

Public Class TalentTableAdapter

    Private conSql2005 As SqlConnection = Nothing
    Private _table As String = String.Empty
    Private _fieldDefs As New Generic.Dictionary(Of String, String)
    Private _keys As New ArrayList
    Private _client As String = ""
    Private _logConnectionString As String = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
    Private _hasError As Boolean = False

    Public Property HasError() As Boolean
        Get
            Return _hasError
        End Get
        Set(ByVal value As Boolean)
            _hasError = value
        End Set
    End Property


    Public Property Client() As String
        Get
            Return _client
        End Get
        Set(ByVal value As String)
            _client = value
        End Set
    End Property

    Public Property FieldDefs() As Generic.Dictionary(Of String, String)
        Get
            Return _fieldDefs
        End Get
        Set(ByVal value As Generic.Dictionary(Of String, String))
            _fieldDefs = value
        End Set
    End Property

    Public Property Keys() As ArrayList
        Get
            Return _keys
        End Get
        Set(ByVal value As ArrayList)
            _keys = value
        End Set
    End Property

    Public Property Table() As String
        Get
            Return _table
        End Get
        Set(ByVal value As String)
            _table = value
        End Set
    End Property

    Public Sub ExecuteCommand(ByVal cmdText As String)
        Dim cmd As New SqlCommand
        Try
            'Execute the command
            cmd.CommandText = cmdText
            cmd.Connection = conSql2005
            cmd.ExecuteNonQuery()

            WriteToLogFile("ExecuteCommand", "ExecuteCommand", "", cmdText)
        Catch ex As Exception
            WriteToLogFile("ExecuteCommandError", ex.Message, "TTA0002", cmdText)
        Finally
            cmd.Dispose()
        End Try
    End Sub

    Public Function GetData(ByVal cmdText As String) As DataTable
        Dim cmd As New SqlCommand
        GetData = New DataTable
        Try
            'Execute the command
            cmd.CommandText = cmdText
            cmd.Connection = conSql2005

            Dim da As New SqlDataAdapter(cmd)

            da.Fill(GetData)

        Catch ex As Exception
        Finally
            cmd.Dispose()
        End Try

        Return GetData
    End Function

    Public Sub InsertRecord(ByVal info As Generic.Dictionary(Of String, String))

        Dim cmd As New SqlCommand
        If Not DuplicateRecord(info) Then


            Dim insertStr As String = String.Empty
            Dim logInfo As String = String.Empty
            Try

                'Populate the sql statement
                insertStr = " INSERT INTO " & _table & " ("
                Dim valueStr As String = " VALUES("

                ' Loop through the field definistions
                Dim key As String
                Dim count As Integer = 0
                For Each key In info.Keys
                    If count > 0 Then
                        insertStr += " ,"
                        valueStr += " ,"
                    End If
                    insertStr += key
                    valueStr += " @parm" & CStr(count)
                    count = count + 1
                Next
                insertStr += ") " & valueStr & ")"

                'Construct the command
                cmd.CommandText = insertStr
                cmd.Connection = conSql2005
                With cmd.Parameters
                    count = 0
                    For Each key In info.Keys
                        .AddWithValue("@parm" & CStr(count), FieldDefs(key)).Value = info.Item(key)
                        logInfo += key & "=" & info.Item(key) & ","
                        count = count + 1
                    Next
                End With

                'Execute the command
                cmd.ExecuteNonQuery()

                WriteToLogFile("InsertRecord", "InsertRecord", "", logInfo)

            Catch ex As Exception
                WriteToLogFile("InsertRecordError", ex.Message, "TTA0003", insertStr & ":-" & logInfo)
            Finally
                cmd.Dispose()
            End Try
        End If
    End Sub

    Public Sub UpdateRecord(ByVal keys As Generic.Dictionary(Of String, String), ByVal info As Generic.Dictionary(Of String, String))

        Dim cmd As New SqlCommand
        Dim updateStr As String = String.Empty
        Dim logInfo As String = String.Empty
        Try

            'Populate the sql statement
            updateStr = " UPDATE " & _table & _
                        " SET "

            ' Loop through the field definistions
            Dim count As Integer = 0
            For Each key As String In info.Keys
                If count > 0 Then
                    updateStr += " ,"
                End If
                updateStr += " " & key & " = @parm" & CStr(count)

                With cmd.Parameters
                    .AddWithValue("@parm" & CStr(count), FieldDefs(key)).Value = info.Item(key)
                    logInfo += key & "=" & info.Item(key) & ","
                End With

                count = count + 1
            Next

            updateStr += " WHERE "
            count = 0
            For Each key As String In keys.Keys
                If count > 0 Then
                    updateStr += " AND "
                End If
                updateStr += " " & key & " = @keyParm" & CStr(count)

                With cmd.Parameters
                    .AddWithValue("@keyParm" & CStr(count), FieldDefs(key)).Value = keys.Item(key)
                    logInfo += key & "=" & keys.Item(key) & ","
                End With

                count = count + 1
            Next

            'Construct the command
            cmd.CommandText = updateStr
            cmd.Connection = conSql2005


            'Execute the command
            cmd.ExecuteNonQuery()

            WriteToLogFile("UpdateRecord", "UpdateRecord", "", logInfo)

        Catch ex As Exception
            WriteToLogFile("UpdateRecordError", ex.Message, "TTA0010", updateStr & ":-" & logInfo)
        Finally
            cmd.Dispose()
        End Try
    End Sub

    Public Sub DeleteRecord(ByVal keys As Generic.Dictionary(Of String, String))

        Dim cmd As New SqlCommand
        Dim deleteStr As String = String.Empty
        Dim logInfo As String = String.Empty
        Try

            'Populate the sql statement
            deleteStr = " DELETE FROM " & _table & _
                        " WHERE "

            ' Loop through the field definistions
            Dim count As Integer = 0
            count = 0
            For Each key As String In keys.Keys
                If count > 0 Then
                    deleteStr += " AND "
                End If
                deleteStr += " " & key & " = @keyParm" & CStr(count)

                With cmd.Parameters
                    .AddWithValue("@keyParm" & CStr(count), FieldDefs(key)).Value = keys.Item(key)
                    logInfo += key & "=" & keys.Item(key) & ","
                End With

                count = count + 1
            Next

            'Only Construct the command and execute if Keys were found
            If count > 0 Then
                cmd.CommandText = deleteStr
                cmd.Connection = conSql2005

                'Execute the command
                cmd.ExecuteNonQuery()
                WriteToLogFile("DeleteRecord", "DeleteRecord", "", logInfo)
            End If

        Catch ex As Exception
            WriteToLogFile("DeleteRecordError", ex.Message, "TTA0011", deleteStr & ":-" & logInfo)
        Finally
            cmd.Dispose()
        End Try
    End Sub

    Private Function DuplicateRecord(ByVal info As Generic.Dictionary(Of String, String)) As Boolean
        DuplicateRecord = False
        Dim selectStr As String = String.Empty
        Dim dtr As SqlDataReader
        Dim cmd As New SqlCommand
        Dim logInfo As String = String.Empty
        Try

            'Only process if keys are specified
            If Keys.Count > 0 Then

                'Populate the sql statement
                selectStr = "SELECT * FROM " & _table & " WHERE "

                ' Loop through the field definistions
                Dim key As String
                Dim count As Integer = 0
                For Each key In Keys
                    If info.ContainsKey(key) Then
                        If count > 0 Then
                            selectStr += " AND "
                        End If
                        selectStr += key & " = @parm" & CStr(count)
                        count = count + 1
                    End If
                Next

                'Construct the command
                cmd.CommandText = selectStr
                cmd.Connection = conSql2005
                With cmd.Parameters
                    count = 0
                    For Each key In Keys
                        If info.ContainsKey(key) Then
                            .AddWithValue("@parm" & CStr(count), FieldDefs(key)).Value = info.Item(key)
                            logInfo += key & "=" & info.Item(key) & ","
                            count = count + 1
                        End If
                    Next
                End With

                'Check for the duplicate record
                dtr = cmd.ExecuteReader()
                If dtr.HasRows Then
                    DuplicateRecord = True
                    WriteToLogFile("DuplicateRecordFound", "DuplicateRecordFound", "", logInfo)
                End If
            End If

        Catch ex As Exception
            WriteToLogFile("DuplicateRecordError", ex.Message, "TTA0005", selectStr)
            DuplicateRecord = True
        Finally
            Try
                dtr.Close()
            Catch ex As Exception
            End Try
            cmd.Dispose()
        End Try
        Return DuplicateRecord
    End Function

    Public Sub SetColumnAttributes()
        Dim cmd As New SqlCommand
        Dim cmd2 As New SqlCommand
        Try
            Dim ds2 As New DataSet
            cmd2.Connection = conSql2005
            cmd2.CommandText = "select id,Name  from sysobjects where xType='U'"
            Dim da2 As New SqlDataAdapter(cmd2)
            da2.Fill(ds2)
            For Each dr2 As DataRow In ds2.Tables(0).Rows
                If UCase(dr2("Name")) = UCase(_table) Then
                    Dim ds As New DataSet
                    cmd.Connection = conSql2005
                    cmd.CommandText = "select name,xtype,length from syscolumns where id=" & dr2("ID")
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(ds)
                    For Each dr As DataRow In ds.Tables(0).Rows
                        _fieldDefs.Add(dr("Name"), dr("xtype"))
                    Next
                    Exit For
                End If
            Next
        Catch ex As Exception
            WriteToLogFile("SetColumnAttributesError", ex.Message, "TTA0004", _table)
        Finally
            cmd.Dispose()
            cmd2.Dispose()
        End Try

    End Sub

    Public Sub New(ByVal connectionString As String)
        Try
            If connectionString.Trim <> "" Then
                conSql2005 = New SqlConnection(connectionString)
                conSql2005.Open()
            End If
        Catch ex As Exception
            WriteToLogFile("NewError", ex.Message, "TTA0001", connectionString)
        End Try
    End Sub

    Public Sub CloseConnection()
        Try
            conSql2005.Close()
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub


    Public Sub WriteToLogFile(ByVal Action As String, _
                        ByVal Message As String, _
                        ByVal ErrNumber As String, _
                        ByVal Info As String)

        Dim con As SqlConnection = Nothing
        con = New SqlConnection(_logConnectionString)
        Dim insertStr As String = String.Empty
        Dim cmd As New SqlCommand

        If ErrNumber.Trim <> "" Then
            _hasError = True
        End If

        Try

            con.Open()

            'Populate the sql statement
            insertStr = " INSERT INTO tbl_logs (LOG_SOURCE_CLASS, LOG_SOURCE_METHOD, LOG_CODE, LOG_FILTER_1, LOG_FILTER_2, " & _
                                        "LOG_FILTER_3, LOG_FILTER_4, LOG_FILTER_5, LOG_CONTENT, LOG_TIMESTAMP) " & _
                                         " VALUES(@parm0, @parm1, @parm2, @parm3, @parm4, @parm5, @parm6, @parm7, @parm8, @parm9)"

            'Construct the command
            Dim d As System.DateTime = Now
            cmd.CommandText = insertStr
            cmd.Connection = con
            With cmd.Parameters
                .Add("@parm0", Data.SqlDbType.NVarChar).Value = "TalentTableAdapter.vb"
                .Add("@parm1", Data.SqlDbType.NVarChar).Value = Action
                If ErrNumber.Trim = "" Then
                    .Add("@parm2", Data.SqlDbType.NVarChar).Value = "LogEntry"
                Else
                    .Add("@parm2", Data.SqlDbType.NVarChar).Value = ErrNumber
                End If
                .Add("@parm3", Data.SqlDbType.NVarChar).Value = _client
                .Add("@parm4", Data.SqlDbType.NVarChar).Value = _table
                .Add("@parm5", Data.SqlDbType.NVarChar).Value = d.ToShortDateString
                .Add("@parm6", Data.SqlDbType.NVarChar).Value = ""
                .Add("@parm7", Data.SqlDbType.NVarChar).Value = Info
                .Add("@parm8", Data.SqlDbType.NVarChar).Value = Message
                .Add("@parm9", Data.SqlDbType.DateTime).Value = d
            End With

            'Execute the command
            cmd.ExecuteNonQuery()

        Catch ex As Exception
        Finally
            Try
                cmd.Dispose()
                con.Close()
            Catch ex As Exception
            End Try
        End Try
    End Sub
End Class
