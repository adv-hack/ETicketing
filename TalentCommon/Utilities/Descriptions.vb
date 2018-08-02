Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Methods relating to descriptions file
'
'       Date                        20th Nov 2006
'
'       Author                      Ben Ford  
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACUTDE- 
'                                   
'--------------------------------------------------------------------------------------------------
'   Modification Summary
'
'   dd/mm/yy    ID      By      Description
'   --------    -----   ---     -----------
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class Descriptions
    '----------------------------------------------------------------------------------------------
    ' Return a string padded to the correct length
    '
    Public Shared Function GetDescription(ByVal connectionString As String, _
                                ByVal strLanguage As String, _
                                ByVal strType As String, _
                                ByVal strCode As String) As String
        Dim strDescription As String = String.Empty
        Dim conTalent As SqlConnection

        Dim cmdGet As SqlCommand
        Dim dtrReader As SqlDataReader

        conTalent = New SqlConnection(connectionString)

        Try
            conTalent.Open()
            Const Param0 As String = "@Param0"
            Const Param1 As String = "@Param1"
            Const Param2 As String = "@Param2"
            Const Desc As String = "DESCRIPTION"
            '
            Const SQLGet As String = "SELECT DESCRIPTION FROM tbl_descriptions_detail WITH (NOLOCK)  " & _
                                 " WHERE LANGUAGE = @Param0 " & _
                                 " AND TYPE = @Param1   " & _
                                 " AND CODE = @Param2   "

            ''todo TYPE in the above script is a reserved word and will eventually cause problems

            cmdGet = New SqlCommand(SQLGet, conTalent)
            With cmdGet.Parameters
                .Add(New SqlParameter(Param0, SqlDbType.Char, 3)).Value = strLanguage
                .Add(New SqlParameter(Param1, SqlDbType.Char, 10)).Value = strType
                .Add(New SqlParameter(Param2, SqlDbType.Char, 20)).Value = strCode
            End With
            dtrReader = cmdGet.ExecuteReader
            With dtrReader
                If .Read Then _
                    strDescription = .Item(Desc).ToString.Trim
                .Close()
            End With
            dtrReader = Nothing
        Catch ex As Exception
            ' LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), "Utilities.vb - GetDeliveryCOuntry - " & Environment.NewLine & ex.Message)
        Finally
            Try
                conTalent.Close()
            Catch ex As Exception
                '  LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), "Utilities.vb - closing connection in finally block" & Environment.NewLine & ex.Message)
            End Try
            conTalent = Nothing
        End Try
        Return strDescription
    End Function

End Class
