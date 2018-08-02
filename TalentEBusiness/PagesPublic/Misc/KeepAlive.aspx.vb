Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    KeepAlive.aspx. Monitor the SQL database is running and 
'                                   the iSeries can be connected to. Output appropriate messages
'
'       Date                        10/07/08
'
'       Author                      Ben Ford
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
Partial Class PagesPublic_Misc_KeepAlive
    Inherits System.Web.UI.Page

    Private _sqlStatus As String = "Y"
    Private _iSeriesStatus As String = "Y"
    Private _noiseStatus As String = "Y"
    Private _lastActivityCountActual As Integer = 0

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim sqlOK As Boolean = False
        Dim iSeriesOK As Boolean = False
        Dim checkBackEnd As Boolean = True
        '-----------------------------------------------------------
        ' Check query string for whether or not to check the backend
        '-----------------------------------------------------------
        If Not Request("nobackend") Is Nothing AndAlso Request("nobackend") = "Y" Then
            checkBackEnd = False
        End If
        '---------------------
        ' Check SQL connection
        '---------------------
        Dim err As New Talent.Common.ErrorObj
        Const SelectStr As String = "SELECT * FROM tbl_bu WITH (NOLOCK)  "

        Dim cmd As New SqlCommand(SelectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))

        Dim ta As New SqlDataAdapter(cmd)
        Dim dtBusinessUnits As New Data.DataTable

        Try
            cmd.Connection.Open()

            ta.Fill(dtBusinessUnits)
            If dtBusinessUnits.Rows.Count > 0 Then
                sqlOK = True
            End If

        Catch ex As Exception
        Finally
            cmd.Connection.Close()
            cmd.Dispose()
            ta.Dispose()
        End Try

        If checkBackEnd AndAlso sqlOK Then
            '--------------------------
            ' Check Back End connection
            '-------------=------------
            Dim utility As New Talent.Common.TalentUtiltities
            utility.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            utility.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            utility.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            utility.Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString

            err = utility.CheckBackEndDatabase
            If Not err.HasError Then
                iSeriesOK = True
            Else
                _iSeriesStatus = "N"
            End If
        End If

        CheckNoiseSessions()

        '---------------
        ' Display result
        '---------------
        If iSeriesOK AndAlso sqlOK Then
            lblResult.Text = "EBUSINESS WEBSITE UP BOTH CONNECTIONS"
            _sqlStatus = "Y"
            _iSeriesStatus = "Y"
        Else
            If sqlOK Then
                lblResult.Text = "EBUSINESS WEBSITE UP NO BACKEND"
                _sqlStatus = "Y"
            Else
                lblResult.Text = "EBUSINESS WEBSITE DOWN"
                _sqlStatus = "N"
            End If
        End If
        DisplayMessage()
    End Sub
    Private Sub CheckNoiseSessions()

        If (Not Request("nonoise") Is Nothing) AndAlso Request("nonoise") = "Y" Then
            _noiseStatus = "Y"
        Else
            Dim noiseLastActivitytMinutes As Integer = 10
            Dim lastActivityCountAllowed As Integer = 0

            If (Not Request("lastactivityminutesabove") Is Nothing) AndAlso IsNumeric(Request("lastactivityminutesabove")) Then
                noiseLastActivitytMinutes = CInt(Request("lastactivityminutesabove"))
            End If
            If (Not Request("lastactivitycountallowed") Is Nothing) AndAlso IsNumeric(Request("lastactivitycountallowed")) Then
                lastActivityCountAllowed = CInt(Request("lastactivitycountallowed"))
            End If
            Const SelectStr As String = "  SELECT COUNT(ID) FROM tbl_active_noise_sessions WITH (NOLOCK) " & _
                                        "  WHERE DATEDIFF(minute,LAST_ACTIVITY,GETDATE())> @LastActivityMinutes"

            Dim cmd As New SqlCommand(SelectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            cmd.Parameters.Add(New SqlParameter("@LastActivityMinutes", noiseLastActivitytMinutes))

            Dim ta As New SqlDataAdapter(cmd)
            Dim dtActiveNoise As New Data.DataTable

            Try
                cmd.Connection.Open()

                ta.Fill(dtActiveNoise)
                If dtActiveNoise.Rows.Count > 0 Then
                    _lastActivityCountActual = CInt(dtActiveNoise.Rows(0)(0))
                End If
                If _lastActivityCountActual > lastActivityCountAllowed Then
                    _noiseStatus = "N"
                Else
                    _noiseStatus = "Y"
                End If

            Catch ex As Exception
                _noiseStatus = "N"
            Finally
                cmd.Connection.Close()
                cmd.Dispose()
                ta.Dispose()
            End Try
        End If
    End Sub

    Private Sub DisplayMessage()
        If (Not Request("version") Is Nothing) AndAlso Request("version") = "2" Then
            Dim status As String = "SQL=" & _sqlStatus & ",TKT=" & _iSeriesStatus & ",NOISE=" & _noiseStatus
            lblVersionResult.Text = "$V2START$" & status & "$V2END$"
            lblVersionResult.Text = lblVersionResult.Text & " " & GetCustomMessage(status)
        End If
    End Sub

    Private Function GetCustomMessage(ByVal displayMessage As String) As String
        Dim customMessage As String = "$MESSAGESTART$"
        If Not Request("nobackend") Is Nothing Then
            customMessage = customMessage & "nobackend = " & Request("nobackend") & " : "
        End If
        If (Not Request("nonoise") Is Nothing) Then
            customMessage = customMessage & "  nonoise = " & Request("nonoise") & " : "
        Else
        End If
        If (Not Request("lastactivityminutesabove") Is Nothing) Then
            customMessage = customMessage & "  lastactivityminutesabove = " & Request("lastactivityminutesabove") & " : "
        End If
        If (Not Request("lastactivitycountallowed") Is Nothing) Then
            customMessage = customMessage & "  lastactivitycountallowed = " & Request("lastactivitycountallowed") & " : "
        End If
        customMessage = customMessage & "  lastactivitycountactual = " & _lastActivityCountActual.ToString
        customMessage = customMessage & " : " & displayMessage & "$MESSAGEEND$"
        Return customMessage
    End Function
End Class
