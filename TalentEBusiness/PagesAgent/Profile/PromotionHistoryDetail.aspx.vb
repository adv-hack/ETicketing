Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesLogin_Profile_PromotionHistoryDetail
    Inherits TalentBase01

#Region "Public Properties"

    Public _wfrPage As WebFormResource
    Public Property ProductCodeText As String
    Public Property PriorityText As String
    Public Property ProductTypeHeaderText As String
    Public Property DescriptionHeaderText As String
    Public Property SeatHeaderText As String
    Public Property PriceCodeHeaderText As String
    Public Property PriceBandHeaderText As String
    Public Property SoldDateHeaderText As String

#End Region

#Region "Private Properties"

    Private _languageCode As String = Nothing

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        _wfrPage = New WebFormResource
        _languageCode = TCUtilities.GetDefaultLanguage
        Dim err As New Talent.Common.ErrorObj
        Dim talpromotions As New TalentPromotions
        Dim Settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim promotionsDataEntity As New DEPromotions

        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PromotionHistory.aspx"
        End With

        SetLiterals()

        promotionsDataEntity.CustomerNumber = Profile.User.Details.LoginID
        Dim ta As New Talent.eCommerce.Agent
        promotionsDataEntity.Company = ta.GetAgentCompany
        talpromotions.Dep = promotionsDataEntity

        talpromotions.Settings = Settings
        talpromotions.Dep.PromotionId = Request.QueryString("PromotionID")
        err = talpromotions.RetrievePromotionHistoryDetail

        If Not err.HasError AndAlso Not talpromotions.ResultDataSet Is Nothing AndAlso talpromotions.ResultDataSet.Tables("PromotionHistoryDetailHeader").Rows.Count > 0 Then
            rptPromotionHistoryDetail.DataSource = talpromotions.ResultDataSet.Tables("PromotionHistoryDetailLine")
            rptPromotionHistoryDetail.DataBind()
            ltlProductTypeValue.Text = talpromotions.ResultDataSet.Tables("PromotionHistoryDetailHeader").Rows(0)("ProductType")
            ltlPriorityValue.Text = talpromotions.ResultDataSet.Tables("PromotionHistoryDetailHeader").Rows(0)("Priority")
            ltlPriceBandValue.Text = talpromotions.ResultDataSet.Tables("PromotionHistoryDetailHeader").Rows(0)("PriceBand")
            Dim pricecodes As String = talpromotions.ResultDataSet.Tables("PromotionHistoryDetailHeader").Rows(0)("PriceCodes")
            For ip As Integer = 0 To 10 Step 2
                ltlPriceCodeValue.Text = Trim(ltlPriceCodeValue.Text) & pricecodes.Substring(ip, 2) & " "
            Next
        End If
        rptPromotionHistoryDetail.Visible = True

        If Not IsPostBack Then
            Session("Request.UrlReferrer") = Request.UrlReferrer
        End If
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As System.EventArgs) Handles btnBack.Click
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
        ' Populate repeater column headings
        ProductTypeHeaderText = SetText("ProductTypeHeaderText")
        DescriptionHeaderText = SetText("DescriptionHeaderText")
        PriceCodeHeaderText = SetText("PriceCodeHeaderText")
        PriceBandHeaderText = SetText("PriceBandHeaderText")
        SeatHeaderText = SetText("SeatHeaderText")
        SoldDateHeaderText = SetText("SoldDateHeaderText")

        ltlProductTypeLabel.Text = SetText("ProductTypeLabel")
        ltlPriorityLabel.Text = SetText("PriorityLabel")
        ltlPriceCodeLabel.Text = SetText("PriceCodeLabel")
        ltlPriceBandLabel.Text = SetText("priceBandLabel")
        btnBack.Text = SetText("BackButtonText")
    End Sub

#End Region

End Class
