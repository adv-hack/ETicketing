Imports Talent.eCommerce

Partial Class UserControls_CreditFinanceSummary
    Inherits ControlBase

    Private _ucr As Talent.Common.UserControlResource = Nothing
    Private _languageCode As String = String.Empty

    Public Property PaymentRef() As String = ""
    Public Property PageCode() As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack And Not PaymentRef.Trim.Equals("") Then
            pnlCreditFinanceSummary.Visible = True

            'Set up the user control
            _ucr = New Talent.Common.UserControlResource
            _languageCode = Talent.Common.Utilities.GetDefaultLanguage
            With _ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "CreditFinanceSummary.ascx"
            End With

            'Populate the user control
            SetLabelText()
            SetDetailText()
            AddressYearsLabel.Visible = False
            AddressYearsDetailsLabel.Visible = False
            Checkout.RemovePaymentDetailsFromSession()
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (ErrorList.Items.Count > 0)
    End Sub

    Protected Sub SetLabelText()
        With _ucr
            TitleLabel.Text = .Content("TitleLabel", _languageCode, True)
            PaymentReferenceLabel.Text = .Content("PaymentReferenceLabel", _languageCode, True)
            AccountNameLabel.Text = .Content("AccountNameLabel", _languageCode, True)
            SortCodeLabel.Text = .Content("SortCodeLabel", _languageCode, True)
            AccountNumberLabel.Text = .Content("AccountNumberLabel", _languageCode, True)
            Address1Label.Text = .Content("Address1Label", _languageCode, True)
            Address2Label.Text = .Content("Address2Label", _languageCode, True)
            Address3Label.Text = .Content("Address3Label", _languageCode, True)
            Address4Label.Text = .Content("Address4Label", _languageCode, True)
            Address5Label.Text = .Content("Address5Label", _languageCode, True)
            PostCodeLabel.Text = .Content("PostCodeLabel", _languageCode, True)
            AddressYearsLabel.Text = .Content("NoOfYearsAtAddressLabel", _languageCode, True)
            InstallmentPlanLabel.Text = .Content("InstallmentPlanLabel", _languageCode, True)
            InstallmentExampleLabel.Text = .Content("InstallmentExampleLabel", _languageCode, True)
        End With
    End Sub

    Protected Sub SetDetailText()
        PaymentReferenceDetailLabel.Text = PaymentRef
        AccountNameDetailLabel.Text = Checkout.RetrievePaymentItemFromSession("AccountName", GlobalConstants.CHECKOUTASPXSTAGE)
        SortCodeDetailLabel.Text = Checkout.RetrievePaymentItemFromSession("SortCode", GlobalConstants.CHECKOUTASPXSTAGE)
        AccountNumberDetailLabel.Text = Checkout.RetrievePaymentItemFromSession("AccountNumber", GlobalConstants.CHECKOUTASPXSTAGE)
        Address1DetailLabel.Text = Checkout.RetrievePaymentItemFromSession("AddressLine1", GlobalConstants.CHECKOUTASPXSTAGE)
        Address2DetailLabel.Text = Checkout.RetrievePaymentItemFromSession("AddressLine2", GlobalConstants.CHECKOUTASPXSTAGE)
        Address3DetailLabel.Text = Checkout.RetrievePaymentItemFromSession("AddressLine3", GlobalConstants.CHECKOUTASPXSTAGE)
        Address4DetailLabel.Text = Checkout.RetrievePaymentItemFromSession("AddressLine4", GlobalConstants.CHECKOUTASPXSTAGE)
        Address5DetailLabel.Text = Checkout.RetrievePaymentItemFromSession("AddressLine5", GlobalConstants.CHECKOUTASPXSTAGE)
        PostCodeDetailLabel.Text = Checkout.RetrievePaymentItemFromSession("PostCode", GlobalConstants.CHECKOUTASPXSTAGE)
        'set the visibility for address fields
        Address1Label.Visible = Address1DetailLabel.Text.Trim.Length > 0
        Address2Label.Visible = Address2DetailLabel.Text.Trim.Length > 0
        Address3Label.Visible = Address3DetailLabel.Text.Trim.Length > 0
        Address4Label.Visible = Address4DetailLabel.Text.Trim.Length > 0
        Address5Label.Visible = Address5DetailLabel.Text.Trim.Length > 0
        PostCodeLabel.Visible = PostCodeDetailLabel.Text.Trim.Length > 0
        AddressYearsDetailsLabel.Text = Checkout.RetrievePaymentItemFromSession("YearsAtAddress", GlobalConstants.CHECKOUTASPXSTAGE)
        GetInstallmentPlanCodeDetails(Checkout.RetrievePaymentItemFromSession("InstallmentPlanCode", GlobalConstants.CHECKOUTASPXSTAGE))
    End Sub

    Private Sub GetInstallmentPlanCodeDetails(ByVal InstallmentPlanCode As String)
        If (Not String.IsNullOrWhiteSpace(InstallmentPlanCode)) Then
            Dim err As New Talent.Common.ErrorObj
            Dim cf As New Talent.Common.TalentCreditFinance
            Dim deCreditFinance As New Talent.Common.DECreditFinance
            With deCreditFinance
                .CreditFinanceCompanyCode = ModuleDefaults.CreditFinanceCompanyCode
                .CreditFinanceOptionCode = InstallmentPlanCode
            End With

            cf.De = deCreditFinance
            cf.Settings = Utilities.GetSettingsObject()
            cf.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            err = cf.CreditFinanceOptionDetails()

            If (Not err.HasError) AndAlso (cf.ResultDataSet IsNot Nothing) Then
                If cf.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    InstallmentPlanDetailLabel.Text = cf.ResultDataSet.Tables(0).Rows(0).Item("ShortDescription")
                    InstallmentExampleDetailLabel.Text = cf.ResultDataSet.Tables(0).Rows(0).Item("Example")
                End If
            End If
        End If
    End Sub

End Class
