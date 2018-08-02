Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities
Imports TalentBusinessLogic.BusinessObjects.Definitions
Imports System.Data
Imports System.Globalization
Imports Talent.Common
Imports System.Collections.Generic
Imports System.Linq

Partial Class UserControls_EditProfileActivity
    Inherits ControlBase

#Region "Private variables"

    Private _ucr As UserControlResource = Nothing
    Private _languageCode As String = String.Empty
    Private _errMessage As TalentErrorMessages = Nothing
    Private _templateId As Integer = 0
    Private _customerActivityUniqueId As Integer = 0
    Private _dtTemplates As DataTable = Nothing
    Private _dtActivityTemplate As DataTable = Nothing
    Private _customerActivities As New TalentActivities
    Private _settings As DESettings = Nothing
    Private _de As DEActivities = Nothing
    Private _err As ErrorObj = Nothing
    Private _dtCustomerActivitiesHeader As DataTable = Nothing
    Private _dtCustomerActivitiesDetail As DataTable = Nothing
    Private _talentErrorMsg As TalentErrorMessage
    Private _customerNumber As String = String.Empty
    Private _questionAnswerMultiplier As Integer = 0
    Private _indexOfQuestionAnswerSet As Integer = 0
    Private _countOfQuestionsInOneSet As Integer = 0
    Private _questionNumber As Integer = 0
    Private _activityCallId As Integer = 0
    Private _dtSetofQuestions As DataTable = Nothing
    Private _dtAllQuestions As DataTable = Nothing
    Private _product As New TalentProduct()
#End Region

#Region "Public Properties"
    Public Property HeaderVisible As Boolean = True
    Public Property TemplateQuestionsVisible As Boolean = True
    Public Property AttachmentsVisible As Boolean = True
    Public Property CommentsVisible As Boolean = True
    Public Property ActivityDate As Date
    Public Property ActivitySubject As String
    Public Property ActivityStatus As String
    Public Property ActivityAgent As String
    Public Property Usage As String



