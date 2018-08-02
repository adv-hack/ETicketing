Imports System
Imports System.Data
Imports System.Xml
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Collections.Generic
Partial Class UserControls_CustomerSearch
    Inherits ControlBase
#Region "Class Level Fields"

    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _settings As New DESettings
    Private _errorObj As New ErrorObj

#End Region

#Region "Public Properties"
    Public Property SearchResults As DataSet
    Public Property ErrorList As New BulletedList
    Public ReadOnly Property GetSessionID As String
        Get
            Return Now.ToString("dd/MM/yyyy HH:mm:ss") & _
                Talent.Common.Utilities.FixStringLength(AgentProfile.Name, 10) & _
                Talent.Common.Utilities.FixStringLength(AgentProfile.Type, 10) & _
                Talent.Common.Utilities.FixStringLength(txtForename.Text, 20) & _
                Talent.Common.Utilities.FixStringLength(txtSurname.Text, 30) & _
                Talent.Common.Utilities.FixStringLength(txtPassportNumber.Text, 30) & _
                Talent.Common.Utilities.FixStringLength(txtAddressLine1.Text, 30) & _
                Talent.Common.Utilities.FixStringLength(txtAddressLine2.Text, 30) & _
                Talent.Common.Utilities.FixStringLength(txtAddressLine3.Text, 25) & _
                Talent.Common.Utilities.FixStringLength(txtAddressLine4.Text, 25) & _
                Talent.Common.Utilities.FixStringLength(txtAddressPostCode.Text, 8) & _
                Talent.Common.Utilities.FixStringLength(txtEmail.Text, 60)
        End Get
    End Property
