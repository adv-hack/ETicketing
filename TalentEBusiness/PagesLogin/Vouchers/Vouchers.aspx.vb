Imports Talent.eCommerce
Imports System.Collections.Generic
Imports System.Text.RegularExpressions
Imports System.Data
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesLogin_Promotions_Vouchers
    Inherits TalentBase01

#Region "Constants"
    Const REDIRECTTOLINK As String = "Book"
    Const EXCHANGE As String = "Exchange"
    Const DELETE As String = "Delete"
    Const NOTCLICKED As String = "No_clicked"
#End Region

#Region "Private Members"
    Private _wfr As Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _agent As New Agent
    Private _vouchers As New Vouchers()
    Private _showExternalVouchers As Boolean
    Private _selectedVoucherScheme As String
    Private _externalVoucherFlag As Boolean
    Private _confirmResult As String
#End Region

#Region "Public Members"
    Public Property UpgradeText() As String
    Public Property ExchangeText() As String
    Public Property DeleteText() As String
    Public Property CustomerNumber() As String

    Public Function IsExchangeContentVisible(ByVal voucherCode As String, ByVal canExchangeToOnAccount As Boolean, ByVal voucherSource As Char, ByVal salesVale As String) As Boolean
        If voucherSource = "E" OrElse salesVale = "0.00" Then Return False
        Dim retValue As Boolean = False
        Try
            retValue = canExchangeToOnAccount
        Catch ex As Exception
            retValue = False
        End Try
        Return retValue
    End Function
#End Region

