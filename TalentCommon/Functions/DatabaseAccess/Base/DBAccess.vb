Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries

<Serializable()> _
Public Class DBAccess
    Private Const CLASSNAME As String = "DBAccess"
    Protected conSystem21 As iDB2Connection = Nothing
    Protected conChorus As iDB2Connection = Nothing
    Protected conSql2005 As SqlConnection = Nothing
    Protected conTALENTQUEUE As SqlConnection = Nothing
    Protected conTALENTCRM As iDB2Connection = Nothing
    Protected conTALENTTKT As iDB2Connection = Nothing
    Protected conADL As iDB2Connection = Nothing
    Protected Const SYSTEM21 As String = "SYSTEM21"
    Protected Const CHORUS As String = "CHORUS"
    Protected Const SQL2005 As String = "SQL2005"
    Protected Const TALENTCRM As String = "TALENTCRM"
    Protected Const TALENTTKT As String = "TALENTTKT"
    Protected Const TALENTQUEUE As String = "TALENTQUEUE"
    Protected Const DEV As String = "DEV"
    Protected Const ADL As String = "ADL"
    Protected Const TALENTADMIN As String = "TalentAdmin"
    Protected Const TALENTDEFINITIONS As String = "TalentDefinitions"
    Protected Const URLREQUEST As String = "URLREQUEST"
    Protected Const T65535 As String = "65535"
    Protected Const T285 As String = "285"
    Protected _resultDataSet As DataSet
    Protected _settings As New DESettings
    Protected _resetConnection As Boolean = False
    Protected Const Param0 As String = "@Param0"
    Protected Const Param1 As String = "@Param1"
    Protected Const Param2 As String = "@Param2"
    Protected Const Param3 As String = "@Param3"
    Protected Const Param4 As String = "@Param4"
    Protected Const Param5 As String = "@Param5"
    Protected Const Param6 As String = "@Param6"
    Protected Const Param7 As String = "@Param7"
    Protected Const Param8 As String = "@Param8"
    Protected Const Param9 As String = "@Param9"
    Protected Const Param10 As String = "@Param10"
    Protected Const Param11 As String = "@Param11"
    Protected Const Param12 As String = "@Param12"
    Protected Const Param13 As String = "@Param13"
    Protected Const Param14 As String = "@Param14"
    Protected Const Param15 As String = "@Param15"
    Protected Const Param16 As String = "@Param16"
    Protected Const Param17 As String = "@Param17"
    Protected Const Param18 As String = "@Param18"
    Protected Const Param19 As String = "@Param19"
    Protected Const Param20 As String = "@Param20"
    Protected Const Source As String = "@Source"
    Protected Const ErrorCode As String = "@Error"
    Protected Const RecordCount As String = "@RecordCount"

    Private _talentLogger As TalentLogging = Nothing
    Public Property ResetConnection() As Boolean
        Get
            Return _resetConnection
        End Get
        Set(ByVal value As Boolean)
            _resetConnection = value
        End Set
    End Property

    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property

    Public Property Settings() As DESettings
        Get
            Return _settings
        End Get
        Set(ByVal value As DESettings)
            _settings = value
        End Set
    End Property

    Protected ReadOnly Property TalentLogger() As TalentLogging
        Get
            If _talentLogger Is Nothing Then
                _talentLogger = New TalentLogging
                _talentLogger.FrontEndConnectionString = Settings.FrontEndConnectionString
            End If
            Return _talentLogger
        End Get
    End Property


    Protected Function Sql2005Open() As ErrorObj
        Dim err As New ErrorObj
        Dim iCounter As Int16 = 0
        '---------------------------------------------------------------------------------
        '   Attempt to open database
        '
        Const strError As String = "Could not establish connection to the SQL database"
        Try
            While iCounter < 10
                If conSql2005 Is Nothing Then
                    conSql2005 = New SqlConnection(Settings.FrontEndConnectionString)
                    conSql2005.Open()
                ElseIf conSql2005.State <> ConnectionState.Open Then
                    conSql2005 = New SqlConnection(Settings.FrontEndConnectionString)
                    conSql2005.Open()
                End If
                '---------------------------------------------------------------------------------
                '   Possible the server needs to wake up so 
                '
                If conSql2005.State = ConnectionState.Open Then
                    Exit While
                Else
                    iCounter += 1
                    If iCounter > 10 Then
                        With err
                            .ErrorMessage = conSql2005.State
                            .ErrorStatus = strError
                            .ErrorNumber = "TACDBAC-P1a"
                            .HasError = True
                        End With
                        Exit While
                    End If
                    System.Threading.Thread.Sleep(2500)             ' Sleep for 2.5 seconds
                End If
                '---------------------------------------------------------------------------------
            End While
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAC-P1b"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "Sql2005Open", strError, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function Sql2005Close() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------
        '   Warning :   Using ErrorObj here when closing the DB does cause a   
        '               problem as it will clear the err object when an actual
        '               error has occured elsewhere   
        '------------------------------------------------------------------------
        Try
            If Not (conSql2005 Is Nothing) Then
                If (conSql2005.State = ConnectionState.Open) Then
                    conSql2005.Close()
                End If
            End If
        Catch ex As Exception
            Const strError4 As String = "Failed to close the Sql2005 database connection"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError4
                .ErrorNumber = "TACDBAC-P2"
                .HasError = True
                Return err
            End With
            TalentLogger.Logging(CLASSNAME, "Sql2005Close", strError4, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function TalentAdminOpen() As ErrorObj
        Dim err As New ErrorObj
        Dim iCounter As Int16 = 0
        Const strError As String = "Could not establish connection to the SQL database"
        Try
            While iCounter < 10
                If conSql2005 Is Nothing Then
                    conSql2005 = New SqlConnection(Settings.BackOfficeConnectionString)
                    conSql2005.Open()
                ElseIf conSql2005.State <> ConnectionState.Open Then
                    conSql2005 = New SqlConnection(Settings.BackOfficeConnectionString)
                    conSql2005.Open()
                End If
                If conSql2005.State = ConnectionState.Open Then
                    Exit While
                Else
                    iCounter += 1
                    If iCounter > 10 Then
                        With err
                            .ErrorMessage = conSql2005.State
                            .ErrorStatus = strError
                            .ErrorNumber = "TACDBAC-P9a"
                            .HasError = True
                        End With
                        Exit While
                    End If
                    System.Threading.Thread.Sleep(2500)
                End If
            End While
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAC-P9b"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "TalentAdminOpen", strError, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function TalentAdminClose() As ErrorObj
        Dim err As New ErrorObj
        Try
            If Not (conSql2005 Is Nothing) Then
                If (conSql2005.State = ConnectionState.Open) Then
                    conSql2005.Close()
                End If
            End If
        Catch ex As Exception
            Const strError4 As String = "Failed to close the Sql2005 database connection"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError4
                .ErrorNumber = "TACDBAC-P9c"
                .HasError = True
                Return err
            End With
            TalentLogger.Logging(CLASSNAME, "TalentAdminClose", strError4, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function TalentDefinitionsOpen() As ErrorObj
        Dim err As New ErrorObj
        Dim iCounter As Int16 = 0
        Const strError As String = "Could not establish connection to the SQL database"
        Try
            While iCounter < 10
                If conSql2005 Is Nothing Then
                    conSql2005 = New SqlConnection(Settings.BackOfficeConnectionString)
                    conSql2005.Open()
                ElseIf conSql2005.State <> ConnectionState.Open Then
                    conSql2005 = New SqlConnection(Settings.BackOfficeConnectionString)
                    conSql2005.Open()
                End If
                If conSql2005.State = ConnectionState.Open Then
                    Exit While
                Else
                    iCounter += 1
                    If iCounter > 10 Then
                        With err
                            .ErrorMessage = conSql2005.State
                            .ErrorStatus = strError
                            .ErrorNumber = "TACDBAC-P11a"
                            .HasError = True
                        End With
                        Exit While
                    End If
                    System.Threading.Thread.Sleep(2500)
                End If
            End While
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAC-P11b"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "TalentDefinitionsOpen", strError, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function


    Protected Function TalentDefinitionsClose() As ErrorObj
        Dim err As New ErrorObj
        Try
            If Not (conSql2005 Is Nothing) Then
                If (conSql2005.State = ConnectionState.Open) Then
                    conSql2005.Close()
                End If
            End If
        Catch ex As Exception
            Const strError4 As String = "Failed to close the Sql2005 database connection"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError4
                .ErrorNumber = "TACDBAC-P11c"
                .HasError = True
                Return err
            End With
            TalentLogger.Logging(CLASSNAME, "TalentDefinitionsClose", strError4, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function TalentQueueOpen() As ErrorObj
        Dim err As New ErrorObj
        Dim iCounter As Int16 = 0
        Const strError As String = "Could not establish connection to the SQL database"
        Try
            While iCounter < 10
                If conTALENTQUEUE Is Nothing Then
                    conTALENTQUEUE = New SqlConnection(Settings.BackOfficeConnectionString)
                    conTALENTQUEUE.Open()
                ElseIf conTALENTQUEUE.State <> ConnectionState.Open Then
                    conTALENTQUEUE = New SqlConnection(Settings.BackOfficeConnectionString)
                    conTALENTQUEUE.Open()
                End If
                If conTALENTQUEUE.State = ConnectionState.Open Then
                    Exit While
                Else
                    iCounter += 1
                    If iCounter > 10 Then
                        With err
                            .ErrorMessage = conTALENTQUEUE.State
                            .ErrorStatus = strError
                            .ErrorNumber = "TACDBAC-P10a"
                            .HasError = True
                        End With
                        Exit While
                    End If
                    System.Threading.Thread.Sleep(2500)             ' Sleep for 2.5 seconds
                End If
            End While
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAC-P10b"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "TalentQueueOpen", strError, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function TalentQueueClose() As ErrorObj
        Dim err As New ErrorObj
        Try
            If Not (conTALENTQUEUE Is Nothing) Then
                If (conTALENTQUEUE.State = ConnectionState.Open) Then
                    conTALENTQUEUE.Close()
                End If
            End If
        Catch ex As Exception
            Const strError4 As String = "Failed to close the Sql2005 database connection"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError4
                .ErrorNumber = "TACDBAC-P10c"
                .HasError = True
                Return err
            End With
            TalentLogger.Logging(CLASSNAME, "TalentQueueClose", strError4, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function System21Open() As ErrorObj
        Dim err As New ErrorObj
        Dim iCounter As Int16 = 0
        '---------------------------------------------------------------------------------
        '   Attempt to open database
        '
        Const strError As String = "Could not establish connection to the System21 database"
        Try
            While iCounter < 10
                If conSystem21 Is Nothing Then
                    conSystem21 = New iDB2Connection(Settings.BackOfficeConnectionString)
                    conSystem21.Open()
                ElseIf conSystem21.State <> ConnectionState.Open Then
                    conSystem21 = New iDB2Connection(Settings.BackOfficeConnectionString)
                    conSystem21.Open()
                End If
                '---------------------------------------------------------------------------------
                '   Possible the iSeries needs to wake up so 
                '
                If conSystem21.State = ConnectionState.Open Then
                    Exit While
                Else
                    iCounter += 1
                    If iCounter > 10 Then
                        With err
                            .ErrorMessage = conSystem21.State
                            .ErrorStatus = strError
                            .ErrorNumber = "TACDBAC-P3a"
                            .HasError = True
                        End With
                        Exit While
                    End If
                    System.Threading.Thread.Sleep(2500)             ' Sleep for 2.5 seconds
                End If
                '---------------------------------------------------------------------------------
            End While
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAC-P3b"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "System21Open", strError, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function System21Close() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------
        '   Warning :   Using ErrorObj here when closing the DB does cause a   
        '               problem as it will clear the err object when an actual
        '               error has occured elsewhere   
        '------------------------------------------------------------------------
        Try
            If Not (conSystem21 Is Nothing) Then
                If (conSystem21.State = ConnectionState.Open) Then
                    conSystem21.Close()
                End If
            End If
            conSystem21 = Nothing
        Catch ex As Exception
            Const strError4 As String = "Failed to close the  System21 database connection"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError4
                .ErrorNumber = "TACDBAC-P4"
                .HasError = True
                Return err
            End With
            TalentLogger.Logging(CLASSNAME, "System21Close", strError4, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function TALENTCRMOpen() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------------
        '   Attempt to open database
        '
        Try
            If conTALENTCRM Is Nothing Then
                conTALENTCRM = New iDB2Connection(Settings.BackOfficeConnectionString)
                conTALENTCRM.Open()
            ElseIf conTALENTCRM.State <> ConnectionState.Open Then
                conTALENTCRM = New iDB2Connection(Settings.BackOfficeConnectionString)
                conTALENTCRM.Open()
            End If
        Catch ex As Exception
            Const strError As String = "Could not establish connection to the TALENTCRM database"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAC-P5"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "TALENTCRMOpen", strError, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function TALENTCRMClose() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------
        '   Warning :   Using ErrorObj here when closing the DB does cause a   
        '               problem as it will clear the err object when an actual
        '               error has occured elsewhere   
        '------------------------------------------------------------------------
        Try
            If Not (conTALENTCRM Is Nothing) Then
                If (conTALENTCRM.State = ConnectionState.Open) Then
                    conTALENTCRM.Close()
                End If
            End If
            conTALENTCRM = Nothing
        Catch ex As Exception
            Const strError4 As String = "Failed to close the TALENTCRM database connection"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError4
                .ErrorNumber = "TACDBAC-P6"
                .HasError = True
                Return err
            End With
            TalentLogger.Logging(CLASSNAME, "TALENTCRMClose", strError4, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function TALENTTKTOpen() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------------
        '   Attempt to open database
        '
        Try
            If conTALENTTKT Is Nothing OrElse conTALENTTKT.State <> ConnectionState.Open Then

                ' We want to set a few connection properties to our default values if they are 
                ' present on the connection string
                Dim connectionString As String = Settings.BackOfficeConnectionString()
                If connectionString.Substring(connectionString.Length - 1, 1) <> ";" Then connectionString += ";"
                If connectionString.IndexOf("CheckConnectionOnOpen") < 0 Then connectionString += "CheckConnectionOnOpen=True;"
                If connectionString.IndexOf("ConnectionTimeout") < 0 Then connectionString += "ConnectionTimeout=10;"

                'Open the connection
                Dim loopCounter As Integer = 1
                Do While loopCounter <= 5
                    conTALENTTKT = New iDB2Connection(connectionString)
                    conTALENTTKT.Open()
                    If conTALENTTKT.State = ConnectionState.Open Then
                        Exit Do
                    Else
                        loopCounter += 1
                    End If
                Loop

                If conTALENTTKT.State <> ConnectionState.Open Then
                    Throw New System.Exception("Cannot open connection after " & loopCounter.ToString & " attempts.")
                End If

            End If
        Catch ex As Exception
            Const strError As String = "Could not establish connection to the TALENTTKT database"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAC-P5"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "TALENTTKTOpen", strError, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function


    Protected Function TALENTTKTClose() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------
        '   Warning :   Using ErrorObj here when closing the DB does cause a   
        '               problem as it will clear the err object when an actual
        '               error has occured elsewhere   
        '------------------------------------------------------------------------
        Try
            If Not (conTALENTTKT Is Nothing) Then
                If (conTALENTTKT.State = ConnectionState.Open) Then
                    conTALENTTKT.Close()
                End If
            End If
            conTALENTTKT = Nothing
        Catch ex As Exception
            Const strError4 As String = "Failed to close the TALENTTKT database connection"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError4
                .ErrorNumber = "TACDBAC-P6"
                .HasError = True
                Return err
            End With
            TalentLogger.Logging(CLASSNAME, "TALENTTKTClose", strError4, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function ADLOpen() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------------
        '   Attempt to open database
        '
        Try
            If conADL Is Nothing Then
                conADL = New iDB2Connection(Settings.BackOfficeConnectionString)
                conADL.Open()
            ElseIf conADL.State <> ConnectionState.Open Then
                conADL = New iDB2Connection(Settings.BackOfficeConnectionString)
                conADL.Open()
            End If
        Catch ex As Exception
            Const strError As String = "Could not establish connection to the ADL database"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAC-P7"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "ADLOpen", strError, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function ADLClose() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------
        '   Warning :   Using ErrorObj here when closing the DB does cause a   
        '               problem as it will clear the err object when an actual
        '               error has occured elsewhere   
        '------------------------------------------------------------------------
        Try
            If Not (conADL Is Nothing) Then
                If (conADL.State = ConnectionState.Open) Then
                    conADL.Close()
                End If
            End If
            conADL = Nothing
        Catch ex As Exception
            Const strError4 As String = "Failed to close the ADL database connection"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError4
                .ErrorNumber = "TACDBAC-P8"
                .HasError = True
                Return err
            End With
            TalentLogger.Logging(CLASSNAME, "ADLClose", strError4, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Public Overridable Function AccessDatabase() As ErrorObj
        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay

        Dim err As New ErrorObj
        '------------------------------------------------------------------------
        '   Warning :   Using ErrorObj here when closing the DB does cause a   
        '               problem as it will clear the err object when an actual 
        '               error has occured in the Access Data 
        '------------------------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                err = System21Open()
                If Not err.HasError Then
                    err = AccessDataBaseSystem21()
                    '
                    ' Start Ignore certain errors
                    If err.HasError And InStr(Settings.IgnoreErrors, err.ErrorNumber.Trim) > 0 Then
                        err = Nothing
                    Else
                        '
                        ' Start RETRY
                        '
                        ' If there is an error AND Retry is switched AND RetryAttempts is > 0 AND the
                        ' error number generated is defined to be retried then ...
                        Dim intPos As Integer = 0
                        Dim intCount As Integer = 0
                        If err.HasError And Settings.RetryFailures And Settings.RetryAttempts > 0 Then
                            intPos = InStr(Settings.RetryErrorNumbers, err.ErrorNumber.Trim)
                            If intPos > 0 Then
                                intCount = 1
                                Do While intCount <= Settings.RetryAttempts And err.HasError

                                    If Settings.RetryWaitTime > 0 Then _
                                        System.Threading.Thread.Sleep(Settings.RetryWaitTime)

                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = String.Empty
                                        .ErrorNumber = String.Empty
                                        .HasError = False
                                    End With
                                    _resetConnection = True
                                    err = AccessDataBaseSystem21()
                                    intCount += 1
                                Loop
                            End If
                        End If
                        ' End   RETRY
                        '
                    End If
                    ' End Ignore certain errors
                    '
                End If

                System21Close()
            Case Is = SQL2005
                err = Sql2005Open()
                If Not err.HasError Then err = AccessDataBaseSQL2005()
                Sql2005Close()
            Case Is = TALENTCRM
                err = TALENTCRMOpen()
                If Not err.HasError Then
                    err = AccessDataBaseTALENTCRM()
                    '
                    ' Start Ignore certain errors
                    If err.HasError And InStr(Settings.IgnoreErrors, err.ErrorNumber.Trim) > 0 Then
                        err = Nothing
                    Else
                        '
                        ' Start RETRY
                        '
                        ' If there is an error AND Retry is switched AND RetryAttempts is > 0 AND the
                        ' error number generated is defined to be retried then ...
                        Dim intPos As Integer = 0
                        Dim intCount As Integer = 0
                        If err.HasError And Settings.RetryFailures And Settings.RetryAttempts > 0 Then
                            intPos = InStr(Settings.RetryErrorNumbers, err.ErrorNumber.Trim)
                            If intPos > 0 Then
                                intCount = 1
                                Do While intCount <= Settings.RetryAttempts And err.HasError
                                    If Settings.RetryWaitTime > 0 Then _
                                        System.Threading.Thread.Sleep(Settings.RetryWaitTime)
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = String.Empty
                                        .ErrorNumber = String.Empty
                                        .HasError = False
                                    End With
                                    err = AccessDataBaseTALENTCRM()
                                    intCount += 1
                                Loop
                            End If
                        End If
                        ' End   RETRY
                        '
                    End If
                    ' End Ignore certain errors
                    '
                End If
                    TALENTCRMClose()
            Case Is = TALENTTKT
                    err = TALENTTKTOpen()
                    If Not err.HasError Then
                    err = AccessDataBaseTALENTTKT()
                    '
                    ' Ignore certain errors
                    If err.HasError And InStr(Settings.IgnoreErrors, err.ErrorNumber.Trim) > 0 Then
                        err = Nothing
                    Else
                        '
                        ' Start RETRY
                        '
                        ' If there is an error AND Retry is switched AND RetryAttempts is > 0 AND the
                        ' error number generated is defined to be retried then ...
                        Dim intPos As Integer = 0
                        Dim intCount As Integer = 0
                        If err.HasError And Settings.RetryFailures And Settings.RetryAttempts > 0 Then
                            intPos = InStr(Settings.RetryErrorNumbers, err.ErrorNumber.Trim)
                            If intPos > 0 Then
                                intCount = 1
                                Do While intCount <= Settings.RetryAttempts And err.HasError
                                    If Settings.RetryWaitTime > 0 Then _
                                        System.Threading.Thread.Sleep(Settings.RetryWaitTime)
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = String.Empty
                                        .ErrorNumber = String.Empty
                                        .HasError = False
                                    End With
                                    err = AccessDataBaseTALENTTKT()
                                    intCount += 1
                                Loop
                            End If
                        End If
                        ' End   RETRY
                        '
                    End If
                    ' End Ignore certain errors
                    '
                End If
                    TALENTTKTClose()
            Case Is = URLREQUEST
                    err = AccessDataBaseURL()
            Case Is = ADL
                err = ADLOpen()
                If Not err.HasError Then
                    err = AccessDataBaseADL()
                    '
                    ' Ignore certain errors
                    If err.HasError And InStr(Settings.IgnoreErrors, err.ErrorNumber.Trim) > 0 Then
                        err = Nothing
                    Else
                        '
                        ' Start RETRY
                        '
                        ' If there is an error AND Retry is switched AND RetryAttempts is > 0 AND the
                        ' error number generated is defined to be retried then ...
                        Dim intPos As Integer = 0
                        Dim intCount As Integer = 0
                        If err.HasError And Settings.RetryFailures And Settings.RetryAttempts > 0 Then
                            intPos = InStr(Settings.RetryErrorNumbers, err.ErrorNumber.Trim)
                            If intPos > 0 Then
                                intCount = 1
                                Do While intCount <= Settings.RetryAttempts And err.HasError
                                    If Settings.RetryWaitTime > 0 Then _
                                        System.Threading.Thread.Sleep(Settings.RetryWaitTime)
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = String.Empty
                                        .ErrorNumber = String.Empty
                                        .HasError = False
                                    End With
                                    err = AccessDataBaseADL()
                                    intCount += 1
                                Loop
                            End If
                        End If
                        ' End   RETRY
                        '
                    End If
                    ' End Ignore certain errors
                    '
                End If
                ADLClose()
            Case Is = CHORUS
                err = ChorusOpen()
                If Not err.HasError Then
                    err = AccessDataBaseChorus()
                    '
                    ' Start Ignore certain errors
                    If err.HasError And InStr(Settings.IgnoreErrors, err.ErrorNumber.Trim) > 0 Then
                        err = Nothing
                    Else
                        '
                        ' Start RETRY
                        '
                        ' If there is an error AND Retry is switched AND RetryAttempts is > 0 AND the
                        ' error number generated is defined to be retried then ...
                        Dim intPos As Integer = 0
                        Dim intCount As Integer = 0
                        If err.HasError And Settings.RetryFailures And Settings.RetryAttempts > 0 Then
                            intPos = InStr(Settings.RetryErrorNumbers, err.ErrorNumber.Trim)
                            If intPos > 0 Then
                                intCount = 1
                                Do While intCount <= Settings.RetryAttempts And err.HasError

                                    If Settings.RetryWaitTime > 0 Then _
                                        System.Threading.Thread.Sleep(Settings.RetryWaitTime)

                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = String.Empty
                                        .ErrorNumber = String.Empty
                                        .HasError = False
                                    End With
                                    _resetConnection = True
                                    err = AccessDataBaseChorus()
                                    intCount += 1
                                Loop
                            End If
                        End If
                        ' End   RETRY
                        '
                    End If
                    ' End Ignore certain errors
                    '
                End If

                ChorusClose()
            Case Is = DEV
                err = AccessDataBaseDev()
            Case Is = TALENTADMIN
                err = TalentAdminOpen()
                If Not err.HasError Then err = AccessDataBaseTalentAdmin()
                TalentAdminClose()
            Case Is = TALENTQUEUE
                err = TalentQueueOpen()
                If Not err.HasError Then err = AccessDataBaseTALENTQUEUE()
                TalentQueueClose()
            Case Is = TALENTDEFINITIONS
                err = TalentDefinitionsOpen()
                If Not err.HasError Then err = AccessDataBaseTalentDefinitions()
                TalentDefinitionsClose()

        End Select

        'Log any errors
        If err.HasError Then Settings.Logging.ErrorObjectLog("DBAccess - AccessDatabase | Module - " & _settings.ModuleName, err)

        Dim logging As New TalentLogging()
        logging.LoadTestLog("DBAccess.vb - AccessDatabase", _settings.ModuleName, timeSpan)
        Return err
    End Function

    Protected Overridable Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function AccessDataBaseChorus() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function AccessDataBaseTALENTCRM() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function AccessDataBaseURL() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function AccessDataBaseADL() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function AccessDataBaseDev() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function AccessDataBaseTalentAdmin() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function AccessDataBaseTalentDefinitions() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function AccessDataBaseTALENTQUEUE() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Public Overridable Function ValidateAgainstDatabase() As ErrorObj
        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay

        Dim err As New ErrorObj
        '------------------------------------------------------------------------
        '   Warning :   Using ErrorObj here when closing the DB does cause a   
        '               problem as it will clear the err object when an actual
        '               error has occured in the Validate   
        '------------------------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                err = System21Open()
                If Not err.HasError Then
                    err = ValidateAgainstDatabaseSystem21()
                    '
                    ' Ignore certain errors
                    If err.HasError And InStr(Settings.IgnoreErrors, err.ErrorNumber.Trim) > 0 Then
                        err = Nothing
                    Else
                        '
                        ' Start RETRY
                        '
                        ' If there is an error AND Retry is switched AND RetryAttempts is > 0 AND the
                        ' error number generated is defined to be retried then ...
                        Dim intPos As Integer = 0
                        Dim intCount As Integer = 0
                        If err.HasError And Settings.RetryFailures And Settings.RetryAttempts > 0 Then
                            intPos = InStr(Settings.RetryErrorNumbers, err.ErrorNumber.Trim)
                            If intPos > 0 Then
                                intCount = 1
                                Do While intCount <= Settings.RetryAttempts And err.HasError
                                    If Settings.RetryWaitTime > 0 Then _
                                        System.Threading.Thread.Sleep(Settings.RetryWaitTime)

                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = String.Empty
                                        .ErrorNumber = String.Empty
                                        .HasError = False
                                    End With
                                    _resetConnection = True
                                    err = ValidateAgainstDatabaseSystem21()
                                    intCount += 1
                                Loop
                            End If
                        End If
                        ' End   RETRY
                        '
                    End If
                    ' End Ignore certain errors
                    '
                End If
                    System21Close()
            Case Is = SQL2005
                    err = Sql2005Open()
                    If Not err.HasError Then err = ValidateAgainstDatabaseSql2005()
                    Sql2005Close()
            Case Is = TALENTCRM
                    err = TALENTCRMOpen()
                    If Not err.HasError Then
                    err = ValidateAgainstDatabaseTALENTCRM()
                    '
                    ' Ignore certain errors
                    If err.HasError And InStr(Settings.IgnoreErrors, err.ErrorNumber.Trim) > 0 Then
                        err = Nothing
                    Else
                        '
                        ' Start RETRY
                        '
                        ' If there is an error AND Retry is switched AND RetryAttempts is > 0 AND the
                        ' error number generated is defined to be retried then ...
                        Dim intPos As Integer = 0
                        Dim intCount As Integer = 0
                        If err.HasError And Settings.RetryFailures And Settings.RetryAttempts > 0 Then
                            intPos = InStr(Settings.RetryErrorNumbers, err.ErrorNumber.Trim)
                            If intPos > 0 Then
                                intCount = 1
                                Do While intCount <= Settings.RetryAttempts And err.HasError
                                    If Settings.RetryWaitTime > 0 Then _
                                        System.Threading.Thread.Sleep(Settings.RetryWaitTime)
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = String.Empty
                                        .ErrorNumber = String.Empty
                                        .HasError = False
                                    End With
                                    err = ValidateAgainstDatabaseTALENTCRM()
                                    intCount += 1
                                Loop
                            End If
                        End If
                        ' End   RETRY
                        '
                    End If
                    ' End Ignore certain errors
                    '
                End If
                    TALENTCRMClose()
            Case Is = TALENTTKT
                    ' Ticketing programs do not validate against the system
            Case Is = URLREQUEST
                err = ValidateAgainstDatabaseURL()

            Case Is = CHORUS
                err = ChorusOpen()
                If Not err.HasError Then
                    err = ValidateAgainstDatabaseChorus()
                    '
                    ' Start Ignore certain errors
                    If err.HasError And InStr(Settings.IgnoreErrors, err.ErrorNumber.Trim) > 0 Then
                        err = Nothing
                    Else
                        '
                        ' Start RETRY
                        '
                        ' If there is an error AND Retry is switched AND RetryAttempts is > 0 AND the
                        ' error number generated is defined to be retried then ...
                        Dim intPos As Integer = 0
                        Dim intCount As Integer = 0
                        If err.HasError And Settings.RetryFailures And Settings.RetryAttempts > 0 Then
                            intPos = InStr(Settings.RetryErrorNumbers, err.ErrorNumber.Trim)
                            If intPos > 0 Then
                                intCount = 1
                                Do While intCount <= Settings.RetryAttempts And err.HasError

                                    If Settings.RetryWaitTime > 0 Then _
                                        System.Threading.Thread.Sleep(Settings.RetryWaitTime)

                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = String.Empty
                                        .ErrorNumber = String.Empty
                                        .HasError = False
                                    End With
                                    _resetConnection = True
                                    err = ValidateAgainstDatabaseChorus()
                                    intCount += 1
                                Loop
                            End If
                        End If
                        ' End   RETRY
                        '
                    End If
                    ' End Ignore certain errors
                    '
                End If

                ChorusClose()

        End Select

        'Log any errors
        If err.HasError Then Settings.Logging.ErrorObjectLog("DBAccess - ValidateAgainstDatabase", err)

        Dim logging As New TalentLogging()
        logging.LoadTestLog("DBAccess.vb - ValidateAgainstDatabase", _settings.ModuleName, timeSpan)
        Return err
    End Function

    Protected Overridable Function ValidateAgainstDatabaseChorus() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function ValidateAgainstDatabaseSql2005() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function ValidateAgainstDatabaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function ValidateAgainstDatabaseTALENTCRM() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function ValidateAgainstDatabaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function ValidateAgainstDatabaseURL() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overridable Function ValidateAgainstDatabaseTALENTQUEUE() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then

        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Function ChorusOpen() As ErrorObj
        Dim err As New ErrorObj
        Dim iCounter As Int16 = 0
        '---------------------------------------------------------------------------------
        '   Attempt to open database
        '
        Const strError As String = "Could not establish connection to the Chorus database"
        Try
            While iCounter < 10
                If conChorus Is Nothing Then
                    conChorus = New iDB2Connection(Settings.BackOfficeConnectionString)
                    conChorus.Open()
                ElseIf conChorus.State <> ConnectionState.Open Then
                    conChorus = New iDB2Connection(Settings.BackOfficeConnectionString)
                    conChorus.Open()
                End If
                '---------------------------------------------------------------------------------
                '   Possible the iSeries needs to wake up so 
                '
                If conChorus.State = ConnectionState.Open Then
                    Exit While
                Else
                    iCounter += 1
                    If iCounter > 10 Then
                        With err
                            .ErrorMessage = conSystem21.State
                            .ErrorStatus = strError
                            .ErrorNumber = "TACDBAC-21a"
                            .HasError = True
                        End With
                        Exit While
                    End If
                    System.Threading.Thread.Sleep(2500)             ' Sleep for 2.5 seconds
                End If
                '---------------------------------------------------------------------------------
            End While
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAC-21b"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "ChorusOpen", strError, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Protected Function ChorusClose() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------
        '   Warning :   Using ErrorObj here when closing the DB does cause a   
        '               problem as it will clear the err object when an actual
        '               error has occured elsewhere   
        '------------------------------------------------------------------------
        Try
            If Not (conChorus Is Nothing) Then
                If (conChorus.State = ConnectionState.Open) Then
                    conChorus.Close()
                End If
            End If
            conChorus = Nothing
        Catch ex As Exception
            Const strError4 As String = "Failed to close the  Chorus database connection"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError4
                .ErrorNumber = "TACDBAC-P20"
                .HasError = True
                Return err
            End With
            TalentLogger.Logging(CLASSNAME, "ChorusClose", strError4, err, ex, LogTypeConstants.TCDBACCESSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Executes the command object using SQLDataAdapter to fill the dataset and return
    ''' </summary>
    ''' <param name="commandToExecute">The command to execute.</param>
    ''' <returns>DataSet</returns>
    Protected Function ExecuteDataSetForSQLDB(ByVal commandToExecute As SqlCommand) As DataSet
        Dim outputDataSet As DataSet = Nothing
        Dim sqlAdapater As New SqlDataAdapter
        Using SqlDataAdapterExecuter As New SqlDataAdapter(commandToExecute)
            outputDataSet = New DataSet()
            SqlDataAdapterExecuter.Fill(outputDataSet)
        End Using
        Return outputDataSet
    End Function

    Public Sub createSupplyNetRequest(ByVal Partner As String, _
                                           ByVal LoginID As String, _
                                           ByVal RequestModule As String, _
                                           ByVal TransactionID As String, _
                                           ByVal TotalRecords As Integer, _
                                           ByVal ProcessedRecords As Integer, _
                                           ByVal TransactionStartDate As DateTime, _
                                           ByVal TransactionEndDate As DateTime, _
                                           ByVal Overwrite As Boolean)

        Const dFormat As String = "dd/MMM/yyyy HH:mm:ss"

        '
        ' If the SupplyNet request name has not been passed, then use the TalentCommon module name.
        '
        If RequestModule = String.Empty Then
            RequestModule = Settings.ModuleName
        End If

        If Settings.LogRequests And TransactionID <> String.Empty Then
            Try
                Sql2005Open()

                Const strSelect1 As String = "SELECT * FROM tbl_supplynet_requests WHERE Transaction_ID = @Param4"
                Const strDelete As String = "DELETE FROM tbl_supplynet_requests WHERE Transaction_ID = @Param4"

                Dim param1 As String = "@Param1"
                Dim param2 As String = "@Param2"
                Dim param3 As String = "@Param3"
                Dim param4 As String = "@Param4"
                Dim param5 As String = "@Param5"
                Dim param6 As String = "@Param6"
                Dim param7 As String = "@Param7"
                Dim param8 As String = "@Param8"

                Dim cmdSelect As SqlCommand = New SqlCommand(strSelect1, conSql2005)
                Dim cmdDelete As SqlCommand = New SqlCommand(strDelete, conSql2005)

                cmdSelect.Parameters.Add(New SqlParameter(param4, SqlDbType.Char, 50)).Value = TransactionID
                Dim reader As SqlDataReader = cmdSelect.ExecuteReader()
                If reader.HasRows And Overwrite = True Then
                    cmdDelete.Parameters.Add(New SqlParameter(param4, SqlDbType.Char, 50)).Value = TransactionID
                    cmdDelete.ExecuteNonQuery()
                End If
                If reader.HasRows And Overwrite = False Then
                    ' Do nothing if we should not overwrite the progress record and an entry
                    ' lready exisits for this transaction ID.
                Else
                    reader.Close()
                    Const strInsert1 As String = "INSERT INTO tbl_supplynet_requests " & _
                                                    "(Partner, LoginID, Module, Transaction_ID, Total_Records, Processed_Records, Transaction_Start_Date " & _
                                                        ") VALUES (@Param1, @Param2, @Param3, @Param4, @Param5, @Param6, @Param7 )"
                    Dim cmdInsert As SqlCommand = New SqlCommand(strInsert1, conSql2005)

                    With cmdInsert
                        With .Parameters()
                            .Add(New SqlParameter(param1, SqlDbType.Char, 50)).Value = Partner
                            .Add(New SqlParameter(param2, SqlDbType.Char, 100)).Value = LoginID
                            .Add(New SqlParameter(param3, SqlDbType.Char, 50)).Value = RequestModule
                            .Add(New SqlParameter(param4, SqlDbType.Char, 50)).Value = TransactionID
                            .Add(New SqlParameter(param5, SqlDbType.BigInt)).Value = TotalRecords
                            .Add(New SqlParameter(param6, SqlDbType.BigInt)).Value = ProcessedRecords
                            .Add(New SqlParameter(param7, SqlDbType.DateTime)).Value = TransactionStartDate.ToString(dFormat)
                        End With
                    End With

                    cmdInsert.ExecuteNonQuery()
                End If
            Catch e As Exception

            Finally
                Sql2005Close()
            End Try
        End If
    End Sub

    Public Function incrementSupplyNetProgressCount(ByVal TransactionID As String) As Integer
        Dim count = 0
        Dim reader As SqlDataReader = Nothing

        If Settings.LogRequests And TransactionID <> String.Empty Then
            Try
                Sql2005Open()

                Const strIncCnt As String = "UPDATE tbl_supplynet_requests SET Processed_Records = Processed_Records + 1 WHERE Transaction_ID = @Param1"
                Const strSelCnt As String = "SELECT * from tbl_supplynet_requests WHERE Transaction_ID = @Param1"

                Dim param1 As String = "@Param1"

                Dim cmdincrement As SqlCommand = New SqlCommand(strIncCnt, conSql2005)
                Dim cmdselectCnt As SqlCommand = New SqlCommand(strSelCnt, conSql2005)

                cmdincrement.Parameters.Add(New SqlParameter(param1, SqlDbType.Char, 50)).Value = TransactionID
                cmdselectCnt.Parameters.Add(New SqlParameter(param1, SqlDbType.Char, 50)).Value = TransactionID

                'increment the count
                reader = cmdincrement.ExecuteReader()
                reader.Close()

                reader = cmdselectCnt.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    count = reader.Item("Processed_Records")
                End If
                reader.Close()

            Catch e As Exception
                If Not reader Is Nothing Then
                    If Not reader.IsClosed Then
                        reader.Close()
                    End If
                End If
            Finally
                Sql2005Close()
            End Try

            Return count
        End If
    End Function

    Public Sub updateSupplyNetProgressCount(ByVal TransactionID As String, ByVal Count As Integer)
        If Settings.LogRequests And TransactionID <> String.Empty Then
            Try
                Sql2005Open()
                Dim param1 As String = "@Param1"
                Dim param2 As String = "@Param2"

                Const strUpdate As String = "UPDATE tbl_supplynet_requests SET " & _
                                                "Processed_Records = @Param1 WHERE Transaction_ID = @Param2"
                Dim cmdUpdate As SqlCommand = New SqlCommand(strUpdate, conSql2005)

                With cmdUpdate
                    With .Parameters()
                        .Add(New SqlParameter(param1, SqlDbType.BigInt)).Value = Count
                        .Add(New SqlParameter(param2, SqlDbType.Char, 50)).Value = TransactionID
                    End With
                End With

                cmdUpdate.ExecuteNonQuery()
            Catch e As Exception
            Finally
                Sql2005Close()
            End Try
        End If
    End Sub

    Public Sub markSupplyNetTransactionAsCompleted(ByVal TransactionID As String)
        Const dFormat As String = "dd/MMM/yyyy HH:mm:ss"
        If Settings.LogRequests And TransactionID <> String.Empty Then
            Try
                Sql2005Open()
                Dim param1 As String = "@Param1"
                Dim param2 As String = "@Param2"

                Const strUpdate As String = "UPDATE tbl_supplynet_requests SET " & _
                                                "Transaction_End_Date = @Param1 WHERE Transaction_ID = @Param2"
                Dim cmdUpdate As SqlCommand = New SqlCommand(strUpdate, conSql2005)

                With cmdUpdate
                    With .Parameters()
                        .Add(New SqlParameter(param1, SqlDbType.DateTime)).Value = Now.ToString(dFormat)
                        .Add(New SqlParameter(param2, SqlDbType.Char, 50)).Value = TransactionID
                    End With
                End With

                cmdUpdate.ExecuteNonQuery()
            Catch e As Exception
            Finally
                Sql2005Close()
            End Try
        End If
    End Sub

End Class