#End Region
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "CustomerSearch.ascx"
        End With
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            PopulateFromQueryString()
            PopulateTextsAndAttributes()
        End If
        If Request.QueryString("displayMode") = "Contacts" Then
            plhCustomerNumber.Visible = True
        End If
    End Sub
    Protected Sub btnBack_Click(ByVal sender As Object, ByVal e As EventArgs)
        ClearDeliveryAddressCntrlSessions()
        txtForename.Text = String.Empty
        txtSurname.Text = String.Empty
        txtPassportNumber.Text = String.Empty
        txtAddressLine1.Text = String.Empty
        txtAddressLine2.Text = String.Empty
        txtAddressLine3.Text = String.Empty
        txtAddressLine4.Text = String.Empty
        txtAddressPostCode.Text = String.Empty
        txtEmail.Text = String.Empty
        txtPhoneNumber.Text = String.Empty

        If Not String.IsNullOrEmpty(Request.QueryString("CompanyNumber")) AndAlso Request.QueryString("displayMode") = "Contacts" Then
            Response.Redirect("~/PagesPublic/CRM/CompanyContacts.aspx?source=companysearch&CompanyNumber=" + Request.QueryString("CompanyNumber"))
        End If
    End Sub
    ''' <summary>
    ''' When redirecting from the OffCanvas menu the search fields are populated from the query string.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateFromQueryString()
        If Request.QueryString("Type") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Request.QueryString("Type")) Then
            If Request.QueryString("Type").ToUpper().Equals("SEARCH") AndAlso String.IsNullOrEmpty(txtForename.Text) _
                AndAlso String.IsNullOrEmpty(txtSurname.Text) AndAlso String.IsNullOrEmpty(txtAddressLine1.Text) _
                AndAlso String.IsNullOrEmpty(txtAddressLine2.Text) AndAlso String.IsNullOrEmpty(txtAddressLine3.Text) _
                AndAlso String.IsNullOrEmpty(txtAddressLine4.Text) AndAlso String.IsNullOrEmpty(txtAddressPostCode.Text) _
                AndAlso String.IsNullOrEmpty(txtEmail.Text) Then
                txtForename.Text = Request.QueryString("Forename")
                txtSurname.Text = Request.QueryString("Surname")
                txtAddressLine1.Text = Request.QueryString("AddressLine1")
                txtAddressLine2.Text = Request.QueryString("AddressLine2")
                txtAddressLine3.Text = Request.QueryString("AddressLine3")
                txtAddressLine4.Text = Request.QueryString("AddressLine4")
                txtAddressPostCode.Text = Request.QueryString("PostCode")
                txtEmail.Text = Request.QueryString("Email")
                txtPhoneNumber.Text = Request.QueryString("PhoneNumber")
            End If
        End If
    End Sub

    Private Sub PopulateTextsAndAttributes()
        ltlCustomerSearchFormHeader.Text = _ucr.Content("CustomerSearchFormHeaderText", _languageCode, True)
        ltlContactNumber.Text = _ucr.Content("ContactNumberText", _languageCode, True)
        ltlForenameLabel.Text = _ucr.Content("ForenameText", _languageCode, True)
        ltlSurnameLabel.Text = _ucr.Content("SurnameText", _languageCode, True)
        ltlPassportNumberLabel.Text = _ucr.Content("PassportNumberText", _languageCode, True)
        ltlAddressLine1Label.Text = _ucr.Content("AddressLine1Text", _languageCode, True)
        ltlAddressLine2Label.Text = _ucr.Content("AddressLine2Text", _languageCode, True)
        ltlAddressLine3Label.Text = _ucr.Content("AddressLine3Text", _languageCode, True)
        ltlAddressLine4Label.Text = _ucr.Content("AddressLine4Text", _languageCode, True)
        ltlAddressPostCodeLabel.Text = _ucr.Content("AddressPostCodeText", _languageCode, True)
        ltlEmailLabel.Text = _ucr.Content("EmailText", _languageCode, True)
        ltlPhoneNumber.Text = _ucr.Content("PhoneNumberText", _languageCode, True)
        plhPassportNumber.Visible = Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowPassportNumberCriteria"))

        'btnPerformCustomerSearch.Text = _ucr.Content("PerformCustomerSearchButtonText", _languageCode, True)
        'btnBack.Text = _ucr.Content("BackButtonText", _languageCode, True)
        btnPerformCustomerSearch.Attributes.Add("title", _ucr.Content("PerformCustomerSearchButtonText", _languageCode, True))
        btnBack.Attributes.Add("title", _ucr.Content("BackButtonText", _languageCode, True))

        txtContactNumber.Attributes.Add("placeholder", ltlContactNumber.Text)
        txtForename.Attributes.Add("placeholder", ltlForenameLabel.Text)
        txtSurname.Attributes.Add("placeholder", ltlSurnameLabel.Text)
        txtPassportNumber.Attributes.Add("placeholder", ltlPassportNumberLabel.Text)
        txtAddressLine1.Attributes.Add("placeholder", ltlAddressLine1Label.Text)
        txtAddressLine2.Attributes.Add("placeholder", ltlAddressLine2Label.Text)
        txtAddressLine3.Attributes.Add("placeholder", ltlAddressLine3Label.Text)
        txtAddressLine4.Attributes.Add("placeholder", ltlAddressLine4Label.Text)
        txtAddressPostCode.Attributes.Add("placeholder", ltlAddressPostCodeLabel.Text)
        txtEmail.Attributes.Add("placeholder", ltlEmailLabel.Text)
        txtPhoneNumber.Attributes.Add("placeholder", ltlPhoneNumber.Text)

        If Not Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("AddressLine1")) Then plhAddressLine1Row.Visible = False
        If Not Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("AddressLine2")) Then plhAddressLine2Row.Visible = False
        If Not Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("AddressLine3")) Then plhAddressLine3Row.Visible = False
        If Not Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("AddressLine4")) Then plhAddressLine4Row.Visible = False
        If Not Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("AddressPostCode")) Then plhAddressPostCodeRow.Visible = False

        Dim txtForenameMaxLength As String = _ucr.Attribute("ForenameMaxLength")
        Dim txtSurnameMaxLength As String = _ucr.Attribute("SurnameMaxLength")
        Dim txtPassportNumberMaxLength As String = _ucr.Attribute("PassportNumberMaxLength")
        Dim txtAddressLine1MaxLength As String = _ucr.Attribute("AddressLine1MaxLength")
        Dim txtAddressLine2MaxLength As String = _ucr.Attribute("AddressLine2MaxLength")
        Dim txtAddressLine3MaxLength As String = _ucr.Attribute("AddressLine3MaxLength")
        Dim txtAddressLine4MaxLength As String = _ucr.Attribute("AddressLine4MaxLength")
        Dim txtAddressPostCodeMaxLength As String = _ucr.Attribute("AddressPostCodeMaxLength")
        Dim txtEmailMaxLength As String = _ucr.Attribute("EmailMaxLength")
        Dim txtPhoneNumberMaxLength As String = _ucr.Attribute("PhoneNumberMaxLength")
        If Not String.IsNullOrEmpty(txtForenameMaxLength) Then txtForename.MaxLength = CInt(txtForenameMaxLength)
        If Not String.IsNullOrEmpty(txtSurnameMaxLength) Then txtSurname.MaxLength = CInt(txtSurnameMaxLength)
        If Not String.IsNullOrEmpty(txtPassportNumberMaxLength) Then txtPassportNumber.MaxLength = CInt(txtPassportNumberMaxLength)
        If Not String.IsNullOrEmpty(txtAddressLine1MaxLength) Then txtAddressLine1.MaxLength = CInt(txtAddressLine1MaxLength)
        If Not String.IsNullOrEmpty(txtAddressLine2MaxLength) Then txtAddressLine2.MaxLength = CInt(txtAddressLine2MaxLength)
        If Not String.IsNullOrEmpty(txtAddressLine3MaxLength) Then txtAddressLine3.MaxLength = CInt(txtAddressLine3MaxLength)
        If Not String.IsNullOrEmpty(txtAddressLine4MaxLength) Then txtAddressLine4.MaxLength = CInt(txtAddressLine4MaxLength)
        If Not String.IsNullOrEmpty(txtAddressPostCodeMaxLength) Then txtAddressPostCode.MaxLength = CInt(txtAddressPostCodeMaxLength)
        If Not String.IsNullOrWhiteSpace(txtEmailMaxLength) Then txtEmail.MaxLength = CInt(txtEmailMaxLength)
        If Not String.IsNullOrWhiteSpace(txtPhoneNumberMaxLength) Then txtPhoneNumber.MaxLength = CInt(txtPhoneNumberMaxLength)
    End Sub
End Class
