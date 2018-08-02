Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Data
Imports TalentBusinessLogic.ModelBuilders.CRM
Imports TalentBusinessLogic.Models
Imports System.Collections.Generic
Imports System.Web.Script.Serialization

Partial Class PagesPublic_CRM_CompanyContacts
    Inherits TalentBase01

#Region "Class Level Fields"
    Private _companyContactsInput As CompanyContactsInputModel
    Private _customerViewModelList As CompanyContactsViewModel
    Private _companyNumber As String = String.Empty
#End Region

#Region "Public Variables"
    Public CustomerNumberHeader As String = String.Empty
    Public ForenameHeader As String = String.Empty
    Public SurnameHeader As String = String.Empty
    Public TelephoneHeader As String = String.Empty
    Public SelectHeader As String = String.Empty
    Public DeleteHeader As String = String.Empty
    Public TalentAPIAddress As String = String.Empty
#End Region

#Region "Protected Page Events"
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        hdfCompanyNumber.Value = Request.QueryString("CompanyNumber")
        hdfAgentName.Value = AgentProfile.Name

        If Request.QueryString("CompanyNumber") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Request.QueryString("CompanyNumber").ToString) Then
            Session("SelectedCompanyNumber") = Request.QueryString("CompanyNumber").ToString
            _companyNumber = Request.QueryString("CompanyNumber").ToString
        ElseIf Session("SelectedCompanyNumber") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Session("SelectedCompanyNumber")) Then
            _companyNumber = Session("SelectedCompanyNumber")
        End If

        If Not AgentProfile.IsAgent OrElse String.IsNullOrEmpty(_companyNumber) Then
            Response.Redirect("~/PagesPublic/Profile/CustomerSelection.aspx?displayMode=ShowCompanySearch")
        End If
        'populate input model
        _companyContactsInput = New CompanyContactsInputModel
        _companyContactsInput.CompanyNumber = _companyNumber

        'populate viewmodel from company builder
        Dim companyBuilder As New CompanyModelBuilders()
        _customerViewModelList = companyBuilder.PopulateCompanyContacts(_companyContactsInput)

        If _customerViewModelList.Error.HasError Then
            errorList.Items.Add(_customerViewModelList.Error.ErrorMessage)
        End If

        rptCompanyContactResults.DataSource = _customerViewModelList.CompanyContacts
        rptCompanyContactResults.DataBind()

        TalentAPIAddress = ModuleDefaults.TalentAPIAddress
        hplSearch.Text = _customerViewModelList.GetPageText("SearchButtonText")
        hplSearch.NavigateUrl = "~/PagesPublic/Profile/CustomerSelection.aspx?displayMode=ShowCompanySearch"
        hplAddExistingCustomer.Text = _customerViewModelList.GetPageText("ExistingCustomerButtonText")
        hplAddExistingCustomer.NavigateUrl = "~/PagesPublic/Profile/CustomerSelection.aspx?displayMode=Contacts&CompanyNumber=" + _companyNumber
        btnAddLoggedInCustomer.Text = _customerViewModelList.GetPageText("AddLoggedInCustomerButtonText")
        CustomerNumberHeader = _customerViewModelList.GetPageText("CustomerNumberHeaderText")
        ForenameHeader = _customerViewModelList.GetPageText("ForenameHeaderText")
        SurnameHeader = _customerViewModelList.GetPageText("SurnameHeaderText")
        TelephoneHeader = _customerViewModelList.GetPageText("TelephoneHeaderText")
        SelectHeader = _customerViewModelList.GetPageText("SelectHeaderText")
        DeleteHeader = _customerViewModelList.GetPageText("DeleteHeaderText")
        btnRegisterAndAddContact.Text = _customerViewModelList.GetPageText("RegisterAndAddContactButtonText")
        hdfRootURL.Value = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/")
    End Sub
    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (errorList.Items.Count > 0)
        'Check Agent Authorities for the company/contact actions allowed.
        btnRegisterAndAddContact.Visible = AgentProfile.AgentPermissions.CanAddContactToCompany
        If Not Profile.IsAnonymous AndAlso AgentProfile.AgentPermissions.CanAddContactToCompany AndAlso Not LoggedInCustomerIsCompanyContact() Then
            btnAddLoggedInCustomer.Visible = True
        End If
        hplAddExistingCustomer.Visible = AgentProfile.AgentPermissions.CanAddContactToCompany
        hdfAllowDelete.Value = ""
        If AgentProfile.AgentPermissions.CanRemoveContactFromCompany Then
            hdfAllowDelete.Value = "true"
        End If

    End Sub
    Protected Sub btnRegisterAndAddContact_Click(sender As Object, e As EventArgs) Handles btnRegisterAndAddContact.Click
        Dim registrationURL As New StringBuilder
        registrationURL.Append("~/PagesPublic/Profile/Registration.aspx").Append("?source=companycontacts")
        registrationURL.Append("&returnurl=").Append("~/PagesPublic/CRM/CompanyContacts.aspx")
        registrationURL.Append("&CompanyNumber=").Append(Session("SelectedCompanyNumber")).Append("""")
        Response.Redirect(registrationURL.ToString)
    End Sub

    Protected Sub btnAddLoggedInCustomer_Click(sender As Object, e As EventArgs) Handles btnAddLoggedInCustomer.Click
        Dim companyBuilder As New CompanyModelBuilders()
        Dim customerCompanyInputModel As New CustomerCompanyInputModel()
        customerCompanyInputModel.CompanyNumber = _companyNumber
        customerCompanyInputModel.CustomerNumber = Profile.UserName
        customerCompanyInputModel.AgentName = AgentProfile.Name
        Dim errorModel As ErrorModel = companyBuilder.AddCustomerCompanyAssociation(customerCompanyInputModel)
        If errorModel.HasError Then
            errorList.Items.Add(errorModel.ErrorMessage)
        Else
            Response.Redirect(Request.RawUrl)
        End If

    End Sub
#End Region

#Region "Private Functions"
    Private Function formatUrlForLocalHost(ByVal url As String) As String
        Dim formattedUrl As String = url
        If Request.Url.AbsoluteUri.Contains("localhost") AndAlso url.ToUpper().Contains("TALENTEBUSINESS") AndAlso url.ToUpper().IndexOf("/TALENTEBUSINESS") > 0 Then
            formattedUrl = url.Substring(url.ToUpper().IndexOf("/TALENTEBUSINESS"), url.Length - url.ToUpper().IndexOf("/TALENTEBUSINESS"))
            Dim port As String = Request.Url.Port
        Else
            formattedUrl = "~" & formattedUrl
        End If
        Return formattedUrl
    End Function
    Private Shared Function RetrieveAgentDetailsFromSessionID(ByVal sessionID As String) As DataTable
        Dim dataObjects As New TalentDataObjects
        dataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        Return dataObjects.AgentSettings.TblAgent.RetrieveAgentDetailsFromSessionID(sessionID)
    End Function

    Private Shared Function CheckAgentLogin(ByVal sessionID As String) As Boolean
        Dim agentDetails As New DataTable()
        Dim retVal As Boolean = True
        agentDetails = RetrieveAgentDetailsFromSessionID(sessionID)
        If agentDetails.Rows.Count = 0 Then
            retVal = False
        Else
            retVal = True
        End If
        Return retVal
    End Function

    Private Function LoggedInCustomerIsCompanyContact() As Boolean
        Dim isValid As Boolean = False
        For Each contact As CompanyContactModel In _customerViewModelList.CompanyContacts
            If contact.CustomerNumber = Profile.UserName Then
                isValid = True
            End If
        Next
        Return isValid
    End Function
#End Region

#Region "Web methods"
    <System.Web.Services.WebMethod()>
    Public Shared Function GetRedirectUrl(ByVal SessionID As String, ByVal customerNumber As String) As String
        If Not CheckAgentLogin(SessionID) Then
            Throw New System.Exception("Invalid session.")
        End If
        Dim clubsTable As Data.DataTable = Talent.eCommerce.Utilities.GetClubs()
        Dim clubs As Data.DataRow() = clubsTable.Select("CLUB_CODE = '" & TalentCache.GetBusinessUnit & "'")
        Dim customerForwardUrl As New StringBuilder
        If clubs.Length > 0 Then
            Dim club As Data.DataRow = clubs(0)
            customerForwardUrl.Append(club("STANDARD_CUSTOMER_FORWARD_URL"))
            customerForwardUrl.Append("&session_id=").Append(SessionID)
            customerForwardUrl.Append("&t=1")
            customerForwardUrl.Append("&l=").Append(Talent.Common.Utilities.TripleDESEncode(customerNumber, club("NOISE_ENCRYPTION_KEY")))
            customerForwardUrl.Append("&p=").Append(Talent.Common.Utilities.TripleDESEncode(Talent.Common.Utilities.RandomString(10), club("NOISE_ENCRYPTION_KEY")))
            customerForwardUrl.Append("&y=N")
        End If
        Return customerForwardUrl.ToString
    End Function
#End Region


End Class
