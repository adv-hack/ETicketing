Imports System.Data
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_OnAccountDetails
    Inherits ControlBase

#Region "Public Properties"

    Public Property Display() As Boolean = True
    Public Property CheckoutSummary() As Boolean = False
    Public ReadOnly Property OnAccountDetailsTotal() As Decimal
        Get
            Return CType(Me.hdfOnAccountDetailsTotal.Value, Decimal)
        End Get
    End Property
    Public ReadOnly Property OnAccountDetailsOptionsChanged() As Boolean
        Get
            Return _OnAccountDetailsChanged
        End Get
    End Property

#End Region

#Region "Class Level Fields"

    Private _OnAccountTotal As Decimal = 0
    Private ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private log As Talent.Common.TalentLogging = Utilities.TalentLogging
    Private _OnAccountDetailsChanged As Boolean = False
    Private ltlRefund As String
    Private ltlSpend As String
    Private ltlReward As String
    Private ltlAdjustment As String
    Private ltlSeatTransfer As String
    Private ltlReprice As String
    Private ltlSeatCancel As String
    Private ltlSaleCancel As String
    Private ltlBuyBackReward As String
    Private ltlVoucherRefund As String
    Private ltlVoucherExchange As String
    Private ltlGiftVoucher As String
    Private ltlPayment As String
    Private ltlPartPayment As String
    Private TotalBalance As Decimal
    Private RefundableBalance As Decimal
    Private ltlRefundToCard As String
    Private ltlTicketExchangeCredit As String
