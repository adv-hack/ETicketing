Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Data
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Partial Class PagesAgent_Profile_CustomerText
    Inherits TalentBase01
    Private wfr As Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Sub Page_Load1(sender As Object, e As EventArgs) Handles Me.Load
        wfr = New Talent.Common.WebFormResource
        SetUpWFR()

        ltlCustomerTextBoxHeading.Text = wfr.Content("ltlCustomerTextBoxHeading", _languageCode, True)
        updateBtn.Text = wfr.Content("updateBtn", _languageCode, True)

        ' Only show in Agent mode
        If Talent.eCommerce.Utilities.IsAgent AndAlso Not Profile.User.Details Is Nothing Then
            plhCustomerText.Visible = True
            If Not Page.IsPostBack Then
                PopulateCustomerTextBox()
            End If

        Else
            plhCustomerText.Visible = False
        End If

    End Sub
    Protected Sub PopulateCustomerTextBox()

        If Talent.eCommerce.Utilities.IsAgent Then

            Dim err As New Talent.Common.ErrorObj
            Dim ct As New TalentCustomerText
            Dim Settings As New DESettings

            With Settings
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .BusinessUnit = TalentCache.GetBusinessUnit
                .Company = wfr.Content("crmExternalKeyName", _languageCode, True)
                .Cacheing = False
                .DestinationDatabase = Talent.eCommerce.Utilities.GetCustomerDestinationDatabase()
                '  .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("TALENTCRM").ToString
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            End With

            ct.Settings = Settings
            ct.deCustomerText.Customer = Profile.User.Details.LoginID.Trim
            ct.deCustomerText.Agent = AgentProfile.Name
            err = ct.RetrieveTalentCustomerText

            ' Was the call successful
            If Not err.HasError AndAlso _
                 ct.ResultDataSet.Tables.Count() > 0 Then
                For Each row As DataRow In ct.ResultDataSet.Tables("CustomerText").Rows
                    txtCustomerText.Text += row.Item("text")
                Next
                Session("customerText") = Trim(txtCustomerText.Text)
            End If
        End If

    End Sub
    Protected Sub SetUpWFR()
        With wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "CustomerText.aspx"
        End With
    End Sub

    Protected Sub UpdateCustomerText()

        If txtCustomerText.Visible = True Then
            Dim ct As New TalentCustomerText
            Dim Settings As New DESettings
            With Settings
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .BusinessUnit = TalentCache.GetBusinessUnit

                .Cacheing = False
                .DestinationDatabase = Talent.eCommerce.Utilities.GetCustomerDestinationDatabase()
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            End With

            ct.Settings = Settings
            Dim Customertext As String = Talent.Common.Utilities.ConvertASCIIHexValue(Trim(txtCustomerText.Text), wfr.BusinessUnit, wfr.PartnerCode, _languageCode, Settings)
            If Customertext <> Session("CustomerText") Then
                With ct.deCustomerText
                    .Customer = Profile.User.Details.LoginID.Trim
                    .CustomerText = Customertext
                    .Agent = AgentProfile.Name
                End With
                ct.UpdateTalentCustomerText()
            End If
        End If
    End Sub

    Protected Sub updateBtn_Click(sender As Object, e As EventArgs) Handles updateBtn.Click
        UpdateCustomerText()
    End Sub
End Class
