Imports System.Data
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders.CRM
Imports Talent.eCommerce

Partial Class PagesAgent_CRM_ActivitiesList
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _dtTemplates As DataTable = Nothing
    Private _activitiesViewModel As ActivitiesListViewModel
    Private _redirectUrl As String = String.Empty

#End Region

#Region "Public Properties"

    Public CustomerHeaderText As String
    Public UserHeaderText As String
    Public StatusHeaderText As String
    Public DateHeaderText As String
    Public SubjectHeaderText As String
    Public ActionsHeaderText As String
    Public SearchActivityText As String

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Check if agent has access on Activities List menu item
        If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessActivities) Or Not AgentProfile.IsAgent Then
            Dim inputModel As ActivitiesListInputModel = setupInputModel()
            processController(inputModel)
            createView()
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If Not String.IsNullOrEmpty(Session("SuccessfullyAddedActivity")) AndAlso Session("SuccessfullyAddedActivity") = True Then
            blSuccessMessages.Items.Add(_activitiesViewModel.GetPageText("SuccessfullyAddedActivity"))
        End If
        If Not String.IsNullOrEmpty(Session("SuccessfullyUpdatedActivity")) AndAlso Session("SuccessfullyUpdatedActivity") = True Then
            blSuccessMessages.Items.Add(_activitiesViewModel.GetPageText("SuccessfullyUpdatedActivity"))
        End If
        revDate.ValidationExpression = _activitiesViewModel.GetPageAttribute("DateFieldRegex")
        revDate.ErrorMessage = _activitiesViewModel.GetPageText("IncorrectDateFormatErrorMessage")
        Session("SuccessfullyAddedActivity") = False
        Session("SuccessfullyUpdatedActivity") = False
        plhErrorList.Visible = (blErrorMessages.Items.Count > 0)
        plhSuccessList.Visible = (blSuccessMessages.Items.Count > 0)
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Setup the activities input model and return the model for use
    ''' </summary>
    ''' <returns>The formatted input model based on form data</returns>
    ''' <remarks></remarks>
    Private Function setupInputModel() As ActivitiesListInputModel
        Dim inputModel As New ActivitiesListInputModel
        If IsPostBack Then
            If Request.Params("__EVENTTARGET") = lbtnCreate.UniqueID Then
                inputModel.ActivitySubject = txtSubject.Text
                inputModel.TemplateID = Request.Params(ddlActivityTemplate.UniqueID)
                inputModel.ActivityUserName = Request.Params(ddlUser.UniqueID)
                inputModel.ActivityStatus = Request.Params(ddlStatus.UniqueID)
                inputModel.ActivityDate = txtDate.Text
            End If
        Else
            If Not String.IsNullOrEmpty(Request.QueryString("Subject")) Then
                txtSubject.Text = Request.QueryString("Subject")
            End If
            If Not String.IsNullOrEmpty(Request.QueryString("Date")) Then
                txtDate.Text = Request.QueryString("Date")
            End If
        End If
        Return inputModel
    End Function

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Process the controller, the code actioning the input values if there are any.
    ''' </summary>
    ''' <param name="inputModel">The input model being worked with</param>
    ''' <remarks></remarks>
    Private Sub processController(ByVal inputModel As ActivitiesListInputModel)
        Dim builder As New ActivitiesModelBuilder
        _activitiesViewModel = builder.ActivitiesList(inputModel)
        If IsPostBack Then
            If Request.Params("__EVENTTARGET") = lbtnCreate.UniqueID Then
                Dim activityTemplateId As Integer = _activitiesViewModel.TemplateID
                If activityTemplateId = 0 Then
                    Dim selectActivityMessage As String = _activitiesViewModel.GetPageText("SelectAnActivityText")
                    Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(selectActivityMessage)
                    If listItemObject Is Nothing Then blErrorMessages.Items.Add(selectActivityMessage)
                Else
                    Session("ActivityUserName") = _activitiesViewModel.ActivityUserName
                    Session("ActivitySubject") = _activitiesViewModel.ActivitySubject
                    Session("ActivityDate") = _activitiesViewModel.ActivityDate
                    Session("ActivityStatus") = _activitiesViewModel.ActivityStatus
                    _redirectUrl = String.Format("ActivitiesEdit.aspx?TemplateId={0}&Id=0", activityTemplateId)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Create the view of the page based on attributes and text values, unless there is a URL to redirect to
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createView()
        If _redirectUrl.Length > 0 Then
            Response.Redirect(_redirectUrl)
        Else
            populateTextAndAttributes()
            populateActivityDropDownList()
            populateActivityStatusDropDownList()
            populateUsers()
            populateHiddenfields()
        End If
    End Sub

    ''' <summary>
    ''' Populate the page text values for labels and column headings and any attribute values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateTextAndAttributes()
        lblSubject.Text = _activitiesViewModel.GetPageText("SubjectTextLabel")
        lblActivity.Text = _activitiesViewModel.GetPageText("ActivityLabel")
        lblUser.Text = _activitiesViewModel.GetPageText("UserLabel")
        lblStatus.Text = _activitiesViewModel.GetPageText("StatusLabel")
        lblDate.Text = _activitiesViewModel.GetPageText("DateLabel")
        txtSubject.Attributes.Add("placeholder", lblSubject.Text)
        txtDate.Attributes.Add("placeholder", lblDate.Text)
        lbtnCreate.Attributes.Add("title", _activitiesViewModel.GetPageText("CreateButtonText"))
        ltlSuccessfullyDeletedActivity.Text = _activitiesViewModel.GetPageText("SuccessfullyDeletedActivityText")

        CustomerHeaderText = _activitiesViewModel.GetPageText("CustomerHeaderText")
        UserHeaderText = _activitiesViewModel.GetPageText("UserHeaderText")
        StatusHeaderText = _activitiesViewModel.GetPageText("StatusHeaderText")
        DateHeaderText = _activitiesViewModel.GetPageText("DateHeaderText")
        SubjectHeaderText = _activitiesViewModel.GetPageText("SubjectHeaderText")
        ActionsHeaderText = _activitiesViewModel.GetPageText("ActionsHeaderText")
        SearchActivityText = _activitiesViewModel.GetPageText("SearchButtonText")

        If Profile.IsAnonymous Then
            plhCreateActivityWarning.Visible = True
            lbtnCreate.OnClientClick = "return cannotCreateActivity();"
            ltlCreateActivityWarning.Text = _activitiesViewModel.GetPageText("CreateActivityWarningText")
        Else
            plhCreateActivityWarning.Visible = False
        End If
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error-handling.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("error-handling.js", "/Application/Status/"), False)
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "activities.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("activities.js", "/Module/CRM/"), False)
    End Sub

    ''' <summary>
    ''' Populate the list of activities into the drop down list based on business unit
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateActivityDropDownList()
        ddlActivityTemplate.DataSource = _activitiesViewModel.TemplatesList
        ddlActivityTemplate.DataTextField = "NAME"
        ddlActivityTemplate.DataValueField = "TEMPLATE_ID"
        ddlActivityTemplate.DataBind()
        ddlActivityTemplate.Items.Insert(0, New ListItem(_activitiesViewModel.GetPageText("AllActivitiesText"), 0))
        If Not String.IsNullOrEmpty(Request.QueryString("ActivityId")) Then
            Dim listItem As ListItem = ddlActivityTemplate.Items.FindByValue(Request.QueryString("ActivityId"))
            If listItem IsNot Nothing Then ddlActivityTemplate.SelectedValue = listItem.Value
        End If
    End Sub

    ''' <summary>
    ''' Retreive the list of agent data and populate the drop down list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateUsers()
        ddlUser.Items.Clear()
        ddlUser.DataSource = _activitiesViewModel.AgentList
        ddlUser.DataTextField = "UserName"
        ddlUser.DataValueField = "UserCode"
        ddlUser.DataBind()
        ddlUser.Items.Insert(0, New ListItem(_activitiesViewModel.GetPageText("AllAgentsText"), String.Empty))
        If Not String.IsNullOrEmpty(Request.QueryString("User")) Then
            Dim listItem As ListItem = ddlUser.Items.FindByValue(Request.QueryString("User"))
            If listItem IsNot Nothing Then ddlUser.SelectedValue = listItem.Value
        End If
    End Sub

    ''' <summary>
    ''' Get the status values and bind them to the drop down list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateActivityStatusDropDownList()
        ddlStatus.DataSource = _activitiesViewModel.StatusList
        ddlStatus.DataTextField = "DESCRIPTION"
        ddlStatus.DataValueField = "DESCRIPTION"
        ddlStatus.DataBind()
        ddlStatus.Items.Insert(0, New ListItem(_activitiesViewModel.GetPageText("AnyStatusText"), String.Empty))
        If Not String.IsNullOrEmpty(Request.QueryString("Status")) Then
            Dim listItem As ListItem = ddlStatus.Items.FindByValue(Request.QueryString("Status"))
            If listItem IsNot Nothing Then ddlStatus.SelectedValue = listItem.Value
        End If
    End Sub

    ''' <summary>
    ''' Populate the hidden field to be used for the TalentAPI integration
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateHiddenfields()
        If Profile.IsAnonymous Then
            hdfCustomerNumber.Value = GlobalConstants.GENERIC_CUSTOMER_NUMBER
        Else
            hdfCustomerNumber.Value = Profile.User.Details.LoginID
        End If
        hdfTalentAPIUrl.Value = ModuleDefaults.TalentAPIAddress
        hdfDataTablesLengthMenu.Value = _activitiesViewModel.GetPageText("DataTablesLengthMenu")
        hdfDataTablesZeroRecords.Value = _activitiesViewModel.GetPageText("DataTablesZeroRecords")
        hdfDataTablesInfo.Value = _activitiesViewModel.GetPageText("DataTablesInfo")
        hdfDataTablesInfoEmpty.Value = _activitiesViewModel.GetPageText("DataTablesInfoEmpty")
        hdfDataTablesInfoFiltered.Value = _activitiesViewModel.GetPageText("DataTablesInfoFiltered")
        hdfDataTablesPreviousPage.Value = _activitiesViewModel.GetPageText("DataTablesPreviousPage")
        hdfDataTablesNextPage.Value = _activitiesViewModel.GetPageText("DataTablesNextPage")
        hdfDatePickerClearDateText.Value = _activitiesViewModel.GetPageText("DatePickerClearDateText")

        hdfAlertifyDeleteActivityTitle.Value = _activitiesViewModel.GetPageText("AlertifyDeleteActivityTitleText")
        hdfAlertifyDeleteActivityMessage.Value = _activitiesViewModel.GetPageText("AlertifyDeleteActivityMessageText")
        hdfAlertifyOK.Value = _activitiesViewModel.GetPageText("AlertifyOKText")
        hdfAlertifyCancel.Value = _activitiesViewModel.GetPageText("AlertifyCancelText")
    End Sub

#End Region

End Class
