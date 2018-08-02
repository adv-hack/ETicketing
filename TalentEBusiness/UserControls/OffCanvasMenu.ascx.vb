Imports Talent.Common
Imports Talent.eCommerce
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Collections.Generic

Partial Class UserControls_OffCanvasMenu
    Inherits ControlBase
#Region "Class Level Fields"
    Private _ucr As Talent.Common.UserControlResource = Nothing
    Private _languageCode As String = Nothing
#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _ucr = New Talent.Common.UserControlResource
        _languageCode = TCUtilities.GetDefaultLanguage
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = TEBUtilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "OffCanvasMenu.ascx"
        End With
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If AgentProfile.IsAgent Then
            PopulateTexts()
            DisplayLastNCustomersUsed()
        Else
            Me.Visible = False
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim customerSelectionRedirect As New StringBuilder
        customerSelectionRedirect.Append("~/PagesPublic/Profile/CustomerSelection.aspx")
        customerSelectionRedirect.Append("?Type=Submit").Append("&FancardBox=").Append(fancardBox.Text)
        customerSelectionRedirect.Append("&MembershipNumber=").Append(txtMembershipNumber.Text)
        Dim returnUrl As String
        If HttpUtility.ParseQueryString(Request.UrlReferrer.Query())("ReturnUrl") IsNot String.Empty AndAlso HttpUtility.ParseQueryString(Request.UrlReferrer.Query())("ReturnUrl") IsNot Nothing Then
            returnUrl = HttpUtility.ParseQueryString(Request.UrlReferrer.Query())("ReturnUrl")
        Else
            returnUrl = Request.Url.AbsoluteUri
        End If

        customerSelectionRedirect.Append("&ReturnUrl=").Append(Server.UrlEncode(returnUrl))

        Response.Redirect(customerSelectionRedirect.ToString())
    End Sub

    Protected Sub btnPerformCustomerSearch_Click(sender As Object, e As EventArgs) Handles btnPerformCustomerSearch.Click
        Dim customerSelectionRedirect As New StringBuilder
        customerSelectionRedirect.Append("~/PagesPublic/Profile/CustomerSelection.aspx")
        customerSelectionRedirect.Append("?Type=Search").Append("&Forename=").Append(txtForename.Text)
        customerSelectionRedirect.Append("&Surname=").Append(txtSurname.Text).Append("&AddressLine1=").Append(txtAddressLine1.Text)
        customerSelectionRedirect.Append("&AddressLine2=").Append(txtAddressLine2.Text).Append("&AddressLine3=").Append(txtAddressLine3.Text)
        customerSelectionRedirect.Append("&AddressLine4=").Append(txtAddressLine4.Text).Append("&PostCode=").Append(txtAddressPostCode.Text)
        customerSelectionRedirect.Append("&Email=").Append(txtEmail.Text)
        customerSelectionRedirect.Append("&ReturnUrl=").Append(Server.UrlEncode(Request.Url.AbsoluteUri))
        Response.Redirect(customerSelectionRedirect.ToString())
    End Sub


    ''' <summary>
    ''' Bind the NavigationURL at runtime based on the CustomerNumber
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub rptLastNCustomerLogins_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptLastNCustomerLogins.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim navigateURL As New StringBuilder
            Dim customerNo As String = String.Empty
            Dim hplLastCustomerLogin As HyperLink = CType(e.Item.FindControl("hplLastCustomerLogin"), HyperLink)
            Dim randStr As String
            randStr = TCUtilities.TripleDESEncode(TCUtilities.RandomString(10), ModuleDefaults.NOISE_ENCRYPTION_KEY)
            customerNo = TCUtilities.TripleDESEncode(e.Item.DataItem(0).ToString(), ModuleDefaults.NOISE_ENCRYPTION_KEY)

            navigateURL.Append("~/redirect/ticketinggateway.aspx?page=validatesession.aspx&function=validatesession")
            navigateURL.Append("&t=1")
            navigateURL.Append("&l=").Append(Server.UrlEncode(customerNo))
            navigateURL.Append("&p=").Append(Server.UrlEncode(randStr))
            navigateURL.Append("&y=N")
            navigateURL.Append("&ReturnUrl=").Append(Server.UrlEncode(Request.Url.AbsoluteUri))

            If e.Item.FindControl("hplLastCustomerLogin") IsNot Nothing Then
                hplLastCustomerLogin.Text = e.Item.DataItem(1).ToString()
                hplLastCustomerLogin.NavigateUrl = navigateURL.ToString()
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"
 

    Private Sub PopulateTexts()
        plhMembershipNumber.Visible = TCUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("Show_MembershipNumber"))
        fancardLabel.Text = _ucr.Content("FancardLabelText", _languageCode, True)
        btnSubmit.Text = _ucr.Content("SubmitButtonText", _languageCode, True)

        ltlCustomerSearchFormHeader.Text = _ucr.Content("CustomerSearchFormHeaderText", _languageCode, True)
        lblMembershipNumber.Text = _ucr.Content("MembershipNumberText", _languageCode, True)
        ltlForenameLabel.Text = _ucr.Content("ForenameText", _languageCode, True)
        ltlSurnameLabel.Text = _ucr.Content("SurnameText", _languageCode, True)
        ltlAddressLine1Label.Text = _ucr.Content("AddressLine1Text", _languageCode, True)
        ltlAddressLine2Label.Text = _ucr.Content("AddressLine2Text", _languageCode, True)
        ltlAddressLine3Label.Text = _ucr.Content("AddressLine3Text", _languageCode, True)
        ltlAddressLine4Label.Text = _ucr.Content("AddressLine4Text", _languageCode, True)
        ltlAddressPostCodeLabel.Text = _ucr.Content("AddressPostCodeText", _languageCode, True)
        ltlEmailLabel.Text = _ucr.Content("EmailText", _languageCode, True)
        btnPerformCustomerSearch.Text = _ucr.Content("PerformCustomerSearchButtonText", _languageCode, True)

        fancardBox.Attributes.Add("placeholder", ltlCustomerSearchFormHeader.Text)
        txtMembershipNumber.Attributes.Add("placeholder", lblMembershipNumber.Text)
        txtForename.Attributes.Add("placeholder", ltlForenameLabel.Text)
        txtSurname.Attributes.Add("placeholder", ltlSurnameLabel.Text)
        txtAddressLine1.Attributes.Add("placeholder", ltlAddressLine1Label.Text)
        txtAddressLine2.Attributes.Add("placeholder", ltlAddressLine2Label.Text)
        txtAddressLine3.Attributes.Add("placeholder", ltlAddressLine3Label.Text)
        txtAddressLine4.Attributes.Add("placeholder", ltlAddressLine4Label.Text)
        txtAddressPostCode.Attributes.Add("placeholder", ltlAddressPostCodeLabel.Text)
        txtEmail.Attributes.Add("placeholder", ltlEmailLabel.Text)

        If Not IsNothing(Session("LastNCustomerLogins")) Then
            Dim lastNCustomerLoginsList As List(Of String()) = Session("LastNCustomerLogins")
            lblLastNCustomersUsed.Text = String.Format(_ucr.Content("lblLastNCustomersUsed", _languageCode, True), lastNCustomerLoginsList.Count.ToString())
        End If
    End Sub

    ''' <summary>
    ''' Display Last N customers used to navigate for reuse
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisplayLastNCustomersUsed()
        Dim LastNCustomersUsed As New List(Of String())
        Dim SortedLastNCustomersUsed As New List(Of String())
        LastNCustomersUsed = CType(Session("LastNCustomerLogins"), List(Of String()))
        If Not IsNothing(Session("LastNCustomerLogins")) AndAlso LastNCustomersUsed.Count > 0 Then
            For Each item In LastNCustomersUsed
                SortedLastNCustomersUsed.Add(item.Clone())
            Next
            SortedLastNCustomersUsed.Reverse()
            rptLastNCustomerLogins.DataSource = SortedLastNCustomersUsed
            rptLastNCustomerLogins.DataBind()
        Else
            plhLastNCustomerLogins.Visible = False
        End If
    End Sub

#End Region

End Class
