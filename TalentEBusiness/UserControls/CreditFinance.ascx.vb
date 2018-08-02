Imports Talent.eCommerce
Imports System.Data

Partial Class UserControls_CreditFinance
    Inherits ControlBase

    Dim ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Dim utilitites As New Talent.Common.TalentUtiltities

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
    Public ReadOnly Property InstallmentPlanDropDownList() As DropDownList
        Get
            Return InstallmentPlanDDL
        End Get
    End Property
    Public ReadOnly Property NoOfYearsAtAddress() As String
        Get
            Return ddlNoOfYears.SelectedValue.ToString
        End Get
    End Property
    Public ReadOnly Property AddressLine1() As TextBox
        Get
            Return Address1
        End Get
    End Property
    Public ReadOnly Property AddressLine2() As TextBox
        Get
            Return Address2
        End Get
    End Property
    Public ReadOnly Property AddressLine3() As TextBox
        Get
            Return Town
        End Get
    End Property
    Public ReadOnly Property AddressLine4() As TextBox
        Get
            Return County
        End Get
    End Property
    Public ReadOnly Property AddressPostCode() As TextBox
        Get
            Return PostCode
        End Get
    End Property

    Public Property ErrorList() As New BulletedList
    Public Property BankName() As String = String.Empty
    Public Property Display() As Boolean = True
    Public Property OnlyAvailablePlan() As String
    Public Property clubProductCodeForFinance() As String
    Public ReadOnly Property MonthsAtAddress() As String
        Get
            Return ddlNoofmonths.SelectedValue.ToString
        End Get
    End Property
    Public ReadOnly Property HomeStatus() As String
        Get
            Return ddlHomeStatus.SelectedValue.ToString
        End Get
    End Property
    Public ReadOnly Property EmploymentStatus() As String
        Get
            Return ddlEmployment.SelectedValue.ToString
        End Get
    End Property
    Public ReadOnly Property GrossIncome() As String
        Get
            Return ddlIncome.SelectedValue.ToString
        End Get
    End Property


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Me.Display Then
            pnlCreditFinance.Visible = True
            pnlCreditFinanceExample.Visible = True
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "CreditFinance.ascx"
            End With
        End If

    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Me.Display AndAlso Profile.Basket.PAYMENT_OPTIONS.Contains(GlobalConstants.CFPAYMENTTYPE) Then
            Dim eComDefs As New ECommerceModuleDefaults()
            def = eComDefs.GetDefaults
            'Set text and validators
            SetText()
            SetValidators()
            GetInstallmentPlanDetails()
        End If
        SetFields()
    End Sub
    Protected Sub SetText()
        With ucr
            ExampleTitleLabel.Text = .Content("ExampleTitleLabel", _languageCode, True)
            lblIntroduction.Text = .Content("Introduction", _languageCode, True)

            TitleLabel.Text = .Content("TitleLabel", _languageCode, True)
            AccountNameLabel.Text = .Content("AccountNameLabel", _languageCode, True)
            SortCodeLabel.Text = .Content("SortCodeLabel", _languageCode, True)
            AccountNumberLabel.Text = .Content("AccountNumberLabel", _languageCode, True)
            InstallmentPlanLabel.Text = .Content("InstallmentPlanLabel", _languageCode, True)

            LocCurrentDetails.Text = .Content("CurrentDetailsLabel", _languageCode, True)
            CustomerNameLabel.Text = .Content("CustomerNameLabel", _languageCode, True)
            MobileLabel.Text = .Content("MobileLabel", _languageCode, True)
            EmailLabel.Text = .Content("EmailLabel", _languageCode, True)
            Address1Label.Text = .Content("Address1Label", _languageCode, True)
            Address2Label.Text = .Content("Address2Label", _languageCode, True)
            TownLabel.Text = .Content("TownLabel", _languageCode, True)
            CountyLabel.Text = .Content("CountyLabel", _languageCode, True)
            PostCodeLabel.Text = .Content("PostCodeLabel", _languageCode, True)
            NoOfYearsLabel.Text = .Content("NoOfYearsLabel", _languageCode, True)

            ' These are for V12 finance only
            If def.CreditFinanceCompanyCode = "V" Then
                NoOfYearsLabel.Text = .Content("NoOfYearsAtAddressLabel", _languageCode, True)
                NoofmonthsLabel.Text = .Content("NoofmonthsLabel", _languageCode, True)
                HomeStatusLabel.Text = .Content("HomeStatusLabel", _languageCode, True)
                EmploymentLabel.Text = .Content("EmploymentLabel", _languageCode, True)
                IncomeLabel.Text = .Content("IncomeLabel", _languageCode, True)
                Dim liBlank As New ListItem(" ")

                Dim i As Integer
                ddlNoofmonths.Items.Clear()
                ddlNoOfYears.Items.Clear()
                ddlIncome.Items.Clear()
                ddlNoofmonths.Items.Add(liBlank)
                For i = 0 To 12
                    ddlNoofmonths.Items.Add(i)
                Next
                ddlNoOfYears.Items.Add(liBlank)
                For i = 0 To 100
                    ddlNoOfYears.Items.Add(i)
                Next
                ddlIncome.Items.Add(liBlank)
                For i = 0 To 100000 Step 1000
                    ddlIncome.Items.Add(i)
                Next
                LoadDDL_HomeStatus()
                LoadDDL_Employment()

                PLHV12Fields.Visible = True

                ' Zebra have years defaulted to 3
            Else
                ddlNoOfYears.Text = "3"
                NoOfYearsLabel.Visible = False
                ddlNoOfYears.Visible = False
                PLHV12Fields.Visible = False
            End If


        End With
    End Sub

    Protected Sub SetFields()

        Dim currentPageName As String = Utilities.GetCurrentPageName.Trim
        Dim payType As String = Checkout.RetrievePaymentItem("PaymentType", UCase(currentPageName), True, True, "CF").Trim
        If (Not payType = "") AndAlso (payType = "CF") Then
            AccountName.Text = Checkout.RetrievePaymentItem("AccountName", currentPageName, True, True, "CF")
            AccountNumber.Text = Checkout.RetrievePaymentItem("AccountNumber", currentPageName, True, True, "CF")
            Dim sortCode As String = Checkout.RetrievePaymentItem("SortCode", currentPageName, True, True, "CF")
            If sortCode.Length = 6 Then
                SortCode1.Text = sortCode.Substring(0, 2)
                SortCode2.Text = sortCode.Substring(2, 2)
                SortCode3.Text = sortCode.Substring(4, 2)
            End If
            InstallmentPlanDropDownList.SelectedValue = Checkout.RetrievePaymentItem("InstallmentPlanCode", currentPageName, True, True, "CF")
        End If

        CustomerName.Text = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Full_Name
        Mobile.Text = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Mobile_Number
        Email.Text = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Email
        Dim address As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)
        Address1.Text = address.Address_Line_2
        Address2.Text = address.Address_Line_3
        Town.Text = address.Address_Line_4
        County.Text = address.Address_Line_5
        PostCode.Text = address.Post_Code

    End Sub

    Protected Sub SetValidators()
        With ucr
            If Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("AccountNameEnableRFV")) Then
                AccountNameRFV.Enabled = True
                AccountNameRegEx.Enabled = True
                AccountNameRFV.ErrorMessage = .Content("NoAccountNameErrorMessage", _languageCode, True)
                AccountNameRegEx.ErrorMessage = .Content("InvalidAccountNameErrorMessage", _languageCode, True)
                AccountNameRegEx.ValidationExpression = .Attribute("AccountNameRegularExpression")
            Else
                AccountNameRFV.Enabled = False
                AccountNameRegEx.Enabled = False
            End If

            If Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("SortCodeEnableRFV")) Then
                SortCode1RegEx.Enabled = True
                SortCode1RegEx.ErrorMessage = .Content("InvalidSortCodeErrorMessage", _languageCode, True)
                SortCode1RegEx.ValidationExpression = .Attribute("SortCodeRegularExpression")
                SortCode2RegEx.Enabled = True
                SortCode2RegEx.ErrorMessage = .Content("InvalidSortCodeErrorMessage", _languageCode, True)
                SortCode2RegEx.ValidationExpression = .Attribute("SortCodeRegularExpression")
                SortCode3RegEx.Enabled = True
                SortCode3RegEx.ErrorMessage = .Content("InvalidSortCodeErrorMessage", _languageCode, True)
                SortCode3RegEx.ValidationExpression = .Attribute("SortCodeRegularExpression")
                csvSortCode.ErrorMessage = .Content("NoSortCodeErrorMessage", _languageCode, True)
                csvSortCode.Enabled = True
            Else
                SortCode1RegEx.Enabled = False
                SortCode2RegEx.Enabled = False
                SortCode3RegEx.Enabled = False
                csvSortCode.Enabled = False
            End If

            If Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("AccountNumberEnableRFV")) Then
                AccountNumberRFV.Enabled = True
                AccountNumberRegEx.Enabled = True
                AccountNumberRFV.ErrorMessage = .Content("NoAccountNumberErrorMessage", _languageCode, True)
                AccountNumberRegEx.ErrorMessage = .Content("InvalidAccountNumberErrorMessage", _languageCode, True)
                AccountNumberRegEx.ValidationExpression = .Attribute("AccountNumberRegularExpression")
            Else
                AccountNumberRFV.Enabled = False
                AccountNumberRegEx.Enabled = False
            End If

            'Following are dropdown lists (only shown for V12) so need RFV but not regular expression validator
            RVFHomeStatus.ErrorMessage = .Content("HomeStatusRequiredMessage", _languageCode, True)
            RVFEmployment.ErrorMessage = .Content("EmploymentStatusRequiredMessage", _languageCode, True)
            RVFIncome.ErrorMessage = .Content("IncomeRequiredMessage", _languageCode, True)
            RFVNoOfYears.ErrorMessage = .Content("NoNoOfYearsErrorMessage", _languageCode, True)
            RVFNoofmonths.ErrorMessage = .Content("MonthsRequiredMessage", _languageCode, True)

            If Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("MobileEnableRFV")) Then
                MobileRFV.Enabled = True
                MobileRFV.ErrorMessage = .Content("MobileErrorMessage", _languageCode, True)
            Else
                MobileRFV.Enabled = False
            End If

            If Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("CountyEnableRFV")) Then
                CountyRFV.Enabled = True
                CountyRFV.ErrorMessage = .Content("CountyErrorMessage", _languageCode, True)
            Else
                CountyRFV.Enabled = False
            End If



        End With
    End Sub

    Private Sub LoadDDL_HomeStatus()

        Dim err As New Talent.Common.ErrorObj
        Dim dt As New DataTable
        Dim utilitites As New Talent.Common.TalentUtiltities
        utilitites.DescriptionKey = "HOMS"
        utilitites.Settings = Utilities.GetSettingsObject()

        err = utilitites.RetrieveDescriptionEntries()
        If utilitites.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
        Else
            dt = utilitites.ResultDataSet.Tables("DescriptionsEntries")
        End If

        ddlHomeStatus.Items.Clear()
        Dim liBlank As New ListItem(" ")
        ddlHomeStatus.Items.Add(liBlank)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            AddDDLOption(ddlHomeStatus, ucr.Content("PleaseSelectOptionText", _languageCode, True), "-1")
            dt.DefaultView.Sort = "DESCRIPTION ASC"
            dt = dt.DefaultView.ToTable
            For Each row As DataRow In dt.Rows
                AddDDLOption(ddlHomeStatus, row("DESCRIPTION"), row("CODE"))
            Next
        End If

    End Sub
    Private Sub LoadDDL_Employment()

        Dim err As New Talent.Common.ErrorObj
        Dim dt As New DataTable
        Dim utilitites As New Talent.Common.TalentUtiltities
        utilitites.DescriptionKey = "EMPS"
        utilitites.Settings = Utilities.GetSettingsObject()

        err = utilitites.RetrieveDescriptionEntries()
        If utilitites.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
        Else
            dt = utilitites.ResultDataSet.Tables("DescriptionsEntries")
        End If

        ddlEmployment.Items.Clear()
        Dim liBlank As New ListItem(" ")
        ddlEmployment.Items.Add(liBlank)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            AddDDLOption(ddlHomeStatus, ucr.Content("PleaseSelectOptionText", _languageCode, True), "-1")
            dt.DefaultView.Sort = "DESCRIPTION ASC"
            dt = dt.DefaultView.ToTable
            For Each row As DataRow In dt.Rows
                AddDDLOption(ddlEmployment, row("DESCRIPTION"), row("CODE"))
            Next
        End If

    End Sub
    Private Sub AddDDLOption(ByRef ddl As DropDownList, ByVal optionText As String, ByVal optionValue As String)
        If Not String.IsNullOrEmpty(optionText) Then
            Dim lstitem As New ListItem
            lstitem.Text = optionText.Trim
            lstitem.Value = optionValue.Trim
            ddl.Items.Add(lstitem)
            lstitem = Nothing
        End If
    End Sub

    Public Function ValidateUserInput() As Boolean

        Dim err As Boolean = False
        ErrorList.Items.Clear()

        If String.IsNullOrEmpty(AccountName.Text) Then
            ErrorList.Items.Add(ucr.Content("NoAccountNameErrorMessage", _languageCode, True))
            err = True
        End If
        If String.IsNullOrEmpty(SortCode1.Text) Or String.IsNullOrEmpty(SortCode2.Text) Or String.IsNullOrEmpty(SortCode3.Text) Then
            ErrorList.Items.Add(ucr.Content("NoSortCodeErrorMessage", _languageCode, True))
            err = True
        End If
        If String.IsNullOrEmpty(AccountNumber.Text) Then
            ErrorList.Items.Add(ucr.Content("NoAccountNumberErrorMessage", _languageCode, True))
            err = True
        End If
        ' Already handled by RFV
        'If String.IsNullOrEmpty(Mobile.Text) Then
        '    ErrorList.Items.Add(ucr.Content("MobileErrorMessage", _languageCode, True))
        '    err = True
        ' End If
        If String.IsNullOrEmpty(County.Text) Then
            ErrorList.Items.Add(ucr.Content("CountyErrorMessage", _languageCode, True))
            err = True
        End If

        Dim errorCode As String = "0"
        Dim sortCode As String = SortCode1.Text & SortCode2.Text & SortCode3.Text

        BankName = String.Empty
        Dim utility As New Talent.Common.Utilities
        Dim accountisKey As String = ucr.Attribute("AccountisKey")
        Dim accountisUrl As String = ucr.Attribute("AccountisUrl")
        If Not err = True Then
            If ucr.Attribute("AccountValidationMethod") = "Accountis" AndAlso accountisKey <> String.Empty Then

                errorCode = utility.ValidateBankAccountWithAccountis(sortCode, AccountNumber.Text, accountisKey, accountisUrl, BankName)
                BankName = BankName.Replace("""", "")

                Select Case errorCode
                    Case Is = "1"
                        ErrorList.Items.Add(ucr.Content("InvalidSortCodeErrorMessage", _languageCode, True))
                        err = True
                    Case Is = "2"
                        ErrorList.Items.Add(ucr.Content("InvalidAccountNumberErrorMessage", _languageCode, True))
                        err = True
                    Case Is = "3"
                        ErrorList.Items.Add(ucr.Content("NoLicenceErrorMessage", _languageCode, True))
                        err = True
                    Case Is = "4"
                        ErrorList.Items.Add(ucr.Content("NoCommsErrorMessage", _languageCode, True))
                        err = True
                End Select

            ElseIf ucr.Attribute("AccountValidationMethod") = "PremiumCredit" Then
                'Premium Credit Mode
                Dim premiumCreditUrl As String = ucr.Attribute("PremiumCreditUrl")
                Dim premiumCreditUserName As String = ucr.Attribute("PremiumCreditUserName")
                Dim premiumCreditpassword As String = ucr.Attribute("PremiumCreditPassword")
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
                            ErrorList.Items.Add(ucr.Content("InvalidSortCodeErrorMessage", _languageCode, True))
                            err = True
                        Case Is = "15"
                            ErrorList.Items.Add(ucr.Content("InvalidAccountNumberErrorMessage", _languageCode, True))
                            err = True
                        Case Is = "2"
                            ErrorList.Items.Add(ucr.Content("NoLicenceErrorMessage", _languageCode, True))
                            err = True
                        Case Is = "999"
                            ErrorList.Items.Add(ucr.Content("NoCommsErrorMessage", _languageCode, True))
                            err = True
                        Case Else
                            ErrorList.Items.Add(ucr.Content("ValidationFailedErrorMessage", _languageCode, True))
                            err = True
                    End Select
                End If
            End If
        End If

        Return Not err
    End Function

    Protected Sub GetInstallmentPlanDetails()

        Dim err As New Talent.Common.ErrorObj
        '
        ' Get the credit Finance options
        '
        Dim cf As New Talent.Common.TalentCreditFinance
        Dim deCreditFinance As New Talent.Common.DECreditFinance
        With deCreditFinance
            .CreditFinanceCompanyCode = def.CreditFinanceCompanyCode
            .OnlyAvailablePlan = Me.onlyAvailablePlan
        End With

        cf.De = deCreditFinance
        cf.Settings = Utilities.GetSettingsObject()
        cf.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        err = cf.CreditFinanceOptions()
        '
        ' Load the drop down list
        '
        If InstallmentPlanDropDownList.Items.Count <= 0 Then
            For Each r As DataRow In cf.ResultDataSet.Tables(0).Rows
                Dim li As New ListItem(r("ShortDescription"), r("PaymentOptionCode"))
                InstallmentPlanDropDownList.Items.Add(li)
            Next
        End If
        '
        ' Display the example plans
        '
        repExample.DataSource = cf.ResultDataSet.Tables(0)
        repExample.DataBind()

    End Sub

    Protected Sub repExample_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles repExample.ItemDataBound
        Const HTML As String = "<span class='cf_name'>{0}</span>{1}"
        If (e.Item.ItemType = ListItemType.Item) Or (e.Item.ItemType = ListItemType.AlternatingItem) Then
            Dim dr As DataRow = TryCast(e.Item.DataItem, DataRowView).Row
            Dim lblExample As Literal = TryCast(e.Item.FindControl("lblExample"), Literal)
            lblExample.Text = String.Format(HTML, dr("ShortDescription").ToString(), dr("Example").ToString())
        End If
    End Sub

    Public Sub ValidateSortCode(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        e.IsValid = (SortCode1Box.Text.Length > 0 AndAlso SortCode2Box.Text.Length > 0 AndAlso SortCode1Box.Text.Length > 0)
    End Sub

End Class
