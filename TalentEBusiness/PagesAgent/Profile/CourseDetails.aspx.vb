Imports Talent.Common
Imports Talent.eCommerce
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesLogin_Profile_CourseDetails
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfrPage As WebFormResource = Nothing
    Private _languageCode As String = Nothing

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        'Check if agent has access on Emergency Contact Details menu item
        If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessEmergencyContactDetails) Or Not AgentProfile.IsAgent Then
            _wfrPage = New WebFormResource
            _languageCode = TCUtilities.GetDefaultLanguage()
            With _wfrPage
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "CourseDetails.aspx"
            End With

            setPageTextValues()
            If Not IsPostBack Then loadCurrentCourseDetails()
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhErrorMessage.Visible = (ltlErrorMessage.Text.Length > 0)
        plhSuccessMessage.Visible = (ltlSuccessMessage.Text.Length > 0)
    End Sub

    Protected Sub btnConfirm_Click(sender As Object, e As System.EventArgs) Handles btnConfirm.Click
        If Page.IsValid Then
            updateCurrentCourseDetails()
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set the form field label values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setPageTextValues()
        ltlCourseDetailsLegend.Text = _wfrPage.Content("FieldsetLegend", _languageCode, True)
        lblCourseDetailsFanFlag.Text = _wfrPage.Content("FanFlagLabel", _languageCode, True)
        lblCourseDetailsContactNumber.Text = _wfrPage.Content("ContactNumberLabel", _languageCode, True)
        lblCourseDetailsContactName.Text = _wfrPage.Content("ContactNameLabel", _languageCode, True)
        lblCourseDetailsMedicalInfo.Text = _wfrPage.Content("MedicalInfoLabel", _languageCode, True)
        regCourseDetailsMedicalInfo.ErrorMessage = _wfrPage.Content("MedicalInfoValidationError", _languageCode, True)
        btnConfirm.Text = _wfrPage.Content("ConfirmButtonText", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Load the current course details into the form for the current customer
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub loadCurrentCourseDetails()
        Dim err As New ErrorObj
        Dim talCustomer As New TalentCustomer
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim customerDataEntity As New DECustomer
        Dim customerList As New DECustomerV11
        customerDataEntity.CustomerNumber = Profile.User.Details.LoginID
        customerList.DECustomersV1.Add(customerDataEntity)
        settings.Company = AgentProfile.GetAgentCompany()
        talCustomer.Settings = settings
        talCustomer.DeV11 = customerList
        err = talCustomer.RetrieveCourseDetails()

        If err.HasError Then
            ltlErrorMessage.Text = _wfrPage.Content("ErrorRetrievingCourseDetails", _languageCode, True)
            chkCourseDetailsFanFlag.Enabled = False
            txtCourseDetailsContactName.ReadOnly = True
            txtCourseDetailsContactNumber.ReadOnly = True
            txtCourseDetailsMedicalInfo.ReadOnly = True
            btnConfirm.Enabled = False
        Else
            chkCourseDetailsFanFlag.Checked = talCustomer.CourseDetailsFanFlag
            txtCourseDetailsContactName.Text = talCustomer.CourseDetailsContactName
            txtCourseDetailsContactNumber.Text = talCustomer.CourseDetailsContactNumber
            txtCourseDetailsMedicalInfo.Text = talCustomer.CourseDetailsMedicalInfo
        End If
    End Sub

    ''' <summary>
    ''' Update the current course details for this customer
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub updateCurrentCourseDetails()
        Dim err As New ErrorObj
        Dim talCustomer As New TalentCustomer
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim customerDataEntity As New DECustomer
        Dim customerList As New DECustomerV11
        customerDataEntity.CustomerNumber = Profile.User.Details.LoginID
        customerList.DECustomersV1.Add(customerDataEntity)
        settings.Company = AgentProfile.GetAgentCompany()
        talCustomer.Settings = settings
        talCustomer.DeV11 = customerList
        talCustomer.CourseDetailsFanFlag = chkCourseDetailsFanFlag.Checked
        talCustomer.CourseDetailsContactName = txtCourseDetailsContactName.Text.Trim()
        talCustomer.CourseDetailsContactNumber = txtCourseDetailsContactNumber.Text.Trim()
        talCustomer.CourseDetailsMedicalInfo = txtCourseDetailsMedicalInfo.Text.Trim()
        err = talCustomer.UpdateCourseDetails()

        If err.HasError Then
            ltlErrorMessage.Text = _wfrPage.Content("ErrorUpdatingCourseDetails", _languageCode, True)
        Else
            ltlSuccessMessage.Text = _wfrPage.Content("SuccessfullyUpdatedCourseDetails", _languageCode, True)
        End If
    End Sub

#End Region

End Class