#Region "Protected Methods"

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        If TypeOf (sender) Is Button Then
            CType(sender, Button).Text = _vouchers.GetText(sender.id)
        ElseIf TypeOf (sender) Is Label Then
            CType(sender, Label).Text = _vouchers.GetText(sender.id)
        ElseIf TypeOf (sender) Is Literal Then
            CType(sender, Literal).Text = _vouchers.GetText(sender.id)
        End If
    End Sub

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _wfr = New Talent.Common.WebFormResource
        If (_agent.IsAgent) Then
            plhExternalVouchers.Visible = True
        Else
            plhExternalVouchers.Visible = False
        End If
    End Sub

    Public Function GetExchangeText(ByVal value As String, ByVal ExchangeText As String) As String
        Return String.Format("{0} {1}", ExchangeText, TDataObjects.PaymentSettings.FormatCurrency(value, _wfr.BusinessUnit, _wfr.PartnerCode))
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .KeyCode = "Vouchers.aspx"
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        plhErrorList.Visible = False
        CustomerNumber = Profile.UserName
        LoadAccountBalance()
        blErrorMessages.Items.Clear()
        Javascript()
        _showExternalVouchers = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("ShowExternalVoucherCodeTextBox"))
        _vouchers.BUSINESSUNIT = _wfr.BusinessUnit
        plhExternalVoucherCode.Visible = _showExternalVouchers
        BindRepeater(True)
        Dim temp As String = AgentProfile.Name
        _confirmResult = Request.Params("__EVENTARGUMENT")
        If Not Page.IsPostBack Then
            BindCompany()
            BindVoucherSchemeAndPrice()
            If Not Request.QueryString("DoRefresh") Is Nothing AndAlso Request.QueryString("DoRefresh") Then
                If (Session("LastVoucherCode") IsNot Nothing) Then
                    VoucherId.Text = Session("LastVoucherCode")
                End If
                If (Session("LastVoucherPrice") IsNot Nothing) Then
                    VoucherAmount.Text = Session("LastVoucherPrice")
                End If
            Else
                Session("AccountTotal") = Nothing
                Session("LastVoucherPrice") = Nothing
                Session("LastVoucherCode") = Nothing
            End If
        Else
            If (Session("AccountTotal") IsNot Nothing) Then
                BalanceAmount.Text = Session("AccountTotal")
            End If
            If (Session("LastVoucherCode") IsNot Nothing) Then
                VoucherId.Text = Session("LastVoucherCode")
            End If
            If (Session("LastVoucherPrice") IsNot Nothing) Then
                VoucherAmount.Text = Session("LastVoucherPrice")
            End If
        End If
        plhRecentlyAdded.Visible = (VoucherId.Text.Length > 0 OrElse VoucherAmount.Text.Length > 0)
    End Sub

    Protected Sub ddlcompany_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlcompany.SelectedIndexChanged
        ddlVoucherScheme.DataSource = _vouchers.GetVoucherScheme(CustomerNumber, ddlcompany.SelectedValue, False)
        ddlVoucherScheme.DataTextField = "Description"
        ddlVoucherScheme.DataValueField = "VoucherId"
        ddlVoucherScheme.DataBind()
        Dim priceList As New List(Of KeyValuePair(Of String, String))
        ddlPrice.DataSource = _vouchers.GetVoucherPrice(CustomerNumber, ddlVoucherScheme.SelectedValue, ddlcompany.SelectedValue, False)
        ddlPrice.DataTextField = "Key"
        ddlPrice.DataValueField = "Value"
        ddlPrice.DataBind()
    End Sub

    Protected Sub ddlVoucherScheme_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlVoucherScheme.SelectedIndexChanged
        ddlPrice.DataSource = _vouchers.GetVoucherPrice(CustomerNumber, ddlVoucherScheme.SelectedValue, ddlcompany.SelectedValue, False)
        ddlPrice.DataTextField = "Key"
        ddlPrice.DataValueField = "Value"
        ddlPrice.DataBind()
    End Sub

    Protected Sub btnAddVoucher_Click(sender As Object, e As System.EventArgs) Handles btnAddVoucher.Click
        Dim VoucherCode As String = txtVoucherCode.Text.Trim()
        _externalVoucherFlag = False
        If Not _confirmResult.Equals(NOTCLICKED) Then
            If (Not Regex.IsMatch(VoucherCode, _wfr.Attribute("InvalidVoucherCode"))) Then
                plhErrorList.Visible = True
                blErrorMessages.Items.Clear()
                blErrorMessages.Items.Add(_wfr.Content("InvalidVoucherText", _languageCode, False))
                HandleFailedAttempts()
            Else
                If Not RedeemOrDeleteOrConvertVoucher(VoucherCode, Talent.Common.RedeemMode.Redeem) Then
                    HandleFailedAttempts()
                Else
                    Session("NoOfAttempts") = 0
                End If
            End If
            If Not plhErrorList.Visible Then
                If Not Request.QueryString("DoRefresh") Is Nothing AndAlso Request.QueryString("DoRefresh") Then
                    Response.Redirect(Request.RawUrl)
                Else
                    Response.Redirect(Request.RawUrl + "?DoRefresh=True")
                End If
            End If
        End If
    End Sub

    Protected Sub btnAddExternalvouchers_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddExternalvouchers.Click
        If Not _confirmResult.Equals(NOTCLICKED) Then
            _externalVoucherFlag = True
            Dim AccountTotal As String = String.Empty
            Dim priceAgreement As String() = ddlPrice.SelectedValue.Split(",")
            Dim price As String = priceAgreement(0)
            Dim agreementCode As String = String.Empty
            Dim characterLimitError As Boolean = False
            If Not String.IsNullOrEmpty(priceAgreement(0).ToString) Then agreementCode = priceAgreement(1)
            If _vouchers.RedeemExternalVoucher(ddlcompany.SelectedValue, CustomerNumber, price, _agent.Name, ddlVoucherScheme.SelectedValue, AccountTotal, txtExternalVoucherCode.Text, _showExternalVouchers, agreementCode, characterLimitError) Then
                Session("AccountTotal") = AccountTotal
                BalanceAmount.Text = Session("AccountTotal")
                Response.Redirect(Request.RawUrl)
            Else
                If characterLimitError Then
                    blErrorMessages.Items.Add(_wfr.Content("VoucherCodeLengthErrorText", _languageCode, False))
                Else
                    blErrorMessages.Items.Add(_wfr.Content("VoucherAddErrorText", _languageCode, False))
                End If

                plhErrorList.Visible = True
                BindRepeater(True)  'Rebind from Session as viewstate is disabled.
            End If
        End If
    End Sub

    Protected Sub rptExperiences_ItemCommand(source As Object, e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptExperiences.ItemCommand
        Dim AccountTotal As String = String.Empty
        Dim UniqueVoucherId As String = e.CommandArgument.ToString()
        If e.CommandName = EXCHANGE Then
            RedeemOrDeleteOrConvertVoucher(UniqueVoucherId, Talent.Common.RedeemMode.Convert)
        ElseIf e.CommandName = DELETE Then
            RedeemOrDeleteOrConvertVoucher(UniqueVoucherId, Talent.Common.RedeemMode.Delete)
            If plhErrorList.Visible Then
            Else
                If Not Request.QueryString("DoRefresh") Is Nothing AndAlso Request.QueryString("DoRefresh") Then
                    Response.Redirect(Request.RawUrl)
                Else
                    Response.Redirect(Request.RawUrl + "?DoRefresh=True")
                End If
            End If
        End If
    End Sub

    Protected Sub rptExperiences_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptExperiences.ItemDataBound
        Dim item As RepeaterItem = e.Item
        Dim drv As DataRowView
        Dim rptLinks As Repeater
        drv = DirectCast(item.DataItem, DataRowView)
        rptLinks = item.FindControl("rptLinks")
        Dim VoucherId As Integer = drv.Item("VoucherId")
        rptLinks.DataSource = _vouchers.GetLinkDetails(VoucherId)
        rptLinks.DataBind()
    End Sub

    Protected Sub rptLinks_ItemCommand(source As Object, e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
        txtVoucherCode.Text = String.Empty
        Response.Redirect(ResolveUrl(e.CommandArgument.ToString()))
    End Sub

    Protected Function IsVisible(ByVal value As String) As Boolean
        Dim retvalue As Boolean = True
        If (String.IsNullOrEmpty(value)) Then
            retvalue = False
        End If
        Return retvalue
    End Function

    Protected Function GetFormattedDate(ByVal value As Date) As String
        Return value.ToString("d")
    End Function

#End Region

#Region "Private Methods"

    Private Sub HandleFailedAttempts()
        If AgentProfile.IsAgent Then
            Session("NoOfAttempts") = 0
        Else
            If Session("NoOfAttempts") Is Nothing Then
                Session("NoOfAttempts") = 0
            End If
            Session("NoOfAttempts") = Integer.Parse(Session("NoOfAttempts").ToString()) + 1
            If Integer.Parse(Session("NoOfAttempts")) = 2 Then
                blErrorMessages.Items.Clear()
                blErrorMessages.Items.Add("This is your last attempt. If it is incorrect, you would be logged out.")
            ElseIf Integer.Parse(Session("NoOfAttempts")) > 2 Then
                Session("NoOfAttempts") = 0
                SendEmail()
            End If
        End If       
    End Sub
    Private Sub SendEmail()
        Dim deCustomerDetailsV11 As New Talent.Common.DECustomerV11
        Dim deCustomerDetails As New Generic.List(Of Talent.Common.DECustomer)
        deCustomerDetails.Add(New Talent.Common.DECustomer)
        deCustomerDetails(0).CustomerNumber = Profile.User.Details.LoginID
        deCustomerDetails(0).SingleFieldMode = Talent.Common.SingleModeFieldsEnum.HOLD20
        deCustomerDetailsV11.DECustomersV1 = deCustomerDetails
        Dim err As New Talent.Common.ErrorObj
        Dim talentCust As New Talent.Common.TalentCustomer
        With talentCust
            .DeV11 = deCustomerDetailsV11
            .Settings = Talent.eCommerce.Utilities.GetSettingsObject
            err = .UpdateCustomerDetailsSingleMode()
        End With
        Talent.Common.Utilities.Email_Send(ConfigurationManager.AppSettings("ErrorFromEmail"),
                                   Profile.User.Details.Email, ConfigurationManager.AppSettings("ErrorToEmail"), String.Empty,
                                   ConfigurationManager.AppSettings("ErrorSubjectField") & " Wrong Voucher code is entered 3 times",
                                   String.Format("Business Unit: {1} {0} Partner: {2} {0} Login ID: {3} {0} Error Number: {4} {0} Error Message: {5} {0} Error Status: {6} {0}", _
                                                                                            vbCrLf, _
                                                                                            TalentCache.GetBusinessUnit, _
                                                                                            TalentCache.GetPartner(Profile), _
                                                                                            Profile.UserName, _
                                                                                            err.ErrorNumber, _
                                                                                            err.ErrorMessage, _
                                                                                            err.ErrorStatus))
        Response.Redirect("~/PagesPublic/Login/LoggedOut.aspx")

    End Sub

    Private Function RedeemOrDeleteOrConvertVoucher(ByVal VoucherCodeOrUniqueId As String, ByVal RedeemMode As Talent.Common.RedeemMode) As Boolean
        Dim VoucherPrice As String = String.Empty
        Dim AccountTotal As String = String.Empty
        Dim VoucherCodeReturned As String = String.Empty
        Dim ReturnCode As String = String.Empty
        If RedeemMode = Talent.Common.RedeemMode.Delete Then
            If _vouchers.DeleteVoucher(VoucherCodeOrUniqueId, CustomerNumber, AccountTotal) Then
                Session("AccountTotal") = AccountTotal
                BalanceAmount.Text = Session("AccountTotal")
                VoucherId.Text = String.Empty
                VoucherAmount.Text = String.Empty
            Else
                blErrorMessages.Items.Add(_wfr.Content("VoucherDeleteErrorText", _languageCode, False))
                plhErrorList.Visible = True
                Return False
            End If
        ElseIf _vouchers.RedeemGiftVoucherOrConvert(VoucherCodeOrUniqueId, CustomerNumber, RedeemMode, VoucherPrice, AccountTotal, VoucherCodeReturned, ReturnCode) Then
            Session("AccountTotal") = AccountTotal
            BalanceAmount.Text = Session("AccountTotal")
            Session("LastVoucherCode") = VoucherCodeReturned
            VoucherId.Text = Session("LastVoucherCode")
            Session("LastVoucherPrice") = VoucherPrice
            VoucherAmount.Text = Session("LastVoucherPrice")
            txtVoucherCode.Text = String.Empty
        Else
            Dim success As Boolean = True
            If (RedeemMode = Talent.Common.RedeemMode.Convert) Then
                blErrorMessages.Items.Add(_wfr.Content("VoucherConvertErrorText", _languageCode, False))
            Else
                If ReturnCode = "US" Then
                    blErrorMessages.Items.Add(_wfr.Content("VoucherAlreadyUsedErrorText", _languageCode, False))
                    success = False
                ElseIf ReturnCode = "VD" Then
                    blErrorMessages.Items.Add(_wfr.Content("VoucherInvalidErrorText", _languageCode, False))
                    success = False
                Else
                    blErrorMessages.Items.Add(_wfr.Content("VoucherAddErrorText", _languageCode, False))
                End If
            End If
            plhErrorList.Visible = True
            BindRepeater(True)  'Rebind from Session as viewstate is disabled.
            Return success
        End If
        BindRepeater(False)
        Return True
    End Function

    Private Sub BindCompany()
        ddlcompany.DataSource = _vouchers.GetCompanyDetails()
        ddlcompany.DataBind()
    End Sub

    Private Sub BindVoucherSchemeAndPrice()
        ddlVoucherScheme.DataSource = _vouchers.GetVoucherScheme(CustomerNumber, ddlcompany.SelectedValue, False)
        ddlVoucherScheme.DataTextField = "Description"
        ddlVoucherScheme.DataValueField = "VoucherId"
        ddlVoucherScheme.DataBind()
        If ddlVoucherScheme.Items.Count > 0 Then
            ddlPrice.DataSource = _vouchers.GetVoucherPrice(CustomerNumber, ddlVoucherScheme.SelectedValue, ddlcompany.SelectedValue, False)
            ddlPrice.DataTextField = "Key"
            ddlPrice.DataValueField = "Value"
            ddlPrice.DataBind()
        End If

    End Sub

    Private Sub BindRepeater(ByVal BindFromSession As Boolean)
        Dim CustomerInformation As New DataTable
        UpgradeText = _vouchers.GetText("UpgradeText")
        If Not BindFromSession OrElse Not Page.IsPostBack Then
            CustomerInformation = _vouchers.GetCompleteVoucherInformation(CustomerNumber, TEBUtilities.GetOriginatingSource(Session.Item("Agent")))
            Session("CustomerInformation") = CustomerInformation
        Else
            CustomerInformation = Session("CustomerInformation")
        End If
        If Not CustomerInformation Is Nothing Then
            If CustomerInformation.Rows.Count > 0 Then
                rptExperiences.DataSource = CustomerInformation
                rptExperiences.DataBind()
                plhRepeater.Visible = True
            Else
                plhRepeater.Visible = False
            End If
        End If
        
    End Sub

    Private Sub LoadAccountBalance()
        Dim AccountBalance As String = _vouchers.GetCustomerAccountBalance(CustomerNumber)
        BalanceAmount.Text = AccountBalance
    End Sub

    Private Sub Javascript()
        Dim ConfirmVoucherEntryText As String = _wfr.Content("ConfirmVoucherEntryText", _languageCode, True)
        If AgentProfile.IsAgent Then
            If Profile.User.Details IsNot Nothing Then
                ConfirmVoucherEntryText = ConfirmVoucherEntryText.Replace("<<CUSTOMER>>", CustomerNumber.TrimStart("0") & "/" & Profile.User.Details.Full_Name)
            Else
                ConfirmVoucherEntryText = ConfirmVoucherEntryText.Replace("<<CUSTOMER>>", CustomerNumber)
            End If
        End If
        Me.btnAddVoucher.Attributes.Add("onclick", "confirmButton();")
        Me.btnAddExternalvouchers.Attributes.Add("onclick", "confirmButton();")
        Dim DoURLOptionJS As New StringBuilder
        DoURLOptionJS.AppendLine("<script language=""javascript"" type=""text/javascript"">")
        DoURLOptionJS.AppendLine("  function confirmButton() {")
        DoURLOptionJS.AppendLine("      var txt = document.getElementById('txtVoucherCode').value;")
        DoURLOptionJS.AppendLine("      if(txt  != '') {")
        DoURLOptionJS.AppendLine("          if(confirm('" & ConfirmVoucherEntryText & "')){")
        DoURLOptionJS.AppendLine("          }else{")
        DoURLOptionJS.AppendLine("            __doPostBack('', 'No_clicked');")
        DoURLOptionJS.AppendLine("          }")
        DoURLOptionJS.AppendLine("      }")
        DoURLOptionJS.AppendLine("}")
        DoURLOptionJS.AppendLine("</script>")
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "ConfirmButton", DoURLOptionJS.ToString())
        DoURLOptionJS = Nothing
    End Sub

#End Region

End Class
