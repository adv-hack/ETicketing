Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Data


Partial Class PagesLogin_Profile_DirectDebitDetails
    Inherits TalentBase01

#Region "Public Properties"

    ' Headings for data repeaters
    Public Property DateHeaderText As String
    Public Property RefHeaderText As String
    Public Property ProductHeaderText As String
    Public Property DescriptionHeaderText As String
    Public Property ValueHeaderText As String
    Public Property PaidStatusHeaderText As String
    Public Property PayrefHeaderText As String
    Public Property BatchHeaderText As String
    Public Property DateCreatedHeaderText As String
    Public Property BalanceHeaderText As String
    Public Property UserHeaderText As String
    Public Property StatusHeaderText As String
    Public Property DEDirectDebitDetails As New DEDirectDebitDetails
#End Region

#Region "Private Properties"
    Private _languageCode As String = Nothing
    Private _errMsg As TalentErrorMessages
    Private _wfrPage As New WebFormResource
#End Region

#Region "Protected Method"

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "DirectDebitDetails.aspx"
        End With
        _languageCode = TCUtilities.GetDefaultLanguage
        _errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), _wfrPage.FrontEndConnectionString)
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim err As New Talent.Common.ErrorObj
        Dim tdd As New TalentDirectDebitDetails
        Dim Settings As New DESettings
        With Settings
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .BusinessUnit = TalentCache.GetBusinessUnit
            .Cacheing = False
            .DestinationDatabase = Talent.eCommerce.Utilities.GetCustomerDestinationDatabase()
            .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("TALENTCRM").ToString
            .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        End With

        tdd.Settings = Settings
        Dim ta As New Talent.eCommerce.Agent
        tdd.Company = ta.GetAgentCompany
        tdd.Customer = Profile.UserName
        tdd.deDirectDebitDetails.showPaidOnAcountRecords = cbShowPaid.Checked

        SetLiterals()

        If Not IsPostBack Then
            ' Get the customers DD details  
            err = tdd.RetrieveTalentDirectDebitDetails
            If Not err.HasError Then
                uscDirectDebit.AccountNameBox.Text = tdd.deDirectDebitDetails.AccountName
                uscDirectDebit.AccountNumberBox.Text = tdd.deDirectDebitDetails.AccountNumber
                uscDirectDebit.SortCode1Box.Text = tdd.deDirectDebitDetails.SortCode1
                uscDirectDebit.SortCode2Box.Text = tdd.deDirectDebitDetails.SortCode2
                uscDirectDebit.SortCode3Box.Text = tdd.deDirectDebitDetails.SortCode3
                cbDirectDebitTreasurer.Checked = tdd.deDirectDebitDetails.DDTreasurer
                plhNoDirectDebitBalances.Visible = False
            End If
        End If

        ' Now get their direct debit on-account details
        err = tdd.RetrieveTalentDirectDebitOnAccount
        If Not err.HasError AndAlso Not tdd.ResultDataSet Is Nothing AndAlso tdd.ResultDataSet.Tables("DirectDebitOnAccount").Rows.Count > 0 Then
            rptDirectDebitOnAccount.DataSource = tdd.ResultDataSet.Tables("DirectDebitOnAccount")
            rptDirectDebitOnAccount.DataBind()
            rptDirectDebitOnAccount.Visible = True
            plhNoDirectDebitOnAccount.Visible = False
        Else
            plhNoDirectDebitOnAccount.Visible = True
            noDDOnAccountText.Text = _wfrPage.Content("noDDOnAccountText", _languageCode, True)
        End If

        ' and finally get their DD balances
        err = tdd.RetrieveTalentDirectDebitBalances
        If Not err.HasError AndAlso Not tdd.ResultDataSet Is Nothing AndAlso tdd.ResultDataSet.Tables("DirectDebitBalances").Rows.Count > 0 Then
            rptDirectDebitBalances.DataSource = tdd.ResultDataSet.Tables("DirectDebitBalances")
            rptDirectDebitBalances.DataBind()
            rptDirectDebitBalances.Visible = True
        Else
            plhNoDirectDebitBalances.Visible = True
            noDDBalancesText.Text = _wfrPage.Content("noDDBalancesText", _languageCode, True)
        End If

        If Not IsPostBack Then
            Session("Request.UrlReferrer") = Request.UrlReferrer
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhDirectDebitOnAccountErrorMessages.Visible = (DirectDebitOnAccountErrorList.Items.Count > 0)
        Page.MaintainScrollPositionOnPostBack = True
        If Not IsPostBack Then

            ' Currently called from customer details screen but allow for future call from other pages  
            Dim PreviousPage As String
            If Request.UrlReferrer IsNot Nothing Then
                PreviousPage = Request.UrlReferrer.ToString
            Else
                PreviousPage = "~/PagesPublic/Home/home.aspx"
            End If
            Session("PreviousPage") = PreviousPage
        End If
    End Sub

    Protected Sub rptDirectDebitOnAccount_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDirectDebitOnAccount.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim btnChangeStatus As Button = CType(e.Item.FindControl("btnChangeStatus"), Button)
            Dim btnDelete As Button = CType(e.Item.FindControl("btnDelete"), Button)
            btnChangeStatus.Text = "Change"
            btnDelete.Text = "Delete"
            Dim myString As String
            myString = CType(e.Item.FindControl("lblDate"), Label).Text
            CType(e.Item.FindControl("lblDate"), Label).Text = TEBUtilities.GetFormattedDateAndTime(myString, String.Empty, String.Empty, ModuleDefaults.GlobalDateFormat, ModuleDefaults.Culture)

            myString = CType(e.Item.FindControl("lblPayref"), Label).Text
            If CDec(myString) > 0 Then
                CType(e.Item.FindControl("lblPaid"), Label).Text = "Y"
            Else
                CType(e.Item.FindControl("lblPaid"), Label).Text = "N"
            End If

            myString = CType(e.Item.FindControl("lblActive"), Label).Text
            If myString = "A" Then
                CType(e.Item.FindControl("lblActive"), Label).Text = SetText("DirectDebitBalanceActive")
            Else
                CType(e.Item.FindControl("lblActive"), Label).Text = SetText("DirectDebitBalanceDeactive")
            End If
        End If
    End Sub

    Protected Sub rptDirectDebitBalances_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDirectDebitBalances.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim myString As String
            myString = CType(e.Item.FindControl("lblDateCreated"), Label).Text
            CType(e.Item.FindControl("lblDateCreated"), Label).Text = TEBUtilities.GetFormattedDateAndTime(myString, String.Empty, String.Empty, ModuleDefaults.GlobalDateFormat, ModuleDefaults.Culture)
        End If
    End Sub

    ''' <summary>
    ''' Update DD Details(Sort code, account name and number) calls WS205R backend  
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnUpdateDirectDebitDetails_Click(sender As Object, e As System.EventArgs) Handles btnUpdateDirectDebitDetails.Click
        Dim err As New Talent.Common.ErrorObj
        Dim tdd As New TalentDirectDebitDetails
        Dim Settings As New DESettings
        With Settings
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .BusinessUnit = TalentCache.GetBusinessUnit
            .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            .Cacheing = False
        End With
        tdd.Settings = Settings
        With tdd.deDirectDebitDetails
            .AccountName = uscDirectDebit.AccountNameBox.Text.Trim()
            .AccountNumber = uscDirectDebit.AccountNumberBox.Text.Trim()
            .DDTreasurer = cbDirectDebitTreasurer.Checked
            .SortCode1 = uscDirectDebit.SortCode1Box.Text.Trim()
            .SortCode2 = uscDirectDebit.SortCode2Box.Text.Trim()
            .SortCode3 = uscDirectDebit.SortCode3Box.Text.Trim()
            .CustomerNumber = Profile.UserName
            .Agent = Session("Agent")
            .OnAccountRef = 0
        End With
        If Not err.HasError Then
            err = tdd.UpdateTalentDirectDebitDetails
            Response.Redirect(Session("PreviousPage"))
        End If
    End Sub

    ''' <summary>
    ''' Process option to change direct debit on-account status - calls WS205R backend
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub rptDirectDebitOnAccount_ItemCommand1(source As Object, e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptDirectDebitOnAccount.ItemCommand
        If e.CommandName = "ChangeOnAccountStatusButton" Or e.CommandName = "DeleteOnAccountButton" Then

            Dim OnAccountRef As String = CType(e.Item.FindControl("btnChangeStatus"), Button).CommandArgument
            Dim tdd As New TalentDirectDebitDetails
            Dim de As New DEDirectDebitDetails
            Dim ta As New Talent.eCommerce.Agent

            Dim Settings As New DESettings
            With Settings
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .BusinessUnit = TalentCache.GetBusinessUnit
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                .Cacheing = False
            End With
            tdd.Settings = Settings
            With tdd.deDirectDebitDetails
                .Company = ta.GetAgentCompany
                .CustomerNumber = Profile.UserName
                .OnAccountRef = OnAccountRef
                .Agent = Session("Agent")
            End With

            If e.CommandName = "ChangeOnAccountStatusButton" Then tdd.ChangeTalentDirectDebitOnAccountStatus()
            If e.CommandName = "DeleteOnAccountButton" Then tdd.DeleteTalentDirectDebitOnAccount()

            Dim Returncode As String

            'Show errors or reload page 
            If tdd.ResultDataSet IsNot Nothing AndAlso tdd.ResultDataSet.Tables("Status") IsNot Nothing AndAlso tdd.ResultDataSet.Tables("Status").Rows.Count > 0 Then
                Returncode = tdd.ResultDataSet.Tables("Status").Rows(0)("returncode")
                Dim talentErrorMsg As TalentErrorMessage = _errMsg.GetErrorMessage(Returncode)
                Dim listItemObject As ListItem = DirectDebitOnAccountErrorList.Items.FindByText(talentErrorMsg.ERROR_MESSAGE)
                If listItemObject Is Nothing Then DirectDebitOnAccountErrorList.Items.Add(talentErrorMsg.ERROR_MESSAGE)
            Else
                Server.Transfer(Request.Path)
            End If
        End If
    End Sub

    Protected Sub backButton_Click(sender As Object, e As System.EventArgs) Handles btnBack.Click
        Dim PreviousPage As String
        If Session("Request.UrlReferrer") IsNot Nothing Then
            PreviousPage = Session("Request.UrlReferrer").ToString
        Else
            PreviousPage = "~/PagesPublic/Home/home.aspx"
        End If
        Response.Redirect(PreviousPage)
    End Sub

    Protected Function SetText(sKey As String) As String
        Dim ret As String = String.Empty
        ret = _wfrPage.Content(sKey, _languageCode, True)
        Return ret
    End Function

#End Region

#Region "Private Methods"

    Private Sub SetLiterals()
        DirectDebitTreasurerlabel.Text = SetText("DirectDebitTreasurerlabel")
        btnUpdateDirectDebitDetails.Text = SetText("btnUpdateDirectDebitDetails")
        DateHeaderText = SetText("DateHeaderText")
        RefHeaderText = SetText("RefHeaderText")
        ProductHeaderText = SetText("ProductHeaderText")
        DescriptionHeaderText = SetText("DescriptionHeaderText")
        ValueHeaderText = SetText("ValueHeaderText")
        PaidStatusHeaderText = SetText("PaidStatusHeaderText")
        PayrefHeaderText = SetText("PayrefHeaderText")
        StatusHeaderText = SetText("StatusHeaderText")
        BatchHeaderText = SetText("BatchHeaderText")
        DateCreatedHeaderText = SetText("DateCreatedHeaderText")
        BalanceHeaderText = SetText("BalanceHeaderText")
        UserHeaderText = SetText("UserHeaderText")
        ltlDirectDebitDetailsTitle.Text = SetText("ltlDirectDebitDetailsTitle")
        ltlDirectDebitBalancesTitle.Text = SetText("ltlDirectDebitBalancesTitle")
        ltlDirectDebitOnAccountTitle.Text = SetText("ltlDirectDebitOnAccountTitle")
        cbShowPaid.Text = SetText("showPaidCheckbox")
        btnBack.Text = SetText("BackButtonText")
    End Sub

#End Region

End Class