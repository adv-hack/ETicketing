Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.CATHelper
Imports Talent.eCommerce.Utilities
Imports System.Xml
Imports System.Globalization
Imports System.Threading
Imports DataSetHelperOrder

Partial Class UserControls_OrderEnquiry2
    Inherits ControlBase

    Public Property TransactionHistoryMode() As Boolean

#Region "Class Level Fields"

    Const _cProductTable = 0
    Const ProductTypeNoProductSelected As String = "--"

    Private _ucr As New Talent.Common.UserControlResource
    Private _businessUnit As String = String.Empty
    Private _partnerCode As String = String.Empty
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _allowCancelAll As Boolean = False
    Private _allowCancel As Boolean = False
    Private _allowTransfer As Boolean = False
    Private _allowAmend As Boolean = False
    Private _isTransactionEnquiry As Boolean = False
    Private _canDisplayCancelAllButton As Boolean = False
    Private _displayCancelAllMsg As Boolean = False
    Private _allowPrint As Boolean = False
    Private _isTicketCollection As Boolean = False
    Private _displayTextTransfer As String = String.Empty
    Private _displayTextAmend As String = String.Empty
    Private _displayTextCancel As String = String.Empty
    Private _displayTextPrint As String = String.Empty
    Private _isBasketHasItems As String = String.Empty
    Private CPage As New Label
    Private CurPage, CurRec, I, totLinks As Integer
    Private vPerpage, vTotalRec, vTotalPages As Int16
    Private objPds As New PagedDataSource()
    Private _dsPurchaseHistory As New DataSet()
    Private _totalRecords, _lastRecordNumber As Integer
    Private _canMergeData As Boolean = False

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _businessUnit = TalentCache.GetBusinessUnit()
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        With _ucr
            .BusinessUnit = _businessUnit
            .PageCode = String.Empty
            .PartnerCode = _partnerCode
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "OrderEnquiry2.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Visible Then
            InitialiseControlsAndFlag()
            SetPagerValues()
            plhCancelAll.Visible = (_canDisplayCancelAllButton AndAlso _allowCancelAll)
            If plhCancelAll.Visible Then
                If _displayCancelAllMsg Then
                    lblCancelAllMsg.Visible = True
                    hlnkCancelAll.Visible = False
                    lblCancelAllMsg.Text = _ucr.Content("lblCancelAllMsgText", _languageCode, True)
                Else
                    hlnkCancelAll.Visible = True
                    lblCancelAllMsg.Visible = False
                    If _isBasketHasItems Then
                        hlnkCancelAll.NavigateUrl = "~/PagesLogin/Orders/CATConfirm.aspx?Mode=CA&TrnEnq=" & _isTransactionEnquiry.ToString & "&PayRef=" & txtPayRef.Text.Trim
                    Else
                        hlnkCancelAll.NavigateUrl = "~/Redirect/TicketingGateway.aspx?page=catconfirm.aspx&function=addtobasket&catmode=CA" &
                                                            "&payref=" & txtPayRef.Text.Trim &
                                                            "&istrnxenq=" & _isTransactionEnquiry.ToString
                    End If
                End If
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhError.Visible = (ltlError.Text.Length > 0)
    End Sub

    Protected Sub promoDDL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles promoDDL.SelectedIndexChanged
        If promoDDL.SelectedIndex = 0 Then
            plhPromotionDetails.Visible = False
            Response.Redirect(Request.Url.AbsoluteUri)
        Else
            Dim myDs As DataSet = GetPromotionDetails()
            If Not myDs Is Nothing AndAlso myDs.Tables.Count > 0 AndAlso myDs.Tables(0).Rows.Count > 0 Then
                For Each rw As DataRow In myDs.Tables(0).Rows
                    If rw("PromotionID") = promoDDL.SelectedValue Then
                        desc.Text = rw("Description1") & "<br/>" & rw("Description2")
                        comp.Text = rw("CompetitionDescription")
                        ' Start and End Dates are no longer specified for single match promotions
                        If Not (rw("StartDate").Equals(DBNull.Value)) Then
                            startDate.Text = rw("StartDate")
                            Me.promoStartWrap.Visible = True
                        Else
                            Me.promoStartWrap.Visible = False
                        End If
                        If Not (rw("EndDate").Equals(DBNull.Value)) Then
                            endDate.Text = rw("EndDate")
                            Me.promoStartWrap.Visible = True
                        Else
                            Me.promoEndWrap.Visible = False
                        End If
                        max.Text = rw("MaxPerPromotion")
                        plhPromotionDetails.Visible = True
                        setLabelValues()
                        Exit For
                    End If
                Next
            End If
        End If

    End Sub

    Protected Sub filterButton_Click1(ByVal sender As Object, ByVal e As System.EventArgs) Handles filterButton.Click
        Session("PurchaseFromDate") = FromDate.Text
        Session("PurchaseToDate") = ToDate.Text
        Session("filterStatus") = Status.SelectedItem.Value
        Session("filterPayRef") = txtPayRef.Text.Trim
        Session("ProductType") = ddlProductType.SelectedValue
        Session("ShowCorporateOnly") = chkShowCorporateProducts.Checked
        Session("OrderEnquiryDetails") = Nothing
        Response.Redirect("~/PagesLogin/Orders/orderEnquiry.aspx?FilterChange=True")
    End Sub

    Protected Sub btnClear_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClear.Click
        Session("PurchaseFromDate") = FromDate.Text
        Session("PurchaseToDate") = ToDate.Text
        Session("filterStatus") = Status.SelectedItem.Value
        Session("filterPayRef") = txtPayRef.Text.Trim
        Session("ProductType") = Nothing
        Session("ShowCorporateOnly") = False
        Session("OrderEnquiryDetails") = Nothing
        Response.Redirect("~/PagesLogin/Orders/orderEnquiry.aspx?FilterChange=True")
    End Sub

    Protected Sub btnPrintAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrintAll.Click
        If Session("filterPayRef") IsNot Nothing AndAlso Session("filterPayRef").ToString.Trim.Length > 0 Then
            ltlError.Text = String.Empty
            Dim printEntity As New DEPrint
            printEntity.PaymentReference = txtPayRef.Text
            ProcessPrintTickets(printEntity)
        End If
    End Sub

    Protected Sub TransactionHistoryRepeater_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles TransactionHistoryRepeater.ItemCommand
        If e.CommandName = "PrintThisTicket" Then
            ltlError.Text = String.Empty
            Dim relativeRecordNumber As String = e.CommandArgument.ToString
            Dim printEntity As New DEPrint
            printEntity.PaymentReference = txtPayRef.Text
            printEntity.RelativeRecordNumber = relativeRecordNumber
            ProcessPrintTickets(printEntity)
        End If
    End Sub

    Protected Sub OrderHistoryRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles OrderHistoryRepeater.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            repeaterItemBound(e)
        End If
    End Sub

    Protected Sub TransactionHistoryRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles TransactionHistoryRepeater.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            repeaterItemBound(e)
        End If
    End Sub

    Protected Sub CATColumnPreRender(ByVal sender As Object, ByVal e As System.EventArgs)
        If Not _isTransactionEnquiry Then
            CType(sender, HtmlGenericControl).Visible = False
        End If
    End Sub

    Protected Sub TransferColumnPreRender(ByVal sender As Object, ByVal e As System.EventArgs)
        If Not _allowTransfer Then
            CType(sender, HtmlGenericControl).Visible = False
        End If
    End Sub

    Protected Sub AmendColumnPreRender(ByVal sender As Object, ByVal e As System.EventArgs)
        If Not _allowAmend Then
            CType(sender, HtmlGenericControl).Visible = False
        End If
    End Sub

    Protected Sub CancelColumnPreRender(ByVal sender As Object, ByVal e As System.EventArgs)
        If Not _allowCancel Then
            CType(sender, HtmlGenericControl).Visible = False
        End If
    End Sub

    Protected Sub PrintColumnPreRender(ByVal sender As Object, ByVal e As System.EventArgs)
        If Not _allowPrint Then
            CType(sender, HtmlGenericControl).Visible = False
        End If
    End Sub

    Protected Sub rptPaymentDetails_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptPaymentDetails.ItemDataBound
        If e.Item.ItemType = ListItemType.Header Then
            Dim ltlPayDetPayTypeLabel As Literal = TryCast(e.Item.FindControl("ltlPayDetPayTypeLabel"), Literal)
            Dim ltlPayDetPayAmountLabel As Literal = TryCast(e.Item.FindControl("ltlPayDetPayAmountLabel"), Literal)
            Dim ltlPayTypeDetailLabel As Literal = TryCast(e.Item.FindControl("ltlPayTypeDetailLabel"), Literal)
            ltlPayDetPayTypeLabel.Text = _ucr.Content("PayTypeLabel", _languageCode, True)
            ltlPayDetPayAmountLabel.Text = _ucr.Content("PayAmountLabel", _languageCode, True)
            ltlPayTypeDetailLabel.Text = _ucr.Content("PayTypeDetailLabel", _languageCode, True)
        ElseIf e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim ltlPayDetPayTypeValue As Literal = TryCast(e.Item.FindControl("ltlPayDetPayTypeValue"), Literal)
            Dim ltlPayDetPayAmountValue As Literal = TryCast(e.Item.FindControl("ltlPayDetPayAmountValue"), Literal)
            Dim ltlPayTypeDetail As Literal = TryCast(e.Item.FindControl("ltlPayTypeDetail"), Literal)
            Dim plhPayTypeDetails As PlaceHolder = TryCast(e.Item.FindControl("plhPayTypeDetails"), PlaceHolder)
            Dim paymentDetailRow As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Dim sbPayTypeDetail As New StringBuilder
            ltlPayDetPayTypeValue.Text = GetPaymentTypeCodeDescription(paymentDetailRow("PayType"), _businessUnit, _partnerCode, _languageCode)
            ltlPayDetPayAmountValue.Text = TDataObjects.PaymentSettings.FormatCurrency(paymentDetailRow("PayAmount"), _ucr.BusinessUnit, _ucr.PartnerCode)
            plhPayTypeDetails.Visible = True
            Select Case CheckForDBNull_String(paymentDetailRow("PayType"))
                Case GlobalConstants.CCPAYMENTTYPE
                    If Not String.IsNullOrWhiteSpace(paymentDetailRow("CardNumber")) Then
                        sbPayTypeDetail.Append(GetPayDetailHtmlListItem("CreditCardNumberLabel", _ucr.Content("EncryptedCardNumberPrefixString", _languageCode, True) & paymentDetailRow("CardNumber")))
                    End If
                    If paymentDetailRow("ExpiryDate").ToString.Length = 4 AndAlso paymentDetailRow("ExpiryDate").ToString <> "0000" Then
                        sbPayTypeDetail.Append(GetPayDetailHtmlListItem("ExpiryDateLabel", paymentDetailRow("ExpiryDate").ToString.Substring(0, 2) &
                                                                                           "/" &
                                                                                           paymentDetailRow("ExpiryDate").ToString.Substring(2, 2)))
                    End If
                    sbPayTypeDetail.Append(GetPayDetailHtmlListItem("IssueDetailLabel", paymentDetailRow("IssueDetail")))
                    If paymentDetailRow("StartDate").ToString.Length = 4 AndAlso paymentDetailRow("StartDate").ToString <> "0000" Then
                        sbPayTypeDetail.Append(GetPayDetailHtmlListItem("StartDateLabel", paymentDetailRow("StartDate").ToString.Substring(0, 2) &
                                                                                           "/" &
                                                                                           paymentDetailRow("StartDate").ToString.Substring(2, 2)))
                    End If
                Case GlobalConstants.DDPAYMENTTYPE
                    sbPayTypeDetail.Append(GetPayDetailHtmlListItem("AccountNumberLabel", paymentDetailRow("AccountNumber")))
                    sbPayTypeDetail.Append(GetPayDetailHtmlListItem("SortCodeLabel", paymentDetailRow("SortCode")))
                    sbPayTypeDetail.Append(GetPayDetailHtmlListItem("AccountNameLabel", paymentDetailRow("AccountName")))
                Case Else
                    plhPayTypeDetails.Visible = False
            End Select
            ltlPayTypeDetail.Text = sbPayTypeDetail.ToString
        End If
    End Sub

    ''' <summary>
    ''' Checks the given seatDetails string to see whether it is a loyalty points attendance record rather than an actual stadium seat.
    ''' Eg. seatDetails = "E/EA/003/0004" is a seat, "*AT/TEND" is an attendance record in the purchase history.
    ''' </summary>
    ''' <param name="seatDetails">the given seat details passed from WS005R</param>
    ''' <param name="productType">the given product type passed from WS005R</param>
    ''' <returns>The seat if it is a valid seat, or a string taken from the SQL database based on a given ATTR_VALUE</returns>
    ''' <remarks></remarks>
    Protected Function CheckSeatDetailsForAttendance(ByVal seatDetails As String, ByVal productType As String) As String
        Dim returnValue As String = String.Empty
        Try
            Dim attendanceString As String = _ucr.Attribute("LoyaltyAttendanceCode")
            If seatDetails = attendanceString Then
                returnValue = _ucr.Content("LoyaltyAttendanceDisplayText", _languageCode, True)
            Else
                returnValue = seatDetails
            End If
            If Not String.IsNullOrWhiteSpace(productType) AndAlso productType = GlobalConstants.ERRORFLAG Then
                returnValue = String.Empty
            End If
        Catch ex As Exception
            returnValue = String.Empty
        End Try
        Return returnValue
    End Function

    ''' <summary>
    ''' Checks the given pointsExpired boolean value from the WS005R record and returns a string if true.
    ''' </summary>
    ''' <param name="pointsExpired">The given boolean value to indicate whethere the points have expired</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function CheckIfLoyaltyPointsExpired(ByVal pointsExpired As Boolean) As String
        Dim returnValue As String = String.Empty
        If pointsExpired Then
            returnValue = _ucr.Content("LoyaltyPointsExpiredText", _languageCode, True)
        End If
        Return returnValue
    End Function

    ''' <summary>
    ''' Check to see if the promotion id is present
    ''' </summary>
    ''' <param name="promotionId">Promotion id as a string</param>
    ''' <returns>Return true if the promotion id is present</returns>
    ''' <remarks></remarks>
    Protected Function ShowPromotion(ByVal promotionId As String) As Boolean
        Dim hasPromotion As Boolean = False
        Try
            If CDec(promotionId) > 0 Then hasPromotion = True
        Catch ex As Exception
            hasPromotion = False
        End Try
        Return hasPromotion
    End Function

    ''' <summary>
    ''' Append the URL, promotion id, original price and sale price together
    ''' </summary>
    ''' <param name="promotionId">the promotion id used against this purchase</param>
    ''' <param name="originalPrice">the original price for this purchase</param>
    ''' <returns>a full url as a string</returns>
    ''' <remarks></remarks>
    Protected Function GetNavigateUrl(ByVal promotionId As String, ByVal originalPrice As String, ByVal salePrice As String) As String
        Dim promotionDetailsUrl As New StringBuilder
        promotionDetailsUrl.Append("~/PagesPublic/Basket/PromotionDetails.aspx?promotionid=")
        promotionDetailsUrl.Append(promotionId)
        promotionDetailsUrl.Append("&originalprice=")
        promotionDetailsUrl.Append(originalPrice)
        promotionDetailsUrl.Append("&saleprice=")
        promotionDetailsUrl.Append(salePrice)
        promotionDetailsUrl.Append("&returnurl=")
        promotionDetailsUrl.Append(Request.RawUrl)
        Return promotionDetailsUrl.ToString()
    End Function

    Protected Function GetText(ByVal PValue As String) As String
        'Dim ucr As New Talent.Common.UserControlResource
        Dim str As String = _ucr.Content(PValue, _languageCode, True)
        Return str
    End Function

    Protected Function showDate(ByVal Pvalue As String) As String
        Dim str As String

        str = Pvalue.Replace("#", "")
        '  str = Pvalue.ToString("dd/MM/yyyy")

        Return str
    End Function

    Protected Function GetStatusText(ByVal PValue As String) As String
        Dim str As String = _ucr.Content(PValue, _languageCode, True)
        If String.IsNullOrEmpty(str) Then
            str = PValue
        End If
        Return str
    End Function

    ''' <summary>
    ''' Get the seat history page Url including the values given as querystring parameters
    ''' </summary>
    ''' <param name="productCode">The product code</param>
    ''' <param name="seat">The full seat string</param>
    ''' <param name="paymentRef">The payment reference</param>
    ''' <returns>The full seat history page Url</returns>
    ''' <remarks></remarks>
    Protected Function GetSeatHistoryUrl(ByVal productCode As String, ByVal seat As String, ByVal paymentRef As String) As String
        Dim navigateUrl As New StringBuilder
        navigateUrl.Append("~/PagesAgent/Orders/SeatHistory.aspx?product=").Append(productCode)
        navigateUrl.Append("&seat=").Append(Server.UrlEncode(seat))
        navigateUrl.Append("&paymentref=").Append(paymentRef)
        Return navigateUrl.ToString()
    End Function

    ''' <summary>
    ''' Get the seat print history page Url including the values given as querystring parameters
    ''' </summary>
    ''' <param name="productCode">The product code</param>
    ''' <param name="seat">The full seat string</param>
    ''' <returns>The full seat print history page Url</returns>
    ''' <remarks></remarks>
    Protected Function GetSeatPrintHistoryUrl(ByVal productCode As String, ByVal seat As String) As String
        Dim navigateUrl As New StringBuilder
        navigateUrl.Append("~/PagesAgent/Orders/SeatPrintHistory.aspx?product=").Append(productCode)
        navigateUrl.Append("&seat=").Append(Server.UrlEncode(seat))
        Return navigateUrl.ToString()
    End Function
    ''' <summary>
    ''' Get the payment details url to pop up in a shadowbox
    ''' </summary>
    ''' <param name="payRef">Pay Ref</param>
    ''' <returns>The full seat print history page Url</returns>
    ''' <remarks></remarks>
    Protected Function GetPaymenDetailsUrl(ByVal payRef As String) As String
        Dim navigateUrl As New StringBuilder
        navigateUrl.Append("~/PagesLogin/Orders/PaymentDetailsPopup.aspx?payref=").Append(payRef)
        Return navigateUrl.ToString()
    End Function

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Format the currency for the given value
    ''' </summary>
    ''' <param name="value">The valye amount</param>
    ''' <returns>The formatted value string</returns>
    ''' <remarks></remarks>
    Public Function FormatCurrency(ByVal value As Decimal) As String
        Dim formattedString As String = value
        formattedString = TDataObjects.PaymentSettings.FormatCurrency(value, _ucr.BusinessUnit, _ucr.PartnerCode)
        Return formattedString
    End Function