#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'Check if agent has access on OnAccount menu item
        If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessOnaccount) Or Not AgentProfile.IsAgent Then
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "OnAccountDetails.ascx"
            End With
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.plhOnAccountDetails.Visible = True
            SetText()
            rptOnAccountDetails.DataSource = GetOnAccountDetailsData()
            rptOnAccountDetails.DataBind()
        End If
    End Sub

    Protected Sub SetText()
        With ucr
            Me.ltlTitle.Text = .Content("ltlOnAccountDetailsTitleText", _languageCode, True)
            ltlRefund = .Content("ltlRefund", _languageCode, True)
            ltlSpend = .Content("ltlSpend", _languageCode, True)
            ltlReward = .Content("ltlReward", _languageCode, True)
            ltlAdjustment = .Content("ltlAdjustment", _languageCode, True)
            ltlSeatTransfer = .Content("ltlSeatTransfer", _languageCode, True)
            ltlReprice = .Content("ltlReprice", _languageCode, True)
            ltlSeatCancel = .Content("ltlSeatCancel", _languageCode, True)
            ltlSeatTransfer = .Content("ltlSeatTransfer", _languageCode, True)
            ltlSaleCancel = .Content("ltlSaleCancel", _languageCode, True)
            ltlBuyBackReward = .Content("ltlBuyBackReward", _languageCode, True)
            ltlTicketExchangeCredit = .Content("ltlTicketExchangeCredit", _languageCode, True)
            ltlVoucherRefund = .Content("ltlVoucherRefund", _languageCode, True)
            ltlVoucherExchange = .Content("ltlVoucherExchange", _languageCode, True)
            ltlGiftVoucher = .Content("ltlGiftVoucher", _languageCode, True)
            ltlPayment = .Content("ltlPayment", _languageCode, True)
            ltlPartPayment = .Content("ltlPartPayment", _languageCode, True)
            ltlRefundToCard = .Content("ltlrefundtocard", _languageCode, True)
            btnPWSRefundToCard.Visible = False
            ' Agents have options of Manual adjustment and refund to card. Both go to OnAccountAdjustment.aspx
            ' PWS only has refund to card option 
            ' Backend table md221 controls what onacount types (if any) may be refunded (separate flags for PWS and boxoffice
            ' this controls if the refund to card shows so dont need a frontend setting
            btnManualAdjustment.Visible = False
            RetrieveOnAccountBalances(TotalBalance, RefundableBalance)
            If RefundableBalance > 0 Then
                btnPWSRefundToCard.Visible = True
                btnPWSRefundToCard.Text = .Content("btnRefundToCard", _languageCode, True)
            End If
            btnManualAdjustment.Visible = False
            If AgentProfile.IsAgent Then
                btnManualAdjustment.Text = .Content("btnManualAdjustment", _languageCode, True)
                btnManualAdjustment.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("AllowManualAdjustment"))
            End If
        End With
    End Sub

    Protected Function SetText(ByVal sKey As String) As String
        Dim ret As String = String.Empty
        ret = ucr.Content(sKey, _languageCode, True)
        Return ret
    End Function

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        If Not Page.IsPostBack Then
            Dim key As String = CType(sender, Literal).ID & "Text"
            CType(sender, Literal).Text = ucr.Content(key, _languageCode, True)
        End If
    End Sub

    Protected Sub GetTotal(ByVal sender As Object, ByVal e As EventArgs)
        CType(sender, Literal).Text = OnAccountDetailsTotal
    End Sub

    Protected Sub rptOnAccountDetails_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptOnAccountDetails.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then

            Dim myDate As String = CType(e.Item.FindControl("lblDate"), Label).Text
            CType(e.Item.FindControl("lblDate"), Label).Text = Talent.Common.Utilities.ISeriesDate(myDate).ToString("dd/MM/yy")

            Dim myAmount As String = CType(e.Item.FindControl("lblAmount"), Label).Text
            CType(e.Item.FindControl("lblAmount"), Label).Text = TDataObjects.PaymentSettings.FormatCurrency(myAmount, ucr.BusinessUnit, ucr.PartnerCode)

            Dim myActivityType As String = CType(e.Item.FindControl("lblActivityType"), Label).Text
            If myActivityType = "RO" Then CType(e.Item.FindControl("lblActivityType"), Label).Text = ltlRefund
            If myActivityType = "SP" Then CType(e.Item.FindControl("lblActivityType"), Label).Text = ltlSpend
            If myActivityType = "RW" Then CType(e.Item.FindControl("lblActivityType"), Label).Text = ltlReward
            If myActivityType = "AJ" Then CType(e.Item.FindControl("lblActivityType"), Label).Text = ltlAdjustment
            If myActivityType = "BB" Then CType(e.Item.FindControl("lblActivityType"), Label).Text = ltlBuyBackReward
            If myActivityType = "TE" Then CType(e.Item.FindControl("lblActivityType"), Label).Text = ltlTicketExchangeCredit
            If myActivityType = "VR" Then CType(e.Item.FindControl("lblActivityType"), Label).Text = ltlVoucherRefund
            If myActivityType = "VE" Then CType(e.Item.FindControl("lblActivityType"), Label).Text = ltlVoucherExchange
            If myActivityType = "VG" Then CType(e.Item.FindControl("lblActivityType"), Label).Text = ltlGiftVoucher
            If myActivityType = "PY" Then CType(e.Item.FindControl("lblActivityType"), Label).Text = ltlPayment
            If myActivityType = "RC" Then CType(e.Item.FindControl("lblActivityType"), Label).Text = ltlrefundtocard

            Dim myRefundFrom As String = CType(e.Item.FindControl("lblRefundFrom"), Label).Text
            If myRefundFrom = "ST" Then CType(e.Item.FindControl("lblRefundFrom"), Label).Text = ltlSeatTransfer
            If myRefundFrom = "RP" Then CType(e.Item.FindControl("lblRefundFrom"), Label).Text = ltlReprice
            If myRefundFrom = "SC" Then CType(e.Item.FindControl("lblRefundFrom"), Label).Text = ltlSeatCancel
            If myRefundFrom = "FC" Then CType(e.Item.FindControl("lblRefundFrom"), Label).Text = ltlSaleCancel
            If myRefundFrom = "PP " Then CType(e.Item.FindControl("lblRefundFrom"), Label).Text = ltlPartPayment

            Dim myRunningBalance As String = CType(e.Item.FindControl("lblRunningBalance"), Label).Text
            CType(e.Item.FindControl("lblRunningBalance"), Label).Text = TDataObjects.PaymentSettings.FormatCurrency(myRunningBalance, ucr.BusinessUnit, ucr.PartnerCode)
        End If
    End Sub

    Protected Sub btnManualAdjustment_Click(ByVal sender As Object, ByVal e As EventArgs)
        Response.Redirect("~/PagesLogin/Profile/OnAccountAdjustment.aspx?Mode=ManualAdjustment")
    End Sub
    Protected Sub btnPWSRefundToCard_Click(ByVal sender As Object, ByVal e As EventArgs)
        Response.Redirect("~/PagesLogin/Profile/OnAccountAdjustment.aspx?Mode=RefundToCard")
    End Sub
