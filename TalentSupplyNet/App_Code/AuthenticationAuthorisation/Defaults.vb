Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Defaults Class
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'  
'       Error Number Code base      TTPDEFA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class Defaults

        Private _accountNo1 As String = String.Empty
        Private _accountNo2 As String = String.Empty
        Private _accountNo3 As String = String.Empty
        Private _accountNo4 As String = String.Empty
        Private _accountNo5 As String = String.Empty
        Private _cacheing As Boolean = False
        Private _cacheTimeMinutes As Integer
        Private _company As String = String.Empty
        Private _defaultCurrentVersion As String = "1.0"
        Private _destinationDatabase As String = String.Empty
        Private _databaseType1 As String = String.Empty
        Private _emailFrom As String = String.Empty
        Private _storeXml As Boolean = False
        Private _transactionNumber As Integer = 0
        Private _webServiceName As String = String.Empty
        Private _writeLog As Boolean = False
        Private _xmlRequestDocPathU As String = String.Empty
        Private _xmlRequestDocPathT As String = String.Empty
        Private _xmlResponseDocPathU As String = String.Empty
        Private _xmlResponseDocPathT As String = String.Empty
        Private _xsd As String = String.Empty
        Private _retryFailures As Boolean = False
        Private _retryAttempts As Integer = 0
        Private _retryWaitTime As Int32 = 0
        Private _retryErrorNumbers As String = String.Empty
        Private _retryGetTransactionNumber As String = "0"
        Private _businessUnit As String = String.Empty

        Public Property AccountNo1() As String
            Get
                Return _accountNo1
            End Get
            Set(ByVal value As String)
                _accountNo1 = value
            End Set
        End Property
        Public Property AccountNo2() As String
            Get
                Return _accountNo2
            End Get
            Set(ByVal value As String)
                _accountNo2 = value
            End Set
        End Property
        Public Property AccountNo3() As String
            Get
                Return _accountNo3
            End Get
            Set(ByVal value As String)
                _accountNo3 = value
            End Set
        End Property
        Public Property AccountNo4() As String
            Get
                Return _accountNo4
            End Get
            Set(ByVal value As String)
                _accountNo4 = value
            End Set
        End Property
        Public Property AccountNo5() As String
            Get
                Return _accountNo5
            End Get
            Set(ByVal value As String)
                _accountNo5 = value
            End Set
        End Property
        Public Property Cacheing() As Boolean
            Get
                Return _cacheing
            End Get
            Set(ByVal value As Boolean)
                _cacheing = value
            End Set
        End Property
        Public Property CacheTimeMinutes() As Integer
            Get
                Return _cacheTimeMinutes
            End Get
            Set(ByVal value As Integer)
                _cacheTimeMinutes = value
            End Set
        End Property
        Public Property Company() As String
            Get
                Return _company
            End Get
            Set(ByVal value As String)
                _company = value
            End Set
        End Property
        Public Property DefaultCurrentVersion() As String
            Get
                Return _defaultCurrentVersion
            End Get
            Set(ByVal value As String)
                _defaultCurrentVersion = value
            End Set
        End Property
        Public Property DestinationDatabase() As String
            Get
                Return _destinationDatabase
            End Get
            Set(ByVal value As String)
                _destinationDatabase = value
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
        Public Property EmailFrom() As String
            Get
                Return _emailFrom
            End Get
            Set(ByVal value As String)
                _emailFrom = value
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
        Public Property TransactionNumber() As Integer
            Get
                Return _transactionNumber
            End Get
            Set(ByVal value As Integer)
                _transactionNumber = value
            End Set
        End Property
        Public Property WebServiceName() As String
            Get
                Return _webServiceName
            End Get
            Set(ByVal value As String)
                _webServiceName = value
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
        Public Property XmlRequestDocPathU() As String
            Get
                Return _xmlRequestDocPathU
            End Get
            Set(ByVal value As String)
                _xmlRequestDocPathU = value
            End Set
        End Property
        Public Property XmlRequestDocPathT() As String
            Get
                Return _xmlRequestDocPathT
            End Get
            Set(ByVal value As String)
                _xmlRequestDocPathT = value
            End Set
        End Property
        Public Property XmlResponseDocPathU() As String
            Get
                Return _xmlResponseDocPathU
            End Get
            Set(ByVal value As String)
                _xmlResponseDocPathU = value
            End Set
        End Property
        Public Property XmlResponseDocPathT() As String
            Get
                Return _xmlResponseDocPathT
            End Get
            Set(ByVal value As String)
                _xmlResponseDocPathT = value
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
        Public Property RetryFailures() As String
            Get
                Return _retryFailures
            End Get
            Set(ByVal value As String)
                _retryFailures = value
            End Set
        End Property
        Public Property RetryAttempts() As String
            Get
                Return _retryAttempts
            End Get
            Set(ByVal value As String)
                _retryAttempts = value
            End Set
        End Property
        Public Property RetryWaitTime() As String
            Get
                Return _retryWaitTime
            End Get
            Set(ByVal value As String)
                _retryWaitTime = value
            End Set
        End Property
        Public Property RetryErrorNumbers() As String
            Get
                Return _retryErrorNumbers
            End Get
            Set(ByVal value As String)
                _retryErrorNumbers = value
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

        Private _repriceBlankPrice As Boolean = False
        Public Property RepriceBlankPrice() As Boolean
            Get
                Return _repriceBlankPrice
            End Get
            Set(ByVal value As Boolean)
                _repriceBlankPrice = value
            End Set
        End Property

        Private _orderCheckForAltProducts As Boolean = False
        Public Property OrderCheckForAltProducts() As Boolean
            Get
                Return _orderCheckForAltProducts
            End Get
            Set(ByVal value As Boolean)
                _orderCheckForAltProducts = value
            End Set
        End Property


        Public Function GetDefaults() As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------------------
            '   Open connection to DB
            '
            Const bSlash As String = "\"
            Dim ReqPathStr As String = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings("TxtLogPath") & _
                                    bSlash & ConfigurationManager.AppSettings("XMLLogFolder") & _
                                    bSlash & ConfigurationManager.AppSettings("RequestsFolder"))
            Dim ResPathStr As String = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings("TxtLogPath") & _
                                    bSlash & ConfigurationManager.AppSettings("XMLLogFolder") & _
                                    bSlash & ConfigurationManager.AppSettings("ResponsesFolder"))
            Try
                Dim conTalent As SqlConnection = Nothing
                Const SqlServer2005 As String = "SqlServer2005"
                conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                conTalent.Open()            '
                '   
                Try
                    Dim cmdSelect As SqlCommand = Nothing
                    Dim dtrDefaults As SqlDataReader = Nothing
                    Dim dtrDatabase As SqlDataReader = Nothing
                    '----------------------------------------------------------------------------------
                    '   Get global defaults
                    ' 
                    ''Begin SA1 - DB Restructure
                    ''Const strSelect1 As String = "SELECT * FROM TBL_DEFAULTS "
                    Const Parm1 As String = "@Parm1"
                    Const strSelect1 As String = "SELECT * FROM tbl_supplynet_defaults WHERE BUSINESS_UNIT = @Parm1"
                    ''End SA1 - DB Restructure
                    cmdSelect = New SqlCommand(strSelect1, conTalent)
                    ''Begin SA1 - DB Restructure
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Parm1, SqlDbType.Char, 50))
                        '  .Parameters(Parm1).Value = ConfigurationManager.AppSettings("DefaultBusinessUnit")
                        .Parameters(Parm1).Value = BusinessUnit
                    End With
                    ''End SA1 - DB Restructure
                    dtrDefaults = cmdSelect.ExecuteReader()
                    If Not dtrDefaults.HasRows Then
                        Const strError2 As String = "No defaults found"
                        With err
                            .ErrorMessage = String.Empty
                            .ErrorStatus = strError2
                            .ErrorNumber = "TTPPROF-01"
                            .HasError = True
                        End With
                        dtrDefaults.Close()
                    Else
                        dtrDefaults.Read()
                        Cacheing = dtrDefaults("CACHEING_ENABLED")
                        '------------------------------------------------------------------------------------------
                        '   LOGGING & XML STORE MOD
                        ' 
                        WriteLog = dtrDefaults("LOGGING_ENABLED")
                        StoreXml = dtrDefaults("STORE_XML")
                        EmailFrom = dtrDefaults("EMAIL_FROM") & String.Empty
                        RepriceBlankPrice = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtrDefaults("REPRICE_BLANK_PRICE"))
                        OrderCheckForAltProducts = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtrDefaults("ORDER_CHECK_FOR_ALT_PRODUCTS"))

                        Dim strWork2 As String = TransactionNumber.ToString("0000000000") & ".xml"
                        '
                        XmlRequestDocPathU = ReqPathStr & "\U" & strWork2
                        XmlRequestDocPathT = ReqPathStr & "\T" & strWork2
                        XmlResponseDocPathU = ResPathStr & "\U" & strWork2
                        XmlResponseDocPathT = ResPathStr & "\T" & strWork2
                        dtrDefaults.Close()
                        '------------------------------------------------------------------------------------------
                        '   Get web service defaults
                        '
                        Const P1 As String = "@Parm1"
                        Const Parm2 As String = "@Parm2"
                        ''Begin SA1 - DB Restructure
                        ''Const SQLString2 As String = "SELECT * FROM TBL_WEB_SERVICE WHERE WEB_SERVICE_NAME = @Parm1"
                        Const SQLString2 As String = "SELECT * FROM tbl_supplynet_module_defaults WHERE MODULE = @Parm1 AND BUSINESS_UNIT = @Parm2"
                        ''End SA1 - DB Restructure
                        cmdSelect = New SqlCommand(SQLString2, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(P1, SqlDbType.Char, 50))
                            .Parameters(P1).Value = WebServiceName().Trim
                            ''Begin SA1 - DB Restructure
                            .Parameters.Add(New SqlParameter(Parm2, SqlDbType.Char, 50))
                            '.Parameters(Parm2).Value = ConfigurationManager.AppSettings("DefaultBusinessUnit")
                            .Parameters(Parm2).Value = BusinessUnit
                            ''End SA1 - DB Restructure
                            dtrDefaults = .ExecuteReader()
                        End With
                        If Not dtrDefaults.HasRows Then
                            Const strError4 As String = "No defaults found for web service "
                            With err
                                .ErrorMessage = String.Empty
                                .ErrorStatus = strError4 & WebServiceName()
                                .ErrorNumber = "TTPPROF-02"
                                .HasError = True
                            End With
                            dtrDefaults.Close()
                        Else
                            '------------------------------------------------------------------------------------------
                            dtrDefaults.Read()
                            Dim tempRequestCurrentVersion As String = dtrDefaults("REQUEST_CURRENT_VERSION").ToString.Trim()
                            If tempRequestCurrentVersion Is Nothing Or tempRequestCurrentVersion.Equals(String.Empty) Then
                                DefaultCurrentVersion = tempRequestCurrentVersion
                            End If
                            If Xsd.Equals(String.Empty) Or Xsd() Is Nothing Then
                                Xsd = dtrDefaults("XML_SCHEMA_DOCUMENT").ToString.Trim()
                            End If
                            'Only allow cacheing on Enquiry web services
                            ''Begin SA1 - DB Restructure
                            ''If dtrDefaults("WEB_SERVICE_TYPE").ToString.Trim().Equals("UPDATE") Then
                            If dtrDefaults("MODULE_TYPE").ToString.Trim().Equals("UPDATE") Then
                                ''End SA1 - DB Restructure
                                Cacheing = False
                            End If
                            '------------------------------------------------------------------------------------------
                            '   LOGGING & XML STORE MOD
                            ' 
                            If Not Convert.ToBoolean(dtrDefaults("LOGGING_ENABLED")) Then
                                WriteLog = Convert.ToBoolean(dtrDefaults("LOGGING_ENABLED"))
                            End If
                            If Not Convert.ToBoolean(dtrDefaults("STORE_XML")) Then
                                StoreXml = Convert.ToBoolean(dtrDefaults("STORE_XML"))
                            End If
                            '------------------------------------------------------------------------------------------
                            '   RETRY (database connection)
                            ' 
                            Try
                                If Convert.ToBoolean(dtrDefaults("RETRY_FAILURES")) Then
                                    RetryFailures = Convert.ToBoolean(dtrDefaults("RETRY_FAILURES"))
                                End If
                                If Convert.ToInt16(dtrDefaults("RETRY_ATTEMPTS")) Then
                                    RetryAttempts = dtrDefaults("RETRY_ATTEMPTS")
                                End If
                                If Convert.ToInt32(dtrDefaults("RETRY_WAIT_TIME")) Then
                                    RetryWaitTime = dtrDefaults("RETRY_WAIT_TIME")
                                End If
                                If Not String.IsNullOrEmpty(dtrDefaults("RETRY_ERROR_NUMBERS")) Then
                                    RetryErrorNumbers = dtrDefaults("RETRY_ERROR_NUMBERS").ToString.Trim
                                End If
                            Catch ex As Exception
                                ' Must have been a Null in there so use defaults 
                            End Try
                            '
                            dtrDefaults.Close()
                            '------------------------------------------------------------------------------------------
                            '   Get company defaults
                            ' 
                            ''Begin SA1 - DB Restructure
                            ''Const SQLString3 As String = "SELECT * FROM TBL_COMPANY WHERE COMPANY = @Parm1"
                            Const SQLString3 As String = "SELECT * FROM TBL_PARTNER WHERE PARTNER = @Parm1"
                            ''End SA1 - DB Restructure
                            Dim cmdSelect2 As SqlCommand
                            cmdSelect2 = New SqlCommand(SQLString3, conTalent)
                            With cmdSelect2
                                ''Begin SA1 - DB Restructure
                                ''.Parameters.Add(New SqlParameter(Parm1, SqlDbType.Char, 20))
                                ''.Parameters(Parm1).Value = Company().Trim
                                .Parameters.Add(New SqlParameter(Parm1, SqlDbType.Char, 50))
                                .Parameters(Parm1).Value = Company().Trim
                                ''End SA1 - DB Restructure
                                dtrDefaults = .ExecuteReader()
                            End With
                            If Not dtrDefaults.HasRows Then
                                Const strError5 As String = "No defaults found for web service "
                                With err
                                    .ErrorMessage = String.Empty
                                    .ErrorStatus = strError5 & WebServiceName()
                                    .ErrorNumber = "TTPPROF-03"
                                    .HasError = True
                                End With
                                dtrDefaults.Close()
                            Else
                                dtrDefaults.Read()
                                If DestinationDatabase().Equals(String.Empty) Or DestinationDatabase() Is Nothing Then
                                    DestinationDatabase = dtrDefaults("DESTINATION_DATABASE").ToString.Trim()
                                End If
                                If Cacheing() Then
                                    Cacheing = CType(dtrDefaults("CACHEING_ENABLED"), Boolean)
                                End If
                                If CacheTimeMinutes = 0 Then
                                    CacheTimeMinutes = dtrDefaults("CACHE_TIME_MINUTES").ToString.Trim()
                                End If

                                AccountNo1 = dtrDefaults("ACCOUNT_NO_1") & String.Empty
                                AccountNo2 = dtrDefaults("ACCOUNT_NO_2") & String.Empty
                                AccountNo3 = dtrDefaults("ACCOUNT_NO_3") & String.Empty
                                AccountNo4 = dtrDefaults("ACCOUNT_NO_4") & String.Empty
                                AccountNo5 = dtrDefaults("ACCOUNT_NO_5") & String.Empty
                                '------------------------------------------------------------------------------------------
                                '   LOGGING & XML STORE MOD
                                ' 
                                If Not Convert.ToBoolean(dtrDefaults("LOGGING_ENABLED")) Then
                                    WriteLog = Convert.ToBoolean(dtrDefaults("LOGGING_ENABLED"))
                                End If
                                If Not Convert.ToBoolean(dtrDefaults("STORE_XML")) Then
                                    StoreXml = Convert.ToBoolean(dtrDefaults("STORE_XML"))
                                End If
                                '
                                dtrDefaults.Close()
                                '------------------------------------------------------------------------------------------
                                '   Get Database defaults
                                ' 
                                If DatabaseType1 <> String.Empty Then
                                    Const SQLString4 As String = "SELECT * FROM TBL_DATABASE_VERSION " & _
                                                                " WHERE DESTINATION_DATABASE = @Parm1 AND " & _
                                                                "       BUSINESS_UNIT = @Parm2 AND " & _
                                                                "       PARTNER = @Parm3 "

                                    cmdSelect = New SqlCommand(SQLString4, conTalent)
                                    With cmdSelect
                                        .Parameters.Add(New SqlParameter(Parm1, SqlDbType.Char, 20))
                                        .Parameters(Parm1).Value = DestinationDatabase
                                        .Parameters.Add(New SqlParameter("PARM2", SqlDbType.Char, 50)).Value = BusinessUnit
                                        .Parameters.Add(New SqlParameter("PARM3", SqlDbType.Char, 50)).Value = Company().Trim

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
                                            .Parameters(Parm1).Value = DestinationDatabase
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
                                                .Parameters(Parm1).Value = DestinationDatabase
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
                    End If

                Catch ex As Exception
                    Const strError5 As String = "Error during profile database access"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError5
                        .ErrorNumber = "TTPDEFA-01"
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
                        .ErrorNumber = "TTPDEFA-02"
                        .HasError = True
                    End With
                End Try

            Catch ex As Exception
                Const strError1 As String = "Could not establish connection to the database"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError1
                    .ErrorNumber = "TTPDEFA-03"
                    .HasError = True
                End With
                '--------------------------------------------------------------------------------------
                '   If we cannot connect to the database set the doc paths as the current time and date
                '
                Dim strWork1 As String = Now.ToString("yyyy-MM-dd-HH-mm") & ".xml"
                '
                XmlRequestDocPathU = ReqPathStr & "\U" & strWork1
                XmlRequestDocPathT = ReqPathStr & "\T" & strWork1
                XmlResponseDocPathU = ResPathStr & "\U" & strWork1
                XmlResponseDocPathT = ResPathStr & "\T" & strWork1
            End Try
            Return err
        End Function
        Public Function GetTransactionNumber() As ErrorObj
            Dim err As New ErrorObj
            Dim conTalent As SqlConnection = Nothing
            '--------------------------------------------------------------------
            Try
                Const SqlServer2005 As String = "SqlServer2005"
                conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                conTalent.Open()
            Catch ex As Exception
                Const strError1 As String = "Could not establish connection to the database"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError1
                    .ErrorNumber = "TTPDEFA-04"
                    .HasError = True
                End With
            End Try

            Dim dtrDefaults As SqlDataReader = Nothing
            '---------------------------------------------------------------------
            If Not err.HasError Then
                Try
                    '----------------------------------------------------------------------------------
                    '   Get global defaults
                    ' 
                    ''Begin SA1 - DB Restructure
                    ''Const strSelect As String = "SELECT TRANSACTION_NUMBER FROM TBL_DEFAULTS"
                    Const strSelect As String = "SELECT TRANSACTION_NUMBER FROM tbl_supplynet_defaults WHERE BUSINESS_UNIT = @Parm1"
                    Const Parm1 As String = "@Parm1"
                    ''End SA1 - DB Restructure
                    Dim cmdSelect As SqlCommand = New SqlCommand(strSelect, conTalent)
                    ''Begin SA1 - DB Restructure
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Parm1, SqlDbType.Char, 50))
                        .Parameters(Parm1).Value = BusinessUnit
                        '.Parameters(Parm1).Value = ConfigurationManager.AppSettings("DefaultBusinessUnit")
                    End With
                    ''End SA1 - DB Restructure
                    dtrDefaults = cmdSelect.ExecuteReader()
                    If Not dtrDefaults.HasRows Then
                        Const strError2 As String = "No defaults found"
                        '-------------------------------------------------
                        ' Temp logging to investigate problem at Westcoast
                        '-------------------------------------------------
                        Dim sb As New StringBuilder
                        With sb
                            .Append("Defaults.vb - No defaults found").Append(Environment.NewLine)
                            .Append("         ")
                            .Append(" Business Unit:").Append(BusinessUnit)
                            '.Append(" Business Unit:").Append(ConfigurationManager.AppSettings("DefaultBusinessUnit"))
                            .Append(Environment.NewLine)
                            .Append("         ")
                            .Append(" Contalent State:" & conTalent.State.ToString).Append(" Retry: ").Append(_retryGetTransactionNumber)
                            .Append("         ")
                            .Append(" Contalent State:" & conTalent.State.ToString).Append(" TransNo: ").Append(TransactionNumber)
                        End With
                        LogWriter.WriteToLog(sb.ToString)
                        ' Do 1 recursive retry..
                        If _retryGetTransactionNumber = "0" Then
                            _retryGetTransactionNumber = "1"
                        Else
                            _retryGetTransactionNumber = "2"
                        End If
                        With err
                            .ErrorMessage = String.Empty
                            .ErrorStatus = strError2
                            .ErrorNumber = "TTPDEFA-05"
                            .HasError = True
                        End With
                        dtrDefaults.Close()
                        cmdSelect = Nothing
                    Else
                        _retryGetTransactionNumber = "2"
                        dtrDefaults.Read()
                        TransactionNumber = Convert.ToInt32(dtrDefaults("TRANSACTION_NUMBER"))
                        '-------------------------------------------------
                        ' Temp logging to investigate problem at Westcoast
                        '-------------------------------------------------
                        Dim sb As New StringBuilder
                        With sb
                            .Append("Defaults.vb - Setting Trans Number").Append(Environment.NewLine)
                            .Append("         ")
                            '.Append(" Business Unit:").Append(ConfigurationManager.AppSettings("DefaultBusinessUnit"))
                            .Append(" Business Unit:").Append(BusinessUnit)
                            .Append(Environment.NewLine)
                            .Append("         ")
                            .Append(" Contalent State:" & conTalent.State.ToString).Append(" Retry: ").Append(_retryGetTransactionNumber)
                            .Append("         ")
                            .Append(" Contalent State:" & conTalent.State.ToString).Append(" TransNo: ").Append(TransactionNumber)
                        End With
                        LogWriter.WriteToLog(sb.ToString)
                        dtrDefaults.Close()
                        ''Begin SA1 - DB Restructure
                        Dim strUpdate As String = _
                                    " UPDATE tbl_supplynet_defaults SET TRANSACTION_NUMBER = " & (TransactionNumber + 1) & _
                                    " WHERE TRANSACTION_NUMBER = " & TransactionNumber & _
                                    " AND BUSINESS_UNIT = @Parm1"
                        ''            Dim strUpdate As String = _
                        ''" UPDATE tbl_defaults SET TRANSACTION_NUMBER = " & (TransactionNumber + 1) & _
                        ''" WHERE TRANSACTION_NUMBER = " & TransactionNumber
                        ''End SA1 - DB Restructure
                        Dim cmdUpdate As SqlCommand = New SqlCommand(strUpdate, conTalent)
                        ''Begin SA1 - DB Restructure
                        With cmdUpdate
                            .Parameters.Add(New SqlParameter(Parm1, SqlDbType.Char, 50))
                            '  .Parameters(Parm1).Value = ConfigurationManager.AppSettings("DefaultBusinessUnit")
                            .Parameters(Parm1).Value = BusinessUnit
                        End With
                        ''End SA1 - DB Restructure
                        cmdUpdate.ExecuteNonQuery()
                        cmdUpdate = Nothing
                    End If
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    '-------------------------------------------------
                    ' Temp logging to investigate problem at Westcoast
                    '-------------------------------------------------
                    Dim sb As New StringBuilder
                    With sb
                        .Append("Defaults2.vb - Error during DB Access").Append(Environment.NewLine)
                        .Append("         ")
                        '   .Append(" Business Unit:").Append(ConfigurationManager.AppSettings("DefaultBusinessUnit"))
                        .Append(" Business Unit:").Append(BusinessUnit)
                        .Append(Environment.NewLine)
                        .Append("         ")
                        .Append(" Contalent State:" & conTalent.State.ToString).Append(" Retry: ").Append(_retryGetTransactionNumber)
                        .Append("         ")
                        .Append(" Error:" & ex.Message).Append(" TransNo: ").Append(TransactionNumber)
                        .Append(Environment.NewLine)
                        If Not dtrDefaults Is Nothing Then
                            For i As Integer = 0 To (dtrDefaults.FieldCount - 1)
                                .Append(dtrDefaults.GetName(i) & ",")
                            Next
                        End If

                    End With
                    LogWriter.WriteToLog(sb.ToString)

                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TTPDEFA-06"
                        .HasError = True
                    End With
                End Try
            End If
            '--------------------------------------------------------------------
            '   Close
            '
            Try
                conTalent.Close()
                conTalent.Dispose()
            Catch ex As Exception
                Const strError9 As String = "Failed to close database connection"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError9
                    .ErrorNumber = "TTPDEFA-07"
                    .HasError = True
                End With
            End Try
            '---------------------
            ' Check if do 1 retry
            '
            If _retryGetTransactionNumber = "1" Then
                err = GetTransactionNumber()
            End If

            Return err
        End Function

    End Class

End Namespace