#End Region

#Region "Private Methods"

    Private Sub InitialiseControlsAndFlag()
        Dim err As Boolean = False
        Session("CATBasketOrderDetails") = Nothing
        plhError.Visible = False
        AssignTransactionEnquiryFlag()
        AssignCATTicketsFlag()
        AssignPrintTicketsFlag()
        Try
            If Not Page.IsPostBack Then
                RegisterClearFieldsJS()
                SetLabelValues()
                SetFieldValues()
                Dim errorCode As String = String.Empty
                If TryParseFilterValues(errorCode) Then
                    err = showData(Request.QueryString("Page"))
                    If Not err Then
                        chooseRepeaterToBind()
                        PopulatePromoDDL()
                    End If
                Else
                    ProcessErrorMessage(errorCode)
                End If
            End If
        Catch ex As Exception

        End Try


    End Sub

    Private Sub SetFieldValues()
        LoadStatusOptions()
        If Session("PurchaseFromDate") IsNot Nothing AndAlso Session("PurchaseToDate") IsNot Nothing AndAlso Request.UrlReferrer IsNot Nothing AndAlso Request.UrlReferrer.PathAndQuery.ToLower.Contains("orderenquiry.aspx") Then
            FromDate.Text = Session("PurchaseFromDate")
            ToDate.Text = Session("PurchaseToDate")
            If Session("filterStatus") IsNot Nothing Then
                Status.SelectedIndex = Status.Items.IndexOf(Status.Items.FindByValue(Session("filterStatus")))
            End If
        End If
        Dim pageName As String = Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper
        If (pageName = "TRANSACTIONDETAILS.ASPX" OrElse pageName = "PAYMENTDETAILSPOPUP.ASPX") AndAlso Session("filterPayRef") IsNot Nothing Then
            txtPayRef.Text = Session("filterPayRef")
            plhFilters.Visible = False
            orderPromotionView.Visible = False
            orderOnAccountView.Visible = False
        End If
        Dim dicProductTypes As New Generic.Dictionary(Of String, String)
        dicProductTypes.Add(ProductTypeNoProductSelected, ProductTypeNoProductSelected)
        dicProductTypes.Add(GlobalConstants.HOMEPRODUCTTYPE, _ucr.Content("ProductType-H", _languageCode, True))
        dicProductTypes.Add(GlobalConstants.AWAYPRODUCTTYPE, _ucr.Content("ProductType-A", _languageCode, True))
        dicProductTypes.Add(GlobalConstants.SEASONTICKETPRODUCTTYPE, _ucr.Content("ProductType-S", _languageCode, True))
        dicProductTypes.Add(GlobalConstants.MEMBERSHIPPRODUCTTYPE, _ucr.Content("ProductType-C", _languageCode, True))
        dicProductTypes.Add(GlobalConstants.TRAVELPRODUCTTYPE, _ucr.Content("ProductType-T", _languageCode, True))
        dicProductTypes.Add(GlobalConstants.EVENTPRODUCTTYPE, _ucr.Content("ProductType-E", _languageCode, True))
        ddlProductType.DataSource = dicProductTypes
        ddlProductType.DataTextField = "Value"
        ddlProductType.DataValueField = "Key"
        ddlProductType.DataBind()
        If Session("ProductType") IsNot Nothing Then ddlProductType.SelectedValue = Session("ProductType")
        If Session("ShowCorporateOnly") IsNot Nothing Then chkShowCorporateProducts.Checked = Session("ShowCorporateOnly")
    End Sub

    Private Sub AssignTransactionEnquiryFlag()
        Dim orderTemplate As String = String.Empty
        Dim orderTemplateSubType As String = String.Empty
        orderPaymentView.Visible = False
        If (Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper = "TRANSACTIONDETAILS.ASPX" OrElse _
            Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper = "PAYMENTDETAILSPOPUP.ASPX") Then
            orderTemplate = "2"
            orderTemplateSubType = "2"
            orderPaymentView.Visible = True
        Else
            If Not String.IsNullOrEmpty(Request.QueryString("OrderType")) Then
                orderTemplate = Request.QueryString("OrderType").Trim
            ElseIf Session("OrderTemplateType") IsNot Nothing Then
                orderTemplate = Session("OrderTemplateType").ToString.Trim
            End If
            If Not String.IsNullOrWhiteSpace(Request.QueryString("OrderTemplateSubType")) Then
                orderTemplateSubType = Request.QueryString("OrderTemplateSubType").Trim
            ElseIf Session("OrderTemplateSubType") IsNot Nothing Then
                orderTemplateSubType = Session("OrderTemplateSubType")
            Else
                orderTemplateSubType = "1"
            End If
            If orderTemplate.Length <= 0 Then
                orderTemplate = MyBase.ModuleDefaults.Order_Enquiry_Template_Type
            End If
        End If

        Select Case orderTemplate
            Case Is = "2"
                If orderTemplateSubType = "1" Then
                    _isTransactionEnquiry = False
                Else
                    _isTransactionEnquiry = True
                End If
            Case Else
                _isTransactionEnquiry = False
        End Select
        If _isTransactionEnquiry Then
            plhPayRefFilterField.Visible = True
        Else
            plhPayRefFilterField.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Assigns the CAT tickets flag of whether to show the CAT button or not based on agent or user
    ''' </summary>
    Private Sub AssignCATTicketsFlag()
        If Session("TicketCollectionMode") IsNot Nothing _
                AndAlso Not String.IsNullOrWhiteSpace(Session("TicketCollectionMode")) Then
            _isTicketCollection = CType(Session("TicketCollectionMode"), Boolean)
        End If
        If _isTicketCollection Then
            _allowCancelAll = False
            _allowCancel = False
            _allowTransfer = False
            _allowAmend = False
        ElseIf (Not String.IsNullOrWhiteSpace(AgentProfile.Name)) AndAlso ModuleDefaults.AllowCATTicketsByAgent Then
            _allowCancelAll = CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("Allow_Cancel_All_By_Agent", True))
            _allowCancel = CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("Allow_Cancel_By_Agent", True))
            _allowTransfer = CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("Allow_Transfer_By_Agent", True))
            _allowAmend = CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("Allow_Amend_By_Agent", True))
        ElseIf ModuleDefaults.AllowCATTicketsByUser Then
            _allowCancelAll = CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("Allow_Cancel_All_By_User", True))
            _allowCancel = CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("Allow_Cancel_By_User", True))
            _allowTransfer = CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("Allow_Transfer_By_User", True))
            _allowAmend = CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("Allow_Amend_By_User", True))
        Else
            _allowCancelAll = False
            _allowCancel = False
            _allowTransfer = False
            _allowAmend = False
        End If
        'for repeater CAT links text
        _displayTextTransfer = _ucr.Content("TransferLinkText", _languageCode, True)
        _displayTextAmend = _ucr.Content("AmendLinkText", _languageCode, True)
        _displayTextCancel = _ucr.Content("CancelLinkText", _languageCode, True)
        hlnkCancelAll.Text = _ucr.Content("CancelAllLinkText", _languageCode, True)
        _isBasketHasItems = Profile.Basket.BasketItems.Count > 0
    End Sub

    Private Sub AssignPrintTicketsFlag()
        'assign Print flag
        If AgentProfile.IsAgent Then
            If _isTicketCollection _
                AndAlso CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("Allow_Print_By_Agent", True)) Then
                _allowPrint = True
            End If
            If CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("Allow_Print_FromDetail_By_Agent", True)) Then
                _allowPrint = True
            End If
            'If _allowPrint AndAlso (Not Page.IsPostBack) Then
            'txtPrinterName.Text = AgentProfile.GetAgentDefaultPrinterName()
            'End If
        Else
            _allowPrint = False
        End If
    End Sub

    Private Sub CancelByPayRefActive()
        _displayCancelAllMsg = False
        If _isTransactionEnquiry AndAlso txtPayRef.Visible AndAlso txtPayRef.Text.Trim.Length > 0 Then
            Dim dtOrderDetails As DataTable = _dsPurchaseHistory.Tables(_cProductTable).DefaultView.Table
            If (dtOrderDetails IsNot Nothing) Then
                If dtOrderDetails.Rows.Count > 0 Then
                    Dim dvOrderDetails As DataView = dtOrderDetails.DefaultView
                    Dim filterCondition As New StringBuilder
                    filterCondition.Append("(PaymentReference='" & txtPayRef.Text.PadLeft(15, "0") & "') AND (StatusCode <> 'CANCEL') AND (FeesCode = ' ')")
                    If IsAgent() Then
                        filterCondition.Append(" AND (CATCancelAgent <> 'True')")
                    Else
                        filterCondition.Append(" AND (CATCancelWeb <> 'True')")
                    End If
                    dtOrderDetails.DefaultView.RowFilter = filterCondition.ToString()
                    If dtOrderDetails.DefaultView.Count > 0 Then
                        _displayCancelAllMsg = True
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub ProcessErrorMessage(ByVal errorCode As String)
        plhError.Visible = True
        ltlError.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content(errorCode, _languageCode, True))
    End Sub

    ''' <summary>
    ''' Registers the clear fields Javascript function.
    ''' </summary>
    Private Sub RegisterClearFieldsJS()
        Dim sbClearFieldsScript As New StringBuilder
        sbClearFieldsScript.AppendLine("<script language=""javascript"" type=""text/javascript"">")
        sbClearFieldsScript.AppendLine("function ClearFields() {")
        sbClearFieldsScript.AppendLine("if (document.getElementById('" & FromDate.ClientID & "') != null) {")
        sbClearFieldsScript.AppendLine("document.getElementById('" & FromDate.ClientID & "').value=""""; }")
        sbClearFieldsScript.AppendLine("if (document.getElementById('" & ToDate.ClientID & "') != null) {")
        sbClearFieldsScript.AppendLine("document.getElementById('" & ToDate.ClientID & "').value=""""; }")
        sbClearFieldsScript.AppendLine("if (document.getElementById('" & Status.ClientID & "') != null) {")
        sbClearFieldsScript.AppendLine("document.getElementById('" & Status.ClientID & "').selectedIndex=0; }")
        sbClearFieldsScript.AppendLine(" return true;}")
        sbClearFieldsScript.AppendLine("</script>")
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "RegisterClearFieldsScript", sbClearFieldsScript.ToString)
        btnClear.Visible = True
    End Sub

    Private Sub showError(ByVal errCode As String)
        plhError.Visible = True
        ltlError.Visible = True
        If errCode = "NORECORDFOUND" Then
            OrderHistoryRepeater.Visible = False
            plhProductSaleDetails.Visible = False
            ltlError.Text = _ucr.Content("NoRecordFoundError", Me._languageCode, True)
        ElseIf errCode = "NS" Or errCode = "WS" Or errCode = "MU" Or errCode = "NS" Then
            ltlError.Text = getErrText(errCode)
        Else
            ltlError.Text = getErrText("XX")
        End If
    End Sub

    Private Sub SetLabelValues()
        With _ucr
            'All other labels
            If Talent.eCommerce.Utilities.IsAgent Then
                AgentLabel.Text = .Content("AgentAuthorityWarningText", _languageCode, True)
            Else
                AgentLabel.Visible = False
            End If
            PayRefLabel.Text = .Content("PayRefLabel", _languageCode, True)
            rgxPaymentReference.ErrorMessage = .Content("InvalidPaymentReferenceError", _languageCode, True)
            FromDateLabel.Text = .Content("FromDateLabel", _languageCode, True)
            ToDateLabel.Text = .Content("ToDateLabel", _languageCode, True)
            StatusLabel.Text = .Content("StatusLabel", _languageCode, True)
            lblProductType.Text = .Content("ProductTypeLabel", _languageCode, True)
            lblShowCorporateProducts.Text = .Content("ShowCorporateProductsLabel", _languageCode, True)
            filterButton.Text = .Content("filterButton", _languageCode, True)

            'set links text
            displayingLabelT.Text = .Content("displayingLabelT", _languageCode, True)
            displayingLabelB.Text = .Content("displayingLabelB", _languageCode, True)
            toLabelT.Text = .Content("toLabelT", _languageCode, True)
            toLabelB.Text = .Content("toLabelB", _languageCode, True)
            ofLabelT.Text = .Content("ofLabelT", _languageCode, True)
            ofLabelB.Text = .Content("ofLabelB", _languageCode, True)

            'set links text
            LnkFirstT.Text = .Content("LnkFirstT", _languageCode, True)
            LnkPrevT.Text = .Content("LnkPrevT", _languageCode, True)
            LnkNextT.Text = .Content("LnkNextT", _languageCode, True)
            LnkLastT.Text = .Content("LnkLastT", _languageCode, True)
            LnkFirstB.Text = .Content("LnkFirstB", _languageCode, True)
            LnkPrevB.Text = .Content("LnkPrevB", _languageCode, True)
            LnkNextB.Text = .Content("LnkNextB", _languageCode, True)
            LnkLastB.Text = .Content("LnkLastB", _languageCode, True)

            'Setup the Promotions Controls
            '----------------------------------
            promoSelectWrap.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowPromotionSelection"))
            promoDescWrap.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowPromotionDescription"))
            promoCompWrap.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowPromotionCompetition"))
            promoStartWrap.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowPromotionStartDate"))
            promoEndWrap.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowPromotionEndDate"))
            promoMaxWrap.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowPromotionMaxAllowed"))
            promoCurrentWrap.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowPromotionCurrentApplied"))

            promoTitle.Text = .Content("PromotionTitleLabel", Me._languageCode, True)
            promoIntro.Text = .Content("PromotionIntroLabel", Me._languageCode, True)

            If promoSelectWrap.Visible Then
                promoLabel.Text = .Content("PromotionSelectionLabel", Me._languageCode, True)
            End If
            If promoDescWrap.Visible Then
                descLabel.Text = .Content("PromotionDescriptionLabel", Me._languageCode, True)
            End If
            If promoCompWrap.Visible Then
                compLabel.Text = .Content("PromotionCompetitionLabel", Me._languageCode, True)
            End If
            If promoStartWrap.Visible Then
                startLabel.Text = .Content("PromotionStartDateLabel", Me._languageCode, True)
            End If
            If promoEndWrap.Visible Then
                endLabel.Text = .Content("PromotionEndDateLabel", Me._languageCode, True)
            End If
            If promoMaxWrap.Visible Then
                maxLabel.Text = .Content("PromotionMaxAllowedLabel", Me._languageCode, True)
            End If
            If promoCurrentWrap.Visible Then
                currentLabel.Text = .Content("PromotionCurrentAppliedLabel", Me._languageCode, True)
            End If
            noPromoLabel.Text = .Content("NoPromotionsLabel", Me._languageCode, True)

            'Setup the Order Return Controls
            '----------------------------------
            orderReturnBankedWrap.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowOrderReturnBanked"))
            orderReturnRewardWrap.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowOrderReturnReward"))
            orderReturnContentWrap.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowOrderReturnContentPane"))

            orderReturnTitle.Text = .Content("orderReturnTitleLabel", Me._languageCode, True)
            orderReturnIntro.Text = .Content("orderReturnIntroLabel", Me._languageCode, True)

            If orderReturnBankedWrap.Visible Then
                orderReturnBankedLabel.Text = .Content("OrderReturnBankedLabel", Me._languageCode, True)
            End If
            If orderReturnRewardWrap.Visible Then
                orderReturnRewardLabel.Text = .Content("OrderReturnRewardLabel", Me._languageCode, True)
            End If

            noOrderReturnLabel.Text = .Content("NoOrderReturnLabel", Me._languageCode, True)


            'Setup the Cashback Controls
            '----------------------------------
            cashbackBankedWrap.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowCashbackBanked"))
            cashbackUnbankedWrap.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowCashbackUnanked"))
            cashbackSpentWrap.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowCashbackSpent"))

            cashbackTitle.Text = .Content("cashbackTitle", Me._languageCode, True)
            cashbackIntro.Text = .Content("cashbackIntro", Me._languageCode, True)

            If cashbackBankedWrap.Visible Then
                cashbackBankedLabel.Text = .Content("cashbackBankedLabel", Me._languageCode, True)
            End If
            If cashbackUnbankedWrap.Visible Then
                cashbackUnbankedLabel.Text = .Content("cashbackUnbankedLabel", Me._languageCode, True)
            End If
            If cashbackSpentWrap.Visible Then
                cashbackSpentLabel.Text = .Content("cashbackSpentLabel", Me._languageCode, True)
            End If

            noCashbackLabel.Text = .Content("noCashbackLabel", Me._languageCode, True)

            cashbackView.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowCashbackView"))
            orderReturnView.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowOrderReturnView"))
            orderPromotionView.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(.Attribute("ShowOrderPromotionView"))

            'Setup the On Account Panel
            '----------------------------------
            If cashbackView.Visible Or orderReturnView.Visible Then
                orderOnAccountView.Visible = True
                onaccountTitle.Text = .Content("onaccountTitle", Me._languageCode, True)
                onAccountIntro.Text = .Content("onaccountIntro", Me._languageCode, True)
            Else
                orderOnAccountView.Visible = False
            End If

            'print
            _displayTextPrint = .Content("PrintButtonText", _languageCode, True)
            btnPrintAll.Text = .Content("PrintAllButtonText", _languageCode, True)
            btnClear.Text = .Content("ClearButtonText", _languageCode, True)
            cvdFromDate.ErrorMessage = .Content("InvalidFromDateText", _languageCode, True)
            cvdToDate.ErrorMessage = .Content("InvalidToDateText", _languageCode, True)
        End With

    End Sub

    Private Sub LoadDatesOnFirstPageLoad()
        If Session("PurchaseFromDate") IsNot Nothing AndAlso Session("PurchaseToDate") IsNot Nothing AndAlso Request.UrlReferrer IsNot Nothing AndAlso Request.UrlReferrer.PathAndQuery.ToLower.Contains("orderenquiry.aspx") Then
            FromDate.Text = Session("PurchaseFromDate")
            ToDate.Text = Session("PurchaseToDate")
        Else
            Dim daysBtwDatesOnFirstLoad As Integer = CheckForDBNull_Int(_ucr.Attribute("DaysBetweenDatesFirstLoad"))
            Dim datesMaxDiff As Integer = CheckForDBNull_Int(_ucr.Attribute("DatesMaxDifference"))
            If daysBtwDatesOnFirstLoad <= 0 Then
                daysBtwDatesOnFirstLoad = 30
            ElseIf daysBtwDatesOnFirstLoad > datesMaxDiff Then
                daysBtwDatesOnFirstLoad = datesMaxDiff
            End If
            'always dd/mm/yyyy
            ToDate.Text = Now.ToString("dd/MM/yyyy")
            FromDate.Text = Now.AddDays(-daysBtwDatesOnFirstLoad).ToString("dd/MM/yyyy")
            Session("PurchaseFromDate") = FromDate.Text
            Session("PurchaseToDate") = ToDate.Text
        End If
    End Sub

    Private Sub PopulatePromoDDL()
        If orderPromotionView.Visible Then
            promoContentWrap.Visible = True
            noPromoWrap.Visible = True
            Dim dsPromotionDetails As DataSet = GetPromotionDetails()
            If Not dsPromotionDetails Is Nothing AndAlso dsPromotionDetails.Tables.Count > 0 AndAlso dsPromotionDetails.Tables(0).Rows.Count > 0 _
                AndAlso Not _dsPurchaseHistory Is Nothing AndAlso _dsPurchaseHistory.Tables.Count > 0 AndAlso _dsPurchaseHistory.Tables(0).Rows.Count > 0 _
                    AndAlso OrderHistoryRepeater.Items.Count > 0 Then

                promoDDL.Items.Clear()

                Dim li As ListItem
                For Each prw As DataRow In dsPromotionDetails.Tables(0).Rows
                    For Each rw As DataRow In _dsPurchaseHistory.Tables(0).Rows
                        If rw("PromotionID") = prw("PromotionID") Then
                            li = New ListItem(prw("ShortDescription"), prw("PromotionID"))
                            promoDDL.Items.Add(li)
                            Exit For
                        End If
                    Next
                Next

                If promoDDL.Items.Count > 0 Then
                    promoDDL.Items.Insert(0, New ListItem(_ucr.Content("PromotionDDLPleaseSelect", _languageCode, True), ""))
                    promoContentWrap.Visible = True
                    noPromoWrap.Visible = False
                Else
                    promoContentWrap.Visible = False
                    noPromoWrap.Visible = True
                End If
            Else
                promoContentWrap.Visible = False
                noPromoWrap.Visible = True
            End If
        End If
    End Sub

    Private Sub LoadStatusOptions()
        If _ucr.Content("StatusOptions", _languageCode, True).Trim.Length > 0 Then
            Dim arrOption As String() = _ucr.Content("StatusOptions", _languageCode, True).Trim.Split(";")
            If arrOption.Length > 0 Then
                For arrIndex As Integer = 0 To arrOption.Length - 1
                    Dim tempListItem As New ListItem
                    tempListItem.Text = _ucr.Content(arrOption(arrIndex).Trim, _languageCode, True)
                    tempListItem.Value = arrOption(arrIndex).Trim
                    Status.Items.Insert(arrIndex, tempListItem)
                Next
            End If
        End If
    End Sub

    Private Sub SetPagerValues()
        If totLinks = 0 OrElse vTotalPages = 0 Then
            plhBottomPager.Visible = False
            plhTopPager.Visible = False
        Else
            displayingValueLabelB.Text = displayingValueLabelT.Text
            toValueLabelB.Text = toValueLabelT.Text
            ofValueLabelB.Text = ofValueLabelT.Text
            LnkPrevB.NavigateUrl = LnkPrevT.NavigateUrl
            LnkNextB.NavigateUrl = LnkNextT.NavigateUrl
            LinksLabelB.Text = LinksLabelT.Text
            If CurPage = 1 Or CurPage = 0 Then
                LnkFirstT.Visible = False
                LnkFirstB.Visible = False
                LnkPrevT.Visible = False
                LnkPrevB.Visible = False
            Else
                LnkFirstT.Visible = True
                LnkFirstT.NavigateUrl = Request.CurrentExecutionFilePath + "?Page=1"
                LnkFirstB.Visible = True
                LnkFirstB.NavigateUrl = Request.CurrentExecutionFilePath + "?Page=1"
            End If
            If CurPage = vTotalPages OrElse totLinks = 0 OrElse vTotalPages = 0 Then
                LnkLastT.Visible = False
                LnkLastB.Visible = False
                LnkNextT.Visible = False
                LnkNextB.Visible = False
            Else
                plhBottomPager.Visible = True
                plhTopPager.Visible = True
                LnkLastT.Visible = True
                LnkLastT.NavigateUrl = Request.CurrentExecutionFilePath + "?Page=" + vTotalPages.ToString
                LnkLastB.Visible = True
                LnkLastB.NavigateUrl = Request.CurrentExecutionFilePath + "?Page=" + vTotalPages.ToString()
            End If
            If (_lastRecordNumber < _totalRecords) Then
                LnkLastT.Visible = False
                LnkLastB.Visible = False
            End If
            ofValueLabelT.Text = _totalRecords.ToString
            ofValueLabelB.Text = _totalRecords.ToString
        End If
    End Sub

    Private Sub SetPageNumbersByCurrentPage(ByVal currentPageNumber As String, ByVal purchaseHistoryView As DataView)
        Dim VnumberOfLinks As String = String.Empty
        If currentPageNumber <> "" Then
            CurPage = currentPageNumber
        Else
            CurPage = 1
        End If

        objPds.CurrentPageIndex = CurPage - 1
        CPage.Text = CurPage.ToString()

        vPerpage = objPds.PageSize
        CurRec = CurPage * vPerpage
        displayingValueLabelT.Text = CurRec - vPerpage + 1

        'vTotalRec = ds1.Tables(_cProductTable).DefaultView.Count
        vTotalRec = purchaseHistoryView.Count
        If vTotalRec = 0 Then
            displayingValueLabelT.Text = 0
        End If
        ofValueLabelT.Text = vTotalRec.ToString()
        If CurRec > vTotalRec Then
            toValueLabelT.Text = vTotalRec
        Else
            toValueLabelT.Text = CurRec
        End If

        Dim linktxt = New String("")

        'add link buttons
        totLinks = Fix(vTotalRec / objPds.PageSize)

        If vTotalRec Mod objPds.PageSize <> 0 Then
            totLinks = totLinks + 1
        End If
        VnumberOfLinks = _ucr.Attribute("numberOfLinks")
        If totLinks > VnumberOfLinks Then
            totLinks = VnumberOfLinks
        End If
        If Status.Text <> "ALL" Then
            chooseRepeaterToBind()
        Else
            If Status.Text = "ALL" Then
                _dsPurchaseHistory.Tables(_cProductTable).DefaultView.RowFilter = ""
            End If
        End If

        If totLinks > 1 Then
            If Not objPds.IsFirstPage Then LnkPrevT.NavigateUrl = Request.CurrentExecutionFilePath + "?Page=" + Convert.ToString(CurPage - 1)

            If Not objPds.IsLastPage Then LnkNextT.NavigateUrl = Request.CurrentExecutionFilePath + "?Page=" + Convert.ToString(CurPage + 1)

            vTotalPages = Fix(vTotalRec / vPerpage)
            If vTotalRec Mod vPerpage <> 0 Then
                vTotalPages = vTotalPages + 1
            End If
            Session("CurrentTotalPages") = vTotalPages
            '----------------------------------------------------------
            'List out the pages, current page does not have a hyperlink
            '----------------------------------------------------------

            If vTotalPages <= totLinks OrElse CurPage <= totLinks Then
                For I As Integer = 1 To totLinks
                    If I <> CurPage Then
                        linktxt = linktxt + " " + "<a href='" + Request.CurrentExecutionFilePath + "?Page=" + I.ToString() + "'>" + I.ToString() + "</a>"
                    Else
                        linktxt = linktxt + " " + I.ToString() + " "
                    End If
                Next
            ElseIf CurPage > (vTotalPages - totLinks) Then
                '--------------------------------------------------
                'display the last 'number_of_links number' of links
                '--------------------------------------------------
                For I As Integer = (vTotalPages - totLinks) To vTotalPages
                    If I = CurPage Then
                        linktxt = linktxt + " " + I.ToString() + " "
                    Else
                        linktxt = linktxt + " " + "<a href='" + Request.CurrentExecutionFilePath + "?Page=" + I.ToString() + "'>" + I.ToString() + "</a>"
                    End If
                Next
            Else
                '---------------------------------
                ' Display the current page halfway
                '---------------------------------
                Dim halfWay As Integer = totLinks / 2
                For I As Integer = 0 To halfWay - 1
                    linktxt = linktxt + " " + "<a href='" + Request.CurrentExecutionFilePath + "?Page=" + (CurPage - halfWay + I).ToString() + "'>" + (CurPage - halfWay + I).ToString() + "</a>"
                    '  Str = Str() & " <a href=""" & linkPage & "&page=" & (_PageNumber - halfWay + counter) & """>" & (_PageNumber - halfWay + counter) & "</a> "
                Next
                linktxt = linktxt & " " & CurPage & " "
                For I As Integer = 1 To halfWay
                    linktxt = linktxt & " <a href=""" & Request.CurrentExecutionFilePath & "?Page=" & (CurPage + I) & """>" & (CurPage + I) & "</a> "
                Next
            End If
        Else
            LnkPrevT.Visible = False
            LnkNextT.Visible = False
            If totLinks <> 0 Then
                linktxt = " 1 "
            Else
                linktxt = " 0 "
            End If

        End If
        LinksLabelT.Text = linktxt
    End Sub

    Private Sub ProcessPrintTickets(ByVal printEntity As DEPrint)
        'assign the common properties
        printEntity.PaymentOwnerName = ltlPayOwnName.Text
        printEntity.AddressLine1 = ltlPayOwnAddressLine1.Text
        printEntity.AddressLine2 = ltlPayOwnAddressLine2.Text
        printEntity.AddressLine3 = ltlPayOwnAddressLine3.Text
        printEntity.AddressLine4 = ltlPayOwnAddressLine4.Text
        printEntity.PostCodePart1 = ltlPayOwnPostCodePart1.Text
        printEntity.PostCodePart2 = ltlPayOwnPostCodePart2.Text

        Dim talPrint As New TalentPrint
        With talPrint
            .PrintEntity = printEntity
            .Settings = GetSettingsObject(False)
            .Settings.Cacheing = False
        End With
        Dim err As New ErrorObj
        err = talPrint.PrintTicketsByWeb()
        If Not err.HasError AndAlso talPrint.ResultDataSet IsNot Nothing Then
            If talPrint.ResultDataSet.Tables.Count > 0 Then
                'resultset tables count < 2
                If talPrint.ResultDataSet.Tables.Count = 1 Then
                    Dim errorCode As String = String.Empty
                    If talPrint.ResultDataSet.Tables("ResultStatus").Rows.Count > 0 Then
                        'assign the error message to lblPrintAllMsg
                        errorCode = CheckForDBNull_String(talPrint.ResultDataSet.Tables("ResultStatus").Rows(0)("ReturnCode")).Trim
                        If errorCode.Length > 0 OrElse (CheckForDBNull_String(talPrint.ResultDataSet.Tables("ResultStatus").Rows(0)("ErrorOccurred")).Trim).Length > 0 Then
                            Dim errMsg As New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
                            ltlError.Text = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, errorCode).ERROR_MESSAGE
                        Else
                            ltlError.Text = _ucr.Content("PrintSuccessMessage", _languageCode, True)
                        End If
                    End If
                End If
            End If
        Else
            'error or resultset is nothing
        End If
    End Sub

    Private Sub BindOrderReturnDetails()
        'Populate Order Return Figures
        If _dsPurchaseHistory.Tables(1).Rows(0).Item("BankedTotal") <> Nothing And _
            _dsPurchaseHistory.Tables(1).Rows(0).Item("BankedTotal").ToString.Trim <> String.Empty Then
            orderReturnBanked.Text = _dsPurchaseHistory.Tables(1).Rows(0).Item("BankedTotal").ToString.Trim
        End If

        If _dsPurchaseHistory.Tables(1).Rows(0).Item("RewardTotal") <> Nothing And _
            _dsPurchaseHistory.Tables(1).Rows(0).Item("RewardTotal").ToString.Trim <> String.Empty Then
            orderReturnReward.Text = _dsPurchaseHistory.Tables(1).Rows(0).Item("RewardTotal").ToString.Trim
        End If

        If orderReturnReward.Text = "0.00" And orderReturnBanked.Text = "0.00" Then
            orderReturnContentWrap.Visible = False
            noOrderReturnWrap.Visible = True
        Else
            orderReturnContentWrap.Visible = True
            noOrderReturnWrap.Visible = False
        End If
    End Sub

    Private Sub BindCashbackDetails()
        If _dsPurchaseHistory.Tables(1).Rows(0).Item("CashbackBankedTotal") <> Nothing And _
                        _dsPurchaseHistory.Tables(1).Rows(0).Item("CashbackBankedTotal").ToString.Trim <> String.Empty Then
            cashbackBanked.Text = _dsPurchaseHistory.Tables(1).Rows(0).Item("CashbackBankedTotal").ToString.Trim
        End If

        If _dsPurchaseHistory.Tables(1).Rows(0).Item("CashbackUnbankedTotal") <> Nothing And _
            _dsPurchaseHistory.Tables(1).Rows(0).Item("CashbackUnbankedTotal").ToString.Trim <> String.Empty Then
            cashbackUnbanked.Text = _dsPurchaseHistory.Tables(1).Rows(0).Item("CashbackUnbankedTotal").ToString.Trim
        End If

        If _dsPurchaseHistory.Tables(1).Rows(0).Item("CashbackSpentTotal") <> Nothing And _
            _dsPurchaseHistory.Tables(1).Rows(0).Item("CashbackSpentTotal").ToString.Trim <> String.Empty Then
            cashbackSpent.Text = _dsPurchaseHistory.Tables(1).Rows(0).Item("CashbackSpentTotal").ToString.Trim
        End If

        If cashbackBanked.Text = "0.00" And cashbackUnbanked.Text = "0.00" And cashbackSpent.Text = "0.00" Then
            cashbackContentWrap.Visible = False
            noCashbackWrap.Visible = True
        Else
            cashbackContentWrap.Visible = True
            noCashbackWrap.Visible = False
        End If
    End Sub

    Private Sub BindPaymentOwnerAndPurchaseDetails(ByVal dtPaymentOwnerDetails As DataTable)
        If orderPaymentView.Visible AndAlso dtPaymentOwnerDetails IsNot Nothing AndAlso dtPaymentOwnerDetails.Rows.Count > 0 Then
            plhPaymentOwnerDetails.Visible = True
            plhPurchaseDetails.Visible = True
            orderPaymentView.Visible = True

            'Payment owner details
            ltlPaymentOwnerDetailsTitle.Text = _ucr.Content("PaymentOwnerDetailsTitle", _languageCode, True)
            ltlPayOwnName.Text = dtPaymentOwnerDetails.Rows(0)("Name")
            ltlPayOwnAddressLine1.Text = dtPaymentOwnerDetails.Rows(0)("AddressLine1")
            ltlPayOwnAddressLine2.Text = dtPaymentOwnerDetails.Rows(0)("AddressLine2")
            ltlPayOwnAddressLine3.Text = dtPaymentOwnerDetails.Rows(0)("AddressLine3")
            ltlPayOwnAddressLine4.Text = dtPaymentOwnerDetails.Rows(0)("AddressLine4")
            ltlPayOwnPostCodePart1.Text = dtPaymentOwnerDetails.Rows(0)("PostCodePart1")
            ltlPayOwnPostCodePart2.Text = dtPaymentOwnerDetails.Rows(0)("PostCodePart2")

            'purchase details label and value
            ltlPurchaseDetailsTitle.Text = _ucr.Content("PurchaseDetailsTitle", _languageCode, True)
            ltlPurDetPayReferenceLabel.Text = _ucr.Content("PurchasePayRefLabel", _languageCode, True)
            ltlPurDetPayReferenceValue.Text = txtPayRef.Text
            If AgentProfile.IsAgent Then
                plhAgentName.Visible = True
                ltlPurDetOriginSourceLabel.Text = _ucr.Content("PurchaseOriginSourceLabel", _languageCode, True)
                ltlPurDetOriginSourceValue.Text = dtPaymentOwnerDetails.Rows(0)("OriginatedSource")
            Else
                plhAgentName.Visible = False
            End If
            ltlPurDetTranDateLabel.Text = _ucr.Content("PurchaseTransDateLabel", _languageCode, True)
            ltlPurDetTranDateValue.Text = dtPaymentOwnerDetails.Rows(0)("TransactionDate")
            If AgentProfile.IsAgent Then
                plhBatchRef.Visible = True
                ltlPurDetBatchLabel.Text = _ucr.Content("PurchaseBatchLabel", _languageCode, True)
                ltlPurDetBatchValue.Text = dtPaymentOwnerDetails.Rows(0)("BatchDetail")
            Else
                plhBatchRef.Visible = False
            End If
        Else
            plhPaymentOwnerDetails.Visible = False
            plhPurchaseDetails.Visible = False
        End If
    End Sub

    Private Sub BindPaymentDetails(ByVal dtPaymentDetails As DataTable)
        If orderPaymentView.Visible AndAlso dtPaymentDetails IsNot Nothing AndAlso dtPaymentDetails.Rows.Count > 0 Then
            plhPaymentDetails.Visible = True
            orderPaymentView.Visible = True
            ltlPaymentDetailsTitle.Text = _ucr.Content("PaymentDetailsTitle", _languageCode, True)
            rptPaymentDetails.DataSource = dtPaymentDetails
            rptPaymentDetails.DataBind()
        Else
            plhPaymentDetails.Visible = False
        End If
    End Sub

    Private Sub MergePurchaseHistoryDataSet(ByVal errObj As ErrorObj, ByVal dsSlotPurchaseHistory As DataSet)
        If dsSlotPurchaseHistory.Tables("StatusResults").Rows(0).Item("ErrorOccurred").ToString <> GlobalConstants.ERRORFLAG _
            AndAlso (Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper <> "TRANSACTIONDETAILS.ASPX" AndAlso _
                     Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper <> "PAYMENTDETAILSPOPUP.ASPX") Then
            If Session("OrderEnquiryDetails") IsNot Nothing Then
                If _canMergeData Then
                    Dim tempDStoMerge As DataSet = Session("OrderEnquiryDetails")
                    If tempDStoMerge.Tables("OrderEnquiryDetails") IsNot Nothing _
                                        AndAlso dsSlotPurchaseHistory.Tables("OrderEnquiryDetails") IsNot Nothing Then
                        tempDStoMerge.Tables("OrderEnquiryDetails").Merge(dsSlotPurchaseHistory.Tables("OrderEnquiryDetails"))
                    End If
                    If tempDStoMerge.Tables("PaymentOwnerDetails") IsNot Nothing _
                        AndAlso dsSlotPurchaseHistory.Tables("PaymentOwnerDetails") IsNot Nothing Then
                        tempDStoMerge.Tables("PaymentOwnerDetails").Merge(dsSlotPurchaseHistory.Tables("PaymentOwnerDetails"))
                    End If
                    If tempDStoMerge.Tables("PaymentDetails") IsNot Nothing _
                        AndAlso dsSlotPurchaseHistory.Tables("PaymentDetails") IsNot Nothing Then
                        tempDStoMerge.Tables("PaymentDetails").Merge(dsSlotPurchaseHistory.Tables("PaymentDetails"))
                    End If
                    If tempDStoMerge.Tables("PaymentDetails") IsNot Nothing _
                        AndAlso dsSlotPurchaseHistory.Tables("PaymentDetails") IsNot Nothing Then
                        tempDStoMerge.Tables("PaymentDetails").Merge(dsSlotPurchaseHistory.Tables("PaymentDetails"))
                    End If
                    If tempDStoMerge.Tables("StatusResults") IsNot Nothing _
                        AndAlso dsSlotPurchaseHistory.Tables("StatusResults") IsNot Nothing Then
                        If tempDStoMerge.Tables("StatusResults").Rows.Count > 0 AndAlso dsSlotPurchaseHistory.Tables("StatusResults").Rows.Count > 0 Then
                            tempDStoMerge.Tables("StatusResults").Rows(0)("LastReturnedRecordNumber") = dsSlotPurchaseHistory.Tables("StatusResults").Rows(0)("LastReturnedRecordNumber")
                            tempDStoMerge.Tables("StatusResults").Rows(0)("TotalRecordNumber") = dsSlotPurchaseHistory.Tables("StatusResults").Rows(0)("TotalRecordNumber")
                        End If
                    End If
                    _dsPurchaseHistory = tempDStoMerge
                    Session("OrderEnquiryDetails") = tempDStoMerge
                Else
                    _dsPurchaseHistory = dsSlotPurchaseHistory
                    Session("OrderEnquiryDetails") = dsSlotPurchaseHistory
                End If
            Else
                _dsPurchaseHistory = dsSlotPurchaseHistory
                If dsSlotPurchaseHistory.Tables("OrderEnquiryDetails") IsNot Nothing AndAlso dsSlotPurchaseHistory.Tables("OrderEnquiryDetails").Rows.Count > 0 Then
                    Session("OrderEnquiryDetails") = dsSlotPurchaseHistory
                End If
            End If
        Else
            _dsPurchaseHistory = dsSlotPurchaseHistory
            If Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper = "TRANSACTIONDETAILS.ASPX" OrElse _
                Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper = "PAYMENTDETAILSPOPUP.ASPX" Then
                Session("OrderEnquiryDetails") = dsSlotPurchaseHistory
            End If
        End If
    End Sub

    Private Sub chooseRepeaterToBind()
        If TransactionHistoryMode Then
            TransactionHistoryRepeater.DataSource = objPds
            TransactionHistoryRepeater.DataBind()
        Else
            OrderHistoryRepeater.DataSource = objPds
            OrderHistoryRepeater.DataBind()
        End If
    End Sub

    Private Sub repeaterItemBound(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        Dim pageName As String = Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper
        Dim hlpPromotion As HyperLink = CType(e.Item.FindControl("hlpPromotion"), HyperLink)
        Dim viewShadowProperties As String = (CheckForDBNull_String(_ucr.Attribute("ViewShadowBoxProperty"))).Trim
        If hlpPromotion.Visible Then
            Dim imgPromotion As Image = CType(e.Item.FindControl("imgPromotion"), Image)
            imgPromotion.AlternateText = _ucr.Content("PromotionsIconAltText", _languageCode, True)
            imgPromotion.ImageUrl = CheckForDBNull_String(_ucr.Attribute("PromotionIconUrl"))
            If (viewShadowProperties.Length > 0) Then
                hlpPromotion.Attributes.Add("rel", viewShadowProperties)
                If hlpPromotion.NavigateUrl.Length > 0 Then
                    hlpPromotion.NavigateUrl += "&ShadowBox=True"
                End If
            Else
                hlpPromotion.Target = "_blank"
            End If
        End If

        'TCA
        Dim orderDetailRow As DataRow = CType(e.Item.DataItem, DataRowView).Row
        Dim hlnkTransfer As HyperLink = CType(e.Item.FindControl("hlnkTransfer"), HyperLink)
        Dim hlnkAmend As HyperLink = CType(e.Item.FindControl("hlnkAmend"), HyperLink)
        Dim hlnkCancel As HyperLink = CType(e.Item.FindControl("hlnkCancel"), HyperLink)

        'Set the cancel links
        If (Talent.eCommerce.Utilities.IsAgent AndAlso orderDetailRow("CATCancelAgent")) OrElse _
             (Not (Talent.eCommerce.Utilities.IsAgent) AndAlso orderDetailRow("CATCancelWeb")) Then
            hlnkCancel.Visible = True
            hlnkCancel.NavigateUrl = GetCATNavigationLink(orderDetailRow, GlobalConstants.CATMODE_CANCEL)
            hlnkCancel.Text = _displayTextCancel

            If (Not _canDisplayCancelAllButton) AndAlso _isTransactionEnquiry AndAlso (Not String.IsNullOrWhiteSpace(txtPayRef.Text)) AndAlso Not _isTicketCollection Then
                _canDisplayCancelAllButton = True
            End If
        End If

        'Set the ammend links
        If (Talent.eCommerce.Utilities.IsAgent AndAlso orderDetailRow("CATAmmendAgent")) OrElse _
             (Not (Talent.eCommerce.Utilities.IsAgent) AndAlso orderDetailRow("CATAmmendWeb")) Then
            hlnkAmend.Visible = True
            hlnkAmend.Text = _displayTextAmend
            hlnkAmend.NavigateUrl = GetCATNavigationLink(orderDetailRow, GlobalConstants.CATMODE_AMEND)
        End If

        'Set the ammend links
        If (Talent.eCommerce.Utilities.IsAgent AndAlso orderDetailRow("CATTransferAgent")) OrElse _
             (Not (Talent.eCommerce.Utilities.IsAgent) AndAlso orderDetailRow("CATTransferWeb")) Then
            hlnkTransfer.Visible = True
            hlnkTransfer.Text = _displayTextTransfer
            hlnkTransfer.NavigateUrl = GetCATNavigationLink(orderDetailRow, GlobalConstants.CATMODE_TRANSFER)
        End If

        'Set the print
        Dim btnPrint As Button = CType(e.Item.FindControl("btnPrint"), Button)
        If _allowPrint Then
            If orderDetailRow("IsPrintable") Then
                btnPrint.Text = _displayTextPrint
                btnPrint.Visible = True
            Else
                btnPrint.Visible = False
            End If
        Else
            btnPrint.Visible = False
        End If

        'Seat History and Print History
        Dim plhSeatHistory As PlaceHolder = CType(e.Item.FindControl("plhSeatHistory"), PlaceHolder)
        Dim plhSeatPrintHistory As PlaceHolder = CType(e.Item.FindControl("plhSeatPrintHistory"), PlaceHolder)
        If plhSeatHistory IsNot Nothing Then
            plhSeatHistory.Visible = False
            plhSeatPrintHistory.Visible = False
            If AgentProfile.IsAgent AndAlso pageName <> "PAYMENTDETAILSPOPUP.ASPX" Then
                plhSeatPrintHistory.Visible = True
                Dim hlkSeatPrintHistory As HyperLink = CType(e.Item.FindControl("hlkSeatPrintHistory"), HyperLink)
                hlkSeatPrintHistory.Attributes.Add("rel", viewShadowProperties)
                Dim payRef As Integer = 0
                If Integer.TryParse(e.Item.DataItem("PaymentReference"), payRef) Then
                    If payRef > 0 Then
                        plhSeatHistory.Visible = True
                        Dim hlkSeatHistory As HyperLink = CType(e.Item.FindControl("hlkSeatHistory"), HyperLink)
                        hlkSeatHistory.Attributes.Add("rel", viewShadowProperties)
                    End If
                End If
            End If
        End If

        ' Payment details 
        Dim plhPaymentDetails As PlaceHolder = CType(e.Item.FindControl("plhPaymentDetails"), PlaceHolder)
        If plhPaymentDetails IsNot Nothing Then
            plhPaymentDetails.Visible = False
            If AgentProfile.IsAgent AndAlso pageName <> "PAYMENTDETAILSPOPUP.ASPX" Then
                Dim payRef2 As Integer = 0
                If Integer.TryParse(e.Item.DataItem("PaymentReference"), payRef2) Then
                    Dim hlkPaymentDetails As HyperLink = CType(e.Item.FindControl("hlkPaymentDetails"), HyperLink)
                    hlkPaymentDetails.Attributes.Add("rel", viewShadowProperties)
                    plhPaymentDetails.Visible = True
                End If
            End If
        End If
    End Sub

    Private Function TryParseFilterValues(ByRef errorCode As String) As Boolean
        Dim isValid As Boolean = False
        If IsValidDateFilters(True) Then
            isValid = True
        Else
            isValid = False
            errorCode = "ErrorDateFilters"
        End If
        Return isValid
    End Function

    Private Function CanGetDataFromDB() As Boolean
        Dim isDBCallRequired As Boolean = False

        If String.IsNullOrWhiteSpace(Request.QueryString("Page")) Then
            isDBCallRequired = True
        End If
        If (Not isDBCallRequired) Then
            Try
                If Session("OrderEnquiryDetails") IsNot Nothing Then
                    If Session("CurrentTotalPages") IsNot Nothing Then
                        If Not String.IsNullOrWhiteSpace(Request.QueryString("Page")) Then
                            If CInt(Session("CurrentTotalPages")) = CInt(Request.QueryString("Page").Trim) Then
                                If Session("LastRecordNumber") IsNot Nothing AndAlso Session("TotalRecords") IsNot Nothing Then
                                    If CInt(Session("LastRecordNumber")) < CInt(Session("TotalRecords")) Then
                                        isDBCallRequired = True
                                        _lastRecordNumber = Session("LastRecordNumber")
                                        _totalRecords = Session("TotalRecords")
                                        _canMergeData = True
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                isDBCallRequired = False
            End Try
        End If
        If (Not isDBCallRequired) AndAlso (Session("OrderEnquiryDetails") Is Nothing) Then
            isDBCallRequired = True
        End If

        Return isDBCallRequired
    End Function

    Private Function GetPurchaseHistory() As ErrorObj
        Dim err As New Talent.Common.ErrorObj
        Try
            If CanGetDataFromDB() Then
                Dim CustNumberPayRefShouldMatch As Boolean = False
                If _isTicketCollection Then
                    CustNumberPayRefShouldMatch = True
                End If

                Dim order As New Talent.Common.TalentOrder
                Dim settings As New Talent.Common.DESettings

                settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                settings.BusinessUnit = TalentCache.GetBusinessUnit()
                settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                If Profile.IsAnonymous Then
                    If (Session("filterDetailsCustomer") IsNot Nothing) Then
                        settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(Session("filterDetailsCustomer"), 12)
                    ElseIf (Session("filterPrintCustomer") IsNot Nothing) AndAlso (Not String.IsNullOrWhiteSpace(Session("filterPrintCustomer"))) Then
                        settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(Session("filterPrintCustomer"), 12)
                    ElseIf (Session("filterPrintCustomer") IsNot Nothing) AndAlso (String.IsNullOrWhiteSpace(Session("filterPrintCustomer"))) Then
                        If CustNumberPayRefShouldMatch Then
                            settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(GlobalConstants.GENERIC_CUSTOMER_NUMBER, 12)
                        Else
                            settings.AccountNo1 = ""
                        End If
                    Else
                        settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(GlobalConstants.GENERIC_CUSTOMER_NUMBER, 12)
                    End If
                Else
                    CustNumberPayRefShouldMatch = True
                    settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)
                End If

                'is it transaction history enquiry
                If _isTransactionEnquiry AndAlso txtPayRef.Text.Trim.Length > 0 Then
                    order.Dep.PaymentReference = Talent.Common.Utilities.PadLeadingZeros(txtPayRef.Text.Trim, 15)
                Else
                    order.Dep.PaymentReference = Talent.Common.Utilities.PadLeadingZeros("", 15)
                End If
                order.Dep.CustNumberPayRefShouldMatch = CustNumberPayRefShouldMatch
                settings.Cacheing = False
                If Talent.eCommerce.Utilities.IsAgent Then
                    settings.OriginatingSource = AgentProfile.Name
                Else
                    If CInt(order.Dep.PaymentReference) <= 0 Then
                        settings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
                        settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
                        settings.AuthorityUserProfile = ModuleDefaults.AuthorityUserProfile
                    End If
                End If
                order.Dep.FromDate = GetFormattedDateString(FromDate.Text.Trim)
                order.Dep.ToDate = GetFormattedDateString(ToDate.Text.Trim)
                order.Dep.OrderStatus = Status.SelectedValue
                order.Dep.CorporateProductsOnly = chkShowCorporateProducts.Checked
                If _lastRecordNumber > 0 Then
                    order.Dep.LastRecordNumber = _lastRecordNumber + 1
                Else
                    order.Dep.LastRecordNumber = _lastRecordNumber
                End If
                order.Dep.LastRecordNumber = _lastRecordNumber
                order.Dep.TotalRecords = _totalRecords
                order.Settings() = settings
                err = order.OrderEnquiryDetails()
                MergePurchaseHistoryDataSet(err, order.ResultDataSet())
            Else
                _dsPurchaseHistory = CType(Session("OrderEnquiryDetails"), DataSet)
            End If
            If _dsPurchaseHistory.Tables("StatusResults") IsNot Nothing _
                AndAlso _dsPurchaseHistory.Tables("StatusResults").Rows.Count > 0 _
                AndAlso _dsPurchaseHistory.Tables("StatusResults").Rows(0).Item("ErrorOccurred").ToString.Trim <> GlobalConstants.ERRORFLAG Then
                _lastRecordNumber = _dsPurchaseHistory.Tables("StatusResults").Rows(0).Item("LastReturnedRecordNumber").ToString
                _totalRecords = _dsPurchaseHistory.Tables("StatusResults").Rows(0).Item("TotalRecordNumber").ToString
                If _totalRecords = 0 AndAlso _lastRecordNumber > _totalRecords Then
                    _totalRecords = _lastRecordNumber
                End If
                Session("LastRecordNumber") = _lastRecordNumber
                Session("TotalRecords") = _totalRecords
            End If
            'is the cancel by payment ref button valid.
            CancelByPayRefActive()
        Catch ex As Exception

        End Try
        Return err
    End Function

    Private Function GetFilteredPurchaseHistoryView(ByVal dtPurchaseHistory As DataTable) As DataView
        Dim DTNew As New DataTable("test")
        Dim sfilter As String = String.Empty
        Dim myRow As DataRow

        For Each col As DataColumn In dtPurchaseHistory.Columns
            If col.ColumnName = "SaleDate" Then
                DTNew.Columns.Add(col.ColumnName, Type.GetType("System.String"))
            Else
                DTNew.Columns.Add(col.ColumnName, col.DataType)
            End If
        Next
        For Each rw As DataRow In dtPurchaseHistory.Rows
            myRow = DTNew.NewRow
            For Each col As DataColumn In DTNew.Columns
                If col.ColumnName = "SaleDate" Then
                    myRow(col.ColumnName) = "#" & rw(col.ColumnName) & "#"
                Else
                    myRow(col.ColumnName) = rw(col.ColumnName)
                End If
            Next
            DTNew.Rows.Add(myRow)
        Next

        If Status.Text <> "ALL" Then sfilter = "StatusCode='" + Status.Text + "'"
        If promoSelectWrap.Visible Then
            If Not String.IsNullOrEmpty(promoDDL.SelectedValue) Then
                If Not String.IsNullOrEmpty(sfilter) Then
                    sfilter += " AND PromotionID = '" & promoDDL.SelectedValue & "'"
                Else
                    sfilter += "PromotionID = '" & promoDDL.SelectedValue & "'"
                End If
            End If
        End If
        If txtPayRef.Visible Then
            If txtPayRef.Text.Trim.Length > 0 Then
                If Not String.IsNullOrEmpty(sfilter) Then
                    sfilter += " AND PaymentReference = '" & txtPayRef.Text.Trim.PadLeft(15, "0") & "'"
                Else
                    sfilter += "PaymentReference = '" & txtPayRef.Text.Trim.PadLeft(15, "0") & "'"
                End If
            End If
        End If
        If ddlProductType.SelectedValue <> ProductTypeNoProductSelected Then
            If Not String.IsNullOrWhiteSpace(sfilter) Then sfilter += " AND "
            sfilter += "ProductType = '" & ddlProductType.SelectedValue & "'"
        End If
        DTNew.DefaultView.RowFilter = sfilter

        If promoCurrentWrap.Visible AndAlso Not String.IsNullOrEmpty(promoDDL.SelectedValue) Then
            current.Text = DTNew.DefaultView.Count
        Else
            current.Text = ""
        End If
        DTNew.DefaultView.Sort = "PaymentReference Desc"
        Return DTNew.DefaultView
    End Function

    Private Function showData(ByVal currentPageNumber As String) As Boolean
        Dim validErr As Boolean = False
        Try
            Dim err As ErrorObj = GetPurchaseHistory()
            If Not err.HasError Then
                If Session("filterValue") <> "" Then
                    If Status.Items.Count <> 0 Then
                        Status.SelectedValue = Session("filterValue")
                    Else
                        'loadStatus()
                        Status.SelectedValue = Session("filterValue")
                    End If
                Else
                    'loadStatus()
                End If

                Dim purchaseHistoryView As DataView
                purchaseHistoryView = GetFilteredPurchaseHistoryView(_dsPurchaseHistory.Tables(_cProductTable).DefaultView.Table)
                objPds.DataSource = purchaseHistoryView
                objPds.AllowPaging = True
                'Get amount of products to be displayed on a single page from the table tbl_control_attribute
                objPds.PageSize = _ucr.Attribute("numberOfProductsVariable")
                If Status.Text <> "ALL" Then
                    chooseRepeaterToBind()
                End If

                SetPageNumbersByCurrentPage(currentPageNumber, purchaseHistoryView)

                If _dsPurchaseHistory.Tables.Count > 1 Then
                    BindOrderReturnDetails()
                    BindCashbackDetails()
                    BindPaymentOwnerAndPurchaseDetails(_dsPurchaseHistory.Tables("PaymentOwnerDetails"))
                    BindPaymentDetails(_dsPurchaseHistory.Tables("PaymentDetails"))
                End If

                If objPds.Count = 0 Then
                    showError("NORECORDFOUND")
                End If
            Else
                If _dsPurchaseHistory.Tables("StatusResults").Rows(0).Item("ErrorOccurred").ToString = GlobalConstants.ERRORFLAG Then
                    showError(_dsPurchaseHistory.Tables("StatusResults").Rows(0).Item("ReturnCode").ToString())
                End If
            End If
        Catch ex As Exception
            validErr = True
        End Try
        Return validErr
    End Function

    Private Function GetPromotionDetails() As DataSet
        Dim dsPromotionDetails As New DataSet
        Dim promos As New Talent.Common.TalentPromotions
        Dim settings As New Talent.Common.DESettings
        Dim promoSettings As New Talent.Common.DEPromotionSettings
        Dim err As New Talent.Common.ErrorObj
        Dim moduleDefaults As New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues
        def = moduleDefaults.GetDefaults()

        promoSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        promoSettings.BusinessUnit = TalentCache.GetBusinessUnit()
        promoSettings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        If Profile.IsAnonymous Then
            promoSettings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(GlobalConstants.GENERIC_CUSTOMER_NUMBER, 12)
        Else
            promoSettings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)
        End If
        promoSettings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
        promoSettings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
        promoSettings.CacheDependencyPath = def.CacheDependencyPath
        promoSettings.OriginatingSource = GetOriginatingSource(AgentProfile.Name)

        promos.Settings() = promoSettings
        promos.Dep = New DEPromotions
        err = promos.GetPromotionDetails
        dsPromotionDetails = promos.ResultDataSet()
        Return dsPromotionDetails
    End Function

    Private Function GetPayDetailHtmlListItem(ByVal labelKeyName As String, ByVal payDetailValue As String) As String
        Dim displayString As String = String.Empty
        If Not String.IsNullOrWhiteSpace(payDetailValue) Then
            displayString = "<li><span class=""label"">" & _ucr.Content(labelKeyName, _languageCode, True) & "</span><span class=""value"">" & payDetailValue & "</span></li>"
        End If
        Return displayString
    End Function

    Private Function GetFormattedCustomerNumber(ByVal customerNumber As String) As String
        If Not String.IsNullOrWhiteSpace(customerNumber) Then
            If customerNumber.ToUpper = "*GENERIC" Then
                customerNumber = GlobalConstants.GENERIC_CUSTOMER_NUMBER
            End If
        End If
        Return customerNumber
    End Function

    Private Function GetCATNavigationLink(ByVal orderDetailRow As DataRow, ByVal catMode As String) As String
        Dim navigateURL As String = String.Empty
        'pay ref, seat, match code, customer number, mode, product type, stadium code
        If _isBasketHasItems Then
            navigateURL = "~/PagesLogin/Orders/CATConfirm.aspx?" & _
                                "TrnEnq=" & _isTransactionEnquiry.ToString & _
                                "&PayRef=" & CheckForDBNull_String(orderDetailRow("PaymentReference")) & _
                                "&Seat=" & CheckForDBNull_String(orderDetailRow("Seat")) & _
                                "&MatchCode=" & CheckForDBNull_String(orderDetailRow("ProductCode")) & _
                                "&CustomerNumber=" & GetFormattedCustomerNumber(CheckForDBNull_String(orderDetailRow("CustomerNumber"))) & _
                                "&ProductType=" & CheckForDBNull_String(orderDetailRow("ProductType")) & _
                                "&StadiumCode=" & CheckForDBNull_String(orderDetailRow("StadiumCode")) & _
                                "&Mode=" & catMode
        Else
            If catMode <> GlobalConstants.CATMODE_TRANSFER Then
                navigateURL = "~/Redirect/TicketingGateway.aspx" &
                        "?page=catconfirm.aspx" &
                        "&function=addtobasket" &
                        "&catmode=" & catMode &
                        "&product=" & CheckForDBNull_String(orderDetailRow("ProductCode")) &
                        "&catseat=" & CheckForDBNull_String(orderDetailRow("Seat")) &
                        "&payref=" & CheckForDBNull_String(orderDetailRow("PaymentReference")) &
                        "&istrnxenq=" & _isTransactionEnquiry.ToString &
                        "&catseatcustomerno=" & GetFormattedCustomerNumber(CheckForDBNull_String(orderDetailRow("CustomerNumber")))
            Else
                navigateURL = GetTransferSeatSelectionURL(CheckForDBNull_String(orderDetailRow("StadiumCode")),
                                                          CheckForDBNull_String(orderDetailRow("ProductCode")),
                                                          "",
                                                          CheckForDBNull_String(orderDetailRow("ProductType")),
                                                          "",
                                                          "")
                navigateURL = navigateURL &
                    "&catmode=" & HttpUtility.UrlEncode(catMode) &
                    "&catseat=" & HttpUtility.UrlEncode(CheckForDBNull_String(orderDetailRow("Seat"))) &
                    "&payref=" & CheckForDBNull_String(orderDetailRow("PaymentReference")) &
                    "&istrnxenq=" & _isTransactionEnquiry.ToString &
                    "&catseatcustomerno=" & GetFormattedCustomerNumber(CheckForDBNull_String(orderDetailRow("CustomerNumber")))
            End If
        End If
        Return navigateURL
    End Function

    Private Function GetTransferSeatSelectionURL(ByVal stadiumCode As String, ByVal productCode As String, ByVal campaignCode As String, _
                                               ByVal productType As String, ByVal productSubType As String, ByVal productHomeAsAway As String) As String
        Dim redirectUrl As String = GetCATTransferSeatSelectionURL(TDataObjects.StadiumSettings.TblStadiums.GetStadiumNameByStadiumCode(stadiumCode, _businessUnit),
                                                                   stadiumCode, productCode, campaignCode, productType, productSubType, productHomeAsAway)
        Return ResolveUrl(redirectUrl)
    End Function


    Private Function IsValidDateFilters(ByVal canAllowEmptyDates As Boolean) As Boolean
        Dim isValid As Boolean = False
        If canAllowEmptyDates AndAlso (String.IsNullOrWhiteSpace(FromDate.Text) AndAlso (String.IsNullOrWhiteSpace(ToDate.Text))) Then
            isValid = True
        ElseIf (Not String.IsNullOrWhiteSpace(FromDate.Text)) AndAlso (Not String.IsNullOrWhiteSpace(ToDate.Text)) Then
            If IsValidDateFormat(FromDate.Text) AndAlso IsValidDateFormat(ToDate.Text) Then
                Dim givenFromDate As New Date(FromDate.Text.Substring(6, 4), FromDate.Text.Substring(3, 2), FromDate.Text.Substring(0, 2))
                Dim givenToDate As New Date(ToDate.Text.Substring(6, 4), ToDate.Text.Substring(3, 2), ToDate.Text.Substring(0, 2))
                Dim datesMaxDiff As Integer = CheckForDBNull_Int(_ucr.Attribute("DatesMaxDifference"))
                If givenFromDate <= givenToDate AndAlso givenFromDate.AddDays(datesMaxDiff - 1) >= givenToDate Then
                    isValid = True
                End If
            End If
        End If
        Return isValid
    End Function

    Private Function IsValidDateFormat(ByVal dateString As String) As Boolean
        Dim isValid As Boolean = False
        Try
            If dateString.Trim.Length > 0 Then
                Dim givenDateTime As New Date(dateString.Substring(6, 4), dateString.Substring(3, 2), dateString.Substring(0, 2))
                isValid = True
            End If
        Catch ex As Exception
            isValid = False
        End Try
        Return isValid
    End Function

    Private Function GetFormattedDateString(ByVal dateString As String) As String
        Dim formattedDateString As String = String.Empty
        If dateString.Length >= 10 Then
            formattedDateString = dateString.Substring(0, 2) & dateString.Substring(3, 2) & dateString.Substring(6, 4)
        End If
        Return formattedDateString
    End Function

    Private Function getErrText(ByVal pCode As String) As String
        Dim wfrPage As New WebFormResource

        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
        End With
        Dim s As String
        s = wfrPage.Content(pCode, _languageCode, True)
        Return s

    End Function

#End Region

#Region "Old Methods Delete the page passed QA"

    'Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    '    Dim err As Boolean
    '    Session("CATBasketOrderDetails") = Nothing
    '    If Visible Then
    '        plhError.Visible = False
    '        AssignCATTicketsFlag()
    '        AssignPrintTicketsFlag()

    '        If Not Page.IsPostBack Then
    '            Try
    '                SetLabelValues()
    '                If Not String.IsNullOrEmpty(Request.QueryString("Page")) Then
    '                    FromDate.Text = Session("filterFromDate")
    '                    ToDate.Text = Session("filterToDate")
    '                    txtPayRef.Text = Session("filterPayRef")
    '                End If
    '                If Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper = "TRANSACTIONDETAILS.ASPX" AndAlso Session("filterPayRef") IsNot Nothing Then
    '                    txtPayRef.Text = Session("filterPayRef")
    '                    plhFilters.Visible = False
    '                    orderPromotionView.Visible = False
    '                    orderOnAccountView.Visible = False
    '                End If
    '                err = showData(Request.QueryString("Page"))
    '                If Not err Then
    '                    OrderHistoryRepeater.DataSource = objPds
    '                    OrderHistoryRepeater.DataBind()
    '                End If
    '                PopulatePromoDDL()

    '            Catch ex As Exception
    '            End Try

    '        Else
    '            If (Session("filterPrintCustomer") IsNot Nothing) AndAlso (Not String.IsNullOrWhiteSpace(Session("filterPrintCustomer"))) Then
    '            Else
    '                Try
    '                    Session("filterValue") = Status.SelectedValue
    '                    err = showData("1")
    '                    If Not err Then
    '                        OrderHistoryRepeater.DataSource = objPds
    '                        OrderHistoryRepeater.DataBind()
    '                    End If
    '                Catch ex As Exception

    '                End Try
    '            End If
    '        End If
    '        setBottomPagerValues()
    '        setDateLinks()
    '        plhCancelAll.Visible = _canDisplayCancelAllButton
    '        If plhCancelAll.Visible Then
    '            If _displayCancelAllMsg Then
    '                lblCancelAllMsg.Visible = True
    '                hlnkCancelAll.Visible = False
    '                lblCancelAllMsg.Text = _ucr.Content("lblCancelAllMsgText", _languageCode, True)
    '            Else
    '                hlnkCancelAll.Visible = True
    '                lblCancelAllMsg.Visible = False
    '                If _isBasketHasItems Then
    '                    hlnkCancelAll.NavigateUrl = "~/PagesLogin/Orders/CATConfirm.aspx?Mode=CA&TrnEnq=" & _isTransactionEnquiry.ToString & "&PayRef=" & txtPayRef.Text.Trim
    '                Else
    '                    hlnkCancelAll.NavigateUrl = "~/Redirect/TicketingGateway.aspx?page=catconfirm.aspx&function=addtobasket&catmode=CA" &
    '                                                        "&payref=" & txtPayRef.Text.Trim &
    '                                                        "&istrnxenq=" & _isTransactionEnquiry.ToString
    '                End If
    '            End If
    '        End If
    '    End If

    'End Sub

    Protected Sub setDateLinks()
        'ToDate.ReadOnly = True
        'FromDate.ReadOnly = True
    End Sub

#Region "Protected Functions"

    Protected Function getFilterCondition() As String
        Dim rString As String = ""
        Dim isIn As Boolean = False

        If Status.SelectedValue <> "ALL" Then
            rString = rString + "Status='" + Status.SelectedValue + "'"
            isIn = True
        End If

        If FromDate.Text <> "" Then
            If isIn Then rString = rString + " and "
            rString = rString + "ProductDate>=#" + FromDate.Text + "# and ProductDate<=#" + ToDate.Text + "#"
        End If
        Return rString
    End Function

    'Protected Function showData(ByVal pCurrentPage As String) As Boolean
    '    Dim validErr As Boolean = False
    '    Try
    '        Dim err As New Talent.Common.ErrorObj
    '        Dim VnumberOfLinks As String
    '        If String.IsNullOrWhiteSpace(Request.QueryString("Page")) OrElse (Session("OrderEnquiryDetails") Is Nothing) Then
    '            Dim CustNumberPayRefShouldMatch As Boolean = False
    '            If _isTicketCollection Then
    '                CustNumberPayRefShouldMatch = True
    '            End If

    '            Dim order As New Talent.Common.TalentOrder
    '            Dim settings As New Talent.Common.DESettings


    '            Dim moduleDefaults As New ECommerceModuleDefaults
    '            Dim def As ECommerceModuleDefaults.DefaultValues
    '            def = moduleDefaults.GetDefaults()
    '            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
    '            settings.BusinessUnit = TalentCache.GetBusinessUnit()
    '            settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
    '            If Profile.IsAnonymous Then
    '                If (Session("filterDetailsCustomer") IsNot Nothing) Then
    '                    settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(Session("filterDetailsCustomer"), 12)
    '                ElseIf (Session("filterPrintCustomer") IsNot Nothing) AndAlso (Not String.IsNullOrWhiteSpace(Session("filterPrintCustomer"))) Then
    '                    settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(Session("filterPrintCustomer"), 12)
    '                ElseIf (Session("filterPrintCustomer") IsNot Nothing) AndAlso (String.IsNullOrWhiteSpace(Session("filterPrintCustomer"))) Then
    '                    If CustNumberPayRefShouldMatch Then
    '                        settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(GlobalConstants.GENERIC_CUSTOMER_NUMBER, 12)
    '                    Else
    '                        settings.AccountNo1 = ""
    '                    End If
    '                Else
    '                    settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(GlobalConstants.GENERIC_CUSTOMER_NUMBER, 12)
    '                End If
    '            Else
    '                CustNumberPayRefShouldMatch = True
    '                settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)
    '            End If

    '            'is it transaction history enquiry
    '            If _isTransactionEnquiry AndAlso txtPayRef.Text.Trim.Length > 0 Then
    '                order.Dep.PaymentReference = Talent.Common.Utilities.PadLeadingZeros(txtPayRef.Text.Trim, 15)
    '            Else
    '                order.Dep.PaymentReference = Talent.Common.Utilities.PadLeadingZeros("", 15)
    '            End If
    '            order.Dep.CustNumberPayRefShouldMatch = CustNumberPayRefShouldMatch

    '            If Talent.eCommerce.Utilities.IsAgent Then
    '                settings.Cacheing = False
    '                settings.OriginatingSource = AgentProfile.Name
    '            Else
    '                If CInt(order.Dep.PaymentReference) <= 0 Then
    '                    settings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
    '                    settings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
    '                    settings.CacheDependencyPath = def.CacheDependencyPath
    '                    settings.AuthorityUserProfile = def.AuthorityUserProfile
    '                End If
    '            End If

    '            order.Settings() = settings
    '            err = order.OrderEnquiryDetails()
    '            ds1 = order.ResultDataSet()
    '            Session("OrderEnquiryDetails") = ds1
    '        Else
    '            ds1 = CType(Session("OrderEnquiryDetails"), DataSet)
    '        End If


    '        'is the cancel by payment ref button valid.
    '        CancelByPayRefActive()

    '        If Not Err.HasError Then
    '            If Session("filterValue") <> "" Then
    '                If Status.Items.Count <> 0 Then
    '                    Status.SelectedValue = Session("filterValue")
    '                Else
    '                    loadStatus()
    '                    Status.SelectedValue = Session("filterValue")
    '                End If
    '            Else
    '                loadStatus()
    '            End If
    '            Dim newDV As DataView
    '            newDV = setDSProductDate(ds1.Tables(_cProductTable).DefaultView.Table)
    '            objPds.DataSource = newDV
    '            objPds.AllowPaging = True

    '            'Get amount of products to be displayed on a single page from the table tbl_control_attribute
    '            objPds.PageSize = _ucr.Attribute("numberOfProductsVariable")

    '            If Status.Text <> "ALL" Then
    '                OrderHistoryRepeater.DataSource = objPds
    '                OrderHistoryRepeater.DataBind()
    '            End If

    '            If pCurrentPage <> "" Then
    '                CurPage = pCurrentPage
    '            Else
    '                CurPage = 1
    '            End If

    '            objPds.CurrentPageIndex = CurPage - 1
    '            CPage.Text = CurPage.ToString()

    '            vPerpage = objPds.PageSize
    '            CurRec = CurPage * vPerpage
    '            displayingValueLabelT.Text = CurRec - vPerpage + 1

    '            'vTotalRec = ds1.Tables(_cProductTable).DefaultView.Count
    '            vTotalRec = newDV.Count
    '            If vTotalRec = 0 Then
    '                displayingValueLabelT.Text = 0
    '            End If
    '            ofValueLabelT.Text = vTotalRec.ToString()
    '            If CurRec > vTotalRec Then
    '                toValueLabelT.Text = vTotalRec
    '            Else
    '                toValueLabelT.Text = CurRec
    '            End If

    '            Dim linktxt = New String("")

    '            'add link buttons
    '            totLinks = Fix(vTotalRec / objPds.PageSize)

    '            If vTotalRec Mod objPds.PageSize <> 0 Then
    '                totLinks = totLinks + 1
    '            End If
    '            VnumberOfLinks = _ucr.Attribute("numberOfLinks")
    '            If totLinks > VnumberOfLinks Then
    '                totLinks = VnumberOfLinks
    '            End If
    '            If Status.Text <> "ALL" Then
    '                OrderHistoryRepeater.DataSource = objPds
    '                OrderHistoryRepeater.DataBind()
    '            Else
    '                If Status.Text = "ALL" Then
    '                    ds1.Tables(_cProductTable).DefaultView.RowFilter = ""
    '                End If
    '            End If

    '            If totLinks > 1 Then
    '                If Not objPds.IsFirstPage Then LnkPrevT.NavigateUrl = Request.CurrentExecutionFilePath + "?Page=" + Convert.ToString(CurPage - 1)

    '                If Not objPds.IsLastPage Then LnkNextT.NavigateUrl = Request.CurrentExecutionFilePath + "?Page=" + Convert.ToString(CurPage + 1)

    '                vTotalPages = vTotalRec / vPerpage
    '                '----------------------------------------------------------
    '                'List out the pages, current page does not have a hyperlink
    '                '----------------------------------------------------------

    '                If vTotalPages <= totLinks OrElse CurPage <= totLinks Then
    '                    For I As Integer = 1 To totLinks
    '                        If I <> CurPage Then
    '                            linktxt = linktxt + " " + "<a href='" + Request.CurrentExecutionFilePath + "?Page=" + I.ToString() + "'>" + I.ToString() + "</a>"
    '                        Else
    '                            linktxt = linktxt + " " + I.ToString() + " "
    '                        End If
    '                    Next
    '                ElseIf CurPage > (vTotalPages - totLinks) Then
    '                    '--------------------------------------------------
    '                    'display the last 'number_of_links number' of links
    '                    '--------------------------------------------------
    '                    For I As Integer = (vTotalPages - totLinks) To vTotalPages
    '                        If I = CurPage Then
    '                            linktxt = linktxt + " " + I.ToString() + " "
    '                        Else
    '                            linktxt = linktxt + " " + "<a href='" + Request.CurrentExecutionFilePath + "?Page=" + I.ToString() + "'>" + I.ToString() + "</a>"
    '                        End If
    '                    Next
    '                Else
    '                    '---------------------------------
    '                    ' Display the current page halfway
    '                    '---------------------------------
    '                    Dim halfWay As Integer = totLinks / 2
    '                    For I As Integer = 0 To halfWay - 1
    '                        linktxt = linktxt + " " + "<a href='" + Request.CurrentExecutionFilePath + "?Page=" + (CurPage - halfWay + I).ToString() + "'>" + (CurPage - halfWay + I).ToString() + "</a>"
    '                        '  Str = Str() & " <a href=""" & linkPage & "&page=" & (_PageNumber - halfWay + counter) & """>" & (_PageNumber - halfWay + counter) & "</a> "
    '                    Next
    '                    linktxt = linktxt & " " & CurPage & " "
    '                    For I As Integer = 1 To halfWay
    '                        linktxt = linktxt & " <a href=""" & Request.CurrentExecutionFilePath & "?Page=" & (CurPage + I) & """>" & (CurPage + I) & "</a> "
    '                    Next
    '                End If
    '            Else
    '                LnkPrevT.Visible = False
    '                LnkNextT.Visible = False
    '                If totLinks <> 0 Then
    '                    linktxt = " 1 "
    '                Else
    '                    linktxt = " 0 "
    '                End If

    '            End If
    '            LinksLabelT.Text = linktxt

    '            'Populate Order Return Figures
    '            If ds1.Tables.Count > 1 Then

    '                If ds1.Tables(1).Rows(0).Item("BankedTotal") <> Nothing And _
    '                    ds1.Tables(1).Rows(0).Item("BankedTotal").ToString.Trim <> String.Empty Then
    '                    orderReturnBanked.Text = ds1.Tables(1).Rows(0).Item("BankedTotal").ToString.Trim
    '                End If

    '                If ds1.Tables(1).Rows(0).Item("RewardTotal") <> Nothing And _
    '                    ds1.Tables(1).Rows(0).Item("RewardTotal").ToString.Trim <> String.Empty Then
    '                    orderReturnReward.Text = ds1.Tables(1).Rows(0).Item("RewardTotal").ToString.Trim
    '                End If

    '                If orderReturnReward.Text = "0.00" And orderReturnBanked.Text = "0.00" Then
    '                    orderReturnContentWrap.Visible = False
    '                    noOrderReturnWrap.Visible = True
    '                Else
    '                    orderReturnContentWrap.Visible = True
    '                    noOrderReturnWrap.Visible = False
    '                End If
    '            End If

    '            'Populate Cashback Figures
    '            If ds1.Tables.Count > 1 Then

    '                If ds1.Tables(1).Rows(0).Item("CashbackBankedTotal") <> Nothing And _
    '                    ds1.Tables(1).Rows(0).Item("CashbackBankedTotal").ToString.Trim <> String.Empty Then
    '                    cashbackBanked.Text = ds1.Tables(1).Rows(0).Item("CashbackBankedTotal").ToString.Trim
    '                End If

    '                If ds1.Tables(1).Rows(0).Item("CashbackUnbankedTotal") <> Nothing And _
    '                    ds1.Tables(1).Rows(0).Item("CashbackUnbankedTotal").ToString.Trim <> String.Empty Then
    '                    cashbackUnbanked.Text = ds1.Tables(1).Rows(0).Item("CashbackUnbankedTotal").ToString.Trim
    '                End If

    '                If ds1.Tables(1).Rows(0).Item("CashbackSpentTotal") <> Nothing And _
    '                    ds1.Tables(1).Rows(0).Item("CashbackSpentTotal").ToString.Trim <> String.Empty Then
    '                    cashbackSpent.Text = ds1.Tables(1).Rows(0).Item("CashbackSpentTotal").ToString.Trim
    '                End If

    '                If cashbackBanked.Text = "0.00" And cashbackUnbanked.Text = "0.00" And cashbackSpent.Text = "0.00" Then
    '                    cashbackContentWrap.Visible = False
    '                    noCashbackWrap.Visible = True
    '                Else
    '                    cashbackContentWrap.Visible = True
    '                    noCashbackWrap.Visible = False
    '                End If
    '                BindPaymentOwnerAndPurchaseDetails(ds1.Tables("PaymentOwnerDetails"))
    '                BindPaymentDetails(ds1.Tables("PaymentDetails"))
    '            End If
    '            If objPds.Count = 0 Then
    '                showError("NORECORDFOUND")
    '            End If
    '        Else
    '            If ds1.Tables("StatusResults").Rows(0).Item("ErrorOccurred").ToString = "E" Then
    '                showError(ds1.Tables("StatusResults").Rows(0).Item("ReturnCode").ToString())
    '            End If
    '        End If
    '    Catch ex As Exception
    '        validErr = True
    '    End Try
    '    Return validErr
    'End Function

    'Protected Function setDSProductDate(ByVal DT As DataTable) As DataView
    '    Dim DTNew As New DataTable("test")
    '    Dim DC1 As New System.Data.DataColumn()
    '    Dim tDate As New DateTime()
    '    Dim cultureUS = New CultureInfo("en-US", True)

    '    Dim toDt As DateTime
    '    Dim fromDt As DateTime
    '    Dim sfilter As String
    '    '

    '    For Each col As DataColumn In DT.Columns
    '        If col.ColumnName = "SaleDate" Then
    '            DTNew.Columns.Add(col.ColumnName, Type.GetType("System.String"))
    '        Else
    '            DTNew.Columns.Add(col.ColumnName, col.DataType)
    '        End If
    '    Next

    '    Dim myRow As DataRow
    '    For Each rw As DataRow In DT.Rows
    '        myRow = DTNew.NewRow
    '        For Each col As DataColumn In DTNew.Columns
    '            If col.ColumnName = "SaleDate" Then
    '                myRow(col.ColumnName) = "#" & rw(col.ColumnName) & "#"
    '            Else
    '                myRow(col.ColumnName) = rw(col.ColumnName)
    '            End If
    '        Next
    '        DTNew.Rows.Add(myRow)
    '    Next

    '    sfilter = ""
    '    ToErr.Text = ""
    '    FromErr.Text = ""
    '    If FromDate.Text <> "" Then
    '        If ToDate.Text <> "" Then
    '            fromDt = DateTime.ParseExact(FromDate.Text, "dd/MM/yyyy", cultureUS)
    '            toDt = DateTime.ParseExact(ToDate.Text, "dd/MM/yyyy", cultureUS)
    '            If toDt < fromDt Then
    '                ToErr.Text = "From Date must be before To Date."
    '                If Status.Text <> "ALL" Then
    '                    sfilter = "StatusCode='" + Status.Text + "'"
    '                End If
    '            Else
    '                ToErr.Text = ""
    '                sfilter = "SaleDate >=#" + fromDt.ToString("MM/dd/yyyy") + "# and SaleDate <=#" + toDt.ToString("MM/dd/yyyy") + "#"
    '                If Status.Text <> "ALL" Then
    '                    sfilter = sfilter + " AND StatusCode='" + Status.Text + "'"
    '                End If
    '            End If
    '        Else
    '            If ToDate.Text = "" And ToErr.Text = "" Then
    '                ToErr.Text = "Please select From Date."
    '            End If
    '            If Status.Text <> "ALL" Then
    '                sfilter = "StatusCode='" + Status.Text + "'"
    '            End If
    '        End If
    '    Else
    '        ' FromErr.Text = "Please select From Date."
    '        If Status.Text <> "ALL" Then
    '            sfilter = "StatusCode='" + Status.Text + "'"
    '        End If
    '    End If

    '    If promoSelectWrap.Visible Then
    '        If Not String.IsNullOrEmpty(promoDDL.SelectedValue) Then
    '            If Not String.IsNullOrEmpty(sfilter) Then
    '                sfilter += " AND PromotionID = '" & promoDDL.SelectedValue & "'"
    '            Else
    '                sfilter += "PromotionID = '" & promoDDL.SelectedValue & "'"
    '            End If
    '        End If
    '    End If

    '    If txtPayRef.Visible Then
    '        If txtPayRef.Text.Trim.Length > 0 Then
    '            If Not String.IsNullOrEmpty(sfilter) Then
    '                sfilter += " AND PaymentReference = '" & txtPayRef.Text.Trim.PadLeft(15, "0") & "'"
    '            Else
    '                sfilter += "PaymentReference = '" & txtPayRef.Text.Trim.PadLeft(15, "0") & "'"
    '            End If
    '        End If
    '    End If

    '    DTNew.DefaultView.RowFilter = sfilter

    '    If promoCurrentWrap.Visible AndAlso Not String.IsNullOrEmpty(promoDDL.SelectedValue) Then
    '        current.Text = DTNew.DefaultView.Count
    '    Else
    '        current.Text = ""
    '    End If
    '    DTNew.DefaultView.Sort = "PaymentReference Desc"

    '    Return DTNew.DefaultView
    'End Function

#End Region
#End Region

End Class

#Region " Class to list Distinct items for a given DataTable updated to show ProductCompetition details"
Public Class DataSetHelperOrder

    Public ds As DataSet
    Public ucr As New Talent.Common.UserControlResource

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Public Sub New(ByRef DataSet As DataSet)
        ds = DataSet
    End Sub

    Public Sub New()
        ds = Nothing
    End Sub

    Public Function SelectDistinct(ByVal TableName As String, ByVal SourceTable As DataTable, ByVal TextFieldName As String, ByVal TextFieldType As Type, ByVal ValueFieldName As String, ByVal ValueFieldType As Type) As DataTable

        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "OrderEnquiry2.ascx"
        End With

        Dim dt As New DataTable(TableName)
        Dim LastValue As Object = Nothing
        Dim sourceDT As New DataTable
        Dim strStatus As String = String.Empty

        dt.Columns.Add(TextFieldName, TextFieldType)
        dt.Columns.Add(ValueFieldName, ValueFieldType)
        SourceTable.DefaultView.RowFilter = String.Empty
        SourceTable.DefaultView.Sort = ValueFieldName
        sourceDT = SourceTable.DefaultView.ToTable()

        For Each dr As DataRow In sourceDT.[Select]("", ValueFieldName)
            If LastValue Is Nothing OrElse Not (ColumnEqual(LastValue, dr(ValueFieldName).ToString.Trim)) Then
                LastValue = dr(ValueFieldName).ToString.Trim
                strStatus = ucr.Content("OrderEnquiryProductStatus" + dr(ValueFieldName).ToString.Trim, _languageCode, True)
                If strStatus.Trim <> "" Then
                    dt.Rows.Add(strStatus, dr(ValueFieldName).ToString.Trim)
                End If
            End If
        Next

        Return dt

    End Function

    Private Function ColumnEqual(ByVal A As Object, ByVal B As Object) As Boolean


        ' Compares two values to see if they are equal. Also compares DBNULL.Value.
        ' Note: If your DataTable contains object fields, then you must extend this
        ' function to handle them in a meaningful way if you intend to group on them.

        If A.Equals(DBNull.Value) AndAlso B.Equals(DBNull.Value) Then
            Return True
            '  both are DBNull.Value
        End If
        If A.Equals(DBNull.Value) OrElse B.Equals(DBNull.Value) Then
            Return False
            '  only one is DBNull.Value
        End If
        Return (A.Equals(B))
        ' value type standard comparison
    End Function
End Class
#End Region