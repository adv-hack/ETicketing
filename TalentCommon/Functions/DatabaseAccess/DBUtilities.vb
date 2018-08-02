Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.IO
Imports System.Data
Imports System.Xml
Imports Talent.Common
Imports Talent.Common.Utilities

'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Utilities 
'
'       Date                        10/07/08
'
'       Author                      Ben Ford
'
'       @ CS Group 2008             All rights reserved.
'                                    
'       Error Number Code base      TACDBUT- 
'                                   
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------

<Serializable()> _
Public Class DBUtilities
    Inherits DBAccess

    Private _moduleName As String
    Public Property ModuleName() As String
        Get
            Return _moduleName
        End Get
        Set(ByVal value As String)
            _moduleName = value
        End Set
    End Property

    Private _SuccessfullCall As Boolean
    Public Property SuccessfullCall() As Boolean
        Get
            Return _SuccessfullCall
        End Get
        Set(ByVal value As Boolean)
            _SuccessfullCall = value
        End Set
    End Property

    Sub New(ByVal _ModuleName_ As String)
        MyBase.New()

        Me.ModuleName = _ModuleName_

    End Sub

    Private _descriptionKey As String = String.Empty
    Public Property DescriptionKey() As String
        Get
            Return _descriptionKey
        End Get
        Set(ByVal value As String)
            _descriptionKey = value
        End Set
    End Property

    Private _customerNumber As String = String.Empty
    Public Property CustomerNumber() As String
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As String)
            _customerNumber = value
        End Set
    End Property

    Private _paymentRef As Integer = 0
    Public Property PaymentRef() As Integer
        Get
            Return _paymentRef
        End Get
        Set(ByVal value As Integer)
            _paymentRef = value
        End Set
    End Property

    Private _utilityCode As Integer = 0
    Public Property UtilityCode() As Integer
        Get
            Return _utilityCode
        End Get
        Set(ByVal value As Integer)
            _utilityCode = value
        End Set
    End Property


    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case ModuleName
            Case Is = "CheckBackEndDatabase"
                err = CheckBackEndDatabase_TALENTTKT()
            Case Is = "RetrieveDescriptionEntries"
                err = AccessDatabaseMD501S()
            Case Is = "VerifyPaymentReference"
                err = AccessDatabaseWS131R()
        End Select

        Return err
    End Function

    Protected Function CheckBackEndDatabase_TALENTTKT() As ErrorObj
        Dim err As New ErrorObj
        err.HasError = True
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS990R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty


        Try
            'Set the connection string
            cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

            'Populate the parameter
            parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
            parmIO.Value = Talent.Common.Utilities.FixStringLength(" ", 1024)
            parmIO.Direction = ParameterDirection.InputOutput

            'Execute
            cmdSELECT.ExecuteNonQuery()

            PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
            If PARAMOUT.Substring(1023, 1) <> "Y" Then
                err.HasError = True
                err.ErrorMessage = "Unable to call stored procedure"
                err.ErrorNumber = "TACDBUT-01"
            Else
                err.HasError = False
            End If
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBUT-53B"
                .HasError = True
            End With

        End Try
        Return err
    End Function

    Private Function AccessDatabaseWS130R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        Dim charHash As String = "#"
        Dim charSemi As String = ";"
        Dim loopCount As Integer = 0

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the restriction descriptions table
        Dim DtDescriptionEntries As New DataTable
        ResultDataSet.Tables.Add(DtDescriptionEntries)
        With DtDescriptionEntries.Columns
            .Add("Code", GetType(String))
            .Add("Description", GetType(String))
        End With

        Try

            'Call WS130R for the description entries
            PARAMOUT = CallWS130R()

            'Set the status data table
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(5119, 1) = "E" Or PARAMOUT.Substring(5117, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(5117, 2).Trim
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            If Not (PARAMOUT.Substring(5119, 1) = "E" Or PARAMOUT.Substring(5117, 2).Trim <> "") Then

                'Set the description table
                With DtDescriptionEntries
                    Dim rowArray() As String = Split(PARAMOUT.Substring(0, 5000), charHash)
                    loopCount = 0
                    Do While loopCount < rowArray.Length - 1
                        dRow = Nothing
                        dRow = DtDescriptionEntries.NewRow
                        Dim itemArray() As String = Split(rowArray(loopCount).ToString, charSemi)
                        dRow("Code") = itemArray(0)
                        dRow("Description") = itemArray(1)
                        DtDescriptionEntries.Rows.Add(dRow)
                        loopCount = loopCount + 1
                    Loop
                End With

            End If

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBUT-WS130R"
                .HasError = True
            End With
        End Try

        Return err


    End Function

    Private Function AccessDatabaseMD501S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtDescriptionsEntries As New DataTable("DescriptionsEntries")
        ResultDataSet.Tables.Add(DtDescriptionsEntries)

        Try
            Dim cmd As iDB2Command = conTALENTTKT.CreateCommand()

            cmd.CommandText = "Call MD501S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7, @PARAM8, @PARAM9, @PARAM10)"
            cmd.CommandType = CommandType.Text

            Dim keys As String() = DescriptionKey.Split(",")
            Dim parmIO_0 As iDB2Parameter
            parmIO_0 = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 4)
            parmIO_0.Value = ""
            If keys.Length > 0 Then parmIO_0.Value = keys(0).ToString
            parmIO_0.Direction = ParameterDirection.Input

            Dim parmIO_1 As iDB2Parameter
            parmIO_1 = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 4)
            parmIO_1.Value = ""
            If keys.Length > 1 Then parmIO_1.Value = keys(1).ToString
            parmIO_1.Direction = ParameterDirection.Input

            Dim parmIO_2 As iDB2Parameter
            parmIO_2 = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 4)
            parmIO_2.Value = ""
            If keys.Length > 2 Then parmIO_2.Value = keys(2).ToString
            parmIO_2.Direction = ParameterDirection.Input

            Dim parmIO_3 As iDB2Parameter
            parmIO_3 = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 4)
            parmIO_3.Value = ""
            If keys.Length > 3 Then parmIO_3.Value = keys(3).ToString
            parmIO_3.Direction = ParameterDirection.Input

            Dim parmIO_4 As iDB2Parameter
            parmIO_4 = cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 4)
            parmIO_4.Value = ""
            If keys.Length > 4 Then parmIO_4.Value = keys(4).ToString
            parmIO_4.Direction = ParameterDirection.Input

            Dim parmIO_5 As iDB2Parameter
            parmIO_5 = cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 4)
            parmIO_5.Value = ""
            If keys.Length > 5 Then parmIO_5.Value = keys(5).ToString
            parmIO_5.Direction = ParameterDirection.Input

            Dim parmIO_6 As iDB2Parameter
            parmIO_6 = cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 4)
            parmIO_6.Value = ""
            If keys.Length > 6 Then parmIO_6.Value = keys(6).ToString
            parmIO_6.Direction = ParameterDirection.Input


            Dim parmIO_7 As iDB2Parameter
            parmIO_7 = cmd.Parameters.Add(Param7, iDB2DbType.iDB2Char, 4)
            parmIO_7.Value = ""
            If keys.Length > 7 Then parmIO_7.Value = keys(7).ToString
            parmIO_7.Direction = ParameterDirection.Input

            Dim parmIO_8 As iDB2Parameter
            parmIO_8 = cmd.Parameters.Add(Param8, iDB2DbType.iDB2Char, 4)
            parmIO_8.Value = ""
            If keys.Length > 8 Then parmIO_8.Value = keys(8).ToString
            parmIO_8.Direction = ParameterDirection.Input

            Dim parmIO_9 As iDB2Parameter
            parmIO_9 = cmd.Parameters.Add(Param9, iDB2DbType.iDB2Char, 4)
            parmIO_9.Value = ""
            If keys.Length > 9 Then parmIO_9.Value = keys(9).ToString
            parmIO_9.Direction = ParameterDirection.Input

            Dim parmIO_10 As iDB2Parameter
            parmIO_10 = cmd.Parameters.Add(Param10, iDB2DbType.iDB2Char, 10)
            parmIO_10.Value = ""
            If keys.Length > 10 Then parmIO_10.Value = keys(10).ToString
            parmIO_10.Direction = ParameterDirection.InputOutput

            Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
            cmdAdapter.SelectCommand = cmd
            cmdAdapter.Fill(ResultDataSet, "DescriptionsEntries")

            Dim drStatus As DataRow = DtStatusResults.NewRow
            If CStr(cmd.Parameters(1).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = CStr(cmd.Parameters(1).Value).Trim
                drStatus("ReturnCode") = "E"
            Else
                drStatus("ErrorOccurred") = ""
                drStatus("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(drStatus)


        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBUT-MD501S"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallWS130R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS130R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS130Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS130R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS130R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS130Parm() As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength("", 5106) & _
                 Utilities.FixStringLength(_descriptionKey, 4) & _
                 Utilities.PadLeadingZeros("0", 6) & _
                 Utilities.FixStringLength(Settings.OriginatingSourceCode, 1) & _
                 Utilities.FixStringLength("", 3)


        Return myString

    End Function

    Private Function AccessDatabaseWS131R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("Success", GetType(String))
            .Add("Email", GetType(String))
        End With


        Try

            'Call WS131R 
            PARAMOUT = CallWS131R()

            'Set the status data table
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(3069, 1) = "E" Or PARAMOUT.Substring(3070, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(5117, 2).Trim
                dRow("Success") = "N"
                dRow("Email") = ""
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                dRow("Success") = PARAMOUT.Substring(3067, 1)
                dRow("Email") = PARAMOUT.Substring(3007, 60)
            End If
            DtStatusResults.Rows.Add(dRow)

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBUT-WS131R"
                .HasError = True
            End With
        End Try

        Return err


    End Function

    Private Function CallWS131R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS131R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS131Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        Return PARAMOUT

    End Function
    Private Function WS131Parm() As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.PadLeadingZeros(UtilityCode, 2) & _
                    Utilities.PadLeadingZeros(CustomerNumber, 12) & _
                    Utilities.PadLeadingZeros(PaymentRef.ToString, 15) & _
                    Utilities.FixStringLength("", 3039) & _
                    Utilities.FixStringLength(Settings.OriginatingSourceCode, 1) & _
                    Utilities.FixStringLength("", 3)

        Return myString

    End Function


End Class
