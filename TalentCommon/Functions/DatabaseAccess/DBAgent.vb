Imports IBM.Data.DB2.iSeries
Imports System.Text

<Serializable()> _
Public Class DBAgent
    Inherits DBAccess

#Region "Class Level Fields"

    Private _agentDataEntity As DEAgent

#End Region

#Region "Public Properties"

    Public Property AgentDataEntity() As DEAgent
        Get
            Return _agentDataEntity
        End Get
        Set(ByVal value As DEAgent)
            _agentDataEntity = value
        End Set
    End Property

#End Region

#Region "Protected Functions"

    ''' <summary>
    ''' Determine the module we are calling for this class and return the error object
    ''' </summary>
    ''' <returns>An error object</returns>
    ''' <remarks></remarks>
    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case Settings.ModuleName
            Case Is = "AgentLogin" : err = AccessDatabaseWS124R()
            Case Is = "AgentLogout" : err = AccessDatabaseWS127R()
            Case Is = "RetrieveAllAgents" : err = AccessDatabaseMN005S()
            Case Is = "RetrieveAgentPrinters" : err = AccessDatabaseWS126R()
            Case Is = "UpdateAgentPrinters" : err = AccessDatabaseWS127R()
            Case Is = "RetrieveApprovedReservationCode" : err = AccessDatabaseWS129R()
            Case Is = "RetrieveSavedSearch" : err = AccessDatabaseMN05DS()
            Case Is = "CreateNewSavedSearch" : err = AccessDatabaseMN05DS()
            Case Is = "DeleteSavedSearch" : err = AccessDatabaseMN05DS()
            Case Is = "RetrieveAgentDepartment" : err = AccessDatabaseAC006S()
            Case Is = "RetrieveAgentGroup" : err = AccessDatabaseWS127R()
            Case Is = "AgentCopy" : err = AccessDatabaseWS004R()
        End Select

        Return err
    End Function

   
#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Build the call to WS124R, populate the data entity and return an error object
    ''' </summary>
    ''' <returns>An error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseWS124R() As ErrorObj
        Dim err As New ErrorObj
        Dim PARAMOUT As String = String.Empty
        Try
            PARAMOUT = CallWS124R()
            If PARAMOUT.Substring(5119, 1) = GlobalConstants.ERRORFLAG Then
                _agentDataEntity.ErrorCode = PARAMOUT.Substring(5117, 2).Trim
                _agentDataEntity.AgentType = String.Empty
            Else
                _agentDataEntity.AgentType = PARAMOUT.Substring(0, 1).Trim()
                _agentDataEntity.PrinterGrp = PARAMOUT.Substring(1, 10).Trim()
                _agentDataEntity.DftTKTPrtr = PARAMOUT.Substring(11, 10).Trim()
                _agentDataEntity.DftSCPrtr = PARAMOUT.Substring(21, 10).Trim()
                _agentDataEntity.Tkt1_Home = PARAMOUT.Substring(31, 10).Trim()
                _agentDataEntity.Tkt2_Event = PARAMOUT.Substring(41, 10).Trim()
                _agentDataEntity.Tkt3_Travel = PARAMOUT.Substring(51, 10).Trim()
                _agentDataEntity.Tkt4_STkt = PARAMOUT.Substring(61, 10).Trim()
                _agentDataEntity.Tkt5_Addr = PARAMOUT.Substring(71, 10).Trim()
                _agentDataEntity.PrintAddrYN = PARAMOUT.Substring(81, 1).Trim()
                _agentDataEntity.PrintRcptYN = PARAMOUT.Substring(82, 1).Trim()
                _agentDataEntity.AgentCompany = PARAMOUT.Substring(83, 3).Trim()
                _agentDataEntity.BulkSalesMode = Utilities.convertToBool(PARAMOUT.Substring(86, 1).Trim())
                _agentDataEntity.AgentAuthorityGroupID = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(124, 7).Trim())
                _agentDataEntity.PrintAlways = Utilities.convertToBool(PARAMOUT.Substring(131, 1).Trim())
                _agentDataEntity.DefaultCaptureMethod = PARAMOUT.Substring(132, 7).Trim().TrimStart("0")
                _agentDataEntity.Department = PARAMOUT.Substring(5076, 10).Trim()
                _agentDataEntity.PrinterNameDefault = PARAMOUT.Substring(5086, 10).Trim()
                _agentDataEntity.ErrorCode = String.Empty
                _agentDataEntity.CorporateHospitalityMode = Utilities.convertToBool(PARAMOUT.Substring(139, 1).Trim())
            End If
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAgent-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Make the call to the WS API and return the outbound parameter as a string
    ''' </summary>
    ''' <returns>The outbound parameter as it is returned from the iSeries</returns>
    ''' <remarks></remarks>
    Private Function CallWS124R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        Dim strHEADER As New StringBuilder

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS124R(@PARAM1)")

        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS124Parm()
        parmIO.Direction = ParameterDirection.InputOutput
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        Return PARAMOUT
    End Function

    ''' <summary>
    ''' Format the inbound parameter value
    ''' </summary>
    ''' <returns>The formatted parameter as a string</returns>
    ''' <remarks></remarks>
    Private Function WS124Parm() As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength(String.Empty, 87))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.SessionID, 36))
        myString.Append(Utilities.FixStringLength(String.Empty, 4953))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.Department, 10))
        myString.Append(Utilities.FixStringLength(String.Empty, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.AgentUsername, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.AgentPassword, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.Source, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))
        Return myString.ToString()
    End Function


    ''' <summary>
    ''' Build the call to MN005S, populate the resultset and return an error object
    ''' </summary>
    ''' <returns>An error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseMN005S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Agent Users data table
        Dim dtAgentUsers As New DataTable("AgentUsers")
        ResultDataSet.Tables.Add(dtAgentUsers)

        Try
            Dim cmd As iDB2Command = conTALENTTKT.CreateCommand()
            cmd.CommandText = "CALL MN005S(@PARAM0, @PARAM1)"
            cmd.CommandType = CommandType.Text

            Dim parmIO_0 As iDB2Parameter
            parmIO_0 = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 1)
            parmIO_0.Value = AgentDataEntity.Source
            parmIO_0.Direction = ParameterDirection.Input

            Dim parmIO_1 As iDB2Parameter
            parmIO_1 = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10)
            parmIO_1.Value = String.Empty
            parmIO_1.Direction = ParameterDirection.InputOutput

            Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
            cmdAdapter.SelectCommand = cmd
            cmdAdapter.Fill(ResultDataSet, "AgentUsers")

            Dim drStatus As DataRow = dtStatusResults.NewRow
            If CStr(cmd.Parameters(1).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
                drStatus("ReturnCode") = CStr(cmd.Parameters(1).Value).Trim
            Else
                drStatus("ErrorOccurred") = String.Empty
                drStatus("ReturnCode") = String.Empty
            End If
            dtStatusResults.Rows.Add(drStatus)

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBAgent-MN005S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build the call to MN005DS, populate the resultset and return an error object
    ''' </summary>
    ''' <returns>An error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseMN05DS() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim dtSavedSearches As New DataTable("SavedSearches")
        ResultDataSet.Tables.Add(dtSavedSearches)

        Try
            Dim cmd As iDB2Command = conTALENTTKT.CreateCommand()
            cmd.CommandText = "CALL MN05DS(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7, @PARAM8, @PARAM9, @PARM10, @PARM11, @PARM12)"
            cmd.CommandType = CommandType.Text

            Dim pSource As iDB2Parameter
            Dim pErrorCode As iDB2Parameter
            Dim pMode As iDB2Parameter
            Dim pSelectedTalentUser As iDB2Parameter
            Dim pSavedSearchLimit As iDB2Parameter
            Dim pKeywordValue As iDB2Parameter
            Dim pCategoryValue As iDB2Parameter
            Dim pLocationValue As iDB2Parameter
            Dim pDateValue As iDB2Parameter
            Dim pSearchType As iDB2Parameter
            Dim pStadiumCode As iDB2Parameter
            Dim pProductType As iDB2Parameter
            Dim pSavedSearchID As iDB2Parameter

            pSource = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 1)
            pErrorCode = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10)
            pErrorCode.Direction = ParameterDirection.InputOutput
            pMode = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1)
            pSelectedTalentUser = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 10)
            pSavedSearchLimit = cmd.Parameters.Add(Param4, iDB2DbType.iDB2Integer)
            pKeywordValue = cmd.Parameters.Add(Param5, iDB2DbType.iDB2VarChar, 250)
            pCategoryValue = cmd.Parameters.Add(Param6, iDB2DbType.iDB2VarChar, 250)
            pLocationValue = cmd.Parameters.Add(Param7, iDB2DbType.iDB2VarChar, 250)
            pDateValue = cmd.Parameters.Add(Param8, iDB2DbType.iDB2VarChar, 250)
            pSearchType = cmd.Parameters.Add(Param9, iDB2DbType.iDB2Char, 1)
            pStadiumCode = cmd.Parameters.Add(Param10, iDB2DbType.iDB2Char, 2)
            pProductType = cmd.Parameters.Add(Param11, iDB2DbType.iDB2Char, 1)
            pSavedSearchID = cmd.Parameters.Add(Param12, iDB2DbType.iDB2Integer)

            pSource.Value = AgentDataEntity.Source
            pErrorCode.Value = String.Empty
            pMode.Value = AgentDataEntity.SavedSearchMode
            pSelectedTalentUser.Value = AgentDataEntity.AgentUsername
            pSavedSearchLimit.Value = AgentDataEntity.SavedSearchLimit
            pKeywordValue.Value = AgentDataEntity.SavedSearchKeyword
            pCategoryValue.Value = AgentDataEntity.SavedSearchCategory
            pLocationValue.Value = AgentDataEntity.SavedSearchLocation
            pDateValue.Value = AgentDataEntity.SavedSearchDate
            pSearchType.Value = AgentDataEntity.SavedSearchType
            pStadiumCode.Value = AgentDataEntity.SavedSearchStadium
            pProductType.Value = AgentDataEntity.SavedSearchProductType
            pSavedSearchID.Value = AgentDataEntity.SavedSearchUniqueID

            Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
            cmdAdapter.SelectCommand = cmd
            cmdAdapter.Fill(ResultDataSet, "SavedSearches")

            Dim drStatus As DataRow = dtStatusResults.NewRow
            If CStr(cmd.Parameters(1).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
                drStatus("ReturnCode") = CStr(cmd.Parameters(1).Value).Trim
            Else
                drStatus("ErrorOccurred") = String.Empty
                drStatus("ReturnCode") = String.Empty
            End If
            dtStatusResults.Rows.Add(drStatus)

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBAgent-MN05DS"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build the call to WS126R(Retrieve Printer Groups and Agent Group/Printers)
    ''' and populate the data entity and return an error object
    ''' </summary>
    ''' <returns>An error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseWS126R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim PARAMOUT As String = String.Empty
        Dim bMoreRecordsGroups As Boolean = True
        Dim bMoreRecordsPrinters As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim lastRRNGroups As String = ""
        Dim lastRRNPrinters As String = ""
        Dim ViewAllowedYN As String = ""

        Dim DtStatusResults As New DataTable
        DtStatusResults.TableName = "ResultStatus"
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtPrinterGroups As New DataTable
        DtPrinterGroups.TableName = "PrinterGroups"
        ResultDataSet.Tables.Add(DtPrinterGroups)
        With DtPrinterGroups.Columns
            .Add("GROUPNAME", GetType(String))
            .Add("GROUPDESCRIPTION", GetType(String))
        End With

        Dim DtPrinters As New DataTable
        DtPrinters.TableName = "Printers"
        ResultDataSet.Tables.Add(DtPrinters)
        With DtPrinters.Columns
            .Add("VIEW_ALLOWED_Y_N", GetType(String))
            .Add("AMEND_ALLOWED_Y_N", GetType(String))
            .Add("PRINTERNAME", GetType(String))
            .Add("PRINTERDESCRIPTION", GetType(String))
            .Add("PRINTERTYPE", GetType(String))
            .Add("PRINTERGROUP", GetType(String))
        End With

        Try
            Do While bMoreRecordsGroups Or bMoreRecordsPrinters

                PARAMOUT = CallWS126R(lastRRNGroups, lastRRNPrinters)

                ' Groups Loop
                Dim hasError As Boolean = True
                dRow = Nothing
                dRow = DtStatusResults.NewRow
                If PARAMOUT.Substring(32764, 1) = "E" Or PARAMOUT.Substring(32762, 2).Trim <> "" Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(32762, 2)
                    bMoreRecordsGroups = False
                    hasError = True
                Else
                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                    bMoreRecordsGroups = Utilities.convertToBool(PARAMOUT.Substring(32719, 1))
                    lastRRNGroups = PARAMOUT.Substring(32720, 15)
                    hasError = False
                End If

                DtStatusResults.Rows.Add(dRow)

                If Not hasError Then
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 50
                        If PARAMOUT.Substring(iPosition, 100).Trim = "" Then
                            Exit Do
                        Else
                            dRow = Nothing
                            dRow = DtPrinterGroups.NewRow
                            dRow("GROUPNAME") = (PARAMOUT.Substring(iPosition, 10)).Trim()
                            dRow("GROUPDESCRIPTION") = (PARAMOUT.Substring(iPosition + 10, 50)).Trim()
                            DtPrinterGroups.Rows.Add(dRow)
                            iPosition = iPosition + 100
                            iCounter = iCounter + 1
                        End If
                    Loop
                End If

                ' Printer Loop
                dRow = Nothing
                dRow = DtStatusResults.NewRow
                If PARAMOUT.Substring(32764, 1) = "E" Or PARAMOUT.Substring(32762, 2).Trim <> "" Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(32762, 2)
                    bMoreRecordsPrinters = False
                    hasError = True
                Else
                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                    bMoreRecordsPrinters = Utilities.convertToBool(PARAMOUT.Substring(32735, 1))
                    lastRRNPrinters = PARAMOUT.Substring(32736, 15)
                    hasError = False
                End If
                DtStatusResults.Rows.Add(dRow)

                If Not hasError Then
                    Dim iPosition As Integer = 5000
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 250
                        If PARAMOUT.Substring(iPosition, 100).Trim = "" Then
                            Exit Do
                        Else
                            dRow = Nothing
                            dRow = DtPrinters.NewRow
                            dRow("VIEW_ALLOWED_Y_N") = (PARAMOUT.Substring(30000, 1)).Trim()
                            dRow("AMEND_ALLOWED_Y_N") = (PARAMOUT.Substring(iPosition, 1)).Trim()
                            dRow("PRINTERNAME") = (PARAMOUT.Substring(iPosition + 1, 10)).Trim()
                            dRow("PRINTERDESCRIPTION") = (PARAMOUT.Substring(iPosition + 11, 50)).Trim()
                            dRow("PRINTERTYPE") = (PARAMOUT.Substring(iPosition + 61, 1)).Trim()
                            dRow("PRINTERGROUP") = (PARAMOUT.Substring(iPosition + 62, 10)).Trim()
                            DtPrinters.Rows.Add(dRow)
                            iPosition = iPosition + 100
                            iCounter = iCounter + 1
                        End If
                    Loop
                End If
            Loop

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAgent-02"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Make the call to the WS API and return the outbound parameter as a string
    ''' </summary>
    ''' <returns>The outbound parameter as it is returned from the iSeries</returns>
    ''' <remarks></remarks>
    Private Function CallWS126R(ByVal lastRRNGroups, ByVal lastRRNPrinters) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        Dim strHEADER As New StringBuilder

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS126R(@PARAM1)")

        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 32765)
        parmIO.Value = WS126Parm(lastRRNGroups, lastRRNPrinters)
        parmIO.Direction = ParameterDirection.InputOutput
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        Return PARAMOUT
    End Function

    ''' <summary>
    ''' Format WS126R inbound parameter value
    ''' </summary>
    ''' <returns>The formatted parameter as a string</returns>
    ''' <remarks></remarks>
    Private Function WS126Parm(ByVal lastRRNGroups As String, ByVal lastRRNPrinters As String) As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength(String.Empty, 32720))
        myString.Append(Utilities.PadLeadingZeros(lastRRNGroups, 15))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.PadLeadingZeros(lastRRNPrinters, 15))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.AgentUsername, 10))
        Return myString.ToString()
    End Function

    ''' <summary>
    ''' Update Agent Group/Printers - WS127R
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseWS127R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("Status")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            'Call WS127R
            PARAMOUT = CallWS127R()

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(32764, 1) = "E" Or PARAMOUT.Substring(32762, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(32762, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAgent-03"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Call the WS program
    ''' </summary>
    ''' <returns>String paramter object</returns>
    ''' <remarks></remarks>
    Private Function CallWS127R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS127R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 32765)
        parmIO.Value = WS127Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        Utilities.TalentCommonLog("CallWS127R", _agentDataEntity.AgentUsername, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        Utilities.TalentCommonLog("CallWS127R", _agentDataEntity.AgentUsername, "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    ''' <summary>
    ''' Build the inbound parameter
    ''' </summary>
    ''' <returns>The constructed parameter</returns>
    ''' <remarks></remarks>
    Private Function WS127Parm() As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength(_agentDataEntity.PrinterGrp, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.DftTKTPrtr, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.DftSCPrtr, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.Tkt1_Home, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.Tkt2_Event, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.Tkt3_Travel, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.Tkt4_STkt, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.Tkt5_Addr, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.PrintAddrYN, 1))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.PrintRcptYN, 1))
        myString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(_agentDataEntity.BulkSalesMode), 1))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.SessionID, 36))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.Department, 10))
        myString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(_agentDataEntity.DeleteSessionRecord), 1))
        myString.Append(Utilities.PadLeadingZeros(_agentDataEntity.AgentAuthorityGroupID, 7))
        myString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(_agentDataEntity.PrintAlways), 1))
        myString.Append(Utilities.PadLeadingZeros(_agentDataEntity.DefaultCaptureMethod, 7))
        myString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(_agentDataEntity.CorporateHospitalityMode), 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 32605))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.AgentUsername, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.Source, 1))
        myString.Append("   ")
        Return myString.ToString()
    End Function

    ''' <summary>
    ''' Return Reservation Codes - WS129R
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseWS129R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        DtStatusResults.TableName = "StatusResults"
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("SellAvailableTickets", GetType(String))
        End With

        'Create the Approved Restriction Codes data table
        Dim DtApprovedRestrictionCodes As New DataTable
        DtApprovedRestrictionCodes.TableName = "ApprovedReservationCodes"
        ResultDataSet.Tables.Add(DtApprovedRestrictionCodes)
        With DtApprovedRestrictionCodes.Columns
            .Add("ReservationCode", GetType(String))
        End With

        Try
            'Call WS129R
            PARAMOUT = CallWS129R()

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(10237, 2) = "E" Or PARAMOUT.Substring(PARAMOUT.Length - 1, 1).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(10239, 1)
                dRow("SellAvailableTickets") = "N"
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                dRow("SellAvailableTickets") = PARAMOUT.Substring(10224, 1)
            End If
            DtStatusResults.Rows.Add(dRow)
            If Not (PARAMOUT.Substring(10237, 2) = "E" Or PARAMOUT.Substring(10239, 1).Trim <> "") Then
                'Set the restriction codes table
                For i As Integer = 0 To 4999
                    If PARAMOUT.Substring(i * 2, 2).Trim <> String.Empty Then
                        dRow = Nothing
                        dRow = DtApprovedRestrictionCodes.NewRow
                        dRow("ReservationCode") = PARAMOUT.Substring(i * 2, 2).Trim
                        DtApprovedRestrictionCodes.Rows.Add(dRow)
                    Else
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAgent-03"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Make the call to the WS API and return the outbound parameter as a string
    ''' </summary>
    ''' <returns>The outbound parameter as it is returned from the iSeries</returns>
    ''' <remarks></remarks>
    Private Function CallWS129R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS129R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10240)
        parmIO.Value = WS129Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        Utilities.TalentCommonLog("CallWS129R", _agentDataEntity.AgentUsername, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        Utilities.TalentCommonLog("CallWS129R", _agentDataEntity.AgentUsername, "Backend Response: PARAMOUT=" & PARAMOUT)
        Return PARAMOUT
    End Function

    ''' <summary>
    ''' Format WS129R inbound parameter value
    ''' </summary>
    ''' <returns>The formatted parameter as a string</returns>
    ''' <remarks></remarks>
    Private Function WS129Parm() As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength("", 10225))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.AgentUsername, 11))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength("", 2))
        myString.Append(Utilities.FixStringLength("", 1))
        Return myString.ToString()
    End Function

    ''' <summary>
    ''' Build the call to AC006S (Retrieve Active Department records)
    ''' and populate the data entity and return an error object
    ''' </summary>
    ''' <returns>An error object</returns>
    Private Function AccessDatabaseAC006S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Saved Searches data table
        Dim dtDepartments As New DataTable("Departments")
        ResultDataSet.Tables.Add(dtDepartments)

        Try
            Dim cmd As iDB2Command = conTALENTTKT.CreateCommand()
            cmd.CommandText = "CALL AC006S(@PARAM0, @PARAM1)"
            cmd.CommandType = CommandType.Text

            Dim pSource As iDB2Parameter
            Dim pErrorCode As iDB2Parameter

            pErrorCode = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
            pErrorCode.Direction = ParameterDirection.InputOutput
            pSource = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)

            pSource.Value = AgentDataEntity.Source
            pErrorCode.Value = String.Empty

            Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
            cmdAdapter.SelectCommand = cmd
            cmdAdapter.Fill(ResultDataSet, "Departments")

            Dim drStatus As DataRow = dtStatusResults.NewRow
            If CStr(cmd.Parameters(0).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
                drStatus("ReturnCode") = CStr(cmd.Parameters(0).Value).Trim
            Else
                drStatus("ErrorOccurred") = String.Empty
                drStatus("ReturnCode") = String.Empty
            End If
            dtStatusResults.Rows.Add(drStatus)

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBAgent-AC006S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function AccessDatabaseWS004R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            'Call WS004R
            PARAMOUT = CallWS004R()

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)


        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAgent-04"
                .HasError = True
            End With
        End Try
        Return err
    End Function


    Private Function CallWS004R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS004R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS004Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        Utilities.TalentCommonLog("CallWS004R", _agentDataEntity.AgentUsername, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        Utilities.TalentCommonLog("CallWS004R", _agentDataEntity.AgentUsername, "Backend Response: PARAMOUT=" & PARAMOUT)
        Return PARAMOUT
    End Function


    Private Function WS004Parm() As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength(_agentDataEntity.AgentUsername, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.NewAgent, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.NewAgentPassword, 10))
        myString.Append(Utilities.FixStringLength(_agentDataEntity.NewAgentDescription, 30))
        myString.Append(Utilities.FixStringLength("", 960))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength("", 3))
        Return myString.ToString()
    End Function

#End Region

End Class