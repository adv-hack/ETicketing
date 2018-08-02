Imports Talent.eCommerce
Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities

Partial Class UserControls_DirectDebitSummary
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ucr As UserControlResource = Nothing
    Private _languageCode As String = String.Empty

#End Region

#Region "Public Properties"

    Public Property PaymentRef() As String = String.Empty

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        _languageCode = TCUtilities.GetDefaultLanguage
        _ucr = New UserControlResource
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = TEBUtilities.GetCurrentPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "DirectDebitSummary.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack AndAlso PaymentRef.Length > 0 AndAlso Not Profile.IsAnonymous Then
            plhDirectDebitSummary.Visible = True
            setLabelText()
            setDetailText()
        End If
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Select Case CType(sender, Control).ID
            Case Is = "lblPaymentDateHeader"
                CType(sender, Label).Text = _ucr.Content("PaymentDateHeaderLabel", _languageCode, True)
            Case Is = "lblPaymentAmountHeader"
                CType(sender, Label).Text = _ucr.Content("PaymentAmountHeaderLabel", _languageCode, True)
            Case Is = "lblPaymentStatustHeader"
                CType(sender, Label).Text = _ucr.Content("PaymentStatusHeaderLabel", _languageCode, True)
        End Select
    End Sub

#End Region

#Region "Private Methods"

    Private Sub setLabelText()
        With _ucr
            ltlTitle.Text = .Content("TitleLabel", _languageCode, True)
            lblGuarantee.Text = .Content("GuaranteeLabel", _languageCode, True)
            lblDDIRef.Text = .Content("DDIRefLabel", _languageCode, True)
            lblAccountName.Text = .Content("AccountNameLabel", _languageCode, True)
            lblSortCode.Text = .Content("SortCodeLabel", _languageCode, True)
            lblAccountNumber.Text = .Content("AccountNumberLabel", _languageCode, True)

            ' Load the direct debit image
            Dim strImage As String = ImagePath.getImagePath("APPTHEME", .Attribute("DirectDebitLogo"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
            If Not strImage.Length > 0 Then
                imgDirectDebit.ImageUrl = strImage
            End If
        End With
    End Sub

    Private Sub setDetailText()
        Dim err As New ErrorObj
        Dim returnErrorCode As String = String.Empty

        'Create the payment object
        Dim payment As New TalentPayment
        Dim dePayment As New DEPayments
        With dePayment
            .PaymentRef = PaymentRef
            .Source = GlobalConstants.SOURCE
            .CustomerNumber = Profile.User.Details.LoginID
        End With

        payment.De = dePayment
        payment.Settings = TEBUtilities.GetSettingsObject()
        payment.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        payment.Settings.StoredProcedureGroup = ModuleDefaults.StoredProcedureGroup
        payment.Settings.Cacheing = False
        payment.Settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath

        'Retrieve the direct debit summary
        err = payment.DirectDebitSummary

        'Populate the ddi reference 
        If Not err.HasError Then
            If Not payment.ResultDataSet Is Nothing Then
                If payment.ResultDataSet.Tables.Contains("StatusResults") Then
                    If payment.ResultDataSet.Tables("StatusResults").Rows(0)("ErrorOccurred").ToString().Trim = GlobalConstants.ERRORFLAG Then
                        Dim errMsg As New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), _ucr.FrontEndConnectionString)
                        Dim message As TalentErrorMessage = errMsg.GetErrorMessage(payment.ResultDataSet.Tables("StatusResults").Rows(0)("ReturnCode").ToString().Trim)
                        ErrorList.Items.Add(message.ERROR_MESSAGE)
                    Else
                        lblDDIRefDetail.Text = payment.ResultDataSet.Tables("StatusResults").Rows(0).Item("DirectDebitDDIRef").ToString.Trim
                        lblAccountNameDetail.Text = payment.ResultDataSet.Tables("StatusResults").Rows(0).Item("AccountName").ToString.Trim
                        lblSortCodeDetail.Text = payment.ResultDataSet.Tables("StatusResults").Rows(0).Item("SortCode").ToString.Trim
                        lblAccountNumberDetail.Text = payment.ResultDataSet.Tables("StatusResults").Rows(0).Item("AccountNumber").ToString.Trim

                        If payment.ResultDataSet.Tables.Contains("DirectDebitSummary") Then
                            rptPaymentDate.DataSource = payment.ResultDataSet.Tables("DirectDebitSummary")
                            rptPaymentDate.DataBind()
                            rptPaymentAmount.DataSource = payment.ResultDataSet.Tables("DirectDebitSummary")
                            rptPaymentAmount.DataBind()
                            rptPaymentStatus.DataSource = payment.ResultDataSet.Tables("DirectDebitSummary")
                            rptPaymentStatus.DataBind()
                        End If
                    End If
                End If
            End If
        Else
            'Return to the basket and display the relevant errors
            ErrorList.Items.Add(_ucr.Content("DirectDebitSummaryError", _languageCode, True))
        End If
    End Sub

#End Region

End Class
