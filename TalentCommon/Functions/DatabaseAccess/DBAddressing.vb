Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Addressing interface to Hopewiser (& QAS?)
'
'       Date                        January 2008
'
'       Author                      Andrew Green
'
'       � CS Group 2006             All rights reserved.
'                                    
'       Error Number Code base      TACDBAD- 
'                                   
'       Modification Summary
'
'       dd/mm/yy    By              Description
'       --------    ---             -----------
'       '
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBAddressing
    Inherits DBAccess

    Private _de As New DeAddress
    Private Const AddressList As String = "AddressList"
    Private Const PrintAddressLabel As String = "PrintAddressLabel"

    Public Property De() As DeAddress
        Get
            Return _de
        End Get
        Set(ByVal value As DeAddress)
            _de = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj

        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = PrintAddressLabel
                err = AccessDatabaseWS147R()
            Case Is = AddressList
                Select Case De.AddressingProvider
                    Case Is = "Hopewiser"
                        err = AccessDatabaseWS802R()
                End Select
        End Select

        Return err

    End Function

    Private Function AccessDatabaseWS802R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim sAddress As String = String.Empty
        Dim intStart As Integer = 0
        Dim intCount As Integer = 0

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the payment Details data table
        Dim DtPaymentResults As New DataTable
        ResultDataSet.Tables.Add(DtPaymentResults)
        With DtPaymentResults.Columns
            .Add("Company", GetType(String))
            .Add("Address1", GetType(String))
            .Add("Address2", GetType(String))
            .Add("Town", GetType(String))
            .Add("County", GetType(String))
            .Add("PostCode1", GetType(String))
            .Add("PostCode2", GetType(String))
            .Add("Country", GetType(String))
        End With

        Try

            'Call WS007R
            PARAMOUT = CallWS802R()

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(20479, 1) = "E" Or PARAMOUT.Substring(20477, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(20477, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            'No errors 
            If PARAMOUT.Substring(20479, 1) <> "E" And PARAMOUT.Substring(20477, 2).Trim = "" Then

                Do While intCount < 100
                    intStart = intCount * 168
                    sAddress = PARAMOUT.Substring(intStart, 168)
                    If sAddress.Trim <> "" Then
                        'Create a new row
                        dRow = Nothing
                        dRow = DtPaymentResults.NewRow
                        dRow("Address1") = sAddress.Substring(0, 30)
                        dRow("Address2") = sAddress.Substring(30, 30)
                        dRow("Town") = sAddress.Substring(60, 30)
                        dRow("PostCode1") = sAddress.Substring(90, 4)
                        dRow("PostCode2") = sAddress.Substring(94, 4)
                        dRow("County") = sAddress.Substring(98, 30)
                        dRow("Company") = sAddress.Substring(128, 40)
                        dRow("Country") = De.Country
                        DtPaymentResults.Rows.Add(dRow)
                    End If
                    intCount = intCount + 1
                Loop

            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBAD-01"
                .HasError = True
            End With
        End Try

        Return err

    End Function
    Public Function AccessDatabaseWS802RTest() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the payment Details data table
        Dim DtPaymentResults As New DataTable
        ResultDataSet.Tables.Add(DtPaymentResults)
        With DtPaymentResults.Columns
            .Add("Company", GetType(String))
            .Add("Address1", GetType(String))
            .Add("Address2", GetType(String))
            .Add("Town", GetType(String))
            .Add("County", GetType(String))
            .Add("PostCode1", GetType(String))
            .Add("PostCode2", GetType(String))
            .Add("Country", GetType(String))
        End With


        'Set the response data
        dRow = Nothing
        dRow = DtStatusResults.NewRow
        dRow("ErrorOccurred") = ""
        dRow("ReturnCode") = ""
        DtStatusResults.Rows.Add(dRow)

        dRow = Nothing
        dRow = DtPaymentResults.NewRow
        dRow("Address1") = "1 Frankby Close"
        dRow("Address2") = ""
        dRow("Town") = "Greasby"
        dRow("PostCode1") = "CH49"
        dRow("PostCode2") = "3PT"
        dRow("County") = "Wirral"
        dRow("Company") = ""
        dRow("Country") = "United Kingdom"
        DtPaymentResults.Rows.Add(dRow)
        dRow = Nothing
        dRow = DtPaymentResults.NewRow
        dRow("Address1") = "2 Frankby Close"
        dRow("Address2") = ""
        dRow("Town") = "Greasby"
        dRow("PostCode1") = "CH49"
        dRow("PostCode2") = "3PT"
        dRow("County") = "Wirral"
        dRow("Company") = ""
        dRow("Country") = "United Kingdom"
        DtPaymentResults.Rows.Add(dRow)
        dRow = Nothing
        dRow = DtPaymentResults.NewRow
        dRow("Address1") = "3 Frankby Close"
        dRow("Address2") = ""
        dRow("Town") = "Greasby"
        dRow("PostCode1") = "CH49"
        dRow("PostCode2") = "3PT"
        dRow("County") = "Wirral"
        dRow("Company") = ""
        dRow("Country") = "United Kingdom"
        DtPaymentResults.Rows.Add(dRow)
        dRow = Nothing
        dRow = DtPaymentResults.NewRow
        dRow("Address1") = "4 Frankby Close"
        dRow("Address2") = ""
        dRow("Town") = "Greasby"
        dRow("PostCode1") = "CH49"
        dRow("PostCode2") = "3PT"
        dRow("County") = "Wirral"
        dRow("Company") = ""
        dRow("Country") = "United Kingdom"
        DtPaymentResults.Rows.Add(dRow)
        dRow = Nothing
        dRow = DtPaymentResults.NewRow
        dRow("Address1") = "5 Frankby Close"
        dRow("Address2") = ""
        dRow("Town") = "Greasby"
        dRow("PostCode1") = "CH49"
        dRow("PostCode2") = "3PT"
        dRow("County") = "Wirral"
        dRow("Company") = ""
        dRow("Country") = "United Kingdom"
        DtPaymentResults.Rows.Add(dRow)
        dRow = Nothing
        dRow = DtPaymentResults.NewRow
        dRow("Address1") = "6 Frankby Close"
        dRow("Address2") = ""
        dRow("Town") = "Greasby"
        dRow("PostCode1") = "CH49"
        dRow("PostCode2") = "3PT"
        dRow("County") = "Wirral"
        dRow("Company") = ""
        dRow("Country") = "United Kingdom"
        DtPaymentResults.Rows.Add(dRow)
        dRow = Nothing
        dRow = DtPaymentResults.NewRow
        dRow("Address1") = "7 Frankby Close"
        dRow("Address2") = ""
        dRow("Town") = "Greasby"
        dRow("PostCode1") = "CH49"
        dRow("PostCode2") = "3PT"
        dRow("County") = "Wirral"
        dRow("Company") = ""
        dRow("Country") = "United Kingdom"
        DtPaymentResults.Rows.Add(dRow)
        dRow = Nothing
        dRow = DtPaymentResults.NewRow
        dRow("Address1") = "8 Frankby Close"
        dRow("Address2") = ""
        dRow("Town") = "Greasby"
        dRow("PostCode1") = "CH49"
        dRow("PostCode2") = "3PT"
        dRow("County") = "Wirral"
        dRow("Company") = ""
        dRow("Country") = "United Kingdom"
        DtPaymentResults.Rows.Add(dRow)
        dRow = Nothing
        dRow = DtPaymentResults.NewRow
        dRow("Address1") = "9 Frankby Close"
        dRow("Address2") = ""
        dRow("Town") = "Greasby"
        dRow("PostCode1") = "CH49"
        dRow("PostCode2") = "3PT"
        dRow("County") = "Wirral"
        dRow("Company") = ""
        dRow("Country") = "United Kingdom"
        DtPaymentResults.Rows.Add(dRow)
        dRow = Nothing
        dRow = DtPaymentResults.NewRow
        dRow("Address1") = "10 Frankby Close"
        dRow("Address2") = ""
        dRow("Town") = "Greasby"
        dRow("PostCode1") = "CH49"
        dRow("PostCode2") = "3PT"
        dRow("County") = "Wirral"
        dRow("Company") = ""
        dRow("Country") = "United Kingdom"
        DtPaymentResults.Rows.Add(dRow)

        Return err

    End Function

    Private Function CallWS802R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS802R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 20480)
        parmIO.Value = WS802Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        Return PARAMOUT

    End Function

    Private Function WS802Parm() As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength("", 20444) & _
                 Utilities.FixStringLength(De.PostalCode, 30) & "      "

        Return myString

    End Function
    Public Function AccessDatabaseWS147R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        PARAMOUT = CallWS147R()

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
        Return err
    End Function
    Private Function CallWS147R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS147R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)

        parmIO.Value = Utilities.FixStringLength(De.CustomerNumber, 12) & _
                       Utilities.FixStringLength(Settings.OriginatingSource, 10)
        parmIO.Direction = ParameterDirection.InputOutput
        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        Return PARAMOUT

    End Function
End Class

