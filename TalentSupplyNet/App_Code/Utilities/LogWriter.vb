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
'       Date                        Nov 2006
'
'       Author                      
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPLOGW- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class LogWriter

        Private Shared bFirstPass As Boolean = True
        Private Shared conTalent As SqlConnection = Nothing

        Private Const Param0 As String = "@Param0"
        Private Const Param1 As String = "@Param1"
        Private Const Param2 As String = "@Param2"
        Private Const Param3 As String = "@Param3"
        Private Const Param4 As String = "@Param4"
        Private Const Param5 As String = "@Param5"
        Private Const Param6 As String = "@Param6"
        Private Const Param7 As String = "@Param7"
        Private Const Param8 As String = "@Param8"

        Private Const dFormat As String = "dd/MMM/yyyy HH:mm:ss"

        Private Shared Function OpenDB() As ErrorObj
            Dim err As New ErrorObj
            ''----------------------------------------------------------------------------------
            ''   Open connection to DB
            ''
            'Try
            '    Const SqlServer2005 As String = "SqlServer2005"
            '    conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
            '    conTalent.Open()            '
            'Catch ex As Exception
            '    Const strError1 As String = "Could not establish connection to the database"
            '    With err
            '        .ErrorMessage = ex.Message
            '        .ErrorStatus = strError1
            '        .ErrorNumber = "TTPLOGW-01"
            '        .HasError = True
            '    End With
            'End Try
            Return err
        End Function
        Public Shared Sub WriteDBLog(ByVal writeLog As Boolean, _
                                        ByVal transactionID As String, _
                                        ByVal status As String, _
                                        ByVal wstype As String, _
                                        ByVal reqXmlU As String, _
                                        ByVal reqXmlT As String, _
                                        ByVal resXmlU As String, _
                                        ByVal resXmlT As String, _
                                        ByVal xsltPath As String, _
                                        ByVal xsdPath As String, _
                                        ByVal xmlDoc As XmlDocument, _
                                        ByVal selectCase As Integer)

        End Sub

        Public Shared Sub WriteDBLog(ByVal writeLog As Boolean, _
                                        ByVal transactionNumber As Long, _
                                        ByVal status As String, _
                                        ByVal wstype As String, _
                                        ByVal reqXmlU As String, _
                                        ByVal reqXmlT As String, _
                                        ByVal resXmlU As String, _
                                        ByVal resXmlT As String, _
                                        ByVal xsltPath As String, _
                                        ByVal xsdPath As String, _
                                        ByVal xmlDoc As XmlDocument, _
                                        ByVal selectCase As Integer)

            'Select Case selectCase
            '    Case 1 'Save the document as the untransformed request
            '        xmlDoc.Save(reqXmlU)
            '    Case 2 'Save the document as the transformed request
            '        xmlDoc.Save(reqXmlT)
            '    Case 3 'Save the document as the untransformed response
            '        xmlDoc.Save(resXmlU)
            '    Case 4 'Save the document as the transformed response
            '        xmlDoc.Save(resXmlT)
            'End Select

            ''Write the log to the DB
            'WriteDBLog(writeLog, transactionNumber, status, wstype, _
            '                            reqXmlU, reqXmlT, _
            '                            resXmlU, resXmlT, _
            '                            xsltPath, xsdPath)
        End Sub
        Public Shared Sub WriteDBLog(ByVal writeLog As Boolean, _
                              ByVal transactionID As String, _
                              ByVal status As String, _
                              ByVal company As String, _
                              ByVal loginId As String, _
                              ByVal webService As String)

        End Sub
        Public Shared Sub WriteDBLog(ByVal writeLog As Boolean, _
                                      ByVal transactionNumber As Long, _
                                      ByVal status As String, _
                                      ByVal company As String, _
                                      ByVal loginId As String, _
                                      ByVal webService As String)

            '----------------------------------------------------------------------------------
            '   The LogWriter class should always write to web_service_control, but the 
            '   writes to web_service_log should be controlled by the logging enabled flags, 
            '   as current.
            '
            'Dim err As New ErrorObj
            'Dim cmdSelect As SqlCommand = Nothing
            'err = OpenDB()
            'If Not err.HasError Then
            '    Try
            '        ''Begin SA1 - DB Restructure
            '        ''Const strInsert1 As String = "INSERT INTO tbl_web_service_control " & _
            '        ''        "(COMPANY, LOGINID, WEB_SERVICE, TRANSACTION_NUMBER, CREATED_TIMESTAMP, CURRENT_STATUS ) VALUES (" & _
            '        ''        " @Param1, @Param2, @Param3, @Param4, @Param5, @Param6 )"
            '        Const strInsert1 As String = "INSERT INTO tbl_supplynet_log_control " & _
            '                                    "(COMPANY, LOGINID, WEB_SERVICE, TRANSACTION_NUMBER, CREATED_TIMESTAMP, CURRENT_STATUS " & _
            '                                    ") VALUES (@Param1, @Param2, @Param3, @Param4, @Param5, @Param6 )"
            '        ''End SA1 - DB Restructure
            '        Dim cmdInsert1 As SqlCommand
            '        cmdInsert1 = New SqlCommand(strInsert1, conTalent)
            '        With cmdInsert1
            '            With .Parameters()
            '                .Add(New SqlParameter(Param1, SqlDbType.Char, 20)).Value = company
            '                .Add(New SqlParameter(Param2, SqlDbType.Char, 20)).Value = loginId
            '                .Add(New SqlParameter(Param3, SqlDbType.Char, 50)).Value = webService
            '                .Add(New SqlParameter(Param4, SqlDbType.BigInt)).Value = transactionNumber
            '                .Add(New SqlParameter(Param5, SqlDbType.DateTime)).Value = Now.ToString(dFormat)
            '                .Add(New SqlParameter(Param6, SqlDbType.Char, 100)).Value = status
            '            End With
            '            .ExecuteNonQuery()
            '        End With
            '        cmdInsert1 = Nothing
            '    Catch ex As Exception
            '        Const strError1 As String = "Error writting to the database"
            '        With err
            '            .ErrorMessage = ex.Message
            '            .ErrorStatus = strError1
            '            .ErrorNumber = "TTPLOGW-12"
            '            .HasError = True
            '        End With
            '    End Try
            'End If
            ''------------------------------------------------------------------------------
            ''   The LogWriter class should always write to web_service_control, but the writes 
            ''   to web_service_log should be controlled by the logging enabled flags, as current.
            ''   
            'Try
            '    If Not err.HasError And writeLog Then
            '        ''Begin SA1 - DB Restructure
            '        Const strInsert2 As String = _
            '                            "INSERT INTO tbl_supplynet_log_audit " & _
            '                            "(TRANSACTION_NUMBER, TIME_STAMP, STATUS, WSTYPE) VALUES (" & _
            '                            " @Param1, @Param2,  @Param3 ,'Create')"
            '        ''Const strInsert2 As String = _
            '        ''                    "INSERT INTO tbl_web_service_log " & _
            '        ''"(TRANSACTION_NUMBER, TIME_STAMP, STATUS, WSTYPE) VALUES (" & _
            '        ''" @Param1, @Param2,  @Param3 ,'Create')"
            '        ''End SA1 - DB Restructure
            '        Dim cmdInsert2 As SqlCommand
            '        cmdInsert2 = New SqlCommand(strInsert2, conTalent)
            '        With cmdInsert2
            '            With .Parameters()
            '                .Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = transactionNumber
            '                .Add(New SqlParameter(Param2, SqlDbType.DateTime)).Value = Now.ToString(dFormat)
            '                .Add(New SqlParameter(Param3, SqlDbType.Char, 20)).Value = status
            '            End With
            '            .ExecuteNonQuery()
            '        End With
            '        ' 
            '        conTalent.Close()
            '    End If
            '    conTalent.Close()
            'Catch ex As Exception
            '    Const strError1 As String = "Error writting to the database"
            '    With err
            '        .ErrorMessage = ex.Message
            '        .ErrorStatus = strError1
            '        .ErrorNumber = "TTPLOGW-13"
            '        .HasError = True
            '    End With
            'End Try
            ''------------------------------------------------------------------------------
            ''   If the database is unavailable, LogWriter should continue to write to file.
            ''
            'If err.HasError Then
            '    Dim al As New ArrayList
            '    al = placeHolderList(1)
            '    al(0) = transactionNumber
            '    al(1) = Now.ToString(dFormat)
            '    al(2) = status
            '    al(3) = company
            '    al(4) = loginId
            '    al(5) = webService
            '    Dim entry As String = createTxtLogEntry(al)
            '    WriteToLog(entry)
            'End If
        End Sub
        Public Shared Sub WriteDBLog(ByVal writeLog As Boolean, _
                                        ByVal transactionID As String, _
                                        ByVal errorMessage As String, _
                                        ByVal errorStatus As String, _
                                        ByVal errorNumber As String)

        End Sub
        Public Shared Sub WriteDBLog(ByVal writeLog As Boolean, _
                                                ByVal transactionNumber As Long, _
                                                ByVal errorMessage As String, _
                                                ByVal errorStatus As String, _
                                                ByVal errorNumber As String)

            ''----------------------------------------------------------------------------------
            ''   The LogWriter class should always write to web_service_control, but the 
            ''   writes to web_service_log should be controlled by the logging enabled flags, 
            ''   as current.
            ''
            'Dim err As New ErrorObj
            'Dim cmdSelect As SqlCommand = Nothing
            ''----------------------------------------------------------------------------------
            'err = OpenDB()
            'If Not err.HasError Then
            '    Try
            '        '------------------------------------------------------------------------------
            '        ''Begin SA1 - DB Restructure
            '        Dim strWork As String = " UPDATE tbl_supplynet_log_control SET " & _
            '                " LAST_UPDATED_TIMESTAMP = @Param0  " & _
            '                ", ERROR_MESSAGE = @Param1 " & _
            '                ", ERROR_STATUS = @Param2 " & _
            '                ", ERROR_CODE = @Param3 " & _
            '                " WHERE TRANSACTION_NUMBER = @Param4 "
            '        ''Dim strWork As String = " UPDATE tbl_web_service_control SET " & _
            '        ''                            " LAST_UPDATED_TIMESTAMP = @Param0  " & _
            '        ''                            ", ERROR_MESSAGE = @Param1 " & _
            '        ''                            ", ERROR_STATUS = @Param2 " & _
            '        ''                            ", ERROR_CODE = @Param3 " & _
            '        ''                            " WHERE TRANSACTION_NUMBER = @Param4 "
            '        ''End SA1 - DB Restructure
            '        '
            '        Dim cmdUpdate As SqlCommand
            '        cmdUpdate = New SqlCommand(strWork, conTalent)
            '        With cmdUpdate
            '            With .Parameters()
            '                .Add(New SqlParameter(Param0, SqlDbType.DateTime)).Value = Now.ToString(dFormat)
            '                .Add(New SqlParameter(Param1, SqlDbType.Char)).Value = errorMessage.Trim
            '                .Add(New SqlParameter(Param2, SqlDbType.Char)).Value = errorStatus.Trim
            '                .Add(New SqlParameter(Param3, SqlDbType.Char)).Value = errorNumber
            '                .Add(New SqlParameter(Param4, SqlDbType.BigInt)).Value = transactionNumber
            '            End With
            '            .ExecuteNonQuery()
            '        End With
            '        Try
            '            conTalent.Close()
            '        Catch ex2 As Exception

            '        End Try

            '    Catch ex As Exception
            '        Try
            '            conTalent.Close()
            '        Catch ex2 As Exception

            '        End Try
            '        '------------------------------------------------------------------------------
            '        '   If the database is unavailable, LogWriter should continue to write to file.
            '        '
            '        Dim al As New ArrayList
            '        al = placeHolderList(3)

            '        al.Add(transactionNumber)
            '        al.Add(Now.ToString(dFormat))
            '        al.Add(errorStatus)

            '        Dim entry As String = createTxtLogEntry(al)
            '        WriteToLog(entry)
            '    Finally

            '    End Try
            'End If
        End Sub
        Public Shared Sub WriteDBLog(ByVal writeLog As Boolean, _
                                ByVal transactionID As String, _
                                ByVal status As String, _
                                ByVal wstype As String)

        End Sub
        Public Shared Sub WriteDBLog(ByVal writeLog As Boolean, _
                                        ByVal transactionNumber As Long, _
                                        ByVal status As String, _
                                        ByVal wstype As String)

            ''----------------------------------------------------------------------------------
            ''   The LogWriter class should always write to web_service_control, but the 
            ''   writes to web_service_log should be controlled by the logging enabled flags, 
            ''   as current.
            ''
            'Dim err As New ErrorObj
            'Dim cmdSelect As SqlCommand = Nothing
            'err = OpenDB()
            'If Not err.HasError Then
            '    Try
            '        '------------------------------------------------------------------------------
            '        ''Begin SA1 - DB Restructure
            '        Dim strUpdate As String = " UPDATE tbl_supplynet_log_control SET " & _
            '            "  CURRENT_STATUS = @Param1, LAST_UPDATED_TIMESTAMP = @Param2 " & _
            '            "  WHERE TRANSACTION_NUMBER = @Param3"
            '        ''Dim strUpdate As String = " UPDATE tbl_web_service_control SET " & _
            '        ''                        "  CURRENT_STATUS = @Param1, LAST_UPDATED_TIMESTAMP = @Param2 " & _
            '        ''                        "  WHERE TRANSACTION_NUMBER = @Param3"
            '        ''End SA1 - DB Restructure
            '        '
            '        Dim cmdUpdate As SqlCommand
            '        cmdUpdate = New SqlCommand(strUpdate, conTalent)
            '        With cmdUpdate
            '            With .Parameters()
            '                .Add(New SqlParameter(Param1, SqlDbType.Char)).Value = status
            '                .Add(New SqlParameter(Param2, SqlDbType.DateTime)).Value = Now.ToString(dFormat)
            '                .Add(New SqlParameter(Param3, SqlDbType.BigInt)).Value = transactionNumber
            '                '
            '            End With
            '            .ExecuteNonQuery()
            '        End With
            '        '------------------------------------------------------------------------------
            '        '   The LogWriter class should always write to web_service_control, but the writes 
            '        '   to web_service_log should be controlled by the logging enabled flags, as current.
            '        '   
            '        If writeLog Then
            '            ''Begin SA1 - DB Restructure
            '            ''    Const strInsert2 As String = _
            '            ''"INSERT INTO tbl_web_service_log " & _
            '            ''"(TRANSACTION_NUMBER, TIME_STAMP, STATUS, WSTYPE) VALUES " & _
            '            ''"(@Param1, @Param2, @Param3, @Param4 )"
            '            Const strInsert2 As String = _
            '                                "INSERT INTO tbl_supplynet_log_audit " & _
            '                                "(TRANSACTION_NUMBER, TIME_STAMP, STATUS, WSTYPE) VALUES " & _
            '                                "(@Param1, @Param2, @Param3, @Param4 )"
            '            ''End SA1 - DB Restructure
            '            Dim cmdInsert2 As SqlCommand
            '            cmdInsert2 = New SqlCommand(strInsert2, conTalent)
            '            With cmdInsert2
            '                With .Parameters()
            '                    .Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = transactionNumber
            '                    .Add(New SqlParameter(Param2, SqlDbType.DateTime)).Value = Now.ToString(dFormat)
            '                    .Add(New SqlParameter(Param3, SqlDbType.Char)).Value = status
            '                    .Add(New SqlParameter(Param4, SqlDbType.Char)).Value = wstype
            '                End With
            '                .ExecuteNonQuery()
            '            End With
            '            ' 
            '            conTalent.Close()
            '        End If
            '    Catch ex As Exception
            '        Try
            '            conTalent.Close()
            '        Catch ex2 As Exception

            '        End Try
            '        '------------------------------------------------------------------------------
            '        '   If the database is unavailable, LogWriter should continue to write to file.
            '        '
            '        Dim al As New ArrayList
            '        al = placeHolderList(3)

            '        al(0) = transactionNumber
            '        al(1) = Now.ToString(dFormat)
            '        al(2) = status

            '        Dim entry As String = createTxtLogEntry(al)
            '        WriteToLog(entry)
            '    End Try
            'End If
        End Sub
        Public Shared Sub WriteDBLog(ByVal writeLog As Boolean, _
                                 ByVal transactionID As String, _
                                 ByVal status As String, _
                                 ByVal wstype As String, _
                                 ByVal reqXmlU As String, _
                                 ByVal reqXmlT As String, _
                                 ByVal resXmlU As String, _
                                 ByVal resXmlT As String, _
                                 ByVal xsltPath As String, _
                                 ByVal xsdPath As String)

        End Sub

        Public Shared Sub WriteDBLog(ByVal writeLog As Boolean, _
                                         ByVal transactionNumber As Long, _
                                         ByVal status As String, _
                                         ByVal wstype As String, _
                                         ByVal reqXmlU As String, _
                                         ByVal reqXmlT As String, _
                                         ByVal resXmlU As String, _
                                         ByVal resXmlT As String, _
                                         ByVal xsltPath As String, _
                                         ByVal xsdPath As String)

            ''----------------------------------------------------------------------------------
            ''   The LogWriter class should always write to web_service_control, but the 
            ''   writes to web_service_log should be controlled by the logging enabled flags, 
            ''   as current.
            ''
            'Dim err As New ErrorObj
            'Dim cmdSelect As SqlCommand = Nothing
            'err = OpenDB()
            'If Not err.HasError Then
            '    Try
            '        '------------------------------------------------------------------------------
            '        '   This next bit does tend to load repeated data
            '        '
            '        '   (1833, '20071220 18:08:24', 'L0300', 'csg', '20071220 18:08:36', 'ben', 
            '        '   'C:\Websites\TalentEBusiness\TalentTradingPortal\Documents\Logs\XMLFiles\Requests\U0000001833.xml', 
            '        '   'C:\Websites\TalentEBusiness\TalentTradingPortal\Documents\Logs\XMLFiles\Requests\T0000001833.xml', 
            '        '   'C:\Websites\TalentEBusiness\TalentTradingPortal\Documents\Logs\XMLFiles\Responses\U0000001833.xml', 
            '        '   'C:\Websites\TalentEBusiness\TalentTradingPortal\Documents\Logs\XMLFiles\Responses\T0000001833.xml', '
            '        '   OrderRequest', NULL, 'C:\Websites\TalentEBusiness\TalentTradingPortal\Documents\XSLT\Responses\', 
            '        '   'C:\Websites\TalentEBusiness\TalentTradingPortal\Documents\XSD\Responses\OrderRequest10.xsd', NULL, NULL, NULL)
            '        '
            '        '   may need to trim data down
            '        '------------------------------------------------------------------------------
            '        Dim strUpdate As New StringBuilder
            '        ''Begin SA1 - DB Restructure
            '        Const strWork1 As String = "UPDATE tbl_supplynet_log_control SET CURRENT_STATUS = @Param0, LAST_UPDATED_TIMESTAMP = @Param1 "
            '        ''Const strWork1 As String = "UPDATE tbl_web_service_control SET CURRENT_STATUS = @Param0, LAST_UPDATED_TIMESTAMP = @Param1 "
            '        ''End SA1 - DB Restructure
            '        With strUpdate
            '            .Append(strWork1)
            '            '
            '            If xsltPath.Trim.Length > 0 Then .Append(", XSLT_PATH = @Param2 ")
            '            If xsdPath.Trim.Length > 0 Then .Append(", XSD_PATH = @Param3  ")
            '            If reqXmlU.Trim.Length > 0 Then .Append(", REQUEST_XML_DOC_PATH_U = @Param4")
            '            If reqXmlT.Trim.Length > 0 Then .Append(", REQUEST_XML_DOC_PATH_T = @Param5")
            '            If resXmlU.Trim.Length > 0 Then .Append(", RESPONSE_XML_DOC_PATH_U = @Param6")
            '            If resXmlT.Trim.Length > 0 Then .Append(", RESPONSE_XML_DOC_PATH_T = @Param7")
            '            '
            '            .Append("  WHERE TRANSACTION_NUMBER = @Param8")
            '            '
            '        End With
            '        Dim strWork As String = strUpdate.ToString

            '        Dim cmdUpdate As SqlCommand
            '        cmdUpdate = New SqlCommand(strWork, conTalent)

            '        With cmdUpdate
            '            With .Parameters()
            '                .Add(New SqlParameter(Param0, SqlDbType.Char, 20)).Value = status
            '                .Add(New SqlParameter(Param1, SqlDbType.DateTime)).Value = Now.ToString(dFormat)
            '                '
            '                If xsltPath.Trim.Length > 0 Then .Add(New SqlParameter(Param2, SqlDbType.Char, 100)).Value = xsltPath
            '                If xsdPath.Trim.Length > 0 Then .Add(New SqlParameter(Param3, SqlDbType.Char, 100)).Value = xsdPath
            '                If reqXmlU.Trim.Length > 0 Then .Add(New SqlParameter(Param4, SqlDbType.Char, 100)).Value = reqXmlU
            '                If reqXmlT.Trim.Length > 0 Then .Add(New SqlParameter(Param5, SqlDbType.Char, 100)).Value = reqXmlT
            '                If resXmlU.Trim.Length > 0 Then .Add(New SqlParameter(Param6, SqlDbType.Char, 100)).Value = resXmlU
            '                If resXmlT.Trim.Length > 0 Then .Add(New SqlParameter(Param7, SqlDbType.Char, 100)).Value = resXmlT
            '                '
            '                .Add(New SqlParameter("@Param8", SqlDbType.BigInt)).Value = transactionNumber
            '                '
            '            End With
            '            .ExecuteNonQuery()
            '        End With
            '        '------------------------------------------------------------------------------
            '        '   The LogWriter class should always write to web_service_control, but the writes 
            '        '   to web_service_log should be controlled by the logging enabled flags, as current.
            '        '   
            '        If writeLog Then
            '            ''Begin SA1 - DB Restructure
            '            ''    Const strInsert2 As String = _
            '            ''"INSERT INTO tbl_web_service_log " & _
            '            ''"(TRANSACTION_NUMBER, TIME_STAMP, STATUS, WSTYPE) VALUES (" & _
            '            ''" @Param1, @Param2, @Param3, @Param4 )"
            '            Const strInsert2 As String = _
            '                                "INSERT INTO tbl_supplynet_log_audit " & _
            '                                "(TRANSACTION_NUMBER, TIME_STAMP, STATUS, WSTYPE) VALUES (" & _
            '                                " @Param1, @Param2, @Param3, @Param4 )"
            '            ''End SA1 - DB Restructure
            '            Dim cmdInsert2 As SqlCommand
            '            cmdInsert2 = New SqlCommand(strInsert2, conTalent)
            '            With cmdInsert2
            '                With .Parameters()
            '                    .Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = transactionNumber
            '                    .Add(New SqlParameter(Param2, SqlDbType.DateTime)).Value = Now.ToString(dFormat)
            '                    .Add(New SqlParameter(Param3, SqlDbType.Char)).Value = status
            '                    .Add(New SqlParameter(Param4, SqlDbType.Char)).Value = wstype
            '                End With
            '                .ExecuteNonQuery()
            '            End With
            '            ' 
            '            conTalent.Close()
            '        End If
            '        Try
            '            conTalent.Close()
            '        Catch ex2 As Exception

            '        End Try

            '    Catch ex As Exception
            '        Try
            '            conTalent.Close()
            '        Catch ex2 As Exception

            '        End Try
            '        '------------------------------------------------------------------------------
            '        '   If the database is unavailable, LogWriter should continue to write to file.
            '        '
            '        Dim al As New ArrayList
            '        al = placeHolderList(2)

            '        al(0) = transactionNumber
            '        al(1) = Now.ToString(dFormat)
            '        al(2) = status
            '        al(3) = reqXmlU
            '        al(4) = reqXmlT
            '        al(5) = resXmlU
            '        al(6) = resXmlT
            '        al(7) = xsltPath
            '        al(8) = xsdPath

            '        Dim entry As String = createTxtLogEntry(al)
            '        WriteToLog(entry)
            '    End Try
            'End If
        End Sub
        Public Shared Sub WriteToLog(ByVal entry As String)
            'Dim strLogFile As New StringBuilder(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings("TxtLogPath")))
            ''
            'Const bslash As String = "\"
            'Const bslashs As String = "\\"
            'Const dash As String = "-"
            'Const colon As String = ":"
            'Const txt As String = ".txt"
            'Dim w As System.IO.StreamWriter
            'Try
            '    With strLogFile

            '        .Append(bslash)
            '        .Replace(bslashs, bslash)
            '        If Not Directory.Exists(.ToString) Then _
            '            Directory.CreateDirectory(.ToString)
            '        '
            '        .Append(DateTime.Now.Year)
            '        .Append(dash)
            '        .Append(DateTime.Now.Month)
            '        .Append(dash)
            '        .Append(DateTime.Now.Day)
            '        .Append(txt)
            '        w = New System.IO.StreamWriter(.ToString, True)
            '        With w
            '            .AutoFlush = True
            '            .WriteLine(DateTime.Now.ToLongTimeString() & colon & entry)
            '            .Close()
            '        End With
            '    End With
            'Catch ex As Exception
            '    ' throw away the exception otherwise we may get into a loop.
            'End Try
        End Sub

        Private Shared Function createTxtLogEntry(ByVal al As ArrayList) As String
            Dim entry As New StringBuilder(String.Empty)
            'Const c1 As String = "| "
            'For iCounter As Integer = 0 To al.Count - 1
            '    With entry
            '        .Append(c1)
            '        .Append(al(iCounter))
            '        .Append(vbTab)
            '    End With
            'Next
            Return entry.ToString
        End Function
        Private Shared Function placeHolderList(ByVal selectCase As Integer) As ArrayList
            '   Builds an ArrayList containing the parameters for the insert
            Dim params As New ArrayList()

            'With params
            '    .Add("@TRANSACTION")                            ' al(0) = transactionNumber
            '    .Add("@TIMESTAMP")                              ' al(1) = Now.ToString(dFormat)

            '    Select Case selectCase
            '        Case 1
            '            .Add("@STATUS")                         ' al(2) = status
            '            .Add("@COMPANY")                        ' al(3) = company
            '            .Add("@LOGINID")                        ' al(4) = loginId
            '            .Add("@WEB_SERVICE")                    ' al(5) = webService

            '        Case 2
            '            .Add("@REQUEST_XML_DOC_PATH_U")         ' al(3) = reqXmlU
            '            .Add("@REQUEST_XML_DOC_PATH_T")         ' al(4) = reqXmlT
            '            .Add("@RESPONSE_XML_DOC_PATH_U")        ' al(5) = resXmlU
            '            .Add("@RESPONSE_XML_DOC_PATH_T")        ' al(6) = resXmlT
            '            .Add("@XSLT_PATH")                      ' al(7) = xsltPath
            '            .Add("@XSD_PATH")                       ' al(8) = xsdPath

            '        Case 3

            '    End Select
            'End With
            Return params
        End Function
        Public Shared Function getFiles(ByVal Location As String, ByVal searchPattern As String) As FileInfo()
            'Get a list of files from the directory
            Dim storefile As New DirectoryInfo(Location)
            Dim files As FileInfo()

            'Populate the list with the files that match the pattern
            files = storefile.GetFiles(searchPattern)
            Return files
        End Function

    End Class

End Namespace