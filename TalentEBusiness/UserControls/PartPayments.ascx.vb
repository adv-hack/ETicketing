Imports System.Data
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_PartPayments
    Inherits ControlBase

#Region "Public Properties"

    Public Property Display() As Boolean = True
    Public Property CheckoutSummary() As Boolean = False
    Public ReadOnly Property PartPaymentTotal() As Decimal
        Get
            Return CType(Me.hdfOnAccountTotal.Value, Decimal)
        End Get
    End Property
    Public ReadOnly Property PartPaymentOptionsChanged() As Boolean
        Get
            Return _partPaymentOptionsChanged
        End Get
    End Property
    Public Property AccountHeader() As String
    Public Property AmountHeader() As String
    Public Property OptionsHeader() As String
    Public Property RemovePartPaymentAllowed() As Boolean = False

#End Region

#Region "Class Level Fields"

    Private _OnAccountTotal As Decimal = 0
    Private ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private log As Talent.Common.TalentLogging = Utilities.TalentLogging
    Private _partPaymentOptionsChanged As Boolean = False

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PartPayments.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Display AndAlso ModuleDefaults.OnAccountEnabled Then
            If Not IsPostBack Then

                'Display the relevant screen
                Me.plhOnAccount.Visible = False
                Me.plhOnAccountSummary.Visible = False
                If CheckoutSummary Then
                    Me.plhOnAccountSummary.Visible = True
                Else
                    Me.plhOnAccount.Visible = True
                End If

                'Set up the screen fields
                SetText()
                SetFields()

                'Populate the repeater
                If CheckoutSummary Then
                    rptOnAccountSummary.DataSource = GetOnAccountData()
                    rptOnAccountSummary.DataBind()
                Else
                    rptOnAccount.DataSource = GetOnAccountData()
                    rptOnAccount.DataBind()
                End If

            End If
        Else
            Me.plhOnAccount.Visible = False
            Me.plhOnAccountSummary.Visible = False
            _OnAccountTotal = 0
            hdfOnAccountTotal.Value = _OnAccountTotal.ToString()
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (ErrorList.Items.Count > 0)
    End Sub

    Protected Sub SetText()
        With ucr
            Me.ltlTitle.Text = .Content("ltlOnAccountTitleText", _languageCode, True)
            AccountHeader = .Content("ltlAccountHeaderText", _languageCode, True)
            AmountHeader = .Content("ltlAmountHeaderText", _languageCode, True)
            OptionsHeader = .Content("ltlOptionsHeaderText", _languageCode, True)
        End With
    End Sub

    Protected Sub SetFields()

    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        If Not Page.IsPostBack Then
            Dim key As String = CType(sender, Literal).ID & "Text"
            CType(sender, Literal).Text = ucr.Content(key, _languageCode, True)
        End If
    End Sub

    Protected Sub GetTotal(ByVal sender As Object, ByVal e As EventArgs)
        CType(sender, Literal).Text = PartPaymentTotal
    End Sub

    Protected Sub btnRefund_OnClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim partPaymentRepeaterItem As RepeaterItem = CType(sender, Button).Parent
        Dim err As New Talent.Common.ErrorObj
        '
        ' Retrieve the total credit on the e-purse
        '
        Dim tp As New Talent.Common.TalentPayment
        Dim dePayment As New Talent.Common.DEPayments
        With dePayment
            .SessionId = Profile.Basket.Basket_Header_ID
            .CardNumber = CType(partPaymentRepeaterItem.FindControl("hdfCardNumber"), HiddenField).Value
            .Amount = CType(partPaymentRepeaterItem.FindControl("hdfPaymentAmount"), HiddenField).Value.ToString.Replace(".", "")
            .PartPaymentId = CType(partPaymentRepeaterItem.FindControl("hdfPartPaymentId"), HiddenField).Value
            .PartPaymentApplyTypeFlag = TEBUtilities.GetPartPaymentFlag()
            If Not Profile.User.Details Is Nothing Then
                .CustomerNumber = Profile.User.Details.LoginID
            End If
        End With
        '
        tp.De = dePayment
        tp.Settings = Utilities.GetSettingsObject()
        tp.Settings.Cacheing = ucr.Attribute("Cacheing")
        tp.Settings.CacheTimeMinutes = ucr.Attribute("CacheTimeMinutes")
        err = tp.CancelPartPayment()
        '
        ' Was the call successful
        If Not err.HasError AndAlso _
                Not tp.ResultDataSet Is Nothing AndAlso _
                tp.ResultDataSet.Tables.Count = 2 AndAlso _
                tp.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then

            'Redirect to load the page again and to prevent an F5 refresh submitting the same request
            Session.Add("SelectPaymentTypeOption", CType(partPaymentRepeaterItem.FindControl("hdfPaymentMethod"), HiddenField).Value)
            If Utilities.GetCurrentPageName().ToUpper() = "CHECKOUT.ASPX" Then
                ProcessRefundInCheckoutPage()
            Else
                Response.Redirect(Request.Url.PathAndQuery)
            End If

        End If
    End Sub

    Protected Sub rptOnAccount_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptOnAccount.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Try
                Select Case e.Item.DataItem("PaymentMethod")
                    ' Epurse
                    Case Is = "EP"
                        CType(e.Item.FindControl("ltlAccount"), Literal).Text = e.Item.DataItem("CardNumber")
                        CType(e.Item.FindControl("ltlAmount"), Literal).Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(CType(e.Item.DataItem("PaymentAmount"), Decimal), 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        CType(e.Item.FindControl("btnRefund"), Button).Text = ucr.Content("btnRefundText", _languageCode, True)
                        CType(e.Item.FindControl("hdfPartPaymentId"), HiddenField).Value = e.Item.DataItem("PartPaymentId")
                        CType(e.Item.FindControl("hdfCardNumber"), HiddenField).Value = e.Item.DataItem("CardNumber")
                        CType(e.Item.FindControl("hdfPaymentAmount"), HiddenField).Value = e.Item.DataItem("PaymentAmount")
                        CType(e.Item.FindControl("hdfPaymentMethod"), HiddenField).Value = e.Item.DataItem("PaymentMethod")
                End Select

            Catch ex As Exception
                Me.plhOnAccount.Visible = False
            End Try
        End If
    End Sub

    Protected Sub rptOnAccountSummary_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptOnAccountSummary.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Try

                If String.IsNullOrEmpty(e.Item.DataItem("CardNumber")) Then
                    CType(e.Item.FindControl("ltlAccount1"), Literal).Text = TDataObjects.PaymentSettings.TblPaymentTypeLang.GetDescriptionByTypeAndLang(e.Item.DataItem("PaymentMethod"), _languageCode)
                Else
                    CType(e.Item.FindControl("ltlAccount1"), Literal).Text = TDataObjects.PaymentSettings.TblPaymentTypeLang.GetDescriptionByTypeAndLang(e.Item.DataItem("PaymentMethod"), _languageCode) & "-" & e.Item.DataItem("CardNumber")
                End If

                CType(e.Item.FindControl("ltlAmount1"), Literal).Text = "&ndash;&nbsp;" & TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(CType(e.Item.DataItem("PaymentAmount"), Decimal), 0.01, False), ucr.BusinessUnit, ucr.PartnerCode) & "<span>(" & TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(CType(e.Item.DataItem("FeeValue"), Decimal), 0.01, False), ucr.BusinessUnit, ucr.PartnerCode) & ")</span>"
                CType(e.Item.FindControl("hdfPartPaymentId"), HiddenField).Value = e.Item.DataItem("PartPaymentId")
                CType(e.Item.FindControl("hdfCardNumber"), HiddenField).Value = e.Item.DataItem("CardNumber")
                CType(e.Item.FindControl("hdfPaymentAmount"), HiddenField).Value = e.Item.DataItem("PaymentAmount")
                CType(e.Item.FindControl("hdfPaymentMethod"), HiddenField).Value = e.Item.DataItem("PaymentMethod")
                If Not RemovePartPaymentAllowed Then CType(e.Item.FindControl("lnkButtonForRefund"), LinkButton).Visible = False
            Catch ex As Exception
                Me.plhOnAccountSummary.Visible = False
            End Try
        ElseIf e.Item.ItemType = ListItemType.Header Then
            If _OnAccountTotal > 0 Then
                AccountHeader = ucr.Content("ltlAccountHeader1Text", _languageCode, True)
            Else
                plhOnAccountSummary.Visible = False
            End If
        End If
    End Sub

    Protected Sub rptOnAccountSummary_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptOnAccountSummary.ItemCommand
        If e.CommandName = "ProcessRefund" Then
            Dim err As New Talent.Common.ErrorObj
            Dim TalPayment As New Talent.Common.TalentPayment
            Dim dePayment As New Talent.Common.DEPayments
            TalPayment.Settings = Utilities.GetSettingsObject()

            With dePayment
                .SessionId = Profile.Basket.Basket_Header_ID
                .PaymentType = CType(e.Item.FindControl("hdfPaymentMethod"), HiddenField).Value
                .CardNumber = CType(e.Item.FindControl("hdfCardNumber"), HiddenField).Value
                .Amount = CType(e.Item.FindControl("hdfPaymentAmount"), HiddenField).Value.ToString.Replace(".", "")
                .PartPaymentId = CType(e.Item.FindControl("hdfPartPaymentId"), HiddenField).Value
                .PartPaymentApplyTypeFlag = TEBUtilities.GetPartPaymentFlag()
                If Not Profile.User.Details Is Nothing Then
                    .CustomerNumber = Profile.User.Details.LoginID
                End If
                .PaymentType = GlobalConstants.EPURSEPAYMENTTYPE
            End With
            TalPayment.De = dePayment

            TalPayment.Settings.Cacheing = False
            err = TalPayment.CancelPartPayment()
            '
            ' Was the call successful
            If Not err.HasError AndAlso _
                    Not TalPayment.ResultDataSet Is Nothing AndAlso _
                    TalPayment.ResultDataSet.Tables.Count = 2 AndAlso _
                    TalPayment.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then

                'Redirect to load the page again and to prevent an F5 refresh submitting the same request
                Session.Add("SelectPaymentTypeOption", CType(e.Item.FindControl("hdfPaymentMethod"), HiddenField).Value)
                ProcessRefundInCheckoutPage()
                Response.Redirect(Request.Url.PathAndQuery)
            End If
        End If

    End Sub



