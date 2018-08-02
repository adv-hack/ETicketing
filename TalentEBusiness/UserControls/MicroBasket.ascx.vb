Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_MicroBasket
    Inherits ControlBase
    Private _ucr As UserControlResource = Nothing
    Private _languageCode As String = Nothing

    Private Const RESTRICTEDPAGENAME As String = "CHECKOUTORDERCONFIRMATION.ASPX"

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _ucr = New UserControlResource
        _languageCode = TEBUtilities.GetCurrentLanguage()
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ConnectionString
            .KeyCode = "MicroBasket.ascx"
            .PageCode = TEBUtilities.GetCurrentPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        End With
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Dim basketText As String = _ucr.Content("BasketLinkText", _languageCode, True)
        If Not _ucr.PageCode.ToUpper.Equals(RESTRICTEDPAGENAME) Then
            Dim totalItems As Decimal = Profile.Basket.BasketSummary.MerchandiseTotalItems + Profile.Basket.BasketSummary.TotalItemsTicketing
            basketText = basketText.Replace("<<BASKET_ITEMS>>", totalItems.ToString())
            If Profile.PartnerInfo.Details IsNot Nothing Then
                If Profile.PartnerInfo.Details.HIDE_PRICES Then
                    basketText = basketText.Replace("<<BASKET_TOTAL>>", "")
                Else
                    basketText = basketText.Replace("<<BASKET_TOTAL>>", TDataObjects.PaymentSettings.FormatCurrency(Profile.Basket.BasketSummary.TotalBasket, _ucr.BusinessUnit, _ucr.PartnerCode))
                End If
            Else
                basketText = basketText.Replace("<<BASKET_TOTAL>>", TDataObjects.PaymentSettings.FormatCurrency(Profile.Basket.BasketSummary.TotalBasket, _ucr.BusinessUnit, _ucr.PartnerCode))
            End If
            hplBasket.Text = basketText
        Else
            basketText = basketText.Replace("<<BASKET_ITEMS>>", "0")
            If Profile.PartnerInfo.Details IsNot Nothing Then
                If Profile.PartnerInfo.Details.HIDE_PRICES Then
                    basketText = basketText.Replace("<<BASKET_TOTAL>>", "")
                Else
                    basketText = basketText.Replace("<<BASKET_TOTAL>>", TDataObjects.PaymentSettings.FormatCurrency(Profile.Basket.BasketSummary.TotalBasket, _ucr.BusinessUnit, _ucr.PartnerCode))
                End If
            Else
                basketText = basketText.Replace("<<BASKET_TOTAL>>", TDataObjects.PaymentSettings.FormatCurrency(Profile.Basket.BasketSummary.TotalBasket, _ucr.BusinessUnit, _ucr.PartnerCode))
            End If
            hplBasket.Text = basketText
        End If
    End Sub

End Class
