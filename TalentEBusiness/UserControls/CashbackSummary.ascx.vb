Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_CashbackSummary
    Inherits ControlBase

    Private _languageCode As String = Nothing
    Private _ucr As UserControlResource = Nothing

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load#
        _ucr = New UserControlResource
        _languageCode = TCUtilities.GetDefaultLanguage()
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = TEBUtilities.GetCurrentPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "CashbackSummary.ascx"
        End With

        Try
            If Profile.IsAnonymous OrElse Profile.Basket.CAT_MODE = GlobalConstants.CATMODE_CANCEL Then
                plhCashBackSummary.Visible = False
            Else
                If Profile.Basket.BasketItems.Count > 0 Then
                    Dim cashbackOrCorporateInBasket As Boolean = False
                    For Each item As TalentBasketItem In Profile.Basket.BasketItems
                        If item.Product = ModuleDefaults.CashBackFeeCode Then
                            cashbackOrCorporateInBasket = True
                            Exit For
                        ElseIf item.PRODUCT_TYPE = GlobalConstants.CORPORATEPRODUCTTYPE Then
                            cashbackOrCorporateInBasket = True
                            Exit For
                        End If
                    Next
                    If cashbackOrCorporateInBasket Then
                        plhCashBackSummary.Visible = False
                    Else
                        plhCashBackSummary.Visible = CBool(_ucr.Attribute("ShowCashbackSummary", True))
                        If plhCashBackSummary.Visible Then
                            Dim cashbackAvailable As String = getCashbackAvailable()
                            If String.IsNullOrEmpty(cashbackAvailable) Then
                                plhCashBackSummary.Visible = False
                            Else
                                ltlCashbackAvailable.Text = _ucr.Content("CashBackAvailableLabel", _languageCode, True).Replace("<<AMOUNT>>", cashbackAvailable)
                                AssignCashbackViewLinkText()
                                ltlInformationText.Text = _ucr.Content("InformationText", _languageCode, True)
                            End If
                        End If
                    End If
                Else
                    plhCashBackSummary.Visible = False
                End If
            End If
        Catch ex As Exception
            plhCashBackSummary.Visible = False
        End Try

        Me.Visible = plhCashBackSummary.Visible
    End Sub

    Private Function getCashbackAvailable() As String
        Dim onAccountAvailable As String = String.Empty
        Dim payment As New TalentPayment
        Dim deSettings As DESettings = TEBUtilities.GetSettingsObject()
        Dim err As New ErrorObj

        deSettings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
        deSettings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
        payment.De.CashbackMode = "E"
        payment.De.Source = "W"
        payment.De.SessionId = Profile.Basket.Basket_Header_ID
        payment.De.CustomerNumber = Profile.User.Details.LoginID
        If Profile.Basket.BasketContentType <> GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
            Dim merchandiseAmount As Decimal = 0
            merchandiseAmount = Profile.Basket.BasketSummary.MerchandiseTotal
            payment.De.Amount = Format(merchandiseAmount, "0.00")
        End If

        payment.Settings() = deSettings
        err = payment.RetrieveCashback

      
        '--------------------------------------------------------------------------------------------------
        ' below code is same as RetrieveCashback() in applyCashback.aspx so if change here change there too. 
        '--------------------------------------------------------------------------------------------------
        ' Backend returns array of what each member may spend in this basket.  
        ' 1st element is payment owner can pay for all products so this gives max we can use     
        Dim maxOnAccountAvailableForBasket As Decimal = 0
        Dim feeTotal As Decimal = Profile.Basket.BasketSummary.TotalTicketingFees
        If Not err.HasError Then
            If payment.ResultDataSet.Tables.Count > 1 Then
                If payment.ResultDataSet.Tables(0).Rows.Count = 0 Then
                    Dim paymentOwnerRow As DataRow
                    paymentOwnerRow = payment.ResultDataSet.Tables(1).Rows(0)
                    If paymentOwnerRow("AvailableReward") > 0 Then
                        maxOnAccountAvailableForBasket = paymentOwnerRow("AvailableReward") + feeTotal
                    End If
                    If maxOnAccountAvailableForBasket > paymentOwnerRow("TotalReward") Then
                        maxOnAccountAvailableForBasket = paymentOwnerRow("TotalReward")
                    End If
                End If
            End If
        End If
        'End If

        If maxOnAccountAvailableForBasket > 0 Then
            onAccountAvailable = TDataObjects.PaymentSettings.FormatCurrency(maxOnAccountAvailableForBasket, _ucr.BusinessUnit, _ucr.PartnerCode)
        End If


        Return onAccountAvailable
    End Function

    Private Sub AssignCashbackViewLinkText()
        If Profile.Basket.BasketItems.Count > 0 Then
            Dim CashbackFeeItem As TalentBasketItem = Profile.Basket.BasketItems.Find(Function(basketDetailItem As TalentBasketItem) basketDetailItem.Product = ModuleDefaults.CashBackFeeCode)
            If CashbackFeeItem IsNot Nothing Then
                Dim cashBackAppliedString As String = _ucr.Content("CashBackAppliedLabel", _languageCode, True)
                Try
                    cashBackAppliedString = cashBackAppliedString.Replace("<<AMOUNT>>", TDataObjects.PaymentSettings.FormatCurrency(CashbackFeeItem.Gross_Price, _ucr.BusinessUnit, _ucr.PartnerCode))
                Catch ex As Exception
                    cashBackAppliedString = cashBackAppliedString.Replace("<<AMOUNT>>", TDataObjects.PaymentSettings.FormatCurrency(0, _ucr.BusinessUnit, _ucr.PartnerCode))
                End Try
                ltlCashbackApplied.Text = cashBackAppliedString
            Else
                ltlCashbackApplied.Visible = False
            End If
        End If
    End Sub

End Class