#End Region
#Region "Private Methods"
    Private Sub ProcessRefundInCheckoutPage()
        _partPaymentOptionsChanged = True
        Dim talBasketSummary As New Talent.Common.TalentBasketSummary
        talBasketSummary.Settings = TEBUtilities.GetSettingsObject()
        talBasketSummary.LoginID = Profile.UserName
        talBasketSummary.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talBasketSummary.Settings.BusinessUnit)
        talBasketSummary.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talBasketSummary.Settings.BusinessUnit)
        If talBasketSummary.UpdateBasketPayTypeOrCardType(Profile.Basket.Basket_Header_ID, GlobalConstants.CCPAYMENTTYPE, "", Profile.Basket.PAYMENT_TYPE, Profile.Basket.CARD_TYPE_CODE, False, False) Then

        End If
        ReBindPartPayments()
        Dim canProcessbookingfees As Boolean = TEBUtilities.ProcessPartPayment()
        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True, canProcessbookingfees)
    End Sub

#End Region


#Region "Public Functions"

    Public Function GetOnAccountData() As DataTable

        'Create the results data table
        Dim dtPartPayments As New DataTable
        _OnAccountTotal = 0
        Dim err As New Talent.Common.ErrorObj
        '
        ' Retrieve the total credit on the e-purse
        '
        Dim tp As New Talent.Common.TalentPayment
        Dim dePayment As New Talent.Common.DEPayments
        With dePayment
            .SessionId = Profile.Basket.Basket_Header_ID.ToString
            If Not Profile.User.Details Is Nothing Then
                .CustomerNumber = Profile.User.Details.LoginID
            End If
        End With
        '
        tp.De = dePayment
        tp.Settings = Utilities.GetSettingsObject()
        tp.Settings.Cacheing = Utilities.CheckForDBNull_Boolean_DefaultTrue(ucr.Attribute("Cacheing"))
        tp.Settings.CacheTimeMinutes = Utilities.CheckForDBNull_Int(ucr.Attribute("CacheTimeMinutes"))
        err = tp.RetrievePartPayments()

        ' Was the call successful
        If Not err.HasError AndAlso Not tp.ResultDataSet Is Nothing AndAlso tp.ResultDataSet.Tables.Count = 2 AndAlso _
            tp.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then

            'Populate the new data table
            dtPartPayments = tp.ResultDataSet.Tables("PartPayments")

            'Retrieve the part payment totals
            For Each dr As DataRow In dtPartPayments.Rows
                _OnAccountTotal = _OnAccountTotal + CType(dr.Item("PaymentAmount"), Decimal)
            Next
        End If
        Me.hdfOnAccountTotal.Value = _OnAccountTotal.ToString

        Return dtPartPayments
    End Function

#End Region

#Region "Public Methods"

    Public Sub ReBindPartPayments()
        If ModuleDefaults.RetrievePartPayments Then
            Me.plhOnAccount.Visible = False
            Me.plhOnAccountSummary.Visible = False
            If CheckoutSummary Then
                Me.plhOnAccountSummary.Visible = True
            Else
                Me.plhOnAccount.Visible = True
            End If
            _partPaymentOptionsChanged = True
            'Populate the repeater
            If CheckoutSummary Then
                rptOnAccountSummary.DataSource = GetOnAccountData()
                rptOnAccountSummary.DataBind()
            Else
                rptOnAccount.DataSource = GetOnAccountData()
                rptOnAccount.DataBind()
            End If
        End If
    End Sub

#End Region

End Class