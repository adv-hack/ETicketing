Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilties = Talent.eCommerce.Utilities

Partial Class PagesLogin_Profile_OnAccount
    Inherits TalentBase01

#Region "Public Properties"
    Public _wfrPage As New WebFormResource
    Public Property DateHeaderText As String
    Public Property TimeHeaderText As String
    Public Property Usertext As String
    Public Property FromCodeText As String
    Public Property ToCodeText As String
#End Region

#Region "Private Properties"
    Private _ucr As UserControlResource = Nothing
    Private _languageCode As String = Nothing
#End Region

    Protected Sub Page_Load1(sender As Object, e As System.EventArgs) Handles Me.Load

        _languageCode = TCUtilities.GetDefaultLanguage
        Dim err As New Talent.Common.ErrorObj
        Dim sc As New TalentStopcodes
        Dim Settings As DESettings = TEBUtilties.GetSettingsObject()


        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "StopCodeAudit.aspx"
        End With


        DateHeaderText = _wfrPage.Content("DateHeaderText", _languageCode, True)
        TimeHeaderText = _wfrPage.Content("TimeHeaderText", _languageCode, True)
        Usertext = _wfrPage.Content("Usertext", _languageCode, True)
        FromCodeText = _wfrPage.Content("FromCodeText", _languageCode, True)
        ToCodeText = _wfrPage.Content("ToCodeText", _languageCode, True)

        sc.Settings = Settings
        Dim ta As New Talent.eCommerce.Agent
        sc.company = ta.GetAgentCompany
        sc.Customer = Profile.UserName

        err = sc.RetrieveTalentStopCodeAudit
        If Not err.HasError AndAlso Not sc.ResultDataSet Is Nothing AndAlso sc.ResultDataSet.Tables("StopCodeAudit").Rows.Count > 0 Then
            rptStopCodeAudit.DataSource = sc.ResultDataSet
            rptStopCodeAudit.DataBind()
            rptStopCodeAudit.Visible = True
            plhNoStopCodeAudit.Visible = False
        Else
            plhNoStopCodeAudit.Visible = True
            ltlNoStopCodeAudit.Text = _wfrPage.Content("NoStopCodeAuditText", _languageCode, True)
        End If
    End Sub

    Protected Sub rptStopCodeAudit_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptStopCodeAudit.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim myString As String
            myString = CType(e.Item.FindControl("lblDate"), Label).Text
            CType(e.Item.FindControl("lblDate"), Label).Text = TCUtilities.ISeriesDate(myString).ToString("dd/MM/yy")
            myString = CType(e.Item.FindControl("lblTime"), Label).Text
            CType(e.Item.FindControl("lblTime"), Label).Text = myString.Substring(0, 2) & ":" & myString.Substring(2, 2)
        End If
    End Sub

    Protected Function SetText(sKey As String) As String
        Dim ret As String = String.Empty
        ret = _ucr.Content(sKey, _languageCode, True)
        Return ret
    End Function

End Class
