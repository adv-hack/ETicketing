Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Linq
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common

Public Class UploadAndPublishHelper

#Region "Public Properties"
    Private Property Exceptions() As List(Of Dictionary(Of String, String))
    Private Property TargetColumns() As List(Of String)
    Private Property KeyColumns() As List(Of String)
    Private Property TargetConnectionString() As String
    Private Property SourceConnectionString() As String
    Private Property Table() As String
#End Region

#Region "Private constants"
    Private Const VOUCHERSEXTERNAL As String = "tbl_vouchers_external"
    Private Const EXPORTNOTPOSSIBLE As String = "The destination is the same as source."
#End Region

#Region "Private Shared Methods"

    Private Function GetKeyColumnsList(ByVal TableName As String) As List(Of String)
        Dim KeyColumnsList As New List(Of String)
        Select Case TableName
            Case VOUCHERSEXTERNAL
                KeyColumnsList.Add("BUSINESS_UNIT")
                KeyColumnsList.Add("PARTNER")
                KeyColumnsList.Add("VOUCHER_ID")
                KeyColumnsList.Add("COMPANY")
                KeyColumnsList.Add("PRICE")
        End Select

        Return KeyColumnsList
    End Function

    Private Function GetTargetColumnsList(ByVal TableName As String) As List(Of String)
        Dim TargetColumnsList As New List(Of String)
        Select Case TableName
            Case VOUCHERSEXTERNAL
                TargetColumnsList.Add("BUSINESS_UNIT")
                TargetColumnsList.Add("PARTNER")
                TargetColumnsList.Add("VOUCHER_ID")
                TargetColumnsList.Add("COMPANY")
                TargetColumnsList.Add("PRICE")
        End Select

        Return TargetColumnsList
    End Function

    Private Function GetExceptionsList(ByVal TableName As String) As List(Of Dictionary(Of String, String))
        Dim ExceptionsList As New List(Of Dictionary(Of String, String))
        Select Case TableName
            Case VOUCHERSEXTERNAL

        End Select

        Return ExceptionsList
    End Function

#End Region

