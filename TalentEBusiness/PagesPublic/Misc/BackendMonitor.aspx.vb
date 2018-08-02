Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    ClientBackendMonitor.aspx. Validate the status of the backend
'
'       Date                        01/03/11
'
'       Author                      Des Webster
'
'       @ CS Group 2007             All rights reserved.
'
'       Error Number Code base      
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       
'--------------------------------------------------------------------------------------------------

Partial Class PagesPublic_Misc_BackendMonitor
    Inherits Base01
    Private conTALENTTKT As iDB2Connection = Nothing
    Private BackEndConnectionString As String = ""
    Private checkMessages As String = "Y"
    Private killMessages As String = "Y"
    Private jobName As String = "QZDASOINIT"
    Private subSystem As String = "*ALL"
    Private ModuleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults

    Private Function RetrieveBackendConnString() As String
        Dim connString As String = ""
        Try
            '---------------------------------------
            ' Retieve Talent admin connection string
            '---------------------------------------
            Const SelectStr As String = "SELECT * FROM tbl_database_version WITH (NOLOCK)  " & _
                                            " where business_unit = @businessUnit " & _
                                            " and partner = @partner " & _
                                            " and destination_database = @desinationDatabase "

            Dim cmd As New SqlCommand(SelectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))

            Dim ta As New SqlDataAdapter(cmd)
            Dim dt As New Data.DataTable

            Try
                cmd.Connection.Open()
                With cmd.Parameters
                    .Add("@businessUnit", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit()
                    .Add("@partner", SqlDbType.NVarChar).Value = "*ALL"
                    .Add("@desinationDatabase", SqlDbType.NVarChar).Value = "TALENTTKT"
                End With

                ta.Fill(dt)
                If dt.Rows.Count > 0 Then
                    connString = dt.Rows(0).Item("CONNECTION_STRING")
                End If

            Catch ex As Exception
            Finally
                cmd.Connection.Close()
                cmd.Dispose()
                ta.Dispose()
            End Try
        Catch ex As Exception
        End Try
        Return connString
    End Function

    Protected Function TALENTTKTOpen() As Boolean
        '---------------------------------------------------------------------------------
        '   Attempt to open database
        '
        TALENTTKTOpen = True
        Try
            If conTALENTTKT Is Nothing Then
                conTALENTTKT = New iDB2Connection(BackEndConnectionString)
                conTALENTTKT.Open()
            ElseIf conTALENTTKT.State <> ConnectionState.Open Then
                conTALENTTKT = New iDB2Connection(BackEndConnectionString)
                conTALENTTKT.Open()
            End If
        Catch ex As Exception
            TALENTTKTOpen = False
        End Try
        Return TALENTTKTOpen
    End Function

    Protected Sub TALENTTKTClose()
        Try
            If Not (conTALENTTKT Is Nothing) Then
                If (conTALENTTKT.State = ConnectionState.Open) Then
                    conTALENTTKT.Close()
                End If
            End If
            conTALENTTKT = Nothing
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            'Please note that we not using talent common, as we need to improve our monitoring without upgrading sites.

            'Retrieve the connection string
            BackEndConnectionString = RetrieveBackendConnString()
            If BackEndConnectionString.Trim.Equals("") Then
                lblResult.Text = "NO CONNECTION STRING - TALENTTKT"
                Exit Sub
            End If

            'Open the connection
            If Not TALENTTKTOpen() Then
                lblResult.Text = "CANNOT OPEN CONNECTION"
                Exit Sub
            End If

            'Retrireve the query string paramaters
            If Not (Request("checkMessages") Is Nothing) Then checkMessages = Request("checkMessages")
            If Not (Request("killMessages") Is Nothing) Then killMessages = Request("killMessages")
            If Not (Request("jobName") Is Nothing) Then jobName = Request("jobName")
            If Not (Request("subSystem") Is Nothing) Then subSystem = Request("subSystem")

            'Check backend database
            lblResult.Text = CheckBackEndDatabase()

        Catch ex As Exception
            lblResult.Text = "MONITOR EXCEPTION - " & ex.Message
        Finally
            'Close the connection
            TALENTTKTClose()
        End Try

    End Sub

    Protected Function CheckBackEndDatabase() As String
        CheckBackEndDatabase = ""
        Try
            Dim cmdSELECT As iDB2Command = Nothing
            Dim strProgram As String = "WS114R"
            Dim strHEADER As String = "CALL " & ModuleDefaults.StoredProcedureGroup.Trim & _
                                        "/" & strProgram & "(@PARAM1)"
            Dim parmIO As iDB2Parameter
            Dim PARAMOUT As String = String.Empty

            'Set the connection string
            cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

            'Populate the parameter
            parmIO = cmdSELECT.Parameters.Add("@Param1", iDB2DbType.iDB2Char, 1024)
            parmIO.Value = Talent.Common.Utilities.FixStringLength(checkMessages, 1) &
                                Talent.Common.Utilities.FixStringLength(killMessages, 1) &
                                Talent.Common.Utilities.FixStringLength(jobName, 10) &
                                Talent.Common.Utilities.FixStringLength(subSystem, 10) &
                                Talent.Common.Utilities.FixStringLength(" ", 1002)
            parmIO.Direction = ParameterDirection.InputOutput

            'Execute
            cmdSELECT.ExecuteNonQuery()

            PARAMOUT = cmdSELECT.Parameters("@Param1").Value.ToString
            If PARAMOUT.Substring(1023, 1) = "Y" AndAlso PARAMOUT.Substring(1018, 1) = "N" AndAlso PARAMOUT.Substring(1019, 4) = "0000" Then
                CheckBackEndDatabase = "TICKETING SERVER IS OK"
            ElseIf PARAMOUT.Substring(1023, 1) = "Y" AndAlso PARAMOUT.Substring(1018, 1) = "N" AndAlso PARAMOUT.Substring(1019, 4) <> "0000" Then
                CheckBackEndDatabase = "TICKETING SERVER HAD " & PARAMOUT.Substring(1019, 4) & " MESSAGE WAITS"
            ElseIf PARAMOUT.Substring(1023, 1) = "Y" AndAlso PARAMOUT.Substring(1018, 1) = "Y" Then
                CheckBackEndDatabase = "TICKETING SERVER IS OK BUT AN ERROR OCCURRED RETRIEVING MESSAGES"
            Else
                CheckBackEndDatabase = "TICKETING SERVER IS DOWN - NO EXCEPTION"
            End If
        Catch ex As Exception
            CheckBackEndDatabase = "TICKETING SERVER IS DOWN - " & ex.Message
        End Try
        Return CheckBackEndDatabase
    End Function
End Class
