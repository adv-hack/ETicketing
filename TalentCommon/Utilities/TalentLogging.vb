Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text
Imports System.Data.SqlClient
Imports System.Data
Imports System.Web

<Serializable()> _
Public Class TalentLogging

#Region "Class Level Fields"

    Private _cacheActive As Boolean = False
    Private _frontEndConnectionString As String = String.Empty
    Private Const LOG_HEADER_ID As Integer = -1
    Private Const LOGGING_MODE_DB As String = "DB"
    Private Const LOGGING_MODE_FILE As String = "FILE"
    Private Const LOGGING_MODE_DBFAILEDTHENFILE As String = "DB_FAILED_THEN_FILE"

#End Region

#Region "Public Properties"

    Public Property FrontEndConnectionString() As String
        Get
            Return _frontEndConnectionString
        End Get
        Set(ByVal value As String)
            _frontEndConnectionString = value
        End Set
    End Property

#End Region

#Region "Public Methods"

    Public Sub New()
        'Check that we have access to the HttpContext.Current Object
        'in order to use caching
        Try
            If Not System.Web.HttpContext.Current Is Nothing Then _cacheActive = True
        Catch ex As Exception
        End Try
    End Sub

    Public Sub Logging(ByVal classORPageName As String, ByVal methodOrModuleName As String, ByVal loggingContent As String, ByVal errorORLogCode As String, Optional ByVal logType As String = "GeneralLog", Optional ByVal businessUnit As String = "", Optional ByVal partner As String = "", Optional ByVal loginid As String = "", Optional ByVal filter4 As String = "", Optional ByVal filter5 As String = "")
        Try
            Dim logDefaultsRow As DataRow = Nothing
            If TryGetLogDefRowByLogType(logType, logDefaultsRow) Then
                Dim isDBLoggingSuccess As Boolean = False
                Dim loggingMode As String = CType(logDefaultsRow.Item("LOGGING_MODE"), String)
                If loggingMode = LOGGING_MODE_DB OrElse loggingMode = LOGGING_MODE_DBFAILEDTHENFILE Then
                    isDBLoggingSuccess = IsLoggedInDBSuccess(classORPageName,
                                                           methodOrModuleName,
                                                           errorORLogCode,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           filter4,
                                                           filter5)
                End If
                If loggingMode = LOGGING_MODE_FILE OrElse (Not isDBLoggingSuccess) Then
                    CreateLogEntry(ConstructFileName(logDefaultsRow),
                                   ConstructLoggingContent(CType(logDefaultsRow.Item("USE_QUOTES"), Boolean),
                                                           classORPageName,
                                                           methodOrModuleName,
                                                           errorORLogCode,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           filter4,
                                                           filter5))
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Sub Logging(ByVal classORPageName As String, ByVal methodOrModuleName As String, ByVal loggingContent As String, ByVal errObj As ErrorObj, Optional ByVal logType As String = "GeneralLog", Optional ByVal businessUnit As String = "", Optional ByVal partner As String = "", Optional ByVal loginid As String = "", Optional ByVal filter4 As String = "", Optional ByVal filter5 As String = "")
        Try
            Dim logDefaultsRow As DataRow = Nothing
            If TryGetLogDefRowByLogType(logType, logDefaultsRow) Then
                Dim isDBLoggingSuccess As Boolean = False
                Dim loggingMode As String = CType(logDefaultsRow.Item("LOGGING_MODE"), String)
                loggingContent = loggingContent & ";" & errObj.ErrorStatus & ";" & errObj.ErrorMessage & ";" & errObj.HasError.ToString
                If loggingMode = LOGGING_MODE_DB OrElse loggingMode = LOGGING_MODE_DBFAILEDTHENFILE Then
                    isDBLoggingSuccess = IsLoggedInDBSuccess(classORPageName,
                                                           methodOrModuleName,
                                                           errObj.ErrorNumber,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           filter4,
                                                           "")
                End If
                If loggingMode = LOGGING_MODE_FILE OrElse (Not isDBLoggingSuccess) Then
                    CreateLogEntry(ConstructFileName(logDefaultsRow),
                                   ConstructLoggingContent(CType(logDefaultsRow.Item("USE_QUOTES"), Boolean),
                                                           classORPageName,
                                                           methodOrModuleName,
                                                           errObj.ErrorNumber,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           filter4,
                                                           ""))
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Sub Logging(ByVal classORPageName As String, ByVal methodOrModuleName As String, ByVal loggingContent As String, ByVal errObj As ErrorObj, ByVal sqlExceptionObject As SqlException, Optional ByVal logType As String = "GeneralLog", Optional ByVal businessUnit As String = "", Optional ByVal partner As String = "", Optional ByVal loginid As String = "", Optional ByVal filter4 As String = "")
        Try
            Dim logDefaultsRow As DataRow = Nothing
            If TryGetLogDefRowByLogType(logType, logDefaultsRow) Then
                Dim isDBLoggingSuccess As Boolean = False
                Dim loggingMode As String = CType(logDefaultsRow.Item("LOGGING_MODE"), String)
                loggingContent = loggingContent & ";" & sqlExceptionObject.ErrorCode & ";" & sqlExceptionObject.LineNumber & ";" & sqlExceptionObject.Number & ";" & sqlExceptionObject.Procedure & ";" & sqlExceptionObject.Server & ";" & sqlExceptionObject.Source & ";" & sqlExceptionObject.TargetSite.Name
                If loggingMode = LOGGING_MODE_DB OrElse loggingMode = LOGGING_MODE_DBFAILEDTHENFILE Then
                    isDBLoggingSuccess = IsLoggedInDBSuccess(classORPageName,
                                                           methodOrModuleName,
                                                           errObj.ErrorNumber,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           sqlExceptionObject.Message,
                                                           sqlExceptionObject.StackTrace)
                End If
                If loggingMode = LOGGING_MODE_FILE OrElse (Not isDBLoggingSuccess) Then
                    CreateLogEntry(ConstructFileName(logDefaultsRow),
                                   ConstructLoggingContent(CType(logDefaultsRow.Item("USE_QUOTES"), Boolean),
                                                           classORPageName,
                                                           methodOrModuleName,
                                                           errObj.ErrorNumber,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           sqlExceptionObject.Message,
                                                           sqlExceptionObject.StackTrace))
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Sub Logging(ByVal classORPageName As String, ByVal methodOrModuleName As String, ByVal loggingContent As String, ByVal errorCode As String, ByVal sqlExceptionObject As SqlException, Optional ByVal logType As String = "GeneralLog", Optional ByVal businessUnit As String = "", Optional ByVal partner As String = "", Optional ByVal loginid As String = "", Optional ByVal filter4 As String = "")
        Try
            Dim logDefaultsRow As DataRow = Nothing
            If TryGetLogDefRowByLogType(logType, logDefaultsRow) Then
                Dim isDBLoggingSuccess As Boolean = False
                Dim loggingMode As String = CType(logDefaultsRow.Item("LOGGING_MODE"), String)
                loggingContent = loggingContent & ";" & sqlExceptionObject.ErrorCode & ";" & sqlExceptionObject.LineNumber & ";" & sqlExceptionObject.Number & ";" & sqlExceptionObject.Procedure & ";" & sqlExceptionObject.Server & ";" & sqlExceptionObject.Source & ";" & sqlExceptionObject.TargetSite.Name
                If loggingMode = LOGGING_MODE_DB OrElse loggingMode = LOGGING_MODE_DBFAILEDTHENFILE Then
                    isDBLoggingSuccess = IsLoggedInDBSuccess(classORPageName,
                                                           methodOrModuleName,
                                                           errorCode,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           sqlExceptionObject.Message,
                                                           sqlExceptionObject.StackTrace)
                End If
                If loggingMode = LOGGING_MODE_FILE OrElse (Not isDBLoggingSuccess) Then
                    CreateLogEntry(ConstructFileName(logDefaultsRow),
                                   ConstructLoggingContent(CType(logDefaultsRow.Item("USE_QUOTES"), Boolean),
                                                           classORPageName,
                                                           methodOrModuleName,
                                                           errorCode,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           sqlExceptionObject.Message,
                                                           sqlExceptionObject.StackTrace))
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Public Sub Logging(ByVal classORPageName As String, ByVal methodOrModuleName As String, ByVal loggingContent As String, ByVal errObj As ErrorObj, ByVal exceptionObject As Exception, Optional ByVal logType As String = "GeneralLog", Optional ByVal businessUnit As String = "", Optional ByVal partner As String = "", Optional ByVal loginid As String = "", Optional ByVal filter4 As String = "")
        Try
            Dim logDefaultsRow As DataRow = Nothing
            If TryGetLogDefRowByLogType(logType, logDefaultsRow) Then
                Dim isDBLoggingSuccess As Boolean = False
                Dim loggingMode As String = CType(logDefaultsRow.Item("LOGGING_MODE"), String)
                loggingContent = loggingContent & ";" & exceptionObject.TargetSite.Name & ";" & exceptionObject.Source
                If loggingMode = LOGGING_MODE_DB OrElse loggingMode = LOGGING_MODE_DBFAILEDTHENFILE Then
                    isDBLoggingSuccess = IsLoggedInDBSuccess(classORPageName,
                                                           methodOrModuleName,
                                                           errObj.ErrorNumber,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           exceptionObject.Message,
                                                           exceptionObject.StackTrace)
                End If
                If loggingMode = LOGGING_MODE_FILE OrElse (Not isDBLoggingSuccess) Then
                    CreateLogEntry(ConstructFileName(logDefaultsRow),
                                   ConstructLoggingContent(CType(logDefaultsRow.Item("USE_QUOTES"), Boolean),
                                                           classORPageName,
                                                           methodOrModuleName,
                                                           errObj.ErrorNumber,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           exceptionObject.Message,
                                                           exceptionObject.StackTrace))
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Public Sub Logging(ByVal classORPageName As String, ByVal methodOrModuleName As String, ByVal loggingContent As String, ByVal errorCode As String, ByVal exceptionObject As Exception, Optional ByVal logType As String = "GeneralLog", Optional ByVal businessUnit As String = "", Optional ByVal partner As String = "", Optional ByVal loginid As String = "", Optional ByVal filter4 As String = "")
        Try
            Dim logDefaultsRow As DataRow = Nothing
            If TryGetLogDefRowByLogType(logType, logDefaultsRow) Then
                Dim isDBLoggingSuccess As Boolean = False
                Dim loggingMode As String = CType(logDefaultsRow.Item("LOGGING_MODE"), String)
                loggingContent = loggingContent & ";" & exceptionObject.TargetSite.Name & ";" & exceptionObject.Source
                If loggingMode = LOGGING_MODE_DB OrElse loggingMode = LOGGING_MODE_DBFAILEDTHENFILE Then
                    isDBLoggingSuccess = IsLoggedInDBSuccess(classORPageName,
                                                           methodOrModuleName,
                                                           errorCode,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           exceptionObject.Message,
                                                           exceptionObject.StackTrace)
                End If
                If loggingMode = LOGGING_MODE_FILE OrElse (Not isDBLoggingSuccess) Then
                    CreateLogEntry(ConstructFileName(logDefaultsRow),
                                   ConstructLoggingContent(CType(logDefaultsRow.Item("USE_QUOTES"), Boolean),
                                                           classORPageName,
                                                           methodOrModuleName,
                                                           errorCode,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           exceptionObject.Message,
                                                           exceptionObject.StackTrace))
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Public Sub LoadTestLog(ByVal page As String, ByVal section As String, ByVal ts As TimeSpan, Optional ByVal logType As String = "LoadTestLog")

        'Retrieve the defaults
        Dim dt As DataTable = RetrieveLogDefaults(logType)

        'Is this type of logging switched on
        If dt.Rows.Count > 0 Then

            'Has this function exceeded the threshold
            Dim timeTaken As TimeSpan = Now.TimeOfDay - ts
            If timeTaken.TotalSeconds > 0.03 Then

                'Extract the time taken field
                Dim value As String = timeTaken.ToString.Substring(6)
                If value.Length > 6 Then value = value.Substring(0, 6)

                'Write the log record
                CreateLogEntry(ConstructFileName(dt.Rows(0)), _
                                ConstructLogValue(CType(dt.Rows(0).Item("USE_QUOTES"), Boolean), page, section, value, "", ""))
            End If

        End If

    End Sub


    Public Sub Logging(ByVal classORPageName As String, ByVal methodOrModuleName As String, ByVal errObj As ErrorObj, ByVal exceptionObject As Exception, Optional ByVal logType As String = "GeneralLog", Optional ByVal businessUnit As String = "", Optional ByVal partner As String = "", Optional ByVal loginid As String = "", Optional ByVal filter4 As String = "", Optional ByVal filter5 As String = "")
        Try
            Dim logDefaultsRow As DataRow = Nothing
            If TryGetLogDefRowByLogType(logType, logDefaultsRow) Then
                Dim isDBLoggingSuccess As Boolean = False
                Dim loggingMode As String = CType(logDefaultsRow.Item("LOGGING_MODE"), String)
                Dim loggingContent As String = exceptionObject.TargetSite.Name & ";" & exceptionObject.Source & ";" & exceptionObject.Message & ";" & exceptionObject.StackTrace
                If loggingMode = LOGGING_MODE_DB OrElse loggingMode = LOGGING_MODE_DBFAILEDTHENFILE Then
                    isDBLoggingSuccess = IsLoggedInDBSuccess(classORPageName,
                                                           methodOrModuleName,
                                                           errObj.ErrorNumber,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           filter4,
                                                           filter5)
                End If
                If loggingMode = LOGGING_MODE_FILE OrElse (Not isDBLoggingSuccess) Then
                    CreateLogEntry(ConstructFileName(logDefaultsRow),
                                   ConstructLoggingContent(CType(logDefaultsRow.Item("USE_QUOTES"), Boolean),
                                                           classORPageName,
                                                           methodOrModuleName,
                                                           errObj.ErrorNumber,
                                                           loggingContent,
                                                           businessUnit,
                                                           partner,
                                                           loginid,
                                                           filter4,
                                                           filter5))
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub
#End Region