#End Region

#Region "Public Functions"

    Public Function GetOnAccountDetailsData() As DataTable

        'Create the results data table
        Dim dtOnAccountDetails As New DataTable
        _OnAccountTotal = 0
        Dim err As New Talent.Common.ErrorObj
        '
        ' Retrieve the total credit on the e-purse
        '
        Dim tp As New Talent.Common.TalentPayment
        Dim settings As Talent.Common.DESettings = Utilities.GetSettingsObject()
        Dim dePayment As New Talent.Common.DEPayments
        With dePayment
            .SessionId = Profile.Basket.Basket_Header_ID.ToString
            If Not Profile.User.Details Is Nothing Then
                .CustomerNumber = Profile.User.Details.LoginID
            End If
            .Source = GlobalConstants.SOURCE
        End With
        '

        ' Call function to return dataset of on-acccount details  
        tp.Settings = settings
        tp.De = dePayment
        err = tp.RetrieveOnAccountDetails()


        ' Was the call successful
        If Not err.HasError AndAlso Not tp.ResultDataSet Is Nothing AndAlso tp.ResultDataSet.Tables.Count = 2 AndAlso _
            tp.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then

            'Populate the new data table
            dtOnAccountDetails = tp.ResultDataSet.Tables("OnAccountDetails")

        End If
        Me.hdfOnAccountDetailsTotal.Value = _OnAccountTotal.ToString

        Return dtOnAccountDetails
    End Function

#End Region

#Region "Public Methods"

    Public Sub ReBindOnAccountDetails()
        Me.plhOnAccountDetails.Visible = True
        rptOnAccountDetails.DataSource = GetOnAccountDetailsData()
        rptOnAccountDetails.DataBind()
    End Sub

#End Region

    Private Sub RetrieveOnAccountBalances(ByRef TotalBalance As Decimal, ByRef RefundableBalance As Decimal)
        Const ModuleName As String = "RetrieveOnAccountDetails"
        Dim err As New Talent.Common.ErrorObj
        Dim tp As New Talent.Common.TalentPayment
        tp.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        tp.Settings.ModuleName = ModuleName
        tp.De.CustomerNumber = Profile.User.Details.LoginID
        tp.De.AgentName = AgentProfile.Name
        tp.De.CashbackMode = "R"
        tp.De.Source = GlobalConstants.SOURCE
        err = tp.RetrieveOnAccountDetails
        If Not err.HasError AndAlso Not tp.ResultDataSet Is Nothing AndAlso tp.ResultDataSet.Tables.Count = 2 AndAlso _
          tp.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
            TotalBalance = Talent.Common.Utilities.CheckForDBNull_Decimal(tp.ResultDataSet.Tables("dtStatusResults").Rows(0)("TotalBalance"))
            RefundableBalance = Talent.Common.Utilities.CheckForDBNull_Decimal(tp.ResultDataSet.Tables("dtStatusResults").Rows(0)("RefundableBalance"))
        End If

    End Sub

End Class