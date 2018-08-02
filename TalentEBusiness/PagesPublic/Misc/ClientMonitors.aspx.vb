Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    ClientMonitors.aspx. Validate the status of the client monitors
'
'       Date                        15/02/11
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
Partial Class PagesPublic_Misc_ClientMonitors
    Inherits Base01

    Private ClientName As String = ""
    Private connStringTA As String = ""
    Private monitorsFound As Boolean = False

    Structure Monitor
        Dim Name As String
        Dim Time As Integer
    End Structure

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            'Client Must be passed in 
            If Request("client") Is Nothing Then
                lblResult.Text = "NO CLIENT"
                Exit Sub
            Else
                ClientName = Request("client")
            End If

            'Retrieve the talent admin database string
            connStringTA = RetrieveAdminConnString()
            If connStringTA.Trim.Equals("") Then
                lblResult.Text = "NO CONNECTION STRING - TALENT ADMIN"
                Exit Sub
            End If

            'Retrieve a list of monitors
            Dim monValid As String = ""
            Dim monInValid As String = ""
            Dim monList As Generic.List(Of Monitor) = RetrieveMonitors()
            If Not monitorsFound Then
                lblResult.Text = "NO MONITORS SETUP - " & ClientName
                Exit Sub
            Else
                For Each mon As Monitor In monList
                    If MonitorValid(mon) Then
                        monValid += mon.Name & ";"
                    Else
                        monInValid += mon.Name & ";"
                    End If
                Next
            End If

            'Send the status message
            If monInValid.Trim.Equals("") Then
                lblResult.Text = "MONITORS WORKING - " & ClientName & " - " & monValid
            Else
                lblResult.Text = "MONITORS NOT WORKING - " & ClientName & " - " & monInValid
            End If
        Catch ex As Exception
            lblResult.Text = "MONITOR EXCEPTION - " & ex.Message
        End Try

    End Sub

    Private Function RetrieveAdminConnString() As String
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
                    .Add("@desinationDatabase", SqlDbType.NVarChar).Value = "TALENTADMIN"
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


    Private Function RetrieveMonitors() As Generic.List(Of Monitor)
        Dim monList As New Generic.List(Of Monitor)
        Try

            Dim selectStr As String = "SELECT cm.MODULE, cm.CLIENT_NAME, cm.AUTO_PROCESS_IN_MINUTES, cm.PROCESS_START_TIME, cm.PROCESS_END_TIME, cm.LIVE_OR_TEST " & _
                                      "FROM tbl_client_modules cm " & _
                                      "WHERE cm.client_name = @clientName and cm.AUTO_PROCESS = @autoProcess and cm.LIVE_OR_TEST = @liveOrTest"


            Dim cmd As New SqlCommand(selectStr, New SqlConnection(connStringTA))

            Dim ta As New SqlDataAdapter(cmd)
            Dim dt As New Data.DataTable

            Try
                cmd.Connection.Open()
                With cmd.Parameters
                    .Add("@clientName", SqlDbType.NVarChar).Value = ClientName
                    .Add("@autoProcess", SqlDbType.Bit).Value = True
                    .Add("@liveOrTest", SqlDbType.NVarChar).Value = "Live"
                End With

                ta.Fill(dt)
                For Each dr As DataRow In dt.Rows
                    If VersionValid(dr("MODULE")) Then
                        monitorsFound = True
                        If Not DownTime(dr("PROCESS_START_TIME"), dr("PROCESS_END_TIME")) Then
                            Dim mon As New Monitor
                            mon.Name = dr("MODULE")
                            mon.Time = dr("AUTO_PROCESS_IN_MINUTES")
                            monList.Add(mon)
                        End If
                    End If
                Next

            Catch ex As Exception
            Finally
                cmd.Connection.Close()
                cmd.Dispose()
                ta.Dispose()
            End Try
        Catch ex As Exception
        End Try
        Return monList
    End Function

    Private Function VersionValid(ByVal monitor As String) As Boolean
        VersionValid = False
        Try

            Dim selectStr As String = "SELECT * from tbl_client_versions cv " & _
                                      "INNER JOIN (SELECT TOP 1 * FROM tbl_module_version_control WHERE MODULE = @module ORDER BY CAST(MODULE_VERSION AS INT) ASC) mvc on mvc.module = @module " & _
                                      "WHERE ( CAST(cv.VERSION AS INT) > CAST(mvc.VERSION AS INT) " & _
                                      "OR (CAST(cv.VERSION AS INT) = CAST(mvc.VERSION AS INT) AND CAST(cv.RELEASE AS INT) > CAST(mvc.RELEASE AS INT)) " & _
                                      "OR (CAST(cv.VERSION AS INT) = CAST(mvc.VERSION AS INT) AND CAST(cv.RELEASE AS INT) = CAST(mvc.RELEASE AS INT) AND CAST(cv.BUILD AS INT) >= CAST(mvc.BUILD AS INT)) " & _
                                      ") " & _
                                      "AND cv.client_name = @clientName and cv.LIVE_OR_TEST = @liveOrTest"


            Dim cmd As New SqlCommand(selectStr, New SqlConnection(connStringTA))

            Dim ta As New SqlDataAdapter(cmd)
            Dim dt As New Data.DataTable

            Try
                cmd.Connection.Open()
                With cmd.Parameters
                    .Add("@clientName", SqlDbType.NVarChar).Value = ClientName
                    .Add("@module", SqlDbType.NVarChar).Value = monitor
                    .Add("@liveOrTest", SqlDbType.NVarChar).Value = "Live"
                End With

                ta.Fill(dt)
                If dt.Rows.Count > 0 Then
                    VersionValid = True
                End If

            Catch ex As Exception
            Finally
                cmd.Connection.Close()
                cmd.Dispose()
                ta.Dispose()
            End Try
        Catch ex As Exception
        End Try
        Return VersionValid
    End Function

    Private Function MonitorValid(ByVal mon As Monitor) As Boolean
        MonitorValid = False
        Try

            Dim selectStr As String = "SELECT Top 1 * from tbl_log_header " & _
                                      "WHERE client_name = @clientName and source_class = @module and is_live = @live and timestamp > @timestamp"


            Dim cmd As New SqlCommand(selectStr, New SqlConnection(connStringTA))

            Dim ta As New SqlDataAdapter(cmd)
            Dim dt As New Data.DataTable

            Try
                cmd.Connection.Open()
                With cmd.Parameters
                    .Add("@clientName", SqlDbType.NVarChar).Value = ClientName
                    .Add("@module", SqlDbType.NVarChar).Value = mon.Name
                    .Add("@live", SqlDbType.Bit).Value = True
                    .Add("@timestamp", SqlDbType.DateTime).Value = Now.AddMinutes(-(mon.Time + 5))
                End With

                ta.Fill(dt)
                If dt.Rows.Count > 0 Then
                    MonitorValid = True
                End If

            Catch ex As Exception
            Finally
                cmd.Connection.Close()
                cmd.Dispose()
                ta.Dispose()
            End Try
        Catch ex As Exception
        End Try
        Return MonitorValid
    End Function

    Private Function DownTime(ByVal ProcessStartTime As Integer, ByVal ProcessEndTime As Integer) As Boolean

        DownTime = False

        'Has down time been specified
        If Not (ProcessStartTime = 0 And ProcessEndTime = 0) Then

            ' Calculate the current time as an integer
            Dim currentTime As Integer = (Now.Hour * 10000) + (Now.Minute * 100) + (Now.Second)

            'Is the end time on a different day than the start time
            If ProcessEndTime < ProcessStartTime Then

                'Are we in a downtime period 
                If currentTime > ProcessEndTime AndAlso currentTime < ProcessStartTime Then
                    DownTime = True
                End If
            Else
                'Are we in a downtime period 
                If currentTime < ProcessStartTime Or currentTime > ProcessEndTime Then
                    DownTime = True
                End If
            End If

            'We need to give the monitors a bit of grace if it's just started
            If Not DownTime AndAlso currentTime > ProcessStartTime AndAlso (currentTime - ProcessStartTime) < 10 Then
                DownTime = True
            End If

        End If
        Return DownTime

    End Function

End Class
