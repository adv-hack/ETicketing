Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Xml


Partial Class UserControls_DBAdmin
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        feedbackList.Items.Clear()
        If Not Page.IsPostBack Then
            xmlPathBox.Text = (New TalentDefaults).DBAdminXmlPath
            PopulateLiveOrTestDDL()
            PopulateClientsDDL()
        End If
    End Sub

    Protected Sub PopulateLiveOrTestDDL()

        liveOrTestDDL.Items.Clear()
        liveOrTestDDL.Items.Add("Live")
        liveOrTestDDL.Items.Add("Test")
        liveOrTestDDL.SelectedIndex = 0

    End Sub

    Protected Sub PopulateClientsDDL()

        Dim dt As DataTable = (New TalentDefaults).AllClients

        If dt.Rows.Count > 0 Then
            With clientNameDDL
                .DataSource = dt
                .DataTextField = "CLIENT_NAME"
                .DataValueField = "CLIENT_NAME"
                .DataBind()
                clientNameDDL_SelectedIndexChanged(New Object, New EventArgs)
            End With
        End If

    End Sub

    Protected Sub clientNameDDL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles clientNameDDL.SelectedIndexChanged

        Dim dt As DataTable = (New TalentDefaults).ClientsWithVersion(liveOrTestDDL.SelectedValue)

        If dt.Rows.Count > 0 Then
            Dim filteredClients As DataRow() = dt.Select("CLIENT_NAME = '" & clientNameDDL.SelectedValue & "'")

            If filteredClients.Length > 0 Then
                Dim dr As DataRow = filteredClients(0)

                With Me
                    .buBox.Text = Convert.ToString(dr("DEFAULT_BUSINESS_UNIT"))
                    .partnerBox.Text = Convert.ToString(dr("DEFAULT_PARTNER"))
                    .LangBox.Text = Convert.ToString(dr("DEFAULT_LANGUAGE"))
                    .verBox.Text = Convert.ToString(dr("VERSION_NO"))
                    .subVerBox.Text = Convert.ToString(dr("SUB_VERSION_NO"))
                    .ptfVerBox.Text = Convert.ToString(dr("PTF_NO"))
                    .clientVerBox.Text = Convert.ToString(dr("CLIENT_SPECIFIC_NO"))

                    .newVerBox.Text = Convert.ToString(dr("VERSION_NO"))
                    .newSubVerBox.Text = Convert.ToString(dr("SUB_VERSION_NO"))
                    .newPtfVerBox.Text = Convert.ToString(dr("PTF_NO"))
                    .newClientVerBox.Text = Convert.ToString(dr("CLIENT_SPECIFIC_NO"))
                End With

                PopulateServersList()
            Else
                ClientServersRepeater1.DataBind()
                With Me
                    .buBox.Text = ""
                    .partnerBox.Text = ""
                    .LangBox.Text = ""
                    .verBox.Text = ""
                    .subVerBox.Text = ""
                    .ptfVerBox.Text = ""
                    .clientVerBox.Text = ""

                    .newVerBox.Text = ""
                    .newSubVerBox.Text = ""
                    .newPtfVerBox.Text = ""
                    .newClientVerBox.Text = ""
                End With
                feedbackList.Items.Add("No data could be found for client '" & clientNameDDL.SelectedValue & "'")
            End If

        End If


    End Sub

    Protected Sub PopulateServersList()
        Dim dt As DataTable = (New TalentDefaults).SpecificClientServerLinks(clientNameDDL.SelectedValue, liveOrTestDDL.SelectedValue)
        If dt.Rows.Count > 0 Then
            ClientServersRepeater1.DataSource = dt
            ClientServersRepeater1.DataBind()
        Else
            ClientServersRepeater1.DataBind()
        End If

    End Sub

    Protected Sub ClientServersRepeater1_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles ClientServersRepeater1.ItemDataBound

        If e.Item.ItemIndex <> -1 Then

            Dim dr As DataRow = CType(e.Item.DataItem, DataRowView).Row

            Dim csLabel As Label = CType(e.Item.FindControl("csLabel"), Label)
            Dim csConnStr As Label = CType(e.Item.FindControl("csConnStr"), Label)
            Dim csCheck As CheckBox = CType(e.Item.FindControl("csCheck"), CheckBox)

            csLabel.Text = Convert.ToString(dr("SERVER_NAME"))
            csCheck.Checked = True

            Dim server As String = Convert.ToString(dr("SERVER_NAME")).ToUpper
            If Not String.IsNullOrEmpty(Convert.ToString(dr("SQLSERVER_INSTANCE"))) Then
                server += "\" & Convert.ToString(dr("SQLSERVER_INSTANCE"))
            End If
            Dim database As String = Convert.ToString(dr("SQL_DATABASE_NAME"))
            Dim userId As String = Convert.ToString(dr("SQL_USER"))
            Dim password As String = Convert.ToString(dr("SQL_PASSWORD"))

            csConnStr.Text = String.Format("Data Source={0};Initial Catalog={1}; User ID={2}; password={3};", server, database, userId, password)

        End If

    End Sub

    Protected Function ValidateVersionNumbers() As Boolean

        ValidateVersionNumbers = True

        If String.IsNullOrEmpty(verBox.Text) Then
            feedbackList.Items.Add("Current Version Number is required")
            ValidateVersionNumbers = False
        Else
            Try
                If Not CInt(verBox.Text) > 0 Then
                    feedbackList.Items.Add("Current Version Number must be greater than 0")
                    ValidateVersionNumbers = False
                End If
            Catch ex As Exception
                feedbackList.Items.Add("Current Version Number is not an integer")
                ValidateVersionNumbers = False
            End Try
        End If
        If Not String.IsNullOrEmpty(subVerBox.Text) Then
            Try
                Dim test As Integer = CInt(subVerBox.Text)
            Catch ex As Exception
                feedbackList.Items.Add("Current Sub-Version Number is not an integer")
                ValidateVersionNumbers = False
            End Try
        Else
            subVerBox.Text = 0
        End If
        If String.IsNullOrEmpty(newVerBox.Text) Then
            feedbackList.Items.Add("New Version Number is required")
            ValidateVersionNumbers = False
        Else
            Try
                If Not CInt(newVerBox.Text) > 0 Then
                    feedbackList.Items.Add("New Version Number must be greater than 0")
                    ValidateVersionNumbers = False
                End If
            Catch ex As Exception
                feedbackList.Items.Add("New Version Number is not an integer")
                ValidateVersionNumbers = False
            End Try
        End If
        If Not String.IsNullOrEmpty(newSubVerBox.Text) Then
            Try
                Dim test As Integer = CInt(newSubVerBox.Text)
            Catch ex As Exception
                feedbackList.Items.Add("New Sub-Version Number is not an integer")
                ValidateVersionNumbers = False
            End Try
        Else
            newSubVerBox.Text = 0
        End If
        If Not String.IsNullOrEmpty(ptfVerBox.Text) Then
            Try
                Dim test As Integer = CInt(ptfVerBox.Text)
            Catch ex As Exception
                feedbackList.Items.Add("Current PTF Version Number is not an integer")
                ValidateVersionNumbers = False
            End Try
        Else
            ptfVerBox.Text = 0
        End If
        If Not String.IsNullOrEmpty(newPtfVerBox.Text) Then
            Try
                Dim test As Integer = CInt(newPtfVerBox.Text)
            Catch ex As Exception
                feedbackList.Items.Add("New PTF Version Number is not an integer")
                ValidateVersionNumbers = False
            End Try
        Else
            newPtfVerBox.Text = 0
        End If

        Return ValidateVersionNumbers

    End Function


    Protected Sub btnXmlFile_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnXmlFileSelect.Click
        'Check folder is valid and has files that are usable
        If Not String.IsNullOrEmpty(xmlPathBox.Text) And Directory.Exists(xmlPathBox.Text) Then
            Dim strFiles() As String = Directory.GetFiles(xmlPathBox.Text, "*.xml")
            Dim strSingleFile As String

            ddlXmlFile.Items.Clear()
            Dim liAllFiles As New ListItem
            liAllFiles.Value = "0"
            liAllFiles.Text = "-- All Files --"
            ddlXmlFile.Items.Insert(0, liAllFiles)

            'Populate and Show DDL and allow user to select a file
            For Each strSingleFile In strFiles
                strSingleFile = strSingleFile.Substring(xmlPathBox.Text.Length)
                If strSingleFile.StartsWith("\") Then
                    strSingleFile = strSingleFile.Remove(0, 1)
                End If
                ddlXmlFile.Items.Add(strSingleFile)
            Next
        Else
            ddlXmlFile.Items.Clear()
            feedbackList.Items.Clear()
            feedbackList.Items.Add("The XML Folder Path does not exist")
        End If

        upgradeNotesRepeater.Visible = False

        If ddlXmlFile.Items.Count > 0 Then
            plhXmlFileSelection.Visible = True
        End If
    End Sub

    Protected Sub goButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles goButton.Click

        feedbackList.Items.Clear()
        If String.IsNullOrEmpty(xmlPathBox.Text) Then
            feedbackList.Items.Add("XML folder path is required")
        End If
        If String.IsNullOrEmpty(buBox.Text) Then
            feedbackList.Items.Add("Business Unit is required")
        End If
        If String.IsNullOrEmpty(partnerBox.Text) Then
            feedbackList.Items.Add("Partner is required")
        End If


        If feedbackList.Items.Count = 0 AndAlso ValidateVersionNumbers() Then
            For Each ri As RepeaterItem In Me.ClientServersRepeater1.Items

                Dim csLabel As Label = CType(ri.FindControl("csLabel"), Label)
                Dim csConnStr As Label = CType(ri.FindControl("csConnStr"), Label)
                Dim csCheck As CheckBox = CType(ri.FindControl("csCheck"), CheckBox)

                If csCheck.Checked Then
                    Try
                        Dim dbAdmin As New TalentDBAdmin
                        dbAdmin.ConnectionString = csConnStr.Text
                        dbAdmin.XmlPath = xmlPathBox.Text
                        If ddlXmlFile.SelectedIndex > 0 Then
                            dbAdmin.XmlFile = ddlXmlFile.SelectedValue
                        End If
                        dbAdmin.BusinessUnit = buBox.Text
                        dbAdmin.Partner = partnerBox.Text
                        dbAdmin.Language = LangBox.Text
                        dbAdmin.CurrentVersion.Version = verBox.Text
                        dbAdmin.CurrentVersion.SubVersion = subVerBox.Text
                        dbAdmin.CurrentVersion.PTF = ptfVerBox.Text
                        dbAdmin.CurrentVersion.Client = clientVerBox.Text
                        dbAdmin.NewVersion.Version = newVerBox.Text
                        dbAdmin.NewVersion.SubVersion = newSubVerBox.Text
                        dbAdmin.NewVersion.PTF = newPtfVerBox.Text
                        dbAdmin.Client = clientNameDDL.SelectedValue

                        'Determine whether to process single file or entire directory
                        If Not ddlXmlFile.SelectedValue = "0" And ddlXmlFile.SelectedIndex > 0 Then
                            dbAdmin.ProcessFile()
                        Else
                            dbAdmin.ProcessDirectory()
                        End If

                        feedbackList.Items.Add(csLabel.Text & " complete.")
                        ProcessUpgradeNotes()
                    Catch ex As Exception
                        feedbackList.Items.Add(csLabel.Text & " ERROR: " & ex.Message)
                    End Try

                End If

            Next
        End If

    End Sub

    Protected Sub liveOrTestDDL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles liveOrTestDDL.SelectedIndexChanged
        clientNameDDL_SelectedIndexChanged(sender, e)
    End Sub

    Protected Sub ProcessUpgradeNotes()
        'Dim path As String = (New TalentDefaults).DBAdminXmlPath
        Dim path As String = xmlPathBox.Text
        If Not path.EndsWith("\") Then path += "\"
        path += "upgrade_notes.xml"


        If System.IO.File.Exists(path) Then

            Dim xdoc As New XmlDocument
            xdoc.Load(path)

            Dim notesTable As New DataTable
            With notesTable.Columns
                .Add(New DataColumn("Version", GetType(String)))
                .Add(New DataColumn("SubVersion", GetType(String)))
                .Add(New DataColumn("PTF", GetType(String)))
                .Add(New DataColumn("ClientVersion", GetType(String)))
                .Add(New DataColumn("Table", GetType(String)))
                .Add(New DataColumn("Description", GetType(String)))
                .Add(New DataColumn("Action", GetType(String)))
            End With

            If ValidateVersionNumbers() Then
                For Each note As XmlNode In xdoc.SelectSingleNode("//EbusinessConfiguration/UpgradeNotes").ChildNodes

                    Dim ver As Integer = Convert.ToInt16(note.Attributes("Version").Value)
                    Dim subVer As Integer = Convert.ToInt16(note.Attributes("SubVersion").Value)
                    Dim ptf As Integer = Convert.ToInt16(note.Attributes("PTF").Value)

                    Dim currVer As Integer = Convert.ToString(Me.verBox.Text)
                    Dim currSubVer As Integer = Convert.ToString(Me.subVerBox.Text)
                    Dim currPtf As Integer = 0

                    Dim newVer As Integer = Convert.ToString(Me.newVerBox.Text)
                    Dim newSubVer As Integer = Convert.ToString(Me.newSubVerBox.Text)
                    Dim newPtf As Integer = 0

                    Dim verPass As Boolean = False

                    If ver > currVer AndAlso ver <= newVer Then
                        verPass = True
                    ElseIf ver = currVer AndAlso subVer > currSubVer AndAlso subVer <= newSubVer Then
                        verPass = True
                    ElseIf ver = currVer AndAlso subVer = currSubVer AndAlso ptf > currPtf AndAlso (ptf <= newPtf Or newPtf = 0) Then
                        verPass = True
                    End If

                    If verPass Then

                        Dim noteRow As DataRow = notesTable.NewRow
                        noteRow("Version") = Convert.ToString(note.Attributes("Version").Value)
                        noteRow("SubVersion") = Convert.ToString(note.Attributes("SubVersion").Value)
                        noteRow("PTF") = Convert.ToString(note.Attributes("PTF").Value)
                        noteRow("ClientVersion") = Convert.ToString(note.Attributes("Client").Value)

                        For Each xNode As XmlNode In note.ChildNodes
                            Select Case xNode.Name.ToLower
                                Case "table"
                                    noteRow("Table") = xNode.InnerText
                                Case "description"
                                    noteRow("Description") = xNode.InnerText
                                Case "action"
                                    noteRow("Action") = xNode.InnerText
                            End Select
                        Next

                        notesTable.Rows.Add(noteRow)
                    End If
                Next

                upgradeNotesRepeater.DataSource = notesTable
                upgradeNotesRepeater.DataBind()
                upgradeNotesRepeater.Visible = True
            End If
        Else
            feedbackList.Items.Add("Could not find upgrade_notes.xml")
        End If

    End Sub

    Protected Sub upgradeNotesRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles upgradeNotesRepeater.ItemDataBound
        If Not e.Item.ItemIndex = -1 Then

            Dim dr As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Dim verLbl As Label = CType(e.Item.FindControl("versionLabel"), Label)
            Dim subVerLbl As Label = CType(e.Item.FindControl("subVersionLabel"), Label)
            Dim ptfLbl As Label = CType(e.Item.FindControl("PTFLabel"), Label)
            Dim clientLbl As Label = CType(e.Item.FindControl("clientLabel"), Label)
            Dim tblNameLbl As Label = CType(e.Item.FindControl("tableNameLabel"), Label)
            Dim descLbl As Label = CType(e.Item.FindControl("descLabel"), Label)
            Dim actionLbl As Label = CType(e.Item.FindControl("actionLabel"), Label)

            verLbl.Text = Convert.ToString(dr("Version"))
            subVerLbl.Text = Convert.ToString(dr("SubVersion"))
            ptfLbl.Text = Convert.ToString(dr("PTF"))
            clientLbl.Text = Convert.ToString(dr("ClientVersion"))
            tblNameLbl.Text = Convert.ToString(dr("Table"))
            descLbl.Text = Convert.ToString(dr("Description"))
            actionLbl.Text = Convert.ToString(dr("Action"))
        End If
    End Sub

    Protected Sub upgradeNotesBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles upgradeNotesBtn.Click
        ProcessUpgradeNotes()
    End Sub
End Class
