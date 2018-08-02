Imports Talent.Common
Imports Talent.eCommerce

Partial Class UserControls_DirectDebitMandate
    Inherits ControlBase

#Region "Class Level Fields"

    Private _display As Boolean = True
    Private _pageCode As String = String.Empty
    Private _ucr As New UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

#End Region

#Region "Public Properties"

    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property

    Public Property PageCode() As String
        Get
            Return _pageCode
        End Get
        Set(ByVal value As String)
            _pageCode = value
        End Set
    End Property

#End Region

#Region "Protected Methods"

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        'Is this user control active
        If Me.Display Then

            'Set up the user control
            With _ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "DirectDebitMandate.ascx"
            End With

            'Populate the user control
            SetLabelText()
            SetDetailText()
        End If
    End Sub

    Protected Sub SetLabelText()
        With _ucr
            TitleLabel.Text = .Content("TitleLabel", _languageCode, True)
            AdressLine1.Text = .Content("AddressLine1", _languageCode, True)
            AdressLine2.Text = .Content("AddressLine2", _languageCode, True)
            AdressLine3.Text = .Content("AddressLine3", _languageCode, True)
            AdressLine4.Text = .Content("AddressLine4", _languageCode, True)
            AdressLine5.Text = .Content("AddressLine5", _languageCode, True)
            AdressLine6.Text = .Content("AddressLine6", _languageCode, True)
            plhAddressLine1.Visible = (AdressLine1.Text.Length > 0)
            plhAddressLine2.Visible = (AdressLine2.Text.Length > 0)
            plhAddressLine3.Visible = (AdressLine3.Text.Length > 0)
            plhAddressLine4.Visible = (AdressLine4.Text.Length > 0)
            plhAddressLine5.Visible = (AdressLine5.Text.Length > 0)
            plhAddressLine6.Visible = (AdressLine6.Text.Length > 0)
            AccountNameLabel.Text = .Content("AccountNameLabel", _languageCode, True)
            AccountNumberLabel.Text = .Content("AccountNumberLabel", _languageCode, True)
            SortCodeLabel.Text = .Content("SortCodeLabel", _languageCode, True)
            InstructionTitleLabel.Text = .Content("InstructionTitleLabel", _languageCode, True)
            BankNameLabel.Text = .Content("BankNameLabel", _languageCode, True)
            OriginatorLabel.Text = .Content("OriginatorLabel", _languageCode, True)
            OriginatorText.Text = .Content("OriginatorText", _languageCode, True)
            ReferenceNumberLabel.Text = .Content("ReferenceNumberLabel", _languageCode, True)
            InstructionTextLabel.Text = .Content("InstructionTextLabel", _languageCode, True)
            InstructionText.Text = .Content("InstructionText", _languageCode, True)
            TodaysDateLabel.Text = .Content("TodaysDateLabel", _languageCode, True)
            AdditionalText.Text = .Content("AdditionalText", _languageCode, True)

            ' Load the direct debit image
            Dim strImage As String = ImagePath.getImagePath("APPTHEME", .Attribute("DirectDebitLogo"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
            If Not strImage.Trim.Equals("") Then
                Me.DirectDebitImage.ImageUrl = strImage
            End If
        End With
    End Sub

    Protected Sub SetDetailText()
        'Populate the account details from session
        AccountName.Text = Checkout.RetrievePaymentItemFromSession("AccountName", PageCode)
        Dim sAccount As String = Checkout.RetrievePaymentItemFromSession("AccountNumber", PageCode)
        PopulateNumberBox("AccountNumber", Checkout.RetrievePaymentItemFromSession("AccountNumber", PageCode))
        PopulateNumberBox("SortCode", Checkout.RetrievePaymentItemFromSession("SortCode", PageCode))
        PopulateNumberBox("OriginatorNumber", Checkout.RetrievePaymentItemFromSession("Originator", PageCode))
        PopulateNumberBox("ReferenceNumber", Checkout.RetrievePaymentItemFromSession("DDIReference", PageCode))
        BankName.Text = Checkout.RetrievePaymentItemFromSession("BankName", PageCode).Replace(",", "<br />")
        Me.TodaysDate.Text = Date.Today.ToString.Substring(0, Date.Today.ToString.IndexOf(" "))
    End Sub

#End Region

#Region "Public Methods"

    Public Sub SetTextandValues()
        SetLabelText()
        SetDetailText()
    End Sub

#End Region

#Region "Private Methods"

    Private Sub PopulateNumberBox(ByVal boxReference As String, ByVal value As String)
        'Populate each element of the relevant number box
        Dim loopCount As Integer = 0
        For loopCount = 0 To value.Length - 1
            CType(Talent.eCommerce.Utilities.FindWebControl(boxReference & loopCount.ToString, Me.Controls), Label).Text = value.Substring(loopCount, 1)
        Next
    End Sub

#End Region

End Class