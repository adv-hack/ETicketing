Imports TCUtilities = Talent.Common.Utilities
Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Data

Partial Class UserControls_NotificationOptions
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ucr As UserControlResource = Nothing
    Private _languageCode As String = Nothing

#End Region

#Region "Public Properties"
    Public ReadOnly Property EmailIDText() As TextBox
        Get
            Return txtEmailID
        End Get
    End Property

    Public ReadOnly Property MobileNumberTxt() As TextBox
        Get
            Return txtMobileNumber
        End Get
    End Property

    Public ReadOnly Property SendEmailSelector() As CheckBox
        Get
            Return chkSendEmail
        End Get
    End Property

    Public ReadOnly Property SaveEmailSelector() As CheckBox
        Get
            Return chkSaveEmail
        End Get
    End Property

    Public ReadOnly Property SaveMobileNumberSelector() As CheckBox
        Get
            Return chkSaveNumber
        End Get
    End Property

    Public Property CATMode() As String
        Get
            Return hdfCATMode.Value
        End Get
        Set(value As String)
            hdfCATMode.Value = value
        End Set
    End Property
    Private Property _sendEmail As Boolean = True
#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If ModuleDefaults.NotificationsOnConfirmPage AndAlso Not Profile.IsAnonymous Then
            _ucr = New UserControlResource
            With _ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "NotificationOptions.ascx"
            End With
            _languageCode = TCUtilities.GetDefaultLanguage
            SetText()
            SetValidators()
            SetValues()
            DisplaySMSConfirmation()
        Else
            Me.Visible = False
        End If
    End Sub
    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If Not Session("DontSendEmail") Is Nothing Then
            _sendEmail = False
            Session.Remove("DontSendEmail")
        End If
        DisplayEmailConfirmation()
    End Sub
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set the text of the labels
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetText()
        lblSendEmail.Text = _ucr.Content("SendEmailLabel", _languageCode, True)
        lblEmailID.Text = _ucr.Content("EmailIDLabel", _languageCode, True)
        lblSaveEmail.Text = _ucr.Content("SaveEmailLabel", _languageCode, True)
        lblEMailPrefFlag.Text = _ucr.Content("UpdateEMailPrefFlagLabel", _languageCode, True)

        lblMobileNumber.Text = _ucr.Content("MobileNoLabel", _languageCode, True)
        lblSaveNumber.Text = _ucr.Content("SaveMobileNoLabel", _languageCode, True)
        lblMobilePrefFlag.Text = _ucr.Content("UpdateMobilePrefFlagLabel", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Set the error messages and validation expressions
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub SetValidators()
        rfvEmailID.ErrorMessage = _ucr.Content("emailRFVErrorMessage", _languageCode, True)
        rfvMobileNumber.ErrorMessage = _ucr.Content("mobileNumberRFVErrorMessage", _languageCode, True)
        regEmailID.ValidationExpression = _ucr.Attribute("EmailIDRegularExpression")
        regEmailID.ErrorMessage = _ucr.Content("EmailRegExMessage", _languageCode, True)
        regMobileNumber.ValidationExpression = _ucr.Attribute("MobilenumberRegularExpression")
        regMobileNumber.ErrorMessage = _ucr.Content("MobileNoRegExMessage", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Set the visibility of the controls for the email 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisplayEmailConfirmation()
        Dim displayEmail As Boolean = TCUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowEmailConfirmation"))
        If displayEmail Then
            plhEmailID.Visible = True
            plhSaveEmail.Visible = True
            plhSendEmail.Visible = True
            rfvEmailID.Enabled = True
            If Not _sendEmail Then
                plhSendEmail.Visible = False
                chkSendEmail.Checked = False
            End If
        Else
            plhEmailID.Visible = False
            plhSaveEmail.Visible = False
            plhSendEmail.Visible = False
            rfvEmailID.Enabled = False
        End If
    End Sub

    ''' <summary>
    ''' Set the visibility of the controls for the sms
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisplaySMSConfirmation()
        Dim displaySMS As Boolean = TCUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowSMSConfirmation"))
        If displaySMS Then
            plhMobileNumber.Visible = True
            plhSaveMobileNumber.Visible = True
            plhMobilePrefFlag.Visible = True
        Else
            plhMobileNumber.Visible = False
            plhSaveMobileNumber.Visible = False
            plhMobilePrefFlag.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Initialize the values of the fields
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetValues()
        If Not Profile.IsAnonymous AndAlso Profile.User IsNot Nothing AndAlso Profile.User.Details IsNot Nothing Then
            txtEmailID.Text = Profile.User.Details.Email
            txtMobileNumber.Text = Profile.User.Details.Mobile_Number
            chkEMailPrefFlag.Checked = Profile.User.Details.CONTACT_BY_EMAIL
            chkMobilePrefFlag.Checked = Profile.User.Details.CONTACT_BY_MOBILE
        Else
            Me.Visible = False
        End If
    End Sub
#End Region

#Region "Public Function"

    ''' <summary>
    '''  Send the email and update the email or mobile number.
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function ConfirmNotificationOptions(Optional ByVal performRedirect As Boolean = True) As ErrorObj
        Dim err As New ErrorObj
        Dim dePayment As New DEPayments
        Dim deCustomer As New DECustomer
        Dim payment As New TalentPayment
        Dim hasUpdates As Boolean = False
        If Profile.IsAnonymous Then
            'Used for generic sales in BUI
            If performRedirect Then
                Response.Redirect(TEBUtilities.GetSiteHomePage())
            End If
        Else
            deCustomer.CustomerNumber = Profile.User.Details.LoginID
            deCustomer.EmailAddress = Profile.User.Details.Email
            deCustomer.MobileNumber = Profile.User.Details.Mobile_Number

            If chkSaveEmail.Checked AndAlso deCustomer.EmailAddress <> txtEmailID.Text Then
                deCustomer.EmailAddress = txtEmailID.Text
                hasUpdates = True
            End If
            If chkEMailPrefFlag.Checked Then
                deCustomer.ContactViaEmail = "Y"
            Else
                deCustomer.ContactViaEmail = "N"
            End If
            If chkEMailPrefFlag.Checked <> Profile.User.Details.CONTACT_BY_EMAIL Then
                hasUpdates = True
            End If

            If chkSaveNumber.Checked AndAlso deCustomer.MobileNumber <> txtMobileNumber.Text Then
                deCustomer.MobileNumber = txtMobileNumber.Text
                hasUpdates = True
            End If
            If chkMobilePrefFlag.Checked Then
                deCustomer.ContactViaMobile = "Y"
            Else
                deCustomer.ContactViaMobile = "N"
            End If
            If chkMobilePrefFlag.Checked <> Profile.User.Details.CONTACT_BY_MOBILE Then
                hasUpdates = True
            End If

            If hasUpdates Then
                dePayment.Source = GlobalConstants.SOURCE
                dePayment.AgentName = AgentProfile.Name
                dePayment.CustomerNumber = Profile.User.Details.LoginID
                dePayment.CustomerDataEntity = deCustomer
                payment.Settings = TEBUtilities.GetSettingsObject
                payment.De = dePayment
                err = payment.OrderCompletionUpdates()

                If (Not err.HasError) AndAlso payment.ResultDataSet IsNot Nothing Then
                    If payment.ResultDataSet.Tables.Count = 1 AndAlso payment.ResultDataSet.Tables("ErrorStatus").Rows.Count > 0 Then
                        If TEBUtilities.CheckForDBNull_String(payment.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred")) = GlobalConstants.ERRORFLAG Then
                            err.HasError = True
                        Else
                            err.HasError = False
                            Profile.User.Details.Email = txtEmailID.Text
                            Profile.User.Details.Mobile_Number = txtMobileNumber.Text
                            Profile.User.Details.CONTACT_BY_EMAIL = chkEMailPrefFlag.Checked
                            Profile.User.Details.CONTACT_BY_MOBILE = chkMobilePrefFlag.Checked
                            Profile.Save()
                        End If
                    End If
                End If
            End If

            ' send email
            payment.De.CATMode = CATMode
            Dim paymentRef As String = Request.QueryString("paymentref")
            If chkSendEmail.Checked AndAlso (Not String.IsNullOrWhiteSpace(paymentRef)) AndAlso Not err.HasError Then
                Dim Order_Email As New Talent.eCommerce.Order_Email
                Dim tGatewayFunctions As New TicketingGatewayFunctions
                Dim fileAttachments As System.Collections.Generic.List(Of String) = Nothing
                If String.IsNullOrWhiteSpace(payment.De.CATMode) Then
                    fileAttachments = tGatewayFunctions.GetHospitalityPDFAttachmentList(paymentRef)
                    Order_Email.SendTicketingConfirmationEmail(paymentRef, Nothing, txtEmailID.Text, fileAttachments)
                ElseIf (Not String.IsNullOrWhiteSpace(payment.De.CATMode)) AndAlso (Not Profile.IsAnonymous) Then
                    If payment.De.CATMode = GlobalConstants.CATMODE_AMEND Then
                        fileAttachments = tGatewayFunctions.GetHospitalityPDFAttachmentList(paymentRef)
                    End If
                    Order_Email.SendCATConfirmationEmail(paymentRef, payment.De.CATMode, txtEmailID.Text, fileAttachments)
                End If
            End If
        End If
        Return err
    End Function
#End Region


End Class
