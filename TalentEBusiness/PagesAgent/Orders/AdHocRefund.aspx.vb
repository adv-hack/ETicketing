Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesAgent_Orders_AdHocRefund
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfrPage As WebFormResource = Nothing
    Private _languageCode As String = Nothing
    Private maxRefundAllowed As String = Nothing
   

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        If AgentProfile.AgentPermissions.CanGiveAdHocRefunds = False Then
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        Else
            _wfrPage = New WebFormResource
            _languageCode = TCUtilities.GetDefaultLanguage
            With _wfrPage
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = TEBUtilities.GetCurrentPageName()
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = .PageCode
            End With
            ddlAdHocRefundProduct.DataSource = retrieveAdHocRefundProducts()
            ddlAdHocRefundProduct.DataTextField = "Desription"
            ddlAdHocRefundProduct.DataValueField = "ProductCode"
            ddlAdHocRefundProduct.DataBind()
            If ddlAdHocRefundProduct.Items.Count > 0 Then
                setPageDefaults()
                pnlAdHocRefund.Visible = True
            Else
                pnlAdHocRefund.Visible = False
                blErrorMessage.Items.Clear()
                blErrorMessage.Items.Add(_wfrPage.Content("ProblemWithSetupError", _languageCode, True))
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        blErrorMessage.Visible = (blErrorMessage.Items.Count > 0)
    End Sub

    Protected Sub btnAdHocRefund_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdHocRefund.Click
        Dim amountValid As Boolean = True
        Page.Validate()
        If Page.IsValid Then
            Dim AdHocRefundProduct As String = String.Empty
            Dim AdHocRefundAmount As String = String.Empty
            AdHocRefundAmount = txtAdHocRefundAmount.Text.Trim()
            AdHocRefundProduct = Trim(ddlAdHocRefundProduct.SelectedValue)

            If Trim(AdHocRefundAmount) = String.Empty OrElse AdHocRefundAmount < 0 Then
                blErrorMessage.Items.Add(_wfrPage.Content("AmountRequired", _languageCode, True))
                amountValid = False
            End If
            If Utilities.CheckForDBNull_Decimal(AdHocRefundAmount) > Utilities.CheckForDBNull_Decimal(maxRefundAllowed) AndAlso Utilities.CheckForDBNull_Decimal(maxRefundAllowed) > 0 Then
                Dim message As String = _wfrPage.Content("MaxRefundAllowed", _languageCode, True) & " " & Trim(maxRefundAllowed.ToString)
                blErrorMessage.Items.Add(message)
                amountValid = False
            End If

            If amountValid Then
                Dim redirectUrl As New StringBuilder
                redirectUrl.Append("~/Redirect/TicketingGateway.aspx?page=productmembership.aspx&function=addvariablepricedtobasket&product=")
                redirectUrl.Append(AdHocRefundProduct)
                redirectUrl.Append("&quantity=1&price=")
                redirectUrl.Append(AdHocRefundAmount)
                Response.Redirect(redirectUrl.ToString())
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"
    Private Sub setPageDefaults()
        ltlAdHocRefundHeader.Text = _wfrPage.Content("AdHocRefundHeader", _languageCode, True)
        lblAdHocRefundAmountText.Text = _wfrPage.Content("AdHocRefundAmountLabel", _languageCode, True)
        btnAdHocRefund.Text = _wfrPage.Content("AdHocRefundButtonText", _languageCode, True)
        lblAdHocRefundProductDropDownList.Text = _wfrPage.Content("AdHocRefundProductText", _languageCode, True)
        maxRefundAllowed = TEBUtilities.CheckForDBNull_Decimal(_wfrPage.Attribute("MaxRefundAllowed"))
        rgxadHocRefund.ErrorMessage = _wfrPage.Content("InvalidAmountError", _languageCode, True)
        rgxadHocRefund.ValidationExpression = TEBUtilities.CheckForDBNull_String(_wfrPage.Attribute("InvalidAmountValidationExpression"))
    End Sub
#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Call TALENT to retrieve the adHoc refund products from file MD042TBL.
    ''' </summary>
    ''' <returns>table of AdHoc Refund products</returns>
    ''' <remarks></remarks>
    Private Function retrieveAdHocRefundProducts() As Data.DataTable
        Dim AdHocRefundProducts As Data.DataTable = Nothing
        Dim err As New ErrorObj
        Dim tp As New TalentPayment
        With tp.De
            .Source = GlobalConstants.SOURCE
            .variablePricedProductType = "ADHOCREFUND"
        End With
        tp.Settings = TEBUtilities.GetSettingsObject()
        err = tp.RetrieveVariablePricedProducts()

        blErrorMessage.Items.Clear()
        If Not err.HasError AndAlso tp.ResultDataSet IsNot Nothing AndAlso tp.ResultDataSet.Tables.Contains("VariablePricedProducts") Then
            If tp.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                Dim errorCode As String = tp.ResultDataSet.Tables("StatusResults").Rows(0)("ReturnCode")
                Dim errMsg As New TalentErrorMessages(_languageCode, _wfrPage.BusinessUnit, _wfrPage.PartnerCode, _wfrPage.FrontEndConnectionString)
                blErrorMessage.Items.Add(errMsg.GetErrorMessage(String.Empty, _wfrPage.PageCode, errorCode).ERROR_MESSAGE)
            Else
                AdHocRefundProducts = tp.ResultDataSet.Tables("VariablePricedProducts")
            End If
        Else
            blErrorMessage.Items.Add(_wfrPage.Content("ErrorRetrievingCard", _languageCode, True))
        End If

        Return AdHocRefundProducts
    End Function

#End Region

End Class