#Region "Private Methods"

    Private Function TryGetLogDefRowByLogType(ByVal logType As String, ByRef row As DataRow) As Boolean
        Dim isLogTypeActive As Boolean = False
        Dim dt As DataTable = RetrieveLogDefaults(logType)
        If dt.Rows.Count > 0 Then
            If Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0)("IS_ACTIVE")) Then
                isLogTypeActive = True
                row = dt.Rows(0)
            End If
        End If
        Return isLogTypeActive
    End Function

    Private Function RetrieveLogDefaults(ByVal logType As String) As DataTable

        Dim dt As New DataTable
        Dim cacheKey As String = "RetrieveLogSettings" & logType

        'First check cache for the log defaults table
        If _cacheActive AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            dt = HttpContext.Current.Cache.Item(cacheKey)
        Else

            Dim cmd As New SqlCommand("", New SqlConnection(FrontEndConnectionString))
            Dim SelectStr As String = " SELECT *  " & _
                                        " FROM tbl_log_defaults WITH (NOLOCK) " & _
                                        " WHERE log_type = @log_type AND IS_ACTIVE = 1"

            cmd.CommandText = SelectStr

            Try
                cmd.Connection.Open()

                With cmd.Parameters
                    .Add("@log_type", SqlDbType.NVarChar).Value = logType
                End With

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)
                da.Dispose()
            Catch ex As Exception
            Finally
                If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            End Try

            'Cache the results
            If _cacheActive Then
                HttpContext.Current.Cache.Insert(cacheKey, dt, Nothing, System.DateTime.Now.AddMinutes(60), Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If

        End If

        Return dt
    End Function

    Private Function ConstructFileName(ByVal row As DataRow) As String
        If row.Item("LOG_PATH") IsNot Nothing AndAlso row.Item("FILE_ABBREVIATION") IsNot Nothing Then
            Return row.Item("LOG_PATH") & row.Item("FILE_ABBREVIATION") & Now.Year & "-" & Now.Month & "-" & Now.Day & ".log"
        Else
            Return String.Empty
        End If
    End Function

    Private Function ConstructLoggingContent(ByVal useQuotes As Boolean,
                                      ByVal logSourceClass As String,
                                      ByVal logSourceMethod As String,
                                      ByVal logCode As String,
                                      ByVal loggingContent As String,
                                      ByVal logFilter1 As String,
                                      ByVal logFilter2 As String,
                                      ByVal logFilter3 As String,
                                      ByVal logfilter4 As String,
                                      ByVal logfilter5 As String) As String
        Dim sbLoggingContent As New StringBuilder
        sbLoggingContent.Append(AppendValue(useQuotes, Now.ToString("G"), False))
        sbLoggingContent.Append(AppendValue(useQuotes, logSourceClass, False))
        sbLoggingContent.Append(AppendValue(useQuotes, logSourceMethod, False))
        sbLoggingContent.Append(AppendValue(useQuotes, logCode, False))
        sbLoggingContent.Append(AppendValue(useQuotes, loggingContent, False))
        sbLoggingContent.Append(AppendValue(useQuotes, logFilter1, False))
        sbLoggingContent.Append(AppendValue(useQuotes, logFilter2, False))
        sbLoggingContent.Append(AppendValue(useQuotes, logFilter3, False))
        sbLoggingContent.Append(AppendValue(useQuotes, logfilter4, False))
        sbLoggingContent.Append(AppendValue(useQuotes, logfilter5, False))
        Return sbLoggingContent.ToString
    End Function

    Private Function AppendValue(ByVal useQuotes As Boolean, ByVal value As String, ByVal lastValue As Boolean) As String
        Dim val As String
        Dim quote As String = """"
        If useQuotes Then
            val = quote & value.Trim & quote
        Else
            val = value.Trim
        End If
        If Not lastValue Then val += ","
        Return val
    End Function

    Private Function IsLoggedInDBSuccess(ByVal logSourceClass As String,
                                       ByVal logSourceMethod As String,
                                       ByVal logCode As String,
                                       ByVal loggingContent As String,
                                       ByVal logFilter1 As String,
                                       ByVal logFilter2 As String,
                                       ByVal logFilter3 As String,
                                       ByVal logfilter4 As String,
                                       ByVal logfilter5 As String) As Boolean
        Dim isLogged As Boolean = False
        Dim rowsAffected As Integer = -2
        Dim sqlCommnd As SqlCommand = Nothing
        Try
            Dim InsertStatement As String = " INSERT INTO tbl_logs  " & _
                                        "([LOG_SOURCE_CLASS], [LOG_SOURCE_METHOD], [LOG_CODE], [LOG_FILTER_1]" & _
                                        ", [LOG_FILTER_2], [LOG_FILTER_3], [LOG_FILTER_4], [LOG_FILTER_5]" & _
                                        ",[LOG_CONTENT], [LOG_HEADER_ID]) VALUES " & _
                                        " (@LogSourceClass, @LogSourceMethod, @LogCode, @LogFilter1, @LogFilter2," & _
                                        " @LogFilter3, @LogFilter4, @LogFilter5, @LogContent, @LogHeaderID) "

            sqlCommnd = New SqlCommand(InsertStatement, New SqlConnection(FrontEndConnectionString))
            sqlCommnd.CommandType = CommandType.Text

            With sqlCommnd.Parameters
                .Add("@LogSourceClass", SqlDbType.NVarChar).Value = logSourceClass
                .Add("@LogSourceMethod", SqlDbType.NVarChar).Value = logSourceMethod
                .Add("@LogCode", SqlDbType.NVarChar).Value = logCode
                .Add("@LogFilter1", SqlDbType.NVarChar).Value = logFilter1
                .Add("@LogFilter2", SqlDbType.NVarChar).Value = logFilter2
                .Add("@LogFilter3", SqlDbType.NVarChar).Value = logFilter3
                .Add("@LogFilter4", SqlDbType.NVarChar).Value = logfilter4
                .Add("@LogFilter5", SqlDbType.NVarChar).Value = logfilter5
                .Add("@LogContent", SqlDbType.NVarChar).Value = loggingContent
                .Add("@LogHeaderID", SqlDbType.BigInt).Value = LOG_HEADER_ID
            End With

            sqlCommnd.Connection.Open()
            rowsAffected = sqlCommnd.ExecuteNonQuery
            If rowsAffected > 0 Then
                isLogged = True
            End If
        Catch ex As Exception
        Finally
            If sqlCommnd.Connection.State = ConnectionState.Open Then sqlCommnd.Connection.Close()
        End Try

        Return isLogged
    End Function

    Private Function ConstructLogValue(ByVal useQuotes As Boolean, _
                                            ByVal value1 As String, _
                                            ByVal value2 As String, _
                                            ByVal value3 As String, _
                                            ByVal value4 As String, _
                                            ByVal value5 As String) As String

        Dim value As New StringBuilder
        value.Append(AppendValue(useQuotes, Now.ToString("G"), False))
        value.Append(AppendValue(useQuotes, value1, False))
        value.Append(AppendValue(useQuotes, value2, False))
        value.Append(AppendValue(useQuotes, value3, False))
        value.Append(AppendValue(useQuotes, value4, False))
        value.Append(AppendValue(useQuotes, value5, True))
        Return value.ToString

    End Function

    Private Sub CreateLogEntry(ByVal logFile As String, Optional ByVal value As String = "")
        Try
            If logFile.Length > 0 Then
                'Are we creating the log file
                Dim append As Boolean = False
                If File.Exists(logFile) Then append = True

                'Write the record to the log file
                Dim objWriter As New StreamWriter(logFile, append)
                objWriter.WriteLine(value.ToString)
                objWriter.Flush()
                objWriter.Close()
            End If
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region "Deprecated Methods"

    Public Sub GeneralLog(ByVal sModule As String, ByVal code As String, ByVal value As String, Optional ByVal logType As String = "GeneralLog")

        'Retrieve the defaults
        Dim dt As DataTable = RetrieveLogDefaults(logType)

        'Is this type of logging switched on
        If dt.Rows.Count > 0 Then

            'Write the log record
            CreateLogEntry(ConstructFileName(dt.Rows(0)), _
                            ConstructLogValue(CType(dt.Rows(0).Item("USE_QUOTES"), Boolean), sModule, code, value, "", ""))

        End If

    End Sub

    Public Sub ErrorObjectLog(ByVal sModule As String, ByVal errObj As ErrorObj, Optional ByVal logType As String = "ErrorObjectLog")

        'Retrieve the defaults
        Dim dt As DataTable = RetrieveLogDefaults(logType)

        'Is this type of logging switched on
        If dt.Rows.Count > 0 Then

            'Write the log record
            CreateLogEntry(ConstructFileName(dt.Rows(0)), _
                            ConstructLogValue(CType(dt.Rows(0).Item("USE_QUOTES"), Boolean), sModule, errObj.ErrorNumber, errObj.ErrorMessage, errObj.ErrorStatus, ""))

        End If

    End Sub

    Public Sub ErrorObjectLog(ByVal sModule As String, ByVal errorCode As String, ByVal errorMessage As String, Optional ByVal logType As String = "ErrorObjectLog")

        'Retrieve the defaults
        Dim dt As DataTable = RetrieveLogDefaults(logType)

        'Is this type of logging switched on
        If dt.Rows.Count > 0 Then

            'Write the log record
            CreateLogEntry(ConstructFileName(dt.Rows(0)), _
                            ConstructLogValue(CType(dt.Rows(0).Item("USE_QUOTES"), Boolean), sModule, errorCode, errorMessage, String.Empty, String.Empty))

        End If

    End Sub

    Public Sub ExceptionLog(ByVal sModule As String, ByVal exceptionString As String, Optional ByVal logType As String = "ExceptionLog")

        'Retrieve the defaults
        Dim dt As DataTable = RetrieveLogDefaults(logType)

        'Is this type of logging switched on
        If dt.Rows.Count > 0 Then

            'Write the log record
            CreateLogEntry(ConstructFileName(dt.Rows(0)), _
                            ConstructLogValue(CType(dt.Rows(0).Item("USE_QUOTES"), Boolean), sModule, exceptionString, "", "", ""))

        End If

    End Sub

    Public Sub ExceptionLog(ByVal sModule As String, ByVal exceptionObject As Exception, Optional ByVal logType As String = "ExceptionLog")
        Dim dt As DataTable = RetrieveLogDefaults(logType)
        Dim memStream As New MemoryStream
        Dim formatter As New BinaryFormatter
        formatter.Serialize(memStream, exceptionObject)
        Dim bytes(memStream.Length) As Byte
        bytes = memStream.ToArray
        'need to write the bytes to a database here
        If dt.Rows.Count > 0 Then
            CreateLogEntry(ConstructFileName(dt.Rows(0)), ConstructLogValue(CType(dt.Rows(0).Item("USE_QUOTES"), Boolean), sModule, exceptionObject.TargetSite.Name, exceptionObject.Message, exceptionObject.Source, exceptionObject.StackTrace))
        End If
    End Sub

#End Region

End Class