#Region "Public Methods"

    Public Sub New(ByVal tbl_Name As String, ByVal t_ConnectionString As String, ByVal s_ConnectionString As String)
        Table = tbl_Name
        KeyColumns = GetKeyColumnsList(Table)
        TargetColumns = GetTargetColumnsList(Table)
        Exceptions = GetExceptionsList(Table)
        'A check to ensure that no attempt is made to export data to the same table as currently data is exported to work tables which are in the same database
        'and this allows the source and destination connection strings to be the same.
        If (t_ConnectionString = s_ConnectionString) Then
            t_ConnectionString = String.Empty
            Throw New Exception(EXPORTNOTPOSSIBLE)
        Else
            TargetConnectionString = t_ConnectionString
            SourceConnectionString = s_ConnectionString
        End If
    End Sub

    Public Function UploadAndPublishTableData() As ErrorObj
        Dim err As New ErrorObj()
        Dim SourceSqlConn As SqlConnection = New SqlConnection(SourceConnectionString)
        Dim TargetSqlConn As SqlConnection = New SqlConnection(TargetConnectionString)
        Dim cmd As SqlCommand
        Dim da As SqlDataAdapter
        Dim SourceDt As New DataTable
        Dim TargetDt As New DataTable
        Dim Msg As StringBuilder
        Dim SQL As StringBuilder
        Dim SelectSQL As String = String.Empty
        Dim identityColumn As String = String.Empty

        Dim Somethingchanged As Boolean = False

        Try
            '
            ' Lets open both databases
            '
            SourceSqlConn.Open()
            TargetSqlConn.Open()
            '
            ' Build the SQL statement to retrieve the potential rows to transfer
            '
            SQL = New StringBuilder("SELECT * FROM " & Table & " ")
            If Exceptions.Count > 0 Then
                SQL.Append("WHERE (")
            End If

            For Each Constraint As Dictionary(Of String, String) In Exceptions

                For i As Integer = 0 To Constraint.Count - 1
                    SQL.Append(Constraint.Keys(i) & " <> '" & Constraint.Values(i) & "' OR ")
                Next
                SQL.Remove(SQL.ToString.LastIndexOf(" OR "), 4)
                SQL.Append(") AND ( ")

            Next
            If Exceptions.Count > 0 Then
                SQL.Remove(SQL.ToString.LastIndexOf(" AND ( "), 6)
            End If

            SelectSQL = SQL.ToString()  ' Save this for later as we'll need it again when we come to do the delete

            cmd = New SqlCommand(SQL.ToString, SourceSqlConn)
            da = New SqlDataAdapter(cmd)
            da.Fill(SourceDt)
            '
            ' Retrieve the idenitity columns as these cannot be used on the insert later
            '
            If SourceDt.Rows.Count > 0 Then
                Dim SelectIdentity As String = "SELECT name FROM syscolumns" & _
                    " WHERE (status & 128) = 128 and OBJECT_NAME(id) = '" & Table & "'"
                cmd = New SqlCommand(SelectIdentity, SourceSqlConn)
                da = New SqlDataAdapter(cmd)
                Dim dtIdentity As New DataTable
                da.Fill(dtIdentity)
                If dtIdentity.Rows.Count > 0 Then
                    identityColumn = dtIdentity.Rows(0).Item("name")
                End If
            End If
            '
            ' We now have the rows that require copying
            '
            For Each dr As DataRow In SourceDt.Rows
                '
                ' Does this row exist in the target table
                '
                SQL = New StringBuilder("SELECT * FROM " & Table & " ")
                If KeyColumns.Count > 0 Then
                    SQL.Append("WHERE ")
                End If
                For Each Col As String In KeyColumns
                    SQL.Append("[" & Col & "] = ")
                    SQL.Append(Utilities.SQLDataValue(dr(Col), SourceDt.Columns(Col).DataType))
                    SQL.Append(" AND ")
                Next
                If KeyColumns.Count > 0 Then
                    SQL.Remove(SQL.ToString.LastIndexOf(" AND "), 5)
                End If

                cmd = New SqlCommand(SQL.ToString, TargetSqlConn)
                da = New SqlDataAdapter(cmd)
                TargetDt = New DataTable
                da.Fill(TargetDt)

                If TargetDt.Rows.Count > 0 Then
                    '
                    ' Update this row
                    '
                    Somethingchanged = False

                    Msg = New StringBuilder("Updated the following columns in table '" & Table & "' : ")
                    SQL = New StringBuilder("UPDATE " & Table & " SET ")
                    For Each Col As String In TargetColumns
                        If TargetDt.Rows(0).Item(Col).ToString() <> dr.Item(Col).ToString() Then
                            SQL.Append("[" & Col & "] = ")
                            SQL.Append(Utilities.SQLDataValue(dr.Item(Col), SourceDt.Columns(Col).DataType))
                            SQL.Append(", ")
                            Msg.Append(Col & "(Was : " & Utilities.SQLDataValue(TargetDt.Rows(0).Item(Col), TargetDt.Columns(Col).DataType))
                            Msg.Append(", Now: " & Utilities.SQLDataValue(dr.Item(Col), SourceDt.Columns(Col).DataType) & "), ")
                            Somethingchanged = True
                        End If
                    Next
                    If Somethingchanged Then
                        SQL.Remove(SQL.ToString.LastIndexOf(", "), 2)
                        Msg.Remove(Msg.ToString.LastIndexOf(", "), 2)

                        SQL.Append(" WHERE ")
                        For Each Col As String In KeyColumns
                            SQL.Append("[" & Col & "] = ")
                            SQL.Append(Utilities.SQLDataValue(dr(Col), SourceDt.Columns(Col).DataType))
                            SQL.Append(" AND ")
                        Next
                        SQL.Remove(SQL.ToString.LastIndexOf(" AND "), 5)
                    End If
                Else
                    '
                    ' Insert this row
                    '
                    Msg = New StringBuilder("Inserted the following record in to table '" & Table & "' : Columns (")
                    SQL = New StringBuilder("INSERT INTO " & Table & " (")
                    For Each Col As DataColumn In SourceDt.Columns
                        If Not Col.ColumnName.Equals(identityColumn) Then
                            SQL.Append("[" & Col.ColumnName & "], ")
                        End If
                    Next
                    SQL.Remove(SQL.ToString.LastIndexOf(", "), 2)

                    SQL.Append(") VALUES (")

                    For Each Col As DataColumn In SourceDt.Columns
                        If Not Col.ColumnName.Equals(identityColumn) Then
                            SQL.Append(Utilities.SQLDataValue(dr.Item(Col.ColumnName), Col.DataType))
                            SQL.Append(", ")
                        End If
                    Next
                    SQL.Remove(SQL.ToString.LastIndexOf(", "), 2)
                    SQL.Append(")")

                    Msg = New StringBuilder("Inserted the following row into table '" & Table & "' : " & SQL.ToString())
                    Somethingchanged = True
                End If

                If Somethingchanged Then
                    Dim sLog As String = ""
                    Dim iInsUpd As Integer = -1
                    cmd = New SqlCommand(SQL.ToString, TargetSqlConn)
                    iInsUpd = cmd.ExecuteNonQuery()
                    If iInsUpd > 0 Then
                        sLog = ("[Success]" & Msg.ToString)
                    Else
                        sLog = ("[FAILED]" & Msg.ToString)
                    End If
                    '
                    ' Log what we have done
                    '
                    Try
                        'Base.Report.WriteDBLog(Constants.ServerMonitors.DB_MONITOR, "DBManager.DuplicateTableData()", "Information", "", "", "", "", "", sLog, Now())
                    Catch ex As Exception
                    End Try
                End If
            Next
            '
            ' Now that we have done the updates and inserts we need 
            ' to delete any rows in the target that are not in the source
            '
            cmd = New SqlCommand(SelectSQL, TargetSqlConn)
            da = New SqlDataAdapter(cmd)
            TargetDt = New DataTable
            da.Fill(TargetDt)

            For Each drTarget As DataRow In TargetDt.Rows
                '
                ' Using our Key columns, see if this row exists in the source table
                '
                SQL = New StringBuilder("SELECT * FROM " & Table & " ")
                If KeyColumns.Count > 0 Then
                    SQL.Append("WHERE ")
                End If
                For Each Col As String In KeyColumns
                    SQL.Append("[" & Col & "] = ")
                    SQL.Append(Utilities.SQLDataValue(drTarget(Col), TargetDt.Columns(Col).DataType))
                    SQL.Append(" AND ")
                Next
                If KeyColumns.Count > 0 Then
                    SQL.Remove(SQL.ToString.LastIndexOf(" AND "), 5)
                End If

                cmd = New SqlCommand(SQL.ToString, SourceSqlConn)
                da = New SqlDataAdapter(cmd)
                SourceDt = New DataTable
                da.Fill(SourceDt)
                '
                ' If it doesn't exist in the source table (i.e. row count = 0) then delete 
                ' it from the taget table
                '
                If SourceDt.Rows.Count = 0 Then
                    SQL.Replace("SELECT * FROM", "DELETE FROM")
                    Dim sLogDel As String = ""
                    Dim iDel As Integer = -1
                    cmd = New SqlCommand(SQL.ToString, TargetSqlConn)
                    iDel = cmd.ExecuteNonQuery()
                    '
                    ' Log what we have done
                    '
                    If iDel > 0 Then
                        sLogDel = ("Deleted the following row in table '" & Table & "' : " & SQL.ToString)
                    Else
                        sLogDel = ("FAILED: Deleting the following row in table '" & Table & "' : " & SQL.ToString)
                    End If
                    Try
                        'Base.Report.WriteDBLog(Constants.ServerMonitors.DB_MONITOR, "DBManager.DuplicateTableData()", "Information", "", "", "", "", "", sLogDel, Now())
                    Catch ex As Exception
                    End Try
                End If

            Next

        Catch ex As Exception
            err.HasError = True
            err.ErrorMessage = ex.Message
            If Not ex.InnerException Is Nothing Then
                err.ErrorMessage = err.ErrorMessage & " " & ex.InnerException.Message
                If Not ex.InnerException.InnerException Is Nothing Then
                    err.ErrorMessage = err.ErrorMessage & " " & ex.InnerException.InnerException.Message
                End If
            End If
            err.ErrorNumber = "MON-DBMgr-03"

        Finally
            SourceSqlConn.Close()
            TargetSqlConn.Close()
        End Try

        Return err

    End Function

#End Region
   
End Class
