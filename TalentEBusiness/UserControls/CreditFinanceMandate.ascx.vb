Imports System.Data
Imports Talent.eCommerce

Partial Class UserControls_CreditFinanceMandate
    Inherits ControlBase

    Private ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults

    Public Property Display() As Boolean
    Public Property PageCode() As String

    Protected Sub SetLabelText()
        With ucr
            TitleLabel.Text = .Content("TitleLabel", _languageCode, True)
            AddressYearsLabel.Text = .Content("AddressYearsLabel", _languageCode, True)
            AccountNameLabel.Text = .Content("AccountNameLabel", _languageCode, True)
            AccountNumberLabel.Text = .Content("AccountNumberLabel", _languageCode, True)
            SortCodeLabel.Text = .Content("SortCodeLabel", _languageCode, True)
            InstructionTitleLabel.Text = .Content("InstructionTitleLabel", _languageCode, True)
            BankNameLabel.Text = .Content("BankNameLabel", _languageCode, True)
            InstallmentPlanLabel.Text = .Content("InstallmentPlanLabel", _languageCode, True)
            InstallmentPlanExampleLabel.Text = .Content("InstallmentPlanExampleLabel", _languageCode, True)
            InstructionTextLabel.Text = .Content("InstructionTextLabel", _languageCode, True)
            InstructionText.Text = .Content("InstructionText", _languageCode, True)
            TodaysDateLabel.Text = .Content("TodaysDateLabel", _languageCode, True)
            AdditionalText.Text = .Content("AdditionalText", _languageCode, True)

            ' Load the credit finance image
            Dim strImage As String = ImagePath.getImagePath("APPTHEME", .Attribute("CreditFinanceLogo"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
            If Not strImage.Trim.Equals("") Then
                Me.CreditFinanceImage.ImageUrl = strImage
            End If

        End With
    End Sub

    Protected Sub SetDetailText()
        'Populate the account details from session
        AccountName.Text = Checkout.RetrievePaymentItemFromSession("AccountName", GlobalConstants.CHECKOUTASPXSTAGE)
        Dim sAccount As String = Checkout.RetrievePaymentItemFromSession("AccountNumber", GlobalConstants.CHECKOUTASPXSTAGE)
        PopulateNumberBox("AccountNumber", Checkout.RetrievePaymentItemFromSession("AccountNumber", GlobalConstants.CHECKOUTASPXSTAGE))
        PopulateNumberBox("SortCode", Checkout.RetrievePaymentItemFromSession("SortCode", GlobalConstants.CHECKOUTASPXSTAGE))
        BankName.Text = Checkout.RetrievePaymentItemFromSession("BankName", GlobalConstants.CHECKOUTASPXSTAGE)
        Me.TodaysDate.Text = Date.Today.ToString.Substring(0, Date.Today.ToString.IndexOf(" "))
        GetInstallmentPlanCodeDetails(Checkout.RetrievePaymentItemFromSession("InstallmentPlanCode", GlobalConstants.CHECKOUTASPXSTAGE))
        AddressYearsText.Text = Checkout.RetrievePaymentItemFromSession("YearsAtAddress", GlobalConstants.CHECKOUTASPXSTAGE)
        AddressLine1.Text = Checkout.RetrievePaymentItemFromSession("AddressLine1", GlobalConstants.CHECKOUTASPXSTAGE)
        AddressLine2.Text = Checkout.RetrievePaymentItemFromSession("AddressLine2", GlobalConstants.CHECKOUTASPXSTAGE)
        AddressLine3.Text = Checkout.RetrievePaymentItemFromSession("AddressLine3", GlobalConstants.CHECKOUTASPXSTAGE)
        AddressLine4.Text = Checkout.RetrievePaymentItemFromSession("AddressLine4", GlobalConstants.CHECKOUTASPXSTAGE)
        AddressPostCode.Text = Checkout.RetrievePaymentItemFromSession("PostCode", GlobalConstants.CHECKOUTASPXSTAGE)
        'set the visibility for address fields
        plhAddressLine1.Visible = AddressLine1.Text.Trim.Length > 0
        plhAddressLine2.Visible = AddressLine2.Text.Trim.Length > 0
        plhAddressLine3.Visible = AddressLine3.Text.Trim.Length > 0
        plhAddressLine4.Visible = AddressLine4.Text.Trim.Length > 0
        plhAddressLine5.Visible = AddressLine5.Text.Trim.Length > 0
        plhAddressPostCode.Visible = AddressPostCode.Text.Trim.Length > 0
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        'Is this user control active
        If Me.Display Then

            'Set up the user control
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "CreditFinanceMandate.ascx"
            End With

            'Populate the user control
            SetLabelText()
            SetDetailText()
            AddressYearsLabel.Visible = False
            AddressYearsText.Visible = False
        End If
    End Sub

    Private Sub PopulateNumberBox(ByVal boxReference As String, ByVal value As String)
        'Populate each element of the relevant number box
        Dim loopCount As Integer = 0
        For loopCount = 0 To value.Length - 1
            CType(Utilities.FindWebControl(boxReference & loopCount.ToString, Me.Controls), Label).Text = value.Substring(loopCount, 1)
        Next
    End Sub

    ''' <summary>
    ''' Gets the installment plan code details.
    ''' </summary>
    ''' <param name="InstallmentPlanCode">The installment plan code.</param>
    Private Sub GetInstallmentPlanCodeDetails(ByVal InstallmentPlanCode As String)
        If (Not String.IsNullOrWhiteSpace(InstallmentPlanCode)) Then
            Dim err As New Talent.Common.ErrorObj

            Dim cf As New Talent.Common.TalentCreditFinance
            Dim deCreditFinance As New Talent.Common.DECreditFinance
            With deCreditFinance
                .CreditFinanceCompanyCode = _def.CreditFinanceCompanyCode
                .CreditFinanceOptionCode = InstallmentPlanCode
            End With

            cf.De = deCreditFinance
            cf.Settings = Utilities.GetSettingsObject()
            cf.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            err = cf.CreditFinanceOptionDetails()

            If (Not err.HasError) AndAlso (cf.ResultDataSet IsNot Nothing) Then
                If cf.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    InstallmentPlanText.Text = cf.ResultDataSet.Tables(0).Rows(0)("ShortDescription")
                    InstallmentPlanExampleText.Text = cf.ResultDataSet.Tables(0).Rows(0)("Example")
                End If
            End If
        End If
    End Sub
End Class
