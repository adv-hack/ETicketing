Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Write textual log information to log file.
'
'       Date                        28.02.07
'
'       Author                      Ben (from V8_4)
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPLOGW- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------

Namespace Talent.eCommerce

    Public Class LogWriter

        Public Const FOLDER_APPLICATION As String = "APPLICATION"
        Public Const DASH As String = "-", PIPE As String = "|"


        Public Shared Sub WriteToLog(ByVal logRoot As String, ByVal entry As String)
            WriteToLog(logRoot, Nothing, entry)
        End Sub

        Public Shared Sub WriteToLog(ByVal logRoot As String, ByVal folder As String, ByVal entry As String)

            Dim strLogFile As String = String.Empty

            Try
                strLogFile &= logRoot
                If Not logRoot.EndsWith("\") Then
                    strLogFile &= "\"
                End If

                If Not folder Is Nothing Then
                    strLogFile &= folder
                    If Not folder.EndsWith("\") Then
                        strLogFile &= "\"
                    End If
                    If Not Directory.Exists(strLogFile) Then
                        Directory.CreateDirectory(strLogFile)
                    End If
                End If

                strLogFile &= DateTime.Now.Year
                strLogFile &= "-"
                strLogFile &= DateTime.Now.Month
                strLogFile &= "-"
                strLogFile &= DateTime.Now.Day
                strLogFile &= ".txt"

                Dim w As System.IO.StreamWriter

                w = New System.IO.StreamWriter(strLogFile, True)
                w.AutoFlush = True
                w.WriteLine(DateTime.Now.ToLongTimeString() & " : " & entry)

                w.Close()
            Catch ex As Exception
                ' throw away the exception otherwise we may get into a loop.
            End Try

        End Sub

        Public Shared Sub WriteCacheIssueToLog(ByVal errorNumber As String, ByVal errorMessage As String, ByVal exceptionMessage As String, Optional ByVal exceptionObj As Exception = Nothing)

            Try
                Dim path As String = ConfigurationManager.AppSettings("LogCacheIssuePath").ToString
                If Not String.IsNullOrWhiteSpace(path) Then
                    If Not path.EndsWith("\") Then path += "\"
                    Dim filename As String = Now.Year & DASH & Now.Month & DASH & Now.Day & ".txt"
                    path += filename
                    Dim exObjMessage As String = ""

                    Dim writer As New System.IO.StreamWriter(path, True)
                    writer.AutoFlush = True
                    writer.WriteLine(Now.ToString & PIPE & _
                                        errorNumber & PIPE & _
                                        errorMessage & PIPE & _
                                        exceptionMessage)
                    If exceptionObj IsNot Nothing Then
                        exObjMessage = exObjMessage + exceptionObj.Message
                        exObjMessage = exObjMessage + exceptionObj.StackTrace
                        writer.WriteLine(exObjMessage)
                    End If

                    writer.Close()
                    writer.Dispose()
                End If

            Catch ex As Exception
                ' throw away the exception otherwise we may get into a loop.
            End Try

        End Sub

    End Class

End Namespace
