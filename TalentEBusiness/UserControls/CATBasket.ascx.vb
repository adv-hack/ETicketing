Imports System.Data
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.CATHelper
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Partial Class UserControls_CATBasket
    Inherits ControlBase

#Region "Class Level Fields"

    Public ComponentQuantityAddColHeader As String = String.Empty
    Public ComponentQuantityDelColHeader As String = String.Empty
    Public ComponentQuantityAmdColHeader As String = String.Empty
    Public ComponentColHeader As String = String.Empty
    Public ComponentPriceColHeader As String = String.Empty
    Private _ucr As UserControlResource
    Private _moduleDefaults As ECommerceModuleDefaults
    Private _moduleDefaultsValue As ECommerceModuleDefaults.DefaultValues
    Private _languageCode As String = String.Empty
    Private _mode As String = String.Empty
    Private _modeText As String = String.Empty
    Private _isTransactionEnquiry As Boolean = False
    Private _paymentReference As String = String.Empty
    Private _paymentDate As String = String.Empty
    Private _seat As String = String.Empty
    Private _matchCode As String = String.Empty
    Private _customerNumber As String = String.Empty
    Private _productType As String = String.Empty
    Private _stadiumCode As String = String.Empty
    Private _productDetails As Generic.List(Of String)
    Private _profileAccountNo1 As String = String.Empty
    Private _profileLoginID As String = String.Empty
    Private _isCATOnPackage As Boolean = False
    Private _isCATOnBulkSale As Boolean = False
    Private _bulkSalesID As Integer = 0
    Private _packageID As Decimal = 0
    Private _callID As String = String.Empty
    Private _dtCatModeCancelMultiple As DataTable
    Private _CATMODE_CANCELMULTIPLE_SESSION_KEY As String = "CATMODE_CANCELMULTIPLE" & TalentCache.GetBusinessUnit
    Private _directDebitPaymentsMadeValue As Decimal = 0
    Private _DIRECT_DEBIT_PAYMENTS_MADE_SESSION_KEY As String = String.Empty
    Private _packageDescription As String = String.Empty
#End Region

#Region "Public Property"
    Public Property CatSeatsPrice() As Decimal = 0
