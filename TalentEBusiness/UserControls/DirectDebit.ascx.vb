Imports Talent.Common
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_DirectDebit
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _bankName As String = String.Empty
    Private _display As Boolean = True
    Private _usePaymentDaysDDL As Boolean = True
    Private _errorMessage As String = String.Empty

#End Region

#Region "Public Properties"

    Public ReadOnly Property AccountNameBox() As TextBox
        Get
            Return AccountName
        End Get
    End Property
    Public ReadOnly Property SortCode1Box() As TextBox
        Get
            Return SortCode1
        End Get
    End Property
    Public ReadOnly Property SortCode2Box() As TextBox
        Get
            Return SortCode2
        End Get
    End Property
    Public ReadOnly Property SortCode3Box() As TextBox
        Get
            Return SortCode3
        End Get
    End Property
    Public ReadOnly Property AccountNumberBox() As TextBox
        Get
            Return AccountNumber
        End Get
    End Property
    Public ReadOnly Property PaymentDayDropDownList() As DropDownList
        Get
            Return PaymentDayDDL
        End Get
    End Property
    Public ReadOnly Property ErrorMessage() As String
        Get
            Return _errorMessage
        End Get
    End Property
    Public Property BankName() As String
        Get
            Return _bankName
        End Get
        Set(ByVal value As String)
            _bankName = value
        End Set
    End Property
    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property
    Public Property UsePaymentDaysDDL() As Boolean
        Get
            Return _usePaymentDaysDDL
        End Get
        Set(ByVal value As Boolean)
            _usePaymentDaysDDL = value
        End Set
    End Property

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Me.Display Then
            plhDirectDebit.Visible = True
            With _ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "DirectDebit.ascx"
            End With
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Me.Display Then
            SetText()
            SetValidators()
            SetFields()
            If _usePaymentDaysDDL Then
                PopulatePaymentDaysDDL()
                plhPaymentDayDDL.Visible = True
            End If
        End If
    End Sub

    Private Sub PopulatePaymentDaysDDL()
        If PaymentDayDDL.Items.Count <= 0 Then
            'Retrieve the valid Payment Days from the backend
            Dim err As New Talent.Common.ErrorObj
            Dim returnErrorCode As String = String.Empty
            Dim timeSpan As TimeSpan = Now.TimeOfDay
            Dim validDays As String = String.Empty

            'Create the payment object
            Dim payment As New Talent.Common.TalentPayment
            Dim dePayment As New Talent.Common.DEPayments
            With dePayment
                .SessionId = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                .Source = "W"
                .PaymentStage = TEBUtilities.GetCurrentPageName.Trim
                .CustomerNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
            End With

            payment.De = dePayment
            payment.Settings = TEBUtilities.GetSettingsObject()
            payment.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            payment.Settings.StoredProcedureGroup = TEBUtilities.GetStoredProcedureGroup
            err = payment.DirectDebitPaymentDays

            'Populate validDays string from the results from call to WS084R
            If Not err.HasError AndAlso Not payment.ResultDataSet Is Nothing AndAlso payment.ResultDataSet.Tables.Count > 1 Then
                validDays = payment.ResultDataSet.Tables(1).Rows(0).Item("PaymentDays").ToString
            Else
                If payment.ResultDataSet.Tables.Count = 1 AndAlso _
                    payment.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode").ToString.Trim <> "" Then
                    HttpContext.Current.Session("TalentErrorCode") = payment.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode").ToString.Trim
                Else
                    HttpContext.Current.Session("TalentErrorCode") = "XX"
                End If
            End If
            TEBUtilities.TalentLogging.LoadTestLog("Checkout.vb", "RetrieveDirectDebitIds", timeSpan)

            ' If single value only then do not display the dropdown list of choices
            ' If more than 1 value then sort and then populate teh dropdown lis
            If validDays.Trim.Length <= 2 Then
                Me.PaymentDayLabel.Visible = False
                Me.PaymentDayDDL.Visible = False
            Else
                'Add each valid payment day to the drop down list 
                Dim i As Integer = 0
                Dim sDay As String = String.Empty
                Dim sDays As New ArrayList(16)
                Dim sExt As String = String.Empty
                For i = 1 To 16
                    sDay = validDays.Substring((i - 1) * 2, 2).Trim
                    If sDay <> "" And sDays.IndexOf(sDay) < 0 Then
                        sDays.Add(sDay)
                    End If
                Next
                sDays.Sort()
                For Each sDay In sDays
                    If sDay.Substring(0, 1) = "0" Then sDay = sDay.Substring(1, 1)
                    Select Case sDay
                        Case Is = "1", "21", "31"
                            sExt = "st"
                        Case Is = "2", "22"
                            sExt = "nd"
                        Case Is = "3", "23"
                            sExt = "rd"
                        Case Else
                            sExt = "th"
                    End Select
                    PaymentDayDDL.Items.Add(New ListItem(sDay + sExt, sDay))
                Next

                ' Ensure that the first item from validDays is the default 
                Dim sDef As String = String.Empty
                sDef = validDays.Substring(0, 2).Trim
                If sDef.Substring(0, 1) = "0" Then sDef = sDef.Substring(1, 1)
                PaymentDayDDL.SelectedValue = sDef
            End If
        End If
    End Sub

    Private Sub SetText()
        With _ucr
            AccountNameLabel.Text = .Content("AccountNameLabel", _languageCode, True)
            SortCodeLabel.Text = .Content("SortCodeLabel", _languageCode, True)
            AccountNumberLabel.Text = .Content("AccountNumberLabel", _languageCode, True)
            PaymentDayLabel.Text = .Content("PaymentDayLabel", _languageCode, True)
        End With
    End Sub

    Private Sub SetFields()
        If AgentProfile.IsAgent AndAlso Not Profile.IsAnonymous Then
            Dim talDirectDebits As New TalentDirectDebitDetails
            Dim err As New ErrorObj
            talDirectDebits.Settings = TEBUtilities.GetSettingsObject()
            talDirectDebits.Company = AgentProfile.GetAgentCompany()
            talDirectDebits.Customer = Profile.User.Details.LoginID
            err = talDirectDebits.RetrieveTalentDirectDebitDetails()
            If Not err.HasError Then
                If AccountNameBox.Text.Length = 0 Then
                    AccountNameBox.Text = talDirectDebits.deDirectDebitDetails.AccountName
                End If
                If SortCode1Box.Text.Length = 0 Then
                    SortCode1Box.Text = talDirectDebits.deDirectDebitDetails.SortCode1
                End If
                If SortCode2Box.Text.Length = 0 Then
                    SortCode2Box.Text = talDirectDebits.deDirectDebitDetails.SortCode2
                End If
                If SortCode3Box.Text.Length = 0 Then
                    SortCode3Box.Text = talDirectDebits.deDirectDebitDetails.SortCode3
                End If
                If AccountNumberBox.Text.Length = 0 Then
                    AccountNumberBox.Text = talDirectDebits.deDirectDebitDetails.AccountNumber
                End If
            End If
        End If
    End Sub

    Private Sub SetValidators()
        With _ucr
            AccountNameRFV.ErrorMessage = .Content("NoAccountNameErrorMessage", _languageCode, True)
            AccountNameRegEx.ErrorMessage = .Content("InvalidAccountNameErrorMessage", _languageCode, True)
            AccountNameRegEx.ValidationExpression = .Attribute("AccountNameRegularExpression")

            SortCode1RegEx.ErrorMessage = .Content("InvalidSortCodeErrorMessage", _languageCode, True)
            SortCode1RegEx.ValidationExpression = .Attribute("SortCodeRegularExpression")
            SortCode2RegEx.ErrorMessage = .Content("InvalidSortCodeErrorMessage", _languageCode, True)
            SortCode2RegEx.ValidationExpression = .Attribute("SortCodeRegularExpression")
            SortCode3RegEx.ErrorMessage = .Content("InvalidSortCodeErrorMessage", _languageCode, True)
            SortCode3RegEx.ValidationExpression = .Attribute("SortCodeRegularExpression")
            csvSortCode.ErrorMessage = .Content("NoSortCodeErrorMessage", _languageCode, True)

            AccountNumberRFV.ErrorMessage = .Content("NoAccountNumberErrorMessage", _languageCode, True)
            AccountNumberRegEx.ErrorMessage = .Content("InvalidAccountNumberErrorMessage", _languageCode, True)
            AccountNumberRegEx.ValidationExpression = .Attribute("AccountNumberRegularExpression")
        End With
    End Sub

