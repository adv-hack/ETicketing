Imports PageAddHtmlIncludeTableAdapters
Imports System.Data
Imports System.IO
Imports Talent.Common

Partial Class PagesMaint_PageAddHtmlInclude
    Inherits PageControlBase

#Region "Class Level Fields"

    Private _PgAddAdp As New tbl_page_htmlTableAdapter
    Private _wfrPage As New WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _QSBusinessUnit As String
    Private _QSPartner As String
    Private _QSPageCode As String
    Private _QSPageId As String
    Private _QSKey As String
    Private _QSMode As String
    Private _htmlIncludeRootPath As String = String.Empty
    Private _htmlImageUploadPath As String = String.Empty

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        IsAuthorizedRole()
        _QSBusinessUnit = Request.QueryString("BusinessUnit")
        _QSPartner = Request.QueryString("Partner")
        _QSPageCode = Request.QueryString("PageName")
        _QSPageId = Request.QueryString("PageId")
        _QSKey = Request.QueryString("ID")
        _QSMode = Request.QueryString("Mode")
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PageCode = String.Empty
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PageAddHtmlInclude.aspx"
            .PageCode = "PageAddHtmlInclude.aspx"
        End With
        If String.IsNullOrEmpty(_QSBusinessUnit) Or String.IsNullOrEmpty(_QSPartner) Or String.IsNullOrEmpty(_QSPageCode) Or String.IsNullOrEmpty(_QSPageId) Or String.IsNullOrEmpty(_QSMode) Then
            Response.Redirect("../MaintenancePortal.aspx")
        End If

        If Page.IsPostBack Then
            'reset the error text on postback
            ErrLabel.Text = ""
        End If
        ErrLabel.Visible = False
        SetHtmlPathValues()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not _htmlIncludeRootPath.ToString.EndsWith("\") Then _htmlIncludeRootPath += "\"
        If Not _htmlImageUploadPath.ToString.EndsWith("\") Then _htmlImageUploadPath += "\"
        If IsPostBack = False Then
            PopulateAvailableFilesDropDown()
            setLabel()
            If ddlAvailableUploadedFiles.SelectedIndex = 0 Then
                btnEditHtmlInclude.Visible = False
            End If
        End If
        DisableControls()
    End Sub

    Protected Sub ConfirmButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ConfirmButton.Click
        Dim bError As Boolean = False
        Dim sErrorMessage As String = ""
        If txtSection.Text.Trim = "" Then
            sErrorMessage = "Section is Required Field!<br />"
            bError = True
        End If
        If IsNumeric(txtSection.Text) Then
            Try
                If Not (Convert.ToInt16(txtSection.Text) > 0 And Convert.ToInt16(txtSection.Text) <= 99) Then
                    sErrorMessage += "Section must be a whole number<br />"
                    bError = True
                End If
            Catch
                sErrorMessage += "Section must be a whole number<br />"
                bError = True
            End Try
        Else
            sErrorMessage += "Section must be a whole number<br />"
            bError = True
        End If
        If TextSequence.Text.Trim = "" Then
            sErrorMessage = "Sequence is Required Field!<br />"
            bError = True
        End If
        If IsNumeric(TextSequence.Text) Then
            Try
                If Not (Convert.ToInt16(TextSequence.Text) > 0 And Convert.ToInt16(TextSequence.Text) <= 999) Then
                    sErrorMessage += "Sequence must be a whole number<br />"
                    bError = True
                End If
            Catch
                sErrorMessage += "Sequence must be a whole number<br />"
                bError = True
            End Try
        Else
            sErrorMessage += "Sequence must be a whole number<br />"
            bError = True
        End If

        If TextHTML1.Text.Trim() = "" And TextHTML2.Text.Trim() = "" And TextHTML3.Text.Trim() = "" And ddlAvailableUploadedFiles.SelectedIndex = 0 Then
            sErrorMessage += _wfrPage.Content("FiledReq", _languageCode, True) + "<br />"
            bError = True
        End If
        If bError = True Then
            ErrLabel.Visible = True
            ErrLabel.Text = sErrorMessage
            Exit Sub
        End If

        Dim PSection As Integer = txtSection.Text
        Dim PSequence As Integer = TextSequence.Text
        Dim PQueryString As String = TextQuery.Text
        ErrLabel.Text = ""
        If _QSMode = "Add" Then
            If _PgAddAdp.IsExistSectionSequence(_QSBusinessUnit, _QSPartner, PSection, PSequence, _QSPageCode, PQueryString) < 1 Then
                Dim ans As Integer = _PgAddAdp.InsertHTMLInclude(_QSBusinessUnit, _QSPartner, _QSPageCode, PSection, PSequence, PQueryString, TextHTML1.Text, TextHTML2.Text, TextHTML3.Text, ddlAvailableUploadedFiles.SelectedValue)
                If ans > 0 Then
                    ErrLabel.Visible = True
                    ErrLabel.Text = (_wfrPage.Content("SuccAdd", _languageCode, True))
                Else
                    ErrLabel.Visible = True
                    ErrLabel.Text = (_wfrPage.Content("UnScc", _languageCode, True))
                End If
                Response.Redirect("PageHtmlIncludes.aspx?PageName=" + _QSPageCode + "&PageId=" + _QSPageId + "&Partner=" + _QSPartner + "&BusinessUnit=" + _QSBusinessUnit)
            Else
                ErrLabel.Visible = True
                ErrLabel.Text = _wfrPage.Content("SSAlready", _languageCode, True)
            End If
        Else
            If _PgAddAdp.IsExistSectionSeqUpdate(_QSBusinessUnit, _QSPartner, PSection, PSequence, _QSPageCode, ViewState("HtmlId"), PQueryString) < 1 Then
                Dim ans As Integer = _PgAddAdp.UpdateHTMLInclude(PSection, PSequence, PQueryString, TextHTML1.Text, TextHTML2.Text, TextHTML3.Text, ddlAvailableUploadedFiles.SelectedValue, _QSKey)
                If ans > 0 Then
                    ErrLabel.Visible = True
                    ErrLabel.Text = (_wfrPage.Content("SuccUpdate", _languageCode, True))
                Else
                    ErrLabel.Visible = True
                    ErrLabel.Text = (_wfrPage.Content("UnScc", _languageCode, True))
                End If
                Response.Redirect("PageHtmlIncludes.aspx?PageName=" + _QSPageCode + "&PageId=" + _QSPageId + "&Partner=" + _QSPartner + "&BusinessUnit=" + _QSBusinessUnit)
            Else
                ErrLabel.Visible = True
                ErrLabel.Text = _wfrPage.Content("SSAlready", _languageCode, True)
            End If
        End If
    End Sub

    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelButton.Click
        Response.Redirect("PageHtmlIncludes.aspx?PageName=" + _QSPageCode + "&PageId=" + _QSPageId + "&Partner=" + _QSPartner + "&BusinessUnit=" + _QSBusinessUnit)
    End Sub

    Public Sub Editdata(ByVal source As Object, ByVal e As EventArgs) Handles btnEditHtml1.Click
        trHTMLEditor1.Visible = True
        uscCKEditor1.Text = TextHTML1.Text
        btnEditHtml1.Visible = False
        TextHTML1.Enabled = False
    End Sub

    Protected Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveButton.Click
        trHTMLEditor1.Visible = False
        TextHTML1.Text = uscCKEditor1.Text
        btnEditHtml1.Visible = True
        TextHTML1.Enabled = True
        ErrLabel.Visible = True
        ErrLabel.Text = ("Don't forget to click the Confirm button to save changes!")
    End Sub

    Protected Sub CancelEditButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelEditButton.Click
        trHTMLEditor1.Visible = False
        uscCKEditor1.Text = ""
        btnEditHtml1.Visible = True
        TextHTML1.Enabled = True
    End Sub

    Public Sub Editdata2(ByVal source As Object, ByVal e As EventArgs) Handles btnEditHtml2.Click
        trHTMLEditor2.Visible = True
        uscCKEditor2.Text = TextHTML2.Text
        btnEditHtml2.Visible = False
        TextHTML2.Enabled = False
    End Sub

    Protected Sub SaveButton2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveButton2.Click
        trHTMLEditor2.Visible = False
        TextHTML2.Text = uscCKEditor2.Text
        btnEditHtml2.Visible = True
        TextHTML2.Enabled = True
        ErrLabel.Visible = True
        ErrLabel.Text = ("Don't forget to click the Confirm button to save changes!")
    End Sub

    Protected Sub CancelEditButton2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelEditButton2.Click
        trHTMLEditor2.Visible = False
        uscCKEditor2.Text = ""
        btnEditHtml2.Visible = True
        TextHTML2.Enabled = True
    End Sub

    Public Sub Editdata3(ByVal source As Object, ByVal e As EventArgs) Handles btnEditHtml3.Click
        trHTMLEditor3.Visible = True
        uscCKEditor3.Text = TextHTML3.Text
        btnEditHtml3.Visible = False
        TextHTML3.Enabled = False
    End Sub

    Protected Sub SaveButton3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveButton3.Click
        trHTMLEditor3.Visible = False
        TextHTML3.Text = uscCKEditor3.Text
        btnEditHtml3.Visible = True
        TextHTML3.Enabled = True
        ErrLabel.Visible = True
        ErrLabel.Text = ("Don't forget to click the Confirm button to save changes!")
    End Sub

    Protected Sub CancelEditButton3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelEditButton3.Click
        trHTMLEditor3.Visible = False
        uscCKEditor3.Text = ""
        btnEditHtml3.Visible = True
        TextHTML3.Enabled = True
    End Sub

    Protected Sub EditFile(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditHtmlInclude.Click
        ErrLabel.Text = ""
        If ddlAvailableUploadedFiles.SelectedIndex = 0 Then
            ErrLabel.Visible = True
            ErrLabel.Text = ("Please select a file name from the 'HTML Include Name' box.")
        Else
            Try
                Dim file As String = _htmlIncludeRootPath
                file += ddlAvailableUploadedFiles.SelectedValue
                Dim myFile As New System.IO.FileInfo(file)
                Dim fs As System.IO.FileStream
                If Not myFile.Exists Then
                    fs = myFile.Create
                    myFile.IsReadOnly = False
                    fs.Flush()
                    fs.Close()
                    fs.Dispose()
                End If

                Dim sr As New System.IO.StreamReader(file)
                uscCKEditor4.Text = sr.ReadToEnd
                sr.Close()
                sr.Dispose()
                trHTMLIncludeEditor.Visible = True
                btnEditHtmlInclude.Visible = False
                TextHTMLIncName.Enabled = False
                ddlAvailableUploadedFiles.Enabled = False
            Catch ex As Exception
                ErrLabel.Visible = True
                ErrLabel.Text = ("Failed to read from file: " & ex.Message.ToString)
            End Try
        End If
    End Sub

    Protected Sub SaveButton4_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveButton4.Click
        Try
            Dim filepath As String = _htmlIncludeRootPath
            filepath += ddlAvailableUploadedFiles.SelectedValue
            Dim myFile As New System.IO.FileInfo(filepath)
            If myFile.Exists Then
                If myFile.IsReadOnly Then
                    myFile.IsReadOnly = False
                End If
            End If
            Dim mySw As New System.IO.StreamWriter(filepath, False)
            mySw.Write(uscCKEditor4.Text)
            mySw.Close()
            mySw.Dispose()

            trHTMLIncludeEditor.Visible = False
            btnEditHtmlInclude.Visible = True
            TextHTMLIncName.Enabled = True
            ddlAvailableUploadedFiles.Enabled = True
        Catch ex As Exception
            ErrLabel.Visible = True
            ErrLabel.Text = ("Failed to write to file: " & ex.Message.ToString)
        End Try
    End Sub

    Protected Sub CancelEditButton4_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelEditButton4.Click
        trHTMLIncludeEditor.Visible = False
        uscCKEditor4.Text = ""
        btnEditHtmlInclude.Visible = True
        TextHTMLIncName.Enabled = True
        ddlAvailableUploadedFiles.Enabled = True
    End Sub

    Protected Sub FileUploadLink_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles FileUploadLink.Click
        ErrLabel.Text = ""
        If Not FileUpload1.HasFile Then
            ErrLabel.Visible = True
            ErrLabel.Text = "Please enter a file name into the 'Upload File' box."
        Else
            Try
                If IO.Directory.Exists(_htmlIncludeRootPath) Then

                    'Check file is not 0 length
                    Dim hasContent As Boolean = False
                    Dim validSize As Boolean = False
                    If FileUpload1.PostedFile.ContentLength > 0 Then
                        hasContent = True
                    End If

                    If hasContent Then
                        If FileUpload1.PostedFile.ContentLength <= CInt(ConfigurationManager.AppSettings("MaxHtmlFileUploadSize_InBytes")) Then
                            validSize = True
                        End If

                        If validSize Then
                            'Check for valid file extensions
                            Dim validFile As Boolean = False
                            Dim fileExe As String = IO.Path.GetExtension(FileUpload1.PostedFile.FileName).ToLower
                            If fileExe = ".htm" _
                                OrElse fileExe = ".html" _
                                OrElse fileExe = ".pdf" _
                                OrElse fileExe = ".xml" _
                                OrElse fileExe = ".xsl" _
                                OrElse fileExe = ".txt" Then
                                validFile = True
                            End If

                            If Not validFile Then
                                ErrLabel.Visible = True
                                ErrLabel.Text = "Files of type '" & fileExe & "' cannot be uploaded to the HTML folder for security reasons."
                            Else
                                If IO.File.Exists(_htmlIncludeRootPath & FileUpload1.FileName) _
                                                        AndAlso Not HtmlOverwriteCheck.Checked Then
                                    ErrLabel.Visible = True
                                    ErrLabel.Text = "The file already exists. Please re-name it, or tick the overwrite box and try again."
                                Else
                                    FileUpload1.SaveAs(_htmlIncludeRootPath & FileUpload1.FileName)
                                End If
                            End If
                        Else
                            ErrLabel.Visible = True
                            ErrLabel.Text = "File '" & FileUpload1.FileName & "' is bigger than the maximum upload file size. Max file size = " & (CInt(ConfigurationManager.AppSettings("MaxHtmlFileUploadSize_InBytes")) / 1024).ToString & "KB"
                        End If
                    Else
                        ErrLabel.Visible = True
                        ErrLabel.Text = "File '" & FileUpload1.FileName & "' is empty. No file was uploaded."
                    End If
                Else
                    ErrLabel.Visible = True
                    ErrLabel.Text = "The folder you are trying to upload html files to does not exist. Please contact your administrator."
                End If
            Catch ex As Exception
                ErrLabel.Visible = True
                ErrLabel.Text = "There was an error uploading the file, please try again."
            End Try

            If String.IsNullOrEmpty(ErrLabel.Text) Then
                ErrLabel.Visible = True
                ErrLabel.Text = "File " & FileUpload1.FileName & " successfully uploaded."
                Dim intIndex As Integer = ddlAvailableUploadedFiles.SelectedIndex
                PopulateAvailableFilesDropDown()
                ddlAvailableUploadedFiles.SelectedIndex = intIndex
            End If
        End If
    End Sub

    Protected Sub ImageUploadLink_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageUploadLink.Click
        ErrLabel.Text = ""
        If Not ImageUpload1.HasFile Then
            ErrLabel.Visible = True
            ErrLabel.Text = ("Please enter a file name into the 'Image Upload' box.")
        Else
            Try
                If IO.Directory.Exists(_htmlImageUploadPath) Then

                    'Check file is not 0 length
                    Dim hasContent As Boolean = False
                    Dim validSize As Boolean = False
                    If ImageUpload1.PostedFile.ContentLength > 0 Then
                        hasContent = True
                    End If
                    If hasContent Then
                        If ImageUpload1.PostedFile.ContentLength <= CInt(ConfigurationManager.AppSettings("MaxImageFileUploadSize_InBytes")) Then
                            validSize = True
                        End If
                        If validSize Then
                            'Check for valid file extensions
                            Dim validFile As Boolean = False
                            Dim fileExe As String = IO.Path.GetExtension(ImageUpload1.PostedFile.FileName).ToLower
                            If fileExe = ".jpg" _
                                OrElse fileExe = ".jpeg" _
                                OrElse fileExe = ".png" _
                                OrElse fileExe = ".gif" _
                                OrElse fileExe = ".swf" _
                                OrElse fileExe = ".pdf" Then
                                validFile = True
                            End If
                            If Not validFile Then
                                ErrLabel.Visible = True
                                ErrLabel.Text = "Files of type '" & fileExe & "' cannot be uploaded to the Images folder for security reasons."
                            Else
                                If IO.File.Exists(_htmlImageUploadPath & ImageUpload1.FileName) _
                                                        AndAlso Not ImageOverwriteCheck.Checked Then
                                    ErrLabel.Visible = True
                                    ErrLabel.Text = "The image already exists. Please re-name it, or tick the overwrite box and try again."
                                Else
                                    ImageUpload1.SaveAs(_htmlImageUploadPath & ImageUpload1.FileName)
                                End If
                            End If
                        Else
                            ErrLabel.Visible = True
                            ErrLabel.Text = "File '" & ImageUpload1.FileName & "' is bigger than the maximum image size. Max image size = " & (CInt(ConfigurationManager.AppSettings("MaxImageFileUploadSize_InBytes")) / 1024).ToString & "KB"
                        End If
                    Else
                        ErrLabel.Visible = True
                        ErrLabel.Text = "File '" & ImageUpload1.FileName & "' is empty. No file was uploaded."
                    End If
                Else
                    ErrLabel.Visible = True
                    ErrLabel.Text = "The folder you are trying to upload images to does not exist. Please contact your administrator."
                End If
            Catch ex As Exception
                ErrLabel.Visible = True
                ErrLabel.Text = ("There was an error uploading the Image, please try again.")
            End Try
            If String.IsNullOrEmpty(ErrLabel.Text) Then
                ErrLabel.Visible = True
                ErrLabel.Text = "Image " & ImageUpload1.FileName & " successfully uploaded."
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub setLabel()
        If Request.QueryString("Mode") = "Add" Then
            TitleLabel.Text = "Add "
        Else
            TitleLabel.Text = "Change "
        End If
        TitleLabel.Text = TitleLabel.Text + "HTML Include: " ' Page Name
        instructionsLabel.Text = _wfrPage.Content("instructionsLabel", _languageCode, True)
        businessunitLabel.Text = _wfrPage.Content("businessunitLabel", _languageCode, True)
        partnerLabel.Text = _wfrPage.Content("partnerLabel", _languageCode, True)
        PageNameLabel.Text = _wfrPage.Content("PageNameLabel", _languageCode, True)
        DescriptionLabel.Text = _wfrPage.Content("DescriptionLabel", _languageCode, True)
        SectionLabel.Text = _wfrPage.Content("SectionLabel", _languageCode, True)
        SequenceLabel.Text = _wfrPage.Content("SequenceLabel", _languageCode, True)
        HTML1Label.Text = _wfrPage.Content("HTML1Label", _languageCode, True)
        HTML2Label.Text = _wfrPage.Content("HTML2Label", _languageCode, True)
        HTML3Label.Text = _wfrPage.Content("HTML3Label", _languageCode, True)
        HTMLIncNameLabel.Text = _wfrPage.Content("HTMLIncNameLabel", _languageCode, True)
        ConfirmButton.Text = _wfrPage.Content("ConfirmButton", _languageCode, True)
        CancelButton.Text = _wfrPage.Content("CancelButton", _languageCode, True)

        'Set Values
        businessunit.Text = _QSBusinessUnit
        partner.Text = _QSPartner
        PageName.Text = _QSPageCode
        Description.Text = _PgAddAdp.GetPageDesc(_QSBusinessUnit, _QSPartner, _QSPageCode)
        TitleLabel.Text = TitleLabel.Text + Description.Text
        If _QSMode = "Edit" Then
            Dim DT As DataTable
            DT = _PgAddAdp.GetHTMLIncludeData(Request.QueryString("ID"))
            txtSection.Text = DT.Rows(0)("SECTION").ToString
            TextSequence.Text = DT.Rows(0)("Sequence").ToString
            TextQuery.Text = DT.Rows(0)("PAGE_QUERYSTRING").ToString
            ViewState("HtmlId") = DT.Rows(0)("page_HTML_ID").ToString
            TextHTML1.Text = DT.Rows(0)("HTML_1").ToString
            TextHTML2.Text = DT.Rows(0)("HTML_2").ToString
            TextHTML3.Text = DT.Rows(0)("HTML_3").ToString
            ddlAvailableUploadedFiles.SelectedValue = DT.Rows(0)("HTML_LOCATION").ToString
        End If
        ltlImagePath.Text = _htmlImageUploadPath
    End Sub

    Private Sub SetHtmlPathValues()
        _htmlIncludeRootPath = Utilities.CheckForDBNull_String(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_QSBusinessUnit, "HTML_PATH_ABSOLUTE"))
        _htmlImageUploadPath = Utilities.CheckForDBNull_String(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_QSBusinessUnit, "IMAGE_PATH_ABSOLUTE"))
    End Sub

    Private Sub PopulateAvailableFilesDropDown()
        If Not String.IsNullOrEmpty(_htmlIncludeRootPath) Then
            Dim strFiles() As String = Directory.GetFiles(_htmlIncludeRootPath)
            Dim strSingleFile As String
            ddlAvailableUploadedFiles.Items.Clear()
            Dim newListItem As New ListItem
            newListItem.Value = ""
            newListItem.Text = " -- "
            ddlAvailableUploadedFiles.Items.Insert(0, newListItem)
            For Each strSingleFile In strFiles
                ddlAvailableUploadedFiles.Items.Add(strSingleFile.Substring(_htmlIncludeRootPath.Length))
            Next
        End If
    End Sub

    Private Sub DisableControls()
        btnEditHtml1.Enabled = Utilities.isEnabled("AllowEditHTMLInclude")
    End Sub

    Private Sub IsAuthorizedRole()
        Dim requestMode As String = Request.QueryString("Mode")
        If String.IsNullOrWhiteSpace(requestMode) Then requestMode = ""
        If (Not Utilities.isEnabled("AllowEditHTMLInclude")) AndAlso requestMode.ToUpper = "EDIT" Then
            Response.Redirect("../Error.aspx?Type=UNAUTH")
        End If
        If (Not Utilities.isEnabled("AllowAddHTMLInclude")) AndAlso requestMode.ToUpper = "ADD" Then
            Response.Redirect("../Error.aspx?Type=UNAUTH")
        End If
    End Sub

#End Region

End Class