Imports System
Imports System.Data
Imports System.Xml
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Collections.Generic
Partial Class UserControls_CompanySearch
    Inherits ControlBase
#Region "Class Level Fields"

    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _settings As New DESettings
    Private _errorObj As New ErrorObj

#End Region
    Public Property SearchResults As DataSet
    Public Property ErrorList As New BulletedList

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "CompanySearch.ascx"
        End With
    End Sub


    Private Sub PopulateTextAndAttributes()
        Dim childCompanyNumber As String = "0"
        Dim parentCompanyNumber As String = "0"
        Dim parentCompanyName As String = String.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("ChildCompanyNumber")) Then
            childCompanyNumber = Request.QueryString("ChildCompanyNumber")
        End If
        If Not String.IsNullOrEmpty(Request.QueryString("ParentCompanyNumber")) Then
            parentCompanyNumber = Request.QueryString("ParentCompanyNumber")
        End If
        If Not String.IsNullOrEmpty(Request.QueryString("ParentCompanyName")) Then
            parentCompanyName = Request.QueryString("ParentCompanyName")
            ltlParentCompanyName.Text = parentCompanyName
        End If
        ltlCompanySearchFormHeader.Text = _ucr.Content("CompanySearchFormHeaderText", _languageCode, True)
        hplAdd.Visible = True
        hplAdd.NavigateUrl = "~/PagesPublic/CRM/CompanyUpdate.aspx?CompanyUpdatePageMode=Add&source=companysearch"
        If Request.QueryString("displayMode") IsNot Nothing Then
            If Request.QueryString("displayMode") = "Parent" Then
                hplBack.Visible = True
                ltlCompanySearchFormHeader.Text = _ucr.Content("ParentSearchFormHeaderText", _languageCode, True)
                hplAdd.NavigateUrl = "~/PagesPublic/CRM/CompanyUpdate.aspx?CompanyUpdatePageMode=Add&source=Parent&AddType=NewParent&ChildCompanyNumber=" + childCompanyNumber
            ElseIf Request.QueryString("displayMode") = "AddToNull" OrElse Request.QueryString("displayMode") = "hdfAddSubsidiaries" Then
                hplBack.Visible = True
                hplAdd.Visible = False
                ltlCompanySearchFormHeader.Text = _ucr.Content("ParentSearchFormHeaderText", _languageCode, True)
            ElseIf Request.QueryString("displayMode") = "Subsidiaries" Then
                hplBack.Visible = True
                ltlCompanySearchFormHeader.Text = _ucr.Content("SubsideriesSearchFormHeaderText", _languageCode, True)
                hplAdd.NavigateUrl = "~/PagesPublic/CRM/CompanyUpdate.aspx?CompanyUpdatePageMode=Add&AddType=AddToNull&source=ParentSearch&ParentCompanyNumber=" + parentCompanyNumber + "&parentCompanyName=" + parentCompanyName
            End If
            hplBack.NavigateUrl = "~/PagesPublic/CRM/CompanyUpdate.aspx?CompanyNumber=" + childCompanyNumber + "&CompanyUpdatePageMode=Update&Source=companysearch"
        End If
        ltlCompanyName.Text = _ucr.Content("CompanyNameText", _languageCode, True)
        ltlAddressLine1.Text = _ucr.Content("AddressLine1Text", _languageCode, True)
        ltlPostCode.Text = _ucr.Content("PostCodeText", _languageCode, True)
        ltlWebAddress.Text = _ucr.Content("WebAddressText", _languageCode, True)
        ltlTelephoneNumber.Text = _ucr.Content("TelephoneNumberText", _languageCode, True)
        hplBack.Attributes.Add("title", _ucr.Content("btnAddButtonText", _languageCode, True))
        btnPerformCompanySearch.Attributes.Add("title", _ucr.Content("PerformCompanySearchButtonText", _languageCode, True))
        hplAdd.Attributes.Add("title", _ucr.Content("btnAddButtonText", _languageCode, True))

        txtCompanyName.Attributes.Add("placeholder", ltlCompanyName.Text)
        txtCompanyAddressLine1.Attributes.Add("placeholder", ltlAddressLine1.Text)
        txtCompanyPostCode.Attributes.Add("placeholder", ltlPostCode.Text)
        txtCompanyWebAddress.Attributes.Add("placeholder", ltlWebAddress.Text)
        txtCompanyTelephoneNumber.Attributes.Add("placeholder", ltlTelephoneNumber.Text)

    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            PopulateTextAndAttributes()
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If hplAdd.Visible Then
            hplAdd.Visible = AgentProfile.AgentPermissions.CanAddCompany
        End If
    End Sub


End Class
