Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Profile Class
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'  
'       Error Number Code base      TTPPROF- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class Profile

        Private _applyIncomingStyleSheet As Boolean = False
        Private _applyOutgoingStyleSheet As Boolean = False
        Private _autoProcess As Boolean = False
        Private _autoProcessDefaultUser As Boolean = False
        Private _cacheTimeMinutes As Integer = 0
        Private _emailCompany As String = String.Empty
        Private _emailUser As String = String.Empty
        Private _emailXmlResponse As Boolean = False
        Private _incomingStyleSheet As String = String.Empty
        Private _outgoingStyleSheet As String = String.Empty
        Private _outgoingXmlResponse As String = String.Empty
        Private _responseVersion As String = "1.0"
        Private _storeXml As Boolean = False
        Private _validUser As Boolean = False
        Private _webServiceDestinationDatabase As String = String.Empty
        Private _webServiceStoredProcedureGroup As String = String.Empty
        Private _writeLog As Boolean = False
        Private _xsd As String = String.Empty
        Private _databaseType1 As String = String.Empty
        Private _businessUnit As String = String.Empty
        Private _logRequests As Boolean = False
        Private _responseDirectory As String = String.Empty

        Public Property ApplyIncomingStyleSheet() As Boolean
            Get
                Return _applyIncomingStyleSheet
            End Get
            Set(ByVal value As Boolean)
                _applyIncomingStyleSheet = value
            End Set
        End Property
        Public Property ApplyOutgoingStyleSheet() As Boolean
            Get
                Return _applyOutgoingStyleSheet
            End Get
            Set(ByVal value As Boolean)
                _applyOutgoingStyleSheet = value
            End Set
        End Property
        Public Property AutoProcess() As Boolean
            Get
                Return _autoProcess
            End Get
            Set(ByVal value As Boolean)
                _autoProcess = value
            End Set
        End Property
        Public Property AutoProcessDefaultUser() As Boolean
            Get
                Return _autoProcessDefaultUser
            End Get
            Set(ByVal value As Boolean)
                _autoProcessDefaultUser = value
            End Set
        End Property
        Public Property CacheTimeMinutes() As String
            Get
                Return _cacheTimeMinutes
            End Get
            Set(ByVal value As String)
                _cacheTimeMinutes = value
            End Set
        End Property
        Public Property EmailCompany() As String
            Get
                Return _emailCompany
            End Get
            Set(ByVal value As String)
                _emailCompany = value
            End Set
        End Property
        Public Property EmailUser() As String
            Get
                Return _emailUser
            End Get
            Set(ByVal value As String)
                _emailUser = value
            End Set
        End Property
        Public Property EmailXmlResponse() As Boolean
            Get
                Return _emailXmlResponse
            End Get
            Set(ByVal value As Boolean)
                _emailXmlResponse = value
            End Set
        End Property
        Public Property IncomingStyleSheet() As String
            Get
                Return _incomingStyleSheet
            End Get
            Set(ByVal value As String)
                _incomingStyleSheet = value
            End Set
        End Property
        Public Property OutgoingStyleSheet() As String
            Get
                Return _outgoingStyleSheet
            End Get
            Set(ByVal value As String)
                _outgoingStyleSheet = value
            End Set
        End Property
        Public Property OutgoingXmlResponse() As String
            Get
                Return _outgoingXmlResponse
            End Get
            Set(ByVal value As String)
                _outgoingXmlResponse = value
            End Set
        End Property
        Public Property ResponseVersion() As String
            Get
                Return _responseVersion
            End Get
            Set(ByVal value As String)
                _responseVersion = value
            End Set
        End Property
        Public Property StoreXml() As Boolean
            Get
                Return _storeXml
            End Get
            Set(ByVal value As Boolean)
                _storeXml = value
            End Set
        End Property
        Public Property ValidUser() As Boolean
            Get
                Return _validUser
            End Get
            Set(ByVal value As Boolean)
                _validUser = value
            End Set
        End Property
        Public Property WebServiceDestinationDatabase() As String
            Get
                Return _webServiceDestinationDatabase
            End Get
            Set(ByVal value As String)
                _webServiceDestinationDatabase = value
            End Set
        End Property
        Public Property WebServiceStoredProcedureGroup() As String
            Get
                Return _webServiceStoredProcedureGroup
            End Get
            Set(ByVal value As String)
                _webServiceStoredProcedureGroup = value
            End Set
        End Property
        Public Property WriteLog() As Boolean
            Get
                Return _writeLog
            End Get
            Set(ByVal value As Boolean)
                _writeLog = value
            End Set
        End Property
        Public Property Xsd() As String
            Get
                Return _xsd
            End Get
            Set(ByVal value As String)
                _xsd = value
            End Set
        End Property
        Public Property DatabaseType1() As String
            Get
                Return _databaseType1
            End Get
            Set(ByVal value As String)
                _databaseType1 = value
            End Set
        End Property
        Public Property BusinessUnit() As String
            Get
                Return _businessUnit
            End Get
            Set(ByVal value As String)
                _businessUnit = value
            End Set
        End Property

        Private _company As String
        Public Property Company() As String
            Get
                Return _company
            End Get
            Set(ByVal value As String)
                _company = value
            End Set
        End Property


        Public Property LogRequests() As Boolean
            Get
                Return _logRequests
            End Get
            Set(ByVal value As Boolean)
                _logRequests = value
            End Set
        End Property

        Public Property ResponseDirectory() As String
            Get
                Return _responseDirectory
            End Get
            Set(ByVal value As String)
                _responseDirectory = value
            End Set
        End Property


        Public Function CreateProfile(ByVal loginId As String, ByVal password As String, ByVal company As String, ByVal webServiceName As String) As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------------------
            '   Open connection to DB
            '
            Dim conTalent As SqlConnection = Nothing
            '---------------------------------------
            ' Investigate WC problem - add a retry
            Dim retryCount As Integer = 0
            Dim retry As Boolean = True

            While retry


                Try
                    Const SqlServer2005 As String = "SqlServer2005"
                    conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                    conTalent.Open()            '

                    '--------------------------------------------------------------------------------------
                    Dim cmdSelect As SqlCommand = Nothing
                    Dim dtrAuthority As SqlDataReader = Nothing
                    Dim dtrDatabase As SqlDataReader = Nothing
                    Const Parm1 As String = "@Parm1"
                    Const Parm2 As String = "@Parm2"
                    Const Parm3 As String = "@Parm3"
                    Const Parm4 As String = "@Parm4"
                    Const SQLString1 As String = "SELECT * FROM TBL_AUTHORIZED_USERS AS AU" & _
                                                " INNER JOIN TBL_PARTNER_USER AS PU " & _
                                                "       ON   AU.PARTNER = PU.PARTNER " & _
                                                "       AND  AU.LOGINID = PU.LOGINID " & _
                                                " WHERE AU.PARTNER = @Parm1 " & _
                                                " AND   AU.LOGINID = @Parm2 " & _
                                                " AND   AU.PASSWORD = @Parm3 " & _
                                                " AND   AU.BUSINESS_UNIT = @Parm4 "
                    Try
                        '------------------------------------------------------------------------------------
                        '   Valid User/Password/Company?
                        '
                        cmdSelect = New SqlCommand(SQLString1, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Parm1, SqlDbType.Char, 50))
                            .Parameters(Parm1).Value = company.Trim
                            .Parameters.Add(New SqlParameter(Parm2, SqlDbType.Char, 20))
                            .Parameters(Parm2).Value = loginId.Trim
                            .Parameters.Add(New SqlParameter(Parm3, SqlDbType.Char, 20))
                            .Parameters(Parm3).Value = password.Trim
                            .Parameters.Add(New SqlParameter(Parm4, SqlDbType.Char, 50))
                            .Parameters(Parm4).Value = BusinessUnit
                            '.Parameters(Parm4).Value = ConfigurationManager.AppSettings("DefaultBusinessUnit")
                            dtrAuthority = .ExecuteReader()

                        End With

                        If Not dtrAuthority.HasRows Then
                            Const strError2 As String = "Invalid loginId, password or company"
                            '-------------------------------------------------
                            ' Temp logging to investigate problem at Westcoast
                            '-------------------------------------------------
                            Dim sb As New StringBuilder
                            With sb
                                .Append("Profile.vb").Append(Environment.NewLine)
                                .Append("         ")
                                .Append(" Partner:").Append(company.Trim)
                                .Append(" Loginid:").Append(loginId.Trim)
                                .Append(" Password:").Append(password.Trim)
                                .Append(" Business Unit:").Append(BusinessUnit)
                                '.Append(" Business Unit:").Append(ConfigurationManager.AppSettings("DefaultBusinessUnit"))
                                .Append(Environment.NewLine)
                                .Append("         ")
                                .Append(" Contalent State:" & conTalent.State.ToString)
                                .Append(Environment.NewLine)
                                .Append("         ")
                                .Append(" Retry:" & retryCount.ToString)

                            End With
                            LogWriter.WriteToLog(sb.ToString)
                            With err
                                .ErrorMessage = String.Empty
                                .ErrorStatus = strError2
                                .ErrorNumber = "TTPPROF-04"
                                .HasError = True
                            End With
                            dtrAuthority.Close()
                        Else
                            dtrAuthority.Read()
                            EmailUser = dtrAuthority("EMAIL").trim
                            dtrAuthority.Close()
                            dtrAuthority = Nothing
                            '--------------------------------------------------------------------------------
                            '   Company Authorised?
                            '
                            Const SQLString2 As String = _
                                            " SELECT     ap.*, smpd.*, p.EMAIL AS cEmail " & _
                                            " FROM       tbl_authorized_partners AS ap " & _
                                            " INNER JOIN TBL_PARTNER AS P " & _
                                            "       ON   ap.PARTNER = P.PARTNER " & _
                                            " INNER JOIN TBL_SUPPLYNET_MODULE_PARTNER_DEFAULTS AS SMPD " & _
                                            "       ON   ap.PARTNER = SMPD.PARTNER " & _
                                            " WHERE      ap.PARTNER = @Parm1 " & _
                                            " AND        ap.BUSINESS_UNIT = @Parm2 " & _
                                            " AND       SMPD.MODULE = @Parm3 "
                            cmdSelect = New SqlCommand(SQLString2, conTalent)
                            With cmdSelect
                                .Parameters.Add(New SqlParameter(Parm1, SqlDbType.Char, 20))
                                .Parameters(Parm1).Value = company.Trim
                                .Parameters.Add(New SqlParameter(Parm2, SqlDbType.Char, 50))
                                .Parameters(Parm2).Value = BusinessUnit
                                '.Parameters(Parm2).Value = ConfigurationManager.AppSettings("DefaultBusinessUnit")
                                .Parameters.Add(New SqlParameter(Parm3, SqlDbType.Char, 50))
                                .Parameters(Parm3).Value = webServiceName.Trim

                                dtrAuthority = .ExecuteReader()


                                '----------------------------
                                ' Temp logging for WC issue
                                Dim sb2 As New StringBuilder
                                With sb2
                                    .Append("Profile4.vb").Append(Environment.NewLine)
                                    .Append("         ")
                                    .Append(" Database:").Append(conTalent.Database)
                                    .Append(" Datasource:").Append(conTalent.DataSource)
                                    .Append(" Contalent State:" & conTalent.State.ToString)

                                End With
                                LogWriter.WriteToLog(sb2.ToString)


                            End With
                            If Not dtrAuthority.HasRows() Then
                                Const strError3 As String = "Company not authorised to use service"
                                '-------------------------------------------------
                                ' Temp logging to investigate problem at Westcoast
                                '-------------------------------------------------
                                Dim sb As New StringBuilder
                                With sb
                                    .Append("Profile2.vb").Append(Environment.NewLine)
                                    .Append("         ")
                                    .Append(" Partner:").Append(company.Trim)
                                    .Append(" Loginid:").Append(loginId.Trim)
                                    .Append(" Password:").Append(password.Trim)
                                    .Append(" Business Unit:").Append(BusinessUnit)
                                    '.Append(" Business Unit:").Append(ConfigurationManager.AppSettings("DefaultBusinessUnit"))
                                    .Append(Environment.NewLine)
                                    .Append("         ")
                                    .Append(" Contalent State:" & conTalent.State.ToString)
                                    .Append(Environment.NewLine)
                                    .Append("         ")
                                    .Append(" Retry:" & retryCount.ToString)

                                End With
                                LogWriter.WriteToLog(sb.ToString)
                                With err
                                    .ErrorMessage = String.Empty
                                    .ErrorStatus = strError3
                                    .ErrorNumber = "TTPPROF-05"
                                    .HasError = True
                                End With
                                dtrAuthority.Close()
                            Else
                                dtrAuthority.Read()

                                '----------------------------
                                ' Temp logging for WC issue
                                'Dim sb3 As New StringBuilder
                                'With sb3
                                '    .Append("Profile5.vb").Append(Environment.NewLine)
                                '    .Append("         ")
                                '    For i As Integer = 0 To (dtrAuthority.FieldCount - 1)
                                '        .Append(dtrAuthority.GetName(i) & ",")
                                '    Next

                                'End With
                                'LogWriter.WriteToLog(sb3.ToString)
                                '----------------------------------------------------------------------------
                                '
                                AutoProcess = dtrAuthority("AUTO_PROCESS")
                                ''AutoProcessDefaultUser = dtrAuthority("AUTO_PROCESS_DEFAULT_USER")
                                CacheTimeMinutes = CType(dtrAuthority("CACHE_TIME_MINUTES"), Integer)
                                EmailCompany = dtrAuthority("cEMAIL").ToString.Trim
                                EmailUser = dtrAuthority("EMAIL").ToString.Trim
                                EmailXmlResponse = dtrAuthority("EMAIL_XML_RESPONSE")
                                IncomingStyleSheet = dtrAuthority("INCOMING_STYLE_SHEET").ToString.Trim
                                OutgoingStyleSheet = dtrAuthority("OUTGOING_STYLE_SHEET").ToString.Trim
                                OutgoingXmlResponse = dtrAuthority("OUTGOING_XML_RESPONSE").ToString.Trim
                                StoreXml = Convert.ToBoolean(dtrAuthority("STORE_XML"))
                                WebServiceDestinationDatabase = dtrAuthority("DESTINATION_DATABASE").ToString.Trim
                                Try
                                    WebServiceStoredProcedureGroup = dtrAuthority("STORED_PROCEDURE_GROUP").ToString.Trim
                                Catch ex As Exception
                                End Try
                                WriteLog = Convert.ToBoolean(dtrAuthority("LOGGING_ENABLED"))
                                Xsd = dtrAuthority("XML_SCHEMA_DOCUMENT").ToString.Trim
                                '----------------------------------------------------------------------------
                                If Not String.IsNullOrEmpty(IncomingStyleSheet) Then _
                                    ApplyIncomingStyleSheet = True

                                If Not String.IsNullOrEmpty(OutgoingStyleSheet) Then _
                                    ApplyOutgoingStyleSheet = True

                                'Ensure the default version is 1.0 if nothing found
                                Dim tempResponseVersion As String = dtrAuthority("RESPONSE_VERSION").ToString.Trim()
                                If Not String.IsNullOrEmpty(tempResponseVersion) Then _
                                    ResponseVersion = tempResponseVersion

                                LogRequests = Convert.ToBoolean(dtrAuthority("LOG_REQUESTS"))
                                If Not dtrAuthority("RESPONSE_DIRECTORY") Is Nothing Then
                                    ResponseDirectory = dtrAuthority("RESPONSE_DIRECTORY").ToString.Trim
                                End If

                                dtrAuthority.Close()
                                '--------------------------------------------------------------------------------
                                '   The code below is for User exclusion
                                ' 
                                Const SQLString3 As String = "SELECT * FROM tbl_excluded_users " & _
                                                            " WHERE PARTNER = @Parm1  " & _
                                                            " AND   LOGINID = @Parm2  " & _
                                                            " AND   MODULE = @Parm3  " & _
                                                            " AND   BUSINESS_UNIT = @Parm4"

                                cmdSelect = New SqlCommand(SQLString3, conTalent)
                                With cmdSelect
                                    .Parameters.Add(New SqlParameter(Parm1, SqlDbType.Char, 20))
                                    .Parameters(Parm1).Value = company.Trim
                                    .Parameters.Add(New SqlParameter(Parm2, SqlDbType.Char, 20))
                                    .Parameters(Parm2).Value = loginId.Trim
                                    .Parameters.Add(New SqlParameter(Parm3, SqlDbType.Char, 50))
                                    .Parameters(Parm3).Value = webServiceName.Trim
                                    .Parameters.Add(New SqlParameter(Parm4, SqlDbType.Char, 50))
                                    .Parameters(Parm4).Value = BusinessUnit
                                    ' .Parameters(Parm4).Value = ConfigurationManager.AppSettings("DefaultBusinessUnit").ToString()
                                    dtrAuthority = .ExecuteReader()
                                End With
                                '
                                If dtrAuthority.HasRows() Then
                                    Const strError4 As String = "Login not authorised to use service"
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = strError4
                                        .ErrorNumber = "TTPPROF-06"
                                        .HasError = True
                                    End With
                                Else
                                    'Passed authentication - this is a valid user!
                                    ValidUser = True
                                End If
                                dtrAuthority.Close()

                                '--------------------------------------------------------------------------------
                                '   Get Database Type
                                ' 
                                If WebServiceDestinationDatabase.Length > 0 Then
                                    Const SQLString4 As String = "SELECT * FROM TBL_DATABASE_VERSION " & _
                                                                " WHERE DESTINATION_DATABASE = @Parm1  AND  " & _
                                                                "       BUSINESS_UNIT = @Parm2 AND " & _
                                                                "       PARTNER = @Parm3 "

                                    cmdSelect = New SqlCommand(SQLString4, conTalent)
                                    With cmdSelect
                                        .Parameters.Add(New SqlParameter(Parm1, SqlDbType.Char, 20))
                                        .Parameters(Parm1).Value = WebServiceDestinationDatabase
                                        .Parameters.Add(New SqlParameter("PARM2", SqlDbType.Char, 50)).Value = BusinessUnit
                                        .Parameters.Add(New SqlParameter("PARM3", SqlDbType.Char, 50)).Value = company

                                        dtrDatabase = .ExecuteReader()
                                    End With
                                    If dtrDatabase.HasRows() Then
                                        dtrDatabase.Read()
                                        DatabaseType1 = dtrDatabase("DATABASE_TYPE1").ToString
                                    Else
                                        dtrDatabase.Close()
                                        '-----------------
                                        ' Try *ALL Partner
                                        '-----------------    
                                        cmdSelect = New SqlCommand(SQLString4, conTalent)
                                        With cmdSelect
                                            .Parameters.Add(New SqlParameter(Parm1, SqlDbType.Char, 20))
                                            .Parameters(Parm1).Value = WebServiceDestinationDatabase
                                            .Parameters.Add(New SqlParameter("PARM2", SqlDbType.Char, 50)).Value = BusinessUnit
                                            .Parameters.Add(New SqlParameter("PARM3", SqlDbType.Char, 50)).Value = "*ALL"

                                            dtrDatabase = .ExecuteReader()
                                        End With
                                        If dtrDatabase.HasRows() Then
                                            dtrDatabase.Read()
                                            DatabaseType1 = dtrDatabase("DATABASE_TYPE1").ToString
                                        Else
                                            dtrDatabase.Close()
                                            '------------
                                            ' Try *ALL BU
                                            '------------
                                            cmdSelect = New SqlCommand(SQLString4, conTalent)
                                            With cmdSelect
                                                .Parameters.Add(New SqlParameter(Parm1, SqlDbType.Char, 20))
                                                .Parameters(Parm1).Value = WebServiceDestinationDatabase
                                                .Parameters.Add(New SqlParameter("PARM2", SqlDbType.Char, 50)).Value = "*ALL"
                                                .Parameters.Add(New SqlParameter("PARM3", SqlDbType.Char, 50)).Value = "*ALL"

                                                dtrDatabase = .ExecuteReader()
                                            End With
                                            If dtrDatabase.HasRows() Then
                                                dtrDatabase.Read()
                                                DatabaseType1 = dtrDatabase("DATABASE_TYPE1").ToString
                                            End If
                                        End If
                                    End If
                                    dtrDatabase.Close()
                                End If
                                End If
                        End If

                    Catch ex As Exception
                        Const strError5 As String = "Error during profile database access"
                        '-------------------------------------------------
                        ' Temp logging to investigate problem at Westcoast
                        '-------------------------------------------------
                        Dim sb As New StringBuilder
                        With sb
                            .Append("Profile3.vb").Append(Environment.NewLine)
                            .Append("         ")
                            .Append(" Error:").Append(ex.Message)
                        End With
                        With err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError5
                            .ErrorNumber = "TTPPROF-07"
                            .HasError = True
                        End With
                    End Try
                    '----------------------------------------------------------------------------------
                    '   Close
                    '
                    Try
                        conTalent.Close()
                    Catch ex As Exception
                        Const strError6 As String = "Failed to close profile database connection"
                        With err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError6
                            .ErrorNumber = "TTPPROF-08"
                            .HasError = True
                        End With
                    End Try

                Catch ex As Exception
                    Const strError1 As String = "Could not establish connection to the database"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError1
                        .ErrorNumber = "TTPPROF-09"
                        .HasError = True
                    End With
                End Try

                '-----------------------
                ' Check if need to retry
                '-----------------------
                retryCount += 1
                retry = False
                If err.HasError = True AndAlso retryCount = 1 Then
                    retry = True
                    err.HasError = False
                End If
            End While

            Return err
        End Function

    End Class

End Namespace