#End Region

#Region "Public Methods"

    Public Function ValidateUserInput() As Boolean
        Dim err As Boolean = False
        _errorMessage = String.Empty

        If String.IsNullOrEmpty(AccountName.Text) Then
            _errorMessage = _ucr.Content("NoAccountNameErrorMessage", _languageCode, True)
            err = True
        End If
        If String.IsNullOrEmpty(SortCode1.Text) Or String.IsNullOrEmpty(SortCode2.Text) Or String.IsNullOrEmpty(SortCode3.Text) Then
            _errorMessage = _ucr.Content("NoSortCodeErrorMessage", _languageCode, True)
            err = True
        End If
        If String.IsNullOrEmpty(AccountNumber.Text) Then
            _errorMessage = _ucr.Content("NoAccountNumberErrorMessage", _languageCode, True)
            err = True
        End If

        Dim errorCode As String = "0"
        Dim sortCode As String = SortCode1.Text & SortCode2.Text & SortCode3.Text

        BankName = String.Empty
        Dim utility As New Talent.Common.Utilities
        Dim accountisKey As String = _ucr.Attribute("AccountisKey")
        Dim accountisUrl As String = _ucr.Attribute("AccountisUrl")
        If Not err = True Then
            If _ucr.Attribute("AccountValidationMethod") = "Accountis" AndAlso accountisKey <> String.Empty Then

                errorCode = utility.ValidateBankAccountWithAccountis(sortCode, AccountNumber.Text, accountisKey, accountisUrl, BankName)
                BankName = BankName.Replace("""", "")

                Select Case errorCode
                    Case Is = "1"
                        _errorMessage = _ucr.Content("InvalidSortCodeErrorMessage", _languageCode, True)
                        err = True
                    Case Is = "2"
                        _errorMessage = _ucr.Content("InvalidAccountNumberErrorMessage", _languageCode, True)
                        err = True
                    Case Is = "3"
                        _errorMessage = _ucr.Content("NoLicenceErrorMessage", _languageCode, True)
                        err = True
                    Case Is = "4"
                        _errorMessage = _ucr.Content("NoCommsErrorMessage", _languageCode, True)
                        err = True
                End Select

            ElseIf _ucr.Attribute("AccountValidationMethod") = "PremiumCredit" Then
                'Premium Credit Mode
                Dim premiumCreditUrl As String = _ucr.Attribute("PremiumCreditUrl")
                Dim premiumCreditUserName As String = _ucr.Attribute("PremiumCreditUserName")
                Dim premiumCreditpassword As String = _ucr.Attribute("PremiumCreditPassword")
                errorCode = utility.ValidateBankAccountWithPremiumCredit( _
                    premiumCreditUrl, _
                    premiumCreditUserName, _
                    premiumCreditpassword, _
                    sortCode, _
                    AccountNumber.Text)

                If errorCode <> "0" Then
                    Dim ta As LoggingDataSet.tbl_ecommerce_error_logDataTable = New LoggingDataSet.tbl_ecommerce_error_logDataTable
                    ta.Addtbl_ecommerce_error_logRow(TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile), Profile.User.Details.LoginID, errorCode, "PremiumCreditErrorCode" & errorCode, "PremiumCreditError", "checkoutDirectDebitDetails.aspx", "DirectDebit.ascx", Now())

                    'Add tbl_ecommerce_error_log Record
                    Select Case errorCode
                        Case Is = "14"
                            _errorMessage = _ucr.Content("InvalidSortCodeErrorMessage", _languageCode, True)
                            err = True
                        Case Is = "15"
                            _errorMessage = _ucr.Content("InvalidAccountNumberErrorMessage", _languageCode, True)
                            err = True
                        Case Is = "2"
                            _errorMessage = _ucr.Content("NoLicenceErrorMessage", _languageCode, True)
                            err = True
                        Case Is = "999"
                            _errorMessage = _ucr.Content("NoCommsErrorMessage", _languageCode, True)
                            err = True
                        Case Else
                            _errorMessage = _ucr.Content("ValidationFailedErrorMessage", _languageCode, True)
                            err = True
                    End Select
                End If
            ElseIf _ucr.Attribute("AccountValidationMethod") = "FundTech" Then
                err = ValidateBankAccountByFundTech(errorCode)
            End If
        End If
        Return Not err
    End Function

    Public Sub ResetForm()
        AccountName.Text = String.Empty
        SortCode1.Text = String.Empty
        SortCode2.Text = String.Empty
        SortCode3.Text = String.Empty
        AccountNumber.Text = String.Empty
        If PaymentDayDDL.Items.Count > 0 Then PaymentDayDDL.SelectedIndex = 0
    End Sub

    Public Sub ValidateSortCode(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        e.IsValid = (SortCode1Box.Text.Length > 0 AndAlso SortCode2Box.Text.Length > 0 AndAlso SortCode1Box.Text.Length > 0)
    End Sub

#End Region

#Region "Private Methods"
    Private Function ValidateBankAccountByFundTech(ByRef errorCode As String) As Boolean
        Dim err As Boolean = False
        Try
            Dim sortCode As String = SortCode1.Text & SortCode2.Text & SortCode3.Text
            Dim utility As New Talent.Common.Utilities
            Dim fundTechKey As String = _ucr.Attribute("FundTechKey")
            Dim fundTechUrl As String = _ucr.Attribute("FundTechUrl")
            If Not fundTechUrl.EndsWith("/") Then fundTechUrl = fundTechUrl & "/"
            errorCode = utility.ValidateBankAccountWithFundTech(sortCode, AccountNumber.Text, fundTechKey, fundTechUrl, "IsAccountNoValid")
            If errorCode.Trim.Length > 0 Then
                Dim ta As LoggingDataSet.tbl_ecommerce_error_logDataTable = New LoggingDataSet.tbl_ecommerce_error_logDataTable
                ta.Addtbl_ecommerce_error_logRow(TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile), Profile.User.Details.LoginID, errorCode, "FundTechErrorCode" & errorCode, "FundTechError", "checkoutDirectDebitDetails.aspx", "DirectDebit.ascx", Now())
                Select Case errorCode
                    Case Is = "E-10102"
                        _errorMessage = _ucr.Content("InvalidSortCodeErrorMessage", _languageCode, True)
                        err = True
                    Case Is = "E-10103"
                        _errorMessage = _ucr.Content("InvalidAccountNumberErrorMessage", _languageCode, True)
                        err = True
                    Case Is = "E-10301"
                        _errorMessage = _ucr.Content("InvalidSortAndAccountErrorMessage", _languageCode, True)
                        err = True
                    Case Is = "E-10001"
                        _errorMessage = _ucr.Content("NoLicenceErrorMessage", _languageCode, True)
                        err = True
                    Case Else
                        _errorMessage = _ucr.Content("NoCommsErrorMessage", _languageCode, True)
                        err = True
                End Select
            End If
        Catch ex As Exception
            errorCode = "E-EXCEP"
            err = True
            Dim errMessage As String = String.Empty
            errMessage = ex.Message + ";" + ex.StackTrace
            Talent.eCommerce.Utilities.TalentLogging.GeneralLog("DirectDebit", errMessage, "FundTechAccountValidationLog")
        End Try

        Return err
    End Function
#End Region

End Class