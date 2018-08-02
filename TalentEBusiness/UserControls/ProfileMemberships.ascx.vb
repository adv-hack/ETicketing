Imports Talent.Common
Imports System.Globalization
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Data

Partial Class UserControls_ProfileMemberships
    Inherits ControlBase

    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected ColumnHeaderText_Membership As String
    Protected ColumnHeaderText_Number As String
    Protected ColumnHeaderText_ExpiryDate As String
    Protected ColumnHeaderText_PurchasedDate As String
    Protected ColumnHeaderText_Loyalty As String
    Protected ColumnHeaderText_MembershipSinceDate As String
    Protected ColumnHeaderText_PriceCode As String
    Protected ColumnHeaderText_PriceCodeDescription As String
    Protected ColumnHeaderText_PriceBand As String
    Protected ColumnHeaderText_PriceBandDescription As String

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = TEBUtilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ProfileMemberships.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Profile.User Is Nothing OrElse Profile.User.Details Is Nothing Then
            If AgentProfile.IsAgent Then
                Response.Redirect("~/PagesPublic/Profile/CustomerSelection.aspx")
            Else
                Response.Redirect("~/PagesPublic/Login/Login.aspx")
            End If
        Else
            InitialiseClassLevelFields()
            LoadMembsershipDetail()
        End If
    End Sub

    Protected Sub InitialiseClassLevelFields()
        ColumnHeaderText_Membership = _ucr.Content("HeaderText_Membership", _languageCode, True)
        ColumnHeaderText_Number = _ucr.Content("HeaderText_Number", _languageCode, True)
        ColumnHeaderText_ExpiryDate = _ucr.Content("HeaderText_ExpiryDate", _languageCode, True)
        ColumnHeaderText_PurchasedDate = _ucr.Content("HeaderText_PurchasedDate", _languageCode, True)
        ColumnHeaderText_Loyalty = _ucr.Content("HeaderText_Loyalty", _languageCode, True)
        ColumnHeaderText_MembershipSinceDate = _ucr.Content("HeaderText_MembershipSinceDate", _languageCode, True)
        ColumnHeaderText_PriceCode = _ucr.Content("HeaderText_PriceCode", _languageCode, True)
        ColumnHeaderText_PriceCodeDescription = _ucr.Content("HeaderText_PriceCodeDescription", _languageCode, True)
        ColumnHeaderText_PriceBand = _ucr.Content("HeaderText_PriceBand", _languageCode, True)
        ColumnHeaderText_PriceBandDescription = _ucr.Content("HeaderText_PriceBandDescription", _languageCode, True)
    End Sub

    Protected Sub LoadMembsershipDetail()
        Dim err As ErrorObj
        Dim resultSet As DataSet
        Dim profile As TalentProfile = HttpContext.Current.Profile
        Dim customerNumber As String = String.Empty
        Dim deCust As New DECustomer()
        Dim deCustV11 As New DECustomerV11()
        Dim tc As New TalentCustomer()

        If profile.User IsNot Nothing AndAlso profile.User.Details IsNot Nothing Then
            customerNumber = profile.User.Details.LoginID
        End If
        deCust.CustomerNumber = customerNumber
        deCust.MembershipsProductSubType = TEBUtilities.CheckForDBNull_String(_ucr.Attribute("MembershipsProductSubTypes"))
        deCustV11.DECustomersV1.Add(deCust)

        With tc
            .DeV11 = deCustV11
            .Settings = TEBUtilities.GetSettingsObject()
            .Settings.Cacheing = False
            err = .CustomerMembershipsRetrieval()
            resultSet = .ResultDataSet
            If err.HasError OrElse resultSet Is Nothing OrElse resultSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                plhMembershipRepeater.Visible = False
                plhErrorList.Visible = True
                blErrorMessages.Items.Add(_ucr.Content("ErrorText_Membership_Not_Found", _languageCode, True))
            Else
                If resultSet.Tables.Contains("CustomerMemberships") Then
                    rptMembership.DataSource = resultSet.Tables("CustomerMemberships")
                    rptMembership.DataBind()
                Else
                    plhMembershipRepeater.Visible = False
                    plhErrorList.Visible = True
                    blErrorMessages.Items.Add(_ucr.Content("ErrorText_Membership_Not_Found", _languageCode, True))
                End If
            End If
        End With
    End Sub

    Protected Sub CanDisplayThisColumn(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Select Case sender.id
                Case "plhNumber"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("Show_MembershipNumber"))
                Case "plhExpiryDate"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("Show_ExpiryDate"))
                Case "plhPurchasedDate"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("Show_PurchasedDate"))
                Case "plhLoyalty"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("Show_Loyalty"))
                Case "plhMembershipSinceDate"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("Show_MembershipSinceDate"))
                Case "plhPriceCode"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("Show_PriceCode"))
                Case "plhPriceCodeDescription"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("Show_PriceCodeDescription"))
                Case "plhPriceBand"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("Show_PriceBand"))
                Case "plhPriceBandDescription"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("Show_PriceBandDescription"))
            End Select
        Catch ex As Exception
            sender.Visible = False
        End Try
    End Sub

    Protected Function GetFormattedDate(ByVal strDate As String) As String
        Return TEBUtilities.GetFormattedDateAndTime(strDate, String.Empty, " ", ModuleDefaults.GlobalDateFormat, ModuleDefaults.Culture)
    End Function

End Class
