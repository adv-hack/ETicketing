Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilties = Talent.eCommerce.Utilities

Partial Class PagesLogin_Profile_PromotionHistory
    Inherits TalentBase01

#Region "Public Properties"
    Public _wfrPage As New WebFormResource
    Public Property dep As DEPromotions
    Public Property ProductTypeHeaderText As String
    Public Property PriorityHeaderText As String
    Public Property ProductHeaderText As String
    Public Property DescriptionHeaderText As String
    Public Property PreRequisiteHeaderText As String
    Public Property MaxNumberOfDiscountPromotionsHeaderText As String
    Public Property MaxNumberOfDiscountProductsHeaderText As String
    Public Property CustomerAllocationHeaderText As String
#End Region

#Region "Private Properties"
    Private _languageCode As String = Nothing
#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        _languageCode = TCUtilities.GetDefaultLanguage
        Dim err As New Talent.Common.ErrorObj
        Dim talpromotions As New TalentPromotions
        Dim promotionsDataEntity As New DEPromotions
        Dim Settings As DESettings = TEBUtilties.GetSettingsObject()

        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PromotionHistory.aspx"
        End With

        SetLiterals()

        promotionsDataEntity.CustomerNumber = Profile.User.Details.LoginID
        promotionsDataEntity.Agent = Session("Agent")
        talpromotions.Dep = promotionsDataEntity
        talpromotions.Settings = Settings
        err = talpromotions.RetrievePromotionHistory
        If Not err.HasError AndAlso Not talpromotions.ResultDataSet Is Nothing AndAlso talpromotions.ResultDataSet.Tables("PromotionHistory").Rows.Count > 0 Then
            rptPromotionHistory.DataSource = talpromotions.ResultDataSet.Tables("PromotionHistory")
            rptPromotionHistory.DataBind()
        End If
        rptPromotionHistory.Visible = True

        If Not IsPostBack Then
            Session("Request.UrlReferrer") = Request.UrlReferrer
        End If

    End Sub

    Protected Sub rptPromotionHistory_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptPromotionHistory.ItemDataBound
        ' Show link to promotion details if customer has been allocated promotions  
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim myString As String
            myString = CType(e.Item.FindControl("ltlCustomerAllocation"), Literal).Text
            If myString <> String.Empty Then
                CType(e.Item.FindControl("hplPromotionDetails"), HyperLink).Text = SetText("DetailsLinkText")
            Else
                CType(e.Item.FindControl("hplPromotionDetails"), HyperLink).Text = String.Empty
            End If
        End If
    End Sub

    Protected Function SetText(sKey As String) As String
        Dim ret As String = String.Empty
        ret = _wfrPage.Content(sKey, _languageCode, True)
        Return ret
    End Function

    Protected Sub backButton_Click(sender As Object, e As System.EventArgs) Handles btnBack.Click
        Dim PreviousPage As String
        If Session("Request.UrlReferrer") IsNot Nothing Then
            PreviousPage = Session("Request.UrlReferrer").ToString
        Else
            PreviousPage = "~/PagesPublic/Home/home.aspx"
        End If
        Response.Redirect(PreviousPage)
    End Sub

#End Region

#Region "Private Methods"

    Private Sub SetLiterals()
        ' Populate repeater column headings
        ProductTypeHeaderText = SetText("ProductTypeHeaderText")
        PriorityHeaderText = SetText("PriorityHeaderText")
        ProductHeaderText = SetText("ProductHeaderText")
        DescriptionHeaderText = SetText("DescriptionHeaderText")
        ProductHeaderText = SetText("ProductHeaderText")
        PreRequisiteHeaderText = SetText("PreRequisiteHeaderText")
        MaxNumberOfDiscountPromotionsHeaderText = SetText("MaxNumberOfDiscountPromotionsHeaderText")
        MaxNumberOfDiscountProductsHeaderText = SetText("MaxNumberOfDiscountProductsHeaderText")
        CustomerAllocationHeaderText = SetText("CustomerAllocationHeaderText")
        btnBack.Text = SetText("BackButtonText")
    End Sub

#End Region

End Class