#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        InitialiseClassLevelFields()
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = TEBUtilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "EditProfileActivity.ascx"
        End With

        If _templateId > 0 Then
            _dtTemplates = TDataObjects.ActivitiesSettings.TblActivityTemplates.GetByBUTemplateID(_ucr.BusinessUnit, _templateId)
            If _dtTemplates.Rows.Count = 0 Then
                _dtTemplates = TDataObjects.ActivitiesSettings.TblActivityTemplates.GetByTemplateID(_templateId)
            End If
            _dtActivityTemplate = TDataObjects.ActivitiesSettings.GetActivityTemplatesTypeByBUTemplateID(_ucr.BusinessUnit, _templateId)
            If _dtActivityTemplate.Rows.Count = 0 Then
                _dtActivityTemplate = TDataObjects.ActivitiesSettings.GetActivityTemplatesTypeByTemplateID(_templateId)
            End If
            SetTextAndAttributes()
            PopulateUsers()
            populateActivityStatusDropDownList()
            If _customerActivityUniqueId > 0 Then
                btnCreateActivity.Visible = False
                btnUpdateActivity.Visible = True
                LoadActivitiesForEditing()
                LoadFileAttachmentsForEditing()
                LoadCommentsForEditing()
                LoadAnswers()
            Else
                plhActivityFileAttachments.Visible = False
                btnAddComment.Visible = False
                btnCreateActivity.Visible = True
                btnUpdateActivity.Visible = False
                LoadActivitiesForCreating()
            End If
            LoadQuestions()
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If _templateId > 0 Then
            hplBack.Text = _ucr.Content("BackButtonText", _languageCode, True)
            If Usage IsNot Nothing AndAlso Usage.ToUpper = "HOSPITALITYDATACAPTURE" Then
                If Not String.IsNullOrEmpty(Session("HospitalityPreBookingProductCode")) AndAlso Not String.IsNullOrEmpty(Session("HospitalityPreBoookingPackageId")) AndAlso Not String.IsNullOrEmpty(Session("HospitalityPreBookingAvailabilityComponentId")) Then
                    Dim redirectUrl As New StringBuilder
                    redirectUrl.Append("~/PagesPublic/Hospitality/HospitalityPackageDetails.aspx?")
                    redirectUrl.Append("product=").Append(Session("HospitalityPreBookingProductCode"))
                    redirectUrl.Append("&packageid=").Append(Session("HospitalityPreBoookingPackageId"))
                    redirectUrl.Append("&availabilitycomponentid=").Append(Session("HospitalityPreBookingAvailabilityComponentId"))
                    hplBack.NavigateUrl = redirectUrl.ToString()
                    Session.Remove("HospitalityPreBoookingPackageId")
                    Session.Remove("HospitalityPreBookingAvailabilityComponentId")
                Else
                    hplBack.NavigateUrl = "~/PagesPublic/Hospitality/Hospitality.aspx"
                End If

                plhActivityHeader.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(HeaderVisible)
                plhTemplateQuestions.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(TemplateQuestionsVisible)
                plhActivityFileAttachments.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(AttachmentsVisible)
                If plhActivityFileAttachments.Visible Then
                    LoadFileAttachmentsForEditing()
                End If
                plhComments.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(CommentsVisible)
                If Not AgentProfile.IsAgent Then
                    txtActivityDate.Text = ActivityDate
                    ddlActivityStatus.SelectedValue = ActivityStatus
                    ddlUser.SelectedValue = ActivityAgent
                    plhActivityHeader.Visible = False
                    ltlCustomerPromptMessage.Text = _ucr.Content("HospitalityPreBookingEnquiryPrompt", Talent.Common.Utilities.GetDefaultLanguage, True)
                    plhCustomerPromptMessage.Visible = True
                End If
                txtActivitySubject.Text = ActivitySubject
                btnCreateActivity.Visible = True
                btnUpdateActivity.Visible = False
                If Not Page.IsPostBack Then
                    LoadActivitiesForCreating()
                End If
            Else
                hplBack.NavigateUrl = "~/PagesAgent/CRM/ActivitiesList.aspx"
            End If
        Else
            If Usage IsNot Nothing AndAlso Usage.ToUpper = "HOSPITALITYDATACAPTURE" Then
                blErrorMessages.Items.Add(Utilities.CheckForDBNull_String(_ucr.Content("NoTempalteIdErrorMessage", _languageCode, True)))
                hideAllItems()
            Else
                Response.Redirect("~/PagesAgent/CRM/ActivitiesList.aspx")
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (blErrorMessages.Items.Count > 0)
        plhSuccessList.Visible = (blSuccessMessages.Items.Count > 0)
    End Sub

    Protected Sub rptActivityQuestions_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim question As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Select Case question("ANSWER_TYPE")
                Case Is = GlobalConstants.FREE_TEXT_FIELD : InitialiseFreeTextField(e.Item, question)
                Case Is = GlobalConstants.CHECKBOX : InitialiseCheckboxField(e.Item, question)
                Case Is = GlobalConstants.QUESTION_DATE : InitialiseDateField(e.Item, question)
                Case Is = GlobalConstants.LIST_OF_ANSWERS : InitialiseSelectField(e.Item, question)
            End Select

            If question("ASK_QUESTION_PER_HOSPITALITY_BOOKING") = False Then
                _questionNumber = _questionNumber + 1
                If _questionNumber Mod _countOfQuestionsInOneSet = 0 Then
                    _indexOfQuestionAnswerSet = _indexOfQuestionAnswerSet + 1
                End If
            End If
        End If
    End Sub

    Protected Sub rptActivityComments_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptActivityComments.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim ltlCommentAgentName As Literal = CType(e.Item.FindControl("ltlCommentAgentName"), Literal)
            Dim ltlCommentBlurb As Literal = CType(e.Item.FindControl("ltlCommentBlurb"), Literal)
            Dim ltlCommentDate As Literal = CType(e.Item.FindControl("ltlCommentDate"), Literal)
            Dim ltlCommentTime As Literal = CType(e.Item.FindControl("ltlCommentTime"), Literal)
            Dim ltlCommentText As Literal = CType(e.Item.FindControl("ltlCommentText"), Literal)
            Dim ltlCommentTextHidden As Literal = CType(e.Item.FindControl("ltlCommentTextHidden"), Literal)
            Dim editComment As HtmlGenericControl = CType(e.Item.FindControl("editComment"), HtmlGenericControl)
            Dim deleteIcon As HtmlGenericControl = CType(e.Item.FindControl("deleteIcon"), HtmlGenericControl)
            Dim commitIcon As HtmlGenericControl = CType(e.Item.FindControl("commitIcon"), HtmlGenericControl)
            Dim clientClickString As New StringBuilder
            Dim agentDefinition As New Agent
            Dim dateUpdated As New Date
            Dim timeUpdated As New TimeSpan
            Dim unformattedTimeUpdated As String = e.Item.DataItem("TimeUpdated")

            ltlCommentAgentName.Text = agentDefinition.GetAgentDescriptiveNameByAgentUserCode(e.Item.DataItem("AgentName"))
            dateUpdated = TCUtilities.ISeries8CharacterDate(e.Item.DataItem("DateUpdated"))
            ltlCommentDate.Text = dateUpdated.ToString(ModuleDefaults.GlobalDateFormat)
            timeUpdated = TimeSpan.ParseExact(unformattedTimeUpdated.PadLeft(6, "0"), "hhmmss", New CultureInfo(ModuleDefaults.Culture))
            ltlCommentTime.Text = timeUpdated.ToString()
            If TCUtilities.convertToBool(e.Item.DataItem("CommentEdited")) Then
                ltlCommentBlurb.Text = _ucr.Content("CommentEditedText", _languageCode, True)
            Else
                ltlCommentBlurb.Text = _ucr.Content("CommentAddedText", _languageCode, True)
            End If
            ltlCommentText.Text = e.Item.DataItem("CommentText")
            ltlCommentTextHidden.Text = e.Item.DataItem("CommentText")

            clientClickString.Clear()
            clientClickString.Append("updateActivityComment(")
            clientClickString.Append(_customerActivityUniqueId).Append(",")
            clientClickString.Append(e.Item.DataItem("CommentId")).Append(",")
            clientClickString.Append(e.Item.ItemIndex)
            clientClickString.Append(");")
            commitIcon.Attributes.Add("onmousedown", clientClickString.ToString())
            editComment.Attributes.Add("class", "ebiz-edit-activity-comment-text ebiz-edit-activity-comment-text-" & e.Item.DataItem("CommentId"))

            clientClickString.Clear()
            clientClickString.Append("deleteActivityComment(").Append(e.Item.DataItem("CommentId")).Append(", ").Append(e.Item.ItemIndex).Append(");")
            deleteIcon.Attributes.Add("onclick", clientClickString.ToString())
        End If
    End Sub

    Protected Sub rptActivityFiles_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptActivityFiles.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim ltlFileAgentName As Literal = CType(e.Item.FindControl("ltlFileAgentName"), Literal)
            Dim ltlFileAttachmentBlurb As Literal = CType(e.Item.FindControl("ltlFileAttachmentBlurb"), Literal)
            Dim ltlFileDateUploaded As Literal = CType(e.Item.FindControl("ltlFileDateUploaded"), Literal)
            Dim ltlFileTimeUploaded As Literal = CType(e.Item.FindControl("ltlFileTimeUploaded"), Literal)
            Dim hplFileAttachment As HyperLink = CType(e.Item.FindControl("hplFileAttachment"), HyperLink)
            Dim ltlFileDescription As Literal = CType(e.Item.FindControl("ltlFileDescription"), Literal)
            Dim plhFileDescription As PlaceHolder = CType(e.Item.FindControl("plhFileDescription"), PlaceHolder)
            Dim btnDeleteFile As HtmlInputButton = CType(e.Item.FindControl("btnDeleteFile"), HtmlInputButton)
            Dim navigateUrlString As String = String.Empty
            Dim navigateUrl As New StringBuilder
            Dim clientClickString As New StringBuilder
            Dim agentDefinition As New Agent
            Dim fileUploadedDate As New Date
            Dim fileUploadedTime As New TimeSpan
            Dim unformattedFileUploadedTime As String = e.Item.DataItem("TimeUploaded")

            ltlFileAgentName.Text = agentDefinition.GetAgentDescriptiveNameByAgentUserCode(e.Item.DataItem("AgentName"))
            ltlFileAttachmentBlurb.Text = _ucr.Content("FileAttachmentBlurbText", _languageCode, True)
            fileUploadedDate = TCUtilities.ISeries8CharacterDate(e.Item.DataItem("DateUploaded"))
            ltlFileDateUploaded.Text = fileUploadedDate.ToString(ModuleDefaults.GlobalDateFormat)
            fileUploadedTime = TimeSpan.ParseExact(unformattedFileUploadedTime.PadLeft(6, "0"), "hhmmss", New CultureInfo(ModuleDefaults.Culture))
            ltlFileTimeUploaded.Text = fileUploadedTime.ToString()
            hplFileAttachment.Text = e.Item.DataItem("FileAttachmentId").ToString() & "_" & e.Item.DataItem("FileName")
            ltlFileDescription.Text = e.Item.DataItem("FileDescription").Trim()
            plhFileDescription.Visible = (ltlFileDescription.Text.Length > 0)
            If _dtActivityTemplate.Rows.Count > 0 AndAlso TEBUtilities.CheckForDBNull_String(_dtActivityTemplate.Rows(0)("REMOTE_ROOT_DIRECTORY")).Length > 0 Then
                navigateUrlString = _dtActivityTemplate.Rows(0)("REMOTE_ROOT_DIRECTORY").ToString()
                If Not navigateUrlString.EndsWith("/") Then navigateUrlString = navigateUrlString & "/"
                If Not navigateUrlString.StartsWith("~") Then navigateUrlString = "~" & navigateUrlString
                navigateUrl.Append(navigateUrlString).Append(e.Item.DataItem("FileAttachmentId").ToString()).Append("_").Append(e.Item.DataItem("FileName").ToString())
            End If
            hplFileAttachment.NavigateUrl = ResolveUrl(navigateUrl.ToString())
            btnDeleteFile.Value = _ucr.Content("DeleteFileButtonText", _languageCode, True)
            clientClickString.Append("deleteActivityFile(").Append(e.Item.DataItem("FileAttachmentId")).Append(", ")
            clientClickString.Append(_templateId).Append(", '")
            clientClickString.Append(e.Item.DataItem("FileName")).Append("', ")
            clientClickString.Append(e.Item.ItemIndex).Append(");")
            btnDeleteFile.Attributes.Add("onclick", clientClickString.ToString())
        End If
    End Sub

    Protected Sub btnCreateActivity_Click(sender As Object, e As EventArgs) Handles btnCreateActivity.Click
        If Page.IsValid Then
            Dim errorMessage As String = String.Empty
            If plhAcivityUser.Visible AndAlso ddlUser.SelectedValue = "-1" Then
                errorMessage = _ucr.Content("PleaseSelectAValidUserError", _languageCode, True)
                Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(errorMessage)
                If listItemObject Is Nothing Then blErrorMessages.Items.Add(errorMessage)
            Else
                _settings = Nothing
                _settings = TEBUtilities.GetSettingsObject()
                _settings.Cacheing = False
                If plhAcivityUser.Visible Then
                    _de.ActivityUserName = ddlUser.SelectedValue
                Else
                    If Usage IsNot Nothing AndAlso Usage.ToUpper().Equals("HOSPITALITYDATACAPTURE") Then
                        _de.ActivityUserName = ActivityAgent
                    Else
                        _de.ActivityUserName = String.Empty
                    End If
                End If
                If plhActivityDate.Visible Or (Usage IsNot Nothing AndAlso Usage.ToUpper().Equals("HOSPITALITYDATACAPTURE")) Then
                    If txtActivityDate.Text = String.Empty Then
                        txtActivityDate.Text = Now.Date
                    End If
                    Dim activityDate As Date = DateTime.ParseExact(txtActivityDate.Text, "dd/MM/yyyy", New CultureInfo(ModuleDefaults.Culture))
                    _de.ActivityDate = TCUtilities.DateToIseries8Format(activityDate)
                Else
                    _de.ActivityDate = 0
                End If
                _de.CustomerNumber = _customerNumber
                _de.ActivitySubject = txtActivitySubject.Text
                _de.ActivityStatus = ddlActivityStatus.SelectedValue
                _de.ActivityTextDelimiter = _ucr.Attribute("ActivityTextDelimiter")
                _de.TemplateID = _templateId
                _de.ActivityCommentText = txtAddComment.Text
                _de.ActivityFileDescription = txtFileDescription.Text
                _de.Source = GlobalConstants.SOURCE
                _customerActivities = Nothing
                _customerActivities = New TalentActivities
                _customerActivities.Settings = _settings

                'Concatenate the answers and questions text into a string array. Ensure the length doesn't exceed the parameter sizes (allow for a buffer)
                Dim boolTextLengthError As Boolean = False
                buildAnswersArray(_de.ActivityQuestionIDArray, _de.ActivityQuestionTextArray, _de.ActivityAnswerTextArray, boolTextLengthError)
                If boolTextLengthError OrElse (_de.ActivityQuestionTextArray IsNot Nothing AndAlso _de.ActivityQuestionTextArray.Length > 3900) OrElse (_de.ActivityAnswerTextArray IsNot Nothing AndAlso _de.ActivityAnswerTextArray.Length > 3900) Then
                    errorMessage = _ucr.Content("QuestionAnswerTextLengthExceededError", _languageCode, True)
                    Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(errorMessage)
                    If listItemObject Is Nothing Then blErrorMessages.Items.Add(errorMessage)
                Else
                    _customerActivities.De = _de
                    _err = _customerActivities.AddCustomerActivity()

                    If Not _err.HasError AndAlso _customerActivities.ResultDataSet IsNot Nothing AndAlso _customerActivities.ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Count > 0 Then
                        If _customerActivities.ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                            _talentErrorMsg = _errMessage.GetErrorMessage(_customerActivities.ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows(0)("ReturnCode"))
                            Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(_talentErrorMsg.ERROR_MESSAGE)
                            If listItemObject Is Nothing Then blErrorMessages.Items.Add(_talentErrorMsg.ERROR_MESSAGE)
                        Else
                            If Usage IsNot Nothing AndAlso Usage.ToUpper = "HOSPITALITYDATACAPTURE" Then
                                blSuccessMessages.Items.Add(Utilities.CheckForDBNull_String(_ucr.Content("HospitalityDataCaptureCreationSuccessMessage", _languageCode, True)))
                                plhSuccessList.Visible = True
                                hideAllItems()
                                clearSessionValues()
                                plhCustomerPromptMessage.Visible = False
                            Else
                                Session("SuccessfullyCreatedActivity") = True
                                Response.Redirect("~/PagesAgent/CRM/ActivitiesList.aspx")
                            End If
                        End If
                    Else
                        clearSessionValues()
                        _talentErrorMsg = _errMessage.GetErrorMessage("ErrorCreatingActivity")
                        Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(_talentErrorMsg.ERROR_MESSAGE)
                        If listItemObject Is Nothing Then blErrorMessages.Items.Add(_talentErrorMsg.ERROR_MESSAGE)
                    End If
                End If
            End If
        End If
    End Sub

    Protected Sub btnUpdateActivity_Click(sender As Object, e As EventArgs) Handles btnUpdateActivity.Click
        If Page.IsValid Then
            Dim errorMessage As String = String.Empty
            If plhAcivityUser.Visible AndAlso ddlUser.SelectedValue = "-1" Then
                errorMessage = _ucr.Content("PleaseSelectAValidUserError", _languageCode, True)
                Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(errorMessage)
                If listItemObject Is Nothing Then blErrorMessages.Items.Add(errorMessage)
            Else
                _settings = Nothing
                _settings = TEBUtilities.GetSettingsObject()
                _settings.Cacheing = False
                If plhAcivityUser.Visible Then
                    _de.ActivityUserName = ddlUser.SelectedValue
                Else
                    _de.ActivityUserName = String.Empty
                End If
                If plhActivityDate.Visible Then
                    If txtActivityDate.Text = String.Empty Then
                        txtActivityDate.Text = Now.Date
                    End If
                    Dim activityDate As Date = DateTime.ParseExact(txtActivityDate.Text, "dd/MM/yyyy", New CultureInfo(ModuleDefaults.Culture))
                    _de.ActivityDate = TCUtilities.DateToIseries8Format(activityDate)
                Else
                    _de.ActivityDate = 0
                End If
                _de.CustomerNumber = _customerNumber
                _de.ActivitySubject = txtActivitySubject.Text
                _de.ActivityStatus = ddlActivityStatus.SelectedValue
                _de.ActivityTextDelimiter = _ucr.Attribute("ActivityTextDelimiter")
                _de.TemplateID = _templateId
                _de.Source = GlobalConstants.SOURCE
                _de.ActivityCallId = _dtCustomerActivitiesHeader.Rows(0).Item("ActivityCallID")
                _activityCallId = _de.ActivityCallId

                'Concatenate the answers and questions text into a string array. Ensure the length doesn't exceed the parameter sizes (allow for a buffer)
                Dim boolTextLengthError As Boolean = False
                buildAnswersArray(_de.ActivityQuestionIDArray, _de.ActivityQuestionTextArray, _de.ActivityAnswerTextArray, boolTextLengthError)
                If boolTextLengthError OrElse (_de.ActivityQuestionTextArray Is Nothing) OrElse (_de.ActivityAnswerTextArray Is Nothing) Then
                    errorMessage = _ucr.Content("QuestionAnswerTextLengthExceededError", _languageCode, True)
                    Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(errorMessage)
                    If listItemObject Is Nothing Then blErrorMessages.Items.Add(errorMessage)
                Else
                    If Not _activityCallId = 0 Then
                        Dim productDetails As New DEProductDetails()
                        _product.Settings = _settings
                        If _dtAllQuestions IsNot Nothing AndAlso _dtAllQuestions.Rows.Count > 0 Then
                            Dim productEntity As New DEProduct()
                            Dim dEProductQuestionsAnswersList As New List(Of DEProductQuestionsAnswers)
                            Dim counter As Integer = 0
                            For Each question In _dtAllQuestions.Rows
                                Dim answerArray() As String = _de.ActivityAnswerTextArray.Split(_ucr.Attribute("ActivityTextDelimiter"))
                                Dim dep = New DEProductQuestionsAnswers()
                                dep.QuestionID = Convert.ToInt32(question("QUESTION_ID"))
                                dep.QuestionText = question("QUESTION_TEXT").ToString()
                                dep.AnswerText = answerArray(counter)
                                dep.CallID = _activityCallId
                                dep.TemplateID = _templateId
                                dep.CustomerNumber = _customerNumber
                                dep.AllocationCustomerNumber = _customerNumber
                                dep.ProductCode = _customerActivities.ResultDataSet.Tables("CustomerActivitiesDetail").Rows(0).Item("Productcode").ToString
                                dep.PriceBand = " "
                                dep.RememberedAnswer = False
                                dep.QuestionPerBooking = Convert.ToBoolean(question("ASK_QUESTION_PER_HOSPITALITY_BOOKING"))
                                dep.SeatData = String.Empty
                                dep.AlphaSeat = " "
                                dep.BasketID = String.Empty
                                dep.NewCallFlag = "Y"
                                dep.ProductDescription = String.Empty
                                dep.Source = GlobalConstants.SOURCE
                                dep.AgentName = _settings.AgentEntity.AgentUsername
                                dEProductQuestionsAnswersList.Add(dep)
                                counter = counter + 1
                            Next
                            productDetails.SessionId = String.Empty
                            productEntity.CollDEProductQuestionAnswers = dEProductQuestionsAnswersList
                            _product.Dep = productEntity
                            _product.De = productDetails
                            _err = _product.AddProductQuestionAnswers()
                        End If
                        If Not _err.HasError AndAlso _product.ResultDataSet IsNot Nothing AndAlso _product.ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Count > 0 Then
                            If _product.ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                                _talentErrorMsg = _errMessage.GetErrorMessage(_customerActivities.ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows(0)("ReturnCode"))
                                Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(_talentErrorMsg.ERROR_MESSAGE)
                                If listItemObject Is Nothing Then blErrorMessages.Items.Add(_talentErrorMsg.ERROR_MESSAGE)
                            End If
                        Else
                            _talentErrorMsg = _errMessage.GetErrorMessage("ErrorUpdatingActivity")
                            Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(_talentErrorMsg.ERROR_MESSAGE)
                            If listItemObject Is Nothing Then blErrorMessages.Items.Add(_talentErrorMsg.ERROR_MESSAGE)
                        End If
                    End If
                    _customerActivities = Nothing
                    _customerActivities = New TalentActivities
                    _customerActivities.Settings = _settings
                    _de.CustomerActivitiesHeaderID = _customerActivityUniqueId
                    _customerActivities.De = _de
                    _err = _customerActivities.UpdateCustomerActivity()

                    If Not _err.HasError AndAlso _customerActivities.ResultDataSet IsNot Nothing AndAlso _customerActivities.ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Count > 0 Then
                        If _customerActivities.ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                            _talentErrorMsg = _errMessage.GetErrorMessage(_customerActivities.ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows(0)("ReturnCode"))
                            Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(_talentErrorMsg.ERROR_MESSAGE)
                            If listItemObject Is Nothing Then blErrorMessages.Items.Add(_talentErrorMsg.ERROR_MESSAGE)
                        Else
                            Session("SuccessfullyUpdatedActivity") = True
                            Response.Redirect("~/PagesAgent/CRM/ActivitiesList.aspx")
                        End If
                    Else
                        _talentErrorMsg = _errMessage.GetErrorMessage("ErrorUpdatingActivity")
                        Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(_talentErrorMsg.ERROR_MESSAGE)
                        If listItemObject Is Nothing Then blErrorMessages.Items.Add(_talentErrorMsg.ERROR_MESSAGE)
                    End If
                End If
            End If
        End If

    End Sub