#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _moduleDefaults = New ECommerceModuleDefaults
        _moduleDefaultsValue = _moduleDefaults.GetDefaults()
        _ucr = New UserControlResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _productDetails = New Generic.List(Of String)
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = TEBUtilities.GetCurrentPageName
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "CATBasket.ascx"
        End With

        If HttpContext.Current.Session.Item(_CATMODE_CANCELMULTIPLE_SESSION_KEY) IsNot Nothing Then
            _dtCatModeCancelMultiple = HttpContext.Current.Session.Item(_CATMODE_CANCELMULTIPLE_SESSION_KEY)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Profile.IsAnonymous AndAlso AgentProfile.IsAgent Then
            _profileAccountNo1 = GlobalConstants.GENERIC_CUSTOMER_NUMBER
            _profileLoginID = GlobalConstants.GENERIC_CUSTOMER_NUMBER
        ElseIf Not Profile.IsAnonymous Then
            _profileAccountNo1 = Profile.User.Details.Account_No_1
            _profileLoginID = Profile.User.Details.LoginID
        End If
        If IsValidQuerystringValues() Or _dtCatModeCancelMultiple IsNot Nothing AndAlso TEBUtilities.GetCurrentPageName.ToUpper = "CATCONFIRM.ASPX" Then
            SetPackageSummaryLabels()
            If Not Page.IsPostBack Then
                If _mode = GlobalConstants.CATMODE_CANCELMULTIPLE AndAlso AgentProfile.BulkSalesMode Then
                    DisplayYouWillBeTakenOutOfBulkMode("YouWillBeTakenOutOfBulkMode")
                End If
                If _isCATOnPackage AndAlso _mode = GlobalConstants.CATMODE_CANCEL Then
                    SetPackageSummaryCancel()
                ElseIf _isCATOnPackage Then
                    SetPackageSummaryAmendOrTransfer()
                Else
                    uscPackageSummaryCancel.Display = False
                    plhPackageSummaryAmendOrTransfer.Visible = False
                    BindOrderDetailsRepeater()
                End If
                SetLabelValues()
                ControlVisibilityByPageName()
            End If
        Else
            Me.Visible = False
        End If
    End Sub

    Protected Sub rptOrderDetails_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptOrderDetails.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim orderDetailRow As DataRow = CType(e.Item.DataItem, DataRowView).Row
            CatSeatsPrice += TEBUtilities.CheckForDBNull_Decimal(orderDetailRow("SalePrice"))
            If String.IsNullOrWhiteSpace(_paymentDate) Then
                _paymentDate = e.Item.DataItem("SaleDate").ToString
            End If
            Dim plhSeatColumn As PlaceHolder = CType(e.Item.FindControl("plhSeatColumn"), PlaceHolder)
            Dim plhQuantityColumn As PlaceHolder = CType(e.Item.FindControl("plhQuantityColumn"), PlaceHolder)
            plhQuantityColumn.Visible = _isCATOnBulkSale
            plhSeatColumn.Visible = Not _isCATOnBulkSale
        End If
    End Sub

    Protected Sub rptOrderDetails_PreRender(sender As Object, e As EventArgs) Handles rptOrderDetails.PreRender
        If rptOrderDetails.Controls(0).Controls(0).FindControl("plhSeatColumn") IsNot Nothing Then
            CType(rptOrderDetails.Controls(0).Controls(0).FindControl("plhSeatColumn"), PlaceHolder).Visible = Not _isCATOnBulkSale
        End If
        If rptOrderDetails.Controls(0).Controls(0).FindControl("plhQuantityColumn") IsNot Nothing Then
            CType(rptOrderDetails.Controls(0).Controls(0).FindControl("plhQuantityColumn"), PlaceHolder).Visible = _isCATOnBulkSale
        End If
    End Sub

    Protected Sub btnYes_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnYes.Click
        Session("CATBasketOrderDetails") = Nothing
        Dim redirectURL As String = String.Empty

        'clear front end basket
        Profile.Basket.EmptyBasket()

        If _mode <> GlobalConstants.CATMODE_TRANSFER Then
            Dim functionToCall As String = "addtobasket"
            If Not String.IsNullOrWhiteSpace(_callID) AndAlso IsNumeric(_callID) AndAlso CLng(_callID) > 0 Then
                functionToCall = "packageaddtobasket"
            End If
            'this call will clear backend basket and add the items so one call to backend will do both
            redirectURL = "../../Redirect/TicketingGateway.aspx" &
                    "?page=catconfirm.aspx" &
                    "&function=" & functionToCall &
                    "&catmode=" & _mode &
                    "&product=" & _matchCode &
                    "&catseat=" & _seat &
                    "&payref=" & _paymentReference &
                    "&istrnxenq=" & _isTransactionEnquiry.ToString &
                    "&catseatcustomerno=" & _customerNumber &
                    "&callid=" & _callID &
                    "&bulksalesid=" & _bulkSalesID
        Else
            'this call will clears backend basket
            Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
            ticketingGatewayFunctions.Basket_ClearBasket(False)
            'check any error occurs on above call, if yes redirect to basket page
            If HttpContext.Current.Session("TicketingGatewayError") IsNot Nothing OrElse HttpContext.Current.Session("TalentErrorCode") IsNot Nothing Then
                Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            End If

            redirectURL = GetTransferSeatSelectionURL(_stadiumCode, _matchCode, "", _productType, "", "")

            redirectURL = redirectURL &
                    "&catmode=" & HttpUtility.UrlEncode(GlobalConstants.CATMODE_TRANSFER) &
                "&catseat=" & HttpUtility.UrlEncode(_seat) &
                "&payref=" & _paymentReference &
                "&istrnxenq=" & _isTransactionEnquiry.ToString &
                "&catseatcustomerno=" & _customerNumber &
                "&callid=" & _callID
            Session("catmode") = GlobalConstants.CATMODE_TRANSFER
            Session("product") = _matchCode
            Session("catseat") = _seat
            Session("calid") = _callID
        End If
        Response.Redirect(redirectURL)
    End Sub

    Protected Sub btnNo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNo.Click
        Session("CATBasketOrderDetails") = Nothing
        Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
    End Sub

    ''' <summary>
    ''' Checks the given seatDetails string to see whether it is a loyalty points attendance record rather than an actual stadium seat.
    ''' Eg. seatDetails = "E/EA/003/0004" is a seat, "*AT/TEND" is an attendance record in the purchase history.
    ''' </summary>
    ''' <param name="seatDetails">the given seat details passed from WS005R</param>
    ''' <param name="productType">the given product type passed from WS005R</param>
    ''' <param name="allocatedSeat">The allocated seat details from WS005R</param>
    ''' <returns>The seat if it is a valid seat, or a string taken from the SQL database based on a given ATTR_VALUE</returns>
    ''' <remarks></remarks>
    Protected Function CheckSeatDetailsForAttendance(ByVal seatDetails As String, ByVal productType As String, ByVal allocatedSeat As String) As String
        Dim returnValue As String = String.Empty
        Try
            Dim attendanceString As String = _ucr.Attribute("LoyaltyAttendanceCode")
            If seatDetails = attendanceString Then
                returnValue = _ucr.Content("LoyaltyAttendanceDisplayText", _languageCode, True)
            Else
                returnValue = TEBUtilities.GetFormattedSeatForDisplay(seatDetails, productType, allocatedSeat)
            End If
            If Not String.IsNullOrWhiteSpace(productType) AndAlso productType = "E" Then
                returnValue = String.Empty
            End If
        Catch ex As Exception
            returnValue = String.Empty
        End Try
        Return returnValue
    End Function

    Protected Function GetText(ByVal PValue As String) As String
        Dim str As String = _ucr.Content(PValue, _languageCode, True)
        Return str
    End Function

    Protected Function GetStatusText(ByVal PValue As String) As String
        Dim str As String = "" 'ucr.Content(PValue, _languageCode, True)
        If String.IsNullOrEmpty(str) Then
            str = PValue
        End If
        Return str
    End Function

    Protected Function showDate(ByVal Pvalue As String) As String
        Dim str As String
        str = Pvalue.Replace("#", "")
        Return str
    End Function

#End Region

#Region "Private Methods"

    Private Function GetTransferSeatSelectionURL(ByVal stadiumCode As String, ByVal productCode As String, ByVal campaignCode As String, _
                                               ByVal productType As String, ByVal productSubType As String, ByVal productHomeAsAway As String) As String
        Dim redirectUrl As String = GetCATTransferSeatSelectionURL(TDataObjects.StadiumSettings.TblStadiums.GetStadiumNameByStadiumCode(stadiumCode, _ucr.BusinessUnit),
                                                                   stadiumCode, productCode, campaignCode, productType, productSubType, productHomeAsAway)
        Return ResolveUrl(redirectUrl)
    End Function

    Private Function IsValidQuerystringValues() As Boolean
        Dim isValidQuery As Boolean = True
        Dim currentPageName As String = TEBUtilities.GetCurrentPageName.ToUpper
        Select Case currentPageName
            Case "BASKET.ASPX", "CHECKOUTORDERCONFIRMATION.ASPX"
                If Not String.IsNullOrWhiteSpace(Profile.Basket.CAT_MODE) Then
                    AssignCATMode()
                    AssignModeText()
                    DisplayHeaderAndInstText("HeaderText_" & _modeText, "InstructionTextTop_" & _modeText)
                    If Not Session("cancelMultipleBulkID") Is Nothing Then
                        _bulkSalesID = Session("cancelMultipleBulkID")
                    End If
                Else
                    isValidQuery = False
                    Me.Visible = False
                    plhCATDetails.Visible = False
                End If
            Case "CATCONFIRM.ASPX"
                If Request.UrlReferrer IsNot Nothing Then
                    uscPackageSummaryCancel.Display = False
                    uscPackageSummaryCancel.Visible = False
                    Session("CATBasketOrderDetails") = Nothing
                    _mode = GetQuerystringValue("Mode")
                    _paymentReference = GetQuerystringValue("PayRef")
                    _seat = GetQuerystringValue("Seat")
                    _matchCode = GetQuerystringValue("MatchCode")
                    _customerNumber = GetQuerystringValue("CustomerNumber")
                    _productType = GetQuerystringValue("ProductType")
                    _stadiumCode = GetQuerystringValue("StadiumCode")
                    _callID = GetQuerystringValue("CallId")
                    _bulkSalesID = TEBUtilities.CheckForDBNull_Int(GetQuerystringValue("bulksalesid"))
                    _isTransactionEnquiry = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(GetQuerystringValue("TrnEnq"))
                    For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                        If UCase(tbi.MODULE_) = "TICKETING" AndAlso Not tbi.IS_FREE Then
                            If Not TEBUtilities.IsTicketingFee(tbi.MODULE_, tbi.Product.Trim, tbi.FEE_CATEGORY) Then
                                If AgentProfile.IsAgent AndAlso tbi.BULK_SALES_ID > 0 Then
                                    _isCATOnBulkSale = True
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                    If String.IsNullOrWhiteSpace(_mode) Then
                        isValidQuery = False
                        ProcessErrorMessage(False, "ErrMess_ParamMissing", False)
                    ElseIf _mode = GlobalConstants.CATMODE_CANCELALL Then
                        If String.IsNullOrWhiteSpace(_paymentReference) Then
                            isValidQuery = False
                            ProcessErrorMessage(False, "ErrMess_ParamMissing", False)
                        End If  
                    ElseIf _mode = GlobalConstants.CATMODE_CANCELMULTIPLE Then
                        If _dtCatModeCancelMultiple Is Nothing Then
                            isValidQuery = False
                            ProcessErrorMessage(False, "ErrMess_ParamMissing", False)
                        End If
                        If String.IsNullOrWhiteSpace(_paymentReference) Then
                            isValidQuery = False
                            ProcessErrorMessage(False, "ErrMess_ParamMissing", False)
                        End If
                    Else
                        If String.IsNullOrWhiteSpace(_callID) OrElse _callID = "0" Then
                            If String.IsNullOrWhiteSpace(_paymentReference) _
                                                        OrElse (String.IsNullOrWhiteSpace(_seat) AndAlso _bulkSalesID = 0) _
                                                        OrElse String.IsNullOrWhiteSpace(_matchCode) _
                                                        OrElse String.IsNullOrWhiteSpace(_customerNumber) _
                                                        OrElse String.IsNullOrWhiteSpace(_productType) _
                                                        OrElse (_mode = GlobalConstants.CATMODE_TRANSFER AndAlso String.IsNullOrWhiteSpace(_stadiumCode)) Then
                                isValidQuery = False
                                ProcessErrorMessage(False, "ErrMess_ParamMissing", False)
                            End If
                        Else
                            If String.IsNullOrWhiteSpace(_paymentReference) _
                                                        OrElse String.IsNullOrWhiteSpace(_matchCode) _
                                                        OrElse String.IsNullOrWhiteSpace(_customerNumber) _
                                                        OrElse String.IsNullOrWhiteSpace(_productType) _
                                                        OrElse (_mode = GlobalConstants.CATMODE_TRANSFER AndAlso String.IsNullOrWhiteSpace(_stadiumCode)) Then
                                isValidQuery = False
                                ProcessErrorMessage(False, "ErrMess_ParamMissing", False)
                            Else
                                _isCATOnPackage = True
                            End If
                        End If

                        'TrnEnq=True&PayRef=23034&MatchCode=F1TBND&CustomerNumber=000000010740&ProductType=S&StadiumCode=SP&CallId=827&Mode=C
                    End If
                    If isValidQuery Then
                        AssignModeText()
                        DisplayHeaderAndInstText("HeaderText_" & _modeText, "InstructionTextTop_" & _modeText)
                    End If
                Else
                    isValidQuery = False
                    ProcessErrorMessage(False, "ErrMess_ParamMissing", False)
                End If
            Case Else
                isValidQuery = False
        End Select

        Return isValidQuery
    End Function

    Private Function GetQuerystringValue(ByVal QueryStringKey As String) As String
        Dim queryValue As String = String.Empty
        If Not String.IsNullOrWhiteSpace(Request.QueryString(QueryStringKey)) Then
            queryValue = Request.QueryString(QueryStringKey).Trim
        End If
        Return queryValue
    End Function

    Private Sub BindOrderDetailsRepeater()
        Dim isSeatFound As Boolean = False
        isSeatFound = isCATSeatFound(_profileAccountNo1)
        If Not isSeatFound Then
            If _paymentReference.Trim.Length > 0 Then
                _isTransactionEnquiry = True
                isSeatFound = isCATSeatFound(_profileAccountNo1)
            End If
        End If
        If (Not isSeatFound) AndAlso (Not Profile.IsAnonymous) AndAlso (_mode = GlobalConstants.CATMODE_TRANSFER OrElse _mode = GlobalConstants.CATMODE_AMEND) Then
            isSeatFound = isCATSeatFound(GlobalConstants.GENERIC_CUSTOMER_NUMBER)
        End If
        If Not isSeatFound Then
            'for given filter condition seat row not found
            If _mode = GlobalConstants.CATMODE_CANCELALL Then
                ProcessErrorMessage(False, "ErrMess_NoActiveSeat_" & _modeText, False)
            Else
                ProcessErrorMessage(False, "ErrMess_InValid", False)
            End If
        End If
    End Sub

    Private Function isCATSeatFound(ByVal profileAccountNo1 As String) As Boolean
        Dim isSeatFound As Boolean = False
        Dim dtOrderDetails As DataTable = GetSeatStatus(profileAccountNo1)
        If (dtOrderDetails IsNot Nothing) Then
            If dtOrderDetails.Rows.Count > 0 Then
                Dim dvOrderDetails As DataView = dtOrderDetails.DefaultView
                dtOrderDetails.DefaultView.RowFilter = GetRowFilterCondition()
                If dtOrderDetails.DefaultView.Count > 0 Then
                    rptOrderDetails.DataSource = dtOrderDetails.DefaultView
                    rptOrderDetails.DataBind()
                    isSeatFound = True
                End If
            End If
        End If
        Return isSeatFound
    End Function

    Private Function GetSeatStatus(ByVal profileAccountNo1 As String) As DataTable
        Dim dsOrderHistory As New DataSet
        Dim dtOrderDetails As DataTable = Nothing
        Try
            If Session("CATBasketOrderDetails") IsNot Nothing Then
                dtOrderDetails = CType(Session("CATBasketOrderDetails"), DataTable)
            Else
                Dim order As New Talent.Common.TalentOrder
                Dim err As New Talent.Common.ErrorObj
                Dim settings As Talent.Common.DESettings = TEBUtilities.GetSettingsObject()
                settings.AccountNo1 = TCUtilities.PadLeadingZeros(profileAccountNo1, 12)
                settings.Cacheing = False
                order.Dep.BulkSalesID = _bulkSalesID
                If _mode = GlobalConstants.CATMODE_CANCELMULTIPLE And _bulkSalesID > 0 Then
                    order.Dep.ShowDespatchInformation = True
                    order.Dep.BulkSalesFlag = True
                End If
                order.Dep.PaymentReference = TCUtilities.PadLeadingZeros(_paymentReference.Trim, 15)
                order.Dep.CustNumberPayRefShouldMatch = False
                order.Settings() = settings
                err = order.OrderEnquiryDetails()
                If (Not err.HasError) AndAlso (order.ResultDataSet IsNot Nothing) Then
                    dsOrderHistory = order.ResultDataSet()
                    If dsOrderHistory.Tables.Count > 1 Then
                        dtOrderDetails = dsOrderHistory.Tables("OrderEnquiryDetails")
                        Session("CATBasketOrderDetails") = dtOrderDetails
                    Else
                        'order detail table is missing or error
                        If dsOrderHistory.Tables.Count > 0 AndAlso dsOrderHistory.Tables("StatusResults").Rows.Count > 0 Then
                            ProcessErrorMessage(True, TEBUtilities.CheckForDBNull_String(dsOrderHistory.Tables("StatusResults").Rows(0)("ReturnCode")), False)
                        Else
                            'status table not having any records consider unexpected error
                            ProcessErrorMessage(False, "ErrMess_NoTableFound", False)
                        End If
                    End If
                Else
                    ProcessErrorMessage(False, "ErrMess_UnExpected", False)
                End If
            End If
        Catch ex As Exception
            ProcessErrorMessage(False, "ErrMess_Exception", False)
        End Try
        Return dtOrderDetails
    End Function

    Private Function GetRowFilterCondition() As String
        Dim filterCondition As New StringBuilder
        Dim currentPageName As String = TEBUtilities.GetCurrentPageName.ToUpper
        If currentPageName = "CATCONFIRM.ASPX" Then
            If _mode <> GlobalConstants.CATMODE_CANCELALL Then
                If _mode = GlobalConstants.CATMODE_CANCELMULTIPLE Then
                    If Not _dtCatModeCancelMultiple Is Nothing AndAlso _dtCatModeCancelMultiple.Rows.Count > 0 Then
                        _paymentReference = ""
                        _matchCode = ""
                        Dim lastMatchCode As String = ""
                        Dim lastSeat As String = ""
                        For i = 0 To _dtCatModeCancelMultiple.Rows.Count - 1
                            Dim seat As New DESeatDetails
                            _paymentReference = _dtCatModeCancelMultiple.Rows(i).Item("paymentreference")
                            _matchCode = _dtCatModeCancelMultiple.Rows(i).Item("productCode")
                            seat.UnFormattedSeat = _dtCatModeCancelMultiple.Rows(i).Item("seat")
                            _seat = seat.FormattedSeat
                            If i = 0 Then
                                filterCondition.Append("PaymentReference='" & _paymentReference.PadLeft(15, "0") & "'")
                                filterCondition.Append(" AND StatusCode <> 'CANCEL'")
                                filterCondition.Append(" AND ProductCode='" & _matchCode & "'")
                                filterCondition.Append(" AND Seat='" & _seat & "'")
                                filterCondition.Append(" AND BulkID = 0")
                                lastMatchCode = _matchCode
                                lastSeat = _seat
                            End If
                            If _matchCode <> lastMatchCode Or _matchCode = lastMatchCode AndAlso _seat <> lastSeat Then
                                filterCondition.Append("OR PaymentReference='" & _paymentReference.PadLeft(15, "0") & "'")
                                filterCondition.Append(" AND StatusCode <> 'CANCEL'")
                                filterCondition.Append(" AND ProductCode='" & _matchCode & "'")
                                filterCondition.Append(" AND Seat='" & _seat & "'")
                                filterCondition.Append(" AND BulkID = 0")
                                lastMatchCode = _matchCode
                                lastSeat = _seat
                            End If
                        Next
                    End If
                Else
                    filterCondition.Append("PaymentReference='" & _paymentReference.PadLeft(15, "0") & "'")
                    filterCondition.Append(" AND StatusCode <> 'CANCEL'")
                    filterCondition.Append(" AND ProductCode='" & _matchCode & "'")
                    filterCondition.Append(" AND (Seat='" & _seat & "' OR BulkID ='" & _bulkSalesID & "')")
                End If
            Else
                filterCondition.Append("PaymentReference='" & _paymentReference.PadLeft(15, "0") & "'")
                filterCondition.Append(" AND StatusCode <> 'CANCEL'")
                If Talent.eCommerce.Utilities.IsAgent Then
                    filterCondition.Append(" AND CATCancelAgent='True'")
                Else
                    filterCondition.Append(" AND CATCancelWeb='True'")
                End If
            End If
        Else
            For Each s As String In _productDetails
                If filterCondition.ToString.Length > 0 Then
                    filterCondition.Append(" OR ")
                End If
                filterCondition.Append("(PaymentReference='" & _paymentReference.PadLeft(15, "0") & "'")
                filterCondition.Append(" AND StatusCode <> 'CANCEL'")
                filterCondition.Append(" AND ProductCode='" & s.Substring(0, 6) & "'")
                filterCondition.Append(" AND (Seat='" & s.Substring(6) & "' OR Seat='" & s.Substring(6) & "/')")
                If _mode = GlobalConstants.CATMODE_CANCELMULTIPLE Then
                    filterCondition.Append("AND BulkID = 0")
                End If
                filterCondition.Append(")")
            Next
        End If
        
        Return filterCondition.ToString()
    End Function

    Private Sub AssignCATMode()
        Dim CATSeatCount As Integer = 0
        Dim CATSeatDetails As String = String.Empty
        Dim seat As String = String.Empty
        Dim payRef As String = String.Empty
        _mode = Profile.Basket.CAT_MODE

        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If UCase(tbi.MODULE_) = "TICKETING" AndAlso Not tbi.IS_FREE Then
                If Not TEBUtilities.IsTicketingFee(tbi.MODULE_, tbi.Product.Trim, tbi.FEE_CATEGORY) Then
                    CATSeatCount += 1
                    'is it package
                    If tbi.PACKAGE_ID > 0 AndAlso TEBUtilities.CheckForDBNull_String(tbi.PRODUCT_TYPE_ACTUAL).Trim.ToUpper = "P" Then
                        _isCATOnPackage = True
                        _packageID = tbi.PACKAGE_ID
                        CATSeatDetails = tbi.CAT_SEAT_DETAILS
                        _packageDescription = tbi.PRODUCT_DESCRIPTION6
                    ElseIf tbi.BULK_SALES_ID > 0 Then
                        _isCATOnBulkSale = True
                        _bulkSalesID = tbi.BULK_SALES_ID
                        CATSeatDetails = tbi.CAT_SEAT_DETAILS
                    Else
                        CATSeatDetails = tbi.CAT_SEAT_DETAILS
                        Dim seatArea As String = CATSeatDetails.Substring(9, 4).Trim
                        If String.IsNullOrWhiteSpace(seatArea) Then seatArea = " "
                        Dim tempSeat = CATSeatDetails.Substring(6, 3).Trim & "/" & seatArea & "/" & CATSeatDetails.Substring(13, 4).Trim & "/" & CATSeatDetails.Substring(17, 4).Trim
                        If CATSeatDetails.Substring(21, 1).Trim.Length > 0 Then
                            tempSeat = tempSeat & "/" & CATSeatDetails.Substring(21, 1).Trim
                        End If
                        _productDetails.Add(CATSeatDetails.Substring(0, 6) & tempSeat)
                        'is seat belongs to F&F
                        If Not _isTransactionEnquiry Then
                            If TCUtilities.PadLeadingZeros(_profileAccountNo1, 12) <> TCUtilities.PadLeadingZeros(tbi.LOGINID, 12) Then
                                _isTransactionEnquiry = True
                            ElseIf (TCUtilities.PadLeadingZeros(_profileAccountNo1, 12) = GlobalConstants.GENERIC_CUSTOMER_NUMBER) AndAlso (TCUtilities.PadLeadingZeros(tbi.LOGINID, 12) = GlobalConstants.GENERIC_CUSTOMER_NUMBER) Then
                                _isTransactionEnquiry = True
                            End If
                        End If
                    End If
                End If
            End If
            
            If CATSeatCount >= 2 Then
                _mode = GlobalConstants.CATMODE_CANCELALL
                _isTransactionEnquiry = True
            End If

            If Not _dtCatModeCancelMultiple Is Nothing AndAlso _dtCatModeCancelMultiple.Rows.Count > 0 Then
                _mode = GlobalConstants.CATMODE_CANCELMULTIPLE
            End If

        Next

        If _isCATOnPackage Then
            _matchCode = CATSeatDetails.Substring(0, 6)
            _callID = CATSeatDetails.Substring(6, 13)
            _paymentReference = CATSeatDetails.Substring(22, 15)
        ElseIf _isCATOnBulkSale Then
            _paymentReference = CATSeatDetails.Substring(22, 15)
            If CATSeatCount > 1 Then _bulkSalesID = 0
        Else
            If (Not _isTransactionEnquiry) AndAlso (CATSeatCount = 1) Then
                _matchCode = CATSeatDetails.Substring(0, 6)
                _seat = _productDetails.Item(0).Substring(6)
                _paymentReference = CATSeatDetails.Substring(22, 15)
            Else
                If _isTransactionEnquiry OrElse CATSeatCount > 1 Then
                    _paymentReference = CATSeatDetails.Substring(22, 15)
                End If
            End If
        End If
    End Sub

    Private Sub AssignModeText()
        If _mode = GlobalConstants.CATMODE_TRANSFER Then
            _modeText = "Transfer"
        ElseIf _mode = GlobalConstants.CATMODE_AMEND Then
            _modeText = "Amend"
        ElseIf _mode = GlobalConstants.CATMODE_CANCEL Then
            _modeText = "Cancel"
        ElseIf _mode = GlobalConstants.CATMODE_CANCELALL Then
            _modeText = "CancelAll"
        ElseIf _mode = GlobalConstants.CATMODE_CANCELMULTIPLE Then
            _modeText = "CancelMultiple"
        End If
    End Sub

    Private Sub ControlVisibilityByPageName()
        If TEBUtilities.GetCurrentPageName.ToUpper <> "CATCONFIRM.ASPX" Then
            plhInstructionBottom.Visible = False
            plhMessage.Visible = False
            plhYesOrNoOrOk.Visible = False
        Else
            DisplayInstructionBottom("InstructionTextBottom_" & _modeText)
            lblMessageToUser.Text = _ucr.Content("ProceedFurtherText_" & _modeText, _languageCode, True)
        End If
    End Sub

    Private Sub SetLabelValues()
        btnYes.Text = _ucr.Content("YesButtonText", _languageCode, True)
        btnNo.Text = _ucr.Content("NoButtonText", _languageCode, True)
        ltltransactionRef.Text = _ucr.Content("PayRefLabel", _languageCode, True)
        ltltransactionRefValue.Text = _paymentReference.TrimStart("0")
        If String.IsNullOrWhiteSpace(_paymentDate) Then
            ltltransactionDate.Visible = False
            ltltransactionDateValue.Visible = False
        Else
            ltltransactionDate.Text = _ucr.Content("DateLabel", _languageCode, True)
            ltltransactionDateValue.Text = _paymentDate
        End If

        If Not String.IsNullOrWhiteSpace(_callID) AndAlso IsNumeric(_callID) AndAlso CLng(_callID) > 0 Then
            ltlCallID.Text = _ucr.Content("CallIDLabel", _languageCode, True)
            ltlCallIDValue.Text = _callID.TrimStart("0"c)
        Else
            ltlCallID.Visible = False
            ltlCallIDValue.Visible = False
        End If

        If _packageDescription <> String.Empty Then
            ltlPackageDescription.Text = _ucr.Content("PackageDescriptionLabel", _languageCode, True)
            ltlPackageDescriptionValue.Text = _packageDescription
            plhPackageDescription.Visible = True
        End If

        If _DIRECT_DEBIT_PAYMENTS_MADE_SESSION_KEY = String.Empty Then
            _DIRECT_DEBIT_PAYMENTS_MADE_SESSION_KEY = "_DIRECT_DEBIT_PAYMENTS_MADE_SESSION_KEY" & _paymentReference
        End If
        If Session(_DIRECT_DEBIT_PAYMENTS_MADE_SESSION_KEY) IsNot Nothing Then
            _directDebitPaymentsMadeValue = Session(_DIRECT_DEBIT_PAYMENTS_MADE_SESSION_KEY)
            ltlDirectDebitPaymentsMade.Text = _ucr.Content("DirectDebitPaymentsMadeLabel", _languageCode, True)
            plhDirectDebitPaymentsMade.Visible = True
            ltlDirectDebitPaymentsMadeValue.Text = _directDebitPaymentsMadeValue
            hplDirectDebitSummary.Visible = False
            uscDirectDebitSummary.Visible = False
            If TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("ShowDirectDebitSummary")) AndAlso TEBUtilities.GetCurrentPageName().ToUpper() <> "CHECKOUTORDERCONFIRMATION.ASPX" Then
                hplDirectDebitSummary.Visible = True
                hplDirectDebitSummary.Attributes.Add("data-open", "ebiz-direct-debit-summary-reveal")
                uscDirectDebitSummary.Visible = True
                uscDirectDebitSummary.PaymentRef = _paymentReference
            End If
        Else
            plhDirectDebitPaymentsMade.Visible = False
        End If
    End Sub

    Private Sub DisplayHeaderAndInstText(ByVal headerText As String, ByVal instructionText As String)
        lblCATHeader.Text = _ucr.Content(headerText, _languageCode, True)
        lblCATInstructionTop.Text = _ucr.Content(instructionText, _languageCode, True)
    End Sub

    Private Sub DisplayInstructionBottom(ByVal instructionText As String)
        If Not String.IsNullOrWhiteSpace(_ucr.Content(instructionText, _languageCode, True)) Then
            plhInstructionBottom.Visible = True
            lblCATInstructionBottom.Visible = True
            lblCATInstructionBottom.Text = _ucr.Content(instructionText, _languageCode, True)
        End If
    End Sub
    Private Sub DisplayYouWillBeTakenOutOfBulkMode(ByVal YouWillBeTakenOutOfBulkMode As String)
        If Not String.IsNullOrWhiteSpace(_ucr.Content(YouWillBeTakenOutOfBulkMode, _languageCode, True)) Then
            plhYouWillBeTakenOutOfBulkMode.Visible = True
            lblYouWillBeTakenOutOfBulkMode.Visible = True
            lblYouWillBeTakenOutOfBulkMode.Text = _ucr.Content(YouWillBeTakenOutOfBulkMode, _languageCode, True)
        End If
    End Sub

    Private Sub ProcessErrorMessage(ByVal isTalentError As Boolean, ByVal errorCode As String, ByVal canDisplayRepeater As Boolean)
        plhCATRepeater.Visible = canDisplayRepeater
        btnYes.Visible = False
        btnNo.Visible = False
        If isTalentError Then
            Dim errMsg As TalentErrorMessages
            errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _
                                                        TalentCache.GetBusinessUnitGroup, _
                                                        TalentCache.GetPartner(Profile), _
                                                        ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
            lblMessageToUser.Text = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                    Talent.eCommerce.Utilities.GetCurrentPageName, _
                                    errorCode).ERROR_MESSAGE()
        Else
            lblMessageToUser.Text = _ucr.Content(errorCode, _languageCode, True)
        End If
    End Sub

    Private Sub SetPackageSummaryCancel()
        plhCATRepeater.Visible = False
        uscPackageSummaryCancel.Display = True
        uscPackageSummaryCancel.PackageId = _packageID
        uscPackageSummaryCancel.ProductCode = _matchCode
        uscPackageSummaryCancel.Mode = PackageSummaryMode.Output
    End Sub

    Private Sub SetPackageSummaryAmendOrTransfer()
        plhCATRepeater.Visible = False
        uscPackageSummaryCancel.Visible = False
        uscPackageSummaryCancel.Display = False
        plhPackageSummaryAmendOrTransfer.Visible = True

        Dim packageBasketPrice As Decimal = 0.0
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If tbi.PACKAGE_ID = _packageID AndAlso tbi.Product = _matchCode Then
                packageBasketPrice = tbi.Gross_Price
                Exit For
            End If
        Next

        Dim package As New Talent.Common.TalentPackage
        'package.RemoveCustomerPackageSession(_packageID, _matchCode)
        Dim err As ErrorObj = package.GetCustomerPackageInformation(TEBUtilities.GetSettingsObject(), Profile.Basket.Basket_Header_ID, _packageID, _matchCode, True, packageBasketPrice)
        If (Not err.HasError) AndAlso package.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") <> "E" Then
            If package.ResultDataSet.Tables("ComponentAmendments") IsNot Nothing AndAlso package.ResultDataSet.Tables("ComponentAmendments").Rows.Count > 0 Then
                rptPackageSummaryAmendOrTransfer.DataSource = package.ResultDataSet.Tables("ComponentAmendments")
                rptPackageSummaryAmendOrTransfer.DataBind()
            End If

        End If

    End Sub

    Private Sub SetPackageSummaryLabels()
        ComponentQuantityAddColHeader = _ucr.Content("ComponentQuantityAddLabel", _languageCode, True)
        ComponentQuantityDelColHeader = _ucr.Content("ComponentQuantityDeleteLabel", _languageCode, True)
        ComponentQuantityAmdColHeader = _ucr.Content("ComponentQuantityAmendLabel", _languageCode, True)
        ComponentColHeader = _ucr.Content("ComponentLabel", _languageCode, True)
        ComponentPriceColHeader = _ucr.Content("ComponentPriceLabel", _languageCode, True)
    End Sub
#End Region

End Class