#End Region

#Region "Public Methods"

    Public Sub SetupRequiredValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim requiredFieldValidator As RequiredFieldValidator = CType(sender, RequiredFieldValidator)
        If Not String.IsNullOrWhiteSpace(_ucr.Content("SubjectRequiredError", _languageCode, True)) Then
            requiredFieldValidator.Enabled = True
            requiredFieldValidator.ErrorMessage = _ucr.Content("SubjectRequiredError", _languageCode, True)
        Else
            requiredFieldValidator.Enabled = False
        End If
    End Sub

    Public Sub SetupRegExValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim regularExpressionValidator As RegularExpressionValidator = CType(sender, RegularExpressionValidator)
        Dim controlRegExpression As String = _ucr.Attribute("DateFieldRegex")
        If String.IsNullOrEmpty(controlRegExpression) Then
            regularExpressionValidator.Enabled = False
        Else
            regularExpressionValidator.ValidationExpression = controlRegExpression
            regularExpressionValidator.ErrorMessage = _ucr.Content("IncorrectActivityDateFormatErrorMessage", _languageCode, True)
        End If
    End Sub

    Public Sub ValidateListAnswer(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        Dim listBoxesAreValid As Boolean = True
        For Each item As RepeaterItem In rptActivityQuestions.Items
            Dim plhListOfAnswers As PlaceHolder = CType(item.FindControl("plhListOfAnswers"), PlaceHolder)
            If plhListOfAnswers IsNot Nothing AndAlso plhListOfAnswers.Visible Then
                Dim csvSpecify As CustomValidator = CType(item.FindControl("csvSpecify"), CustomValidator)
                If csvSpecify IsNot Nothing AndAlso csvSpecify.Enabled Then
                    Dim txtSpecify As TextBox = CType(item.FindControl("txtSpecify"), TextBox)
                    Dim ddlAnswers As DropDownList = CType(item.FindControl("ddlAnswers"), DropDownList)
                    If txtSpecify IsNot Nothing AndAlso ddlAnswers IsNot Nothing Then
                        If txtSpecify.Text.Trim().Length = 0 AndAlso ddlAnswers.SelectedValue = _ucr.Content("OtherText", _languageCode, True) Then
                            listBoxesAreValid = False
                            Exit For
                        End If
                    End If
                End If
            End If
        Next
        e.IsValid = listBoxesAreValid
    End Sub

    Public Sub ValidateCheckboxAnswer(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        Dim listBoxesAreValid As Boolean = True
        For Each item As RepeaterItem In rptActivityQuestions.Items
            Dim plhCheckbox As PlaceHolder = CType(item.FindControl("plhCheckbox"), PlaceHolder)
            If plhCheckbox IsNot Nothing AndAlso plhCheckbox.Visible Then
                Dim csvQuestionCheck As CustomValidator = CType(item.FindControl("csvQuestionCheck"), CustomValidator)
                If csvQuestionCheck IsNot Nothing AndAlso csvQuestionCheck.Enabled Then
                    Dim chkQuestionCheck As CheckBox = CType(item.FindControl("chkQuestionCheck"), CheckBox)
                    If Not chkQuestionCheck.Checked Then
                        listBoxesAreValid = False
                        Exit For
                    End If
                End If
            End If
        Next
        e.IsValid = listBoxesAreValid
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set the class level objects
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitialiseClassLevelFields()
        _ucr = New UserControlResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _errMessage = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
        _templateId = TEBUtilities.CheckForDBNull_Int(Request.Params("TemplateId"))
        _customerActivityUniqueId = TEBUtilities.CheckForDBNull_Int(Request.Params("Id"))
        _customerActivities = New TalentActivities
        _settings = TEBUtilities.GetSettingsObject()
        _de = New DEActivities
        _err = New ErrorObj
        _dtCustomerActivitiesHeader = New DataTable
        Session("SuccessfullyAddedActivity") = False
        Session("SuccessfullyUpdatedActivity") = False
    End Sub

    ''' <summary>
    ''' Set the text properties and display defaults
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetTextAndAttributes()
        'Activities header details
        lblActivity.Text = _ucr.Content("lblActivity", _languageCode, True)
        plhActivityDate.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("ShowDateColumn"))
        plhActivitySubject.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("ShowSubjectColumn"))
        plhActivityStatus.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("ShowStatusColumn"))
        plhAcivityUser.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("ShowUserColumn"))
        If plhActivityDate.Visible Then lblActivityDate.Text = _ucr.Content("lblDate", _languageCode, True)
        If plhActivitySubject.Visible Then
            lblActivitySubject.Text = _ucr.Content("lblSubject", _languageCode, True)
            rfvActivitySubject.ErrorMessage = _ucr.Content("SubjectRequiredError", _languageCode, True)
            rfvActivitySubject.Enabled = True
            txtActivitySubject.Attributes.Add("placeholder", lblActivitySubject.Text)
        Else
            rfvActivitySubject.Enabled = False
        End If
        If plhActivityStatus.Visible Then lblActivityStatus.Text = _ucr.Content("lblStatus", _languageCode, True)
        If plhAcivityUser.Visible Then lblUserDropdown.Text = _ucr.Content("lblUserDropdown", _languageCode, True)

        'File upload
        lblFileDescription.Text = _ucr.Content("FileDescriptionText", _languageCode, True)
        ltlAttachmentHeader.Text = _ucr.Content("AddAttachmentHeaderText", _languageCode, True)
        hdfAlertifyDeleteFileTitle.Value = _ucr.Content("AlertifyDeleteFileTitleText", _languageCode, True)
        hdfAlertifyDeleteFileMessage.Value = _ucr.Content("AlertifyDeleteFileMessageText", _languageCode, True)

        'Comments
        Dim clientClickString As New StringBuilder
        clientClickString.Append("createActivityComment(")
        clientClickString.Append(_customerActivityUniqueId)
        clientClickString.Append(");")
        btnAddComment.Attributes.Add("onclick", clientClickString.ToString())
        btnAddComment.Value = _ucr.Content("AddCommentButtonText", _languageCode, True)
        ltlCommentHeader.Text = _ucr.Content("AddCommentHeaderText", _languageCode, True)
        hdfAlertifyDeleteCommentTitle.Value = _ucr.Content("AlertifyDeleteCommentTitleText", _languageCode, True)
        hdfAlertifyDeleteCommentMessage.Value = _ucr.Content("AlertifyDeleteCommentMessageText", _languageCode, True)

        btnCreateActivity.Text = _ucr.Content("SaveActivity", _languageCode, True)
        btnUpdateActivity.Text = _ucr.Content("SaveActivity", _languageCode, True)
        hdfTalentAPIUrl.Value = ModuleDefaults.TalentAPIAddress
        hdfTemplateID.Value = _templateId
        hdfCustomerActivityUniqueID.Value = _customerActivityUniqueId
        hdfAlertifyOK.Value = _ucr.Content("AlertifyOKText", _languageCode, True)
        hdfAlertifyCancel.Value = _ucr.Content("AlertifyCancelText", _languageCode, True)
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "fine-uploader.min.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("fine-uploader.min.js", "/vendor/"), False)
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error-handling.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("error-handling.js", "/Application/Status/"), False)
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "activities.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("activities.js", "/Module/CRM/"), False)
    End Sub

    ''' <summary>
    ''' Retreive the list of agent data and populate the drop down list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateUsers()
        Dim talAgent As New TalentAgent
        Dim err As New ErrorObj
        Dim users As New DataTable
        Dim agentDataEntity As New DEAgent
        talAgent.Settings = TEBUtilities.GetSettingsObject()
        agentDataEntity.Source = GlobalConstants.SOURCE
        talAgent.AgentDataEntity = agentDataEntity
        err = talAgent.RetrieveAllAgents()
        If Not err.HasError AndAlso talAgent.ResultDataSet IsNot Nothing AndAlso talAgent.ResultDataSet.Tables("ErrorStatus").Rows.Count > 0 Then
            If talAgent.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                Dim talentErrorMsg As TalentErrorMessage = _errMessage.GetErrorMessage(talAgent.ResultDataSet.Tables("ErrorStatus").Rows(0)("ReturnCode"))
                Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(talentErrorMsg.ERROR_MESSAGE)
                If listItemObject Is Nothing Then blErrorMessages.Items.Add(talentErrorMsg.ERROR_MESSAGE)
            Else
                users = talAgent.ResultDataSet.Tables("AgentUsers")
                AddDDLUserOptions(users)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Add the user options from the data table to the drop down list
    ''' </summary>
    ''' <param name="dt">The data table of users</param>
    ''' <remarks></remarks>
    Private Sub AddDDLUserOptions(ByVal dt As DataTable)
        ddlUser.Items.Clear()
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            AddDDLUserOption(_ucr.Content("PleaseSelectOptionText", _languageCode, True), "-1")
            For Each row As DataRow In dt.Rows
                AddDDLUserOption(row("UserName"), row("UserCode"))
            Next
        End If
    End Sub

    ''' <summary>
    ''' Add the option to the user drop down list
    ''' </summary>
    ''' <param name="optionText">The display option text</param>
    ''' <param name="optionValue">The select option value</param>
    ''' <remarks></remarks>
    Private Sub AddDDLUserOption(ByVal optionText As String, ByVal optionValue As String)
        AddDDLOption(ddlUser, optionText, optionValue)
    End Sub

    ''' <summary>
    ''' Add the option to the drop down list
    ''' </summary>
    ''' <param name="ddl">The drop down list to add the option to</param>
    ''' <param name="optionText">The text value</param>
    ''' <param name="optionValue">The select value</param>
    ''' <remarks></remarks>
    Private Sub AddDDLOption(ByRef ddl As DropDownList, ByVal optionText As String, ByVal optionValue As String)
        If Not String.IsNullOrEmpty(optionText) Then
            Dim lstitem As New ListItem
            lstitem.Text = optionText
            lstitem.Value = optionValue
            ddl.Items.Add(lstitem)
            lstitem = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Get the status values and bind them to the drop down list
    ''' Filter only the allowable status's based on the type of activity
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateActivityStatusDropDownList()
        '    Dim dtActivityStatus As DataTable = TDataObjects.ActivitiesSettings.GetActivityStatusByBUAndTemplateId(TalentCache.GetBusinessUnit, _templateId)
        Dim dtActivityStatus As DataTable = TDataObjects.ActivitiesSettings.GetActivityStatusByBusinessUnit(TalentCache.GetBusinessUnit)
        If dtActivityStatus.Rows.Count > 0 Then
            ddlActivityStatus.DataSource = dtActivityStatus
            ddlActivityStatus.DataTextField = "DESCRIPTION"
            ddlActivityStatus.DataValueField = "DESCRIPTION"
            ddlActivityStatus.DataBind()
        End If
    End Sub

    ''' <summary>
    ''' Load the questions from the template of questions
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadQuestions()
        _dtAllQuestions = New DataTable()
        _dtSetofQuestions = New DataTable()
        _dtSetofQuestions = TDataObjects.ActivitiesSettings.GetExistingActivityQuestionByTemplateID(_templateId)
        Dim questionsInOneSet() As DataRow = _dtSetofQuestions.Select("ASK_QUESTION_PER_HOSPITALITY_BOOKING ='" & False & "'")
        _countOfQuestionsInOneSet = questionsInOneSet.Count
        _dtAllQuestions = _dtSetofQuestions.Clone

        If Not _questionAnswerMultiplier = 0 AndAlso Not TEBUtilities.CheckForDBNull_Int(_questionAnswerMultiplier) Then
            For i As Integer = 1 To _questionAnswerMultiplier
                For Each dataRow As DataRow In _dtSetofQuestions.Rows
                    If dataRow.Item("ASK_QUESTION_PER_HOSPITALITY_BOOKING") = True Then
                        Dim foundRow() As DataRow = _dtAllQuestions.Select("Question_ID ='" & dataRow.Item("Question_ID").ToString & "'")
                        If foundRow.Count = 0 Then
                            _dtAllQuestions.Rows.Add(dataRow.ItemArray)
                        End If
                    Else
                        _dtAllQuestions.Rows.Add(dataRow.ItemArray)
                    End If
                Next
            Next i
        Else
            _dtAllQuestions = _dtSetofQuestions
        End If
        rptActivityQuestions.DataSource = _dtAllQuestions
        rptActivityQuestions.DataBind()
    End Sub

    ''' <summary>
    ''' Load the activities for editing an activitiy
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadActivitiesForEditing()
        _settings = Nothing
        _settings = TEBUtilities.GetSettingsObject()
        _settings.Cacheing = False
        _de.CustomerActivitiesHeaderID = _customerActivityUniqueId
        _de.Source = GlobalConstants.SOURCE
        _customerActivities = Nothing
        _customerActivities = New TalentActivities
        _customerActivities.Settings = _settings
        _customerActivities.De = _de
        _err = _customerActivities.CustomerActivitiesRetrieval()

        If Not _err.HasError AndAlso _customerActivities.ResultDataSet IsNot Nothing AndAlso _customerActivities.ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Count > 0 Then
            If _customerActivities.ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                Dim talentErrorMsg As TalentErrorMessage = _errMessage.GetErrorMessage(_customerActivities.ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows(0)("ReturnCode"))
                Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(talentErrorMsg.ERROR_MESSAGE)
                If listItemObject Is Nothing Then blErrorMessages.Items.Add(talentErrorMsg.ERROR_MESSAGE)
            Else
                _dtCustomerActivitiesHeader = _customerActivities.ResultDataSet.Tables("CustomerActivitiesHeader")
                Dim row As DataRow = _dtCustomerActivitiesHeader.Rows(0)
                lblActivityValue.Text = TEBUtilities.CheckForDBNull_String(_dtTemplates.Rows(0)("NAME"))
                txtActivitySubject.Text = row("ActivitySubject")
                txtActivityDate.Text = TCUtilities.ISeries8CharacterDate(row("ActivityDate"))
                ddlActivityStatus.SelectedValue = row("ActivityStatus")
                _customerNumber = row("CustomerNumber").ToString().PadLeft(12, "0")
                _questionAnswerMultiplier = row("ActivityQaMultiplier")
                Dim userFound As Boolean = False
                For Each item As ListItem In ddlUser.Items
                    If item.Value = row("ActivityUserName") Then
                        ddlUser.SelectedValue = row("ActivityUserName")
                        userFound = True
                        Exit For
                    End If
                Next
                If Not userFound Then
                    Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(_ucr.Content("UserNotFoundError", _languageCode, True))
                    If listItemObject Is Nothing Then blErrorMessages.Items.Add(_ucr.Content("UserNotFoundError", _languageCode, True))
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Load the activities for creating an activity
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadActivitiesForCreating()
        If Usage IsNot Nothing AndAlso Usage.ToUpper = "HOSPITALITYDATACAPTURE" Then
            If Not String.IsNullOrEmpty(ActivityAgent) Then
                ddlUser.SelectedValue = ActivityAgent
            Else
                ddlUser.SelectedValue = AgentProfile.Name
            End If

            If Not String.IsNullOrEmpty(ActivitySubject) Then
                txtActivitySubject.Text = ActivitySubject
            End If

            If Not String.IsNullOrEmpty(ActivityDate) Then
                txtActivityDate.Text = ActivityDate
            Else
                txtActivityDate.Text = New Date(Now.Year, Now.Month, Now.Day)
            End If

            If Not String.IsNullOrEmpty(ActivityStatus) Then
                ddlActivityStatus.SelectedValue = ActivityStatus
            End If
        Else
            If String.IsNullOrEmpty(Session("ActivityUserName")) OrElse Session("ActivityUserName") = "-1" Then
                ddlUser.SelectedValue = AgentProfile.Name
            Else
                ddlUser.SelectedValue = Session("ActivityUserName")
            End If
            If Session("ActivitySubject") IsNot Nothing Then
                txtActivitySubject.Text = Session("ActivitySubject")
            End If
            If String.IsNullOrEmpty(Session("ActivityDate")) Then
                txtActivityDate.Text = New Date(Now.Year, Now.Month, Now.Day)
            Else
                txtActivityDate.Text = Session("ActivityDate")
            End If
            If Session("ActivityStatus") IsNot Nothing Then
                ddlActivityStatus.SelectedValue = Session("ActivityStatus")
            End If
        End If
        lblActivityValue.Text = TEBUtilities.CheckForDBNull_String(_dtTemplates.Rows(0)("NAME"))
        _customerNumber = Profile.User.Details.LoginID
    End Sub

    ''' <summary>
    ''' Load the answers if editing an activity
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadAnswers()
        _settings = Nothing
        _settings = TEBUtilities.GetSettingsObject()
        _settings.Cacheing = False
        _customerActivities = Nothing
        _customerActivities = New TalentActivities
        _de.CustomerNumber = _customerNumber
        _de.CustomerActivitiesHeaderID = _customerActivityUniqueId
        _de.Source = GlobalConstants.SOURCE
        _customerActivities.Settings = _settings
        _customerActivities.De = _de
        _err = _customerActivities.ActivitiesQNARetrieval()

        If Not _err.HasError AndAlso _customerActivities.ResultDataSet IsNot Nothing AndAlso _customerActivities.ResultDataSet.Tables("ErrorStatus").Rows.Count > 0 Then
            If _customerActivities.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                Dim talentErrorMsg As TalentErrorMessage = _errMessage.GetErrorMessage(_customerActivities.ResultDataSet.Tables("ErrorStatus").Rows(0)("ReturnCode"))
                Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(talentErrorMsg.ERROR_MESSAGE)
                If listItemObject Is Nothing Then blErrorMessages.Items.Add(talentErrorMsg.ERROR_MESSAGE)
            Else
                _dtCustomerActivitiesDetail = _customerActivities.ResultDataSet.Tables("CustomerActivitiesDetail")
            End If
        End If
    End Sub

    ''' <summary>
    ''' Set the properties for a free text field answer type
    ''' </summary>
    ''' <param name="item">The given repeater item</param>
    ''' <param name="question">The question related to the current item</param>
    ''' <remarks></remarks>
    Private Sub InitialiseFreeTextField(ByVal item As RepeaterItem, ByVal question As DataRow)
        Dim plhFreeTextField As PlaceHolder = CType(item.FindControl("plhFreeTextField"), PlaceHolder)
        Dim txtQuestionText As TextBox = CType(item.FindControl("txtQuestionText"), TextBox)
        Dim lblQuestionText As Label = CType(item.FindControl("lblQuestionText"), Label)
        Dim revQuestionText As RegularExpressionValidator = CType(item.FindControl("revQuestionText"), RegularExpressionValidator)
        Dim rfvQuestionText As RequiredFieldValidator = CType(item.FindControl("rfvQuestionText"), RequiredFieldValidator)

        plhFreeTextField.Visible = True
        rfvQuestionText.ErrorMessage = getFormattedErrorText(question("QUESTION_TEXT"), True)
        If question("MANDATORY") = True Then
            rfvQuestionText.Enabled = True
        End If
        lblQuestionText.Text = question("QUESTION_TEXT")
        txtQuestionText.Attributes.Add("placeholder", lblQuestionText.Text)

        If Not IsDBNull(question("REGULAR_EXPRESSION")) Then
            If Not String.IsNullOrEmpty(question("REGULAR_EXPRESSION")) Then
                revQuestionText.Enabled = True
                revQuestionText.ValidationExpression = question("REGULAR_EXPRESSION")
                revQuestionText.ErrorMessage = getFormattedErrorText(question("QUESTION_TEXT"), False)
            End If
        End If
        If Not IsDBNull(question("HYPERLINK")) Then
            If Not String.IsNullOrEmpty(question("HYPERLINK")) Then
                Dim hyperlinkString As String = question("HYPERLINK")
                Dim hyperlinkData() As String = hyperlinkString.Split("^")
                Dim hplFreeTextFieldExternalLink As HyperLink = CType(item.FindControl("hplFreeTextFieldExternalLink"), HyperLink)
                hplFreeTextFieldExternalLink.Visible = True
                hplFreeTextFieldExternalLink.NavigateUrl = hyperlinkData(1)
                If String.IsNullOrEmpty(hyperlinkData(0)) Then
                    hplFreeTextFieldExternalLink.Text = hyperlinkData(1)
                Else
                    hplFreeTextFieldExternalLink.Text = hyperlinkData(0)
                End If
            End If
        End If

        If _customerActivityUniqueId = 0 Then
            txtQuestionText.Text = String.Empty
        Else
            Dim index As Integer = _indexOfQuestionAnswerSet
            If _dtCustomerActivitiesDetail IsNot Nothing Then
                For Each row As DataRow In _dtCustomerActivitiesDetail.Rows
                    If TEBUtilities.CheckForDBNull_Int(row("QuestionID")) > 0 AndAlso TEBUtilities.CheckForDBNull_Int(row("QuestionID")) = TEBUtilities.CheckForDBNull_Int(question("QUESTION_ID")) Then
                        If question("ASK_QUESTION_PER_HOSPITALITY_BOOKING") = True Then
                            txtQuestionText.Text = TEBUtilities.CheckForDBNull_String(row("AnswerText"))
                        Else
                            If index = 0 Then
                                txtQuestionText.Text = TEBUtilities.CheckForDBNull_String(row("AnswerText"))
                                Exit For
                            End If
                            index = index - 1
                        End If
                    End If
                Next
            End If

        End If
    End Sub

    ''' <summary>
    ''' Set the properties for a Checkbox answer type
    ''' </summary>
    ''' <param name="item">The given repeater item</param>
    ''' <param name="question">The question related to the current item</param>
    ''' <remarks></remarks>
    Private Sub InitialiseCheckboxField(ByVal item As RepeaterItem, ByVal question As DataRow)
        Dim plhCheckbox As PlaceHolder = CType(item.FindControl("plhCheckbox"), PlaceHolder)
        Dim chkQuestionCheck As CheckBox = CType(item.FindControl("chkQuestionCheck"), CheckBox)
        Dim lblQuestionCheckText As Label = CType(item.FindControl("lblQuestionCheckText"), Label)
        Dim csvQuestionCheck As CustomValidator = CType(item.FindControl("csvQuestionCheck"), CustomValidator)

        plhCheckbox.Visible = True
        lblQuestionCheckText.Text = question("QUESTION_TEXT")
        If question("MANDATORY") = True Then
            chkQuestionCheck.CssClass = "ebiz-mandatory-checkbox"
            csvQuestionCheck.Enabled = True
            csvQuestionCheck.ErrorMessage = getFormattedErrorText(question("QUESTION_TEXT"), True)
            CheckboxValidation(chkQuestionCheck)
            Page.ClientScript.RegisterExpandoAttribute(csvQuestionCheck.ClientID, "chkId", chkQuestionCheck.ClientID)
        End If

        If Not IsDBNull(question("HYPERLINK")) Then
            If Not String.IsNullOrEmpty(question("HYPERLINK")) Then
                Dim hyperlinkString As String = question("HYPERLINK")
                Dim hyperlinkData() As String = hyperlinkString.Split("^")
                Dim hplCheckBoxExternalLink As HyperLink = CType(item.FindControl("hplCheckBoxExternalLink"), HyperLink)
                hplCheckBoxExternalLink.Visible = True
                hplCheckBoxExternalLink.NavigateUrl = hyperlinkData(1)
                If String.IsNullOrEmpty(hyperlinkData(0)) Then
                    hplCheckBoxExternalLink.Text = hyperlinkData(1)
                Else
                    hplCheckBoxExternalLink.Text = hyperlinkData(0)
                End If
            End If
        End If

        If _customerActivityUniqueId = 0 Then
            chkQuestionCheck.Checked = False
        Else
            Dim index As Integer = _indexOfQuestionAnswerSet
            If _dtCustomerActivitiesDetail IsNot Nothing Then
                For Each row As DataRow In _dtCustomerActivitiesDetail.Rows
                    If TEBUtilities.CheckForDBNull_Int(row("QuestionID")) > 0 AndAlso TEBUtilities.CheckForDBNull_Int(row("QuestionID")) = TEBUtilities.CheckForDBNull_Int(question("QUESTION_ID")) Then
                        If question("ASK_QUESTION_PER_HOSPITALITY_BOOKING") = True Then
                            chkQuestionCheck.Checked = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(row("AnswerText"))
                        Else
                            If index = 0 Then
                                chkQuestionCheck.Checked = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(row("AnswerText"))
                                Exit For
                            End If
                            index = index - 1
                        End If
                    End If
                Next
            End If
        End If
    End Sub

    ''' <summary>
    ''' Set the properties for a date picker answer type
    ''' </summary>
    ''' <param name="item">The given repeater item</param>
    ''' <param name="question">The question related to the current item</param>
    ''' <remarks></remarks>
    Private Sub InitialiseDateField(ByVal item As RepeaterItem, ByVal question As DataRow)
        Dim plhDate As PlaceHolder = CType(item.FindControl("plhDate"), PlaceHolder)
        Dim rfvDate As RequiredFieldValidator = CType(item.FindControl("rfvDate"), RequiredFieldValidator)
        Dim txtDate As TextBox = CType(item.FindControl("txtDate"), TextBox)
        Dim lblDate As Label = CType(item.FindControl("lblDate"), Label)
        Dim revDate As RegularExpressionValidator = CType(item.FindControl("revDate"), RegularExpressionValidator)
        revDate.ValidationExpression = _ucr.Attribute("DateFieldRegex")
        revDate.ErrorMessage = _ucr.Content("IncorrectDateFormatErrorMessage", _languageCode, True)
        plhDate.Visible = True
        lblDate.Text = question("QUESTION_TEXT")
        If question("MANDATORY") = True Then
            rfvDate.Enabled = True
            rfvDate.ErrorMessage = getFormattedErrorText(question("QUESTION_TEXT"), True)
        End If

        If Not IsDBNull(question("HYPERLINK")) Then
            If Not String.IsNullOrEmpty(question("HYPERLINK")) Then
                Dim hyperlinkString As String = question("HYPERLINK")
                Dim hyperlinkData() As String = hyperlinkString.Split("^")
                Dim hplDateExternalLink As HyperLink = CType(item.FindControl("hplDateExternalLink"), HyperLink)
                hplDateExternalLink.Visible = True
                hplDateExternalLink.NavigateUrl = hyperlinkData(1)
                If String.IsNullOrEmpty(hyperlinkData(0)) Then
                    hplDateExternalLink.Text = hyperlinkData(1)
                Else
                    hplDateExternalLink.Text = hyperlinkData(0)
                End If
            End If
        End If

        txtDate.Attributes.Add("placeholder", lblDate.Text)

        If _customerActivityUniqueId = 0 Then
            txtDate.Text = String.Empty
        Else
            Dim index As Integer = _indexOfQuestionAnswerSet
            If _dtCustomerActivitiesDetail IsNot Nothing Then
                For Each row As DataRow In _dtCustomerActivitiesDetail.Rows
                    If TEBUtilities.CheckForDBNull_Int(row("QuestionID")) > 0 AndAlso TEBUtilities.CheckForDBNull_Int(row("QuestionID")) = TEBUtilities.CheckForDBNull_Int(question("QUESTION_ID")) Then
                        If question("ASK_QUESTION_PER_HOSPITALITY_BOOKING") = True Then
                            If TEBUtilities.CheckForDBNull_String(row("AnswerText")).Equals(GlobalConstants.DEFAULT_DATE) Then
                                txtDate.Text = String.Empty
                            Else
                                txtDate.Text = TEBUtilities.CheckForDBNull_String(row("AnswerText"))
                            End If
                        Else
                            If index = 0 Then
                                If TEBUtilities.CheckForDBNull_String(row("AnswerText")).Equals(GlobalConstants.DEFAULT_DATE) Then
                                    txtDate.Text = String.Empty
                                Else
                                    txtDate.Text = TEBUtilities.CheckForDBNull_String(row("AnswerText"))
                                End If
                                Exit For
                            End If
                            index = index - 1
                        End If
                    End If
                Next
            End If
        End If
    End Sub

    ''' <summary>
    ''' Set the properties for a drop down list answer type
    ''' </summary>
    ''' <param name="item">The given repeater item</param>
    ''' <param name="question">The question related to the current item</param>
    ''' <remarks></remarks>
    Private Sub InitialiseSelectField(ByVal item As RepeaterItem, ByVal question As DataRow)
        Dim plhListOfAnswers As PlaceHolder = CType(item.FindControl("plhListOfAnswers"), PlaceHolder)
        Dim ddlAnswers As DropDownList = CType(item.FindControl("ddlAnswers"), DropDownList)
        Dim lblListOfAnswers As Label = CType(item.FindControl("lblListOfAnswers"), Label)
        Dim txtSpecify As TextBox = CType(item.FindControl("txtSpecify"), TextBox)
        Dim specifyanswer As HtmlGenericControl = CType(item.FindControl("specifyanswer"), HtmlGenericControl)
        Dim lblSpecify As Label = CType(item.FindControl("lblSpecify"), Label)
        Dim revSpecify As RegularExpressionValidator = CType(item.FindControl("revSpecify"), RegularExpressionValidator)
        Dim csvSpecify As CustomValidator = CType(item.FindControl("csvSpecify"), CustomValidator)
        Dim regularExpression As String = String.Empty

        plhListOfAnswers.Visible = True
        lblListOfAnswers.Text = question("QUESTION_TEXT")
        ddlAnswers.DataSource = TDataObjects.ActivitiesSettings.GetAnswerByQuestionID(question("QUESTION_ID").ToString())
        ddlAnswers.DataValueField = "ANSWER_ID"
        ddlAnswers.DataTextField = "ANSWER_TEXT"
        ddlAnswers.DataBind()

        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(question.Item("ALLOW_SELECT_OTHER_OPTION")) Then
            If Not IsDBNull(question("REGULAR_EXPRESSION")) Then
                If Not String.IsNullOrEmpty(question("REGULAR_EXPRESSION")) Then
                    regularExpression = question("REGULAR_EXPRESSION")
                End If
            End If
            ScriptRegistrationForListBox(TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(question("MANDATORY")), regularExpression)
            Page.ClientScript.RegisterExpandoAttribute(csvSpecify.ClientID, "ddlId", ddlAnswers.ClientID)
            Page.ClientScript.RegisterExpandoAttribute(csvSpecify.ClientID, "txtId", txtSpecify.ClientID)
            specifyanswer.Visible = True
            lblSpecify.Text = _ucr.Content("SpecifyText", _languageCode, True)
            If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(question("MANDATORY")) Then
                csvSpecify.Enabled = True
                csvSpecify.ErrorMessage = getFormattedErrorText(question("QUESTION_TEXT"), False)
            End If
            ddlAnswers.Attributes.Add("onchange", "showOtherOptionTextbox(" & specifyanswer.ClientID & ", this);")
            ddlAnswers.Items.Insert(ddlAnswers.Items.Count, _ucr.Content("OtherText", _languageCode, True))
        Else
            specifyanswer.Visible = False
        End If

        If Not IsDBNull(question("HYPERLINK")) Then
            If Not String.IsNullOrEmpty(question("HYPERLINK")) Then
                Dim hyperlinkString As String = question("HYPERLINK")
                Dim hyperlinkData() As String = hyperlinkString.Split("^")
                Dim hplListOfAnswersExternalLink As HyperLink = CType(item.FindControl("hplListOfAnswersExternalLink"), HyperLink)
                hplListOfAnswersExternalLink.Visible = True
                hplListOfAnswersExternalLink.NavigateUrl = hyperlinkData(1)
                If String.IsNullOrEmpty(hyperlinkData(0)) Then
                    hplListOfAnswersExternalLink.Text = hyperlinkData(1)
                Else
                    hplListOfAnswersExternalLink.Text = hyperlinkData(0)
                End If
            End If
        End If

        If _customerActivityUniqueId = 0 Then
            If specifyanswer.Visible Then specifyanswer.Attributes.Add("style", "display:none;")
        Else
            Dim index As Integer = _indexOfQuestionAnswerSet
            If _dtCustomerActivitiesDetail IsNot Nothing Then
                For Each row As DataRow In _dtCustomerActivitiesDetail.Rows
                    If TEBUtilities.CheckForDBNull_Int(row("QuestionID")) > 0 AndAlso TEBUtilities.CheckForDBNull_Int(row("QuestionID")) = TEBUtilities.CheckForDBNull_Int(question("QUESTION_ID")) Then
                        If (question("ASK_QUESTION_PER_HOSPITALITY_BOOKING") = False AndAlso index = 0) OrElse question("ASK_QUESTION_PER_HOSPITALITY_BOOKING") = True Then
                            Dim answerFound As Boolean = False
                            For Each answer As ListItem In ddlAnswers.Items
                                If answer.Text = TEBUtilities.CheckForDBNull_String(row("AnswerText")) Then
                                    ddlAnswers.SelectedValue = answer.Value
                                    answerFound = True
                                    Exit For
                                End If
                            Next
                            '"Other" option specified
                            If Not answerFound AndAlso TEBUtilities.CheckForDBNull_String(row("AnswerText")).Length > 0 Then
                                txtSpecify.Text = TEBUtilities.CheckForDBNull_String(row("AnswerText"))
                                ddlAnswers.SelectedIndex = ddlAnswers.Items.Count - 1
                            Else
                                If specifyanswer.Visible Then specifyanswer.Attributes.Add("style", "display:none;")
                            End If
                            Exit For
                        End If
                        If question("ASK_QUESTION_PER_HOSPITALITY_BOOKING") = False Then
                            index = index - 1
                        End If
                    End If
                Next
            End If
        End If
    End Sub

    ''' <summary>
    ''' Combine the answers text, questions text and questions ids into a string array with a specific delimiter
    ''' </summary>
    ''' <param name="questionsIDArray">The question ID array to populate</param>
    ''' <param name="questionsTextArray">The questions text array to populate</param>
    ''' <param name="answersTextArray">The answers text array to populate</param>
    ''' <remarks></remarks>
    Private Sub buildAnswersArray(ByRef questionsIDArray As String, ByRef questionsTextArray As String, ByRef answersTextArray As String, ByRef boolTextLengthError As Boolean)
        Dim addDelimiter As Boolean = False
        For Each item As RepeaterItem In rptActivityQuestions.Items
            If addDelimiter Then
                questionsIDArray += _ucr.Attribute("ActivityTextDelimiter")
                questionsTextArray += _ucr.Attribute("ActivityTextDelimiter")
                answersTextArray += _ucr.Attribute("ActivityTextDelimiter")
            End If
            Dim plhFreeTextField As PlaceHolder = CType(item.FindControl("plhFreeTextField"), PlaceHolder)
            Dim plhCheckbox As PlaceHolder = CType(item.FindControl("plhCheckbox"), PlaceHolder)
            Dim plhDate As PlaceHolder = CType(item.FindControl("plhDate"), PlaceHolder)
            Dim plhListOfAnswers As PlaceHolder = CType(item.FindControl("plhListOfAnswers"), PlaceHolder)
            Dim hdfQuestionID As HiddenField = CType(item.FindControl("hdfQuestionID"), HiddenField)

            If plhFreeTextField.Visible Then
                Dim lblQuestionText As Label = CType(item.FindControl("lblQuestionText"), Label)
                Dim txtQuestionText As TextBox = CType(item.FindControl("txtQuestionText"), TextBox)
                If lblQuestionText.Text.Trim.Length > 500 Then boolTextLengthError = True
                If txtQuestionText.Text.Trim.Length > 500 Then boolTextLengthError = True
                questionsTextArray += TCUtilities.ConvertASCIIHexValue(lblQuestionText.Text, _ucr.BusinessUnit, _ucr.PartnerCode, _languageCode, _settings)
                answersTextArray += TCUtilities.ConvertASCIIHexValue(txtQuestionText.Text, _ucr.BusinessUnit, _ucr.PartnerCode, _languageCode, _settings)
            ElseIf plhCheckbox.Visible Then
                Dim lblQuestionCheckText As Label = CType(item.FindControl("lblQuestionCheckText"), Label)
                Dim chkQuestionCheck As CheckBox = CType(item.FindControl("chkQuestionCheck"), CheckBox)
                questionsTextArray += TCUtilities.ConvertASCIIHexValue(lblQuestionCheckText.Text, _ucr.BusinessUnit, _ucr.PartnerCode, _languageCode, _settings)
                answersTextArray += chkQuestionCheck.Checked.ToString()
            ElseIf plhDate.Visible Then
                Dim lblDate As Label = CType(item.FindControl("lblDate"), Label)
                Dim txtDate As TextBox = CType(item.FindControl("txtDate"), TextBox)
                If String.IsNullOrEmpty(txtDate.Text) Then
                    txtDate.Text = GlobalConstants.DEFAULT_DATE
                End If
                questionsTextArray += TCUtilities.ConvertASCIIHexValue(lblDate.Text, _ucr.BusinessUnit, _ucr.PartnerCode, _languageCode, _settings)
                answersTextArray += TCUtilities.ConvertASCIIHexValue(txtDate.Text, _ucr.BusinessUnit, _ucr.PartnerCode, _languageCode, _settings)
            ElseIf plhListOfAnswers.Visible Then
                Dim lblListOfAnswers As Label = CType(item.FindControl("lblListOfAnswers"), Label)
                Dim ddlAnswers As DropDownList = CType(item.FindControl("ddlAnswers"), DropDownList)
                Dim txtSpecify As TextBox = CType(item.FindControl("txtSpecify"), TextBox)
                questionsTextArray += TCUtilities.ConvertASCIIHexValue(lblListOfAnswers.Text, _ucr.BusinessUnit, _ucr.PartnerCode, _languageCode, _settings)
                If txtSpecify.Text.Trim().Length > 0 Then
                    answersTextArray += TCUtilities.ConvertASCIIHexValue(txtSpecify.Text, _ucr.BusinessUnit, _ucr.PartnerCode, _languageCode, _settings)
                Else
                    answersTextArray += TCUtilities.ConvertASCIIHexValue(ddlAnswers.SelectedItem.Text, _ucr.BusinessUnit, _ucr.PartnerCode, _languageCode, _settings)
                End If
            End If
            questionsIDArray += hdfQuestionID.Value
            addDelimiter = True
        Next
    End Sub

    ''' <summary>
    ''' Script registration for javascript functionality for the list of answers.
    ''' </summary>
    ''' <param name="isMandatory">Is the current question mandatory</param>
    ''' <remarks></remarks>
    Private Sub ScriptRegistrationForListBox(ByVal isMandatory As Boolean, ByVal regularExpressionValue As String)
        Dim sbJavaScript As New StringBuilder
        sbJavaScript.Append("function showOtherOptionTextbox(control, ddl) { ")
        sbJavaScript.Append("   var sInd = ddl.selectedIndex;")
        sbJavaScript.Append("   var sVal = ddl.options[sInd].text;")
        sbJavaScript.Append("   if(sVal == """).Append(_ucr.Content("OtherText", _languageCode, True)).Append("""){ ")
        sbJavaScript.Append("       $(control).show();")
        sbJavaScript.Append("   }else{")
        sbJavaScript.Append("       $(control).hide();}")
        sbJavaScript.Append("}")

        sbJavaScript.Append("function ValidateListAnswer(sender, e) {")
        sbJavaScript.Append("   var ddl = document.getElementById(sender.ddlId);")
        sbJavaScript.Append("   var sInd = ddl.selectedIndex;")
        sbJavaScript.Append("   var sVal = ddl.options[sInd].text;")
        sbJavaScript.Append("   if (sVal == """).Append(_ucr.Content("OtherText", _languageCode, True)).Append("""){ ")
        sbJavaScript.Append("       var txtBox = document.getElementById(sender.txtId);")
        sbJavaScript.Append("       if (txtBox.value.length > 0){")
        If regularExpressionValue.Length > 0 Then
            sbJavaScript.Append("           var re = new RegExp(""").Append(regularExpressionValue).Append(""");")
            sbJavaScript.Append("           e.IsValid = re.test(txtBox.value);")
        Else
            sbJavaScript.Append("           e.IsValid = true;")
        End If
        sbJavaScript.Append("       }else{")
        If isMandatory Then
            sbJavaScript.Append("           e.IsValid = false;}")
        Else
            sbJavaScript.Append("           e.IsValid = true;}")
        End If
        sbJavaScript.Append("}}")
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "ScriptForSelectBox", sbJavaScript.ToString(), True)
    End Sub

    ''' <summary>
    ''' The checkbox validation script
    ''' </summary>
    ''' <param name="chkQuestionCheck">The current question checkbox</param>
    ''' <remarks></remarks>
    Private Sub CheckboxValidation(ByRef chkQuestionCheck As CheckBox)
        Dim sbJavaScript As New StringBuilder
        sbJavaScript.Append("function ValidateCheckboxAnswer(sender, e) {")
        sbJavaScript.Append("   var chkBox = document.getElementById(sender.chkId);")
        sbJavaScript.Append("   if (chkBox.checked == true) {")
        sbJavaScript.Append("       e.IsValid = true;")
        sbJavaScript.Append("   } else {")
        sbJavaScript.Append("       e.IsValid = false;")
        sbJavaScript.Append("   }")
        sbJavaScript.Append("}")
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "ValidateCheckboxAnswer", sbJavaScript.ToString(), True)
    End Sub

    ''' <summary>
    ''' Load the comments from the table into the repeater
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadCommentsForEditing()
        If _customerActivities.ResultDataSet IsNot Nothing Then
            If _customerActivities.ResultDataSet.Tables("CustomerActivitiesComments") IsNot Nothing AndAlso _customerActivities.ResultDataSet.Tables("CustomerActivitiesComments").Rows.Count > 0 Then
                rptActivityComments.DataSource = _customerActivities.ResultDataSet.Tables("CustomerActivitiesComments")
                rptActivityComments.DataBind()
                hdfActivityCommentItemIndex.Value = rptActivityComments.Items.Count
            End If
        End If
    End Sub

    ''' <summary>
    ''' Load the file attachments from the table into the repeater
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadFileAttachmentsForEditing()
        Dim fileTypes() As String
        Dim fileTypeString As New StringBuilder
        Dim fileTypeStringForDisplay As New StringBuilder
        Dim invalidFileErrorMessage As String = String.Empty
        Dim counter As Integer = 0

        plhActivityFileAttachments.Visible = True
        _dtActivityTemplate = TDataObjects.ActivitiesSettings.GetActivityTemplatesTypeByBUTemplateID(_ucr.BusinessUnit, _templateId)
        If _dtActivityTemplate.Rows.Count = 0 Then
            _dtActivityTemplate = TDataObjects.ActivitiesSettings.GetActivityTemplatesTypeByTemplateID(_templateId)
        End If
        hdfMaxFileSize.Value = TEBUtilities.CheckForDBNull_Int(_dtActivityTemplate.Rows(0)("MAX_FILE_UPLOAD_SIZE")).ToString()
        fileTypes = _dtActivityTemplate.Rows(0)("ALLOWABLE_FILE_TYPES").ToString().ToLower().Split(",")
        If fileTypes IsNot Nothing AndAlso fileTypes.Length > 0 Then
            For Each fileType As String In fileTypes
                If counter > 0 Then
                    fileTypeString.Append(",")
                    fileTypeStringForDisplay.Append(", ")
                End If
                fileTypeString.Append(fileType)
                fileTypeStringForDisplay.Append(fileType)
                fileTypeStringForDisplay.Append("'").Append(", ").Append("'")
                counter += 1
            Next
        End If
        invalidFileErrorMessage = _ucr.Content("InvalidFileErrorText", _languageCode, True)
        invalidFileErrorMessage = invalidFileErrorMessage.Replace("[[ALLOWABLE_FILE_TYPES]]", fileTypeStringForDisplay.ToString())
        invalidFileErrorMessage = invalidFileErrorMessage.Replace("[[MAX_FILE_UPLOAD_SIZE]]", hdfMaxFileSize.Value)
        hdfAllowedFileTypes.Value = fileTypeString.ToString()
        hdfInvalidFileErrorMessage.Value = invalidFileErrorMessage
        If _customerActivities.ResultDataSet IsNot Nothing Then
            If _customerActivities.ResultDataSet.Tables("CustomerActivitiesFileAttachments") IsNot Nothing AndAlso _customerActivities.ResultDataSet.Tables("CustomerActivitiesFileAttachments").Rows.Count > 0 Then
                rptActivityFiles.DataSource = _customerActivities.ResultDataSet.Tables("CustomerActivitiesFileAttachments")
                rptActivityFiles.DataBind()
                hdfActivityFileItemIndex.Value = rptActivityFiles.Items.Count
            End If
        End If
    End Sub

    ''' <summary>
    ''' Hide everything on the user control except the success / error messages
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub hideAllItems()
        plhActivityHeader.Visible = False
        plhTemplateQuestions.Visible = False
        plhActivityFileAttachments.Visible = False
        plhComments.Visible = False
        plhButtons.Visible = False
    End Sub

    ''' <summary>
    ''' Clear the session values passed in from the HospitalityPreBookingPage
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub clearSessionValues()
        If Not String.IsNullOrEmpty(Session("HospitalityPreBookingProductCode")) Then
            Session.Remove("HospitalityPreBookingProductCode")
        End If
        If Not String.IsNullOrEmpty(Session("HospitalityPreBookingProductDescription")) Then
            Session.Remove("HospitalityPreBookingProductDescription")
        End If
        If Not String.IsNullOrEmpty(Session("HospitalityPreBookingPackageCode")) Then
            Session.Remove("HospitalityPreBookingPackageCode")
        End If
        If Not String.IsNullOrEmpty(Session("HospitalityPreBookingPackageDescription")) Then
            Session.Remove("HospitalityPreBookingPackageDescription")
        End If
    End Sub
#End Region
#Region "Private Functions"

    ''' <summary>
    ''' Format the RFV or RegEx error based on the given question text and error type
    ''' </summary>
    ''' <param name="questionText">The question text</param>
    ''' <param name="isRequiredFieldValidator">Is this a required field validator error (true) or regular expression validator (false)</param>
    ''' <returns>Formatted answer text</returns>
    ''' <remarks></remarks>
    Private Function getFormattedErrorText(ByVal questionText As String, ByVal isRequiredFieldValidator As Boolean) As String
        Dim errorMessage As String = String.Empty
        If isRequiredFieldValidator Then
            errorMessage = _ucr.Content("MandatoryQuestionError", _languageCode, True).Replace("<<QUESTION_TEXT>>", questionText)
        Else
            errorMessage = _ucr.Content("InvalidAnswerError", _languageCode, True).Replace("<<QUESTION_TEXT>>", questionText)
        End If
        Return errorMessage
    End Function

#End Region

End Class
